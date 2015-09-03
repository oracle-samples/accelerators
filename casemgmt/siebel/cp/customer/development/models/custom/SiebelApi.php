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
 *  SHA1: $Id: fadc015e11c4548629fb27e0a395d111c441ee2a $
 * *********************************************************************************************
 *  File: SiebelApi.php
 * ****************************************************************************************** */

namespace Custom\Models;

use RightNow\Connect\v1_2 as RNCPHP;

class SiebelApi extends \RightNow\Models\Base {

    private $log;

    function __construct() {
        parent::__construct();
        $this->CI->load->library('CurlRequest');
        $this->CI->load->library('SiebelRequestGenerator');
        $this->CI->load->library('Utility');

        $this->log = $this->CI->utility->getLogWrapper();
    }

    /**
     * Generate the SOAP request, send to Siebel server and parse the result
     * @param array $extConfigVerb External config verb
     * @param string $extObj Related external object names
     * @param string $extAction Related external action, it could be read, update, create, or delete
     * @param RNCPHP\Incident $incident Incident related to this request
     * @param RNCPHP\Contact $contact Contact related to this request
     * @param array $requestParams Parameters required in the request payload
     * @param array $requestEntries Entries required in the request payload, it could be a SR detail or SR note
     * @param array $responseFields Fields will be parsed from the response
     * @param array $errorFields The name of the fields that contains the error message
     * @return array $result|null SOAP request result
     */
    function sendSoapRequest(array $extConfigVerb, $extObj, $extAction, RNCPHP\Incident $incident = null, RNCPHP\Contact $contact = null, array $requestParams = null, array $requestEntries = null, array $responseFields = null, array $errorFields = null) {
        // check input params
        if ($extConfigVerb === null || $extObj === null || $extAction === null) {
            $errorMsg = __METHOD__ . ":: {$extObj} :: {$extAction} :: Unable to get ext_config_verb, extObj, or extAction";
            $this->log->error($errorMsg, __METHOD__, array($incident, $contact));
            return $this->getResponseObject(null, null, $errorMsg);
        }

        // get ext service seting from config
        $extBaseUrl = $extConfigVerb['integration']['ext_base_url'];
        $username = $extConfigVerb['integration']['username'];
        $password = $extConfigVerb['integration']['password'];
        $extServices = $extConfigVerb['integration']['ext_services'];
        $extServiceSetting = $extServices[$extObj][$extAction];
        if ($extServiceSetting === null) {
            $errorMsg = __METHOD__ . "{$extObj} :: {$extAction} :: Unable to get ext_service_setting";
            $this->log->error($errorMsg, __METHOD__, array($incident, $contact));
            return $this->getResponseObject(null, null, $errorMsg);
        }
        $soapAction = $extServiceSetting['soap_action'];
        $endpoint = $extBaseUrl . $extServiceSetting['relative_path'];
        $logMsg = "{$extObj} :: {$extAction} :: ";

        // prepare SOAP request payload and HTTP headers   
        list($requestString, $httpHeaders) = $this->generateSoapRequest($username, $password, $extObj, $extAction, $soapAction, $extBaseUrl, $requestParams, $requestEntries);

        // send cURL request
        $result = $this->CI->curlrequest->sendCurlSoapRequest($endpoint, $httpHeaders, $requestString, $logMsg, $incident, $contact);

        // check result
        if ($result['status'] === false) {
            $this->log->error("{$extObj} :: {$extAction} :: Request failed", __METHOD__, array($incident, $contact), json_encode($result));
            return $this->getResponseObject(null, null, "Request failed." . $result['response']);
        } else if ($result['status'] === true) {
            return $this->parseSoapResponse($extObj, $extAction, $result['response'], $responseFields, $errorFields, $incident, $contact);
        }
    }

    /**
     * Generate the SOAP request payload and HTTP headers for cURL
     * @param string $username Siebel account username
     * @param string $password Siebel account password
     * @param string $extObj Related external object names
     * @param string $extAction Related external action, it could be read, update, create, or delete
     * @param string $soapAction SOAP action field required in the HTTP headers
     * @param string $baseUrl Base URL of the external server
     * @param array $requestParams Parameters required in the request payload
     * @param array $requestEntries Data required in the request payload, it could be a SR detail, SR notes, or a list of field entries
     * @return array|null Array contains $requestString and $httpHeaders
     */
    private function generateSoapRequest($username, $password, $extObj, $extAction, $soapAction, $baseUrl, array $requestParams = null, array $requestEntries = null) {
        $requestPayload = null;
        if ($extObj === 'service_request_detail') {
            if ($extAction === 'read') {
                $srID = $requestParams['srID'];
                $requestPayload = $this->CI->siebelrequestgenerator->generateGetSRRequest($username, $password, $srID, $requestEntries);
            } else if ($extAction === 'create') {
                $requestPayload = $this->CI->siebelrequestgenerator->generateCreateSRRequest($username, $password, $requestEntries);
            } else if ($extAction === 'update') {
                $requestPayload = $this->CI->siebelrequestgenerator->generateUpdateSRRequest($username, $password, $requestEntries);
            }
        } else if ($extObj === 'service_request_note') {
            if ($extAction === 'read') {
                $srID = $requestParams['srID'];
                $pageSize = $requestParams['pageSize'];
                $requestPayload = $this->CI->siebelrequestgenerator->generateGetNoteRequest($username, $password, $srID, $requestEntries, $pageSize);
            } else if ($extAction === 'create') {
                $srID = $requestParams['srID'];
                $requestPayload = $this->CI->siebelrequestgenerator->generateCreateNoteRequest($username, $password, $srID, $requestEntries);
            }
        } else if ($extObj === 'service_request_list') {
            if ($extAction === 'read') {
                $contactPartyID = $requestParams['contactPartyID'];
                $requestPayload = $this->CI->siebelrequestgenerator->generateGetSRListRequest($username, $password, $contactPartyID, $requestEntries);
            }
        } else if ($extObj === 'asset_management') {
            if ($extAction === 'read') {
                $requestPayload = $this->CI->siebelrequestgenerator->generateAssetQueryRequest($username, $password, $requestEntries);
            }
        }

        $httpHeaders = $this->CI->siebelrequestgenerator->generateSiebelRequestHttpHeaders($soapAction, strlen($requestPayload), $baseUrl);
        return array($requestPayload, $httpHeaders);
    }

