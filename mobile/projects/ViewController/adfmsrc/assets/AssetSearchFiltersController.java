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
 *  date: Tue Aug 23 16:35:48 PDT 2016

 *  revision: rnw-16-8-fixes-release-01
 *  SHA1: $Id: 72b26f6e8fe1c7493f3c3a410427fa4e1e0c95fc $
 * *********************************************************************************************
 *  File: AssetSearchFiltersController.java
 * *********************************************************************************************/

package assets;

import java.util.HashMap;
import javax.el.ValueExpression;
import oracle.adfmf.bindings.dbf.AmxIteratorBinding;
import oracle.adfmf.framework.api.AdfmfJavaUtilities;

public class AssetSearchFiltersController {
    AssetSearchFilters _filters =  new AssetSearchFilters();
    AssetSearchFilters _savedFilters =  new AssetSearchFilters();
    private static String Login = (String) AdfmfJavaUtilities.evaluateELExpression("#{securityContext.userName}");
    
    public AssetSearchFiltersController() {
        super();
    }

    public void setFilters(AssetSearchFilters _filters) {
        this._filters = _filters;
    }

    public AssetSearchFilters getFilters() {
        return _filters;
    }

    public void setSavedFilters(AssetSearchFilters _savedFilters) {
        this._savedFilters = _savedFilters;
    }

    public AssetSearchFilters getSavedFilters() {
        return _savedFilters;
    }
    
    private HashMap<String, String> initDefaultFilters(){
        HashMap<String, String> searchCriteria = new HashMap<String, String>(); 

        //Add Default Search Filters
        _filters.setAssigned(Login);
        searchCriteria.put("Assigned",  Login);
        return searchCriteria;
    }
    
    public HashMap<String, String> updateFilters(){
        HashMap<String, String> searchCriteria = new HashMap<String, String>();
        if(_filters.getProductId() != null && _filters.getProductId() != 0) {
            searchCriteria.put("Product", _filters.getProductId().toString());
        }
        if(_filters.getAssetName() != null && !_filters.getAssetName().isEmpty()) {
            searchCriteria.put("Name", _filters.getAssetName());
        }
        if(_filters.getAssetSerialNumber() != null && !_filters.getAssetSerialNumber().isEmpty()) {
            searchCriteria.put("Serial #", _filters.getAssetSerialNumber());
        }
        if(_filters.getOrgName() != null && !_filters.getOrgName().isEmpty()) {
            searchCriteria.put("Organization", _filters.getOrgName());
        }
        if(_filters.getContactFirstName() != null && !_filters.getContactFirstName().isEmpty()) {
            searchCriteria.put("Contact First Name", _filters.getContactFirstName());
        }
        if(_filters.getContactLastName() != null && !_filters.getContactLastName().isEmpty()) {
            searchCriteria.put("Contact Last Name", _filters.getContactLastName());
        }
        if(_filters.getAssigned() != null && !_filters.getAssigned().isEmpty()){
            searchCriteria.put("Assigned", _filters.getAssigned());
        }
        if(searchCriteria.isEmpty()){
            searchCriteria = initDefaultFilters();
        }
        _savedFilters = new AssetSearchFilters(_filters);
        return searchCriteria;
    }
    
    public void resetFilters(){
        _filters = new AssetSearchFilters();
        cleanSeachForSelectCache();
        refreshFilters();   
    }
    
    public void cancelModifiedFilters(){
        _filters = new AssetSearchFilters(_savedFilters);
        refreshFilters();
    }
    
    public void refreshFilters() {
        ValueExpression iter = AdfmfJavaUtilities.getValueExpression("#{bindings.filtersIterator}", Object.class);
        AmxIteratorBinding iteratorBinding = (AmxIteratorBinding)iter.getValue(AdfmfJavaUtilities.getELContext());
        iteratorBinding.getIterator().refresh();
    }
    
    public void fetchAndSetIDsFromScope() {
        Object productCatalogueId = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.productCatalogueId}");
        Object productCatalogueName = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.productCatalogueName}");
        if(productCatalogueId != null){
            _filters.setProductId(Integer.parseInt((String)productCatalogueId));
            _filters.setProduct((String)productCatalogueName);
            AdfmfJavaUtilities.setELValue("#{pageFlowScope.productCatalogueId}", null);
            AdfmfJavaUtilities.setELValue("#{pageFlowScope.productCatalogueName}", null);
        }
    }
    
    private void cleanSeachForSelectCache() {
        // Clear search keys on the search bar
        AdfmfJavaUtilities.setELValue("#{bindings.searchValue.inputValue}", "");
        
        // Clear cached search results
        ValueExpression iterOfProduct = AdfmfJavaUtilities.getValueExpression("#{bindings.searchSelectionFromDBIterator}", Object.class);
        AmxIteratorBinding iteratorBindingOfProduct = (AmxIteratorBinding)iterOfProduct.getValue(AdfmfJavaUtilities.getELContext());
        iteratorBindingOfProduct.refresh();
    }
}
