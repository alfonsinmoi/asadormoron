<?php
    function image_resize($file_name, $width, $height, $crop=FALSE) {
        list($wid, $ht) = getimagesize($file_name);
        $r = $wid / $ht;
        if ($crop) {
           if ($wid > $ht) {
              $wid = ceil($wid-($width*abs($r-$width/$height)));
           } else {
              $ht = ceil($ht-($ht*abs($r-$w/$h)));
           }
           $new_width = $width;
           $new_height = $height;
        } else {
           if ($width/$height > $r) {
              $new_width = $height*$r;
              $new_height = $height;
           } else {
              $new_height = $width/$r;
              $new_width = $width;
           }
        }
        $source = imagecreatefromjpeg($file_name);
        $dst = imagecreatetruecolor($new_width, $new_height);
        image_copy_resampled($dst, $source, 0, 0, 0, 0, $new_width, $new_height, $wid, $ht);
        return $dst;
     }


     $verifyimg = getimagesize($_FILES['file']['tmp_name']);

     if($verifyimg['mime'] != 'image/png' && $verifyimg['mime'] != 'image/jpg' && $verifyimg['mime'] != 'image/jpeg') {
       exit;
     }
    $uploads_dir = './images/'.$_GET['carpeta']; //Directory to save the file that comes from client application.
    //if ($_FILES["file"]["error"] == UPLOAD_ERR_OK) {
    if (!file_exists($uploads_dir)) {
        mkdir($uploads_dir, 0777, true);
    }
    $tmp_name = $_FILES["file"]["tmp_name"];
    $name = $_FILES["file"]["name"];
    $img_to_resize = image_resize($uploads_dir."/". $_GET['archivo'], 45, 45);
    move_uploaded_file($img_to_resize, "$uploads_dir");

    $nombre=$_GET['nombre'];


    if(rename("$uploads_dir/$name","$uploads_dir/$nombre"))
        echo 'done';
    //}
    ?>