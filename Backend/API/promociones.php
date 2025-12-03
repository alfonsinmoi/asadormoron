<?php
include "config.php";
include "utils.php";


$dbConn =  connect($db);
/*
  listar todos los posts o solo uno
 */
if ($_SERVER['REQUEST_METHOD'] == 'GET')
{
  if (isset($_GET['ofertas']))
  {
  $sql = $dbConn->prepare("SELECT * FROM qo_ofertas where idGrupo=:idGrupo order by id Desc");
  $sql->bindValue(':idGrupo', $_GET['idGrupo']);  
  $sql->execute();
    $sql->setFetchMode(PDO::FETCH_ASSOC);
    header("HTTP/1.1 200 OK");
    echo json_encode( $sql->fetchAll()  );
    exit();
  }else if (isset($_GET['cuponSQL']))
  {
    $sql = $dbConn->prepare("SELECT * FROM `qo_cupones_sql` WHERE codigo=:cupon and estado=1 and pueblo=:idPueblo");
    $sql->bindValue(':cupon', $_GET['cupon']);  
    $sql->bindValue(':idPueblo', $_GET['idPueblo']);  
    $sql->execute();
    $result = $sql->fetch(PDO::FETCH_OBJ);
    $sentencia= $result->sentencia;
    $valorSentencia=$result->valorSentencia;
    $sql2 = $dbConn->prepare($sentencia." AND idUsuario=:idUsuario");
    $sql2->bindValue(':idUsuario', $_GET['idUsuario']);  
    $sql2->execute();
    $result2 = $sql2->fetch(PDO::FETCH_OBJ);
    $contador=$result2->contador;
    $devolver=false;
    if ($contador==$valorSentencia)
      $devolver=true;
    header("HTTP/1.1 200 OK");
    if ($devolver)
      echo json_encode( $result );
    else
      echo "";
    exit();
  }else if (isset($_GET['cupones']))
  {
    $sql = $dbConn->prepare("SELECT * FROM qo_cupones where idGrupo=:idGrupo order by id desc");
  $sql->bindValue(':idGrupo', $_GET['idGrupo']);  
    $sql->execute();
    $sql->setFetchMode(PDO::FETCH_ASSOC);
    header("HTTP/1.1 200 OK");
    echo json_encode( $sql->fetchAll()  );
    exit();
  }else if (isset($_GET['sorteo']))
  {
    $sql = $dbConn->prepare("SELECT * FROM qo_sorteos_numeros where idCliente=:id order by id");
  $sql->bindValue(':id', $_GET['idUsuario']);  
    $sql->execute();
    $sql->setFetchMode(PDO::FETCH_ASSOC);
    header("HTTP/1.1 200 OK");
    echo json_encode( $sql->fetchAll()  );
    exit();
  }else if (isset($_GET['promocionAmigo']))
  {
    $sql = $dbConn->prepare("SELECT * FROM qo_promocion_amigo where idPueblo=:idPueblo order by id desc");
  $sql->bindValue(':idPueblo', $_GET['idPueblo']);  
    $sql->execute();
    $sql->setFetchMode(PDO::FETCH_ASSOC);
    header("HTTP/1.1 200 OK");
    echo json_encode( $sql->fetchAll()  );
    exit();
  }else if (isset($_GET['compruebaCodigoAmigo']))
  {
    $sql = $dbConn->prepare("select * from qo_users u WHERE codigo=:codigo and estado=1 and rol=1 and idPueblo=:idPueblo");
    $sql->bindValue(':idPueblo', $_GET['idPueblo']);
    $sql->bindValue(':codigo', $_GET['codigo']);  
    $sql->execute();
    $sql->setFetchMode(PDO::FETCH_ASSOC);
    header("HTTP/1.1 200 OK");
    echo json_encode( $sql->fetchAll()  );
    exit();
  }else if (isset($_GET['ofertasUsuario']))
  {
  $sql = $dbConn->prepare("SELECT * FROM qo_ofertas where estado=1 and fechaHasta>=now() and idGrupo=:idGrupo and (DAYOFWEEK(now()=1) and Domingo=1)
                          and (DAYOFWEEK(now()=2) and Lunes=1) and (DAYOFWEEK(now()=3) and Martes=1) and (DAYOFWEEK(now()=4) and Miercoles=1) 
                          and (DAYOFWEEK(now()=5) and Jueves=1) and (DAYOFWEEK(now()=6) and Viernes=1) and (DAYOFWEEK(now()=7) and Sabado=1)");
  $sql->bindValue(':idGrupo', $_GET['idGrupo']);  
  $sql->execute();
    $sql->setFetchMode(PDO::FETCH_ASSOC);
    header("HTTP/1.1 200 OK");
    echo json_encode( $sql->fetchAll()  );
    exit();
  }else if (isset($_GET['activacion']))
  {
  $sql = $dbConn->prepare("SELECT * FROM `qo_amigos` where idAmigo=:id and canjeado=0 and idPueblo=:idPueblo");
  $sql->bindValue(':idPueblo', $_GET['idPueblo']);  
  $sql->bindValue(':id', $_GET['id']);  
  $sql->execute();
    $sql->setFetchMode(PDO::FETCH_ASSOC);
    header("HTTP/1.1 200 OK");
    echo json_encode( $sql->fetchAll()  );
    exit();
  }else if (isset($_GET['cuponesUsuario']))
  {
    $sql = $dbConn->prepare("SELECT * FROM qo_cupones where estado=1 and fechaHasta>=now() and idGrupo=:idGrupo and (DAYOFWEEK(now()=1) and Domingo=1)
    and (DAYOFWEEK(now()=2) and Lunes=1) and (DAYOFWEEK(now()=3) and Martes=1) and (DAYOFWEEK(now()=4) and Miercoles=1) 
    and (DAYOFWEEK(now()=5) and Jueves=1) and (DAYOFWEEK(now()=6) and Viernes=1) and (DAYOFWEEK(now()=7) and Sabado=1)");
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
  if (isset($_GET['cupones']))
  {
    $input = json_decode(file_get_contents('php://input'), true);
    $sql = "INSERT INTO `qo_cupones`(`codigoCupon`, `limitado`, `cantidad`, `pueblo`, `idPueblo`, `establecimiento`, `idEstablecimiento`, `producto`, `idProducto`, `categoria`, `idCategoria`, 
    `gastos`, `tipoOferta`, `fechaDesde`, `fechaHasta`, `Lunes`, `Martes`, `Miercoles`, `Jueves`, `Viernes`, `Sabado`, `Domingo`, `estado`, `creador`, 
    `valor`, `idGrupo`) 
    VALUES (:codigoCupon,:limitado,:cantidad,:pueblo,:idPueblo,:establecimiento,:idEstablecimiento,:producto,:idProducto,:categoria,:idCategoria,:gastos,:tipoOferta,
    :fechaDesde,:fechaHasta,:Lunes,:Martes,:Miercoles,:Jueves,:Viernes,:Sabado,:Domingo,:estado,:creador,:valor,:idGrupo)";
    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':codigoCupon', $input['codigoCupon']);
    $statement->bindValue(':limitado', $input['limitado']);
    $statement->bindValue(':cantidad', $input['cantidad']);
    $statement->bindValue(':pueblo', $input['pueblo']);
    $statement->bindValue(':idPueblo', $input['idPueblo']);
    $statement->bindValue(':establecimiento', $input['establecimiento']);
    $statement->bindValue(':idEstablecimiento', $input['idEstablecimiento']);
    $statement->bindValue(':producto', $input['producto']);
    $statement->bindValue(':idProducto', $input['idProducto']);
    $statement->bindValue(':categoria', $input['categoria']);
    $statement->bindValue(':idCategoria', $input['idCategoria']);
    $statement->bindValue(':gastos', $input['gastos']);
    $statement->bindValue(':tipoOferta', $input['tipoOferta']);
    $statement->bindValue(':fechaDesde', $input['fechaDesde']);
    $statement->bindValue(':fechaHasta', $input['fechaHasta']);
    $statement->bindValue(':Lunes', $input['lunes']);
    $statement->bindValue(':Martes', $input['martes']);
    $statement->bindValue(':Miercoles', $input['miercoles']);
    $statement->bindValue(':Jueves', $input['jueves']);
    $statement->bindValue(':Viernes', $input['viernes']);
    $statement->bindValue(':Sabado', $input['sabado']);
    $statement->bindValue(':Domingo', $input['domingo']);
    $statement->bindValue(':estado', $input['estado']);
    $statement->bindValue(':creador', $input['creador']);
    $statement->bindValue(':valor', $input['valor']);
    $statement->bindValue(':idGrupo', $input['idGrupo']);
    //bindAllValues($statement, $input);
    $statement->execute();
    $postId = $dbConn->lastInsertId();
    if($postId)
    {
      $input['id'] = $postId;
      header("HTTP/1.1 200 OK");
      echo json_encode($input);
      exit();
   }
  }else if (isset($_GET['promoAmigos'])){

    $input = json_decode(file_get_contents('php://input'), true);
    $sql = "INSERT INTO `qo_promocion_amigo`(nombre, descripcion, fechaInicio, fechaFin, idPueblo, saldoUsuario, saldoAmigo, personasAlcanzadas, saldoRepartido, activo, pedidoMinimo) VALUES (:nombre, :descripcion, :fechaInicio, :fechaFin, :idPueblo, :saldoUsuario, :saldoAmigo, :personasAlcanzadas, :saldoRepartido, :activo, :pedidoMinimo)";
    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':nombre', $input['nombre']);
    $statement->bindValue(':descripcion', $input['descripcion']);
    $statement->bindValue(':fechaInicio', $input['fechaInicio']);
    $statement->bindValue(':fechaFin', $input['fechaFin']);
    $statement->bindValue(':idPueblo', $input['idPueblo']);
    $statement->bindValue(':saldoUsuario', $input['saldoUsuario']);
    $statement->bindValue(':saldoAmigo', $input['saldoAmigo']);
    $statement->bindValue(':personasAlcanzadas', $input['personasAlcanzadas']);
    $statement->bindValue(':saldoRepartido', $input['saldoRepartido']);
    $statement->bindValue(':activo', $input['activo']);
    $statement->bindValue(':pedidoMinimo', $input['pedidoMinimo']);
    //bindAllValues($statement, $input);
    $statement->execute();
    $postId = $dbConn->lastInsertId();
    if($postId)
    {
      $input['id'] = $postId;
      header("HTTP/1.1 200 OK");
      echo json_encode($input);
      exit();
	 }
  }else if (isset($_GET['amigos'])){

    $input = json_decode(file_get_contents('php://input'), true);

    $sql = $dbConn->prepare("SELECT * FROM `qo_users` WHERE codigo=:codigo and estado=1 and idPueblo=:idPueblo");
    $sql->bindValue(':codigo', $input['codigo']);  
    $sql->bindValue(':idPueblo', $input['idPueblo']);  
    $sql->execute();
    $result = $sql->fetch(PDO::FETCH_OBJ);
    $idCliente= $result->id;

    $sql = $dbConn->prepare("SELECT * FROM `qo_users` WHERE telefono=:telefono and idPueblo=:idPueblo");
    $sql->bindValue(':telefono', $input['telefono']);  
    $sql->bindValue(':idPueblo', $input['idPueblo']);  
    $sql->execute();
    $result = $sql->fetch(PDO::FETCH_OBJ);
    $idAmigo= $result->id;

    $sql = $dbConn->prepare("SELECT * FROM `qo_promocion_amigo` WHERE id=:idPromo and activo=1 and now()>=fechaInicio and now()<=fechaFin");
    $sql->bindValue(':idPromo', $input['idPromo']);  
    $sql->execute();
    $result = $sql->fetch(PDO::FETCH_OBJ);
    $saldoCliente= $result->saldoUsuario;
    $saldoAmigo= $result->saldoAmigo;


    $sql = "INSERT INTO `qo_amigos`(idCliente, idAmigo, idPueblo, saldoCliente,saldoAmigo,idPromo) VALUES (:idCliente, :idAmigo, :idPueblo, :saldoCliente,:saldoAmigo,:idPromo)";
    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':idCliente', $idCliente);
    $statement->bindValue(':idAmigo', $idAmigo);
    $statement->bindValue(':idPueblo', $input['idPueblo']);
    $statement->bindValue(':saldoCliente', $saldoCliente);
    $statement->bindValue(':saldoAmigo', $saldoAmigo);
    $statement->bindValue(':idPromo', $input['idPromo']); 
    //bindAllValues($statement, $input);
    $statement->execute();
    $postId = $dbConn->lastInsertId();
    if($postId)
    {
      $input['id'] = $postId;
      header("HTTP/1.1 200 OK");
      echo json_encode($input);
      exit();
	 }
  }else if (isset($_GET['activaAmigo'])){

    $input = json_decode(file_get_contents('php://input'), true);

    $sql = $dbConn->prepare("UPDATE qo_amigos set canjeado=1 WHere id=:idPromo");
    $sql->bindValue(':idPromo', $input['idPromo']);  
    $sql->execute();


    $sql = $dbConn->prepare("SELECT * FROM `qo_amigos` WHere id=:idPromo");
    $sql->bindValue(':idPromo', $input['idPromo']);  
    $sql->execute();
    $result = $sql->fetch(PDO::FETCH_OBJ);
    $idAmigo= $result->idAmigo;
    $idCliente= $result->idCliente;
    $saldoAmigo= $result->saldoAmigo;
    $saldoCliente= $result->saldoCliente;
    $saldo=$saldoAmigo+$saldoCliente;
    $sql = $dbConn->prepare("UPDATE qo_users set saldo=saldo+:saldo WHere id=:idCliente");
    $sql->bindValue(':saldo', $saldoCliente);  
    $sql->bindValue(':idCliente', $idCliente);  
    $sql->execute();

    $sql = $dbConn->prepare("UPDATE qo_users set saldo=saldo+:saldo WHere id=:idAmigo");
    $sql->bindValue(':saldo', $saldoAmigo);  
    $sql->bindValue(':idAmigo', $idAmigo);  
    $sql->execute();

    $sql = $dbConn->prepare("UPDATE qo_promocion_amigo set personasAlcanzadas=personasAlcanzadas+2,saldoRepartido=saldoRepartido+:saldo WHere id=:idPromo");
    $sql->bindValue(':saldo', $saldo);  
    $sql->bindValue(':idPromo', $input['idPromo']);  
    $sql->execute();

      header("HTTP/1.1 200 OK");
      echo json_encode("OK");
      exit();
  }else if (isset($_GET['sorteo'])){

    $i = 1;
    while ($i <= 4) {
      try {
        $sql = $dbConn->prepare("INSERT INTO `qo_sorteos_numeros` (idSorteo,numero,idCliente,fecha) select s.id,LPAD(FLOOR(RAND()*(9999+1)), 4, '0'),:idUsuario,now() from qo_sorteos s left join qo_sorteos_numeros n on (s.id=n.idSorteo) Where s.idPueblo=:idPueblo limit 1");
        $sql->bindValue(':idUsuario', $_GET['idUsuario']);  
        $sql->bindValue(':idPueblo', $_GET['idPueblo']);  
        $sql->execute();
        if ($sql->rowCount()==1)
          $i=$i+1;
      } catch (Exception $e) {

      }
    }
    $sql = $dbConn->prepare("SELECT group_concat(numero) as numero FROM (select numero from `qo_sorteos_numeros` WHere idCliente=:idUsuario order by id desc limit 4) a");
    $sql->bindValue(':idUsuario', $_GET['idUsuario']);  
    $sql->execute();
    $result = $sql->fetch(PDO::FETCH_OBJ);
    $numero= $result->numero;

      header("HTTP/1.1 200 OK");
      echo json_encode($numero);
      exit();
  }else{

    $input = json_decode(file_get_contents('php://input'), true);
    $sql = "INSERT INTO `qo_productos_cat`(nombre,nombre_eng,nombre_fr,nombre_ger, estado, idEstablecimiento, tipo) VALUES (:nombre,:nombre,:nombre,:nombre,:estado,:idEstablecimiento,:tipo)";
    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':nombre', $input['nombre']);
    $statement->bindValue(':estado', $input['estado']);
    $statement->bindValue(':idEstablecimiento', $input['idEstablecimiento']);
    $statement->bindValue(':tipo', $input['idTipo']);
    //bindAllValues($statement, $input);
    $statement->execute();
    $postId = $dbConn->lastInsertId();
    if($postId)
    {
      $input['id'] = $postId;
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
  }
}

