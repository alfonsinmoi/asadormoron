-- ============================================================================
-- OPTIMIZACIONES DE RENDIMIENTO - BASE DE DATOS ASADOR MORÓN
-- ============================================================================
-- Ejecutar este script para mejorar significativamente el rendimiento de las
-- consultas SQL. Se recomienda ejecutarlo en horario de bajo tráfico.
-- ============================================================================

-- Configuración para el script
SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;

-- ============================================================================
-- PARTE 1: ÍNDICES CRÍTICOS (IMPACTO ALTO)
-- ============================================================================
-- Estas tablas se consultan frecuentemente en la app y carecen de índices.

-- -----------------------------------------------------------------------------
-- 1.1 qo_productos_cat (Categorías de productos)
-- Consulta crítica: WHERE idEstablecimiento=:id ORDER BY orden
-- -----------------------------------------------------------------------------
ALTER TABLE `qo_productos_cat`
    ADD INDEX `idx_idEstablecimiento` (`idEstablecimiento`),
    ADD INDEX `idx_idEstablecimiento_orden` (`idEstablecimiento`, `orden`),
    ADD INDEX `idx_estado` (`estado`);

-- -----------------------------------------------------------------------------
-- 1.2 qo_productos_est (Productos del establecimiento)
-- Consulta crítica: WHERE idCategoria=:id AND eliminado=0 ORDER BY nombre
-- -----------------------------------------------------------------------------
ALTER TABLE `qo_productos_est`
    ADD INDEX `idx_idCategoria` (`idCategoria`),
    ADD INDEX `idx_idCategoria_eliminado` (`idCategoria`, `eliminado`),
    ADD INDEX `idx_idCategoria_estado` (`idCategoria`, `estado`),
    ADD INDEX `idx_eliminado` (`eliminado`),
    ADD INDEX `idx_estado` (`estado`);

-- -----------------------------------------------------------------------------
-- 1.3 qo_configuracion_est (Configuración de establecimiento)
-- Consulta crítica: WHERE idEstablecimiento=:id
-- -----------------------------------------------------------------------------
ALTER TABLE `qo_configuracion_est`
    ADD INDEX `idx_idEstablecimiento` (`idEstablecimiento`);

-- -----------------------------------------------------------------------------
-- 1.4 qo_puntos_usuario (Puntos de usuario por establecimiento)
-- Consulta crítica: WHERE idEstablecimiento=:id AND idUsuario=:id
-- -----------------------------------------------------------------------------
ALTER TABLE `qo_puntos_usuario`
    ADD INDEX `idx_idUsuario` (`idUsuario`),
    ADD INDEX `idx_idEstablecimiento` (`idEstablecimiento`),
    ADD UNIQUE INDEX `idx_usuario_establecimiento` (`idUsuario`, `idEstablecimiento`);

-- -----------------------------------------------------------------------------
-- 1.5 qo_establecimientos (Establecimientos)
-- Consulta crítica: WHERE idGrupo=:id AND estado=1
-- -----------------------------------------------------------------------------
ALTER TABLE `qo_establecimientos`
    ADD INDEX `idx_idGrupo` (`idGrupo`),
    ADD INDEX `idx_idPueblo` (`idPueblo`),
    ADD INDEX `idx_estado` (`estado`),
    ADD INDEX `idx_idGrupo_estado` (`idGrupo`, `estado`);

-- ============================================================================
-- PARTE 2: ÍNDICES IMPORTANTES (IMPACTO MEDIO)
-- ============================================================================

-- -----------------------------------------------------------------------------
-- 2.1 qo_pedidos (Pedidos) - ÍNDICES CRÍTICOS PARA RENDIMIENTO
-- Consultas: historico, por usuario, por establecimiento, por fecha
-- -----------------------------------------------------------------------------
ALTER TABLE `qo_pedidos`
    ADD INDEX `idx_idUsuario` (`idUsuario`),
    ADD INDEX `idx_idEstablecimiento` (`idEstablecimiento`),
    ADD INDEX `idx_horaPedido` (`horaPedido`),
    ADD INDEX `idx_estado` (`estado`),
    ADD INDEX `idx_idEstablecimiento_horaPedido` (`idEstablecimiento`, `horaPedido`),
    ADD INDEX `idx_idUsuario_idEstablecimiento` (`idUsuario`, `idEstablecimiento`),
    ADD INDEX `idx_fechaCierre` (`fechaCierre`),
    ADD INDEX `idx_idRepartidor` (`idRepartidor`),
    -- NUEVOS: Índices compuestos para consultas principales de pedidos.php
    ADD INDEX `idx_est_estado_anulado` (`idEstablecimiento`, `estado`, `anulado`),
    ADD INDEX `idx_estado_anulado` (`estado`, `anulado`),
    ADD INDEX `idx_idCuenta` (`idCuenta`),
    ADD INDEX `idx_codigo` (`codigo`),
    ADD INDEX `idx_completo_anulado` (`completo`, `anulado`);

