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
 *  SHA1: $Id: fae95374f929e877cfa5f77803bb02772bdc113c $
 * *********************************************************************************************
 *  File: ctiMessagingAddin.js
 * ****************************************************************************************** */
define(["require", "exports", "../utils/messageCache", "../utils/messageConstants", "../utils/ctiMessageViewHelper", "../adapter/twilioMessagingAdapter", "../utils/ctiMessages", "../utils/ctiLogger"], function (require, exports, messageCache_1, messageConstants_1, ctiMessageViewHelper_1, twilioMessagingAdapter_1, ctiMessages_1, ctiLogger_1) {
    "use strict";
    exports.__esModule = true;
    var CtiMessagingAddin = /** @class */ (function () {
        function CtiMessagingAddin(ctiMessagingAdapter) {
            var _this = this;
            this.contactFieldId = 'CId';
            this.workspaceFvcListeners = {};
            this.maxTry = 5;
            this.currentTry = 0;
            this.interval = 1000;
            //TODO - Remove this temp handling
            this.handledReports = {};
            this.logPreMessage = 'CtiMessagingAddin' + ctiMessages_1.CtiMessages.MESSAGE_APPENDER;
            /**
             * handler for TabChanged Event
             *
             * @param tabChangeData
             */
            this.tabChangedListener = function (tabChangeData) {
                var currentWorkspaceData = tabChangeData.getWorkspaceRecord().getCurrentWorkspace();
                if (currentWorkspaceData.objectType === 'Console' || currentWorkspaceData.objectType === 'analytics') {
                    messageCache_1.MessageCache.clearCache();
                    _this.disableMessaging();
                    return;
                }
                if (currentWorkspaceData.objectType !== 'Incident') {
                }
                //Check for message in MessageCache for the new Workspace
                var messageKey = _this.getMessageKey(tabChangeData.getWorkspaceRecord());
                _this.currentMessageKey = messageKey;
                var message = messageCache_1.MessageCache.get(messageKey);
                if (message) {
                    _this.enableMessaging(message);
                    return;
                }
                //2. If no message in cache, populate it.
                if (_this.isMessagingEnabledWorkspace(currentWorkspaceData)) {
                    _this.extensionSdk.registerWorkspaceExtension(function (newWorkspaceRecord) {
                        var primaryFieldName = currentWorkspaceData.objectType + '.' + _this.contactFieldId;
                        if ('Incident' === currentWorkspaceData.objectType) {
                            _this.incidentWorkspace = newWorkspaceRecord;
                        }
                        else {
                            _this.incidentWorkspace = null;
                        }
                        //Fetch the contact details, populate message object and add it to MessageCache
                        newWorkspaceRecord.prefetchWorkspaceFields([primaryFieldName, 'Contact.PhMobile',
                            'Contact.Name.First', 'Contact.Email', 'Contact.Name.Last'])
                            .addExtensionLoadedListener(function (callbackData) {
                            _this.searchContact(primaryFieldName, messageKey, callbackData, _this.incidentWorkspace);
                        });
                        if (!_this.workspaceFvcListeners[messageKey] && (currentWorkspaceData.objectType === 'Incident' ||
                            currentWorkspaceData.objectType === 'Interaction')) {
                            _this.workspaceFvcListeners[messageKey] = true;
                            newWorkspaceRecord.addFieldValueListener(primaryFieldName, function (fvcCallbackData) {
                                if (fvcCallbackData.event.value) {
                                    _this.currentTry = 1;
                                    if (_this.timeoutHandle) {
                                        clearTimeout(_this.timeoutHandle);
                                    }
                                    _this.timeoutHandle = setTimeout(function () {
                                        _this.fieldValueChangeHandler(currentWorkspaceData, messageKey, primaryFieldName);
                                    }, _this.interval);
                                }
                                else {
                                    messageCache_1.MessageCache.remove(messageKey);
                                }
                                //disable previous messaging
                                _this.disableMessaging();
                            });
                        }
                        //Listen to RecordClosing to remove message from cache
                        newWorkspaceRecord.addRecordClosingListener(function (closingCallbackData) {
                            messageCache_1.MessageCache.remove(messageKey);
                            //TODO Temp fix since tabchange is not working on last workspace
                            if (messageCache_1.MessageCache.getCacheSize() === 0) {
                                _this.disableMessaging();
                            }
                        });
                    }, currentWorkspaceData.objectType, currentWorkspaceData.objectId);
                }
                else {
                    _this.disableMessaging();
                }
            };
            /**
             * Handler for Value change of Contact field
             *
             * @param currentWorkspaceData
             * @param messageKey
             * @param primaryFieldName
             */
            this.fieldValueChangeHandler = function (currentWorkspaceData, messageKey, primaryFieldName) {
                _this.extensionSdk.registerWorkspaceExtension(function (registerCallBackData) {
                    var incidentWorkspaceRecord;
                    if (currentWorkspaceData.objectType === 'Incident') {
                        incidentWorkspaceRecord = registerCallBackData;
                    }
                    else {
                        incidentWorkspaceRecord = null;
                    }
                    registerCallBackData.addExtensionLoadedListener(function (extLoadedCallbackData) {
                        if (extLoadedCallbackData.getField('Contact.Name.First').getValue()
                            || extLoadedCallbackData.getField('Contact.Name.Last').getValue()) {
                            _this.searchContact(primaryFieldName, messageKey, extLoadedCallbackData, incidentWorkspaceRecord);
                        }
                        else if (_this.currentTry <= _this.maxTry) {
                            _this.currentTry++;
                            _this.timeoutHandle = setTimeout(function () {
                                _this.fieldValueChangeHandler(currentWorkspaceData, messageKey, primaryFieldName);
                            }, _this.interval);
                            ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage + ctiMessages_1.CtiMessages.MESSAGE_RETRY_CONTACT_FETCH +
                                ctiMessages_1.CtiMessages.MESSAGE_APPENDER + _this.currentTry);
                        }
                        else {
                            if (_this.timeoutHandle) {
                                clearTimeout(_this.timeoutHandle);
                            }
                            _this.currentTry = 0;
                        }
                    }).prefetchWorkspaceFields(['Contact.PhMobile',
                        'Contact.Name.First', 'Contact.Email', 'Contact.Name.Last', primaryFieldName]);
                }, currentWorkspaceData.objectType, currentWorkspaceData.objectId);
            };
            /**
             * Search for the contact details
             *
             * @param primaryFieldName
             * @param messageKey
             * @param callbackData
             * @param incidentWorkspaceRecord
             */
            this.searchContact = function (primaryFieldName, messageKey, callbackData, incidentWorkspaceRecord) {
                if (callbackData.getField('Contact.PhMobile') && callbackData.getField('Contact.PhMobile').getValue()) {
                    _this.extensionSdk.getGlobalContext().then(function (globalContext) {
                        globalContext.getSessionToken().then(function (sessionToken) {
                            ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage + ctiMessages_1.CtiMessages.MESSAGE_SEARCH_CONTACT);
                            _this.ctiMessagingAdapter.searchContact(callbackData.getField('Contact.PhMobile').getValue(), callbackData.getField(primaryFieldName).getValue(), sessionToken, globalContext.getInterfaceUrl())
                                .done(function (data) {
                                ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage + ctiMessages_1.CtiMessages.MESSAGE_SEARCH_COMPLETE);
                                _this.populateMessage(messageKey, JSON.parse(data), incidentWorkspaceRecord);
                            }).fail(function (data) {
                                ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage + ctiMessages_1.CtiMessages.MESSAGE_SEARCH_FAILED);
                                console.log(data);
                            });
                        });
                    });
                }
                else {
                    _this.disableMessaging();
                }
            };
            /**
             * Populate and put message in the message cache
             *
             * @param messageKey
             * @param searchResult
             * @param incidentWorkspaceRecord
             */
            this.populateMessage = function (messageKey, searchResult, incidentWorkspaceRecord) {
                var message = {
                    key: messageKey,
                    message: '',
                    contact: searchResult.contact,
                    incidentWorkspace: incidentWorkspaceRecord
                };
                messageCache_1.MessageCache.put(messageKey, message);
                _this.enableMessaging(message);
            };
            /**
             * Updates message cache when user type the message
             *
             * @param messageKey
             * @param event
             */
            this.messageValueChangeListener = function (messageKey, event) {
                var currentMessage = messageCache_1.MessageCache.get(messageKey);
                if (event && event.target && currentMessage) {
                    currentMessage.message = event.target.value;
                }
            };
            /**
             * Send message
             *
             * @param messageKey
             */
            this.messageBoxButtonClickHandler = function (messageKey) {
                _this.extensionSdk.getGlobalContext().then(function (globalContext) {
                    globalContext.getSessionToken().then(function (sessionToken) {
                        var agentProfile = {
                            interfaceUrl: globalContext.getInterfaceUrl(),
                            accountId: '' + globalContext.getAccountId(),
                            sessionId: sessionToken
                        };
                        ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage + ctiMessages_1.CtiMessages.MESSAGE_SEND_SMS);
                        _this.lastMessageKey = messageKey;
                        var message = messageCache_1.MessageCache.get(messageKey);
                        _this.ctiMessagingAdapter.sendMessage(message, agentProfile)
                            .done(function (data) {
                            _this.sendSuccessHandler(messageKey);
                        })
                            .fail(function (data) {
                            _this.sendFailureHandler();
                        });
                    });
                });
            };
            /**
             * Update SMS UI on successful message sending
             * and removes message from cache
             */
            this.sendSuccessHandler = function (messageKey) {
                ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage + ctiMessages_1.CtiMessages.MESSAGE_SEND_SMS_SUCCESS);
                var message = messageCache_1.MessageCache.get(messageKey);
                if (message.incidentWorkspace && message.incidentWorkspace.getWorkspaceRecordId()) {
                    message.incidentWorkspace.getCurrentEditedThread('NOTE', true)
                        .then(function (threadEntry) {
                        var content = 'SMS sent at : ' + new Date() + ' \n ' +
                            'Contact Name : ' + message.contact.name + ' \n ' +
                            'Contact Number : ' + message.contact.phone + ' \n ' +
                            'Message : ' + message.message;
                        threadEntry.setContent(content)
                            .then(function () {
                            message.incidentWorkspace.executeEditorCommand('save');
                            ctiMessageViewHelper_1.CtiMessageViewHelper.enableSendButtonControlOnSuccess();
                            messageCache_1.MessageCache.clearMessage(_this.lastMessageKey);
                        });
                    });
                }
                else {
                    ctiMessageViewHelper_1.CtiMessageViewHelper.enableSendButtonControlOnSuccess();
                    messageCache_1.MessageCache.clearMessage(_this.lastMessageKey);
                }
            };
            /**
             * Update UI on failure of message sending
             */
            this.sendFailureHandler = function () {
                ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage + ctiMessages_1.CtiMessages.MESSAGE_SEND_SMS_FAILED);
                ctiMessageViewHelper_1.CtiMessageViewHelper.enableSendButtonControlOnFailure();
            };
            this.ctiMessagingAdapter = ctiMessagingAdapter;
        }
        /**
         * Register the Messaging addin
         */
        CtiMessagingAddin.prototype.registerAddin = function () {
            var _this = this;
            ORACLE_SERVICE_CLOUD.extension_loader.load(messageConstants_1.MessageConstants.BUI_CTI_SMS_ADDIN_ID, messageConstants_1.MessageConstants.BUI_CTI_SMS_ADDIN_VERSION).then(function (sdk) {
                _this.extensionSdk = sdk;
                sdk.registerUserInterfaceExtension(function (userInterfaceContext) {
                    userInterfaceContext.getLeftSidePaneContext().then(function (leftSidePaneContext) {
                        leftSidePaneContext.getSidePane(messageConstants_1.MessageConstants.BUI_CTI_LEFT_PANEL_SMS_MENU_ID)
                            .then(function (leftPanelMenu) {
                            _this.buiCtiLeftPanelSMSMenu = leftPanelMenu;
                            leftPanelMenu.setLabel(messageConstants_1.MessageConstants.BUI_CTI_LEFT_PANEL_SMS_MENU_DEFAULT_LABEL);
                            leftPanelMenu.setVisible(false);
                            var icon = leftPanelMenu.createIcon(messageConstants_1.MessageConstants.BUI_CTI_SMS_ADDIN_ICON_TYPE);
                            icon.setIconClass(messageConstants_1.MessageConstants.BUI_CTI_LEFT_PANEL_SMS_ICON);
                            leftPanelMenu.addIcon(icon);
                            leftPanelMenu.render();
                            _this.addTabChangeListener();
                            _this.checkAnalyticsWorkspace();
                        });
                    });
                });
            });
        };
        /**
         * Adds tabchange listener, which tracks opening of
         * workspaces. This is used to pick contact mobile numbers
         * for SMS functionality
         */
        CtiMessagingAddin.prototype.addTabChangeListener = function () {
            var _this = this;
            this.extensionSdk.registerWorkspaceExtension(function (workspaceRecord) {
                // this listener is added to make sure while addins are getting loaded if someone opens up an workspace,
                // still messaging addin should function
                workspaceRecord.addExtensionLoadedListener(function (tabChangeData) {
                    _this.tabChangedListener(tabChangeData);
                });
                // this listener is the actual listener which will responds to tab change event to enable/disable left side bar
                workspaceRecord.addCurrentEditorTabChangedListener(function (tabChangeData) {
                    _this.tabChangedListener(tabChangeData);
                }).prefetchWorkspaceFields([]);
                ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage + ctiMessages_1.CtiMessages.MESSAGE_ADDIN_INITIALIZED);
            });
        };
        //TODO - Remove this handling
        CtiMessagingAddin.prototype.checkAnalyticsWorkspace = function () {
            var _this = this;
            setInterval(function () {
                var contextData = ORACLE_SERVICE_CLOUD.globalContextListener.globalContextData;
                if (contextData && contextData.entityList && contextData.entityList.length > 0) {
                    var lastEntity = contextData.entityList[contextData.entityList.length - 1];
                    if (lastEntity && 'analytics' === lastEntity.objectType && !_this.handledReports[lastEntity.contextId]) {
                        _this.handledReports[lastEntity.contextId] = true;
                        _this.disableMessaging();
                    }
                }
            }, 1000);
        };
        /**
         * This method generates the lookup key for messages
         *
         * @param tabChangeParameter
         */
        CtiMessagingAddin.prototype.getMessageKey = function (workspaceRecord) {
            return workspaceRecord.getCurrentWorkspace().contextId;
        };
        /**
         * Checks if Messaging shall be enabled on the workspace
         *
         * @param workspaceData
         * @returns {boolean}
         */
        CtiMessagingAddin.prototype.isMessagingEnabledWorkspace = function (workspaceData) {
            return (workspaceData.objectType === 'Incident' || workspaceData.objectType === 'Interaction'
                || workspaceData.objectType === 'Contact');
        };
        /**
         * This method enables the SMS icon on left panel
         * @param contactMobile
         */
        CtiMessagingAddin.prototype.enableMessaging = function (message) {
            ctiLogger_1.CtiLogger.logInfoMessage(this.logPreMessage + ctiMessages_1.CtiMessages.MESSAGE_ENABLE_SMS_OPTION);
            this.buiCtiLeftPanelSMSMenu.setLabel(messageConstants_1.MessageConstants.BUI_CTI_LEFT_PANEL_SMS_MENU_DEFAULT_LABEL);
            this.buiCtiLeftPanelSMSMenu.setVisible(true);
            var icon = this.buiCtiLeftPanelSMSMenu.createIcon(messageConstants_1.MessageConstants.BUI_CTI_SMS_ADDIN_ICON_TYPE);
            icon.setIconClass(messageConstants_1.MessageConstants.BUI_CTI_LEFT_PANEL_SMS_ICON);
            icon.setIconColor(messageConstants_1.MessageConstants.BUI_CTI_LEFT_PANEL_SMS_ICON_COLOR);
            this.buiCtiLeftPanelSMSMenu.addIcon(icon);
            this.buiCtiLeftPanelSMSMenu.render();
            ctiMessageViewHelper_1.CtiMessageViewHelper.enableMessagingView(message, this.messageValueChangeListener, this.messageBoxButtonClickHandler);
        };
        /**
         * This method hides the SMS Icon and
         * controls
         *
         */
        CtiMessagingAddin.prototype.disableMessaging = function () {
            if (this.buiCtiLeftPanelSMSMenu) {
                this.buiCtiLeftPanelSMSMenu.setVisible(false);
                this.buiCtiLeftPanelSMSMenu.render();
            }
        };
        return CtiMessagingAddin;
    }());
    exports.CtiMessagingAddin = CtiMessagingAddin;
    new CtiMessagingAddin(new twilioMessagingAdapter_1.TwilioMessagingAdapter()).registerAddin();
});
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiY3RpTWVzc2FnaW5nQWRkaW4uanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyJjdGlNZXNzYWdpbmdBZGRpbi50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTs7Ozs7Z0dBS2dHOzs7O0lBa0JoRztRQW1CSSwyQkFBWSxtQkFBd0M7WUFBcEQsaUJBRUM7WUFmTyxtQkFBYyxHQUFVLEtBQUssQ0FBQztZQUM5QiwwQkFBcUIsR0FBMEIsRUFBRSxDQUFDO1lBQ2xELFdBQU0sR0FBVSxDQUFDLENBQUM7WUFDbEIsZUFBVSxHQUFVLENBQUMsQ0FBQztZQUN0QixhQUFRLEdBQVUsSUFBSSxDQUFDO1lBSS9CLGtDQUFrQztZQUMxQixtQkFBYyxHQUE0QixFQUFFLENBQUM7WUFFN0Msa0JBQWEsR0FBVSxtQkFBbUIsR0FBRyx5QkFBVyxDQUFDLGdCQUFnQixDQUFDO1lBc0VsRjs7OztlQUlHO1lBQ0ssdUJBQWtCLEdBQUcsVUFBQyxhQUE0QztnQkFFdEUsSUFBSSxvQkFBb0IsR0FBaUIsYUFBYSxDQUFDLGtCQUFrQixFQUFFLENBQUMsbUJBQW1CLEVBQUUsQ0FBQztnQkFDbEcsRUFBRSxDQUFDLENBQUMsb0JBQW9CLENBQUMsVUFBVSxLQUFLLFNBQVMsSUFBSSxvQkFBb0IsQ0FBQyxVQUFVLEtBQUssV0FBVyxDQUFDLENBQUMsQ0FBQztvQkFDbkcsMkJBQVksQ0FBQyxVQUFVLEVBQUUsQ0FBQztvQkFDMUIsS0FBSSxDQUFDLGdCQUFnQixFQUFFLENBQUM7b0JBQ3hCLE1BQU0sQ0FBQztnQkFDWCxDQUFDO2dCQUVELEVBQUUsQ0FBQyxDQUFDLG9CQUFvQixDQUFDLFVBQVUsS0FBSyxVQUFVLENBQUMsQ0FBQyxDQUFDO2dCQUVyRCxDQUFDO2dCQUVELHlEQUF5RDtnQkFDekQsSUFBSSxVQUFVLEdBQVUsS0FBSSxDQUFDLGFBQWEsQ0FBQyxhQUFhLENBQUMsa0JBQWtCLEVBQUUsQ0FBQyxDQUFDO2dCQUMvRSxLQUFJLENBQUMsaUJBQWlCLEdBQUcsVUFBVSxD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QUFJLENBQUMsc0JBQXNCLENBQUMsTUFBTSxFQUFFLENBQUM7WUFDekMsQ0FBQztRQUNMLENBQUM7UUFDTCx3QkFBQztJQUFELENBQUMsQUF0WUQsSUFzWUM7SUF0WVksOENBQWlCO0lBd1k5QixJQUFJLGlCQUFpQixDQUFDLElBQUksK0NBQXNCLEVBQUUsQ0FBQyxDQUFDLGFBQWEsRUFBRSxDQUFDIiwic291cmNlc0NvbnRlbnQiOlsiLyogKiAqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqXG4gKiAgJEFDQ0VMRVJBVE9SX0hFQURFUl9QTEFDRV9IT0xERVIkXG4gKiAgU0hBMTogJElkOiBmYWU5NTM3NGY5MjllODc3Y2ZhNWY3NzgwM2JiMDI3NzJiZGMxMTNjICRcbiAqICoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKlxuICogIEZpbGU6ICRBQ0NFTEVSQVRPUl9IRUFERVJfRklMRV9OQU1FX1BMQUNFX0hPTERFUiRcbiAqICoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKiAqL1xuXG5pbXBvcnQgSVNpZGVQYW5lID0gT1JBQ0xFX1NFUlZJQ0VfQ0xPVUQuSVNpZGVQYW5lO1xuaW1wb3J0IElXb3Jrc3BhY2VSZWNvcmQgPSBPUkFDTEVfU0VSVklDRV9DTE9VRC5JV29ya3NwYWNlUmVjb3JkO1xuaW1wb3J0IElXb3Jrc3BhY2VSZWNvcmRFdmVudFBhcmFtZXRlciA9IE9SQUNMRV9TRVJWSUNFX0NMT1VELklXb3Jrc3BhY2VSZWNvcmRFdmVudFBhcmFtZXRlcjtcbmltcG9ydCBJT2JqZWN0RGV0YWlsID0gT1JBQ0xFX1NFUlZJQ0VfQ0xPVUQuSU9iamVjdERldGFpbDtcbmltcG9ydCBJU2lkZVBhbmVDb250ZXh0ID0gT1JBQ0xFX1NFUlZJQ0VfQ0xPVUQuSVNpZGVQYW5lQ29udGV4dDtcbmltcG9ydCB7TWVzc2FnZUNhY2hlfSBmcm9tICcuLi91dGlscy9tZXNzYWdlQ2FjaGUnO1xuaW1wb3J0IHtNZXNzYWdlQ29uc3RhbnRzfSBmcm9tICcuLi91dGlscy9tZXNzYWdlQ29uc3RhbnRzJztcbmltcG9ydCB7TWVzc2FnZX0gZnJvbSAnLi4vbW9kZWwvbWVzc2FnZSc7XG5pbXBvcnQge0N0aU1lc3NhZ2VWaWV3SGVscGVyfSBmcm9tICcuLi91dGlscy9jdGlNZXNzYWdlVmlld0hlbHBlcic7XG5pbXBvcnQge0lDdGlNZXNzYWdpbmdBZGFwdGVyfSBmcm9tICcuLi9jb250cmFjdHMvaUN0aU1lc3NhZ2luZ0FkYXB0ZXInO1xuaW1wb3J0IHtUd2lsaW9NZXNzYWdpbmdBZGFwdGVyfSBmcm9tICcuLi9hZGFwdGVyL3R3aWxpb01lc3NhZ2luZ0FkYXB0ZXInO1xuaW1wb3J0IHtBZ2VudFByb2ZpbGV9IGZyb20gXCIuLi9tb2RlbC9hZ2VudFByb2ZpbGVcIjtcbmltcG9ydCB7Q29udGFjdFNlYXJjaFJlc3VsdH0gZnJvbSBcIi4uL21vZGVsL2NvbnRhY3RTZWFyY2hSZXN1bHRcIjtcbmltcG9ydCB7Q3RpTWVzc2FnZXN9IGZyb20gXCIuLi91dGlscy9jdGlNZXNzYWdlc1wiO1xuaW1wb3J0IHtDdGlMb2dnZXJ9IGZyb20gXCIuLi91dGlscy9jdGlMb2dnZXJcIjtcblxuZXhwb3J0IGNsYXNzIEN0aU1lc3NhZ2luZ0FkZGluIHtcbiAgICBwcml2YXRlIGJ1aUN0aUxlZnRQYW5lbFNNU01lbnU6SVNpZGVQYW5lO1xuICAgIHByaXZhdGUgZXh0ZW5zaW9uU2RrOk9SQUNMRV9TRVJWSUNFX0NMT1VELklFeHRlbnNpb25Qcm92aWRlcjtcbiAgICBwcml2YXRlIGN1cnJlbnRNZXNzYWdlS2V5OnN0cmluZztcbiAgICBwcml2YXRlIGxhc3RNZXNzYWdlS2V5OnN0cmluZztcbiAgICBwcml2YXRlIGN0aU1lc3NhZ2luZ0FkYXB0ZXI6SUN0aU1lc3NhZ2luZ0FkYXB0ZXI7XG4gICAgcHJpdmF0ZSBjb250YWN0RmllbGRJZDpzdHJpbmcgPSAnQ0lkJztcbiAgICBwcml2YXRlIHdvcmtzcGFjZUZ2Y0xpc3RlbmVyczp7W2tleTpzdHJpbmddOmJvb2xlYW59ID0ge307XG4gICAgcHJpdmF0ZSBtYXhUcnk6bnVtYmVyID0gNTtcbiAgICBwcml2YXRlIGN1cnJlbnRUcnk6bnVtYmVyID0gMDtcbiAgICBwcml2YXRlIGludGVydmFsOm51bWJlciA9IDEwMDA7XG4gICAgcHJpdmF0ZSB0aW1lb3V0SGFuZGxlOm51bWJlcjtcbiAgICBwcml2YXRlIGluY2lkZW50V29ya3NwYWNlOklJbmNpZGVudFdvcmtzcGFjZVJlY29yZDtcblxuICAgIC8vVE9ETyAtIFJlbW92ZSB0aGlzIHRlbXAgaGFuZGxpbmdcbiAgICBwcml2YXRlIGhhbmRsZWRSZXBvcnRzOntba2V5OiBzdHJpbmddOiBib29sZWFufSA9IHt9O1xuXG4gICAgcHJpdmF0ZSBsb2dQcmVNZXNzYWdlOnN0cmluZyA9ICdDdGlNZXNzYWdpbmdBZGRpbicgKyBDdGlNZXNzYWdlcy5NRVNTQUdFX0FQUEVOREVSO1xuXG4gICAgY29uc3RydWN0b3IoY3RpTWVzc2FnaW5nQWRhcHRlcjpJQ3RpTWVzc2FnaW5nQWRhcHRlcikge1xuICAgICAgICB0aGlzLmN0aU1lc3NhZ2luZ0FkYXB0ZXIgPSBjdGlNZXNzYWdpbmdBZGFwdGVyO1xuICAgIH1cblxuICAgIC8qKlxuICAgICAqIFJlZ2lzdGVyIHRoZSBNZXNzYWdpbmcgYWRkaW5cbiAgICAgKi9cbiAgICBwdWJsaWMgcmVnaXN0ZXJBZGRpbigpOnZvaWQge1xuICAgICAgICBPUkFDTEVfU0VSVklDRV9DTE9VRC5leHRlbnNpb25fbG9hZGVyLmxvYWQoTWVzc2FnZUNvbnN0YW50cy5CVUlfQ1RJX1NNU19BRERJTl9JRCxcbiAgICAgICAgICAgIE1lc3NhZ2VDb25zdGFudHMuQlVJX0NUSV9TTVNfQURESU5fVkVSU0lPTikudGhlbigoc2RrKSA9PiB7XG4gICAgICAgICAgICB0aGlzLmV4dGVuc2lvblNkayA9IHNkaztcbiAgICAgICAgICAgIHNkay5yZWdpc3RlclVzZXJJbnRlcmZhY2VFeHRlbnNpb24oKHVzZXJJbnRlcmZhY2VDb250ZXh0KSA9PiB7XG4gICAgICAgICAgICAgICAgdXNlckludGVyZmFjZUNvbnRleHQuZ2V0TGVmdFNpZGVQYW5lQ29udGV4dCgpLnRoZW4oXG4gICAgICAgICAgICAgICAgICAgIChsZWZ0U2lkZVBhbmVDb250ZXh0KSA9PiB7XG4gICAgICAgICAgICAgICAgICAgICAgICBsZWZ0U2lkZVBhbmVDb250ZXh0LmdldFNpZGVQYW5lKE1lc3NhZ2VDb25zdGFudHMuQlVJX0NUSV9MRUZUX1BBTkVMX1NNU19NRU5VX0lEKVxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIC50aGVuKChsZWZ0UGFuZWxNZW51OklTaWRlUGFuZSkgPT4ge1xuXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRoaXMuYnVpQ3RpTGVmdFBhbmVsU01TTWVudSA9IGxlZnRQYW5lbE1lbnU7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGxlZnRQYW5lbE1lbnUuc2V0TGFiZWwoTWVzc2FnZUNvbnN0YW50cy5CVUlfQ1RJX0xFRlRfUEFORUxfU01TX01FTlVfREVGQVVMVF9MQUJFTCk7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGxlZnRQYW5lbE1lbnUuc2V0VmlzaWJsZShmYWxzZSk7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHZhciBpY29uID0gbGVmdFBhbmVsTWVudS5jcmVhdGVJY29uKE1lc3NhZ2VDb25zdGFudHMuQlVJX0NUSV9TTVNfQURESU5fSUNPTl9UWVBFKTtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgaWNvbi5zZXRJY29uQ2xhc3MoTWVzc2FnZUNvbnN0YW50cy5CVUlfQ1RJX0xFRlRfUEFORUxfU01TX0lDT04pO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBsZWZ0UGFuZWxNZW51LmFkZEljb24oaWNvbik7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGxlZnRQYW5lbE1lbnUucmVuZGVyKCk7XG5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5hZGRUYWJDaGFuZ2VMaXN0ZW5lcigpO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB0aGlzLmNoZWNrQW5hbHl0aWNzV29ya3NwYWNlKCk7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgfSk7XG4gICAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICApO1xuICAgICAgICAgICAgfSk7XG4gICAgICAgIH0pO1xuICAgIH1cblxuICAgIC8qKlxuICAgICAqIEFkZHMgdGFiY2hhbmdlIGxpc3RlbmVyLCB3aGljaCB0cmFja3Mgb3BlbmluZyBvZlxuICAgICAqIHdvcmtzcGFjZXMuIFRoaXMgaXMgdXNlZCB0byBwaWNrIGNvbnRhY3QgbW9iaWxlIG51bWJlcnNcbiAgICAgKiBmb3IgU01TIGZ1bmN0aW9uYWxpdHlcbiAgICAgKi9cbiAgICBwdWJsaWMgYWRkVGFiQ2hhbmdlTGlzdGVuZXIoKTp2b2lkIHtcbiAgICAgICAgdGhpcy5leHRlbnNpb25TZGsucmVnaXN0ZXJXb3Jrc3BhY2VFeHRlbnNpb24oKHdvcmtzcGFjZVJlY29yZDpJV29ya3NwYWNlUmVjb3JkKSA9PiB7XG4gICAgICAgICAgICAvLyB0aGlzIGxpc3RlbmVyIGlzIGFkZGVkIHRvIG1ha2Ugc3VyZSB3aGlsZSBhZGRpbnMgYXJlIGdldHRpbmcgbG9hZGVkIGlmIHNvbWVvbmUgb3BlbnMgdXAgYW4gd29ya3NwYWNlLFxuICAgICAgICAgICAgLy8gc3RpbGwgbWVzc2FnaW5nIGFkZGluIHNob3VsZCBmdW5jdGlvblxuICAgICAgICAgICAgd29ya3NwYWNlUmVjb3JkLmFkZEV4dGVuc2lvbkxvYWRlZExpc3RlbmVyKCh0YWJDaGFuZ2VEYXRhOklXb3Jrc3BhY2VSZWNvcmRFdmVudFBhcmFtZXRlcikgPT4ge1xuICAgICAgICAgICAgICAgIHRoaXMudGFiQ2hhbmdlZExpc3RlbmVyKHRhYkNoYW5nZURhdGEpO1xuICAgICAgICAgICAgfSk7XG4gICAgICAgICAgICAvLyB0aGlzIGxpc3RlbmVyIGlzIHRoZSBhY3R1YWwgbGlzdGVuZXIgd2hpY2ggd2lsbCByZXNwb25kcyB0byB0YWIgY2hhbmdlIGV2ZW50IHRvIGVuYWJsZS9kaXNhYmxlIGxlZnQgc2lkZSBiYXJcbiAgICAgICAgICAgIHdvcmtzcGFjZVJlY29yZC5hZGRDdXJyZW50RWRpdG9yVGFiQ2hhbmdlZExpc3RlbmVyKCh0YWJDaGFuZ2VEYXRhOklXb3Jrc3BhY2VSZWNvcmRFdmVudFBhcmFtZXRlcikgPT4ge1xuICAgICAgICAgICAgICAgIHRoaXMudGFiQ2hhbmdlZExpc3RlbmVyKHRhYkNoYW5nZURhdGEpO1xuICAgICAgICAgICAgfSkucHJlZmV0Y2hXb3Jrc3BhY2VGaWVsZHMoW10pO1xuICAgICAgICAgICAgQ3RpTG9nZ2VyLmxvZ0luZm9NZXNzYWdlKHRoaXMubG9nUHJlTWVzc2FnZSArIEN0aU1lc3NhZ2VzLk1FU1NBR0VfQURESU5fSU5JVElBTElaRUQpO1xuICAgICAgICB9KTtcbiAgICB9XG5cbiAgICAvL1RPRE8gLSBSZW1vdmUgdGhpcyBoYW5kbGluZ1xuICAgIHByaXZhdGUgY2hlY2tBbmFseXRpY3NXb3Jrc3BhY2UoKTp2b2lkIHtcbiAgICAgICAgc2V0SW50ZXJ2YWwoKCkgPT4ge1xuICAgICAgICAgICAgdmFyIGNvbnRleHREYXRhOmFueSA9ICg8YW55Pk9SQUNMRV9TRVJWSUNFX0NMT1VEKS5nbG9iYWxDb250ZXh0TGlzdGVuZXIuZ2xvYmFsQ29udGV4dERhdGE7XG4gICAgICAgICAgICBpZiAoY29udGV4dERhdGEgJiYgY29udGV4dERhdGEuZW50aXR5TGlzdCAmJiBjb250ZXh0RGF0YS5lbnRpdHlMaXN0Lmxlbmd0aCA+IDApIHtcbiAgICAgICAgICAgICAgICB2YXIgbGFzdEVudGl0eTphbnkgPSBjb250ZXh0RGF0YS5lbnRpdHlMaXN0W2NvbnRleHREYXRhLmVudGl0eUxpc3QubGVuZ3RoIC0gMV07XG4gICAgICAgICAgICAgICAgaWYgKGxhc3RFbnRpdHkgJiYgJ2FuYWx5dGljcycgPT09IGxhc3RFbnRpdHkub2JqZWN0VHlwZSAmJiAhdGhpcy5oYW5kbGVkUmVwb3J0c1tsYXN0RW50aXR5LmNvbnRleHRJZF0pIHtcbiAgICAgICAgICAgICAgICAgICAgdGhpcy5oYW5kbGVkUmVwb3J0c1tsYXN0RW50aXR5LmNvbnRleHRJZF0gPSB0cnVlO1xuICAgICAgICAgICAgICAgICAgICB0aGlzLmRpc2FibGVNZXNzYWdpbmcoKTtcbiAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICB9XG4gICAgICAgIH0sIDEwMDApO1xuICAgIH1cblxuICAgIC8qKlxuICAgICAqIGhhbmRsZXIgZm9yIFRhYkNoYW5nZWQgRXZlbnRcbiAgICAgKlxuICAgICAqIEBwYXJhbSB0YWJDaGFuZ2VEYXRhXG4gICAgICovXG4gICAgcHJpdmF0ZSB0YWJDaGFuZ2VkTGlzdGVuZXIgPSAodGFiQ2hhbmdlRGF0YTpJV29ya3NwYWNlUmVjb3JkRXZlbnRQYXJhbWV0ZXIpOnZvaWQgPT4ge1xuXG4gICAgICAgIHZhciBjdXJyZW50V29ya3NwYWNlRGF0YTpJT2JqZWN0RGV0YWlsID0gdGFiQ2hhbmdlRGF0YS5nZXRXb3Jrc3BhY2VSZWNvcmQoKS5nZXRDdXJyZW50V29ya3NwYWNlKCk7XG4gICAgICAgIGlmIChjdXJyZW50V29ya3NwYWNlRGF0YS5vYmplY3RUeXBlID09PSAnQ29uc29sZScgfHwgY3VycmVudFdvcmtzcGFjZURhdGEub2JqZWN0VHlwZSA9PT0gJ2FuYWx5dGljcycpIHtcbiAgICAgICAgICAgIE1lc3NhZ2VDYWNoZS5jbGVhckNhY2hlKCk7XG4gICAgICAgICAgICB0aGlzLmRpc2FibGVNZXNzYWdpbmcoKTtcbiAgICAgICAgICAgIHJldHVybjtcbiAgICAgICAgfVxuXG4gICAgICAgIGlmIChjdXJyZW50V29ya3NwYWNlRGF0YS5vYmplY3RUeXBlICE9PSAnSW5jaWRlbnQnKSB7XG5cbiAgICAgICAgfVxuXG4gICAgICAgIC8vQ2hlY2sgZm9yIG1lc3NhZ2UgaW4gTWVzc2FnZUNhY2hlIGZvciB0aGUgbmV3IFdvcmtzcGFjZVxuICAgICAgICB2YXIgbWVzc2FnZUtleTpzdHJpbmcgPSB0aGlzLmdldE1lc3NhZ2VLZXkodGFiQ2hhbmdlRGF0YS5nZXRXb3Jrc3BhY2VSZWNvcmQoKSk7XG4gICAgICAgIHRoaXMuY3VycmVudE1lc3NhZ2VLZXkgPSBtZXNzYWdlS2V5O1xuICAgICAgICB2YXIgbWVzc2FnZTpNZXNzYWdlID0gTWVzc2FnZUNhY2hlLmdldChtZXNzYWdlS2V5KTtcblxuICAgICAgICBpZiAobWVzc2FnZSkge1xuICAgICAgICAgICAgdGhpcy5lbmFibGVNZXNzYWdpbmcobWVzc2FnZSk7XG4gICAgICAgICAgICByZXR1cm47XG4gICAgICAgIH1cblxuICAgICAgICAvLzIuIElmIG5vIG1lc3NhZ2UgaW4gY2FjaGUsIHBvcHVsYXRlIGl0LlxuICAgICAgICBpZiAodGhpcy5pc01lc3NhZ2luZ0VuYWJsZWRXb3Jrc3BhY2UoY3VycmVudFdvcmtzcGFjZURhdGEpKSB7XG4gICAgICAgICAgICB0aGlzLmV4dGVuc2lvblNkay5yZWdpc3RlcldvcmtzcGFjZUV4dGVuc2lvbigobmV3V29ya3NwYWNlUmVjb3JkOklXb3Jrc3BhY2VSZWNvcmQpID0+IHtcbiAgICAgICAgICAgICAgICB2YXIgcHJpbWFyeUZpZWxkTmFtZTpzdHJpbmcgPSBjdXJyZW50V29ya3NwYWNlRGF0YS5vYmplY3RUeXBlICsgJy4nICsgdGhpcy5jb250YWN0RmllbGRJZDtcblxuICAgICAgICAgICAgICAgIGlmICgnSW5jaWRlbnQnID09PSBjdXJyZW50V29ya3NwYWNlRGF0YS5vYmplY3RUeXBlKSB7XG4gICAgICAgICAgICAgICAgICAgIHRoaXMuaW5jaWRlbnRXb3Jrc3BhY2UgPSA8SUluY2lkZW50V29ya3NwYWNlUmVjb3JkPm5ld1dvcmtzcGFjZVJlY29yZDtcbiAgICAgICAgICAgICAgICB9IGVsc2Uge1xuICAgICAgICAgICAgICAgICAgICB0aGlzLmluY2lkZW50V29ya3NwYWNlID0gbnVsbDtcbiAgICAgICAgICAgICAgICB9XG5cbiAgICAgICAgICAgICAgICAvL0ZldGNoIHRoZSBjb250YWN0IGRldGFpbHMsIHBvcHVsYXRlIG1lc3NhZ2Ugb2JqZWN0IGFuZCBhZGQgaXQgdG8gTWVzc2FnZUNhY2hlXG4gICAgICAgICAgICAgICAgbmV3V29ya3NwYWNlUmVjb3JkLnByZWZldGNoV29ya3NwYWNlRmllbGRzKFtwcmltYXJ5RmllbGROYW1lLCAnQ29udGFjdC5QaE1vYmlsZScsXG4gICAgICAgICAgICAgICAgICAgICAgICAnQ29udGFjdC5OYW1lLkZpcnN0JywgJ0NvbnRhY3QuRW1haWwnLCAnQ29udGFjdC5OYW1lLkxhc3QnXSlcbiAgICAgICAgICAgICAgICAgICAgLmFkZEV4dGVuc2lvbkxvYWRlZExpc3RlbmVyKChjYWxsYmFja0RhdGE6SVdvcmtzcGFjZVJlY29yZEV2ZW50UGFyYW1ldGVyKSA9PiB7XG4gICAgICAgICAgICAgICAgICAgICAgICB0aGlzLnNlYXJjaENvbnRhY3QocHJpbWFyeUZpZWxkTmFtZSwgbWVzc2FnZUtleSwgY2FsbGJhY2tEYXRhLCB0aGlzLmluY2lkZW50V29ya3NwYWNlKTtcbiAgICAgICAgICAgICAgICAgICAgfSk7XG5cbiAgICAgICAgICAgICAgICBpZiAoIXRoaXMud29ya3NwYWNlRnZjTGlzdGVuZXJzW21lc3NhZ2VLZXldICYmIChjdXJyZW50V29ya3NwYWNlRGF0YS5vYmplY3RUeXBlID09PSAnSW5jaWRlbnQnIHx8XG4gICAgICAgICAgICAgICAgICAgIGN1cnJlbnRXb3Jrc3BhY2VEYXRhLm9iamVjdFR5cGUgPT09ICdJbnRlcmFjdGlvbicpKSB7XG4gICAgICAgICAgICAgICAgICAgIHRoaXMud29ya3NwYWNlRnZjTGlzdGVuZXJzW21lc3NhZ2VLZXldID0gdHJ1ZTtcbiAgICAgICAgICAgICAgICAgICAgbmV3V29ya3NwYWNlUmVjb3JkLmFkZEZpZWxkVmFsdWVMaXN0ZW5lcihwcmltYXJ5RmllbGROYW1lLFxuICAgICAgICAgICAgICAgICAgICAgICAgKGZ2Y0NhbGxiYWNrRGF0YTpJV29ya3NwYWNlUmVjb3JkRXZlbnRQYXJhbWV0ZXIpID0+IHtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBpZiAoZnZjQ2FsbGJhY2tEYXRhLmV2ZW50LnZhbHVlKSB7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRoaXMuY3VycmVudFRyeSA9IDE7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGlmICh0aGlzLnRpbWVvdXRIYW5kbGUpIHtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGNsZWFyVGltZW91dCh0aGlzLnRpbWVvdXRIYW5kbGUpO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRoaXMudGltZW91dEhhbmRsZSA9IHNldFRpbWVvdXQoKCkgPT4ge1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5maWVsZFZhbHVlQ2hhbmdlSGFuZGxlcihjdXJyZW50V29ya3NwYWNlRGF0YSwgbWVzc2FnZUtleSwgcHJpbWFyeUZpZWxkTmFtZSk7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIH0sIHRoaXMuaW50ZXJ2YWwpO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIH0gZWxzZSB7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIE1lc3NhZ2VDYWNoZS5yZW1vdmUobWVzc2FnZUtleSk7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIC8vZGlzYWJsZSBwcmV2aW91cyBtZXNzYWdpbmdcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0aGlzLmRpc2FibGVNZXNzYWdpbmcoKTtcbiAgICAgICAgICAgICAgICAgICAgICAgIH0pO1xuICAgICAgICAgICAgICAgIH1cblxuICAgICAgICAgICAgICAgIC8vTGlzdGVuIHRvIFJlY29yZENsb3NpbmcgdG8gcmVtb3ZlIG1lc3NhZ2UgZnJvbSBjYWNoZVxuICAgICAgICAgICAgICAgIG5ld1dvcmtzcGFjZVJlY29yZC5hZGRSZWNvcmRDbG9zaW5nTGlzdGVuZXIoKGNsb3NpbmdDYWxsYmFja0RhdGE6SVdvcmtzcGFjZVJlY29yZEV2ZW50UGFyYW1ldGVyKSA9PiB7XG4gICAgICAgICAgICAgICAgICAgIE1lc3NhZ2VDYWNoZS5yZW1vdmUobWVzc2FnZUtleSk7XG4gICAgICAgICAgICAgICAgICAgIC8vVE9ETyBUZW1wIGZpeCBzaW5jZSB0YWJjaGFuZ2UgaXMgbm90IHdvcmtpbmcgb24gbGFzdCB3b3Jrc3BhY2VcbiAgICAgICAgICAgICAgICAgICAgaWYgKE1lc3NhZ2VDYWNoZS5nZXRDYWNoZVNpemUoKSA9PT0gMCkge1xuICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5kaXNhYmxlTWVzc2FnaW5nKCk7XG4gICAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICB9KTtcblxuICAgICAgICAgICAgfSwgY3VycmVudFdvcmtzcGFjZURhdGEub2JqZWN0VHlwZSwgY3VycmVudFdvcmtzcGFjZURhdGEub2JqZWN0SWQpO1xuICAgICAgICB9IGVsc2Uge1xuICAgICAgICAgICAgdGhpcy5kaXNhYmxlTWVzc2FnaW5nKCk7XG4gICAgICAgIH1cbiAgICB9O1xuXG4gICAgLyoqXG4gICAgICogSGFuZGxlciBmb3IgVmFsdWUgY2hhbmdlIG9mIENvbnRhY3QgZmllbGRcbiAgICAgKlxuICAgICAqIEBwYXJhbSBjdXJyZW50V29ya3NwYWNlRGF0YVxuICAgICAqIEBwYXJhbSBtZXNzYWdlS2V5XG4gICAgICogQHBhcmFtIHByaW1hcnlGaWVsZE5hbWVcbiAgICAgKi9cbiAgICBwcml2YXRlIGZpZWxkVmFsdWVDaGFuZ2VIYW5kbGVyID0gKGN1cnJlbnRXb3Jrc3BhY2VEYXRhOklPYmplY3REZXRhaWwsIG1lc3NhZ2VLZXk6c3RyaW5nLCBwcmltYXJ5RmllbGROYW1lOnN0cmluZykgPT4ge1xuICAgICAgICB0aGlzLmV4dGVuc2lvblNkay5yZWdpc3RlcldvcmtzcGFjZUV4dGVuc2lvbigocmVnaXN0ZXJDYWxsQmFja0RhdGE6SVdvcmtzcGFjZVJlY29yZCkgPT4ge1xuICAgICAgICAgICAgdmFyIGluY2lkZW50V29ya3NwYWNlUmVjb3JkOklJbmNpZGVudFdvcmtzcGFjZVJlY29yZDtcbiAgICAgICAgICAgIGlmIChjdXJyZW50V29ya3NwYWNlRGF0YS5vYmplY3RUeXBlID09PSAnSW5jaWRlbnQnKSB7XG4gICAgICAgICAgICAgICAgaW5jaWRlbnRXb3Jrc3BhY2VSZWNvcmQgPSA8SUluY2lkZW50V29ya3NwYWNlUmVjb3JkPnJlZ2lzdGVyQ2FsbEJhY2tEYXRhO1xuICAgICAgICAgICAgfSBlbHNlIHtcbiAgICAgICAgICAgICAgICBpbmNpZGVudFdvcmtzcGFjZVJlY29yZCA9IG51bGw7XG4gICAgICAgICAgICB9XG4gICAgICAgICAgICByZWdpc3RlckNhbGxCYWNrRGF0YS5hZGRFeHRlbnNpb25Mb2FkZWRMaXN0ZW5lcihcbiAgICAgICAgICAgICAgICAoZXh0TG9hZGVkQ2FsbGJhY2tEYXRhOklXb3Jrc3BhY2VSZWNvcmRFdmVudFBhcmFtZXRlcikgPT4ge1xuICAgICAgICAgICAgICAgICAgICBpZiAoZXh0TG9hZGVkQ2FsbGJhY2tEYXRhLmdldEZpZWxkKCdDb250YWN0Lk5hbWUuRmlyc3QnKS5nZXRWYWx1ZSgpXG4gICAgICAgICAgICAgICAgICAgICAgICB8fCBleHRMb2FkZWRDYWxsYmFja0RhdGEuZ2V0RmllbGQoJ0NvbnRhY3QuTmFtZS5MYXN0JykuZ2V0VmFsdWUoKSkge1xuICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5zZWFyY2hDb250YWN0KHByaW1hcnlGaWVsZE5hbWUsIG1lc3NhZ2VLZXksIGV4dExvYWRlZENhbGxiYWNrRGF0YSwgaW5jaWRlbnRXb3Jrc3BhY2VSZWNvcmQpO1xuICAgICAgICAgICAgICAgICAgICB9IGVsc2UgaWYgKHRoaXMuY3VycmVudFRyeSA8PSB0aGlzLm1heFRyeSkge1xuICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5jdXJyZW50VHJ5Kys7XG4gICAgICAgICAgICAgICAgICAgICAgICB0aGlzLnRpbWVvdXRIYW5kbGUgPSBzZXRUaW1lb3V0KCgpID0+IHtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0aGlzLmZpZWxkVmFsdWVDaGFuZ2VIYW5kbGVyKGN1cnJlbnRXb3Jrc3BhY2VEYXRhLCBtZXNzYWdlS2V5LCBwcmltYXJ5RmllbGROYW1lKTtcbiAgICAgICAgICAgICAgICAgICAgICAgIH0sIHRoaXMuaW50ZXJ2YWwpO1xuICAgICAgICAgICAgICAgICAgICAgICAgQ3RpTG9nZ2VyLmxvZ0luZm9NZXNzYWdlKHRoaXMubG9nUHJlTWVzc2FnZSArIEN0aU1lc3NhZ2VzLk1FU1NBR0VfUkVUUllfQ09OVEFDVF9GRVRDSCArXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgQ3RpTWVzc2FnZXMuTUVTU0FHRV9BUFBFTkRFUiArIHRoaXMuY3VycmVudFRyeSk7XG4gICAgICAgICAgICAgICAgICAgIH0gZWxzZSB7XG4gICAgICAgICAgICAgICAgICAgICAgICBpZiAodGhpcy50aW1lb3V0SGFuZGxlKSB7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgY2xlYXJUaW1lb3V0KHRoaXMudGltZW91dEhhbmRsZSk7XG4gICAgICAgICAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgICAgICAgICB0aGlzLmN1cnJlbnRUcnkgPSAwO1xuICAgICAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgfSkucHJlZmV0Y2hXb3Jrc3BhY2VGaWVsZHMoWydDb250YWN0LlBoTW9iaWxlJyxcbiAgICAgICAgICAgICAgICAnQ29udGFjdC5OYW1lLkZpcnN0JywgJ0NvbnRhY3QuRW1haWwnLCAnQ29udGFjdC5OYW1lLkxhc3QnLCBwcmltYXJ5RmllbGROYW1lXSk7XG4gICAgICAgIH0sIGN1cnJlbnRXb3Jrc3BhY2VEYXRhLm9iamVjdFR5cGUsIGN1cnJlbnRXb3Jrc3BhY2VEYXRhLm9iamVjdElkKTtcbiAgICB9O1xuXG4gICAgLyoqXG4gICAgICogU2VhcmNoIGZvciB0aGUgY29udGFjdCBkZXRhaWxzXG4gICAgICpcbiAgICAgKiBAcGFyYW0gcHJpbWFyeUZpZWxkTmFtZVxuICAgICAqIEBwYXJhbSBtZXNzYWdlS2V5XG4gICAgICogQHBhcmFtIGNhbGxiYWNrRGF0YVxuICAgICAqIEBwYXJhbSBpbmNpZGVudFdvcmtzcGFjZVJlY29yZFxuICAgICAqL1xuICAgIHByaXZhdGUgc2VhcmNoQ29udGFjdCA9IChwcmltYXJ5RmllbGROYW1lOnN0cmluZywgbWVzc2FnZUtleTpzdHJpbmcsIGNhbGxiYWNrRGF0YTpJV29ya3NwYWNlUmVjb3JkRXZlbnRQYXJhbWV0ZXIsXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgIGluY2lkZW50V29ya3NwYWNlUmVjb3JkOklJbmNpZGVudFdvcmtzcGFjZVJlY29yZCkgPT4ge1xuICAgICAgICBpZiAoY2FsbGJhY2tEYXRhLmdldEZpZWxkKCdDb250YWN0LlBoTW9iaWxlJykgJiYgY2FsbGJhY2tEYXRhLmdldEZpZWxkKCdDb250YWN0LlBoTW9iaWxlJykuZ2V0VmFsdWUoKSkge1xuICAgICAgICAgICAgdGhpcy5leHRlbnNpb25TZGsuZ2V0R2xvYmFsQ29udGV4dCgpLnRoZW4oXG4gICAgICAgICAgICAgICAgKGdsb2JhbENvbnRleHQ6T1JBQ0xFX1NFUlZJQ0VfQ0xPVUQuSUV4dGVuc2lvbkdsb2JhbENvbnRleHQpID0+IHtcbiAgICAgICAgICAgICAgICAgICAgZ2xvYmFsQ29udGV4dC5nZXRTZXNzaW9uVG9rZW4oKS50aGVuKChzZXNzaW9uVG9rZW46c3RyaW5nKSA9PiB7XG4gICAgICAgICAgICAgICAgICAgICAgICBDdGlMb2dnZXIubG9nSW5mb01lc3NhZ2UodGhpcy5sb2dQcmVNZXNzYWdlICsgQ3RpTWVzc2FnZXMuTUVTU0FHRV9TRUFSQ0hfQ09OVEFDVCk7XG4gICAgICAgICAgICAgICAgICAgICAgICB0aGlzLmN0aU1lc3NhZ2luZ0FkYXB0ZXIuc2VhcmNoQ29udGFjdChjYWxsYmFja0RhdGEuZ2V0RmllbGQoJ0NvbnRhY3QuUGhNb2JpbGUnKS5nZXRWYWx1ZSgpLFxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGNhbGxiYWNrRGF0YS5nZXRGaWVsZChwcmltYXJ5RmllbGROYW1lKS5nZXRWYWx1ZSgpLFxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHNlc3Npb25Ub2tlbiwgZ2xvYmFsQ29udGV4dC5nZXRJbnRlcmZhY2VVcmwoKSlcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAuZG9uZSgoZGF0YTphbnkpID0+IHtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgQ3RpTG9nZ2VyLmxvZ0luZm9NZXNzYWdlKHRoaXMubG9nUHJlTWVzc2FnZSArIEN0aU1lc3NhZ2VzLk1FU1NBR0VfU0VBUkNIX0NPTVBMRVRFKTtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5wb3B1bGF0ZU1lc3NhZ2UobWVzc2FnZUtleSwgSlNPTi5wYXJzZShkYXRhKSwgaW5jaWRlbnRXb3Jrc3BhY2VSZWNvcmQpO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIH0pLmZhaWwoKGRhdGE6YW55KSA9PiB7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgQ3RpTG9nZ2VyLmxvZ0luZm9NZXNzYWdlKHRoaXMubG9nUHJlTWVzc2FnZSArIEN0aU1lc3NhZ2VzLk1FU1NBR0VfU0VBUkNIX0ZBSUxFRCk7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgY29uc29sZS5sb2coZGF0YSk7XG4gICAgICAgICAgICAgICAgICAgICAgICB9KTtcbiAgICAgICAgICAgICAgICAgICAgfSlcbiAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICApO1xuICAgICAgICB9IGVsc2Uge1xuICAgICAgICAgICAgdGhpcy5kaXNhYmxlTWVzc2FnaW5nKCk7XG4gICAgICAgIH1cbiAgICB9O1xuXG5cbiAgICAvKipcbiAgICAgKiBQb3B1bGF0ZSBhbmQgcHV0IG1lc3NhZ2UgaW4gdGhlIG1lc3NhZ2UgY2FjaGVcbiAgICAgKlxuICAgICAqIEBwYXJhbSBtZXNzYWdlS2V5XG4gICAgICogQHBhcmFtIHNlYXJjaFJlc3VsdFxuICAgICAqIEBwYXJhbSBpbmNpZGVudFdvcmtzcGFjZVJlY29yZFxuICAgICAqL1xuICAgIHByaXZhdGUgcG9wdWxhdGVNZXNzYWdlID0gKG1lc3NhZ2VLZXk6c3RyaW5nLCBzZWFyY2hSZXN1bHQ6Q29udGFjdFNlYXJjaFJlc3VsdCwgaW5jaWRlbnRXb3Jrc3BhY2VSZWNvcmQ6SUluY2lkZW50V29ya3NwYWNlUmVjb3JkKTp2b2lkID0+IHtcbiAgICAgICAgdmFyIG1lc3NhZ2U6TWVzc2FnZSA9IDxNZXNzYWdlPiB7XG4gICAgICAgICAgICBrZXk6IG1lc3NhZ2VLZXksXG4gICAgICAgICAgICBtZXNzYWdlOiAnJyxcbiAgICAgICAgICAgIGNvbnRhY3Q6IHNlYXJjaFJlc3VsdC5jb250YWN0LFxuICAgICAgICAgICAgaW5jaWRlbnRXb3Jrc3BhY2U6IGluY2lkZW50V29ya3NwYWNlUmVjb3JkXG4gICAgICAgIH07XG5cbiAgICAgICAgTWVzc2FnZUNhY2hlLnB1dChtZXNzYWdlS2V5LCBtZXNzYWdlKTtcbiAgICAgICAgdGhpcy5lbmFibGVNZXNzYWdpbmcobWVzc2FnZSk7XG4gICAgfTtcblxuICAgIC8qKlxuICAgICAqIFRoaXMgbWV0aG9kIGdlbmVyYXRlcyB0aGUgbG9va3VwIGtleSBmb3IgbWVzc2FnZXNcbiAgICAgKlxuICAgICAqIEBwYXJhbSB0YWJDaGFuZ2VQYXJhbWV0ZXJcbiAgICAgKi9cbiAgICBwcml2YXRlIGdldE1lc3NhZ2VLZXkod29ya3NwYWNlUmVjb3JkOiBJV29ya3NwYWNlUmVjb3JkKTpzdHJpbmcge1xuICAgICAgICByZXR1cm4gd29ya3NwYWNlUmVjb3JkLmdldEN1cnJlbnRXb3Jrc3BhY2UoKS5jb250ZXh0SWQ7XG4gICAgfVxuXG5cbiAgICAvKipcbiAgICAgKiBDaGVja3MgaWYgTWVzc2FnaW5nIHNoYWxsIGJlIGVuYWJsZWQgb24gdGhlIHdvcmtzcGFjZVxuICAgICAqXG4gICAgICogQHBhcmFtIHdvcmtzcGFjZURhdGFcbiAgICAgKiBAcmV0dXJucyB7Ym9vbGVhbn1cbiAgICAgKi9cblxuICAgIHByaXZhdGUgaXNNZXNzYWdpbmdFbmFibGVkV29ya3NwYWNlKHdvcmtzcGFjZURhdGE6SU9iamVjdERldGFpbCk6Ym9vbGVhbiB7XG4gICAgICAgIHJldHVybiAod29ya3NwYWNlRGF0YS5vYmplY3RUeXBlID09PSAnSW5jaWRlbnQnIHx8IHdvcmtzcGFjZURhdGEub2JqZWN0VHlwZSA9PT0gJ0ludGVyYWN0aW9uJ1xuICAgICAgICB8fCB3b3Jrc3BhY2VEYXRhLm9iamVjdFR5cGUgPT09ICdDb250YWN0Jyk7XG4gICAgfVxuXG4gICAgLyoqXG4gICAgICogVGhpcyBtZXRob2QgZW5hYmxlcyB0aGUgU01TIGljb24gb24gbGVmdCBwYW5lbFxuICAgICAqIEBwYXJhbSBjb250YWN0TW9iaWxlXG4gICAgICovXG4gICAgcHJpdmF0ZSBlbmFibGVNZXNzYWdpbmcobWVzc2FnZTpNZXNzYWdlKTp2b2lkIHtcbiAgICAgICAgQ3RpTG9nZ2VyLmxvZ0luZm9NZXNzYWdlKHRoaXMubG9nUHJlTWVzc2FnZSArIEN0aU1lc3NhZ2VzLk1FU1NBR0VfRU5BQkxFX1NNU19PUFRJT04pO1xuICAgICAgICB0aGlzLmJ1aUN0aUxlZnRQYW5lbFNNU01lbnUuc2V0TGFiZWwoTWVzc2FnZUNvbnN0YW50cy5CVUlfQ1RJX0xFRlRfUEFORUxfU01TX01FTlVfREVGQVVMVF9MQUJFTCk7XG4gICAgICAgIHRoaXMuYnVpQ3RpTGVmdFBhbmVsU01TTWVudS5zZXRWaXNpYmxlKHRydWUpO1xuICAgICAgICB2YXIgaWNvbiA9IHRoaXMuYnVpQ3RpTGVmdFBhbmVsU01TTWVudS5jcmVhdGVJY29uKE1lc3NhZ2VDb25zdGFudHMuQlVJX0NUSV9TTVNfQURESU5fSUNPTl9UWVBFKTtcbiAgICAgICAgaWNvbi5zZXRJY29uQ2xhc3MoTWVzc2FnZUNvbnN0YW50cy5CVUlfQ1RJX0xFRlRfUEFORUxfU01TX0lDT04pO1xuICAgICAgICBpY29uLnNldEljb25Db2xvcihNZXNzYWdlQ29uc3RhbnRzLkJVSV9DVElfTEVGVF9QQU5FTF9TTVNfSUNPTl9DT0xPUik7XG4gICAgICAgIHRoaXMuYnVpQ3RpTGVmdFBhbmVsU01TTWVudS5hZGRJY29uKGljb24pO1xuICAgICAgICB0aGlzLmJ1aUN0aUxlZnRQYW5lbFNNU01lbnUucmVuZGVyKCk7XG5cbiAgICAgICAgQ3RpTWVzc2FnZVZpZXdIZWxwZXIuZW5hYmxlTWVzc2FnaW5nVmlldyhtZXNzYWdlLCB0aGlzLm1lc3NhZ2VWYWx1ZUNoYW5nZUxpc3RlbmVyLCB0aGlzLm1lc3NhZ2VCb3hCdXR0b25DbGlja0hhbmRsZXIpO1xuICAgIH1cblxuICAgIC8qKlxuICAgICAqIFVwZGF0ZXMgbWVzc2FnZSBjYWNoZSB3aGVuIHVzZXIgdHlwZSB0aGUgbWVzc2FnZVxuICAgICAqXG4gICAgICogQHBhcmFtIG1lc3NhZ2VLZXlcbiAgICAgKiBAcGFyYW0gZXZlbnRcbiAgICAgKi9cbiAgICBwcml2YXRlIG1lc3NhZ2VWYWx1ZUNoYW5nZUxpc3RlbmVyID0gKG1lc3NhZ2VLZXk6c3RyaW5nLCBldmVudDphbnkpOnZvaWQgPT4ge1xuICAgICAgICB2YXIgY3VycmVudE1lc3NhZ2U6TWVzc2FnZSA9IE1lc3NhZ2VDYWNoZS5nZXQobWVzc2FnZUtleSk7XG4gICAgICAgIGlmIChldmVudCAmJiBldmVudC50YXJnZXQgJiYgY3VycmVudE1lc3NhZ2UpIHtcbiAgICAgICAgICAgIGN1cnJlbnRNZXNzYWdlLm1lc3NhZ2UgPSBldmVudC50YXJnZXQudmFsdWU7XG4gICAgICAgIH1cbiAgICB9O1xuXG4gICAgLyoqXG4gICAgICogU2VuZCBtZXNzYWdlXG4gICAgICpcbiAgICAgKiBAcGFyYW0gbWVzc2FnZUtleVxuICAgICAqL1xuICAgIHByaXZhdGUgbWVzc2FnZUJveEJ1dHRvbkNsaWNrSGFuZGxlciA9IChtZXNzYWdlS2V5OnN0cmluZyk6dm9pZCA9PiB7XG4gICAgICAgIHRoaXMuZXh0ZW5zaW9uU2RrLmdldEdsb2JhbENvbnRleHQoKS50aGVuKFxuICAgICAgICAgICAgKGdsb2JhbENvbnRleHQ6T1JBQ0xFX1NFUlZJQ0VfQ0xPVUQuSUV4dGVuc2lvbkdsb2JhbENvbnRleHQpID0+IHtcbiAgICAgICAgICAgICAgICBnbG9iYWxDb250ZXh0LmdldFNlc3Npb25Ub2tlbigpLnRoZW4oKHNlc3Npb25Ub2tlbjpzdHJpbmcpID0+IHtcbiAgICAgICAgICAgICAgICAgICAgdmFyIGFnZW50UHJvZmlsZTpBZ2VudFByb2ZpbGUgPSA8QWdlbnRQcm9maWxlPiB7XG4gICAgICAgICAgICAgICAgICAgICAgICBpbnRlcmZhY2VVcmw6IGdsb2JhbENvbnRleHQuZ2V0SW50ZXJmYWNlVXJsKCksXG4gICAgICAgICAgICAgICAgICAgICAgICBhY2NvdW50SWQ6ICcnICsgZ2xvYmFsQ29udGV4dC5nZXRBY2NvdW50SWQoKSxcbiAgICAgICAgICAgICAgICAgICAgICAgIHNlc3Npb25JZDogc2Vzc2lvblRva2VuXG4gICAgICAgICAgICAgICAgICAgIH07XG4gICAgICAgICAgICAgICAgICAgIEN0aUxvZ2dlci5sb2dJbmZvTWVzc2FnZSh0aGlzLmxvZ1ByZU1lc3NhZ2UgKyBDdGlNZXNzYWdlcy5NRVNTQUdFX1NFTkRfU01TKTtcbiAgICAgICAgICAgICAgICAgICAgdGhpcy5sYXN0TWVzc2FnZUtleSA9IG1lc3NhZ2VLZXk7XG4gICAgICAgICAgICAgICAgICAgIHZhciBtZXNzYWdlOk1lc3NhZ2UgPSBNZXNzYWdlQ2FjaGUuZ2V0KG1lc3NhZ2VLZXkpO1xuXG4gICAgICAgICAgICAgICAgICAgIHRoaXMuY3RpTWVzc2FnaW5nQWRhcHRlci5zZW5kTWVzc2FnZShtZXNzYWdlLCBhZ2VudFByb2ZpbGUpXG4gICAgICAgICAgICAgICAgICAgICAgICAuZG9uZSgoZGF0YTphbnkpID0+IHtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0aGlzLnNlbmRTdWNjZXNzSGFuZGxlcihtZXNzYWdlS2V5KTtcbiAgICAgICAgICAgICAgICAgICAgICAgIH0pXG4gICAgICAgICAgICAgICAgICAgICAgICAuZmFpbCgoZGF0YTphbnkpID0+IHtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0aGlzLnNlbmRGYWlsdXJlSGFuZGxlcigpO1xuICAgICAgICAgICAgICAgICAgICAgICAgfSk7XG4gICAgICAgICAgICAgICAgfSlcbiAgICAgICAgICAgIH1cbiAgICAgICAgKTtcbiAgICB9O1xuXG4gICAgLyoqXG4gICAgICogVXBkYXRlIFNNUyBVSSBvbiBzdWNjZXNzZnVsIG1lc3NhZ2Ugc2VuZGluZ1xuICAgICAqIGFuZCByZW1vdmVzIG1lc3NhZ2UgZnJvbSBjYWNoZVxuICAgICAqL1xuICAgIHByaXZhdGUgc2VuZFN1Y2Nlc3NIYW5kbGVyID0gKG1lc3NhZ2VLZXk6c3RyaW5nKTp2b2lkID0+IHtcbiAgICAgICAgQ3RpTG9nZ2VyLmxvZ0luZm9NZXNzYWdlKHRoaXMubG9nUHJlTWVzc2FnZSArIEN0aU1lc3NhZ2VzLk1FU1NBR0VfU0VORF9TTVNfU1VDQ0VTUyk7XG4gICAgICAgIHZhciBtZXNzYWdlOk1lc3NhZ2UgPSBNZXNzYWdlQ2FjaGUuZ2V0KG1lc3NhZ2VLZXkpO1xuICAgICAgICBpZiAobWVzc2FnZS5pbmNpZGVudFdvcmtzcGFjZSAmJiBtZXNzYWdlLmluY2lkZW50V29ya3NwYWNlLmdldFdvcmtzcGFjZVJlY29yZElkKCkpIHtcbiAgICAgICAgICAgICg8SUluY2lkZW50V29ya3NwYWNlUmVjb3JkPm1lc3NhZ2UuaW5jaWRlbnRXb3Jrc3BhY2UpLmdldEN1cnJlbnRFZGl0ZWRUaHJlYWQoJ05PVEUnLCB0cnVlKVxuICAgICAgICAgICAgICAgIC50aGVuKCh0aHJlYWRFbnRyeTphbnkpID0+IHtcbiAgICAgICAgICAgICAgICAgICAgdmFyIGNvbnRlbnQ6c3RyaW5nID0gJ1NNUyBzZW50IGF0IDogJyArIG5ldyBEYXRlKCkgKyAnIFxcbiAnICtcbiAgICAgICAgICAgICAgICAgICAgICAgICdDb250YWN0IE5hbWUgOiAnICsgbWVzc2FnZS5jb250YWN0Lm5hbWUgKyAnIFxcbiAnICtcbiAgICAgICAgICAgICAgICAgICAgICAgICdDb250YWN0IE51bWJlciA6ICcgKyBtZXNzYWdlLmNvbnRhY3QucGhvbmUgKyAnIFxcbiAnICtcbiAgICAgICAgICAgICAgICAgICAgICAgICdNZXNzYWdlIDogJyArIG1lc3NhZ2UubWVzc2FnZTtcblxuICAgICAgICAgICAgICAgICAgICB0aHJlYWRFbnRyeS5zZXRDb250ZW50KGNvbnRlbnQpXG4gICAgICAgICAgICAgICAgICAgICAgICAudGhlbigoKSA9PiB7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgbWVzc2FnZS5pbmNpZGVudFdvcmtzcGFjZS5leGVjdXRlRWRpdG9yQ29tbWFuZCgnc2F2ZScpO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIEN0aU1lc3NhZ2VWaWV3SGVscGVyLmVuYWJsZVNlbmRCdXR0b25Db250cm9sT25TdWNjZXNzKCk7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgTWVzc2FnZUNhY2hlLmNsZWFyTWVzc2FnZSh0aGlzLmxhc3RNZXNzYWdlS2V5KTtcbiAgICAgICAgICAgICAgICAgICAgICAgIH0pO1xuICAgICAgICAgICAgICAgIH0pO1xuICAgICAgICB9IGVsc2Uge1xuICAgICAgICAgICAgQ3RpTWVzc2FnZVZpZXdIZWxwZXIuZW5hYmxlU2VuZEJ1dHRvbkNvbnRyb2xPblN1Y2Nlc3MoKTtcbiAgICAgICAgICAgIE1lc3NhZ2VDYWNoZS5jbGVhck1lc3NhZ2UodGhpcy5sYXN0TWVzc2FnZUtleSk7XG4gICAgICAgIH1cblxuICAgIH07XG5cbiAgICAvKipcbiAgICAgKiBVcGRhdGUgVUkgb24gZmFpbHVyZSBvZiBtZXNzYWdlIHNlbmRpbmdcbiAgICAgKi9cbiAgICBwcml2YXRlIHNlbmRGYWlsdXJlSGFuZGxlciA9ICgpOnZvaWQgPT4ge1xuICAgICAgICBDdGlMb2dnZXIubG9nSW5mb01lc3NhZ2UodGhpcy5sb2dQcmVNZXNzYWdlICsgQ3RpTWVzc2FnZXMuTUVTU0FHRV9TRU5EX1NNU19GQUlMRUQpO1xuICAgICAgICBDdGlNZXNzYWdlVmlld0hlbHBlci5lbmFibGVTZW5kQnV0dG9uQ29udHJvbE9uRmFpbHVyZSgpO1xuICAgIH07XG5cbiAgICAvKipcbiAgICAgKiBUaGlzIG1ldGhvZCBoaWRlcyB0aGUgU01TIEljb24gYW5kXG4gICAgICogY29udHJvbHNcbiAgICAgKlxuICAgICAqL1xuICAgIHByaXZhdGUgZGlzYWJsZU1lc3NhZ2luZygpOnZvaWQge1xuICAgICAgICBpZiAodGhpcy5idWlDdGlMZWZ0UGFuZWxTTVNNZW51KSB7XG4gICAgICAgICAgICB0aGlzLmJ1aUN0aUxlZnRQYW5lbFNNU01lbnUuc2V0VmlzaWJsZShmYWxzZSk7XG4gICAgICAgICAgICB0aGlzLmJ1aUN0aUxlZnRQYW5lbFNNU01lbnUucmVuZGVyKCk7XG4gICAgICAgIH1cbiAgICB9XG59XG5cbm5ldyBDdGlNZXNzYWdpbmdBZGRpbihuZXcgVHdpbGlvTWVzc2FnaW5nQWRhcHRlcigpKS5yZWdpc3RlckFkZGluKCk7Il19