//Actualizar
if ($_SERVER['REQUEST_METHOD'] == 'PUT')
{
  if (isset($_GET['cupones'])){
    $input = json_decode(file_get_contents('php://input'), true);
    $userId = $input['id'];
    $fields = getParams($input);

    $sql = "
          UPDATE qo_cupones
          SET $fields
          WHERE id='$userId'
           ";

    $statement = $dbConn->prepare($sql);
    bindAllValues($statement, $input);

    $statement->execute();
  }else if (isset($_GET['oferta'])){
    $input = json_decode(file_get_contents('php://input'), true);
    $userId = $input['id'];
    $fields = getParams($input);

    $sql = "
          UPDATE qo_ofertas
          SET $fields
          WHERE id='$userId'
           ";

    $statement = $dbConn->prepare($sql);
    bindAllValues($statement, $input);

    $statement->execute();
  }else if (isset($_GET['promoAmigos'])){
    $input = json_decode(file_get_contents('php://input'), true);
    $userId = $input['id'];
    $fields = getParams($input);

    $sql = "
          UPDATE qo_promocion_amigo
          SET $fields
          WHERE id='$userId'
           ";

    $statement = $dbConn->prepare($sql);
    bindAllValues($statement, $input);

    $statement->execute();
  }
    header("HTTP/1.1 200 OK");
    echo json_encode($input);
    exit();
}


//En caso de que ninguna de las opciones anteriores se haya ejecutado
header("HTTP/1.1 400 Bad Request");

?>