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
 *  SHA1: $Id: e5dddabdb2031643b8c0c3cfa4e7894204645b4a $
 * *********************************************************************************************
 *  File: AuthorizerConstants.java
 * ****************************************************************************************** */

package com.oracle.osvc.ds.constants;

public final class AuthorizerConstants {

    private AuthorizerConstants() {
        //Prevents instantiating the class.
    }

    public static final String AUTH_HEADER = "Authorization";
    public static final String DOMAIN_CONFIG = "DOMAIN";
    public static final String APP_CONTEXT = "auth_fn_context";

}
