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
 *  SHA1: $Id: 71acfc7d13754b0f4f22eb3444e673fc5c809609 $
 * *********************************************************************************************
 *  File: ctiMessageViewHelper.ts
 * ****************************************************************************************** */

import $ = require('jquery');
import {Message} from "../model/message";

/**
 * This utility class is used to update the UI
 */
export class CtiMessageViewHelper {
    /**
     * Enable messaging for a workspace
     *
     * @param message
     * @param inputListener
     * @param buttonClickHandler
     */
    public static enableMessagingView(message: Message, inputListener: any, buttonClickHandler: any): void {
        var inputElement: any = $("#messaging_content");
        $("#messaging_contact_image").attr('src', message.contact.dp);
        $("#messaging_name").html(message.contact.name);
        $('#messaging_contact_number').html(message.contact.phone);
        inputElement.val(message.message);
        inputElement.off().on('change', (event: any) => {
            inputListener(message.key, event);
        });
        var sendButtonElement: any = $("#message_send_btn");
        sendButtonElement.off().on('click', (event: any) => {
            sendButtonElement.attr('class', 'button button-green disabled');
            buttonClickHandler(message.key);
        });
    }

    /**
     * Change UI when sending message is success
     */
    public static enableSendButtonControlOnSuccess(): void {
        var sendButtonElement: any = $("#message_send_btn");
        var inputElement: any = $("#messaging_content");
        inputElement.val('');
        sendButtonElement.attr('class', 'button button-green');
    }

    /**
     * Change UI when sending message is failure
     */
    public static enableSendButtonControlOnFailure(): void {
        var sendButtonElement: any = $("#message_send_btn");
        alert('Unable to send message.');
        sendButtonElement.attr('class', 'button button-green');
    }

}
