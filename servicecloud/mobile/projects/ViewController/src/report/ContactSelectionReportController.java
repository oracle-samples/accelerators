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

package report;


import java.util.ArrayList;

import oracle.adfmf.framework.api.AdfmfJavaUtilities;
import oracle.adfmf.json.JSONArray;
import oracle.adfmf.json.JSONException;
import oracle.adfmf.json.JSONObject;

import rest_adapter.RestAdapter;

public class ContactSelectionReportController {
    
    private static final String REPORT_URL_CONNECTION = (String) AdfmfJavaUtilities.evaluateELExpression("#{applicationScope.QUERY_URL_CONNECTION}");
    private static final String REPORT_GET_URI = (String) AdfmfJavaUtilities.evaluateELExpression("#{applicationScope.QUERY_REPORT_URI}");
    private static final int DEFAULT_LIMIT = 2000;
    
    private String _reportName = null;
    private String _srchObjName = null;

    public ContactSelectionReportController() {
        super();
    }
    
    public void initReport(String reportName, String searchObjectName) {
        System.out.println("initReport(ContactSelectionReportController) called, reportName:"+reportName+ " searchingObj: "+searchObjectName);

        this._reportName = reportName;
        this._srchObjName = searchObjectName;
        
        AdfmfJavaUtilities.setELValue("#{pageFlowScope.searchLinkItem}",  _srchObjName );
    }
    
    public ReportItem getSelection(){
        System.out.println("getSelection(ContactSelectionReportController) called ");
        ReportItem item = new ReportItem();
        return item;
    }
    
    public void setSelection(ReportItem item) {
        System.out.println("setSelection called "+item);
    }
    
    /**
     * objectName Organization has 1 filters, others (e.g. Contacts) have 2 !!!
     * 
     * @param objectName
     * @param searchValue
     * @param isRequired
     * @return
     */
    public ReportItem[] searchContactWithReport(String objectName, String searchValue, boolean isRequired) {
        System.out.println("searchContactWithReport called, searchValue:"+searchValue );
        boolean isTwoFilters = !"Organization".equals(objectName);
        ArrayList<ReportItem> returnValue = new ArrayList<ReportItem> ();
        
        if (!isRequired){
            System.out.println("searchContactWithReport called, is NOT required!");
            ReportItem noValue = new ReportItem();
            noValue.setAttr1("[ No Value ]");
            if (isTwoFilters) {
                noValue.setAttr2("");
            }
            noValue.setId("0");
            returnValue.add(noValue);
        }
                
        if (searchValue != null && !searchValue.isEmpty()) {
            // not empty, start search
            try {
                JSONObject queryObj = new JSONObject();

                queryObj.put("lookupName", this._reportName);
                
                searchValue = searchValue + "%";
                JSONArray filterArray = new JSONArray();
                filterArray.put( new JSONObject().put("name", "fn").put("values", new JSONArray().put( searchValue )) );
                if (isTwoFilters) {
                    filterArray.put( new JSONObject().put("name", "ln").put("values", new JSONArray().put( searchValue )) );
                }
                queryObj.put("filters", filterArray);                  
                
                queryObj.put("limit", DEFAULT_LIMIT);
                queryObj.put("offset", 0);

                String response = RestAdapter.doPOST(REPORT_URL_CONNECTION, REPORT_GET_URI, queryObj.toString());

                if (response != null) {
                    JSONObject obj = null;
                    try {
                        obj = new JSONObject(response);
                    } catch (JSONException e) {
                        System.out.println("Exception while parsing response: "+e);
                        System.out.println("Response: "+response);
                        obj = null;
                        
                        ReportItem noValue = new ReportItem();
                        noValue.setAttr1("[ Server Error retrieving results ]");
                        if (isTwoFilters) {
                            noValue.setAttr2("");
                        }
                        noValue.setId("0");
                        returnValue.add(noValue);
                    }
                    
                    if (obj != null) {
                        JSONArray rows = obj.getJSONArray("rows");
    //                    int fieldCount = obj.getJSONArray("columnNames").length();
    //                    System.out.println("Search Response has N="+fieldCount+" columns and " + rows.length() + " rows");
                        int i;
                        for (i = 0; rows != null && i < rows.length(); i++) {
    
                            JSONArray fields = (JSONArray) rows.get(i);
                            ReportItem reportObj = new ReportItem();
                            
                            String idValue = fields.getString(0);
                            String firstNameValue = fields.getString(1);
        
                            System.out.println("Got idValue="+idValue );                        
                            reportObj.setId((idValue));
                            reportObj.setAttr1(firstNameValue);
                            if (isTwoFilters) {
                                String lastNameValue = fields.getString(2);
                                reportObj.setAttr2(lastNameValue);
                            }
                                                    
                            returnValue.add(reportObj);
                        }
                        if (i != 0) {
                            // set this if we have results;
                            AdfmfJavaUtilities.setELValue("#{pageFlowScope.searchLinkItem}", _srchObjName );
                        } else {
                            AdfmfJavaUtilities.setELValue("#{pageFlowScope.searchLinkItem}", "Nodata" );
                            System.out.println("Search Response has  " + rows.length() + " rows");
                        }
                    }
                }

            } catch (Exception e) {
                e.printStackTrace();
            }
        }
        
        ReportItem[] returnItems = returnValue.toArray(new ReportItem[returnValue.size()]); 
        return returnItems;
    }

