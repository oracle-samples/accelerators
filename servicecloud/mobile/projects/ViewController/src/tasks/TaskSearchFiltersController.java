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
 *  SHA1: $Id: 7bf2507725762d73247f8b888a43a1ac0c3c6e42 $
 * *********************************************************************************************
 *  File: TaskSearchFiltersController.java
 * *********************************************************************************************/

package tasks;

import java.text.SimpleDateFormat;

import java.util.Date;
import java.util.HashMap;

import java.util.concurrent.TimeUnit;

import javax.el.ValueExpression;

import oracle.adfmf.bindings.dbf.AmxIteratorBinding;
import oracle.adfmf.framework.api.AdfmfJavaUtilities;

import organizations.OrganizationSearchFilters;

public class TaskSearchFiltersController {
    TaskSearchFilters _filters = new TaskSearchFilters();
    TaskSearchFilters _savedFilters = new TaskSearchFilters();
    private static String Login = (String) AdfmfJavaUtilities.evaluateELExpression("#{securityContext.userName}");
    
    public TaskSearchFiltersController() {
        super();
    }

    public void setFilters(TaskSearchFilters _filters) {
        this._filters = _filters;
    }

    public TaskSearchFilters getFilters() {
        return _filters;
    }

    public void setSavedFilters(TaskSearchFilters _savedFilters) {
        this._savedFilters = _savedFilters;
    }

    public TaskSearchFilters getSavedFilters() {
        return _savedFilters;
    }
    
    public HashMap<String, String> updateFilters() {
        HashMap<String, String> searchCriteria = new HashMap<String, String>(); 
        if(_filters.getName()!= null && !_filters.getName().isEmpty()){
            searchCriteria.put("Name", _filters.getName());
        }
        if( _filters.getAssigneeFirstName() != null ){
            if ("".equals(_filters.getAssigneeFirstName()))
                System.out.println("Setting Assigned First to _empty_, in tasks search.");
            searchCriteria.put("AssigneeFN", _filters.getAssigneeFirstName());
        }
        if( _filters.getAssigneeLastName() != null ){
            if ("".equals(_filters.getAssigneeLastName()))
                System.out.println("Setting Assigned Last to _empty_, in tasks search.");
            searchCriteria.put("AssigneeLN", _filters.getAssigneeLastName());
        }
        if(_filters.getStatusId() != null && _filters.getStatusId()!= 0){
            searchCriteria.put("Status", _filters.getStatusId().toString());
        }
        if(_filters.getDueDate()!=null){
            String ddfmtd = new SimpleDateFormat("yyyy-MM-dd").format(
                    new Date(_filters.getDueDate().getTime() + TimeUnit.DAYS.toMillis(1)) );
            
            //analytics bug LTEQ doesn't work System.out.println("updateFilters in Task, setting DueDate to: "+ddfmtd);
            searchCriteria.put("Due Date", ddfmtd);
        }
        
        if(searchCriteria.isEmpty()){
            searchCriteria = initDefaultFilters();
        }
        _savedFilters = new TaskSearchFilters(_filters);

        return searchCriteria;
    }
    
    public void resetFilters(){
        _filters = new TaskSearchFilters();
        refreshFilters();        
    }
    
    public void cancelModifiedFilters(){
        _filters = new TaskSearchFilters(_savedFilters);
        refreshFilters();
    }

    private HashMap<String, String> initDefaultFilters() {
        HashMap<String, String> searchCriteria = new HashMap<String, String>(); 
        
        //Add Default Search Filters
        _filters.setLogin(Login);
        searchCriteria.put("Assigned",  Login);
        return searchCriteria;
    }
    
    private void refreshFilters() {
        ValueExpression iter = AdfmfJavaUtilities.getValueExpression("#{bindings.filtersIterator}", Object.class);
        AmxIteratorBinding iteratorBinding = (AmxIteratorBinding)iter.getValue(AdfmfJavaUtilities.getELContext());
        iteratorBinding.getIterator().refresh();
    }
}
