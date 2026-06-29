-- ============================================================================
-- Rollback de la migración 001 — Tablas del agente de voz IA
-- ATENCIÓN: borra todas las llamadas, transcripciones y reglas registradas.
-- Solo ejecutar en staging o si se decide cancelar el proyecto.
-- ============================================================================

SET FOREIGN_KEY_CHECKS = 0;

ALTER TABLE qo_pedidos
    DROP INDEX IF EXISTS idx_origen,
    DROP INDEX IF EXISTS idx_llamada;

ALTER TABLE qo_pedidos
    DROP COLUMN IF EXISTS origen,
    DROP COLUMN IF EXISTS llamada_id;

DROP TABLE IF EXISTS qo_transcripciones;
DROP TABLE IF EXISTS qo_llamadas;
DROP TABLE IF EXISTS qo_reglas_asignacion;
DROP TABLE IF EXISTS qo_blacklist_telefonos;
DROP TABLE IF EXISTS qo_config_agente;

SET FOREIGN_KEY_CHECKS = 1;
