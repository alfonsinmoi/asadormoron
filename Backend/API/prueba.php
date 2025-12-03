<?php
include "config.php";
include "utils.php";


$fields = getParams($input);
echo json_encode($input);

    header("HTTP/1.1 200 OK");
    exit();
?>