/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set 
 *  published by Oracle under the Universal Permissive License (UPL), Version 1.0
 *  Copyright (c) 2017 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: Telephony and SMS Accelerator for Twilio
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 17D (November 2017)
 *  reference: 161212-000059
 *  date: Monday Oct 30 13:4:52 UTC 2017
 *  revision: rnw-17-11-fixes-releases
 * 
 *  SHA1: $Id: a060964cac1a986e178e377ec48d8bd1d7bf4480 $
 * *********************************************************************************************
 *  File: osvcExtension.d.ts
 * ****************************************************************************************** */

declare module ORACLE_SERVICE_CLOUD {
  var extension_loader: IExtensionLoader;

  interface IExtensionLoader extends IExtensionDisposable {
    load(appId: string, version?: string, userScriptList?: Array<string>): IExtensionPromise<IExtensionProvider>;
  }

  interface IExtensionDisposable {
    dispose: () => void;
    disposeChild: (disposalKey: string) => void;
    getDisposalKey: () => string;
  }

  interface IFieldObject {
    label: string;
    value: string;
  }

  export interface ICurrency {
    id?: number;
    symbol?: string;
    value: number;
    abbreviation: string;
    decimalPrecision?: string;
    dataType: string;
  }

  export interface IEvent {
    event: string;
    field?: string;
    value?: string;
    fields?: { [s: string]: any; };
    fieldObjects?: { [s: string]: IFieldObject; };
  }

  export interface IErrorData {
    getDesc(): string;
  }

  export interface IEventHandler {
    cancel(): void;
    isCancelled(): boolean;
  }

  export interface IWorkspaceRecordEventParameter {
    event: IEvent;
    getWorkspaceRecord(): IWorkspaceRecord;
    getCurrentEvent(): IEventHandler;
    newWorkspace?: IObjectDetail;
    oldWorkspace?: IObjectDetail;
    getField(fieldName: string): IFieldData;
  }

  export interface IWorkspaceOperationParameter {
    event: string;
    objectId: number;
    objectType: string;
  }

  export interface IBrowserControl {
    getId: () => string;
    getUrl: () => string;
    setUrl: (url: string) => void;
  }

  export interface IReportDefinition {
    getAcId: () => number;
    getName: () => string;
    getRowLimit: () => number;
    getColumnDefinitions: () => IReportColumnDefinition[];
  }
  export interface IReportColumnDefinition {
    getColumnReference: () => string;
    getSortDirection: () => string;
    getSortOrder: () => number;
    getColumnName: () => string;
    setSortOrder: (sortOrder: number) => void;
    setSortDirection: (sortDirection: string) => void;
  }
  export interface IReportColumnDetails {
    header: string;
    columnReference: string;
    sortOrder: number;
    sortDirection: string;
  }
  export interface IAnalyticsFilter {
    getFilterId: () => number;
    getDataType: () => any;
    getOperatorType: () => any;
    getValue: () => any;
    setValue: (value: any) => void;
  }

  export interface IAnalyticsFilterDetails {
    filterId: number;
    dataType: any;
    operatorType: any;
    value: any;
  }

  export interface IExtensionReport {
    getReportDefinition: () => IReportDefinition;
    getReportFilters: () => IExtensionFilterDetails;
    getReportData: () => IReportData;
    getRelatedEntities: () => IReportRelatedEntityDetails[];
    getReportWorkspaceContext: () => IReportWorkspaceContextDetails;
    getRelatedEntity(entityType: string): IReportRelatedEntityDetails;
    createReportData: () => IReportData;
    createReportDataRow: () => IReportDataRow;
    createReportDataCell: () => IReportDataCell;
    setDataHandler(dataHandler: (param: any) => void): void;
    executeReport: () => IExtensionPromise<any>;
  }
  export interface IExtensionFilterDetails {
    getFilterList: () => IExtensionFilter[];
    getRowsPerPage: () => number;
    setRowsPerPage: (rowsPerPage: number) => void;
    getPageNumber: () => number;
    setPageNumber: (pageNumber: number) => void;
  }
  export interface IExtensionFilterDetailParams {
    filterList: IExtensionFilterParams[];
    rowsPerPage: number;
    pageNumber: number;
  }
  export interface IReportRelatedEntity {
    entityType: string;
    relatedFields: IReportRelatedField[];
  }
  export interface IReportRelatedField {
    fieldName: string;
    fieldValue: any;
  }
  export interface IReportRelatedEntityDetails {
    getEntityType: () => string;
    getRelatedFieldValues: () => IReportRelatedFieldDetails[];
  }
  export interface IReportRelatedFieldDetails {
    getFieldName: () => string;
    getFieldValue: () => any;
  }
  export interface IReportWorkspaceContext {
    objectType: string;
    objectId: number;
  }

