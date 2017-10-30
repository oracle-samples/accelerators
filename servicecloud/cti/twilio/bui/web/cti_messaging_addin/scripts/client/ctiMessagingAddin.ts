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
 *  File: ctiMessagingAddin.ts
 * ****************************************************************************************** */

import ISidePane = ORACLE_SERVICE_CLOUD.ISidePane;
import IWorkspaceRecord = ORACLE_SERVICE_CLOUD.IWorkspaceRecord;
import IWorkspaceRecordEventParameter = ORACLE_SERVICE_CLOUD.IWorkspaceRecordEventParameter;
import IObjectDetail = ORACLE_SERVICE_CLOUD.IObjectDetail;
import ISidePaneContext = ORACLE_SERVICE_CLOUD.ISidePaneContext;
import {MessageCache} from '../utils/messageCache';
import {MessageConstants} from '../utils/messageConstants';
import {Message} from '../model/message';
import {CtiMessageViewHelper} from '../utils/ctiMessageViewHelper';
import {ICtiMessagingAdapter} from '../contracts/iCtiMessagingAdapter';
import {TwilioMessagingAdapter} from '../adapter/twilioMessagingAdapter';
import {AgentProfile} from "../model/agentProfile";
import {ContactSearchResult} from "../model/contactSearchResult";
import {CtiMessages} from "../utils/ctiMessages";
import {CtiLogger} from "../utils/ctiLogger";

export class CtiMessagingAddin {
    private buiCtiLeftPanelSMSMenu:ISidePane;
    private extensionSdk:ORACLE_SERVICE_CLOUD.IExtensionProvider;
    private currentMessageKey:string;
    private lastMessageKey:string;
    private ctiMessagingAdapter:ICtiMessagingAdapter;
    private contactFieldId:string = 'CId';
    private workspaceFvcListeners:{[key:string]:boolean} = {};
    private maxTry:number = 5;
    private currentTry:number = 0;
    private interval:number = 1000;
    private timeoutHandle:number;
    private incidentWorkspace:IIncidentWorkspaceRecord;

    //TODO - Remove this temp handling
    private handledReports:{[key: string]: boolean} = {};

    private logPreMessage:string = 'CtiMessagingAddin' + CtiMessages.MESSAGE_APPENDER;

    constructor(ctiMessagingAdapter:ICtiMessagingAdapter) {
        this.ctiMessagingAdapter = ctiMessagingAdapter;
    }

    /**
     * Register the Messaging addin
     */
    public registerAddin():void {
        ORACLE_SERVICE_CLOUD.extension_loader.load(MessageConstants.BUI_CTI_SMS_ADDIN_ID,
            MessageConstants.BUI_CTI_SMS_ADDIN_VERSION).then((sdk) => {
            this.extensionSdk = sdk;
            sdk.registerUserInterfaceExtension((userInterfaceContext) => {
                userInterfaceContext.getLeftSidePaneContext().then(
                    (leftSidePaneContext) => {
                        leftSidePaneContext.getSidePane(MessageConstants.BUI_CTI_LEFT_PANEL_SMS_MENU_ID)
                            .then((leftPanelMenu:ISidePane) => {

                                this.buiCtiLeftPanelSMSMenu = leftPanelMenu;
                                leftPanelMenu.setLabel(MessageConstants.BUI_CTI_LEFT_PANEL_SMS_MENU_DEFAULT_LABEL);
                                leftPanelMenu.setVisible(false);
                                var icon = leftPanelMenu.createIcon(MessageConstants.BUI_CTI_SMS_ADDIN_ICON_TYPE);
                                icon.setIconClass(MessageConstants.BUI_CTI_LEFT_PANEL_SMS_ICON);
                                leftPanelMenu.addIcon(icon);
                                leftPanelMenu.render();

                                this.addTabChangeListener();
                                this.checkAnalyticsWorkspace();
                            });
                    }
                );
            });
        });
    }

    /**
     * Adds tabchange listener, which tracks opening of
     * workspaces. This is used to pick contact mobile numbers
     * for SMS functionality
     */
    public addTabChangeListener():void {
        this.extensionSdk.registerWorkspaceExtension((workspaceRecord:IWorkspaceRecord) => {
            // this listener is added to make sure while addins are getting loaded if someone opens up an workspace,
            // still messaging addin should function
            workspaceRecord.addExtensionLoadedListener((tabChangeData:IWorkspaceRecordEventParameter) => {
                this.tabChangedListener(tabChangeData);
            });
            // this listener is the actual listener which will responds to tab change event to enable/disable left side bar
            workspaceRecord.addCurrentEditorTabChangedListener((tabChangeData:IWorkspaceRecordEventParameter) => {
                this.tabChangedListener(tabChangeData);
            }).prefetchWorkspaceFields([]);
            CtiLogger.logInfoMessage(this.logPreMessage + CtiMessages.MESSAGE_ADDIN_INITIALIZED);
        });
    }

