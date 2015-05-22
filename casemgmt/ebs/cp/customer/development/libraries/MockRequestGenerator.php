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
 *  SHA1: $Id: 9e2b34706523906969e2ee9928150d7d5b9c268a $
 * *********************************************************************************************
 *  File: MockRequestGenerator.php
 * ****************************************************************************************** */

namespace Custom\Libraries;

class MockRequestGenerator {

    /**
     * This library can be loaded a few different ways depending on where it's being called:
     *
     * From a widget or model: $this->CI->load->library('Sample');
     *
     * From a custom controller: $this->load->library('Sample');
     *
     * Everywhere else, including other libraries: $CI = get_instance();
     *                                             $CI->load->library('Sample')->sampleFunction();
     */
    function __construct() {
        
    }

    /**
     * Once loaded as described above, this function would be called in the following ways, depending on where it's being called:
     *
     * From a widget or model: $this->CI->sample->sampleFunction();
     *
     * From a custom controller: $this->sample->sampleFunction();
     *
     * Everywhere else, including other libraries: $CI = get_instance();
     *                                             $CI->sample->sampleFunction();
     */

    /**
     * Compose the credential component in soap request header
     * @param string $usr Mock server account username
     * @param string $pwd Mock server account password
     * @return string Credential header contains username and password
     */
    function composeCredentialInHeader($usr, $pwd) {
        return <<<CREDENTIAL
<Credential>
    <username>$usr</username>
    <password>$pwd</password>
</Credential>
CREDENTIAL;
    }

    /**
     * Compose the HTTP request headers in an array
     * @param string $action Action name
     * @param int $contentLength Length of the request string
     * @return array Http headers list
     */
    function composeWSRequestHttpHeaders($action, $contentLength) {
        return array(
            'Accept-Encoding: gzip,deflate',
            'Content-Type: text/xml;charset=UTF-8',
            'SOAPAction: "http://tempuri.org/IEBSMockSvc/' . $action . '"',
            'Content-Length: ' . $contentLength,
            'Proxy-Connection: Keep-Alive',
            'User-Agent: Apache-HttpClient/4.1.1 (java 1.5)',
        );
    }

    /**
     * Compose SOAP request post string for lookupSrsByContactPartyId API
     * @param string $usr Mock server account username
     * @param string $pwd Mock server account password
     * @param int $contactPartyID Mock server account contact party id
     * @param int $maxRow Maximum number of SR will be returned
     * @return string Soap request payload
     */
    function composeLookupSRsByContactPartyIDRequest($usr, $pwd, $contactPartyID, $maxRow) {
        $credential = $this->composeCredentialInHeader($usr, $pwd);
        return <<<REQUEST
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:tem="http://tempuri.org/">
    <soapenv:Header>$credential</soapenv:Header>
    <soapenv:Body>
        <tem:wssLookupSRsByContactPartyID>
            <tem:contact_id>$contactPartyID</tem:contact_id>
            <tem:maxRows>$maxRow</tem:maxRows>
            <tem:noIncident>true</tem:noIncident>
        </tem:wssLookupSRsByContactPartyID>
    </soapenv:Body>
</soapenv:Envelope>
REQUEST;
    }

    /**
     * Compose SOAP request string for WssLookUpSrBySrId API
     * @param string $usr Mock server account username
     * @param string $pwd Mock server account password
     * @param int $srId Service Request ID
     * @param int $contactPartyID Mock server account contact party id
     * @return string Soap request payload
     */
    function composeWssLookUpSrBySrIdRequest($usr, $pwd, $srId, $contactPartyID) {
        $credential = $this->composeCredentialInHeader($usr, $pwd);
        return <<<REQUEST
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:tem="http://tempuri.org/">
    <soapenv:Header>$credential</soapenv:Header> 
    <soapenv:Body>
       <tem:wssLookupSRBySR_ID>
          <tem:sr_id>$srId</tem:sr_id>
          <tem:contactPartyID>$contactPartyID</tem:contactPartyID>    
       </tem:wssLookupSRBySR_ID>
    </soapenv:Body>
</soapenv:Envelope>
REQUEST;
    }

