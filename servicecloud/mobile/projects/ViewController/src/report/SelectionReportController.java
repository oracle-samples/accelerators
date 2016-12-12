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
 *  date: Tue Aug 23 16:35:59 PDT 2016

 *  revision: rnw-16-8-fixes-release-01
 *  SHA1: $Id: a85feed137270a2d5f2aaa3f1fae6d6b0abf4e4f $
 * *********************************************************************************************
 *  File: SelectionReportController.java
 * *********************************************************************************************/

package report;

import incidents.Incident;
import incidents.IncidentAttributes;

import java.sql.Connection;
import java.sql.ResultSet;
import java.sql.Statement;

import java.util.ArrayList;

import oracle.adfmf.util.Utility;
import util.Util;

import database.ConnectionFactory;

import java.sql.DatabaseMetaData;

import oracle.adfmf.framework.api.AdfmfJavaUtilities;
import oracle.adfmf.json.JSONArray;
import oracle.adfmf.json.JSONException;
import oracle.adfmf.json.JSONObject;

import rest_adapter.RestAdapter;

public class SelectionReportController{

    private static final String REPORT_URL_CONNECTION = (String) AdfmfJavaUtilities.evaluateELExpression("#{applicationScope.QUERY_URL_CONNECTION}");
    private static final String REPORT_GET_URI = (String) AdfmfJavaUtilities.evaluateELExpression("#{applicationScope.QUERY_REPORT_URI}");
    private static final int DEFAULT_LIMIT = 2000;
    
    private String _reportName = null;
    private String _filterName = null;
    private String _filterValue = null;
    private String _dataControl = null;
    
    private Connection conn = null;
    
    public SelectionReportController() {
        super();
    }
    
    public ReportItem getSelection(){
        ReportItem item = new ReportItem();
        return item;
    }
    
    public void setSelection(ReportItem item) {
    }
    
    public ReportItem[] searchSelectionFromDB(String objectName, String searchValue, boolean isRequired) {
        Utility.ApplicationLogger.severe("searchSelectionFromDB " + objectName);
        ArrayList<ReportItem> returnValue = new ArrayList<ReportItem> ();
        
        if(!isRequired && (searchValue == null || searchValue.isEmpty())){
            ReportItem noValue = new ReportItem();
            noValue.setAttr1("[ No Value ]");
            noValue.setId("0");
            returnValue.add(noValue);
            // Assume [No Value] is one return item and set isNoDataFound to false to show the item
            AdfmfJavaUtilities.setELValue("#{pageFlowScope.isNoDataFound}", false);
        }
        
        if (searchValue != null && !searchValue.trim().isEmpty()){
            try {
                conn = ConnectionFactory.getConnection();
                DatabaseMetaData dbm =conn.getMetaData();

                // check if $objectName table is there
                ResultSet tables = dbm.getTables(null, null, objectName, null);
                if (! tables.next()){
                    Utility.ApplicationLogger.severe("no table " + objectName);
                    //Table is not existed, create table and insert data
                    this.CreateTable(objectName);
                    this.InsertSelectionsToDB(objectName);
                }
                Utility.ApplicationLogger.severe("query " + objectName);
                Statement stmt = conn.createStatement();
                if(searchValue.endsWith("*")){
                    searchValue = searchValue.substring(0, searchValue.length()-1);
                }
                searchValue = Util.escapeQuotesInQueryStmt(searchValue);
                String queryString = "SELECT * FROM " + objectName + " WHERE LookupName LIKE '" + searchValue + "%' ORDER BY ID DESC;";
                Utility.ApplicationLogger.severe("query string: " + queryString);
                
                
                ResultSet result = stmt.executeQuery(queryString);
                
                Utility.ApplicationLogger.severe("do query" + objectName);
                
                while (result.next()) {
                    ReportItem reportObj = new ReportItem();
                    reportObj.setAttr1(result.getString("LookupName"));
                    reportObj.setId(Integer.toString(result.getInt("ID")));
                    Utility.ApplicationLogger.severe("ID " + result.getInt("ID"));
                    returnValue.add(reportObj);
                        
                }
    
            } catch (Exception ex) {
                Utility.ApplicationLogger.severe(ex.getMessage());
                ex.printStackTrace();
                throw new RuntimeException(ex);
            }
            
            if(returnValue.size() == 0){
                AdfmfJavaUtilities.setELValue("#{pageFlowScope.isNoDataFound}", true);
            } else{
                AdfmfJavaUtilities.setELValue("#{pageFlowScope.isNoDataFound}", false);
            }
        }
        ReportItem[] returnItems = returnValue.toArray(new ReportItem[returnValue.size()]); 
        return returnItems;
    }
    
