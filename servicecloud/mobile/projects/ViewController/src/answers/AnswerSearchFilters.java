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

public class AnswerSearchFilters {
    private Integer _productId;
    private String _product;
    private Integer _categoryId;
    private String _category;
    
    private String _phrases;
    private String _similarPhrases;
    private String _exactSearch;

    public AnswerSearchFilters() {
        super();
    }

    public AnswerSearchFilters(AnswerSearchFilters filters) {
        super();
        this._productId = filters._productId;
        this._product = filters._product;
        this._categoryId = filters._categoryId;
        this._category = filters._category;
        this._phrases = filters._phrases;
        this._similarPhrases = filters._similarPhrases;
        this._exactSearch = filters._exactSearch;
    }

    public void setProduct(String _product) {
        this._product = _product;
    }

    public String getProduct() {
        return _product;
    }

    public void setCategory(String _category) {
        this._category = _category;
    }

    public String getCategory() {
        return _category;
    }

    public void setPhrases(String _phrases) {
        this._phrases = _phrases;
    }

    public String getPhrases() {
        return _phrases;
    }

    public void setSimilarPhrases(String _similarPhrases) {
        this._similarPhrases = _similarPhrases;
    }

    public String getSimilarPhrases() {
        return _similarPhrases;
    }

    public void setExactSearch(String _exactSearch) {
        this._exactSearch = _exactSearch;
    }

    public String getExactSearch() {
        return _exactSearch;
    }

    public void setProductId(Integer _productId) {
        this._productId = _productId;
    }

    public Integer getProductId() {
        return _productId;
    }

    public void setCategoryId(Integer _categoryId) {
        this._categoryId = _categoryId;
    }

    public Integer getCategoryId() {
        return _categoryId;
    }
}