-- -----------------------------------------------------------------------------
-- 2.2 qo_pedidos_detalle (Detalle de pedidos)
-- Consultas: detalles por pedido, productos más vendidos
-- -----------------------------------------------------------------------------
ALTER TABLE `qo_pedidos_detalle`
    ADD INDEX `idx_idProducto` (`idProducto`),
    ADD INDEX `idx_tipoVenta` (`tipoVenta`);
    -- Ya tiene índice en idPedido

-- -----------------------------------------------------------------------------
-- 2.3 qo_productos_opc (Opciones de producto)
-- Consulta: GROUP BY idProducto
-- -----------------------------------------------------------------------------
ALTER TABLE `qo_productos_opc`
    ADD INDEX `idx_idProducto` (`idProducto`);

-- -----------------------------------------------------------------------------
-- 2.4 qo_ingredientes_producto (Ingredientes por producto)
-- Consulta: WHERE idProducto=:id
-- -----------------------------------------------------------------------------
ALTER TABLE `qo_ingredientes_producto`
    ADD INDEX `idx_idProducto` (`idProducto`),
    ADD INDEX `idx_idIngrediente` (`idIngrediente`);

-- -----------------------------------------------------------------------------
-- 2.5 qo_productos_ing_aler (Alérgenos por producto)
-- Consulta: WHERE idProducto=:id
-- -----------------------------------------------------------------------------
ALTER TABLE `qo_productos_ing_aler`
    ADD INDEX `idx_idProducto` (`idProducto`),
    ADD INDEX `idx_idAlergeno` (`idAlergeno`);

-- -----------------------------------------------------------------------------
-- 2.6 qo_ingredientes_establecimiento (Ingredientes por establecimiento)
-- Consulta: WHERE idEstablecimiento=:id AND estado=1
-- -----------------------------------------------------------------------------
ALTER TABLE `qo_ingredientes_establecimiento`
    ADD INDEX `idx_idEstablecimiento` (`idEstablecimiento`),
    ADD INDEX `idx_estado` (`estado`),
    ADD INDEX `idx_idEstablecimiento_estado` (`idEstablecimiento`, `estado`);

-- -----------------------------------------------------------------------------
-- 2.7 qo_users (Usuarios)
-- Consultas: login, búsqueda por email, por pueblo
-- -----------------------------------------------------------------------------
ALTER TABLE `qo_users`
    ADD INDEX `idx_email` (`email`),
    ADD INDEX `idx_idPueblo` (`idPueblo`),
    ADD INDEX `idx_estado` (`estado`),
    ADD INDEX `idx_token` (`token`(100));

-- -----------------------------------------------------------------------------
-- 2.8 qo_configuracion (Configuración por pueblo)
-- Consulta: WHERE idPueblo=:id
-- -----------------------------------------------------------------------------
ALTER TABLE `qo_configuracion`
    ADD INDEX `idx_idPueblo` (`idPueblo`),
    ADD INDEX `idx_idGrupo` (`idGrupo`);

-- -----------------------------------------------------------------------------
-- 2.9 qo_categorias (Categorías generales)
-- Consulta: WHERE idGrupo=:id ORDER BY orden
-- -----------------------------------------------------------------------------
ALTER TABLE `qo_categorias`
    ADD INDEX `idx_idGrupo` (`idGrupo`),
    ADD INDEX `idx_idPueblo` (`idPueblo`),
    ADD INDEX `idx_estado` (`estado`),
    ADD INDEX `idx_idGrupo_orden` (`idGrupo`, `orden`);

-- ============================================================================
-- PARTE 3: ÍNDICES SECUNDARIOS (IMPACTO BAJO-MEDIO)
-- ============================================================================

-- 3.1 qo_repartidores
ALTER TABLE `qo_repartidores`
    ADD INDEX `idx_idPueblo` (`idPueblo`),
    ADD INDEX `idx_idGrupo` (`idGrupo`),
    ADD INDEX `idx_activo` (`activo`);

-- 3.2 qo_repartidores_pedidos
ALTER TABLE `qo_repartidores_pedidos`
    ADD INDEX `idx_idRepartidor` (`idRepartidor`),
    ADD INDEX `idx_fechaAsignacion` (`fechaAsignacion`);

-- 3.3 qo_cupones
ALTER TABLE `qo_cupones`
    ADD INDEX `idx_codigoCupon` (`codigoCupon`),
    ADD INDEX `idx_estado` (`estado`),
    ADD INDEX `idx_idGrupo` (`idGrupo`);

