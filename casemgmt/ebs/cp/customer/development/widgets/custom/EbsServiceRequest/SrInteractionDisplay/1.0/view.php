<!--
/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC WSS + EBS Case Management Accelerator
 *  link: http://www-content.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.2 (February 2015)
 *  EBS release: 12.1.3
 *  reference: 140626-000078
 *  date: Fri May 15 13:41:43 PDT 2015

 *  revision: rnw-15-2-fixes-release-01
 *  SHA1: $Id: 7a90b7f885ac402a182b7288d1f98d0a763af75c $
 * *********************************************************************************************
 *  File: view.php
 * ****************************************************************************************** */
-->

<div id="rn_<?= $this->instanceID; ?>" class="rn_IncidentThreadDisplay rn_Output">
    <rn:block id="preLoadingIndicator"/>
    <div id="rn_<?= $this->instanceID; ?>_Loading" ></div>

    <rn:block id="postLoadingIndicator"/>
    <div id="rn_<?= $this->instanceID; ?>_Content" class="rn_Content"></div>
</div>
