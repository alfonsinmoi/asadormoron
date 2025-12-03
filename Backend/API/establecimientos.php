<?php
include "config.php";
include "utils.php";


$dbConn =  connect($db);
/*
  listar todos los posts o solo uno
 */
if ($_SERVER['REQUEST_METHOD'] == 'GET')
{
  if (isset($_GET['id']))
  {
      $sql = $dbConn->prepare("SELECT 0 as favorito,e.*,e.id as idEstablecimiento,if(ce.servicioActivo=1,'True','False') as servicioActivo,ce.pedidoMinimo,ce.tiempoEntrega,ce.numeroPedidosSoportado,
      if((WEEKDAY(now()) +1)=1 and hour(now())<17 and c.activoLunes=1 and ce.activoLunes=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioLunes>ce.inicioLunes,TIME_FORMAT(c.inicioLunes, '%H:%i'),TIME_FORMAT(ce.inicioLunes, '%H:%i')),' - ',if(c.finLunes<ce.finLunes,TIME_FORMAT(c.finLunes, '%H:%i'),TIME_FORMAT(ce.FinLunes, '%H:%i'))),if((WEEKDAY(now()) +1)=1 and hour(now())>=17 and c.activoLunesTarde=1 and ce.activoLunesTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioLunesTarde>ce.inicioLunesTarde,TIME_FORMAT(c.inicioLunesTarde, '%H:%i'),TIME_FORMAT(ce.inicioLunesTarde, '%H:%i')),' - ',if(c.finLunesTarde<ce.finLunesTarde,TIME_FORMAT(c.finLunesTarde, '%H:%i'),TIME_FORMAT(ce.FinLunesTarde, '%H:%i'))),if((WEEKDAY(now()) +1)=2 and hour(now())<17 and c.activoMartes=1 and ce.activoMartes=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioMartes>ce.inicioMartes,TIME_FORMAT(c.inicioMartes, '%H:%i'),TIME_FORMAT(ce.inicioMartes, '%H:%i')),' - ',if(c.finMartes<ce.finMartes,TIME_FORMAT(c.finMartes, '%H:%i'),TIME_FORMAT(ce.FinMartes, '%H:%i'))),if((WEEKDAY(now()) +1)=2 and hour(now())>=17 and c.activoMartesTarde=1 and ce.activoMartesTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioMartesTarde>ce.inicioMartesTarde,TIME_FORMAT(c.inicioMartesTarde, '%H:%i'),TIME_FORMAT(ce.inicioMartesTarde, '%H:%i')),' - ',if(c.finMartesTarde<ce.finMartesTarde,TIME_FORMAT(c.finMartesTarde, '%H:%i'),TIME_FORMAT(ce.FinMartesTarde, '%H:%i'))),if((WEEKDAY(now()) +1)=3 and hour(now())<17 and c.activoMiercoles=1 and ce.activoMiercoles=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioMiercoles>ce.inicioMiercoles,TIME_FORMAT(c.inicioMiercoles, '%H:%i'),TIME_FORMAT(ce.inicioMiercoles, '%H:%i')),' - ',if(c.finMiercoles<ce.finMiercoles,TIME_FORMAT(c.finMiercoles, '%H:%i'),TIME_FORMAT(ce.FinMiercoles, '%H:%i'))),if((WEEKDAY(now()) +1)=3 and hour(now())>=17 and c.activoMiercolesTarde=1 and ce.activoMiercolesTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioMiercolesTarde>ce.inicioMiercolesTarde,TIME_FORMAT(c.inicioMiercolesTarde, '%H:%i'),TIME_FORMAT(ce.inicioMiercolesTarde, '%H:%i')),' - ',if(c.finMiercolesTarde<ce.finMiercolesTarde,TIME_FORMAT(c.finMiercolesTarde, '%H:%i'),TIME_FORMAT(ce.FinMiercolesTarde, '%H:%i'))),if((WEEKDAY(now()) +1)=4 and hour(now())<17 and c.activoJueves=1 and ce.activoJueves=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioJueves>ce.inicioJueves,TIME_FORMAT(c.inicioJueves, '%H:%i'),TIME_FORMAT(ce.inicioJueves, '%H:%i')),' - ',if(c.finJueves<ce.finJueves,TIME_FORMAT(c.finJueves, '%H:%i'),TIME_FORMAT(ce.FinJueves, '%H:%i'))),if((WEEKDAY(now()) +1)=4 and hour(now())>=17 and c.activoJuevesTarde=1 and ce.activoJuevesTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioJuevesTarde>ce.inicioJuevesTarde,TIME_FORMAT(c.inicioJuevesTarde, '%H:%i'),TIME_FORMAT(ce.inicioJuevesTarde, '%H:%i')),' - ',if(c.finJuevesTarde<ce.finJuevesTarde,TIME_FORMAT(c.finJuevesTarde, '%H:%i'),TIME_FORMAT(ce.FinJuevesTarde, '%H:%i'))),if((WEEKDAY(now()) +1)=5 and hour(now())<17 and c.activoViernes=1 and ce.activoViernes=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioViernes>ce.inicioViernes,TIME_FORMAT(c.inicioViernes, '%H:%i'),TIME_FORMAT(ce.inicioViernes, '%H:%i')),' - ',if(c.finViernes<ce.finViernes,TIME_FORMAT(c.finViernes, '%H:%i'),TIME_FORMAT(ce.FinViernes, '%H:%i'))),if((WEEKDAY(now()) +1)=5 and hour(now())>=17 and c.activoViernesTarde=1 and ce.activoViernesTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioViernesTarde>ce.inicioViernesTarde,TIME_FORMAT(c.inicioViernesTarde, '%H:%i'),TIME_FORMAT(ce.inicioViernesTarde, '%H:%i')),' - ',if(c.finViernesTarde<ce.finViernesTarde,TIME_FORMAT(c.finViernesTarde, '%H:%i'),TIME_FORMAT(ce.FinViernesTarde, '%H:%i'))),if((WEEKDAY(now()) +1)=6 and hour(now())<17 and c.activoSabado=1 and ce.activoSabado=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioSabado>ce.inicioSabado,TIME_FORMAT(c.inicioSabado, '%H:%i'),TIME_FORMAT(ce.inicioSabado, '%H:%i')),' - ',if(c.finSabado<ce.finSabado,TIME_FORMAT(c.finSabado, '%H:%i'),TIME_FORMAT(ce.FinSabado, '%H:%i'))),if((WEEKDAY(now()) +1)=6 and hour(now())>=17 and c.activoSabadoTarde=1 and ce.activoSabadoTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioSabadoTarde>ce.inicioSabadoTarde,TIME_FORMAT(c.inicioSabadoTarde, '%H:%i'),TIME_FORMAT(ce.inicioSabadoTarde, '%H:%i')),' - ',if(c.finSabadoTarde<ce.finSabadoTarde,TIME_FORMAT(c.finSabadoTarde, '%H:%i'),TIME_FORMAT(ce.FinSabadoTarde, '%H:%i'))),if((WEEKDAY(now()) +1)=7 and hour(now())<17 and c.activoDomingo=1 and ce.activoDomingo=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioDomingo>ce.inicioDomingo,TIME_FORMAT(c.inicioDomingo, '%H:%i'),TIME_FORMAT(ce.inicioDomingo, '%H:%i')),' - ',if(c.finDomingo<ce.finDomingo,TIME_FORMAT(c.finDomingo, '%H:%i'),TIME_FORMAT(ce.FinDomingo, '%H:%i'))),if((WEEKDAY(now()) +1)=7 and hour(now())>=17 and c.activoDomingoTarde=1 and ce.activoDomingoTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioDomingoTarde<ce.inicioDomingoTarde,TIME_FORMAT(c.inicioDomingoTarde, '%H:%i'),TIME_FORMAT(ce.inicioDomingoTarde, '%H:%i')),' - ',if(c.finDomingoTarde<ce.finDomingoTarde,TIME_FORMAT(c.finDomingoTarde, '%H:%i'),TIME_FORMAT(ce.FinDomingoTarde, '%H:%i'))),'CERRADO')))))))))))))) as horario,if((WEEKDAY(now()) +1)=1 and hour(now())<17,if(c.inicioLunes>ce.inicioLunes,c.inicioLunes,ce.inicioLunes),if((WEEKDAY(now()) +1)=1 and hour(now())>=17,if(c.inicioLunesTarde>ce.inicioLunesTarde,c.inicioLunesTarde,ce.inicioLunesTarde),if((WEEKDAY(now()) +1)=2 and hour(now())<17,if(c.inicioMartes>ce.inicioMartes,c.inicioMartes,ce.inicioMartes),if((WEEKDAY(now()) +1)=2 and hour(now())>=17,if(c.inicioMartesTarde>ce.inicioMartesTarde,c.inicioMartesTarde,ce.inicioMartesTarde),if((WEEKDAY(now()) +1)=3 and hour(now())<17,if(c.inicioMiercoles>ce.inicioMiercoles,c.inicioMiercoles,ce.inicioMiercoles),if((WEEKDAY(now()) +1)=3 and hour(now())>=17,if(c.inicioMiercolesTarde>ce.inicioMiercolesTarde,c.inicioMiercolesTarde,ce.inicioMiercolesTarde),if((WEEKDAY(now()) +1)=4 and hour(now())<17,if(c.inicioJueves>ce.inicioJueves,c.inicioJueves,ce.inicioJueves),if((WEEKDAY(now()) +1)=4 and hour(now())>=17,if(c.inicioJuevesTarde>ce.inicioJuevesTarde,c.inicioJuevesTarde,ce.inicioJuevesTarde),if((WEEKDAY(now()) +1)=5 and hour(now())<17,if(c.inicioViernes>ce.inicioViernes,c.inicioViernes,ce.inicioViernes),if((WEEKDAY(now()) +1)=5 and hour(now())>=17,if(c.inicioViernesTarde>ce.inicioViernesTarde,c.inicioViernesTarde,ce.inicioViernesTarde),if((WEEKDAY(now()) +1)=6 and hour(now())<17,if(c.inicioSabado>ce.inicioSabado,c.inicioSabado,ce.inicioSabado),if((WEEKDAY(now()) +1)=6 and hour(now())>=17,if(c.inicioSabadoTarde>ce.inicioSabadoTarde,c.inicioSabadoTarde,ce.inicioSabadoTarde),if((WEEKDAY(now()) +1)=7 and hour(now())<17,if(c.inicioDomingo>ce.inicioDomingo,c.inicioDomingo,ce.inicioDomingo),if((WEEKDAY(now()) +1)=7 and hour(now())>=17,if(c.inicioDomingoTarde>ce.inicioDomingoTarde,c.inicioDomingoTarde,ce.inicioDomingoTarde),null)))))))))))))) as inicioHoy,if((WEEKDAY(now()) +1)=1 and hour(now())<17,if(c.finLunes<ce.finLunes,c.finLunes,ce.finLunes),if((WEEKDAY(now()) +1)=1 and hour(now())>=17,if(c.finLunesTarde<ce.finLunesTarde,c.finLunesTarde,ce.finLunesTarde),if((WEEKDAY(now()) +1)=2 and hour(now())<17,if(c.finMartes<ce.finMartes,c.finMartes,ce.finMartes),if((WEEKDAY(now()) +1)=2 and hour(now())>=17,if(c.finMartesTarde<ce.finMartesTarde,c.finMartesTarde,ce.finMartesTarde),if((WEEKDAY(now()) +1)=3 and hour(now())<17,if(c.finMiercoles<ce.finMiercoles,c.finMiercoles,ce.finMiercoles),if((WEEKDAY(now()) +1)=3 and hour(now())>=17,if(c.finMiercolesTarde<ce.finMiercolesTarde,c.finMiercolesTarde,ce.finMiercolesTarde),if((WEEKDAY(now()) +1)=4 and hour(now())<17,if(c.finJueves<ce.finJueves,c.finJueves,ce.finJueves),if((WEEKDAY(now()) +1)=4 and hour(now())>=17,if(c.finJuevesTarde<ce.finJuevesTarde,c.finJuevesTarde,ce.finJuevesTarde),if((WEEKDAY(now()) +1)=5 and hour(now())<17,if(c.finViernes<ce.finViernes,c.finViernes,ce.finViernes),if((WEEKDAY(now()) +1)=5 and hour(now())>=17,if(c.finViernesTarde<ce.finViernesTarde,c.finViernesTarde,ce.finViernesTarde),if((WEEKDAY(now()) +1)=6 and hour(now())<17,if(c.finSabado<ce.finSabado,c.finSabado,ce.finSabado),if((WEEKDAY(now()) +1)=6 and hour(now())>=17,if(c.finSabadoTarde<ce.finSabadoTarde,c.finSabadoTarde,ce.finSabadoTarde),if((WEEKDAY(now()) +1)=7 and hour(now())<17,if(c.finDomingo<ce.finDomingo,c.finDomingo,ce.finDomingo),if((WEEKDAY(now()) +1)=7 and hour(now())>=17,if(c.finDomingoTarde<ce.finDomingoTarde,c.finDomingoTarde,ce.finDomingoTarde),null)))))))))))))) as finHoy,
      if(c.servicioActivo=0,'False',if(ce.servicioActivo=0,'False',if((WEEKDAY(now()) +1)=1 and (ce.activoLunes=1 or ce.activoLunesTarde=1) and (c.activoLunes=1 or c.activoLunesTarde=1),'True',if((WEEKDAY(now()) +1)=2 and (ce.activoMartes=1 or ce.activoMartesTarde=1) and (ce.activoMartes=1 or ce.activoMartesTarde=1),'True',if((WEEKDAY(now()) +1)=3 and (ce.activoMiercoles=1 or ce.activoMiercolesTarde=1) and (c.activoMiercoles=1 or c.activoMiercolesTarde=1),
      'True',if((WEEKDAY(now()) +1)=4 and (ce.activoJueves=1 or ce.activoJuevesTarde=1)  and (c.activoJueves=1 or c.activoJuevesTarde=1),'True',if((WEEKDAY(now()) +1)=5 and (ce.activoViernes=1 or ce.activoViernesTarde=1) and (c.activoViernes=1 or c.activoViernesTarde=1),'True',if((WEEKDAY(now()) +1)=6 and (ce.activoSabado=1 or ce.activoSabadoTarde=1) and (c.activoSabado=1 or c.activoSabadoTarde=1),
      'True',if((WEEKDAY(now()) +1)=7 and (ce.activoDomingo=1 or ce.activoDomingoTarde=1) and (c.activoDomingo=1 or c.activoDomingoTarde=1),'True','False'))))))))) as activoHoy,
      if((WEEKDAY(now()) +1)=1,if(c.inicioLunes>ce.inicioLunes,c.inicioLunes,ce.inicioLunes),if((WEEKDAY(now()) +1)=2,if(c.inicioMartes>ce.inicioMartes,c.inicioMartes,ce.inicioMartes),if((WEEKDAY(now()) +1)=3,if(c.inicioMiercoles>ce.inicioMiercoles,c.inicioMiercoles,ce.inicioMiercoles),if((WEEKDAY(now()) +1)=4,if(c.inicioJueves>ce.inicioJueves,c.inicioJueves,ce.inicioJueves),if((WEEKDAY(now()) +1)=5,if(c.inicioViernes>ce.inicioViernes,c.inicioViernes,ce.inicioViernes),if((WEEKDAY(now()) +1)=6,if(c.inicioSabado>ce.inicioSabado,c.inicioSabado,ce.inicioSabado),if((WEEKDAY(now()) +1)=7,if(c.inicioDomingo>ce.inicioDomingo,c.inicioDomingo,ce.inicioDomingo),null))))))) as inicioMan,
      if((WEEKDAY(now()) +1)=1,if(c.finLunes<ce.finLunes,c.finLunes,ce.finLunes),if((WEEKDAY(now()) +1)=2,if(c.finMartes<ce.finMartes,c.finMartes,ce.finMartes),if((WEEKDAY(now()) +1)=3,if(c.finMiercoles<ce.finMiercoles,c.finMiercoles,ce.finMiercoles),if((WEEKDAY(now()) +1)=4,if(c.finJueves<ce.finJueves,c.finJueves,ce.finJueves),if((WEEKDAY(now()) +1)=5,if(c.finViernes<ce.finViernes,c.finViernes,ce.finViernes),if((WEEKDAY(now()) +1)=6,if(c.finSabado<ce.finSabado,c.finSabado,ce.finSabado),if((WEEKDAY(now()) +1)=7,if(c.finDomingo<ce.finDomingo,c.finDomingo,ce.finDomingo),null))))))) as finMan,
      if((WEEKDAY(now()) +1)=1,if(c.inicioLunesTarde>ce.inicioLunesTarde,c.inicioLunesTarde,ce.inicioLunesTarde),if((WEEKDAY(now()) +1)=2,if(c.inicioMartesTarde>ce.inicioMartesTarde,c.inicioMartesTarde,ce.inicioMartesTarde),if((WEEKDAY(now()) +1)=3,if(c.inicioMiercolesTarde>ce.inicioMiercolesTarde,c.inicioMiercolesTarde,ce.inicioMiercolesTarde),if((WEEKDAY(now()) +1)=4,if(c.inicioJuevesTarde>ce.inicioJuevesTarde,c.inicioJuevesTarde,ce.inicioJuevesTarde),if((WEEKDAY(now()) +1)=5,if(c.inicioViernesTarde>ce.inicioViernesTarde,c.inicioViernesTarde,ce.inicioViernesTarde),if((WEEKDAY(now()) +1)=6,if(c.inicioSabadoTarde>ce.inicioSabadoTarde,c.inicioSabadoTarde,ce.inicioSabadoTarde),if((WEEKDAY(now()) +1)=7,if(c.inicioDomingoTarde>ce.inicioDomingoTarde,c.inicioDomingoTarde,ce.inicioDomingoTarde),null))))))) as inicioTarde,
      if((WEEKDAY(now()) +1)=1,if(c.finLunesTarde<ce.finLunesTarde,c.finLunesTarde,ce.finLunesTarde),if((WEEKDAY(now()) +1)=2,if(c.finMartesTarde<ce.finMartesTarde,c.finMartesTarde,ce.finMartesTarde),if((WEEKDAY(now()) +1)=3,if(c.finMiercolesTarde<ce.finMiercolesTarde,c.finMiercolesTarde,ce.finMiercolesTarde),if((WEEKDAY(now()) +1)=4,if(c.finJuevesTarde<ce.finJuevesTarde,c.finJuevesTarde,ce.finJuevesTarde),if((WEEKDAY(now()) +1)=5,if(c.finViernesTarde<ce.finViernesTarde,c.finViernesTarde,ce.finViernesTarde),if((WEEKDAY(now()) +1)=6,if(c.finSabadoTarde<ce.finSabadoTarde,c.finSabadoTarde,ce.finSabadoTarde),if((WEEKDAY(now()) +1)=7,if(c.finDomingoTarde<ce.finDomingoTarde,c.finDomingoTarde,ce.finDomingoTarde),null))))))) as finTarde,
      if((WEEKDAY(now()) +1)=1 and c.activoLunes=1,ce.activoLunes,if((WEEKDAY(now()) +1)=2 and c.activoMartes=1,ce.activoMartes,if((WEEKDAY(now()) +1)=3 and c.activoMiercoles=1,ce.activoMiercoles,if((WEEKDAY(now()) +1)=4 and c.activoJueves=1,ce.activoJueves,if((WEEKDAY(now()) +1)=5 and c.activoViernes=1,ce.activoViernes,if((WEEKDAY(now()) +1)=6 and c.activoSabado=1,ce.activoSabado,if((WEEKDAY(now()) +1)=7 and c.activoDomingo=1,ce.activoDomingo,0))))))) as activoMan,
      if((WEEKDAY(now()) +1)=1 and c.activoLunesTarde=1,ce.activoLunesTarde,if((WEEKDAY(now()) +1)=2 and c.activoMartesTarde=1,ce.activoMartesTarde,if((WEEKDAY(now()) +1)=3 and c.activoMiercolesTarde=1,ce.activoMiercolesTarde,if((WEEKDAY(now()) +1)=4 and c.activoJuevesTarde=1,ce.activoJuevesTarde,if((WEEKDAY(now()) +1)=5 and c.activoViernesTarde=1,ce.activoViernesTarde ,if((WEEKDAY(now()) +1)=6 and c.activoSabadoTarde=1,ce.activoSabadoTarde,if((WEEKDAY(now()) +1)=7 and c.activoDomingoTarde=1,ce.activoDomingoTarde,0))))))) as activoTarde
      from qo_establecimientos e inner join qo_configuracion_est ce on (ce.idEstablecimiento=e.id) inner join qo_configuracion c on (c.idGrupo=e.idGrupo) Where e.id=:id order by favorito desc,orden");
      $sql->bindValue(':id', $_GET['id']); 
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll());
      exit();
  }else if (isset($_GET['idFiscal']))
  {
      $sql = $dbConn->prepare("SELECT * from qo_establecimientos_fiscal WHERE idEstablecimiento=:id");
      $sql->bindValue(':id', $_GET['idFiscal']); 
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll());
      exit();
  }else if (isset($_GET['idEstablecimientoFactura']))
  {
      $sql = $dbConn->prepare("SELECT * from qo_facturas f Where f.idEstablecimiento=:id order by numero");
      $sql->bindValue(':id', $_GET['idEstablecimientoFactura']); 
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll());
      exit();
  }else if (isset($_GET['popular']))
  {
    if ($_GET['idGrupo']=='0'){
      $sql = $dbConn->prepare("SELECT count(*) as pedidos,0 as favorito,e.*,e.id as idEstablecimiento,if(ce.servicioActivo=1,'True','False') as servicioActivo,ce.pedidoMinimo,ce.tiempoEntrega,ce.numeroPedidosSoportado,
      if((WEEKDAY(now()) +1)=1 and hour(now())<17 and c.activoLunes=1 and ce.activoLunes=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioLunes>ce.inicioLunes,TIME_FORMAT(c.inicioLunes, '%H:%i'),TIME_FORMAT(ce.inicioLunes, '%H:%i')),' - ',if(c.finLunes<ce.finLunes,TIME_FORMAT(c.finLunes, '%H:%i'),TIME_FORMAT(ce.FinLunes, '%H:%i'))),if((WEEKDAY(now()) +1)=1 and hour(now())>=17 and c.activoLunesTarde=1 and ce.activoLunesTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioLunesTarde>ce.inicioLunesTarde,TIME_FORMAT(c.inicioLunesTarde, '%H:%i'),TIME_FORMAT(ce.inicioLunesTarde, '%H:%i')),' - ',if(c.finLunesTarde<ce.finLunesTarde,TIME_FORMAT(c.finLunesTarde, '%H:%i'),TIME_FORMAT(ce.FinLunesTarde, '%H:%i'))),if((WEEKDAY(now()) +1)=2 and hour(now())<17 and c.activoMartes=1 and ce.activoMartes=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioMartes>ce.inicioMartes,TIME_FORMAT(c.inicioMartes, '%H:%i'),TIME_FORMAT(ce.inicioMartes, '%H:%i')),' - ',if(c.finMartes<ce.finMartes,TIME_FORMAT(c.finMartes, '%H:%i'),TIME_FORMAT(ce.FinMartes, '%H:%i'))),if((WEEKDAY(now()) +1)=2 and hour(now())>=17 and c.activoMartesTarde=1 and ce.activoMartesTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioMartesTarde>ce.inicioMartesTarde,TIME_FORMAT(c.inicioMartesTarde, '%H:%i'),TIME_FORMAT(ce.inicioMartesTarde, '%H:%i')),' - ',if(c.finMartesTarde<ce.finMartesTarde,TIME_FORMAT(c.finMartesTarde, '%H:%i'),TIME_FORMAT(ce.FinMartesTarde, '%H:%i'))),if((WEEKDAY(now()) +1)=3 and hour(now())<17 and c.activoMiercoles=1 and ce.activoMiercoles=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioMiercoles>ce.inicioMiercoles,TIME_FORMAT(c.inicioMiercoles, '%H:%i'),TIME_FORMAT(ce.inicioMiercoles, '%H:%i')),' - ',if(c.finMiercoles<ce.finMiercoles,TIME_FORMAT(c.finMiercoles, '%H:%i'),TIME_FORMAT(ce.FinMiercoles, '%H:%i'))),if((WEEKDAY(now()) +1)=3 and hour(now())>=17 and c.activoMiercolesTarde=1 and ce.activoMiercolesTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioMiercolesTarde>ce.inicioMiercolesTarde,TIME_FORMAT(c.inicioMiercolesTarde, '%H:%i'),TIME_FORMAT(ce.inicioMiercolesTarde, '%H:%i')),' - ',if(c.finMiercolesTarde<ce.finMiercolesTarde,TIME_FORMAT(c.finMiercolesTarde, '%H:%i'),TIME_FORMAT(ce.FinMiercolesTarde, '%H:%i'))),if((WEEKDAY(now()) +1)=4 and hour(now())<17 and c.activoJueves=1 and ce.activoJueves=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioJueves>ce.inicioJueves,TIME_FORMAT(c.inicioJueves, '%H:%i'),TIME_FORMAT(ce.inicioJueves, '%H:%i')),' - ',if(c.finJueves<ce.finJueves,TIME_FORMAT(c.finJueves, '%H:%i'),TIME_FORMAT(ce.FinJueves, '%H:%i'))),if((WEEKDAY(now()) +1)=4 and hour(now())>=17 and c.activoJuevesTarde=1 and ce.activoJuevesTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioJuevesTarde>ce.inicioJuevesTarde,TIME_FORMAT(c.inicioJuevesTarde, '%H:%i'),TIME_FORMAT(ce.inicioJuevesTarde, '%H:%i')),' - ',if(c.finJuevesTarde<ce.finJuevesTarde,TIME_FORMAT(c.finJuevesTarde, '%H:%i'),TIME_FORMAT(ce.FinJuevesTarde, '%H:%i'))),if((WEEKDAY(now()) +1)=5 and hour(now())<17 and c.activoViernes=1 and ce.activoViernes=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioViernes>ce.inicioViernes,TIME_FORMAT(c.inicioViernes, '%H:%i'),TIME_FORMAT(ce.inicioViernes, '%H:%i')),' - ',if(c.finViernes<ce.finViernes,TIME_FORMAT(c.finViernes, '%H:%i'),TIME_FORMAT(ce.FinViernes, '%H:%i'))),if((WEEKDAY(now()) +1)=5 and hour(now())>=17 and c.activoViernesTarde=1 and ce.activoViernesTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioViernesTarde>ce.inicioViernesTarde,TIME_FORMAT(c.inicioViernesTarde, '%H:%i'),TIME_FORMAT(ce.inicioViernesTarde, '%H:%i')),' - ',if(c.finViernesTarde<ce.finViernesTarde,TIME_FORMAT(c.finViernesTarde, '%H:%i'),TIME_FORMAT(ce.FinViernesTarde, '%H:%i'))),if((WEEKDAY(now()) +1)=6 and hour(now())<17 and c.activoSabado=1 and ce.activoSabado=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioSabado>ce.inicioSabado,TIME_FORMAT(c.inicioSabado, '%H:%i'),TIME_FORMAT(ce.inicioSabado, '%H:%i')),' - ',if(c.finSabado<ce.finSabado,TIME_FORMAT(c.finSabado, '%H:%i'),TIME_FORMAT(ce.FinSabado, '%H:%i'))),if((WEEKDAY(now()) +1)=6 and hour(now())>=17 and c.activoSabadoTarde=1 and ce.activoSabadoTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioSabadoTarde>ce.inicioSabadoTarde,TIME_FORMAT(c.inicioSabadoTarde, '%H:%i'),TIME_FORMAT(ce.inicioSabadoTarde, '%H:%i')),' - ',if(c.finSabadoTarde<ce.finSabadoTarde,TIME_FORMAT(c.finSabadoTarde, '%H:%i'),TIME_FORMAT(ce.FinSabadoTarde, '%H:%i'))),if((WEEKDAY(now()) +1)=7 and hour(now())<17 and c.activoDomingo=1 and ce.activoDomingo=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioDomingo>ce.inicioDomingo,TIME_FORMAT(c.inicioDomingo, '%H:%i'),TIME_FORMAT(ce.inicioDomingo, '%H:%i')),' - ',if(c.finDomingo<ce.finDomingo,TIME_FORMAT(c.finDomingo, '%H:%i'),TIME_FORMAT(ce.FinDomingo, '%H:%i'))),if((WEEKDAY(now()) +1)=7 and hour(now())>=17 and c.activoDomingoTarde=1 and ce.activoDomingoTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioDomingoTarde<ce.inicioDomingoTarde,TIME_FORMAT(c.inicioDomingoTarde, '%H:%i'),TIME_FORMAT(ce.inicioDomingoTarde, '%H:%i')),' - ',if(c.finDomingoTarde<ce.finDomingoTarde,TIME_FORMAT(c.finDomingoTarde, '%H:%i'),TIME_FORMAT(ce.FinDomingoTarde, '%H:%i'))),'CERRADO')))))))))))))) as horario,if((WEEKDAY(now()) +1)=1 and hour(now())<17,if(c.inicioLunes>ce.inicioLunes,c.inicioLunes,ce.inicioLunes),if((WEEKDAY(now()) +1)=1 and hour(now())>=17,if(c.inicioLunesTarde>ce.inicioLunesTarde,c.inicioLunesTarde,ce.inicioLunesTarde),if((WEEKDAY(now()) +1)=2 and hour(now())<17,if(c.inicioMartes>ce.inicioMartes,c.inicioMartes,ce.inicioMartes),if((WEEKDAY(now()) +1)=2 and hour(now())>=17,if(c.inicioMartesTarde>ce.inicioMartesTarde,c.inicioMartesTarde,ce.inicioMartesTarde),if((WEEKDAY(now()) +1)=3 and hour(now())<17,if(c.inicioMiercoles>ce.inicioMiercoles,c.inicioMiercoles,ce.inicioMiercoles),if((WEEKDAY(now()) +1)=3 and hour(now())>=17,if(c.inicioMiercolesTarde>ce.inicioMiercolesTarde,c.inicioMiercolesTarde,ce.inicioMiercolesTarde),if((WEEKDAY(now()) +1)=4 and hour(now())<17,if(c.inicioJueves>ce.inicioJueves,c.inicioJueves,ce.inicioJueves),if((WEEKDAY(now()) +1)=4 and hour(now())>=17,if(c.inicioJuevesTarde>ce.inicioJuevesTarde,c.inicioJuevesTarde,ce.inicioJuevesTarde),if((WEEKDAY(now()) +1)=5 and hour(now())<17,if(c.inicioViernes>ce.inicioViernes,c.inicioViernes,ce.inicioViernes),if((WEEKDAY(now()) +1)=5 and hour(now())>=17,if(c.inicioViernesTarde>ce.inicioViernesTarde,c.inicioViernesTarde,ce.inicioViernesTarde),if((WEEKDAY(now()) +1)=6 and hour(now())<17,if(c.inicioSabado>ce.inicioSabado,c.inicioSabado,ce.inicioSabado),if((WEEKDAY(now()) +1)=6 and hour(now())>=17,if(c.inicioSabadoTarde>ce.inicioSabadoTarde,c.inicioSabadoTarde,ce.inicioSabadoTarde),if((WEEKDAY(now()) +1)=7 and hour(now())<17,if(c.inicioDomingo>ce.inicioDomingo,c.inicioDomingo,ce.inicioDomingo),if((WEEKDAY(now()) +1)=7 and hour(now())>=17,if(c.inicioDomingoTarde>ce.inicioDomingoTarde,c.inicioDomingoTarde,ce.inicioDomingoTarde),null)))))))))))))) as inicioHoy,if((WEEKDAY(now()) +1)=1 and hour(now())<17,if(c.finLunes<ce.finLunes,c.finLunes,ce.finLunes),if((WEEKDAY(now()) +1)=1 and hour(now())>=17,if(c.finLunesTarde<ce.finLunesTarde,c.finLunesTarde,ce.finLunesTarde),if((WEEKDAY(now()) +1)=2 and hour(now())<17,if(c.finMartes<ce.finMartes,c.finMartes,ce.finMartes),if((WEEKDAY(now()) +1)=2 and hour(now())>=17,if(c.finMartesTarde<ce.finMartesTarde,c.finMartesTarde,ce.finMartesTarde),if((WEEKDAY(now()) +1)=3 and hour(now())<17,if(c.finMiercoles<ce.finMiercoles,c.finMiercoles,ce.finMiercoles),if((WEEKDAY(now()) +1)=3 and hour(now())>=17,if(c.finMiercolesTarde<ce.finMiercolesTarde,c.finMiercolesTarde,ce.finMiercolesTarde),if((WEEKDAY(now()) +1)=4 and hour(now())<17,if(c.finJueves<ce.finJueves,c.finJueves,ce.finJueves),if((WEEKDAY(now()) +1)=4 and hour(now())>=17,if(c.finJuevesTarde<ce.finJuevesTarde,c.finJuevesTarde,ce.finJuevesTarde),if((WEEKDAY(now()) +1)=5 and hour(now())<17,if(c.finViernes<ce.finViernes,c.finViernes,ce.finViernes),if((WEEKDAY(now()) +1)=5 and hour(now())>=17,if(c.finViernesTarde<ce.finViernesTarde,c.finViernesTarde,ce.finViernesTarde),if((WEEKDAY(now()) +1)=6 and hour(now())<17,if(c.finSabado<ce.finSabado,c.finSabado,ce.finSabado),if((WEEKDAY(now()) +1)=6 and hour(now())>=17,if(c.finSabadoTarde<ce.finSabadoTarde,c.finSabadoTarde,ce.finSabadoTarde),if((WEEKDAY(now()) +1)=7 and hour(now())<17,if(c.finDomingo<ce.finDomingo,c.finDomingo,ce.finDomingo),if((WEEKDAY(now()) +1)=7 and hour(now())>=17,if(c.finDomingoTarde<ce.finDomingoTarde,c.finDomingoTarde,ce.finDomingoTarde),null)))))))))))))) as finHoy,
      if(c.servicioActivo=0,'False',if(ce.servicioActivo=0,'False',if((WEEKDAY(now()) +1)=1 and (ce.activoLunes=1 or ce.activoLunesTarde=1) and (c.activoLunes=1 or c.activoLunesTarde=1),'True',if((WEEKDAY(now()) +1)=2 and (ce.activoMartes=1 or ce.activoMartesTarde=1) and (ce.activoMartes=1 or ce.activoMartesTarde=1),'True',if((WEEKDAY(now()) +1)=3 and (ce.activoMiercoles=1 or ce.activoMiercolesTarde=1) and (c.activoMiercoles=1 or c.activoMiercolesTarde=1),
      'True',if((WEEKDAY(now()) +1)=4 and (ce.activoJueves=1 or ce.activoJuevesTarde=1)  and (c.activoJueves=1 or c.activoJuevesTarde=1),'True',if((WEEKDAY(now()) +1)=5 and (ce.activoViernes=1 or ce.activoViernesTarde=1) and (c.activoViernes=1 or c.activoViernesTarde=1),'True',if((WEEKDAY(now()) +1)=6 and (ce.activoSabado=1 or ce.activoSabadoTarde=1) and (c.activoSabado=1 or c.activoSabadoTarde=1),
      'True',if((WEEKDAY(now()) +1)=7 and (ce.activoDomingo=1 or ce.activoDomingoTarde=1) and (c.activoDomingo=1 or c.activoDomingoTarde=1),'True','False'))))))))) as activoHoy,
      if((WEEKDAY(now()) +1)=1,if(c.inicioLunes>ce.inicioLunes,c.inicioLunes,ce.inicioLunes),if((WEEKDAY(now()) +1)=2,if(c.inicioMartes>ce.inicioMartes,c.inicioMartes,ce.inicioMartes),if((WEEKDAY(now()) +1)=3,if(c.inicioMiercoles>ce.inicioMiercoles,c.inicioMiercoles,ce.inicioMiercoles),if((WEEKDAY(now()) +1)=4,if(c.inicioJueves>ce.inicioJueves,c.inicioJueves,ce.inicioJueves),if((WEEKDAY(now()) +1)=5,if(c.inicioViernes>ce.inicioViernes,c.inicioViernes,ce.inicioViernes),if((WEEKDAY(now()) +1)=6,if(c.inicioSabado>ce.inicioSabado,c.inicioSabado,ce.inicioSabado),if((WEEKDAY(now()) +1)=7,if(c.inicioDomingo>ce.inicioDomingo,c.inicioDomingo,ce.inicioDomingo),null))))))) as inicioMan,
      if((WEEKDAY(now()) +1)=1,if(c.finLunes<ce.finLunes,c.finLunes,ce.finLunes),if((WEEKDAY(now()) +1)=2,if(c.finMartes<ce.finMartes,c.finMartes,ce.finMartes),if((WEEKDAY(now()) +1)=3,if(c.finMiercoles<ce.finMiercoles,c.finMiercoles,ce.finMiercoles),if((WEEKDAY(now()) +1)=4,if(c.finJueves<ce.finJueves,c.finJueves,ce.finJueves),if((WEEKDAY(now()) +1)=5,if(c.finViernes<ce.finViernes,c.finViernes,ce.finViernes),if((WEEKDAY(now()) +1)=6,if(c.finSabado<ce.finSabado,c.finSabado,ce.finSabado),if((WEEKDAY(now()) +1)=7,if(c.finDomingo<ce.finDomingo,c.finDomingo,ce.finDomingo),null))))))) as finMan,
      if((WEEKDAY(now()) +1)=1,if(c.inicioLunesTarde>ce.inicioLunesTarde,c.inicioLunesTarde,ce.inicioLunesTarde),if((WEEKDAY(now()) +1)=2,if(c.inicioMartesTarde>ce.inicioMartesTarde,c.inicioMartesTarde,ce.inicioMartesTarde),if((WEEKDAY(now()) +1)=3,if(c.inicioMiercolesTarde>ce.inicioMiercolesTarde,c.inicioMiercolesTarde,ce.inicioMiercolesTarde),if((WEEKDAY(now()) +1)=4,if(c.inicioJuevesTarde>ce.inicioJuevesTarde,c.inicioJuevesTarde,ce.inicioJuevesTarde),if((WEEKDAY(now()) +1)=5,if(c.inicioViernesTarde>ce.inicioViernesTarde,c.inicioViernesTarde,ce.inicioViernesTarde),if((WEEKDAY(now()) +1)=6,if(c.inicioSabadoTarde>ce.inicioSabadoTarde,c.inicioSabadoTarde,ce.inicioSabadoTarde),if((WEEKDAY(now()) +1)=7,if(c.inicioDomingoTarde>ce.inicioDomingoTarde,c.inicioDomingoTarde,ce.inicioDomingoTarde),null))))))) as inicioTarde,
      if((WEEKDAY(now()) +1)=1,if(c.finLunesTarde<ce.finLunesTarde,c.finLunesTarde,ce.finLunesTarde),if((WEEKDAY(now()) +1)=2,if(c.finMartesTarde<ce.finMartesTarde,c.finMartesTarde,ce.finMartesTarde),if((WEEKDAY(now()) +1)=3,if(c.finMiercolesTarde<ce.finMiercolesTarde,c.finMiercolesTarde,ce.finMiercolesTarde),if((WEEKDAY(now()) +1)=4,if(c.finJuevesTarde<ce.finJuevesTarde,c.finJuevesTarde,ce.finJuevesTarde),if((WEEKDAY(now()) +1)=5,if(c.finViernesTarde<ce.finViernesTarde,c.finViernesTarde,ce.finViernesTarde),if((WEEKDAY(now()) +1)=6,if(c.finSabadoTarde<ce.finSabadoTarde,c.finSabadoTarde,ce.finSabadoTarde),if((WEEKDAY(now()) +1)=7,if(c.finDomingoTarde<ce.finDomingoTarde,c.finDomingoTarde,ce.finDomingoTarde),null))))))) as finTarde,
      if((WEEKDAY(now()) +1)=1 and c.activoLunes=1,ce.activoLunes,if((WEEKDAY(now()) +1)=2 and c.activoMartes=1,ce.activoMartes,if((WEEKDAY(now()) +1)=3 and c.activoMiercoles=1,ce.activoMiercoles,if((WEEKDAY(now()) +1)=4 and c.activoJueves=1,ce.activoJueves,if((WEEKDAY(now()) +1)=5 and c.activoViernes=1,ce.activoViernes,if((WEEKDAY(now()) +1)=6 and c.activoSabado=1,ce.activoSabado,if((WEEKDAY(now()) +1)=7 and c.activoDomingo=1,ce.activoDomingo,0))))))) as activoMan,
      if((WEEKDAY(now()) +1)=1 and c.activoLunesTarde=1,ce.activoLunesTarde,if((WEEKDAY(now()) +1)=2 and c.activoMartesTarde=1,ce.activoMartesTarde,if((WEEKDAY(now()) +1)=3 and c.activoMiercolesTarde=1,ce.activoMiercolesTarde,if((WEEKDAY(now()) +1)=4 and c.activoJuevesTarde=1,ce.activoJuevesTarde,if((WEEKDAY(now()) +1)=5 and c.activoViernesTarde=1,ce.activoViernesTarde ,if((WEEKDAY(now()) +1)=6 and c.activoSabadoTarde=1,ce.activoSabadoTarde,if((WEEKDAY(now()) +1)=7 and c.activoDomingoTarde=1,ce.activoDomingoTarde,0))))))) as activoTarde
      from qo_establecimientos e 
      inner join qo_configuracion_est ce on (ce.idEstablecimiento=e.id) 
      inner join qo_configuracion c on (c.idGrupo=e.idGrupo) 
      inner join qo_pedidos p on (p.idEstablecimiento=e.id) 
      inner join qo_users u on (u.id=p.idUsuario) 
      WHERE e.estado=1 GROUP BY p.idEstablecimiento ORDER BY `pedidos` DESC limit 10");
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
    }else{
      $sql = $dbConn->prepare("SELECT count(*) as pedidos,0 as favorito,e.*,e.id as idEstablecimiento,if(ce.servicioActivo=1,'True','False') as servicioActivo,ce.pedidoMinimo,ce.tiempoEntrega,ce.numeroPedidosSoportado,
      if((WEEKDAY(now()) +1)=1 and hour(now())<17 and c.activoLunes=1 and ce.activoLunes=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioLunes>ce.inicioLunes,TIME_FORMAT(c.inicioLunes, '%H:%i'),TIME_FORMAT(ce.inicioLunes, '%H:%i')),' - ',if(c.finLunes<ce.finLunes,TIME_FORMAT(c.finLunes, '%H:%i'),TIME_FORMAT(ce.FinLunes, '%H:%i'))),if((WEEKDAY(now()) +1)=1 and hour(now())>=17 and c.activoLunesTarde=1 and ce.activoLunesTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioLunesTarde>ce.inicioLunesTarde,TIME_FORMAT(c.inicioLunesTarde, '%H:%i'),TIME_FORMAT(ce.inicioLunesTarde, '%H:%i')),' - ',if(c.finLunesTarde<ce.finLunesTarde,TIME_FORMAT(c.finLunesTarde, '%H:%i'),TIME_FORMAT(ce.FinLunesTarde, '%H:%i'))),if((WEEKDAY(now()) +1)=2 and hour(now())<17 and c.activoMartes=1 and ce.activoMartes=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioMartes>ce.inicioMartes,TIME_FORMAT(c.inicioMartes, '%H:%i'),TIME_FORMAT(ce.inicioMartes, '%H:%i')),' - ',if(c.finMartes<ce.finMartes,TIME_FORMAT(c.finMartes, '%H:%i'),TIME_FORMAT(ce.FinMartes, '%H:%i'))),if((WEEKDAY(now()) +1)=2 and hour(now())>=17 and c.activoMartesTarde=1 and ce.activoMartesTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioMartesTarde>ce.inicioMartesTarde,TIME_FORMAT(c.inicioMartesTarde, '%H:%i'),TIME_FORMAT(ce.inicioMartesTarde, '%H:%i')),' - ',if(c.finMartesTarde<ce.finMartesTarde,TIME_FORMAT(c.finMartesTarde, '%H:%i'),TIME_FORMAT(ce.FinMartesTarde, '%H:%i'))),if((WEEKDAY(now()) +1)=3 and hour(now())<17 and c.activoMiercoles=1 and ce.activoMiercoles=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioMiercoles>ce.inicioMiercoles,TIME_FORMAT(c.inicioMiercoles, '%H:%i'),TIME_FORMAT(ce.inicioMiercoles, '%H:%i')),' - ',if(c.finMiercoles<ce.finMiercoles,TIME_FORMAT(c.finMiercoles, '%H:%i'),TIME_FORMAT(ce.FinMiercoles, '%H:%i'))),if((WEEKDAY(now()) +1)=3 and hour(now())>=17 and c.activoMiercolesTarde=1 and ce.activoMiercolesTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioMiercolesTarde>ce.inicioMiercolesTarde,TIME_FORMAT(c.inicioMiercolesTarde, '%H:%i'),TIME_FORMAT(ce.inicioMiercolesTarde, '%H:%i')),' - ',if(c.finMiercolesTarde<ce.finMiercolesTarde,TIME_FORMAT(c.finMiercolesTarde, '%H:%i'),TIME_FORMAT(ce.FinMiercolesTarde, '%H:%i'))),if((WEEKDAY(now()) +1)=4 and hour(now())<17 and c.activoJueves=1 and ce.activoJueves=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioJueves>ce.inicioJueves,TIME_FORMAT(c.inicioJueves, '%H:%i'),TIME_FORMAT(ce.inicioJueves, '%H:%i')),' - ',if(c.finJueves<ce.finJueves,TIME_FORMAT(c.finJueves, '%H:%i'),TIME_FORMAT(ce.FinJueves, '%H:%i'))),if((WEEKDAY(now()) +1)=4 and hour(now())>=17 and c.activoJuevesTarde=1 and ce.activoJuevesTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioJuevesTarde>ce.inicioJuevesTarde,TIME_FORMAT(c.inicioJuevesTarde, '%H:%i'),TIME_FORMAT(ce.inicioJuevesTarde, '%H:%i')),' - ',if(c.finJuevesTarde<ce.finJuevesTarde,TIME_FORMAT(c.finJuevesTarde, '%H:%i'),TIME_FORMAT(ce.FinJuevesTarde, '%H:%i'))),if((WEEKDAY(now()) +1)=5 and hour(now())<17 and c.activoViernes=1 and ce.activoViernes=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioViernes>ce.inicioViernes,TIME_FORMAT(c.inicioViernes, '%H:%i'),TIME_FORMAT(ce.inicioViernes, '%H:%i')),' - ',if(c.finViernes<ce.finViernes,TIME_FORMAT(c.finViernes, '%H:%i'),TIME_FORMAT(ce.FinViernes, '%H:%i'))),if((WEEKDAY(now()) +1)=5 and hour(now())>=17 and c.activoViernesTarde=1 and ce.activoViernesTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioViernesTarde>ce.inicioViernesTarde,TIME_FORMAT(c.inicioViernesTarde, '%H:%i'),TIME_FORMAT(ce.inicioViernesTarde, '%H:%i')),' - ',if(c.finViernesTarde<ce.finViernesTarde,TIME_FORMAT(c.finViernesTarde, '%H:%i'),TIME_FORMAT(ce.FinViernesTarde, '%H:%i'))),if((WEEKDAY(now()) +1)=6 and hour(now())<17 and c.activoSabado=1 and ce.activoSabado=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioSabado>ce.inicioSabado,TIME_FORMAT(c.inicioSabado, '%H:%i'),TIME_FORMAT(ce.inicioSabado, '%H:%i')),' - ',if(c.finSabado<ce.finSabado,TIME_FORMAT(c.finSabado, '%H:%i'),TIME_FORMAT(ce.FinSabado, '%H:%i'))),if((WEEKDAY(now()) +1)=6 and hour(now())>=17 and c.activoSabadoTarde=1 and ce.activoSabadoTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioSabadoTarde>ce.inicioSabadoTarde,TIME_FORMAT(c.inicioSabadoTarde, '%H:%i'),TIME_FORMAT(ce.inicioSabadoTarde, '%H:%i')),' - ',if(c.finSabadoTarde<ce.finSabadoTarde,TIME_FORMAT(c.finSabadoTarde, '%H:%i'),TIME_FORMAT(ce.FinSabadoTarde, '%H:%i'))),if((WEEKDAY(now()) +1)=7 and hour(now())<17 and c.activoDomingo=1 and ce.activoDomingo=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioDomingo>ce.inicioDomingo,TIME_FORMAT(c.inicioDomingo, '%H:%i'),TIME_FORMAT(ce.inicioDomingo, '%H:%i')),' - ',if(c.finDomingo<ce.finDomingo,TIME_FORMAT(c.finDomingo, '%H:%i'),TIME_FORMAT(ce.FinDomingo, '%H:%i'))),if((WEEKDAY(now()) +1)=7 and hour(now())>=17 and c.activoDomingoTarde=1 and ce.activoDomingoTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioDomingoTarde<ce.inicioDomingoTarde,TIME_FORMAT(c.inicioDomingoTarde, '%H:%i'),TIME_FORMAT(ce.inicioDomingoTarde, '%H:%i')),' - ',if(c.finDomingoTarde<ce.finDomingoTarde,TIME_FORMAT(c.finDomingoTarde, '%H:%i'),TIME_FORMAT(ce.FinDomingoTarde, '%H:%i'))),'CERRADO')))))))))))))) as horario,if((WEEKDAY(now()) +1)=1 and hour(now())<17,if(c.inicioLunes>ce.inicioLunes,c.inicioLunes,ce.inicioLunes),if((WEEKDAY(now()) +1)=1 and hour(now())>=17,if(c.inicioLunesTarde>ce.inicioLunesTarde,c.inicioLunesTarde,ce.inicioLunesTarde),if((WEEKDAY(now()) +1)=2 and hour(now())<17,if(c.inicioMartes>ce.inicioMartes,c.inicioMartes,ce.inicioMartes),if((WEEKDAY(now()) +1)=2 and hour(now())>=17,if(c.inicioMartesTarde>ce.inicioMartesTarde,c.inicioMartesTarde,ce.inicioMartesTarde),if((WEEKDAY(now()) +1)=3 and hour(now())<17,if(c.inicioMiercoles>ce.inicioMiercoles,c.inicioMiercoles,ce.inicioMiercoles),if((WEEKDAY(now()) +1)=3 and hour(now())>=17,if(c.inicioMiercolesTarde>ce.inicioMiercolesTarde,c.inicioMiercolesTarde,ce.inicioMiercolesTarde),if((WEEKDAY(now()) +1)=4 and hour(now())<17,if(c.inicioJueves>ce.inicioJueves,c.inicioJueves,ce.inicioJueves),if((WEEKDAY(now()) +1)=4 and hour(now())>=17,if(c.inicioJuevesTarde>ce.inicioJuevesTarde,c.inicioJuevesTarde,ce.inicioJuevesTarde),if((WEEKDAY(now()) +1)=5 and hour(now())<17,if(c.inicioViernes>ce.inicioViernes,c.inicioViernes,ce.inicioViernes),if((WEEKDAY(now()) +1)=5 and hour(now())>=17,if(c.inicioViernesTarde>ce.inicioViernesTarde,c.inicioViernesTarde,ce.inicioViernesTarde),if((WEEKDAY(now()) +1)=6 and hour(now())<17,if(c.inicioSabado>ce.inicioSabado,c.inicioSabado,ce.inicioSabado),if((WEEKDAY(now()) +1)=6 and hour(now())>=17,if(c.inicioSabadoTarde>ce.inicioSabadoTarde,c.inicioSabadoTarde,ce.inicioSabadoTarde),if((WEEKDAY(now()) +1)=7 and hour(now())<17,if(c.inicioDomingo>ce.inicioDomingo,c.inicioDomingo,ce.inicioDomingo),if((WEEKDAY(now()) +1)=7 and hour(now())>=17,if(c.inicioDomingoTarde>ce.inicioDomingoTarde,c.inicioDomingoTarde,ce.inicioDomingoTarde),null)))))))))))))) as inicioHoy,if((WEEKDAY(now()) +1)=1 and hour(now())<17,if(c.finLunes<ce.finLunes,c.finLunes,ce.finLunes),if((WEEKDAY(now()) +1)=1 and hour(now())>=17,if(c.finLunesTarde<ce.finLunesTarde,c.finLunesTarde,ce.finLunesTarde),if((WEEKDAY(now()) +1)=2 and hour(now())<17,if(c.finMartes<ce.finMartes,c.finMartes,ce.finMartes),if((WEEKDAY(now()) +1)=2 and hour(now())>=17,if(c.finMartesTarde<ce.finMartesTarde,c.finMartesTarde,ce.finMartesTarde),if((WEEKDAY(now()) +1)=3 and hour(now())<17,if(c.finMiercoles<ce.finMiercoles,c.finMiercoles,ce.finMiercoles),if((WEEKDAY(now()) +1)=3 and hour(now())>=17,if(c.finMiercolesTarde<ce.finMiercolesTarde,c.finMiercolesTarde,ce.finMiercolesTarde),if((WEEKDAY(now()) +1)=4 and hour(now())<17,if(c.finJueves<ce.finJueves,c.finJueves,ce.finJueves),if((WEEKDAY(now()) +1)=4 and hour(now())>=17,if(c.finJuevesTarde<ce.finJuevesTarde,c.finJuevesTarde,ce.finJuevesTarde),if((WEEKDAY(now()) +1)=5 and hour(now())<17,if(c.finViernes<ce.finViernes,c.finViernes,ce.finViernes),if((WEEKDAY(now()) +1)=5 and hour(now())>=17,if(c.finViernesTarde<ce.finViernesTarde,c.finViernesTarde,ce.finViernesTarde),if((WEEKDAY(now()) +1)=6 and hour(now())<17,if(c.finSabado<ce.finSabado,c.finSabado,ce.finSabado),if((WEEKDAY(now()) +1)=6 and hour(now())>=17,if(c.finSabadoTarde<ce.finSabadoTarde,c.finSabadoTarde,ce.finSabadoTarde),if((WEEKDAY(now()) +1)=7 and hour(now())<17,if(c.finDomingo<ce.finDomingo,c.finDomingo,ce.finDomingo),if((WEEKDAY(now()) +1)=7 and hour(now())>=17,if(c.finDomingoTarde<ce.finDomingoTarde,c.finDomingoTarde,ce.finDomingoTarde),null)))))))))))))) as finHoy,
      if(c.servicioActivo=0,'False',if(ce.servicioActivo=0,'False',if((WEEKDAY(now()) +1)=1 and (ce.activoLunes=1 or ce.activoLunesTarde=1) and (c.activoLunes=1 or c.activoLunesTarde=1),'True',if((WEEKDAY(now()) +1)=2 and (ce.activoMartes=1 or ce.activoMartesTarde=1) and (ce.activoMartes=1 or ce.activoMartesTarde=1),'True',if((WEEKDAY(now()) +1)=3 and (ce.activoMiercoles=1 or ce.activoMiercolesTarde=1) and (c.activoMiercoles=1 or c.activoMiercolesTarde=1),
      'True',if((WEEKDAY(now()) +1)=4 and (ce.activoJueves=1 or ce.activoJuevesTarde=1)  and (c.activoJueves=1 or c.activoJuevesTarde=1),'True',if((WEEKDAY(now()) +1)=5 and (ce.activoViernes=1 or ce.activoViernesTarde=1) and (c.activoViernes=1 or c.activoViernesTarde=1),'True',if((WEEKDAY(now()) +1)=6 and (ce.activoSabado=1 or ce.activoSabadoTarde=1) and (c.activoSabado=1 or c.activoSabadoTarde=1),
      'True',if((WEEKDAY(now()) +1)=7 and (ce.activoDomingo=1 or ce.activoDomingoTarde=1) and (c.activoDomingo=1 or c.activoDomingoTarde=1),'True','False'))))))))) as activoHoy,
      if((WEEKDAY(now()) +1)=1,if(c.inicioLunes>ce.inicioLunes,c.inicioLunes,ce.inicioLunes),if((WEEKDAY(now()) +1)=2,if(c.inicioMartes>ce.inicioMartes,c.inicioMartes,ce.inicioMartes),if((WEEKDAY(now()) +1)=3,if(c.inicioMiercoles>ce.inicioMiercoles,c.inicioMiercoles,ce.inicioMiercoles),if((WEEKDAY(now()) +1)=4,if(c.inicioJueves>ce.inicioJueves,c.inicioJueves,ce.inicioJueves),if((WEEKDAY(now()) +1)=5,if(c.inicioViernes>ce.inicioViernes,c.inicioViernes,ce.inicioViernes),if((WEEKDAY(now()) +1)=6,if(c.inicioSabado>ce.inicioSabado,c.inicioSabado,ce.inicioSabado),if((WEEKDAY(now()) +1)=7,if(c.inicioDomingo>ce.inicioDomingo,c.inicioDomingo,ce.inicioDomingo),null))))))) as inicioMan,
      if((WEEKDAY(now()) +1)=1,if(c.finLunes<ce.finLunes,c.finLunes,ce.finLunes),if((WEEKDAY(now()) +1)=2,if(c.finMartes<ce.finMartes,c.finMartes,ce.finMartes),if((WEEKDAY(now()) +1)=3,if(c.finMiercoles<ce.finMiercoles,c.finMiercoles,ce.finMiercoles),if((WEEKDAY(now()) +1)=4,if(c.finJueves<ce.finJueves,c.finJueves,ce.finJueves),if((WEEKDAY(now()) +1)=5,if(c.finViernes<ce.finViernes,c.finViernes,ce.finViernes),if((WEEKDAY(now()) +1)=6,if(c.finSabado<ce.finSabado,c.finSabado,ce.finSabado),if((WEEKDAY(now()) +1)=7,if(c.finDomingo<ce.finDomingo,c.finDomingo,ce.finDomingo),null))))))) as finMan,
      if((WEEKDAY(now()) +1)=1,if(c.inicioLunesTarde>ce.inicioLunesTarde,c.inicioLunesTarde,ce.inicioLunesTarde),if((WEEKDAY(now()) +1)=2,if(c.inicioMartesTarde>ce.inicioMartesTarde,c.inicioMartesTarde,ce.inicioMartesTarde),if((WEEKDAY(now()) +1)=3,if(c.inicioMiercolesTarde>ce.inicioMiercolesTarde,c.inicioMiercolesTarde,ce.inicioMiercolesTarde),if((WEEKDAY(now()) +1)=4,if(c.inicioJuevesTarde>ce.inicioJuevesTarde,c.inicioJuevesTarde,ce.inicioJuevesTarde),if((WEEKDAY(now()) +1)=5,if(c.inicioViernesTarde>ce.inicioViernesTarde,c.inicioViernesTarde,ce.inicioViernesTarde),if((WEEKDAY(now()) +1)=6,if(c.inicioSabadoTarde>ce.inicioSabadoTarde,c.inicioSabadoTarde,ce.inicioSabadoTarde),if((WEEKDAY(now()) +1)=7,if(c.inicioDomingoTarde>ce.inicioDomingoTarde,c.inicioDomingoTarde,ce.inicioDomingoTarde),null))))))) as inicioTarde,
      if((WEEKDAY(now()) +1)=1,if(c.finLunesTarde<ce.finLunesTarde,c.finLunesTarde,ce.finLunesTarde),if((WEEKDAY(now()) +1)=2,if(c.finMartesTarde<ce.finMartesTarde,c.finMartesTarde,ce.finMartesTarde),if((WEEKDAY(now()) +1)=3,if(c.finMiercolesTarde<ce.finMiercolesTarde,c.finMiercolesTarde,ce.finMiercolesTarde),if((WEEKDAY(now()) +1)=4,if(c.finJuevesTarde<ce.finJuevesTarde,c.finJuevesTarde,ce.finJuevesTarde),if((WEEKDAY(now()) +1)=5,if(c.finViernesTarde<ce.finViernesTarde,c.finViernesTarde,ce.finViernesTarde),if((WEEKDAY(now()) +1)=6,if(c.finSabadoTarde<ce.finSabadoTarde,c.finSabadoTarde,ce.finSabadoTarde),if((WEEKDAY(now()) +1)=7,if(c.finDomingoTarde<ce.finDomingoTarde,c.finDomingoTarde,ce.finDomingoTarde),null))))))) as finTarde,
      if((WEEKDAY(now()) +1)=1 and c.activoLunes=1,ce.activoLunes,if((WEEKDAY(now()) +1)=2 and c.activoMartes=1,ce.activoMartes,if((WEEKDAY(now()) +1)=3 and c.activoMiercoles=1,ce.activoMiercoles,if((WEEKDAY(now()) +1)=4 and c.activoJueves=1,ce.activoJueves,if((WEEKDAY(now()) +1)=5 and c.activoViernes=1,ce.activoViernes,if((WEEKDAY(now()) +1)=6 and c.activoSabado=1,ce.activoSabado,if((WEEKDAY(now()) +1)=7 and c.activoDomingo=1,ce.activoDomingo,0))))))) as activoMan,
      if((WEEKDAY(now()) +1)=1 and c.activoLunesTarde=1,ce.activoLunesTarde,if((WEEKDAY(now()) +1)=2 and c.activoMartesTarde=1,ce.activoMartesTarde,if((WEEKDAY(now()) +1)=3 and c.activoMiercolesTarde=1,ce.activoMiercolesTarde,if((WEEKDAY(now()) +1)=4 and c.activoJuevesTarde=1,ce.activoJuevesTarde,if((WEEKDAY(now()) +1)=5 and c.activoViernesTarde=1,ce.activoViernesTarde ,if((WEEKDAY(now()) +1)=6 and c.activoSabadoTarde=1,ce.activoSabadoTarde,if((WEEKDAY(now()) +1)=7 and c.activoDomingoTarde=1,ce.activoDomingoTarde,0))))))) as activoTarde
      from qo_establecimientos e 
      inner join qo_configuracion_est ce on (ce.idEstablecimiento=e.id) 
      inner join qo_configuracion c on (c.idGrupo=e.idGrupo) 
      inner join qo_pedidos p on (p.idEstablecimiento=e.id) 
      inner join qo_users u on (u.id=p.idUsuario) 
      WHERE e.idGrupo=:idGrupo and e.estado=1 GROUP BY p.idEstablecimiento ORDER BY `pedidos` DESC limit 10");
      $sql->bindValue(':idGrupo', $_GET['idGrupo']);  
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
    }
      exit();
  }else if (isset($_GET['popularTodos']))
  {
    if ($_GET['idGrupo']=='0'){
      $sql = $dbConn->prepare("SELECT count(*) as pedidos,0 as favorito,e.*,e.id as idEstablecimiento,if(ce.servicioActivo=1,'True','False') as servicioActivo,ce.pedidoMinimo,ce.tiempoEntrega,ce.numeroPedidosSoportado,
      if((WEEKDAY(now()) +1)=1 and hour(now())<17 and c.activoLunes=1 and ce.activoLunes=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioLunes>ce.inicioLunes,TIME_FORMAT(c.inicioLunes, '%H:%i'),TIME_FORMAT(ce.inicioLunes, '%H:%i')),' - ',if(c.finLunes<ce.finLunes,TIME_FORMAT(c.finLunes, '%H:%i'),TIME_FORMAT(ce.FinLunes, '%H:%i'))),if((WEEKDAY(now()) +1)=1 and hour(now())>=17 and c.activoLunesTarde=1 and ce.activoLunesTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioLunesTarde>ce.inicioLunesTarde,TIME_FORMAT(c.inicioLunesTarde, '%H:%i'),TIME_FORMAT(ce.inicioLunesTarde, '%H:%i')),' - ',if(c.finLunesTarde<ce.finLunesTarde,TIME_FORMAT(c.finLunesTarde, '%H:%i'),TIME_FORMAT(ce.FinLunesTarde, '%H:%i'))),if((WEEKDAY(now()) +1)=2 and hour(now())<17 and c.activoMartes=1 and ce.activoMartes=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioMartes>ce.inicioMartes,TIME_FORMAT(c.inicioMartes, '%H:%i'),TIME_FORMAT(ce.inicioMartes, '%H:%i')),' - ',if(c.finMartes<ce.finMartes,TIME_FORMAT(c.finMartes, '%H:%i'),TIME_FORMAT(ce.FinMartes, '%H:%i'))),if((WEEKDAY(now()) +1)=2 and hour(now())>=17 and c.activoMartesTarde=1 and ce.activoMartesTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioMartesTarde>ce.inicioMartesTarde,TIME_FORMAT(c.inicioMartesTarde, '%H:%i'),TIME_FORMAT(ce.inicioMartesTarde, '%H:%i')),' - ',if(c.finMartesTarde<ce.finMartesTarde,TIME_FORMAT(c.finMartesTarde, '%H:%i'),TIME_FORMAT(ce.FinMartesTarde, '%H:%i'))),if((WEEKDAY(now()) +1)=3 and hour(now())<17 and c.activoMiercoles=1 and ce.activoMiercoles=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioMiercoles>ce.inicioMiercoles,TIME_FORMAT(c.inicioMiercoles, '%H:%i'),TIME_FORMAT(ce.inicioMiercoles, '%H:%i')),' - ',if(c.finMiercoles<ce.finMiercoles,TIME_FORMAT(c.finMiercoles, '%H:%i'),TIME_FORMAT(ce.FinMiercoles, '%H:%i'))),if((WEEKDAY(now()) +1)=3 and hour(now())>=17 and c.activoMiercolesTarde=1 and ce.activoMiercolesTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioMiercolesTarde>ce.inicioMiercolesTarde,TIME_FORMAT(c.inicioMiercolesTarde, '%H:%i'),TIME_FORMAT(ce.inicioMiercolesTarde, '%H:%i')),' - ',if(c.finMiercolesTarde<ce.finMiercolesTarde,TIME_FORMAT(c.finMiercolesTarde, '%H:%i'),TIME_FORMAT(ce.FinMiercolesTarde, '%H:%i'))),if((WEEKDAY(now()) +1)=4 and hour(now())<17 and c.activoJueves=1 and ce.activoJueves=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioJueves>ce.inicioJueves,TIME_FORMAT(c.inicioJueves, '%H:%i'),TIME_FORMAT(ce.inicioJueves, '%H:%i')),' - ',if(c.finJueves<ce.finJueves,TIME_FORMAT(c.finJueves, '%H:%i'),TIME_FORMAT(ce.FinJueves, '%H:%i'))),if((WEEKDAY(now()) +1)=4 and hour(now())>=17 and c.activoJuevesTarde=1 and ce.activoJuevesTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioJuevesTarde>ce.inicioJuevesTarde,TIME_FORMAT(c.inicioJuevesTarde, '%H:%i'),TIME_FORMAT(ce.inicioJuevesTarde, '%H:%i')),' - ',if(c.finJuevesTarde<ce.finJuevesTarde,TIME_FORMAT(c.finJuevesTarde, '%H:%i'),TIME_FORMAT(ce.FinJuevesTarde, '%H:%i'))),if((WEEKDAY(now()) +1)=5 and hour(now())<17 and c.activoViernes=1 and ce.activoViernes=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioViernes>ce.inicioViernes,TIME_FORMAT(c.inicioViernes, '%H:%i'),TIME_FORMAT(ce.inicioViernes, '%H:%i')),' - ',if(c.finViernes<ce.finViernes,TIME_FORMAT(c.finViernes, '%H:%i'),TIME_FORMAT(ce.FinViernes, '%H:%i'))),if((WEEKDAY(now()) +1)=5 and hour(now())>=17 and c.activoViernesTarde=1 and ce.activoViernesTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioViernesTarde>ce.inicioViernesTarde,TIME_FORMAT(c.inicioViernesTarde, '%H:%i'),TIME_FORMAT(ce.inicioViernesTarde, '%H:%i')),' - ',if(c.finViernesTarde<ce.finViernesTarde,TIME_FORMAT(c.finViernesTarde, '%H:%i'),TIME_FORMAT(ce.FinViernesTarde, '%H:%i'))),if((WEEKDAY(now()) +1)=6 and hour(now())<17 and c.activoSabado=1 and ce.activoSabado=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioSabado>ce.inicioSabado,TIME_FORMAT(c.inicioSabado, '%H:%i'),TIME_FORMAT(ce.inicioSabado, '%H:%i')),' - ',if(c.finSabado<ce.finSabado,TIME_FORMAT(c.finSabado, '%H:%i'),TIME_FORMAT(ce.FinSabado, '%H:%i'))),if((WEEKDAY(now()) +1)=6 and hour(now())>=17 and c.activoSabadoTarde=1 and ce.activoSabadoTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioSabadoTarde>ce.inicioSabadoTarde,TIME_FORMAT(c.inicioSabadoTarde, '%H:%i'),TIME_FORMAT(ce.inicioSabadoTarde, '%H:%i')),' - ',if(c.finSabadoTarde<ce.finSabadoTarde,TIME_FORMAT(c.finSabadoTarde, '%H:%i'),TIME_FORMAT(ce.FinSabadoTarde, '%H:%i'))),if((WEEKDAY(now()) +1)=7 and hour(now())<17 and c.activoDomingo=1 and ce.activoDomingo=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioDomingo>ce.inicioDomingo,TIME_FORMAT(c.inicioDomingo, '%H:%i'),TIME_FORMAT(ce.inicioDomingo, '%H:%i')),' - ',if(c.finDomingo<ce.finDomingo,TIME_FORMAT(c.finDomingo, '%H:%i'),TIME_FORMAT(ce.FinDomingo, '%H:%i'))),if((WEEKDAY(now()) +1)=7 and hour(now())>=17 and c.activoDomingoTarde=1 and ce.activoDomingoTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioDomingoTarde<ce.inicioDomingoTarde,TIME_FORMAT(c.inicioDomingoTarde, '%H:%i'),TIME_FORMAT(ce.inicioDomingoTarde, '%H:%i')),' - ',if(c.finDomingoTarde<ce.finDomingoTarde,TIME_FORMAT(c.finDomingoTarde, '%H:%i'),TIME_FORMAT(ce.FinDomingoTarde, '%H:%i'))),'CERRADO')))))))))))))) as horario,if((WEEKDAY(now()) +1)=1 and hour(now())<17,if(c.inicioLunes>ce.inicioLunes,c.inicioLunes,ce.inicioLunes),if((WEEKDAY(now()) +1)=1 and hour(now())>=17,if(c.inicioLunesTarde>ce.inicioLunesTarde,c.inicioLunesTarde,ce.inicioLunesTarde),if((WEEKDAY(now()) +1)=2 and hour(now())<17,if(c.inicioMartes>ce.inicioMartes,c.inicioMartes,ce.inicioMartes),if((WEEKDAY(now()) +1)=2 and hour(now())>=17,if(c.inicioMartesTarde>ce.inicioMartesTarde,c.inicioMartesTarde,ce.inicioMartesTarde),if((WEEKDAY(now()) +1)=3 and hour(now())<17,if(c.inicioMiercoles>ce.inicioMiercoles,c.inicioMiercoles,ce.inicioMiercoles),if((WEEKDAY(now()) +1)=3 and hour(now())>=17,if(c.inicioMiercolesTarde>ce.inicioMiercolesTarde,c.inicioMiercolesTarde,ce.inicioMiercolesTarde),if((WEEKDAY(now()) +1)=4 and hour(now())<17,if(c.inicioJueves>ce.inicioJueves,c.inicioJueves,ce.inicioJueves),if((WEEKDAY(now()) +1)=4 and hour(now())>=17,if(c.inicioJuevesTarde>ce.inicioJuevesTarde,c.inicioJuevesTarde,ce.inicioJuevesTarde),if((WEEKDAY(now()) +1)=5 and hour(now())<17,if(c.inicioViernes>ce.inicioViernes,c.inicioViernes,ce.inicioViernes),if((WEEKDAY(now()) +1)=5 and hour(now())>=17,if(c.inicioViernesTarde>ce.inicioViernesTarde,c.inicioViernesTarde,ce.inicioViernesTarde),if((WEEKDAY(now()) +1)=6 and hour(now())<17,if(c.inicioSabado>ce.inicioSabado,c.inicioSabado,ce.inicioSabado),if((WEEKDAY(now()) +1)=6 and hour(now())>=17,if(c.inicioSabadoTarde>ce.inicioSabadoTarde,c.inicioSabadoTarde,ce.inicioSabadoTarde),if((WEEKDAY(now()) +1)=7 and hour(now())<17,if(c.inicioDomingo>ce.inicioDomingo,c.inicioDomingo,ce.inicioDomingo),if((WEEKDAY(now()) +1)=7 and hour(now())>=17,if(c.inicioDomingoTarde>ce.inicioDomingoTarde,c.inicioDomingoTarde,ce.inicioDomingoTarde),null)))))))))))))) as inicioHoy,if((WEEKDAY(now()) +1)=1 and hour(now())<17,if(c.finLunes<ce.finLunes,c.finLunes,ce.finLunes),if((WEEKDAY(now()) +1)=1 and hour(now())>=17,if(c.finLunesTarde<ce.finLunesTarde,c.finLunesTarde,ce.finLunesTarde),if((WEEKDAY(now()) +1)=2 and hour(now())<17,if(c.finMartes<ce.finMartes,c.finMartes,ce.finMartes),if((WEEKDAY(now()) +1)=2 and hour(now())>=17,if(c.finMartesTarde<ce.finMartesTarde,c.finMartesTarde,ce.finMartesTarde),if((WEEKDAY(now()) +1)=3 and hour(now())<17,if(c.finMiercoles<ce.finMiercoles,c.finMiercoles,ce.finMiercoles),if((WEEKDAY(now()) +1)=3 and hour(now())>=17,if(c.finMiercolesTarde<ce.finMiercolesTarde,c.finMiercolesTarde,ce.finMiercolesTarde),if((WEEKDAY(now()) +1)=4 and hour(now())<17,if(c.finJueves<ce.finJueves,c.finJueves,ce.finJueves),if((WEEKDAY(now()) +1)=4 and hour(now())>=17,if(c.finJuevesTarde<ce.finJuevesTarde,c.finJuevesTarde,ce.finJuevesTarde),if((WEEKDAY(now()) +1)=5 and hour(now())<17,if(c.finViernes<ce.finViernes,c.finViernes,ce.finViernes),if((WEEKDAY(now()) +1)=5 and hour(now())>=17,if(c.finViernesTarde<ce.finViernesTarde,c.finViernesTarde,ce.finViernesTarde),if((WEEKDAY(now()) +1)=6 and hour(now())<17,if(c.finSabado<ce.finSabado,c.finSabado,ce.finSabado),if((WEEKDAY(now()) +1)=6 and hour(now())>=17,if(c.finSabadoTarde<ce.finSabadoTarde,c.finSabadoTarde,ce.finSabadoTarde),if((WEEKDAY(now()) +1)=7 and hour(now())<17,if(c.finDomingo<ce.finDomingo,c.finDomingo,ce.finDomingo),if((WEEKDAY(now()) +1)=7 and hour(now())>=17,if(c.finDomingoTarde<ce.finDomingoTarde,c.finDomingoTarde,ce.finDomingoTarde),null)))))))))))))) as finHoy,
      if(c.servicioActivo=0,'False',if(ce.servicioActivo=0,'False',if((WEEKDAY(now()) +1)=1 and (ce.activoLunes=1 or ce.activoLunesTarde=1) and (c.activoLunes=1 or c.activoLunesTarde=1),'True',if((WEEKDAY(now()) +1)=2 and (ce.activoMartes=1 or ce.activoMartesTarde=1) and (ce.activoMartes=1 or ce.activoMartesTarde=1),'True',if((WEEKDAY(now()) +1)=3 and (ce.activoMiercoles=1 or ce.activoMiercolesTarde=1) and (c.activoMiercoles=1 or c.activoMiercolesTarde=1),
      'True',if((WEEKDAY(now()) +1)=4 and (ce.activoJueves=1 or ce.activoJuevesTarde=1)  and (c.activoJueves=1 or c.activoJuevesTarde=1),'True',if((WEEKDAY(now()) +1)=5 and (ce.activoViernes=1 or ce.activoViernesTarde=1) and (c.activoViernes=1 or c.activoViernesTarde=1),'True',if((WEEKDAY(now()) +1)=6 and (ce.activoSabado=1 or ce.activoSabadoTarde=1) and (c.activoSabado=1 or c.activoSabadoTarde=1),
      'True',if((WEEKDAY(now()) +1)=7 and (ce.activoDomingo=1 or ce.activoDomingoTarde=1) and (c.activoDomingo=1 or c.activoDomingoTarde=1),'True','False'))))))))) as activoHoy,
      if((WEEKDAY(now()) +1)=1,if(c.inicioLunes>ce.inicioLunes,c.inicioLunes,ce.inicioLunes),if((WEEKDAY(now()) +1)=2,if(c.inicioMartes>ce.inicioMartes,c.inicioMartes,ce.inicioMartes),if((WEEKDAY(now()) +1)=3,if(c.inicioMiercoles>ce.inicioMiercoles,c.inicioMiercoles,ce.inicioMiercoles),if((WEEKDAY(now()) +1)=4,if(c.inicioJueves>ce.inicioJueves,c.inicioJueves,ce.inicioJueves),if((WEEKDAY(now()) +1)=5,if(c.inicioViernes>ce.inicioViernes,c.inicioViernes,ce.inicioViernes),if((WEEKDAY(now()) +1)=6,if(c.inicioSabado>ce.inicioSabado,c.inicioSabado,ce.inicioSabado),if((WEEKDAY(now()) +1)=7,if(c.inicioDomingo>ce.inicioDomingo,c.inicioDomingo,ce.inicioDomingo),null))))))) as inicioMan,
      if((WEEKDAY(now()) +1)=1,if(c.finLunes<ce.finLunes,c.finLunes,ce.finLunes),if((WEEKDAY(now()) +1)=2,if(c.finMartes<ce.finMartes,c.finMartes,ce.finMartes),if((WEEKDAY(now()) +1)=3,if(c.finMiercoles<ce.finMiercoles,c.finMiercoles,ce.finMiercoles),if((WEEKDAY(now()) +1)=4,if(c.finJueves<ce.finJueves,c.finJueves,ce.finJueves),if((WEEKDAY(now()) +1)=5,if(c.finViernes<ce.finViernes,c.finViernes,ce.finViernes),if((WEEKDAY(now()) +1)=6,if(c.finSabado<ce.finSabado,c.finSabado,ce.finSabado),if((WEEKDAY(now()) +1)=7,if(c.finDomingo<ce.finDomingo,c.finDomingo,ce.finDomingo),null))))))) as finMan,
      if((WEEKDAY(now()) +1)=1,if(c.inicioLunesTarde>ce.inicioLunesTarde,c.inicioLunesTarde,ce.inicioLunesTarde),if((WEEKDAY(now()) +1)=2,if(c.inicioMartesTarde>ce.inicioMartesTarde,c.inicioMartesTarde,ce.inicioMartesTarde),if((WEEKDAY(now()) +1)=3,if(c.inicioMiercolesTarde>ce.inicioMiercolesTarde,c.inicioMiercolesTarde,ce.inicioMiercolesTarde),if((WEEKDAY(now()) +1)=4,if(c.inicioJuevesTarde>ce.inicioJuevesTarde,c.inicioJuevesTarde,ce.inicioJuevesTarde),if((WEEKDAY(now()) +1)=5,if(c.inicioViernesTarde>ce.inicioViernesTarde,c.inicioViernesTarde,ce.inicioViernesTarde),if((WEEKDAY(now()) +1)=6,if(c.inicioSabadoTarde>ce.inicioSabadoTarde,c.inicioSabadoTarde,ce.inicioSabadoTarde),if((WEEKDAY(now()) +1)=7,if(c.inicioDomingoTarde>ce.inicioDomingoTarde,c.inicioDomingoTarde,ce.inicioDomingoTarde),null))))))) as inicioTarde,
      if((WEEKDAY(now()) +1)=1,if(c.finLunesTarde<ce.finLunesTarde,c.finLunesTarde,ce.finLunesTarde),if((WEEKDAY(now()) +1)=2,if(c.finMartesTarde<ce.finMartesTarde,c.finMartesTarde,ce.finMartesTarde),if((WEEKDAY(now()) +1)=3,if(c.finMiercolesTarde<ce.finMiercolesTarde,c.finMiercolesTarde,ce.finMiercolesTarde),if((WEEKDAY(now()) +1)=4,if(c.finJuevesTarde<ce.finJuevesTarde,c.finJuevesTarde,ce.finJuevesTarde),if((WEEKDAY(now()) +1)=5,if(c.finViernesTarde<ce.finViernesTarde,c.finViernesTarde,ce.finViernesTarde),if((WEEKDAY(now()) +1)=6,if(c.finSabadoTarde<ce.finSabadoTarde,c.finSabadoTarde,ce.finSabadoTarde),if((WEEKDAY(now()) +1)=7,if(c.finDomingoTarde<ce.finDomingoTarde,c.finDomingoTarde,ce.finDomingoTarde),null))))))) as finTarde,
      if((WEEKDAY(now()) +1)=1 and c.activoLunes=1,ce.activoLunes,if((WEEKDAY(now()) +1)=2 and c.activoMartes=1,ce.activoMartes,if((WEEKDAY(now()) +1)=3 and c.activoMiercoles=1,ce.activoMiercoles,if((WEEKDAY(now()) +1)=4 and c.activoJueves=1,ce.activoJueves,if((WEEKDAY(now()) +1)=5 and c.activoViernes=1,ce.activoViernes,if((WEEKDAY(now()) +1)=6 and c.activoSabado=1,ce.activoSabado,if((WEEKDAY(now()) +1)=7 and c.activoDomingo=1,ce.activoDomingo,0))))))) as activoMan,
      if((WEEKDAY(now()) +1)=1 and c.activoLunesTarde=1,ce.activoLunesTarde,if((WEEKDAY(now()) +1)=2 and c.activoMartesTarde=1,ce.activoMartesTarde,if((WEEKDAY(now()) +1)=3 and c.activoMiercolesTarde=1,ce.activoMiercolesTarde,if((WEEKDAY(now()) +1)=4 and c.activoJuevesTarde=1,ce.activoJuevesTarde,if((WEEKDAY(now()) +1)=5 and c.activoViernesTarde=1,ce.activoViernesTarde ,if((WEEKDAY(now()) +1)=6 and c.activoSabadoTarde=1,ce.activoSabadoTarde,if((WEEKDAY(now()) +1)=7 and c.activoDomingoTarde=1,ce.activoDomingoTarde,0))))))) as activoTarde
      from qo_establecimientos e 
      inner join qo_configuracion_est ce on (ce.idEstablecimiento=e.id) 
      inner join qo_configuracion c on (c.idGrupo=e.idGrupo) 
      inner join qo_pedidos p on (p.idEstablecimiento=e.id) 
      inner join qo_users u on (u.id=p.idUsuario) 
      WHERE e.estado=1 GROUP BY p.idEstablecimiento ORDER BY `pedidos` DESC");
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
    }else{
      $sql = $dbConn->prepare("SELECT count(*) as pedidos,0 as favorito,e.*,e.id as idEstablecimiento,if(ce.servicioActivo=1,'True','False') as servicioActivo,ce.pedidoMinimo,ce.tiempoEntrega,ce.numeroPedidosSoportado,
      if((WEEKDAY(now()) +1)=1 and hour(now())<17 and c.activoLunes=1 and ce.activoLunes=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioLunes>ce.inicioLunes,TIME_FORMAT(c.inicioLunes, '%H:%i'),TIME_FORMAT(ce.inicioLunes, '%H:%i')),' - ',if(c.finLunes<ce.finLunes,TIME_FORMAT(c.finLunes, '%H:%i'),TIME_FORMAT(ce.FinLunes, '%H:%i'))),if((WEEKDAY(now()) +1)=1 and hour(now())>=17 and c.activoLunesTarde=1 and ce.activoLunesTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioLunesTarde>ce.inicioLunesTarde,TIME_FORMAT(c.inicioLunesTarde, '%H:%i'),TIME_FORMAT(ce.inicioLunesTarde, '%H:%i')),' - ',if(c.finLunesTarde<ce.finLunesTarde,TIME_FORMAT(c.finLunesTarde, '%H:%i'),TIME_FORMAT(ce.FinLunesTarde, '%H:%i'))),if((WEEKDAY(now()) +1)=2 and hour(now())<17 and c.activoMartes=1 and ce.activoMartes=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioMartes>ce.inicioMartes,TIME_FORMAT(c.inicioMartes, '%H:%i'),TIME_FORMAT(ce.inicioMartes, '%H:%i')),' - ',if(c.finMartes<ce.finMartes,TIME_FORMAT(c.finMartes, '%H:%i'),TIME_FORMAT(ce.FinMartes, '%H:%i'))),if((WEEKDAY(now()) +1)=2 and hour(now())>=17 and c.activoMartesTarde=1 and ce.activoMartesTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioMartesTarde>ce.inicioMartesTarde,TIME_FORMAT(c.inicioMartesTarde, '%H:%i'),TIME_FORMAT(ce.inicioMartesTarde, '%H:%i')),' - ',if(c.finMartesTarde<ce.finMartesTarde,TIME_FORMAT(c.finMartesTarde, '%H:%i'),TIME_FORMAT(ce.FinMartesTarde, '%H:%i'))),if((WEEKDAY(now()) +1)=3 and hour(now())<17 and c.activoMiercoles=1 and ce.activoMiercoles=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioMiercoles>ce.inicioMiercoles,TIME_FORMAT(c.inicioMiercoles, '%H:%i'),TIME_FORMAT(ce.inicioMiercoles, '%H:%i')),' - ',if(c.finMiercoles<ce.finMiercoles,TIME_FORMAT(c.finMiercoles, '%H:%i'),TIME_FORMAT(ce.FinMiercoles, '%H:%i'))),if((WEEKDAY(now()) +1)=3 and hour(now())>=17 and c.activoMiercolesTarde=1 and ce.activoMiercolesTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioMiercolesTarde>ce.inicioMiercolesTarde,TIME_FORMAT(c.inicioMiercolesTarde, '%H:%i'),TIME_FORMAT(ce.inicioMiercolesTarde, '%H:%i')),' - ',if(c.finMiercolesTarde<ce.finMiercolesTarde,TIME_FORMAT(c.finMiercolesTarde, '%H:%i'),TIME_FORMAT(ce.FinMiercolesTarde, '%H:%i'))),if((WEEKDAY(now()) +1)=4 and hour(now())<17 and c.activoJueves=1 and ce.activoJueves=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioJueves>ce.inicioJueves,TIME_FORMAT(c.inicioJueves, '%H:%i'),TIME_FORMAT(ce.inicioJueves, '%H:%i')),' - ',if(c.finJueves<ce.finJueves,TIME_FORMAT(c.finJueves, '%H:%i'),TIME_FORMAT(ce.FinJueves, '%H:%i'))),if((WEEKDAY(now()) +1)=4 and hour(now())>=17 and c.activoJuevesTarde=1 and ce.activoJuevesTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioJuevesTarde>ce.inicioJuevesTarde,TIME_FORMAT(c.inicioJuevesTarde, '%H:%i'),TIME_FORMAT(ce.inicioJuevesTarde, '%H:%i')),' - ',if(c.finJuevesTarde<ce.finJuevesTarde,TIME_FORMAT(c.finJuevesTarde, '%H:%i'),TIME_FORMAT(ce.FinJuevesTarde, '%H:%i'))),if((WEEKDAY(now()) +1)=5 and hour(now())<17 and c.activoViernes=1 and ce.activoViernes=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioViernes>ce.inicioViernes,TIME_FORMAT(c.inicioViernes, '%H:%i'),TIME_FORMAT(ce.inicioViernes, '%H:%i')),' - ',if(c.finViernes<ce.finViernes,TIME_FORMAT(c.finViernes, '%H:%i'),TIME_FORMAT(ce.FinViernes, '%H:%i'))),if((WEEKDAY(now()) +1)=5 and hour(now())>=17 and c.activoViernesTarde=1 and ce.activoViernesTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioViernesTarde>ce.inicioViernesTarde,TIME_FORMAT(c.inicioViernesTarde, '%H:%i'),TIME_FORMAT(ce.inicioViernesTarde, '%H:%i')),' - ',if(c.finViernesTarde<ce.finViernesTarde,TIME_FORMAT(c.finViernesTarde, '%H:%i'),TIME_FORMAT(ce.FinViernesTarde, '%H:%i'))),if((WEEKDAY(now()) +1)=6 and hour(now())<17 and c.activoSabado=1 and ce.activoSabado=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioSabado>ce.inicioSabado,TIME_FORMAT(c.inicioSabado, '%H:%i'),TIME_FORMAT(ce.inicioSabado, '%H:%i')),' - ',if(c.finSabado<ce.finSabado,TIME_FORMAT(c.finSabado, '%H:%i'),TIME_FORMAT(ce.FinSabado, '%H:%i'))),if((WEEKDAY(now()) +1)=6 and hour(now())>=17 and c.activoSabadoTarde=1 and ce.activoSabadoTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioSabadoTarde>ce.inicioSabadoTarde,TIME_FORMAT(c.inicioSabadoTarde, '%H:%i'),TIME_FORMAT(ce.inicioSabadoTarde, '%H:%i')),' - ',if(c.finSabadoTarde<ce.finSabadoTarde,TIME_FORMAT(c.finSabadoTarde, '%H:%i'),TIME_FORMAT(ce.FinSabadoTarde, '%H:%i'))),if((WEEKDAY(now()) +1)=7 and hour(now())<17 and c.activoDomingo=1 and ce.activoDomingo=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioDomingo>ce.inicioDomingo,TIME_FORMAT(c.inicioDomingo, '%H:%i'),TIME_FORMAT(ce.inicioDomingo, '%H:%i')),' - ',if(c.finDomingo<ce.finDomingo,TIME_FORMAT(c.finDomingo, '%H:%i'),TIME_FORMAT(ce.FinDomingo, '%H:%i'))),if((WEEKDAY(now()) +1)=7 and hour(now())>=17 and c.activoDomingoTarde=1 and ce.activoDomingoTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioDomingoTarde<ce.inicioDomingoTarde,TIME_FORMAT(c.inicioDomingoTarde, '%H:%i'),TIME_FORMAT(ce.inicioDomingoTarde, '%H:%i')),' - ',if(c.finDomingoTarde<ce.finDomingoTarde,TIME_FORMAT(c.finDomingoTarde, '%H:%i'),TIME_FORMAT(ce.FinDomingoTarde, '%H:%i'))),'CERRADO')))))))))))))) as horario,if((WEEKDAY(now()) +1)=1 and hour(now())<17,if(c.inicioLunes>ce.inicioLunes,c.inicioLunes,ce.inicioLunes),if((WEEKDAY(now()) +1)=1 and hour(now())>=17,if(c.inicioLunesTarde>ce.inicioLunesTarde,c.inicioLunesTarde,ce.inicioLunesTarde),if((WEEKDAY(now()) +1)=2 and hour(now())<17,if(c.inicioMartes>ce.inicioMartes,c.inicioMartes,ce.inicioMartes),if((WEEKDAY(now()) +1)=2 and hour(now())>=17,if(c.inicioMartesTarde>ce.inicioMartesTarde,c.inicioMartesTarde,ce.inicioMartesTarde),if((WEEKDAY(now()) +1)=3 and hour(now())<17,if(c.inicioMiercoles>ce.inicioMiercoles,c.inicioMiercoles,ce.inicioMiercoles),if((WEEKDAY(now()) +1)=3 and hour(now())>=17,if(c.inicioMiercolesTarde>ce.inicioMiercolesTarde,c.inicioMiercolesTarde,ce.inicioMiercolesTarde),if((WEEKDAY(now()) +1)=4 and hour(now())<17,if(c.inicioJueves>ce.inicioJueves,c.inicioJueves,ce.inicioJueves),if((WEEKDAY(now()) +1)=4 and hour(now())>=17,if(c.inicioJuevesTarde>ce.inicioJuevesTarde,c.inicioJuevesTarde,ce.inicioJuevesTarde),if((WEEKDAY(now()) +1)=5 and hour(now())<17,if(c.inicioViernes>ce.inicioViernes,c.inicioViernes,ce.inicioViernes),if((WEEKDAY(now()) +1)=5 and hour(now())>=17,if(c.inicioViernesTarde>ce.inicioViernesTarde,c.inicioViernesTarde,ce.inicioViernesTarde),if((WEEKDAY(now()) +1)=6 and hour(now())<17,if(c.inicioSabado>ce.inicioSabado,c.inicioSabado,ce.inicioSabado),if((WEEKDAY(now()) +1)=6 and hour(now())>=17,if(c.inicioSabadoTarde>ce.inicioSabadoTarde,c.inicioSabadoTarde,ce.inicioSabadoTarde),if((WEEKDAY(now()) +1)=7 and hour(now())<17,if(c.inicioDomingo>ce.inicioDomingo,c.inicioDomingo,ce.inicioDomingo),if((WEEKDAY(now()) +1)=7 and hour(now())>=17,if(c.inicioDomingoTarde>ce.inicioDomingoTarde,c.inicioDomingoTarde,ce.inicioDomingoTarde),null)))))))))))))) as inicioHoy,if((WEEKDAY(now()) +1)=1 and hour(now())<17,if(c.finLunes<ce.finLunes,c.finLunes,ce.finLunes),if((WEEKDAY(now()) +1)=1 and hour(now())>=17,if(c.finLunesTarde<ce.finLunesTarde,c.finLunesTarde,ce.finLunesTarde),if((WEEKDAY(now()) +1)=2 and hour(now())<17,if(c.finMartes<ce.finMartes,c.finMartes,ce.finMartes),if((WEEKDAY(now()) +1)=2 and hour(now())>=17,if(c.finMartesTarde<ce.finMartesTarde,c.finMartesTarde,ce.finMartesTarde),if((WEEKDAY(now()) +1)=3 and hour(now())<17,if(c.finMiercoles<ce.finMiercoles,c.finMiercoles,ce.finMiercoles),if((WEEKDAY(now()) +1)=3 and hour(now())>=17,if(c.finMiercolesTarde<ce.finMiercolesTarde,c.finMiercolesTarde,ce.finMiercolesTarde),if((WEEKDAY(now()) +1)=4 and hour(now())<17,if(c.finJueves<ce.finJueves,c.finJueves,ce.finJueves),if((WEEKDAY(now()) +1)=4 and hour(now())>=17,if(c.finJuevesTarde<ce.finJuevesTarde,c.finJuevesTarde,ce.finJuevesTarde),if((WEEKDAY(now()) +1)=5 and hour(now())<17,if(c.finViernes<ce.finViernes,c.finViernes,ce.finViernes),if((WEEKDAY(now()) +1)=5 and hour(now())>=17,if(c.finViernesTarde<ce.finViernesTarde,c.finViernesTarde,ce.finViernesTarde),if((WEEKDAY(now()) +1)=6 and hour(now())<17,if(c.finSabado<ce.finSabado,c.finSabado,ce.finSabado),if((WEEKDAY(now()) +1)=6 and hour(now())>=17,if(c.finSabadoTarde<ce.finSabadoTarde,c.finSabadoTarde,ce.finSabadoTarde),if((WEEKDAY(now()) +1)=7 and hour(now())<17,if(c.finDomingo<ce.finDomingo,c.finDomingo,ce.finDomingo),if((WEEKDAY(now()) +1)=7 and hour(now())>=17,if(c.finDomingoTarde<ce.finDomingoTarde,c.finDomingoTarde,ce.finDomingoTarde),null)))))))))))))) as finHoy,
      if(c.servicioActivo=0,'False',if(ce.servicioActivo=0,'False',if((WEEKDAY(now()) +1)=1 and (ce.activoLunes=1 or ce.activoLunesTarde=1) and (c.activoLunes=1 or c.activoLunesTarde=1),'True',if((WEEKDAY(now()) +1)=2 and (ce.activoMartes=1 or ce.activoMartesTarde=1) and (ce.activoMartes=1 or ce.activoMartesTarde=1),'True',if((WEEKDAY(now()) +1)=3 and (ce.activoMiercoles=1 or ce.activoMiercolesTarde=1) and (c.activoMiercoles=1 or c.activoMiercolesTarde=1),
      'True',if((WEEKDAY(now()) +1)=4 and (ce.activoJueves=1 or ce.activoJuevesTarde=1)  and (c.activoJueves=1 or c.activoJuevesTarde=1),'True',if((WEEKDAY(now()) +1)=5 and (ce.activoViernes=1 or ce.activoViernesTarde=1) and (c.activoViernes=1 or c.activoViernesTarde=1),'True',if((WEEKDAY(now()) +1)=6 and (ce.activoSabado=1 or ce.activoSabadoTarde=1) and (c.activoSabado=1 or c.activoSabadoTarde=1),
      'True',if((WEEKDAY(now()) +1)=7 and (ce.activoDomingo=1 or ce.activoDomingoTarde=1) and (c.activoDomingo=1 or c.activoDomingoTarde=1),'True','False'))))))))) as activoHoy,
      if((WEEKDAY(now()) +1)=1,if(c.inicioLunes>ce.inicioLunes,c.inicioLunes,ce.inicioLunes),if((WEEKDAY(now()) +1)=2,if(c.inicioMartes>ce.inicioMartes,c.inicioMartes,ce.inicioMartes),if((WEEKDAY(now()) +1)=3,if(c.inicioMiercoles>ce.inicioMiercoles,c.inicioMiercoles,ce.inicioMiercoles),if((WEEKDAY(now()) +1)=4,if(c.inicioJueves>ce.inicioJueves,c.inicioJueves,ce.inicioJueves),if((WEEKDAY(now()) +1)=5,if(c.inicioViernes>ce.inicioViernes,c.inicioViernes,ce.inicioViernes),if((WEEKDAY(now()) +1)=6,if(c.inicioSabado>ce.inicioSabado,c.inicioSabado,ce.inicioSabado),if((WEEKDAY(now()) +1)=7,if(c.inicioDomingo>ce.inicioDomingo,c.inicioDomingo,ce.inicioDomingo),null))))))) as inicioMan,
      if((WEEKDAY(now()) +1)=1,if(c.finLunes<ce.finLunes,c.finLunes,ce.finLunes),if((WEEKDAY(now()) +1)=2,if(c.finMartes<ce.finMartes,c.finMartes,ce.finMartes),if((WEEKDAY(now()) +1)=3,if(c.finMiercoles<ce.finMiercoles,c.finMiercoles,ce.finMiercoles),if((WEEKDAY(now()) +1)=4,if(c.finJueves<ce.finJueves,c.finJueves,ce.finJueves),if((WEEKDAY(now()) +1)=5,if(c.finViernes<ce.finViernes,c.finViernes,ce.finViernes),if((WEEKDAY(now()) +1)=6,if(c.finSabado<ce.finSabado,c.finSabado,ce.finSabado),if((WEEKDAY(now()) +1)=7,if(c.finDomingo<ce.finDomingo,c.finDomingo,ce.finDomingo),null))))))) as finMan,
      if((WEEKDAY(now()) +1)=1,if(c.inicioLunesTarde>ce.inicioLunesTarde,c.inicioLunesTarde,ce.inicioLunesTarde),if((WEEKDAY(now()) +1)=2,if(c.inicioMartesTarde>ce.inicioMartesTarde,c.inicioMartesTarde,ce.inicioMartesTarde),if((WEEKDAY(now()) +1)=3,if(c.inicioMiercolesTarde>ce.inicioMiercolesTarde,c.inicioMiercolesTarde,ce.inicioMiercolesTarde),if((WEEKDAY(now()) +1)=4,if(c.inicioJuevesTarde>ce.inicioJuevesTarde,c.inicioJuevesTarde,ce.inicioJuevesTarde),if((WEEKDAY(now()) +1)=5,if(c.inicioViernesTarde>ce.inicioViernesTarde,c.inicioViernesTarde,ce.inicioViernesTarde),if((WEEKDAY(now()) +1)=6,if(c.inicioSabadoTarde>ce.inicioSabadoTarde,c.inicioSabadoTarde,ce.inicioSabadoTarde),if((WEEKDAY(now()) +1)=7,if(c.inicioDomingoTarde>ce.inicioDomingoTarde,c.inicioDomingoTarde,ce.inicioDomingoTarde),null))))))) as inicioTarde,
      if((WEEKDAY(now()) +1)=1,if(c.finLunesTarde<ce.finLunesTarde,c.finLunesTarde,ce.finLunesTarde),if((WEEKDAY(now()) +1)=2,if(c.finMartesTarde<ce.finMartesTarde,c.finMartesTarde,ce.finMartesTarde),if((WEEKDAY(now()) +1)=3,if(c.finMiercolesTarde<ce.finMiercolesTarde,c.finMiercolesTarde,ce.finMiercolesTarde),if((WEEKDAY(now()) +1)=4,if(c.finJuevesTarde<ce.finJuevesTarde,c.finJuevesTarde,ce.finJuevesTarde),if((WEEKDAY(now()) +1)=5,if(c.finViernesTarde<ce.finViernesTarde,c.finViernesTarde,ce.finViernesTarde),if((WEEKDAY(now()) +1)=6,if(c.finSabadoTarde<ce.finSabadoTarde,c.finSabadoTarde,ce.finSabadoTarde),if((WEEKDAY(now()) +1)=7,if(c.finDomingoTarde<ce.finDomingoTarde,c.finDomingoTarde,ce.finDomingoTarde),null))))))) as finTarde,
      if((WEEKDAY(now()) +1)=1 and c.activoLunes=1,ce.activoLunes,if((WEEKDAY(now()) +1)=2 and c.activoMartes=1,ce.activoMartes,if((WEEKDAY(now()) +1)=3 and c.activoMiercoles=1,ce.activoMiercoles,if((WEEKDAY(now()) +1)=4 and c.activoJueves=1,ce.activoJueves,if((WEEKDAY(now()) +1)=5 and c.activoViernes=1,ce.activoViernes,if((WEEKDAY(now()) +1)=6 and c.activoSabado=1,ce.activoSabado,if((WEEKDAY(now()) +1)=7 and c.activoDomingo=1,ce.activoDomingo,0))))))) as activoMan,
      if((WEEKDAY(now()) +1)=1 and c.activoLunesTarde=1,ce.activoLunesTarde,if((WEEKDAY(now()) +1)=2 and c.activoMartesTarde=1,ce.activoMartesTarde,if((WEEKDAY(now()) +1)=3 and c.activoMiercolesTarde=1,ce.activoMiercolesTarde,if((WEEKDAY(now()) +1)=4 and c.activoJuevesTarde=1,ce.activoJuevesTarde,if((WEEKDAY(now()) +1)=5 and c.activoViernesTarde=1,ce.activoViernesTarde ,if((WEEKDAY(now()) +1)=6 and c.activoSabadoTarde=1,ce.activoSabadoTarde,if((WEEKDAY(now()) +1)=7 and c.activoDomingoTarde=1,ce.activoDomingoTarde,0))))))) as activoTarde
      from qo_establecimientos e 
      inner join qo_configuracion_est ce on (ce.idEstablecimiento=e.id) 
      inner join qo_configuracion c on (c.idGrupo=e.idGrupo) 
      inner join qo_pedidos p on (p.idEstablecimiento=e.id) 
      inner join qo_users u on (u.id=p.idUsuario) 
      WHERE e.idGrupo=:idGrupo and e.estado=1 GROUP BY p.idEstablecimiento ORDER BY `pedidos` DESC");
      $sql->bindValue(':idGrupo', $_GET['idGrupo']);  
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
    }
      exit();
  }else if (isset($_GET['homeAdmin']))
  {
    $sql = $dbConn->prepare("SELECT DISTINCT 0 as pedidos, if(isnull(f.id),0,1) as favorito,e.*,e.id as idEstablecimiento,if(ce.servicioActivo=1,'True','False') as servicioActivo,ce.pedidoMinimo,ce.tiempoEntrega,ce.numeroPedidosSoportado,
    if((WEEKDAY(now()) +1)=1 and hour(now())<17 and c.activoLunes=1 and ce.activoLunes=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioLunes>ce.inicioLunes,TIME_FORMAT(c.inicioLunes, '%H:%i'),TIME_FORMAT(ce.inicioLunes, '%H:%i')),' - ',if(c.finLunes<ce.finLunes,TIME_FORMAT(c.finLunes, '%H:%i'),TIME_FORMAT(ce.FinLunes, '%H:%i'))),if((WEEKDAY(now()) +1)=1 and hour(now())>=17 and c.activoLunesTarde=1 and ce.activoLunesTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioLunesTarde>ce.inicioLunesTarde,TIME_FORMAT(c.inicioLunesTarde, '%H:%i'),TIME_FORMAT(ce.inicioLunesTarde, '%H:%i')),' - ',if(c.finLunesTarde<ce.finLunesTarde,TIME_FORMAT(c.finLunesTarde, '%H:%i'),TIME_FORMAT(ce.FinLunesTarde, '%H:%i'))),if((WEEKDAY(now()) +1)=2 and hour(now())<17 and c.activoMartes=1 and ce.activoMartes=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioMartes>ce.inicioMartes,TIME_FORMAT(c.inicioMartes, '%H:%i'),TIME_FORMAT(ce.inicioMartes, '%H:%i')),' - ',if(c.finMartes<ce.finMartes,TIME_FORMAT(c.finMartes, '%H:%i'),TIME_FORMAT(ce.FinMartes, '%H:%i'))),if((WEEKDAY(now()) +1)=2 and hour(now())>=17 and c.activoMartesTarde=1 and ce.activoMartesTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioMartesTarde>ce.inicioMartesTarde,TIME_FORMAT(c.inicioMartesTarde, '%H:%i'),TIME_FORMAT(ce.inicioMartesTarde, '%H:%i')),' - ',if(c.finMartesTarde<ce.finMartesTarde,TIME_FORMAT(c.finMartesTarde, '%H:%i'),TIME_FORMAT(ce.FinMartesTarde, '%H:%i'))),if((WEEKDAY(now()) +1)=3 and hour(now())<17 and c.activoMiercoles=1 and ce.activoMiercoles=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioMiercoles>ce.inicioMiercoles,TIME_FORMAT(c.inicioMiercoles, '%H:%i'),TIME_FORMAT(ce.inicioMiercoles, '%H:%i')),' - ',if(c.finMiercoles<ce.finMiercoles,TIME_FORMAT(c.finMiercoles, '%H:%i'),TIME_FORMAT(ce.FinMiercoles, '%H:%i'))),if((WEEKDAY(now()) +1)=3 and hour(now())>=17 and c.activoMiercolesTarde=1 and ce.activoMiercolesTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioMiercolesTarde>ce.inicioMiercolesTarde,TIME_FORMAT(c.inicioMiercolesTarde, '%H:%i'),TIME_FORMAT(ce.inicioMiercolesTarde, '%H:%i')),' - ',if(c.finMiercolesTarde<ce.finMiercolesTarde,TIME_FORMAT(c.finMiercolesTarde, '%H:%i'),TIME_FORMAT(ce.FinMiercolesTarde, '%H:%i'))),if((WEEKDAY(now()) +1)=4 and hour(now())<17 and c.activoJueves=1 and ce.activoJueves=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioJueves>ce.inicioJueves,TIME_FORMAT(c.inicioJueves, '%H:%i'),TIME_FORMAT(ce.inicioJueves, '%H:%i')),' - ',if(c.finJueves<ce.finJueves,TIME_FORMAT(c.finJueves, '%H:%i'),TIME_FORMAT(ce.FinJueves, '%H:%i'))),if((WEEKDAY(now()) +1)=4 and hour(now())>=17 and c.activoJuevesTarde=1 and ce.activoJuevesTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioJuevesTarde>ce.inicioJuevesTarde,TIME_FORMAT(c.inicioJuevesTarde, '%H:%i'),TIME_FORMAT(ce.inicioJuevesTarde, '%H:%i')),' - ',if(c.finJuevesTarde<ce.finJuevesTarde,TIME_FORMAT(c.finJuevesTarde, '%H:%i'),TIME_FORMAT(ce.FinJuevesTarde, '%H:%i'))),if((WEEKDAY(now()) +1)=5 and hour(now())<17 and c.activoViernes=1 and ce.activoViernes=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioViernes>ce.inicioViernes,TIME_FORMAT(c.inicioViernes, '%H:%i'),TIME_FORMAT(ce.inicioViernes, '%H:%i')),' - ',if(c.finViernes<ce.finViernes,TIME_FORMAT(c.finViernes, '%H:%i'),TIME_FORMAT(ce.FinViernes, '%H:%i'))),if((WEEKDAY(now()) +1)=5 and hour(now())>=17 and c.activoViernesTarde=1 and ce.activoViernesTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioViernesTarde>ce.inicioViernesTarde,TIME_FORMAT(c.inicioViernesTarde, '%H:%i'),TIME_FORMAT(ce.inicioViernesTarde, '%H:%i')),' - ',if(c.finViernesTarde<ce.finViernesTarde,TIME_FORMAT(c.finViernesTarde, '%H:%i'),TIME_FORMAT(ce.FinViernesTarde, '%H:%i'))),if((WEEKDAY(now()) +1)=6 and hour(now())<17 and c.activoSabado=1 and ce.activoSabado=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioSabado>ce.inicioSabado,TIME_FORMAT(c.inicioSabado, '%H:%i'),TIME_FORMAT(ce.inicioSabado, '%H:%i')),' - ',if(c.finSabado<ce.finSabado,TIME_FORMAT(c.finSabado, '%H:%i'),TIME_FORMAT(ce.FinSabado, '%H:%i'))),if((WEEKDAY(now()) +1)=6 and hour(now())>=17 and c.activoSabadoTarde=1 and ce.activoSabadoTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioSabadoTarde>ce.inicioSabadoTarde,TIME_FORMAT(c.inicioSabadoTarde, '%H:%i'),TIME_FORMAT(ce.inicioSabadoTarde, '%H:%i')),' - ',if(c.finSabadoTarde<ce.finSabadoTarde,TIME_FORMAT(c.finSabadoTarde, '%H:%i'),TIME_FORMAT(ce.FinSabadoTarde, '%H:%i'))),if((WEEKDAY(now()) +1)=7 and hour(now())<17 and c.activoDomingo=1 and ce.activoDomingo=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioDomingo>ce.inicioDomingo,TIME_FORMAT(c.inicioDomingo, '%H:%i'),TIME_FORMAT(ce.inicioDomingo, '%H:%i')),' - ',if(c.finDomingo<ce.finDomingo,TIME_FORMAT(c.finDomingo, '%H:%i'),TIME_FORMAT(ce.FinDomingo, '%H:%i'))),if((WEEKDAY(now()) +1)=7 and hour(now())>=17 and c.activoDomingoTarde=1 and ce.activoDomingoTarde=1 and c.servicioActivo=1 and ce.servicioActivo=1,CONCAT(if(c.inicioDomingoTarde<ce.inicioDomingoTarde,TIME_FORMAT(c.inicioDomingoTarde, '%H:%i'),TIME_FORMAT(ce.inicioDomingoTarde, '%H:%i')),' - ',if(c.finDomingoTarde<ce.finDomingoTarde,TIME_FORMAT(c.finDomingoTarde, '%H:%i'),TIME_FORMAT(ce.FinDomingoTarde, '%H:%i'))),'CERRADO')))))))))))))) as horario,if((WEEKDAY(now()) +1)=1 and hour(now())<17,if(c.inicioLunes>ce.inicioLunes,c.inicioLunes,ce.inicioLunes),if((WEEKDAY(now()) +1)=1 and hour(now())>=17,if(c.inicioLunesTarde>ce.inicioLunesTarde,c.inicioLunesTarde,ce.inicioLunesTarde),if((WEEKDAY(now()) +1)=2 and hour(now())<17,if(c.inicioMartes>ce.inicioMartes,c.inicioMartes,ce.inicioMartes),if((WEEKDAY(now()) +1)=2 and hour(now())>=17,if(c.inicioMartesTarde>ce.inicioMartesTarde,c.inicioMartesTarde,ce.inicioMartesTarde),if((WEEKDAY(now()) +1)=3 and hour(now())<17,if(c.inicioMiercoles>ce.inicioMiercoles,c.inicioMiercoles,ce.inicioMiercoles),if((WEEKDAY(now()) +1)=3 and hour(now())>=17,if(c.inicioMiercolesTarde>ce.inicioMiercolesTarde,c.inicioMiercolesTarde,ce.inicioMiercolesTarde),if((WEEKDAY(now()) +1)=4 and hour(now())<17,if(c.inicioJueves>ce.inicioJueves,c.inicioJueves,ce.inicioJueves),if((WEEKDAY(now()) +1)=4 and hour(now())>=17,if(c.inicioJuevesTarde>ce.inicioJuevesTarde,c.inicioJuevesTarde,ce.inicioJuevesTarde),if((WEEKDAY(now()) +1)=5 and hour(now())<17,if(c.inicioViernes>ce.inicioViernes,c.inicioViernes,ce.inicioViernes),if((WEEKDAY(now()) +1)=5 and hour(now())>=17,if(c.inicioViernesTarde>ce.inicioViernesTarde,c.inicioViernesTarde,ce.inicioViernesTarde),if((WEEKDAY(now()) +1)=6 and hour(now())<17,if(c.inicioSabado>ce.inicioSabado,c.inicioSabado,ce.inicioSabado),if((WEEKDAY(now()) +1)=6 and hour(now())>=17,if(c.inicioSabadoTarde>ce.inicioSabadoTarde,c.inicioSabadoTarde,ce.inicioSabadoTarde),if((WEEKDAY(now()) +1)=7 and hour(now())<17,if(c.inicioDomingo>ce.inicioDomingo,c.inicioDomingo,ce.inicioDomingo),if((WEEKDAY(now()) +1)=7 and hour(now())>=17,if(c.inicioDomingoTarde>ce.inicioDomingoTarde,c.inicioDomingoTarde,ce.inicioDomingoTarde),null)))))))))))))) as inicioHoy,if((WEEKDAY(now()) +1)=1 and hour(now())<17,if(c.finLunes<ce.finLunes,c.finLunes,ce.finLunes),if((WEEKDAY(now()) +1)=1 and hour(now())>=17,if(c.finLunesTarde<ce.finLunesTarde,c.finLunesTarde,ce.finLunesTarde),if((WEEKDAY(now()) +1)=2 and hour(now())<17,if(c.finMartes<ce.finMartes,c.finMartes,ce.finMartes),if((WEEKDAY(now()) +1)=2 and hour(now())>=17,if(c.finMartesTarde<ce.finMartesTarde,c.finMartesTarde,ce.finMartesTarde),if((WEEKDAY(now()) +1)=3 and hour(now())<17,if(c.finMiercoles<ce.finMiercoles,c.finMiercoles,ce.finMiercoles),if((WEEKDAY(now()) +1)=3 and hour(now())>=17,if(c.finMiercolesTarde<ce.finMiercolesTarde,c.finMiercolesTarde,ce.finMiercolesTarde),if((WEEKDAY(now()) +1)=4 and hour(now())<17,if(c.finJueves<ce.finJueves,c.finJueves,ce.finJueves),if((WEEKDAY(now()) +1)=4 and hour(now())>=17,if(c.finJuevesTarde<ce.finJuevesTarde,c.finJuevesTarde,ce.finJuevesTarde),if((WEEKDAY(now()) +1)=5 and hour(now())<17,if(c.finViernes<ce.finViernes,c.finViernes,ce.finViernes),if((WEEKDAY(now()) +1)=5 and hour(now())>=17,if(c.finViernesTarde<ce.finViernesTarde,c.finViernesTarde,ce.finViernesTarde),if((WEEKDAY(now()) +1)=6 and hour(now())<17,if(c.finSabado<ce.finSabado,c.finSabado,ce.finSabado),if((WEEKDAY(now()) +1)=6 and hour(now())>=17,if(c.finSabadoTarde<ce.finSabadoTarde,c.finSabadoTarde,ce.finSabadoTarde),if((WEEKDAY(now()) +1)=7 and hour(now())<17,if(c.finDomingo<ce.finDomingo,c.finDomingo,ce.finDomingo),if((WEEKDAY(now()) +1)=7 and hour(now())>=17,if(c.finDomingoTarde<ce.finDomingoTarde,c.finDomingoTarde,ce.finDomingoTarde),null)))))))))))))) as finHoy,
    if(c.servicioActivo=0,'False',if(ce.servicioActivo=0,'False',if((WEEKDAY(now()) +1)=1 and (ce.activoLunes=1 or ce.activoLunesTarde=1) and (c.activoLunes=1 or c.activoLunesTarde=1),'True',if((WEEKDAY(now()) +1)=2 and (ce.activoMartes=1 or ce.activoMartesTarde=1) and (ce.activoMartes=1 or ce.activoMartesTarde=1),'True',if((WEEKDAY(now()) +1)=3 and (ce.activoMiercoles=1 or ce.activoMiercolesTarde=1) and (c.activoMiercoles=1 or c.activoMiercolesTarde=1),
      'True',if((WEEKDAY(now()) +1)=4 and (ce.activoJueves=1 or ce.activoJuevesTarde=1)  and (c.activoJueves=1 or c.activoJuevesTarde=1),'True',if((WEEKDAY(now()) +1)=5 and (ce.activoViernes=1 or ce.activoViernesTarde=1) and (c.activoViernes=1 or c.activoViernesTarde=1),'True',if((WEEKDAY(now()) +1)=6 and (ce.activoSabado=1 or ce.activoSabadoTarde=1) and (c.activoSabado=1 or c.activoSabadoTarde=1),
      'True',if((WEEKDAY(now()) +1)=7 and (ce.activoDomingo=1 or ce.activoDomingoTarde=1) and (c.activoDomingo=1 or c.activoDomingoTarde=1),'True','False'))))))))) as activoHoy,
    if((WEEKDAY(now()) +1)=1,if(c.inicioLunes>ce.inicioLunes,c.inicioLunes,ce.inicioLunes),if((WEEKDAY(now()) +1)=2,if(c.inicioMartes>ce.inicioMartes,c.inicioMartes,ce.inicioMartes),if((WEEKDAY(now()) +1)=3,if(c.inicioMiercoles>ce.inicioMiercoles,c.inicioMiercoles,ce.inicioMiercoles),if((WEEKDAY(now()) +1)=4,if(c.inicioJueves>ce.inicioJueves,c.inicioJueves,ce.inicioJueves),if((WEEKDAY(now()) +1)=5,if(c.inicioViernes>ce.inicioViernes,c.inicioViernes,ce.inicioViernes),if((WEEKDAY(now()) +1)=6,if(c.inicioSabado>ce.inicioSabado,c.inicioSabado,ce.inicioSabado),if((WEEKDAY(now()) +1)=7,if(c.inicioDomingo>ce.inicioDomingo,c.inicioDomingo,ce.inicioDomingo),null))))))) as inicioMan,
    if((WEEKDAY(now()) +1)=1,if(c.finLunes<ce.finLunes,c.finLunes,ce.finLunes),if((WEEKDAY(now()) +1)=2,if(c.finMartes<ce.finMartes,c.finMartes,ce.finMartes),if((WEEKDAY(now()) +1)=3,if(c.finMiercoles<ce.finMiercoles,c.finMiercoles,ce.finMiercoles),if((WEEKDAY(now()) +1)=4,if(c.finJueves<ce.finJueves,c.finJueves,ce.finJueves),if((WEEKDAY(now()) +1)=5,if(c.finViernes<ce.finViernes,c.finViernes,ce.finViernes),if((WEEKDAY(now()) +1)=6,if(c.finSabado<ce.finSabado,c.finSabado,ce.finSabado),if((WEEKDAY(now()) +1)=7,if(c.finDomingo<ce.finDomingo,c.finDomingo,ce.finDomingo),null))))))) as finMan,
    if((WEEKDAY(now()) +1)=1,if(c.inicioLunesTarde>ce.inicioLunesTarde,c.inicioLunesTarde,ce.inicioLunesTarde),if((WEEKDAY(now()) +1)=2,if(c.inicioMartesTarde>ce.inicioMartesTarde,c.inicioMartesTarde,ce.inicioMartesTarde),if((WEEKDAY(now()) +1)=3,if(c.inicioMiercolesTarde>ce.inicioMiercolesTarde,c.inicioMiercolesTarde,ce.inicioMiercolesTarde),if((WEEKDAY(now()) +1)=4,if(c.inicioJuevesTarde>ce.inicioJuevesTarde,c.inicioJuevesTarde,ce.inicioJuevesTarde),if((WEEKDAY(now()) +1)=5,if(c.inicioViernesTarde>ce.inicioViernesTarde,c.inicioViernesTarde,ce.inicioViernesTarde),if((WEEKDAY(now()) +1)=6,if(c.inicioSabadoTarde>ce.inicioSabadoTarde,c.inicioSabadoTarde,ce.inicioSabadoTarde),if((WEEKDAY(now()) +1)=7,if(c.inicioDomingoTarde>ce.inicioDomingoTarde,c.inicioDomingoTarde,ce.inicioDomingoTarde),null))))))) as inicioTarde,
    if((WEEKDAY(now()) +1)=1,if(c.finLunesTarde<ce.finLunesTarde,c.finLunesTarde,ce.finLunesTarde),if((WEEKDAY(now()) +1)=2,if(c.finMartesTarde<ce.finMartesTarde,c.finMartesTarde,ce.finMartesTarde),if((WEEKDAY(now()) +1)=3,if(c.finMiercolesTarde<ce.finMiercolesTarde,c.finMiercolesTarde,ce.finMiercolesTarde),if((WEEKDAY(now()) +1)=4,if(c.finJuevesTarde<ce.finJuevesTarde,c.finJuevesTarde,ce.finJuevesTarde),if((WEEKDAY(now()) +1)=5,if(c.finViernesTarde<ce.finViernesTarde,c.finViernesTarde,ce.finViernesTarde),if((WEEKDAY(now()) +1)=6,if(c.finSabadoTarde<ce.finSabadoTarde,c.finSabadoTarde,ce.finSabadoTarde),if((WEEKDAY(now()) +1)=7,if(c.finDomingoTarde<ce.finDomingoTarde,c.finDomingoTarde,ce.finDomingoTarde),null))))))) as finTarde,
    if((WEEKDAY(now()) +1)=1 and c.activoLunes=1,ce.activoLunes,if((WEEKDAY(now()) +1)=2 and c.activoMartes=1,ce.activoMartes,if((WEEKDAY(now()) +1)=3 and c.activoMiercoles=1,ce.activoMiercoles,if((WEEKDAY(now()) +1)=4 and c.activoJueves=1,ce.activoJueves,if((WEEKDAY(now()) +1)=5 and c.activoViernes=1,ce.activoViernes,if((WEEKDAY(now()) +1)=6 and c.activoSabado=1,ce.activoSabado,if((WEEKDAY(now()) +1)=7 and c.activoDomingo=1,ce.activoDomingo,0))))))) as activoMan,
    if((WEEKDAY(now()) +1)=1 and c.activoLunesTarde=1,ce.activoLunesTarde,if((WEEKDAY(now()) +1)=2 and c.activoMartesTarde=1,ce.activoMartesTarde,if((WEEKDAY(now()) +1)=3 and c.activoMiercolesTarde=1,ce.activoMiercolesTarde,if((WEEKDAY(now()) +1)=4 and c.activoJuevesTarde=1,ce.activoJuevesTarde,if((WEEKDAY(now()) +1)=5 and c.activoViernesTarde=1,ce.activoViernesTarde ,if((WEEKDAY(now()) +1)=6 and c.activoSabadoTarde=1,ce.activoSabadoTarde,if((WEEKDAY(now()) +1)=7 and c.activoDomingoTarde=1,ce.activoDomingoTarde,0))))))) as activoTarde
    from qo_establecimientos e 
    inner join qo_configuracion_est ce on (ce.idEstablecimiento=e.id) 
    inner join qo_configuracion c on (c.idGrupo=e.idGrupo) 
    Where e.idGrupo=:idGrupo and ((e.idPueblo=:idPueblo) or (e.idPueblo<>:idPueblo and e.visibleFuera=1))  ORDER BY favorito desc,orden");
    $sql->bindValue(':idGrupo', $_GET['idGrupo']);  
    $sql->bindValue(':idUsuario', $_GET['idUsuario']);  
    $sql->bindValue(':idPueblo', $_GET['idPueblo']);  
    $sql->execute();
    $sql->setFetchMode(PDO::FETCH_ASSOC);
    header("HTTP/1.1 200 OK");
    echo json_encode( $sql->fetchAll()  );
    exit();
  }else if (isset($_GET['homeAdmin2']))
  {
    $sql = $dbConn->prepare("SELECT DISTINCT 0 as pedidos, 0 as favorito,e.*,e.id as idEstablecimiento,if(ce.servicioActivo=1,'True','False') as servicioActivo,ce.pedidoMinimo,ce.tiempoEntrega,ce.numeroPedidosSoportado,
    if(c.servicioActivo=0,'False',if(ce.servicioActivo=0,'False',if((WEEKDAY(now()) +1)=1 and (ce.activoLunes=1 or ce.activoLunesTarde=1) and (c.activoLunes=1 or c.activoLunesTarde=1),'True',if((WEEKDAY(now()) +1)=2 and (ce.activoMartes=1 or ce.activoMartesTarde=1) and (ce.activoMartes=1 or ce.activoMartesTarde=1),'True',if((WEEKDAY(now()) +1)=3 and (ce.activoMiercoles=1 or ce.activoMiercolesTarde=1) and (c.activoMiercoles=1 or c.activoMiercolesTarde=1),
          'True',if((WEEKDAY(now()) +1)=4 and (ce.activoJueves=1 or ce.activoJuevesTarde=1)  and (c.activoJueves=1 or c.activoJuevesTarde=1),'True',if((WEEKDAY(now()) +1)=5 and (ce.activoViernes=1 or ce.activoViernesTarde=1) and (c.activoViernes=1 or c.activoViernesTarde=1),'True',if((WEEKDAY(now()) +1)=6 and (ce.activoSabado=1 or ce.activoSabadoTarde=1) and (c.activoSabado=1 or c.activoSabadoTarde=1),
          'True',if((WEEKDAY(now()) +1)=7 and (ce.activoDomingo=1 or ce.activoDomingoTarde=1) and (c.activoDomingo=1 or c.activoDomingoTarde=1),'True','False'))))))))) as activoHoy,
        if((WEEKDAY(now()) +1)=1,ce.inicioLunes,if ((WEEKDAY(now()) +1)=2,ce.inicioMartes,if((WEEKDAY(now()) +1)=3,ce.inicioMiercoles,if ((WEEKDAY(now()) +1)=4,ce.inicioJueves,if ((WEEKDAY(now()) +1)=5,ce.inicioViernes,if ((WEEKDAY(now()) +1)=6,ce.inicioSabado,ce.inicioDomingo)))))) as inicioMan,
        if((WEEKDAY(now()) +1)=1,ce.finLunes,if ((WEEKDAY(now()) +1)=2,ce.finMartes,if((WEEKDAY(now()) +1)=3,ce.finMiercoles,if ((WEEKDAY(now()) +1)=4,ce.finJueves,if ((WEEKDAY(now()) +1)=5,ce.finViernes,if ((WEEKDAY(now()) +1)=6,ce.finSabado,ce.finDomingo)))))) as finMan,
        if((WEEKDAY(now()) +1)=1,ce.inicioLunesTarde,if ((WEEKDAY(now()) +1)=2,ce.inicioMartesTarde,if((WEEKDAY(now()) +1)=3,ce.inicioMiercolesTarde,if ((WEEKDAY(now()) +1)=4,ce.inicioJuevesTarde,if ((WEEKDAY(now()) +1)=5,ce.inicioViernesTarde,if ((WEEKDAY(now()) +1)=6,ce.inicioSabadoTarde,ce.inicioDomingoTarde)))))) as inicioTarde,
        if((WEEKDAY(now()) +1)=1,ce.finLunesTarde,if ((WEEKDAY(now()) +1)=2,ce.finMartesTarde,if((WEEKDAY(now()) +1)=3,ce.finMiercolesTarde,if ((WEEKDAY(now()) +1)=4,ce.finJuevesTarde,if ((WEEKDAY(now()) +1)=5,ce.finViernesTarde,if ((WEEKDAY(now()) +1)=6,ce.finSabadoTarde,ce.finDomingoTarde)))))) as finTarde,
        if((WEEKDAY(now()) +1)=1 and c.activoLunes=1,ce.activoLunes,if((WEEKDAY(now()) +1)=2 and c.activoMartes=1,ce.activoMartes,if((WEEKDAY(now()) +1)=3 and c.activoMiercoles=1,ce.activoMiercoles,if((WEEKDAY(now()) +1)=4 and c.activoJueves=1,ce.activoJueves,if((WEEKDAY(now()) +1)=5 and c.activoViernes=1,ce.activoViernes,if((WEEKDAY(now()) +1)=6 and c.activoSabado=1,ce.activoSabado,if((WEEKDAY(now()) +1)=7 and c.activoDomingo=1,ce.activoDomingo,0))))))) as activoMan,
        if((WEEKDAY(now()) +1)=1 and c.activoLunesTarde=1,ce.activoLunesTarde,if((WEEKDAY(now()) +1)=2 and c.activoMartesTarde=1,ce.activoMartesTarde,if((WEEKDAY(now()) +1)=3 and c.activoMiercolesTarde=1,ce.activoMiercolesTarde,if((WEEKDAY(now()) +1)=4 and c.activoJuevesTarde=1,ce.activoJuevesTarde,if((WEEKDAY(now()) +1)=5 and c.activoViernesTarde=1,ce.activoViernesTarde ,if((WEEKDAY(now()) +1)=6 and c.activoSabadoTarde=1,ce.activoSabadoTarde,if((WEEKDAY(now()) +1)=7 and c.activoDomingoTarde=1,ce.activoDomingoTarde,0))))))) as activoTarde
        from qo_establecimientos e 
        inner join qo_configuracion_est ce on (ce.idEstablecimiento=e.id) 
        inner join qo_configuracion c on (c.idGrupo=e.idGrupo) 
    Where e.idGrupo=:idGrupo and ((e.idPueblo=:idPueblo) or (e.idPueblo<>:idPueblo and e.visibleFuera=1)) ORDER BY favorito desc,orden");
    $sql->bindValue(':idGrupo', $_GET['idGrupo']);  
    $sql->bindValue(':idUsuario', $_GET['idUsuario']);  
    $sql->bindValue(':idPueblo', $_GET['idPueblo']);  
    $sql->execute();
    $sql->setFetchMode(PDO::FETCH_ASSOC);
    header("HTTP/1.1 200 OK");
    echo json_encode( $sql->fetchAll()  );
    exit();
  }else if (isset($_GET['mesas']))
  {
      $sql = $dbConn->prepare("SELECT 0 as favorito,e.*,e.id as idEstablecimiento,if(ce.servicioActivo=1,'True','False') as servicioActivo,ce.pedidoMinimo,ce.tiempoEntrega,ce.numeroPedidosSoportado,
      if((WEEKDAY(now()) +1)=1 and hour(now())<17 and ce.activoLunesLocal=1 and ce.servicioActivo=1,
      CONCAT(TIME_FORMAT(ce.inicioLunesLocal, '%H:%i'),' - ',TIME_FORMAT(ce.FinLunesLocal, '%H:%i')),if((WEEKDAY(now()) +1)=1 and hour(now())>=17 and ce.activoLunesTardeLocal=1 and ce.servicioActivo=1,
      CONCAT(TIME_FORMAT(ce.inicioLunesTardeLocal, '%H:%i'),' - ',TIME_FORMAT(ce.FinLunesTardeLocal, '%H:%i')),if((WEEKDAY(now()) +1)=2 and hour(now())<17 and ce.activoMartesLocal=1 and ce.servicioActivo=1,
      CONCAT(TIME_FORMAT(ce.inicioMartesLocal, '%H:%i'),' - ',TIME_FORMAT(ce.FinMartesLocal, '%H:%i')),if((WEEKDAY(now()) +1)=2 and hour(now())>=17 and ce.activoMartesTardeLocal=1 and ce.servicioActivo=1,
      CONCAT(TIME_FORMAT(ce.inicioMartesTardeLocal, '%H:%i'),' - ',TIME_FORMAT(ce.FinMartesTardeLocal, '%H:%i')),if((WEEKDAY(now()) +1)=3 and hour(now())<17 and ce.activoMiercolesLocal=1 and ce.servicioActivo=1,
      CONCAT(TIME_FORMAT(ce.inicioMiercolesLocal, '%H:%i'),' - ',TIME_FORMAT(ce.FinMiercolesLocal, '%H:%i')),if((WEEKDAY(now()) +1)=3 and hour(now())>=17 and ce.activoMiercolesTardeLocal=1 and ce.servicioActivo=1,
      CONCAT(TIME_FORMAT(ce.inicioMiercolesTardeLocal, '%H:%i'),' - ',TIME_FORMAT(ce.FinMiercolesTardeLocal, '%H:%i')),if((WEEKDAY(now()) +1)=4 and hour(now())<17 and ce.activoJuevesLocal=1 and ce.servicioActivo=1,
      CONCAT(TIME_FORMAT(ce.inicioJuevesLocal, '%H:%i'),' - ',TIME_FORMAT(ce.FinJuevesLocal, '%H:%i')),if((WEEKDAY(now()) +1)=4 and hour(now())>=17 and ce.activoJuevesTardeLocal=1 and ce.servicioActivo=1,
      CONCAT(TIME_FORMAT(ce.inicioJuevesTardeLocal, '%H:%i'),' - ',TIME_FORMAT(ce.FinJuevesTardeLocal, '%H:%i')),if((WEEKDAY(now()) +1)=5 and hour(now())<17 and ce.activoViernesLocal=1 and ce.servicioActivo=1,
      CONCAT(TIME_FORMAT(ce.inicioViernesLocal, '%H:%i'),' - ',TIME_FORMAT(ce.FinViernesLocal, '%H:%i')),if((WEEKDAY(now()) +1)=5 and hour(now())>=17 and ce.activoViernesTarde=1 and ce.servicioActivo=1,
      CONCAT(TIME_FORMAT(ce.inicioViernesTardeLocal, '%H:%i'),' - ',TIME_FORMAT(ce.FinViernesTardeLocal, '%H:%i')),if((WEEKDAY(now()) +1)=6 and hour(now())<17 and ce.activoSabadoLocal=1 and ce.servicioActivo=1,
      CONCAT(TIME_FORMAT(ce.inicioSabadoLocal, '%H:%i'),' - ',TIME_FORMAT(ce.FinSabadoLocal, '%H:%i')),if((WEEKDAY(now()) +1)=6 and hour(now())>=17 and ce.activoSabadoTardeLocal=1 and ce.servicioActivo=1,
      CONCAT(TIME_FORMAT(ce.inicioSabadoTardeLocal, '%H:%i'),' - ',TIME_FORMAT(ce.FinSabadoTardeLocal, '%H:%i')),if((WEEKDAY(now()) +1)=7 and hour(now())<17 and ce.activoDomingoLocal=1 and ce.servicioActivo=1,
      CONCAT(TIME_FORMAT(ce.inicioDomingoLocal, '%H:%i'),' - ',TIME_FORMAT(ce.FinDomingoLocal, '%H:%i')),if((WEEKDAY(now()) +1)=7 and hour(now())>=17 and ce.activoDomingoTardeLocal=1 and ce.servicioActivo=1,
      CONCAT(TIME_FORMAT(ce.inicioDomingoTardeLocal, '%H:%i'),' - ',TIME_FORMAT(ce.FinDomingoTardeLocal, '%H:%i')),'CERRADO')))))))))))))) as horario,if((WEEKDAY(now()) +1)=1 and hour(now())<17,ce.inicioLunesLocal,if((WEEKDAY(now()) +1)=1 and hour(now())>=17,ce.inicioLunesTardeLocal,if((WEEKDAY(now()) +1)=2 and hour(now())<17,ce.inicioMartesLocal,if((WEEKDAY(now()) +1)=2 and hour(now())>=17,ce.inicioMartesTardeLocal,if((WEEKDAY(now()) +1)=3 and hour(now())<17,ce.inicioMiercolesLocal,if((WEEKDAY(now()) +1)=3 and hour(now())>=17,ce.inicioMiercolesTardeLocal,if((WEEKDAY(now()) +1)=4 and hour(now())<17,ce.inicioJuevesLocal,if((WEEKDAY(now()) +1)=4 and hour(now())>=17,ce.inicioJuevesTardeLocal,if((WEEKDAY(now()) +1)=5 and hour(now())<17,ce.inicioViernesLocal,if((WEEKDAY(now()) +1)=5 and hour(now())>=17,ce.inicioViernesTardeLocal,if((WEEKDAY(now()) +1)=6 and hour(now())<17,ce.inicioSabadoLocal,if((WEEKDAY(now()) +1)=6 and hour(now())>=17,ce.inicioSabadoTardeLocal,if((WEEKDAY(now()) +1)=7 and hour(now())<17,ce.inicioDomingoLocal,if((WEEKDAY(now()) +1)=7 and hour(now())>=17,ce.inicioDomingoTardeLocal,null)))))))))))))) as inicioHoy,if((WEEKDAY(now()) +1)=1 and hour(now())<17,ce.finLunesLocal,if((WEEKDAY(now()) +1)=1 and hour(now())>=17,ce.finLunesTardeLocal,if((WEEKDAY(now()) +1)=2 and hour(now())<17,ce.finMartesLocal,if((WEEKDAY(now()) +1)=2 and hour(now())>=17,ce.finMartesTardeLocal,if((WEEKDAY(now()) +1)=3 and hour(now())<17,ce.finMiercolesLocal,if((WEEKDAY(now()) +1)=3 and hour(now())>=17,ce.finMiercolesTardeLocal,if((WEEKDAY(now()) +1)=4 and hour(now())<17,ce.finJuevesLocal,if((WEEKDAY(now()) +1)=4 and hour(now())>=17,ce.finJuevesTardeLocal,if((WEEKDAY(now()) +1)=5 and hour(now())<17,ce.finViernesLocal,if((WEEKDAY(now()) +1)=5 and hour(now())>=17,ce.finViernesTardeLocal,if((WEEKDAY(now()) +1)=6 and hour(now())<17,ce.finSabadoLocal,if((WEEKDAY(now()) +1)=6 and hour(now())>=17,ce.finSabadoTardeLocal,if((WEEKDAY(now()) +1)=7 and hour(now())<17,ce.finDomingoLocal,if((WEEKDAY(now()) +1)=7 and hour(now())>=17,ce.finDomingoTardeLocal,null)))))))))))))) as finHoy,
      if(c.servicioActivo=0,'False',if(ce.servicioActivo=0,'False',if((WEEKDAY(now()) +1)=1 and (ce.activoLunes=1 or ce.activoLunesTarde=1) and (c.activoLunes=1 or c.activoLunesTarde=1),'True',if((WEEKDAY(now()) +1)=2 and (ce.activoMartes=1 or ce.activoMartesTarde=1) and (ce.activoMartes=1 or ce.activoMartesTarde=1),'True',if((WEEKDAY(now()) +1)=3 and (ce.activoMiercoles=1 or ce.activoMiercolesTarde=1) and (c.activoMiercoles=1 or c.activoMiercolesTarde=1),
          'True',if((WEEKDAY(now()) +1)=4 and (ce.activoJueves=1 or ce.activoJuevesTarde=1)  and (c.activoJueves=1 or c.activoJuevesTarde=1),'True',if((WEEKDAY(now()) +1)=5 and (ce.activoViernes=1 or ce.activoViernesTarde=1) and (c.activoViernes=1 or c.activoViernesTarde=1),'True',if((WEEKDAY(now()) +1)=6 and (ce.activoSabado=1 or ce.activoSabadoTarde=1) and (c.activoSabado=1 or c.activoSabadoTarde=1),
          'True',if((WEEKDAY(now()) +1)=7 and (ce.activoDomingo=1 or ce.activoDomingoTarde=1) and (c.activoDomingo=1 or c.activoDomingoTarde=1),'True','False'))))))))) as activoHoy,
        if((WEEKDAY(now()) +1)=1,ce.inicioLunes,if ((WEEKDAY(now()) +1)=2,ce.inicioMartes,if((WEEKDAY(now()) +1)=3,ce.inicioMiercoles,if ((WEEKDAY(now()) +1)=4,ce.inicioJueves,if ((WEEKDAY(now()) +1)=5,ce.inicioViernes,if ((WEEKDAY(now()) +1)=6,ce.inicioSabado,ce.inicioDomingo)))))) as inicioMan,
        if((WEEKDAY(now()) +1)=1,ce.finLunes,if ((WEEKDAY(now()) +1)=2,ce.finMartes,if((WEEKDAY(now()) +1)=3,ce.finMiercoles,if ((WEEKDAY(now()) +1)=4,ce.finJueves,if ((WEEKDAY(now()) +1)=5,ce.finViernes,if ((WEEKDAY(now()) +1)=6,ce.finSabado,ce.finDomingo)))))) as finMan,
        if((WEEKDAY(now()) +1)=1,ce.inicioLunesTarde,if ((WEEKDAY(now()) +1)=2,ce.inicioMartesTarde,if((WEEKDAY(now()) +1)=3,ce.inicioMiercolesTarde,if ((WEEKDAY(now()) +1)=4,ce.inicioJuevesTarde,if ((WEEKDAY(now()) +1)=5,ce.inicioViernesTarde,if ((WEEKDAY(now()) +1)=6,ce.inicioSabadoTarde,ce.inicioDomingoTarde)))))) as inicioTarde,
        if((WEEKDAY(now()) +1)=1,ce.finLunesTarde,if ((WEEKDAY(now()) +1)=2,ce.finMartesTarde,if((WEEKDAY(now()) +1)=3,ce.finMiercolesTarde,if ((WEEKDAY(now()) +1)=4,ce.finJuevesTarde,if ((WEEKDAY(now()) +1)=5,ce.finViernesTarde,if ((WEEKDAY(now()) +1)=6,ce.finSabadoTarde,ce.finDomingoTarde)))))) as finTarde,
        if((WEEKDAY(now()) +1)=1 and c.activoLunes=1,ce.activoLunes,if((WEEKDAY(now()) +1)=2 and c.activoMartes=1,ce.activoMartes,if((WEEKDAY(now()) +1)=3 and c.activoMiercoles=1,ce.activoMiercoles,if((WEEKDAY(now()) +1)=4 and c.activoJueves=1,ce.activoJueves,if((WEEKDAY(now()) +1)=5 and c.activoViernes=1,ce.activoViernes,if((WEEKDAY(now()) +1)=6 and c.activoSabado=1,ce.activoSabado,if((WEEKDAY(now()) +1)=7 and c.activoDomingo=1,ce.activoDomingo,0))))))) as activoMan,
        if((WEEKDAY(now()) +1)=1 and c.activoLunesTarde=1,ce.activoLunesTarde,if((WEEKDAY(now()) +1)=2 and c.activoMartesTarde=1,ce.activoMartesTarde,if((WEEKDAY(now()) +1)=3 and c.activoMiercolesTarde=1,ce.activoMiercolesTarde,if((WEEKDAY(now()) +1)=4 and c.activoJuevesTarde=1,ce.activoJuevesTarde,if((WEEKDAY(now()) +1)=5 and c.activoViernesTarde=1,ce.activoViernesTarde ,if((WEEKDAY(now()) +1)=6 and c.activoSabadoTarde=1,ce.activoSabadoTarde,if((WEEKDAY(now()) +1)=7 and c.activoDomingoTarde=1,ce.activoDomingoTarde,0))))))) as activoTarde
      from qo_establecimientos e inner join qo_configuracion_est ce on (ce.idEstablecimiento=e.id) inner join qo_configuracion c on (e.idPueblo=c.idPueblo) Where e.idGrupo=:id and ((e.idPueblo=:idPueblo) or (e.idPueblo<>:idPueblo and e.visibleFuera=1)) and e.local=1 ORDER BY orden");
      $sql->bindValue(':id', $_GET['idGrupo']);  
      $sql->bindValue(':idPueblo', $_GET['idPueblo']);  
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
      exit();
  }else if (isset($_GET['idEstablecimientoHistorico']))
  {
      $sql = $dbConn->prepare("SELECT DISTINCT if(p.estado=1,'#000000',if(p.estado=2,'#4fa9d2',if (p.estado=3,'#ff5800',if(p.estado=4,'#0fa046','#4d4fa0')))) as ColorPedido,p.repartidor,p.idRepartidor,if(isnull(re.foto),'',re.foto) as fotoRepartidor,p.tipoPago,p.tipoVenta,if(isnull(p.horaEntrega) or p.horaEntrega='0000-00-00 00:00:00',p.horaPedido,p.horaEntrega) as horaEntrega,concat(u.nombre,' ',u.apellidos) as nombreUsuario,if (isnull(p.direccion) or p.direccion='',u.direccion,p.direccion) as direccionUsuario,if (isnull(p.idZona) or p.idZona=0,u.idZona,p.idZona) as idZona,u.email as emailUsuario,u.telefono as telefonoUsuario,p.nuevoPedido,esta.nombre as nombreEstablecimiento,p.id as idPedido,p.codigo as codigoPedido,p.idUsuario,p.idEstablecimiento,p.horaPedido,p.comentario,p.estado as idEstadoPedido,est.nombre as estadoPedido,d.total as precioTotalPedido FROM `qo_pedidos` p inner join qo_establecimientos esta on (esta.id=p.idEstablecimiento) inner join qo_estados est on (p.estado=est.id) inner join qo_users u on (p.idUsuario=u.id) left join qo_repartidores re on (re.id=p.idRepartidor) inner join (select idPedido,sum(cantidad*precio) total from qo_pedidos_detalle GROUP by idPedido ) d on (d.idPedido=p.id) 
      Where p.estado>=4 and p.anulado=0 and p.idEstablecimiento=:id and esta.idGrupo=:idGrupo and ((esta.idPueblo=:idPueblo) or (esta.idPueblo<>:idPueblo and esta.visibleFuera=1)) order by p.id");
      $sql->bindValue(':id', $_GET['idEstablecimientoHistorico']);  
      $sql->bindValue(':idGrupo', $_GET['idGrupo']);  
      $sql->bindValue(':idPueblo', $_GET['idPueblo']);  
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
      exit();
  }else if (isset($_GET['combos']))
  {
      $sql = $dbConn->prepare("SELECT * FROM `qo_combo_entrantes`");
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
      exit();
  }else if (isset($_GET['idEstablecimientoHistorico2']))
  {
      $sql = $dbConn->prepare("select p.id,p.codigo,p.tipoVenta,p.tipoPago,(d.cantidad *d.precio) as precio,d.tipo,d.pagadoConPuntos from qo_pedidos p inner join qo_pedidos_detalle d on (d.idPedido=p.id) where facturado=0 and p.estado>=4 and p.anulado=0 and p.idEstablecimiento=:id and date(p.horaPedido) between :desde and :hasta order by p.id");
      $sql->bindValue(':id', $_GET['idEstablecimientoHistorico2']);  
      $sql->bindValue(':desde', $_GET['desde']);  
      $sql->bindValue(':hasta', $_GET['hasta']);  
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
      exit();
  }else if (isset($_GET['idEstableicimientoZonas']))
  {
      $sql = $dbConn->prepare("SELECT * from qo_establecimientos_zonas WHERE idEstablecimiento=:id order by nombre");
      $sql->bindValue(':id', $_GET['idEstableicimientoZonas']);  
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
      exit();
  }else if (isset($_GET['idEstablecimientoToken']))
  {
      $sql = $dbConn->prepare("SELECT u.token from qo_users u inner join qo_users_est est on (est.idUser=u.id) where est.idEstablecimiento=:id");
      $sql->bindValue(':id', $_GET['idEstablecimientoToken']);  
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
      exit();
  }else if (isset($_GET['idEstablecimientoTienePedido']))
  {
      $sql = $dbConn->prepare("SELECT count(*) resultado from qo_pedidos WHERE idEstablecimiento=:id and 1>1");
      $sql->bindValue(':id', $_GET['idEstablecimientoTienePedido']);  
      $sql->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetch(PDO::FETCH_ASSOC)  );
      exit();
  }else if (isset($_GET['puebloCliente']))
  {
      $sql = $dbConn->prepare("SELECT gastos from qo_union_pueblos WHERE idPuebloOrigen=:idOrigen and idPuebloDestino=:idDestino and estado=1");
      $sql->bindValue(':idOrigen', $_GET['puebloEstablecimiento']);  
      $sql->bindValue(':idDestino', $_GET['puebloCliente']);  
      $sql->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetch(PDO::FETCH_ASSOC)  );
      exit();
  }else if (isset($_GET['idEstablecimientoFranja']))
  {
      $sql = $dbConn->prepare("SELECT count(*) resultado FROM `qo_pedidos` WHERE horaEntrega>=:desde and horaEntrega<=:hasta and tipoVenta<>'Encargo' and idEstablecimiento=:id");
      $sql->bindValue(':id', $_GET['idEstablecimientoFranja']);  
      $sql->bindValue(':desde', $_GET['desde']);  
      $sql->bindValue(':hasta', $_GET['hasta']);  
      $sql->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetch(PDO::FETCH_ASSOC)  );
      exit();
  }else if (isset($_GET['idEstablecimientoFranjaOtroPueblo']))
  {
      $sql = $dbConn->prepare("select DATE_FORMAT(ph.hora,'%H:%i') as horaInicio,ph.hora as horaFin,DATE_ADD(ph.hora,INTERVAL -(c.tiempoEntrega+20) MINUTE) as horaInicioReal,'#5d38bc' as color from qo_pueblos_horarios ph inner join qo_establecimientos e on (e.idPueblo=ph.idPuebloOrigen) inner join qo_configuracion_est c on (c.idEstablecimiento=e.id) Where ph.idPuebloOrigen=:desde and ph.idPuebloDestino=:hasta and ph.dia=DAYOFWEEK(now()) and ph.hora>TIME(DATE_ADD(time(:inicio),INTERVAL -c.tiempoEntrega MINUTE)) and ph.hora<=time(:fin) and e.id=:id order by ph.hora");
      $sql->bindValue(':id', $_GET['idEstablecimientoFranjaOtroPueblo']);  
      $sql->bindValue(':desde', $_GET['origen']);  
      $sql->bindValue(':hasta', $_GET['destino']); 
      $sql->bindValue(':inicio', $_GET['inicio']); 
      $sql->bindValue(':fin', $_GET['fin']);  
      $sql->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
      exit();
  }else if (isset($_GET['idEstablecimientoFranjaEncargo']))
  {
      $sql = $dbConn->prepare("SELECT count(*) resultado FROM `qo_pedidos` WHERE horaEntrega>=:desde and horaEntrega<=:hasta and tipoVenta='Encargo' and idEstablecimiento=:id");
      $sql->bindValue(':id', $_GET['idEstablecimientoFranjaEncargo']);  
      $sql->bindValue(':desde', $_GET['desde']);  
      $sql->bindValue(':hasta', $_GET['hasta']);  
      $sql->execute();
      header("HTTP/1.1 200 OK");
      echo json_encode(  $sql->fetch(PDO::FETCH_ASSOC)  );
      exit();
  }else if (isset($_GET['todos']))
  {
    $sql = $dbConn->prepare("SELECT id as idEstablecimiento,nombre,visibleFuera,idGrupo from qo_establecimientos WHERE idPueblo=:idPueblo order by nombre");
      $sql->bindValue(':idPueblo', $_GET['idPueblo']);
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
      exit();
  }else if (isset($_GET['pueblo']))
  {
    $sql = $dbConn->prepare("SELECT id as idEstablecimiento,nombre,visibleFuera,idGrupo from qo_establecimientos WHERE idPueblo=:idPueblo order by nombre");
      $sql->bindValue(':idPueblo', $_GET['idPueblo']);
      $sql->execute();
      $sql->setFetchMode(PDO::FETCH_ASSOC);
      header("HTTP/1.1 200 OK");
      echo json_encode( $sql->fetchAll()  );
      exit();
  }else
  {
    if ($_GET['idGrupo']==0)
      $sql = $dbConn->prepare("SELECT id as idEstablecimiento,nombre,visibleFuera,idGrupo,estado from qo_establecimientos order by nombre");
    else{
      $sql = $dbConn->prepare("SELECT id as idEstablecimiento,nombre,visibleFuera,idGrupo,estado from qo_establecimientos WHERE idGrupo=:idGrupo order by nombre");
      $sql->bindValue(':idGrupo', $_GET['idGrupo']);
    }
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
    if (isset($_GET['visitas']))
    {
      $sql="INSERT INTO qo_establecimientos_visitas (idUsuario,idEstablecimiento,modo) VALUES (:idUsuario,:idEstablecimiento,:modo)";
      $statement = $dbConn->prepare($sql);
      $statement->bindValue(':idUsuario', $input['idUsuario']);
      $statement->bindValue(':idEstablecimiento', $input['idEstablecimiento']);
      $statement->bindValue(':modo', $input['modo']);
      $statement->execute();
      $postId = $dbConn->lastInsertId();
    }else if (isset($_GET['fiscal']))
    {
      $sql="INSERT INTO `qo_establecimientos_fiscal`(`razonSocial`, `direccion`, `cp`, `poblacion`, `provincia`, `telefono`, `cif`, `idEstablecimiento`, `iban`) VALUES 
        (:razonSocial,:direccion,:cp,:poblacion,:provincia,:telefono,:cif,:idEstablecimiento,:iban)";
      $statement = $dbConn->prepare($sql);
      $statement->bindValue(':razonSocial', $input['razonSocial']);
      $statement->bindValue(':direccion', $input['direccion']);
      $statement->bindValue(':cp', $input['cp']);
      $statement->bindValue(':poblacion', $input['poblacion']);
      $statement->bindValue(':provincia', $input['provincia']);
      $statement->bindValue(':telefono', $input['telefono']);
      $statement->bindValue(':cif', $input['cif']);
      $statement->bindValue(':idEstablecimiento', $input['idEstablecimiento']);
      $statement->bindValue(':iban', $input['iban']);
      $statement->execute();
      $postId = $dbConn->lastInsertId();
    }else if (isset($_GET['zona']))
    {
      $sql="INSERT INTO qo_establecimientos_zonas (idEstablecimiento,nombre,activo) VALUES (:idEstablecimiento,:nombre,:activo)";
      $statement = $dbConn->prepare($sql);
      $statement->bindValue(':idEstablecimiento', $input['idEstablecimiento']);
      $statement->bindValue(':nombre', $input['nombre']);
      $statement->bindValue(':activo', $input['activo']);
      $statement->execute();
      $postId = $dbConn->lastInsertId();
    }else if (isset($_GET['valoraciones']))
    {
      $sql="INSERT INTO qo_establecimientos_valoraciones (idUsuario,idEstablecimiento,valoracion,observaciones) VALUES (:idUsuario,:idEstablecimiento,:valoracion,:observaciones)";
      $statement = $dbConn->prepare($sql);
      $statement->bindValue(':idUsuario', $input['idUsuario']);
      $statement->bindValue(':idEstablecimiento', $input['idEstablecimiento']);
      $statement->bindValue(':valoracion', $input['valoracion']);
      $statement->bindValue(':observaciones', $input['observaciones']);
      $statement->execute();
      $postId = $dbConn->lastInsertId();
    }else if (isset($_GET['password'])){
      $sql = "INSERT INTO `qo_establecimientos` (web,llevaAMesa,recogeEnBarra,telefono2,whatsapp,emailContacto,idPueblo,idGrupo,eSComercio,idZona,orden,logo,imagen,idCategoria,nombre, direccion, poblacion, provincia, codPostal, tipo, estado,telefono,email,latitud,longitud,envio,recogida,puedeReservar) VALUES (:web,:llevaAMesa,:recogeEnBarra,:telefono2,:whatsapp,:emailContacto,:idPueblo,:idGrupo,:esComercio,:idZona,:orden,:logo,:imagen,:idCategoria,:nombre,:direccion,:poblacion,:provincia,:codPostal,3,:estado,:telefono,:email,:latitud,:longitud,:envio,:recogida,:puedeReservar)";
      $statement = $dbConn->prepare($sql);
      $statement->bindValue(':idPueblo', $input['idPueblo']);
      $statement->bindValue(':esComercio', $input['esComercio']);
      if ($input['idPueblo']==1){
        $statement->bindValue(':idZona', 1);
        $statement->bindValue(':idGrupo', 1);
      }else if ($input['idPueblo']==2){
        $statement->bindValue(':idZona', 10);
        $statement->bindValue(':idGrupo', 2);
      }else if ($input['idPueblo']==7){
        $statement->bindValue(':idZona', 29);
        $statement->bindValue(':idGrupo', 3);
      }else if ($input['idPueblo']==8){
        $statement->bindValue(':idZona', 30);
        $statement->bindValue(':idGrupo', 4);
      }else{
        $statement->bindValue(':idZona', $input['idZona']);
        $statement->bindValue(':idGrupo', $input['idGrupo']);
      }
      $statement->bindValue(':orden', $input['orden']);
      $statement->bindValue(':logo', $input['logo']);
      $statement->bindValue(':imagen', $input['imagen']);
      $statement->bindValue(':idCategoria', $input['idCategoria']);
      $statement->bindValue(':nombre', $input['nombre']);
      $statement->bindValue(':direccion', $input['direccion']);
      $statement->bindValue(':poblacion', $input['poblacion']);
      $statement->bindValue(':provincia', $input['provincia']);
      $statement->bindValue(':esComercio', $input['esComercio']);
      $statement->bindValue(':codPostal', $input['codPostal']);
      $statement->bindValue(':estado', $input['estado']);
      $statement->bindValue(':llevaAMesa', $input['llevaAMesa']);
      $statement->bindValue(':recogeEnBarra', $input['recogeEnBarra']);
      if ($input['telefono']==null)
        $statement->bindValue(':telefono', '');
      else
        $statement->bindValue(':telefono', $input['telefono']);
        if ($input['telefono2']==null)
        $statement->bindValue(':telefono2', '');
      else
        $statement->bindValue(':telefono2', $input['telefono2']);
      if ($input['whatsapp']==null)
        $statement->bindValue(':whatsapp', '');
      else
        $statement->bindValue(':whatsapp', $input['whatsapp']);
      if ($input['web']==null)
        $statement->bindValue(':web', '');
      else
        $statement->bindValue(':web', $input['web']);

      if ($input['emailContacto']==null)
        $statement->bindValue(':emailContacto', '');
      else
        $statement->bindValue(':emailContacto', $input['emailContacto']);

      $statement->bindValue(':envio', $input['envio']);
      $statement->bindValue(':recogida', $input['recogida']);
      $statement->bindValue(':puedeReservar', $input['puedeReservar']);
      $statement->bindValue(':email', $input['email']);
      $statement->bindValue(':latitud', $input['latitud']);
      $statement->bindValue(':longitud', $input['longitud']);
      //bindAllValues($statement, $input);
      $statement->execute();
      $postId = $dbConn->lastInsertId();
      
      if($postId)
      {
        
        $sql="INSERT INTO `qo_users`(idPueblo,`nombre`, `apellidos`, `cod_postal`, `poblacion`, `provincia`, `direccion`, `fechaNacimiento`, `fechaAlta`, 
        `telefono`, `email`, `password`, `username`, `foto`, `rol`, `estado`, `plataforma`, `token`, `tipoRegistro`, `demo`, `pin`, `verificado`, `idZona`, `version`) 
        VALUES (:idPueblo,:nombre,'',:codPostal,:poblacion,:provincia,:direccion,'2000-01-01',now(),:telefono,:email,:pass,:email,:logo,2,1,'','',0,0,'',1,1,'')";
        $statement = $dbConn->prepare($sql);
        $statement->bindValue(':nombre', $input['nombre']);
        $statement->bindValue(':idPueblo', $input['idPueblo']);
        $statement->bindValue(':codPostal', $input['codPostal']);
        $statement->bindValue(':poblacion', $input['poblacion']);
        $statement->bindValue(':provincia', $input['provincia']);
        $statement->bindValue(':direccion', $input['direccion']);
        $statement->bindValue(':telefono', $input['telefono']);
        if ($input['idPueblo']==1){
          $statement->bindValue(':email', str_replace('@qoorder.com','@qoorder-moron.com',$input['email']));
        }else if ($input['idPueblo']==2){
          $statement->bindValue(':email', str_replace('@qoorder.com','@qoorder-arahal.com',$input['email']));
        }else if ($input['idPueblo']==7){
          $statement->bindValue(':email', str_replace('@qoorder.com','@qoorder-lebrija.com',$input['email']));
        }else if ($input['idPueblo']==8){
          $statement->bindValue(':email', str_replace('@qoorder.com','@qoorder-osuna.com',$input['email']));
        }else{
          $statement->bindValue(':email',$input['email']);
        }
        
        $statement->bindValue(':pass', str_replace(' ','+',$_GET['password']));
        $statement->bindValue(':logo', $input['logo']);
        //bindAllValues($statement, $input);
        $statement->execute();
        $postId2 = $dbConn->lastInsertId();

        $sql = "INSERT INTO `qo_users_est`(idEstablecimiento, idUSer,activo) VALUES ($postId,$postId2,1)";
        $statement = $dbConn->prepare($sql);
        $statement->execute();

        $sql="INSERT INTO `qo_configuracion_est` (`idEstablecimiento`, `activoLunes`, `activoMartes`, `activoMiercoles`, `activoJueves`, `activoViernes`, `activoSabado`, `activoDomingo`, `inicioLunes`, `inicioMartes`, `inicioMiercoles`, `inicioJueves`, `inicioViernes`, `inicioSabado`, `inicioDomingo`, `finLunes`, `finMartes`, `finMiercoles`, `finJueves`, `finViernes`, `finSabado`, `finDomingo`, `servicioActivo`, `tiempoEntrega`, `inicioLunesTarde`, `inicioMartesTarde`, `inicioMiercolesTarde`, `inicioJuevesTarde`, `inicioViernesTarde`, `inicioSabadoTarde`, `inicioDomingoTarde`, `finLunesTarde`, `finMartesTarde`, `finMiercolesTarde`, `finJuevesTarde`, `finViernesTarde`, `finSabadoTarde`, `finDomingoTarde`, `pedidoMinimo`, `numeroPedidosSoportado`) VALUES ($postId, '0', '1', '1', '1', '1', '1', '1', '13:00:00', '13:00:00', '13:00:00', '13:00:00', '13:00:00', '13:00:00', '13:00:00', '16:00:00', '16:00:00', '16:00:00', '16:00:00', '16:00:00', '16:00:00', '16:00:00', '1', '45', '20:00:00', '19:30:00', '19:30:00', '19:30:00', '19:30:00', '19:30:00', '19:30:00', '22:30:00', '22:30:00', '22:30:00', '22:30:00', '22:30:00', '22:30:00', '22:30:00', '10.00', '4')";
        $statement = $dbConn->prepare($sql);
        $statement->execute();
      }
    }
      $input['id'] = $postId;
      header("HTTP/1.1 200 OK");
      echo json_encode($input);
      exit();
}

