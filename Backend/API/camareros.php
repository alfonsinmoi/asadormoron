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
      //Mostrar un post
      $sql = $dbConn->prepare("SELECT * FROM qo_camareros where idEstablecimiento=:idEstablecimiento");
      $sql->bindValue(':idEstablecimiento', $_GET['idEstablecimiento']);
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetchAll()  );
      exit();
	  }else if (isset($_GET['idCamareroZona']))
    {
      //Mostrar un post
      $sql = $dbConn->prepare("select c.* from qo_establecimientos_zonas z inner join qo_camareros_zonas c on (c.idZona=z.id) WHERE c.idCamarero=:idCamareroZona");
      $sql->bindValue(':idCamareroZona', $_GET['idCamareroZona']);
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetchAll()  );
      exit();
	  }else if (isset($_GET['idEstablecimientoMesa']))
    {
      //Mostrar un post
      $sql = $dbConn->prepare("SELECT c.* FROM `qo_camareros_zonas` z inner join qo_camareros c on (c.id=z.idCamarero) where idZona=:idZona and inicio<=:mesa and fin>=:mesa");
      $sql->bindValue(':idZona', $_GET['idZona']);
      $sql->bindValue(':mesa', $_GET['mesa']);
      $sql->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetch(PDO::FETCH_ASSOC)  );
      exit();
	  }else if (isset($_GET['idTokenCamarero']))
    {
      //Mostrar un post
      $sql = $dbConn->prepare("SELECT u.token FROM `qo_users` u inner join qo_camareros r on (r.idUsuario=u.id) WHERE r.id=:idCamarero");
      $sql->bindValue(':idCamarero', $_GET['idTokenCamarero']);
      $sql->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetch(PDO::FETCH_ASSOC)  );
      exit();
	  }else if (isset($_GET['idCamareroUsuario']))
    {
      //Mostrar un post
      $sql = $dbConn->prepare("select * from qo_camareros WHERE idUsuario=:idCamarero order by nombre");
      $sql->bindValue(':idCamarero', $_GET['idCamareroUsuario']);
      $sql->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetch(PDO::FETCH_ASSOC)  );
      exit();
	  }else if (isset($_GET['idPedidosCamarero']))
    {
      //Mostrar un post
      $sql = $dbConn->prepare("(SELECT DISTINCT p.idEstablecimiento,p.idZonaEstablecimiento idZona,p.zonaEstablecimiento as zona,p.mesa,concat(u.nombre,' ',u.apellidos) as nombreUsuario,p.codigo as codigoPedido,p.idUsuario,p.horaPedido,p.estado as idEstadoPedido,est.nombre as estadoPedido, GROUP_CONCAT(concat(d.cantidad,' ',d.concepto)) as mensaje FROM `qo_pedidos` p inner join qo_estados est on (p.estado=est.id) inner join qo_users u on (u.id=p.idUsuario) 
      inner join qo_camareros_zonas cz on (cz.idZona=p.idZonaEstablecimiento and cz.inicio<=p.mesa and cz.fin>=p.mesa) 
      inner join qo_camareros cam on (cam.id=cz.idCamarero) inner join qo_pedidos_detalle d on (d.idPedido=p.id)
      inner join qo_cuentas cu on (cu.idCuenta=p.idCuenta)
       Where cam.idUsuario=:idCamarero and p.estado<5 and p.tipoVenta='Local' and cu.cerrada=0 GROUP BY d.idPedido order by p.id) UNION (SELECT idEstablecimiento,idZona,zona,mesa,usuario as nombreUsuario,id as codigoPedido,idUsuario,hora as horaPedido,0 as idEstadoPedido,'Mensaje' as estadoPedido, mensaje FROM qo_mensajes_camarero WHERE visto=0 and idCamarero=:idCamarero order by id)");
      $sql->bindValue(':idCamarero', $_GET['idPedidosCamarero']);
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetchAll()  );
      exit();
	  }else if (isset($_GET['idEstablecimientoRepartidor']))
    {
      //Mostrar un post
      $sql = $dbConn->prepare("SELECT id,idRepartidor,idEstablecimiento FROM qo_repartidores_establecimientos Where idRepartidor=:idRepartidor");
      $sql->bindValue(':idRepartidor', $_GET['idEstablecimientoRepartidor']);
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetchAll()  );
      exit();
	  }
    else {
      //Mostrar lista de post
      $sql = $dbConn->prepare("SELECT * FROM qo_camareros ORDER BY nombre");
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
      if (isset($_GET['mensaje'])){
        $sql = "INSERT INTO `qo_mensajes_camarero`(`idCamarero`, `codigoPedido`, `mensaje`, `visto`, `idUsuario`, `usuario`, `hora`, `mesa`, `idZona`, `zona`, `idEstablecimiento`) VALUES (:idCamarero,:codigoPedido,:mensaje,0,:idUsuario,:nombreUsuario,now(),:mesa,:idZona,:zona,:idEstablecimiento)";
        $statement = $dbConn->prepare($sql);
        $statement->bindValue(':idCamarero', $input['idCamarero']);
        $statement->bindValue(':codigoPedido', $input['codigoPedido']);
        $statement->bindValue(':mensaje', $input['mensaje']);
        $statement->bindValue(':idUsuario', $input['idUsuario']);
        $statement->bindValue(':nombreUsuario', $input['nombreUsuario']);
        $statement->bindValue(':mesa', $input['mesa']);
        $statement->bindValue(':idZona', $input['idZona']);
        $statement->bindValue(':zona', $input['zona']);
        $statement->bindValue(':idEstablecimiento', $input['idEstablecimiento']);
        $statement->execute();
        $postId = $dbConn->lastInsertId();

        if($postId)
        {
          $input['id'] = $postId;
          header("HTTP/1.1 200 OK");
          echo json_encode($input);
          exit();
        }
      }else{
        $sql = "INSERT INTO `qo_camareros`(nombre,activo,foto,idEstablecimiento,idUsuario) VALUES (:nombre,:activo,:foto,:idEstablecimiento,:idUsuario)";
        $statement = $dbConn->prepare($sql);
        $statement->bindValue(':nombre', $input['nombre']);
        $statement->bindValue(':activo', $input['activo']);
        $statement->bindValue(':foto', $input['foto']);
        $statement->bindValue(':idUsuario', $input['idUsuario']);
        $statement->bindValue(':idEstablecimiento', $input['idEstablecimiento']);
        $statement->execute();
        $postId = $dbConn->lastInsertId();

        $sql="UPDATE qo_users set rol=6 where id=:idUsuario";
        $statement = $dbConn->prepare($sql);
        $statement->bindValue(':idUsuario', $input['idUsuario']);
        $statement->execute();

        if($postId)
        {
          $zonas=explode(", ",$_GET['zonas']);
          foreach ($zonas as $z) {
              $mesas=explode("--",$z);
              // CORREGIDO: Prepared statement para evitar SQL Injection
              $sql="INSERT INTO `qo_camareros_zonas`(`idZona`, `idCamarero`, `inicio`, `fin`) VALUES (:idZona, :idCamarero, :inicio, :fin)";
              $statement = $dbConn->prepare($sql);
              $statement->bindValue(':idZona', intval($mesas[0]), PDO::PARAM_INT);
              $statement->bindValue(':idCamarero', $postId, PDO::PARAM_INT);
              $statement->bindValue(':inicio', intval($mesas[1]), PDO::PARAM_INT);
              $statement->bindValue(':fin', intval($mesas[2]), PDO::PARAM_INT);
              $statement->execute();
          }
          $input['id'] = $postId;
          header("HTTP/1.1 200 OK");
          echo json_encode($input);
          exit();
        }
      }
}

