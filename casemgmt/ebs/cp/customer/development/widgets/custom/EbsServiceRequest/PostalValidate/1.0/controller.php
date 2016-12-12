<?php

/** *******************************************************************************************
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
 *  date: Thu Nov 12 00:52:37 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: f9da2f6871185e6b61a7f4dfdb979465d305df22 $
 * *********************************************************************************************
 *  File: controller.php
 * ****************************************************************************************** */

namespace Custom\Widgets\EbsServiceRequest;

class PostalValidate extends \RightNow\Widgets\TextInput {
    private $log; // if used

    function __construct($attrs) {
        parent::__construct($attrs);

        $this->setAjaxHandlers(array(
            'addressverification_ajax_endpoint' => array(
                'method'      => 'handleAddressverAjaxEndpoint',
                'clickstream' => 'custom_action',
            ),
        ));
    }

    function getData() {
        return parent::getData();
    }

    /**
     * Handles the addressverification_ajax_endpoint AJAX request
     * @param array $params Get / Post parameters
     */
    function handleAddressverAjaxEndpoint(array $params) {
        // Perform AJAX-handling here...
        $street = $params['street'];
        $city = $params['city'];
        $state = $params['state'];
        $zipFive = $params['zip'];
        $country = $params['country'];

        $usaExpr = '/US|USA|U\.S\.A|United States|Estados Unidos.*Am.rica/i';
        // check the country first. Specific for USPS; made just to provide LOGGING from PHP
        if (!preg_match($usaExpr, $country)) {
            $this->CI->load->library('Utility');
            $this->log = $this->CI->utility->getLogWrapper();
            $this->log->notice("Please--USA only postal address verification!", __METHOD__, null, var_export($params, true));

            echo json_encode(array(
                'isValid' => false,
                'message' => 'Please--USA only postal address verification!',
                'isError' => false,
                'isInvalidCountry' => true,
                'address' => array()
                ));
        } else if (($street !== null && $street !== '')
            || ($city !== null && $city !== '')) {
            // echo response.  Callin only if at least 1 of 2 important lines haz value
            $result = $this->CI->model('custom/EbsServiceRequest')->callAddressVerification(
                $street, $city, $state, $zipFive);

            $jsobj = $result->result;
            if ($jsobj) {
                echo json_encode($jsobj); // unwraps one result
            }
        } else {
            // empty lines have to return something.
            echo json_encode(array(
                'isValid' => false,
                'message' => 'Empty address lines--error!',
                'isError' => false,
                'isInvalidCountry' => false,
                'address' => array()
            ));
        }
    }
}