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
 *  date: Wed Sep  2 23:14:33 PDT 2015

 *  revision: rnw-15-8-fixes-release-01
 *  SHA1: $Id: 483fe454b2ba551c2233fdee5899960019255f80 $
 * *********************************************************************************************
 *  File: SiebelServiceRequest.php
 * ****************************************************************************************** */

namespace Custom\Models;

use RightNow\Connect\v1_2 as RNCPHP;

class SiebelServiceRequest extends \RightNow\Models\Base {

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
     * Get Service Request by ID
     * If SR is cached, get from cache directly,
     * otherwise, fetch SR from Siebel by sending a curl request.
     * Save the SR in the in-process cache to avoid send multiple requests to Siebel.
     * Save the SR in the session for incident creation usage.
     * @param int $srID Service Request ID
     * @return RNCPHP\RNObject Get SR result
     */
    function getSRDetailByID($srID) {
        if ($srID === null) {
            return $this->getResponseObject(null, null, 'Error occurs when getSRbyID :: invalid sr_id');
        }

        // check if the SR has been cached in the in-process cache
        // If cached, save it in the session and return the SR.
        $cacheKey = 'sr_' . $srID;
        $srDetail = \RightNow\Utils\Framework::checkCache($cacheKey);
        if ($srDetail) {
            return $this->getResponseObject($srDetail, 'is_array');
        }

        // if not cachaed, send request to Siebel server to get the SR,
        // and save in the session and cache
        $extObj = 'service_request_detail';
        $extAction = 'read';
        $requestParams = array('srID' => $srID);
        $requestFieldData = array(
            'Id',
            'Created',
            'Abstract',
            'ContactId',
            'CustomerProductId',
            'DefaultStatus',
            'Description',
            'OwnedById',
            'Owner',
            'Product',
            'ProductId',
            'SRNumber',
            'SRType',
            'SerialNumber',
            'Severity',
            'Status',
            'Type',
            'IntegrationId'
        );
        $responseFields = $requestFieldData;
        $errorFields = array('FAULTSTRING', 'X_MSG_DATA');

        // send curl request
        $getSRResult = $this->CI->model('custom/SiebelApi')->sendSoapRequest($this->extConfigVerb, $extObj, $extAction, null, $this->contact, $requestParams, $requestFieldData, $responseFields, $errorFields);

        // check the result. if success, save SR in the in-process cache and the session
        if ($getSRResult->error === null) {
            \RightNow\Utils\Framework::setCache($cacheKey, $getSRResult->result, true);
            $sessionKey = $cacheKey;
            $this->CI->session->setSessionData(array($sessionKey => $getSRResult->result));
        }

        return $getSRResult;
    }

    /**
     * Get list of Service Requests by ContactParyID
     * @return RNCPHP\RNObject Get SR list result
     */
    public function getSRList() {
        $contactPartyID = ($this->contact !== null) ? $this->contact->CustomFields->Accelerator->siebel_contact_party_id : null;
        if ($contactPartyID === null) {
            $errorMsg = __METHOD__ . ':: Unable to find the Siebel ContactPartyID';
            $this->log->error($errorMsg, __METHOD__, array(null, $this->contact));
            return $this->getResponseObject(null, null, $errorMsg);
        }

        // prepare curl params
        $extObj = 'service_request_list';
        $extAction = 'read';
        $requestParams = array('contactPartyID' => $contactPartyID);
        $requestFieldData = array(
            'Id',
            'Created',
            'Abstract',
            'Account',
            'AccountId',
            'ContactAccount',
            'ContactAccountId',
            'CustomerProductId',
            'Description3',
            'OwnedById',
            'Owner',
            'Product',
            'ProductId',
            'SRNumber',
            'SRType',
            'SerialNumber',
            'Severity',
            'Status',
            'Type',
            'IntegrationId'
        );
        $responseFields = $requestFieldData;
        $errorFields = array('FAULTSTRING', 'X_MSG_DATA');

        // send curl request and return response
        return $this->CI->model('custom/SiebelApi')->sendSoapRequest($this->extConfigVerb, $extObj, $extAction, null, $this->contact, $requestParams, $requestFieldData, $responseFields, $errorFields);
    }

