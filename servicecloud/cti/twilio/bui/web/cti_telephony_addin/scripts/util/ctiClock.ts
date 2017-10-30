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
 *  SHA1: $Id: b3c07ff2750e7b693d477e22315ca27ce2ec8590 $
 * *********************************************************************************************
 *  File: ctiClock.ts
 * ****************************************************************************************** */

import $ = require('jquery');

export class CtiClock {
    private worker: Worker;
    private elementId: string;
    private callLength: string;
    private callStartTime: Date;
    private callEndTime: Date;
    private ctiToken: string = 'ORACLE_OSVC_CTI';
    private callDuration: string = '00:00:00';
    private isRunning: boolean = false;
    
    public constructor(elementId: string) {
        this.elementId = elementId;
    }

    public startClock(): void {
        this.callStartTime = new Date();
        this.isRunning = true;
        if(typeof(Worker) !== "undefined") {
            this.worker = new Worker('../scripts/util/ctiClockWorker.js');
            this.worker.onmessage = (event: any) => {
                if(event && event.data){
                    var data: any = JSON.parse(event.data);
                    if(data.token === this.ctiToken){
                        this.callDuration = data.duration;
                        this.callLength = data.duration;
                        $('#'+this.elementId).html(data.duration);
                    }
                }
            };
            
            this.worker.postMessage(JSON.stringify({token: this.ctiToken, command: 'START'}));
        }
    }

    public resetUI():void {
        $('#'+this.elementId).html(this.callDuration);
    }

    public stopClock(): void {
        if(this.isRunning) {
            this.worker.terminate();
            this.callEndTime = new Date();
            this.callLength = this.callDuration;
            this.callDuration = '00:00:00';
            $('#'+this.elementId).html(this.callLength);
            this.isRunning = false;
        }
    }

    /**
     * This method returns the call duration as a string
     *
     * @returns {string}
     */
    public getCallLength(): string {
        return this.callLength;
    }

    /**
     * This method returns the start time of the clock as a date object
     * @returns {Date}
     */
    public getClockStartTime(): Date {
        return this.callStartTime;
    }

    /**
     * This method returns the end time of clock as a date object
     *
     * @returns {Date}
     */
    public getClockEndTime(): Date {
        return this.callEndTime? this.callEndTime : new Date();
    }
}