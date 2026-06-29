<?php
/**
 * GET /api/cliente.php?telefono=+34600123456&idPueblo=1
 *
 * Identifica al cliente por número de teléfono. Devuelve datos básicos
 * y resumen de su histórico de pedidos.
 *
 * Llamado por la herramienta `get_cliente_por_telefono` del agente.
 */

include __DIR__ . "/../config.php";
include __DIR__ . "/../utils.php";

header("Content-Type: application/json; charset=utf-8");

if ($_SERVER['REQUEST_METHOD'] !== 'GET') {
    http_response_code(405);
    echo json_encode(['error' => 'Method not allowed']);
    exit();
}

$telefono = trim($_GET['telefono'] ?? '');
$idPueblo = intval($_GET['idPueblo'] ?? 0);

if ($telefono === '' || $idPueblo <= 0) {
    http_response_code(400);
    echo json_encode(['error' => 'Parametros invalidos']);
    exit();
}

// Normalización mínima: quitar espacios, paréntesis y guiones. Permitir + inicial.
$telefonoNorm = preg_replace('/[^0-9+]/', '', $telefono);
// Versión sin prefijo internacional para comparar también con números locales guardados
$telefonoLocal = preg_replace('/^\+?34/', '', $telefonoNorm);

$dbConn = connect($db);

// Busca por coincidencia exacta o por sufijo (últimos 9 dígitos)
$sql = $dbConn->prepare("
    SELECT u.id AS idUsuario, u.nombre, u.apellidos, u.direccion, u.cod_postal,
           u.poblacion, u.telefono, u.idPueblo,
           (SELECT COUNT(*) FROM qo_pedidos p WHERE p.idUsuario = u.id) AS numeroPedidos,
           (SELECT MAX(p.horaPedido) FROM qo_pedidos p WHERE p.idUsuario = u.id) AS ultimoPedido,
           u.bloqueado
    FROM qo_users u
    WHERE u.idPueblo = :idPueblo
      AND (
            u.telefono = :tel_full
         OR u.telefono = :tel_local
         OR u.telefono LIKE :tel_suffix
      )
    ORDER BY u.id DESC
    LIMIT 1
");
$sql->bindValue(':idPueblo',   $idPueblo,                    PDO::PARAM_INT);
$sql->bindValue(':tel_full',   $telefonoNorm);
$sql->bindValue(':tel_local',  $telefonoLocal);
$sql->bindValue(':tel_suffix', '%' . substr($telefonoLocal, -9));
$sql->execute();
$row = $sql->fetch(PDO::FETCH_ASSOC);

http_response_code(200);

if (!$row) {
    echo json_encode([
        'encontrado' => false,
        'telefono'   => $telefonoNorm,
        'idPueblo'   => $idPueblo
    ]);
    exit();
}

$bloqueado = (int)$row['bloqueado'] === 1;

echo json_encode([
    'encontrado'     => true,
    'idUsuario'      => (int)$row['idUsuario'],
    'nombre'         => $row['nombre'],
    'apellidos'      => $row['apellidos'],
    'direccion'      => $row['direccion'],
    'codPostal'      => $row['cod_postal'],
    'poblacion'      => $row['poblacion'],
    'telefono'       => $row['telefono'],
    'idPueblo'       => (int)$row['idPueblo'],
    'numeroPedidos'  => (int)$row['numeroPedidos'],
    'ultimoPedido'   => $row['ultimoPedido'],
    'bloqueado'      => $bloqueado
]);
