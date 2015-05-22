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
 *  SHA1: $Id: bedb9a5f0857b1b88c5e0398111ffeadfee72fdf $
 * *********************************************************************************************
 *  File: EbsRequestGenerator.php
 * ****************************************************************************************** */

namespace Custom\Libraries;

class EbsRequestGenerator {

    const RESPONSIBILITY = 'SERVICE';
    const RESPAPPLICATION = 'CS';
    const SECURITYGROUP = 'STANDARD';
    const NLSLANGUAGE = 'AMERICAN';
    const ORGID = 204;

    function __construct() {
        
    }

    /**
     * Compose the HTTP headers for SOAP request
     * @param string $action Action name
     * @param int $contentLength Length of the request payload
     * @param string $baseUrl The base URL of the EBS server
     * @return array Http headers of the SOAP request 
     */
    function generateEbsRequestHttpHeaders($action, $contentLength, $baseUrl) {
        // example of baseUrl :  http://rws3220164.us.oracle.com:8055/
        // example of host: rws3220164.us.oracle.com:8055
        $host = preg_replace('#^https?://#', '', $baseUrl);
        if (substr($host, -1) == '/') {
            $host = substr($host, 0, -1);
        }

        $httpHeaders = array(
            'Content-Type: text/xml;charset=UTF-8',
            'SOAPAction: "' . $action . '"',
            'Content-Length: ' . $contentLength,
            'Host: ' . $host,
            'Connection: Keep-Alive',
            'User-Agent: Apache-HttpClient/4.1.1 (java 1.5)',
        );
        return $httpHeaders;
    }

    /**
     * Generate the SOA header
     * @return string SOA header
     */
    private function generateEbsRequestSoaHeader() {
        $responsibility = self::RESPONSIBILITY;
        $respApplication = self::RESPAPPLICATION;
        $securityGroup = self::SECURITYGROUP;
        $nlsLangurate = self::NLSLANGUAGE;
        $orgID = self::ORGID;

        return <<<SOAPHEADER
<SOAHeader xmlns="http://xmlns.oracle.com/apps/cs/soaprovider/plsql/cs_servicerequest_pub/">
    <Responsibility>{$responsibility}</Responsibility>
    <RespApplication>{$respApplication}</RespApplication>
    <SecurityGroup>{$securityGroup}</SecurityGroup>
    <NLSLanguage>{$nlsLangurate}</NLSLanguage>
    <Org_Id>{$orgID}</Org_Id>
</SOAHeader>
SOAPHEADER;
    }

    /**
     * Generate the Security header
     * @param string $username EBS username
     * @param string $password EBS password
     * @return string Security header
     */
    private function generateEbsRequestSecurityHeader($username, $password) {
        return <<<SECURITY
<wsse:Security soapenv:mustUnderstand="1" xmlns:wsse="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd">
    <wsse:UsernameToken wsu:Id="UsernameToken-2" xmlns:wsu="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd">
        <wsse:Username>{$username}</wsse:Username>
        <wsse:Password Type="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText">{$password}</wsse:Password>
    </wsse:UsernameToken>
</wsse:Security>
SECURITY;
    }

    /**
     * Generate the SOAP request for get SR API
     * @param string $username EBS username
     * @param string $password EBS password
     * @param int $srID Service Request ID
     * @return string SOAP request payload
     */
    function generateEbsGetSRRequest($username, $password, $srID) {
        $requestHeader = $this->generateEbsGetSRRequestHeader($username, $password);
        $requestBody = $this->generateEbsGetSRRequestBody($srID);

        return <<<REQUEST
<soapenv:Envelope 
    xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" 
    xmlns:cs="http://xmlns.oracle.com/apps/cs/soaprovider/plsql/cs_servicerequest_pub/" 
    xmlns:get="http://xmlns.oracle.com/apps/cs/soaprovider/plsql/cs_servicerequest_pub/get_sr_info/">
    {$requestHeader}
    {$requestBody}
</soapenv:Envelope>
REQUEST;
    }

