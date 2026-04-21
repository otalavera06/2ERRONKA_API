-- MySQL dump 10.13  Distrib 8.0.38, for Win64 (x86_64)
--
-- Host: localhost    Database: erronka2026
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
) ENGINE=InnoDB AUTO_INCREMENT=40 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `erreserbak`
--

LOCK TABLES `erreserbak` WRITE;
/*!40000 ALTER TABLE `erreserbak` DISABLE KEYS */;
INSERT INTO `erreserbak` VALUES (38,'2026-04-21 22:00:00',1,NULL,1),(39,'2026-04-20 22:00:00',0,NULL,1);
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
) ENGINE=InnoDB AUTO_INCREMENT=263 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `eskaerak`
--

LOCK TABLES `eskaerak` WRITE;
/*!40000 ALTER TABLE `eskaerak` DISABLE KEYS */;
INSERT INTO `eskaerak` VALUES (259,'Albondigak Saltsan',9.5,'2026-04-21 06:37:06',0,119,1),(260,'Albondigak Saltsan',9.5,'2026-04-21 06:37:41',0,119,1),(261,'Albondigak Saltsan',9.5,'2026-04-21 06:37:50',0,120,1),(262,'Kas Laranja',2,'2026-04-21 06:53:50',0,121,50);
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
) ENGINE=InnoDB AUTO_INCREMENT=45 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `fakturak`
--

LOCK TABLES `fakturak` WRITE;
/*!40000 ALTER TABLE `fakturak` DISABLE KEYS */;
INSERT INTO `fakturak` VALUES (24,4,94),(25,8,95),(26,11.5,96),(27,10,97),(28,2.7,99),(29,8,100),(30,4.2,105),(31,4.2,106),(32,4.2,107),(33,53,113),(34,234.8,114),(35,9.5,115),(37,23.4,117),(39,31,116),(40,4.1,118),(41,26.1,112),(42,10.5,111),(43,19,119),(44,9.5,120);
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
  `chat_baimena` tinyint DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `fk_langileak_mahaiak_idx` (`mahaiak_id`),
  CONSTRAINT `fk_langileak_mahaiak` FOREIGN KEY (`mahaiak_id`) REFERENCES `langileak` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `langileak`
--

LOCK TABLES `langileak` WRITE;
/*!40000 ALTER TABLE `langileak` DISABLE KEYS */;
INSERT INTO `langileak` VALUES (6,'Oier','Talavera','Muñoa','1234','aoao','67674312',1,6,1);
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
  `erabiltzailea` varchar(45) DEFAULT NULL,
  `pasahitza` varchar(45) DEFAULT NULL,
  `chat_baimena` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `mahaiak`
--

LOCK TABLES `mahaiak` WRITE;
/*!40000 ALTER TABLE `mahaiak` DISABLE KEYS */;
INSERT INTO `mahaiak` VALUES (1,'Mahaia 1','m1','123','1'),(2,'Mahaia 2','m2','123','1'),(3,'Mahaia 3','m3','123','1'),(4,'Mahaia 4','m4','123','1'),(5,'Mahaia 5','m5','123','1'),(6,'Barra','b','123','1');
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
  `prezioa` float DEFAULT NULL,
  `argazkia` varchar(1000) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=25 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `platerak`
--

LOCK TABLES `platerak` WRITE;
/*!40000 ALTER TABLE `platerak` DISABLE KEYS */;
INSERT INTO `platerak` VALUES (6,'Entsalada Mistoa','Lehenengo platera',6.5,'entsalada_mistoa.png'),(7,'Arrain Zopa','Lehenengo platera',7.5,'arrain_zopa.png'),(8,'Kalabazin Krema','Lehenengo platera',6,'kalabazin_krema.png'),(9,'Barazki Arroza','Lehenengo platera',7,'barazki_arroza.png'),(10,'Makarronak Tomatearekin','Lehenengo platera',7.2,'makarronak.png'),(11,'Dilista Etxekoak','Lehenengo platera',6.8,'dilistak.png'),(12,'Itsaski Paella','Lehenengo platera',8.5,'paella.png'),(13,'Oilaskoa Pil-Pilean','Bigarren platera',10.5,'oilasko_pilpil.png'),(14,'Legatza Plantxan','Bigarren platera',11,'legatza.png'),(15,'Txerri Txuleta','Bigarren platera',10,'txuleta.png'),(16,'Albondigak Saltsan','Bigarren platera',9.5,'albondigak.png'),(17,'Patata Tortilla','Bigarren platera',8,'tortilla.png'),(18,'Bakailaoa Labean','Bigarren platera',11.5,'bakailaoa.png'),(19,'Entrekota Patatekin','Bigarren platera',13.5,'entrekota.png'),(20,'Gazta Tarta','postreak',5.5,'gazta_tarta.png'),(21,'Flan Etxekoa','postreak',4.5,'flan.png'),(22,'Arroz Esnea','postreak',4.8,'arroz_esnea.png'),(23,'Banilla Izozkia','postreak',4,'izozkia.png'),(24,'Txokolate Brownie','postreak',5.8,'brownie.png');
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
) ENGINE=InnoDB AUTO_INCREMENT=113 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `produktuak`
--

