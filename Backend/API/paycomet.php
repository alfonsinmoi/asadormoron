<?php
/**
 * PAYCOMET JET IFRAME callback
 * Tracking ID: SSX-7MS-JX3J
 *
 * @author PAYCOMET
 * @copyright Copyright (c) 2019, PAYCOMET
 * @version 1.1 2019-11-07
 */
 
$jetID 			= "qeQ0yXMufV3gpdU7acrNT84CFIzonRbh";
$merchantCode   = "v2gjg7f6";
$terminal       = "54153";
$password       = "PcMrIj3Kvq1aFQNeEZH9";

if (isset($_POST["paytpvToken"])) {
    date_default_timezone_set("Europe/Madrid");

    $token = $_POST["paytpvToken"];

    if ($token && strlen($token) == 64) {

        $endPoint       			= "https://api.paycomet.com/gateway/xml-bankstore?wsdl";
        $productDescription         = "TPV Virtual";
        $owner                      = "El Pollo Andaluz";

        $signature
            = hash("sha512",
                $merchantCode
                .$token
                .$jetID
                .$terminal
                .$password
        );


        $ip				= $_SERVER["REMOTE_ADDR"];

        try {
            $context = stream_context_create(array(
                'ssl' => array(
                    'verify_peer' => false,
                    'verify_peer_name' => false,
                    'allow_self_signed' => true
                )
            ));

            $clientSOAP = new SoapClient($endPoint, array('stream_context' => $context));
            $addUserTokenResponse
                = $clientSOAP->add_user_token(
                    $merchantCode,
                    $terminal,
                    $token,
                    $jetID,
                    $signature,
                    $ip
				);					

			if ($addUserTokenResponse["DS_ERROR_ID"] == "0") {
				echo "Proceso correcto";
				$data=array("DS_IDUSER"=>$addUserTokenResponse['DS_IDUSER'],"DS_TOKEN_USER"=>$addUserTokenResponse['DS_TOKEN_USER'],"DS_ERROR_ID"=>$addUserTokenResponse['DS_ERROR_ID']);
				echo json_encode($data);
				return true;
			} else {
				//KO
				//var_dump("Error al obtener el ejecutar la compra");
				//var_dump($executePurchaseResponse["DS_ERROR_ID"]);
				echo "Proceso Incorrecto";
				$data=array("DS_IDUSER"=>$addUserTokenResponse['DS_IDUSER'],"DS_TOKEN_USER"=>$addUserTokenResponse['DS_TOKEN_USER'],"DS_ERROR_ID"=>$addUserTokenResponse['DS_ERROR_ID']);
				echo json_encode($data);
				return false;
			}
        } catch(SoapFault $e){
			var_dump("Proceso Incorrecto");
            //var_dump($e);
        }
	} else {
        var_dump("Error, no se ha obtenido token");
        return false;
    }
    return false;
}
?>
<script>
</script>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<html>
<head>
</head>
<meta name="viewport" content="width=device-width, initial-scale=1.0">
<body style="background-color: #d82e34; background-size: cover;">
	<script src="//cdnjs.cloudflare.com/ajax/libs/jquery/3.2.1/jquery.min.js"></script>
	<link href="//maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css" rel="stylesheet" id="bootstrap-css">
	<script src="//maxcdn.bootstrapcdn.com/bootstrap/4.0.0/js/bootstrap.min.js"></script>
	<!------ Include the above in your HEAD tag ---------->
	<link rel="stylesheet" href="https://use.fontawesome.com/releases/v5.0.8/css/all.css">
	<div class="container">
			<div class="row">
				<aside class="col-sm-6">
					<article class="card" style="background-color:#d82e34;">
						<div class="card-body p-5" style="padding: 10px !important;">
							<form role="form" id="paycometPaymentForm" method="POST">
							<input type="hidden" name="amount" value="0">
							<input type="hidden" data-paycomet="jetID" value="<?php echo $jetID; ?>">
							<img src="https://qoorder.com/pa_ws/images/logoQoorder.png" style="
    height: 60px;
    margin-left: auto;
    margin-right: auto;
    display: block;
