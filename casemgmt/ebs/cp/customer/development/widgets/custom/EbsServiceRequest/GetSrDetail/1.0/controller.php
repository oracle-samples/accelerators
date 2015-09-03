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
 *  SHA1: $Id: 2753afb1636999fde611d4269542f4c9ebea2739 $
 * *********************************************************************************************
 *  File: controller.php
 * ****************************************************************************************** */

namespace Custom\Widgets\EbsServiceRequest;

class GetSrDetail extends \RightNow\Libraries\Widget\Base {

    private $extServerType;

    /**
     * Key is EBS field name, while value is CP field name
     */
    private $ebsCpFieldMapping = array(
        'INCIDENT_ID' => 'SrId',
        'INCIDENT_NUMBER' => 'SrNum',
        'EXTATTRIBUTE14' => 'IncidentRef', //EBS use EXTATTRIBUTE14 to store Incident Reference number
        'CREATION_DATE' => 'RequestDate',
        'SUMMARY' => 'Subject',
        'INCIDENT_STATUS' => 'Status',
        'SERIAL_NUMBER' => 'SerialNumber',
        'PRODUCT' => 'Product',
    );

    function __construct($attrs) {
        parent::__construct($attrs);

        $this->CI->load->library('Utility');

        // get external server type
        $this->extServerType = $this->CI->model('custom/ExtIntegrationConfigVerb')->getExtServerType();

        // set up Ajax Handler
        $this->setAjaxHandlers(array(
            'get_sr_detail_ajax_endpoint' => array(
                'method' => 'handleGetSrDetailAjaxEndpoint',
                'clickstream' => 'custom_action',
            ),
        ));
    }

    function getData() {
        // get srId from URL
        $srID = \RightNow\Utils\Url::getParameter('sr_id');
        if (!$srID = intval(\RightNow\Utils\Url::getParameter('sr_id'))) {
            // get i_id from URL
            $incidentID = \RightNow\Utils\Url::getParameter('i_id');
            if (!$incidentID = intval(\RightNow\Utils\Url::getParameter('i_id'))) {
                echo $this->reportError(sprintf('Invalid ID'));
                return false;
            }
            // fetch Incident from DB by ID
            if ($incident = RNCPHP\Incident::fetch(intval($incidentID))) {
                $srID = $incident->CustomFields->Accelerator->ebs_sr_id;
            }
        }
        if (!$srID) {
            echo $this->reportError(sprintf('Invalid Service Request ID'));
            return false;
        }

        // render to js
        $this->data['js']['sr_id'] = $srID;
        $this->data['js']['ext_server_type'] = $this->extServerType;
        $this->data['js']['development_mode'] = IS_DEVELOPMENT;

        return parent::getData();
    }

    /**
     * Handle the getSrDetail Ajax request
     * @param array $params Get and Post parameters
     */
    function handleGetSrDetailAjaxEndpoint($params) {
        $srId = $params['sr_id'];
        $extServerType = $params['ext_server_type'];

        if ($extServerType === 'MOCK') {
            // Mock Server specified
        } else if ($extServerType === 'EBS') {
            echo $this->getDataForEbs($srId);
        } else {
            echo $this->reportError('Invalid External Server type');
        }
    }

    /**
     * GetData function for EBS
     * - get Service Request by calling EbsServiceRequest model
     * - prepare the data for display
     * @param int $srID Service Request ID
     * @return array Service Request data for display
     */
    private function getDataForEbs($srID) {
        if (!$srID = intval($srID)) {
            echo $this->reportError('Invalid Service Request ID');
            return false;
        }

        // get Service Request by calling SR model
        $getSRResult = $this->CI->model('custom/EbsServiceRequest')->getSRDetailByID($srID);
        if ($getSRResult->error) {
            return $this->getAjaxJsonResponse(null, $getSRResult->error->externalMessage);
        }
        // prepare the SR for display
        return $this->getAjaxJsonResponse($this->prepareSrDisplayDataForEbs($getSRResult->result), null);
    }

    /**
     * Prepare the SR data for dislay
     * - map EBS field name to CP field name
     * - if IncidentRef is empty, display 'NA'
     * - convert the date format
     * - get the ProductName through ProductId
     * - if value is null, display a empty space to keep the UI layout
     * @param array $serviceRequestDetail Service Request object return from EBS
     * @return array Service Request object for display
     */
    private function prepareSrDisplayDataForEbs($serviceRequestDetail) {
        $displayData = array();
        foreach ($serviceRequestDetail as $key => $value) {
            $key = $this->ebsCpFieldMapping[$key];

            if ($key === 'IncidentRef') {
                $value = $value === null ? 'NA' : $value;
            } else if ($key === 'RequestDate') {
                $value = date('m/d/Y h:i A', strtotime($value));
            } else if ($key === 'Product') {
                $ebsProductId = $value;
                if ($ebsProductId !== null) {
                    if ($product = $this->CI->utility->getProductByPartNumber($ebsProductId)) {
                        $value = $product[Name];
                    }
                }
            }
            //if value is null, display a empty space to keep the layout
            if ($value === null && $key !== null) {
                $value = '&nbsp;';
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
    private function getAjaxJsonResponse(array $result = null, $error = null) {
        return json_encode(array('result' => $result, 'error' => $error));
    }
}
