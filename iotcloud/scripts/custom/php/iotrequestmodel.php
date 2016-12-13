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
 *  SHA1: $Id: ae351bfc2149ac2eaa7ce96512d55a6495a2dda2 $
 * *********************************************************************************************
 *  File: iotrequestmodel.php
 * ****************************************************************************************** */

 

use RightNow\Connect\v1_2 as RNCPHP;

/*
 * Class to represent the request.
 * The public methods are:
 *     getSerialNumber()
 *     setSerialNumber()
 *     getDeviceId()
 *     setDeviceId()
 *     getServiceProductId()
 *     setServiceProductId()
 *     getPayload()
 *     setPayload()
 *     getOsvcIncidentReference()
 *     setOsvcIncidentReference()
 * 
 */
 class IoTRequestModel {
	
	private $id;//The primary key in Service Cloud - RightNow.
	private $serialNumber, $device_id, $create_time, $sent_time, $data_time, $status_id, $device_type, $error_message, $service_prodid, $payload_data, $msg_id;
	
	public function __construct($json_objpayload) // to remember use public
    {
		$serialNum = $json_objpayload["payload"]["data"]["source_serialnumber"];
		$service_prodid = $json_objpayload["payload"]["data"]["source_service_prodid"];		
		$dev_id = $json_objpayload["payload"]["data"]["msg_source"];
		$message_id = $json_objpayload["id"];
		$payload_data = $json_objpayload;
		
		$this->setSerialNumber($serialNum);
		$this->setServiceProductId($service_prodid);
		$this->setDeviceId($dev_id);
		$this->setPayload($payload_data);
		$this->setMessageId($message_id);
    }

	public function getSerialNumber()
	{
		return $this->serialNumber;
	}

	public function setSerialNumber($serialNum)
	{
		$this->serialNumber = $serialNum;
	}	
	public function getDeviceId()
	{
		return $this->device_id;
	}

	public function setDeviceId($dev_id)
	{
		$this->device_id = $dev_id;
	}
	
	public function getServiceProductId()
	{
		return $this->service_prodid;
	}
	
	public function setServiceProductId($service_prodid)
	{
		$this->service_prodid = $service_prodid;
	}
	
	public function getPayload()
	{
		return $this->payload_data;
	}
	
	public function setPayload($payload_data)
	{
		$this->payload_data = $payload_data;
	}
	
	public function getMessageId()
	{
		return $this->msg_id;
	}

	public function setMessageId($message_id)
	{
		$this->msg_id = $message_id;
	}	
 }

 ?>