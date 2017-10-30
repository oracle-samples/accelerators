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
 *  SHA1: $Id: 84a31f3a91e5278351baec144ed58014898aeac5 $
 * *********************************************************************************************
 *  File: ctiTelephonyAddin.js
 * ****************************************************************************************** */
define(["require", "exports", "./../util/ctiConstants", "./../util/ctiViewHelper", "../adapter/twilioAdapter", "../util/ctiLogger", "../util/ctiClock", "../util/ctiMessages"], function (require, exports, ctiConstants_1, ctiViewHelper_1, twilioAdapter_1, ctiLogger_1, ctiClock_1, ctiMessages_1) {
    "use strict";
    exports.__esModule = true;
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
    var CtiTelephonyAddin = /** @class */ (function () {
        function CtiTelephonyAddin(ctiClientAdapter) {
            var _this = this;
            this.isLoggedIn = false;
            this.onCall = false;
            this.isNotifyAllowed = true;
            this.isCallSummarized = false;
            this.onCallTransfer = false;
            this.logPreMessage = 'CtiTelephonyAddin' + ctiMessages_1.CtiMessages.MESSAGE_APPENDER;
            /**
             * This method initialize the Toolbar Menu Icon
             * for CTI Addin. This is the icon
             * from where Agent can login to the CTI Tool
             *
             */
            this.initializeCtiToolBarMenu = function () {
                ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage +
                    ctiMessages_1.CtiMessages.MESSAGE_INITIALIZE_ADDIN);
                _this.initializeRingMediaElement();
                _this.enableDialPadControls();
                _this.registerUnloadHandler();
                //Load Menubar addin
                _this.initializeLeftpaneIcon();
                _this.extensionSdk.registerUserInterfaceExtension(function (userInterfaceContext) {
                    userInterfaceContext.getGlobalHeaderContext()
                        .then(function (ribbonBarContext) {
                        _this.globalHeaderContext = ribbonBarContext;
                        _this.ctiProviderConfig = _this.ctiAdapter.getConfiguration();
                        ribbonBarContext.getMenu(ctiConstants_1.CtiConstants.BUI_CTI_RIBBONBAR_MENU_ID)
                            .then(function (ribbonBarMenu) {
                            _this.globalHeaderMenu = ribbonBarMenu;
                            var icon = ribbonBarMenu.createIcon(ctiConstants_1.CtiConstants.BUI_CTI_RIBBONBAR_ICON_TYPE);
                            icon.setIconClass(ctiConstants_1.CtiConstants.BUI_CTI_RIBBONBAR_ICON_DEFAULT_CLASS);
                            icon.setIconColor(ctiConstants_1.CtiConstants.BUI_CTI_RIBBONBAR_ICON_DEFAULT_COLOR);
                            ribbonBarMenu.addIcon(icon);
                            ribbonBarMenu.setDisabled(false);
                            ribbonBarMenu.setLabel(_this.ctiProviderConfig.providerName + ' ' + ctiConstants_1.CtiConstants.BUI_CTI_LABEL_LOGGED_OUT);
                            _this.menuItemLogin = ribbonBarMenu.createMenuItem();
                            _this.menuItemLogin.setLabel(ctiConstants_1.CtiConstants.BUI_CTI_LABEL_LOGIN);
                            _this.menuItemLogin.setHandler(function (menuItem) {
                                _this.handleLogin();
                            });
                            ribbonBarMenu.addMenuItem(_this.menuItemLogin);
                            ribbonBarMenu.render();
                        });
                        _this.isLoggedIn = false;
                    });
                });
            };
            /**
             * This method initiates an outbound call. It triggers a search
             * for the given contact and renders the on-call UI. The actual
             * call will not be triggered from here. It will be triggered
             * after a the search completion
             *
             * @param event
             */
            this.outgoingCallHandler = function (event) {
                var dialedNumber = ctiViewHelper_1.CtiViewHelper.getOutgoingContactNumber();
                _this.logCallAction(ctiMessages_1.CtiMessages.MESSAGE_HANDLE_OUTGOING_CALL +
                    ctiMessages_1.CtiMessages.MESSAGE_APPENDER + dialedNumber);
                if (dialedNumber !== '') {
                    _this.updateAgentStatus(ctiConstants_1.CtiConstants.BUSY);
                    _this.onCall = true;
                    _this.isNotifyAllowed = false;
                    _this.globalContext.getSessionToken().then(function (sessionToken) {
                        _this.ctiAdapter.searchContact(ctiViewHelper_1.CtiViewHelper.getOutgoingContactNumber(), sessionToken);
                    });
                    ctiViewHelper_1.CtiViewHelper.showOnCallUI(dialedNumber, ctiConstants_1.CtiConstants.DEFAULT_DISPLAY_ICON);
                }
            };
            /**
             * Handler for events - 'search.contact.complete' and 'search.contact.failed'
             * In both cases we update the on-call UI with search results (Either contact name OR Unknown).
             * Also the actual call is tirggered from here.
             *
             * @param searchResult
             */
            this.contactSearchHandler = function (searchResult) {
                _this.dialedContact = searchResult.contact;
                _this.isOutGoing = true;
                ctiViewHelper_1.CtiViewHelper.updateOutgoingContactDetails(searchResult.contact.name, searchResult.contact.dp);
                _this.ctiAdapter.dialANumber(ctiViewHelper_1.CtiViewHelper.getOutgoingContactNumber().trim());
            };
            /**
             *
             *
             * @param event
             */
            this.outgoingHangupHandler = function (event) {
                ctiViewHelper_1.CtiViewHelper.renderOutgoingHangupUI(event);
                _this.onCall = false;
                _this.isNotifyAllowed = true;
            };
            /**
             * handleLogin - Initiate a login request
             * with the CTI tool, using the CTIAdapter instance
             */
            this.handleLogin = function () {
                ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage + ctiMessages_1.CtiMessages.MESSAGE_INITIATE_LOGIN);
                _this.registerListenersToAdapter();
                if (!_this.isLoggedIn && _this.globalHeaderMenu) {
                    _this.updateGlobalHeaderMenuIcon(ctiConstants_1.CtiConstants.WAIT);
                    _this.globalContext.getSessionToken().then(function (sessionToken) {
                        var agentProfile = {
                            interfaceUrl: _this.globalContext.getInterfaceUrl(),
                            accountId: '' + _this.globalContext.getAccountId(),
                            sessionId: sessionToken
                        };
                        _this.agentProfile = agentProfile;
                        _this.ctiAdapter.login(agentProfile);
                    });
                }
                else {
                    ctiLogger_1.CtiLogger.logWarningMessage(_this.logPreMessage +
                        ctiMessages_1.CtiMessages.MESSAGE_PARTIAL_LOGOUT);
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
            this.handleLoginSuccess = function (data) {
                if (_this.isLoggedIn) {
                    ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage +
                        ctiMessages_1.CtiMessages.MESSAGE_ALREADY_LOGGED_IN);
                    _this.ctiAdapter.updateActivity(_this.ctiProviderConfig.defaultStatus);
                    return;
                }
                ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage +
                    ctiMessages_1.CtiMessages.MESSAGE_UI_UPDATE_AFTER_LOGIN_SUCCESS);
                _this.globalHeaderContext.getMenu(ctiConstants_1.CtiConstants.BUI_CTI_RIBBONBAR_MENU_ID)
                    .then(function (ribbonBarMenu) {
                    _this.globalHeaderMenu = ribbonBarMenu;
                    _this.menuItemLogin.dispose();
                    //1. Update Ribbonbar menu
                    _this.updateRibbonbarMenuAfterLogin();
                    //2. Update leftpanel menu
                    _this.updateLeftPanelMenuAfterLogin();
                    _this.isLoggedIn = true;
                    _this.prevStatus = _this.ctiProviderConfig.defaultStatus;
                });
            };
            /**
             * This method reset the login icon, if login fails
             */
            this.handleLoginFailure = function (data) {
                if (_this.globalHeaderMenu) {
                    var icon = _this.globalHeaderMenu.createIcon(ctiConstants_1.CtiConstants.BUI_CTI_RIBBONBAR_ICON_TYPE);
                    icon.setIconClass(ctiConstants_1.CtiConstants.BUI_CTI_RIBBONBAR_ICON_DEFAULT_CLASS);
                    icon.setIconColor(ctiConstants_1.CtiConstants.BUI_CTI_RIBBONBAR_ICON_DEFAULT_COLOR);
                    _this.globalHeaderMenu.addIcon(icon);
                    _this.globalHeaderMenu.setDisabled(false);
                    _this.globalHeaderMenu.setLabel(_this.ctiProviderConfig.providerName + ' ' + ctiConstants_1.CtiConstants.BUI_CTI_LABEL_LOGGED_OUT);
                    _this.globalHeaderMenu.render();
                }
                else {
                    _this.initializeCtiToolBarMenu();
                }
            };
            /**
             * Updates the UI according to the status updates from CTI Platform
             *
             * @param ctiUpdateData
             */
            this.handleStatusUpdatesFromServer = function (ctiUpdateData) {
                ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage +
                    ctiMessages_1.CtiMessages.MESSAGE_CLIENT_STATUS +
                    ctiMessages_1.CtiMessages.MESSAGE_APPENDER + _this.prevStatus);
                ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage +
                    ctiMessages_1.CtiMessages.MESSAGE_SERVER_STATUS +
                    ctiMessages_1.CtiMessages.MESSAGE_APPENDER + ctiUpdateData);
                if (_this.prevStatus !== ctiUpdateData) {
                    _this.updateAgentUIOnServerUpdate(ctiUpdateData);
                    _this.prevStatus = status;
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
            this.handleIncoming = function (ctiData) {
                ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage +
                    ctiMessages_1.CtiMessages.MESSAGE_HANDLE_CALL_INCOMING);
                if (!_this.onCall && _this.isNotifyAllowed) {
                    _this.isNotifyAllowed = false;
                    if (!ctiData.contact.email) {
                        ctiData.contact.email = ctiMessages_1.CtiMessages.MESSAGE_MAIL_NOT_AVAILABLE;
                    }
                    ctiViewHelper_1.CtiViewHelper.renderIncomingView(ctiData, _this.ringMedia);
                    _this.buiCtiLeftPanelMenu.setLabel(ctiConstants_1.CtiConstants.BUI_CTI_LABEL_INCOMING_CALL);
                    var icon = _this.buiCtiLeftPanelMenu.createIcon(ctiConstants_1.CtiConstants.BUI_CTI_LEFT_PANEL_ICON_TYPE);
                    icon.setIconClass(ctiConstants_1.CtiConstants.BUI_CTI_LEFT_PANEL_ICON_NOTIFY);
                    icon.setIconColor('red');
                    _this.buiCtiLeftPanelMenu.addIcon(icon);
                    _this.buiCtiLeftPanelMenu.expand();
                    _this.buiCtiLeftPanelMenu.render();
                    setTimeout(function () {
                        if (!_this.onCall) {
                            _this.buiCtiLeftPanelMenu.setLabel(_this.ctiProviderConfig.providerName + ' ' + ctiConstants_1.CtiConstants.BUI_CTI_LABEL_AVAILABLE);
                            var icon = _this.buiCtiLeftPanelMenu.createIcon(ctiConstants_1.CtiConstants.BUI_CTI_LEFT_PANEL_ICON_TYPE);
                            icon.setIconClass(ctiConstants_1.CtiConstants.BUI_CTI_LEFT_PANEL_ICON);
                            icon.setIconColor('green');
                            _this.buiCtiLeftPanelMenu.addIcon(icon);
                            _this.buiCtiLeftPanelMenu.render();
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
            this.handleCallConnected = function (ctiData) {
                ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage +
                    ctiMessages_1.CtiMessages.MESSAGE_HANDLE_CALL_CONNECTED);
                _this.isCallSummarized = false;
                //var isOutbound: boolean = this.isOutGoing;
                if (_this.isOutGoing) {
                    if (_this.dialedContact) {
                        ctiData.contact = _this.dialedContact;
                        _this.dialedContact = null;
                    }
                    _this.isOutGoing = false;
                }
                else {
                    _this.logCallAction(ctiMessages_1.CtiMessages.MESSAGE_CALL_ACCEPTED_BY_AGENT + _this.agentProfile.accountId);
                }
                _this.onCall = true;
                _this.openInteractionWorkspace(ctiData);
                _this.ctiInCallClock = new ctiClock_1.CtiClock('call_in_clock');
                _this.callerData = ctiData.contact;
                if (!ctiData.contact.email) {
                    ctiData.contact.email = ctiMessages_1.CtiMessages.MESSAGE_MAIL_NOT_AVAILABLE;
                }
                ctiViewHelper_1.CtiViewHelper.renderOnCallView(ctiData, _this.ringMedia, _this.searchAvailableAgents);
                _this.ctiInCallClock.startClock();
            };
            /**
             * This function updates the IncidentWorkspaceRecord object when agent changes the
             * incident associated with the InteractionWorkspace
             *
             * @param fieldValueChangeData
             */
            this.incidentUpdateHandler = function (fieldValueChangeData) {
                if (fieldValueChangeData && fieldValueChangeData.event && fieldValueChangeData.event.value) {
                    if (parseInt(fieldValueChangeData.event.value, 10) < 0) {
                        setTimeout(function () {
                            _this.extensionSdk.registerWorkspaceExtension(function (workspaceRecord) {
                                _this.incidentWorkspace = workspaceRecord;
                            }, 'Incident', parseInt(fieldValueChangeData.event.value, 10));
                        }, 1000);
                    }
                    else {
                        _this.workspaceRecord.editWorkspaceRecord('Incident', parseInt(fieldValueChangeData.event.value, 10), function (incidentWorkspaceRecord) {
                            _this.incidentWorkspace = incidentWorkspaceRecord;
                        });
                    }
                }
                else {
                    //Value cleared
                    _this.incidentWorkspace = null;
                }
            };
            /**
             * Handler for event - 'disconnected'
             *
             * This is the handler for 'disconnected' event. This handler will be
             * invoked by the CTI Adapter when agent hang-up the call
             */
            this.handleCallDisconnected = function (data) {
                ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage +
                    ctiMessages_1.CtiMessages.MESSAGE_HANDLE_CALL_DISCONNECT);
                _this.onCallTransfer = false;
                _this.onCall = false;
                _this.isNotifyAllowed = true;
                _this.ctiInCallClock.stopClock();
                _this.ctiInCallClock.resetUI();
                ctiViewHelper_1.CtiViewHelper.renderCallDisconnectView();
                _this.summarizeCall(false, null);
            };
            /**
             * Handler for event - 'cancelled'
             *
             * This is the handler for 'canceled' event. This handler will be invoked by
             * the CTIAdapter when the call is rejected by the agent
             *
             * @param data
             */
            this.handleCallCancelled = function (data) {
                ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage +
                    ctiMessages_1.CtiMessages.MESSAGE_HANDLE_CALL_CANCEL);
                _this.isNotifyAllowed = true;
                _this.onCall = false;
                _this.ringMedia.pause();
                _this.ringMedia.currentTime = 0;
                _this.logCallAction(ctiMessages_1.CtiMessages.MESSAGE_CALL_REJECTED_BY_AGENT + _this.agentProfile.accountId);
                ctiViewHelper_1.CtiViewHelper.renderCallCancelledView();
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
            this.handleCallTimeOut = function (data) {
                ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage +
                    ctiMessages_1.CtiMessages.MESSAGE_HANDLE_CALL_TIMEOUT);
                _this.isNotifyAllowed = true;
                _this.ringMedia.pause();
                _this.ringMedia.currentTime = 0;
                ctiViewHelper_1.CtiViewHelper.renderCallTimeOutView();
            };
            /**
             * initiates a search for available agents.
             */
            this.searchAvailableAgents = function () {
                ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage +
                    ctiMessages_1.CtiMessages.MESSAGE_INITIATE_AGENT_SEARCH);
                ctiViewHelper_1.CtiViewHelper.renderAgentSearchUI();
                _this.globalContext.getSessionToken().then(function (sessionToken) {
                    _this.ctiAdapter.searchAvailableAgents(sessionToken);
                });
            };
            /**
             * Handler for event - 'search.agentlist.complete'
             *
             * @param availableAgents
             */
            this.handleAgentSearchSuccess = function (availableAgents) {
                ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage +
                    ctiMessages_1.CtiMessages.MESSAGE_HANDLE_AGENT_SEARCH_COMPLETION);
                ctiViewHelper_1.CtiViewHelper.renderAgentList(availableAgents, _this.transferCallHandler);
            };
            /**
             * Initiates a transfer call request. This will be invoked when
             * an agent clicks on the transfer call button.
             *
             * @param workerId
             * @param agentName
             */
            this.transferCallHandler = function (workerId, agentName) {
                if (_this.onCallTransfer) {
                    ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage +
                        ctiMessages_1.CtiMessages.MESSAGE_WAIT_WHILE_TRANSFER);
                    return;
                }
                ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage +
                    ctiMessages_1.CtiMessages.MESSAGE_INITIATE_TRANSFER);
                _this.onCallTransfer = true;
                ctiViewHelper_1.CtiViewHelper.disableOnCallControls();
                if (_this.incidentWorkspace) {
                    //Transfer the call only after save
                    _this.incidentWorkspace.addRecordClosingListener(function (eventData) {
                        //CtiViewHelper.disableOnCallControls();
                        var incidentId = eventData.getWorkspaceRecord().getWorkspaceRecordId();
                        _this.globalContext.getSessionToken().then(function (sessionToken) {
                            _this.ctiInCallClock.stopClock();
                            _this.ctiAdapter.transferCall(sessionToken, workerId, incidentId);
                            _this.incidentWorkspace = null;
                        });
                    });
                }
                else {
                    _this.globalContext.getSessionToken().then(function (sessionToken) {
                        _this.ctiAdapter.transferCall(sessionToken, workerId);
                    });
                }
                _this.summarizeCall(true, agentName)["catch"](function () {
                    //Re-Enable the controls, if saving workspace fails
                    ctiViewHelper_1.CtiViewHelper.enableOnCallControls();
                    _this.onCallTransfer = false;
                    _this.isCallSummarized = false;
                });
                _this.isCallSummarized = true;
            };
            /**
             * Requests for token updation
             *
             * @param data
             */
            this.tokenExpiryHandler = function (data) {
                ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage +
                    ctiMessages_1.CtiMessages.MESSAGE_HANDLE_TOKEN_EXPIRY);
                _this.globalContext.getSessionToken().then(function (sessionToken) {
                    _this.ctiAdapter.renewCtiToken(sessionToken);
                });
            };
            /**
             *
             * @param status
             *
             * updateAgentStatus - handler for status update
             * requests
             *
             */
            this.updateAgentStatus = function (status) {
                //Update icon
                if (!_this.onCall && _this.prevStatus !== status) {
                    ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage +
                        ctiMessages_1.CtiMessages.MESSAGE_INITIATE_ACTIVITY_UPDATE +
                        ctiMessages_1.CtiMessages.MESSAGE_APPENDER + status);
                    _this.ctiAdapter.updateActivity(status);
                }
            };
            /**
             * Updates the UI based on status updates from server
             *
             * @param status
             */
            this.updateAgentUIOnServerUpdate = function (status) {
                _this.updateGlobalHeaderMenuIcon(status);
                _this.updateLeftPanelMenuIcon(status);
            };
            /**
             *
             * @param status
             *
             * updateGlobalHeaderMenuIcon - update the ribbonbar menu icon
             * for the given status.
             *
             */
            this.updateGlobalHeaderMenuIcon = function (status) {
                var iconData = _this.getIconDetailsForStatus(status);
                var icon = _this.globalHeaderMenu.createIcon(ctiConstants_1.CtiConstants.BUI_CTI_RIBBONBAR_ICON_TYPE);
                icon.setIconClass(iconData["class"]);
                icon.setIconColor(iconData.color);
                _this.globalHeaderMenu.addIcon(icon);
                _this.globalHeaderMenu.setLabel(_this.ctiProviderConfig.providerName + ' ' + iconData.label);
                _this.globalHeaderMenu.render();
            };
            /**
             *
             * @param status
             *
             * updateLeftPanelMenuIcon - Update the leftpanel menu icon
             * for the given status
             */
            this.updateLeftPanelMenuIcon = function (status) {
                var iconData = _this.getIconDetailsForStatus(status);
                var icon = _this.buiCtiLeftPanelMenu.createIcon(ctiConstants_1.CtiConstants.BUI_CTI_LEFT_PANEL_ICON_TYPE);
                icon.setIconClass(ctiConstants_1.CtiConstants.BUI_CTI_LEFT_PANEL_ICON);
                icon.setIconColor(iconData.color);
                _this.buiCtiLeftPanelMenu.addIcon(icon);
                _this.buiCtiLeftPanelMenu.setLabel(_this.ctiProviderConfig.providerName + ' ' + iconData.label);
                _this.buiCtiLeftPanelMenu.render();
            };
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
        CtiTelephonyAddin.prototype.enableCtiAddin = function () {
            var _this = this;
            //Register handler only for enabling addin
            ctiLogger_1.CtiLogger.logInfoMessage(this.logPreMessage + ctiMessages_1.CtiMessages.MESSAGE_LOAD_EXTENSION);
            this.ctiAdapter.addEventHandler('cti.enabled', this.initializeCtiToolBarMenu);
            ORACLE_SERVICE_CLOUD.extension_loader.load(ctiConstants_1.CtiConstants.BUI_CTI_ADDIN_ID, ctiConstants_1.CtiConstants.BUI_CTI_ADDIN_VERSION)
                .then(function (sdk) {
                _this.extensionSdk = sdk;
                ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage +
                    ctiMessages_1.CtiMessages.MESSAGE_OBTAINED_SDK);
                sdk.getGlobalContext().then(function (globalContext) {
                    _this.globalContext = globalContext;
                    globalContext.getSessionToken().then(function (sessionToken) {
                        _this.ctiAdapter.authorizeAgent(globalContext.getInterfaceUrl(), sessionToken);
                    });
                });
            });
        };
        /**
         * Create the audio element which is used to play
         * the 'ring' for incoming calls
         */
        CtiTelephonyAddin.prototype.initializeRingMediaElement = function () {
            this.ringMedia = document.createElement('audio');
            this.ringMedia.setAttribute('src', this.ctiAdapter.getRingMediaUrl());
            this.ringMedia.addEventListener('ended', function () {
                this.play();
            }, false);
        };
        /**
         * Initializes the left side icon. This icon is used to control
         * the dialer/on-call UI. This icon will be kept hidden until user
         * login to CTI
         */
        CtiTelephonyAddin.prototype.initializeLeftpaneIcon = function () {
            var _this = this;
            ctiLogger_1.CtiLogger.logInfoMessage(this.logPreMessage +
                ctiMessages_1.CtiMessages.MESSAGE_INITIALIZE_SIDEPANEL);
            this.extensionSdk.registerUserInterfaceExtension(function (userInterfaceContext) {
                userInterfaceContext.getLeftSidePaneContext().then(function (leftSidePaneContext) {
                    leftSidePaneContext.getSidePane(ctiConstants_1.CtiConstants.BUI_CTI_LEFT_PANEL_MENU_ID)
                        .then(function (leftPanelMenu) {
                        _this.buiCtiLeftPanelMenu = leftPanelMenu;
                        leftPanelMenu.setLabel('');
                        leftPanelMenu.setVisible(false);
                        var icon = leftPanelMenu.createIcon(ctiConstants_1.CtiConstants.BUI_CTI_RIBBONBAR_ICON_TYPE);
                        icon.setIconClass(ctiConstants_1.CtiConstants.BUI_CTI_LEFT_PANEL_ICON);
                        leftPanelMenu.addIcon(icon);
                        leftPanelMenu.render();
                    });
                });
            });
        };
        /**
         * registerListenersToAdapter - Register listerns
         * with the CTI adapter for events to be handled
         *
         * The CTIAdapter will invoke required handlers, when an
         * event occurs.
         *
         */
        CtiTelephonyAddin.prototype.registerListenersToAdapter = function () {
            //Clear any previous handlers
            this.ctiAdapter.clearAllEventHandlers();
            ctiLogger_1.CtiLogger.logInfoMessage(this.logPreMessage + ctiMessages_1.CtiMessages.MESSAGE_REGISTER_EVENT_HANDLERS);
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
        };
        /**
         * This function adds event listeners to the
         * CTI Dialer
         */
        CtiTelephonyAddin.prototype.enableDialPadControls = function () {
            ctiViewHelper_1.CtiViewHelper.addDialPadControls(this.outgoingCallHandler);
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
        CtiTelephonyAddin.prototype.openInteractionWorkspace = function (connectedData) {
            var _this = this;
            ctiLogger_1.CtiLogger.logInfoMessage(this.logPreMessage + ctiMessages_1.CtiMessages.MESSAGE_OPEN_INTERACTION_WORKSPACE);
            this.incidentWorkspace = null;
            this.extensionSdk.registerWorkspaceExtension(function (workspaceRecord) {
                _this.workspaceRecord = workspaceRecord;
                workspaceRecord.createWorkspaceRecord('Interaction', function (interactionObject) {
                    _this.interactionWorkspace = interactionObject;
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
                    interactionObject.addFieldValueListener('Interaction.IId', _this.incidentUpdateHandler);
                    if (connectedData && connectedData.incident) {
                        interactionObject.updateField('Interaction.IId', connectedData.incident);
                        workspaceRecord.editWorkspaceRecord('Incident', connectedData.incident, function (incidentWorkspace) {
                            _this.incidentWorkspace = incidentWorkspace;
                            setTimeout(function () {
                                ctiViewHelper_1.CtiViewHelper.enableOnCallControls();
                            }, 1000);
                        });
                    }
                    else {
                        setTimeout(function () {
                            ctiViewHelper_1.CtiViewHelper.enableOnCallControls();
                        }, 1000);
                    }
                });
            });
        };
        CtiTelephonyAddin.prototype.logCallAction = function (actionMessage) {
            var _this = this;
            var message = actionMessage +
                ctiMessages_1.CtiMessages.MESSAGE_BY_AGENT + this.agentProfile.accountId;
            this.globalContext.getSessionToken().then(function (sessionToken) {
                _this.ctiAdapter.logMessage(sessionToken, actionMessage);
            });
        };
        /**
         * updateRibbonbarMenuAfterLogin - Adds and render
         * status update and logout options to the ribbonbar menu
         *
         */
        CtiTelephonyAddin.prototype.updateRibbonbarMenuAfterLogin = function () {
            var _this = this;
            //Change the icon
            this.updateGlobalHeaderMenuIcon(this.ctiProviderConfig.defaultStatus);
            //Add options
            var menuItemAvailable = this.globalHeaderMenu.createMenuItem();
            menuItemAvailable.setLabel(ctiConstants_1.CtiConstants.BUI_CTI_LABEL_AVAILABLE);
            menuItemAvailable.setHandler(function () {
                _this.updateAgentStatus(ctiConstants_1.CtiConstants.AVAILABLE);
            });
            this.globalHeaderMenu.addMenuItem(menuItemAvailable);
            var menuItemNotAvailable = this.globalHeaderMenu.createMenuItem();
            menuItemNotAvailable.setLabel(ctiConstants_1.CtiConstants.BUI_CTI_LABEL_NOT_AVAILABLE);
            menuItemNotAvailable.setHandler(function () {
                _this.updateAgentStatus(ctiConstants_1.CtiConstants.NOT_AVAILABLE);
            });
            this.globalHeaderMenu.addMenuItem(menuItemNotAvailable);
            var menuItemBusy = this.globalHeaderMenu.createMenuItem();
            menuItemBusy.setLabel(ctiConstants_1.CtiConstants.BUI_CTI_LABEL_BUSY);
            menuItemBusy.setHandler(function () {
                _this.updateAgentStatus(ctiConstants_1.CtiConstants.BUSY);
            });
            this.globalHeaderMenu.addMenuItem(menuItemBusy);
            var menuItemLogout = this.globalHeaderMenu.createMenuItem();
            menuItemLogout.setLabel(ctiConstants_1.CtiConstants.BUI_CTI_LABEL_LOGOUT);
            menuItemLogout.setHandler(function () {
                _this.handleLogout();
            });
            this.globalHeaderMenu.addMenuItem(menuItemLogout);
            this.globalHeaderMenu.render();
        };
        /**
         * updateLeftPanelMenuAfterLogin - Updates the leftpanel menu
         * after successful login.
         */
        CtiTelephonyAddin.prototype.updateLeftPanelMenuAfterLogin = function () {
            this.buiCtiLeftPanelMenu.setLabel(this.ctiProviderConfig.providerName + ' ' + ctiConstants_1.CtiConstants.BUI_CTI_LABEL_AVAILABLE);
            this.buiCtiLeftPanelMenu.setVisible(true);
            this.buiCtiLeftPanelMenu.setDisabled(false);
            var icon = this.buiCtiLeftPanelMenu.createIcon(ctiConstants_1.CtiConstants.BUI_CTI_RIBBONBAR_ICON_TYPE);
            icon.setIconClass(ctiConstants_1.CtiConstants.BUI_CTI_LEFT_PANEL_ICON);
            //Get icon color based on default status
            var iconData = this.getIconDetailsForStatus(this.ctiProviderConfig.defaultStatus);
            icon.setIconColor(iconData.color);
            this.buiCtiLeftPanelMenu.addIcon(icon);
            this.buiCtiLeftPanelMenu.render();
        };
        /**
         *
         * @param status
         *
         * getIconDetailsForStatus - Returns the Icon details for
         * a given status code.
         */
        CtiTelephonyAddin.prototype.getIconDetailsForStatus = function (status) {
            var iconData = {
                "class": '',
                color: '',
                label: ''
            };
            switch (status) {
                case ctiConstants_1.CtiConstants.AVAILABLE:
                    iconData["class"] = ctiConstants_1.CtiConstants.BUI_CTI_ICON_CLASS_AVAILABLE;
                    iconData.color = 'green';
                    iconData.label = ctiConstants_1.CtiConstants.BUI_CTI_LABEL_AVAILABLE;
                    break;
                case ctiConstants_1.CtiConstants.NOT_AVAILABLE:
                    iconData["class"] = ctiConstants_1.CtiConstants.BUI_CTI_ICON_CLASS_NOT_AVAILABLE;
                    iconData.color = 'black';
                    iconData.label = ctiConstants_1.CtiConstants.BUI_CTI_LABEL_NOT_AVAILABLE;
                    break;
                case ctiConstants_1.CtiConstants.BUSY:
                    iconData["class"] = ctiConstants_1.CtiConstants.BUI_CTI_ICON_CLASS_BUSY;
                    iconData.color = 'red';
                    iconData.label = ctiConstants_1.CtiConstants.BUI_CTI_LABEL_BUSY;
                    break;
                case ctiConstants_1.CtiConstants.WAIT:
                    iconData["class"] = ctiConstants_1.CtiConstants.BUI_CTI_RIBBONBAR_ICON_WAIT;
                    iconData.color = 'black';
                    iconData.label = ctiConstants_1.CtiConstants.BUI_CTI_LABEL_WAIT;
            }
            return iconData;
        };
        /**
         * handleLogout - submit logout request
         * to the CTI tool. Also updates the UI
         */
        CtiTelephonyAddin.prototype.handleLogout = function () {
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
        };
        /**
         * summarizeCall - All handling for disconnecting
         * a call goes here
         *
         */
        CtiTelephonyAddin.prototype.summarizeCall = function (isTransfer, agentName) {
            var _this = this;
            var resolveRef;
            var rejectRef;
            var promiseRef = new ExtensionPromise(function (resolve, reject) {
                resolveRef = resolve;
                rejectRef = reject;
            });
            if (this.isCallSummarized) {
                ctiLogger_1.CtiLogger.logInfoMessage(this.logPreMessage +
                    ctiMessages_1.CtiMessages.MESSAGE_CALL_SUMMARIZED);
                return;
            }
            ctiLogger_1.CtiLogger.logInfoMessage(this.logPreMessage + ctiMessages_1.CtiMessages.MESSAGE_SUMMARIZE_CALL);
            this.onCall = false;
            this.isNotifyAllowed = true;
            if (this.workspaceRecord) {
                this.workspaceRecord.triggerNamedEvent('CallEnded');
            }
            //Add note to the thread
            if (this.incidentWorkspace && this.incidentWorkspace.getWorkspaceRecordId()) {
                this.incidentWorkspace.getCurrentEditedThread('NOTE', true)
                    .then(function (threadEntry) {
                    var content = ctiMessages_1.CtiMessages.MESSAGE_CALL_DURATION + _this.ctiInCallClock.getCallLength() + ' \n '
                        + ctiMessages_1.CtiMessages.MESSAGE_CALL_START + _this.ctiInCallClock.getClockStartTime() + ' \n '
                        + ctiMessages_1.CtiMessages.MESSAGE_CALL_END + _this.ctiInCallClock.getClockEndTime() + ' \n ';
                    if (isTransfer && agentName) {
                        content = content + ctiMessages_1.CtiMessages.MESSAGE_CALL_TRANSFERRED_TO + agentName;
                    }
                    threadEntry.setContent(content)
                        .then(function () {
                        var objectType = _this.incidentWorkspace.getWorkspaceRecordType();
                        var objectId = +_this.incidentWorkspace.getWorkspaceRecordId();
                        if (_this.workspaceRecord.isEditorOpen(objectType, objectId)) {
                            _this.incidentWorkspace.executeEditorCommand('saveAndClose').then(function () {
                                resolveRef();
                            })["catch"](function () {
                                rejectRef();
                            });
                        }
                        if (_this.interactionWorkspace.getWorkspaceRecordId()) {
                            _this.interactionWorkspace.executeEditorCommand('saveAndClose').then(function () {
                                resolveRef();
                            })["catch"](function () {
                                rejectRef();
                            });
                        }
                    });
                });
            }
            else {
                setTimeout(function () {
                    if (_this.interactionWorkspace && _this.interactionWorkspace.getWorkspaceRecordId()) {
                        _this.interactionWorkspace.closeEditor().then(function () {
                            resolveRef();
                        })["catch"](function () {
                            rejectRef();
                        });
                    }
                }, 100);
            }
            return promiseRef;
        };
        /***
         * Handles window unload, when user logs out from BUI
         */
        CtiTelephonyAddin.prototype.registerUnloadHandler = function () {
            var _this = this;
            window.addEventListener('beforeunload', function () {
                _this.handleLogout();
            });
        };
        return CtiTelephonyAddin;
    }());
    exports.CtiTelephonyAddin = CtiTelephonyAddin;
    new CtiTelephonyAddin(new twilioAdapter_1.TwilioAdapter()).enableCtiAddin();
});
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiY3RpVGVsZXBob255QWRkaW4uanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyJjdGlUZWxlcGhvbnlBZGRpbi50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTs7Ozs7Z0dBS2dHOzs7O0lBMEJoRzs7Ozs7Ozs7Ozs7Ozs7Ozs7OztPQW1CRztJQUNIO1FBNkJJLDJCQUFZLGdCQUE0QjtZQUF4QyxpQkFFQztZQWpCTyxlQUFVLEdBQVcsS0FBSyxDQUFDO1lBQzNCLFdBQU0sR0FBVyxLQUFLLENBQUM7WUFDdkIsb0JBQWUsR0FBVyxJQUFJLENBQUM7WUFLL0IscUJBQWdCLEdBQVksS0FBSyxDQUFDO1lBSWxDLG1CQUFjLEdBQVksS0FBSyxDQUFDO1lBRWhDLGtCQUFhLEdBQVcsbUJBQW1CLEdBQUcseUJBQVcsQ0FBQyxnQkFBZ0IsQ0FBQztZQW1DbkY7Ozs7O2VBS0c7WUFDSSw2QkFBd0IsR0FBRztnQkFDOUIscUJBQVMsQ0FBQyxjQUFjLENBQUMsS0FBSSxDQUFDLGFBQWE7b0JBQ3ZDLHlCQUFXLENBQUMsd0JBQXdCLENBQUMsQ0FBQztnQkFDMUMsS0FBSSxDQUFDLDBCQUEwQixFQUFFLENBQUM7Z0JBQ2xDLEtBQUksQ0FBQyxxQkFBcUIsRUFBRSxDQUFDO2dCQUM3QixLQUFJLENBQUMscUJBQXFCLEVBQUUsQ0FBQztnQkFFN0Isb0JBQW9CO2dCQUNwQixLQUFJLENBQUMsc0JBQXNCLEVBQUUsQ0FBQztnQkFDOUIsS0FBSSxDQUFDLFlBQVksQ0FBQyw4QkFBOEIsQ0FBQyxVQUFDLG9CQUEwQztvQkFDeEYsb0JBQW9CLENBQUMsc0JBQXNCLEVBQUU7eUJBQ3hDLElBQUksQ0FBQyxVQUFDLGdCQUFxQzt3QkFDcEMsS0FBSSxDQUFDLG1CQUFtQixHQUFHLGdCQUFnQixDQUFDO3dCQUM1QyxLQUFJLENBQUMsaUJBQWlCLEdBQUcsS0FBSSxDQUFDLFVBQVUsQ0FBQyxnQkFBZ0IsRUFBRSxDQUFDO3dCQUM1RCxnQkFBZ0IsQ0FBQyxPQUFPLENBQUMsMkJB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Q0FBQyxDQUFDO3dCQUNQLENBQUM7b0JBQ0wsQ0FBQyxDQUNKLENBQUM7Z0JBQ1YsQ0FBQyxDQUFDLENBQUM7WUFDWCxDQUFDO1lBQUMsSUFBSSxDQUFDLENBQUM7Z0JBQ0osVUFBVSxDQUFDO29CQUNQLEVBQUUsQ0FBQyxDQUFDLEtBQUksQ0FBQyxvQkFBb0IsSUFBSSxLQUFJLENBQUMsb0JBQW9CLENBQUMsb0JBQW9CLEVBQUUsQ0FBQyxDQUFDLENBQUM7d0JBQ2hGLEtBQUksQ0FBQyxvQkFBb0IsQ0FBQyxXQUFXLEVBQUUsQ0FBQyxJQUFJLENBQUM7NEJBQ3pDLFVBQVUsRUFBRSxDQUFDO3dCQUNqQixDQUFDLENBQUMsQ0FBQyxPQUFLLENBQUEsQ0FBQzs0QkFDTCxTQUFTLEVBQUUsQ0FBQzt3QkFDaEIsQ0FBQyxDQUFDLENBQUM7b0JBQ1AsQ0FBQztnQkFDTCxDQUFDLEVBQUUsR0FBRyxDQUFDLENBQUM7WUFDWixDQUFDO1lBQ0QsTUFBTSxDQUFDLFVBQVUsQ0FBQztRQUN0QixDQUFDO1FBRUQ7O1dBRUc7UUFDSyxpREFBcUIsR0FBN0I7WUFBQSxpQkFJQztZQUhHLE1BQU0sQ0FBQyxnQkFBZ0IsQ0FBQyxjQUFjLEVBQUU7Z0JBQ3BDLEtBQUksQ0FBQyxZQUFZLEVBQUUsQ0FBQztZQUN4QixDQUFDLENBQUMsQ0FBQztRQUNQLENBQUM7UUFDTCx3QkFBQztJQUFELENBQUMsQUFoNEJELElBZzRCQztJQWg0QlksOENBQWlCO0lBazRCOUIsSUFBSSxpQkFBaUIsQ0FBQyxJQUFJLDZCQUFhLEVBQUUsQ0FBQyxDQUFDLGNBQWMsRUFBRSxDQUFDIiwic291cmNlc0NvbnRlbnQiOlsiLyogKiAqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqXG4gKiAgJEFDQ0VMRVJBVE9SX0hFQURFUl9QTEFDRV9IT0xERVIkXG4gKiAgU0hBMTogJElkOiA4NGEzMWYzYTkxZTUyNzgzNTFiYWVjMTQ0ZWQ1ODAxNDg5OGFlYWM1ICRcbiAqICoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKlxuICogIEZpbGU6ICRBQ0NFTEVSQVRPUl9IRUFERVJfRklMRV9OQU1FX1BMQUNFX0hPTERFUiRcbiAqICoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKiAqL1xuXG4vLy88cmVmZXJlbmNlIHBhdGg9Jy4uLy4uLy4uL2RlZmluaXRpb25zL29zdmNFeHRlbnNpb24uZC50cycgLz5cblxuaW1wb3J0IHtDdGlDb25zdGFudHN9IGZyb20gJy4vLi4vdXRpbC9jdGlDb25zdGFudHMnO1xuaW1wb3J0IHtJQ1RJQWRhcHRlcn0gZnJvbSAnLi4vY29udHJhY3RzL2lDVElBZGFwdGVyJztcbmltcG9ydCB7Q3RpQ29uZmlndXJhdGlvbn0gZnJvbSAnLi4vbW9kZWwvY3RpQ29uZmlndXJhdGlvbic7XG5pbXBvcnQge0ljb25EYXRhfSBmcm9tICcuLi9tb2RlbC9pY29uRGF0YSc7XG5pbXBvcnQge0N0aVZpZXdIZWxwZXJ9IGZyb20gJy4vLi4vdXRpbC9jdGlWaWV3SGVscGVyJztcbmltcG9ydCB7QWdlbnRQcm9maWxlfSBmcm9tIFwiLi4vbW9kZWwvYWdlbnRQcm9maWxlXCI7XG5pbXBvcnQgSUV4dGVuc2lvblByb3ZpZGVyID0gT1JBQ0xFX1NFUlZJQ0VfQ0xPVUQuSUV4dGVuc2lvblByb3ZpZGVyO1xuaW1wb3J0IElVc2VySW50ZXJmYWNlQ29udGV4dCA9IE9SQUNMRV9TRVJWSUNFX0NMT1VELklVc2VySW50ZXJmYWNlQ29udGV4dDtcbmltcG9ydCBJR2xvYmFsSGVhZGVyQ29udGV4dCA9IE9SQUNMRV9TRVJWSUNFX0NMT1VELklHbG9iYWxIZWFkZXJDb250ZXh0O1xuaW1wb3J0IElHbG9iYWxIZWFkZXJNZW51ID0gT1JBQ0xFX1NFUlZJQ0VfQ0xPVUQuSUdsb2JhbEhlYWRlck1lbnU7XG5pbXBvcnQgSUlDb24gPSBPUkFDTEVfU0VSVklDRV9DTE9VRC5JSUNvbjtcbmltcG9ydCBJR2xvYmFsSGVhZGVyTWVudUl0ZW0gPSBPUkFDTEVfU0VSVklDRV9DTE9VRC5JR2xvYmFsSGVhZGVyTWVudUl0ZW07XG5pbXBvcnQgSVdvcmtzcGFjZVJlY29yZCA9IE9SQUNMRV9TRVJWSUNFX0NMT1VELklXb3Jrc3BhY2VSZWNvcmQ7XG5pbXBvcnQgSUluY2lkZW50V29ya3NwYWNlUmVjb3JkID0gT1JBQ0xFX1NFUlZJQ0VfQ0xPVUQuSUluY2lkZW50V29ya3NwYWNlUmVjb3JkO1xuaW1wb3J0IElTaWRlUGFuZUNvbnRleHQgPSBPUkFDTEVfU0VSVklDRV9DTE9VRC5JU2lkZVBhbmVDb250ZXh0O1xuaW1wb3J0IElTaWRlUGFuZSA9IE9SQUNMRV9TRVJWSUNFX0NMT1VELklTaWRlUGFuZTtcbmltcG9ydCB7VHdpbGlvQWRhcHRlcn0gZnJvbSBcIi4uL2FkYXB0ZXIvdHdpbGlvQWRhcHRlclwiO1xuaW1wb3J0IHtBZ2VudERhdGF9IGZyb20gXCIuLi9tb2RlbC9hZ2VudERhdGFcIjtcbmltcG9ydCB7Q3RpTG9nZ2VyfSBmcm9tIFwiLi4vdXRpbC9jdGlMb2dnZXJcIjtcbmltcG9ydCB7Q3RpQ2xvY2t9IGZyb20gXCIuLi91dGlsL2N0aUNsb2NrXCI7XG5pbXBvcnQge0N0aU1lc3NhZ2VzfSBmcm9tIFwiLi4vdXRpbC9jdGlNZXNzYWdlc1wiO1xuXG4vKipcbiAqIEN0aVRlbGVwaG9ueUFkZGluIC0gVGhpcyBjbGFzcyBjb250cm9scyB0aGUgd2hvbGUgQ2xpZW50IHNpZGVcbiAqIGFkZGluIG9wZXJhdGlvbnMuXG4gKlxuICogVGhlIEN0aVRlbGVwaG9ueUFkZGluIGlzIGRlc2lnbmVkIGFzIGEgbGVmdCBTaWRlUGFuZSBhZGRpblxuICpcbiAqIEFuIGluc3RhbmNlIG9mIHRoZSBJQ1RJQWRhcHRlciBpbXBsZW1lbnRhdGlvblxuICogaGFzIHRvIGJlIGdpdmVuIGF0IHRoZSB0aW1lIG9mIGNyZWF0aW9uLCB3aGljaCB3aWxsIGJlIHVzZWRcbiAqIGZvciBjb21tdW5pY2F0aW5nIHdpdGggQ1RJIFBsYXRmb3JtXG4gKlxuICogQ3RpVGVsZXBob255QWRkaW4gd2lsbCBub3QgbWFrZSBhbnkgZGlyZWN0IGNvbW11bmljYXRpb24gd2l0aFxuICogdGhlIHVuZGVybHlpbmcgQ1RJIFBsYXRmb3JtL1NlcnZlci4gQWxsIGNvbW11bmljYXRpb24gc2hvdWxkIGJlXG4gKiB0aHJvdWdoIENUSUFkYXB0ZXJcbiAqXG4gKiBDdGlUZWxlcGhvbnlBZGRpbiB1dGlsaXplIGV2ZW50IGJhc2VkIGNvbW11bmljYXRpb24gd2l0aCB0aGVcbiAqIHVuZGVybHlpbmcgQ1RJIFBsYXRmb3JtLiBUaGUgYWRkaW4gd2lsbCByZWdpc3RlciBoYW5kbGVyc1xuICogZm9yIHZhcmlvdXMgZXZlbnRzIHdpdGggdGhlIHN5c3RlbSBhbmQgdGhlIHN5c3RlbSBpbnZva2UgY29ycmVzcG9uZGluZ1xuICogaGFuZGxlciB3aGVuIGFuIGV2ZW50IG9jY3Vycy5cbiAqXG4gKi9cbmV4cG9ydCBjbGFzcyBDdGlUZWxlcGhvbnlBZGRpbiB7XG5cbiAgICBwcml2YXRlIGV4dGVuc2lvblNkazpJRXh0ZW5zaW9uUHJvdmlkZXI7XG4gICAgcHJpdmF0ZSBnbG9iYWxIZWFkZXJDb250ZXh0OklHbG9iYWxIZWFkZXJDb250ZXh0O1xuICAgIHByaXZhdGUgaW50ZXJhY3Rpb25Xb3Jrc3BhY2U6SVdvcmtzcGFjZVJlY29yZDtcbiAgICBwcml2YXRlIHdvcmtzcGFjZVJlY29yZDpJV29ya3NwYWNlUmVjb3JkO1xuICAgIHByaXZhdGUgaW5jaWRlbnRXb3Jrc3BhY2U6SUluY2lkZW50V29ya3NwYWNlUmVjb3JkO1xuICAgIHByaXZhdGUgYnVpQ3RpTGVmdFBhbmVsTWVudTpJU2lkZVBhbmU7XG4gICAgcHJpdmF0ZSBnbG9iYWxIZWFkZXJNZW51OklHbG9iYWxIZWFkZXJNZW51O1xuXG4gICAgcHJpdmF0ZSBjdGlQcm92aWRlckNvbmZpZzpDdGlDb25maWd1cmF0aW9uO1xuICAgIHByaXZhdGUgY3RpQWRhcHRlcjpJQ1RJQWRhcHRlcjtcbiAgICBwcml2YXRlIGN0aUluQ2FsbENsb2NrOkN0aUNsb2NrO1xuICAgIHByaXZhdGUgcHJldlN0YXR1czpzdHJpbmc7XG4gICAgcHJpdmF0ZSBpc0xvZ2dlZEluOmJvb2xlYW4gPSBmYWxzZTtcbiAgICBwcml2YXRlIG9uQ2FsbDpib29sZWFuID0gZmFsc2U7XG4gICAgcHJpdmF0ZSBpc05vdGlmeUFsbG93ZWQ6Ym9vbGVhbiA9IHRydWU7XG4gICAgcHJpdmF0ZSBjYWxsZXJEYXRhOmFueTtcbiAgICBwcml2YXRlIGRpYWxlZENvbnRhY3Q6YW55O1xuICAgIHByaXZhdGUgaXNPdXRHb2luZzpib29sZWFuO1xuICAgIHByaXZhdGUgbWVudUl0ZW1Mb2dpbjpJR2xvYmFsSGVhZGVyTWVudUl0ZW07XG4gICAgcHJpdmF0ZSBpc0NhbGxTdW1tYXJpemVkOiBib29sZWFuID0gZmFsc2U7XG4gICAgcHJpdmF0ZSBnbG9iYWxDb250ZXh0OiBPUkFDTEVfU0VSVklDRV9DTE9VRC5JRXh0ZW5zaW9uR2xvYmFsQ29udGV4dDtcbiAgICBwcml2YXRlIGFnZW50UHJvZmlsZTogQWdlbnRQcm9maWxlO1xuICAgIHByaXZhdGUgcmluZ01lZGlhOiBhbnk7XG4gICAgcHJpdmF0ZSBvbkNhbGxUcmFuc2ZlcjogYm9vbGVhbiA9IGZhbHNlO1xuXG4gICAgcHJpdmF0ZSBsb2dQcmVNZXNzYWdlOiBzdHJpbmcgPSAnQ3RpVGVsZXBob255QWRkaW4nICsgQ3RpTWVzc2FnZXMuTUVTU0FHRV9BUFBFTkRFUjtcblxuICAgIGNvbnN0cnVjdG9yKGN0aUNsaWVudEFkYXB0ZXI6SUNUSUFkYXB0ZXIpIHtcbiAgICAgICAgdGhpcy5jdGlBZGFwdGVyID0gY3RpQ2xpZW50QWRhcHRlcjtcbiAgICB9XG5cbiAgICAvKipcbiAgICAgKiBUaGlzIG1ldGhvZCBpbml0aWF0ZXMgdGhlIGF1dGhvcml6YXRpb24gcHJvY2Vzcy4gSXQgY2hlY2tzIHRoZSBDVEkgYWNjZXNzIGZvclxuICAgICAqIGN1cnJlbnRseSBsb2dnZWQtaW4gdXNlciBhbmQgZW5hYmxlcyB0aGUgYWRkaW4gaWYgaXQgaXMgYWNjZXNzaWJsZSB0byB0aGVcbiAgICAgKiBhZ2VudC5cbiAgICAgKlxuICAgICAqIEl0IHJlZ2lzdGVycyBhIGhhbmRsZXIgZm9yIGV2ZW50IC0gJ2N0aS5lbmFibGVkJyB3aXRoIHRoZSBDVElBZGFwdGVyLlxuICAgICAqIFRoaXMgaGFuZGxlciB3aWxsIGVuYWJsZSBhZGRpbiBmb3IgYWdlbnQuIFVuZGVybHlpbmcgQ1RJIEFkYXB0ZXJcbiAgICAgKiBhdXRob3JpemUgdGhlIGFnZW50IGFuZCBpbnZva2VzIHRoZSBoYW5kbGVyIGlmIENUSSBpcyBlbmFibGVkIGZvciBjdXJyZW50IGFnZW50XG4gICAgICpcbiAgICAgKi9cbiAgICBwdWJsaWMgZW5hYmxlQ3RpQWRkaW4oKTp2b2lkIHtcbiAgICAgICAgLy9SZWdpc3RlciBoYW5kbGVyIG9ubHkgZm9yIGVuYWJsaW5nIGFkZGluXG4gICAgICAgIEN0aUxvZ2dlci5sb2dJbmZvTWVzc2FnZSh0aGlzLmxvZ1ByZU1lc3NhZ2UrIEN0aU1lc3NhZ2VzLk1FU1NBR0VfTE9BRF9FWFRFTlNJT04pO1xuICAgICAgICB0aGlzLmN0aUFkYXB0ZXIuYWRkRXZlbnRIYW5kbGVyKCdjdGkuZW5hYmxlZCcsIHRoaXMuaW5pdGlhbGl6ZUN0aVRvb2xCYXJNZW51KTtcbiAgICAgICAgT1JBQ0xFX1NFUlZJQ0VfQ0xPVUQuZXh0ZW5zaW9uX2xvYWRlci5sb2FkKEN0aUNvbnN0YW50cy5CVUlfQ1RJX0FERElOX0lELCBDdGlDb25zdGFudHMuQlVJX0NUSV9BRERJTl9WRVJTSU9OKVxuICAgICAgICAgICAgLnRoZW4oKHNkazpJRXh0ZW5zaW9uUHJvdmlkZXIpID0+IHtcbiAgICAgICAgICAgICAgICB0aGlzLmV4dGVuc2lvblNkayA9IHNkaztcbiAgICAgICAgICAgICAgICBDdGlMb2dnZXIubG9nSW5mb01lc3NhZ2UodGhpcy5sb2dQcmVNZXNzYWdlK1xuICAgICAgICAgICAgICAgICAgICBDdGlNZXNzYWdlcy5NRVNTQUdFX09CVEFJTkVEX1NESyk7XG4gICAgICAgICAgICAgICAgc2RrLmdldEdsb2JhbENvbnRleHQoKS50aGVuKFxuICAgICAgICAgICAgICAgICAgICAoZ2xvYmFsQ29udGV4dDpPUkFDTEVfU0VSVklDRV9DTE9VRC5JRXh0ZW5zaW9uR2xvYmFsQ29udGV4dCkgPT4ge1xuICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5nbG9iYWxDb250ZXh0ID0gZ2xvYmFsQ29udGV4dDtcbiAgICAgICAgICAgICAgICAgICAgICAgIGdsb2JhbENvbnRleHQuZ2V0U2Vzc2lvblRva2VuKCkudGhlbigoc2Vzc2lvblRva2VuOnN0cmluZykgPT4ge1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRoaXMuY3RpQWRhcHRlci5hdXRob3JpemVBZ2VudChnbG9iYWxDb250ZXh0LmdldEludGVyZmFjZVVybCgpLCBzZXNzaW9uVG9rZW4pO1xuICAgICAgICAgICAgICAgICAgICAgICAgfSk7XG4gICAgICAgICAgICAgICAgICAgIH0pO1xuICAgICAgICAgICAgfSk7XG4gICAgfVxuXG4gICAgLyoqXG4gICAgICogVGhpcyBtZXRob2QgaW5pdGlhbGl6ZSB0aGUgVG9vbGJhciBNZW51IEljb25cbiAgICAgKiBmb3IgQ1RJIEFkZGluLiBUaGlzIGlzIHRoZSBpY29uXG4gICAgICogZnJvbSB3aGVyZSBBZ2VudCBjYW4gbG9naW4gdG8gdGhlIENUSSBUb29sXG4gICAgICpcbiAgICAgKi9cbiAgICBwdWJsaWMgaW5pdGlhbGl6ZUN0aVRvb2xCYXJNZW51ID0gKCk6dm9pZCA9PiB7XG4gICAgICAgIEN0aUxvZ2dlci5sb2dJbmZvTWVzc2FnZSh0aGlzLmxvZ1ByZU1lc3NhZ2UrXG4gICAgICAgICAgICBDdGlNZXNzYWdlcy5NRVNTQUdFX0lOSVRJQUxJWkVfQURESU4pO1xuICAgICAgICB0aGlzLmluaXRpYWxpemVSaW5nTWVkaWFFbGVtZW50KCk7XG4gICAgICAgIHRoaXMuZW5hYmxlRGlhbFBhZENvbnRyb2xzKCk7XG4gICAgICAgIHRoaXMucmVnaXN0ZXJVbmxvYWRIYW5kbGVyKCk7XG5cbiAgICAgICAgLy9Mb2FkIE1lbnViYXIgYWRkaW5cbiAgICAgICAgdGhpcy5pbml0aWFsaXplTGVmdHBhbmVJY29uKCk7XG4gICAgICAgIHRoaXMuZXh0ZW5zaW9uU2RrLnJlZ2lzdGVyVXNlckludGVyZmFjZUV4dGVuc2lvbigodXNlckludGVyZmFjZUNvbnRleHQ6SVVzZXJJbnRlcmZhY2VDb250ZXh0KSA9PiB7XG4gICAgICAgICAgICB1c2VySW50ZXJmYWNlQ29udGV4dC5nZXRHbG9iYWxIZWFkZXJDb250ZXh0KClcbiAgICAgICAgICAgICAgICAudGhlbigocmliYm9uQmFyQ29udGV4dDpJR2xvYmFsSGVhZGVyQ29udGV4dCkgPT4ge1xuICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5nbG9iYWxIZWFkZXJDb250ZXh0ID0gcmliYm9uQmFyQ29udGV4dDtcbiAgICAgICAgICAgICAgICAgICAgICAgIHRoaXMuY3RpUHJvdmlkZXJDb25maWcgPSB0aGlzLmN0aUFkYXB0ZXIuZ2V0Q29uZmlndXJhdGlvbigpO1xuICAgICAgICAgICAgICAgICAgICAgICAgcmliYm9uQmFyQ29udGV4dC5nZXRNZW51KEN0aUNvbnN0YW50cy5CVUlfQ1RJX1JJQkJPTkJBUl9NRU5VX0lEKVxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIC50aGVuKChyaWJib25CYXJNZW51OklHbG9iYWxIZWFkZXJNZW51KSA9PiB7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRoaXMuZ2xvYmFsSGVhZGVyTWVudSA9IHJpYmJvbkJhck1lbnU7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHZhciBpY29uOklJQ29uID0gcmliYm9uQmFyTWVudS5jcmVhdGVJY29uKEN0aUNvbnN0YW50cy5CVUlfQ1RJX1JJQkJPTkJBUl9JQ09OX1RZUEUpO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBpY29uLnNldEljb25DbGFzcyhDdGlDb25zdGFudHMuQlVJX0NUSV9SSUJCT05CQVJfSUNPTl9ERUZBVUxUX0NMQVNTKTtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgaWNvbi5zZXRJY29uQ29sb3IoQ3RpQ29uc3RhbnRzLkJVSV9DVElfUklCQk9OQkFSX0lDT05fREVGQVVMVF9DT0xPUik7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHJpYmJvbkJhck1lbnUuYWRkSWNvbihpY29uKTtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgcmliYm9uQmFyTWVudS5zZXREaXNhYmxlZChmYWxzZSk7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHJpYmJvbkJhck1lbnUuc2V0TGFiZWwodGhpcy5jdGlQcm92aWRlckNvbmZpZy5wcm92aWRlck5hbWUgKyAnICcgKyBDdGlDb25zdGFudHMuQlVJX0NUSV9MQUJFTF9MT0dHRURfT1VUKTtcblxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB0aGlzLm1lbnVJdGVtTG9naW4gPSByaWJib25CYXJNZW51LmNyZWF0ZU1lbnVJdGVtKCk7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRoaXMubWVudUl0ZW1Mb2dpbi5zZXRMYWJlbChDdGlDb25zdGFudHMuQlVJX0NUSV9MQUJFTF9MT0dJTik7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRoaXMubWVudUl0ZW1Mb2dpbi5zZXRIYW5kbGVyKFxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgKG1lbnVJdGVtOklHbG9iYWxIZWFkZXJNZW51SXRlbSkgPT4ge1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRoaXMuaGFuZGxlTG9naW4oKTtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIH0pO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICByaWJib25CYXJNZW51LmFkZE1lbnVJdGVtKHRoaXMubWVudUl0ZW1Mb2dpbik7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHJpYmJvbkJhck1lbnUucmVuZGVyKCk7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgfSk7XG4gICAgICAgICAgICAgICAgICAgICAgICB0aGlzLmlzTG9nZ2VkSW4gPSBmYWxzZTtcbiAgICAgICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgICk7XG4gICAgICAgIH0pO1xuICAgIH07XG5cbiAgICAvKipcbiAgICAgKiBDcmVhdGUgdGhlIGF1ZGlvIGVsZW1lbnQgd2hpY2ggaXMgdXNlZCB0byBwbGF5XG4gICAgICogdGhlICdyaW5nJyBmb3IgaW5jb21pbmcgY2FsbHNcbiAgICAgKi9cbiAgICBwdWJsaWMgaW5pdGlhbGl6ZVJpbmdNZWRpYUVsZW1lbnQoKTogdm9pZCB7XG4gICAgICAgIHRoaXMucmluZ01lZGlhID0gZG9jdW1lbnQuY3JlYXRlRWxlbWVudCgnYXVkaW8nKTtcbiAgICAgICAgdGhpcy5yaW5nTWVkaWEuc2V0QXR0cmlidXRlKCdzcmMnLCB0aGlzLmN0aUFkYXB0ZXIuZ2V0UmluZ01lZGlhVXJsKCkpO1xuICAgICAgICB0aGlzLnJpbmdNZWRpYS5hZGRFdmVudExpc3RlbmVyKCdlbmRlZCcsIGZ1bmN0aW9uKCkge1xuICAgICAgICAgICAgdGhpcy5wbGF5KCk7XG4gICAgICAgIH0sIGZhbHNlKTtcbiAgICB9XG5cbiAgICAvKipcbiAgICAgKiBJbml0aWFsaXplcyB0aGUgbGVmdCBzaWRlIGljb24uIFRoaXMgaWNvbiBpcyB1c2VkIHRvIGNvbnRyb2xcbiAgICAgKiB0aGUgZGlhbGVyL29uLWNhbGwgVUkuIFRoaXMgaWNvbiB3aWxsIGJlIGtlcHQgaGlkZGVuIHVudGlsIHVzZXJcbiAgICAgKiBsb2dpbiB0byBDVElcbiAgICAgKi9cbiAgICBwdWJsaWMgaW5pdGlhbGl6ZUxlZnRwYW5lSWNvbigpOnZvaWQge1xuICAgICAgICBDdGlMb2dnZXIubG9nSW5mb01lc3NhZ2UodGhpcy5sb2dQcmVNZXNzYWdlK1xuICAgICAgICAgICAgQ3RpTWVzc2FnZXMuTUVTU0FHRV9JTklUSUFMSVpFX1NJREVQQU5FTCk7XG4gICAgICAgIHRoaXMuZXh0ZW5zaW9uU2RrLnJlZ2lzdGVyVXNlckludGVyZmFjZUV4dGVuc2lvbigodXNlckludGVyZmFjZUNvbnRleHQ6SVVzZXJJbnRlcmZhY2VDb250ZXh0KSA9PiB7XG4gICAgICAgICAgICB1c2VySW50ZXJmYWNlQ29udGV4dC5nZXRMZWZ0U2lkZVBhbmVDb250ZXh0KCkudGhlbihcbiAgICAgICAgICAgICAgICAobGVmdFNpZGVQYW5lQ29udGV4dDpJU2lkZVBhbmVDb250ZXh0KSA9PiB7XG4gICAgICAgICAgICAgICAgICAgIGxlZnRTaWRlUGFuZUNvbnRleHQuZ2V0U2lkZVBhbmUoQ3RpQ29uc3RhbnRzLkJVSV9DVElfTEVGVF9QQU5FTF9NRU5VX0lEKVxuICAgICAgICAgICAgICAgICAgICAgICAgLnRoZW4oKGxlZnRQYW5lbE1lbnU6SVNpZGVQYW5lKSA9PiB7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5idWlDdGlMZWZ0UGFuZWxNZW51ID0gbGVmdFBhbmVsTWVudTtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBsZWZ0UGFuZWxNZW51LnNldExhYmVsKCcnKTtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBsZWZ0UGFuZWxNZW51LnNldFZpc2libGUoZmFsc2UpO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHZhciBpY29uID0gbGVmdFBhbmVsTWVudS5jcmVhdGVJY29uKEN0aUNvbnN0YW50cy5CVUlfQ1RJX1JJQkJPTkJBUl9JQ09OX1RZUEUpO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGljb24uc2V0SWNvbkNsYXNzKEN0aUNvbnN0YW50cy5CVUlfQ1RJX0xFRlRfUEFORUxfSUNPTik7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgbGVmdFBhbmVsTWVudS5hZGRJY29uKGljb24pO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGxlZnRQYW5lbE1lbnUucmVuZGVyKCk7XG4gICAgICAgICAgICAgICAgICAgICAgICB9KTtcbiAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICApO1xuICAgICAgICB9KTtcbiAgICB9XG5cbiAgICAvKipcbiAgICAgKiByZWdpc3Rlckxpc3RlbmVyc1RvQWRhcHRlciAtIFJlZ2lzdGVyIGxpc3Rlcm5zXG4gICAgICogd2l0aCB0aGUgQ1RJIGFkYXB0ZXIgZm9yIGV2ZW50cyB0byBiZSBoYW5kbGVkXG4gICAgICpcbiAgICAgKiBUaGUgQ1RJQWRhcHRlciB3aWxsIGludm9rZSByZXF1aXJlZCBoYW5kbGVycywgd2hlbiBhblxuICAgICAqIGV2ZW50IG9jY3Vycy5cbiAgICAgKlxuICAgICAqL1xuICAgIHB1YmxpYyByZWdpc3Rlckxpc3RlbmVyc1RvQWRhcHRlcigpOnZvaWQge1xuICAgICAgICAvL0NsZWFyIGFueSBwcmV2aW91cyBoYW5kbGVyc1xuICAgICAgICB0aGlzLmN0aUFkYXB0ZXIuY2xlYXJBbGxFdmVudEhhbmRsZXJzKCk7XG4gICAgICAgIEN0aUxvZ2dlci5sb2dJbmZvTWVzc2FnZSh0aGlzLmxvZ1ByZU1lc3NhZ2UgKyBDdGlNZXNzYWdlcy5NRVNTQUdFX1JFR0lTVEVSX0VWRU5UX0hBTkRMRVJTKTtcbiAgICAgICAgLy9SZWdpc3RlciBoYW5kbGVyc1xuICAgICAgICB0aGlzLmN0aUFkYXB0ZXIuYWRkRXZlbnRIYW5kbGVyKCdsb2dpbi5zdWNjZXNzJywgdGhpcy5oYW5kbGVMb2dpblN1Y2Nlc3MpO1xuICAgICAgICB0aGlzLmN0aUFkYXB0ZXIuYWRkRXZlbnRIYW5kbGVyKCdhY3Rpdml0eS51cGRhdGUnLCB0aGlzLmhhbmRsZVN0YXR1c1VwZGF0ZXNGcm9tU2VydmVyKTtcbiAgICAgICAgdGhpcy5jdGlBZGFwdGVyLmFkZEV2ZW50SGFuZGxlcignbG9naW4uZmFpbGVkJywgdGhpcy5oYW5kbGVMb2dpbkZhaWx1cmUpO1xuICAgICAgICB0aGlzLmN0aUFkYXB0ZXIuYWRkRXZlbnRIYW5kbGVyKCdpbmNvbWluZycsIHRoaXMuaGFuZGxlSW5jb21pbmcpO1xuICAgICAgICB0aGlzLmN0aUFkYXB0ZXIuYWRkRXZlbnRIYW5kbGVyKCdjb25uZWN0ZWQnLCB0aGlzLmhhbmRsZUNhbGxDb25uZWN0ZWQpO1xuICAgICAgICB0aGlzLmN0aUFkYXB0ZXIuYWRkRXZlbnRIYW5kbGVyKCdkaXNjb25uZWN0ZWQnLCB0aGlzLmhhbmRsZUNhbGxEaXNjb25uZWN0ZWQpO1xuICAgICAgICB0aGlzLmN0aUFkYXB0ZXIuYWRkRXZlbnRIYW5kbGVyKCdjYW5jZWxlZCcsIHRoaXMuaGFuZGxlQ2FsbENhbmNlbGxlZCk7XG4gICAgICAgIHRoaXMuY3RpQWRhcHRlci5hZGRFdmVudEhhbmRsZXIoJ3RpbWVvdXQnLCB0aGlzLmhhbmRsZUNhbGxUaW1lT3V0KTtcbiAgICAgICAgdGhpcy5jdGlBZGFwdGVyLmFkZEV2ZW50SGFuZGxlcignc2VhcmNoLmNvbnRhY3QuZmFpbGVkJywgdGhpcy5jb250YWN0U2VhcmNoSGFuZGxlcik7XG4gICAgICAgIHRoaXMuY3RpQWRhcHRlci5hZGRFdmVudEhhbmRsZXIoJ3NlYXJjaC5jb250YWN0LmNvbXBsZXRlJywgdGhpcy5jb250YWN0U2VhcmNoSGFuZGxlcik7XG4gICAgICAgIHRoaXMuY3RpQWRhcHRlci5hZGRFdmVudEhhbmRsZXIoJ3NlYXJjaC5hZ2VudGxpc3QuY29tcGxldGUnLCB0aGlzLmhhbmRsZUFnZW50U2VhcmNoU3VjY2Vzcyk7XG4gICAgICAgIHRoaXMuY3RpQWRhcHRlci5hZGRFdmVudEhhbmRsZXIoJ3Rva2VuLmV4cGlyZWQnLCB0aGlzLnRva2VuRXhwaXJ5SGFuZGxlcik7XG4gICAgfVxuXG4gICAgLyoqXG4gICAgICogVGhpcyBmdW5jdGlvbiBhZGRzIGV2ZW50IGxpc3RlbmVycyB0byB0aGVcbiAgICAgKiBDVEkgRGlhbGVyXG4gICAgICovXG4gICAgcHVibGljIGVuYWJsZURpYWxQYWRDb250cm9scygpOnZvaWQge1xuICAgICAgICBDdGlWaWV3SGVscGVyLmFkZERpYWxQYWRDb250cm9scyh0aGlzLm91dGdvaW5nQ2FsbEhhbmRsZXIpO1xuICAgIH1cblxuICAgIC8qKlxuICAgICAqIFRoaXMgbWV0aG9kIGluaXRpYXRlcyBhbiBvdXRib3VuZCBjYWxsLiBJdCB0cmlnZ2VycyBhIHNlYXJjaFxuICAgICAqIGZvciB0aGUgZ2l2ZW4gY29udGFjdCBhbmQgcmVuZGVycyB0aGUgb24tY2FsbCBVSS4gVGhlIGFjdHVhbFxuICAgICAqIGNhbGwgd2lsbCBub3QgYmUgdHJpZ2dlcmVkIGZyb20gaGVyZS4gSXQgd2lsbCBiZSB0cmlnZ2VyZWRcbiAgICAgKiBhZnRlciBhIHRoZSBzZWFyY2ggY29tcGxldGlvblxuICAgICAqXG4gICAgICogQHBhcmFtIGV2ZW50XG4gICAgICovXG4gICAgcHVibGljIG91dGdvaW5nQ2FsbEhhbmRsZXIgPSAoZXZlbnQ6YW55KTp2b2lkID0+IHtcbiAgICAgICAgdmFyIGRpYWxlZE51bWJlcjpzdHJpbmcgPSBDdGlWaWV3SGVscGVyLmdldE91dGdvaW5nQ29udGFjdE51bWJlcigpO1xuICAgICAgICB0aGlzLmxvZ0NhbGxBY3Rpb24oQ3RpTWVzc2FnZXMuTUVTU0FHRV9IQU5ETEVfT1VUR09JTkdfQ0FMTCArXG4gICAgICAgICAgICBDdGlNZXNzYWdlcy5NRVNTQUdFX0FQUEVOREVSICsgZGlhbGVkTnVtYmVyKTtcbiAgICAgICAgaWYgKGRpYWxlZE51bWJlciAhPT0gJycpIHtcbiAgICAgICAgICAgIHRoaXMudXBkYXRlQWdlbnRTdGF0dXMoQ3RpQ29uc3RhbnRzLkJVU1kpO1xuICAgICAgICAgICAgdGhpcy5vbkNhbGwgPSB0cnVlO1xuICAgICAgICAgICAgdGhpcy5pc05vdGlmeUFsbG93ZWQgPSBmYWxzZTtcblxuICAgICAgICAgICAgdGhpcy5nbG9iYWxDb250ZXh0LmdldFNlc3Npb25Ub2tlbigpLnRoZW4oKHNlc3Npb25Ub2tlbjpzdHJpbmcpID0+IHtcbiAgICAgICAgICAgICAgICB0aGlzLmN0aUFkYXB0ZXIuc2VhcmNoQ29udGFjdChDdGlWaWV3SGVscGVyLmdldE91dGdvaW5nQ29udGFjdE51bWJlcigpLCBzZXNzaW9uVG9rZW4pO1xuICAgICAgICAgICAgfSk7XG5cbiAgICAgICAgICAgIEN0aVZpZXdIZWxwZXIuc2hvd09uQ2FsbFVJKGRpYWxlZE51bWJlciwgQ3RpQ29uc3RhbnRzLkRFRkFVTFRfRElTUExBWV9JQ09OKTtcbiAgICAgICAgfVxuICAgIH07XG5cbiAgICAvKipcbiAgICAgKiBIYW5kbGVyIGZvciBldmVudHMgLSAnc2VhcmNoLmNvbnRhY3QuY29tcGxldGUnIGFuZCAnc2VhcmNoLmNvbnRhY3QuZmFpbGVkJ1xuICAgICAqIEluIGJvdGggY2FzZXMgd2UgdXBkYXRlIHRoZSBvbi1jYWxsIFVJIHdpdGggc2VhcmNoIHJlc3VsdHMgKEVpdGhlciBjb250YWN0IG5hbWUgT1IgVW5rbm93bikuXG4gICAgICogQWxzbyB0aGUgYWN0dWFsIGNhbGwgaXMgdGlyZ2dlcmVkIGZyb20gaGVyZS5cbiAgICAgKlxuICAgICAqIEBwYXJhbSBzZWFyY2hSZXN1bHRcbiAgICAgKi9cbiAgICBwdWJsaWMgY29udGFjdFNlYXJjaEhhbmRsZXIgPSAoc2VhcmNoUmVzdWx0KSA9PiB7XG4gICAgICAgIHRoaXMuZGlhbGVkQ29udGFjdCA9IHNlYXJjaFJlc3VsdC5jb250YWN0O1xuICAgICAgICB0aGlzLmlzT3V0R29pbmcgPSB0cnVlO1xuICAgICAgICBDdGlWaWV3SGVscGVyLnVwZGF0ZU91dGdvaW5nQ29udGFjdERldGFpbHMoc2VhcmNoUmVzdWx0LmNvbnRhY3QubmFtZSwgc2VhcmNoUmVzdWx0LmNvbnRhY3QuZHApO1xuICAgICAgICB0aGlzLmN0aUFkYXB0ZXIuZGlhbEFOdW1iZXIoQ3RpVmlld0hlbHBlci5nZXRPdXRnb2luZ0NvbnRhY3ROdW1iZXIoKS50cmltKCkpO1xuICAgIH07XG5cbiAgICAvKipcbiAgICAgKlxuICAgICAqXG4gICAgICogQHBhcmFtIGV2ZW50XG4gICAgICovXG4gICAgcHVibGljIG91dGdvaW5nSGFuZ3VwSGFuZGxlciA9IChldmVudDphbnkpID0+IHtcbiAgICAgICAgQ3RpVmlld0hlbHBlci5yZW5kZXJPdXRnb2luZ0hhbmd1cFVJKGV2ZW50KTtcbiAgICAgICAgdGhpcy5vbkNhbGwgPSBmYWxzZTtcbiAgICAgICAgdGhpcy5pc05vdGlmeUFsbG93ZWQgPSB0cnVlO1xuICAgIH07XG5cbiAgICAvKipcbiAgICAgKiBoYW5kbGVMb2dpbiAtIEluaXRpYXRlIGEgbG9naW4gcmVxdWVzdFxuICAgICAqIHdpdGggdGhlIENUSSB0b29sLCB1c2luZyB0aGUgQ1RJQWRhcHRlciBpbnN0YW5jZVxuICAgICAqL1xuICAgIHB1YmxpYyBoYW5kbGVMb2dpbiA9ICgpOnZvaWQgPT4ge1xuICAgICAgICBDdGlMb2dnZXIubG9nSW5mb01lc3NhZ2UodGhpcy5sb2dQcmVNZXNzYWdlICsgQ3RpTWVzc2FnZXMuTUVTU0FHRV9JTklUSUFURV9MT0dJTik7XG4gICAgICAgIHRoaXMucmVnaXN0ZXJMaXN0ZW5lcnNUb0FkYXB0ZXIoKTtcbiAgICAgICAgaWYgKCF0aGlzLmlzTG9nZ2VkSW4gJiYgdGhpcy5nbG9iYWxIZWFkZXJNZW51KSB7XG4gICAgICAgICAgICB0aGlzLnVwZGF0ZUdsb2JhbEhlYWRlck1lbnVJY29uKEN0aUNvbnN0YW50cy5XQUlUKTtcbiAgICAgICAgICAgIHRoaXMuZ2xvYmFsQ29udGV4dC5nZXRTZXNzaW9uVG9rZW4oKS50aGVuKChzZXNzaW9uVG9rZW46c3RyaW5nKSA9PiB7XG4gICAgICAgICAgICAgICAgdmFyIGFnZW50UHJvZmlsZTpBZ2VudFByb2ZpbGUgPSA8QWdlbnRQcm9maWxlPiB7XG4gICAgICAgICAgICAgICAgICAgIGludGVyZmFjZVVybDogdGhpcy5nbG9iYWxDb250ZXh0LmdldEludGVyZmFjZVVybCgpLFxuICAgICAgICAgICAgICAgICAgICBhY2NvdW50SWQ6ICcnICsgdGhpcy5nbG9iYWxDb250ZXh0LmdldEFjY291bnRJZCgpLFxuICAgICAgICAgICAgICAgICAgICBzZXNzaW9uSWQ6IHNlc3Npb25Ub2tlblxuICAgICAgICAgICAgICAgIH07XG4gICAgICAgICAgICAgICAgdGhpcy5hZ2VudFByb2ZpbGUgPSBhZ2VudFByb2ZpbGU7XG4gICAgICAgICAgICAgICAgdGhpcy5jdGlBZGFwdGVyLmxvZ2luKGFnZW50UHJvZmlsZSk7XG4gICAgICAgICAgICB9KTtcbiAgICAgICAgfSBlbHNlIHtcbiAgICAgICAgICAgIEN0aUxvZ2dlci5sb2dXYXJuaW5nTWVzc2FnZSh0aGlzLmxvZ1ByZU1lc3NhZ2UgK1xuICAgICAgICAgICAgICAgIEN0aU1lc3NhZ2VzLk1FU1NBR0VfUEFSVElBTF9MT0dPVVQpO1xuICAgICAgICB9XG4gICAgfTtcblxuICAgIC8qKlxuICAgICAqIHVwZGF0ZUljb25zQWZ0ZXJMb2dpbiAtIFRha2UgY2FyZSBvZlxuICAgICAqIHJlbmRlcmluZyB0aGUgbGVmdHBhbmVsIGljb24sIHRvb2xiYXIgbWVudSBpY29uXG4gICAgICogYWZ0ZXIgYSBzdWNjZXNzZnVsIGxvZ2luIHdpdGggdGhlIENUSSBUb29sXG4gICAgICpcbiAgICAgKiBJdCBkb2VzIHRoZSBmb2xsb3dpbmcgb3BlcmF0aW9uc1xuICAgICAqXG4gICAgICogMS4gVXBkYXRlIHRoZSByaWJib25iYXIgbWVudSB3aXRoIHN0YXR1cyB1cGRhdGVcbiAgICAgKiAgICBhbmQgbG9nb3V0IG9wdGlvbnNcbiAgICAgKiAyLiBVcGRhdGUgdGhlIHJpYmJvbmJhciBpY29uIGFuZCBsYWJlbCBjb3JyZXNwb25kaW5nIHRvIHRoZVxuICAgICAqICAgIGRlZmF1bHQgc3RhdHVzIHByb3ZpZGVkIGluIHRoZSBDVEkgQ29uZmlndXJhdGlvblxuICAgICAqIDMuIFJlbmRlciB0aGUgbGVmdHBhbmVsIGljb24gd2l0aCB0aGUgZGVmYXVsdCBzdGF0dXNcbiAgICAgKlxuICAgICAqL1xuICAgIHB1YmxpYyBoYW5kbGVMb2dpblN1Y2Nlc3MgPSAoZGF0YTogYW55KTp2b2lkID0+IHtcbiAgICAgICAgaWYodGhpcy5pc0xvZ2dlZEluKXtcbiAgICAgICAgICAgIEN0aUxvZ2dlci5sb2dJbmZvTWVzc2FnZSh0aGlzLmxvZ1ByZU1lc3NhZ2UgK1xuICAgICAgICAgICAgICAgIEN0aU1lc3NhZ2VzLk1FU1NBR0VfQUxSRUFEWV9MT0dHRURfSU4pO1xuICAgICAgICAgICAgdGhpcy5jdGlBZGFwdGVyLnVwZGF0ZUFjdGl2aXR5KHRoaXMuY3RpUHJvdmlkZXJDb25maWcuZGVmYXVsdFN0YXR1cyk7XG4gICAgICAgICAgICByZXR1cm47XG4gICAgICAgIH1cbiAgICAgICAgQ3RpTG9nZ2VyLmxvZ0luZm9NZXNzYWdlKHRoaXMubG9nUHJlTWVzc2FnZSArXG4gICAgICAgICAgICBDdGlNZXNzYWdlcy5NRVNTQUdFX1VJX1VQREFURV9BRlRFUl9MT0dJTl9TVUNDRVNTKTtcbiAgICAgICAgdGhpcy5nbG9iYWxIZWFkZXJDb250ZXh0LmdldE1lbnUoQ3RpQ29uc3RhbnRzLkJVSV9DVElfUklCQk9OQkFSX01FTlVfSUQpXG4gICAgICAgICAgICAudGhlbigocmliYm9uQmFyTWVudTpJR2xvYmFsSGVhZGVyTWVudSkgPT4ge1xuICAgICAgICAgICAgICAgIHRoaXMuZ2xvYmFsSGVhZGVyTWVudSA9IHJpYmJvbkJhck1lbnU7XG4gICAgICAgICAgICAgICAgdGhpcy5tZW51SXRlbUxvZ2luLmRpc3Bvc2UoKTtcblxuICAgICAgICAgICAgICAgIC8vMS4gVXBkYXRlIFJpYmJvbmJhciBtZW51XG4gICAgICAgICAgICAgICAgdGhpcy51cGRhdGVSaWJib25iYXJNZW51QWZ0ZXJMb2dpbigpO1xuXG4gICAgICAgICAgICAgICAgLy8yLiBVcGRhdGUgbGVmdHBhbmVsIG1lbnVcbiAgICAgICAgICAgICAgICB0aGlzLnVwZGF0ZUxlZnRQYW5lbE1lbnVBZnRlckxvZ2luKCk7XG5cbiAgICAgICAgICAgICAgICB0aGlzLmlzTG9nZ2VkSW4gPSB0cnVlO1xuICAgICAgICAgICAgICAgIHRoaXMucHJldlN0YXR1cyA9IHRoaXMuY3RpUHJvdmlkZXJDb25maWcuZGVmYXVsdFN0YXR1cztcblxuICAgICAgICAgICAgfSk7XG4gICAgfTtcblxuICAgIC8qKlxuICAgICAqIFRoaXMgbWV0aG9kIHJlc2V0IHRoZSBsb2dpbiBpY29uLCBpZiBsb2dpbiBmYWlsc1xuICAgICAqL1xuICAgIHB1YmxpYyBoYW5kbGVMb2dpbkZhaWx1cmUgPSAoZGF0YTogYW55KTp2b2lkID0+IHtcbiAgICAgICAgaWYgKHRoaXMuZ2xvYmFsSGVhZGVyTWVudSkge1xuICAgICAgICAgICAgdmFyIGljb246SUlDb24gPSB0aGlzLmdsb2JhbEhlYWRlck1lbnUuY3JlYXRlSWNvbihDdGlDb25zdGFudHMuQlVJX0NUSV9SSUJCT05CQVJfSUNPTl9UWVBFKTtcbiAgICAgICAgICAgIGljb24uc2V0SWNvbkNsYXNzKEN0aUNvbnN0YW50cy5CVUlfQ1RJX1JJQkJPTkJBUl9JQ09OX0RFRkFVTFRfQ0xBU1MpO1xuICAgICAgICAgICAgaWNvbi5zZXRJY29uQ29sb3IoQ3RpQ29uc3RhbnRzLkJVSV9DVElfUklCQk9OQkFSX0lDT05fREVGQVVMVF9DT0xPUik7XG4gICAgICAgICAgICB0aGlzLmdsb2JhbEhlYWRlck1lbnUuYWRkSWNvbihpY29uKTtcbiAgICAgICAgICAgIHRoaXMuZ2xvYmFsSGVhZGVyTWVudS5zZXREaXNhYmxlZChmYWxzZSk7XG4gICAgICAgICAgICB0aGlzLmdsb2JhbEhlYWRlck1lbnUuc2V0TGFiZWwodGhpcy5jdGlQcm92aWRlckNvbmZpZy5wcm92aWRlck5hbWUgKyAnICcgKyBDdGlDb25zdGFudHMuQlVJX0NUSV9MQUJFTF9MT0dHRURfT1VUKTtcbiAgICAgICAgICAgIHRoaXMuZ2xvYmFsSGVhZGVyTWVudS5yZW5kZXIoKTtcbiAgICAgICAgfSBlbHNlIHtcbiAgICAgICAgICAgIHRoaXMuaW5pdGlhbGl6ZUN0aVRvb2xCYXJNZW51KCk7XG4gICAgICAgIH1cbiAgICB9O1xuXG4gICAgLyoqXG4gICAgICogVXBkYXRlcyB0aGUgVUkgYWNjb3JkaW5nIHRvIHRoZSBzdGF0dXMgdXBkYXRlcyBmcm9tIENUSSBQbGF0Zm9ybVxuICAgICAqXG4gICAgICogQHBhcmFtIGN0aVVwZGF0ZURhdGFcbiAgICAgKi9cbiAgICBwdWJsaWMgaGFuZGxlU3RhdHVzVXBkYXRlc0Zyb21TZXJ2ZXIgPSAoY3RpVXBkYXRlRGF0YTphbnkpID0+IHtcbiAgICAgICAgQ3RpTG9nZ2VyLmxvZ0luZm9NZXNzYWdlKHRoaXMubG9nUHJlTWVzc2FnZSArXG4gICAgICAgICAgICBDdGlNZXNzYWdlcy5NRVNTQUdFX0NMSUVOVF9TVEFUVVMrXG4gICAgICAgICAgICBDdGlNZXNzYWdlcy5NRVNTQUdFX0FQUEVOREVSICsgdGhpcy5wcmV2U3RhdHVzKTtcbiAgICAgICAgQ3RpTG9nZ2VyLmxvZ0luZm9NZXNzYWdlKHRoaXMubG9nUHJlTWVzc2FnZSArXG4gICAgICAgICAgICBDdGlNZXNzYWdlcy5NRVNTQUdFX1NFUlZFUl9TVEFUVVMgK1xuICAgICAgICAgICAgQ3RpTWVzc2FnZXMuTUVTU0FHRV9BUFBFTkRFUiArIGN0aVVwZGF0ZURhdGEpO1xuICAgICAgICBpZiAodGhpcy5wcmV2U3RhdHVzICE9PSBjdGlVcGRhdGVEYXRhKSB7XG4gICAgICAgICAgICB0aGlzLnVwZGF0ZUFnZW50VUlPblNlcnZlclVwZGF0ZShjdGlVcGRhdGVEYXRhKTtcbiAgICAgICAgICAgIHRoaXMucHJldlN0YXR1cyA9IHN0YXR1cztcbiAgICAgICAgfVxuICAgIH07XG5cbiAgICAvKipcbiAgICAgKiBIYW5kbGVyIGZvciBldmVudCAtICdpbmNvbWluZydcbiAgICAgKlxuICAgICAqIFRoaXMgaXMgdGhlIGhhbmRsZSBmb3IgJ2luY29taW5nJyBldmVudC4gVGhpcyBoYW5kbGVyIHdpbGwgYmUgaW52b2tlZCBieSB0aGVcbiAgICAgKiBDVEkgQWRhcHRlciB3aGVuIHRoZXJlIGlzIGFuIGluY29taW5nIGNhbGwgZm9yIHRoZSBhZ2VudFxuICAgICAqXG4gICAgICogQHBhcmFtIGN0aURhdGFcbiAgICAgKi9cbiAgICBwdWJsaWMgaGFuZGxlSW5jb21pbmcgPSAoY3RpRGF0YTphbnkpOnZvaWQgPT4ge1xuICAgICAgICBDdGlMb2dnZXIubG9nSW5mb01lc3NhZ2UodGhpcy5sb2dQcmVNZXNzYWdlICtcbiAgICAgICAgICAgIEN0aU1lc3NhZ2VzLk1FU1NBR0VfSEFORExFX0NBTExfSU5DT01JTkcpO1xuICAgICAgICBpZiAoIXRoaXMub25DYWxsICYmIHRoaXMuaXNOb3RpZnlBbGxvd2VkKSB7XG4gICAgICAgICAgICB0aGlzLmlzTm90aWZ5QWxsb3dlZCA9IGZhbHNlO1xuICAgICAgICAgICAgaWYoIWN0aURhdGEuY29udGFjdC5lbWFpbCl7XG4gICAgICAgICAgICAgICAgY3RpRGF0YS5jb250YWN0LmVtYWlsID0gQ3RpTWVzc2FnZXMuTUVTU0FHRV9NQUlMX05PVF9BVkFJTEFCTEU7XG4gICAgICAgICAgICB9XG4gICAgICAgICAgICBDdGlWaWV3SGVscGVyLnJlbmRlckluY29taW5nVmlldyhjdGlEYXRhLCB0aGlzLnJpbmdNZWRpYSk7XG5cbiAgICAgICAgICAgIHRoaXMuYnVpQ3RpTGVmdFBhbmVsTWVudS5zZXRMYWJlbChDdGlDb25zdGFudHMuQlVJX0NUSV9MQUJFTF9JTkNPTUlOR19DQUxMKTtcbiAgICAgICAgICAgIHZhciBpY29uID0gdGhpcy5idWlDdGlMZWZ0UGFuZWxNZW51LmNyZWF0ZUljb24oQ3RpQ29uc3RhbnRzLkJVSV9DVElfTEVGVF9QQU5FTF9JQ09OX1RZUEUpO1xuICAgICAgICAgICAgaWNvbi5zZXRJY29uQ2xhc3MoQ3RpQ29uc3RhbnRzLkJVSV9DVElfTEVGVF9QQU5FTF9JQ09OX05PVElGWSk7XG4gICAgICAgICAgICBpY29uLnNldEljb25Db2xvcigncmVkJyk7XG5cbiAgICAgICAgICAgIHRoaXMuYnVpQ3RpTGVmdFBhbmVsTWVudS5hZGRJY29uKGljb24pO1xuICAgICAgICAgICAgdGhpcy5idWlDdGlMZWZ0UGFuZWxNZW51LmV4cGFuZCgpO1xuICAgICAgICAgICAgdGhpcy5idWlDdGlMZWZ0UGFuZWxNZW51LnJlbmRlcigpO1xuXG4gICAgICAgICAgICBzZXRUaW1lb3V0KCgpID0+IHtcbiAgICAgICAgICAgICAgICBpZiAoIXRoaXMub25DYWxsKSB7XG4gICAgICAgICAgICAgICAgICAgIHRoaXMuYnVpQ3RpTGVmdFBhbmVsTWVudS5zZXRMYWJlbCh0aGlzLmN0aVByb3ZpZGVyQ29uZmlnLnByb3ZpZGVyTmFtZSArICcgJyArIEN0aUNvbnN0YW50cy5CVUlfQ1RJX0xBQkVMX0FWQUlMQUJMRSk7XG4gICAgICAgICAgICAgICAgICAgIHZhciBpY29uID0gdGhpcy5idWlDdGlMZWZ0UGFuZWxNZW51LmNyZWF0ZUljb24oQ3RpQ29uc3RhbnRzLkJVSV9DVElfTEVGVF9QQU5FTF9JQ09OX1RZUEUpO1xuICAgICAgICAgICAgICAgICAgICBpY29uLnNldEljb25DbGFzcyhDdGlDb25zdGFudHMuQlVJX0NUSV9MRUZUX1BBTkVMX0lDT04pO1xuICAgICAgICAgICAgICAgICAgICBpY29uLnNldEljb25Db2xvcignZ3JlZW4nKTtcblxuICAgICAgICAgICAgICAgICAgICB0aGlzLmJ1aUN0aUxlZnRQYW5lbE1lbnUuYWRkSWNvbihpY29uKTtcbiAgICAgICAgICAgICAgICAgICAgdGhpcy5idWlDdGlMZWZ0UGFuZWxNZW51LnJlbmRlcigpO1xuICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgIH0sIDQwMDApO1xuICAgICAgICB9XG4gICAgfTtcblxuICAgIC8qKlxuICAgICAqIEhhbmRsZXIgZm9yIGV2ZW50IC0gJ2Nvbm5lY3RlZCdcbiAgICAgKlxuICAgICAqIFRoaXMgaXMgdGhlIGhhbmRsZXIgZm9yICdjb25uZWN0ZWQnIGV2ZW50LiBUaGlzIGhhbmRsZXIgd2lsbCBiZSBpbnZva2VkXG4gICAgICogYnkgdGhlIENUSUFkYXB0ZXIgYWZ0ZXIgdGhlIEFnZW50IGFjY2VwdHMgYW4gaW5jb21pbmcgY2FsbC5cbiAgICAgKlxuICAgICAqIEBwYXJhbSBjdGlEYXRhXG4gICAgICovXG4gICAgcHVibGljIGhhbmRsZUNhbGxDb25uZWN0ZWQgPSAoY3RpRGF0YTphbnkpOnZvaWQgPT4ge1xuICAgICAgICBDdGlMb2dnZXIubG9nSW5mb01lc3NhZ2UodGhpcy5sb2dQcmVNZXNzYWdlICtcbiAgICAgICAgICAgIEN0aU1lc3NhZ2VzLk1FU1NBR0VfSEFORExFX0NBTExfQ09OTkVDVEVEKTtcbiAgICAgICAgdGhpcy5pc0NhbGxTdW1tYXJpemVkID0gZmFsc2U7XG4gICAgICAgIC8vdmFyIGlzT3V0Ym91bmQ6IGJvb2xlYW4gPSB0aGlzLmlzT3V0R29pbmc7XG4gICAgICAgIGlmICh0aGlzLmlzT3V0R29pbmcpIHtcbiAgICAgICAgICAgIGlmICh0aGlzLmRpYWxlZENvbnRhY3QpIHtcbiAgICAgICAgICAgICAgICBjdGlEYXRhLmNvbnRhY3QgPSB0aGlzLmRpYWxlZENvbnRhY3Q7XG4gICAgICAgICAgICAgICAgdGhpcy5kaWFsZWRDb250YWN0ID0gbnVsbDtcbiAgICAgICAgICAgIH1cbiAgICAgICAgICAgIHRoaXMuaXNPdXRHb2luZyA9IGZhbHNlO1xuICAgICAgICB9ZWxzZXtcbiAgICAgICAgICAgIHRoaXMubG9nQ2FsbEFjdGlvbihDdGlNZXNzYWdlcy5NRVNTQUdFX0NBTExfQUNDRVBURURfQllfQUdFTlQgKyB0aGlzLmFnZW50UHJvZmlsZS5hY2NvdW50SWQpO1xuICAgICAgICB9XG4gICAgICAgIHRoaXMub25DYWxsID0gdHJ1ZTtcbiAgICAgICAgdGhpcy5vcGVuSW50ZXJhY3Rpb25Xb3Jrc3BhY2UoY3RpRGF0YSk7XG4gICAgICAgIHRoaXMuY3RpSW5DYWxsQ2xvY2sgPSBuZXcgQ3RpQ2xvY2soJ2NhbGxfaW5fY2xvY2snKTtcbiAgICAgICAgdGhpcy5jYWxsZXJEYXRhID0gY3RpRGF0YS5jb250YWN0O1xuICAgICAgICBpZighY3RpRGF0YS5jb250YWN0LmVtYWlsKXtcbiAgICAgICAgICAgIGN0aURhdGEuY29udGFjdC5lbWFpbCA9IEN0aU1lc3NhZ2VzLk1FU1NBR0VfTUFJTF9OT1RfQVZBSUxBQkxFO1xuICAgICAgICB9XG4gICAgICAgIEN0aVZpZXdIZWxwZXIucmVuZGVyT25DYWxsVmlldyhjdGlEYXRhLCB0aGlzLnJpbmdNZWRpYSwgdGhpcy5zZWFyY2hBdmFpbGFibGVBZ2VudHMpO1xuICAgICAgICB0aGlzLmN0aUluQ2FsbENsb2NrLnN0YXJ0Q2xvY2soKTtcbiAgICB9O1xuXG4gICAgLyoqXG4gICAgICpcbiAgICAgKiBAcGFyYW0gY29ubmVjdGVkRGF0YVxuICAgICAqVGhpcyBtZXRob2Qgb3BlbnNcbiAgICAgKlxuICAgICAqIDEuIEFuIGludGVyYWN0aW9uIHdvcmtzcGFjZSBmb3IgdGhlIGFjY2VwdGVkIGNhbGxcbiAgICAgKiAyLiBTZXQgdGhlIGNvbnRhY3QgYW5kIGluY2lkZW50IGluZm9ybWF0aW9uIHRvIHRoZSBpbnRlcmFjdGlvbiB3b3Jrc3BhY2VcbiAgICAgKiAzLiBPcGVuIHRoZSBhc3NvY2lhdGVkIGluY2lkZW50IHdpdGggdGhlIGludGVyYWN0aW9uLCBpZiBhbnlcbiAgICAgKlxuICAgICAqL1xuICAgIHB1YmxpYyBvcGVuSW50ZXJhY3Rpb25Xb3Jrc3BhY2UoY29ubmVjdGVkRGF0YTphbnkpOnZvaWQge1xuICAgICAgICBDdGlMb2dnZXIubG9nSW5mb01lc3NhZ2UodGhpcy5sb2dQcmVNZXNzYWdlICsgQ3RpTWVzc2FnZXMuTUVTU0FHRV9PUEVOX0lOVEVSQUNUSU9OX1dPUktTUEFDRSk7XG4gICAgICAgIHRoaXMuaW5jaWRlbnRXb3Jrc3BhY2UgPSBudWxsO1xuICAgICAgICB0aGlzLmV4dGVuc2lvblNkay5yZWdpc3RlcldvcmtzcGFjZUV4dGVuc2lvbihcbiAgICAgICAgICAgICh3b3Jrc3BhY2VSZWNvcmQpID0+IHtcbiAgICAgICAgICAgICAgICB0aGlzLndvcmtzcGFjZVJlY29yZCA9IHdvcmtzcGFjZVJlY29yZDtcbiAgICAgICAgICAgICAgICB3b3Jrc3BhY2VSZWNvcmQuY3JlYXRlV29ya3NwYWNlUmVjb3JkKCdJbnRlcmFjdGlvbicsXG4gICAgICAgICAgICAgICAgICAgIChpbnRlcmFjdGlvbk9iamVjdCkgPT4ge1xuICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5pbnRlcmFjdGlvbldvcmtzcGFjZSA9IGludGVyYWN0aW9uT2JqZWN0O1xuICAgICAgICAgICAgICAgICAgICAgICAgaWYgKGNvbm5lY3RlZERhdGEuY29udGFjdC5pZCkge1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGludGVyYWN0aW9uT2JqZWN0LnVwZGF0ZUZpZWxkKCdJbnRlcmFjdGlvbi5DSWQnLCBjb25uZWN0ZWREYXRhLmNvbnRhY3QuaWQpO1xuICAgICAgICAgICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgICAgICAgICAgaWYgKGNvbm5lY3RlZERhdGEuY29udGFjdC5maXJzdE5hbWUpIHtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBpbnRlcmFjdGlvbk9iamVjdC51cGRhdGVGaWVsZCgnSW50ZXJhY3Rpb24uTmFtZS5GaXJzdCcsIGNvbm5lY3RlZERhdGEuY29udGFjdC5maXJzdE5hbWUpO1xuICAgICAgICAgICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgICAgICAgICAgaWYgKGNvbm5lY3RlZERhdGEuY29udGFjdC5sYXN0TmFtZSkge1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGludGVyYWN0aW9uT2JqZWN0LnVwZGF0ZUZpZWxkKCdJbnRlcmFjdGlvbi5OYW1lLkxhc3QnLCBjb25uZWN0ZWREYXRhLmNvbnRhY3QubGFzdE5hbWUpO1xuICAgICAgICAgICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgICAgICAgICAgaWYgKGNvbm5lY3RlZERhdGEuY29udGFjdC5lbWFpbCkge1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGludGVyYWN0aW9uT2JqZWN0LnVwZGF0ZUZpZWxkKCdJbnRlcmFjdGlvbi5FbWFpbC5BZGRyJywgY29ubmVjdGVkRGF0YS5jb250YWN0LmVtYWlsKTtcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cblxuICAgICAgICAgICAgICAgICAgICAgICAgaW50ZXJhY3Rpb25PYmplY3QuYWRkRmllbGRWYWx1ZUxpc3RlbmVyKCdJbnRlcmFjdGlvbi5JSWQnLCB0aGlzLmluY2lkZW50VXBkYXRlSGFuZGxlcik7XG4gICAgICAgICAgICAgICAgICAgICAgICBpZiAoY29ubmVjdGVkRGF0YSAmJiBjb25uZWN0ZWREYXRhLmluY2lkZW50KSB7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgaW50ZXJhY3Rpb25PYmplY3QudXBkYXRlRmllbGQoJ0ludGVyYWN0aW9uLklJZCcsIGNvbm5lY3RlZERhdGEuaW5jaWRlbnQpO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHdvcmtzcGFjZVJlY29yZC5lZGl0V29ya3NwYWNlUmVjb3JkKCdJbmNpZGVudCcsIGNvbm5lY3RlZERhdGEuaW5jaWRlbnQsIChpbmNpZGVudFdvcmtzcGFjZSkgPT4ge1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB0aGlzLmluY2lkZW50V29ya3NwYWNlID0gPElJbmNpZGVudFdvcmtzcGFjZVJlY29yZD5pbmNpZGVudFdvcmtzcGFjZTtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgc2V0VGltZW91dCgoKSA9PiB7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBDdGlWaWV3SGVscGVyLmVuYWJsZU9uQ2FsbENvbnRyb2xzKCk7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIH0sIDEwMDApO1xuXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgfSk7XG4gICAgICAgICAgICAgICAgICAgICAgICB9IGVsc2Uge1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHNldFRpbWVvdXQoKCkgPT4ge1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBDdGlWaWV3SGVscGVyLmVuYWJsZU9uQ2FsbENvbnRyb2xzKCk7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgfSwgMTAwMCk7XG4gICAgICAgICAgICAgICAgICAgICAgICB9XG5cbiAgICAgICAgICAgICAgICAgICAgfSk7XG4gICAgICAgICAgICB9KTtcbiAgICB9XG5cbiAgICAvKipcbiAgICAgKiBUaGlzIGZ1bmN0aW9uIHVwZGF0ZXMgdGhlIEluY2lkZW50V29ya3NwYWNlUmVjb3JkIG9iamVjdCB3aGVuIGFnZW50IGNoYW5nZXMgdGhlXG4gICAgICogaW5jaWRlbnQgYXNzb2NpYXRlZCB3aXRoIHRoZSBJbnRlcmFjdGlvbldvcmtzcGFjZVxuICAgICAqXG4gICAgICogQHBhcmFtIGZpZWxkVmFsdWVDaGFuZ2VEYXRhXG4gICAgICovXG4gICAgcHJpdmF0ZSBpbmNpZGVudFVwZGF0ZUhhbmRsZXIgPSAoZmllbGRWYWx1ZUNoYW5nZURhdGE6T1JBQ0xFX1NFUlZJQ0VfQ0xPVUQuSVdvcmtzcGFjZVJlY29yZEV2ZW50UGFyYW1ldGVyKSA9PiB7XG4gICAgICAgIGlmIChmaWVsZFZhbHVlQ2hhbmdlRGF0YSAmJiBmaWVsZFZhbHVlQ2hhbmdlRGF0YS5ldmVudCAmJiBmaWVsZFZhbHVlQ2hhbmdlRGF0YS5ldmVudC52YWx1ZSkge1xuICAgICAgICAgICAgaWYgKHBhcnNlSW50KGZpZWxkVmFsdWVDaGFuZ2VEYXRhLmV2ZW50LnZhbHVlLCAxMCkgPCAwKSB7XG4gICAgICAgICAgICAgICAgc2V0VGltZW91dCgoKSA9PiB7XG4gICAgICAgICAgICAgICAgICAgIHRoaXMuZXh0ZW5zaW9uU2RrLnJlZ2lzdGVyV29ya3NwYWNlRXh0ZW5zaW9uKCh3b3Jrc3BhY2VSZWNvcmQ6T1JBQ0xFX1NFUlZJQ0VfQ0xPVUQuSVdvcmtzcGFjZVJlY29yZCkgPT4ge1xuICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5pbmNpZGVudFdvcmtzcGFjZSA9IDxPUkFDTEVfU0VSVklDRV9DTE9VRC5JSW5jaWRlbnRXb3Jrc3BhY2VSZWNvcmQ+d29ya3NwYWNlUmVjb3JkO1xuICAgICAgICAgICAgICAgICAgICB9LCAnSW5jaWRlbnQnLCBwYXJzZUludChmaWVsZFZhbHVlQ2hhbmdlRGF0YS5ldmVudC52YWx1ZSwgMTApKTtcbiAgICAgICAgICAgICAgICB9LCAxMDAwKTtcbiAgICAgICAgICAgIH0gZWxzZSB7XG4gICAgICAgICAgICAgICAgdGhpcy53b3Jrc3BhY2VSZWNvcmQuZWRpdFdvcmtzcGFjZVJlY29yZCgnSW5jaWRlbnQnLCBwYXJzZUludChcbiAgICAgICAgICAgICAgICAgICAgZmllbGRWYWx1ZUNoYW5nZURhdGEuZXZlbnQudmFsdWUsIDEwKSwgKGluY2lkZW50V29ya3NwYWNlUmVjb3JkOklJbmNpZGVudFdvcmtzcGFjZVJlY29yZCkgPT4ge1xuICAgICAgICAgICAgICAgICAgICB0aGlzLmluY2lkZW50V29ya3NwYWNlID0gaW5jaWRlbnRXb3Jrc3BhY2VSZWNvcmQ7XG4gICAgICAgICAgICAgICAgfSlcbiAgICAgICAgICAgIH1cbiAgICAgICAgfSBlbHNlIHtcbiAgICAgICAgICAgIC8vVmFsdWUgY2xlYXJlZFxuICAgICAgICAgICAgdGhpcy5pbmNpZGVudFdvcmtzcGFjZSA9IG51bGw7XG4gICAgICAgIH1cbiAgICB9O1xuXG4gICAgLyoqXG4gICAgICogSGFuZGxlciBmb3IgZXZlbnQgLSAnZGlzY29ubmVjdGVkJ1xuICAgICAqXG4gICAgICogVGhpcyBpcyB0aGUgaGFuZGxlciBmb3IgJ2Rpc2Nvbm5lY3RlZCcgZXZlbnQuIFRoaXMgaGFuZGxlciB3aWxsIGJlXG4gICAgICogaW52b2tlZCBieSB0aGUgQ1RJIEFkYXB0ZXIgd2hlbiBhZ2VudCBoYW5nLXVwIHRoZSBjYWxsXG4gICAgICovXG4gICAgcHVibGljIGhhbmRsZUNhbGxEaXNjb25uZWN0ZWQgPSAoZGF0YTogYW55KTp2b2lkID0+IHtcbiAgICAgICAgQ3RpTG9nZ2VyLmxvZ0luZm9NZXNzYWdlKHRoaXMubG9nUHJlTWVzc2FnZSArXG4gICAgICAgICAgICBDdGlNZXNzYWdlcy5NRVNTQUdFX0hBTkRMRV9DQUxMX0RJU0NPTk5FQ1QpO1xuICAgICAgICB0aGlzLm9uQ2FsbFRyYW5zZmVyID0gIGZhbHNlO1xuICAgICAgICB0aGlzLm9uQ2FsbCA9IGZhbHNlO1xuICAgICAgICB0aGlzLmlzTm90aWZ5QWxsb3dlZCA9IHRydWU7XG4gICAgICAgIHRoaXMuY3RpSW5DYWxsQ2xvY2suc3RvcENsb2NrKCk7XG4gICAgICAgIHRoaXMuY3RpSW5DYWxsQ2xvY2sucmVzZXRVSSgpO1xuICAgICAgICBDdGlWaWV3SGVscGVyLnJlbmRlckNhbGxEaXNjb25uZWN0VmlldygpO1xuICAgICAgICB0aGlzLnN1bW1hcml6ZUNhbGwoZmFsc2UsIG51bGwpO1xuICAgIH07XG5cbiAgICAvKipcbiAgICAgKiBIYW5kbGVyIGZvciBldmVudCAtICdjYW5jZWxsZWQnXG4gICAgICpcbiAgICAgKiBUaGlzIGlzIHRoZSBoYW5kbGVyIGZvciAnY2FuY2VsZWQnIGV2ZW50LiBUaGlzIGhhbmRsZXIgd2lsbCBiZSBpbnZva2VkIGJ5XG4gICAgICogdGhlIENUSUFkYXB0ZXIgd2hlbiB0aGUgY2FsbCBpcyByZWplY3RlZCBieSB0aGUgYWdlbnRcbiAgICAgKlxuICAgICAqIEBwYXJhbSBkYXRhXG4gICAgICovXG4gICAgcHVibGljIGhhbmRsZUNhbGxDYW5jZWxsZWQgPSAoZGF0YTogYW55KTp2b2lkID0+IHtcbiAgICAgICAgQ3RpTG9nZ2VyLmxvZ0luZm9NZXNzYWdlKHRoaXMubG9nUHJlTWVzc2FnZSArXG4gICAgICAgICAgICBDdGlNZXNzYWdlcy5NRVNTQUdFX0hBTkRMRV9DQUxMX0NBTkNFTCk7XG4gICAgICAgIHRoaXMuaXNOb3RpZnlBbGxvd2VkID0gdHJ1ZTtcbiAgICAgICAgdGhpcy5vbkNhbGwgPSBmYWxzZTtcbiAgICAgICAgdGhpcy5yaW5nTWVkaWEucGF1c2UoKTtcbiAgICAgICAgdGhpcy5yaW5nTWVkaWEuY3VycmVudFRpbWUgPSAwO1xuICAgICAgICB0aGlzLmxvZ0NhbGxBY3Rpb24oQ3RpTWVzc2FnZXMuTUVTU0FHRV9DQUxMX1JFSkVDVEVEX0JZX0FHRU5UICsgdGhpcy5hZ2VudFByb2ZpbGUuYWNjb3VudElkKTtcbiAgICAgICAgQ3RpVmlld0hlbHBlci5yZW5kZXJDYWxsQ2FuY2VsbGVkVmlldygpO1xuICAgIH07XG5cbiAgICAvKipcbiAgICAgKiBIYW5kbGVyIGZvciBldmVudCAtICd0aW1lb3V0J1xuICAgICAqXG4gICAgICogVGhpcyBpcyB0aGUgaGFuZGxlciBmb3IgJ3RpbWVvdXQnIGV2ZW50LlxuICAgICAqIFRoaXMgaGFuZGxlciB3aWxsIGJlIGludm9rZWQgYnkgdGhlIENUSUFkYXB0ZXJcbiAgICAgKiB3aGVuIGFuIGluY29taW5nIGNhbGwgdGltZXMgb3V0XG4gICAgICpcbiAgICAgKiBAcGFyYW0gZGF0YVxuICAgICAqL1xuICAgIHB1YmxpYyBoYW5kbGVDYWxsVGltZU91dCA9IChkYXRhOmFueSk6dm9pZCA9PiB7XG4gICAgICAgIEN0aUxvZ2dlci5sb2dJbmZvTWVzc2FnZSh0aGlzLmxvZ1ByZU1lc3NhZ2UgK1xuICAgICAgICAgICAgQ3RpTWVzc2FnZXMuTUVTU0FHRV9IQU5ETEVfQ0FMTF9USU1FT1VUKTtcbiAgICAgICAgdGhpcy5pc05vdGlmeUFsbG93ZWQgPSB0cnVlO1xuICAgICAgICB0aGlzLnJpbmdNZWRpYS5wYXVzZSgpO1xuICAgICAgICB0aGlzLnJpbmdNZWRpYS5jdXJyZW50VGltZSA9IDA7XG4gICAgICAgIEN0aVZpZXdIZWxwZXIucmVuZGVyQ2FsbFRpbWVPdXRWaWV3KCk7XG4gICAgfTtcblxuXG4gICAgLyoqXG4gICAgICogaW5pdGlhdGVzIGEgc2VhcmNoIGZvciBhdmFpbGFibGUgYWdlbnRzLlxuICAgICAqL1xuICAgIHB1YmxpYyBzZWFyY2hBdmFpbGFibGVBZ2VudHMgPSAoKSA9PiB7XG4gICAgICAgIEN0aUxvZ2dlci5sb2dJbmZvTWVzc2FnZSh0aGlzLmxvZ1ByZU1lc3NhZ2UgK1xuICAgICAgICAgICAgQ3RpTWVzc2FnZXMuTUVTU0FHRV9JTklUSUFURV9BR0VOVF9TRUFSQ0gpO1xuICAgICAgICBDdGlWaWV3SGVscGVyLnJlbmRlckFnZW50U2VhcmNoVUkoKTtcbiAgICAgICAgdGhpcy5nbG9iYWxDb250ZXh0LmdldFNlc3Npb25Ub2tlbigpLnRoZW4oKHNlc3Npb25Ub2tlbjpzdHJpbmcpID0+IHtcbiAgICAgICAgICAgIHRoaXMuY3RpQWRhcHRlci5zZWFyY2hBdmFpbGFibGVBZ2VudHMoc2Vzc2lvblRva2VuKTtcbiAgICAgICAgfSk7XG4gICAgfTtcblxuICAgIC8qKlxuICAgICAqIEhhbmRsZXIgZm9yIGV2ZW50IC0gJ3NlYXJjaC5hZ2VudGxpc3QuY29tcGxldGUnXG4gICAgICpcbiAgICAgKiBAcGFyYW0gYXZhaWxhYmxlQWdlbnRzXG4gICAgICovXG4gICAgcHVibGljIGhhbmRsZUFnZW50U2VhcmNoU3VjY2VzcyA9IChhdmFpbGFibGVBZ2VudHM6QWdlbnREYXRhW10pID0+IHtcbiAgICAgICAgQ3RpTG9nZ2VyLmxvZ0luZm9NZXNzYWdlKHRoaXMubG9nUHJlTWVzc2FnZSArXG4gICAgICAgICAgICBDdGlNZXNzYWdlcy5NRVNTQUdFX0hBTkRMRV9BR0VOVF9TRUFSQ0hfQ09NUExFVElPTik7XG4gICAgICAgIEN0aVZpZXdIZWxwZXIucmVuZGVyQWdlbnRMaXN0KGF2YWlsYWJsZUFnZW50cywgdGhpcy50cmFuc2ZlckNhbGxIYW5kbGVyKTtcbiAgICB9O1xuXG4gICAgLyoqXG4gICAgICogSW5pdGlhdGVzIGEgdHJhbnNmZXIgY2FsbCByZXF1ZXN0LiBUaGlzIHdpbGwgYmUgaW52b2tlZCB3aGVuXG4gICAgICogYW4gYWdlbnQgY2xpY2tzIG9uIHRoZSB0cmFuc2ZlciBjYWxsIGJ1dHRvbi5cbiAgICAgKlxuICAgICAqIEBwYXJhbSB3b3JrZXJJZFxuICAgICAqIEBwYXJhbSBhZ2VudE5hbWVcbiAgICAgKi9cbiAgICBwdWJsaWMgdHJhbnNmZXJDYWxsSGFuZGxlciA9ICh3b3JrZXJJZDogc3RyaW5nLCBhZ2VudE5hbWU6IHN0cmluZykgPT4ge1xuICAgICAgICBpZih0aGlzLm9uQ2FsbFRyYW5zZmVyKSB7XG4gICAgICAgICAgICBDdGlMb2dnZXIubG9nSW5mb01lc3NhZ2UodGhpcy5sb2dQcmVNZXNzYWdlICtcbiAgICAgICAgICAgICAgICBDdGlNZXNzYWdlcy5NRVNTQUdFX1dBSVRfV0hJTEVfVFJBTlNGRVIpO1xuICAgICAgICAgICAgcmV0dXJuO1xuICAgICAgICB9XG5cbiAgICAgICAgQ3RpTG9nZ2VyLmxvZ0luZm9NZXNzYWdlKHRoaXMubG9nUHJlTWVzc2FnZSArXG4gICAgICAgICAgICBDdGlNZXNzYWdlcy5NRVNTQUdFX0lOSVRJQVRFX1RSQU5TRkVSKTtcbiAgICAgICAgdGhpcy5vbkNhbGxUcmFuc2ZlciA9IHRydWU7XG4gICAgICAgIEN0aVZpZXdIZWxwZXIuZGlzYWJsZU9uQ2FsbENvbnRyb2xzKCk7XG5cbiAgICAgICAgaWYodGhpcy5pbmNpZGVudFdvcmtzcGFjZSl7XG4gICAgICAgICAgICAvL1RyYW5zZmVyIHRoZSBjYWxsIG9ubHkgYWZ0ZXIgc2F2ZVxuICAgICAgICAgICAgdGhpcy5pbmNpZGVudFdvcmtzcGFjZS5hZGRSZWNvcmRDbG9zaW5nTGlzdGVuZXIoIChldmVudERhdGE6IE9SQUNMRV9TRVJWSUNFX0NMT1VELklXb3Jrc3BhY2VSZWNvcmRFdmVudFBhcmFtZXRlcikgPT4ge1xuICAgICAgICAgICAgICAgIC8vQ3RpVmlld0hlbHBlci5kaXNhYmxlT25DYWxsQ29udHJvbHMoKTtcbiAgICAgICAgICAgICAgICB2YXIgaW5jaWRlbnRJZDogbnVtYmVyID0gZXZlbnREYXRhLmdldFdvcmtzcGFjZVJlY29yZCgpLmdldFdvcmtzcGFjZVJlY29yZElkKCk7XG4gICAgICAgICAgICAgICAgdGhpcy5nbG9iYWxDb250ZXh0LmdldFNlc3Npb25Ub2tlbigpLnRoZW4oKHNlc3Npb25Ub2tlbjpzdHJpbmcpID0+IHtcbiAgICAgICAgICAgICAgICAgICAgdGhpcy5jdGlJbkNhbGxDbG9jay5zdG9wQ2xvY2soKTtcbiAgICAgICAgICAgICAgICAgICAgdGhpcy5jdGlBZGFwdGVyLnRyYW5zZmVyQ2FsbChzZXNzaW9uVG9rZW4sIHdvcmtlcklkLCBpbmNpZGVudElkKTtcbiAgICAgICAgICAgICAgICAgICAgdGhpcy5pbmNpZGVudFdvcmtzcGFjZSA9IG51bGw7XG4gICAgICAgICAgICAgICAgfSk7XG4gICAgICAgICAgICB9KTtcbiAgICAgICAgfWVsc2V7XG4gICAgICAgICAgICB0aGlzLmdsb2JhbENvbnRleHQuZ2V0U2Vzc2lvblRva2VuKCkudGhlbigoc2Vzc2lvblRva2VuOnN0cmluZykgPT4ge1xuICAgICAgICAgICAgICAgIHRoaXMuY3RpQWRhcHRlci50cmFuc2ZlckNhbGwoc2Vzc2lvblRva2VuLCB3b3JrZXJJZCk7XG4gICAgICAgICAgICB9KTtcbiAgICAgICAgfVxuXG4gICAgICAgIHRoaXMuc3VtbWFyaXplQ2FsbCh0cnVlLCBhZ2VudE5hbWUpLmNhdGNoKCgpPT57XG4gICAgICAgICAgICAvL1JlLUVuYWJsZSB0aGUgY29udHJvbHMsIGlmIHNhdmluZyB3b3Jrc3BhY2UgZmFpbHNcbiAgICAgICAgICAgIEN0aVZpZXdIZWxwZXIuZW5hYmxlT25DYWxsQ29udHJvbHMoKTtcbiAgICAgICAgICAgIHRoaXMub25DYWxsVHJhbnNmZXIgPSBmYWxzZTtcbiAgICAgICAgICAgIHRoaXMuaXNDYWxsU3VtbWFyaXplZCA9IGZhbHNlO1xuICAgICAgICB9KTtcbiAgICAgICAgdGhpcy5pc0NhbGxTdW1tYXJpemVkID0gdHJ1ZTtcbiAgICB9O1xuXG4gICAgLyoqXG4gICAgICogUmVxdWVzdHMgZm9yIHRva2VuIHVwZGF0aW9uXG4gICAgICpcbiAgICAgKiBAcGFyYW0gZGF0YVxuICAgICAqL1xuICAgIHB1YmxpYyB0b2tlbkV4cGlyeUhhbmRsZXIgPSAoZGF0YTogYW55KSA9PiB7XG4gICAgICAgIEN0aUxvZ2dlci5sb2dJbmZvTWVzc2FnZSh0aGlzLmxvZ1ByZU1lc3NhZ2UgK1xuICAgICAgICAgICAgQ3RpTWVzc2FnZXMuTUVTU0FHRV9IQU5ETEVfVE9LRU5fRVhQSVJZKTtcbiAgICAgICAgdGhpcy5nbG9iYWxDb250ZXh0LmdldFNlc3Npb25Ub2tlbigpLnRoZW4oIChzZXNzaW9uVG9rZW46IHN0cmluZykgPT4ge1xuICAgICAgICAgICAgdGhpcy5jdGlBZGFwdGVyLnJlbmV3Q3RpVG9rZW4oc2Vzc2lvblRva2VuKTtcbiAgICAgICAgfSlcbiAgICB9O1xuXG4gICAgcHVibGljIGxvZ0NhbGxBY3Rpb24oYWN0aW9uTWVzc2FnZTogc3RyaW5nKTogdm9pZCB7XG4gICAgICAgIHZhciBtZXNzYWdlOiBzdHJpbmcgPSBhY3Rpb25NZXNzYWdlICtcbiAgICAgICAgICAgIEN0aU1lc3NhZ2VzLk1FU1NBR0VfQllfQUdFTlQrIHRoaXMuYWdlbnRQcm9maWxlLmFjY291bnRJZDtcbiAgICAgICAgdGhpcy5nbG9iYWxDb250ZXh0LmdldFNlc3Npb25Ub2tlbigpLnRoZW4oKHNlc3Npb25Ub2tlbjogc3RyaW5nKSA9PiB7XG4gICAgICAgICAgICB0aGlzLmN0aUFkYXB0ZXIubG9nTWVzc2FnZShzZXNzaW9uVG9rZW4sIGFjdGlvbk1lc3NhZ2UpO1xuICAgICAgICB9KTtcbiAgICB9XG5cblxuICAgIC8qKlxuICAgICAqIHVwZGF0ZVJpYmJvbmJhck1lbnVBZnRlckxvZ2luIC0gQWRkcyBhbmQgcmVuZGVyXG4gICAgICogc3RhdHVzIHVwZGF0ZSBhbmQgbG9nb3V0IG9wdGlvbnMgdG8gdGhlIHJpYmJvbmJhciBtZW51XG4gICAgICpcbiAgICAgKi9cbiAgICBwdWJsaWMgdXBkYXRlUmliYm9uYmFyTWVudUFmdGVyTG9naW4oKTp2b2lkIHtcbiAgICAgICAgLy9DaGFuZ2UgdGhlIGljb25cbiAgICAgICAgdGhpcy51cGRhdGVHbG9iYWxIZWFkZXJNZW51SWNvbih0aGlzLmN0aVByb3ZpZGVyQ29uZmlnLmRlZmF1bHRTdGF0dXMpO1xuXG4gICAgICAgIC8vQWRkIG9wdGlvbnNcbiAgICAgICAgdmFyIG1lbnVJdGVtQXZhaWxhYmxlID0gdGhpcy5nbG9iYWxIZWFkZXJNZW51LmNyZWF0ZU1lbnVJdGVtKCk7XG4gICAgICAgIG1lbnVJdGVtQXZhaWxhYmxlLnNldExhYmVsKEN0aUNvbnN0YW50cy5CVUlfQ1RJX0xBQkVMX0FWQUlMQUJMRSk7XG4gICAgICAgIG1lbnVJdGVtQXZhaWxhYmxlLnNldEhhbmRsZXIoXG4gICAgICAgICAgICAoKSA9PiB7XG4gICAgICAgICAgICAgICAgdGhpcy51cGRhdGVBZ2VudFN0YXR1cyhDdGlDb25zdGFudHMuQVZBSUxBQkxFKTtcbiAgICAgICAgICAgIH0pO1xuICAgICAgICB0aGlzLmdsb2JhbEhlYWRlck1lbnUuYWRkTWVudUl0ZW0obWVudUl0ZW1BdmFpbGFibGUpO1xuXG4gICAgICAgIHZhciBtZW51SXRlbU5vdEF2YWlsYWJsZSA9IHRoaXMuZ2xvYmFsSGVhZGVyTWVudS5jcmVhdGVNZW51SXRlbSgpO1xuICAgICAgICBtZW51SXRlbU5vdEF2YWlsYWJsZS5zZXRMYWJlbChDdGlDb25zdGFudHMuQlVJX0NUSV9MQUJFTF9OT1RfQVZBSUxBQkxFKTtcbiAgICAgICAgbWVudUl0ZW1Ob3RBdmFpbGFibGUuc2V0SGFuZGxlcihcbiAgICAgICAgICAgICgpID0+IHtcbiAgICAgICAgICAgICAgICB0aGlzLnVwZGF0ZUFnZW50U3RhdHVzKEN0aUNvbnN0YW50cy5OT1RfQVZBSUxBQkxFKTtcbiAgICAgICAgICAgIH0pO1xuICAgICAgICB0aGlzLmdsb2JhbEhlYWRlck1lbnUuYWRkTWVudUl0ZW0obWVudUl0ZW1Ob3RBdmFpbGFibGUpO1xuXG4gICAgICAgIHZhciBtZW51SXRlbUJ1c3kgPSB0aGlzLmdsb2JhbEhlYWRlck1lbnUuY3JlYXRlTWVudUl0ZW0oKTtcbiAgICAgICAgbWVudUl0ZW1CdXN5LnNldExhYmVsKEN0aUNvbnN0YW50cy5CVUlfQ1RJX0xBQkVMX0JVU1kpO1xuICAgICAgICBtZW51SXRlbUJ1c3kuc2V0SGFuZGxlcihcbiAgICAgICAgICAgICgpID0+IHtcbiAgICAgICAgICAgICAgICB0aGlzLnVwZGF0ZUFnZW50U3RhdHVzKEN0aUNvbnN0YW50cy5CVVNZKTtcbiAgICAgICAgICAgIH0pO1xuICAgICAgICB0aGlzLmdsb2JhbEhlYWRlck1lbnUuYWRkTWVudUl0ZW0obWVudUl0ZW1CdXN5KTtcblxuICAgICAgICB2YXIgbWVudUl0ZW1Mb2dvdXQgPSB0aGlzLmdsb2JhbEhlYWRlck1lbnUuY3JlYXRlTWVudUl0ZW0oKTtcbiAgICAgICAgbWVudUl0ZW1Mb2dvdXQuc2V0TGFiZWwoQ3RpQ29uc3RhbnRzLkJVSV9DVElfTEFCRUxfTE9HT1VUKTtcbiAgICAgICAgbWVudUl0ZW1Mb2dvdXQuc2V0SGFuZGxlcihcbiAgICAgICAgICAgICgpID0+IHtcbiAgICAgICAgICAgICAgICB0aGlzLmhhbmRsZUxvZ291dCgpO1xuICAgICAgICAgICAgfSk7XG4gICAgICAgIHRoaXMuZ2xvYmFsSGVhZGVyTWVudS5hZGRNZW51SXRlbShtZW51SXRlbUxvZ291dCk7XG5cbiAgICAgICAgdGhpcy5nbG9iYWxIZWFkZXJNZW51LnJlbmRlcigpO1xuICAgIH1cblxuICAgIC8qKlxuICAgICAqIHVwZGF0ZUxlZnRQYW5lbE1lbnVBZnRlckxvZ2luIC0gVXBkYXRlcyB0aGUgbGVmdHBhbmVsIG1lbnVcbiAgICAgKiBhZnRlciBzdWNjZXNzZnVsIGxvZ2luLlxuICAgICAqL1xuICAgIHB1YmxpYyB1cGRhdGVMZWZ0UGFuZWxNZW51QWZ0ZXJMb2dpbigpOnZvaWQge1xuICAgICAgICB0aGlzLmJ1aUN0aUxlZnRQYW5lbE1lbnUuc2V0TGFiZWwodGhpcy5jdGlQcm92aWRlckNvbmZpZy5wcm92aWRlck5hbWUgKyAnICcgKyBDdGlDb25zdGFudHMuQlVJX0NUSV9MQUJFTF9BVkFJTEFCTEUpO1xuICAgICAgICB0aGlzLmJ1aUN0aUxlZnRQYW5lbE1lbnUuc2V0VmlzaWJsZSh0cnVlKTtcbiAgICAgICAgdGhpcy5idWlDdGlMZWZ0UGFuZWxNZW51LnNldERpc2FibGVkKGZhbHNlKTtcbiAgICAgICAgdmFyIGljb24gPSB0aGlzLmJ1aUN0aUxlZnRQYW5lbE1lbnUuY3JlYXRlSWNvbihDdGlDb25zdGFudHMuQlVJX0NUSV9SSUJCT05CQVJfSUNPTl9UWVBFKTtcbiAgICAgICAgaWNvbi5zZXRJY29uQ2xhc3MoQ3RpQ29uc3RhbnRzLkJVSV9DVElfTEVGVF9QQU5FTF9JQ09OKTtcbiAgICAgICAgLy9HZXQgaWNvbiBjb2xvciBiYXNlZCBvbiBkZWZhdWx0IHN0YXR1c1xuICAgICAgICB2YXIgaWNvbkRhdGEgPSB0aGlzLmdldEljb25EZXRhaWxzRm9yU3RhdHVzKHRoaXMuY3RpUHJvdmlkZXJDb25maWcuZGVmYXVsdFN0YXR1cyk7XG4gICAgICAgIGljb24uc2V0SWNvbkNvbG9yKGljb25EYXRhLmNvbG9yKTtcbiAgICAgICAgdGhpcy5idWlDdGlMZWZ0UGFuZWxNZW51LmFkZEljb24oaWNvbik7XG4gICAgICAgIHRoaXMuYnVpQ3RpTGVmdFBhbmVsTWVudS5yZW5kZXIoKTtcbiAgICB9XG5cbiAgICAvKipcbiAgICAgKlxuICAgICAqIEBwYXJhbSBzdGF0dXNcbiAgICAgKlxuICAgICAqIHVwZGF0ZUFnZW50U3RhdHVzIC0gaGFuZGxlciBmb3Igc3RhdHVzIHVwZGF0ZVxuICAgICAqIHJlcXVlc3RzXG4gICAgICpcbiAgICAgKi9cbiAgICBwdWJsaWMgdXBkYXRlQWdlbnRTdGF0dXMgPSAoc3RhdHVzOnN0cmluZyk6dm9pZCA9PiB7XG4gICAgICAgIC8vVXBkYXRlIGljb25cbiAgICAgICAgaWYgKCF0aGlzLm9uQ2FsbCAmJiB0aGlzLnByZXZTdGF0dXMgIT09IHN0YXR1cykge1xuICAgICAgICAgICAgQ3RpTG9nZ2VyLmxvZ0luZm9NZXNzYWdlKHRoaXMubG9nUHJlTWVzc2FnZSArXG4gICAgICAgICAgICAgICAgQ3RpTWVzc2FnZXMuTUVTU0FHRV9JTklUSUFURV9BQ1RJVklUWV9VUERBVEUgK1xuICAgICAgICAgICAgICAgIEN0aU1lc3NhZ2VzLk1FU1NBR0VfQVBQRU5ERVIgKyBzdGF0dXMpO1xuICAgICAgICAgICAgdGhpcy5jdGlBZGFwdGVyLnVwZGF0ZUFjdGl2aXR5KHN0YXR1cyk7XG4gICAgICAgIH1cbiAgICB9O1xuXG4gICAgLyoqXG4gICAgICogVXBkYXRlcyB0aGUgVUkgYmFzZWQgb24gc3RhdHVzIHVwZGF0ZXMgZnJvbSBzZXJ2ZXJcbiAgICAgKiBcbiAgICAgKiBAcGFyYW0gc3RhdHVzXG4gICAgICovXG4gICAgcHVibGljIHVwZGF0ZUFnZW50VUlPblNlcnZlclVwZGF0ZSA9IChzdGF0dXM6c3RyaW5nKTp2b2lkID0+IHtcbiAgICAgICAgdGhpcy51cGRhdGVHbG9iYWxIZWFkZXJNZW51SWNvbihzdGF0dXMpO1xuICAgICAgICB0aGlzLnVwZGF0ZUxlZnRQYW5lbE1lbnVJY29uKHN0YXR1cyk7XG4gICAgfTtcblxuICAgIC8qKlxuICAgICAqXG4gICAgICogQHBhcmFtIHN0YXR1c1xuICAgICAqXG4gICAgICogdXBkYXRlR2xvYmFsSGVhZGVyTWVudUljb24gLSB1cGRhdGUgdGhlIHJpYmJvbmJhciBtZW51IGljb25cbiAgICAgKiBmb3IgdGhlIGdpdmVuIHN0YXR1cy5cbiAgICAgKlxuICAgICAqL1xuICAgIHB1YmxpYyB1cGRhdGVHbG9iYWxIZWFkZXJNZW51SWNvbiA9IChzdGF0dXM6c3RyaW5nKTp2b2lkID0+IHtcbiAgICAgICAgdmFyIGljb25EYXRhOkljb25EYXRhID0gdGhpcy5nZXRJY29uRGV0YWlsc0ZvclN0YXR1cyhzdGF0dXMpO1xuICAgICAgICB2YXIgaWNvbiA9IHRoaXMuZ2xvYmFsSGVhZGVyTWVudS5jcmVhdGVJY29uKEN0aUNvbnN0YW50cy5CVUlfQ1RJX1JJQkJPTkJBUl9JQ09OX1RZUEUpO1xuICAgICAgICBpY29uLnNldEljb25DbGFzcyhpY29uRGF0YS5jbGFzcyk7XG4gICAgICAgIGljb24uc2V0SWNvbkNvbG9yKGljb25EYXRhLmNvbG9yKTtcbiAgICAgICAgdGhpcy5nbG9iYWxIZWFkZXJNZW51LmFkZEljb24oaWNvbik7XG4gICAgICAgIHRoaXMuZ2xvYmFsSGVhZGVyTWVudS5zZXRMYWJlbCh0aGlzLmN0aVByb3ZpZGVyQ29uZmlnLnByb3ZpZGVyTmFtZSArICcgJyArIGljb25EYXRhLmxhYmVsKTtcbiAgICAgICAgdGhpcy5nbG9iYWxIZWFkZXJNZW51LnJlbmRlcigpO1xuICAgIH07XG5cbiAgICAvKipcbiAgICAgKlxuICAgICAqIEBwYXJhbSBzdGF0dXNcbiAgICAgKlxuICAgICAqIHVwZGF0ZUxlZnRQYW5lbE1lbnVJY29uIC0gVXBkYXRlIHRoZSBsZWZ0cGFuZWwgbWVudSBpY29uXG4gICAgICogZm9yIHRoZSBnaXZlbiBzdGF0dXNcbiAgICAgKi9cbiAgICBwdWJsaWMgdXBkYXRlTGVmdFBhbmVsTWVudUljb24gPSAoc3RhdHVzOnN0cmluZyk6dm9pZCA9PiB7XG4gICAgICAgIHZhciBpY29uRGF0YSA9IHRoaXMuZ2V0SWNvbkRldGFpbHNGb3JTdGF0dXMoc3RhdHVzKTtcbiAgICAgICAgdmFyIGljb24gPSB0aGlzLmJ1aUN0aUxlZnRQYW5lbE1lbnUuY3JlYXRlSWNvbihDdGlDb25zdGFudHMuQlVJX0NUSV9MRUZUX1BBTkVMX0lDT05fVFlQRSk7XG4gICAgICAgIGljb24uc2V0SWNvbkNsYXNzKEN0aUNvbnN0YW50cy5CVUlfQ1RJX0xFRlRfUEFORUxfSUNPTik7XG4gICAgICAgIGljb24uc2V0SWNvbkNvbG9yKGljb25EYXRhLmNvbG9yKTtcbiAgICAgICAgdGhpcy5idWlDdGlMZWZ0UGFuZWxNZW51LmFkZEljb24oaWNvbik7XG4gICAgICAgIHRoaXMuYnVpQ3RpTGVmdFBhbmVsTWVudS5zZXRMYWJlbCh0aGlzLmN0aVByb3ZpZGVyQ29uZmlnLnByb3ZpZGVyTmFtZSArICcgJyArIGljb25EYXRhLmxhYmVsKTtcbiAgICAgICAgdGhpcy5idWlDdGlMZWZ0UGFuZWxNZW51LnJlbmRlcigpO1xuICAgIH07XG5cbiAgICAvKipcbiAgICAgKlxuICAgICAqIEBwYXJhbSBzdGF0dXNcbiAgICAgKlxuICAgICAqIGdldEljb25EZXRhaWxzRm9yU3RhdHVzIC0gUmV0dXJucyB0aGUgSWNvbiBkZXRhaWxzIGZvclxuICAgICAqIGEgZ2l2ZW4gc3RhdHVzIGNvZGUuXG4gICAgICovXG4gICAgcHVibGljIGdldEljb25EZXRhaWxzRm9yU3RhdHVzKHN0YXR1czpzdHJpbmcpOkljb25EYXRhIHtcbiAgICAgICAgdmFyIGljb25EYXRhOkljb25EYXRhID0gPEljb25EYXRhPntcbiAgICAgICAgICAgIGNsYXNzOiAnJyxcbiAgICAgICAgICAgIGNvbG9yOiAnJyxcbiAgICAgICAgICAgIGxhYmVsOiAnJ1xuICAgICAgICB9O1xuXG4gICAgICAgIHN3aXRjaCAoc3RhdHVzKSB7XG4gICAgICAgICAgICBjYXNlIEN0aUNvbnN0YW50cy5BVkFJTEFCTEU6XG4gICAgICAgICAgICAgICAgaWNvbkRhdGEuY2xhc3MgPSBDdGlDb25zdGFudHMuQlVJX0NUSV9JQ09OX0NMQVNTX0FWQUlMQUJMRTtcbiAgICAgICAgICAgICAgICBpY29uRGF0YS5jb2xvciA9ICdncmVlbic7XG4gICAgICAgICAgICAgICAgaWNvbkRhdGEubGFiZWwgPSBDdGlDb25zdGFudHMuQlVJX0NUSV9MQUJFTF9BVkFJTEFCTEU7XG4gICAgICAgICAgICAgICAgYnJlYWs7XG4gICAgICAgICAgICBjYXNlIEN0aUNvbnN0YW50cy5OT1RfQVZBSUxBQkxFOlxuICAgICAgICAgICAgICAgIGljb25EYXRhLmNsYXNzID0gQ3RpQ29uc3RhbnRzLkJVSV9DVElfSUNPTl9DTEFTU19OT1RfQVZBSUxBQkxFO1xuICAgICAgICAgICAgICAgIGljb25EYXRhLmNvbG9yID0gJ2JsYWNrJztcbiAgICAgICAgICAgICAgICBpY29uRGF0YS5sYWJlbCA9IEN0aUNvbnN0YW50cy5CVUlfQ1RJX0xBQkVMX05PVF9BVkFJTEFCTEU7XG4gICAgICAgICAgICAgICAgYnJlYWs7XG4gICAgICAgICAgICBjYXNlIEN0aUNvbnN0YW50cy5CVVNZOlxuICAgICAgICAgICAgICAgIGljb25EYXRhLmNsYXNzID0gQ3RpQ29uc3RhbnRzLkJVSV9DVElfSUNPTl9DTEFTU19CVVNZO1xuICAgICAgICAgICAgICAgIGljb25EYXRhLmNvbG9yID0gJ3JlZCc7XG4gICAgICAgICAgICAgICAgaWNvbkRhdGEubGFiZWwgPSBDdGlDb25zdGFudHMuQlVJX0NUSV9MQUJFTF9CVVNZO1xuICAgICAgICAgICAgICAgIGJyZWFrO1xuICAgICAgICAgICAgY2FzZSBDdGlDb25zdGFudHMuV0FJVDpcbiAgICAgICAgICAgICAgICBpY29uRGF0YS5jbGFzcyA9IEN0aUNvbnN0YW50cy5CVUlfQ1RJX1JJQkJPTkJBUl9JQ09OX1dBSVQ7XG4gICAgICAgICAgICAgICAgaWNvbkRhdGEuY29sb3IgPSAnYmxhY2snO1xuICAgICAgICAgICAgICAgIGljb25EYXRhLmxhYmVsID0gQ3RpQ29uc3RhbnRzLkJVSV9DVElfTEFCRUxfV0FJVDtcbiAgICAgICAgfVxuICAgICAgICByZXR1cm4gaWNvbkRhdGE7XG4gICAgfVxuXG4gICAgLyoqXG4gICAgICogaGFuZGxlTG9nb3V0IC0gc3VibWl0IGxvZ291dCByZXF1ZXN0XG4gICAgICogdG8gdGhlIENUSSB0b29sLiBBbHNvIHVwZGF0ZXMgdGhlIFVJXG4gICAgICovXG4gICAgcHVibGljIGhhbmRsZUxvZ291dCgpOnZvaWQge1xuICAgICAgICBpZiAodGhpcy5pc0xvZ2dlZEluICYmICF0aGlzLm9uQ2FsbCkge1xuICAgICAgICAgICAgdGhpcy5jdGlBZGFwdGVyLmxvZ291dCgpO1xuXG4gICAgICAgICAgICAvL2Rpc3Bvc2UgdGhlIGV4aXN0aW5nIG1lbnUgb3B0aW9uc1xuICAgICAgICAgICAgdGhpcy5nbG9iYWxIZWFkZXJNZW51LmRpc3Bvc2UoKTtcblxuICAgICAgICAgICAgdGhpcy5idWlDdGlMZWZ0UGFuZWxNZW51LnNldFZpc2libGUoZmFsc2UpO1xuICAgICAgICAgICAgdGhpcy5idWlDdGlMZWZ0UGFuZWxNZW51LnJlbmRlcigpO1xuXG4gICAgICAgICAgICB0aGlzLnN1bW1hcml6ZUNhbGwoZmFsc2UsIG51bGwpO1xuXG4gICAgICAgICAgICB0aGlzLmdsb2JhbEhlYWRlck1lbnUgPSBudWxsO1xuICAgICAgICAgICAgdGhpcy5vbkNhbGwgPSBmYWxzZTtcbiAgICAgICAgICAgIHRoaXMuaXNOb3RpZnlBbGxvd2VkID0gdHJ1ZTtcbiAgICAgICAgICAgIHRoaXMuaXNMb2dnZWRJbiA9IGZhbHNlO1xuICAgICAgICAgICAgdGhpcy5wcmV2U3RhdHVzID0gbnVsbDtcbiAgICAgICAgICAgIC8vUmVzZXQgdG8gaW5pdGlhbCBzdGF0ZVxuICAgICAgICAgICAgdGhpcy5pbml0aWFsaXplQ3RpVG9vbEJhck1lbnUoKTtcbiAgICAgICAgfVxuICAgIH1cblxuICAgIC8qKlxuICAgICAqIHN1bW1hcml6ZUNhbGwgLSBBbGwgaGFuZGxpbmcgZm9yIGRpc2Nvbm5lY3RpbmdcbiAgICAgKiBhIGNhbGwgZ29lcyBoZXJlXG4gICAgICpcbiAgICAgKi9cbiAgICBwdWJsaWMgc3VtbWFyaXplQ2FsbCAoaXNUcmFuc2ZlcjogYm9vbGVhbiwgYWdlbnROYW1lOiBzdHJpbmcpOklFeHRlbnNpb25Qcm9taXNlPGFueT4ge1xuICAgICAgICB2YXIgcmVzb2x2ZVJlZjogYW55O1xuICAgICAgICB2YXIgcmVqZWN0UmVmOiBhbnk7XG5cbiAgICAgICAgdmFyIHByb21pc2VSZWY6IElFeHRlbnNpb25Qcm9taXNlPGFueT4gPSBuZXcgRXh0ZW5zaW9uUHJvbWlzZSgocmVzb2x2ZTogYW55LCByZWplY3Q6IGFueSkgPT4ge1xuICAgICAgICAgICAgcmVzb2x2ZVJlZiA9IHJlc29sdmU7XG4gICAgICAgICAgICByZWplY3RSZWYgPSByZWplY3Q7XG4gICAgICAgIH0pO1xuXG4gICAgICAgIGlmKHRoaXMuaXNDYWxsU3VtbWFyaXplZCkge1xuICAgICAgICAgICAgQ3RpTG9nZ2VyLmxvZ0luZm9NZXNzYWdlKHRoaXMubG9nUHJlTWVzc2FnZSArXG4gICAgICAgICAgICAgICAgQ3RpTWVzc2FnZXMuTUVTU0FHRV9DQUxMX1NVTU1BUklaRUQpO1xuICAgICAgICAgICAgcmV0dXJuO1xuICAgICAgICB9XG4gICAgICAgIEN0aUxvZ2dlci5sb2dJbmZvTWVzc2FnZSh0aGlzLmxvZ1ByZU1lc3NhZ2UgKyBDdGlNZXNzYWdlcy5NRVNTQUdFX1NVTU1BUklaRV9DQUxMKTtcbiAgICAgICAgdGhpcy5vbkNhbGwgPSBmYWxzZTtcbiAgICAgICAgdGhpcy5pc05vdGlmeUFsbG93ZWQgPSB0cnVlO1xuXG4gICAgICAgIGlmICh0aGlzLndvcmtzcGFjZVJlY29yZCkge1xuICAgICAgICAgICAgdGhpcy53b3Jrc3BhY2VSZWNvcmQudHJpZ2dlck5hbWVkRXZlbnQoJ0NhbGxFbmRlZCcpO1xuICAgICAgICB9XG5cbiAgICAgICAgLy9BZGQgbm90ZSB0byB0aGUgdGhyZWFkXG4gICAgICAgIGlmICh0aGlzLmluY2lkZW50V29ya3NwYWNlICYmIHRoaXMuaW5jaWRlbnRXb3Jrc3BhY2UuZ2V0V29ya3NwYWNlUmVjb3JkSWQoKSkge1xuICAgICAgICAgICAgdGhpcy5pbmNpZGVudFdvcmtzcGFjZS5nZXRDdXJyZW50RWRpdGVkVGhyZWFkKCdOT1RFJywgdHJ1ZSlcbiAgICAgICAgICAgICAgICAudGhlbigodGhyZWFkRW50cnk6YW55KSA9PiB7XG4gICAgICAgICAgICAgICAgICAgIHZhciBjb250ZW50ID0gQ3RpTWVzc2FnZXMuTUVTU0FHRV9DQUxMX0RVUkFUSU9OICsgdGhpcy5jdGlJbkNhbGxDbG9jay5nZXRDYWxsTGVuZ3RoKCkgKyAnIFxcbiAnXG4gICAgICAgICAgICAgICAgICAgICAgICArIEN0aU1lc3NhZ2VzLk1FU1NBR0VfQ0FMTF9TVEFSVCArIHRoaXMuY3RpSW5DYWxsQ2xvY2suZ2V0Q2xvY2tTdGFydFRpbWUoKSArICcgXFxuICdcbiAgICAgICAgICAgICAgICAgICAgICAgICsgQ3RpTWVzc2FnZXMuTUVTU0FHRV9DQUxMX0VORCArIHRoaXMuY3RpSW5DYWxsQ2xvY2suZ2V0Q2xvY2tFbmRUaW1lKCkgKyAnIFxcbiAnO1xuICAgICAgICAgICAgICAgICAgICBpZihpc1RyYW5zZmVyICYmIGFnZW50TmFtZSl7XG4gICAgICAgICAgICAgICAgICAgICAgICBjb250ZW50ID1jb250ZW50ICsgQ3RpTWVzc2FnZXMuTUVTU0FHRV9DQUxMX1RSQU5TRkVSUkVEX1RPICsgYWdlbnROYW1lO1xuICAgICAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgICAgIHRocmVhZEVudHJ5LnNldENvbnRlbnQoY29udGVudClcbiAgICAgICAgICAgICAgICAgICAgICAgIC50aGVuKCgpID0+IHtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdmFyIG9iamVjdFR5cGU6c3RyaW5nID0gdGhpcy5pbmNpZGVudFdvcmtzcGFjZS5nZXRXb3Jrc3BhY2VSZWNvcmRUeXBlKCk7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHZhciBvYmplY3RJZDpudW1iZXIgPSArdGhpcy5pbmNpZGVudFdvcmtzcGFjZS5nZXRXb3Jrc3BhY2VSZWNvcmRJZCgpO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBpZiAodGhpcy53b3Jrc3BhY2VSZWNvcmQuaXNFZGl0b3JPcGVuKG9iamVjdFR5cGUsIG9iamVjdElkKSkge1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5pbmNpZGVudFdvcmtzcGFjZS5leGVjdXRlRWRpdG9yQ29tbWFuZCgnc2F2ZUFuZENsb3NlJykudGhlbigoKT0+e1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHJlc29sdmVSZWYoKTtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIH0pLmNhdGNoKCgpPT57XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgcmVqZWN0UmVmKCk7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB9KTtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBpZiAodGhpcy5pbnRlcmFjdGlvbldvcmtzcGFjZS5nZXRXb3Jrc3BhY2VSZWNvcmRJZCgpKSB7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB0aGlzLmludGVyYWN0aW9uV29ya3NwYWNlLmV4ZWN1dGVFZGl0b3JDb21tYW5kKCdzYXZlQW5kQ2xvc2UnKS50aGVuKCgpPT57XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgcmVzb2x2ZVJlZigpO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgfSkuY2F0Y2goKCk9PntcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICByZWplY3RSZWYoKTtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIH0pO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgICAgICAgICAgKTtcbiAgICAgICAgICAgICAgICB9KTtcbiAgICAgICAgfSBlbHNlIHtcbiAgICAgICAgICAgIHNldFRpbWVvdXQoKCkgPT4ge1xuICAgICAgICAgICAgICAgIGlmICh0aGlzLmludGVyYWN0aW9uV29ya3NwYWNlICYmIHRoaXMuaW50ZXJhY3Rpb25Xb3Jrc3BhY2UuZ2V0V29ya3NwYWNlUmVjb3JkSWQoKSkge1xuICAgICAgICAgICAgICAgICAgICB0aGlzLmludGVyYWN0aW9uV29ya3NwYWNlLmNsb3NlRWRpdG9yKCkudGhlbigoKT0+e1xuICAgICAgICAgICAgICAgICAgICAgICAgcmVzb2x2ZVJlZigpO1xuICAgICAgICAgICAgICAgICAgICB9KS5jYXRjaCgoKT0+e1xuICAgICAgICAgICAgICAgICAgICAgICAgcmVqZWN0UmVmKCk7XG4gICAgICAgICAgICAgICAgICAgIH0pO1xuICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgIH0sIDEwMCk7XG4gICAgICAgIH1cbiAgICAgICAgcmV0dXJuIHByb21pc2VSZWY7XG4gICAgfVxuXG4gICAgLyoqKlxuICAgICAqIEhhbmRsZXMgd2luZG93IHVubG9hZCwgd2hlbiB1c2VyIGxvZ3Mgb3V0IGZyb20gQlVJXG4gICAgICovXG4gICAgcHJpdmF0ZSByZWdpc3RlclVubG9hZEhhbmRsZXIoKTogdm9pZCB7XG4gICAgICAgIHdpbmRvdy5hZGRFdmVudExpc3RlbmVyKCdiZWZvcmV1bmxvYWQnLCAoKSA9PiB7XG4gICAgICAgICAgICB0aGlzLmhhbmRsZUxvZ291dCgpO1xuICAgICAgICB9KTtcbiAgICB9XG59XG5cbm5ldyBDdGlUZWxlcGhvbnlBZGRpbihuZXcgVHdpbGlvQWRhcHRlcigpKS5lbmFibGVDdGlBZGRpbigpOyJdfQ==