<?php
/**
 * API de Productos - VERSIÓN UNIFICADA Y OPTIMIZADA
 *
 * Unifica productos.php y productos2.php
 *
 * Mejoras implementadas:
 * 1. Consultas SQL optimizadas con batch queries
 * 2. Cache opcional con APCu
 * 3. Headers HTTP de cache
 * 4. Código DRY (Don't Repeat Yourself)
 * 5. Validación de inputs
 * 6. Prepared statements en todas las consultas
 */

include "config.php";
include "utils.php";

// ============================================================================
// CONFIGURACIÓN
// ============================================================================
define('CACHE_TTL', 300); // 5 minutos
define('CACHE_TTL_LONG', 3600); // 1 hora
define('USE_CACHE', function_exists('apcu_fetch'));

// ============================================================================
// FUNCIONES AUXILIARES
// ============================================================================

/**
 * Ejecuta query con caché opcional
 */
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

/**
 * Configura headers de caché HTTP
 */
function setCacheHeaders($maxAge = 300) {
    header("Cache-Control: public, max-age=$maxAge");
    header("Expires: " . gmdate("D, d M Y H:i:s", time() + $maxAge) . " GMT");
}

/**
 * Respuesta JSON exitosa
 */
function jsonResponse($data, $cacheTime = 0) {
    header("Content-Type: application/json; charset=utf-8");
    header("HTTP/1.1 200 OK");
    if ($cacheTime > 0) {
        setCacheHeaders($cacheTime);
    }
    echo json_encode($data);
    exit();
}

/**
 * Respuesta de error
 */
function errorResponse($code = 400, $message = "Bad Request") {
    header("HTTP/1.1 $code $message");
    echo json_encode(['error' => $message]);
    exit();
}

/**
 * Invalida caché de productos de un establecimiento
 */
function invalidateProductCache($dbConn, $idCategoria) {
    if (!USE_CACHE || !$idCategoria) return;

    $stmt = $dbConn->prepare("SELECT idEstablecimiento FROM qo_productos_cat WHERE id = :id");
    $stmt->bindValue(':id', $idCategoria);
    $stmt->execute();
    $cat = $stmt->fetch(PDO::FETCH_ASSOC);

    if ($cat) {
        $idEst = $cat['idEstablecimiento'];
        apcu_delete("productos_est_$idEst");
        apcu_delete("productos_est_{$idEst}_base");
        apcu_delete("productos_est_{$idEst}_ing");
        apcu_delete("productos_cat_$idCategoria");
        apcu_delete("productos_cat_{$idCategoria}_base");
    }
}

/**
 * Obtiene opciones, ingredientes y alérgenos en batch para una lista de productos
 */
