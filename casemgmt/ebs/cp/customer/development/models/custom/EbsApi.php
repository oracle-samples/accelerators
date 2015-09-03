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
 *  date: Wed Sep  2 23:11:31 PDT 2015

 *  revision: rnw-15-8-fixes-release-01
 *  SHA1: $Id: c9a7cf59472c6315be0fef42899c5d8168c97d72 $
 * *********************************************************************************************
 *  File: EbsApi.php
 * ****************************************************************************************** */

namespace Custom\Models;

use RightNow\Connect\v1_2 as RNCPHP;

class EbsApi extends \RightNow\Models\Base {

    private $log;

    function __construct() {
        parent::__construct();

        $this->CI->load->library('CurlRequest');
        $this->CI->load->library('EbsRequestGenerator');
        $this->CI->load->library('Utility');

        $this->log = $this->CI->utility->getLogWrapper();
    }

    /**
     * Generate the SOAP request, send to EBS server and parse the result
     * @param array $extConfigVerb External config verb
     * @param string $extObj Related external object name
     * @param string $extAction Related external action, it could be read, update, create, or delete
     * @param RNCPHP\Incident $incident Incident related to this request
     * @param RNCPHP\Contact $contact Contact related to this request
     * @param array $requestParams Parameters required in the request payload
     * @param array $requestEntries Entries required in the request payload, it could be a SR detail or SR note
     * @param array $responseEntries Fields will be parsed from the response
     * @param array $errorFields Error field name in the response
     * @return RNCPHP\RNObject SOAP request result
     */
    function sendSoapRequest($extConfigVerb, $extObj, $extAction, RNCPHP\Incident $incident = null, RNCPHP\Contact $contact = null, array $requestParams = null, array $requestEntries = null, array $responseEntries = null, array $errorFields = null) {
        // check input params
        if ($extConfigVerb === null || $extObj === null || $extAction === null) {
            $errorMsg = "{$extObj} :: {$extAction} :: Unable to get ext_config_verb, extObj, or extAction";
            $this->log->error($errorMsg, __METHOD__, array($incident, $contact));
            return $this->getResponseObject(null, null, $errorMsg);
        }

        // get ext service seting from config verb
        $extBaseUrl = $extConfigVerb['integration']['ext_base_url'];
        $username = $extConfigVerb['integration']['username'];
        $password = $extConfigVerb['integration']['password'];
        $extServices = $extConfigVerb['integration']['ext_services'];
        $extServiceSetting = $extServices[$extObj][$extAction];
        if ($extServiceSetting === null) {
            $errorMsg = "{$extObj} :: {$extAction} :: Unable to get ext_service_setting";
            $this->log->error($errorMsg, __METHOD__, array($incident, $contact));
            return $this->getResponseObject(null, null, $errorMsg);
        }
        $soapAction = $extServiceSetting['soap_action'];
        $endpoint = $extBaseUrl . $extServiceSetting['relative_path'];
        $logMsg = "{$extObj} :: {$extAction} :: ";

        // prepare SOAP request payload and HTTP headers   
        list($requestString, $httpHeaders) = $this->generateEbsSoapRequestPayload($username, $password, $extObj, $extAction, $soapAction, $extBaseUrl, $requestParams, $requestEntries);

        // send cURL request
        $result = $this->CI->curlrequest->sendCurlSoapRequest($endpoint, $httpHeaders, $requestString, $logMsg, null, $contact);

        // check the result
        if ($result['status'] === false) {
            $this->log->error("{$extObj} :: {$extAction} :: Request failed", __METHOD__, array($incident, $contact), json_encode($result));
            return $this->getResponseObject(null, null, $result['response']);
        } else if ($result['status'] === true) {
            return $this->parseEbsSoapResponse($extObj, $extAction, $result['response'], $responseEntries, $errorFields, $incident, $contact);
        }
    }