    /**
     * Generate the header of the EBS get SR request
     * @param string $username EBS username
     * @param string $password EBS password
     * @return string Request header
     */
    private function generateEbsGetSRRequestHeader($username, $password) {
        $soaHeader = $this->generateEbsRequestSoaHeader();
        $securityHeader = $this->generateEbsRequestSecurityHeader($username, $password);

        return <<<HEADER
<soapenv:Header>
    {$soaHeader}
    {$securityHeader}
</soapenv:Header>
HEADER;
    }

    /**
     * Generate the body of the EBS get SR request
     * @param int $srID Service Request ID
     * @return string SOAP request body
     */
    private function generateEbsGetSRRequestBody($srID) {
        return <<<BODY
<soapenv:Body>
    <get:InputParameters>
    <get:P_API_VERSION>1.0</get:P_API_VERSION>
    <get:P_INCIDENT_ID>{$srID}</get:P_INCIDENT_ID>
    <get:P_INCIDENT_NUMBER>NULL</get:P_INCIDENT_NUMBER>
    <get:X_GETSR_OUT_REC/>
    <get:X_TASKS/>
    <get:X_NOTES/>
    <get:X_CONTACT/>
    </get:InputParameters>
</soapenv:Body>
BODY;
    }

    /**
     * Generate the SOAP request payload for the create SR API
     * @param string $username EBS username
     * @param string $password EBS password
     * @param array $srData Service Request field entries
     * @param int $contactPartyID EBS contact party id
     * @return string SOAP request payload
     */
    function generateEbsCreateSRRequest($username, $password, array $srData, $contactPartyID) {
        $requestHeader = $this->generateEbsCreateSRRequestHeader($username, $password);
        $requestBody = $this->generateEbsCreateSRRequestBody($srData, $contactPartyID);

        return <<<REQUEST
<soapenv:Envelope xmlns:soapenv = "http://schemas.xmlsoap.org/soap/envelope/"
    xmlns:cs = "http://xmlns.oracle.com/apps/cs/soaprovider/plsql/cs_servicerequest_pub/"
    xmlns:cre = "http://xmlns.oracle.com/apps/cs/soaprovider/plsql/cs_servicerequest_pub/create_servicerequest/"
    xmlns:soap = "http://schemas.xmlsoap.org/soap/envelope/"
    xmlns:xsi = "http://www.w3.org/2001/XMLSchema-instance"
    xmlns:xsd = "http://www.w3.org/2001/XMLSchema"
    xmlns:wsa = "http://schemas.xmlsoap.org/ws/2004/08/addressing"
    xmlns:wsse = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"
    xmlns:wsu = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd">
    {$requestHeader}
    {$requestBody}
</soapenv:Envelope>
REQUEST;
    }

    /**
     * Generate the header of the EBS create SR request
     * @param string $username EBS username
     * @param string $password EBS password
     * @return string Request header
     */
    private function generateEbsCreateSRRequestHeader($username, $password) {
        $soaHeader = $this->generateEbsRequestSoaHeader();
        $securityHeader = $this->generateEbsRequestSecurityHeader($username, $password);

        return <<<HEADER
<soapenv:Header>
    {$soaHeader}
    {$securityHeader}
</soapenv:Header>
HEADER;
    }

