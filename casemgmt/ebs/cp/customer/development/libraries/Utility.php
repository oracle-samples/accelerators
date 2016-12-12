<?php

/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + EBS Enhancement
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  EBS release: 12.1.3
 *  reference: 150505-000099, 150420-000127
 *  date: Thu Nov 12 00:52:35 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: 183fcde322a86f9ebc2d530e819902f859451f2b $
 * *********************************************************************************************
 *  File: Utility.php
 * ****************************************************************************************** */

namespace Custom\Libraries;

use RightNow\Connect\v1_2 as RNCPHP;

//require_once (APPPATH . 'libraries/scTools/SCLog.php');
//require_once (APPPATH . 'libraries/log/SCLog.php');
require_once (APPPATH . 'libraries/log/LogWrapper.php');
require_once (APPPATH . 'libraries/log/DefaultLog.php');

class Utility {

    function __construct() {
        
    }

    /**
     * Create log wrapper
     * @return Logwrapper object
     */
    function getLogWrapper() {
        //$log = new SCLog();
        $log = new DefaultLog();
        $logWrapper = new LogWrapper($log);
        return $logWrapper;
    }

    /**
     * Get the product object by ID using ROQL
     * @param int $productID Product ID used in RightNow
     * @return object|null Product queried by it's ID
     */
    function getProductByServiceProductID($productID) {
        if ($productID === null) {
            return null;
        }
        $queryString = "SELECT S.PartNumber, S.Name, S.Descriptions.LabelText FROM SalesProduct S WHERE S.ServiceProduct.ID={$productID}";
        $roqlResultSet = RNCPHP\ROQL::query($queryString)->next();
        if ($product = $roqlResultSet->next()) {
            return $product;
        }
    }

    /**
     * Get the product object by part number  using ROQL
     * @param int $partNumber Product part number used in RightNow
     * @return object|null Product queried by it's ID
     */
    function getProductByPartNumber($partNumber) {
        if ($partNumber === null) {
            return null;
        }
        $queryString = "SELECT S.ServiceProduct.ID, S.PartNumber, S.Name, S.Descriptions.LabelText FROM SalesProduct S WHERE S.PartNumber='{$partNumber}'";
        $roqlResultSet = RNCPHP\ROQL::query($queryString)->next();
        if ($product = $roqlResultSet->next()) {
            return $product;
        }
    }

    /**
     * Get login contact
     * @return RNCPHP\Contact Contact object
     */
    function getLoginContact() {
        $CI = get_instance();
        $contactID = $CI->session ? ($CI->session->getProfileData('contactID') ? : 0) : 0;
        return ($contactID !== 0) ? RNCPHP\Contact::fetch($contactID) : null;
    }

    /**
     * Check if the contact party id and contact org id have been set
     * @param RNCPHP\Contact $contact Contact object
     * @return boolean Valid nor not
     */
    function validateEbsContactID(RNCPHP\Contact $contact = null) {
        $contactPartyID = ($contact !== null) ? $contact->CustomFields->Accelerator->ebs_contact_party_id : null;
        $contactOrgID = ($contact !== null) ? $contact->CustomFields->Accelerator->ebs_contact_org_id : null;
        if ($contactPartyID === null || $contactOrgID === null) {
            return false;
        } else {
            return true;
        }
    }

}
