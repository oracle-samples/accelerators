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
 *  date: Mon Aug 24 09:01:22 PDT 2015

 *  revision: rnw-15-11-fixes-release-01
 *  SHA1: $Id: d1864a9afdcbeb4e46159c89f68c4bda796d505d $
 * *********************************************************************************************
 *  File: itoalog.php
 * ****************************************************************************************** */

interface IToaLog
{
    public function debug($summary, $source = null, array $xRefArray = null, $message = null, $timeElapsed = null);

    public function error($summary, $source = null, array $xRefArray = null, $message = null, $timeElapsed = null);

    public function notice($summary, $source = null, array $xRefArray = null, $message = null, $timeElapsed = null);

    public function fatal($summary, $source = null, array $xRefArray = null, $message = null, $timeElapsed = null);

    public function click($summary, $source = null, array $xRefArray = null, $message = null, $timeElapsed = null);

    public function warning($summary, $source = null, array $xRefArray = null, $message = null, $timeElapsed = null);
}

