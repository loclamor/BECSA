<?php
session_start();

//nom du site
define('SITE_NAME', 'CSA');

//version du site (utile en particulier pour automatiser la mise � jour de la BDD
define('VERSION','0.0.1');

//config pour savoir si on est en local ou pas
if($_SERVER['SERVER_ADDR'] == '127.0.0.1')  {
	define('APPLICATION_ENV', 'dev');
}
else {
	define('APPLICATION_ENV','prod');
}

define('SITE','site'); //c'est quoi ça deja ?

//pr�fixe des tables de la base de donn�e
define('TABLE_PREFIX','CSA_');

//definition des variables de conf
if(APPLICATION_ENV == 'dev') {//on est en local
//mysql
	define('MYSQL_SERVER','localhost');
	define('MYSQL_USER','root');
	define('MYSQL_PWD','');
	define('MYSQL_DB','csa');
	
	define('FORCE_DEBUG', TRUE);
	define('HOST_OF_SITE', 'http://localhost/workspace-php/BECSA');
       
        define('BPCF', './bpcf');
}
else {//on est en prod
	//mysql
	//this file have to define MYSQL_SERVER, MYSQL_USER, MYSQL_PWD, MYSQL_DB like on APPLICATION_ENV == 'dev'
	require_once 'conf/dbpass.php';
	 
	define('FORCE_DEBUG', FALSE);
	define('HOST_OF_SITE', 'http://'.$_SERVER['SERVER_NAME']);
        
        define('BPCF', 'bpcf');
}

define('DEFAULT_CONTROLLER', 'default');
define('DEFAULT_ACTION', 'index');

define('PLUGINS_FOLDER', 'plugin/');

require_once BPCF.'/conf.php';

//script de mise à jour de la BDD en fonction de la version
$versionFile = 'conf/'.VERSION.'.version';
if(!file_exists( $versionFile )){
	require_once 'conf/bdinstal.php';
	if ($fp = fopen( $versionFile ,"w")){
		fclose($fp);
	}
	else {
		die("erreur de changement de version vers ".VERSION);
	}
}


