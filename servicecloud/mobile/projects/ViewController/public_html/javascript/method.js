/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: Mobile Agent App Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 16.8 (August 2016)
 *  MAF release: 2.3
 *  reference: 151217-000185
 *  date: Tue Aug 23 16:35:54 PDT 2016

 *  revision: rnw-16-8-fixes-release-01
 *  SHA1: $Id: 26ae1d1282202902c8d211c88af4478d559cc34c $
 * *********************************************************************************************
 *  File: method.js
 * ****************************************************************************************** */

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