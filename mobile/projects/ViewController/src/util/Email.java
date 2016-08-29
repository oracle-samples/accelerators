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
 *  date: Tue Aug 23 16:36:00 PDT 2016

 *  revision: rnw-16-8-fixes-release-01
 *  SHA1: $Id: be864d920e6b77e4d9b8a9548caa447fac0b6852 $
 * *********************************************************************************************
 *  File: Email.java
 * *********************************************************************************************/

package util;

import java.util.ArrayList;
import java.util.Collection;
import java.util.Collections;

/**
 * A simple Java wrapper to hold REST attributes for an email
 */
public class Email implements OsvcResource {

    public static final String RESOURCE_NAME = "emails";
    private static ArrayList<Attribute> create = new ArrayList<Attribute>();

    static {
        // create attributes
        create.add(new Attribute("Address", AttrType.STRING, "address"));
        create.add(new Attribute("AddressType", AttrType.STRING, "addressType.lookupName"));
    }

    private String _address;
    private String _addressType;

    public Email() {
        super();
    }

    public Email(String anAddress, String aType) {
        super();
        this._address = anAddress;
        this._addressType = aType;
    }

    @Override
    public String getResourceName() {
        return Email.RESOURCE_NAME;
    }

    @Override
    public Collection<Attribute> getCreateAttributes() {
        return Email.create;
    }

    @Override
    public Collection<Attribute> getReadAttributes() {
        // TODO Implement this method
        return Collections.emptySet();
    }

    @Override
    public Collection<Attribute> getUpdateAttributes() {
        return Email.create;
    }

    public void setAddress(String _address) {
        this._address = _address;
    }

    public String getAddress() {
        return _address;
    }

    public void setAddressType(String _addressType) {
        this._addressType = _addressType;
    }

    public String getAddressType() {
        return _addressType;
    }
}
