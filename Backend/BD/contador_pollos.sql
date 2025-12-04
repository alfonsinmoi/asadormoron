-- ============================================================================
-- TABLA CONTADOR DIARIO DE POLLOS ASADOS
-- ============================================================================
-- Almacena el contador diario de pollos asados vendidos por establecimiento

CREATE TABLE IF NOT EXISTS `qo_contador_pollos` (
    `id` INT(11) NOT NULL AUTO_INCREMENT,
    `idEstablecimiento` INT(11) NOT NULL,
    `fecha` DATE NOT NULL,
    `cantidad` INT(11) NOT NULL DEFAULT 0,
    `fechaCreacion` DATETIME DEFAULT CURRENT_TIMESTAMP,
    `fechaModificacion` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`id`),
    UNIQUE KEY `idx_establecimiento_fecha` (`idEstablecimiento`, `fecha`),
    KEY `idx_fecha` (`fecha`),
    KEY `idx_idEstablecimiento` (`idEstablecimiento`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- NOTAS:
-- - Un registro por día y establecimiento
-- - El índice único evita duplicados para el mismo día/establecimiento
-- - cantidad: número de pollos asados vendidos ese día
-- ============================================================================
