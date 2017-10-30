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
 *  SHA1: $Id: 84a31f3a91e5278351baec144ed58014898aeac5 $
 * *********************************************************************************************
 *  File: ctiTelephonyAddin.ts
 * ****************************************************************************************** */

///<reference path='../../../definitions/osvcExtension.d.ts' />

import {CtiConstants} from './../util/ctiConstants';
import {ICTIAdapter} from '../contracts/iCTIAdapter';
import {CtiConfiguration} from '../model/ctiConfiguration';
import {IconData} from '../model/iconData';
import {CtiViewHelper} from './../util/ctiViewHelper';
import {AgentProfile} from "../model/agentProfile";
import IExtensionProvider = ORACLE_SERVICE_CLOUD.IExtensionProvider;
import IUserInterfaceContext = ORACLE_SERVICE_CLOUD.IUserInterfaceContext;
import IGlobalHeaderContext = ORACLE_SERVICE_CLOUD.IGlobalHeaderContext;
import IGlobalHeaderMenu = ORACLE_SERVICE_CLOUD.IGlobalHeaderMenu;
import IICon = ORACLE_SERVICE_CLOUD.IICon;
import IGlobalHeaderMenuItem = ORACLE_SERVICE_CLOUD.IGlobalHeaderMenuItem;
import IWorkspaceRecord = ORACLE_SERVICE_CLOUD.IWorkspaceRecord;
import IIncidentWorkspaceRecord = ORACLE_SERVICE_CLOUD.IIncidentWorkspaceRecord;
import ISidePaneContext = ORACLE_SERVICE_CLOUD.ISidePaneContext;
import ISidePane = ORACLE_SERVICE_CLOUD.ISidePane;
import {TwilioAdapter} from "../adapter/twilioAdapter";
import {AgentData} from "../model/agentData";
import {CtiLogger} from "../util/ctiLogger";
import {CtiClock} from "../util/ctiClock";
import {CtiMessages} from "../util/ctiMessages";

/**
 * CtiTelephonyAddin - This class controls the whole Client side
 * addin operations.
 *
 * The CtiTelephonyAddin is designed as a left SidePane addin
 *
 * An instance of the ICTIAdapter implementation
 * has to be given at the time of creation, which will be used
 * for communicating with CTI Platform
 *
 * CtiTelephonyAddin will not make any direct communication with
 * the underlying CTI Platform/Server. All communication should be
 * through CTIAdapter
 *
 * CtiTelephonyAddin utilize event based communication with the
 * underlying CTI Platform. The addin will register handlers
 * for various events with the system and the system invoke corresponding
 * handler when an event occurs.
 *
 */
export class CtiTelephonyAddin {

    private extensionSdk:IExtensionProvider;
    private globalHeaderContext:IGlobalHeaderContext;
    private interactionWorkspace:IWorkspaceRecord;
    private workspaceRecord:IWorkspaceRecord;
    private incidentWorkspace:IIncidentWorkspaceRecord;
    private buiCtiLeftPanelMenu:ISidePane;
    private globalHeaderMenu:IGlobalHeaderMenu;

    private ctiProviderConfig:CtiConfiguration;
    private ctiAdapter:ICTIAdapter;
    private ctiInCallClock:CtiClock;
    private prevStatus:string;
    private isLoggedIn:boolean = false;
    private onCall:boolean = false;
    private isNotifyAllowed:boolean = true;
    private callerData:any;
    private dialedContact:any;
    private isOutGoing:boolean;
    private menuItemLogin:IGlobalHeaderMenuItem;
    private isCallSummarized: boolean = false;
    private globalContext: ORACLE_SERVICE_CLOUD.IExtensionGlobalContext;
    private agentProfile: AgentProfile;
    private ringMedia: any;
    private onCallTransfer: boolean = false;

    private logPreMessage: string = 'CtiTelephonyAddin' + CtiMessages.MESSAGE_APPENDER;

    constructor(ctiClientAdapter:ICTIAdapter) {
        this.ctiAdapter = ctiClientAdapter;
    }

    /**
     * This method initiates the authorization process. It checks the CTI access for
     * currently logged-in user and enables the addin if it is accessible to the
     * agent.
     *
     * It registers a handler for event - 'cti.enabled' with the CTIAdapter.
     * This handler will enable addin for agent. Underlying CTI Adapter
     * authorize the agent and invokes the handler if CTI is enabled for current agent
     *
     */
    public enableCtiAddin():void {
        //Register handler only for enabling addin
        CtiLogger.logInfoMessage(this.logPreMessage+ CtiMessages.MESSAGE_LOAD_EXTENSION);
        this.ctiAdapter.addEventHandler('cti.enabled', this.initializeCtiToolBarMenu);
        ORACLE_SERVICE_CLOUD.extension_loader.load(CtiConstants.BUI_CTI_ADDIN_ID, CtiConstants.BUI_CTI_ADDIN_VERSION)
            .then((sdk:IExtensionProvider) => {
                this.extensionSdk = sdk;
                CtiLogger.logInfoMessage(this.logPreMessage+
                    CtiMessages.MESSAGE_OBTAINED_SDK);
                sdk.getGlobalContext().then(
                    (globalContext:ORACLE_SERVICE_CLOUD.IExtensionGlobalContext) => {
                        this.globalContext = globalContext;
                        globalContext.getSessionToken().then((sessionToken:string) => {
                            this.ctiAdapter.authorizeAgent(globalContext.getInterfaceUrl(), sessionToken);
                        });
                    });
            });
    }

