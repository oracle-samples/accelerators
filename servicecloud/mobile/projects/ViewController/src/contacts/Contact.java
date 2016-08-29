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
 *  SHA1: $Id: e9397d6c80d78cd9e62340f85f9240b74487b57e $
 * *********************************************************************************************
 *  File: Contact.java
 * *********************************************************************************************/

package contacts;

import java.util.Collection;
import java.util.Collections;

import util.Attribute;
import util.OsvcResource;

/**
 * This is a Java Bean to hold the display attributes of a contact.
 */
public class Contact implements OsvcResource {
    public static final String RESOURCE_NAME = "contacts";

    private Integer _id;
    private String _name;
    private String _orgName;
    private Integer _orgId;
    private String _title;
    private String _address;
    private String _officePhone;
    private String _mobilePhone;
    private String _email;
    private String _street;
    private String _city;
    private String _state;
    private Integer _stateId;
    private String _zip;
    private String _country;
    private Integer _countryId;
    private String _phone;
    private String _firstName;
    private String _lastName;
    private String _homePhone;
    private String _alternateEmail;

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

    public void setOrgName(String _orgName) {
        this._orgName = _orgName;
    }

    public String getOrgName() {
        return _orgName;
    }

    public void setOrgId(Integer _orgId) {
        this._orgId = _orgId;
    }

    public Integer getOrgId() {
        return _orgId;
    }

    public void setTitle(String _title) {
        this._title = _title;
    }

    public String getTitle() {
        return _title;
    }

    public String getAddress() {
        StringBuffer sb = new StringBuffer(200);
        if ( null != _street) sb.append(_street);
        if ( null != _city) sb.append(", ").append(_city);
        if ( null != _state) sb.append(", ").append(_state);
        if ( null != _zip) sb.append(" ").append(_zip);
        if ( null != _country) sb.append(" ").append(_country);
        return sb.toString();
    }

    public void setOfficePhone(String _officePhone) {
        this._officePhone = _officePhone;
    }

    public String getOfficePhone() {
        return _officePhone;
    }

    public void setMobilePhone(String _mobilePhone) {
        this._mobilePhone = _mobilePhone;
    }

    public String getMobilePhone() {
        return _mobilePhone;
    }

    public void setEmail(String _email) {
        this._email = _email;
    }

    public String getEmail() {
        return _email;
    }

    public void setAddress(String _address) {
        this._address = _address;
    }

    public void setStreet(String _street) {
        this._street = _street;
    }

    public String getStreet() {
        return _street;
    }

    public void setCity(String _city) {
        this._city = _city;
    }

    public String getCity() {
        return _city;
    }

    public void setState(String _state) {
        this._state = _state;
    }

    public String getState() {
        return _state;
    }

    public void setZip(String _zip) {
        this._zip = _zip;
    }

    public String getZip() {
        return _zip;
    }

    public void setCountry(String _country) {
        this._country = _country;
    }

    public String getCountry() {
        return _country;
    }

    public void setPhone(String _phone) {
        this._phone = _phone;
    }

    public String getPhone() {
        return _phone;
    }

    public boolean isOrgDisabled() {
        return null == this.getOrgId();
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

    public void setStateId(Integer _stateId) {
        this._stateId = 0 == _stateId ? null : _stateId;
    }

    public Integer getStateId() {
        return _stateId;
    }

    public void setCountryId(Integer _countryId) {
        this._countryId = 0 == _countryId ? null : _countryId;
    }

    public Integer getCountryId() {
        return _countryId;
    }

    @Override
    public String getResourceName() {
        return "contacts";
    }

    @Override
    public Collection<Attribute> getCreateAttributes() {
        return ContactAttributes.create;
    }

    @Override
    public Collection<Attribute> getReadAttributes() {
        return ContactAttributes.detail;
    }

    @Override
    public Collection<Attribute> getUpdateAttributes() {
        // TODO Implement this method
        return Collections.emptySet();
    }

    public void setHomePhone(String _homePhone) {
        this._homePhone = _homePhone;
    }

    public String getHomePhone() {
        return _homePhone;
    }

    public void setAlternateEmail(String _alternateEmail) {
        this._alternateEmail = _alternateEmail;
    }

    public String getAlternateEmail() {
        return _alternateEmail;
    }

}

