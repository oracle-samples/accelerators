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
 *  SHA1: $Id: 71386f427b336c2537ae8f78db8fd75be1bcd7d0 $
 * *********************************************************************************************
 *  File: ReportController.java
 * *********************************************************************************************/

package report;

import contacts.ContactSearchFiltersController;
import incidents.IncidentSearchFilters;
import incidents.IncidentSearchFiltersController;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

import java.util.Map;

import javax.el.ValueExpression;

import oracle.adfmf.amx.event.RangeChangeEvent;
import oracle.adfmf.amx.event.RangeChangeListener;
import oracle.adfmf.bindings.dbf.AmxIteratorBinding;
import oracle.adfmf.framework.api.AdfmfContainerUtilities;
import oracle.adfmf.framework.api.AdfmfJavaUtilities;

import rest_adapter.RestAdapter;

import oracle.adfmf.json.JSONArray;
import oracle.adfmf.json.JSONObject;

import organizations.OrganizationSearchFiltersController;

import tasks.TaskSearchFiltersController;


public class ReportController implements RangeChangeListener {

    private static final String REPORT_URL_CONNECTION =
        (String) AdfmfJavaUtilities.evaluateELExpression("#{applicationScope.QUERY_URL_CONNECTION}");
    private static final String REPORT_GET_URI =
        (String) AdfmfJavaUtilities.evaluateELExpression("#{applicationScope.QUERY_REPORT_URI}");
    private static final int DEFAULT_LIMIT = 15;

    private List _reportItems = null;
    private String _reportName = null;
    private int _offset = 0;
    private String _filterName = null;
    private String _filterValue = null;
    private String _dataControl = null;

    private Map<String, String> _searchCriteria = null;
    private boolean _isSearch;

    private String _dashPieSelection = null;
    private HashMap<String, String> searchCriteria;

    public ReportController() {
        //TODO
        System.out.println("ReportController");
    }


    public void initReport(String reportName, String dataControl, String filterName, String filterValue) {

        invalidateCache();

        this._reportName = reportName;
        this._dataControl = dataControl;
        this._filterName = filterName;
        this._filterValue = filterValue;
        this._searchCriteria = null;
        this._isSearch = false;


        System.out.println("initReport [" + this._reportName + "]," + reportName + " +filter: " + filterName );

    }


    public void initReport(String reportName, String dataControl) {
        invalidateCache();

/*
        if (checkNotification() == true) {
            return;
        }
*/

        this._reportName = reportName;
        this._dataControl = dataControl;
        this._filterName = null;
        this._filterValue = null;
        this._searchCriteria = null;
        this._isSearch = false;
        
        System.out.println("initReport [" + this._reportName + "]," + reportName + " ctrl:" + dataControl);
        
        String dpSelFilter = (String) AdfmfJavaUtilities.evaluateELExpression("#{applicationScope.pieItemFilter}");
        if (null != dpSelFilter && !dpSelFilter.equals("")) {
            _dashPieSelection = dpSelFilter;
            System.out.println("initReport pieItemFilter specified: [" + _dashPieSelection + "],");
            AdfmfJavaUtilities.setELValue("#{applicationScope.pieItemFilter}", null);
        }
    }

    public void initSearchReport(String reportName, String dataControl) {

        invalidateCache();

        this._isSearch = true;
        this._reportName = reportName;
        this._dataControl = dataControl;

        System.out.println("initSearchReport [" + this._reportName + "]," + reportName);

    }

    /**
     * For the first release, just invalidate the list view cache everytime the list view is fetched
     */
    private void invalidateCache() {
        this._reportItems = null;
        this._offset = 0;
    }

    /**
     * getReport()
     */
    public ReportItem[] getReport() {
        // set the pageFlowScope.ScannerPage for barcode scanner js check
        AdfmfJavaUtilities.setELValue("#{pageFlowScope.ScannerPage}", "NotAssetQuickSearch");
        //TODO
        System.out.println("getReport");

        Object obj = AdfmfJavaUtilities.getELValue("#{applicationScope.myAttachmentsRefresh}");
        String _specialRefreshId = null;
        if ( (obj instanceof String && !obj.equals("false")) )  {
            _specialRefreshId = (String)obj;
            AdfmfJavaUtilities.setELValue("#{applicationScope.myAttachmentsRefresh}", null);
            this.initReport(_reportName, _dataControl, _filterName, _specialRefreshId);
        }

        if (this._reportItems == null || _specialRefreshId != null) {
            // Initialise & load initial list from the external source
            this._reportItems = new ArrayList();
            this._offset = 0;
            loadData(this._reportName, DEFAULT_LIMIT);
        }
        ReportItem[] results = (ReportItem[]) _reportItems.toArray(new ReportItem[_reportItems.size()]);
        if (results.length == 0) {
            AdfmfJavaUtilities.setELValue("#{pageFlowScope.isNoDataFound"+this._dataControl+"}", true);
        } else {
            AdfmfJavaUtilities.setELValue("#{pageFlowScope.isNoDataFound"+this._dataControl+"}", false);
        }
        return results;

    }

