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
 *  SHA1: $Id: 7aee75fba9cc0c7e4bdcff10cad5c40a81d03253 $
 * *********************************************************************************************
 *  File: ctiLogger.js
 * ****************************************************************************************** */
define(["require", "exports", "./logLevels"], function (require, exports, logLevels_1) {
    "use strict";
    exports.__esModule = true;
    var CtiLogger = /** @class */ (function () {
        function CtiLogger() {
        }
        CtiLogger.logMessage = function (logLevel, message) {
            if (logLevel) {
                switch (logLevel) {
                    case logLevels_1.LogLevels.ERROR:
                        CtiLogger.logErrorMessage(message);
                        break;
                    case logLevels_1.LogLevels.INFO:
                        CtiLogger.logInfoMessage(message);
                        break;
                    case logLevels_1.LogLevels.WARN:
                        CtiLogger.logWarningMessage(message);
                }
            }
        };
        CtiLogger.logWarningMessage = function (message) {
            console.warn('CTILogger >> WARNING >> ' + message);
        };
        CtiLogger.logErrorMessage = function (message) {
            console.error('CTILogger >> ERROR >> ' + message);
        };
        CtiLogger.logInfoMessage = function (message) {
            console.log('CTILogger >> INFO >> ' + message);
        };
        return CtiLogger;
    }());
    exports.CtiLogger = CtiLogger;
});
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiY3RpTG9nZ2VyLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiY3RpTG9nZ2VyLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBOzs7OztnR0FLZ0c7Ozs7SUFHaEc7UUFBQTtRQTRCQSxDQUFDO1FBM0JpQixvQkFBVSxHQUF4QixVQUF5QixRQUFtQixFQUFFLE9BQWU7WUFDekQsRUFBRSxDQUFBLENBQUMsUUFBUSxDQUFDLENBQUMsQ0FBQztnQkFDVixNQUFNLENBQUMsQ0FBQyxRQUFRLENBQUMsQ0FBQSxDQUFDO29CQUNkLEtBQUsscUJBQVMsQ0FBQyxLQUFLO3dCQUNoQixTQUFTLENBQUMsZUFBZSxDQUFDLE9BQU8sQ0FBQyxDQUFDO3dCQUNuQyxLQUFLLENBQUM7b0JBQ1YsS0FBSyxxQkFBUyxDQUFDLElBQUk7d0JBQ2YsU0FBUyxDQUFDLGNBQWMsQ0FBQyxPQUFPLENBQUMsQ0FBQzt3QkFDbEMsS0FBSyxDQUFDO29CQUNWLEtBQUsscUJBQVMsQ0FBQyxJQUFJO3dCQUNmLFNBQVMsQ0FBQyxpQkFBaUIsQ0FBQyxPQUFPLENBQUMsQ0FBQztnQkFDN0MsQ0FBQztZQUNMLENBQUM7UUFDTCxDQUFDO1FBRWEsMkJBQWlCLEdBQS9CLFVBQWdDLE9BQWU7WUFDM0MsT0FBTyxDQUFDLElBQUksQ0FBQywwQkFBMEIsR0FBQyxPQUFPLENBQUMsQ0FBQztRQUNyRCxDQUFDO1FBRWEseUJBQWUsR0FBN0IsVUFBOEIsT0FBZTtZQUN6QyxPQUFPLENBQUMsS0FBSyxDQUFDLHdCQUF3QixHQUFDLE9BQU8sQ0FBQyxDQUFDO1FBQ3BELENBQUM7UUFFYSx3QkFBYyxHQUE1QixVQUE2QixPQUFlO1lBQ3hDLE9BQU8sQ0FBQyxHQUFHLENBQUMsdUJBQXVCLEdBQUMsT0FBTyxDQUFDLENBQUM7UUFDakQsQ0FBQztRQUVMLGdCQUFDO0lBQUQsQ0FBQyxBQTVCRCxJQTRCQztJQTVCWSw4QkFBUyIsInNvdXJjZXNDb250ZW50IjpbIi8qICogKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKlxuICogICRBQ0NFTEVSQVRPUl9IRUFERVJfUExBQ0VfSE9MREVSJFxuICogIFNIQTE6ICRJZDogN2FlZTc1ZmJhOWNjMGM3ZTRiZGNmZjEwY2FkNWM0MGE4MWQwMzI1MyAkXG4gKiAqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKipcbiAqICBGaWxlOiAkQUNDRUxFUkFUT1JfSEVBREVSX0ZJTEVfTkFNRV9QTEFDRV9IT0xERVIkXG4gKiAqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKiogKi9cblxuaW1wb3J0IHtMb2dMZXZlbHN9IGZyb20gXCIuL2xvZ0xldmVsc1wiO1xuZXhwb3J0IGNsYXNzIEN0aUxvZ2dlciB7XG4gICAgcHVibGljIHN0YXRpYyBsb2dNZXNzYWdlKGxvZ0xldmVsOiBMb2dMZXZlbHMsIG1lc3NhZ2U6IHN0cmluZyk6IHZvaWQge1xuICAgICAgICBpZihsb2dMZXZlbCkge1xuICAgICAgICAgICAgc3dpdGNoIChsb2dMZXZlbCl7XG4gICAgICAgICAgICAgICAgY2FzZSBMb2dMZXZlbHMuRVJST1I6XG4gICAgICAgICAgICAgICAgICAgIEN0aUxvZ2dlci5sb2dFcnJvck1lc3NhZ2UobWVzc2FnZSk7XG4gICAgICAgICAgICAgICAgICAgIGJyZWFrO1xuICAgICAgICAgICAgICAgIGNhc2UgTG9nTGV2ZWxzLklORk86XG4gICAgICAgICAgICAgICAgICAgIEN0aUxvZ2dlci5sb2dJbmZvTWVzc2FnZShtZXNzYWdlKTtcbiAgICAgICAgICAgICAgICAgICAgYnJlYWs7XG4gICAgICAgICAgICAgICAgY2FzZSBMb2dMZXZlbHMuV0FSTjpcbiAgICAgICAgICAgICAgICAgICAgQ3RpTG9nZ2VyLmxvZ1dhcm5pbmdNZXNzYWdlKG1lc3NhZ2UpO1xuICAgICAgICAgICAgfVxuICAgICAgICB9XG4gICAgfVxuXG4gICAgcHVibGljIHN0YXRpYyBsb2dXYXJuaW5nTWVzc2FnZShtZXNzYWdlOiBzdHJpbmcpOiB2b2lkIHtcbiAgICAgICAgY29uc29sZS53YXJuKCdDVElMb2dnZXIgPj4gV0FSTklORyA+PiAnK21lc3NhZ2UpO1xuICAgIH1cblxuICAgIHB1YmxpYyBzdGF0aWMgbG9nRXJyb3JNZXNzYWdlKG1lc3NhZ2U6IHN0cmluZyk6IHZvaWQge1xuICAgICAgICBjb25zb2xlLmVycm9yKCdDVElMb2dnZXIgPj4gRVJST1IgPj4gJyttZXNzYWdlKTtcbiAgICB9XG5cbiAgICBwdWJsaWMgc3RhdGljIGxvZ0luZm9NZXNzYWdlKG1lc3NhZ2U6IHN0cmluZyk6IHZvaWQge1xuICAgICAgICBjb25zb2xlLmxvZygnQ1RJTG9nZ2VyID4+IElORk8gPj4gJyttZXNzYWdlKTtcbiAgICB9XG5cbn0iXX0=