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
 *  date: Mon Nov 30 20:14:19 PST 2015

 *  revision: rnw-15-11-fixes-release-2
 *  SHA1: $Id: 2feb705c0c707d0884bf39508388e466349d78a9 $
 * *********************************************************************************************
 *  File: SiebelRequestGenerator.php
 * ****************************************************************************************** */

namespace Custom\Libraries;

class SiebelRequestGenerator {

    function __construct() {
        
    }

    /**
     * Compose HTTP headers for SOAP request
     * @param string $action Action name
     * @param int $contentLength Length of the payload
     * @param string $baseUrl The base URL of the Siebel server
     * @return array HTTP headers needed for the SOAP request 
     */
    function generateSiebelRequestHttpHeaders($action, $contentLength, $baseUrl) {
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
     * Generate the header of the get SR list request payload
     * @param string $username Siebel account username
     * @param string $password Siebel account password
     * @return string Header section in request payload
     */
    private function generateSiebelRequestHeader($username, $password) {
        return <<<HEADER
<soapenv:Header>
    <UsernameToken xmlns="http://siebel.com/webservices">{$username}</UsernameToken>
    <PasswordText xmlns="http://siebel.com/webservices">{$password}</PasswordText>
</soapenv:Header>
HEADER;
    }

    /**
     * Generate the soap request for getSRList API
     * @param string $username Siebel account username
     * @param string $password Siebel account password
     * @param int $contactPartyID Siebel account contact id
     * @param array $queryFields List of query fields in the payload
     * @return string SOAP request payload
     */
    function generateGetSRListRequest($username, $password, $contactPartyID, array $queryFields) {
        $requestHeader = $this->generateSiebelRequestHeader($username, $password);
        $requestBody = $this->generateGetSRListRequestBody($contactPartyID, $queryFields);

        return <<<REQUEST
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:cus="http://siebel.com/CustomUI" xmlns:quer="http://www.siebel.com/xml/WC_Contacts_IO/Query">
    {$requestHeader}
    {$requestBody}
</soapenv:Envelope>
REQUEST;
    }

    /**
     * Generate the body of the Siebel getSRList request payload
     * @param int $contactPartyID Siebel account contact id
     * @param array $queryFields List of query fields in the payload
     * @return string Request body
     */
    private function generateGetSRListRequestBody($contactPartyID, array $queryFields) {
        $queryEntries = "";
        foreach ($queryFields as $field) {
            $queryEntries .= "<quer:{$field} />";
        }

        return <<<BODY
<soapenv:Body>
    <cus:WC_Contacts_BSQueryPage_Input>
        <quer:ListOfWc_Contacts_Io pagesize="10" startrownum="0" recordcountneeded="TRUE">
            <quer:Contact searchspec="">
                <quer:Id sortorder="" sortsequence="">='{$contactPartyID}'</quer:Id>
                <quer:ListOfServiceRequest pagesize="100" startrownum="0" recordcountneeded="TRUE">
                    <quer:ServiceRequest searchspec="">
                    {$queryEntries}
                    </quer:ServiceRequest>
                </quer:ListOfServiceRequest>
            </quer:Contact>
        </quer:ListOfWc_Contacts_Io>
        <cus:LOVLanguageMode>LIC</cus:LOVLanguageMode>
        <cus:ViewMode>All</cus:ViewMode>
    </cus:WC_Contacts_BSQueryPage_Input>
 </soapenv:Body>
BODY;
    }

    /**
     * Generate the SOAP request payload for getSR API
     * @param string $username Siebel account username
     * @param string $password Siebel account password
     * @param int $srID Service Request ID
     * @param array $queryFields List of query fields in the payload
     * @return string SOAP request payload
     */
    function generateGetSRRequest($username, $password, $srID, array $queryFields) {
        $requestHeader = $this->generateSiebelRequestHeader($username, $password);
        $requestBody = $this->generateGetSRRequestBody($srID, $queryFields);

        return <<<REQUEST
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:cus="http://siebel.com/CustomUI" xmlns:quer="http://www.siebel.com/xml/WC_Service_Request_IO/Query">
    {$requestHeader}
    {$requestBody}
</soapenv:Envelope>
REQUEST;
    }

    /**
     * Generate the body of the Siebel getSR request payload
     * @param int $srID Service Request ID
     * @param array $queryFields List of query fields in the payload
     * @return string SOAP request body
     */
    private function generateGetSRRequestBody($srID, array $queryFields) {
        $queryEntries = "";
        foreach ($queryFields as $field) {
            if ($field === "Id") {
                $queryEntries .= "<quer:{$field}>='{$srID}'</quer:{$field}>";
            } else {
                $queryEntries .= "<quer:{$field} />";
            }
        }

        return <<<BODY
<soapenv:Body>
    <cus:WC_Service_Request_BSQueryPage_Input>
        <quer:ListOfWc_Service_Request_Io pagesize="10" startrownum="0" recordcountneeded="TRUE">
            <quer:ServiceRequest searchspec="">
            {$queryEntries}
            </quer:ServiceRequest>
        </quer:ListOfWc_Service_Request_Io>
        <cus:LOVLanguageMode>LIC</cus:LOVLanguageMode>
        <cus:ViewMode>All</cus:ViewMode>
    </cus:WC_Service_Request_BSQueryPage_Input>
</soapenv:Body>
BODY;
    }

    /**
     * Generate the SOAP request payload for the createSR API
     * @param string $username Siebel account username
     * @param string $password Siebel account password
     * @param array $srData Service Request field entries
     * @return string SOAP request payload
     */
    function generateCreateSRRequest($username, $password, array $srData) {
        $requestHeader = $this->generateSiebelRequestHeader($username, $password);
        $requestBody = $this->generateCreateSRRequestBody($srData);

        return <<<REQUEST
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" 
    xmlns:cus="http://siebel.com/CustomUI" 
    xmlns:data="http://www.siebel.com/xml/WC_Service_Request_IO/Data">
    {$requestHeader}
    {$requestBody}
</soapenv:Envelope>
REQUEST;
    }

    /**
     * Generate the body of the Siebel createSR request payload
     * @param array $srData Service Request field entries
     * @return string SOAP request body
     */
    private function generateCreateSRRequestBody(array $srData) {
        $dataEntries = '';
        foreach ($srData as $key => $value) {
            if ($value) {
                $dataEntries .= "<data:{$key}>{$value}</data:{$key}>";
            } else {
                $dataEntries .= "<data:{$key}/>";
            }
        }

        return <<<BODY
<soapenv:Body>
    <cus:WC_Service_Request_BSInsert_Input>
        <data:ListOfWc_Service_Request_Io>
            <data:ServiceRequest>
            {$dataEntries}
            </data:ServiceRequest>
        </data:ListOfWc_Service_Request_Io>
        <cus:LOVLanguageMode>LIC</cus:LOVLanguageMode>
        <cus:ViewMode>All</cus:ViewMode>
    </cus:WC_Service_Request_BSInsert_Input>
</soapenv:Body>
BODY;
    }

    /**
     * Generate the SOAP request payload for createNote API
     * @param string $username Siebel account username
     * @param string $password Siebel account password
     * @param int $srID Service Request ID
     * @param array $noteData Service Request note field entries
     * @return string SOAP request payload
     */
    function generateCreateNoteRequest($username, $password, $srID, array $noteData) {
        $requestHeader = $this->generateSiebelRequestHeader($username, $password);
        $requestBody = $this->generateCreateNoteRequestBody($srID, $noteData);

        return <<<REQUEST
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:cus="http://siebel.com/CustomUI" xmlns:data="http://www.siebel.com/xml/WC_Service_Request_IO/Data">
        {$requestHeader}
        {$requestBody}
</soapenv:Envelope>
REQUEST;
    }

    /**
     * Generate the body of the Siebel createNote request
     * @param int $srID Service Request ID
     * @param array $noteData Service Request note field entries
     * @return string SOAP request boday
     */
    private function generateCreateNoteRequestBody($srID, array $noteData) {
        $dataEntries = '';
        foreach ($noteData as $key => $value) {
            if (!is_null($value)) {
                $dataEntries .= "<data:{$key}>{$value}</data:{$key}>";
            } else {
                $dataEntries .= "<data:{$key}/>";
            }
        }

        return <<<BODY
<soapenv:Body>
    <cus:WC_Service_Request_BSInsert_Input>
        <data:ListOfWc_Service_Request_Io>
            <data:ServiceRequest>
                <data:Id>$srID</data:Id>
                <data:ListOfAction>
                    <data:Action>
                    {$dataEntries}
                    </data:Action>
                </data:ListOfAction>
           </data:ServiceRequest>
        </data:ListOfWc_Service_Request_Io>
        <cus:LOVLanguageMode>LIC</cus:LOVLanguageMode>
        <cus:ViewMode>All</cus:ViewMode>
    </cus:WC_Service_Request_BSInsert_Input>
</soapenv:Body>
BODY;
    }

    /**
     * Generate the SOAP request for getNote API
     * @param string $username Siebel account username
     * @param string $password Siebel account password
     * @param int $srID Service Request ID
     * @param array $queryFields List of query fields in the payload
     * @param int $pageSize Number of note that will return
     * @return string SOAP request payload
     */
    function generateGetNoteRequest($username, $password, $srID, array $queryFields, $pageSize) {
        $requestHeader = $this->generateSiebelRequestHeader($username, $password);
        $requestBody = $this->generateGetNoteRequestBody($srID, $queryFields, $pageSize);

        return <<<REQUEST
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:cus="http://siebel.com/CustomUI" xmlns:quer="http://www.siebel.com/xml/WC_Service_Request_IO/Query">
    {$requestHeader}
    {$requestBody}
</soapenv:Envelope>
REQUEST;
    }

    /**
     * Generate the body of getNote request
     * @param int $srID Service Request ID
     * @param array $queryFields List of query fields in the payload
     * @param int $pageSize Number of note that will return
     * @return string SOAP request body
     */
    private function generateGetNoteRequestBody($srID, array $queryFields, $pageSize) {
        $queryEntries = "";
        foreach ($queryFields as $field) {
            $queryEntries .= "<quer:{$field} />";
        }

        return <<<BODY
<soapenv:Body>
    <cus:WC_Service_Request_BSQueryPage_Input>
        <quer:ListOfWc_Service_Request_Io pagesize="10" startrownum="0" recordcountneeded="TRUE">
            <quer:ServiceRequest searchspec="">
                <quer:Id sortorder="" sortsequence="">='{$srID}'</quer:Id>
                <quer:ListOfAction pagesize="{$pageSize}" startrownum="" recordcountneeded="">
                    <quer:Action searchspec="">
                    {$queryEntries}
                    </quer:Action>
                </quer:ListOfAction>
            </quer:ServiceRequest>
        </quer:ListOfWc_Service_Request_Io>
        <cus:LOVLanguageMode>LIC</cus:LOVLanguageMode>
        <cus:ViewMode>All</cus:ViewMode>
    </cus:WC_Service_Request_BSQueryPage_Input>
</soapenv:Body>
BODY;
    }

    /**
     * Generate the SOAP request for updateSR API
     * @param string $username Siebel account username
     * @param string $password Siebel account password
     * @param array $srData Service Request field entries
     * @return string SOAP request payload
     */
    function generateUpdateSRRequest($username, $password, array $srData) {
        $requestHeader = $this->generateSiebelRequestHeader($username, $password);
        $requestBody = $this->generateUpdateSRRequestBody($srData);

        return <<<REQUEST
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:cus="http://siebel.com/CustomUI" xmlns:data="http://www.siebel.com/xml/WC_Service_Request_IO/Data">
    {$requestHeader}
    {$requestBody}
</soapenv:Envelope>
REQUEST;
    }

    /**
     * Generate the body of updateSR request
     * @param array $srData Service Request field entries
     * @return string SOAP request body
     */
    private function generateUpdateSRRequestBody(array $srData) {
        $dataEntries = "";
        foreach ($srData as $key => $value) {
            $dataEntries .= "<data:{$key}>{$value}</data:{$key}>";
        }

        return <<<BODY
<soapenv:Body>
    <cus:WC_Service_Request_BSUpdate_Input>
        <data:ListOfWc_Service_Request_Io lastpage="?" recordcount="?">
            <data:ServiceRequest operation="?">
            {$dataEntries}
            </data:ServiceRequest>
        </data:ListOfWc_Service_Request_Io>
        <cus:LOVLanguageMode>LIC</cus:LOVLanguageMode>
        <cus:ViewMode>All</cus:ViewMode>
    </cus:WC_Service_Request_BSUpdate_Input>
</soapenv:Body>
BODY;
    }

    /**
     * Generate the SOAP request for Asset Query API
     * @param string $username Siebel account username
     * @param string $password Siebel account password
     * @param array $queryFields List of query fields in the payload
     * @return string SOAP request payload
     */
    function generateAssetQueryRequest($username, $password, array $queryFields) {
        $requestHeader = $this->generateSiebelRequestHeader($username, $password);
        $requestBody = $this->generateAssetQueryRequestBody($queryFields);

        return <<<REQUEST
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ass="http://www.siebel.com/Service/FS/Assets" xmlns:quer="http://www.siebel.com/xml/Asset_Management_IO/Query">
    {$requestHeader}
    {$requestBody}
</soapenv:Envelope>
REQUEST;
    }

    /**
     * Generate the body of  Asset Query request
     * @param array $queryFields List of query fields in the payload
     * @return string SOAP request body
     */
    private function generateAssetQueryRequestBody(array $queryFields) {
        $queryEntries = "";
        foreach ($queryFields as $key => $value) {
            if ($value === null) {
                $queryEntries .= "<quer:{$key}/>";
            } else {
                $queryEntries .= "<quer:{$key}>='{$value}'</quer:{$key}>";
            }
        }

        return <<<BODY
 <soapenv:Body>
      <ass:AssetManagementQueryPage_Input>
         <ass:NamedSearchSpec />
         <quer:ListOfAsset_Management_Io pagesize="10" startrownum="" recordcountneeded="">
            <quer:AssetMgmt-Asset searchspec="">
            {$queryEntries}
            </quer:AssetMgmt-Asset>
         </quer:ListOfAsset_Management_Io>
         <ass:LOVLanguageMode>LIC</ass:LOVLanguageMode>
         <ass:ViewMode>All</ass:ViewMode>
      </ass:AssetManagementQueryPage_Input>
   </soapenv:Body>
BODY;
    }

}
