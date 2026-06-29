<?php
/**
 * Configuración del agente (clave/valor JSON).
 *
 *  GET /api/config-agente.php              → devuelve todas las claves como objeto plano
 *  GET /api/config-agente.php?clave=X      → devuelve solo esa clave
 *  PUT /api/config-agente.php              → body { clave1: valor, clave2: valor }  (merge upsert)
 */

include __DIR__ . "/../config.php";
include __DIR__ . "/../utils.php";

header("Content-Type: application/json; charset=utf-8");
$dbConn = connect($db);
$method = $_SERVER['REQUEST_METHOD'];

try {
    if ($method === 'GET') {
        if (isset($_GET['clave'])) {
            $stmt = $dbConn->prepare("SELECT valor FROM qo_config_agente WHERE clave = :c");
            $stmt->bindValue(':c', $_GET['clave']);
            $stmt->execute();
            $raw = $stmt->fetchColumn();
            if ($raw === false) {
                http_response_code(404);
                echo json_encode(['error' => 'clave no encontrada']);
                exit();
            }
            echo json_encode(['clave' => $_GET['clave'], 'valor' => json_decode($raw, true)]);
            exit();
        }

        $stmt = $dbConn->query("SELECT clave, valor, descripcion, fecha_actualizacion FROM qo_config_agente ORDER BY clave");
        $out = [];
        foreach ($stmt->fetchAll(PDO::FETCH_ASSOC) as $row) {
            $out[$row['clave']] = [
                'valor'                => json_decode($row['valor'], true),
                'descripcion'          => $row['descripcion'],
                'fecha_actualizacion'  => $row['fecha_actualizacion']
            ];
        }
        echo json_encode($out);
        exit();
    }

    if ($method === 'PUT') {
        $input = json_decode(file_get_contents('php://input'), true);
        if (!is_array($input) || count($input) === 0) {
            http_response_code(400);
            echo json_encode(['error' => 'Body JSON requerido con al menos una clave']);
            exit();
        }

        $stmt = $dbConn->prepare("
            INSERT INTO qo_config_agente (clave, valor)
            VALUES (:c, :v)
            ON DUPLICATE KEY UPDATE valor = VALUES(valor), fecha_actualizacion = NOW()
        ");
        $actualizadas = [];
        foreach ($input as $clave => $valor) {
            $stmt->bindValue(':c', $clave);
            $stmt->bindValue(':v', json_encode($valor, JSON_UNESCAPED_UNICODE));
            $stmt->execute();
            $actualizadas[] = $clave;
        }
        echo json_encode(['ok' => true, 'actualizadas' => $actualizadas]);
        exit();
    }

    http_response_code(405);
    echo json_encode(['error' => 'Method not allowed']);

} catch (Exception $e) {
    error_log('[config-agente] ' . $e->getMessage());
    http_response_code(500);
    echo json_encode(['error' => 'Internal error']);
}
