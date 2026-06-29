<?php
/**
 * GET /api/slots-recogida.php?idEstablecimiento=67&minutos=15
 *
 * Devuelve los próximos slots de recogida (huecos de 15 min por defecto)
 * con su capacidad y plazas libres según la carga real de cocina.
 *
 * Llamado por la herramienta `get_slots_recogida` del agente.
 */

include __DIR__ . "/../config.php";
include __DIR__ . "/../utils.php";

header("Content-Type: application/json; charset=utf-8");

if ($_SERVER['REQUEST_METHOD'] !== 'GET') {
    http_response_code(405);
    echo json_encode(['error' => 'Method not allowed']);
    exit();
}

$idEst    = intval($_GET['idEstablecimiento'] ?? 0);
$minutos  = intval($_GET['minutos'] ?? 15);          // tamaño del slot
$cantidad = intval($_GET['cantidad'] ?? 8);          // nº de slots a devolver

if ($idEst <= 0) {
    http_response_code(400);
    echo json_encode(['error' => 'idEstablecimiento invalido']);
    exit();
}
if ($minutos < 5 || $minutos > 60)  $minutos  = 15;
if ($cantidad < 1 || $cantidad > 24) $cantidad = 8;

$dbConn = connect($db);

// Configuración del agente
$cfgQuery = $dbConn->prepare("SELECT clave, valor FROM qo_config_agente WHERE clave IN (
    'tiempo_preparacion_base_minutos',
    'tiempo_extra_por_pollo',
    'buffer_seguridad_minutos',
    'modo_saturacion_umbral'
)");
$cfgQuery->execute();
$cfg = [];
foreach ($cfgQuery->fetchAll(PDO::FETCH_ASSOC) as $row) {
    $cfg[$row['clave']] = json_decode($row['valor'], true);
}
$tiempoBase   = (int)($cfg['tiempo_preparacion_base_minutos'] ?? 20);
$tiempoExtra  = (int)($cfg['tiempo_extra_por_pollo']          ?? 5);
$buffer       = (int)($cfg['buffer_seguridad_minutos']        ?? 5);
$capacidad    = (int)($cfg['modo_saturacion_umbral']          ?? 10);

// Carga actual de cocina
$sql = $dbConn->prepare("
    SELECT COALESCE(cantidad, 0)
    FROM qo_contador_pollos
    WHERE idEstablecimiento = :id AND fecha = CURDATE()
    LIMIT 1
");
$sql->bindValue(':id', $idEst, PDO::PARAM_INT);
$sql->execute();
$pollosActuales = (int)($sql->fetchColumn() ?: 0);

// Pedidos pendientes (no entregados) que se cocinan en los próximos 90 min
$sql = $dbConn->prepare("
    SELECT horaEntrega, COUNT(*) AS n
    FROM qo_pedidos
    WHERE idEstablecimiento = :id
      AND anulado = 0
      AND completo = 0
      AND horaEntrega BETWEEN NOW() AND DATE_ADD(NOW(), INTERVAL 90 MINUTE)
    GROUP BY horaEntrega
");
$sql->bindValue(':id', $idEst, PDO::PARAM_INT);
$sql->execute();

$cargaPorSlot = []; // clave = 'YYYY-mm-dd HH:MM', valor = nº pedidos
foreach ($sql->fetchAll(PDO::FETCH_ASSOC) as $r) {
    $cargaPorSlot[$r['horaEntrega']] = (int)$r['n'];
}

date_default_timezone_set('Europe/Madrid');
$ahora = new DateTime('now');

// Hora del primer slot: ahora + tiempo de preparación base + buffer + extra por carga actual
$arranque = clone $ahora;
$arranque->modify('+' . ($tiempoBase + $buffer + $pollosActuales * $tiempoExtra) . ' minutes');

// Redondeo al siguiente múltiplo de `minutos`
$minutoActual = (int)$arranque->format('i');
$resto = $minutoActual % $minutos;
if ($resto !== 0) {
    $arranque->modify('+' . ($minutos - $resto) . ' minutes');
}
$arranque->setTime((int)$arranque->format('H'), (int)$arranque->format('i'), 0);

$slots = [];
for ($i = 0; $i < $cantidad; $i++) {
    $horaSlot = (clone $arranque)->modify('+' . ($i * $minutos) . ' minutes');
    $key = $horaSlot->format('Y-m-d H:i:s');
    $ocupado = $cargaPorSlot[$key] ?? 0;
    $libre   = max(0, $capacidad - $ocupado);
    $slots[] = [
        'hora'       => $horaSlot->format('c'),
        'horaCorta'  => $horaSlot->format('H:i'),
        'capacidad'  => $capacidad,
        'ocupados'   => $ocupado,
        'libre'      => $libre,
        'disponible' => $libre > 0
    ];
}

http_response_code(200);
echo json_encode([
    'idEstablecimiento' => $idEst,
    'cargaActual'       => $pollosActuales,
    'slotMinutos'       => $minutos,
    'slots'             => $slots
]);
