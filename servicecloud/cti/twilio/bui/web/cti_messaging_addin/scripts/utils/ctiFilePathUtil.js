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
 *  SHA1: $Id: fe328afe56a452fb129eee8cc97f84c6731b8b75 $
 * *********************************************************************************************
 *  File: ctiFilePathUtil.js
 * ****************************************************************************************** */
define(["require", "exports"], function (require, exports) {
    "use strict";
    exports.__esModule = true;
    var CtiFilePathUtil = /** @class */ (function () {
        function CtiFilePathUtil() {
        }
        /**
         * This function calculates the absolute file path
         * for the addin html
         *
         * @returns {string}
         */
        CtiFilePathUtil.getAbsolutePath = function (addinFilePath) {
            var base = window.location.href;
            var relative = addinFilePath;
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
        return CtiFilePathUtil;
    }());
    exports.CtiFilePathUtil = CtiFilePathUtil;
});
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiY3RpRmlsZVBhdGhVdGlsLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiY3RpRmlsZVBhdGhVdGlsLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBOzs7OztnR0FLZ0c7Ozs7SUFFaEc7UUFBQTtRQXdCQSxDQUFDO1FBdkJHOzs7OztXQUtHO1FBRVcsK0JBQWUsR0FBN0IsVUFBOEIsYUFBcUI7WUFDL0MsSUFBSSxJQUFJLEdBQVcsTUFBTSxDQUFDLFFBQVEsQ0FBQyxJQUFJLENBQUM7WUFDeEMsSUFBSSxRQUFRLEdBQVcsYUFBYSxDQUFDO1lBQ3JDLElBQUksS0FBSyxHQUFHLElBQUksQ0FBQyxLQUFLLENBQUMsR0FBRyxDQUFDLEVBQ3ZCLEtBQUssR0FBRyxRQUFRLENBQUMsS0FBSyxDQUFDLEdBQUcsQ0FBQyxDQUFDO1lBQ2hDLEtBQUssQ0FBQyxHQUFHLEVBQUUsQ0FBQztZQUNaLEdBQUcsQ0FBQyxDQUFDLElBQUksQ0FBQyxHQUFDLENBQUMsRUFBRSxDQUFDLEdBQUMsS0FBSyxDQUFDLE1BQU0sRUFBRSxDQUFDLEVBQUUsRUFBRSxDQUFDO2dCQUNoQyxFQUFFLENBQUMsQ0FBQyxLQUFLLENBQUMsQ0FBQyxDQUFDLElBQUksR0FBRyxDQUFDO29CQUNoQixRQUFRLENBQUM7Z0JBQ2IsRUFBRSxDQUFDLENBQUMsS0FBSyxDQUFDLENBQUMsQ0FBQyxJQUFJLElBQUksQ0FBQztvQkFDakIsS0FBSyxDQUFDLEdBQUcsRUFBRSxDQUFDO2dCQUNoQixJQUFJO29CQUNBLEtBQUssQ0FBQyxJQUFJLENBQUMsS0FBSyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUM7WUFDN0IsQ0FBQztZQUNELE1BQU0sQ0FBQyxLQUFLLENBQUMsSUFBSSxDQUFDLEdBQUcsQ0FBQyxDQUFDO1FBQzNCLENBQUM7UUFDTCxzQkFBQztJQUFELENBQUMsQUF4QkQsSUF3QkM7SUF4QlksMENBQWUiLCJzb3VyY2VzQ29udGVudCI6WyIvKiAqICoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKipcbiAqICAkQUNDRUxFUkFUT1JfSEVBREVSX1BMQUNFX0hPTERFUiRcbiAqICBTSEExOiAkSWQ6IGZlMzI4YWZlNTZhNDUyZmIxMjllZWU4Y2M5N2Y4NGM2NzMxYjhiNzUgJFxuICogKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqXG4gKiAgRmlsZTogJEFDQ0VMRVJBVE9SX0hFQURFUl9GSUxFX05BTUVfUExBQ0VfSE9MREVSJFxuICogKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqICovXG5cbmV4cG9ydCBjbGFzcyBDdGlGaWxlUGF0aFV0aWwge1xuICAgIC8qKlxuICAgICAqIFRoaXMgZnVuY3Rpb24gY2FsY3VsYXRlcyB0aGUgYWJzb2x1dGUgZmlsZSBwYXRoXG4gICAgICogZm9yIHRoZSBhZGRpbiBodG1sXG4gICAgICpcbiAgICAgKiBAcmV0dXJucyB7c3RyaW5nfVxuICAgICAqL1xuXG4gICAgcHVibGljIHN0YXRpYyBnZXRBYnNvbHV0ZVBhdGgoYWRkaW5GaWxlUGF0aDogc3RyaW5nKTogc3RyaW5nIHtcbiAgICAgICAgdmFyIGJhc2U6IHN0cmluZyA9IHdpbmRvdy5sb2NhdGlvbi5ocmVmO1xuICAgICAgICB2YXIgcmVsYXRpdmU6IHN0cmluZyA9IGFkZGluRmlsZVBhdGg7XG4gICAgICAgIHZhciBzdGFjayA9IGJhc2Uuc3BsaXQoXCIvXCIpLFxuICAgICAgICAgICAgcGFydHMgPSByZWxhdGl2ZS5zcGxpdChcIi9cIik7XG4gICAgICAgIHN0YWNrLnBvcCgpO1xuICAgICAgICBmb3IgKHZhciBpPTA7IGk8cGFydHMubGVuZ3RoOyBpKyspIHtcbiAgICAgICAgICAgIGlmIChwYXJ0c1tpXSA9PSBcIi5cIilcbiAgICAgICAgICAgICAgICBjb250aW51ZTtcbiAgICAgICAgICAgIGlmIChwYXJ0c1tpXSA9PSBcIi4uXCIpXG4gICAgICAgICAgICAgICAgc3RhY2sucG9wKCk7XG4gICAgICAgICAgICBlbHNlXG4gICAgICAgICAgICAgICAgc3RhY2sucHVzaChwYXJ0c1tpXSk7XG4gICAgICAgIH1cbiAgICAgICAgcmV0dXJuIHN0YWNrLmpvaW4oXCIvXCIpO1xuICAgIH1cbn0iXX0=