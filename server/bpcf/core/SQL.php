<?php
class SQL {
	/**
	 * @var SQL
	 */
	private static $instance = null;
	private $last_sql_query = null;
	private $last_sql_error = null;
	private $nb_query = 0;
	private $nb_adm_query = 0;
	private $nb_sql_errors = 0;
	
	private $log;
	
	public static function getInstance() {
		if(is_null(self::$instance)) {
		self::$instance = new SQL();
		}
		return self::$instance;
	}
	
	public function __construct(){
		$this->log = new Logger('./logs');
		$this->log->setBaseString('SQL : ');
	}
	/**
	 * 
	 * @param string $requete
	 * @return array or false if no result
	 */
	public function exec($requete){
		$this->setLastQuery($requete);
		$this->nb_query++;
		//on fait la connexion � mysqli
		$link = mysqli_connect(MYSQL_SERVER,  MYSQL_USER, MYSQL_PWD);
		mysqli_select_db($link, MYSQL_DB);
		
		$this->setLastError();
		//on fait la requete
		$rep = mysqli_query($link, $requete);
		//debug($rep);
		$this->setLastError(mysqli_error($link));
		if($this->last_sql_error != ''){
			//echo $this->last_sql_error;
			$this->nb_sql_errors++;
			$this->log->log('sql', 'erreurs_sql', $this->getLastQuery() . ' : ' . $this->last_sql_error, Logger::GRAN_MONTH);
		}
		
		$row = false;
		if(strtoupper(substr($requete, 0, 6)) == 'SELECT') {
			if(!is_null($rep) && !empty($rep)) {
			/*	if(mysqli_num_rows($rep) > 1) {
					while($res = mysqli_fetch_assoc($rep)){
						$row[] = $res;
					}
				}
				else {
					$row = mysqli_fetch_assoc($rep);
				}
				*/
				while($res = mysqli_fetch_assoc($rep)){
					$row[] = $res;
				}
			}
		}
		elseif(strtoupper(substr($requete, 0, 6)) == 'INSERT') {
			$row = mysqli_insert_id($link);
		}

		//on se d�connecte
		mysqli_close($link);
		//on retourne le tableau de résultat
		return $row;
	}
	
	//execution de requete SQL reserve � l'administration
	public function exec2($requete){
		$this->setLastQuery($requete);
		$this->nb_adm_query++;
		
		//on fait la connexion à mysqli
		$link = mysqli_connect(MYSQL_SERVER,  MYSQL_USER, MYSQL_PWD);
		mysqli_select_db($link, MYSQL_DB);
		
		$this->setLastError();
		//on fait la requete
		$rep = mysqli_query($link, $requete);
		//debug($rep);
		$this->setLastError(mysqli_error($link));
		
		if($this->last_sql_error != ''){
			//echo $this->last_sql_error;
			$this->nb_sql_errors++;
			$this->log->log('sql', 'erreurs_sql', $this->getLastQuery() . ' : ' . $this->last_sql_error, Logger::GRAN_MONTH);
		}
		
		$row = false;
		if(strtoupper(substr($requete, 0, 6)) == 'SELECT') {
			if(!is_null($rep) && !empty($rep)) {
				if(mysqli_num_rows($rep) > 1) {
					while($res = mysqli_fetch_assoc($rep)){
						$row[] = $res;
					}
				}
				else {
					$row = mysqli_fetch_assoc($rep);
				}
			}
		}
		elseif(strtoupper(substr($requete, 0, 6)) == 'INSERT') {
			$row = mysqli_insert_id($link);
		}

		//on se déconnecte
		mysqli_close($link);
		//on retourne le tableau de résultat
		return $row;
	}
	
	public function setLastError($err = ''){
		$this->last_sql_error = $err;
	}
	
	public function getLastError() {
		return $this->last_sql_error;
	}
	
	public function setLastQuery($q = null){
		$this->last_sql_query = $q;
	}
	
	public function getLastQuery() {
		return $this->last_sql_query;
	}
	
	public function getNbQuery() {
		return $this->nb_query;
	}
	
	public function getNbAdmQuery() {
		return $this->nb_adm_query;
	}
	
	public function getNbErrors() {
		return $this->nb_sql_errors;
	}
}