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
 *  SHA1: $Id: a6b1bcd9d7011921779fe1248511696c4bdc0e43 $
 * *********************************************************************************************
 *  File: controller.php
 * ****************************************************************************************** */

namespace Custom\Widgets\EbsServiceRequest;

use RightNow\Connect\v1_2 as RNCPHP,
    RightNow\Utils\Url as URL;

class SrInteractionDisplay extends \RightNow\Libraries\Widget\Base {

    /**
     * External server type
     * @var string
     */
    private $extServerType;

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
     * Handle the AJAX request
     * @param array $params Get and Post parameters
     */
    function handleGetInteractionAjaxEndpoint(array $params) {
        $srID = $params['sr_id'];
        $extServerType = $params['ext_server_type'];

        // switch to different function based on external server type
        if ($extServerType === 'MOCK') {
            // Mock Server specified
        } else if ($extServerType === 'EBS') {
            echo $this->getDataForEbs($srID);
        } else {
            echo $this->reportError('Invalid External Server type');
        }
    }

    /**
     * GetData function of the widget
     * @return null
     */
    function getData() {
        // get srID
        if (!$srID = intval(URL::getParameter('sr_id'))) {
            $incidentID = URL::getParameter('i_id');
            if (!$incidentID = intval(URL::getParameter('i_id'))) {
                echo $this->reportError('Invalid i_id');
                return;
            }
            $incident = RNCPHP\Incident::fetch(intval($incidentID));
            if (!is_null($incident)) {
                $srID = $incident->CustomFields->Accelerator->ebs_sr_id;
            }
        }

        // render data to javascript
        $this->data['js']['sr_id'] = $srID;
        $this->data['js']['ext_server_type'] = $this->extServerType;
        $this->data['js']['development_mode'] = IS_DEVELOPMENT;
    }

    /**
     * GetData function for EBS
     * -  get interactions by calling the SR model
     * -  prepare interaction data for display
     * @param int $srID Service Request ID
     * @return json $displayData Json-formart Service Request ineractions for display   
     */
    private function getDataForEbs($srID) {
        if ($srID === null) {
            echo $this->reportError('Invalid Service Request ID');
            return false;
        }

        // get interactions
        $getSRNotesResult = $this->CI->model('custom/EbsServiceRequest')->getSRNotes($srID);

        if ($getSRNotesResult->error) {
            return $this->getAjaxJsonResponse(null, $getSRNotesResult->error->externalMessage);
        }

        // return json format interaction data for display
        return $this->getAjaxJsonResponse($this->prepareDisplayDataForEbs($getSRNotesResult->result, $this->data['attrs']['maxrows']), null);
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
     * Prepare data for display
     * - map the data from ebs field name to cp field name
     * - change date time format if need
     * @param array $ebsList Interaction list from EBS server
     * @param int $maxRows The maxmum rows of thread that will displayed
     * @return array Interaction list for CP display
     */
    private function prepareDisplayDataForEbs(array $ebsList, $maxRows) {
        if ($ebsList === null) {
            return null;
        }

        $cpList = array();
        $count = -1;
        foreach ($ebsList as $item) {
            // filter out private note
            if ($item['NOTE_STATUS'] === 'P') {
                continue;
            }

            $count++;
            if ($count >= $maxRows) {
                break;
            }

            $cpItem = array();

            // note detail
            $noteDetail = htmlspecialchars($item['NOTES_DETAIL']);
            $noteDetail = str_replace("\n", '<br>', $noteDetail);
            $cpItem['Text'] = $noteDetail;

            // entry type
            $cpItem['EntryType'] = ucfirst(strtolower($item['CREATED_BY_USER_TYPE']));

            // channel
            $cpItem['Channel'] = $this->getNoteChannel($item);

            // created time
            $ebsTime = $item['CREATION_DATE']; // row data use local time
            $cpItem['CreatedTime'] = date('m/d/Y h:i A', strtotime($ebsTime)); // display the local time

            array_push($cpList, $cpItem);
        }

        return $cpList;
    }

    /**
     * Get Note author and channel info
     * @param array $note Service Request Note data
     * @return array List of [author, channel]
     */
    private function getNoteChannel(array $note) {
        $noteExtraInfo = $note['NOTES'];
        $noteExtraInfo = str_replace('\n', ' ', $noteExtraInfo);
        $noteExtraInfo = str_replace('\t', ' ', $noteExtraInfo);
        $pos = strpos($noteExtraInfo, 'via');
        if ($pos !== false) {
            $channel = substr($noteExtraInfo, $pos + 5, -1);
            return $channel;
        } else {
            return null;
        }
    }
}
