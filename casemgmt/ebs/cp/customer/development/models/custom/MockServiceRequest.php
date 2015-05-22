<?php

/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC WSS + EBS Case Management Accelerator
 *  link: http://www-content.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.2 (February 2015)
 *  EBS release: 12.1.3
 *  reference: 140626-000078
 *  date: Fri May 15 13:41:41 PDT 2015

 *  revision: rnw-15-2-fixes-release-01
 *  SHA1: $Id: 15409c45ddb8ff8c0ac899ffb9ab4d8ec3476e98 $
 * *********************************************************************************************
 *  File: MockServiceRequest.php
 * ****************************************************************************************** */

namespace Custom\Models;

use RightNow\Connect\v1_2 as RNCPHP;
use \PS\Log\v2 as PSLog;

require_once(APPPATH . "libraries/PSLog-2.0.php");

class MockServiceRequest extends \RightNow\Models\Base {

    private $log;
    private $contact;
    private $contactPartyID;
    private $extConfigVerb;
    private $requestTypeIDMapping;

    function __construct() {
        parent::__construct();
        $this->setUpLog();
        $this->setLoginContact();
        $this->getExtIntegrationConfig();
    }

    /**
     * Set up PSLog 
     * In development mode, show error log on the web page.
     * Otherwide, only show error log in CS console
     */
    private function setUpLog() {
        $this->log = new PSLog\Log();
        if (IS_DEVELOPMENT === true) {
            $this->log->stdOutputThreshold(PSLog\Severity::Error);
        }
    }

    /**
     * Get login Contact
     */
    private function setLoginContact() {
        $contactID = $this->CI->session ? ($this->CI->session->getProfileData('contactID') ? : 0) : 0;
        $this->contact = ($contactID === 0) ? null : RNCPHP\Contact::fetch($contactID);
        $this->contactPartyID = $this->contact->CustomFields->Accelerator->ebs_contact_party_id;
    }

    /**
     * Get EXT_INTEGRATION_CONFIG of ConfigVerb Model
     * get username, password, endpoint, ext_services from the config verb
     */
    function getExtIntegrationConfig() {
        $this->extConfigVerb = $this->CI->model('custom/ExtIntegrationConfigVerb')->getExtIntegrationCofigVerb();
        $this->requestTypeIDMapping = $this->extConfigVerb["integration"]["request_type_mapping"];
    }

    /**
     * Get Service Request by ID
     * If the SR is cached, get it from cache directly,
     * otherwise, fetch SR from EBS by sending a curl request.
     * Save the SR in the in-process cache to avoid send multiple requests to EBS.
     * Save the SR in the session for incident creation usage.
     * @param int $srID Service Request ID
     * @return array|null Get SR detail result
     */
    function getSRDetailByID($srID) {
        // check input $srID;
        if ($srID === null) {
            return null;
        }

        // check if SR cached in the in-process cache.
        // If cached, save it in the session and return the SR.
        $cacheKey = "sr_" . $srID;
        $srDetail = \RightNow\Utils\Framework::checkCache($cacheKey);
        // if cached, save or update in session, return it.
        if ($srDetail) {
            $result = array(
                "status" => true,
                "response" => $srDetail
            );
            return $result;
        }

        // if not cachaed, send request to EBS server to get the SR,
        // and save in session and cache
        // prepare the curl data
        $contactPartyID = $this->contact->CustomFields->Accelerator->ebs_contact_party_id;

        $extObj = "service_request_detail";
        $extAction = "read";
        $requestParams = array("srID" => $srID, "contactPartyID" => $contactPartyID);
        $requestFieldData = null;
        $responseFields = array(
            "CONTACTID",
            "CONTRACTID",
            "ERRORMESSAGE",
            "INCIDENTID",
            "INCIDENTOCCURREDDATE",
            "INCIDENTREF",
            "NOTES",
            "OWNER",
            "REQUESTDATE",
            "REQUESTID",
            "REQUESTNUMBER",
            "REQUESTTYPE",
            "REQUESTTYPEID",
            "SERIALNUMBER",
            "SEVERITY",
            "SEVERITYID",
            "STATUS",
            "STATUSID",
            "SUMMARY"
        );
        $errorFields = array(
            'FAULTSTRING',
            'ERRORMESSAGE');

        // send curl request
        $result = $this->CI->model('custom/MockApi')->sendSoapRequest($this->extConfigVerb, $extObj, $extAction, null, $this->contact, $requestParams, $requestFieldData, $responseFields, $errorFields, $isFromPrePageRenderHook);

        // check the result     
        if ($result !== null && $result['status'] === true) {
            // if success, save SR in the in-process cache and the session
            $srDetail = $result["response"];
            \RightNow\Utils\Framework::setCache($cacheKey, $srDetail, true);
            $sessionKey = $cacheKey;
            $this->CI->session->setSessionData(array($sessionKey => $srDetail));
            $this->log->debug(__METHOD__ . ":: Success", array(null, $this->contact), json_encode($result));
            return $result;
        }
    }

