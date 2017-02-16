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
