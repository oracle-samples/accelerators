<!--
/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC Contact Center + Siebel Case Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  Siebel release: 8.1.1.15
 *  reference: 150520-000047
 *  date: Mon Nov 30 20:14:20 PST 2015

 *  revision: rnw-15-11-fixes-release-2
 *  SHA1: $Id: 6f76fa150641ab827d0037c7ab4a2dec7afcdd9a $
 * *********************************************************************************************
 *  File: error.php
 * ****************************************************************************************** */
-->

<?
$errorID = \RightNow\Utils\Url::getParameter('error_id');
$errorID = intval($errorID);

switch ($errorID) {
    case 8:
         // configuration error
        list($errorTitle, $errorMessage) = array("Configuration Error", 'The site name is not set in configuration verb CUSTOM_CFG_Accel' . '_Ext_Integrations');
        break;
    case 9:
        // invalid i_id
        list($errorTitle, $errorMessage) = array("Permission Denied", "An illegal value was received for the parameter 'i_id'");
        break;
    case 10:
         // invalid sr_id
        list($errorTitle, $errorMessage) = array("Permission Denied", "An illegal value was received for the parameter 'sr_id'");
        break;
    case 11:
         // Siebel server error
        list($errorTitle, $errorMessage) = array("Siebel Server Error", "There has been an error communicating with Siebel");
        break;
    case 12:
        // contact_party_id and/or contact_org_id not provided
        list($errorTitle, $errorMessage) = array("Permission Denied", "Please set Siebel Contact Party ID and/or Siebel Contact Org ID first.");
        break;
    case 13:
         // configuration error
        list($errorTitle, $errorMessage) = array("Configuration Error", 'Invalid configuration setting for CUSTOM_CFG_Accel_' . 'Ext_Integrations');
        break;
    default:
        list($errorTitle, $errorMessage) = \RightNow\Utils\Framework::getErrorMessageFromCode($errorID);
        break;
}
?>

<rn:meta title="#rn:msg:ERROR_LBL#" template="standard.php" login_required="false" />

<div class="rn_Hero">
    <div class="rn_Container">
        <h1><?=$errorTitle;?></h1>
    </div>
</div>
<div class="rn_PageContent rn_ErrorPage rn_Container">
    <p><?=$errorMessage;?></p>
</div>
