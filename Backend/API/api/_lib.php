<?php
/**
 * Librería común para los endpoints del agente de voz.
 *
 *  - Cargador de /etc/qoorder/agente.env
 *  - Cliente Redis singleton (cache + rate-limit)
 *  - Logger estructurado JSON con call_id
 *  - Rate-limit por IP y por clave personalizada
 *
 * Cargada con: require_once __DIR__ . '/_lib.php';
 */

// ─────────────────────────────────────────────────────────────────────
// Cargador de /etc/qoorder/agente.env (lazy, sólo al primer agente_env)
// ─────────────────────────────────────────────────────────────────────
function agente_env(string $clave, ?string $default = null): ?string {
    static $loaded = false;
    static $cache  = [];
    if (!$loaded) {
        $loaded = true;
        $path   = '/etc/qoorder/agente.env';
        if (is_readable($path)) {
            // Parser propio: tolera comentarios, paréntesis y guiones unicode.
            // Solo parsea líneas con formato KEY=VALUE.
            foreach (file($path, FILE_IGNORE_NEW_LINES | FILE_SKIP_EMPTY_LINES) as $linea) {
                $linea = ltrim($linea);
                if ($linea === '' || $linea[0] === '#') continue;
                $pos = strpos($linea, '=');
                if ($pos === false) continue;
                $k = trim(substr($linea, 0, $pos));
                $v = trim(substr($linea, $pos + 1));
                // Quitar comillas si las hay
                if (strlen($v) >= 2 && (
                    ($v[0] === '"' && $v[-1] === '"') ||
                    ($v[0] === "'" && $v[-1] === "'")
                )) {
                    $v = substr($v, 1, -1);
                }
                $cache[$k] = $v;
            }
        }
    }
    // Prioridad: variable de entorno del proceso > .env > default
    $envProc = getenv($clave);
    if ($envProc !== false && $envProc !== '') return $envProc;
    return $cache[$clave] ?? $default;
}

// ─────────────────────────────────────────────────────────────────────
// Redis singleton (lazy + tolerante a fallos)
// Devuelve un \Redis (extensión php-redis) o null si no está disponible.
// ─────────────────────────────────────────────────────────────────────
function agente_redis() {
    static $r = null;
    static $tried = false;
    if ($tried) return $r;
    $tried = true;

    if (!extension_loaded('redis') || !class_exists('Redis')) return null;

    try {
        $client = new \Redis();
        $host   = agente_env('REDIS_HOST', '127.0.0.1');
        $port   = (int)agente_env('REDIS_PORT', '6379');
        if (!$client->connect($host, $port, 0.5)) return null;
        $pass = agente_env('REDIS_PASSWORD', '');
        if ($pass) $client->auth($pass);
        $r = $client;
        return $r;
    } catch (\Throwable $e) {
        error_log('[agente_redis] connect failed: ' . $e->getMessage());
        return null;
    }
}

// ─────────────────────────────────────────────────────────────────────
// Logger JSON estructurado
// Uso: agente_log('info', 'evento', ['call_id'=>..., 'data'=>...]);
// Se vuelca con error_log() y va a /var/log/nginx/error.log o php-fpm.log
// ─────────────────────────────────────────────────────────────────────
function agente_log(string $level, string $msg, array $context = []): void {
    $base = [
        'ts'      => date('c'),
        'level'   => $level,
        'msg'     => $msg,
        'remote'  => $_SERVER['REMOTE_ADDR']     ?? null,
        'uri'     => $_SERVER['REQUEST_URI']     ?? null,
        'method'  => $_SERVER['REQUEST_METHOD']  ?? null,
    ];
    error_log(json_encode(array_merge($base, $context), JSON_UNESCAPED_UNICODE | JSON_UNESCAPED_SLASHES));
}

