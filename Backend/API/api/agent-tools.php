<?php
/**
 * POST /api/agent-tools.php
 *
 * Dispatcher único de tool-calls de Vapi. Vapi envía:
 *   { "message": { "type": "tool-calls", "toolCalls": [ {id, function: {name, arguments}} ] } }
 *
 * Y espera:
 *   { "results": [ {toolCallId, result: "<json string or text>"} ] }
 *
 * Cada tool delega en endpoints PHP existentes (cliente, estado-local, slots-recogida,
 * validar-zona, crear-pedido) reutilizando la conexión PDO interna.
 *
 * idEstablecimiento del Asador Morón = 67. idPueblo = 1. Hardcoded de momento
 * porque este assistant es específico de ese local; se puede parametrizar
 * cuando demos servicio a varios establecimientos.
 */

declare(strict_types=1);

include __DIR__ . "/../config.php";
include __DIR__ . "/../utils.php";
require_once __DIR__ . "/_lib.php";

header("Content-Type: application/json; charset=utf-8");

if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    http_response_code(405);
    echo json_encode(['error' => 'Method not allowed']);
    exit();
}

// ───── Configuración fija del local ──────────────────────────────────
const ID_ESTABLECIMIENTO = 67;
const ID_PUEBLO          = 1;
// Usuario sistema "Agente Voz" para pedidos sin cliente identificado.
// Asegura que el INNER JOIN qo_users de pedidos.php no excluya el pedido.
const ID_USUARIO_AGENTE  = 38134;

// ───── Validar firma HMAC de Vapi (reutiliza el secret del webhook) ──
$rawBody = file_get_contents('php://input');

// Validación de seguridad: Vapi envía el secret como header "X-Vapi-Secret" en plain text
// (NO HMAC). Comprobamos que coincida con el secret configurado en cada tool.
// Más info: https://docs.vapi.ai/server-url/setting-server-urls#authentication
$secret  = agente_env('VAPI_WEBHOOK_SECRET', '');
if ($secret !== '') {
    $headerSecret = $_SERVER['HTTP_X_VAPI_SECRET'] ?? '';
    if ($headerSecret === '' || !hash_equals($secret, $headerSecret)) {
        // Log para diagnóstico (cabeceras que sí llegaron)
        $vapiHeaders = [];
        foreach ($_SERVER as $k => $v) {
            if (strpos($k, 'HTTP_X_VAPI') === 0) $vapiHeaders[$k] = is_string($v) ? substr($v, 0, 8) . '…' : '';
        }
        agente_log('warn', 'tools_auth_invalido', ['headers' => $vapiHeaders]);
        http_response_code(401);
        echo json_encode(['error' => 'Invalid signature']);
        exit();
    }
}

$payload = json_decode($rawBody, true);
if (!is_array($payload)) {
    http_response_code(400);
    echo json_encode(['error' => 'JSON invalido']);
    exit();
}

$message   = $payload['message'] ?? $payload;
$toolCalls = $message['toolCalls'] ?? [];
$callMeta  = $message['call']      ?? [];
$callId    = $callMeta['id']       ?? '';
$telefono  = $callMeta['customer']['number'] ?? null;

if (count($toolCalls) === 0) {
    http_response_code(400);
    echo json_encode(['error' => 'toolCalls vacío']);
    exit();
}

$dbConn  = connect($db);
$results = [];

foreach ($toolCalls as $tc) {
    $toolCallId = $tc['id'] ?? '';
    $name       = $tc['function']['name'] ?? '';
    $argsRaw    = $tc['function']['arguments'] ?? '{}';
    $args       = is_array($argsRaw) ? $argsRaw : (json_decode($argsRaw, true) ?: []);

    try {
        $result = dispatch_tool($name, $args, $callId, $telefono, $dbConn);
    } catch (\Throwable $e) {
        agente_log('error', 'tool_excepcion', [
            'tool'   => $name,
            'call_id'=> $callId,
            'err'    => $e->getMessage()
        ]);
        $result = ['error' => 'tool_error', 'message' => $e->getMessage()];
    }

    agente_log('info', 'tool_invocada', [
        'tool'        => $name,
        'call_id'     => $callId,
        'tool_call_id'=> $toolCallId,
        'args_keys'   => array_keys($args),
        'ok'          => !isset($result['error'])
    ]);

    $results[] = [
        'toolCallId' => $toolCallId,
        'result'     => is_string($result) ? $result : json_encode($result, JSON_UNESCAPED_UNICODE)
    ];
}

http_response_code(200);
echo json_encode(['results' => $results], JSON_UNESCAPED_UNICODE);

// ═════════════════════════════════════════════════════════════════════
// IMPLEMENTACIÓN DE LAS TOOLS
// ═════════════════════════════════════════════════════════════════════

function dispatch_tool(string $name, array $args, string $callId, ?string $telefonoLlamada, PDO $db): array {
    switch ($name) {

        case 'get_cliente':
            $tel = $args['telefono'] ?? $telefonoLlamada ?? '';
            return tool_get_cliente($db, (string)$tel);

        case 'get_menu':
            $buscar = (string)($args['buscar'] ?? '');
            return tool_get_menu($db, $buscar);

        case 'get_opciones':
            return tool_get_opciones($db, (int)($args['idProducto'] ?? 0));

        case 'get_ingredientes':
            return tool_get_ingredientes($db, (int)($args['idProducto'] ?? 0));

        case 'get_estado_local':
            return tool_get_estado_local($db);

        case 'get_slots_recogida':
            $cantidad = (int)($args['cantidad'] ?? 6);
            return tool_get_slots_recogida($db, $cantidad);

        case 'validar_zona':
            $direccion = (string)($args['direccion'] ?? '');
            return tool_validar_zona($db, $direccion);

        case 'crear_pedido':
            return tool_crear_pedido($db, $args, $callId, $telefonoLlamada);

        case 'resumen_pedido':
            return tool_resumen_pedido($db, $args);

        default:
            return ['error' => 'tool_desconocida', 'name' => $name];
    }
}

