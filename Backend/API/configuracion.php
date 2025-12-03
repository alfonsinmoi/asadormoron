<?php
include "config.php";
include "utils.php";
$dbConn =  connect($db);
/*
  listar todos los posts o solo uno
 */
if ($_SERVER['REQUEST_METHOD'] == 'GET')
{
  if (isset($_GET['idEstablecimiento']))
  {
    $sql = $dbConn->prepare("SELECT * FROM qo_configuracion_est WHERE idEstablecimiento=:id");
    $sql->bindValue(':id', $_GET['idEstablecimiento']);
    $sql->execute();
    header("HTTP/1.1 200 OK");
    echo json_encode(  $sql->fetch(PDO::FETCH_ASSOC)  );
  }else if (isset($_GET['general']))
  {
    $sql = $dbConn->prepare("SELECT * FROM qo_configuracion_global");
    $sql->execute();
    header("HTTP/1.1 200 OK");
    echo json_encode(  $sql->fetch(PDO::FETCH_ASSOC)  );
  }else if (isset($_GET['idPueblo']))
  {
    $sql = $dbConn->prepare("SELECT * FROM qo_configuracion WHERE idPueblo=:idPueblo");
    $sql->bindValue(':idPueblo', $_GET['idPueblo']);
    $sql->execute();
    header("HTTP/1.1 200 OK");
    echo json_encode(  $sql->fetch(PDO::FETCH_ASSOC)  );
  }else{
      $sql = $dbConn->prepare("SELECT * FROM qo_configuracion WHERE idGrupo=:idGrupo");
      $sql->bindValue(':idGrupo', $_GET['idGrupo']);
      $sql->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetch(PDO::FETCH_ASSOC)  );
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
      if ($input['idPueblo']==7){
        $input['efectivo']=0;
        $input['datafono']=0;
        $input['bizum']=0;
      }
      $userId = $input['id'];
      $sql = "
            UPDATE qo_configuracion
            SET $fields WHERE id='$userId'";
            
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