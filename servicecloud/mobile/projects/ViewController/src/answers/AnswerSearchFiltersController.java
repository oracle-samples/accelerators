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

package answers;

import java.util.HashMap;

import javax.el.ValueExpression;
import oracle.adfmf.bindings.dbf.AmxIteratorBinding;
import oracle.adfmf.framework.api.AdfmfJavaUtilities;

public class AnswerSearchFiltersController {
    AnswerSearchFilters _filters = new AnswerSearchFilters();
    AnswerSearchFilters _savedFilters = new AnswerSearchFilters();
    
    public AnswerSearchFiltersController() {
        super();
    }

    public void setFilters(AnswerSearchFilters _filters) {
        this._filters = _filters;
    }

    public AnswerSearchFilters getFilters() {
        return _filters;
    }

    public void setSavedFilters(AnswerSearchFilters _savedFilters) {
        this._savedFilters = _savedFilters;
    }

    public AnswerSearchFilters getSavedFilters() {
        return _savedFilters;
    }
    
    private HashMap<String, String> initDefaultFilters(){
        HashMap<String, String> searchCriteria = new HashMap<String, String>(); 
        return searchCriteria;
    }
    
    public HashMap<String, String> updateFilters(){
        HashMap<String, String> searchCriteria = new HashMap<String, String>(); 
        if(_filters.getProductId() != null && _filters.getProductId() != 0) {
            searchCriteria.put("map_prod_hierarchy", _filters.getProductId().toString());
        }
        if(_filters.getCategoryId() != null && _filters.getCategoryId() != 0) {
            searchCriteria.put("map_cat_hierarchy", _filters.getCategoryId().toString());
        }
        if(_filters.getPhrases() != null && !_filters.getPhrases().isEmpty()) {
            searchCriteria.put("search_nl", _filters.getPhrases());
        }
        if(_filters.getSimilarPhrases() != null && !_filters.getSimilarPhrases().isEmpty()) {
            searchCriteria.put("search_fnl", _filters.getSimilarPhrases());
        }
        if(_filters.getExactSearch() != null && !_filters.getExactSearch().isEmpty()) {
            searchCriteria.put("search_ex", _filters.getExactSearch());
        }
        if(searchCriteria.isEmpty()){
            searchCriteria = initDefaultFilters();
        }
        _savedFilters = new AnswerSearchFilters(_filters);
        return searchCriteria;
    }
    
    public void resetFilters(){
        _filters = new AnswerSearchFilters();
        cleanSeachForSelectCache();
        refreshFilters();   
    }
    
    public void cancelModifiedFilters(){
        _filters = new AnswerSearchFilters(_savedFilters);
        refreshFilters();
    }
    
    public void fetchAndSetIDsFromScope() {
        Object productId = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.productId}");
        Object productName = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.productName}");
        if(productId != null){
            _filters.setProductId(Integer.parseInt((String)productId));
            _filters.setProduct((String)productName);
            AdfmfJavaUtilities.setELValue("#{pageFlowScope.productId}", null);
            AdfmfJavaUtilities.setELValue("#{pageFlowScope.productName}", null);
        }
        Object categoryId = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.categoryId}");
        Object categoryName = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.categoryName}");
        if(categoryId != null){
            _filters.setCategoryId(Integer.parseInt((String)categoryId));
            _filters.setCategory((String)categoryName);
            AdfmfJavaUtilities.setELValue("#{pageFlowScope.categoryId}", null);
            AdfmfJavaUtilities.setELValue("#{pageFlowScope.categoryName}", null);
        }
    }
    
    public void refreshFilters() {
        ValueExpression iter = AdfmfJavaUtilities.getValueExpression("#{bindings.filtersIterator}", Object.class);
        AmxIteratorBinding iteratorBinding = (AmxIteratorBinding)iter.getValue(AdfmfJavaUtilities.getELContext());
        iteratorBinding.getIterator().refresh();
    }
    
    private void cleanSeachForSelectCache() {
        // Clear search keys on the search bar
        AdfmfJavaUtilities.setELValue("#{bindings.searchValueOfProduct.inputValue}", "");
        AdfmfJavaUtilities.setELValue("#{bindings.searchValueOfCategory.inputValue}", "");
        
        // Clear cached search results
        ValueExpression iterOfProduct = AdfmfJavaUtilities.getValueExpression("#{bindings.searchProductSelectionFromDBIterator}", Object.class);
        AmxIteratorBinding iteratorBindingOfProduct = (AmxIteratorBinding)iterOfProduct.getValue(AdfmfJavaUtilities.getELContext());
        iteratorBindingOfProduct.refresh();
        
        ValueExpression iterOfCategory = AdfmfJavaUtilities.getValueExpression("#{bindings.searchCategorySelectionFromDBIterator}", Object.class);
        AmxIteratorBinding iteratorBindingOfCategory = (AmxIteratorBinding)iterOfCategory.getValue(AdfmfJavaUtilities.getELContext());
        iteratorBindingOfCategory.refresh();
    }
}
