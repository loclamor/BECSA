<?php

/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Description of Controller
 *
 * @author loclamor
 */
abstract class Controller {
	
    
    
	public $isJSON = true;
	
	public function getAction( $action ){
        
		$action = firstchartolower( $action );
        $subClass = get_class($this);
        
        $this->logger = new Logger('./logs');
        $this->logger->setBaseString($subClass.' : '.$action.' :');
        //$this->logger->log('infos', 'infos_general', 'requested', Logger::GRAN_MONTH);
        
		$this->$action();
		
		
		$simpleName = firstchartolower( str_replace( "Controller_", "", $subClass ) );
		$pathView = "view/".$simpleName."/".$action.".phtml";
		
		$content = "";
		if( file_exists( $pathView ) ){
			ob_start();
			require $pathView;
			$content = ob_get_clean();
		}
		
		if( $this->isJSON ) {
            header('Content-type: application/json; charset=utf-8');
			echo $content;
			die();
		}

        return $content;
		//return '<div id="'.$simpleName.'-'.$action.'" >'.$content.'</div>';
	}
	
}

?>
