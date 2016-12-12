<?php

/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + OFSC Reference Integration
 *  link: http://www-content.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.2 (Feb 2015)
 *  OFSC release: 15.2 (Feb 2015)
 *  reference: 150622-000130
 *  date: Thu Sep  3 23:14:07 PDT 2015

 *  revision: rnw-15-11-fixes-release-03
 *  SHA1: $Id: 64f3c29a6026a8d448b4e0ed24497620bfc8547f $
 * *********************************************************************************************
 *  File: toalogservice.php
 * ****************************************************************************************** */


require_once(OFSC_ROOT . 'toadefaultlog.php');

/**
* Helper class for Logging.
* Public methods are
* 	ToaPSLog::GetLog()
*
*/
class ToaLogService
{
	static $logWrapper = null;
    
	/**
	* Returns the logger object
	*/
	public static function GetLog($IsAuthorized = 1)
	{
		if($logWrapper == null)
			ToaLogService::$logWrapper = new ToaDefaultLog();                
		return ToaLogService::$logWrapper;
    }	
	
	/**
     * Private ctor so nobody else can instance it
     *
     */
    private function __construct()
    {

    } 
}

 ?>