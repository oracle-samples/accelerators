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
 *  SHA1: $Id: 87e2d983769e7171042856f06adacd7d6aa9b248 $
 * *********************************************************************************************
 *  File: OrganizationSearchFiltersController.java
 * *********************************************************************************************/

package organizations;

import java.util.HashMap;

import javax.el.ValueExpression;

import oracle.adfmf.bindings.dbf.AmxIteratorBinding;
import oracle.adfmf.framework.api.AdfmfJavaUtilities;

public class OrganizationSearchFiltersController {
    OrganizationSearchFilters _filters = new OrganizationSearchFilters();
    OrganizationSearchFilters _savedFilters = new OrganizationSearchFilters();
    private static String Login = (String) AdfmfJavaUtilities.evaluateELExpression("#{securityContext.userName}");
    
    public OrganizationSearchFiltersController() {
        super();
    }

    public void setFilters(OrganizationSearchFilters _filters) {
        this._filters = _filters;
    }

    public OrganizationSearchFilters getFilters() {
        return _filters;
    }
    
    public void setSavedFilters(OrganizationSearchFilters _savedFilters) {
        this._savedFilters = _savedFilters;
    }

    public OrganizationSearchFilters getSavedFilters() {
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
        if(_filters.getName()!= null && !_filters.getName().isEmpty()){
            searchCriteria.put("Name", _filters.getName());
        }
        if(_filters.getIndustryId()!= null && _filters.getIndustryId()!=0 ){    
            searchCriteria.put("Industry", _filters.getIndustryId().toString());
        }
        if(searchCriteria.isEmpty()){
            searchCriteria = initDefaultFilters();
        }
        _savedFilters = new OrganizationSearchFilters(_filters);
        return searchCriteria;
    }
    
    public void resetFilters(){
        _filters = new OrganizationSearchFilters();
        refreshFilters();
    }
    
    public void cancelModifiedFilters(){
        _filters = new OrganizationSearchFilters(_savedFilters);
        refreshFilters();
    }
    
    private void refreshFilters() {
        ValueExpression iter = AdfmfJavaUtilities.getValueExpression("#{bindings.filtersIterator}", Object.class);
        AmxIteratorBinding iteratorBinding = (AmxIteratorBinding)iter.getValue(AdfmfJavaUtilities.getELContext());
        iteratorBinding.getIterator().refresh();
    }
}
