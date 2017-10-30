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
 *  SHA1: $Id: 3ceecf8f8d616d20f02d76855f70a67afed35df1 $
 * *********************************************************************************************
 *  File: pagination.js
 * ****************************************************************************************** */
define(["require", "exports"], function (require, exports) {
    "use strict";
    exports.__esModule = true;
    var Pagination = /** @class */ (function () {
        function Pagination() {
        }
        return Pagination;
    }());
    exports.Pagination = Pagination;
});
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoicGFnaW5hdGlvbi5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbInBhZ2luYXRpb24udHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUE7Ozs7O2dHQUtnRzs7OztJQUVoRztRQUFBO1FBTUEsQ0FBQztRQUFELGlCQUFDO0lBQUQsQ0FBQyxBQU5ELElBTUM7SUFOWSxnQ0FBVSIsInNvdXJjZXNDb250ZW50IjpbIi8qICogKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKlxuICogICRBQ0NFTEVSQVRPUl9IRUFERVJfUExBQ0VfSE9MREVSJFxuICogIFNIQTE6ICRJZDogM2NlZWNmOGY4ZDYxNmQyMGYwMmQ3Njg1NWY3MGE2N2FmZWQzNWRmMSAkXG4gKiAqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKipcbiAqICBGaWxlOiAkQUNDRUxFUkFUT1JfSEVBREVSX0ZJTEVfTkFNRV9QTEFDRV9IT0xERVIkXG4gKiAqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKiogKi9cblxuZXhwb3J0IGNsYXNzIFBhZ2luYXRpb24ge1xuICAgIGxvd2VyQm91bmQ6IG51bWJlcjtcbiAgICB1cHBlckJvdW5kOiBudW1iZXI7XG4gICAgbmV4dFBhZ2U6IG51bWJlcjtcbiAgICB0b3RhbFJlY29yZHM6IG51bWJlcjtcbiAgICByZWNvcmRzUGVyUGFnZTogbnVtYmVyO1xufSJdfQ==