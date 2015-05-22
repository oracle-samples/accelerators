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
 *  date: Fri May 15 13:41:40 PDT 2015

 *  revision: rnw-15-2-fixes-release-01
 *  SHA1: $Id: d11e5feaaffa24c8dd38034d95c9ef2307ae11b8 $
 * *********************************************************************************************
 *  File: EbsServiceRequest.php
 * ****************************************************************************************** */

namespace Custom\Models;

use RightNow\Connect\v1_2 as RNCPHP;

class EbsServiceRequest extends \RightNow\Models\Base {

    private $log;
    private $contact;
    private $extConfigVerb;

    function __construct() {
        parent::__construct();

        $this->CI->load->library('Utility');

        $this->log = $this->CI->utility->getLogWrapper();
        $this->contact = $this->CI->utility->getLoginContact();
        $this->extConfigVerb = $this->CI->model('custom/ExtIntegrationConfigVerb')->getExtIntegrationCofigVerb(false);
    }

    /**
     * Get Service Request by ID.
     * If SR is cached, get from cache directly,
     * otherwise, fetch SR from EBS by sending a cURL request.
     * Save the SR in the in-process cache to avoid sending multiple requests to EBS.
     * Save the SR in the session for incident creation usage.
     * @param int $srID Service Request ID
     * @return RNCPHP\RNObject Get SR result
     */
    function getSRDetailByID($srID) {
        if ($srID === null) {
            return $this->getResponseObject(null, null, 'Error occurs when getSRbyID :: invalid sr_id');
        }

        // check if the SR has been cached in the in-process cache
        // If cached, save it in the session and return the SR
        $cacheKey = 'sr_' . $srID;
        $srDetail = \RightNow\Utils\Framework::checkCache($cacheKey);
        if ($srDetail) {
            return $this->getResponseObject($srDetail, 'is_array');
        }

        // if not cachaed, send request to EBS server to get the SR,
        // and save in session and cache
        $extObj = 'service_request_detail';
        $extAction = 'read';
        $requestParams = array('srID' => $srID);
        $requestEntries = null;
        $responseFields = array(
            'INCIDENT_NUMBER',
            'INCIDENT_ID',
            'SUMMARY',
            'INCIDENT_TYPE',
            'INCIDENT_TYPE_ID',
            'INCIDENT_STATUS',
            'INCIDENT_STATUS_ID',
            'INCIDENT_SEVERITY',
            'INCIDENT_SEVERITY_ID',
            'CREATION_DATE',
            'OBJECT_VERSION_NUMBER',
            'SR_OWNER_ID',
            'CUSTOMER_TYPE',
            'SERIAL_NUMBER',
            'PRODUCT',
            'CUSTOMER_ID',
            'EXTATTRIBUTE13',
            'EXTATTRIBUTE14',
            'EXTATTRIBUTE15',
            'CONTACT_PARTY_ID');
        $errorFields = array(
            'FAULTSTRING',
            'X_MSG_DATA');

        // send curl request
        $getSRResult = $this->CI->model('custom/EbsApi')->sendSoapRequest($this->extConfigVerb, $extObj, $extAction, null, $this->contact, $requestParams, $requestEntries, $responseFields, $errorFields);

        // check the result. if success, save SR in the in-process cache and the session
        if ($getSRResult->error === null) {
            \RightNow\Utils\Framework::setCache($cacheKey, $getSRResult->result, true);
            $sessionKey = $cacheKey;
            $this->CI->session->setSessionData(array($sessionKey => $getSRResult->result));
        }

        return $getSRResult;
    }

