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
 *  SHA1: $Id: b0eb5f136c95113501c19dcf1d110add8957e8ee $
 * *********************************************************************************************
 *  File: MockApi.php
 * ****************************************************************************************** */

namespace Custom\Models;

use \PS\Log\v2 as PSLog;

require_once(APPPATH . "libraries/PSLog-2.0.php");

class MockApi extends \RightNow\Models\Base {

    function __construct() {
        parent::__construct();

        $this->CI->load->library('CurlRequest');
        $this->CI->load->library('MockRequestGenerator');

        $this->setUpLog();
    }

    /**
     * Set Up Log 
     * If in development mode, show error msg on page
     */
    private function setUpLog() {
        $this->log = new PSLog\Log();
        if (IS_DEVELOPMENT == true) {
            $this->log->stdOutputThreshold(PSLog\Severity::Error);
        }
    }

    /**
     * Generate the soap request, send to Mock server and parse the result
     * The return result is in format
     * $result = array(
     *      'status' => 'true/false',
     *      'response' => 'response or error msg'
     * )
     * @param array $extConfigVerb External config verb
     * @param string $extObj Related external object, it could be service_request_detail, service_request_list, or service_request_interaction
     * @param string $extAction Related external action, it could be read, update, create, or delete
     * @param object $incident Incident related to the request
     * @param object $contact Contact related to the request
     * @param array $requestParams Parameters required for the request
     * @param array $requestFieldData Object data requred for the request, it could be a SR detail or SR notes
     * @param array $responseFields Fields will be parsed from the response
     * @param array $errorFields Error fields that may contains the error message
     * @return array $result|null Soap request result
     */
    function sendSoapRequest($extConfigVerb, $extObj, $extAction, $incident, $contact, $requestParams, $requestFieldData, $responseFields, $errorFields) {
        // check input params
        if ($extConfigVerb == null || $extObj == null || $extAction == null) {
            $logMsg = "sendSoapRequest :: {$extObj} :: {$extAction} :: Failed to get ext_config_verb, extObj, or extAction";
            $this->log->notice($logMsg, array($incident, $contact));
            return null;
        }

        // get ext service seeting from config
        $extBaseUrl = $extConfigVerb["integration"]["ext_base_url"];
        $username = $extConfigVerb["integration"]["username"];
        $password = $extConfigVerb["integration"]["password"];
        $extServices = $extConfigVerb["integration"]["ext_services"];
        $extServiceSetting = $extServices[$extObj][$extAction];
        if ($extServiceSetting == null) {
            $logMsg = "{$extObj} :: {$extAction} :: sendSoapRequest :: Unable to get ext_service_setting";
            $this->log->notice($logMsg, array($incident, $contact));
            return null;
        }
        $soapAction = $extServiceSetting["soap_action"];
        $endpoint = $extBaseUrl . $extServiceSetting["relative_path"];
        $actionName = $extServiceSetting["service_name"];

        // prepare the log info for cURL
        $logMsg = "{$extObj} :: {$extAction} :: ";

        // prepare soap request string and http headers   
        list($requestString, $httpHeaders) = $this->generateSoapRequest($username, $password, $actionName, $soapAction, $requestParams, $requestFieldData);
        if ($requestString == null || $httpHeaders == null) {
            $logMsg = "{$extObj} :: {$extAction} :: sendSoapRequest :: Error when generate request String";
            $this->log->notice($logMsg, array($incident, $contact));
            return null;
        }

        // send curl request
        $result = $this->CI->curlrequest->sendCurlSoapRequest($endpoint, $httpHeaders, $requestString, $logMsg, $incident, $contact);

        // check result and parse the response
        if (!$result) {
            $this->log->notice($logMsg . "Return NULL", array(null, $contact));
            return null;
        } else if (!$result['status']) {
            $this->log->notice($logMsg . "Request failed.", array(null, $contact));
            return null;
        } else if ($result["status"]) {
            $response = $result["response"];
            $parseResult = $this->parseSoapResponse($actionName, $response, $responseFields, $errorFields, $logMsg, $incident, $contact);
            return $parseResult;
        }
    }

