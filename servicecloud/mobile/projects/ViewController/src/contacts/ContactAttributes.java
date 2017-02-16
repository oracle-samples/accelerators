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

import java.util.ArrayList;
import util.AttrType;
import util.Attribute;

/**
 * A simple Java wrapper to hold REST attributes for a contact
 */
public class ContactAttributes {
    public ContactAttributes() {
        super();
    }
    
    protected static ArrayList<Attribute> detail = new ArrayList<Attribute>();
    protected static ArrayList<Attribute> create = new ArrayList<Attribute>();

    static {
        // detail attributes
        detail.add(new Attribute("Id", AttrType.INT, "id"));
        detail.add(new Attribute("Name", AttrType.STRING, "lookupName"));
        detail.add(new Attribute("Title", AttrType.STRING, "title"));
        detail.add(new Attribute("City", AttrType.STRING, "address.city"));
        detail.add(new Attribute("Zip", AttrType.STRING, "address.postalCode"));
        detail.add(new Attribute("State", AttrType.STRING, "address.stateOrProvince.lookupName"));
        detail.add(new Attribute("Street", AttrType.STRING, "address.street"));
        detail.add(new Attribute("Country", AttrType.STRING, "address.country.lookupName"));
        detail.add(new Attribute("OrgId", AttrType.MENU, "organization.id"));
        detail.add(new Attribute("OrgName", AttrType.STRING, "organization.lookupName"));
        detail.add(new Attribute("FirstName", AttrType.STRING, "name.first"));
        detail.add(new Attribute("LastName", AttrType.STRING, "name.last"));
        detail.add(new Attribute("StateId", AttrType.MENU, "address.stateOrProvince.id"));
        detail.add(new Attribute("CountryId", AttrType.MENU, "address.country.id"));

        // create attributes
        create.add(new Attribute("Title", AttrType.STRING, "title"));
        create.add(new Attribute("FirstName", AttrType.STRING, "name.first"));
        create.add(new Attribute("LastName", AttrType.STRING, "name.last"));
        create.add(new Attribute("OrgId", AttrType.MENU, "organization.id"));
        create.add(new Attribute("City", AttrType.STRING, "address.city"));
        create.add(new Attribute("Zip", AttrType.STRING, "address.postalCode"));
        create.add(new Attribute("StateId", AttrType.MENU, "address.stateOrProvince.id"));
        create.add(new Attribute("Street", AttrType.STRING, "address.street"));
        create.add(new Attribute("CountryId", AttrType.MENU, "address.country.id"));

    }
}