    /**
     * This method initialize the Toolbar Menu Icon
     * for CTI Addin. This is the icon
     * from where Agent can login to the CTI Tool
     *
     */
    public initializeCtiToolBarMenu = ():void => {
        CtiLogger.logInfoMessage(this.logPreMessage+
            CtiMessages.MESSAGE_INITIALIZE_ADDIN);
        this.initializeRingMediaElement();
        this.enableDialPadControls();
        this.registerUnloadHandler();

        //Load Menubar addin
        this.initializeLeftpaneIcon();
        this.extensionSdk.registerUserInterfaceExtension((userInterfaceContext:IUserInterfaceContext) => {
            userInterfaceContext.getGlobalHeaderContext()
                .then((ribbonBarContext:IGlobalHeaderContext) => {
                        this.globalHeaderContext = ribbonBarContext;
                        this.ctiProviderConfig = this.ctiAdapter.getConfiguration();
                        ribbonBarContext.getMenu(CtiConstants.BUI_CTI_RIBBONBAR_MENU_ID)
                            .then((ribbonBarMenu:IGlobalHeaderMenu) => {
                                this.globalHeaderMenu = ribbonBarMenu;
                                var icon:IICon = ribbonBarMenu.createIcon(CtiConstants.BUI_CTI_RIBBONBAR_ICON_TYPE);
                                icon.setIconClass(CtiConstants.BUI_CTI_RIBBONBAR_ICON_DEFAULT_CLASS);
                                icon.setIconColor(CtiConstants.BUI_CTI_RIBBONBAR_ICON_DEFAULT_COLOR);
                                ribbonBarMenu.addIcon(icon);
                                ribbonBarMenu.setDisabled(false);
                                ribbonBarMenu.setLabel(this.ctiProviderConfig.providerName + ' ' + CtiConstants.BUI_CTI_LABEL_LOGGED_OUT);

                                this.menuItemLogin = ribbonBarMenu.createMenuItem();
                                this.menuItemLogin.setLabel(CtiConstants.BUI_CTI_LABEL_LOGIN);
                                this.menuItemLogin.setHandler(
                                    (menuItem:IGlobalHeaderMenuItem) => {
                                        this.handleLogin();
                                    });
                                ribbonBarMenu.addMenuItem(this.menuItemLogin);
                                ribbonBarMenu.render();
                            });
                        this.isLoggedIn = false;
                    }
                );
        });
    };

    /**
     * Create the audio element which is used to play
     * the 'ring' for incoming calls
     */
    public initializeRingMediaElement(): void {
        this.ringMedia = document.createElement('audio');
        this.ringMedia.setAttribute('src', this.ctiAdapter.getRingMediaUrl());
        this.ringMedia.addEventListener('ended', function() {
            this.play();
        }, false);
    }

    /**
     * Initializes the left side icon. This icon is used to control
     * the dialer/on-call UI. This icon will be kept hidden until user
     * login to CTI
     */
    public initializeLeftpaneIcon():void {
        CtiLogger.logInfoMessage(this.logPreMessage+
            CtiMessages.MESSAGE_INITIALIZE_SIDEPANEL);
        this.extensionSdk.registerUserInterfaceExtension((userInterfaceContext:IUserInterfaceContext) => {
            userInterfaceContext.getLeftSidePaneContext().then(
                (leftSidePaneContext:ISidePaneContext) => {
                    leftSidePaneContext.getSidePane(CtiConstants.BUI_CTI_LEFT_PANEL_MENU_ID)
                        .then((leftPanelMenu:ISidePane) => {
                            this.buiCtiLeftPanelMenu = leftPanelMenu;
                            leftPanelMenu.setLabel('');
                            leftPanelMenu.setVisible(false);
                            var icon = leftPanelMenu.createIcon(CtiConstants.BUI_CTI_RIBBONBAR_ICON_TYPE);
                            icon.setIconClass(CtiConstants.BUI_CTI_LEFT_PANEL_ICON);
                            leftPanelMenu.addIcon(icon);
                            leftPanelMenu.render();
                        });
                }
            );
        });
    }

