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

package incidents;

import java.text.SimpleDateFormat;

import java.util.HashMap;

import javax.el.ValueExpression;

import oracle.adfmf.bindings.dbf.AmxIteratorBinding;
import oracle.adfmf.framework.api.AdfmfJavaUtilities;

public class IncidentSearchFiltersController {
    IncidentSearchFilters _filters = new IncidentSearchFilters();
    IncidentSearchFilters _savedFilters = new IncidentSearchFilters();
    private static String Login = (String) AdfmfJavaUtilities.evaluateELExpression("#{securityContext.userName}");

    public IncidentSearchFiltersController() {
        super();
    }
    
    public void setFilters(IncidentSearchFilters _filters) {
        this._filters = _filters;
    }

    public IncidentSearchFilters getFilters() {
        return _filters;
    }
    
    public void setSavedFilters(IncidentSearchFilters _savedFilters) {
        this._savedFilters = _savedFilters;
    }

    public IncidentSearchFilters getSavedFilters() {
        return _savedFilters;
    }
    
    private HashMap<String, String> initDefaultFilters(){       
        HashMap<String, String> searchCriteria = new HashMap<String, String>(); 
        
        //Add Default Search Filters
        _filters.setAssigned(Login);
        _filters.setStatusTypeId(2);
        searchCriteria.put("Assigned",  Login);
        searchCriteria.put("Status Type (Not)", "2");
        return searchCriteria;
    }
    
    
    public HashMap<String, String> updateFilters(){
        HashMap<String, String> searchCriteria = new HashMap<String, String>(); 
        if(_filters.getSubject() != null && !_filters.getSubject().isEmpty()){
            searchCriteria.put("Subject", _filters.getSubject());
        }
        if(_filters.getAssigned() != null && !_filters.getAssigned().isEmpty()){
            searchCriteria.put("Assigned", _filters.getAssigned());
        }
        if(_filters.getStatusTypeId() != null && _filters.getStatusTypeId()!= 0){
            searchCriteria.put("Status Type (Not)", _filters.getStatusTypeId().toString());
        }
        if(_filters.getContactFirstName() != null && !_filters.getContactFirstName().isEmpty()){
            searchCriteria.put("Contact First Name", _filters.getContactFirstName());
        }
        if(_filters.getContactLastName() != null && !_filters.getContactLastName().isEmpty()){
            searchCriteria.put("Contact Last Name", _filters.getContactLastName());
        }
        if(_filters.getStatusId() != null && _filters.getStatusId()!= 0){
            searchCriteria.put("Status", _filters.getStatusId().toString());
        }
        if(_filters.getRefNo() != null && !_filters.getRefNo().isEmpty()){
            searchCriteria.put("Ref #", _filters.getRefNo());
        }
        if(_filters.getUpdatedSince() != null){
            searchCriteria.put("Updated Since", new SimpleDateFormat("yyyy-MM-dd").format(_filters.getUpdatedSince()));
        }
        if(_filters.getCreatedSince() != null){
            searchCriteria.put("Created Since", new SimpleDateFormat("yyyy-MM-dd").format(_filters.getCreatedSince()));
        }
        if(_filters.getAssetSerialNumber()!= null && !_filters.getAssetSerialNumber().isEmpty()){
            searchCriteria.put("Serial #", _filters.getAssetSerialNumber());
        }
        if(searchCriteria.isEmpty()){
            searchCriteria = initDefaultFilters();
        }
        _savedFilters = new IncidentSearchFilters(_filters);
        return searchCriteria;
    }
    
    public void resetFilters(){
        _filters = new IncidentSearchFilters();
        refreshFilters();  
    }
    
    public void cancelModifiedFilters(){
        _filters = new IncidentSearchFilters(_savedFilters);
        refreshFilters();
    }

    public static String getLogin() {
        return Login;
    }
    
    private void refreshFilters() {
        ValueExpression iter = AdfmfJavaUtilities.getValueExpression("#{bindings.filtersIterator}", Object.class);
        AmxIteratorBinding iteratorBinding = (AmxIteratorBinding)iter.getValue(AdfmfJavaUtilities.getELContext());
        iteratorBinding.getIterator().refresh();
    }
}
