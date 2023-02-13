
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
 *  SHA1: $Id: e343168c62afc0ab2ae5927c01288f93681766af $
 * *********************************************************************************************
 *  File: getInsights.ts
 * ****************************************************************************************** */

/// <reference path="./osvcExtension.d.ts" />


interface PredictionResponse {
    prediction: {
        product: {
            confidenceScore: number,
            prediction: number
        },
        category: {
            prediction: number,
            confidenceScore: number
        },
        disposition: {
            confidenceScore: number,
            prediction: number
        }
    }
}

interface Subject {
    name?: string,
    values?: Array<String>
}

interface ReportRequest {
    id: number,
    limit?: number,
    filters?: Subject[]
}

interface PredictionLogRequest {
    IncidentId: {
        id: number
    },
    PredictedProduct?: {
        id: number
    },
    PredictedCategory?: {
        id: number
    },
    PredictedDisposition?: {
        id: number
    },
    CreatedCategory?: {
        id: number
    },
    CreatedProduct?: {
        id: number
    },
    CreatedDisposition?: {
        id: number
    },
    CreatedProductId?: number,
    CreatedCategoryId?: number,
    CreatedDispositionId?: number,
    PredictedProductId?: number,
    PredictedCategoryId?: number,
    PredictedDispositionId?: number,
    ConfidenceScoreDisposition?: String,
    ConfidenceScoreProduct?: String,
    ConfidenceScoreCategory?: String,
    MinConfidenceScoreProduct?: String,
    MinConfidenceScoreDisposition?: String,
    Source?: {
        id: number
    }
}

class GetInsights {
    private OPT_ID_PRODUCT: number = 9;
    private OPT_ID_DISPOSITION: number = 15;
    private OPT_ID_CATEGORY: number = 12;
    private STATUS_SOLVED: number = 2;
    private CUSTOM_CFG_CPM_CONFIG = 'CUSTOM_CFG_CPM_CONFIG';
    private prodMinConfidenceScore = 1;
    private catMinConfidenceScore = 1;
    private dispMinConfideceScore = 1;
    private extensionProviderPromise: IExtensionPromise<IExtensionProvider> = null;
    private globalContextPromise: IExtensionPromise<IExtensionGlobalContext> = null;
    private openWorkspacesIds: Set<number> = new Set();
    private INSIGHT_TEXT_PREDICT = 3;
    private getCircularReplacer = () => {
        const seen = new WeakSet();
        return (key, value) => {
            if (typeof value === "object" && value !== null) {
                if (seen.has(value)) {
                    return
                }
                seen.add(value)
            }
            return value
        }
    }
    private prefetchFields = ["Incident.ProdId", "Incident.CatId", "Incident.DispId", "Prediction$IncidentIntentDetail.InsightMLTriggered", "Prediction$IncidentIntentDetail.AutoMLTriggered", "Prediction$IncidentIntentDetail.IsAcceptedProdML", "Prediction$IncidentIntentDetail.IsAcceptedCatML", "Prediction$IncidentIntentDetail.IsAcceptedDispML", 'Incident.Threads'];


    public async initialize() {
        let globalContext: IExtensionGlobalContext = await this.getGlobalContext();
        globalContext.registerAction('getFeedback', (param) => this.getFeedback(param));

        globalContext.registerAction('FormatInsightRequestForIncidentClassificationService', (param) => this.FormatInsightRequestForIncidentClassificationService(param));

        let configPromise = new Promise(async (resolve, reject) => {
            let configurationListResponse = await this.makeRequest('GET', 'configurations', '');
            if (configurationListResponse != null) {
                let configurationList = JSON.parse(configurationListResponse)?.items;
                configurationList.forEach(async (item) => {
                    if (item.lookupName == this.CUSTOM_CFG_CPM_CONFIG) {
                        resolve(await this.processConfigurations(item))
                    }
                });

            }
        });

        globalContext.registerAction('FormatResponseFromIncidentClassificationService', (param) => this.FormatResponseFromIncidentClassificationService(param, configPromise));

        let workSpaceRecord: ORACLE_SERVICE_CLOUD.IIncidentWorkspaceRecord = await this.getWorkspaceRecord();

        workSpaceRecord.addCurrentEditorTabChangedListener((currentWorkspaceRecord: ORACLE_SERVICE_CLOUD.IWorkspaceRecordEventParameter) => this.preProcessOnTabChange(currentWorkspaceRecord));

    }