    /**
     * Get a list of a Service Request by ContactParyID
     * @return RNCPHP\RNObject Get SR list result
     */
    public function getSRList() {
        $contactPartyID = ($this->contact !== null) ? $this->contact->CustomFields->Accelerator->ebs_contact_party_id : null;
        if ($contactPartyID === null) {
            $errorMsg = 'Error occurs when getSRList :: EBS ContactPartyID is empty';
            $this->log->error($errorMsg, __METHOD__, array(null, $this->contact));
            return $this->getResponseObject(null, null, $errorMsg);
        }

        // prepare curl params
        $extObj = 'service_request_list';
        $extAction = 'read';
        $requestParams = array('contactPartyID' => $contactPartyID);
        $requestEntries = null;
        $responseFields = array(
            'INCIDENT_NUMBER',
            'INCIDENT_ID',
            'SUMMARY',
            'INCIDENT_TYPE_ID',
            'INCIDENT_STATUS',
            'INCIDENT_STATUS_ID',
            'INCIDENT_SEVERITY',
            'INCIDENT_SEVERITY_ID',
            'CREATION_DATE',
            'EXTATTRIBUTE14',
            'EXTATTRIBUTE15',
            'CONTACT_PARTY_ID');
        $errorFields = array(
            'FAULTSTRING',
            'X_MSG_DATA');

        // send curl request
        return $this->CI->model('custom/EbsApi')->sendSoapRequest($this->extConfigVerb, $extObj, $extAction, null, $this->contact, $requestParams, $requestEntries, $responseFields, $errorFields);
    }

    /**
     * Create new Service Request
     * @param RNCPHP\Incident $incident The incident which the new Service Request will associate with
     * @param array $ebsItemInstance Item_instance data from EBS
     * @return array|null Array conatins $srNum and $srID
     */
    function createSR(RNCPHP\Incident $incident, array $ebsItemInstance = null) {
        // prepare curl params
        $contactPartyID = ($this->contact !== null) ? $this->contact->CustomFields->Accelerator->ebs_contact_party_id : null;
        $extObj = 'service_request_detail';
        $extAction = 'create';
        $requestParams = array('contactPartyID' => $contactPartyID);
        $requestEntries = array(
            'REQUEST_DATE' => gmdate('Y-m-d\TH:i:s', $incident->CreatedTime),
            'TYPE_ID' => $this->getEbsRequestType($incident),
            'STATUS_ID' => 1,
            'SEVERITY_ID' => null,
            'SEVERITY_NAME' => null,
            'OWNER_ID' => $incident->CustomFields->Accelerator->ebs_sr_owner_id,
            'SUMMARY' => $incident->Subject,
            'CALLER_TYPE' => 'ORGANIZATION',
            'CUSTOMER_ID' => $this->contact->CustomFields->Accelerator->ebs_contact_org_id,
            'CUSTOMER_PRODUCT_ID' => $ebsItemInstance['INSTANCE_ID'],
            'INVENTORY_ORG_ID' => 204,
            'CURRENT_SERIAL_NUMBER' => $incident->CustomFields->Accelerator->ebs_serial_number,
            'EXTERNAL_ATTRIBUTE_13' => $this->CI->model('custom/ExtIntegrationConfigVerb')->getRntHost(),
            'EXTERNAL_ATTRIBUTE_14' => $incident->ReferenceNumber,
            'EXTERNAL_ATTRIBUTE_15' => $incident->ID,
            'public_COMMENT_FLAG' => null,
            'INCIDENT_OCCURRED_DATE' => gmdate('Y-m-d\TH:i:s', $incident->CreatedTime),
        );
        $responseFields = array(
            'REQUEST_NUMBER',
            'REQUEST_ID');
        $errorFields = array(
            'FAULTSTRING',
            'X_MSG_DATA');

        // send curl request
        $result = $this->CI->model('custom/EbsApi')->sendSoapRequest($this->extConfigVerb, $extObj, $extAction, $incident, $this->contact, $requestParams, $requestEntries, $responseFields, $errorFields);

        // return srID and srNum if success
        if (!$result->error) {
            $srNum = $result->result['REQUEST_NUMBER'];
            $srID = $result->result['REQUEST_ID'];
            $this->log->debug("SR#{$srID} has been created for Incident#{$incident->ID}", __METHOD__, array($incident, $this->contact));
            return array($srID, $srNum);
        }

        return null;
    }

