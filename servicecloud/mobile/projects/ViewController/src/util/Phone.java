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
 *  date: Tue Aug 23 16:36:01 PDT 2016

 *  revision: rnw-16-8-fixes-release-01
 *  SHA1: $Id: c1d3c6985b505e0d4ef28b48574e81c28f73d31a $
 * *********************************************************************************************
 *  File: Phone.java
 * *********************************************************************************************/

package util;

import java.util.ArrayList;
import java.util.Collection;
import java.util.Collections;

/**
 * A simple Java wrapper to hold REST attributes for a phone
 */
public class Phone implements OsvcResource {

    public static final String RESOURCE_NAME = "phones";
    private static ArrayList<Attribute> create = new ArrayList<Attribute>();

    static {
        // create attributes
        create.add(new Attribute("Number", AttrType.STRING, "number"));
        create.add(new Attribute("PhoneType", AttrType.STRING, "phoneType.lookupName"));
    }

    private String _number;
    private String _phoneType;

    public Phone() {
        super();
    }

    public Phone(String aNumber, String aType) {
        super();
        this._number = aNumber;
        this._phoneType = aType;
    }

    @Override
    public String getResourceName() {
        return Phone.RESOURCE_NAME;
    }

    @Override
    public Collection<Attribute> getCreateAttributes() {
        return Phone.create;
    }

    @Override
    public Collection<Attribute> getReadAttributes() {
        // TODO Implement this method
        return Collections.emptySet();
    }

    @Override
    public Collection<Attribute> getUpdateAttributes() {
        return Phone.create;
    }

    public void setNumber(String _number) {
        this._number = _number;
    }

    public String getNumber() {
        return _number;
    }

    public void setPhoneType(String _phoneType) {
        this._phoneType = _phoneType;
    }

    public String getPhoneType() {
        return _phoneType;
    }
}
