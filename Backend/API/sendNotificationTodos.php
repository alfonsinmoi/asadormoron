<?php
function sendMessage() {
    $content = array(
        "en" => $_GET['mensaje']
    );
    $title = array(
        "en" => $_GET['titulo']
    );

    $fields = array(
        'app_id' => "000cf2d3-9e1c-40c9-a6e6-56bafe0b3946",
        'included_segments' => array('All'),
        'data' => array("foo" => "bar"),
        'contents' => $content,
        'large_icon' => "icon_notification.png",
        'headings' => $title
    );

    $fields = json_encode($fields);

    $ch = curl_init();
    curl_setopt($ch, CURLOPT_URL, "https://api.onesignal.com/notifications");
    curl_setopt($ch, CURLOPT_HTTPHEADER, array('Content-Type: application/json; charset=utf-8',
                                            'Authorization: Key os_v2_app_aagpfu46dramtjxgk25p4czzi342yntwnutew754lzskxy3s452x3g5gagacqqrcxqwfrhlq4q5u7dvtqqjyug3q4ddvy3ce6pjxiti'));
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
