<?php
/**
 * Health check de proveedores del agente.
 *
 *  CLI:  php health-check.php          # ejecuta y guarda en Redis
 *  HTTP: GET /api/health-check.php     # devuelve el último estado guardado
 *
 *  Pingaa Vapi (/assistant) y Twilio (/Accounts/.json). Si alguno falla,
 *  manda alerta (agente_alert) si el estado cambia respecto al anterior.
 *
 *  Crontab sugerido (cada minuto):
 *      * * * * *  www-data  /usr/bin/php /var/www/qoorder/pa_ws/api/health-check.php
 */

declare(strict_types=1);

include __DIR__ . "/../config.php";
include __DIR__ . "/../utils.php";
require_once __DIR__ . "/_lib.php";

$esCli = PHP_SAPI === 'cli';

if (!$esCli) {
    // Modo HTTP: solo lee el último estado de Redis
    header('Content-Type: application/json; charset=utf-8');
    $r = agente_redis();
    if (!$r) {
        echo json_encode(['error' => 'redis_no_disponible']);
        exit();
    }
    $cached = $r->get('agente:health');
    echo $cached !== false ? $cached : json_encode(['estado' => 'sin_datos']);
    exit();
}

// ───── Modo CLI: ejecutar checks ─────────────────────────────────────
$apiVapi  = agente_env('VAPI_API_KEY', '');
$twAcct   = agente_env('TWILIO_ACCOUNT_SID', '');
$twTok    = agente_env('TWILIO_AUTH_TOKEN', '');
$ts       = date('c');
$result   = ['timestamp' => $ts, 'checks' => []];

// Check Vapi
$start = microtime(true);
$ch = curl_init('https://api.vapi.ai/assistant');
curl_setopt($ch, CURLOPT_HTTPHEADER, ['Authorization: Bearer ' . $apiVapi]);
curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
curl_setopt($ch, CURLOPT_TIMEOUT, 8);
curl_setopt($ch, CURLOPT_CONNECTTIMEOUT, 4);
curl_exec($ch);
$httpVapi = (int)curl_getinfo($ch, CURLINFO_HTTP_CODE);
$latVapi  = (int)round((microtime(true) - $start) * 1000);
$result['checks']['vapi'] = [
    'ok'         => $httpVapi >= 200 && $httpVapi < 300,
    'http'       => $httpVapi,
    'latency_ms' => $latVapi
];

// Check Twilio
$start = microtime(true);
$ch = curl_init("https://api.twilio.com/2010-04-01/Accounts/{$twAcct}.json");
curl_setopt($ch, CURLOPT_USERPWD, "{$twAcct}:{$twTok}");
curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
curl_setopt($ch, CURLOPT_TIMEOUT, 8);
curl_setopt($ch, CURLOPT_CONNECTTIMEOUT, 4);
curl_exec($ch);
$httpTw = (int)curl_getinfo($ch, CURLINFO_HTTP_CODE);
$latTw  = (int)round((microtime(true) - $start) * 1000);
$result['checks']['twilio'] = [
    'ok'         => $httpTw >= 200 && $httpTw < 300,
    'http'       => $httpTw,
    'latency_ms' => $latTw
];

// Check BD propia
$start = microtime(true);
try {
    $db_ok = false;
    $dbConn = connect($db);
    $row = $dbConn->query("SELECT 1")->fetchColumn();
    $db_ok = ($row == 1);
} catch (\Throwable $e) {
    $db_ok = false;
}
$result['checks']['db'] = [
    'ok'         => $db_ok,
    'latency_ms' => (int)round((microtime(true) - $start) * 1000)
];

// Estado global
$todoOk = $result['checks']['vapi']['ok']
       && $result['checks']['twilio']['ok']
       && $result['checks']['db']['ok'];
$result['estado'] = $todoOk ? 'ok' : 'degradado';

// Guardar en Redis (TTL 3 min para que la última lectura nunca quede "fantasma")
$r = agente_redis();
if ($r) {
    $r->setex('agente:health', 180, json_encode($result));

    // Comparar con el estado anterior para alertar solo en transiciones
    $previo = $r->get('agente:health:estado-previo');
    if ($previo !== false && $previo !== $result['estado']) {
        $cambioBuenoOMalo = $result['estado'] === 'degradado' ? 'caída' : 'recuperación';
        $detalle = "Vapi http={$httpVapi} ({$latVapi}ms) | Twilio http={$httpTw} ({$latTw}ms) | DB " . ($db_ok ? 'OK' : 'KO');
        agente_alert(
            "Health {$cambioBuenoOMalo}: {$result['estado']}",
            "Estado cambió de '{$previo}' a '{$result['estado']}'.\n\n{$detalle}",
            $result['checks']
        );
    }
    $r->setex('agente:health:estado-previo', 3600, $result['estado']);
}

echo json_encode($result, JSON_PRETTY_PRINT) . PHP_EOL;
exit($todoOk ? 0 : 1);
