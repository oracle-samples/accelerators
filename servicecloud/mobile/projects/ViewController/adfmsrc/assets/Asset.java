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

import java.util.Date;

import oracle.adfmf.framework.api.AdfmfJavaUtilities;

public class Asset {
    private Integer _id;
    private String _name;
    private String _description;
    private Integer _statusId;
    private String _status;
    private Integer _contactId;
    private String _contact;
    private Integer _organizationId;
    private String _organization;
    private Integer _productId;    
    private String _product;
    private Date _purchasedDate;
    private String _purchasedDateString;
    private Date _installedDate;
    private String _installedDateString;
    private Date _retiredDate;
    private String _retiredDateString;
    private String _priceCurrency;
    private Integer _priceCurrencyId;
    private Double _priceValue;
    private String _assetSerialNumber;
    
    public void setId(Integer _id) {
        this._id = _id;
    }

    public Integer getId() {
        return _id;
    }

    public void setName(String _name) {
        this._name = _name;
    }

    public String getName() {
        return _name;
    }

    public void setDescription(String _description) {
        this._description = _description;
    }

    public String getDescription() {
        return _description;
    }

    public void setStatusId(Integer _statusId) {
        this._statusId = _statusId;
    }

    public Integer getStatusId() {
        return _statusId;
    }

    public void setStatus(String _status) {
        this._status = _status;
    }

    public String getStatus() {
        return _status;
    }

    public void setContactId(Integer _contactId) {
        this._contactId = _contactId;
    }

    public Integer getContactId() {
        return _contactId;
    }

    public void setContact(String _contact) {
        this._contact = _contact;
    }

    public String getContact() {
        return _contact;
    }

    public void setOrganizationId(Integer _organizationId) {
        this._organizationId = _organizationId;
    }

    public Integer getOrganizationId() {
        return _organizationId;
    }

    public void setOrganization(String _organization) {
        this._organization = _organization;
    }

    public String getOrganization() {
        return _organization;
    }

    public void setProductId(Integer _productId) {
        if (_productId == 0) 
            _productId = null;
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

    public Asset() {
        super();
    }

    public void setPurchasedDate(Date _purchasedDate) {
        this._purchasedDate = _purchasedDate;
    }

    public Date getPurchasedDate() {
        return _purchasedDate;
    }

    public void setPurchasedDateString(String _purchasedDateString) {
        this._purchasedDateString = _purchasedDateString;
    }

    public String getPurchasedDateString() {
        return _purchasedDateString;
    }

    public void setInstalledDate(Date _installedDate) {
        this._installedDate = _installedDate;
    }

    public Date getInstalledDate() {
        return _installedDate;
    }

    public void setInstalledDateString(String _installedDateString) {
        this._installedDateString = _installedDateString;
    }

    public String getInstalledDateString() {
        return _installedDateString;
    }

    public void setRetiredDate(Date _retiredDate) {
        this._retiredDate = _retiredDate;
    }

    public Date getRetiredDate() {
        return _retiredDate;
    }

    public void setRetiredDateString(String _retiredDateString) {
        this._retiredDateString = _retiredDateString;
    }

    public String getRetiredDateString() {
        return _retiredDateString;
    }

    public void setPriceCurrency(String _priceCurrency) {
        this._priceCurrency = _priceCurrency;
    }

    public String getPriceCurrency() {
        switch (_priceCurrency) {
        case "USD":
            AdfmfJavaUtilities.setELValue("#{pageFlowScope.currencySymbol}", '$');
            break;
        case "GBP":
            AdfmfJavaUtilities.setELValue("#{pageFlowScope.currencySymbol}", (char)(Integer.parseInt("00A3",16)));
            break;
        case "YEN":
            AdfmfJavaUtilities.setELValue("#{pageFlowScope.currencySymbol}", (char)(Integer.parseInt("00A5",16)));
            break;
        default:
            AdfmfJavaUtilities.setELValue("#{pageFlowScope.currencySymbol}", ' ');
        }
        return _priceCurrency;
    }

    public void setPriceValue(Double _priceValue) {
        this._priceValue = _priceValue;
    }

    public Double getPriceValue() {
        return _priceValue;
    }

    public void setPriceCurrencyId(Integer _priceCurrencyId) {
        this._priceCurrencyId = _priceCurrencyId;
    }

    public Integer getPriceCurrencyId() {
        return _priceCurrencyId;
    }

    public void setAssetSerialNumber(String _assetSerialNumber) {
        this._assetSerialNumber = _assetSerialNumber;
    }

    public String getAssetSerialNumber() {
        return _assetSerialNumber;
    }
}
