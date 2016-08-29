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
 *  date: Tue Aug 23 16:35:57 PDT 2016

 *  revision: rnw-16-8-fixes-release-01
 *  SHA1: $Id: 8fcd6da4292c884d3272799431283f113ba2df6c $
 * *********************************************************************************************
 *  File: Incident.java
 * *********************************************************************************************/

package incidents;

import java.util.Date;

import util.Util;

public class Incident {
    
    private Integer _id;
    private String _refNo;
    private String _subject;
    private Integer _statusId;
    private String _status;
    private Date _updatedTime;
    private String _updatedTimeString;

    private Integer _contactId;
    private String _contact;
    private Date _createdTime;
    private String _createdTimeString;
    private Integer _organizationId;
    private String _organization;
        
    private Integer _productId;    
    private String _product;
    private String _queue;
    private Integer _categoryId;
    private String _category;
    private Integer _severityId;
    private String _severity;
    private String _disposition;
    private Integer _assignedId;
    private String _assigned;
    private String _fileName;
    private String _data;
    
    private String _contactFirstName;
    private String _contactLastName;
    private Integer _assetId;
    private String _assetName;;
    private String _assetSerialNumber;

    public Incident() {
        super();
    }
    
    public int getId() {
        return _id;
    }

    public void setId(Integer id) {
        this._id = id;
    }
    
    public String getRefNo() {
        return _refNo;
    }

    public void setRefNo(String refNo) {
        this._refNo = refNo;
    }
    
    public String getSubject() {
        return _subject;
    }

    public void setSubject(String subject) {
        if(subject == "")
            subject = null;
        this._subject = subject;
    }

    public Integer getStatusId() {
        return _statusId;
    }

    public void setStatusId(Integer statusId) {
        this._statusId = statusId;
    }
    
    public String getStatus() {
        return _status;
    }

    public void setStatus(String status) {
        this._status = status;
    }
    
    public Integer getContactId() {
        return _contactId;
    }

    public void setContactId(Integer contactId) {
        if(contactId == 0)
            contactId = null;
        this._contactId = contactId;
    }

    public String getContact() {
        return _contact;
    }

    public void setContact(String contact) {
        this._contact = contact;
    }
    
    public Date getUpdatedTime() {
        return _updatedTime;
    }

    public void setUpdatedTime(Date updatedTime) {
        this._updatedTime = updatedTime;
    }

    public void setProductId(Integer productId) {
        if(productId == 0)
            productId = null;
        this._productId = productId;
    }

    public Integer getProductId() {
        return _productId;
    }
    
    public void setProduct(String product) {
        this._product = product;
    }

    public String getProduct() {
        return _product;
    }
    
    public void setQueue(String queue) {
        this._queue = queue;
    }

    public String getQueue() {
        return _queue;
    }

    public void setCategoryId(Integer categoryId) {
        if(categoryId == 0)
            categoryId = null;
        this._categoryId = categoryId;
    }
    
    public Integer getCategoryId() {
        return _categoryId;
    }

    public void setCategory(String _category) {
        this._category = _category;
    }
    
    public String getCategory() {
        return _category;
    }
    
    public void setSeverityId(Integer severityId) {
        if(severityId == 0)
            severityId = null;
        this._severityId = severityId;
    }

    public Integer getSeverityId() {
        return _severityId;
    }
    
    public void setSeverity(String severity) {
        this._severity = severity;
    }

    public String getSeverity() {
        return _severity;
    }

    public void setDisposition(String disposition) {
        this._disposition = disposition;
    }

    public String getDisposition() {
        return _disposition;
    }

    public void setAssignedId(Integer assignedId) {
        if(assignedId == 0)
            assignedId = null;
        this._assignedId = assignedId;
    }

    public Integer getAssignedId() {
        return _assignedId;
    }
    
    public void setAssigned(String assigned) {
        this._assigned = assigned;
    }

    public String getAssigned() {
        return _assigned;
    }

    public void setCreatedTime(Date createdTime) {
        this._createdTime = createdTime;
    }

    public Date getCreatedTime() {
        return _createdTime;
    }

    public void setOrganizationId(Integer organizationId) {
        this._organizationId = organizationId;
    }

    public Integer getOrganizationId() {
        return _organizationId;
    }
    
    public void setOrganization(String organization) {
        this._organization = organization;
    }

    public String getOrganization() {
        return _organization;
    }
    
    public void setUpdatedTimeString(String _updatedTimeString) {
        this._updatedTimeString = _updatedTimeString;
    }

    public String getUpdatedTimeString() {
        return _updatedTimeString;
    }

    public void setCreatedTimeString(String _createdTimeString) {
        this._createdTimeString = _createdTimeString;
    }

    public String getCreatedTimeString() {
        return _createdTimeString;
    }
    public void setFileName(String _fileName) {
        this._fileName = _fileName;
    }

    public String getFileName() {
        return _fileName;
    }

    public void setData(String _data) {
        this._data = _data;
    }
    /**
     * for attachment.
     * @return
     */
    public String getData() {
        return _data;
    }

    public boolean isOrganizationDisabled() {
        return null == this.getOrganizationId();
    }

    public boolean isContactDisabled() {
        return null == this.getContactId();
    }
    
    /**
     * for incident search.
     * @return
     */
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

    public void setAssetId(Integer _assetId) {
        this._assetId = _assetId;
    }

    public Integer getAssetId() {
        return _assetId;
    }

    public void setAssetName(String _assetName) {
        this._assetName = _assetName;
    }

    public String getAssetName() {
        return _assetName;
    }
    public void setAssetSerialNumber(String assetSerialNumber) {
        this._assetSerialNumber = assetSerialNumber;
    }

    public String getAssetSerialNumber() {
        return _assetSerialNumber;
    }

}
