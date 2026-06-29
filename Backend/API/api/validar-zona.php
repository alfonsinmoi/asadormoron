<?php
/**
 * GET /api/validar-zona.php?direccion=<texto>&idPueblo=1
 *
 * Valida si una dirección está dentro de las zonas de reparto del pueblo.
 *
 * Implementación inicial (sin geocoding):
 *   - Si el texto de la dirección contiene el código postal o nombre de
 *     una zona, se considera válida y se devuelve la zona.
 *   - Si no, se devuelve `valida=false`.
 *
 * Más adelante (Fase 3) se integrará Google Maps o Nominatim para hacer
 * geocoding real con coordenadas.
 *
 * Llamado por la herramienta `validar_zona_reparto` del agente.
 */

include __DIR__ . "/../config.php";
include __DIR__ . "/../utils.php";

header("Content-Type: application/json; charset=utf-8");

if ($_SERVER['REQUEST_METHOD'] !== 'GET') {
    http_response_code(405);
    echo json_encode(['error' => 'Method not allowed']);
    exit();
}

$direccion = trim($_GET['direccion'] ?? '');
$idPueblo  = intval($_GET['idPueblo'] ?? 0);

if ($direccion === '' || $idPueblo <= 0) {
    http_response_code(400);
    echo json_encode(['error' => 'Parametros invalidos']);
    exit();
}

$dbConn = connect($db);
$sql = $dbConn->prepare("
    SELECT id AS idZona, nombre, gastos, pedidoMinimo, activo
    FROM qo_zonas
    WHERE idPueblo = :idPueblo AND activo = 1
    ORDER BY id
");
$sql->bindValue(':idPueblo', $idPueblo, PDO::PARAM_INT);
$sql->execute();
$zonas = $sql->fetchAll(PDO::FETCH_ASSOC);

if (count($zonas) === 0) {
    http_response_code(200);
    echo json_encode([
        'valida'   => false,
        'motivo'   => 'sin_zonas_configuradas',
        'idPueblo' => $idPueblo
    ]);
    exit();
}

// Matching simple: si el nombre de alguna zona aparece como palabra completa
// dentro de la dirección, asumimos que es esa.
$direccionLower = mb_strtolower($direccion, 'UTF-8');
$zonaEncontrada = null;

foreach ($zonas as $z) {
    $nombreLower = mb_strtolower($z['nombre'], 'UTF-8');
    if ($nombreLower !== '' && mb_strpos($direccionLower, $nombreLower) !== false) {
        $zonaEncontrada = $z;
        break;
    }
}

http_response_code(200);

if ($zonaEncontrada) {
    echo json_encode([
        'valida'        => true,
        'idZona'        => (int)$zonaEncontrada['idZona'],
        'nombre'        => $zonaEncontrada['nombre'],
        'gastos'        => (float)$zonaEncontrada['gastos'],
        'pedidoMinimo'  => (float)$zonaEncontrada['pedidoMinimo'],
        'metodo'        => 'match_nombre'
    ]);
} else {
    echo json_encode([
        'valida'      => false,
        'motivo'      => 'fuera_de_zona',
        'idPueblo'    => $idPueblo,
        'zonasDisponibles' => array_map(fn($z) => $z['nombre'], $zonas)
    ]);
}
