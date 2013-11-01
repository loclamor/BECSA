<?php

/**
 * Description of Model_User
 *
 * @author Loïc
 */
class Model_Piece extends Entite {
    
    public $DB_table = 'piece';
    public $DB_equiv = array(
        "id"             => "id",
        "nom"            => "nom",
        "aLumiere"       => "aLumiere",
        "lumiereAllumee" => "lumiereAllumee",
        "aVolet"         => "aVolet",
        "voletOuvert"    => "voletOuvert",
        "aPorte"         => "aPorte",
        "porteVerrouillee"    => "porteVerrouillee"
    );
    
    public $nom;
    public $aLumiere;
    public $lumiereAllumee;
    public $aVolet;
    public $voletOuvert;
    public $aPorte;
    public $porteVerrouillee;
    
    public function getNom() {
        return $this->nom;
    }
    
    public function aLumiere() {
        return ( $this->aLumiere == 1 );
    }
    
    public function lumiereAllumee() {
        return ( $this->lumiereAllumee == 1 );
    }
    
    public function aVolet() {
        return ( $this->aVolet == 1 );
    }
    
    public function voletOuvert() {
        return ( $this->voletOuvert == 1 );
    }
    
    public function aPorte() {
        return ( $this->aPorte == 1 );
    }
    
    public function porteVerrouillee() {
        return ( $this->porteVerrouillee == 1 );
    }
    
    public function setNom( $nom ) {
        $this->nom = $nom;
    }
    
    public function setALumiere( $status ) {
        $this->aLumiere = ( $status?1:0 );
    }
    
    public function setLumiereAllumee() {
        $this->lumiereAllumee = true;
    }
    
    public function setLumiereEteinte() {
        $this->lumiereAllumee = false;
    }
    
    public function setAVolet( $status ) {
        $this->aVolet = ( $status?1:0 );
    }
    
    public function setVoletOuvert() {
        $this->voletOuvert = true;
    }
    
    public function setVoletFerme() {
        $this->voletOuvert = false;
    }
    
    public function setAPorte( $status ) {
        $this->aPorte = ( $status?1:0 );
    }
    
    public function setPorteVerrouillee() {
        $this->porteVerrouillee = true;
    }
    
    public function setPorteDeverrouillee() {
        $this->porteVerrouillee = false;
    }
    
    public function getState() {
        return array(
            "id" => $this->id,
            "nom" => $this->getNom(),
            "aLumiere" => $this->aLumiere(),
            "lumiereAllumee" => $this->lumiereAllumee(),
            "aVolet" => $this->aVolet(),
            "voletOuvert" => $this->voletOuvert(),
            "aPorte" => $this->aPorte(),
            "porteVerrouillee" => $this->porteVerrouillee()
        );
    }
}