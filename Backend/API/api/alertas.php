<?php
/**
 * Alertas operativas del agente.
 *
 *  Comprueba en la última hora:
 *  - Tasa de error >10% (sample mínimo 10 llamadas).
 *  - Coste diario acumulado > PRESUPUESTO_DIARIO_EUR.
 *  - 5+ llamadas fallidas consecutivas.
 *  - Pico de concurrencia alcanzado el límite de Vapi (10).
 *
 *  Cada alerta se dispara sólo si no se ha mandado ya en la última hora
 *  (deduplicación con Redis para no inundar el email).
 *
 *  CLI:  php alertas.php          # ejecuta
 *
 *  Crontab sugerido (cada 5 min):
 *      cada 5 min, www-data, php /var/www/qoorder/pa_ws/api/alertas.php
 */

declare(strict_types=1);

if (PHP_SAPI !== 'cli') {
    http_response_code(403);
    exit('Solo CLI');
}

include __DIR__ . "/../config.php";
include __DIR__ . "/../utils.php";
require_once __DIR__ . "/_lib.php";

$dbConn = connect($db);
$redis  = agente_redis();

/** Dispara una alerta sólo si no se mandó la misma en los últimos $ttl segundos. */
function disparar(string $clave, string $titulo, string $cuerpo, array $ctx = [], int $ttl = 3600): void {
    global $redis;
    $k = "alerta:cooldown:{$clave}";
    if ($redis) {
        if ($redis->get($k) !== false) return; // ya disparada
        $redis->setex($k, $ttl, '1');
    }
    agente_alert($titulo, $cuerpo, $ctx);
    agente_log('warn', 'alerta_disparada', ['clave' => $clave, 'titulo' => $titulo]);
}

// ───── 1. Tasa de error última hora ──────────────────────────────────
$row = $dbConn->query("
    SELECT
      COUNT(*)                                                      AS total,
      SUM(CASE WHEN estado IN ('fallida','no_pedido') THEN 1 ELSE 0 END) AS fallos
    FROM qo_llamadas
    WHERE fecha_inicio >= DATE_SUB(NOW(), INTERVAL 1 HOUR)
")->fetch(PDO::FETCH_ASSOC);
$total = (int)($row['total'] ?? 0);
$fall  = (int)($row['fallos'] ?? 0);
if ($total >= 10) {
    $pct = round($fall * 100 / $total, 1);
    if ($pct > 10) {
        disparar(
            "error_rate",
            "Tasa de error alta: {$pct}%",
            "En la última hora: {$total} llamadas, {$fall} fallidas/no_pedido ({$pct}%).",
            ['total' => $total, 'fallos' => $fall, 'pct' => $pct]
        );
    }
}

// ───── 2. Presupuesto diario superado ────────────────────────────────
$presupuesto = (float)(agente_env('PRESUPUESTO_DIARIO_EUR', '50'));
$gasto = (float)$dbConn->query("
    SELECT COALESCE(SUM(coste_estimado), 0)
    FROM qo_llamadas
    WHERE DATE(fecha_inicio) = CURDATE()
")->fetchColumn();
if ($gasto > $presupuesto) {
    disparar(
        "presupuesto_superado",
        "Presupuesto diario superado",
        "Gasto hoy: " . number_format($gasto, 2) . " USD (presupuesto: {$presupuesto}).",
        ['gasto' => $gasto, 'presupuesto' => $presupuesto],
        7200
    );
}

// ───── 3. Fallidas consecutivas ──────────────────────────────────────
$ultimas = $dbConn->query("
    SELECT estado FROM qo_llamadas
    ORDER BY fecha_inicio DESC LIMIT 5
")->fetchAll(PDO::FETCH_COLUMN);
if (count($ultimas) === 5) {
    $todasMal = count(array_filter($ultimas, fn($e) => in_array($e, ['fallida','no_pedido']))) === 5;
    if ($todasMal) {
        disparar(
            "fallidas_consecutivas",
            "5 llamadas fallidas seguidas",
            "Las últimas 5 llamadas han sido fallidas o sin pedido. Posible problema con el agente.",
            ['ultimas' => $ultimas]
        );
    }
}

// ───── 4. Pico de concurrencia ───────────────────────────────────────
$pico = (int)$dbConn->query("
    SELECT MAX(concurrentes) FROM (
        SELECT (SELECT COUNT(*) FROM qo_llamadas l2
                WHERE l2.fecha_inicio <= l1.fecha_inicio
                  AND (l2.fecha_fin IS NULL OR l2.fecha_fin >= l1.fecha_inicio)
                  AND l2.fecha_inicio >= DATE_SUB(NOW(), INTERVAL 1 DAY)
               ) AS concurrentes
        FROM qo_llamadas l1
        WHERE l1.fecha_inicio >= DATE_SUB(NOW(), INTERVAL 1 DAY)
    ) sub
")->fetchColumn();
if ($pico >= 10) {
    disparar(
        "limite_concurrencia",
        "Pico de concurrencia: {$pico}",
        "Se han alcanzado {$pico} llamadas simultáneas en las últimas 24h. "
        . "Límite Vapi del plan: 10. Considerar ampliar (10 USD/mes por línea).",
        ['pico' => $pico],
        86400
    );
}

echo json_encode([
    'ok' => true,
    'metricas' => [
        'total_1h' => $total,
        'fallos_1h' => $fall,
        'gasto_hoy' => $gasto,
        'pico_24h' => $pico
    ]
]) . PHP_EOL;