//Borrar
if ($_SERVER['REQUEST_METHOD'] == 'DELETE')
{
	$id = $_GET['id'];
  $statement = $dbConn->prepare("DELETE FROM qo_establecimientos where id=:id");
  $statement->bindValue(':id', $id);
  $statement->execute();
	header("HTTP/1.1 200 OK");
	exit();
}

//Actualizar
if ($_SERVER['REQUEST_METHOD'] == 'PUT')
{
    $input = json_decode(file_get_contents('php://input'), true);
    if (isset($_GET['zona']))
    {
      $id = $input['id'];
      $sql="UPDATE qo_establecimientos_zonas set nombre=:nombre,activo=:activo WHERE id=$id";
      $statement = $dbConn->prepare($sql);
      $statement->bindValue(':nombre', $input['nombre']);
      $statement->bindValue(':activo', $input['activo']);
      $statement->execute();
      $postId = $dbConn->lastInsertId();
    }else if (isset($_GET['fiscal']))
    {
      $sql="UPDATE `qo_establecimientos_fiscal` SET `razonSocial`=:razonSocial,`direccion`=:direccion,`cp`=:cp,`poblacion`=:poblacion,`provincia`=:provincia,`telefono`=:telefono,`cif`=:cif,`idEstablecimiento`=:idEstablecimiento,`iban`=:iban WHERE id=:id";
      $statement = $dbConn->prepare($sql);
      $statement->bindValue(':razonSocial', $input['razonSocial']);
      $statement->bindValue(':direccion', $input['direccion']);
      $statement->bindValue(':cp', $input['cp']);
      $statement->bindValue(':poblacion', $input['poblacion']);
      $statement->bindValue(':provincia', $input['provincia']);
      $statement->bindValue(':telefono', $input['telefono']);
      $statement->bindValue(':cif', $input['cif']);
      $statement->bindValue(':idEstablecimiento', $input['idEstablecimiento']);
      $statement->bindValue(':iban', $input['iban']);
      $statement->bindValue(':id', $input['id']);
      $statement->execute();
      $postId = $dbConn->lastInsertId();
    }else if (isset($_GET['actualizaEstablecimiento']))
    {
      $id = $input['idEstablecimiento'];
    $sql = "UPDATE `qo_establecimientos` SET nombreImpresoraBarra=:nombreImpresoraBarra,tipoImpresora=:tipoImpresora,llevaAMesa=:llevaAMesa,recogeEnBarra=:recogeEnBarra,telefono2=:telefono2,whatsapp=:whatsapp,web=:web,emailContacto=:emailContacto,esComercio=:esComercio,idZona=:idZona,orden=:orden, logo=:logo,idCategoria=:idCategoria,`local`=:local,
      envio=:envio,recogida=:recogida,llamadaCamarero=:llamadaCamarero,puedeReservar=:puedeReservar,`nombre`=:nombre, `direccion`=:direccion, 
      imagen=:imagen,poblacion=:poblacion,provincia=:provincia, codPostal=:codPostal, tipo=:idTipo, estado=:estado, email=:email, telefono=:telefono, 
      latitud=:latitud, longitud=:longitud,envio=:envio,recogida=:recogida,puedeReservar=:puedeReservar,idPueblo=:idPueblo,idGrupo=:idGrupo,tieneMenuDiario=:tieneMenuDiario,visibleFuera=:visibleFuera WHERE id=$id";
    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':nombreImpresoraBarra', $input['nombreImpresoraBarra']);
    $statement->bindValue(':tipoImpresora', $input['tipoImpresora']);
    $statement->bindValue(':esComercio', $input['esComercio']);
    $statement->bindValue(':idPueblo', $input['idPueblo']);
    if ($input['idPueblo']==1){
      $statement->bindValue(':idZona', 1);
      $statement->bindValue(':idGrupo', 1);
    }else if ($input['idPueblo']==2){
      $statement->bindValue(':idZona', 10);
      $statement->bindValue(':idGrupo', 2);
    }else if ($input['idPueblo']==7){
      $statement->bindValue(':idZona', 29);
      $statement->bindValue(':idGrupo', 3);
    }else if ($input['idPueblo']==8){
      $statement->bindValue(':idZona', 30);
      $statement->bindValue(':idGrupo', 4);
    }else{
      $statement->bindValue(':idZona', $input['idZona']);
      $statement->bindValue(':idGrupo', $input['idGrupo']);
    }
    $statement->bindValue(':orden', $input['orden']);
    $statement->bindValue(':logo', $input['logo']);
    $statement->bindValue(':idCategoria', $input['idCategoria']);
    $statement->bindValue(':local', $input['local']);
    $statement->bindValue(':envio', $input['envio']);
    $statement->bindValue(':recogida', $input['recogida']);
    $statement->bindValue(':llamadaCamarero', $input['llamadaCamarero']);
    $statement->bindValue(':puedeReservar', $input['puedeReservar']);
    $statement->bindValue(':nombre', $input['nombre']);
    $statement->bindValue(':direccion', $input['direccion']);
    $statement->bindValue(':imagen', $input['imagen']);
    $statement->bindValue(':poblacion', $input['poblacion']);
    $statement->bindValue(':provincia', $input['provincia']);
    $statement->bindValue(':codPostal', $input['codPostal']);
    if ($input['tieneMenuDiario']==null)
      $statement->bindValue(':tieneMenuDiario', 0);
    else
      $statement->bindValue(':tieneMenuDiario', $input['tieneMenuDiario']);
    if ($input['visibleFuera']==null)
      $statement->bindValue(':visibleFuera', 0);
    else
      $statement->bindValue(':visibleFuera', $input['visibleFuera']);
    $statement->bindValue(':idTipo', $input['idTipo']);
    $statement->bindValue(':estado', $input['estado']);
    $statement->bindValue(':email', $input['email']);
    $statement->bindValue(':envio', $input['envio']);
    $statement->bindValue(':recogida', $input['recogida']);
    $statement->bindValue(':puedeReservar', $input['puedeReservar']);
    $statement->bindValue(':telefono', $input['telefono']);
    $statement->bindValue(':telefono2', $input['telefono2']);
    $statement->bindValue(':whatsapp', $input['whatsapp']);
    if ($input['web']==null)
      $statement->bindValue(':web', '');
    else
      $statement->bindValue(':web', $input['web']);
    $statement->bindValue(':emailContacto', $input['emailContacto']);
    $statement->bindValue(':latitud', $input['latitud']);
    $statement->bindValue(':longitud', $input['longitud']);
    $statement->bindValue(':llevaAMesa', $input['llevaAMesa']);
    $statement->bindValue(':recogeEnBarra', $input['recogeEnBarra']);
    $statement->execute();


    }else
    {
    $id = $input['idEstablecimiento'];
    $sql = "UPDATE `qo_establecimientos` SET llevaAMesa=:llevaAMesa,recogeEnBarra=:recogeEnBarra,telefono2=:telefono2,whatsapp=:whatsapp,web=:web,emailContacto=:emailContacto,esComercio=:esComercio,idZona=:idZona,orden=:orden, logo=:logo,idCategoria=:idCategoria,`local`=:local,
      envio=:envio,recogida=:recogida,llamadaCamarero=:llamadaCamarero,puedeReservar=:puedeReservar,`nombre`=:nombre, `direccion`=:direccion, 
      imagen=:imagen,poblacion=:poblacion,provincia=:provincia, codPostal=:codPostal, tipo=:idTipo, estado=:estado, email=:email, telefono=:telefono, 
      latitud=:latitud, longitud=:longitud,envio=:envio,recogida=:recogida,puedeReservar=:puedeReservar,idPueblo=:idPueblo,idGrupo=:idGrupo,tieneMenuDiario=:tieneMenuDiario WHERE id=$id";
    $statement = $dbConn->prepare($sql);
    $statement->bindValue(':esComercio', $input['esComercio']);
    $statement->bindValue(':idPueblo', $input['idPueblo']);
    if ($input['idPueblo']==1){
      $statement->bindValue(':idZona', 1);
      $statement->bindValue(':idGrupo', 1);
    }else if ($input['idPueblo']==2){
      $statement->bindValue(':idZona', 10);
      $statement->bindValue(':idGrupo', 2);
    }else if ($input['idPueblo']==7){
      $statement->bindValue(':idZona', 29);
      $statement->bindValue(':idGrupo', 3);
    }else if ($input['idPueblo']==8){
      $statement->bindValue(':idZona', 30);
      $statement->bindValue(':idGrupo', 4);
    }else{
      $statement->bindValue(':idZona', $input['idZona']);
      $statement->bindValue(':idGrupo', $input['idGrupo']);
    }
    $statement->bindValue(':orden', $input['orden']);
    $statement->bindValue(':logo', $input['logo']);
    $statement->bindValue(':idCategoria', $input['idCategoria']);
    $statement->bindValue(':local', $input['local']);
    $statement->bindValue(':envio', $input['envio']);
    $statement->bindValue(':recogida', $input['recogida']);
    $statement->bindValue(':llamadaCamarero', $input['llamadaCamarero']);
    $statement->bindValue(':puedeReservar', $input['puedeReservar']);
    $statement->bindValue(':nombre', $input['nombre']);
    $statement->bindValue(':direccion', $input['direccion']);
    $statement->bindValue(':imagen', $input['imagen']);
    $statement->bindValue(':poblacion', $input['poblacion']);
    $statement->bindValue(':provincia', $input['provincia']);
    $statement->bindValue(':codPostal', $input['codPostal']);
    $statement->bindValue(':idTipo', $input['idTipo']);
    $statement->bindValue(':estado', $input['estado']);
    $statement->bindValue(':email', $input['email']);
    $statement->bindValue(':envio', $input['envio']);
    $statement->bindValue(':recogida', $input['recogida']);
    $statement->bindValue(':puedeReservar', $input['puedeReservar']);
    $statement->bindValue(':telefono', $input['telefono']);
    $statement->bindValue(':telefono2', $input['telefono2']);
    $statement->bindValue(':whatsapp', $input['whatsapp']);
    if ($input['web']==null)
      $statement->bindValue(':web', '');
    else
      $statement->bindValue(':web', $input['web']);
    if ($input['tieneMenuDiario']==null)
      $statement->bindValue(':tieneMenuDiario', 0);
    else
      $statement->bindValue(':tieneMenuDiario', $input['tieneMenuDiario']);
    $statement->bindValue(':emailContacto', $input['emailContacto']);
    $statement->bindValue(':latitud', $input['latitud']);
    $statement->bindValue(':longitud', $input['longitud']);
    $statement->bindValue(':llevaAMesa', $input['llevaAMesa']);
    $statement->bindValue(':recogeEnBarra', $input['recogeEnBarra']);
    $statement->execute();
    }
    header("HTTP/1.1 200 OK");
    exit();
}


//En caso de que ninguna de las opciones anteriores se haya ejecutado
header("HTTP/1.1 400 Bad Request");

?>