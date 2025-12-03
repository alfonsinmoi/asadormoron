<?php
include "config.php";
include "utils.php";


$dbConn =  connect($db);
/*
  listar todos los posts o solo uno
 */
if ($_SERVER['REQUEST_METHOD'] == 'GET')
{
  if (isset($_GET['idUsuarioApp']))
  {
    $sql = $dbConn->prepare("SELECT * FROM qo_valoraciones_usuarios Where tipoValoracion=3 and idUsuario=:idUsuario");
      $sql->bindValue(':idUsuario', $_GET['idUsuarioApp']);
      $sql->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetch(PDO::FETCH_ASSOC)  );
        exit();
  }
}

// Crear un nuevo post
if ($_SERVER['REQUEST_METHOD'] == 'POST')
{
    $input = json_decode(file_get_contents('php://input'), true);
    $sql="INSERT INTO qo_valoraciones_usuarios (`idValoracion`, `tipoValoracion`, `valoracion`, `codigoPedido`, `fecha`, `rechazada`, `comentario`, `idUsuario`) VALUES (:idValoracion,:tipoValoracion,:valoracion, :codigoPedido, now(), 0, :comentario, :idUsuario)";
      $statement = $dbConn->prepare($sql);
      $statement->bindValue(':idUsuario', $input['idUsuario']);
      $statement->bindValue(':comentario', $input['comentario']);
      $statement->bindValue(':codigoPedido', $input['codigoPedido']);
      $statement->bindValue(':valoracion', $input['valoracion']);
      $statement->bindValue(':tipoValoracion', $input['tipoValoracion']);
      $statement->bindValue(':idValoracion', $input['idValoracion']);
      $statement->execute();
      $postId = $dbConn->lastInsertId();
    
      $input['id'] = $postId;
      header("HTTP/1.1 200 OK");
      echo json_encode($input);
      exit();
}

//Borrar
if ($_SERVER['REQUEST_METHOD'] == 'DELETE')
{
	$id = $_GET['id'];
  $statement = $dbConn->prepare("DELETE FROM qo_establecimientos where id=:id");
  $statement->bindValue(':id', $id);
  $statement->execute();
	header("HTTP/1.1 200 OK");
	exit();
}

//Actualizar
if ($_SERVER['REQUEST_METHOD'] == 'PUT')
{
    $input = json_decode(file_get_contents('php://input'), true);
    if (isset($_GET['zona']))
    {
      $id = $input['id'];
      $sql="UPDATE qo_establecimientos_zonas set nombre=:nombre,activo=:activo WHERE id=$id";
      $statement = $dbConn->prepare($sql);
      $statement->bindValue(':nombre', $input['nombre']);
      $statement->bindValue(':activo', $input['activo']);
      $statement->execute();
      $postId = $dbConn->lastInsertId();
    }else
    {
    $id = $input['idEstablecimiento'];
    $sql = "UPDATE `qo_establecimientos` SET esComercio=:esComercio,idZona=:idZona,orden=:orden, logo=:logo,idCategoria=:idCategoria,`local`=:local,
      envio=:envio,recogida=:recogida,llamadaCamarero=:llamadaCamarero,puedeReservar=:puedeReservar,`nombre`=:nombre, `direccion`=:direccion, 
      imagen=:imagen,poblacion=:poblacion,provincia=:provincia, codPostal=:codPostal, tipo=:idTipo, estado=:estado, email=:email, telefono=:telefono, 
      latitud=:latitud, longitud=:longitud,envio=:envio,recogida=:recogida,puedeReservar=:puedeReservar,idPueblo=:idPueblo WHERE id=$id";
    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':esComercio', $input['esComercio']);
    $statement->bindValue(':idPueblo', $input['idPueblo']);
    $statement->bindValue(':idZona', $input['idZona']);
    $statement->bindValue(':orden', $input['orden']);
    $statement->bindValue(':logo', $input['logo']);
    $statement->bindValue(':idCategoria', $input['idCategoria']);
    $statement->bindValue(':local', $input['local']);
    $statement->bindValue(':envio', $input['envio']);
    $statement->bindValue(':recogida', $input['recogida']);
    $statement->bindValue(':llamadaCamarero', $input['llamadaCamarero']);
    $statement->bindValue(':puedeReservar', $input['puedeReservar']);
    $statement->bindValue(':nombre', $input['nombre']);
    $statement->bindValue(':direccion', $input['direccion']);
    $statement->bindValue(':imagen', $input['imagen']);
    $statement->bindValue(':poblacion', $input['poblacion']);
    $statement->bindValue(':provincia', $input['provincia']);
    $statement->bindValue(':codPostal', $input['codPostal']);
    $statement->bindValue(':idTipo', $input['idTipo']);
    $statement->bindValue(':estado', $input['estado']);
    $statement->bindValue(':email', $input['email']);
    $statement->bindValue(':envio', $input['envio']);
      $statement->bindValue(':recogida', $input['recogida']);
      $statement->bindValue(':puedeReservar', $input['puedeReservar']);
    $statement->bindValue(':telefono', $input['telefono']);
    $statement->bindValue(':latitud', $input['latitud']);
    $statement->bindValue(':longitud', $input['longitud']);
    $statement->execute();
    }
    header("HTTP/1.1 200 OK");
    exit();
}


//En caso de que ninguna de las opciones anteriores se haya ejecutado
header("HTTP/1.1 400 Bad Request");

?>