    /**
     * Generate the body of the EBS create SR request
     * @param array $srData Service Request field entries
     * @param int $contactPartyID EBS contact party id
     * @return string SOAP request body
     */
    private function generateEbsCreateSRRequestBody(array $srData, $contactPartyID) {
        $serviceRequestRec = '';
        foreach ($srData as $key => $value) {
            if ($value) {
                $serviceRequestRec .= "<{$key}>{$value}</{$key}>";
            } else {
                $serviceRequestRec .= "<{$key} xsi:nil=\"true\"/>";
            }
        }

        $contact = <<<CONTACT
<P_CONTACTS>
    <P_CONTACTS_ITEM>
        <PARTY_ID>{$contactPartyID}</PARTY_ID>
        <CONTACT_POINT_ID></CONTACT_POINT_ID>
        <CONTACT_POINT_TYPE>PHONE</CONTACT_POINT_TYPE>
        <PRIMARY_FLAG>Y</PRIMARY_FLAG>
        <CONTACT_TYPE>PARTY_RELATIONSHIP</CONTACT_TYPE>
        <PARTY_ROLE_CODE>CONTACT</PARTY_ROLE_CODE>
    </P_CONTACTS_ITEM>
</P_CONTACTS>
CONTACT;

        return <<<BODY
<soapenv:Body>
    <InputParameters xmlns = "http://xmlns.oracle.com/apps/cs/soaprovider/plsql/cs_servicerequest_pub/create_servicerequest/">
        <P_API_VERSION>4.0</P_API_VERSION>
        <P_INIT_MSG_LIST>T</P_INIT_MSG_LIST>
        <P_COMMIT>T</P_COMMIT>
        <P_REQUEST_NUMBER/>
        <P_SERVICE_REQUEST_REC>{$serviceRequestRec}</P_SERVICE_REQUEST_REC>
        <P_NOTES/>
        {$contact}
        <P_AUTO_ASSIGN/>
        <P_AUTO_GENERATE_TASKS/>
        <P_DEFAULT_CONTRACT_SLA_IND>Y</P_DEFAULT_CONTRACT_SLA_IND>
    </InputParameters>
</soapenv:Body>
BODY;
    }

    /**
     * Generate the SOAP request for the Update SR API
     * @param string $username EBS username
     * @param string $password EBS password
     * @param array $srData Service Request field entries
     * @param int $srID Service Request ID
     * @param int $srObjVerNum Service Request version number
     * @return string SOAP request payload
     */
    function generateEbsUpdateSRRequest($username, $password, array $srData, $srID, $srObjVerNum) {
        $requestHeader = $this->generateEbsUpdateSRRequestHeader($username, $password);
        $requestBody = $this->generateEbsUpdateSRRequestBody($srData, $srID, $srObjVerNum);

        return <<<REQUEST
<soap:Envelope xmlns:soap = "http://schemas.xmlsoap.org/soap/envelope/"
    xmlns:xsi = "http://www.w3.org/2001/XMLSchema-instance"
    xmlns:xsd = "http://www.w3.org/2001/XMLSchema"
    xmlns:wsa = "http://schemas.xmlsoap.org/ws/2004/08/addressing"
    xmlns:wsse = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd" 
    xmlns:wsu = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd">
    {$requestHeader}
    {$requestBody}
</soap:Envelope>
REQUEST;
    }

    /**
     * Generate the header of the EBS update SR request
     * @param string $username EBS username
     * @param string $password EBS password
     * @return string Request header
     */
    private function generateEbsUpdateSRRequestHeader($username, $password) {
        $soaHeader = $this->generateEbsRequestSoaHeader();

        return <<<HEADER
<soap:Header>
    {$soaHeader}
                
    <wsa:Action>http://rws3220164.us.oracle.com:8055/webservices/SOAProvider/plsql/cs_servicerequest_pub/</wsa:Action>
    <wsa:MessageID>urn:uuid:5c817c4d-5725-4d65-8770-47518643e402</wsa:MessageID>
    <wsa:ReplyTo><wsa:Address>http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</wsa:Address></wsa:ReplyTo>
    <wsa:To>http://rws3220164.us.oracle.com:8055/webservices/SOAProvider/plsql/cs_servicerequest_pub/</wsa:To>
    
    <wsse:Security soap:mustUnderstand="1" xmlns:wsse="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd">
        <wsse:UsernameToken wsu:Id="UsernameToken-2" xmlns:wsu="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd">
            <wsse:Username>{$username}</wsse:Username>
            <wsse:Password Type="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText">{$password}</wsse:Password>
         </wsse:UsernameToken>
    </wsse:Security> 
</soap:Header>
HEADER;
    }