    /**
     * registerListenersToAdapter - Register listerns
     * with the CTI adapter for events to be handled
     *
     * The CTIAdapter will invoke required handlers, when an
     * event occurs.
     *
     */
    public registerListenersToAdapter():void {
        //Clear any previous handlers
        this.ctiAdapter.clearAllEventHandlers();
        CtiLogger.logInfoMessage(this.logPreMessage + CtiMessages.MESSAGE_REGISTER_EVENT_HANDLERS);
        //Register handlers
        this.ctiAdapter.addEventHandler('login.success', this.handleLoginSuccess);
        this.ctiAdapter.addEventHandler('activity.update', this.handleStatusUpdatesFromServer);
        this.ctiAdapter.addEventHandler('login.failed', this.handleLoginFailure);
        this.ctiAdapter.addEventHandler('incoming', this.handleIncoming);
        this.ctiAdapter.addEventHandler('connected', this.handleCallConnected);
        this.ctiAdapter.addEventHandler('disconnected', this.handleCallDisconnected);
        this.ctiAdapter.addEventHandler('canceled', this.handleCallCancelled);
        this.ctiAdapter.addEventHandler('timeout', this.handleCallTimeOut);
        this.ctiAdapter.addEventHandler('search.contact.failed', this.contactSearchHandler);
        this.ctiAdapter.addEventHandler('search.contact.complete', this.contactSearchHandler);
        this.ctiAdapter.addEventHandler('search.agentlist.complete', this.handleAgentSearchSuccess);
        this.ctiAdapter.addEventHandler('token.expired', this.tokenExpiryHandler);
    }

    /**
     * This function adds event listeners to the
     * CTI Dialer
     */
    public enableDialPadControls():void {
        CtiViewHelper.addDialPadControls(this.outgoingCallHandler);
    }

    /**
     * This method initiates an outbound call. It triggers a search
     * for the given contact and renders the on-call UI. The actual
     * call will not be triggered from here. It will be triggered
     * after a the search completion
     *
     * @param event
     */
    public outgoingCallHandler = (event:any):void => {
        var dialedNumber:string = CtiViewHelper.getOutgoingContactNumber();
        this.logCallAction(CtiMessages.MESSAGE_HANDLE_OUTGOING_CALL +
            CtiMessages.MESSAGE_APPENDER + dialedNumber);
        if (dialedNumber !== '') {
            this.updateAgentStatus(CtiConstants.BUSY);
            this.onCall = true;
            this.isNotifyAllowed = false;

            this.globalContext.getSessionToken().then((sessionToken:string) => {
                this.ctiAdapter.searchContact(CtiViewHelper.getOutgoingContactNumber(), sessionToken);
            });

            CtiViewHelper.showOnCallUI(dialedNumber, CtiConstants.DEFAULT_DISPLAY_ICON);
        }
    };

    /**
     * Handler for events - 'search.contact.complete' and 'search.contact.failed'
     * In both cases we update the on-call UI with search results (Either contact name OR Unknown).
     * Also the actual call is tirggered from here.
     *
     * @param searchResult
     */
    public contactSearchHandler = (searchResult) => {
        this.dialedContact = searchResult.contact;
        this.isOutGoing = true;
        CtiViewHelper.updateOutgoingContactDetails(searchResult.contact.name, searchResult.contact.dp);
        this.ctiAdapter.dialANumber(CtiViewHelper.getOutgoingContactNumber().trim());
    };

    /**
     *
     *
     * @param event
     */
    public outgoingHangupHandler = (event:any) => {
        CtiViewHelper.renderOutgoingHangupUI(event);
        this.onCall = false;
        this.isNotifyAllowed = true;
    };

    /**
     * handleLogin - Initiate a login request
     * with the CTI tool, using the CTIAdapter instance
     */
    public handleLogin = ():void => {
        CtiLogger.logInfoMessage(this.logPreMessage + CtiMessages.MESSAGE_INITIATE_LOGIN);
        this.registerListenersToAdapter();
        if (!this.isLoggedIn && this.globalHeaderMenu) {
            this.updateGlobalHeaderMenuIcon(CtiConstants.WAIT);
            this.globalContext.getSessionToken().then((sessionToken:string) => {
                var agentProfile:AgentProfile = <AgentProfile> {
                    interfaceUrl: this.globalContext.getInterfaceUrl(),
                    accountId: '' + this.globalContext.getAccountId(),
                    sessionId: sessionToken
                };
                this.agentProfile = agentProfile;
                this.ctiAdapter.login(agentProfile);
            });
        } else {
            CtiLogger.logWarningMessage(this.logPreMessage +
                CtiMessages.MESSAGE_PARTIAL_LOGOUT);
        }
    };

    /**
     * updateIconsAfterLogin - Take care of
     * rendering the leftpanel icon, toolbar menu icon
     * after a successful login with the CTI Tool
     *
     * It does the following operations
     *
     * 1. Update the ribbonbar menu with status update
     *    and logout options
     * 2. Update the ribbonbar icon and label corresponding to the
     *    default status provided in the CTI Configuration
     * 3. Render the leftpanel icon with the default status
     *
     */
    public handleLoginSuccess = (data: any):void => {
        if(this.isLoggedIn){
            CtiLogger.logInfoMessage(this.logPreMessage +
                CtiMessages.MESSAGE_ALREADY_LOGGED_IN);
            this.ctiAdapter.updateActivity(this.ctiProviderConfig.defaultStatus);
            return;
        }
        CtiLogger.logInfoMessage(this.logPreMessage +
            CtiMessages.MESSAGE_UI_UPDATE_AFTER_LOGIN_SUCCESS);
        this.globalHeaderContext.getMenu(CtiConstants.BUI_CTI_RIBBONBAR_MENU_ID)
            .then((ribbonBarMenu:IGlobalHeaderMenu) => {
                this.globalHeaderMenu = ribbonBarMenu;
                this.menuItemLogin.dispose();

                //1. Update Ribbonbar menu
                this.updateRibbonbarMenuAfterLogin();

                //2. Update leftpanel menu
                this.updateLeftPanelMenuAfterLogin();

                this.isLoggedIn = true;
                this.prevStatus = this.ctiProviderConfig.defaultStatus;

            });
    };

