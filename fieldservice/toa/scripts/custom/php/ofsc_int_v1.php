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
 *  SHA1: $Id: 4b7adc2a0c4f349d6ad2201b864d9cb615b6bf9a $
 * *********************************************************************************************
 *  File: ofsc_int_v1.php
 * ****************************************************************************************** */
 
define('OFSC_ROOT', get_cfg_var('doc_root').'/custom/src/');
require_once(OFSC_ROOT . 'toasoapcontroller.php');

$toaSoapController = new ToaSOAPController();
$toaSoapController->handleSOAPRequest();
?>