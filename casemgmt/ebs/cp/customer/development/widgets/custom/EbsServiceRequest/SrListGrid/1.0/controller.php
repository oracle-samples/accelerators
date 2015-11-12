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
 *  date: Thu Nov 12 00:52:38 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: f16c7235dc26060b2e8f981eb24d74647460fac5 $
 * *********************************************************************************************
 *  File: controller.php
 * ****************************************************************************************** */

namespace Custom\Widgets\EbsServiceRequest;

class SrListGrid extends \RightNow\Libraries\Widget\Base {

    /**
     * External server type
     * @var string 
     */
    private $extServerType;

     /**
     * Key is CP field, value is EBS field
     * @var array 
     */
    private $cpEbsFieldMapping = array(
        'SrId' => 'INCIDENT_ID',
        'SrNum' => 'INCIDENT_NUMBER',
        'IncidentRef' => 'ATTRIBUTE14',
        'IncidentID' => 'ATTRIBUTE15',
        'CreatedTime' => 'CREATION_DATE',
        'Subject' => 'SUMMARY',
        'Status' => 'INCIDENT_STATUS',
    );

    /**
     * Define the Service Request List Display Template.
     * Key is the CP field name, while value includes the info like
     * if the field is in special data type like 'data',
     * or if the field is a link.
     * This function works for both EBS and Mock Server
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
     * @param array $params Get / Post parameters
     */
    function handleGetSRListAjaxEndpoint(array $params) {
        // switch between functions based on external server type
        $extServerType = $params['ext_server_type'];
        if ($extServerType === 'MOCK') {
            // Mock Server specified
        } else if ($extServerType === 'EBS') {
            echo $this->getDataForEBS();
        } else {
            echo $this->getAjaxJsonResponse(null, 'Invalid External Server type');
        }
    }

    /**
     * Compose the Ajax response in Json format
     * @param array $result Result of the ajax results
     * @param string $error Error message of the ajax requests
     * @return json  Ajax response in Json format
     */
    private function getAjaxJsonResponse(array $result = null, $error = null) {
        return json_encode(array('result' => $result, 'error' => $error));
    }

    /**
     * GetData function for EBS
     * - get SrList by calling SR model
     * - prepare SrList for display 
     * @return array Json-format ServiceRequestList data for dsiplay
     */
    function getDataForEbs() {
        // get attribute from widget declaration
        $displayAttrsString = $this->data['attrs']['display_attrs'];
        $displayFields = explode(',', $displayAttrsString);
        $maxRow = $this->data['attrs']['max_row'];

        // get service request list from EBS
        $getSRListResult = $this->CI->model('custom/EbsServiceRequest')->getSRList();

        // generate list headers
        $srListTableData = array();
        $srListTableData['headers'] = $this->getListHeader($displayFields, $this->srListDisplayTemplate);

        // check get list result    
        if ($getSRListResult->error) {
            return $this->getAjaxJsonResponse(null, $getSRListResult->error->externalMessage);
        } else {
            $srListTableData['data'] = $this->prepareSRListDisplayDataForEbs($getSRListResult->result, $displayFields, $this->srListDisplayTemplate, $maxRow);
            return $this->getAjaxJsonResponse($srListTableData, null);
        }
    }

    /**
     * Prepare list result
     * change date time formate or add link if needed.
     * @param array $srListData List of Service Request
     * @param array $displayFieldList Field will be displayed in the list
     * @param array $displayTemplate List display template
     * @param int $maxRow Maximum rows of Service Request that will be displayed
     * @return array $listData Service Request list data for display 
     */
    private function prepareSRListDisplayDataForEbs(array $srListData, array $displayFieldList, array $displayTemplate, $maxRow) {
        if (is_null($srListData)) {
            return array();
        }

        $listData = array();
        $host = $_SERVER['HTTP_HOST'];
        $count = -1;
        foreach ($srListData as $srItem) {
            // only display SR not aossociate with an Incident
            if ($srItem['EXTATTRIBUTE14'] || $srItem['EXTATTRIBUTE15']) {
                continue;
            }

            $count++;
            if ($count >= $maxRow) {
                break;
            }

            $itemData = null;
            foreach ($displayFieldList as $cpField) {
                // get mock field name
                $ebsField = $this->cpEbsFieldMapping[$cpField];

                // get value
                $value = $srItem[$ebsField];

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
                if ($displayTemplate[$cpField]['link'] === true) {
                    $link = "<a href='//{$host}/app/account/questions/detail/sr_id/{$srItem['INCIDENT_ID']}";
                    if($srItem['INCIDENT_STATUS'] === 'Closed'){
                        $link .= '/readonly/1';
                    }
                    $link .= "'>";
                            
                    $itemData[] = $link . $value;
                    continue;
                }

                $itemData[] = $value;
            }

            $listData[] = $itemData;
        }
        return $listData;
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
}
