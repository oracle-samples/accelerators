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
 *  SHA1: $Id: 7f293da59449c0ed8543a2b7835e3a88505224f3 $
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
        //CtiReportingAddin
        CtiMessages.MESSAGE_ADDIN_REGISTER = 'Initializing CTI Reporting Addin.';
        CtiMessages.MESSAGE_ADDIN_INITIALIZED = 'CTI Reporting addin initialized..';
        CtiMessages.MESSAGE_START_REPORT_EXECUTION = 'Initializing report execution..';
        CtiMessages.MESSAGE_GET_AGENT_DATA = 'Fetching agent statistics..';
        CtiMessages.MESSAGE_REPORT_EXECUTION_FAILED = 'Report execution failed due to ';
        CtiMessages.MESSAGE_NO_AGENTS_FOUND = 'No agent data found.';
        CtiMessages.MESSAGE_GET_AGENT_DATA_COMPLETED = 'Fetching agent data completed..';
        CtiMessages.MESSAGE_PROCESSING_DATA = 'Processing agent data..';
        CtiMessages.MESSAGE_PROCESSING_DATA_COMPLETED = 'Data processing completed.';
        return CtiMessages;
    }());
    exports.CtiMessages = CtiMessages;
});
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiY3RpTWVzc2FnZXMuanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyJjdGlNZXNzYWdlcy50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTs7Ozs7Z0dBS2dHOzs7O0lBRWhHO1FBQUE7UUFlQSxDQUFDO1FBYkcsUUFBUTtRQUNNLDRCQUFnQixHQUFXLE1BQU0sQ0FBQztRQUVoRCxtQkFBbUI7UUFDTCxrQ0FBc0IsR0FBVyxtQ0FBbUMsQ0FBQztRQUNyRSxxQ0FBeUIsR0FBVyxtQ0FBbUMsQ0FBQztRQUN4RSwwQ0FBOEIsR0FBVyxpQ0FBaUMsQ0FBQztRQUMzRSxrQ0FBc0IsR0FBVyw2QkFBNkIsQ0FBQztRQUMvRCwyQ0FBK0IsR0FBVyxpQ0FBaUMsQ0FBQztRQUM1RSxtQ0FBdUIsR0FBVyxzQkFBc0IsQ0FBQztRQUN6RCw0Q0FBZ0MsR0FBVyxpQ0FBaUMsQ0FBQztRQUM3RSxtQ0FBdUIsR0FBVyx5QkFBeUIsQ0FBQztRQUM1RCw2Q0FBaUMsR0FBVyw0QkFBNEIsQ0FBQztRQUMzRixrQkFBQztLQUFBLEFBZkQsSUFlQztJQWZZLGtDQUFXIiwic291cmNlc0NvbnRlbnQiOlsiLyogKiAqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqXG4gKiAgJEFDQ0VMRVJBVE9SX0hFQURFUl9QTEFDRV9IT0xERVIkXG4gKiAgU0hBMTogJElkOiA3ZjI5M2RhNTk0NDljMGVkODU0M2EyYjc4MzVlM2E4ODUwNTIyNGYzICRcbiAqICoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKlxuICogIEZpbGU6ICRBQ0NFTEVSQVRPUl9IRUFERVJfRklMRV9OQU1FX1BMQUNFX0hPTERFUiRcbiAqICoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKiAqL1xuXG5leHBvcnQgY2xhc3MgQ3RpTWVzc2FnZXMge1xuXG4gICAgLy9DT01NT05cbiAgICBwdWJsaWMgc3RhdGljIE1FU1NBR0VfQVBQRU5ERVI6IHN0cmluZyA9ICcgPj4gJztcblxuICAgIC8vQ3RpUmVwb3J0aW5nQWRkaW5cbiAgICBwdWJsaWMgc3RhdGljIE1FU1NBR0VfQURESU5fUkVHSVNURVI6IHN0cmluZyA9ICdJbml0aWFsaXppbmcgQ1RJIFJlcG9ydGluZyBBZGRpbi4nO1xuICAgIHB1YmxpYyBzdGF0aWMgTUVTU0FHRV9BRERJTl9JTklUSUFMSVpFRDogc3RyaW5nID0gJ0NUSSBSZXBvcnRpbmcgYWRkaW4gaW5pdGlhbGl6ZWQuLic7XG4gICAgcHVibGljIHN0YXRpYyBNRVNTQUdFX1NUQVJUX1JFUE9SVF9FWEVDVVRJT046IHN0cmluZyA9ICdJbml0aWFsaXppbmcgcmVwb3J0IGV4ZWN1dGlvbi4uJztcbiAgICBwdWJsaWMgc3RhdGljIE1FU1NBR0VfR0VUX0FHRU5UX0RBVEE6IHN0cmluZyA9ICdGZXRjaGluZyBhZ2VudCBzdGF0aXN0aWNzLi4nO1xuICAgIHB1YmxpYyBzdGF0aWMgTUVTU0FHRV9SRVBPUlRfRVhFQ1VUSU9OX0ZBSUxFRDogc3RyaW5nID0gJ1JlcG9ydCBleGVjdXRpb24gZmFpbGVkIGR1ZSB0byAnO1xuICAgIHB1YmxpYyBzdGF0aWMgTUVTU0FHRV9OT19BR0VOVFNfRk9VTkQ6IHN0cmluZyA9ICdObyBhZ2VudCBkYXRhIGZvdW5kLic7XG4gICAgcHVibGljIHN0YXRpYyBNRVNTQUdFX0dFVF9BR0VOVF9EQVRBX0NPTVBMRVRFRDogc3RyaW5nID0gJ0ZldGNoaW5nIGFnZW50IGRhdGEgY29tcGxldGVkLi4nO1xuICAgIHB1YmxpYyBzdGF0aWMgTUVTU0FHRV9QUk9DRVNTSU5HX0RBVEE6IHN0cmluZyA9ICdQcm9jZXNzaW5nIGFnZW50IGRhdGEuLic7XG4gICAgcHVibGljIHN0YXRpYyBNRVNTQUdFX1BST0NFU1NJTkdfREFUQV9DT01QTEVURUQ6IHN0cmluZyA9ICdEYXRhIHByb2Nlc3NpbmcgY29tcGxldGVkLic7XG59Il19