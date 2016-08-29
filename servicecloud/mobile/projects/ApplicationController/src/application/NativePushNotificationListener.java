/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: Mobile Agent App Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 16.8 (August 2016)
 *  MAF release: 2.3
 *  reference: 151217-000185
 *  date: Tue Aug 23 16:35:47 PDT 2016

 *  revision: rnw-16-8-fixes-release-01
 *  SHA1: $Id: 11073db587bc7bd8436da5be2118173a40ba159e $
 * *********************************************************************************************
 *  File: NativePushNotificationListener.java
 * *********************************************************************************************/

package application;

import java.util.HashMap;

import javax.el.ValueExpression;

import oracle.adfmf.framework.api.AdfmfContainerUtilities;
import oracle.adfmf.framework.api.AdfmfJavaUtilities;
import oracle.adfmf.framework.api.JSONBeanSerializationHelper;
import oracle.adfmf.framework.event.Event;
import oracle.adfmf.framework.event.EventListener;
import oracle.adfmf.framework.exception.AdfException;

public class NativePushNotificationListener implements EventListener {
    public NativePushNotificationListener() {
        super();
    }

    public void onMessage(Event event) {
        String msg = event.getPayload();
        
        // Parse the payload of the push notification
        HashMap payload = null;
        String pushMsg = "";
        String id = "";
        String feature = "";
        try {
            payload = (HashMap) JSONBeanSerializationHelper.fromJSON(HashMap.class, msg);
            pushMsg = (String) payload.get("alert");
            feature = (String) payload.get("feature");
            id = (String) payload.get("id");
        } catch (Exception e) {
            e.printStackTrace();
        }
        
        // Write the push message to app scope to display to the user
        ValueExpression ve = AdfmfJavaUtilities.getValueExpression("#{applicationScope.isNotification}", String.class);
        ve.setValue(AdfmfJavaUtilities.getELContext(), "true");
        
        ValueExpression ve1 = AdfmfJavaUtilities.getValueExpression("#{applicationScope.notificationId}", String.class);
        ve1.setValue(AdfmfJavaUtilities.getELContext(), id);
        
        ValueExpression ve2 = AdfmfJavaUtilities.getValueExpression("#{applicationScope.notificationFeature}", String.class);
        ve2.setValue(AdfmfJavaUtilities.getELContext(), feature);        
        
        AdfmfJavaUtilities.flushDataChangeEvent();
        
        AdfmfContainerUtilities.gotoFeature(feature);
        
    }

    public void onError(AdfException adfException) {
        System.out.println("#### Error: " + adfException.toString());
        // Write the error into app scope
        ValueExpression ve = AdfmfJavaUtilities.getValueExpression("#{applicationScope.errorMessage}", String.class);
        ve.setValue(AdfmfJavaUtilities.getELContext(), adfException.toString());

    }

    public void onOpen(String token) {
        // Clear error in app scope
        ValueExpression ve = AdfmfJavaUtilities.getValueExpression("#{applicationScope.errorMessage}", String.class);
        ve.setValue(AdfmfJavaUtilities.getELContext(), null);

        // Write the token into app scope
        ve = AdfmfJavaUtilities.getValueExpression("#{applicationScope.deviceToken}", String.class);
        ve.setValue(AdfmfJavaUtilities.getELContext(), token);
    }

}
