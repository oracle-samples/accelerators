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

package assets;

public class AssetSearchFilters {
    private Integer _productId;
    private String _product;
    private String _assetName;
    private String _assetSerialNumber;
    private String _contactFirstName;
    private String _contactLastName;
    private String _orgName;
    private String _assigned;
    
    public AssetSearchFilters() {
        super();
    }

    public AssetSearchFilters(AssetSearchFilters filters) {
        super();
        this._productId = filters._productId;
        this._product = filters._product;
        this._assetName = filters._assetName;
        this._assetSerialNumber = filters._assetSerialNumber;
        this._contactFirstName = filters._contactFirstName;
        this._contactLastName = filters._contactLastName;
        this._orgName = filters._orgName;
        this._assigned = filters._assigned;
    }

    public void setProductId(Integer _productId) {
        this._productId = _productId;
    }

    public Integer getProductId() {
        return _productId;
    }

    public void setProduct(String _product) {
        this._product = _product;
    }

    public String getProduct() {
        return _product;
    }
    
    public void setAssetName(String _assetName) {
        this._assetName = _assetName;
    }

    public String getAssetName() {
        return _assetName;
    }

    public void setAssetSerialNumber(String _assetSerialNumber) {
        this._assetSerialNumber = _assetSerialNumber;
    }

    public String getAssetSerialNumber() {
        return _assetSerialNumber;
    }

    public void setContactFirstName(String _contactFirstName) {
        this._contactFirstName = _contactFirstName;
    }

    public String getContactFirstName() {
        return _contactFirstName;
    }

    public void setContactLastName(String _contactLastName) {
        this._contactLastName = _contactLastName;
    }

    public String getContactLastName() {
        return _contactLastName;
    }

    public void setOrgName(String _orgName) {
        this._orgName = _orgName;
    }

    public String getOrgName() {
        return _orgName;
    }

    public void setAssigned(String _assigned) {
        this._assigned = _assigned;
    }

    public String getAssigned() {
        return _assigned;
    }
}
