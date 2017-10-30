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
 *  SHA1: $Id: 3736c865a28707b8427d97a183f3b231a615ac9d $
 * *********************************************************************************************
 *  File: serverEventHandler.ts
 * ****************************************************************************************** */

import $ = require('jquery');
import {CtiLogger} from "../util/ctiLogger";
import {CtiMessages} from "../util/ctiMessages";
/**
 * ServerEventHandler - This class defines and handles the events used for communication
 * between addin and the adapter. This acts as pub-sub system for the CTI addin.
 *
 * It stores handlers for various events and invokes them when an event occurs.
 *
 * We can add required events in the store and register handlers. later when we despatch
 * an event, the corresponding handler will be invoked
 */
export class ServerEventHandler {

  private store:any = {
    // Internal Events
    'reservation.created': [],
    'reservation.timeout': [],
    'token.expired': [],

    // Application level events
    'cti.enabled': [],
    'cti.disabled': [],
    'activity.update': [],
    'login.success': [],
    'login.failed': [],
    'incoming': [],
    'connected': [],
    'disconnected': [],
    'timeout': [],
    'canceled': [],
    'search.contact.complete': [],
    'search.contact.failed': [],
    'search.agentlist.complete': []
  };

  private logPreMessage: string = 'ServerEventHandler' + CtiMessages.MESSAGE_APPENDER;

  /**
   * Binds handler to an event
   *
   * @param handle
   * @param event
   * @returns {this}
   */
  public on(handle: any, event: any):ServerEventHandler {
    if (typeof event !== 'function') {
      if (( typeof event === 'object' ) && ( typeof event.func !== 'function' )) {
        return;
      }
    }

    if (!( handle in this.store )) {
      return;
    }

    this.store[handle].push(event);
    return this;
  }

  /**
   * Removes handler for an event
   *
   * @param handle
   * @returns {this}
   */
  public off(handle: any):ServerEventHandler {
    if (!( handle in this.store )) {
      return;
    }

    this.store[handle] = [];
    return this;
  }

  /**
   * Invoke an event handler along with associated data
   *
   * @param handle
   * @param data
   * @returns {this}
   */
  public despatch(handle: any, data: any):ServerEventHandler {
    if (!( handle in this.store )) {
      return;
    }
    CtiLogger.logInfoMessage(this.logPreMessage + CtiMessages.MESSAGE_EVENT_DISPATCH
      + CtiMessages.MESSAGE_APPENDER + handle + CtiMessages.MESSAGE_WITH_DATA + CtiMessages.MESSAGE_APPENDER + JSON.stringify(data));
    $.each(this.store[handle], (idx)=> {
      var func = this.store[handle][idx];
      if (typeof func === 'object') {
        func.count--;
        func.func(data);
        if (func.count <= 0) {
          this.store[handle].splice(idx, 1);
        }
        return this;
      }
      func(data);
    });
    return this;
  }

}
