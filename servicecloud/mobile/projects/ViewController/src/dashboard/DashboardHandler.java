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

package dashboard;

import java.util.HashMap;

import javax.el.ValueExpression;

import oracle.adf.model.datacontrols.device.DeviceManagerFactory;

import oracle.adfmf.application.LifeCycleListener;
import oracle.adfmf.framework.ApplicationInformation;
import oracle.adfmf.framework.api.AdfmfContainerUtilities;
import oracle.adfmf.framework.api.AdfmfJavaUtilities;
import oracle.adfmf.json.JSONArray;
import oracle.adfmf.json.JSONObject;
import rest_adapter.RestAdapter;

public class DashboardHandler implements LifeCycleListener {
    private final static String QUERY_URL_CONNECTION = (String) AdfmfJavaUtilities.getELValue("#{applicationScope.QUERY_URL_CONNECTION}");
    private final static String QUERY_GET_URI = (String) AdfmfJavaUtilities.getELValue("#{applicationScope.QUERY_RESULTS_URI}");
    private int userProfileId;
    
    public DashboardHandler() {
        super();
    }

    public void activate() {
        // set it here regardless of the config verb exists or not
        AdfmfJavaUtilities.setELValue("#{applicationScope.configuration.server_timezone}", "America/Los_Angeles");
        AdfmfJavaUtilities.setELValue("#{applicationScope.configuration.client_datetime_format}", "MM/dd/yyyy h:mm a");
        AdfmfJavaUtilities.setELValue("#{applicationScope.configuration.client_dateOnly_format}", "MM/dd/yyyy");
        
        String config = this.getConfiguration("CUSTOM_CFG_Accel_Mobile");
        if(config == null || config == ""){
            return;
        }
        this.updateConfigurations(config);
        
        this.getUserProfileId();
        this.setupObjectsPermissions();
    }

    public void deactivate() {
    }

    public void start() {
    }

    public void stop() {
    }
    
    private String getConfiguration(String configurationName){
        String query =
            QUERY_GET_URI + "/?query=%20select%20value%20from%20configurations%20where%20lookupName%20=%20%27" + configurationName + "%27";
        String getResponse = RestAdapter.doGET(QUERY_URL_CONNECTION, query);
        String config = "";
        try {
            if (getResponse != null) {
                System.out.println("get response: " + getResponse);
                JSONObject jsonObj = new JSONObject(getResponse);
                JSONArray items = jsonObj.getJSONArray("items");

                if (items != null && items.length() == 0)
                    return config;

                JSONObject item = items.getJSONObject(0);
                JSONArray rows = item.getJSONArray("rows");

                if (rows == null || rows.length() == 0)
                    return config;
                JSONArray fields = (JSONArray) rows.get(0);
                config = fields.getString(0);
                System.out.println("configVerb: " + config);
            }
            
        } catch (Exception e) {
            e.printStackTrace();
        }
        return config;
    }
    
