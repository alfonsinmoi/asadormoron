<?php
/**
 * POST /api/webhook-vapi.php
 *
 * Receptor de eventos de Vapi.ai. Valida la firma HMAC y persiste:
 *   - call-start         → INSERT en qo_llamadas (estado='en_curso')
 *   - transcript-final   → INSERT en qo_transcripciones
 *   - function-call      → (procesado por las tools del propio Vapi vía endpoints)
 *   - call-end           → UPDATE qo_llamadas con duración, coste, audio, estado
 *
 * El secreto se lee de /etc/qoorder/.env (VAPI_WEBHOOK_SECRET).
 *
 * Idempotente: el mismo evento procesado dos veces no duplica datos.
 */

include __DIR__ . "/../config.php";
include __DIR__ . "/../utils.php";
require_once __DIR__ . "/_lib.php";

header("Content-Type: application/json; charset=utf-8");

if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    http_response_code(405);
    echo json_encode(['error' => 'Method not allowed']);
    exit();
}

// Rate-limit por IP: 120 eventos/minuto (Vapi puede mandar varios por llamada)
$ip = $_SERVER['REMOTE_ADDR'] ?? 'unknown';
$rl = agente_rate_limit("webhook-vapi:ip:$ip", 120, 60);
if (!$rl['permitido']) {
    http_response_code(429);
    header('Retry-After: ' . $rl['reset']);
    agente_log('warn', 'webhook_rate_limit', ['ip' => $ip]);
    echo json_encode(['error' => 'rate_limit', 'reset' => $rl['reset']]);
    exit();
}

// ───── 1. Leer secreto ──────────────────────────────────────────────────
$envFile = '/etc/qoorder/.env';
$secret  = '';
if (is_readable($envFile)) {
    $env = parse_ini_file($envFile);
    $secret = $env['VAPI_WEBHOOK_SECRET'] ?? '';
}
// Permitir override por variable de entorno del proceso
$secret = getenv('VAPI_WEBHOOK_SECRET') ?: $secret;

$rawBody = file_get_contents('php://input');

// ───── 2. Validar secret de Vapi ────────────────────────────────────────
// Vapi envía el secret como header X-Vapi-Secret en plain text (no HMAC).
if ($secret !== '') {
    $headerSecret = $_SERVER['HTTP_X_VAPI_SECRET'] ?? '';
    if ($headerSecret === '' || !hash_equals($secret, $headerSecret)) {
        http_response_code(401);
        error_log("[webhook-vapi] Secret invalido");
        echo json_encode(['error' => 'Invalid signature']);
        exit();
    }
} else {
    error_log("[webhook-vapi] VAPI_WEBHOOK_SECRET no configurado, omitiendo validacion");
}

$payload = json_decode($rawBody, true);
if (!is_array($payload)) {
    http_response_code(400);
    echo json_encode(['error' => 'JSON invalido']);
    exit();
}

$tipo   = $payload['type']    ?? $payload['message']['type'] ?? '';
$call   = $payload['call']    ?? $payload['message']['call'] ?? [];
$callId = $call['id']         ?? '';

if ($callId === '') {
    http_response_code(400);
    echo json_encode(['error' => 'call.id requerido']);
    exit();
}

$dbConn = connect($db);

