/* * *******************************************************************************************
 *  This file is part of the Oracle B2C Service Accelerator Reference Integration set published
 *  by Oracle B2C Service licensed under the Universal Permissive License (UPL), Version 1.0 as shown at 
 *  http://oss.oracle.com/licenses/upl
 *  Copyright (c) 2023, Oracle and/or its affiliates.
 ***********************************************************************************************
 *  Accelerator Package: Incident Text Based Classification
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 23C (August 2023) 
 *  date: Tue Aug 22 11:57:47 IST 2023
 
 *  revision: RNW-23C
 *  SHA1: $Id: 6ac40137fbbbec45258b8f471ed3b6985b4c7920 $
 * *********************************************************************************************
 *  File: chatEmotion.ts
 * ****************************************************************************************** */

/// <reference path="./osvcExtension.d.ts" />
/// <reference path ="./engagementPanelJs.d.ts" />

enum LogTypes {
    ERROR = 'Error',
    WARN = 'Warn',
    DEBUG = 'Debug',
    TRACE = 'Trace',
    INFO = 'Info'
}

enum RoleTypes {
    AGENT = 'AGENT',
    END_USER = 'END_USER',
    SYSTEM = 'SYSTEM',
    NOT_ENDUSER = 'NOT_ENDUSER',
    CONFEREE = 'CONFEREE'

}


interface ReportPayload {
    lookupName: string;
    filters: {
        name: string;
        values: string[];
    }[];
}

interface saveSentimentEmotionRequest {
    ChatAIResultSummary: { id: number },
    ChatText: string,
    ChatOrder: number,
    Emotion?: { lookupName: string },
    SuggestEmotion?: { lookupName: string },
    EmotionConf?: string,
    RequestManagerIntervene?: boolean,
    SuggestManagerIntervene?: boolean,
    RequestManagerInterveneConf?: string,
    ChatRole: { lookupName: string }
}

interface SaveParentRecordRequest {
    ChatId: number,
    IsEndUserInNegativeEmotion: boolean,
    HaveAgentInNegativeEmotion: boolean,
    IsAlertTriggered: boolean,
    RequestManagerIntervene: boolean,
    RequestManagerInterveneCount: number,
    IsActive: boolean,
    CurrentQueue: { id: number },
    MaxNegativeMessageCount: number,
    MinEmotionConf: String,
    MinRequestManagerInterveneConf: String,
    InitialMessageToSkipCount: number,
    NegativeMessageCount: number,
    AccountId: { id: number },
    ContactId: { id: number },
    IsPrivateChat: boolean
}
const NEGATIVE_EMOTION = 2;
const POSITIVE_EMOTION = 1;
class ChatEmotion {
    private addedListener = false;
    private INITIAL_MESSAGES_TO_SKIP: number;
    private MAX_NEGATIVE_CHAT_COUNT: number;
    private MIN_REQUEST_MANAGER_CONFIG: number;
    private MIN_EMOTION_CONFIG: number;
    private EMOTION_XO_NAME: string;
    private SUPERVISOR_XO_NAME: string;
    private CUSTOM_CFG_EMOTION = "CUSTOM_CFG_EMOTION";
    private IS_MANAGER_ASK_ACTIVE: boolean;
    private IS_EMOTION_ACTIVE: boolean;
    private SUPERVISOR_ENDPOINT: string;
    private EMOTION_ENDPOINT: string;
    private AGENT_EMOTION_ENDPOINT: string;
    private POSITIVE_LOOKUPNAME: string = 'Positive';
    private NEGATIVE_LOOKUPNAME: string = 'Negative';
    private NEUTRAL_LOOKUPNAME: string  = 'Neutral';
    private CX_API_VERSION = "v1.4";
    private CX_API_URL_PATH = `/connect/${this.CX_API_VERSION}/`;
    private CX_SERVICE_URL = "";
    private CX_GET_PARENT_REPORT = "analyticsReportResults/";
    private CX_PARENT_OBJECT_URL = "AIML.ChatAIResultSummary/";
    private CX_CHILD_OBJECT_URL = "AIML.ChatAIPredictionInfo/";
    private CX_CONFIG_URL = "configurations";
    private engagementDetails: any = {};
    private engagementWithActionRegistered = new Set();
    private sessionToken: String;
    private APP_NAME = "chat_emotion";
    private globalContextPromise: IExtensionPromise<IExtensionGlobalContext> = null;
    private extensionProviderPromise: IExtensionPromise<IExtensionProvider> = null;
    private txtConvertElement = document.createElement("textarea");
    private EN_IN_FORMAT = 'en-IN';
    private FRACTION_LENGTH = 2;
    private HUNDRED_MULTIPLIER = 100;

