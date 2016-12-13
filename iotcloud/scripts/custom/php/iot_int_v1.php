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
 *  SHA1: $Id: fdd4a8e475237c966f4b7b98300d0c396276a6ea $
 * *********************************************************************************************
 *  File: iot_int_v1.php
 * ****************************************************************************************** */


define('OFSC_ROOT', get_cfg_var('doc_root').'/custom/src/');
require_once(OFSC_ROOT . 'iotrestcontroller.php');

$iotRESTController = new IoTRESTController();
$iotRESTController->handleRESTRequest();
?>