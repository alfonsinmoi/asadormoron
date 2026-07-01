<?php
/**
 * KPIs agregados del agente para el dashboard.
 *
 *  GET /api/dashboard-metricas.php
 *      ?desde=YYYY-MM-DD   (por defecto: hace 30 días)
 *      ?hasta=YYYY-MM-DD   (por defecto: hoy)
 *      ?idEstablecimiento=...
 *
 *  Cachea 5 min en APCu si está disponible.
 */

include __DIR__ . "/../config.php";
include __DIR__ . "/../utils.php";
require_once __DIR__ . "/_lib.php";

header("Content-Type: application/json; charset=utf-8");

if ($_SERVER['REQUEST_METHOD'] !== 'GET') {
    http_response_code(405);
    echo json_encode(['error' => 'Method not allowed']);
    exit();
}

$desde = $_GET['desde'] ?? date('Y-m-d', strtotime('-30 days'));
$hasta = $_GET['hasta'] ?? date('Y-m-d');
$idEst = isset($_GET['idEstablecimiento']) ? (int)$_GET['idEstablecimiento'] : null;

function calcular_metricas(array $dbConf, string $desde, string $hasta, ?int $idEst): array {
    $dbConn = connect($dbConf);

    $where  = ['l.fecha_inicio BETWEEN :desde AND :hasta'];
    $params = [':desde' => $desde . ' 00:00:00', ':hasta' => $hasta . ' 23:59:59'];
    if ($idEst !== null) {
        $where[]        = 'l.idEstablecimiento = :est';
        $params[':est'] = $idEst;
    }
    $whereSql = implode(' AND ', $where);

    // Contadores por estado
    $stmt = $dbConn->prepare("
        SELECT estado, COUNT(*) AS n
        FROM qo_llamadas l
        WHERE $whereSql
        GROUP BY estado
    ");
    foreach ($params as $k => $v) $stmt->bindValue($k, $v);
    $stmt->execute();
    $porEstado = [
        'en_curso'    => 0,
        'completada'  => 0,
        'transferida' => 0,
        'no_pedido'   => 0,
        'fallida'     => 0
    ];
    foreach ($stmt->fetchAll(PDO::FETCH_ASSOC) as $r) {
        $porEstado[$r['estado']] = (int)$r['n'];
    }

    // Agregados generales
    $stmt = $dbConn->prepare("
        SELECT COUNT(*)                                                      AS total,
               COALESCE(SUM(coste_estimado), 0)                              AS coste_total,
               COALESCE(AVG(duracion_segundos), 0)                           AS duracion_media,
               COALESCE(SUM(CASE WHEN pedido_id IS NOT NULL THEN 1 ELSE 0 END), 0) AS convertidas
        FROM qo_llamadas l
        WHERE $whereSql
    ");
    foreach ($params as $k => $v) $stmt->bindValue($k, $v);
    $stmt->execute();
    $agg   = $stmt->fetch(PDO::FETCH_ASSOC);
    $total = (int)$agg['total'];

    // Llamadas por hora (heatmap)
    $stmt = $dbConn->prepare("
        SELECT DAYOFWEEK(l.fecha_inicio) AS dia,
               HOUR(l.fecha_inicio)      AS hora,
               COUNT(*)                   AS n
        FROM qo_llamadas l
        WHERE $whereSql
        GROUP BY dia, hora
    ");
    foreach ($params as $k => $v) $stmt->bindValue($k, $v);
    $stmt->execute();
    $heatmap = $stmt->fetchAll(PDO::FETCH_ASSOC);

    // Pico de concurrencia (llamadas con solapamiento de fecha_inicio/fecha_fin)
    $stmt = $dbConn->prepare("
        SELECT MAX(concurrentes) AS pico
        FROM (
            SELECT l1.id,
                   (SELECT COUNT(*) FROM qo_llamadas l2
                    WHERE l2.fecha_inicio <= l1.fecha_inicio
                      AND (l2.fecha_fin IS NULL OR l2.fecha_fin >= l1.fecha_inicio)
                      AND " . str_replace('l.', 'l2.', $whereSql) . "
                   ) AS concurrentes
            FROM qo_llamadas l1
            WHERE " . str_replace('l.', 'l1.', $whereSql) . "
        ) AS sub
    ");
    foreach ($params as $k => $v) $stmt->bindValue($k, $v);
    $stmt->execute();
    $pico = (int)($stmt->fetchColumn() ?: 0);

    $tasa = $total > 0 ? round(((int)$agg['convertidas']) * 100 / $total, 1) : 0;

    // ─── Métricas de acento/reconocimiento (P5) ──────────────────────────
    // Búsquedas de get_menu en el rango; % que encontró producto = proxy de
    // calidad de transcripción. + nº de transcripciones que se normalizaron.
    $stmt = $dbConn->prepare("
        SELECT COUNT(*) AS busquedas,
               COALESCE(SUM(CASE WHEN resultados = 0 THEN 1 ELSE 0 END), 0) AS sin_resultado
        FROM qo_agente_busquedas
        WHERE fecha BETWEEN :desde AND :hasta
    ");
    foreach ($params as $k => $v) $stmt->bindValue($k, $v);
    $stmt->execute();
    $bus = $stmt->fetch(PDO::FETCH_ASSOC) ?: ['busquedas' => 0, 'sin_resultado' => 0];
    $busquedas    = (int)$bus['busquedas'];
    $sinResultado = (int)$bus['sin_resultado'];
    $tasaRecon    = $busquedas > 0 ? round(($busquedas - $sinResultado) * 100 / $busquedas, 1) : 0;

    $stmt = $dbConn->prepare("
        SELECT COALESCE(SUM(CASE WHEN texto_normalizado IS NOT NULL
                                  AND texto_normalizado <> texto THEN 1 ELSE 0 END), 0) AS normalizadas
        FROM qo_transcripciones
        WHERE fecha BETWEEN :desde AND :hasta
    ");
    foreach ($params as $k => $v) $stmt->bindValue($k, $v);
    $stmt->execute();
    $normalizadas = (int)($stmt->fetchColumn() ?: 0);

    return [
        'desde'                 => $desde,
        'hasta'                 => $hasta,
        'idEstablecimiento'     => $idEst,
        'llamadas_totales'      => $total,
        'convertidas_en_pedido' => (int)$agg['convertidas'],
        'tasa_conversion_pct'   => $tasa,
        'por_estado'            => $porEstado,
        'coste_total_eur'       => round((float)$agg['coste_total'], 2),
        'duracion_media_seg'    => (int)round((float)$agg['duracion_media']),
        'pico_concurrencia'     => $pico,
        'heatmap'               => $heatmap,
        // Reconocimiento de acento (P5)
        'busquedas_menu'        => $busquedas,
        'busquedas_sin_resultado' => $sinResultado,
        'tasa_reconocimiento_pct' => $tasaRecon,
        'transcripciones_normalizadas' => $normalizadas,
        'generado'              => date('c')
    ];
}

try {
    $cacheKey  = "dashboard:$desde:$hasta:" . ($idEst ?? 'all');
    $resultado = agente_cache($cacheKey, 300, fn() => calcular_metricas($db, $desde, $hasta, $idEst));
    echo json_encode($resultado);
} catch (Exception $e) {
    agente_log('error', 'dashboard_error', ['err' => $e->getMessage()]);
    http_response_code(500);
    echo json_encode(['error' => 'Internal error', 'detalle' => $e->getMessage()]);
}