    private POST_METHOD = 'POST';
    private GET_METHOD = 'GET';
    private REST_CONTENT_TYPE = 'application/json';
    private AS_WAIT_MAX_TIME: number = 20000;
    private TIME_OUT_INTERVAL = 200;

    private async getExtensionProvider(): Promise<ORACLE_SERVICE_CLOUD.IExtensionProvider> {
        if (this.extensionProviderPromise == null) {
            this.extensionProviderPromise = ORACLE_SERVICE_CLOUD.extension_loader.load(this.APP_NAME);
        }
        return await this.extensionProviderPromise;
    }

    private async getGlobalContext(): Promise<ORACLE_SERVICE_CLOUD.IExtensionGlobalContext> {
        if (this.globalContextPromise == null) {
            this.globalContextPromise = (await this.getExtensionProvider()).getGlobalContext();
        }
        return await this.globalContextPromise;
    }

    private async getWorkspaceRecord(objectId : number, type : string): Promise<IWorkspaceRecord> {
        let extensionProvide: ORACLE_SERVICE_CLOUD.IExtensionProvider = await this.getExtensionProvider();
        let workspaceRecordPromise: IExtensionPromise<IWorkspaceRecord> = new ExtensionPromise();
        extensionProvide.registerWorkspaceExtension(function (workspaceRecord: IWorkspaceRecord) {
            workspaceRecordPromise.resolve(workspaceRecord);
        }, type , objectId);

        return await workspaceRecordPromise;
    }

    public async waitUntil(funcRef:()=> any) : Promise<any> {
        let retunValue = false;
        while (!retunValue) {
            retunValue = funcRef();
            if(!retunValue){
                await Promise.resolve();
            }
        }
        return retunValue;
    }

    public async initialize(): Promise<void> {
        const globalContext: IExtensionGlobalContext = await this.getGlobalContext();

        const chatAPIInfo: IChatAPIInfo  =  await this.waitUntil(() => globalContext.getChatAPIInfo());

        await (ORACLE_SERVICE_CLOUD as any).scriptLoader.loadScript([chatAPIInfo.getChatAPIURL()])
        this.sessionToken = await globalContext.getSessionToken();
        this.addLog("call to get agent session", LogTypes.DEBUG);
        await this.loadAndSetupAgentChatSentiment();
        this.addLog("call to get agent session end", LogTypes.DEBUG);

        await this.loadSentimentLabels();

    }

    private async loadSentimentLabels() {
        let emotionList = await this.makeGETAPIRequest("AIML.Emotion");
        if (emotionList != null) {
            let emotionListJson = await emotionList.json();
            let emotions = emotionListJson?.items;
            var self = this;
            emotions.forEach(async (item: any) => {
                if (item.lookupName.toLowerCase() == self.POSITIVE_LOOKUPNAME?.toLowerCase()) {
                    self.POSITIVE_LOOKUPNAME = item.lookupName;
                } else if (item.lookupName.toLowerCase() == self.NEGATIVE_LOOKUPNAME?.toLowerCase()) {
                    self.NEGATIVE_LOOKUPNAME = item.lookupName;
                } else if (item.lookupName.toLowerCase() == self.NEUTRAL_LOOKUPNAME?.toLowerCase()) {
                    self.NEUTRAL_LOOKUPNAME = item.lookupName;
                }
            });
        }
    }

    async loadAndSetupAgentChatSentiment(): Promise<void> {
        await this.loadConfigurations();
        let as: IOracleChatClient.IAgentSession = await this.waitUntilAgentSessionIsLoaded(this.AS_WAIT_MAX_TIME);
        await as.sessionStatusChanged(async (statInfo: IChatSessionStatusEventArgs) => {
            if (statInfo.LoggedIn && !this.addedListener) {
                await this.agentSessionEventsHandler();
            }
        });

    }

    private async waitUntilAgentSessionIsLoaded(maxWaitTime: number): Promise<IOracleChatClient.IAgentSession> {
        let retry = true;
        while (retry) {
            try {
                let agentSession: IOracleChatClient.IAgentSession = await new Promise(async (resolve, reject) => {
                    let timeoutMaxTime = window.setTimeout(() => {
                        retry = false;
                        reject(null);
                    }, maxWaitTime);

                    let timeoutRef = window.setTimeout(() => {
                        reject(null);
                    }, this.TIME_OUT_INTERVAL);
                    let agentSession = await IOracleChatClient.getAgentSession();
                    if (agentSession === undefined) {
                        reject(null);
                    }
                    window.clearTimeout(timeoutRef);
                    window.clearTimeout(timeoutMaxTime);
                    resolve(agentSession);

                });
                retry = false;
                return agentSession;
            } catch (error) {
                this.addLog("Inside get agent session :" + error, LogTypes.ERROR);
            }
        }

    }