LOCK TABLES `produktuak` WRITE;
/*!40000 ALTER TABLE `produktuak` DISABLE KEYS */;
INSERT INTO `produktuak` VALUES (1,'Platera',0,999,'platera.png',8),(6,'Ardoa',6,0,'ardoa.png',6),(7,'Kafea',2,0,'kafea.png',6),(8,'Estrella',2,2,'estrella.png',6),(49,'Sagardo',3.5,11,'Sagardo.png',6),(50,'Kas Laranja',2,1,'Kas_Laranja.png',6),(51,'Aquarius',2.2,0,'Aquarius.png',6),(52,'Coca-Cola',2.1,11,'Coca-Cola.png',6),(53,'Nestea',2.3,5,'Nestea.png',6),(54,'Fanta Limon',2,0,'Fanta_Limon.png',6),(55,'Agua Bezoya',1.2,0,'Agua_Bezoya.png',6),(56,'Red Bull',2.5,4,'Red_Bull.png',6),(57,'Zumo Laranja',2.8,9,'Zumo_Laranja.png',6),(58,'Kas Limón',2,9,'Kas_Limon.png',6),(59,'Sugus',1.5,13,'Sugus.png',7),(60,'Haribo Gominolas',2,10,'Haribo_Gominolas.png',7),(61,'KitKat',1.2,5,'KitKat.png',7),(62,'Kinder Bueno',1.5,6,'Kinder_Bueno.png',7),(63,'Txupa Txups',0.8,18,'Txupa_Txups.png',7),(64,'Oreo',1.7,2,'Oreo.png',7),(65,'Donuts',1.5,5,'Donuts.png',7),(66,'Milka Txokolatea',2.2,9,'Milka_Txokolatea.png',7),(67,'M&M',1.9,13,'M&M.png',7),(68,'Twix',1.4,4,'Twix.png',7),(69,'Letxuga',1,30,'letxuga.png',8),(70,'Tomatea',0.8,40,'tomatea.png',8),(71,'Tipula',0.6,35,'tipula.png',8),(72,'Olibak',1.2,20,'olibak.png',8),(73,'Arraina',3.5,20,'arraina.png',8),(74,'Salda',1,25,'salda.png',8),(75,'Porrua',0.9,20,'porrua.png',8),(76,'Azenarioa',0.7,30,'azenarioa.png',8),(77,'Kalabazina',1.2,25,'kalabazina.png',8),(78,'Patata',0.6,50,'patata.png',8),(79,'Esnegaina',1,20,'esnegaina.png',8),(80,'Arroza',1,50,'arroza.png',8),(81,'Piperra',0.9,30,'piperra.png',8),(82,'Ilarra',1.1,20,'ilarra.png',8),(83,'Makarronak',1.2,40,'makarronak.png',8),(84,'Tomate Saltsa',1,30,'tomate_saltsa.png',8),(85,'Gazta Birrindua',1.5,20,'gazta.png',8),(86,'Dilistak',1.3,35,'dilistak.png',8),(87,'Txorizoa',2,20,'txorizoa.png',8),(88,'Itsaskia',4,20,'itsaskia.png',8),(89,'Azafraia',1.5,15,'azafraia.png',8),(90,'Oilaskoa',3.5,25,'oilaskoa.png',8),(91,'Baratxuria',0.5,40,'baratxuria.png',8),(92,'Piper Mina',0.4,25,'piper_mina.png',8),(93,'Oliba Olioa',1.2,30,'olioa.png',8),(94,'Legatza',4.2,20,'legatza.png',8),(95,'Limoia',0.6,25,'limoia.png',8),(96,'Txerri Haragia',3.8,20,'txerri.png',8),(97,'Haragi xehatua',3,20,'haragi.png',8),(98,'Ogi birrindua',0.8,20,'ogia.png',8),(99,'Arrautza',0.4,0,'arrautza.png',8),(100,'Tomate Saltsa',1.1,25,'saltsa.png',8),(101,'Arrautzak',1.2,40,'arrautzak.png',8),(102,'Bakailaoa',4.5,20,'bakailaoa.png',8),(103,'Entrekota',5.5,15,'entrekota.png',8),(104,'Gazta Krema',2,20,'gazta_krema.png',8),(105,'Gailetak',1,25,'gailetak.png',8),(106,'Gurina',1.2,20,'gurina.png',8),(107,'Azukrea',0.5,50,'azukrea.png',8),(108,'Esnea',0.9,30,'esnea.png',8),(109,'Karamelua',0.7,20,'karamelua.png',8),(110,'Kanela',0.5,20,'kanela.png',8),(111,'Banilla',1,15,'banilla.png',8),(112,'Txokolatea',1.5,25,'txokolatea.png',8);
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
INSERT INTO `produktuak_has_platerak` VALUES (69,6),(70,6),(71,6),(72,6),(73,7),(74,7),(75,7),(76,7),(77,8),(78,8),(79,8),(80,9),(81,9),(82,9),(83,10),(84,10),(85,10),(86,11),(87,11),(80,12),(88,12),(89,12),(90,13),(91,13),(92,13),(93,13),(93,14),(94,14),(95,14),(93,15),(96,15),(97,16),(98,16),(99,16),(100,16),(78,17),(93,17),(101,17),(93,18),(95,18),(102,18),(78,19),(93,19),(103,19),(104,20),(105,20),(106,20),(107,20),(99,21),(107,21),(108,21),(109,21),(80,22),(107,22),(108,22),(110,22),(107,23),(108,23),(111,23),(99,24),(106,24),(107,24),(112,24);
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
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `produktuen_motak`
--

