<?php
include "config.php";
include "utils.php";


$dbConn =  connect($db);
/*
  listar todos los posts o solo uno
 */
if ($_SERVER['REQUEST_METHOD'] == 'GET')
{
    if (isset($_GET['idUsuario']))
    {
      //Mostrar un post
      $sql = $dbConn->prepare("SELECT * FROM qo_repartidores where idUsuario=:idUsuario and eliminado=0");
      $sql->bindValue(':idUsuario', $_GET['idUsuario']);
      $sql->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetch(PDO::FETCH_ASSOC)  );
      exit();
	  }else if (isset($_GET['idTokenRepartidor']))
    {
      //Mostrar un post
      $sql = $dbConn->prepare("SELECT u.token FROM `qo_users` u inner join qo_repartidores r on (r.idUsuario=u.id) WHERE r.id=:idRepartidor");
      $sql->bindValue(':idRepartidor', $_GET['idTokenRepartidor']);
      $sql->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetch(PDO::FETCH_ASSOC)  );
      exit();
	  }else if (isset($_GET['idRepartidorMensajes']))
    {
      //Mostrar un post
      if ($_GET['admin']==0){
        $sql = $dbConn->prepare("SELECT * FROM `qo_mensajes_repartidor` where idRepartidor=:idRepartidor and date(fechaEnvio)=date(now()) and anulado=0 and idSender=:idSender and admin=0 order by fechaEnvio desc");
        $sql->bindValue(':idRepartidor', $_GET['idRepartidorMensajes']);
        $sql->bindValue(':idSender', $_GET['idSender']);
        $sql->execute();
        $sql->setFetchMode(PDO::FETCH_ASSOC);
        header("HTTP/1.1 200 OK");
        echo json_encode(  $sql->fetchAll()  );
      }else{
        $sql = $dbConn->prepare("SELECT * FROM `qo_mensajes_repartidor` where idRepartidor=:idRepartidor and date(fechaEnvio)=date(now()) and anulado=0 and admin=1 order by fechaEnvio desc");
        $sql->bindValue(':idRepartidor', $_GET['idRepartidorMensajes']);
        $sql->execute();
        $sql->setFetchMode(PDO::FETCH_ASSOC);
        header("HTTP/1.1 200 OK");
        echo json_encode(  $sql->fetchAll()  );
      }
      exit();
	  }else if (isset($_GET['predefinidos']))
    {
      //Mostrar un post
      $sql = $dbConn->prepare("SELECT * FROM `qo_mensajes_repartidor_predef` where estado=1 order by textoCorto asc");
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetchAll()  );
      exit();
	  }else if (isset($_GET['idRepartidorMensajesNoLeidos']))
    {
      //Mostrar un post
      $sql = $dbConn->prepare("SELECT * FROM `qo_mensajes_repartidor` where idRepartidor=:idRepartidor and date(fechaEnvio)=date(now()) and anulado=0 and contestado=0 order by fechaEnvio asc");
      $sql->bindValue(':idRepartidor', $_GET['idRepartidorMensajesNoLeidos']);
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetchAll()  );
      exit();
	  }else if (isset($_GET['idRepartidorUsuario']))
    {
      //Mostrar un post
      $sql = $dbConn->prepare("select id,nombre,activo,foto,pin,idUsuario,idPueblo,idGrupo,telefono from qo_repartidores WHERE idUsuario=:idRepartidor and eliminado=0 order by nombre");
      $sql->bindValue(':idRepartidor', $_GET['idRepartidorUsuario']);
      $sql->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetch(PDO::FETCH_ASSOC)  );
      exit();
	  }else if (isset($_GET['idPedidosRepartidor']))
    {
      //Mostrar un post
      $sql = $dbConn->prepare("SELECT DISTINCT u.provincia as provinciaUsuario,u.poblacion as localidadUsuario,concat(u.nombre,' ',u.apellidos) as nombreUsuario,p.idCuenta,p.mesa,p.idZonaEstablecimiento,p.zonaEstablecimiento,p.tipoVenta,p.tipo,p.transaccion,p.pagado,
      if(isnull(re.foto),'',re.foto) as fotoRepartidor, p.idRepartidor,if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',p.horaPedido,p.horaEntrega) as horaEntrega,if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',p.horaPedido,p.horaEntrega) as fechaEntrega, 
      if (isnull(z.nombre),'',z.nombre) as zona, if (isnull(z.color),'#FFFFFF',z.color) as colorZona,repartidor,
      p.direccion as direccionUsuario,p.idZona as idZona, p.nuevoPedido,esta.nombre as nombreEstablecimiento,p.id as idPedido,p.codigo as codigoPedido,
      p.idUsuario,p.idEstablecimiento,DATE_ADD(if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',p.horaPedido,p.horaEntrega), INTERVAL conf.tiempoEntrega MINUTE) as horaPedido,d.total as precioTotalPedido,
      p.comentario,p.estado as idEstadoPedido,est.nombre as estadoPedido
      FROM `qo_pedidos` p inner join qo_zonas z on (z.id=p.idZona) 
	  inner join qo_configuracion_est conf on (conf.idEstablecimiento=p.idEstablecimiento)
      inner join (select idPedido,sum(cantidad*precio) total from qo_pedidos_detalle GROUP by idPedido ) d on (d.idPedido=p.id)
      inner join qo_establecimientos esta on (esta.id=p.idEstablecimiento) 
      inner join qo_estados est on (p.estado=est.id) 
      inner join qo_users u on (u.id=p.idUsuario)
      left join qo_repartidores re on (re.id=p.idRepartidor and re.eliminado=0) 
      left join qo_repartidores_establecimientos ree on (ree.idEstablecimiento=esta.id and ree.idRepartidor=:idRepartidor)
      Where ((ree.idRepartidor is null and p.idRepartidor=:idRepartidor) or not ree.idRepartidor is null) and  ((p.idRepartidor<>0 and p.estado<5) or (p.idRepartidor=0 and p.estado<=4)) and p.completo=1 and p.anulado=0 and p.tipoVenta='Envío' order by p.id");
      $sql->bindValue(':idRepartidor', $_GET['idPedidosRepartidor']);
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetchAll()  );
      exit();
	  }else if (isset($_GET['idPedidosRepartidor2']))
    {
      //Mostrar un post
      $sql = $dbConn->prepare("SELECT DISTINCT if(isnull(u.id),pu.textoPueblo,if(pu.id<>pu2.id,pu2.textoPueblo,'')) as textoPueblo,if(isnull(u.id),pu.colorPueblo,if(pu.id<>pu2.id,pu2.colorPueblo,pu.colorPueblo)) as colorPueblo,u.provincia as provinciaUsuario,u.poblacion as localidadUsuario,concat(u.nombre,' ',u.apellidos) as nombreUsuario,p.idCuenta,p.mesa,p.idZonaEstablecimiento,p.zonaEstablecimiento,p.tipoPago,p.tipoVenta,p.tipo,p.transaccion,p.pagado,
      if(isnull(re.foto),'',re.foto) as fotoRepartidor, p.idRepartidor,if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',p.horaPedido,p.horaEntrega) as horaEntrega,if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',p.horaPedido,p.horaEntrega) as fechaEntrega, 
      if (isnull(z.nombre),'',z.nombre) as zona, if (isnull(z.color),'#FFFFFF',z.color) as colorZona,repartidor,
      p.direccion as direccionUsuario,p.idZona as idZona, p.nuevoPedido,esta.nombre as nombreEstablecimiento,p.id as idPedido,p.codigo as codigoPedido,
      p.idUsuario,p.idEstablecimiento,DATE_ADD(if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',p.horaPedido,p.horaEntrega), INTERVAL conf.tiempoEntrega MINUTE) as horaPedido,d.total as precioTotalPedido,
      p.comentario,p.estado as idEstadoPedido,est.nombre as estadoPedido
      FROM `qo_pedidos` p inner join qo_zonas z on (z.id=p.idZona) 
	  inner join qo_configuracion_est conf on (conf.idEstablecimiento=p.idEstablecimiento)
      inner join (select idPedido,sum(cantidad*precio) total from qo_pedidos_detalle GROUP by idPedido ) d on (d.idPedido=p.id)
      inner join qo_establecimientos esta on (esta.id=p.idEstablecimiento) 
      inner join qo_estados est on (p.estado=est.id) 
      left join qo_users u on (u.id=p.idUsuario)
      inner join qo_pueblos pu on (pu.id=esta.idPueblo)
      left join qo_pueblos pu2 on (pu2.id=z.idPueblo)
      left join qo_repartidores re on (re.id=p.idRepartidor and re.eliminado=0 and re.idGrupo=pu.idGrupo) 
      left join qo_repartidores_establecimientos ree on (ree.idEstablecimiento=esta.id and ree.idRepartidor=:idRepartidor)
      Where ((ree.idRepartidor is null and p.idRepartidor=:idRepartidor) or not ree.idRepartidor is null) and  ((p.idRepartidor<>0 and p.estado<5) or (p.idRepartidor=0 and p.estado<=4)) and p.completo=1 and p.anulado=0 and p.tipoVenta='Envío' order by p.id");
      $sql->bindValue(':idRepartidor', $_GET['idPedidosRepartidor2']);
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetchAll()  );
      exit();
	  }else if (isset($_GET['rutaPedido']))
    {
      //Mostrar un post
      $sql = $dbConn->prepare("select e.*, pos.* from qo_pedidos p inner join qo_pedidos_estado e on (e.idPedido=p.id and e.estado=4) inner join qo_pedidos_estado e2 on (e2.idPedido=p.id and e2.estado=5) inner join qo_posicion_repartidor_hco pos on (pos.idRepartidor=p.idRepartidor) where p.codigo='CDvgzHZ0' and pos.fecha>=e.fecha and pos.fecha<=e2.fecha order by pos.id");
      //$sql->bindValue(':idRepartidor', $_GET['idPosicionRepartidor']);
      $sql->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetch(PDO::FETCH_ASSOC)  );
      exit();
	  }else if (isset($_GET['idPosicionRepartidor']))
    {
      //Mostrar un post
      $sql = $dbConn->prepare("SELECT latitud as latitude,longitud as longitude FROM `qo_posicion_repartidor` WHERE idRepartidor=:idRepartidor and activo=1 and eliminado=0");
      $sql->bindValue(':idRepartidor', $_GET['idPosicionRepartidor']);
      $sql->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetch(PDO::FETCH_ASSOC)  );
      exit();
	  }else if (isset($_GET['idPosicionRepartidores']))
    {
      //Mostrar un post
      $sql = $dbConn->prepare("SELECT latitud as latitude,longitud as longitude,idRepartidor FROM qo_posicion_repartidor pr inner join qo_repartidores r on (r.id=pr.idRepartidor)  WHERE r.idGrupo=:idGrupo and r.activo=1 and r.eliminado=0");
      $sql->bindValue(':idGrupo', $_GET['idPosicionRepartidores']);
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
 	  else if (isset($_GET['multiAdmin'])){
       //Mostrar lista de post
      $ids= $_GET['idPueblos'];
      $sql = $dbConn->prepare("SELECT * FROM qo_repartidores WHERE idPueblo in($ids) and eliminado=0 ORDER BY nombre");
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
      exit();
     }else {
      //Mostrar lista de post
      $sql = $dbConn->prepare("SELECT * FROM qo_repartidores WHERE idGrupo=:idGrupo and eliminado=0 ORDER BY nombre");
      $sql->bindValue(':idGrupo', $_GET['idGrupo']); 
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
  if (isset($_GET['latitud']))
    {
      //Mostrar un post
      $sql = $dbConn->prepare("UPDATE `qo_posicion_repartidor` SET `latitud`=:latitud,longitud=:longitud,fecha=now() WHERE idRepartidor=:idRepartidor");
      $sql->bindValue(':latitud', $_GET['latitud']);
      $sql->bindValue(':longitud', $_GET['longitud']);
      $sql->bindValue(':idRepartidor', $_GET['idRepartidor']);
      $sql->execute();

      $sql = $dbConn->prepare("INSERT INTO `qo_posicion_repartidor_hco` (`latitud`,longitud,fecha,idRepartidor) values (:latitud,:longitud,now(),:idRepartidor)");
      $sql->bindValue(':latitud', $_GET['latitud']);
      $sql->bindValue(':longitud', $_GET['longitud']);
      $sql->bindValue(':idRepartidor', $_GET['idRepartidor']);
      $sql->execute();

      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetch(PDO::FETCH_ASSOC)  );
      exit();
	  }else if (isset($_GET['mensaje']))
    {
      $input = json_decode(file_get_contents('php://input'), true);
      $sql = "INSERT INTO `qo_mensajes_repartidor` (`idRepartidor`, `mensaje`, `ok`, `contestado`, `fechaEnvio`, `fechaContestacion`, `anulado`, `idSender`,sender,admin) VALUES (:idRepartidor, :mensaje, 0,0, now(), NULL, 0, :idSender,:sender,:admin)";
      $statement = $dbConn->prepare($sql);
      $statement->bindValue(':idRepartidor', $input['idRepartidor']);
      $statement->bindValue(':mensaje', $input['mensaje']);
      $statement->bindValue(':idSender', $input['idSender']);
      $statement->bindValue(':sender', $input['sender']);
      $statement->bindValue(':admin', $input['admin']);
      $statement->execute();
    }else if (isset($_GET['gasto']))
    {
      $input = json_decode(file_get_contents('php://input'), true);
      $sql = "INSERT INTO `qo_gastos` (`idRepartidor`, `concepto`, `precio`, `fecha`) VALUES (:idRepartidor, :concepto, :precio,now())";
      $statement = $dbConn->prepare($sql);
      $statement->bindValue(':idRepartidor', $input['idRepartidor']);
      $statement->bindValue(':concepto', $input['concepto']);
      $statement->bindValue(':precio', $input['precio']);
      $statement->execute();
    }else{
      $input = json_decode(file_get_contents('php://input'), true);
      $sql = "INSERT INTO `qo_repartidores`(nombre,activo,foto,pin,idPueblo,idGrupo,telefono) VALUES (:nombre,:activo,:foto,:pin,:idPueblo,:idGrupo,:telefono)";
      $statement = $dbConn->prepare($sql);
      $statement->bindValue(':nombre', $input['nombre']);
      $statement->bindValue(':activo', $input['activo']);
      $statement->bindValue(':foto', $input['foto']);
      $statement->bindValue(':pin', $input['pin']);
      $statement->bindValue(':telefono', $input['telefono']);
      $statement->bindValue(':idPueblo', $input['idPueblo']);
      $statement->bindValue(':idGrupo', $input['idGrupo']);
      $statement->execute();
      $postId = $dbConn->lastInsertId();

      $sql = "INSERT INTO `qo_posicion_repartidor` (idRepartidor,longitud,latitud) VALUES (:idRepartidor,0,0)";
      $statement = $dbConn->prepare($sql);
      $statement->bindValue(':idRepartidor', $postId);
      $statement->execute();

      if($postId)
      {
        $establecimientos=explode(", ",$_GET['establecimientos']);
        foreach ($establecimientos as $z) {
            $sql="INSERT INTO `qo_repartidores_establecimientos`(`idRepartidor`, `idEstablecimiento`) VALUES ($postId,$z)";
            $statement = $dbConn->prepare($sql);
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
  $statement = $dbConn->prepare("DELETE FROM qo_repartidores where id=:id");
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
    if (isset($_GET['mensaje']))
    {
      $input = json_decode(file_get_contents('php://input'), true);
      $sql = "UPDATE `qo_mensajes_repartidor` SET `ok`=:ok, `contestado`=1, `fechaContestacion`=now() WHERE id=:id";
      $statement = $dbConn->prepare($sql);
      $statement->bindValue(':ok', $input['ok']);
      $statement->bindValue(':id', $input['id']);
      $statement->execute();
    }else{
    $sql = "UPDATE `qo_repartidores` SET `nombre`=:nombre,foto=:foto, activo=:activo,pin=:pin,idPueblo=:idPueblo,idGrupo=:idGrupo,eliminado=:eliminado,telefono=:telefono WHERE id=$id";
    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':nombre', $input['nombre']);
    $statement->bindValue(':activo', $input['activo']);
    $statement->bindValue(':foto', $input['foto']);
    $statement->bindValue(':pin', $input['pin']);
    $statement->bindValue(':idPueblo', $input['idPueblo']);
    $statement->bindValue(':idGrupo', $input['idGrupo']);
    $statement->bindValue(':eliminado', $input['eliminado']);
    $statement->bindValue(':telefono', $input['telefono']);
    $statement->execute();

    $establecimientos=explode(", ",$_GET['establecimientos']);
    $statement = $dbConn->prepare("DELETE FROM qo_repartidores_establecimientos where idRepartidor=:id");
    $statement->bindValue(':id', $id);
    $statement->execute();

    foreach ($establecimientos as $z) {
        $sql="INSERT INTO `qo_repartidores_establecimientos` (`idRepartidor`, `idEstablecimiento`) VALUES ($id,$z)";
        $statement = $dbConn->prepare($sql);
        $statement->execute();
    }
  }
    header("HTTP/1.1 200 OK");
    exit();
}


//En caso de que ninguna de las opciones anteriores se haya ejecutado
header("HTTP/1.1 400 Bad Request");

?>