    /**
     * getIncidentFromWS
     * -   get incident from server based on the incident seq in online mode
     * -   using RestServiceAdpter GET method
     */
    private ArrayList<ReportItem> loadData(String reportName, int limit) {

        ArrayList<ReportItem> loadedData = new ArrayList<ReportItem>();

        try {

            JSONObject queryObj = new JSONObject();

            queryObj.put("lookupName", reportName);
            if (null != this._searchCriteria) {
                System.out.println("setSearch");
                queryObj.put("filters", getFiltersArray());
            } else if (null !=
                       this._filterName) { // if a filter is present, add it to the request
                queryObj.put("filters",
                             new JSONObject().put("name", this._filterName).put("values",
                                                                                new JSONArray().put(this._filterValue)));
            } else {
                queryObj.put("filters", new JSONArray());
            }
            queryObj.put("limit", limit);
            queryObj.put("offset", this._offset);
            this._offset += limit;

            String response = RestAdapter.doPOST(REPORT_URL_CONNECTION, REPORT_GET_URI, queryObj.toString());


            if (response != null) {
                JSONObject obj = new JSONObject(response);
                JSONArray rows = obj.getJSONArray("rows");
                int fieldCount = obj.getJSONArray("columnNames").length();

                for (int i = 0; i < rows.length(); i++) {

                    JSONArray fields = (JSONArray) rows.get(i);
                    ReportItem item = new ReportItem();

                    for (int j = 0; j < fieldCount; j++) {
                        item.setAttr(j, fields.getString(j));
                    }

                    loadedData.add(item);
                    _reportItems.add(item);

                }
            }

        } catch (Exception e) {
            e.printStackTrace();
        }


        return loadedData;
    }


    public JSONArray getFiltersArray() {
        JSONArray filtersArray = new JSONArray();
        try {
            for (Map.Entry<String, String> filterEntry : this._searchCriteria.entrySet()) {
                switch (filterEntry.getKey()) {
                    // Search for Answer Category/Product ID in 6 different levels of hierarchy (the max number of levels is 6)
                    case "map_prod_hierarchy":
                    case "map_cat_hierarchy":
                        ArrayList<String> idList = new ArrayList<String>();
                        for (int i = 1; i <= 6; i++) {
                            idList.add(i + "." + filterEntry.getValue());
                        }
                        filtersArray.put(new JSONObject().put("name", filterEntry.getKey()).put("values",
                                                                                                new JSONArray(idList)));
                        break;
                    default:    
                        filtersArray.put(new JSONObject().put("name", filterEntry.getKey()).put("values",
                                                                                                new JSONArray().put(filterEntry.getValue())));
                }
            }
        } catch (Exception ex) {

        }
        return filtersArray;
    }

    public boolean checkNotification() {
        Object hasNotification = AdfmfJavaUtilities.getELValue("#{applicationScope.isNotification}");

        if (hasNotification != null) {
            String hasNotificationString = (String) hasNotification;

            if (hasNotificationString.equals("true")) {
                this._reportName = null;
                this._dataControl = null;
                navigateToNotified();
                return true;
            }
        }
        return false;
    }

