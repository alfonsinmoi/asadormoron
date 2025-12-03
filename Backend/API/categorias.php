<?php
include "config.php";
include "utils.php";


$dbConn =  connect($db);
/*
  listar todos los posts o solo uno
 */
if ($_SERVER['REQUEST_METHOD'] == 'GET')
{
  if (isset($_GET['home']))
  {
  $sql = $dbConn->prepare("SELECT * from qo_productos_cat");
  $sql->execute();
    $sql->setFetchMode(PDO::FETCH_ASSOC);
    header("HTTP/1.1 200 OK");
    echo json_encode( $sql->fetchAll()  );
    exit();
  }else if (isset($_GET['admin']))
  {
  $sql = $dbConn->prepare("SELECT DISTINCT c.id,c.imagen,c.nombre,c.nombre_eng,c.nombre_ger,c.nombre_fr,c.estado,c.orden,c.raiz,c.proximamente,c.idGrupo,c.espuntos,c.navidad FROM qo_categorias c WHERE idGrupo=:idGrupo order by orden,nombre");
  $sql->bindValue(':idGrupo', $_GET['idGrupo']); 
    $sql->execute();
    $sql->setFetchMode(PDO::FETCH_ASSOC);
    header("HTTP/1.1 200 OK");
    echo json_encode( $sql->fetchAll()  );
    exit();
  }else if (isset($_GET['est']))
  {
  $sql = $dbConn->prepare("SELECT * from qo_categorias WHERE idGrupo=:idGrupo ORDER BY orden,nombre");
  $sql->bindValue(':idGrupo', $_GET['idGrupo']); 
    $sql->execute();
    $sql->setFetchMode(PDO::FETCH_ASSOC);
    header("HTTP/1.1 200 OK");
    echo json_encode( $sql->fetchAll()  );
    exit();
  }else if (isset($_GET['estPueblo']))
  {
  $sql = $dbConn->prepare("SELECT * from qo_categorias WHERE idPueblo=:idPueblo ORDER BY orden,nombre");
  $sql->bindValue(':idPueblo', $_GET['idPueblo']); 
    $sql->execute();
    $sql->setFetchMode(PDO::FETCH_ASSOC);
    header("HTTP/1.1 200 OK");
    echo json_encode( $sql->fetchAll()  );
    exit();
  }else if (isset($_GET['idEstablecimiento']))
  {
    $sql = $dbConn->prepare("SELECT c.navidad,c.espuntos,c.imagen,c.orden,c.numeroImpresora,c.id,c.nombre,c.nombre_eng,c.nombre_fr,c.nombre_ger,c.idEstablecimiento,if(c.tipo=0,'Bebidas','Comidas') as tipo,c.estado,c.tipo as idTipo, if(isnull(p.contador),0,p.contador) as numeroProductos FROM `qo_productos_cat` c left join (select count(*) as contador,es.idCategoria from qo_productos_est es inner join qo_productos_cat ca on (es.idCategoria=ca.id)  group by es.idCategoria) p on (p.idCategoria=c.id) WHERE idEstablecimiento=:id ORDER BY orden");
    $sql->bindValue(':id', $_GET['idEstablecimiento']);
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
  if (isset($_GET['establecimiento']))
  {
    $input = json_decode(file_get_contents('php://input'), true);
    $sql = "INSERT INTO `qo_categorias`(nombre,nombre_eng,nombre_ger,nombre_fr,estado,imagen,idPueblo,idGrupo,orden,raiz) VALUES (:nombre,:nombre_eng,:nombre_ger,:nombre_fr,:estado,:imagen,:idPueblo,:idGrupo,:orden,:raiz)";
    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':nombre', $input['nombre']);
    $statement->bindValue(':nombre_eng', $input['nombre']);
    $statement->bindValue(':nombre_ger', $input['nombre']);
    $statement->bindValue(':nombre_fr', $input['nombre']);
    $statement->bindValue(':estado', $input['estado']);
    $statement->bindValue(':imagen', $input['imagen']);
    $statement->bindValue(':idPueblo', $input['idPueblo']);
    $statement->bindValue(':idGrupo', $input['idGrupo']);
    $statement->bindValue(':orden', $input['orden']);
    $statement->bindValue(':raiz', $input['raiz']);
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
  }else{

    $input = json_decode(file_get_contents('php://input'), true);
    $sql = "INSERT INTO `qo_productos_cat`(orden,imagen,numeroImpresora,nombre,nombre_eng,nombre_fr,nombre_ger, estado, idEstablecimiento, tipo) VALUES (:orden,:imagen,:numeroImpresora,:nombre,:nombre,:nombre,:nombre,:estado,:idEstablecimiento,:tipo)";
    $numeroImpresora=1;
    if ($input['numeroImpresora']!=null){
      $numeroImpresora=$input['numeroImpresora'];
    }
    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':nombre', $input['nombre']);
    $statement->bindValue(':orden', $input['orden']);
    $statement->bindValue(':imagen', $input['imagen']);
    $statement->bindValue(':numeroImpresora', $numeroImpresora);
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
    $statement = $dbConn->prepare("DELETE from qo_productos_cat WHERE idEstablecimiento=$id and 1>1");
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
  if (isset($_GET['establecimiento'])){
    $input = json_decode(file_get_contents('php://input'), true);
    $userId = $input['id'];
    $fields = getParams($input);

    $sql = "
          UPDATE qo_categorias
          SET $fields
          WHERE id='$userId'
           ";

    $statement = $dbConn->prepare($sql);
    bindAllValues($statement, $input);

    $statement->execute();
  }else{
    $input = json_decode(file_get_contents('php://input'), true);
    $id = $input['id'];
    $sql = "UPDATE `qo_productos_cat` SET numeroImpresora=:numeroImpresora,`nombre`=:nombre,nombre_eng=:nombre,nombre_fr=:nombre,nombre_ger=:nombre, tipo=:tipo, estado=:estado, idEstablecimiento=:idEstablecimiento,imagen=:imagen,orden=:orden WHERE id=$id";

    $numeroImpresora=1;
    if ($input['numeroImpresora']!=null){
      $numeroImpresora=$input['numeroImpresora'];
    }
    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':nombre', $input['nombre']);
    $statement->bindValue(':orden', $input['orden']);
    $statement->bindValue(':imagen', $input['imagen']);
    $statement->bindValue(':numeroImpresora', $numeroImpresora);
    $statement->bindValue(':tipo', $input['idTipo']);
    $statement->bindValue(':estado', $input['estado']);
    $statement->bindValue(':idEstablecimiento', $input['idEstablecimiento']);
    $statement->execute();
  }
    header("HTTP/1.1 200 OK");
    exit();
}


//En caso de que ninguna de las opciones anteriores se haya ejecutado
header("HTTP/1.1 400 Bad Request");

?>