    /**
     * Get EBS request type for SR creation.
     * For example, incident request 'break/fix'(id = 1) map to the sr request type 13150
     * @param RNCPHP\Incident $incident RightNow Incidentobject
     * @return string|null EBS request type
     */
    private function getEbsRequestType(RNCPHP\Incident $incident) {
        // get request type mapping from config verb
        $this->requestTypeMapping = $this->extConfigVerb['integration']['request_type_mapping'];
        if ($this->requestTypeMapping === null) {
            $this->log->error('Unable to get request type mapping from Config Verb', __METHOD__, array($incident, $this->contact));
            return;
        }

        // return the ebs request type
        $incRequestType = $incident->CustomFields->Accelerator->ebs_sr_request_type->ID;
        foreach ($this->requestTypeMapping as $type) {
            if ($type['inc_type_id'] === $incRequestType) {
                return $type['sr_type_id'];
            }
        }
        return null;
    }

    /**
     * Update Service Request to link with an incident in CP
     * by editing the IncidentID and IncidentREF fields.
     * In the current EBS API,
     * use EXTATTRIBUTE13 to store current rnt host,
     * use EXTERNAL_ATTRIBUTE_14 to store IncidentRef,
     * and use EXTERNAL_ATTRIBUTE_15 to store incidentID.
     * @param int $srID Service Request ID
     * @param array $srDetail Service Request detail
     * @param RNCPHP\Incident $incident Incident related to this Service Request
     */
    function updateSR($srID, array $srDetail, RNCPHP\Incident $incident) {
        // prepare curl params
        $srObjVerNum = $srDetail['OBJECT_VERSION_NUMBER'];
        $extObj = 'service_request_detail';
        $extAction = 'update';
        $requestParams = array(
            'srID' => $srID,
            'srObjVerNum' => $srObjVerNum);
        $requestEntries = array(
            'REQUEST_DATE' => $srDetail['CREATION_DATE'],
            'TYPE_ID' => $srDetail['INCIDENT_TYPE_ID'],
            'STATUS_ID' => $srDetail['INCIDENT_STATUS_ID'],
            'SEVERITY_ID' => $srDetail['INCIDENT_SEVERITY_ID'],
            'OWNER_ID' => $srDetail['SR_OWNER_ID'],
            'SUMMARY' => $srDetail['SUMMARY'],
            'CALLER_TYPE' => $srDetail['CUSTOMER_TYPE'],
            'CUSTOMER_ID' => $srDetail['CUSTOMER_ID'],
            'VERIFY_CP_FLAG' => 'N',
            'CURRENT_SERIAL_NUMBER' => $srDetail['SERIAL_NUMBER'],
            'EXTERNAL_ATTRIBUTE_13' => $this->CI->model('custom/ExtIntegrationConfigVerb')->getRntHost(),
            'EXTERNAL_ATTRIBUTE_14' => $incident->ReferenceNumber,
            'EXTERNAL_ATTRIBUTE_15' => $incident->ID
        );
        $responseFields = null;
        $errorFields = null;

        // send curl request
        $result = $this->CI->model('custom/EbsApi')->sendSoapRequest($this->extConfigVerb, $extObj, $extAction, $incident, $this->contact, $requestParams, $requestEntries, $responseFields, $errorFields);

        // check result
        if (!$result->error) {
            $this->log->debug("SR#{$srID} has been updated", __METHOD__, array($incident, $this->contact));
        }
    }

