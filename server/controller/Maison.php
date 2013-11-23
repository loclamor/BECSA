<?php

/**
 * Description of Maison
 *
 * @author Loïc
 */
class Controller_Maison extends Controller {
    
    public function getState(){
        if( isset($_GET['dest']) and !empty($_GET['dest']) ){
            $dest = $_GET['dest']; // nom du destinataire
        }
        else {
            $this->code = 400;
            return;
        }
        
        $state = array();
        
        $pieces = Gestionnaire::getGestionnaire('piece')->getAll();
        $state['pieces'] = array();
        foreach ( $pieces as $p ) {
            if( $p instanceof Model_Piece ){
                $state['pieces'][] = $p->getState();
            }
        }

        $reveils = Gestionnaire::getGestionnaire('reveil')->getAll();
        $state['reveils'] = array();
        foreach ( $reveils as $r ) {
            if( $r instanceof Model_Reveil ){
                $state['reveils'][] = $r->getState();
            }
        }
        
        $actions = Gestionnaire::getGestionnaire('action')->getOf( array( 'destinataire' => $dest ) );
        $state['actions'] = array();
        foreach ( $actions as $a ) {
            if( $a instanceof Model_Action ){
                $state['actions'][] = $a->getState();
            }
        }
        
        $this->state = $state;
        $this->code = 202;
    }
}
?>