    public void updateConfigurations(String config){
        try {
            
            JSONObject jsonConfig = new JSONObject(config);
            JSONArray jsonHosts = jsonConfig.optJSONArray("hosts");
            if (jsonHosts == null)
                return;
            
            //Get the config of current rnt host
            String serverURL = getScopedConfigString("#{preferenceScope.application.config.server_url}");
            System.out.println("serverURL: " + serverURL);
            JSONObject currentHost = null;
            for (int i=0; i < jsonHosts.length(); i++) {
                JSONObject jsonHost = jsonHosts.getJSONObject(i);
                String rntHost = jsonHost.optString("rnt_host");
                if(rntHost != null && rntHost.equals(serverURL)){
                    currentHost = jsonHost;
                    break;
                }
            }
            
            if(currentHost == null)
                return;
            
            //Parse config of current host
            String serverTimezone = currentHost.optString("server_timezone");
            if (serverTimezone.equals("") || serverTimezone == null){
                AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities.getFeatureId(),
                            "navigator.notification.alert",
                            new Object[] {"Configuration server_timezone is empty. Default America/Los_Angeles is used", null, "Warning", "OK"});
            }
            else 
                AdfmfJavaUtilities.setELValue("#{applicationScope.configuration.server_timezone}", serverTimezone);
            

            String client_datetime_format = currentHost.optString("client_datetime_format");
            if (client_datetime_format.equals("") || client_datetime_format == null){
                /*AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities.getFeatureId(),
                            "navigator.notification.alert",
                            new Object[] {"Configuration client_datetime_format is empty. Default MM/dd/yyyy h:mm a is used", null, "Warning", "OK"});*/
            }
            else
                AdfmfJavaUtilities.setELValue("#{applicationScope.configuration.client_datetime_format}", client_datetime_format);
 
            String client_dateOnly_format = currentHost.optString("client_dateOnly_format");
            if (client_dateOnly_format.equals("") || client_dateOnly_format == null){
                /*AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities.getFeatureId(),
                            "navigator.notification.alert",
                            new Object[] {"Configuration client_datetOnly_format is empty. Default MM/dd/yyyy is used", null, "Warning", "OK"});*/
            }
            else
                AdfmfJavaUtilities.setELValue("#{applicationScope.configuration.client_dateOnly_format}", client_dateOnly_format);
            
            JSONObject pushNotification = currentHost.optJSONObject("push_notification");
            if (pushNotification != null){
                JSONObject gcmConfig = pushNotification.optJSONObject("gcm");
                if(gcmConfig != null){
                    String senderId = gcmConfig.optString("senderId");
                    senderId = (senderId==null)?"":senderId;
                    //AdfmfJavaUtilities.setELValue("#{applicationScope.configuration.gcmSenderId}", senderId);
                    AdfmfJavaUtilities.setELValue("#{preferenceScope.application.config.gcm_senderId}", senderId);                
                }
                
                JSONObject mcsConfig = pushNotification.optJSONObject("mcs");
                if(mcsConfig != null){
                    String basedURL = mcsConfig.optString("basedURL");
                    basedURL = (basedURL==null)?"":basedURL;
                    
                    AdfmfJavaUtilities.overrideConnectionProperty("MCSServerConnection", "urlconnection", "url", basedURL);
                    
                    String mobileBackendId = mcsConfig.optString("mobileBackendId");
                    mobileBackendId = (mobileBackendId==null)?"":mobileBackendId;
                    
                    String basicAuthKey = mcsConfig.optString("basicAuthKey");
                    basicAuthKey = (basicAuthKey==null)?"":basicAuthKey;
                    
                    AdfmfJavaUtilities.setELValue("#{applicationScope.configuration.oracleMobileBackendID}", mobileBackendId);
                    AdfmfJavaUtilities.setELValue("#{applicationScope.configuration.oracleMobileBackendToken}", basicAuthKey);
                }
            }
            
                

        } catch (Exception e) {
            e.printStackTrace();
        }
    }
    
    private String getScopedConfigString(String ConfigKey) {
        ValueExpression ve1 = AdfmfJavaUtilities.getValueExpression(ConfigKey, String.class);
        return (String) ve1.getValue(AdfmfJavaUtilities.getELContext());
    }

    private void getUserProfileId() {
        String login = (String) AdfmfJavaUtilities.evaluateELExpression("#{securityContext.userName}");
        String queryString = "";
        queryString = "/?query=%20select%20" + "Profile.ID" + "%20from%20Accounts%20where%20login='" + login + "'";
       
        String response = RestAdapter.doGET(QUERY_URL_CONNECTION, QUERY_GET_URI + queryString);
        System.out.println("response: " + response);

        try {
            JSONObject jsonObj = new JSONObject(response);
            JSONArray items = jsonObj.getJSONArray("items");
            JSONObject item = items.getJSONObject(0);
            JSONArray rows = item.getJSONArray("rows");
            JSONArray fields = (JSONArray) rows.get(0);
            userProfileId = fields.getInt(0);
            // if other places need user profile id, uncomment following
            //AdfmfJavaUtilities.setELValue("#{applicationScope.USER_PROFILE_ID}", profileId);
        } catch (Exception e) {
            e.printStackTrace();
        }
    }
    
    private void setupObjectsPermissions() {
        if (userProfileId == 2) {
            AdfmfJavaUtilities.setELValue("#{applicationScope.CanAddEdit_Asset}", true);
            AdfmfJavaUtilities.setELValue("#{applicationScope.CanAddEdit_Contact}", true);
            AdfmfJavaUtilities.setELValue("#{applicationScope.CanAddEdit_Incident}", true);
            AdfmfJavaUtilities.setELValue("#{applicationScope.CanAddEdit_Task}", true);
            AdfmfJavaUtilities.setELValue("#{applicationScope.CanAddEdit_Organization}", true);
        }
        else {
            AdfmfJavaUtilities.setELValue("#{applicationScope.CanAddEdit_Asset}", false);
            AdfmfJavaUtilities.setELValue("#{applicationScope.CanAddEdit_Contact}", false);
            AdfmfJavaUtilities.setELValue("#{applicationScope.CanAddEdit_Incident}", false);
            AdfmfJavaUtilities.setELValue("#{applicationScope.CanAddEdit_Task}", false);
            AdfmfJavaUtilities.setELValue("#{applicationScope.CanAddEdit_Organization}", false);
        }
    }
}
