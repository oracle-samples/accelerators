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
 *  SHA1: $Id: a09d3563f373bbf8240f2b130334c5e3af8b99f2 $
 * *********************************************************************************************
 *  File: ctiMessages.ts
 * ****************************************************************************************** */

export class CtiMessages {

    //COMMON
    public static MESSAGE_APPENDER: string = ' >> ';

    //CtiMessagingAddin
    public static MESSAGE_ADDIN_INITIALIZED: string = 'CTI Messaging addin initialized.';
    public static MESSAGE_RETRY_CONTACT_FETCH: string = 'Retry fetching contact details ';
    public static MESSAGE_SEARCH_CONTACT: string = 'Searching for contact..';
    public static MESSAGE_SEARCH_COMPLETE: string = 'Contact search completed.';
    public static MESSAGE_SEARCH_FAILED: string = 'Contact search has failed.';
    public static MESSAGE_ENABLE_SMS_OPTION: string = 'Enabling SMS option..';
    public static MESSAGE_SEND_SMS: string = 'Sending message ..';
    public static MESSAGE_SEND_SMS_SUCCESS = 'SMS sent successfully';
    public static MESSAGE_SEND_SMS_FAILED = 'Sending SMS failed';
}