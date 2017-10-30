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
 *  SHA1: $Id: 2415247e88a6b0fc326b8b343967a25c131c6a20 $
 * *********************************************************************************************
 *  File: twilioCommunicationHandler.ts
 * ****************************************************************************************** */

///<reference path="../../../definitions/twilio.d.ts" />

import {ServerEventHandler} from "./serverEventHandler";
import $ = require('jquery');
import {CtiConstants} from "../util/ctiConstants";
import {CtiLogger} from "../util/ctiLogger";
import {Contact} from "../model/contact";
import {CtiMessages} from "../util/ctiMessages";

/**
 * TwilioCommunicationHandler - All communication with the custom controller
 *  is handled in this class. Here we submit requests to the server and despatch (publish)
 *  associated events based on result.
 */
export class TwilioCommunicationHandler {
    private activities: any = {};
    private loggedIn: boolean = false;

    private workerName: string;
    private sessionId: string;
    private agentId: string;
    private serverURI: string;
    private deviceToken: any;
    private workerToken: any;
    private worker: any;
    private reservation: any;
    private contactInfo: any;
    private serverEventHandler:ServerEventHandler;
    private routes: any;
    private incidentId: any;
    private attributes: any;
    private dequeueConfigId: string;
    private firstLogin: boolean = true;
    private timeoutHandle: number;
    private ICE: any;
    private dialedContact: Contact;
    private callSid: string;

    private logPreMessage: string = 'TwilioCommunicationHandler' + CtiMessages.MESSAGE_APPENDER;

    constructor() {
        this.serverEventHandler = new ServerEventHandler();
    }

    /**
     * Submits a login request with server. Also
     * it publishes 'login.success' and 'login.failed' events
     *
     *
     * @param serverURI
     * @param sessionId
     * @param agentId
     */
    public login(serverURI, sessionId, agentId): any {
        this.sessionId = sessionId;
        this.agentId = agentId;
        this.serverURI = serverURI.replace(/\/$/, "");
        CtiLogger.logInfoMessage(this.logPreMessage + CtiMessages.MESSAGE_LOGIN);

        $.ajax({
            url: this.serverURI + "/login",
            type: "POST",
            data: {
                session_id: this.sessionId
            },
            dataType: "JSON"
        })
            .done((data: any) => {

                if(data && !data.success){
                    CtiLogger.logErrorMessage(this.logPreMessage + CtiMessages.MESSAGE_LOGIN_ERROR_DEVICE +
                        JSON.stringify(data));
                    this.serverEventHandler.despatch( 'login.failed', data );
                    return;
                }
                try{
                    this.initializeTwilioDevice(data);
                    this.firstLogin = false;
                }catch(exception) {
                    CtiLogger.logErrorMessage(this.logPreMessage +
                        CtiMessages.MESSAGE_EVENT_DISPATCH + JSON.stringify(exception));
                    this.serverEventHandler.despatch( 'login.failed', {} );
                }
            })
            .fail( (failureMessage: any) => {
                CtiLogger.logErrorMessage(this.logPreMessage +
                    CtiMessages.MESSAGE_LOGIN_FAILURE + JSON.stringify(failureMessage));
                this.serverEventHandler.despatch( 'login.failed', name );
        });

    }

    /**
     * Submits a status update to the server with
     * status Offline
     */
    public logout(): void {
        CtiLogger.logInfoMessage(this.logPreMessage + CtiMessages.MESSAGE_LOGOUT);
        this.updateActivity(CtiConstants.NOT_AVAILABLE);
        this.workerName = null;
        this.worker = null;
        this.sessionId = null;
        this.agentId = null;
        this.loggedIn = false;
    }

    /**
     * Submits status update request to the server.
     * @param name
     */
    public updateActivity(name: string): void {
        if( !this.loggedIn )
        {
            throw(CtiMessages.MESSAGE_NOT_LOGGEDIN_FOR_ACTION);
        }
        if( name in this.activities )
        {
            CtiLogger.logInfoMessage(this.logPreMessage +
                CtiMessages.MESSAGE_REQUEST_ACTIVITY_UPDATE +
                CtiMessages.MESSAGE_APPENDER + name);
            this.worker.update("ActivitySid", this.activities[name], (error, worker) => {
                if(error) {
                    CtiLogger.logErrorMessage(this.logPreMessage +
                        CtiMessages.MESSAGE_ACTIVITY_UPDATE_ERROR + JSON.stringify(error));
                } else {
                    CtiLogger.logInfoMessage(this.logPreMessage + worker.activityName);
                }
            });
        }
    }

    /**
     * Returns the currently associated
     * instance of ServerEventHandler
     *
     * @returns {ServerEventHandler}
     */
    public getServerEventHandler():ServerEventHandler {
        return this.serverEventHandler;
    }

