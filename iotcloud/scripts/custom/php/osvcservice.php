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
 *  SHA1: $Id: 26d3ed7bfb1f24ade5c02b10fd74c2041313dddb $
 * *********************************************************************************************
 *  File: osvcservice.php
 * ****************************************************************************************** */

 
require(OFSC_ROOT . 'iotrequestmodel.php');
define("CUSTOM_SCRIPT", true);

 use RightNow\Connect\v1_2 as RNCPHP;

/*
 * Class for performing Oracle Service Cloud operations
 * The public methods are:
 *     checkForOpenIncidents()
 *     getServiceAsset()
 *     getOpenIncident()
 *     createIncident()
 *     convertToInternalModel()
 *     updateIncident()
 */
 class OSvCService {
	
	static $query = "select A.ID, A.Contact, A.Organization from Asset A where SerialNumber = '_REPLACE_SERIALNUMBER_' and Product.ID = _REPLACE_PRODUCTID_  and StatusWithType.StatusType = 19";
	
	/**
	* Checks for open incidents for the given product and serial number 
	* @return
	*/
	public static function checkForOpenIncidents($serviceProductId, $serialNumber, $iot_device_id)
	{
		
		$asset = self::getServiceAsset($serviceProductId, $serialNumber, $iot_device_id);
		
		if($asset != null)
		{
			//Step 2: If asset exists, search for any open incidents.
			$asset = self::getOpenIncident($asset);
		}
		
		return $asset;
		
	}
	
	/**
	* Fetches service asset based on the given product id(non primary key), and serial number provided.
	* @return iotassetmodel
	*/
	public static function getServiceAsset($serviceProductId, $serialNumber, $iot_device_id){
		
		//Validating the serial number.
		if((strlen($serialNumber) > 0 && strlen($serialNumber)<= 80 && (preg_match('/[^a-z0-9\*\#\-]+/i', $serialNumber) === 0))) {
			$serialNumber = RNCPHP\ROQL::escapeString($serialNumber);
			$serviceProductId = RNCPHP\ROQL::escapeString($serviceProductId);
			
			$salesProduct = RNCPHP\SalesProduct::find("PartNumber = '" .$serviceProductId. "'");
		
			if($salesProduct[0] == null)
			{
				//Sales/Service Product not found
				return null;
			}

			$str = str_replace("_REPLACE_SERIALNUMBER_", $serialNumber, self::$query);
			$final_query = str_replace("_REPLACE_PRODUCTID_", $salesProduct[0]->ID, $str);
			
			$result = RNCPHP\ROQL::query($final_query)->next();
			if($result != null && ($asset = $result->next())!= null)
			{				
				$asset_id = $asset['ID'];
				$contact_id = $asset['Contact'];
				$organization_id = $asset['Organization'];
				
				$iotAssetModel = new IOTAssetModel();
				$iotAssetModel->setAssetId($asset_id);
				$iotAssetModel->setContactId($contact_id);
				$iotAssetModel->setOrganizationId($organization_id);
				$iotAssetModel->setSerialNumber($serialNumber);
				
				if($iot_device_id != null){
					self::updateAssetIOTExtension($asset_id, $iot_device_id);
				}
				
				return $iotAssetModel;
			}
			else
			{
				//Asset not found
				return null;
			}
		}
		else{
			//Serial Number Validation failed
			return null;
		}
		
		
	}

	/**
	* Get Open incident for a given asset
	* @return asset
	*/	
	public static function getOpenIncident($asset)
	{
		$assetId = $asset->getAssetId();
		$incidents = RNCPHP\Incident::find("Asset.ID = " . $assetId . " and StatusWithType.StatusType.ID != 2");
		
		if($incidents != null && count($incidents) > 0)
		{
			$incident = $incidents[0];
			$asset->setIncidentId($incident->ID);
		}
		
		return $asset;
	}
	
	/**
	* Creates a service incident
	* @return incident
	*/	
	public static function createIncident($asset, $payload)
	{
		try
		{
			$incident = new RNCPHP\Incident();
			$incident->Subject = "Incident for asset " . $asset->getSerialNumber();

			$incident->Asset = RNCPHP\Asset::fetch($asset->getAssetId());

			$incident->StatusWithType               = new RNCPHP\StatusWithType() ;
			$incident->StatusWithType->Status       = new RNCPHP\NamedIDOptList() ;
			$incident->StatusWithType->Status->ID   = 1 ;

			//Adding JSON payload into the thread of the incident.
			$incident->Threads = new RNCPHP\ThreadArray();
			$incident->Threads[0] = new RNCPHP\Thread();
			$incident->Threads[0]->EntryType = new RNCPHP\NamedIDOptList();
			$incident->Threads[0]->EntryType->ID = 1;
			$payload = json_encode($payload);
			$payload = str_replace(array(",", "{", "}"), array(", \r\n", "{ \r\n", "} \r\n"), $payload);
			$incident->Threads[0]->Text = "Message from Device : \n" . $payload;
			
			if($asset->getContactId() != null)
			{
				$incident->PrimaryContact = RNCPHP\Contact::fetch($asset->getContactId());
			}
			else
			{
				return null;
			}
			
			if($asset->getOrganizationId() != null)
			{
				$incident->Organization = RNCPHP\Organization::fetch($asset->getOrganizationId());
			}
			
			//Check for any open incident
			$asset = self::getOpenIncident($asset);
			
			if($asset->getIncidentId() == null){
				$incident->save(RNCPHP\RNObject::SuppressAll);
				return $incident;
			}
		}
		catch(Exception $e)
		{
			ErrorService::setErrorCode(503);
			return null;
		}		
	}	
	
	private function mysql_escape($inp) { 
		if(is_array($inp)) 
			return array_map(__METHOD__, $inp); 
		
		if(!empty($inp) && is_string($inp)) { 
			return str_replace(array('\\', "\0", "\n", "\r", "'", '"', "\x1a"), array('\\\\', '\\0', '\\n', '\\r', "\\'", '\\"', '\\Z'), $inp); 
		} 
		return $inp;
	} 
	
	/**
	 * Input validation is required to prevent malicious data being processed and unexpected data being returned. Sanitizing the input in order to avoid SQL injection attack
	 * @return null or sanitized input
	 */
    private function sanitizeInput($input) {
        $result = trim($input);
	
	    if(strlen($result) > 0 && (preg_match('/^[1-9][0-9]*$/', $result) === 1)) {
	        return intval($result);
        }
	    return null;
    }
	
	
	public static function convertToInternalModel($payload_data)
	{
		$payload_data = json_decode($payload_data, true);
		$size = count($payload_data);
		$iotcustomObject = array();
		for($i=0; $i < $size; $i++)
		{
			$iotcustomObject[$i] = new IoTRequestModel($payload_data[$i]);
		}
		
		return $iotcustomObject;
	}

	private function updateAssetIOTExtension($asset_id, $iot_device_id){
		$extensionAsset = RNCPHP\CO\AssetIotExtension::first('asset_id = ' . $asset_id);
		if($extensionAsset != null){
			if($extensionAsset->iot_device_id == null){
				//if a row exists with null iot device id update the iot_device_id
				$extensionAsset->iot_device_id = $iot_device_id;
				$extensionAsset->save(RNCPHP\RNObject::SuppressAll);
			}
		}
		else{
			//if no row exists create a new row.
			$asset_iot_extension_obj = new RNCPHP\CO\AssetIotExtension();
			$service_asset = RNCPHP\Asset::fetch($asset_id);
			$asset_iot_extension_obj->asset_id = $service_asset;
			$asset_iot_extension_obj->iot_device_id = $iot_device_id;
			$asset_iot_extension_obj->save(RNCPHP\RNObject::SuppressAll);
		}
	}
	
	/**
	* Creates a service incident
	* @return incident
	*/	
	public static function updateIncident($incident_id, $payload)
	{
		try
		{			
			$incident = RNCPHP\Incident::fetch($incident_id);			
			//Adding JSON payload into the thread of the incident.
			$incident->Threads = new RNCPHP\ThreadArray();
			$incident->Threads[0] = new RNCPHP\Thread();
			$incident->Threads[0]->EntryType = new RNCPHP\NamedIDOptList();
			$incident->Threads[0]->EntryType->ID = 1;
			$payload = json_encode($payload);
			$payload = str_replace(array(",", "{", "}"), array(", \r\n", "{ \r\n", "} \r\n"), $payload);
			$incident->Threads[0]->Text = "Message Update from Device : \n" . $payload;

			$incident->save(RNCPHP\RNObject::SuppressAll);
			return $incident;

		}
		catch(Exception $e)
		{
			ErrorService::setErrorCode(503);
			return null;
		}		
	}
 }
 ?>