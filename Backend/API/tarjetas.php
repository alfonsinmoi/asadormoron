<?php
include "config.php";
include "utils.php";


$dbConn =  connect($db);
/*
  listar todos los posts o solo uno
 */
if ($_SERVER['REQUEST_METHOD'] == 'GET')
{
  $sql = $dbConn->prepare("SELECT * from qo_users_cards where idUsuario=:id");
    $sql->bindValue(':id', $_GET['idUsuario']);
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
    $sql = "INSERT INTO `qo_users_cards`(`pan`, `cardBrand`, `cardType`, `expiryDate`, `idUsuario`, `idUser`, `tokenUser`) VALUES (:pan,:cardBrand,:cardType,:expiryDate,:idUsuario,:idUser,:tokenUser)";
    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':pan', $input['pan']);
    $statement->bindValue(':cardBrand', $input['cardBrand']);
    $statement->bindValue(':cardType', $input['cardType']);
    $statement->bindValue(':expiryDate', $input['expiryDate']);
    $statement->bindValue(':idUsuario', $input['idUsuario']);
    $statement->bindValue(':idUser', $input['idUser']);
    $statement->bindValue(':tokenUser', $input['tokenUser']);
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
    $statement = $dbConn->prepare("DELETE from qo_users_cards WHERE id=$id");
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
    $sql = "UPDATE `qo_users_cards` SET `nombre`=:nombre,nombre_eng=:nombre,nombre_fr=:nombre,nombre_ger=:nombre, tipo=:tipo, estado=:estado, idEstablecimiento=:idEstablecimiento WHERE id=$id";

    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':nombre', $input['nombre']);
    $statement->bindValue(':tipo', $input['idTipo']);
    $statement->bindValue(':estado', $input['estado']);
    $statement->bindValue(':idEstablecimiento', $input['idEstablecimiento']);
    $statement->execute();

    header("HTTP/1.1 200 OK");
    exit();
}


//En caso de que ninguna de las opciones anteriores se haya ejecutado
header("HTTP/1.1 400 Bad Request");

?>