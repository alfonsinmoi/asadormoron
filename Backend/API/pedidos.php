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
  $sql = $dbConn->prepare("SELECT DISTINCT if (u.idPueblo=0,'',if (u.idPueblo!=z.idPueblo,pu.textoPueblo,'') ) textoPueblo,if(isnull(re.foto),'',re.foto) as fotoRepartidor,p.nombreUsuario,p.tipoPago,p.idRepartidor,p.idCuenta,p.mesa,p.idZonaEstablecimiento,p.zonaEstablecimiento,p.tipoVenta,p.tipo,p.transaccion,p.pagado,if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',p.horaPedido,p.horaEntrega) as horaEntrega,if (isnull(z.nombre),'',z.nombre) as zona,repartidor,concat(u.nombre,' ',u.apellidos) as nombreUsuarioCompleto,if (isnull(p.direccion) or p.direccion='',u.direccion,p.direccion) as direccionUsuario,if (isnull(p.idZona) or p.idZona=0,u.idZona,p.idZona) as idZona,u.email as emailUsuario,u.telefono as telefonoUsuario,p.nuevoPedido,esta.nombre as nombreEstablecimiento,p.id as idPedido,p.codigo as codigoPedido,p.idUsuario,p.idEstablecimiento,DATE_ADD(if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',p.horaPedido,p.horaEntrega), INTERVAL conf.tiempoEntrega MINUTE) as horaPedido,p.comentario,p.estado as idEstadoPedido,est.nombre as estadoPedido ,d.total as precioTotalPedido,if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',p.horaPedido,p.horaEntrega) as fechaEntrega
  FROM `qo_pedidos` p left join qo_zonas z on (z.id=p.idZona) 
  inner join qo_configuracion_est conf on (conf.idEstablecimiento=p.idEstablecimiento)
  inner join (select idPedido,sum(cantidad*precio) total from qo_pedidos_detalle GROUP by idPedido ) d on (d.idPedido=p.id)
  inner join qo_establecimientos esta on (esta.id=p.idEstablecimiento) 
  inner join qo_estados est on (p.estado=est.id) 
  inner join qo_users u on (p.idUsuario=u.id) 
  left join qo_pueblos pu on (pu.id=z.idPueblo)
  left join qo_cuentas cu on (cu.idCuenta=p.idCuenta) 
  left join qo_repartidores re on (re.id=p.idRepartidor)
  Where (cu.id is null or cu.cerrada=0) and  p.idEstablecimiento=:id and p.estado<5 and p.anulado=0 order by p.id");
  $sql->bindValue(':id', $_GET['idEstablecimiento']);  
  $sql->execute();
    $sql->setFetchMode(PDO::FETCH_ASSOC);
    header("HTTP/1.1 200 OK");
    echo json_encode( $sql->fetchAll()  );
    exit();
  }if (isset($_GET['idEstablecimientoMulti']))
  {
  $sql = $dbConn->prepare("SELECT DISTINCT if (u.idPueblo=0,'',if (u.idPueblo!=z.idPueblo,pu.textoPueblo,'') ) textoPueblo,if(isnull(re.foto),'',re.foto) as fotoRepartidor,p.nombreUsuario,p.tipoPago,p.idRepartidor,p.idCuenta,p.mesa,p.idZonaEstablecimiento,p.zonaEstablecimiento,p.tipoVenta,p.tipo,p.transaccion,p.pagado,if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',p.horaPedido,p.horaEntrega) as horaEntrega,if (isnull(z.nombre),'',z.nombre) as zona,repartidor,concat(u.nombre,' ',u.apellidos) as nombreUsuarioCompleto,if (isnull(p.direccion) or p.direccion='',u.direccion,p.direccion) as direccionUsuario,if (isnull(p.idZona) or p.idZona=0,u.idZona,p.idZona) as idZona,u.email as emailUsuario,u.telefono as telefonoUsuario,p.nuevoPedido,esta.nombre as nombreEstablecimiento,p.id as idPedido,p.codigo as codigoPedido,p.idUsuario,p.idEstablecimiento,DATE_ADD(if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',p.horaPedido,p.horaEntrega), INTERVAL conf.tiempoEntrega MINUTE) as horaPedido,p.comentario,p.estado as idEstadoPedido,est.nombre as estadoPedido ,d.total as precioTotalPedido,if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',p.horaPedido,p.horaEntrega) as fechaEntrega
  FROM `qo_pedidos` p left join qo_zonas z on (z.id=p.idZona) 
  inner join qo_configuracion_est conf on (conf.idEstablecimiento=p.idEstablecimiento)
  inner join (select idPedido,sum(cantidad*precio) total from qo_pedidos_detalle GROUP by idPedido ) d on (d.idPedido=p.id)
  inner join qo_establecimientos esta on (esta.id=p.idEstablecimiento) 
  inner join qo_estados est on (p.estado=est.id) 
  inner join qo_users u on (p.idUsuario=u.id) 
  left join qo_pueblos pu on (pu.id=z.idPueblo)
  left join qo_cuentas cu on (cu.idCuenta=p.idCuenta) 
  left join qo_repartidores re on (re.id=p.idRepartidor)
  Where (cu.id is null or cu.cerrada=0) and  p.idEstablecimiento in (".$_GET['idEstablecimientoMulti'].") and p.estado<4 and p.anulado=0 order by p.id");
  $sql->execute();
    $sql->setFetchMode(PDO::FETCH_ASSOC);
    header("HTTP/1.1 200 OK");
    echo json_encode( $sql->fetchAll()  );
    exit();
  }else if (isset($_GET['idEstablecimiento2']))
  {
  $sql = $dbConn->prepare("SELECT DISTINCT p.nombreUsuario,p.tipoPago,p.idCuenta,p.mesa,p.idZonaEstablecimiento,p.zonaEstablecimiento,p.tipoVenta,p.tipo,p.transaccion,p.pagado,
  if(isnull(re.foto),'',re.foto) as fotoRepartidor, p.idRepartidor,if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',p.horaPedido,p.horaEntrega) as horaEntrega,if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',DATE_ADD(p.horaPedido, INTERVAL con.tiempoEntrega MINUTE),DATE_ADD(p.horaEntrega, INTERVAL con.tiempoEntrega MINUTE)) as horaEntrega2,concat(u.nombre,' ',u.apellidos) as nombreUsuarioCompleto,if (isnull(p.direccion) or p.direccion='',u.direccion,p.direccion) as direccionUsuario,if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',p.horaPedido,p.horaEntrega) as fechaEntrega, 
  if (isnull(z.nombre),'',z.nombre) as zona, if (isnull(z.color),'#5d38bc',z.color) as colorZona,repartidor,u.email as emailUsuario,u.telefono as telefonoUsuario,
  p.direccion as direccionUsuario,if (isnull(p.idZona) or p.idZona=0,u.idZona,p.idZona) as idZona, p.nuevoPedido,esta.nombre as nombreEstablecimiento,p.id as idPedido,p.codigo as codigoPedido,
  p.idUsuario,p.idEstablecimiento,if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',p.horaPedido,p.horaEntrega) as horaPedido,d.total as precioTotalPedido,
  p.comentario,p.estado as idEstadoPedido,est.nombre as estadoPedido
  FROM `qo_pedidos` p left join qo_zonas z on (z.id=p.idZona) 
  inner join (select idPedido,sum(cantidad*precio) total from qo_pedidos_detalle GROUP by idPedido ) d on (d.idPedido=p.id)
  inner join qo_establecimientos esta on (esta.id=p.idEstablecimiento) 
  inner join qo_configuracion_est con on (con.idEstablecimiento=p.idEstablecimiento)
  inner join qo_estados est on (p.estado=est.id) 
  inner join qo_users u on (p.idUsuario=u.id)
  left join qo_repartidores re on (re.id=p.idRepartidor)  Where p.idEstablecimiento=:id and p.estado<4 and p.anulado=0 order by p.horaEntrega,p.id");
  $sql->bindValue(':id', $_GET['idEstablecimiento2']);  
  $sql->execute();
    $sql->setFetchMode(PDO::FETCH_ASSOC);
    header("HTTP/1.1 200 OK");
    echo json_encode( $sql->fetchAll()  );
    exit();
  }else if (isset($_GET['idUsuarioCuenta']))
  {
  $sql = $dbConn->prepare("SELECT * FROM qo_cuentas where idUsuario=:idUsuario and cerrada=0");
  $sql->bindValue(':idUsuario', $_GET['idUsuarioCuenta']);  
  $sql->execute();
    header("HTTP/1.1 200 OK");
    echo json_encode(  $sql->fetch(PDO::FETCH_ASSOC)  );
    exit();
  }else if (isset($_GET['idCuentaMesa']))
  {
  $sql = $dbConn->prepare("SELECT * FROM qo_cuentas where idCuenta=:idCuentaMesa and cerrada=0");
  $sql->bindValue(':idCuentaMesa', $_GET['idCuentaMesa']);  
  $sql->execute();
    header("HTTP/1.1 200 OK");
    echo json_encode(  $sql->fetch(PDO::FETCH_ASSOC)  );
    exit();
  }else if (isset($_GET['idEstablecimientoCuentas']))
  {
    $sql = $dbConn->prepare("SELECT c.id,c.fecha,c.idUsuario,c.idEstablecimiento,c.fechaPago,c.cuentaPedida,c.cerrada,c.idZona,c.mesa,z.nombre as transaccion, c.idCuenta,c.codigo FROM `qo_cuentas` c inner join qo_establecimientos_zonas z on (z.id=c.idZona) WHERE c.cuentaPedida=1 and c.cerrada=0 and c.idEstablecimiento=:id GROUP BY c.idCuenta");
    $sql->bindValue(':id', $_GET['idEstablecimientoCuentas']);  
    $sql->execute();
    $sql->setFetchMode(PDO::FETCH_ASSOC);
    header("HTTP/1.1 200 OK");
    echo json_encode( $sql->fetchAll()  );
    exit();
  }else if (isset($_GET['idCuentaPedida']))
  {
    $sql = $dbConn->prepare("SELECT * FROM `qo_cuentas` WHERE cuentaPedida=1 and idCuenta=:id");
    $sql->bindValue(':id', $_GET['idCuentaPedida']);  
    $sql->execute();
    $row_cnt = $sql->num_rows;
    echo $row_cnt;
    $result=true;
    if ($row_cnt==0)
      $result=false;
    header("HTTP/1.1 200 OK");
    echo json_encode( $result);
    exit();
  }else if (isset($_GET['idUsuario']))
  {
  $sql = $dbConn->prepare("SELECT DISTINCT p.nombreUsuario,p.tipoPago,p.idCuenta,p.mesa,p.idZonaEstablecimiento,p.zonaEstablecimiento,p.tipoVenta,p.tipo,p.transaccion,p.pagado,if(isnull(re.pin),'',re.pin) as fotoRepartidor, p.idRepartidor,if(isnull(z.color),'#5d38bc',z.color) as colorZona, if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',p.horaPedido,p.horaEntrega) as horaEntrega,if (isnull(z.nombre),'',z.nombre) as zona,repartidor,concat(u.nombre,' ',u.apellidos) as nombreUsuarioCompleto,if (isnull(p.direccion) or p.direccion='',u.direccion,p.direccion) as direccionUsuario,if (isnull(p.idZona) or p.idZona=0,u.idZona,p.idZona) as idZona,u.email as emailUsuario,u.telefono as telefonoUsuario,p.nuevoPedido,cat.tipo as tipoProducto,esta.nombre as nombreEstablecimiento,p.id as idPedido,p.codigo as codigoPedido,p.idUsuario,p.idEstablecimiento,d.id as idDetalle,p.horaPedido,d.idProducto,concat('(',SUBSTRING(cat.nombre,1,3),') ',pr.nombre) as nombreProducto,pr.descripcion as descripcionProducto,pr.imagen as imagenProducto,p.comentario,p.estado as idEstadoPedido,est.nombre as estadoPedido, d.cantidad,d.precio,(d.cantidad*d.precio) as importe FROM `qo_pedidos` p left join qo_zonas z on (z.id=p.idZona) inner join qo_pedidos_detalle d on (p.id=d.idPedido) inner join qo_establecimientos esta on (esta.id=p.idEstablecimiento) inner join qo_productos_est pr on (pr.id=d.idProducto) inner join qo_productos_cat cat on (cat.id=pr.idCategoria) inner join qo_estados est on (p.estado=est.id) left join qo_repartidores re on (re.id=p.idRepartidor)  inner join qo_users u on (p.idUsuario=u.id) Where p.idUsuario=:id and p.estado>=4 and p.anulado=0 order by p.id,d.id,d.estado");
  $sql->bindValue(':id', $_GET['idUsuario']);  
  $sql->execute();
    $sql->setFetchMode(PDO::FETCH_ASSOC);
    header("HTTP/1.1 200 OK");
    echo json_encode( $sql->fetchAll()  );
    exit();
  }else if (isset($_GET['idCuenta']))
  {
  $sql = $dbConn->prepare("SELECT DISTINCT p.nombreUsuario,p.tipoPago,p.idCuenta,p.mesa,p.idZonaEstablecimiento,p.zonaEstablecimiento,p.tipoVenta,p.tipo,p.transaccion,p.pagado,if(isnull(re.pin),'',re.pin) as fotoRepartidor, p.idRepartidor,if(isnull(z.color),'#5d38bc',z.color) as colorZona, if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',p.horaPedido,p.horaEntrega) as horaEntrega,if (isnull(z.nombre),'',z.nombre) as zona,repartidor,concat(u.nombre,' ',u.apellidos) as nombreUsuarioCompleto,if (isnull(p.direccion) or p.direccion='',u.direccion,p.direccion) as direccionUsuario,if (isnull(p.idZona) or p.idZona=0,u.idZona,p.idZona) as idZona,u.email as emailUsuario,u.telefono as telefonoUsuario,p.nuevoPedido,cat.tipo as tipoProducto,esta.nombre as nombreEstablecimiento,p.id as idPedido,p.codigo as codigoPedido,p.idUsuario,p.idEstablecimiento,d.id as idDetalle,p.horaPedido,d.idProducto,concat('(',SUBSTRING(cat.nombre,1,3),') ',pr.nombre) as nombreProducto,pr.descripcion as descripcionProducto,pr.imagen as imagenProducto,p.comentario,p.estado as idEstadoPedido,est.nombre as estadoPedido, d.cantidad,d.precio,(d.cantidad*d.precio) as importe FROM `qo_pedidos` p left join qo_zonas z on (z.id=p.idZona) inner join qo_pedidos_detalle d on (p.id=d.idPedido) inner join qo_establecimientos esta on (esta.id=p.idEstablecimiento) inner join qo_productos_est pr on (pr.id=d.idProducto) inner join qo_productos_cat cat on (cat.id=pr.idCategoria) inner join qo_estados est on (p.estado=est.id) left join qo_repartidores re on (re.id=p.idRepartidor)  inner join qo_users u on (p.idUsuario=u.id) Where p.idCuenta=:id and p.anulado=0 order by p.id,d.id,d.estado");
  $sql->bindValue(':id', $_GET['idCuenta']);  
  $sql->execute();
    $sql->setFetchMode(PDO::FETCH_ASSOC);
    header("HTTP/1.1 200 OK");
    echo json_encode( $sql->fetchAll()  );
    exit();
  }else if (isset($_GET['historicoAdmin']))
  {
  $sql = $dbConn->prepare("SELECT DISTINCT if(p.estado=1,'#000000',if(p.estado=2,'#4fa9d2',if (p.estado=3,'#ff5800',if(p.estado=4,'#0fa046','#4d4fa0')))) as ColorPedido,p.nombreUsuario,p.repartidor,p.idRepartidor,if(isnull(re.foto),'',re.foto) as fotoRepartidor,p.tipoPago,p.tipoVenta,if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',p.horaPedido,p.horaEntrega) as horaEntrega,concat(u.nombre,' ',u.apellidos) as nombreUsuarioCompleto,if (isnull(p.direccion) or p.direccion='',u.direccion,p.direccion) as direccionUsuario,if (isnull(p.idZona) or p.idZona=0,u.idZona,p.idZona) as idZona,u.email as emailUsuario,u.telefono as telefonoUsuario,p.nuevoPedido,esta.nombre as nombreEstablecimiento,p.id as idPedido,p.codigo as codigoPedido,p.idUsuario,p.idEstablecimiento,p.horaPedido,p.comentario,p.estado as idEstadoPedido,est.nombre as estadoPedido,d.total as precioTotalPedido FROM `qo_pedidos` p inner join qo_establecimientos esta on (esta.id=p.idEstablecimiento) inner join qo_estados est on (p.estado=est.id) inner join qo_users u on (p.idUsuario=u.id) left join qo_repartidores re on (re.id=p.idRepartidor) inner join (select idPedido,sum(cantidad*precio) total from qo_pedidos_detalle Where pagadoConPuntos=0 GROUP by idPedido) d on (d.idPedido=p.id) 
  Where ((p.estado=4 and p.repartidor=0) or p.estado=5) and p.anulado=0  and esta.idGrupo=:idGrupo and ((esta.idPueblo=:idPueblo) or (esta.idPueblo<>:idPueblo and esta.visibleFuera=1)) and p.horaPedido>DATE_SUB(now(), INTERVAL :numero DAY) order by p.id");
  $sql->bindValue(':idGrupo', $_GET['idGrupo']);
  $sql->bindValue(':idPueblo', $_GET['idPueblo']); 
  $sql->bindValue(':numero', $_GET['numero']*15);  
  $sql->execute();
    $sql->setFetchMode(PDO::FETCH_ASSOC);
    header("HTTP/1.1 200 OK");
    echo json_encode( $sql->fetchAll()  );
    exit();
  }else if (isset($_GET['historicoMultiAdmin']))
  {
  $ids=$_GET['ids'];
  $sql = $dbConn->prepare("SELECT DISTINCT pu.nombre as poblacion,if(p.estado=1,'#000000',if(p.estado=2,'#4fa9d2',if (p.estado=3,'#ff5800',if(p.estado=4,'#0fa046','#4d4fa0')))) as ColorPedido,p.nombreUsuario,p.repartidor,p.idRepartidor,if(isnull(re.foto),'',re.foto) as fotoRepartidor,p.tipoPago,p.tipoVenta,if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',p.horaPedido,p.horaEntrega) as horaEntrega,concat(u.nombre,' ',u.apellidos) as nombreUsuarioCompleto,if (isnull(p.direccion) or p.direccion='',u.direccion,p.direccion) as direccionUsuario,if (isnull(p.idZona) or p.idZona=0,u.idZona,p.idZona) as idZona,u.email as emailUsuario,u.telefono as telefonoUsuario,p.nuevoPedido,esta.nombre as nombreEstablecimiento,p.id as idPedido,p.codigo as codigoPedido,p.idUsuario,p.idEstablecimiento,p.horaPedido,p.comentario,p.estado as idEstadoPedido,est.nombre as estadoPedido,d.total as precioTotalPedido FROM `qo_pedidos` p inner join qo_establecimientos esta on (esta.id=p.idEstablecimiento) inner join qo_pueblos pu on (pu.id=esta.idPueblo) inner join qo_estados est on (p.estado=est.id) inner join qo_users u on (p.idUsuario=u.id) left join qo_repartidores re on (re.id=p.idRepartidor) inner join (select idPedido,sum(cantidad*precio) total from qo_pedidos_detalle Where pagadoConPuntos=0 GROUP by idPedido) d on (d.idPedido=p.id) 
  Where ((p.estado=4 and p.repartidor=0) or p.estado=5) and p.anulado=0  and p.horaPedido>DATE_SUB(now(), INTERVAL :numero DAY) order by p.id");
  $sql->bindValue(':numero', $_GET['numero']*15);  
  $sql->execute();
    $sql->setFetchMode(PDO::FETCH_ASSOC);
    header("HTTP/1.1 200 OK");
    echo json_encode( $sql->fetchAll()  );
    exit();
  }else if (isset($_GET['historicoAnuladosMultiAdmin']))
  {
  $ids=$_GET['ids'];
  $sql = $dbConn->prepare("SELECT DISTINCT pu.nombre as poblacion,if(p.estado=1,'#000000',if(p.estado=2,'#4fa9d2',if (p.estado=3,'#ff5800',if(p.estado=4,'#0fa046','#4d4fa0')))) as ColorPedido,p.nombreUsuario,p.repartidor,p.idRepartidor,if(isnull(re.foto),'',re.foto) as fotoRepartidor,p.tipoPago,p.tipoVenta,if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',p.horaPedido,p.horaEntrega) as horaEntrega,concat(u.nombre,' ',u.apellidos) as nombreUsuarioCompleto,if (isnull(p.direccion) or p.direccion='',u.direccion,p.direccion) as direccionUsuario,if (isnull(p.idZona) or p.idZona=0,u.idZona,p.idZona) as idZona,u.email as emailUsuario,u.telefono as telefonoUsuario,p.nuevoPedido,esta.nombre as nombreEstablecimiento,p.id as idPedido,p.codigo as codigoPedido,p.idUsuario,p.idEstablecimiento,p.horaPedido,p.comentario,p.estado as idEstadoPedido,est.nombre as estadoPedido,d.total as precioTotalPedido FROM `qo_pedidos` p inner join qo_establecimientos esta on (esta.id=p.idEstablecimiento) inner join qo_pueblos pu on (pu.id=esta.idPueblo) inner join qo_estados est on (p.estado=est.id) inner join qo_users u on (p.idUsuario=u.id) left join qo_repartidores re on (re.id=p.idRepartidor) inner join (select idPedido,sum(cantidad*precio) total from qo_pedidos_detalle Where pagadoConPuntos=0 GROUP by idPedido) d on (d.idPedido=p.id) 
  Where p.anulado=1  and p.horaPedido>DATE_SUB(now(), INTERVAL :numero DAY) order by p.id");
  $sql->bindValue(':numero', $_GET['numero']*15);  
  $sql->execute();
    $sql->setFetchMode(PDO::FETCH_ASSOC);
    header("HTTP/1.1 200 OK");
    echo json_encode( $sql->fetchAll()  );
    exit();
  }else if (isset($_GET['historicoGastosAdmin']))
  {
  $ids=$_GET['ids'];
  $sql = $dbConn->prepare("SELECT g.*,r.nombre as nombreRepartidor FROM qo_gastos g inner join qo_repartidores r on (r.id=g.idRepartidor)
  Where  g.fecha>DATE_SUB(now(), INTERVAL :numero DAY) order by g.id");
  $sql->bindValue(':numero', $_GET['numero']*15);  
  $sql->execute();
    $sql->setFetchMode(PDO::FETCH_ASSOC);
    header("HTTP/1.1 200 OK");
    echo json_encode( $sql->fetchAll()  );
    exit();
  }else if (isset($_GET['historicoAdminPuntos']))
  {
  $sql = $dbConn->prepare("SELECT DISTINCT if(p.estado=1,'#000000',if(p.estado=2,'#4fa9d2',if (p.estado=3,'#ff5800',if(p.estado=4,'#0fa046','#4d4fa0')))) as ColorPedido,p.nombreUsuario,p.repartidor,p.idRepartidor,if(isnull(re.foto),'',re.foto) as fotoRepartidor,p.tipoPago,p.tipoVenta,if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',p.horaPedido,p.horaEntrega) as horaEntrega,concat(u.nombre,' ',u.apellidos) as nombreUsuarioCompleto,if (isnull(p.direccion) or p.direccion='',u.direccion,p.direccion) as direccionUsuario,if (isnull(p.idZona) or p.idZona=0,u.idZona,p.idZona) as idZona,u.email as emailUsuario,u.telefono as telefonoUsuario,p.nuevoPedido,esta.nombre as nombreEstablecimiento,p.id as idPedido,p.codigo as codigoPedido,p.idUsuario,p.idEstablecimiento,p.horaPedido,p.comentario,p.estado as idEstadoPedido,est.nombre as estadoPedido,d.total as precioTotalPedido FROM `qo_pedidos` p inner join qo_establecimientos esta on (esta.id=p.idEstablecimiento) inner join qo_estados est on (p.estado=est.id) inner join qo_users u on (p.idUsuario=u.id) left join qo_repartidores re on (re.id=p.idRepartidor) inner join (select idPedido,sum(cantidad*precio) total from qo_pedidos_detalle Where pagadoConPuntos=1 GROUP by idPedido) d on (d.idPedido=p.id) 
  Where ((p.estado=4 and p.repartidor=0) or p.estado=5) and p.anulado=0  and esta.idGrupo=:idGrupo and ((esta.idPueblo=:idPueblo) or (esta.idPueblo<>:idPueblo and esta.visibleFuera=1)) order by p.id");
  $sql->bindValue(':idGrupo', $_GET['idGrupo']);
  $sql->bindValue(':idPueblo', $_GET['idPueblo']);  
  $sql->execute();
    $sql->setFetchMode(PDO::FETCH_ASSOC);
    header("HTTP/1.1 200 OK");
    echo json_encode( $sql->fetchAll()  );
    exit();
  }else if (isset($_GET['historicoMultiAdminPuntos']))
  {
    $ids=$_GET['ids'];
  $sql = $dbConn->prepare("SELECT DISTINCT if(p.estado=1,'#000000',if(p.estado=2,'#4fa9d2',if (p.estado=3,'#ff5800',if(p.estado=4,'#0fa046','#4d4fa0')))) as ColorPedido,p.nombreUsuario,p.repartidor,p.idRepartidor,if(isnull(re.foto),'',re.foto) as fotoRepartidor,p.tipoPago,p.tipoVenta,if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',p.horaPedido,p.horaEntrega) as horaEntrega,concat(u.nombre,' ',u.apellidos) as nombreUsuarioCompleto,if (isnull(p.direccion) or p.direccion='',u.direccion,p.direccion) as direccionUsuario,if (isnull(p.idZona) or p.idZona=0,u.idZona,p.idZona) as idZona,u.email as emailUsuario,u.telefono as telefonoUsuario,p.nuevoPedido,esta.nombre as nombreEstablecimiento,p.id as idPedido,p.codigo as codigoPedido,p.idUsuario,p.idEstablecimiento,p.horaPedido,p.comentario,p.estado as idEstadoPedido,est.nombre as estadoPedido,d.total as precioTotalPedido FROM `qo_pedidos` p inner join qo_establecimientos esta on (esta.id=p.idEstablecimiento) inner join qo_estados est on (p.estado=est.id) inner join qo_users u on (p.idUsuario=u.id) left join qo_repartidores re on (re.id=p.idRepartidor) inner join (select idPedido,sum(cantidad*precio) total from qo_pedidos_detalle Where pagadoConPuntos=1 GROUP by idPedido) d on (d.idPedido=p.id) 
  Where ((p.estado=4 and p.repartidor=0) or p.estado=5) and p.anulado=0  and esta.idPueblo in ($ids) order by p.id");
  $sql->execute();
    $sql->setFetchMode(PDO::FETCH_ASSOC);
    header("HTTP/1.1 200 OK");
    echo json_encode( $sql->fetchAll()  );
    exit();
  }else if (isset($_GET['idUsuarioHistorico']))
  {
  $sql = $dbConn->prepare("SELECT DISTINCT p.tipoPago,p.idCuenta,p.mesa,p.idZonaEstablecimiento,p.zonaEstablecimiento,p.tipoVenta,p.tipo,p.transaccion,p.pagado,if(isnull(re.foto),'',re.foto) as fotoRepartidor, p.nombreUsuario,p.idRepartidor,if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',p.horaPedido,p.horaEntrega) as horaEntrega, if (isnull(z.nombre),'',z.nombre) as zona, if (isnull(z.color),'#5d38bc',z.color) as colorZona,repartidor,concat(u.nombre,' ',u.apellidos) as nombreUsuarioCompleto,if (isnull(p.direccion) or p.direccion='',u.direccion,p.direccion) as direccionUsuario,if (isnull(p.idZona) or p.idZona=0,u.idZona,p.idZona) as idZona,u.email as emailUsuario,u.telefono as telefonoUsuario, p.nuevoPedido,if (isnull(cat.tipo),0,cat.tipo) as tipoProducto,esta.nombre as nombreEstablecimiento,p.id as idPedido,p.codigo as codigoPedido,p.idUsuario,p.idEstablecimiento,d.id as idDetalle,if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',p.horaPedido,p.horaEntrega) as horaPedido,d.idProducto,if(isnull(pr.nombre),if (d.tipo=1,'GASTOS DE ENVÍO',if(d.tipo=3,'DESCUENTO',d.concepto)),concat('(',SUBSTRING(cat.nombre,1,3),') ',pr.nombre)) as nombreProducto,if (isnull(pr.descripcion),'',pr.descripcion) as descripcionProducto,if (isnull(pr.imagen),'',pr.imagen) as imagenProducto,p.comentario,p.estado as idEstadoPedido,est.nombre as estadoPedido, d.cantidad,d.precio,(d.cantidad*d.precio) as importe FROM `qo_pedidos` p left join qo_zonas z on (z.id=p.idZona) inner join qo_pedidos_detalle d on (p.id=d.idPedido) inner join qo_establecimientos esta on (esta.id=p.idEstablecimiento) left join qo_productos_est pr on (pr.id=d.idProducto) left join qo_productos_cat cat on (cat.id=pr.idCategoria) inner join qo_estados est on (p.estado=est.id) left join qo_repartidores re on (re.id=p.idRepartidor) inner join qo_users u on (p.idUsuario=u.id) Where p.idUsuario=:id and p.anulado=0  order by p.id,d.id,d.estado");
  $sql->bindValue(':id', $_GET['idUsuarioHistorico']);  
  $sql->execute();
    $sql->setFetchMode(PDO::FETCH_ASSOC);
    header("HTTP/1.1 200 OK");
    echo json_encode( $sql->fetchAll()  );
    exit();
  }else if (isset($_GET['codigoPedido']))
  {
    $sql = $dbConn->prepare("SELECT DISTINCT if (isnull(pu.nombre),pu2.nombre,pu.nombre) as poblacion,if(isnull(cat.numeroImpresora),1,cat.numeroImpresora) as numeroImpresora,p.nombreUsuario,p.tipoPago,p.idCuenta,p.mesa,p.idZonaEstablecimiento,p.zonaEstablecimiento,p.tipoVenta,p.tipo,if (isnull(p.transaccion),'',p.transaccion) as transaccion,p.pagado,if(isnull(re.foto),'',re.foto) as fotoRepartidor, p.idRepartidor,if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',p.horaPedido,p.horaEntrega) as horaEntrega,p.horaPedido as horaRealizacion, z.nombre as zona, z.color as colorZona,repartidor,concat(u.nombre,' ',u.apellidos) as nombreUsuarioCompleto,if (isnull(p.direccion) or p.direccion='',u.direccion,p.direccion) as direccionUsuario,if (isnull(p.idZona) or p.idZona=0,u.idZona,p.idZona) as idZona,u.email as emailUsuario,u.telefono as telefonoUsuario, p.nuevoPedido,if (isnull(cat.tipo),0,cat.tipo) as tipoProducto,esta.nombre as nombreEstablecimiento,p.id as idPedido,p.codigo as codigoPedido,p.idUsuario,p.idEstablecimiento,d.id as idDetalle,d.comentario as comentarioProducto,p.horaPedido as horaPedido,d.idProducto,if(isnull(pr.nombre),if (d.tipo=1,'GASTOS DE ENVÍO',if(d.tipo=3,'DESCUENTO',d.concepto)),if(isnull(d.concepto),pr.nombre,d.concepto)) as nombreProducto,if (isnull(pr.descripcion),'',pr.descripcion) as descripcionProducto,if (isnull(pr.imagen),'',pr.imagen) as imagenProducto,p.comentario,p.estado as idEstadoPedido,est.nombre as estadoPedido, d.cantidad,d.precio,(d.cantidad*d.precio) as importe 
    FROM `qo_pedidos` p 
    left join qo_zonas z on (z.id=p.idZona) 
    inner join qo_pedidos_detalle d on (p.id=d.idPedido) 
    inner join qo_establecimientos esta on (esta.id=p.idEstablecimiento) 
    left join qo_productos_est pr on (pr.id=d.idProducto) 
    left join qo_productos_cat cat on (cat.id=pr.idCategoria) 
    inner join qo_estados est on (p.estado=est.id) 
    left join qo_repartidores re on (re.id=p.idRepartidor) 
    inner join qo_users u on (p.idUsuario=u.id) 
    left join qo_pueblos pu on (pu.id=z.idPueblo) 
    inner join qo_pueblos pu2 on (pu2.id=u.idPueblo) Where p.codigo=:codigo order by p.id,d.id,d.estado");
    $sql->bindValue(':codigo', $_GET['codigoPedido']);  
  $sql->execute();
  $sql->setFetchMode(PDO::FETCH_ASSOC);
    header("HTTP/1.1 200 OK");
    echo json_encode( $sql->fetchAll()  );
    exit();
  }else if (isset($_GET['idPedido']))
  {
  $sql = $dbConn->prepare("SELECT DISTINCT p.nombreUsuario,p.tipoPago,p.idCuenta,p.mesa,p.idZonaEstablecimiento,p.zonaEstablecimiento,p.tipoVenta,p.tipo,p.transaccion,p.pagado,if(isnull(re.foto),'',re.foto) as fotoRepartidor, p.idRepartidor,if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',p.horaPedido,p.horaEntrega) as horaEntrega, if (isnull(z.nombre),'',z.nombre) as zona, if (isnull(z.color),'#5d38bc',z.color) as colorZona,repartidor,concat(u.nombre,' ',u.apellidos) as nombreUsuarioCompleto,if (isnull(p.direccion) or p.direccion='',u.direccion,p.direccion) as direccionUsuario,if (isnull(p.idZona) or p.idZona=0,u.idZona,p.idZona) as idZona,u.email as emailUsuario,u.telefono as telefonoUsuario, p.nuevoPedido,if (isnull(cat.tipo),0,cat.tipo) as tipoProducto,esta.nombre as nombreEstablecimiento,p.id as idPedido,p.codigo as codigoPedido,p.idUsuario,p.idEstablecimiento,d.id as idDetalle,p.horaentrega as horaPedido,d.idProducto,if(isnull(pr.nombre),if (d.tipo=1,'GASTOS DE ENVÍO',if(d.tipo=3,'DESCUENTO',d.concepto)),concat('(',SUBSTRING(cat.nombre,1,3),') ',if(isnull(d.concepto),pr.nombre,d.concepto))) as nombreProducto,if (isnull(pr.descripcion),'',pr.descripcion) as descripcionProducto,if (isnull(pr.imagen),'',pr.imagen) as imagenProducto,p.comentario,p.estado as idEstadoPedido,est.nombre as estadoPedido, d.cantidad,d.precio,(d.cantidad*d.precio) as importe FROM `qo_pedidos` p  left join qo_zonas z on (z.id=p.idZona) inner join qo_pedidos_detalle d on (p.id=d.idPedido) inner join qo_establecimientos esta on (esta.id=p.idEstablecimiento) left join qo_productos_est pr on (pr.id=d.idProducto) left join qo_productos_cat cat on (cat.id=pr.idCategoria) inner join qo_estados est on (p.estado=est.id) left join qo_repartidores re on (re.id=p.idRepartidor) inner join qo_users u on (p.idUsuario=u.id) Where p.id=:id and p.anulado=0 order by p.id,d.id,d.estado");
  $sql->bindValue(':id', $_GET['idPedido']);  
  $sql->execute();
  header("HTTP/1.1 200 OK");
  echo json_encode(  $sql->fetch(PDO::FETCH_ASSOC)  );
    exit();
  }else if (isset($_GET['superAdmin']))
  {
    $ids=$_GET['superAdmin'];
    $sql = $dbConn->prepare("SELECT DISTINCT if(isnull(u.id),pu.textoPueblo,if(pu.id<>pu2.id,pu2.textoPueblo,pu.textoPueblo)) as textoPueblo,if(isnull(u.id),pu.colorPueblo,p.nombreUsuario,p.tipoPago,p.idCuenta,p.mesa,p.idZonaEstablecimiento,p.zonaEstablecimiento,p.tipoVenta,p.tipo,p.transaccion,p.pagado,
    if(isnull(re.foto),'',re.foto) as fotoRepartidor, p.idRepartidor,if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',p.horaPedido,p.horaEntrega) as horaEntrega,if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',DATE_ADD(p.horaPedido, INTERVAL con.tiempoEntrega MINUTE),DATE_ADD(p.horaEntrega, INTERVAL con.tiempoEntrega MINUTE)) as horaEntrega2,if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',p.horaPedido,p.horaEntrega) as fechaEntrega, 
    if (isnull(z.nombre),'RECOGIDA',z.nombre) as zona, if (isnull(z.color),'#5d38bc',z.color) as colorZona,repartidor,
    p.direccion as direccionUsuario,p.idZona as idZona, p.nuevoPedido,esta.nombre as nombreEstablecimiento,p.id as idPedido,p.codigo as codigoPedido,
    p.idUsuario,p.idEstablecimiento,if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',p.horaPedido,p.horaEntrega) as horaPedido,d.total as precioTotalPedido,
    p.comentario,p.estado as idEstadoPedido,est.nombre as estadoPedido
    FROM `qo_pedidos` p left join qo_zonas z on (z.id=p.idZona) 
    left join qo_users u on (u.id=p.idUsuario)
    inner join (select idPedido,sum(cantidad*precio) total from qo_pedidos_detalle GROUP by idPedido ) d on (d.idPedido=p.id)
    inner join qo_establecimientos esta on (esta.id=p.idEstablecimiento) 
    inner join qo_configuracion_est con on (con.idEstablecimiento=p.idEstablecimiento)
    inner join qo_estados est on (p.estado=est.id) 
    inner join qo_pueblos pu on (pu.id=esta.idPueblo) 
    left join qo_pueblos pu2 on (pu2.id=u.idPueblo) 
    left join qo_repartidores re on (re.id=p.idRepartidor) 
    Where p.estado<5 and p.completo=1 and esta.idPueblo in ($ids) and p.anulado=0 and (p.tipo=1 or p.tipo=2) and p.tipoVenta <>'Local' order by p.horaEntrega,p.id");
    $sql->execute();
    $sql->setFetchMode(PDO::FETCH_ASSOC);
    header("HTTP/1.1 200 OK");
    echo json_encode( $sql->fetchAll()  );
    exit();
  }else if (isset($_GET['multiAdmin']))
  {
    $ids = $_GET['idPueblos'];
    $sql = $dbConn->prepare("SELECT DISTINCT if(isnull(u.id),pu.textoPueblo,if(pu.id<>pu2.id,pu2.textoPueblo,pu.textoPueblo)) as textoPueblo,if(isnull(u.id),pu.colorPueblo,if(pu.id<>pu2.id,pu2.colorPueblo,pu.colorPueblo)) as colorPueblo,p.nombreUsuario,p.tipoPago,p.idCuenta,p.mesa,p.idZonaEstablecimiento,p.zonaEstablecimiento,p.tipoVenta,p.tipo,p.transaccion,p.pagado,
    if(isnull(re.foto),'',re.foto) as fotoRepartidor, p.idRepartidor,if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',p.horaPedido,p.horaEntrega) as horaEntrega,if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',DATE_ADD(p.horaPedido, INTERVAL con.tiempoEntrega MINUTE),DATE_ADD(p.horaEntrega, INTERVAL con.tiempoEntrega MINUTE)) as horaEntrega2,if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',p.horaPedido,p.horaEntrega) as fechaEntrega, 
    if (isnull(z.nombre),'RECOGIDA',z.nombre) as zona, if (isnull(z.color),'#5d38bc',z.color) as colorZona,repartidor,
    p.direccion as direccionUsuario,p.idZona as idZona, p.nuevoPedido,esta.nombre as nombreEstablecimiento,p.id as idPedido,p.codigo as codigoPedido,
    p.idUsuario,p.idEstablecimiento,if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',p.horaPedido,p.horaEntrega) as horaPedido,d.total as precioTotalPedido,
    p.comentario,p.estado as idEstadoPedido,est.nombre as estadoPedido
    FROM `qo_pedidos` p 
    left join qo_zonas z on (z.id=p.idZona) 
    left join qo_users u on (u.id=p.idUsuario)
    inner join (select idPedido,sum(cantidad*precio) total from qo_pedidos_detalle GROUP by idPedido ) d on (d.idPedido=p.id)
    inner join qo_establecimientos esta on (esta.id=p.idEstablecimiento) 
    inner join qo_configuracion_est con on (con.idEstablecimiento=p.idEstablecimiento)
    inner join qo_estados est on (p.estado=est.id) 
    inner join qo_pueblos pu on (pu.id=esta.idPueblo) 
    left join qo_pueblos pu2 on (pu2.id=z.idPueblo) 
    left join qo_repartidores re on (re.id=p.idRepartidor) 
    Where p.estado<5 and p.completo=1 and p.anulado=0 and esta.idPueblo in ($ids) and (p.tipo=1 or p.tipo=2) and p.tipoVenta <>'Local' order by p.horaEntrega,p.id");

    $sql->execute();
    $sql->setFetchMode(PDO::FETCH_ASSOC);
    header("HTTP/1.1 200 OK");
    echo json_encode( $sql->fetchAll()  );
    exit();
  }else
  {
    $sql = $dbConn->prepare("SELECT DISTINCT p.nombreUsuario,p.tipoPago,p.idCuenta,p.mesa,p.idZonaEstablecimiento,p.zonaEstablecimiento,p.tipoVenta,p.tipo,p.transaccion,p.pagado,
    if(isnull(re.foto),'',re.foto) as fotoRepartidor, p.idRepartidor,if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',p.horaPedido,p.horaEntrega) as horaEntrega,if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',DATE_ADD(p.horaPedido, INTERVAL con.tiempoEntrega MINUTE),DATE_ADD(p.horaEntrega, INTERVAL con.tiempoEntrega MINUTE)) as horaEntrega2,if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',p.horaPedido,p.horaEntrega) as fechaEntrega, 
    if (isnull(z.nombre),'RECOGIDA',z.nombre) as zona, if (isnull(z.color),'#5d38bc',z.color) as colorZona,repartidor,
    p.direccion as direccionUsuario,p.idZona as idZona, p.nuevoPedido,esta.nombre as nombreEstablecimiento,p.id as idPedido,p.codigo as codigoPedido,
    p.idUsuario,p.idEstablecimiento,if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',p.horaPedido,p.horaEntrega) as horaPedido,d.total as precioTotalPedido,
    p.comentario,p.estado as idEstadoPedido,est.nombre as estadoPedido
    FROM `qo_pedidos` p left join qo_zonas z on (z.id=p.idZona) 
    inner join (select idPedido,sum(cantidad*precio) total from qo_pedidos_detalle GROUP by idPedido ) d on (d.idPedido=p.id)
    inner join qo_establecimientos esta on (esta.id=p.idEstablecimiento) 
    inner join qo_configuracion_est con on (con.idEstablecimiento=p.idEstablecimiento)
    inner join qo_estados est on (p.estado=est.id) 
    left join qo_repartidores re on (re.id=p.idRepartidor) 
    Where p.estado<5 and p.completo=1 and p.anulado=0 and esta.idGrupo=:idGrupo  and ((esta.idPueblo=:idPueblo) or (esta.idPueblo<>:idPueblo and esta.visibleFuera=1)) and (p.tipo=1 or p.tipo=2) and p.tipoVenta <>'Local' order by p.horaEntrega,p.id");
    $sql->bindValue(':idGrupo', $_GET['idGrupo']);  
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
    if (isset($_GET['cuenta']))
  {
    $sql = "INSERT INTO `qo_cuentas`(codigo,fecha,idUsuario,idEstablecimiento,cuentaPedida,cerrada,idZona,mesa,idCuenta) VALUES 
    (:codigo,:fecha,:idUsuario,:idEstablecimiento,:cuentaPedida,:cerrada,:idZona,:mesa,0)";
    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':fecha', $input['fecha']);
    $statement->bindValue(':idUsuario', $input['idUsuario']);
    $statement->bindValue(':idEstablecimiento', $input['idEstablecimiento']);
    $statement->bindValue(':cuentaPedida', $input['cuentaPedida']);
    $statement->bindValue(':cerrada', $input['cerrada']);
    $statement->bindValue(':idZona', $input['idZona']);
    $statement->bindValue(':mesa', $input['mesa']);
    $statement->bindValue(':codigo', $input['codigo']);
    $statement->execute();
    $postId = $dbConn->lastInsertId();
    if($postId)
    {
      $sql = "UPDATE `qo_cuentas` SET `idCuenta`=$postId WHERE id=$postId";
      $statement = $dbConn->prepare($sql);
      $statement->execute();
      $input['id'] = $postId;
      $input['idCuenta'] = $postId;
      header("HTTP/1.1 200 OK");
      echo json_encode($input);
      exit();
	 }
  }else if (isset($_GET['pagoCuenta'])){
    $id = $input['idCuenta'];
    $orden=$input['codigo'];
    $sql = "UPDATE `qo_cuentas` p SET p.cerrada=1,p.fechaPago=now(),transaccion='$orden' WHERE  idCuenta=$id";
    $statement = $dbConn->prepare($sql);
    $statement->execute();
    $sql = "UPDATE `qo_pedidos` p SET p.estado=5,p.pagado=1,transaccion='$orden' WHERE  idCuenta=$id";
    $statement = $dbConn->prepare($sql);
    $statement->execute();
  }else  if (isset($_GET['valoraPedido']))
  {
    if ($input['comentario']==null)
    $input['comentario']='';
    $sql="INSERT INTO `qo_valoracion_pedidos`(`idUsuario`, `idPedido`, `valoracionEstablecimiento`, `valoracionServicio`, `valoracionPuntualidad`, `valoracionRepartidor`, `comentario`, `fecha`) VALUES (:idUsuario,:idPedido,:valoracionEstablecimiento,:valoracionServicio,:valoracionPuntualidad,:valoracionRepartidor,:comentario,:fecha)";
    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':idUsuario', $input['idUsuario']);
    $statement->bindValue(':idPedido', $input['idPedido']);
    $statement->bindValue(':valoracionEstablecimiento', $input['valoracionEstablecimiento']);
    $statement->bindValue(':valoracionServicio', $input['valoracionServicio']);
    $statement->bindValue(':valoracionPuntualidad', $input['valoracionPuntualidad']);
    $statement->bindValue(':valoracionRepartidor', $input['valoracionRepartidor']);
    $statement->bindValue(':comentario', $input['comentario']);
    $statement->bindValue(':fecha', $input['fecha']);
    //bindAllValues($statement, $input);
    $statement->execute();
    $postId = $dbConn->lastInsertId();

    $id=$input['idPedido'];
    $sql = "UPDATE `qo_pedidos` SET valorado=1 WHERE id=$id";
      $statement = $dbConn->prepare($sql);
      $statement->execute();
    header("HTTP/1.1 200 OK");
      echo json_encode($input);
      exit();
  }else  if (isset($_GET['autoPedido']))
  {

    $sql = "INSERT INTO `qo_users`(version,idSocial,social,idZona,pin,`nombre`, `apellidos`, `dni`, `cod_postal`, `poblacion`, `provincia`, `direccion`, `fechaNacimiento`, `fechaAlta`, `telefono`, `email`, `password`, `username`, `foto`, `rol`, `estado`, `plataforma`, `token`, `tipoRegistro`,idPueblo) 
    VALUES ('','','',:idZona,'',:nombre,:apellidos,'','','','',:direccion,now(),now(),:telefono,'','','','',1,0,'','',0,0)";
    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':idZona', $input['idZona']);
    $statement->bindValue(':nombre', $input['nombre']);
    $statement->bindValue(':apellidos', $input['apellidos']);
    $statement->bindValue(':direccion', $input['direccion']);
    $statement->bindValue(':telefono', $input['telefono']);
    $statement->execute();
    $idUsuario = $dbConn->lastInsertId();

    if ($input['idZona']=='0')
      $input['idZona']=10;
      $miEstado=3;
      if (isset($_GET['estado'])){
        $miEstado=$_GET['estado'];
      }else{
        $miEstado=3;
      }
      if (isset($input['tipoPago'])){
        $miTipoPago=$input['tipoPago'];
      }else{
        $miTipoPago="Efectivo";
      }
    $sql = "INSERT INTO `qo_pedidos`(nombreUsuario,tipoPago,idCuenta,mesa,idZonaEstablecimiento,zonaEstablecimiento,tipoVenta,codigo, idEstablecimiento, horaPedido, idUsuario, estado,idZona,nuevoPedido,direccion,comentario,horaEntrega,transaccion,pagado,tipo,valorado) VALUES 
    (:nombreUsuario,:tipoPago,:idCuenta,:mesa,:idZonaEstablecimiento,:zonaEstablecimiento,:tipoVenta,:codigo,:idEstablecimiento,now(),:idUsuario,:estado,:idZona,1,:direccion,:comentario,:horaEntrega,:transaccion,:pagado,:tipo,0)";
    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':codigo', $input['codigo']);
    $statement->bindValue(':nombreUsuario', $input['nombre'].' '.$input['apellidos']);
    $statement->bindValue(':tipo', 1);
    $statement->bindValue(':tipoPago', $miTipoPago);
    $statement->bindValue(':idEstablecimiento', $input['idEstablecimiento']);
    $statement->bindValue(':idUsuario', $idUsuario);
    $statement->bindValue(':idZona', $input['idZona']);
    $statement->bindValue(':transaccion', '');
    $statement->bindValue(':pagado', 0);
    $statement->bindValue(':direccion', $input['direccion']);
    $statement->bindValue(':comentario', '');
    $statement->bindValue(':horaEntrega', $input['hora']);
    $statement->bindValue(':tipoVenta','Envío');
    $statement->bindValue(':mesa', 0);
    $statement->bindValue(':estado', 2);
    $statement->bindValue(':idCuenta',0);
    $statement->bindValue(':idZonaEstablecimiento', 0);
    $statement->bindValue(':zonaEstablecimiento', '');
    //bindAllValues($statement, $input);
    $statement->execute();
    $postId = $dbConn->lastInsertId();

    $sql = "INSERT INTO `qo_pedidos_detalle`(tipoVenta,`idPedido`, `idProducto`, `precio`, `cantidad`,tipo,concepto) VALUES (:tipoVenta,:idPedido,:idProducto,:precio,:cantidad,:tipo,:concepto)";
    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':idPedido', $postId);
    $statement->bindValue(':idProducto', 0);
    $statement->bindValue(':precio', $input['importe']);
    $statement->bindValue(':cantidad', 1);
    $statement->bindValue(':tipo', 0);
    $statement->bindValue(':concepto', 'AUTO PEDIDO');
    $statement->bindValue(':tipoVenta', 'Envío');
    $statement->execute();

    $sql = "INSERT INTO `qo_pedidos_detalle`(tipoVenta,`idPedido`, `idProducto`, `precio`, `cantidad`,tipo,concepto) VALUES (:tipoVenta,:idPedido,:idProducto,:precio,:cantidad,:tipo,:concepto)";
    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':idPedido', $postId);
    $statement->bindValue(':idProducto', 0);
    $statement->bindValue(':precio', $input['importeZona']);
    $statement->bindValue(':cantidad', 1);
    $statement->bindValue(':tipo', 1);
    $statement->bindValue(':concepto', 'Gastos de Envío');
    $statement->bindValue(':tipoVenta', 'Envío');
    $statement->execute();

    if($postId)
    {
      header("HTTP/1.1 200 OK");
      echo $postId;
      exit();
	  }
  }else  if (isset($_GET['autoPedido2']))
  {

    $sql = "INSERT INTO `qo_users`(version,idSocial,social,idZona,pin,`nombre`, `apellidos`, `dni`, `cod_postal`, `poblacion`, `provincia`, `direccion`, `fechaNacimiento`, `fechaAlta`, `telefono`, `email`, `password`, `username`, `foto`, `rol`, `estado`, `plataforma`, `token`, `tipoRegistro`,idPueblo) 
    VALUES ('','','',:idZona,'',:nombre,:apellidos,'',:codPostal,:poblacion,:provincia,:direccion,now(),now(),:telefono,'','','','',1,0,'','',0,:idPueblo)";
    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':idZona', $input['idZona']);
    $statement->bindValue(':nombre', $input['nombre']);
    $statement->bindValue(':apellidos', $input['apellidos']);
    $statement->bindValue(':direccion', $input['direccion']);
    $statement->bindValue(':telefono', $input['telefono']);
    $statement->bindValue(':idPueblo', $input['idPueblo']);
    $statement->bindValue(':codPostal', $input['codPostal']);
    $statement->bindValue(':poblacion', $input['poblacion']);
    $statement->bindValue(':provincia', $input['provincia']);
    $statement->execute();
    $idUsuario = $dbConn->lastInsertId();

    if ($input['idZona']=='0')
      $input['idZona']=10;
      $miEstado=3;
      if (isset($_GET['estado'])){
        $miEstado=$_GET['estado'];
      }else{
        $miEstado=3;
      }
      if (isset($input['tipoPago'])){
        $miTipoPago=$input['tipoPago'];
      }else{
        $miTipoPago="Efectivo";
      }
    $sql = "INSERT INTO `qo_pedidos`(nombreUsuario,tipoPago,idCuenta,mesa,idZonaEstablecimiento,zonaEstablecimiento,tipoVenta,codigo, idEstablecimiento, horaPedido, idUsuario, estado,idZona,nuevoPedido,direccion,comentario,horaEntrega,transaccion,pagado,tipo,valorado) VALUES 
    (:nombreUsuario,:tipoPago,:idCuenta,:mesa,:idZonaEstablecimiento,:zonaEstablecimiento,:tipoVenta,:codigo,:idEstablecimiento,now(),:idUsuario,:estado,:idZona,1,:direccion,:comentario,:horaEntrega,:transaccion,:pagado,:tipo,0)";
    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':codigo', $input['codigo']);
    $statement->bindValue(':nombreUsuario', $input['nombre'].' '.$input['apellidos']);
    $statement->bindValue(':tipo', 1);
    $statement->bindValue(':tipoPago', $miTipoPago);
    $statement->bindValue(':idEstablecimiento', $input['idEstablecimiento']);
    $statement->bindValue(':idUsuario', $idUsuario);
    $statement->bindValue(':idZona', $input['idZona']);
    $statement->bindValue(':transaccion', '');
    $statement->bindValue(':pagado', 0);
    $statement->bindValue(':direccion', $input['direccion']);
    $statement->bindValue(':comentario', '');
    $statement->bindValue(':horaEntrega', $input['hora']);
    $statement->bindValue(':tipoVenta','Envío');
    $statement->bindValue(':mesa', 0);
    $statement->bindValue(':estado', 2);
    $statement->bindValue(':idCuenta',0);
    $statement->bindValue(':idZonaEstablecimiento', 0);
    $statement->bindValue(':zonaEstablecimiento', '');
    //bindAllValues($statement, $input);
    $statement->execute();
    $postId = $dbConn->lastInsertId();

    $sql = "INSERT INTO `qo_pedidos_detalle`(tipoVenta,`idPedido`, `idProducto`, `precio`, `cantidad`,tipo,concepto) VALUES (:tipoVenta,:idPedido,:idProducto,:precio,:cantidad,:tipo,:concepto)";
    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':idPedido', $postId);
    $statement->bindValue(':idProducto', 0);
    $statement->bindValue(':precio', $input['importe']);
    $statement->bindValue(':cantidad', 1);
    $statement->bindValue(':tipo', 0);
    $statement->bindValue(':concepto', 'AUTO PEDIDO');
    $statement->bindValue(':tipoVenta', 'Envío');
    $statement->execute();

    $sql = "INSERT INTO `qo_pedidos_detalle`(tipoVenta,`idPedido`, `idProducto`, `precio`, `cantidad`,tipo,concepto) VALUES (:tipoVenta,:idPedido,:idProducto,:precio,:cantidad,:tipo,:concepto)";
    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':idPedido', $postId);
    $statement->bindValue(':idProducto', 0);
    $statement->bindValue(':precio', $input['importeZona']);
    $statement->bindValue(':cantidad', 1);
    $statement->bindValue(':tipo', 1);
    $statement->bindValue(':concepto', 'Gastos de Envío');
    $statement->bindValue(':tipoVenta', 'Envío');
    $statement->execute();

    if($postId)
    {
      header("HTTP/1.1 200 OK");
      echo $postId;
      exit();
	  }
  }else  if (isset($_GET['autoPedido3']))
  {
    $sql = "INSERT INTO `qo_pedidos`(completo,nombreUsuario,tipoPago,idCuenta,mesa,idZonaEstablecimiento,zonaEstablecimiento,tipoVenta,codigo, idEstablecimiento, horaPedido, idUsuario, estado,idZona,nuevoPedido,direccion,comentario,horaEntrega,transaccion,pagado,tipo,valorado,repartidor,idRepartidor) VALUES 
    (1,:nombreUsuario,'Efectivo',0,0,0,'','Envío',:codigo,67,now(),:idUsuario,4,:idZona,1,:direccion,:comentario,:horaEntrega,:transaccion,:pagado,:tipo,0,1,:idRepartidor)";
    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':nombreUsuario', 'Auto Pedido');
    $statement->bindValue(':codigo', $input['codigo']);
    $statement->bindValue(':idUsuario', $input['idUsuario']);
    $statement->bindValue(':tipo', 1);
    $statement->bindValue(':idZona', $input['idZona']);
    $statement->bindValue(':transaccion', '');
    $statement->bindValue(':pagado', 0);
    $statement->bindValue(':direccion', $input['direccion']);
    $statement->bindValue(':comentario', $input['comentario']);
    $statement->bindValue(':horaEntrega', $input['hora']);
    $statement->bindValue(':idRepartidor', $_GET['idRepartidor']);
    //bindAllValues($statement, $input);
    $statement->execute();
    $postId = $dbConn->lastInsertId();

    $sql = "INSERT INTO `qo_pedidos_detalle`(tipoVenta,`idPedido`, `idProducto`, `precio`, `cantidad`,tipo,concepto) VALUES (:tipoVenta,:idPedido,:idProducto,:precio,:cantidad,:tipo,:concepto)";
    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':idPedido', $postId);
    $statement->bindValue(':idProducto', 0);
    $statement->bindValue(':precio', $input['importe']);
    $statement->bindValue(':cantidad', 1);
    $statement->bindValue(':tipo', 0);
    $statement->bindValue(':concepto', 'AUTO PEDIDO');
    $statement->bindValue(':tipoVenta', 'Envío');
    $statement->execute();
    if($postId)
    {
      header("HTTP/1.1 200 OK");
      echo $postId;
      exit();
	  }
  }else{
    $sql = "INSERT INTO `qo_pedidos`(nombreUsuario,tipoPago,idCuenta,mesa,idZonaEstablecimiento,zonaEstablecimiento,tipoVenta,codigo, idEstablecimiento, horaPedido, idUsuario, estado,idZona,nuevoPedido,direccion,comentario,horaEntrega,transaccion,pagado,tipo,valorado) VALUES 
    ((SELECT concat(nombre,' ',apellidos) from qo_users where id=:idUsuario),:tipoPago,:idCuenta,:mesa,:idZonaEstablecimiento,:zonaEstablecimiento,:tipoVenta,:codigo,:idEstablecimiento,now(),:idUsuario,2,:idZona,1,:direccion,:comentario,:horaEntrega,:transaccion,:pagado,:tipo,0)";
    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':codigo', $input['codigoPedido']);
    $statement->bindValue(':tipo', $input['tipo']);
    $statement->bindValue(':tipoPago', $input['tipoPago']);
    $statement->bindValue(':idEstablecimiento', $input['idEstablecimiento']);
    $statement->bindValue(':idUsuario', $input['idUsuario']);
    $statement->bindValue(':idZona', $input['idZona']);
    $statement->bindValue(':transaccion', $input['transaccion']);
    $statement->bindValue(':pagado', $input['pagado']);
    $statement->bindValue(':direccion', $input['direccionUsuario']);
    $statement->bindValue(':comentario', $input['comentario']);
    $statement->bindValue(':horaEntrega', $input['horaEntrega']);
    $statement->bindValue(':tipoVenta', $input['tipoVenta']);