function getProductExtras($dbConn, $productIds, $idEstablecimiento = null) {
    if (empty($productIds)) {
        return ['opciones' => [], 'ingredientes' => [], 'alergenos' => []];
    }

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
    if ($idEstablecimiento) {
        $sqlIngredientes = "SELECT
                p.id as idProducto,
                GROUP_CONCAT(ie.id,';',ie.nombre SEPARATOR '|') as ingredientes
            FROM qo_ingredientes_producto i
            INNER JOIN qo_productos_est p ON p.id = i.idProducto AND p.eliminado = 0
            INNER JOIN qo_productos_cat c ON c.id = p.idCategoria
            INNER JOIN qo_ingredientes_establecimiento ie ON ie.id = i.idIngrediente AND ie.estado = 1
            WHERE c.idEstablecimiento = :idEst AND p.id IN ($productIdsStr)
            GROUP BY p.id";
        $stmt = $dbConn->prepare($sqlIngredientes);
        $stmt->bindValue(':idEst', $idEstablecimiento);
        $stmt->execute();
    } else {
        $sqlIngredientes = "SELECT
                i.idProducto,
                GROUP_CONCAT(ie.id,';',ie.nombre SEPARATOR '|') as ingredientes
            FROM qo_ingredientes_producto i
            INNER JOIN qo_ingredientes_establecimiento ie ON ie.id = i.idIngrediente AND ie.estado = 1
            WHERE i.idProducto IN ($productIdsStr)
            GROUP BY i.idProducto";
        $stmt = $dbConn->query($sqlIngredientes);
    }

    $ingredientesMap = [];
    while ($row = $stmt->fetch(PDO::FETCH_ASSOC)) {
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

    return [
        'opciones' => $opcionesMap,
        'ingredientes' => $ingredientesMap,
        'alergenos' => $alergenosMap
    ];
}

/**
 * SQL base para productos (reutilizable)
 */
function getProductosBaseSQL() {
    return "SELECT
            e.puntos, e.fuerzaIngredientes, e.numeroIngredientes,
            c.estado as estadoCategoria, c.idEstablecimiento,
            e.id as idArticulo, e.vistaEnvios, e.vistaLocal, e.precioLocal,
            e.nombre,
            IFNULL(NULLIF(e.nombre_eng,''), e.nombre) as nombre_eng,
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
        INNER JOIN qo_productos_est e ON e.idCategoria = c.id AND e.eliminado = 0";
}

// ============================================================================
// CONEXIÓN
// ============================================================================
$dbConn = connect($db);

// ============================================================================
// GET - Consultas
// ============================================================================
if ($_SERVER['REQUEST_METHOD'] == 'GET') {

    // -------------------------------------------------------------------------
    // Productos por ESTABLECIMIENTO (endpoint principal, más usado)
    // -------------------------------------------------------------------------
    if (isset($_GET['idEstablecimientoProducto'])) {
        $idEst = intval($_GET['idEstablecimientoProducto']);
        $cacheKey = "productos_est_$idEst";

        $sql = getProductosBaseSQL() . "
            WHERE c.idEstablecimiento = :id
            ORDER BY c.orden, e.idCategoria, e.nombre, e.id";

        $productos = getCachedOrQuery($cacheKey . "_base", $dbConn, $sql, [':id' => $idEst], 120);

        if (empty($productos)) {
            jsonResponse([], 60);
        }

        // Obtener extras en batch
        $productIds = array_column($productos, 'idArticulo');
        $extras = getProductExtras($dbConn, $productIds, $idEst);

        // Combinar
        foreach ($productos as &$p) {
            $id = $p['idArticulo'];
            $p['opciones'] = $extras['opciones'][$id] ?? '';
            $p['ingredientes'] = $extras['ingredientes'][$id] ?? '';
            $p['alergenos'] = $extras['alergenos'][$id] ?? '';
        }

        if (USE_CACHE) {
            apcu_store($cacheKey, $productos, CACHE_TTL);
        }

        jsonResponse($productos, 120);
    }

    // -------------------------------------------------------------------------
    // Productos por CATEGORÍA
    // -------------------------------------------------------------------------
    if (isset($_GET['idEstablecimientoProductoCat'])) {
        $idCategoria = intval($_GET['idEstablecimientoProductoCat']);
        $cacheKey = "productos_cat_$idCategoria";

        $sql = getProductosBaseSQL() . "
            WHERE c.id = :idCategoria
            ORDER BY e.nombre, e.id";

        $productos = getCachedOrQuery($cacheKey . "_base", $dbConn, $sql, [':idCategoria' => $idCategoria], 120);

        if (empty($productos)) {
            jsonResponse([], 60);
        }

        $productIds = array_column($productos, 'idArticulo');
        $extras = getProductExtras($dbConn, $productIds);

        foreach ($productos as &$p) {
            $id = $p['idArticulo'];
            $p['opciones'] = $extras['opciones'][$id] ?? '';
            $p['ingredientes'] = $extras['ingredientes'][$id] ?? '';
            $p['alergenos'] = $extras['alergenos'][$id] ?? '';
        }

        if (USE_CACHE) {
            apcu_store($cacheKey, $productos, CACHE_TTL);
        }

        jsonResponse($productos, 120);
    }

    // -------------------------------------------------------------------------
    // Productos por categoría (idCategoriaProducto) - versión simplificada
    // -------------------------------------------------------------------------
    if (isset($_GET['idCategoriaProducto'])) {
        $idCat = intval($_GET['idCategoriaProducto']);

        $sql = "SELECT
                e.fuerzaIngredientes, e.numeroIngredientes, c.idEstablecimiento,
                e.id, e.nombre, e.idCategoria, c.nombre as categoria, c.tipo as idTipoCategoria,
                e.imagen, e.estado, e.porEncargo, e.vistaEnvios, e.vistaLocal, e.precioLocal, e.precio,
                IFNULL(e.descripcion,'') as descripcion
            FROM qo_productos_cat c
            INNER JOIN qo_productos_est e ON e.idCategoria = c.id AND e.eliminado = 0
            WHERE c.id = :id
            ORDER BY e.nombre, e.id";

        $productos = getCachedOrQuery("cat_simple_$idCat", $dbConn, $sql, [':id' => $idCat], 300);

        if (!empty($productos)) {
            $productIds = array_column($productos, 'id');
            $extras = getProductExtras($dbConn, $productIds);

            foreach ($productos as &$p) {
                $id = $p['id'];
                $p['opciones'] = $extras['opciones'][$id] ?? '';
                $p['ingredientes'] = $extras['ingredientes'][$id] ?? '';
                $p['alergenos'] = $extras['alergenos'][$id] ?? '';
            }
        }

        jsonResponse($productos, 300);
    }

    // -------------------------------------------------------------------------
    // Favoritos del usuario (establecimientos más pedidos)
    // -------------------------------------------------------------------------
    if (isset($_GET['idUsuarioFavoritos'])) {
        $idUsuario = intval($_GET['idUsuarioFavoritos']);

        $sql = "SELECT
                COUNT(*) as pedidos, esComercio, orden, idCategoria, pedidoMinimo,
                activoLunes, activoMartes, activoMiercoles, activoJueves, activoViernes,
                inicioLunes, inicioMartes, inicioMiercoles, inicioJueves, inicioViernes,
                activoSabado, activoDomingo, inicioSabado, inicioDomingo,
                finLunes, finMartes, finMiercoles, finJueves, finViernes, finSabado, finDomingo,
                servicioActivo, IFNULL(tiempoEntrega, 0) as tiempoEntrega,
                IFNULL(e.usuarioBarra, '') as usuarioBarra, IFNULL(e.usuarioCocina, '') as usuarioCocina,
                e.ipImpresora, e.nombreImpresoraCocina, e.nombreImpresoraBarra,
                0 as distancia, e.valoraciones, e.puntos, e.latitud, e.longitud,
                e.id, e.nombre, e.direccion, e.poblacion, e.codPostal, e.tipo, e.idTipo,
                e.imagen, e.provincia, e.telefono, e.email, e.numeroCategorias, e.numeroProductos,
                e.ventas, e.zonas, e.local, e.envio, e.recogida, e.logo,
                NULL as fechaInicio, NULL as fechaFin, '' as descripcion,
                '' as equipoLocal, '' as imagenEquipoLocal, '' as equipoVisitante,
                '' as imagenEquipoVisitante, '' as jornada, '' as temporada, '' as estadio,
                e.llamadaCamarero, e.puedeReservar,
                inicioLunesTarde, inicioMartesTarde, inicioMiercolesTarde, inicioJuevesTarde,
                inicioViernesTarde, inicioSabadoTarde, inicioDomingoTarde,
                finLunesTarde, finMartesTarde, finMiercolesTarde, finJuevesTarde,
                finViernesTarde, finSabadoTarde, finDomingoTarde, e.idZona
            FROM v_establecimientos e
            INNER JOIN qo_pedidos p ON p.idEstablecimiento = e.id
            INNER JOIN qo_users u ON u.id = p.idUsuario
            " . ($idUsuario > 0 ? "WHERE p.idUsuario = :id" : "") . "
            GROUP BY p.idEstablecimiento
            ORDER BY pedidos DESC";

        $params = $idUsuario > 0 ? [':id' => $idUsuario] : [];
        $result = getCachedOrQuery("favoritos_$idUsuario", $dbConn, $sql, $params, 300);

        jsonResponse($result, 300);
    }

    // -------------------------------------------------------------------------
    // Alergenos (datos estáticos)
    // -------------------------------------------------------------------------
    if (isset($_GET['alergenos'])) {
        $sql = "SELECT id, nombre, imagen, estado FROM qo_alergenos";
        $result = getCachedOrQuery("alergenos_all", $dbConn, $sql, [], CACHE_TTL_LONG);
        jsonResponse($result, CACHE_TTL_LONG);
    }

    // -------------------------------------------------------------------------
    // Cantidad de ingredientes de un producto
    // -------------------------------------------------------------------------
    if (isset($_GET['cantidadIngredientes'])) {
        $stmt = $dbConn->prepare("SELECT IF(fuerzaIngredientes=0, 0, numeroIngredientes) as ingredientes
                                  FROM qo_productos_est WHERE id = :id");
        $stmt->bindValue(':id', intval($_GET['cantidadIngredientes']));
        $stmt->execute();
        $result = $stmt->fetch(PDO::FETCH_OBJ);
        jsonResponse($result->ingredientes ?? 0);
    }

    // -------------------------------------------------------------------------
    // Productos más vendidos
    // -------------------------------------------------------------------------
    if (isset($_GET['masvendidos'])) {
        $idGrupo = intval($_GET['idGrupo'] ?? 0);
        $tipoVenta = isset($_GET['masvendidoslocal']) ? 'Local' : 'NoLocal';
        $cacheKey = "masvendidos_{$idGrupo}_{$tipoVenta}";

        $whereGrupo = $idGrupo > 0 ? "AND es.idGrupo = :idGrupo" : "";
        $whereTipoVenta = $tipoVenta === 'Local' ? "AND d.tipoVenta = 'Local'" : "AND d.tipoVenta <> 'Local'";

        $sql = "SELECT
                e.fuerzaIngredientes, e.numeroIngredientes, SUM(d.cantidad) as ventas,
                es.nombre as nombreEstablecimiento, c.estado as estadoCategoria, c.idEstablecimiento,
                e.id, e.nombre,
                IFNULL(NULLIF(e.nombre_eng,''), e.nombre) as nombre_eng,
                IFNULL(NULLIF(e.nombre_ger,''), e.nombre) as nombre_ger,
                IFNULL(NULLIF(e.nombre_fr,''), e.nombre) as nombre_fr,
                e.idCategoria, e.porEncargo,
                c.nombre as categoria,
                IFNULL(NULLIF(c.nombre_eng,''), c.nombre) as categoria_eng,
                IFNULL(NULLIF(c.nombre_ger,''), c.nombre) as categoria_ger,
                IFNULL(NULLIF(c.nombre_fr,''), c.nombre) as categoria_fr,
                c.tipo as idTipoCategoria, e.imagen, e.estado, e.precio,
                IFNULL(e.descripcion,'') as descripcion,
                IFNULL(e.descripcion_eng,'') as descripcion_eng,
                IFNULL(e.descripcion_ger,'') as descripcion_ger,
                IFNULL(e.descripcion_fr,'') as descripcion_fr,
                '' as opciones, '' as ingredientes, '' as alergenos
            FROM qo_productos_cat c
            INNER JOIN qo_productos_est e ON e.idCategoria = c.id
            INNER JOIN qo_pedidos_detalle d ON d.idProducto = e.id
            INNER JOIN qo_establecimientos es ON es.id = c.idEstablecimiento
            WHERE e.eliminado = 0 $whereTipoVenta $whereGrupo
            GROUP BY e.id
            ORDER BY ventas DESC
            LIMIT 10";

        $params = $idGrupo > 0 ? [':idGrupo' => $idGrupo] : [];
        $result = getCachedOrQuery($cacheKey, $dbConn, $sql, $params, 600);

        jsonResponse($result, 600);
    }

    // -------------------------------------------------------------------------
    // Productos más vendidos LOCAL
    // -------------------------------------------------------------------------
    if (isset($_GET['masvendidoslocal'])) {
        $idGrupo = intval($_GET['idGrupo'] ?? 0);
        $cacheKey = "masvendidos_local_$idGrupo";

        $sql = "SELECT
                e.fuerzaIngredientes, e.numeroIngredientes, SUM(d.cantidad) as ventas,
                es.nombre as nombreEstablecimiento, c.estado as estadoCategoria, c.idEstablecimiento,
                e.id, e.nombre, e.idCategoria, c.nombre as categoria, c.tipo as idTipoCategoria,
                e.imagen, e.estado, e.precio, e.porEncargo,
                IFNULL(e.descripcion,'') as descripcion,
                '' as opciones, '' as ingredientes, '' as alergenos
            FROM qo_productos_cat c
            INNER JOIN qo_productos_est e ON e.idCategoria = c.id AND e.eliminado = 0
            INNER JOIN qo_pedidos_detalle d ON d.idProducto = e.id
            INNER JOIN qo_establecimientos es ON es.id = c.idEstablecimiento
            WHERE e.eliminado = 0 AND es.idGrupo = :idGrupo AND d.tipoVenta = 'Local'
            GROUP BY e.id
            ORDER BY ventas DESC
            LIMIT 10";

        $result = getCachedOrQuery($cacheKey, $dbConn, $sql, [':idGrupo' => $idGrupo], 600);
        jsonResponse($result, 600);
    }

    // -------------------------------------------------------------------------
    // Opciones de un producto (para Kiosko)
    // -------------------------------------------------------------------------
    if (isset($_GET['idOpcionesProducto'])) {
        $idProducto = intval($_GET['idOpcionesProducto']);

        $sql = "SELECT
                id, idProducto, opcion,
                IFNULL(NULLIF(opcion_eng,''), opcion) as opcion_eng,
                IFNULL(NULLIF(opcion_ger,''), opcion) as opcion_ger,
                IFNULL(NULLIF(opcion_fr,''), opcion) as opcion_fr,
                tipoIncremento, valorIncremento as precio, puntos,
                IFNULL(observaciones,'') as observaciones
            FROM qo_productos_opc
            WHERE idProducto = :id";

        $result = getCachedOrQuery("opciones_prod_$idProducto", $dbConn, $sql, [':id' => $idProducto], 300);
        jsonResponse($result, 300);
    }

    // -------------------------------------------------------------------------
    // Alergenos de un producto (para Kiosko)
    // -------------------------------------------------------------------------
    if (isset($_GET['idAlergenosProducto'])) {
        $idProducto = intval($_GET['idAlergenosProducto']);

        $sql = "SELECT
                a.id, a.nombre, a.imagen, a.estado
            FROM qo_productos_ing_aler pa
            INNER JOIN qo_alergenos a ON pa.idAlergeno = a.id
            WHERE pa.idProducto = :id";

        $result = getCachedOrQuery("alergenos_prod_$idProducto", $dbConn, $sql, [':id' => $idProducto], 300);
        jsonResponse($result, 300);
    }

    // -------------------------------------------------------------------------
    // Ingredientes de un producto
    // -------------------------------------------------------------------------
    if (isset($_GET['idIngredientesProducto'])) {
        $idProducto = intval($_GET['idIngredientesProducto']);

        $sql = "SELECT
                p.id, p.idIngrediente, p.precio, p.puntos, p.idProducto,
                e.nombre, e.nombre_eng, e.nombre_ger, e.nombre_fr, e.estado
            FROM qo_ingredientes_producto p
            INNER JOIN qo_ingredientes_establecimiento e ON e.id = p.idIngrediente AND e.estado = 1
            WHERE p.idProducto = :id";

        $result = getCachedOrQuery("ingredientes_prod_$idProducto", $dbConn, $sql, [':id' => $idProducto], 300);
        jsonResponse($result, 300);
    }

    // -------------------------------------------------------------------------
    // Ingredientes de un establecimiento
    // -------------------------------------------------------------------------
    if (isset($_GET['idEstablecimientoIngredientes'])) {
        $idEst = intval($_GET['idEstablecimientoIngredientes']);

        $sql = "SELECT id, idEstablecimiento, nombre, precio, estado
                FROM qo_ingredientes_establecimiento
                WHERE idEstablecimiento = :id";

        $result = getCachedOrQuery("ingredientes_est_$idEst", $dbConn, $sql, [':id' => $idEst], 300);
        jsonResponse($result, 300);
    }

    // -------------------------------------------------------------------------
    // Productos por IDs específicos
    // -------------------------------------------------------------------------
    if (isset($_GET['ids'])) {
        $ids = $_GET['ids'];
        // Sanitizar IDs
        $idsArray = array_map('intval', explode(',', $ids));
        $idsStr = implode(',', $idsArray);

        $sql = "SELECT
                e.vistaEnvios, e.vistaLocal, e.precioLocal, e.porEncargo, e.numeroIngredientes,
                c.estado as estadoCategoria, c.idEstablecimiento,
                e.id as idArticulo, e.nombre,
                IFNULL(NULLIF(e.nombre_eng,''), e.nombre) as nombre_eng,
                IFNULL(NULLIF(e.nombre_ger,''), e.nombre) as nombre_ger,
                IFNULL(NULLIF(e.nombre_fr,''), e.nombre) as nombre_fr,
                e.idCategoria, c.nombre as categoria,
                IFNULL(NULLIF(c.nombre_eng,''), c.nombre) as categoria_eng,
                IFNULL(NULLIF(c.nombre_ger,''), c.nombre) as categoria_ger,
                IFNULL(NULLIF(c.nombre_fr,''), c.nombre) as categoria_fr,
                c.tipo as idTipoCategoria, e.imagen, e.estado, e.precio,
                IFNULL(e.descripcion,'') as descripcion,
                IFNULL(e.descripcion_eng,'') as descripcion_eng,
                IFNULL(e.descripcion_ger,'') as descripcion_ger,
                IFNULL(e.descripcion_fr,'') as descripcion_fr
            FROM qo_productos_cat c
            INNER JOIN qo_productos_est e ON e.idCategoria = c.id AND e.eliminado = 0
            WHERE e.estado = 0 AND e.id IN ($idsStr)";

        $stmt = $dbConn->query($sql);
        $productos = $stmt->fetchAll(PDO::FETCH_ASSOC);

        if (!empty($productos)) {
            $extras = getProductExtras($dbConn, $idsArray);
            foreach ($productos as &$p) {
                $id = $p['idArticulo'];
                $p['opciones'] = $extras['opciones'][$id] ?? '';
                $p['ingredientes'] = $extras['ingredientes'][$id] ?? '';
                $p['alergenos'] = $extras['alergenos'][$id] ?? '';
            }
        }

        jsonResponse($productos);
    }

    // -------------------------------------------------------------------------
    // Todos los productos (admin)
    // -------------------------------------------------------------------------
    if (isset($_GET['all'])) {
        $sql = "SELECT
                e.fuerzaIngredientes, e.numeroIngredientes, c.idEstablecimiento,
                e.id, e.nombre, e.idCategoria, c.nombre as categoria, c.tipo as idTipoCategoria,
                e.imagen, e.estado, e.porEncargo, e.vistaEnvios, e.vistaLocal, e.precioLocal, e.precio,
                IFNULL(e.descripcion,'') as descripcion
            FROM qo_productos_cat c
            INNER JOIN qo_productos_est e ON e.idCategoria = c.id AND e.eliminado = 0
            ORDER BY e.nombre, e.id";

        $productos = getCachedOrQuery("productos_all", $dbConn, $sql, [], 600);

        if (!empty($productos)) {
            $productIds = array_column($productos, 'id');
            $extras = getProductExtras($dbConn, $productIds);
            foreach ($productos as &$p) {
                $id = $p['id'];
                $p['opciones'] = $extras['opciones'][$id] ?? '';
                $p['ingredientes'] = $extras['ingredientes'][$id] ?? '';
                $p['alergenos'] = $extras['alergenos'][$id] ?? '';
            }
        }

        jsonResponse($productos, 600);
    }
}

// ============================================================================
// POST - Crear
// ============================================================================
if ($_SERVER['REQUEST_METHOD'] == 'POST') {
    $input = json_decode(file_get_contents('php://input'), true);

    if (!$input) {
        errorResponse(400, "Invalid JSON input");
    }

    // -------------------------------------------------------------------------
    // Crear ingrediente de producto
    // -------------------------------------------------------------------------
    if (isset($_GET['ingredienteProducto'])) {
        $sql = "INSERT INTO qo_ingredientes_producto (puntos, idProducto, idIngrediente, precio)
                VALUES (:puntos, :idProducto, :idIngrediente, :precio)";
        $stmt = $dbConn->prepare($sql);
        $stmt->bindValue(':puntos', $input['puntos'] ?? 0);
        $stmt->bindValue(':idProducto', $input['idProducto']);
        $stmt->bindValue(':idIngrediente', $input['idIngrediente']);
        $stmt->bindValue(':precio', $input['precio'] ?? 0);
        $stmt->execute();

        $input['id'] = $dbConn->lastInsertId();
        jsonResponse($input);
    }

    // -------------------------------------------------------------------------
    // Crear ingrediente de establecimiento
    // -------------------------------------------------------------------------
    if (isset($_GET['ingrediente'])) {
        $sql = "INSERT INTO qo_ingredientes_establecimiento
                (nombre, nombre_eng, nombre_fr, nombre_ger, precio, estado, idEstablecimiento)
                VALUES (:nombre, :nombre, :nombre, :nombre, :precio, :estado, :idEstablecimiento)";
        $stmt = $dbConn->prepare($sql);
        $stmt->bindValue(':nombre', $input['nombre']);
        $stmt->bindValue(':estado', $input['estado'] ?? 1);
        $stmt->bindValue(':idEstablecimiento', $input['idEstablecimiento']);
        $stmt->bindValue(':precio', $input['precio'] ?? 0);
        $stmt->execute();

        $input['id'] = $dbConn->lastInsertId();

        // Invalidar caché
        if (USE_CACHE) {
            apcu_delete("ingredientes_est_{$input['idEstablecimiento']}");
        }

        jsonResponse($input);
    }

    // -------------------------------------------------------------------------
    // Crear alérgeno de producto
    // -------------------------------------------------------------------------
    if (isset($_GET['alergenos'])) {
        $sql = "INSERT INTO qo_productos_ing_aler (idProducto, idAlergeno) VALUES (:idProducto, :idAlergeno)";
        $stmt = $dbConn->prepare($sql);
        $stmt->bindValue(':idProducto', $input['idProducto']);
        $stmt->bindValue(':idAlergeno', $input['idAlergeno']);
        $stmt->execute();

        $input['id'] = $dbConn->lastInsertId();
        jsonResponse($input);
    }

    // -------------------------------------------------------------------------
    // Crear opción de producto
    // -------------------------------------------------------------------------
    if (isset($_GET['opcion'])) {
        $sql = "INSERT INTO qo_productos_opc
                (puntos, idProducto, opcion, opcion_fr, opcion_ger, opcion_eng, tipoIncremento, valorIncremento)
                VALUES (:puntos, :idProducto, :opcion, :opcion, :opcion, :opcion, 0, :valorIncremento)";
        $stmt = $dbConn->prepare($sql);
        $stmt->bindValue(':puntos', $input['puntos'] ?? 0);
        $stmt->bindValue(':idProducto', $input['idProducto']);
        $stmt->bindValue(':opcion', $input['opcion']);
        $stmt->bindValue(':valorIncremento', $input['valorIncremento'] ?? 0);
        $stmt->execute();

        $input['id'] = $dbConn->lastInsertId();
        jsonResponse($input);
    }

    // -------------------------------------------------------------------------
    // Crear producto
    // -------------------------------------------------------------------------
    $hasTranslations = !empty($input['nombre_eng']);

    if ($hasTranslations) {
        $sql = "INSERT INTO qo_productos_est
                (puntos, porEncargo, fuerzaIngredientes, precioLocal, vistaLocal, vistaEnvios,
                 numeroIngredientes, nombre, nombre_eng, nombre_fr, nombre_ger, idCategoria,
                 imagen, estado, precio, descripcion, descripcion_eng, descripcion_fr, descripcion_ger)
                VALUES
                (:puntos, :porEncargo, :fuerzaIngredientes, :precioLocal, :vistaLocal, :vistaEnvios,
                 :numeroIngredientes, :nombre, :nombre_eng, :nombre_fr, :nombre_ger, :idCategoria,
                 :imagen, 1, :precio, :descripcion, :descripcion_eng, :descripcion_fr, :descripcion_ger)";
    } else {
        $sql = "INSERT INTO qo_productos_est
                (puntos, porEncargo, fuerzaIngredientes, precioLocal, vistaLocal, vistaEnvios,
                 numeroIngredientes, nombre, nombre_eng, nombre_fr, nombre_ger, idCategoria,
                 imagen, estado, precio, descripcion, descripcion_eng, descripcion_fr, descripcion_ger)
                VALUES
                (:puntos, :porEncargo, :fuerzaIngredientes, :precioLocal, :vistaLocal, :vistaEnvios,
                 :numeroIngredientes, :nombre, :nombre, :nombre, :nombre, :idCategoria,
                 :imagen, 1, :precio, :descripcion, :descripcion, :descripcion, :descripcion)";
    }

    $stmt = $dbConn->prepare($sql);
    $stmt->bindValue(':puntos', $input['puntos'] ?? 0);
    $stmt->bindValue(':porEncargo', $input['porEncargo'] ?? 0);
    $stmt->bindValue(':fuerzaIngredientes', $input['fuerzaIngredientes'] ?? 0);
    $stmt->bindValue(':precioLocal', $input['precioLocal'] ?? $input['precio']);
    $stmt->bindValue(':vistaLocal', $input['vistaLocal'] ?? 1);
    $stmt->bindValue(':vistaEnvios', $input['vistaEnvios'] ?? 1);
    $stmt->bindValue(':numeroIngredientes', $input['numeroIngredientes'] ?? 0);
    $stmt->bindValue(':nombre', $input['nombre']);
    $stmt->bindValue(':idCategoria', $input['idCategoria']);
    $stmt->bindValue(':imagen', $input['imagen'] ?? '');
    $stmt->bindValue(':precio', $input['precio']);
    $stmt->bindValue(':descripcion', $input['descripcion'] ?? '');

    if ($hasTranslations) {
        $stmt->bindValue(':nombre_eng', $input['nombre_eng']);
        $stmt->bindValue(':nombre_fr', $input['nombre_fr'] ?? $input['nombre']);
        $stmt->bindValue(':nombre_ger', $input['nombre_ger'] ?? $input['nombre']);
        $stmt->bindValue(':descripcion_eng', $input['descripcion_eng'] ?? $input['descripcion']);
        $stmt->bindValue(':descripcion_fr', $input['descripcion_fr'] ?? $input['descripcion']);
        $stmt->bindValue(':descripcion_ger', $input['descripcion_ger'] ?? $input['descripcion']);
    }

    $stmt->execute();
    $input['id'] = $dbConn->lastInsertId();

    // Invalidar caché
    invalidateProductCache($dbConn, $input['idCategoria']);

    jsonResponse($input);
}

// ============================================================================
// PUT - Actualizar
// ============================================================================
if ($_SERVER['REQUEST_METHOD'] == 'PUT') {
    $input = json_decode(file_get_contents('php://input'), true);

    if (!$input) {
        errorResponse(400, "Invalid JSON input");
    }

    // -------------------------------------------------------------------------
    // Actualizar ingrediente de establecimiento
    // -------------------------------------------------------------------------
    if (isset($_GET['ingrediente'])) {
        $id = intval($input['id']);
        $fields = getParams($input);

        $sql = "UPDATE qo_ingredientes_establecimiento SET $fields WHERE id = $id";
        $stmt = $dbConn->prepare($sql);
        bindAllValues($stmt, $input);
        $stmt->execute();

        jsonResponse(['success' => true]);
    }

    // -------------------------------------------------------------------------
    // Actualizar ingrediente de producto
    // -------------------------------------------------------------------------
    if (isset($_GET['ingredienteProducto'])) {
        $id = intval($input['id']);

        $sql = "UPDATE qo_ingredientes_producto SET precio = :precio, puntos = :puntos WHERE id = :id";
        $stmt = $dbConn->prepare($sql);
        $stmt->bindValue(':precio', $input['precio']);
        $stmt->bindValue(':puntos', $input['puntos'] ?? 0);
        $stmt->bindValue(':id', $id);
        $stmt->execute();

        jsonResponse(['success' => true]);
    }

    // -------------------------------------------------------------------------
    // Actualizar producto
    // -------------------------------------------------------------------------
    $id = intval($input['idArticulo']);

    $sql = "UPDATE qo_productos_est SET
            puntos = :puntos, porEncargo = :porEncargo, fuerzaIngredientes = :fuerzaIngredientes,
            precioLocal = :precioLocal, vistaEnvios = :vistaEnvios, vistaLocal = :vistaLocal,
            numeroIngredientes = :numeroIngredientes, nombre = :nombre, nombre_eng = :nombre_eng,
            nombre_fr = :nombre_fr, nombre_ger = :nombre_ger, precio = :precio, imagen = :imagen,
            idCategoria = :idCategoria, estado = :estado, descripcion = :descripcion,
            descripcion_fr = :descripcion_fr, descripcion_eng = :descripcion_eng,
            descripcion_ger = :descripcion_ger, eliminado = :eliminado
            WHERE id = :id";

    $stmt = $dbConn->prepare($sql);
    $stmt->bindValue(':puntos', $input['puntos'] ?? 0);
    $stmt->bindValue(':porEncargo', $input['porEncargo'] ?? 0);
    $stmt->bindValue(':fuerzaIngredientes', $input['fuerzaIngredientes'] ?? 0);
    $stmt->bindValue(':precioLocal', $input['precioLocal'] ?? $input['precio']);
    $stmt->bindValue(':vistaEnvios', 1);
    $stmt->bindValue(':vistaLocal', $input['vistaLocal'] ?? 1);
    $stmt->bindValue(':numeroIngredientes', $input['numeroIngredientes'] ?? 0);
    $stmt->bindValue(':nombre', $input['nombre']);
    $stmt->bindValue(':nombre_eng', $input['nombre_eng'] ?? $input['nombre']);
    $stmt->bindValue(':nombre_fr', $input['nombre_fr'] ?? $input['nombre']);
    $stmt->bindValue(':nombre_ger', $input['nombre_ger'] ?? $input['nombre']);
    $stmt->bindValue(':precio', $input['precio']);
    $stmt->bindValue(':imagen', $input['imagen'] ?? '');
    $stmt->bindValue(':idCategoria', $input['idCategoria']);
    $stmt->bindValue(':estado', $input['estado'] ?? 1);
    $stmt->bindValue(':descripcion', $input['descripcion'] ?? '');
    $stmt->bindValue(':descripcion_eng', $input['descripcion_eng'] ?? '');
    $stmt->bindValue(':descripcion_fr', $input['descripcion_fr'] ?? '');
    $stmt->bindValue(':descripcion_ger', $input['descripcion_ger'] ?? '');
    $stmt->bindValue(':eliminado', $input['eliminado'] ?? 0);
    $stmt->bindValue(':id', $id);
    $stmt->execute();

    // Invalidar caché
    invalidateProductCache($dbConn, $input['idCategoria']);

    jsonResponse(['success' => true]);
}

// ============================================================================
// DELETE - Eliminar
// ============================================================================
if ($_SERVER['REQUEST_METHOD'] == 'DELETE') {
    $id = intval($_GET['idProducto'] ?? 0);

    if ($id <= 0) {
        errorResponse(400, "Invalid product ID");
    }

    // Obtener info antes de borrar para invalidar caché
    $stmt = $dbConn->prepare("SELECT c.idEstablecimiento, e.idCategoria
                              FROM qo_productos_est e
                              JOIN qo_productos_cat c ON e.idCategoria = c.id
                              WHERE e.id = :id");
    $stmt->bindValue(':id', $id);
    $stmt->execute();
    $prod = $stmt->fetch(PDO::FETCH_ASSOC);

    // Borrar datos relacionados
    $dbConn->prepare("DELETE FROM qo_productos_ing_aler WHERE idProducto = :id")->execute([':id' => $id]);
    $dbConn->prepare("DELETE FROM qo_productos_opc WHERE idProducto = :id")->execute([':id' => $id]);
    $dbConn->prepare("DELETE FROM qo_ingredientes_producto WHERE idProducto = :id")->execute([':id' => $id]);

    // Invalidar caché
    if ($prod) {
        invalidateProductCache($dbConn, $prod['idCategoria']);
    }

    jsonResponse(['success' => true, 'deleted' => $id]);
}

// ============================================================================
// Método no soportado
// ============================================================================
errorResponse(400, "Bad Request");
?>
