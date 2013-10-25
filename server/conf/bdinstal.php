<?php

$link = mysqli_connect(MYSQL_SERVER,  MYSQL_USER, MYSQL_PWD);
mysqli_select_db($link, MYSQL_DB);

$requete = "CREATE TABLE IF NOT EXISTS `".TABLE_PREFIX."piece`(
	`id` int(11) NOT NULL AUTO_INCREMENT,
	`nom` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
	`aLumiere` TINYINT NOT NULL DEFAULT '1',
	`lumiereAllumee` TINYINT NOT NULL DEFAULT '0',
	`aVolet` TINYINT NOT NULL DEFAULT '1',
	`voletOuvert` TINYINT NOT NULL DEFAULT '1',
	PRIMARY KEY (`id`)
)";
mysqli_query($link, $requete);

mysqli_close($link);