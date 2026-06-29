<?php
/**
 * POST /api/crear-pedido.php
 *
 * Endpoint dedicado al agente de voz para crear pedidos.
 * No toca el pedidos.php existente para evitar regresiones.
 *
 * Inserta en qo_pedidos con origen='voz' + llamada_id, y los detalles
 * en qo_pedidos_detalle. Vincula la llamada en qo_llamadas.
 *
 * Body JSON esperado:
 * {
 *   "idEstablecimiento": 67,
 *   "idUsuario": 35103,                   // null si no registrado
 *   "nombreUsuario": "Juan Pérez",
 *   "telefono": "+34600123456",
 *   "vapi_call_id": "vapi-xxx",           // opcional, para vincular llamada
 *   "tipoVenta": "Envío" | "Recogida",
 *   "direccion": "Calle X 5",             // si Envío
 *   "idZona": 1,
 *   "horaEntrega": "2026-05-11 21:00:00", // hora prevista
 *   "tipoPago": "Efectivo" | "Datafono" | "Tarjeta",
 *   "comentario": "Sin sal",
 *   "lineas": [
 *      { "idProducto": 123, "cantidad": 1, "precio": 12.5, "concepto": "Pollo asado entero", "comentario": "" },
 *      { "idProducto": 124, "cantidad": 2, "precio":  3.0, "concepto": "Patatas fritas", "comentario": "" }
 *   ]
 * }
 *
 * Respuesta:
 * { "id": 12345, "codigo": "ASA00012345", "total": 18.50 }
 */

include __DIR__ . "/../config.php";
include __DIR__ . "/../utils.php";
require_once __DIR__ . "/_lib.php";

header("Content-Type: application/json; charset=utf-8");

if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    http_response_code(405);
    echo json_encode(['error' => 'Method not allowed']);
    exit();
}

// Rate-limit por IP: 20 pedidos/minuto
$ip = $_SERVER['REMOTE_ADDR'] ?? 'unknown';
$rl = agente_rate_limit("crear-pedido:ip:$ip", 20, 60);
if (!$rl['permitido']) {
    http_response_code(429);
    header('Retry-After: ' . $rl['reset']);
    agente_log('warn', 'rate_limit_ip', ['ip' => $ip, 'endpoint' => 'crear-pedido']);
    echo json_encode(['error' => 'rate_limit_ip', 'reset' => $rl['reset']]);
    exit();
}

$input = json_decode(file_get_contents('php://input'), true);
if (!is_array($input)) {
    http_response_code(400);
    echo json_encode(['error' => 'JSON invalido']);
    exit();
}

// ───── Validación de campos obligatorios ─────────────────────────────
$idEst        = intval($input['idEstablecimiento'] ?? 0);
$idUsuario    = isset($input['idUsuario']) && $input['idUsuario'] !== null ? intval($input['idUsuario']) : 0;
// Fallback: usuario sistema "Agente Voz" (id 38134) si no se identificó al cliente.
// Necesario porque pedidos.php hace INNER JOIN con qo_users.
if ($idUsuario <= 0) $idUsuario = 38134;
$nombreUser   = trim($input['nombreUsuario'] ?? 'Cliente voz');
$telefono     = trim($input['telefono'] ?? '');
$vapiCallId   = trim($input['vapi_call_id'] ?? '');
$tipoVenta    = trim($input['tipoVenta'] ?? 'Envío');
$direccion    = trim($input['direccion'] ?? '');
$idZona       = intval($input['idZona'] ?? 1);
$horaEntrega  = trim($input['horaEntrega'] ?? '');
$tipoPago     = trim($input['tipoPago'] ?? 'Efectivo');
$comentario   = trim($input['comentario'] ?? '');
$lineas       = $input['lineas'] ?? [];

