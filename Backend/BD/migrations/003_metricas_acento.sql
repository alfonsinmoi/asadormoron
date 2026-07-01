-- ============================================================================
-- 003_metricas_acento.sql
-- P5 — métricas de reconocimiento de acento andaluz.
--
--  - qo_agente_busquedas: registra cada búsqueda de get_menu con el término
--    original, el normalizado y el nº de resultados. Permite medir la "tasa de
--    reconocimiento" (búsquedas que encuentran producto / total) como proxy de
--    calidad de transcripción, sin necesidad de corpus etiquetado.
--  - qo_transcripciones.texto_normalizado: versión normalizada del turno del
--    cliente (para auditar original vs normalizado en el detalle de llamada).
-- Idempotente (IF NOT EXISTS).
-- ============================================================================

CREATE TABLE IF NOT EXISTS `qo_agente_busquedas` (
  `id`                INT NOT NULL AUTO_INCREMENT,
  `idEstablecimiento` INT NOT NULL,
  `termino`           VARCHAR(255) NOT NULL,
  `termino_norm`      VARCHAR(255) NOT NULL,
  `resultados`        INT NOT NULL DEFAULT 0,
  `fecha`             DATETIME NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_bus_est_fecha` (`idEstablecimiento`, `fecha`),
  KEY `idx_bus_resultados` (`resultados`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

ALTER TABLE `qo_transcripciones`
  ADD COLUMN IF NOT EXISTS `texto_normalizado` TEXT NULL AFTER `texto`;
