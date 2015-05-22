<?php

namespace Custom\Libraries;

class LogWrapper {

    private $log;

    function __construct($log) {      
        $this->log = $log;
    }

    /**
     * Create log in debug level
     * @param string $summary Log summary
     * @param string $source Log source
     * @param array $xRefArray Related Incident and Contact
     * @param string $message Log message
     */
    public function debug($summary, $source = null, array $xRefArray = null, $message = null, $timeElapsed = null) {
        $this->log->debug($summary, $source, $xRefArray, $message, $timeElapsed);
    }

     /**
     * Create log in error level
     * @param string $summary Log summary
     * @param string $source Log source
     * @param array $xRefArray Related Incident and Contact
     * @param string $message Log message
     */
    public function error($summary, $source = null, array $xRefArray = null, $message = null, $timeElapsed = null) {
        $this->log->debug($summary, $source, $xRefArray, $message, $timeElapsed);
    }

     /**
     * Create log in notice level
     * @param string $summary Log summary
     * @param string $source Log source
     * @param array $xRefArray Related Incident and Contact
     * @param string $message Log message
     */
    public function notice($summary, $source = null, array $xRefArray = null, $message = null, $timeElapsed = null) {
        $this->log->notice($summary, $source, $xRefArray, $message, $timeElapsed);
    }

}