// ─────────────────────────────────────────────────────────────────────
// Rate limit basado en Redis (ventana fija de N segundos)
// Devuelve [permitido, restantes, reset_seg].
// Si Redis está caído, devuelve permitido=true (degradación abierta —
// preferimos servir antes que bloquear si el rate-limit falla).
// ─────────────────────────────────────────────────────────────────────
function agente_rate_limit(string $clave, int $max, int $ventanaSeg): array {
    $r = agente_redis();
    if (!$r) {
        return ['permitido' => true, 'restantes' => $max, 'reset' => $ventanaSeg, 'backend' => 'none'];
    }
    try {
        $key = "rl:$clave";
        $count = $r->incr($key);
        if ($count === 1) $r->expire($key, $ventanaSeg);
        $ttl = $r->ttl($key);
        return [
            'permitido' => $count <= $max,
            'restantes' => max(0, $max - $count),
            'reset'     => $ttl >= 0 ? $ttl : $ventanaSeg,
            'backend'   => 'redis'
        ];
    } catch (\Throwable $e) {
        agente_log('warn', 'rate_limit_redis_error', ['err' => $e->getMessage()]);
        return ['permitido' => true, 'restantes' => $max, 'reset' => $ventanaSeg, 'backend' => 'error'];
    }
}

// ─────────────────────────────────────────────────────────────────────
// Cache helper. Devuelve valor cacheado o lo calcula con $loader().
// ─────────────────────────────────────────────────────────────────────
function agente_cache(string $clave, int $ttl, callable $loader) {
    $r = agente_redis();
    if (!$r) return $loader();

    try {
        $cached = $r->get($clave);
        if ($cached !== false) return json_decode($cached, true);
        $value = $loader();
        $r->setex($clave, $ttl, json_encode($value, JSON_UNESCAPED_UNICODE));
        return $value;
    } catch (\Throwable $e) {
        agente_log('warn', 'cache_error', ['err' => $e->getMessage()]);
        return $loader();
    }
}

function agente_cache_delete(string $patron): int {
    $r = agente_redis();
    if (!$r) return 0;
    try {
        $eliminadas = 0;
        $iterator = null;
        while ($keys = $r->scan($iterator, $patron, 100)) {
            $eliminadas += $r->del(...$keys);
        }
        return $eliminadas;
    } catch (\Throwable) {
        return 0;
    }
}

// ─────────────────────────────────────────────────────────────────────
// Push notifications a través de OneSignal.
// Envía un push a una lista de subscription_ids con título/cuerpo y data.
// Las credenciales de OneSignal se cargan de agente.env si están definidas;
// si no, se usan las mismas hardcoded que el resto de PHPs del proyecto.
// ─────────────────────────────────────────────────────────────────────
const ONESIGNAL_APP_ID_DEFAULT      = '000cf2d3-9e1c-40c9-a6e6-56bafe0b3946';
const ONESIGNAL_REST_API_KEY_DEFAULT = 'os_v2_app_aagpfu46dramtjxgk25p4czzi2b5tztqre5u5rupmrx223buaxs4l4awhwlsdccw2iala2uttjvndnnva4q26f4gbgywelarzqyruuq';

