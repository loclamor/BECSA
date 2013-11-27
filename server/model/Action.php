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
        $temp = explode( ";", $this->params );
        if( $temp[0] == "")
            $temp = array();
        return $temp;
    }
    
    public function setParams( array $params ) {
        $this->params = implode( ";", $params );
    }
    
    public function addParam( $param ) {
        $logger = new Logger('./logs');
        $logger->setBaseString('Action : addParam :');
        $temp = $this->getParams();
        if($temp == null)
            $temp = array();
        $temp[] = $param;
        $this->setParams( $temp );
        $logger->log('infos', 'infos_general', $param . ' - ' . $this->params . ' - ' . $temp, Logger::GRAN_MONTH);
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