$statement->bindValue(':mesa', $input['mesa']);
$statement->bindValue(':idCuenta', $input['idCuenta']);
$statement->bindValue(':idZonaEstablecimiento', $input['idZonaEstablecimiento']);
$statement->bindValue(':zonaEstablecimiento', $input['zonaEstablecimiento']);
    //bindAllValues($statement, $input);
    $statement->execute();
    $postId = $dbConn->lastInsertId();

    foreach ($input['lineasPedidos'] as $linea) {
            if ($linea['comentario']==null)
                $linea['comentario']='';
            if ($linea['tipoComida']==4 && $linea['cantidad']==0){
              $linea['cantidad']=1;
              $linea['precio']='0.2';
            }
            $sql = "INSERT INTO `qo_pedidos_detalle`(comentario,pagadoConPuntos,tipoVenta,`idPedido`, `idProducto`, `precio`, `cantidad`,tipo,concepto) VALUES (:comentario,:pagadoConPuntos,:tipoVenta,:idPedido,:idProducto,:precio,:cantidad,:tipo,:concepto)";
            $statement = $dbConn->prepare($sql);
            $statement->bindValue(':idPedido', $postId);
            $statement->bindValue(':idProducto', $linea['idProducto']);
            $statement->bindValue(':comentario', $linea['comentario']);
            $statement->bindValue(':precio', $linea['precio']);
            $statement->bindValue(':cantidad', $linea['cantidad']);
            $statement->bindValue(':tipo', $linea['tipoComida']);
            $statement->bindValue(':concepto', $linea['nombreProducto']);
            $statement->bindValue(':tipoVenta', $linea['tipoVenta']);
            $statement->bindValue(':pagadoConPuntos', $linea['pagadoConPuntos']);
            $statement->execute();
    }

    if($postId)
    {
      $input['idPedido'] = $postId;
      header("HTTP/1.1 200 OK");
      echo json_encode($input);
      exit();
	 }
  }
}

