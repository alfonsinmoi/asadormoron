<?php
/**
 * PAYCOMET JET IFRAME callback
 * Usa REST API v1 (reemplaza SOAP obsoleto)
 */

$jetID        = "qeQ0yXMufV3gpdU7acrNT84CFIzonRbh";
$terminal     = "54153";

if (isset($_POST["paytpvToken"])) {
    date_default_timezone_set("Europe/Madrid");

    $token = $_POST["paytpvToken"];

    if ($token && strlen($token) == 64) {

        // Obtener API key de la base de datos
        include "config.php";
        include "utils.php";
        $dbConn = connect($db);
        $stmt = $dbConn->prepare("SELECT apiPaycomet, terminalPaycomet FROM qo_configuracion_global LIMIT 1");
        $stmt->execute();
        $row = $stmt->fetch(PDO::FETCH_ASSOC);
        $apiKey   = $row ? $row['apiPaycomet']   : "";
        $terminal = $row ? $row['terminalPaycomet'] : $terminal;

        $fields = json_encode([
            "terminal" => (int)$terminal,
            "jetToken" => $token,
            "order"    => "ADDCARD" . time()
        ]);

        $ch = curl_init();
        curl_setopt($ch, CURLOPT_URL, "https://rest.paycomet.com/v1/cards");
        curl_setopt($ch, CURLOPT_HTTPHEADER, [
            "Content-Type: application/json",
            "PAYCOMET-API-TOKEN: " . $apiKey
        ]);
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
        curl_setopt($ch, CURLOPT_POST, true);
        curl_setopt($ch, CURLOPT_POSTFIELDS, $fields);
        curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, false);
        curl_setopt($ch, CURLOPT_TIMEOUT, 30);

        $response = curl_exec($ch);
        curl_close($ch);

        $result = json_decode($response, true);

        if ($result && isset($result['errorCode']) && $result['errorCode'] == 0) {
            $data = [
                "DS_IDUSER"     => $result['idUser'],
                "DS_TOKEN_USER" => $result['tokenUser'],
                "DS_ERROR_ID"   => "0"
            ];
            echo "Proceso correcto";
            echo json_encode($data);
        } else {
            $errorCode = isset($result['errorCode']) ? $result['errorCode'] : "999";
            $data = [
                "DS_IDUSER"     => "",
                "DS_TOKEN_USER" => "",
                "DS_ERROR_ID"   => (string)$errorCode
            ];
            echo "Proceso Incorrecto";
            echo json_encode($data);
        }
    } else {
        echo "Error, no se ha obtenido token";
    }
    exit();
}
?>
<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Añadir Tarjeta</title>
    <link href="https://fonts.googleapis.com/css2?family=Nunito:wght@400;600;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css">
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Nunito', -apple-system, BlinkMacSystemFont, sans-serif;
            background-color: #F5F5F5;
            min-height: 100vh;
            padding: 16px;
        }

        .card {
            background: white;
            border-radius: 20px;
            padding: 24px;
            box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
            max-width: 400px;
            margin: 0 auto;
        }

        .card-header {
            text-align: center;
            margin-bottom: 24px;
        }

        .card-header h2 {
            color: #333333;
            font-size: 20px;
            font-weight: 700;
        }

        .card-icon {
            width: 60px;
            height: 60px;
            background: linear-gradient(135deg, #C41E3A, #a01830);
            border-radius: 16px;
            display: flex;
            align-items: center;
            justify-content: center;
            margin: 0 auto 16px;
        }

        .card-icon i {
            color: white;
            font-size: 24px;
        }

        .form-group {
            margin-bottom: 20px;
        }

        .form-group label {
            display: block;
            color: #333333;
            font-size: 14px;
            font-weight: 600;
            margin-bottom: 8px;
        }

        .input-wrapper {
            position: relative;
        }

        .input-wrapper i {
            position: absolute;
            left: 14px;
            top: 50%;
            transform: translateY(-50%);
            color: #999999;
            font-size: 16px;
        }

        .form-control {
            width: 100%;
            height: 48px;
            padding: 0 14px 0 44px;
            border: 2px solid #E8E8E8;
            border-radius: 12px;
            font-size: 15px;
            font-family: 'Nunito', sans-serif;
            color: #333333;
            background: #FAFAFA;
            transition: all 0.2s ease;
        }

        .form-control:focus {
            outline: none;
            border-color: #C41E3A;
            background: white;
        }

        .form-control::placeholder {
            color: #AAAAAA;
        }

        .row {
            display: flex;
            gap: 12px;
        }

        .col-8 {
            flex: 2;
        }

        .col-4 {
            flex: 1;
        }

        .date-row {
            display: flex;
            gap: 8px;
            align-items: center;
        }

        .date-row select {
            flex: 1;
            height: 48px;
            padding: 0 10px;
            border: 2px solid #E8E8E8;
            border-radius: 12px;
            font-size: 14px;
            font-family: 'Nunito', sans-serif;
            color: #333333;
            background: #FAFAFA;
            cursor: pointer;
            transition: all 0.2s ease;
        }

        .date-row select:focus {
            outline: none;
            border-color: #C41E3A;
            background: white;
        }

        .date-separator {
            color: #999999;
            font-weight: 600;
        }

        #paycomet-pan {
            width: 100%;
            height: 48px;
            padding: 0 14px 0 44px;
            border: 2px solid #E8E8E8;
            border-radius: 12px;
            background: #FAFAFA;
        }

        #paycomet-cvc2 {
            width: 100%;
            height: 48px;
            padding: 0 14px;
            border: 2px solid #E8E8E8;
            border-radius: 12px;
            background: #FAFAFA;
        }

        .btn-submit {
            width: 100%;
            height: 52px;
            background: linear-gradient(135deg, #C41E3A, #a01830);
            border: none;
            border-radius: 14px;
            color: white;
            font-size: 16px;
            font-weight: 700;
            font-family: 'Nunito', sans-serif;
            cursor: pointer;
            margin-top: 8px;
            transition: all 0.2s ease;
            box-shadow: 0 4px 12px rgba(196, 30, 58, 0.3);
        }

        .btn-submit:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 16px rgba(196, 30, 58, 0.4);
        }

        .btn-submit:active {
            transform: translateY(0);
        }

        .secure-badge {
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            margin-top: 20px;
            padding-top: 16px;
            border-top: 1px solid #E8E8E8;
        }

        .secure-badge i {
            color: #4CAF50;
            font-size: 14px;
        }

        .secure-badge span {
            color: #888888;
            font-size: 12px;
        }

        #paymentErrorMsg {
            color: #C41E3A;
            font-size: 14px;
            text-align: center;
            margin-top: 12px;
            padding: 12px;
            background: #FFF5F5;
            border-radius: 8px;
            display: none;
        }

        #paymentErrorMsg:not(:empty) {
            display: block;
        }
    </style>
