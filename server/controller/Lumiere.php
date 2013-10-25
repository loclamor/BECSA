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
    
    /**
     * allume la lumiere dans une piece
     * @param $_GET['piece'] nom de la piece
     */
    public function allumer() {
       $pieceNom = $_GET['piece']; // nom de la piece
       $gesPiece = Gestionnaire::getGestionnaire("piece");
       $piece = $gesPiece->getOneOf( array( 'nom' => $pieceNom ) );
       if( $piece instanceof Model_Piece ) {
           if( $piece->aLumiere() ) {
               $piece->setLumiereAllumee();
               $piece->enregistrer( array("lumiereAllumee") );
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
     * eteind la lumiere dans une piece
     * @param $_GET['piece'] nom de la piece
     */
    public function eteindre() {
       $pieceNom = $_GET['piece']; // nom de la piece
       $piece = Gestionnaire::getGestionnaire('Model_Piece')->getOneOf( array( 'nom' => $pieceNom ) );
       if( $piece instanceof Model_Piece ) {
           if( $piece->aLumiere() ) {
               $piece->setLumiereEteinte();
               $piece->enregistrer( array("lumiereAllumee") );
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
    
    
}

?>
