<?php

/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Description of Lumiere
 *
 * @author Loïc
 */
class Controller_Lumiere extends Controller {
    
    //TODO : allumer/eteindre ouvrir/fermer acceptent aussi l'ID en plus du nom de la piece
    
    /**
     * allume la lumiere dans une piece
     * @param $_GET['piece'] nom de la piece
     * or
     * @param $_GET['id'] id de la piece
     */
    public function allumer() {
        $gesPiece = Gestionnaire::getGestionnaire("piece");
        $piece = null;
        $this->piece = null;
        if( isset($_GET['piece']) and !empty($_GET['piece']) ){
            $pieceNom = $_GET['piece']; // nom de la piece
            $piece = $gesPiece->getOneOf( array( 'nom' => $pieceNom ) );
        }
        elseif ( isset($_GET['id']) and !empty($_GET['id']) ) {
            $pieceId = $_GET['id']; // nom de la piece
            $piece = $gesPiece->getOne( $pieceId );
        } 
        else {
            $this->code = 400;
        }
        if( $piece instanceof Model_Piece ) {
            if( $piece->aLumiere() ) {
                $piece->setLumiereAllumee();
                $piece->enregistrer( array("lumiereAllumee") );
                $this->code = 200;
                $this->piece = $piece->getState();
            }
            else {
                $this->code = 415;
                $this->piece = $piece->getState();
            }
        }
        else {
            $this->code = 404;
        }
    }
    
    /**
     * allume la lumiere dans toutes les pieces disposant de lumiere
     */
    public function allumerTout() {
        $gesPiece = Gestionnaire::getGestionnaire("piece");
        $pieces = $gesPiece->getOf(array('aLumiere' => 1));
        $this->pieces = array();
        if( $pieces ){
            foreach ($pieces as $piece) {
                if ($piece instanceof Model_Piece) {
                    if ($piece->aLumiere()) {
                        $piece->setLumiereAllumee();
                        $piece->enregistrer(array("lumiereAllumee"));
                    }
                    $this->pieces[] = $piece->getState();
                }
            }
            $this->code = 202;
        }
        else {
            $this->code = 404;
        }
    }
    
    /**
     * eteind la lumiere dans une piece
     * @param $_GET['piece'] nom de la piece
     * or
     * @param $_GET['id'] id de la piece
     */
    public function eteindre() {
        $gesPiece = Gestionnaire::getGestionnaire("piece");
        $piece = null;
        $this->piece = null;
        if( isset($_GET['piece']) and !empty($_GET['piece']) ){
            $pieceNom = $_GET['piece']; // nom de la piece
            $piece = $gesPiece->getOneOf( array( 'nom' => $pieceNom ) );
        }
        elseif ( isset($_GET['id']) and !empty($_GET['id']) ) {
            $pieceId = $_GET['id']; // nom de la piece
            $piece = $gesPiece->getOne( $pieceId );
        } 
        else {
            $this->code = 400;
        }
        if( $piece instanceof Model_Piece ) {
            if( $piece->aLumiere() ) {
                $piece->setLumiereEteinte();
                $piece->enregistrer( array("lumiereAllumee") );
                $this->code = 200;
                $this->piece = $piece->getState();
            }
            else {
                $this->code = 415;
                $this->piece = $piece->getState();
            }
        }
        else {
            $this->code = 404;
        }
    }
    
    
    /**
     * éteind la lumiere dans toutes les pieces disposant de lumiere
     */
    public function eteindreTout() {
        $gesPiece = Gestionnaire::getGestionnaire("piece");
        $pieces = $gesPiece->getOf(array('aLumiere' => 1));
        $this->pieces = array();
        if( $pieces ){
            foreach ($pieces as $piece) {
                if ($piece instanceof Model_Piece) {
                    if ($piece->aLumiere()) {
                        $piece->setLumiereEteinte();
                        $piece->enregistrer(array("lumiereAllumee"));
                    }
                    $this->pieces[] = $piece->getState();
                }
            }
            $this->code = 202;
        }
        else {
            $this->code = 404;
        }
    }
    
    /**
     * liste toutes les pieces disposant de lumiere
     */
    public function lister() {
        $gesPiece = Gestionnaire::getGestionnaire("piece");
        $pieces = $gesPiece->getOf(array('aLumiere' => 1));
        $this->pieces = array();
        if( $pieces ){
            foreach ($pieces as $piece) {
                if ($piece instanceof Model_Piece) {
                    $this->pieces[] = $piece->getState();
                }
            }
            $this->code = 202;
        }
        else {
            $this->code = 404;
        }
    }
    
}

?>
