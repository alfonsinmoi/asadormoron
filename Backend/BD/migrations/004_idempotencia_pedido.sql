-- ============================================================================
-- 004_idempotencia_pedido.sql
-- Hardening: backstop de idempotencia a nivel BD. Una llamada (qo_llamadas)
-- solo puede vincularse a UN pedido. Si dos tool-calls concurrentes del mismo
-- callId intentan crear pedido, el segundo UPDATE pedido_id perderá la carrera
-- (índice único) y la transacción hará rollback, en vez de duplicar el pedido.
--
-- NULL permitido y múltiple (llamadas sin pedido): MySQL/MariaDB permite varios
-- NULL en un índice UNIQUE. Verificado: no hay pedido_id duplicados actuales.
-- Idempotente (IF NOT EXISTS).
-- ============================================================================

ALTER TABLE `qo_llamadas`
  ADD UNIQUE INDEX IF NOT EXISTS `uniq_llamada_pedido` (`pedido_id`);