    /**
     * Create new Note in EBS according to the lastest thread of the incident in CP
     * @param int $srID Serivce Request ID
     * @param RNCPHP\Incident $incident Incident related to this Service Request
     * @return int|null The ID of the newly created note
     */
    function createNote($srID, RNCPHP\Incident $incident) {
        $thread = $incident->Threads[0];

        $noteDetail = $thread->Text;
        if (count($incident->Threads) === 1 && ($validateResult = $incident->CustomFields->Accelerator->cp_ebs_product_validation) !== null) {
            $noteDetail = <<<NOTEDETAIL
###   {$validateResult}   ###
$noteDetail
NOTEDETAIL;
        }

        $createdTime = gmdate('Y-m-d\TH:i:s', $thread->CreatedTime) . 'Z';

        // prepare note data
        $noteData = array(
            'P_API_VERSION' => '1', //HARDCODE
            'P_COMMIT' => 'T', //HARDCODE
            'P_ORG_ID' => 204, //HARDCODE
            'P_SOURCE_OBJECT_ID' => $srID,
            'P_SOURCE_OBJECT_CODE' => 'SR', //HARDCODE
            'P_NOTES' => "{$thread->Contact->LookupName} via [{$thread->Channel->LookupName}]",
            'P_NOTES_DETAIL' => $noteDetail,
            'P_NOTE_STATUS' => 'E', //HARDCODE
            'P_ENTERED_BY' => '0', //HARDCODE
            'P_ENTERED_DATE' => $createdTime,
            'P_LAST_UPDATE_DATE' => $createdTime,
            'P_LAST_UPDATED_BY' => '0', //HARDCODE
            'P_CREATION_DATE' => $createdTime,
            'P_CREATED_BY' => '0', //HARDCODE
            'P_NOTE_TYPE' => 'CS_PROBLEM', //HARDCODE
        );

        // prepare curl params
        $extObj = 'service_request_note';
        $extAction = 'create';
        $requestParams = null;
        $requestFieldData = $noteData;
        $responseFields = array('X_JTF_NOTE_ID');
        $errorFields = array('FAULTSTRING', 'X_MSG_DATA');

        // send curl request
        $result = $this->CI->model('custom/EbsApi')->sendSoapRequest($this->extConfigVerb, $extObj, $extAction, $incident, $this->contact, $requestParams, $requestFieldData, $responseFields, $errorFields);

        // return the note id if success
        if (!$result->error) {
            $noteID = $result->result['X_JTF_NOTE_ID'];
            $this->log->debug("Note#{$noteID} has been created for SR#{$srID}", __METHOD__, array($incident, $this->contact));
            return $noteID;
        }

        return null;
    }

    /**
     * Get notes by SR ID
     * @param int $srID Service Request ID
     * @return RNCPHP\RNObject Result of the getSRNotes request
     */
    public function getSRNotes($srID) {
        // prepare request and response params
        $extObj = 'service_request_note';
        $extAction = 'read';
        $requestParams = array('srID' => $srID);
        $requestFieldData = null;
        $responseFields = array(
            'NOTES',
            'NOTES_DETAIL',
            'NOTE_TYPE',
            'NOTE_TYPE_MEANING',
            'NOTE_STATUS',
            'NOTE_STATUS_MEANING',
            'CREATED_BY',
            'CREATION_DATE',
            'CREATED_BY_NAME',
            'CREATED_BY_USER_TYPE',
            'CONTACT_PARTY_ID');
        $errorFields = array('FAULTSTRING', 'X_MSG_DATA');

        // send curl request
        return $this->CI->model('custom/EbsApi')->sendSoapRequest($this->extConfigVerb, $extObj, $extAction, null, $this->contact, $requestParams, $requestFieldData, $responseFields, $errorFields);
    }