if ($_SERVER['REQUEST_METHOD'] == 'DELETE')
{
	$id = $_GET['id'];
  $statement = $dbConn->prepare("DELETE FROM qo_camareros where id=:id");
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

    // CORREGIDO: Prepared statement para evitar SQL Injection
    $sql = "UPDATE `qo_camareros` SET `nombre`=:nombre,foto=:foto, activo=:activo,idEstablecimiento=:idEstablecimiento WHERE id=:id";
    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':nombre', $input['nombre']);
    $statement->bindValue(':activo', $input['activo']);
    $statement->bindValue(':foto', $input['foto']);
    $statement->bindValue(':idEstablecimiento', $input['idEstablecimiento']);
    $statement->bindValue(':id', intval($id), PDO::PARAM_INT);
    $statement->execute();

    $statement = $dbConn->prepare("DELETE FROM qo_camareros_zonas where idCamarero=:id");
    $statement->bindValue(':id', $id);
    $statement->execute();

    $zonas=explode(", ",$_GET['zonas']);
    foreach ($zonas as $z) {
        $mesas=explode("--",$z);
        // CORREGIDO: Prepared statement para evitar SQL Injection
        $sql="INSERT INTO `qo_camareros_zonas`(`idZona`, `idCamarero`, `inicio`, `fin`) VALUES (:idZona, :idCamarero, :inicio, :fin)";
        $statement = $dbConn->prepare($sql);
        $statement->bindValue(':idZona', intval($mesas[0]), PDO::PARAM_INT);
        $statement->bindValue(':idCamarero', intval($id), PDO::PARAM_INT);
        $statement->bindValue(':inicio', intval($mesas[1]), PDO::PARAM_INT);
        $statement->bindValue(':fin', intval($mesas[2]), PDO::PARAM_INT);
        $statement->execute();
    }
    header("HTTP/1.1 200 OK");
    exit();
}


//En caso de que ninguna de las opciones anteriores se haya ejecutado
header("HTTP/1.1 400 Bad Request");

?>