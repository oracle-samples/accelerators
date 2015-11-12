<?php

/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + EBS Enhancement
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  EBS release: 12.1.3
 *  reference: 150505-000099, 150420-000127
 *  date: Thu Nov 12 00:52:34 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: cd1cf238139dc3d27eb3ea988f74d90e44a6e29c $
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
     * Handle the form to create a new Incident to associate with a
     * Service Request in 3rd party based on the external server type
     */
    function sendFormToCreateIncidentToLinkWithSR() {
        if ($this->extServerType === 'EBS') {
            $this->sendFormToCreateIncidentToLinkWithSRForEbs();
        } else if ($this->extServerType === 'MOCK') {
            // Mock Server specified
        }
    }

    /**
     * Handle the request to create a new Incident in CP using SR data
     * @return null
     */
    private function sendFormToCreateIncidentToLinkWithSRForEbs() {
        \RightNow\Libraries\AbuseDetection::check($this->input->post('f_tok'));
        $data = json_decode($this->input->post('form'));

        if (!$data) {
            header('HTTP/1.1 400 Bad Request');
            // Pad the error message with spaces so IE will actually display it instead of a misleading, but pretty, error message.
            \RightNow\Utils\Framework::writeContentWithLengthAndExit(json_encode(\RightNow\Utils\Config::getMessage(END_REQS_BODY_REQUESTS_FORMATTED_MSG)) . str_repeat('\n', 512));
        }

        // get srID from the hidden Incident Custom Field 'ebs_sr_id'
        $srID = null;
        foreach ($data as $field) {
            if ($field->name === 'Incident.CustomFields.Accelerator.ebs_sr_id') {
                $srID = $field->value;
                break;
            }
        }
        if ($srID === null) {
            $this->log->error('ebs_sr_id is null', __METHOD__, array(null, $this->contact));
            return;
        }

        // get SR from session by srID
        $sessionKey = 'sr_' . $srID;
        $srDetail = $this->session->getSessionData($sessionKey);
        if (!$srDetail) {
            $getSRResult = $this->model('custom/EbsServiceRequest')->getSRDetailByID($srID);
            if ($getSRResult->error) {
                $this->log->error("Unable to get SR#{$srID}", __METHOD__, array(null, $this->contact));
                return null;
            }
            $srDetail = $getSRResult->result;
        }

        // set extra Incident fields used the value from SR
        $data[] = (object) array(
                    'name' => 'Incident.Subject',
                    'value' => $srDetail['SUMMARY']
        );
        $data[] = (object) array(
                    'name' => 'Incident.CustomFields.Accelerator.ebs_sr_num',
                    'value' => $srDetail['INCIDENT_NUMBER']
        );
        $data[] = (object) array(
                    'name' => 'Incident.CustomFields.Accelerator.ebs_serial_number',
                    'value' => $srDetail['SERIAL_NUMBER']
        );
        $data[] = (object) array(
                    'name' => 'Incident.CustomFields.Accelerator.ebs_sr_owner_id',
                    'value' => $srDetail['SR_OWNER_ID']
        );
        if ($srDetail['PRODUCT']) {
            if ($rnProduct = $this->utility->getProductByPartNumber($srDetail['PRODUCT'])) {
                $data[] = (object) array(
                            'name' => 'Incident.Product',
                            'value' => $rnProduct['ID']
                );
            } else {
                $data[] = (object) array(
                            'name' => 'Incident.CustomFields.Accelerator.cp_ebs_product_validation',
                            'value' => "Service Request Product '{$srDetail['PRODUCT']}' can't be found in RightNow"
                );
            }
        }
        // add SR request type
        $srTypeMapping = $this->model('custom/ExtIntegrationConfigVerb')->getExtRequestTypeMapping();
        if ($srTypeMapping === null) {
            $this->log->error('Unable to get request type mapping from Config Verb', __METHOD__, array(null, $this->contact));
            return;
        }
        $srRequestType = $srDetail['INCIDENT_TYPE_ID'];
        foreach ($srTypeMapping as $type) {
            if (intval($type['sr_type_id']) === intval($srRequestType)) {
                $data[] = (object) array(
                            'name' => 'Incident.CustomFields.Accelerator.ebs_sr_request_type',
                            'value' => $type['inc_type_id']
                );
                break;
            }
        }

        // create the Incident in RNW
        $incidentID = $this->input->post('i_id');
        $smartAssistant = $this->input->post('smrt_asst');
        echo $this->model('Field')->sendForm($data, intval($incidentID), ($smartAssistant === 'true'))->toJson();
    }

}
