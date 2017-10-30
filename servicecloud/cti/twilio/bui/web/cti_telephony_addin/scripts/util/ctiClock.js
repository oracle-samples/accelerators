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
 *  SHA1: $Id: b3c07ff2750e7b693d477e22315ca27ce2ec8590 $
 * *********************************************************************************************
 *  File: ctiClock.js
 * ****************************************************************************************** */
define(["require", "exports", "jquery"], function (require, exports, $) {
    "use strict";
    exports.__esModule = true;
    var CtiClock = /** @class */ (function () {
        function CtiClock(elementId) {
            this.ctiToken = 'ORACLE_OSVC_CTI';
            this.callDuration = '00:00:00';
            this.isRunning = false;
            this.elementId = elementId;
        }
        CtiClock.prototype.startClock = function () {
            var _this = this;
            this.callStartTime = new Date();
            this.isRunning = true;
            if (typeof (Worker) !== "undefined") {
                this.worker = new Worker('../scripts/util/ctiClockWorker.js');
                this.worker.onmessage = function (event) {
                    if (event && event.data) {
                        var data = JSON.parse(event.data);
                        if (data.token === _this.ctiToken) {
                            _this.callDuration = data.duration;
                            _this.callLength = data.duration;
                            $('#' + _this.elementId).html(data.duration);
                        }
                    }
                };
                this.worker.postMessage(JSON.stringify({ token: this.ctiToken, command: 'START' }));
            }
        };
        CtiClock.prototype.resetUI = function () {
            $('#' + this.elementId).html(this.callDuration);
        };
        CtiClock.prototype.stopClock = function () {
            if (this.isRunning) {
                this.worker.terminate();
                this.callEndTime = new Date();
                this.callLength = this.callDuration;
                this.callDuration = '00:00:00';
                $('#' + this.elementId).html(this.callLength);
                this.isRunning = false;
            }
        };
        /**
         * This method returns the call duration as a string
         *
         * @returns {string}
         */
        CtiClock.prototype.getCallLength = function () {
            return this.callLength;
        };
        /**
         * This method returns the start time of the clock as a date object
         * @returns {Date}
         */
        CtiClock.prototype.getClockStartTime = function () {
            return this.callStartTime;
        };
        /**
         * This method returns the end time of clock as a date object
         *
         * @returns {Date}
         */
        CtiClock.prototype.getClockEndTime = function () {
            return this.callEndTime ? this.callEndTime : new Date();
        };
        return CtiClock;
    }());
    exports.CtiClock = CtiClock;
});
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiY3RpQ2xvY2suanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyJjdGlDbG9jay50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTs7Ozs7Z0dBS2dHOzs7O0lBSWhHO1FBVUksa0JBQW1CLFNBQWlCO1lBSjVCLGFBQVEsR0FBVyxpQkFBaUIsQ0FBQztZQUNyQyxpQkFBWSxHQUFXLFVBQVUsQ0FBQztZQUNsQyxjQUFTLEdBQVksS0FBSyxDQUFDO1lBRy9CLElBQUksQ0FBQyxTQUFTLEdBQUcsU0FBUyxDQUFDO1FBQy9CLENBQUM7UUFFTSw2QkFBVSxHQUFqQjtZQUFBLGlCQWtCQztZQWpCRyxJQUFJLENBQUMsYUFBYSxHQUFHLElBQUksSUFBSSxFQUFFLENBQUM7WUFDaEMsSUFBSSxDQUFDLFNBQVMsR0FBRyxJQUFJLENBQUM7WUFDdEIsRUFBRSxDQUFBLENBQUMsT0FBTSxDQUFDLE1BQU0sQ0FBQyxLQUFLLFdBQVcsQ0FBQyxDQUFDLENBQUM7Z0JBQ2hDLElBQUksQ0FBQyxNQUFNLEdBQUcsSUFBSSxNQUFNLENBQUMsbUNBQW1DLENBQUMsQ0FBQztnQkFDOUQsSUFBSSxDQUFDLE1BQU0sQ0FBQyxTQUFTLEdBQUcsVUFBQyxLQUFVO29CQUMvQixFQUFFLENBQUEsQ0FBQyxLQUFLLElBQUksS0FBSyxDQUFDLElBQUksQ0FBQyxDQUFBLENBQUM7d0JBQ3BCLElBQUksSUFBSSxHQUFRLElBQUksQ0FBQyxLQUFLLENBQUMsS0FBSyxDQUFDLElBQUksQ0FBQyxDQUFDO3dCQUN2QyxFQUFFLENBQUEsQ0FBQyxJQUFJLENBQUMsS0FBSyxLQUFLLEtBQUksQ0FBQyxRQUFRLENBQUMsQ0FBQSxDQUFDOzRCQUM3QixLQUFJLENBQUMsWUFBWSxHQUFHLElBQUksQ0FBQyxRQUFRLENBQUM7NEJBQ2xDLEtBQUksQ0FBQyxVQUFVLEdBQUcsSUFBSSxDQUFDLFFBQVEsQ0FBQzs0QkFDaEMsQ0FBQyxDQUFDLEdBQUcsR0FBQyxLQUFJLENBQUMsU0FBUyxDQUFDLENBQUMsSUFBSSxDQUFDLElBQUksQ0FBQyxRQUFRLENBQUMsQ0FBQzt3QkFDOUMsQ0FBQztvQkFDTCxDQUFDO2dCQUNMLENBQUMsQ0FBQztnQkFFRixJQUFJLENBQUMsTUFBTSxDQUFDLFdBQVcsQ0FBQyxJQUFJLENBQUMsU0FBUyxDQUFDLEVBQUMsS0FBSyxFQUFFLElBQUksQ0FBQyxRQUFRLEVBQUUsT0FBTyxFQUFFLE9BQU8sRUFBQyxDQUFDLENBQUMsQ0FBQztZQUN0RixDQUFDO1FBQ0wsQ0FBQztRQUVNLDBCQUFPLEdBQWQ7WUFDSSxDQUFDLENBQUMsR0FBRyxHQUFDLElBQUksQ0FBQyxTQUFTLENBQUMsQ0FBQyxJQUFJLENBQUMsSUFBSSxDQUFDLFlBQVksQ0FBQyxDQUFDO1FBQ2xELENBQUM7UUFFTSw0QkFBUyxHQUFoQjtZQUNJLEVBQUUsQ0FBQSxDQUFDLElBQUksQ0FBQyxTQUFTLENBQUMsQ0FBQyxDQUFDO2dCQUNoQixJQUFJLENBQUMsTUFBTSxDQUFDLFNBQVMsRUFBRSxDQUFDO2dCQUN4QixJQUFJLENBQUMsV0FBVyxHQUFHLElBQUksSUFBSSxFQUFFLENBQUM7Z0JBQzlCLElBQUksQ0FBQyxVQUFVLEdBQUcsSUFBSSxDQUFDLFlBQVksQ0FBQztnQkFDcEMsSUFBSSxDQUFDLFlBQVksR0FBRyxVQUFVLENBQUM7Z0JBQy9CLENBQUMsQ0FBQyxHQUFHLEdBQUMsSUFBSSxDQUFDLFNBQVMsQ0FBQyxDQUFDLElBQUksQ0FBQyxJQUFJLENBQUMsVUFBVSxDQUFDLENBQUM7Z0JBQzVDLElBQUksQ0FBQyxTQUFTLEdBQUcsS0FBSyxDQUFDO1lBQzNCLENBQUM7UUFDTCxDQUFDO1FBRUQ7Ozs7V0FJRztRQUNJLGdDQUFhLEdBQXBCO1lBQ0ksTUFBTSxDQUFDLElBQUksQ0FBQyxVQUFVLENBQUM7UUFDM0IsQ0FBQztRQUVEOzs7V0FHRztRQUNJLG9DQUFpQixHQUF4QjtZQUNJLE1BQU0sQ0FBQyxJQUFJLENBQUMsYUFBYSxDQUFDO1FBQzlCLENBQUM7UUFFRDs7OztXQUlHO1FBQ0ksa0NBQWUsR0FBdEI7WUFDSSxNQUFNLENBQUMsSUFBSSxDQUFDLFdBQVcsQ0FBQSxDQUFDLENBQUMsSUFBSSxDQUFDLFdBQVcsQ0FBQyxDQUFDLENBQUMsSUFBSSxJQUFJLEVBQUUsQ0FBQztRQUMzRCxDQUFDO1FBQ0wsZUFBQztJQUFELENBQUMsQUExRUQsSUEwRUM7SUExRVksNEJBQVEiLCJzb3VyY2VzQ29udGVudCI6WyIvKiAqICoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKipcbiAqICAkQUNDRUxFUkFUT1JfSEVBREVSX1BMQUNFX0hPTERFUiRcbiAqICBTSEExOiAkSWQ6IGIzYzA3ZmYyNzUwZTdiNjkzZDQ3N2UyMjMxNWNhMjdjZTJlYzg1OTAgJFxuICogKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqXG4gKiAgRmlsZTogJEFDQ0VMRVJBVE9SX0hFQURFUl9GSUxFX05BTUVfUExBQ0VfSE9MREVSJFxuICogKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqICovXG5cbmltcG9ydCAkID0gcmVxdWlyZSgnanF1ZXJ5Jyk7XG5cbmV4cG9ydCBjbGFzcyBDdGlDbG9jayB7XG4gICAgcHJpdmF0ZSB3b3JrZXI6IFdvcmtlcjtcbiAgICBwcml2YXRlIGVsZW1lbnRJZDogc3RyaW5nO1xuICAgIHByaXZhdGUgY2FsbExlbmd0aDogc3RyaW5nO1xuICAgIHByaXZhdGUgY2FsbFN0YXJ0VGltZTogRGF0ZTtcbiAgICBwcml2YXRlIGNhbGxFbmRUaW1lOiBEYXRlO1xuICAgIHByaXZhdGUgY3RpVG9rZW46IHN0cmluZyA9ICdPUkFDTEVfT1NWQ19DVEknO1xuICAgIHByaXZhdGUgY2FsbER1cmF0aW9uOiBzdHJpbmcgPSAnMDA6MDA6MDAnO1xuICAgIHByaXZhdGUgaXNSdW5uaW5nOiBib29sZWFuID0gZmFsc2U7XG4gICAgXG4gICAgcHVibGljIGNvbnN0cnVjdG9yKGVsZW1lbnRJZDogc3RyaW5nKSB7XG4gICAgICAgIHRoaXMuZWxlbWVudElkID0gZWxlbWVudElkO1xuICAgIH1cblxuICAgIHB1YmxpYyBzdGFydENsb2NrKCk6IHZvaWQge1xuICAgICAgICB0aGlzLmNhbGxTdGFydFRpbWUgPSBuZXcgRGF0ZSgpO1xuICAgICAgICB0aGlzLmlzUnVubmluZyA9IHRydWU7XG4gICAgICAgIGlmKHR5cGVvZihXb3JrZXIpICE9PSBcInVuZGVmaW5lZFwiKSB7XG4gICAgICAgICAgICB0aGlzLndvcmtlciA9IG5ldyBXb3JrZXIoJy4uL3NjcmlwdHMvdXRpbC9jdGlDbG9ja1dvcmtlci5qcycpO1xuICAgICAgICAgICAgdGhpcy53b3JrZXIub25tZXNzYWdlID0gKGV2ZW50OiBhbnkpID0+IHtcbiAgICAgICAgICAgICAgICBpZihldmVudCAmJiBldmVudC5kYXRhKXtcbiAgICAgICAgICAgICAgICAgICAgdmFyIGRhdGE6IGFueSA9IEpTT04ucGFyc2UoZXZlbnQuZGF0YSk7XG4gICAgICAgICAgICAgICAgICAgIGlmKGRhdGEudG9rZW4gPT09IHRoaXMuY3RpVG9rZW4pe1xuICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5jYWxsRHVyYXRpb24gPSBkYXRhLmR1cmF0aW9uO1xuICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5jYWxsTGVuZ3RoID0gZGF0YS5kdXJhdGlvbjtcbiAgICAgICAgICAgICAgICAgICAgICAgICQoJyMnK3RoaXMuZWxlbWVudElkKS5odG1sKGRhdGEuZHVyYXRpb24pO1xuICAgICAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgfTtcbiAgICAgICAgICAgIFxuICAgICAgICAgICAgdGhpcy53b3JrZXIucG9zdE1lc3NhZ2UoSlNPTi5zdHJpbmdpZnkoe3Rva2VuOiB0aGlzLmN0aVRva2VuLCBjb21tYW5kOiAnU1RBUlQnfSkpO1xuICAgICAgICB9XG4gICAgfVxuXG4gICAgcHVibGljIHJlc2V0VUkoKTp2b2lkIHtcbiAgICAgICAgJCgnIycrdGhpcy5lbGVtZW50SWQpLmh0bWwodGhpcy5jYWxsRHVyYXRpb24pO1xuICAgIH1cblxuICAgIHB1YmxpYyBzdG9wQ2xvY2soKTogdm9pZCB7XG4gICAgICAgIGlmKHRoaXMuaXNSdW5uaW5nKSB7XG4gICAgICAgICAgICB0aGlzLndvcmtlci50ZXJtaW5hdGUoKTtcbiAgICAgICAgICAgIHRoaXMuY2FsbEVuZFRpbWUgPSBuZXcgRGF0ZSgpO1xuICAgICAgICAgICAgdGhpcy5jYWxsTGVuZ3RoID0gdGhpcy5jYWxsRHVyYXRpb247XG4gICAgICAgICAgICB0aGlzLmNhbGxEdXJhdGlvbiA9ICcwMDowMDowMCc7XG4gICAgICAgICAgICAkKCcjJyt0aGlzLmVsZW1lbnRJZCkuaHRtbCh0aGlzLmNhbGxMZW5ndGgpO1xuICAgICAgICAgICAgdGhpcy5pc1J1bm5pbmcgPSBmYWxzZTtcbiAgICAgICAgfVxuICAgIH1cblxuICAgIC8qKlxuICAgICAqIFRoaXMgbWV0aG9kIHJldHVybnMgdGhlIGNhbGwgZHVyYXRpb24gYXMgYSBzdHJpbmdcbiAgICAgKlxuICAgICAqIEByZXR1cm5zIHtzdHJpbmd9XG4gICAgICovXG4gICAgcHVibGljIGdldENhbGxMZW5ndGgoKTogc3RyaW5nIHtcbiAgICAgICAgcmV0dXJuIHRoaXMuY2FsbExlbmd0aDtcbiAgICB9XG5cbiAgICAvKipcbiAgICAgKiBUaGlzIG1ldGhvZCByZXR1cm5zIHRoZSBzdGFydCB0aW1lIG9mIHRoZSBjbG9jayBhcyBhIGRhdGUgb2JqZWN0XG4gICAgICogQHJldHVybnMge0RhdGV9XG4gICAgICovXG4gICAgcHVibGljIGdldENsb2NrU3RhcnRUaW1lKCk6IERhdGUge1xuICAgICAgICByZXR1cm4gdGhpcy5jYWxsU3RhcnRUaW1lO1xuICAgIH1cblxuICAgIC8qKlxuICAgICAqIFRoaXMgbWV0aG9kIHJldHVybnMgdGhlIGVuZCB0aW1lIG9mIGNsb2NrIGFzIGEgZGF0ZSBvYmplY3RcbiAgICAgKlxuICAgICAqIEByZXR1cm5zIHtEYXRlfVxuICAgICAqL1xuICAgIHB1YmxpYyBnZXRDbG9ja0VuZFRpbWUoKTogRGF0ZSB7XG4gICAgICAgIHJldHVybiB0aGlzLmNhbGxFbmRUaW1lPyB0aGlzLmNhbGxFbmRUaW1lIDogbmV3IERhdGUoKTtcbiAgICB9XG59Il19