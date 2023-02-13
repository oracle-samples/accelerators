/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
 *  http://oss.oracle.com/licenses/upl
 *  Copyright (c) 2023, Oracle and/or its affiliates.
 ***********************************************************************************************
 *  Accelerator Package: Incident Text Based Classification
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 23A (February 2023) 

 *  date: Fri Feb 10 15:54:08 IST 2023

 *  revision: rnw-23-02-initial
 *  SHA1: $Id: 87f7a7464228bc65fd8435c030bf98108c8c41a1 $
 * *********************************************************************************************
 *  File: getInsights.js
 * ****************************************************************************************** */


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
var GetInsights = (function () {
    function GetInsights() {
        this.OPT_ID_PRODUCT = 9;
        this.OPT_ID_DISPOSITION = 15;
        this.OPT_ID_CATEGORY = 12;
        this.STATUS_SOLVED = 2;
        this.CUSTOM_CFG_CPM_CONFIG = 'CUSTOM_CFG_CPM_CONFIG';
        this.prodMinConfidenceScore = 1;
        this.catMinConfidenceScore = 1;
        this.dispMinConfideceScore = 1;
        this.extensionProviderPromise = null;
        this.globalContextPromise = null;
        this.openWorkspacesIds = new Set();
        this.INSIGHT_TEXT_PREDICT = 3;
        this.getCircularReplacer = function () {
            var seen = new WeakSet();
            return function (key, value) {
                if (typeof value === "object" && value !== null) {
                    if (seen.has(value)) {
                        return;
                    }
                    seen.add(value);
                }
                return value;
            };
        };
        this.prefetchFields = ["Incident.ProdId", "Incident.CatId", "Incident.DispId", "Prediction$IncidentIntentDetail.InsightMLTriggered", "Prediction$IncidentIntentDetail.AutoMLTriggered", "Prediction$IncidentIntentDetail.IsAcceptedProdML", "Prediction$IncidentIntentDetail.IsAcceptedCatML", "Prediction$IncidentIntentDetail.IsAcceptedDispML", 'Incident.Threads'];
    }
    GetInsights.prototype.initialize = function () {
        return __awaiter(this, void 0, void 0, function () {
            var globalContext, configPromise, workSpaceRecord;
            var _this = this;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4, this.getGlobalContext()];
                    case 1:
                        globalContext = _a.sent();
                        globalContext.registerAction('getFeedback', function (param) { return _this.getFeedback(param); });
                        globalContext.registerAction('FormatInsightRequestForIncidentClassificationService', function (param) { return _this.FormatInsightRequestForIncidentClassificationService(param); });
                        configPromise = new Promise(function (resolve, reject) { return __awaiter(_this, void 0, void 0, function () {
                            var configurationListResponse, configurationList;
                            var _this = this;
                            var _a;
                            return __generator(this, function (_b) {
                                switch (_b.label) {
                                    case 0: return [4, this.makeRequest('GET', 'configurations', '')];
                                    case 1:
                                        configurationListResponse = _b.sent();
                                        if (configurationListResponse != null) {
                                            configurationList = (_a = JSON.parse(configurationListResponse)) === null || _a === void 0 ? void 0 : _a.items;
                                            configurationList.forEach(function (item) { return __awaiter(_this, void 0, void 0, function () {
                                                var _a;
                                                return __generator(this, function (_b) {
                                                    switch (_b.label) {
                                                        case 0:
                                                            if (!(item.lookupName == this.CUSTOM_CFG_CPM_CONFIG)) return [3, 2];
                                                            _a = resolve;
                                                            return [4, this.processConfigurations(item)];
                                                        case 1:
                                                            _a.apply(void 0, [_b.sent()]);
                                                            _b.label = 2;
                                                        case 2: return [2];
                                                    }
                                                });
                                            }); });
                                        }
                                        return [2];
                                }
                            });
                        }); });
                        globalContext.registerAction('FormatResponseFromIncidentClassificationService', function (param) { return _this.FormatResponseFromIncidentClassificationService(param, configPromise); });
                        return [4, this.getWorkspaceRecord()];
                    case 2:
                        workSpaceRecord = _a.sent();
                        workSpaceRecord.addCurrentEditorTabChangedListener(function (currentWorkspaceRecord) { return _this.preProcessOnTabChange(currentWorkspaceRecord); });
                        return [2];
                }
            });
        });
    };
    GetInsights.prototype.processConfigurations = function (item) {
        var _a;
        return __awaiter(this, void 0, void 0, function () {
            var response, config;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0: return [4, this.makeRequest('GET', 'configurations/' + item.id, '')];
                    case 1:
                        response = _b.sent();
                        config = JSON.parse((_a = JSON.parse(response)) === null || _a === void 0 ? void 0 : _a.value);
                        this.prodMinConfidenceScore = config === null || config === void 0 ? void 0 : config.PRODUCT_MIN_CONFIDENCE_SCORE;
                        this.dispMinConfideceScore = config === null || config === void 0 ? void 0 : config.DISPOSITION_ITEMS_MIN_CONFIDENCE_SCORE;
                        this.catMinConfidenceScore = config === null || config === void 0 ? void 0 : config.CATEGORY_ITEMS_MIN_CONFIDENCE_SCORE;
                        return [2];
                }
            });
        });
    };
    GetInsights.prototype.removeClosedIncidentIdFromLookUp = function (incidentId) {
        return __awaiter(this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                if (this.openWorkspacesIds.has(incidentId)) {
                    this.openWorkspacesIds.delete(incidentId);
                }
                return [2];
            });
        });
    };
    GetInsights.prototype.preProcessOnTabChange = function (workspaceRecordEventParameter) {
        return __awaiter(this, void 0, void 0, function () {
            var currentWorkspaceRecord, globalContext, fields_1;
            var _this = this;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        currentWorkspaceRecord = workspaceRecordEventParameter.getWorkspaceRecord();
                        return [4, this.getGlobalContext()];
                    case 1:
                        globalContext = _a.sent();
                        if (!(workspaceRecordEventParameter.newWorkspace.objectType == 'Incident' && !this.openWorkspacesIds.has(workspaceRecordEventParameter.newWorkspace.objectId))) return [3, 3];
                        currentWorkspaceRecord.addRecordClosingListener(function (currentWorkspaceRecord) { return _this.removeClosedIncidentIdFromLookUp(workspaceRecordEventParameter.newWorkspace.objectId); });
                        this.openWorkspacesIds.add(workspaceRecordEventParameter.newWorkspace.objectId);
                        return [4, currentWorkspaceRecord.getFieldValues(this.prefetchFields)];
                    case 2:
                        fields_1 = _a.sent();
                        currentWorkspaceRecord.addFieldValueListener('Incident.ProdId', function (param) {
                            if ((fields_1['Prediction$IncidentIntentDetail.InsightMLTriggered'] == 1 || fields_1['Prediction$IncidentIntentDetail.AutoMLTriggered'] == 1) && fields_1['Prediction$IncidentIntentDetail.IsAcceptedProdML'] == 1) {
                                currentWorkspaceRecord.updateField('Prediction$IncidentIntentDetail.IsAcceptedProdML', '0');
                            }
                        });
                        _a.label = 3;
                    case 3: return [2];
                }
            });
        });
    };
    GetInsights.prototype.getFeedback = function (param) {
        return __awaiter(this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                switch (param.insightID) {
                    case 'Incident.DispId':
                        if (param.isAccepted == true) {
                            this.updateWorkspaceField('setDispMLToTrue');
                        }
                        else {
                            this.updateWorkspaceField('setDispMLToFalse');
                        }
                        break;
                    case 'Incident.ProdId':
                        if (param.isAccepted == false) {
                            this.updateWorkspaceField('setProdMLToFalse');
                        }
                        else {
                            this.updateWorkspaceField('setProdMLToTrue');
                        }
                        break;
                    case 'Incident.CatId':
                        if (param.isAccepted == false) {
                            this.updateWorkspaceField('setCatMLToFalse');
                        }
                        else {
                            this.updateWorkspaceField('setCatMLToTrue');
                        }
                        break;
                }
                return [2, ""];
            });
        });
    };
    GetInsights.prototype.updateWorkspaceField = function (action) {
        return __awaiter(this, void 0, void 0, function () {
            var currentWorkspaceRecord, fields, _a;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0: return [4, this.getWorkspaceRecord()];
                    case 1:
                        currentWorkspaceRecord = _b.sent();
                        return [4, currentWorkspaceRecord.getFieldValues(this.prefetchFields)];
                    case 2:
                        fields = _b.sent();
                        _a = action;
                        switch (_a) {
                            case 'setDispMLToTrue': return [3, 3];
                            case 'setDispMLToFalse': return [3, 6];
                            case 'setProdMLToFalse': return [3, 8];
                            case 'setProdMLToTrue': return [3, 10];
                            case 'setCatMLToTrue': return [3, 13];
                            case 'setCatMLToFalse': return [3, 16];
                        }
                        return [3, 18];
                    case 3:
                        if (fields['Prediction$IncidentIntentDetail.IsAcceptedDispML'] != null) {
                            return [2, false];
                        }
                        return [4, currentWorkspaceRecord.updateField("Prediction$IncidentIntentDetail.IsAcceptedDispML", '1')];
                    case 4:
                        _b.sent();
                        return [4, currentWorkspaceRecord.updateField("Prediction$IncidentIntentDetail.PredictedDisposition", fields.getField('Incident.DispId').getValue())];
                    case 5:
                        _b.sent();
                        return [3, 18];
                    case 6: return [4, currentWorkspaceRecord.updateField("Prediction$IncidentIntentDetail.IsAcceptedDispML", '0')];
                    case 7:
                        _b.sent();
                        return [3, 18];
                    case 8: return [4, currentWorkspaceRecord.updateField("Prediction$IncidentIntentDetail.IsAcceptedProdML", '0')];
                    case 9:
                        _b.sent();
                        return [3, 18];
                    case 10: return [4, currentWorkspaceRecord.updateField("Prediction$IncidentIntentDetail.IsAcceptedProdML", '1')];
                    case 11:
                        _b.sent();
                        return [4, currentWorkspaceRecord.updateField("Prediction$IncidentIntentDetail.PredictedProduct", fields.getField('Incident.ProdId').getValue())];
                    case 12:
                        _b.sent();
                        return [3, 18];
                    case 13: return [4, currentWorkspaceRecord.updateField("Prediction$IncidentIntentDetail.IsAcceptedCatML", '1')];
                    case 14:
                        _b.sent();
                        return [4, currentWorkspaceRecord.updateField("Prediction$IncidentIntentDetail.PredictedCategory", fields.getField('Incident.CatId').getValue())];
                    case 15:
                        _b.sent();
                        return [3, 18];
                    case 16: return [4, currentWorkspaceRecord.updateField("Prediction$IncidentIntentDetail.IsAcceptedCatML", '0')];
                    case 17:
                        _b.sent();
                        return [3, 18];
                    case 18: return [2];
                }
            });
        });
    };
    GetInsights.prototype.updateInsightsTriggered = function () {
        return __awaiter(this, void 0, void 0, function () {
            var fields, currentWorkspaceRecord;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4, this.getFieldValues(this.prefetchFields)];
                    case 1:
                        fields = _a.sent();
                        return [4, this.getWorkspaceRecord()];
                    case 2:
                        currentWorkspaceRecord = _a.sent();
                        if (fields['Prediction$IncidentIntentDetail.InsightMLTriggered'] == true) {
                            return [2, false];
                        }
                        currentWorkspaceRecord.updateField('Prediction$IncidentIntentDetail.InsightMLTriggered', '1');
                        return [2];
                }
            });
        });
    };
    GetInsights.prototype.getExtensionProvider = function () {
        return __awaiter(this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        if (this.extensionProviderPromise == null) {
                            this.extensionProviderPromise = ORACLE_SERVICE_CLOUD.extension_loader.load("ML_Insight");
                        }
                        return [4, this.extensionProviderPromise];
                    case 1: return [2, _a.sent()];
                }
            });
        });
    };
    GetInsights.prototype.getGlobalContext = function () {
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
    GetInsights.prototype.FormatInsightRequestForIncidentClassificationService = function (param) {
        var _this = this;
        return new ExtensionPromise(function (resolve, reject) { return __awaiter(_this, void 0, void 0, function () {
            var fieldVal, cpmUpdated, subject, description;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4, new Promise(function (done) {
                            window.setTimeout(done, 100);
                        })];
                    case 1:
                        _a.sent();
                        return [4, this.getFieldValues(['Prediction$IncidentIntentDetail.AutoMLTriggered'])];
                    case 2:
                        fieldVal = _a.sent();
                        cpmUpdated = fieldVal.getField('Prediction$IncidentIntentDetail.AutoMLTriggered').getValue();
                        if (cpmUpdated == null) {
                            reject(null);
                            param.jsonData = { 'inquiry': ' ', 'product': 0, 'category': 0, 'disposition': 0 };
                        }
                        else {
                            subject = param['subject'];
                            description = param['description'];
                            param.jsonData = { 'inquiry': subject + ' ' + description, 'product': 0, 'category': 0, 'disposition': 0 };
                            resolve(param);
                        }
                        return [2];
                }
            });
        }); });
    };
    GetInsights.prototype.FormatResponseFromIncidentClassificationService = function (param, configurationPromise) {
        return __awaiter(this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4, configurationPromise];
                    case 1:
                        _a.sent();
                        return [4, this.responseProcess(param)];
                    case 2:
                        _a.sent();
                        return [2];
                }
            });
        });
    };
    GetInsights.prototype.responseProcess = function (param) {
        var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m, _o, _p, _q, _r, _s, _t, _u, _v, _w, _x, _y, _z, _0, _1;
        return __awaiter(this, void 0, void 0, function () {
            var weneedtolog, insights, prediction, payload, fieldVal, currentProdVal, predictedProdLbl, predictedDispLbl, currentDispValue, currentCatVal, predictedCatLbl, isAcceptedProdMLVal, isAcceptedDispML, isAcceptedCatML, cpmUpdated, statusType, wId, predictedCategoryId, insightsContext;
            return __generator(this, function (_2) {
                switch (_2.label) {
                    case 0:
                        weneedtolog = false;
                        insights = [];
                        if (!(param.insights.prediction != null)) return [3, 8];
                        prediction = param.insights.prediction;
                        payload = {
                            IncidentId: {
                                id: 0
                            }
                        };
                        return [4, this.getFieldValues(['Prediction$IncidentIntentDetail.AutoMLTriggered', 'Incident.Status.Type', 'Incident.ID', 'Incident.ProdId', 'Incident.DispId', 'Incident.CatId', 'Prediction$IncidentIntentDetail.IsAcceptedProdML', 'Prediction$IncidentIntentDetail.IsAcceptedDispML', 'Prediction$IncidentIntentDetail.IsAcceptedCatML'])];
                    case 1:
                        fieldVal = _2.sent();
                        currentProdVal = fieldVal.getField('Incident.ProdId').getValue();
                        return [4, this.getOptList(prediction.product.prediction, 'Incident.ProdId')];
                    case 2:
                        predictedProdLbl = _2.sent();
                        return [4, this.getOptList(prediction.disposition.prediction, 'Incident.DispId')];
                    case 3:
                        predictedDispLbl = _2.sent();
                        currentDispValue = fieldVal.getField('Incident.DispId').getValue();
                        currentCatVal = fieldVal.getField('Incident.CatId').getValue();
                        return [4, this.getOptList(prediction.category.prediction, 'Incident.CatId')];
                    case 4:
                        predictedCatLbl = _2.sent();
                        isAcceptedProdMLVal = fieldVal.getField('Prediction$IncidentIntentDetail.IsAcceptedProdML').getValue();
                        isAcceptedDispML = fieldVal.getField('Prediction$IncidentIntentDetail.IsAcceptedDispML').getValue();
                        isAcceptedCatML = fieldVal.getField('Prediction$IncidentIntentDetail.IsAcceptedCatML').getValue();
                        cpmUpdated = fieldVal.getField('Prediction$IncidentIntentDetail.AutoMLTriggered').getValue();
                        statusType = fieldVal.getField('Incident.Status.Type').getValue();
                        wId = fieldVal.getParent().getParent().getEntityId();
                        if (cpmUpdated == null) {
                            return [2];
                        }
                        payload.IncidentId.id = parseInt(wId);
                        if (parseInt(param.insights.prediction.product.prediction) > 0) {
                            payload.PredictedProduct = { "id": parseInt((_c = (_b = (_a = param === null || param === void 0 ? void 0 : param.insights) === null || _a === void 0 ? void 0 : _a.prediction) === null || _b === void 0 ? void 0 : _b.product) === null || _c === void 0 ? void 0 : _c.prediction) };
                            payload.PredictedProductId = parseInt((_f = (_e = (_d = param === null || param === void 0 ? void 0 : param.insights) === null || _d === void 0 ? void 0 : _d.prediction) === null || _e === void 0 ? void 0 : _e.product) === null || _f === void 0 ? void 0 : _f.prediction);
                            payload.ConfidenceScoreProduct = String((prediction.product.confidenceScore).toFixed(2));
                        }
                        if (parseInt(currentProdVal) > 0) {
                            payload.CreatedProduct = { "id": parseInt(currentProdVal) };
                        }
                        if (statusType != this.STATUS_SOLVED && prediction.product != null && parseInt(currentProdVal) != parseInt((_g = prediction === null || prediction === void 0 ? void 0 : prediction.product) === null || _g === void 0 ? void 0 : _g.prediction) && (isAcceptedProdMLVal == null && ((_h = prediction === null || prediction === void 0 ? void 0 : prediction.product) === null || _h === void 0 ? void 0 : _h.confidenceScore) > this.prodMinConfidenceScore)) {
                            insights.push({
                                'confidence': ((_j = prediction === null || prediction === void 0 ? void 0 : prediction.product) === null || _j === void 0 ? void 0 : _j.confidenceScore) * 100,
                                'insightType': 'setField',
                                'insightValue': (_m = (_l = (_k = param === null || param === void 0 ? void 0 : param.insights) === null || _k === void 0 ? void 0 : _k.prediction) === null || _l === void 0 ? void 0 : _l.product) === null || _m === void 0 ? void 0 : _m.prediction,
                                'insightField': 'Incident.ProdId',
                                'insightId': 'Incident.ProdId',
                                'description': 'The system has identified the product as ' + predictedProdLbl + '. Do you want to update the product to the suggested value?'
                            });
                            weneedtolog = true;
                        }
                        if (statusType != this.STATUS_SOLVED && prediction.disposition != null && parseInt(currentDispValue) != parseInt((_o = prediction === null || prediction === void 0 ? void 0 : prediction.disposition) === null || _o === void 0 ? void 0 : _o.prediction) && (isAcceptedDispML == null && ((_p = prediction === null || prediction === void 0 ? void 0 : prediction.disposition) === null || _p === void 0 ? void 0 : _p.confidenceScore) > this.dispMinConfideceScore)) {
                            insights.push({
                                'confidence': ((_q = prediction === null || prediction === void 0 ? void 0 : prediction.disposition) === null || _q === void 0 ? void 0 : _q.confidenceScore) * 100,
                                'insightType': 'setField',
                                'insightValue': (_r = prediction === null || prediction === void 0 ? void 0 : prediction.disposition) === null || _r === void 0 ? void 0 : _r.prediction,
                                'insightField': 'Incident.DispId',
                                'insightId': 'Incident.DispId',
                                'description': 'The system has identified the disposition as ' + predictedDispLbl + '. Do you want to update the disposition to the suggested value?'
                            });
                            weneedtolog = true;
                        }
                        predictedCategoryId = parseInt((_s = prediction === null || prediction === void 0 ? void 0 : prediction.category) === null || _s === void 0 ? void 0 : _s.prediction);
                        payload.PredictedCategoryId = predictedCategoryId;
                        payload.PredictedCategory = predictedCategoryId != null && predictedCategoryId > 0 ? { "id": predictedCategoryId } : null;
                        if (parseInt(currentCatVal) > 0) {
                            payload.CreatedCategoryId = parseInt(currentCatVal);
                        }
                        payload.CreatedCategory = currentCatVal != null && parseInt(currentCatVal) > 0 ? { "id": parseInt(currentCatVal) } : null;
                        payload.PredictedDispositionId = parseInt(prediction.disposition.prediction);
                        payload.PredictedDisposition = parseInt(prediction.disposition.prediction) != null && parseInt(prediction.disposition.prediction) > 0 ? { "id": parseInt(prediction.disposition.prediction) } : null;
                        if (parseInt(currentDispValue) > 0) {
                            payload.CreatedDispositionId = parseInt(currentDispValue);
                        }
                        payload.CreatedDisposition = currentDispValue != null && parseInt(currentDispValue) > 0 ? { "id": parseInt(currentDispValue) } : null;
                        payload.ConfidenceScoreDisposition = String(((_t = prediction === null || prediction === void 0 ? void 0 : prediction.disposition) === null || _t === void 0 ? void 0 : _t.confidenceScore).toFixed(2));
                        payload.ConfidenceScoreCategory = String(((_u = prediction === null || prediction === void 0 ? void 0 : prediction.category) === null || _u === void 0 ? void 0 : _u.confidenceScore).toFixed(2));
                        if (statusType != this.STATUS_SOLVED && prediction.category != null && parseInt(currentCatVal) != parseInt((_v = prediction === null || prediction === void 0 ? void 0 : prediction.category) === null || _v === void 0 ? void 0 : _v.prediction) && (isAcceptedCatML == null && ((_w = prediction === null || prediction === void 0 ? void 0 : prediction.category) === null || _w === void 0 ? void 0 : _w.confidenceScore) > this.catMinConfidenceScore)) {
                            if (currentCatVal != ((_z = (_y = (_x = param === null || param === void 0 ? void 0 : param.insights) === null || _x === void 0 ? void 0 : _x.prediction) === null || _y === void 0 ? void 0 : _y.category) === null || _z === void 0 ? void 0 : _z.prediction)) {
                                insights.push({
                                    'confidence': ((_0 = prediction === null || prediction === void 0 ? void 0 : prediction.category) === null || _0 === void 0 ? void 0 : _0.confidenceScore) * 100,
                                    'insightType': 'setField',
                                    'insightValue': (_1 = prediction === null || prediction === void 0 ? void 0 : prediction.category) === null || _1 === void 0 ? void 0 : _1.prediction,
                                    'insightField': 'Incident.CatId',
                                    'insightId': 'Incident.CatId',
                                    'description': 'The system has identified the category as ' + predictedCatLbl + '. Do you want to update the category to the suggested value?'
                                });
                            }
                            weneedtolog = true;
                        }
                        return [4, this.getExtensionProvider()];
                    case 5: return [4, (_2.sent()).getInsightsContext()];
                    case 6:
                        insightsContext = _2.sent();
                        insightsContext.handleInsightResponseReady([{
                                contextId: param['contextId'],
                                insightConnectionId: param['insightConnectionId'],
                                response: insights
                            }]);
                        if (!(weneedtolog == true)) return [3, 8];
                        return [4, this.updateInsightsTriggered()];
                    case 7:
                        _2.sent();
                        this.logPredictionData(payload);
                        _2.label = 8;
                    case 8: return [2];
                }
            });
        });
    };
    GetInsights.prototype.getOptList = function (id, optlist) {
        return __awaiter(this, void 0, void 0, function () {
            var optId, extensionPromise, extentionProvider, globalContext;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        if (id == 0) {
                            return [2, Promise.resolve()];
                        }
                        if (optlist == 'Incident.ProdId') {
                            optId = this.OPT_ID_PRODUCT;
                        }
                        else if (optlist == 'Incident.DispId') {
                            optId = this.OPT_ID_DISPOSITION;
                        }
                        else {
                            optId = this.OPT_ID_CATEGORY;
                        }
                        extensionPromise = new ExtensionPromise();
                        return [4, ORACLE_SERVICE_CLOUD.extension_loader.load("ML_Insight")];
                    case 1:
                        extentionProvider = _a.sent();
                        return [4, this.getGlobalContext()];
                    case 2:
                        globalContext = _a.sent();
                        return [4, globalContext.getOptListContext().then(function (optListContext) {
                                var optListLabelSearchFilter = optListContext.createOptListSearchFilter();
                                optListLabelSearchFilter.setSearchBy('Id');
                                optListLabelSearchFilter.setSearchValue(id);
                                optListLabelSearchFilter.setCondition('isEqual');
                                var getOptListRequest = optListContext.createOptListRequest();
                                getOptListRequest.setOptListId(optId);
                                getOptListRequest.setOptListSearchFilter(optListLabelSearchFilter);
                                var optListItemPromise = optListContext.getOptList(getOptListRequest);
                                optListItemPromise.then(function (optListItemResult) {
                                    if (optListItemResult.getOptListChildren().length > 0) {
                                        var child = optListItemResult.getOptListChildren();
                                        while (child[0]) {
                                            var childId = child[0].getId();
                                            if (id == childId) {
                                                extensionPromise.resolve(child[0].getLabel());
                                            }
                                            child = child[0].getOptListChildren();
                                        }
                                    }
                                });
                            })];
                    case 3:
                        _a.sent();
                        return [2, extensionPromise];
                }
            });
        });
    };
    GetInsights.prototype.refreshToken = function () {
        return __awaiter(this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4, this.getGlobalContext()];
                    case 1: return [2, (_a.sent()).getSessionToken()];
                }
            });
        });
    };
    GetInsights.prototype.makeRequest = function (method, uri, payload) {
        return __awaiter(this, void 0, void 0, function () {
            var extensionPromise, xhr, globalContext, restUrl, _a, _b, _c, _d;
            return __generator(this, function (_e) {
                switch (_e.label) {
                    case 0:
                        extensionPromise = new ExtensionPromise();
                        xhr = new XMLHttpRequest();
                        return [4, this.getGlobalContext()];
                    case 1:
                        globalContext = _e.sent();
                        restUrl = globalContext.getInterfaceServiceUrl('REST') + "/connect/latest/";
                        xhr.open(method, restUrl + uri, true);
                        xhr.setRequestHeader('osvc-crest-application-context', 'Insights');
                        xhr.setRequestHeader('Content-Type', 'application/json');
                        xhr.setRequestHeader('Accept', 'application/json');
                        _b = (_a = xhr).setRequestHeader;
                        _c = ['Authorization'];
                        _d = 'Session ';
                        return [4, this.refreshToken()];
                    case 2:
                        _b.apply(_a, _c.concat([_d + (_e.sent())]));
                        xhr.onreadystatechange = function () {
                            if (this.readyState == 4) {
                                if (this.status == 200 || this.status == 201) {
                                    extensionPromise.resolve(xhr.response);
                                }
                                else {
                                    if (this.status == 401) {
                                        var getInsight = new GetInsights();
                                        getInsight.refreshToken();
                                        getInsight.makeRequest(method, uri, payload);
                                    }
                                    extensionPromise.reject(new ORACLE_SERVICE_CLOUD.ErrorData(xhr.responseText));
                                }
                            }
                        };
                        if (method == 'PATCH' || method == 'POST') {
                            xhr.send(JSON.stringify(payload, this.getCircularReplacer()));
                        }
                        else {
                            xhr.send();
                        }
                        return [2, extensionPromise];
                }
            });
        });
    };
    GetInsights.prototype.logPredictionData = function (payload) {
        return __awaiter(this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                payload.MinConfidenceScoreProduct = String(this.prodMinConfidenceScore.toFixed(2));
                payload.MinConfidenceScoreDisposition = String(this.dispMinConfideceScore.toFixed(2));
                payload.MinConfidenceScoreCategory = String(this.catMinConfidenceScore.toFixed(2));
                payload.Source = { "id": this.INSIGHT_TEXT_PREDICT };
                this.makeRequest('POST', 'Prediction.PredictionLog', payload);
                return [2];
            });
        });
    };
    GetInsights.prototype.getFieldValues = function (fieldNameArray) {
        return __awaiter(this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4, this.getWorkspaceRecord()];
                    case 1: return [4, (_a.sent()).getFieldValues(fieldNameArray)];
                    case 2: return [2, _a.sent()];
                }
            });
        });
    };
    GetInsights.prototype.getWorkspaceRecord = function () {
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
                        });
                        return [4, workspaceRecordPromise];
                    case 2: return [2, _a.sent()];
                }
            });
        });
    };
    return GetInsights;
}());
new GetInsights().initialize();
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiZ2V0SW5zaWdodHMuanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyJnZXRJbnNpZ2h0cy50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiOzs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7QUFxRkE7SUFBQTtRQUNZLG1CQUFjLEdBQVcsQ0FBQyxDQUFDO1FBQzNCLHVCQUFrQixHQUFXLEVBQUUsQ0FBQztRQUNoQyxvQkFBZSxHQUFXLEVBQUUsQ0FBQztRQUM3QixrQkFBYSxHQUFXLENBQUMsQ0FBQztRQUMxQiwwQkFBcUIsR0FBRyx1QkFBdUIsQ0FBQztRQUNoRCwyQkFBc0IsR0FBRyxDQUFDLENBQUM7UUFDM0IsMEJBQXFCLEdBQUcsQ0FBQyxDQUFDO1FBQzFCLDBCQUFxQixHQUFHLENBQUMsQ0FBQztRQUMxQiw2QkFBd0IsR0FBMEMsSUFBSSxDQUFDO1FBQ3ZFLHlCQUFvQixHQUErQyxJQUFJLENBQUM7UUFDeEUsc0JBQWlCLEdBQWdCLElBQUksR0FBRyxFQUFFLENBQUM7UUFDM0MseUJBQW9CLEdBQUcsQ0FBQyxDQUFDO1FBQ3pCLHdCQUFtQixHQUFHO1lBQzFCLElBQU0sSUFBSSxHQUFHLElBQUksT0FBTyxFQUFFLENBQUM7WUFDM0IsT0FBTyxVQUFDLEdBQUcsRUFBRSxLQUFLO2dCQUNkLElBQUksT0FBTyxLQUFLLEtBQUssUUFBUSxJQUFJLEtBQUssS0FBSyxJQUFJLEVBQUU7b0JBQzdDLElBQUksSUFBSSxDQUFDLEdBQUcsQ0FBQyxLQUFLLENBQUMsRUFBRTt3QkFDakIsT0FBTTtxQkFDVDtvQkFDRCxJQUFJLENBQUMsR0FBRyxDQUFDLEtBQUssQ0FBQyxDQUFBO2lCQUNsQjtnQkFDRCxPQUFPLEtBQUssQ0FBQTtZQUNoQixDQUFDLENBQUE7UUFDTCxDQUFDLENBQUE7UUFDTyxtQkFBYyxHQUFHLENBQUMsaUJBQWlCLEVBQUUsZ0JBQWdCLEVBQUUsaUJBQWlCLEVBQUUsb0RBQW9ELEVBQUUsaURBQWlELEVBQUUsa0RBQWtELEVBQUUsaURBQWlELEVBQUUsa0RBQWtELEVBQUUsa0JBQWtCLENBQUMsQ0FBQztJQW9ZOVcsQ0FBQztJQWpZZ0IsZ0NBQVUsR0FBdkI7Ozs7Ozs0QkFDaUQsV0FBTSxJQUFJLENBQUMsZ0JBQWdCLEVBQUUsRUFBQTs7d0JBQXRFLGFBQWEsR0FBNEIsU0FBNkI7d0JBQzFFLGFBQWEsQ0FBQyxjQUFjLENBQUMsYUFBYSxFQUFFLFVBQUMsS0FBSyxJQUFLLE9BQUEsS0FBSSxDQUFDLFdBQVcsQ0FBQyxLQUFLLENBQUMsRUFBdkIsQ0FBdUIsQ0FBQyxDQUFDO3dCQUVoRixhQUFhLENBQUMsY0FBYyxDQUFDLHNEQUFzRCxFQUFFLFVBQUMsS0FBSyxJQUFLLE9BQUEsS0FBSSxDQUFDLG9EQUFvRCxDQUFDLEtBQUssQ0FBQyxFQUFoRSxDQUFnRSxDQUFDLENBQUM7d0JBRTlKLGFBQWEsR0FBRyxJQUFJLE9BQU8sQ0FBQyxVQUFPLE9BQU8sRUFBRSxNQUFNOzs7Ozs7NENBQ2xCLFdBQU0sSUFBSSxDQUFDLFdBQVcsQ0FBQyxLQUFLLEVBQUUsZ0JBQWdCLEVBQUUsRUFBRSxDQUFDLEVBQUE7O3dDQUEvRSx5QkFBeUIsR0FBRyxTQUFtRDt3Q0FDbkYsSUFBSSx5QkFBeUIsSUFBSSxJQUFJLEVBQUU7NENBQy9CLGlCQUFpQixHQUFHLE1BQUEsSUFBSSxDQUFDLEtBQUssQ0FBQyx5QkFBeUIsQ0FBQywwQ0FBRSxLQUFLLENBQUM7NENBQ3JFLGlCQUFpQixDQUFDLE9BQU8sQ0FBQyxVQUFPLElBQUk7Ozs7O2lFQUM3QixDQUFBLElBQUksQ0FBQyxVQUFVLElBQUksSUFBSSxDQUFDLHFCQUFxQixDQUFBLEVBQTdDLGNBQTZDOzREQUM3QyxLQUFBLE9BQU8sQ0FBQTs0REFBQyxXQUFNLElBQUksQ0FBQyxxQkFBcUIsQ0FBQyxJQUFJLENBQUMsRUFBQTs7NERBQTlDLGtCQUFRLFNBQXNDLEVBQUMsQ0FBQTs7Ozs7aURBRXRELENBQUMsQ0FBQzt5Q0FFTjs7Ozs2QkFDSixDQUFDLENBQUM7d0JBRUgsYUFBYSxDQUFDLGNBQWMsQ0FBQyxpREFBaUQsRUFBRSxVQUFDLEtBQUssSUFBSyxPQUFBLEtBQUksQ0FBQywrQ0FBK0MsQ0FBQyxLQUFLLEVBQUUsYUFBYSxDQUFDLEVBQTFFLENBQTBFLENBQUMsQ0FBQzt3QkFFbEcsV0FBTSxJQUFJLENBQUMsa0JBQWtCLEVBQUUsRUFBQTs7d0JBQWhHLGVBQWUsR0FBa0QsU0FBK0I7d0JBRXBHLGVBQWUsQ0FBQyxrQ0FBa0MsQ0FBQyxVQUFDLHNCQUEyRSxJQUFLLE9BQUEsS0FBSSxDQUFDLHFCQUFxQixDQUFDLHNCQUFzQixDQUFDLEVBQWxELENBQWtELENBQUMsQ0FBQzs7Ozs7S0FFM0w7SUFFSywyQ0FBcUIsR0FBM0IsVUFBNEIsSUFBSTs7Ozs7OzRCQUNiLFdBQU0sSUFBSSxDQUFDLFdBQVcsQ0FBQyxLQUFLLEVBQUUsaUJBQWlCLEdBQUcsSUFBSSxDQUFDLEVBQUUsRUFBRSxFQUFFLENBQUMsRUFBQTs7d0JBQXpFLFFBQVEsR0FBRyxTQUE4RDt3QkFDekUsTUFBTSxHQUFHLElBQUksQ0FBQyxLQUFLLENBQUMsTUFBQSxJQUFJLENBQUMsS0FBSyxDQUFDLFFBQVEsQ0FBQywwQ0FBRSxLQUFLLENBQUMsQ0FBQzt3QkFDckQsSUFBSSxDQUFDLHNCQUFzQixHQUFHLE1BQU0sYUFBTixNQUFNLHVCQUFOLE1BQU0sQ0FBRSw0QkFBNEIsQ0FBQzt3QkFDbkUsSUFBSSxDQUFDLHFCQUFxQixHQUFHLE1BQU0sYUFBTixNQUFNLHVCQUFOLE1BQU0sQ0FBRSxzQ0FBc0MsQ0FBQzt3QkFDNUUsSUFBSSxDQUFDLHFCQUFxQixHQUFHLE1BQU0sYUFBTixNQUFNLHVCQUFOLE1BQU0sQ0FBRSxtQ0FBbUMsQ0FBQzs7Ozs7S0FDNUU7SUFFSyxzREFBZ0MsR0FBdEMsVUFBdUMsVUFBa0I7OztnQkFDckQsSUFBSSxJQUFJLENBQUMsaUJBQWlCLENBQUMsR0FBRyxDQUFDLFVBQVUsQ0FBQyxFQUFFO29CQUN4QyxJQUFJLENBQUMsaUJBQWlCLENBQUMsTUFBTSxDQUFDLFVBQVUsQ0FBQyxDQUFDO2lCQUM3Qzs7OztLQUNKO0lBRUssMkNBQXFCLEdBQTNCLFVBQTRCLDZCQUFrRjs7Ozs7Ozt3QkFDdEcsc0JBQXNCLEdBQXFCLDZCQUE2QixDQUFDLGtCQUFrQixFQUFFLENBQUM7d0JBQ2hDLFdBQU0sSUFBSSxDQUFDLGdCQUFnQixFQUFFLEVBQUE7O3dCQUEzRixhQUFhLEdBQWlELFNBQTZCOzZCQUMzRixDQUFBLDZCQUE2QixDQUFDLFlBQVksQ0FBQyxVQUFVLElBQUksVUFBVSxJQUFJLENBQUMsSUFBSSxDQUFDLGlCQUFpQixDQUFDLEdBQUcsQ0FBQyw2QkFBNkIsQ0FBQyxZQUFZLENBQUMsUUFBUSxDQUFDLENBQUEsRUFBdkosY0FBdUo7d0JBQ3ZKLHNCQUFzQixDQUFDLHdCQUF3QixDQUFDLFVBQUMsc0JBQTJFLElBQUssT0FBQSxLQUFJLENBQUMsZ0NBQWdDLENBQUMsNkJBQTZCLENBQUMsWUFBWSxDQUFDLFFBQVEsQ0FBQyxFQUExRixDQUEwRixDQUFDLENBQUM7d0JBQzdOLElBQUksQ0FBQyxpQkFBaUIsQ0FBQyxHQUFHLENBQUMsNkJBQTZCLENBQUMsWUFBWSxDQUFDLFFBQVEsQ0FBQyxDQUFDO3dCQUUvQixXQUFNLHNCQUFzQixDQUFDLGNBQWMsQ0FBQyxJQUFJLENBQUMsY0FBYyxDQUFDLEVBQUE7O3dCQUE3RyxXQUE2QyxTQUFnRTt3QkFFakgsc0JBQXNCLENBQUMscUJBQXFCLENBQUMsaUJBQWlCLEVBQUUsVUFBVSxLQUFLOzRCQUMzRSxJQUFJLENBQUMsUUFBTSxDQUFDLG9EQUFvRCxDQUFDLElBQUksQ0FBQyxJQUFJLFFBQU0sQ0FBQyxpREFBaUQsQ0FBQyxJQUFJLENBQUMsQ0FBQyxJQUFJLFFBQU0sQ0FBQyxrREFBa0QsQ0FBQyxJQUFJLENBQUMsRUFBRTtnQ0FDMU0sc0JBQXNCLENBQUMsV0FBVyxDQUFDLGtEQUFrRCxFQUFFLEdBQUcsQ0FBQyxDQUFDOzZCQUMvRjt3QkFDTCxDQUFDLENBQUMsQ0FBQzs7Ozs7O0tBR1Y7SUFHWSxpQ0FBVyxHQUF4QixVQUF5QixLQUFVOzs7Z0JBQy9CLFFBQVEsS0FBSyxDQUFDLFNBQVMsRUFBRTtvQkFDckIsS0FBSyxpQkFBaUI7d0JBQ2xCLElBQUksS0FBSyxDQUFDLFVBQVUsSUFBSSxJQUFJLEVBQUU7NEJBQzFCLElBQUksQ0FBQyxvQkFBb0IsQ0FBQyxpQkFBaUIsQ0FBQyxDQUFDO3lCQUNoRDs2QkFBTTs0QkFDSCxJQUFJLENBQUMsb0JBQW9CLENBQUMsa0JBQWtCLENBQUMsQ0FBQzt5QkFDakQ7d0JBQ0QsTUFBTTtvQkFDVixLQUFLLGlCQUFpQjt3QkFDbEIsSUFBSSxLQUFLLENBQUMsVUFBVSxJQUFJLEtBQUssRUFBRTs0QkFDM0IsSUFBSSxDQUFDLG9CQUFvQixDQUFDLGtCQUFrQixDQUFDLENBQUE7eUJBQ2hEOzZCQUFNOzRCQUNILElBQUksQ0FBQyxvQkFBb0IsQ0FBQyxpQkFBaUIsQ0FBQyxDQUFBO3lCQUMvQzt3QkFDRCxNQUFNO29CQUNWLEtBQUssZ0JBQWdCO3dCQUNqQixJQUFJLEtBQUssQ0FBQyxVQUFVLElBQUksS0FBSyxFQUFFOzRCQUMzQixJQUFJLENBQUMsb0JBQW9CLENBQUMsaUJBQWlCLENBQUMsQ0FBQTt5QkFDL0M7NkJBQU07NEJBQ0gsSUFBSSxDQUFDLG9CQUFvQixDQUFDLGdCQUFnQixDQUFDLENBQUE7eUJBQzlDO3dCQUNELE1BQU07aUJBQ2I7Z0JBQ0QsV0FBTyxFQUFFLEVBQUM7OztLQUNiO0lBRWEsMENBQW9CLEdBQWxDLFVBQW1DLE1BQWM7Ozs7OzRCQUNFLFdBQU0sSUFBSSxDQUFDLGtCQUFrQixFQUFFLEVBQUE7O3dCQUExRSxzQkFBc0IsR0FBcUIsU0FBK0I7d0JBQzdCLFdBQU0sc0JBQXNCLENBQUMsY0FBYyxDQUFDLElBQUksQ0FBQyxjQUFjLENBQUMsRUFBQTs7d0JBQTdHLE1BQU0sR0FBdUMsU0FBZ0U7d0JBQ3pHLEtBQUEsTUFBTSxDQUFBOztpQ0FDTCxpQkFBaUIsQ0FBQyxDQUFsQixjQUFpQjtpQ0FLakIsa0JBQWtCLENBQUMsQ0FBbkIsY0FBa0I7aUNBR2xCLGtCQUFrQixDQUFDLENBQW5CLGNBQWtCO2lDQUdsQixpQkFBaUIsQ0FBQyxDQUFsQixlQUFpQjtpQ0FJakIsZ0JBQWdCLENBQUMsQ0FBakIsZUFBZ0I7aUNBSWhCLGlCQUFpQixDQUFDLENBQWxCLGVBQWlCOzs7O3dCQWxCbEIsSUFBSSxNQUFNLENBQUMsa0RBQWtELENBQUMsSUFBSSxJQUFJLEVBQUU7NEJBQUUsV0FBTyxLQUFLLEVBQUM7eUJBQUU7d0JBQ3pGLFdBQU0sc0JBQXNCLENBQUMsV0FBVyxDQUFDLGtEQUFrRCxFQUFFLEdBQUcsQ0FBQyxFQUFBOzt3QkFBakcsU0FBaUcsQ0FBQzt3QkFDbEcsV0FBTSxzQkFBc0IsQ0FBQyxXQUFXLENBQUMsc0RBQXNELEVBQUUsTUFBTSxDQUFDLFFBQVEsQ0FBQyxpQkFBaUIsQ0FBQyxDQUFDLFFBQVEsRUFBRSxDQUFDLEVBQUE7O3dCQUEvSSxTQUErSSxDQUFDO3dCQUNoSixlQUFNOzRCQUVOLFdBQU0sc0JBQXNCLENBQUMsV0FBVyxDQUFDLGtEQUFrRCxFQUFFLEdBQUcsQ0FBQyxFQUFBOzt3QkFBakcsU0FBaUcsQ0FBQzt3QkFDbEcsZUFBTTs0QkFFTixXQUFNLHNCQUFzQixDQUFDLFdBQVcsQ0FBQyxrREFBa0QsRUFBRSxHQUFHLENBQUMsRUFBQTs7d0JBQWpHLFNBQWlHLENBQUM7d0JBQ2xHLGVBQU07NkJBRU4sV0FBTSxzQkFBc0IsQ0FBQyxXQUFXLENBQUMsa0RBQWtELEVBQUUsR0FBRyxDQUFDLEVBQUE7O3dCQUFqRyxTQUFpRyxDQUFDO3dCQUNsRyxXQUFNLHNCQUFzQixDQUFDLFdBQVcsQ0FBQyxrREFBa0QsRUFBRSxNQUFNLENBQUMsUUFBUSxDQUFDLGlCQUFpQixDQUFDLENBQUMsUUFBUSxFQUFFLENBQUMsRUFBQTs7d0JBQTNJLFNBQTJJLENBQUM7d0JBQzVJLGVBQU07NkJBRU4sV0FBTSxzQkFBc0IsQ0FBQyxXQUFXLENBQUMsaURBQWlELEVBQUUsR0FBRyxDQUFDLEVBQUE7O3dCQUFoRyxTQUFnRyxDQUFDO3dCQUNqRyxXQUFNLHNCQUFzQixDQUFDLFdBQVcsQ0FBQyxtREFBbUQsRUFBRSxNQUFNLENBQUMsUUFBUSxDQUFDLGdCQUFnQixDQUFDLENBQUMsUUFBUSxFQUFFLENBQUMsRUFBQTs7d0JBQTNJLFNBQTJJLENBQUM7d0JBQzVJLGVBQU07NkJBRU4sV0FBTSxzQkFBc0IsQ0FBQyxXQUFXLENBQUMsaURBQWlELEVBQUUsR0FBRyxDQUFDLEVBQUE7O3dCQUFoRyxTQUFnRyxDQUFDO3dCQUNqRyxlQUFNOzs7OztLQUVqQjtJQUdhLDZDQUF1QixHQUFyQzs7Ozs7NEJBQ3FELFdBQU0sSUFBSSxDQUFDLGNBQWMsQ0FBQyxJQUFJLENBQUMsY0FBYyxDQUFDLEVBQUE7O3dCQUEzRixNQUFNLEdBQXVDLFNBQThDO3dCQUNuQixXQUFNLElBQUksQ0FBQyxrQkFBa0IsRUFBRSxFQUFBOzt3QkFBdkcsc0JBQXNCLEdBQWtELFNBQStCO3dCQUMzRyxJQUFJLE1BQU0sQ0FBQyxvREFBb0QsQ0FBQyxJQUFJLElBQUksRUFBRTs0QkFBRSxXQUFPLEtBQUssRUFBQzt5QkFBRTt3QkFDM0Ysc0JBQXNCLENBQUMsV0FBVyxDQUFDLG9EQUFvRCxFQUFFLEdBQUcsQ0FBQyxDQUFDOzs7OztLQUNqRztJQUVhLDBDQUFvQixHQUFsQzs7Ozs7d0JBQ0ksSUFBSSxJQUFJLENBQUMsd0JBQXdCLElBQUksSUFBSSxFQUFFOzRCQUN2QyxJQUFJLENBQUMsd0JBQXdCLEdBQUcsb0JBQW9CLENBQUMsZ0JBQWdCLENBQUMsSUFBSSxDQUFDLFlBQVksQ0FBQyxDQUFDO3lCQUM1Rjt3QkFDTSxXQUFNLElBQUksQ0FBQyx3QkFBd0IsRUFBQTs0QkFBMUMsV0FBTyxTQUFtQyxFQUFDOzs7O0tBQzlDO0lBRWEsc0NBQWdCLEdBQTlCOzs7Ozs7NkJBQ1EsQ0FBQSxJQUFJLENBQUMsb0JBQW9CLElBQUksSUFBSSxDQUFBLEVBQWpDLGNBQWlDO3dCQUNqQyxLQUFBLElBQUksQ0FBQTt3QkFBeUIsV0FBTSxJQUFJLENBQUMsb0JBQW9CLEVBQUUsRUFBQTs7d0JBQTlELEdBQUssb0JBQW9CLEdBQUcsQ0FBQyxTQUFpQyxDQUFDLENBQUMsZ0JBQWdCLEVBQUUsQ0FBQzs7NEJBRWhGLFdBQU0sSUFBSSxDQUFDLG9CQUFvQixFQUFBOzRCQUF0QyxXQUFPLFNBQStCLEVBQUM7Ozs7S0FDMUM7SUFHTSwwRUFBb0QsR0FBM0QsVUFBNEQsS0FBVTtRQUF0RSxpQkFrQkM7UUFqQkcsT0FBTyxJQUFJLGdCQUFnQixDQUFDLFVBQU8sT0FBTyxFQUFFLE1BQU07Ozs7NEJBQzlDLFdBQU0sSUFBSSxPQUFPLENBQUMsVUFBQyxJQUFJOzRCQUNuQixNQUFNLENBQUMsVUFBVSxDQUFDLElBQUksRUFBRSxHQUFHLENBQUMsQ0FBQzt3QkFDakMsQ0FBQyxDQUFDLEVBQUE7O3dCQUZGLFNBRUUsQ0FBQzt3QkFDZ0QsV0FBTSxJQUFJLENBQUMsY0FBYyxDQUFDLENBQUMsaURBQWlELENBQUMsQ0FBQyxFQUFBOzt3QkFBN0gsUUFBUSxHQUF1QyxTQUE4RTt3QkFDN0gsVUFBVSxHQUFHLFFBQVEsQ0FBQyxRQUFRLENBQUMsaURBQWlELENBQUMsQ0FBQyxRQUFRLEVBQUUsQ0FBQzt3QkFDakcsSUFBSSxVQUFVLElBQUksSUFBSSxFQUFFOzRCQUNwQixNQUFNLENBQUMsSUFBSSxDQUFDLENBQUM7NEJBQ2IsS0FBSyxDQUFDLFFBQVEsR0FBRyxFQUFFLFNBQVMsRUFBRSxHQUFHLEVBQUUsU0FBUyxFQUFFLENBQUMsRUFBRSxVQUFVLEVBQUUsQ0FBQyxFQUFFLGFBQWEsRUFBRSxDQUFDLEVBQUUsQ0FBQzt5QkFDdEY7NkJBQU07NEJBQ0MsT0FBTyxHQUFXLEtBQUssQ0FBQyxTQUFTLENBQUMsQ0FBQzs0QkFDbkMsV0FBVyxHQUFXLEtBQUssQ0FBQyxhQUFhLENBQUMsQ0FBQzs0QkFDL0MsS0FBSyxDQUFDLFFBQVEsR0FBRyxFQUFFLFNBQVMsRUFBRSxPQUFPLEdBQUcsR0FBRyxHQUFHLFdBQVcsRUFBRSxTQUFTLEVBQUUsQ0FBQyxFQUFFLFVBQVUsRUFBRSxDQUFDLEVBQUUsYUFBYSxFQUFFLENBQUMsRUFBRSxDQUFDOzRCQUMzRyxPQUFPLENBQUMsS0FBSyxDQUFDLENBQUM7eUJBQ2xCOzs7O2FBRUosQ0FBQyxDQUFBO0lBQ04sQ0FBQztJQUdZLHFFQUErQyxHQUE1RCxVQUE2RCxLQUFVLEVBQUUsb0JBQWtDOzs7OzRCQUN2RyxXQUFNLG9CQUFvQixFQUFBOzt3QkFBMUIsU0FBMEIsQ0FBQzt3QkFDM0IsV0FBTSxJQUFJLENBQUMsZUFBZSxDQUFDLEtBQUssQ0FBQyxFQUFBOzt3QkFBakMsU0FBaUMsQ0FBQTs7Ozs7S0FDcEM7SUFFWSxxQ0FBZSxHQUE1QixVQUE2QixLQUFVOzs7Ozs7O3dCQUMvQixXQUFXLEdBQUcsS0FBSyxDQUFDO3dCQUNwQixRQUFRLEdBQUcsRUFBRSxDQUFDOzZCQUNkLENBQUEsS0FBSyxDQUFDLFFBQVEsQ0FBQyxVQUFVLElBQUksSUFBSSxDQUFBLEVBQWpDLGNBQWlDO3dCQUM3QixVQUFVLEdBQUcsS0FBSyxDQUFDLFFBQVEsQ0FBQyxVQUFVLENBQUM7d0JBQ3ZDLE9BQU8sR0FBeUI7NEJBQ2hDLFVBQVUsRUFBRTtnQ0FDUixFQUFFLEVBQUUsQ0FBQzs2QkFDUjt5QkFDSixDQUFDO3dCQUVpRCxXQUFNLElBQUksQ0FBQyxjQUFjLENBQUMsQ0FBQyxpREFBaUQsRUFBRSxzQkFBc0IsRUFBRSxhQUFhLEVBQUUsaUJBQWlCLEVBQUUsaUJBQWlCLEVBQUUsZ0JBQWdCLEVBQUUsa0RBQWtELEVBQUUsa0RBQWtELEVBQUUsaURBQWlELENBQUMsQ0FBQyxFQUFBOzt3QkFBdlgsUUFBUSxHQUF1QyxTQUF3VTt3QkFFdlgsY0FBYyxHQUFHLFFBQVEsQ0FBQyxRQUFRLENBQUMsaUJBQWlCLENBQUMsQ0FBQyxRQUFRLEVBQUUsQ0FBQzt3QkFDOUMsV0FBTSxJQUFJLENBQUMsVUFBVSxDQUFDLFVBQVUsQ0FBQyxPQUFPLENBQUMsVUFBVSxFQUFFLGlCQUFpQixDQUFDLEVBQUE7O3dCQUExRixnQkFBZ0IsR0FBRyxTQUF1RTt3QkFDdkUsV0FBTSxJQUFJLENBQUMsVUFBVSxDQUFDLFVBQVUsQ0FBQyxXQUFXLENBQUMsVUFBVSxFQUFFLGlCQUFpQixDQUFDLEVBQUE7O3dCQUE5RixnQkFBZ0IsR0FBRyxTQUEyRTt3QkFDOUYsZ0JBQWdCLEdBQUcsUUFBUSxDQUFDLFFBQVEsQ0FBQyxpQkFBaUIsQ0FBQyxDQUFDLFFBQVEsRUFBRSxDQUFDO3dCQUNuRSxhQUFhLEdBQUcsUUFBUSxDQUFDLFFBQVEsQ0FBQyxnQkFBZ0IsQ0FBQyxDQUFDLFFBQVEsRUFBRSxDQUFDO3dCQUM3QyxXQUFNLElBQUksQ0FBQyxVQUFVLENBQUMsVUFBVSxDQUFDLFFBQVEsQ0FBQyxVQUFVLEVBQUUsZ0JBQWdCLENBQUMsRUFBQTs7d0JBQXpGLGVBQWUsR0FBRyxTQUF1RTt3QkFDekYsbUJBQW1CLEdBQUcsUUFBUSxDQUFDLFFBQVEsQ0FBQyxrREFBa0QsQ0FBQyxDQUFDLFFBQVEsRUFBRSxDQUFDO3dCQUN2RyxnQkFBZ0IsR0FBRyxRQUFRLENBQUMsUUFBUSxDQUFDLGtEQUFrRCxDQUFDLENBQUMsUUFBUSxFQUFFLENBQUM7d0JBQ3BHLGVBQWUsR0FBRyxRQUFRLENBQUMsUUFBUSxDQUFDLGlEQUFpRCxDQUFDLENBQUMsUUFBUSxFQUFFLENBQUM7d0JBQ2xHLFVBQVUsR0FBRyxRQUFRLENBQUMsUUFBUSxDQUFDLGlEQUFpRCxDQUFDLENBQUMsUUFBUSxFQUFFLENBQUM7d0JBQzdGLFVBQVUsR0FBRyxRQUFRLENBQUMsUUFBUSxDQUFDLHNCQUFzQixDQUFDLENBQUMsUUFBUSxFQUFFLENBQUM7d0JBQ2xFLEdBQUcsR0FBUyxRQUFTLENBQUMsU0FBUyxFQUFFLENBQUMsU0FBUyxFQUFFLENBQUMsV0FBVyxFQUFFLENBQUM7d0JBQ2hFLElBQUksVUFBVSxJQUFJLElBQUksRUFBRTs0QkFDcEIsV0FBTzt5QkFDVjt3QkFDRCxPQUFPLENBQUMsVUFBVSxDQUFDLEVBQUUsR0FBRyxRQUFRLENBQUMsR0FBRyxDQUFDLENBQUM7d0JBQ3RDLElBQUksUUFBUSxDQUFDLEtBQUssQ0FBQyxRQUFRLENBQUMsVUFBVSxDQUFDLE9BQU8sQ0FBQyxVQUFVLENBQUMsR0FBRyxDQUFDLEVBQUU7NEJBQzVELE9BQU8sQ0FBQyxnQkFBZ0IsR0FBRyxFQUFFLElBQUksRUFBRSxRQUFRLENBQUMsTUFBQSxNQUFBLE1BQUEsS0FBSyxhQUFMLEtBQUssdUJBQUwsS0FBSyxDQUFFLFFBQVEsMENBQUUsVUFBVSwwQ0FBRSxPQUFPLDBDQUFFLFVBQVUsQ0FBQyxFQUFFLENBQUM7NEJBQ2hHLE9BQU8sQ0FBQyxrQkFBa0IsR0FBRyxRQUFRLENBQUMsTUFBQSxNQUFBLE1BQUEsS0FBSyxhQUFMLEtBQUssdUJBQUwsS0FBSyxDQUFFLFFBQVEsMENBQUUsVUFBVSwwQ0FBRSxPQUFPLDBDQUFFLFVBQVUsQ0FBQyxDQUFDOzRCQUN4RixPQUFPLENBQUMsc0JBQXNCLEdBQUcsTUFBTSxDQUFDLENBQUMsVUFBVSxDQUFDLE9BQU8sQ0FBQyxlQUFlLENBQUMsQ0FBQyxPQUFPLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQzt5QkFDNUY7d0JBQ0QsSUFBSSxRQUFRLENBQUMsY0FBYyxDQUFDLEdBQUcsQ0FBQyxFQUFFOzRCQUM5QixPQUFPLENBQUMsY0FBYyxHQUFHLEVBQUUsSUFBSSxFQUFFLFFBQVEsQ0FBQyxjQUFjLENBQUMsRUFBRSxDQUFDO3lCQUMvRDt3QkFFRCxJQUFJLFVBQVUsSUFBSSxJQUFJLENBQUMsYUFBYSxJQUFJLFVBQVUsQ0FBQyxPQUFPLElBQUksSUFBSSxJQUFJLFFBQVEsQ0FBQyxjQUFjLENBQUMsSUFBSSxRQUFRLENBQUMsTUFBQSxVQUFVLGFBQVYsVUFBVSx1QkFBVixVQUFVLENBQUUsT0FBTywwQ0FBRSxVQUFVLENBQUMsSUFBSSxDQUFDLG1CQUFtQixJQUFJLElBQUksSUFBSSxDQUFBLE1BQUEsVUFBVSxhQUFWLFVBQVUsdUJBQVYsVUFBVSxDQUFFLE9BQU8sMENBQUUsZUFBZSxJQUFHLElBQUksQ0FBQyxzQkFBc0IsQ0FBQyxFQUFFOzRCQUNoUCxRQUFRLENBQUMsSUFBSSxDQUFDO2dDQUNWLFlBQVksRUFBRSxDQUFBLE1BQUEsVUFBVSxhQUFWLFVBQVUsdUJBQVYsVUFBVSxDQUFFLE9BQU8sMENBQUUsZUFBZSxJQUFHLEdBQUc7Z0NBQ3hELGFBQWEsRUFBRSxVQUFVO2dDQUN6QixjQUFjLEVBQUUsTUFBQSxNQUFBLE1BQUEsS0FBSyxhQUFMLEtBQUssdUJBQUwsS0FBSyxDQUFFLFFBQVEsMENBQUUsVUFBVSwwQ0FBRSxPQUFPLDBDQUFFLFVBQVU7Z0NBQ2hFLGNBQWMsRUFBRSxpQkFBaUI7Z0NBQ2pDLFdBQVcsRUFBRSxpQkFBaUI7Z0NBQzlCLGFBQWEsRUFBRSwyQ0FBMkMsR0FBRyxnQkFBZ0IsR0FBRyw2REFBNkQ7NkJBQ2hKLENBQUMsQ0FBQzs0QkFDSCxXQUFXLEdBQUcsSUFBSSxDQUFDO3lCQUN0Qjt3QkFHRCxJQUFJLFVBQVUsSUFBSSxJQUFJLENBQUMsYUFBYSxJQUFJLFVBQVUsQ0FBQyxXQUFXLElBQUksSUFBSSxJQUFJLFFBQVEsQ0FBQyxnQkFBZ0IsQ0FBQyxJQUFJLFFBQVEsQ0FBQyxNQUFBLFVBQVUsYUFBVixVQUFVLHVCQUFWLFVBQVUsQ0FBRSxXQUFXLDBDQUFFLFVBQVUsQ0FBQyxJQUFJLENBQUMsZ0JBQWdCLElBQUksSUFBSSxJQUFJLENBQUEsTUFBQSxVQUFVLGFBQVYsVUFBVSx1QkFBVixVQUFVLENBQUUsV0FBVywwQ0FBRSxlQUFlLElBQUcsSUFBSSxDQUFDLHFCQUFxQixDQUFDLEVBQUU7NEJBQzFQLFFBQVEsQ0FBQyxJQUFJLENBQUM7Z0NBQ1YsWUFBWSxFQUFFLENBQUEsTUFBQSxVQUFVLGFBQVYsVUFBVSx1QkFBVixVQUFVLENBQUUsV0FBVywwQ0FBRSxlQUFlLElBQUcsR0FBRztnQ0FDNUQsYUFBYSxFQUFFLFVBQVU7Z0NBQ3pCLGNBQWMsRUFBRSxNQUFBLFVBQVUsYUFBVixVQUFVLHVCQUFWLFVBQVUsQ0FBRSxXQUFXLDBDQUFFLFVBQVU7Z0NBQ25ELGNBQWMsRUFBRSxpQkFBaUI7Z0NBQ2pDLFdBQVcsRUFBRSxpQkFBaUI7Z0NBQzlCLGFBQWEsRUFBRSwrQ0FBK0MsR0FBRyxnQkFBZ0IsR0FBRyxpRUFBaUU7NkJBQ3hKLENBQUMsQ0FBQzs0QkFDSCxXQUFXLEdBQUcsSUFBSSxDQUFDO3lCQUN0Qjt3QkFDRyxtQkFBbUIsR0FBRyxRQUFRLENBQUMsTUFBQSxVQUFVLGFBQVYsVUFBVSx1QkFBVixVQUFVLENBQUUsUUFBUSwwQ0FBRSxVQUFVLENBQUMsQ0FBQzt3QkFDckUsT0FBTyxDQUFDLG1CQUFtQixHQUFHLG1CQUFtQixDQUFDO3dCQUNsRCxPQUFPLENBQUMsaUJBQWlCLEdBQUcsbUJBQW1CLElBQUksSUFBSSxJQUFJLG1CQUFtQixHQUFHLENBQUMsQ0FBQyxDQUFDLENBQUMsRUFBRSxJQUFJLEVBQUUsbUJBQW1CLEVBQUUsQ0FBQyxDQUFDLENBQUMsSUFBSSxDQUFDO3dCQUMxSCxJQUFJLFFBQVEsQ0FBQyxhQUFhLENBQUMsR0FBRyxDQUFDLEVBQUU7NEJBQzdCLE9BQU8sQ0FBQyxpQkFBaUIsR0FBRyxRQUFRLENBQUMsYUFBYSxDQUFDLENBQUM7eUJBQ3ZEO3dCQUNELE9BQU8sQ0FBQyxlQUFlLEdBQUcsYUFBYSxJQUFJLElBQUksSUFBSSxRQUFRLENBQUMsYUFBYSxDQUFDLEdBQUcsQ0FBQyxDQUFDLENBQUMsQ0FBQyxFQUFFLElBQUksRUFBRSxRQUFRLENBQUMsYUFBYSxDQUFDLEVBQUUsQ0FBQyxDQUFDLENBQUMsSUFBSSxDQUFDO3dCQUMxSCxPQUFPLENBQUMsc0JBQXNCLEdBQUcsUUFBUSxDQUFDLFVBQVUsQ0FBQyxXQUFXLENBQUMsVUFBVSxDQUFDLENBQUM7d0JBQzdFLE9BQU8sQ0FBQyxvQkFBb0IsR0FBRyxRQUFRLENBQUMsVUFBVSxDQUFDLFdBQVcsQ0FBQyxVQUFVLENBQUMsSUFBSSxJQUFJLElBQUksUUFBUSxDQUFDLFVBQVUsQ0FBQyxXQUFXLENBQUMsVUFBVSxDQUFDLEdBQUcsQ0FBQyxDQUFDLENBQUMsQ0FBQyxFQUFFLElBQUksRUFBRSxRQUFRLENBQUMsVUFBVSxDQUFDLFdBQVcsQ0FBQyxVQUFVLENBQUMsRUFBRSxDQUFDLENBQUMsQ0FBQyxJQUFJLENBQUM7d0JBQ3JNLElBQUksUUFBUSxDQUFDLGdCQUFnQixDQUFDLEdBQUcsQ0FBQyxFQUFFOzRCQUNoQyxPQUFPLENBQUMsb0JBQW9CLEdBQUcsUUFBUSxDQUFDLGdCQUFnQixDQUFDLENBQUM7eUJBQzdEO3dCQUNELE9BQU8sQ0FBQyxrQkFBa0IsR0FBRyxnQkFBZ0IsSUFBSSxJQUFJLElBQUksUUFBUSxDQUFDLGdCQUFnQixDQUFDLEdBQUcsQ0FBQyxDQUFDLENBQUMsQ0FBQyxFQUFFLElBQUksRUFBRSxRQUFRLENBQUMsZ0JBQWdCLENBQUMsRUFBRSxDQUFDLENBQUMsQ0FBQyxJQUFJLENBQUM7d0JBQ3RJLE9BQU8sQ0FBQywwQkFBMEIsR0FBRyxNQUFNLENBQUMsQ0FBQyxNQUFBLFVBQVUsYUFBVixVQUFVLHVCQUFWLFVBQVUsQ0FBRSxXQUFXLDBDQUFFLGVBQWUsQ0FBQyxDQUFDLE9BQU8sQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDO3dCQUduRyxPQUFPLENBQUMsdUJBQXVCLEdBQUcsTUFBTSxDQUFDLENBQUMsTUFBQSxVQUFVLGFBQVYsVUFBVSx1QkFBVixVQUFVLENBQUUsUUFBUSwwQ0FBRSxlQUFlLENBQUMsQ0FBQyxPQUFPLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQzt3QkFFN0YsSUFBSSxVQUFVLElBQUksSUFBSSxDQUFDLGFBQWEsSUFBSSxVQUFVLENBQUMsUUFBUSxJQUFJLElBQUksSUFBSSxRQUFRLENBQUMsYUFBYSxDQUFDLElBQUksUUFBUSxDQUFDLE1BQUEsVUFBVSxhQUFWLFVBQVUsdUJBQVYsVUFBVSxDQUFFLFFBQVEsMENBQUUsVUFBVSxDQUFDLElBQUksQ0FBQyxlQUFlLElBQUksSUFBSSxJQUFJLENBQUEsTUFBQSxVQUFVLGFBQVYsVUFBVSx1QkFBVixVQUFVLENBQUUsUUFBUSwwQ0FBRSxlQUFlLElBQUcsSUFBSSxDQUFDLHFCQUFxQixDQUFDLEVBQUU7NEJBQzdPLElBQUksYUFBYSxLQUFJLE1BQUEsTUFBQSxNQUFBLEtBQUssYUFBTCxLQUFLLHVCQUFMLEtBQUssQ0FBRSxRQUFRLDBDQUFFLFVBQVUsMENBQUUsUUFBUSwwQ0FBRSxVQUFVLENBQUEsRUFBRTtnQ0FDcEUsUUFBUSxDQUFDLElBQUksQ0FBQztvQ0FDVixZQUFZLEVBQUUsQ0FBQSxNQUFBLFVBQVUsYUFBVixVQUFVLHVCQUFWLFVBQVUsQ0FBRSxRQUFRLDBDQUFFLGVBQWUsSUFBRyxHQUFHO29DQUN6RCxhQUFhLEVBQUUsVUFBVTtvQ0FDekIsY0FBYyxFQUFFLE1BQUEsVUFBVSxhQUFWLFVBQVUsdUJBQVYsVUFBVSxDQUFFLFFBQVEsMENBQUUsVUFBVTtvQ0FDaEQsY0FBYyxFQUFFLGdCQUFnQjtvQ0FDaEMsV0FBVyxFQUFFLGdCQUFnQjtvQ0FDN0IsYUFBYSxFQUFFLDRDQUE0QyxHQUFHLGVBQWUsR0FBRyw4REFBOEQ7aUNBQ2pKLENBQUMsQ0FBQzs2QkFDTjs0QkFDRCxXQUFXLEdBQUcsSUFBSSxDQUFDO3lCQUN0Qjt3QkFHNEIsV0FBTSxJQUFJLENBQUMsb0JBQW9CLEVBQUUsRUFBQTs0QkFBeEMsV0FBTSxDQUFDLFNBQWlDLENBQUMsQ0FBQyxrQkFBa0IsRUFBRSxFQUFBOzt3QkFBaEYsZUFBZSxHQUFHLFNBQThEO3dCQUNwRixlQUFlLENBQUMsMEJBQTBCLENBQUMsQ0FBQztnQ0FDeEMsU0FBUyxFQUFFLEtBQUssQ0FBQyxXQUFXLENBQUM7Z0NBQzdCLG1CQUFtQixFQUFFLEtBQUssQ0FBQyxxQkFBcUIsQ0FBQztnQ0FDakQsUUFBUSxFQUFFLFFBQVE7NkJBQ3JCLENBQUMsQ0FBQyxDQUFDOzZCQUVBLENBQUEsV0FBVyxJQUFJLElBQUksQ0FBQSxFQUFuQixjQUFtQjt3QkFDbkIsV0FBTSxJQUFJLENBQUMsdUJBQXVCLEVBQUUsRUFBQTs7d0JBQXBDLFNBQW9DLENBQUM7d0JBQ3JDLElBQUksQ0FBQyxpQkFBaUIsQ0FBQyxPQUFPLENBQUMsQ0FBQzs7Ozs7O0tBRzNDO0lBQ1ksZ0NBQVUsR0FBdkIsVUFBd0IsRUFBVSxFQUFFLE9BQWU7Ozs7Ozt3QkFHL0MsSUFBSSxFQUFFLElBQUksQ0FBQyxFQUFFOzRCQUNULFdBQU8sT0FBTyxDQUFDLE9BQU8sRUFBRSxFQUFDO3lCQUM1Qjt3QkFDRCxJQUFJLE9BQU8sSUFBSSxpQkFBaUIsRUFBRTs0QkFDOUIsS0FBSyxHQUFHLElBQUksQ0FBQyxjQUFjLENBQUM7eUJBQy9COzZCQUFNLElBQUksT0FBTyxJQUFJLGlCQUFpQixFQUFFOzRCQUNyQyxLQUFLLEdBQUcsSUFBSSxDQUFDLGtCQUFrQixDQUFDO3lCQUNuQzs2QkFDSTs0QkFDRCxLQUFLLEdBQUcsSUFBSSxDQUFDLGVBQWUsQ0FBQzt5QkFDaEM7d0JBQ0csZ0JBQWdCLEdBQUcsSUFBSSxnQkFBZ0IsRUFBRSxDQUFDO3dCQUNGLFdBQU0sb0JBQW9CLENBQUMsZ0JBQWdCLENBQUMsSUFBSSxDQUFDLFlBQVksQ0FBQyxFQUFBOzt3QkFBdEcsaUJBQWlCLEdBQXVCLFNBQThEO3dCQUM3RCxXQUFNLElBQUksQ0FBQyxnQkFBZ0IsRUFBRSxFQUFBOzt3QkFBdEUsYUFBYSxHQUE0QixTQUE2Qjt3QkFDMUUsV0FBTSxhQUFhLENBQUMsaUJBQWlCLEVBQUUsQ0FBQyxJQUFJLENBQUMsVUFBVSxjQUFjO2dDQUVqRSxJQUFJLHdCQUF3QixHQUFHLGNBQWMsQ0FBQyx5QkFBeUIsRUFBRSxDQUFDO2dDQUMxRSx3QkFBd0IsQ0FBQyxXQUFXLENBQUMsSUFBSSxDQUFDLENBQUM7Z0NBQzNDLHdCQUF3QixDQUFDLGNBQWMsQ0FBQyxFQUFFLENBQUMsQ0FBQztnQ0FDNUMsd0JBQXdCLENBQUMsWUFBWSxDQUFDLFNBQVMsQ0FBQyxDQUFDO2dDQUNqRCxJQUFJLGlCQUFpQixHQUFHLGNBQWMsQ0FBQyxvQkFBb0IsRUFBRSxDQUFDO2dDQUM5RCxpQkFBaUIsQ0FBQyxZQUFZLENBQUMsS0FBSyxDQUFDLENBQUM7Z0NBQ3RDLGlCQUFpQixDQUFDLHNCQUFzQixDQUFDLHdCQUF3QixDQUFDLENBQUM7Z0NBQ25FLElBQUksa0JBQWtCLEdBQUcsY0FBYyxDQUFDLFVBQVUsQ0FBQyxpQkFBaUIsQ0FBQyxDQUFDO2dDQUN0RSxrQkFBa0IsQ0FBQyxJQUFJLENBQUMsVUFBVSxpQkFBaUI7b0NBRS9DLElBQUksaUJBQWlCLENBQUMsa0JBQWtCLEVBQUUsQ0FBQyxNQUFNLEdBQUcsQ0FBQyxFQUFFO3dDQUNuRCxJQUFJLEtBQUssR0FBRyxpQkFBaUIsQ0FBQyxrQkFBa0IsRUFBRSxDQUFDO3dDQUNuRCxPQUFPLEtBQUssQ0FBQyxDQUFDLENBQUMsRUFBRTs0Q0FDYixJQUFNLE9BQU8sR0FBVyxLQUFLLENBQUMsQ0FBQyxDQUFDLENBQUMsS0FBSyxFQUFFLENBQUM7NENBQ3pDLElBQUksRUFBRSxJQUFJLE9BQU8sRUFBRTtnREFDZixnQkFBZ0IsQ0FBQyxPQUFPLENBQUMsS0FBSyxDQUFDLENBQUMsQ0FBQyxDQUFDLFFBQVEsRUFBRSxDQUFDLENBQUM7NkNBQ2pEOzRDQUNELEtBQUssR0FBRyxLQUFLLENBQUMsQ0FBQyxDQUFDLENBQUMsa0JBQWtCLEVBQUUsQ0FBQzt5Q0FDekM7cUNBR0o7Z0NBQ0wsQ0FBQyxDQUFDLENBQUM7NEJBR1AsQ0FBQyxDQUFDLEVBQUE7O3dCQTNCRixTQTJCRSxDQUFDO3dCQUVILFdBQU8sZ0JBQWdCLEVBQUM7Ozs7S0FDM0I7SUFHWSxrQ0FBWSxHQUF6Qjs7Ozs0QkFDWSxXQUFNLElBQUksQ0FBQyxnQkFBZ0IsRUFBRSxFQUFBOzRCQUFyQyxXQUFPLENBQUMsU0FBNkIsQ0FBQyxDQUFDLGVBQWUsRUFBRSxFQUFDOzs7O0tBQzVEO0lBRVksaUNBQVcsR0FBeEIsVUFBeUIsTUFBTSxFQUFFLEdBQUcsRUFBRSxPQUFPOzs7Ozs7d0JBQ3JDLGdCQUFnQixHQUFHLElBQUksZ0JBQWdCLEVBQUUsQ0FBQzt3QkFDMUMsR0FBRyxHQUFHLElBQUksY0FBYyxFQUFFLENBQUM7d0JBQ2MsV0FBTSxJQUFJLENBQUMsZ0JBQWdCLEVBQUUsRUFBQTs7d0JBQXRFLGFBQWEsR0FBNEIsU0FBNkI7d0JBQ3RFLE9BQU8sR0FBRyxhQUFhLENBQUMsc0JBQXNCLENBQUMsTUFBTSxDQUFDLEdBQUcsa0JBQWtCLENBQUE7d0JBQy9FLEdBQUcsQ0FBQyxJQUFJLENBQUMsTUFBTSxFQUFFLE9BQU8sR0FBRyxHQUFHLEVBQUUsSUFBSSxDQUFDLENBQUM7d0JBQ3RDLEdBQUcsQ0FBQyxnQkFBZ0IsQ0FBQyxnQ0FBZ0MsRUFBRSxVQUFVLENBQUMsQ0FBQzt3QkFDbkUsR0FBRyxDQUFDLGdCQUFnQixDQUFDLGNBQWMsRUFBRSxrQkFBa0IsQ0FBQyxDQUFDO3dCQUN6RCxHQUFHLENBQUMsZ0JBQWdCLENBQUMsUUFBUSxFQUFFLGtCQUFrQixDQUFDLENBQUM7d0JBQ25ELEtBQUEsQ0FBQSxLQUFBLEdBQUcsQ0FBQSxDQUFDLGdCQUFnQixDQUFBOzhCQUFDLGVBQWU7d0JBQUUsS0FBQSxVQUFVLENBQUE7d0JBQUksV0FBTSxJQUFJLENBQUMsWUFBWSxFQUFFLEVBQUE7O3dCQUE3RSx3QkFBc0MsS0FBYSxDQUFDLFNBQXlCLENBQUMsR0FBQyxDQUFDO3dCQUVoRixHQUFHLENBQUMsa0JBQWtCLEdBQUc7NEJBQ3JCLElBQUksSUFBSSxDQUFDLFVBQVUsSUFBSSxDQUFDLEVBQUU7Z0NBQ3RCLElBQUksSUFBSSxDQUFDLE1BQU0sSUFBSSxHQUFHLElBQUksSUFBSSxDQUFDLE1BQU0sSUFBSSxHQUFHLEVBQUU7b0NBQzFDLGdCQUFnQixDQUFDLE9BQU8sQ0FBQyxHQUFHLENBQUMsUUFBUSxDQUFDLENBQUM7aUNBQzFDO3FDQUFNO29DQUNILElBQUksSUFBSSxDQUFDLE1BQU0sSUFBSSxHQUFHLEVBQUU7d0NBQ3BCLElBQUksVUFBVSxHQUFHLElBQUksV0FBVyxFQUFFLENBQUM7d0NBQ25DLFVBQVUsQ0FBQyxZQUFZLEVBQUUsQ0FBQzt3Q0FDMUIsVUFBVSxDQUFDLFdBQVcsQ0FBQyxNQUFNLEVBQUUsR0FBRyxFQUFFLE9BQU8sQ0FBQyxDQUFDO3FDQUNoRDtvQ0FFRCxnQkFBZ0IsQ0FBQyxNQUFNLENBQUMsSUFBVSxvQkFBcUIsQ0FBQyxTQUFTLENBQUMsR0FBRyxDQUFDLFlBQVksQ0FBQyxDQUFDLENBQUM7aUNBQ3hGOzZCQUNKO3dCQUNMLENBQUMsQ0FBQzt3QkFFRixJQUFJLE1BQU0sSUFBSSxPQUFPLElBQUksTUFBTSxJQUFJLE1BQU0sRUFBRTs0QkFDdkMsR0FBRyxDQUFDLElBQUksQ0FBQyxJQUFJLENBQUMsU0FBUyxDQUFDLE9BQU8sRUFBRSxJQUFJLENBQUMsbUJBQW1CLEVBQUUsQ0FBQyxDQUFDLENBQUE7eUJBQ2hFOzZCQUFNOzRCQUNILEdBQUcsQ0FBQyxJQUFJLEVBQUUsQ0FBQzt5QkFDZDt3QkFDRCxXQUFPLGdCQUFnQixFQUFDOzs7O0tBQzNCO0lBRVksdUNBQWlCLEdBQTlCLFVBQStCLE9BQU87OztnQkFDbEMsT0FBTyxDQUFDLHlCQUF5QixHQUFHLE1BQU0sQ0FBQyxJQUFJLENBQUMsc0JBQXNCLENBQUMsT0FBTyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUM7Z0JBQ25GLE9BQU8sQ0FBQyw2QkFBNkIsR0FBRyxNQUFNLENBQUMsSUFBSSxDQUFDLHFCQUFxQixDQUFDLE9BQU8sQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDO2dCQUN0RixPQUFPLENBQUMsMEJBQTBCLEdBQUcsTUFBTSxDQUFDLElBQUksQ0FBQyxxQkFBcUIsQ0FBQyxPQUFPLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQztnQkFFbkYsT0FBTyxDQUFDLE1BQU0sR0FBRyxFQUFFLElBQUksRUFBRSxJQUFJLENBQUMsb0JBQW9CLEVBQUUsQ0FBQztnQkFDckQsSUFBSSxDQUFDLFdBQVcsQ0FBQyxNQUFNLEVBQUUsMEJBQTBCLEVBQUUsT0FBTyxDQUFDLENBQUM7Ozs7S0FDakU7SUFFWSxvQ0FBYyxHQUEzQixVQUE0QixjQUE2Qjs7Ozs0QkFDdkMsV0FBTSxJQUFJLENBQUMsa0JBQWtCLEVBQUUsRUFBQTs0QkFBdEMsV0FBTSxDQUFDLFNBQStCLENBQUMsQ0FBQyxjQUFjLENBQUMsY0FBYyxDQUFDLEVBQUE7NEJBQTdFLFdBQU8sU0FBc0UsRUFBQzs7OztLQUNqRjtJQUVhLHdDQUFrQixHQUFoQzs7Ozs7NEJBQ29FLFdBQU0sSUFBSSxDQUFDLG9CQUFvQixFQUFFLEVBQUE7O3dCQUE3RixnQkFBZ0IsR0FBNEMsU0FBaUM7d0JBQzdGLHNCQUFzQixHQUFnRCxJQUFJLGdCQUFnQixFQUFFLENBQUM7d0JBQ2pHLGdCQUFnQixDQUFDLDBCQUEwQixDQUFDLFVBQVUsZUFBeUM7NEJBQzNGLHNCQUFzQixDQUFDLE9BQU8sQ0FBQyxlQUFlLENBQUMsQ0FBQzt3QkFDcEQsQ0FBQyxDQUFDLENBQUM7d0JBRUksV0FBTSxzQkFBc0IsRUFBQTs0QkFBbkMsV0FBTyxTQUE0QixFQUFDOzs7O0tBQ3ZDO0lBRUwsa0JBQUM7QUFBRCxDQUFDLEFBN1pELElBNlpDO0FBR0QsSUFBSSxXQUFXLEVBQUUsQ0FBQyxVQUFVLEVBQUUsQ0FBQyIsInNvdXJjZXNDb250ZW50IjpbIi8qICogKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKlxyXG4gKiAgVGhpcyBmaWxlIGlzIHBhcnQgb2YgdGhlIE9yYWNsZSBTZXJ2aWNlIENsb3VkIEFjY2VsZXJhdG9yIFJlZmVyZW5jZSBJbnRlZ3JhdGlvbiBzZXQgcHVibGlzaGVkXHJcbiAqICBieSBPcmFjbGUgU2VydmljZSBDbG91ZCB1bmRlciB0aGUgVW5pdmVyc2FsIFBlcm1pc3NpdmUgTGljZW5zZSAoVVBMKSwgVmVyc2lvbiAxLjAgYXMgc2hvd24gYXQgXHJcbiAqICBodHRwOi8vb3NzLm9yYWNsZS5jb20vbGljZW5zZXMvdXBsXHJcbiAqICBDb3B5cmlnaHQgKGMpIDIwMjMsIE9yYWNsZSBhbmQvb3IgaXRzIGFmZmlsaWF0ZXMuXHJcbiAqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKlxyXG4gKiAgQWNjZWxlcmF0b3IgUGFja2FnZTogSW5jaWRlbnQgVGV4dCBCYXNlZCBDbGFzc2lmaWNhdGlvblxyXG4gKiAgbGluazogaHR0cDovL3d3dy5vcmFjbGUuY29tL3RlY2huZXR3b3JrL2luZGV4ZXMvc2FtcGxlY29kZS9hY2NlbGVyYXRvci1vc3ZjLTI1MjUzNjEuaHRtbFxyXG4gKiAgT1N2QyByZWxlYXNlOiAyM0EgKEZlYnJ1YXJ5IDIwMjMpIFxyXG4gKiAgZGF0ZTogTW9uIEZlYiAgNiAyMzowMTo0NiBJU1QgMjAyM1xyXG4gXHJcbiAqICByZXZpc2lvbjogcm53LTIzLTAyLWluaXRpYWxcclxuICogIFNIQTE6ICRJZDogNGI2N2ZjMzY0M2I0OGI0NmI3MGFmMGNjNTQ4OGU5ZTcxOGViZDQyOSAkXHJcbiAqICoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKlxyXG4gKiAgRmlsZTogZ2V0SW5zaWdodHMudHNcclxuICogKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqICovXHJcblxyXG4vLy8gPHJlZmVyZW5jZSBwYXRoPVwiLi9vc3ZjRXh0ZW5zaW9uLmQudHNcIiAvPlxyXG5cclxuaW50ZXJmYWNlIFByZWRpY3Rpb25SZXNwb25zZSB7XHJcbiAgICBwcmVkaWN0aW9uOiB7XHJcbiAgICAgICAgcHJvZHVjdDoge1xyXG4gICAgICAgICAgICBjb25maWRlbmNlU2NvcmU6IG51bWJlcixcclxuICAgICAgICAgICAgcHJlZGljdGlvbjogbnVtYmVyXHJcbiAgICAgICAgfSxcclxuICAgICAgICBjYXRlZ29yeToge1xyXG4gICAgICAgICAgICBwcmVkaWN0aW9uOiBudW1iZXIsXHJcbiAgICAgICAgICAgIGNvbmZpZGVuY2VTY29yZTogbnVtYmVyXHJcbiAgICAgICAgfSxcclxuICAgICAgICBkaXNwb3NpdGlvbjoge1xyXG4gICAgICAgICAgICBjb25maWRlbmNlU2NvcmU6IG51bWJlcixcclxuICAgICAgICAgICAgcHJlZGljdGlvbjogbnVtYmVyXHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG59XHJcblxyXG5pbnRlcmZhY2UgU3ViamVjdCB7XHJcbiAgICBuYW1lPzogc3RyaW5nLFxyXG4gICAgdmFsdWVzPzogQXJyYXk8U3RyaW5nPlxyXG59XHJcblxyXG5pbnRlcmZhY2UgUmVwb3J0UmVxdWVzdCB7XHJcbiAgICBpZDogbnVtYmVyLFxyXG4gICAgbGltaXQ/OiBudW1iZXIsXHJcbiAgICBmaWx0ZXJzPzogU3ViamVjdFtdXHJcbn1cclxuXHJcbmludGVyZmFjZSBQcmVkaWN0aW9uTG9nUmVxdWVzdCB7XHJcbiAgICBJbmNpZGVudElkOiB7XHJcbiAgICAgICAgaWQ6IG51bWJlclxyXG4gICAgfSxcclxuICAgIFByZWRpY3RlZFByb2R1Y3Q/OiB7XHJcbiAgICAgICAgaWQ6IG51bWJlclxyXG4gICAgfSxcclxuICAgIFByZWRpY3RlZENhdGVnb3J5Pzoge1xyXG4gICAgICAgIGlkOiBudW1iZXJcclxuICAgIH0sXHJcbiAgICBQcmVkaWN0ZWREaXNwb3NpdGlvbj86IHtcclxuICAgICAgICBpZDogbnVtYmVyXHJcbiAgICB9LFxyXG4gICAgQ3JlYXRlZENhdGVnb3J5Pzoge1xyXG4gICAgICAgIGlkOiBudW1iZXJcclxuICAgIH0sXHJcbiAgICBDcmVhdGVkUHJvZHVjdD86IHtcclxuICAgICAgICBpZDogbnVtYmVyXHJcbiAgICB9LFxyXG4gICAgQ3JlYXRlZERpc3Bvc2l0aW9uPzoge1xyXG4gICAgICAgIGlkOiBudW1iZXJcclxuICAgIH0sXHJcbiAgICBDcmVhdGVkUHJvZHVjdElkPzogbnVtYmVyLFxyXG4gICAgQ3JlYXRlZENhdGVnb3J5SWQ/OiBudW1iZXIsXHJcbiAgICBDcmVhdGVkRGlzcG9zaXRpb25JZD86IG51bWJlcixcclxuICAgIFByZWRpY3RlZFByb2R1Y3RJZD86IG51bWJlcixcclxuICAgIFByZWRpY3RlZENhdGVnb3J5SWQ/OiBudW1iZXIsXHJcbiAgICBQcmVkaWN0ZWREaXNwb3NpdGlvbklkPzogbnVtYmVyLFxyXG4gICAgQ29uZmlkZW5jZVNjb3JlRGlzcG9zaXRpb24/OiBTdHJpbmcsXHJcbiAgICBDb25maWRlbmNlU2NvcmVQcm9kdWN0PzogU3RyaW5nLFxyXG4gICAgQ29uZmlkZW5jZVNjb3JlQ2F0ZWdvcnk/OiBTdHJpbmcsXHJcbiAgICBNaW5Db25maWRlbmNlU2NvcmVQcm9kdWN0PzogU3RyaW5nLFxyXG4gICAgTWluQ29uZmlkZW5jZVNjb3JlRGlzcG9zaXRpb24/OiBTdHJpbmcsXHJcbiAgICBTb3VyY2U/OiB7XHJcbiAgICAgICAgaWQ6IG51bWJlclxyXG4gICAgfVxyXG59XHJcblxyXG5jbGFzcyBHZXRJbnNpZ2h0cyB7XHJcbiAgICBwcml2YXRlIE9QVF9JRF9QUk9EVUNUOiBudW1iZXIgPSA5O1xyXG4gICAgcHJpdmF0ZSBPUFRfSURfRElTUE9TSVRJT046IG51bWJlciA9IDE1O1xyXG4gICAgcHJpdmF0ZSBPUFRfSURfQ0FURUdPUlk6IG51bWJlciA9IDEyO1xyXG4gICAgcHJpdmF0ZSBTVEFUVVNfU09MVkVEOiBudW1iZXIgPSAyO1xyXG4gICAgcHJpdmF0ZSBDVVNUT01fQ0ZHX0NQTV9DT05GSUcgPSAnQ1VTVE9NX0NGR19DUE1fQ09ORklHJztcclxuICAgIHByaXZhdGUgcHJvZE1pbkNvbmZpZGVuY2VTY29yZSA9IDE7XHJcbiAgICBwcml2YXRlIGNhdE1pbkNvbmZpZGVuY2VTY29yZSA9IDE7XHJcbiAgICBwcml2YXRlIGRpc3BNaW5Db25maWRlY2VTY29yZSA9IDE7XHJcbiAgICBwcml2YXRlIGV4dGVuc2lvblByb3ZpZGVyUHJvbWlzZTogSUV4dGVuc2lvblByb21pc2U8SUV4dGVuc2lvblByb3ZpZGVyPiA9IG51bGw7XHJcbiAgICBwcml2YXRlIGdsb2JhbENvbnRleHRQcm9taXNlOiBJRXh0ZW5zaW9uUHJvbWlzZTxJRXh0ZW5zaW9uR2xvYmFsQ29udGV4dD4gPSBudWxsO1xyXG4gICAgcHJpdmF0ZSBvcGVuV29ya3NwYWNlc0lkczogU2V0PG51bWJlcj4gPSBuZXcgU2V0KCk7XHJcbiAgICBwcml2YXRlIElOU0lHSFRfVEVYVF9QUkVESUNUID0gMztcclxuICAgIHByaXZhdGUgZ2V0Q2lyY3VsYXJSZXBsYWNlciA9ICgpID0+IHtcclxuICAgICAgICBjb25zdCBzZWVuID0gbmV3IFdlYWtTZXQoKTtcclxuICAgICAgICByZXR1cm4gKGtleSwgdmFsdWUpID0+IHtcclxuICAgICAgICAgICAgaWYgKHR5cGVvZiB2YWx1ZSA9PT0gXCJvYmplY3RcIiAmJiB2YWx1ZSAhPT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgaWYgKHNlZW4uaGFzKHZhbHVlKSkge1xyXG4gICAgICAgICAgICAgICAgICAgIHJldHVyblxyXG4gICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgc2Vlbi5hZGQodmFsdWUpXHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgcmV0dXJuIHZhbHVlXHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG4gICAgcHJpdmF0ZSBwcmVmZXRjaEZpZWxkcyA9IFtcIkluY2lkZW50LlByb2RJZFwiLCBcIkluY2lkZW50LkNhdElkXCIsIFwiSW5jaWRlbnQuRGlzcElkXCIsIFwiUHJlZGljdGlvbiRJbmNpZGVudEludGVudERldGFpbC5JbnNpZ2h0TUxUcmlnZ2VyZWRcIiwgXCJQcmVkaWN0aW9uJEluY2lkZW50SW50ZW50RGV0YWlsLkF1dG9NTFRyaWdnZXJlZFwiLCBcIlByZWRpY3Rpb24kSW5jaWRlbnRJbnRlbnREZXRhaWwuSXNBY2NlcHRlZFByb2RNTFwiLCBcIlByZWRpY3Rpb24kSW5jaWRlbnRJbnRlbnREZXRhaWwuSXNBY2NlcHRlZENhdE1MXCIsIFwiUHJlZGljdGlvbiRJbmNpZGVudEludGVudERldGFpbC5Jc0FjY2VwdGVkRGlzcE1MXCIsICdJbmNpZGVudC5UaHJlYWRzJ107XHJcblxyXG5cclxuICAgIHB1YmxpYyBhc3luYyBpbml0aWFsaXplKCkge1xyXG4gICAgICAgIGxldCBnbG9iYWxDb250ZXh0OiBJRXh0ZW5zaW9uR2xvYmFsQ29udGV4dCA9IGF3YWl0IHRoaXMuZ2V0R2xvYmFsQ29udGV4dCgpO1xyXG4gICAgICAgIGdsb2JhbENvbnRleHQucmVnaXN0ZXJBY3Rpb24oJ2dldEZlZWRiYWNrJywgKHBhcmFtKSA9PiB0aGlzLmdldEZlZWRiYWNrKHBhcmFtKSk7XHJcblxyXG4gICAgICAgIGdsb2JhbENvbnRleHQucmVnaXN0ZXJBY3Rpb24oJ0Zvcm1hdEluc2lnaHRSZXF1ZXN0Rm9ySW5jaWRlbnRDbGFzc2lmaWNhdGlvblNlcnZpY2UnLCAocGFyYW0pID0+IHRoaXMuRm9ybWF0SW5zaWdodFJlcXVlc3RGb3JJbmNpZGVudENsYXNzaWZpY2F0aW9uU2VydmljZShwYXJhbSkpO1xyXG5cclxuICAgICAgICBsZXQgY29uZmlnUHJvbWlzZSA9IG5ldyBQcm9taXNlKGFzeW5jIChyZXNvbHZlLCByZWplY3QpID0+IHtcclxuICAgICAgICAgICAgbGV0IGNvbmZpZ3VyYXRpb25MaXN0UmVzcG9uc2UgPSBhd2FpdCB0aGlzLm1ha2VSZXF1ZXN0KCdHRVQnLCAnY29uZmlndXJhdGlvbnMnLCAnJyk7XHJcbiAgICAgICAgICAgIGlmIChjb25maWd1cmF0aW9uTGlzdFJlc3BvbnNlICE9IG51bGwpIHtcclxuICAgICAgICAgICAgICAgIGxldCBjb25maWd1cmF0aW9uTGlzdCA9IEpTT04ucGFyc2UoY29uZmlndXJhdGlvbkxpc3RSZXNwb25zZSk/Lml0ZW1zO1xyXG4gICAgICAgICAgICAgICAgY29uZmlndXJhdGlvbkxpc3QuZm9yRWFjaChhc3luYyAoaXRlbSkgPT4ge1xyXG4gICAgICAgICAgICAgICAgICAgIGlmIChpdGVtLmxvb2t1cE5hbWUgPT0gdGhpcy5DVVNUT01fQ0ZHX0NQTV9DT05GSUcpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcmVzb2x2ZShhd2FpdCB0aGlzLnByb2Nlc3NDb25maWd1cmF0aW9ucyhpdGVtKSlcclxuICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgZ2xvYmFsQ29udGV4dC5yZWdpc3RlckFjdGlvbignRm9ybWF0UmVzcG9uc2VGcm9tSW5jaWRlbnRDbGFzc2lmaWNhdGlvblNlcnZpY2UnLCAocGFyYW0pID0+IHRoaXMuRm9ybWF0UmVzcG9uc2VGcm9tSW5jaWRlbnRDbGFzc2lmaWNhdGlvblNlcnZpY2UocGFyYW0sIGNvbmZpZ1Byb21pc2UpKTtcclxuXHJcbiAgICAgICAgbGV0IHdvcmtTcGFjZVJlY29yZDogT1JBQ0xFX1NFUlZJQ0VfQ0xPVUQuSUluY2lkZW50V29ya3NwYWNlUmVjb3JkID0gYXdhaXQgdGhpcy5nZXRXb3Jrc3BhY2VSZWNvcmQoKTtcclxuXHJcbiAgICAgICAgd29ya1NwYWNlUmVjb3JkLmFkZEN1cnJlbnRFZGl0b3JUYWJDaGFuZ2VkTGlzdGVuZXIoKGN1cnJlbnRXb3Jrc3BhY2VSZWNvcmQ6IE9SQUNMRV9TRVJWSUNFX0NMT1VELklXb3Jrc3BhY2VSZWNvcmRFdmVudFBhcmFtZXRlcikgPT4gdGhpcy5wcmVQcm9jZXNzT25UYWJDaGFuZ2UoY3VycmVudFdvcmtzcGFjZVJlY29yZCkpO1xyXG5cclxuICAgIH1cclxuXHJcbiAgICBhc3luYyBwcm9jZXNzQ29uZmlndXJhdGlvbnMoaXRlbSkge1xyXG4gICAgICAgIGxldCByZXNwb25zZSA9IGF3YWl0IHRoaXMubWFrZVJlcXVlc3QoJ0dFVCcsICdjb25maWd1cmF0aW9ucy8nICsgaXRlbS5pZCwgJycpO1xyXG4gICAgICAgIGxldCBjb25maWcgPSBKU09OLnBhcnNlKEpTT04ucGFyc2UocmVzcG9uc2UpPy52YWx1ZSk7XHJcbiAgICAgICAgdGhpcy5wcm9kTWluQ29uZmlkZW5jZVNjb3JlID0gY29uZmlnPy5QUk9EVUNUX01JTl9DT05GSURFTkNFX1NDT1JFO1xyXG4gICAgICAgIHRoaXMuZGlzcE1pbkNvbmZpZGVjZVNjb3JlID0gY29uZmlnPy5ESVNQT1NJVElPTl9JVEVNU19NSU5fQ09ORklERU5DRV9TQ09SRTtcclxuICAgICAgICB0aGlzLmNhdE1pbkNvbmZpZGVuY2VTY29yZSA9IGNvbmZpZz8uQ0FURUdPUllfSVRFTVNfTUlOX0NPTkZJREVOQ0VfU0NPUkU7XHJcbiAgICB9XHJcblxyXG4gICAgYXN5bmMgcmVtb3ZlQ2xvc2VkSW5jaWRlbnRJZEZyb21Mb29rVXAoaW5jaWRlbnRJZDogbnVtYmVyKSB7XHJcbiAgICAgICAgaWYgKHRoaXMub3BlbldvcmtzcGFjZXNJZHMuaGFzKGluY2lkZW50SWQpKSB7XHJcbiAgICAgICAgICAgIHRoaXMub3BlbldvcmtzcGFjZXNJZHMuZGVsZXRlKGluY2lkZW50SWQpO1xyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBhc3luYyBwcmVQcm9jZXNzT25UYWJDaGFuZ2Uod29ya3NwYWNlUmVjb3JkRXZlbnRQYXJhbWV0ZXI6IE9SQUNMRV9TRVJWSUNFX0NMT1VELklXb3Jrc3BhY2VSZWNvcmRFdmVudFBhcmFtZXRlcikge1xyXG4gICAgICAgIGxldCBjdXJyZW50V29ya3NwYWNlUmVjb3JkOiBJV29ya3NwYWNlUmVjb3JkID0gd29ya3NwYWNlUmVjb3JkRXZlbnRQYXJhbWV0ZXIuZ2V0V29ya3NwYWNlUmVjb3JkKCk7XHJcbiAgICAgICAgbGV0IGdsb2JhbENvbnRleHQ6IE9SQUNMRV9TRVJWSUNFX0NMT1VELklFeHRlbnNpb25HbG9iYWxDb250ZXh0ID0gYXdhaXQgdGhpcy5nZXRHbG9iYWxDb250ZXh0KCk7XHJcbiAgICAgICAgaWYgKHdvcmtzcGFjZVJlY29yZEV2ZW50UGFyYW1ldGVyLm5ld1dvcmtzcGFjZS5vYmplY3RUeXBlID09ICdJbmNpZGVudCcgJiYgIXRoaXMub3BlbldvcmtzcGFjZXNJZHMuaGFzKHdvcmtzcGFjZVJlY29yZEV2ZW50UGFyYW1ldGVyLm5ld1dvcmtzcGFjZS5vYmplY3RJZCkpIHtcclxuICAgICAgICAgICAgY3VycmVudFdvcmtzcGFjZVJlY29yZC5hZGRSZWNvcmRDbG9zaW5nTGlzdGVuZXIoKGN1cnJlbnRXb3Jrc3BhY2VSZWNvcmQ6IE9SQUNMRV9TRVJWSUNFX0NMT1VELklXb3Jrc3BhY2VSZWNvcmRFdmVudFBhcmFtZXRlcikgPT4gdGhpcy5yZW1vdmVDbG9zZWRJbmNpZGVudElkRnJvbUxvb2tVcCh3b3Jrc3BhY2VSZWNvcmRFdmVudFBhcmFtZXRlci5uZXdXb3Jrc3BhY2Uub2JqZWN0SWQpKTtcclxuICAgICAgICAgICAgdGhpcy5vcGVuV29ya3NwYWNlc0lkcy5hZGQod29ya3NwYWNlUmVjb3JkRXZlbnRQYXJhbWV0ZXIubmV3V29ya3NwYWNlLm9iamVjdElkKTtcclxuXHJcbiAgICAgICAgICAgIGxldCBmaWVsZHM6IE9SQUNMRV9TRVJWSUNFX0NMT1VELklGaWVsZERldGFpbHMgPSBhd2FpdCBjdXJyZW50V29ya3NwYWNlUmVjb3JkLmdldEZpZWxkVmFsdWVzKHRoaXMucHJlZmV0Y2hGaWVsZHMpO1xyXG5cclxuICAgICAgICAgICAgY3VycmVudFdvcmtzcGFjZVJlY29yZC5hZGRGaWVsZFZhbHVlTGlzdGVuZXIoJ0luY2lkZW50LlByb2RJZCcsIGZ1bmN0aW9uIChwYXJhbSkge1xyXG4gICAgICAgICAgICAgICAgaWYgKChmaWVsZHNbJ1ByZWRpY3Rpb24kSW5jaWRlbnRJbnRlbnREZXRhaWwuSW5zaWdodE1MVHJpZ2dlcmVkJ10gPT0gMSB8fCBmaWVsZHNbJ1ByZWRpY3Rpb24kSW5jaWRlbnRJbnRlbnREZXRhaWwuQXV0b01MVHJpZ2dlcmVkJ10gPT0gMSkgJiYgZmllbGRzWydQcmVkaWN0aW9uJEluY2lkZW50SW50ZW50RGV0YWlsLklzQWNjZXB0ZWRQcm9kTUwnXSA9PSAxKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgY3VycmVudFdvcmtzcGFjZVJlY29yZC51cGRhdGVGaWVsZCgnUHJlZGljdGlvbiRJbmNpZGVudEludGVudERldGFpbC5Jc0FjY2VwdGVkUHJvZE1MJywgJzAnKTtcclxuICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgIH1cclxuXHJcblxyXG4gICAgcHVibGljIGFzeW5jIGdldEZlZWRiYWNrKHBhcmFtOiBhbnkpIHtcclxuICAgICAgICBzd2l0Y2ggKHBhcmFtLmluc2lnaHRJRCkge1xyXG4gICAgICAgICAgICBjYXNlICdJbmNpZGVudC5EaXNwSWQnOlxyXG4gICAgICAgICAgICAgICAgaWYgKHBhcmFtLmlzQWNjZXB0ZWQgPT0gdHJ1ZSkge1xyXG4gICAgICAgICAgICAgICAgICAgIHRoaXMudXBkYXRlV29ya3NwYWNlRmllbGQoJ3NldERpc3BNTFRvVHJ1ZScpO1xyXG4gICAgICAgICAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgICAgICAgICB0aGlzLnVwZGF0ZVdvcmtzcGFjZUZpZWxkKCdzZXREaXNwTUxUb0ZhbHNlJyk7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICBicmVhaztcclxuICAgICAgICAgICAgY2FzZSAnSW5jaWRlbnQuUHJvZElkJzpcclxuICAgICAgICAgICAgICAgIGlmIChwYXJhbS5pc0FjY2VwdGVkID09IGZhbHNlKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgdGhpcy51cGRhdGVXb3Jrc3BhY2VGaWVsZCgnc2V0UHJvZE1MVG9GYWxzZScpXHJcbiAgICAgICAgICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICAgICAgICAgIHRoaXMudXBkYXRlV29ya3NwYWNlRmllbGQoJ3NldFByb2RNTFRvVHJ1ZScpXHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICBicmVhaztcclxuICAgICAgICAgICAgY2FzZSAnSW5jaWRlbnQuQ2F0SWQnOlxyXG4gICAgICAgICAgICAgICAgaWYgKHBhcmFtLmlzQWNjZXB0ZWQgPT0gZmFsc2UpIHtcclxuICAgICAgICAgICAgICAgICAgICB0aGlzLnVwZGF0ZVdvcmtzcGFjZUZpZWxkKCdzZXRDYXRNTFRvRmFsc2UnKVxyXG4gICAgICAgICAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgICAgICAgICB0aGlzLnVwZGF0ZVdvcmtzcGFjZUZpZWxkKCdzZXRDYXRNTFRvVHJ1ZScpXHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICBicmVhaztcclxuICAgICAgICB9XHJcbiAgICAgICAgcmV0dXJuIFwiXCI7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBhc3luYyB1cGRhdGVXb3Jrc3BhY2VGaWVsZChhY3Rpb246IHN0cmluZykge1xyXG4gICAgICAgIGxldCBjdXJyZW50V29ya3NwYWNlUmVjb3JkOiBJV29ya3NwYWNlUmVjb3JkID0gYXdhaXQgdGhpcy5nZXRXb3Jrc3BhY2VSZWNvcmQoKTtcclxuICAgICAgICBsZXQgZmllbGRzOiBPUkFDTEVfU0VSVklDRV9DTE9VRC5JRmllbGREZXRhaWxzID0gYXdhaXQgY3VycmVudFdvcmtzcGFjZVJlY29yZC5nZXRGaWVsZFZhbHVlcyh0aGlzLnByZWZldGNoRmllbGRzKTtcclxuICAgICAgICBzd2l0Y2ggKGFjdGlvbikge1xyXG4gICAgICAgICAgICBjYXNlICdzZXREaXNwTUxUb1RydWUnOlxyXG4gICAgICAgICAgICAgICAgaWYgKGZpZWxkc1snUHJlZGljdGlvbiRJbmNpZGVudEludGVudERldGFpbC5Jc0FjY2VwdGVkRGlzcE1MJ10gIT0gbnVsbCkgeyByZXR1cm4gZmFsc2U7IH1cclxuICAgICAgICAgICAgICAgIGF3YWl0IGN1cnJlbnRXb3Jrc3BhY2VSZWNvcmQudXBkYXRlRmllbGQoXCJQcmVkaWN0aW9uJEluY2lkZW50SW50ZW50RGV0YWlsLklzQWNjZXB0ZWREaXNwTUxcIiwgJzEnKTtcclxuICAgICAgICAgICAgICAgIGF3YWl0IGN1cnJlbnRXb3Jrc3BhY2VSZWNvcmQudXBkYXRlRmllbGQoXCJQcmVkaWN0aW9uJEluY2lkZW50SW50ZW50RGV0YWlsLlByZWRpY3RlZERpc3Bvc2l0aW9uXCIsIGZpZWxkcy5nZXRGaWVsZCgnSW5jaWRlbnQuRGlzcElkJykuZ2V0VmFsdWUoKSk7XHJcbiAgICAgICAgICAgICAgICBicmVhaztcclxuICAgICAgICAgICAgY2FzZSAnc2V0RGlzcE1MVG9GYWxzZSc6XHJcbiAgICAgICAgICAgICAgICBhd2FpdCBjdXJyZW50V29ya3NwYWNlUmVjb3JkLnVwZGF0ZUZpZWxkKFwiUHJlZGljdGlvbiRJbmNpZGVudEludGVudERldGFpbC5Jc0FjY2VwdGVkRGlzcE1MXCIsICcwJyk7XHJcbiAgICAgICAgICAgICAgICBicmVhaztcclxuICAgICAgICAgICAgY2FzZSAnc2V0UHJvZE1MVG9GYWxzZSc6XHJcbiAgICAgICAgICAgICAgICBhd2FpdCBjdXJyZW50V29ya3NwYWNlUmVjb3JkLnVwZGF0ZUZpZWxkKFwiUHJlZGljdGlvbiRJbmNpZGVudEludGVudERldGFpbC5Jc0FjY2VwdGVkUHJvZE1MXCIsICcwJyk7XHJcbiAgICAgICAgICAgICAgICBicmVhaztcclxuICAgICAgICAgICAgY2FzZSAnc2V0UHJvZE1MVG9UcnVlJzpcclxuICAgICAgICAgICAgICAgIGF3YWl0IGN1cnJlbnRXb3Jrc3BhY2VSZWNvcmQudXBkYXRlRmllbGQoXCJQcmVkaWN0aW9uJEluY2lkZW50SW50ZW50RGV0YWlsLklzQWNjZXB0ZWRQcm9kTUxcIiwgJzEnKTtcclxuICAgICAgICAgICAgICAgIGF3YWl0IGN1cnJlbnRXb3Jrc3BhY2VSZWNvcmQudXBkYXRlRmllbGQoXCJQcmVkaWN0aW9uJEluY2lkZW50SW50ZW50RGV0YWlsLlByZWRpY3RlZFByb2R1Y3RcIiwgZmllbGRzLmdldEZpZWxkKCdJbmNpZGVudC5Qcm9kSWQnKS5nZXRWYWx1ZSgpKTtcclxuICAgICAgICAgICAgICAgIGJyZWFrO1xyXG4gICAgICAgICAgICBjYXNlICdzZXRDYXRNTFRvVHJ1ZSc6XHJcbiAgICAgICAgICAgICAgICBhd2FpdCBjdXJyZW50V29ya3NwYWNlUmVjb3JkLnVwZGF0ZUZpZWxkKFwiUHJlZGljdGlvbiRJbmNpZGVudEludGVudERldGFpbC5Jc0FjY2VwdGVkQ2F0TUxcIiwgJzEnKTtcclxuICAgICAgICAgICAgICAgIGF3YWl0IGN1cnJlbnRXb3Jrc3BhY2VSZWNvcmQudXBkYXRlRmllbGQoXCJQcmVkaWN0aW9uJEluY2lkZW50SW50ZW50RGV0YWlsLlByZWRpY3RlZENhdGVnb3J5XCIsIGZpZWxkcy5nZXRGaWVsZCgnSW5jaWRlbnQuQ2F0SWQnKS5nZXRWYWx1ZSgpKTtcclxuICAgICAgICAgICAgICAgIGJyZWFrO1xyXG4gICAgICAgICAgICBjYXNlICdzZXRDYXRNTFRvRmFsc2UnOlxyXG4gICAgICAgICAgICAgICAgYXdhaXQgY3VycmVudFdvcmtzcGFjZVJlY29yZC51cGRhdGVGaWVsZChcIlByZWRpY3Rpb24kSW5jaWRlbnRJbnRlbnREZXRhaWwuSXNBY2NlcHRlZENhdE1MXCIsICcwJyk7XHJcbiAgICAgICAgICAgICAgICBicmVhaztcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG5cclxuICAgIHByaXZhdGUgYXN5bmMgdXBkYXRlSW5zaWdodHNUcmlnZ2VyZWQoKSB7XHJcbiAgICAgICAgbGV0IGZpZWxkczogT1JBQ0xFX1NFUlZJQ0VfQ0xPVUQuSUZpZWxkRGV0YWlscyA9IGF3YWl0IHRoaXMuZ2V0RmllbGRWYWx1ZXModGhpcy5wcmVmZXRjaEZpZWxkcyk7XHJcbiAgICAgICAgbGV0IGN1cnJlbnRXb3Jrc3BhY2VSZWNvcmQ6IE9SQUNMRV9TRVJWSUNFX0NMT1VELklJbmNpZGVudFdvcmtzcGFjZVJlY29yZCA9IGF3YWl0IHRoaXMuZ2V0V29ya3NwYWNlUmVjb3JkKCk7XHJcbiAgICAgICAgaWYgKGZpZWxkc1snUHJlZGljdGlvbiRJbmNpZGVudEludGVudERldGFpbC5JbnNpZ2h0TUxUcmlnZ2VyZWQnXSA9PSB0cnVlKSB7IHJldHVybiBmYWxzZTsgfVxyXG4gICAgICAgIGN1cnJlbnRXb3Jrc3BhY2VSZWNvcmQudXBkYXRlRmllbGQoJ1ByZWRpY3Rpb24kSW5jaWRlbnRJbnRlbnREZXRhaWwuSW5zaWdodE1MVHJpZ2dlcmVkJywgJzEnKTtcclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIGFzeW5jIGdldEV4dGVuc2lvblByb3ZpZGVyKCk6IFByb21pc2U8T1JBQ0xFX1NFUlZJQ0VfQ0xPVUQuSUV4dGVuc2lvblByb3ZpZGVyPiB7XHJcbiAgICAgICAgaWYgKHRoaXMuZXh0ZW5zaW9uUHJvdmlkZXJQcm9taXNlID09IG51bGwpIHtcclxuICAgICAgICAgICAgdGhpcy5leHRlbnNpb25Qcm92aWRlclByb21pc2UgPSBPUkFDTEVfU0VSVklDRV9DTE9VRC5leHRlbnNpb25fbG9hZGVyLmxvYWQoXCJNTF9JbnNpZ2h0XCIpO1xyXG4gICAgICAgIH1cclxuICAgICAgICByZXR1cm4gYXdhaXQgdGhpcy5leHRlbnNpb25Qcm92aWRlclByb21pc2U7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBhc3luYyBnZXRHbG9iYWxDb250ZXh0KCk6IFByb21pc2U8T1JBQ0xFX1NFUlZJQ0VfQ0xPVUQuSUV4dGVuc2lvbkdsb2JhbENvbnRleHQ+IHtcclxuICAgICAgICBpZiAodGhpcy5nbG9iYWxDb250ZXh0UHJvbWlzZSA9PSBudWxsKSB7XHJcbiAgICAgICAgICAgIHRoaXMuZ2xvYmFsQ29udGV4dFByb21pc2UgPSAoYXdhaXQgdGhpcy5nZXRFeHRlbnNpb25Qcm92aWRlcigpKS5nZXRHbG9iYWxDb250ZXh0KCk7XHJcbiAgICAgICAgfVxyXG4gICAgICAgIHJldHVybiBhd2FpdCB0aGlzLmdsb2JhbENvbnRleHRQcm9taXNlO1xyXG4gICAgfVxyXG5cclxuXHJcbiAgICBwdWJsaWMgRm9ybWF0SW5zaWdodFJlcXVlc3RGb3JJbmNpZGVudENsYXNzaWZpY2F0aW9uU2VydmljZShwYXJhbTogYW55KSB7XHJcbiAgICAgICAgcmV0dXJuIG5ldyBFeHRlbnNpb25Qcm9taXNlKGFzeW5jIChyZXNvbHZlLCByZWplY3QpID0+IHtcclxuICAgICAgICAgICAgYXdhaXQgbmV3IFByb21pc2UoKGRvbmUpID0+IHtcclxuICAgICAgICAgICAgICAgIHdpbmRvdy5zZXRUaW1lb3V0KGRvbmUsIDEwMCk7XHJcbiAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICBsZXQgZmllbGRWYWw6IE9SQUNMRV9TRVJWSUNFX0NMT1VELklGaWVsZERldGFpbHMgPSBhd2FpdCB0aGlzLmdldEZpZWxkVmFsdWVzKFsnUHJlZGljdGlvbiRJbmNpZGVudEludGVudERldGFpbC5BdXRvTUxUcmlnZ2VyZWQnXSk7XHJcbiAgICAgICAgICAgIGxldCBjcG1VcGRhdGVkID0gZmllbGRWYWwuZ2V0RmllbGQoJ1ByZWRpY3Rpb24kSW5jaWRlbnRJbnRlbnREZXRhaWwuQXV0b01MVHJpZ2dlcmVkJykuZ2V0VmFsdWUoKTtcclxuICAgICAgICAgICAgaWYgKGNwbVVwZGF0ZWQgPT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgcmVqZWN0KG51bGwpO1xyXG4gICAgICAgICAgICAgICAgcGFyYW0uanNvbkRhdGEgPSB7ICdpbnF1aXJ5JzogJyAnLCAncHJvZHVjdCc6IDAsICdjYXRlZ29yeSc6IDAsICdkaXNwb3NpdGlvbic6IDAgfTtcclxuICAgICAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgICAgIGxldCBzdWJqZWN0OiBzdHJpbmcgPSBwYXJhbVsnc3ViamVjdCddO1xyXG4gICAgICAgICAgICAgICAgbGV0IGRlc2NyaXB0aW9uOiBzdHJpbmcgPSBwYXJhbVsnZGVzY3JpcHRpb24nXTtcclxuICAgICAgICAgICAgICAgIHBhcmFtLmpzb25EYXRhID0geyAnaW5xdWlyeSc6IHN1YmplY3QgKyAnICcgKyBkZXNjcmlwdGlvbiwgJ3Byb2R1Y3QnOiAwLCAnY2F0ZWdvcnknOiAwLCAnZGlzcG9zaXRpb24nOiAwIH07XHJcbiAgICAgICAgICAgICAgICByZXNvbHZlKHBhcmFtKTtcclxuICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICB9KVxyXG4gICAgfVxyXG5cclxuXHJcbiAgICBwdWJsaWMgYXN5bmMgRm9ybWF0UmVzcG9uc2VGcm9tSW5jaWRlbnRDbGFzc2lmaWNhdGlvblNlcnZpY2UocGFyYW06IGFueSwgY29uZmlndXJhdGlvblByb21pc2U6IFByb21pc2U8YW55Pikge1xyXG4gICAgICAgIGF3YWl0IGNvbmZpZ3VyYXRpb25Qcm9taXNlO1xyXG4gICAgICAgIGF3YWl0IHRoaXMucmVzcG9uc2VQcm9jZXNzKHBhcmFtKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBhc3luYyByZXNwb25zZVByb2Nlc3MocGFyYW06IGFueSkge1xyXG4gICAgICAgIGxldCB3ZW5lZWR0b2xvZyA9IGZhbHNlO1xyXG4gICAgICAgIGxldCBpbnNpZ2h0cyA9IFtdO1xyXG4gICAgICAgIGlmIChwYXJhbS5pbnNpZ2h0cy5wcmVkaWN0aW9uICE9IG51bGwpIHtcclxuICAgICAgICAgICAgbGV0IHByZWRpY3Rpb24gPSBwYXJhbS5pbnNpZ2h0cy5wcmVkaWN0aW9uO1xyXG4gICAgICAgICAgICBsZXQgcGF5bG9hZDogUHJlZGljdGlvbkxvZ1JlcXVlc3QgPSB7XHJcbiAgICAgICAgICAgICAgICBJbmNpZGVudElkOiB7XHJcbiAgICAgICAgICAgICAgICAgICAgaWQ6IDBcclxuICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgfTtcclxuXHJcbiAgICAgICAgICAgIGxldCBmaWVsZFZhbDogT1JBQ0xFX1NFUlZJQ0VfQ0xPVUQuSUZpZWxkRGV0YWlscyA9IGF3YWl0IHRoaXMuZ2V0RmllbGRWYWx1ZXMoWydQcmVkaWN0aW9uJEluY2lkZW50SW50ZW50RGV0YWlsLkF1dG9NTFRyaWdnZXJlZCcsICdJbmNpZGVudC5TdGF0dXMuVHlwZScsICdJbmNpZGVudC5JRCcsICdJbmNpZGVudC5Qcm9kSWQnLCAnSW5jaWRlbnQuRGlzcElkJywgJ0luY2lkZW50LkNhdElkJywgJ1ByZWRpY3Rpb24kSW5jaWRlbnRJbnRlbnREZXRhaWwuSXNBY2NlcHRlZFByb2RNTCcsICdQcmVkaWN0aW9uJEluY2lkZW50SW50ZW50RGV0YWlsLklzQWNjZXB0ZWREaXNwTUwnLCAnUHJlZGljdGlvbiRJbmNpZGVudEludGVudERldGFpbC5Jc0FjY2VwdGVkQ2F0TUwnXSk7XHJcblxyXG4gICAgICAgICAgICBsZXQgY3VycmVudFByb2RWYWwgPSBmaWVsZFZhbC5nZXRGaWVsZCgnSW5jaWRlbnQuUHJvZElkJykuZ2V0VmFsdWUoKTtcclxuICAgICAgICAgICAgbGV0IHByZWRpY3RlZFByb2RMYmwgPSBhd2FpdCB0aGlzLmdldE9wdExpc3QocHJlZGljdGlvbi5wcm9kdWN0LnByZWRpY3Rpb24sICdJbmNpZGVudC5Qcm9kSWQnKTtcclxuICAgICAgICAgICAgbGV0IHByZWRpY3RlZERpc3BMYmwgPSBhd2FpdCB0aGlzLmdldE9wdExpc3QocHJlZGljdGlvbi5kaXNwb3NpdGlvbi5wcmVkaWN0aW9uLCAnSW5jaWRlbnQuRGlzcElkJyk7XHJcbiAgICAgICAgICAgIGxldCBjdXJyZW50RGlzcFZhbHVlID0gZmllbGRWYWwuZ2V0RmllbGQoJ0luY2lkZW50LkRpc3BJZCcpLmdldFZhbHVlKCk7XHJcbiAgICAgICAgICAgIGxldCBjdXJyZW50Q2F0VmFsID0gZmllbGRWYWwuZ2V0RmllbGQoJ0luY2lkZW50LkNhdElkJykuZ2V0VmFsdWUoKTtcclxuICAgICAgICAgICAgbGV0IHByZWRpY3RlZENhdExibCA9IGF3YWl0IHRoaXMuZ2V0T3B0TGlzdChwcmVkaWN0aW9uLmNhdGVnb3J5LnByZWRpY3Rpb24sICdJbmNpZGVudC5DYXRJZCcpO1xyXG4gICAgICAgICAgICBsZXQgaXNBY2NlcHRlZFByb2RNTFZhbCA9IGZpZWxkVmFsLmdldEZpZWxkKCdQcmVkaWN0aW9uJEluY2lkZW50SW50ZW50RGV0YWlsLklzQWNjZXB0ZWRQcm9kTUwnKS5nZXRWYWx1ZSgpO1xyXG4gICAgICAgICAgICBsZXQgaXNBY2NlcHRlZERpc3BNTCA9IGZpZWxkVmFsLmdldEZpZWxkKCdQcmVkaWN0aW9uJEluY2lkZW50SW50ZW50RGV0YWlsLklzQWNjZXB0ZWREaXNwTUwnKS5nZXRWYWx1ZSgpO1xyXG4gICAgICAgICAgICBsZXQgaXNBY2NlcHRlZENhdE1MID0gZmllbGRWYWwuZ2V0RmllbGQoJ1ByZWRpY3Rpb24kSW5jaWRlbnRJbnRlbnREZXRhaWwuSXNBY2NlcHRlZENhdE1MJykuZ2V0VmFsdWUoKTtcclxuICAgICAgICAgICAgbGV0IGNwbVVwZGF0ZWQgPSBmaWVsZFZhbC5nZXRGaWVsZCgnUHJlZGljdGlvbiRJbmNpZGVudEludGVudERldGFpbC5BdXRvTUxUcmlnZ2VyZWQnKS5nZXRWYWx1ZSgpO1xyXG4gICAgICAgICAgICBsZXQgc3RhdHVzVHlwZSA9IGZpZWxkVmFsLmdldEZpZWxkKCdJbmNpZGVudC5TdGF0dXMuVHlwZScpLmdldFZhbHVlKCk7XHJcbiAgICAgICAgICAgIGxldCB3SWQgPSAoPGFueT5maWVsZFZhbCkuZ2V0UGFyZW50KCkuZ2V0UGFyZW50KCkuZ2V0RW50aXR5SWQoKTtcclxuICAgICAgICAgICAgaWYgKGNwbVVwZGF0ZWQgPT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIHBheWxvYWQuSW5jaWRlbnRJZC5pZCA9IHBhcnNlSW50KHdJZCk7XHJcbiAgICAgICAgICAgIGlmIChwYXJzZUludChwYXJhbS5pbnNpZ2h0cy5wcmVkaWN0aW9uLnByb2R1Y3QucHJlZGljdGlvbikgPiAwKSB7XHJcbiAgICAgICAgICAgICAgICBwYXlsb2FkLlByZWRpY3RlZFByb2R1Y3QgPSB7IFwiaWRcIjogcGFyc2VJbnQocGFyYW0/Lmluc2lnaHRzPy5wcmVkaWN0aW9uPy5wcm9kdWN0Py5wcmVkaWN0aW9uKSB9O1xyXG4gICAgICAgICAgICAgICAgcGF5bG9hZC5QcmVkaWN0ZWRQcm9kdWN0SWQgPSBwYXJzZUludChwYXJhbT8uaW5zaWdodHM/LnByZWRpY3Rpb24/LnByb2R1Y3Q/LnByZWRpY3Rpb24pO1xyXG4gICAgICAgICAgICAgICAgcGF5bG9hZC5Db25maWRlbmNlU2NvcmVQcm9kdWN0ID0gU3RyaW5nKChwcmVkaWN0aW9uLnByb2R1Y3QuY29uZmlkZW5jZVNjb3JlKS50b0ZpeGVkKDIpKTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICBpZiAocGFyc2VJbnQoY3VycmVudFByb2RWYWwpID4gMCkge1xyXG4gICAgICAgICAgICAgICAgcGF5bG9hZC5DcmVhdGVkUHJvZHVjdCA9IHsgXCJpZFwiOiBwYXJzZUludChjdXJyZW50UHJvZFZhbCkgfTtcclxuICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgaWYgKHN0YXR1c1R5cGUgIT0gdGhpcy5TVEFUVVNfU09MVkVEICYmIHByZWRpY3Rpb24ucHJvZHVjdCAhPSBudWxsICYmIHBhcnNlSW50KGN1cnJlbnRQcm9kVmFsKSAhPSBwYXJzZUludChwcmVkaWN0aW9uPy5wcm9kdWN0Py5wcmVkaWN0aW9uKSAmJiAoaXNBY2NlcHRlZFByb2RNTFZhbCA9PSBudWxsICYmIHByZWRpY3Rpb24/LnByb2R1Y3Q/LmNvbmZpZGVuY2VTY29yZSA+IHRoaXMucHJvZE1pbkNvbmZpZGVuY2VTY29yZSkpIHtcclxuICAgICAgICAgICAgICAgIGluc2lnaHRzLnB1c2goe1xyXG4gICAgICAgICAgICAgICAgICAgICdjb25maWRlbmNlJzogcHJlZGljdGlvbj8ucHJvZHVjdD8uY29uZmlkZW5jZVNjb3JlICogMTAwLFxyXG4gICAgICAgICAgICAgICAgICAgICdpbnNpZ2h0VHlwZSc6ICdzZXRGaWVsZCcsXHJcbiAgICAgICAgICAgICAgICAgICAgJ2luc2lnaHRWYWx1ZSc6IHBhcmFtPy5pbnNpZ2h0cz8ucHJlZGljdGlvbj8ucHJvZHVjdD8ucHJlZGljdGlvbixcclxuICAgICAgICAgICAgICAgICAgICAnaW5zaWdodEZpZWxkJzogJ0luY2lkZW50LlByb2RJZCcsXHJcbiAgICAgICAgICAgICAgICAgICAgJ2luc2lnaHRJZCc6ICdJbmNpZGVudC5Qcm9kSWQnLFxyXG4gICAgICAgICAgICAgICAgICAgICdkZXNjcmlwdGlvbic6ICdUaGUgc3lzdGVtIGhhcyBpZGVudGlmaWVkIHRoZSBwcm9kdWN0IGFzICcgKyBwcmVkaWN0ZWRQcm9kTGJsICsgJy4gRG8geW91IHdhbnQgdG8gdXBkYXRlIHRoZSBwcm9kdWN0IHRvIHRoZSBzdWdnZXN0ZWQgdmFsdWU/J1xyXG4gICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgICAgICB3ZW5lZWR0b2xvZyA9IHRydWU7XHJcbiAgICAgICAgICAgIH1cclxuXHJcblxyXG4gICAgICAgICAgICBpZiAoc3RhdHVzVHlwZSAhPSB0aGlzLlNUQVRVU19TT0xWRUQgJiYgcHJlZGljdGlvbi5kaXNwb3NpdGlvbiAhPSBudWxsICYmIHBhcnNlSW50KGN1cnJlbnREaXNwVmFsdWUpICE9IHBhcnNlSW50KHByZWRpY3Rpb24/LmRpc3Bvc2l0aW9uPy5wcmVkaWN0aW9uKSAmJiAoaXNBY2NlcHRlZERpc3BNTCA9PSBudWxsICYmIHByZWRpY3Rpb24/LmRpc3Bvc2l0aW9uPy5jb25maWRlbmNlU2NvcmUgPiB0aGlzLmRpc3BNaW5Db25maWRlY2VTY29yZSkpIHtcclxuICAgICAgICAgICAgICAgIGluc2lnaHRzLnB1c2goe1xyXG4gICAgICAgICAgICAgICAgICAgICdjb25maWRlbmNlJzogcHJlZGljdGlvbj8uZGlzcG9zaXRpb24/LmNvbmZpZGVuY2VTY29yZSAqIDEwMCxcclxuICAgICAgICAgICAgICAgICAgICAnaW5zaWdodFR5cGUnOiAnc2V0RmllbGQnLFxyXG4gICAgICAgICAgICAgICAgICAgICdpbnNpZ2h0VmFsdWUnOiBwcmVkaWN0aW9uPy5kaXNwb3NpdGlvbj8ucHJlZGljdGlvbixcclxuICAgICAgICAgICAgICAgICAgICAnaW5zaWdodEZpZWxkJzogJ0luY2lkZW50LkRpc3BJZCcsXHJcbiAgICAgICAgICAgICAgICAgICAgJ2luc2lnaHRJZCc6ICdJbmNpZGVudC5EaXNwSWQnLFxyXG4gICAgICAgICAgICAgICAgICAgICdkZXNjcmlwdGlvbic6ICdUaGUgc3lzdGVtIGhhcyBpZGVudGlmaWVkIHRoZSBkaXNwb3NpdGlvbiBhcyAnICsgcHJlZGljdGVkRGlzcExibCArICcuIERvIHlvdSB3YW50IHRvIHVwZGF0ZSB0aGUgZGlzcG9zaXRpb24gdG8gdGhlIHN1Z2dlc3RlZCB2YWx1ZT8nXHJcbiAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgICAgIHdlbmVlZHRvbG9nID0gdHJ1ZTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICBsZXQgcHJlZGljdGVkQ2F0ZWdvcnlJZCA9IHBhcnNlSW50KHByZWRpY3Rpb24/LmNhdGVnb3J5Py5wcmVkaWN0aW9uKTtcclxuICAgICAgICAgICAgcGF5bG9hZC5QcmVkaWN0ZWRDYXRlZ29yeUlkID0gcHJlZGljdGVkQ2F0ZWdvcnlJZDtcclxuICAgICAgICAgICAgcGF5bG9hZC5QcmVkaWN0ZWRDYXRlZ29yeSA9IHByZWRpY3RlZENhdGVnb3J5SWQgIT0gbnVsbCAmJiBwcmVkaWN0ZWRDYXRlZ29yeUlkID4gMCA/IHsgXCJpZFwiOiBwcmVkaWN0ZWRDYXRlZ29yeUlkIH0gOiBudWxsO1xyXG4gICAgICAgICAgICBpZiAocGFyc2VJbnQoY3VycmVudENhdFZhbCkgPiAwKSB7XHJcbiAgICAgICAgICAgICAgICBwYXlsb2FkLkNyZWF0ZWRDYXRlZ29yeUlkID0gcGFyc2VJbnQoY3VycmVudENhdFZhbCk7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgcGF5bG9hZC5DcmVhdGVkQ2F0ZWdvcnkgPSBjdXJyZW50Q2F0VmFsICE9IG51bGwgJiYgcGFyc2VJbnQoY3VycmVudENhdFZhbCkgPiAwID8geyBcImlkXCI6IHBhcnNlSW50KGN1cnJlbnRDYXRWYWwpIH0gOiBudWxsO1xyXG4gICAgICAgICAgICBwYXlsb2FkLlByZWRpY3RlZERpc3Bvc2l0aW9uSWQgPSBwYXJzZUludChwcmVkaWN0aW9uLmRpc3Bvc2l0aW9uLnByZWRpY3Rpb24pO1xyXG4gICAgICAgICAgICBwYXlsb2FkLlByZWRpY3RlZERpc3Bvc2l0aW9uID0gcGFyc2VJbnQocHJlZGljdGlvbi5kaXNwb3NpdGlvbi5wcmVkaWN0aW9uKSAhPSBudWxsICYmIHBhcnNlSW50KHByZWRpY3Rpb24uZGlzcG9zaXRpb24ucHJlZGljdGlvbikgPiAwID8geyBcImlkXCI6IHBhcnNlSW50KHByZWRpY3Rpb24uZGlzcG9zaXRpb24ucHJlZGljdGlvbikgfSA6IG51bGw7XHJcbiAgICAgICAgICAgIGlmIChwYXJzZUludChjdXJyZW50RGlzcFZhbHVlKSA+IDApIHtcclxuICAgICAgICAgICAgICAgIHBheWxvYWQuQ3JlYXRlZERpc3Bvc2l0aW9uSWQgPSBwYXJzZUludChjdXJyZW50RGlzcFZhbHVlKTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICBwYXlsb2FkLkNyZWF0ZWREaXNwb3NpdGlvbiA9IGN1cnJlbnREaXNwVmFsdWUgIT0gbnVsbCAmJiBwYXJzZUludChjdXJyZW50RGlzcFZhbHVlKSA+IDAgPyB7IFwiaWRcIjogcGFyc2VJbnQoY3VycmVudERpc3BWYWx1ZSkgfSA6IG51bGw7XHJcbiAgICAgICAgICAgIHBheWxvYWQuQ29uZmlkZW5jZVNjb3JlRGlzcG9zaXRpb24gPSBTdHJpbmcoKHByZWRpY3Rpb24/LmRpc3Bvc2l0aW9uPy5jb25maWRlbmNlU2NvcmUpLnRvRml4ZWQoMikpO1xyXG5cclxuXHJcbiAgICAgICAgICAgIHBheWxvYWQuQ29uZmlkZW5jZVNjb3JlQ2F0ZWdvcnkgPSBTdHJpbmcoKHByZWRpY3Rpb24/LmNhdGVnb3J5Py5jb25maWRlbmNlU2NvcmUpLnRvRml4ZWQoMikpO1xyXG5cclxuICAgICAgICAgICAgaWYgKHN0YXR1c1R5cGUgIT0gdGhpcy5TVEFUVVNfU09MVkVEICYmIHByZWRpY3Rpb24uY2F0ZWdvcnkgIT0gbnVsbCAmJiBwYXJzZUludChjdXJyZW50Q2F0VmFsKSAhPSBwYXJzZUludChwcmVkaWN0aW9uPy5jYXRlZ29yeT8ucHJlZGljdGlvbikgJiYgKGlzQWNjZXB0ZWRDYXRNTCA9PSBudWxsICYmIHByZWRpY3Rpb24/LmNhdGVnb3J5Py5jb25maWRlbmNlU2NvcmUgPiB0aGlzLmNhdE1pbkNvbmZpZGVuY2VTY29yZSkpIHtcclxuICAgICAgICAgICAgICAgIGlmIChjdXJyZW50Q2F0VmFsICE9IHBhcmFtPy5pbnNpZ2h0cz8ucHJlZGljdGlvbj8uY2F0ZWdvcnk/LnByZWRpY3Rpb24pIHtcclxuICAgICAgICAgICAgICAgICAgICBpbnNpZ2h0cy5wdXNoKHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgJ2NvbmZpZGVuY2UnOiBwcmVkaWN0aW9uPy5jYXRlZ29yeT8uY29uZmlkZW5jZVNjb3JlICogMTAwLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAnaW5zaWdodFR5cGUnOiAnc2V0RmllbGQnLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAnaW5zaWdodFZhbHVlJzogcHJlZGljdGlvbj8uY2F0ZWdvcnk/LnByZWRpY3Rpb24sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICdpbnNpZ2h0RmllbGQnOiAnSW5jaWRlbnQuQ2F0SWQnLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAnaW5zaWdodElkJzogJ0luY2lkZW50LkNhdElkJyxcclxuICAgICAgICAgICAgICAgICAgICAgICAgJ2Rlc2NyaXB0aW9uJzogJ1RoZSBzeXN0ZW0gaGFzIGlkZW50aWZpZWQgdGhlIGNhdGVnb3J5IGFzICcgKyBwcmVkaWN0ZWRDYXRMYmwgKyAnLiBEbyB5b3Ugd2FudCB0byB1cGRhdGUgdGhlIGNhdGVnb3J5IHRvIHRoZSBzdWdnZXN0ZWQgdmFsdWU/J1xyXG4gICAgICAgICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgd2VuZWVkdG9sb2cgPSB0cnVlO1xyXG4gICAgICAgICAgICB9XHJcblxyXG5cclxuICAgICAgICAgICAgbGV0IGluc2lnaHRzQ29udGV4dCA9IGF3YWl0IChhd2FpdCB0aGlzLmdldEV4dGVuc2lvblByb3ZpZGVyKCkpLmdldEluc2lnaHRzQ29udGV4dCgpO1xyXG4gICAgICAgICAgICBpbnNpZ2h0c0NvbnRleHQuaGFuZGxlSW5zaWdodFJlc3BvbnNlUmVhZHkoW3tcclxuICAgICAgICAgICAgICAgIGNvbnRleHRJZDogcGFyYW1bJ2NvbnRleHRJZCddLFxyXG4gICAgICAgICAgICAgICAgaW5zaWdodENvbm5lY3Rpb25JZDogcGFyYW1bJ2luc2lnaHRDb25uZWN0aW9uSWQnXSxcclxuICAgICAgICAgICAgICAgIHJlc3BvbnNlOiBpbnNpZ2h0c1xyXG4gICAgICAgICAgICB9XSk7XHJcblxyXG4gICAgICAgICAgICBpZiAod2VuZWVkdG9sb2cgPT0gdHJ1ZSkge1xyXG4gICAgICAgICAgICAgICAgYXdhaXQgdGhpcy51cGRhdGVJbnNpZ2h0c1RyaWdnZXJlZCgpO1xyXG4gICAgICAgICAgICAgICAgdGhpcy5sb2dQcmVkaWN0aW9uRGF0YShwYXlsb2FkKTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH1cclxuICAgIH1cclxuICAgIHB1YmxpYyBhc3luYyBnZXRPcHRMaXN0KGlkOiBudW1iZXIsIG9wdGxpc3Q6IFN0cmluZykge1xyXG4gICAgICAgIGxldCBvcHRJZDogbnVtYmVyO1xyXG5cclxuICAgICAgICBpZiAoaWQgPT0gMCkge1xyXG4gICAgICAgICAgICByZXR1cm4gUHJvbWlzZS5yZXNvbHZlKCk7XHJcbiAgICAgICAgfVxyXG4gICAgICAgIGlmIChvcHRsaXN0ID09ICdJbmNpZGVudC5Qcm9kSWQnKSB7XHJcbiAgICAgICAgICAgIG9wdElkID0gdGhpcy5PUFRfSURfUFJPRFVDVDtcclxuICAgICAgICB9IGVsc2UgaWYgKG9wdGxpc3QgPT0gJ0luY2lkZW50LkRpc3BJZCcpIHtcclxuICAgICAgICAgICAgb3B0SWQgPSB0aGlzLk9QVF9JRF9ESVNQT1NJVElPTjtcclxuICAgICAgICB9XHJcbiAgICAgICAgZWxzZSB7XHJcbiAgICAgICAgICAgIG9wdElkID0gdGhpcy5PUFRfSURfQ0FURUdPUlk7XHJcbiAgICAgICAgfVxyXG4gICAgICAgIGxldCBleHRlbnNpb25Qcm9taXNlID0gbmV3IEV4dGVuc2lvblByb21pc2UoKTtcclxuICAgICAgICBsZXQgZXh0ZW50aW9uUHJvdmlkZXI6IElFeHRlbnNpb25Qcm92aWRlciA9IGF3YWl0IE9SQUNMRV9TRVJWSUNFX0NMT1VELmV4dGVuc2lvbl9sb2FkZXIubG9hZChcIk1MX0luc2lnaHRcIik7XHJcbiAgICAgICAgbGV0IGdsb2JhbENvbnRleHQ6IElFeHRlbnNpb25HbG9iYWxDb250ZXh0ID0gYXdhaXQgdGhpcy5nZXRHbG9iYWxDb250ZXh0KCk7XHJcbiAgICAgICAgYXdhaXQgZ2xvYmFsQ29udGV4dC5nZXRPcHRMaXN0Q29udGV4dCgpLnRoZW4oZnVuY3Rpb24gKG9wdExpc3RDb250ZXh0KSB7XHJcblxyXG4gICAgICAgICAgICB2YXIgb3B0TGlzdExhYmVsU2VhcmNoRmlsdGVyID0gb3B0TGlzdENvbnRleHQuY3JlYXRlT3B0TGlzdFNlYXJjaEZpbHRlcigpO1xyXG4gICAgICAgICAgICBvcHRMaXN0TGFiZWxTZWFyY2hGaWx0ZXIuc2V0U2VhcmNoQnkoJ0lkJyk7XHJcbiAgICAgICAgICAgIG9wdExpc3RMYWJlbFNlYXJjaEZpbHRlci5zZXRTZWFyY2hWYWx1ZShpZCk7XHJcbiAgICAgICAgICAgIG9wdExpc3RMYWJlbFNlYXJjaEZpbHRlci5zZXRDb25kaXRpb24oJ2lzRXF1YWwnKTtcclxuICAgICAgICAgICAgdmFyIGdldE9wdExpc3RSZXF1ZXN0ID0gb3B0TGlzdENvbnRleHQuY3JlYXRlT3B0TGlzdFJlcXVlc3QoKTtcclxuICAgICAgICAgICAgZ2V0T3B0TGlzdFJlcXVlc3Quc2V0T3B0TGlzdElkKG9wdElkKTtcclxuICAgICAgICAgICAgZ2V0T3B0TGlzdFJlcXVlc3Quc2V0T3B0TGlzdFNlYXJjaEZpbHRlcihvcHRMaXN0TGFiZWxTZWFyY2hGaWx0ZXIpO1xyXG4gICAgICAgICAgICB2YXIgb3B0TGlzdEl0ZW1Qcm9taXNlID0gb3B0TGlzdENvbnRleHQuZ2V0T3B0TGlzdChnZXRPcHRMaXN0UmVxdWVzdCk7XHJcbiAgICAgICAgICAgIG9wdExpc3RJdGVtUHJvbWlzZS50aGVuKGZ1bmN0aW9uIChvcHRMaXN0SXRlbVJlc3VsdCkge1xyXG5cclxuICAgICAgICAgICAgICAgIGlmIChvcHRMaXN0SXRlbVJlc3VsdC5nZXRPcHRMaXN0Q2hpbGRyZW4oKS5sZW5ndGggPiAwKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgbGV0IGNoaWxkID0gb3B0TGlzdEl0ZW1SZXN1bHQuZ2V0T3B0TGlzdENoaWxkcmVuKCk7XHJcbiAgICAgICAgICAgICAgICAgICAgd2hpbGUgKGNoaWxkWzBdKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGNvbnN0IGNoaWxkSWQ6IG51bWJlciA9IGNoaWxkWzBdLmdldElkKCk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGlmIChpZCA9PSBjaGlsZElkKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBleHRlbnNpb25Qcm9taXNlLnJlc29sdmUoY2hpbGRbMF0uZ2V0TGFiZWwoKSk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgY2hpbGQgPSBjaGlsZFswXS5nZXRPcHRMaXN0Q2hpbGRyZW4oKTtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG5cclxuICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgfSk7XHJcblxyXG5cclxuICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgcmV0dXJuIGV4dGVuc2lvblByb21pc2U7XHJcbiAgICB9XHJcblxyXG5cclxuICAgIHB1YmxpYyBhc3luYyByZWZyZXNoVG9rZW4oKSB7XHJcbiAgICAgICAgcmV0dXJuIChhd2FpdCB0aGlzLmdldEdsb2JhbENvbnRleHQoKSkuZ2V0U2Vzc2lvblRva2VuKCk7XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIGFzeW5jIG1ha2VSZXF1ZXN0KG1ldGhvZCwgdXJpLCBwYXlsb2FkKSB7XHJcbiAgICAgICAgbGV0IGV4dGVuc2lvblByb21pc2UgPSBuZXcgRXh0ZW5zaW9uUHJvbWlzZSgpO1xyXG4gICAgICAgIGxldCB4aHIgPSBuZXcgWE1MSHR0cFJlcXVlc3QoKTtcclxuICAgICAgICBsZXQgZ2xvYmFsQ29udGV4dDogSUV4dGVuc2lvbkdsb2JhbENvbnRleHQgPSBhd2FpdCB0aGlzLmdldEdsb2JhbENvbnRleHQoKTtcclxuICAgICAgICBsZXQgcmVzdFVybCA9IGdsb2JhbENvbnRleHQuZ2V0SW50ZXJmYWNlU2VydmljZVVybCgnUkVTVCcpICsgXCIvY29ubmVjdC9sYXRlc3QvXCJcclxuICAgICAgICB4aHIub3BlbihtZXRob2QsIHJlc3RVcmwgKyB1cmksIHRydWUpO1xyXG4gICAgICAgIHhoci5zZXRSZXF1ZXN0SGVhZGVyKCdvc3ZjLWNyZXN0LWFwcGxpY2F0aW9uLWNvbnRleHQnLCAnSW5zaWdodHMnKTtcclxuICAgICAgICB4aHIuc2V0UmVxdWVzdEhlYWRlcignQ29udGVudC1UeXBlJywgJ2FwcGxpY2F0aW9uL2pzb24nKTtcclxuICAgICAgICB4aHIuc2V0UmVxdWVzdEhlYWRlcignQWNjZXB0JywgJ2FwcGxpY2F0aW9uL2pzb24nKTtcclxuICAgICAgICB4aHIuc2V0UmVxdWVzdEhlYWRlcignQXV0aG9yaXphdGlvbicsICdTZXNzaW9uICcgKyAoYXdhaXQgdGhpcy5yZWZyZXNoVG9rZW4oKSkpO1xyXG5cclxuICAgICAgICB4aHIub25yZWFkeXN0YXRlY2hhbmdlID0gZnVuY3Rpb24gKCkge1xyXG4gICAgICAgICAgICBpZiAodGhpcy5yZWFkeVN0YXRlID09IDQpIHtcclxuICAgICAgICAgICAgICAgIGlmICh0aGlzLnN0YXR1cyA9PSAyMDAgfHwgdGhpcy5zdGF0dXMgPT0gMjAxKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgZXh0ZW5zaW9uUHJvbWlzZS5yZXNvbHZlKHhoci5yZXNwb25zZSk7XHJcbiAgICAgICAgICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICAgICAgICAgIGlmICh0aGlzLnN0YXR1cyA9PSA0MDEpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgbGV0IGdldEluc2lnaHQgPSBuZXcgR2V0SW5zaWdodHMoKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgZ2V0SW5zaWdodC5yZWZyZXNoVG9rZW4oKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgZ2V0SW5zaWdodC5tYWtlUmVxdWVzdChtZXRob2QsIHVyaSwgcGF5bG9hZCk7XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICBleHRlbnNpb25Qcm9taXNlLnJlamVjdChuZXcgKDxhbnk+T1JBQ0xFX1NFUlZJQ0VfQ0xPVUQpLkVycm9yRGF0YSh4aHIucmVzcG9uc2VUZXh0KSk7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9O1xyXG5cclxuICAgICAgICBpZiAobWV0aG9kID09ICdQQVRDSCcgfHwgbWV0aG9kID09ICdQT1NUJykge1xyXG4gICAgICAgICAgICB4aHIuc2VuZChKU09OLnN0cmluZ2lmeShwYXlsb2FkLCB0aGlzLmdldENpcmN1bGFyUmVwbGFjZXIoKSkpXHJcbiAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgeGhyLnNlbmQoKTtcclxuICAgICAgICB9XHJcbiAgICAgICAgcmV0dXJuIGV4dGVuc2lvblByb21pc2U7XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIGFzeW5jIGxvZ1ByZWRpY3Rpb25EYXRhKHBheWxvYWQpIHtcclxuICAgICAgICBwYXlsb2FkLk1pbkNvbmZpZGVuY2VTY29yZVByb2R1Y3QgPSBTdHJpbmcodGhpcy5wcm9kTWluQ29uZmlkZW5jZVNjb3JlLnRvRml4ZWQoMikpO1xyXG4gICAgICAgIHBheWxvYWQuTWluQ29uZmlkZW5jZVNjb3JlRGlzcG9zaXRpb24gPSBTdHJpbmcodGhpcy5kaXNwTWluQ29uZmlkZWNlU2NvcmUudG9GaXhlZCgyKSk7XHJcbiAgICAgICAgcGF5bG9hZC5NaW5Db25maWRlbmNlU2NvcmVDYXRlZ29yeSA9IFN0cmluZyh0aGlzLmNhdE1pbkNvbmZpZGVuY2VTY29yZS50b0ZpeGVkKDIpKTtcclxuXHJcbiAgICAgICAgcGF5bG9hZC5Tb3VyY2UgPSB7IFwiaWRcIjogdGhpcy5JTlNJR0hUX1RFWFRfUFJFRElDVCB9O1xyXG4gICAgICAgIHRoaXMubWFrZVJlcXVlc3QoJ1BPU1QnLCAnUHJlZGljdGlvbi5QcmVkaWN0aW9uTG9nJywgcGF5bG9hZCk7XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIGFzeW5jIGdldEZpZWxkVmFsdWVzKGZpZWxkTmFtZUFycmF5OiBBcnJheTxzdHJpbmc+KTogUHJvbWlzZTxJRmllbGREZXRhaWxzPiB7XHJcbiAgICAgICAgcmV0dXJuIGF3YWl0IChhd2FpdCB0aGlzLmdldFdvcmtzcGFjZVJlY29yZCgpKS5nZXRGaWVsZFZhbHVlcyhmaWVsZE5hbWVBcnJheSk7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBhc3luYyBnZXRXb3Jrc3BhY2VSZWNvcmQoKTogUHJvbWlzZTxJSW5jaWRlbnRXb3Jrc3BhY2VSZWNvcmQ+IHtcclxuICAgICAgICBsZXQgZXh0ZW5zaW9uUHJvdmlkZTogT1JBQ0xFX1NFUlZJQ0VfQ0xPVUQuSUV4dGVuc2lvblByb3ZpZGVyID0gYXdhaXQgdGhpcy5nZXRFeHRlbnNpb25Qcm92aWRlcigpO1xyXG4gICAgICAgIGxldCB3b3Jrc3BhY2VSZWNvcmRQcm9taXNlOiBJRXh0ZW5zaW9uUHJvbWlzZTxJSW5jaWRlbnRXb3Jrc3BhY2VSZWNvcmQ+ID0gbmV3IEV4dGVuc2lvblByb21pc2UoKTtcclxuICAgICAgICBleHRlbnNpb25Qcm92aWRlLnJlZ2lzdGVyV29ya3NwYWNlRXh0ZW5zaW9uKGZ1bmN0aW9uICh3b3Jrc3BhY2VSZWNvcmQ6IElJbmNpZGVudFdvcmtzcGFjZVJlY29yZCkge1xyXG4gICAgICAgICAgICB3b3Jrc3BhY2VSZWNvcmRQcm9taXNlLnJlc29sdmUod29ya3NwYWNlUmVjb3JkKTtcclxuICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgcmV0dXJuIGF3YWl0IHdvcmtzcGFjZVJlY29yZFByb21pc2U7XHJcbiAgICB9XHJcblxyXG59XHJcblxyXG5cclxubmV3IEdldEluc2lnaHRzKCkuaW5pdGlhbGl6ZSgpOyJdfQ==