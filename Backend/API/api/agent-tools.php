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
            if (strpos($k, 'HTTP_X_VAPI') === 0) $vapiHeaders[$k] = '***REDACTED***';
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
        $result = ['error' => 'tool_error'];  // detalle ya logueado; no exponer en HTTP
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

// Normaliza una palabra para búsqueda tolerante a acento/transcripción andaluza:
// quita acentos y mapea errores típicos de STT a la raíz del producto real.
function normalizar_andaluz(string $palabra): string {
    // La colación de BD (utf8mb4_unicode_ci) ya es insensible a acentos/ñ, así que
    // NO tocamos acentos aquí (evita romper "aliños", "bocata", etc.). Solo mapeamos
    // errores CLAROS de transcripción a una raíz que SÍ existe en el menú, y siempre
    // mediante LIKE %substring% (matchea plural/singular).
    $p = mb_strtolower(trim($palabra), 'UTF-8');
    static $map = [
        'boyo'=>'pollo','bollo'=>'pollo','rollo'=>'pollo','poyo'=>'pollo',
        'patada'=>'patata',
        'chocha'=>'choco',
        'ganbas'=>'gamba','ganbon'=>'gambon',
        'anburgesa'=>'hamburguesa','amburguesa'=>'hamburguesa','hamburgesa'=>'hamburguesa',
        'pisa'=>'pizza',
    ];
    return $map[$p] ?? $p;
}