//Borrar
if ($_SERVER['REQUEST_METHOD'] == 'DELETE')
{
  if (isset($_GET['idEstablecimiento'])){
    $id = $_GET['idEstablecimiento'];
    $statement = $dbConn->prepare("DELETE from qo_productos_cat WHERE idEstablecimiento=$id");
    $statement->bindValue(':id', $id);
    $statement->execute();
    header("HTTP/1.1 200 OK");
    exit();
  }else if (isset($_GET['id'])){
    $id = $_GET['id'];
    $statement = $dbConn->prepare("DELETE from qo_productos_cat WHERE id=$id");
    $statement->bindValue(':id', $id);
    $statement->execute();
    header("HTTP/1.1 200 OK");
    exit();
  }else if (isset($_GET['idPedido'])){
    $idPedido = $_GET['idPedido'];
    $idProducto = $_GET['idProducto'];
    $statement = $dbConn->prepare("DELETE from qo_pedidos_detalle WHERE idProducto=$idProducto and idPedido=$idPedido");
    $statement->bindValue(':id', $id);
    $statement->execute();
    header("HTTP/1.1 200 OK");
    exit();
  }else if (isset($_GET['idPedidoCompleto'])){
    $idPedido = $_GET['idPedidoCompleto'];
    $statement = $dbConn->prepare("DELETE from qo_pedidos WHERE id=$idPedido");
    $statement->execute();
    $statement = $dbConn->prepare("DELETE from qo_pedidos_detalle WHERE idPedido=$idPedido");
    $statement->execute();
    header("HTTP/1.1 200 OK");
    exit();
  }
}

