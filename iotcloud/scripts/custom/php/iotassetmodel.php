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
 *  SHA1: $Id: fde65213c579a93b06d7c9135ce791ac93490682 $
 * *********************************************************************************************
 *  File: iotassetmodel.php
 * ****************************************************************************************** */

 

/*
 * Model Class for IOT Asset
 * The public methods are:
 * 
 */
 class IOTAssetModel {
	private $asset_id;
	private $contact_id;
	private $organization_id;
	
	private $incident_id;
	private $serial_number;
	
	
	public function getAssetId()
	{
		return $this->asset_id;
	}

	public function setAssetId($asset_id)
	{
		$this->asset_id = $asset_id;
	}
	
	public function getContactId()
	{
		return $this->contact_id;
	}

	public function setContactId($contact_id)
	{
		$this->contact_id = $contact_id;
	}
	
	public function getOrganizationId()
	{
		return $this->organization_id;
	}

	public function setOrganizationId($organization_id)
	{
		$this->organization_id = $organization_id;
	}
	
	public function getSerialNumber()
	{
		return $this->serial_number;
	}

	public function setSerialNumber($serial_number)
	{
		$this->serial_number = $serial_number;
	}
	
	public function getIncidentId()
	{
		return $this->incident_id;
	}

	public function setIncidentId($incident_id)
	{
		$this->incident_id = $incident_id;
	}
 }
 ?>