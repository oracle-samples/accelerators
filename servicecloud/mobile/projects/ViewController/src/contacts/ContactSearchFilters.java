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

public class ContactSearchFilters {
    private String _orgName;
    private String _email;
    private String _phone;
    private String _firstName;
    private String _lastName;
    private String _assetSerialNumber;
    private String _login;

    public ContactSearchFilters() {
        super();
    }
    
    public ContactSearchFilters(ContactSearchFilters filters) {
        super();
        this._orgName = filters._orgName;
        this._email = filters._email;
        this._phone = filters._phone;
        this._firstName = filters._firstName;
        this._lastName = filters._lastName;
        this._assetSerialNumber = filters._assetSerialNumber;
        this._login = filters._login;
    }
    
    public void setOrgName(String _orgName) {
        this._orgName = _orgName;
    }

    public String getOrgName() {
        return _orgName;
    }

    public void setEmail(String _email) {
        this._email = _email;
    }

    public String getEmail() {
        return _email;
    }

    public void setPhone(String _phone) {
        this._phone = _phone;
    }

    public String getPhone() {
        return _phone;
    }

    public void setFirstName(String _firstName) {
        this._firstName = _firstName;
    }

    public String getFirstName() {
        return _firstName;
    }

    public void setLastName(String _lastName) {
        this._lastName = _lastName;
    }

    public String getLastName() {
        return _lastName;
    }

    public void setAssetSerialNumber(String _assetSerialNumber) {
        this._assetSerialNumber = _assetSerialNumber;
    }

    public String getAssetSerialNumber() {
        return _assetSerialNumber;
    }

    public void setLogin(String _login) {
        this._login = _login;
    }

    public String getLogin() {
        return _login;
    }

}
