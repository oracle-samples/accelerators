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
 *  date: Thu Nov 12 00:55:26 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: ef4096e698a745305db313999e7ac483d2fa5e30 $
 * *********************************************************************************************
 *  File: ServiceRequestController.php
 * ****************************************************************************************** */

namespace Custom\Controllers;

class ServiceRequestController extends \RightNow\Controllers\Base {

    private $log;
    private $contact;
    private $extServerType;

    function __construct() {
        parent::__construct();

        $this->load->library('Utility');

        $this->log = $this->utility->getLogWrapper();
        $this->contact = $this->utility->getLoginContact();
        $this->extServerType = $this->model('custom/ExtIntegrationConfigVerb')->getExtServerType();
    }

    /**
     * Handle the form to create a new Incident to associate with
     * Service Request in 3rd party based on the external server type.
     * @return null
     */
    function sendFormToCreateIncidentToLinkWithSR() {
        if ($this->extServerType === 'SIEBEL') {
            $this->createIncidentBasedOnSRForSiebel();
        } else if ($this->extServerType === 'MOCK') {
            //Mock Specific
        } else {
            $this->log->error('Invalid external server type', __METHOD__);
        }
    }

    /**
     * Create new Incident in CP using the Service Request data
     * @return null
     */
    private function createIncidentBasedOnSRForSiebel() {
        \RightNow\Libraries\AbuseDetection::check($this->input->post('f_tok'));
        $data = json_decode($this->input->post('form'));

        if (!$data) {
            header("HTTP/1.1 400 Bad Request");
            // Pad the error message with spaces so IE will actually display it instead of a misleading, but pretty, error message.
            \RightNow\Utils\Framework::writeContentWithLengthAndExit(json_encode(\RightNow\Utils\Config::getMessage(END_REQS_BODY_REQUESTS_FORMATTED_MSG)) . str_repeat("\n", 512));
        }

        // get srID from hidden Incident CustomField siebel_sr_id
        $srID = null;
        foreach ($data as $field) {
            if ($field->name === 'Incident.CustomFields.Accelerator.siebel_sr_id') {
                $srID = $field->value;
                break;
            }
        }
        if ($srID === null) {
            $this->log->error('srID is NULL', __METHOD__, array(null, $this->contact));
            return;
        }

        // get SR from session by srID
        $sessionKey = 'sr_' . $srID;
        $srDetail = $this->session->getSessionData($sessionKey);
        if (!$srDetail) {
            $getSRResult = $this->model('custom/SiebelServiceRequest')->getSRDetailByID($srID);
            $srDetail = $getSRResult->result;
        }

        // set extra Incident fields used the value of SR
        $data[] = (object) array(
                    'name' => 'Incident.Subject',
                    'value' => $srDetail['ABSTRACT']
        );
        $data[] = (object) array(
                    'name' => 'Incident.CustomFields.Accelerator.siebel_sr_num',
                    'value' => $srDetail['SRNUMBER']
        );
        $data[] = (object) array(
                    'name' => 'Incident.CustomFields.Accelerator.siebel_serial_number',
                    'value' => $srDetail['SERIALNUMBER']
        );
        if ($srDetail['PRODUCTID']) {
            if ($rnProduct = $this->utility->getProductByPartNumber($srDetail['PRODUCTID'])) {
                $data[] = (object) array(
                            'name' => 'Incident.Product',
                            'value' => $rnProduct['ID']
                );
            } else {
                $data[] = (object) array(
                            'name' => 'Incident.CustomFields.Accelerator.cp_siebel_product_validation',
                            'value' => "Service Request Product '{$srDetail['PRODUCT']}' can't be found in RightNow"
                );
            }
        }

        // create the Incident by calling the sendFrom function in CP core
        $incidentID = $this->input->post('i_id');
        $smartAssistant = $this->input->post('smrt_asst');
        echo $this->model('Field')->sendForm($data, intval($incidentID), ($smartAssistant === 'true'))->toJson();
    }

}
