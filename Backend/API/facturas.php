<?php
include "config.php";
include "utils.php";


$dbConn =  connect($db);
/*
  listar todos los posts o solo uno
 */
if ($_SERVER['REQUEST_METHOD'] == 'GET')
{
  if (isset($_GET['todo']))
  {
      $sql = $dbConn->prepare("select sum(f.total) as total,e.idPueblo,p.nombre as poblacion,year(f.desde) as anyo from qo_facturas f inner join qo_establecimientos e on (e.id=f.idEstablecimiento) inner join qo_pueblos p on (p.id=e.idPueblo) group by e.idPueblo,month(f.desde)");
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
      exit();
  }else if (isset($_GET['cuentasAdministradores']))
  {
      $sql = $dbConn->prepare("SELECT if (isnull(a.totalVentas),0,a.totalVentas) as totalVentas,y.totalPedidos,if (isnull(x.totalVentasSinGastos),0,x.totalVentasSinGastos) as totalVentasSinGastos, if (isnull(b.pagosAEstablecimientos),0,b.pagosAEstablecimientos) as pagosAEstablecimientos,if (isnull(c.totalVentasTarjeta),0,c.totalVentasTarjeta) as totalVentasTarjeta ,if (isnull(c.fijoTarjeta),0,c.fijoTarjeta) as fijoTarjeta,if (isnull(c.variableTarjeta),0,c.variableTarjeta) as variableTarjeta,if(isnull(d.totalVentasDatafono),0,d.totalVentasDatafono) as totalVentasDatafono,if (isnull(d.comisionDatafono),0,d.comisionDatafono) as comisionDatafono,if (isnull((b.pagosAEstablecimientos*co.comision/100)),0,(b.pagosAEstablecimientos*co.comision/100)) as comision,a.idPueblo
      FROM (SELECT SUM(d.cantidad*d.precio) as totalVentas, e.idPueblo from qo_pedidos p inner join qo_pedidos_detalle d on (d.idPedido=p.id) inner join qo_establecimientos e on (e.id=p.idEstablecimiento) Where date(horaPedido) between :desde and :hasta and p.anulado=0 GROUP by e.idPueblo) a 
      left join (SELECT SUM(d.cantidad*d.precio) as totalVentasSinGastos, e.idPueblo from qo_pedidos p inner join qo_pedidos_detalle d on (d.idPedido=p.id) inner join qo_establecimientos e on (e.id=p.idEstablecimiento) Where date(horaPedido) between :desde and :hasta and p.anulado=0 and d.tipo<>1 GROUP by e.idPueblo) x on (x.idPueblo=a.idPueblo) 
      left join (SELECT count(*) totalPedidos, e.idPueblo from qo_pedidos p inner join qo_establecimientos e on (e.id=p.idEstablecimiento) Where date(horaPedido) between :desde and :hasta and p.anulado=0 GROUP by e.idPueblo) y on (y.idPueblo=a.idPueblo) 
      left join (SELECT  sum(total) as pagosAEstablecimientos,e.idPueblo FROM `qo_facturas` f inner join qo_establecimientos e on (e.id=f.idEstablecimiento) WHERE f.desde>=:desde and f.hasta<=:hasta GROUP by e.idPueblo) b on (a.idPueblo=b.idPueblo)
      left join (SELECT SUM(d.cantidad*d.precio) as totalVentasTarjeta, a.pedidos*c.fijoTarjeta as fijoTarjeta,(SUM(d.cantidad*d.precio)*c.variableTarjeta/100) as variableTarjeta, e.idPueblo from qo_pedidos p inner join qo_pedidos_detalle d on (d.idPedido=p.id) inner join qo_establecimientos e on (e.id=p.idEstablecimiento) inner join qo_configuracion c on (c.idPueblo=e.idPueblo) inner join (SELECT count(*) as pedidos,e.idPueblo from qo_pedidos p inner join qo_establecimientos e on (e.id=p.idEstablecimiento) Where date(horaPedido) between :desde and :hasta and tipoPago='Tarjeta' and anulado=0 group by e.idPueblo) a on (a.idPueblo=e.idPueblo) Where date(horaPedido) between :desde and :hasta and p.tipoPago='Tarjeta' and p.anulado=0 GROUP by e.idPueblo) c on (c.idPueblo=a.idPueblo)
      left join (SELECT SUM(d.cantidad*d.precio) as totalVentasDatafono,(SUM(d.cantidad*d.precio)*c.variableDatafono/100) as comisionDatafono, e.idPueblo from qo_pedidos p inner join qo_pedidos_detalle d on (d.idPedido=p.id) inner join qo_establecimientos e on (e.id=p.idEstablecimiento) inner join qo_configuracion c on (c.idPueblo=e.idPueblo) Where date(horaPedido) between :desde and :hasta and p.tipoPago='Datafono' and p.anulado=0 GROUP by e.idPueblo) d on (d.idPueblo=a.idPueblo)
      inner join qo_configuracion co on (co.idPueblo=a.idPueblo)");
      $sql->bindValue(':desde', $_GET['desde']);
      $sql->bindValue(':hasta', $_GET['hasta']);
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
      exit();
  }else if (isset($_GET['idFiscal']))
  {
      $sql = $dbConn->prepare("SELECT nombreTicket as razonSocial,direccionTicket as direccion, cpTicket as cp,poblacionTicket as poblacion, provinciaTicket as provincia, telefonoTicket as telefono, CIFTicket as cif,idPueblo, iban FROM `qo_configuracion` WHERE idPueblo=:idPueblo");
      $sql->bindValue(':idPueblo', $_GET['idFiscal']);
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
      exit();
  }else if (isset($_GET['idPueblo']))
  {
      $sql = $dbConn->prepare("SELECT * from qo_facturas_administradores f Where f.idPueblo=:idPueblo order by numero");
      $sql->bindValue(':idPueblo', $_GET['idPueblo']);
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
      exit();
  }else {
      $sql = $dbConn->prepare("SELECT * from qo_facturas where idEstablecimiento=:idEstbalecimiento");
      $sql->bindValue(':idEstablecimiento', $_GET['id']);
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

    if (isset($_GET['administrador']))
    {
      $statement = $dbConn->prepare("DELETE FROM qo_facturas_administradores where numero=:numero");
      $statement->bindValue(':numero', $input['numero']);
      $statement->execute();

      $sql = "INSERT INTO `qo_facturas_administradores` (`ruta`, `nombre`, `numero`, `desde`, `hasta`, `idPueblo`, `nombreAdministrador`, `total`) VALUES 
      (:ruta,:nombre,:numero,:desde,:hasta,:idPueblo,:nombreAdministrador,:total)";
      $statement = $dbConn->prepare($sql);
      $statement->bindValue(':ruta', $input['ruta']);
      $statement->bindValue(':nombre', $input['nombre']);
      $statement->bindValue(':numero', $input['numero']);
      $statement->bindValue(':desde', $input['desde']);
      $statement->bindValue(':hasta', $input['hasta']);
      $statement->bindValue(':idPueblo', $input['idPueblo']);
      $statement->bindValue(':nombreAdministrador', $input['nombreAdministrador']);
      $statement->bindValue(':total', $input['total']);
      $statement->execute();
    }else{
      $statement = $dbConn->prepare("DELETE FROM qo_facturas where numero=:numero");
      $statement->bindValue(':numero', $input['numero']);
      $statement->execute();

      $sql = "INSERT INTO `qo_facturas`(`ruta`, `nombre`, `numero`, `desde`, `hasta`, `idEstablecimiento`, `nombreEstablecimiento`, `total`) VALUES 
      (:ruta,:nombre,:numero,:desde,:hasta,:idEstablecimiento,:nombreEstablecimiento,:total)";
      $statement = $dbConn->prepare($sql);
      $statement->bindValue(':ruta', $input['ruta']);
      $statement->bindValue(':nombre', $input['nombre']);
      $statement->bindValue(':numero', $input['numero']);
      $statement->bindValue(':desde', $input['desde']);
      $statement->bindValue(':hasta', $input['hasta']);
      $statement->bindValue(':idEstablecimiento', $input['idEstablecimiento']);
      $statement->bindValue(':nombreEstablecimiento', $input['nombreEstablecimiento']);
      $statement->bindValue(':total', $input['total']);
      $statement->execute();

      $sql = "UPDATE qo_pedidos set facturado=1,numFactura=:numero WHERE idEstablecimiento=:idEstablecimiento and estado>=4 and anulado=0 and date(horaPedido) between :desde and :hasta";
      $statement = $dbConn->prepare($sql);
      $statement->bindValue(':numero', $input['numero']);
      $statement->bindValue(':desde', $input['desde']);
      $statement->bindValue(':hasta', $input['hasta']);
      $statement->bindValue(':idEstablecimiento', $input['idEstablecimiento']);
      $statement->execute();
    }
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
  if (isset($_GET['administrador']))
  {
    $statement = $dbConn->prepare("DELETE FROM qo_facturas where id=:id");
  }else{
    $statement = $dbConn->prepare("DELETE FROM qo_facturas_administradores where id=:id");
  }
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
    if (isset($_GET['administrador']))
    {
      $sql = "UPDATE `qo_facturas` SET provincia=:provincia,codPostal=:codPostal,idGrupo=:idGrupo, `nombre`=:nombre,activo=:activo,idUsuario=:idUsuario,longitud=:longitud,latitud=:latitud WHERE id=$id";

      $statement = $dbConn->prepare($sql);
      $statement->bindValue(':nombre', $input['nombre']);
      $statement->bindValue(':activo', $input['activo']);
      $statement->bindValue(':codPostal', $input['codPostal']);
      $statement->bindValue(':provincia', $input['Provincia']);
      $statement->bindValue(':idUsuario', $input['idUsuario']);
      $statement->bindValue(':latitud', $input['latitud']);
      $statement->bindValue(':longitud', $input['longitud']);
      $statement->bindValue(':idGrupo', $input['idGrupo']);
      $statement->execute();

      $sql = "UPDATE `qo_users` SET idPueblo=:idPueblo WHERE id=:idUsuario";
        $statement = $dbConn->prepare($sql);
        $statement->bindValue(':idUsuario', $input['idUsuario']);
        $statement->bindValue(':idPueblo', $id);
        $statement->execute();
    }else{
      $sql = "UPDATE `qo_facturas_administradores` SET provincia=:provincia,codPostal=:codPostal,idGrupo=:idGrupo, `nombre`=:nombre,activo=:activo,idUsuario=:idUsuario,longitud=:longitud,latitud=:latitud WHERE id=$id";

      $statement = $dbConn->prepare($sql);
      $statement->bindValue(':nombre', $input['nombre']);
      $statement->bindValue(':activo', $input['activo']);
      $statement->bindValue(':codPostal', $input['codPostal']);
      $statement->bindValue(':provincia', $input['Provincia']);
      $statement->bindValue(':idUsuario', $input['idUsuario']);
      $statement->bindValue(':latitud', $input['latitud']);
      $statement->bindValue(':longitud', $input['longitud']);
      $statement->bindValue(':idGrupo', $input['idGrupo']);
      $statement->execute();

      $sql = "UPDATE `qo_users` SET idPueblo=:idPueblo WHERE id=:idUsuario";
      $statement = $dbConn->prepare($sql);
      $statement->bindValue(':idUsuario', $input['idUsuario']);
      $statement->bindValue(':idPueblo', $id);
      $statement->execute();
    }

    header("HTTP/1.1 200 OK");
    exit();
}


//En caso de que ninguna de las opciones anteriores se haya ejecutado
header("HTTP/1.1 400 Bad Request");

?>