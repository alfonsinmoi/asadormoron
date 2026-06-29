-- ============================================================================
-- Migración 001 — Tablas del agente de voz IA
-- Fecha: 2026-05-11
-- Plan: docs/Plan_Implementacion_Agente_Voz.md §Fase 1
-- Target: MariaDB 10.11+ (verificado en 82.223.139.121)
--
-- Cambios:
--   1. Nuevas tablas: llamadas, transcripciones, reglas_asignacion,
--      blacklist_telefonos, config_agente
--   2. Ampliación de qo_pedidos: solo `origen` y `llamada_id`
--      (idRepartidor y horaEntrega ya existen — se reutilizan)
--   3. Semilla de configuración por defecto del agente
--
-- Características:
--   - Todo es aditivo (CREATE / ALTER ADD COLUMN). Cero riesgo para producción.
--   - Idempotente: usa IF NOT EXISTS donde MariaDB lo soporta.
--   - Charset utf8mb4 para soportar emojis y acentos en transcripciones.
-- ============================================================================

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 1;

-- ----------------------------------------------------------------------------
-- 1. qo_llamadas — registro de llamadas atendidas por el agente
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS qo_llamadas (
    id BIGINT PRIMARY KEY AUTO_INCREMENT,
    vapi_call_id VARCHAR(100) NOT NULL UNIQUE,
    telefono_origen VARCHAR(20) DEFAULT NULL,
    cliente_id BIGINT DEFAULT NULL COMMENT 'FK lógica a qo_users.id',
    idEstablecimiento INT DEFAULT NULL COMMENT 'Establecimiento que atiende',
    estado ENUM('en_curso','completada','transferida','no_pedido','fallida') NOT NULL DEFAULT 'en_curso',
    pedido_id INT DEFAULT NULL COMMENT 'FK lógica a qo_pedidos.id (int)',
    duracion_segundos INT DEFAULT 0,
    coste_estimado DECIMAL(8,4) DEFAULT 0.0000,
    audio_url VARCHAR(500) DEFAULT NULL,
    fecha_inicio DATETIME NOT NULL,
    fecha_fin DATETIME DEFAULT NULL,
    metadatos JSON DEFAULT NULL,
    fecha_creacion DATETIME DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_fecha (fecha_inicio),
    INDEX idx_estado (estado),
    INDEX idx_telefono (telefono_origen),
    INDEX idx_pedido (pedido_id),
    INDEX idx_establecimiento (idEstablecimiento)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------------------------------------------------------
-- 2. qo_transcripciones — transcripción turno a turno de cada llamada
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS qo_transcripciones (
    id BIGINT PRIMARY KEY AUTO_INCREMENT,
    llamada_id BIGINT NOT NULL,
    texto LONGTEXT NOT NULL COMMENT 'Texto plano concatenado para búsqueda',
    texto_estructurado JSON DEFAULT NULL COMMENT 'Array de {rol, texto, timestamp}',
    fecha DATETIME NOT NULL,
    INDEX idx_llamada (llamada_id),
    INDEX idx_fecha (fecha),
    FULLTEXT KEY ft_texto (texto),
    CONSTRAINT fk_transcripcion_llamada
        FOREIGN KEY (llamada_id) REFERENCES qo_llamadas(id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------------------------------------------------------
-- 3. qo_reglas_asignacion — motor de reglas para asignar repartidor
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS qo_reglas_asignacion (
    id INT PRIMARY KEY AUTO_INCREMENT,
    idEstablecimiento INT DEFAULT NULL COMMENT 'NULL = regla global',
    nombre VARCHAR(100) NOT NULL,
    prioridad INT NOT NULL DEFAULT 0 COMMENT 'Menor número = mayor prioridad',
    tipo ENUM('zona','carga','vehiculo','turno','round_robin') NOT NULL,
    parametros JSON NOT NULL,
    activa TINYINT(1) NOT NULL DEFAULT 1,
    fecha_creacion DATETIME DEFAULT CURRENT_TIMESTAMP,
    fecha_actualizacion DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    INDEX idx_prioridad (prioridad),
    INDEX idx_establecimiento (idEstablecimiento),
    INDEX idx_activa (activa)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------------------------------------------------------
-- 4. qo_blacklist_telefonos — números bloqueados
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS qo_blacklist_telefonos (
    telefono VARCHAR(20) PRIMARY KEY,
    motivo VARCHAR(255) DEFAULT NULL,
    bloqueado_por VARCHAR(100) DEFAULT NULL COMMENT 'auto | nombre del admin',
    fecha DATETIME DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_fecha (fecha)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------------------------------------------------------
-- 5. qo_config_agente — configuración global del agente (clave/valor JSON)
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS qo_config_agente (
    clave VARCHAR(100) PRIMARY KEY,
    valor JSON NOT NULL,
    descripcion VARCHAR(255) DEFAULT NULL,
    fecha_actualizacion DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------------------------------------------------------
-- 6. Ampliación de qo_pedidos
--    Solo dos columnas nuevas. idRepartidor y horaEntrega ya existen y se reutilizan.
-- ----------------------------------------------------------------------------
ALTER TABLE qo_pedidos
    ADD COLUMN IF NOT EXISTS origen ENUM('app','voz','manual','web') NOT NULL DEFAULT 'manual' AFTER id,
    ADD COLUMN IF NOT EXISTS llamada_id BIGINT DEFAULT NULL AFTER origen;

-- Índices (idempotente vía IF NOT EXISTS desde MariaDB 10.5.4)
ALTER TABLE qo_pedidos
    ADD INDEX IF NOT EXISTS idx_origen (origen),
    ADD INDEX IF NOT EXISTS idx_llamada (llamada_id);

-- ----------------------------------------------------------------------------
-- 7. Semilla de configuración por defecto
-- ----------------------------------------------------------------------------
-- Nota MariaDB: el tipo JSON es alias de LONGTEXT, por lo que CAST(x AS JSON)
-- no es válido. Pasamos los valores como string-JSON o usando JSON_OBJECT/JSON_ARRAY.
INSERT INTO qo_config_agente (clave, valor, descripcion) VALUES
    ('saludo_personalizado',
     '"Hola, soy el asistente del Asador Morón. ¿En qué puedo ayudarte?"',
     'Saludo inicial del agente'),
    ('voz_id',                          '""',  'ID de voz de ElevenLabs'),
    ('tiempo_preparacion_base_minutos', '20',  'Tiempo base de preparación de cocina'),
    ('tiempo_extra_por_pollo',          '5',   'Minutos extra por cada pollo en cola'),
    ('velocidad_reparto_kmh',           '25',  'Velocidad media estimada del reparto'),
    ('buffer_seguridad_minutos',        '5',   'Buffer añadido a la hora estimada'),
    ('max_llamadas_hora_por_numero',    '3',   'Anti-abuso: llamadas/hora desde mismo número'),
    ('max_importe_sin_humano_eur',      '100', 'Pedidos por encima de este importe requieren confirmación humana'),
    ('modo_saturacion_umbral',          '10',  'Pollos en cola a partir de los cuales se activa modo saturación'),
    ('presupuesto_diario_eur',          '50',  'Alerta si el coste diario supera este valor'),
    ('horario',
     JSON_OBJECT(
        'lunes',     JSON_ARRAY('12:00','16:00','20:00','23:30'),
        'martes',    JSON_ARRAY('12:00','16:00','20:00','23:30'),
        'miercoles', JSON_ARRAY('12:00','16:00','20:00','23:30'),
        'jueves',    JSON_ARRAY('12:00','16:00','20:00','23:30'),
        'viernes',   JSON_ARRAY('12:00','16:00','20:00','23:30'),
        'sabado',    JSON_ARRAY('12:00','16:00','20:00','23:30'),
        'domingo',   JSON_ARRAY('12:00','16:00','20:00','23:30')
     ),
     'Horario de apertura por día (pares apertura/cierre)')
ON DUPLICATE KEY UPDATE clave = clave;  -- no-op si ya existe

-- ----------------------------------------------------------------------------
-- 8. Semilla de reglas de asignación por defecto (round-robin)
-- ----------------------------------------------------------------------------
INSERT INTO qo_reglas_asignacion (idEstablecimiento, nombre, prioridad, tipo, parametros, activa)
SELECT NULL, 'Solo turno activo', 10, 'turno',
       JSON_OBJECT('solo_jornada_activa', true), 1
WHERE NOT EXISTS (SELECT 1 FROM qo_reglas_asignacion WHERE nombre = 'Solo turno activo');

INSERT INTO qo_reglas_asignacion (idEstablecimiento, nombre, prioridad, tipo, parametros, activa)
SELECT NULL, 'Carga máxima 3', 20, 'carga',
       JSON_OBJECT('max_pedidos_simultaneos', 3), 1
WHERE NOT EXISTS (SELECT 1 FROM qo_reglas_asignacion WHERE nombre = 'Carga máxima 3');

INSERT INTO qo_reglas_asignacion (idEstablecimiento, nombre, prioridad, tipo, parametros, activa)
SELECT NULL, 'Desempate round-robin', 99, 'round_robin',
       JSON_OBJECT(), 1
WHERE NOT EXISTS (SELECT 1 FROM qo_reglas_asignacion WHERE nombre = 'Desempate round-robin');

-- ============================================================================
-- Fin migración 001
-- ============================================================================
