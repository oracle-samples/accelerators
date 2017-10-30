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
 *  File: ctiClockWorker.ts
 * ****************************************************************************************** */

module ORACLE_CTI {
    export class CtiClockWorker {
        private clockHours: number;
        private clockMinutes: number;
        private clockSeconds: number;
        private callDuration: string;
        private isClockStarted: boolean = false;
        private ctiToken: string ='ORACLE_OSVC_CTI';

        public constructor() {
            this.clockHours = 0;
            this.clockMinutes = 0;
            this.clockSeconds = 0;
            this.callDuration = '00:00:00';

            addEventListener('message', (event: any)=> {
                if(!this.isClockStarted && event && event.data){
                    var data: any = JSON.parse(event.data);
                    if(data && data.token === this.ctiToken && data.command === 'START') {
                        this.runClock();
                        this.isClockStarted = true;
                    }
                }
            });
        }

        public runClock = () => {
            this.clockSeconds++;
            if (this.clockSeconds === 60) {
                this.clockMinutes++;
                this.clockSeconds = 0;

                if (this.clockMinutes === 60) {
                    this.clockHours++;
                    this.clockMinutes = 0;
                }
            }

            this.callDuration = (this.clockHours < 10 ? '0' + this.clockHours : this.clockHours) + ':' +
                (this.clockMinutes < 10 ? '0' + this.clockMinutes : this.clockMinutes) + ':' +
                (this.clockSeconds < 10 ? '0' + this.clockSeconds : this.clockSeconds);

            var postMethod: any = postMessage;
            postMethod(JSON.stringify({token: this.ctiToken, duration: this.callDuration}));

            setTimeout(this.runClock, 1000);
        }
    }
}

new ORACLE_CTI.CtiClockWorker();