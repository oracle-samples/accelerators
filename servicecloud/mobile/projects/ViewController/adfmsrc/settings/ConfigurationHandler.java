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

package settings;

import javax.el.ValueExpression;

import oracle.adf.model.datacontrols.device.DeviceManagerFactory;

import oracle.adfmf.framework.api.AdfmfContainerUtilities;
import oracle.adfmf.framework.api.AdfmfJavaUtilities;
import oracle.adfmf.framework.exception.AdfException;
import oracle.adfmf.java.beans.PropertyChangeListener;
import oracle.adfmf.java.beans.PropertyChangeSupport;
import oracle.adfmf.util.Utility;

import util.Util;
//import org.apache.commons.validator.routines.UrlValidator;

public class ConfigurationHandler {

    private PropertyChangeSupport propertyChangeSupport = new PropertyChangeSupport(this);

    public ConfigurationHandler() {
        super();
    }

    public void logout() {
        AdfmfJavaUtilities.logout();
        //AdfmfContainerUtilities.resetFeature("Logout");
    }

    private String getScopedConfigString(String ConfigKey) {
        ValueExpression ve1 = AdfmfJavaUtilities.getValueExpression(ConfigKey, String.class);
        return (String) ve1.getValue(AdfmfJavaUtilities.getELContext());
    }

    public void updateConnectionsEndpoints() {
        
        String serverURL = getScopedConfigString("#{preferenceScope.application.config.server_url}");
        System.out.println("Configured URL from preferences: " + serverURL);

        String CFG_SERVICE_CONN_NAME = getScopedConfigString("#{applicationScope.CFG_SERVICE_CONN_NAME}");
        String CFG_LOGIN_CONN_NAME = getScopedConfigString("#{applicationScope.CFG_LOGIN_CONN_NAME}");
        String CFG_LOGIN = getScopedConfigString("#{applicationScope.CFG_REST_URI}");
        String CFG_LOGOUT = getScopedConfigString("#{applicationScope.CFG_REST_URI}");
        String CFG_SERVICE = getScopedConfigString("#{applicationScope.CFG_REST_URI}");

        if (!Utility.isEmpty(serverURL)) {
            //String[] schemes = {"http","https"};
            //UrlValidator urlValidator = new UrlValidator(schemes);
            //if (!urlValidator.isValid(serverURL)) {
            if (!serverURL.matches("(http(s)?://)?([\\w-]+\\.)+[\\w-]+(/[\\w- ;,./?%&=]*)?")) {
                AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities.getFeatureId(),
                                                                          "navigator.notification.alert", new Object[] {
                                                                          "url is invalid", null, "Error", "OK"
                });
                return;
            }

            AdfmfJavaUtilities.clearSecurityConfigOverrides(CFG_LOGIN_CONN_NAME);
            AdfmfJavaUtilities.clearSecurityConfigOverrides(CFG_SERVICE_CONN_NAME);
            AdfmfJavaUtilities.overrideConnectionProperty(CFG_LOGIN_CONN_NAME, "login", "url", serverURL + CFG_LOGIN);
            AdfmfJavaUtilities.overrideConnectionProperty(CFG_LOGIN_CONN_NAME, "logout", "url", serverURL + CFG_LOGOUT);
            AdfmfJavaUtilities.overrideConnectionProperty(CFG_SERVICE_CONN_NAME, "urlconnection", "url",
                                                          serverURL + CFG_SERVICE);
            AdfmfJavaUtilities.addWhiteListEntry(AdfmfJavaUtilities.getFeatureId(), serverURL, false);
            AdfmfJavaUtilities.updateApplicationInformation(false);
        }
    }

    public void addPropertyChangeListener(PropertyChangeListener l) {
        propertyChangeSupport.addPropertyChangeListener(l);
    }

    public void removePropertyChangeListener(PropertyChangeListener l) {
        propertyChangeSupport.removePropertyChangeListener(l);
    }

    public void storeNotificationRegisterId(String token) {
        if (token != null && !token.isEmpty()){
            Util.saveRegisterId(token);
            String deviceOS = DeviceManagerFactory.getDeviceManager().getOs();
            if (deviceOS.contains("iOS")) {
                String oracleMobileBackendId = (String)AdfmfJavaUtilities.getELValue("#{applicationScope.configuration.oracleMobileBackendID}");
                String basicAuth = (String)AdfmfJavaUtilities.getELValue("#{applicationScope.configuration.oracleMobileBackendToken}");
                if(oracleMobileBackendId == null || oracleMobileBackendId.isEmpty()){
                    return;
                }else if (basicAuth == null || basicAuth.isEmpty()){
                    return;
                }else{
                    Util.MCSRegistration(token, oracleMobileBackendId, basicAuth);
                }
            }
        }
    }
    
    public String getLoginUser(){
        return Util.getUserDisplayName();
    }

}
