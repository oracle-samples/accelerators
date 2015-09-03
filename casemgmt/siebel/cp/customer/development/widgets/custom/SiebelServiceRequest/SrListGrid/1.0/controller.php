<?php

/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC Contact Center + Siebel Case Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.5 (May 2015)
 *  Siebel release: 8.1.1.15
 *  reference: 141216-000121
 *  date: Wed Sep  2 23:14:35 PDT 2015

 *  revision: rnw-15-8-fixes-release-01
 *  SHA1: $Id: 4db24ee605db9fd5f3d00264de5f30f9abe602ca $
 * *********************************************************************************************
 *  File: controller.php
 * ****************************************************************************************** */

namespace Custom\Widgets\SiebelServiceRequest;

class SrListGrid extends \RightNow\Libraries\Widget\Base {

    /**
     * External server type
     * @var string 
     */
    private $extServerType;

    /**
     * Key is CP field name, value is Siebel field name
     * @var array 
     */
    private $cpSiebelFieldMapping = array(
        'SrId' => 'ID',
        'SrNum' => 'SRNUMBER',
        'IncidentRef' => 'INTEGRATIONID',
        'IncidentID' => 'INTEGRATIONID',
        'CreatedTime' => 'CREATED',
        'Subject' => 'ABSTRACT',
        'Status' => 'STATUS',
    );

    /**
     * Define the Service Request List Display Template.
     * Key is the CP field name, while value includes the info like
     * if the field is in special data type like 'data',
     * or if the field is a link.
     * This function works for both Siebel and Mock Server
     * @var array
     */
    private $srListDisplayTemplate = array(
        'SrId' => array(
            'heading' => 'SR ID',
            'data_type' => 'String',
            'link' => false
        ),
        'SrNum' => array(
            'heading' => 'SR NUM',
            'data_type' => 'String',
            'link' => false
        ),
        'IncidentRef' => array(
            'heading' => 'Reference #',
            'data_type' => 'String',
            'link' => false
        ),
        'IncidentID' => array(
            'heading' => 'Incident ID',
            'data_type' => 'String',
            'link' => false
        ),
        'CreatedTime' => array(
            'heading' => 'Date Created',
            'data_type' => 'Date',
            'link' => false
        ),
        'Subject' => array(
            'heading' => 'Subject',
            'data_type' => 'String',
            'link' => true
        ),
        'Status' => array(
            'heading' => 'Status',
            'data_type' => 'String',
            'link' => false
        )
    );

    function __construct($attrs) {
        parent::__construct($attrs);

        // get external server type
        $this->extServerType = $this->CI->model('custom/ExtIntegrationConfigVerb')->getExtServerType();

        // set up Ajax handler
        $this->setAjaxHandlers(array(
            'get_sr_list_ajax_endpoint' => array(
                'method' => 'handleGetSRListAjaxEndpoint',
                'clickstream' => 'custom_action',
            ),
        ));
    }

    function getData() {
        $this->data['js']['ext_server_type'] = $this->extServerType;
        $this->data['js']['development_mode'] = IS_DEVELOPMENT;
    }

    /**
     * Handles the getSrList_ajax_endpoint AJAX request
     * @param array $params Get or Post parameters
     */
    function handleGetSRListAjaxEndpoint(array $params) {
        // switch between functions based on external server type
        $extServerType = $params['ext_server_type'];
        if ($extServerType === 'MOCK') {
            //Mock server specific
        } else if ($extServerType === 'SIEBEL') {
            echo $this->getDataForSiebel();
        } else {
            echo $this->getAjaxJsonResponse(null, 'Invalid External Server type');
        }
    }

    /**
     * GetData function for Siebel
     * - get SrList by calling SR model
     * - prepare SrList for display 
     * @return array ServiceRequestList data for dsiplay
     */
    private function getDataForSiebel() {
        // get attribute from widget declaration
        $displayAttrsString = $this->data['attrs']['display_attrs'];
        $displayFields = explode(',', $displayAttrsString);
        $maxRow = $this->data['attrs']['max_row'];

        // get service request list from Siebel
        $getSRListResult = $this->CI->model('custom/SiebelServiceRequest')->getSRList();

        // generate list headers
        $srListTableData = array();
        $srListTableData['headers'] = $this->getListHeader($displayFields, $this->srListDisplayTemplate);

        // check get list result    
        if (!$getSRListResult->error) {
            $srListTableData['data'] = $this->prepareSRListDisplayDataForSiebel($getSRListResult->result, $displayFields, $this->srListDisplayTemplate, $maxRow);
            return $this->getAjaxJsonResponse($srListTableData, null);
        } else {
            return $this->getAjaxJsonResponse(null, $getSRListResult->error->externalMessage);
        }
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

    /*
     * Prepare list headers
     * @param array $displayFields Fields will be displayed
     * @param array $displayTempalte Display template
     * @return array $headers Grid headers
     */

    private function getListHeader($displayFields, $displayTempalte) {
        if (is_null($displayFields) || is_null($displayTempalte)) {
            return null;
        }

        $index = 0;
        foreach ($displayFields as $field) {
            $index++;
            $headers[] = array(
                'heading' => $displayTempalte[$field]['heading'],
                'width' => null,
                'data_type' => $displayTempalte[$field]['data_type'],
                'col_id' => $index + 1,
                'order' => $index,
                'col_definition' => '',
                'visible' => 1,
            );
        }
        return $headers;
    }

    /**
     * Prepare list result for display
     * @param array $srListData List of Service Requests
     * @param array $displayFieldList Fields will be displayed in the list
     * @param array $displayTemplate List display template
     * @param int $maxRow Maximum rows of Service Request that will be displayed
     * @return array $listData Service Request list data for display 
     */
    private function prepareSRListDisplayDataForSiebel(array $srListData, array $displayFieldList, array $displayTemplate, $maxRow) {
        if (is_null($srListData)) {
            return array();
        }

        $listData = array();
        $count = -1;
        foreach ($srListData as $srItem) {
            // only display SR not associate with an Incident
            if ($srItem['INTEGRATIONID']) {
                continue;
            }

            $count++;
            if ($count === $maxRow) {
                break;
            }

            $itemData = array();
            foreach ($displayFieldList as $cpField) {
                $siebelField = $this->cpSiebelFieldMapping[$cpField];
                $value = $srItem[$siebelField];

                if ($value === '' || is_null($value)) {
                    $itemData[] = 'N/A';
                    continue;
                }

                // check if need to change data format
                if ($displayTemplate[$cpField]['data_type'] === 'Date') {
                    $itemData[] = date('m/d/Y', strtotime($value));
                    continue;
                }

                // check if need to add link
                if ($displayTemplate[$cpField]['link']) {
                    $itemData[] = "<a href='//{$_SERVER['HTTP_HOST']}/app/account/questions/detail/sr_id/{$srItem['ID']}'>{$value}";
                } else {
                    $itemData[] = $value;
                }
            }

            $listData[] = $itemData;
        }
        return $listData;
    }

}
