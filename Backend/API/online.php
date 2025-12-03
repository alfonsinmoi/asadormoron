<?php
include "config.php";
include "utils.php";


$dbConn =  connect($db);
/*
  listar todos los posts o solo uno
 */
if ($_SERVER['REQUEST_METHOD'] == 'GET')
{
  $sql="";
  if ($_GET['idGrupo']=='0'){
    $sql = $dbConn->prepare("SELECT distinct o.idUsuario,o.tokenUsuario,o.idPueblo,o.horaInicio,o.horaCierre,o.horaBackground,o.horaResume FROM qo_online o inner join qo_pueblos p on (p.id=o.idPueblo) where not isnull(horaInicio) and isnull(horaBackground) and date(horaInicio)=date(now())");
  }else{
    $sql = $dbConn->prepare("SELECT distinct o.idUsuario,o.tokenUsuario,o.idPueblo,o.horaInicio,o.horaCierre,o.horaBackground,o.horaResume FROM qo_online o inner join qo_pueblos p on (p.id=o.idPueblo) where not isnull(horaInicio) and isnull(horaBackground) and date(horaInicio)=date(now()) and p.idGrupo=:idGrupo");
    $sql->bindValue(':idGrupo', $_GET['idGrupo']);  
  }
    $sql->execute();
    $sql->setFetchMode(PDO::FETCH_ASSOC);
    header("HTTP/1.1 200 OK");
    echo json_encode( $sql->fetchAll()  );
    exit();
}

// Crear un nuevo post
if ($_SERVER['REQUEST_METHOD'] == 'POST')
{
    $input = json_decode(file_get_contents('php://input'), true);
    $sql = "INSERT INTO `qo_online`( `idUsuario`, `tokenUsuario`, `idPueblo`, `horaInicio`) 
    VALUES (:idUsuario,:tokenUsuario,:idPueblo,:horaInicio)";
    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':tokenUsuario', $input['tokenUsuario']);
    $statement->bindValue(':idUsuario', $input['idUsuario']);
    $statement->bindValue(':idPueblo', $input['idPueblo']);
    $statement->bindValue(':horaInicio', $input['horaInicio']);
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
          UPDATE qo_online
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