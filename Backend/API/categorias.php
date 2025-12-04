<?php
/**
 * API de Categorías - VERSIÓN OPTIMIZADA
 *
 * Mejoras implementadas:
 * 1. Caché de resultados en memoria (APCu si disponible)
 * 2. Consultas SQL optimizadas con campos específicos
 * 3. Headers de caché HTTP para reducir peticiones
 * 4. Conexión persistente a BD
 */

include "config.php";
include "utils.php";

// Configuración de caché
define('CACHE_TTL', 300); // 5 minutos
define('USE_CACHE', function_exists('apcu_fetch'));

/**
 * Obtiene datos de caché o ejecuta la consulta
 */
function getCachedOrQuery($cacheKey, $dbConn, $sql, $params = [], $ttl = CACHE_TTL) {
    // Intentar obtener de caché
    if (USE_CACHE) {
        $cached = apcu_fetch($cacheKey, $success);
        if ($success) {
            return $cached;
        }
    }

    // Ejecutar consulta
    $stmt = $dbConn->prepare($sql);
    foreach ($params as $key => $value) {
        $stmt->bindValue($key, $value);
    }
    $stmt->execute();
    $stmt->setFetchMode(PDO::FETCH_ASSOC);
    $result = $stmt->fetchAll();

    // Guardar en caché
    if (USE_CACHE && !empty($result)) {
        apcu_store($cacheKey, $result, $ttl);
    }

    return $result;
}

/**
 * Establece headers de caché HTTP
 */
function setCacheHeaders($maxAge = 300) {
    header("Cache-Control: public, max-age=$maxAge");
    header("Expires: " . gmdate("D, d M Y H:i:s", time() + $maxAge) . " GMT");
    header("Vary: Accept-Encoding");
}

// Conexión persistente a BD (reutiliza conexiones)
$dbConn = connect($db);

if ($_SERVER['REQUEST_METHOD'] == 'GET') {

    // Headers comunes
    header("Content-Type: application/json; charset=utf-8");

    if (isset($_GET['idEstablecimiento'])) {
        $idEst = intval($_GET['idEstablecimiento']);
        $cacheKey = "categorias_est_$idEst";

        // CONSULTA OPTIMIZADA:
        // - Campos específicos en lugar de SELECT *
        // - Subquery optimizada con índices
        // - Evita JOIN innecesario calculando contador con COUNT directo
        $sql = "SELECT
                    c.navidad, c.espuntos, c.imagen, c.orden, c.numeroImpresora,
                    c.id, c.nombre, c.nombre_eng, c.nombre_fr, c.nombre_ger,
                    c.idEstablecimiento,
                    IF(c.tipo=0,'Bebidas','Comidas') as tipo,
                    c.estado, c.tipo as idTipo,
                    COALESCE((SELECT COUNT(*) FROM qo_productos_est WHERE idCategoria = c.id AND eliminado = 0), 0) as numeroProductos
                FROM qo_productos_cat c
                WHERE c.idEstablecimiento = :id
                ORDER BY c.orden";

        $result = getCachedOrQuery($cacheKey, $dbConn, $sql, [':id' => $idEst]);

        setCacheHeaders(300); // 5 minutos
        header("HTTP/1.1 200 OK");
        echo json_encode($result);
        exit();
    }

    if (isset($_GET['home'])) {
        $cacheKey = "categorias_home";

        // Solo campos necesarios
        $sql = "SELECT id, nombre, nombre_eng, nombre_ger, nombre_fr, estado, imagen
                FROM qo_productos_cat
                WHERE estado = 1";

        $result = getCachedOrQuery($cacheKey, $dbConn, $sql, [], 600); // 10 min cache

        setCacheHeaders(600);
        header("HTTP/1.1 200 OK");
        echo json_encode($result);
        exit();
    }
}

// POST - Invalidar caché al crear
if ($_SERVER['REQUEST_METHOD'] == 'POST') {
    $input = json_decode(file_get_contents('php://input'), true);

    
        $sql = "INSERT INTO qo_productos_cat (orden, imagen, numeroImpresora, nombre, nombre_eng, nombre_fr, nombre_ger, estado, idEstablecimiento, tipo)
                VALUES (:orden, :imagen, :numeroImpresora, :nombre, :nombre, :nombre, :nombre, :estado, :idEstablecimiento, :tipo)";

        $numeroImpresora = $input['numeroImpresora'] ?? 1;

        $statement = $dbConn->prepare($sql);
        $statement->bindValue(':nombre', $input['nombre']);
        $statement->bindValue(':orden', $input['orden']);
        $statement->bindValue(':imagen', $input['imagen']);
        $statement->bindValue(':numeroImpresora', $numeroImpresora);
        $statement->bindValue(':estado', $input['estado']);
        $statement->bindValue(':idEstablecimiento', $input['idEstablecimiento']);
        $statement->bindValue(':tipo', $input['idTipo']);
        $statement->execute();

        $postId = $dbConn->lastInsertId();

        // Invalidar caché
        if (USE_CACHE) {
            apcu_delete("categorias_est_{$input['idEstablecimiento']}");
        }

        if ($postId) {
            $input['id'] = $postId;
            header("HTTP/1.1 200 OK");
            echo json_encode($input);
            exit();
        }
}

// DELETE
if ($_SERVER['REQUEST_METHOD'] == 'DELETE') {
    if (isset($_GET['id'])) {
        $id = intval($_GET['id']);

        // Obtener idEstablecimiento antes de borrar para invalidar caché
        $stmt = $dbConn->prepare("SELECT idEstablecimiento FROM qo_productos_cat WHERE id = :id");
        $stmt->bindValue(':id', $id);
        $stmt->execute();
        $cat = $stmt->fetch(PDO::FETCH_ASSOC);

        $statement = $dbConn->prepare("DELETE FROM qo_productos_cat WHERE id = :id");
        $statement->bindValue(':id', $id);
        $statement->execute();

        // Invalidar caché
        if (USE_CACHE && $cat) {
            apcu_delete("categorias_est_{$cat['idEstablecimiento']}");
        }

        header("HTTP/1.1 200 OK");
        exit();
    }
}

// PUT - Actualizar
if ($_SERVER['REQUEST_METHOD'] == 'PUT') {
    $input = json_decode(file_get_contents('php://input'), true);

        $id = intval($input['id']);
        $numeroImpresora = $input['numeroImpresora'] ?? 1;

        $sql = "UPDATE qo_productos_cat SET
                    numeroImpresora = :numeroImpresora,
                    nombre = :nombre,
                    nombre_eng = :nombre,
                    nombre_fr = :nombre,
                    nombre_ger = :nombre,
                    tipo = :tipo,
                    estado = :estado,
                    idEstablecimiento = :idEstablecimiento,
                    imagen = :imagen,
                    orden = :orden
                WHERE id = :id";

        $statement = $dbConn->prepare($sql);
        $statement->bindValue(':nombre', $input['nombre']);
        $statement->bindValue(':orden', $input['orden']);
        $statement->bindValue(':imagen', $input['imagen']);
        $statement->bindValue(':numeroImpresora', $numeroImpresora);
        $statement->bindValue(':tipo', $input['idTipo']);
        $statement->bindValue(':estado', $input['estado']);
        $statement->bindValue(':idEstablecimiento', $input['idEstablecimiento']);
        $statement->bindValue(':id', $id);
        $statement->execute();

        // Invalidar caché
        if (USE_CACHE) {
            apcu_delete("categorias_est_{$input['idEstablecimiento']}");
        }

    header("HTTP/1.1 200 OK");
    exit();
}

header("HTTP/1.1 400 Bad Request");
?>
