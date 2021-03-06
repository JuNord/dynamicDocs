-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server Version:               10.1.37-MariaDB - mariadb.org binary distribution
-- Server Betriebssystem:        Win32
-- HeidiSQL Version:             9.5.0.5196
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;


-- Exportiere Datenbank Struktur für processmanagement
CREATE DATABASE IF NOT EXISTS `processmanagement` /*!40100 DEFAULT CHARACTER SET latin1 */;
USE `processmanagement`;

-- Exportiere Struktur von Tabelle processmanagement.archivepermission
CREATE TABLE IF NOT EXISTS `archivepermission` (
  `PROCESS_ID` int(11) NOT NULL,
  `MAIL` varchar(255) NOT NULL,
  PRIMARY KEY (`PROCESS_ID`,`MAIL`),
  KEY `AUTHORIZEDUSER_ID` (`MAIL`),
  CONSTRAINT `archivepermission_ibfk_1` FOREIGN KEY (`PROCESS_ID`) REFERENCES `processinstance` (`ID`) ON DELETE CASCADE,
  CONSTRAINT `archivepermission_ibfk_2` FOREIGN KEY (`MAIL`) REFERENCES `user` (`EMAIL`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Daten Export vom Benutzer nicht ausgewählt
-- Exportiere Struktur von Tabelle processmanagement.doctemplate
CREATE TABLE IF NOT EXISTS `doctemplate` (
  `ID` varchar(255) NOT NULL,
  `FILEPATH` varchar(255) NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Daten Export vom Benutzer nicht ausgewählt
-- Exportiere Struktur von Tabelle processmanagement.entry
CREATE TABLE IF NOT EXISTS `entry` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `PROCESS_ID` int(11) NOT NULL,
  `FIELDNAME` varchar(255) NOT NULL,
  `DATA` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`ID`,`FIELDNAME`,`PROCESS_ID`),
  KEY `PROCESS_ID` (`PROCESS_ID`),
  CONSTRAINT `Entry_FK` FOREIGN KEY (`PROCESS_ID`) REFERENCES `processinstance` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=40 DEFAULT CHARSET=latin1;

-- Daten Export vom Benutzer nicht ausgewählt
-- Exportiere Struktur von Tabelle processmanagement.pendinginstance
CREATE TABLE IF NOT EXISTS `pendinginstance` (
  `INSTANCE_ID` int(11) NOT NULL,
  `MAIL` varchar(255) NOT NULL,
  `ROLE` varchar(255) NOT NULL,
  PRIMARY KEY (`INSTANCE_ID`),
  KEY `INSTANCE_ID` (`INSTANCE_ID`),
  CONSTRAINT `pending_ibfk_1` FOREIGN KEY (`INSTANCE_ID`) REFERENCES `processinstance` (`ID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Daten Export vom Benutzer nicht ausgewählt
-- Exportiere Struktur von Tabelle processmanagement.processinstance
CREATE TABLE IF NOT EXISTS `processinstance` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `TEMPLATE_ID` varchar(255) DEFAULT NULL,
  `OWNER_ID` varchar(255) DEFAULT NULL,
  `CURRENTSTEP` int(11) DEFAULT NULL,
  `DECLINED` tinyint(1) DEFAULT NULL,
  `ARCHIVED` tinyint(1) DEFAULT NULL,
  `LOCKED` tinyint(1) DEFAULT NULL,
  `CREATED` datetime NOT NULL,
  `CHANGED` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `SUBJECT` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`ID`),
  KEY `TEMPLATE_ID` (`TEMPLATE_ID`),
  KEY `OWNER_ID` (`OWNER_ID`),
  CONSTRAINT `runningprocess_ibfk_1` FOREIGN KEY (`TEMPLATE_ID`) REFERENCES `processtemplate` (`ID`) ON DELETE CASCADE,
  CONSTRAINT `runningprocess_ibfk_2` FOREIGN KEY (`OWNER_ID`) REFERENCES `user` (`EMAIL`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=latin1;

-- Daten Export vom Benutzer nicht ausgewählt
-- Exportiere Struktur von Tabelle processmanagement.processtemplate
CREATE TABLE IF NOT EXISTS `processtemplate` (
  `ID` varchar(255) NOT NULL,
  `FILEPATH` varchar(255) DEFAULT NULL,
  `DESCRIPTION` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Daten Export vom Benutzer nicht ausgewählt
-- Exportiere Struktur von Tabelle processmanagement.roles
CREATE TABLE IF NOT EXISTS `roles` (
  `ID` varchar(255) NOT NULL,
  `MAIL` varchar(255) NOT NULL,
  PRIMARY KEY (`ID`,`MAIL`),
  KEY `USER_ID` (`MAIL`),
  CONSTRAINT `Roles_FK` FOREIGN KEY (`MAIL`) REFERENCES `user` (`EMAIL`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Daten Export vom Benutzer nicht ausgewählt
-- Exportiere Struktur von Tabelle processmanagement.user
CREATE TABLE IF NOT EXISTS `user` (
  `EMAIL` varchar(255) NOT NULL,
  `PASSWORD` varchar(255) DEFAULT NULL,
  `PERMISSIONLEVEL` int(11) DEFAULT NULL,
  PRIMARY KEY (`EMAIL`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Daten Export vom Benutzer nicht ausgewählt
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
