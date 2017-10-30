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
 *  SHA1: $Id: 3653a81814e090a69a6ade1c9276e3ee0b6585c9 $
 * *********************************************************************************************
 *  File: reportRow.ts
 * ****************************************************************************************** */

export class ReportRow {
   agentName: string;
   reservationAccepted: number;
   reservationCancelled: number;
   reservationCreated: number;
   reservationRejected: number;
   reservationTimedOut: number;
   readyDuration: string;
   offlineDuration: string;
   reservedDuration: string;
   busyDuration: string;
}