  export interface IReportWorkspaceContextDetails {
    getObjectType(): string;
    getObjectId(): number;
  }
  export interface IExtensionFilterParams {
    filterId: number;
    dataType: any;
    operatorType: string;
    value: any;
    displayName: string;
    filterType: string;
    appliesTo: string;
    isHierFlat: boolean;
    columnReference: string;
  }

  export interface IExtensionFilter {
    getFilterId: () => number;
    getDataType: () => any;
    getOperatorType: () => any;
    getValue: () => any;
    setValue: (value: any) => void;
    getFilterType: () => string;
    getAppliesTo: () => string;
    getPrompt: () => string;
    getHierFlat: () => boolean;
    getColumnReference: () => string;
  }

  export interface IObjectDetail {
    objectId: number;
    objectType: string;
    contextId: string;
  }

  export interface ITabChangeEventParameter {
    newWorkspace?: IObjectDetail;
    oldWorkspace?: IObjectDetail;
  }

  export interface IWorkspaceRecord {
    getWorkspaceRecordType(): string;
    getWorkspaceRecordId(): number;
    getSubscriptionPriority(): number;
    getCurrentWorkspace(): IObjectDetail;
    closeEditor(): IExtensionPromise<ORACLE_SERVICE_CLOUD.IWorkspaceOperationParameter>;
    editWorkspaceRecord(workspaceType: string, objectId: number, callbackFunctionReference?: (param: IWorkspaceRecord) => void): IExtensionPromise<IWorkspaceRecord>;
    createWorkspaceRecord(objectType: string, callbackFunctionReference?: (param: IWorkspaceRecord) => void): IExtensionPromise<IWorkspaceRecord>;
    deleteWorkspaceRecord(objectType: string, objectId: number, callbackFunctionReference?: (param: IWorkspaceOperationParameter) => void): IExtensionPromise<IWorkspaceOperationParameter>;
    executeEditorCommand(command: string, callbackFunctionReference?: (param: ORACLE_SERVICE_CLOUD.IWorkspaceRecord) => void): IExtensionPromise<IWorkspaceRecord>;
    isEditorCommandAvailable(command: string): boolean;
    getAllBrowserControls(): ORACLE_SERVICE_CLOUD.IBrowserControl[];
    addFieldValueListener(fieldName: string, functionRef: (param: IWorkspaceRecordEventParameter) => void, context?: any): IWorkspaceRecord;
    addEditorLoadedListener(callbackFunctionReference: (param: IWorkspaceRecordEventParameter) => void, context?: any): ORACLE_SERVICE_CLOUD.IWorkspaceRecord;
    addDataLoadedListener(callbackFunctionReference: (param: IWorkspaceRecordEventParameter) => void, context?: any): ORACLE_SERVICE_CLOUD.IWorkspaceRecord;
    addExtensionLoadedListener(callbackFunctionReference: (param: IWorkspaceRecordEventParameter) => void, context?: any): ORACLE_SERVICE_CLOUD.IWorkspaceRecord;
    addRecordSavingListener(callbackFunctionReference: (param: IWorkspaceRecordEventParameter) => void, context?: any): ORACLE_SERVICE_CLOUD.IWorkspaceRecord;
    addRecordSavedListener(callbackFunctionReference: (param: IWorkspaceRecordEventParameter) => void, context?: any): ORACLE_SERVICE_CLOUD.IWorkspaceRecord;
    addRecordClosingListener(callbackFunctionReference: (param: IWorkspaceRecordEventParameter) => void, context?: any): ORACLE_SERVICE_CLOUD.IWorkspaceRecord;
    addCurrentEditorTabChangedListener(callbackFunctionReference: (param: IWorkspaceRecordEventParameter) => void, context?: any): ORACLE_SERVICE_CLOUD.IWorkspaceRecord;
    setFieldHidden(fieldName: string): void;
    setFieldVisible(fieldName: string): void;
    setFieldReadOnly(fieldName: string): void;
    setFieldEditable(fieldName: string): void;
    setFieldRequired(fieldName: string): void;
    setFieldOptional(fieldName: string): void;
    setAppendedValue(fieldName: string, value: string): void;
    setPrependedValue(fieldName: string, value: string): void;
    updateField(fieldName: string, value: string): IExtensionPromise<IWorkspaceRecord>;
    updateFieldByLabel(fieldName: string, value: string): IExtensionPromise<IWorkspaceRecord>;
    includeMenuItems(fieldName: string, menuItems: any[]): IExtensionPromise<IWorkspaceRecord>;
    includeAllMenuItems(fieldName: string): IExtensionPromise<IWorkspaceRecord>;
    excludeMenuItems(fieldName: string, menuItems: any[]): IExtensionPromise<IWorkspaceRecord>;
    findAndFocus(workspaceType: string, workspaceRecordId: number, callbackFunctionReference?: (param: ORACLE_SERVICE_CLOUD.IWorkspaceRecord) => void): boolean;
    isEditorOpen(workspaceType: string, workspaceRecordId: number): boolean;
    addNamedEventListener(eventName: string, callbackFunctionReference: (param: ORACLE_SERVICE_CLOUD.IWorkspaceRecordEventParameter) => void, context?: any): ORACLE_SERVICE_CLOUD.IWorkspaceRecord;
    triggerNamedEvent(eventName: string): void;
    prefetchWorkspaceFields(fieldNameArr: string[]): ORACLE_SERVICE_CLOUD.IWorkspaceRecord;
    dispose(): void;
    getContextId(): string;
    getCurrentAttachmentContext(): IExtensionPromise<IAttachmentContext>;
  }

