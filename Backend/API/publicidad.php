<?php
include "config.php";
include "utils.php";


$dbConn =  connect($db);
/*
  listar todos los posts o solo uno
 */
if ($_SERVER['REQUEST_METHOD'] == 'GET')
{
  if (isset($_GET['idPueblo']))
    {
      $sql = $dbConn->prepare("SELECT * FROM qo_publicidad WHERE idPueblo=:idPueblo and estado=1 and numeroVisualizaciones > visualizaciones and now() between fechaDesde and fechaHasta order by preferencia, apariciones, visualizaciones");
      $sql->bindValue(':idPueblo', $_GET['idPueblo']);
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
    $sql = "INSERT INTO `qo_pueblos`(nombre, activo,codPostal,provincia,idUsuario,latitud,longitud) VALUES 
                (:nombre,:activo,:codPostal,:provincia,:idUsuario,:latitud,:longitud,:idGrupo)";
    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':nombre', $input['nombre']);
    $statement->bindValue(':activo', $input['activo']);
    $statement->bindValue(':codPostal', $input['codPostal']);
    $statement->bindValue(':provincia', $input['Provincia']);
    $statement->bindValue(':idUsuario', $input['idUsuario']);
    $statement->bindValue(':latitud', $input['latitud']);
    $statement->bindValue(':longitud', $input['longitud']);
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
    if (isset($_GET['visualizaciones']))
    {
      $sql = "UPDATE `qo_publicidad` SET visualizaciones=visualizaciones+1 WHERE id=$id";
      $statement = $dbConn->prepare($sql);
      $statement->execute();
    }else if (isset($_GET['links']))
    {
      $sql = "UPDATE `qo_publicidad` SET links=links+1 WHERE id=$id";
      $statement = $dbConn->prepare($sql);
      $statement->execute();
    }

      $sql = $dbConn->prepare("SELECT * from qo_publicidad Where id=$id");
      $sql->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetch(PDO::FETCH_ASSOC)  );
    exit();
}


//En caso de que ninguna de las opciones anteriores se haya ejecutado
header("HTTP/1.1 400 Bad Request");

?>