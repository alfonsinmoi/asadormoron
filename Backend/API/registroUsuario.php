<?php
include "config.php";
include "utils.php";

$dbConn = connect($db);

$nombre = $_GET['nombre'] ?? '';
$apellidos = $_GET['apellidos'] ?? '';
$dni = $_GET['dni'] ?? '';
$cod_postal = $_GET['codPostal'] ?? '';
$poblacion = $_GET['poblacion'] ?? '';
$provincia = $_GET['provincia'] ?? '';
$direccion = $_GET['direccion'] ?? '';
$fechaNacimiento = $_GET['fechaNacimiento'] ?? '';
$telefono = $_GET['telefono'] ?? '';
$email = $_GET['email'] ?? '';
$password = $_GET['password'] ?? '';
$username = $_GET['username'] ?? '';
$version = $_GET['version'] ?? '';
$idZona = intval($_GET['idZona'] ?? 0);
$foto = str_replace("___", "&", $_GET['foto'] ?? '');
$pin = $_GET['pin'] ?? '';
$idPueblo = intval($_GET['idPueblo'] ?? 0);
$idSocial = $_GET['idSocial'] ?? '';
$social = $_GET['social'] ?? '';

// Comprobar si ya existe el usuario
$sql = $dbConn->prepare("SELECT COUNT(*) as total FROM qo_users WHERE (email = :email OR telefono = :telefono) AND idPueblo = :idPueblo");
$sql->bindValue(':email', $email);
$sql->bindValue(':telefono', $telefono);
$sql->bindValue(':idPueblo', $idPueblo);
$sql->execute();
$row = $sql->fetch(PDO::FETCH_ASSOC);

if ($row['total'] == 0) {
    // Insertar nuevo usuario
    $insert = $dbConn->prepare("INSERT INTO qo_users (version, idSocial, social, idZona, pin, nombre, apellidos, dni, cod_postal, poblacion, provincia, direccion, fechaNacimiento, fechaAlta, telefono, email, password, username, foto, rol, estado, plataforma, token, tipoRegistro, idPueblo)
        VALUES (:version, :idSocial, :social, :idZona, :pin, :nombre, :apellidos, :dni, :cod_postal, :poblacion, :provincia, :direccion, :fechaNacimiento, NOW(), :telefono, :email, :password, :username, :foto, 1, 1, '', '', 0, :idPueblo)");
    $insert->bindValue(':version', $version);
    $insert->bindValue(':idSocial', $idSocial);
    $insert->bindValue(':social', $social);
    $insert->bindValue(':idZona', $idZona);
    $insert->bindValue(':pin', $pin);
    $insert->bindValue(':nombre', $nombre);
    $insert->bindValue(':apellidos', $apellidos);
    $insert->bindValue(':dni', $dni);
    $insert->bindValue(':cod_postal', $cod_postal);
    $insert->bindValue(':poblacion', $poblacion);
    $insert->bindValue(':provincia', $provincia);
    $insert->bindValue(':direccion', $direccion);
    $insert->bindValue(':fechaNacimiento', $fechaNacimiento);
    $insert->bindValue(':telefono', $telefono);
    $insert->bindValue(':email', $email);
    $insert->bindValue(':password', $password);
    $insert->bindValue(':username', $username);
    $insert->bindValue(':foto', $foto);
    $insert->bindValue(':idPueblo', $idPueblo);
    $insert->execute();

    $lastId = $dbConn->lastInsertId();
    if ($lastId) {
        $update = $dbConn->prepare("UPDATE qo_users SET codigo = CONCAT(UPPER(SUBSTRING(nombre,1,1)), UPPER(SUBSTRING(apellidos,1,1)), LPAD(id,6,'0')) WHERE id = :id");
        $update->bindValue(':id', $lastId);
        $update->execute();
        echo $lastId;
    }
} else {
    // Comprobar si está no verificado
    $sql2 = $dbConn->prepare("SELECT COUNT(*) as total FROM qo_users WHERE (email = :email OR telefono = :telefono) AND idPueblo = :idPueblo AND verificado = 0");
    $sql2->bindValue(':email', $email);
    $sql2->bindValue(':telefono', $telefono);
    $sql2->bindValue(':idPueblo', $idPueblo);
    $sql2->execute();
    $row2 = $sql2->fetch(PDO::FETCH_ASSOC);

    if ($row2['total'] == 0) {
        echo "ERROR: El email o el teléfono ya existen en esta población";
    } else {
        echo "ERROR: Su usuario está registrado, pero no verificado. Por favor, llame al 626692828 para solucionar el problema\nDisculpe las molestias.";
    }
}
?>
