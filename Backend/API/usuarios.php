<?php
include "config.php";
include "utils.php";


$dbConn =  connect($db);
/*
  listar todos los posts o solo uno
 */
if ($_SERVER['REQUEST_METHOD'] == 'GET')
{
  if (isset($_GET['verificaPin']))
  {
    //Mostrar un post
    $sql = $dbConn->prepare("SELECT count(*) as resultado FROM `qo_users` WHERE email=:email and pin=:pin and estado=1");
    $sql->bindValue(':email', $_GET['email']);
    $sql->bindValue(':pin', $_GET['pin']);
    $sql->execute();
    header("HTTP/1.1 200 OK");
    echo json_encode(  $sql->fetch(PDO::FETCH_ASSOC)  );
    exit();
  }else if (isset($_GET['usuarioVerificado']))
  {
    $sql = $dbConn->prepare("SELECT count(*) as resultado FROM `qo_users` WHERE email=:email");
    $sql->bindValue(':email', $_GET['usuarioVerificado']);
    $sql->execute();
      $result = $sql->fetch(PDO::FETCH_OBJ);
    $res= $result->resultado;
    if ($res>=1){
    //Mostrar un post
      $sql = $dbConn->prepare("SELECT count(*) as resultado FROM `qo_users` WHERE email=:email and verificado=1 and estado=1");
      $sql->bindValue(':email', $_GET['usuarioVerificado']);
      $sql->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetch(PDO::FETCH_ASSOC)  );
    }else{
      $sql = $dbConn->prepare("SELECT 1 as resultado FROM `qo_users` limit 1");
      $sql->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetch(PDO::FETCH_ASSOC)  );
    }
    exit();
  }else if (isset($_GET['social']))
  {
    //Mostrar un post
    $sql = $dbConn->prepare("SELECT codigo,saldo,`version`,demo,idZona,`id` as idUsuario,idSocial,social,`nombre`,dni,`apellidos`,`cod_postal`,`poblacion`,`provincia`,`direccion`,`fechaNacimiento`,plataforma,`fechaAlta`,`telefono`,`email`,`foto`,`rol`,`estado`,token,username,idPueblo,kiosko FROM `qo_users` where email=:email and idSocial=:id and estado=1");
    $sql->bindValue(':email', $_GET['email']);
    $sql->bindValue(':id', $_GET['id']);
    $sql->execute();
    header("HTTP/1.1 200 OK");
    echo json_encode(  $sql->fetch(PDO::FETCH_ASSOC)  );
    exit();
  }else if (isset($_GET['email']) && isset($_GET['password']))
    {
      //Mostrar un post
      $sql = $dbConn->prepare("SELECT password,codigo,saldo,`version`,versionFW,demo,idZona,`id` as idUsuario,idSocial,social,`nombre`,dni,`apellidos`,`cod_postal`,`poblacion`,`provincia`,`direccion`,`fechaNacimiento`,plataforma,`fechaAlta`,`telefono`,`email`,`foto`,`rol`,`estado`,token,username,idPueblo,kiosko FROM `qo_users` where email=:email and (password=:password or password='') and verificado=1 and estado=1");
      $sql->bindValue(':email', $_GET['email']);
      $sql->bindValue(':password', str_replace(' ','+',$_GET['password']));
      $sql->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetch(PDO::FETCH_ASSOC)  );
      exit();
	  }else if (isset($_GET['email']))
    {
      //Mostrar un post
      $sql = $dbConn->prepare("SELECT * FROM qo_users where email=:email");
      $sql->bindValue(':email', $_GET['email']);
      $sql->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetchAll(PDO::FETCH_ASSOC)  );
      exit();
	  }else if (isset($_GET['idUsuarioEstablecimiento']))
    {
      //Mostrar un post
      $sql = $dbConn->prepare("SELECT e.visibleFuera,e.tieneMenuDiario,e.visibleFuera,e.tipoImpresora,e.idPueblo,e.idGrupo,e.idZona,e.esComercio,e.idCategoria,e.puedeReservar,e.llamadaCamarero,if(isnull(e.usuarioBarra),'',e.usuarioBarra) as usuarioBarra,if(isnull(e.usuarioCocina),'',e.usuarioCocina) as usuarioCocina,e.ipImpresora,e.nombreImpresoraCocina,e.nombreImpresoraBarra,e.valoraciones,e.puntos,0 as distancia, e.id as idEstablecimiento, e.nombre,e.direccion,e.poblacion,e.telefono,e.email,e.provincia,e.codPostal,e.latitud,e.longitud,e.tipo as idTipo, 3 as tipo,e.imagen,e.estado, if(isnull(c.contador),0,c.contador) as numeroCategorias, if(isnull(p.contador),0,p.contador) as numeroProductos, if(isnull(z.contador),0,z.contador) as zonas, if(isnull(v.contador),0,v.contador) as ventas,e.local,e.envio,e.recogida,e.logo,e.textoMulti,e.llevaAMesa,e.recogeEnBarra FROM qo_establecimientos e inner join qo_users_est u on (e.id=u.idEstablecimiento) left join (select count(*) as contador,idEstablecimiento from qo_productos_cat group by idEstablecimiento) c on (c.idEstablecimiento=e.id)  left join (select count(*) as contador,idEstablecimiento from qo_establecimientos_zonas group by idEstablecimiento) z on (z.idEstablecimiento=e.id) left join (select count(*) as contador,idEstablecimiento from qo_productos_est es inner join qo_productos_cat ca on (es.idCategoria=ca.id) group by idEstablecimiento) p on (p.idEstablecimiento=e.id) left join (select sum(cantidad*precio) as contador,idEstablecimiento from qo_pedidos_detalle det inner join qo_pedidos pe on (det.idPedido=pe.id) group by pe.idEstablecimiento) v on (v.idEstablecimiento=e.id) WHERE  u.activo=1 and u.idUser=:idUsuario");
      $sql->bindValue(':idUsuario', $_GET['idUsuarioEstablecimiento']);
      $sql->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetchAll(PDO::FETCH_ASSOC)  );
      exit();
	  }else if (isset($_GET['idUsuarioPedidoPendiente']))
    {
      //Mostrar un post
      $sql = $dbConn->prepare("SELECT DISTINCT p.tipoPago,p.idCuenta,p.mesa,p.idZonaEstablecimiento,p.zonaEstablecimiento,p.tipoVenta,p.tipo,p.transaccion,p.pagado,if(isnull(re.pin),'',re.pin) as fotoRepartidor, p.idRepartidor,z.color as colorZona, if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',p.horaPedido,p.horaEntrega) as horaEntrega,z.nombre as zona,repartidor,concat(u.nombre,' ',u.apellidos) as nombreUsuario,if (isnull(p.direccion) or p.direccion='',u.direccion,p.direccion) as direccionUsuario,if (isnull(p.idZona) or p.idZona=0,u.idZona,p.idZona) as idZona,u.email as emailUsuario,u.telefono as telefonoUsuario,p.nuevoPedido,cat.tipo as tipoProducto,esta.nombre as nombreEstablecimiento,p.id as idPedido,p.codigo as codigoPedido,p.idUsuario,p.idEstablecimiento,d.id as idDetalle,p.horaPedido,d.idProducto,concat('(',SUBSTRING(cat.nombre,1,3),') ',pr.nombre) as nombreProducto,pr.descripcion as descripcionProducto,pr.imagen as imagenProducto,p.comentario,p.estado as idEstadoPedido,est.nombre as estadoPedido, d.cantidad,d.precio,(d.cantidad*d.precio) as importe,esta.textoMulti,esta.llevaAMesa,esta.recogeEnBarra FROM `qo_pedidos` p inner join qo_zonas z on (z.id=p.idZona) inner join qo_pedidos_detalle d on (p.id=d.idPedido) inner join qo_establecimientos esta on (esta.id=p.idEstablecimiento) inner join qo_productos_est pr on (pr.id=d.idProducto) inner join qo_productos_cat cat on (cat.id=pr.idCategoria) inner join qo_estados est on (p.estado=est.id) left join qo_repartidores re on (re.id=p.idRepartidor)  inner join qo_users u on (p.idUsuario=u.id) Where p.idUsuario=:idUsuario and p.valorado=0 and p.repartidor=1 and p.anulado=0 order by p.id,d.id,d.estado");
      $sql->bindValue(':idUsuario', $_GET['idUsuarioPedidoPendiente']);
      $sql->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetchAll(PDO::FETCH_ASSOC)  );
      exit();
	  }else if (isset($_GET['conPin']))
    {
      //Mostrar un post
      $sql = $dbConn->prepare("SELECT * FROM qo_users where pin<>'' and verificado=0 and estado=1");
      $sql->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetchAll(PDO::FETCH_ASSOC)  );
      exit();
	  }else if (isset($_GET['contador']))
    {
      //Mostrar un post
      $sql = $dbConn->prepare("SELECT count(*) as resultado FROM `qo_users` u inner join qo_pueblos p on (p.id=u.idPueblo)  WHERE rol=1 and estado=1 and p.idGrupo=:idGrupo");
      $sql->bindValue('idGrupo', $_GET['idGrupo']);
      $sql->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetch(PDO::FETCH_ASSOC)  );
      exit();
	  }else if (isset($_GET['idUsuarioZona']))
    {
      //Mostrar un post
      $sql = $dbConn->prepare("SELECT z.* FROM `qo_users` u inner join qo_zonas z on (u.idZona=z.id) WHERE u.id=:id");
      $sql->bindValue('id', $_GET['idUsuarioZona']);
      $sql->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetch(PDO::FETCH_ASSOC)  );
      exit();
	  }else if (isset($_GET['tokenAdmin']))
    {
      //Mostrar un post
      $sql = $dbConn->prepare("SELECT token FROM `qo_users` u inner join qo_pueblos p on (p.id=u.idPueblo) WHERE rol=3 and p.idGrupo=:id");
      $sql->bindValue(':id', $_GET['tokenAdmin']);
      $sql->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetchAll(PDO::FETCH_ASSOC)  );
      exit();
	  }else if (isset($_GET['tokenMultiAdmin']))
    {
      //Mostrar un post
      $sql = $dbConn->prepare("SELECT distinct u.token FROM `qo_users` u inner join qo_administradores_pueblos a on (a.idUser=u.id) WHERE a.idPueblo=:id and u.token<>'' and u.estado=1");
      $sql->bindValue(':id', $_GET['tokenMultiAdmin']);
      $sql->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetchAll(PDO::FETCH_ASSOC)  );
      exit();
	  }else if (isset($_GET['tokenRepartidores']))
    {
      //Mostrar un post
      $sql = $dbConn->prepare("SELECT token FROM `qo_users` u inner join qo_pueblos p on (p.id=u.idPueblo) WHERE rol=4 and p.idGrupo=:idGrupo");
      $sql->bindValue('idGrupo', $_GET['idGrupo']);
      $sql->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetchAll(PDO::FETCH_ASSOC)  );
      exit();
	  }else if (isset($_GET['tokenRepartidoresEst']))
    {
      //Mostrar un post
      $sql = $dbConn->prepare("select u.token from qo_repartidores r inner join qo_repartidores_establecimientos re on (re.idRepartidor=r.id) inner join qo_users u on (r.idUsuario=u.id) Where re.idEstablecimiento=:id and u.token<>''");
      $sql->bindValue(':id', $_GET['tokenRepartidoresEst']);
      $sql->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetchAll(PDO::FETCH_ASSOC)  );
      exit();
	  }else if (isset($_GET['idUsuarioToken']))
    {
      //Mostrar un post
      $sql = $dbConn->prepare("SELECT token FROM `qo_users` WHERE id=:id");
      $sql->bindValue(':id', $_GET['idUsuarioToken']);
      $sql->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetch(PDO::FETCH_ASSOC)  );
      exit();
	  }else if (isset($_GET['idUsuario']))
    {
      //Mostrar un post
      $sql = $dbConn->prepare("SELECT demo,idZona,`id` as idUsuario,`nombre`,dni,`apellidos`,`cod_postal`,`poblacion`,`provincia`,`direccion`,`fechaNacimiento`,plataforma,`fechaAlta`,`telefono`,`email`,`foto`,`rol`,`estado`,token,username,idPueblo FROM `qo_users` WHERE id=:id");
      $sql->bindValue(':id', $_GET['idUsuario']);
      $sql->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetch(PDO::FETCH_ASSOC)  );
      exit();
	  }else if (isset($_GET['emailUsuario']))
    {
      //Mostrar un post
      $sql = $dbConn->prepare("SELECT u.`id` as idUsuario,u.`nombre`,u.email,u.`apellidos`,u.`rol`,u.`estado`,u.`password`,u.idPueblo FROM `qo_users` u inner join qo_pueblos p on (p.id=u.idPueblo) WHERE email=:id and p.idGrupo=:idGrupo");
      $sql->bindValue(':id', $_GET['emailUsuario']);
      $sql->bindValue(':idGrupo', $_GET['idGrupo']);
      $sql->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetch(PDO::FETCH_ASSOC)  );
      exit();
	  }else if (isset($_GET['filtro']))
    {
      //Mostrar un post
      $filtro=$_GET['filtro'];
      $sql = $dbConn->prepare($filtro);
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
      exit();
	  }else if (isset($_GET['administradores']))
    {
      $sql = $dbConn->prepare('SELECT demo,idZona,`id` as idUsuario,`nombre`,dni,`apellidos`,`cod_postal`,`poblacion`,`provincia`,`direccion`,`fechaNacimiento`,plataforma,`fechaAlta`,`telefono`,`email`,`foto`,`rol`,`estado`,token,username,idPueblo FROM `qo_users` WHERE rol=3 and estado=1 and id not in (SELECT idUsuario FROM qo_pueblos) order by nombre');
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
      exit();
	  }else if (isset($_GET['clientes']))
    {
      // Obtener todos los clientes (rol=1)
      $sql = $dbConn->prepare('SELECT DISTINCT `id` as idUsuario, `nombre`, `apellidos`, `cod_postal`, `poblacion`, `provincia`, `direccion`, `fechaNacimiento`, `fechaAlta`, `telefono`, `email`, `foto`, `estado`, `saldo`, `kiosko` FROM `qo_users` WHERE rol=1 ORDER BY nombre, apellidos');
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode($sql->fetchAll());
      exit();
	  }else if (isset($_GET['historicoPedidosCliente']))
    {
      // Obtener histórico de pedidos de un cliente
      $sql = $dbConn->prepare('SELECT p.id as idPedido, p.codigo as codigoPedido, p.idEstablecimiento, esta.nombre as nombreEstablecimiento, p.horaPedido, p.estado as idEstadoPedido, est.nombre as estadoPedido, p.tipoPago, p.pagado, (SELECT SUM(d.cantidad*d.precio) FROM qo_pedidos_detalle d WHERE d.idPedido=p.id) as precioTotalPedido, p.direccion as direccionUsuario, p.comentario, p.tipoVenta FROM qo_pedidos p INNER JOIN qo_estados est ON p.estado = est.id INNER JOIN qo_establecimientos esta ON p.idEstablecimiento = esta.id WHERE p.idUsuario = :idCliente AND p.anulado = 0 ORDER BY p.horaPedido DESC LIMIT 50');
      $sql->bindValue(':idCliente', $_GET['idCliente']);
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode($sql->fetchAll());
      exit();
	  }else if (isset($_GET['puntosCliente']))
    {
      // Obtener puntos de un cliente
      $sql = $dbConn->prepare('SELECT pu.id, pu.idUsuario, pu.idEstablecimiento, pu.puntos, e.nombre as nombreEstablecimiento FROM qo_puntos_usuario pu INNER JOIN qo_establecimientos e ON pu.idEstablecimiento = e.id WHERE pu.idUsuario = :idCliente ORDER BY e.nombre');
      $sql->bindValue(':idCliente', $_GET['idCliente']);
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode($sql->fetchAll());
      exit();
	  }
    else {
      //Mostrar lista de post
      $sql = $dbConn->prepare("SELECT * FROM qo_users");
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
    $sql = "INSERT INTO `qo_users`(`nombre`, `apellidos`, `email`, `password`,pin, `fechaAlta`,idSocial,social,idPueblo) VALUES
          (:nombre, :apellidos, :email, :password,:pin,now(),:idSocial,:social,:idPueblo)";
    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':nombre', $input['nombre']);
    $statement->bindValue(':apellidos', $input['apellidos']);
    $statement->bindValue(':password', $input['password']);
    $statement->bindValue(':email', $input['email']);
    $statement->bindValue(':pin', $input['PIN']);
    $statement->bindValue(':idSocial', $input['idSocial']);
    $statement->bindValue(':social', $input['social']);
    $statement->bindValue(':idPueblo', $input['idPueblo']);
    bindAllValues($statement, $input);
    $statement->execute();
    $postId = $dbConn->lastInsertId();
    if($postId)
    {
      $input['id'] = $postId;
      $sql = $dbConn->prepare("update qo_users set codigo=concat(upper(substring(nombre,1,1)),upper(substring(apellidos,1,1)),lpad(id,6,'0')) WHere id=$postId");
      $sql->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode($input);
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
    if (isset($_GET['actualizarCliente']))
    {
      // Actualizar datos del cliente
      $sql = $dbConn->prepare("UPDATE `qo_users` SET
        `nombre`=:nombre,
        `apellidos`=:apellidos,
        `email`=:email,
        `telefono`=:telefono,
        `direccion`=:direccion,
        `poblacion`=:poblacion,
        `provincia`=:provincia,
        `cod_postal`=:cod_postal,
        `estado`=:estado,
        `saldo`=:saldo,
        `kiosko`=:kiosko
        WHERE id=:idUsuario");
      $sql->bindValue(':idUsuario', $input['idUsuario']);
      $sql->bindValue(':nombre', $input['nombre']);
      $sql->bindValue(':apellidos', $input['apellidos']);
      $sql->bindValue(':email', $input['email']);
      $sql->bindValue(':telefono', $input['telefono']);
      $sql->bindValue(':direccion', $input['direccion']);
      $sql->bindValue(':poblacion', $input['poblacion']);
      $sql->bindValue(':provincia', $input['provincia']);
      $sql->bindValue(':cod_postal', $input['cod_postal']);
      $sql->bindValue(':estado', $input['estado']);
      $sql->bindValue(':saldo', $input['saldo']);
      $sql->bindValue(':kiosko', $input['kiosko']);
      $result = $sql->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode($result);
      exit();
	  }else if (isset($_GET['actualizarPuntos']))
    {
      // Actualizar puntos del cliente en un establecimiento
      $sql = $dbConn->prepare("UPDATE `qo_puntos_usuario` SET `puntos`=:puntos WHERE idUsuario=:idUsuario AND idEstablecimiento=:idEstablecimiento");
      $sql->bindValue(':idUsuario', $input['idUsuario']);
      $sql->bindValue(':idEstablecimiento', $input['idEstablecimiento']);
      $sql->bindValue(':puntos', $input['puntos']);
      $result = $sql->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode($result);
      exit();
	  }else if (isset($_GET['borrarToken']))
    {
      //Mostrar un post
      $sql = $dbConn->prepare("UPDATE `qo_users` SET `token`='' WHERE email=:email");
      $sql->bindValue(':email',$input['email']);
      $sql->execute();
      
	  }else if (isset($_GET['actualizaImagen']))
    {
      //Mostrar un post
      $sql = $dbConn->prepare("UPDATE `qo_users` SET `foto`=:foto WHERE idSocial=:idSocial");
      $sql->bindValue(':idSocial',$input['idSocial']);
      $sql->bindValue(':foto',$input['foto']);
      $sql->execute();
      
	  }else if (isset($_GET['cambiaPass']))
    {
      $sql = $dbConn->prepare("SELECT * FROM qo_users WHERE email=:email");
      $sql->bindValue(':email', $input['email']);
      $sql->execute();
      $result=$sql->fetch(PDO::FETCH_ASSOC);

      if (isset($result['email'])){
        if($result['social']=='facebook' || $result['social']=='google')
          echo 'ERROR: su cuenta ha sido creada con la red social '. strtoupper($result['social']). '. Por favor, inicie sesión con dicha Red Social. Gracias';
        else{
          $sql = $dbConn->prepare("UPDATE `qo_users` SET `password`=:pass,pin=:pin,verificado=0 WHERE email=:email");
          $sql->bindValue(':email',$input['email']);
          $sql->bindValue(':pin',$input['pin']);
          $sql->bindValue(':pass',$input['password']);
          $sql->execute();
        }
      }else
        echo 'ERROR: El email no existe, por favor inténtenlo de nuevo. Disculpe las molestias';

      //Mostrar un post
      /**/
      
	  }else if (isset($_GET['cambiaPassEncrypt']))
    {
      //Mostrar un post
      $sql = $dbConn->prepare("UPDATE `qo_users` SET `password`=:pass WHERE email=:email");
      $sql->bindValue(':email',$input['email']);
      $sql->bindValue(':pass',$input['password']);
      $sql->execute();
      
	  }else if (isset($_GET['actualizaPin']))
    {
      //Mostrar un post
      $sql = $dbConn->prepare("UPDATE `qo_users` SET `pin`=:pin WHERE email=:email");
      $sql->bindValue(':email',$input['email']);
      $sql->bindValue(':pin',$input['pin']);
      $sql->execute();
      
	  }else if (isset($_GET['verificado']))
    {
      //Mostrar un post
      $sql = $dbConn->prepare("UPDATE `qo_users` SET verificado=1,pin='' WHERE email=:email");
      $sql->bindValue(':email',$input['email']);
      $sql->execute();
      
	  }else if (isset($_GET['idUsuarioRol']))
    {
      //Mostrar un post
      $sql = $dbConn->prepare("UPDATE `qo_users` SET `estado`=:estado,rol=:rol,`password`=:pass WHERE id=:id");
      $sql->bindValue(':estado',$input['estado']);
      $sql->bindValue(':rol',$input['rol']);
      $sql->bindValue(':pass',$input['password']);
      $sql->bindValue(':id',$input['idUsuario']);
      $sql->execute();

      $statement = $dbConn->prepare("DELETE FROM qo_users_est where idUser=:id");
      $statement->bindValue(':id', $input['idUsuario']);
      $statement->execute();

      if ($input['rol']==2){
        $sql = $dbConn->prepare("INSERT INTO `qo_users_est` (idUser,idEstablecimiento) VALUES (:id,:idEstablecimiento)");
        $sql->bindValue(':idEstablecimiento',$_GET['idUsuarioRol']);
        $sql->bindValue(':id',$input['idUsuario']);
        $sql->execute();
      }else if ($input['rol']==4){
        $sql = $dbConn->prepare("UPDATE qo_repartidores SET idUsuario=:id Where id=:idRepartidor");
        $sql->bindValue(':idRepartidor',$_GET['idRepartidorRol']);
        $sql->bindValue(':id',$input['idUsuario']);
        $sql->execute();
      }
      
	  }else{
      $userId = $input['idUsuario'];
      $sql = "
            UPDATE qo_users
            SET saldo=:saldo,idPueblo=:idPueblo,direccion=:direccion,foto=:foto,idZona=:idZona,poblacion=:poblacion,provincia=:provincia,cod_postal=:codPostal,token=:token,plataforma=:plataforma,version=:version
            WHERE id='$userId'";
      $statement = $dbConn->prepare($sql);
      $statement->bindValue(':direccion',$input['direccion']);
      $statement->bindValue(':plataforma',$input['plataforma']);
      $statement->bindValue(':token',$input['token']);
      $statement->bindValue(':foto',$input['foto']);
      $statement->bindValue(':idZona',$input['idZona']);
      $statement->bindValue(':poblacion',$input['poblacion']);
      $statement->bindValue(':provincia',$input['provincia']);
      $statement->bindValue(':codPostal',$input['cod_postal']);
      $statement->bindValue(':version',$input['version']);
      $statement->bindValue(':idPueblo', $input['idPueblo']);
      $statement->bindValue(':saldo', $input['saldo']);
      $statement->execute();
    }
      header("HTTP/1.1 200 OK");
      exit();
}


//En caso de que ninguna de las opciones anteriores se haya ejecutado
header("HTTP/1.1 400 Bad Request");

?>