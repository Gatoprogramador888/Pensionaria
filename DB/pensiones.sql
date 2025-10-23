-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 23-10-2025 a las 20:25:05
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `pensiones`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `andenes`
--

CREATE TABLE `andenes` (
  `numero_anden` int(11) NOT NULL,
  `ubicacion` varchar(150) DEFAULT NULL,
  `capacidad` varchar(50) DEFAULT NULL,
  `estado` enum('disponible','ocupado','mantenimiento') DEFAULT 'disponible'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `andenes_log`
--

CREATE TABLE `andenes_log` (
  `id_log` int(11) NOT NULL,
  `numero_anden` int(11) NOT NULL,
  `placa` varchar(20) NOT NULL,
  `conductor` varchar(100) NOT NULL,
  `hora_entrada` datetime NOT NULL,
  `hora_salida` datetime DEFAULT NULL,
  `estado` enum('proceso','finalizado') DEFAULT 'proceso'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `camion`
--

CREATE TABLE `camion` (
  `placa` varchar(20) NOT NULL,
  `tipo` varchar(50) DEFAULT NULL,
  `empresa` int(11) DEFAULT NULL,
  `servicio` enum('carga','descarga','c/d','pension') NOT NULL,
  `conductor` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `empresas`
--

CREATE TABLE `empresas` (
  `id_empresa` int(11) NOT NULL,
  `nombre_empresa` varchar(150) NOT NULL,
  `numero_telefonico` varchar(20) DEFAULT NULL,
  `correo` varchar(150) DEFAULT NULL,
  `fecha_afiliacion` date DEFAULT NULL,
  `fecha_expiracion_afiliacion` date DEFAULT NULL,
  `descuento` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`descuento`))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Disparadores `empresas`
--
DELIMITER $$
CREATE TRIGGER `update_descuento` AFTER UPDATE ON `empresas` FOR EACH ROW BEGIN
  IF NEW.descuento <> OLD.descuento THEN
    INSERT INTO Eventos(id_empresa, tipo_evento, fecha_evento, descripcion)
    VALUES (
      NEW.id_empresa,
      'accion',
      NOW(),
      CONCAT('Se modificó el descuento de la empresa ', NEW.nombre_empresa,
             ' de ', OLD.descuento, ' a ', NEW.descuento)
    );
  END IF;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `eventos`
--

CREATE TABLE `eventos` (
  `id_evento` int(11) NOT NULL,
  `id_empresa` int(11) NOT NULL,
  `tipo_evento` enum('error','advertencia','accion') NOT NULL,
  `fecha_evento` datetime DEFAULT current_timestamp(),
  `descripcion` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `pensiones`
--

CREATE TABLE `pensiones` (
  `numero_pension` int(11) NOT NULL,
  `ubicacion` varchar(150) DEFAULT NULL,
  `capacidad` varchar(50) DEFAULT NULL,
  `estado` enum('disponible','ocupado','mantenimiento') DEFAULT 'disponible'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `pensiones_log`
--

CREATE TABLE `pensiones_log` (
  `id_log` int(11) NOT NULL,
  `numero_pension` int(11) NOT NULL,
  `placa` varchar(20) NOT NULL,
  `conductor` varchar(100) NOT NULL,
  `hora_entrada` datetime NOT NULL,
  `hora_salida` datetime DEFAULT NULL,
  `estado` enum('disponible','ocupado','finalizado','excedido') DEFAULT 'disponible'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `andenes`
--
ALTER TABLE `andenes`
  ADD PRIMARY KEY (`numero_anden`);

--
-- Indices de la tabla `andenes_log`
--
ALTER TABLE `andenes_log`
  ADD PRIMARY KEY (`id_log`),
  ADD KEY `numero_anden` (`numero_anden`),
  ADD KEY `placa` (`placa`,`conductor`);

--
-- Indices de la tabla `camion`
--
ALTER TABLE `camion`
  ADD PRIMARY KEY (`placa`),
  ADD UNIQUE KEY `uk_placa_conductor` (`placa`,`conductor`),
  ADD KEY `empresa` (`empresa`);

--
-- Indices de la tabla `empresas`
--
ALTER TABLE `empresas`
  ADD PRIMARY KEY (`id_empresa`);

--
-- Indices de la tabla `eventos`
--
ALTER TABLE `eventos`
  ADD PRIMARY KEY (`id_evento`),
  ADD KEY `id_empresa` (`id_empresa`);

--
-- Indices de la tabla `pensiones`
--
ALTER TABLE `pensiones`
  ADD PRIMARY KEY (`numero_pension`);

--
-- Indices de la tabla `pensiones_log`
--
ALTER TABLE `pensiones_log`
  ADD PRIMARY KEY (`id_log`),
  ADD KEY `numero_pension` (`numero_pension`),
  ADD KEY `placa` (`placa`,`conductor`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `andenes_log`
--
ALTER TABLE `andenes_log`
  MODIFY `id_log` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `empresas`
--
ALTER TABLE `empresas`
  MODIFY `id_empresa` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `eventos`
--
ALTER TABLE `eventos`
  MODIFY `id_evento` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `pensiones_log`
--
ALTER TABLE `pensiones_log`
  MODIFY `id_log` int(11) NOT NULL AUTO_INCREMENT;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `andenes_log`
--
ALTER TABLE `andenes_log`
  ADD CONSTRAINT `andenes_log_ibfk_1` FOREIGN KEY (`numero_anden`) REFERENCES `andenes` (`numero_anden`),
  ADD CONSTRAINT `andenes_log_ibfk_2` FOREIGN KEY (`placa`,`conductor`) REFERENCES `camion` (`placa`, `conductor`);

--
-- Filtros para la tabla `camion`
--
ALTER TABLE `camion`
  ADD CONSTRAINT `camion_ibfk_1` FOREIGN KEY (`empresa`) REFERENCES `empresas` (`id_empresa`);

--
-- Filtros para la tabla `eventos`
--
ALTER TABLE `eventos`
  ADD CONSTRAINT `eventos_ibfk_1` FOREIGN KEY (`id_empresa`) REFERENCES `empresas` (`id_empresa`);

--
-- Filtros para la tabla `pensiones_log`
--
ALTER TABLE `pensiones_log`
  ADD CONSTRAINT `pensiones_log_ibfk_1` FOREIGN KEY (`numero_pension`) REFERENCES `pensiones` (`numero_pension`),
  ADD CONSTRAINT `pensiones_log_ibfk_2` FOREIGN KEY (`placa`,`conductor`) REFERENCES `camion` (`placa`, `conductor`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