  export interface IExtensionProvider extends IExtensionDisposable {
    registerWorkspaceExtension(userFunction: (param: IWorkspaceRecord) => void, objectType?: string, objectId?: number): void;
    registerAnalyticsExtension?(userFunction: (param: ORACLE_SERVICE_CLOUD.IAnalyticsContext) => void, objectId?: number, objectType?: string): void;
    getGlobalContext?(): ORACLE_SERVICE_CLOUD.IExtensionPromise<IExtensionGlobalContext>;
    changeSubscriptionPriority(priority: number): void;
    getSubscriptionPriority(): number;
    registerUserInterfaceExtension(userFunction: (param: IUserInterfaceContext) => void): void;
  }

  export interface IAnalyticsContext {
    createReport(reportId: number): IExtensionPromise<IExtensionReport>;
    addTableDataRequestListener(tableName: string, callback: (param: IExtensionReport) => any): IAnalyticsContext;
  }
  export interface IAttachment {
    getName(): string;
    getFileId(): string;
    getType(): string;
    getSize(): string;
  }

  export interface IAttachmentContext {
    getAttachments(): IExtensionPromise<IAttachment[]>;
    getAttachmentsFrom(entityType: string, entityId: string): IExtensionPromise<IAttachment[]>;
    displayAttachmentDialog(): IExtensionPromise<IWorkspaceRecord>;
    uploadAttachment(attachment: IAttachment): IExtensionPromise<IWorkspaceRecord>;
    attachFromUrl(fileName: string, url: string): IExtensionPromise<IWorkspaceRecord>;
  }

  export interface IExtensionPromise<T> {
    then(onCompleted: (sdk: T) => void, onRejected?: (param: ORACLE_SERVICE_CLOUD.IErrorData) => void): ORACLE_SERVICE_CLOUD.IExtensionPromise<T>;
    catch(onRejected: (param: ORACLE_SERVICE_CLOUD.IErrorData) => void): void;
  }

  export interface IFieldData {
    getLabel(): string;
    getValue(): any;
  }
  export interface IIncidentWorkspaceRecord extends IWorkspaceRecord {
    getCurrentEditedThread(entryType: string, isNew: boolean): IExtensionPromise<IThreadEntry>;
    getThreadEntryTypes(): string[];
  }

  export interface IThreadEntry {
    getThreadId(): string;
    getContent(): string;
    getEntryType(): string;
    getChannelType(): string;
    isDraft(): boolean;
    setContent(content: string): IExtensionPromise<any>;
    delete(): IExtensionPromise<any>;
    addFieldValueListener(fieldName: string, callback: (param: IWorkspaceRecordEventParameter) => void, callbackContext?: any): IThreadEntry;
  }

  export interface INoteEntry {
    getChannelId(): number;
    getCreated(): Date;
    getCreatedBy(): number;
    getContent(): string;
    getNoteId(): string;
    getSeq(): string;
    getUpdated(): Date;
    getUpdatedBy(): number;
  }

