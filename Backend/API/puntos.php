<?php
/**
 * API de Puntos - VERSIÓN OPTIMIZADA
 *
 * Mejoras implementadas:
 * 1. Prepared statements seguros (sin SQL injection)
 * 2. Campos específicos en lugar de SELECT *
 * 3. Caché para consultas frecuentes
 * 4. Validación de entrada
 */

include "config.php";
include "utils.php";

// Configuración de caché
define('CACHE_TTL', 60); // 1 minuto (los puntos cambian frecuentemente)
define('USE_CACHE', function_exists('apcu_fetch'));

function getCachedOrQuery($cacheKey, $dbConn, $sql, $params = [], $ttl = CACHE_TTL) {
    if (USE_CACHE) {
        $cached = apcu_fetch($cacheKey, $success);
        if ($success) return $cached;
    }

    $stmt = $dbConn->prepare($sql);
    foreach ($params as $key => $value) {
        $stmt->bindValue($key, $value);
    }
    $stmt->execute();
    $result = $stmt->fetch(PDO::FETCH_ASSOC);

    if (USE_CACHE && $result !== false) {
        apcu_store($cacheKey, $result, $ttl);
    }

    return $result;
}

function invalidateCache($idUsuario, $idEstablecimiento) {
    if (USE_CACHE) {
        apcu_delete("puntos_{$idUsuario}_{$idEstablecimiento}");
    }
}

$dbConn = connect($db);

// ==========================================================================
// GET - Obtener puntos de usuario
// ==========================================================================
if ($_SERVER['REQUEST_METHOD'] == 'GET') {
    header("Content-Type: application/json; charset=utf-8");

    if (isset($_GET['puntosEstablecimiento'])) {
        // VALIDACIÓN: Convertir a enteros para evitar inyección
        $idUsuario = intval($_GET['idUsuario']);
        $idEstablecimiento = intval($_GET['idEstablecimiento']);

        if ($idUsuario <= 0 || $idEstablecimiento <= 0) {
            header("HTTP/1.1 400 Bad Request");
            echo json_encode(['error' => 'Parámetros inválidos']);
            exit();
        }

        $cacheKey = "puntos_{$idUsuario}_{$idEstablecimiento}";

        // OPTIMIZACIÓN: Solo campo necesario, no SELECT *
        $sql = "SELECT puntos as resultado
                FROM qo_puntos_usuario
                WHERE idEstablecimiento = :idEstablecimiento
                AND idUsuario = :idUsuario";

        $result = getCachedOrQuery($cacheKey, $dbConn, $sql, [
            ':idUsuario' => $idUsuario,
            ':idEstablecimiento' => $idEstablecimiento
        ]);

        header("HTTP/1.1 200 OK");
        echo json_encode($result ?: ['resultado' => 0]);
        exit();
    }
}

// ==========================================================================
// DELETE - Eliminar (mantiene lógica original pero con prepared statement)
// ==========================================================================
if ($_SERVER['REQUEST_METHOD'] == 'DELETE') {
    $id = intval($_GET['id']);

    if ($id <= 0) {
        header("HTTP/1.1 400 Bad Request");
        echo json_encode(['error' => 'ID inválido']);
        exit();
    }

    // SEGURIDAD: Prepared statement
    $statement = $dbConn->prepare("DELETE FROM qo_users WHERE id = :id");
    $statement->bindValue(':id', $id);
    $statement->execute();

    header("HTTP/1.1 200 OK");
    exit();
}

// ==========================================================================
// PUT - Restar puntos
// ==========================================================================
if ($_SERVER['REQUEST_METHOD'] == 'PUT') {
    $input = json_decode(file_get_contents('php://input'), true);

    // VALIDACIÓN
    $puntos = intval($input['puntos'] ?? 0);
    $idUsuario = intval($input['idUsuario'] ?? 0);
    $idEstablecimiento = intval($input['idEstablecimiento'] ?? 0);

    if ($puntos <= 0 || $idUsuario <= 0 || $idEstablecimiento <= 0) {
        header("HTTP/1.1 400 Bad Request");
        echo json_encode(['error' => 'Parámetros inválidos']);
        exit();
    }

    // SEGURIDAD: Prepared statement con parámetros
    $sql = $dbConn->prepare("UPDATE qo_puntos_usuario
                              SET puntos = puntos - :puntos
                              WHERE idEstablecimiento = :idEstablecimiento
                              AND idUsuario = :idUsuario");
    $sql->bindValue(':puntos', $puntos);
    $sql->bindValue(':idUsuario', $idUsuario);
    $sql->bindValue(':idEstablecimiento', $idEstablecimiento);
    $sql->execute();

    // Invalidar caché
    invalidateCache($idUsuario, $idEstablecimiento);

    header("HTTP/1.1 200 OK");
    exit();
}

// ==========================================================================
// POST - Agregar puntos (basado en último pedido)
// ==========================================================================
if ($_SERVER['REQUEST_METHOD'] == 'POST') {
    $input = json_decode(file_get_contents('php://input'), true);

    // VALIDACIÓN
    $idUsuario = intval($input['idUsuario'] ?? 0);
    $idEstablecimiento = intval($input['idEstablecimiento'] ?? 0);

    if ($idUsuario <= 0 || $idEstablecimiento <= 0) {
        header("HTTP/1.1 400 Bad Request");
        echo json_encode(['error' => 'Parámetros inválidos']);
        exit();
    }

    // OPTIMIZACIÓN: Verificar existencia con COUNT (más rápido que SELECT *)
    $sql = $dbConn->prepare("SELECT COUNT(*) as existe
                              FROM qo_puntos_usuario
                              WHERE idEstablecimiento = :idEstablecimiento
                              AND idUsuario = :idUsuario");
    $sql->bindValue(':idUsuario', $idUsuario);
    $sql->bindValue(':idEstablecimiento', $idEstablecimiento);
    $sql->execute();
    $existe = $sql->fetch(PDO::FETCH_ASSOC)['existe'] > 0;

    // Calcular puntos del último pedido
    $sql = $dbConn->prepare("SELECT SUM(d.cantidad * d.precio) AS total
                              FROM qo_pedidos_detalle d
                              WHERE d.idPedido = (
                                  SELECT p.id
                                  FROM qo_pedidos p
                                  WHERE p.idUsuario = :idUsuario
                                  ORDER BY p.id DESC
                                  LIMIT 1
                              )");
    $sql->bindValue(':idUsuario', $idUsuario);
    $sql->execute();

    $total = $sql->fetchColumn();
    $nuevosPuntos = (int)($total * 2);

    if ($existe) {
        // Actualizar puntos existentes
        $sql = $dbConn->prepare("UPDATE qo_puntos_usuario
                                  SET puntos = puntos + :puntos
                                  WHERE idEstablecimiento = :idEstablecimiento
                                  AND idUsuario = :idUsuario");
    } else {
        // Insertar nuevo registro
        $sql = $dbConn->prepare("INSERT INTO qo_puntos_usuario (puntos, idUsuario, idEstablecimiento)
                                  VALUES (:puntos, :idUsuario, :idEstablecimiento)");
    }

    $sql->bindValue(':puntos', $nuevosPuntos);
    $sql->bindValue(':idUsuario', $idUsuario);
    $sql->bindValue(':idEstablecimiento', $idEstablecimiento);
    $sql->execute();

    // Invalidar caché
    invalidateCache($idUsuario, $idEstablecimiento);

    header("HTTP/1.1 200 OK");
    echo json_encode(['puntos_agregados' => $nuevosPuntos]);
    exit();
}

header("HTTP/1.1 400 Bad Request");
?>
