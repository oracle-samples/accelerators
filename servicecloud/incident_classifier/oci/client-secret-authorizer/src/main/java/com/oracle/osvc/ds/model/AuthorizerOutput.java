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
 *  SHA1: $Id: 0ef2f57da0e2fc27d329d6e5498345505578b8cf $
 * *********************************************************************************************
 *  File: AuthorizerOutput.java
 * ****************************************************************************************** */

package com.oracle.osvc.ds.model;

public class AuthorizerOutput {
    private Boolean active;
    private String wwwAuthenticate;

    public Boolean getActive() {
        return active;
    }

    public void setActive(Boolean active) {
        this.active = active;
    }

    public String getWwwAuthenticate() {
        return wwwAuthenticate;
    }

    public void setWwwAuthenticate(String wwwAuthenticate) {
        this.wwwAuthenticate = wwwAuthenticate;
    }

}