    /**
     * Generate the Soap request string and Http headers for Curl
     * @param string $username EBS account username, read from the config verb
     * @param string $password EBS account password, read from the config verb
     * @param string $actionName Function generate the corresponding request string based on the action name 
     * @param string $soapAction String used in the SOAP request headers
     * @param array $requestParams Parameters required for the request
     * @param array $requestFieldData Data requred for the request, it could be a SR detail or a SR note
     * @return array|null Request String and Http Headers
     */
    function generateSoapRequest($username, $password, $actionName, $soapAction, $requestParams, $requestFieldData) {
        switch ($actionName) {
            case "wssLookupSRsByContactPartyID":
                $contactPartyID = $requestParams["contactPartyID"];
                $maxRow = $requestParams["maxRow"];
                $requestString = $this->CI->mockrequestgenerator->composeLookupSRsByContactPartyIDRequest($username, $password, $contactPartyID, $maxRow);
                $httpHeaders = $this->CI->mockrequestgenerator->composeWSRequestHttpHeaders($soapAction, strlen($requestString));
                break;

            case "wssLookupSRBySR_ID":
                $srID = $requestParams["srID"];
                $contactPartyID = $requestParams["contactPartyID"];
                $requestString = $this->CI->mockrequestgenerator->composeWssLookUpSrBySrIdRequest($username, $password, $srID, $contactPartyID);
                $httpHeaders = $this->CI->mockrequestgenerator->composeWSRequestHttpHeaders($soapAction, strlen($requestString));
                break;

            case "wssCreateSR":
                $requestString = $this->CI->mockrequestgenerator->composeWssCreateSrRequest($username, $password, $requestFieldData);
                $httpHeaders = $this->CI->mockrequestgenerator->composeWSRequestHttpHeaders($soapAction, strlen($requestString));
                break;

            case "wssUpdateSR":
                $requestString = $this->CI->mockrequestgenerator->composeWssUpdateSrRequest($username, $password, $requestFieldData);
                $httpHeaders = $this->CI->mockrequestgenerator->composeWSRequestHttpHeaders($soapAction, strlen($requestString));
                break;

            case "wssLookupInteractionsBySR_ID":
                $srID = $requestParams["srID"];
                $contactPartyID = $requestParams["contactPartyID"];
                $maxRows = $requestParams["maxRows"];
                $this->log->debug(wssLookupInteractionsBySR_ID);
                $requestString = $this->CI->mockrequestgenerator->composeWssLookupInteractionsBySrIdRequest($username, $password, $srID, $contactPartyID, $maxRows);
                $httpHeaders = $this->CI->mockrequestgenerator->composeWSRequestHttpHeaders($soapAction, strlen($requestString));
                break;

            case "wssCreateInteractionsForSR_ID":
                $requestString = $this->CI->mockrequestgenerator->composeWssCreateInteractionsForSrIdRequest($username, $password, $requestFieldData);
                $httpHeaders = $this->CI->mockrequestgenerator->composeWSRequestHttpHeaders($soapAction, strlen($requestString));
                break;

            default:
                $requestString = null;
                $httpHeaders = null;
                break;
        }

        return array($requestString, $httpHeaders);
    }

