/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published 
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 
 *  included in the original distribution. 
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved. 
  ***********************************************************************************************
 *  Accelerator Package: OSVC Mobile Application Accelerator 
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html 
 *  OSvC release: 16.11 (November 2016) 
 *  date: Mon Dec 12 02:05:30 PDT 2016 
 *  revision: rnw-16-11

 *  SHA1: $Id$
 * *********************************************************************************************
 *  File: This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.

 * *********************************************************************************************/

package rest_adapter;

import java.util.HashMap;

import oracle.maf.api.dc.ws.rest.RestServiceAdapter;
import oracle.maf.api.dc.ws.rest.RestServiceAdapterFactory;
import oracle.adfmf.framework.api.AdfmfContainerUtilities;
import oracle.adfmf.framework.api.AdfmfJavaUtilities;
import oracle.adfmf.util.Utility;


/**------------------------------------------------------------------------
 *
 * RestAdapter
 *
 * - Based on RestServiceAdapter
 * - provide GET, POST request functions
 *
 ------------------------------------------------------------------------*/
public class RestAdapter {

    private final static String LOG_TAG = RestAdapter.class.getSimpleName();

    public RestAdapter() {
        super();
    }

    /**
     * Does GET.
     *
     * @param connectionName
     * @param requestURI
     * @return
     */
    public static String doGET(String connectionName, String requestURI) {
        return doHttpMethodByParam(connectionName, requestURI, RestServiceAdapter.REQUEST_TYPE_GET);
    }

    /**
     *
     * @param connectionName
     * @param requestURI
     * @return
     */
    public static String doDELETE(String connectionName, String requestURI) {
        return doHttpMethodByParam(connectionName, requestURI, RestServiceAdapter.REQUEST_TYPE_DELETE);
    }

    /**
     * does HttpMethod like GET or DELETE. Private.
     * @param connectionName
     * @param requestURI
     * @param method e.g. RestServiceAdapter.REQUEST_TYPE_GET
     * @return
     */
    private static String doHttpMethodByParam(String connectionName, String requestURI, String method) {
        RestServiceAdapter restServiceAdapter = RestServiceAdapterFactory.newFactory().createRestServiceAdapter();
        restServiceAdapter.clearRequestProperties();
        restServiceAdapter.setConnectionName(connectionName);
        restServiceAdapter.setRequestMethod( method );
        restServiceAdapter.setRetryLimit(0);
        restServiceAdapter.setRequestURI(requestURI);

        String response = "";

        try {
            //TODO
            System.out.println("do"+method+" uri=>" + requestURI);

            response = restServiceAdapter.send("");

        } catch (Exception e) {
            Utility.ApplicationLogger.severe(e.getMessage());
            e.printStackTrace();

            int status = restServiceAdapter.getResponseStatus();
            // TODO:
            System.out.println("Status: " +status);

            // pop up error message
            if(status != 200){
                AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities.getFeatureName(),
                                                                        "navigator.notification.alert",
                                                                        new Object[] {e.getMessage(), null, "Server Error", "Ok" });

            }
        }

