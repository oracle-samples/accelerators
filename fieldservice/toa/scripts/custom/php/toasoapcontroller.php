<?php

/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + OFSC Reference Integration
 *  link: http://www-content.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.2 (Feb 2015)
 *  OFSC release: 15.2 (Feb 2015)
 *  reference: 150622-000130
 *  date: Mon Aug 24 09:01:23 PDT 2015

 *  revision: rnw-15-11-fixes-release-01
 *  SHA1: $Id: 49c5cc41921a8705ab0eb285a2ba94a0627a395f $
 * *********************************************************************************************
 *  File: toasoapcontroller.php
 * ****************************************************************************************** */

class ToaSOAPController  {
	var $this_host;
    var $orig_interface;
    var $isSSL = FALSE;
	var $post_payload = null;
	// URL upto the custom directory(including '/' in the end).
	// e.g. http://toa-int-endpoint.kl.pqr/cgi-bin/toa_int_endpoint.cfg/php/custom/
	const CUSTOM_ROOT_URL = null;
	const ENDPOINT_NAME = "ofsc_int_v1.php";
	const USERNAME_PATTERN = "/((<|&lt;)\s*username\s*(>|&gt;))(.*)((<|&lt;)\s*\/\s*username\s*(>|&gt;))/i";
	const PASSWORD_PATTERN = "/((<|&lt;)\s*password\s*(>|&gt;))(.*)((<|&lt;)\s*\/\s*password\s*(>|&gt;))/i";
	const REPLACE_TOKEN = "$1*$5";
	
	public function __construct() // to remember use public
    {
		$this->isSSL = $_SERVER['RNT_SSL'];
        $this->this_host = $_SERVER['SERVER_NAME'];
		//Define the location for WSDL.
		define('WSDL_LOC', OFSC_ROOT . 'toa_outbound_client_wsdl.php');
		$this->definecontrollerpath();
    }
		
	public function handleSOAPRequest()
    {      
        if($_SERVER['REQUEST_METHOD'] == 'GET') {
            if ($_ENV["HTTP_RNT_SSL"] == "yes" && is_string( $_GET['wsdl'] ) ) {
                header('Content-Type: text/xml;charset=UTF-8');
                $wsdl = file_get_contents(WSDL_LOC);
				$wsdl = $this->replace_endpoint_urls($wsdl);
				echo $wsdl;
            }else{
				//$this->show_http_request_headers();
				//echo "SSL:" . $this->isSSL;
				//echo "SSL Env:" . $_ENV["HTTP_RNT_SSL"];
				//$this->showAlert("HOST:" . $this->this_host);
				//$this->showAlert("CONTROLLER_PATH:" . CONTROLLER_PATH);				
			}
        }elseif($_ENV["HTTP_RNT_SSL"] == "yes" && $_SERVER['REQUEST_METHOD'] == 'POST') {
			try{
			header('Content-Type: text/xml;charset=UTF-8');	
			//$work_order_models = $this->get_work_order_models();
			
			$work_order_models = $this->get_work_order_models();
			//authentication
			$this->authenticate($work_order_models);
			$this->debug_log("Processing individual work orders", "");
 			$this->update_work_orders($work_order_models);
			$response = $this->create_soap_response($work_order_models);
			echo($response);
			$this->debug_log("SOAP Response ", $response);
			}catch(Exception $e){				
				$error_response = $this->server_error('SOAP-ENV:Server','Internal Server Error','There was a problem with the server please contact administrator');
				echo $error_response;
				$this->error_log("SOAP Error Response ", $error_response);
				$this->error_log("Exception Message ", $e->getMessage());
				$this->error_log("Exception Stack Trace ", $e->getTraceAsString());
			}
		}
		else{
				$error_response = $this->server_error('SOAP-ENV:Client','Client Request Error','The request must be sent over SSL only');
				echo $error_response;
		}
    }
	
