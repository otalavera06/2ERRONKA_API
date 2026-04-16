-- MySQL dump 10.13  Distrib 8.0.38, for Win64 (x86_64)
--
-- Host: localhost    Database: erronka2026_definitibo
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
  `izena` varchar(45) DEFAULT NULL,
  `abizena` varchar(45) DEFAULT NULL,
  `email` varchar(45) DEFAULT NULL,
  `pasahitza` varchar(255) DEFAULT NULL,
  `telefonoa` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `erabiltzaileak`
--

LOCK TABLES `erabiltzaileak` WRITE;
/*!40000 ALTER TABLE `erabiltzaileak` DISABLE KEYS */;
INSERT INTO `erabiltzaileak` VALUES (4,'Barra','TPV','barra@sushineli',NULL,NULL);
/*!40000 ALTER TABLE `erabiltzaileak` ENABLE KEYS */;
UNLOCK TABLES;

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
  KEY `fk_erreserbak_erabiltzaileak1_idx` (`erabiltzaileak_id`),
  KEY `fk_erreserbak_mahaiak1_idx` (`mahaiak_id`),
  CONSTRAINT `fk_erreserbak_erabiltzaileak1` FOREIGN KEY (`erabiltzaileak_id`) REFERENCES `erabiltzaileak` (`id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `fk_erreserbak_mahaiak1` FOREIGN KEY (`mahaiak_id`) REFERENCES `mahaiak` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=28 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `erreserbak`
--

LOCK TABLES `erreserbak` WRITE;
/*!40000 ALTER TABLE `erreserbak` DISABLE KEYS */;
INSERT INTO `erreserbak` VALUES (20,'2026-05-06 22:00:00',0,4,5),(21,'2026-05-06 22:00:00',1,4,3),(23,'2026-01-28 23:00:00',0,4,5),(25,'2026-01-28 23:00:00',0,4,5),(27,'2026-03-25 23:00:00',1,4,4);
/*!40000 ALTER TABLE `erreserbak` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `eskaerak`
--

DROP TABLE IF EXISTS `eskaerak`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `eskaerak` (
  `id` int NOT NULL AUTO_INCREMENT,
  `izena` varchar(50) DEFAULT NULL,
  `prezioa` float DEFAULT NULL,
  `data` timestamp NULL DEFAULT NULL,
  `egoera` tinyint DEFAULT NULL,
  `zerbitzua_id` int DEFAULT NULL,
  `produktua_id` int NOT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_eskaerak_zerbitzua1_idx` (`zerbitzua_id`),
  KEY `fk_eskaerak_produktua_idx` (`produktua_id`),
  CONSTRAINT `fk_eskaerak_produktua` FOREIGN KEY (`produktua_id`) REFERENCES `produktuak` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=165 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `eskaerak`
--

LOCK TABLES `eskaerak` WRITE;
/*!40000 ALTER TABLE `eskaerak` DISABLE KEYS */;
INSERT INTO `eskaerak` VALUES (133,'Kas Laranja',2,'2026-01-21 13:10:14',1,85,50),(134,'Fanta Limon',2,'2026-01-21 13:10:14',1,85,54),(135,'Kas Laranja',2,'2026-01-21 13:17:17',1,86,50),(136,'Fanta Limon',2,'2026-01-21 13:17:17',1,86,54),(137,'Ardoa',6,'2026-01-21 13:26:27',1,87,6),(138,'Kas Laranja',2,'2026-01-21 13:26:27',1,87,50),(139,'Ardoa',6,'2026-01-21 14:20:53',1,88,6),(140,'Kas Laranja',2,'2026-01-21 14:20:53',1,88,50),(141,'Ardoa',6,'2026-01-21 15:02:39',1,89,6),(142,'Kas Laranja',2,'2026-01-21 15:02:39',1,89,50),(143,'Kas Laranja',2,'2026-01-21 15:08:30',1,90,50),(144,'Fanta Limon',2,'2026-01-21 15:08:30',1,90,54),(145,'Ardoa',6,'2026-01-21 15:26:52',1,91,6),(146,'Kas Laranja',2,'2026-01-21 15:26:52',1,91,50),(147,'Ardoa',6,'2026-01-22 15:41:13',1,92,6),(148,'Kas Laranja',2,'2026-01-22 15:41:13',1,92,50),(149,'Ardoa',6,'2026-01-26 12:14:20',1,93,6),(150,'Kas Laranja',2,'2026-01-26 12:14:20',1,93,50),(151,'Kas Laranja',2,'2026-01-28 17:31:37',1,94,50),(152,'Fanta Limon',2,'2026-01-28 17:31:37',1,94,54),(153,'Ardoa',6,'2026-01-28 18:12:37',1,95,6),(154,'Kas Laranja',2,'2026-01-28 18:12:37',1,95,50),(155,'Sagardo',3.5,'2026-01-28 19:27:23',1,96,49),(156,'Kas Limón',2,'2026-01-28 19:27:23',1,96,58),(157,'Ardoa',6,'2026-01-28 19:27:23',1,96,6),(158,'Fanta Limon',2,'2026-01-29 21:27:03',1,97,54),(159,'Fanta Limon',2,'2026-01-29 21:27:03',1,97,54),(160,'Fanta Limon',2,'2026-01-29 21:27:03',1,97,54),(161,'Fanta Limon',2,'2026-01-29 21:27:03',1,97,54),(162,'Estrella',2,'2026-01-29 21:27:03',1,97,8),(163,'Agua Bezoya',1.2,'2026-03-25 13:18:42',1,99,55),(164,'Sugus',1.5,'2026-03-25 13:18:42',1,99,59);
/*!40000 ALTER TABLE `eskaerak` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `fakturak`
--

DROP TABLE IF EXISTS `fakturak`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `fakturak` (
  `id` int NOT NULL AUTO_INCREMENT,
  `prezio_totala` float DEFAULT NULL,
  `zerbitzua_id` int DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_faktura_zerbitzua_idx` (`zerbitzua_id`),
  CONSTRAINT `fk_faktura_zerbitzua` FOREIGN KEY (`zerbitzua_id`) REFERENCES `zerbitzua` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=29 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `fakturak`
--

LOCK TABLES `fakturak` WRITE;
/*!40000 ALTER TABLE `fakturak` DISABLE KEYS */;
INSERT INTO `fakturak` VALUES (24,4,94),(25,8,95),(26,11.5,96),(27,10,97),(28,2.7,99);
/*!40000 ALTER TABLE `fakturak` ENABLE KEYS */;
UNLOCK TABLES;

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
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `langileak`
--

LOCK TABLES `langileak` WRITE;
/*!40000 ALTER TABLE `langileak` DISABLE KEYS */;
INSERT INTO `langileak` VALUES (6,'Oier','Talavera','Muñoa','1234','munoatala@shushinelli.com','888665533',1,6),(7,'Iraitz','Guisado','Guisi','567','guisado@shushinelli.com','554477332',0,NULL);
/*!40000 ALTER TABLE `langileak` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `mahaiak`
--

DROP TABLE IF EXISTS `mahaiak`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `mahaiak` (
  `id` int NOT NULL AUTO_INCREMENT,
  `izena` varchar(45) DEFAULT NULL,
  `egoera` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `mahaiak`
--

LOCK TABLES `mahaiak` WRITE;
/*!40000 ALTER TABLE `mahaiak` DISABLE KEYS */;
INSERT INTO `mahaiak` VALUES (1,'Mahaia 1',NULL),(2,'Mahaia 2',NULL),(3,'Mahaia 3',NULL),(4,'Mahaia 4',NULL),(5,'Mahaia 5',NULL),(6,'Barra',NULL);
/*!40000 ALTER TABLE `mahaiak` ENABLE KEYS */;
UNLOCK TABLES;

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
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `platerak`
--

LOCK TABLES `platerak` WRITE;
/*!40000 ALTER TABLE `platerak` DISABLE KEYS */;
/*!40000 ALTER TABLE `platerak` ENABLE KEYS */;
UNLOCK TABLES;

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
  KEY `fk_platerak_has_eskaerak_eskaerak1_idx` (`eskaerak_id`),
  KEY `fk_platerak_has_eskaerak_platerak1_idx` (`platerak_id`),
  CONSTRAINT `fk_platerak_has_eskaerak_eskaerak1` FOREIGN KEY (`eskaerak_id`) REFERENCES `eskaerak` (`id`),
  CONSTRAINT `fk_platerak_has_eskaerak_platerak1` FOREIGN KEY (`platerak_id`) REFERENCES `platerak` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `platerak_has_eskaerak`
--

LOCK TABLES `platerak_has_eskaerak` WRITE;
/*!40000 ALTER TABLE `platerak_has_eskaerak` DISABLE KEYS */;
/*!40000 ALTER TABLE `platerak_has_eskaerak` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `produktuak`
--

DROP TABLE IF EXISTS `produktuak`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `produktuak` (
  `id` int NOT NULL AUTO_INCREMENT,
  `izena` varchar(50) DEFAULT NULL,
  `prezioa` float DEFAULT NULL,
  `stock` int DEFAULT NULL,
  `irudia` varchar(255) DEFAULT NULL,
  `produktuen_motak_id` int NOT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_produktuak_produktuen_motak1_idx` (`produktuen_motak_id`),
  CONSTRAINT `fk_produktuak_produktuen_motak1` FOREIGN KEY (`produktuen_motak_id`) REFERENCES `produktuen_motak` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=69 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `produktuak`
--

LOCK TABLES `produktuak` WRITE;
/*!40000 ALTER TABLE `produktuak` DISABLE KEYS */;
INSERT INTO `produktuak` VALUES (6,'Ardoa',6,2,'ardoa.png',6),(7,'Kafea',2,5,'kafea.png',6),(8,'Estrella',2,6,'estrella.png',6),(49,'Sagardo',3.5,11,'Sagardo.png',6),(50,'Kas Laranja',2,6,'Kas_Laranja.png',6),(51,'Aquarius',2.2,9,'Aquarius.png',6),(52,'Coca-Cola',2.1,15,'Coca-Cola.png',6),(53,'Nestea',2.3,7,'Nestea.png',6),(54,'Fanta Limon',2,0,'Fanta_Limon.png',6),(55,'Agua Bezoya',1.2,10,'Agua_Bezoya.png',6),(56,'Red Bull',2.5,5,'Red_Bull.png',6),(57,'Zumo Laranja',2.8,9,'Zumo_Laranja.png',6),(58,'Kas Limón',2,10,'Kas_Limon.png',6),(59,'Sugus',1.5,13,'Sugus.png',7),(60,'Haribo Gominolas',2,10,'Haribo_Gominolas.png',7),(61,'KitKat',1.2,6,'KitKat.png',7),(62,'Kinder Bueno',1.5,8,'Kinder_Bueno.png',7),(63,'Txupa Txups',0.8,18,'Txupa_Txups.png',7),(64,'Oreo',1.7,7,'Oreo.png',7),(65,'Donuts',1.5,5,'Donuts.png',7),(66,'Milka Txokolatea',2.2,9,'Milka_Txokolatea.png',7),(67,'M&M',1.9,13,'M&M.png',7),(68,'Twix',1.4,4,'Twix.png',7);
/*!40000 ALTER TABLE `produktuak` ENABLE KEYS */;
UNLOCK TABLES;

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
  KEY `fk_produktuak_has_platerak_platerak1_idx` (`platerak_id`),
  KEY `fk_produktuak_has_platerak_produktuak1_idx` (`produktuak_id`),
  CONSTRAINT `fk_produktuak_has_platerak_platerak1` FOREIGN KEY (`platerak_id`) REFERENCES `platerak` (`id`),
  CONSTRAINT `fk_produktuak_has_platerak_produktuak1` FOREIGN KEY (`produktuak_id`) REFERENCES `produktuak` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `produktuak_has_platerak`
--

LOCK TABLES `produktuak_has_platerak` WRITE;
/*!40000 ALTER TABLE `produktuak_has_platerak` DISABLE KEYS */;
/*!40000 ALTER TABLE `produktuak_has_platerak` ENABLE KEYS */;
UNLOCK TABLES;

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
-- Dumping data for table `produktuen_motak`
--

LOCK TABLES `produktuen_motak` WRITE;
/*!40000 ALTER TABLE `produktuen_motak` DISABLE KEYS */;
INSERT INTO `produktuen_motak` VALUES (6,'edariak'),(7,'txutxeriak');
/*!40000 ALTER TABLE `produktuen_motak` ENABLE KEYS */;
UNLOCK TABLES;

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
  KEY `fk_zerbitzua_mahaiak1_idx` (`mahaiak_id`),
  KEY `fk_zerbitzua_erreserbak_idx` (`erreserba_id`),
  CONSTRAINT `fk_zerbitzua_erreserbak` FOREIGN KEY (`erreserba_id`) REFERENCES `erreserbak` (`id`),
  CONSTRAINT `fk_zerbitzua_mahaiak1` FOREIGN KEY (`mahaiak_id`) REFERENCES `mahaiak` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=100 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `zerbitzua`
--

LOCK TABLES `zerbitzua` WRITE;
/*!40000 ALTER TABLE `zerbitzua` DISABLE KEYS */;
INSERT INTO `zerbitzua` VALUES (85,4,'2026-01-21 13:10:14',1,NULL,6),(86,4,'2026-01-21 13:17:17',1,NULL,6),(87,8,'2026-01-21 13:26:27',1,NULL,6),(88,8,'2026-01-21 14:20:53',1,NULL,6),(89,8,'2026-01-21 15:02:39',1,NULL,6),(90,4,'2026-01-21 15:08:30',1,NULL,6),(91,8,'2026-01-21 15:26:52',1,NULL,6),(92,8,'2026-01-22 15:41:13',1,NULL,6),(93,8,'2026-01-26 12:14:20',1,NULL,6),(94,4,'2026-01-28 17:31:37',1,NULL,6),(95,8,'2026-01-28 18:12:37',1,NULL,6),(96,11.5,'2026-01-28 19:27:23',1,NULL,6),(97,10,'2026-01-29 21:27:03',1,NULL,6),(99,2.7,'2026-03-25 13:18:42',1,NULL,6);
/*!40000 ALTER TABLE `zerbitzua` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-04-15 16:16:28
