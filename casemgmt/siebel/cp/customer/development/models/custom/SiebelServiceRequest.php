<?php

/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC Contact Center + Siebel Case Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  Siebel release: 8.1.1.15
 *  reference: 150520-000047
 *  date: Mon Nov 30 19:59:29 PST 2015

 *  revision: rnw-15-11-fixes-release-2
 *  SHA1: $Id: a17c362effb634556e948e5330feaa98937b0128 $
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
            'OrganizationId',
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
        if ($isError) {
            $this->log->error("Serial Number validation :: {$message}", __METHOD__, array($incident, $this->contact));
        } else if (!$isError && !$isValid) {
            $this->log->notice("Serial Number validation :: {$message}", __METHOD__, array($incident, $this->contact));
        } else {
            $this->log->debug("Serial Number validation :: {$message}", __METHOD__, array($incident, $this->contact));
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
                    'asset' => $asset,
                    'isError' => $isError
        );

        return $this->getResponseObject($result);
    }


    /**
     * Calls a USPS web-service for Address Verification
     * @param string $street addres first or ONLY LINE
     * @param string $city
     * @param string $state  State or Province
     * @param string $zip5   zip or other postal code
     * @param string $zip4Ext = null by default (4 digit zip)
     */
    public function callAddressVerification($street, $city, $state, $zip5=NULL, $zip4Ext=NULL) 
    {
        if ($zip5==NULL) {
            $zip5 = '';
        }
        if ($zip4Ext == NULL) {
            $zip4Ext = '';
        }
        
        // get ext 'seting' from config verb
        $extValidateUrl = $this->extConfigVerb['postalValidation']['ext_address_validate_url'];
        $username = $this->extConfigVerb['postalValidation']['username'];
        $password = $this->extConfigVerb['postalValidation']['password'];  // maybe not used
        //
        // Config Verb section expected :
        // "postalValidation":{
        // "ext_address_validate_url": "http://production.shippingapis.com/ShippingAPITest.dll",
        // "username": "2XXX54YY"
        // },
              
        if ($extValidateUrl == '') {
            if ($username == '') {
                $tempObj = $this->composeAddressVerificationResult($false, 
                        "postalValidation username AND ext_address_validate_url are not configured", $result->result);
                $this->log->debug("Returning Response2:", __METHOD__, null, print_r($tempObj, TRUE));        
                return $tempObj;
            } else {
                // default USPS URL.. not gonna change unless you move the server??
                $extValidateUrl = 'http://production.shippingapis.com/ShippingAPITest.dll';
            }
        }
        
        // prepare request and response params
        $xml = '<AddressValidateRequest USERID="'.$username.'"><Address><Address1></Address1><Address2>'
                .$street.'</Address2><City>'.$city.'</City><State>'
                .$state.'</State><Zip5>'.$zip5.'</Zip5><Zip4>'.$zip4Ext.'</Zip4></Address></AddressValidateRequest>';
        
        $requestParams = array(
            'API'=>'Verify',
            'XML'=>$xml
        );

        // ReturnText doesn't always show up, but can be e.g. "Default address: The address you entered was found but more information is needed (such as an apartment, suite, or box number) to match to a specific address"
        // Error is the parent element; Description and (error)Number always inside. 
        // The parsing takes the first error element, so putting 'Description'  if you want to get the content of the error. 
        $parseForFields = array('ADDRESS2', 'CITY', 'STATE', 'ZIP5', 'ZIP4', 'RETURNTEXT' );
        $errorFields = array('DESCRIPTION', 'ERROR');

        // 
        // Send request
        $result = $this->CI->model('custom/SiebelApi')->sendAddressRequest(
                $extValidateUrl, $requestParams, $parseForFields, $errorFields);
        
//        if (IS_DEVELOPMENT === true) {
//            $tIS_DEVELOPMENThis->log->debug("Result from sendAddressRequest", __METHOD__, null, print_r($result, true));
//        }
        
        // check the result and return a message and a 'correct' flag. Also output log. 
        $returnVals = $this->makeSanitizedUserMessage($result);
        extract($returnVals);  // = $is_correct, $message

        $tempObj = $this->composeAddressVerificationResult($is_correct, 
                $message, $result->result);  // the address is inside ->result 
        // this is a DBG message; shows return to controller --Jordan
        $this->log->debug("Returning Response2:", __METHOD__, null, print_r($tempObj, TRUE));        
        return $tempObj;
    }

    /**
     * just checks the result WHETHER IT IS CORRECT AND MAKES AN is_correct flag;
     * and checks the message inside the result, depending on the situation:
     * errors may have two messages, and non-error may have one message(ReturnText)
     * 
     * Therefore we 'sanitize' this message when returning a hardcoded one. 
     * 
     * @param \RightNow\Libraries\ResponseObject $result
     * @return array ( Hardcoded message to be displayed to the user , is_correct flag)
     */
    private function makeSanitizedUserMessage( \RightNow\Libraries\ResponseObject $result)
    {
        // check result and make $is_correct flag, 
        if ($result == null || $result->error || $result->result === null) {
            // result->errors[0]->externalMessage may be filled here ; changed to notice for non-critical error.
            $is_correct = false;
        } else {
            $is_correct = true;
        }

        $seriousError = false;
        if (!$is_correct) {
            // access this array only if there was error. 
            $message = $result->errors[0]->externalMessage;
            $origErrorText = $message;
            
            // 'Sanitizing' message
            if (preg_match('/not found/i', $message)) {
                $message = "Address Not Found.  ";
            } else if (preg_match('/Multiple.*no default/i', $message)) {
                $message = "Multiple addresses found; no default exists.";
            } else if (preg_match('/Invalid.*State/i', $message)) {
                $message = "Invalid or Incomplete State Code.";
            } else if (preg_match('/Invalid.*City/i', $message)) {
                $message = "Invalid City.";
            } else if (preg_match('/password|Authorization|xceed.*len/i', $message)) {
	      	$message = "Misconfiguration--invalid password or other error received.";
                $seriousError = true;
            } else if (preg_match('/disabled|test request.*valid|valid API/i', $message)) {
                $message = "Misconfiguration--check the URL, API or some other error.";
                $seriousError = true;
            } else if (preg_match('/not.*connect/i', $message)) {
                // When the URL is bad, it is reponse 502 containing HTML: html.+not\s*connect
                $message = "Misconfiguration--invalid URL or cannot connect. Please see the Error log!";  // html/https error included.
                $seriousError = true;
            } else {
                $message = "Misconfiguration or some other Error; or invalid data received. Please see the Error log!";
            }
            
            // original message shows up only in Dev mode (Per Req) and if msg is not huge or HTML
            if (IS_DEVELOPMENT === true && strlen($origErrorText)>4 && preg_match('/\<html/i', $origErrorText) !== 1 ) {
                if (strlen($origErrorText)<320) {
                    $message = 'Not valid: '.$origErrorText;   
                } else {
                    $message = 'Not valid: '.substr($origErrorText, 0, 320).'...';
                }
            } else {
                $message = 'Not valid: '.$message;
            }
            
            // separate levels of logging; due to requirement. Notice all info logged on 'error'
            if ($seriousError) {
                $this->log->error("Error calling address verification", __METHOD__, null, 
                        "Result: ".print_r($result, true));
            } else {
                $this->log->notice("Address Not Found or Empty(error)", __METHOD__, null, 
                        "Message: ".print_r($message, TRUE));
            }
        } else {
            // BOTH correct situations ; with return text and without.            
            if ($result->result['RETURNTEXT']!='') {      // $result->result is an array          
                $message = $result->result['RETURNTEXT'];
                if (IS_DEVELOPMENT !== true) {
                    if (preg_match('/was\s+found.*more\s+information/i', $message)) {
                        $message = 'Address verified: The address '
                                . ' was found but more information is needed (such as an apt., suite)'
                                . ' to match to a specific address.';
                    } else {
                        $message = 'Address verified: The address you entered is found.';
                    }                
                } else if (strlen($message)>320) {
                    // in Dev mode, displaying a 'shortened', un-'sanitized' message. 
                    $message = substr($message, 0, 320).'...';
                }
                
                // removing extra long message.
                $copyArray = $result->result;
                unset($copyArray['RETURNTEXT']);
                $result->result = $copyArray;                                
                
                // putting back in the log by request from dev/qa
                if (IS_DEVELOPMENT === true) {
                    // logging with a raised Log level (~ 4) due to the addtl info
                    $this->log->notice("Address message modified to:", __METHOD__, null, $message);
                }
            } else {
                // lower log level for a completely correct result. 
                // $this->log->debug("Result from sendAddressRequest", __METHOD__, null, print_r($result, true));                
                $message = 'Address verified.';
            }
        }
        return array ('message' => $message, 'is_correct' => $is_correct);
    }
    /**
     * Compose the address result.
     * Operations contains:
     * @param boolean $isValid Indicate if the validation is correct
     * @param string $message Validation result message
     * @param boolean $isError If the response is an error. for example, "Serial Number not found" is invalid, but not an error
     * @return array Verification result contains status, response, and item
     */
    private function composeAddressVerificationResult($isValid, $message, array $address = null) {
        if (!is_array($address)) {
            $address = array('Address2' => '');
        }
        $result = (object) array(
                    'isValid' => $isValid,
                    'message' => $message,
                    'address' => $address,
                    'isError' => false
        );
        return $this->getResponseObject($result);
    }

}
