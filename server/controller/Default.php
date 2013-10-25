<?php

/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Description of Test
 *
 * @author loclamor
 */
class Controller_Default extends Controller {
	//put your code here
	public function index() {
        $this->isJSON = true;
		$this->msg = "it...";
	}
}

?>
