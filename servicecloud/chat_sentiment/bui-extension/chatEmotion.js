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
 *  SHA1: $Id: 200501edc94013e5a68003c350f863cc678f9a40 $
 * *********************************************************************************************
 *  File: chatEmotion.js
 * ****************************************************************************************** */

"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (g && (g = 0, op[0] && (_ = 0)), _) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
var LogTypes;
(function (LogTypes) {
    LogTypes["ERROR"] = "Error";
    LogTypes["WARN"] = "Warn";
    LogTypes["DEBUG"] = "Debug";
    LogTypes["TRACE"] = "Trace";
    LogTypes["INFO"] = "Info";
})(LogTypes || (LogTypes = {}));
var RoleTypes;
(function (RoleTypes) {
    RoleTypes["AGENT"] = "AGENT";
    RoleTypes["END_USER"] = "END_USER";
    RoleTypes["SYSTEM"] = "SYSTEM";
    RoleTypes["NOT_ENDUSER"] = "NOT_ENDUSER";
    RoleTypes["CONFEREE"] = "CONFEREE";
})(RoleTypes || (RoleTypes = {}));
var NEGATIVE_EMOTION = 2;
var POSITIVE_EMOTION = 1;
var ChatEmotion = (function () {
    function ChatEmotion() {
        this.addedListener = false;
        this.CUSTOM_CFG_EMOTION = "CUSTOM_CFG_EMOTION";
        this.POSITIVE_LOOKUPNAME = 'Positive';
        this.NEGATIVE_LOOKUPNAME = 'Negative';
        this.NEUTRAL_LOOKUPNAME = 'Neutral';
        this.CX_API_VERSION = "v1.4";
        this.CX_API_URL_PATH = "/connect/".concat(this.CX_API_VERSION, "/");
        this.CX_SERVICE_URL = "";
        this.CX_GET_PARENT_REPORT = "analyticsReportResults/";
        this.CX_PARENT_OBJECT_URL = "AIML.ChatAIResultSummary/";
        this.CX_CHILD_OBJECT_URL = "AIML.ChatAIPredictionInfo/";
        this.CX_CONFIG_URL = "configurations";
        this.engagementDetails = {};
        this.engagementWithActionRegistered = new Set();
        this.APP_NAME = "chat_emotion";
        this.globalContextPromise = null;
        this.extensionProviderPromise = null;
        this.txtConvertElement = document.createElement("textarea");
        this.EN_IN_FORMAT = 'en-IN';
        this.FRACTION_LENGTH = 2;
        this.HUNDRED_MULTIPLIER = 100;
        this.POST_METHOD = 'POST';
        this.GET_METHOD = 'GET';
        this.REST_CONTENT_TYPE = 'application/json';
        this.AS_WAIT_MAX_TIME = 20000;
        this.TIME_OUT_INTERVAL = 200;
    }
    ChatEmotion.prototype.getExtensionProvider = function () {
        return __awaiter(this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        if (this.extensionProviderPromise == null) {
                            this.extensionProviderPromise = ORACLE_SERVICE_CLOUD.extension_loader.load(this.APP_NAME);
                        }
                        return [4, this.extensionProviderPromise];
                    case 1: return [2, _a.sent()];
                }
            });
        });
    };
    ChatEmotion.prototype.getGlobalContext = function () {
        return __awaiter(this, void 0, void 0, function () {
            var _a;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        if (!(this.globalContextPromise == null)) return [3, 2];
                        _a = this;
                        return [4, this.getExtensionProvider()];
                    case 1:
                        _a.globalContextPromise = (_b.sent()).getGlobalContext();
                        _b.label = 2;
                    case 2: return [4, this.globalContextPromise];
                    case 3: return [2, _b.sent()];
                }
            });
        });
    };
    ChatEmotion.prototype.getWorkspaceRecord = function (objectId, type) {
        return __awaiter(this, void 0, void 0, function () {
            var extensionProvide, workspaceRecordPromise;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4, this.getExtensionProvider()];
                    case 1:
                        extensionProvide = _a.sent();
                        workspaceRecordPromise = new ExtensionPromise();
                        extensionProvide.registerWorkspaceExtension(function (workspaceRecord) {
                            workspaceRecordPromise.resolve(workspaceRecord);
                        }, type, objectId);
                        return [4, workspaceRecordPromise];
                    case 2: return [2, _a.sent()];
                }
            });
        });
    };
    ChatEmotion.prototype.waitUntil = function (funcRef) {
        return __awaiter(this, void 0, void 0, function () {
            var retunValue;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        retunValue = false;
                        _a.label = 1;
                    case 1:
                        if (!!retunValue) return [3, 4];
                        retunValue = funcRef();
                        if (!!retunValue) return [3, 3];
                        return [4, Promise.resolve()];
                    case 2:
                        _a.sent();
                        _a.label = 3;
                    case 3: return [3, 1];
                    case 4: return [2, retunValue];
                }
            });
        });
    };
    ChatEmotion.prototype.initialize = function () {
        return __awaiter(this, void 0, void 0, function () {
            var globalContext, chatAPIInfo, _a;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0: return [4, this.getGlobalContext()];
                    case 1:
                        globalContext = _b.sent();
                        return [4, this.waitUntil(function () { return globalContext.getChatAPIInfo(); })];
                    case 2:
                        chatAPIInfo = _b.sent();
                        return [4, ORACLE_SERVICE_CLOUD.scriptLoader.loadScript([chatAPIInfo.getChatAPIURL()])];
                    case 3:
                        _b.sent();
                        _a = this;
                        return [4, globalContext.getSessionToken()];
                    case 4:
                        _a.sessionToken = _b.sent();
                        this.addLog("call to get agent session", LogTypes.DEBUG);
                        return [4, this.loadAndSetupAgentChatSentiment()];
                    case 5:
                        _b.sent();
                        this.addLog("call to get agent session end", LogTypes.DEBUG);
                        return [4, this.loadSentimentLabels()];
                    case 6:
                        _b.sent();
                        return [2];
                }
            });
        });
    };
    ChatEmotion.prototype.loadSentimentLabels = function () {
        return __awaiter(this, void 0, void 0, function () {
            var emotionList, emotionListJson, emotions, self;
            var _this = this;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4, this.makeGETAPIRequest("AIML.Emotion")];
                    case 1:
                        emotionList = _a.sent();
                        if (!(emotionList != null)) return [3, 3];
                        return [4, emotionList.json()];
                    case 2:
                        emotionListJson = _a.sent();
                        emotions = emotionListJson === null || emotionListJson === void 0 ? void 0 : emotionListJson.items;
                        self = this;
                        emotions.forEach(function (item) { return __awaiter(_this, void 0, void 0, function () {
                            var _a, _b, _c;
                            return __generator(this, function (_d) {
                                if (item.lookupName.toLowerCase() == ((_a = self.POSITIVE_LOOKUPNAME) === null || _a === void 0 ? void 0 : _a.toLowerCase())) {
                                    self.POSITIVE_LOOKUPNAME = item.lookupName;
                                }
                                else if (item.lookupName.toLowerCase() == ((_b = self.NEGATIVE_LOOKUPNAME) === null || _b === void 0 ? void 0 : _b.toLowerCase())) {
                                    self.NEGATIVE_LOOKUPNAME = item.lookupName;
                                }
                                else if (item.lookupName.toLowerCase() == ((_c = self.NEUTRAL_LOOKUPNAME) === null || _c === void 0 ? void 0 : _c.toLowerCase())) {
                                    self.NEUTRAL_LOOKUPNAME = item.lookupName;
                                }
                                return [2];
                            });
                        }); });
                        _a.label = 3;
                    case 3: return [2];
                }
            });
        });
    };
    ChatEmotion.prototype.loadAndSetupAgentChatSentiment = function () {
        return __awaiter(this, void 0, void 0, function () {
            var as;
            var _this = this;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4, this.loadConfigurations()];
                    case 1:
                        _a.sent();
                        return [4, this.waitUntilAgentSessionIsLoaded(this.AS_WAIT_MAX_TIME)];
                    case 2:
                        as = _a.sent();
                        return [4, as.sessionStatusChanged(function (statInfo) { return __awaiter(_this, void 0, void 0, function () {
                                return __generator(this, function (_a) {
                                    switch (_a.label) {
                                        case 0:
                                            if (!(statInfo.LoggedIn && !this.addedListener)) return [3, 2];
                                            return [4, this.agentSessionEventsHandler()];
                                        case 1:
                                            _a.sent();
                                            _a.label = 2;
                                        case 2: return [2];
                                    }
                                });
                            }); })];
                    case 3:
                        _a.sent();
                        return [2];
                }
            });
        });
    };
    ChatEmotion.prototype.waitUntilAgentSessionIsLoaded = function (maxWaitTime) {
        return __awaiter(this, void 0, void 0, function () {
            var retry, agentSession, error_1;
            var _this = this;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        retry = true;
                        _a.label = 1;
                    case 1:
                        if (!retry) return [3, 6];
                        _a.label = 2;
                    case 2:
                        _a.trys.push([2, 4, , 5]);
                        return [4, new Promise(function (resolve, reject) { return __awaiter(_this, void 0, void 0, function () {
                                var timeoutMaxTime, timeoutRef, agentSession;
                                return __generator(this, function (_a) {
                                    switch (_a.label) {
                                        case 0:
                                            timeoutMaxTime = window.setTimeout(function () {
                                                retry = false;
                                                reject(null);
                                            }, maxWaitTime);
                                            timeoutRef = window.setTimeout(function () {
                                                reject(null);
                                            }, this.TIME_OUT_INTERVAL);
                                            return [4, IOracleChatClient.getAgentSession()];
                                        case 1:
                                            agentSession = _a.sent();
                                            if (agentSession === undefined) {
                                                reject(null);
                                            }
                                            window.clearTimeout(timeoutRef);
                                            window.clearTimeout(timeoutMaxTime);
                                            resolve(agentSession);
                                            return [2];
                                    }
                                });
                            }); })];
                    case 3:
                        agentSession = _a.sent();
                        retry = false;
                        return [2, agentSession];
                    case 4:
                        error_1 = _a.sent();
                        this.addLog("Inside get agent session :" + error_1, LogTypes.ERROR);
                        return [3, 5];
                    case 5: return [3, 1];
                    case 6: return [2];
                }
            });
        });
    };
    ChatEmotion.prototype.loadConfigurations = function () {
        return __awaiter(this, void 0, void 0, function () {
            var globalContext, configurationListResponse, configurationListResponseJson, configurationList;
            var _this = this;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4, this.getGlobalContext()];
                    case 1:
                        globalContext = _a.sent();
                        this.CX_SERVICE_URL = globalContext.getInterfaceServiceUrl('REST');
                        return [4, this.makeGETAPIRequest(this.CX_CONFIG_URL)];
                    case 2:
                        configurationListResponse = _a.sent();
                        if (!(configurationListResponse != null)) return [3, 4];
                        return [4, configurationListResponse.json()];
                    case 3:
                        configurationListResponseJson = _a.sent();
                        configurationList = configurationListResponseJson === null || configurationListResponseJson === void 0 ? void 0 : configurationListResponseJson.items;
                        configurationList.forEach(function (item) { return __awaiter(_this, void 0, void 0, function () {
                            return __generator(this, function (_a) {
                                switch (_a.label) {
                                    case 0:
                                        if (!(item.lookupName == this.CUSTOM_CFG_EMOTION)) return [3, 2];
                                        return [4, this.processConfigurations(item)];
                                    case 1:
                                        _a.sent();
                                        _a.label = 2;
                                    case 2: return [2];
                                }
                            });
                        }); });
                        _a.label = 4;
                    case 4: return [2];
                }
            });
        });
    };
    ChatEmotion.prototype.agentSessionEventsHandler = function () {
        return __awaiter(this, void 0, void 0, function () {
            var agentSession, engagement, hasCurrentEngagement, exceptionVar_1, engagementId, _a, parentAlreadyPresent, parentId;
            var _this = this;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0: return [4, this.waitUntilAgentSessionIsLoaded(this.AS_WAIT_MAX_TIME)];
                    case 1:
                        agentSession = _b.sent();
                        this.addLog("Inside get agent session", LogTypes.DEBUG);
                        this.addLog("status change event: inside AVAILABLE", LogTypes.DEBUG);
                        engagement = null;
                        hasCurrentEngagement = false;
                        if (!(this.IS_MANAGER_ASK_ACTIVE || this.IS_EMOTION_ACTIVE)) return [3, 10];
                        agentSession.engagementRemoved(function (engRemovedEvtArgs) { return _this.engagementRemovedHandler(engRemovedEvtArgs); });
                        agentSession.engagementAssigned(function (eaEvt) { return _this.engagementAssignedHandler(eaEvt); });
                        agentSession.engagementAccepted(function (eaEvt) { return __awaiter(_this, void 0, void 0, function () { return __generator(this, function (_a) {
                            return [2, this.engagementAcceptedHandler(eaEvt, agentSession)];
                        }); }); });
                        this.addedListener = true;
                        _b.label = 2;
                    case 2:
                        _b.trys.push([2, 4, , 5]);
                        return [4, agentSession.getCurrentEngagement()];
                    case 3:
                        engagement = _b.sent();
                        hasCurrentEngagement = true;
                        return [3, 5];
                    case 4:
                        exceptionVar_1 = _b.sent();
                        this.addLog("no engagement present for chat", LogTypes.DEBUG);
                        return [3, 5];
                    case 5:
                        if (!hasCurrentEngagement) return [3, 10];
                        _a = Number;
                        return [4, engagement.getEngagementId()];
                    case 6:
                        engagementId = _a.apply(void 0, [_b.sent()]);
                        this.engagementDetails[engagementId] = {
                            assignedTime: new Date()
                        };
                        if (engagement.MyRole == RoleTypes.CONFEREE) {
                            this.engagementDetails[engagementId].isConference = true;
                        }
                        return [4, this.checkParentAlreadyPresent(engagementId, true)];
                    case 7:
                        parentAlreadyPresent = _b.sent();
                        if (parentAlreadyPresent['count'] > 0) {
                            parentId = parentAlreadyPresent['rows'][0][0];
                            this.engagementDetails[engagementId].parentId = parentId;
                        }
                        return [4, this.createParentEntry(engagementId)];
                    case 8:
                        _b.sent();
                        return [4, this.addEngagementCallback(agentSession, engagementId, parentAlreadyPresent)];
                    case 9:
                        _b.sent();
                        _b.label = 10;
                    case 10: return [2];
                }
            });
        });
    };
    ChatEmotion.prototype.engagementAssignedHandler = function (eaEvt) {
        var contactInfo = eaEvt.ContactInfo;
        this.addLog("Inside engagementAssigned", LogTypes.DEBUG);
        var agentInfo = eaEvt.AgentInfo;
        this.engagementDetails[eaEvt.EngagementId] = {
            accountId: agentInfo.AccountId ? parseInt(agentInfo.AccountId) : 0,
            contactEmail: eaEvt.ContactInfo.EmailAddress,
            agentName: agentInfo.Name,
            agentId: agentInfo.AccountId,
            displayName: "",
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
    };
    ChatEmotion.prototype.checkParentAlreadyPresent = function (chatId, getActive) {
        return __awaiter(this, void 0, void 0, function () {
            var payload, ISACTIVE_FLAG, checkForParentUrl, response;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        payload = {
                            lookupName: "CheckForParentRecord",
                            filters: [{
                                    name: "Chat Id",
                                    values: [chatId === null || chatId === void 0 ? void 0 : chatId.toString()]
                                }]
                        };
                        if (getActive) {
                            ISACTIVE_FLAG = "1";
                            payload.filters = [{
                                    name: "Chat Id",
                                    values: [chatId === null || chatId === void 0 ? void 0 : chatId.toString()]
                                }, {
                                    name: "Active",
                                    values: [ISACTIVE_FLAG]
                                }];
                        }
                        checkForParentUrl = this.CX_SERVICE_URL + this.CX_API_URL_PATH + this.CX_GET_PARENT_REPORT;
                        return [4, this.callPostReportAPI(checkForParentUrl, payload)];
                    case 1:
                        response = _a.sent();
                        return [4, response.json()];
                    case 2: return [2, _a.sent()];
                }
            });
        });
    };
    ChatEmotion.prototype.checkChatDBTime = function (chatId) {
        return __awaiter(this, void 0, void 0, function () {
            var payload, checkForParentUrl, response;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        payload = {
                            lookupName: "ChatDurationDetail",
                            filters: [{
                                    name: "ChatId",
                                    values: [chatId === null || chatId === void 0 ? void 0 : chatId.toString()]
                                }]
                        };
                        checkForParentUrl = this.CX_SERVICE_URL + this.CX_API_URL_PATH + this.CX_GET_PARENT_REPORT;
                        return [4, this.callPostReportAPI(checkForParentUrl, payload)];
                    case 1:
                        response = _a.sent();
                        return [4, response.json()];
                    case 2: return [2, _a.sent()];
                }
            });
        });
    };
    ChatEmotion.prototype.callPostReportAPI = function (checkForParentUrl, payload) {
        return __awaiter(this, void 0, void 0, function () {
            var apiResponse, error_2;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        _a.trys.push([0, 2, , 3]);
                        return [4, fetch(checkForParentUrl, {
                                method: this.POST_METHOD,
                                headers: {
                                    'Content-Type': this.REST_CONTENT_TYPE,
                                    'OSvC-CREST-Application-Context': 'Chat Sentiment Analysis',
                                    'OSvC-CREST-Time-UTC': 'true',
                                    'Authorization': "Session ".concat(this.sessionToken)
                                },
                                body: JSON.stringify(payload)
                            })];
                    case 1:
                        apiResponse = _a.sent();
                        return [3, 3];
                    case 2:
                        error_2 = _a.sent();
                        this.addLog("Error while calling " + checkForParentUrl + " api " + error_2, LogTypes.ERROR);
                        return [3, 3];
                    case 3: return [2, apiResponse];
                }
            });
        });
    };
    ChatEmotion.prototype.callGETAPI = function (apiUrl) {
        return __awaiter(this, void 0, void 0, function () {
            var apiResponse, error_3;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        _a.trys.push([0, 2, , 3]);
                        return [4, fetch(apiUrl, {
                                method: this.GET_METHOD,
                                headers: {
                                    'Content-Type': this.REST_CONTENT_TYPE,
                                    'OSvC-CREST-Application-Context': 'Chat Sentiment Analysis',
                                    'Authorization': "Session ".concat(this.sessionToken)
                                }
                            })];
                    case 1:
                        apiResponse = _a.sent();
                        return [3, 3];
                    case 2:
                        error_3 = _a.sent();
                        this.addLog("Error while calling " + apiUrl + " api " + error_3, LogTypes.ERROR);
                        return [3, 3];
                    case 3: return [2, apiResponse];
                }
            });
        });
    };
    ChatEmotion.prototype.saveInitialParentEntry = function (engagementId) {
        return __awaiter(this, void 0, void 0, function () {
            var saveParentResponse, payload;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        saveParentResponse = null;
                        payload = null;
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
                        return [4, this.callPOSTRestAPI(payload, this.CX_SERVICE_URL + this.CX_API_URL_PATH + this.CX_PARENT_OBJECT_URL)];
                    case 1:
                        saveParentResponse = _a.sent();
                        return [2, saveParentResponse === null || saveParentResponse === void 0 ? void 0 : saveParentResponse.json()];
                }
            });
        });
    };
    ChatEmotion.prototype.evaluateAndSaveChatResult = function (chatMessage, engagementId, chatOrder, isThisPrivateChat) {
        var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m, _o, _p;
        return __awaiter(this, void 0, void 0, function () {
            var saveSentimentResponse, payload, messageBody, emotionSore, emotion, supervisor, supervisorScore, emotionResponsePromise, supervisorResponsePromise, emotionResponse, emotionValue, supervisorResponse, supervisorAsk;
            return __generator(this, function (_q) {
                switch (_q.label) {
                    case 0:
                        saveSentimentResponse = null;
                        payload = null;
                        messageBody = this.unEscape(chatMessage.Body);
                        emotionSore = 0;
                        emotion = null;
                        supervisor = null;
                        supervisorScore = 0;
                        if (!(chatMessage.Sender != RoleTypes.SYSTEM)) return [3, 8];
                        emotionResponsePromise = void 0;
                        supervisorResponsePromise = void 0;
                        if (!this.IS_EMOTION_ACTIVE) return [3, 2];
                        if (!(chatMessage.Sender == RoleTypes.END_USER || (chatMessage.Sender == RoleTypes.AGENT && this.AGENT_EMOTION_ENDPOINT != undefined && this.AGENT_EMOTION_ENDPOINT != null))) return [3, 2];
                        return [4, this.getEmotion(messageBody, chatMessage.Sender)];
                    case 1:
                        emotionResponsePromise = _q.sent();
                        _q.label = 2;
                    case 2:
                        if (!(this.IS_MANAGER_ASK_ACTIVE && chatMessage.Sender == RoleTypes.END_USER)) return [3, 4];
                        return [4, this.getSupervisorAsk(messageBody)];
                    case 3:
                        supervisorResponsePromise = _q.sent();
                        _q.label = 4;
                    case 4: return [4, emotionResponsePromise];
                    case 5:
                        emotionResponse = _q.sent();
                        if (emotionResponse === null || emotionResponse === void 0 ? void 0 : emotionResponse.hasOwnProperty("documents")) {
                            emotionValue = ((_b = (_a = emotionResponse === null || emotionResponse === void 0 ? void 0 : emotionResponse.documents[0]) === null || _a === void 0 ? void 0 : _a.textClassification[0]) === null || _b === void 0 ? void 0 : _b.label) == NEGATIVE_EMOTION ? this.NEGATIVE_LOOKUPNAME : ((_d = (_c = emotionResponse === null || emotionResponse === void 0 ? void 0 : emotionResponse.documents[0]) === null || _c === void 0 ? void 0 : _c.textClassification[0]) === null || _d === void 0 ? void 0 : _d.label) == POSITIVE_EMOTION ? this.POSITIVE_LOOKUPNAME : this.NEUTRAL_LOOKUPNAME;
                            emotionSore = ((_f = (_e = emotionResponse === null || emotionResponse === void 0 ? void 0 : emotionResponse.documents[0]) === null || _e === void 0 ? void 0 : _e.textClassification[0]) === null || _f === void 0 ? void 0 : _f.score) == undefined ? 0 : (_h = (_g = emotionResponse === null || emotionResponse === void 0 ? void 0 : emotionResponse.documents[0]) === null || _g === void 0 ? void 0 : _g.textClassification[0]) === null || _h === void 0 ? void 0 : _h.score;
                            emotion = emotionValue;
                        }
                        else if (emotionResponse === null || emotionResponse === void 0 ? void 0 : emotionResponse.hasOwnProperty("error")) {
                            this.addLog("Error while calling emotion model", LogTypes.ERROR);
                        }
                        if (!(chatMessage.Sender == RoleTypes.END_USER)) return [3, 7];
                        return [4, supervisorResponsePromise];
                    case 6:
                        supervisorResponse = _q.sent();
                        if (supervisorResponse === null || supervisorResponse === void 0 ? void 0 : supervisorResponse.hasOwnProperty("documents")) {
                            supervisor = (_k = (_j = supervisorResponse === null || supervisorResponse === void 0 ? void 0 : supervisorResponse.documents[0]) === null || _j === void 0 ? void 0 : _j.textClassification[0]) === null || _k === void 0 ? void 0 : _k.label;
                            supervisorScore = ((_m = (_l = supervisorResponse === null || supervisorResponse === void 0 ? void 0 : supervisorResponse.documents[0]) === null || _l === void 0 ? void 0 : _l.textClassification[0]) === null || _m === void 0 ? void 0 : _m.score) == undefined ? 0 : (_p = (_o = supervisorResponse === null || supervisorResponse === void 0 ? void 0 : supervisorResponse.documents[0]) === null || _o === void 0 ? void 0 : _o.textClassification[0]) === null || _p === void 0 ? void 0 : _p.score;
                        }
                        else if (supervisorResponse === null || supervisorResponse === void 0 ? void 0 : supervisorResponse.hasOwnProperty("error")) {
                            this.addLog("Error while calling supervisor model", LogTypes.ERROR);
                        }
                        _q.label = 7;
                    case 7:
                        supervisorAsk = supervisor == null ? null : (supervisor == 1 && ((supervisorScore * this.HUNDRED_MULTIPLIER) > this.MIN_REQUEST_MANAGER_CONFIG)) ? true : false;
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
                        return [3, 9];
                    case 8:
                        payload = {
                            ChatAIResultSummary: { id: isThisPrivateChat ? this.engagementDetails[engagementId].confParentId : this.engagementDetails[engagementId].parentId },
                            ChatText: messageBody,
                            ChatOrder: chatOrder,
                            RequestManagerIntervene: false,
                            RequestManagerInterveneConf: '0.00',
                            EmotionConf: '0.00',
                            ChatRole: { lookupName: chatMessage.Sender }
                        };
                        _q.label = 9;
                    case 9: return [4, this.callPOSTRestAPI(payload, this.CX_SERVICE_URL + this.CX_API_URL_PATH + this.CX_CHILD_OBJECT_URL)];
                    case 10:
                        saveSentimentResponse = _q.sent();
                        return [2, saveSentimentResponse];
                }
            });
        });
    };
    ChatEmotion.prototype.engagementAcceptedHandler = function (eaEvt, as) {
        var _a;
        return __awaiter(this, void 0, void 0, function () {
            var nameResponse, nameResponseJson, parentAlreadyPresent, engagement;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        this.addLog("Inside engagementAccepted", LogTypes.DEBUG);
                        if (!(this.engagementDetails[eaEvt.EngagementId].displayName == "")) return [3, 3];
                        return [4, this.callGETAPI(this.CX_SERVICE_URL + this.CX_API_URL_PATH + "accounts/" + this.engagementDetails[eaEvt.EngagementId].agentId)];
                    case 1:
                        nameResponse = _b.sent();
                        return [4, nameResponse.json()];
                    case 2:
                        nameResponseJson = _b.sent();
                        if (nameResponseJson === null || nameResponseJson === void 0 ? void 0 : nameResponseJson.hasOwnProperty("displayName")) {
                            this.engagementDetails[eaEvt.EngagementId].displayName = nameResponseJson === null || nameResponseJson === void 0 ? void 0 : nameResponseJson.displayName;
                        }
                        _b.label = 3;
                    case 3:
                        if (!((_a = this.engagementDetails[eaEvt.EngagementId].displayName) === null || _a === void 0 ? void 0 : _a.includes(eaEvt.AgentName))) return [3, 8];
                        this.addLog("Inside ACCEPTED event", LogTypes.DEBUG);
                        return [4, this.checkParentAlreadyPresent(eaEvt.EngagementId, false)];
                    case 4:
                        parentAlreadyPresent = _b.sent();
                        return [4, as.getEngagement(eaEvt.EngagementId)];
                    case 5:
                        engagement = _b.sent();
                        if (engagement.MyRole == RoleTypes.CONFEREE) {
                            this.engagementDetails[eaEvt.EngagementId].isConference = true;
                        }
                        return [4, this.createParentEntry(eaEvt.EngagementId)];
                    case 6:
                        _b.sent();
                        this.addLog(" Inside engagementAccepted before getEngagement" + eaEvt.EngagementId, LogTypes.DEBUG);
                        return [4, this.addEngagementCallback(as, eaEvt.EngagementId, parentAlreadyPresent)];
                    case 7:
                        _b.sent();
                        _b.label = 8;
                    case 8: return [2];
                }
            });
        });
    };
    ChatEmotion.prototype.addEngagementCallback = function (as, engagementId, parentAlreadyPresent) {
        return __awaiter(this, void 0, void 0, function () {
            var engagement, messages;
            var _this = this;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4, as.getEngagement(engagementId)];
                    case 1:
                        engagement = _a.sent();
                        if (!this.engagementWithActionRegistered.has(engagementId)) {
                            engagement.messagePosted(function (chatMessage) { return __awaiter(_this, void 0, void 0, function () {
                                var chatOrder, messagePostedTime, isThisPrivateChat, parentAlreadyPresent_1, i, lastParent;
                                return __generator(this, function (_a) {
                                    switch (_a.label) {
                                        case 0:
                                            chatOrder = this.nextChatOrder(engagementId);
                                            messagePostedTime = new Date(chatMessage.PostTime);
                                            this.checkAndUpdateUserIdToParent(engagementId);
                                            if (!(messagePostedTime > this.engagementDetails[engagementId].assignedTime)) return [3, 7];
                                            this.addLog("Inside message posted :", LogTypes.DEBUG);
                                            isThisPrivateChat = false;
                                            if (!(this.engagementDetails[engagementId].isConference || chatMessage.Visibility == RoleTypes.NOT_ENDUSER)) return [3, 2];
                                            if (chatMessage.Visibility == RoleTypes.NOT_ENDUSER) {
                                                isThisPrivateChat = true;
                                            }
                                            if (!(this.engagementDetails[engagementId].confParentId == 0)) return [3, 2];
                                            return [4, this.checkParentAlreadyPresent(engagementId, true)];
                                        case 1:
                                            parentAlreadyPresent_1 = _a.sent();
                                            if (parentAlreadyPresent_1['count'] > 1) {
                                                i = 1;
                                                lastParent = 0;
                                                while (i < parentAlreadyPresent_1['rows'].length) {
                                                    this.engagementDetails[engagementId].confParentIdSet.add(Number(parentAlreadyPresent_1['rows'][i][0]));
                                                    lastParent = Number(parentAlreadyPresent_1['rows'][i][0]);
                                                    i++;
                                                }
                                                this.engagementDetails[engagementId].confParentId = lastParent;
                                            }
                                            _a.label = 2;
                                        case 2:
                                            if (!(this.engagementDetails[engagementId].isConference && isThisPrivateChat)) return [3, 5];
                                            if (!this.engagementDetails[engagementId].confParentIdSet.has(this.engagementDetails[engagementId].parentId)) return [3, 4];
                                            return [4, this.evaluateAndSaveChatResult(chatMessage, engagementId, chatOrder, isThisPrivateChat)];
                                        case 3:
                                            _a.sent();
                                            this.addLog("save messagePosted chat with id :" + engagementId, LogTypes.DEBUG);
                                            _a.label = 4;
                                        case 4: return [3, 7];
                                        case 5:
                                            if (!(!this.engagementDetails[engagementId].isConference && !isThisPrivateChat)) return [3, 7];
                                            return [4, this.evaluateAndSaveChatResult(chatMessage, engagementId, chatOrder, false)];
                                        case 6:
                                            _a.sent();
                                            this.addLog("save messagePosted chat with id :" + engagementId, LogTypes.DEBUG);
                                            _a.label = 7;
                                        case 7: return [2];
                                    }
                                });
                            }); });
                            engagement.concluded(function (onConcluded) { return __awaiter(_this, void 0, void 0, function () {
                                return __generator(this, function (_a) {
                                    switch (_a.label) {
                                        case 0:
                                            this.addLog("Engagement has been concluded." + onConcluded, LogTypes.DEBUG);
                                            return [4, this.saveConcludedChatResult("Engagement has been concluded.", engagementId, false)];
                                        case 1:
                                            _a.sent();
                                            return [2];
                                    }
                                });
                            }); });
                            this.engagementWithActionRegistered.add(engagementId);
                        }
                        return [4, engagement.getMessages()];
                    case 2:
                        messages = _a.sent();
                        this.addLog("process getMessages: ", LogTypes.DEBUG);
                        if (parentAlreadyPresent['count'] == 0) {
                            messages.forEach(function (message) { return __awaiter(_this, void 0, void 0, function () {
                                var chatOrder;
                                return __generator(this, function (_a) {
                                    switch (_a.label) {
                                        case 0:
                                            chatOrder = this.nextChatOrder(engagementId);
                                            return [4, this.evaluateAndSaveChatResult(message, engagementId, chatOrder, false)];
                                        case 1:
                                            _a.sent();
                                            this.addLog("save get message chat  with id : ", LogTypes.DEBUG);
                                            return [2];
                                    }
                                });
                            }); });
                        }
                        this.checkAndUpdateUserIdToParent(engagementId);
                        return [2];
                }
            });
        });
    };
    ChatEmotion.prototype.checkAndUpdateUserIdToParent = function (engagementId) {
        var _this = this;
        if (this.engagementDetails[engagementId].userId <= 0) {
            var entityList = ORACLE_SERVICE_CLOUD.globalContextListener.globalContextData.entityList;
            entityList.forEach(function (entity) { return __awaiter(_this, void 0, void 0, function () {
                var objectType, objectId, ws_1, workGroupContext, workGroupEntity;
                var _this = this;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            objectType = entity.objectType;
                            objectId = entity.objectId;
                            if (!(objectType == "Interaction")) return [3, 4];
                            return [4, this.getWorkspaceRecord(objectId, objectType)];
                        case 1:
                            ws_1 = _a.sent();
                            return [4, ws_1.getWorkGroupContext()];
                        case 2:
                            workGroupContext = _a.sent();
                            return [4, workGroupContext.getWorkGroupEntities()];
                        case 3:
                            workGroupEntity = _a.sent();
                            workGroupEntity.forEach(function (workEntity) { return __awaiter(_this, void 0, void 0, function () {
                                var fields, value;
                                return __generator(this, function (_a) {
                                    switch (_a.label) {
                                        case 0:
                                            if (!(workEntity.objectId == engagementId && workEntity.objectType == 'Chat')) return [3, 2];
                                            return [4, ws_1.getFieldValues(['Interaction.CId'])];
                                        case 1:
                                            fields = _a.sent();
                                            value = Number(fields.getField('Interaction.CId').getValue());
                                            if (value > 0) {
                                                this.updateParentWithContact(value, engagementId);
                                            }
                                            _a.label = 2;
                                        case 2: return [2];
                                    }
                                });
                            }); });
                            _a.label = 4;
                        case 4: return [2];
                    }
                });
            }); });
        }
    };
    ChatEmotion.prototype.updateParentWithContact = function (value, engagementId) {
        var payload = {
            ContactId: { id: value }
        };
        this.updateParent(payload, this.engagementDetails[engagementId].parentId);
        this.engagementDetails[engagementId].userId = value;
    };
    ChatEmotion.prototype.nextChatOrder = function (engagementId) {
        var chatOrder = this.engagementDetails[engagementId].chatOrder;
        this.engagementDetails[engagementId].chatOrder = chatOrder + 1;
        return chatOrder;
    };
    ChatEmotion.prototype.createParentEntry = function (engagementId) {
        return __awaiter(this, void 0, void 0, function () {
            var parentResponse;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        if (!(this.engagementDetails[engagementId].parentId == 0)) return [3, 3];
                        return [4, this.saveInitialParentEntry(engagementId)];
                    case 1:
                        parentResponse = _a.sent();
                        if (this.engagementDetails[engagementId].parentId == 0) {
                            this.engagementDetails[engagementId].parentId = parentResponse.id;
                        }
                        return [4, this.checkAndUpdateProductDBTime(engagementId, false)];
                    case 2:
                        _a.sent();
                        this.addLog("Parent Record created with id" + parentResponse.id, LogTypes.DEBUG);
                        _a.label = 3;
                    case 3: return [2];
                }
            });
        });
    };
    ChatEmotion.prototype.engagementRemovedHandler = function (engRemovedEvtArgs) {
        return __awaiter(this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        this.addLog("Engagement has been removed." + engRemovedEvtArgs.EngagementId, LogTypes.DEBUG);
                        return [4, this.saveConcludedChatResult("Engagement has been concluded.", engRemovedEvtArgs.EngagementId, true)];
                    case 1:
                        _a.sent();
                        return [4, this.checkAndUpdateProductDBTime(engRemovedEvtArgs.EngagementId, true)];
                    case 2:
                        _a.sent();
                        return [2];
                }
            });
        });
    };
    ChatEmotion.prototype.checkAndUpdateProductDBTime = function (engagementId, checkForCompleted) {
        return __awaiter(this, void 0, void 0, function () {
            var i, response, payload;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        i = 0;
                        _a.label = 1;
                    case 1:
                        if (!(i < 10)) return [3, 6];
                        return [4, new Promise(function (resolve) { return setTimeout(resolve, 1000); })];
                    case 2:
                        _a.sent();
                        return [4, this.checkChatDBTime(engagementId)];
                    case 3:
                        response = _a.sent();
                        if (!(response['count'] > 0)) return [3, 5];
                        if (!(response['rows'][0][1] || response['rows'][0][2])) return [3, 5];
                        payload = {
                            FirstEngagementTime: response['rows'][0][1],
                            CompletedTime: response['rows'][0][2]
                        };
                        return [4, this.updateParent(payload, this.engagementDetails[engagementId].parentId)];
                    case 4:
                        _a.sent();
                        if (checkForCompleted && response['rows'][0][2]) {
                            i = 20;
                        }
                        else if (!checkForCompleted && response['rows'][0][1]) {
                            i = 20;
                        }
                        _a.label = 5;
                    case 5:
                        i++;
                        return [3, 1];
                    case 6: return [2];
                }
            });
        });
    };
    ChatEmotion.prototype.saveConcludedChatResult = function (messageBody, engagementId, sendMessage) {
        return __awaiter(this, void 0, void 0, function () {
            var saveConcludedChatResponse, payload, parentId;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        saveConcludedChatResponse = null;
                        payload = null;
                        parentId = this.engagementDetails[engagementId].parentId;
                        if (!sendMessage) return [3, 2];
                        payload = {
                            ChatAIResultSummary: { id: parentId },
                            ChatText: messageBody,
                            ChatOrder: this.nextChatOrder(engagementId),
                            RequestManagerInterveneConf: '0.00',
                            EmotionConf: '0.00',
                            ChatRole: { lookupName: RoleTypes.SYSTEM }
                        };
                        return [4, this.callPOSTRestAPI(payload, this.CX_SERVICE_URL + this.CX_API_URL_PATH + this.CX_CHILD_OBJECT_URL)];
                    case 1:
                        saveConcludedChatResponse = _a.sent();
                        _a.label = 2;
                    case 2: return [4, this.updateFinalParentEntry(parentId, false)];
                    case 3:
                        _a.sent();
                        return [2, saveConcludedChatResponse];
                }
            });
        });
    };
    ChatEmotion.prototype.updateFinalParentEntry = function (parentId, isActive) {
        return __awaiter(this, void 0, void 0, function () {
            var payload;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        payload = null;
                        payload = {
                            IsActive: isActive
                        };
                        return [4, this.updateParent(payload, parentId)];
                    case 1: return [2, _a.sent()];
                }
            });
        });
    };
    ChatEmotion.prototype.getEmotion = function (message, sender) {
        return __awaiter(this, void 0, void 0, function () {
            var apiPath, enpoint, payload;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        apiPath = '';
                        enpoint = '';
                        if (sender == RoleTypes.AGENT) {
                            enpoint = this.AGENT_EMOTION_ENDPOINT;
                        }
                        else {
                            enpoint = this.EMOTION_ENDPOINT;
                        }
                        payload = { "documents": [{ "key": "1", "text": message, "languageCode": "en" }], "endpointId": enpoint };
                        return [4, this.getXOResponse(apiPath, payload, this.EMOTION_XO_NAME)];
                    case 1: return [2, _a.sent()];
                }
            });
        });
    };
    ChatEmotion.prototype.getSupervisorAsk = function (message) {
        return __awaiter(this, void 0, void 0, function () {
            var payload, apiPath;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        payload = { "documents": [{ "key": "1", "text": message, "languageCode": "en" }], "endpointId": this.SUPERVISOR_ENDPOINT };
                        apiPath = "";
                        return [4, this.getXOResponse(apiPath, payload, this.SUPERVISOR_XO_NAME)];
                    case 1: return [2, _a.sent()];
                }
            });
        });
    };
    ChatEmotion.prototype.getXOResponse = function (apiPath, payload, connectionName) {
        return __awaiter(this, void 0, void 0, function () {
            var response, globalContext, containerCtx, extensionContext, connectionCollection, connection, needRetry, exceptionVar_2, error_4;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        response = null;
                        _a.label = 1;
                    case 1:
                        _a.trys.push([1, 12, , 13]);
                        return [4, this.getGlobalContext()];
                    case 2:
                        globalContext = _a.sent();
                        return [4, globalContext.getContainerContext()];
                    case 3:
                        containerCtx = _a.sent();
                        return [4, globalContext.getExtensionContext(containerCtx.extensionName)];
                    case 4:
                        extensionContext = _a.sent();
                        return [4, extensionContext.getConnections(connectionName)];
                    case 5:
                        connectionCollection = _a.sent();
                        connection = connectionCollection.get(connectionName);
                        connection.open(this.POST_METHOD, apiPath);
                        connection.setContentType(this.REST_CONTENT_TYPE);
                        needRetry = false;
                        _a.label = 6;
                    case 6:
                        _a.trys.push([6, 8, , 9]);
                        return [4, connection.send(JSON.stringify(payload))];
                    case 7:
                        response = _a.sent();
                        return [3, 9];
                    case 8:
                        exceptionVar_2 = _a.sent();
                        this.addLog("Error occurred in " + apiPath + " call :" + exceptionVar_2, LogTypes.ERROR);
                        if (exceptionVar_2.status == 504) {
                            needRetry = true;
                        }
                        return [3, 9];
                    case 9:
                        if (!needRetry) return [3, 11];
                        this.addLog("Retry due to timeout " + apiPath, LogTypes.ERROR);
                        return [4, connection.send(JSON.stringify(payload))];
                    case 10:
                        response = _a.sent();
                        _a.label = 11;
                    case 11: return [3, 13];
                    case 12:
                        error_4 = _a.sent();
                        this.addLog("Error while calling " + apiPath + " model" + error_4, LogTypes.ERROR);
                        return [3, 13];
                    case 13: return [2, response];
                }
            });
        });
    };
    ChatEmotion.prototype.callPOSTRestAPI = function (payload, url) {
        return __awaiter(this, void 0, void 0, function () {
            var response;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4, fetch(url, {
                            method: this.POST_METHOD,
                            headers: {
                                'Content-Type': this.REST_CONTENT_TYPE,
                                'OSvC-CREST-Application-Context': 'Chat Sentiment save child',
                                'Authorization': "Session ".concat(this.sessionToken)
                            },
                            body: JSON.stringify(payload)
                        })];
                    case 1:
                        response = _a.sent();
                        return [2, response];
                }
            });
        });
    };
    ChatEmotion.prototype.updateParent = function (payload, id) {
        return __awaiter(this, void 0, void 0, function () {
            var response;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4, fetch(this.CX_SERVICE_URL + this.CX_API_URL_PATH + this.CX_PARENT_OBJECT_URL + id, {
                            method: 'PATCH',
                            headers: {
                                'Content-Type': this.REST_CONTENT_TYPE,
                                'OSvC-CREST-Application-Context': 'Chat Sentiment Analysis update',
                                'Authorization': "Session ".concat(this.sessionToken)
                            },
                            body: JSON.stringify(payload)
                        })];
                    case 1:
                        response = _a.sent();
                        return [2, response];
                }
            });
        });
    };
    ChatEmotion.prototype.unEscape = function (htmlStr) {
        var _a;
        var textarea = this.txtConvertElement;
        textarea.innerHTML = htmlStr;
        return (_a = textarea.value) === null || _a === void 0 ? void 0 : _a.replace(/(<([^>]+)>)/ig, '');
    };
    ChatEmotion.prototype.makeGETAPIRequest = function (url) {
        return __awaiter(this, void 0, void 0, function () {
            var response;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4, fetch(this.CX_SERVICE_URL + this.CX_API_URL_PATH + url, {
                            method: 'GET',
                            headers: {
                                'Content-Type': this.REST_CONTENT_TYPE,
                                'OSvC-CREST-Application-Context': 'Chat Sentiment Analysis',
                                'Authorization': "Session ".concat(this.sessionToken)
                            }
                        })];
                    case 1:
                        response = _a.sent();
                        return [2, response];
                }
            });
        });
    };
    ChatEmotion.prototype.processConfigurations = function (item) {
        return __awaiter(this, void 0, void 0, function () {
            var response, responsebody, config;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4, this.makeGETAPIRequest('configurations/' + item.id)];
                    case 1:
                        response = _a.sent();
                        return [4, response.json()];
                    case 2:
                        responsebody = _a.sent();
                        config = JSON.parse(responsebody === null || responsebody === void 0 ? void 0 : responsebody.value);
                        this.INITIAL_MESSAGES_TO_SKIP = config === null || config === void 0 ? void 0 : config.INITIAL_MESSAGES_TO_SKIP;
                        this.MAX_NEGATIVE_CHAT_COUNT = config === null || config === void 0 ? void 0 : config.MAX_NEGATIVE_CHAT_COUNT;
                        this.MIN_REQUEST_MANAGER_CONFIG = config === null || config === void 0 ? void 0 : config.MIN_REQUEST_MANAGER_CONFIG;
                        this.MIN_EMOTION_CONFIG = config === null || config === void 0 ? void 0 : config.MIN_EMOTION_CONFIG;
                        this.IS_EMOTION_ACTIVE = config === null || config === void 0 ? void 0 : config.IS_EMOTION_ACTIVE;
                        this.IS_MANAGER_ASK_ACTIVE = config === null || config === void 0 ? void 0 : config.IS_MANAGER_ASK_ACTIVE;
                        this.EMOTION_XO_NAME = config === null || config === void 0 ? void 0 : config.EMOTION_XO_NAME;
                        this.SUPERVISOR_XO_NAME = config === null || config === void 0 ? void 0 : config.SUPERVISOR_XO_NAME;
                        this.SUPERVISOR_ENDPOINT = config === null || config === void 0 ? void 0 : config.SUPERVISOR_ENDPOINT;
                        this.EMOTION_ENDPOINT = config === null || config === void 0 ? void 0 : config.EMOTION_ENDPOINT;
                        this.AGENT_EMOTION_ENDPOINT = config === null || config === void 0 ? void 0 : config.AGENT_EMOTION_ENDPOINT;
                        if (config === null || config === void 0 ? void 0 : config.NEGATIVE_LOOKUPNAME) {
                            this.NEGATIVE_LOOKUPNAME = config === null || config === void 0 ? void 0 : config.NEGATIVE_LOOKUPNAME;
                        }
                        if (config === null || config === void 0 ? void 0 : config.POSITIVE_LOOKUPNAME) {
                            this.POSITIVE_LOOKUPNAME = config === null || config === void 0 ? void 0 : config.POSITIVE_LOOKUPNAME;
                        }
                        if (config === null || config === void 0 ? void 0 : config.NEUTRAL_LOOKUPNAME) {
                            this.NEUTRAL_LOOKUPNAME = config === null || config === void 0 ? void 0 : config.NEUTRAL_LOOKUPNAME;
                        }
                        return [2];
                }
            });
        });
    };
    ChatEmotion.prototype.addLog = function (message, logType) {
        ORACLE_SERVICE_CLOUD.extension_loader.load(this.APP_NAME).then(function (extensionProvider) {
            var defaultLogger = extensionProvider.getLogger();
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
    };
    return ChatEmotion;
}());
(function () { return __awaiter(void 0, void 0, void 0, function () {
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0: return [4, new ChatEmotion().initialize()];
            case 1:
                _a.sent();
                return [2];
        }
    });
}); })();
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiY2hhdEVtb3Rpb24uanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyJjaGF0RW1vdGlvbi50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiOzs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7O0FBVUEsSUFBSyxRQU1KO0FBTkQsV0FBSyxRQUFRO0lBQ1QsMkJBQWUsQ0FBQTtJQUNmLHlCQUFhLENBQUE7SUFDYiwyQkFBZSxDQUFBO0lBQ2YsMkJBQWUsQ0FBQTtJQUNmLHlCQUFhLENBQUE7QUFDakIsQ0FBQyxFQU5JLFFBQVEsS0FBUixRQUFRLFFBTVo7QUFFRCxJQUFLLFNBT0o7QUFQRCxXQUFLLFNBQVM7SUFDViw0QkFBZSxDQUFBO0lBQ2Ysa0NBQXFCLENBQUE7SUFDckIsOEJBQWlCLENBQUE7SUFDakIsd0NBQTJCLENBQUE7SUFDM0Isa0NBQXFCLENBQUE7QUFFekIsQ0FBQyxFQVBJLFNBQVMsS0FBVCxTQUFTLFFBT2I7QUEwQ0QsSUFBTSxnQkFBZ0IsR0FBRyxDQUFDLENBQUM7QUFDM0IsSUFBTSxnQkFBZ0IsR0FBRyxDQUFDLENBQUM7QUFDM0I7SUFBQTtRQUNZLGtCQUFhLEdBQUcsS0FBSyxDQUFDO1FBT3RCLHVCQUFrQixHQUFHLG9CQUFvQixDQUFDO1FBTTFDLHdCQUFtQixHQUFXLFVBQVUsQ0FBQztRQUN6Qyx3QkFBbUIsR0FBVyxVQUFVLENBQUM7UUFDekMsdUJBQWtCLEdBQVksU0FBUyxDQUFDO1FBQ3hDLG1CQUFjLEdBQUcsTUFBTSxDQUFDO1FBQ3hCLG9CQUFlLEdBQUcsbUJBQVksSUFBSSxDQUFDLGNBQWMsTUFBRyxDQUFDO1FBQ3JELG1CQUFjLEdBQUcsRUFBRSxDQUFDO1FBQ3BCLHlCQUFvQixHQUFHLHlCQUF5QixDQUFDO1FBQ2pELHlCQUFvQixHQUFHLDJCQUEyQixDQUFDO1FBQ25ELHdCQUFtQixHQUFHLDRCQUE0QixDQUFDO1FBQ25ELGtCQUFhLEdBQUcsZ0JBQWdCLENBQUM7UUFDakMsc0JBQWlCLEdBQVEsRUFB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QUMsQ0FBQyxJQUFJLENBQUMsVUFBQyxpQkFBcUM7WUFDakcsSUFBTSxhQUFhLEdBQXFCLGlCQUFpQixDQUFDLFNBQVMsRUFBRSxDQUFDO1lBQ3RFLFFBQVEsT0FBTyxFQUFFO2dCQUNiLEtBQUssUUFBUSxDQUFDLEtBQUs7b0JBQ2YsYUFBYSxDQUFDLEtBQUssQ0FBQyxPQUFPLENBQUMsQ0FBQztvQkFDN0IsTUFBTTtnQkFDVixLQUFLLFFBQVEsQ0FBQyxJQUFJO29CQUNkLGFBQWEsQ0FBQyxJQUFJLENBQUMsT0FBTyxDQUFDLENBQUM7b0JBQzVCLE1BQU07Z0JBQ1YsS0FBSyxRQUFRLENBQUMsSUFBSTtvQkFDZCxhQUFhLENBQUMsSUFBSSxDQUFDLE9BQU8sQ0FBQyxDQUFDO29CQUM1QixNQUFNO2dCQUNWLEtBQUssUUFBUSxDQUFDLEtBQUs7b0JBQ2YsYUFBYSxDQUFDLEtBQUssQ0FBQyxPQUFPLENBQUMsQ0FBQztvQkFDN0IsTUFBTTtnQkFDVjtvQkFDSSxhQUFhLENBQUMsSUFBSSxDQUFDLE9BQU8sQ0FBQyxDQUFDO2FBQ25DO1FBQ0wsQ0FBQyxDQUFDLENBQUM7SUFDUCxDQUFDO0lBQ0wsa0JBQUM7QUFBRCxDQUFDLEFBMXVCRCxJQTB1QkM7QUFFRCxDQUFDOzs7b0JBQ0csV0FBTSxJQUFJLFdBQVcsRUFBRSxDQUFDLFVBQVUsRUFBRSxFQUFBOztnQkFBcEMsU0FBb0MsQ0FBQzs7OztLQUN4QyxDQUFDLEVBQUUsQ0FBQSIsInNvdXJjZXNDb250ZW50IjpbIi8qICogKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKlxuKiAgJEFDQ0VMRVJBVE9SX0hFQURFUl9QTEFDRV9IT0xERVIkXG4qICBTSEExOiAkSWQ6ICRcbiogKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqXG4qICBGaWxlOiAkQUNDRUxFUkFUT1JfSEVBREVSX0ZJTEVfTkFNRV9QTEFDRV9IT0xERVIkXG4qICoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKiAqL1xuXG4vLy8gPHJlZmVyZW5jZSBwYXRoPVwiLi9vc3ZjRXh0ZW5zaW9uLmQudHNcIiAvPlxuLy8vIDxyZWZlcmVuY2UgcGF0aCA9XCIuL2VuZ2FnZW1lbnRQYW5lbEpzLmQudHNcIiAvPlxuXG5lbnVtIExvZ1R5cGVzIHtcbiAgICBFUlJPUiA9ICdFcnJvcicsXG4gICAgV0FSTiA9ICdXYXJuJyxcbiAgICBERUJVRyA9ICdEZWJ1ZycsXG4gICAgVFJBQ0UgPSAnVHJhY2UnLFxuICAgIElORk8gPSAnSW5mbydcbn1cblxuZW51bSBSb2xlVHlwZXMge1xuICAgIEFHRU5UID0gJ0FHRU5UJyxcbiAgICBFTkRfVVNFUiA9ICdFTkRfVVNFUicsXG4gICAgU1lTVEVNID0gJ1NZU1RFTScsXG4gICAgTk9UX0VORFVTRVIgPSAnTk9UX0VORFVTRVInLFxuICAgIENPTkZFUkVFID0gJ0NPTkZFUkVFJ1xuXG59XG5cblxuaW50ZXJmYWNlIFJlcG9ydFBheWxvYWQge1xuICAgIGxvb2t1cE5hbWU6IHN0cmluZztcbiAgICBmaWx0ZXJzOiB7XG4gICAgICAgIG5hbWU6IHN0cmluZztcbiAgICAgICAgdmFsdWVzOiBzdHJpbmdbXTtcbiAgICB9W107XG59XG5cbmludGVyZmFjZSBzYXZlU2VudGltZW50RW1vdGlvblJlcXVlc3Qge1xuICAgIENoYXRBSVJlc3VsdFN1bW1hcnk6IHsgaWQ6IG51bWJlciB9LFxuICAgIENoYXRUZXh0OiBzdHJpbmcsXG4gICAgQ2hhdE9yZGVyOiBudW1iZXIsXG4gICAgRW1vdGlvbj86IHsgbG9va3VwTmFtZTogc3RyaW5nIH0sXG4gICAgU3VnZ2VzdEVtb3Rpb24/OiB7IGxvb2t1cE5hbWU6IHN0cmluZyB9LFxuICAgIEVtb3Rpb25Db25mPzogc3RyaW5nLFxuICAgIFJlcXVlc3RNYW5hZ2VySW50ZXJ2ZW5lPzogYm9vbGVhbixcbiAgICBTdWdnZXN0TWFuYWdlckludGVydmVuZT86IGJvb2xlYW4sXG4gICAgUmVxdWVzdE1hbmFnZXJJbnRlcnZlbmVDb25mPzogc3RyaW5nLFxuICAgIENoYXRSb2xlOiB7IGxvb2t1cE5hbWU6IHN0cmluZyB9XG59XG5cbmludGVyZmFjZSBTYXZlUGFyZW50UmVjb3JkUmVxdWVzdCB7XG4gICAgQ2hhdElkOiBudW1iZXIsXG4gICAgSXNFbmRVc2VySW5OZWdhdGl2ZUVtb3Rpb246IGJvb2xlYW4sXG4gICAgSGF2ZUFnZW50SW5OZWdhdGl2ZUVtb3Rpb246IGJvb2xlYW4sXG4gICAgSXNBbGVydFRyaWdnZXJlZDogYm9vbGVhbixcbiAgICBSZXF1ZXN0TWFuYWdlckludGVydmVuZTogYm9vbGVhbixcbiAgICBSZXF1ZXN0TWFuYWdlckludGVydmVuZUNvdW50OiBudW1iZXIsXG4gICAgSXNBY3RpdmU6IGJvb2xlYW4sXG4gICAgQ3VycmVudFF1ZXVlOiB7IGlkOiBudW1iZXIgfSxcbiAgICBNYXhOZWdhdGl2ZU1lc3NhZ2VDb3VudDogbnVtYmVyLFxuICAgIE1pbkVtb3Rpb25Db25mOiBTdHJpbmcsXG4gICAgTWluUmVxdWVzdE1hbmFnZXJJbnRlcnZlbmVDb25mOiBTdHJpbmcsXG4gICAgSW5pdGlhbE1lc3NhZ2VUb1NraXBDb3VudDogbnVtYmVyLFxuICAgIE5lZ2F0aXZlTWVzc2FnZUNvdW50OiBudW1iZXIsXG4gICAgQWNjb3VudElkOiB7IGlkOiBudW1iZXIgfSxcbiAgICBDb250YWN0SWQ6IHsgaWQ6IG51bWJlciB9LFxuICAgIElzUHJpdmF0ZUNoYXQ6IGJvb2xlYW5cbn1cbmNvbnN0IE5FR0FUSVZFX0VNT1RJT04gPSAyO1xuY29uc3QgUE9TSVRJVkVfRU1PVElPTiA9IDE7XG5jbGFzcyBDaGF0RW1vdGlvbiB7XG4gICAgcHJpdmF0ZSBhZGRlZExpc3RlbmVyID0gZmFsc2U7XG4gICAgcHJpdmF0ZSBJTklUSUFMX01FU1NBR0VTX1RPX1NLSVA6IG51bWJlcjtcbiAgICBwcml2YXRlIE1BWF9ORUdBVElWRV9DSEFUX0NPVU5UOiBudW1iZXI7XG4gICAgcHJpdmF0ZSBNSU5fUkVRVUVTVF9NQU5BR0VSX0NPTkZJRzogbnVtYmVyO1xuICAgIHByaXZhdGUgTUlOX0VNT1RJT05fQ09ORklHOiBudW1iZXI7XG4gICAgcHJpdmF0ZSBFTU9USU9OX1hPX05BTUU6IHN0cmluZztcbiAgICBwcml2YXRlIFNVUEVSVklTT1JfWE9fTkFNRTogc3RyaW5nO1xuICAgIHByaXZhdGUgQ1VTVE9NX0NGR19FTU9USU9OID0gXCJDVVNUT01fQ0ZHX0VNT1RJT05cIjtcbiAgICBwcml2YXRlIElTX01BTkFHRVJfQVNLX0FDVElWRTogYm9vbGVhbjtcbiAgICBwcml2YXRlIElTX0VNT1RJT05fQUNUSVZFOiBib29sZWFuO1xuICAgIHByaXZhdGUgU1VQRVJWSVNPUl9FTkRQT0lOVDogc3RyaW5nO1xuICAgIHByaXZhdGUgRU1PVElPTl9FTkRQT0lOVDogc3RyaW5nO1xuICAgIHByaXZhdGUgQUdFTlRfRU1PVElPTl9FTkRQT0lOVDogc3RyaW5nO1xuICAgIHByaXZhdGUgUE9TSVRJVkVfTE9PS1VQTkFNRTogc3RyaW5nID0gJ1Bvc2l0aXZlJztcbiAgICBwcml2YXRlIE5FR0FUSVZFX0xPT0tVUE5BTUU6IHN0cmluZyA9ICdOZWdhdGl2ZSc7XG4gICAgcHJpdmF0ZSBORVVUUkFMX0xPT0tVUE5BTUU6IHN0cmluZyAgPSAnTmV1dHJhbCc7XG4gICAgcHJpdmF0ZSBDWF9BUElfVkVSU0lPTiA9IFwidjEuNFwiO1xuICAgIHByaXZhdGUgQ1hfQVBJX1VSTF9QQVRIID0gYC9jb25uZWN0LyR7dGhpcy5DWF9BUElfVkVSU0lPTn0vYDtcbiAgICBwcml2YXRlIENYX1NFUlZJQ0VfVVJMID0gXCJcIjtcbiAgICBwcml2YXRlIENYX0dFVF9QQVJFTlRfUkVQT1JUID0gXCJhbmFseXRpY3NSZXBvcnRSZXN1bHRzL1wiO1xuICAgIHByaXZhdGUgQ1hfUEFSRU5UX09CSkVDVF9VUkwgPSBcIkFJTUwuQ2hhdEFJUmVzdWx0U3VtbWFyeS9cIjtcbiAgICBwcml2YXRlIENYX0NISUxEX09CSkVDVF9VUkwgPSBcIkFJTUwuQ2hhdEFJUHJlZGljdGlvbkluZm8vXCI7XG4gICAgcHJpdmF0ZSBDWF9DT05GSUdfVVJMID0gXCJjb25maWd1cmF0aW9uc1wiO1xuICAgIHByaXZhdGUgZW5nYWdlbWVudERldGFpbHM6IGFueSA9IHt9O1xuICAgIHByaXZhdGUgZW5nYWdlbWVudFdpdGhBY3Rpb25SZWdpc3RlcmVkID0gbmV3IFNldCgpO1xuICAgIHByaXZhdGUgc2Vzc2lvblRva2VuOiBTdHJpbmc7XG4gICAgcHJpdmF0ZSBBUFBfTkFNRSA9IFwiY2hhdF9lbW90aW9uXCI7XG4gICAgcHJpdmF0ZSBnbG9iYWxDb250ZXh0UHJvbWlzZTogSUV4dGVuc2lvblByb21pc2U8SUV4dGVuc2lvbkdsb2JhbENvbnRleHQ+ID0gbnVsbDtcbiAgICBwcml2YXRlIGV4dGVuc2lvblByb3ZpZGVyUHJvbWlzZTogSUV4dGVuc2lvblByb21pc2U8SUV4dGVuc2lvblByb3ZpZGVyPiA9IG51bGw7XG4gICAgcHJpdmF0ZSB0eHRDb252ZXJ0RWxlbWVudCA9IGRvY3VtZW50LmNyZWF0ZUVsZW1lbnQoXCJ0ZXh0YXJlYVwiKTtcbiAgICBwcml2YXRlIEVOX0lOX0ZPUk1BVCA9ICdlbi1JTic7XG4gICAgcHJpdmF0ZSBGUkFDVElPTl9MRU5HVEggPSAyO1xuICAgIHByaXZhdGUgSFVORFJFRF9NVUxUSVBMSUVSID0gMTAwO1xuXG4gICAgcHJpdmF0ZSBQT1NUX01FVEhPRCA9ICdQT1NUJztcbiAgICBwcml2YXRlIEdFVF9NRVRIT0QgPSAnR0VUJztcbiAgICBwcml2YXRlIFJFU1RfQ09OVEVOVF9UWVBFID0gJ2FwcGxpY2F0aW9uL2pzb24nO1xuICAgIHByaXZhdGUgQVNfV0FJVF9NQVhfVElNRTogbnVtYmVyID0gMjAwMDA7XG4gICAgcHJpdmF0ZSBUSU1FX09VVF9JTlRFUlZBTCA9IDIwMDtcblxuICAgIHByaXZhdGUgYXN5bmMgZ2V0RXh0ZW5zaW9uUHJvdmlkZXIoKTogUHJvbWlzZTxPUkFDTEVfU0VSVklDRV9DTE9VRC5JRXh0ZW5zaW9uUHJvdmlkZXI+IHtcbiAgICAgICAgaWYgKHRoaXMuZXh0ZW5zaW9uUHJvdmlkZXJQcm9taXNlID09IG51bGwpIHtcbiAgICAgICAgICAgIHRoaXMuZXh0ZW5zaW9uUHJvdmlkZXJQcm9taXNlID0gT1JBQ0xFX1NFUlZJQ0VfQ0xPVUQuZXh0ZW5zaW9uX2xvYWRlci5sb2FkKHRoaXMuQVBQX05BTUUpO1xuICAgICAgICB9XG4gICAgICAgIHJldHVybiBhd2FpdCB0aGlzLmV4dGVuc2lvblByb3ZpZGVyUHJvbWlzZTtcbiAgICB9XG5cbiAgICBwcml2YXRlIGFzeW5jIGdldEdsb2JhbENvbnRleHQoKTogUHJvbWlzZTxPUkFDTEVfU0VSVklDRV9DTE9VRC5JRXh0ZW5zaW9uR2xvYmFsQ29udGV4dD4ge1xuICAgICAgICBpZiAodGhpcy5nbG9iYWxDb250ZXh0UHJvbWlzZSA9PSBudWxsKSB7XG4gICAgICAgICAgICB0aGlzLmdsb2JhbENvbnRleHRQcm9taXNlID0gKGF3YWl0IHRoaXMuZ2V0RXh0ZW5zaW9uUHJvdmlkZXIoKSkuZ2V0R2xvYmFsQ29udGV4dCgpO1xuICAgICAgICB9XG4gICAgICAgIHJldHVybiBhd2FpdCB0aGlzLmdsb2JhbENvbnRleHRQcm9taXNlO1xuICAgIH1cblxuICAgIHByaXZhdGUgYXN5bmMgZ2V0V29ya3NwYWNlUmVjb3JkKG9iamVjdElkIDogbnVtYmVyLCB0eXBlIDogc3RyaW5nKTogUHJvbWlzZTxJV29ya3NwYWNlUmVjb3JkPiB7XG4gICAgICAgIGxldCBleHRlbnNpb25Qcm92aWRlOiBPUkFDTEVfU0VSVklDRV9DTE9VRC5JRXh0ZW5zaW9uUHJvdmlkZXIgPSBhd2FpdCB0aGlzLmdldEV4dGVuc2lvblByb3ZpZGVyKCk7XG4gICAgICAgIGxldCB3b3Jrc3BhY2VSZWNvcmRQcm9taXNlOiBJRXh0ZW5zaW9uUHJvbWlzZTxJV29ya3NwYWNlUmVjb3JkPiA9IG5ldyBFeHRlbnNpb25Qcm9taXNlKCk7XG4gICAgICAgIGV4dGVuc2lvblByb3ZpZGUucmVnaXN0ZXJXb3Jrc3BhY2VFeHRlbnNpb24oZnVuY3Rpb24gKHdvcmtzcGFjZVJlY29yZDogSVdvcmtzcGFjZVJlY29yZCkge1xuICAgICAgICAgICAgd29ya3NwYWNlUmVjb3JkUHJvbWlzZS5yZXNvbHZlKHdvcmtzcGFjZVJlY29yZCk7XG4gICAgICAgIH0sIHR5cGUgLCBvYmplY3RJZCk7XG5cbiAgICAgICAgcmV0dXJuIGF3YWl0IHdvcmtzcGFjZVJlY29yZFByb21pc2U7XG4gICAgfVxuXG4gICAgcHVibGljIGFzeW5jIHdhaXRVbnRpbChmdW5jUmVmOigpPT4gYW55KSA6IFByb21pc2U8YW55PiB7XG4gICAgICAgIGxldCByZXR1blZhbHVlID0gZmFsc2U7XG4gICAgICAgIHdoaWxlICghcmV0dW5WYWx1ZSkge1xuICAgICAgICAgICAgcmV0dW5WYWx1ZSA9IGZ1bmNSZWYoKTtcbiAgICAgICAgICAgIGlmKCFyZXR1blZhbHVlKXtcbiAgICAgICAgICAgICAgICBhd2FpdCBQcm9taXNlLnJlc29sdmUoKTtcbiAgICAgICAgICAgIH1cbiAgICAgICAgfVxuICAgICAgICByZXR1cm4gcmV0dW5WYWx1ZTtcbiAgICB9XG5cbiAgICBwdWJsaWMgYXN5bmMgaW5pdGlhbGl6ZSgpOiBQcm9taXNlPHZvaWQ+IHtcbiAgICAgICAgY29uc3QgZ2xvYmFsQ29udGV4dDogSUV4dGVuc2lvbkdsb2JhbENvbnRleHQgPSBhd2FpdCB0aGlzLmdldEdsb2JhbENvbnRleHQoKTtcblxuICAgICAgICBjb25zdCBjaGF0QVBJSW5mbzogSUNoYXRBUElJbmZvICA9ICBhd2FpdCB0aGlzLndhaXRVbnRpbCgoKSA9PiBnbG9iYWxDb250ZXh0LmdldENoYXRBUElJbmZvKCkpO1xuXG4gICAgICAgIGF3YWl0IChPUkFDTEVfU0VSVklDRV9DTE9VRCBhcyBhbnkpLnNjcmlwdExvYWRlci5sb2FkU2NyaXB0KFtjaGF0QVBJSW5mby5nZXRDaGF0QVBJVVJMKCldKVxuICAgICAgICB0aGlzLnNlc3Npb25Ub2tlbiA9IGF3YWl0IGdsb2JhbENvbnRleHQuZ2V0U2Vzc2lvblRva2VuKCk7XG4gICAgICAgIHRoaXMuYWRkTG9nKFwiY2FsbCB0byBnZXQgYWdlbnQgc2Vzc2lvblwiLCBMb2dUeXBlcy5ERUJVRyk7XG4gICAgICAgIGF3YWl0IHRoaXMubG9hZEFuZFNldHVwQWdlbnRDaGF0U2VudGltZW50KCk7XG4gICAgICAgIHRoaXMuYWRkTG9nKFwiY2FsbCB0byBnZXQgYWdlbnQgc2Vzc2lvbiBlbmRcIiwgTG9nVHlwZXMuREVCVUcpO1xuXG4gICAgICAgIGF3YWl0IHRoaXMubG9hZFNlbnRpbWVudExhYmVscygpO1xuXG4gICAgfVxuXG4gICAgcHJpdmF0ZSBhc3luYyBsb2FkU2VudGltZW50TGFiZWxzKCkge1xuICAgICAgICBsZXQgZW1vdGlvbkxpc3QgPSBhd2FpdCB0aGlzLm1ha2VHRVRBUElSZXF1ZXN0KFwiQUlNTC5FbW90aW9uXCIpO1xuICAgICAgICBpZiAoZW1vdGlvbkxpc3QgIT0gbnVsbCkge1xuICAgICAgICAgICAgbGV0IGVtb3Rpb25MaXN0SnNvbiA9IGF3YWl0IGVtb3Rpb25MaXN0Lmpzb24oKTtcbiAgICAgICAgICAgIGxldCBlbW90aW9ucyA9IGVtb3Rpb25MaXN0SnNvbj8uaXRlbXM7XG4gICAgICAgICAgICB2YXIgc2VsZiA9IHRoaXM7XG4gICAgICAgICAgICBlbW90aW9ucy5mb3JFYWNoKGFzeW5jIChpdGVtOiBhbnkpID0+IHtcbiAgICAgICAgICAgICAgICBpZiAoaXRlbS5sb29rdXBOYW1lLnRvTG93ZXJDYXNlKCkgPT0gc2VsZi5QT1NJVElWRV9MT09LVVBOQU1FPy50b0xvd2VyQ2FzZSgpKSB7XG4gICAgICAgICAgICAgICAgICAgIHNlbGYuUE9TSVRJVkVfTE9PS1VQTkFNRSA9IGl0ZW0ubG9va3VwTmFtZTtcbiAgICAgICAgICAgICAgICB9IGVsc2UgaWYgKGl0ZW0ubG9va3VwTmFtZS50b0xvd2VyQ2FzZSgpID09IHNlbGYuTkVHQVRJVkVfTE9PS1VQTkFNRT8udG9Mb3dlckNhc2UoKSkge1xuICAgICAgICAgICAgICAgICAgICBzZWxmLk5FR0FUSVZFX0xPT0tVUE5BTUUgPSBpdGVtLmxvb2t1cE5hbWU7XG4gICAgICAgICAgICAgICAgfSBlbHNlIGlmIChpdGVtLmxvb2t1cE5hbWUudG9Mb3dlckNhc2UoKSA9PSBzZWxmLk5FVVRSQUxfTE9PS1VQTkFNRT8udG9Mb3dlckNhc2UoKSkge1xuICAgICAgICAgICAgICAgICAgICBzZWxmLk5FVVRSQUxfTE9PS1VQTkFNRSA9IGl0ZW0ubG9va3VwTmFtZTtcbiAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICB9KTtcbiAgICAgICAgfVxuICAgIH1cblxuICAgIGFzeW5jIGxvYWRBbmRTZXR1cEFnZW50Q2hhdFNlbnRpbWVudCgpOiBQcm9taXNlPHZvaWQ+IHtcbiAgICAgICAgYXdhaXQgdGhpcy5sb2FkQ29uZmlndXJhdGlvbnMoKTtcbiAgICAgICAgbGV0IGFzOiBJT3JhY2xlQ2hhdENsaWVudC5JQWdlbnRTZXNzaW9uID0gYXdhaXQgdGhpcy53YWl0VW50aWxBZ2VudFNlc3Npb25Jc0xvYWRlZCh0aGlzLkFTX1dBSVRfTUFYX1RJTUUpO1xuICAgICAgICBhd2FpdCBhcy5zZXNzaW9uU3RhdHVzQ2hhbmdlZChhc3luYyAoc3RhdEluZm86IElDaGF0U2Vzc2lvblN0YXR1c0V2ZW50QXJncykgPT4ge1xuICAgICAgICAgICAgaWYgKHN0YXRJbmZvLkxvZ2dlZEluICYmICF0aGlzLmFkZGVkTGlzdGVuZXIpIHtcbiAgICAgICAgICAgICAgICBhd2FpdCB0aGlzLmFnZW50U2Vzc2lvbkV2ZW50c0hhbmRsZXIoKTtcbiAgICAgICAgICAgIH1cbiAgICAgICAgfSk7XG5cbiAgICB9XG5cbiAgICBwcml2YXRlIGFzeW5jIHdhaXRVbnRpbEFnZW50U2Vzc2lvbklzTG9hZGVkKG1heFdhaXRUaW1lOiBudW1iZXIpOiBQcm9taXNlPElPcmFjbGVDaGF0Q2xpZW50LklBZ2VudFNlc3Npb24+IHtcbiAgICAgICAgbGV0IHJldHJ5ID0gdHJ1ZTtcbiAgICAgICAgd2hpbGUgKHJldHJ5KSB7XG4gICAgICAgICAgICB0cnkge1xuICAgICAgICAgICAgICAgIGxldCBhZ2VudFNlc3Npb246IElPcmFjbGVDaGF0Q2xpZW50LklBZ2VudFNlc3Npb24gPSBhd2FpdCBuZXcgUHJvbWlzZShhc3luYyAocmVzb2x2ZSwgcmVqZWN0KSA9PiB7XG4gICAgICAgICAgICAgICAgICAgIGxldCB0aW1lb3V0TWF4VGltZSA9IHdpbmRvdy5zZXRUaW1lb3V0KCgpID0+IHtcbiAgICAgICAgICAgICAgICAgICAgICAgIHJldHJ5ID0gZmFsc2U7XG4gICAgICAgICAgICAgICAgICAgICAgICByZWplY3QobnVsbCk7XG4gICAgICAgICAgICAgICAgICAgIH0sIG1heFdhaXRUaW1lKTtcblxuICAgICAgICAgICAgICAgICAgICBsZXQgdGltZW91dFJlZiA9IHdpbmRvdy5zZXRUaW1lb3V0KCgpID0+IHtcbiAgICAgICAgICAgICAgICAgICAgICAgIHJlamVjdChudWxsKTtcbiAgICAgICAgICAgICAgICAgICAgfSwgdGhpcy5USU1FX09VVF9JTlRFUlZBTCk7XG4gICAgICAgICAgICAgICAgICAgIGxldCBhZ2VudFNlc3Npb24gPSBhd2FpdCBJT3JhY2xlQ2hhdENsaWVudC5nZXRBZ2VudFNlc3Npb24oKTtcbiAgICAgICAgICAgICAgICAgICAgaWYgKGFnZW50U2Vzc2lvbiA9PT0gdW5kZWZpbmVkKSB7XG4gICAgICAgICAgICAgICAgICAgICAgICByZWplY3QobnVsbCk7XG4gICAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICAgICAgd2luZG93LmNsZWFyVGltZW91dCh0aW1lb3V0UmVmKTtcbiAgICAgICAgICAgICAgICAgICAgd2luZG93LmNsZWFyVGltZW91dCh0aW1lb3V0TWF4VGltZSk7XG4gICAgICAgICAgICAgICAgICAgIHJlc29sdmUoYWdlbnRTZXNzaW9uKTtcblxuICAgICAgICAgICAgICAgIH0pO1xuICAgICAgICAgICAgICAgIHJldHJ5ID0gZmFsc2U7XG4gICAgICAgICAgICAgICAgcmV0dXJuIGFnZW50U2Vzc2lvbjtcbiAgICAgICAgICAgIH0gY2F0Y2ggKGVycm9yKSB7XG4gICAgICAgICAgICAgICAgdGhpcy5hZGRMb2coXCJJbnNpZGUgZ2V0IGFnZW50IHNlc3Npb24gOlwiICsgZXJyb3IsIExvZ1R5cGVzLkVSUk9SKTtcbiAgICAgICAgICAgIH1cbiAgICAgICAgfVxuXG4gICAgfVxuXG4gICAgcHJpdmF0ZSBhc3luYyBsb2FkQ29uZmlndXJhdGlvbnMoKTogUHJvbWlzZTx2b2lkPiB7XG4gICAgICAgIGNvbnN0IGdsb2JhbENvbnRleHQ6IE9SQUNMRV9TRVJWSUNFX0NMT1VELklFeHRlbnNpb25HbG9iYWxDb250ZXh0ID0gYXdhaXQgdGhpcy5nZXRHbG9iYWxDb250ZXh0KCk7XG4gICAgICAgIHRoaXMuQ1hfU0VSVklDRV9VUkwgPSBnbG9iYWxDb250ZXh0LmdldEludGVyZmFjZVNlcnZpY2VVcmwoJ1JFU1QnKTtcbiAgICAgICAgbGV0IGNvbmZpZ3VyYXRpb25MaXN0UmVzcG9uc2UgPSBhd2FpdCB0aGlzLm1ha2VHRVRBUElSZXF1ZXN0KHRoaXMuQ1hfQ09ORklHX1VSTCk7XG4gICAgICAgIGlmIChjb25maWd1cmF0aW9uTGlzdFJlc3BvbnNlICE9IG51bGwpIHtcbiAgICAgICAgICAgIGxldCBjb25maWd1cmF0aW9uTGlzdFJlc3BvbnNlSnNvbiA9IGF3YWl0IGNvbmZpZ3VyYXRpb25MaXN0UmVzcG9uc2UuanNvbigpO1xuICAgICAgICAgICAgbGV0IGNvbmZpZ3VyYXRpb25MaXN0ID0gY29uZmlndXJhdGlvbkxpc3RSZXNwb25zZUpzb24/Lml0ZW1zO1xuICAgICAgICAgICAgY29uZmlndXJhdGlvbkxpc3QuZm9yRWFjaChhc3luYyAoaXRlbTogYW55KSA9PiB7XG4gICAgICAgICAgICAgICAgaWYgKGl0ZW0ubG9va3VwTmFtZSA9PSB0aGlzLkNVU1RPTV9DRkdfRU1PVElPTikge1xuICAgICAgICAgICAgICAgICAgICBhd2FpdCB0aGlzLnByb2Nlc3NDb25maWd1cmF0aW9ucyhpdGVtKTtcbiAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICB9KTtcblxuICAgICAgICB9XG4gICAgfVxuXG4gICAgYXN5bmMgYWdlbnRTZXNzaW9uRXZlbnRzSGFuZGxlcigpOiBQcm9taXNlPHZvaWQ+IHtcbiAgICAgICAgbGV0IGFnZW50U2Vzc2lvbiA9IGF3YWl0IHRoaXMud2FpdFVudGlsQWdlbnRTZXNzaW9uSXNMb2FkZWQodGhpcy5BU19XQUlUX01BWF9USU1FKTtcbiAgICAgICAgdGhpcy5hZGRMb2coXCJJbnNpZGUgZ2V0IGFnZW50IHNlc3Npb25cIiwgTG9nVHlwZXMuREVCVUcpO1xuICAgICAgICB0aGlzLmFkZExvZyhcInN0YXR1cyBjaGFuZ2UgZXZlbnQ6IGluc2lkZSBBVkFJTEFCTEVcIiwgTG9nVHlwZXMuREVCVUcpO1xuICAgICAgICBsZXQgZW5nYWdlbWVudDogSU9yYWNsZUNoYXRDbGllbnQuSUVuZ2FnZW1lbnQgPSBudWxsO1xuICAgICAgICBsZXQgaGFzQ3VycmVudEVuZ2FnZW1lbnQgPSBmYWxzZTtcbiAgICAgICAgaWYgKHRoaXMuSVNfTUFOQUdFUl9BU0tfQUNUSVZFIHx8IHRoaXMuSVNfRU1PVElPTl9BQ1RJVkUpIHtcbiAgICAgICAgICAgIGFnZW50U2Vzc2lvbi5lbmdhZ2VtZW50UmVtb3ZlZCgoZW5nUmVtb3ZlZEV2dEFyZ3M6IElDaGF0RW5nYWdlbWVudFJlbW92ZWRFdmVudEFyZ3MpID0+IHRoaXMuZW5nYWdlbWVudFJlbW92ZWRIYW5kbGVyKGVuZ1JlbW92ZWRFdnRBcmdzKSk7XG4gICAgICAgICAgICBhZ2VudFNlc3Npb24uZW5nYWdlbWVudEFzc2lnbmVkKChlYUV2dDogSUNoYXRFbmdhZ2VtZW50QXNzaWdubWVudEV2ZW50QXJncykgPT4gdGhpcy5lbmdhZ2VtZW50QXNzaWduZWRIYW5kbGVyKGVhRXZ0KSk7XG4gICAgICAgICAgICBhZ2VudFNlc3Npb24uZW5nYWdlbWVudEFjY2VwdGVkKGFzeW5jIChlYUV2dDogSUNoYXRFbmdhZ2VtZW50QWNjZXB0ZWRFdmVudEFyZ3MpID0+IHRoaXMuZW5nYWdlbWVudEFjY2VwdGVkSGFuZGxlcihlYUV2dCwgYWdlbnRTZXNzaW9uKSk7XG5cbiAgICAgICAgICAgIHRoaXMuYWRkZWRMaXN0ZW5lciA9IHRydWU7XG5cbiAgICAgICAgICAgIHRyeSB7XG4gICAgICAgICAgICAgICAgZW5nYWdlbWVudCA9IGF3YWl0IGFnZW50U2Vzc2lvbi5nZXRDdXJyZW50RW5nYWdlbWVudCgpO1xuXG4gICAgICAgICAgICAgICAgaGFzQ3VycmVudEVuZ2FnZW1lbnQgPSB0cnVlO1xuICAgICAgICAgICAgfSBjYXRjaCAoZXhjZXB0aW9uVmFyKSB7XG4gICAgICAgICAgICAgICAgdGhpcy5hZGRMb2coXCJubyBlbmdhZ2VtZW50IHByZXNlbnQgZm9yIGNoYXRcIiwgTG9nVHlwZXMuREVCVUcpO1xuICAgICAgICAgICAgfVxuICAgICAgICAgICAgaWYgKGhhc0N1cnJlbnRFbmdhZ2VtZW50KSB7XG4gICAgICAgICAgICAgICAgbGV0IGVuZ2FnZW1lbnRJZCA9IE51bWJlcihhd2FpdCBlbmdhZ2VtZW50LmdldEVuZ2FnZW1lbnRJZCgpKTtcblxuICAgICAgICAgICAgICAgIHRoaXMuZW5nYWdlbWVudERldGFpbHNbZW5nYWdlbWVudElkXSA9IHtcbiAgICAgICAgICAgICAgICAgICAgYXNzaWduZWRUaW1lOiBuZXcgRGF0ZSgpXG4gICAgICAgICAgICAgICAgfTtcblxuICAgICAgICAgICAgICAgIGlmIChlbmdhZ2VtZW50Lk15Um9sZSA9PSBSb2xlVHlwZXMuQ09ORkVSRUUpIHtcbiAgICAgICAgICAgICAgICAgICAgdGhpcy5lbmdhZ2VtZW50RGV0YWlsc1tlbmdhZ2VtZW50SWRdLmlzQ29uZmVyZW5jZSA9IHRydWU7XG4gICAgICAgICAgICAgICAgfVxuXG4gICAgICAgICAgICAgICAgbGV0IHBhcmVudEFscmVhZHlQcmVzZW50ID0gYXdhaXQgdGhpcy5jaGVja1BhcmVudEFscmVhZHlQcmVzZW50KGVuZ2FnZW1lbnRJZCwgdHJ1ZSk7XG4gICAgICAgICAgICAgICAgaWYgKHBhcmVudEFscmVhZHlQcmVzZW50Wydjb3VudCddID4gMCkge1xuICAgICAgICAgICAgICAgICAgICBsZXQgcGFyZW50SWQgPSBwYXJlbnRBbHJlYWR5UHJlc2VudFsncm93cyddWzBdWzBdO1xuICAgICAgICAgICAgICAgICAgICB0aGlzLmVuZ2FnZW1lbnREZXRhaWxzW2VuZ2FnZW1lbnRJZF0ucGFyZW50SWQgPSBwYXJlbnRJZDtcbiAgICAgICAgICAgICAgICB9XG5cbiAgICAgICAgICAgICAgICBhd2FpdCB0aGlzLmNyZWF0ZVBhcmVudEVudHJ5KGVuZ2FnZW1lbnRJZCk7XG4gICAgICAgICAgICAgICAgYXdhaXQgdGhpcy5hZGRFbmdhZ2VtZW50Q2FsbGJhY2soYWdlbnRTZXNzaW9uLCBlbmdhZ2VtZW50SWQsIHBhcmVudEFscmVhZHlQcmVzZW50KTtcbiAgICAgICAgICAgIH1cbiAgICAgICAgfVxuXG4gICAgfVxuXG4gICAgZW5nYWdlbWVudEFzc2lnbmVkSGFuZGxlcihlYUV2dDogSUNoYXRFbmdhZ2VtZW50QXNzaWdubWVudEV2ZW50QXJncyk6IHZvaWQge1xuICAgICAgICBsZXQgY29udGFjdEluZm8gPSBlYUV2dC5Db250YWN0SW5mbztcbiAgICAgICAgdGhpcy5hZGRMb2coXCJJbnNpZGUgZW5nYWdlbWVudEFzc2lnbmVkXCIsIExvZ1R5cGVzLkRFQlVHKTtcbiAgICAgICAgbGV0IGFnZW50SW5mbyA9IGVhRXZ0LkFnZW50SW5mbztcbiAgICAgICAgdGhpcy5lbmdhZ2VtZW50RGV0YWlsc1tlYUV2dC5FbmdhZ2VtZW50SWRdID0ge1xuICAgICAgICAgICAgYWNjb3VudElkOiBhZ2VudEluZm8uQWNjb3VudElkID8gcGFyc2VJbnQoYWdlbnRJbmZvLkFjY291bnRJZCkgOiAwLFxuICAgICAgICAgICAgY29udGFjdEVtYWlsOiBlYUV2dC5Db250YWN0SW5mby5FbWFpbEFkZHJlc3MsXG4gICAgICAgICAgICBhZ2VudE5hbWU6IGFnZW50SW5mby5OYW1lLFxuICAgICAgICAgICAgYWdlbnRJZCA6IGFnZW50SW5mby5BY2NvdW50SWQsXG4gICAgICAgICAgICBkaXNwbGF5TmFtZSA6IFwiXCIsXG4gICAgICAgICAgICBxdWV1ZUlkOiBlYUV2dC5RdWV1ZUlkLFxuICAgICAgICAgICAgdXNlckZpcnN0TmFtZTogY29udGFjdEluZm8uRmlyc3ROYW1lLFxuICAgICAgICAgICAgdXNlckxhc3ROYW1lOiBjb250YWN0SW5mby5MYXN0TmFtZSxcbiAgICAgICAgICAgIHVzZXJJZDogY29udGFjdEluZm8uQ29udGFjdElkID8gcGFyc2VJbnQoY29udGFjdEluZm8uQ29udGFjdElkKSA6IDAsXG4gICAgICAgICAgICBwYXJlbnRJZDogMCxcbiAgICAgICAgICAgIGNvbmZQYXJlbnRJZFNldDogbmV3IFNldCgpLFxuICAgICAgICAgICAgY29uZlBhcmVudElkOiAwLFxuICAgICAgICAgICAgY2hhdE9yZGVyOiAwLFxuICAgICAgICAgICAgaXNDb25mZXJlbmNlOiBmYWxzZSxcbiAgICAgICAgICAgIGFzc2lnbmVkVGltZTogbmV3IERhdGUoKVxuICAgICAgICB9O1xuXG4gICAgfVxuXG4gICAgYXN5bmMgY2hlY2tQYXJlbnRBbHJlYWR5UHJlc2VudChjaGF0SWQ6IG51bWJlciwgZ2V0QWN0aXZlOiBib29sZWFuKTogUHJvbWlzZTxhbnk+IHtcblxuICAgICAgICBsZXQgcGF5bG9hZCA9IHtcbiAgICAgICAgICAgIGxvb2t1cE5hbWU6IFwiQ2hlY2tGb3JQYXJlbnRSZWNvcmRcIixcbiAgICAgICAgICAgIGZpbHRlcnM6IFt7XG4gICAgICAgICAgICAgICAgbmFtZTogXCJDaGF0IElkXCIsXG4gICAgICAgICAgICAgICAgdmFsdWVzOiBbY2hhdElkPy50b1N0cmluZygpXVxuICAgICAgICAgICAgfV1cbiAgICAgICAgfTtcbiAgICAgICAgaWYgKGdldEFjdGl2ZSkge1xuICAgICAgICAgICAgY29uc3QgSVNBQ1RJVkVfRkxBRyA9IFwiMVwiO1xuICAgICAgICAgICAgcGF5bG9hZC5maWx0ZXJzID0gW3tcbiAgICAgICAgICAgICAgICBuYW1lOiBcIkNoYXQgSWRcIixcbiAgICAgICAgICAgICAgICB2YWx1ZXM6IFtjaGF0SWQ/LnRvU3RyaW5nKCldXG4gICAgICAgICAgICB9LCB7XG4gICAgICAgICAgICAgICAgbmFtZTogXCJBY3RpdmVcIixcbiAgICAgICAgICAgICAgICB2YWx1ZXM6IFtJU0FDVElWRV9GTEFHXVxuICAgICAgICAgICAgfV1cbiAgICAgICAgfVxuICAgICAgICBsZXQgY2hlY2tGb3JQYXJlbnRVcmwgPSB0aGlzLkNYX1NFUlZJQ0VfVVJMICsgdGhpcy5DWF9BUElfVVJMX1BBVEggKyB0aGlzLkNYX0dFVF9QQVJFTlRfUkVQT1JUO1xuICAgICAgICBsZXQgcmVzcG9uc2UgPSBhd2FpdCB0aGlzLmNhbGxQb3N0UmVwb3J0QVBJKGNoZWNrRm9yUGFyZW50VXJsLCBwYXlsb2FkKTtcbiAgICAgICAgcmV0dXJuIGF3YWl0IHJlc3BvbnNlLmpzb24oKTtcbiAgICB9XG5cbiAgICBhc3luYyBjaGVja0NoYXREQlRpbWUoY2hhdElkOiBudW1iZXIpOiBQcm9taXNlPGFueT4ge1xuXG4gICAgICAgIGxldCBwYXlsb2FkID0ge1xuICAgICAgICAgICAgbG9va3VwTmFtZTogXCJDaGF0RHVyYXRpb25EZXRhaWxcIixcbiAgICAgICAgICAgIGZpbHRlcnM6IFt7XG4gICAgICAgICAgICAgICAgbmFtZTogXCJDaGF0SWRcIixcbiAgICAgICAgICAgICAgICB2YWx1ZXM6IFtjaGF0SWQ/LnRvU3RyaW5nKCldXG4gICAgICAgICAgICB9XVxuICAgICAgICB9O1xuICAgICAgICBsZXQgY2hlY2tGb3JQYXJlbnRVcmwgPSB0aGlzLkNYX1NFUlZJQ0VfVVJMICsgdGhpcy5DWF9BUElfVVJMX1BBVEggKyB0aGlzLkNYX0dFVF9QQVJFTlRfUkVQT1JUO1xuICAgICAgICBsZXQgcmVzcG9uc2UgPSBhd2FpdCB0aGlzLmNhbGxQb3N0UmVwb3J0QVBJKGNoZWNrRm9yUGFyZW50VXJsLCBwYXlsb2FkKTtcbiAgICAgICAgcmV0dXJuIGF3YWl0IHJlc3BvbnNlLmpzb24oKTtcbiAgICB9XG5cbiAgICBwcml2YXRlIGFzeW5jIGNhbGxQb3N0UmVwb3J0QVBJKGNoZWNrRm9yUGFyZW50VXJsOiBzdHJpbmcsIHBheWxvYWQ6IFJlcG9ydFBheWxvYWQpIHtcbiAgICAgICAgbGV0IGFwaVJlc3BvbnNlO1xuICAgICAgICB0cnkge1xuICAgICAgICAgICAgYXBpUmVzcG9uc2UgPSBhd2FpdCBmZXRjaChjaGVja0ZvclBhcmVudFVybCwge1xuICAgICAgICAgICAgICAgIG1ldGhvZDogdGhpcy5QT1NUX01FVEhPRCxcbiAgICAgICAgICAgICAgICBoZWFkZXJzOiB7XG4gICAgICAgICAgICAgICAgICAgICdDb250ZW50LVR5cGUnOiB0aGlzLlJFU1RfQ09OVEVOVF9UWVBFLFxuICAgICAgICAgICAgICAgICAgICAnT1N2Qy1DUkVTVC1BcHBsaWNhdGlvbi1Db250ZXh0JzogJ0NoYXQgU2VudGltZW50IEFuYWx5c2lzJyxcbiAgICAgICAgICAgICAgICAgICAgJ09TdkMtQ1JFU1QtVGltZS1VVEMnIDogJ3RydWUnICxcbiAgICAgICAgICAgICAgICAgICAgJ0F1dGhvcml6YXRpb24nOiBgU2Vzc2lvbiAke3RoaXMuc2Vzc2lvblRva2VufWBcbiAgICAgICAgICAgICAgICB9LFxuICAgICAgICAgICAgICAgIGJvZHk6IEpTT04uc3RyaW5naWZ5KHBheWxvYWQpXG4gICAgICAgICAgICB9KTtcbiAgICAgICAgfSBjYXRjaCAoZXJyb3IpIHtcbiAgICAgICAgICAgIHRoaXMuYWRkTG9nKFwiRXJyb3Igd2hpbGUgY2FsbGluZyBcIiArIGNoZWNrRm9yUGFyZW50VXJsICsgXCIgYXBpIFwiICsgZXJyb3IsIExvZ1R5cGVzLkVSUk9SKTtcbiAgICAgICAgfVxuXG5cbiAgICAgICAgcmV0dXJuIGFwaVJlc3BvbnNlO1xuICAgIH1cblxuICAgIHByaXZhdGUgYXN5bmMgY2FsbEdFVEFQSShhcGlVcmw6IHN0cmluZykge1xuICAgICAgICBsZXQgYXBpUmVzcG9uc2U7XG4gICAgICAgIHRyeSB7XG4gICAgICAgICAgICBhcGlSZXNwb25zZSA9IGF3YWl0IGZldGNoKGFwaVVybCwge1xuICAgICAgICAgICAgICAgIG1ldGhvZDogdGhpcy5HRVRfTUVUSE9ELFxuICAgICAgICAgICAgICAgIGhlYWRlcnM6IHtcbiAgICAgICAgICAgICAgICAgICAgJ0NvbnRlbnQtVHlwZSc6IHRoaXMuUkVTVF9DT05URU5UX1RZUEUsXG4gICAgICAgICAgICAgICAgICAgICdPU3ZDLUNSRVNULUFwcGxpY2F0aW9uLUNvbnRleHQnOiAnQ2hhdCBTZW50aW1lbnQgQW5hbHlzaXMnLFxuICAgICAgICAgICAgICAgICAgICAnQXV0aG9yaXphdGlvbic6IGBTZXNzaW9uICR7dGhpcy5zZXNzaW9uVG9rZW59YFxuICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgIH0pO1xuICAgICAgICB9IGNhdGNoIChlcnJvcikge1xuICAgICAgICAgICAgdGhpcy5hZGRMb2coXCJFcnJvciB3aGlsZSBjYWxsaW5nIFwiICsgYXBpVXJsICsgXCIgYXBpIFwiICsgZXJyb3IsIExvZ1R5cGVzLkVSUk9SKTtcbiAgICAgICAgfVxuICAgICAgICByZXR1cm4gYXBpUmVzcG9uc2U7XG4gICAgfVxuXG4gICAgYXN5bmMgc2F2ZUluaXRpYWxQYXJlbnRFbnRyeShlbmdhZ2VtZW50SWQ6IG51bWJlcik6IFByb21pc2U8YW55PiB7XG4gICAgICAgIGxldCBzYXZlUGFyZW50UmVzcG9uc2UgPSBudWxsO1xuICAgICAgICBsZXQgcGF5bG9hZDogU2F2ZVBhcmVudFJlY29yZFJlcXVlc3QgPSBudWxsO1xuXG4gICAgICAgIHBheWxvYWQgPSB7XG4gICAgICAgICAgICBDaGF0SWQ6IGVuZ2FnZW1lbnRJZCxcbiAgICAgICAgICAgIElzRW5kVXNlckluTmVnYXRpdmVFbW90aW9uOiBmYWxzZSxcbiAgICAgICAgICAgIEhhdmVBZ2VudEluTmVnYXRpdmVFbW90aW9uOiBmYWxzZSxcbiAgICAgICAgICAgIElzQWxlcnRUcmlnZ2VyZWQ6IGZhbHNlLFxuICAgICAgICAgICAgUmVxdWVzdE1hbmFnZXJJbnRlcnZlbmU6IGZhbHNlLFxuICAgICAgICAgICAgUmVxdWVzdE1hbmFnZXJJbnRlcnZlbmVDb3VudDogMCxcbiAgICAgICAgICAgIElzQWN0aXZlOiB0cnVlLFxuICAgICAgICAgICAgQ3VycmVudFF1ZXVlOiB7IGlkOiB0aGlzLmVuZ2FnZW1lbnREZXRhaWxzW2VuZ2FnZW1lbnRJZF0ucXVldWVJZCB9LFxuICAgICAgICAgICAgTWF4TmVnYXRpdmVNZXNzYWdlQ291bnQ6IHRoaXMuTUFYX05FR0FUSVZFX0NIQVRfQ09VTlQsXG4gICAgICAgICAgICBNaW5FbW90aW9uQ29uZjogU3RyaW5nKHRoaXMuTUlOX0VNT1RJT05fQ09ORklHKSxcbiAgICAgICAgICAgIE1pblJlcXVlc3RNYW5hZ2VySW50ZXJ2ZW5lQ29uZjogU3RyaW5nKHRoaXMuTUlOX1JFUVVFU1RfTUFOQUdFUl9DT05GSUcpLFxuICAgICAgICAgICAgSW5pdGlhbE1lc3NhZ2VUb1NraXBDb3VudDogdGhpcy5JTklUSUFMX01FU1NBR0VTX1RPX1NLSVAsXG4gICAgICAgICAgICBOZWdhdGl2ZU1lc3NhZ2VDb3VudDogMCxcbiAgICAgICAgICAgIEFjY291bnRJZDogdGhpcy5lbmdhZ2VtZW50RGV0YWlsc1tlbmdhZ2VtZW50SWRdLmFjY291bnRJZCA+IDAgPyB7IGlkOiB0aGlzLmVuZ2FnZW1lbnREZXRhaWxzW2VuZ2FnZW1lbnRJZF0uYWNjb3VudElkIH0gOiBudWxsLFxuICAgICAgICAgICAgQ29udGFjdElkOiB0aGlzLmVuZ2FnZW1lbnREZXRhaWxzW2VuZ2FnZW1lbnRJZF0udXNlcklkID4gMCA/IHsgaWQ6IHRoaXMuZW5nYWdlbWVudERldGFpbHNbZW5nYWdlbWVudElkXS51c2VySWQgfSA6IG51bGwsXG4gICAgICAgICAgICBJc1ByaXZhdGVDaGF0OiB0aGlzLmVuZ2FnZW1lbnREZXRhaWxzW2VuZ2FnZW1lbnRJZF0uaXNDb25mZXJlbmNlID8gdHJ1ZSA6IGZhbHNlXG4gICAgICAgIH07XG5cblxuICAgICAgICBzYXZlUGFyZW50UmVzcG9uc2UgPSBhd2FpdCB0aGlzLmNhbGxQT1NUUmVzdEFQSShwYXlsb2FkLCB0aGlzLkNYX1NFUlZJQ0VfVVJMICsgdGhpcy5DWF9BUElfVVJMX1BBVEggKyB0aGlzLkNYX1BBUkVOVF9PQkpFQ1RfVVJMKTtcbiAgICAgICAgcmV0dXJuIHNhdmVQYXJlbnRSZXNwb25zZT8uanNvbigpO1xuICAgIH1cblxuICAgIGFzeW5jIGV2YWx1YXRlQW5kU2F2ZUNoYXRSZXN1bHQoY2hhdE1lc3NhZ2U6IElDaGF0TWVzc2FnZSwgZW5nYWdlbWVudElkOiBudW1iZXIsIGNoYXRPcmRlcjogbnVtYmVyLCBpc1RoaXNQcml2YXRlQ2hhdDogYm9vbGVhbik6IFByb21pc2U8c3RyaW5nPiB7XG4gICAgICAgIGxldCBzYXZlU2VudGltZW50UmVzcG9uc2U6IGFueSA9IG51bGw7XG4gICAgICAgIGxldCBwYXlsb2FkOiBzYXZlU2VudGltZW50RW1vdGlvblJlcXVlc3QgPSBudWxsO1xuICAgICAgICBsZXQgbWVzc2FnZUJvZHkgPSB0aGlzLnVuRXNjYXBlKGNoYXRNZXNzYWdlLkJvZHkpO1xuICAgICAgICBsZXQgZW1vdGlvblNvcmUgPSAwO1xuICAgICAgICBsZXQgZW1vdGlvbiA9IG51bGw7XG4gICAgICAgIGxldCBzdXBlcnZpc29yID0gbnVsbDtcbiAgICAgICAgbGV0IHN1cGVydmlzb3JTY29yZSA9IDA7XG5cbiAgICAgICAgaWYgKGNoYXRNZXNzYWdlLlNlbmRlciAhPSBSb2xlVHlwZXMuU1lTVEVNKSB7XG4gICAgICAgICAgICBsZXQgZW1vdGlvblJlc3BvbnNlUHJvbWlzZTogYW55O1xuICAgICAgICAgICAgbGV0IHN1cGVydmlzb3JSZXNwb25zZVByb21pc2U6IGFueTtcbiAgICAgICAgICAgIGlmICh0aGlzLklTX0VNT1RJT05fQUNUSVZFICkge1xuICAgICAgICAgICAgICAgIGlmKGNoYXRNZXNzYWdlLlNlbmRlciA9PSBSb2xlVHlwZXMuRU5EX1VTRVIgfHwgKGNoYXRNZXNzYWdlLlNlbmRlciA9PSBSb2xlVHlwZXMuQUdFTlQgJiYgdGhpcy5BR0VOVF9FTU9USU9OX0VORFBPSU5UICE9IHVuZGVmaW5lZCAmJiB0aGlzLkFHRU5UX0VNT1RJT05fRU5EUE9JTlQgIT0gbnVsbCkpIHtcbiAgICAgICAgICAgICAgICAgICAgZW1vdGlvblJlc3BvbnNlUHJvbWlzZSA9IGF3YWl0IHRoaXMuZ2V0RW1vdGlvbihtZXNzYWdlQm9keSwgY2hhdE1lc3NhZ2UuU2VuZGVyKTtcbiAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICB9XG4gICAgICAgICAgICBpZiAodGhpcy5JU19NQU5BR0VSX0FTS19BQ1RJVkUgJiYgY2hhdE1lc3NhZ2UuU2VuZGVyID09IFJvbGVUeXBlcy5FTkRfVVNFUikge1xuICAgICAgICAgICAgICAgIHN1cGVydmlzb3JSZXNwb25zZVByb21pc2UgPSBhd2FpdCB0aGlzLmdldFN1cGVydmlzb3JBc2sobWVzc2FnZUJvZHkpO1xuICAgICAgICAgICAgfVxuXG4gICAgICAgICAgICBsZXQgZW1vdGlvblJlc3BvbnNlID0gYXdhaXQgZW1vdGlvblJlc3BvbnNlUHJvbWlzZTtcbiAgICAgICAgICAgIGlmIChlbW90aW9uUmVzcG9uc2U/Lmhhc093blByb3BlcnR5KFwiZG9jdW1lbnRzXCIpKSB7XG4gICAgICAgICAgICAgICAgbGV0IGVtb3Rpb25WYWx1ZSA9IGVtb3Rpb25SZXNwb25zZT8uZG9jdW1lbnRzWzBdPy50ZXh0Q2xhc3NpZmljYXRpb25bMF0/LmxhYmVsID09IE5FR0FUSVZFX0VNT1RJT04gPyB0aGlzLk5FR0FUSVZFX0xPT0tVUE5BTUUgOiBlbW90aW9uUmVzcG9uc2U/LmRvY3VtZW50c1swXT8udGV4dENsYXNzaWZpY2F0aW9uWzBdPy5sYWJlbCA9PSBQT1NJVElWRV9FTU9USU9OID8gdGhpcy5QT1NJVElWRV9MT09LVVBOQU1FOiB0aGlzLk5FVVRSQUxfTE9PS1VQTkFNRTtcbiAgICAgICAgICAgICAgICBlbW90aW9uU29yZSA9IGVtb3Rpb25SZXNwb25zZT8uZG9jdW1lbnRzWzBdPy50ZXh0Q2xhc3NpZmljYXRpb25bMF0/LnNjb3JlID09IHVuZGVmaW5lZCA/IDAgOiBlbW90aW9uUmVzcG9uc2U/LmRvY3VtZW50c1swXT8udGV4dENsYXNzaWZpY2F0aW9uWzBdPy5zY29yZTtcbiAgICAgICAgICAgICAgICBlbW90aW9uID0gZW1vdGlvblZhbHVlO1xuICAgICAgICAgICAgfSBlbHNlIGlmIChlbW90aW9uUmVzcG9uc2U/Lmhhc093blByb3BlcnR5KFwiZXJyb3JcIikpIHtcbiAgICAgICAgICAgICAgICB0aGlzLmFkZExvZyhcIkVycm9yIHdoaWxlIGNhbGxpbmcgZW1vdGlvbiBtb2RlbFwiLCBMb2dUeXBlcy5FUlJPUik7XG4gICAgICAgICAgICB9XG4gICAgICAgICAgICBpZiAoY2hhdE1lc3NhZ2UuU2VuZGVyID09IFJvbGVUeXBlcy5FTkRfVVNFUikge1xuICAgICAgICAgICAgICAgIGxldCBzdXBlcnZpc29yUmVzcG9uc2UgPSBhd2FpdCBzdXBlcnZpc29yUmVzcG9uc2VQcm9taXNlO1xuICAgICAgICAgICAgICAgIGlmIChzdXBlcnZpc29yUmVzcG9uc2U/Lmhhc093blByb3BlcnR5KFwiZG9jdW1lbnRzXCIpKSB7XG4gICAgICAgICAgICAgICAgICAgIHN1cGVydmlzb3IgPSBzdXBlcnZpc29yUmVzcG9uc2U/LmRvY3VtZW50c1swXT8udGV4dENsYXNzaWZpY2F0aW9uWzBdPy5sYWJlbDtcbiAgICAgICAgICAgICAgICAgICAgc3VwZXJ2aXNvclNjb3JlID0gc3VwZXJ2aXNvclJlc3BvbnNlPy5kb2N1bWVudHNbMF0/LnRleHRDbGFzc2lmaWNhdGlvblswXT8uc2NvcmUgPT0gdW5kZWZpbmVkID8gMCA6c3VwZXJ2aXNvclJlc3BvbnNlPy5kb2N1bWVudHNbMF0/LnRleHRDbGFzc2lmaWNhdGlvblswXT8uc2NvcmU7XG4gICAgICAgICAgICAgICAgfSBlbHNlIGlmIChzdXBlcnZpc29yUmVzcG9uc2U/Lmhhc093blByb3BlcnR5KFwiZXJyb3JcIikpIHtcbiAgICAgICAgICAgICAgICAgICAgdGhpcy5hZGRMb2coXCJFcnJvciB3aGlsZSBjYWxsaW5nIHN1cGVydmlzb3IgbW9kZWxcIiwgTG9nVHlwZXMuRVJST1IpO1xuICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgIH1cbiAgICAgICAgICAgIGxldCBzdXBlcnZpc29yQXNrID0gIHN1cGVydmlzb3IgPT0gbnVsbCA/IG51bGwgOiAoc3VwZXJ2aXNvciA9PSAxICYmICgoc3VwZXJ2aXNvclNjb3JlICogdGhpcy5IVU5EUkVEX01VTFRJUExJRVIpID4gdGhpcy5NSU5fUkVRVUVTVF9NQU5BR0VSX0NPTkZJRykpID8gdHJ1ZSA6IGZhbHNlO1xuICAgICAgICAgICAgcGF5bG9hZCA9IHtcbiAgICAgICAgICAgICAgICBDaGF0QUlSZXN1bHRTdW1tYXJ5OiB7IGlkOiBpc1RoaXNQcml2YXRlQ2hhdCA/IHRoaXMuZW5nYWdlbWVudERldGFpbHNbZW5nYWdlbWVudElkXS5jb25mUGFyZW50SWQgOiB0aGlzLmVuZ2FnZW1lbnREZXRhaWxzW2VuZ2FnZW1lbnRJZF0ucGFyZW50SWQgfSxcbiAgICAgICAgICAgICAgICBDaGF0VGV4dDogbWVzc2FnZUJvZHksXG4gICAgICAgICAgICAgICAgQ2hhdE9yZGVyOiBjaGF0T3JkZXIsXG4gICAgICAgICAgICAgICAgRW1vdGlvbjogbnVsbCA9PSBlbW90aW9uID8gbnVsbCA6IHsgbG9va3VwTmFtZTogZW1vdGlvbiB9LFxuICAgICAgICAgICAgICAgIFN1Z2dlc3RFbW90aW9uOiBudWxsID09IGVtb3Rpb24gPyBudWxsIDogeyBsb29rdXBOYW1lOiBlbW90aW9uIH0sXG4gICAgICAgICAgICAgICAgRW1vdGlvbkNvbmY6IG5ldyBJbnRsLk51bWJlckZvcm1hdCh0aGlzLkVOX0lOX0ZPUk1BVCwgeyBtaW5pbXVtRnJhY3Rpb25EaWdpdHM6IHRoaXMuRlJBQ1RJT05fTEVOR1RILCBtYXhpbXVtRnJhY3Rpb25EaWdpdHM6IHRoaXMuRlJBQ1RJT05fTEVOR1RIIH0pLmZvcm1hdChlbW90aW9uU29yZSAqIHRoaXMuSFVORFJFRF9NVUxUSVBMSUVSKSxcbiAgICAgICAgICAgICAgICBSZXF1ZXN0TWFuYWdlckludGVydmVuZTogc3VwZXJ2aXNvckFzayxcbiAgICAgICAgICAgICAgICBTdWdnZXN0TWFuYWdlckludGVydmVuZTogc3VwZXJ2aXNvckFzayxcbiAgICAgICAgICAgICAgICBSZXF1ZXN0TWFuYWdlckludGVydmVuZUNvbmY6IG5ldyBJbnRsLk51bWJlckZvcm1hdCh0aGlzLkVOX0lOX0ZPUk1BVCwgeyBtaW5pbXVtRnJhY3Rpb25EaWdpdHM6IHRoaXMuRlJBQ1RJT05fTEVOR1RILCBtYXhpbXVtRnJhY3Rpb25EaWdpdHM6IHRoaXMuRlJBQ1RJT05fTEVOR1RIIH0pLmZvcm1hdChzdXBlcnZpc29yU2NvcmUgKiB0aGlzLkhVTkRSRURfTVVMVElQTElFUiksXG4gICAgICAgICAgICAgICAgQ2hhdFJvbGU6IHsgbG9va3VwTmFtZTogY2hhdE1lc3NhZ2UuU2VuZGVyIH1cbiAgICAgICAgICAgIH07XG4gICAgICAgIH0gZWxzZSB7XG4gICAgICAgICAgICBwYXlsb2FkID0ge1xuICAgICAgICAgICAgICAgIENoYXRBSVJlc3VsdFN1bW1hcnk6IHsgaWQ6IGlzVGhpc1ByaXZhdGVDaGF0ID8gdGhpcy5lbmdhZ2VtZW50RGV0YWlsc1tlbmdhZ2VtZW50SWRdLmNvbmZQYXJlbnRJZCA6IHRoaXMuZW5nYWdlbWVudERldGFpbHNbZW5nYWdlbWVudElkXS5wYXJlbnRJZCB9LFxuICAgICAgICAgICAgICAgIENoYXRUZXh0OiBtZXNzYWdlQm9keSxcbiAgICAgICAgICAgICAgICBDaGF0T3JkZXI6IGNoYXRPcmRlcixcbiAgICAgICAgICAgICAgICBSZXF1ZXN0TWFuYWdlckludGVydmVuZTogZmFsc2UsXG4gICAgICAgICAgICAgICAgUmVxdWVzdE1hbmFnZXJJbnRlcnZlbmVDb25mOiAnMC4wMCcsXG4gICAgICAgICAgICAgICAgRW1vdGlvbkNvbmY6ICcwLjAwJyxcbiAgICAgICAgICAgICAgICBDaGF0Um9sZTogeyBsb29rdXBOYW1lOiBjaGF0TWVzc2FnZS5TZW5kZXIgfVxuICAgICAgICAgICAgfTtcbiAgICAgICAgfVxuICAgICAgICBzYXZlU2VudGltZW50UmVzcG9uc2UgPSBhd2FpdCB0aGlzLmNhbGxQT1NUUmVzdEFQSShwYXlsb2FkLCB0aGlzLkNYX1NFUlZJQ0VfVVJMICsgdGhpcy5DWF9BUElfVVJMX1BBVEggKyB0aGlzLkNYX0NISUxEX09CSkVDVF9VUkwpO1xuICAgICAgICByZXR1cm4gc2F2ZVNlbnRpbWVudFJlc3BvbnNlO1xuICAgIH1cblxuICAgIGFzeW5jIGVuZ2FnZW1lbnRBY2NlcHRlZEhhbmRsZXIoZWFFdnQ6IElDaGF0RW5nYWdlbWVudEFjY2VwdGVkRXZlbnRBcmdzLCBhczogSU9yYWNsZUNoYXRDbGllbnQuSUFnZW50U2Vzc2lvbik6IFByb21pc2U8dm9pZD4ge1xuICAgICAgICB0aGlzLmFkZExvZyhcIkluc2lkZSBlbmdhZ2VtZW50QWNjZXB0ZWRcIiwgTG9nVHlwZXMuREVCVUcpO1xuICAgICAgICBpZih0aGlzLmVuZ2FnZW1lbnREZXRhaWxzW2VhRXZ0LkVuZ2FnZW1lbnRJZF0uZGlzcGxheU5hbWUgPT0gXCJcIil7XG4gICAgICAgICAgICBsZXQgbmFtZVJlc3BvbnNlID0gYXdhaXQgdGhpcy5jYWxsR0VUQVBJKHRoaXMuQ1hfU0VSVklDRV9VUkwgKyB0aGlzLkNYX0FQSV9VUkxfUEFUSCArICBcImFjY291bnRzL1wiICsgdGhpcy5lbmdhZ2VtZW50RGV0YWlsc1tlYUV2dC5FbmdhZ2VtZW50SWRdLmFnZW50SWQpO1xuICAgICAgICAgICAgbGV0IG5hbWVSZXNwb25zZUpzb24gPSBhd2FpdCBuYW1lUmVzcG9uc2UuanNvbigpO1xuICAgICAgICAgICAgaWYgKG5hbWVSZXNwb25zZUpzb24/Lmhhc093blByb3BlcnR5KFwiZGlzcGxheU5hbWVcIikpIHtcbiAgICAgICAgICAgICAgICB0aGlzLmVuZ2FnZW1lbnREZXRhaWxzW2VhRXZ0LkVuZ2FnZW1lbnRJZF0uZGlzcGxheU5hbWUgID0gbmFtZVJlc3BvbnNlSnNvbj8uZGlzcGxheU5hbWU7XG4gICAgICAgICAgICB9XG4gICAgICAgIH1cblxuICAgICAgICBpZiAodGhpcy5lbmdhZ2VtZW50RGV0YWlsc1tlYUV2dC5FbmdhZ2VtZW50SWRdLmRpc3BsYXlOYW1lPy5pbmNsdWRlcyhlYUV2dC5BZ2VudE5hbWUpKSB7XG4gICAgICAgICAgICB0aGlzLmFkZExvZyhcIkluc2lkZSBBQ0NFUFRFRCBldmVudFwiLCBMb2dUeXBlcy5ERUJVRyk7XG4gICAgICAgICAgICBsZXQgcGFyZW50QWxyZWFkeVByZXNlbnQgPSBhd2FpdCB0aGlzLmNoZWNrUGFyZW50QWxyZWFkeVByZXNlbnQoZWFFdnQuRW5nYWdlbWVudElkLCBmYWxzZSk7XG4gICAgICAgICAgICBsZXQgZW5nYWdlbWVudCA9IGF3YWl0IGFzLmdldEVuZ2FnZW1lbnQoZWFFdnQuRW5nYWdlbWVudElkKTtcbiAgICAgICAgICAgIGlmIChlbmdhZ2VtZW50Lk15Um9sZSA9PSBSb2xlVHlwZXMuQ09ORkVSRUUpIHtcbiAgICAgICAgICAgICAgICB0aGlzLmVuZ2FnZW1lbnREZXRhaWxzW2VhRXZ0LkVuZ2FnZW1lbnRJZF0uaXNDb25mZXJlbmNlID0gdHJ1ZTtcbiAgICAgICAgICAgIH1cbiAgICAgICAgICAgIGF3YWl0IHRoaXMuY3JlYXRlUGFyZW50RW50cnkoZWFFdnQuRW5nYWdlbWVudElkKTtcbiAgICAgICAgICAgIHRoaXMuYWRkTG9nKFwiIEluc2lkZSBlbmdhZ2VtZW50QWNjZXB0ZWQgYmVmb3JlIGdldEVuZ2FnZW1lbnRcIiArIGVhRXZ0LkVuZ2FnZW1lbnRJZCwgTG9nVHlwZXMuREVCVUcpO1xuICAgICAgICAgICAgYXdhaXQgdGhpcy5hZGRFbmdhZ2VtZW50Q2FsbGJhY2soYXMsIGVhRXZ0LkVuZ2FnZW1lbnRJZCwgcGFyZW50QWxyZWFkeVByZXNlbnQpO1xuICAgICAgICB9XG4gICAgfVxuXG4gICAgYXN5bmMgYWRkRW5nYWdlbWVudENhbGxiYWNrKGFzOiBJT3JhY2xlQ2hhdENsaWVudC5JQWdlbnRTZXNzaW9uLCBlbmdhZ2VtZW50SWQ6IG51bWJlciwgcGFyZW50QWxyZWFkeVByZXNlbnQ6IGFueSk6IFByb21pc2U8dm9pZD4ge1xuICAgICAgICBsZXQgZW5nYWdlbWVudCA9IGF3YWl0IGFzLmdldEVuZ2FnZW1lbnQoZW5nYWdlbWVudElkKTtcbiAgICAgICAgaWYgKCF0aGlzLmVuZ2FnZW1lbnRXaXRoQWN0aW9uUmVnaXN0ZXJlZC5oYXMoZW5nYWdlbWVudElkKSkge1xuICAgICAgICAgICAgZW5nYWdlbWVudC5tZXNzYWdlUG9zdGVkKGFzeW5jIChjaGF0TWVzc2FnZTogSUNoYXRNZXNzYWdlKSA9PiB7XG4gICAgICAgICAgICAgICAgbGV0IGNoYXRPcmRlciA9IHRoaXMubmV4dENoYXRPcmRlcihlbmdhZ2VtZW50SWQpO1xuICAgICAgICAgICAgICAgIGxldCBtZXNzYWdlUG9zdGVkVGltZSA9IG5ldyBEYXRlKGNoYXRNZXNzYWdlLlBvc3RUaW1lKTtcbiAgICAgICAgICAgICAgICB0aGlzLmNoZWNrQW5kVXBkYXRlVXNlcklkVG9QYXJlbnQoZW5nYWdlbWVudElkKTtcblxuXG4gICAgICAgICAgICAgICAgaWYgKG1lc3NhZ2VQb3N0ZWRUaW1lID4gdGhpcy5lbmdhZ2VtZW50RGV0YWlsc1tlbmdhZ2VtZW50SWRdLmFzc2lnbmVkVGltZSkge1xuXG4gICAgICAgICAgICAgICAgICAgIHRoaXMuYWRkTG9nKFwiSW5zaWRlIG1lc3NhZ2UgcG9zdGVkIDpcIiwgTG9nVHlwZXMuREVCVUcpO1xuICAgICAgICAgICAgICAgICAgICBsZXQgaXNUaGlzUHJpdmF0ZUNoYXQgPSBmYWxzZTtcbiAgICAgICAgICAgICAgICAgICAgaWYgKHRoaXMuZW5nYWdlbWVudERldGFpbHNbZW5nYWdlbWVudElkXS5pc0NvbmZlcmVuY2UgfHwgY2hhdE1lc3NhZ2UuVmlzaWJpbGl0eSA9PSBSb2xlVHlwZXMuTk9UX0VORFVTRVIpIHtcbiAgICAgICAgICAgICAgICAgICAgICAgIGlmIChjaGF0TWVzc2FnZS5WaXNpYmlsaXR5ID09IFJvbGVUeXBlcy5OT1RfRU5EVVNFUikge1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGlzVGhpc1ByaXZhdGVDaGF0ID0gdHJ1ZTtcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICAgICAgICAgIGlmICh0aGlzLmVuZ2FnZW1lbnREZXRhaWxzW2VuZ2FnZW1lbnRJZF0uY29uZlBhcmVudElkID09IDApIHtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBsZXQgcGFyZW50QWxyZWFkeVByZXNlbnQgPSBhd2FpdCB0aGlzLmNoZWNrUGFyZW50QWxyZWFkeVByZXNlbnQoZW5nYWdlbWVudElkLCB0cnVlKTtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBpZiAocGFyZW50QWxyZWFkeVByZXNlbnRbJ2NvdW50J10gPiAxKSB7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGxldCBpID0gMTtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgbGV0IGxhc3RQYXJlbnQgPSAwXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHdoaWxlIChpIDwgcGFyZW50QWxyZWFkeVByZXNlbnRbJ3Jvd3MnXS5sZW5ndGgpIHtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRoaXMuZW5nYWdlbWVudERldGFpbHNbZW5nYWdlbWVudElkXS5jb25mUGFyZW50SWRTZXQuYWRkKE51bWJlcihwYXJlbnRBbHJlYWR5UHJlc2VudFsncm93cyddW2ldWzBdKSk7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBsYXN0UGFyZW50ID0gTnVtYmVyKHBhcmVudEFscmVhZHlQcmVzZW50Wydyb3dzJ11baV1bMF0pO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgaSsrO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRoaXMuZW5nYWdlbWVudERldGFpbHNbZW5nYWdlbWVudElkXS5jb25mUGFyZW50SWQgPSBsYXN0UGFyZW50O1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICAgICAgICAgIH1cblxuICAgICAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgICAgIGlmICh0aGlzLmVuZ2FnZW1lbnREZXRhaWxzW2VuZ2FnZW1lbnRJZF0uaXNDb25mZXJlbmNlICYmIGlzVGhpc1ByaXZhdGVDaGF0KSB7XG4gICAgICAgICAgICAgICAgICAgICAgICBpZiAodGhpcy5lbmdhZ2VtZW50RGV0YWlsc1tlbmdhZ2VtZW50SWRdLmNvbmZQYXJlbnRJZFNldC5oYXModGhpcy5lbmdhZ2VtZW50RGV0YWlsc1tlbmdhZ2VtZW50SWRdLnBhcmVudElkKSkge1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGF3YWl0IHRoaXMuZXZhbHVhdGVBbmRTYXZlQ2hhdFJlc3VsdChjaGF0TWVzc2FnZSwgZW5nYWdlbWVudElkLCBjaGF0T3JkZXIsIGlzVGhpc1ByaXZhdGVDaGF0KTtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0aGlzLmFkZExvZyhcInNhdmUgbWVzc2FnZVBvc3RlZCBjaGF0IHdpdGggaWQgOlwiICsgZW5nYWdlbWVudElkLCBMb2dUeXBlcy5ERUJVRyk7XG4gICAgICAgICAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgICAgIH0gZWxzZSBpZiAoIXRoaXMuZW5nYWdlbWVudERldGFpbHNbZW5nYWdlbWVudElkXS5pc0NvbmZlcmVuY2UgJiYgIWlzVGhpc1ByaXZhdGVDaGF0KSB7XG4gICAgICAgICAgICAgICAgICAgICAgICBhd2FpdCB0aGlzLmV2YWx1YXRlQW5kU2F2ZUNoYXRSZXN1bHQoY2hhdE1lc3NhZ2UsIGVuZ2FnZW1lbnRJZCwgY2hhdE9yZGVyLCBmYWxzZSk7XG4gICAgICAgICAgICAgICAgICAgICAgICB0aGlzLmFkZExvZyhcInNhdmUgbWVzc2FnZVBvc3RlZCBjaGF0IHdpdGggaWQgOlwiICsgZW5nYWdlbWVudElkLCBMb2dUeXBlcy5ERUJVRyk7XG4gICAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICB9KTtcbiAgICAgICAgICAgIGVuZ2FnZW1lbnQuY29uY2x1ZGVkKGFzeW5jIChvbkNvbmNsdWRlZDogYW55KSA9PiB7XG4gICAgICAgICAgICAgICAgdGhpcy5hZGRMb2coXCJFbmdhZ2VtZW50IGhhcyBiZWVuIGNvbmNsdWRlZC5cIiArIG9uQ29uY2x1ZGVkLCBMb2dUeXBlcy5ERUJVRyk7XG4gICAgICAgICAgICAgICAgYXdhaXQgdGhpcy5zYXZlQ29uY2x1ZGVkQ2hhdFJlc3VsdChcIkVuZ2FnZW1lbnQgaGFzIGJlZW4gY29uY2x1ZGVkLlwiLCBlbmdhZ2VtZW50SWQsIGZhbHNlKTtcbiAgICAgICAgICAgIH0pO1xuICAgICAgICAgICAgdGhpcy5lbmdhZ2VtZW50V2l0aEFjdGlvblJlZ2lzdGVyZWQuYWRkKGVuZ2FnZW1lbnRJZCk7XG4gICAgICAgIH1cblxuICAgICAgICBsZXQgbWVzc2FnZXMgPSBhd2FpdCBlbmdhZ2VtZW50LmdldE1lc3NhZ2VzKCk7XG4gICAgICAgIHRoaXMuYWRkTG9nKFwicHJvY2VzcyBnZXRNZXNzYWdlczogXCIsIExvZ1R5cGVzLkRFQlVHKTtcbiAgICAgICAgaWYgKHBhcmVudEFscmVhZHlQcmVzZW50Wydjb3VudCddID09IDApIHtcbiAgICAgICAgICAgIG1lc3NhZ2VzLmZvckVhY2goYXN5bmMgKG1lc3NhZ2U6IElDaGF0TWVzc2FnZSkgPT4ge1xuICAgICAgICAgICAgICAgIGxldCBjaGF0T3JkZXIgPSB0aGlzLm5leHRDaGF0T3JkZXIoZW5nYWdlbWVudElkKTtcbiAgICAgICAgICAgICAgICBhd2FpdCB0aGlzLmV2YWx1YXRlQW5kU2F2ZUNoYXRSZXN1bHQobWVzc2FnZSwgZW5nYWdlbWVudElkLCBjaGF0T3JkZXIsIGZhbHNlKTtcbiAgICAgICAgICAgICAgICB0aGlzLmFkZExvZyhcInNhdmUgZ2V0IG1lc3NhZ2UgY2hhdCAgd2l0aCBpZCA6IFwiLCBMb2dUeXBlcy5ERUJVRyk7XG4gICAgICAgICAgICB9KTtcbiAgICAgICAgfVxuICAgICAgICB0aGlzLmNoZWNrQW5kVXBkYXRlVXNlcklkVG9QYXJlbnQoZW5nYWdlbWVudElkKTtcblxuICAgIH1cblxuICAgIHByaXZhdGUgY2hlY2tBbmRVcGRhdGVVc2VySWRUb1BhcmVudChlbmdhZ2VtZW50SWQ6IG51bWJlcikge1xuICAgICAgICBpZiAodGhpcy5lbmdhZ2VtZW50RGV0YWlsc1tlbmdhZ2VtZW50SWRdLnVzZXJJZCA8PSAwKSB7XG4gICAgICAgICAgICBsZXQgZW50aXR5TGlzdCA9KE9SQUNMRV9TRVJWSUNFX0NMT1VEIGFzIGFueSkuZ2xvYmFsQ29udGV4dExpc3RlbmVyLmdsb2JhbENvbnRleHREYXRhLmVudGl0eUxpc3Q7XG4gICAgICAgICAgICBlbnRpdHlMaXN0LmZvckVhY2goYXN5bmMgKGVudGl0eTogYW55KSA9PiB7XG4gICAgICAgICAgICAgICAgbGV0IG9iamVjdFR5cGUgPSBlbnRpdHkub2JqZWN0VHlwZTtcbiAgICAgICAgICAgICAgICBsZXQgb2JqZWN0SWQgPSBlbnRpdHkub2JqZWN0SWQ7XG4gICAgICAgICAgICAgICAgaWYgKG9iamVjdFR5cGUgPT0gXCJJbnRlcmFjdGlvblwiKSB7XG4gICAgICAgICAgICAgICAgICAgIGxldCB3cyA9IGF3YWl0IHRoaXMuZ2V0V29ya3NwYWNlUmVjb3JkKG9iamVjdElkLCBvYmplY3RUeXBlKTtcbiAgICAgICAgICAgICAgICAgICAgbGV0IHdvcmtHcm91cENvbnRleHQgPSBhd2FpdCB3cy5nZXRXb3JrR3JvdXBDb250ZXh0KCk7XG4gICAgICAgICAgICAgICAgICAgIGxldCB3b3JrR3JvdXBFbnRpdHkgPSBhd2FpdCB3b3JrR3JvdXBDb250ZXh0LmdldFdvcmtHcm91cEVudGl0aWVzKCk7XG4gICAgICAgICAgICAgICAgICAgIHdvcmtHcm91cEVudGl0eS5mb3JFYWNoKGFzeW5jICh3b3JrRW50aXR5OiBhbnkpID0+IHtcbiAgICAgICAgICAgICAgICAgICAgICAgIGlmICh3b3JrRW50aXR5Lm9iamVjdElkID09IGVuZ2FnZW1lbnRJZCAmJiB3b3JrRW50aXR5Lm9iamVjdFR5cGUgPT0gJ0NoYXQnKSB7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgbGV0IGZpZWxkczogT1JBQ0xFX1NFUlZJQ0VfQ0xPVUQuSUZpZWxkRGV0YWlscyA9IGF3YWl0IHdzLmdldEZpZWxkVmFsdWVzKFsnSW50ZXJhY3Rpb24uQ0lkJ10pO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGxldCB2YWx1ZSA9IE51bWJlcihmaWVsZHMuZ2V0RmllbGQoJ0ludGVyYWN0aW9uLkNJZCcpLmdldFZhbHVlKCkpO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGlmKHZhbHVlID4gMCkge1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB0aGlzLnVwZGF0ZVBhcmVudFdpdGhDb250YWN0KHZhbHVlLCBlbmdhZ2VtZW50SWQpO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICAgICAgfSk7XG4gICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgfSk7XG4gICAgICAgIH1cbiAgICB9XG5cblxuICAgIHByaXZhdGUgdXBkYXRlUGFyZW50V2l0aENvbnRhY3QodmFsdWU6IG51bWJlciwgZW5nYWdlbWVudElkOiBudW1iZXIpIHtcbiAgICAgICAgbGV0IHBheWxvYWQgPSB7XG4gICAgICAgICAgICBDb250YWN0SWQ6IHsgaWQ6IHZhbHVlIH1cbiAgICAgICAgfTtcbiAgICAgICAgdGhpcy51cGRhdGVQYXJlbnQocGF5bG9hZCwgdGhpcy5lbmdhZ2VtZW50RGV0YWlsc1tlbmdhZ2VtZW50SWRdLnBhcmVudElkKTtcbiAgICAgICAgdGhpcy5lbmdhZ2VtZW50RGV0YWlsc1tlbmdhZ2VtZW50SWRdLnVzZXJJZCA9IHZhbHVlO1xuICAgIH1cblxuICAgIHByaXZhdGUgbmV4dENoYXRPcmRlcihlbmdhZ2VtZW50SWQ6IG51bWJlcikge1xuICAgICAgICBsZXQgY2hhdE9yZGVyID0gdGhpcy5lbmdhZ2VtZW50RGV0YWlsc1tlbmdhZ2VtZW50SWRdLmNoYXRPcmRlcjtcbiAgICAgICAgdGhpcy5lbmdhZ2VtZW50RGV0YWlsc1tlbmdhZ2VtZW50SWRdLmNoYXRPcmRlciA9IGNoYXRPcmRlciArIDE7XG4gICAgICAgIHJldHVybiBjaGF0T3JkZXI7XG4gICAgfVxuXG4gICAgYXN5bmMgY3JlYXRlUGFyZW50RW50cnkoZW5nYWdlbWVudElkOiBudW1iZXIpOiBQcm9taXNlPHZvaWQ+IHtcbiAgICAgICAgaWYgKHRoaXMuZW5nYWdlbWVudERldGFpbHNbZW5nYWdlbWVudElkXS5wYXJlbnRJZCA9PSAwKSB7XG4gICAgICAgICAgICBsZXQgcGFyZW50UmVzcG9uc2UgPSBhd2FpdCB0aGlzLnNhdmVJbml0aWFsUGFyZW50RW50cnkoZW5nYWdlbWVudElkKTtcbiAgICAgICAgICAgIGlmICh0aGlzLmVuZ2FnZW1lbnREZXRhaWxzW2VuZ2FnZW1lbnRJZF0ucGFyZW50SWQgPT0gMCkge1xuICAgICAgICAgICAgICAgIHRoaXMuZW5nYWdlbWVudERldGFpbHNbZW5nYWdlbWVudElkXS5wYXJlbnRJZCA9IHBhcmVudFJlc3BvbnNlLmlkO1xuICAgICAgICAgICAgfVxuICAgICAgICAgICAgYXdhaXQgdGhpcy5jaGVja0FuZFVwZGF0ZVByb2R1Y3REQlRpbWUoZW5nYWdlbWVudElkLCBmYWxzZSk7XG5cbiAgICAgICAgICAgIHRoaXMuYWRkTG9nKFwiUGFyZW50IFJlY29yZCBjcmVhdGVkIHdpdGggaWRcIiArIHBhcmVudFJlc3BvbnNlLmlkLCBMb2dUeXBlcy5ERUJVRyk7XG4gICAgICAgIH1cbiAgICB9XG5cbiAgICBhc3luYyBlbmdhZ2VtZW50UmVtb3ZlZEhhbmRsZXIoZW5nUmVtb3ZlZEV2dEFyZ3M6IElDaGF0RW5nYWdlbWVudFJlbW92ZWRFdmVudEFyZ3MpOiBQcm9taXNlPHZvaWQ+IHtcbiAgICAgICAgdGhpcy5hZGRMb2coXCJFbmdhZ2VtZW50IGhhcyBiZWVuIHJlbW92ZWQuXCIgKyBlbmdSZW1vdmVkRXZ0QXJncy5FbmdhZ2VtZW50SWQsIExvZ1R5cGVzLkRFQlVHKTtcbiAgICAgICAgYXdhaXQgdGhpcy5zYXZlQ29uY2x1ZGVkQ2hhdFJlc3VsdChcIkVuZ2FnZW1lbnQgaGFzIGJlZW4gY29uY2x1ZGVkLlwiLCBlbmdSZW1vdmVkRXZ0QXJncy5FbmdhZ2VtZW50SWQsIHRydWUpO1xuICAgICAgICBhd2FpdCB0aGlzLmNoZWNrQW5kVXBkYXRlUHJvZHVjdERCVGltZShlbmdSZW1vdmVkRXZ0QXJncy5FbmdhZ2VtZW50SWQsIHRydWUpO1xuICAgIH1cblxuICAgIHByaXZhdGUgYXN5bmMgY2hlY2tBbmRVcGRhdGVQcm9kdWN0REJUaW1lKGVuZ2FnZW1lbnRJZDogbnVtYmVyLCBjaGVja0ZvckNvbXBsZXRlZDogYm9vbGVhbikge1xuICAgICAgICBsZXQgaSA9IDA7XG4gICAgICAgIHdoaWxlIChpIDwgMTApIHtcbiAgICAgICAgICAgIGF3YWl0IG5ldyBQcm9taXNlKChyZXNvbHZlKSA9PiBzZXRUaW1lb3V0KHJlc29sdmUsIDEwMDApKTtcbiAgICAgICAgICAgIGxldCByZXNwb25zZSA9IGF3YWl0IHRoaXMuY2hlY2tDaGF0REJUaW1lKGVuZ2FnZW1lbnRJZCk7XG4gICAgICAgICAgICBpZiAocmVzcG9uc2VbJ2NvdW50J10gPiAwKSB7XG5cbiAgICAgICAgICAgICAgICBpZiAocmVzcG9uc2VbJ3Jvd3MnXVswXVsxXSB8fCByZXNwb25zZVsncm93cyddWzBdWzJdKSB7XG4gICAgICAgICAgICAgICAgICAgIGxldCBwYXlsb2FkID0ge1xuICAgICAgICAgICAgICAgICAgICAgICAgRmlyc3RFbmdhZ2VtZW50VGltZTogcmVzcG9uc2VbJ3Jvd3MnXVswXVsxXSxcbiAgICAgICAgICAgICAgICAgICAgICAgIENvbXBsZXRlZFRpbWU6IHJlc3BvbnNlWydyb3dzJ11bMF1bMl1cbiAgICAgICAgICAgICAgICAgICAgfTtcbiAgICAgICAgICAgICAgICAgICAgYXdhaXQgdGhpcy51cGRhdGVQYXJlbnQocGF5bG9hZCwgdGhpcy5lbmdhZ2VtZW50RGV0YWlsc1tlbmdhZ2VtZW50SWRdLnBhcmVudElkKTtcblxuICAgICAgICAgICAgICAgICAgICBpZihjaGVja0ZvckNvbXBsZXRlZCAmJiByZXNwb25zZVsncm93cyddWzBdWzJdKXtcbiAgICAgICAgICAgICAgICAgICAgICAgIGkgPSAyMDtcbiAgICAgICAgICAgICAgICAgICAgfSBlbHNlIGlmKCAhY2hlY2tGb3JDb21wbGV0ZWQgJiYgcmVzcG9uc2VbJ3Jvd3MnXVswXVsxXSkge1xuICAgICAgICAgICAgICAgICAgICAgICAgaSA9IDIwO1xuICAgICAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgfVxuICAgICAgICAgICAgaSsrO1xuICAgICAgICB9XG4gICAgfVxuXG4gICAgYXN5bmMgc2F2ZUNvbmNsdWRlZENoYXRSZXN1bHQobWVzc2FnZUJvZHk6IHN0cmluZywgZW5nYWdlbWVudElkOiBudW1iZXIsIHNlbmRNZXNzYWdlOiBib29sZWFuKTogUHJvbWlzZTxSZXNwb25zZT4ge1xuICAgICAgICBsZXQgc2F2ZUNvbmNsdWRlZENoYXRSZXNwb25zZSA9IG51bGw7XG4gICAgICAgIGxldCBwYXlsb2FkID0gbnVsbDtcbiAgICAgICAgbGV0IHBhcmVudElkID0gdGhpcy5lbmdhZ2VtZW50RGV0YWlsc1tlbmdhZ2VtZW50SWRdLnBhcmVudElkO1xuICAgICAgICBpZiAoc2VuZE1lc3NhZ2UpIHtcbiAgICAgICAgICAgIHBheWxvYWQgPSB7XG4gICAgICAgICAgICAgICAgQ2hhdEFJUmVzdWx0U3VtbWFyeTogeyBpZDogcGFyZW50SWQgfSxcbiAgICAgICAgICAgICAgICBDaGF0VGV4dDogbWVzc2FnZUJvZHksXG4gICAgICAgICAgICAgICAgQ2hhdE9yZGVyOiB0aGlzLm5leHRDaGF0T3JkZXIoZW5nYWdlbWVudElkKSxcbiAgICAgICAgICAgICAgICBSZXF1ZXN0TWFuYWdlckludGVydmVuZUNvbmY6ICcwLjAwJyxcbiAgICAgICAgICAgICAgICBFbW90aW9uQ29uZjogJzAuMDAnLFxuICAgICAgICAgICAgICAgIENoYXRSb2xlOiB7IGxvb2t1cE5hbWU6IFJvbGVUeXBlcy5TWVNURU0gfVxuICAgICAgICAgICAgfTtcbiAgICAgICAgICAgIHNhdmVDb25jbHVkZWRDaGF0UmVzcG9uc2UgPSBhd2FpdCB0aGlzLmNhbGxQT1NUUmVzdEFQSShwYXlsb2FkLCB0aGlzLkNYX1NFUlZJQ0VfVVJMICsgdGhpcy5DWF9BUElfVVJMX1BBVEggKyB0aGlzLkNYX0NISUxEX09CSkVDVF9VUkwpO1xuICAgICAgICB9XG5cbiAgICAgICAgYXdhaXQgdGhpcy51cGRhdGVGaW5hbFBhcmVudEVudHJ5KHBhcmVudElkLCBmYWxzZSk7XG4gICAgICAgIHJldHVybiBzYXZlQ29uY2x1ZGVkQ2hhdFJlc3BvbnNlO1xuICAgIH1cblxuICAgIGFzeW5jIHVwZGF0ZUZpbmFsUGFyZW50RW50cnkocGFyZW50SWQ6IG51bWJlciwgaXNBY3RpdmU6IGJvb2xlYW4pOiBQcm9taXNlPFJlc3BvbnNlPiB7XG4gICAgICAgIGxldCBwYXlsb2FkID0gbnVsbDtcbiAgICAgICAgcGF5bG9hZCA9IHtcbiAgICAgICAgICAgIElzQWN0aXZlOiBpc0FjdGl2ZVxuICAgICAgICB9O1xuICAgICAgICByZXR1cm4gYXdhaXQgdGhpcy51cGRhdGVQYXJlbnQocGF5bG9hZCwgcGFyZW50SWQpO1xuICAgIH1cblxuICAgIGFzeW5jIGdldEVtb3Rpb24obWVzc2FnZTogc3RyaW5nLCBzZW5kZXI6IHN0cmluZykge1xuXG4gICAgICAgIGxldCBhcGlQYXRoOiBzdHJpbmcgPSAnJztcbiAgICAgICAgbGV0IGVucG9pbnQgOiBzdHJpbmcgPSAnJztcbiAgICAgICAgaWYgKHNlbmRlciA9PSBSb2xlVHlwZXMuQUdFTlQpIHtcbiAgICAgICAgICAgIGVucG9pbnQgPSB0aGlzLkFHRU5UX0VNT1RJT05fRU5EUE9JTlQ7XG4gICAgICAgIH0gZWxzZSB7XG4gICAgICAgICAgICBlbnBvaW50ID0gdGhpcy5FTU9USU9OX0VORFBPSU5UO1xuICAgICAgICB9XG4gICAgICAgIGNvbnN0IHBheWxvYWQgPSB7XCJkb2N1bWVudHNcIjpbe1wia2V5XCI6XCIxXCIsXCJ0ZXh0XCI6bWVzc2FnZSxcImxhbmd1YWdlQ29kZVwiOlwiZW5cIn1dLFwiZW5kcG9pbnRJZFwiOmVucG9pbnR9O1xuICAgICAgICByZXR1cm4gYXdhaXQgdGhpcy5nZXRYT1Jlc3BvbnNlKGFwaVBhdGgsIHBheWxvYWQsIHRoaXMuRU1PVElPTl9YT19OQU1FKTtcbiAgICB9XG5cbiAgICBhc3luYyBnZXRTdXBlcnZpc29yQXNrKG1lc3NhZ2U6IHN0cmluZykge1xuICAgICAgICBjb25zdCBwYXlsb2FkID0ge1wiZG9jdW1lbnRzXCI6W3tcImtleVwiOlwiMVwiLFwidGV4dFwiOm1lc3NhZ2UsXCJsYW5ndWFnZUNvZGVcIjpcImVuXCJ9XSxcImVuZHBvaW50SWRcIjp0aGlzLlNVUEVSVklTT1JfRU5EUE9JTlR9O1xuICAgICAgICBsZXQgYXBpUGF0aCA9IFwiXCI7XG4gICAgICAgIHJldHVybiBhd2FpdCB0aGlzLmdldFhPUmVzcG9uc2UoYXBpUGF0aCwgcGF5bG9hZCwgdGhpcy5TVVBFUlZJU09SX1hPX05BTUUpO1xuICAgIH1cblxuICAgIHByaXZhdGUgYXN5bmMgZ2V0WE9SZXNwb25zZShhcGlQYXRoOiBzdHJpbmcsIHBheWxvYWQ6IGFueSwgY29ubmVjdGlvbk5hbWUgOiBzdHJpbmcpOiBQcm9taXNlPGFueT4ge1xuICAgICAgICBsZXQgcmVzcG9uc2UgPSBudWxsO1xuICAgICAgICB0cnkge1xuICAgICAgICAgICAgbGV0IGdsb2JhbENvbnRleHQgPSBhd2FpdCB0aGlzLmdldEdsb2JhbENvbnRleHQoKTtcbiAgICAgICAgICAgIGxldCBjb250YWluZXJDdHggPSBhd2FpdCBnbG9iYWxDb250ZXh0LmdldENvbnRhaW5lckNvbnRleHQoKTtcbiAgICAgICAgICAgIGxldCBleHRlbnNpb25Db250ZXh0ID0gYXdhaXQgZ2xvYmFsQ29udGV4dC5nZXRFeHRlbnNpb25Db250ZXh0KGNvbnRhaW5lckN0eC5leHRlbnNpb25OYW1lKTtcbiAgICAgICAgICAgIGxldCBjb25uZWN0aW9uQ29sbGVjdGlvbiA9IGF3YWl0IGV4dGVuc2lvbkNvbnRleHQuZ2V0Q29ubmVjdGlvbnMoY29ubmVjdGlvbk5hbWUpO1xuICAgICAgICAgICAgY29uc3QgY29ubmVjdGlvbiA9IGNvbm5lY3Rpb25Db2xsZWN0aW9uLmdldChjb25uZWN0aW9uTmFtZSk7XG4gICAgICAgICAgICBjb25uZWN0aW9uLm9wZW4odGhpcy5QT1NUX01FVEhPRCwgYXBpUGF0aCk7XG4gICAgICAgICAgICBjb25uZWN0aW9uLnNldENvbnRlbnRUeXBlKHRoaXMuUkVTVF9DT05URU5UX1RZUEUpO1xuICAgICAgICAgICAgbGV0IG5lZWRSZXRyeSA9IGZhbHNlO1xuICAgICAgICAgICAgdHJ5IHtcbiAgICAgICAgICAgICAgICByZXNwb25zZSA9IGF3YWl0IGNvbm5lY3Rpb24uc2VuZChKU09OLnN0cmluZ2lmeShwYXlsb2FkKSk7XG4gICAgICAgICAgICB9IGNhdGNoIChleGNlcHRpb25WYXIpIHtcbiAgICAgICAgICAgICAgICB0aGlzLmFkZExvZyhcIkVycm9yIG9jY3VycmVkIGluIFwiICsgYXBpUGF0aCArIFwiIGNhbGwgOlwiICsgZXhjZXB0aW9uVmFyLCBMb2dUeXBlcy5FUlJPUik7XG4gICAgICAgICAgICAgICAgaWYgKGV4Y2VwdGlvblZhci5zdGF0dXMgPT0gNTA0KSB7XG4gICAgICAgICAgICAgICAgICAgIG5lZWRSZXRyeSA9IHRydWU7XG4gICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgfVxuICAgICAgICAgICAgaWYgKG5lZWRSZXRyeSkge1xuICAgICAgICAgICAgICAgIHRoaXMuYWRkTG9nKFwiUmV0cnkgZHVlIHRvIHRpbWVvdXQgXCIgKyBhcGlQYXRoLCBMb2dUeXBlcy5FUlJPUik7XG4gICAgICAgICAgICAgICAgcmVzcG9uc2UgPSBhd2FpdCBjb25uZWN0aW9uLnNlbmQoSlNPTi5zdHJpbmdpZnkocGF5bG9hZCkpO1xuICAgICAgICAgICAgfVxuXG5cbiAgICAgICAgfSBjYXRjaCAoZXJyb3IpIHtcbiAgICAgICAgICAgIHRoaXMuYWRkTG9nKFwiRXJyb3Igd2hpbGUgY2FsbGluZyBcIiArIGFwaVBhdGggKyBcIiBtb2RlbFwiICsgZXJyb3IsIExvZ1R5cGVzLkVSUk9SKTtcblxuICAgICAgICB9XG4gICAgICAgIHJldHVybiByZXNwb25zZTtcblxuICAgIH1cblxuICAgIGFzeW5jIGNhbGxQT1NUUmVzdEFQSShwYXlsb2FkOiBhbnksIHVybDogc3RyaW5nKTogUHJvbWlzZTxSZXNwb25zZT4ge1xuICAgICAgICBsZXQgcmVzcG9uc2UgPSBhd2FpdCBmZXRjaCh1cmwsIHtcbiAgICAgICAgICAgIG1ldGhvZDogdGhpcy5QT1NUX01FVEhPRCxcbiAgICAgICAgICAgIGhlYWRlcnM6IHtcbiAgICAgICAgICAgICAgICAnQ29udGVudC1UeXBlJzogdGhpcy5SRVNUX0NPTlRFTlRfVFlQRSxcbiAgICAgICAgICAgICAgICAnT1N2Qy1DUkVTVC1BcHBsaWNhdGlvbi1Db250ZXh0JzogJ0NoYXQgU2VudGltZW50IHNhdmUgY2hpbGQnLFxuICAgICAgICAgICAgICAgICdBdXRob3JpemF0aW9uJzogYFNlc3Npb24gJHt0aGlzLnNlc3Npb25Ub2tlbn1gXG4gICAgICAgICAgICB9LFxuICAgICAgICAgICAgYm9keTogSlNPTi5zdHJpbmdpZnkocGF5bG9hZClcbiAgICAgICAgfSk7XG4gICAgICAgIHJldHVybiByZXNwb25zZTtcblxuICAgIH1cblxuICAgIGFzeW5jIHVwZGF0ZVBhcmVudChwYXlsb2FkOiBPYmplY3QsIGlkOiBudW1iZXIpOiBQcm9taXNlPFJlc3BvbnNlPiB7XG4gICAgICAgIGxldCByZXNwb25zZSA9IGF3YWl0IGZldGNoKHRoaXMuQ1hfU0VSVklDRV9VUkwgKyB0aGlzLkNYX0FQSV9VUkxfUEFUSCArIHRoaXMuQ1hfUEFSRU5UX09CSkVDVF9VUkwgKyBpZCwge1xuICAgICAgICAgICAgbWV0aG9kOiAnUEFUQ0gnLFxuICAgICAgICAgICAgaGVhZGVyczoge1xuICAgICAgICAgICAgICAgICdDb250ZW50LVR5cGUnOiB0aGlzLlJFU1RfQ09OVEVOVF9UWVBFLFxuICAgICAgICAgICAgICAgICdPU3ZDLUNSRVNULUFwcGxpY2F0aW9uLUNvbnRleHQnOiAnQ2hhdCBTZW50aW1lbnQgQW5hbHlzaXMgdXBkYXRlJyxcbiAgICAgICAgICAgICAgICAnQXV0aG9yaXphdGlvbic6IGBTZXNzaW9uICR7dGhpcy5zZXNzaW9uVG9rZW59YFxuICAgICAgICAgICAgfSxcbiAgICAgICAgICAgIGJvZHk6IEpTT04uc3RyaW5naWZ5KHBheWxvYWQpXG4gICAgICAgIH0pO1xuICAgICAgICByZXR1cm4gcmVzcG9uc2U7XG5cbiAgICB9XG5cbiAgICB1bkVzY2FwZShodG1sU3RyOiBzdHJpbmcpOiBzdHJpbmcge1xuICAgICAgICBsZXQgdGV4dGFyZWEgPSB0aGlzLnR4dENvbnZlcnRFbGVtZW50O1xuICAgICAgICB0ZXh0YXJlYS5pbm5lckhUTUwgPSBodG1sU3RyO1xuICAgICAgICByZXR1cm4gdGV4dGFyZWEudmFsdWU/LnJlcGxhY2UoLyg8KFtePl0rKT4pL2lnLCAnJyk7XG4gICAgfVxuXG4gICAgYXN5bmMgbWFrZUdFVEFQSVJlcXVlc3QodXJsOiBzdHJpbmcpOiBQcm9taXNlPFJlc3BvbnNlPiB7XG4gICAgICAgIGxldCByZXNwb25zZSA9IGF3YWl0IGZldGNoKHRoaXMuQ1hfU0VSVklDRV9VUkwgKyB0aGlzLkNYX0FQSV9VUkxfUEFUSCArIHVybCwge1xuICAgICAgICAgICAgbWV0aG9kOiAnR0VUJyxcbiAgICAgICAgICAgIGhlYWRlcnM6IHtcbiAgICAgICAgICAgICAgICAnQ29udGVudC1UeXBlJzogdGhpcy5SRVNUX0NPTlRFTlRfVFlQRSxcbiAgICAgICAgICAgICAgICAnT1N2Qy1DUkVTVC1BcHBsaWNhdGlvbi1Db250ZXh0JzogJ0NoYXQgU2VudGltZW50IEFuYWx5c2lzJyxcbiAgICAgICAgICAgICAgICAnQXV0aG9yaXphdGlvbic6IGBTZXNzaW9uICR7dGhpcy5zZXNzaW9uVG9rZW59YFxuICAgICAgICAgICAgfVxuICAgICAgICB9KTtcbiAgICAgICAgcmV0dXJuIHJlc3BvbnNlO1xuICAgIH1cblxuICAgIGFzeW5jIHByb2Nlc3NDb25maWd1cmF0aW9ucyhpdGVtOiBhbnkpOiBQcm9taXNlPHZvaWQ+IHtcbiAgICAgICAgbGV0IHJlc3BvbnNlID0gYXdhaXQgdGhpcy5tYWtlR0VUQVBJUmVxdWVzdCgnY29uZmlndXJhdGlvbnMvJyArIGl0ZW0uaWQpO1xuICAgICAgICBsZXQgcmVzcG9uc2Vib2R5ID0gYXdhaXQgcmVzcG9uc2UuanNvbigpO1xuICAgICAgICBsZXQgY29uZmlnID0gSlNPTi5wYXJzZShyZXNwb25zZWJvZHk/LnZhbHVlKTtcbiAgICAgICAgdGhpcy5JTklUSUFMX01FU1NBR0VTX1RPX1NLSVAgPSBjb25maWc/LklOSVRJQUxfTUVTU0FHRVNfVE9fU0tJUDtcbiAgICAgICAgdGhpcy5NQVhfTkVHQVRJVkVfQ0hBVF9DT1VOVCA9IGNvbmZpZz8uTUFYX05FR0FUSVZFX0NIQVRfQ09VTlQ7XG4gICAgICAgIHRoaXMuTUlOX1JFUVVFU1RfTUFOQUdFUl9DT05GSUcgPSBjb25maWc/Lk1JTl9SRVFVRVNUX01BTkFHRVJfQ09ORklHO1xuICAgICAgICB0aGlzLk1JTl9FTU9USU9OX0NPTkZJRyA9IGNvbmZpZz8uTUlOX0VNT1RJT05fQ09ORklHO1xuICAgICAgICB0aGlzLklTX0VNT1RJT05fQUNUSVZFID0gY29uZmlnPy5JU19FTU9USU9OX0FDVElWRTtcbiAgICAgICAgdGhpcy5JU19NQU5BR0VSX0FTS19BQ1RJVkUgPSBjb25maWc/LklTX01BTkFHRVJfQVNLX0FDVElWRTtcbiAgICAgICAgdGhpcy5FTU9USU9OX1hPX05BTUUgPSBjb25maWc/LkVNT1RJT05fWE9fTkFNRTtcbiAgICAgICAgdGhpcy5TVVBFUlZJU09SX1hPX05BTUUgPSBjb25maWc/LlNVUEVSVklTT1JfWE9fTkFNRTtcbiAgICAgICAgdGhpcy5TVVBFUlZJU09SX0VORFBPSU5UID0gY29uZmlnPy5TVVBFUlZJU09SX0VORFBPSU5UO1xuICAgICAgICB0aGlzLkVNT1RJT05fRU5EUE9JTlQgPSBjb25maWc/LkVNT1RJT05fRU5EUE9JTlQ7XG4gICAgICAgIHRoaXMuQUdFTlRfRU1PVElPTl9FTkRQT0lOVCA9IGNvbmZpZz8uQUdFTlRfRU1PVElPTl9FTkRQT0lOVDtcbiAgICAgICAgaWYoY29uZmlnPy5ORUdBVElWRV9MT09LVVBOQU1FKSB7XG4gICAgICAgICAgICB0aGlzLk5FR0FUSVZFX0xPT0tVUE5BTUUgPSBjb25maWc/Lk5FR0FUSVZFX0xPT0tVUE5BTUU7XG4gICAgICAgIH1cbiAgICAgICAgaWYoY29uZmlnPy5QT1NJVElWRV9MT09LVVBOQU1FKSB7XG4gICAgICAgICAgICB0aGlzLlBPU0lUSVZFX0xPT0tVUE5BTUUgPSBjb25maWc/LlBPU0lUSVZFX0xPT0tVUE5BTUU7XG4gICAgICAgIH1cbiAgICAgICAgaWYoY29uZmlnPy5ORVVUUkFMX0xPT0tVUE5BTUUpIHtcbiAgICAgICAgICAgIHRoaXMuTkVVVFJBTF9MT09LVVBOQU1FID0gY29uZmlnPy5ORVVUUkFMX0xPT0tVUE5BTUU7XG4gICAgICAgIH1cbiAgICB9XG5cbiAgICBwdWJsaWMgYWRkTG9nKG1lc3NhZ2U6IHN0cmluZywgbG9nVHlwZTogTG9nVHlwZXMpOiB2b2lkIHtcbiAgICAgICAgT1JBQ0xFX1NFUlZJQ0VfQ0xPVUQuZXh0ZW5zaW9uX2xvYWRlci5sb2FkKHRoaXMuQVBQX05BTUUpLnRoZW4oKGV4dGVuc2lvblByb3ZpZGVyOiBJRXh0ZW5zaW9uUHJvdmlkZXIpID0+IHtcbiAgICAgICAgICAgIGNvbnN0IGRlZmF1bHRMb2dnZXI6IElFeHRlbnNpb25Mb2dnZXIgPSBleHRlbnNpb25Qcm92aWRlci5nZXRMb2dnZXIoKTtcbiAgICAgICAgICAgIHN3aXRjaCAobG9nVHlwZSkge1xuICAgICAgICAgICAgICAgIGNhc2UgTG9nVHlwZXMuRVJST1I6XG4gICAgICAgICAgICAgICAgICAgIGRlZmF1bHRMb2dnZXIuZXJyb3IobWVzc2FnZSk7XG4gICAgICAgICAgICAgICAgICAgIGJyZWFrO1xuICAgICAgICAgICAgICAgIGNhc2UgTG9nVHlwZXMuV0FSTjpcbiAgICAgICAgICAgICAgICAgICAgZGVmYXVsdExvZ2dlci53YXJuKG1lc3NhZ2UpO1xuICAgICAgICAgICAgICAgICAgICBicmVhaztcbiAgICAgICAgICAgICAgICBjYXNlIExvZ1R5cGVzLklORk86XG4gICAgICAgICAgICAgICAgICAgIGRlZmF1bHRMb2dnZXIuaW5mbyhtZXNzYWdlKTtcbiAgICAgICAgICAgICAgICAgICAgYnJlYWs7XG4gICAgICAgICAgICAgICAgY2FzZSBMb2dUeXBlcy5UUkFDRTpcbiAgICAgICAgICAgICAgICAgICAgZGVmYXVsdExvZ2dlci50cmFjZShtZXNzYWdlKTtcbiAgICAgICAgICAgICAgICAgICAgYnJlYWs7XG4gICAgICAgICAgICAgICAgZGVmYXVsdDpcbiAgICAgICAgICAgICAgICAgICAgZGVmYXVsdExvZ2dlci53YXJuKG1lc3NhZ2UpO1xuICAgICAgICAgICAgfVxuICAgICAgICB9KTtcbiAgICB9XG59XG5cbihhc3luYyAoKSA9PiB7XG4gICAgYXdhaXQgbmV3IENoYXRFbW90aW9uKCkuaW5pdGlhbGl6ZSgpO1xufSkoKSJdfQ==