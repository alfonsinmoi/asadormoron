<?php
/**
 * Reglas de asignación de repartidores.
 *
 *  GET    /api/reglas-asignacion.php                    → lista ordenada por prioridad
 *  GET    /api/reglas-asignacion.php?idEstablecimiento=67
 *  POST   /api/reglas-asignacion.php   body: regla     → crea una nueva
 *  PUT    /api/reglas-asignacion.php   body: array     → reemplaza el set completo (transacción)
 *  DELETE /api/reglas-asignacion.php?id=X
 *
 * Formato regla:
 *  { id?, idEstablecimiento, nombre, prioridad, tipo, parametros: {...}, activa }
 */

include __DIR__ . "/../config.php";
include __DIR__ . "/../utils.php";

header("Content-Type: application/json; charset=utf-8");
$dbConn = connect($db);
$method = $_SERVER['REQUEST_METHOD'];

const TIPOS_VALIDOS = ['zona','carga','vehiculo','turno','round_robin'];

function reglaFromInput(array $r): array {
    return [
        'idEstablecimiento' => isset($r['idEstablecimiento']) ? (int)$r['idEstablecimiento'] : null,
        'nombre'            => trim($r['nombre'] ?? ''),
        'prioridad'         => (int)($r['prioridad'] ?? 0),
        'tipo'              => $r['tipo'] ?? '',
        'parametros'        => $r['parametros'] ?? new stdClass(),
        'activa'            => isset($r['activa']) ? ($r['activa'] ? 1 : 0) : 1
    ];
}

try {
    if ($method === 'GET') {
        $sql = "SELECT id, idEstablecimiento, nombre, prioridad, tipo, parametros, activa,
                       fecha_creacion, fecha_actualizacion
                FROM qo_reglas_asignacion
                WHERE 1=1";
        $params = [];
        if (isset($_GET['idEstablecimiento'])) {
            $sql .= " AND (idEstablecimiento = :est OR idEstablecimiento IS NULL)";
            $params[':est'] = (int)$_GET['idEstablecimiento'];
        }
        $sql .= " ORDER BY prioridad, id";
        $stmt = $dbConn->prepare($sql);
        foreach ($params as $k => $v) $stmt->bindValue($k, $v, PDO::PARAM_INT);
        $stmt->execute();
        $out = [];
        foreach ($stmt->fetchAll(PDO::FETCH_ASSOC) as $r) {
            $r['parametros'] = json_decode($r['parametros'], true);
            $r['activa'] = (bool)$r['activa'];
            $out[] = $r;
        }
        echo json_encode($out);
        exit();
    }

    if ($method === 'POST') {
        $input = json_decode(file_get_contents('php://input'), true);
        if (!is_array($input)) { http_response_code(400); echo json_encode(['error'=>'JSON invalido']); exit(); }
        $r = reglaFromInput($input);
        if ($r['nombre'] === '' || !in_array($r['tipo'], TIPOS_VALIDOS)) {
            http_response_code(400);
            echo json_encode(['error' => 'nombre y tipo validos requeridos', 'tipos' => TIPOS_VALIDOS]);
            exit();
        }
        $stmt = $dbConn->prepare("
            INSERT INTO qo_reglas_asignacion (idEstablecimiento, nombre, prioridad, tipo, parametros, activa)
            VALUES (:est, :nom, :pri, :tipo, :params, :act)
        ");
        $stmt->bindValue(':est',    $r['idEstablecimiento'], $r['idEstablecimiento'] === null ? PDO::PARAM_NULL : PDO::PARAM_INT);
        $stmt->bindValue(':nom',    $r['nombre']);
        $stmt->bindValue(':pri',    $r['prioridad'], PDO::PARAM_INT);
        $stmt->bindValue(':tipo',   $r['tipo']);
        $stmt->bindValue(':params', json_encode($r['parametros'], JSON_UNESCAPED_UNICODE));
        $stmt->bindValue(':act',    $r['activa'], PDO::PARAM_INT);
        $stmt->execute();
        echo json_encode(['ok' => true, 'id' => (int)$dbConn->lastInsertId()]);
        exit();
    }

    if ($method === 'PUT') {
        $input = json_decode(file_get_contents('php://input'), true);
        if (!is_array($input)) { http_response_code(400); echo json_encode(['error'=>'Array de reglas requerido']); exit(); }

        $dbConn->beginTransaction();
        try {
            // Reemplazo total: borra y reinserta. Si vienen con id se respetan.
            $idEst = isset($_GET['idEstablecimiento']) ? (int)$_GET['idEstablecimiento'] : null;
            if ($idEst !== null) {
                $del = $dbConn->prepare("DELETE FROM qo_reglas_asignacion WHERE idEstablecimiento = :est OR idEstablecimiento IS NULL");
                $del->bindValue(':est', $idEst, PDO::PARAM_INT);
                $del->execute();
            } else {
                $dbConn->exec("DELETE FROM qo_reglas_asignacion");
            }

            $ins = $dbConn->prepare("
                INSERT INTO qo_reglas_asignacion (idEstablecimiento, nombre, prioridad, tipo, parametros, activa)
                VALUES (:est, :nom, :pri, :tipo, :params, :act)
            ");
            foreach ($input as $raw) {
                $r = reglaFromInput($raw);
                if ($r['nombre'] === '' || !in_array($r['tipo'], TIPOS_VALIDOS)) {
                    throw new RuntimeException("Regla invalida: nombre o tipo erroneos");
                }
                $ins->bindValue(':est',    $r['idEstablecimiento'], $r['idEstablecimiento'] === null ? PDO::PARAM_NULL : PDO::PARAM_INT);
                $ins->bindValue(':nom',    $r['nombre']);
                $ins->bindValue(':pri',    $r['prioridad'], PDO::PARAM_INT);
                $ins->bindValue(':tipo',   $r['tipo']);
                $ins->bindValue(':params', json_encode($r['parametros'], JSON_UNESCAPED_UNICODE));
                $ins->bindValue(':act',    $r['activa'], PDO::PARAM_INT);
                $ins->execute();
            }
            $dbConn->commit();
            echo json_encode(['ok' => true, 'reglas' => count($input)]);
            exit();
        } catch (Exception $e) {
            $dbConn->rollBack();
            throw $e;
        }
    }

    if ($method === 'DELETE') {
        $id = (int)($_GET['id'] ?? 0);
        if ($id <= 0) { http_response_code(400); echo json_encode(['error'=>'id invalido']); exit(); }
        $stmt = $dbConn->prepare("DELETE FROM qo_reglas_asignacion WHERE id = :id");
        $stmt->bindValue(':id', $id, PDO::PARAM_INT);
        $stmt->execute();
        echo json_encode(['ok' => true, 'eliminados' => $stmt->rowCount()]);
        exit();
    }

    http_response_code(405);
    echo json_encode(['error' => 'Method not allowed']);

} catch (Exception $e) {
    error_log('[reglas-asignacion] ' . $e->getMessage());
    http_response_code(500);
    echo json_encode(['error' => 'Internal error', 'detalle' => $e->getMessage()]);
}