    /**
     * Create new Service Request in Siebel
     * @param RNCPHP\Incident $incident The incident which the new Service Request will associate with
     * @param boolean $ifPropagateSerialNumberAndProduct Indicate if need to propagete Serial Number and Product when creating SR
     * @param array $asset Corresponding asset of the serial number
     * @return string|null ID of the created SR
     */
    function createSR(RNCPHP\Incident $incident, $ifPropagateSerialNumberAndProduct = false, array $asset = null) {
        $contactPartyID = ($this->contact !== null) ? $this->contact->CustomFields->Accelerator->siebel_contact_party_id : null;
        $extObj = 'service_request_detail';
        $extAction = 'create';
        $requestParams = null;
        $requestFieldData = array(
            'Abstract' => $incident->Subject,
            'ContactId' => $contactPartyID,
            'Created' => gmdate('Y-m-d\TH:i:s', $incident->CreatedTime),
            'IntegrationId' => $incident->ID . ',' . $incident->ReferenceNumber,
            'IntegrationSite' => $this->CI->model('custom/ExtIntegrationConfigVerb')->getRntHost()
        );
        // propagate serial number and product if needed
        if ($ifPropagateSerialNumberAndProduct === true && $asset !== null) {
            $requestFieldData['ProductId'] = $asset['PRODUCTID'];
            $requestFieldData['SerialNumber'] = $incident->CustomFields->Accelerator->siebel_serial_number;
        }
        $responseFields = array('ID');
        $errorFields = array('FAULTSTRING', 'X_MSG_DATA');

        // send curl request
        $result = $this->CI->model('custom/SiebelApi')->sendSoapRequest($this->extConfigVerb, $extObj, $extAction, $incident, $this->contact, $requestParams, $requestFieldData, $responseFields, $errorFields);

        // return srID if success
        if (!$result->error) {
            $srID = $result->result['ID'];
            $this->log->debug("SR#{$srID} has been created for Incident#{$incident->ID}", __METHOD__, array($incident, $this->contact));
            return $srID;
        }
        return null;
    }

    /**
     * Update Service Request to link with an incident in CP
     * use SR.description to store the incidentID and incidentRef in json format
     * @param int $srID Service Request ID
     * @param RNCPHP\Incident $incident Incident related to this Service Request
     */
    function updateSR($srID, RNCPHP\Incident $incident) {
        // prepare curl params
        $extObj = 'service_request_detail';
        $extAction = 'update';
        $requestParams = null;
        $requestFieldData = array(
            'Id' => $srID,
            'IntegrationId' => $incident->ID . ',' . $incident->ReferenceNumber,
            'IntegrationSite' => $this->CI->model('custom/ExtIntegrationConfigVerb')->getRntHost()
        );
        $responseFields = array('ID', 'MODID');
        $errorFields = array('FAULTSTRING', 'X_MSG_DATA');

        // send curl request
        $result = $this->CI->model('custom/SiebelApi')->sendSoapRequest($this->extConfigVerb, $extObj, $extAction, $incident, $this->contact, $requestParams, $requestFieldData, $responseFields, $errorFields);

        // check result
        if (!$result->error) {
            $this->log->debug("SR#{$srID} has been updated", __METHOD__, array($incident, $this->contact));
        }
    }

