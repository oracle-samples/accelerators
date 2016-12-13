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
 *  SHA1: $Id: cfc2197d00b03fc8caf558818faf9b358e8fcf3a $
 * *********************************************************************************************
 *  File: agentauthenticator.php
 * ****************************************************************************************** */

 
define("CUSTOM_SCRIPT", true);

// Find our position in the file tree
if (!defined('DOCROOT')) {
    define('DOCROOT', get_cfg_var('doc_root'));
}

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
	public function authenticate_credentials($base64_encoded_credential) {
		require_once (DOCROOT . '/include/ConnectPHP/Connect_init.phph');
		$result = -1;
		
		try{
			if($base64_encoded_credential != null){
				$credentials = base64_decode($base64_encoded_credential);
				list($AgentLogin, $AgentPassword) = split(":", $credentials, 2);
			
				initConnectAPI($AgentLogin, $AgentPassword, null, "RightNow\Connect\v1_2\ConnectAPI::AuthOptTransient");
			
				$queryString = "SELECT Account FROM Account WHERE Login = '" . $AgentLogin ."' limit 1";
				$res = RightNow\Connect\v1_2\ROQL::queryObject($queryString)->next();
				while($acc = $res->next()){
					$result = $acc->ID;
				}
			
				$ip = $_SERVER['REMOTE_ADDR'];
				$isIPValid = $this->validateIPAddress($ip, $this->isIPv6($ip));
				if(!$isIPValid){
					ErrorService::setErrorCode(401);
					return (-1);
				}
			}
			else{
				return ($result); 
			}
			
			return ($result);
		}
		catch(RightNow\Connect\v1_2\ConnectAPIError $err){
			ErrorService::setErrorCode(401);
			return (-1); 
		}
	}
	
	/**
	* Validates IP address
	*/
	private function validateIPAddress($ip, $ifIPv6){
		$config = RightNow\Connect\v1_2\Configuration::fetch(CUSTOM_CFG_IOT_Accel_Integrations);
		$configVerb = json_decode($config->Value, true);
		
		if($ifIPv6){
			$configValue = $configVerb["integration"]["ics_service"]["valid_ipv6_hosts"];
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
			$configValue = $configVerb["integration"]["ics_service"]["valid_ipv4_hosts"];
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
 }

 ?>