<?php

/**
 * Description of Piece
 *
 * @author Loïc
 */
class Controller_Piece extends Controller {
    
    /**
     * cree une nouvelle piece
     * @param string piece le nom de la piece
     * @param boolean alumiere [optionnel, defaut true] definit s'il y a une lumiere dans la piece
     * @param boolean avolet [optionnel, defaut true] definit s'il y a un volet dans la piece
     * (paramettres en POST
     */
    public function creer() {
        if( isset($_POST["piece"]) and !empty($_POST["piece"] ) ) {
            $nom = $_POST["piece"];
            $alumiere = ( isset($_POST['alumiere']) and !empty($_POST['alumiere'] ) ) ? ($_POST['alumiere'] == "true" or $_POST['alumiere'] === true) : false;
            $avolet = ( isset($_POST['avolet']) and !empty($_POST['avolet'] ) ) ? ($_POST['avolet'] == "true" or $_POST['avolet'] === true) : false;
            
            $piece = new Model_Piece();
            $piece->setNom( $nom );
            $piece->setALumiere( $alumiere );
            $piece->setAVolet( $avolet );
            $piece->enregistrer();
            
            $this->code = 201;
            $this->piece = $piece;
        }
        else {
            $this->code = 400;
        }
        
    }
    
    /**
     * recupere une piece
     * @param $_GET['piece'] nom de la piece
     * or
     * @param $_GET['id'] id de la piece
     */
    public function get() {
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
            $this->code = 200;
            $this->piece = $piece->getState();
        }
        else {
            $this->code = 404;
        }
    }
    
    /**
     * liste toutes les pieces
     */
    public function lister() {
        $pieces = Gestionnaire::getGestionnaire("piece")->getAll();
        $this->pieces = array();
        if( $pieces ){
            foreach ($pieces as $piece) {
                if( $piece instanceof Model_Piece ) {
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
