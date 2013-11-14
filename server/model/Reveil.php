<?php
/**
 * Description of Model_Reveil
 *
 * @author Loïc
 */
class Model_Reveil extends Entite {
    
    public $DB_table = 'reveil';
    public $DB_equiv = array(
        "id"             => "id",
        "nom"            => "nom",
        "heure"          => "heure",
        "jour"           => "jour",
        "repetition"     => "repetition",
    );
    
    public $nom;
    public $heure;
    public $jour;
    public $repetition;

    /**
     * Retourne le nom du reveil
     * @return String nom
     */
    public function getNom(){
        return $this->nom;
    }
    
    /**
     * Definit le nom du reveil
     * @param String $nom
     */
    public function setNom( $nom ) {
        $this->nom = $nom;
    }
    
    /**
     * retourne l'heure du reveil 
     * @return Time heure HH:MM:SS
     */
    public function getHeure(){
        return $this->heure;
    }
    
    /**
     * Definit l'heure du reveil
     * @param Time $heure HH:MM:SS
     */
    public function setHeure( $heure ) {
        $this->heure = $heure;
    }
    
    /**
     * Retourne les jours où sonne le reveil
     * @return String jours "X,X,X,X,X,X,X" où X dans [0,1]
     */
    public function getJours(){
        return $this->jour;
    }
    
    /**
     * Le reveil sonne t il le jour numero $jour ?
     * @param Int $jour in [0..6]
     * @return boolean
     */
    public function sonneJour( $jour ) {
        $jours = explode(",", $this->jour);
        return intval($jours[$jour]) == 1;
    }
    
    /**
     * Retourne true si le reveil sonne actuellement
     * @return boolean
     */
    public function sonne(){
        $numJour = intval( date("N") );
        $numJour--;
        if( !$this->sonneJour( $numJour ) )
            return false;
        $heureM = intval( date("H") );
        $minM = intval( date("i") );
        $heureMinR = explode( ":",  $this->heure );
        $heureR = intval( $heureMinR[0] );
        $minR = intval( $heureMinR[1] );
        
        return ( $heureM == $heureR && $minM == $minR );
    }
    
    /**
     * Definit si le reveil sonne le jour numero $jour ?
     * @param Int $jour in [0..6]
     * @param Boolean $status sonnerie active ? true/false
     */
    public function setSonneJour( $jour, $status = true ) {
        $jours = explode(",", $this->jour);
        $jours[$jour] = ( $status === true ? "1" : "0" );
        $this->setJours( implode(",", $jours) );
    }
    
    /**
     * Definit les jours où sonne le reveil
     * @return String jours "X,X,X,X,X,X,X" où X dans [0,1]
     */
    public function setJours( $jours ) {
        $this->jour = $jours;
    }
    
    /**
     * Retourne le nombre de foi où le reveil doit encore sonner
     * @return Int repetition restante : 0=disable; -1=infinite
     */
    public function getRepetition(){
        return $this->repetition;
    }
    
    /**
     * Définit le nombre de fois ou le reveil doit encore sonner
     * @param Int $repetition repetition restante : 0=disable; -1=infinite
     */
    public function setRepetition( $repetition ) {
        $this->repetition = $repetition;
    }
}

?>
