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
 *  SHA1: $Id: be943703537c7ef53ae27dd47d1a6db63412b6a0 $
 * *********************************************************************************************
 *  File: logLevels.js
 * ****************************************************************************************** */
define(["require", "exports"], function (require, exports) {
    "use strict";
    exports.__esModule = true;
    var LogLevels;
    (function (LogLevels) {
        LogLevels[LogLevels["INFO"] = 'INFO'] = "INFO";
        LogLevels[LogLevels["WARN"] = 'WARN'] = "WARN";
        LogLevels[LogLevels["ERROR"] = 'ERROR'] = "ERROR";
    })(LogLevels = exports.LogLevels || (exports.LogLevels = {}));
});
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoibG9nTGV2ZWxzLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsibG9nTGV2ZWxzLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBOzs7OztnR0FLZ0c7Ozs7SUFFaEcsSUFBWSxTQUlYO0lBSkQsV0FBWSxTQUFTO1FBQ2pCLDhCQUFZLE1BQU0sVUFBQSxDQUFBO1FBQ2xCLDhCQUFZLE1BQU0sVUFBQSxDQUFBO1FBQ2xCLCtCQUFhLE9BQU8sV0FBQSxDQUFBO0lBQ3hCLENBQUMsRUFKVyxTQUFTLEdBQVQsaUJBQVMsS0FBVCxpQkFBUyxRQUlwQiIsInNvdXJjZXNDb250ZW50IjpbIi8qICogKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKlxuICogICRBQ0NFTEVSQVRPUl9IRUFERVJfUExBQ0VfSE9MREVSJFxuICogIFNIQTE6ICRJZDogYmU5NDM3MDM1MzdjN2VmNTNhZTI3ZGQ0N2QxYTZkYjYzNDEyYjZhMCAkXG4gKiAqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKipcbiAqICBGaWxlOiAkQUNDRUxFUkFUT1JfSEVBREVSX0ZJTEVfTkFNRV9QTEFDRV9IT0xERVIkXG4gKiAqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKiogKi9cblxuZXhwb3J0IGVudW0gTG9nTGV2ZWxzIHtcbiAgICBJTkZPID0gPGFueT4nSU5GTycsXG4gICAgV0FSTiA9IDxhbnk+J1dBUk4nLFxuICAgIEVSUk9SID0gPGFueT4nRVJST1InXG59XG4iXX0=