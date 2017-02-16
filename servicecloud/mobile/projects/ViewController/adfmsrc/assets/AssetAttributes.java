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

package assets;

import java.util.ArrayList;

import util.AttrType;
import util.Attribute;

public class AssetAttributes {
    public AssetAttributes() {
        super();
    }
    
    protected static ArrayList<Attribute> assetDetailsAttrs = new ArrayList<Attribute>();
    protected static ArrayList<Attribute> assetEditAttrs = new ArrayList<Attribute>();

    static
    {
        // detail attrs
        assetDetailsAttrs.add(new Attribute("Id", AttrType.INT, "id"));
        assetDetailsAttrs.add(new Attribute("Name", AttrType.STRING, "name"));
        assetDetailsAttrs.add(new Attribute("Description", AttrType.STRING, "description"));
        assetDetailsAttrs.add(new Attribute("AssetSerialNumber", AttrType.STRING, "serialNumber"));
        assetDetailsAttrs.add(new Attribute("ContactId", AttrType.MENU, "contact.id"));
        assetDetailsAttrs.add(new Attribute("Contact", AttrType.STRING, "contact.lookupName"));
        assetDetailsAttrs.add(new Attribute("OrganizationId", AttrType.MENU, "organization.id"));
        assetDetailsAttrs.add(new Attribute("Organization", AttrType.STRING, "organization.lookupName"));
        assetDetailsAttrs.add(new Attribute("ProductId", AttrType.MENU, "product.id"));
        assetDetailsAttrs.add(new Attribute("Product", AttrType.STRING, "product.lookupName"));
        assetDetailsAttrs.add(new Attribute("StatusId", AttrType.MENU, "statusWithType.status.id"));
        assetDetailsAttrs.add(new Attribute("Status", AttrType.STRING, "statusWithType.status.lookupName"));
        assetDetailsAttrs.add(new Attribute("PurchasedDate", AttrType.DATE, "purchasedDate"));
        assetDetailsAttrs.add(new Attribute("InstalledDate", AttrType.DATE, "installedDate"));
        assetDetailsAttrs.add(new Attribute("RetiredDate", AttrType.DATE, "retiredDate"));
        assetDetailsAttrs.add(new Attribute("PriceCurrencyId", AttrType.MENU, "price.currency.id"));
        assetDetailsAttrs.add(new Attribute("PriceCurrency", AttrType.STRING, "price.currency.lookupName"));
        assetDetailsAttrs.add(new Attribute("PriceValue", AttrType.DECIMAL, "price.value"));
        
        // edit attrs
        assetEditAttrs.add(new Attribute("Name", AttrType.STRING, "name"));
        assetEditAttrs.add(new Attribute("Description", AttrType.STRING, "description"));
        assetEditAttrs.add(new Attribute("AssetSerialNumber", AttrType.STRING, "serialNumber"));
        assetEditAttrs.add(new Attribute("ContactId", AttrType.MENU, "contact.id"));
        assetEditAttrs.add(new Attribute("OrganizationId", AttrType.MENU, "organization.id"));
        assetEditAttrs.add(new Attribute("ProductId", AttrType.MENU, "product.id"));
        assetEditAttrs.add(new Attribute("StatusId", AttrType.MENU, "statusWithType.status.id"));
        assetEditAttrs.add(new Attribute("PurchasedDate", AttrType.DATE, "purchasedDate"));
        assetEditAttrs.add(new Attribute("InstalledDate", AttrType.DATE, "installedDate"));
        assetEditAttrs.add(new Attribute("RetiredDate", AttrType.DATE, "retiredDate"));
        assetEditAttrs.add(new Attribute("PriceCurrencyId", AttrType.MENU, "price.currency.id"));
        assetEditAttrs.add(new Attribute("PriceValue", AttrType.DECIMAL, "price.value"));
    }
}
