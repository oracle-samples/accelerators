/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: Mobile Agent App Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 16.8 (August 2016)
 *  MAF release: 2.3
 *  reference: 151217-000185
 *  date: Tue Aug 23 16:36:00 PDT 2016

 *  revision: rnw-16-8-fixes-release-01
 *  SHA1: $Id: 4f9c84c4dc90d40438107283ac3e523e37005354 $
 * *********************************************************************************************
 *  File: ThreadController.java
 * *********************************************************************************************/

package threads;

import java.time.ZoneId;
import java.time.ZonedDateTime;
import java.time.format.DateTimeFormatter;
import java.util.TimeZone;
import lov.ListOfValue;
import lov.ListOfValueController;
import oracle.adfmf.framework.api.AdfmfContainerUtilities;
import oracle.adfmf.framework.api.AdfmfJavaUtilities;
import oracle.adfmf.json.JSONObject;
import rest_adapter.RestAdapter;
import util.Util;

public class ThreadController {
    public static final String QUERY_URL_CONNECTION =
        (String) AdfmfJavaUtilities.evaluateELExpression("#{applicationScope.QUERY_URL_CONNECTION}");
    private Thread thread;
    private ListOfValue[] channelLOV = null;
    private ListOfValue[] entryTypeLOV = null;

    public void setThread(Thread thread) {
        this.thread = thread;
    }

    public Thread getThread() {
        AdfmfJavaUtilities.setELValue("#{pageFlowScope.isText}", false);
        AdfmfJavaUtilities.setELValue("#{pageFlowScope.isHtml}", false);
        
        String incidentId = (String) AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.incidentId}");
        String threadId = (String) AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.threadId}");
        thread = new Thread();

        if (threadId != null && !threadId.isEmpty()) {
            String restResourceURI = "/incidents/" + incidentId + "/threads/" + threadId;
            String response = RestAdapter.doGET(QUERY_URL_CONNECTION, restResourceURI);
            String contentType;
            String text, dateCreated;
            try {
                JSONObject jsonObj = new JSONObject(response);                
                contentType = jsonObj.getJSONObject("contentType").getString("lookupName");
                
                text = jsonObj.getString("text");
                AdfmfJavaUtilities.setELValue("#{pageFlowScope.text}", text);
                thread.setText(text);

                if (contentType.contentEquals("text/plain")) {
                    AdfmfJavaUtilities.setELValue("#{pageFlowScope.isText}", true);                    
                } 
                else {
                    AdfmfJavaUtilities.setELValue("#{pageFlowScope.isHtml}", true);  
                }  
                // thread object not available in REST ROQL, so need to do it separately
                String clientFormat = (String) AdfmfJavaUtilities.evaluateELExpression("#{applicationScope.configuration.client_datetime_format}");
                DateTimeFormatter localFormat = DateTimeFormatter.ofPattern(clientFormat);
                ZonedDateTime serverZonedTime = ZonedDateTime.parse(jsonObj.getString("createdTime"));
                
                TimeZone localTimeZone = TimeZone.getDefault();
                String localTimeZoneString = localTimeZone.getID();
                
                String tzNoColon00 = null;
                // Windows 10 format has :00 at the end, need to remove 
                if (localTimeZoneString.contains(":")) {
                    tzNoColon00 = localTimeZoneString.split(":")[0];
                    localTimeZoneString = tzNoColon00;
                }             
                ZoneId localZone = ZoneId.of(localTimeZoneString); 
                ZonedDateTime localZonedTime = serverZonedTime.withZoneSameInstant(localZone);
               thread.setDateCreated(localZonedTime.format(localFormat));

            } catch (Exception e) {
                System.out.println("getThread() JSONException : ");
                e.printStackTrace();
            }
        }
        return thread;
    }

    public ThreadController() {
        super();
    }

    public Thread newThread() {
        Thread thread = new Thread();
        thread.setEntryTypeId(4);
        return thread;
    }

    public ListOfValue[] getChannelLOV() {
        channelLOV = ListOfValueController.getLov("AcceleratorThreadChanneForSelect");
        return channelLOV;

    }

    public void setChannelLOV(ListOfValue[] channels) {
        channelLOV = channels;
    }

    public ListOfValue[] getEntryTypeLOV() {
       entryTypeLOV = ListOfValueController.getLov("staticIncidentThreadEntryTypeLOV");
        return entryTypeLOV;
    }

    public void setEntryTypeLOV(ListOfValue[] entryTypes) {
        entryTypeLOV = entryTypes;
    }

    public void submitNewThread(Thread thread) {
        String incidentId = (String) AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.incidentId}");

        Util.createObject(thread, "incidents/" + incidentId + "/threads", ThreadAttributes.threadDetailsAttrs);
        AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities.getFeatureId(),
                                                                  "adf.mf.api.amx.doNavigation", new Object[] {
                                                                  "__back" });
    }

}