    /**
     * Create new web-inbound action in Siebel according to the latest thread of the incident
     * @param int $srID Serivce Request ID
     * @param RNCPHP\Incident $incident Incident related to the Service Request
     * @return int|null ID of the newly created web-inbound action
     */
    function createWebInboundAction($srID, RNCPHP\Incident $incident) {
        $comment = $incident->Threads[0]->Text;
        // append serial number validation result if needed
        if (count($incident->Threads) === 1 && ($serialNumberValidationResult = $incident->CustomFields->Accelerator->cp_siebel_product_validation) !== null) {
            $comment = <<<COMMENT
###   {$serialNumberValidationResult}   ### 
$comment
COMMENT;
        }
        $extObj = 'service_request_note';
        $extAction = 'create';
        $requestParams = array('srID' => $srID);
        $requestFieldData = array(
            'Comment' => $comment,
            'Type' => 'Web - Inbound',
            'Description2' => $this->prepareActionDescription($incident, $incident->Threads[0]->Text)
        );
        $responseFields = array('ID');
        $errorFields = array('FAULTSTRING', 'X_MSG_DATA');

        // send curl request
        $result = $this->CI->model('custom/SiebelApi')->sendSoapRequest($this->extConfigVerb, $extObj, $extAction, $incident, $this->contact, $requestParams, $requestFieldData, $responseFields, $errorFields);

        // return the note id if success
        if (!$result->error) {
            $noteID = $result->result['ID'];
            $this->log->debug("Web-Inbound action#{$noteID} has been created for SR#{$srID}", __METHOD__, array($incident, $this->contact));
            return $noteID;
        }
        return null;
    }

    /**
     * Prepare the action description containing contact, channel, and the comment info.
     * The test will be truncated within 100 characters.
     * @param RNCPHP\Incident $incident The incident this action belongs to 
     * @param string $text Text in the comment field which will be truncated and displayed in discription field 
     * @return string
     */
    private function prepareActionDescription(RNCPHP\Incident $incident, $text) {
        $extraInfo = "{$incident->Threads[0]->Contact->LookupName} via [{$incident->Threads[0]->Channel->LookupName}]";
        $description = $extraInfo . ' ' . $text;

        if (strlen($description) > 100) {
            $description = substr($description, 0, 97);
            $description .= '...';
        }
        return $description;
    }

    /**
     * Get communication actions of the Service Request by SR ID
     * @param int $srID Service Request ID
     * @param int $pageSize The number of actions will be returned
     * @return RNCPHP\RNObject Get communication action list result
     */
    public function getCommunicationActions($srID, $pageSize = null) {
        // prepare curl params
        $extObj = 'service_request_note';
        $extAction = 'read';
        $requestParams = array('srID' => $srID, 'pageSize' => $pageSize);
        $requestFieldData = array(
            'Created',
            'CreatedByName',
            'Comment',
            'Type',
            'Private'
        );
        $responseFields = $requestFieldData;
        $errorFields = array('FAULTSTRING', 'X_MSG_DATA');

        // send curl request and return the response
        return $this->CI->model('custom/SiebelApi')->sendSoapRequest($this->extConfigVerb, $extObj, $extAction, null, $this->contact, $requestParams, $requestFieldData, $responseFields, $errorFields);
    }