</head>
<body>
    <div class="card">
        <div class="card-header">
            <div class="card-icon">
                <i class="fas fa-credit-card"></i>
            </div>
            <h2>Añadir Tarjeta</h2>
        </div>

        <form role="form" id="paycometPaymentForm" method="POST">
            <input type="hidden" name="amount" value="0">
            <input type="hidden" data-paycomet="jetID" value="<?php echo $jetID; ?>">

            <div class="form-group">
                <label for="cardHolderName">Titular de la tarjeta</label>
                <div class="input-wrapper">
                    <i class="fas fa-user"></i>
                    <input type="text"
                           class="form-control"
                           name="username"
                           data-paycomet="cardHolderName"
                           id="cardHolderName"
                           placeholder="Nombre del titular"
                           required
                           value="<?php echo isset($_GET['nombre']) ? htmlspecialchars($_GET['nombre']) : ''; ?>">
                </div>
            </div>

            <div class="form-group">
                <label>Número de la tarjeta</label>
                <div class="input-wrapper">
                    <i class="fas fa-credit-card"></i>
                    <div id="paycomet-pan"></div>
                    <input paycomet-name="pan" paycomet-style="width:100%;height:44px;font-size:15px;padding-left:44px;border:0;font-family:'Nunito',sans-serif;color:#333333;background:transparent;">
                </div>
            </div>

            <div class="row">
                <div class="col-8">
                    <div class="form-group">
                        <label>Fecha de expiración</label>
                        <div class="date-row">
                            <select data-paycomet="dateMonth">
                                <option value="">Mes</option>
                                <option value="01">01</option>
                                <option value="02">02</option>
                                <option value="03">03</option>
                                <option value="04">04</option>
                                <option value="05">05</option>
                                <option value="06">06</option>
                                <option value="07">07</option>
                                <option value="08">08</option>
                                <option value="09">09</option>
                                <option value="10">10</option>
                                <option value="11">11</option>
                                <option value="12">12</option>
                            </select>
                            <span class="date-separator">/</span>
                            <select data-paycomet="dateYear">
                                <option value="">Año</option>
                                <option value="25">2025</option>
                                <option value="26">2026</option>
                                <option value="27">2027</option>
                                <option value="28">2028</option>
                                <option value="29">2029</option>
                                <option value="30">2030</option>
                                <option value="31">2031</option>
                                <option value="32">2032</option>
                                <option value="33">2033</option>
                                <option value="34">2034</option>
                            </select>
                        </div>
                    </div>
                </div>

                <div class="col-4">
                    <div class="form-group">
                        <label>CVV</label>
                        <div id="paycomet-cvc2"></div>
                        <input paycomet-name="cvc2" paycomet-style="width:100%;height:44px;font-size:15px;text-align:center;border:0;font-family:'Nunito',sans-serif;color:#333333;background:transparent;" required type="text">
                    </div>
                </div>
            </div>

            <button class="btn-submit" type="submit">
                <i class="fas fa-lock" style="margin-right: 8px;"></i>
                Guardar Tarjeta
            </button>
        </form>

        <div id="paymentErrorMsg"></div>

        <div class="secure-badge">
            <i class="fas fa-shield-alt"></i>
            <span>Pago seguro con encriptación SSL</span>
        </div>
    </div>

    <script src="https://api.paycomet.com/gateway/paycomet.jetiframe.js?lang=es"></script>
</body>
</html>