    //TODO - Remove this handling
    private checkAnalyticsWorkspace():void {
        setInterval(() => {
            var contextData:any = (<any>ORACLE_SERVICE_CLOUD).globalContextListener.globalContextData;
            if (contextData && contextData.entityList && contextData.entityList.length > 0) {
                var lastEntity:any = contextData.entityList[contextData.entityList.length - 1];
                if (lastEntity && 'analytics' === lastEntity.objectType && !this.handledReports[lastEntity.contextId]) {
                    this.handledReports[lastEntity.contextId] = true;
                    this.disableMessaging();
                }
            }
        }, 1000);
    }

    /**
     * handler for TabChanged Event
     *
     * @param tabChangeData
     */
    private tabChangedListener = (tabChangeData:IWorkspaceRecordEventParameter):void => {

        var currentWorkspaceData:IObjectDetail = tabChangeData.getWorkspaceRecord().getCurrentWorkspace();
        if (currentWorkspaceData.objectType === 'Console' || currentWorkspaceData.objectType === 'analytics') {
            MessageCache.clearCache();
            this.disableMessaging();
            return;
        }

        if (currentWorkspaceData.objectType !== 'Incident') {

        }

        //Check for message in MessageCache for the new Workspace
        var messageKey:string = this.getMessageKey(tabChangeData.getWorkspaceRecord());
        this.currentMessageKey = messageKey;
        var message:Message = MessageCache.get(messageKey);

        if (message) {
            this.enableMessaging(message);
            return;
        }

        //2. If no message in cache, populate it.
        if (this.isMessagingEnabledWorkspace(currentWorkspaceData)) {
            this.extensionSdk.registerWorkspaceExtension((newWorkspaceRecord:IWorkspaceRecord) => {
                var primaryFieldName:string = currentWorkspaceData.objectType + '.' + this.contactFieldId;

                if ('Incident' === currentWorkspaceData.objectType) {
                    this.incidentWorkspace = <IIncidentWorkspaceRecord>newWorkspaceRecord;
                } else {
                    this.incidentWorkspace = null;
                }

                //Fetch the contact details, populate message object and add it to MessageCache
                newWorkspaceRecord.prefetchWorkspaceFields([primaryFieldName, 'Contact.PhMobile',
                        'Contact.Name.First', 'Contact.Email', 'Contact.Name.Last'])
                    .addExtensionLoadedListener((callbackData:IWorkspaceRecordEventParameter) => {
                        this.searchContact(primaryFieldName, messageKey, callbackData, this.incidentWorkspace);
                    });

                if (!this.workspaceFvcListeners[messageKey] && (currentWorkspaceData.objectType === 'Incident' ||
                    currentWorkspaceData.objectType === 'Interaction')) {
                    this.workspaceFvcListeners[messageKey] = true;
                    newWorkspaceRecord.addFieldValueListener(primaryFieldName,
                        (fvcCallbackData:IWorkspaceRecordEventParameter) => {
                            if (fvcCallbackData.event.value) {
                                this.currentTry = 1;
                                if (this.timeoutHandle) {
                                    clearTimeout(this.timeoutHandle);
                                }
                                this.timeoutHandle = setTimeout(() => {
                                    this.fieldValueChangeHandler(currentWorkspaceData, messageKey, primaryFieldName);
                                }, this.interval);
                            } else {
                                MessageCache.remove(messageKey);
                            }
                            //disable previous messaging
                            this.disableMessaging();
                        });
                }

                //Listen to RecordClosing to remove message from cache
                newWorkspaceRecord.addRecordClosingListener((closingCallbackData:IWorkspaceRecordEventParameter) => {
                    MessageCache.remove(messageKey);
                    //TODO Temp fix since tabchange is not working on last workspace
                    if (MessageCache.getCacheSize() === 0) {
                        this.disableMessaging();
                    }
                });

            }, currentWorkspaceData.objectType, currentWorkspaceData.objectId);
        } else {
            this.disableMessaging();
        }
    };

