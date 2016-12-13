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
 *  SHA1: $Id: f1f873041975c19f9625ed69510fdafa46ec3de3 $
 * *********************************************************************************************
 *  File: requestprocessor.php
 * ****************************************************************************************** */

 define("CUSTOM_SCRIPT", true);

// Find our position in the file tree
if (!defined('DOCROOT')) {
    define('DOCROOT', get_cfg_var('doc_root'));
}

require(OFSC_ROOT . 'osvcservice.php');
require(OFSC_ROOT . 'iotassetmodel.php');
require(DOCROOT . '/include/ConnectPHP/Connect_init.phph');

use RightNow\Connect\v1_2 as RNCPHP;

/*
 * Class to process the request received.
 * The public methods are:
 *     processRESTRequest()
 * 
 */
 class RequestProcessor {
	
	private $iotrequestObject = null;

	/*
     * Process the REST request.
     * Success: Creates an incident if no open incident exists for the asset.
     * Failure: sets the status code as 409.
     */
	public function processRESTRequest($post_payload)
	{		
		$this->iotrequestObject = OSvCService::convertToInternalModel($post_payload);

		$size = count($this->iotrequestObject);
		$resp = array();

		for($i=0; $i < $size; $i++)
		{			
			$resp[$i]->msg_id = $this->iotrequestObject[$i]->getMessageId();
			//Checking for empty or null device id in request.
			if(strlen($this->iotrequestObject[$i]->getDeviceId()) == 0){
				$resp[$i]->status = "Device Id Empty Or Null";
				continue;
			}
			
			$resp[$i]->status = $this->createOrUpdateIncident($this->iotrequestObject[$i]);
		}
		echo json_encode($resp);
	}
	
	/*
     * Checks for open incident, creates one if no open incident exists.
     * Success: Creates an incident if no open incident exists for the asset.
     */
	private function createOrUpdateIncident($iotmessage)
	{
		$serialNumber = $iotmessage->getSerialNumber();
		$serviceProductId = $iotmessage->getServiceProductId();
		$device_id = $iotmessage->getDeviceId();
		$payload = $iotmessage->getPayload();
		
		//Step 1: Search for asset first. If no asset, there won't be any incident for asset with given serial number and service product.
		$iotassetmodel = OSvCService::checkForOpenIncidents($serviceProductId, $serialNumber, $device_id);
		if($iotassetmodel != null)
		{
			if($iotassetmodel->getIncidentId() == null)
			{
				//Step 3: Create Incident.
				$newincident = OSvCService::createIncident($iotassetmodel, $payload);
				if($newincident != null)
				{
					$iotassetmodel->setIncidentId($newincident->ID);
					return "Incident Created";
				}else
				{
					//TODO: Do we need set error code?
					return "Incident Creation Failed";
				}
			}
			else
			{
				//Update Incident by adding message details in thread.
				OSvCService::updateIncident($iotassetmodel->getIncidentId(),$payload);
				return "Incident Updated";
			}
		}
		else
		{
			//TODO: Remove this else clause before final delivery
			//If iotassetmodel is null, that mean no asset exists.
			return "Asset doesn't exist hence Incident cannot be created or updated";
		}
	}	
	
 }

 ?>