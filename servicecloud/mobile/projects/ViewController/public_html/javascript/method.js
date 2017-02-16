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
    discardConfirmation = function (message, title) {
        navigator.notification.confirm( message, onDiscardConfirm, title,['Yes','No']  );
    };

    function onDiscardConfirm(buttonIndex) {
        if (buttonIndex === 1){
            adf.mf.api.amx.doNavigation('goToSpringboard');
        }
    }

    yesNoForDeletion = function (message, title, incId, attId) {
        navigator.notification.confirm( message, function(buttonIndex){onYesGoBack(buttonIndex,incId, attId);}, title, ['Yes','No']  );
    };

    function onYesGoBack(buttonIndex, incId, attId) {
        if (buttonIndex === 1){
            adf.mf.api.invokeMethod("incidents.IncidentAttachmentsController", "deleteAttachment", incId, attId, onInvokeSuccess, onFail); 
        } 
    }
    
    function onInvokeSuccess() {
        
    };
    
    function onFail() {
       
    };

}
)();