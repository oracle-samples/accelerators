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

package contacts;

import java.util.HashMap;

import javax.el.ValueExpression;

import oracle.adfmf.bindings.dbf.AmxIteratorBinding;
import oracle.adfmf.framework.api.AdfmfJavaUtilities;

public class ContactSearchFiltersController {
    ContactSearchFilters _filters = new ContactSearchFilters();
    ContactSearchFilters _savedFilters = new ContactSearchFilters();
    private static String Login = (String) AdfmfJavaUtilities.evaluateELExpression("#{securityContext.userName}");
    
    public ContactSearchFiltersController() {
        super();
    }

    public void setFilters(ContactSearchFilters _filters) {
        this._filters = _filters;
    }

    public ContactSearchFilters getFilters() {
        return _filters;
    }
    
    public void setSavedFilters(ContactSearchFilters _savedFilters) {
        this._savedFilters = _savedFilters;
    }

    public ContactSearchFilters getSavedFilters() {
        return _savedFilters;
    }
    
    private HashMap<String, String> initDefaultFilters(){
        HashMap<String, String> searchCriteria = new HashMap<String, String>(); 
        
        //Add Default Search Filters
        _filters.setLogin(Login);
        searchCriteria.put("Login",  Login);
        return searchCriteria;
    }

    public HashMap<String, String> updateFilters() {
        HashMap<String, String> searchCriteria = new HashMap<String, String>(); 
        if(_filters.getFirstName()!= null && !_filters.getFirstName().isEmpty()){
            searchCriteria.put("First Name", _filters.getFirstName());
        }
        if(_filters.getLastName()!= null && !_filters.getLastName().isEmpty()){
            searchCriteria.put("Last Name", _filters.getLastName());
        }
        if(_filters.getPhone() != null && !_filters.getPhone().isEmpty()){
            searchCriteria.put("Phone", _filters.getPhone() );
        }
        if(_filters.getEmail() != null && !_filters.getEmail().isEmpty()){
            searchCriteria.put("Email", _filters.getEmail() );
        }
        if(_filters.getOrgName()!= null && !_filters.getOrgName().isEmpty()){
            searchCriteria.put("Org", _filters.getOrgName() );
        }

        if(_filters.getAssetSerialNumber()!= null && !_filters.getAssetSerialNumber().isEmpty()){
            searchCriteria.put("Serial #", _filters.getAssetSerialNumber());
        }
        if(searchCriteria.isEmpty()){
            searchCriteria = initDefaultFilters();
        }
        _savedFilters = new ContactSearchFilters(_filters);
        return searchCriteria;
    }
    
    public void resetFilters(){
        _filters = new ContactSearchFilters();
        refreshFilters();
    }
    
    public void cancelModifiedFilters(){
        _filters = new ContactSearchFilters(_savedFilters);
        refreshFilters();
    }
    
    private void refreshFilters() {
        ValueExpression iter = AdfmfJavaUtilities.getValueExpression("#{bindings.filtersIterator}", Object.class);
        AmxIteratorBinding iteratorBinding = (AmxIteratorBinding)iter.getValue(AdfmfJavaUtilities.getELContext());
        iteratorBinding.getIterator().refresh();
    }
}
