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
 *  date: Tue Aug 23 16:35:59 PDT 2016

 *  revision: rnw-16-8-fixes-release-01
 *  SHA1: $Id: 40606a16734f611357330f9bca39fa7fd47a5a3a $
 * *********************************************************************************************
 *  File: DashboardReportController.java
 * *********************************************************************************************/

package springboard;


import java.util.ArrayList;

import oracle.adfmf.amx.event.SelectionEvent;
import oracle.adfmf.framework.api.AdfmfJavaUtilities;

import oracle.adfmf.java.beans.PropertyChangeListener;
import oracle.adfmf.java.beans.PropertyChangeSupport;
import oracle.adfmf.json.JSONArray;
import oracle.adfmf.json.JSONException;
import oracle.adfmf.json.JSONObject;

import oracle.jbo.Row;

import report.ReportItem;

import rest_adapter.RestAdapter;

import tasks.Task;

public class DashboardReportController {
    private static final String REPORT_URL_CONNECTION = (String) AdfmfJavaUtilities.evaluateELExpression("#{applicationScope.QUERY_URL_CONNECTION}");
    private static final String REPORT_GET_URI = (String) AdfmfJavaUtilities.evaluateELExpression("#{applicationScope.QUERY_REPORT_URI}");
    private static final int DEFAULT_LIMIT = 2000;
    
    private String _reportName = null;
    private String _filterName = null;
    private String _filterValue = null;
    private String _dataControl = null;
    private DashboardReportController.PieSlice[] _savedSlicesReturnItems;
    private int _totalNumber;
    private PropertyChangeSupport propertyChangeSupport = new PropertyChangeSupport(this);

    public DashboardReportController() {
        super();
    }
        
