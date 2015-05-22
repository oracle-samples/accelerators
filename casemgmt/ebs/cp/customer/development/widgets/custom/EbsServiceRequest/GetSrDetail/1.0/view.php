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
 *  date: Fri May 15 13:41:41 PDT 2015

 *  revision: rnw-15-2-fixes-release-01
 *  SHA1: $Id: 0bdee61fbe5f21f4859c81caf07340d113c908e7 $
 * *********************************************************************************************
 *  File: view.php
 * ****************************************************************************************** */
-->
<div  id="rn_<?= $this->instanceID; ?>" class="<?= $this->classList ?>">
    <rn:block id="top"/>

    <!-- loading spinner -->
    <div id="rn_<?= $this->instanceID; ?>_Loading"></div>

    <!-- content --->
    <div id="rn_<?= $this->instanceID; ?>_Content" class="rn_Content"></div>
    
    <rn:block id="bottom"/>
</div>
