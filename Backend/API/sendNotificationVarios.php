<?php
include "config.php";
include "utils.php";
$dbConn =  connect($db);
$filtro=$_GET['filtro'];
        $sql = $dbConn->prepare($filtro);
        $sql->execute();
        $sql->setFetchMode(PDO::FETCH_ASSOC);
        header("HTTP/1.1 200 OK");
        $ids=$sql->fetchAll();
        $ids2='';
        for($i = 0; $i < count($ids); ++$i) {
            if ($ids2!='')
            $ids2=$ids2.',';
            $ids2=$ids2.$ids[$i]['token'];
        }
        $ids3 = explode(",", $ids2);
    $con=mysqli_connect(

        "db5010045577.hosting-data.io"

        ,"dbu438349"

        ,"MoMa0408_"
                    
        ,"dbs8515813"
);
    if (!$con->set_charset('utf8')) {
        exit();
    }
    function sendMessage($ids){
        $content = array(
            "en" => $_GET['mensaje']
            );
        $title = array(
            "en" => $_GET['titulo']
            );

        


        $fields = array(
            'app_id' => "000cf2d3-9e1c-40c9-a6e6-56bafe0b3946",
            'include_player_ids' => $ids,
            'data' => array("foo" => "bar"),
            'contents' => $content,
            'large_icon' =>"icon_notification.png",
            'headings' => $title
        );

        $fields = json_encode($fields);
        print("\nJSON sent:\n");
        print($fields);

        $ch = curl_init();
        curl_setopt($ch, CURLOPT_URL, "https://onesignal.com/api/v1/notifications");
        curl_setopt($ch, CURLOPT_HTTPHEADER, array('Content-Type: application/json; charset=utf-8',
                                                'Authorization: Basic ZjIyNDQ5NGYtNDY5My00ZDhkLWI0ZWEtZjU2OTNjZmNmYzRj'));
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, TRUE);
        curl_setopt($ch, CURLOPT_HEADER, FALSE);
        curl_setopt($ch, CURLOPT_POST, TRUE);
        curl_setopt($ch, CURLOPT_POSTFIELDS, $fields);
        curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, FALSE);    

        $response = curl_exec($ch);
        curl_close($ch);

        return $response;
    }
    
    $response = sendMessage($ids3);
    $return["allresponses"] = $response;
    $return = json_encode( $return);
    print("\n\nJSON received:\n");
    print($return);
    print("\n");
?>