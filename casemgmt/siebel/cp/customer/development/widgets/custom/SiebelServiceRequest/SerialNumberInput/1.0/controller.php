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
 *  date: Mon Nov 30 19:59:30 PST 2015

 *  revision: rnw-15-11-fixes-release-2
 *  SHA1: $Id: c7100415f8b32edfe9b8528e46acc69729db8a90 $
 * *********************************************************************************************
 *  File: controller.php
 * ****************************************************************************************** */

namespace Custom\Widgets\SiebelServiceRequest;

class SerialNumberInput extends \RightNow\Widgets\TextInput {

    function __construct($attrs) {
        parent::__construct($attrs);

        // set up Ajax Hander
        $this->setAjaxHandlers(array(
            'serial_number_validate_ajax' => array(
                'method' => 'handleValidateSerialNumberAjaxEndpoint',
                'clickstream' => 'custom_action',
            ),
        ));
    }

    function getData() {
        $this->data['js']['development_mode'] = IS_DEVELOPMENT;
        return parent::getData();
    }

    function handleValidateSerialNumberAjaxEndpoint($params) {
        $productID = $params['product_id'];
        $serialNumber = $params['serial_number'];

        if ($serialNumber !== null && $serialNumber !== '') {
            $result = $this->CI->model('custom/SiebelServiceRequest')->validateSerialNumber($serialNumber, $productID);
            if ($result->result) {
                echo json_encode($result->result);
            }
        }
    }

}
