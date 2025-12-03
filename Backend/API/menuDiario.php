<?php
include "config.php";
include "utils.php";


$dbConn =  connect($db);
/*
  listar todos los posts o solo uno
 */
if ($_SERVER['REQUEST_METHOD'] == 'GET')
{
  if (isset($_GET['configuracion']))
  {
    
    $sql=$dbConn->prepare("SELECT * FROM qo_menu_diario_conf where idEstablecimiento=:id");
    $sql->bindValue(':id', $_GET['configuracion']);  
    $sql->execute();
    header("HTTP/1.1 200 OK");
    echo json_encode(  $sql->fetch(PDO::FETCH_ASSOC)  );
    exit();
  }else if (isset($_GET['productos']))
  {
    $sql = $dbConn->prepare("SELECT * from qo_menu_diario_prod Where idMenu=:id order by tipo");
    $sql->bindValue(':id', $_GET['productos']);  
    $sql->execute();
    $sql->setFetchMode(PDO::FETCH_ASSOC);
    header("HTTP/1.1 200 OK");
    echo json_encode( $sql->fetchAll()  );
    exit();
  }else{
    $sql = $dbConn->prepare("SELECT * from qo_menu_diario Where idEstablecimiento=:id");
    $sql->bindValue(':id', $_GET['idEstablecimiento']);  
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
    if (isset($_GET['productos']))
  {
    $sql = "INSERT INTO `qo_menu_diario_prod` (idMenu,tipo,nombre,imagen) VALUES 
    (:idMenu,:tipo,:nombre,:imagen)";
    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':idMenu', $input['idMenu']);
    $statement->bindValue(':tipo', $input['tipo']);
    $statement->bindValue(':nombre', $input['nombre']);
    $statement->bindValue(':imagen', $input['imagen']);
    $statement->execute();
    $postId = $dbConn->lastInsertId();
    if($postId)
    {
      $input['id'] = $postId;
      header("HTTP/1.1 200 OK");
      echo json_encode($input);
      exit();
	 }
  }else if (isset($_GET['configuracion']))
  {
    $sql = "INSERT INTO `qo_menu_diario_conf` (`idEstablecimiento`, `horaInicioLunes`, `horaFinLunes`, `horaInicioMartes`, `horaFinMartes`, `horaInicioMiercoles`, `horaFinMiercoles`, `horaInicioJueves`, `horaFinJueves`, `horaInicioViernes`, `horaFinViernes`, `tiempoMaximo`, `activoLunes`, `activoMartes`, `activoMiercoles`, `activoJueves`, `activoViernes`) VALUES 
    (:idEstablecimiento`, :horaInicioLunes, :horaFinLunes, :horaInicioMartes, :horaFinMartes, :horaInicioMiercoles, :horaFinMiercoles, :horaInicioJueves, :horaFinJueves, :horaInicioViernes, :horaFinViernes, :tiempoMaximo, :activoLunes, :activoMartes, :activoMiercoles, :activoJueves, :activoViernes)";
    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':idEstablecimiento', $input['idEstablecimiento']);
    $statement->bindValue(':horaInicioLunes', $input['horaInicioLunes']);
    $statement->bindValue(':horaFinLunes', $input['horaFinLunes']);
    $statement->bindValue(':horaInicioMartes', $input['horaInicioMartes']);

    $statement->bindValue(':horaFinMartes', $input['horaFinMartes']);
    $statement->bindValue(':horaInicioMiercoles', $input['horaInicioMiercoles']);
    $statement->bindValue(':horaFinMiercoles', $input['horaFinMiercoles']);
    $statement->bindValue(':horaInicioJueves', $input['horaInicioJueves']);
    $statement->bindValue(':horaFinJueves', $input['horaFinJueves']);
    $statement->bindValue(':horaInicioViernes', $input['horaInicioViernes']);
    $statement->bindValue(':horaFinViernes', $input['horaFinViernes']);
    $statement->bindValue(':tiempoMaximo', $input['tiempoMaximo']);
    $statement->bindValue(':activoLunes', $input['activoLunes']);
    $statement->bindValue(':activoMartes', $input['activoMartes']);
    $statement->bindValue(':activoMiercoles', $input['activoMiercoles']);
    $statement->bindValue(':activoJueves', $input['activoJueves']);
    $statement->bindValue(':activoViernes', $input['activoViernes']);
    $statement->bindValue(':maxPedidos', $input['maxPedidos']);
    $statement->bindValue(':cartaYMenu', $input['cartaYMenu']);
    $statement->bindValue(':postreObligatorio', $input['postreObligatorio']);
    $statement->bindValue(':extraPostre', $input['extraPostre']);
    $statement->bindValue(':bebidaObligatoria', $input['bebidaObligatoria']);
    $statement->bindValue(':extraBebida', $input['extraBebida']);
    $statement->bindValue(':platoUnico', $input['platoUnico']);
    $statement->bindValue(':precioPlatoUnico', $input['precioPlatoUnico']);
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
    $sql = "INSERT INTO `qo_menu_diario` (idEstablecimiento,precio,nombre,descripcion,activo) VALUES 
    (:idEstablecimiento,:precio,:nombre,:descripcion,:activo)";
    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':idEstablecimiento', $input['idEstablecimiento']);
    $statement->bindValue(':precio', $input['precio']);
    $statement->bindValue(':nombre', $input['nombre']);
    $statement->bindValue(':descripcion', $input['descripcion']);
    $statement->bindValue(':activo', $input['activo']);
    $statement->execute();
    $postId = $dbConn->lastInsertId();
    if($postId)
    {
      $input['id'] = $postId;
      $sql="INSERT INTO `qo_menu_diario_conf` (`idEstablecimiento`, `horaInicioLunes`, `horaFinLunes`, `horaInicioMartes`, `horaFinMartes`, `horaInicioMiercoles`, `horaFinMiercoles`, `horaInicioJueves`, `horaFinJueves`, `horaInicioViernes`, `horaFinViernes`, `tiempoMaximo`, `activoLunes`, `activoMartes`, `activoMiercoles`, `activoJueves`, `activoViernes`,maxPedidos) VALUES 
      (:idEstablecimiento, '13:00', '16:00', '13:00', '16:00', '13:00', '16:00', '13:00', '16:00', '13:00', '16:00', 30, 0, 0, 0, 0, 0,999)";
      $statement = $dbConn->prepare($sql);
      $statement->bindValue(':idEstablecimiento', $input['idEstablecimiento']);
      $statement->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode($input);
      exit();
	  }
  }
}

//Actualizar
if ($_SERVER['REQUEST_METHOD'] == 'PUT')
{
    $input = json_decode(file_get_contents('php://input'), true);
    $userId = $input['id'];
    $fields = getParams($input);
    if (isset($_GET['productos'])){
      $sql = "
          UPDATE qo_menu_diario_prod
          SET $fields
          WHERE id='$userId'
           ";
           $statement = $dbConn->prepare($sql);
    bindAllValues($statement, $input);

    $statement->execute();
    header("HTTP/1.1 200 OK");
    exit();
    }else if (isset($_GET['configuracion'])){
      $sql = "UPDATE `qo_menu_diario_conf` SET `idEstablecimiento`=:idEstablecimiento,`horaInicioLunes`=:horaInicioLunes,`horaFinLunes`=:horaFinLunes,`horaInicioMartes`=:horaInicioMartes,`horaFinMartes`=:horaFinMartes,
            `horaInicioMiercoles`=:horaInicioMiercoles,`horaFinMiercoles`=:horaFinMiercoles,`horaInicioJueves`=:horaInicioJueves,`horaFinJueves`=:horaFinJueves,`horaInicioViernes`=:horaInicioViernes,
            `horaFinViernes`=:horaFinViernes,`tiempoMaximo`=:tiempoMaximo,`activoLunes`=:activoLunes,`activoMartes`=:activoMartes,`activoMiercoles`=:activoMiercoles,`activoJueves`=:activoJueves,
            `activoViernes`=:activoViernes,`maxPedidos`=:maxPedidos,`cartaYMenu`=:cartaYMenu,`postreObligatorio`=:postreObligatorio,`extraPostre`=:extraPostre,`bebidaObligatoria`=:bebidaObligatoria,
            `extraBebida`=:extraBebida,`platoUnico`=:platoUnico,`precioPlatoUnico`=:precioPlatoUnico WHERE id='$userId'";
          
          $statement = $dbConn->prepare($sql);
    $statement->bindValue(':idEstablecimiento', $input['idEstablecimiento']);
    $statement->bindValue(':horaInicioLunes', $input['horaInicioLunes']);
    $statement->bindValue(':horaFinLunes', $input['horaFinLunes']);
    $statement->bindValue(':horaInicioMartes', $input['horaInicioMartes']);
    $statement->bindValue(':horaFinMartes', $input['horaFinMartes']);
    $statement->bindValue(':horaInicioMiercoles', $input['horaInicioMiercoles']);
    $statement->bindValue(':horaFinMiercoles', $input['horaFinMiercoles']);
    $statement->bindValue(':horaInicioJueves', $input['horaInicioJueves']);
    $statement->bindValue(':horaFinJueves', $input['horaFinJueves']);
    $statement->bindValue(':horaInicioViernes', $input['horaInicioViernes']);
    $statement->bindValue(':horaFinViernes', $input['horaFinViernes']);
    $statement->bindValue(':tiempoMaximo', $input['tiempoMaximo']);
    $statement->bindValue(':activoLunes', $input['activoLunes']);
    $statement->bindValue(':activoMartes', $input['activoMartes']);
    $statement->bindValue(':activoMiercoles', $input['activoMiercoles']);
    $statement->bindValue(':activoJueves', $input['activoJueves']);
    $statement->bindValue(':activoViernes', $input['activoViernes']);
    $statement->bindValue(':maxPedidos', $input['maxPedidos']);
    $statement->bindValue(':cartaYMenu', $input['cartaYMenu']);
    $statement->bindValue(':postreObligatorio', $input['postreObligatorio']);
    $statement->bindValue(':extraPostre', $input['extraPostre']);
    $statement->bindValue(':bebidaObligatoria', $input['bebidaObligatoria']);
    $statement->bindValue(':extraBebida', $input['extraBebida']);
    $statement->bindValue(':platoUnico', $input['platoUnico']);
    $statement->bindValue(':precioPlatoUnico', $input['precioPlatoUnico']);
    $statement->execute();

    header("HTTP/1.1 200 OK");
    exit();
    }else{
        $sql = "
            UPDATE qo_menu_diario 
            SET idEstablecimiento=:idEstablecimiento, precio=:precio, activo=:activo, descripcion=:descripcion, nombre=:nombre
            WHERE id='$userId'
           ";
           $statement = $dbConn->prepare($sql);
    $statement->bindValue(':idEstablecimiento', $input['idEstablecimiento']);
    $statement->bindValue(':precio', $input['precio']);
    $statement->bindValue(':nombre', $input['nombre']);
    $statement->bindValue(':descripcion', $input['descripcion']);
    $statement->bindValue(':activo', $input['activo']);

    $statement->execute();
    header("HTTP/1.1 200 OK");
    exit();
    }
    
}


//En caso de que ninguna de las opciones anteriores se haya ejecutado
header("HTTP/1.1 400 Bad Request");

?>