    private async loadConfigurations(): Promise<void> {
        const globalContext: ORACLE_SERVICE_CLOUD.IExtensionGlobalContext = await this.getGlobalContext();
        this.CX_SERVICE_URL = globalContext.getInterfaceServiceUrl('REST');
        let configurationListResponse = await this.makeGETAPIRequest(this.CX_CONFIG_URL);
        if (configurationListResponse != null) {
            let configurationListResponseJson = await configurationListResponse.json();
            let configurationList = configurationListResponseJson?.items;
            configurationList.forEach(async (item: any) => {
                if (item.lookupName == this.CUSTOM_CFG_EMOTION) {
                    await this.processConfigurations(item);
                }
            });

        }
    }

    async agentSessionEventsHandler(): Promise<void> {
        let agentSession = await this.waitUntilAgentSessionIsLoaded(this.AS_WAIT_MAX_TIME);
        this.addLog("Inside get agent session", LogTypes.DEBUG);
        this.addLog("status change event: inside AVAILABLE", LogTypes.DEBUG);
        let engagement: IOracleChatClient.IEngagement = null;
        let hasCurrentEngagement = false;
        if (this.IS_MANAGER_ASK_ACTIVE || this.IS_EMOTION_ACTIVE) {
            agentSession.engagementRemoved((engRemovedEvtArgs: IChatEngagementRemovedEventArgs) => this.engagementRemovedHandler(engRemovedEvtArgs));
            agentSession.engagementAssigned((eaEvt: IChatEngagementAssignmentEventArgs) => this.engagementAssignedHandler(eaEvt));
            agentSession.engagementAccepted(async (eaEvt: IChatEngagementAcceptedEventArgs) => this.engagementAcceptedHandler(eaEvt, agentSession));

            this.addedListener = true;

            try {
                engagement = await agentSession.getCurrentEngagement();

                hasCurrentEngagement = true;
            } catch (exceptionVar) {
                this.addLog("no engagement present for chat", LogTypes.DEBUG);
            }
            if (hasCurrentEngagement) {
                let engagementId = Number(await engagement.getEngagementId());

                this.engagementDetails[engagementId] = {
                    assignedTime: new Date()
                };

                if (engagement.MyRole == RoleTypes.CONFEREE) {
                    this.engagementDetails[engagementId].isConference = true;
                }

                let parentAlreadyPresent = await this.checkParentAlreadyPresent(engagementId, true);
                if (parentAlreadyPresent['count'] > 0) {
                    let parentId = parentAlreadyPresent['rows'][0][0];
                    this.engagementDetails[engagementId].parentId = parentId;
                }

                await this.createParentEntry(engagementId);
                await this.addEngagementCallback(agentSession, engagementId, parentAlreadyPresent);
            }
        }

    }

    engagementAssignedHandler(eaEvt: IChatEngagementAssignmentEventArgs): void {
        let contactInfo = eaEvt.ContactInfo;
        this.addLog("Inside engagementAssigned", LogTypes.DEBUG);
        let agentInfo = eaEvt.AgentInfo;
        this.engagementDetails[eaEvt.EngagementId] = {
            accountId: agentInfo.AccountId ? parseInt(agentInfo.AccountId) : 0,
            contactEmail: eaEvt.ContactInfo.EmailAddress,
            agentName: agentInfo.Name,
            agentId : agentInfo.AccountId,
            displayName : "",
            queueId: eaEvt.QueueId,
            userFirstName: contactInfo.FirstName,
            userLastName: contactInfo.LastName,
            userId: contactInfo.ContactId ? parseInt(contactInfo.ContactId) : 0,
            parentId: 0,
            confParentIdSet: new Set(),
            confParentId: 0,
            chatOrder: 0,
            isConference: false,
            assignedTime: new Date()
        };

    }

    async checkParentAlreadyPresent(chatId: number, getActive: boolean): Promise<any> {

        let payload = {
            lookupName: "CheckForParentRecord",
            filters: [{
                name: "Chat Id",
                values: [chatId?.toString()]
            }]
        };
        if (getActive) {
            const ISACTIVE_FLAG = "1";
            payload.filters = [{
                name: "Chat Id",
                values: [chatId?.toString()]
            }, {
                name: "Active",
                values: [ISACTIVE_FLAG]
            }]
        }
        let checkForParentUrl = this.CX_SERVICE_URL + this.CX_API_URL_PATH + this.CX_GET_PARENT_REPORT;
        let response = await this.callPostReportAPI(checkForParentUrl, payload);
        return await response.json();
    }

