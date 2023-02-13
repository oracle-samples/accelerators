/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
 *  http://oss.oracle.com/licenses/upl
 *  Copyright (c) 2023, Oracle and/or its affiliates.
 ***********************************************************************************************
 *  Accelerator Package: Incident Text Based Classification
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 23A (February 2023) 
 *  date: Tue Jan 31 13:02:56 IST 2023
 
 *  revision: rnw-23-02-initial
 *  SHA1: $Id: 43e5ea93dbb17a65f3f5a4b9b3613332a24bd191 $
 * *********************************************************************************************
 *  File: ConnectionUtil.java
 * ****************************************************************************************** */

package com.oracle.osvc.ds.utils;

import java.io.IOException;
import java.net.HttpURLConnection;
import java.net.URL;
import java.util.logging.Level;
import java.util.logging.Logger;

public class ConnectionUtil {
    private static final Logger logger = Logger.getAnonymousLogger();


    public static HttpURLConnection getConnection(String domain) throws IOException {
        URL url =  new URL("https://" + domain + "/services/rest/connect/v1.4/");
        logger.log(Level.INFO, "Verifying credentials against: " + url);

        return (HttpURLConnection) url.openConnection();
    }
}