if ($idEst <= 0) {
    http_response_code(400);
    echo json_encode(['error' => 'idEstablecimiento requerido']);
    exit();
}
if (!is_array($lineas) || count($lineas) === 0) {
    http_response_code(400);
    echo json_encode(['error' => 'lineas vacias']);
    exit();
}
if (!in_array($tipoVenta, ['Envío', 'Recogida', 'Envio'])) {
    http_response_code(400);
    echo json_encode(['error' => 'tipoVenta invalido']);
    exit();
}
if ($tipoVenta === 'Envío' && $direccion === '') {
    http_response_code(400);
    echo json_encode(['error' => 'direccion requerida para envio']);
    exit();
}
// Normalizar horaEntrega: aceptar "HH:MM", "YYYY-MM-DD HH:MM" o ya completo.
date_default_timezone_set('Europe/Madrid');
if ($horaEntrega === '') {
    $horaEntrega = date('Y-m-d H:i:s', strtotime('+30 minutes'));
} elseif (preg_match('/^\d{1,2}:\d{2}$/', $horaEntrega)) {
    $hoy = date('Y-m-d');
    $ts = strtotime("$hoy $horaEntrega:00");
    if ($ts !== false && $ts < time()) $ts = strtotime("+1 day", $ts);
    $horaEntrega = date('Y-m-d H:i:s', $ts ?: strtotime('+30 minutes'));
} elseif (preg_match('/^\d{4}-\d{2}-\d{2} \d{1,2}:\d{2}$/', $horaEntrega)) {
    $horaEntrega .= ':00';
} elseif (!preg_match('/^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}$/', $horaEntrega)) {
    $ts = strtotime($horaEntrega);
    $horaEntrega = $ts ? date('Y-m-d H:i:s', $ts) : date('Y-m-d H:i:s', strtotime('+30 minutes'));
}

// ───── Control de importe máximo sin humano ──────────────────────────
$total = 0.0;
foreach ($lineas as $l) {
    $total += floatval($l['precio'] ?? 0) * intval($l['cantidad'] ?? 0);
}

$dbConn = connect($db);

// Obtener umbral
$stmt = $dbConn->prepare("SELECT valor FROM qo_config_agente WHERE clave = 'max_importe_sin_humano_eur'");
$stmt->execute();
$umbralImporte = (float)(json_decode($stmt->fetchColumn(), true) ?: 100);
if ($total > $umbralImporte) {
    http_response_code(409); // Conflict — el agente debe transferir
    echo json_encode([
        'error'            => 'importe_supera_umbral',
        'total'            => $total,
        'umbral'           => $umbralImporte,
        'requiere_humano'  => true
    ]);
    exit();
}

// ───── Control de blacklist ──────────────────────────────────────────
if ($telefono !== '') {
    $telefonoNorm = preg_replace('/[^0-9+]/', '', $telefono);
    $stmt = $dbConn->prepare("SELECT motivo FROM qo_blacklist_telefonos WHERE telefono = :tel");
    $stmt->bindValue(':tel', $telefonoNorm);
    $stmt->execute();
    $motivo = $stmt->fetchColumn();
    if ($motivo !== false) {
        http_response_code(403);
        echo json_encode([
            'error'  => 'telefono_bloqueado',
            'motivo' => $motivo
        ]);
        exit();
    }
}