    /**
     * Generate the SOAP request body for update SR API
     * @param array $srData Service Request field entries
     * @param int $srID Service Request ID
     * @param int $srObjVerNum Service Request version number
     * @return string SOAP request body
     */
    private function generateEbsUpdateSRRequestBody(array $srData, $srID, $srObjVerNum) {
        $serviceRequestRec = '';
        foreach ($srData as $key => $value) {
            if ($value) {
                $serviceRequestRec .= "<{$key}>{$value}</{$key}>";
            } else {
                $serviceRequestRec .= "<{$key}></{$key}>";
            }
        }

        //P_LAST_UPDATE_DATE is hardcoded but it will be edited by EBS automaticlly
        return <<<BODY
<soap:Body>
    <InputParameters xmlns = "http://xmlns.oracle.com/apps/cs/soaprovider/plsql/cs_servicerequest_pub/update_servicerequest/">
        <P_API_VERSION>4.0</P_API_VERSION>
        <P_INIT_MSG_LIST>T</P_INIT_MSG_LIST>
        <P_COMMIT>T</P_COMMIT>
        <P_REQUEST_ID>{$srID}</P_REQUEST_ID>
        <P_REQUEST_NUMBER></P_REQUEST_NUMBER>
        <P_OBJECT_VERSION_NUMBER>{$srObjVerNum}</P_OBJECT_VERSION_NUMBER>
        <P_SERVICE_REQUEST_REC>{$serviceRequestRec}</P_SERVICE_REQUEST_REC>
        <P_NOTES/>
        <P_CONTACTS></P_CONTACTS>
        <P_AUTO_ASSIGN/>
        <P_LAST_UPDATE_DATE>2014-10-21T16:38:33Z</P_LAST_UPDATE_DATE>
        <P_LAST_UPDATED_BY>0</P_LAST_UPDATED_BY>
    </InputParameters>
</soap:Body>
BODY;
    }

    /**
     * Generate the SOAP request payload for create Note API
     * @param string $username EBS username
     * @param string $password EBS password
     * @param array $noteData Service Request note field entries
     * @return string SOAP request payload
     */
    function generateEbsCreateNoteRequest($username, $password, array $noteData) {
        $requestHeader = $this->generateEbsCreateNoteRequestHeader($username, $password);
        $requestBody = $this->generateEbsCreateNoteRequestBody($noteData);

        return <<<REQUEST
<soapenv:Envelope
    xmlns:soapenv = "http://schemas.xmlsoap.org/soap/envelope/"
    xmlns:cs = "http://xmlns.oracle.com/apps/cac/soaprovider/plsql/jtf_notes_pub/"
    xmlns:sec = "http://xmlns.oracle.com/apps/cac/soaprovider/plsql/jtf_notes_pub/secure_create_note/">
    {$requestHeader}
    {$requestBody}
</soapenv:Envelope>
REQUEST;
    }

    /**
     * Generate the header of the EBS create Note request
     * @param string $username EBS username
     * @param string $password EBS password
     * @return string Request header
     */
    private function generateEbsCreateNoteRequestHeader($username, $password) {
        $responsibility = self::RESPONSIBILITY;
        $respApplication = self::RESPAPPLICATION;
        $securityGroup = self::SECURITYGROUP;
        $nlsLangurate = self::NLSLANGUAGE;
        $orgID = self::ORGID;
        $securityHeader = $this->generateEbsRequestSecurityHeader($username, $password);

        return <<<HEADER
<soapenv:Header>
    <cs:SOAHeader xmlns="http://xmlns.oracle.com/apps/cac/soaprovider/plsql/jtf_notes_pub/">
         <cs:Responsibility>{$responsibility}</cs:Responsibility>
         <cs:RespApplication>{$respApplication}</cs:RespApplication>
         <cs:SecurityGroup>{$securityGroup}</cs:SecurityGroup>
         <cs:NLSLanguage>{$nlsLangurate}</cs:NLSLanguage>
         <cs:Org_Id>{$orgID}</cs:Org_Id>
      </cs:SOAHeader>
      {$securityHeader}
</soapenv:Header>
HEADER;
    }

