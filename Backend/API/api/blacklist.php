<?php
/**
 * Blacklist de números de teléfono.
 *
 *  GET  /api/blacklist.php                  → lista todos
 *  POST /api/blacklist.php  body JSON       → añade { telefono, motivo, bloqueado_por }
 *  DELETE /api/blacklist.php?telefono=...   → elimina
 */

include __DIR__ . "/../config.php";
include __DIR__ . "/../utils.php";

header("Content-Type: application/json; charset=utf-8");
$dbConn = connect($db);
$method = $_SERVER['REQUEST_METHOD'];

try {
    if ($method === 'GET') {
        $stmt = $dbConn->query("
            SELECT telefono, motivo, bloqueado_por, fecha
            FROM qo_blacklist_telefonos
            ORDER BY fecha DESC
        ");
        echo json_encode($stmt->fetchAll(PDO::FETCH_ASSOC));
        exit();
    }

    if ($method === 'POST') {
        $input = json_decode(file_get_contents('php://input'), true);
        if (!is_array($input)) {
            http_response_code(400);
            echo json_encode(['error' => 'JSON invalido']);
            exit();
        }
        $telefono = preg_replace('/[^0-9+]/', '', trim($input['telefono'] ?? ''));
        $motivo   = trim($input['motivo'] ?? '');
        $por      = trim($input['bloqueado_por'] ?? 'admin');

        if ($telefono === '') {
            http_response_code(400);
            echo json_encode(['error' => 'telefono requerido']);
            exit();
        }

        $stmt = $dbConn->prepare("
            INSERT INTO qo_blacklist_telefonos (telefono, motivo, bloqueado_por, fecha)
            VALUES (:tel, :mot, :por, NOW())
            ON DUPLICATE KEY UPDATE
                motivo = VALUES(motivo),
                bloqueado_por = VALUES(bloqueado_por),
                fecha = NOW()
        ");
        $stmt->bindValue(':tel', $telefono);
        $stmt->bindValue(':mot', $motivo);
        $stmt->bindValue(':por', $por);
        $stmt->execute();
        echo json_encode(['ok' => true, 'telefono' => $telefono]);
        exit();
    }

    if ($method === 'DELETE') {
        $telefono = preg_replace('/[^0-9+]/', '', trim($_GET['telefono'] ?? ''));
        if ($telefono === '') {
            http_response_code(400);
            echo json_encode(['error' => 'telefono requerido']);
            exit();
        }
        // Tolerante: match exacto o por sufijo (últimos 9 dígitos) para sortear
        // el caso de URL "+34..." que llega como " 34..." sin URL-encode.
        $digitos = preg_replace('/\D/', '', $telefono);
        $stmt = $dbConn->prepare("
            DELETE FROM qo_blacklist_telefonos
            WHERE telefono = :exacto
               OR telefono LIKE :sufijo
        ");
        $stmt->bindValue(':exacto', $telefono);
        $stmt->bindValue(':sufijo', '%' . substr($digitos, -9));
        $stmt->execute();
        echo json_encode(['ok' => true, 'eliminados' => $stmt->rowCount()]);
        exit();
    }

    http_response_code(405);
    echo json_encode(['error' => 'Method not allowed']);

} catch (Exception $e) {
    error_log('[blacklist] ' . $e->getMessage());
    http_response_code(500);
    echo json_encode(['error' => 'Internal error']);
}
