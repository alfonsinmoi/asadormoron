<?php
    $verifyimg = getimagesize($_FILES['file']['tmp_name']);

    if($verifyimg['mime'] != 'image/png' && $verifyimg['mime'] != 'image/jpg' && $verifyimg['mime'] != 'image/jpeg' && $_FILES['file']['type']!='application/pdf') {
        exit;
    }
    $uploads_dir = './images/'.$_GET['carpeta']; //Directory to save the file that comes from client application.
    if ($_FILES['file']['type']=='application/pdf')
        $uploads_dir = './documentos/'.$_GET['carpeta'];
    //if ($_FILES["file"]["error"] == UPLOAD_ERR_OK) {
    if (!file_exists($uploads_dir)) {
        mkdir($uploads_dir, 0777, true);
    }
    $tmp_name = $_FILES["file"]["tmp_name"];
    $name = $_FILES["file"]["name"];
    move_uploaded_file($tmp_name, "$uploads_dir/$name");

    $nombre=$_GET['nombre'];
    if ($_GET['carpeta']!=''){
        if (strpos($_GET['antiguo'], 'logo_producto')==false){
            unlink($_GET['antiguo']);
        }
    }
    if(rename("$uploads_dir/$name","$uploads_dir/$nombre"))
        echo 'done';
    //}
    ?>