// ─── get_cliente ─────────────────────────────────────────────────────
// ─── get_menu: catálogo del establecimiento ───────────────────────────
// Si se pasa `buscar` filtra por nombre/categoría (LIKE) para reducir resultados.
// Si no, devuelve los primeros 40 productos (resumen).
function tool_get_menu(PDO $db, string $buscar = ''): array {
    $buscar = trim($buscar);

    if ($buscar !== '') {
        // Búsqueda fuzzy + tolerante a acento andaluz: normaliza cada palabra
        // (errores típicos de transcripción) antes del LIKE, y descarta palabras
        // vacías/muy cortas que no ayudan a filtrar.
        $palabras = array_filter(
            array_map('normalizar_andaluz', preg_split('/\s+/', mb_strtolower($buscar, 'UTF-8'))),
            fn($p) => mb_strlen($p) >= 3
        );
        if (count($palabras) === 0) { // fallback: usa el término tal cual
            $palabras = [mb_strtolower($buscar, 'UTF-8')];
        }
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

    // Métrica de reconocimiento (P5): registra la búsqueda y su nº de resultados.
    // Best-effort: nunca rompe get_menu por un fallo de métricas.
    if ($buscar !== '') {
        try {
            $normWords = array_map('normalizar_andaluz', preg_split('/\s+/', mb_strtolower($buscar, 'UTF-8')));
            $bs = $db->prepare("INSERT INTO qo_agente_busquedas (idEstablecimiento, termino, termino_norm, resultados, fecha)
                                VALUES (:e, :t, :n, :r, NOW())");
            $bs->bindValue(':e', ID_ESTABLECIMIENTO, PDO::PARAM_INT);
            $bs->bindValue(':t', mb_substr($buscar, 0, 255));
            $bs->bindValue(':n', mb_substr(implode(' ', $normWords), 0, 255));
            $bs->bindValue(':r', count($productos), PDO::PARAM_INT);
            $bs->execute();
        } catch (\Throwable $e) { /* métricas no bloquean */ }
    }

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

    $capacRow = $db->query("SELECT valor FROM qo_config_agente WHERE clave='modo_saturacion_umbral'")->fetchColumn();
    $capac    = (int)(json_decode((string)$capacRow, true) ?: 10);

    $sql = $db->prepare("SELECT COALESCE(cantidad,0) FROM qo_contador_pollos WHERE idEstablecimiento=:id AND fecha=CURDATE() LIMIT 1");
    $sql->bindValue(':id', ID_ESTABLECIMIENTO, PDO::PARAM_INT);
    $sql->execute();
    $pollos = (int)($sql->fetchColumn() ?: 0);

    // Mismo cálculo de ETA que resumen/crear (evita ofrecer un slot distinto al grabado).
    date_default_timezone_set('Europe/Madrid');
    $arranque = (new DateTime('now'))->modify('+' . eta_minutos($db, 0) . ' minutes');
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
    // Vincular llamada + IDEMPOTENCIA: si esta llamada ya generó un pedido,
    // no creamos otro (reintento del LLM / doble tool-call). Devolvemos el existente.
    $llamadaId = null;
    if ($callId !== '') {
        $s = $db->prepare("SELECT id, pedido_id FROM qo_llamadas WHERE vapi_call_id = :v");
        $s->bindValue(':v', $callId);
        $s->execute();
        $row = $s->fetch(PDO::FETCH_ASSOC) ?: null;
        if ($row) {
            $llamadaId = (int)$row['id'];
            if (!empty($row['pedido_id'])) {
                $pe = $db->prepare("SELECT codigo FROM qo_pedidos WHERE id = :p");
                $pe->bindValue(':p', (int)$row['pedido_id'], PDO::PARAM_INT);
                $pe->execute();
                $codExist = (string)($pe->fetchColumn() ?: '');
                $codLeer = '';
                for ($i = 0; $i < strlen($codExist); $i++) {
                    $codLeer .= ($codLeer === '' ? '' : ' ') . $codExist[$i];
                    if ($i === 2) $codLeer .= '.';
                }
                return [
                    'ok'          => true,
                    'idempotente' => true,
                    'id'          => (int)$row['pedido_id'],
                    'codigo'      => $codExist,
                    'codigo_leer' => $codLeer,
                    'mensaje'     => "El pedido ya estaba confirmado. Tu código es: $codLeer. Hasta pronto.",
                ];
            }
        }
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

    // Precio AUTORITATIVO server-side por línea (revalida opción e ingredientes
    // contra BD ANTES de crear el pedido; recomputa el precio ignorando el del LLM).
    $totalProductos = 0.0;
    $lineasCalc = [];
    foreach ($lineas as $l) {
        $cant = max(1, (int)($l['cantidad'] ?? 1));
        $calc = precio_linea_autoritativo($db, $l);
        if (!$calc['ok']) {
            return [
                'error'         => $calc['error'],
                'idProducto'    => $calc['idProducto']    ?? null,
                'idOpcion'      => $calc['idOpcion']      ?? null,
                'idIngrediente' => $calc['idIngrediente'] ?? null,
                'hint'          => 'id no válido para ese producto; reconsulta get_menu/get_opciones/get_ingredientes y reintenta.',
            ];
        }
        // Auditoría anti-alucinación: si el precio del LLM difiere del autoritativo, se loguea.
        $precioLLM = (float)($l['precio'] ?? 0);
        if (abs($precioLLM - $calc['precioUnit']) > 0.01) {
            agente_log('warn', 'precio_divergente', [
                'idProducto' => (int)($l['idProducto'] ?? 0),
                'concepto'   => $calc['nombre'],
                'precio_llm' => $precioLLM,
                'precio_srv' => $calc['precioUnit'],
            ]);
        }
        $totalProductos += $calc['precioUnit'] * $cant;
        $lineasCalc[] = ['orig' => $l, 'cant' => $cant, 'calc' => $calc];
    }

    // Gastos de envío (tipo=1) y bolsa (tipo=4, como la app) — nunca del LLM.
    $gastosEnvio = ($tipoVenta === 'Envío') ? agente_config_num($db, 'gastos_envio_eur', 1.90) : 0.0;
    $bolsaInfo   = calcular_bolsa($db, $totalProductos);
    $gastosBolsa = $bolsaInfo ? $bolsaInfo['total'] : 0.0;
    $total = $totalProductos + $gastosEnvio + $gastosBolsa;

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
        if ($pedidoId <= 0) throw new RuntimeException('lastInsertId devolvió ' . $pedidoId);

        // Contrato de línea canónico (voz ↔ app): persistimos idOpcion e
        // ingredientes_json de forma estructurada. NULL si la línea no los trae
        // (P1 deja el path write-ready; el cálculo de precio por opción/ingrediente
        // se blindará server-side en P3).
        $insDet = $db->prepare("
            INSERT INTO qo_pedidos_detalle
                (idPedido, idProducto, idOpcion, precio, cantidad, tipo, concepto, ingredientes_json, comentario, tipoVenta, pagadoConPuntos)
            VALUES (:p, :prod, :opc, :precio, :cant, 0, :conc, :ing, :coment, :tv, 0)
        ");
        foreach ($lineasCalc as $lc) {
            $l    = $lc['orig'];
            $calc = $lc['calc'];
            $idOpcion = (isset($l['idOpcion']) && (int)$l['idOpcion'] > 0) ? (int)$l['idOpcion'] : null;
            // ingredientes_json a partir de la versión NORMALIZADA (validada contra BD)
            $ingJson = count($calc['ingNorm']) > 0
                ? json_encode($calc['ingNorm'], JSON_UNESCAPED_UNICODE) : null;
            // concepto legible: nombre + opción + ingredientes
            $concepto = $calc['nombre']
                . ($calc['opcion'] ? ' (' . $calc['opcion'] . ')' : '')
                . ($calc['ingTxt'] !== '' ? ' ' . $calc['ingTxt'] : '');

            $insDet->bindValue(':p',      $pedidoId, PDO::PARAM_INT);
            $insDet->bindValue(':prod',   (int)($l['idProducto'] ?? 0), PDO::PARAM_INT);
            $insDet->bindValue(':opc',    $idOpcion, $idOpcion === null ? PDO::PARAM_NULL : PDO::PARAM_INT);
            $insDet->bindValue(':precio', $calc['precioUnit']);   // AUTORITATIVO, no el del LLM
            $insDet->bindValue(':cant',   $lc['cant'], PDO::PARAM_INT);
            $insDet->bindValue(':conc',   $concepto);
            $insDet->bindValue(':ing',    $ingJson, $ingJson === null ? PDO::PARAM_NULL : PDO::PARAM_STR);
            $insDet->bindValue(':coment', (string)($l['comentario'] ?? ''));
            $insDet->bindValue(':tv',     $tipoVenta);
            $insDet->execute();
        }

        // Línea de gastos de envío (idProducto=0, tipo=1) — mismo patrón que la app
        if ($gastosEnvio > 0) {
            $insExtra = $db->prepare("
                INSERT INTO qo_pedidos_detalle
                    (idPedido, idProducto, precio, cantidad, tipo, concepto, comentario, tipoVenta, pagadoConPuntos)
                VALUES (:p, 0, :precio, 1, :tipo, :conc, '', :tv, 0)
            ");
            $insExtra->bindValue(':p', $pedidoId, PDO::PARAM_INT);
            $insExtra->bindValue(':precio', $gastosEnvio);
            $insExtra->bindValue(':tipo', 1, PDO::PARAM_INT);
            $insExtra->bindValue(':conc', 'Gastos de envío');
            $insExtra->bindValue(':tv', $tipoVenta);
            $insExtra->execute();
        }

        // Línea de bolsa (idProducto=0, tipo=4, precio unitario × nBolsas) — igual que la app.
        if ($bolsaInfo && $bolsaInfo['total'] > 0) {
            $insBolsa = $db->prepare("
                INSERT INTO qo_pedidos_detalle
                    (idPedido, idProducto, precio, cantidad, tipo, concepto, comentario, tipoVenta, pagadoConPuntos)
                VALUES (:p, 0, :precio, :cant, 4, 'Bolsa', '', :tv, 0)
            ");
            $insBolsa->bindValue(':p', $pedidoId, PDO::PARAM_INT);
            $insBolsa->bindValue(':precio', $bolsaInfo['precioUnit']);
            $insBolsa->bindValue(':cant', $bolsaInfo['cantidad'], PDO::PARAM_INT);
            $insBolsa->bindValue(':tv', $tipoVenta);
            $insBolsa->execute();
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
        agente_log('error', 'pedido_excepcion', ['call_id' => $callId, 'err' => $e->getMessage()]);
        return ['error' => 'pedido_fallido'];  // detalle solo en logs, nunca en HTTP
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

    // Precio AUTORITATIVO server-side por línea (opción absoluta + ingredientes).
    // Si una línea trae opción/ingrediente inválido, devolvemos error+hint para que
    // el LLM reintente (nunca inventamos precio).
    $totalProductos = 0.0;
    $items          = [];
    foreach ($lineas as $l) {
        $cant = max(1, (int)($l['cantidad'] ?? 1));
        $calc = precio_linea_autoritativo($db, $l);
        if (!$calc['ok']) {
            return ['error' => $calc['error'], 'idProducto' => $calc['idProducto'] ?? null,
                    'hint' => 'Vuelve a consultar get_menu/get_opciones/get_ingredientes; el id no es válido para ese producto.'];
        }
        $totalProductos += $calc['precioUnit'] * $cant;
        $items[] = [
            'cantidad' => $cant,
            'nombre'   => $calc['nombre'],
            'opcion'   => $calc['opcion'],
            'ingTxt'   => $calc['ingTxt'],
            'precio'   => $calc['precioUnit'],
        ];
    }

    // Envío + bolsa (bolsa calculada como en la app: nBolsas por rango)
    $envio     = ($tipoVenta === 'Envío') ? agente_config_num($db, 'gastos_envio_eur', 1.90) : 0.0;
    $bolsaInfo = calcular_bolsa($db, $totalProductos);
    $bolsa     = $bolsaInfo ? $bolsaInfo['total'] : 0.0;
    $total     = $totalProductos + $envio + $bolsa;

    // Texto natural de los productos (con opción e ingredientes)
    $itemsTxt = '';
    foreach ($items as $it) {
        $cant = $it['cantidad'];
        $nom  = mb_strtolower($it['nombre'], 'UTF-8');
        if ($it['opcion']) $nom .= ' ' . mb_strtolower($it['opcion'], 'UTF-8');
        if ($it['ingTxt'] !== '') $nom .= ' ' . $it['ingTxt'];
        $itemsTxt .= ($itemsTxt === '' ? '' : ', ');
        $itemsTxt .= ($cant === 1 ? 'un ' . $nom : $cant . ' ' . $nom);
    }
    if ($itemsTxt !== '') $itemsTxt = mb_strtoupper(mb_substr($itemsTxt, 0, 1, 'UTF-8'), 'UTF-8') . mb_substr($itemsTxt, 1, null, 'UTF-8');

    // ETA según saturación de cocina (determinista, no LLM)
    $eta      = tiempo_estimado_minutos($db, $lineas);
    $horaIso  = date('Y-m-d H:i:s', strtotime("+{$eta} minutes"));
    $etaTxt   = "en {$eta} minutos";
    $totalNat = euro_natural($total);

    // Frase de coste: menciona envío y/o bolsa solo si aplican
    $extras = [];
    if ($envio > 0) $extras[] = 'envío';
    if ($bolsa > 0) $extras[] = 'bolsa';
    $conExtras = count($extras) ? ' con ' . implode(' y ', $extras) : '';

    if ($tipoVenta === 'Envío') {
        $dir = $direccion !== '' ? $direccion : 'la dirección indicada';
        $resumen = "$itemsTxt. Envío a $dir, $etaTxt lo tiene. Total$conExtras: $totalNat. ¿Está conforme?";
    } else {
        $resumen = "$itemsTxt. Recogida en local, $etaTxt. Total$conExtras: $totalNat. ¿Está conforme?";
    }

    return [
        'resumen'         => $resumen,
        'total'           => round($total, 2),
        'totalProductos'  => round($totalProductos, 2),
        'gastosEnvio'     => round($envio, 2),
        'gastosBolsa'     => round($bolsa, 2),
        'etaMinutos'      => $eta,
        'horaEntrega'     => $horaIso,
        'totalNatural'    => $totalNat,
        'items'           => $items
    ];
}

// ─── Precio autoritativo de una línea (compartido resumen ↔ crear) ────────
// Fuente de verdad server-side: producto base, opción (precio ABSOLUTO en
// valorIncremento) e ingredientes añadidos (precio incremental). Nunca usa el
// precio que manda el LLM. Devuelve ok=false + error si algún id no es válido.
function precio_linea_autoritativo(PDO $db, array $l): array {
    $pid = (int)($l['idProducto'] ?? 0);
    if ($pid <= 0) return ['ok' => false, 'error' => 'idProducto_invalido'];

    // Cantidad válida (>0). No la "corregimos" en silencio: rechazamos para no crear
    // una línea que el cliente no pidió.
    if (isset($l['cantidad']) && (int)$l['cantidad'] <= 0) {
        return ['ok' => false, 'error' => 'cantidad_invalida', 'idProducto' => $pid];
    }
    // Ingredientes: si vienen, deben ser un array (no una cadena/JSON malformado).
    if (isset($l['ingredientes']) && !is_array($l['ingredientes'])) {
        return ['ok' => false, 'error' => 'ingredientes_formato_invalido', 'idProducto' => $pid];
    }

    $st = $db->prepare("SELECT nombre, precio FROM qo_productos_est WHERE id=:id AND eliminado=0");
    $st->bindValue(':id', $pid, PDO::PARAM_INT); $st->execute();
    $prod = $st->fetch(PDO::FETCH_ASSOC);
    if (!$prod) return ['ok' => false, 'error' => 'producto_no_existe', 'idProducto' => $pid];

    $nombre     = trim($prod['nombre']);
    $precioUnit = (float)$prod['precio'];
    $opcionTxt  = null;

    // Opción → precio ABSOLUTO
    $idOpcion = (isset($l['idOpcion']) && (int)$l['idOpcion'] > 0) ? (int)$l['idOpcion'] : null;
    if ($idOpcion !== null) {
        $so = $db->prepare("SELECT opcion, valorIncremento FROM qo_productos_opc WHERE id=:o AND idProducto=:p");
        $so->bindValue(':o', $idOpcion, PDO::PARAM_INT); $so->bindValue(':p', $pid, PDO::PARAM_INT); $so->execute();
        $op = $so->fetch(PDO::FETCH_ASSOC);
        if (!$op) return ['ok' => false, 'error' => 'opcion_invalida', 'idProducto' => $pid, 'idOpcion' => $idOpcion];
        $precioUnit = (float)$op['valorIncremento'];
        $opcionTxt  = trim($op['opcion']);
    }

    // Ingredientes añadidos/quitados
    $ingTxt = ''; $ingNorm = [];
    if (isset($l['ingredientes']) && is_array($l['ingredientes'])) {
        foreach ($l['ingredientes'] as $ing) {
            $iid = (int)($ing['idIngrediente'] ?? 0);
            if ($iid <= 0) continue;
            $add = !empty($ing['esAnadir']);
            $si = $db->prepare("
                SELECT ie.nombre, ip.precio
                FROM qo_ingredientes_producto ip
                JOIN qo_ingredientes_establecimiento ie ON ie.id = ip.idIngrediente
                WHERE ip.idProducto=:p AND ip.idIngrediente=:i
            ");
            $si->bindValue(':p', $pid, PDO::PARAM_INT); $si->bindValue(':i', $iid, PDO::PARAM_INT); $si->execute();
            $ir = $si->fetch(PDO::FETCH_ASSOC);
            if (!$ir) return ['ok' => false, 'error' => 'ingrediente_invalido', 'idProducto' => $pid, 'idIngrediente' => $iid];
            $precioReal = (float)$ir['precio'];          // precio catálogo del ingrediente
            $efecto     = $add ? $precioReal : 0.0;       // solo suma al total si se AÑADE
            $precioUnit += $efecto;
            // Guardamos el precio real (auditoría) y el efecto aplicado; cocina ve qué se quitó y su valor.
            $ingNorm[] = ['idIngrediente' => $iid, 'esAnadir' => $add, 'precio' => $precioReal, 'efecto' => $efecto, 'nombre' => trim($ir['nombre'])];
            $ingTxt   .= ($add ? ' con ' : ' sin ') . mb_strtolower(trim($ir['nombre']), 'UTF-8');
        }
    }

    return [
        'ok'         => true,
        'precioUnit' => round($precioUnit, 2),
        'nombre'     => $nombre,
        'opcion'     => $opcionTxt,
        'ingTxt'     => trim($ingTxt),
        'ingNorm'    => $ingNorm,
    ];
}

// Calcula el cargo de bolsa igual que la app móvil (FranjasHorariasViewModel):
// precioBolsa y rangoBolsas viven en qo_configuracion_est con nombres de columna
// legacy reutilizados: tieneMediaPizza=precioBolsa, idCategoriaPizza=rangoBolsas.
//   nBolsas = floor(totalProductos / rangoBolsas)  (mínimo 1 si hay cargo)
//   totalBolsa = nBolsas * precioBolsa
// Devuelve null si no hay cargo configurado (precioBolsa<=0 o rango<=0).
function calcular_bolsa(PDO $db, float $totalProductos): ?array {
    $st = $db->prepare("SELECT tieneMediaPizza AS precioBolsa, idCategoriaPizza AS rangoBolsas
                        FROM qo_configuracion_est WHERE idEstablecimiento = :id LIMIT 1");
    $st->bindValue(':id', ID_ESTABLECIMIENTO, PDO::PARAM_INT);
    $st->execute();
    $cfg = $st->fetch(PDO::FETCH_ASSOC);
    if (!$cfg) return null;
    $precioBolsa = (float)$cfg['precioBolsa'];
    $rango       = (float)$cfg['rangoBolsas'];
    if ($precioBolsa <= 0 || $rango <= 0) return null;

    $n = (int)floor($totalProductos / $rango);
    if ($n < 1) $n = 1;
    $precioUnit = round($precioBolsa, 2);
    return [
        'cantidad'   => $n,
        'precioUnit' => $precioUnit,
        'total'      => round($n * $precioUnit, 2),   // coherente: cantidad × precioUnit
    ];
}

/** Lee una clave numérica de qo_config_agente (los valores están JSON-encoded). */
function agente_config_num(PDO $db, string $clave, float $default): float {
    $st = $db->prepare("SELECT valor FROM qo_config_agente WHERE clave = :c");
    $st->bindValue(':c', $clave); $st->execute();
    $v = $st->fetchColumn();
    if ($v === false) return $default;
    $dec = json_decode((string)$v, true);
    return is_numeric($dec) ? (float)$dec : (is_numeric($v) ? (float)$v : $default);
}

// Núcleo ÚNICO de cálculo de ETA (usado por resumen/crear y por get_slots_recogida,
// para que el tiempo ofrecido y el grabado nunca difieran). Defaults unificados.
function eta_minutos(PDO $db, int $pollosPedido = 0): int {
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

    $eta = $base + $buffer + ($pollosCola + $pollosPedido) * $extra;
    if ($pollosCola >= $capac) $eta += 10;              // saturación
    return max(20, min(90, (int)(ceil($eta / 5) * 5))); // redondeo 5 min, 20..90
}

/** ETA de un pedido: cuenta los pollos por idProducto/categoría y delega en eta_minutos(). */
function tiempo_estimado_minutos(PDO $db, array $lineas = []): int {
    $pollosPedido = 0;
    $ids = [];
    foreach ($lineas as $l) {
        $pid = (int)($l['idProducto'] ?? 0);
        if ($pid > 0) $ids[$pid] = ($ids[$pid] ?? 0) + max(1, (int)($l['cantidad'] ?? 1));
    }
    if (count($ids) > 0) {
        $in = implode(',', array_fill(0, count($ids), '?'));
        $q = $db->prepare("
            SELECT pe.id
            FROM qo_productos_est pe
            JOIN qo_productos_cat pc ON pc.id = pe.idCategoria
            WHERE pe.id IN ($in)
              AND (LOWER(pe.nombre) LIKE '%pollo%' OR LOWER(pc.nombre) LIKE '%pollo%')
        ");
        foreach (array_keys($ids) as $i => $v) $q->bindValue($i + 1, $v, PDO::PARAM_INT);
        $q->execute();
        foreach ($q->fetchAll(PDO::FETCH_COLUMN) as $pid) {
            $pollosPedido += $ids[(int)$pid] ?? 1;
        }
    }
    return eta_minutos($db, $pollosPedido);
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
