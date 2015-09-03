<!--
/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + EBS Enhancement
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.5 (May 2015)
 *  EBS release: 12.1.3
 *  reference: 150202-000157
 *  date: Wed Sep  2 23:11:32 PDT 2015

 *  revision: rnw-15-8-fixes-release-01
 *  SHA1: $Id: 3d6adc85f2c71300b887dcfeab5f65fc052cfdb4 $
 * *********************************************************************************************
 *  File: error.php
 * ****************************************************************************************** */
-->

<rn:meta title="#rn:msg:ERROR_LBL#" template="standard.php" login_required="false" />

<?
$errorID = \RightNow\Utils\Url::getParameter('error_id');
$errorID = intval($errorID);

switch ($errorID) {
    case 8:
         // configuration error
        list($errorTitle, $errorMessage) = array("Configuration Error", 'The site name is not set in configuration verb CUSTOM_CFG_EBS_' . 'Web_Service_Endpoint');
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
         // EBS server error
        list($errorTitle, $errorMessage) = array("EBS Server Error", "Unable to resolve EBS host");
        break;
    case 12:
        // contact_party_id and/or contact_org_id not provided
        list($errorTitle, $errorMessage) = array("Permission Denied", "Please set EBS Contact Party ID and/or EBS Contact Org ID first.");
        break;
    default:
        list($errorTitle, $errorMessage) = \RightNow\Utils\Framework::getErrorMessageFromCode($errorID);
        break;
}
?>

<div id="rn_PageTitle" class="rn_ErrorPage">
    <h1><?=$errorTitle;?></h1>
</div>

<div id="rn_PageContent" class="rn_ErrorPage">
    <div class="rn_Padding">
        <p><?=$errorMessage;?></p>
    </div>
</div>