    /**
     * Generate the SOAP request payload and HTTP headers for cURL
     * @param string $username EBS account username
     * @param string $password EBS account password
     * @param string $extObj Related external object name
     * @param string $extAction Related external action, it could be read, update, create, or delete
     * @param string $soapAction SOAP action field required in the HTTP headers
     * @param string $baseUrl Base URL of the external server
     * @param array $requestParams Parameters required in the request payload
     * @param array $requestEntries Entries required in the request payload, it could be a SR detail or SR note
     * @return array|null Array contains $requestString and $httpHeaders
     */
    private function generateEbsSoapRequestPayload($username, $password, $extObj, $extAction, $soapAction, $baseUrl, array $requestParams = null, array $requestEntries = null) {
        $requestPayload = null;
        if ($extObj === 'service_request_detail') {
            if ($extAction === 'read') {
                $srID = $requestParams['srID'];
                $requestPayload = $this->CI->ebsrequestgenerator->generateEbsGetSRRequest($username, $password, $srID);
            } else if ($extAction === 'create') {
                $contactPartyID = $requestParams['contactPartyID'];
                $requestPayload = $this->CI->ebsrequestgenerator->generateEbsCreateSRRequest($username, $password, $requestEntries, $contactPartyID);
            } else if ($extAction === 'update') {
                $srID = $requestParams['srID'];
                $srObjVerNum = $requestParams['srObjVerNum'];
                $requestPayload = $this->CI->ebsrequestgenerator->generateEbsUpdateSRRequest($username, $password, $requestEntries, $srID, $srObjVerNum);
            }
        } else if ($extObj === 'service_request_note') {
            if ($extAction === 'read') {
                $srID = $requestParams['srID'];
                $requestPayload = $this->CI->ebsrequestgenerator->generateEbsGetNotesRequest($username, $password, $srID);
            } else if ($extAction === 'create') {
                $requestPayload = $this->CI->ebsrequestgenerator->generateEbsCreateNoteRequest($username, $password, $requestEntries);
            }
        } else if ($extObj === 'service_request_list') {
            if ($extAction === 'read') {
                $contactPartyID = $requestParams['contactPartyID'];
                $requestPayload = $this->CI->ebsrequestgenerator->generateEbsGetSRListRequest($username, $password, $contactPartyID);
            }
        } else if ($extObj === 'service_request_item_instance') {
            if ($extAction === 'read') {
                $serialNumber = $requestParams['serialNumber'];
                $partyID = $requestParams['partyID'];
                $requestPayload = $this->CI->ebsrequestgenerator->generateEbsGetItemInstanceRequest($username, $password, $serialNumber, $partyID);
            }
        }

        $httpHeaders = $this->CI->ebsrequestgenerator->generateEbsRequestHttpHeaders($soapAction, strlen($requestPayload), $baseUrl);
        return array($requestPayload, $httpHeaders);
    }

    /**
     * Parse the XML response from EBS server
     * @param string $extObj Related external object name
     * @param string $extAction Related external action, it could be read, update, create, or delete
     * @param String $response XML response from EBS Server
     * @param array $responseEntries List of fields that will be parsed from the response
     * @param array $errorFields List of error fields that may contains the error message
     * @param RNCPHP\Incident $incident Incident related to this response
     * @param RNCPHP\Contact $contact Contact related to this response
     * @return RNCPHP\RNObject Parsing result
     */
    private function parseEbsSoapResponse($extObj, $extAction, $response, array $responseEntries = null, array $errorFields = null, RNCPHP\Incident $incident = null, RNCPHP\Contact $contact = null) {
        if ($extObj === 'service_request_detail') {
            if ($extAction === 'read' || $extAction === 'create') {
                $parsingResult = $this->parseSingleItemFromSoapResponse($response, $responseEntries, $errorFields);
            } else if ($extAction === 'update') {
                $parsingResult = $this->parseEbsResponseForUpdateSr($response);
            }
        } else if ($extObj === 'service_request_note') {
            if ($extAction === 'read') {
                $parsingResult = $this->parseListFromSoapResponse($response, 'X_NOTES_ITEM', $responseEntries, $errorFields);
            } else if ($extAction === 'create') {
                $parsingResult = $this->parseSingleItemFromSoapResponse($response, $responseEntries, $errorFields);
            }
        } else if ($extObj === 'service_request_list') {
            if ($extAction === 'read') {
                $parsingResult = $this->parseListFromSoapResponse($response, 'X_SR_DET_TBL_ITEM', $responseEntries, $errorFields);
            }
        } else if ($extObj === 'service_request_item_instance') {
            if ($extAction === 'read') {
                $parsingResult = $this->parseSingleItemFromSoapResponse($response, $responseEntries, $errorFields, false);
            }
        }

        // check the parsing result
        if ($parsingResult->error) {
            $this->log->error("{$extObj} :: {$extAction} :: failed", __METHOD__, array($incident, $contact), json_encode($parsingResult->error->externalMessage));
        } else {
            $this->log->debug("{$extObj} :: {$extAction} :: success", __METHOD__, array($incident, $contact), json_encode($parsingResult->result));
        }

        return $parsingResult;
    }