function agente_push(array $subscriptionIds, string $titulo, string $mensaje, array $data = []): bool {
    if (count($subscriptionIds) === 0) return false;

    $appId  = agente_env('ONESIGNAL_APP_ID',      ONESIGNAL_APP_ID_DEFAULT);
    $apiKey = agente_env('ONESIGNAL_REST_API_KEY', ONESIGNAL_REST_API_KEY_DEFAULT);

    $payload = [
        'app_id'                   => $appId,
        'include_subscription_ids' => array_values($subscriptionIds),
        'target_channel'           => 'push',
        'contents'                 => ['en' => $mensaje, 'es' => $mensaje],
        'headings'                 => ['en' => $titulo,  'es' => $titulo],
        'large_icon'               => 'icon_notification.png',
        'data'                     => $data
    ];

    try {
        $ch = curl_init('https://api.onesignal.com/notifications');
        curl_setopt($ch, CURLOPT_HTTPHEADER, [
            'Content-Type: application/json; charset=utf-8',
            'Authorization: Key ' . $apiKey
        ]);
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
        curl_setopt($ch, CURLOPT_POST, true);
        curl_setopt($ch, CURLOPT_POSTFIELDS, json_encode($payload, JSON_UNESCAPED_UNICODE));
        curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, false);
        curl_setopt($ch, CURLOPT_TIMEOUT, 5);
        curl_setopt($ch, CURLOPT_CONNECTTIMEOUT, 3);

        $resp = curl_exec($ch);
        $http = (int)curl_getinfo($ch, CURLINFO_HTTP_CODE);

        $ok = $http >= 200 && $http < 300;
        agente_log($ok ? 'info' : 'warn', 'push_enviado', [
            'http'           => $http,
            'destinatarios'  => count($subscriptionIds),
            'titulo'         => $titulo,
            'response_head'  => substr((string)$resp, 0, 200)
        ]);
        return $ok;
    } catch (\Throwable $e) {
        agente_log('error', 'push_excepcion', ['err' => $e->getMessage()]);
        return false;
    }
}

// ─────────────────────────────────────────────────────────────────────
// SMTP minimalista para alertas operativas.
// Conexión TLS implícita (puerto 465). Sólo soporta AUTH LOGIN, que es
// lo que aceptan los servidores de la mayoría de proveedores estándar.
// ─────────────────────────────────────────────────────────────────────
function agente_smtp_send(string $to, string $subject, string $body): bool {
    $host = agente_env('SMTP_HOST', 'mail.qoorder.com');
    $port = (int)agente_env('SMTP_PORT', '465');
    $user = agente_env('SMTP_USER', 'conta@qoorder.com');
    $pass = agente_env('SMTP_PASS', '');
    $from = agente_env('SMTP_FROM', $user);

    if ($pass === '' || $to === '') {
        agente_log('warn', 'smtp_no_configurado', ['to' => $to]);
        return false;
    }

    $errno = 0; $errstr = '';
    $sock = @stream_socket_client("ssl://{$host}:{$port}", $errno, $errstr, 10);
    if (!$sock) {
        agente_log('error', 'smtp_connect_fail', ['host' => $host, 'err' => $errstr]);
        return false;
    }
    stream_set_timeout($sock, 10);

    $send = function(string $cmd) use ($sock) {
        fwrite($sock, $cmd . "\r\n");
        $resp = '';
        while (($line = fgets($sock, 512)) !== false) {
            $resp .= $line;
            if (strlen($line) >= 4 && $line[3] === ' ') break;
        }
        return $resp;
    };
    $read = function() use ($sock) {
        $resp = '';
        while (($line = fgets($sock, 512)) !== false) {
            $resp .= $line;
            if (strlen($line) >= 4 && $line[3] === ' ') break;
        }
        return $resp;
    };

    try {
        $read();
        $send("EHLO " . ($_SERVER['SERVER_NAME'] ?? 'agente'));
        $send("AUTH LOGIN");
        $send(base64_encode($user));
        $send(base64_encode($pass));
        $send("MAIL FROM:<{$from}>");
        $send("RCPT TO:<{$to}>");
        $send("DATA");
        $headers  = "From: Asador Morón Agente <{$from}>\r\n";
        $headers .= "To: {$to}\r\n";
        $headers .= "Subject: " . mb_encode_mimeheader($subject, 'UTF-8') . "\r\n";
        $headers .= "MIME-Version: 1.0\r\nContent-Type: text/plain; charset=UTF-8\r\n";
        fwrite($sock, $headers . "\r\n" . $body . "\r\n.\r\n");
        $finish = $read();
        $send("QUIT");
        fclose($sock);

        $ok = strpos($finish, '250') !== false;
        agente_log($ok ? 'info' : 'warn', 'smtp_send', ['to' => $to, 'subject' => $subject, 'resp' => trim($finish)]);
        return $ok;
    } catch (\Throwable $e) {
        @fclose($sock);
        agente_log('error', 'smtp_excepcion', ['err' => $e->getMessage()]);
        return false;
    }
}

