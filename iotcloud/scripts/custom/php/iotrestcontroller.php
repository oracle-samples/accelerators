<?php

/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: IoT OSvC Bi-directional Integration Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 16.11 (November 2016) 
 *  reference: 151217-000026
 *  date: Tue Dec 13 13:23:41 PST 2016
 
 *  revision: rnw-16-11-fixes-release
 *  SHA1: $Id: f11ee8145beb90a50c686337dc92ffea1cad2ccf $
 * *********************************************************************************************
 *  File: iotrestcontroller.php
 * ****************************************************************************************** */


require(OFSC_ROOT . 'requestprocessor.php');
require(OFSC_ROOT . 'errorservice.php');

/*
 * Class to control the request.
 * The public methods are:
 *     handleRESTRequest()
 */

class IoTRESTController  {
	var $this_host;
    var $orig_interface;
    var $isSSL = FALSE;
	var $post_payload;
	var $iotReqProc = null;
	const CUSTOM_ROOT_URL = null;
	const ENDPOINT_NAME = "iot_int_v1.php";
	const USERNAME_PATTERN = "/((<|&lt;)\s*username\s*(>|&gt;))(.*)((<|&lt;)\s*\/\s*username\s*(>|&gt;))/i";
	const PASSWORD_PATTERN = "/((<|&lt;)\s*password\s*(>|&gt;))(.*)((<|&lt;)\s*\/\s*password\s*(>|&gt;))/i";
	const REPLACE_TOKEN = "$1*$5";
	var $get_json_response = array("links" => array(array("rel" => "self","href" => "__REPLACE_REST_URL__"),array("rel" => "canonical","href" => "__REPLACE_REST_URL__")));
	var $error_response = array( "type" => "__REPLACE_REST_UNSUP_METHOD__","title" => "", "status" => 400, "detail" => "Unsupported method for request: POST", "instance" => "__REPLACE_REST_URL__","o:errorCode" => "OSC-CREST-00002");
	var $status_code = array(400 => array("title" => "An unsupported HTTP method was used", "detail" => "Unsupported method for request: __REQUEST_NAME__","int_error_code" => "OSC-CREST-00002", "type" => "services/rest/connect/exceptions/OSC-CREST-00002"), 
	500 => array("title" => "An unsupported HTTP method was used", "detail" => "Unsupported method for request: __REQUEST_NAME__","int_error_code" => "OSC-CREST-00002", "type" => "services/rest/connect/exceptions/OSC-CREST-00002"));
	
	public function __construct()
    {
		$this->isSSL = $_SERVER['RNT_SSL'];
        $this->this_host = $_SERVER['SERVER_NAME'];
		$this->definecontrollerpath();
    }
		
	public function handleRESTRequest()
    {      
        if($_ENV["HTTP_RNT_SSL"] == "yes" && $_SERVER['REQUEST_METHOD'] == 'POST') {
			try
			{
			    header('Content-Type: application/json; charset=UTF-8');
				$this->authenticate();
			    $this->iotReqProc = new RequestProcessor();
				$this->iotReqProc->processRESTRequest($this->get_http_post_data());			
			}catch(Exception $e)
			{
				ErrorService::setErrorCode(500);
				echo $e . chr(13);			
			}
		}
		else{
			//TODO: Set Error Code
		}
    }
	
	private function get_http_request_headers()	{
		$headers = array();
		foreach($_SERVER as $key => $value) {
			if (substr($key, 0, 5) <> 'HTTP_') {
				continue;
			}
			$header = str_replace(' ', '-', ucwords(str_replace('_', ' ', strtolower(substr($key, 5)))));
			$headers[$header] = $value;
		}
		return $headers;
	}
	
	private function get_http_post_data(){
		$req_post_payload = file_get_contents('php://input');
		$this->post_payload = $req_post_payload;
		return $this->post_payload;
	}
	
	private function show_http_post_data(){
		$this->show_http_request_headers();
		$payload = file_get_contents('php://input');
		$this->showAlert("POST payload" . $payload);
	}	
	
	private function show_http_request_headers(){
		$headers = $this->get_http_request_headers();
		echo '<script language="javascript">';
		foreach($headers as $key => $value){
			//$this->showAlert($key . '=' . $value);
			echo 'alert("' . $key . '=' . $value . '");';
		}
		echo '</script>';
	}
	
	private function showAlert($str){
		echo '<script language="javascript">';
		echo 'alert("' . $str . '");';
		echo '</script>';
	}
	
	/**
	* Authenticates the credentials and returns the agent id
	* @return Agent Id
	*/
	private function authenticate(){
		$credentials_from_request = $_SERVER['HTTP_AUTHORIZATION'];//"Basic cGFua2lsOldlbGNvbWUx";
		$credentials = explode(" ", $credentials_from_request);
		//Supporting Basic Authentication only.
		if($credentials != null && strcmp( $credentials[0], 'Basic' ) == 0)
		{
			$base64_encoded_credentials = $credentials[1];
		}
		else
		{
			ErrorService::setErrorCode(401);
			exit;
		}
			
		require_once(OFSC_ROOT . 'agentauthenticator.php');
		$agent_authenticator = new AgentAuthenticator();
		$agent_id = $agent_authenticator -> authenticate_credentials($base64_encoded_credentials);
		
		if($agent_id < 0){
			ErrorService::setErrorCode(401);
			exit();
		}
	}
	
	private function processRequest()
	{
		$payload = $this->get_http_post_data();
		$payload = htmlspecialchars_decode($payload);	
		
	}
	
	private function endswith($string, $test) {
		$strlen = strlen($string);
		$testlen = strlen($test);
		if ($testlen > $strlen) return false;
		return substr_compare($string, $test, $strlen - $testlen, $testlen) === 0;
	}
	
	private function definecontrollerpath() {
		//Try to build the endpoint to be displayed in WSDL		
		if(null != IoTRESTController::CUSTOM_ROOT_URL)
		{
			define('CONTROLLER_PATH', IoTRESTController::CUSTOM_ROOT_URL . IoTRESTController::ENDPOINT_NAME );
		}else
		{			
			$domain = NULL;
			if($this->endswith($this->this_host,".dv.lan"))
			{
				$domain = ".dv.lan";
			}elseif($this->endswith($this->this_host,".qb.lan"))
			{
				$domain = ".qb.lan";
			}elseif($this->endswith($this->this_host,".pr.rightnow.com"))
			{
				$domain = ".pr.rightnow.com";
			}else
			{
				// TODO
				//log warning in initialization php endpoint
				$domain = NULL;
			}
			$protocol = "http";
			if($this->isSSL)
			{
				$protocol = "https";
			}
			if(!empty($domain))
			{
				$this->orig_interface = str_replace('-', '_', str_replace($domain, '.cfg', $this->this_host));
			}else
			{
				// TODO
				//log warning in initialization of php endpoint
				$this->orig_interface = "[replace-with-your-interface-name].cfg";
			}
			define('CONTROLLER_PATH', $protocol . '://'.$this->this_host.'/cgi-bin/'.$this->orig_interface.'/php/custom/' . IoTRESTController::ENDPOINT_NAME );
			define('REST_ERROR_UNSUPPORTED_METHOD', $protocol . '://'.$this->this_host.'/services/rest/connect/exceptions/OSC-CREST-00002');
		}
	}	
}
?>