	/**
     * Just replaces strings in XML
     * @param string $str
     * @return string
     */
    private function replace_endpoint_urls($str) {
        //$s_controller_path = str_replace('http', 'https', CONTROLLER_PATH);
        // for the hardcoded file 
        $str = str_replace("__REPLACE_SOAP_URL__", CONTROLLER_PATH, $str);
        //$str = str_replace("__REPLACE_SECURE_SOAP_URL__", $s_controller_path, $str);
        
        // for any possible non-hardcoded  remote URLs
        //$s_remote_cp = str_replace('http', 'https', REMOTE_CONTROLLER_PATH);
        //$str = str_replace(REMOTE_CONTROLLER_PATH, CONTROLLER_PATH, $str);        
        //$str = str_replace($s_remote_cp, $s_controller_path, $str);
        
        return $str;
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
		if(!empty($this->post_payload)){
			return $this->post_payload;
		}
		else{
			$this->post_payload = file_get_contents('php://input');
		}
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
	private function authenticate($work_order_models){
		require_once(OFSC_ROOT . 'agentauthenticator.php');
		$agent_authenticator = new AgentAuthenticator();
		$agent_id = $agent_authenticator -> authenticate_credentials($work_order_models);
		
		//logging the soap payload
		$payload = $this->get_http_post_data();		
		$changed_payload = preg_replace(ToaSOAPController::USERNAME_PATTERN,ToaSOAPController::REPLACE_TOKEN,$payload);
		$changed_payload = preg_replace(ToaSOAPController::PASSWORD_PATTERN,ToaSOAPController::REPLACE_TOKEN,$changed_payload);
		$this->debug_log("SOAP Payload ", $changed_payload, $agent_authenticator->IsAuthorised);
		
		if($agent_id < 0){
			$response = $this->create_soap_response($work_order_models);
			$this->debug_log("SOAP Response ", $response, $agent_authenticator->IsAuthorised);
			echo($response);
			exit();
		}
	}
	
	/**
	 * Get WorkOrder models from soap payload
	 * @return Array of WorkOrder models
	 */
	private function get_work_order_models(){
	    require_once(OFSC_ROOT . 'xtree.php');
		require_once(OFSC_ROOT . 'workorder.php');
		
		$workOrders = array();
		$payload = $this->get_http_post_data();
		$payload = htmlspecialchars_decode($payload);
		$payloadTree = new xtree(array('xmlRaw' => $payload, 'stripNamespaces' => true));
        $messages = $payloadTree->xtree->Envelope->Body->send_message->messages->message;
		
		if(count($messages) == 1) {
		    $workOrder = new WorkOrder();
			$workOrder->setMsgId($messages->message_id->_['value']);
			$workOrder->setUsername($messages->body->username->_['value']);
			$workOrder->setPassword($messages->body->password->_['value']);
			$workOrder->setWorkOrderId($messages->body->appt_number->_['value']);             
			$workOrder->setContact($messages->body->customer_number->_['value']);
			$workOrder->setContactPhone($messages->body->cphone->_['value']);
			$workOrder->setContactEmail($messages->body->cemail->_['value']);
			$workOrder->setContactMobilePhone($messages->body->ccell->_['value']);
			$workOrder->setWorkOrderTimeSlot($messages->body->time_slot->_['value']);
			$workOrder->setWorkOrderDate($messages->body->date->_['value']);
			$workOrder->setWorkOrderStatus($messages->body->astatus->_['value']);
			$workOrder->setEndTime($messages->body->end_time->_['value']);
			$workOrder->setStartEndTime($messages->body->eta_end_time->_['value']);
			$workOrder->setEta($messages->body->ETA->_['value']);
			$workOrder->setExternalId($messages->body->aid->_['value']);
			$workOrder->setDeliveryWindowStart($messages->body->delivery_window_start->_['value']);
			$workOrder->setDeliveryWindowEnd($messages->body->delivery_window_end->_['value']);
			$workOrder->setResource($messages->body->pname->_['value']);
			$workOrder->setTravelTime($messages->body->travel->_['value']);
			$workOrder->setFieldServiceNote($messages->body->XA_ACTIVITY_NOTES->_['value']);
			$workOrder->setDuration($messages->body->duration->_['value']);
			array_push($workOrders, $workOrder);
		}
        else {
		    foreach ($messages as $val) {			
			    $workOrder = new WorkOrder();
			    $workOrder->setUsername($val->body->username->_['value']);
                $workOrder->setPassword($val->body->password->_['value']);
                $workOrder->setMsgId($val->message_id->_['value']);
                $workOrder->setWorkOrderId($val->body->appt_number->_['value']);             
                $workOrder->setContact($val->body->customer_number->_['value']);
                $workOrder->setContactPhone($val->body->cphone->_['value']);
                $workOrder->setContactEmail($val->body->cemail->_['value']);
                $workOrder->setContactMobilePhone($val->body->ccell->_['value']);
                $workOrder->setWorkOrderTimeSlot($val->body->time_slot->_['value']);
                $workOrder->setWorkOrderDate($val->body->date->_['value']);
                $workOrder->setWorkOrderStatus($val->body->astatus->_['value']);
                $workOrder->setEndTime($val->body->end_time->_['value']);
                $workOrder->setStartEndTime($val->body->eta_end_time->_['value']);
                $workOrder->setEta($val->body->ETA->_['value']);
                $workOrder->setExternalId($val->body->aid->_['value']);
                $workOrder->setDeliveryWindowStart($val->body->delivery_window_start->_['value']);
                $workOrder->setDeliveryWindowEnd($val->body->delivery_window_end->_['value']);
                $workOrder->setResource($val->body->pname->_['value']);
                $workOrder->setTravelTime($val->body->travel->_['value']);
                $workOrder->setFieldServiceNote($val->body->XA_ACTIVITY_NOTES->_['value']);
                $workOrder->setDuration($val->body->duration->_['value']);
				array_push($workOrders, $workOrder);
            }
		}
		
		return $workOrders;		
	}
	
	private function create_soap_response($work_order_objects){
		$response = '<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:urn="urn:toatech:agent">
		<soapenv:Header/><soapenv:Body><urn:send_message_response>';
		
		foreach ($work_order_objects as $wo){
			$msgid = $wo->getMsgId();
			$status = $wo->getResponseStatus();
			$description = $wo->getResponseDescription();
			$response = $response . '<urn:message_response><urn:message_id>'.$msgid.'</urn:message_id><urn:status>'.$status.'</urn:status><urn:description>'.$description.'</urn:description></urn:message_response>';
		}
		
		$response = $response . '</urn:send_message_response></soapenv:Body></soapenv:Envelope>';
		return ($response);
	}
	
	/**
	 * Update WorkOrder
	 * @param Array of WorkOrder models
	 */
	private function update_work_orders($work_order_models){
	    foreach($work_order_models as $workOrder) {
		    $workOrder->updateWorkOrder();
		}
	}
	
	private function endswith($string, $test) {
		$strlen = strlen($string);
		$testlen = strlen($test);
		if ($testlen > $strlen) return false;
		return substr_compare($string, $test, $strlen - $testlen, $testlen) === 0;
	}
	
	private function definecontrollerpath() {
		//Try to build the endpoint to be displayed in WSDL		
		if(null != ToaSOAPController::CUSTOM_ROOT_URL)
		{
			define('CONTROLLER_PATH', ToaSOAPController::CUSTOM_ROOT_URL . ToaSOAPController::ENDPOINT_NAME );
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
			define('CONTROLLER_PATH', $protocol . '://'.$this->this_host.'/cgi-bin/'.$this->orig_interface.'/php/custom/' . ToaSOAPController::ENDPOINT_NAME );
		}
	}	
	
	private function server_error($faultcode, $faultstring, $detail)
	{
		$response = '<SOAP-ENV:Envelope xmlns:SOAP-ENV="http://schemas.xmlsoap.org/soap/envelope/">
					<SOAP-ENV:Header/>
						<SOAP-ENV:Body>
						<SOAP-ENV:Fault>
							<faultcode>' . $faultcode . '</faultcode>
							<faultstring>' . $faultstring . '</faultstring>
							<detail>' . $detail . '</detail>
						</SOAP-ENV:Fault>
						</SOAP-ENV:Body>
					</SOAP-ENV:Envelope>';
		return $response;
	}
	
	/**
	* Logs the material in debug mode.
	*/
	private function debug_log($title, $log_matter, $IsAuthorised){
		require_once(OFSC_ROOT . 'toalogservice.php');
		$log = ToaLogService::GetLog($IsAuthorised);
		if($log != null){
			$log->debug($title, __METHOD__, array(null, null), $log_matter);
		}
	}
	
	/**
	* Logs the data in error mode.
	*/
	private function error_log($title, $log_matter, $IsAuthorised){
		require_once(OFSC_ROOT . 'toalogservice.php');
		$log = ToaLogService::GetLog($IsAuthorised);
		if($log != null){
			$log->error($title, __METHOD__, array(null, null), $log_matter);
		}
	}	
}
?>