    /**
     * Compose SOAP request string for createSr API
     * @param string $usr Mock server account username
     * @param string $pwd Mock server account password
     * @param array $srData Service Request data, key is the field name and value is the field value
     * @return string|null Soap request payload
     */
    function composeWssCreateSrRequest($usr, $pwd, $srData) {
        // check input
        if (!$srData) {
            return null;
        }

        $requiredFields = '';
        foreach ($srData as $key => $value) {
            $requiredFields .= "<ebs:{$key}>{$value}</ebs:{$key}>";
        }

        $credential = $this->composeCredentialInHeader($usr, $pwd);
        return <<<REQUEST
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/"
    xmlns:tem="http://tempuri.org/" 
    xmlns:ebs="http://schemas.datacontract.org/2004/07/EBSMockSvcLib">
    <soapenv:Header>$credential</soapenv:Header> 
    <soapenv:Body>
        <tem:wssCreateSR>        
            <tem:req>$requiredFields</tem:req>
        </tem:wssCreateSR>
    </soapenv:Body>
</soapenv:Envelope>
REQUEST;
    }

    /**
     * Compose SOAP request string for updateSr API
     * @param string $usr Mock server account username
     * @param string $pwd Mock server account password
     * @param array $srData Service Request data, key is the field name and value is the field value
     * @return string|null Soap request payload
     */
    function composeWssUpdateSrRequest($usr, $pwd, $srData) {
        if (!$srData) {
            return null;
        }

        $requiredFields = '';
        foreach ($srData as $key => $value) {
            $requiredFields .= "<ebs:{$key}>{$value}</ebs:{$key}>";
        }

        $credential = $this->composeCredentialInHeader($usr, $pwd);
        return <<<REQUEST
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" 
    xmlns:tem="http://tempuri.org/" 
    xmlns:ebs="http://schemas.datacontract.org/2004/07/EBSMockSvcLib">
    <soapenv:Header>$credential</soapenv:Header> 
    <soapenv:Body>
        <tem:wssUpdateSR>
            <tem:req>$requiredFields</tem:req>
        </tem:wssUpdateSR>
    </soapenv:Body>
</soapenv:Envelope>
REQUEST;
    }

    /**
     * Compose the request string for WssLookupInteractionsBySrId API
     * @param string $usr Mock server account username
     * @param string $pwd Mock server account password
     * @param int $srId Service Request ID
     * @param int $contactPartyID Mock server account contact party id
     * @param int $maxRow Maximum number of SR will be returned
     * @return string Soap request payload
     */
    function composeWssLookupInteractionsBySrIdRequest($usr, $pwd, $srId, $contactPartyID, $maxRow) {
        $credential = $this->composeCredentialInHeader($usr, $pwd);

        $requiredFields = <<<REQUESTFIELDS
<tem:sr_id>$srId</tem:sr_id>
<tem:contactPartyID>$contactPartyID</tem:contactPartyID>
<tem:maxRows>$maxRow</tem:maxRows>
REQUESTFIELDS;

        return <<<REQUEST
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" 
    xmlns:tem="http://tempuri.org/" 
    xmlns:ebs="http://schemas.datacontract.org/2004/07/EBSMockSvcLib">
    <soapenv:Header>$credential</soapenv:Header>      
    <soapenv:Body>
        <tem:wssLookupInteractionsBySR_ID>$requiredFields</tem:wssLookupInteractionsBySR_ID>
    </soapenv:Body>
</soapenv:Envelope>
REQUEST;
    }

    /**
     * Compose the request string for wssCreateInteractionsForSR_ID API
     * @param string $usr Mock server account username
     * @param string $pwd Mock server account password
     * @param array $interactionData Interaction data contains key and value
     * @return string|null
     */
    function composeWssCreateInteractionsForSrIdRequest($usr, $pwd, $interactionData) {
        if (!$interactionData) {
            return null;
        }

        $requiredFields = '';
        foreach ($interactionData as $key => $value) {
            $requiredFields .= "<ebs:{$key}>{$value}</ebs:{$key}>";
        }

        $credential = $this->composeCredentialInHeader($usr, $pwd);

        return <<<REQUEST
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" 
    xmlns:tem="http://tempuri.org/" 
        xmlns:ebs="http://schemas.datacontract.org/2004/07/EBSMockSvcLib">
    <soapenv:Header>$credential</soapenv:Header> 
    <soapenv:Body>
        <tem:wssCreateInteractionsForSR_ID>
            <tem:req>$requiredFields</tem:req>
        </tem:wssCreateInteractionsForSR_ID>
    </soapenv:Body>
</soapenv:Envelope>
REQUEST;
    }

}
