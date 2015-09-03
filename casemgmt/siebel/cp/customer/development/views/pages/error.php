<!--
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
 *  date: Wed Sep  2 23:14:34 PDT 2015

 *  revision: rnw-15-8-fixes-release-01
 *  SHA1: $Id: 43f8e7464a7c9c56f2a09a76b6071df6fcde79cf $
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
        list($errorTitle, $errorMessage) = array("Siebel Server Error", "Unable to resolve Siebel host");
        break;
        case 12:
        // contact_party_id and/or contact_org_id not provided
        list($errorTitle, $errorMessage) = array("Permission Denied", "Please set Siebel Contact Party ID and/or Siebel Contact Org ID first.");
        break;
    default:
        list($errorTitle, $errorMessage) = \RightNow\Utils\Framework::getErrorMessageFromCode($errorID);
        break;
}
?>
<rn:meta title="#rn:msg:ERROR_LBL#" template="standard.php" login_required="false" />
<div id="rn_PageTitle" class="rn_ErrorPage">
    <h1><?php echo $errorTitle; ?></h1>
</div>

<div id="rn_PageContent" class="rn_ErrorPage">
    <div class="rn_Padding">
        <p><?php echo $errorMessage; ?></p>
    </div>
</div>