  export interface IMenuErrorData extends IErrorData {
    getRejectedIDs(): number[];
  }

  export interface IUserInterfaceContext {
    getContentPaneContext(): IExtensionPromise<IContentPaneContext>;
    getGlobalHeaderContext(): IExtensionPromise<IGlobalHeaderContext>;
    getLeftSidePaneContext(): IExtensionPromise<ISidePaneContext>;
    getRightSidePaneContext(): IExtensionPromise<ISidePaneContext>;
    getStatusBarContext(): IExtensionPromise<IStatusBarContext>;
    getNavigationSetContext(): IExtensionPromise<INavigationSetContext>;
    getModalWindowContext(): IExtensionPromise<IModalWindowContext>;
    getPopupWindowContext(): IExtensionPromise<IPopupWindowContext>;
    getExtensionBarContext(): IExtensionPromise<IExtensionBarContext>;
  }

  export interface ISidePaneContext {
    getSidePane(id: string): IExtensionPromise<ISidePane>;
  }

  export interface IContentPaneContext {
    createContentPane(): IExtensionPromise<IContentPane>;
  }

  export interface IStatusBarContext {
    getStatusBarItem(id: string): IExtensionPromise<IStatusBarItem>;
  }

  export interface INavigationSetContext {
    getNavigationItem(id: string): IExtensionPromise<INavigationItem>;
    getNavigationList(): IExtensionPromise<INavigationItem[]>;
  }

  export interface IGlobalHeaderContext {
    getMenu(id: string): IExtensionPromise<IGlobalHeaderMenu>;
  }

  export interface IGlobalHeaderMenuItem extends IExtensionDisposable {
    getId(): string;
    getLabel(): string;
    setLabel(label: string): void;
    setHandler(callback: (globalHeaderMenuItem: IGlobalHeaderMenuItem) => void): void;
    getHandler(): (globalHeaderMenuItem: IGlobalHeaderMenuItem) => void;
  }

  export interface IGlobalHeaderMenu extends IExtensionDisposable {
    getId(): string;
    getLabel(): string;
    setLabel(label: string): void;
    isDisabled(): boolean;
    setDisabled(disabled: boolean): void;
    addMenuItem(menuItem: IGlobalHeaderMenuItem): void;
    createMenuItem(): IGlobalHeaderMenuItem;
    render(): void;
    createIcon(type: string): IICon;
    addIcon(icon: IICon): void;
  }
  export interface IContentPane extends IWorkspaceRecord {
    setContentUrl(url: string): void;
    setName(name: string): void;
    getName(): string;
    getContentUrl(): string;
  }

  export interface ISidePane {
    getId(): string;
    getLabel(): string;
    setLabel(label: string): void;
    isDisabled(): boolean;
    setDisabled(disabled: boolean): void;
    getContentUrl(): string;
    setContentUrl(contentUrl: string): void;
    isExpanded(): boolean;
    expand(): void;
    collapse(): void;
    render(): void;
    createIcon(type: string): IICon;
    addIcon(icon: IICon): void;
    setVisible(visible: boolean): void;
  }

  export interface IStatusBarItem {
    getId(): string;
    getLabel(): string;
    setLabel(label: string): void;
    isVisible(): boolean;
    setVisible(disabled: boolean): void;
    getContentUrl(): string;
    setContentUrl(contentUrl: string): void;
    getWidth(): string;
    setWidth(width: string): void;
    render(): void;
  }

  export interface IStandardTextItem {
    getId(): number;
    getName(): string;
    getContent(): IExtensionPromise<string>;
  }
  export interface INavigationItem extends IExtensionDisposable {
    getId(): string;
    getLabel(): string;
    createChildItem(): INavigationItem;
    addChildItem(child: INavigationItem): void;
    getChildren(): IExtensionPromise<INavigationItem[]>;
    setLabel(text: string): void;
    setHandler(handler: (data: INavigationItem) => void): void;
    render(): void;
  }
  export interface IReportDataCell {
    data: any;
    getData(): any;
    setData(data: any): void;
  }
  export interface IReportDataRow {
    cells: IReportDataCell[];
    getCells(): IReportDataCell[];
  }
  export interface IReportData {
    rows: IReportDataRow[];
    getTotalRecordCount(): number;
    setTotalRecordCount(count: number): void;
    getRows(): IReportDataRow[];
  }

