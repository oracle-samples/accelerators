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
 *  SHA1: $Id: 609f4b24f615da6309858f594fceb6cdd91c4b09 $
 * *********************************************************************************************
 *  File: iCtiMessagingAdapter.js
 * ****************************************************************************************** */
define(["require", "exports"], function (require, exports) {
    "use strict";
    exports.__esModule = true;
});
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiaUN0aU1lc3NhZ2luZ0FkYXB0ZXIuanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyJpQ3RpTWVzc2FnaW5nQWRhcHRlci50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTs7Ozs7Z0dBS2dHIiwic291cmNlc0NvbnRlbnQiOlsiLyogKiAqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqXG4gKiAgJEFDQ0VMRVJBVE9SX0hFQURFUl9QTEFDRV9IT0xERVIkXG4gKiAgU0hBMTogJElkOiA2MDlmNGIyNGY2MTVkYTYzMDk4NThmNTk0ZmNlYjZjZGQ5MWM0YjA5ICRcbiAqICoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKlxuICogIEZpbGU6ICRBQ0NFTEVSQVRPUl9IRUFERVJfRklMRV9OQU1FX1BMQUNFX0hPTERFUiRcbiAqICoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKiAqL1xuXG5pbXBvcnQge01lc3NhZ2V9IGZyb20gXCIuLi9tb2RlbC9tZXNzYWdlXCI7XG5pbXBvcnQge0FnZW50UHJvZmlsZX0gZnJvbSBcIi4uL21vZGVsL2FnZW50UHJvZmlsZVwiO1xuZXhwb3J0IGludGVyZmFjZSBJQ3RpTWVzc2FnaW5nQWRhcHRlciB7XG5cbiAgICAvKipcbiAgICAgKiBJbXBsZW1lbnRhdGlvbiBzaG91bGQgc2VuZCBhIG1lc3NhZ2VcbiAgICAgKiBcbiAgICAgKiBAcGFyYW0gbWVzc2FnZVxuICAgICAqIEBwYXJhbSBwcm9maWxlRGF0YVxuICAgICAqL1xuICAgIHNlbmRNZXNzYWdlKG1lc3NhZ2U6IE1lc3NhZ2UscHJvZmlsZURhdGE6IEFnZW50UHJvZmlsZSk6IGFueTtcblxuICAgIC8qKlxuICAgICAqIEltcGxlbWVudGF0aW9uIHNob3VsZCBzZWFyY2ggZm9yIGEgY29udGFjdFxuICAgICAqXG4gICAgICogQHBhcmFtIGNJZFxuICAgICAqIEBwYXJhbSBzZXNzaW9uSWRcbiAgICAgKiBAcGFyYW0gc2VydmVyVXJpXG4gICAgICogQHBhcmFtIHBob25lXG4gICAgICovXG4gICAgc2VhcmNoQ29udGFjdChwaG9uZTogc3RyaW5nLCBjSWQ6IHN0cmluZywgc2Vzc2lvbklkOiBzdHJpbmcsIHNlcnZlclVyaTogc3RyaW5nKTogYW55O1xufSJdfQ==