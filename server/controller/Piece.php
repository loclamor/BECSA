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
        debug( $_POST );
        if( isset($_POST["piece"]) and !empty($_POST["piece"] ) ) {
            $nom = $_POST["piece"];
            $alumiere = ( isset($_POST['alumiere']) and !empty($_POST['alumiere'] ) ) ? ($_POST['alumiere'] == "true" or $_POST['alumiere'] === true) : false;
            $avolet = ( isset($_POST['avolet']) and !empty($_POST['avolet'] ) ) ? ($_POST['avolet'] == "true" or $_POST['avolet'] === true) : false;
            
            $piece = new Model_Piece();
            $piece->setNom( $nom );
            $piece->setALumiere( $alumiere );
            $piece->setAVolet( $avolet );
            $piece->enregistrer();
            
            $this->code = 200;
            $this->piece = $piece;
        }
        else {
            $this->code = 400;
        }
        
    }
}

?>