    async checkChatDBTime(chatId: number): Promise<any> {

        let payload = {
            lookupName: "ChatDurationDetail",
            filters: [{
                name: "ChatId",
                values: [chatId?.toString()]
            }]
        };
        let checkForParentUrl = this.CX_SERVICE_URL + this.CX_API_URL_PATH + this.CX_GET_PARENT_REPORT;
        let response = await this.callPostReportAPI(checkForParentUrl, payload);
        return await response.json();
    }

    private async callPostReportAPI(checkForParentUrl: string, payload: ReportPayload) {
        let apiResponse;
        try {
            apiResponse = await fetch(checkForParentUrl, {
                method: this.POST_METHOD,
                headers: {
                    'Content-Type': this.REST_CONTENT_TYPE,
                    'OSvC-CREST-Application-Context': 'Chat Sentiment Analysis',
                    'OSvC-CREST-Time-UTC' : 'true' ,
                    'Authorization': `Session ${this.sessionToken}`
                },
                body: JSON.stringify(payload)
            });
        } catch (error) {
            this.addLog("Error while calling " + checkForParentUrl + " api " + error, LogTypes.ERROR);
        }


        return apiResponse;
    }

    private async callGETAPI(apiUrl: string) {
        let apiResponse;
        try {
            apiResponse = await fetch(apiUrl, {
                method: this.GET_METHOD,
                headers: {
                    'Content-Type': this.REST_CONTENT_TYPE,
                    'OSvC-CREST-Application-Context': 'Chat Sentiment Analysis',
                    'Authorization': `Session ${this.sessionToken}`
                }
            });
        } catch (error) {
            this.addLog("Error while calling " + apiUrl + " api " + error, LogTypes.ERROR);
        }
        return apiResponse;
    }

    async saveInitialParentEntry(engagementId: number): Promise<any> {
        let saveParentResponse = null;
        let payload: SaveParentRecordRequest = null;

        payload = {
            ChatId: engagementId,
            IsEndUserInNegativeEmotion: false,
            HaveAgentInNegativeEmotion: false,
            IsAlertTriggered: false,
            RequestManagerIntervene: false,
            RequestManagerInterveneCount: 0,
            IsActive: true,
            CurrentQueue: { id: this.engagementDetails[engagementId].queueId },
            MaxNegativeMessageCount: this.MAX_NEGATIVE_CHAT_COUNT,
            MinEmotionConf: String(this.MIN_EMOTION_CONFIG),
            MinRequestManagerInterveneConf: String(this.MIN_REQUEST_MANAGER_CONFIG),
            InitialMessageToSkipCount: this.INITIAL_MESSAGES_TO_SKIP,
            NegativeMessageCount: 0,
            AccountId: this.engagementDetails[engagementId].accountId > 0 ? { id: this.engagementDetails[engagementId].accountId } : null,
            ContactId: this.engagementDetails[engagementId].userId > 0 ? { id: this.engagementDetails[engagementId].userId } : null,
            IsPrivateChat: this.engagementDetails[engagementId].isConference ? true : false
        };


        saveParentResponse = await this.callPOSTRestAPI(payload, this.CX_SERVICE_URL + this.CX_API_URL_PATH + this.CX_PARENT_OBJECT_URL);
        return saveParentResponse?.json();
    }