    public ReportItem[] searchIncidentWithReport(String searchValue, boolean isRequired) {
        System.out.println("searchIncidentWithReport: searchValue = " + searchValue);
        ArrayList<ReportItem> results = new ArrayList<ReportItem> ();
        
        if (!isRequired){
            System.out.println("searchIncidentWithReport called, is NOT required!");
            ReportItem noValueItem = new ReportItem();
            noValueItem.setId("0");
            noValueItem.setAttr1("[ No Value ]");
            results.add(noValueItem);
        }
                
        if (searchValue != null && !searchValue.trim().isEmpty()) {
            try {
                JSONArray filterArray = new JSONArray();
                // put search value String directly
                filterArray.put(new JSONObject().put("name", "refno").put("values", new JSONArray().put( searchValue )));

                JSONObject queryObj = new JSONObject();
                queryObj.put("lookupName", this._reportName);
                queryObj.put("filters", filterArray);                
                queryObj.put("limit", DEFAULT_LIMIT);
                queryObj.put("offset", 0);

                String response = RestAdapter.doPOST(REPORT_URL_CONNECTION, REPORT_GET_URI, queryObj.toString());

                if (response != null) {
                    JSONObject obj = null;
                    try {
                        obj = new JSONObject(response);
                    } catch (JSONException e) {
                        System.out.println("Exception while parsing response: " + e);
                        System.out.println("Response: " + response);
                        obj = null;
                        
                        ReportItem itemWithErrorMsg = new ReportItem();
                        itemWithErrorMsg.setId("0");
                        itemWithErrorMsg.setAttr1("[ Server Error retrieving results ]");
                        results.add(itemWithErrorMsg);
                    }
                    
                    if (obj != null) {
                        JSONArray rows = obj.getJSONArray("rows");
                        int i;
                        // rows != null??
                        for (i = 0; rows != null && i < rows.length(); i++) {    
                            JSONArray fields = (JSONArray) rows.get(i);
                            ReportItem reportObj = new ReportItem();
                            
                            String idValue = fields.getString(0);
                            String attr1Value = fields.getString(1);
                            String attr2Value = fields.getString(2);
                            
                            System.out.println("Retrieved item with idValue = " + idValue);                        
                            reportObj.setId(idValue);
                            reportObj.setAttr1(attr1Value);
                            reportObj.setAttr2(attr2Value);
                                                    
                            results.add(reportObj);
                        }
                        if (i == 0) {
                            AdfmfJavaUtilities.setELValue("#{pageFlowScope.searchLinkItem}", "Nodata" );
                            System.out.println("Search Response has  " + rows.length() + " rows");
                        } else {
                            // Set this if we have results;
                            AdfmfJavaUtilities.setELValue("#{pageFlowScope.searchLinkItem}", _srchObjName );                            
                        }
                    }
                }
            } catch (Exception e) {
                e.printStackTrace();
            }
        }
        ReportItem[] returnItems = results.toArray(new ReportItem[results.size()]); 
        return returnItems;
    }
}
