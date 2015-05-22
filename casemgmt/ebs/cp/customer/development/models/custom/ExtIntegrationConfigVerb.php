<?php

/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC WSS + EBS Case Management Accelerator
 *  link: http://www-content.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.2 (February 2015)
 *  EBS release: 12.1.3
 *  reference: 140626-000078
 *  date: Fri May 15 13:41:40 PDT 2015

 *  revision: rnw-15-2-fixes-release-01
 *  SHA1: $Id: 128c51c51b746ee97c2e93d072dfb97058dc2e51 $
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
    private $ebsDefaultSROwnerID;

    function __construct() {
        parent::__construct();

        $this->CI->load->library('Utility');

        $this->log = $this->CI->utility->getLogWrapper();
        $this->contact = $this->CI->utility->getLoginContact();
        $this->checkExtIntegrationConfigVerb();
    }

    /**
     * Get the endpoint, username and password of the current site from custom 
     * Config Verb CUSTOM_CFG_EBS_Web_Service_Endpoint.
     * @return null
     */
    function checkExtIntegrationConfigVerb() {
        $url = $_SERVER['REQUEST_URI'];
        if (Text::beginsWith($url, '/app/error/')) {
            return;
        }
        // check if CUSTOM_CFG_EBS_Web_Service_Endpoint is defined in the current site
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
                $this->extServerType = $host['integration']['server_type'];
                $this->rntHost = $host['rnt_host'];
                $this->ebsDefaultSROwnerID = $host['integration']['ebs_default_sr_owner_id'];
                return;
            }
        }

        // if no config verb match the current host
        $this->log->error("CUSTOM_CFG_Accel_Ext_Integrations :: host name isn't included in hosts", __METHOD__, array(null, $this->contact));
        Url::redirectToErrorPage(8);
    }

    /**
     * Get external server type 
     * @return string External server type, could be EBS or MOCK
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
     * Get external request type field mapping
     * @return array External request type field mapping
     */
    function getExtRequestTypeMapping() {
        return $this->extConfigVerb['integration']['request_type_mapping'];
    }

    /**
     * Get current Rightnow Host
     * @return string Current Rightnow Host url
     */
    function getRntHost() {
        return $this->rntHost;
    }

    /**
     * Get the default value of EBS SR owner ID
     * @return string Default value of EBS SR owner ID
     */
    function getEbsDefaultSROwnerID() {
        return $this->ebsDefaultSROwnerID;
    }

}
