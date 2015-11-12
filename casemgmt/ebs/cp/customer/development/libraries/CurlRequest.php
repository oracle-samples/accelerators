<?php

/** *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + EBS Enhancement
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  EBS release: 12.1.3
 *  reference: 150505-000099, 150420-000127
 *  date: Thu Nov 12 00:52:34 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: 8d00a8add88108521e93b343755146debb617df1 $
 * *********************************************************************************************
 *  File: CurlRequest.php
 * ****************************************************************************************** */

namespace Custom\Libraries;

use RightNow\Connect\v1_2 as RNCPHP;

class CurlRequest {

    const CONNECT_TIMEOUT = 10;
    const TIMEOUT = 30;

    private $log;

    function __construct() {
        $CI = get_instance();
        $CI->load->library('Utility');
        $this->log = $CI->utility->getLogWrapper();
    }

    /**
     * Send a SOAP (or an HTTP!) request to 3rd party server using cURL.
     * The format of the response is
     * array(
     *  'status' => true/false,  // indicate if the curl request is successful
     *  'response' => string     // response or error message
     * )
     * @param string $url SOAP request URL
     * @param array $headers HTTP headers of the SOAP request
     * @param string $postString Request payload; if you set this to null, it sends a GET.
     * @param string $logMsg Log message
     * @param RNCPHP\Incident $incident Incident related to the request
     * @param RNCPHP\Contact $contact Contact related to the request
     * @return array SOAP request result
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
            $logNote .= '\n' . $header;
        }
        $this->log->debug($logMsg . 'cURL Request', __METHOD__, array($incident, $contact), $logNote);

        // start time
        $startTime = intval(microtime(true) * 1000);

        // load curl
        if (!extension_loaded('curl') && !\load_curl()) {
            $result['status'] = false;
            $result['response'] = __METHOD__ . ' :: Server Error :: cUrl cannot be loaded';
            $this->log->error($logMsg . ' :: Server Error :: cUrl can not be loaded', __METHOD__, array($incident, $contact), $logNote);
            return $result;
        }

        // init curl
        $soapCURL = curl_init();
        curl_setopt($soapCURL, CURLOPT_URL, $url);
        curl_setopt($soapCURL, CURLOPT_CONNECTTIMEOUT, CONNECT_TIMEOUT);
        curl_setopt($soapCURL, CURLOPT_TIMEOUT, TIMEOUT);
        curl_setopt($soapCURL, CURLOPT_RETURNTRANSFER, true);
        curl_setopt($soapCURL, CURLOPT_SSL_VERIFYPEER, false);
        curl_setopt($soapCURL, CURLOPT_SSL_VERIFYHOST, false);
        curl_setopt($soapCURL, CURLOPT_HTTPHEADER, $headers);
        // addtnl param: Method
        if ($postString === null) {
            curl_setopt($soapCURL, CURLOPT_HTTPGET, true);
            curl_setopt($soapCURL, CURLOPT_HTTPPROXYTUNNEL, true);
            curl_setopt($soapCURL, CURLOPT_PROXY, 'www-proxy.us.oracle.com');
            curl_setopt($soapCURL, CURLOPT_PROXYPORT, 80);
            curl_setopt($soapCURL, CURLOPT_PROXYTYPE, CURLPROXY_HTTP);
        } else {
            curl_setopt($soapCURL, CURLOPT_POST, true);
            curl_setopt($soapCURL, CURLOPT_POSTFIELDS, $postString);
        }

        // execute cURL
        $response = curl_exec($soapCURL);
        $err = curl_error($soapCURL);

        // check response
        $endTime = intval(microtime(true) * 1000);
        if (empty($err)) {
            $info = curl_getinfo($soapCURL);
            if ($info['http_code'] !== 200) {
                $result['status'] = false;
                $result['response'] = $response;
                $this->log->error("{$logMsg} cURL Response({$info['http_code']})", __METHOD__, array($incident, $contact), $response, $endTime - $startTime);
            } else {
                $result['status'] = true;
                $result['response'] = $response;
                $this->log->debug("{$logMsg} cURL Response", __METHOD__, array($incident, $contact), $response, $endTime - $startTime);
            }
        } else {
            $result['status'] = false;
            $result['response'] = $err;
            $this->log->error("{$logMsg} cURL Error", __METHOD__, array($incident, $contact), $err, $endTime - $startTime);
        }

        curl_close($soapCURL);

        return $result;
    }

}
