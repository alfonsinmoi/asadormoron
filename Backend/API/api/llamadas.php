<?php
/**
 * Listado y detalle de llamadas para la app del gestor.
 *
 *  GET /api/llamadas.php
 *      ?desde=YYYY-MM-DD
 *      ?hasta=YYYY-MM-DD
 *      ?estado=en_curso|completada|transferida|no_pedido|fallida
 *      ?telefono=...
 *      ?idEstablecimiento=...
 *      ?page=1
 *      ?per_page=20
 *
 *  GET /api/llamadas.php?id=123    → detalle + transcripción
 */

include __DIR__ . "/../config.php";
include __DIR__ . "/../utils.php";

header("Content-Type: application/json; charset=utf-8");

if ($_SERVER['REQUEST_METHOD'] !== 'GET') {
    http_response_code(405);
    echo json_encode(['error' => 'Method not allowed']);
    exit();
}

$dbConn = connect($db);

try {
    // ───── Detalle de una llamada ──────────────────────────────────────
    if (isset($_GET['id'])) {
        $id = (int)$_GET['id'];
        if ($id <= 0) {
            http_response_code(400);
            echo json_encode(['error' => 'id invalido']);
            exit();
        }

        $stmt = $dbConn->prepare("
            SELECT l.*, p.codigo AS pedido_codigo
            FROM qo_llamadas l
            LEFT JOIN qo_pedidos p ON p.id = l.pedido_id
            WHERE l.id = :id
        ");
        $stmt->bindValue(':id', $id, PDO::PARAM_INT);
        $stmt->execute();
        $llamada = $stmt->fetch(PDO::FETCH_ASSOC);

        if (!$llamada) {
            http_response_code(404);
            echo json_encode(['error' => 'no encontrada']);
            exit();
        }

        $llamada['metadatos'] = $llamada['metadatos'] ? json_decode($llamada['metadatos'], true) : null;

        // Transcripción concatenada
        $stmtTrans = $dbConn->prepare("
            SELECT id, texto, texto_estructurado, fecha
            FROM qo_transcripciones
            WHERE llamada_id = :id
            ORDER BY fecha, id
        ");
        $stmtTrans->bindValue(':id', $id, PDO::PARAM_INT);
        $stmtTrans->execute();
        $turnos = [];
        foreach ($stmtTrans->fetchAll(PDO::FETCH_ASSOC) as $t) {
            $estructura = $t['texto_estructurado'] ? json_decode($t['texto_estructurado'], true) : null;
            $turnos[] = [
                'id'        => (int)$t['id'],
                'texto'     => $t['texto'],
                'turnos'    => $estructura,
                'fecha'     => $t['fecha']
            ];
        }
        $llamada['transcripcion'] = $turnos;

        echo json_encode($llamada);
        exit();
    }

    // ───── Listado paginado ────────────────────────────────────────────
    $where  = [];
    $params = [];

    if (!empty($_GET['desde']))  { $where[] = 'l.fecha_inicio >= :desde';     $params[':desde'] = $_GET['desde'] . ' 00:00:00'; }
    if (!empty($_GET['hasta']))  { $where[] = 'l.fecha_inicio <= :hasta';     $params[':hasta'] = $_GET['hasta'] . ' 23:59:59'; }
    if (!empty($_GET['estado'])) { $where[] = 'l.estado = :estado';            $params[':estado'] = $_GET['estado']; }
    if (!empty($_GET['telefono'])) {
        $tel = preg_replace('/[^0-9+]/', '', $_GET['telefono']);
        $where[] = 'l.telefono_origen LIKE :tel';
        $params[':tel'] = '%' . substr($tel, -9);
    }
    if (!empty($_GET['idEstablecimiento'])) {
        $where[] = 'l.idEstablecimiento = :est';
        $params[':est'] = (int)$_GET['idEstablecimiento'];
    }

    $whereSql = $where ? ' WHERE ' . implode(' AND ', $where) : '';

    // Total
    $stmtCount = $dbConn->prepare("SELECT COUNT(*) FROM qo_llamadas l $whereSql");
    foreach ($params as $k => $v) $stmtCount->bindValue($k, $v);
    $stmtCount->execute();
    $total = (int)$stmtCount->fetchColumn();

    // Paginación
    $page     = max(1, (int)($_GET['page']     ?? 1));
    $perPage  = min(100, max(1, (int)($_GET['per_page'] ?? 20)));
    $offset   = ($page - 1) * $perPage;

    $stmt = $dbConn->prepare("
        SELECT l.id, l.vapi_call_id, l.telefono_origen, l.cliente_id, l.idEstablecimiento,
               l.estado, l.pedido_id, l.duracion_segundos, l.coste_estimado,
               l.fecha_inicio, l.fecha_fin,
               p.codigo AS pedido_codigo
        FROM qo_llamadas l
        LEFT JOIN qo_pedidos p ON p.id = l.pedido_id
        $whereSql
        ORDER BY l.fecha_inicio DESC
        LIMIT :lim OFFSET :off
    ");
    foreach ($params as $k => $v) $stmt->bindValue($k, $v);
    $stmt->bindValue(':lim', $perPage, PDO::PARAM_INT);
    $stmt->bindValue(':off', $offset,  PDO::PARAM_INT);
    $stmt->execute();

    echo json_encode([
        'items'    => $stmt->fetchAll(PDO::FETCH_ASSOC),
        'total'    => $total,
        'page'     => $page,
        'per_page' => $perPage,
        'pages'    => (int)ceil($total / $perPage)
    ]);

} catch (Exception $e) {
    error_log('[llamadas] ' . $e->getMessage());
    http_response_code(500);
    echo json_encode(['error' => 'Internal error']);
}
