<?php

/**
 * Description of Model_HifiPlayer
 *
 * @author Xavier
 */
class Model_Hifi extends Entite {
    
    public $DB_table = 'hifi';
    public $DB_equiv = array(
        "id"             => "id",
        "artist"         => "artist",
        "title"       	 => "title"
    );
    
    public $artist;
    public $title;
    
    public function getArtist() {
        return $this->artist;
    }
    
    public function getTitle() {
        return $this->title;
    }
    
    public function setArtist( $artist ) {
        $this->artist = $artist;
    }
    
    public function setTitle( $title ) {
        $this->title = ( $title );
    }
      
    public function getState() {
        return array(
            "id" => $this->id,
            "artist" => $this->getArtist(),
            "title" => $this->getTitle()
        );
    }    
    
}