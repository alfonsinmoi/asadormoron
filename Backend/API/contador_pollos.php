<?php
/**
 * API Contador de Pollos Asados
 */

error_reporting(E_ALL);
ini_set('display_errors', 0);
header("Content-Type: application/json; charset=utf-8");

try {
    include "config.php";
    $dbConn = connect($db);
} catch (Exception $e) {
    http_response_code(500);
    echo json_encode(['error' => 'Error de conexión: ' . $e->getMessage()]);
    exit();
}

// ==========================================================================
// GET - Obtener contador del día
// ==========================================================================
if ($_SERVER['REQUEST_METHOD'] == 'GET') {

    if (isset($_GET['idEstablecimiento'])) {
        $idEst = intval($_GET['idEstablecimiento']);
        $fecha = date('Y-m-d'); // Fecha de hoy

        // Buscar registro de hoy
        $sql = $dbConn->prepare("SELECT id, cantidad, fecha
                                  FROM qo_contador_pollos
                                  WHERE idEstablecimiento = :idEst AND fecha = :fecha");
        $sql->bindValue(':idEst', $idEst);
        $sql->bindValue(':fecha', $fecha);
        $sql->execute();
        $result = $sql->fetch(PDO::FETCH_ASSOC);

        if ($result) {
            echo json_encode([
                'id' => (int)$result['id'],
                'cantidad' => (int)$result['cantidad'],
                'fecha' => $result['fecha'],
                'idEstablecimiento' => $idEst
            ]);
        } else {
            // No existe registro para hoy, crear uno con cantidad 0
            $sqlInsert = $dbConn->prepare("INSERT INTO qo_contador_pollos (idEstablecimiento, fecha, cantidad)
                                           VALUES (:idEst, :fecha, 0)");
            $sqlInsert->bindValue(':idEst', $idEst);
            $sqlInsert->bindValue(':fecha', $fecha);
            $sqlInsert->execute();

            echo json_encode([
                'id' => (int)$dbConn->lastInsertId(),
                'cantidad' => 0,
                'fecha' => $fecha,
                'idEstablecimiento' => $idEst
            ]);
        }
        exit();
    }

    // Obtener histórico (últimos 30 días)
    if (isset($_GET['historico']) && isset($_GET['idEstablecimiento'])) {
        $idEst = intval($_GET['idEstablecimiento']);

        $sql = $dbConn->prepare("SELECT id, cantidad, fecha
                                  FROM qo_contador_pollos
                                  WHERE idEstablecimiento = :idEst
                                  ORDER BY fecha DESC
                                  LIMIT 30");
        $sql->bindValue(':idEst', $idEst);
        $sql->execute();
        $results = $sql->fetchAll(PDO::FETCH_ASSOC);

        echo json_encode($results);
        exit();
    }

    http_response_code(400);
    echo json_encode(['error' => 'Parámetros faltantes']);
    exit();
}

// ==========================================================================
// POST - Incrementar o decrementar contador
// ==========================================================================
if ($_SERVER['REQUEST_METHOD'] == 'POST') {

    $input = json_decode(file_get_contents('php://input'), true);

    if (!isset($input['idEstablecimiento']) || !isset($input['operacion'])) {
        http_response_code(400);
        echo json_encode(['error' => 'Parámetros faltantes: idEstablecimiento y operacion requeridos']);
        exit();
    }

    $idEst = intval($input['idEstablecimiento']);
    $operacion = $input['operacion']; // 'sumar' o 'restar'
    $cantidad = isset($input['cantidad']) ? intval($input['cantidad']) : 1;
    $fecha = date('Y-m-d');

    try {
        $dbConn->beginTransaction();

        // Verificar si existe registro para hoy
        $sql = $dbConn->prepare("SELECT id, cantidad FROM qo_contador_pollos
                                  WHERE idEstablecimiento = :idEst AND fecha = :fecha
                                  FOR UPDATE");
        $sql->bindValue(':idEst', $idEst);
        $sql->bindValue(':fecha', $fecha);
        $sql->execute();
        $result = $sql->fetch(PDO::FETCH_ASSOC);

        if ($result) {
            // Actualizar registro existente
            $nuevaCantidad = $result['cantidad'];
            if ($operacion === 'sumar') {
                $nuevaCantidad += $cantidad;
            } else if ($operacion === 'restar') {
                $nuevaCantidad = max(0, $nuevaCantidad - $cantidad); // No permitir negativos
            }

            $sqlUpdate = $dbConn->prepare("UPDATE qo_contador_pollos
                                            SET cantidad = :cantidad
                                            WHERE id = :id");
            $sqlUpdate->bindValue(':cantidad', $nuevaCantidad);
            $sqlUpdate->bindValue(':id', $result['id']);
            $sqlUpdate->execute();

            $id = $result['id'];
        } else {
            // Crear nuevo registro
            $nuevaCantidad = ($operacion === 'sumar') ? $cantidad : 0;

            $sqlInsert = $dbConn->prepare("INSERT INTO qo_contador_pollos (idEstablecimiento, fecha, cantidad)
                                           VALUES (:idEst, :fecha, :cantidad)");
            $sqlInsert->bindValue(':idEst', $idEst);
            $sqlInsert->bindValue(':fecha', $fecha);
            $sqlInsert->bindValue(':cantidad', $nuevaCantidad);
            $sqlInsert->execute();

            $id = $dbConn->lastInsertId();
        }

        $dbConn->commit();

        echo json_encode([
            'id' => (int)$id,
            'cantidad' => (int)$nuevaCantidad,
            'fecha' => $fecha,
            'idEstablecimiento' => $idEst,
            'operacion' => $operacion
        ]);

    } catch (Exception $e) {
        $dbConn->rollBack();
        http_response_code(500);
        echo json_encode(['error' => 'Error al actualizar contador: ' . $e->getMessage()]);
    }
    exit();
}

// ==========================================================================
// PUT - Establecer cantidad directamente
// ==========================================================================
if ($_SERVER['REQUEST_METHOD'] == 'PUT') {

    $input = json_decode(file_get_contents('php://input'), true);

    if (!isset($input['idEstablecimiento']) || !isset($input['cantidad'])) {
        http_response_code(400);
        echo json_encode(['error' => 'Parámetros faltantes']);
        exit();
    }

    $idEst = intval($input['idEstablecimiento']);
    $cantidad = max(0, intval($input['cantidad'])); // No permitir negativos
    $fecha = date('Y-m-d');

    // Upsert (insertar o actualizar)
    $sql = $dbConn->prepare("INSERT INTO qo_contador_pollos (idEstablecimiento, fecha, cantidad)
                              VALUES (:idEst, :fecha, :cantidad)
                              ON DUPLICATE KEY UPDATE cantidad = :cantidad2");
    $sql->bindValue(':idEst', $idEst);
    $sql->bindValue(':fecha', $fecha);
    $sql->bindValue(':cantidad', $cantidad);
    $sql->bindValue(':cantidad2', $cantidad);
    $sql->execute();

    echo json_encode([
        'cantidad' => $cantidad,
        'fecha' => $fecha,
        'idEstablecimiento' => $idEst
    ]);
    exit();
}

http_response_code(405);
echo json_encode(['error' => 'Method not allowed']);
?>
