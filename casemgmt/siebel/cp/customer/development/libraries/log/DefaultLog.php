<?php

namespace Custom\Libraries;

require_once (APPPATH . 'libraries/log/Log.php');

class DefaultLog implements Log {

    function __construct() {
        
    }

    /**
     * Create log in debug level
     * @param string $summary Log summary
     * @param string $source Log source
     * @param array $xRefArray Related Incident and Contact
     * @param string $message Log message
     * @return boolean Simply return true
     */
    public function debug($summary, $source = null, array $xRefArray = null, $message = null, $timeElapsed = null) {
        return true;
    }

    /**
     * Create log in error level
     * @param string $summary Log summary
     * @param string $source Log source
     * @param array $xRefArray Related Incident and Contact
     * @param string $message Log message
     * @return boolean Simply return true
     */
    public function error($summary, $source = null, array $xRefArray = null, $message = null, $timeElapsed = null) {
         return true;
    }

    /**
     * Create log in notice level
     * @param string $summary Log summary
     * @param string $source Log source
     * @param array $xRefArray Related Incident and Contact
     * @param string $message Log message
     * @return boolean Simply return true
     */
    public function notice($summary, $source = null, array $xRefArray = null, $message = null, $timeElapsed = null) {
         return true;
    }

}