// ───── Rate-limit por número (anti-abuso) ────────────────────────────
if ($telefono !== '') {
    $telefonoNorm = preg_replace('/[^0-9+]/', '', $telefono);
    $stmt = $dbConn->prepare("SELECT valor FROM qo_config_agente WHERE clave = 'max_llamadas_hora_por_numero'");
    $stmt->execute();
    $maxHora = (int)(json_decode($stmt->fetchColumn(), true) ?: 3);

    $stmt = $dbConn->prepare("
        SELECT COUNT(*) FROM qo_pedidos p
        JOIN qo_llamadas l ON l.pedido_id = p.id
        WHERE l.telefono_origen = :tel
          AND p.horaPedido >= DATE_SUB(NOW(), INTERVAL 1 HOUR)
          AND p.origen = 'voz'
    ");
    $stmt->bindValue(':tel', $telefonoNorm);
    $stmt->execute();
    $pedidosUltimaHora = (int)$stmt->fetchColumn();
    if ($pedidosUltimaHora >= $maxHora) {
        http_response_code(429);
        echo json_encode([
            'error'                => 'rate_limit',
            'max_por_hora'         => $maxHora,
            'pedidos_ultima_hora'  => $pedidosUltimaHora
        ]);
        exit();
    }
}

// ───── Obtener llamada_id si viene vapi_call_id ──────────────────────
$llamadaId = null;
if ($vapiCallId !== '') {
    $stmt = $dbConn->prepare("SELECT id FROM qo_llamadas WHERE vapi_call_id = :vid");
    $stmt->bindValue(':vid', $vapiCallId);
    $stmt->execute();
    $llamadaId = $stmt->fetchColumn() ?: null;
}

// ───── Generar código de pedido (ASA + 7 dígitos = 10 chars, cabe en VARCHAR(10)) ─
function generarCodigoPedido(PDO $db): string {
    $intentos = 0;
    do {
        $codigo = 'ASA' . str_pad((string)random_int(1, 9999999), 7, '0', STR_PAD_LEFT);
        $stmt = $db->prepare("SELECT COUNT(*) FROM qo_pedidos WHERE codigo = :c");
        $stmt->bindValue(':c', $codigo);
        $stmt->execute();
        if ((int)$stmt->fetchColumn() === 0) return $codigo;
    } while (++$intentos < 5);
    throw new RuntimeException('No se pudo generar codigo unico');
}

// ───── Transacción: pedido + detalles + estado inicial ───────────────
try {
    $dbConn->beginTransaction();

    $codigoPedido = generarCodigoPedido($dbConn);

    // Cabecera
    $stmt = $dbConn->prepare("
        INSERT INTO qo_pedidos
            (origen, llamada_id, codigo, idEstablecimiento, horaPedido, idUsuario,
             estado, idZona, nuevoPedido, direccion, comentario, horaEntrega,
             tipo, tipoVenta, tipoPago, nombreUsuario, valorado, completo, anulado, recoger)
        VALUES
            ('voz', :llid, :cod, :est, NOW(), :usr,
             1, :zona, 1, :dir, :com, :hent,
             1, :tipov, :tipop, :nomu, 1, 0, 0, :recoger)
    ");
    $stmt->bindValue(':llid',    $llamadaId, $llamadaId === null ? PDO::PARAM_NULL : PDO::PARAM_INT);
    $stmt->bindValue(':cod',     $codigoPedido);
    $stmt->bindValue(':est',     $idEst,     PDO::PARAM_INT);
    $stmt->bindValue(':usr',     $idUsuario, PDO::PARAM_INT);
    $stmt->bindValue(':zona',    $idZona,    PDO::PARAM_INT);
    $stmt->bindValue(':dir',     $direccion);
    $stmt->bindValue(':com',     $comentario);
    $stmt->bindValue(':hent',    $horaEntrega);
    $stmt->bindValue(':tipov',   $tipoVenta === 'Envio' ? 'Envío' : $tipoVenta);
    $stmt->bindValue(':tipop',   $tipoPago);
    $stmt->bindValue(':nomu',    $nombreUser);
    $stmt->bindValue(':recoger', $tipoVenta === 'Recogida' ? 1 : 0, PDO::PARAM_INT);
    $stmt->execute();

    $pedidoId = (int)$dbConn->lastInsertId();

    // Detalles
    $stmtDet = $dbConn->prepare("
        INSERT INTO qo_pedidos_detalle
            (idPedido, idProducto, precio, cantidad, tipo, concepto, comentario, tipoVenta, pagadoConPuntos)
        VALUES
            (:pid, :prod, :precio, :cant, :tipo, :concepto, :coment, :tipov, :puntos)
    ");
    foreach ($lineas as $l) {
        $stmtDet->bindValue(':pid',      $pedidoId, PDO::PARAM_INT);
        $stmtDet->bindValue(':prod',     intval($l['idProducto'] ?? 0), PDO::PARAM_INT);
        $stmtDet->bindValue(':precio',   floatval($l['precio'] ?? 0));
        $stmtDet->bindValue(':cant',     intval($l['cantidad'] ?? 1), PDO::PARAM_INT);
        $stmtDet->bindValue(':tipo',     intval($l['tipo'] ?? 0), PDO::PARAM_INT);
        $stmtDet->bindValue(':concepto', $l['concepto'] ?? '');
        $stmtDet->bindValue(':coment',   $l['comentario'] ?? '');
        $stmtDet->bindValue(':tipov',    $tipoVenta === 'Envio' ? 'Envío' : $tipoVenta);
        $stmtDet->bindValue(':puntos',   intval($l['pagadoConPuntos'] ?? 0), PDO::PARAM_INT);
        $stmtDet->execute();
    }

    // Estado inicial
    $stmt = $dbConn->prepare("
        INSERT INTO qo_pedidos_estado (estado, idUsuario, fecha, idPedido)
        VALUES (1, :usr, NOW(), :pid)
    ");
    $stmt->bindValue(':usr', $idUsuario, PDO::PARAM_INT);
    $stmt->bindValue(':pid', $pedidoId, PDO::PARAM_INT);
    $stmt->execute();

    // Vincular llamada con el pedido (si había llamada)
    if ($llamadaId !== null) {
        $stmt = $dbConn->prepare("UPDATE qo_llamadas SET pedido_id = :pid WHERE id = :lid");
        $stmt->bindValue(':pid', $pedidoId,  PDO::PARAM_INT);
        $stmt->bindValue(':lid', $llamadaId, PDO::PARAM_INT);
        $stmt->execute();
    }

    $dbConn->commit();

    agente_log('info', 'pedido_creado', [
        'pedido_id'    => $pedidoId,
        'codigo'       => $codigoPedido,
        'llamada_id'   => $llamadaId,
        'vapi_call_id' => $vapiCallId,
        'total'        => round($total, 2),
        'idEst'        => $idEst
    ]);

    // Push al staff
    $tokens = agente_tokens_staff($dbConn, $idEst);
    if (count($tokens) > 0) {
        $resumen = '';
        foreach ($lineas as $l) {
            $cant = (int)($l['cantidad'] ?? 1);
            $conc = (string)($l['concepto'] ?? '');
            $resumen .= ($resumen === '' ? '' : ', ') . "{$cant}× {$conc}";
        }
        $mensaje = "📞 Pedido voz #{$codigoPedido} · " . number_format($total, 2) . "€ · {$tipoVenta}"
                 . ($resumen !== '' ? "\n{$resumen}" : '');
        agente_push(
            $tokens,
            'Asador Morón',
            $mensaje,
            [
                'tipo'      => 'pedido_voz',
                'pedido_id' => $pedidoId,
                'codigo'    => $codigoPedido,
                'tipoVenta' => $tipoVenta
            ]
        );
    }

    http_response_code(200);
    echo json_encode([
        'id'             => $pedidoId,
        'codigo'         => $codigoPedido,
        'total'          => round($total, 2),
        'horaEntrega'    => $horaEntrega,
        'origen'         => 'voz',
        'llamada_id'     => $llamadaId
    ]);

} catch (Exception $e) {
    $dbConn->rollBack();
    agente_log('error', 'pedido_fallido', [
        'vapi_call_id' => $vapiCallId,
        'err'          => $e->getMessage()
    ]);
    http_response_code(500);
    echo json_encode(['error' => 'Internal error', 'detalle' => $e->getMessage()]);
}
