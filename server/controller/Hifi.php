<?php

/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Description of Hifi
 *
 * @author Xavier
 */
class Controller_Hifi extends Controller {
    
    
    /**
     * recupere une chanson 
     * @param $_GET['id'] id de la chanson
     */
    public function get($id) {
 		$gesHifi = Gestionnaire::getGestionnaire("hifi");
        $song = null;
        $this->song = null;
        if ( isset($_GET['id']) and !empty($_GET['id']) ) {
            $songId = $_GET['id']; 
            $song = $gesHifi->getOne( $songId );
        } 
        else {
            $this->code = 400;
        }
        
        if( $song instanceof Model_Hifi ) {
            $this->code = 200;
        }
        else {
            $this->code = 404;
        }
    }

    
    /**
     * liste toutes les chansons 
     */
    public function lister() {
        $gesHifi = Gestionnaire::getGestionnaire("hifi");
        $songs = $gesHifi->getAll();
        $this->songs = array();
        if( $songs ){
            foreach ($songs as $song) {
                if ($song instanceof Model_Hifi) {
                    $this->songs[] = $song->getState();
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
