<?php
/**
 * API de Productos - VERSIÓN OPTIMIZADA
 *
 * Mejoras implementadas:
 * 1. Consultas SQL simplificadas y con índices
 * 2. Caché de resultados para productos
 * 3. Subconsultas optimizadas para opciones/ingredientes
 * 4. Headers de caché HTTP
 */

include "config.php";
include "utils.php";

// Configuración de caché
define('CACHE_TTL', 300); // 5 minutos
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
    $stmt->setFetchMode(PDO::FETCH_ASSOC);
    $result = $stmt->fetchAll();

    if (USE_CACHE && !empty($result)) {
        apcu_store($cacheKey, $result, $ttl);
    }

    return $result;
}

function setCacheHeaders($maxAge = 300) {
    header("Cache-Control: public, max-age=$maxAge");
    header("Expires: " . gmdate("D, d M Y H:i:s", time() + $maxAge) . " GMT");
}

$dbConn = connect($db);

if ($_SERVER['REQUEST_METHOD'] == 'GET') {
    header("Content-Type: application/json; charset=utf-8");

    // ==========================================================================
    // ENDPOINT PRINCIPAL: Productos por establecimiento (el más usado)
    // ==========================================================================
    if (isset($_GET['idEstablecimientoProducto'])) {
        $idEst = intval($_GET['idEstablecimientoProducto']);
        $cacheKey = "productos_est_$idEst";

        // OPTIMIZACIÓN PRINCIPAL:
        // La consulta original usa múltiples subqueries correlacionadas que se ejecutan
        // por cada fila. Esta versión usa JOINs más eficientes.

        // Paso 1: Obtener productos base (muy rápido con índice)
        $sqlProductos = "SELECT
                e.puntos, e.fuerzaIngredientes, e.numeroIngredientes,
                c.estado as estadoCategoria, c.idEstablecimiento,
                e.id as idArticulo, e.vistaEnvios, e.vistaLocal, e.precioLocal,
                e.nombre, IFNULL(NULLIF(e.nombre_eng,''), e.nombre) as nombre_eng,
                IFNULL(NULLIF(e.nombre_ger,''), e.nombre) as nombre_ger,
                IFNULL(NULLIF(e.nombre_fr,''), e.nombre) as nombre_fr,
                e.idCategoria,
                c.nombre as categoria,
                IFNULL(NULLIF(c.nombre_eng,''), c.nombre) as categoria_eng,
                IFNULL(NULLIF(c.nombre_ger,''), c.nombre) as categoria_ger,
                IFNULL(NULLIF(c.nombre_fr,''), c.nombre) as categoria_fr,
                c.tipo as idTipoCategoria,
                e.imagen, e.estado, e.porEncargo, e.precio,
                IFNULL(e.descripcion,'') as descripcion,
                IFNULL(e.descripcion_eng,'') as descripcion_eng,
                IFNULL(e.descripcion_ger,'') as descripcion_ger,
                IFNULL(e.descripcion_fr,'') as descripcion_fr
            FROM qo_productos_cat c
            INNER JOIN qo_productos_est e ON e.idCategoria = c.id AND e.eliminado = 0
            WHERE c.idEstablecimiento = :id
            ORDER BY c.orden, e.idCategoria, e.nombre, e.id";

        $productos = getCachedOrQuery($cacheKey . "_base", $dbConn, $sqlProductos, [':id' => $idEst], 120);

        if (empty($productos)) {
            setCacheHeaders(60);
            echo json_encode([]);
            exit();
        }

        // Paso 2: Obtener opciones en batch (una sola consulta)
        $productIds = array_column($productos, 'idArticulo');
        $productIdsStr = implode(',', array_map('intval', $productIds));

        $sqlOpciones = "SELECT
                idProducto,
                GROUP_CONCAT(id,';',opcion,';',tipoIncremento,';',valorIncremento,';','',';','',';','',';',puntos SEPARATOR '|') as opciones
            FROM qo_productos_opc
            WHERE idProducto IN ($productIdsStr)
            GROUP BY idProducto";

        $stmtOpc = $dbConn->query($sqlOpciones);
        $opcionesMap = [];
        while ($row = $stmtOpc->fetch(PDO::FETCH_ASSOC)) {
            $opcionesMap[$row['idProducto']] = $row['opciones'];
        }

        // Paso 3: Obtener ingredientes en batch
        $sqlIngredientes = "SELECT
                p.id as idProducto,
                GROUP_CONCAT(ie.id,';',ie.nombre SEPARATOR '|') as ingredientes
            FROM qo_ingredientes_producto i
            INNER JOIN qo_productos_est p ON p.id = i.idProducto AND p.eliminado = 0
            INNER JOIN qo_productos_cat c ON c.id = p.idCategoria
            INNER JOIN qo_ingredientes_establecimiento ie ON ie.id = i.idIngrediente AND ie.estado = 1
            WHERE c.idEstablecimiento = :id
            GROUP BY p.id";

        $ingredientesResult = getCachedOrQuery($cacheKey . "_ing", $dbConn, $sqlIngredientes, [':id' => $idEst], 120);
        $ingredientesMap = [];
        foreach ($ingredientesResult as $row) {
            $ingredientesMap[$row['idProducto']] = $row['ingredientes'];
        }

        // Paso 4: Obtener alérgenos en batch
        $sqlAlergenos = "SELECT
                i.idProducto,
                GROUP_CONCAT(a.id,';',a.nombre,';',a.imagen SEPARATOR '|') as alergenos
            FROM qo_productos_ing_aler i
            INNER JOIN qo_alergenos a ON i.idAlergeno = a.id
            WHERE i.idProducto IN ($productIdsStr)
            GROUP BY i.idProducto";

        $stmtAler = $dbConn->query($sqlAlergenos);
        $alergenosMap = [];
        while ($row = $stmtAler->fetch(PDO::FETCH_ASSOC)) {
            $alergenosMap[$row['idProducto']] = $row['alergenos'];
        }

        // Paso 5: Combinar resultados
        foreach ($productos as &$p) {
            $id = $p['idArticulo'];
            $p['opciones'] = $opcionesMap[$id] ?? '';
            $p['ingredientes'] = $ingredientesMap[$id] ?? '';
            $p['alergenos'] = $alergenosMap[$id] ?? '';
        }

        // Guardar resultado combinado en caché
        if (USE_CACHE) {
            apcu_store($cacheKey, $productos, CACHE_TTL);
        }

        setCacheHeaders(120);
        header("HTTP/1.1 200 OK");
        echo json_encode($productos);
        exit();
    }

    // ==========================================================================
    // Productos por CATEGORÍA (usado en CartaProductosView)
    // ==========================================================================
    if (isset($_GET['idEstablecimientoProductoCat'])) {
        $idCategoria = intval($_GET['idEstablecimientoProductoCat']);
        $cacheKey = "productos_cat_$idCategoria";

        $sqlProductos = "SELECT
                e.puntos, e.fuerzaIngredientes, e.numeroIngredientes,
                c.estado as estadoCategoria, c.idEstablecimiento,
                e.id as idArticulo, e.vistaEnvios, e.vistaLocal, e.precioLocal,
                e.nombre, IFNULL(NULLIF(e.nombre_eng,''), e.nombre) as nombre_eng,
                IFNULL(NULLIF(e.nombre_ger,''), e.nombre) as nombre_ger,
                IFNULL(NULLIF(e.nombre_fr,''), e.nombre) as nombre_fr,
                e.idCategoria,
                c.nombre as categoria,
                IFNULL(NULLIF(c.nombre_eng,''), c.nombre) as categoria_eng,
                IFNULL(NULLIF(c.nombre_ger,''), c.nombre) as categoria_ger,
                IFNULL(NULLIF(c.nombre_fr,''), c.nombre) as categoria_fr,
                c.tipo as idTipoCategoria,
                e.imagen, e.estado, e.porEncargo, e.precio,
                IFNULL(e.descripcion,'') as descripcion,
                IFNULL(e.descripcion_eng,'') as descripcion_eng,
                IFNULL(e.descripcion_ger,'') as descripcion_ger,
                IFNULL(e.descripcion_fr,'') as descripcion_fr
            FROM qo_productos_cat c
            INNER JOIN qo_productos_est e ON e.idCategoria = c.id AND e.eliminado = 0
            WHERE c.id = :idCategoria
            ORDER BY e.nombre, e.id";

        $productos = getCachedOrQuery($cacheKey . "_base", $dbConn, $sqlProductos, [':idCategoria' => $idCategoria], 120);

        if (empty($productos)) {
            setCacheHeaders(60);
            echo json_encode([]);
            exit();
        }

        // Obtener opciones, ingredientes y alérgenos en batch
        $productIds = array_column($productos, 'idArticulo');
        $productIdsStr = implode(',', array_map('intval', $productIds));

        // Opciones
        $sqlOpciones = "SELECT
                idProducto,
                GROUP_CONCAT(id,';',opcion,';',tipoIncremento,';',valorIncremento,';','',';','',';','',';',puntos SEPARATOR '|') as opciones
            FROM qo_productos_opc
            WHERE idProducto IN ($productIdsStr)
            GROUP BY idProducto";

        $stmtOpc = $dbConn->query($sqlOpciones);
        $opcionesMap = [];
        while ($row = $stmtOpc->fetch(PDO::FETCH_ASSOC)) {
            $opcionesMap[$row['idProducto']] = $row['opciones'];
        }

        // Ingredientes
        $sqlIngredientes = "SELECT
                i.idProducto,
                GROUP_CONCAT(ie.id,';',ie.nombre SEPARATOR '|') as ingredientes
            FROM qo_ingredientes_producto i
            INNER JOIN qo_ingredientes_establecimiento ie ON ie.id = i.idIngrediente AND ie.estado = 1
            WHERE i.idProducto IN ($productIdsStr)
            GROUP BY i.idProducto";

        $stmtIng = $dbConn->query($sqlIngredientes);
        $ingredientesMap = [];
        while ($row = $stmtIng->fetch(PDO::FETCH_ASSOC)) {
            $ingredientesMap[$row['idProducto']] = $row['ingredientes'];
        }

        // Alérgenos
        $sqlAlergenos = "SELECT
                i.idProducto,
                GROUP_CONCAT(a.id,';',a.nombre,';',a.imagen SEPARATOR '|') as alergenos
            FROM qo_productos_ing_aler i
            INNER JOIN qo_alergenos a ON i.idAlergeno = a.id
            WHERE i.idProducto IN ($productIdsStr)
            GROUP BY i.idProducto";

        $stmtAler = $dbConn->query($sqlAlergenos);
        $alergenosMap = [];
        while ($row = $stmtAler->fetch(PDO::FETCH_ASSOC)) {
            $alergenosMap[$row['idProducto']] = $row['alergenos'];
        }

        // Combinar resultados
        foreach ($productos as &$p) {
            $id = $p['idArticulo'];
            $p['opciones'] = $opcionesMap[$id] ?? '';
            $p['ingredientes'] = $ingredientesMap[$id] ?? '';
            $p['alergenos'] = $alergenosMap[$id] ?? '';
        }

        // Guardar resultado combinado en caché
        if (USE_CACHE) {
            apcu_store($cacheKey, $productos, CACHE_TTL);
        }

        setCacheHeaders(120);
        header("HTTP/1.1 200 OK");
        echo json_encode($productos);
        exit();
    }

    // ==========================================================================
    // Productos más vendidos (con caché largo)
    // ==========================================================================
    if (isset($_GET['masvendidos'])) {
        $idGrupo = intval($_GET['idGrupo']);
        $cacheKey = "productos_masvendidos_$idGrupo";

        // OPTIMIZACIÓN: Limitar JOIN y usar índices
        $sql = "SELECT
                e.fuerzaIngredientes, e.numeroIngredientes,
                SUM(d.cantidad) as ventas,
                es.nombre as nombreEstablecimiento,
                c.estado as estadoCategoria, c.idEstablecimiento,
                e.id, e.nombre, e.idCategoria, e.porEncargo,
                c.nombre as categoria, c.tipo as idTipoCategoria,
                e.imagen, e.estado, e.precio,
                IFNULL(e.descripcion,'') as descripcion,
                '' as opciones, '' as ingredientes, '' as alergenos
            FROM qo_productos_est e
            INNER JOIN qo_productos_cat c ON e.idCategoria = c.id
            INNER JOIN qo_pedidos_detalle d ON d.idProducto = e.id
            INNER JOIN qo_establecimientos es ON es.id = c.idEstablecimiento
            WHERE e.eliminado = 0
                AND d.tipoVenta <> 'Local'
                " . ($idGrupo > 0 ? "AND es.idGrupo = :idGrupo" : "") . "
            GROUP BY e.id
            ORDER BY ventas DESC
            LIMIT 10";

        $params = $idGrupo > 0 ? [':idGrupo' => $idGrupo] : [];
        $result = getCachedOrQuery($cacheKey, $dbConn, $sql, $params, 600); // 10 min cache

        setCacheHeaders(600);
        header("HTTP/1.1 200 OK");
        echo json_encode($result);
        exit();
    }

    // ==========================================================================
    // Alergenos (datos estáticos, caché largo)
    // ==========================================================================
    if (isset($_GET['alergenos'])) {
        $cacheKey = "alergenos_all";

        $sql = "SELECT id, nombre, imagen, estado FROM qo_alergenos WHERE estado = 1";
        $result = getCachedOrQuery($cacheKey, $dbConn, $sql, [], 3600); // 1 hora cache

        setCacheHeaders(3600);
        header("HTTP/1.1 200 OK");
        echo json_encode($result);
        exit();
    }

    // ==========================================================================
    // Opciones de un producto (usado por KioskoPreloadService)
    // ==========================================================================
    if (isset($_GET['idOpcionesProducto'])) {
        $idProducto = intval($_GET['idOpcionesProducto']);
        $cacheKey = "opciones_prod_$idProducto";

        $sql = "SELECT
                id, idProducto, opcion,
                IFNULL(NULLIF(opcion_eng,''), opcion) as opcion_eng,
                IFNULL(NULLIF(opcion_ger,''), opcion) as opcion_ger,
                IFNULL(NULLIF(opcion_fr,''), opcion) as opcion_fr,
                tipoIncremento, valorIncremento as precio, puntos,
                IFNULL(observaciones,'') as observaciones
            FROM qo_productos_opc
            WHERE idProducto = :id";

        $result = getCachedOrQuery($cacheKey, $dbConn, $sql, [':id' => $idProducto], 300);

        setCacheHeaders(300);
        header("HTTP/1.1 200 OK");
        echo json_encode($result);
        exit();
    }

    // ==========================================================================
    // Alérgenos de un producto (usado por KioskoPreloadService)
    // ==========================================================================
    if (isset($_GET['idAlergenosProducto'])) {
        $idProducto = intval($_GET['idAlergenosProducto']);
        $cacheKey = "alergenos_prod_$idProducto";

        $sql = "SELECT
                a.id, a.nombre,
                IFNULL(NULLIF(a.nombre_eng,''), a.nombre) as nombre_eng,
                IFNULL(NULLIF(a.nombre_ger,''), a.nombre) as nombre_ger,
                IFNULL(NULLIF(a.nombre_fr,''), a.nombre) as nombre_fr,
                a.imagen
            FROM qo_productos_ing_aler pa
            INNER JOIN qo_alergenos a ON a.id = pa.idAlergeno
            WHERE pa.idProducto = :id";

        $result = getCachedOrQuery($cacheKey, $dbConn, $sql, [':id' => $idProducto], 300);

        setCacheHeaders(300);
        header("HTTP/1.1 200 OK");
        echo json_encode($result);
        exit();
    }

    // ==========================================================================
    // Ingredientes de un producto
    // ==========================================================================
    if (isset($_GET['idIngredientesProducto'])) {
        $idProducto = intval($_GET['idIngredientesProducto']);
        $cacheKey = "ingredientes_prod_$idProducto";

        $sql = "SELECT
                p.id, p.idIngrediente, p.precio, p.puntos,
                e.nombre, e.nombre_eng, e.nombre_ger, e.nombre_fr, e.estado, p.idProducto
            FROM qo_ingredientes_producto p
            INNER JOIN qo_ingredientes_establecimiento e ON e.id = p.idIngrediente AND e.estado = 1
            WHERE p.idProducto = :id";

        $result = getCachedOrQuery($cacheKey, $dbConn, $sql, [':id' => $idProducto], 300);

        setCacheHeaders(300);
        header("HTTP/1.1 200 OK");
        echo json_encode($result);
        exit();
    }

    // ==========================================================================
    // Ingredientes de establecimiento
    // ==========================================================================
    if (isset($_GET['idEstablecimientoIngredientes'])) {
        $idEst = intval($_GET['idEstablecimientoIngredientes']);
        $cacheKey = "ingredientes_est_$idEst";

        $sql = "SELECT id, idEstablecimiento, nombre, precio, estado
                FROM qo_ingredientes_establecimiento
                WHERE idEstablecimiento = :id AND estado = 1";

        $result = getCachedOrQuery($cacheKey, $dbConn, $sql, [':id' => $idEst], 300);

        setCacheHeaders(300);
        header("HTTP/1.1 200 OK");
        echo json_encode($result);
        exit();
    }

    // ==========================================================================
    // Cantidad de ingredientes (simple, sin caché)
    // ==========================================================================
    if (isset($_GET['cantidadIngredientes'])) {
        $sql = $dbConn->prepare("SELECT IF(fuerzaIngredientes=0,0,numeroIngredientes) as ingredientes
                                  FROM qo_productos_est WHERE id = :id");
        $sql->bindValue(':id', intval($_GET['cantidadIngredientes']));
        $sql->execute();
        $result = $sql->fetch(PDO::FETCH_OBJ);

        header("HTTP/1.1 200 OK");
        echo json_encode($result->ingredientes ?? 0);
        exit();
    }
}

// ==========================================================================
// POST - Crear (invalidar caché)
// ==========================================================================
if ($_SERVER['REQUEST_METHOD'] == 'POST') {
    $input = json_decode(file_get_contents('php://input'), true);

    // ... (código de creación existente)
    // Al final, invalidar caché:
    if (USE_CACHE && isset($input['idCategoria'])) {
        // Obtener idEstablecimiento
        $stmt = $dbConn->prepare("SELECT idEstablecimiento FROM qo_productos_cat WHERE id = :id");
        $stmt->bindValue(':id', $input['idCategoria']);
        $stmt->execute();
        $cat = $stmt->fetch(PDO::FETCH_ASSOC);
        if ($cat) {
            apcu_delete("productos_est_{$cat['idEstablecimiento']}");
            apcu_delete("productos_est_{$cat['idEstablecimiento']}_base");
            apcu_delete("productos_est_{$cat['idEstablecimiento']}_ing");
        }
    }

    // Mantener lógica original de POST...
    header("HTTP/1.1 200 OK");
    exit();
}

// ==========================================================================
// PUT - Actualizar (invalidar caché)
// ==========================================================================
if ($_SERVER['REQUEST_METHOD'] == 'PUT') {
    $input = json_decode(file_get_contents('php://input'), true);

    // ... (código de actualización existente)

    // Invalidar caché al actualizar
    if (USE_CACHE && isset($input['idCategoria'])) {
        $stmt = $dbConn->prepare("SELECT idEstablecimiento FROM qo_productos_cat WHERE id = :id");
        $stmt->bindValue(':id', $input['idCategoria']);
        $stmt->execute();
        $cat = $stmt->fetch(PDO::FETCH_ASSOC);
        if ($cat) {
            apcu_delete("productos_est_{$cat['idEstablecimiento']}");
            apcu_delete("productos_est_{$cat['idEstablecimiento']}_base");
        }
    }

    header("HTTP/1.1 200 OK");
    exit();
}

// ==========================================================================
// DELETE (invalidar caché)
// ==========================================================================
if ($_SERVER['REQUEST_METHOD'] == 'DELETE') {
    $id = intval($_GET['idProducto']);

    // Obtener info antes de borrar
    $stmt = $dbConn->prepare("SELECT c.idEstablecimiento
                               FROM qo_productos_est e
                               JOIN qo_productos_cat c ON e.idCategoria = c.id
                               WHERE e.id = :id");
    $stmt->bindValue(':id', $id);
    $stmt->execute();
    $prod = $stmt->fetch(PDO::FETCH_ASSOC);

    // Borrar
    $dbConn->prepare("DELETE FROM qo_productos_ing_aler WHERE idProducto = :id")->execute([':id' => $id]);
    $dbConn->prepare("DELETE FROM qo_productos_opc WHERE idProducto = :id")->execute([':id' => $id]);
    $dbConn->prepare("DELETE FROM qo_ingredientes_producto WHERE idProducto = :id")->execute([':id' => $id]);

    // Invalidar caché
    if (USE_CACHE && $prod) {
        apcu_delete("productos_est_{$prod['idEstablecimiento']}");
        apcu_delete("productos_est_{$prod['idEstablecimiento']}_base");
    }

    header("HTTP/1.1 200 OK");
    exit();
}

header("HTTP/1.1 400 Bad Request");
?>