    async evaluateAndSaveChatResult(chatMessage: IChatMessage, engagementId: number, chatOrder: number, isThisPrivateChat: boolean): Promise<string> {
        let saveSentimentResponse: any = null;
        let payload: saveSentimentEmotionRequest = null;
        let messageBody = this.unEscape(chatMessage.Body);
        let emotionSore = 0;
        let emotion = null;
        let supervisor = null;
        let supervisorScore = 0;

        if (chatMessage.Sender != RoleTypes.SYSTEM) {
            let emotionResponsePromise: any;
            let supervisorResponsePromise: any;
            if (this.IS_EMOTION_ACTIVE ) {
                if(chatMessage.Sender == RoleTypes.END_USER || (chatMessage.Sender == RoleTypes.AGENT && this.AGENT_EMOTION_ENDPOINT != undefined && this.AGENT_EMOTION_ENDPOINT != null)) {
                    emotionResponsePromise = await this.getEmotion(messageBody, chatMessage.Sender);
                }
            }
            if (this.IS_MANAGER_ASK_ACTIVE && chatMessage.Sender == RoleTypes.END_USER) {
                supervisorResponsePromise = await this.getSupervisorAsk(messageBody);
            }

            let emotionResponse = await emotionResponsePromise;
            if (emotionResponse?.hasOwnProperty("documents")) {
                let emotionValue = emotionResponse?.documents[0]?.textClassification[0]?.label == NEGATIVE_EMOTION ? this.NEGATIVE_LOOKUPNAME : emotionResponse?.documents[0]?.textClassification[0]?.label == POSITIVE_EMOTION ? this.POSITIVE_LOOKUPNAME: this.NEUTRAL_LOOKUPNAME;
                emotionSore = emotionResponse?.documents[0]?.textClassification[0]?.score == undefined ? 0 : emotionResponse?.documents[0]?.textClassification[0]?.score;
                emotion = emotionValue;
            } else if (emotionResponse?.hasOwnProperty("error")) {
                this.addLog("Error while calling emotion model", LogTypes.ERROR);
            }
            if (chatMessage.Sender == RoleTypes.END_USER) {
                let supervisorResponse = await supervisorResponsePromise;
                if (supervisorResponse?.hasOwnProperty("documents")) {
                    supervisor = supervisorResponse?.documents[0]?.textClassification[0]?.label;
                    supervisorScore = supervisorResponse?.documents[0]?.textClassification[0]?.score == undefined ? 0 :supervisorResponse?.documents[0]?.textClassification[0]?.score;
                } else if (supervisorResponse?.hasOwnProperty("error")) {
                    this.addLog("Error while calling supervisor model", LogTypes.ERROR);
                }
            }
            let supervisorAsk =  supervisor == null ? null : (supervisor == 1 && ((supervisorScore * this.HUNDRED_MULTIPLIER) > this.MIN_REQUEST_MANAGER_CONFIG)) ? true : false;
            payload = {
                ChatAIResultSummary: { id: isThisPrivateChat ? this.engagementDetails[engagementId].confParentId : this.engagementDetails[engagementId].parentId },
                ChatText: messageBody,
                ChatOrder: chatOrder,
                Emotion: null == emotion ? null : { lookupName: emotion },
                SuggestEmotion: null == emotion ? null : { lookupName: emotion },
                EmotionConf: new Intl.NumberFormat(this.EN_IN_FORMAT, { minimumFractionDigits: this.FRACTION_LENGTH, maximumFractionDigits: this.FRACTION_LENGTH }).format(emotionSore * this.HUNDRED_MULTIPLIER),
                RequestManagerIntervene: supervisorAsk,
                SuggestManagerIntervene: supervisorAsk,
                RequestManagerInterveneConf: new Intl.NumberFormat(this.EN_IN_FORMAT, { minimumFractionDigits: this.FRACTION_LENGTH, maximumFractionDigits: this.FRACTION_LENGTH }).format(supervisorScore * this.HUNDRED_MULTIPLIER),
                ChatRole: { lookupName: chatMessage.Sender }
            };
        } else {
            payload = {
                ChatAIResultSummary: { id: isThisPrivateChat ? this.engagementDetails[engagementId].confParentId : this.engagementDetails[engagementId].parentId },
                ChatText: messageBody,
                ChatOrder: chatOrder,
                RequestManagerIntervene: false,
                RequestManagerInterveneConf: '0.00',
                EmotionConf: '0.00',
                ChatRole: { lookupName: chatMessage.Sender }
            };
        }
        saveSentimentResponse = await this.callPOSTRestAPI(payload, this.CX_SERVICE_URL + this.CX_API_URL_PATH + this.CX_CHILD_OBJECT_URL);
        return saveSentimentResponse;
    }

    async engagementAcceptedHandler(eaEvt: IChatEngagementAcceptedEventArgs, as: IOracleChatClient.IAgentSession): Promise<void> {
        this.addLog("Inside engagementAccepted", LogTypes.DEBUG);
        if(this.engagementDetails[eaEvt.EngagementId].displayName == ""){
            let nameResponse = await this.callGETAPI(this.CX_SERVICE_URL + this.CX_API_URL_PATH +  "accounts/" + this.engagementDetails[eaEvt.EngagementId].agentId);
            let nameResponseJson = await nameResponse.json();
            if (nameResponseJson?.hasOwnProperty("displayName")) {
                this.engagementDetails[eaEvt.EngagementId].displayName  = nameResponseJson?.displayName;
            }
        }

        if (this.engagementDetails[eaEvt.EngagementId].displayName?.includes(eaEvt.AgentName)) {
            this.addLog("Inside ACCEPTED event", LogTypes.DEBUG);
            let parentAlreadyPresent = await this.checkParentAlreadyPresent(eaEvt.EngagementId, false);
            let engagement = await as.getEngagement(eaEvt.EngagementId);
            if (engagement.MyRole == RoleTypes.CONFEREE) {
                this.engagementDetails[eaEvt.EngagementId].isConference = true;
            }
            await this.createParentEntry(eaEvt.EngagementId);
            this.addLog(" Inside engagementAccepted before getEngagement" + eaEvt.EngagementId, LogTypes.DEBUG);
            await this.addEngagementCallback(as, eaEvt.EngagementId, parentAlreadyPresent);
        }
    }