  export interface IExtensionGlobalContext {
    getProfileId(): number;
    getProfileName(): string;
    getInterfaceId(): number;
    getInterfaceName(): string;
    getInterfaceUrl(): string;
    getAccountId(): number;
    getLanguageId(): number;
    getLanguage(): string;
    getInterfaceServiceUrl(connectServiceType: string): string;
    getLogin(): string;
    getSessionToken(): IExtensionPromise<String>;
    getStandardTextItemById(id: number): IExtensionPromise<IStandardTextItem>;
    getStandardTextItemByName(name: string): IExtensionPromise<IStandardTextItem[]>;
    getStandardTextList(startIndex?: number, limit?: number): IExtensionPromise<IStandardTextItem[]>;
  }
  export interface IModalWindowContext {
    createModalWindow(): IModalWindow;
    getCurrentModalWindow(): IExtensionPromise<IModalWindow>;

  }
  export interface IModalWindow {
    getTitle(): string;
    setTitle(title: string): void;
    setContentUrl(url: string): void;
    setWidth(width: string): void;
    setHeight(height: string): void;
    setClosable(isClosable: boolean): void;
    render(): IExtensionPromise<IModalWindow>;
    close(): IExtensionPromise<any>;
  }
  export interface IPopupWindowContext {
    createPopupWindow(id: string): IPopupWindow;
    getCurrentPopupWindows(): IExtensionPromise<IPopupWindow[]>;

  }
  export interface IPopupWindow {
    setContentUrl(url: string): void;
    setWidth(width: string): void;
    setHeight(height: string): void;
    setClosable(isClosable: boolean): void;
    setTitle(title: string): void;
    render(): IExtensionPromise<IPopupWindow>;
    close(): IExtensionPromise<any>;
  }
  export interface IExtensionBarItem {
    setContentUrl(url: string): void;
    setWidth(width: number): void;
    setHeight(height: number): void;
    getId(): string;
    getContentUrl(): string;
    getWidth(): number;
    getHeight(): number;
    render(): void;
  }

  export interface IExtensionBarContext {
    getExtensionBarItem(id: string): IExtensionPromise<IExtensionBarItem>;
    getAllExtensionBarItems(): IExtensionPromise<IExtensionBarItem[]>;
    getDefaultDockingPosition(): string;
    getDockingPosition(): string;
    isDockable(): boolean;
    setDockable(dockable: boolean): void;
    setDockingPosition(dockingPosition: string): void;
    setDefaultDockingPosition(dockingPosition: string): void;
    render(): void;
  }

  export interface IICon {
    setIconClass(className: string): void;
    setIconColor(color: string): void
  }
}

