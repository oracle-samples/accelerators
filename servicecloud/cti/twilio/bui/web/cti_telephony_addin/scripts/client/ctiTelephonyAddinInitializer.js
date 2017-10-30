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
 *  SHA1: $Id: 0432a2fc676de8f507bef8b05cf7a300da9c4e3e $
 * *********************************************************************************************
 *  File: ctiTelephonyAddinInitializer.js
 * ****************************************************************************************** */
define(["require", "exports", "./../util/ctiConstants"], function (require, exports, ctiConstants_1) {
    "use strict";
    exports.__esModule = true;
    /**
     * This class initializes the CTI Addin.
     * 1. It crates a left SidePane addin, set iit's content URL and set visibility
     *    to false, so that it will be rendered only after agent login to the tool
     *
     * 2. When the content is downloaded, it executes the CtiConsoleAddin.initialize()
     *    function which in turn render the GlobalHeaderMenu icon with the
     *    login option
     *
     */
    var CtiTelephonyAddinInitializer = /** @class */ (function () {
        function CtiTelephonyAddinInitializer() {
        }
        /**
         * This function calculates the absolute file path
         * for the addin html
         *
         * @returns {string}
         */
        CtiTelephonyAddinInitializer.getAbsolutePath = function () {
            var base = window.location.href;
            var relative = CtiTelephonyAddinInitializer.addinFilePath;
            var stack = base.split("/"), parts = relative.split("/");
            stack.pop();
            for (var i = 0; i < parts.length; i++) {
                if (parts[i] == ".")
                    continue;
                if (parts[i] == "..")
                    stack.pop();
                else
                    stack.push(parts[i]);
            }
            return stack.join("/");
        };
        /**
         * This is the entry point of CTI Addin.
         * This function initialize the sidepane addin, set content url and
         * set visibility to false.
         *
         */
        CtiTelephonyAddinInitializer.initialize = function () {
            ORACLE_SERVICE_CLOUD.extension_loader.load(ctiConstants_1.CtiConstants.BUI_CTI_ADDIN_ID, ctiConstants_1.CtiConstants.BUI_CTI_ADDIN_VERSION).then(function (sdk) {
                sdk.registerUserInterfaceExtension(function (userInterfaceContext) {
                    userInterfaceContext.getLeftSidePaneContext().then(function (leftSidePaneContext) {
                        leftSidePaneContext.getSidePane(ctiConstants_1.CtiConstants.BUI_CTI_LEFT_PANEL_MENU_ID).then(function (leftPanelMenu) {
                            leftPanelMenu.setContentUrl(CtiTelephonyAddinInitializer.getAbsolutePath());
                            leftPanelMenu.setLabel(ctiConstants_1.CtiConstants.BUI_CTI_LEFT_PANEL_MENU_DEFAULT_LABEL);
                            leftPanelMenu.setVisible(false);
                            var icon = leftPanelMenu.createIcon(ctiConstants_1.CtiConstants.BUI_CTI_LEFT_PANEL_ICON_TYPE);
                            icon.setIconClass(ctiConstants_1.CtiConstants.BUI_CTI_LEFT_PANEL_ICON);
                            leftPanelMenu.addIcon(icon);
                            leftPanelMenu.render();
                        });
                    });
                });
            });
        };
        CtiTelephonyAddinInitializer.addinFilePath = 'ctiTelephonyAddin.html';
        return CtiTelephonyAddinInitializer;
    }());
    exports.CtiTelephonyAddinInitializer = CtiTelephonyAddinInitializer;
    //Initialize the addin
    CtiTelephonyAddinInitializer.initialize();
});
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiY3RpVGVsZXBob255QWRkaW5Jbml0aWFsaXplci5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbImN0aVRlbGVwaG9ueUFkZGluSW5pdGlhbGl6ZXIudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUE7Ozs7O2dHQUtnRzs7OztJQUtoRzs7Ozs7Ozs7O09BU0c7SUFDSDtRQUFBO1FBbURBLENBQUM7UUFoREM7Ozs7O1dBS0c7UUFDWSw0Q0FBZSxHQUE5QjtZQUNFLElBQUksSUFBSSxHQUFXLE1BQU0sQ0FBQyxRQUFRLENBQUMsSUFBSSxDQUFDO1lBQ3hDLElBQUksUUFBUSxHQUFXLDRCQUE0QixDQUFDLGFBQWEsQ0FBQztZQUNsRSxJQUFJLEtBQUssR0FBRyxJQUFJLENBQUMsS0FBSyxDQUFDLEdBQUcsQ0FBQyxFQUN6QixLQUFLLEdBQUcsUUFBUSxDQUFDLEtBQUssQ0FBQyxHQUFHLENBQUMsQ0FBQztZQUM5QixLQUFLLENBQUMsR0FBRyxFQUFFLENBQUM7WUFDWixHQUFHLENBQUMsQ0FBQyxJQUFJLENBQUMsR0FBQyxDQUFDLEVBQUUsQ0FBQyxHQUFDLEtBQUssQ0FBQyxNQUFNLEVBQUUsQ0FBQyxFQUFFLEVBQUUsQ0FBQztnQkFDbEMsRUFBRSxDQUFDLENBQUMsS0FBSyxDQUFDLENBQUMsQ0FBQyxJQUFJLEdBQUcsQ0FBQztvQkFDbEIsUUFBUSxDQUFDO2dCQUNYLEVBQUUsQ0FBQyxDQUFDLEtBQUssQ0FBQyxDQUFDLENBQUMsSUFBSSxJQUFJLENBQUM7b0JBQ25CLEtBQUssQ0FBQyxHQUFHLEVBQUUsQ0FBQztnQkFDZCxJQUFJO29CQUNGLEtBQUssQ0FBQyxJQUFJLENBQUMsS0FBSyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUM7WUFDekIsQ0FBQztZQUNELE1BQU0sQ0FBQyxLQUFLLENBQUMsSUFBSSxDQUFDLEdBQUcsQ0FBQyxDQUFDO1FBQ3pCLENBQUM7UUFFRDs7Ozs7V0FLRztRQUNXLHVDQUFVLEdBQXhCO1lBQ0Usb0JBQW9CLENBQUMsZ0JBQWdCLENBQUMsSUFBSSxDQUFDLDJCQUFZLENBQUMsZ0JBQWdCLEVBQUUsMkJBQVksQ0FBQyxxQkFBcUIsQ0FBQyxDQUFDLElBQUksQ0FBQyxVQUFDLEdBQUc7Z0JBQ3JILEdBQUcsQ0FBQyw4QkFBOEIsQ0FBQyxVQUFDLG9CQUFvQjtvQkFDdEQsb0JBQW9CLENBQUMsc0JBQXNCLEVBQUUsQ0FBQyxJQUFJLENBQ2hELFVBQUMsbUJBQW1CO3dCQUNsQixtQkFBbUIsQ0FBQyxXQUFXLENBQUMsMkJBQVksQ0FBQywwQkFBMEIsQ0FBQyxDQUFDLElBQUksQ0FBQyxVQUFDLGFBQWE7NEJBQzFGLGFBQWEsQ0FBQyxhQUFhLENBQUMsNEJBQTRCLENBQUMsZUFBZSxFQUFFLENBQUMsQ0FBQzs0QkFDNUUsYUFBYSxDQUFDLFFBQVEsQ0FBQywyQkFBWSxDQUFDLHFDQUFxQyxDQUFDLENBQUM7NEJBQzNFLGFBQWEsQ0FBQyxVQUFVLENBQUMsS0FBSyxDQUFDLENBQUM7NEJBQ2hDLElBQUksSUFBSSxHQUFHLGFBQWEsQ0FBQyxVQUFVLENBQUMsMkJBQVksQ0FBQyw0QkFBNEIsQ0FBQyxDQUFDOzRCQUMvRSxJQUFJLENBQUMsWUFBWSxDQUFDLDJCQUFZLENBQUMsdUJBQXVCLENBQUMsQ0FBQzs0QkFDeEQsYUFBYSxDQUFDLE9BQU8sQ0FBQyxJQUFJLENBQUMsQ0FBQzs0QkFDNUIsYUFBYSxDQUFDLE1BQU0sRUFBRSxDQUFDO3dCQUN6QixDQUFDLENBQUMsQ0FBQztvQkFDTCxDQUFDLENBQ0YsQ0FBQztnQkFDSixDQUFDLENBQUMsQ0FBQztZQUNMLENBQUMsQ0FBQyxDQUFDO1FBQ0wsQ0FBQztRQWpEYywwQ0FBYSxHQUFVLHdCQUF3QixDQUFDO1FBa0RqRSxtQ0FBQztLQUFBLEFBbkRELElBbURDO0lBbkRZLG9FQUE0QjtJQXFEekMsc0JBQXNCO0lBQ3RCLDRCQUE0QixDQUFDLFVBQVUsRUFBRSxDQUFDIiwic291cmNlc0NvbnRlbnQiOlsiLyogKiAqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqXG4gKiAgJEFDQ0VMRVJBVE9SX0hFQURFUl9QTEFDRV9IT0xERVIkXG4gKiAgU0hBMTogJElkOiAwNDMyYTJmYzY3NmRlOGY1MDdiZWY4YjA1Y2Y3YTMwMGRhOWM0ZTNlICRcbiAqICoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKlxuICogIEZpbGU6ICRBQ0NFTEVSQVRPUl9IRUFERVJfRklMRV9OQU1FX1BMQUNFX0hPTERFUiRcbiAqICoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKiAqL1xuXG4vLy88cmVmZXJlbmNlIHBhdGg9Jy4uLy4uLy4uL2RlZmluaXRpb25zL29zdmNFeHRlbnNpb24uZC50cycgLz5cblxuaW1wb3J0IHtDdGlDb25zdGFudHN9IGZyb20gJy4vLi4vdXRpbC9jdGlDb25zdGFudHMnO1xuLyoqXG4gKiBUaGlzIGNsYXNzIGluaXRpYWxpemVzIHRoZSBDVEkgQWRkaW4uXG4gKiAxLiBJdCBjcmF0ZXMgYSBsZWZ0IFNpZGVQYW5lIGFkZGluLCBzZXQgaWl0J3MgY29udGVudCBVUkwgYW5kIHNldCB2aXNpYmlsaXR5XG4gKiAgICB0byBmYWxzZSwgc28gdGhhdCBpdCB3aWxsIGJlIHJlbmRlcmVkIG9ubHkgYWZ0ZXIgYWdlbnQgbG9naW4gdG8gdGhlIHRvb2xcbiAqXG4gKiAyLiBXaGVuIHRoZSBjb250ZW50IGlzIGRvd25sb2FkZWQsIGl0IGV4ZWN1dGVzIHRoZSBDdGlDb25zb2xlQWRkaW4uaW5pdGlhbGl6ZSgpXG4gKiAgICBmdW5jdGlvbiB3aGljaCBpbiB0dXJuIHJlbmRlciB0aGUgR2xvYmFsSGVhZGVyTWVudSBpY29uIHdpdGggdGhlXG4gKiAgICBsb2dpbiBvcHRpb25cbiAqXG4gKi9cbmV4cG9ydCBjbGFzcyBDdGlUZWxlcGhvbnlBZGRpbkluaXRpYWxpemVyIHtcbiAgcHJpdmF0ZSBzdGF0aWMgYWRkaW5GaWxlUGF0aDpzdHJpbmcgPSAnY3RpVGVsZXBob255QWRkaW4uaHRtbCc7XG5cbiAgLyoqXG4gICAqIFRoaXMgZnVuY3Rpb24gY2FsY3VsYXRlcyB0aGUgYWJzb2x1dGUgZmlsZSBwYXRoXG4gICAqIGZvciB0aGUgYWRkaW4gaHRtbFxuICAgKiBcbiAgICogQHJldHVybnMge3N0cmluZ31cbiAgICovXG4gIHByaXZhdGUgc3RhdGljIGdldEFic29sdXRlUGF0aCgpOiBzdHJpbmcge1xuICAgIHZhciBiYXNlOiBzdHJpbmcgPSB3aW5kb3cubG9jYXRpb24uaHJlZjtcbiAgICB2YXIgcmVsYXRpdmU6IHN0cmluZyA9IEN0aVRlbGVwaG9ueUFkZGluSW5pdGlhbGl6ZXIuYWRkaW5GaWxlUGF0aDtcbiAgICB2YXIgc3RhY2sgPSBiYXNlLnNwbGl0KFwiL1wiKSxcbiAgICAgIHBhcnRzID0gcmVsYXRpdmUuc3BsaXQoXCIvXCIpO1xuICAgIHN0YWNrLnBvcCgpO1xuICAgIGZvciAodmFyIGk9MDsgaTxwYXJ0cy5sZW5ndGg7IGkrKykge1xuICAgICAgaWYgKHBhcnRzW2ldID09IFwiLlwiKVxuICAgICAgICBjb250aW51ZTtcbiAgICAgIGlmIChwYXJ0c1tpXSA9PSBcIi4uXCIpXG4gICAgICAgIHN0YWNrLnBvcCgpO1xuICAgICAgZWxzZVxuICAgICAgICBzdGFjay5wdXNoKHBhcnRzW2ldKTtcbiAgICB9XG4gICAgcmV0dXJuIHN0YWNrLmpvaW4oXCIvXCIpO1xuICB9XG5cbiAgLyoqXG4gICAqIFRoaXMgaXMgdGhlIGVudHJ5IHBvaW50IG9mIENUSSBBZGRpbi5cbiAgICogVGhpcyBmdW5jdGlvbiBpbml0aWFsaXplIHRoZSBzaWRlcGFuZSBhZGRpbiwgc2V0IGNvbnRlbnQgdXJsIGFuZFxuICAgKiBzZXQgdmlzaWJpbGl0eSB0byBmYWxzZS5cbiAgICogXG4gICAqL1xuICBwdWJsaWMgc3RhdGljIGluaXRpYWxpemUoKTp2b2lkIHtcbiAgICBPUkFDTEVfU0VSVklDRV9DTE9VRC5leHRlbnNpb25fbG9hZGVyLmxvYWQoQ3RpQ29uc3RhbnRzLkJVSV9DVElfQURESU5fSUQsIEN0aUNvbnN0YW50cy5CVUlfQ1RJX0FERElOX1ZFUlNJT04pLnRoZW4oKHNkaykgPT4ge1xuICAgICAgc2RrLnJlZ2lzdGVyVXNlckludGVyZmFjZUV4dGVuc2lvbigodXNlckludGVyZmFjZUNvbnRleHQpID0+IHtcbiAgICAgICAgdXNlckludGVyZmFjZUNvbnRleHQuZ2V0TGVmdFNpZGVQYW5lQ29udGV4dCgpLnRoZW4oXG4gICAgICAgICAgKGxlZnRTaWRlUGFuZUNvbnRleHQpID0+IHtcbiAgICAgICAgICAgIGxlZnRTaWRlUGFuZUNvbnRleHQuZ2V0U2lkZVBhbmUoQ3RpQ29uc3RhbnRzLkJVSV9DVElfTEVGVF9QQU5FTF9NRU5VX0lEKS50aGVuKChsZWZ0UGFuZWxNZW51KSA9PiB7XG4gICAgICAgICAgICAgIGxlZnRQYW5lbE1lbnUuc2V0Q29udGVudFVybChDdGlUZWxlcGhvbnlBZGRpbkluaXRpYWxpemVyLmdldEFic29sdXRlUGF0aCgpKTtcbiAgICAgICAgICAgICAgbGVmdFBhbmVsTWVudS5zZXRMYWJlbChDdGlDb25zdGFudHMuQlVJX0NUSV9MRUZUX1BBTkVMX01FTlVfREVGQVVMVF9MQUJFTCk7XG4gICAgICAgICAgICAgIGxlZnRQYW5lbE1lbnUuc2V0VmlzaWJsZShmYWxzZSk7XG4gICAgICAgICAgICAgIHZhciBpY29uID0gbGVmdFBhbmVsTWVudS5jcmVhdGVJY29uKEN0aUNvbnN0YW50cy5CVUlfQ1RJX0xFRlRfUEFORUxfSUNPTl9UWVBFKTtcbiAgICAgICAgICAgICAgaWNvbi5zZXRJY29uQ2xhc3MoQ3RpQ29uc3RhbnRzLkJVSV9DVElfTEVGVF9QQU5FTF9JQ09OKTtcbiAgICAgICAgICAgICAgbGVmdFBhbmVsTWVudS5hZGRJY29uKGljb24pO1xuICAgICAgICAgICAgICBsZWZ0UGFuZWxNZW51LnJlbmRlcigpO1xuICAgICAgICAgICAgfSk7XG4gICAgICAgICAgfVxuICAgICAgICApO1xuICAgICAgfSk7XG4gICAgfSk7XG4gIH1cbn1cblxuLy9Jbml0aWFsaXplIHRoZSBhZGRpblxuQ3RpVGVsZXBob255QWRkaW5Jbml0aWFsaXplci5pbml0aWFsaXplKCk7Il19