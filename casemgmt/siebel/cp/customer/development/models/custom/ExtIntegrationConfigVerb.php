<?php

/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC Contact Center + Siebel Case Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.5 (May 2015)
 *  Siebel release: 8.1.1.15
 *  reference: 141216-000121
 *  date: Wed Sep  2 23:14:33 PDT 2015

 *  revision: rnw-15-8-fixes-release-01
 *  SHA1: $Id: 566ed4fcc5f0c9b50f37d61128ca68288e56eba5 $
 * *********************************************************************************************
 *  File: ExtIntegrationConfigVerb.php
 * ****************************************************************************************** */

namespace Custom\Models;

use RightNow\Connect\v1_2 as RNCPHP,
    \RightNow\Utils\Url as Url,
    \RightNow\Utils\Text as Text;

class ExtIntegrationConfigVerb extends \RightNow\Models\Base {

    private $log;
    private $contact;
    private $extServerType;
    private $extConfigVerb;
    private $rntHost;

    function __construct() {
        parent::__construct();

        $this->CI->load->library('Utility');

        $this->log = $this->CI->utility->getLogWrapper();
        $this->contact = $this->CI->utility->getLoginContact();
        $this->checkExtIntegrationConfigVerb();
    }

    /**
     * Get corresponding endpoint, username and password of current site, 
     * from custom Config Verb CUSTOM_CFG_Accel_Ext_Integrations.
     * @return null 
     */
    function checkExtIntegrationConfigVerb() {
        $url = $_SERVER['REQUEST_URI'];
        if (Text::beginsWith($url, '/app/error/')) {
            return;
        }

        // check if CUSTOM_CFG_Accel_Ext_Integrations is defined in current site
        if (IS_DEVELOPMENT === true && !defined('CUSTOM_CFG_Accel_Ext_Integrations')) {
            $this->log->error('CUSTOM_CFG_' . 'Accel_Ext_Integrations is not set', __METHOD__, array(null, $this->contact));
            Url::redirectToErrorPage(8);
        }

        // get the value of config verb CUSTOM_CFG_Accel_Ext_Integrations
        $config = RNCPHP\Configuration::fetch(CUSTOM_CFG_Accel_Ext_Integrations);
        $configVerb = json_decode($config->Value, true);
        if (is_null($configVerb)) {
            $this->log->error('Unable to get the value of CUSTOM_CFG_' . 'Accel_Ext_Integrations', __METHOD__, array(null, $this->contact), $config);
            Url::redirectToErrorPage(8);
        }

        // check if current site is defined in the config rnt_host
        $server = \RightNow\Utils\Config::getConfig(OE_WEB_SERVER);
        $hosts = $configVerb['hosts'];
        if (is_null($hosts)) {
            $this->log->error('Unable to find hosts inside CUSTOM_CFG_' . 'Accel_Ext_Integrations', __METHOD__, array(null, $this->contact), json_encode($configVerb));
            Url::redirectToErrorPage(8);
        }
        foreach ($hosts as $host) {
            if ($server === $host['rnt_host']) {
                $this->extConfigVerb = $host;
                $this->extServerType = strtoupper($host['integration']['server_type']);
                $this->rntHost = $host['rnt_host'];
                return;
            }
        }

        // if no config verb match the current host
        $this->log->error('CUSTOM_CFG_Accel_Ext_Integrations :: host name isn\'t included in hosts', __METHOD__, array(null, $this->contact));
        Url::redirectToErrorPage(8);
    }

    /**
     * Get external server type 
     * @return string External server type, could be Siebel or MOCK
     */
    function getExtServerType() {
        return $this->extServerType;
    }

    /**
     * Get external integration config verb
     * @return array External integration config verb
     */
    function getExtIntegrationCofigVerb() {
        return $this->extConfigVerb;
    }

    /**
     * Get current Rightnow Host
     * @return string Current Rightnow Host URL
     */
    function getRntHost() {
        return $this->rntHost;
    }

}