    /**
     * Generate the SOAP request header for create Note API
     * @param array $noteData Service Request note field entries
     * @return string SOAP request body
     */
    private function generateEbsCreateNoteRequestBody(array $noteData) {
        $dataEntries = '';
        foreach ($noteData as $key => $value) {
            if (!is_null($value)) {
                $dataEntries .= "<sec:{$key}>{$value}</sec:{$key}>";
            } else {
                $dataEntries .= "<sec:{$key}/>";
            }
        }

        return <<<BODY
<soapenv:Body>
    <sec:InputParameters>
        {$dataEntries}
        <sec:P_JTF_NOTE_CONTEXTS_TAB>
        </sec:P_JTF_NOTE_CONTEXTS_TAB>
        <sec:P_USE_AOL_SECURITY/>
    </sec:InputParameters>
</soapenv:Body>
BODY;
    }

    /**
     * Generate the SOAP request payload for get Note API
     * @param string $username EBS username
     * @param string $password EBS password
     * @param int $srID Service Request ID
     * @return string SOAP request payload
     */
    function generateEbsGetNotesRequest($username, $password, $srID) {
        $requestHeader = $this->generateEbsGetNotesRequestHeader($username, $password);
        $requestBody = $this->generateEbsGetNotesRequestBody($srID);

        return <<<REQUEST
<soapenv:Envelope xmlns:soapenv = "http://schemas.xmlsoap.org/soap/envelope/"
    xmlns:cs = "http://xmlns.oracle.com/apps/cs/soaprovider/plsql/cs_servicerequest_pub/"
    xmlns:get = "http://xmlns.oracle.com/apps/cs/soaprovider/plsql/cs_servicerequest_pub/get_sr_notes_details/">
    {$requestHeader}
    {$requestBody}
</soapenv:Envelope>
REQUEST;
    }

    /**
     * Generate the header of the EBS get Note request
     * @param string $username EBS username
     * @param string $password EBS password
     * @return string Request header
     */
    private function generateEbsGetNotesRequestHeader($username, $password) {
        $soaHeader = $this->generateEbsRequestSoaHeader();
        $securityHeader = $this->generateEbsRequestSecurityHeader($username, $password);

        return <<<HEADER
<soapenv:Header>
    {$soaHeader}
    {$securityHeader}
</soapenv:Header>
HEADER;
    }

    /**
     * Generate the body of the EBS get Note request
     * @param int $srID Service Request ID
     * @return string Request body
     */
    private function generateEbsGetNotesRequestBody($srID) {
        return <<<BODY
<soapenv:Body>
    <get:InputParameters>
    <get:P_API_VERSION>1.0</get:P_API_VERSION>
    <get:P_INCIDENT_ID>{$srID}</get:P_INCIDENT_ID>
    <get:P_INCIDENT_NUMBER>NULL</get:P_INCIDENT_NUMBER>
    </get:InputParameters>
</soapenv:Body>
BODY;
    }

    /**
     * Generate the SOAP request for get SR list API
     * @param string $username EBS username
     * @param string $password EBS password
     * @param int $contactPartyID EBS contact party ID
     * @return string SOAP request payload
     */
    function generateEbsGetSRListRequest($username, $password, $contactPartyID) {
        $requestHeader = $this->generateEbsGetSRListRequestHeader($username, $password);
        $requestBody = $this->generateEbsGetSRListRequestBody($contactPartyID);

        return <<<REQUEST
<soapenv:Envelope xmlns:soapenv = "http://schemas.xmlsoap.org/soap/envelope/"
    xmlns:cs = "http://xmlns.oracle.com/apps/cs/soaprovider/plsql/cs_servicerequest_pub/"
    xmlns:get = "http://xmlns.oracle.com/apps/cs/soaprovider/plsql/cs_servicerequest_pub/get_srinfo_bycontact/">
    {$requestHeader}
    {$requestBody}
</soapenv:Envelope>
REQUEST;
    }

    /**
     * Generate the header of the EBS get SR list request
     * @param string $username EBS username
     * @param string $password EBS password
     * @return string Request header
     */
    private function generateEbsGetSRListRequestHeader($username, $password) {
        $soaHeader = $this->generateEbsRequestSoaHeader();
        $securityHeader = $this->generateEbsRequestSecurityHeader($username, $password);

        return <<<HEADER
<soapenv:Header>
    {$soaHeader}
    {$securityHeader}
</soapenv:Header>
HEADER;
    }

