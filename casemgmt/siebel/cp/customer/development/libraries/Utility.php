<?php

/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC Contact Center + Siebel Case Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  Siebel release: 8.1.1.15
 *  reference: 150520-000047
 *  date: Mon Nov 30 20:14:19 PST 2015

 *  revision: rnw-15-11-fixes-release-2
 *  SHA1: $Id: c5557f363bc60edf665df6aab248c593b416c1d9 $
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

    const BUSINESS_FUNCTION = 'OSvC to Siebel WSS Integration';
    const EXTENTION_NAME = 'OSvC to Siebel WSS Integration Extension';
    const EXTENTION_SIGNATURE = null; // TBD
    const SCLOG_STDOUT_LEVEL = 1; // Fatal

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
     * @param int $productID RN Product ID
     * @return object|null Product
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
     * Get the product object by ID using ROQL
     * @param int $partNumber RN Product ID
     * @return object|null Product
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
     * @param RNCPHP\Contact $contact Contact
     * @return boolean
     */
    function validateSiebelContactID(RNCPHP\Contact $contact = null) {
        $contactPartyID = ($contact !== null) ? $contact->CustomFields->Accelerator->siebel_contact_party_id : null;
        $contactOrgID = ($contact !== null) ? $contact->CustomFields->Accelerator->siebel_contact_org_id : null;
        if ($contactPartyID === null || $contactOrgID === null) {
            return false;
        } else {
            return true;
        }
    }

}