LOCK TABLES `produktuen_motak` WRITE;
/*!40000 ALTER TABLE `produktuen_motak` DISABLE KEYS */;
INSERT INTO `produktuen_motak` VALUES (6,'edariak'),(7,'txutxeriak'),(8,'osagaia');
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
) ENGINE=InnoDB AUTO_INCREMENT=122 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `zerbitzua`
--

LOCK TABLES `zerbitzua` WRITE;
/*!40000 ALTER TABLE `zerbitzua` DISABLE KEYS */;
INSERT INTO `zerbitzua` VALUES (85,4,'2026-01-21 13:10:14',1,NULL,6),(86,4,'2026-01-21 13:17:17',1,NULL,6),(87,8,'2026-01-21 13:26:27',1,NULL,6),(88,8,'2026-01-21 14:20:53',1,NULL,6),(89,8,'2026-01-21 15:02:39',1,NULL,6),(90,4,'2026-01-21 15:08:30',1,NULL,6),(91,8,'2026-01-21 15:26:52',1,NULL,6),(92,8,'2026-01-22 15:41:13',1,NULL,6),(93,8,'2026-01-26 12:14:20',1,NULL,6),(94,4,'2026-01-28 17:31:37',1,NULL,6),(95,8,'2026-01-28 18:12:37',1,NULL,6),(96,11.5,'2026-01-28 19:27:23',1,NULL,6),(97,10,'2026-01-29 21:27:03',1,NULL,6),(99,2.7,'2026-03-25 13:18:42',1,NULL,6),(100,8,'2026-04-16 06:33:11',1,NULL,6),(101,8,'2026-04-16 07:12:42',1,NULL,6),(105,4.2,'2026-04-16 14:30:14',1,NULL,6),(106,4.2,'2026-04-17 11:57:46',1,NULL,6),(107,4.2,'2026-04-17 12:28:49',1,NULL,6),(111,10.5,'2025-05-17 10:00:00',1,NULL,3),(112,26.1,'2025-05-17 10:00:00',1,NULL,2),(113,53,'2026-04-17 14:15:02',1,NULL,1),(114,234.8,'2026-04-17 14:19:45',1,NULL,1),(115,9.5,'2026-04-17 14:59:08',1,NULL,1),(116,31,'2026-04-20 07:23:37',1,NULL,1),(117,23.4,'2026-04-20 09:23:48',1,NULL,6),(118,4.1,'2026-04-20 14:07:53',1,NULL,1),(119,19,'2026-04-21 06:37:06',1,NULL,6),(120,9.5,'2026-04-21 06:37:50',1,NULL,6),(121,2,'2026-04-21 06:53:50',0,NULL,6);
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

-- Dump completed on 2026-04-21  8:58:28