    async addEngagementCallback(as: IOracleChatClient.IAgentSession, engagementId: number, parentAlreadyPresent: any): Promise<void> {
        let engagement = await as.getEngagement(engagementId);
        if (!this.engagementWithActionRegistered.has(engagementId)) {
            engagement.messagePosted(async (chatMessage: IChatMessage) => {
                let chatOrder = this.nextChatOrder(engagementId);
                let messagePostedTime = new Date(chatMessage.PostTime);
                this.checkAndUpdateUserIdToParent(engagementId);


                if (messagePostedTime > this.engagementDetails[engagementId].assignedTime) {

                    this.addLog("Inside message posted :", LogTypes.DEBUG);
                    let isThisPrivateChat = false;
                    if (this.engagementDetails[engagementId].isConference || chatMessage.Visibility == RoleTypes.NOT_ENDUSER) {
                        if (chatMessage.Visibility == RoleTypes.NOT_ENDUSER) {
                            isThisPrivateChat = true;
                        }
                        if (this.engagementDetails[engagementId].confParentId == 0) {
                            let parentAlreadyPresent = await this.checkParentAlreadyPresent(engagementId, true);
                            if (parentAlreadyPresent['count'] > 1) {
                                let i = 1;
                                let lastParent = 0
                                while (i < parentAlreadyPresent['rows'].length) {
                                    this.engagementDetails[engagementId].confParentIdSet.add(Number(parentAlreadyPresent['rows'][i][0]));
                                    lastParent = Number(parentAlreadyPresent['rows'][i][0]);
                                    i++;
                                }
                                this.engagementDetails[engagementId].confParentId = lastParent;
                            }
                        }

                    }
                    if (this.engagementDetails[engagementId].isConference && isThisPrivateChat) {
                        if (this.engagementDetails[engagementId].confParentIdSet.has(this.engagementDetails[engagementId].parentId)) {
                            await this.evaluateAndSaveChatResult(chatMessage, engagementId, chatOrder, isThisPrivateChat);
                            this.addLog("save messagePosted chat with id :" + engagementId, LogTypes.DEBUG);
                        }
                    } else if (!this.engagementDetails[engagementId].isConference && !isThisPrivateChat) {
                        await this.evaluateAndSaveChatResult(chatMessage, engagementId, chatOrder, false);
                        this.addLog("save messagePosted chat with id :" + engagementId, LogTypes.DEBUG);
                    }
                }
            });
            engagement.concluded(async (onConcluded: any) => {
                this.addLog("Engagement has been concluded." + onConcluded, LogTypes.DEBUG);
                await this.saveConcludedChatResult("Engagement has been concluded.", engagementId, false);
            });
            this.engagementWithActionRegistered.add(engagementId);
        }

        let messages = await engagement.getMessages();
        this.addLog("process getMessages: ", LogTypes.DEBUG);
        if (parentAlreadyPresent['count'] == 0) {
            messages.forEach(async (message: IChatMessage) => {
                let chatOrder = this.nextChatOrder(engagementId);
                await this.evaluateAndSaveChatResult(message, engagementId, chatOrder, false);
                this.addLog("save get message chat  with id : ", LogTypes.DEBUG);
            });
        }
        this.checkAndUpdateUserIdToParent(engagementId);

    }

    private checkAndUpdateUserIdToParent(engagementId: number) {
        if (this.engagementDetails[engagementId].userId <= 0) {
            let entityList =(ORACLE_SERVICE_CLOUD as any).globalContextListener.globalContextData.entityList;
            entityList.forEach(async (entity: any) => {
                let objectType = entity.objectType;
                let objectId = entity.objectId;
                if (objectType == "Interaction") {
                    let ws = await this.getWorkspaceRecord(objectId, objectType);
                    let workGroupContext = await ws.getWorkGroupContext();
                    let workGroupEntity = await workGroupContext.getWorkGroupEntities();
                    workGroupEntity.forEach(async (workEntity: any) => {
                        if (workEntity.objectId == engagementId && workEntity.objectType == 'Chat') {
                            let fields: ORACLE_SERVICE_CLOUD.IFieldDetails = await ws.getFieldValues(['Interaction.CId']);
                            let value = Number(fields.getField('Interaction.CId').getValue());
                            if(value > 0) {
                                this.updateParentWithContact(value, engagementId);
                            }
                        }
                    });
                }
            });
        }
    }


    private updateParentWithContact(value: number, engagementId: number) {
        let payload = {
            ContactId: { id: value }
        };
        this.updateParent(payload, this.engagementDetails[engagementId].parentId);
        this.engagementDetails[engagementId].userId = value;
    }

