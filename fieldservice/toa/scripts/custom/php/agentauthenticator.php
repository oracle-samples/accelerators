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
 *  date: Thu Sep  3 23:14:06 PDT 2015

 *  revision: rnw-15-11-fixes-release-03
 *  SHA1: $Id: 4503c37559b9890f32ce37063d1e1ea8b96f343e $
 * *********************************************************************************************
 *  File: agentauthenticator.php
 * ****************************************************************************************** */
 
define("CUSTOM_SCRIPT", true);

// Find our position in the file tree
if (!defined('DOCROOT')) {
    define('DOCROOT', get_cfg_var('doc_root'));
}


require_once(OFSC_ROOT . 'toalogservice.php');

use RightNow\Connect\v1_2 as RNCPHP;

/*
 * Class to authenticate use of a custom php page on the RightNow system.
 * The public methods are:
 *     authenticate_credentials()
 * 
 */
 class AgentAuthenticator {
	
	public $IsAuthorised = 1;
	
	/*
     * Authenticate the login/password pair in our data.
     * Success: returns acct_id.
     * Failure: return failure response.
     */
	public function authenticate_credentials($work_order_objects) {
		require_once (DOCROOT . '/include/ConnectPHP/Connect_init.phph');
		$result = -1;
		try{
			$first_work_order = $work_order_objects[0];
			$AgentLogin = $first_work_order->getUsername();
			$AgentPassword = $first_work_order->getPassword();
			
			if(!isset($AgentLogin) || trim($AgentLogin)===''){
				foreach ($work_order_objects as $wo){
					$wo->setResponseStatus('failed');
					$wo->setResponseDescription('Authentication Failed');
				}
			$this->logAuthFailure("Login failed due to empty/null username.");
			return ($result);
			}

			initConnectAPI($AgentLogin, $AgentPassword, null, "RightNow\Connect\v1_2\ConnectAPI::AuthOptTransient");
			
			$queryString = "SELECT Account FROM Account WHERE Login = '" . $AgentLogin ."' limit 1";
			$res = RightNow\Connect\v1_2\ROQL::queryObject($queryString)->next();
			while($acc = $res->next()){
				$result = $acc->ID;
			}
			
			$ip = $_SERVER['REMOTE_ADDR'];
			$isIPValid = $this->validateIPAddress($ip, $this->isIPv6($ip));
			if(!$isIPValid){
				foreach ($work_order_objects as $wo){
					$wo->setResponseStatus('failed');
					$wo->setResponseDescription('Authentication Failed');
				}
				$this->logAuthFailure("Invalid IP detected : " . $ip);
				return (-1);
			}
					
			//Setting up log			
			$log = ToaLogService::GetLog();
			
			if($log != null){
				$log->debug("Login successful !");
			}
			
			return ($result);
		}
		catch(RightNow\Connect\v1_2\ConnectAPIError $err){
			foreach ($work_order_objects as $wo){
				$wo->setResponseStatus('failed');
				$wo->setResponseDescription('Authentication Failed');
			}
			
			$this->logAuthFailure("Login failed ! ");
			return ($result); 
		}
	}
	
	/**
	* Validates IP address
	*/
	private function validateIPAddress($ip, $ifIPv6){
		$config = RightNow\Connect\v1_2\Configuration::fetch(CUSTOM_CFG_FS_Accel_Integrations);
		$configVerb = json_decode($config->Value, true);
		
		if($ifIPv6){
			$configValue = $configVerb["integration"]["valid_ipv6_hosts"];
			if($configValue == null){
				return true;
			}
			
			$values = explode(',',$configValue);
			
			foreach($values as $cidrnet){
				$cidrnet = trim($cidrnet);
				$res = $this->validateIPv6Address($ip, $cidrnet);
				if($res){
					return true;
				}
			}
			return (false);
		}
		else {
			$configValue = $configVerb["integration"]["valid_ipv4_hosts"];
			if($configValue == null){
				return true;
			}
			
			$values = explode(',',$configValue);
			
			foreach($values as $cidrnet){
				$cidrnet = trim($cidrnet);
				list($config_addr,$maskbits)=explode('/',$cidrnet);
				$res = $this->validateIPv4Address($ip, $config_addr, $maskbits);
				if($res){
					return true;
				}
			}
			return false;
		}
	}
	
	/**
	* Determine if the IP received is an IPv6 or IPv4
	*/
	private function isIPv6($ip) {
		if(strpos($ip, ":") !== false && strpos($ip, ".") === false) {
			return true; //Pure format
		}
		elseif(strpos($ip, ":") !== false && strpos($ip, ".") !== false){
			return true; //dual format
		}
		else{
			return false;
		}
	}
	
	/**
	*converts inet_pton output to string with bits
	*/
	private function inet_to_bits($inet, $IPv6) 
	{
		$unpacked = unpack('A16', $inet);
		$unpacked = str_split($unpacked[1]);
		$binaryip = '';
		
		foreach ($unpacked as $char) {
			$binaryip .= str_pad(decbin(ord($char)), 8, '0', STR_PAD_LEFT);
		}
		return $binaryip;
	}
	
	/**
	* Validates IPv6 address
	*/
	private function validateIPv6Address($ip, $cidrnet){
		$ip = inet_pton($ip);
		
		$binaryip=$this->inet_to_bits($ip);
		
		list($net,$maskbits)=explode('/',$cidrnet);
		$net=inet_pton($net);
		$binarynet=$this->inet_to_bits($net);
		$ip_net_bits=substr($binaryip,0,$maskbits);
		$net_bits   =substr($binarynet,0,$maskbits);
		
		if($ip_net_bits!==$net_bits) 
			return false;
		else 
			return true;
	}
	
	/**
	* Validates IPv4 address
	*/
	private function validateIPv4Address($request_ip, $config_addr, $net_mask){
		if($net_mask < 0){
			return false; 
		} 
        $ip_binary_string = sprintf("%032b",ip2long($request_ip)); 
        $net_binary_string = sprintf("%032b",ip2long($config_addr)); 
        if(substr_compare($ip_binary_string,$net_binary_string,0,$net_mask) === 0){
			return true;
		}
		else {
			return false;
		}
	}
	
	
	/**
	* Logging incase of auth failures.
	* To be used in case of auth failures only.
	*/
	private function logAuthFailure($message){			
		// If authentication is failed, then pass '0' while fetching logging object
		$this->IsAuthorised = 0;
		$log = ToaLogService::GetLog($this->IsAuthorised);
		if($log != null){
		       $log->error($message);
		}
	}	
 }

 ?>