    /**
     * Handler for Value change of Contact field
     *
     * @param currentWorkspaceData
     * @param messageKey
     * @param primaryFieldName
     */
    private fieldValueChangeHandler = (currentWorkspaceData:IObjectDetail, messageKey:string, primaryFieldName:string) => {
        this.extensionSdk.registerWorkspaceExtension((registerCallBackData:IWorkspaceRecord) => {
            var incidentWorkspaceRecord:IIncidentWorkspaceRecord;
            if (currentWorkspaceData.objectType === 'Incident') {
                incidentWorkspaceRecord = <IIncidentWorkspaceRecord>registerCallBackData;
            } else {
                incidentWorkspaceRecord = null;
            }
            registerCallBackData.addExtensionLoadedListener(
                (extLoadedCallbackData:IWorkspaceRecordEventParameter) => {
                    if (extLoadedCallbackData.getField('Contact.Name.First').getValue()
                        || extLoadedCallbackData.getField('Contact.Name.Last').getValue()) {
                        this.searchContact(primaryFieldName, messageKey, extLoadedCallbackData, incidentWorkspaceRecord);
                    } else if (this.currentTry <= this.maxTry) {
                        this.currentTry++;
                        this.timeoutHandle = setTimeout(() => {
                            this.fieldValueChangeHandler(currentWorkspaceData, messageKey, primaryFieldName);
                        }, this.interval);
                        CtiLogger.logInfoMessage(this.logPreMessage + CtiMessages.MESSAGE_RETRY_CONTACT_FETCH +
                            CtiMessages.MESSAGE_APPENDER + this.currentTry);
                    } else {
                        if (this.timeoutHandle) {
                            clearTimeout(this.timeoutHandle);
                        }
                        this.currentTry = 0;
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
    private searchContact = (primaryFieldName:string, messageKey:string, callbackData:IWorkspaceRecordEventParameter,
                             incidentWorkspaceRecord:IIncidentWorkspaceRecord) => {
        if (callbackData.getField('Contact.PhMobile') && callbackData.getField('Contact.PhMobile').getValue()) {
            this.extensionSdk.getGlobalContext().then(
                (globalContext:ORACLE_SERVICE_CLOUD.IExtensionGlobalContext) => {
                    globalContext.getSessionToken().then((sessionToken:string) => {
                        CtiLogger.logInfoMessage(this.logPreMessage + CtiMessages.MESSAGE_SEARCH_CONTACT);
                        this.ctiMessagingAdapter.searchContact(callbackData.getField('Contact.PhMobile').getValue(),
                            callbackData.getField(primaryFieldName).getValue(),
                            sessionToken, globalContext.getInterfaceUrl())
                            .done((data:any) => {
                                CtiLogger.logInfoMessage(this.logPreMessage + CtiMessages.MESSAGE_SEARCH_COMPLETE);
                                this.populateMessage(messageKey, JSON.parse(data), incidentWorkspaceRecord);
                            }).fail((data:any) => {
                            CtiLogger.logInfoMessage(this.logPreMessage + CtiMessages.MESSAGE_SEARCH_FAILED);
                            console.log(data);
                        });
                    })
                }
            );
        } else {
            this.disableMessaging();
        }
    };


    /**
     * Populate and put message in the message cache
     *
     * @param messageKey
     * @param searchResult
     * @param incidentWorkspaceRecord
     */
    private populateMessage = (messageKey:string, searchResult:ContactSearchResult, incidentWorkspaceRecord:IIncidentWorkspaceRecord):void => {
        var message:Message = <Message> {
            key: messageKey,
            message: '',
            contact: searchResult.contact,
            incidentWorkspace: incidentWorkspaceRecord
        };

        MessageCache.put(messageKey, message);
        this.enableMessaging(message);
    };

    /**
     * This method generates the lookup key for messages
     *
     * @param tabChangeParameter
     */
    private getMessageKey(workspaceRecord: IWorkspaceRecord):string {
        return workspaceRecord.getCurrentWorkspace().contextId;
    }


    /**
     * Checks if Messaging shall be enabled on the workspace
     *
     * @param workspaceData
     * @returns {boolean}
     */

    private isMessagingEnabledWorkspace(workspaceData:IObjectDetail):boolean {
        return (workspaceData.objectType === 'Incident' || workspaceData.objectType === 'Interaction'
        || workspaceData.objectType === 'Contact');
    }

    /**
     * This method enables the SMS icon on left panel
     * @param contactMobile
     */
    private enableMessaging(message:Message):void {
        CtiLogger.logInfoMessage(this.logPreMessage + CtiMessages.MESSAGE_ENABLE_SMS_OPTION);
        this.buiCtiLeftPanelSMSMenu.setLabel(MessageConstants.BUI_CTI_LEFT_PANEL_SMS_MENU_DEFAULT_LABEL);
        this.buiCtiLeftPanelSMSMenu.setVisible(true);
        var icon = this.buiCtiLeftPanelSMSMenu.createIcon(MessageConstants.BUI_CTI_SMS_ADDIN_ICON_TYPE);
        icon.setIconClass(MessageConstants.BUI_CTI_LEFT_PANEL_SMS_ICON);
        icon.setIconColor(MessageConstants.BUI_CTI_LEFT_PANEL_SMS_ICON_COLOR);
        this.buiCtiLeftPanelSMSMenu.addIcon(icon);
        this.buiCtiLeftPanelSMSMenu.render();

        CtiMessageViewHelper.enableMessagingView(message, this.messageValueChangeListener, this.messageBoxButtonClickHandler);
    }

    /**
     * Updates message cache when user type the message
     *
     * @param messageKey
     * @param event
     */
    private messageValueChangeListener = (messageKey:string, event:any):void => {
        var currentMessage:Message = MessageCache.get(messageKey);
        if (event && event.target && currentMessage) {
            currentMessage.message = event.target.value;
        }
    };

    /**
     * Send message
     *
     * @param messageKey
     */
    private messageBoxButtonClickHandler = (messageKey:string):void => {
        this.extensionSdk.getGlobalContext().then(
            (globalContext:ORACLE_SERVICE_CLOUD.IExtensionGlobalContext) => {
                globalContext.getSessionToken().then((sessionToken:string) => {
                    var agentProfile:AgentProfile = <AgentProfile> {
                        interfaceUrl: globalContext.getInterfaceUrl(),
                        accountId: '' + globalContext.getAccountId(),
                        sessionId: sessionToken
                    };
                    CtiLogger.logInfoMessage(this.logPreMessage + CtiMessages.MESSAGE_SEND_SMS);
                    this.lastMessageKey = messageKey;
                    var message:Message = MessageCache.get(messageKey);

                    this.ctiMessagingAdapter.sendMessage(message, agentProfile)
                        .done((data:any) => {
                            this.sendSuccessHandler(messageKey);
                        })
                        .fail((data:any) => {
                            this.sendFailureHandler();
                        });
                })
            }
        );
    };

    /**
     * Update SMS UI on successful message sending
     * and removes message from cache
     */
    private sendSuccessHandler = (messageKey:string):void => {
        CtiLogger.logInfoMessage(this.logPreMessage + CtiMessages.MESSAGE_SEND_SMS_SUCCESS);
        var message:Message = MessageCache.get(messageKey);
        if (message.incidentWorkspace && message.incidentWorkspace.getWorkspaceRecordId()) {
            (<IIncidentWorkspaceRecord>message.incidentWorkspace).getCurrentEditedThread('NOTE', true)
                .then((threadEntry:any) => {
                    var content:string = 'SMS sent at : ' + new Date() + ' \n ' +
                        'Contact Name : ' + message.contact.name + ' \n ' +
                        'Contact Number : ' + message.contact.phone + ' \n ' +
                        'Message : ' + message.message;

                    threadEntry.setContent(content)
                        .then(() => {
                            message.incidentWorkspace.executeEditorCommand('save');
                            CtiMessageViewHelper.enableSendButtonControlOnSuccess();
                            MessageCache.clearMessage(this.lastMessageKey);
                        });
                });
        } else {
            CtiMessageViewHelper.enableSendButtonControlOnSuccess();
            MessageCache.clearMessage(this.lastMessageKey);
        }

    };

    /**
     * Update UI on failure of message sending
     */
    private sendFailureHandler = ():void => {
        CtiLogger.logInfoMessage(this.logPreMessage + CtiMessages.MESSAGE_SEND_SMS_FAILED);
        CtiMessageViewHelper.enableSendButtonControlOnFailure();
    };

    /**
     * This method hides the SMS Icon and
     * controls
     *
     */
    private disableMessaging():void {
        if (this.buiCtiLeftPanelSMSMenu) {
            this.buiCtiLeftPanelSMSMenu.setVisible(false);
            this.buiCtiLeftPanelSMSMenu.render();
        }
    }
}

new CtiMessagingAddin(new TwilioMessagingAdapter()).registerAddin();