    /**
     * This method reset the login icon, if login fails
     */
    public handleLoginFailure = (data: any):void => {
        if (this.globalHeaderMenu) {
            var icon:IICon = this.globalHeaderMenu.createIcon(CtiConstants.BUI_CTI_RIBBONBAR_ICON_TYPE);
            icon.setIconClass(CtiConstants.BUI_CTI_RIBBONBAR_ICON_DEFAULT_CLASS);
            icon.setIconColor(CtiConstants.BUI_CTI_RIBBONBAR_ICON_DEFAULT_COLOR);
            this.globalHeaderMenu.addIcon(icon);
            this.globalHeaderMenu.setDisabled(false);
            this.globalHeaderMenu.setLabel(this.ctiProviderConfig.providerName + ' ' + CtiConstants.BUI_CTI_LABEL_LOGGED_OUT);
            this.globalHeaderMenu.render();
        } else {
            this.initializeCtiToolBarMenu();
        }
    };

    /**
     * Updates the UI according to the status updates from CTI Platform
     *
     * @param ctiUpdateData
     */
    public handleStatusUpdatesFromServer = (ctiUpdateData:any) => {
        CtiLogger.logInfoMessage(this.logPreMessage +
            CtiMessages.MESSAGE_CLIENT_STATUS+
            CtiMessages.MESSAGE_APPENDER + this.prevStatus);
        CtiLogger.logInfoMessage(this.logPreMessage +
            CtiMessages.MESSAGE_SERVER_STATUS +
            CtiMessages.MESSAGE_APPENDER + ctiUpdateData);
        if (this.prevStatus !== ctiUpdateData) {
            this.updateAgentUIOnServerUpdate(ctiUpdateData);
            this.prevStatus = status;
        }
    };

    /**
     * Handler for event - 'incoming'
     *
     * This is the handle for 'incoming' event. This handler will be invoked by the
     * CTI Adapter when there is an incoming call for the agent
     *
     * @param ctiData
     */
    public handleIncoming = (ctiData:any):void => {
        CtiLogger.logInfoMessage(this.logPreMessage +
            CtiMessages.MESSAGE_HANDLE_CALL_INCOMING);
        if (!this.onCall && this.isNotifyAllowed) {
            this.isNotifyAllowed = false;
            if(!ctiData.contact.email){
                ctiData.contact.email = CtiMessages.MESSAGE_MAIL_NOT_AVAILABLE;
            }
            CtiViewHelper.renderIncomingView(ctiData, this.ringMedia);

            this.buiCtiLeftPanelMenu.setLabel(CtiConstants.BUI_CTI_LABEL_INCOMING_CALL);
            var icon = this.buiCtiLeftPanelMenu.createIcon(CtiConstants.BUI_CTI_LEFT_PANEL_ICON_TYPE);
            icon.setIconClass(CtiConstants.BUI_CTI_LEFT_PANEL_ICON_NOTIFY);
            icon.setIconColor('red');

            this.buiCtiLeftPanelMenu.addIcon(icon);
            this.buiCtiLeftPanelMenu.expand();
            this.buiCtiLeftPanelMenu.render();

            setTimeout(() => {
                if (!this.onCall) {
                    this.buiCtiLeftPanelMenu.setLabel(this.ctiProviderConfig.providerName + ' ' + CtiConstants.BUI_CTI_LABEL_AVAILABLE);
                    var icon = this.buiCtiLeftPanelMenu.createIcon(CtiConstants.BUI_CTI_LEFT_PANEL_ICON_TYPE);
                    icon.setIconClass(CtiConstants.BUI_CTI_LEFT_PANEL_ICON);
                    icon.setIconColor('green');

                    this.buiCtiLeftPanelMenu.addIcon(icon);
                    this.buiCtiLeftPanelMenu.render();
                }
            }, 4000);
        }
    };