    /**
     * Parse the xml response from Mock server
     * @param string $actionName Parsing the soap response differenctly based on the action name
     * @param String $response Xml response from Mock Server
     * @param array $responseFields List of gields that will be parsed from the response
     * @param array $errorFields List of error fields that may contains the error message
     * @param string $logMsg PSLog message prefix, contains the external object and external action info
     * @param object $incident Add incident info in the PSLog
     * @param object $contact Add contact info in the PSLog
     * @return array|null Parsing result
     */
    function parseSoapResponse($actionName, $response, $responseFields, $errorFields, $logMsg, $incident = null, $contact = null) {
        switch ($actionName) {
            case "wssLookupSRsByContactPartyID":
                $result = $this->parseListFromSoapResponse($response, "MOCKEBSSERVICEREQUEST", $responseFields, $errorFields);
                break;
            case "wssLookupSRBySR_ID":
                $result = $this->parseBasicMockResponse($response, $responseFields, $errorFields);
                break;

            case "wssCreateSR":
                $result = $this->parseBasicMockResponse($response, $responseFields, $errorFields);
                break;

            case "wssUpdateSR":
                $result = $this->parseBasicMockResponse($response, $responseFields, $errorFields);
                break;

            case "wssLookupInteractionsBySR_ID":
                $result = $this->parseListFromSoapResponse($response, "MOCKEBSINTERACTION", $responseFields, $errorFields);
                break;

            case "wssCreateInteractionsForSR_ID":
                $result = $this->parseBasicMockResponse($response, $responseFields, $errorFields);
                break;
        }

        // check parse result
        if (!$result) {
            $this->log->notice($logMsg . "Return NULL from response parser", array($incident, $contact));
            return null;
        } else if ($result['status'] == false) {
            $logNote = json_encode($result);
            $this->log->notice($logMsg . "Response parse failed", array($incident, $contact), $logNote);
            return $result;
        } else if ($result['status'] == true) {
            $logNote = json_encode($result);
            $this->log->debug($logMsg . "Response parse success", array($incident, $contact), $logNote);
            return $result;
        }

        return null;
    }

    /**
     * Parse list response from SOAP. like SR list and Note list
     * The return result is in format
     * $result = array(
     *      'status' => 'true/false',
     *      'response' => 'response or error msg'
     * )
     * @param string $responseString Response from EBS server
     * @param string $itemOpenTag Open tag of a single item in the list 
     * @param array $requiredFields List of fields will be parsed from the response
     * @param array $errorFields List of error fields that may contains the error message
     * @return array|null Parsing result
     */
    function parseListFromSoapResponse($responseString, $itemOpenTag, $requiredFields, $errorFields) {
        // check input
        if ($responseString == null) {
            return null;
        }

        $parser = xml_parser_create();
        $responseStruct = null;
        xml_parse_into_struct($parser, $responseString, $responseStruct);

        $response = null;
        $count = -1;
        foreach ($responseStruct as $item) {
            // remove the namespace
            $tag = str_ireplace('A:', '', $item['tag']);

            // check if return error msg
            if (in_array($tag, $errorFields) && $item['value'] != null) {
                $result['status'] = false;
                $result['response'] = $item['value'];
                return $result;
            }

            // check if a new item
            if ($tag == $itemOpenTag && $item['type'] == 'open') {
                $count++;
                continue;
            }

            // only parse the tag in the required field list
            if (in_array($tag, $requiredFields)) {
                $response[$count][$tag] = $item[value];
            }
        }

        $result['status'] = true;
        $result['response'] = $response;

        return $result;
    }

    /**
     * Parse Basic Response string(Not a list of object)
     * The return result is in format
     * $result = array(
     *      'status' => 'true/false',
     *      'response' => 'response or error msg'
     * )
     * @param array $responseString Response from Mock server
     * @param array $requiredFields List of fields will be parsed from the response
     * @param array $errorFields List of error fields that may contains the error message
     * @return array|null Parsing result
     */
    function parseBasicMockResponse($responseString, $requiredFields, $errorFields) {
        // check input
        if ($responseString == null) {
            return null;
        }

        $parser = xml_parser_create();
        $responseStruct = null;
        xml_parse_into_struct($parser, $responseString, $responseStruct);

        $response = null;
        foreach ($responseStruct as $item) {
            $tag = str_ireplace('A:', '', $item['tag']);

            if (in_array($tag, $errorFields) && $item['value'] != null) {
                $result['status'] = false;
                $result['response'] = $item['value'];
                return $result;
            }

            // only parse the tag in the required field list
            if (in_array($tag, $requiredFields)) {
                $response[$tag] = $item[value];
            }
        }

        if (is_null($response)) {
            $result['status'] = false;
            $result['response'] = null;
        } else {
            $result['status'] = true;
            $result['response'] = $response;
        }

        return $result;
    }

}
