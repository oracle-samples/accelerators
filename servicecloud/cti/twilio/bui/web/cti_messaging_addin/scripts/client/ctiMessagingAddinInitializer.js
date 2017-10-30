/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set 
 *  published by Oracle under the Universal Permissive License (UPL), Version 1.0
 *  Copyright (c) 2017 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: Telephony and SMS Accelerator for Twilio
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 17D (November 2017)
 *  reference: 161212-000059
 *  date: Monday Oct 30 13:4:53 UTC 2017
 *  revision: rnw-17-11-fixes-releases
 * 
 *  SHA1: $Id: 69ac8c6014ffe435fb185c7a5ba688f18b8c0f36 $
 * *********************************************************************************************
 *  File: ctiMessagingAddinInitializer.js
 * ****************************************************************************************** */
define(["require", "exports", "../utils/messageConstants", "../utils/ctiFilePathUtil"], function (require, exports, messageConstants_1, ctiFilePathUtil_1) {
    "use strict";
    exports.__esModule = true;
    var CtiMessagingAddinInitializer = /** @class */ (function () {
        function CtiMessagingAddinInitializer() {
        }
        CtiMessagingAddinInitializer.initialize = function () {
            //TODO - Authorization
            ORACLE_SERVICE_CLOUD.extension_loader.load(messageConstants_1.MessageConstants.BUI_CTI_SMS_ADDIN_ID, messageConstants_1.MessageConstants.BUI_CTI_SMS_ADDIN_VERSION).then(function (sdk) {
                sdk.registerUserInterfaceExtension(function (userInterfaceContext) {
                    userInterfaceContext.getLeftSidePaneContext().then(function (leftSidePaneContext) {
                        leftSidePaneContext.getSidePane(messageConstants_1.MessageConstants.BUI_CTI_LEFT_PANEL_SMS_MENU_ID).then(function (leftPanelMenu) {
                            //TODO - check this path
                            leftPanelMenu.setContentUrl(ctiFilePathUtil_1.CtiFilePathUtil.getAbsolutePath(CtiMessagingAddinInitializer.addinFilePath));
                            leftPanelMenu.setVisible(false);
                            leftPanelMenu.render();
                        });
                    });
                });
            });
        };
        CtiMessagingAddinInitializer.addinFilePath = 'ctiMessagingAddin.html';
        return CtiMessagingAddinInitializer;
    }());
    exports.CtiMessagingAddinInitializer = CtiMessagingAddinInitializer;
    CtiMessagingAddinInitializer.initialize();
});
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiY3RpTWVzc2FnaW5nQWRkaW5Jbml0aWFsaXplci5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbImN0aU1lc3NhZ2luZ0FkZGluSW5pdGlhbGl6ZXIudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUE7Ozs7O2dHQUtnRzs7OztJQU1oRztRQUFBO1FBcUJBLENBQUM7UUFqQmlCLHVDQUFVLEdBQXhCO1lBQ0ksc0JBQXNCO1lBQ3RCLG9CQUFvQixDQUFDLGdCQUFnQixDQUFDLElBQUksQ0FBQyxtQ0FBZ0IsQ0FBQyxvQkFBb0IsRUFBRSxtQ0FBZ0IsQ0FBQyx5QkFBeUIsQ0FBQyxDQUFDLElBQUksQ0FBQyxVQUFDLEdBQUc7Z0JBQ25JLEdBQUcsQ0FBQyw4QkFBOEIsQ0FBQyxVQUFDLG9CQUFvQjtvQkFDcEQsb0JBQW9CLENBQUMsc0JBQXNCLEVBQUUsQ0FBQyxJQUFJLENBQzlDLFVBQUMsbUJBQW1CO3dCQUNoQixtQkFBbUIsQ0FBQyxXQUFXLENBQUMsbUNBQWdCLENBQUMsOEJBQThCLENBQUMsQ0FBQyxJQUFJLENBQUMsVUFBQyxhQUF3Qjs0QkFDM0csd0JBQXdCOzRCQUN4QixhQUFhLENBQUMsYUFBYSxDQUFDLGlDQUFlLENBQUMsZUFBZSxDQUFDLDRCQUE0QixDQUFDLGFBQWEsQ0FBQyxDQUFDLENBQUM7NEJBQ3pHLGFBQWEsQ0FBQyxVQUFVLENBQUMsS0FBSyxDQUFDLENBQUM7NEJBQ2hDLGFBQWEsQ0FBQyxNQUFNLEVBQUUsQ0FBQzt3QkFDM0IsQ0FBQyxDQUFDLENBQUM7b0JBQ1AsQ0FBQyxDQUNKLENBQUM7Z0JBQ04sQ0FBQyxDQUFDLENBQUM7WUFDUCxDQUFDLENBQUMsQ0FBQztRQUNQLENBQUM7UUFsQmMsMENBQWEsR0FBVSx3QkFBd0IsQ0FBQztRQW1CbkUsbUNBQUM7S0FBQSxBQXJCRCxJQXFCQztJQXJCWSxvRUFBNEI7SUF1QnpDLDRCQUE0QixDQUFDLFVBQVUsRUFBRSxDQUFDIiwic291cmNlc0NvbnRlbnQiOlsiLyogKiAqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqXG4gKiAgJEFDQ0VMRVJBVE9SX0hFQURFUl9QTEFDRV9IT0xERVIkXG4gKiAgU0hBMTogJElkOiA2OWFjOGM2MDE0ZmZlNDM1ZmIxODVjN2E1YmE2ODhmMThiOGMwZjM2ICRcbiAqICoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKlxuICogIEZpbGU6ICRBQ0NFTEVSQVRPUl9IRUFERVJfRklMRV9OQU1FX1BMQUNFX0hPTERFUiRcbiAqICoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKiAqL1xuXG5pbXBvcnQge01lc3NhZ2VDb25zdGFudHN9IGZyb20gXCIuLi91dGlscy9tZXNzYWdlQ29uc3RhbnRzXCI7XG5pbXBvcnQge0N0aUZpbGVQYXRoVXRpbH0gZnJvbSBcIi4uL3V0aWxzL2N0aUZpbGVQYXRoVXRpbFwiO1xuaW1wb3J0IElTaWRlUGFuZSA9IE9SQUNMRV9TRVJWSUNFX0NMT1VELklTaWRlUGFuZTtcblxuZXhwb3J0IGNsYXNzIEN0aU1lc3NhZ2luZ0FkZGluSW5pdGlhbGl6ZXIge1xuXG4gICAgcHJpdmF0ZSBzdGF0aWMgYWRkaW5GaWxlUGF0aDpzdHJpbmcgPSAnY3RpTWVzc2FnaW5nQWRkaW4uaHRtbCc7XG5cbiAgICBwdWJsaWMgc3RhdGljIGluaXRpYWxpemUoKTogdm9pZCB7XG4gICAgICAgIC8vVE9ETyAtIEF1dGhvcml6YXRpb25cbiAgICAgICAgT1JBQ0xFX1NFUlZJQ0VfQ0xPVUQuZXh0ZW5zaW9uX2xvYWRlci5sb2FkKE1lc3NhZ2VDb25zdGFudHMuQlVJX0NUSV9TTVNfQURESU5fSUQsIE1lc3NhZ2VDb25zdGFudHMuQlVJX0NUSV9TTVNfQURESU5fVkVSU0lPTikudGhlbigoc2RrKSA9PiB7XG4gICAgICAgICAgICBzZGsucmVnaXN0ZXJVc2VySW50ZXJmYWNlRXh0ZW5zaW9uKCh1c2VySW50ZXJmYWNlQ29udGV4dCkgPT4ge1xuICAgICAgICAgICAgICAgIHVzZXJJbnRlcmZhY2VDb250ZXh0LmdldExlZnRTaWRlUGFuZUNvbnRleHQoKS50aGVuKFxuICAgICAgICAgICAgICAgICAgICAobGVmdFNpZGVQYW5lQ29udGV4dCkgPT4ge1xuICAgICAgICAgICAgICAgICAgICAgICAgbGVmdFNpZGVQYW5lQ29udGV4dC5nZXRTaWRlUGFuZShNZXNzYWdlQ29uc3RhbnRzLkJVSV9DVElfTEVGVF9QQU5FTF9TTVNfTUVOVV9JRCkudGhlbigobGVmdFBhbmVsTWVudTogSVNpZGVQYW5lKSA9PiB7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgLy9UT0RPIC0gY2hlY2sgdGhpcyBwYXRoXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgbGVmdFBhbmVsTWVudS5zZXRDb250ZW50VXJsKEN0aUZpbGVQYXRoVXRpbC5nZXRBYnNvbHV0ZVBhdGgoQ3RpTWVzc2FnaW5nQWRkaW5Jbml0aWFsaXplci5hZGRpbkZpbGVQYXRoKSk7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgbGVmdFBhbmVsTWVudS5zZXRWaXNpYmxlKGZhbHNlKTtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBsZWZ0UGFuZWxNZW51LnJlbmRlcigpO1xuICAgICAgICAgICAgICAgICAgICAgICAgfSk7XG4gICAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICApO1xuICAgICAgICAgICAgfSk7XG4gICAgICAgIH0pO1xuICAgIH1cbn1cblxuQ3RpTWVzc2FnaW5nQWRkaW5Jbml0aWFsaXplci5pbml0aWFsaXplKCk7Il19