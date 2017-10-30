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
 *  File: ctiReportingAddin.ts
 * ****************************************************************************************** */

///<reference path='../../../definitions/osvcExtension.d.ts' />

import $ = require('jquery');
import {ReportRow} from "./../model/reportRow";
import {CtiMessages} from "../util/ctiMessages";
import {CtiLogger} from "../util/ctiLogger";
import {Pagination} from "../model/pagination";
export class CtiReportingAddin {

    private workerCount: number;
    private completedCount: number = 0;
    private workerData: {[key: string]: any} = {};
    private serverURI: string;
    private resolveRef: any; private rejectRef:any;
    private sessionId: string;
    private globalContext: ORACLE_SERVICE_CLOUD.IExtensionGlobalContext;
    private logPreMessage: string = 'CtiReportingAddin ' + CtiMessages.MESSAGE_APPENDER;

    public registerCtiReportingAdin(): void{
        CtiLogger.logInfoMessage(this.logPreMessage + CtiMessages.MESSAGE_ADDIN_REGISTER);
        ORACLE_SERVICE_CLOUD.extension_loader.load('DataListenerApp').then(
            (extensionProvider : ORACLE_SERVICE_CLOUD.IExtensionProvider) =>{
                extensionProvider.registerAnalyticsExtension(
                    (analyticsContext: any) => {
                        analyticsContext.addTableDataRequestListener(
                            'accelerator$CtiAgentStats', (report: ORACLE_SERVICE_CLOUD.IExtensionReport) => this.reportDataSourceHandler(report)
                        );
                        CtiLogger.logInfoMessage(this.logPreMessage + CtiMessages.MESSAGE_ADDIN_INITIALIZED);
                    }
                );
                extensionProvider.getGlobalContext().then(
                    (globalContext: ORACLE_SERVICE_CLOUD.IExtensionGlobalContext) => {
                        this.globalContext = globalContext;
                        this.serverURI = globalContext.getInterfaceUrl().match(/^[^\/]+:\/\/[^\/]+\//)[0].replace(/^http(?!s)/i, 'https') +
                                'cc/CTI/twilioProxy';
                        globalContext.getSessionToken().then(
                            (sessionToken: string) => {
                                this.sessionId = sessionToken;
                            }
                        );
                    }
                )
            }
        );
    }

    public reportDataSourceHandler(report: ORACLE_SERVICE_CLOUD.IExtensionReport): any{
        CtiLogger.logInfoMessage(this.logPreMessage + CtiMessages.MESSAGE_START_REPORT_EXECUTION);
        return new ExtensionPromise( (resolve: any, reject: any) => {
            var startDate: any;
            var endDate: any;
            var pageNumber: number;
            var rowsPerPage: number;

            let filterList: ORACLE_SERVICE_CLOUD.IExtensionFilter[] = report.getReportFilters().getFilterList();
            pageNumber = report.getReportFilters().getPageNumber();
            rowsPerPage = report.getReportFilters().getRowsPerPage();
            
            for(let i=0; i<filterList.length; i++){
                let filter: ORACLE_SERVICE_CLOUD.IExtensionFilter = filterList[i];
                if(filter.getColumnReference() === 'accelerator$CtiAgentStats.fromDate'){
                    startDate = filter.getValue();
                }else if(filter.getColumnReference() === 'accelerator$CtiAgentStats.toDate'){
                    endDate = filter.getValue();
                }
            }
            var reportData = report.createReportData();

            this.getReportData(this.sessionId, startDate, endDate).then(
                (reportRows: ReportRow[]) => {
                    var totalRecordsCount: number = reportRows.length;
                    reportData.setTotalRecordCount(totalRecordsCount);
                    if(!pageNumber){
                        pageNumber = 0;
                    }
                    if(!rowsPerPage){
                        rowsPerPage = 10;
                    }
                    var pagination: Pagination = this.getPaginationData(pageNumber, rowsPerPage, totalRecordsCount);

                    for(var i = pagination.lowerBound; i< pagination.upperBound; i++){
                        let reportRow: ReportRow = reportRows[i];
                        var row = report.createReportDataRow();

                        var agentId: any = reportRow.agentName;
                        if(!isNaN(agentId)){
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
                }
            );
        });
    }
    
    private getPaginationData(pageNumber: number, recordsPerPage: number, totalRecords: number): Pagination {
        var pagination: Pagination = new Pagination;
        pagination.lowerBound = 0;
        pagination.upperBound = 0;
        pagination.totalRecords = totalRecords;
        pagination.recordsPerPage = recordsPerPage;

        if(recordsPerPage >= totalRecords || (pageNumber * recordsPerPage) >= totalRecords){
            pagination.upperBound = totalRecords;
        }else{
            if(pageNumber) {
                pagination.upperBound = pageNumber * recordsPerPage;
                pagination.lowerBound = pagination.upperBound - recordsPerPage;
                pagination.nextPage = pageNumber++;
            }else{
                pagination.upperBound = recordsPerPage;
            }
        }

        return pagination;
    }

    public getReportData(sessionId: string, startDate: any, endDate: any): ORACLE_SERVICE_CLOUD.IExtensionPromise<any> {
        var promiseObj =  new ExtensionPromise((resolve,reject) => {
          this.resolveRef = resolve;
          this.rejectRef = reject;
        });
        
        var startDateTime: Date;
        var endDateTime: Date;

        if(startDate){
            startDateTime = new Date(startDate);
        }else{
            startDateTime = new Date();
            startDateTime.setHours(startDateTime.getHours() - 8);
        }

        if(endDate){
            endDateTime = new Date(endDate);
        }else{
            endDateTime = new Date();
        }

        this.fetchReportData(this.serverURI, this.sessionId, startDateTime.toISOString(), endDateTime.toISOString());
        return promiseObj;
    }

    private fetchReportData(serverUri: string, sessionId: string, startDate: string, endDate: string): void {
        CtiLogger.logInfoMessage(this.logPreMessage + CtiMessages.MESSAGE_GET_AGENT_DATA);
        var apiGetWorkers: string = 'https://taskrouter.twilio.com/v1/Workspaces/{WORKSPACE_SID}/Workers';
        $.ajax({
                type: "POST",
                url: serverUri,
                data: {
                    session_id: sessionId,
                    uri: apiGetWorkers
                }
            })
            .done((workersResult: string) => {
                var allWorkerData: any = JSON.parse(workersResult);
                if(allWorkerData && allWorkerData.workers && allWorkerData.workers.length > 0){
                    this.workerCount = allWorkerData.workers.length;
                    this.fetchWorkerStatistics(allWorkerData.workers, serverUri, sessionId, startDate, endDate)
                }else{
                    CtiLogger.logInfoMessage(this.logPreMessage + CtiMessages.MESSAGE_NO_AGENTS_FOUND);
                }
            })
            .fail((message: any) => {
                CtiLogger.logErrorMessage(this.logPreMessage +
                    CtiMessages.MESSAGE_APPENDER + CtiMessages.MESSAGE_REPORT_EXECUTION_FAILED + message);
            });
    }

    private fetchWorkerStatistics(allWorkers: any[], serverUri: string, sessionId: string,
                                  startDate: string, endDate: string): void {

        var workerCount: number = allWorkers.length;

        for(var count= 0; count<workerCount; count++) {
            var worker: any = allWorkers[count];
            this.workerData[worker.sid] = {name: worker.friendly_name};

            var apiWorkerStat: string = 'https://taskrouter.twilio.com/v1/Workspaces/{WORKSPACE_SID}/Workers/'+worker.sid+'/Statistics?StartDate='+startDate+'&EndDate='+endDate;
            $.ajax({
                    type: "POST",
                    url: serverUri,
                    data: {
                        session_id: sessionId,
                        uri: apiWorkerStat
                    }
                })
                .done((workerResult: string) => {
                    var workDetail: any = JSON.parse(workerResult);
                    this.addWorkDetails(workDetail);
                })
                .fail((data: any) => {
                    this.addWorkDetails(null);
                });
        }

    }

    private addWorkDetails(workDetail: any): void {
        if(workDetail && workDetail.worker_sid){
            this.workerData[workDetail.worker_sid]['work'] = workDetail;
        }
        this.completedCount++;
        if(this.completedCount >= this.workerCount){
            CtiLogger.logInfoMessage(this.logPreMessage + CtiMessages.MESSAGE_GET_AGENT_DATA_COMPLETED);
            this.processWorkerData();
        }
    }

    private processWorkerData(): void {
        var report: ReportRow[] = [];
        var rowCount: number = 0;
        CtiLogger.logInfoMessage(this.logPreMessage + CtiMessages.MESSAGE_PROCESSING_DATA);

        for(var key in this.workerData){
            if(this.workerData.hasOwnProperty(key)){
                var workDetails: any = this.workerData[key];
                var reportRow: ReportRow = new ReportRow();
                report[rowCount] = reportRow;
                reportRow.agentName = workDetails.name.substring(workDetails.name.lastIndexOf('_')+1, workDetails.name.length);
                rowCount++;
                if(workDetails.work && workDetails.work.cumulative){
                    var statistics: any = workDetails.work.cumulative;
                    reportRow.reservationAccepted = statistics.reservations_accepted;
                    reportRow.reservationCancelled = statistics.reservations_canceled;
                    reportRow.reservationCreated = statistics.reservations_created;
                    reportRow.reservationRejected = statistics.reservations_rejected;
                    reportRow.reservationTimedOut = statistics.reservations_timed_out;

                    var activityDurations: any[] = statistics.activity_durations;
                    if(activityDurations && activityDurations.length > 0){
                        for(var index in activityDurations){
                            if(activityDurations.hasOwnProperty(index)){
                                var activityData: any = activityDurations[index];
                                var duration: string = (activityData.total/60).toFixed(2);
                                switch(activityData.friendly_name){
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
        CtiLogger.logInfoMessage(this.logPreMessage + CtiMessages.MESSAGE_PROCESSING_DATA_COMPLETED);

    }

}
new CtiReportingAddin().registerCtiReportingAdin();
