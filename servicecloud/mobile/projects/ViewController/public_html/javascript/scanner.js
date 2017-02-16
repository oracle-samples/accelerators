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

(function () {

    function initialise() {
        // Nothing to initialise
    }
    
    function onSuccess(result) {

     var scannerPage;
     adf.mf.el.getValue("#{pageFlowScope.ScannerPage}", 
        function(req, res) {scannerPage= res[0]['value'];},function() {})

        adf.mf.api.setValue( { "name": "#{applicationScope.barcodeError}", 
                               "value": ""}, 
                               function() {}, 
                               function() {});
                               
        if (scannerPage.valueOf() == "AssetQuickSearch")
                adf.mf.api.setValue( { "name": "#{bindings.searchValue.inputValue}", 
                               "value": result.text}, 
                               function() {}, 
                               function() {});
        else if (scannerPage.valueOf() == "NotAssetQuickSearch")
               adf.mf.api.setValue( { "name": "#{bindings.assetSerialNumber.inputValue}", 
                               "value": result.text}, 
                               function() {}, 
                               function() {});                
        adf.mf.api.setValue( { "name": "#{applicationScope.barcodeFormat}", 
                               "value": result.format}, 
                               function() {}, 
                               function() {});

        adf.mf.api.setValue( { "name": "#{applicationScope.barcodeCancelled}", 
                               "value": result.cancelled == 1 ? "Yes" : "No"}, 
                               function() {}, 
                               function() {});
        adf.mf.api.amx.doNavigation('goToBarcode');
    }
    
    function onError(error) {
        adf.mf.api.setValue( { "name": "#{applicationScope.barcodeError}", 
                               "value": "ERROR: " + error.text}, 
                               function() {}, 
                               function() {});
    }
    
    // Callable externally
    scanBarcodeFromJavaBean = function() {
        cordova.plugins.barcodeScanner.scan(onSuccess, onError);
    }

    document.addEventListener("showpagecomplete", initialise, false);
    
}) ();