try {
    switch ($tipo) {

        // ─── Inicio de llamada ──────────────────────────────────────────
        case 'call-start':
        case 'status-update': // Vapi a veces envia este al inicio
            $telefono = $call['customer']['number']
                     ?? $call['phoneNumber']['number']
                     ?? null;
            $idEst    = (int)($call['assistantOverrides']['variableValues']['idEstablecimiento']
                              ?? $payload['idEstablecimiento']
                              ?? 0);

            $stmt = $dbConn->prepare("
                INSERT INTO qo_llamadas (vapi_call_id, telefono_origen, idEstablecimiento, estado, fecha_inicio, metadatos)
                VALUES (:vid, :tel, :est, 'en_curso', NOW(), :meta)
                ON DUPLICATE KEY UPDATE telefono_origen = COALESCE(VALUES(telefono_origen), telefono_origen)
            ");
            $stmt->bindValue(':vid',  $callId);
            $stmt->bindValue(':tel',  $telefono);
            $stmt->bindValue(':est',  $idEst > 0 ? $idEst : null, $idEst > 0 ? PDO::PARAM_INT : PDO::PARAM_NULL);
            $stmt->bindValue(':meta', json_encode(['raw_first_event' => $tipo]));
            $stmt->execute();
            break;

        // ─── Transcripción final de un turno ────────────────────────────
        case 'transcript':
        case 'transcript-final':
            // Vapi envía partials con transcriptType="partial" y finales con "final".
            // Solo guardamos los finales para no llenar la BD de ruido.
            $transcriptType = $payload['transcriptType']
                           ?? $payload['message']['transcriptType']
                           ?? 'final';
            if ($transcriptType !== 'final') break;

            $texto = $payload['transcript']
                  ?? $payload['message']['transcript']
                  ?? '';
            $role  = $payload['role']
                  ?? $payload['message']['role']
                  ?? 'desconocido';

            // Obtener id interno de la llamada
            $stmt = $dbConn->prepare("SELECT id FROM qo_llamadas WHERE vapi_call_id = :vid");
            $stmt->bindValue(':vid', $callId);
            $stmt->execute();
            $llamadaId = $stmt->fetchColumn();

            if ($llamadaId !== false && $texto !== '') {
                $turno = [['rol' => $role, 'texto' => $texto, 'timestamp' => date('c')]];
                $ins = $dbConn->prepare("
                    INSERT INTO qo_transcripciones (llamada_id, texto, texto_estructurado, fecha)
                    VALUES (:llid, :txt, :json, NOW())
                ");
                $ins->bindValue(':llid', $llamadaId, PDO::PARAM_INT);
                $ins->bindValue(':txt',  $texto);
                $ins->bindValue(':json', json_encode($turno));
                $ins->execute();
            }
            break;

        // ─── Fin de llamada ─────────────────────────────────────────────
        case 'end-of-call-report':
        case 'call-end':
            // Vapi puede mandar la duración como:
            //   - durationSeconds (int) en el root del message
            //   - duration (segs) o durationMs (ms) en call
            //   - startedAt / endedAt en ISO 8601 → calculamos diff
            $duracion = (int)(
                $payload['durationSeconds']
                ?? $payload['message']['durationSeconds']
                ?? $call['duration']
                ?? 0
            );
            if ($duracion === 0 && isset($call['durationMs'])) {
                $duracion = (int)round($call['durationMs'] / 1000);
            }
            if ($duracion === 0) {
                $startedAt = $call['startedAt'] ?? $payload['startedAt'] ?? null;
                $endedAt   = $call['endedAt']   ?? $payload['endedAt']   ?? null;
                if ($startedAt && $endedAt) {
                    $duracion = max(0, strtotime($endedAt) - strtotime($startedAt));
                }
            }

            $coste = (float)(
                $payload['cost']
                ?? $payload['message']['cost']
                ?? $call['cost']
                ?? 0
            );

            $audioUrl = $payload['recordingUrl']
                     ?? $payload['message']['recordingUrl']
                     ?? $call['recordingUrl']
                     ?? null;

            $endedReason = $payload['endedReason']
                        ?? $payload['message']['endedReason']
                        ?? $call['endedReason']
                        ?? '';

            $estado = 'completada';
            if (stripos($endedReason, 'transfer') !== false) {
                $estado = 'transferida';
            } elseif (stripos($endedReason, 'failed') !== false
                   || stripos($endedReason, 'error')  !== false) {
                $estado = 'fallida';
            } elseif ($duracion < 5) {
                $estado = 'no_pedido';
            }

            // Si tenemos pedido_id asociado, la consideramos completada con pedido
            // independientemente de la duración (caso edge: pedido rápido).
            $stmt2 = $dbConn->prepare("SELECT pedido_id, telefono_origen FROM qo_llamadas WHERE vapi_call_id = :vid");
            $stmt2->bindValue(':vid', $callId);
            $stmt2->execute();
            $rowLlamada = $stmt2->fetch(PDO::FETCH_ASSOC) ?: [];
            if (!empty($rowLlamada['pedido_id'])) $estado = 'completada';

            // ─── Auto-blacklist anti-trolleo ─────────────────────────────
            // Si este teléfono ha tenido >= 5 llamadas no_pedido/fallida en los últimos 7 días
            // sin un solo pedido completado, lo añadimos a la blacklist automáticamente.
            if (in_array($estado, ['no_pedido', 'fallida']) && !empty($rowLlamada['telefono_origen'])) {
                $tel = $rowLlamada['telefono_origen'];
                $countCheck = $dbConn->prepare("
                    SELECT
                        SUM(CASE WHEN estado IN ('no_pedido','fallida') THEN 1 ELSE 0 END) AS fallidas,
                        SUM(CASE WHEN pedido_id IS NOT NULL THEN 1 ELSE 0 END)              AS exitosas
                    FROM qo_llamadas
                    WHERE telefono_origen = :tel
                      AND fecha_inicio >= DATE_SUB(NOW(), INTERVAL 7 DAY)
                ");
                $countCheck->bindValue(':tel', $tel);
                $countCheck->execute();
                $stats = $countCheck->fetch(PDO::FETCH_ASSOC);
                $fallidas = (int)($stats['fallidas'] ?? 0);
                $exitosas = (int)($stats['exitosas'] ?? 0);

                if ($fallidas >= 50 && $exitosas === 0) {
                    $ins = $dbConn->prepare("
                        INSERT INTO qo_blacklist_telefonos (telefono, motivo, bloqueado_por, fecha)
                        VALUES (:tel, :mot, 'auto', NOW())
                        ON DUPLICATE KEY UPDATE motivo = VALUES(motivo), fecha = NOW()
                    ");
                    $ins->bindValue(':tel', $tel);
                    $ins->bindValue(':mot', "Auto: {$fallidas} llamadas sin pedido en 7 días");
                    $ins->execute();
                    agente_log('warn', 'autoblacklist_aplicada', [
                        'telefono' => $tel,
                        'fallidas_7d' => $fallidas
                    ]);
                }
            }

            // fecha_fin: usar la enviada por Vapi si existe, si no NOW()
            $endedAt = $payload['endedAt']
                    ?? $payload['message']['endedAt']
                    ?? $call['endedAt']
                    ?? null;
            $fechaFinSql = $endedAt ? date('Y-m-d H:i:s', strtotime($endedAt)) : null;

            $stmt = $dbConn->prepare("
                UPDATE qo_llamadas
                SET estado = :estado,
                    duracion_segundos = :dur,
                    coste_estimado = :coste,
                    audio_url = COALESCE(:audio, audio_url),
                    fecha_fin = COALESCE(:fechafin, fecha_fin, NOW())
                WHERE vapi_call_id = :vid
            ");
            $stmt->bindValue(':estado',   $estado);
            $stmt->bindValue(':dur',      $duracion, PDO::PARAM_INT);
            $stmt->bindValue(':coste',    $coste);
            $stmt->bindValue(':audio',    $audioUrl);
            $stmt->bindValue(':fechafin', $fechaFinSql);
            $stmt->bindValue(':vid',      $callId);
            $stmt->execute();
            break;

        // ─── Function calling (las herramientas reales se invocan vía sus
        //     propios endpoints; aquí solo logueamos el evento) ────────────
        case 'function-call':
        case 'tool-calls':
            error_log("[webhook-vapi] function-call en $callId: " . substr($rawBody, 0, 200));
            break;

        default:
            agente_log('warn', 'evento_desconocido', ['tipo' => $tipo, 'call_id' => $callId]);
    }

    agente_log('info', 'webhook_procesado', ['tipo' => $tipo, 'call_id' => $callId]);
    http_response_code(200);
    echo json_encode(['ok' => true, 'tipo' => $tipo, 'callId' => $callId]);

} catch (Exception $e) {
    http_response_code(500);
    agente_log('error', 'webhook_excepcion', ['call_id' => $callId, 'err' => $e->getMessage()]);
    echo json_encode(['error' => 'Internal error']);
}