    async processConfigurations(item) {
        let response = await this.makeRequest('GET', 'configurations/' + item.id, '');
        let config = JSON.parse(JSON.parse(response)?.value);
        this.prodMinConfidenceScore = config?.PRODUCT_MIN_CONFIDENCE_SCORE;
        this.dispMinConfideceScore = config?.DISPOSITION_ITEMS_MIN_CONFIDENCE_SCORE;
        this.catMinConfidenceScore = config?.CATEGORY_ITEMS_MIN_CONFIDENCE_SCORE;
    }

    async removeClosedIncidentIdFromLookUp(incidentId: number) {
        if (this.openWorkspacesIds.has(incidentId)) {
            this.openWorkspacesIds.delete(incidentId);
        }
    }

    async preProcessOnTabChange(workspaceRecordEventParameter: ORACLE_SERVICE_CLOUD.IWorkspaceRecordEventParameter) {
        let currentWorkspaceRecord: IWorkspaceRecord = workspaceRecordEventParameter.getWorkspaceRecord();
        let globalContext: ORACLE_SERVICE_CLOUD.IExtensionGlobalContext = await this.getGlobalContext();
        if (workspaceRecordEventParameter.newWorkspace.objectType == 'Incident' && !this.openWorkspacesIds.has(workspaceRecordEventParameter.newWorkspace.objectId)) {
            currentWorkspaceRecord.addRecordClosingListener((currentWorkspaceRecord: ORACLE_SERVICE_CLOUD.IWorkspaceRecordEventParameter) => this.removeClosedIncidentIdFromLookUp(workspaceRecordEventParameter.newWorkspace.objectId));
            this.openWorkspacesIds.add(workspaceRecordEventParameter.newWorkspace.objectId);

            let fields: ORACLE_SERVICE_CLOUD.IFieldDetails = await currentWorkspaceRecord.getFieldValues(this.prefetchFields);

            currentWorkspaceRecord.addFieldValueListener('Incident.ProdId', function (param) {
                if ((fields['Prediction$IncidentIntentDetail.InsightMLTriggered'] == 1 || fields['Prediction$IncidentIntentDetail.AutoMLTriggered'] == 1) && fields['Prediction$IncidentIntentDetail.IsAcceptedProdML'] == 1) {
                    currentWorkspaceRecord.updateField('Prediction$IncidentIntentDetail.IsAcceptedProdML', '0');
                }
            });
        }

    }


    public async getFeedback(param: any) {
        switch (param.insightID) {
            case 'Incident.DispId':
                if (param.isAccepted == true) {
                    this.updateWorkspaceField('setDispMLToTrue');
                } else {
                    this.updateWorkspaceField('setDispMLToFalse');
                }
                break;
            case 'Incident.ProdId':
                if (param.isAccepted == false) {
                    this.updateWorkspaceField('setProdMLToFalse')
                } else {
                    this.updateWorkspaceField('setProdMLToTrue')
                }
                break;
            case 'Incident.CatId':
                if (param.isAccepted == false) {
                    this.updateWorkspaceField('setCatMLToFalse')
                } else {
                    this.updateWorkspaceField('setCatMLToTrue')
                }
                break;
        }
        return "";
    }

