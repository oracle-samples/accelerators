/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set 
 *  published by Oracle under the Universal Permissive License (UPL), Version 1.0
 *  Copyright (c) 2017 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: Telephony and SMS Accelerator for Twilio
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 17D (November 2017)
 *  reference: 161212-000059
 *  date: Monday Oct 30 13:8:16 UTC 2017
 *  revision: rnw-17-11-fixes-releases
 * 
 *  SHA1: $Id: 3ea7967f67f467ba32548deba803321df7d75cbf $
 * *********************************************************************************************
 *  File: messageConstants.js
 * ****************************************************************************************** */
define(["require", "exports"], function (require, exports) {
    "use strict";
    exports.__esModule = true;
    /**
     * All constants used by CTI Messaging Addin
     */
    var MessageConstants = /** @class */ (function () {
        function MessageConstants() {
        }
        MessageConstants.BUI_CTI_SMS_ADDIN_ID = 'bui_cti_sms_addin';
        MessageConstants.BUI_CTI_SMS_ADDIN_VERSION = '1.0';
        MessageConstants.BUI_CTI_LEFT_PANEL_SMS_MENU_ID = 'bui-cti-left-panel-sms-addin-10456824-65466';
        MessageConstants.BUI_CTI_LEFT_PANEL_SMS_ICON = 'fa fa-envelope-square';
        MessageConstants.BUI_CTI_LEFT_PANEL_SMS_ICON_COLOR = 'green';
        MessageConstants.BUI_CTI_LEFT_PANEL_SMS_MENU_DEFAULT_LABEL = 'Message';
        MessageConstants.BUI_CTI_SMS_ADDIN_ICON_TYPE = 'font awesome';
        return MessageConstants;
    }());
    exports.MessageConstants = MessageConstants;
});
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoibWVzc2FnZUNvbnN0YW50cy5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIm1lc3NhZ2VDb25zdGFudHMudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUE7Ozs7O2dHQUtnRzs7OztJQUVoRzs7T0FFRztJQUNIO1FBQUE7UUFRQSxDQUFDO1FBUGUscUNBQW9CLEdBQVcsbUJBQW1CLENBQUM7UUFDbkQsMENBQXlCLEdBQVcsS0FBSyxDQUFDO1FBQzFDLCtDQUE4QixHQUFXLDZDQUE2QyxDQUFDO1FBQ3ZGLDRDQUEyQixHQUFZLHVCQUF1QixDQUFDO1FBQy9ELGtEQUFpQyxHQUFXLE9BQU8sQ0FBQztRQUNwRCwwREFBeUMsR0FBWSxTQUFTLENBQUM7UUFDL0QsNENBQTJCLEdBQVcsY0FBYyxDQUFDO1FBQ3JFLHVCQUFDO0tBQUEsQUFSRCxJQVFDO0lBUlksNENBQWdCIiwic291cmNlc0NvbnRlbnQiOlsiLyogKiAqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqXG4gKiAgJEFDQ0VMRVJBVE9SX0hFQURFUl9QTEFDRV9IT0xERVIkXG4gKiAgU0hBMTogJElkOiAzZWE3OTY3ZjY3ZjQ2N2JhMzI1NDhkZWJhODAzMzIxZGY3ZDc1Y2JmICRcbiAqICoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKlxuICogIEZpbGU6ICRBQ0NFTEVSQVRPUl9IRUFERVJfRklMRV9OQU1FX1BMQUNFX0hPTERFUiRcbiAqICoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKiAqL1xuXG4vKipcbiAqIEFsbCBjb25zdGFudHMgdXNlZCBieSBDVEkgTWVzc2FnaW5nIEFkZGluXG4gKi9cbmV4cG9ydCBjbGFzcyBNZXNzYWdlQ29uc3RhbnRzIHtcbiAgcHVibGljIHN0YXRpYyBCVUlfQ1RJX1NNU19BRERJTl9JRDogc3RyaW5nID0gJ2J1aV9jdGlfc21zX2FkZGluJztcbiAgcHVibGljIHN0YXRpYyBCVUlfQ1RJX1NNU19BRERJTl9WRVJTSU9OOiBzdHJpbmcgPSAnMS4wJztcbiAgcHVibGljIHN0YXRpYyBCVUlfQ1RJX0xFRlRfUEFORUxfU01TX01FTlVfSUQ6IHN0cmluZyA9ICdidWktY3RpLWxlZnQtcGFuZWwtc21zLWFkZGluLTEwNDU2ODI0LTY1NDY2JztcbiAgcHVibGljIHN0YXRpYyBCVUlfQ1RJX0xFRlRfUEFORUxfU01TX0lDT046IHN0cmluZyAgPSAnZmEgZmEtZW52ZWxvcGUtc3F1YXJlJztcbiAgcHVibGljIHN0YXRpYyBCVUlfQ1RJX0xFRlRfUEFORUxfU01TX0lDT05fQ09MT1I6IHN0cmluZyA9ICdncmVlbic7XG4gIHB1YmxpYyBzdGF0aWMgQlVJX0NUSV9MRUZUX1BBTkVMX1NNU19NRU5VX0RFRkFVTFRfTEFCRUw6IHN0cmluZyAgPSAnTWVzc2FnZSc7XG4gIHB1YmxpYyBzdGF0aWMgQlVJX0NUSV9TTVNfQURESU5fSUNPTl9UWVBFOiBzdHJpbmcgPSAnZm9udCBhd2Vzb21lJztcbn0iXX0=