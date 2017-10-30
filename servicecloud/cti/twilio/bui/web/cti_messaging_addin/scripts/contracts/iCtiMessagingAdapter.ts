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
 *  SHA1: $Id: 609f4b24f615da6309858f594fceb6cdd91c4b09 $
 * *********************************************************************************************
 *  File: iCtiMessagingAdapter.ts
 * ****************************************************************************************** */

import {Message} from "../model/message";
import {AgentProfile} from "../model/agentProfile";
export interface ICtiMessagingAdapter {

    /**
     * Implementation should send a message
     * 
     * @param message
     * @param profileData
     */
    sendMessage(message: Message,profileData: AgentProfile): any;

    /**
     * Implementation should search for a contact
     *
     * @param cId
     * @param sessionId
     * @param serverUri
     * @param phone
     */
    searchContact(phone: string, cId: string, sessionId: string, serverUri: string): any;
}