/**
 * Envía una alerta al canal configurado (email + opcional Slack webhook).
 */
function agente_alert(string $titulo, string $cuerpo, array $contexto = []): void {
    $email = agente_env('ALERTA_EMAIL', '');
    if ($email !== '') {
        $extra = $contexto ? "\n\nContexto:\n" . json_encode($contexto, JSON_PRETTY_PRINT | JSON_UNESCAPED_UNICODE) : '';
        agente_smtp_send($email, "[Agente Asador] {$titulo}", $cuerpo . $extra);
    }

    $slack = agente_env('ALERTA_SLACK_WEBHOOK', '');
    if ($slack !== '' && filter_var($slack, FILTER_VALIDATE_URL)) {
        try {
            $payload = json_encode(['text' => "*[Agente Asador]* {$titulo}\n{$cuerpo}"]);
            $ch = curl_init($slack);
            curl_setopt($ch, CURLOPT_HTTPHEADER, ['Content-Type: application/json']);
            curl_setopt($ch, CURLOPT_POST, true);
            curl_setopt($ch, CURLOPT_POSTFIELDS, $payload);
            curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
            curl_setopt($ch, CURLOPT_TIMEOUT, 5);
            curl_exec($ch);
        } catch (\Throwable $e) {
            agente_log('warn', 'slack_alert_fail', ['err' => $e->getMessage()]);
        }
    }
}

/**
 * Devuelve los subscription_ids (tokens OneSignal) del staff de un establecimiento.
 * Incluye: administradores del pueblo (rol 2 = Establecimiento, rol 3 = Administrador).
 */
function agente_tokens_staff(PDO $db, int $idEstablecimiento): array {
    try {
        $stmt = $db->prepare("
            SELECT DISTINCT u.token
            FROM qo_users u
            JOIN qo_establecimientos e ON e.idPueblo = u.idPueblo
            WHERE e.id = :est
              AND u.rol IN (2, 3)
              AND u.estado = 1
              AND u.bloqueado = 0
              AND u.token IS NOT NULL AND u.token <> ''
        ");
        $stmt->bindValue(':est', $idEstablecimiento, PDO::PARAM_INT);
        $stmt->execute();
        return $stmt->fetchAll(PDO::FETCH_COLUMN) ?: [];
    } catch (\Throwable $e) {
        agente_log('warn', 'tokens_staff_error', ['err' => $e->getMessage()]);
        return [];
    }
}

// ─────────────────────────────────────────────────────────────────────
// Normalización de transcripción andaluza (P5). Corrige errores típicos de
// STT palabra a palabra para almacenar `texto_normalizado` y para métricas.
// La colación de BD ya es insensible a acentos, así que aquí solo mapeamos
// confusiones frecuentes (no tocamos tildes).
// ─────────────────────────────────────────────────────────────────────
function normalizar_transcripcion(string $texto): string {
    static $map = [
        'boyo'=>'pollo','bollo'=>'pollo','rollo'=>'pollo','poyo'=>'pollo',
        'patada'=>'patatas','papas'=>'patatas','papa'=>'patata',
        'chocha'=>'chocos','choco'=>'chocos',
        'ganbas'=>'gambas','ganbon'=>'gambón',
        'anburgesa'=>'hamburguesa','amburguesa'=>'hamburguesa','hamburgesa'=>'hamburguesa',
        'pisa'=>'pizza',
        'ganbarjillo'=>'gambas al ajillo',
    ];
    $palabras = preg_split('/(\s+)/u', $texto, -1, PREG_SPLIT_DELIM_CAPTURE);
    $out = '';
    foreach ($palabras as $tok) {
        if (trim($tok) === '') { $out .= $tok; continue; }
        $low = mb_strtolower($tok, 'UTF-8');
        $out .= $map[$low] ?? $tok;
    }
    return $out;
}
