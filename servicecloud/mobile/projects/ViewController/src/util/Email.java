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
