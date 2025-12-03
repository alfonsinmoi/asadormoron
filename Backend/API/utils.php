<?php

  //Abrir conexion a la base de datos
  function connect($db)
  {
      try {
          $conn = new PDO("mysql:host={$db['host']};dbname={$db['db']};charset=utf8", $db['username'], $db['password']);

          // set the PDO error mode to exception
          $conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

          return $conn;
      } catch (PDOException $exception) {
          exit($exception->getMessage());
      }
  }


 //Obtener parametros para updates
 function getParams($input)
 {
    $filterParams = [];
    foreach($input as $param => $value)
    {
      if ($param!='id')
            $filterParams[] = "$param=:$param";
    }
    return implode(", ", $filterParams);
  }
  function getEstablecimientoParams($input)
 {
    $filterParams = [];
    foreach($input as $param => $value)
    {
      if ($param!='idEstablecimiento' && $param!='idTipo' && $param!='numeroCategorias' && $param!='numeroProductos' && $param!='zonas' && $param!='ventas' && $param!='distancia')
            $filterParams[] = "$param=:$param";
    }
    return implode(", ", $filterParams);
  }
  function getCategoriaEstablecimientoParams($input)
 {
    $filterParams = [];
    foreach($input as $param => $value)
    {
      if ($param!='idTipo'){
        if ($param!='tipo' && $param!='numeroProductos'){
            $filterParams[] = "$param=:$param";
        }
      }else
            $filterParams[] = "$tipo=:$param";
    }
    return implode(", ", $filterParams);
  }


  //Asociar todos los parametros a un sql
	function bindAllValues($statement, $params)
  {
		foreach($params as $param => $value)
    {
      if ($param!='id')
				$statement->bindValue(':'.$param, $value);
		}
		return $statement;
   }
   function bindEstablecimientoValues($statement, $params)
  {
		foreach($params as $param => $value)
    {
      if($param!=='idEstablecimiento' && $param!='idTipo' && $param!='numeroCategorias' && $param!='numeroProductos' && $param!='zonas' && $param!='ventas' && $param!='distancia')
				$statement->bindValue(':'.$param, $value);
		}
		return $statement;
   }
   function bindCategoriaEstablecimientoValues($statement, $params)
  {
		foreach($params as $param => $value)
    {
      if ($param!='tipo' && $param!='numeroProductos')
				$statement->bindValue(':'.$param, $value);
		}
		return $statement;
   }
   /*function bindAllValuesProd($statement, $params)
  {
		foreach($params as $param => $value)
    {
      if($param=='precio' || $param=='precioOferta' || $param=='valoracion')
        $statement->bindValue(':'.$param, str_replace(',','.',$value));
      else
        $statement->bindValue(':'.$param, $value);
		}
		return $statement;
   }
   function bindAllValuesEst($statement, $params)
  {
		foreach($params as $param => $value)
    {
      if ($param!='fotos' && $param!='productos' && $param!='categorias')
				$statement->bindValue(':'.$param, $value);
		}
		return $statement;
   }
   function bindAllValuesFotos($statement, $params)
  {
		foreach($params as $param => $value)
    {
      if ($param!='isEmpty' && $param!='Dispatcher' && $param!='BindingContext')
				$statement->bindValue(':'.$param, $value);
		}
		return $statement;
   }
   function bindAllValuesCat($statement, $params)
  {
		foreach($params as $param => $value)
    {
      if ($param!='establecimientos' && $param!='Dispatcher' && $param!='BindingContext')
				$statement->bindValue(':'.$param, $value);
		}
		return $statement;
   }*/
 ?>