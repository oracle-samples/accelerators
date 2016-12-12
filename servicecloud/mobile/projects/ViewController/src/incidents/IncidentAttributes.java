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
 *  SHA1: $Id: 2cba6849faa3fd7438556f4cf7cbe779ec382fa8 $
 * *********************************************************************************************
 *  File: IncidentAttributes.java
 * *********************************************************************************************/

package incidents;

import java.util.ArrayList;

import java.util.List;

import util.AttrType;
import util.Attribute;

public class IncidentAttributes {
    public IncidentAttributes() {
        super();
    }
    
    protected static ArrayList<Attribute> incidentDetailsAttrs = new ArrayList<Attribute>();
    protected static ArrayList<Attribute> incidentEditAttrs = new ArrayList<Attribute>();
    protected static List<Attribute> incidentAttachmentAttrs = new ArrayList<Attribute>();

    static
    {
        // detail attrs
        incidentDetailsAttrs.add(new Attribute("Id", AttrType.INT, "id"));
        incidentDetailsAttrs.add(new Attribute("RefNo", AttrType.STRING, "referenceNumber"));
        incidentDetailsAttrs.add(new Attribute("Subject", AttrType.STRING, "subject"));
        incidentDetailsAttrs.add(new Attribute("StatusId", AttrType.MENU, "statusWithType.status.id"));
        incidentDetailsAttrs.add(new Attribute("Status", AttrType.STRING, "statusWithType.status.lookupName"));
        incidentDetailsAttrs.add(new Attribute("ContactId", AttrType.MENU, "primaryContact.contact.id"));
        incidentDetailsAttrs.add(new Attribute("Contact", AttrType.STRING, "primaryContact.contact.lookupName"));
        incidentDetailsAttrs.add(new Attribute("OrganizationId", AttrType.MENU, "organization.id"));
        incidentDetailsAttrs.add(new Attribute("Organization", AttrType.STRING, "organization.lookupName"));
        incidentDetailsAttrs.add(new Attribute("CreatedTime", AttrType.DATETIME, "createdTime"));
        incidentDetailsAttrs.add(new Attribute("UpdatedTime", AttrType.DATETIME, "updatedTime"));
        incidentDetailsAttrs.add(new Attribute("Queue", AttrType.STRING, "queue.lookupName"));
        
        incidentDetailsAttrs.add(new Attribute("ProductId", AttrType.MENU, "product.id"));
        incidentDetailsAttrs.add(new Attribute("Product", AttrType.STRING, "product.lookupName"));
        incidentDetailsAttrs.add(new Attribute("CategoryId", AttrType.MENU, "category.id"));
        incidentDetailsAttrs.add(new Attribute("Category", AttrType.STRING, "category.name"));
        incidentDetailsAttrs.add(new Attribute("SeverityId", AttrType.MENU, "severity.id"));
        incidentDetailsAttrs.add(new Attribute("Severity", AttrType.STRING, "severity.lookupName"));
        incidentDetailsAttrs.add(new Attribute("Disposition", AttrType.STRING, "disposition.name"));
        incidentDetailsAttrs.add(new Attribute("AssignedId", AttrType.MENU, "assignedTo.account.id"));
        incidentDetailsAttrs.add(new Attribute("Assigned", AttrType.STRING, "assignedTo.account.lookupName"));

        incidentDetailsAttrs.add(new Attribute("GpsLatitude", AttrType.STRING, "CustomFields.Mobile.gps_latitude"));
        incidentDetailsAttrs.add(new Attribute("GpsLongitude", AttrType.STRING, "CustomFields.Mobile.gps_longitude"));

        incidentDetailsAttrs.add(new Attribute("AssetId", AttrType.INT, "asset.id"));
        incidentDetailsAttrs.add(new Attribute("AssetName", AttrType.STRING, "asset.lookupName"));
        incidentDetailsAttrs.add(new Attribute("AssetSerialNumber", AttrType.STRING, "parentAsset.serialNumber"));

        // edit attrs
        incidentEditAttrs.add(new Attribute("Subject", AttrType.STRING, "subject"));
        incidentEditAttrs.add(new Attribute("StatusId", AttrType.MENU, "statusWithType.status.id"));
        incidentEditAttrs.add(new Attribute("ContactId", AttrType.MENU, "primaryContact.id"));
        incidentEditAttrs.add(new Attribute("ProductId", AttrType.MENU, "product.id"));
        incidentEditAttrs.add(new Attribute("CategoryId", AttrType.MENU, "category.id"));
        incidentEditAttrs.add(new Attribute("SeverityId", AttrType.MENU, "severity.id"));
        incidentEditAttrs.add(new Attribute("AssignedId", AttrType.MENU, "assignedTo.account.id"));
        incidentEditAttrs.add(new Attribute("OrganizationId", AttrType.MENU, "organization.id"));
        incidentEditAttrs.add(new Attribute("AssetId", AttrType.MENU, "asset.id"));
        
        // add attach attrs 
        incidentAttachmentAttrs.add(new Attribute("FileName", AttrType.MENU, "fileName"));
        incidentAttachmentAttrs.add(new Attribute("Data", AttrType.MENU, "data"));
        
    }
}
