<?php
include "config.php";
include "utils.php";


$dbConn =  connect($db);
/*
  listar todos los posts o solo uno
 */
if ($_SERVER['REQUEST_METHOD'] == 'GET')
{
  if (isset($_GET['rol']))
  {
    $sql = $dbConn->prepare("SELECT * FROM qo_menu where rol=:rol and (visible=:vis or visible=3) order by orden");
    $sql->bindValue(':rol', $_GET['rol']);  
    $sql->bindValue(':vis', $_GET['vis']); 
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
    $sql = "INSERT INTO `qo_cuentas`(codigo,fecha,idUsuario,idEstablecimiento,cuentaPedida,cerrada,idZona,mesa,idCuenta) VALUES 
    (:codigo,:fecha,:idUsuario,:idEstablecimiento,:cuentaPedida,:cerrada,:idZona,:mesa,:idCuenta)";
    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':fecha', $input['fecha']);
    $statement->bindValue(':idUsuario', $input['idUsuario']);
    $statement->bindValue(':idEstablecimiento', $input['idEstablecimiento']);
    $statement->bindValue(':cuentaPedida', $input['cuentaPedida']);
    $statement->bindValue(':cerrada', $input['cerrada']);
    $statement->bindValue(':idZona', $input['idZona']);
    $statement->bindValue(':mesa', $input['mesa']);
    $statement->bindValue(':codigo', $input['codigo']);
    $statement->bindValue(':idCuenta', $input['idCuenta']);
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