<?php
/**
 * POST /api/pago-paycomet.php
 *
 * Proxy server-side de operaciones PayComet (REST API v1).
 * Mantiene el PAYCOMET-API-TOKEN EN EL SERVIDOR — nunca se expone al cliente.
 * El terminal y la API key se leen de qo_configuracion_global.
 *
 * Body JSON:
 * {
 *   "action": "payment" | "bizum" | "refund" | "payment_info" | "card_info" | "card_delete",
 *   "idUser": 12345,            // pagos / tarjetas
 *   "tokenUser": "....",        // pagos / tarjetas
 *   "amount": "1290",           // céntimos (pagos / refund)
 *   "order": "ASA0001234",      // pagos / refund / info
 *   "authCode": "...."          // refund
 * }
 *
 * Devuelve VERBATIM el JSON de PayComet (errorCode, challengeUrl, cardBrand…)
 * para que los modelos del cliente sigan deserializando igual.
 */

include __DIR__ . "/config.php";
include __DIR__ . "/utils.php";

header("Content-Type: application/json; charset=utf-8");

if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    http_response_code(405);
    echo json_encode(['errorCode' => 905, 'error' => 'Method not allowed']);
    exit();
}

$input = json_decode(file_get_contents('php://input'), true);
if (!is_array($input)) {
    http_response_code(400);
    echo json_encode(['errorCode' => 901, 'error' => 'JSON invalido']);
    exit();
}

$action = $input['action'] ?? '';

// ───── Credenciales SOLO en servidor ─────────────────────────────────
$dbConn = connect($db);
$stmt = $dbConn->prepare("SELECT apiPaycomet, terminalPaycomet FROM qo_configuracion_global LIMIT 1");
$stmt->execute();
$cfg = $stmt->fetch(PDO::FETCH_ASSOC);
$apiKey   = $cfg['apiPaycomet'] ?? '';
$terminal = (int)($cfg['terminalPaycomet'] ?? 0);

if ($apiKey === '' || $terminal === 0) {
    http_response_code(500);
    echo json_encode(['errorCode' => 902, 'error' => 'Config PayComet ausente']);
    exit();
}

// IP real del cliente (cardholder) para scoring antifraude / 3DS.
function ip_cliente(): string {
    foreach (['HTTP_X_FORWARDED_FOR', 'HTTP_X_REAL_IP', 'REMOTE_ADDR'] as $h) {
        if (!empty($_SERVER[$h])) {
            $ip = trim(explode(',', $_SERVER[$h])[0]);
            if (filter_var($ip, FILTER_VALIDATE_IP)) return $ip;
        }
    }
    return '0.0.0.0';
}

$base = "https://rest.paycomet.com/v1/";

function paycomet_call(string $url, array $body, string $apiKey): array {
    $ch = curl_init($url);
    curl_setopt_array($ch, [
        CURLOPT_HTTPHEADER     => ["Content-Type: application/json", "PAYCOMET-API-TOKEN: " . $apiKey],
        CURLOPT_RETURNTRANSFER => true,
        CURLOPT_POST           => true,
        CURLOPT_POSTFIELDS     => json_encode($body),
        CURLOPT_SSL_VERIFYPEER => true,
        CURLOPT_TIMEOUT        => 40,
    ]);
    $resp = curl_exec($ch);
    $http = curl_getinfo($ch, CURLINFO_HTTP_CODE);
    $err  = curl_error($ch);
    curl_close($ch);
    return ['http' => $http, 'body' => $resp, 'curlErr' => $err];
}

// ───── Construcción de la petición según acción ──────────────────────
$ip = ip_cliente();

switch ($action) {

    case 'payment':   // tarjeta (methodId 1)
    case 'bizum':     // Bizum (methodId 11)
        $body = ['payment' => [
            'terminal'   => $terminal,
            'amount'     => (string)($input['amount'] ?? '0'),
            'currency'   => 'EUR',
            'idUser'     => (int)($input['idUser'] ?? 0),
            'tokenUser'  => (string)($input['tokenUser'] ?? ''),
            'methodId'   => $action === 'bizum' ? 11 : 1,
            'order'      => (string)($input['order'] ?? ''),
            'originalIp' => $ip,
            'secure'     => 1,
        ]];
        $r = paycomet_call($base . "payments", $body, $apiKey);
        break;

    case 'refund':
        $order = rawurlencode((string)($input['order'] ?? ''));
        $body = ['payment' => [
            'terminal'   => $terminal,
            'amount'     => (string)($input['amount'] ?? '0'),
            'authCode'   => (string)($input['authCode'] ?? ''),
            'currency'   => 'EUR',
            'originalIp' => $ip,
        ]];
        $r = paycomet_call($base . "payments/" . $order . "/refund", $body, $apiKey);
        break;

    case 'payment_info':
        $order = rawurlencode((string)($input['order'] ?? ''));
        $body = ['payment' => ['terminal' => $terminal]];
        $r = paycomet_call($base . "payments/" . $order . "/info", $body, $apiKey);
        break;

    case 'card_info':
        $body = [
            'terminal'  => $terminal,
            'idUser'    => (int)($input['idUser'] ?? 0),
            'tokenUser' => (string)($input['tokenUser'] ?? ''),
        ];
        $r = paycomet_call($base . "cards/info", $body, $apiKey);
        break;

    case 'card_delete':
        $body = [
            'terminal'  => $terminal,
            'idUser'    => (int)($input['idUser'] ?? 0),
            'tokenUser' => (string)($input['tokenUser'] ?? ''),
        ];
        $r = paycomet_call($base . "cards/delete", $body, $apiKey);
        break;

    default:
        http_response_code(400);
        echo json_encode(['errorCode' => 903, 'error' => 'accion desconocida']);
        exit();
}

// Log mínimo sin datos sensibles (solo acción, orden, http, errorCode)
$bodyDecoded = json_decode($r['body'] ?? '', true);
$errCode = is_array($bodyDecoded) ? ($bodyDecoded['errorCode'] ?? '?') : '?';
@file_put_contents(
    "/tmp/paycomet_pago.log",
    date("Y-m-d H:i:s") . " action=$action order=" . ($input['order'] ?? '-') .
    " http=" . $r['http'] . " errorCode=" . $errCode .
    ($r['curlErr'] ? " curlErr=" . $r['curlErr'] : "") . "\n",
    FILE_APPEND
);

if ($r['body'] === false || $r['body'] === null || $r['body'] === '') {
    http_response_code(502);
    echo json_encode(['errorCode' => 904, 'error' => 'Sin respuesta de PayComet', 'detalle' => $r['curlErr']]);
    exit();
}

http_response_code($r['http'] >= 200 && $r['http'] < 300 ? 200 : $r['http']);
echo $r['body']; // verbatim
