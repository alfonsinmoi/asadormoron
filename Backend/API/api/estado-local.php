<?php
/**
 * GET /api/estado-local.php?idEstablecimiento=67
 *
 * Estado agregado del local que el agente necesita conocer antes de aceptar
 * un pedido: si está abierto, si está saturado, próxima apertura.
 *
 * Llamado por la herramienta `get_estado_local` del agente.
 */

include __DIR__ . "/../config.php";
include __DIR__ . "/../utils.php";

header("Content-Type: application/json; charset=utf-8");

if ($_SERVER['REQUEST_METHOD'] !== 'GET') {
    http_response_code(405);
    echo json_encode(['error' => 'Method not allowed']);
    exit();
}

$idEst = intval($_GET['idEstablecimiento'] ?? 0);
if ($idEst <= 0) {
    http_response_code(400);
    echo json_encode(['error' => 'idEstablecimiento invalido']);
    exit();
}

$dbConn = connect($db);

// 1. Carga actual de cocina (contador de pollos del día)
$sql = $dbConn->prepare("
    SELECT COALESCE(cantidad, 0) AS cantidad
    FROM qo_contador_pollos
    WHERE idEstablecimiento = :id AND fecha = CURDATE()
    LIMIT 1
");
$sql->bindValue(':id', $idEst, PDO::PARAM_INT);
$sql->execute();
$pollos = (int)($sql->fetchColumn() ?: 0);

// 2. Configuración del agente para conocer el umbral de saturación
$sql = $dbConn->prepare("SELECT valor FROM qo_config_agente WHERE clave = 'modo_saturacion_umbral'");
$sql->execute();
$umbralRaw = $sql->fetchColumn();
$umbralSaturacion = $umbralRaw !== false ? (int)json_decode($umbralRaw, true) : 10;

// 3. Horario del establecimiento (de qo_config_agente.horario)
$sql = $dbConn->prepare("SELECT valor FROM qo_config_agente WHERE clave = 'horario'");
$sql->execute();
$horarioJson = $sql->fetchColumn();
$horario = $horarioJson ? json_decode($horarioJson, true) : null;

date_default_timezone_set('Europe/Madrid');
$ahora = new DateTime('now');
$diaSemana = ['domingo','lunes','martes','miercoles','jueves','viernes','sabado'][(int)$ahora->format('w')];
$horaActual = $ahora->format('H:i');

$abierto = false;
$proximaApertura = null;

if ($horario && isset($horario[$diaSemana])) {
    // El horario viene como pares [apertura, cierre, apertura, cierre]
    $franjas = $horario[$diaSemana];
    for ($i = 0; $i < count($franjas); $i += 2) {
        $apertura = $franjas[$i] ?? null;
        $cierre   = $franjas[$i + 1] ?? null;
        if ($apertura && $cierre && $horaActual >= $apertura && $horaActual < $cierre) {
            $abierto = true;
            break;
        }
        if ($apertura && $horaActual < $apertura && $proximaApertura === null) {
            $proximaApertura = $ahora->format('Y-m-d') . ' ' . $apertura . ':00';
        }
    }
}

$saturado = $pollos >= $umbralSaturacion;

$motivo = 'ok';
if (!$abierto)        $motivo = 'fuera_horario';
elseif ($saturado)    $motivo = 'saturado';

http_response_code(200);
echo json_encode([
    'idEstablecimiento' => $idEst,
    'abierto'           => $abierto,
    'saturado'          => $saturado,
    'motivo'            => $motivo,
    'cargaCocina'       => $pollos,
    'umbralSaturacion'  => $umbralSaturacion,
    'proximaApertura'   => $proximaApertura,
    'horaServidor'      => $ahora->format('c')
]);
