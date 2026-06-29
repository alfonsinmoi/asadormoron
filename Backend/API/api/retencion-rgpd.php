<?php
/**
 * Job de retención RGPD del agente de voz.
 *
 *   - Borra `audio_url` de qo_llamadas cuyo audio supere RETENTION_AUDIO_DIAS (default 90).
 *   - Borra qo_transcripciones de llamadas que superen RETENTION_TRANSCRIPCION_DIAS (default 365).
 *   - Borra qo_llamadas que superen el doble del periodo de transcripciones (ya sin contenido útil).
 *
 * Pensado para ejecutarse desde cron diario.
 *
 * Uso manual:
 *   php /var/www/qoorder/pa_ws/api/retencion-rgpd.php          # ejecuta
 *   php /var/www/qoorder/pa_ws/api/retencion-rgpd.php --dry    # solo cuenta, no borra
 *
 * Crontab sugerido:
 *   30 3 * * *  www-data  /usr/bin/php /var/www/qoorder/pa_ws/api/retencion-rgpd.php
 */

declare(strict_types=1);

// Solo permitido vía CLI o desde IP local (cron)
$esCli = PHP_SAPI === 'cli';
$ipLocal = in_array($_SERVER['REMOTE_ADDR'] ?? '', ['127.0.0.1', '::1']);
if (!$esCli && !$ipLocal) {
    http_response_code(403);
    exit('Solo CLI');
}

include __DIR__ . "/../config.php";
include __DIR__ . "/../utils.php";
require_once __DIR__ . "/_lib.php";

$dryRun = in_array('--dry', $argv ?? []);

$retencionAudio = (int)(agente_env('RETENTION_AUDIO_DIAS',         '90'));
$retencionTrans = (int)(agente_env('RETENTION_TRANSCRIPCION_DIAS', '365'));
$retencionLlam  = $retencionTrans * 2;   // borrado total del registro de llamada

$dbConn = connect($db);
$stats = [
    'audios_borrados'         => 0,
    'transcripciones_borradas'=> 0,
    'llamadas_borradas'       => 0,
    'dry_run'                 => $dryRun,
    'retencion_audio_dias'    => $retencionAudio,
    'retencion_transcripcion_dias' => $retencionTrans,
];

try {
    // 1. Audios fuera de retención (solo limpia el campo, no borra la llamada)
    $sel = $dbConn->prepare("
        SELECT COUNT(*) FROM qo_llamadas
        WHERE audio_url IS NOT NULL AND audio_url <> ''
          AND fecha_inicio < DATE_SUB(NOW(), INTERVAL :dias DAY)
    ");
    $sel->bindValue(':dias', $retencionAudio, PDO::PARAM_INT);
    $sel->execute();
    $stats['audios_borrados'] = (int)$sel->fetchColumn();

    if (!$dryRun && $stats['audios_borrados'] > 0) {
        $upd = $dbConn->prepare("
            UPDATE qo_llamadas
            SET audio_url = NULL
            WHERE audio_url IS NOT NULL
              AND fecha_inicio < DATE_SUB(NOW(), INTERVAL :dias DAY)
        ");
        $upd->bindValue(':dias', $retencionAudio, PDO::PARAM_INT);
        $upd->execute();
    }

    // 2. Transcripciones fuera de retención
    $sel = $dbConn->prepare("
        SELECT COUNT(*) FROM qo_transcripciones t
        JOIN qo_llamadas l ON l.id = t.llamada_id
        WHERE l.fecha_inicio < DATE_SUB(NOW(), INTERVAL :dias DAY)
    ");
    $sel->bindValue(':dias', $retencionTrans, PDO::PARAM_INT);
    $sel->execute();
    $stats['transcripciones_borradas'] = (int)$sel->fetchColumn();

    if (!$dryRun && $stats['transcripciones_borradas'] > 0) {
        $del = $dbConn->prepare("
            DELETE t FROM qo_transcripciones t
            JOIN qo_llamadas l ON l.id = t.llamada_id
            WHERE l.fecha_inicio < DATE_SUB(NOW(), INTERVAL :dias DAY)
        ");
        $del->bindValue(':dias', $retencionTrans, PDO::PARAM_INT);
        $del->execute();
    }

    // 3. Llamadas completamente fuera de retención (>2× transcripciones)
    $sel = $dbConn->prepare("
        SELECT COUNT(*) FROM qo_llamadas
        WHERE fecha_inicio < DATE_SUB(NOW(), INTERVAL :dias DAY)
    ");
    $sel->bindValue(':dias', $retencionLlam, PDO::PARAM_INT);
    $sel->execute();
    $stats['llamadas_borradas'] = (int)$sel->fetchColumn();

    if (!$dryRun && $stats['llamadas_borradas'] > 0) {
        $del = $dbConn->prepare("
            DELETE FROM qo_llamadas
            WHERE fecha_inicio < DATE_SUB(NOW(), INTERVAL :dias DAY)
        ");
        $del->bindValue(':dias', $retencionLlam, PDO::PARAM_INT);
        $del->execute();
    }

    agente_log('info', 'retencion_rgpd_ejecutada', $stats);
    echo json_encode($stats, JSON_PRETTY_PRINT) . PHP_EOL;

} catch (\Throwable $e) {
    agente_log('error', 'retencion_rgpd_error', ['err' => $e->getMessage()]);
    fwrite(STDERR, 'Error: ' . $e->getMessage() . PHP_EOL);
    exit(1);
}