    public void navigateToNotified() {
        Object notificationId = AdfmfJavaUtilities.getELValue("#{applicationScope.notificationId}");
        if (notificationId != null) {
            ValueExpression ve = AdfmfJavaUtilities.getValueExpression("#{pageFlowScope.incidentId}", String.class);
            ve.setValue(AdfmfJavaUtilities.getELContext(), notificationId);

        }

        System.out.println("invoke navigation");
        ValueExpression ve = AdfmfJavaUtilities.getValueExpression("#{applicationScope.isNotification}", String.class);
        ve.setValue(AdfmfJavaUtilities.getELContext(), "false");

        AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities.getFeatureId(),
                                                                  "adf.mf.api.amx.doNavigation", new Object[] {
                                                                  "goToDetail" });

    }

    @Override
    public void rangeChange(RangeChangeEvent rangeChangeEvent) {

        //TODO
        System.out.println("rangeChange");

        List newRows = null;
        if (rangeChangeEvent.isDataExhausted()) {
            newRows = loadData(this._reportName, rangeChangeEvent.getFetchSize());
            AdfmfJavaUtilities.addDataControlProviders(this._dataControl, rangeChangeEvent.getProviderKey(),
                                                       rangeChangeEvent.getKeyAttribute(), newRows,
                                                       newRows.size() >= DEFAULT_LIMIT);
        }
    }

    public void doSearch(Object filters) {

        this._searchCriteria = this.setSearchFilters(filters);

        invalidateCache();

        this._isSearch = true;
        //checkSearchCriteria();

        //Refresh List
        ValueExpression iter = AdfmfJavaUtilities.getValueExpression("#{bindings.reportIterator}", Object.class);
        AmxIteratorBinding iteratorBinding = (AmxIteratorBinding) iter.getValue(AdfmfJavaUtilities.getELContext());
        iteratorBinding.getIterator().refresh();
    }

    private HashMap<String, String> setSearchFilters(Object filters) {
        HashMap<String, String> searchCriteria = new HashMap<String, String>();
        switch (this._reportName) {
        case "AcceleratorIncidentList":
            IncidentSearchFiltersController incidentFilters = (IncidentSearchFiltersController) filters;
            incidentFilters.getFilters().setAssigned("");
            incidentFilters.getFilters().setStatusTypeId(0);
            searchCriteria = incidentFilters.updateFilters();
            break;
        case "AcceleratorContactList":
            ContactSearchFiltersController contactFilters = (ContactSearchFiltersController) filters;
            searchCriteria = contactFilters.updateFilters();
            break;
        case "AcceleratorOrganizationList":
            OrganizationSearchFiltersController orgFilters = (OrganizationSearchFiltersController) filters;
            searchCriteria = orgFilters.updateFilters();
            break;
        case "AcceleratorTaskList":
            TaskSearchFiltersController taskFilCon = (TaskSearchFiltersController) filters;
            searchCriteria = taskFilCon.updateFilters();
            break;
        }
        return searchCriteria;
    }

    public void initSearchCriteria(Object filters) {
        
        switch (this._reportName) {
        case "AcceleratorIncidentList":
            IncidentSearchFiltersController incidentFilters = (IncidentSearchFiltersController) filters;
            
            if (_dashPieSelection != null) {
                System.out.println("_dashPieSelection  " + _dashPieSelection);
                
                IncidentSearchFilters pieFilters = new IncidentSearchFilters();
                pieFilters.setStatusId(Integer.valueOf(_dashPieSelection));
                pieFilters.setAssigned(IncidentSearchFiltersController.getLogin());
                
                incidentFilters.setFilters(pieFilters);
            }

            this._searchCriteria = incidentFilters.updateFilters();
            break;
        case "AcceleratorContactList":
            ContactSearchFiltersController contactFilters = (ContactSearchFiltersController) filters;
            this._searchCriteria = contactFilters.updateFilters();
            break;
        case "AcceleratorOrganizationList":
            OrganizationSearchFiltersController orgFilters = (OrganizationSearchFiltersController) filters;
            this._searchCriteria = orgFilters.updateFilters();
            break;
        case "AcceleratorTaskList":
            TaskSearchFiltersController taskFilCon = (TaskSearchFiltersController) filters;
            this._searchCriteria = taskFilCon.updateFilters();
            break;
        }
    }

    public void doReset(Object filters) {
        switch (this._reportName) {
        case "AcceleratorIncidentList":
            IncidentSearchFiltersController incidentFilters = (IncidentSearchFiltersController) filters;
            incidentFilters.resetFilters();
            break;
        case "AcceleratorContactList":
            ContactSearchFiltersController contactFilters = (ContactSearchFiltersController) filters;
            contactFilters.resetFilters();
            break;
        case "AcceleratorOrganizationList":
            OrganizationSearchFiltersController orgFilters = (OrganizationSearchFiltersController) filters;
            orgFilters.resetFilters();
            break;
        case "AcceleratorTaskList":
            TaskSearchFiltersController taskFilCon = (TaskSearchFiltersController) filters;
            taskFilCon.resetFilters();
            break;
        }
    }

    public void doCancel(Object filters) {
        switch (this._reportName) {
        case "AcceleratorIncidentList":
            IncidentSearchFiltersController incidentFilters = (IncidentSearchFiltersController) filters;
            incidentFilters.cancelModifiedFilters();
            break;
        case "AcceleratorContactList":
            ContactSearchFiltersController contactFilters = (ContactSearchFiltersController) filters;
            contactFilters.cancelModifiedFilters();
            break;
        case "AcceleratorOrganizationList":
            OrganizationSearchFiltersController orgFilters = (OrganizationSearchFiltersController) filters;
            orgFilters.cancelModifiedFilters();
            break;
        case "AcceleratorTaskList":
            TaskSearchFiltersController taskFilCon = (TaskSearchFiltersController) filters;
            taskFilCon.cancelModifiedFilters();
            break;
        }
    }
}

