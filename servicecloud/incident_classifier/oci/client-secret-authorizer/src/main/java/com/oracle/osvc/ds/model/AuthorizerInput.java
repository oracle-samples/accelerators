/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
 *  http://oss.oracle.com/licenses/upl
 *  Copyright (c) 2023, Oracle and/or its affiliates.
 ***********************************************************************************************
 *  Accelerator Package: Incident Text Based Classification
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 23A (February 2023) 
 *  date: Tue Jan 31 13:02:55 IST 2023
 
 *  revision: rnw-23-02-initial
 *  SHA1: $Id: 4e76ec4e17164a8ac82b54c4bbd12224bb96f897 $
 * *********************************************************************************************
 *  File: AuthorizerInput.java
 * ****************************************************************************************** */

package com.oracle.osvc.ds.model;

import java.util.Map;

public class AuthorizerInput {

    private String type = "USER_DEFINED";
    private Map data;

    public String getType() {
        return type;
    }

    public void setType(String type) {
        this.type = type;
    }

    public Map getData() {
        return data;
    }

    public void setData(Map data) {
        this.data = data;
    }


}
