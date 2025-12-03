-- phpMyAdmin SQL Dump
-- version 4.9.11
-- https://www.phpmyadmin.net/
--
-- Servidor: db5016478383.hosting-data.io
-- Tiempo de generación: 03-12-2025 a las 09:42:46
-- Versión del servidor: 8.0.36
-- Versión de PHP: 7.4.33

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `dbs13376654`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_administradores_pueblos`
--

CREATE TABLE `qo_administradores_pueblos` (
  `id` int NOT NULL,
  `idUser` int NOT NULL,
  `idPueblo` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_alergenos`
--

CREATE TABLE `qo_alergenos` (
  `id` int NOT NULL,
  `nombre` varchar(255) NOT NULL,
  `estado` int NOT NULL DEFAULT '0',
  `imagen` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 ROW_FORMAT=DYNAMIC;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_amigos`
--

CREATE TABLE `qo_amigos` (
  `id` int NOT NULL,
  `idCliente` int NOT NULL,
  `idAmigo` int NOT NULL,
  `idPueblo` int NOT NULL,
  `canjeado` int NOT NULL DEFAULT '0',
  `saldoCliente` double(25,2) NOT NULL DEFAULT '0.00',
  `saldoAmigo` double(25,2) NOT NULL DEFAULT '0.00',
  `idPromo` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_camareros`
--

CREATE TABLE `qo_camareros` (
  `id` int NOT NULL,
  `idUsuario` int NOT NULL,
  `nombre` varchar(255) NOT NULL,
  `foto` varchar(255) NOT NULL,
  `activo` int NOT NULL DEFAULT '0',
  `idEstablecimiento` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 ROW_FORMAT=DYNAMIC;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_camareros_zonas`
--

CREATE TABLE `qo_camareros_zonas` (
  `id` int NOT NULL,
  `idZona` int NOT NULL,
  `idCamarero` int NOT NULL,
  `inicio` int NOT NULL DEFAULT '0',
  `fin` int NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_codigos_postales`
--

CREATE TABLE `qo_codigos_postales` (
  `id` int NOT NULL,
  `idPueblo` int NOT NULL,
  `codPostal` varchar(10) NOT NULL,
  `activo` int NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_combo_entrantes`
--

CREATE TABLE `qo_combo_entrantes` (
  `id` int NOT NULL,
  `nombre` varchar(255) NOT NULL,
  `tipo` int NOT NULL DEFAULT '1',
  `nombreTipo` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_configuracion`
--

CREATE TABLE `qo_configuracion` (
  `id` int NOT NULL,
  `servicioActivo` int NOT NULL DEFAULT '1',
  `activoLunes` int NOT NULL DEFAULT '0',
  `activoMartes` int NOT NULL DEFAULT '0',
  `activoMiercoles` int NOT NULL DEFAULT '0',
  `activoJueves` int NOT NULL DEFAULT '1',
  `activoViernes` int NOT NULL DEFAULT '1',
  `activoSabado` int NOT NULL DEFAULT '1',
  `activoDomingo` int NOT NULL DEFAULT '1',
  `inicioLunes` time NOT NULL,
  `inicioMartes` time NOT NULL,
  `inicioMiercoles` time NOT NULL,
  `inicioJueves` time NOT NULL,
  `inicioViernes` time NOT NULL,
  `inicioSabado` time NOT NULL,
  `inicioDomingo` time NOT NULL,
  `finLunes` time NOT NULL,
  `finMartes` time NOT NULL,
  `finMiercoles` time NOT NULL,
  `finJueves` time NOT NULL,
  `finViernes` time NOT NULL,
  `finSabado` time NOT NULL,
  `finDomingo` time NOT NULL,
  `gastosEnvio` double(5,2) NOT NULL DEFAULT '1.90',
  `beneficios` int NOT NULL,
  `inicioLunesTarde` time NOT NULL,
  `inicioMartesTarde` time NOT NULL,
  `inicioMiercolesTarde` time NOT NULL,
  `inicioJuevesTarde` time NOT NULL,
  `inicioViernesTarde` time NOT NULL,
  `inicioSabadoTarde` time NOT NULL,
  `inicioDomingoTarde` time NOT NULL,
  `finLunesTarde` time NOT NULL,
  `finMartesTarde` time NOT NULL,
  `finMiercolesTarde` time NOT NULL,
  `finJuevesTarde` time NOT NULL,
  `finViernesTarde` time NOT NULL,
  `finSabadoTarde` time NOT NULL,
  `finDomingoTarde` time NOT NULL,
  `versionMinima` int NOT NULL,
  `latitud` double NOT NULL,
  `longitud` double NOT NULL,
  `inicioComercio` time NOT NULL,
  `finComercio` time NOT NULL,
  `distanciaMapas` double NOT NULL,
  `activoLunesTarde` int NOT NULL DEFAULT '0',
  `activoMartesTarde` int NOT NULL DEFAULT '0',
  `activoMiercolesTarde` int NOT NULL DEFAULT '0',
  `activoJuevesTarde` int NOT NULL DEFAULT '0',
  `activoViernesTarde` int NOT NULL DEFAULT '0',
  `activoSabadoTarde` int NOT NULL DEFAULT '0',
  `activoDomingoTarde` int NOT NULL DEFAULT '0',
  `inicioComercioTarde` time NOT NULL,
  `FinComercioTarde` time NOT NULL,
  `telefono` varchar(20) NOT NULL,
  `whatsapp` varchar(20) NOT NULL,
  `email` varchar(255) NOT NULL,
  `pedidoMinimo` double(5,2) NOT NULL DEFAULT '0.00',
  `pedidoMinimoComercio` double(5,2) NOT NULL DEFAULT '0.00',
  `idPueblo` int NOT NULL DEFAULT '1',
  `efectivo` int NOT NULL DEFAULT '1',
  `tarjeta` int NOT NULL DEFAULT '1',
  `bizum` int NOT NULL DEFAULT '1',
  `datafono` int NOT NULL DEFAULT '0',
  `nombreTicket` varchar(255) NOT NULL,
  `CIFTicket` varchar(255) NOT NULL,
  `direccionTicket` varchar(255) NOT NULL,
  `telefonoTicket` varchar(255) NOT NULL,
  `cpTicket` varchar(10) NOT NULL,
  `poblacionTicket` varchar(255) NOT NULL,
  `provinciaTicket` varchar(255) NOT NULL,
  `extraLunes` int NOT NULL DEFAULT '0',
  `extraMartes` int NOT NULL DEFAULT '0',
  `extraMiercoles` int NOT NULL DEFAULT '0',
  `extraJueves` int NOT NULL DEFAULT '0',
  `extraViernes` int NOT NULL DEFAULT '0',
  `extraSabado` int NOT NULL DEFAULT '0',
  `extraDomingo` int NOT NULL DEFAULT '0',
  `versionMinimaAndroid` int NOT NULL DEFAULT '0',
  `versionMinimaIOS` int NOT NULL DEFAULT '0',
  `nombreImpresora` varchar(255) NOT NULL,
  `iban` varchar(255) NOT NULL,
  `idGrupo` int NOT NULL DEFAULT '1',
  `variableTarjeta` double(5,2) NOT NULL DEFAULT '0.00',
  `fijoTarjeta` double(5,2) NOT NULL DEFAULT '0.00',
  `comision` double(25,2) NOT NULL DEFAULT '0.00',
  `variableDatafono` double(25,2) NOT NULL DEFAULT '0.00',
  `ticketSize` int NOT NULL DEFAULT '30',
  `tipoImpresora` int NOT NULL DEFAULT '0',
  `tiempoEntreMenus` int NOT NULL DEFAULT '60',
  `categoriaPizzas` int NOT NULL DEFAULT '0',
  `visibleRS` int NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_configuracion_est`
--

CREATE TABLE `qo_configuracion_est` (
  `id` int NOT NULL,
  `idEstablecimiento` int NOT NULL,
  `activoLunes` int NOT NULL DEFAULT '0',
  `activoMartes` int NOT NULL DEFAULT '0',
  `activoMiercoles` int NOT NULL DEFAULT '0',
  `activoJueves` int NOT NULL DEFAULT '1',
  `activoViernes` int NOT NULL DEFAULT '1',
  `activoSabado` int NOT NULL DEFAULT '1',
  `activoDomingo` int NOT NULL DEFAULT '1',
  `inicioLunes` time NOT NULL,
  `inicioMartes` time NOT NULL,
  `inicioMiercoles` time NOT NULL,
  `inicioJueves` time NOT NULL,
  `inicioViernes` time NOT NULL,
  `inicioSabado` time NOT NULL,
  `inicioDomingo` time NOT NULL,
  `finLunes` time NOT NULL,
  `finMartes` time NOT NULL,
  `finMiercoles` time NOT NULL,
  `finJueves` time NOT NULL,
  `finViernes` time NOT NULL,
  `finSabado` time NOT NULL,
  `finDomingo` time NOT NULL,
  `servicioActivo` int NOT NULL DEFAULT '1',
  `tiempoEntrega` int NOT NULL DEFAULT '45',
  `inicioLunesTarde` time NOT NULL,
  `inicioMartesTarde` time NOT NULL,
  `inicioMiercolesTarde` time NOT NULL,
  `inicioJuevesTarde` time NOT NULL,
  `inicioViernesTarde` time NOT NULL,
  `inicioSabadoTarde` time NOT NULL,
  `inicioDomingoTarde` time NOT NULL,
  `finLunesTarde` time NOT NULL,
  `finMartesTarde` time NOT NULL,
  `finMiercolesTarde` time NOT NULL,
  `finJuevesTarde` time NOT NULL,
  `finViernesTarde` time NOT NULL,
  `finSabadoTarde` time NOT NULL,
  `finDomingoTarde` time NOT NULL,
  `pedidoMinimo` double(4,2) NOT NULL DEFAULT '15.00',
  `numeroPedidosSoportado` int NOT NULL DEFAULT '4',
  `activoLunesTarde` int NOT NULL DEFAULT '0',
  `activoMartesTarde` int NOT NULL DEFAULT '0',
  `activoMiercolesTarde` int NOT NULL DEFAULT '0',
  `activoJuevesTarde` int NOT NULL DEFAULT '0',
  `activoViernesTarde` int NOT NULL DEFAULT '0',
  `activoSabadoTarde` int NOT NULL DEFAULT '0',
  `activoDomingoTarde` int NOT NULL DEFAULT '0',
  `InicioLunesLocal` time NOT NULL,
  `InicioMartesLocal` time NOT NULL,
  `InicioMiercolesLocal` time NOT NULL,
  `InicioJuevesLocal` time NOT NULL,
  `InicioViernesLocal` time NOT NULL,
  `InicioSabadoLocal` time NOT NULL,
  `InicioDomingoLocal` time NOT NULL,
  `FinLunesLocal` time NOT NULL,
  `FinMartesLocal` time NOT NULL,
  `FinMiercolesLocal` time NOT NULL,
  `FinJuevesLocal` time NOT NULL,
  `FinViernesLocal` time NOT NULL,
  `FinSabadoLocal` time NOT NULL,
  `FinDomingoLocal` time NOT NULL,
  `InicioLunesTardeLocal` time NOT NULL,
  `InicioMartesTardeLocal` time NOT NULL,
  `InicioMiercolesTardeLocal` time NOT NULL,
  `InicioJuevesTardeLocal` time NOT NULL,
  `InicioViernesTardeLocal` time NOT NULL,
  `InicioSabadoTardeLocal` time NOT NULL,
  `InicioDomingoTardeLocal` time NOT NULL,
  `FinLunesTardeLocal` time NOT NULL,
  `FinMartesTardeLocal` time NOT NULL,
  `FinMiercolesTardeLocal` time NOT NULL,
  `FinJuevesTardeLocal` time NOT NULL,
  `FinViernesTardeLocal` time NOT NULL,
  `FinSabadoTardeLocal` time NOT NULL,
  `FinDomingoTardeLocal` time NOT NULL,
  `activoLunesLocal` int NOT NULL DEFAULT '0',
  `activoMartesLocal` int NOT NULL DEFAULT '0',
  `activoMiercolesLocal` int NOT NULL DEFAULT '0',
  `activoJuevesLocal` int NOT NULL DEFAULT '0',
  `activoViernesLocal` int NOT NULL DEFAULT '0',
  `activoSabadoLocal` int NOT NULL DEFAULT '0',
  `activoDomingoLocal` int NOT NULL DEFAULT '0',
  `activoLunesTardeLocal` int NOT NULL DEFAULT '0',
  `activoMartesTardeLocal` int NOT NULL DEFAULT '0',
  `activoMiercolesTardeLocal` int NOT NULL DEFAULT '0',
  `activoJuevesTardeLocal` int NOT NULL DEFAULT '0',
  `activoViernesTardeLocal` int NOT NULL DEFAULT '0',
  `activoSabadoTardeLocal` int NOT NULL DEFAULT '0',
  `activoDomingoTardeLocal` int NOT NULL DEFAULT '0',
  `comision` double(24,2) NOT NULL DEFAULT '18.00',
  `comisionLocal` double(24,2) NOT NULL DEFAULT '0.00' COMMENT 'Comisión por pedidos en local',
  `comisionRecogida` double(24,2) NOT NULL DEFAULT '0.00' COMMENT 'Comisión por pedidos a recoger por el cliente',
  `comisionReparto` double(20,2) NOT NULL DEFAULT '0.00' COMMENT 'Comisión por pedidos repartidos por el restaurante',
  `modoEscaparate` int NOT NULL DEFAULT '0',
  `detalles` int NOT NULL DEFAULT '0',
  `textoDetalle` varchar(255) NOT NULL DEFAULT '',
  `textoDetalle_eng` varchar(255) DEFAULT NULL,
  `textoDetalle_ger` varchar(255) DEFAULT NULL,
  `textoDetalle_fr` varchar(255) DEFAULT NULL,
  `textoIngredientes` varchar(255) NOT NULL DEFAULT 'EXTRAS',
  `textoIngredientes_eng` varchar(255) DEFAULT NULL,
  `textoIngredientes_ger` varchar(255) DEFAULT NULL,
  `textoIngredientes_fr` varchar(255) DEFAULT NULL,
  `nombreImpresora` varchar(255) NOT NULL,
  `nombreImpresora2` varchar(100) NOT NULL DEFAULT '',
  `nombreImpresora3` varchar(100) NOT NULL DEFAULT '',
  `nombreImpresora4` varchar(100) NOT NULL DEFAULT '',
  `nombreImpresora5` varchar(100) NOT NULL DEFAULT '',
  `nombreImpresora6` varchar(100) NOT NULL DEFAULT '',
  `nombreImpresora7` varchar(100) NOT NULL DEFAULT '',
  `nombreImpresora8` varchar(100) NOT NULL DEFAULT '',
  `nombreImpresora9` varchar(100) NOT NULL DEFAULT '',
  `nombreImpresora10` varchar(100) NOT NULL DEFAULT '',
  `alturaLineaImpresora` int NOT NULL DEFAULT '25',
  `cuotaFija` double(24,2) NOT NULL DEFAULT '0.00',
  `otrasCuotas` double(24,2) NOT NULL DEFAULT '0.00',
  `repartoPropio` int NOT NULL DEFAULT '0',
  `repartoPolloAndaluz` int NOT NULL DEFAULT '1',
  `preferenciaReparto` varchar(1) NOT NULL DEFAULT 'Q',
  `gastosEnvioPropio` decimal(5,2) NOT NULL DEFAULT '1.00',
  `comisionAutoPedido` decimal(5,2) NOT NULL DEFAULT '0.00',
  `encargosPorHora` int NOT NULL DEFAULT '0',
  `encargosDiasDesde` int NOT NULL DEFAULT '0',
  `encargosDiasHasta` int NOT NULL DEFAULT '0',
  `aceptaEncargos` int NOT NULL DEFAULT '0',
  `visibilidadHoras` int NOT NULL DEFAULT '0',
  `textoPuntos` text NOT NULL,
  `puntosPorEuro` int NOT NULL DEFAULT '0',
  `puntosPorPedido` int NOT NULL DEFAULT '0',
  `sistemaPuntos` int NOT NULL DEFAULT '0',
  `numImpresion` int NOT NULL DEFAULT '3',
  `tipoAutoPedido` int NOT NULL DEFAULT '1',
  `estadoAutoPedido` int NOT NULL DEFAULT '3',
  `idZonaAutoPedido` int NOT NULL DEFAULT '4',
  `tiempoRepartoComercio` int NOT NULL DEFAULT '24',
  `repartoComercioM` int NOT NULL DEFAULT '0',
  `repartoComercioT` int NOT NULL DEFAULT '0',
  `ticketSize` int NOT NULL DEFAULT '30',
  `cualquierHoraOtroPueblo` int NOT NULL DEFAULT '1',
  `efectivoPropio` int NOT NULL DEFAULT '0',
  `datafonoPropio` int NOT NULL DEFAULT '0',
  `tarjetaPropia` int NOT NULL DEFAULT '0',
  `horaSoloTarjetaDesde` time NOT NULL DEFAULT '00:00:00',
  `horaSoloTarjetaHasta` time NOT NULL DEFAULT '00:00:00',
  `activaSoloTarjeta` int NOT NULL DEFAULT '0',
  `suplementoMediaPizza` double(25,2) NOT NULL DEFAULT '0.00',
  `categoriaPizzas` int NOT NULL DEFAULT '0',
  `tieneMediaPizza` double(25,2) NOT NULL DEFAULT '0.00',
  `idCategoriaPizza` double(25,2) NOT NULL DEFAULT '0.00',
  `textoCerrado` text NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_configuracion_global`
--

CREATE TABLE `qo_configuracion_global` (
  `id` int NOT NULL,
  `redessociales` int NOT NULL DEFAULT '0',
  `pedidoenmesa` int NOT NULL DEFAULT '0',
  `longitudCodigo` int NOT NULL DEFAULT '8',
  `telefonoTwilio` varchar(100) NOT NULL,
  `mensajeRegistro` text NOT NULL,
  `terminalPaycomet` int NOT NULL,
  `apiPaycomet` varchar(255) NOT NULL,
  `numeroFactura` int NOT NULL DEFAULT '0',
  `distanciaLocal` int NOT NULL DEFAULT '50'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_cuentas`
--

CREATE TABLE `qo_cuentas` (
  `id` int NOT NULL,
  `fecha` datetime NOT NULL,
  `idUsuario` int NOT NULL,
  `idEstablecimiento` int NOT NULL,
  `fechaPago` datetime DEFAULT NULL,
  `cuentaPedida` int NOT NULL DEFAULT '0',
  `cerrada` int NOT NULL DEFAULT '0',
  `idZona` int NOT NULL,
  `mesa` varchar(10) NOT NULL,
  `transaccion` varchar(255) NOT NULL,
  `idCuenta` int NOT NULL,
  `codigo` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 ROW_FORMAT=DYNAMIC;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_cuentas_detalle`
--

CREATE TABLE `qo_cuentas_detalle` (
  `id` int NOT NULL,
  `idCuenta` int NOT NULL,
  `codigoPedido` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 ROW_FORMAT=DYNAMIC;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_cupones`
--

CREATE TABLE `qo_cupones` (
  `id` int NOT NULL,
  `codigoCupon` varchar(255) NOT NULL,
  `limitado` int NOT NULL DEFAULT '0',
  `cantidad` int NOT NULL DEFAULT '0',
  `pueblo` int NOT NULL DEFAULT '0',
  `idPueblo` varchar(255) NOT NULL,
  `establecmiento` int NOT NULL DEFAULT '0',
  `idEstablecimiento` varchar(255) NOT NULL,
  `producto` int NOT NULL DEFAULT '0',
  `idProducto` varchar(255) NOT NULL,
  `categoria` int NOT NULL DEFAULT '0',
  `idCategoria` varchar(255) NOT NULL,
  `gastos` int NOT NULL DEFAULT '0',
  `tipoOferta` int NOT NULL COMMENT '0-fijo, 1-%',
  `fechaDesde` datetime NOT NULL,
  `fechaHasta` datetime NOT NULL,
  `Lunes` int NOT NULL DEFAULT '0',
  `Martes` int NOT NULL DEFAULT '0',
  `Miércoles` int NOT NULL DEFAULT '0',
  `Jueves` int NOT NULL DEFAULT '0',
  `Viernes` int NOT NULL DEFAULT '0',
  `Sabado` int NOT NULL DEFAULT '0',
  `Domingo` int NOT NULL DEFAULT '0',
  `estado` int NOT NULL DEFAULT '0',
  `creador` int NOT NULL DEFAULT '3' COMMENT '0-Qoorder,1-Administrador,2-Establecimiento',
  `valor` double(10,2) NOT NULL DEFAULT '0.00',
  `idGrupo` int NOT NULL DEFAULT '1'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_cupones_sql`
--

CREATE TABLE `qo_cupones_sql` (
  `id` int NOT NULL,
  `codigo` varchar(20) NOT NULL,
  `sentencia` text NOT NULL,
  `usuario` int NOT NULL DEFAULT '0',
  `pueblo` int NOT NULL DEFAULT '1',
  `estado` int NOT NULL DEFAULT '1',
  `valorSentencia` int NOT NULL DEFAULT '0',
  `tipoDescuento` int NOT NULL COMMENT '1:Fijo,0:Porcentaje',
  `descuento` double(5,2) NOT NULL,
  `gastosEnvio` int NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_cupones_usuario`
--

CREATE TABLE `qo_cupones_usuario` (
  `id` int NOT NULL,
  `idCupon` int NOT NULL,
  `idUsuario` int NOT NULL,
  `utilizado` int NOT NULL DEFAULT '0',
  `fechaUtilizacion` datetime NOT NULL,
  `fechaAnulacion` datetime NOT NULL,
  `anulado` int NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_equipos`
--

CREATE TABLE `qo_equipos` (
  `id` int NOT NULL,
  `nombre` varchar(255) NOT NULL,
  `imagen` varchar(255) NOT NULL,
  `estado` int NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 ROW_FORMAT=DYNAMIC;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_establecimientos`
--

CREATE TABLE `qo_establecimientos` (
  `id` int NOT NULL,
  `nombre` varchar(255) NOT NULL,
  `idCategoria` int NOT NULL,
  `direccion` varchar(255) NOT NULL,
  `poblacion` varchar(255) NOT NULL,
  `codPostal` varchar(10) NOT NULL,
  `latitud` float NOT NULL,
  `longitud` float NOT NULL,
  `tipo` int NOT NULL DEFAULT '0' COMMENT '0: Evento, 1: Restaurante',
  `imagen` varchar(255) NOT NULL,
  `estado` int NOT NULL DEFAULT '0',
  `provincia` varchar(255) NOT NULL,
  `telefono` varchar(20) NOT NULL,
  `email` varchar(255) NOT NULL,
  `local` int NOT NULL DEFAULT '0',
  `envio` int NOT NULL DEFAULT '0',
  `recogida` int NOT NULL DEFAULT '0',
  `logo` varchar(255) NOT NULL,
  `valoraciones` int NOT NULL DEFAULT '0',
  `ipImpresora` varchar(255) DEFAULT NULL,
  `nombreImpresoraBarra` varchar(255) DEFAULT NULL,
  `nombreImpresoraCocina` varchar(255) DEFAULT NULL,
  `usuarioBarra` varchar(255) DEFAULT NULL,
  `usuarioCocina` varchar(255) DEFAULT NULL,
  `llamadaCamarero` int NOT NULL DEFAULT '0',
  `puedeReservar` int NOT NULL DEFAULT '0',
  `orden` int NOT NULL,
  `idZona` int NOT NULL DEFAULT '0',
  `esComercio` tinyint NOT NULL DEFAULT '0',
  `idPueblo` int NOT NULL DEFAULT '1',
  `visitas` int NOT NULL DEFAULT '0',
  `valoracion` double(3,2) NOT NULL DEFAULT '0.00',
  `puntos` double(5,2) DEFAULT '0.00',
  `telefono2` varchar(30) NOT NULL,
  `whatsapp` varchar(30) NOT NULL,
  `emailContacto` varchar(255) NOT NULL,
  `llevaAMesa` int NOT NULL DEFAULT '0',
  `recogeEnBarra` int NOT NULL DEFAULT '0',
  `idGrupo` int NOT NULL DEFAULT '1',
  `textoMulti` varchar(255) NOT NULL DEFAULT '',
  `colorMulti` varchar(40) NOT NULL DEFAULT '',
  `visibleFuera` int NOT NULL DEFAULT '0',
  `web` varchar(255) NOT NULL DEFAULT '',
  `tipoImpresora` int NOT NULL DEFAULT '0',
  `tieneMenuDiario` int NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 ROW_FORMAT=DYNAMIC;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_establecimientos_fiscal`
--

CREATE TABLE `qo_establecimientos_fiscal` (
  `id` int NOT NULL,
  `razonSocial` varchar(255) NOT NULL,
  `direccion` varchar(255) NOT NULL,
  `cp` varchar(10) NOT NULL,
  `poblacion` varchar(255) NOT NULL,
  `provincia` varchar(255) NOT NULL,
  `telefono` varchar(20) NOT NULL,
  `cif` varchar(20) NOT NULL,
  `idEstablecimiento` int NOT NULL,
  `iban` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_establecimientos_valoraciones`
--

CREATE TABLE `qo_establecimientos_valoraciones` (
  `id` int NOT NULL,
  `idUsuario` int NOT NULL,
  `idEstablecimiento` int NOT NULL,
  `valoracion` double(3,2) NOT NULL,
  `fecha` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `observaciones` text NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_establecimientos_visitas`
--

CREATE TABLE `qo_establecimientos_visitas` (
  `id` int NOT NULL,
  `idUsuario` int NOT NULL,
  `idEstablecimiento` int NOT NULL,
  `fecha` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `modo` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_establecimientos_zonas`
--

CREATE TABLE `qo_establecimientos_zonas` (
  `id` int NOT NULL,
  `idEstablecimiento` int NOT NULL,
  `nombre` varchar(255) NOT NULL,
  `activo` int NOT NULL DEFAULT '1',
  `codigo` varchar(3) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 ROW_FORMAT=DYNAMIC;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_estados`
--

CREATE TABLE `qo_estados` (
  `id` int NOT NULL,
  `nombre` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 ROW_FORMAT=DYNAMIC;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_eventos`
--

CREATE TABLE `qo_eventos` (
  `id` int NOT NULL,
  `idCategoria` int NOT NULL,
  `nombre` varchar(255) NOT NULL,
  `idEstadio` int NOT NULL,
  `imagen` varchar(255) NOT NULL,
  `fechaInicio` datetime NOT NULL,
  `fechaFin` datetime NOT NULL,
  `descripcion` text NOT NULL,
  `estado` int NOT NULL DEFAULT '0',
  `idUsuario` int NOT NULL,
  `idEquipoLocal` int NOT NULL,
  `idEquipoVisitante` int NOT NULL,
  `jornada` int NOT NULL,
  `temporada` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 ROW_FORMAT=DYNAMIC;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_facturas`
--

CREATE TABLE `qo_facturas` (
  `id` int NOT NULL,
  `ruta` text NOT NULL,
  `nombre` varchar(255) NOT NULL,
  `numero` varchar(100) NOT NULL,
  `desde` date NOT NULL,
  `hasta` date NOT NULL,
  `idEstablecimiento` int NOT NULL,
  `nombreEstablecimiento` varchar(255) NOT NULL,
  `total` double(5,2) NOT NULL DEFAULT '0.00'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_facturas_administradores`
--

CREATE TABLE `qo_facturas_administradores` (
  `id` int NOT NULL,
  `ruta` text NOT NULL,
  `nombre` varchar(255) NOT NULL,
  `numero` varchar(100) NOT NULL,
  `desde` date NOT NULL,
  `hasta` date NOT NULL,
  `idPueblo` int NOT NULL,
  `nombreAdministrador` varchar(255) NOT NULL,
  `total` double(5,2) NOT NULL DEFAULT '0.00'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_facturas_detalle`
--

CREATE TABLE `qo_facturas_detalle` (
  `id` int NOT NULL,
  `cantidad` int NOT NULL,
  `concepto` varchar(255) NOT NULL,
  `precio` double(15,2) NOT NULL DEFAULT '0.00',
  `total` double(15,2) NOT NULL DEFAULT '0.00',
  `baseImponible` double(15,2) NOT NULL DEFAULT '0.00',
  `iva` double(15,2) NOT NULL DEFAULT '0.00',
  `idFactura` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_forma_pago`
--

CREATE TABLE `qo_forma_pago` (
  `id` int NOT NULL,
  `nombre` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 ROW_FORMAT=DYNAMIC;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_gastos`
--

CREATE TABLE `qo_gastos` (
  `id` int NOT NULL,
  `idRepartidor` int NOT NULL,
  `precio` double(25,2) NOT NULL DEFAULT '0.00',
  `concepto` text NOT NULL,
  `fecha` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_ingredientes`
--

CREATE TABLE `qo_ingredientes` (
  `id` int NOT NULL,
  `nombre` varchar(255) NOT NULL,
  `imagen` varchar(255) NOT NULL,
  `estado` int NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 ROW_FORMAT=DYNAMIC;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_ingredientes_establecimiento`
--

CREATE TABLE `qo_ingredientes_establecimiento` (
  `id` int NOT NULL,
  `idEstablecimiento` int NOT NULL,
  `nombre` varchar(255) NOT NULL,
  `nombre_eng` varchar(255) DEFAULT NULL,
  `nombre_ger` varchar(255) DEFAULT NULL,
  `nombre_fr` varchar(255) DEFAULT NULL,
  `precio` double(5,2) NOT NULL DEFAULT '0.00',
  `estado` tinyint(1) NOT NULL DEFAULT '1',
  `puntos` int NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_ingredientes_producto`
--

CREATE TABLE `qo_ingredientes_producto` (
  `id` int NOT NULL,
  `idProducto` int NOT NULL,
  `idIngrediente` int NOT NULL,
  `precio` double(5,2) NOT NULL DEFAULT '0.00',
  `puntos` int NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_logs`
--

CREATE TABLE `qo_logs` (
  `id` int NOT NULL,
  `idUsuario` int NOT NULL,
  `pantalla` varchar(255) NOT NULL,
  `observaciones` text NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_logs_app`
--

CREATE TABLE `qo_logs_app` (
  `id` int NOT NULL,
  `pantalla` varchar(255) NOT NULL,
  `idCategoria` int NOT NULL,
  `idEstablecimiento` int NOT NULL,
  `filtro` varchar(255) NOT NULL,
  `fecha` datetime NOT NULL,
  `idUsuario` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_mensajes`
--

CREATE TABLE `qo_mensajes` (
  `id` int NOT NULL,
  `clave` varchar(255) NOT NULL,
  `valor` text NOT NULL,
  `valor_eng` varchar(255) DEFAULT NULL,
  `valor_ger` varchar(255) DEFAULT NULL,
  `valor_fr` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_mensajes_camarero`
--

CREATE TABLE `qo_mensajes_camarero` (
  `id` int NOT NULL,
  `idCamarero` int NOT NULL,
  `codigoPedido` varchar(50) NOT NULL,
  `mensaje` text NOT NULL,
  `visto` int NOT NULL DEFAULT '0',
  `idUsuario` int NOT NULL,
  `usuario` varchar(255) NOT NULL,
  `hora` datetime NOT NULL,
  `mesa` int NOT NULL,
  `idZona` int NOT NULL,
  `zona` varchar(255) NOT NULL,
  `idEstablecimiento` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_mensajes_repartidor`
--

CREATE TABLE `qo_mensajes_repartidor` (
  `id` int NOT NULL,
  `idRepartidor` int NOT NULL,
  `mensaje` text NOT NULL,
  `ok` int NOT NULL DEFAULT '0',
  `contestado` int NOT NULL DEFAULT '0',
  `fechaEnvio` datetime NOT NULL,
  `fechaContestacion` datetime DEFAULT NULL,
  `anulado` int NOT NULL DEFAULT '0',
  `idSender` int NOT NULL,
  `sender` varchar(255) NOT NULL,
  `admin` int NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_mensajes_repartidor_predef`
--

CREATE TABLE `qo_mensajes_repartidor_predef` (
  `id` int NOT NULL,
  `texto` varchar(255) NOT NULL,
  `textoCorto` varchar(255) NOT NULL,
  `estado` int NOT NULL DEFAULT '0',
  `administrador` int NOT NULL DEFAULT '0',
  `establecimiento` int NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_menu`
--

CREATE TABLE `qo_menu` (
  `id` int NOT NULL,
  `nombre` varchar(255) NOT NULL,
  `nombre_ingles` varchar(255) NOT NULL,
  `nombre_frances` varchar(255) NOT NULL,
  `nombre_aleman` varchar(255) NOT NULL,
  `rol` int NOT NULL,
  `viewmodel` varchar(255) NOT NULL,
  `imagen` varchar(255) NOT NULL,
  `orden` int NOT NULL,
  `idParent` int NOT NULL,
  `visible` int NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_menu_diario`
--

CREATE TABLE `qo_menu_diario` (
  `id` int NOT NULL,
  `idEstablecimiento` int NOT NULL,
  `precio` double(25,2) NOT NULL DEFAULT '0.00',
  `activo` int NOT NULL DEFAULT '0',
  `descripcion` text NOT NULL,
  `nombre` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_menu_diario_conf`
--

CREATE TABLE `qo_menu_diario_conf` (
  `id` int NOT NULL,
  `idEstablecimiento` int NOT NULL,
  `horaInicioLunes` time NOT NULL,
  `horaFinLunes` time NOT NULL,
  `horaInicioMartes` time NOT NULL,
  `horaFinMartes` time NOT NULL,
  `horaInicioMiercoles` time NOT NULL,
  `horaFinMiercoles` time NOT NULL,
  `horaInicioJueves` time NOT NULL,
  `horaFinJueves` time NOT NULL,
  `horaInicioViernes` time NOT NULL,
  `horaFinViernes` time NOT NULL,
  `tiempoMaximo` int NOT NULL,
  `activoLunes` int NOT NULL DEFAULT '0',
  `activoMartes` int NOT NULL DEFAULT '0',
  `activoMiercoles` int NOT NULL DEFAULT '0',
  `activoJueves` int NOT NULL DEFAULT '0',
  `activoViernes` int NOT NULL DEFAULT '0',
  `maxPedidos` int NOT NULL DEFAULT '0',
  `cartaYMenu` int NOT NULL DEFAULT '0',
  `postreObligatorio` int NOT NULL DEFAULT '0',
  `extraPostre` double(5,2) NOT NULL DEFAULT '0.00',
  `bebidaObligatoria` int NOT NULL DEFAULT '0',
  `extraBebida` double(5,2) NOT NULL DEFAULT '0.00',
  `platoUnico` int NOT NULL DEFAULT '0',
  `precioPlatoUnico` double(25,2) NOT NULL DEFAULT '0.00'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_menu_diario_prod`
--

CREATE TABLE `qo_menu_diario_prod` (
  `id` int NOT NULL,
  `idMenu` int NOT NULL,
  `nombre` varchar(255) NOT NULL,
  `tipo` int NOT NULL DEFAULT '1',
  `imagen` varchar(255) NOT NULL,
  `activo` int NOT NULL DEFAULT '1'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_multi_establecimiento`
--

CREATE TABLE `qo_multi_establecimiento` (
  `id` int NOT NULL,
  `idUsuario` int NOT NULL,
  `idEstablecimiento` int NOT NULL,
  `activo` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_numero_usuarios`
--

CREATE TABLE `qo_numero_usuarios` (
  `id` int NOT NULL,
  `numero` int NOT NULL,
  `hora` datetime NOT NULL,
  `idGrupo` int NOT NULL DEFAULT '1'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_ofertas`
--

CREATE TABLE `qo_ofertas` (
  `id` int NOT NULL,
  `pueblo` int NOT NULL DEFAULT '0',
  `idPueblo` varchar(255) NOT NULL,
  `establecmiento` int NOT NULL DEFAULT '0',
  `idEstablecimiento` varchar(255) NOT NULL,
  `producto` int NOT NULL DEFAULT '0',
  `idProducto` varchar(255) NOT NULL,
  `categoria` int NOT NULL DEFAULT '0',
  `idCategoria` varchar(255) NOT NULL,
  `gastos` int NOT NULL DEFAULT '0',
  `tipoOferta` int NOT NULL COMMENT '0-fijo, 1-%',
  `fechaDesde` datetime NOT NULL,
  `fechaHasta` datetime NOT NULL,
  `Lunes` int NOT NULL DEFAULT '0',
  `Martes` int NOT NULL DEFAULT '0',
  `Miércoles` int NOT NULL DEFAULT '0',
  `Jueves` int NOT NULL DEFAULT '0',
  `Viernes` int NOT NULL DEFAULT '0',
  `Sabado` int NOT NULL DEFAULT '0',
  `Domingo` int NOT NULL DEFAULT '0',
  `Imagen` varchar(255) NOT NULL,
  `estado` int NOT NULL DEFAULT '0',
  `creador` int NOT NULL DEFAULT '3' COMMENT '0-Qoorder,1-Administrador,2-Establecimiento',
  `dosPorUno` int NOT NULL DEFAULT '0',
  `nombre` varchar(255) NOT NULL,
  `idGrupo` int NOT NULL DEFAULT '1',
  `valor` double(10,2) NOT NULL DEFAULT '0.00'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_online`
--

CREATE TABLE `qo_online` (
  `id` int NOT NULL,
  `idUsuario` int NOT NULL,
  `tokenUsuario` varchar(255) NOT NULL,
  `idPueblo` int NOT NULL,
  `horaInicio` datetime NOT NULL,
  `horaCierre` datetime DEFAULT NULL,
  `horaBackground` datetime DEFAULT NULL,
  `horaResume` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_pedidos`
--

CREATE TABLE `qo_pedidos` (
  `id` int NOT NULL,
  `codigo` varchar(10) NOT NULL,
  `idEstablecimiento` int NOT NULL,
  `horaPedido` datetime NOT NULL,
  `idUsuario` int NOT NULL,
  `estado` int NOT NULL DEFAULT '2' COMMENT '0:Realizado,1:En preceso,2:Entregado,3:anulado',
  `idZona` int NOT NULL DEFAULT '1',
  `nuevoPedido` int NOT NULL DEFAULT '1',
  `cerrado` int NOT NULL DEFAULT '0',
  `fechaCierre` date NOT NULL,
  `visto` int NOT NULL DEFAULT '0',
  `recoger` int NOT NULL DEFAULT '0',
  `direccion` varchar(255) NOT NULL,
  `comentario` text NOT NULL,
  `repartidor` int NOT NULL DEFAULT '0',
  `idRepartidor` int NOT NULL DEFAULT '0',
  `horaEntrega` datetime NOT NULL,
  `completo` int NOT NULL DEFAULT '0',
  `anulado` int NOT NULL DEFAULT '0',
  `pagado` int NOT NULL DEFAULT '0',
  `transaccion` varchar(255) NOT NULL,
  `tipo` int NOT NULL DEFAULT '1' COMMENT '1:Envio,2:Recogida,3:local',
  `tipoVenta` varchar(255) NOT NULL DEFAULT 'Envío',
  `idZonaEstablecimiento` int NOT NULL DEFAULT '0',
  `Mesa` varchar(11) NOT NULL DEFAULT '0',
  `zonaEstablecimiento` varchar(255) NOT NULL,
  `idCuenta` int NOT NULL DEFAULT '0',
  `tipoPago` varchar(100) NOT NULL DEFAULT 'Efectivo',
  `valorado` int NOT NULL DEFAULT '1',
  `nombreUsuario` varchar(255) NOT NULL DEFAULT '',
  `idReserva` int NOT NULL DEFAULT '0',
  `numFactura` varchar(255) NOT NULL DEFAULT '',
  `facturado` int NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 ROW_FORMAT=DYNAMIC;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_pedidos_detalle`
--

CREATE TABLE `qo_pedidos_detalle` (
  `id` int NOT NULL,
  `idPedido` int NOT NULL,
  `idProducto` int NOT NULL,
  `precio` double(10,2) NOT NULL DEFAULT '0.00',
  `estado` int NOT NULL DEFAULT '0',
  `cantidad` int NOT NULL DEFAULT '1',
  `tipo` int NOT NULL DEFAULT '0',
  `concepto` text NOT NULL,
  `comentario` text NOT NULL,
  `tipoVenta` varchar(255) NOT NULL DEFAULT 'Envío',
  `pagadoConPuntos` int NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 ROW_FORMAT=DYNAMIC;

--
-- Disparadores `qo_pedidos_detalle`
--
DELIMITER $$
CREATE TRIGGER `completo_ins2` AFTER INSERT ON `qo_pedidos_detalle` FOR EACH ROW IF new.tipo = 1 THEN
    UPDATE `qo_pedidos` SET completo=1 WHERE id=new.idPedido;
END IF
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `completo_upd2` BEFORE UPDATE ON `qo_pedidos_detalle` FOR EACH ROW IF new.tipo = 1 THEN
    UPDATE `qo_pedidos` SET completo=1 WHERE id=new.idPedido;
END IF
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_pedidos_estado`
--

CREATE TABLE `qo_pedidos_estado` (
  `id` int NOT NULL,
  `estado` int NOT NULL,
  `idUsuario` int NOT NULL,
  `fecha` datetime NOT NULL,
  `idPedido` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_posicion_repartidor`
--

CREATE TABLE `qo_posicion_repartidor` (
  `id` int NOT NULL,
  `idRepartidor` int NOT NULL,
  `longitud` double NOT NULL,
  `latitud` double NOT NULL,
  `fecha` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_productos_cat`
--

CREATE TABLE `qo_productos_cat` (
  `id` int NOT NULL,
  `nombre` varchar(255) NOT NULL,
  `idEstablecimiento` int NOT NULL,
  `estado` int NOT NULL DEFAULT '0',
  `tipo` int NOT NULL DEFAULT '0' COMMENT '0: Bebida,1:Comida',
  `color` varchar(7) NOT NULL,
  `nombre_eng` varchar(255) NOT NULL,
  `nombre_ger` varchar(255) NOT NULL,
  `nombre_fr` varchar(255) NOT NULL,
  `orden` int NOT NULL DEFAULT '0',
  `numeroImpresora` int NOT NULL DEFAULT '1',
  `imagen` varchar(255) NOT NULL,
  `dosPorUno` int NOT NULL DEFAULT '0',
  `tresPorDos` int NOT NULL DEFAULT '0',
  `espuntos` int NOT NULL DEFAULT '0',
  `navidad` int NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 ROW_FORMAT=DYNAMIC;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_productos_est`
--

CREATE TABLE `qo_productos_est` (
  `id` int NOT NULL,
  `nombre` varchar(255) NOT NULL,
  `idCategoria` int NOT NULL,
  `imagen` varchar(255) NOT NULL DEFAULT 'http://qoorder.com/pa_ws/images/logo.png',
  `estado` int NOT NULL DEFAULT '0',
  `precio` double(24,2) NOT NULL,
  `descripcion` text NOT NULL,
  `nombre_eng` varchar(255) NOT NULL,
  `descripcion_eng` text NOT NULL,
  `nombre_ger` varchar(255) NOT NULL,
  `descripcion_ger` text NOT NULL,
  `nombre_fr` varchar(255) NOT NULL,
  `descripcion_fr` text NOT NULL,
  `numeroIngredientes` int NOT NULL DEFAULT '0',
  `vistaEnvios` int NOT NULL DEFAULT '1',
  `vistaLocal` int NOT NULL DEFAULT '0',
  `precioLocal` double(7,2) NOT NULL DEFAULT '0.00',
  `eliminado` int NOT NULL DEFAULT '0',
  `fuerzaIngredientes` int NOT NULL DEFAULT '0',
  `porEncargo` int NOT NULL DEFAULT '0',
  `puntos` int NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 ROW_FORMAT=DYNAMIC;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_productos_ing`
--

CREATE TABLE `qo_productos_ing` (
  `id` int NOT NULL,
  `nombre` varchar(255) NOT NULL,
  `idProducto` int NOT NULL,
  `incremento` double(5,2) NOT NULL DEFAULT '0.00',
  `estado` int NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 ROW_FORMAT=DYNAMIC;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_productos_ing_aler`
--

CREATE TABLE `qo_productos_ing_aler` (
  `id` int NOT NULL,
  `idProducto` int NOT NULL,
  `idAlergeno` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 ROW_FORMAT=DYNAMIC;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_productos_opc`
--

CREATE TABLE `qo_productos_opc` (
  `idProducto` int NOT NULL,
  `opcion` varchar(255) NOT NULL,
  `opcion_eng` varchar(255) DEFAULT NULL,
  `opcion_ger` varchar(255) DEFAULT NULL,
  `opcion_fr` varchar(255) DEFAULT NULL,
  `tipoIncremento` int NOT NULL DEFAULT '0' COMMENT '0: Tamaño, 1:Otro',
  `valorIncremento` double(10,2) NOT NULL DEFAULT '0.00',
  `id` int NOT NULL,
  `puntos` int NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 ROW_FORMAT=DYNAMIC;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_promocion_amigo`
--

CREATE TABLE `qo_promocion_amigo` (
  `id` int NOT NULL,
  `nombre` varchar(255) NOT NULL,
  `descripcion` text NOT NULL,
  `fechaInicio` date NOT NULL,
  `fechaFin` date NOT NULL,
  `idPueblo` int NOT NULL,
  `saldoUsuario` double(25,2) NOT NULL DEFAULT '0.00',
  `saldoAmigo` double(25,2) NOT NULL DEFAULT '0.00',
  `personasAlcanzadas` int NOT NULL DEFAULT '0',
  `saldoRepartido` double(25,2) NOT NULL DEFAULT '0.00',
  `activo` int NOT NULL DEFAULT '0',
  `pedidoMinimo` double(25,2) NOT NULL DEFAULT '20.00'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_publicidad`
--

CREATE TABLE `qo_publicidad` (
  `id` int NOT NULL,
  `nombre` varchar(255) NOT NULL,
  `imagen` varchar(255) NOT NULL,
  `idGrupo` int NOT NULL DEFAULT '0',
  `idPueblo` int NOT NULL DEFAULT '0',
  `linkIdEstablecimiento` int NOT NULL DEFAULT '0',
  `linkWeb` varchar(255) NOT NULL,
  `fechaDesde` datetime NOT NULL,
  `fechaHasta` datetime NOT NULL,
  `numeroVisualizaciones` int NOT NULL,
  `visualizaciones` int NOT NULL,
  `apariciones` int NOT NULL,
  `links` int NOT NULL DEFAULT '0',
  `preferencia` int NOT NULL DEFAULT '1',
  `estado` int NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_pueblos`
--

CREATE TABLE `qo_pueblos` (
  `id` int NOT NULL,
  `nombre` varchar(255) NOT NULL,
  `activo` int NOT NULL DEFAULT '0',
  `codPostal` varchar(5) NOT NULL,
  `Provincia` varchar(255) NOT NULL,
  `idUsuario` int NOT NULL DEFAULT '0',
  `latitud` double NOT NULL,
  `longitud` double NOT NULL,
  `demo` int NOT NULL DEFAULT '0',
  `idGrupo` int NOT NULL DEFAULT '1',
  `textoPueblo` varchar(100) NOT NULL,
  `colorPueblo` varchar(7) NOT NULL,
  `visibleListado` int NOT NULL DEFAULT '1',
  `pedidoMinimo` double(25,2) NOT NULL DEFAULT '8.00',
  `minutosAntes` int NOT NULL DEFAULT '0',
  `radio` int NOT NULL DEFAULT '0',
  `especial` int NOT NULL DEFAULT '0',
  `entregaCasa` int NOT NULL DEFAULT '0',
  `direccionEntrega` text NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_pueblos_grupos`
--

CREATE TABLE `qo_pueblos_grupos` (
  `id` int NOT NULL,
  `idGrupo` int NOT NULL,
  `nombreGrupo` varchar(255) NOT NULL,
  `idPueblo` int NOT NULL,
  `estado` int NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_pueblos_horarios`
--

CREATE TABLE `qo_pueblos_horarios` (
  `id` int NOT NULL,
  `idPuebloOrigen` int NOT NULL,
  `idPuebloDestino` int NOT NULL,
  `hora` time NOT NULL,
  `dia` varchar(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_puntos_usuario`
--

CREATE TABLE `qo_puntos_usuario` (
  `id` int NOT NULL,
  `idUsuario` int NOT NULL,
  `idEstablecimiento` int NOT NULL,
  `puntos` int NOT NULL,
  `puntosAnt` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_repartidores`
--

CREATE TABLE `qo_repartidores` (
  `id` int NOT NULL,
  `nombre` varchar(255) NOT NULL,
  `foto` varchar(255) NOT NULL,
  `pin` varchar(255) NOT NULL,
  `idUsuario` int NOT NULL DEFAULT '0',
  `activo` tinyint(1) NOT NULL DEFAULT '1',
  `idPueblo` int NOT NULL DEFAULT '1',
  `eliminado` int NOT NULL DEFAULT '0',
  `idGrupo` int NOT NULL DEFAULT '1',
  `telefono` varchar(20) NOT NULL DEFAULT ''
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_repartidores_establecimientos`
--

CREATE TABLE `qo_repartidores_establecimientos` (
  `id` int NOT NULL,
  `idRepartidor` int NOT NULL,
  `idEstablecimiento` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_repartidores_pedidos`
--

CREATE TABLE `qo_repartidores_pedidos` (
  `id` int NOT NULL,
  `idPedido` int NOT NULL,
  `idRepartidor` int NOT NULL,
  `estadoPedido` int NOT NULL,
  `fechaAsignacion` datetime NOT NULL,
  `fechaEntrega` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_repartidores_zonas`
--

CREATE TABLE `qo_repartidores_zonas` (
  `id` int NOT NULL,
  `idRepartidor` int NOT NULL,
  `idZona` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_reservas`
--

CREATE TABLE `qo_reservas` (
  `id` int NOT NULL,
  `idEstablecimiento` int NOT NULL,
  `idUsuario` int NOT NULL,
  `comensales` int NOT NULL,
  `bebes` int NOT NULL,
  `estado` int NOT NULL DEFAULT '1' COMMENT '1-Pendiente de confirmar,2-Aceptada,3-Solicitud Enviada,4-Rechazada'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_reservas_estados`
--

CREATE TABLE `qo_reservas_estados` (
  `id` int NOT NULL,
  `idReserva` int NOT NULL,
  `comentario` text NOT NULL,
  `fecha` datetime NOT NULL,
  `estado` int NOT NULL COMMENT '1-Pendiente de confirmar,2-Aceptada,3-Solicitud Enviada,4-Rechazada',
  `idZonaEstablecimiento` int NOT NULL,
  `stampi` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_roles`
--

CREATE TABLE `qo_roles` (
  `id` int NOT NULL,
  `nombre` varchar(255) DEFAULT NULL,
  `estado` int DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_sorteos`
--

CREATE TABLE `qo_sorteos` (
  `id` int NOT NULL,
  `idPueblo` int NOT NULL,
  `activo` int NOT NULL DEFAULT '0',
  `numeros` int NOT NULL,
  `cantidad` int NOT NULL,
  `fechaInicio` date NOT NULL,
  `fechaFin` date NOT NULL,
  `nombre` varchar(255) NOT NULL,
  `descripcion` text NOT NULL,
  `pedidoMinimo` double(25,2) NOT NULL DEFAULT '0.00'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_sorteos_numeros`
--

CREATE TABLE `qo_sorteos_numeros` (
  `id` int NOT NULL,
  `idSorteo` int NOT NULL,
  `numero` varchar(10) NOT NULL,
  `idCliente` int NOT NULL,
  `fecha` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_tipo_establecimiento`
--

CREATE TABLE `qo_tipo_establecimiento` (
  `id` int NOT NULL,
  `nombre` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 ROW_FORMAT=DYNAMIC;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_tipo_pedido`
--

CREATE TABLE `qo_tipo_pedido` (
  `id` int NOT NULL,
  `nombre` varchar(255) NOT NULL,
  `plusvalia` double(3,2) NOT NULL,
  `tipoPlusvalia` int NOT NULL DEFAULT '1'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 ROW_FORMAT=DYNAMIC;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_tokens`
--

CREATE TABLE `qo_tokens` (
  `id` int NOT NULL,
  `nombre` varchar(255) NOT NULL,
  `token` text NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_union_pueblos`
--

CREATE TABLE `qo_union_pueblos` (
  `id` int NOT NULL,
  `idPuebloOrigen` int NOT NULL,
  `idPuebloDestino` int NOT NULL,
  `estado` int NOT NULL DEFAULT '0',
  `gastos` double(5,2) NOT NULL DEFAULT '0.00'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_users`
--

CREATE TABLE `qo_users` (
  `nombre` varchar(255) DEFAULT NULL,
  `apellidos` varchar(255) DEFAULT NULL,
  `dni` varchar(10) DEFAULT NULL,
  `cod_postal` varchar(20) DEFAULT NULL,
  `poblacion` varchar(255) DEFAULT NULL,
  `provincia` varchar(255) DEFAULT NULL,
  `direccion` varchar(255) DEFAULT NULL,
  `fechaNacimiento` date DEFAULT NULL,
  `fechaAlta` date DEFAULT NULL,
  `telefono` varchar(20) DEFAULT NULL,
  `email` varchar(255) DEFAULT NULL,
  `password` varchar(255) DEFAULT NULL,
  `username` varchar(255) DEFAULT NULL,
  `foto` varchar(255) DEFAULT NULL,
  `rol` int DEFAULT NULL,
  `estado` int DEFAULT '0',
  `plataforma` varchar(255) NOT NULL,
  `token` varchar(255) NOT NULL,
  `tipoRegistro` int NOT NULL DEFAULT '0' COMMENT '0:Email,1:Facebook,2:Twitter,3:Instagram',
  `demo` int NOT NULL DEFAULT '0',
  `id` int NOT NULL,
  `pin` varchar(4) NOT NULL,
  `verificado` int NOT NULL DEFAULT '0',
  `idZona` int NOT NULL DEFAULT '1',
  `version` varchar(30) DEFAULT NULL,
  `idPueblo` int NOT NULL DEFAULT '1',
  `idSocial` varchar(255) NOT NULL DEFAULT '0',
  `social` varchar(255) DEFAULT NULL,
  `bloqueado` int NOT NULL DEFAULT '0',
  `versionFW` int NOT NULL DEFAULT '1',
  `codigo` varchar(20) NOT NULL DEFAULT '',
  `saldo` double(25,2) NOT NULL DEFAULT '0.00',
  `kiosko` int NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_users_cards`
--

CREATE TABLE `qo_users_cards` (
  `id` int NOT NULL,
  `pan` varchar(255) NOT NULL,
  `cardBrand` varchar(100) NOT NULL,
  `cardType` varchar(100) NOT NULL,
  `expiryDate` varchar(30) NOT NULL,
  `idUsuario` int NOT NULL,
  `idUser` int NOT NULL,
  `tokenUser` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_users_especial`
--

CREATE TABLE `qo_users_especial` (
  `id` int NOT NULL,
  `idUsuario` int NOT NULL,
  `idZona` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_users_est`
--

CREATE TABLE `qo_users_est` (
  `id` int NOT NULL,
  `idUser` int NOT NULL,
  `idEstablecimiento` int NOT NULL,
  `activo` int NOT NULL DEFAULT '1'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 ROW_FORMAT=DYNAMIC;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_valoraciones`
--

CREATE TABLE `qo_valoraciones` (
  `id` int NOT NULL,
  `idUsuario` int NOT NULL,
  `idEstablecimiento` int NOT NULL,
  `valoracion` double(5,1) NOT NULL,
  `fecha` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 ROW_FORMAT=DYNAMIC;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_valoraciones_usuarios`
--

CREATE TABLE `qo_valoraciones_usuarios` (
  `id` int NOT NULL,
  `idValoracion` int NOT NULL DEFAULT '0',
  `tipoValoracion` int NOT NULL DEFAULT '0' COMMENT '1=Establecimiento,2:Producto,3:App,4:Repartidor,5:Tiempo Entrega',
  `valoracion` double(5,2) NOT NULL DEFAULT '0.00',
  `codigoPedido` varchar(100) NOT NULL,
  `fecha` datetime NOT NULL,
  `rechazada` int NOT NULL DEFAULT '0',
  `comentario` text NOT NULL,
  `idUsuario` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_valoracion_pedidos`
--

CREATE TABLE `qo_valoracion_pedidos` (
  `id` int NOT NULL,
  `idUsuario` int NOT NULL,
  `idPedido` int NOT NULL,
  `valoracionEstablecimiento` double(5,2) NOT NULL DEFAULT '0.00',
  `valoracionServicio` double(5,2) NOT NULL DEFAULT '0.00',
  `valoracionPuntualidad` double(5,2) NOT NULL DEFAULT '0.00',
  `valoracionRepartidor` double(5,2) NOT NULL DEFAULT '0.00',
  `comentario` text NOT NULL,
  `fecha` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `qo_zonas`
--

CREATE TABLE `qo_zonas` (
  `id` int NOT NULL,
  `nombre` varchar(255) NOT NULL,
  `gastos` decimal(5,2) NOT NULL DEFAULT '1.90',
  `pedidoMinimo` double(5,2) NOT NULL DEFAULT '12.00',
  `color` varchar(20) NOT NULL,
  `cambiaDireccion` tinyint(1) NOT NULL DEFAULT '1',
  `direccionEnvio` varchar(255) NOT NULL,
  `activo` int NOT NULL DEFAULT '0',
  `idPueblo` int NOT NULL DEFAULT '1',
  `modificable` int NOT NULL DEFAULT '1'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Estructura Stand-in para la vista `v_categorias_est`
-- (Véase abajo para la vista actual)
--
CREATE TABLE `v_categorias_est` (
`contador` bigint
,`idEstablecimiento` int
);

-- --------------------------------------------------------

--
-- Estructura Stand-in para la vista `v_productos_est`
-- (Véase abajo para la vista actual)
--
CREATE TABLE `v_productos_est` (
`contador` bigint
,`idEstablecimiento` int
);

-- --------------------------------------------------------

--
-- Estructura Stand-in para la vista `v_ventas_est`
-- (Véase abajo para la vista actual)
--
CREATE TABLE `v_ventas_est` (
`contador` double(19,2)
,`idEstablecimiento` int
);

-- --------------------------------------------------------

--
-- Estructura Stand-in para la vista `v_zonas_est`
-- (Véase abajo para la vista actual)
--
CREATE TABLE `v_zonas_est` (
`contador` bigint
,`idEstablecimiento` int
);

-- --------------------------------------------------------

--
-- Estructura para la vista `v_categorias_est`
--
DROP TABLE IF EXISTS `v_categorias_est`;

CREATE ALGORITHM=UNDEFINED DEFINER=`o13376654`@`%` SQL SECURITY DEFINER VIEW `v_categorias_est`  AS SELECT count(0) AS `contador`, `qo_productos_cat`.`idEstablecimiento` AS `idEstablecimiento` FROM `qo_productos_cat` GROUP BY `qo_productos_cat`.`idEstablecimiento``idEstablecimiento` ;

-- --------------------------------------------------------

--
-- Estructura para la vista `v_productos_est`
--
DROP TABLE IF EXISTS `v_productos_est`;

CREATE ALGORITHM=UNDEFINED DEFINER=`o13376654`@`%` SQL SECURITY DEFINER VIEW `v_productos_est`  AS SELECT count(0) AS `contador`, `ca`.`idEstablecimiento` AS `idEstablecimiento` FROM (`qo_productos_est` `es` join `qo_productos_cat` `ca` on((`es`.`idCategoria` = `ca`.`id`))) GROUP BY `ca`.`idEstablecimiento``idEstablecimiento` ;

-- --------------------------------------------------------

--
-- Estructura para la vista `v_ventas_est`
--
DROP TABLE IF EXISTS `v_ventas_est`;

CREATE ALGORITHM=UNDEFINED DEFINER=`o13376654`@`%` SQL SECURITY DEFINER VIEW `v_ventas_est`  AS SELECT sum((`det`.`cantidad` * `det`.`precio`)) AS `contador`, `pe`.`idEstablecimiento` AS `idEstablecimiento` FROM (`qo_pedidos_detalle` `det` join `qo_pedidos` `pe` on((`det`.`idPedido` = `pe`.`id`))) GROUP BY `pe`.`idEstablecimiento``idEstablecimiento` ;

-- --------------------------------------------------------

--
-- Estructura para la vista `v_zonas_est`
--
DROP TABLE IF EXISTS `v_zonas_est`;

CREATE ALGORITHM=UNDEFINED DEFINER=`o13376654`@`%` SQL SECURITY DEFINER VIEW `v_zonas_est`  AS SELECT count(0) AS `contador`, `qo_establecimientos_zonas`.`idEstablecimiento` AS `idEstablecimiento` FROM `qo_establecimientos_zonas` GROUP BY `qo_establecimientos_zonas`.`idEstablecimiento``idEstablecimiento` ;

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `qo_administradores_pueblos`
--
ALTER TABLE `qo_administradores_pueblos`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_alergenos`
--
ALTER TABLE `qo_alergenos`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_amigos`
--
ALTER TABLE `qo_amigos`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_camareros`
--
ALTER TABLE `qo_camareros`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_camareros_zonas`
--
ALTER TABLE `qo_camareros_zonas`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_codigos_postales`
--
ALTER TABLE `qo_codigos_postales`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_combo_entrantes`
--
ALTER TABLE `qo_combo_entrantes`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_configuracion`
--
ALTER TABLE `qo_configuracion`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_configuracion_est`
--
ALTER TABLE `qo_configuracion_est`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_configuracion_global`
--
ALTER TABLE `qo_configuracion_global`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_cuentas`
--
ALTER TABLE `qo_cuentas`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_cuentas_detalle`
--
ALTER TABLE `qo_cuentas_detalle`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_cupones`
--
ALTER TABLE `qo_cupones`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_cupones_sql`
--
ALTER TABLE `qo_cupones_sql`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_cupones_usuario`
--
ALTER TABLE `qo_cupones_usuario`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_equipos`
--
ALTER TABLE `qo_equipos`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_establecimientos`
--
ALTER TABLE `qo_establecimientos`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_establecimientos_fiscal`
--
ALTER TABLE `qo_establecimientos_fiscal`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_establecimientos_valoraciones`
--
ALTER TABLE `qo_establecimientos_valoraciones`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_establecimientos_visitas`
--
ALTER TABLE `qo_establecimientos_visitas`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_establecimientos_zonas`
--
ALTER TABLE `qo_establecimientos_zonas`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_estados`
--
ALTER TABLE `qo_estados`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_eventos`
--
ALTER TABLE `qo_eventos`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_facturas`
--
ALTER TABLE `qo_facturas`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_facturas_administradores`
--
ALTER TABLE `qo_facturas_administradores`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_facturas_detalle`
--
ALTER TABLE `qo_facturas_detalle`
  ADD PRIMARY KEY (`id`),
  ADD KEY `idFactura` (`idFactura`);

--
-- Indices de la tabla `qo_forma_pago`
--
ALTER TABLE `qo_forma_pago`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_gastos`
--
ALTER TABLE `qo_gastos`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_ingredientes`
--
ALTER TABLE `qo_ingredientes`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_ingredientes_establecimiento`
--
ALTER TABLE `qo_ingredientes_establecimiento`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_ingredientes_producto`
--
ALTER TABLE `qo_ingredientes_producto`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_logs`
--
ALTER TABLE `qo_logs`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_logs_app`
--
ALTER TABLE `qo_logs_app`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_mensajes`
--
ALTER TABLE `qo_mensajes`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_mensajes_camarero`
--
ALTER TABLE `qo_mensajes_camarero`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_mensajes_repartidor`
--
ALTER TABLE `qo_mensajes_repartidor`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_mensajes_repartidor_predef`
--
ALTER TABLE `qo_mensajes_repartidor_predef`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_menu`
--
ALTER TABLE `qo_menu`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_menu_diario`
--
ALTER TABLE `qo_menu_diario`
  ADD PRIMARY KEY (`id`),
  ADD KEY `idEstablecimiento` (`idEstablecimiento`);

--
-- Indices de la tabla `qo_menu_diario_conf`
--
ALTER TABLE `qo_menu_diario_conf`
  ADD PRIMARY KEY (`id`),
  ADD KEY `idEstablecimiento` (`idEstablecimiento`);

--
-- Indices de la tabla `qo_menu_diario_prod`
--
ALTER TABLE `qo_menu_diario_prod`
  ADD PRIMARY KEY (`id`),
  ADD KEY `idMenu` (`idMenu`);

--
-- Indices de la tabla `qo_multi_establecimiento`
--
ALTER TABLE `qo_multi_establecimiento`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_numero_usuarios`
--
ALTER TABLE `qo_numero_usuarios`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_ofertas`
--
ALTER TABLE `qo_ofertas`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_online`
--
ALTER TABLE `qo_online`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_pedidos`
--
ALTER TABLE `qo_pedidos`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `codigo` (`codigo`);

--
-- Indices de la tabla `qo_pedidos_detalle`
--
ALTER TABLE `qo_pedidos_detalle`
  ADD PRIMARY KEY (`id`),
  ADD KEY `indice` (`id`,`idPedido`);

--
-- Indices de la tabla `qo_pedidos_estado`
--
ALTER TABLE `qo_pedidos_estado`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_posicion_repartidor`
--
ALTER TABLE `qo_posicion_repartidor`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_productos_cat`
--
ALTER TABLE `qo_productos_cat`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_productos_est`
--
ALTER TABLE `qo_productos_est`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_productos_ing`
--
ALTER TABLE `qo_productos_ing`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_productos_ing_aler`
--
ALTER TABLE `qo_productos_ing_aler`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_productos_opc`
--
ALTER TABLE `qo_productos_opc`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_promocion_amigo`
--
ALTER TABLE `qo_promocion_amigo`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_publicidad`
--
ALTER TABLE `qo_publicidad`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_pueblos`
--
ALTER TABLE `qo_pueblos`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_pueblos_grupos`
--
ALTER TABLE `qo_pueblos_grupos`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_pueblos_horarios`
--
ALTER TABLE `qo_pueblos_horarios`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_repartidores`
--
ALTER TABLE `qo_repartidores`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_repartidores_establecimientos`
--
ALTER TABLE `qo_repartidores_establecimientos`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_repartidores_pedidos`
--
ALTER TABLE `qo_repartidores_pedidos`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_repartidores_zonas`
--
ALTER TABLE `qo_repartidores_zonas`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_reservas`
--
ALTER TABLE `qo_reservas`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_roles`
--
ALTER TABLE `qo_roles`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_sorteos`
--
ALTER TABLE `qo_sorteos`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_sorteos_numeros`
--
ALTER TABLE `qo_sorteos_numeros`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `numero` (`numero`,`idSorteo`),
  ADD KEY `idCliente` (`idCliente`),
  ADD KEY `idSorteo` (`idSorteo`);

--
-- Indices de la tabla `qo_tipo_establecimiento`
--
ALTER TABLE `qo_tipo_establecimiento`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_tipo_pedido`
--
ALTER TABLE `qo_tipo_pedido`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_tokens`
--
ALTER TABLE `qo_tokens`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_union_pueblos`
--
ALTER TABLE `qo_union_pueblos`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_users`
--
ALTER TABLE `qo_users`
  ADD PRIMARY KEY (`id`),
  ADD KEY `rol` (`rol`);

--
-- Indices de la tabla `qo_users_cards`
--
ALTER TABLE `qo_users_cards`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_users_especial`
--
ALTER TABLE `qo_users_especial`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_users_est`
--
ALTER TABLE `qo_users_est`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_valoraciones`
--
ALTER TABLE `qo_valoraciones`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_valoraciones_usuarios`
--
ALTER TABLE `qo_valoraciones_usuarios`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_valoracion_pedidos`
--
ALTER TABLE `qo_valoracion_pedidos`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `qo_zonas`
--
ALTER TABLE `qo_zonas`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `qo_administradores_pueblos`
--
ALTER TABLE `qo_administradores_pueblos`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_alergenos`
--
ALTER TABLE `qo_alergenos`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_amigos`
--
ALTER TABLE `qo_amigos`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_camareros`
--
ALTER TABLE `qo_camareros`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_camareros_zonas`
--
ALTER TABLE `qo_camareros_zonas`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_codigos_postales`
--
ALTER TABLE `qo_codigos_postales`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_combo_entrantes`
--
ALTER TABLE `qo_combo_entrantes`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_configuracion`
--
ALTER TABLE `qo_configuracion`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_configuracion_est`
--
ALTER TABLE `qo_configuracion_est`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_configuracion_global`
--
ALTER TABLE `qo_configuracion_global`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_cuentas`
--
ALTER TABLE `qo_cuentas`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_cuentas_detalle`
--
ALTER TABLE `qo_cuentas_detalle`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_cupones`
--
ALTER TABLE `qo_cupones`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_cupones_sql`
--
ALTER TABLE `qo_cupones_sql`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_cupones_usuario`
--
ALTER TABLE `qo_cupones_usuario`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_equipos`
--
ALTER TABLE `qo_equipos`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_establecimientos`
--
ALTER TABLE `qo_establecimientos`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_establecimientos_fiscal`
--
ALTER TABLE `qo_establecimientos_fiscal`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_establecimientos_valoraciones`
--
ALTER TABLE `qo_establecimientos_valoraciones`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_establecimientos_visitas`
--
ALTER TABLE `qo_establecimientos_visitas`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_establecimientos_zonas`
--
ALTER TABLE `qo_establecimientos_zonas`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_estados`
--
ALTER TABLE `qo_estados`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_eventos`
--
ALTER TABLE `qo_eventos`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_facturas`
--
ALTER TABLE `qo_facturas`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_facturas_administradores`
--
ALTER TABLE `qo_facturas_administradores`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_facturas_detalle`
--
ALTER TABLE `qo_facturas_detalle`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_forma_pago`
--
ALTER TABLE `qo_forma_pago`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_gastos`
--
ALTER TABLE `qo_gastos`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_ingredientes`
--
ALTER TABLE `qo_ingredientes`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_ingredientes_establecimiento`
--
ALTER TABLE `qo_ingredientes_establecimiento`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_ingredientes_producto`
--
ALTER TABLE `qo_ingredientes_producto`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_logs`
--
ALTER TABLE `qo_logs`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_logs_app`
--
ALTER TABLE `qo_logs_app`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_mensajes`
--
ALTER TABLE `qo_mensajes`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_mensajes_camarero`
--
ALTER TABLE `qo_mensajes_camarero`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_mensajes_repartidor`
--
ALTER TABLE `qo_mensajes_repartidor`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_mensajes_repartidor_predef`
--
ALTER TABLE `qo_mensajes_repartidor_predef`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_menu`
--
ALTER TABLE `qo_menu`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_menu_diario`
--
ALTER TABLE `qo_menu_diario`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_menu_diario_conf`
--
ALTER TABLE `qo_menu_diario_conf`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_menu_diario_prod`
--
ALTER TABLE `qo_menu_diario_prod`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_multi_establecimiento`
--
ALTER TABLE `qo_multi_establecimiento`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_numero_usuarios`
--
ALTER TABLE `qo_numero_usuarios`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_ofertas`
--
ALTER TABLE `qo_ofertas`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_online`
--
ALTER TABLE `qo_online`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_pedidos`
--
ALTER TABLE `qo_pedidos`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_pedidos_detalle`
--
ALTER TABLE `qo_pedidos_detalle`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_pedidos_estado`
--
ALTER TABLE `qo_pedidos_estado`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_posicion_repartidor`
--
ALTER TABLE `qo_posicion_repartidor`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_productos_cat`
--
ALTER TABLE `qo_productos_cat`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_productos_est`
--
ALTER TABLE `qo_productos_est`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_productos_ing_aler`
--
ALTER TABLE `qo_productos_ing_aler`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_productos_opc`
--
ALTER TABLE `qo_productos_opc`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_promocion_amigo`
--
ALTER TABLE `qo_promocion_amigo`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_publicidad`
--
ALTER TABLE `qo_publicidad`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_pueblos`
--
ALTER TABLE `qo_pueblos`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_pueblos_grupos`
--
ALTER TABLE `qo_pueblos_grupos`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_pueblos_horarios`
--
ALTER TABLE `qo_pueblos_horarios`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_repartidores`
--
ALTER TABLE `qo_repartidores`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_repartidores_establecimientos`
--
ALTER TABLE `qo_repartidores_establecimientos`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_repartidores_pedidos`
--
ALTER TABLE `qo_repartidores_pedidos`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_repartidores_zonas`
--
ALTER TABLE `qo_repartidores_zonas`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_reservas`
--
ALTER TABLE `qo_reservas`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_sorteos`
--
ALTER TABLE `qo_sorteos`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_sorteos_numeros`
--
ALTER TABLE `qo_sorteos_numeros`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_tokens`
--
ALTER TABLE `qo_tokens`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_union_pueblos`
--
ALTER TABLE `qo_union_pueblos`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_users`
--
ALTER TABLE `qo_users`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_users_cards`
--
ALTER TABLE `qo_users_cards`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_users_especial`
--
ALTER TABLE `qo_users_especial`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_users_est`
--
ALTER TABLE `qo_users_est`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_valoraciones_usuarios`
--
ALTER TABLE `qo_valoraciones_usuarios`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_valoracion_pedidos`
--
ALTER TABLE `qo_valoracion_pedidos`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `qo_zonas`
--
ALTER TABLE `qo_zonas`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `qo_facturas_detalle`
--
ALTER TABLE `qo_facturas_detalle`
  ADD CONSTRAINT `qo_facturas_detalle_ibfk_1` FOREIGN KEY (`idFactura`) REFERENCES `qo_facturas` (`id`);

--
-- Filtros para la tabla `qo_menu_diario`
--
ALTER TABLE `qo_menu_diario`
  ADD CONSTRAINT `qo_menu_diario_ibfk_1` FOREIGN KEY (`idEstablecimiento`) REFERENCES `qo_establecimientos` (`id`);

--
-- Filtros para la tabla `qo_menu_diario_conf`
--
ALTER TABLE `qo_menu_diario_conf`
  ADD CONSTRAINT `qo_menu_diario_conf_ibfk_1` FOREIGN KEY (`idEstablecimiento`) REFERENCES `qo_establecimientos` (`id`);

--
-- Filtros para la tabla `qo_menu_diario_prod`
--
ALTER TABLE `qo_menu_diario_prod`
  ADD CONSTRAINT `qo_menu_diario_prod_ibfk_1` FOREIGN KEY (`idMenu`) REFERENCES `qo_menu_diario` (`id`);

--
-- Filtros para la tabla `qo_sorteos_numeros`
--
ALTER TABLE `qo_sorteos_numeros`
  ADD CONSTRAINT `qo_sorteos_numeros_ibfk_1` FOREIGN KEY (`idCliente`) REFERENCES `qo_users` (`id`),
  ADD CONSTRAINT `qo_sorteos_numeros_ibfk_2` FOREIGN KEY (`idSorteo`) REFERENCES `qo_sorteos` (`id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
