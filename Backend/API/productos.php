<?php
include "config.php";
include "utils.php";


$dbConn =  connect($db);
/*
  listar todos los posts o solo uno
 */
if ($_SERVER['REQUEST_METHOD'] == 'GET')
{
  if (isset($_GET['idUsuarioFavoritos']))
  {
    if ($_GET['idUsuarioFavoritos']==0){
      $sql = $dbConn->prepare("SELECT count(*) as pedidos,esComercio,orden,idCategoria,pedidoMinimo,activoLunes,activoMartes,activoMiercoles,activoJueves,activoViernes,inicioLunes,inicioMartes,inicioMiercoles,inicioJueves,inicioViernes,activoSabado,activoDomingo,inicioSabado,inicioDomingo,finLunes,finMartes,finMiercoles,finJueves,finViernes,finSabado,finDomingo,servicioActivo,if(isnull(tiempoEntrega),0,tiempoEntrega) as tiempoEntrega,if(isnull(e.usuarioBarra),'',e.usuarioBarra) as usuarioBarra,if(isnull(e.usuarioCocina),'',e.usuarioCocina) as usuarioCocina,e.ipImpresora,e.nombreImpresoraCocina,e.nombreImpresoraBarra,0 as distancia,e.valoraciones,e.puntos,e.latitud,e.longitud,e.id,e.nombre,e.direccion,e.poblacion,e.codPostal,e.tipo,e.idTipo, e.imagen,e.provincia,e.telefono,e.email, e.numeroCategorias, e.numeroProductos, e.ventas, e.zonas,e.local,e.envio,e.recogida,e.logo,null as fechaInicio,null as fechaFin,'' as descripcion,'' as equipoLocal,'' as imagenEquipoLocal,'' as equipoVisitante,'' as imagenEquipoVisitante,'' as jornada,'' as temporada,'' as estadio, e.llamadaCamarero,e.puedeReservar,`inicioLunesTarde`, `inicioMartesTarde`, `inicioMiercolesTarde`, `inicioJuevesTarde`, `inicioViernesTarde`, `inicioSabadoTarde`, `inicioDomingoTarde`, `finLunesTarde`, `finMartesTarde`, `finMiercolesTarde`, `finJuevesTarde`, `finViernesTarde`, `finSabadoTarde`, `finDomingoTarde`,e.idZona from v_establecimientos e inner join qo_pedidos p on (p.idEstablecimiento=e.id) inner join qo_users u on (u.id=p.idUsuario) GROUP BY p.idEstablecimiento ORDER BY `pedidos` DESC");
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
    }else{
      $sql = $dbConn->prepare("SELECT count(*) as pedidos,esComercio,orden,idCategoria,pedidoMinimo,activoLunes,activoMartes,activoMiercoles,activoJueves,activoViernes,inicioLunes,inicioMartes,inicioMiercoles,inicioJueves,inicioViernes,activoSabado,activoDomingo,inicioSabado,inicioDomingo,finLunes,finMartes,finMiercoles,finJueves,finViernes,finSabado,finDomingo,servicioActivo,if(isnull(tiempoEntrega),0,tiempoEntrega) as tiempoEntrega,if(isnull(e.usuarioBarra),'',e.usuarioBarra) as usuarioBarra,if(isnull(e.usuarioCocina),'',e.usuarioCocina) as usuarioCocina,e.ipImpresora,e.nombreImpresoraCocina,e.nombreImpresoraBarra,0 as distancia,e.valoraciones,e.puntos,e.latitud,e.longitud,e.id,e.nombre,e.direccion,e.poblacion,e.codPostal,e.tipo,e.idTipo, e.imagen,e.provincia,e.telefono,e.email, e.numeroCategorias, e.numeroProductos, e.ventas, e.zonas,e.local,e.envio,e.recogida,e.logo,null as fechaInicio,null as fechaFin,'' as descripcion,'' as equipoLocal,'' as imagenEquipoLocal,'' as equipoVisitante,'' as imagenEquipoVisitante,'' as jornada,'' as temporada,'' as estadio, e.llamadaCamarero,e.puedeReservar,`inicioLunesTarde`, `inicioMartesTarde`, `inicioMiercolesTarde`, `inicioJuevesTarde`, `inicioViernesTarde`, `inicioSabadoTarde`, `inicioDomingoTarde`, `finLunesTarde`, `finMartesTarde`, `finMiercolesTarde`, `finJuevesTarde`, `finViernesTarde`, `finSabadoTarde`, `finDomingoTarde`,e.idZona from v_establecimientos e inner join qo_pedidos p on (p.idEstablecimiento=e.id) inner join qo_users u on (u.id=p.idUsuario) WHERE p.idUsuario=:id GROUP BY p.idEstablecimiento ORDER BY `pedidos` DESC");
      $sql->bindValue(':id', $_GET['idUsuarioFavoritos']);  
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
    }
      exit();
  }else if (isset($_GET['alergenos']))
  {
      $sql = $dbConn->prepare("select * from qo_alergenos"); 
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
      exit();
  }else if (isset($_GET['cantidadIngredientes']))
  {
     $sql=$dbConn->prepare("SELECT if(fuerzaIngredientes=0,0,numeroIngredientes) as ingredientes FROM qo_productos_est where id=:id");
     $sql->bindValue(':id', $_GET['cantidadIngredientes']);  
     $sql->execute();
     $result = $sql->fetch(PDO::FETCH_OBJ);
      header("HTTP/1.1 200 OK");
      echo json_encode($result->ingredientes);
      exit();
  }else if (isset($_GET['masvendidos']))
  {
    if ($_GET['idGrupo']=='0'){
      $sql = $dbConn->prepare("select fuerzaIngredientes,numeroIngredientes, sum(d.cantidad) as ventas,es.nombre as nombreEstablecimiento,c.estado as estadoCategoria,c.idEstablecimiento,e.id,e.nombre,if(e.nombre_eng='',e.nombre,e.nombre_eng) as nombre_eng,if(e.nombre_ger='',e.nombre,e.nombre_ger) as nombre_ger,if(e.nombre_fr='',e.nombre,e.nombre_fr) as nombre_fr,e.idCategoria,e.porEncargo,c.nombre as categoria,if(c.nombre_eng='',c.nombre,c.nombre_eng) as categoria_eng,if(c.nombre_ger='',c.nombre,c.nombre_ger) as categoria_ger,if(c.nombre_fr='',c.nombre,c.nombre_fr) as categoria_fr,c.tipo as idTipoCategoria,e.imagen,e.estado,e.precio,if(isnull(e.descripcion),'',e.descripcion) as descripcion,if(isnull(e.descripcion_eng),'',e.descripcion_eng) as descripcion_eng,if(isnull(e.descripcion_ger),'',e.descripcion_ger) as descripcion_ger,if(isnull(e.descripcion_fr),'',e.descripcion_fr) as descripcion_fr,'' as opciones,'' as ingredientes,'' as alergenos from qo_productos_cat c inner join qo_productos_est e on (e.idCategoria=c.id) inner join qo_pedidos_detalle d on (d.idProducto=e.id) inner join qo_establecimientos es on (es.id=c.idEstablecimiento) WHERE e.eliminado=0 and d.tipoVenta<>'Local' group by e.id ORDER BY `ventas`  DESC limit 10");
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
    }else{
      $sql = $dbConn->prepare("select fuerzaIngredientes,numeroIngredientes, sum(d.cantidad) as ventas,es.nombre as nombreEstablecimiento,c.estado as estadoCategoria,c.idEstablecimiento,e.id,e.nombre,if(e.nombre_eng='',e.nombre,e.nombre_eng) as nombre_eng,if(e.nombre_ger='',e.nombre,e.nombre_ger) as nombre_ger,if(e.nombre_fr='',e.nombre,e.nombre_fr) as nombre_fr,e.idCategoria,e.porEncargo,c.nombre as categoria,if(c.nombre_eng='',c.nombre,c.nombre_eng) as categoria_eng,if(c.nombre_ger='',c.nombre,c.nombre_ger) as categoria_ger,if(c.nombre_fr='',c.nombre,c.nombre_fr) as categoria_fr,c.tipo as idTipoCategoria,e.imagen,e.estado,e.precio,if(isnull(e.descripcion),'',e.descripcion) as descripcion,if(isnull(e.descripcion_eng),'',e.descripcion_eng) as descripcion_eng,if(isnull(e.descripcion_ger),'',e.descripcion_ger) as descripcion_ger,if(isnull(e.descripcion_fr),'',e.descripcion_fr) as descripcion_fr,'' as opciones,'' as ingredientes,'' as alergenos from qo_productos_cat c inner join qo_productos_est e on (e.idCategoria=c.id) inner join qo_pedidos_detalle d on (d.idProducto=e.id) inner join qo_establecimientos es on (es.id=c.idEstablecimiento) WHERE e.eliminado=0 and es.idGrupo=:idGrupo and d.tipoVenta<>'Local' group by e.id ORDER BY `ventas`  DESC limit 10");
      $sql->bindValue(':idGrupo', $_GET['idGrupo']);  
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
    }
      exit();
  }else if (isset($_GET['masvendidoslocal']))
  {
      $sql = $dbConn->prepare("select fuerzaIngredientes,numeroIngredientes, sum(d.cantidad) as ventas,es.nombre as nombreEstablecimiento,c.estado as estadoCategoria,c.idEstablecimiento,e.id,e.nombre,if(e.nombre_eng='',e.nombre,e.nombre_eng) as nombre_eng,if(e.nombre_ger='',e.nombre,e.nombre_ger) as nombre_ger,if(e.nombre_fr='',e.nombre,e.nombre_fr) as nombre_fr,e.idCategoria,c.nombre as categoria,if(c.nombre_eng='',c.nombre,c.nombre_eng) as categoria_eng,if(c.nombre_ger='',c.nombre,c.nombre_ger) as categoria_ger,if(c.nombre_fr='',c.nombre,c.nombre_fr) as categoria_fr,c.tipo as idTipoCategoria,e.imagen,e.estado,e.precio,e.porEncargo,if(isnull(e.descripcion),'',e.descripcion) as descripcion,if(isnull(e.descripcion_eng),'',e.descripcion_eng) as descripcion_eng,if(isnull(e.descripcion_ger),'',e.descripcion_ger) as descripcion_ger,if(isnull(e.descripcion_fr),'',e.descripcion_fr) as descripcion_fr,'' as opciones,'' as ingredientes,'' as alergenos from qo_productos_cat c inner join qo_productos_est e on (e.idCategoria=c.id and e.eliminado=0) inner join qo_pedidos_detalle d on (d.idProducto=e.id) inner join qo_establecimientos es on (es.id=c.idEstablecimiento) WHERE e.eliminado=0 and es.idGrupo=:idGrupo and d.tipoVenta='Local' group by e.id ORDER BY `ventas`  DESC limit 10");
      $sql->bindValue(':idGrupo', $_GET['idGrupo']);  
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
      exit();
  }else if (isset($_GET['idEstablecimientoProducto']))
  {
      $sql = $dbConn->prepare("SELECT puntos,fuerzaIngredientes,numeroIngredientes,c.estado as estadoCategoria,c.idEstablecimiento,e.id as idArticulo,e.vistaEnvios,e.vistaLocal,e.precioLocal,e.nombre,if(e.nombre_eng='',e.nombre,e.nombre_eng) as nombre_eng,if(e.nombre_ger='',e.nombre,e.nombre_ger) as nombre_ger,if(e.nombre_fr='',e.nombre,e.nombre_fr) as nombre_fr,e.idCategoria,c.nombre as categoria,if(c.nombre_eng='',c.nombre,c.nombre_eng) as categoria_eng,if(c.nombre_ger='',c.nombre,c.nombre_ger) as categoria_ger,if(c.nombre_fr='',c.nombre,c.nombre_fr) as categoria_fr,c.tipo as idTipoCategoria,e.imagen,e.estado,e.porEncargo,e.precio,if(isnull(e.descripcion),'',e.descripcion) as descripcion,if(isnull(e.descripcion_eng),'',e.descripcion_eng) as descripcion_eng,if(isnull(e.descripcion_ger),'',e.descripcion_ger) as descripcion_ger,if(isnull(e.descripcion_fr),'',e.descripcion_fr) as descripcion_fr,if(isnull(o.opciones),'',o.opciones) as opciones,if(isnull(i.ingredientes),'',i.ingredientes) as ingredientes,if(isnull(a.alergenos),'',a.alergenos) as alergenos from qo_productos_cat c inner join qo_productos_est e on (e.idCategoria=c.id and e.eliminado=0) left join (select GROUP_CONCAT(o.id,';',o.opcion,';', o.tipoIncremento,';', o.valorIncremento,';', '',';', '',';', '' ,';', o.puntos SEPARATOR '|') as opciones,idProducto from qo_productos_opc o group by idProducto) o on (o.idProducto=e.id) left join (select GROUP_CONCAT(a.id,';',a.nombre,';', a.imagen SEPARATOR '|') as alergenos,idProducto from qo_productos_ing_aler i inner join qo_alergenos a on (i.idAlergeno=a.id) group by i.idProducto) a on (a.idProducto=e.id)  left join (select GROUP_CONCAT(ie.id,';',ie.nombre SEPARATOR '|') as ingredientes,p.id as idProducto from qo_ingredientes_producto i inner join qo_productos_est p on (p.id=i.idProducto and p.eliminado=0) inner join qo_productos_cat c on (c.id=p.idCategoria) inner join qo_establecimientos e on (e.id=c.idEstablecimiento) inner join qo_ingredientes_establecimiento ie on (ie.id=i.idIngrediente) Where ie.estado=1 and e.id=:id group by p.id) i on (i.idProducto=e.id) where c.idEstablecimiento=:id order by c.orden,e.idCategoria,e.nombre,e.id");
      $sql->bindValue(':id', $_GET['idEstablecimientoProducto']);  
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
      exit();
  }else if (isset($_GET['idEstablecimientoProductoCat']))
  {
      $sql = $dbConn->prepare("SELECT puntos,fuerzaIngredientes,numeroIngredientes,c.estado as estadoCategoria,c.idEstablecimiento,e.id as idArticulo,e.vistaEnvios,e.vistaLocal,e.precioLocal,e.nombre,if(e.nombre_eng='',e.nombre,e.nombre_eng) as nombre_eng,if(e.nombre_ger='',e.nombre,e.nombre_ger) as nombre_ger,if(e.nombre_fr='',e.nombre,e.nombre_fr) as nombre_fr,e.idCategoria,c.nombre as categoria,if(c.nombre_eng='',c.nombre,c.nombre_eng) as categoria_eng,if(c.nombre_ger='',c.nombre,c.nombre_ger) as categoria_ger,if(c.nombre_fr='',c.nombre,c.nombre_fr) as categoria_fr,c.tipo as idTipoCategoria,e.imagen,e.estado,e.porEncargo,e.precio,if(isnull(e.descripcion),'',e.descripcion) as descripcion,if(isnull(e.descripcion_eng),'',e.descripcion_eng) as descripcion_eng,if(isnull(e.descripcion_ger),'',e.descripcion_ger) as descripcion_ger,if(isnull(e.descripcion_fr),'',e.descripcion_fr) as descripcion_fr,if(isnull(o.opciones),'',o.opciones) as opciones,if(isnull(i.ingredientes),'',i.ingredientes) as ingredientes,if(isnull(a.alergenos),'',a.alergenos) as alergenos from qo_productos_cat c inner join qo_productos_est e on (e.idCategoria=c.id and e.eliminado=0) left join (select GROUP_CONCAT(o.id,';',o.opcion,';', o.tipoIncremento,';', o.valorIncremento,';', '',';', '',';', '',';', o.puntos SEPARATOR '|') as opciones,idProducto from qo_productos_opc o group by idProducto) o on (o.idProducto=e.id) left join (select GROUP_CONCAT(a.id,';',a.nombre,';', a.imagen SEPARATOR '|') as alergenos,idProducto from qo_productos_ing_aler i inner join qo_alergenos a on (i.idAlergeno=a.id) group by i.idProducto) a on (a.idProducto=e.id)  left join (select GROUP_CONCAT(ie.id,';',ie.nombre SEPARATOR '|') as ingredientes,p.id as idProducto from qo_ingredientes_producto i inner join qo_productos_est p on (p.id=i.idProducto and p.eliminado=0) inner join qo_productos_cat c on (c.id=p.idCategoria) inner join qo_establecimientos e on (e.id=c.idEstablecimiento) inner join qo_ingredientes_establecimiento ie on (ie.id=i.idIngrediente) Where ie.estado=1 and e.id=:id group by p.id) i on (i.idProducto=e.id) where c.id=:id order by c.orden,e.idCategoria,e.nombre,e.id");
      $sql->bindValue(':id', $_GET['idEstablecimientoProductoCat']);  
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
      exit();
  }else if (isset($_GET['idCategoriaProducto']))
  {
      $sql = $dbConn->prepare("SELECT fuerzaIngredientes,numeroIngredientes,c.idEstablecimiento,e.id,e.nombre,e.idCategoria,c.nombre as categoria,c.tipo as idTipoCategoria,e.imagen,e.estado,e.porEncargo,e.vistaEnvios,e.vistaLocal,e.precioLocal,e.precio,if(isnull(e.descripcion),'',e.descripcion) as descripcion,if(isnull(o.opciones),'',o.opciones) as opciones,if(isnull(i.ingredientes),'',i.ingredientes) as ingredientes,if(isnull(a.alergenos),'',a.alergenos) as alergenos from qo_productos_cat c inner join qo_productos_est e on (e.idCategoria=c.id and e.eliminado=0) left join (select GROUP_CONCAT(o.id,';',o.opcion,';', o.tipoIncremento,';', o.valorIncremento,';', o.puntos) as opciones,idProducto from qo_productos_opc o group by idProducto) o on (o.idProducto=e.id) left join (select GROUP_CONCAT(a.id,';',a.nombre,';', a.imagen) as alergenos,idProducto from qo_productos_ing_aler i inner join qo_alergenos a on (i.idAlergeno=a.id) group by i.idProducto) a on (a.idProducto=e.id) left join (select GROUP_CONCAT(i.id,';',i.nombre) as ingredientes,idProducto from qo_productos_ing i group by idProducto) i on (i.idProducto=e.id) where c.id=:id order by e.nombre,e.id");
      $sql->bindValue(':id', $_GET['idCategoriaProducto']);  
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
      exit();
  }else if (isset($_GET['all']))
  {
      $sql = $dbConn->prepare("SELECT fuerzaIngredientes,numeroIngredientes,c.idEstablecimiento,e.id,e.nombre,e.idCategoria,c.nombre as categoria,c.tipo as idTipoCategoria,e.imagen,e.estado,e.porEncargo,e.vistaEnvios,e.vistaLocal,e.precioLocal,e.precio,if(isnull(e.descripcion),'',e.descripcion) as descripcion,if(isnull(o.opciones),'',o.opciones) as opciones,if(isnull(i.ingredientes),'',i.ingredientes) as ingredientes,if(isnull(a.alergenos),'',a.alergenos) as alergenos from qo_productos_cat c inner join qo_productos_est e on (e.idCategoria=c.id and e.eliminado=0) left join (select GROUP_CONCAT(o.id,';',o.opcion,';', o.tipoIncremento,';', o.valorIncremento,';', o.puntos) as opciones,idProducto from qo_productos_opc o group by idProducto) o on (o.idProducto=e.id) left join (select GROUP_CONCAT(a.id,';',a.nombre,';', a.imagen) as alergenos,idProducto from qo_productos_ing_aler i inner join qo_alergenos a on (i.idAlergeno=a.id) group by i.idProducto) a on (a.idProducto=e.id) left join (select GROUP_CONCAT(i.id,';',i.nombre) as ingredientes,idProducto from qo_productos_ing i group by idProducto) i on (i.idProducto=e.id) order by e.nombre,e.id");
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
      exit();
  }else if (isset($_GET['idIngredientesProducto']))
  {
      $sql = $dbConn->prepare("SELECT p.id,p.idIngrediente,p.precio,e.nombre,e.nombre_eng,e.nombre_ger,e.nombre_fr,e.estado,p.idProducto,p.puntos FROM `qo_ingredientes_producto` p inner join qo_ingredientes_establecimiento e on (e.id=p.idIngrediente) WHERE e.estado=1 and p.idProducto=:id");
      $sql->bindValue(':id', $_GET['idIngredientesProducto']);  
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
      exit();
  }else if (isset($_GET['idEstablecimientoIngredientes']))
  {
      $sql = $dbConn->prepare("SELECT id,idEstablecimiento,nombre,precio,estado FROM `qo_ingredientes_establecimiento` WHERE idEstablecimiento=:id");
      $sql->bindValue(':id', $_GET['idEstablecimientoIngredientes']);  
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
      exit();
  }else if (isset($_GET['ids']))
  {

      $sql = $dbConn->prepare("SELECT e.vistaEnvios,e.vistaLocal,e.precioLocal,e.porEncargo,numeroIngredientes,c.estado as estadoCategoria,c.idEstablecimiento,e.id as idArticulo,e.nombre,if(e.nombre_eng='',e.nombre,e.nombre_eng) as nombre_eng,if(e.nombre_ger='',e.nombre,e.nombre_ger) as nombre_ger,if(e.nombre_fr='',e.nombre,e.nombre_fr) as nombre_fr,e.idCategoria,c.nombre as categoria,if(c.nombre_eng='',c.nombre,c.nombre_eng) as categoria_eng,if(c.nombre_ger='',c.nombre,c.nombre_ger) as categoria_ger,if(c.nombre_fr='',c.nombre,c.nombre_fr) as categoria_fr,c.tipo as idTipoCategoria,e.imagen,e.estado,e.precio,if(isnull(e.descripcion),'',e.descripcion) as descripcion,if(isnull(e.descripcion_eng),'',e.descripcion_eng) as descripcion_eng,if(isnull(e.descripcion_ger),'',e.descripcion_ger) as descripcion_ger,if(isnull(e.descripcion_fr),'',e.descripcion_fr) as descripcion_fr,if(isnull(o.opciones),'',o.opciones) as opciones,if(isnull(i.ingredientes),'',i.ingredientes) as ingredientes,if(isnull(a.alergenos),'',a.alergenos) as alergenos from qo_productos_cat c inner join qo_productos_est e on (e.idCategoria=c.id and e.eliminado=0) left join (select GROUP_CONCAT(o.id,';',o.opcion,';', o.tipoIncremento,';', o.valorIncremento,';', o.puntos SEPARATOR '|') as opciones,idProducto from qo_productos_opc o group by idProducto) o on (o.idProducto=e.id) left join (select GROUP_CONCAT(a.id,';',a.nombre,';', a.imagen SEPARATOR '|') as alergenos,idProducto from qo_productos_ing_aler i inner join qo_alergenos a on (i.idAlergeno=a.id) group by i.idProducto) a on (a.idProducto=e.id) left join (select GROUP_CONCAT(i.id,';',i.nombre SEPARATOR '|') as ingredientes,idProducto from qo_productos_ing i group by idProducto) i on (i.idProducto=e.id) WHERE  e.estado=0 and e.id in (:ids)");
      $sql->bindValue(':ids', $_GET['ids']);  
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
    if (isset($_GET['ingredienteProducto'])){
      $sql = "INSERT INTO `qo_ingredientes_producto`(puntos,idProducto,idIngrediente, precio) VALUES (:puntos,:idProducto,:idIngrediente,:precio)";
      $statement = $dbConn->prepare($sql);
      $statement->bindValue(':idProducto', $input['idProducto']);
      $statement->bindValue(':idIngrediente', $input['idIngrediente']);
      $statement->bindValue(':precio', $input['precio']);
      $statement->bindValue(':puntos', $input['puntos']);
      $statement->execute();
      $postId = $dbConn->lastInsertId();
      if($postId)
      {
        $input['id'] = $postId;
        header("HTTP/1.1 200 OK");
        echo json_encode($input);
        exit();
      }
    }else if (isset($_GET['ingrediente'])){
      $sql = "INSERT INTO `qo_ingredientes_establecimiento`(nombre,nombre_eng,nombre_fr,nombre_ger,precio, estado, idEstablecimiento) VALUES (:nombre,:nombre,:nombre,:nombre,:precio,:estado,:idEstablecimiento)";
      $statement = $dbConn->prepare($sql);
      $statement->bindValue(':nombre', $input['nombre']);
      $statement->bindValue(':estado', $input['estado']);
      $statement->bindValue(':idEstablecimiento', $input['idEstablecimiento']);
      $statement->bindValue(':precio', $input['precio']);
      $statement->execute();
      $postId = $dbConn->lastInsertId();
      if($postId)
      {
        $input['id'] = $postId;
        header("HTTP/1.1 200 OK");
        echo json_encode($input);
        exit();
      }
    }else if (isset($_GET['alergenos'])){
      $sql = "INSERT INTO `qo_productos_ing_aler`( `idProducto`, `idAlergeno`) VALUES (:idProducto,:idAlergeno)";
      $statement = $dbConn->prepare($sql);
      $statement->bindValue(':idProducto', $input['idProducto']);
      $statement->bindValue(':idAlergeno', $input['idAlergeno']);
      $statement->execute();
      $postId = $dbConn->lastInsertId();
      if($postId)
      {
        $input['id'] = $postId;
        header("HTTP/1.1 200 OK");
        echo json_encode($input);
        exit();
      }
    }else if (isset($_GET['opcion'])){
      $sql = "INSERT INTO `qo_productos_opc`( puntos,`idProducto`, `opcion`,opcion_fr,opcion_ger,opcion_eng,tipoIncremento,valorIncremento) VALUES (:puntos,:idProducto,:opcion,:opcion,:opcion,:opcion,0,:valorIncremento)";
      $statement = $dbConn->prepare($sql);
      $statement->bindValue(':idProducto', $input['idProducto']);
      $statement->bindValue(':opcion', $input['opcion']);
      $statement->bindValue(':valorIncremento', $input['valorIncremento']);
      $statement->bindValue(':puntos', $input['puntos']);
      $statement->execute();
      $postId = $dbConn->lastInsertId();
      if($postId)
      {
        $input['id'] = $postId;
        header("HTTP/1.1 200 OK");
        echo json_encode($input);
        exit();
      }
    }else {
      if ($input['nombre_eng']==null){
        $sql = "INSERT INTO `qo_productos_est`(puntos,porEncargo,fuerzaIngredientes,precioLocal,vistaLocal,vistaEnvios,numeroIngredientes,`nombre`,nombre_eng,nombre_fr,nombre_ger, `idCategoria`, `imagen`, `estado`, `precio`, `descripcion`,descripcion_eng,descripcion_fr,descripcion_ger) VALUES 
        (:puntos,:porEncargo,:fuerzaIngredientes,:precioLocal,:vistaLocal,:vistaEnvios,:numeroIngredientes,:nombre,:nombre,:nombre,:nombre,:idCategoria,:imagen,1,:precio,:descripcion,:descripcion,:descripcion,:descripcion)";
        $statement = $dbConn->prepare($sql);
        $statement->bindValue(':puntos', $input['puntos']);
        $statement->bindValue(':porEncargo', $input['porEncargo']);
        $statement->bindValue(':fuerzaIngredientes', $input['fuerzaIngredientes']);
        $statement->bindValue(':precioLocal', $input['precioLocal']);
        $statement->bindValue(':vistaLocal', $input['vistaLocal']);
        $statement->bindValue(':vistaEnvios', $input['vistaEnvios']);
        $statement->bindValue(':numeroIngredientes', $input['numeroIngredientes']);
        $statement->bindValue(':nombre', $input['nombre']);
        $statement->bindValue(':idCategoria', $input['idCategoria']);
        $statement->bindValue(':imagen', $input['imagen']);
        $statement->bindValue(':precio', $input['precio']);
        $statement->bindValue(':descripcion', $input['descripcion']);
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
        $sql = "INSERT INTO `qo_productos_est`(puntos,porEncargo,fuerzaIngredientes,precioLocal,vistaLocal,vistaEnvios,numeroIngredientes,`nombre`,nombre_eng,nombre_fr,nombre_ger, `idCategoria`, `imagen`, `estado`, `precio`, `descripcion`,descripcion_eng,descripcion_fr,descripcion_ger) VALUES 
          (:puntos,:porEncargo,:fuerzaIngredientes,:precioLocal,:vistaLocal,:vistaEnvios,:numeroIngredientes,:nombre,:nombre_eng,:nombre_fr,:nombre_ger,:idCategoria,:imagen,1,:precio,:descripcion,:descripcion_eng,:descripcion_fr,:descripcion_ger)";
        $statement = $dbConn->prepare($sql);
        $statement->bindValue(':puntos', $input['puntos']);
        $statement->bindValue(':porEncargo', $input['porEncargo']);
        $statement->bindValue(':fuerzaIngredientes', $input['fuerzaIngredientes']);
        $statement->bindValue(':precioLocal', $input['precioLocal']);
        $statement->bindValue(':vistaLocal', $input['vistaLocal']);
        $statement->bindValue(':vistaEnvios', 1);
        $statement->bindValue(':numeroIngredientes', $input['numeroIngredientes']);
        $statement->bindValue(':nombre', $input['nombre']);
        $statement->bindValue(':nombre_eng', $input['nombre_eng']);
        $statement->bindValue(':nombre_fr', $input['nombre_fr']);
        $statement->bindValue(':nombre_ger', $input['nombre_ger']);
        $statement->bindValue(':idCategoria', $input['idCategoria']);
        $statement->bindValue(':imagen', $input['imagen']);
        $statement->bindValue(':precio', $input['precio']);
        $statement->bindValue(':descripcion', $input['descripcion']);
        $statement->bindValue(':descripcion_eng', $input['descripcion']);
        $statement->bindValue(':descripcion_fr', $input['descripcion']);
        $statement->bindValue(':descripcion_ger', $input['descripcion']);
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
    //bindAllValues($statement, $input);
    
}

//Borrar
if ($_SERVER['REQUEST_METHOD'] == 'DELETE')
{
  $id = $_GET['idProducto'];
  
  $statement = $dbConn->prepare("DELETE FROM qo_productos_ing_aler where idProducto=:id");
  $statement->bindValue(':id', $id);
  $statement->execute();

  $statement = $dbConn->prepare("DELETE FROM qo_productos_opc where idProducto=:id");
  $statement->bindValue(':id', $id);
  $statement->execute();

  $statement = $dbConn->prepare("DELETE FROM qo_ingredientes_producto where idProducto=:id");
  $statement->bindValue(':id', $id);
  $statement->execute();
	header("HTTP/1.1 200 OK");
	exit();
}

//Actualizar
if ($_SERVER['REQUEST_METHOD'] == 'PUT')
{
    $input = json_decode(file_get_contents('php://input'), true);
    if (isset($_GET['ingrediente'])){
      $userId = $input['id'];
      $fields = getParams($input);

      $sql = "
            UPDATE qo_ingredientes_establecimiento
            SET $fields
            WHERE id='$userId'
            ";

      $statement = $dbConn->prepare($sql);
      bindAllValues($statement, $input);

      $statement->execute();
    }else if (isset($_GET['ingredienteProducto'])){
      $userId = $input['id'];
    
      $sql = "UPDATE `qo_ingredientes_producto` SET `precio`=:precio,puntos=:puntos WHERE id=$userId";
      
      $statement = $dbConn->prepare($sql);
      $statement->bindValue(':precio', $input['precio']);
      $statement->bindValue(':puntos', $input['puntos']);

      $statement->execute();
    }else{

      $userId = $input['idArticulo'];
    
      $sql = "UPDATE `qo_productos_est` SET puntos=:puntos,porEncargo=:porEncargo,fuerzaIngredientes=:fuerzaIngredientes,precioLocal=:precioLocal,vistaEnvios=:vistaEnvios,vistaLocal=:vistaLocal,numeroIngredientes=:numeroIngredientes,`nombre`=:nombre,`nombre_eng`=:nombre_eng,`nombre_fr`=:nombre_fr,`nombre_ger`=:nombre_ger,precio=:precio,
          imagen=:imagen,idCategoria=:idCategoria,estado=:estado,descripcion=:descripcion,descripcion_fr=:descripcion_fr
          ,descripcion_eng=:descripcion_eng,descripcion_ger=:descripcion_ger,eliminado=:eliminado WHERE id=$userId";
      
      $statement = $dbConn->prepare($sql);
      $statement->bindValue(':nombre', $input['nombre']);
      $statement->bindValue(':nombre_eng', $input['nombre_eng']);
      $statement->bindValue(':nombre_fr', $input['nombre_fr']);
      $statement->bindValue(':nombre_ger', $input['nombre_ger']);
      $statement->bindValue(':porEncargo', $input['porEncargo']);
      $statement->bindValue(':fuerzaIngredientes', $input['fuerzaIngredientes']);
      $statement->bindValue(':numeroIngredientes', $input['numeroIngredientes']);
      $statement->bindValue(':precio', $input['precio']);
      $statement->bindValue(':imagen', $input['imagen']);
      $statement->bindValue(':idCategoria', $input['idCategoria']);
      $statement->bindValue(':estado', $input['estado']);
      $statement->bindValue(':descripcion', $input['descripcion']);
      $statement->bindValue(':descripcion_eng', $input['descripcion_eng']);
      $statement->bindValue(':descripcion_fr', $input['descripcion_fr']);
      $statement->bindValue(':descripcion_ger', $input['descripcion_ger']);
      $statement->bindValue(':precioLocal', $input['precioLocal']);
      $statement->bindValue(':vistaLocal', $input['vistaLocal']);
      //$statement->bindValue(':vistaEnvios', $input['vistaEnvios']);
      $statement->bindValue(':vistaEnvios', 1);
      $statement->bindValue(':eliminado', $input['eliminado']);
      $statement->bindValue(':puntos', $input['puntos']);
      
      $statement->execute();
    }
    header("HTTP/1.1 200 OK");
    exit();
}


//En caso de que ninguna de las opciones anteriores se haya ejecutado
header("HTTP/1.1 400 Bad Request");

?>