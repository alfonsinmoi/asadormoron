<?php
/**
 * POST /api/asignar-repartidor.php   body: {"idPedido":123}
 *
 * Ejecuta el motor de asignación (Fase 3) sobre un pedido concreto: aplica las
 * reglas activas (turno + carga + round-robin) y asigna el mejor repartidor
 * disponible, notificándole por push. Complementa el "coger manual".
 *
 * Reutilizable desde la app/panel para autoasignar pedidos manuales o de la app.
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

const ID_ESTABLECIMIENTO = 67;
const ID_PUEBLO          = 1;

$input    = json_decode(file_get_contents('php://input'), true) ?: [];
$idPedido = (int)($input['idPedido'] ?? $_GET['idPedido'] ?? 0);
if ($idPedido <= 0) {
    http_response_code(400);
    echo json_encode(['error' => 'idPedido_requerido']);
    exit();
}

// idEstablecimiento/idPueblo del pedido (por si sirve a varios locales en el futuro).
$dbConn = connect($db);
$stmt = $dbConn->prepare("
    SELECT p.idEstablecimiento, e.idPueblo
    FROM qo_pedidos p JOIN qo_establecimientos e ON e.id = p.idEstablecimiento
    WHERE p.id = :id
");
$stmt->bindValue(':id', $idPedido, PDO::PARAM_INT);
$stmt->execute();
$row = $stmt->fetch(PDO::FETCH_ASSOC);
$idEst    = $row ? (int)$row['idEstablecimiento'] : ID_ESTABLECIMIENTO;
$idPueblo = $row ? (int)$row['idPueblo'] : ID_PUEBLO;

$res = asignar_repartidor($dbConn, $idPedido, $idEst, $idPueblo);
http_response_code(200);
echo json_encode($res, JSON_UNESCAPED_UNICODE);
