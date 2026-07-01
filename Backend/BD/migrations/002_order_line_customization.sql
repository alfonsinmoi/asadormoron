-- ============================================================================
-- 002_order_line_customization.sql
-- P1 del bloque de personalización (auditoría 2026-07-01).
--
-- Objetivo: que qo_pedidos_detalle pueda representar un pedido COMPLETO igual
-- que la app móvil: opción de producto elegida + ingredientes añadidos/quitados
-- de forma ESTRUCTURADA (no solo texto libre en `concepto`).
--
-- Seguridad:
--   - Ambas columnas son NULL (no rompen filas ni INSERTs existentes).
--   - Todos los INSERT del código usan lista de columnas explícita (verificado),
--     así que no hay INSERTs posicionales que se rompan al añadir columnas.
--   - Los triggers completo_ins2/completo_upd2 solo leen NEW.tipo → no afectados.
--   - ingredientes_json se declara LONGTEXT (no JSON nativo) para no introducir
--     un CHECK json_valid() que pudiera rechazar escrituras legacy; la validez
--     JSON se garantiza en la capa de aplicación (contrato de línea).
--
-- Idempotente: usa ADD COLUMN IF NOT EXISTS (MariaDB 10.11).
-- ============================================================================

ALTER TABLE `qo_pedidos_detalle`
  ADD COLUMN IF NOT EXISTS `idOpcion` INT NULL AFTER `idProducto`,
  ADD COLUMN IF NOT EXISTS `ingredientes_json` LONGTEXT NULL AFTER `concepto`;

-- Índice para reportes/consultas por opción (opcional, ligero).
ALTER TABLE `qo_pedidos_detalle`
  ADD INDEX IF NOT EXISTS `idx_detalle_idOpcion` (`idOpcion`);

-- ----------------------------------------------------------------------------
-- Contrato de línea canónico (voz ↔ app). Documentado aquí como referencia:
--
--   {
--     "idProducto": 123,
--     "cantidad": 1,
--     "idOpcion": 45,                 -- NULL si el producto no tiene opciones
--     "ingredientes": [               -- [] si no aplica
--       { "idIngrediente": 9,  "esAnadir": true,  "precio": 1.50 },
--       { "idIngrediente": 12, "esAnadir": false, "precio": 0.00 }
--     ],
--     "precio": 12.90,                -- precio_final RECALCULADO server-side
--     "concepto": "Pollo asado entero, sin piel",
--     "comentario": ""
--   }
--
--   precio_final = precio_base(qo_productos_est)
--                + valorIncremento(qo_productos_opc del idOpcion)
--                + SUM(precio de ingredientes con esAnadir=true)
--
-- Tipos de línea en qo_pedidos_detalle.tipo:
--   0 = producto | 1 = gastos de envío | 2 = bolsa/embalaje (P3)
-- ----------------------------------------------------------------------------
