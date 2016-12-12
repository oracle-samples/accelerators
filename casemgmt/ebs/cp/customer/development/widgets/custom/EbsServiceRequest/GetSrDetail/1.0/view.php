<!--
/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + EBS Enhancement
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  EBS release: 12.1.3
 *  reference: 150505-000099, 150420-000127
 *  date: Thu Nov 12 00:52:36 PST 2015

 *  revision: rnw-15-11-fixes-release-1
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
