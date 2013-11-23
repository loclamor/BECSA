<?php

/**
 * Description of Controller_Action
 *
 * @author loclamor
 */
class Controller_Action extends Controller {
	
	public function get() {
        $ges = Gestionnaire::getGestionnaire("action");
        $actions = null;
        $this->actions = array();
        if ( isset($_GET['dest']) and !empty($_GET['dest']) ) {
            $dest = $_GET['dest'];
            $actions = $ges->getOf( array( "destinataire" => $dest ) );
        } 
        else {
            $this->code = 400;
        }
        if( $actions ) {
            foreach ( $actions as $action ) {
                if( $action instanceof Model_Reveil ) {
                    $this->actions[] = $action->getState();
                }
            }
            $this->code = 202;
        }
        else {
            $this->code = 404;
        }
	}
    
    public function post() {
        if ( isset($_POST['action']) && !empty($_POST['action']) 
             && isset($_POST['dest']) && !empty($_POST['dest']) 
        ){
            $action = new Model_Action();
            $action->setAction($_POST['action']);
            $action->setDestinataire($_POST['dest']);
            $action->enregistrer();
            $this->code = 201;
        }
        else {
            $this->code = 400;
        }
	}
}

?>
