-- Rollback de 002_order_line_customization.sql
-- Elimina las columnas de personalización de línea. Seguro: son NULL y no las
-- referencian triggers ni claves foráneas.

ALTER TABLE `qo_pedidos_detalle` DROP INDEX IF EXISTS `idx_detalle_idOpcion`;

ALTER TABLE `qo_pedidos_detalle`
  DROP COLUMN IF EXISTS `ingredientes_json`,
  DROP COLUMN IF EXISTS `idOpcion`;