-- 3.4 qo_cupones_usuario
ALTER TABLE `qo_cupones_usuario`
    ADD INDEX `idx_idUsuario` (`idUsuario`),
    ADD INDEX `idx_idCupon` (`idCupon`),
    ADD INDEX `idx_utilizado` (`utilizado`);

-- 3.5 qo_valoraciones
ALTER TABLE `qo_valoraciones`
    ADD INDEX `idx_idEstablecimiento` (`idEstablecimiento`),
    ADD INDEX `idx_idUsuario` (`idUsuario`);

-- 3.6 qo_pueblos
ALTER TABLE `qo_pueblos`
    ADD INDEX `idx_idGrupo` (`idGrupo`),
    ADD INDEX `idx_activo` (`activo`);

-- 3.7 qo_menu_diario
ALTER TABLE `qo_menu_diario`
    ADD INDEX `idx_idEstablecimiento` (`idEstablecimiento`),
    ADD INDEX `idx_activo` (`activo`);

-- 3.8 qo_menu_diario_conf
ALTER TABLE `qo_menu_diario_conf`
    ADD INDEX `idx_idEstablecimiento` (`idEstablecimiento`);

-- 3.9 qo_menu_diario_prod
-- Ya tiene índice en idMenu

-- 3.10 qo_online (usuarios online)
ALTER TABLE `qo_online`
    ADD INDEX `idx_idUsuario` (`idUsuario`),
    ADD INDEX `idx_idPueblo` (`idPueblo`),
    ADD INDEX `idx_horaInicio` (`horaInicio`);

-- 3.11 qo_users_est (relación usuario-establecimiento)
ALTER TABLE `qo_users_est`
    ADD INDEX `idx_idUser` (`idUser`),
    ADD INDEX `idx_idEstablecimiento` (`idEstablecimiento`);

-- ============================================================================
-- PARTE 4: OPTIMIZACIÓN DE TABLAS EXISTENTES
-- ============================================================================
-- Reorganiza los datos y actualiza las estadísticas de los índices

OPTIMIZE TABLE `qo_productos_cat`;
OPTIMIZE TABLE `qo_productos_est`;
OPTIMIZE TABLE `qo_configuracion_est`;
OPTIMIZE TABLE `qo_establecimientos`;
OPTIMIZE TABLE `qo_pedidos`;
OPTIMIZE TABLE `qo_pedidos_detalle`;
OPTIMIZE TABLE `qo_users`;
OPTIMIZE TABLE `qo_puntos_usuario`;

-- ============================================================================
-- PARTE 5: ANÁLISIS DE TABLAS
-- ============================================================================
-- Actualiza las estadísticas para que el optimizador de consultas funcione mejor

ANALYZE TABLE `qo_productos_cat`;
ANALYZE TABLE `qo_productos_est`;
ANALYZE TABLE `qo_configuracion_est`;
ANALYZE TABLE `qo_establecimientos`;
ANALYZE TABLE `qo_pedidos`;
ANALYZE TABLE `qo_pedidos_detalle`;
ANALYZE TABLE `qo_users`;
ANALYZE TABLE `qo_puntos_usuario`;
ANALYZE TABLE `qo_categorias`;
ANALYZE TABLE `qo_configuracion`;

-- Confirmar transacción
COMMIT;

-- ============================================================================
-- RESUMEN DE IMPACTO ESPERADO
-- ============================================================================
--
-- | Consulta                           | Antes        | Después      | Mejora   |
-- |------------------------------------|--------------|--------------|----------|
-- | Categorías por establecimiento     | Full scan    | Index scan   | 10-50x   |
-- | Productos por categoría            | Full scan    | Index seek   | 10-100x  |
-- | Configuración establecimiento      | Full scan    | Index seek   | 5-20x    |
-- | Puntos usuario                     | Full scan    | Index seek   | 10-50x   |
-- | Histórico pedidos                  | Full scan    | Index range  | 10-100x  |
-- | Productos más vendidos             | Full scan    | Index scan   | 5-20x    |
--
-- ============================================================================
-- NOTAS IMPORTANTES
-- ============================================================================
--
-- 1. Ejecutar en horario de bajo tráfico (madrugada)
-- 2. Hacer backup completo antes de ejecutar
-- 3. Los comandos ALTER TABLE pueden tardar varios minutos en tablas grandes
-- 4. OPTIMIZE TABLE puede bloquear las tablas temporalmente
-- 5. Si algún índice ya existe, el comando fallará - es seguro continuar
--
-- Para verificar los índices existentes:
--   SHOW INDEX FROM nombre_tabla;
--
-- Para eliminar un índice si es necesario:
--   DROP INDEX nombre_indice ON nombre_tabla;
--
-- ============================================================================
