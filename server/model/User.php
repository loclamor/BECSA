<?php

/**
 * Description of Model_User
 *
 * @author Loïc
 */
class Model_User extends Entite {
    
    public $memberType = [ 'username' => 'varchar(10)', 'password' => 'integer', 'birthdate' => 'date' ];
    
    protected $username;
    protected $password;
    protected $birthdate;
}