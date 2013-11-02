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
class Controller_Porte extends Controller {
    
    /**
     * deverrouille la porte d'une piece
     * @param $_GET['piece'] nom de la piece
     * or
     * @param $_GET['id'] id de la piece
     */
    public function deverrouiller() {
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
            if( $piece->aPorte() ) {
                $piece->setPorteDeverrouillee();
                $piece->enregistrer( array("porteVerrouillee") );
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
     * deverrouille les portes de toutes les pieces disposant de porte
     */
    public function deverrouillerTout() {
        $gesPiece = Gestionnaire::getGestionnaire("piece");
        $pieces = $gesPiece->getOf(array('aPorte' => 1));
        $this->pieces = array();
        if( $pieces ){
            foreach ($pieces as $piece) {
                if ($piece instanceof Model_Piece) {
                    if( $piece->aPorte() ) {
                        $piece->setPorteDeverrouillee();
                        $piece->enregistrer( array("porteVerrouillee") );
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
     * verrouille la porte d'une piece
     * @param $_GET['piece'] nom de la piece
     * or
     * @param $_GET['id'] id de la piece
     */
    public function verrouiller() {
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
            if( $piece->aPorte() ) {
                $piece->setPorteVerrouillee();
                $piece->enregistrer( array("porteVerrouillee") );
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
     * verrouille les portes de toutes les pieces disposant de porte
     */
    public function verrouillerTout() {
        $gesPiece = Gestionnaire::getGestionnaire("piece");
        $pieces = $gesPiece->getOf(array('aPorte' => 1));
        $this->pieces = array();
        if( $pieces ){
            foreach ($pieces as $piece) {
                if ($piece instanceof Model_Piece) {
                    if( $piece->aPorte() ) {
                        $piece->setPorteVerrouillee();
                        $piece->enregistrer( array("porteVerrouillee") );
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
     * liste toutes les pieces disposant de porte
     */
    public function lister() {
        $gesPiece = Gestionnaire::getGestionnaire("piece");
        $pieces = $gesPiece->getOf(array('aPorte' => 1));
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
