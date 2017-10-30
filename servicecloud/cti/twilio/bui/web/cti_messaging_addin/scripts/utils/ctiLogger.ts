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
 *  SHA1: $Id: 7aee75fba9cc0c7e4bdcff10cad5c40a81d03253 $
 * *********************************************************************************************
 *  File: ctiLogger.ts
 * ****************************************************************************************** */

import {LogLevels} from "./logLevels";
export class CtiLogger {
    public static logMessage(logLevel: LogLevels, message: string): void {
        if(logLevel) {
            switch (logLevel){
                case LogLevels.ERROR:
                    CtiLogger.logErrorMessage(message);
                    break;
                case LogLevels.INFO:
                    CtiLogger.logInfoMessage(message);
                    break;
                case LogLevels.WARN:
                    CtiLogger.logWarningMessage(message);
            }
        }
    }

    public static logWarningMessage(message: string): void {
        console.warn('CTILogger >> WARNING >> '+message);
    }

    public static logErrorMessage(message: string): void {
        console.error('CTILogger >> ERROR >> '+message);
    }

    public static logInfoMessage(message: string): void {
        console.log('CTILogger >> INFO >> '+message);
    }

}