    private nextChatOrder(engagementId: number) {
        let chatOrder = this.engagementDetails[engagementId].chatOrder;
        this.engagementDetails[engagementId].chatOrder = chatOrder + 1;
        return chatOrder;
    }

    async createParentEntry(engagementId: number): Promise<void> {
        if (this.engagementDetails[engagementId].parentId == 0) {
            let parentResponse = await this.saveInitialParentEntry(engagementId);
            if (this.engagementDetails[engagementId].parentId == 0) {
                this.engagementDetails[engagementId].parentId = parentResponse.id;
            }
            await this.checkAndUpdateProductDBTime(engagementId, false);

            this.addLog("Parent Record created with id" + parentResponse.id, LogTypes.DEBUG);
        }
    }

    async engagementRemovedHandler(engRemovedEvtArgs: IChatEngagementRemovedEventArgs): Promise<void> {
        this.addLog("Engagement has been removed." + engRemovedEvtArgs.EngagementId, LogTypes.DEBUG);
        await this.saveConcludedChatResult("Engagement has been concluded.", engRemovedEvtArgs.EngagementId, true);
        await this.checkAndUpdateProductDBTime(engRemovedEvtArgs.EngagementId, true);
    }

    private async checkAndUpdateProductDBTime(engagementId: number, checkForCompleted: boolean) {
        let i = 0;
        while (i < 10) {
            await new Promise((resolve) => setTimeout(resolve, 1000));
            let response = await this.checkChatDBTime(engagementId);
            if (response['count'] > 0) {

                if (response['rows'][0][1] || response['rows'][0][2]) {
                    let payload = {
                        FirstEngagementTime: response['rows'][0][1],
                        CompletedTime: response['rows'][0][2]
                    };
                    await this.updateParent(payload, this.engagementDetails[engagementId].parentId);

                    if(checkForCompleted && response['rows'][0][2]){
                        i = 20;
                    } else if( !checkForCompleted && response['rows'][0][1]) {
                        i = 20;
                    }
                }
            }
            i++;
        }
    }

    async saveConcludedChatResult(messageBody: string, engagementId: number, sendMessage: boolean): Promise<Response> {
        let saveConcludedChatResponse = null;
        let payload = null;
        let parentId = this.engagementDetails[engagementId].parentId;
        if (sendMessage) {
            payload = {
                ChatAIResultSummary: { id: parentId },
                ChatText: messageBody,
                ChatOrder: this.nextChatOrder(engagementId),
                RequestManagerInterveneConf: '0.00',
                EmotionConf: '0.00',
                ChatRole: { lookupName: RoleTypes.SYSTEM }
            };
            saveConcludedChatResponse = await this.callPOSTRestAPI(payload, this.CX_SERVICE_URL + this.CX_API_URL_PATH + this.CX_CHILD_OBJECT_URL);
        }

        await this.updateFinalParentEntry(parentId, false);
        return saveConcludedChatResponse;
    }

    async updateFinalParentEntry(parentId: number, isActive: boolean): Promise<Response> {
        let payload = null;
        payload = {
            IsActive: isActive
        };
        return await this.updateParent(payload, parentId);
    }

    async getEmotion(message: string, sender: string) {

        let apiPath: string = '';
        let enpoint : string = '';
        if (sender == RoleTypes.AGENT) {
            enpoint = this.AGENT_EMOTION_ENDPOINT;
        } else {
            enpoint = this.EMOTION_ENDPOINT;
        }
        const payload = {"documents":[{"key":"1","text":message,"languageCode":"en"}],"endpointId":enpoint};
        return await this.getXOResponse(apiPath, payload, this.EMOTION_XO_NAME);
    }

    async getSupervisorAsk(message: string) {
        const payload = {"documents":[{"key":"1","text":message,"languageCode":"en"}],"endpointId":this.SUPERVISOR_ENDPOINT};
        let apiPath = "";
        return await this.getXOResponse(apiPath, payload, this.SUPERVISOR_XO_NAME);
    }

    private async getXOResponse(apiPath: string, payload: any, connectionName : string): Promise<any> {
        let response = null;
        try {
            let globalContext = await this.getGlobalContext();
            let containerCtx = await globalContext.getContainerContext();
            let extensionContext = await globalContext.getExtensionContext(containerCtx.extensionName);
            let connectionCollection = await extensionContext.getConnections(connectionName);
            const connection = connectionCollection.get(connectionName);
            connection.open(this.POST_METHOD, apiPath);
            connection.setContentType(this.REST_CONTENT_TYPE);
            let needRetry = false;
            try {
                response = await connection.send(JSON.stringify(payload));
            } catch (exceptionVar) {
                this.addLog("Error occurred in " + apiPath + " call :" + exceptionVar, LogTypes.ERROR);
                if (exceptionVar.status == 504) {
                    needRetry = true;
                }
            }
            if (needRetry) {
                this.addLog("Retry due to timeout " + apiPath, LogTypes.ERROR);
                response = await connection.send(JSON.stringify(payload));
            }


        } catch (error) {
            this.addLog("Error while calling " + apiPath + " model" + error, LogTypes.ERROR);

        }
        return response;

    }

