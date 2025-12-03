<?php
include "config.php";
include "utils.php";
$dbConn =  connect($db);
/*
  listar todos los posts o solo uno
 */
if ($_SERVER['REQUEST_METHOD'] == 'GET')
{
  if (isset($_GET['todos']))
    {
      $sql = $dbConn->prepare("SELECT p.*,0 as cantidad FROM qo_pueblos p WHEre activo=1 order by p.nombre");
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
    }else{
      $sql = $dbConn->prepare("SELECT p.*,0 as cantidad FROM qo_administradores_pueblos a inner join qo_pueblos p on (p.id=a.idPueblo) WHERE a.idUser=:idUsuario order by p.nombre");
      $sql->bindValue(':idUsuario', $_GET['idUsuario']);
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
    }
    exit();
}

// Crear un nuevo post
if ($_SERVER['REQUEST_METHOD'] == 'POST')
{  
  exit();
}

//Borrar
if ($_SERVER['REQUEST_METHOD'] == 'DELETE')
{
	exit();
}

//Actualizar
if ($_SERVER['REQUEST_METHOD'] == 'PUT')
{
    $input = json_decode(file_get_contents('php://input'), true);
    if (isset($_GET['idEstablecimiento']))
    {
      $userId = $input['id'];
      $fields = getParams($input);
  
      $sql = "
            UPDATE qo_configuracion_est
            SET $fields
            WHERE id='$userId'
             ";
      $statement = $dbConn->prepare($sql);
      bindAllValues($statement, $input);
      $statement->execute();
    }else if (isset($_GET['general'])){
      $fields = getParams($input);

      $sql = "
            UPDATE qo_configuracion_global
            SET $fields";

      $statement = $dbConn->prepare($sql);
      bindAllValues($statement, $input);

      $statement->execute();
    
    }else{
      $fields = getParams($input);

      $sql = "
            UPDATE qo_configuracion
            SET $fields";

      $statement = $dbConn->prepare($sql);
      bindAllValues($statement, $input);

      $statement->execute();
    }
    header("HTTP/1.1 200 OK");
    exit();
}


//En caso de que ninguna de las opciones anteriores se haya ejecutado
header("HTTP/1.1 400 Bad Request");

?>