    /**
     * Handler for event - 'connected'
     *
     * This is the handler for 'connected' event. This handler will be invoked
     * by the CTIAdapter after the Agent accepts an incoming call.
     *
     * @param ctiData
     */
    public handleCallConnected = (ctiData:any):void => {
        CtiLogger.logInfoMessage(this.logPreMessage +
            CtiMessages.MESSAGE_HANDLE_CALL_CONNECTED);
        this.isCallSummarized = false;
        //var isOutbound: boolean = this.isOutGoing;
        if (this.isOutGoing) {
            if (this.dialedContact) {
                ctiData.contact = this.dialedContact;
                this.dialedContact = null;
            }
            this.isOutGoing = false;
        }else{
            this.logCallAction(CtiMessages.MESSAGE_CALL_ACCEPTED_BY_AGENT + this.agentProfile.accountId);
        }
        this.onCall = true;
        this.openInteractionWorkspace(ctiData);
        this.ctiInCallClock = new CtiClock('call_in_clock');
        this.callerData = ctiData.contact;
        if(!ctiData.contact.email){
            ctiData.contact.email = CtiMessages.MESSAGE_MAIL_NOT_AVAILABLE;
        }
        CtiViewHelper.renderOnCallView(ctiData, this.ringMedia, this.searchAvailableAgents);
        this.ctiInCallClock.startClock();
    };

    /**
     *
     * @param connectedData
     *This method opens
     *
     * 1. An interaction workspace for the accepted call
     * 2. Set the contact and incident information to the interaction workspace
     * 3. Open the associated incident with the interaction, if any
     *
     */
    public openInteractionWorkspace(connectedData:any):void {
        CtiLogger.logInfoMessage(this.logPreMessage + CtiMessages.MESSAGE_OPEN_INTERACTION_WORKSPACE);
        this.incidentWorkspace = null;
        this.extensionSdk.registerWorkspaceExtension(
            (workspaceRecord) => {
                this.workspaceRecord = workspaceRecord;
                workspaceRecord.createWorkspaceRecord('Interaction',
                    (interactionObject) => {
                        this.interactionWorkspace = interactionObject;
                        if (connectedData.contact.id) {
                            interactionObject.updateField('Interaction.CId', connectedData.contact.id);
                        }
                        if (connectedData.contact.firstName) {
                            interactionObject.updateField('Interaction.Name.First', connectedData.contact.firstName);
                        }
                        if (connectedData.contact.lastName) {
                            interactionObject.updateField('Interaction.Name.Last', connectedData.contact.lastName);
                        }
                        if (connectedData.contact.email) {
                            interactionObject.updateField('Interaction.Email.Addr', connectedData.contact.email);
                        }

                        interactionObject.addFieldValueListener('Interaction.IId', this.incidentUpdateHandler);
                        if (connectedData && connectedData.incident) {
                            interactionObject.updateField('Interaction.IId', connectedData.incident);
                            workspaceRecord.editWorkspaceRecord('Incident', connectedData.incident, (incidentWorkspace) => {
                                this.incidentWorkspace = <IIncidentWorkspaceRecord>incidentWorkspace;
                                setTimeout(() => {
                                    CtiViewHelper.enableOnCallControls();
                                }, 1000);

                            });
                        } else {
                            setTimeout(() => {
                                CtiViewHelper.enableOnCallControls();
                            }, 1000);
                        }

                    });
            });
    }

    /**
     * This function updates the IncidentWorkspaceRecord object when agent changes the
     * incident associated with the InteractionWorkspace
     *
     * @param fieldValueChangeData
     */
    private incidentUpdateHandler = (fieldValueChangeData:ORACLE_SERVICE_CLOUD.IWorkspaceRecordEventParameter) => {
        if (fieldValueChangeData && fieldValueChangeData.event && fieldValueChangeData.event.value) {
            if (parseInt(fieldValueChangeData.event.value, 10) < 0) {
                setTimeout(() => {
                    this.extensionSdk.registerWorkspaceExtension((workspaceRecord:ORACLE_SERVICE_CLOUD.IWorkspaceRecord) => {
                        this.incidentWorkspace = <ORACLE_SERVICE_CLOUD.IIncidentWorkspaceRecord>workspaceRecord;
                    }, 'Incident', parseInt(fieldValueChangeData.event.value, 10));
                }, 1000);
            } else {
                this.workspaceRecord.editWorkspaceRecord('Incident', parseInt(
                    fieldValueChangeData.event.value, 10), (incidentWorkspaceRecord:IIncidentWorkspaceRecord) => {
                    this.incidentWorkspace = incidentWorkspaceRecord;
                })
            }
        } else {
            //Value cleared
            this.incidentWorkspace = null;
        }
    };

    /**
     * Handler for event - 'disconnected'
     *
     * This is the handler for 'disconnected' event. This handler will be
     * invoked by the CTI Adapter when agent hang-up the call
     */
    public handleCallDisconnected = (data: any):void => {
        CtiLogger.logInfoMessage(this.logPreMessage +
            CtiMessages.MESSAGE_HANDLE_CALL_DISCONNECT);
        this.onCallTransfer =  false;
        this.onCall = false;
        this.isNotifyAllowed = true;
        this.ctiInCallClock.stopClock();
        this.ctiInCallClock.resetUI();
        CtiViewHelper.renderCallDisconnectView();
        this.summarizeCall(false, null);
    };

