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
 *  SHA1: $Id: 856f79ab1fe08da48005150723b04b6924fc9db4 $
 * *********************************************************************************************
 *  File: ctiConstants.js
 * ****************************************************************************************** */
define(["require", "exports"], function (require, exports) {
    "use strict";
    exports.__esModule = true;
    /**
     * All constants used by CTI Addin
     */
    var CtiConstants = /** @class */ (function () {
        function CtiConstants() {
        }
        CtiConstants.BUI_CTI_ADDIN_ID = 'bui_cti_addin';
        CtiConstants.BUI_CTI_ADDIN_VERSION = '1.0';
        CtiConstants.BUI_CTI_LEFT_PANEL_MENU_ID = 'bui-cti-left-panel-addin-10456824-65465';
        CtiConstants.BUI_CTI_LEFT_PANEL_MENU_DEFAULT_LABEL = 'BUI CTI ADD-IN';
        CtiConstants.BUI_CTI_LEFT_PANEL_ICON_TYPE = 'font awesome';
        CtiConstants.BUI_CTI_LEFT_PANEL_ICON = 'fa-phone-square';
        CtiConstants.BUI_CTI_LEFT_PANEL_ICON_NOTIFY = 'fa-phone-square flash-animated';
        CtiConstants.BUI_CTI_RIBBONBAR_ICON_WAIT = 'fa fa-hourglass-half flash-animated';
        CtiConstants.BUI_CTI_RIBBONBAR_MENU_ID = 'bui-cti-toolbar-menu-addin-900824-65465';
        CtiConstants.BUI_CTI_RIBBONBAR_ICON_TYPE = 'font awesome';
        CtiConstants.BUI_CTI_RIBBONBAR_ICON_DEFAULT_CLASS = 'fa fa-toggle-off';
        CtiConstants.BUI_CTI_RIBBONBAR_ICON_DEFAULT_COLOR = 'black';
        CtiConstants.BUI_CTI_ICON_CLASS_AVAILABLE = 'fa fa-toggle-on';
        CtiConstants.BUI_CTI_ICON_CLASS_NOT_AVAILABLE = 'fa fa-toggle-on';
        CtiConstants.BUI_CTI_ICON_CLASS_BUSY = 'fa fa-toggle-on';
        CtiConstants.BUI_CTI_LABEL_LOGGED_IN = 'Logged-in';
        CtiConstants.BUI_CTI_LABEL_LOGGED_OUT = 'Logged-out';
        CtiConstants.BUI_CTI_LABEL_AVAILABLE = 'Available';
        CtiConstants.BUI_CTI_LABEL_NOT_AVAILABLE = 'Not Available';
        CtiConstants.BUI_CTI_LABEL_BUSY = 'Busy';
        CtiConstants.BUI_CTI_LABEL_LOGIN = 'Login';
        CtiConstants.BUI_CTI_LABEL_LOGOUT = 'Logout';
        CtiConstants.BUI_CTI_LABEL_INCOMING_CALL = 'Incoming Call..';
        CtiConstants.BUI_CTI_LABEL_WAIT = 'Please wait..';
        CtiConstants.NOT_AVAILABLE = 'Offline';
        CtiConstants.AVAILABLE = 'Ready';
        CtiConstants.BUSY = 'Busy';
        CtiConstants.WAIT = 'Wait';
        CtiConstants.UNKNOWN_CALLER = 'Unknown Caller';
        CtiConstants.DEFAULT_DISPLAY_ICON = 'https://www.gravatar.com/avatar/23463b99b62a72f26ed677cc556c44e8?d=mm';
        return CtiConstants;
    }());
    exports.CtiConstants = CtiConstants;
});
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiY3RpQ29uc3RhbnRzLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiY3RpQ29uc3RhbnRzLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBOzs7OztnR0FLZ0c7Ozs7SUFFaEc7O09BRUc7SUFDSDtRQUFBO1FBc0NBLENBQUM7UUFwQ2UsNkJBQWdCLEdBQVcsZUFBZSxDQUFDO1FBQzNDLGtDQUFxQixHQUFZLEtBQUssQ0FBQztRQUN2Qyx1Q0FBMEIsR0FBWSx5Q0FBeUMsQ0FBQztRQUNoRixrREFBcUMsR0FBWSxnQkFBZ0IsQ0FBQztRQUNsRSx5Q0FBNEIsR0FBWSxjQUFjLENBQUM7UUFDdkQsb0NBQXVCLEdBQVksaUJBQWlCLENBQUM7UUFDckQsMkNBQThCLEdBQVksZ0NBQWdDLENBQUM7UUFDM0Usd0NBQTJCLEdBQVkscUNBQXFDLENBQUM7UUFFN0Usc0NBQXlCLEdBQVkseUNBQXlDLENBQUM7UUFDL0Usd0NBQTJCLEdBQVksY0FBYyxDQUFDO1FBQ3RELGlEQUFvQyxHQUFZLGtCQUFrQixDQUFDO1FBQ25FLGlEQUFvQyxHQUFZLE9BQU8sQ0FBQztRQUV4RCx5Q0FBNEIsR0FBWSxpQkFBaUIsQ0FBQztRQUMxRCw2Q0FBZ0MsR0FBWSxpQkFBaUIsQ0FBQztRQUM5RCxvQ0FBdUIsR0FBWSxpQkFBaUIsQ0FBQztRQUVyRCxvQ0FBdUIsR0FBWSxXQUFXLENBQUM7UUFDL0MscUNBQXdCLEdBQVksWUFBWSxDQUFDO1FBQ2pELG9DQUF1QixHQUFZLFdBQVcsQ0FBQztRQUMvQyx3Q0FBMkIsR0FBWSxlQUFlLENBQUM7UUFDdkQsK0JBQWtCLEdBQVksTUFBTSxDQUFDO1FBQ3JDLGdDQUFtQixHQUFZLE9BQU8sQ0FBQztRQUN2QyxpQ0FBb0IsR0FBWSxRQUFRLENBQUM7UUFDekMsd0NBQTJCLEdBQVksaUJBQWlCLENBQUM7UUFDekQsK0JBQWtCLEdBQVksZUFBZSxDQUFDO1FBRTlDLDBCQUFhLEdBQVksU0FBUyxDQUFDO1FBQ25DLHNCQUFTLEdBQVksT0FBTyxDQUFDO1FBQzdCLGlCQUFJLEdBQVksTUFBTSxDQUFDO1FBQ3ZCLGlCQUFJLEdBQVksTUFBTSxDQUFDO1FBRXZCLDJCQUFjLEdBQVcsZ0JBQWdCLENBQUM7UUFDMUMsaUNBQW9CLEdBQVcsdUVBQXVFLENBQUM7UUFFdkgsbUJBQUM7S0FBQSxBQXRDRCxJQXNDQztJQXRDWSxvQ0FBWSIsInNvdXJjZXNDb250ZW50IjpbIi8qICogKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKlxuICogICRBQ0NFTEVSQVRPUl9IRUFERVJfUExBQ0VfSE9MREVSJFxuICogIFNIQTE6ICRJZDogODU2Zjc5YWIxZmUwOGRhNDgwMDUxNTA3MjNiMDRiNjkyNGZjOWRiNCAkXG4gKiAqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKipcbiAqICBGaWxlOiAkQUNDRUxFUkFUT1JfSEVBREVSX0ZJTEVfTkFNRV9QTEFDRV9IT0xERVIkXG4gKiAqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKiogKi9cblxuLyoqXG4gKiBBbGwgY29uc3RhbnRzIHVzZWQgYnkgQ1RJIEFkZGluXG4gKi9cbmV4cG9ydCBjbGFzcyBDdGlDb25zdGFudHMge1xuICBcbiAgcHVibGljIHN0YXRpYyBCVUlfQ1RJX0FERElOX0lEOiBzdHJpbmcgPSAnYnVpX2N0aV9hZGRpbic7XG4gIHB1YmxpYyBzdGF0aWMgQlVJX0NUSV9BRERJTl9WRVJTSU9OOiBzdHJpbmcgID0gJzEuMCc7XG4gIHB1YmxpYyBzdGF0aWMgQlVJX0NUSV9MRUZUX1BBTkVMX01FTlVfSUQ6IHN0cmluZyAgPSAnYnVpLWN0aS1sZWZ0LXBhbmVsLWFkZGluLTEwNDU2ODI0LTY1NDY1JztcbiAgcHVibGljIHN0YXRpYyBCVUlfQ1RJX0xFRlRfUEFORUxfTUVOVV9ERUZBVUxUX0xBQkVMOiBzdHJpbmcgID0gJ0JVSSBDVEkgQURELUlOJztcbiAgcHVibGljIHN0YXRpYyBCVUlfQ1RJX0xFRlRfUEFORUxfSUNPTl9UWVBFOiBzdHJpbmcgID0gJ2ZvbnQgYXdlc29tZSc7XG4gIHB1YmxpYyBzdGF0aWMgQlVJX0NUSV9MRUZUX1BBTkVMX0lDT046IHN0cmluZyAgPSAnZmEtcGhvbmUtc3F1YXJlJztcbiAgcHVibGljIHN0YXRpYyBCVUlfQ1RJX0xFRlRfUEFORUxfSUNPTl9OT1RJRlk6IHN0cmluZyAgPSAnZmEtcGhvbmUtc3F1YXJlIGZsYXNoLWFuaW1hdGVkJztcbiAgcHVibGljIHN0YXRpYyBCVUlfQ1RJX1JJQkJPTkJBUl9JQ09OX1dBSVQ6IHN0cmluZyAgPSAnZmEgZmEtaG91cmdsYXNzLWhhbGYgZmxhc2gtYW5pbWF0ZWQnO1xuXG4gIHB1YmxpYyBzdGF0aWMgQlVJX0NUSV9SSUJCT05CQVJfTUVOVV9JRDogc3RyaW5nICA9ICdidWktY3RpLXRvb2xiYXItbWVudS1hZGRpbi05MDA4MjQtNjU0NjUnO1xuICBwdWJsaWMgc3RhdGljIEJVSV9DVElfUklCQk9OQkFSX0lDT05fVFlQRTogc3RyaW5nICA9ICdmb250IGF3ZXNvbWUnO1xuICBwdWJsaWMgc3RhdGljIEJVSV9DVElfUklCQk9OQkFSX0lDT05fREVGQVVMVF9DTEFTUzogc3RyaW5nICA9ICdmYSBmYS10b2dnbGUtb2ZmJztcbiAgcHVibGljIHN0YXRpYyBCVUlfQ1RJX1JJQkJPTkJBUl9JQ09OX0RFRkFVTFRfQ09MT1I6IHN0cmluZyAgPSAnYmxhY2snO1xuXG4gIHB1YmxpYyBzdGF0aWMgQlVJX0NUSV9JQ09OX0NMQVNTX0FWQUlMQUJMRTogc3RyaW5nICA9ICdmYSBmYS10b2dnbGUtb24nO1xuICBwdWJsaWMgc3RhdGljIEJVSV9DVElfSUNPTl9DTEFTU19OT1RfQVZBSUxBQkxFOiBzdHJpbmcgID0gJ2ZhIGZhLXRvZ2dsZS1vbic7XG4gIHB1YmxpYyBzdGF0aWMgQlVJX0NUSV9JQ09OX0NMQVNTX0JVU1k6IHN0cmluZyAgPSAnZmEgZmEtdG9nZ2xlLW9uJztcblxuICBwdWJsaWMgc3RhdGljIEJVSV9DVElfTEFCRUxfTE9HR0VEX0lOOiBzdHJpbmcgID0gJ0xvZ2dlZC1pbic7XG4gIHB1YmxpYyBzdGF0aWMgQlVJX0NUSV9MQUJFTF9MT0dHRURfT1VUOiBzdHJpbmcgID0gJ0xvZ2dlZC1vdXQnO1xuICBwdWJsaWMgc3RhdGljIEJVSV9DVElfTEFCRUxfQVZBSUxBQkxFOiBzdHJpbmcgID0gJ0F2YWlsYWJsZSc7XG4gIHB1YmxpYyBzdGF0aWMgQlVJX0NUSV9MQUJFTF9OT1RfQVZBSUxBQkxFOiBzdHJpbmcgID0gJ05vdCBBdmFpbGFibGUnO1xuICBwdWJsaWMgc3RhdGljIEJVSV9DVElfTEFCRUxfQlVTWTogc3RyaW5nICA9ICdCdXN5JztcbiAgcHVibGljIHN0YXRpYyBCVUlfQ1RJX0xBQkVMX0xPR0lOOiBzdHJpbmcgID0gJ0xvZ2luJztcbiAgcHVibGljIHN0YXRpYyBCVUlfQ1RJX0xBQkVMX0xPR09VVDogc3RyaW5nICA9ICdMb2dvdXQnO1xuICBwdWJsaWMgc3RhdGljIEJVSV9DVElfTEFCRUxfSU5DT01JTkdfQ0FMTDogc3RyaW5nICA9ICdJbmNvbWluZyBDYWxsLi4nO1xuICBwdWJsaWMgc3RhdGljIEJVSV9DVElfTEFCRUxfV0FJVDogc3RyaW5nICA9ICdQbGVhc2Ugd2FpdC4uJztcblxuICBwdWJsaWMgc3RhdGljIE5PVF9BVkFJTEFCTEU6IHN0cmluZyAgPSAnT2ZmbGluZSc7XG4gIHB1YmxpYyBzdGF0aWMgQVZBSUxBQkxFOiBzdHJpbmcgID0gJ1JlYWR5JztcbiAgcHVibGljIHN0YXRpYyBCVVNZOiBzdHJpbmcgID0gJ0J1c3knO1xuICBwdWJsaWMgc3RhdGljIFdBSVQ6IHN0cmluZyAgPSAnV2FpdCc7XG5cbiAgcHVibGljIHN0YXRpYyBVTktOT1dOX0NBTExFUjogc3RyaW5nID0gJ1Vua25vd24gQ2FsbGVyJztcbiAgcHVibGljIHN0YXRpYyBERUZBVUxUX0RJU1BMQVlfSUNPTjogc3RyaW5nID0gJ2h0dHBzOi8vd3d3LmdyYXZhdGFyLmNvbS9hdmF0YXIvMjM0NjNiOTliNjJhNzJmMjZlZDY3N2NjNTU2YzQ0ZTg/ZD1tbSc7XG4gIFxufSJdfQ==