"/>
							<div class="form-group">
								<label for="username" style="color:white">Titular de la tarjeta</label>
								<div class="input-group">
									<div class="input-group-prepend">
										<span class="input-group-text"><i class="fa fa-user"></i></span>
									</div>
									<input type="text" class="form-control" name="username" data-paycomet="cardHolderName" id="cardHolderName" placeholder="" required="" text="<?php echo $jetID; ?>" value="<?php echo $_GET['nombre']; ?>">
								</div> <!-- input-group.// -->
							</div> <!-- form-group.// -->

							<div class="form-group">
								<label for="cardNumber" style="color:white">Número de la tarjeta</label>
								<div class="input-group">
									<div class="input-group-prepend">
										<span class="input-group-text"><i class="fa fa-credit-card"></i></span>
									</div>
									<div id="paycomet-pan" style="padding:0px;height:36px;padding-left: 0px;border: 1px solid #ced4da;border-radius: .25em;border-left: 0px;border-bottom-left-radius: 0;border-top-left-radius: 0;width: 85%;background-color: #fff;"></div>
									<input paycomet-style="height: 34px;font-size: 1rem;padding-left: 7px;padding-top: 0px;border: 0px;font-family: Arial;color: #495057;" paycomet-name="pan">
								</div>
							</div>
							<div class="row">
								<div class="col-sm-8">
									<div class="form-group">
										<label><span class="hidden-xs" style="color:white">Fecha Expiración</span> </label>
										<div class="form-inline">
											<select class="form-control" style="width:55%" data-paycomet="dateMonth">
												<option>MES</option>
												<option value="01">01 - Enero</option>
												<option value="02">02 - Feberero</option>
												<option value="03">03 - Marzo</option>
												<option value="04">04 - Abril</option>
												<option value="05">05 - Mayo</option>
												<option value="06">06 - Junio</option>
												<option value="07">07 - Julio</option>
												<option value="08">08 - Agosto</option>
												<option value="09">09 - Septiembre</option>
												<option value="10">10 - Octubre</option>
												<option value="11">11 - Noviembre</option>
												<option value="12">12 - Diciembre</option>
											</select>
											<span style="width:10%; text-align: center"> / </span>
											<select class="form-control" style="width:35%" data-paycomet="dateYear">
												<option>AÑO</option>
												<option value="21">2021</option>
												<option value="22">2022</option>
												<option value="23">2023</option>
												<option value="24">2024</option>
												<option value="25">2025</option>
												<option value="26">2026</option>
												<option value="27">2027</option>
												<option value="28">2028</option>
												<option value="29">2029</option>
												<option value="30">2030</option>
											</select>
										</div>
									</div>
								</div>

								<div class="col-sm-4">
									<div class="form-group">
										<label style="color:white" data-toggle="tooltip" title=""
											data-original-title="Código de 3 dígitos situado detrás de la tarjeta">CVV <i
												class="fa fa-question-circle"></i></label>
										<div id="paycomet-cvc2" style="padding:0px;height:36px;padding-left: 10px;padding-top: 2px;border: 1px solid #ced4da;border-radius: .25em; background-color:white"></div>
										<input paycomet-name="cvc2" paycomet-style="border: 0px;font-size: 1rem;padding-left: 7px;font-family: Arial;color: rgb(73, 80, 87);padding: 0px;height: 34px;width: 100%;text-align: center;margin-left: -10px;" class="form-control" required="" type="text">
									</div> <!-- form-group.// -->
								</div>
							</div> <!-- row.// -->
							<button class="subscribe btn btn-primary btn-block" type="submit" style="
    background-color: #000000;
    border-color: #000000;
    border-radius: 15px;
    color: white;
    width: 50%;
    margin-left: auto;
    margin-right: auto;
"> Guardar Tarjeta </button>
						</form>
							<div id="paymentErrorMsg" style="
    color: white;
    font-family: Arial;
    font-size: 1rem;
">

							</div>
						</div> <!-- card-body.// -->
					</article> <!-- card.// -->
				</aside>
			</div>
		</div>
		<script src="https://api.paycomet.com/gateway/paycomet.jetiframe.js?lang=es"></script>
</body>

</html>