    /**
     * Gives some pie, slice by slice
     * 
     * @param objectName
     * @param arg2
     * @return
     */
    public DashboardReportController.PieSlice[] pieChartFromReport(String objectName, String arg2) {
        boolean isPercents = false;
        System.out.println("pieChartFromReport called, object:"+objectName );
        ArrayList<ReportItem> returnValue = new ArrayList<ReportItem> ();
                
        try {
            JSONObject queryObj = new JSONObject();

            queryObj.put("lookupName", "AcceleratorDashStatus");
            
            JSONArray filterArray = new JSONArray();
            //
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
                    
                    PieSlice noValue = new PieSlice();
                    noValue.setAttr1("[ Server Error retrieving results ]");
                    noValue.setAttr2("");
                    noValue.setId("0");
                    returnValue.add(noValue);
                }

                setTotalNumber(0);                
                if (obj != null) {
                    JSONArray rows = obj.getJSONArray("rows");
                    int fieldCount = obj.getJSONArray("columnNames").length();
                    System.out.println("Report Response has N="+fieldCount+" columns/attrs and " + rows.length() + " rows");

                    int i = 0;
                    // calc tots
                    if (isPercents) {
                        for (i = 0; rows != null && i < rows.length(); i++) {
                            JSONArray fields = (JSONArray) rows.get(i);
                            int number = fields.getInt(1);       
                            _totalNumber += number;                                                
                        }
                        System.out.println("Collected totalNumber="+_totalNumber +" for percent calc.");
                    }
                    
                    for (i = 0; rows != null && i < rows.length(); i++) {
        
                        JSONArray fields = (JSONArray) rows.get(i);
                        PieSlice slice = new PieSlice();
                        
                        String name = fields.getString(0);
                        String idValue = String.valueOf ( fields.getInt(2) ); // id is status_id id 
                        
                        slice.setId(idValue);
                        slice.setName(name);
                        System.out.println("Got statusname="+name + " ; type ID:"+idValue );                       
                        
                        int number = fields.getInt(1);
                        if (isPercents && _totalNumber != 0) {
                            //slice.setNumber(Math.ceil(((double)number / _totalNumber)*10000) / 100);
                            slice.setNumber( (((double)number / _totalNumber)) );
                        } else {
                            _totalNumber += number;                                                
                            slice.setNumber( number );
                        }
                        System.out.println("Got numberCount: "+ number + " (% number set: "+slice.getNumber()+")" );                       
                        
                        returnValue.add(slice);
                    }
                    
                    // just in case, putting a flag after incidents status
                    if (i == 0) {
                        AdfmfJavaUtilities.setELValue("#{pageFlowScope.searchLinkItem}", "Nodata" );
                        System.out.println("Search Response has  " + rows.length() + " rows");
                    }
                    
                    setTotalNumber(_totalNumber);
                }
            }

        } catch (Exception e) {
            e.printStackTrace();
        }
        
        PieSlice[] returnItems = returnValue.toArray(new PieSlice[returnValue.size()]); 
        _savedSlicesReturnItems = returnItems;
        return returnItems;
    }
    
    /**
     *
     * @param selectionEvent
     */
    public void pieChartSelectionListener(SelectionEvent selectionEvent) {
        try {
        // Makes selected slice as current row
//        invokeEL("#{bindings.Employees1.collectionModel.makeCurrent}", new Class[] { SelectionEvent.class }, new Object[] {
//                 selectionEvent });
            // Get the selected row (Use pie chart iterator name)
            Row selectedRow = (Row)  AdfmfJavaUtilities.evaluateELExpression("#{bindings.PieSlice.currentRow}"); // get the current selected row
            // Get any attribute from selected row
            System.out.println("Selected slice is is-" + selectedRow.getAttribute("name"));
        } catch (Exception e) {
            System.out.println( e ); 
        }
    }
    /**
     * 
     * @param objectName
     * @return
     */
    public PerfData incidentPerformanceReport(String objectName) {
        System.out.println("incidentTitles PERFORMANCE REPORT called, object:"+objectName );
        PerfData returnValue = new PerfData ();

//        ReportItem ri = new ReportItem();
//        ri.setId("0");
//        ri.setAttr1("Total Handle");
//        ri.setAttr2("Response to Resolve");
//        ri.setAttr3("Initial Resolve");
//        returnValue.add(ri);
        
        try {
            JSONObject queryObj = new JSONObject();

            queryObj.put("lookupName", "AcceleratorDashPerformance");
            
            JSONArray filterArray = new JSONArray();
            //
            queryObj.put("filters", filterArray);                  
            
            queryObj.put("limit", 20);  // CHANGE to DEFAULT_LIMIT IF WILL BE CALCULATING DATA! 
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
                    
                    returnValue.setTime1("Err");
                }
                
                if (obj != null) {
                    JSONArray rows = obj.getJSONArray("rows");
                    int fieldCount = obj.getJSONArray("columnNames").length();
                    System.out.println("Report Response(Performance) has N="+fieldCount+" columns and " + rows.length() + " rows");

                    int i;
                    for (i = 0; rows != null && i < rows.length(); i++) {
                    
                        JSONArray fields = (JSONArray) rows.get(i);
                        
                        String numberItem = fields.getString(0);
                        returnValue.setTime1(numberItem);
                        
                        numberItem = fields.getString(1);       
                        returnValue.setTime2(numberItem);
                        
                        numberItem = fields.getString(2);       
                        returnValue.setPercent(numberItem);
                
                    }
                }                
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
        
                
        return returnValue;
    }

    public void setTotalNumber(int _totalNumber) {
        int oldTotalNumber = this._totalNumber;
        this._totalNumber = _totalNumber;
        propertyChangeSupport.firePropertyChange("TotalNumber", oldTotalNumber, _totalNumber);
    }

    public int getTotalNumber() {
        System.out.println("totalNumber called = " + _totalNumber);
        return _totalNumber;
    }

    public void addPropertyChangeListener(PropertyChangeListener l) {
        propertyChangeSupport.addPropertyChangeListener(l);
    }

    public void removePropertyChangeListener(PropertyChangeListener l) {
        propertyChangeSupport.removePropertyChangeListener(l);
    }

    public static class PieSlice extends ReportItem {
        private String name;
        private double number;

        public void setName(String name) {
            this.name = name;
        }

        public String getName() {
            return name;
        }

        public void setNumber(double number) {
            this.number = number;
        }

        public double getNumber() {
            return number;
        }
    }
    
    public static class PerfData {
        private String time1; 
        private String time2;
        private String percent;

        public void setTime1(String time1) {
            this.time1 = time1;
        }

        public String getTime1() {
            return time1;
        }

        public void setTime2(String time2) {
            this.time2 = time2;
        }

        public String getTime2() {
            return time2;
        }

        public void setPercent(String percent) {
            this.percent = percent;
        }

        public String getPercent() {
            return percent;
        }

    }
}