        return response;
    }


    public static String doPOST(String connectionName, String requestURL, String payload) {

        RestServiceAdapter restServiceAdapter = RestServiceAdapterFactory.newFactory().createRestServiceAdapter();
        restServiceAdapter.clearRequestProperties();
        restServiceAdapter.setConnectionName(connectionName);
        restServiceAdapter.setRequestMethod(RestServiceAdapter.REQUEST_TYPE_POST);
        restServiceAdapter.setRetryLimit(0);
        restServiceAdapter.setRequestURI(requestURL);

        String response;

        //TODO
        System.out.println("doPOST=>" + payload);//);

        try {

            response = restServiceAdapter.send(payload);

            //TODO
            System.out.println(response);

            return response;

        } catch (Exception e) {
            Utility.ApplicationLogger.severe(e.getMessage());
            e.printStackTrace();

            // pop up error message
            AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities.getFeatureName(),
                                                                        "navigator.notification.alert",
                                                                      new Object[] {e.getMessage(), null, "Error", "Ok" });

        }

        return null;
    }

    public static RestAdapter.Status doPATCH(String connectionName, String requestURL, String payload) {
        String retValStatus = "200";
        String retValMessage = null;

        RestServiceAdapter restServiceAdapter = RestServiceAdapterFactory.newFactory().createRestServiceAdapter();
        restServiceAdapter.clearRequestProperties();
        restServiceAdapter.setConnectionName(connectionName);
        restServiceAdapter.setRequestMethod(RestServiceAdapter.REQUEST_TYPE_POST);
        restServiceAdapter.setRetryLimit(0);
        restServiceAdapter.setRequestURI(requestURL);

        restServiceAdapter.addRequestProperty("X-HTTP-Method-Override", "PATCH");

        String response = "";

        // todo
        System.out.println("Request URL: " + requestURL);
        System.out.println("Request type: " + restServiceAdapter.getRequestMethod() + " (plus PATCH)");
        System.out.println("Request: " + payload);

        try {

            response = restServiceAdapter.send(payload);

            // TODO:
            System.out.println("Response:" +response);

        } catch (Exception e) {
            Utility.ApplicationLogger.severe(e.getMessage());
            Throwable cause = e.getCause();

            // response: is: Bad Request
            int status = restServiceAdapter.getResponseStatus();
            retValStatus = String.valueOf(status);
            retValMessage = cause.getMessage();

            System.out.println("Status: " +status);
            if (cause!=null)
                System.out.println("Cause getMessage: "+retValMessage);
            else
                e.printStackTrace();

            // // pop up error message
//            AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities.getFeatureName(),
//                                                                        "navigator.notification.alert",
//                                                                      new Object[] {e.getMessage(), null, "Error", "Ok" });
        }

        return new Status(retValStatus, retValMessage);
    }
    
    public static String doGeneralHttp(String connectionName, String requestURI, String payload, String method, HashMap<String, String> properties) {
        RestServiceAdapter restServiceAdapter = RestServiceAdapterFactory.newFactory().createRestServiceAdapter();
        restServiceAdapter.clearRequestProperties();
        restServiceAdapter.setConnectionName(connectionName);
        restServiceAdapter.setRequestMethod( method );
        restServiceAdapter.setRetryLimit(0);
        restServiceAdapter.setRequestURI(requestURI);

        //add request header properties
        properties.forEach((k,v)->restServiceAdapter.addRequestProperty(k, v));

        if(payload == null || payload.isEmpty()){
            payload = "";
        }
        String response = "";

        try {
            //TODO
            System.out.println("do"+method+" uri=>" + requestURI);
            System.out.println("payload => "+payload);

            response = restServiceAdapter.send(payload);

        } catch (Exception e) {
            Utility.ApplicationLogger.severe(e.getMessage());
            e.printStackTrace();

            int status = restServiceAdapter.getResponseStatus();
            // TODO:
            System.out.println("Status: " +status);

            // pop up error message
            if(status != 200){
                AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities.getFeatureName(),
                                                                        "navigator.notification.alert",
                                                                        new Object[] {e.getMessage(), null, "Server Error", "Ok" });

            }
        }

        return response;
    }

    /**
     * Public class which contains a Status code
     * and a Message
     */
    public static class Status {
        protected String status;
        protected String message;

        public Status(String status, String message){
            this.status = status;
            this.message = message;
        }
        public String getStatus() {
            if (status == null)
                return "400";
            return status;
        }

        /**
         * only used if you have an int status code
         * @param int statusCode
         * @return
         */
        protected String setStatus(int statusCode) {
            status = String.valueOf(statusCode);
            return status;
        }

        public String getMessage() {
            if (message==null)
                return "";
            return message;
        }
    }
}
