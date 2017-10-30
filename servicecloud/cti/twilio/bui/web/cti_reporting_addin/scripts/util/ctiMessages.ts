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
 *  SHA1: $Id: 7f293da59449c0ed8543a2b7835e3a88505224f3 $
 * *********************************************************************************************
 *  File: ctiMessages.ts
 * ****************************************************************************************** */

export class CtiMessages {

    //COMMON
    public static MESSAGE_APPENDER: string = ' >> ';

    //CtiReportingAddin
    public static MESSAGE_ADDIN_REGISTER: string = 'Initializing CTI Reporting Addin.';
    public static MESSAGE_ADDIN_INITIALIZED: string = 'CTI Reporting addin initialized..';
    public static MESSAGE_START_REPORT_EXECUTION: string = 'Initializing report execution..';
    public static MESSAGE_GET_AGENT_DATA: string = 'Fetching agent statistics..';
    public static MESSAGE_REPORT_EXECUTION_FAILED: string = 'Report execution failed due to ';
    public static MESSAGE_NO_AGENTS_FOUND: string = 'No agent data found.';
    public static MESSAGE_GET_AGENT_DATA_COMPLETED: string = 'Fetching agent data completed..';
    public static MESSAGE_PROCESSING_DATA: string = 'Processing agent data..';
    public static MESSAGE_PROCESSING_DATA_COMPLETED: string = 'Data processing completed.';
}