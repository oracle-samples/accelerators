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
 *  SHA1: $Id: a09d3563f373bbf8240f2b130334c5e3af8b99f2 $
 * *********************************************************************************************
 *  File: ctiMessages.js
 * ****************************************************************************************** */
define(["require", "exports"], function (require, exports) {
    "use strict";
    exports.__esModule = true;
    var CtiMessages = /** @class */ (function () {
        function CtiMessages() {
        }
        //COMMON
        CtiMessages.MESSAGE_APPENDER = ' >> ';
        //CtiMessagingAddin
        CtiMessages.MESSAGE_ADDIN_INITIALIZED = 'CTI Messaging addin initialized.';
        CtiMessages.MESSAGE_RETRY_CONTACT_FETCH = 'Retry fetching contact details ';
        CtiMessages.MESSAGE_SEARCH_CONTACT = 'Searching for contact..';
        CtiMessages.MESSAGE_SEARCH_COMPLETE = 'Contact search completed.';
        CtiMessages.MESSAGE_SEARCH_FAILED = 'Contact search has failed.';
        CtiMessages.MESSAGE_ENABLE_SMS_OPTION = 'Enabling SMS option..';
        CtiMessages.MESSAGE_SEND_SMS = 'Sending message ..';
        CtiMessages.MESSAGE_SEND_SMS_SUCCESS = 'SMS sent successfully';
        CtiMessages.MESSAGE_SEND_SMS_FAILED = 'Sending SMS failed';
        return CtiMessages;
    }());
    exports.CtiMessages = CtiMessages;
});
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiY3RpTWVzc2FnZXMuanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyJjdGlNZXNzYWdlcy50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTs7Ozs7Z0dBS2dHOzs7O0lBRWhHO1FBQUE7UUFlQSxDQUFDO1FBYkcsUUFBUTtRQUNNLDRCQUFnQixHQUFXLE1BQU0sQ0FBQztRQUVoRCxtQkFBbUI7UUFDTCxxQ0FBeUIsR0FBVyxrQ0FBa0MsQ0FBQztRQUN2RSx1Q0FBMkIsR0FBVyxpQ0FBaUMsQ0FBQztRQUN4RSxrQ0FBc0IsR0FBVyx5QkFBeUIsQ0FBQztRQUMzRCxtQ0FBdUIsR0FBVywyQkFBMkIsQ0FBQztRQUM5RCxpQ0FBcUIsR0FBVyw0QkFBNEIsQ0FBQztRQUM3RCxxQ0FBeUIsR0FBVyx1QkFBdUIsQ0FBQztRQUM1RCw0QkFBZ0IsR0FBVyxvQkFBb0IsQ0FBQztRQUNoRCxvQ0FBd0IsR0FBRyx1QkFBdUIsQ0FBQztRQUNuRCxtQ0FBdUIsR0FBRyxvQkFBb0IsQ0FBQztRQUNqRSxrQkFBQztLQUFBLEFBZkQsSUFlQztJQWZZLGtDQUFXIiwic291cmNlc0NvbnRlbnQiOlsiLyogKiAqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqXG4gKiAgJEFDQ0VMRVJBVE9SX0hFQURFUl9QTEFDRV9IT0xERVIkXG4gKiAgU0hBMTogJElkOiBhMDlkMzU2M2YzNzNiYmY4MjQwZjJiMTMwMzM0YzVlM2FmOGI5OWYyICRcbiAqICoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKlxuICogIEZpbGU6ICRBQ0NFTEVSQVRPUl9IRUFERVJfRklMRV9OQU1FX1BMQUNFX0hPTERFUiRcbiAqICoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKiAqL1xuXG5leHBvcnQgY2xhc3MgQ3RpTWVzc2FnZXMge1xuXG4gICAgLy9DT01NT05cbiAgICBwdWJsaWMgc3RhdGljIE1FU1NBR0VfQVBQRU5ERVI6IHN0cmluZyA9ICcgPj4gJztcblxuICAgIC8vQ3RpTWVzc2FnaW5nQWRkaW5cbiAgICBwdWJsaWMgc3RhdGljIE1FU1NBR0VfQURESU5fSU5JVElBTElaRUQ6IHN0cmluZyA9ICdDVEkgTWVzc2FnaW5nIGFkZGluIGluaXRpYWxpemVkLic7XG4gICAgcHVibGljIHN0YXRpYyBNRVNTQUdFX1JFVFJZX0NPTlRBQ1RfRkVUQ0g6IHN0cmluZyA9ICdSZXRyeSBmZXRjaGluZyBjb250YWN0IGRldGFpbHMgJztcbiAgICBwdWJsaWMgc3RhdGljIE1FU1NBR0VfU0VBUkNIX0NPTlRBQ1Q6IHN0cmluZyA9ICdTZWFyY2hpbmcgZm9yIGNvbnRhY3QuLic7XG4gICAgcHVibGljIHN0YXRpYyBNRVNTQUdFX1NFQVJDSF9DT01QTEVURTogc3RyaW5nID0gJ0NvbnRhY3Qgc2VhcmNoIGNvbXBsZXRlZC4nO1xuICAgIHB1YmxpYyBzdGF0aWMgTUVTU0FHRV9TRUFSQ0hfRkFJTEVEOiBzdHJpbmcgPSAnQ29udGFjdCBzZWFyY2ggaGFzIGZhaWxlZC4nO1xuICAgIHB1YmxpYyBzdGF0aWMgTUVTU0FHRV9FTkFCTEVfU01TX09QVElPTjogc3RyaW5nID0gJ0VuYWJsaW5nIFNNUyBvcHRpb24uLic7XG4gICAgcHVibGljIHN0YXRpYyBNRVNTQUdFX1NFTkRfU01TOiBzdHJpbmcgPSAnU2VuZGluZyBtZXNzYWdlIC4uJztcbiAgICBwdWJsaWMgc3RhdGljIE1FU1NBR0VfU0VORF9TTVNfU1VDQ0VTUyA9ICdTTVMgc2VudCBzdWNjZXNzZnVsbHknO1xuICAgIHB1YmxpYyBzdGF0aWMgTUVTU0FHRV9TRU5EX1NNU19GQUlMRUQgPSAnU2VuZGluZyBTTVMgZmFpbGVkJztcbn0iXX0=