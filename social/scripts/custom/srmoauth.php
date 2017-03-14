<?php

/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  Copyright (c) 2014, 2015, 2016, 2017 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + SRM Integration Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 17.2 (February 2017) 
 *  reference: 160628-000117
 *  date: Fri Feb 10 19:47:44 PST 2017
 
 *  revision: rnw-17-2-fixes-release-2
 *  SHA1: $Id: df36e6c1cf617a66587eb758cb7163047cdba738 $
 * *********************************************************************************************
 *  File: srmoauth.php
 * *********************************************************************************************/

/**
 * This script is the primary script that brokers requests between an SRM add-in and CX or SRM.
 */
//We have increased error reporting.  This should be decreased in production.
error_reporting(E_ERROR | E_WARNING | E_PARSE);

//We simply fail if this is not HTTPS. HTTP is unaccceptable.
if (!isset($_SERVER['RNT_SSL']))
{
    die("You must submit this request via HTTPS");
}

//Set the docroot dir for OSvC for later file includes
if (!defined('DOCROOT'))
{
    $docroot = get_cfg_var('doc_root');
    define('DOCROOT', $docroot);
}

use Oracle\Accelerator\SRM as SRM;

//Include the SRM controller class
require_once DOCROOT . "/custom/oauthcontroller.phph";

//We will get the agent session ID
$session_id = !empty($_GET['session_id']) ? $_GET['session_id'] : -1;

//Run agent authenticator on the session ID that was passed.
//Agent authenticator blocks moving forward if the session id does not validate
require_once DOCROOT . '/include/services/AgentAuthenticator.phph';
$account = AgentAuthenticator::authenticateSessionID($session_id);

//Extract and validate informtion from the posted data
//All data must be present in the request to proceed.
if (!empty($account['acct_id']) && !empty($_POST['f']) && !empty($_POST['process_id']) && !empty($_POST['request_time']))
{
    $function = $_POST['f'];
    $request_time = $_POST['request_time'];
    $process_id = $_POST['process_id'];

    //If our validate request method fails, then stop the process
    if (SRM\OAuthController::validate_request($account['acct_id'], $process_id, $request_time))
    {
        //Call the function from our OAuthController Class that is requested via the URL.
        $oauthController = new SRM\OAuthController();
        if ($function == "refresh_access_token" || $function == "get_token")
        {
            $oauthController->$function($account['acct_id']);
        }
        else if(!empty($function) && method_exists($oauthController, $function)){
            $oauthController->$function();
        }
        else
        {
            echo "Invalid function request";
        }
    }
    else
    {
        echo "Invalid information provided. This action has been logged.";
    }
}
else
{
    echo "Data not provided in request";
}
