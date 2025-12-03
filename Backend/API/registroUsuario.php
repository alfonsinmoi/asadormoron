<?php
    
    $nombre=$_GET['nombre'];
    $apellidos=$_GET['apellidos'];
    $dni=$_GET['dni'];
    $cod_postal=$_GET['codPostal'];
    $poblacion=$_GET['poblacion'];
    $provincia=$_GET['provincia'];
    $direccion=$_GET['direccion'];
    $fechaNacimiento=$_GET['fechaNacimiento'];
    $telefono=$_GET['telefono'];
    $email=$_GET['email'];
    $password=$_GET['password'];
    $username=$_GET['username'];
    $version=$_GET['version'];
    $idZona=$_GET['idZona'];
    $foto= str_replace("___","&",$_GET['foto']);
    $pin=$_GET['pin'];
    $idPueblo=$_GET['idPueblo'];
    $idSocial=$_GET['idSocial'];
    $social=$_GET['social'];

 $con=mysqli_connect(

    "db5016478383.hosting-data.io"

    ,"dbu1717954"

    ,"MoMa0408_"
                
    ,"dbs13376654"
);

    // Check conection
    if ($con->conect_error) {
        die("conection failed: " . $con->conect_error);
    }
    if (!$con->set_charset('utf8')) {
        exit();
    }
    
    $sql="SELECT * FROM qo_users WHERE (email='$email' or telefono='$telefono') and idPueblo=$idPueblo";
    if ($result=mysqli_query($con, $sql)) {
        $numero_filas = $result->num_rows;
        if ($numero_filas==0){
            $sql = "INSERT INTO `qo_users`(version,idSocial,social,idZona,pin,`nombre`, `apellidos`, `dni`, `cod_postal`, `poblacion`, `provincia`, `direccion`, `fechaNacimiento`, `fechaAlta`, `telefono`, `email`, `password`, `username`, `foto`, `rol`, `estado`, `plataforma`, `token`, `tipoRegistro`,idPueblo) VALUES ('$version','$idSocial','$social',$idZona,'$pin','$nombre','$apellidos','$dni','$cod_postal','$poblacion','$provincia','$direccion','$fechaNacimiento',now(),'$telefono','$email','$password','$username','$foto',1,1,'','',0,$idPueblo)";
            //echo $sql;
            if (mysqli_query($con, $sql)) {
                $last_id = mysqli_insert_id($con);
                $sql = "update qo_users set codigo=concat(upper(substring(nombre,1,1)),upper(substring(apellidos,1,1)),lpad(id,6,'0')) WHere id=$last_id";
                mysqli_query($con, $sql);
                echo $last_id;
            } else {
                echo "EFROR: " . $sql . "<br>" . mysqli_error($con);
            }
        }else{
            $sql="SELECT * FROM qo_users WHERE (email='$email' or telefono='$telefono') and idPueblo=$idPueblo and verificado=0";
            if ($result=mysqli_query($con, $sql)) {
                $numero_filas = $result->num_rows;
                if ($numero_filas==0){
                    echo "ERROR: El email o el teléfono ya existen en esta población";
                }else{
                    echo "ERROR: Su usuario está registrado, pero no verificado. Por favor, llame al 626692828 para solucionar el problema\nDisculpe las molestias.";
                }
            }
        }
    } else {
        echo "EFROR: " . $sql . "<br>" . mysqli_error($con);
    }
    $con->close();
?>


