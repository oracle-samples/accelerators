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
 *  date: Wed Sep  2 23:14:32 PDT 2015

 *  revision: rnw-15-8-fixes-release-01
 *  SHA1: $Id: ec9a2b914c6c449acc674eb036a3b5aa0f1ae8c8 $
 * *********************************************************************************************
 *  File: CurlRequest.php
 * ****************************************************************************************** */

namespace Custom\Libraries;

use RightNow\Connect\v1_2 as RNCPHP;

class CurlRequest {

    const CONNECT_TIMEOUT = 5000;
    const TIMEOUT = 50000;

    private $log;

    function __construct() {
        $CI = get_instance();
        $CI->load->library('Utility');

        $this->log = $CI->utility->getLogWrapper();
    }

    /**
     * Send soap request to 3rd party server using cURL/
     * The format of the response is
     * array(
     *  'status' => true/false,  // indicate if the curl request is successful
     *  'response' => string     // response or error message
     * )
     * @param string $url SOAP request URL
     * @param array $headers HTTP headers of the SOAP request
     * @param string $postString SOAP request payload
     * @param string $logMsg Log message
     * @param RNCPHP\Incident $incident Incident related to the request
     * @param RNCPHP\Contact $contact Contact related to the request
     * @return array Soap request result
     */
    function sendCurlSoapRequest($url, array $headers, $postString, $logMsg = null, RNCPHP\Incident $incident = null, RNCPHP\Contact $contact = null) {
        // log the request payload and HTTP headers
        $logNote = <<<LOG
REQUEST=>   
{$postString}

-----
ENDPOINT=>  {$url}
HTTP HEADERS=>
LOG;
        foreach ($headers as $header) {
            $logNote .= "\n" . $header;
        }
        $this->log->debug("{$logMsg} cURL Request", __METHOD__, array($incident, $contact), $logNote);

        // start time
        $startTime = intval(microtime(true) * 1000);

        // load curl
        if (!extension_loaded('curl') && !\load_curl()) {
            $result['status'] = false;
            $result['response'] = __METHOD__ . " :: Server Error :: cUrl can't be loaded";
            return $result;
        }

        // init curl
        $soapCURL = curl_init();
        curl_setopt($soapCURL, CURLOPT_URL, $url);
        curl_setopt($soapCURL, CURLOPT_CONNECTTIMEOUT, self::CONNECT_TIMEOUT);
        curl_setopt($soapCURL, CURLOPT_TIMEOUT, self::TIMEOUT);
        curl_setopt($soapCURL, CURLOPT_RETURNTRANSFER, true);
        curl_setopt($soapCURL, CURLOPT_SSL_VERIFYPEER, false);
        curl_setopt($soapCURL, CURLOPT_SSL_VERIFYHOST, false);
        curl_setopt($soapCURL, CURLOPT_POST, true);
        curl_setopt($soapCURL, CURLOPT_POSTFIELDS, $postString);
        curl_setopt($soapCURL, CURLOPT_HTTPHEADER, $headers);

        // execute cURL
        $response = curl_exec($soapCURL);
        $err = curl_error($soapCURL);

        // log the soap response
        $endTime = intval(microtime(true) * 1000);
        $this->log->debug("{$logMsg} cURL Response", __METHOD__, array($incident, $contact), $response, $endTime - $startTime);

        if (empty($err)) {
            $info = curl_getinfo($soapCURL);
            if ($info['http_code'] !== 200) {
                $result['status'] = false;
                $result['response'] = $response;
            } else {
                $result['status'] = true;
                $result['response'] = $response;
            }
        } else {
            $result['status'] = false;
            $result['response'] = $err;
            $this->log->error("{$logMsg} cURL Error", __METHOD__, array($incident, $contact), $err);
        }

        curl_close($soapCURL);

        return $result;
    }

}