    /**
     * Get Service Request List By contact ID
     * @param int $maxRow Maximun number of Service Request that will be returned
     * @return array|null Get SR list result
     */
    public function getSRList($maxRow) {
        if ($this->contactPartyID === null) {
            $logMsg = "getSRList :: RNow Contact is not associated to Mock Server Contact";
            $this->log->notice($logMsg, array(null, $this->contact));
            return null;
        }

        // prepare curl params
        $extObj = "service_request_list";
        $extAction = "read";
        $requestParams = array(
            "contactPartyID" => $this->contactPartyID,
            "maxRow" => $maxRow);
        $requestFieldData = null;
        $responseFields = array(
            "CONTACTID",
            "CONTRACTID",
            "ERRORMESSAGE",
            "INCIDENTID",
            "INCIDENTOCCURREDDATE",
            "INCIDENTREF",
            "NOTES",
            "OWNER",
            "REQUESTDATE",
            "REQUESTID",
            "REQUESTNUMBER",
            "REQUESTTYPE",
            "REQUESTTYPEID",
            "SERIALNUMBER",
            "SEVERITY",
            "SEVERITYID",
            "STATUS",
            "STATUSID",
            "SUMMARY");
        $errorFields = array(
            'FAULTSTRING',
            'ERRORMESSAGE');

        // send curl request
        return $this->CI->model('custom/MockApi')->sendSoapRequest($this->extConfigVerb, $extObj, $extAction, null, $this->contact, $requestParams, $requestFieldData, $responseFields, $errorFields, null);
    }

    /**
     * Create new Service Request in EBS to link with the incident just created
     * @param object $incident The incident which the new Service Request associated with
     * @return int|null ID of the new Service Request
     */
    function createSR($incident) {
        // get sr request type
        if ($this->requestTypeIDMapping === null) {
            $this->log->error("Unable to get Request Type Mapping info from Config Verb", array($incident, $this->contact));
            return;
        }
        $incRequestTypeID = $incident->CustomFields->Accelerator->ebs_sr_request_type->ID;
        foreach ($this->requestTypeIDMapping as $type) {
            if ($type["inc_type_id"] === $incRequestTypeID) {
                $srRequestTypeID = $type["sr_type_id"];
                break;
            }
        }
        //prepare SR data
        $newSRData = array(
            'ContactID' => $this->contactPartyID,
            'IncidentID' => $incident->ID,
            'IncidentRef' => $incident->ReferenceNumber,
            'RequestDate' => gmdate("Y-m-d\TH:i:s", $incident->CreatedTime),
            'RequestID' => '111', // Hardcode field, it's a dummy field but required by mock server
            'RequestNumber' => '', // Hardcode field, it's a dummy field but required by mock server
            'RequestType' => $incident->CustomFields->Accelerator->ebs_sr_request_type->LookupName, // e.g.'Break/Fix Repair'
            'RequestTypeID' => $srRequestTypeID, // e.'13150'
            'SerialNumber' => $incident->CustomFields->Accelerator->ebs_serial_number,
            'Severity' => 'High', //hardcode
            'SeverityID' => '3', //hardcode
            'Status' => $incident->StatusWithType->Status->LookupName,
            'StatusID' => $incident->StatusWithType->Status->ID,
            'Summary' => $incident->Subject,
        );

        // prepare curl params
        $extObj = "service_request_detail";
        $extAction = "create";
        $requestParams = null;
        $requestFieldData = $newSRData;
        $responseFields = array(
            'REQUESTID',
            "REQUESTNUMBER");
        $errorFields = array(
            'FAULTSTRING',
            'ERRORMESSAGE');

        // send curl request
        $result = $this->CI->model('custom/MockApi')->sendSoapRequest($this->extConfigVerb, $extObj, $extAction, $incident, $this->contact, $requestParams, $requestFieldData, $responseFields, $errorFields, null);

        // check result
        if ($result !== null && $result['status'] === true) {
            $srID = $result['response']['REQUESTID'];
            $srNum = $result['response']['REQUESTNUMBER'];
            $this->log->debug("SR#" . $srID . " has been created for Incident#" . $incident->ID, array($incident, $this->contact), json_encode($result['response']));
            return array($srID, $srNum);
        }
    }

