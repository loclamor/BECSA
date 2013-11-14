<?php

$link = mysqli_connect(MYSQL_SERVER,  MYSQL_USER, MYSQL_PWD);
mysqli_select_db($link, MYSQL_DB);

/**
 * v0.0.1 creation table piece
 */
$requete = "CREATE TABLE IF NOT EXISTS `".TABLE_PREFIX."piece`(
	`id` int(11) NOT NULL AUTO_INCREMENT,
	`nom` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
	`aLumiere` TINYINT NOT NULL DEFAULT '1',
	`lumiereAllumee` TINYINT NOT NULL DEFAULT '0',
	`aVolet` TINYINT NOT NULL DEFAULT '1',
	`voletOuvert` TINYINT NOT NULL DEFAULT '1',
	PRIMARY KEY (`id`)
)";
$res = mysqli_query($link, $requete);
if( $res === false ) {
    echo mysqli_error( $link );
}

/**
 * v0.0.2 ajout de la notion de porte a la table piece
 */
$requete = "ALTER TABLE  `".TABLE_PREFIX."piece` 
    ADD  `aPorte` TINYINT NOT NULL DEFAULT  '0',
    ADD  `porteVerrouillee` TINYINT NOT NULL DEFAULT  '1'";
$res = mysqli_query($link, $requete);
if( $res === false ) {
    echo mysqli_error( $link );
}

/**
 * v0.0.3 creation table reveil
 */
$requete = "CREATE TABLE IF NOT EXISTS `".TABLE_PREFIX."reveil`(
	`id` int(11) NOT NULL AUTO_INCREMENT,
	`nom` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
	`heure` TIME NOT NULL,
	`jour` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '0,0,0,0,0,0,0',
	`repetition` INT NOT NULL DEFAULT '1',
	PRIMARY KEY (`id`)
)";
$res = mysqli_query($link, $requete);
if( $res === false ) {
    echo mysqli_error( $link );
}

/**
 * v0.0.4 ajout de la notion lastRing a la table piece
 */
$requete = "ALTER TABLE  `".TABLE_PREFIX."reveil` 
    ADD  `lastRing` Date DEFAULT '1000-01-01'";
$res = mysqli_query($link, $requete);
if( $res === false ) {
    echo mysqli_error( $link );
}

mysqli_close($link);