    async callPOSTRestAPI(payload: any, url: string): Promise<Response> {
        let response = await fetch(url, {
            method: this.POST_METHOD,
            headers: {
                'Content-Type': this.REST_CONTENT_TYPE,
                'OSvC-CREST-Application-Context': 'Chat Sentiment save child',
                'Authorization': `Session ${this.sessionToken}`
            },
            body: JSON.stringify(payload)
        });
        return response;

    }

    async updateParent(payload: Object, id: number): Promise<Response> {
        let response = await fetch(this.CX_SERVICE_URL + this.CX_API_URL_PATH + this.CX_PARENT_OBJECT_URL + id, {
            method: 'PATCH',
            headers: {
                'Content-Type': this.REST_CONTENT_TYPE,
                'OSvC-CREST-Application-Context': 'Chat Sentiment Analysis update',
                'Authorization': `Session ${this.sessionToken}`
            },
            body: JSON.stringify(payload)
        });
        return response;

    }

    unEscape(htmlStr: string): string {
        let textarea = this.txtConvertElement;
        textarea.innerHTML = htmlStr;
        return textarea.value?.replace(/(<([^>]+)>)/ig, '');
    }

    async makeGETAPIRequest(url: string): Promise<Response> {
        let response = await fetch(this.CX_SERVICE_URL + this.CX_API_URL_PATH + url, {
            method: 'GET',
            headers: {
                'Content-Type': this.REST_CONTENT_TYPE,
                'OSvC-CREST-Application-Context': 'Chat Sentiment Analysis',
                'Authorization': `Session ${this.sessionToken}`
            }
        });
        return response;
    }

    async processConfigurations(item: any): Promise<void> {
        let response = await this.makeGETAPIRequest('configurations/' + item.id);
        let responsebody = await response.json();
        let config = JSON.parse(responsebody?.value);
        this.INITIAL_MESSAGES_TO_SKIP = config?.INITIAL_MESSAGES_TO_SKIP;
        this.MAX_NEGATIVE_CHAT_COUNT = config?.MAX_NEGATIVE_CHAT_COUNT;
        this.MIN_REQUEST_MANAGER_CONFIG = config?.MIN_REQUEST_MANAGER_CONFIG;
        this.MIN_EMOTION_CONFIG = config?.MIN_EMOTION_CONFIG;
        this.IS_EMOTION_ACTIVE = config?.IS_EMOTION_ACTIVE;
        this.IS_MANAGER_ASK_ACTIVE = config?.IS_MANAGER_ASK_ACTIVE;
        this.EMOTION_XO_NAME = config?.EMOTION_XO_NAME;
        this.SUPERVISOR_XO_NAME = config?.SUPERVISOR_XO_NAME;
        this.SUPERVISOR_ENDPOINT = config?.SUPERVISOR_ENDPOINT;
        this.EMOTION_ENDPOINT = config?.EMOTION_ENDPOINT;
        this.AGENT_EMOTION_ENDPOINT = config?.AGENT_EMOTION_ENDPOINT;
        if(config?.NEGATIVE_LOOKUPNAME) {
            this.NEGATIVE_LOOKUPNAME = config?.NEGATIVE_LOOKUPNAME;
        }
        if(config?.POSITIVE_LOOKUPNAME) {
            this.POSITIVE_LOOKUPNAME = config?.POSITIVE_LOOKUPNAME;
        }
        if(config?.NEUTRAL_LOOKUPNAME) {
            this.NEUTRAL_LOOKUPNAME = config?.NEUTRAL_LOOKUPNAME;
        }
    }

    public addLog(message: string, logType: LogTypes): void {
        ORACLE_SERVICE_CLOUD.extension_loader.load(this.APP_NAME).then((extensionProvider: IExtensionProvider) => {
            const defaultLogger: IExtensionLogger = extensionProvider.getLogger();
            switch (logType) {
                case LogTypes.ERROR:
                    defaultLogger.error(message);
                    break;
                case LogTypes.WARN:
                    defaultLogger.warn(message);
                    break;
                case LogTypes.INFO:
                    defaultLogger.info(message);
                    break;
                case LogTypes.TRACE:
                    defaultLogger.trace(message);
                    break;
                default:
                    defaultLogger.warn(message);
            }
        });
    }
}

(async () => {
    await new ChatEmotion().initialize();
})()