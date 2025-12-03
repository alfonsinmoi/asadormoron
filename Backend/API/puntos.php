<?php
include "config.php";
include "utils.php";


$dbConn =  connect($db);
/*
  listar todos los posts o solo uno
 */
if ($_SERVER['REQUEST_METHOD'] == 'GET')
{
  if (isset($_GET['puntosEstablecimiento']))
  {
    //Mostrar un post
    $sql = $dbConn->prepare("SELECT puntos as resultado FROM qo_puntos_usuario Where idEstablecimiento=:idEstablecimiento and idUsuario=:idUsuario");
    $sql->bindValue(':idUsuario', $_GET['idUsuario']);
    $sql->bindValue(':idEstablecimiento', $_GET['idEstablecimiento']);
    $sql->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetch(PDO::FETCH_ASSOC)  );
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
      $sql = $dbConn->prepare("UPDATE `qo_puntos_usuario` SET puntos=puntos-:puntos WHERE idEstablecimiento=:idEstablecimiento and idUsuario=:idUsuario");
      $sql->bindValue(':puntos',$input['puntos']);
      $sql->bindValue(':idUsuario',$input['idUsuario']);
      $sql->bindValue(':idEstablecimiento',$input['idEstablecimiento']);
      $sql->execute();
      header("HTTP/1.1 200 OK");
      exit();
}

if ($_SERVER['REQUEST_METHOD'] == 'POST')
{
      $input = json_decode(file_get_contents('php://input'), true);

      $sql = $dbConn->prepare("SELECT * FROM qo_puntos_usuario WHERE idEstablecimiento=:idEstablecimiento and idUsuario=:idUsuario");
      $sql->bindValue(':idUsuario',$input['idUsuario']);
      $sql->bindValue(':idEstablecimiento',$input['idEstablecimiento']);
      $sql->execute();
      $filas = $sql->rowCount();

      $sql = $dbConn->prepare("SELECT SUM(d.cantidad * d.precio) AS total
        FROM qo_pedidos_detalle d
        WHERE d.idPedido = (
            SELECT p.id
            FROM qo_pedidos p
            WHERE p.idUsuario = :idUsuario
            ORDER BY p.id DESC
            LIMIT 1
        )");
      $sql->bindValue(':idUsuario',$input['idUsuario']);
      $sql->execute();

      $total = $sql->fetchColumn();
      $nuevosPuntos= (int)($total * 2);
      if ($filas>0){

        $sql = $dbConn->prepare("UPDATE `qo_puntos_usuario` SET puntos=puntos+:puntos WHERE idEstablecimiento=:idEstablecimiento and idUsuario=:idUsuario");
        $sql->bindValue(':puntos',$nuevosPuntos);
        $sql->bindValue(':idUsuario',$input['idUsuario']);
        $sql->bindValue(':idEstablecimiento',$input['idEstablecimiento']);
        $sql->execute();
      }else{
        $sql = $dbConn->prepare("INSERT `qo_puntos_usuario` (puntos,idUsuario,idEstablecimiento) VALUES (:puntos,:idUsuario,:idEstablecimiento)");
        $sql->bindValue(':puntos',$nuevosPuntos);
        $sql->bindValue(':idUsuario',$input['idUsuario']);
        $sql->bindValue(':idEstablecimiento',$input['idEstablecimiento']);
        $sql->execute();
      }
      header("HTTP/1.1 200 OK");
      exit();
}


//En caso de que ninguna de las opciones anteriores se haya ejecutado
header("HTTP/1.1 400 Bad Request");

?>