    /**
     * Handler for event - 'cancelled'
     *
     * This is the handler for 'canceled' event. This handler will be invoked by
     * the CTIAdapter when the call is rejected by the agent
     *
     * @param data
     */
    public handleCallCancelled = (data: any):void => {
        CtiLogger.logInfoMessage(this.logPreMessage +
            CtiMessages.MESSAGE_HANDLE_CALL_CANCEL);
        this.isNotifyAllowed = true;
        this.onCall = false;
        this.ringMedia.pause();
        this.ringMedia.currentTime = 0;
        this.logCallAction(CtiMessages.MESSAGE_CALL_REJECTED_BY_AGENT + this.agentProfile.accountId);
        CtiViewHelper.renderCallCancelledView();
    };

    /**
     * Handler for event - 'timeout'
     *
     * This is the handler for 'timeout' event.
     * This handler will be invoked by the CTIAdapter
     * when an incoming call times out
     *
     * @param data
     */
    public handleCallTimeOut = (data:any):void => {
        CtiLogger.logInfoMessage(this.logPreMessage +
            CtiMessages.MESSAGE_HANDLE_CALL_TIMEOUT);
        this.isNotifyAllowed = true;
        this.ringMedia.pause();
        this.ringMedia.currentTime = 0;
        CtiViewHelper.renderCallTimeOutView();
    };


    /**
     * initiates a search for available agents.
     */
    public searchAvailableAgents = () => {
        CtiLogger.logInfoMessage(this.logPreMessage +
            CtiMessages.MESSAGE_INITIATE_AGENT_SEARCH);
        CtiViewHelper.renderAgentSearchUI();
        this.globalContext.getSessionToken().then((sessionToken:string) => {
            this.ctiAdapter.searchAvailableAgents(sessionToken);
        });
    };

    /**
     * Handler for event - 'search.agentlist.complete'
     *
     * @param availableAgents
     */
    public handleAgentSearchSuccess = (availableAgents:AgentData[]) => {
        CtiLogger.logInfoMessage(this.logPreMessage +
            CtiMessages.MESSAGE_HANDLE_AGENT_SEARCH_COMPLETION);
        CtiViewHelper.renderAgentList(availableAgents, this.transferCallHandler);
    };

    /**
     * Initiates a transfer call request. This will be invoked when
     * an agent clicks on the transfer call button.
     *
     * @param workerId
     * @param agentName
     */
    public transferCallHandler = (workerId: string, agentName: string) => {
        if(this.onCallTransfer) {
            CtiLogger.logInfoMessage(this.logPreMessage +
                CtiMessages.MESSAGE_WAIT_WHILE_TRANSFER);
            return;
        }

        CtiLogger.logInfoMessage(this.logPreMessage +
            CtiMessages.MESSAGE_INITIATE_TRANSFER);
        this.onCallTransfer = true;
        CtiViewHelper.disableOnCallControls();

        if(this.incidentWorkspace){
            //Transfer the call only after save
            this.incidentWorkspace.addRecordClosingListener( (eventData: ORACLE_SERVICE_CLOUD.IWorkspaceRecordEventParameter) => {
                //CtiViewHelper.disableOnCallControls();
                var incidentId: number = eventData.getWorkspaceRecord().getWorkspaceRecordId();
                this.globalContext.getSessionToken().then((sessionToken:string) => {
                    this.ctiInCallClock.stopClock();
                    this.ctiAdapter.transferCall(sessionToken, workerId, incidentId);
                    this.incidentWorkspace = null;
                });
            });
        }else{
            this.globalContext.getSessionToken().then((sessionToken:string) => {
                this.ctiAdapter.transferCall(sessionToken, workerId);
            });
        }

        this.summarizeCall(true, agentName).catch(()=>{
            //Re-Enable the controls, if saving workspace fails
            CtiViewHelper.enableOnCallControls();
            this.onCallTransfer = false;
            this.isCallSummarized = false;
        });
        this.isCallSummarized = true;
    };

    /**
     * Requests for token updation
     *
     * @param data
     */
    public tokenExpiryHandler = (data: any) => {
        CtiLogger.logInfoMessage(this.logPreMessage +
            CtiMessages.MESSAGE_HANDLE_TOKEN_EXPIRY);
        this.globalContext.getSessionToken().then( (sessionToken: string) => {
            this.ctiAdapter.renewCtiToken(sessionToken);
        })
    };

    public logCallAction(actionMessage: string): void {
        var message: string = actionMessage +
            CtiMessages.MESSAGE_BY_AGENT+ this.agentProfile.accountId;
        this.globalContext.getSessionToken().then((sessionToken: string) => {
            this.ctiAdapter.logMessage(sessionToken, actionMessage);
        });
    }


