<?php
include "config.php";
include "utils.php";


$dbConn =  connect($db);
/*
  listar todos los posts o solo uno
 */
if ($_SERVER['REQUEST_METHOD'] == 'GET')
{
  if (isset($_GET['home']))
  {
    if ($_GET['idUsuario']==''){
        if ($_GET['idGrupo']==1){
          $_GET['idUsuario']='39';
        }else if ($_GET['idGrupo']==2){
          $_GET['idUsuario']='5506';
        }else if ($_GET['idGrupo']==3){
          $_GET['idUsuario']='5664';
        }else if ($_GET['idGrupo']==4){
          $_GET['idUsuario']='5883';
        }
    }
    
    $sql=$dbConn->prepare("SELECT * FROM qo_users where id=:idUsuario");
    $sql->bindValue(':idUsuario', $_GET['idUsuario']);  
    $sql->execute();
    $result = $sql->fetch(PDO::FETCH_OBJ);
    $idPueblo= $result->idPueblo;
    $sql = $dbConn->prepare("SELECT DISTINCT c.id,c.imagen,c.nombre,c.nombre_eng,c.nombre_ger,c.nombre_fr,c.estado,if (isnull(a.orden),0,a.orden) as orden,c.raiz,c.proximamente,c.idGrupo 
    FROM qo_categorias c 
    left join (SELECT count(*) orden,idCategoria FROM qo_logs_app Where idUsuario=:idUsuario GROUP BY idCategoria) a on (a.idCategoria=c.id)
    left join qo_establecimientos e on (e.idCategoria=c.id) 
    Where (e.estado=1 or (e.estado is null and c.id in (select c2.raiz from qo_categorias c2 where c2.raiz<>0))) and c.idGrupo=:idGrupo and ((e.idPueblo=:idPueblo) or (e.idPueblo<>:idPueblo and e.visibleFuera=1)) order by a.orden desc,nombre limit 5");
    $sql->bindValue(':idUsuario', $_GET['idUsuario']);  
    $sql->bindValue(':idPueblo', $idPueblo);  
    $sql->bindValue(':idGrupo', $_GET['idGrupo']);  
    $sql->execute();
    $sql->setFetchMode(PDO::FETCH_ASSOC);
    header("HTTP/1.1 200 OK");
    echo json_encode( $sql->fetchAll()  );
    exit();
  }else if (isset($_GET['idEstablecimientoMesa']))
  {
    $sql = $dbConn->prepare("SELECT c.*,GROUP_CONCAT(u.nombre) as usuarios,GROUP_CONCAT(u.id) as idUsuarios,z.nombre as zona FROM qo_cuentas c inner join qo_users u on (u.id=c.idUsuario) inner join qo_establecimientos_zonas z on (z.id=c.idZona) WHERE c.idEstablecimiento=:idEstablecimientoMesa and c.cerrada=0 GROUP BY c.idZona,c.mesa");
    $sql->bindValue(':idEstablecimientoMesa', $_GET['idEstablecimientoMesa']);  
    $sql->execute();
    $sql->setFetchMode(PDO::FETCH_ASSOC);
    header("HTTP/1.1 200 OK");
    echo json_encode( $sql->fetchAll()  );
    exit();
  }
}

// Crear un nuevo post
if ($_SERVER['REQUEST_METHOD'] == 'POST')
{
    $input = json_decode(file_get_contents('php://input'), true);
    $sql = "INSERT INTO `qo_logs_app` (pantalla,filtro,idUsuario,idEstablecimiento,idCategoria,fecha) VALUES 
    (:pantalla,:filtro,:idUsuario,:idEstablecimiento,:idCategoria,:fecha)";
    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':pantalla', $input['pantalla']);
    $statement->bindValue(':idUsuario', $input['idUsuario']);
    $statement->bindValue(':idEstablecimiento', $input['idEstablecimiento']);
    $statement->bindValue(':filtro', $input['filtro']);
    $statement->bindValue(':idCategoria', $input['idCategoria']);
    $statement->bindValue(':fecha', $input['fecha']);
    $statement->execute();
    $postId = $dbConn->lastInsertId();
    if($postId)
    {
      $input['id'] = $postId;
      header("HTTP/1.1 200 OK");
      echo json_encode($input);
      exit();
	 }
}

//Borrar
if ($_SERVER['REQUEST_METHOD'] == 'DELETE')
{
	$id = $_GET['id'];
  $statement = $dbConn->prepare("DELETE FROM qo_users where id=:id");
  $statement->bindValue(':id', $id);
  $statement->execute();
	header("HTTP/1.1 200 OK");
	exit();
}

//Actualizar
if ($_SERVER['REQUEST_METHOD'] == 'PUT')
{
    $input = json_decode(file_get_contents('php://input'), true);
    $userId = $input['id'];
    $fields = getParams($input);

    $sql = "
          UPDATE qo_users
          SET $fields
          WHERE id='$userId'
           ";

    $statement = $dbConn->prepare($sql);
    bindAllValues($statement, $input);

    $statement->execute();
    header("HTTP/1.1 200 OK");
    exit();
}


//En caso de que ninguna de las opciones anteriores se haya ejecutado
header("HTTP/1.1 400 Bad Request");

?>