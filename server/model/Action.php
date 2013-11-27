<?php

/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Description of Action
 *
 * @author Loïc
 */
class Model_Action extends Entite {
    
    public $DB_table = 'action';
    public $DB_equiv = array(
        "id"           => "id",
        "action"       => "action",
        "envoie"       => "envoie",
        "destinataire" => "destinataire",
        "params" => "params"
    );
    
    public function getAction() {
        return $this->action;
    }
    
    public function setAction( $action ) {
        $this->action = $action;
    }
    
    public function getEnvoie() {
        return $this->envoie;
    }
    
    public function setEnvoie( $envoie ) {
        $this->envoie = $envoie;
    }
    
    public function getDestinataire() {
        return $this->destinataire;
    }
    
    public function setDestinataire( $dest ) {
        $this->destinataire = $dest;
    }
    
    public function getParams(){
        return explode( ";", $this->params );
    }
    
    public function setParams( array $params ) {
        $this->params = implode( ";", $params );
    }
    
    public function addParam( $param ) {
        $temp = $this->getParams();
        $temp[] = $param;
        $this->setParams( $temp );
    }
    
    public function getState(){
        return Array(
            "id" => $this->id,
            "action" => $this->getAction(),
            "envoie" => $this->getEnvoie(),
            "dest" => $this->getDestinataire(),
            "params" => $this->getParams()
        );
    }
}

?>