    /**
     * updateRibbonbarMenuAfterLogin - Adds and render
     * status update and logout options to the ribbonbar menu
     *
     */
    public updateRibbonbarMenuAfterLogin():void {
        //Change the icon
        this.updateGlobalHeaderMenuIcon(this.ctiProviderConfig.defaultStatus);

        //Add options
        var menuItemAvailable = this.globalHeaderMenu.createMenuItem();
        menuItemAvailable.setLabel(CtiConstants.BUI_CTI_LABEL_AVAILABLE);
        menuItemAvailable.setHandler(
            () => {
                this.updateAgentStatus(CtiConstants.AVAILABLE);
            });
        this.globalHeaderMenu.addMenuItem(menuItemAvailable);

        var menuItemNotAvailable = this.globalHeaderMenu.createMenuItem();
        menuItemNotAvailable.setLabel(CtiConstants.BUI_CTI_LABEL_NOT_AVAILABLE);
        menuItemNotAvailable.setHandler(
            () => {
                this.updateAgentStatus(CtiConstants.NOT_AVAILABLE);
            });
        this.globalHeaderMenu.addMenuItem(menuItemNotAvailable);

        var menuItemBusy = this.globalHeaderMenu.createMenuItem();
        menuItemBusy.setLabel(CtiConstants.BUI_CTI_LABEL_BUSY);
        menuItemBusy.setHandler(
            () => {
                this.updateAgentStatus(CtiConstants.BUSY);
            });
        this.globalHeaderMenu.addMenuItem(menuItemBusy);

        var menuItemLogout = this.globalHeaderMenu.createMenuItem();
        menuItemLogout.setLabel(CtiConstants.BUI_CTI_LABEL_LOGOUT);
        menuItemLogout.setHandler(
            () => {
                this.handleLogout();
            });
        this.globalHeaderMenu.addMenuItem(menuItemLogout);

        this.globalHeaderMenu.render();
    }

    /**
     * updateLeftPanelMenuAfterLogin - Updates the leftpanel menu
     * after successful login.
     */
    public updateLeftPanelMenuAfterLogin():void {
        this.buiCtiLeftPanelMenu.setLabel(this.ctiProviderConfig.providerName + ' ' + CtiConstants.BUI_CTI_LABEL_AVAILABLE);
        this.buiCtiLeftPanelMenu.setVisible(true);
        this.buiCtiLeftPanelMenu.setDisabled(false);
        var icon = this.buiCtiLeftPanelMenu.createIcon(CtiConstants.BUI_CTI_RIBBONBAR_ICON_TYPE);
        icon.setIconClass(CtiConstants.BUI_CTI_LEFT_PANEL_ICON);
        //Get icon color based on default status
        var iconData = this.getIconDetailsForStatus(this.ctiProviderConfig.defaultStatus);
        icon.setIconColor(iconData.color);
        this.buiCtiLeftPanelMenu.addIcon(icon);
        this.buiCtiLeftPanelMenu.render();
    }

    /**
     *
     * @param status
     *
     * updateAgentStatus - handler for status update
     * requests
     *
     */
    public updateAgentStatus = (status:string):void => {
        //Update icon
        if (!this.onCall && this.prevStatus !== status) {
            CtiLogger.logInfoMessage(this.logPreMessage +
                CtiMessages.MESSAGE_INITIATE_ACTIVITY_UPDATE +
                CtiMessages.MESSAGE_APPENDER + status);
            this.ctiAdapter.updateActivity(status);
        }
    };

    /**
     * Updates the UI based on status updates from server
     * 
     * @param status
     */
    public updateAgentUIOnServerUpdate = (status:string):void => {
        this.updateGlobalHeaderMenuIcon(status);
        this.updateLeftPanelMenuIcon(status);
    };

    /**
     *
     * @param status
     *
     * updateGlobalHeaderMenuIcon - update the ribbonbar menu icon
     * for the given status.
     *
     */
    public updateGlobalHeaderMenuIcon = (status:string):void => {
        var iconData:IconData = this.getIconDetailsForStatus(status);
        var icon = this.globalHeaderMenu.createIcon(CtiConstants.BUI_CTI_RIBBONBAR_ICON_TYPE);
        icon.setIconClass(iconData.class);
        icon.setIconColor(iconData.color);
        this.globalHeaderMenu.addIcon(icon);
        this.globalHeaderMenu.setLabel(this.ctiProviderConfig.providerName + ' ' + iconData.label);
        this.globalHeaderMenu.render();
    };

    /**
     *
     * @param status
     *
     * updateLeftPanelMenuIcon - Update the leftpanel menu icon
     * for the given status
     */
    public updateLeftPanelMenuIcon = (status:string):void => {
        var iconData = this.getIconDetailsForStatus(status);
        var icon = this.buiCtiLeftPanelMenu.createIcon(CtiConstants.BUI_CTI_LEFT_PANEL_ICON_TYPE);
        icon.setIconClass(CtiConstants.BUI_CTI_LEFT_PANEL_ICON);
        icon.setIconColor(iconData.color);
        this.buiCtiLeftPanelMenu.addIcon(icon);
        this.buiCtiLeftPanelMenu.setLabel(this.ctiProviderConfig.providerName + ' ' + iconData.label);
        this.buiCtiLeftPanelMenu.render();
    };

