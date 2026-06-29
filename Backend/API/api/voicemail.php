<?php
/**
 * TwiML de respaldo si Vapi cae.
 *
 * Twilio invoca esta URL como `voice_fallback_url` solo cuando la URL primaria
 * (Vapi) devuelve error. Aquí se reproduce un mensaje y se ofrece dejar un
 * mensaje grabado en buzón.
 *
 *   - Twilio llama: POST /api/voicemail.php          → reproduce mensaje + Record
 *   - Twilio llama: POST /api/voicemail.php?save=1   → recibe grabación y la guarda
 */

declare(strict_types=1);

include __DIR__ . "/../config.php";
include __DIR__ . "/../utils.php";
require_once __DIR__ . "/_lib.php";

header('Content-Type: application/xml; charset=utf-8');

if (isset($_GET['save'])) {
    // Twilio entrega la URL de la grabación + duración tras el <Record>
    $recordingUrl = $_POST['RecordingUrl'] ?? '';
    $duration     = (int)($_POST['RecordingDuration'] ?? 0);
    $from         = $_POST['From'] ?? '';
    $callSid      = $_POST['CallSid'] ?? '';

    try {
        $dbConn = connect($db);
        $stmt = $dbConn->prepare("
            INSERT INTO qo_llamadas
                (vapi_call_id, telefono_origen, estado, fecha_inicio, fecha_fin,
                 duracion_segundos, audio_url, metadatos)
            VALUES
                (:vid, :tel, 'fallida', NOW(), NOW(),
                 :dur, :audio, :meta)
        ");
        $stmt->bindValue(':vid',   'voicemail-' . $callSid);
        $stmt->bindValue(':tel',   $from);
        $stmt->bindValue(':dur',   $duration, PDO::PARAM_INT);
        $stmt->bindValue(':audio', $recordingUrl . '.mp3');
        $stmt->bindValue(':meta',  json_encode(['fallback' => true, 'reason' => 'vapi_down']));
        $stmt->execute();

        agente_log('warn', 'voicemail_recibido', [
            'from' => $from, 'duration' => $duration, 'recording' => $recordingUrl
        ]);
        // Avisar al staff por email y push
        agente_alert(
            'Buzón usado (Vapi caído)',
            "Llamada de {$from} fue al buzón de respaldo.\nGrabación: {$recordingUrl}\nDuración: {$duration}s",
            ['callSid' => $callSid]
        );
        $tokens = agente_tokens_staff($dbConn, 67);
        if (count($tokens) > 0) {
            agente_push(
                $tokens,
                'Asador Morón',
                "📞 Buzón: {$from} ({$duration}s). Vapi caído.",
                ['tipo' => 'voicemail', 'audio' => $recordingUrl . '.mp3']
            );
        }
    } catch (\Throwable $e) {
        agente_log('error', 'voicemail_save_error', ['err' => $e->getMessage()]);
    }

    // Confirmación TwiML al cliente
    echo '<?xml version="1.0" encoding="UTF-8"?>
<Response>
    <Say language="es-ES" voice="Polly.Conchita">Gracias por su mensaje. Le llamaremos en breve. Hasta pronto.</Say>
    <Hangup/>
</Response>';
    exit();
}

// Modo por defecto: reproducir mensaje + grabar buzón
echo '<?xml version="1.0" encoding="UTF-8"?>
<Response>
    <Say language="es-ES" voice="Polly.Conchita">
        Hola, ha llamado a Asador Morón. En este momento el asistente automático no está disponible.
        Si lo desea, deje un mensaje tras la señal y le llamaremos lo antes posible.
        También puede llamarnos al 6 2 6 6 9 2 8 2 8.
    </Say>
    <Record
        action="https://qoorder.com/pa_ws/api/voicemail.php?save=1"
        method="POST"
        maxLength="60"
        finishOnKey="#"
        playBeep="true"
        timeout="3"/>
    <Say language="es-ES" voice="Polly.Conchita">No se grabó ningún mensaje. Hasta pronto.</Say>
    <Hangup/>
</Response>';
