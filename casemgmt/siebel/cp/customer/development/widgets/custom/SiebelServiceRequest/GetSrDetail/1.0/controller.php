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
 *  date: Mon Nov 30 20:14:21 PST 2015

 *  revision: rnw-15-11-fixes-release-2
 *  SHA1: $Id: 840a4f759aceb15fed62d0cb1b7532df821dcebb $
 * *********************************************************************************************
 *  File: controller.php
 * ****************************************************************************************** */

namespace Custom\Widgets\SiebelServiceRequest;

class GetSrDetail extends \RightNow\Libraries\Widget\Base {

    private $extServerType;

    /**
     * Key is Siebel Service Request field name, while value is corresponding CP field name
     */
    private $siebelCPFieldMapping = array(
        'ID' => 'SrId',
        'SRNUMBER' => 'SrNum',
        'INTEGRATIONID' => 'IntegrationID',
        'CREATED' => 'RequestDate',
        'ABSTRACT' => 'Subject',
        'STATUS' => 'Status',
        'SERIALNUMBER' => 'SerialNumber',
        'PRODUCT' => 'Product',
    );

    function __construct($attrs) {
        parent::__construct($attrs);

        // get external server type
        $this->extServerType = $this->CI->model('custom/ExtIntegrationConfigVerb')->getExtServerType();

        // set up Ajax Hander
        $this->setAjaxHandlers(array(
            'get_sr_detail_ajax_endpoint' => array(
                'method' => 'handleGetSRDetailAjaxEndpoint',
                'clickstream' => 'custom_action',
            ),
        ));
    }

    function getData() {
        // get srId from URL
        if (!$srID = \RightNow\Utils\Url::getParameter('sr_id')) {
            echo $this->reportError('Invalid Service Request ID');
            return false;
        }

        // render to js
        $this->data['js']['sr_id'] = $srID;
        $this->data['js']['ext_server_type'] = $this->extServerType;
        $this->data['js']['development_mode'] = IS_DEVELOPMENT;
    }

    /**
     * Handles the getSrDetail AJAX request
     * @param array $params Get and Post parameters
     */
    function handleGetSRDetailAjaxEndpoint(array $params) {
        $srId = $params['sr_id'];
        $extServerType = $params['ext_server_type'];

        // switch between different server type
        if ($extServerType === 'MOCK') {
            // Mock server specific
        } else if ($extServerType === 'SIEBEL') {
            echo $this->getDataForSiebel($srId);
        } else {
            echo $this->getAjaxJsonResponse(null, 'Invalid External Server type');
        }
    }

    /**
     * GetData function for Siebel
     * - get SR by calling SiebelServiceRequest model
     * - prepare the SR for display
     * @param int $srID Service Request ID
     * @return array Service Request data for display
     */
    private function getDataForSiebel($srID) {
        // check srId
        if (!$srID) {
            return $this->getAjaxJsonResponse(null, 'Invalid Service Request ID');
        }

        // get Service Request Detail by calling SR model
        $getSRResult = $this->CI->model('custom/SiebelServiceRequest')->getSRDetailByID($srID);
        if ($getSRResult->error) {
            return $this->getAjaxJsonResponse(null, $getSRResult->error->externalMessage);
        }
        // prepare the SR data for display
        return $this->getAjaxJsonResponse($this->prepareSrDisplayDataForSiebel($getSRResult->result), null);
    }

    /**
     * Prepare the SR data for dislay
     * - map Siebel field name to CP field name
     * - if IncidentRef is empty, display 'NA'
     * - convert the date format
     * - if value is null, display a empty space to keep the layout
     * @param array $serviceRequestDetail Service Request object return from Siebel
     * @return array Service Request object for display
     */
    private function prepareSrDisplayDataForSiebel(array $serviceRequestDetail) {
        $displayData = array();
        foreach ($serviceRequestDetail as $key => $value) {
            $key = $this->siebelCPFieldMapping[$key];

            // use Description to store IncidentId and IncidentRef
            if ($key === 'IntegrationID' && $value !== null) {
                $value = explode(",", $value);
                $incidentID = $value[0];
                $incidentRef = $value[1];
                $displayData['IncidentID'] = $incidentID === null ? 'NA' : $incidentID;
                $displayData['IncidentRef'] = $incidentRef === null ? 'NA' : $incidentRef;
            } else if ($key === 'RequestDate') {
                $value = date('m/d/Y h:i A', strtotime($value));
            }
            // escape html chracters
            if ($value !== null) {
                $value = htmlspecialchars($value);
            }

            $displayData[$key] = $value;
        }

        return $displayData;
    }

    /**
     * Compose the Ajax response in Json format
     * @param array $result Result of the ajax results
     * @param string $error Error message of the Ajax requests
     * @return json Ajax response in Json format
     */
    private function getAjaxJsonResponse($result, $error) {
        return json_encode(array('result' => $result, 'error' => $error));
    }

}