    /**
     * Search for a given contact.
     * Publish events 'search.contact.complete' and 'search.contact.failed'
     *
     * @param contact
     * @param sessionId
     */
    public searchContact(contact: string, sessionId: string): void {
        CtiLogger.logInfoMessage(this.logPreMessage +
            CtiMessages.MESSAGE_CONTACT_SEARCH +
            CtiMessages.MESSAGE_APPENDER + contact);
        $.ajax({
            url: this.serverURI + '/searchPhone',
            type: 'POST',
            data: {
                phone: contact,
                session_id: sessionId
            }
        }).done((searchResult: any) => {
            CtiLogger.logInfoMessage(this.logPreMessage +
                CtiMessages.MESSAGE_CONTACT_SEARCH_SUCCESS +
                CtiMessages.MESSAGE_APPENDER + searchResult);
            var searchResultJson: any = JSON.parse(searchResult);
            this.dialedContact = searchResultJson.contact;
            this.serverEventHandler.despatch('search.contact.complete', searchResultJson);
        }).fail((message: any) => {
            CtiLogger.logErrorMessage(this.logPreMessage + 'Contact search failed >> '+message);
            this.serverEventHandler.despatch('search.contact.failed', {success: false});
        });
    }

    /**
     * Make an outbound call
     *
     * @param contact
     */
    public dialANumber(contact: string): void {
        CtiLogger.logInfoMessage(this.logPreMessage +
            CtiMessages.MESSAGE_DIALING +
            CtiMessages.MESSAGE_APPENDER + contact);
        Twilio.Device.connect({To: contact,
        Direction: 'outbound'});
    }


    /**
     * Search for all available agents.
     * Publish event - 'search.agentlist.complete'
     *
     * @param sessionId
     */
    public getAvailableAgents(sessionId: string): void {
        CtiLogger.logInfoMessage(this.logPreMessage +
            CtiMessages.MESSAGE_SEARCH_AVAILABLE_AGENTS);
        this.sessionId = sessionId;
        $.ajax({
            url: this.serverURI + '/getConnectedAgents',
            type: 'POST',
            data: {
                session_id: sessionId
            }
        }).done((searchResult: string) => {
            CtiLogger.logInfoMessage(this.logPreMessage +
                CtiMessages.MESSAGE_AGENT_SEARCH_SUCCESS +
                CtiMessages.MESSAGE_APPENDER + searchResult);
            this.serverEventHandler.despatch('search.agentlist.complete', JSON.parse(searchResult));
        }).fail((message: any) => {
            CtiLogger.logErrorMessage(this.logPreMessage +
                CtiMessages.MESSAGE_AGENT_SEARCH_FAILURE +
                CtiMessages.MESSAGE_APPENDER + message);
            this.serverEventHandler.despatch('search.agentlist.complete', []);
        });
    }

