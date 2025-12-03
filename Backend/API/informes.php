<?php
include "config.php";
include "utils.php";


$dbConn =  connect($db);
/*
  listar todos los posts o solo uno
 */
if ($_SERVER['REQUEST_METHOD'] == 'GET')
{
    if (isset($_GET['beneficioDiario']))
    {
      $sql = $dbConn->prepare("select sum(d.cantidad*d.precio*c.comision/100) as pedidos, date(p.horaEntrega) as formato, date(p.horaEntrega) as fecha,date(p.horaEntrega) as fechaHasta,sum(d.cantidad*d.precio*c.comision/100)+d2.total as total,d2.total as gastos from qo_pedidos p inner join qo_pedidos_detalle d on (p.id=d.idPedido)inner join qo_configuracion_est c on (c.idEstablecimiento=p.idEstablecimiento) inner join (select sum(d3.cantidad*d3.precio) as total, date(p3.horaEntrega) as dia from qo_pedidos_detalle d3 inner join qo_pedidos p3 on (p3.id=d3.idPedido) inner join qo_establecimientos e on (p.idEstablecimiento=e.id) WHERE e.idGrupo=:idGrupo and date(p3.horaEntrega)<>'0000-00-00' and d3.tipo=1 GROUP by date(p3.horaEntrega)) d2 on (d2.dia=date(p.horaEntrega)) WHERE date(p.horaEntrega)<>'0000-00-00' and d.tipo=0 GROUP by date(p.horaEntrega) order by p.horaEntrega");
      $sql->bindValue(':idGrupo', $_GET['idGrupo']);
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
      exit();
    }else if (isset($_GET['beneficioSemanal']))
    {
      $sql = $dbConn->prepare("select sum(d.cantidad*d.precio*c.comision/100) as pedidos, concat(week(p.horaEntrega),'/',year(p.horaEntrega)) as formato, date(p.horaEntrega) as fecha,DATE_ADD(date(p.horaEntrega), INTERVAL 6 DAY) as fechaHasta,sum(d.cantidad*d.precio*c.comision/100)+d2.total as total,d2.total as gastos from qo_pedidos p inner join qo_pedidos_detalle d on (p.id=d.idPedido)inner join qo_configuracion_est c on (c.idEstablecimiento=p.idEstablecimiento) inner join (select sum(d3.cantidad*d3.precio) as total, concat(week(p3.horaEntrega),'/',year(p3.horaEntrega)) as formato from qo_pedidos_detalle d3 inner join qo_pedidos p3 on (p3.id=d3.idPedido) inner join qo_establecimientos e on (p.idEstablecimiento=e.id) WHERE e.idGrupo=:idGrupo and date(p3.horaEntrega)<>'0000-00-00' and d3.tipo=1 GROUP by week(p3.horaEntrega),year(p3.horaEntrega)) d2 on (d2.formato=concat(week(p.horaEntrega),'/',year(p.horaEntrega))) WHERE date(p.horaEntrega)<>'0000-00-00' and d.tipo=0 GROUP by week(p.horaEntrega),year(p.horaEntrega) order by p.horaEntrega");
      $sql->bindValue(':idGrupo', $_GET['idGrupo']);
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
      exit();
    }else if(isset($_GET['ticketMedio'])){
	$hasta=$_GET['hasta'];
	$desde=$_GET['desde'];
        $sql = $dbConn->prepare("select sum(cantidad*precio)/(SELECT count(*) from qo_pedidos Where idEstablecimiento=e.id and horaPedido BETWEEN '$desde' and '$hasta') as ticketMedio,p.idEstablecimiento,e.nombre from qo_pedidos_detalle d inner join qo_pedidos p on (p.id=d.idPedido) inner join qo_establecimientos e on (e.id=p.idEstablecimiento) where e.idGrupo=:idGrupo and d.tipo=0 and p.horaPedido BETWEEN '$desde' and '$hasta' group by p.idEstablecimiento order by e.nombre");
        $sql->bindValue(':idGrupo', $_GET['idGrupo']);
        $sql->execute();
        $sql->setFetchMode(PDO::FETCH_ASSOC);
        header("HTTP/1.1 200 OK");
        echo json_encode( $sql->fetchAll()  );
        exit();   
    }else if (isset($_GET['beneficioMensual']))
    {
      $sql = $dbConn->prepare("select sum(d.cantidad*d.precio) as pedidos, concat(month(p.horaEntrega),'/',year(p.horaEntrega)) as formato, concat(year(p.horaEntrega),'-',month(p.horaEntrega),'-1') as fecha,DATE_ADD(DATE_ADD(concat(year(p.horaEntrega),'-',month(p.horaEntrega),'-1'), INTERVAL 1 MONTH),INTERVAL -1 DAY) as fechaHasta,sum(d.cantidad*d.precio)+d2.total as total,d2.total as gastos from qo_pedidos p inner join qo_pedidos_detalle d on (p.id=d.idPedido)inner join qo_configuracion_est c on (c.idEstablecimiento=p.idEstablecimiento) inner join (select sum(d3.cantidad*d3.precio) as total, concat(month(p3.horaEntrega),'/',year(p3.horaEntrega)) as formato from qo_pedidos_detalle d3 inner join qo_pedidos p3 on (p3.id=d3.idPedido) WHERE date(p3.horaEntrega)<>'0000-00-00' and d3.tipo=1 GROUP by month(p3.horaEntrega),year(p3.horaEntrega)) d2 on (d2.formato=concat(month(p.horaEntrega),'/',year(p.horaEntrega))) inner join qo_establecimientos e on (p.idEstablecimiento=e.id) WHERE e.idGrupo=:idGrupo and date(p.horaEntrega)<>'0000-00-00' and d.tipo=0 GROUP by month(p.horaEntrega),year(p.horaEntrega) order by p.horaEntrega");
      $sql->bindValue(':idGrupo', $_GET['idGrupo']);
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
      exit();
    }else if (isset($_GET['ticketDiario']))
    {
      $sql = $dbConn->prepare("select sum(d.cantidad*d.precio*c.comision/100) as pedidos, date(p.horaEntrega) as dia,d2.total as gastos from qo_pedidos p inner join qo_pedidos_detalle d on (p.id=d.idPedido)inner join qo_configuracion_est c on (c.idEstablecimiento=p.idEstablecimiento) inner join (select sum(d3.cantidad*d3.precio) as total, date(p3.horaEntrega) as dia from qo_pedidos_detalle d3 inner join qo_pedidos p3 on (p3.id=d3.idPedido) WHERE date(p3.horaEntrega)<>'0000-00-00' and d3.tipo=1 GROUP by date(p3.horaEntrega)) d2 on (d2.dia=date(p.horaEntrega)) inner join qo_establecimientos e on (p.idEstablecimiento=e.id) WHERE e.idGrupo=:idGrupo date(p.horaEntrega)<>'0000-00-00' and d.tipo=0 and date(p.horaEntrega) between '$desde' and '$hasta' GROUP by date(p.horaEntrega) order by p.horaEntrega");
      $sql->bindValue(':idGrupo', $_GET['idGrupo']);
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
      exit();
    }else if (isset($_GET['ticketSemanal']))
    {
      $sql = $dbConn->prepare("select sum(d.cantidad*d.precio*c.comision/100) as pedidos, date(p.horaEntrega) as dia,d2.total as gastos from qo_pedidos p inner join qo_pedidos_detalle d on (p.id=d.idPedido)inner join qo_configuracion_est c on (c.idEstablecimiento=p.idEstablecimiento) inner join (select sum(d3.cantidad*d3.precio) as total, date(p3.horaEntrega) as dia from qo_pedidos_detalle d3 inner join qo_pedidos p3 on (p3.id=d3.idPedido) WHERE date(p3.horaEntrega)<>'0000-00-00' and d3.tipo=1 GROUP by date(p3.horaEntrega)) d2 on (d2.dia=date(p.horaEntrega)) inner join qo_establecimientos e on (p.idEstablecimiento=e.id) WHERE e.idGrupo=:idGrupo date(p.horaEntrega)<>'0000-00-00' and d.tipo=0 and date(p.horaEntrega) between '$desde' and '$hasta' GROUP by date(p.horaEntrega) order by p.horaEntrega");
      $sql->bindValue(':idGrupo', $_GET['idGrupo']);
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
      exit();
    }else if (isset($_GET['ticketMensual']))
    {
      $sql = $dbConn->prepare("select sum(d.cantidad*d.precio*c.comision/100) as pedidos, date(p.horaEntrega) as dia,d2.total as gastos from qo_pedidos p inner join qo_pedidos_detalle d on (p.id=d.idPedido)inner join qo_configuracion_est c on (c.idEstablecimiento=p.idEstablecimiento) inner join (select sum(d3.cantidad*d3.precio) as total, date(p3.horaEntrega) as dia from qo_pedidos_detalle d3 inner join qo_pedidos p3 on (p3.id=d3.idPedido) WHERE date(p3.horaEntrega)<>'0000-00-00' and d3.tipo=1 GROUP by date(p3.horaEntrega)) d2 on (d2.dia=date(p.horaEntrega)) inner join qo_establecimientos e on (p.idEstablecimiento=e.id) WHERE e.idGrupo=:idGrupo date(p.horaEntrega)<>'0000-00-00' and d.tipo=0 and date(p.horaEntrega) between '$desde' and '$hasta' GROUP by date(p.horaEntrega) order by p.horaEntrega");
      $sql->bindValue(':idGrupo', $_GET['idGrupo']);
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
      exit();
    }else if (isset($_GET['masVendidos']))
    {
      $idEstablecimiento=$_GET['idEstablecimiento'];
      $idZona=$_GET['idZona'];
      $idGrupo=$_GET['idGrupo'];
      $sql="";
      if ($idZona==0 && $idEstablecimiento==0)
        $sql = $dbConn->prepare("select sum(cantidad) as cantidad,p.nombre as producto,e.nombre as establecimiento,e.id as idEstablecimiento from qo_pedidos_detalle d inner join qo_productos_est p on (p.id=d.idProducto) inner join qo_productos_cat c on (c.id=p.idCategoria) inner join qo_establecimientos e on (e.id=c.idEstablecimiento) where e.idGrupo=$idGrupo and d.tipo=0 group by idProducto ORDER BY `cantidad` DESC");
      else if ($idZona!==0 && $idEstablecimiento==0)
      $sql = $dbConn->prepare("select sum(cantidad) as cantidad,p.nombre as producto,e.nombre as establecimiento,e.id as idEstablecimiento from qo_pedidos_detalle d inner join qo_productos_est p on (p.id=d.idProducto) inner join qo_productos_cat c on (c.id=p.idCategoria) inner join qo_establecimientos e on (e.id=c.idEstablecimiento) inner join qo_pedidos pe on (pe.id=d.idPedido) where pe.idZona=$idZona and d.tipo=0 group by idProducto ORDER BY `cantidad` DESC");
      else if ($idZona==0 && $idEstablecimiento!==0)
      $sql = $dbConn->prepare("select sum(cantidad) as cantidad,p.nombre as producto,e.nombre as establecimiento,e.id as idEstablecimiento from qo_pedidos_detalle d inner join qo_productos_est p on (p.id=d.idProducto) inner join qo_productos_cat c on (c.id=p.idCategoria) inner join qo_establecimientos e on (e.id=c.idEstablecimiento) inner join qo_pedidos pe on (pe.id=d.idPedido) where pe.idEstablecimiento=$idEstablecimiento and d.tipo=0 group by idProducto ORDER BY `cantidad` DESC");
      else if ($idZona!==0 && $idEstablecimiento!==0)
      $sql = $dbConn->prepare("select sum(cantidad) as cantidad,p.nombre as producto,e.nombre as establecimiento,e.id as idEstablecimiento from qo_pedidos_detalle d inner join qo_productos_est p on (p.id=d.idProducto) inner join qo_productos_cat c on (c.id=p.idCategoria) inner join qo_establecimientos e on (e.id=c.idEstablecimiento) inner join qo_pedidos pe on (pe.id=d.idPedido) where pe.idZona=$idZona and pe.idEstablecimiento=$idEstablecimiento and d.tipo=0 group by idProducto ORDER BY `cantidad` DESC");
      
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
      exit();
    }else if (isset($_GET['mejoresClientes']))
    {
      $idEstablecimiento=$_GET['idEstablecimiento'];
      $idZona=$_GET['idZona'];
      $idGrupo=$_GET['idGrupo'];
      $sql="";
      if ($idZona==0 && $idEstablecimiento==0)
        $sql = $dbConn->prepare("select p.idUsuario,count(*) as pedidos,u.nombre,u.apellidos,u.email,u.telefono from qo_pedidos p inner join qo_users u on (u.id=p.idUsuario) inner join qo_pueblos pu on (pu.id=u.idPueblo) WHERE pu.idGrupo=$idGrupo group by p.idUsuario ORDER BY `pedidos`  DESC");
      else if ($idZona!==0 && $idEstablecimiento==0)
        $sql = $dbConn->prepare("select p.idUsuario,count(*) as pedidos,u.nombre,u.apellidos,u.email,u.telefono from qo_pedidos p inner join qo_users u on (u.id=p.idUsuario) WHERE p.idZona=$idZona group by p.idUsuario ORDER BY `pedidos`  DESC");
      else if ($idZona==0 && $idEstablecimiento!==0)
        $sql = $dbConn->prepare("select p.idUsuario,count(*) as pedidos,u.nombre,u.apellidos,u.email,u.telefono from qo_pedidos p inner join qo_users u on (u.id=p.idUsuario) WHERE p.idEstablecimiento=$idEstablecimiento group by p.idUsuario ORDER BY `pedidos`  DESC");
      else if ($idZona!==0 && $idEstablecimiento!==0)
        $sql = $dbConn->prepare("select p.idUsuario,count(*) as pedidos,u.nombre,u.apellidos,u.email,u.telefono from qo_pedidos p inner join qo_users u on (u.id=p.idUsuario) WHERE p.idZona=$idZona and p.idEstablecimiento=$isEstablecimiento group by p.idUsuario ORDER BY `pedidos`  DESC");
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
      exit();
    }else if (isset($_GET['clientesInactivos']))
    {
      $sql = $dbConn->prepare("select u.* from qo_users u inner join qo_pueblos p on (p.id=u.idPueblo) where u.id not in (select idUsuario from qo_pedidos) and u.rol=1 and p.idGrupo=:idGrupo");
      $sql->bindValue(':idGrupo', $_GET['idGrupo']);
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
      exit();
    }else if (isset($_GET['cuentasEstablecimiento']))
    {
      $sql = $dbConn->prepare("(select 'Completo' as proceso,if (isnull(p2.pedidos),0,p2.pedidos) as cantidad,if (isnull(ce.comision),0,ce.comision) as comision,if (isnull(sum(d.cantidad*d.precio)),0,sum(d.cantidad*d.precio)) as totalEstablecimiento, if (isnull(Sum(d.cantidad*d.precio)-(Sum(d.cantidad*d.precio)*ce.comision/100)),0,Sum(d.cantidad*d.precio)-(Sum(d.cantidad*d.precio)*ce.comision/100)) as totalPagar 
      from qo_establecimientos e 
      inner join qo_pedidos p on (e.id=p.idEstablecimiento) 
      inner join (select count(*) pedidos,idEstablecimiento from qo_pedidos where tipoVenta like 'Envío%' and anulado=0 and date(horaPedido)>=:desde and date(horaPedido)<=:hasta GROUP BY idEstablecimiento) p2 on (p2.idEstablecimiento=p.idEstablecimiento)
      inner join qo_configuracion_est ce on (ce.idEstablecimiento=e.id)
      inner join qo_pedidos_detalle d on (d.idPedido=p.id and d.tipo=0 and p.tipoVenta like 'Envío%')
      where date(p.horaPedido)>=:desde and date(p.horaPedido)<=:hasta and p.idEstablecimiento=:idEstablecimiento )
      UNION
      (select 'Recogida' as proceso,if (isnull(p2.pedidos),0,p2.pedidos) as cantidad,if (isnull(comisionRecogida),0,comisionRecogida) as comision,if (isnull(sum(d.cantidad*d.precio)),0,sum(d.cantidad*d.precio)) as totalEstablecimiento, if (isnull(Sum(d.cantidad*d.precio)-(Sum(d.cantidad*d.precio)*ce.comisionRecogida/100)),0,Sum(d.cantidad*d.precio)-(Sum(d.cantidad*d.precio)*ce.comisionRecogida/100)) as totalPagar 
      from qo_establecimientos e 
      inner join qo_pedidos p on (e.id=p.idEstablecimiento) 
       inner join (select count(*) pedidos,idEstablecimiento from qo_pedidos where tipoVenta like 'Recogida%' and anulado=0 and date(horaPedido)>=:desde and date(horaPedido)<=:hasta GROUP BY idEstablecimiento) p2 on (p2.idEstablecimiento=p.idEstablecimiento)
      inner join qo_configuracion_est ce on (ce.idEstablecimiento=e.id)
      inner join qo_pedidos_detalle d on (d.idPedido=p.id and d.tipo=0 and p.tipoVenta like 'Recogida%')
      where date(p.horaPedido)>=:desde and date(p.horaPedido)<=:hasta and p.idEstablecimiento=:idEstablecimiento)
      UNION
      (select 'AutoPedido' as proceso,if (isnull(p2.pedidos),0,p2.pedidos) as cantidad,if (isnull(comisionAutoPedido),0,comisionAutoPedido) as comision,if (isnull(sum(d.cantidad*d.precio)),0,sum(d.cantidad*d.precio)) as totalEstablecimiento, if (isnull(Sum(d.cantidad*d.precio)-(Sum(d.cantidad*d.precio)*ce.comisionAutoPedido/100)),0,Sum(d.cantidad*d.precio)-(Sum(d.cantidad*d.precio)*ce.comisionAutoPedido/100)) as totalPagar 
      from qo_establecimientos e 
      inner join qo_pedidos p on (e.id=p.idEstablecimiento) 
      inner join (select count(*) pedidos,idEstablecimiento from qo_pedidos where tipoVenta like 'Auto%' and anulado=0 and date(horaPedido)>=:desde and date(horaPedido)<=:hasta GROUP BY idEstablecimiento) p2 on (p2.idEstablecimiento=p.idEstablecimiento)
      inner join qo_configuracion_est ce on (ce.idEstablecimiento=e.id)
      inner join qo_pedidos_detalle d on (d.idPedido=p.id and d.tipo=0 and p.tipoVenta like 'Auto%')
      where date(p.horaPedido)>=:desde and date(p.horaPedido)<=:hasta and p.idEstablecimiento=:idEstablecimiento)
      UNION
      (select 'Reparto Propio' as proceso,if (isnull(p2.pedidos),0,p2.pedidos) as cantidad,if (isnull(comisionReparto),0,comisionReparto) as comision,if (isnull(sum(d.cantidad*d.precio)),0,sum(d.cantidad*d.precio)) as totalEstablecimiento, if (isnull(Sum(d.cantidad*d.precio)-(Sum(d.cantidad*d.precio)*ce.comisionReparto/100)),0,Sum(d.cantidad*d.precio)-(Sum(d.cantidad*d.precio)*ce.comisionReparto/100))  as totalPagar 
      from qo_establecimientos e 
      inner join qo_pedidos p on (e.id=p.idEstablecimiento) 
      inner join (select count(*) pedidos,idEstablecimiento from qo_pedidos where tipoVenta like 'Reparto%' and anulado=0 and date(horaPedido)>=:desde and date(horaPedido)<=:hasta GROUP BY idEstablecimiento) p2 on (p2.idEstablecimiento=p.idEstablecimiento)
      inner join qo_configuracion_est ce on (ce.idEstablecimiento=e.id)
      inner join qo_pedidos_detalle d on (d.idPedido=p.id and p.tipoVenta like 'Reparto%')
      where date(p.horaPedido)>=:desde and date(p.horaPedido)<=:hasta and p.idEstablecimiento=:idEstablecimiento)
      UNION (Select 'Cuota Fija' as proceso,1 as cantidad,0 as comision,cuotaFija as totalEstablecimiento,cuotaFija as totalPagar FROM qo_configuracion_est WHere idEstablecimiento=:idEstablecimiento)");
      $sql->bindValue(':idEstablecimiento', $_GET['idEstablecimiento']);
      $sql->bindValue(':desde', $_GET['desde']);
      $sql->bindValue(':hasta', $_GET['hasta']);
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
      exit();
    }else if (isset($_GET['cuentasAdministrador']))
    {
      $sql = $dbConn->prepare("(select 'Gastos de envío' as proceso,if (isnull(p2.pedidos),0,p2.pedidos) as cantidad,if (isnull(sum(d.precio)),0,sum(d.precio)) as total,if(isnull(g.idGrupo),0,g.idGrupo ) as idGrupo
      from qo_establecimientos e 
      inner join qo_pueblos_grupos g on (g.idGrupo=e.idGrupo)
      inner join qo_pedidos p on (e.id=p.idEstablecimiento) 
      inner join (select count(*) pedidos,idGrupo from qo_pedidos p inner join qo_establecimientos e on (e.id=p.idEstablecimiento) where tipoVenta like 'Envío%' and tipoPago='Tarjeta' and anulado=0 and date(horaPedido)>=:desde and date(horaPedido)<=:hasta GROUP BY idGrupo) p2 on (p2.idGrupo=e.idGrupo)
      inner join qo_pedidos_detalle d on (d.idPedido=p.id and d.tipo=1)
      where p.anulado=0 and p.tipoVenta like 'Envío%' and p.tipoPago='Tarjeta' and p.anulado=0 and date(p.horaPedido)>=:desde and date(p.horaPedido)<=:hasta and g.idGrupo=:idGrupo)
      UNION (
      SELECT 'Comisión Envíos' as proceso,if (isnull(p2.pedidos),0,p2.pedidos) as cantidad,if (isnull(d2.precio),0,d2.precio) as total,if(isnull(g.idGrupo),0,g.idGrupo ) as idGrupo
      from qo_establecimientos e 
      inner join qo_pueblos_grupos g on (g.idGrupo=e.idGrupo)
      inner join qo_pedidos p on (e.id=p.idEstablecimiento) 
      inner join (select count(*) pedidos,idGrupo from qo_pedidos p inner join qo_establecimientos e on (e.id=p.idEstablecimiento) where (tipoVenta like 'Envío%' or tipoVenta like 'Auto%') and tipoPago='Tarjeta' and anulado=0 and date(horaPedido)>=:desde and date(horaPedido)<=:hasta GROUP BY idGrupo) p2 on (p2.idGrupo=e.idGrupo)
      inner join (SELECT SUM(d.cantidad*d.precio)*c.comision/100 as precio,g.idGrupo from qo_pedidos p
      inner join qo_establecimientos e on (e.id=p.idEstablecimiento)
      inner join qo_pueblos_grupos g on (g.idGrupo=e.idGrupo)
      inner join qo_pedidos_detalle d on (d.idPedido=p.id and d.tipo=0 and (p.tipoVenta like 'Envío%' or p.tipoVenta like 'Auto%'))
      inner join qo_configuracion_est c on (c.idEstablecimiento=p.idEstablecimiento) Where p.anulado=0 and date(p.horaPedido)>=:desde and date(p.horaPedido)<=:hasta and p.tipoPago='Tarjeta' and (p.tipoVenta like 'Envío%' or p.tipoVenta like 'Auto%') Group by e.idGrupo) d2 on (d2.idGrupo=e.idGrupo)
      where p.tipoPago='Tarjeta' and p.anulado=0 and date(p.horaPedido)>=:desde and date(p.horaPedido)<=:hasta and g.idGrupo=:idGrupo)
      UNION (
      select 'Comisión Recogida Cliente' as proceso,if (isnull(p2.pedidos),0,p2.pedidos) as cantidad,if (isnull(d2.precio),0,d2.precio) as total,if(isnull(g.idGrupo),0,g.idGrupo ) as idGrupo
      from qo_establecimientos e 
      inner join qo_pueblos_grupos g on (g.idGrupo=e.idGrupo)
      inner join qo_pedidos p on (e.id=p.idEstablecimiento) 
      inner join (select count(*) pedidos,idGrupo from qo_pedidos p inner join qo_establecimientos e on (e.id=p.idEstablecimiento) where tipoPago='Tarjeta' and tipoVenta like 'Recogida%' and anulado=0 and date(horaPedido)>=:desde and date(horaPedido)<=:hasta GROUP BY idGrupo) p2 on (p2.idGrupo=e.idGrupo)
      inner join (SELECT SUM(d.cantidad*d.precio)*c.comisionrecogida/100 as precio,g.idGrupo from qo_pedidos p
      inner join qo_establecimientos e on (e.id=p.idEstablecimiento)
      inner join qo_pueblos_grupos g on (g.idGrupo=e.idGrupo)
      inner join qo_pedidos_detalle d on (d.idPedido=p.id and d.tipo=0 and p.tipoVenta like 'Recogida%')
      inner join qo_configuracion_est c on (c.idEstablecimiento=p.idEstablecimiento) Where p.tipoPago='Tarjeta' and p.anulado=0 and date(p.horaPedido)>=:desde and date(p.horaPedido)<=:hasta and p.tipoVenta like 'Recogida%' Group by e.idGrupo) d2 on (d2.idGrupo=e.idGrupo)
      where p.tipoPago='Tarjeta' and p.anulado=0 and date(p.horaPedido)>=:desde and date(p.horaPedido)<=:hasta and g.idGrupo=:idGrupo)
      UNION (
      select 'Comisión Reparto Propio' as proceso,if (isnull(p2.pedidos),0,p2.pedidos) as cantidad,if (isnull(d2.precio),0,d2.precio) as total,if(isnull(g.idGrupo),0,g.idGrupo ) as idGrupo
      from qo_establecimientos e 
      inner join qo_pueblos_grupos g on (g.idGrupo=e.idGrupo)
      inner join qo_pedidos p on (e.id=p.idEstablecimiento) 
      inner join (select count(*) pedidos,idGrupo from qo_pedidos p inner join qo_establecimientos e on (e.id=p.idEstablecimiento) where tipoPago='Tarjeta' and tipoVenta  = 'Reparto Propio' and anulado=0 and date(horaPedido)>=:desde and date(horaPedido)<=:hasta GROUP BY idGrupo) p2 on (p2.idGrupo=e.idGrupo)
      inner join (SELECT SUM(d.cantidad*d.precio)*c.comisionReparto/100 as precio,g.idGrupo from qo_pedidos p
      inner join qo_establecimientos e on (e.id=p.idEstablecimiento)
      inner join qo_pueblos_grupos g on (g.idGrupo=e.idGrupo)
      inner join qo_pedidos_detalle d on (d.idPedido=p.id and d.tipo=0 and p.tipoVenta = 'Reparto Propio')
      inner join qo_configuracion_est c on (c.idEstablecimiento=p.idEstablecimiento) Where p.tipoPago='Tarjeta' and p.anulado=0 and date(p.horaPedido)>=:desde and date(p.horaPedido)<=:hasta and  p.tipoVenta= 'Reparto Propio' Group by e.idGrupo) d2 on (d2.idGrupo=e.idGrupo)
      where p.tipoPago='Tarjeta' and p.anulado=0 and date(p.horaPedido)>=:desde and date(p.horaPedido)<=:hasta and g.idGrupo=:idGrupo)
      UNION (
      Select 'Cuota Fija' as proceso,if(isnull(e2.cantidad),0,e2.cantidad) as cantidad,if(isnull(cuotaFija),0,sum(cuotaFija)) as total,if(isnull(e.idGrupo),0,e.idGrupo) as idGrupo FROM qo_configuracion_est c 
      inner join qo_establecimientos e on (e.id=c.idEstablecimiento and e.estado=1) 
      inner join (SELECT count(*) cantidad,idGrupo FROM qo_establecimientos where estado=1 group by idGrupo) e2 on (e2.idGrupo=e.idGrupo)
      group by idGrupo having  idGrupo=:idGrupo)
      UNION (
      Select 'Cobrado en efectivo' as proceso,if(isnull(p2.pedidos),0,p2.pedidos) as cantidad,if(isnull(d.precio),0,sum(d.cantidad*d.precio*-1)) as total,if(isnull(e.idGrupo),0,e.idGrupo) as idGrupo
      FROM qo_pedidos p
      inner join qo_establecimientos e on (e.id=p.idEstablecimiento) 
      inner join qo_pueblos_grupos g on (g.idGrupo=e.idGrupo)
      inner join qo_pedidos_detalle d on (d.idPedido=p.id)
      inner join (select count(*) pedidos,idGrupo from qo_pedidos p inner join qo_establecimientos e on (e.id=p.idEstablecimiento) where tipoPago<>'Tarjeta' and anulado=0 and date(horaPedido)>=:desde and date(horaPedido)<=:hasta GROUP BY idGrupo) p2 on (p2.idGrupo=e.idGrupo)
      WHERE date(p.horaPedido)>=:desde and date(p.horaPedido)<=:hasta and p.tipoPago<>'Tarjeta' and p.anulado=0 and g.idGrupo=:idGrupo) 
      UNION (
      Select 'Gastos de tarjeta' as proceso,if(isnull(p2.pedidos),0,p2.pedidos) as cantidad,if(isnull(m.comision),0,sum(m.comision)*-1) as total,if(isnull(e.idGrupo),0,e.idGrupo) as idGrupo
      FROM qo_pedidos p 
      inner join qo_establecimientos e on (e.id=p.idEstablecimiento)
      inner join (select count(*) pedidos,idGrupo from qo_pedidos p inner join qo_establecimientos e on (e.id=p.idEstablecimiento) where tipoPago='Tarjeta' and anulado=0 and date(horaPedido)>=:desde and date(horaPedido)<=:hasta and p.anulado=0 GROUP BY idGrupo) p2 on (p2.idGrupo=e.idGrupo)
      inner join (Select (sum(cantidad*precio)*c.variableTarjeta/100)+c.fijoTarjeta as comision,e.idGrupo,d.idPedido
      from qo_pedidos_detalle d 
      inner join qo_pedidos p on (p.id=d.idPedido) 
      inner join qo_establecimientos e on (e.id=p.idEstablecimiento)
      inner join qo_configuracion c on (c.idGrupo=e.idGrupo) 
      Where p.tipoPago='Tarjeta' and date(p.horaPedido)>=:desde and date(p.horaPedido)<=:hasta and p.anulado=0
      group by idPedido) m on (e.idGrupo=m.idGrupo and p.id=m.idPedido)
      WHERE e.idGrupo=:idGrupo)");
      $sql->bindValue(':idGrupo', $_GET['idGrupo']);
      $sql->bindValue(':desde', $_GET['desde']);
      $sql->bindValue(':hasta', $_GET['hasta']);
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
      exit();
    }
}


//En caso de que ninguna de las opciones anteriores se haya ejecutado
header("HTTP/1.1 400 Bad Request");

?>