    /**
     *
     * @param status
     *
     * getIconDetailsForStatus - Returns the Icon details for
     * a given status code.
     */
    public getIconDetailsForStatus(status:string):IconData {
        var iconData:IconData = <IconData>{
            class: '',
            color: '',
            label: ''
        };

        switch (status) {
            case CtiConstants.AVAILABLE:
                iconData.class = CtiConstants.BUI_CTI_ICON_CLASS_AVAILABLE;
                iconData.color = 'green';
                iconData.label = CtiConstants.BUI_CTI_LABEL_AVAILABLE;
                break;
            case CtiConstants.NOT_AVAILABLE:
                iconData.class = CtiConstants.BUI_CTI_ICON_CLASS_NOT_AVAILABLE;
                iconData.color = 'black';
                iconData.label = CtiConstants.BUI_CTI_LABEL_NOT_AVAILABLE;
                break;
            case CtiConstants.BUSY:
                iconData.class = CtiConstants.BUI_CTI_ICON_CLASS_BUSY;
                iconData.color = 'red';
                iconData.label = CtiConstants.BUI_CTI_LABEL_BUSY;
                break;
            case CtiConstants.WAIT:
                iconData.class = CtiConstants.BUI_CTI_RIBBONBAR_ICON_WAIT;
                iconData.color = 'black';
                iconData.label = CtiConstants.BUI_CTI_LABEL_WAIT;
        }
        return iconData;
    }

    /**
     * handleLogout - submit logout request
     * to the CTI tool. Also updates the UI
     */
    public handleLogout():void {
        if (this.isLoggedIn && !this.onCall) {
            this.ctiAdapter.logout();

            //dispose the existing menu options
            this.globalHeaderMenu.dispose();

            this.buiCtiLeftPanelMenu.setVisible(false);
            this.buiCtiLeftPanelMenu.render();

            this.summarizeCall(false, null);

            this.globalHeaderMenu = null;
            this.onCall = false;
            this.isNotifyAllowed = true;
            this.isLoggedIn = false;
            this.prevStatus = null;
            //Reset to initial state
            this.initializeCtiToolBarMenu();
        }
    }

    /**
     * summarizeCall - All handling for disconnecting
     * a call goes here
     *
     */
    public summarizeCall (isTransfer: boolean, agentName: string):IExtensionPromise<any> {
        var resolveRef: any;
        var rejectRef: any;

        var promiseRef: IExtensionPromise<any> = new ExtensionPromise((resolve: any, reject: any) => {
            resolveRef = resolve;
            rejectRef = reject;
        });

        if(this.isCallSummarized) {
            CtiLogger.logInfoMessage(this.logPreMessage +
                CtiMessages.MESSAGE_CALL_SUMMARIZED);
            return;
        }
        CtiLogger.logInfoMessage(this.logPreMessage + CtiMessages.MESSAGE_SUMMARIZE_CALL);
        this.onCall = false;
        this.isNotifyAllowed = true;

        if (this.workspaceRecord) {
            this.workspaceRecord.triggerNamedEvent('CallEnded');
        }

        //Add note to the thread
        if (this.incidentWorkspace && this.incidentWorkspace.getWorkspaceRecordId()) {
            this.incidentWorkspace.getCurrentEditedThread('NOTE', true)
                .then((threadEntry:any) => {
                    var content = CtiMessages.MESSAGE_CALL_DURATION + this.ctiInCallClock.getCallLength() + ' \n '
                        + CtiMessages.MESSAGE_CALL_START + this.ctiInCallClock.getClockStartTime() + ' \n '
                        + CtiMessages.MESSAGE_CALL_END + this.ctiInCallClock.getClockEndTime() + ' \n ';
                    if(isTransfer && agentName){
                        content =content + CtiMessages.MESSAGE_CALL_TRANSFERRED_TO + agentName;
                    }
                    threadEntry.setContent(content)
                        .then(() => {
                                var objectType:string = this.incidentWorkspace.getWorkspaceRecordType();
                                var objectId:number = +this.incidentWorkspace.getWorkspaceRecordId();
                                if (this.workspaceRecord.isEditorOpen(objectType, objectId)) {
                                    this.incidentWorkspace.executeEditorCommand('saveAndClose').then(()=>{
                                        resolveRef();
                                    }).catch(()=>{
                                        rejectRef();
                                    });
                                }
                                if (this.interactionWorkspace.getWorkspaceRecordId()) {
                                    this.interactionWorkspace.executeEditorCommand('saveAndClose').then(()=>{
                                        resolveRef();
                                    }).catch(()=>{
                                        rejectRef();
                                    });
                                }
                            }
                        );
                });
        } else {
            setTimeout(() => {
                if (this.interactionWorkspace && this.interactionWorkspace.getWorkspaceRecordId()) {
                    this.interactionWorkspace.closeEditor().then(()=>{
                        resolveRef();
                    }).catch(()=>{
                        rejectRef();
                    });
                }
            }, 100);
        }
        return promiseRef;
    }

    /***
     * Handles window unload, when user logs out from BUI
     */
    private registerUnloadHandler(): void {
        window.addEventListener('beforeunload', () => {
            this.handleLogout();
        });
    }
}

new CtiTelephonyAddin(new TwilioAdapter()).enableCtiAddin();