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
 *  SHA1: $Id: 97a3fb906346f928802df9ac1fb0f32d0b512be8 $
 * *********************************************************************************************
 *  File: ctiReportingAddin.js
 * ****************************************************************************************** */
define(["require", "exports", "jquery", "./../model/reportRow", "../util/ctiMessages", "../util/ctiLogger", "../model/pagination"], function (require, exports, $, reportRow_1, ctiMessages_1, ctiLogger_1, pagination_1) {
    "use strict";
    exports.__esModule = true;
    var CtiReportingAddin = /** @class */ (function () {
        function CtiReportingAddin() {
            this.completedCount = 0;
            this.workerData = {};
            this.logPreMessage = 'CtiReportingAddin ' + ctiMessages_1.CtiMessages.MESSAGE_APPENDER;
        }
        CtiReportingAddin.prototype.registerCtiReportingAdin = function () {
            var _this = this;
            ctiLogger_1.CtiLogger.logInfoMessage(this.logPreMessage + ctiMessages_1.CtiMessages.MESSAGE_ADDIN_REGISTER);
            ORACLE_SERVICE_CLOUD.extension_loader.load('DataListenerApp').then(function (extensionProvider) {
                extensionProvider.registerAnalyticsExtension(function (analyticsContext) {
                    analyticsContext.addTableDataRequestListener('accelerator$CtiAgentStats', function (report) { return _this.reportDataSourceHandler(report); });
                    ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage + ctiMessages_1.CtiMessages.MESSAGE_ADDIN_INITIALIZED);
                });
                extensionProvider.getGlobalContext().then(function (globalContext) {
                    _this.globalContext = globalContext;
                    _this.serverURI = globalContext.getInterfaceUrl().match(/^[^\/]+:\/\/[^\/]+\//)[0].replace(/^http(?!s)/i, 'https') +
                        'cc/CTI/twilioProxy';
                    globalContext.getSessionToken().then(function (sessionToken) {
                        _this.sessionId = sessionToken;
                    });
                });
            });
        };
        CtiReportingAddin.prototype.reportDataSourceHandler = function (report) {
            var _this = this;
            ctiLogger_1.CtiLogger.logInfoMessage(this.logPreMessage + ctiMessages_1.CtiMessages.MESSAGE_START_REPORT_EXECUTION);
            return new ExtensionPromise(function (resolve, reject) {
                var startDate;
                var endDate;
                var pageNumber;
                var rowsPerPage;
                var filterList = report.getReportFilters().getFilterList();
                pageNumber = report.getReportFilters().getPageNumber();
                rowsPerPage = report.getReportFilters().getRowsPerPage();
                for (var i = 0; i < filterList.length; i++) {
                    var filter = filterList[i];
                    if (filter.getColumnReference() === 'accelerator$CtiAgentStats.fromDate') {
                        startDate = filter.getValue();
                    }
                    else if (filter.getColumnReference() === 'accelerator$CtiAgentStats.toDate') {
                        endDate = filter.getValue();
                    }
                }
                var reportData = report.createReportData();
                _this.getReportData(_this.sessionId, startDate, endDate).then(function (reportRows) {
                    var totalRecordsCount = reportRows.length;
                    reportData.setTotalRecordCount(totalRecordsCount);
                    if (!pageNumber) {
                        pageNumber = 0;
                    }
                    if (!rowsPerPage) {
                        rowsPerPage = 10;
                    }
                    var pagination = _this.getPaginationData(pageNumber, rowsPerPage, totalRecordsCount);
                    for (var i = pagination.lowerBound; i < pagination.upperBound; i++) {
                        var reportRow = reportRows[i];
                        var row = report.createReportDataRow();
                        var agentId = reportRow.agentName;
                        if (!isNaN(agentId)) {
                            var cellName = report.createReportDataCell();
                            cellName.setData(reportRow.agentName);
                            row.cells.push(cellName);
                            var cellAccceptedCalls = report.createReportDataCell();
                            cellAccceptedCalls.setData(reportRow.reservationAccepted);
                            row.cells.push(cellAccceptedCalls);
                            var cellCancelledCalls = report.createReportDataCell();
                            cellCancelledCalls.setData(reportRow.reservationCancelled);
                            row.cells.push(cellCancelledCalls);
                            var cellTimedOutCalls = report.createReportDataCell();
                            cellTimedOutCalls.setData(reportRow.reservationTimedOut);
                            row.cells.push(cellTimedOutCalls);
                            var cellRejectedCalls = report.createReportDataCell();
                            cellRejectedCalls.setData(reportRow.reservationRejected);
                            row.cells.push(cellRejectedCalls);
                            var cellReservationsCreated = report.createReportDataCell();
                            cellReservationsCreated.setData(reportRow.reservationCreated);
                            row.cells.push(cellReservationsCreated);
                            var cellReservedDuation = report.createReportDataCell();
                            cellReservedDuation.setData(reportRow.reservedDuration);
                            row.cells.push(cellReservedDuation);
                            var cellAvailableDuration = report.createReportDataCell();
                            cellAvailableDuration.setData(reportRow.readyDuration);
                            row.cells.push(cellAvailableDuration);
                            var cellBusyDuration = report.createReportDataCell();
                            cellBusyDuration.setData(reportRow.busyDuration);
                            row.cells.push(cellBusyDuration);
                            var cellNotAvailableDuration = report.createReportDataCell();
                            cellNotAvailableDuration.setData(reportRow.offlineDuration);
                            row.cells.push(cellNotAvailableDuration);
                            reportData.rows.push(row);
                        }
                    }
                    resolve(reportData);
                });
            });
        };
        CtiReportingAddin.prototype.getPaginationData = function (pageNumber, recordsPerPage, totalRecords) {
            var pagination = new pagination_1.Pagination;
            pagination.lowerBound = 0;
            pagination.upperBound = 0;
            pagination.totalRecords = totalRecords;
            pagination.recordsPerPage = recordsPerPage;
            if (recordsPerPage >= totalRecords || (pageNumber * recordsPerPage) >= totalRecords) {
                pagination.upperBound = totalRecords;
            }
            else {
                if (pageNumber) {
                    pagination.upperBound = pageNumber * recordsPerPage;
                    pagination.lowerBound = pagination.upperBound - recordsPerPage;
                    pagination.nextPage = pageNumber++;
                }
                else {
                    pagination.upperBound = recordsPerPage;
                }
            }
            return pagination;
        };
        CtiReportingAddin.prototype.getReportData = function (sessionId, startDate, endDate) {
            var _this = this;
            var promiseObj = new ExtensionPromise(function (resolve, reject) {
                _this.resolveRef = resolve;
                _this.rejectRef = reject;
            });
            var startDateTime;
            var endDateTime;
            if (startDate) {
                startDateTime = new Date(startDate);
            }
            else {
                startDateTime = new Date();
                startDateTime.setHours(startDateTime.getHours() - 8);
            }
            if (endDate) {
                endDateTime = new Date(endDate);
            }
            else {
                endDateTime = new Date();
            }
            this.fetchReportData(this.serverURI, this.sessionId, startDateTime.toISOString(), endDateTime.toISOString());
            return promiseObj;
        };
        CtiReportingAddin.prototype.fetchReportData = function (serverUri, sessionId, startDate, endDate) {
            var _this = this;
            ctiLogger_1.CtiLogger.logInfoMessage(this.logPreMessage + ctiMessages_1.CtiMessages.MESSAGE_GET_AGENT_DATA);
            var apiGetWorkers = 'https://taskrouter.twilio.com/v1/Workspaces/{WORKSPACE_SID}/Workers';
            $.ajax({
                type: "POST",
                url: serverUri,
                data: {
                    session_id: sessionId,
                    uri: apiGetWorkers
                }
            })
                .done(function (workersResult) {
                var allWorkerData = JSON.parse(workersResult);
                if (allWorkerData && allWorkerData.workers && allWorkerData.workers.length > 0) {
                    _this.workerCount = allWorkerData.workers.length;
                    _this.fetchWorkerStatistics(allWorkerData.workers, serverUri, sessionId, startDate, endDate);
                }
                else {
                    ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage + ctiMessages_1.CtiMessages.MESSAGE_NO_AGENTS_FOUND);
                }
            })
                .fail(function (message) {
                ctiLogger_1.CtiLogger.logErrorMessage(_this.logPreMessage +
                    ctiMessages_1.CtiMessages.MESSAGE_APPENDER + ctiMessages_1.CtiMessages.MESSAGE_REPORT_EXECUTION_FAILED + message);
            });
        };
        CtiReportingAddin.prototype.fetchWorkerStatistics = function (allWorkers, serverUri, sessionId, startDate, endDate) {
            var _this = this;
            var workerCount = allWorkers.length;
            for (var count = 0; count < workerCount; count++) {
                var worker = allWorkers[count];
                this.workerData[worker.sid] = { name: worker.friendly_name };
                var apiWorkerStat = 'https://taskrouter.twilio.com/v1/Workspaces/{WORKSPACE_SID}/Workers/' + worker.sid + '/Statistics?StartDate=' + startDate + '&EndDate=' + endDate;
                $.ajax({
                    type: "POST",
                    url: serverUri,
                    data: {
                        session_id: sessionId,
                        uri: apiWorkerStat
                    }
                })
                    .done(function (workerResult) {
                    var workDetail = JSON.parse(workerResult);
                    _this.addWorkDetails(workDetail);
                })
                    .fail(function (data) {
                    _this.addWorkDetails(null);
                });
            }
        };
        CtiReportingAddin.prototype.addWorkDetails = function (workDetail) {
            if (workDetail && workDetail.worker_sid) {
                this.workerData[workDetail.worker_sid]['work'] = workDetail;
            }
            this.completedCount++;
            if (this.completedCount >= this.workerCount) {
                ctiLogger_1.CtiLogger.logInfoMessage(this.logPreMessage + ctiMessages_1.CtiMessages.MESSAGE_GET_AGENT_DATA_COMPLETED);
                this.processWorkerData();
            }
        };
        CtiReportingAddin.prototype.processWorkerData = function () {
            var report = [];
            var rowCount = 0;
            ctiLogger_1.CtiLogger.logInfoMessage(this.logPreMessage + ctiMessages_1.CtiMessages.MESSAGE_PROCESSING_DATA);
            for (var key in this.workerData) {
                if (this.workerData.hasOwnProperty(key)) {
                    var workDetails = this.workerData[key];
                    var reportRow = new reportRow_1.ReportRow();
                    report[rowCount] = reportRow;
                    reportRow.agentName = workDetails.name.substring(workDetails.name.lastIndexOf('_') + 1, workDetails.name.length);
                    rowCount++;
                    if (workDetails.work && workDetails.work.cumulative) {
                        var statistics = workDetails.work.cumulative;
                        reportRow.reservationAccepted = statistics.reservations_accepted;
                        reportRow.reservationCancelled = statistics.reservations_canceled;
                        reportRow.reservationCreated = statistics.reservations_created;
                        reportRow.reservationRejected = statistics.reservations_rejected;
                        reportRow.reservationTimedOut = statistics.reservations_timed_out;
                        var activityDurations = statistics.activity_durations;
                        if (activityDurations && activityDurations.length > 0) {
                            for (var index in activityDurations) {
                                if (activityDurations.hasOwnProperty(index)) {
                                    var activityData = activityDurations[index];
                                    var duration = (activityData.total / 60).toFixed(2);
                                    switch (activityData.friendly_name) {
                                        case 'Not Available':
                                            reportRow.offlineDuration = duration;
                                            break;
                                        case 'Busy':
                                            reportRow.busyDuration = duration;
                                            break;
                                        case 'Reserved':
                                            reportRow.reservedDuration = duration;
                                            break;
                                        case 'Ready':
                                            reportRow.readyDuration = duration;
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            this.resolveRef(report);
            this.workerCount = null;
            this.completedCount = 0;
            this.workerData = {};
            ctiLogger_1.CtiLogger.logInfoMessage(this.logPreMessage + ctiMessages_1.CtiMessages.MESSAGE_PROCESSING_DATA_COMPLETED);
        };
        return CtiReportingAddin;
    }());
    exports.CtiReportingAddin = CtiReportingAddin;
    new CtiReportingAddin().registerCtiReportingAdin();
});
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiY3RpUmVwb3J0aW5nQWRkaW4uanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyJjdGlSZXBvcnRpbmdBZGRpbi50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTs7Ozs7Z0dBS2dHOzs7O0lBU2hHO1FBQUE7WUFHWSxtQkFBYyxHQUFXLENBQUMsQ0FBQztZQUMzQixlQUFVLEdBQXlCLEVBQUUsQ0FBQztZQUt0QyxrQkFBYSxHQUFXLG9CQUFvQixHQUFHLHlCQUFXLENBQUMsZ0JBQWdCLENBQUM7UUFrU3hGLENBQUM7UUFoU1Usb0RBQXdCLEdBQS9CO1lBQUEsaUJBMEJDO1lBekJHLHFCQUFTLENBQUMsY0FBYyxDQUFDLElBQUksQ0FBQyxhQUFhLEdBQUcseUJBQVcsQ0FBQyxzQkFBc0IsQ0FBQyxDQUFDO1lBQ2xGLG9CQUFvQixDQUFDLGdCQUFnQixDQUFDLElBQUksQ0FBQyxpQkFBaUIsQ0FBQyxDQUFDLElBQUksQ0FDOUQsVUFBQyxpQkFBMkQ7Z0JBQ3hELGlCQUFpQixDQUFDLDBCQUEwQixDQUN4QyxVQUFDLGdCQUFxQjtvQkFDbEIsZ0JBQWdCLENBQUMsMkJBQTJCLENBQ3hDLDJCQUEyQixFQUFFLFVBQUMsTUFBNkMsSUFBSyxPQUFBLEtBQUksQ0FBQyx1QkFBdUIsQ0FBQyxNQUFNLENBQUMsRUFBcEMsQ0FBb0MsQ0FDdkgsQ0FBQztvQkFDRixxQkFBUyxDQUFDLGNBQWMsQ0FBQyxLQUFJLENBQUMsYUFBYSxHQUFHLHlCQUFXLENBQUMseUJBQXlCLENBQUMsQ0FBQztnQkFDekYsQ0FBQyxDQUNKLENBQUM7Z0JBQ0YsaUJBQWlCLENBQUMsZ0JBQWdCLEVBQUUsQ0FBQyxJQUFJLENBQ3JDLFVBQUMsYUFBMkQ7b0JBQ3hELEtBQUksQ0FBQyxhQUFhLEdBQUcsYUFBYSxDQUFDO29CQUNuQyxLQUFJLENBQUMsU0FBUyxHQUFHLGFBQWEsQ0FBQyxlQUFlLEVBQUUsQ0FBQyxLQUFLLENBQUMsc0JBQXNCLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxP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VixLQUFLLE9BQU87NENBQ1IsU0FBUyxDQUFDLGFBQWEsR0FBRyxRQUFRLENBQUM7NENBQ25DLEtBQUssQ0FBQztvQ0FDZCxDQUFDO2dDQUNMLENBQUM7NEJBRUwsQ0FBQzt3QkFDTCxDQUFDO29CQUNMLENBQUM7Z0JBQ0wsQ0FBQztZQUNMLENBQUM7WUFFRCxJQUFJLENBQUMsVUFBVSxDQUFDLE1BQU0sQ0FBQyxDQUFDO1lBQ3hCLElBQUksQ0FBQyxXQUFXLEdBQUcsSUFBSSxDQUFDO1lBQ3hCLElBQUksQ0FBQyxjQUFjLEdBQUcsQ0FBQyxDQUFDO1lBQ3hCLElBQUksQ0FBQyxVQUFVLEdBQUcsRUFBRSxDQUFDO1lBQ3JCLHFCQUFTLENBQUMsY0FBYyxDQUFDLElBQUksQ0FBQyxhQUFhLEdBQUcseUJBQVcsQ0FBQyxpQ0FBaUMsQ0FBQyxDQUFDO1FBRWpHLENBQUM7UUFFTCx3QkFBQztJQUFELENBQUMsQUEzU0QsSUEyU0M7SUEzU1ksOENBQWlCO0lBNFM5QixJQUFJLGlCQUFpQixFQUFFLENBQUMsd0JBQXdCLEVBQUUsQ0FBQyIsInNvdXJjZXNDb250ZW50IjpbIi8qICogKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKlxuICogICRBQ0NFTEVSQVRPUl9IRUFERVJfUExBQ0VfSE9MREVSJFxuICogIFNIQTE6ICRJZDogOTdhM2ZiOTA2MzQ2ZjkyODgwMmRmOWFjMWZiMGYzMmQwYjUxMmJlOCAkXG4gKiAqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKipcbiAqICBGaWxlOiAkQUNDRUxFUkFUT1JfSEVBREVSX0ZJTEVfTkFNRV9QTEFDRV9IT0xERVIkXG4gKiAqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKiogKi9cblxuLy8vPHJlZmVyZW5jZSBwYXRoPScuLi8uLi8uLi9kZWZpbml0aW9ucy9vc3ZjRXh0ZW5zaW9uLmQudHMnIC8+XG5cbmltcG9ydCAkID0gcmVxdWlyZSgnanF1ZXJ5Jyk7XG5pbXBvcnQge1JlcG9ydFJvd30gZnJvbSBcIi4vLi4vbW9kZWwvcmVwb3J0Um93XCI7XG5pbXBvcnQge0N0aU1lc3NhZ2VzfSBmcm9tIFwiLi4vdXRpbC9jdGlNZXNzYWdlc1wiO1xuaW1wb3J0IHtDdGlMb2dnZXJ9IGZyb20gXCIuLi91dGlsL2N0aUxvZ2dlclwiO1xuaW1wb3J0IHtQYWdpbmF0aW9ufSBmcm9tIFwiLi4vbW9kZWwvcGFnaW5hdGlvblwiO1xuZXhwb3J0IGNsYXNzIEN0aVJlcG9ydGluZ0FkZGluIHtcblxuICAgIHByaXZhdGUgd29ya2VyQ291bnQ6IG51bWJlcjtcbiAgICBwcml2YXRlIGNvbXBsZXRlZENvdW50OiBudW1iZXIgPSAwO1xuICAgIHByaXZhdGUgd29ya2VyRGF0YToge1trZXk6IHN0cmluZ106IGFueX0gPSB7fTtcbiAgICBwcml2YXRlIHNlcnZlclVSSTogc3RyaW5nO1xuICAgIHByaXZhdGUgcmVzb2x2ZVJlZjogYW55OyBwcml2YXRlIHJlamVjdFJlZjphbnk7XG4gICAgcHJpdmF0ZSBzZXNzaW9uSWQ6IHN0cmluZztcbiAgICBwcml2YXRlIGdsb2JhbENvbnRleHQ6IE9SQUNMRV9TRVJWSUNFX0NMT1VELklFeHRlbnNpb25HbG9iYWxDb250ZXh0O1xuICAgIHByaXZhdGUgbG9nUHJlTWVzc2FnZTogc3RyaW5nID0gJ0N0aVJlcG9ydGluZ0FkZGluICcgKyBDdGlNZXNzYWdlcy5NRVNTQUdFX0FQUEVOREVSO1xuXG4gICAgcHVibGljIHJlZ2lzdGVyQ3RpUmVwb3J0aW5nQWRpbigpOiB2b2lke1xuICAgICAgICBDdGlMb2dnZXIubG9nSW5mb01lc3NhZ2UodGhpcy5sb2dQcmVNZXNzYWdlICsgQ3RpTWVzc2FnZXMuTUVTU0FHRV9BRERJTl9SRUdJU1RFUik7XG4gICAgICAgIE9SQUNMRV9TRVJWSUNFX0NMT1VELmV4dGVuc2lvbl9sb2FkZXIubG9hZCgnRGF0YUxpc3RlbmVyQXBwJykudGhlbihcbiAgICAgICAgICAgIChleHRlbnNpb25Qcm92aWRlciA6IE9SQUNMRV9TRVJWSUNFX0NMT1VELklFeHRlbnNpb25Qcm92aWRlcikgPT57XG4gICAgICAgICAgICAgICAgZXh0ZW5zaW9uUHJvdmlkZXIucmVnaXN0ZXJBbmFseXRpY3NFeHRlbnNpb24oXG4gICAgICAgICAgICAgICAgICAgIChhbmFseXRpY3NDb250ZXh0OiBhbnkpID0+IHtcbiAgICAgICAgICAgICAgICAgICAgICAgIGFuYWx5dGljc0NvbnRleHQuYWRkVGFibGVEYXRhUmVxdWVzdExpc3RlbmVyKFxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICdhY2NlbGVyYXRvciRDdGlBZ2VudFN0YXRzJywgKHJlcG9ydDogT1JBQ0xFX1NFUlZJQ0VfQ0xPVUQuSUV4dGVuc2lvblJlcG9ydCkgPT4gdGhpcy5yZXBvcnREYXRhU291cmNlSGFuZGxlcihyZXBvcnQpXG4gICAgICAgICAgICAgICAgICAgICAgICApO1xuICAgICAgICAgICAgICAgICAgICAgICAgQ3RpTG9nZ2VyLmxvZ0luZm9NZXNzYWdlKHRoaXMubG9nUHJlTWVzc2FnZSArIEN0aU1lc3NhZ2VzLk1FU1NBR0VfQURESU5fSU5JVElBTElaRUQpO1xuICAgICAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgKTtcbiAgICAgICAgICAgICAgICBleHRlbnNpb25Qcm92aWRlci5nZXRHbG9iYWxDb250ZXh0KCkudGhlbihcbiAgICAgICAgICAgICAgICAgICAgKGdsb2JhbENvbnRleHQ6IE9SQUNMRV9TRVJWSUNFX0NMT1VELklFeHRlbnNpb25HbG9iYWxDb250ZXh0KSA9PiB7XG4gICAgICAgICAgICAgICAgICAgICAgICB0aGlzLmdsb2JhbENvbnRleHQgPSBnbG9iYWxDb250ZXh0O1xuICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5zZXJ2ZXJVUkkgPSBnbG9iYWxDb250ZXh0LmdldEludGVyZmFjZVVybCgpLm1hdGNoKC9eW15cXC9dKzpcXC9cXC9bXlxcL10rXFwvLylbMF0ucmVwbGFjZSgvXmh0dHAoPyFzKS9pLCAnaHR0cHMnKSArXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICdjYy9DVEkvdHdpbGlvUHJveHknO1xuICAgICAgICAgICAgICAgICAgICAgICAgZ2xvYmFsQ29udGV4dC5nZXRTZXNzaW9uVG9rZW4oKS50aGVuKFxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIChzZXNzaW9uVG9rZW46IHN0cmluZykgPT4ge1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB0aGlzLnNlc3Npb25JZCA9IHNlc3Npb25Ub2tlbjtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgICAgICAgICApO1xuICAgICAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgKVxuICAgICAgICAgICAgfVxuICAgICAgICApO1xuICAgIH1cblxuICAgIHB1YmxpYyByZXBvcnREYXRhU291cmNlSGFuZGxlcihyZXBvcnQ6IE9SQUNMRV9TRVJWSUNFX0NMT1VELklFeHRlbnNpb25SZXBvcnQpOiBhbnl7XG4gICAgICAgIEN0aUxvZ2dlci5sb2dJbmZvTWVzc2FnZSh0aGlzLmxvZ1ByZU1lc3NhZ2UgKyBDdGlNZXNzYWdlcy5NRVNTQUdFX1NUQVJUX1JFUE9SVF9FWEVDVVRJT04pO1xuICAgICAgICByZXR1cm4gbmV3IEV4dGVuc2lvblByb21pc2UoIChyZXNvbHZlOiBhbnksIHJlamVjdDogYW55KSA9PiB7XG4gICAgICAgICAgICB2YXIgc3RhcnREYXRlOiBhbnk7XG4gICAgICAgICAgICB2YXIgZW5kRGF0ZTogYW55O1xuICAgICAgICAgICAgdmFyIHBhZ2VOdW1iZXI6IG51bWJlcjtcbiAgICAgICAgICAgIHZhciByb3dzUGVyUGFnZTogbnVtYmVyO1xuXG4gICAgICAgICAgICBsZXQgZmlsdGVyTGlzdDogT1JBQ0xFX1NFUlZJQ0VfQ0xPVUQuSUV4dGVuc2lvbkZpbHRlcltdID0gcmVwb3J0LmdldFJlcG9ydEZpbHRlcnMoKS5nZXRGaWx0ZXJMaXN0KCk7XG4gICAgICAgICAgICBwYWdlTnVtYmVyID0gcmVwb3J0LmdldFJlcG9ydEZpbHRlcnMoKS5nZXRQYWdlTnVtYmVyKCk7XG4gICAgICAgICAgICByb3dzUGVyUGFnZSA9IHJlcG9ydC5nZXRSZXBvcnRGaWx0ZXJzKCkuZ2V0Um93c1BlclBhZ2UoKTtcbiAgICAgICAgICAgIFxuICAgICAgICAgICAgZm9yKGxldCBpPTA7IGk8ZmlsdGVyTGlzdC5sZW5ndGg7IGkrKyl7XG4gICAgICAgICAgICAgICAgbGV0IGZpbHRlcjogT1JBQ0xFX1NFUlZJQ0VfQ0xPVUQuSUV4dGVuc2lvbkZpbHRlciA9IGZpbHRlckxpc3RbaV07XG4gICAgICAgICAgICAgICAgaWYoZmlsdGVyLmdldENvbHVtblJlZmVyZW5jZSgpID09PSAnYWNjZWxlcmF0b3IkQ3RpQWdlbnRTdGF0cy5mcm9tRGF0ZScpe1xuICAgICAgICAgICAgICAgICAgICBzdGFydERhdGUgPSBmaWx0ZXIuZ2V0VmFsdWUoKTtcbiAgICAgICAgICAgICAgICB9ZWxzZSBpZihmaWx0ZXIuZ2V0Q29sdW1uUmVmZXJlbmNlKCkgPT09ICdhY2NlbGVyYXRvciRDdGlBZ2VudFN0YXRzLnRvRGF0ZScpe1xuICAgICAgICAgICAgICAgICAgICBlbmREYXRlID0gZmlsdGVyLmdldFZhbHVlKCk7XG4gICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgfVxuICAgICAgICAgICAgdmFyIHJlcG9ydERhdGEgPSByZXBvcnQuY3JlYXRlUmVwb3J0RGF0YSgpO1xuXG4gICAgICAgICAgICB0aGlzLmdldFJlcG9ydERhdGEodGhpcy5zZXNzaW9uSWQsIHN0YXJ0RGF0ZSwgZW5kRGF0ZSkudGhlbihcbiAgICAgICAgICAgICAgICAocmVwb3J0Um93czogUmVwb3J0Um93W10pID0+IHtcbiAgICAgICAgICAgICAgICAgICAgdmFyIHRvdGFsUmVjb3Jkc0NvdW50OiBudW1iZXIgPSByZXBvcnRSb3dzLmxlbmd0aDtcbiAgICAgICAgICAgICAgICAgICAgcmVwb3J0RGF0YS5zZXRUb3RhbFJlY29yZENvdW50KHRvdGFsUmVjb3Jkc0NvdW50KTtcbiAgICAgICAgICAgICAgICAgICAgaWYoIXBhZ2VOdW1iZXIpe1xuICAgICAgICAgICAgICAgICAgICAgICAgcGFnZU51bWJlciA9IDA7XG4gICAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICAgICAgaWYoIXJvd3NQZXJQYWdlKXtcbiAgICAgICAgICAgICAgICAgICAgICAgIHJvd3NQZXJQYWdlID0gMTA7XG4gICAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICAgICAgdmFyIHBhZ2luYXRpb246IFBhZ2luYXRpb24gPSB0aGlzLmdldFBhZ2luYXRpb25EYXRhKHBhZ2VOdW1iZXIsIHJvd3NQZXJQYWdlLCB0b3RhbFJlY29yZHNDb3VudCk7XG5cbiAgICAgICAgICAgICAgICAgICAgZm9yKHZhciBpID0gcGFnaW5hdGlvbi5sb3dlckJvdW5kOyBpPCBwYWdpbmF0aW9uLnVwcGVyQm91bmQ7IGkrKyl7XG4gICAgICAgICAgICAgICAgICAgICAgICBsZXQgcmVwb3J0Um93OiBSZXBvcnRSb3cgPSByZXBvcnRSb3dzW2ldO1xuICAgICAgICAgICAgICAgICAgICAgICAgdmFyIHJvdyA9IHJlcG9ydC5jcmVhdGVSZXBvcnREYXRhUm93KCk7XG5cbiAgICAgICAgICAgICAgICAgICAgICAgIHZhciBhZ2VudElkOiBhbnkgPSByZXBvcnRSb3cuYWdlbnROYW1lO1xuICAgICAgICAgICAgICAgICAgICAgICAgaWYoIWlzTmFOKGFnZW50SWQpKXtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB2YXIgY2VsbE5hbWUgPSByZXBvcnQuY3JlYXRlUmVwb3J0RGF0YUNlbGwoKTtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBjZWxsTmFtZS5zZXREYXRhKHJlcG9ydFJvdy5hZ2VudE5hbWUpO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHJvdy5jZWxscy5wdXNoKGNlbGxOYW1lKTtcblxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHZhciBjZWxsQWNjY2VwdGVkQ2FsbHMgPSByZXBvcnQuY3JlYXRlUmVwb3J0RGF0YUNlbGwoKTtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBjZWxsQWNjY2VwdGVkQ2FsbHMuc2V0RGF0YShyZXBvcnRSb3cucmVzZXJ2YXRpb25BY2NlcHRlZCk7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgcm93LmNlbGxzLnB1c2goY2VsbEFjY2NlcHRlZENhbGxzKTtcblxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHZhciBjZWxsQ2FuY2VsbGVkQ2FsbHMgPSByZXBvcnQuY3JlYXRlUmVwb3J0RGF0YUNlbGwoKTtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBjZWxsQ2FuY2VsbGVkQ2FsbHMuc2V0RGF0YShyZXBvcnRSb3cucmVzZXJ2YXRpb25DYW5jZWxsZWQpO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHJvdy5jZWxscy5wdXNoKGNlbGxDYW5jZWxsZWRDYWxscyk7XG5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB2YXIgY2VsbFRpbWVkT3V0Q2FsbHMgPSByZXBvcnQuY3JlYXRlUmVwb3J0RGF0YUNlbGwoKTtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBjZWxsVGltZWRPdXRDYWxscy5zZXREYXRhKHJlcG9ydFJvdy5yZXNlcnZhdGlvblRpbWVkT3V0KTtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICByb3cuY2VsbHMucHVzaChjZWxsVGltZWRPdXRDYWxscyk7XG5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB2YXIgY2VsbFJlamVjdGVkQ2FsbHMgPSByZXBvcnQuY3JlYXRlUmVwb3J0RGF0YUNlbGwoKTtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBjZWxsUmVqZWN0ZWRDYWxscy5zZXREYXRhKHJlcG9ydFJvdy5yZXNlcnZhdGlvblJlamVjdGVkKTtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICByb3cuY2VsbHMucHVzaChjZWxsUmVqZWN0ZWRDYWxscyk7XG5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB2YXIgY2VsbFJlc2VydmF0aW9uc0NyZWF0ZWQgPSByZXBvcnQuY3JlYXRlUmVwb3J0RGF0YUNlbGwoKTtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBjZWxsUmVzZXJ2YXRpb25zQ3JlYXRlZC5zZXREYXRhKHJlcG9ydFJvdy5yZXNlcnZhdGlvbkNyZWF0ZWQpO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHJvdy5jZWxscy5wdXNoKGNlbGxSZXNlcnZhdGlvbnNDcmVhdGVkKTtcblxuXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdmFyIGNlbGxSZXNlcnZlZER1YXRpb24gPSByZXBvcnQuY3JlYXRlUmVwb3J0RGF0YUNlbGwoKTtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBjZWxsUmVzZXJ2ZWREdWF0aW9uLnNldERhdGEocmVwb3J0Um93LnJlc2VydmVkRHVyYXRpb24pO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHJvdy5jZWxscy5wdXNoKGNlbGxSZXNlcnZlZER1YXRpb24pO1xuXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdmFyIGNlbGxBdmFpbGFibGVEdXJhdGlvbiA9IHJlcG9ydC5jcmVhdGVSZXBvcnREYXRhQ2VsbCgpO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGNlbGxBdmFpbGFibGVEdXJhdGlvbi5zZXREYXRhKHJlcG9ydFJvdy5yZWFkeUR1cmF0aW9uKTtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICByb3cuY2VsbHMucHVzaChjZWxsQXZhaWxhYmxlRHVyYXRpb24pO1xuXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdmFyIGNlbGxCdXN5RHVyYXRpb24gPSByZXBvcnQuY3JlYXRlUmVwb3J0RGF0YUNlbGwoKTtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBjZWxsQnVzeUR1cmF0aW9uLnNldERhdGEocmVwb3J0Um93LmJ1c3lEdXJhdGlvbik7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgcm93LmNlbGxzLnB1c2goY2VsbEJ1c3lEdXJhdGlvbik7XG5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB2YXIgY2VsbE5vdEF2YWlsYWJsZUR1cmF0aW9uID0gcmVwb3J0LmNyZWF0ZVJlcG9ydERhdGFDZWxsKCk7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgY2VsbE5vdEF2YWlsYWJsZUR1cmF0aW9uLnNldERhdGEocmVwb3J0Um93Lm9mZmxpbmVEdXJhdGlvbik7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgcm93LmNlbGxzLnB1c2goY2VsbE5vdEF2YWlsYWJsZUR1cmF0aW9uKTtcblxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHJlcG9ydERhdGEucm93cy5wdXNoKHJvdyk7XG4gICAgICAgICAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICAgICAgcmVzb2x2ZShyZXBvcnREYXRhKTtcbiAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICApO1xuICAgICAgICB9KTtcbiAgICB9XG4gICAgXG4gICAgcHJpdmF0ZSBnZXRQYWdpbmF0aW9uRGF0YShwYWdlTnVtYmVyOiBudW1iZXIsIHJlY29yZHNQZXJQYWdlOiBudW1iZXIsIHRvdGFsUmVjb3JkczogbnVtYmVyKTogUGFnaW5hdGlvbiB7XG4gICAgICAgIHZhciBwYWdpbmF0aW9uOiBQYWdpbmF0aW9uID0gbmV3IFBhZ2luYXRpb247XG4gICAgICAgIHBhZ2luYXRpb24ubG93ZXJCb3VuZCA9IDA7XG4gICAgICAgIHBhZ2luYXRpb24udXBwZXJCb3VuZCA9IDA7XG4gICAgICAgIHBhZ2luYXRpb24udG90YWxSZWNvcmRzID0gdG90YWxSZWNvcmRzO1xuICAgICAgICBwYWdpbmF0aW9uLnJlY29yZHNQZXJQYWdlID0gcmVjb3Jkc1BlclBhZ2U7XG5cbiAgICAgICAgaWYocmVjb3Jkc1BlclBhZ2UgPj0gdG90YWxSZWNvcmRzIHx8IChwYWdlTnVtYmVyICogcmVjb3Jkc1BlclBhZ2UpID49IHRvdGFsUmVjb3Jkcyl7XG4gICAgICAgICAgICBwYWdpbmF0aW9uLnVwcGVyQm91bmQgPSB0b3RhbFJlY29yZHM7XG4gICAgICAgIH1lbHNle1xuICAgICAgICAgICAgaWYocGFnZU51bWJlcikge1xuICAgICAgICAgICAgICAgIHBhZ2luYXRpb24udXBwZXJCb3VuZCA9IHBhZ2VOdW1iZXIgKiByZWNvcmRzUGVyUGFnZTtcbiAgICAgICAgICAgICAgICBwYWdpbmF0aW9uLmxvd2VyQm91bmQgPSBwYWdpbmF0aW9uLnVwcGVyQm91bmQgLSByZWNvcmRzUGVyUGFnZTtcbiAgICAgICAgICAgICAgICBwYWdpbmF0aW9uLm5leHRQYWdlID0gcGFnZU51bWJlcisrO1xuICAgICAgICAgICAgfWVsc2V7XG4gICAgICAgICAgICAgICAgcGFnaW5hdGlvbi51cHBlckJvdW5kID0gcmVjb3Jkc1BlclBhZ2U7XG4gICAgICAgICAgICB9XG4gICAgICAgIH1cblxuICAgICAgICByZXR1cm4gcGFnaW5hdGlvbjtcbiAgICB9XG5cbiAgICBwdWJsaWMgZ2V0UmVwb3J0RGF0YShzZXNzaW9uSWQ6IHN0cmluZywgc3RhcnREYXRlOiBhbnksIGVuZERhdGU6IGFueSk6IE9SQUNMRV9TRVJWSUNFX0NMT1VELklFeHRlbnNpb25Qcm9taXNlPGFueT4ge1xuICAgICAgICB2YXIgcHJvbWlzZU9iaiA9ICBuZXcgRXh0ZW5zaW9uUHJvbWlzZSgocmVzb2x2ZSxyZWplY3QpID0+IHtcbiAgICAgICAgICB0aGlzLnJlc29sdmVSZWYgPSByZXNvbHZlO1xuICAgICAgICAgIHRoaXMucmVqZWN0UmVmID0gcmVqZWN0O1xuICAgICAgICB9KTtcbiAgICAgICAgXG4gICAgICAgIHZhciBzdGFydERhdGVUaW1lOiBEYXRlO1xuICAgICAgICB2YXIgZW5kRGF0ZVRpbWU6IERhdGU7XG5cbiAgICAgICAgaWYoc3RhcnREYXRlKXtcbiAgICAgICAgICAgIHN0YXJ0RGF0ZVRpbWUgPSBuZXcgRGF0ZShzdGFydERhdGUpO1xuICAgICAgICB9ZWxzZXtcbiAgICAgICAgICAgIHN0YXJ0RGF0ZVRpbWUgPSBuZXcgRGF0ZSgpO1xuICAgICAgICAgICAgc3RhcnREYXRlVGltZS5zZXRIb3VycyhzdGFydERhdGVUaW1lLmdldEhvdXJzKCkgLSA4KTtcbiAgICAgICAgfVxuXG4gICAgICAgIGlmKGVuZERhdGUpe1xuICAgICAgICAgICAgZW5kRGF0ZVRpbWUgPSBuZXcgRGF0ZShlbmREYXRlKTtcbiAgICAgICAgfWVsc2V7XG4gICAgICAgICAgICBlbmREYXRlVGltZSA9IG5ldyBEYXRlKCk7XG4gICAgICAgIH1cblxuICAgICAgICB0aGlzLmZldGNoUmVwb3J0RGF0YSh0aGlzLnNlcnZlclVSSSwgdGhpcy5zZXNzaW9uSWQsIHN0YXJ0RGF0ZVRpbWUudG9JU09TdHJpbmcoKSwgZW5kRGF0ZVRpbWUudG9JU09TdHJpbmcoKSk7XG4gICAgICAgIHJldHVybiBwcm9taXNlT2JqO1xuICAgIH1cblxuICAgIHByaXZhdGUgZmV0Y2hSZXBvcnREYXRhKHNlcnZlclVyaTogc3RyaW5nLCBzZXNzaW9uSWQ6IHN0cmluZywgc3RhcnREYXRlOiBzdHJpbmcsIGVuZERhdGU6IHN0cmluZyk6IHZvaWQge1xuICAgICAgICBDdGlMb2dnZXIubG9nSW5mb01lc3NhZ2UodGhpcy5sb2dQcmVNZXNzYWdlICsgQ3RpTWVzc2FnZXMuTUVTU0FHRV9HRVRfQUdFTlRfREFUQSk7XG4gICAgICAgIHZhciBhcGlHZXRXb3JrZXJzOiBzdHJpbmcgPSAnaHR0cHM6Ly90YXNrcm91dGVyLnR3aWxpby5jb20vdjEvV29ya3NwYWNlcy97V09SS1NQQUNFX1NJRH0vV29ya2Vycyc7XG4gICAgICAgICQuYWpheCh7XG4gICAgICAgICAgICAgICAgdHlwZTogXCJQT1NUXCIsXG4gICAgICAgICAgICAgICAgdXJsOiBzZXJ2ZXJVcmksXG4gICAgICAgICAgICAgICAgZGF0YToge1xuICAgICAgICAgICAgICAgICAgICBzZXNzaW9uX2lkOiBzZXNzaW9uSWQsXG4gICAgICAgICAgICAgICAgICAgIHVyaTogYXBpR2V0V29ya2Vyc1xuICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgIH0pXG4gICAgICAgICAgICAuZG9uZSgod29ya2Vyc1Jlc3VsdDogc3RyaW5nKSA9PiB7XG4gICAgICAgICAgICAgICAgdmFyIGFsbFdvcmtlckRhdGE6IGFueSA9IEpTT04ucGFyc2Uod29ya2Vyc1Jlc3VsdCk7XG4gICAgICAgICAgICAgICAgaWYoYWxsV29ya2VyRGF0YSAmJiBhbGxXb3JrZXJEYXRhLndvcmtlcnMgJiYgYWxsV29ya2VyRGF0YS53b3JrZXJzLmxlbmd0aCA+IDApe1xuICAgICAgICAgICAgICAgICAgICB0aGlzLndvcmtlckNvdW50ID0gYWxsV29ya2VyRGF0YS53b3JrZXJzLmxlbmd0aDtcbiAgICAgICAgICAgICAgICAgICAgdGhpcy5mZXRjaFdvcmtlclN0YXRpc3RpY3MoYWxsV29ya2VyRGF0YS53b3JrZXJzLCBzZXJ2ZXJVcmksIHNlc3Npb25JZCwgc3RhcnREYXRlLCBlbmREYXRlKVxuICAgICAgICAgICAgICAgIH1lbHNle1xuICAgICAgICAgICAgICAgICAgICBDdGlMb2dnZXIubG9nSW5mb01lc3NhZ2UodGhpcy5sb2dQcmVNZXNzYWdlICsgQ3RpTWVzc2FnZXMuTUVTU0FHRV9OT19BR0VOVFNfRk9VTkQpO1xuICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgIH0pXG4gICAgICAgICAgICAuZmFpbCgobWVzc2FnZTogYW55KSA9PiB7XG4gICAgICAgICAgICAgICAgQ3RpTG9nZ2VyLmxvZ0Vycm9yTWVzc2FnZSh0aGlzLmxvZ1ByZU1lc3NhZ2UgK1xuICAgICAgICAgICAgICAgICAgICBDdGlNZXNzYWdlcy5NRVNTQUdFX0FQUEVOREVSICsgQ3RpTWVzc2FnZXMuTUVTU0FHRV9SRVBPUlRfRVhFQ1VUSU9OX0ZBSUxFRCArIG1lc3NhZ2UpO1xuICAgICAgICAgICAgfSk7XG4gICAgfVxuXG4gICAgcHJpdmF0ZSBmZXRjaFdvcmtlclN0YXRpc3RpY3MoYWxsV29ya2VyczogYW55W10sIHNlcnZlclVyaTogc3RyaW5nLCBzZXNzaW9uSWQ6IHN0cmluZyxcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBzdGFydERhdGU6IHN0cmluZywgZW5kRGF0ZTogc3RyaW5nKTogdm9pZCB7XG5cbiAgICAgICAgdmFyIHdvcmtlckNvdW50OiBudW1iZXIgPSBhbGxXb3JrZXJzLmxlbmd0aDtcblxuICAgICAgICBmb3IodmFyIGNvdW50PSAwOyBjb3VudDx3b3JrZXJDb3VudDsgY291bnQrKykge1xuICAgICAgICAgICAgdmFyIHdvcmtlcjogYW55ID0gYWxsV29ya2Vyc1tjb3VudF07XG4gICAgICAgICAgICB0aGlzLndvcmtlckRhdGFbd29ya2VyLnNpZF0gPSB7bmFtZTogd29ya2VyLmZyaWVuZGx5X25hbWV9O1xuXG4gICAgICAgICAgICB2YXIgYXBpV29ya2VyU3RhdDogc3RyaW5nID0gJ2h0dHBzOi8vdGFza3JvdXRlci50d2lsaW8uY29tL3YxL1dvcmtzcGFjZXMve1dPUktTUEFDRV9TSUR9L1dvcmtlcnMvJyt3b3JrZXIuc2lkKycvU3RhdGlzdGljcz9TdGFydERhdGU9JytzdGFydERhdGUrJyZFbmREYXRlPScrZW5kRGF0ZTtcbiAgICAgICAgICAgICQuYWpheCh7XG4gICAgICAgICAgICAgICAgICAgIHR5cGU6IFwiUE9TVFwiLFxuICAgICAgICAgICAgICAgICAgICB1cmw6IHNlcnZlclVyaSxcbiAgICAgICAgICAgICAgICAgICAgZGF0YToge1xuICAgICAgICAgICAgICAgICAgICAgICAgc2Vzc2lvbl9pZDogc2Vzc2lvbklkLFxuICAgICAgICAgICAgICAgICAgICAgICAgdXJpOiBhcGlXb3JrZXJTdGF0XG4gICAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICB9KVxuICAgICAgICAgICAgICAgIC5kb25lKCh3b3JrZXJSZXN1bHQ6IHN0cmluZykgPT4ge1xuICAgICAgICAgICAgICAgICAgICB2YXIgd29ya0RldGFpbDogYW55ID0gSlNPTi5wYXJzZSh3b3JrZXJSZXN1bHQpO1xuICAgICAgICAgICAgICAgICAgICB0aGlzLmFkZFdvcmtEZXRhaWxzKHdvcmtEZXRhaWwpO1xuICAgICAgICAgICAgICAgIH0pXG4gICAgICAgICAgICAgICAgLmZhaWwoKGRhdGE6IGFueSkgPT4ge1xuICAgICAgICAgICAgICAgICAgICB0aGlzLmFkZFdvcmtEZXRhaWxzKG51bGwpO1xuICAgICAgICAgICAgICAgIH0pO1xuICAgICAgICB9XG5cbiAgICB9XG5cbiAgICBwcml2YXRlIGFkZFdvcmtEZXRhaWxzKHdvcmtEZXRhaWw6IGFueSk6IHZvaWQge1xuICAgICAgICBpZih3b3JrRGV0YWlsICYmIHdvcmtEZXRhaWwud29ya2VyX3NpZCl7XG4gICAgICAgICAgICB0aGlzLndvcmtlckRhdGFbd29ya0RldGFpbC53b3JrZXJfc2lkXVsnd29yayddID0gd29ya0RldGFpbDtcbiAgICAgICAgfVxuICAgICAgICB0aGlzLmNvbXBsZXRlZENvdW50Kys7XG4gICAgICAgIGlmKHRoaXMuY29tcGxldGVkQ291bnQgPj0gdGhpcy53b3JrZXJDb3VudCl7XG4gICAgICAgICAgICBDdGlMb2dnZXIubG9nSW5mb01lc3NhZ2UodGhpcy5sb2dQcmVNZXNzYWdlICsgQ3RpTWVzc2FnZXMuTUVTU0FHRV9HRVRfQUdFTlRfREFUQV9DT01QTEVURUQpO1xuICAgICAgICAgICAgdGhpcy5wcm9jZXNzV29ya2VyRGF0YSgpO1xuICAgICAgICB9XG4gICAgfVxuXG4gICAgcHJpdmF0ZSBwcm9jZXNzV29ya2VyRGF0YSgpOiB2b2lkIHtcbiAgICAgICAgdmFyIHJlcG9ydDogUmVwb3J0Um93W10gPSBbXTtcbiAgICAgICAgdmFyIHJvd0NvdW50OiBudW1iZXIgPSAwO1xuICAgICAgICBDdGlMb2dnZXIubG9nSW5mb01lc3NhZ2UodGhpcy5sb2dQcmVNZXNzYWdlICsgQ3RpTWVzc2FnZXMuTUVTU0FHRV9QUk9DRVNTSU5HX0RBVEEpO1xuXG4gICAgICAgIGZvcih2YXIga2V5IGluIHRoaXMud29ya2VyRGF0YSl7XG4gICAgICAgICAgICBpZih0aGlzLndvcmtlckRhdGEuaGFzT3duUHJvcGVydHkoa2V5KSl7XG4gICAgICAgICAgICAgICAgdmFyIHdvcmtEZXRhaWxzOiBhbnkgPSB0aGlzLndvcmtlckRhdGFba2V5XTtcbiAgICAgICAgICAgICAgICB2YXIgcmVwb3J0Um93OiBSZXBvcnRSb3cgPSBuZXcgUmVwb3J0Um93KCk7XG4gICAgICAgICAgICAgICAgcmVwb3J0W3Jvd0NvdW50XSA9IHJlcG9ydFJvdztcbiAgICAgICAgICAgICAgICByZXBvcnRSb3cuYWdlbnROYW1lID0gd29ya0RldGFpbHMubmFtZS5zdWJzdHJpbmcod29ya0RldGFpbHMubmFtZS5sYXN0SW5kZXhPZignXycpKzEsIHdvcmtEZXRhaWxzLm5hbWUubGVuZ3RoKTtcbiAgICAgICAgICAgICAgICByb3dDb3VudCsrO1xuICAgICAgICAgICAgICAgIGlmKHdvcmtEZXRhaWxzLndvcmsgJiYgd29ya0RldGFpbHMud29yay5jdW11bGF0aXZlKXtcbiAgICAgICAgICAgICAgICAgICAgdmFyIHN0YXRpc3RpY3M6IGFueSA9IHdvcmtEZXRhaWxzLndvcmsuY3VtdWxhdGl2ZTtcbiAgICAgICAgICAgICAgICAgICAgcmVwb3J0Um93LnJlc2VydmF0aW9uQWNjZXB0ZWQgPSBzdGF0aXN0aWNzLnJlc2VydmF0aW9uc19hY2NlcHRlZDtcbiAgICAgICAgICAgICAgICAgICAgcmVwb3J0Um93LnJlc2VydmF0aW9uQ2FuY2VsbGVkID0gc3RhdGlzdGljcy5yZXNlcnZhdGlvbnNfY2FuY2VsZWQ7XG4gICAgICAgICAgICAgICAgICAgIHJlcG9ydFJvdy5yZXNlcnZhdGlvbkNyZWF0ZWQgPSBzdGF0aXN0aWNzLnJlc2VydmF0aW9uc19jcmVhdGVkO1xuICAgICAgICAgICAgICAgICAgICByZXBvcnRSb3cucmVzZXJ2YXRpb25SZWplY3RlZCA9IHN0YXRpc3RpY3MucmVzZXJ2YXRpb25zX3JlamVjdGVkO1xuICAgICAgICAgICAgICAgICAgICByZXBvcnRSb3cucmVzZXJ2YXRpb25UaW1lZE91dCA9IHN0YXRpc3RpY3MucmVzZXJ2YXRpb25zX3RpbWVkX291dDtcblxuICAgICAgICAgICAgICAgICAgICB2YXIgYWN0aXZpdHlEdXJhdGlvbnM6IGFueVtdID0gc3RhdGlzdGljcy5hY3Rpdml0eV9kdXJhdGlvbnM7XG4gICAgICAgICAgICAgICAgICAgIGlmKGFjdGl2aXR5RHVyYXRpb25zICYmIGFjdGl2aXR5RHVyYXRpb25zLmxlbmd0aCA+IDApe1xuICAgICAgICAgICAgICAgICAgICAgICAgZm9yKHZhciBpbmRleCBpbiBhY3Rpdml0eUR1cmF0aW9ucyl7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgaWYoYWN0aXZpdHlEdXJhdGlvbnMuaGFzT3duUHJvcGVydHkoaW5kZXgpKXtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdmFyIGFjdGl2aXR5RGF0YTogYW55ID0gYWN0aXZpdHlEdXJhdGlvbnNbaW5kZXhdO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB2YXIgZHVyYXRpb246IHN0cmluZyA9IChhY3Rpdml0eURhdGEudG90YWwvNjApLnRvRml4ZWQoMik7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHN3aXRjaChhY3Rpdml0eURhdGEuZnJpZW5kbHlfbmFtZSl7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBjYXNlICdOb3QgQXZhaWxhYmxlJzpcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICByZXBvcnRSb3cub2ZmbGluZUR1cmF0aW9uID0gZHVyYXRpb247XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgYnJlYWs7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBjYXNlICdCdXN5JzpcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICByZXBvcnRSb3cuYnVzeUR1cmF0aW9uID0gZHVyYXRpb247XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgYnJlYWs7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBjYXNlICdSZXNlcnZlZCc6XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgcmVwb3J0Um93LnJlc2VydmVkRHVyYXRpb24gPSBkdXJhdGlvbjtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBicmVhaztcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGNhc2UgJ1JlYWR5JzpcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICByZXBvcnRSb3cucmVhZHlEdXJhdGlvbiA9IGR1cmF0aW9uO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGJyZWFrO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgfVxuXG4gICAgICAgICAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICB9XG4gICAgICAgIH1cblxuICAgICAgICB0aGlzLnJlc29sdmVSZWYocmVwb3J0KTtcbiAgICAgICAgdGhpcy53b3JrZXJDb3VudCA9IG51bGw7XG4gICAgICAgIHRoaXMuY29tcGxldGVkQ291bnQgPSAwO1xuICAgICAgICB0aGlzLndvcmtlckRhdGEgPSB7fTtcbiAgICAgICAgQ3RpTG9nZ2VyLmxvZ0luZm9NZXNzYWdlKHRoaXMubG9nUHJlTWVzc2FnZSArIEN0aU1lc3NhZ2VzLk1FU1NBR0VfUFJPQ0VTU0lOR19EQVRBX0NPTVBMRVRFRCk7XG5cbiAgICB9XG5cbn1cbm5ldyBDdGlSZXBvcnRpbmdBZGRpbigpLnJlZ2lzdGVyQ3RpUmVwb3J0aW5nQWRpbigpO1xuIl19