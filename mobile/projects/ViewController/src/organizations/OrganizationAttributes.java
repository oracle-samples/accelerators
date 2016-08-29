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
 *  date: Tue Aug 23 16:35:58 PDT 2016

 *  revision: rnw-16-8-fixes-release-01
 *  SHA1: $Id: 24cc8a85d370ff90b98180376e76ad969904298d $
 * *********************************************************************************************
 *  File: OrganizationAttributes.java
 * *********************************************************************************************/

package organizations;
import java.util.ArrayList;
import util.AttrType;
import util.Attribute;

public class OrganizationAttributes {

    public OrganizationAttributes() {
        super();
    }

    protected static ArrayList<Attribute> organizationAttrs = new ArrayList<Attribute>();
    protected static ArrayList<Attribute> organizationMultiAttrs = new ArrayList<Attribute>();

    static {
        organizationAttrs.add(new Attribute("Id", AttrType.INT, "id"));
        organizationAttrs.add(new Attribute("IndustryId", AttrType.MENU, "industry.id"));
        organizationAttrs.add(new Attribute("Industry", AttrType.STRING, "industry.lookupName"));
        organizationAttrs.add(new Attribute("Name", AttrType.STRING, "name"));
        organizationAttrs.add(new Attribute("Parent", AttrType.STRING, "parent.lookupName"));

        organizationAttrs.add(new Attribute("CssState", AttrType.INT, "CRMModules.Service"));
        organizationAttrs.add(new Attribute("MaState", AttrType.INT, "CRMModules.Marketing")); // outreach
        organizationAttrs.add(new Attribute("SaState", AttrType.INT, "CRMModules.Sales"));  // opps

        organizationAttrs.add(new Attribute("Sla", AttrType.STRING, "ServiceSettings.SLAInstances.NameOfSLA.LookupName"));
        organizationAttrs.add(new Attribute("StateOfSla", AttrType.STRING, "ServiceSettings.SLAInstances.StateOfSLA.LookupName"));

        organizationMultiAttrs.add(new Attribute("Street", AttrType.STRING, "addresses.street"));
        organizationMultiAttrs.add(new Attribute("City", AttrType.STRING, "addresses.city"));
        organizationMultiAttrs.add(new Attribute("State", AttrType.STRING, "addresses.StateOrProvince.LookupName"));
        organizationMultiAttrs.add(new Attribute("PostalCode", AttrType.STRING, "addresses.PostalCode"));
        organizationMultiAttrs.add(new Attribute("Country", AttrType.STRING, "addresses.Country.LookupName"));
        organizationMultiAttrs.add(new Attribute("CountryId", AttrType.INT, "addresses.Country.id"));
        organizationMultiAttrs.add(new Attribute("StateId", AttrType.INT, "addresses.StateOrProvince.id"));

    }
}
