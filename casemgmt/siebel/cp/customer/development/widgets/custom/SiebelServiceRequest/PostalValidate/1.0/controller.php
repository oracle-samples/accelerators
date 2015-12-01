<?php

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
 *  date: Mon Nov 30 20:14:21 PST 2015

 *  revision: rnw-15-11-fixes-release-2
 *  SHA1: $Id: 2ac46038a9eb31896088cdb825c30a3bc3ad97cd $
 * *********************************************************************************************
 *  File: controller.php
 * ****************************************************************************************** */

namespace Custom\Widgets\SiebelServiceRequest;

class PostalValidate extends \RightNow\Widgets\TextInput {
    private $log; // if used;
    
    function __construct($attrs) {
        parent::__construct($attrs);
        
        $this->setAjaxHandlers(array(
            'addressverification_ajax_endpoint' => array(
                'method'      => 'handle_addressver_ajax_endpoint',
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
    function handle_addressver_ajax_endpoint(array $params) {
        // Perform AJAX-handling here...
        $street = $params['street'];
        $city = $params['city'];
        $state = $params['state'];
        $zip5 = $params['zip'];
        $country = $params['country'];

        $usaExpr = '/US|USA|U\.S\.A|United States|Estados Unidos.*Am.rica/i';
        // check the country first. Specific for USPS; made just to provide LOGGING from PHP
        if (!preg_match($usaExpr, $country)) {
            $this->CI->load->library('Utility');
            $this->log = $this->CI->utility->getLogWrapper();
            $this->log->notice("Please--USA only postal address verification!", __METHOD__, null, print_r($params, true));
            
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
            $result = $this->CI->model('custom/SiebelServiceRequest')->callAddressVerification(
                    $street, $city, $state, $zip5);

            $jsobj = $result->result;
            if ($jsobj) { // unwraps one result
                echo json_encode($jsobj);
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
    
    /**
     * Overridable methods from TextInput:
     */
    // public function outputConstraints()
    // protected function determineDisplayType($fieldName, $dataType, $constraints)
}