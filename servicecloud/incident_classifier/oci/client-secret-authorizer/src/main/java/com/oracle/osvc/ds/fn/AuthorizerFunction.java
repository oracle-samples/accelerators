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
 *  SHA1: $Id: 110f5de878239d448bdf00236c9813c716d3f9c7 $
 * *********************************************************************************************
 *  File: AuthorizerFunction.java
 * ****************************************************************************************** */

package com.oracle.osvc.ds.fn;

import com.fnproject.fn.api.RuntimeContext;
import com.oracle.osvc.ds.constants.AuthorizerConstants;
import com.oracle.osvc.ds.model.AuthorizerInput;
import com.oracle.osvc.ds.model.AuthorizerOutput;
import com.oracle.osvc.ds.utils.ConnectionUtil;
import org.apache.commons.lang3.ObjectUtils;
import org.apache.commons.lang3.StringUtils;

import java.io.IOException;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.util.logging.Level;
import java.util.logging.Logger;

public class AuthorizerFunction {

    private static final Logger logger = Logger.getAnonymousLogger();
    private static final String TOKEN_PREFIX = "Basic ";


    public AuthorizerOutput handleRequest(AuthorizerInput input, RuntimeContext ctx) {

        logger.log(Level.INFO, "Authenticating request source...");
        AuthorizerOutput output = new AuthorizerOutput();

        if (!ctx.getConfigurationByKey(AuthorizerConstants.DOMAIN_CONFIG).isPresent()
                || StringUtils.isEmpty(ctx.getConfigurationByKey(AuthorizerConstants.DOMAIN_CONFIG).get())) {
            throw new RuntimeException("Please set config parameter" +
                    " DOMAIN to b2c_site_domain!");
        }

        String domain = ctx.getConfigurationByKey(AuthorizerConstants.DOMAIN_CONFIG).get();

        if (ObjectUtils.isEmpty(input) || ObjectUtils.isEmpty(input.getData())
                || ObjectUtils.isEmpty(input.getData().get(AuthorizerConstants.AUTH_HEADER))) {

            logger.log(Level.WARNING, "Authentication Error - Missing credentials !!!");
            output.setActive(false);
            output.setWwwAuthenticate("Basic realm= " + domain);

        } else if (!input.getData().get(AuthorizerConstants.AUTH_HEADER).toString().startsWith(TOKEN_PREFIX)) {

            logger.log(Level.WARNING, "Request Error!!! Invalid authorization type.");
            output.setActive(false);
            output.setWwwAuthenticate("Basic realm= " + domain);
        } else {

            logger.log(Level.INFO, "Verifying authorization token");
            String authHeader = input.getData().get(AuthorizerConstants.AUTH_HEADER).toString();

            try {

                HttpURLConnection con =  ConnectionUtil.getConnection(domain);
                con.setRequestProperty(AuthorizerConstants.AUTH_HEADER, authHeader);
                con.setRequestProperty("osvc-crest-application-context", AuthorizerConstants.APP_CONTEXT);
                con.connect();

                int responseCode = con.getResponseCode();
                logger.log(Level.INFO, "Verification response code: " + responseCode);

                if (HttpURLConnection.HTTP_OK == responseCode) {
                    logger.log(Level.INFO, "Successfully verified client identity.");
                    output.setActive(true);
                } else {
                    logger.log(Level.INFO, "Verification failed!!");
                    output.setActive(false);
                    output.setWwwAuthenticate("Basic realm= " + domain);
                }
            } catch (MalformedURLException e) {
                logger.log(Level.SEVERE, "Exception occurred due to malformed URL: \n" + e);
                output.setActive(false);
                output.setWwwAuthenticate("Basic realm= " + domain);

            } catch (IOException e) {
                logger.log(Level.SEVERE, "Exception creating connection : \n" + e);
                output.setActive(false);
                output.setWwwAuthenticate("Basic realm= " + domain);
            }
        }
        return output;
    }

}