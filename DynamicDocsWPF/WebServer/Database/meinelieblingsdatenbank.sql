-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server Version:               10.1.36-MariaDB - mariadb.org binary distribution
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

-- Exportiere Struktur von Tabelle processmanagement.archivedprocess
CREATE TABLE IF NOT EXISTS `archivedprocess` (
  `CURRENTPROCESS_ID` int(11) NOT NULL,
  `PROCESSTEMPLATE_ID` varchar(255) DEFAULT NULL,
  `OWNER_ID` int(11) DEFAULT NULL,
  `CURRENTSTEP` int(11) DEFAULT NULL,
  `DECLINED` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`CURRENTPROCESS_ID`),
  KEY `PROCESSTEMPLATE_ID` (`PROCESSTEMPLATE_ID`),
  KEY `OWNER_ID` (`OWNER_ID`),
  CONSTRAINT `archivedprocess_ibfk_1` FOREIGN KEY (`PROCESSTEMPLATE_ID`) REFERENCES `processtemplate` (`PROCESS_ID`) ON DELETE CASCADE,
  CONSTRAINT `archivedprocess_ibfk_2` FOREIGN KEY (`OWNER_ID`) REFERENCES `user` (`USER_ID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Daten Export vom Benutzer nicht ausgewählt
-- Exportiere Struktur von Tabelle processmanagement.archivepermission
CREATE TABLE IF NOT EXISTS `archivepermission` (
  `ARCHIVEDPROCESS_ID` int(11) NOT NULL,
  `AUTHORIZEDUSER_ID` int(11) NOT NULL,
  PRIMARY KEY (`ARCHIVEDPROCESS_ID`,`AUTHORIZEDUSER_ID`),
  KEY `AUTHORIZEDUSER_ID` (`AUTHORIZEDUSER_ID`),
  CONSTRAINT `archivepermission_ibfk_1` FOREIGN KEY (`ARCHIVEDPROCESS_ID`) REFERENCES `archivedprocess` (`CURRENTPROCESS_ID`) ON DELETE CASCADE,
  CONSTRAINT `archivepermission_ibfk_2` FOREIGN KEY (`AUTHORIZEDUSER_ID`) REFERENCES `user` (`USER_ID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Daten Export vom Benutzer nicht ausgewählt
-- Exportiere Struktur von Tabelle processmanagement.doctemplate
CREATE TABLE IF NOT EXISTS `doctemplate` (
  `DOCTEMPLATE_ID` varchar(255) NOT NULL,
  `FILEPATH` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`DOCTEMPLATE_ID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Daten Export vom Benutzer nicht ausgewählt
-- Exportiere Struktur von Tabelle processmanagement.entry
CREATE TABLE IF NOT EXISTS `entry` (
  `ENTRY_ID` int(11) NOT NULL,
  `PROCESS_ID` int(11) DEFAULT NULL,
  `FIELDNAME` varchar(255) DEFAULT NULL,
  `DATATYPE` varchar(255) DEFAULT NULL,
  `DATA` varchar(255) DEFAULT NULL,
  `PERMISSIONLEVEL` int(11) DEFAULT NULL,
  PRIMARY KEY (`ENTRY_ID`),
  KEY `PROCESS_ID` (`PROCESS_ID`),
  CONSTRAINT `entry_ibfk_1` FOREIGN KEY (`PROCESS_ID`) REFERENCES `runningprocess` (`CURRENTPROCESS_ID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Daten Export vom Benutzer nicht ausgewählt
-- Exportiere Struktur von Tabelle processmanagement.processtemplate
CREATE TABLE IF NOT EXISTS `processtemplate` (
  `PROCESS_ID` varchar(255) NOT NULL,
  `FILEPATH` varchar(255) DEFAULT NULL,
  `DESCRIPTION` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`PROCESS_ID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Daten Export vom Benutzer nicht ausgewählt
-- Exportiere Struktur von Tabelle processmanagement.roles
CREATE TABLE IF NOT EXISTS `roles` (
  `ROLE_ID` varchar(255) NOT NULL,
  `USER_ID` int(11) DEFAULT NULL,
  PRIMARY KEY (`ROLE_ID`),
  KEY `USER_ID` (`USER_ID`),
  CONSTRAINT `roles_ibfk_1` FOREIGN KEY (`USER_ID`) REFERENCES `user` (`USER_ID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Daten Export vom Benutzer nicht ausgewählt
-- Exportiere Struktur von Tabelle processmanagement.runningprocess
CREATE TABLE IF NOT EXISTS `runningprocess` (
  `CURRENTPROCESS_ID` int(11) NOT NULL,
  `PROCESSTEMPLATE_ID` varchar(255) DEFAULT NULL,
  `OWNER_ID` int(11) DEFAULT NULL,
  `CURRENTSTEP` int(11) DEFAULT NULL,
  `DECLINED` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`CURRENTPROCESS_ID`),
  KEY `PROCESSTEMPLATE_ID` (`PROCESSTEMPLATE_ID`),
  KEY `OWNER_ID` (`OWNER_ID`),
  CONSTRAINT `runningprocess_ibfk_1` FOREIGN KEY (`PROCESSTEMPLATE_ID`) REFERENCES `processtemplate` (`PROCESS_ID`) ON DELETE CASCADE,
  CONSTRAINT `runningprocess_ibfk_2` FOREIGN KEY (`OWNER_ID`) REFERENCES `user` (`USER_ID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Daten Export vom Benutzer nicht ausgewählt
-- Exportiere Struktur von Tabelle processmanagement.user
CREATE TABLE IF NOT EXISTS `user` (
  `USER_ID` int(11) NOT NULL,
  `EMAIL` varchar(255) DEFAULT NULL,
  `PASSWORDHASH` varchar(255) DEFAULT NULL,
  `PERMISSIONLEVEL` int(11) DEFAULT NULL,
  PRIMARY KEY (`USER_ID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Daten Export vom Benutzer nicht ausgewählt
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
