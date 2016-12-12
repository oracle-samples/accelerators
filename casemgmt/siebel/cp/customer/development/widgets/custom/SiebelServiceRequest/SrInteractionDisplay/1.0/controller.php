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
 *  date: Mon Nov 30 20:14:22 PST 2015

 *  revision: rnw-15-11-fixes-release-2
 *  SHA1: $Id: dee18c1f32aefa370a4a52197baaf1213aad612d $
 * *********************************************************************************************
 *  File: controller.php
 * ****************************************************************************************** */

namespace Custom\Widgets\SiebelServiceRequest;

use RightNow\Connect\v1_2 as RNCPHP,
    RightNow\Utils\Url as URL;

class SrInteractionDisplay extends \RightNow\Libraries\Widget\Base {

    /**
     * External server type
     * @var string
     */
    private $extServerType;

    /**
     * Key is Siebel field name, while value is CP field name
     * @var array 
     */
    private $siebelCpFieldMapping = array(
        'CREATEDBYNAME' => 'Author',
        'CREATED' => 'CreatedTime',
        'COMMENT' => 'Text',
        'TYPE' => 'Channel'
    );

    /**
     * Define the public note type that will be displayed
     * @var type 
     */
    private $siebelNoteTypes = array('Email', 'Call', 'Fax', 'Letter - Inbound', 'Web - Inbound', 'Chat - Transfer', 'Other');

    function __construct($attrs) {
        parent::__construct($attrs);

        // get external server type
        $this->extServerType = $this->CI->model('custom/ExtIntegrationConfigVerb')->getExtServerType();

        // set up Ajax handlers
        $this->setAjaxHandlers(array(
            'get_interaction_ajax_endpoint' => array(
                'method' => 'handleGetInteractionAjaxEndpoint',
                'clickstream' => 'custom_action',
            ),
        ));
    }

    /**
     * Handles the default_ajax_endpoint AJAX request
     * @param array $params Get / Post parameters
     */
    function handleGetInteractionAjaxEndpoint(array $params) {
        $serviceRequestID = $params['sr_id'];
        $extServerType = $params['ext_server_type'];

        // switch to different function based on external server type
        if ($extServerType === 'MOCK') {
            // Mock Server specific
        } else if ($extServerType === 'SIEBEL') {
            echo $this->getDataForSiebel($serviceRequestID);
        } else {
            echo $this->getAjaxJsonResponse(null, 'Invalid External Server type');
        }
    }

    /**
     * GetData function of the widget
     * @return null
     */
    function getData() {
        // get srId from URL
        $srID = URL::getParameter('sr_id');
        if (!$srID) {
            // get srID from incident
            if (!$incidentID = intval(URL::getParameter('i_id'))) {
                echo $this->reportError('Invalid ID');
                return;
            }
            $incident = RNCPHP\Incident::fetch(intval($incidentID));
            if (!is_null($incident)) {
                $srID = $incident->CustomFields->Accelerator->siebel_sr_id;
            }
        }

        // render data to javascript
        $this->data['js']['sr_id'] = $srID;
        $this->data['js']['ext_server_type'] = $this->extServerType;
        $this->data['js']['development_mode'] = IS_DEVELOPMENT;
    }

    /**
     * GetData function for Siebel
     * -  get interactions by calling SR model
     * -  prepare interaction data for display
     * @param int $srID Service Request ID
     * @return array $displayData Service Request ineractions for display   
     */
    private function getDataForSiebel($srID) {
        if (!$srID) {
            return $this->getAjaxJsonResponse(null, 'Unable to find the associated Service Request ID of the Incident.');
        }

        // get maxRows attributes
        $maxRows = $this->data['attrs']['max_rows'];

        // get interactions
        $getThreadResult = $this->CI->model('custom/SiebelServiceRequest')->getCommunicationActions($srID, $maxRows);
        if ($getThreadResult->error) {
            return $this->getAjaxJsonResponse(null, $getThreadResult->error->externalMessage);
        }

        // return json format interaction display data
        return $this->getAjaxJsonResponse($this->prepareDisplayDataForSiebel($getThreadResult->result, $maxRows), null);
    }

    /**
     * Get Service Request field of Siebel SR by CP tag name
     * - map the list data from Siebel field name to cp field name.
     * - change date time format if need.
     * @param array $siebelList Interaction list from mock server
     * @return array $cpList Interaction list for CP display
     */
    private function prepareDisplayDataForSiebel(array $siebelList = null) {
        if ($siebelList === null) {
            return null;
        }

        $cpList = array();
        foreach ($siebelList as $siebelItem) {
            // only display public note type
            if (!in_array($siebelItem['TYPE'], $this->siebelNoteTypes) || $siebelItem['PRIVATE'] === 'true') {
                continue;
            }

            $cpItem = array();
            foreach ($siebelItem as $siebelKey => $siebelValue) {
                $cpKey = $this->siebelCpFieldMapping[$siebelKey];
                if ($cpKey === 'CreatedTime') {
                    // convert date display format
                    $siebelValue = date('m/d/Y h:i A', strtotime($siebelValue));
                }
                $siebelValue = htmlspecialchars($siebelValue);
                $cpItem[$cpKey] = str_replace("\n", '<br>', $siebelValue);
            }
            array_push($cpList, $cpItem);
        }

        return $cpList;
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
