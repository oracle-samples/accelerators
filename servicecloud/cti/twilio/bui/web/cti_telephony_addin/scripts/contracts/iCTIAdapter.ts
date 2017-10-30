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
 *  SHA1: $Id: ffbd21b76e97f806beaeef64dae6913064b0a201 $
 * *********************************************************************************************
 *  File: iCTIAdapter.ts
 * ****************************************************************************************** */

import {AgentProfile} from "./../model/agentProfile";
import {CtiConfiguration} from "./../model/ctiConfiguration";

/**
 * ICTIAdapter - This is the contract between CTI Client
 * and the server. Any communication to the backend (CTI Platform Provider)
 * should only go through this adapter
 *
 */
export interface ICTIAdapter {
  /**
   * Implementation should provide login functionality
   * with the underlying
   * CTI Tool/Platform
   *
   * @param profileData
   */
  login(profileData: AgentProfile): void;

  /**
   * Implementation should provide functionality to add
   * different event handlers
   * 
   * @param eventName
   * @param handler
   */
  addEventHandler(eventName: string, handler: any): void;

  /**
   * Implementation should provide functionality to remove
   * handler for an event
   * @param eventName
   */
  clearEventHandler(eventName: string): void;

  /**
   * Implementation should provide functionality to remove all
   * registered event handlers
   */
  clearAllEventHandlers(): void;

  /**
   * Implementation should provide logout functionality
   * from the underlying
   * CTI Tool/Platform
   *
   */
  logout(): void;

  /**
   * Implementation should provide functionality to update
   * activity
   *
   * @param status
   */
  updateActivity(status: string): void;

  /**
   * Implementation should return
   * the Basic CTI Tool Configuration
   * data required by the Client
   *
   */
  getConfiguration(): CtiConfiguration;

  /**
   * Implementation should search for a contact
   *
   * @param contact
   * @param sessionId
   */
  searchContact(contact: string, sessionId: string): void;

  /**
   *Implementation should support making outgoing calls
   * using the underlying CTI platform
   *
   * @param contact
   */
  dialANumber(contact: string): void;

  /**
   * Implementation should return a list of all available agents
   * Available agents - Agents with status 'Ready/Available' to accept a call
   *
   * @param sessionId
   */
  searchAvailableAgents(sessionId: string) : void;

  /**
   * Implementation should authorize the agent for CTI Access
   *
   * @param interfaceUrl
   * @param sessionId
   */
  authorizeAgent(interfaceUrl: string, sessionId: string): void;

  /**
   * Implementation should transfer a call to a given worker
   *
   * @param sessionId
   * @param workerId
   * @param incidentId
   */
  transferCall(sessionId: string, workerId: string, incidentId ?: number): void;

  /**
   *Implementation should renew the capability token
   *
   * @param sessionId
   */
  renewCtiToken(sessionId: string);

  /**
   * Implementation should be able to log messages at server side
   *
   * @param sessionId
   * @param message
     */
  logMessage(sessionId: string, message: string);

  /**
   * Implementation should return the URL for incoming notification
   *
   */
  getRingMediaUrl(): string;
}