    /**
     * Generate the body of the EBS get SR list request
     * @param int $contactPartyID EBS contact party ID
     * @return string Request body
     */
    private function generateEbsGetSRListRequestBody($contactPartyID) {
        return <<<BODY
<soapenv:Body>
    <get:InputParameters>
        <get:P_API_VERSION>1.0</get:P_API_VERSION>
        <get:P_INIT_MSG_LIST>T</get:P_INIT_MSG_LIST>
        <get:P_COMMIT>T</get:P_COMMIT>
        <get:P_CONTACT>{$contactPartyID}</get:P_CONTACT>
    </get:InputParameters>
</soapenv:Body>
BODY;
    }

    /**
     * Generate the SOAP request payload for get Item Instance API
     * @param string $username EBS username
     * @param string $password EBS password
     * @param string $serialNumber Serial Number
     * @param string $partyID EBS contact party id
     * @return string SOAP request payload
     */
    function generateEbsGetItemInstanceRequest($username, $password, $serialNumber, $partyID) {
        $requestHeader = $this->generateEbsGetItemInstanceRequestHeader($username, $password);
        $requestBody = $this->generateEbsGetItemInstanceRequestBody($serialNumber, $partyID);

        return <<<REQUEST
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/" 
    xmlns:wsa="http://schemas.xmlsoap.org/ws/2004/08/addressing" 
    xmlns:wsse="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd" 
    xmlns:wsu="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd" 
    xmlns:xsd="http://www.w3.org/2001/XMLSchema" 
    xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">  
    {$requestHeader}
    {$requestBody}
</soap:Envelope>
REQUEST;
    }

    /**
     * Generate the header of EBS get Item Instance request
     * @param string $username EBS username
     * @param string $password EBS password
     * @return string request header
     */
    private function generateEbsGetItemInstanceRequestHeader($username, $password) {
        $soaHeader = $this->generateEbsRequestSoaHeader();
        $securityHeader = $this->generateEbsRequestSecuritySoapHeader($username, $password);

        return <<<HEADER
<soap:Header>
    {$soaHeader}
    {$securityHeader}
</soap:Header>
HEADER;
    }
    
    /**
     * Generate the body of EBS get Item Instance request
     * @param string $serialNumber Serial Number
     * @param string $partyID EBS contact party id
     * @return string request body
     */
    private function generateEbsGetItemInstanceRequestBody($serialNumber, $partyID) {
        return <<<REQUEST
  <soap:Body>
      <InputParameters xmlns="http://xmlns.oracle.com/apps/csi/soaprovider/plsql/csi_item_instance_pub/get_instances_by_item_serial/">
         <P_API_VERSION>1</P_API_VERSION>
         <P_COMMIT>F</P_COMMIT>
         <P_INIT_MSG_LIST>T</P_INIT_MSG_LIST>
         <P_VALIDATION_LEVEL>1</P_VALIDATION_LEVEL>
         <P_SERIAL_NUMBER>{$serialNumber}</P_SERIAL_NUMBER>
         <P_PARTY_ID>{$partyID}</P_PARTY_ID>
         <P_PARTY_REL_TYPE_CODE>OWNER</P_PARTY_REL_TYPE_CODE>
         <P_RESOLVE_ID_COLUMNS>T</P_RESOLVE_ID_COLUMNS>
         <P_ACTIVE_INSTANCE_ONLY>F</P_ACTIVE_INSTANCE_ONLY>
      </InputParameters>
   </soap:Body>
REQUEST;
    }

    /**
     * Generate the security header
     * @param string $username EBS username
     * @param string $password EBS password
     * @return string Security header
     */
    private function generateEbsRequestSecuritySoapHeader($username, $password) {
        return <<<SECURITY
<wsse:Security soap:mustUnderstand="1" xmlns:wsse="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd">
    <wsse:UsernameToken wsu:Id="UsernameToken-2" xmlns:wsu="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd">
        <wsse:Username>{$username}</wsse:Username>
        <wsse:Password Type="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText">{$password}</wsse:Password>
    </wsse:UsernameToken>
</wsse:Security>
SECURITY;
    }


}
