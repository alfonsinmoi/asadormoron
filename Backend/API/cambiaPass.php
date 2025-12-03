<?php
include "config.php";
    $email=$_GET['email'];
    $pin=$_GET['pin'];
    $pass=$_GET['pass'];
    // Check conection
    if ($con->conect_error) {
        die("conection failed: " . $con->conect_error);
    }
    if (!$con->set_charset('utf8')) {
        exit();
    }
    $sql="SELECT * FROM qo_users WHERE email='$email'";
    
    if ($result=mysqli_query($con, $sql)) {
        $numero_filas = $result->num_rows;
        if ($numero_filas>0){
            $sql = "UPDATE qo_users SET `password`='$pass',pin='$pin',verificado=0 where email='$email'";
            //echo $sql;
            if (mysqli_query($con, $sql)) {
                echo "";
            } else {
                echo "EFROR: " . $sql . "<br>" . mysqli_error($con);
            }
        }else{
            echo "ERROR: El correo electrónico no existe.";
        }
    } else {
        echo "EFROR: " . $sql . "<br>" . mysqli_error($con);
    }
    $con->close();
?>


