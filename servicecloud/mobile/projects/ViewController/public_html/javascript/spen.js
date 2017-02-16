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
    launchSurfacePopup = function (noteId, retType) {
        var popupOptions = {};
        popupOptions.id = noteId;
        popupOptions.sPenFlags = samsung.spen.FLAG_PEN_SETTINGS
            | samsung.spen.FLAG_BACKGROUND | samsung.spen.FLAG_SELECTION;
        popupOptions.backgroundColor = "#F3F781";
        popupOptions.returnType = samsung.spen.RETURN_TYPE_IMAGE_DATA;
        samsung.spen.launchSurfacePopup(popupOptions,successCallback,  
                                        errorCallback);
    }

    //return value is string of Base64 encoded image
    function successCallback(imageData){
        if (imageData) {
            adf.mf.api.setValue( { "name": "#{applicationScope.sPenData}", 
                               "value": imageData}, 
                               function() {}, 
                               function() {});
            adf.mf.api.amx.doNavigation('goToAddSPenImage');
        } else {
            adf.mf.api.setValue( { "name": "#{applicationScope.sPenData}", 
                               "value": "" }, 
                               function() {}, 
                               function() {});
        }
    }
 		
    function errorCallback(errorMessage){
        alert(errorMessage);
    }

}
)();

