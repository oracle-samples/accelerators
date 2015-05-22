<?php

namespace Custom\Libraries;

class DefaultLog {
    
    function __construct() {
        
    }

    /**
     * Create log in debug level
     * @return boolean Simply return true
     */
    public function debug() {
        return true;
    }
    
    /**
     * Create log in error level
     * @return boolean Simply return true
     */
    public function error() {
        return true;
    }
    
    /**
     * Create log in notice level
     * @return boolean Simply return true
     */
    public function notice() {
        return true;
    }
}
