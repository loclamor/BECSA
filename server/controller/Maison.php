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
        
        $this->isUpdated = true;
        /**
         * LONG POLLING
         */
        if( isset($_GET['timestamp']) and !empty($_GET['timestamp']) ){
            $clientTime = $_GET['timestamp'];
            sleep( 1 );
            if( !file_exists('lastUpdate.time') ){
                $timeFile = fopen('lastUpdate.time', 'w');
                fputs($timeFile, time() );
                fclose($timeFile);
            }
            $timeFile = fopen('lastUpdate.time', 'r');
            $lastUpdate = intval( fgets($timeFile) );
            fclose($timeFile);
            
            if( $clientTime > $lastUpdate ) {
                $this->isUpdated = false;
            }
            
//            $continue = $clientTime > $lastUpdate;
//            $nbTimesRemaining = 20;
//            $currTime = time();
//            $lastTime = $currTime;
//            while( $continue ) {
//                
//                usleep(50000); // sleep 0.05s = 500000µs
//                $timeFile = fopen('lastUpdate.time', 'r');
//                $lastUpdate = intval( fgets($timeFile) );
//                fclose($timeFile);
//                $continue = $clientTime > $lastUpdate;
//                $nbTimesRemaining--;
//                if( $nbTimesRemaining == 0 ) {
//                    $continue = false;
//                    $this->isUpdated = false;
//                }
//            }
        }
        if( $this->isUpdated ) {
            $state = array();

            $pieces = Gestionnaire::getGestionnaire('piece')->getAll();
            $state['pieces'] = array();
            if( $pieces ) {
                foreach ( $pieces as $p ) {
                    if( $p instanceof Model_Piece ){
                        $state['pieces'][] = $p->getState();
                    }
                }
            }

            $reveils = Gestionnaire::getGestionnaire('reveil')->getAll();
            $state['reveils'] = array();
            if( $reveils ) {
                foreach ( $reveils as $r ) {
                    if( $r instanceof Model_Reveil ){
                        $state['reveils'][] = $r->getState();
                    }
                }
            }

            $envoie = date("Y-m-d H:i:s", time()-60);
            $actions = Gestionnaire::getGestionnaire('action')->getOf( array( 'destinataire' => $dest, "envoie" => array( ">", $envoie ) ) );
            $state['actions'] = array();
            if( $actions ) {
                foreach ( $actions as $a ) {
                    if( $a instanceof Model_Action ){
                        $state['actions'][] = $a->getState();
                    }
                }
            }
            
            if( isset($_GET['hifi']) ){
                $songs = Gestionnaire::getGestionnaire('hifi')->getAll();
                $state['songs'] = array();
                if( $songs ) {
                    foreach ( $songs as $r ) {
                        if( $r instanceof Model_Hifi ){
                            $state['songs'][] = $r->getState();
                        }
                    }
                }
            }

            $this->code = 202;
            $this->state = $state;
        }
    }
}
?>
