-- MySQL dump 10.13  Distrib 8.0.38, for Win64 (x86_64)
--
-- Host: localhost    Database: erronka2_2026
-- ------------------------------------------------------
-- Server version	8.0.39

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `erabiltzaileak`
--

DROP TABLE IF EXISTS `erabiltzaileak`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `erabiltzaileak` (
  `id` int NOT NULL AUTO_INCREMENT,
  `erabiltzailea` varchar(100) DEFAULT NULL,
  `email` varchar(100) DEFAULT NULL,
  `pasahitza` varchar(255) DEFAULT NULL,
  `rola_id` int DEFAULT NULL,
  `ezabatua` tinyint DEFAULT '0',
  `chat` tinyint DEFAULT '0',
  `izena` varchar(45) DEFAULT NULL,
  `abizena` varchar(45) DEFAULT NULL,
  `telefonoa` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_erabil_email` (`email`),
  KEY `ix_erabil_rola` (`rola_id`),
  CONSTRAINT `fk_erabil_rola` FOREIGN KEY (`rola_id`) REFERENCES `rolak` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `erreserbak`
--

DROP TABLE IF EXISTS `erreserbak`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `erreserbak` (
  `id` int NOT NULL AUTO_INCREMENT,
  `data` timestamp NULL DEFAULT NULL,
  `mota` tinyint DEFAULT NULL,
  `erabiltzaileak_id` int DEFAULT NULL,
  `mahaiak_id` int NOT NULL,
  PRIMARY KEY (`id`),
  KEY `ix_erreserbak_erabil` (`erabiltzaileak_id`),
  KEY `ix_erreserbak_mahaia` (`mahaiak_id`),
  CONSTRAINT `fk_erreserbak_erabil` FOREIGN KEY (`erabiltzaileak_id`) REFERENCES `erabiltzaileak` (`id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `fk_erreserbak_mahaiak` FOREIGN KEY (`mahaiak_id`) REFERENCES `mahaiak` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `eskaera_mahaiak`
--

DROP TABLE IF EXISTS `eskaera_mahaiak`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `eskaera_mahaiak` (
  `id` int NOT NULL AUTO_INCREMENT,
  `eskaera_id` int NOT NULL,
  `mahaia_id` int NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_eskaera_mahaia` (`eskaera_id`,`mahaia_id`),
  KEY `ix_em_mahaia` (`mahaia_id`),
  CONSTRAINT `fk_em_eskaera` FOREIGN KEY (`eskaera_id`) REFERENCES `eskaerak` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_em_mahaia` FOREIGN KEY (`mahaia_id`) REFERENCES `mahaiak` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `eskaera_produktuak`
--

DROP TABLE IF EXISTS `eskaera_produktuak`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `eskaera_produktuak` (
  `id` int NOT NULL AUTO_INCREMENT,
  `eskaera_id` int NOT NULL,
  `produktua_id` int NOT NULL,
  `kantitatea` int NOT NULL DEFAULT '1',
  `prezio_unitarioa` decimal(10,2) NOT NULL,
  `guztira` decimal(10,2) GENERATED ALWAYS AS ((`kantitatea` * `prezio_unitarioa`)) STORED,
  PRIMARY KEY (`id`),
  KEY `ix_ep_eskaera` (`eskaera_id`),
  KEY `ix_ep_produktua` (`produktua_id`),
  CONSTRAINT `fk_ep_eskaera` FOREIGN KEY (`eskaera_id`) REFERENCES `eskaerak` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_ep_produktua` FOREIGN KEY (`produktua_id`) REFERENCES `produktuak` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `eskaerak`
--

DROP TABLE IF EXISTS `eskaerak`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `eskaerak` (
  `id` int NOT NULL AUTO_INCREMENT,
  `mahaia_id` int DEFAULT NULL,
  `erabiltzaile_id` int DEFAULT NULL,
  `komensalak` tinyint DEFAULT NULL,
  `egoera` varchar(50) DEFAULT 'irekita',
  `sukaldea_egoera` varchar(50) DEFAULT 'zain',
  `sortze_data` datetime DEFAULT CURRENT_TIMESTAMP,
  `itxiera_data` datetime DEFAULT NULL,
  `izena` varchar(50) DEFAULT NULL,
  `prezioa` float DEFAULT NULL,
  `data` timestamp NULL DEFAULT NULL,
  `zerbitzua_id` int DEFAULT NULL,
  `produktua_id` int DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `ix_eskaerak_mahaia` (`mahaia_id`),
  KEY `ix_eskaerak_erabiltzaile` (`erabiltzaile_id`),
  KEY `ix_eskaerak_zerbitzua` (`zerbitzua_id`),
  KEY `ix_eskaerak_produktua` (`produktua_id`),
  CONSTRAINT `fk_eskaerak_erabil` FOREIGN KEY (`erabiltzaile_id`) REFERENCES `erabiltzaileak` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_eskaerak_mahaia` FOREIGN KEY (`mahaia_id`) REFERENCES `mahaiak` (`id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `fk_eskaerak_produktua` FOREIGN KEY (`produktua_id`) REFERENCES `produktuak` (`id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `fk_eskaerak_zerbitzua` FOREIGN KEY (`zerbitzua_id`) REFERENCES `zerbitzua` (`id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fakturak`
--

DROP TABLE IF EXISTS `fakturak`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `fakturak` (
  `id` int NOT NULL AUTO_INCREMENT,
  `eskaera_id` int DEFAULT NULL,
  `pdf_izena` varchar(255) DEFAULT NULL,
  `data` datetime DEFAULT CURRENT_TIMESTAMP,
  `guztira` decimal(10,2) DEFAULT NULL,
  `prezio_totala` float DEFAULT NULL,
  `zerbitzua_id` int DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `ix_fakturak_eskaera` (`eskaera_id`),
  KEY `ix_fakturak_zerbitzua` (`zerbitzua_id`),
  CONSTRAINT `fk_fakturak_eskaera` FOREIGN KEY (`eskaera_id`) REFERENCES `eskaerak` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_fakturak_zerbitzua` FOREIGN KEY (`zerbitzua_id`) REFERENCES `zerbitzua` (`id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `kategoriak`
--

DROP TABLE IF EXISTS `kategoriak`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `kategoriak` (
  `id` int NOT NULL AUTO_INCREMENT,
  `izena` varchar(100) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_kategoriak_izena` (`izena`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `langileak`
--

DROP TABLE IF EXISTS `langileak`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `langileak` (
  `id` int NOT NULL AUTO_INCREMENT,
  `izena` varchar(100) DEFAULT NULL,
  `abizena` varchar(100) DEFAULT NULL,
  `erabiltzailea` varchar(100) DEFAULT NULL,
  `pasahitza` varchar(100) DEFAULT NULL,
  `email` varchar(50) DEFAULT NULL,
  `telefonoa` varchar(9) DEFAULT NULL,
  `baimena` tinyint DEFAULT NULL,
  `mahaiak_id` int DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_langileak_mahaiak_idx` (`mahaiak_id`),
  CONSTRAINT `fk_langileak_mahaiak` FOREIGN KEY (`mahaiak_id`) REFERENCES `langileak` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `mahaiak`
--

DROP TABLE IF EXISTS `mahaiak`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `mahaiak` (
  `id` int NOT NULL AUTO_INCREMENT,
  `izena` varchar(45) DEFAULT NULL,
  `kapazitatea` int DEFAULT NULL,
  `egoera` varchar(50) DEFAULT 'libre',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `osagaiak`
--

DROP TABLE IF EXISTS `osagaiak`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `osagaiak` (
  `id` int NOT NULL AUTO_INCREMENT,
  `izena` varchar(150) NOT NULL,
  `unitatea` varchar(20) DEFAULT NULL,
  `stock_aktuala` decimal(10,2) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_osagaiak_izena` (`izena`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `platera_motak`
--

DROP TABLE IF EXISTS `platera_motak`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `platera_motak` (
  `id` int NOT NULL AUTO_INCREMENT,
  `izena` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `platerak`
--

DROP TABLE IF EXISTS `platerak`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `platerak` (
  `id` int NOT NULL AUTO_INCREMENT,
  `izena` varchar(45) DEFAULT NULL,
  `mota` varchar(45) DEFAULT NULL,
  `perezioa` float DEFAULT NULL,
  `platera_motak_id` int NOT NULL,
  PRIMARY KEY (`id`),
  KEY `ix_platerak_mota` (`platera_motak_id`),
  CONSTRAINT `fk_platerak_platera_motak` FOREIGN KEY (`platera_motak_id`) REFERENCES `platera_motak` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `platerak_has_eskaerak`
--

DROP TABLE IF EXISTS `platerak_has_eskaerak`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `platerak_has_eskaerak` (
  `platerak_id` int NOT NULL,
  `eskaerak_id` int NOT NULL,
  PRIMARY KEY (`platerak_id`,`eskaerak_id`),
  KEY `ix_phe_eskaerak` (`eskaerak_id`),
  KEY `ix_phe_platerak` (`platerak_id`),
  CONSTRAINT `fk_phe_eskaerak` FOREIGN KEY (`eskaerak_id`) REFERENCES `eskaerak` (`id`),
  CONSTRAINT `fk_phe_platerak` FOREIGN KEY (`platerak_id`) REFERENCES `platerak` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `platerak_has_kontsumizioa`
--

DROP TABLE IF EXISTS `platerak_has_kontsumizioa`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `platerak_has_kontsumizioa` (
  `platerak_id` int NOT NULL,
  `kontsumizioa_id` int NOT NULL,
  PRIMARY KEY (`platerak_id`,`kontsumizioa_id`),
  KEY `ix_phk_kontsumizioa` (`kontsumizioa_id`),
  CONSTRAINT `fk_phk_kontsumizioa` FOREIGN KEY (`kontsumizioa_id`) REFERENCES `produktuak` (`id`),
  CONSTRAINT `fk_phk_platerak` FOREIGN KEY (`platerak_id`) REFERENCES `platerak` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `produktu_osagaiak`
--

DROP TABLE IF EXISTS `produktu_osagaiak`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `produktu_osagaiak` (
  `produktua_id` int NOT NULL,
  `osagaia_id` int NOT NULL,
  `kantitatea` decimal(10,2) NOT NULL,
  `unitatea` varchar(20) NOT NULL,
  PRIMARY KEY (`produktua_id`,`osagaia_id`),
  KEY `ix_produktu_osagaiak_osagaia` (`osagaia_id`),
  CONSTRAINT `fk_produktu_osagaiak_osagaia` FOREIGN KEY (`osagaia_id`) REFERENCES `osagaiak` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_produktu_osagaiak_produktua` FOREIGN KEY (`produktua_id`) REFERENCES `produktuak` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `produktuak`
--

DROP TABLE IF EXISTS `produktuak`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `produktuak` (
  `id` int NOT NULL AUTO_INCREMENT,
  `izena` varchar(200) NOT NULL,
  `prezioa` decimal(10,2) NOT NULL,
  `stock_min` int DEFAULT NULL,
  `stock_max` int DEFAULT NULL,
  `irudia` varchar(255) DEFAULT NULL,
  `kategoria_id` int DEFAULT NULL,
  `produktuen_motak_id` int DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `ix_produktuak_kategoria` (`kategoria_id`),
  KEY `ix_produktuak_mota` (`produktuen_motak_id`),
  CONSTRAINT `fk_produktuak_kategoria` FOREIGN KEY (`kategoria_id`) REFERENCES `kategoriak` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_produktuak_mota` FOREIGN KEY (`produktuen_motak_id`) REFERENCES `produktuen_motak` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=69 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `produktuak_has_platerak`
--

DROP TABLE IF EXISTS `produktuak_has_platerak`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `produktuak_has_platerak` (
  `produktuak_id` int NOT NULL,
  `platerak_id` int NOT NULL,
  PRIMARY KEY (`produktuak_id`,`platerak_id`),
  KEY `ix_php_platerak` (`platerak_id`),
  CONSTRAINT `fk_php_platerak` FOREIGN KEY (`platerak_id`) REFERENCES `platerak` (`id`),
  CONSTRAINT `fk_php_produktuak` FOREIGN KEY (`produktuak_id`) REFERENCES `produktuak` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `produktuen_eskaerak`
--

DROP TABLE IF EXISTS `produktuen_eskaerak`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `produktuen_eskaerak` (
  `id` int NOT NULL AUTO_INCREMENT,
  `kantitatea` int DEFAULT NULL,
  `kant_max` int DEFAULT NULL,
  `kant_min` int DEFAULT NULL,
  `produktuak_id` int NOT NULL,
  PRIMARY KEY (`id`),
  KEY `ix_prod_eskaerak_prod` (`produktuak_id`),
  CONSTRAINT `fk_prod_eskaerak_prod` FOREIGN KEY (`produktuak_id`) REFERENCES `produktuak` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `produktuen_motak`
--

DROP TABLE IF EXISTS `produktuen_motak`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `produktuen_motak` (
  `id` int NOT NULL AUTO_INCREMENT,
  `izena` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `rolak`
--

DROP TABLE IF EXISTS `rolak`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `rolak` (
  `id` int NOT NULL AUTO_INCREMENT,
  `izena` varchar(50) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `zerbitzua`
--

DROP TABLE IF EXISTS `zerbitzua`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `zerbitzua` (
  `id` int NOT NULL AUTO_INCREMENT,
  `prezioTotala` float DEFAULT NULL,
  `data` timestamp NULL DEFAULT NULL,
  `ordainduta` tinyint DEFAULT NULL,
  `erreserba_id` int DEFAULT NULL,
  `mahaiak_id` int DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `ix_zerbitzua_mahaia` (`mahaiak_id`),
  KEY `ix_zerbitzua_erreserba` (`erreserba_id`),
  CONSTRAINT `fk_zerbitzua_erreserbak` FOREIGN KEY (`erreserba_id`) REFERENCES `erreserbak` (`id`),
  CONSTRAINT `fk_zerbitzua_mahaiak` FOREIGN KEY (`mahaiak_id`) REFERENCES `mahaiak` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-03-26 13:59:07
