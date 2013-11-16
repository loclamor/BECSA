<?php

/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Description of Reveil
 *
 * @author Loïc
 */
class Controller_Reveil extends Controller {
    
    /**
     * cree un nouveau reveil
     * @param String reveil le nom du reveil
     * @param Time heure 'HH:MM' l'heure du reveil
     * @param String jours 'x,x,x,x,x,x,x' où x dans [0,1] les jours où le reveil sonne (exemple '1,0,0,0,0,0,0' pour lundi suelement)
     * @param Int repetition nombre de fois où le reveil sonne (0=inactif, -1=infini)
     * (parametres en POST)
     */
    public function creer() {
        if( isset($_POST["reveil"]) and !empty($_POST["reveil"] ) 
            && isset($_POST["heure"]) and !empty($_POST["heure"] ) 
            && isset($_POST["jours"]) and !empty($_POST["jours"] ) 
            && isset($_POST["repetition"]) and !empty($_POST["repetition"] ) 
            ) {
            $nom = $_POST["reveil"];
            $heure = $_POST["heure"];
            $jours = $_POST["jours"];
            $repetition = $_POST["repetition"];
            
            $reveil = new Model_Reveil();
            $reveil->setNom( $nom );
            $reveil->setHeure($heure);
            $reveil->setJours($jours);
            $reveil->setRepetition($repetition);
            $reveil->enregistrer();
            
            $this->code = 201;
            $this->reveil = $reveil;
        }
        else {
            $this->code = 400;
        }
        
    }
    
    /**
     * recupere un reveil
     * @param $_GET['id'] id de la piece
     */
    public function get() {
        $ges = Gestionnaire::getGestionnaire("reveil");
        $reveil = null;
        $this->reveil = null;
        if ( isset($_GET['id']) and !empty($_GET['id']) ) {
            $id = $_GET['id']; // nom de la piece
            $reveil = $ges->getOne( $id );
        } 
        else {
            $this->code = 400;
        }
        if( $reveil instanceof Model_Reveil ) {
            $this->code = 200;
            $this->reveil = $reveil->getState();
        }
        else {
            $this->code = 404;
        }
    }
    
    /**
     * supprime un reveil existant
     * @param Int id identifiant du reveil à supprimer
     * (parametre en GET)
     */
    public function supprimer() {
        if ( isset($_GET["id"]) and !empty($_GET["id"] ) ){
            $reveil = Gestionnaire::getGestionnaire("reveil")->getOne( $_GET["id"] );
            if ( $reveil ) {
                $reveil->supprimer();
                $this->code = 205;
            }
            else {
                $this->code = 404;
            }
        }
        else {
            $this->code = 400;
        }
    }
    
    /**
     * liste tous les reveils
     */
    public function lister() {
        $gesReveil = Gestionnaire::getGestionnaire("reveil");
        $reveils = $gesReveil->getAll();
        if( $reveils ) {
            $this->reveils = array();
            foreach ($reveils as $reveil) {
                if ($reveil instanceof Model_Reveil) {
                    $this->reveils[] = $reveil->getState();
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