declare type IExtensionLoader = ORACLE_SERVICE_CLOUD.IExtensionLoader;
declare type IExtensionDisposable = ORACLE_SERVICE_CLOUD.IExtensionDisposable;
declare type IFieldObject = ORACLE_SERVICE_CLOUD.IFieldObject;
declare type IEvent = ORACLE_SERVICE_CLOUD.IEvent;
declare type IErrorData = ORACLE_SERVICE_CLOUD.IErrorData;
declare type IEventHandler = ORACLE_SERVICE_CLOUD.IEventHandler;
declare type IWorkspaceRecordEventParameter = ORACLE_SERVICE_CLOUD.IWorkspaceRecordEventParameter;
declare type IWorkspaceOperationParameter = ORACLE_SERVICE_CLOUD.IWorkspaceOperationParameter;
declare type IBrowserControl = ORACLE_SERVICE_CLOUD.IBrowserControl;
declare type IReportDefinition = ORACLE_SERVICE_CLOUD.IReportDefinition;
declare type IReportColumnDefinition = ORACLE_SERVICE_CLOUD.IReportColumnDefinition;
declare type IReportColumnDetails = ORACLE_SERVICE_CLOUD.IReportColumnDetails;
declare type IAnalyticsFilter = ORACLE_SERVICE_CLOUD.IAnalyticsFilter;
declare type IAnalyticsFilterDetails = ORACLE_SERVICE_CLOUD.IAnalyticsFilterDetails;
declare type IExtensionReport = ORACLE_SERVICE_CLOUD.IExtensionReport;
declare type IExtensionFilterDetails = ORACLE_SERVICE_CLOUD.IExtensionFilterDetails;
declare type IExtensionFilterDetailParams = ORACLE_SERVICE_CLOUD.IExtensionFilterDetailParams;
declare type IReportRelatedEntity = ORACLE_SERVICE_CLOUD.IReportRelatedEntity;
declare type IReportRelatedField = ORACLE_SERVICE_CLOUD.IReportRelatedField;
declare type IReportRelatedEntityDetails = ORACLE_SERVICE_CLOUD.IReportRelatedEntityDetails;
declare type IReportRelatedFieldDetails = ORACLE_SERVICE_CLOUD.IReportRelatedFieldDetails;
declare type IReportWorkspaceContext = ORACLE_SERVICE_CLOUD.IReportWorkspaceContext;
declare type IReportWorkspaceContextDetails = ORACLE_SERVICE_CLOUD.IReportWorkspaceContextDetails;
declare type IExtensionFilterParams = ORACLE_SERVICE_CLOUD.IExtensionFilterParams;
declare type IExtensionFilter = ORACLE_SERVICE_CLOUD.IExtensionFilter;
declare type IObjectDetail = ORACLE_SERVICE_CLOUD.IObjectDetail;
declare type ITabChangeEventParameter = ORACLE_SERVICE_CLOUD.ITabChangeEventParameter;
declare type IWorkspaceRecord = ORACLE_SERVICE_CLOUD.IWorkspaceRecord;
declare type IExtensionProvider = ORACLE_SERVICE_CLOUD.IExtensionProvider;
declare type IAnalyticsContext = ORACLE_SERVICE_CLOUD.IAnalyticsContext;
declare type IExtensionPromise<T> = ORACLE_SERVICE_CLOUD.IExtensionPromise<T>;
declare type IFieldData = ORACLE_SERVICE_CLOUD.IFieldData;
declare type IIncidentWorkspaceRecord = ORACLE_SERVICE_CLOUD.IIncidentWorkspaceRecord;
declare type IThreadEntry = ORACLE_SERVICE_CLOUD.IThreadEntry;
declare type INoteEntry = ORACLE_SERVICE_CLOUD.INoteEntry;
declare type IMenuErrorData = ORACLE_SERVICE_CLOUD.IMenuErrorData;
declare type IUserInterfaceContext = ORACLE_SERVICE_CLOUD.IUserInterfaceContext;
declare type ISidePaneContext = ORACLE_SERVICE_CLOUD.ISidePaneContext;
declare type IContentPaneContext = ORACLE_SERVICE_CLOUD.IContentPaneContext;
declare type IStatusBarContext = ORACLE_SERVICE_CLOUD.IStatusBarContext;
declare type INavigationSetContext = ORACLE_SERVICE_CLOUD.INavigationSetContext;
declare type IGlobalHeaderContext = ORACLE_SERVICE_CLOUD.IGlobalHeaderContext;
declare type IGlobalHeaderMenuItem = ORACLE_SERVICE_CLOUD.IGlobalHeaderMenuItem;
declare type IGlobalHeaderMenu = ORACLE_SERVICE_CLOUD.IGlobalHeaderMenu;
declare type IContentPane = ORACLE_SERVICE_CLOUD.IContentPane;
declare type ISidePane = ORACLE_SERVICE_CLOUD.ISidePane;
declare type IStatusBarItem = ORACLE_SERVICE_CLOUD.IStatusBarItem;
declare type INavigationItem = ORACLE_SERVICE_CLOUD.INavigationItem;
declare type IReportDataCell = ORACLE_SERVICE_CLOUD.IReportDataCell;
declare type IReportDataRow = ORACLE_SERVICE_CLOUD.IReportDataRow;
declare type IReportData = ORACLE_SERVICE_CLOUD.IReportData;
declare type IExtensionGlobalContext = ORACLE_SERVICE_CLOUD.IExtensionGlobalContext;
declare type IModalWindowContext = ORACLE_SERVICE_CLOUD.IModalWindowContext;
declare type IModalWindow = ORACLE_SERVICE_CLOUD.IModalWindow;
declare type IPopupWindowContext = ORACLE_SERVICE_CLOUD.IPopupWindowContext;
declare type IPopupWindow = ORACLE_SERVICE_CLOUD.IPopupWindow;
declare type IExtensionBarItem = ORACLE_SERVICE_CLOUD.IExtensionBarItem;
declare type IExtensionBarContext = ORACLE_SERVICE_CLOUD.IExtensionBarContext;
declare type IStandardTextItem = ORACLE_SERVICE_CLOUD.IStandardTextItem;

declare var ExtensionPromise: { new(handler: ((resolve: any, reject: any) => void)): IExtensionPromise<any>; };