    private async updateWorkspaceField(action: string) {
        let currentWorkspaceRecord: IWorkspaceRecord = await this.getWorkspaceRecord();
        let fields: ORACLE_SERVICE_CLOUD.IFieldDetails = await currentWorkspaceRecord.getFieldValues(this.prefetchFields);
        switch (action) {
            case 'setDispMLToTrue':
                if (fields['Prediction$IncidentIntentDetail.IsAcceptedDispML'] != null) { return false; }
                await currentWorkspaceRecord.updateField("Prediction$IncidentIntentDetail.IsAcceptedDispML", '1');
                await currentWorkspaceRecord.updateField("Prediction$IncidentIntentDetail.PredictedDisposition", fields.getField('Incident.DispId').getValue());
                break;
            case 'setDispMLToFalse':
                await currentWorkspaceRecord.updateField("Prediction$IncidentIntentDetail.IsAcceptedDispML", '0');
                break;
            case 'setProdMLToFalse':
                await currentWorkspaceRecord.updateField("Prediction$IncidentIntentDetail.IsAcceptedProdML", '0');
                break;
            case 'setProdMLToTrue':
                await currentWorkspaceRecord.updateField("Prediction$IncidentIntentDetail.IsAcceptedProdML", '1');
                await currentWorkspaceRecord.updateField("Prediction$IncidentIntentDetail.PredictedProduct", fields.getField('Incident.ProdId').getValue());
                break;
            case 'setCatMLToTrue':
                await currentWorkspaceRecord.updateField("Prediction$IncidentIntentDetail.IsAcceptedCatML", '1');
                await currentWorkspaceRecord.updateField("Prediction$IncidentIntentDetail.PredictedCategory", fields.getField('Incident.CatId').getValue());
                break;
            case 'setCatMLToFalse':
                await currentWorkspaceRecord.updateField("Prediction$IncidentIntentDetail.IsAcceptedCatML", '0');
                break;
        }
    }


    private async updateInsightsTriggered() {
        let fields: ORACLE_SERVICE_CLOUD.IFieldDetails = await this.getFieldValues(this.prefetchFields);
        let currentWorkspaceRecord: ORACLE_SERVICE_CLOUD.IIncidentWorkspaceRecord = await this.getWorkspaceRecord();
        if (fields['Prediction$IncidentIntentDetail.InsightMLTriggered'] == true) { return false; }
        currentWorkspaceRecord.updateField('Prediction$IncidentIntentDetail.InsightMLTriggered', '1');
    }

    private async getExtensionProvider(): Promise<ORACLE_SERVICE_CLOUD.IExtensionProvider> {
        if (this.extensionProviderPromise == null) {
            this.extensionProviderPromise = ORACLE_SERVICE_CLOUD.extension_loader.load("ML_Insight");
        }
        return await this.extensionProviderPromise;
    }

    private async getGlobalContext(): Promise<ORACLE_SERVICE_CLOUD.IExtensionGlobalContext> {
        if (this.globalContextPromise == null) {
            this.globalContextPromise = (await this.getExtensionProvider()).getGlobalContext();
        }
        return await this.globalContextPromise;
    }


    public FormatInsightRequestForIncidentClassificationService(param: any) {
        return new ExtensionPromise(async (resolve, reject) => {
            await new Promise((done) => {
                window.setTimeout(done, 100);
            });
            let fieldVal: ORACLE_SERVICE_CLOUD.IFieldDetails = await this.getFieldValues(['Prediction$IncidentIntentDetail.AutoMLTriggered']);
            let cpmUpdated = fieldVal.getField('Prediction$IncidentIntentDetail.AutoMLTriggered').getValue();
            if (cpmUpdated == null) {
                reject(null);
                param.jsonData = { 'inquiry': ' ', 'product': 0, 'category': 0, 'disposition': 0 };
            } else {
                let subject: string = param['subject'];
                let description: string = param['description'];
                param.jsonData = { 'inquiry': subject + ' ' + description, 'product': 0, 'category': 0, 'disposition': 0 };
                resolve(param);
            }

        })
    }


    public async FormatResponseFromIncidentClassificationService(param: any, configurationPromise: Promise<any>) {
        await configurationPromise;
        await this.responseProcess(param)
    }

