<?php
include "config.php";
include "utils.php";


$dbConn =  connect($db);
/*
  listar todos los posts o solo uno
 */
if ($_SERVER['REQUEST_METHOD'] == 'GET')
{
  if (isset($_GET['grupos']))
    {
      $sql = $dbConn->prepare("SELECT idGrupo as id, nombreGrupo as nombre from qo_pueblos_grupos group by idGrupo order by idGrupo");
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
      exit();
    }else{
    $sql = $dbConn->prepare("SELECT p.id,p.idGrupo,p.nombre,p.activo,p.Provincia,p.idUsuario,p.latitud,p.longitud,p.demo,if (isnull(e.cantidad),0,e.cantidad) as cantidad, GROUP_CONCAT(c.codPostal) as codPostal,p.visibleListado,p.radio,p.pedidoMinimo,p.especial,p.minutosAntes,p.entregaCasa,p.direccionEntrega FROM qo_pueblos p inner join qo_codigos_postales c on (c.idPueblo=p.id) left join (SELECt count(*) cantidad,idPueblo From qo_establecimientos e group by e.idPueblo) e on (e.idPueblo=p.id) group by p.id order by nombre");
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
    $sql = "INSERT INTO `qo_pueblos`(nombre, activo,codPostal,provincia,idUsuario,latitud,longitud,visibleListado,radio,pedidoMinimo,especial,minutosAntes) VALUES 
                (:nombre,:activo,:codPostal,:provincia,:idUsuario,:latitud,:longitud,:idGrupo,:visibleListado,:radio,:pedidoMinimo,:especial,:minutosAntes)";
    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':nombre', $input['nombre']);
    $statement->bindValue(':activo', $input['activo']);
    $statement->bindValue(':codPostal', $input['codPostal']);
    $statement->bindValue(':provincia', $input['Provincia']);
    $statement->bindValue(':idUsuario', $input['idUsuario']);
    $statement->bindValue(':latitud', $input['latitud']);
    $statement->bindValue(':longitud', $input['longitud']);
    $statement->bindValue(':visibleListado', $input['visibleListado']);
    $statement->bindValue(':radio', $input['radio']);
    $statement->bindValue(':pedidoMinimo', $input['pedidoMinimo']);
    $statement->bindValue(':especial', $input['especial']);
    $statement->bindValue(':minutosAntes', $input['minutosAntes']);
    $statement->bindValue(':idGrupo', $input['idGrupo']);
    $statement->execute();
    $postId = $dbConn->lastInsertId();
    if($postId)
    {
      $sql = "UPDATE `qo_users` SET idPueblo=:idPueblo WHERE id=:idUsuario";
      $statement = $dbConn->prepare($sql);
      $statement->bindValue(':idUsuario', $input['idUsuario']);
      $statement->bindValue(':idPueblo', $postId);
      $statement->execute();
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
  $statement = $dbConn->prepare("DELETE FROM qo_pueblos where id=:id");
  $statement->bindValue(':id', $id);
  $statement->execute();
	header("HTTP/1.1 200 OK");
	exit();
}

//Actualizar
if ($_SERVER['REQUEST_METHOD'] == 'PUT')
{
    $input = json_decode(file_get_contents('php://input'), true);
    $id = $input['id'];
    $fields = getParams($input);

    $sql = "UPDATE `qo_pueblos` SET minutosAntes=:minutosAntes,especial=:especial,pedidoMinimo=:pedidoMinimo,radio=:radio,visibleListado=:visibleListado,provincia=:provincia,codPostal=:codPostal,idGrupo=:idGrupo, `nombre`=:nombre,activo=:activo,idUsuario=:idUsuario,longitud=:longitud,latitud=:latitud WHERE id=$id";

    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':nombre', $input['nombre']);
    $statement->bindValue(':activo', $input['activo']);
    $statement->bindValue(':codPostal', $input['codPostal']);
    $statement->bindValue(':provincia', $input['Provincia']);
    $statement->bindValue(':idUsuario', $input['idUsuario']);
    $statement->bindValue(':latitud', $input['latitud']);
    $statement->bindValue(':longitud', $input['longitud']);
    $statement->bindValue(':idGrupo', $input['idGrupo']);
    $statement->bindValue(':visibleListado', $input['visibleListado']);
    $statement->bindValue(':radio', $input['radio']);
    $statement->bindValue(':pedidoMinimo', $input['pedidoMinimo']);
    $statement->bindValue(':especial', $input['especial']);
    $statement->bindValue(':minutosAntes', $input['minutosAntes']);
    $statement->execute();

    $sql = "UPDATE `qo_users` SET idPueblo=:idPueblo WHERE id=:idUsuario";
      $statement = $dbConn->prepare($sql);
      $statement->bindValue(':idUsuario', $input['idUsuario']);
      $statement->bindValue(':idPueblo', $id);
      $statement->execute();

    header("HTTP/1.1 200 OK");
    exit();
}


//En caso de que ninguna de las opciones anteriores se haya ejecutado
header("HTTP/1.1 400 Bad Request");

?>