    /**
     * Parse the XML response from Siebel server
     * @param string $extObj Related external object names
     * @param string $extAction Related external action, it could be read, update, create, or delete
     * @param String $response XML response from Siebel Server
     * @param array $responseFields List of fields that will be parsed from the response
     * @param array $errorFields List of error fields that contain error message
     * @param RNCPHP\Incident $incident Incident related to this response
     * @param RNCPHP\Contact $contact Contact related to this response
     * @return RNCPHP\RNObject Parsing result
     */
    private function parseSoapResponse($extObj, $extAction, $response, array $responseFields = null, array $errorFields = null, RNCPHP\Incident $incident = null, RNCPHP\Contact $contact = null) {
        if ($extObj === 'service_request_detail') {
            if ($extAction === 'read' || $extAction === 'create' || $extAction === 'update') {
                $parsingResult = $this->parseBasicSoapResponse($response, $responseFields, $errorFields);
            }
        } else if ($extObj === 'service_request_note') {
            if ($extAction === 'read') {
                $parsingResult = $this->parseListFromSoapResponse($response, 'ACTION', $responseFields, $errorFields);
            } else if ($extAction === 'create') {
                $parsingResult = $this->parseBasicSoapResponse($response, $responseFields, $errorFields);
            }
        } else if ($extObj === 'service_request_list') {
            if ($extAction === 'read') {
                $parsingResult = $this->parseListFromSoapResponse($response, 'SERVICEREQUEST', $responseFields, $errorFields);
            }
        } else if ($extObj === 'asset_management') {
            if ($extAction === 'read') {
                $parsingResult = $this->parseBasicSoapResponse($response, $responseFields, $errorFields, false);
            }
        }

        // check parse result
        if ($parsingResult->error) {
            $this->log->error("{$extObj} :: {$extAction} :: failed", __METHOD__, array($incident, $contact), json_encode($parsingResult->error->externalMessage));
        } else {
            $this->log->debug("{$extObj} :: {$extAction} :: success", __METHOD__, array($incident, $contact), json_encode($parsingResult->result));
        }

        return $parsingResult;
    }

    /**
     * Parse response contains a list of items, like SR list and Note list
     * @param string $responseString Response from Siebel server
     * @param string $itemOpenTag Open tag of a single item in the list 
     * @param array $responseFields List of fields will be parsed from the response
     * @param array $errorFields List of error fields that may contains the error message
     * @return RNCPHP\RNObject Parsing result
     */
    private function parseListFromSoapResponse($responseString, $itemOpenTag, array $responseFields = null, array $errorFields = null) {
        if ($responseString === null) {
            return $this->getResponseObject(null, null, "Unable to parse Siebel response :: soap response is empty");
        }

        $parser = xml_parser_create();
        $responseStruct = array();
        xml_parse_into_struct($parser, $responseString, $responseStruct);

        $responseFields = array_map('strtoupper', $responseFields);
        $response = null;
        $count = -1;
        foreach ($responseStruct as $item) {
            // check if return errors
            if (in_array($item['tag'], $errorFields) && $item['value'] !== null) {
                return $this->getResponseObject(null, null, $item['value']);
            }

            // check if start parsing a new item in the list
            if ($item['tag'] === $itemOpenTag && $item['type'] === 'open') {
                $count++;
                continue;
            }

            // only parse the tag in the required field list
            if (in_array($item['tag'], $responseFields) && $item['level'] === 8) {
                $response[$count][$item['tag']] = $item[value];
            }
        }

        return $this->getResponseObject($response, null);
    }

    /**
     * Parse Response contains a single item, for example a single Service Request
     * @param array $responseString Response from Siebel server
     * @param array $responseFields List of fields will be parsed from the response
     * @param array $errorFields List of error fields that may contains the error message
     * @param boolean $treatEmptyAsError Indicate if treat empty parsing result as an error
     * @return RNCPHP\RNObject Parsing result
     */
    private function parseBasicSoapResponse($responseString, array $responseFields = null, array $errorFields = null, $treateEmptyAsError = true) {
        if ($responseString === null) {
            return $this->getResponseObject(null, null, "Unable to parse Siebel response :: soap response is empty");
        }

        $responseStruct = array();
        $parser = xml_parser_create();
        xml_parse_into_struct($parser, $responseString, $responseStruct);

        $responseFields = array_map('strtoupper', $responseFields);
        $response = null;
        foreach ($responseStruct as $item) {
            // check if return error
            if (in_array($item['tag'], $errorFields) && $item['value'] !== null) {
                return $this->getResponseObject(null, null, $item['value']);
            }

            // only parse the tag in the required field list
            if (in_array($item['tag'], $responseFields)) {
                $response[$item['tag']] = $item[value];
            }
        }

        if ($response === null) {
            if ($treateEmptyAsError) {
                return $this->getResponseObject(null, null, "Parsing Error :: parsing result is empty");
            } else {
                return $this->getResponseObject(null, null);
            }
        } else {
            return $this->getResponseObject($response, 'is_array');
        }
    }

}