    /**
     * Parse response contains a list of items, like SR list and Note list
     * @param string $responseString SOAP Response from EBS server
     * @param string $itemOpenTag Open tag of a single item in the list 
     * @param array $responseEntries List of fields will be parsed from the response
     * @param array $errorFields List of error fields that may contains the error message
     * @return RNCPHP\RNObject Parsing result
     */
    private function parseListFromSoapResponse($responseString, $itemOpenTag, array $responseEntries = null, array $errorFields = null) {
        if ($responseString === null) {
            return $this->getResponseObject(null, null, 'Unable to parse EBS response :: soap response is empty');
        }

        $parser = xml_parser_create();
        $responseStruct = array();
        xml_parse_into_struct($parser, $responseString, $responseStruct);

        $response = array();
        $count = -1;
        foreach ($responseStruct as $item) {
            // check if return error
            if (in_array($item['tag'], $errorFields) && $item['value'] !== null) {
                return $this->getResponseObject(null, null, $item['value']);
            }

            // check if a new item start
            if ($item['tag'] === $itemOpenTag && $item['type'] === 'open') {
                $count++;
                continue;
            }

            // only parse the tag in the required field list
            if (in_array($item['tag'], $responseEntries)) {
                $response[$count][$item['tag']] = $item[value];
            }
        }

        return $this->getResponseObject($response, 'is_array');
    }

    /**
     * Parse Response contains a single item, for example a single Service Request
     * @param array $responseString SOAP Response from EBS server
     * @param array $responseEntries List of fields will be parsed from the response
     * @param array $errorFields List of error fields that may contains the error message
     * @param boolean $treatEmptyAsError Indicate if treat empty parsing result as an error
     * @return RNCPHP\RNObject Parsing result
     */
    private function parseSingleItemFromSoapResponse($responseString, array $responseEntries = null, array $errorFields = null, $treatEmptyAsError = true) {
        if ($responseString === null) {
            return $this->getResponseObject(null, null, 'Unable to parse EBS response :: soap response is empty');
        }

        $responseStruct = array();
        $parser = xml_parser_create();
        xml_parse_into_struct($parser, $responseString, $responseStruct);

        $response = array();
        foreach ($responseStruct as $item) {
            // check if error returned
            if (in_array($item['tag'], $errorFields) && $item['value'] !== null) {
                return $this->getResponseObject(null, null, $item['value']);
            }

            // only parse the tag in the required field list
            if (in_array($item['tag'], $responseEntries)) {
                $response[$item['tag']] = $item[value];
            }
        }

        if ($this->isResponseDataEmpty($response)) {
            if ($treatEmptyAsError) {
                return $this->getResponseObject(null, null, 'Unable to parse EBS response :: parsing result is empty');
            } else {
                return $this->getResponseObject(null, null);
            }
        } else {
            return $this->getResponseObject($response, 'is_array');
        }
    }

    /**
     * Check if the parsed response data is empty
     * @param array $data Parsed response data
     * @return boolean If the parsed response data is empty
     */
    private function isResponseDataEmpty(array $data) {
        if (empty($data)) {
            return true;
        }
        foreach ($data as $key => $value) {
            if ($value !== null) {
                return false;
            }
        }
        return true;
    }

    /**
     * Special parser for EBS udpateSR API.
     * The reason that updateSR has a seperate parser is the API dosen't use 
     * the common error field. It uses the field 'X_RETURN_STATUS' to indicate
     * if the update is successful or not.
     * @param string $responseString Response from EBS server
     * @return array|null Parse result
     */
    private function parseEbsResponseForUpdateSr($responseString) {
        if ($responseString === null) {
            return $this->getResponseObject(null, null, 'Unable to parse EBS response :: soap response is empty');
        }

        $parser = xml_parser_create();
        $responseStruct = null;
        xml_parse_into_struct($parser, $responseString, $responseStruct);

        foreach ($responseStruct as $item) {
            if ($item['tag'] === 'X_RETURN_STATUS') {
                if ($item['value'] === 'S') {
                    return $this->getResponseObject('EBS Service Request update success');
                } else {
                    return $this->getResponseObject(null, null, 'EBS Service Request update failed');
                }
            }
        }
        return $this->getResponseObject(null, null, 'EBS Service Request update failed');
    }

}
