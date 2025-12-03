<?php
include "config.php";
include "utils.php";


$dbConn =  connect($db);
/*
  listar todos los posts o solo uno
 */
if ($_SERVER['REQUEST_METHOD'] == 'GET')
{
  if (isset($_GET['idZona']))
  {
    $sql = $dbConn->prepare("SELECT id as idZona,nombre,activo,gastos,pedidoMinimo,color,cambiaDireccion,direccionEnvio,idPueblo,modificable FROM qo_zonas WHERE id=:idZona order by nombre");
    $sql->bindValue(':idZona', $_GET['idZona']);
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
      exit();
  }else{
    $sql = $dbConn->prepare("SELECT id as idZona,nombre,activo,gastos,pedidoMinimo,color,cambiaDireccion,direccionEnvio,idPueblo,modificable FROM qo_zonas WHERE idPueblo=:idPueblo order by nombre");
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
    $sql = "INSERT INTO `qo_zonas`(nombre, activo,gastos,pedidoMinimo,color,cambiaDireccion,direccionEnvio,idPueblo,modificable) VALUES 
                (:nombre,:activo,:gastos,:pedidoMinimo,:color,:cambiaDireccion,:direccion,:idPueblo,:modificable)";
    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':nombre', $input['nombre']);
    $statement->bindValue(':activo', $input['activo']);
    $statement->bindValue(':gastos', $input['gastos']);
    $statement->bindValue(':pedidoMinimo', $input['pedidoMinimo']);
    $statement->bindValue(':cambiaDireccion', $input['cambiaDireccion']);
    $statement->bindValue(':direccion', $input['direccionEnvio']);
    $statement->bindValue(':color', $input['color']);
    $statement->bindValue(':idPueblo', $input['idPueblo']);
    $statement->bindValue(':modificable', $input['modificable']);
    //bindAllValues($statement, $input);
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
    $id = $input['idZona'];
    $fields = getParams($input);

    $sql = "UPDATE `qo_zonas` SET color=:color,cambiaDireccion=:cambiaDireccion,direccionEnvio=:direccion,gastos=:gastos,pedidoMinimo=:pedidoMinimo , `nombre`=:nombre,activo=:activo,modificable=:modificable WHERE id=$id";

    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':nombre', $input['nombre']);
    $statement->bindValue(':activo', $input['activo']);
    $statement->bindValue(':gastos', $input['gastos']);
    $statement->bindValue(':pedidoMinimo', $input['pedidoMinimo']);
    $statement->bindValue(':cambiaDireccion', $input['cambiaDireccion']);
    $statement->bindValue(':direccion', $input['direccionEnvio']);
    $statement->bindValue(':color', $input['color']);
    $statement->bindValue(':modificable', $input['modificable']);
    //bindAllValues($statement, $input);

    $statement->execute();
    header("HTTP/1.1 200 OK");
    exit();
}


//En caso de que ninguna de las opciones anteriores se haya ejecutado
header("HTTP/1.1 400 Bad Request");

?>