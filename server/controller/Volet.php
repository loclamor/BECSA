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
class Controller_Volet extends Controller {
    
    /**
     * ouvre les volets d'une piece
     * @param $_GET['piece'] nom de la piece
     * or
     * @param $_GET['id'] id de la piece
     */
    public function ouvrir() {
       $gesPiece = Gestionnaire::getGestionnaire("piece");
        $piece = null;
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
            if( $piece->aVolet() ) {
                $piece->setVoletOuvert();
                $piece->enregistrer( array("voletOuvert") );
                $this->code = 200;
            }
            else {
                $this->code = 415;
            }
        }
        else {
            $this->code = 404;
        }
    }
    
    /**
     * ouvre les volets de toutes les pieces disposant de volets
     */
    public function ouvrirTout() {
        $gesPiece = Gestionnaire::getGestionnaire("piece");
        $pieces = $gesPiece->getOf(array('aVolet' => 1));
        foreach ($pieces as $piece) {
            if ($piece instanceof Model_Piece) {
                if ($piece->aVolet()) {
                    $piece->setVoletOuvert();
                    $piece->enregistrer(array("voletOuvert"));
                }
            }
        }
        $this->code = 202;
    }
    
    /**
     * ferme les volets d'une piece
     * @param $_GET['piece'] nom de la piece
     * or
     * @param $_GET['id'] id de la piece
     */
    public function fermer() {
       $gesPiece = Gestionnaire::getGestionnaire("piece");
        $piece = null;
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
            if( $piece->aVolet() ) {
                $piece->setVoletFerme();
                $piece->enregistrer( array("voletOuvert") );
                $this->code = 200;
            }
            else {
                $this->code = 415;
            }
        }
        else {
            $this->code = 404;
        }
    }
    
    
    /**
     * ferme les volets de toutes les pieces disposant de volets
     */
    public function fermerTout() {
        $gesPiece = Gestionnaire::getGestionnaire("piece");
        $pieces = $gesPiece->getOf(array('aVolet' => 1));
        foreach ($pieces as $piece) {
            if ($piece instanceof Model_Piece) {
                if ($piece->aLumiere()) {
                    $piece->setVoletFerme();
                    $piece->enregistrer(array("voletOuvert"));
                }
            }
        }
        $this->code = 202;
    }
    
    
}

?>