    public async responseProcess(param: any) {
        let weneedtolog = false;
        let insights = [];
        if (param.insights.prediction != null) {
            let prediction = param.insights.prediction;
            let payload: PredictionLogRequest = {
                IncidentId: {
                    id: 0
                }
            };

            let fieldVal: ORACLE_SERVICE_CLOUD.IFieldDetails = await this.getFieldValues(['Prediction$IncidentIntentDetail.AutoMLTriggered', 'Incident.Status.Type', 'Incident.ID', 'Incident.ProdId', 'Incident.DispId', 'Incident.CatId', 'Prediction$IncidentIntentDetail.IsAcceptedProdML', 'Prediction$IncidentIntentDetail.IsAcceptedDispML', 'Prediction$IncidentIntentDetail.IsAcceptedCatML']);

            let currentProdVal = fieldVal.getField('Incident.ProdId').getValue();
            let predictedProdLbl = await this.getOptList(prediction.product.prediction, 'Incident.ProdId');
            let predictedDispLbl = await this.getOptList(prediction.disposition.prediction, 'Incident.DispId');
            let currentDispValue = fieldVal.getField('Incident.DispId').getValue();
            let currentCatVal = fieldVal.getField('Incident.CatId').getValue();
            let predictedCatLbl = await this.getOptList(prediction.category.prediction, 'Incident.CatId');
            let isAcceptedProdMLVal = fieldVal.getField('Prediction$IncidentIntentDetail.IsAcceptedProdML').getValue();
            let isAcceptedDispML = fieldVal.getField('Prediction$IncidentIntentDetail.IsAcceptedDispML').getValue();
            let isAcceptedCatML = fieldVal.getField('Prediction$IncidentIntentDetail.IsAcceptedCatML').getValue();
            let cpmUpdated = fieldVal.getField('Prediction$IncidentIntentDetail.AutoMLTriggered').getValue();
            let statusType = fieldVal.getField('Incident.Status.Type').getValue();
            let wId = (<any>fieldVal).getParent().getParent().getEntityId();
            if (cpmUpdated == null) {
                return;
            }
            payload.IncidentId.id = parseInt(wId);
            if (parseInt(param.insights.prediction.product.prediction) > 0) {
                payload.PredictedProduct = { "id": parseInt(param?.insights?.prediction?.product?.prediction) };
                payload.PredictedProductId = parseInt(param?.insights?.prediction?.product?.prediction);
                payload.ConfidenceScoreProduct = String((prediction.product.confidenceScore).toFixed(2));
            }
            if (parseInt(currentProdVal) > 0) {
                payload.CreatedProduct = { "id": parseInt(currentProdVal) };
            }

            if (statusType != this.STATUS_SOLVED && prediction.product != null && parseInt(currentProdVal) != parseInt(prediction?.product?.prediction) && (isAcceptedProdMLVal == null && prediction?.product?.confidenceScore > this.prodMinConfidenceScore)) {
                insights.push({
                    'confidence': prediction?.product?.confidenceScore * 100,
                    'insightType': 'setField',
                    'insightValue': param?.insights?.prediction?.product?.prediction,
                    'insightField': 'Incident.ProdId',
                    'insightId': 'Incident.ProdId',
                    'description': 'The system has identified the product as ' + predictedProdLbl + '. Do you want to update the product to the suggested value?'
                });
                weneedtolog = true;
            }


            if (statusType != this.STATUS_SOLVED && prediction.disposition != null && parseInt(currentDispValue) != parseInt(prediction?.disposition?.prediction) && (isAcceptedDispML == null && prediction?.disposition?.confidenceScore > this.dispMinConfideceScore)) {
                insights.push({
                    'confidence': prediction?.disposition?.confidenceScore * 100,
                    'insightType': 'setField',
                    'insightValue': prediction?.disposition?.prediction,
                    'insightField': 'Incident.DispId',
                    'insightId': 'Incident.DispId',
                    'description': 'The system has identified the disposition as ' + predictedDispLbl + '. Do you want to update the disposition to the suggested value?'
                });
                weneedtolog = true;
            }
            let predictedCategoryId = parseInt(prediction?.category?.prediction);
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
            payload.ConfidenceScoreDisposition = String((prediction?.disposition?.confidenceScore).toFixed(2));


            payload.ConfidenceScoreCategory = String((prediction?.category?.confidenceScore).toFixed(2));

            if (statusType != this.STATUS_SOLVED && prediction.category != null && parseInt(currentCatVal) != parseInt(prediction?.category?.prediction) && (isAcceptedCatML == null && prediction?.category?.confidenceScore > this.catMinConfidenceScore)) {
                if (currentCatVal != param?.insights?.prediction?.category?.prediction) {
                    insights.push({
                        'confidence': prediction?.category?.confidenceScore * 100,
                        'insightType': 'setField',
                        'insightValue': prediction?.category?.prediction,
                        'insightField': 'Incident.CatId',
                        'insightId': 'Incident.CatId',
                        'description': 'The system has identified the category as ' + predictedCatLbl + '. Do you want to update the category to the suggested value?'
                    });
                }
                weneedtolog = true;
            }


            let insightsContext = await (await this.getExtensionProvider()).getInsightsContext();
            insightsContext.handleInsightResponseReady([{
                contextId: param['contextId'],
                insightConnectionId: param['insightConnectionId'],
                response: insights
            }]);

            if (weneedtolog == true) {
                await this.updateInsightsTriggered();
                this.logPredictionData(payload);
            }
        }
    }
    public async getOptList(id: number, optlist: String) {
        let optId: number;

        if (id == 0) {
            return Promise.resolve();
        }
        if (optlist == 'Incident.ProdId') {
            optId = this.OPT_ID_PRODUCT;
        } else if (optlist == 'Incident.DispId') {
            optId = this.OPT_ID_DISPOSITION;
        }
        else {
            optId = this.OPT_ID_CATEGORY;
        }
        let extensionPromise = new ExtensionPromise();
        let extentionProvider: IExtensionProvider = await ORACLE_SERVICE_CLOUD.extension_loader.load("ML_Insight");
        let globalContext: IExtensionGlobalContext = await this.getGlobalContext();
        await globalContext.getOptListContext().then(function (optListContext) {

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
                    let child = optListItemResult.getOptListChildren();
                    while (child[0]) {
                        const childId: number = child[0].getId();
                        if (id == childId) {
                            extensionPromise.resolve(child[0].getLabel());
                        }
                        child = child[0].getOptListChildren();
                    }


                }
            });


        });

        return extensionPromise;
    }


    public async refreshToken() {
        return (await this.getGlobalContext()).getSessionToken();
    }

    public async makeRequest(method, uri, payload) {
        let extensionPromise = new ExtensionPromise();
        let xhr = new XMLHttpRequest();
        let globalContext: IExtensionGlobalContext = await this.getGlobalContext();
        let restUrl = globalContext.getInterfaceServiceUrl('REST') + "/connect/latest/"
        xhr.open(method, restUrl + uri, true);
        xhr.setRequestHeader('osvc-crest-application-context', 'Insights');
        xhr.setRequestHeader('Content-Type', 'application/json');
        xhr.setRequestHeader('Accept', 'application/json');
        xhr.setRequestHeader('Authorization', 'Session ' + (await this.refreshToken()));

        xhr.onreadystatechange = function () {
            if (this.readyState == 4) {
                if (this.status == 200 || this.status == 201) {
                    extensionPromise.resolve(xhr.response);
                } else {
                    if (this.status == 401) {
                        let getInsight = new GetInsights();
                        getInsight.refreshToken();
                        getInsight.makeRequest(method, uri, payload);
                    }

                    extensionPromise.reject(new (<any>ORACLE_SERVICE_CLOUD).ErrorData(xhr.responseText));
                }
            }
        };

        if (method == 'PATCH' || method == 'POST') {
            xhr.send(JSON.stringify(payload, this.getCircularReplacer()))
        } else {
            xhr.send();
        }
        return extensionPromise;
    }

    public async logPredictionData(payload) {
        payload.MinConfidenceScoreProduct = String(this.prodMinConfidenceScore.toFixed(2));
        payload.MinConfidenceScoreDisposition = String(this.dispMinConfideceScore.toFixed(2));
        payload.MinConfidenceScoreCategory = String(this.catMinConfidenceScore.toFixed(2));

        payload.Source = { "id": this.INSIGHT_TEXT_PREDICT };
        this.makeRequest('POST', 'Prediction.PredictionLog', payload);
    }

    public async getFieldValues(fieldNameArray: Array<string>): Promise<IFieldDetails> {
        return await (await this.getWorkspaceRecord()).getFieldValues(fieldNameArray);
    }

    private async getWorkspaceRecord(): Promise<IIncidentWorkspaceRecord> {
        let extensionProvide: ORACLE_SERVICE_CLOUD.IExtensionProvider = await this.getExtensionProvider();
        let workspaceRecordPromise: IExtensionPromise<IIncidentWorkspaceRecord> = new ExtensionPromise();
        extensionProvide.registerWorkspaceExtension(function (workspaceRecord: IIncidentWorkspaceRecord) {
            workspaceRecordPromise.resolve(workspaceRecord);
        });

        return await workspaceRecordPromise;
    }

}


new GetInsights().initialize();