<?php

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
 *  date: Fri May 15 13:41:42 PDT 2015

 *  revision: rnw-15-2-fixes-release-01
 *  SHA1: $Id: c81b18bc5a4928353a48240882e916d409002b97 $
 * *********************************************************************************************
 *  File: controller.php
 * ****************************************************************************************** */

namespace Custom\Widgets\EbsServiceRequest;

class SrFieldDisplay extends \RightNow\Libraries\Widget\Base {

    function __construct($attrs) {
        parent::__construct($attrs);
    }

    function getData() {
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