    /**
     * Update Service Request to link with Incident in CP
     * by editing the Incident.ID and Incident.REF fields
     * @param int $srID Service Request ID
     * @param array $srDetail Service Request detail data which will be updated
     * @param object $incident Incident associated with the updated Service Request
     */
    function updateSR($srID, $srDetail, $incident) {
        // prepare update SR data
        $srData = array(
            "ContactID" => $srDetail['CONTACTID'],
            "ContractID" => $srDetail['CONTRACTID'],
            "ErrorMessage" => $srDetail['ERRORMESSAGE'],
            "IncidentID" => $incident->ID,
            "IncidentOccurredDate" => $srDetail['INCIDENTOCCURREDDATE'],
            "IncidentRef" => $incident->ReferenceNumber,
            "Notes" => $srDetail['NOTES'],
            "Owner" => $srDetail['OWNER'],
            "RequestDate" => $srDetail['REQUESTDATE'],
            "RequestID" => $srID,
            "RequestNumber" => $srDetail['REQUESTNUMBER'],
            "RequestType" => $srDetail['REQUESTTYPE'],
            "RequestTypeID" => $srDetail['REQUESTTYPEID'],
            "SerialNumber" => $srDetail['SERIALNUMBER'],
            "Severity" => $srDetail['SEVERITY'],
            "SeverityID" => $srDetail['SEVERITYID'],
            "Status" => $srDetail['STATUS'],
            "StatusID" => $srDetail['STATUSID'],
            "Summary" => $srDetail['SUMMARY'],
        );

        // prepare request/response params
        $extObj = "service_request_detail";
        $extAction = "update";
        $requestParams = null;
        $requestFieldData = $srData;
        $responseFields = array("REQUESTID");
        $errorFields = array(
            'FAULTSTRING',
            'ERRORMESSAGE');

        // send curl request
        $result = $this->CI->model('custom/MockApi')->sendSoapRequest($this->extConfigVerb, $extObj, $extAction, $incident, $this->contact, $requestParams, $requestFieldData, $responseFields, $errorFields, null);

        // check result
        if ($result !== null && $result['status'] === true) {
            $this->log->debug("SR#" . $srID . " has been updated in EBS", array($incident, $this->contact));
        }
    }

    /**
     * Create new Interaction in Mock Server according the thread of the Incident in CP
     * @param int $srID Service Request ID
     * @param object $incident The incident which associate with the Service Request
     * @return int|null ID of the new interaction
     */
    function createInteraction($srID, $incident) {
        //get Incident.threads and create new interaction in EBS for each threads
        $threadArray = $incident->Threads;
        $thread = $threadArray[0];

        // prepare new interaction data
        $interactionData = array(
            'Author' => $thread->Contact->LookupName,
            'Channel' => $thread->Channel->LookupName,
            'Content' => $thread->Text,
            'Created' => gmdate("Y-m-d\TH:i:s", $thread->CreatedTime),
            'SrID' => $srID,
        );

        // prepare request / response params
        $extObj = "service_request_interaction";
        $extAction = "create";
        $requestParams = null;
        $requestFieldData = $interactionData;
        $responseFields = array('INTERACTIONID');
        $errorFields = array(
            'FAULTSTRING',
            'ERRORMESSAGE');

        // send curl request
        $result = $this->CI->model('custom/MockApi')->sendSoapRequest($this->extConfigVerb, $extObj, $extAction, $incident, $this->contact, $requestParams, $requestFieldData, $responseFields, $errorFields, null);

        // check result
        if ($result !== null && $result["status"] === true) {
            $noteID = $result['response']['INTERACTIONID'];
            $this->log->debug("Interaction#{$noteID} has been created for SR#{$srID}", array($incident, $this->contact), json_encode($result));
            return $noteID;
        }
    }

    /**
     * Get Interaction of a Service Request by SrID
     * @param int $srID Service Request ID
     * @param int $maxRow Maxmum number of interactions that will be returned
     * @return array|null Get SR interaction result
     */
    public function getSRInteraction($srID, $maxRow) {
        // prepare request and response params
        $extObj = "service_request_interaction";
        $extAction = "read";
        $requestParams = array(
            "srID" => $srID,
            "contactPartyID" => $this->contactPartyID,
            "maxRows" => $maxRow
        );
        $requestFieldData = null;
        $responseFields = array(
            "AUTHOR",
            "CHANNEL",
            "CONTENT",
            "CREATED",
            "INTERACTIONID",
            "SRID");

        $errorFields = array(
            "FAULTSTRING",
            "ERRORMESSAGE");

        // send curl request
        return$this->CI->model('custom/MockApi')->sendSoapRequest($this->extConfigVerb, $extObj, $extAction, null, $this->contact, $requestParams, $requestFieldData, $responseFields, $errorFields, null);
    }

}
