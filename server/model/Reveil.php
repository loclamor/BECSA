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
        "lastRing"       => "lastRing"
    );
    
    public $nom;
    public $heure;
    public $jour;
    public $repetition;
    public $lastRing;

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
            return false; //pas de reveil aujourd'hui
        if( $this->getRepetition() == 0 )
            return false; //reveil disable
        
        $heureM = intval( date("H") );
        $minM = intval( date("i") );
        $heureMinR = explode( ":",  $this->heure );
        $heureR = intval( $heureMinR[0] );
        $minR = intval( $heureMinR[1] );
        
        $sonne = ( $heureM == $heureR && $minM == $minR );
        
        //decrementation repetition ?
        if( $sonne ) {
            $dateM = date("Y-m-d");
            $dateR = $this->lastRing;
            if( $dateM != $dateR){
                //premiere fois aujourd'hui que c'est l'heure du reveil et
                //que on check si il sonne : on decremente repetition
                $this->repetition--;
                $this->lastRing = $dateM;
                $this->enregistrer( array("repetition", "lastRing") );
            }
        }
        
        return $sonne;
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
    
    /**
     * retourne la date de la derniere sonnerie
     * @return Date 'YYYY-MM-DD'
     */
    public function getLastRing(){
        return $this->lastRing;
    }
    
    /**
     * Definit la date de la derniere sonnerie
     * Ne devrait pas etre utilisee, la fonction sonne() le fait elle meme pour savoir si il faut decrementer le nombre de repetition
     * @param Date $date 'YYYY-MM-DD'
     */
    public function setLastRing( $date ) {
        $this->date = $date;
    }
    
    public function getState() {
        return array(
            "id" => $this->id,
            "nom" => $this->getNom(),
            "heure" => $this->heure,
            
        );
    }
}

?>
