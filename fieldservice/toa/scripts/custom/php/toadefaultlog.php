<?php

/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + OFSC Reference Integration
 *  link: http://www-content.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.2 (Feb 2015)
 *  OFSC release: 15.2 (Feb 2015)
 *  reference: 150622-000130
 *  date: Thu Sep  3 23:14:07 PDT 2015

 *  revision: rnw-15-11-fixes-release-03
 *  SHA1: $Id: e7edcda2d53aab6835f3a49d516023dd58b12fe2 $
 * *********************************************************************************************
 *  File: toadefaultlog.php
 * ****************************************************************************************** */

require_once(OFSC_ROOT . 'itoalog.php');

class ToaDefaultLog implements IToaLog {

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
	
	/**
     * Create log in fatal level
     * @param string $summary Log summary
     * @param string $source Log source
     * @param array $xRefArray Related Incident and Contact
     * @param string $message Log message
     * @return boolean Simply return true
     */
    public function fatal($summary, $source = null, array $xRefArray = null, $message = null, $timeElapsed = null) {
         return true;
    }
	
	/**
     * Create log in warning level
     * @param string $summary Log summary
     * @param string $source Log source
     * @param array $xRefArray Related Incident and Contact
     * @param string $message Log message
     * @return boolean Simply return true
     */
    public function warning($summary, $source = null, array $xRefArray = null, $message = null, $timeElapsed = null) {
         return true;
    }
	
	/**
     * Create log in click level
     * @param string $summary Log summary
     * @param string $source Log source
     * @param array $xRefArray Related Incident and Contact
     * @param string $message Log message
     * @return boolean Simply return true
     */
    public function click($summary, $source = null, array $xRefArray = null, $message = null, $timeElapsed = null) {
         return true;
    }

}