    /**
     * Validate the serail number and product mapping
     * @param string $serialNumber Serial Number of the product
     * @param int $productID ID of the product
     * @param RNCPHP\Incident $incident Related incident
     * @return RNCPHP\RNObject Serial Number Validation result
     */
    public function validateSerialNumber($serialNumber, $productID, RNCPHP\Incident $incident = null) {
        // check parameters
        if ($serialNumber === null) {
            return $this->composeSerialNumberAndProductValidationResult(false, 'Unable to validate Serial Number because it is not provided', false, false, $incident, null, true);
        }

        if (!$this->CI->utility->validateSiebelContactID($this->contact)) {
            return $this->composeSerialNumberAndProductValidationResult(false, 'Unable to validate Serial Number. Please set contact_party_id and/or contact_org_id first', false, false, $incident, null, true);
        }

        // prepare request and response params
        $extObj = 'asset_management';
        $extAction = 'read';
        $requestParams = null;
        $requestFieldData = array(
            'Id' => null,
            'OrganizationId' => $this->contact->CustomFields->Accelerator->siebel_contact_org_id,
            'ProductId' => null,
            'ProductName' => null,
            'SerialNumber' => $serialNumber,
        );
        $responseFields = array(
            'Id',
            'AccountOrgId',
            'ProductId',
            'ProductName',
            'SerialNumber'
        );
        $errorFields = array(
            'FAULTSTRING',
            'X_MSG_DATA'
        );

        // send curl request
        $result = $this->CI->model('custom/SiebelApi')->sendSoapRequest($this->extConfigVerb, $extObj, $extAction, $incident, $this->contact, $requestParams, $requestFieldData, $responseFields, $errorFields);

        if ($result->error) {
            return $this->composeSerialNumberAndProductValidationResult(false, $result->error->externalMessage, true, false, $incident, null, true);
        } else {
            // check if product matches
            $asset = $result->result;
            if ($asset === null) {
                return $this->composeSerialNumberAndProductValidationResult(false, 'Serial Number Not Found', true, false, $incident, null);
            }
            if ($productID === null) {
                return $this->composeSerialNumberAndProductValidationResult(false, "Product: '{$asset['PRODUCTNAME']}' is found based on provided Serial Number", true, false, $incident, null);
            } else {
                $rnProduct = $this->CI->utility->getProductByServiceProductID($productID);
                if ($asset['PRODUCTID'] === $rnProduct['PartNumber'] && $asset['PRODUCTNAME'] === $rnProduct['Name']) {
                    return $this->composeSerialNumberAndProductValidationResult(true, 'Product selection is correct based on provided Serial Number', false, true, $incident, $asset);
                } else {
                    return $this->composeSerialNumberAndProductValidationResult(false, "Product: '{$asset['PRODUCTNAME']}' is found based on provided Serial Number", true, false, $incident, null);
                }
            }
        }
    }

    /**
     * Compose the Serial Number and Product validation result.Operations contains:
     * 1. log the validation result
     * 2. save the result in the custom attribute  'siebel_product_validation_result' if needed
     * 3. return the result contains isValid, message, ifPropagateSerialNumberAndProduct, and asset
     * @param boolean $isValid Indicate if the validation is correct
     * @param string $message Validation result message
     * @param boolean $ifNeedToSaveInIncident Indicate if the checking result need to be saved in the incident
     * @param boolean $ifPropagateSerialNumberAndProduct Indicate if need to propagete Serial Number and Product when creating SR
     * @param RNCPHP\Incident $incident Related incident object
     * @param array $item Item of the provided Serial Number
     * @return array Validation result contains status, reponse, and item
     */
    private function composeSerialNumberAndProductValidationResult($isValid, $message, $ifNeedToSaveInIncident, $ifPropagateSerialNumberAndProduct, RNCPHP\Incident $incident = null, array $asset = null, $isError = false) {
        // log the validation result
        // TODO:: invalid sn# log as error???
        if ($isValid === true) {
            $this->log->debug("Serial Number validation :: {$message}", __METHOD__, array($incident, $this->contact));
        } else {
            if ($isError) {
                $this->log->error("Serial Number validation :: {$message}", __METHOD__, array($incident, $this->contact));
            } else {
                $this->log->debug("Serial Number validation :: {$message}", __METHOD__, array($incident, $this->contact));
            }
        }

        // save the validation result to incident if needed
        if ($incident !== null && $ifNeedToSaveInIncident === true) {
            $incident->CustomFields->Accelerator->cp_siebel_product_validation = $message;
            $incident->save(RNCPHP\RNObject::SuppressAll);
        }

        $result = (object) array(
                    'isValid' => $isValid,
                    'message' => $message,
                    'ifPropagateSerialNumberAndProduct' => $ifPropagateSerialNumberAndProduct,
                    'asset' => $asset
        );

        return $this->getResponseObject($result);
    }

}
