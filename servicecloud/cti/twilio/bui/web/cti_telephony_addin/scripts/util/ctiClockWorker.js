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
 *  SHA1: $Id: 8c31317517f4f3df4b760f7bb79b73ab6cb84e51 $
 * *********************************************************************************************
 *  File: ctiClockWorker.js
 * ****************************************************************************************** */
var ORACLE_CTI;
(function (ORACLE_CTI) {
    var CtiClockWorker = /** @class */ (function () {
        function CtiClockWorker() {
            var _this = this;
            this.isClockStarted = false;
            this.ctiToken = 'ORACLE_OSVC_CTI';
            this.runClock = function () {
                _this.clockSeconds++;
                if (_this.clockSeconds === 60) {
                    _this.clockMinutes++;
                    _this.clockSeconds = 0;
                    if (_this.clockMinutes === 60) {
                        _this.clockHours++;
                        _this.clockMinutes = 0;
                    }
                }
                _this.callDuration = (_this.clockHours < 10 ? '0' + _this.clockHours : _this.clockHours) + ':' +
                    (_this.clockMinutes < 10 ? '0' + _this.clockMinutes : _this.clockMinutes) + ':' +
                    (_this.clockSeconds < 10 ? '0' + _this.clockSeconds : _this.clockSeconds);
                var postMethod = postMessage;
                postMethod(JSON.stringify({ token: _this.ctiToken, duration: _this.callDuration }));
                setTimeout(_this.runClock, 1000);
            };
            this.clockHours = 0;
            this.clockMinutes = 0;
            this.clockSeconds = 0;
            this.callDuration = '00:00:00';
            addEventListener('message', function (event) {
                if (!_this.isClockStarted && event && event.data) {
                    var data = JSON.parse(event.data);
                    if (data && data.token === _this.ctiToken && data.command === 'START') {
                        _this.runClock();
                        _this.isClockStarted = true;
                    }
                }
            });
        }
        return CtiClockWorker;
    }());
    ORACLE_CTI.CtiClockWorker = CtiClockWorker;
})(ORACLE_CTI || (ORACLE_CTI = {}));
new ORACLE_CTI.CtiClockWorker();
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiY3RpQ2xvY2tXb3JrZXIuanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyJjdGlDbG9ja1dvcmtlci50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTs7Ozs7Z0dBS2dHO0FBRWhHLElBQU8sVUFBVSxDQWdEaEI7QUFoREQsV0FBTyxVQUFVO0lBQ2I7UUFRSTtZQUFBLGlCQWVDO1lBbEJPLG1CQUFjLEdBQVksS0FBSyxDQUFDO1lBQ2hDLGFBQVEsR0FBVSxpQkFBaUIsQ0FBQztZQW1CckMsYUFBUSxHQUFHO2dCQUNkLEtBQUksQ0FBQyxZQUFZLEVBQUUsQ0FBQztnQkFDcEIsRUFBRSxDQUFDLENBQUMsS0FBSSxDQUFDLFlBQVksS0FBSyxFQUFFLENBQUMsQ0FBQyxDQUFDO29CQUMzQixLQUFJLENBQUMsWUFBWSxFQUFFLENBQUM7b0JBQ3BCLEtBQUksQ0FBQyxZQUFZLEdBQUcsQ0FBQyxDQUFDO29CQUV0QixFQUFFLENBQUMsQ0FBQyxLQUFJLENBQUMsWUFBWSxLQUFLLEVBQUUsQ0FBQyxDQUFDLENBQUM7d0JBQzNCLEtBQUksQ0FBQyxVQUFVLEVBQUUsQ0FBQzt3QkFDbEIsS0FBSSxDQUFDLFlBQVksR0FBRyxDQUFDLENBQUM7b0JBQzFCLENBQUM7Z0JBQ0wsQ0FBQztnQkFFRCxLQUFJLENBQUMsWUFBWSxHQUFHLENBQUMsS0FBSSxDQUFDLFVBQVUsR0FBRyxFQUFFLENBQUMsQ0FBQyxDQUFDLEdBQUcsR0FBRyxLQUFJLENBQUMsVUFBVSxDQUFDLENBQUMsQ0FBQyxLQUFJLENBQUMsVUFBVSxDQUFDLEdBQUcsR0FBRztvQkFDdEYsQ0FBQyxLQUFJLENBQUMsWUFBWSxHQUFHLEVBQUUsQ0FBQyxDQUFDLENBQUMsR0FBRyxHQUFHLEtBQUksQ0FBQyxZQUFZLENBQUMsQ0FBQyxDQUFDLEtBQUksQ0FBQyxZQUFZLENBQUMsR0FBRyxHQUFHO29CQUM1RSxDQUFDLEtBQUksQ0FBQyxZQUFZLEdBQUcsRUFBRSxDQUFDLENBQUMsQ0FBQyxHQUFHLEdBQUcsS0FBSSxDQUFDLFlBQVksQ0FBQyxDQUFDLENBQUMsS0FBSSxDQUFDLFlBQVksQ0FBQyxDQUFDO2dCQUUzRSxJQUFJLFVBQVUsR0FBUSxXQUFXLENBQUM7Z0JBQ2xDLFVBQVUsQ0FBQyxJQUFJLENBQUMsU0FBUyxDQUFDLEVBQUMsS0FBSyxFQUFFLEtBQUksQ0FBQyxRQUFRLEVBQUUsUUFBUSxFQUFFLEtBQUksQ0FBQyxZQUFZLEVBQUMsQ0FBQyxDQUFDLENBQUM7Z0JBRWhGLFVBQVUsQ0FBQyxLQUFJLENBQUMsUUFBUSxFQUFFLElBQUksQ0FBQyxDQUFDO1lBQ3BDLENBQUMsQ0FBQTtZQXBDRyxJQUFJLENBQUMsVUFBVSxHQUFHLENBQUMsQ0FBQztZQUNwQixJQUFJLENBQUMsWUFBWSxHQUFHLENBQUMsQ0FBQztZQUN0QixJQUFJLENBQUMsWUFBWSxHQUFHLENBQUMsQ0FBQztZQUN0QixJQUFJLENBQUMsWUFBWSxHQUFHLFVBQVUsQ0FBQztZQUUvQixnQkFBZ0IsQ0FBQyxTQUFTLEVBQUUsVUFBQyxLQUFVO2dCQUNuQyxFQUFFLENBQUEsQ0FBQyxDQUFDLEtBQUksQ0FBQyxjQUFjLElBQUksS0FBSyxJQUFJLEtBQUssQ0FBQyxJQUFJLENBQUMsQ0FBQSxDQUFDO29CQUM1QyxJQUFJLElBQUksR0FBUSxJQUFJLENBQUMsS0FBSyxDQUFDLEtBQUssQ0FBQyxJQUFJLENBQUMsQ0FBQztvQkFDdkMsRUFBRSxDQUFBLENBQUMsSUFBSSxJQUFJLElBQUksQ0FBQyxLQUFLLEtBQUssS0FBSSxDQUFDLFFBQVEsSUFBSSxJQUFJLENBQUMsT0FBTyxLQUFLLE9BQU8sQ0FBQyxDQUFDLENBQUM7d0JBQ2xFLEtBQUksQ0FBQyxRQUFRLEVBQUUsQ0FBQzt3QkFDaEIsS0FBSSxDQUFDLGNBQWMsR0FBRyxJQUFJLENBQUM7b0JBQy9CLENBQUM7Z0JBQ0wsQ0FBQztZQUNMLENBQUMsQ0FBQyxDQUFDO1FBQ1AsQ0FBQztRQXVCTCxxQkFBQztJQUFELENBQUMsQUE5Q0QsSUE4Q0M7SUE5Q1kseUJBQWMsaUJBOEMxQixDQUFBO0FBQ0wsQ0FBQyxFQWhETSxVQUFVLEtBQVYsVUFBVSxRQWdEaEI7QUFFRCxJQUFJLFVBQVUsQ0FBQyxjQUFjLEVBQUUsQ0FBQyIsInNvdXJjZXNDb250ZW50IjpbIi8qICogKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKlxuICogICRBQ0NFTEVSQVRPUl9IRUFERVJfUExBQ0VfSE9MREVSJFxuICogIFNIQTE6ICRJZDogOGMzMTMxNzUxN2Y0ZjNkZjRiNzYwZjdiYjc5YjczYWI2Y2I4NGU1MSAkXG4gKiAqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKipcbiAqICBGaWxlOiAkQUNDRUxFUkFUT1JfSEVBREVSX0ZJTEVfTkFNRV9QTEFDRV9IT0xERVIkXG4gKiAqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKiogKi9cblxubW9kdWxlIE9SQUNMRV9DVEkge1xuICAgIGV4cG9ydCBjbGFzcyBDdGlDbG9ja1dvcmtlciB7XG4gICAgICAgIHByaXZhdGUgY2xvY2tIb3VyczogbnVtYmVyO1xuICAgICAgICBwcml2YXRlIGNsb2NrTWludXRlczogbnVtYmVyO1xuICAgICAgICBwcml2YXRlIGNsb2NrU2Vjb25kczogbnVtYmVyO1xuICAgICAgICBwcml2YXRlIGNhbGxEdXJhdGlvbjogc3RyaW5nO1xuICAgICAgICBwcml2YXRlIGlzQ2xvY2tTdGFydGVkOiBib29sZWFuID0gZmFsc2U7XG4gICAgICAgIHByaXZhdGUgY3RpVG9rZW46IHN0cmluZyA9J09SQUNMRV9PU1ZDX0NUSSc7XG5cbiAgICAgICAgcHVibGljIGNvbnN0cnVjdG9yKCkge1xuICAgICAgICAgICAgdGhpcy5jbG9ja0hvdXJzID0gMDtcbiAgICAgICAgICAgIHRoaXMuY2xvY2tNaW51dGVzID0gMDtcbiAgICAgICAgICAgIHRoaXMuY2xvY2tTZWNvbmRzID0gMDtcbiAgICAgICAgICAgIHRoaXMuY2FsbER1cmF0aW9uID0gJzAwOjAwOjAwJztcblxuICAgICAgICAgICAgYWRkRXZlbnRMaXN0ZW5lcignbWVzc2FnZScsIChldmVudDogYW55KT0+IHtcbiAgICAgICAgICAgICAgICBpZighdGhpcy5pc0Nsb2NrU3RhcnRlZCAmJiBldmVudCAmJiBldmVudC5kYXRhKXtcbiAgICAgICAgICAgICAgICAgICAgdmFyIGRhdGE6IGFueSA9IEpTT04ucGFyc2UoZXZlbnQuZGF0YSk7XG4gICAgICAgICAgICAgICAgICAgIGlmKGRhdGEgJiYgZGF0YS50b2tlbiA9PT0gdGhpcy5jdGlUb2tlbiAmJiBkYXRhLmNvbW1hbmQgPT09ICdTVEFSVCcpIHtcbiAgICAgICAgICAgICAgICAgICAgICAgIHRoaXMucnVuQ2xvY2soKTtcbiAgICAgICAgICAgICAgICAgICAgICAgIHRoaXMuaXNDbG9ja1N0YXJ0ZWQgPSB0cnVlO1xuICAgICAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgfSk7XG4gICAgICAgIH1cblxuICAgICAgICBwdWJsaWMgcnVuQ2xvY2sgPSAoKSA9PiB7XG4gICAgICAgICAgICB0aGlzLmNsb2NrU2Vjb25kcysrO1xuICAgICAgICAgICAgaWYgKHRoaXMuY2xvY2tTZWNvbmRzID09PSA2MCkge1xuICAgICAgICAgICAgICAgIHRoaXMuY2xvY2tNaW51dGVzKys7XG4gICAgICAgICAgICAgICAgdGhpcy5jbG9ja1NlY29uZHMgPSAwO1xuXG4gICAgICAgICAgICAgICAgaWYgKHRoaXMuY2xvY2tNaW51dGVzID09PSA2MCkge1xuICAgICAgICAgICAgICAgICAgICB0aGlzLmNsb2NrSG91cnMrKztcbiAgICAgICAgICAgICAgICAgICAgdGhpcy5jbG9ja01pbnV0ZXMgPSAwO1xuICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgIH1cblxuICAgICAgICAgICAgdGhpcy5jYWxsRHVyYXRpb24gPSAodGhpcy5jbG9ja0hvdXJzIDwgMTAgPyAnMCcgKyB0aGlzLmNsb2NrSG91cnMgOiB0aGlzLmNsb2NrSG91cnMpICsgJzonICtcbiAgICAgICAgICAgICAgICAodGhpcy5jbG9ja01pbnV0ZXMgPCAxMCA/ICcwJyArIHRoaXMuY2xvY2tNaW51dGVzIDogdGhpcy5jbG9ja01pbnV0ZXMpICsgJzonICtcbiAgICAgICAgICAgICAgICAodGhpcy5jbG9ja1NlY29uZHMgPCAxMCA/ICcwJyArIHRoaXMuY2xvY2tTZWNvbmRzIDogdGhpcy5jbG9ja1NlY29uZHMpO1xuXG4gICAgICAgICAgICB2YXIgcG9zdE1ldGhvZDogYW55ID0gcG9zdE1lc3NhZ2U7XG4gICAgICAgICAgICBwb3N0TWV0aG9kKEpTT04uc3RyaW5naWZ5KHt0b2tlbjogdGhpcy5jdGlUb2tlbiwgZHVyYXRpb246IHRoaXMuY2FsbER1cmF0aW9ufSkpO1xuXG4gICAgICAgICAgICBzZXRUaW1lb3V0KHRoaXMucnVuQ2xvY2ssIDEwMDApO1xuICAgICAgICB9XG4gICAgfVxufVxuXG5uZXcgT1JBQ0xFX0NUSS5DdGlDbG9ja1dvcmtlcigpOyJdfQ==