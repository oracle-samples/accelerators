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
 *  date: Tue Aug 23 16:35:58 PDT 2016

 *  revision: rnw-16-8-fixes-release-01
 *  SHA1: $Id: eefce08d8c10f4ec8dae1d0d8d4700193499c77c $
 * *********************************************************************************************
 *  File: IncidentSearchFilters.java
 * *********************************************************************************************/

package incidents;

import java.util.Date;


public class IncidentSearchFilters {
    private String _refNo;
    private String _subject;
    private Integer _statusId;
    private String _status;
    private Date _updatedSince;
    private String _updatedSinceString;

    private Date _createdSince;
    private String _createdSinceString;
    
    private String _contactFirstName;
    private String _contactLastName;
    private String _assetSerialNumber;
    
    private Integer _statusTypeId;
    private String _statusType;
    private String _assigned;

    public IncidentSearchFilters() {
        super();
    }
    
    public IncidentSearchFilters(IncidentSearchFilters filters){
        super();
        this._refNo = filters._refNo;
        this._subject = filters._subject;
        this._statusId = filters._statusId;
        this._status = filters._status;
        this._updatedSince = filters._updatedSince;
        this._createdSince = filters._createdSince;
        this._updatedSinceString = filters._updatedSinceString;
        this._createdSinceString = filters._createdSinceString;
        this._contactFirstName = filters._contactFirstName;
        this._contactLastName = filters._contactLastName;
        this._assetSerialNumber = filters._assetSerialNumber;
        this._statusTypeId = filters._statusTypeId;
        this._statusType = filters._statusType;
        this._assigned = filters._assigned;
    }
    
    public void setRefNo(String _refNo) {
        this._refNo = _refNo;
    }

    public String getRefNo() {
        return _refNo;
    }

    public void setSubject(String _subject) {
        this._subject = _subject;
    }

    public String getSubject() {
        return _subject;
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

    public void setUpdatedSince(Date _updatedSince) {
        this._updatedSince = _updatedSince;
    }

    public Date getUpdatedSince() {
        return _updatedSince;
    }

    public void setUpdatedSinceString(String _updatedSinceString) {
        this._updatedSinceString = _updatedSinceString;
    }

    public String getUpdatedSinceString() {
        return _updatedSinceString;
    }

    public void setCreatedSince(Date _createdSince) {
        this._createdSince = _createdSince;
    }

    public Date getCreatedSince() {
        return _createdSince;
    }

    public void setCreatedSinceString(String _createdSinceString) {
        this._createdSinceString = _createdSinceString;
    }

    public String getCreatedSinceString() {
        return _createdSinceString;
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

    public void setAssetSerialNumber(String _assetSerialNumber) {
        this._assetSerialNumber = _assetSerialNumber;
    }

    public String getAssetSerialNumber() {
        return _assetSerialNumber;
    }

    public void setStatusTypeId(Integer _statusTypeId) {
        this._statusTypeId = _statusTypeId;
    }

    public Integer getStatusTypeId() {
        return _statusTypeId;
    }

    public void setStatusType(String _statusType) {
        this._statusType = _statusType;
    }

    public String getStatusType() {
        return _statusType;
    }

    public void setAssigned(String _assigned) {
        this._assigned = _assigned;
    }

    public String getAssigned() {
        return _assigned;
    }
    
}