    /**
     * Validate the serail number and product mapping
     * @param string $serialNumber Serial Number
     * @param int $productID Product ID
     * @param RNCPHP\Incident $incident Related incident
     * @return RNCPHP\RNObject Validation result
     */
    public function validateSerialNumber($serialNumber, $productID, RNCPHP\Incident $incident = null) {
        if ($serialNumber === null) {
            return $this->composeSerialNumberValidationResult(false, 'Unable to validate Serial Number because it is not provided', null, $incident, false);
        }
        if (!$this->CI->utility->validateEbsContactID($this->contact)) {
            return $this->composeSerialNumberValidationResult(false, 'Unable to validation Serial Number. Please set contact_party_id and/or contact_org_id first', null, $incident, false);
        }

        // prepare request and response params
        $extObj = service_request_item_instance;
        $extAction = read;
        $requestParams = array(
            'serialNumber' => $serialNumber,
            'partyID' => $this->contact->CustomFields->Accelerator->ebs_contact_org_id
        );
        $requestFieldData = null;
        $responseFields = array('INSTANCE_ID', 'INVENTORY_ITEM_NAME');
        $errorFields = array(
            'FAULTSTRING',
            'X_MSG_DATA'
        );

        // send curl request
        $result = $this->CI->model('custom/EbsApi')->sendSoapRequest($this->extConfigVerb, $extObj, $extAction, null, $this->contact, $requestParams, $requestFieldData, $responseFields, $errorFields);

        // check result
        if ($result->error) {
            return $this->composeSerialNumberValidationResult(false, $result->error->externalMessage, null, $incident, true, true);
        } else {
            if ($result->result === null) {
                return $this->composeSerialNumberValidationResult(false, 'Serial Number not found', null, $incident, true);
            }
            return $this->checkSerialNumberAndProductMapping($productID, $incident, $result->result);
        }
    }

    /**
     * Check if the provided Serial Number and Product mapping is correct
     * @param int $productID ID of the product
     * @param RNCPHP\Incident $incident Related incident
     * @param array $ebsItem EBS Item Instance data 
     * @return  RNCPHP\RNObject Mapping validation result
     */
    private function checkSerialNumberAndProductMapping($productID, RNCPHP\Incident $incident = null, array $ebsItem = null) {
        if ($productID !== null && $rnProduct = $this->CI->utility->getProductByServiceProductID($productID)) {
            if ($ebsItem['INVENTORY_ITEM_NAME'] === $rnProduct['PartNumber']) {
                return $this->composeSerialNumberValidationResult(true, 'Product selection is correct based on provided Serial Number', $ebsItem, $incident, false);
            }
        }

        // check if EBS.INVENTORY_ITEM_NAME matchs one of the RN.Product.PartNumber
        if ($rnProduct = $this->CI->utility->getProductByPartNumber($ebsItem['INVENTORY_ITEM_NAME'])) {
            return $this->composeSerialNumberValidationResult(false, "Product: '{$rnProduct['Name']}' is found based on provided Serial Number", null, $incident, true);
        } else {
            return $this->composeSerialNumberValidationResult(false, "Product: '{$ebsItem['INVENTORY_ITEM_NAME']}' is found based on provided Serial Number", null, $incident, true);
        }
    }

    /**
     * Compose the Serial Number and Product validation result.
     * Operations contains:
     * 1. log the validation result
     * 2. save the result in the custom attribute  'cp_ebs_product_validation' if needed
     * 3. return the result containing isValid, meesage and item object
     * @param boolean $isValid Indicate if the validation is correct
     * @param string $message Validation result message
     * @param array $ebsItem Item of the provided Serial Number
     * @param RNCPHP\Incident $incident Related incident object
     * @param boolean $ifNeedToSaveInIncident Indicate if the checking result need to be saved in the incident
     * @param boolean $isError If the response is an error. for example, "Serial Number not found" is invalid, but not an error
     * @return array Validation result contains status, response, and item
     */
    private function composeSerialNumberValidationResult($isValid, $message, array $ebsItem = null, RNCPHP\Incident $incident = null, $ifNeedToSaveInIncident = false, $isError = false) {
        if ($isError) {
            $this->log->error("Serial Number validation :: {$message}", __METHOD__, array($incident, $this->contact));
        } else {
            $this->log->debug("Serial Number validation :: {$message}", __METHOD__, array($incident, $this->contact));
        }

        // save the validation result to incident if needed
        if ($incident !== null && $ifNeedToSaveInIncident === true) {
            $incident->CustomFields->Accelerator->cp_ebs_product_validation = $message;
            $incident->save(RNCPHP\RNObject::SuppressAll);
        }

        $result = (object) array(
                    'isValid' => $isValid,
                    'message' => $message,
                    'ebsItem' => $ebsItem
        );
        return $this->getResponseObject($result);
    }

}