    private void CreateTable(String objectName){
        try {
            Statement createTblStmt = conn.createStatement();
            
            String createTableSQL = "CREATE TABLE " + objectName + 
                                    " (LookupName VARCHAR(128), ID INTEGER PRIMARY KEY);";
            Utility.ApplicationLogger.severe(createTableSQL);
            createTblStmt.executeQuery(createTableSQL);
            
        } catch (Exception ex) {
            Utility.ApplicationLogger.severe(ex.getMessage());
            ex.printStackTrace();
            throw new RuntimeException(ex);
        }
    }
    
    private void InsertSelectionsToDB(String objectName){

        try {

            JSONObject queryObj = new JSONObject();

            queryObj.put("lookupName", this._reportName);
            
            if (this._filterName == null || this._filterName.isEmpty()){
                queryObj.put("filters", new JSONArray());
            }else{
                queryObj.put("filters",
                             new JSONObject().put("name", this._filterName).put("values",new JSONArray().put(this._filterValue)));
                
            }
            
            queryObj.put("limit", DEFAULT_LIMIT);
            queryObj.put("offset", 0);

            String response = RestAdapter.doPOST(REPORT_URL_CONNECTION, REPORT_GET_URI, queryObj.toString());


            if (response != null) {
                JSONObject obj = new JSONObject(response);
                JSONArray rows = obj.getJSONArray("rows");
                int fieldCount = obj.getJSONArray("columnNames").length();

                for (int i = 0; i < rows.length(); i++) {

                    JSONArray fields = (JSONArray) rows.get(i);
                    // make the sql statement
                    Statement insertItemStmt = conn.createStatement();
                    
                    String addItemSQL = "INSERT INTO " + objectName + " ";
                    String columnNames = "( ID, LookupName )";
                    String values = "( '" + fields.getString(0) + "', '" 
                                    + Util.escapeQuotesInQueryStmt(fields.getString(1)) + "')";
                    
                    addItemSQL += columnNames + " VALUES " + values + ";";
                    
                    
                    insertItemStmt.executeQuery(addItemSQL);

                }
            }

        } catch (Exception e) {
            e.printStackTrace();
        }

    }
    
    public void initReport(String reportName, String dataControl, String filterName, String filterValue) {
        this._reportName = reportName;
        this._dataControl = dataControl;
        this._filterName = filterName;
        this._filterValue = filterValue;
    }

    public ReportItem[] searchContactWithReport(String objectName, String searchValue, boolean isRequired) {
        ArrayList<ReportItem> returnValue = new ArrayList<ReportItem>();

        if (!isRequired && (searchValue == null || searchValue.isEmpty())) {
            ReportItem noValue = new ReportItem();
            noValue.setAttr1("[ No Value ]");
            noValue.setId("0");
            returnValue.add(noValue);
        }

        if (searchValue != null && !searchValue.isEmpty()) {
            try {
                JSONObject queryObj = new JSONObject();
                searchValue = searchValue + "%";
                JSONArray filterArray = new JSONArray();

                queryObj.put("lookupName", this._reportName);

                if (this._filterName != null && !this._filterName.isEmpty()) {
                    filterArray.put(new JSONObject().put("name", this._filterName).put("values",
                                                                                       new JSONArray().put(searchValue)));
                }
                queryObj.put("filters", filterArray);
                queryObj.put("limit", DEFAULT_LIMIT);
                queryObj.put("offset", 0);

                String response = RestAdapter.doPOST(REPORT_URL_CONNECTION, REPORT_GET_URI, queryObj.toString());

                if (response != null) {
                    JSONObject obj = new JSONObject(response);
                    if (obj != null) {
                    JSONArray rows = obj.getJSONArray("rows");
                    
                        for (int i = 0; rows != null && i < rows.length(); i++) {
                        
                            JSONArray fields = (JSONArray) rows.get(i);
                            ReportItem reportObj = new ReportItem();
                            
                            String idValue = fields.getString(0);
                            String attr1Value = fields.getString(1);
                        
                            System.out.println("Got idValue="+idValue );                        
                            reportObj.setId((idValue));
                            reportObj.setAttr1(attr1Value);             
                                                    
                            returnValue.add(reportObj);
                        }
                    }
                }
            } catch (Exception e) {
                e.printStackTrace();
            }
        }
        ReportItem[] returnItems = returnValue.toArray(new ReportItem[returnValue.size()]); 
        
        if(returnItems.length == 0){
            AdfmfJavaUtilities.setELValue("#{pageFlowScope.isNoDataFound}",true);
        }else{
            AdfmfJavaUtilities.setELValue("#{pageFlowScope.isNoDataFound}",false);
        }
        return returnItems;
    }
}
