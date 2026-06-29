<?php
function sendMessage(){
    $content = array(
        "en" => $_GET['mensaje']
        );
    $title = array(
        "en" => $_GET['titulo']
        );

    $fields = array(
        'app_id' => "000cf2d3-9e1c-40c9-a6e6-56bafe0b3946",
        'include_subscription_ids' => array($_GET['id']),
        'target_channel' => 'push',
        'data' => array("foo" => "bar"),
        'contents' => $content,
        'large_icon' =>"icon_notification.png",
        'headings' => $title
    );

    $fields = json_encode($fields);

    $ch = curl_init();
    curl_setopt($ch, CURLOPT_URL, "https://api.onesignal.com/notifications");
    curl_setopt($ch, CURLOPT_HTTPHEADER, array('Content-Type: application/json; charset=utf-8',
                                            'Authorization: Key os_v2_app_aagpfu46dramtjxgk25p4czzi2b5tztqre5u5rupmrx223buaxs4l4awhwlsdccw2iala2uttjvndnnva4q26f4gbgywelarzqyruuq'));
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, TRUE);
    curl_setopt($ch, CURLOPT_HEADER, FALSE);
    curl_setopt($ch, CURLOPT_POST, TRUE);
    curl_setopt($ch, CURLOPT_POSTFIELDS, $fields);
    curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, FALSE);

    $response = curl_exec($ch);
    curl_close($ch);

    return $response;
}

$response = sendMessage();
$return["allresponses"] = $response;
$return = json_encode($return);
print($return);
?>
