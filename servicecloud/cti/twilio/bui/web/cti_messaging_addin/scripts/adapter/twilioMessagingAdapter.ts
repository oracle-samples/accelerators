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
 *  SHA1: $Id: 7cb46288d1f68e66049dae8722290385e3c29019 $
 * *********************************************************************************************
 *  File: twilioMessagingAdapter.ts
 * ****************************************************************************************** */

import {ICtiMessagingAdapter} from "../contracts/iCtiMessagingAdapter";
import {Message} from "../model/message";
import {CtiMessageConfiguration} from "../model/ctiMessageConfiguration";
import {AgentProfile} from "../model/agentProfile";

/**
 * Implements the functionality defined by ICtiMessagingAdapter
 * 
 */
export class TwilioMessagingAdapter implements ICtiMessagingAdapter {

    private messageConfiguration: CtiMessageConfiguration;

    constructor() {
        this.messageConfiguration = <CtiMessageConfiguration> {
            providerName: 'Twilio',
            providerPath: 'cc/CTI'
        };
    }

    /**
     * This method submits a request to send message and
     * returns the jquery promise
     *
     * @param message
     * @param profileData
     * @returns {any}
     */
    public sendMessage(message: Message, profileData: AgentProfile): any {

        var messageUrl = profileData.interfaceUrl.match(/^[^\/]+:\/\/[^\/]+\//)[0].replace(/^http(?!s)/i, 'https')
            + this.messageConfiguration.providerPath+'/sendSMS';
        var incidentId = null;
        if(message.incidentWorkspace && message.incidentWorkspace.getWorkspaceRecordId() > 0){
            incidentId = message.incidentWorkspace.getWorkspaceRecordId();
        }

       return $.ajax({
            url: messageUrl,
            type: "POST",
            data: {
                session_id: profileData.sessionId,
                message: message.message,
                number: message.contact.phone,
                incident: incidentId
            },
            dataType: "JSON"
        });
    }

    /**
     * this function search for a contact
     *
     * @param cId
     * @param sessionId
     * @param serverUri
     * @param phone
     */
    public searchContact(phone: string, cId: string, sessionId: string, serverUri: string): any {
        var searchUrl = serverUri.match(/^[^\/]+:\/\/[^\/]+\//)[0].replace(/^http(?!s)/i, 'https')
            + this.messageConfiguration.providerPath+'/searchPhone';

        return $.ajax({
            url: searchUrl,
            type: 'POST',
            data: {
                phone: phone,
                session_id: sessionId,
                id: cId
            }
        });
    }

}