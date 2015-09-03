<?php

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
 *  SHA1: $Id: 141f454cdbeacf37102b58670c6b70b3e7fcc45f $
 * *********************************************************************************************
 *  File: controller.php
 * ****************************************************************************************** */

namespace Custom\Widgets\EbsServiceRequest;

class SerialNumberInput extends \RightNow\Widgets\TextInput {

    function __construct($attrs) {
        parent::__construct($attrs);

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

    /**
     * Handle the Serial Number validation ajax request
     * @param array $params Ajax parameters
     */
    function handleValidateSerialNumberAjaxEndpoint(array $params) {
        $productID = $params['product_id'];
        $serialNumber = $params['serial_number'];

        if ($serialNumber !== null && $serialNumber !== '') {
            $result = $this->CI->model('custom/EbsServiceRequest')->validateSerialNumber($serialNumber, $productID);
            if ($result->result) {
                echo json_encode($result->result);
            }
        }
    }

}