//Actualizar
if ($_SERVER['REQUEST_METHOD'] == 'PUT')
{
  $input = json_decode(file_get_contents('php://input'), true);
  if (isset($_GET['cambiaEstado'])){
    $id = $input['id'];
    if ($input['idEstadoPedido']==99){
      $sql = "UPDATE `qo_pedidos` SET anulado=1 WHERE id=$id";
      $statement = $dbConn->prepare($sql);
      $statement->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode($input);
      exit();
    }else{
      $sql = "UPDATE `qo_pedidos` SET `estado`=:estado WHERE id=$id";
      $statement = $dbConn->prepare($sql);
      $statement->bindValue(':estado', $input['idEstadoPedido']);
      $statement->execute();
      if ($input['idUsuario']!=0){
        $sql = "INSERT INTO `qo_pedidos_estado` (estado,idUsuario,fecha,idPedido) VALUES (:estado,:idUsuario,now(),$id)";
        $statement = $dbConn->prepare($sql);
        $statement->bindValue(':estado', $input['idEstadoPedido']);
        $statement->bindValue(':idUsuario', $input['idUsuario']);
        $statement->execute();
      }
      header("HTTP/1.1 200 OK");
      echo json_encode($input);
      exit();
    }
    
  }else if (isset($_GET['cambiaEstadoNuevo'])){
    $id = $input['id'];
    if ($input['idEstadoPedido']==99){
      $sql = "UPDATE `qo_pedidos` SET anulado=1 WHERE id=$id";
      $statement = $dbConn->prepare($sql);
      $statement->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode($statement->rowCount());
      exit();
    }else{
      $sql = "UPDATE `qo_pedidos` SET `estado`=:estado WHERE id=$id";
      $statement = $dbConn->prepare($sql);
      $statement->bindValue(':estado', $input['idEstadoPedido']);
      $statement->execute();
      $contador=$statement->rowCount();
      if ($input['idUsuario']!=0 && $contador>0){
        $sql = "INSERT INTO `qo_pedidos_estado` (estado,idUsuario,fecha,idPedido) VALUES (:estado,:idUsuario,now(),$id)";
        $statement = $dbConn->prepare($sql);
        $statement->bindValue(':estado', $input['idEstadoPedido']);
        $statement->bindValue(':idUsuario', $input['idUsuario']);
        $statement->execute();
      }
      header("HTTP/1.1 200 OK");
      echo json_encode($contador);
      exit();
    }
    
  }else if (isset($_GET['cambiaTipoVenta'])){
    $id = $input['id'];
    $sql = "UPDATE `qo_pedidos` SET `tipoVenta`=:tipo WHERE id=$id";
      $statement = $dbConn->prepare($sql);
      $statement->bindValue(':tipo', $input['tipoVenta']);
      $statement->execute();
  }else if (isset($_GET['noValorado'])){
    $input = json_decode(file_get_contents('php://input'), true);
    $id = $input['id'];
    $sql = "UPDATE `qo_pedidos` SET valorado=:valorado WHERE id=$id";
      $statement = $dbConn->prepare($sql);
      $statement->bindValue(':valorado', $input['valorado']);
      $statement->execute();
  }else if (isset($_GET['cambiaEstado2'])){
    $codigoPedido = $input['codigoPedido'];
    $sql = "UPDATE `qo_pedidos` SET `estado`=:estado WHERE codigo='$codigoPedido'";
      $statement = $dbConn->prepare($sql);
      $statement->bindValue(':estado', $input['idEstadoPedido']);
      $statement->execute();
      $sql = "INSERT INTO `qo_pedidos_estado` (estado,idUsuario,fecha,idPedido) SELECT :estado,:idUsuario,now(),id FROM qo_pedidos WHERE codigo='$codigoPedido'";
        $statement = $dbConn->prepare($sql);
        $statement->bindValue(':estado', $input['idEstadoPedido']);
        $statement->bindValue(':idUsuario', $input['idUsuario']);
        $statement->execute();
        header("HTTP/1.1 200 OK");
        echo json_encode($input);
        exit();
  }else if (isset($_GET['cambiaEstadoMensaje'])){
    $codigoPedido = $input['codigoPedido'];
    $sql = "UPDATE `qo_mensajes_camarero` SET `visto`=1 WHERE id=$codigoPedido";
      $statement = $dbConn->prepare($sql);
      $statement->execute();
  }else if (isset($_GET['cerrarCuenta'])){
    $id = $input['idCuenta'];
    $sql = "UPDATE `qo_cuentas` p SET p.cerrada=1,p.fechaPago=now() WHERE  idCuenta=$id";
    $statement = $dbConn->prepare($sql);
    $statement->execute();
  }
  else if (isset($_GET['pedirCuenta'])){
    $id = $input['idCuenta'];
    $sql = "UPDATE `qo_cuentas` p SET p.cuentaPedida=1 WHERE  idCuenta=$id";
    $statement = $dbConn->prepare($sql);
    $statement->execute();
  }else if (isset($_GET['pedidoRepartidor'])){
    $id = $input['id'];
    if ($id==0){
      $sql = "UPDATE `qo_pedidos` p SET p.repartidor=0,p.idRepartidor=0 WHERE  p.codigo=:codigo";
    }else{
      $sql = "UPDATE `qo_pedidos` p SET p.repartidor=1,p.idRepartidor=$id WHERE  p.codigo=:codigo";
    }
    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':codigo', $input['codigoPedido']);
    $statement->execute();
  }else{
    $input = json_decode(file_get_contents('php://input'), true);
    $userId = $input['id'];
    $fields = getParams($input);

    $sql = "
          UPDATE qo_productos_cat
          SET $fields
          WHERE id='$userId'
           ";

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