    /**
     * Check for CTI access for the given agent.
     * Publish events - 'cti.enabled' and 'cti.disabled'
     *
     * @param interfaceUrl
     * @param servicePath
     * @param sessionId
     */
    public isCtiEnabled(interfaceUrl: string, servicePath: string, sessionId: string): void {
        CtiLogger.logInfoMessage(this.logPreMessage + CtiMessages.MESSAGE_CTI_AUTHORIZE);
        var requestUrl = interfaceUrl.match(/^[^\/]+:\/\/[^\/]+\//)[0].replace(/^http(?!s)/i, 'https')
            + servicePath + '/isCTIEnabled';

        $.ajax({
            url: requestUrl,
            type: 'POST',
            data: {
                session_id: sessionId
            }
        }).done((data: string) => {
            var jsonData: any = JSON.parse(data);
            if(jsonData && jsonData.enabled){
                CtiLogger.logInfoMessage(this.logPreMessage + CtiMessages.MESSAGE_CTI_ENABLED);
                this.serverEventHandler.despatch('cti.enabled', {});
            }else{
                this.serverEventHandler.despatch('cti.disabled', {});
                CtiLogger.logInfoMessage(this.logPreMessage + CtiMessages.MESSAGE_CTI_DISABLED);
            }
        }).fail((data: any) => {
            this.serverEventHandler.despatch('cti.disabled', {});
            CtiLogger.logErrorMessage(this.logPreMessage +
                CtiMessages.MESSAGE_CTI_AUTHORIZATION_FAILURE +
                CtiMessages.MESSAGE_APPENDER + data);
        });
    }

    /**
     * Submits transfer call request to the server.
     *
     * @param sessionId
     * @param workerId
     * @param incidentId
     */
    public transferCurrentCall(sessionId: string, workerId: string, incidentId: number): void {
        CtiLogger.logInfoMessage(this.logPreMessage +
            CtiMessages.MESSAGE_REQUEST_CALL_TRANSFER +
            CtiMessages.MESSAGE_APPENDER + workerId);
        this.sessionId = sessionId;
        var lookup: boolean = false;
        if(!this.attributes){ //Outbound call
            lookup = true;
            this.attributes = {
                callSid: this.callSid,
                contact: this.dialedContact,
                incident: null
            }
        }

        if(incidentId){
            this.attributes.incident = incidentId;
        }else{
            this.attributes.incident = null;
        }

        $.ajax({
            url: this.serverURI + '/transferCall',
            type: 'POST',
            data: {
                session_id: sessionId,
                attributes: JSON.stringify(this.attributes),
                worker: workerId,
                lookup: lookup
            }
        });
    }

    /**
     * Renews the TWILIO capability token for
     * Worker and Device
     *
     * @param sessionId
     */
    public renewToken (sessionId: string): void{
        $.ajax({
                type: "POST",
                url: this.serverURI + "/renewTokens",
                data: {
                    session_id: sessionId,
                    tokens: 'device,worker'
                }
            })
            .done( (data: any) => {
                CtiLogger.logInfoMessage(this.logPreMessage + CtiMessages.MESSAGE_UPDATE_DEVICE);
                
                var jsonData: any = JSON.parse(data);
                this.workerToken = jsonData.worker;
                this.deviceToken = jsonData.device;

                CtiLogger.logInfoMessage(this.logPreMessage + jsonData);

                Twilio.Device.setup(this.deviceToken, {
                    iceServers: this.ICE
                });

                try{
                    this.worker.updateToken( this.workerToken );
                    CtiLogger.logInfoMessage(this.logPreMessage + CtiMessages.MESSAGE_UPDATE_DEVICE_SUCCESS);
                }
                catch(message){
                    CtiLogger.logErrorMessage(this.logPreMessage +
                        CtiMessages.MESSAGE_TOKEN_UPDATE_FAILURE +
                        CtiMessages.MESSAGE_APPENDER + message);
                }
            })
            .fail((message: any) => {
                CtiLogger.logErrorMessage(this.logPreMessage +
                    CtiMessages.MESSAGE_TOKEN_UPDATE_FAILURE +
                    CtiMessages.MESSAGE_APPENDER + message);
            })
    };

    /**
     * Log a given message on server
     *
     * @param sessionId
     * @param message
     */
    public logAuditMessage(sessionId: string, message: string) {
        $.ajax({
                type: "POST",
                url: this.serverURI + "/logCallAction",
                data: {
                    session_id: sessionId,
                    action: message
                }
            })
            .done( (data: any) => {
                CtiLogger.logInfoMessage(this.logPreMessage +
                    CtiMessages.MESSAGE_LOG_ACTION +
                    CtiMessages.MESSAGE_APPENDER + message);
            })
            .fail((message: any) => {
                CtiLogger.logErrorMessage(this.logPreMessage +
                    CtiMessages.MESSAGE_LOG_ATION_FAILURE +
                    CtiMessages.MESSAGE_APPENDER + message);
            })
    }


    /**
     * initializeTwilioDevice - This method initializes the Twilio Device on login success.
     *
     * Attaching handlers for Twilio.Device.incoming and Twilio.Device.connect
     * has to be done only once on first login. Repeated attachments will result
     * in multiple invocation of handlers.
     *
     * @param data
     */
    private initializeTwilioDevice = (data: any) => {
        this.dequeueConfigId = data.config.number;
        this.deviceToken = data.device;
        this.workerToken = data.worker;
        this.routes = data.config.routes;
        this.ICE =data.ICE;

        if(this.firstLogin){
            CtiLogger.logInfoMessage(this.logPreMessage +
                CtiMessages.MESSAGE_DEVICE_INITIALIZE );

            Twilio.Device.setup( this.deviceToken, {
                iceServers: data.ICE
            } );

            Twilio.Device.incoming( (conn: any) => {
                CtiLogger.logInfoMessage(this.logPreMessage +
                    CtiMessages.MESSAGE_INCOMING_CONNECTION+ conn.parameters.From);
                conn.accept();
            });

            Twilio.Device.connect( (conn: any) => {
                CtiLogger.logInfoMessage(this.logPreMessage +
                    CtiMessages.MESSAGE_CONNECTION_ESTABLISHED);
                var data = {
                    contact: this.contactInfo,
                    incident: this.incidentId,

                    hangup: () => {
                        conn.disconnect();
                        this.updateActivity("Ready");
                    },

                    mute: ( flag ) => {
                        conn.mute( flag );
                    }
                };
                this.callSid = conn.parameters.CallSid;
                this.serverEventHandler.despatch( "connected", data );
            });

            Twilio.Device.disconnect( (conn: any) => {
                CtiLogger.logInfoMessage(this.logPreMessage +
                    CtiMessages.MESSAGE_CONNECTION_BROKE);
                this.attributes =null;
                this.incidentId = null;
                this.updateActivity("Ready");
                this.serverEventHandler.despatch( "disconnected", {} );
            });
        }

        if(this.workerToken == null){
            throw("Worker token is null. Cannot provide access to Twilio for this user.");
        }

        this.worker = new Twilio.TaskRouter.Worker( this.workerToken );

        this.worker.on("ready",  ( worker: any ) => {
            CtiLogger.logInfoMessage(this.logPreMessage + "Worker " + worker.friendlyName + " is ready");
            this.loggedIn = true;
            CtiLogger.logInfoMessage(this.logPreMessage +
                CtiMessages.MESSAGE_DISPATCH_LOGIN_SUCCESS);
            this.serverEventHandler.despatch( 'login.success', name );
            this.updateActivity("Ready");
        });

        this.worker.on("reservation.created", (reservation: any) => {
            CtiLogger.logInfoMessage(this.logPreMessage +
                CtiMessages.MESSAGE_RESERVATION_CREATED);
            this.reservation = reservation;
            var phone = reservation.task.attributes.from;
            this.attributes = reservation.task.attributes;
            phone = phone.replace("client:","");
            var timeout = 30;

            var contact: any = reservation.task.attributes.contact;
            this.incidentId = reservation.task.attributes.incident;

            contact['name'] = contact.firstName + " " + contact.lastName;
            this.contactInfo = contact;

            var data = {
                timeout: timeout,

                accept: () => {
                    clearTimeout( this.timeoutHandle );
                    reservation.dequeue( this.dequeueConfigId );
                },
                reject: () => {
                    clearTimeout( this.timeoutHandle );
                    this.attributes = null;
                    this.incidentId = null;
                    reservation.reject();
                }
            };

            data["contact"] = contact;
            this.timeoutHandle = setTimeout( () => {
                this.serverEventHandler.despatch( "timeout", {} );
                if(this.reservation) {
                    this.reservation.reject();
                    this.reservation = null;
                }
                this.attributes = null;
                this.incidentId = null;
            }, timeout * 1000 );
            this.serverEventHandler.despatch( "incoming", data );
        });

        this.worker.on("reservation.canceled", (reservation) => {
            CtiLogger.logInfoMessage(this.logPreMessage +
                CtiMessages.MESSAGE_RESERVATION_CANCELLED);
            this.attributes = null;
            this.incidentId = null;
            clearTimeout( this.timeoutHandle );
            this.serverEventHandler.despatch( "canceled", {} );
        });

        this.worker.on("reservation.accepted", (reservation) => {
            clearTimeout( this.timeoutHandle );
            CtiLogger.logInfoMessage(this.logPreMessage +
                CtiMessages.MESSAGE_RESERVATION_ACCEPTED);
        });

        this.worker.on("reservation.rejected", (reservation) => {
            clearTimeout( this.timeoutHandle );
            this.attributes = null;
            this.incidentId = null;
            CtiLogger.logInfoMessage(this.logPreMessage +
                CtiMessages.MESSAGE_RESERVATION_REJECTED);
            this.serverEventHandler.despatch( "canceled", {} );
        });

        this.worker.on("token.expired", () => {
            CtiLogger.logInfoMessage(this.logPreMessage +
                CtiMessages.MESSAGE_TOKEN_EXPIRED);
            this.serverEventHandler.despatch('token.expired', {});
        });

        this.worker.on("activity.update", (worker) => {
            this.serverEventHandler.despatch( 'activity.update', worker.activityName );
        });

        this.worker.activities.fetch(
            (error: any, activityList: any) => {
                if(error) {
                    CtiLogger.logErrorMessage(this.logPreMessage + error.code);
                    CtiLogger.logErrorMessage(this.logPreMessage + error.message);
                    return;
                }
                var data = activityList.data;
                for(var i=0; i<data.length; i++) {
                    this.activities[data[i].friendlyName] = data[i].sid;
                }

                this.worker.update("ActivitySid", this.activities["Ready"], (error, worker) => {
                    if(error) {
                        CtiLogger.logErrorMessage(this.logPreMessage + error.code);
                        CtiLogger.logErrorMessage(this.logPreMessage + error.message);
                    } else {
                        CtiLogger.logInfoMessage(worker.activityName);
                    }
                });
            }
        );
    };

}