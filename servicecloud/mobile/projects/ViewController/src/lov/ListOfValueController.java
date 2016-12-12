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
 *  date: Tue Aug 23 16:35:58 PDT 2016

 *  revision: rnw-16-8-fixes-release-01
 *  SHA1: $Id: c41f3eedad6aa5339759d21de5f834baf18521e4 $
 * *********************************************************************************************
 *  File: ListOfValueController.java
 * *********************************************************************************************/

package lov;

import java.util.ArrayList;

import java.util.HashMap;

import java.util.Map;

import oracle.adfmf.framework.api.AdfmfJavaUtilities;
import oracle.adfmf.json.JSONArray;
import oracle.adfmf.json.JSONObject;

import report.ReportItem;

import rest_adapter.RestAdapter;


public class ListOfValueController {
    private static final String REPORT_URL_CONNECTION =
        (String) AdfmfJavaUtilities.evaluateELExpression("#{applicationScope.QUERY_URL_CONNECTION}");
    private static final String QUERY_LOV_URI =
        (String) AdfmfJavaUtilities.evaluateELExpression("#{applicationScope.QUERY_LOV_URI}");
    private static HashMap<String, ListOfValue[]> lovCache = new HashMap<String, ListOfValue[]>();
    private static final String REPORT_GET_URI =
        (String) AdfmfJavaUtilities.evaluateELExpression("#{applicationScope.QUERY_REPORT_URI}");
    private static final HashMap<Integer, String> staticIncidentThreadEntryTypeLOV;
    static {
        staticIncidentThreadEntryTypeLOV = new HashMap<Integer, String>();
        staticIncidentThreadEntryTypeLOV.put(1, "Private Note");
        staticIncidentThreadEntryTypeLOV.put(2, "Response");
        staticIncidentThreadEntryTypeLOV.put(4, "Customer Entry");
    }

    public ListOfValueController() {
        super();

    }

    public static ListOfValue[] getLov(String uri) {

        if (lovCache.containsKey(uri)) {

            return lovCache.get(uri);
        }

        ArrayList<ListOfValue> loadedLov = new ArrayList<ListOfValue>();

        try {
            // static lov
            if (uri.startsWith("static")) {
                // use case statement later for more cases
                if (uri == "staticIncidentThreadEntryTypeLOV") {
                    for (Map.Entry<Integer, String> entry : staticIncidentThreadEntryTypeLOV.entrySet()) {
                        ListOfValue value = new ListOfValue(entry.getKey(), entry.getValue());
                        loadedLov.add(value);
                    }
                }
            }
            // use report
            else if (uri.startsWith("Accelerator")) {
                String payload = "{\"lookupName\":\"" + uri + "\"}";
                String response = RestAdapter.doPOST(REPORT_URL_CONNECTION, REPORT_GET_URI, payload);

                if (response != null) {
                    JSONObject obj = new JSONObject(response);
                    JSONArray rows = obj.getJSONArray("rows");
                    int fieldCount = obj.getJSONArray("columnNames").length();

                    for (int i = 0; i < rows.length(); i++) {
                        JSONArray fields = (JSONArray) rows.get(i);
                        ReportItem item = new ReportItem();

                        Integer id = (Integer) fields.getInt(0);
                        String lookupName = fields.getString(1);
                        ListOfValue value = new ListOfValue(id, lookupName);
                        loadedLov.add(value);
                    }
                }
            } 
            // use ROQL : salesProducts attribute attributes.IsServiceProduct not available in report
                else if (uri.startsWith("/?query")) {                
                    String QUERY_URL_CONNECTION =
                        (String) AdfmfJavaUtilities.evaluateELExpression("#{applicationScope.QUERY_URL_CONNECTION}");
                    String QUERY_GET_URI =
                        (String) AdfmfJavaUtilities.evaluateELExpression("#{applicationScope.QUERY_RESULTS_URI}");

                    String response = RestAdapter.doGET(QUERY_URL_CONNECTION, QUERY_GET_URI + uri);
                    System.out.println("response: " + response);

                    try {
                        JSONObject jsonObj = new JSONObject(response);
                        JSONArray items = jsonObj.getJSONArray("items");
                        JSONObject item = items.getJSONObject(0);
                        JSONArray rows = item.getJSONArray("rows");
                        for (int i = 0; i < rows.length(); i++) {
                        JSONArray fields = (JSONArray) rows.get(i);

                        Integer id = (Integer) fields.getInt(0);
                        String lookupName = fields.getString(1);
                        ListOfValue value = new ListOfValue(id, lookupName);
                        loadedLov.add(value);  
                        }
                    } catch (Exception e) {
                        e.printStackTrace();
                    }
                } else {
                String response = RestAdapter.doGET(REPORT_URL_CONNECTION, QUERY_LOV_URI + uri);
                if (response != null) {
                    JSONObject obj = new JSONObject(response);
                    JSONArray items = obj.getJSONArray("items");

                    for (int i = 0; i < items.length(); i++) {

                        JSONObject item = (JSONObject) items.get(i);

                        Integer id = (Integer) item.get("id");
                        String lookupName = (String) item.get("lookupName");
                        ListOfValue value = new ListOfValue(id, lookupName);
                        loadedLov.add(value);

                    }
                }
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
        ListOfValue[] lovs = loadedLov.toArray(new ListOfValue[loadedLov.size()]);
        lovCache.put(uri, lovs);

        return lovs;

    }
}
