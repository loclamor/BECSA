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
