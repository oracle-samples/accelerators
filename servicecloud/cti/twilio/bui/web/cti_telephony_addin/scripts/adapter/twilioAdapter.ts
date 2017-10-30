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
 *  SHA1: $Id: 2985e7d6db45a10cda4b4295fab9e6896d301030 $
 * *********************************************************************************************
 *  File: twilioAdapter.ts
 * ****************************************************************************************** */

import {ICTIAdapter} from '../contracts/iCTIAdapter';
import {AgentProfile} from '../model/agentProfile';
import {CtiConfiguration} from '../model/ctiConfiguration';
import {CtiConstants} from "../util/ctiConstants";
import {TwilioCommunicationHandler} from "./twilioCommunicationHandler";
import {CtiLogger} from "../util/ctiLogger";
import {CtiMessages} from "../util/ctiMessages";

/**
 *This class implements the ICTIAdapter interface, which will be used by the
 * adddin to communicate with underlying CTI platform
 * 
 */
export class TwilioAdapter implements ICTIAdapter {
  private ctiConfiguration: CtiConfiguration;
  private eventHandlers: {[key: string]: any};
  private serverCommunicationHandler: TwilioCommunicationHandler;
  private isHandlersAttached: boolean;
  private logPreMessage: string = 'TwilioAdapter' + CtiMessages.MESSAGE_APPENDER;
  private serverUrl: string;

  constructor() {
    this.ctiConfiguration = <CtiConfiguration>{
      providerName: 'Twilio',
      providerPath: 'cc/CTI',
      defaultStatus: 'Ready',
      recordByDefault: false
    };
    this.eventHandlers = {};
    this.serverCommunicationHandler = new TwilioCommunicationHandler();
  }
  /**
   * This function handles login to the underlying
   * CTI Tool/Platform
   *
   * @param profileData
     */
  public login(profileData: AgentProfile): void {
    var loginUrl = profileData.interfaceUrl.match(/^[^\/]+:\/\/[^\/]+\//)[0].replace(/^http(?!s)/i, 'https') + this.ctiConfiguration.providerPath;
    this.eventHandlers = {};
    this.serverCommunicationHandler.login(loginUrl, profileData.sessionId, profileData.accountId);
  }

  /**
   * Adds handler for an event. This events will be used by the adapter
   * while logging in to register with the tool
   *
   * @param eventName
   * @param handler
   */
  public addEventHandler(eventName: string, handler: any): void {
    this.eventHandlers[eventName] = handler;
    //Currently one handler per event is allowed
    this.serverCommunicationHandler.getServerEventHandler()
        .off(eventName).on(eventName, handler);

    CtiLogger.logInfoMessage(this.logPreMessage + CtiMessages.MESSAGE_HANDLER_ADDED
      + CtiMessages.MESSAGE_APPENDER + eventName);
  }

  /**
   * Clears handler associated with an event
   *
   * @param eventName
   */
  public clearEventHandler(eventName: string): void {
    this.eventHandlers[eventName] = null;
    this.serverCommunicationHandler.getServerEventHandler().off(eventName);
    CtiLogger.logInfoMessage(this.logPreMessage +
        CtiMessages.MESSAGE_HANDLER_REMOVED + CtiMessages.MESSAGE_APPENDER + eventName);
  }

  /**
   * Clears all event handlers added to the adapter
   *
   */
  public clearAllEventHandlers(): void {
    CtiLogger.logInfoMessage(this.logPreMessage + CtiMessages.MESSAGE_ALL_HANDLERS_REMOVED);
    for(var key in this.eventHandlers) {
      if(key) {
        this.eventHandlers[key] = null;
        this.serverCommunicationHandler.getServerEventHandler().off(key);
      }
    }
    this.isHandlersAttached = false;
  }

  /**
   * This function handles logout from the underlying
   * CTI Tool/Platform
   *
   */
  public logout(): void {
    this.serverCommunicationHandler.logout();
    this.clearAllEventHandlers();
  }

  /**
   * This function handles activity updates to the underlying
   * CTI Tool/Platform
   *
   * @param status
   */
  public updateActivity(status: string): void {
    this.serverCommunicationHandler.updateActivity(status);
  }

  /**
   * This function returns the basic CTI configuration required by the
   * addin
   *
   * @returns {CtiConfiguration}
   */
  public getConfiguration(): CtiConfiguration {
    return this.ctiConfiguration;
  }

  /**
   * This function returns the handler associated with an event
   *
   * @param eventName
   * @returns {any}
   */
  public getEventHandler(eventName: string): any {
    return this.eventHandlers[eventName];
  }

  /**
   * this function search for a contact
   *
   * @param contact
   * @param sessionId
   */
  public searchContact(contact: string, sessionId: string): void {
    this.serverCommunicationHandler.searchContact(contact, sessionId);
  }

  /**
   * Make an outbound call with the given
   * contact
   *
   * @param contact
     */
  public dialANumber(contact: string): void {
    this.serverCommunicationHandler.dialANumber(contact);
  }

  /**
   * Searches for all available agents in the system
   *
   * @param sessionId
     */
  public searchAvailableAgents(sessionId: string): void {
    this.serverCommunicationHandler.getAvailableAgents(sessionId);
  }

  /**
   * Authorize agent for CTI Access
   *
   * @param interfaceUrl
   * @param sessionId
     */
  public authorizeAgent(interfaceUrl: string, sessionId: string): void {
    this.serverUrl = interfaceUrl.match(/^[^\/]+:\/\/[^\/]+\//)[0].replace(/^http(?!s)/i, 'https');
    this.serverCommunicationHandler.isCtiEnabled(interfaceUrl, this.ctiConfiguration.providerPath, sessionId);
  }

  /**
   * Transfer a call to another agent(agent with a given workerId)
   *
   * @param sessionId
   * @param workerId
   * @param incidentId
     */
  public transferCall(sessionId: string, workerId: string, incidentId ?: number): void {
    this.serverCommunicationHandler.transferCurrentCall(sessionId, workerId, incidentId);
  }

  /**
   * Initiates a request to renew the CTI Capability Token
   * @param sessionId
     */
  public renewCtiToken(sessionId: string) {
    this.serverCommunicationHandler.renewToken(sessionId)
  }

  /**
   * Logs a message at the server side
   *
   * @param sessionId
   * @param message
     */
  public logMessage(sessionId: string, message: string): void {
    this.serverCommunicationHandler.logAuditMessage(sessionId, new Date() + message);
  }

  /**
   * Returns the url of incoming notification audio
   * @returns {string}
     */
  public getRingMediaUrl(): string {
    //TODO - Correct this url
    return 'https://rameshtr.info/ring.mp3';
    //return this.serverUrl + this.ctiConfiguration.providerPath +'/ringMedia';
  }
}