// ─── get_cliente ─────────────────────────────────────────────────────
// ─── get_menu: catálogo del establecimiento ───────────────────────────
// Si se pasa `buscar` filtra por nombre/categoría (LIKE) para reducir resultados.
// Si no, devuelve los primeros 40 productos (resumen).
function tool_get_menu(PDO $db, string $buscar = ''): array {
    $buscar = trim($buscar);

    if ($buscar !== '') {
        // Búsqueda fuzzy: divide en palabras y exige que TODAS aparezcan en nombre o categoría
        $palabras = preg_split('/\s+/', mb_strtolower($buscar, 'UTF-8'));
        $where = ["pc.idEstablecimiento = :est", "pe.estado = 1", "pe.eliminado = 0", "pe.precio > 0"];
        $params = [':est' => ID_ESTABLECIMIENTO];
        foreach ($palabras as $i => $p) {
            $key = ":p{$i}";
            $where[] = "(LOWER(pe.nombre) LIKE {$key} OR LOWER(pc.nombre) LIKE {$key})";
            $params[$key] = '%' . $p . '%';
        }
        $sql = "SELECT pe.id, pe.nombre, pe.precio, pc.nombre AS categoria,
                       EXISTS(SELECT 1 FROM qo_productos_opc o WHERE o.idProducto = pe.id) AS tieneOpciones,
                       EXISTS(SELECT 1 FROM qo_ingredientes_producto ip WHERE ip.idProducto = pe.id) AS tieneIngredientes,
                       pe.numeroIngredientes AS numIng
                FROM qo_productos_est pe
                JOIN qo_productos_cat pc ON pc.id = pe.idCategoria
                WHERE " . implode(' AND ', $where) . "
                ORDER BY pe.precio ASC
                LIMIT 25";
        $stmt = $db->prepare($sql);
        foreach ($params as $k => $v) {
            $stmt->bindValue($k, $v, $k === ':est' ? PDO::PARAM_INT : PDO::PARAM_STR);
        }
        $stmt->execute();
        $rows = $stmt->fetchAll(PDO::FETCH_ASSOC);
    } else {
        // Sin filtro: top productos por presencia
        $stmt = $db->prepare("
            SELECT pe.id, pe.nombre, pe.precio, pc.nombre AS categoria,
                   EXISTS(SELECT 1 FROM qo_productos_opc o WHERE o.idProducto = pe.id) AS tieneOpciones,
                   EXISTS(SELECT 1 FROM qo_ingredientes_producto ip WHERE ip.idProducto = pe.id) AS tieneIngredientes,
                   pe.numeroIngredientes AS numIng
            FROM qo_productos_est pe
            JOIN qo_productos_cat pc ON pc.id = pe.idCategoria
            WHERE pc.idEstablecimiento = :est
              AND pe.estado = 1 AND pe.eliminado = 0 AND pe.precio > 0
            ORDER BY pc.orden, pe.nombre
            LIMIT 40
        ");
        $stmt->bindValue(':est', ID_ESTABLECIMIENTO, PDO::PARAM_INT);
        $stmt->execute();
        $rows = $stmt->fetchAll(PDO::FETCH_ASSOC);
    }

    $productos = array_map(fn($r) => [
        'id'   => (int)$r['id'],
        'n'    => trim($r['nombre']),
        'p'    => (float)$r['precio'],
        'c'    => trim($r['categoria']),
        // Flags de personalización: el agente sabe si debe llamar a get_opciones /
        // get_ingredientes tras elegir este producto (P2).
        'opc'  => (int)($r['tieneOpciones'] ?? 0) === 1,
        'ing'  => (int)($r['tieneIngredientes'] ?? 0) === 1,
        'nIng' => (int)($r['numIng'] ?? 0),
    ], $rows);

    return [
        'productos' => $productos,
        'total'     => count($productos),
        'buscar'    => $buscar
    ];
}

// ─── get_opciones: opciones de un producto (entero/medio/cuarto, menús…) ──
// valorIncremento es el PRECIO ABSOLUTO de la opción (no un incremento): al
// elegir una opción, el precio de la línea = ese valor. Semántica idéntica a
// la app móvil (OpcionSeleccionada.precio).
function tool_get_opciones(PDO $db, int $idProducto): array {
    if ($idProducto <= 0) return ['error' => 'idProducto_requerido'];
    $stmt = $db->prepare("
        SELECT id AS idOpcion, opcion, valorIncremento AS precio
        FROM qo_productos_opc
        WHERE idProducto = :id
        ORDER BY valorIncremento ASC, id ASC
    ");
    $stmt->bindValue(':id', $idProducto, PDO::PARAM_INT);
    $stmt->execute();
    $rows = $stmt->fetchAll(PDO::FETCH_ASSOC);

    $opciones = array_map(fn($r) => [
        'idOpcion' => (int)$r['idOpcion'],
        'opcion'   => trim($r['opcion']),
        'precio'   => (float)$r['precio'],   // ABSOLUTO
    ], $rows);

    return [
        'idProducto' => $idProducto,
        'opciones'   => $opciones,
        'total'      => count($opciones),
    ];
}

// ─── get_ingredientes: ingredientes configurables de un producto ──────────
// Nombre en qo_ingredientes_establecimiento; precio incremental (a SUMAR al
// añadir) en qo_ingredientes_producto.precio. Mismo origen que la app.
function tool_get_ingredientes(PDO $db, int $idProducto): array {
    if ($idProducto <= 0) return ['error' => 'idProducto_requerido'];
    $stmt = $db->prepare("
        SELECT ie.id AS idIngrediente, ie.nombre, ip.precio
        FROM qo_ingredientes_producto ip
        JOIN qo_ingredientes_establecimiento ie ON ie.id = ip.idIngrediente AND ie.estado = 1
        WHERE ip.idProducto = :id
        ORDER BY ie.nombre ASC
    ");
    $stmt->bindValue(':id', $idProducto, PDO::PARAM_INT);
    $stmt->execute();
    $rows = $stmt->fetchAll(PDO::FETCH_ASSOC);

    $ingredientes = array_map(fn($r) => [
        'idIngrediente' => (int)$r['idIngrediente'],
        'nombre'        => trim($r['nombre']),
        'precio'        => (float)$r['precio'],  // incremental al añadir
    ], $rows);

    return [
        'idProducto'    => $idProducto,
        'ingredientes'  => $ingredientes,
        'total'         => count($ingredientes),
    ];
}

function tool_get_cliente(PDO $db, string $telefono): array {
    if ($telefono === '') return ['encontrado' => false, 'motivo' => 'telefono_vacio'];

    $tel = preg_replace('/[^0-9+]/', '', $telefono);
    $local = preg_replace('/^\+?34/', '', $tel);

    $sql = $db->prepare("
        SELECT u.id AS idUsuario, u.nombre, u.apellidos, u.direccion, u.cod_postal,
               u.poblacion, u.telefono, u.bloqueado,
               (SELECT COUNT(*) FROM qo_pedidos p WHERE p.idUsuario = u.id) AS numeroPedidos,
               (SELECT MAX(p.horaPedido) FROM qo_pedidos p WHERE p.idUsuario = u.id) AS ultimoPedido
        FROM qo_users u
        WHERE u.idPueblo = :pueblo
          AND (u.telefono = :full OR u.telefono = :local OR u.telefono LIKE :suf)
        ORDER BY u.id DESC LIMIT 1
    ");
    $sql->bindValue(':pueblo', ID_PUEBLO, PDO::PARAM_INT);
    $sql->bindValue(':full',   $tel);
    $sql->bindValue(':local',  $local);
    $sql->bindValue(':suf',    '%' . substr($local, -9));
    $sql->execute();
    $row = $sql->fetch(PDO::FETCH_ASSOC);

    if (!$row) return ['encontrado' => false, 'telefono' => $tel];

    return [
        'encontrado'    => true,
        'idUsuario'     => (int)$row['idUsuario'],
        'nombre'        => $row['nombre'],
        'apellidos'     => $row['apellidos'],
        'direccion'     => $row['direccion'],
        'poblacion'     => $row['poblacion'],
        'numeroPedidos' => (int)$row['numeroPedidos'],
        'ultimoPedido'  => $row['ultimoPedido'],
        'bloqueado'     => (int)$row['bloqueado'] === 1
    ];
}

// ─── get_estado_local ────────────────────────────────────────────────
function tool_get_estado_local(PDO $db): array {
    // Carga actual de cocina
    $sql = $db->prepare("SELECT COALESCE(cantidad,0) FROM qo_contador_pollos WHERE idEstablecimiento=:id AND fecha=CURDATE() LIMIT 1");
    $sql->bindValue(':id', ID_ESTABLECIMIENTO, PDO::PARAM_INT);
    $sql->execute();
    $pollos = (int)($sql->fetchColumn() ?: 0);

    // Configuración
    $umbralRow = $db->query("SELECT valor FROM qo_config_agente WHERE clave='modo_saturacion_umbral'")->fetchColumn();
    $umbral    = (int)(json_decode((string)$umbralRow, true) ?: 10);
    $horarioRow= $db->query("SELECT valor FROM qo_config_agente WHERE clave='horario'")->fetchColumn();
    $horario   = json_decode((string)$horarioRow, true);

    date_default_timezone_set('Europe/Madrid');
    $ahora     = new DateTime('now');
    $dia       = ['domingo','lunes','martes','miercoles','jueves','viernes','sabado'][(int)$ahora->format('w')];
    $horaActual= $ahora->format('H:i');

    $abierto = false;
    $proxima = null;
    if ($horario && isset($horario[$dia])) {
        $franjas = $horario[$dia];
        for ($i = 0; $i < count($franjas); $i += 2) {
            $ap = $franjas[$i] ?? null;
            $ci = $franjas[$i+1] ?? null;
            if ($ap && $ci && $horaActual >= $ap && $horaActual < $ci) { $abierto = true; break; }
            if ($ap && $horaActual < $ap && $proxima === null) $proxima = $ahora->format('Y-m-d') . ' ' . $ap;
        }
    }
    $saturado = $pollos >= $umbral;

    return [
        'abierto'         => $abierto,
        'saturado'        => $saturado,
        'cargaCocina'     => $pollos,
        'umbralSaturacion'=> $umbral,
        'proxima_apertura'=> $proxima,
        'motivo'          => !$abierto ? 'fuera_horario' : ($saturado ? 'saturado' : 'ok')
    ];
}

// ─── get_slots_recogida ──────────────────────────────────────────────
function tool_get_slots_recogida(PDO $db, int $cantidad): array {
    $cantidad = max(1, min(12, $cantidad));

    $cfg = [];
    foreach ($db->query("SELECT clave, valor FROM qo_config_agente WHERE clave IN
        ('tiempo_preparacion_base_minutos','tiempo_extra_por_pollo','buffer_seguridad_minutos','modo_saturacion_umbral')
    ")->fetchAll(PDO::FETCH_ASSOC) as $r) {
        $cfg[$r['clave']] = json_decode($r['valor'], true);
    }
    $base   = (int)($cfg['tiempo_preparacion_base_minutos'] ?? 20);
    $extra  = (int)($cfg['tiempo_extra_por_pollo']          ?? 5);
    $buffer = (int)($cfg['buffer_seguridad_minutos']        ?? 5);
    $capac  = (int)($cfg['modo_saturacion_umbral']          ?? 10);

    $sql = $db->prepare("SELECT COALESCE(cantidad,0) FROM qo_contador_pollos WHERE idEstablecimiento=:id AND fecha=CURDATE() LIMIT 1");
    $sql->bindValue(':id', ID_ESTABLECIMIENTO, PDO::PARAM_INT);
    $sql->execute();
    $pollos = (int)($sql->fetchColumn() ?: 0);

    date_default_timezone_set('Europe/Madrid');
    $arranque = (new DateTime('now'))->modify('+' . ($base + $buffer + $pollos * $extra) . ' minutes');
    $m = (int)$arranque->format('i');
    if ($m % 15 !== 0) $arranque->modify('+' . (15 - ($m % 15)) . ' minutes');
    $arranque->setTime((int)$arranque->format('H'), (int)$arranque->format('i'), 0);

    $slots = [];
    for ($i = 0; $i < $cantidad; $i++) {
        $h = (clone $arranque)->modify('+' . ($i * 15) . ' minutes');
        $slots[] = [
            'hora'      => $h->format('H:i'),
            'fecha'     => $h->format('Y-m-d'),
            'disponible'=> true,
            'capacidad' => $capac
        ];
    }
    return ['cargaActual' => $pollos, 'slots' => $slots];
}

// ─── validar_zona ────────────────────────────────────────────────────
function tool_validar_zona(PDO $db, string $direccion): array {
    if (trim($direccion) === '') return ['valida' => false, 'motivo' => 'direccion_vacia'];

    $sql = $db->prepare("SELECT id, nombre, gastos, pedidoMinimo FROM qo_zonas WHERE idPueblo=:p AND activo=1 ORDER BY id");
    $sql->bindValue(':p', ID_PUEBLO, PDO::PARAM_INT);
    $sql->execute();
    $zonas = $sql->fetchAll(PDO::FETCH_ASSOC);

    if (count($zonas) === 0) return ['valida' => false, 'motivo' => 'sin_zonas'];

    $dirLower = mb_strtolower($direccion, 'UTF-8');
    foreach ($zonas as $z) {
        $n = mb_strtolower($z['nombre'], 'UTF-8');
        if ($n !== '' && mb_strpos($dirLower, $n) !== false) {
            return [
                'valida'      => true,
                'idZona'      => (int)$z['id'],
                'nombre'      => $z['nombre'],
                'gastos'      => (float)$z['gastos'],
                'pedidoMinimo'=> (float)$z['pedidoMinimo']
            ];
        }
    }
    // Si no hay match por nombre, aceptamos la dirección con la primera zona activa
    // (criterio del cliente: aceptar reparto a cualquier dirección).
    return [
        'valida'      => true,
        'idZona'      => (int)$zonas[0]['id'],
        'nombre'      => $zonas[0]['nombre'],
        'gastos'      => (float)$zonas[0]['gastos'],
        'pedidoMinimo'=> (float)$zonas[0]['pedidoMinimo'],
        'metodo'      => 'fallback'
    ];
}

// ─── crear_pedido ────────────────────────────────────────────────────
function tool_crear_pedido(PDO $db, array $args, string $callId, ?string $telefonoLlamada): array {
    // Vincular llamada
    $llamadaId = null;
    if ($callId !== '') {
        $s = $db->prepare("SELECT id FROM qo_llamadas WHERE vapi_call_id = :v");
        $s->bindValue(':v', $callId);
        $s->execute();
        $llamadaId = $s->fetchColumn() ?: null;
    }

    $idUsuario = (int)($args['idUsuario'] ?? 0);
    if ($idUsuario <= 0) $idUsuario = ID_USUARIO_AGENTE;
    $nombreUsr = (string)($args['nombreUsuario'] ?? 'Cliente voz');
    $telefono  = (string)($args['telefono'] ?? $telefonoLlamada ?? '');
    $tipoVenta = (string)($args['tipoVenta'] ?? 'Recogida');
    $direccion = (string)($args['direccion'] ?? '');
    $idZona    = (int)($args['idZona'] ?? 1);
    $hora      = (string)($args['horaEntrega'] ?? '');
    $tipoPago  = (string)($args['tipoPago'] ?? 'Efectivo');
    $coment    = (string)($args['comentario'] ?? '');
    $lineas    = $args['lineas'] ?? [];

    if (!in_array($tipoVenta, ['Envío', 'Envio', 'Recogida'])) {
        return ['error' => 'tipoVenta_invalido', 'recibido' => $tipoVenta];
    }
    if ($tipoVenta === 'Envío' || $tipoVenta === 'Envio') {
        $tipoVenta = 'Envío';
        if ($direccion === '') return ['error' => 'direccion_requerida'];
    }
    if (!is_array($lineas) || count($lineas) === 0) {
        return ['error' => 'lineas_vacias'];
    }
    // Normalizar horaEntrega — el agente suele mandar solo "HH:MM" o "16:00"
    date_default_timezone_set('Europe/Madrid');
    if ($hora === '') {
        // ETA determinista por saturación (NO lo elige el LLM)
        $eta  = tiempo_estimado_minutos($db, $lineas);
        $hora = date('Y-m-d H:i:s', strtotime("+{$eta} minutes"));
    } elseif (preg_match('/^\d{1,2}:\d{2}$/', $hora)) {
        // Solo HH:MM → fecha de hoy. Si ya pasó, rechazamos.
        $hoy       = date('Y-m-d');
        $candidato = strtotime("$hoy $hora:00");
        if ($candidato === false || $candidato < time() - 60) {
            return ['error' => 'hora_pasada', 'hora_solicitada' => $hora, 'hora_actual' => date('H:i')];
        }
        $hora = date('Y-m-d H:i:s', $candidato);
    } elseif (preg_match('/^\d{4}-\d{2}-\d{2} \d{1,2}:\d{2}$/', $hora)) {
        $hora .= ':00';
        if (strtotime($hora) < time() - 60) {
            return ['error' => 'hora_pasada', 'hora_solicitada' => $hora, 'hora_actual' => date('Y-m-d H:i')];
        }
    } elseif (!preg_match('/^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}$/', $hora)) {
        $ts = strtotime($hora);
        if ($ts === false) {
            return ['error' => 'hora_invalida', 'hora_solicitada' => $hora];
        }
        if ($ts < time() - 60) {
            return ['error' => 'hora_pasada', 'hora_solicitada' => $hora, 'hora_actual' => date('Y-m-d H:i')];
        }
        $hora = date('Y-m-d H:i:s', $ts);
    } elseif (strtotime($hora) < time() - 60) {
        return ['error' => 'hora_pasada', 'hora_solicitada' => $hora, 'hora_actual' => date('Y-m-d H:i')];
    }

    $totalProductos = 0.0;
    foreach ($lineas as $l) {
        $totalProductos += (float)($l['precio'] ?? 0) * (int)($l['cantidad'] ?? 0);
    }

    // Gastos de envío automáticos (tipo=1 en qo_pedidos_detalle).
    // Solo si es envío y el cliente no ha mandado ya una línea de portes.
    $gastosEnvio = 0.0;
    if ($tipoVenta === 'Envío') {
        $envRow = $db->query("SELECT valor FROM qo_config_agente WHERE clave='gastos_envio_eur'")->fetchColumn();
        $gastosEnvio = (float)(json_decode((string)$envRow, true) ?: 1.90);
    }
    $total = $totalProductos + $gastosEnvio;

    // Umbral importe
    $umbralRow = $db->query("SELECT valor FROM qo_config_agente WHERE clave='max_importe_sin_humano_eur'")->fetchColumn();
    $umbral    = (float)(json_decode((string)$umbralRow, true) ?: 100);
    if ($total > $umbral) {
        return ['error' => 'importe_supera_umbral', 'total' => $total, 'umbral' => $umbral, 'requiere_humano' => true];
    }

    // Blacklist
    if ($telefono !== '') {
        $telNorm = preg_replace('/[^0-9+]/', '', $telefono);
        $s = $db->prepare("SELECT motivo FROM qo_blacklist_telefonos WHERE telefono = :t");
        $s->bindValue(':t', $telNorm);
        $s->execute();
        if ($motivo = $s->fetchColumn()) {
            return ['error' => 'telefono_bloqueado', 'motivo' => $motivo];
        }
    }

    // Código único (ASA + 7 dígitos = 10 chars)
    $intentos = 0;
    do {
        $codigo = 'ASA' . str_pad((string)random_int(1, 9999999), 7, '0', STR_PAD_LEFT);
        $check = $db->prepare("SELECT 1 FROM qo_pedidos WHERE codigo = :c");
        $check->bindValue(':c', $codigo);
        $check->execute();
        if ($check->fetchColumn() === false) break;
    } while (++$intentos < 5);

    try {
        $db->beginTransaction();

        $ins = $db->prepare("
            INSERT INTO qo_pedidos
                (origen, llamada_id, codigo, idEstablecimiento, horaPedido, idUsuario,
                 estado, idZona, nuevoPedido, direccion, comentario, horaEntrega,
                 tipo, tipoVenta, tipoPago, nombreUsuario, valorado, completo, anulado, recoger)
            VALUES
                ('voz', :llid, :cod, :est, NOW(), :usr,
                 1, :zona, 1, :dir, :com, :hent,
                 1, :tipov, :tipop, :nomu, 1, 0, 0, :recoger)
        ");
        $ins->bindValue(':llid',   $llamadaId, $llamadaId === null ? PDO::PARAM_NULL : PDO::PARAM_INT);
        $ins->bindValue(':cod',    $codigo);
        $ins->bindValue(':est',    ID_ESTABLECIMIENTO, PDO::PARAM_INT);
        $ins->bindValue(':usr',    $idUsuario, PDO::PARAM_INT);
        $ins->bindValue(':zona',   $idZona,    PDO::PARAM_INT);
        $ins->bindValue(':dir',    $direccion);
        $ins->bindValue(':com',    $coment);
        $ins->bindValue(':hent',   $hora);
        $ins->bindValue(':tipov',  $tipoVenta);
        $ins->bindValue(':tipop',  $tipoPago);
        $ins->bindValue(':nomu',   $nombreUsr);
        $ins->bindValue(':recoger',$tipoVenta === 'Recogida' ? 1 : 0, PDO::PARAM_INT);
        $ins->execute();

        $pedidoId = (int)$db->lastInsertId();

        // Contrato de línea canónico (voz ↔ app): persistimos idOpcion e
        // ingredientes_json de forma estructurada. NULL si la línea no los trae
        // (P1 deja el path write-ready; el cálculo de precio por opción/ingrediente
        // se blindará server-side en P3).
        $insDet = $db->prepare("
            INSERT INTO qo_pedidos_detalle
                (idPedido, idProducto, idOpcion, precio, cantidad, tipo, concepto, ingredientes_json, comentario, tipoVenta, pagadoConPuntos)
            VALUES (:p, :prod, :opc, :precio, :cant, 0, :conc, :ing, :coment, :tv, 0)
        ");
        foreach ($lineas as $l) {
            // idOpcion: entero o NULL
            $idOpcion = isset($l['idOpcion']) && $l['idOpcion'] !== null && (int)$l['idOpcion'] > 0
                ? (int)$l['idOpcion'] : null;
            // ingredientes: JSON válido o NULL (nunca cadena vacía por el contrato)
            $ingJson = null;
            if (isset($l['ingredientes']) && is_array($l['ingredientes']) && count($l['ingredientes']) > 0) {
                $ingJson = json_encode(array_values($l['ingredientes']), JSON_UNESCAPED_UNICODE);
            }

            $insDet->bindValue(':p',      $pedidoId, PDO::PARAM_INT);
            $insDet->bindValue(':prod',   (int)($l['idProducto'] ?? 0), PDO::PARAM_INT);
            $insDet->bindValue(':opc',    $idOpcion, $idOpcion === null ? PDO::PARAM_NULL : PDO::PARAM_INT);
            $insDet->bindValue(':precio', (float)($l['precio'] ?? 0));
            $insDet->bindValue(':cant',   (int)($l['cantidad'] ?? 1), PDO::PARAM_INT);
            $insDet->bindValue(':conc',   (string)($l['concepto'] ?? ''));
            $insDet->bindValue(':ing',    $ingJson, $ingJson === null ? PDO::PARAM_NULL : PDO::PARAM_STR);
            $insDet->bindValue(':coment', (string)($l['comentario'] ?? ''));
            $insDet->bindValue(':tv',     $tipoVenta);
            $insDet->execute();
        }

        // Línea de gastos de envío (idProducto=0, tipo=1) — mismo patrón que la app
        if ($gastosEnvio > 0) {
            $insEnv = $db->prepare("
                INSERT INTO qo_pedidos_detalle
                    (idPedido, idProducto, precio, cantidad, tipo, concepto, comentario, tipoVenta, pagadoConPuntos)
                VALUES (:p, 0, :precio, 1, 1, '', '', :tv, 0)
            ");
            $insEnv->bindValue(':p',      $pedidoId, PDO::PARAM_INT);
            $insEnv->bindValue(':precio', $gastosEnvio);
            $insEnv->bindValue(':tv',     $tipoVenta);
            $insEnv->execute();
        }

        $insEst = $db->prepare("INSERT INTO qo_pedidos_estado (estado, idUsuario, fecha, idPedido) VALUES (1, :u, NOW(), :p)");
        $insEst->bindValue(':u', $idUsuario, PDO::PARAM_INT);
        $insEst->bindValue(':p', $pedidoId, PDO::PARAM_INT);
        $insEst->execute();

        if ($llamadaId !== null) {
            $upd = $db->prepare("UPDATE qo_llamadas SET pedido_id = :p WHERE id = :l");
            $upd->bindValue(':p', $pedidoId, PDO::PARAM_INT);
            $upd->bindValue(':l', $llamadaId, PDO::PARAM_INT);
            $upd->execute();
        }

        $db->commit();

        // Push al staff: pedido por voz recién creado
        $tokens = agente_tokens_staff($db, ID_ESTABLECIMIENTO);
        if (count($tokens) > 0) {
            $resumen = '';
            foreach ($lineas as $l) {
                $cant = (int)($l['cantidad'] ?? 1);
                $conc = (string)($l['concepto'] ?? '');
                $resumen .= ($resumen === '' ? '' : ', ') . "{$cant}× {$conc}";
            }
            $mensaje = "📞 Pedido voz #{$codigo} · " . number_format($total, 2) . "€ · {$tipoVenta}"
                     . ($resumen !== '' ? "\n{$resumen}" : '');
            agente_push(
                $tokens,
                'Asador Morón',
                $mensaje,
                [
                    'tipo'      => 'pedido_voz',
                    'pedido_id' => $pedidoId,
                    'codigo'    => $codigo,
                    'tipoVenta' => $tipoVenta
                ]
            );
        }

        // Versión "deletreada" del código para que el TTS lo lea claramente.
        // ASA4765252 → "A S A. 4. 7. 6. 5. 2. 5. 2"
        $codigoLeer = '';
        for ($i = 0; $i < strlen($codigo); $i++) {
            $c = $codigo[$i];
            $codigoLeer .= ($codigoLeer === '' ? '' : ' ') . $c;
            // tras los 3 caracteres alfa pone un punto pequeño para pausa
            if ($i === 2) $codigoLeer .= '.';
        }

        return [
            'ok'           => true,
            'id'           => $pedidoId,
            'codigo'       => $codigo,
            'codigo_leer'  => $codigoLeer,
            'mensaje'      => "Pedido confirmado. Tu código es: $codigoLeer. Hasta pronto.",
            'total'        => round($total, 2),
            'horaEntrega'  => $hora,
            'origen'       => 'voz',
            'llamada_id'   => $llamadaId
        ];

    } catch (\Throwable $e) {
        $db->rollBack();
        return ['error' => 'pedido_fallido', 'detalle' => $e->getMessage()];
    }
}

// ─── resumen_pedido ──────────────────────────────────────────────────
// Devuelve el resumen final (frase para leer + total exacto).
// El agente DEBE llamar a esta tool antes de pedir confirmación final,
// y leer literalmente el campo `resumen`. Así garantizamos que el total
// y la hora coinciden con lo que crear_pedido va a guardar.
// La hora la calculamos NOSOTROS según saturación de cocina: el agente
// no la pregunta al cliente, simplemente la informa.
function tool_resumen_pedido(PDO $db, array $args): array {
    $lineas    = is_array($args['lineas'] ?? null) ? $args['lineas'] : [];
    $tipoVenta = (string)($args['tipoVenta'] ?? 'Envío');
    if ($tipoVenta === 'Envio') $tipoVenta = 'Envío';
    $direccion = trim((string)($args['direccion'] ?? ''));

    if (count($lineas) === 0) return ['error' => 'lineas_vacias'];

    date_default_timezone_set('Europe/Madrid');

    // Lookup precios autoritativos en BD por idProducto (anti-alucinación del LLM)
    $ids = [];
    foreach ($lineas as $l) {
        $pid = (int)($l['idProducto'] ?? 0);
        if ($pid > 0) $ids[] = $pid;
    }
    $preciosDb = [];
    $nombresDb = [];
    if (count($ids) > 0) {
        $in  = implode(',', array_fill(0, count($ids), '?'));
        $stmt = $db->prepare("SELECT id, nombre, precio FROM qo_productos_est WHERE id IN ($in)");
        foreach (array_values($ids) as $i => $v) $stmt->bindValue($i + 1, $v, PDO::PARAM_INT);
        $stmt->execute();
        foreach ($stmt->fetchAll(PDO::FETCH_ASSOC) as $r) {
            $preciosDb[(int)$r['id']] = (float)$r['precio'];
            $nombresDb[(int)$r['id']] = trim($r['nombre']);
        }
    }

    $totalProductos = 0.0;
    $items          = [];
    foreach ($lineas as $l) {
        $pid    = (int)($l['idProducto'] ?? 0);
        $cant   = max(1, (int)($l['cantidad'] ?? 1));
        $precio = isset($preciosDb[$pid]) ? $preciosDb[$pid] : (float)($l['precio'] ?? 0);
        $nombre = $nombresDb[$pid] ?? trim((string)($l['concepto'] ?? 'producto'));
        $totalProductos += $precio * $cant;
        $items[] = ['cantidad' => $cant, 'nombre' => $nombre, 'precio' => $precio];
    }

    // Envío
    $envio = 0.0;
    if ($tipoVenta === 'Envío') {
        $envRow = $db->query("SELECT valor FROM qo_config_agente WHERE clave='gastos_envio_eur'")->fetchColumn();
        $envio  = (float)(json_decode((string)$envRow, true) ?: 1.90);
    }
    $total = $totalProductos + $envio;

    // Texto natural de los productos
    $itemsTxt = '';
    foreach ($items as $it) {
        $cant = $it['cantidad'];
        $nom  = mb_strtolower($it['nombre'], 'UTF-8');
        $itemsTxt .= ($itemsTxt === '' ? '' : ', ');
        $itemsTxt .= ($cant === 1 ? ($cant_articulo = preg_match('/^[aeiouáéíóú]/u', $nom) ? 'un' : 'un') . ' ' . $nom
                                  : $cant . ' ' . $nom);
    }
    // Capitalizar la primera letra
    if ($itemsTxt !== '') $itemsTxt = mb_strtoupper(mb_substr($itemsTxt, 0, 1, 'UTF-8'), 'UTF-8') . mb_substr($itemsTxt, 1, null, 'UTF-8');

    // ETA según saturación de cocina (determinista, no LLM)
    $eta      = tiempo_estimado_minutos($db, $lineas);
    $horaIso  = date('Y-m-d H:i:s', strtotime("+{$eta} minutes"));
    $etaTxt   = "en {$eta} minutos";
    $totalNat = euro_natural($total);

    if ($tipoVenta === 'Envío') {
        $dir = $direccion !== '' ? $direccion : 'la dirección indicada';
        $resumen = "$itemsTxt. Envío a $dir, $etaTxt lo tiene. Total con envío: $totalNat. ¿Está conforme?";
    } else {
        $resumen = "$itemsTxt. Recogida en local, $etaTxt. Total: $totalNat. ¿Está conforme?";
    }

    return [
        'resumen'         => $resumen,
        'total'           => round($total, 2),
        'totalProductos'  => round($totalProductos, 2),
        'gastosEnvio'     => round($envio, 2),
        'etaMinutos'      => $eta,
        'horaEntrega'     => $horaIso,
        'totalNatural'    => $totalNat,
        'items'           => $items
    ];
}

/** Tiempo estimado: base + extra por pollo en cola + buffer. Redondeado a 5 min. */
function tiempo_estimado_minutos(PDO $db, array $lineas = []): int {
    $cfg = [];
    foreach ($db->query("SELECT clave, valor FROM qo_config_agente WHERE clave IN
        ('tiempo_preparacion_base_minutos','tiempo_extra_por_pollo','buffer_seguridad_minutos','modo_saturacion_umbral')
    ")->fetchAll(PDO::FETCH_ASSOC) as $r) {
        $cfg[$r['clave']] = json_decode($r['valor'], true);
    }
    $base   = (int)($cfg['tiempo_preparacion_base_minutos'] ?? 30);
    $extra  = (int)($cfg['tiempo_extra_por_pollo']          ?? 6);
    $buffer = (int)($cfg['buffer_seguridad_minutos']        ?? 0);
    $capac  = (int)($cfg['modo_saturacion_umbral']          ?? 10);

    $stmt = $db->prepare("SELECT COALESCE(cantidad,0) FROM qo_contador_pollos WHERE idEstablecimiento=:id AND fecha=CURDATE() LIMIT 1");
    $stmt->bindValue(':id', ID_ESTABLECIMIENTO, PDO::PARAM_INT);
    $stmt->execute();
    $pollosCola = (int)($stmt->fetchColumn() ?: 0);

    // Pollos en este pedido (aproximado por concepto)
    $pollosPedido = 0;
    foreach ($lineas as $l) {
        $c = mb_strtolower((string)($l['concepto'] ?? ''), 'UTF-8');
        if (strpos($c, 'pollo') !== false) $pollosPedido += max(1, (int)($l['cantidad'] ?? 1));
    }

    $eta = $base + $buffer + ($pollosCola + $pollosPedido) * $extra;
    // saturación → empuja un poco más
    if ($pollosCola >= $capac) $eta += 10;
    // redondeo a 5 min superior, mín 20, máx 90
    $eta = max(20, min(90, (int)(ceil($eta / 5) * 5)));
    return $eta;
}

// ─── Helpers de lenguaje natural ─────────────────────────────────────

/** "14:30" → "para las dos y media del mediodía"; "" → "lo antes posible" */
function hora_natural(string $hora): string {
    $hora = trim($hora);
    if ($hora === '' || preg_match('/^(ahora|ya|asap|cuanto antes)$/i', $hora)) {
        return 'lo antes posible';
    }
    // Aceptar "HH:MM" o "YYYY-MM-DD HH:MM[:SS]"
    if (preg_match('/(\d{1,2}):(\d{2})/', $hora, $m)) {
        $h = (int)$m[1]; $mi = (int)$m[2];
    } else {
        return 'lo antes posible';
    }

    static $unidades = [1=>'una',2=>'dos',3=>'tres',4=>'cuatro',5=>'cinco',6=>'seis',
                        7=>'siete',8=>'ocho',9=>'nueve',10=>'diez',11=>'once',12=>'doce'];
    $h12 = $h % 12; if ($h12 === 0) $h12 = 12;
    $palabraHora = $unidades[$h12] ?? (string)$h12;

    if ($mi === 0)       $minTxt = '';
    elseif ($mi === 15)  $minTxt = ' y cuarto';
    elseif ($mi === 30)  $minTxt = ' y media';
    elseif ($mi === 45)  $minTxt = ' menos cuarto';
    else                 $minTxt = ' y ' . numero_a_palabras($mi);

    if ($h === 0 || $h === 24)       $franja = 'de la madrugada';
    elseif ($h < 6)                  $franja = 'de la madrugada';
    elseif ($h < 12)                 $franja = 'de la mañana';
    elseif ($h === 12 || $h === 13)  $franja = 'del mediodía';
    elseif ($h < 20)                 $franja = 'de la tarde';
    else                             $franja = 'de la noche';

    // Si son las "menos cuarto" la hora cuenta es la siguiente
    if ($mi === 45) {
        $hNext = ($h + 1) % 24;
        $hNext12 = $hNext % 12; if ($hNext12 === 0) $hNext12 = 12;
        $palabraHora = $unidades[$hNext12] ?? (string)$hNext12;
    }

    $articulo = ($palabraHora === 'una') ? 'la' : 'las';
    return "para $articulo $palabraHora$minTxt $franja";
}

/** 30.80 → "treinta euros con ochenta"; 30.00 → "treinta euros" */
function euro_natural(float $v): string {
    $entero   = (int)floor($v);
    $cents    = (int)round(($v - $entero) * 100);
    $eurTxt   = $entero === 1 ? 'un euro' : (numero_a_palabras($entero) . ' euros');
    if ($cents === 0) return $eurTxt;
    return $eurTxt . ' con ' . numero_a_palabras($cents);
}

/** Convierte un entero 0..999 a palabras en castellano. */
function numero_a_palabras(int $n): string {
    if ($n < 0) return (string)$n;
    static $u = ['cero','uno','dos','tres','cuatro','cinco','seis','siete','ocho','nueve',
                 'diez','once','doce','trece','catorce','quince','dieciséis','diecisiete','dieciocho','diecinueve',
                 'veinte','veintiuno','veintidós','veintitrés','veinticuatro','veinticinco','veintiséis','veintisiete','veintiocho','veintinueve'];
    static $dec = [2=>'veinte',3=>'treinta',4=>'cuarenta',5=>'cincuenta',6=>'sesenta',7=>'setenta',8=>'ochenta',9=>'noventa'];
    static $cen = [1=>'ciento',2=>'doscientos',3=>'trescientos',4=>'cuatrocientos',5=>'quinientos',
                   6=>'seiscientos',7=>'setecientos',8=>'ochocientos',9=>'novecientos'];
    if ($n < 30) return $u[$n];
    if ($n < 100) {
        $d = intdiv($n, 10); $r = $n % 10;
        return $dec[$d] . ($r === 0 ? '' : ' y ' . $u[$r]);
    }
    if ($n === 100) return 'cien';
    if ($n < 1000) {
        $c = intdiv($n, 100); $r = $n % 100;
        return $cen[$c] . ($r === 0 ? '' : ' ' . numero_a_palabras($r));
    }
    return (string)$n;
}
