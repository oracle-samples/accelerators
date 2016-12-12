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
 *  date: Tue Aug 23 16:35:47 PDT 2016

 *  revision: rnw-16-8-fixes-release-01
 *  SHA1: $Id: f8b32b55a1d7a2e5b9c29ec5e9525a8078a4c9b9 $
 * *********************************************************************************************
 *  File: AssetController.java
 * *********************************************************************************************/

package assets;

import java.util.ArrayList;
import java.util.List;
import oracle.adfmf.framework.api.AdfmfContainerUtilities;
import oracle.adfmf.framework.api.AdfmfJavaUtilities;

import util.Util;

public class AssetController {
    private Asset asset;
    private Asset cachedAsset;
    
    public AssetController() {
        super();
    }
    
    public void setAsset(Asset asset) {
        this.asset = asset;
    }

    public Asset newAsset() {
        // set the pageFlowScope.ScannerPage for barcode scanner js check
        AdfmfJavaUtilities.setELValue("#{pageFlowScope.ScannerPage}", "NotAssetQuickSearch");
        if(cachedAsset != null){
            System.out.println("cachedAsset");
            asset = new Asset();
            asset = cachedAsset;
                      
            fetchAndSetIDsFromScope();
            
            cachedAsset = null;
            return asset; 
        }
        Asset asset = new Asset();
        asset.setPriceCurrencyId(1);
        asset.setPriceValue(0.00);

        return asset;
    }
    
    public void cacheEditedAsset(Asset asset) {
        System.out.println("cachingAsset");
        this.cachedAsset = asset;
    }
    
    public void submitNewAsset(Asset asset) {
        // Validate required fields
        if (! verifyInvalidFieldsDisplayAlert(asset) )
            return;
        Util.createObject(asset, "assets", AssetAttributes.assetEditAttrs);     
        AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities.getFeatureId(),
                                                                  "adf.mf.api.amx.doNavigation", new Object[] {
                                                                  "__back" });
    }
    
    public void submitEditedAsset(Asset asset) {
        // Validate required fields
        if ( ! verifyInvalidFieldsDisplayAlert(asset) )
            return;

        System.out.println("Reached Save Edited. Will update asset ID: "+asset.getId());
        
        // Submit edited task
        boolean updateOk;
        updateOk = Util.updateObject(asset, "assets", Integer.toString(asset.getId()), AssetAttributes.assetEditAttrs);
        if (updateOk) 
            AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities.getFeatureId(),
                "adf.mf.api.amx.doNavigation", new Object[] {"__back"});
    }

    private boolean verifyInvalidFieldsDisplayAlert(Asset asset) {       
        List<String> invalidFields = new ArrayList<String>();
        if (asset.getName() == null || asset.getName().trim().isEmpty()) {
            invalidFields.add("Name");
        }
        if (asset.getProductId() == null || asset.getProductId() == 0) {
            invalidFields.add("Product");
        }       
        if (asset.getPurchasedDate() == null) {
            invalidFields.add("Date Purchased");
        }  
        
        if (asset.getPriceValue() == null) {
            asset.setPriceCurrencyId(1);
            asset.setPriceValue(0.0);
        }  
        
        if (!invalidFields.isEmpty()) {
            AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities.getFeatureId(),
                                                                      "navigator.notification.alert", new Object[] {
                                                                      Util.join(invalidFields, ", ",
                                                                                " cannot be empty!"), null, "Alert",
                                                                      "OK"
            });
            return false;
        }

        return true;
    }
    
    private void fetchAndSetIDsFromScope() {       
        Object organizationId = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.organizationId}");
        Object organizationName = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.organizationName}");
        if (organizationId != null) {
            asset.setOrganizationId(Integer.parseInt((String)organizationId));
            asset.setOrganization((String)organizationName);
            AdfmfJavaUtilities.setELValue("#{pageFlowScope.organizationId}", null);
            AdfmfJavaUtilities.setELValue("#{pageFlowScope.organizationName}", null);
        }
       
        Object contactId = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.contactId}");
        Object contactName = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.contactName}");
        if(contactId != null){
            asset.setContactId(Integer.parseInt((String)contactId));
            asset.setContact((String)contactName);
            AdfmfJavaUtilities.setELValue("#{pageFlowScope.contactId}", null);
            AdfmfJavaUtilities.setELValue("#{pageFlowScope.contactName}", null);
        }
        Object productCatalogueId = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.productCatalogueId}");
        Object productCatalogueName = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.productCatalogueName}");
        if(productCatalogueId != null){
            asset.setProductId(Integer.parseInt((String)productCatalogueId));
            asset.setProduct((String)productCatalogueName);
            AdfmfJavaUtilities.setELValue("#{pageFlowScope.productCatalogueId}", null);
            AdfmfJavaUtilities.setELValue("#{pageFlowScope.productCatalogueName}", null);
        }
    }
    
    public Asset getAsset() {
        // set the pageFlowScope.ScannerPage for barcode scanner js check
        AdfmfJavaUtilities.setELValue("#{pageFlowScope.ScannerPage}", "NotAssetQuickSearch");
        if (cachedAsset!= null) {
            System.out.println("Retrieving cachedAssetin getAsset");
            asset = cachedAsset;

            fetchAndSetIDsFromScope();
            
            cachedAsset = null;
            return asset; 
        }
        
        Integer id;
        Object ido = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.assetId}");

        if (ido instanceof String) {
            id = Integer.valueOf((String) ido);
        } else {
            id = (Integer) ido;
        }

        asset = new Asset();
        System.out.println("loadAsset id: " + id);
        Object obj = Util.loadObject("assets.Asset", "Assets", id.toString(), AssetAttributes.assetDetailsAttrs);

        if (obj != null) {
            asset = (Asset) obj;
        }
        return asset;
    }
}
