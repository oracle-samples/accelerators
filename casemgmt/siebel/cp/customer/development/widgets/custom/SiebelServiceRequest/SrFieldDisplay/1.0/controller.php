<?php

/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC Contact Center + Siebel Case Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  Siebel release: 8.1.1.15
 *  reference: 150520-000047
 *  date: Mon Nov 30 19:59:30 PST 2015

 *  revision: rnw-15-11-fixes-release-2
 *  SHA1: $Id: 194208ca6ff04c55472fcff7906a25d58bfd1217 $
 * *********************************************************************************************
 *  File: controller.php
 * ****************************************************************************************** */

namespace Custom\Widgets\SiebelServiceRequest;

class SrFieldDisplay extends \RightNow\Libraries\Widget\Base {

    function __construct($attrs) {
        parent::__construct($attrs);
    }

    function getData() {
        // get name attribute
        $fieldName = $this->data['attrs']['name'];
        $fieldKey = explode('.', $fieldName);
        if ($fieldKey[0] !== 'SR') {
            echo $this->reportError('Only support Service Request field display');
            return false;
        }
        
        // render to js
        $this->data['js']['name'] = $fieldKey[1];
        $this->data['js']['label'] = $this->data['attrs']['label'];
    }

}
