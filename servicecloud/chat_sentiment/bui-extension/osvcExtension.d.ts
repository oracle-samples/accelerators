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
 *  SHA1: $Id: 647d102fec85bb655942ef0dd2355b22ac3351d6 $
 * *********************************************************************************************
 *  File: osvcExtension.d.ts
 * ****************************************************************************************** */
declare module ORACLE_SERVICE_CLOUD {
  var extension_loader: IExtensionLoader;
  var extensionLoadPromise: IExtensionPromise<any>;

  interface IExtensionLoader extends IExtensionDisposable {
    load(appId: string, version?: string, userScriptList?: string[]): IExtensionPromise<IExtensionProvider>;
  }

  interface IExtensionDisposable {
    dispose: () => void;
    disposeChild: (disposalKey: string) => void;
    getDisposalKey: () => string;
  }

  interface IFieldObject {
    label: string;
    value: any;
  }

  export interface ICurrency {
    id?: number;
    symbol?: string;
    value: number;
    abbreviation: string;
    decimalPrecision?: number;
    dataType: string;
  }

  export interface IStandardText {
    event: IEvent;
    standardText: string;
  }

  export interface IGlobalActionResult {
    result: any[];
  }

  export interface IStandardTextFocusChange {
    event: IEvent;
    newFocusId: string;
    focusChanged: boolean;
  }

  export interface IEvent {
    event: string;
    field?: string;
    value?: any;
    oldValue?: string;
    fields?: { [s: string]: any };
    fieldObjects?: { [s: string]: IFieldObject };
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

  export interface ISubscriptionResult {
    result: any[];
  }
  export interface IWorkspaceOperationParameter {
    event: string;
    objectId: number;
    objectType: string;
  }

  export interface IBrowserControl {
    getId: () => string;
    getUrl: () => string;
    setUrl: (url: string, useGETMethod?: boolean) => void;
  }

  export interface IReportDefinition {
    getAcId: () => number;
    getName: () => string;
    getRowLimit: () => number;
    getDisplayOptions: () => IReportDisplayDefinition;
    setDisplayOptions: (reportDisplayOptions: IReportDisplayDefinition) => void;
    getColumnDefinitions: () => IReportColumnDefinition[];
  }
  export interface IReportColumnDefinition {
    getColumnReference: () => string;
    getSortDirection: () => string;
    getSortOrder: () => number;
    getColumnName: () => string;
    setSortOrder: (sortOrder: number) => void;
    setSortDirection: (sortDirection: string) => void;
    getColumnOrder: () => number;
    getDisplayOptions: () => IReportColumnDisplayDefinition;
    setDisplayOptions: (columnDisplayoptions: IReportColumnDisplayDefinition) => void;
  }
  export interface IReportDisplayDefinition {
    showColumnsInMultipleLine(showAllColumnsMultLine: boolean): void;
    canShowColumnsInMultipleLine(): boolean;
    hideColumnHeaders(hideColumnHeaders: boolean): void;
    canHideColumnHeaders(): boolean;
    hideReportCommands(hideReportCommands: boolean): void;
    canHideReportCommands(): boolean;
    hideReportToolBar(hideReportToolBar: boolean): void;
    canHideReportToolBar(): boolean;
  }
  export interface IReportColumnDisplayDefinition {
    canWrapData(): boolean;
    wrapData(wrapData: boolean): void;
    canDisplayMoreLink(): boolean;
    displayMoreLink(displayMoreLink: boolean): void;
  }
  export interface IReportColumnDetails {
    header: string;
    columnReference: string;
    sortOrder: number;
    sortDirection: string;
    dataOrder: number;
    formatOptions: string[];
  }
  export interface IExtensionFilter {
    getFilterId: () => number;
    getDataType: () => any;
    getOperatorType: () => any;
    getFilterType: () => string;
    getAppliesTo: () => string;
    getPrompt: () => string;
    getHierFlat: () => boolean;
    getColumnReference: () => string;
    setValue: (value: any) => void;
    getValue: () => any;
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
    createReportRecordInfo: () => IReportRecordInfo;
    setDataHandler(dataHandler: (param: any) => void): void;
    executeReport: (configuration?: IReportConfiguration) => void;
    getExtensionReportId(): string;
    createReportConfiguration(): IReportConfiguration;
    getReportExecutionContext(): IReportExecutionContext;
    addSearchReportValueSetListener(
      callbackFunction: (param: IRecordSelectionContext) => number | IExtensionPromise<number>
    ): IExtensionReport;
  }

  export interface IRecordSelectionContext {
    getSearchContext(): ISearchReportContext;
    getSelectedRow(): IReportRow;
  }

  export interface IEditedReportDataContext {
    getExecutionContext(): IUserInterface;
    getEditedRows(): IReportRow[];
    getTableName(): string;
    getReportId(): number;
  }

  export interface IExtensionFilterDetails {
    getFilterList: () => IExtensionFilter[];
    getRowsPerPage: () => number;
    setRowsPerPage: (rowsPerPage: number) => void;
    getPageNumber: () => number;
    setPageNumber: (pageNumber: number) => void;
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

  export interface IExtensionRangeFilter extends IExtensionFilter {
    getFilterStartValue: () => any;
    setFilterStartValue: (startValue: any) => void;
    getFilterEndValue: () => any;
    setFilterEndValue: (endValue: any) => void;
  }

  export interface IObjectDetail {
    objectId: number;
    objectType: string;
    contextId: string;
  }

  export interface IReportConfiguration {
    setUserInterface: (userInterface: IUserInterface) => void;
    setTitle: (title: string) => void;
    setWidth: (width: string) => void;
    setHeight: (height: string) => void;
  }

  export interface ITabChangeEventParameter {
    newWorkspace?: IObjectDetail;
    oldWorkspace?: IObjectDetail;
  }

  export interface IWorkspaceRecord extends IUserInterface {
    addNote(): IExtensionPromise<INoteEntry>;
    getWorkspaceRecordType(): string;
    getWorkspaceRecordId(): number;
    getSubscriptionPriority(): number;
    getCurrentWorkspace(): IObjectDetail;
    closeEditor(): IExtensionPromise<ORACLE_SERVICE_CLOUD.IWorkspaceOperationParameter>;
    editWorkspaceRecord(
      workspaceType: string,
      objectId: number,
      callbackFunctionReference?: (param: IWorkspaceRecord) => void
    ): IExtensionPromise<IWorkspaceRecord>;
    createWorkspaceRecord(
      objectType: string,
      callbackFunctionReference?: (param: IWorkspaceRecord) => void
    ): IExtensionPromise<IWorkspaceRecord>;
    deleteWorkspaceRecord(
      objectType: string,
      objectId: number,
      callbackFunctionReference?: (param: IWorkspaceOperationParameter) => void
    ): IExtensionPromise<IWorkspaceOperationParameter>;
    executeEditorCommand(
      command: string,
      callbackFunctionReference?: (param: ORACLE_SERVICE_CLOUD.IWorkspaceRecord) => void
    ): IExtensionPromise<IWorkspaceRecord>;
    isEditorCommandAvailable(command: string): boolean;
    getAllBrowserControls(): ORACLE_SERVICE_CLOUD.IBrowserControl[];
    addFieldValueListener(
      fieldName: string,
      functionRef: (param: IWorkspaceRecordEventParameter) => void,
      context?: any
    ): IWorkspaceRecord;
    addEditorLoadedListener(
      callbackFunctionReference: (param: IWorkspaceRecordEventParameter) => void,
      context?: any
    ): ORACLE_SERVICE_CLOUD.IWorkspaceRecord;
    addDataLoadedListener(
      callbackFunctionReference: (param: IWorkspaceRecordEventParameter) => void,
      context?: any
    ): ORACLE_SERVICE_CLOUD.IWorkspaceRecord;
    addExtensionLoadedListener(
      callbackFunctionReference: (param: IWorkspaceRecordEventParameter) => void,
      context?: any
    ): ORACLE_SERVICE_CLOUD.IWorkspaceRecord;
    addRecordSavingListener(
      callbackFunctionReference: (param: IWorkspaceRecordEventParameter) => void,
      context?: any
    ): ORACLE_SERVICE_CLOUD.IWorkspaceRecord;
    addRecordSavedListener(
      callbackFunctionReference: (param: IWorkspaceRecordEventParameter) => void,
      context?: any
    ): ORACLE_SERVICE_CLOUD.IWorkspaceRecord;
    addRecordClosingListener(
      callbackFunctionReference: (param: IWorkspaceRecordEventParameter) => void,
      context?: any
    ): ORACLE_SERVICE_CLOUD.IWorkspaceRecord;
    addCurrentEditorTabChangedListener(
      callbackFunctionReference: (param: IWorkspaceRecordEventParameter) => void,
      context?: any
    ): ORACLE_SERVICE_CLOUD.IWorkspaceRecord;
    addRecordAcceptingListener(
      callbackFunctionReference: (param: IWorkspaceRecordEventParameter) => void,
      context?: any
    ): ORACLE_SERVICE_CLOUD.IWorkspaceRecord;
    addRecordRejectingListener(
      callbackFunctionReference: (param: IWorkspaceRecordEventParameter) => void,
      context?: any
    ): ORACLE_SERVICE_CLOUD.IWorkspaceRecord;
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
    findAndFocus(
      workspaceType: string,
      workspaceRecordId: number,
      callbackFunctionReference?: (param: ORACLE_SERVICE_CLOUD.IWorkspaceRecord) => void
    ): boolean;
    isEditorOpen(workspaceType: string, workspaceRecordId: number): boolean;
    addNamedEventListener(
      eventName: string,
      callbackFunctionReference: (param: ORACLE_SERVICE_CLOUD.IWorkspaceRecordEventParameter) => void,
      context?: any
    ): ORACLE_SERVICE_CLOUD.IWorkspaceRecord;
    triggerNamedEvent(eventName: string): IExtensionPromise<ISubscriptionResult>;
    prefetchWorkspaceFields(fieldNameArr: string[], isLabelsRequired?: boolean): ORACLE_SERVICE_CLOUD.IWorkspaceRecord;
    getFieldValues(fieldNameArr: string[], isLabelsRequired?: boolean): IExtensionPromise<IFieldDetails>;
    dispose(): void;
    getContextId(): string;
    getCurrentAttachmentContext(): IExtensionPromise<IAttachmentContext>;
    createRecordCommandContext(commandName: string): IRecordCommandContext;
    registerRecordCommand(commandContext: IRecordCommandContext, isInjectImmediately?: boolean): IWorkspaceRecord;
    createIcon(): IIcon;
    getRibbonBarItems(): IExtensionPromise<IRibbonBarItem[]>;
    getAllIntelligentAdvisorControls(): IExtensionPromise<IIntelligentAdvisorControl[]>;
    getAllIntelligentAdvisorControlsByInterview(interviewName: string): IExtensionPromise<IIntelligentAdvisorControl[]>;
    getWorkGroupContext(): IExtensionPromise<IExtensionWorkGroupContext>;
    getEntityType(): string;
    getEntityId(): number;
    isVisible(): IExtensionPromise<boolean>;
    getScriptContext(): IExtensionPromise<IScriptContext>;
  }
  export interface IExtensionWorkGroupContext {
    getWorkGroupEntities(): IExtensionPromise<IWorkGroupEntity[]>;
    getPrimaryWorkGroupEntity(): IExtensionPromise<IWorkGroupEntity>;
    createWorkspaceConfig(): IWorkspaceConfig;
    createWorkGroupEntity(entityType: string, config: IWorkspaceConfig): IExtensionPromise<IWorkGroupEntity>;
    findAndFocus(
      entityType: string,
      entityId: number,
      callbackFunctionReference?: (param: IWorkspaceRecord) => void
    ): boolean;
    editWorkGroupEntity(
      entityType: string,
      entityId: number,
      config: IWorkspaceConfig,
      callbackFunctionReference?: (param: IWorkspaceRecord) => void
    ): IExtensionPromise<IWorkGroupEntity>;
    removeWorkGroupEntity(entityType: string, entityId: number): IExtensionPromise<void>;
  }
  export interface IWorkGroupEntity {
    getEntityType(): string;
    getEntityId(): number;
    isVisible(): IExtensionPromise<boolean>;
    getWorkspaceRecord(): IExtensionPromise<IWorkspaceRecord>;
  }
  export interface IWorkspaceConfig {
    setRenderInUI(renderInUI: boolean): void;
    hasRenderedInUI(): boolean;
    setUnfocused(unfocus: boolean): void;
    isFocused(): boolean;
  }
  export interface IExtensionProvider extends IExtensionDisposable {
    getApplicationId(): string;
    registerWorkspaceExtension(
      userFunction: (param: IWorkspaceRecord) => void,
      objectType?: string,
      objectId?: number
    ): void;
    registerAnalyticsExtension?(
      userFunction: (param: ORACLE_SERVICE_CLOUD.IAnalyticsContext) => void,
      objectId?: number,
      objectType?: string
    ): void;
    getGlobalContext?(): ORACLE_SERVICE_CLOUD.IExtensionPromise<IExtensionGlobalContext>;
    changeSubscriptionPriority(priority: number): void;
    getSubscriptionPriority(): number;
    registerUserInterfaceExtension(userFunction: (param: IUserInterfaceContext) => void): void;
    getLogger(loggerName?: string): IExtensionLogger;
  }

  export interface IExtensionLogger {
    trace(message: string): void;
    debug(message: string): void;
    info(message: string): void;
    warn(message: string): void;
    error(message: string): void;
  }

  export interface IAnalyticsContext {
    createReport(reportId: number): IExtensionPromise<IExtensionReport>;
    addTableDataRequestListener(tableName: string, callback: (param: IExtensionReport) => any): IAnalyticsContext;
    addTableDataModifiedRequestListener(
      tableName: string,
      callback: (param: IEditedReportDataContext) => void
    ): IAnalyticsContext;
    addExternalMenuDataRequestListener(
      columnReference: string,
      callback: (param: IExternalMenuExecutionContext) => IExtensionPromise<IExternalMenuResultSet>
    ): IAnalyticsContext;
    createRecordCommandContext(commandName: string): IRecordCommandContext;
    registerRecordCommand(commandContext: IRecordCommandContext): IAnalyticsContext;
    createIcon(): IIcon;
  }

  export interface IRecordCommandContext {
    setIcon(iconClass: IIcon): void;
    setLabel(label: string): void;
    setTooltip(tooltip: string): void;
    setReportId(reportId: number): void;
    setRecordId(recordId: number): void;
    showAsLink(isLink: boolean): void;
    showLinkAsIcon(showLinkIcon: boolean): void;
    addValidatorCallback(callbackFunction: (param: IExtensionCommandContext[]) => any): void;
    addExecutorCallback(callbackFunction: (param: IExtensionCommandContext[]) => any): void;
    addInjectionValidatorCallback(callbackFunction: (param: IExtensionCommandInfo) => any): void;
  }

  export interface IIcon {
    setIconClass(iconClass: string): void;
    setIconColor(color: string): void;
  }

  export interface IExtensionCommandContext {
    getCommandName(): string;
  }

  export interface IReportRow extends IExtensionCommandContext {
    getRowId(): number;
    getCells(): IReportCell[];
    getEditedCells(): IReportCell[];
    getRecords(): IReportRecord[];
  }

  export interface IWorkspaceCommandContext extends IExtensionCommandContext {
    getWorkspaceRecord(): IWorkspaceRecord;
    getWorkspaceId(): number;
  }

  export interface IReportCell {
    getName(): string;
    getValue(): any;
    getDisplayValue(): string;
    getColumnReference(): string;
    isEdited(): boolean;
    getPreviousValue(): any;
  }

  export interface IExtensionCommandInfo {
    getCommandName(): string;
  }

  export interface IReportInfo extends IExtensionCommandInfo {
    getReportId(): number;
    getExtensionReportId(): string;
  }

  export interface IWorkspaceInfo extends IExtensionCommandInfo {
    getWorkspaceRecord(): IWorkspaceRecord;
    getWorkspaceId(): number;
  }

  export interface IReportRecord {
    getRecordType(): string;
    getRecordId(): number;
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
    then(
      onCompleted: (sdk: T) => void,
      onRejected?: (param: ORACLE_SERVICE_CLOUD.IErrorData) => void
    ): ORACLE_SERVICE_CLOUD.IExtensionPromise<T>;
    catch(onRejected: (param: ORACLE_SERVICE_CLOUD.IErrorData) => void): void;
    resolve(resolveReason: T): IExtensionPromise<T>;
    reject(rejectReason: IErrorData): IExtensionPromise<T>;
  }

  export interface IFieldDetails {
    getField(fieldName: string): IFieldData;
  }

  export interface IFieldData {
    getLabel(): string;
    getValue(): any;
  }
  export interface IIncidentWorkspaceRecord extends IWorkspaceRecord {
    getCurrentEditedThread(entryType: string, isNew: boolean): IExtensionPromise<IThreadEntry>;
  }

  export interface IMailRecipient {
    getType(): string;
  }

  export interface IMailAccount extends IMailRecipient {
    setEmail(email: string): void;
    setLabel(label: string): void;
    setId(id: number): void;
    getEmail(): string;
    getLabel(): string;
    getId(): number;
  }

  export interface IMailGroup extends IMailRecipient {
    setLabel(label: string): void;
    setId(id: number): void;
    getLabel(): string;
    getId(): number;
  }

  export interface IEmailRecipient extends IMailRecipient {
    setEmail(email: string): void;
    getEmail(): string;
  }

  export interface IThreadEntry {
    getThreadId(): string;
    getContent(): string;
    getEntryType(): string;
    getChannelType(): string;
    isDraft(): boolean;
    setContent(content: string): IExtensionPromise<any>;
    setContentType(type: string): void;
    delete(): IExtensionPromise<any>;
    addFieldValueListener(
      fieldName: string,
      callback: (param: IWorkspaceRecordEventParameter) => void,
      callbackContext?: any
    ): IThreadEntry;
  }

  export interface ICustomerResponseThreadEntry extends IThreadEntry {
    getCcRecipients(): IMailRecipient[];
    getBccRecipients(): IMailRecipient[];
    addCcRecipients(ccRecipients: IMailRecipient[]): IExtensionPromise<ICustomerResponseThreadEntry>;
    addBccRecipients(iMailRecipients: IMailRecipient[]): IExtensionPromise<ICustomerResponseThreadEntry>;
    clearCcRecipients(): IExtensionPromise<ICustomerResponseThreadEntry>;
    clearBccRecipients(): IExtensionPromise<ICustomerResponseThreadEntry>;
    createAccountGroupRecipient(): IMailRecipient;
    createAccountRecipient(): IMailRecipient;
    createDistributionListRecipient(): IMailRecipient;
    createMailingAddressRecipient(): IMailRecipient;
    createEmailAddressRecipient(): IMailRecipient;
  }

  export interface IMailErrorData extends IErrorData {
    getResolvedMailRecipients(): IMailRecipient[];
    getRejectedMailRecipients(): IMailRecipient[];
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
    setChannelId(channelId: number): void;
    setContent(content: string): void;
    apply(): IExtensionPromise<INoteEntry>;
    addFieldValueListener(
      fieldName: string,
      callback: (param: IWorkspaceRecordEventParameter) => void,
      callbackContext?: any
    ): INoteEntry;
    delete(): IExtensionPromise<INoteEntry>;
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
    getNotificationContext(): IExtensionPromise<INotificationContext>;
    getStandardTextContext(): IExtensionPromise<IStandardTextContext>;
  }
  export interface IUserInterface {
    getId(): string;
    getUIType(): string;
  }

  export interface ISidePaneContext {
    getSidePane(id: string, groupId?: string): IExtensionPromise<ISidePane>;
    fetchGroupedSidePaneAttributes(groupId: string): IExtensionPromise<IGroupSidePaneAttributes>;
  }

  export interface IStandardTextContext {
    getStandardTextPane(): IExtensionPromise<IStandardTextPane>;
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

  export interface IGlobalHeaderMenuItem {
    getLabel(): string;
    setLabel(label: string): void;
    setHandler(callback: (globalHeaderMenuItem: IGlobalHeaderMenuItem) => void): void;
    getHandler(): (globalHeaderMenuItem: IGlobalHeaderMenuItem) => void;
  }

  export interface IGlobalHeaderMenu extends IUserInterface {
    getLabel(): string;
    setLabel(label: string): void;
    isDisabled(): boolean;
    setDisabled(disabled: boolean): void;
    addMenuItem(menuItem: IGlobalHeaderMenuItem): void;
    createMenuItem(): IGlobalHeaderMenuItem;
    render(): void;
    setHandler(callback: (globalHeaderMenu: IGlobalHeaderMenu) => void): void;
  }
  export interface IContentPane extends IWorkspaceRecord, IUserInterface {
    setContentUrl(url: string, useGETMethod?: boolean): void;
    setName(name: string): void;
    getName(): string;
    getContentUrl(): string;
  }

  export interface ISidePane extends IUserInterface {
    getLabel(): string;
    setLabel(label: string): void;
    isDisabled(): boolean;
    setDisabled(disabled: boolean): void;
    getContentUrl(): string;
    setContentUrl(contentUrl: string): void;
    isExpanded(): IExtensionPromise<boolean>;
    getGroupId(): string;
    expand(): void;
    collapse(): void;
    render(): void;
    activate(): void;
    setResizeEnabled(resizeEnabled: boolean): void;
    dispose(): void;
    setWidth(width: number): void;
  }

  export interface IGroupSidePaneAttributes {
    getGroupId(): string;
    getType(): string;
    isOpen(): boolean;
  }

  export interface IStandardTextDataNode {
    getLabel(): string;
    getChildren(): IStandardTextDataNode[];
    getValue(): number;
    getHotKey(): string;
    isCsrContentType(): boolean;
    isWorkflowContentType(): boolean;
    isLiveContentType(): boolean;
    isLiveUrlContentType(): boolean;
    isFolderType(): boolean;
  }

  export interface IStandardTextPane {
    setFilterHandler(handler: (standardTextDataNode: IStandardTextDataNode[]) => IStandardTextDataNode[]): void;
    getFilterHandler(): (standardTextDataNode: IStandardTextDataNode[]) => IStandardTextDataNode[];
    setSelectionHandler(handler: (param: IStandardText) => void): void;
    getSelectionHandler(): (param: IStandardText) => void;
    setFocusChangedHandler(handler: (param: IStandardTextFocusChange) => void): void;
    getFocusChangedHandler(): (param: IStandardTextFocusChange) => void;
    setLabel(label: string): void;
    getLabel(): string;
    getFocusId(): string;
    setStandardTextContentTypes(contentType: string[]): void;
    getStandardTextContentTypes(): string[];
    render(): IExtensionPromise<any>;
    renderHotKeyDialog(): void;
    dispose(): void;
    disableAddMessage(): void;
    enableAddMessage(): void;
  }

  export interface IStatusBarItem extends IUserInterface {
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

  export interface INavigationItem extends IExtensionDisposable, IUserInterface {
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
    getRecordInfoList(): IReportRecordInfo[];
  }
  export interface IReportData {
    rows: IReportDataRow[];
    getTotalRecordCount(): number;
    setTotalRecordCount(count: number): void;
    getRows(): IReportDataRow[];
  }
  export interface IReportRecordInfo extends IReportRecord {
    setRecordType(recordType: string): void;
    setRecordId(recordId: number): void;
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
    getOptListContext(): IExtensionPromise<IExtensionOptListContext>;
    registerAction(actionName: string, callbackFunction: (param: any) => any): void;
    invokeAction(actionName: string, param?: any): IExtensionPromise<IGlobalActionResult>;
    addLoggingOutEventListener(callbackFunction: (param: ILogOutEventDetails) => IExtensionPromise<void>): void;
    getExtensionContext(extensionName: string): IExtensionPromise<IExtensionContext>;
    getContainerContext(): IExtensionPromise<IContainerContext>;
    getChatAPIInfo(): IExtensionPromise<IChatAPIInfo>;
  }

  export interface IExtensionOptListContext {
    createOptListRequest: () => IGetOptListRequest;
    createOptListSearchFilter: () => IOptListSearchFilter;
    createExternalMenuItem: () => IExternalMenuItem;
    createExternalMenuResultSet: () => IExternalMenuResultSet;
    groupFiltersWithAndOperator: (optListFilters: IOptListSearchFilter[]) => IOptListSearchFilter;
    groupFiltersWithOrOperator: (optListFilters: IOptListSearchFilter[]) => IOptListSearchFilter;
    getOptList: (optListRequest: IGetOptListRequest) => IExtensionPromise<IOptListItem>;
  }

  export interface IExternalMenuExecutionContext extends IGlobalContextEventDetails {
    getExternalMenuSearchCriteria: () => IExternalMenuSearchCriteria;
  }

  export interface IExternalMenuSearchCriteria {
    getMenuReference: () => string;
    getMenuId: () => number;
    getLimit: () => number;
    getSearchString: () => string;
    getStartingIndex: () => number;
    getParentId: () => number;
    getMenuItemIds: () => number[];
  }

  export interface IGetOptListRequest extends IExtensionDisposable {
    setOptListId: (optListId: number) => void;
    setLimit: (limit: number) => void;
    setOptListSearchFilter: (optListFilter: IOptListSearchFilter) => void;
    getFilterExpression: () => string;
  }

  export interface IOptListSearchFilter extends IExtensionDisposable {
    setSearchBy: (searchBy: string) => void;
    setSearchValue: (searchValue: any) => void;
    setCondition: (condition: string) => void;
    getFilterExpression: () => string;
  }

  export interface IOptListItem extends IExtensionDisposable {
    getId(): number;
    getLabel(): string;
    isRootNode(): boolean;
    getOptListChildren(): IOptListItem[];
    hasMoreOptListChildrenToLoad(): boolean;
    getOptListRequest(): IGetOptListRequest;
    setOptListRequest(optListRequest: IGetOptListRequest);
    loadMoreOptListChildren(): IExtensionPromise<IOptListItem>;
  }

  export interface IExternalMenuItem {
    getId(): number;
    setId(id: number): void;
    getLabel(): string;
    setLabel(label: string): void;
    getChildren(): IExternalMenuItem[];
    setChildren(children: IExternalMenuItem[]);
    hasMoreChildrenToLoad(): boolean;
    setMoreChildrenToLoad(moreChildrenToLoad: boolean);
  }

  export interface IExternalMenuResultSet extends IGlobalContextEventCallBackResult {
    getExternalMenuItems(): IExternalMenuItem[];
    setExternalMenuItems(externalMenuItems: IExternalMenuItem[]): void;
    hasMoreItems(): boolean;
    setHasMoreItems(moreItems: boolean);
  }

  export interface IModalWindowContext {
    createModalWindow(): IModalWindow;
    getCurrentModalWindow(): IExtensionPromise<IModalWindow>;
  }
  export interface IModalWindow extends IUserInterface {
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
  export interface IPopupWindow extends IUserInterface {
    setContentUrl(url: string): void;
    setWidth(width: string): void;
    setHeight(height: string): void;
    setClosable(isClosable: boolean): void;
    setTitle(title: string): void;
    render(): IExtensionPromise<IPopupWindow>;
    close(): IExtensionPromise<any>;
  }
  export interface IExtensionBarItem extends IUserInterface {
    setContentUrl(url: string): void;
    setWidth(width: number): void;
    setHeight(height: number): void;
    setVisible(visible: boolean): void;
    getId(): string;
    getContentUrl(): string;
    getWidth(): number;
    getHeight(): number;
    getVisible(): boolean;
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
    setMaxHeight(height: number): void;
    render(): void;
  }

  export interface INotificationContext {
    createNotificationConfig(): INotificationConfig;
    showNotification(notificationConfig: INotificationConfig): IExtensionPromise<void>;
  }

  export interface INotificationConfig {
    getMessage(): string;
    setMessage(message: string): void;
    getDuration(): number;
    setDuration(duration: number): void;
    getIconUrl(): string;
    setIconUrl(iconUrl: string): void;
    getActions(): INotificationAction[];
    addAction(action: INotificationAction): void;
    createAction(): INotificationAction;
    addClosedListener(handler: (config: INotificationConfig) => void): void;
    getClosedListeners(): ((config: INotificationConfig) => void)[];
    getPriority(): number;
    setPriority(priority: number): void;
  }

  export interface INotificationAction {
    getLabel(): string;
    setLabel(label: string): void;
    getHandler(): (notificationAction: INotificationAction) => void;
    setHandler(action: (action: INotificationAction) => void): void;
  }

  export interface IReportExecutionContext {
    getContext(): IUserInterface;
  }

  export interface ISearchReportContext extends IUserInterface {
    getSearchFieldDataKey(): ISearchFieldDataKey;
    getSearchType(): string;
  }

  export interface ISearchFieldDataKey {
    getEntityType(): string;
    getFieldKey(): string;
  }

  export interface IReportContext extends IUserInterface {}

  export interface IRibbonBarItem {
    getName(): string;
    isVisible(): boolean;
    isEnabled(): boolean;
    getDisplayText(): string;
    render(): IExtensionPromise<IRibbonBarItem>;
    setHidden(hidden: boolean): void;
  }

  export interface IGlobalContextEventDetails {}

  export interface IGlobalContextEventCallBackResult {}

  export interface ILogOutEventDetails extends IGlobalContextEventDetails {
    getReason(): string;
  }

  export interface IIntelligentAdvisorControl {
    getIntelligentAdvisorInterviewName(): string;
    setIntelligentAdvisorInterviewName(interviewName: string): void;
    getLocale(): string;
    setLocale(locale: string): void;
    render(): IExtensionPromise<IIntelligentAdvisorControl>;
  }

  export interface IExtensionContext {
    getProperties(configNames: string[]): IExtensionPromise<IExtensionPropertyCollection>;
    getConnections(connectionName?: string): IExtensionPromise<IExtensionConnectionCollection>;
  }

  export interface IContainerContext extends IExtensionContext {
    getExtensionName(): string;
    extensionName: string;
  }

  export interface IExtensionConnectionCollection {
    get(name: string): IExtensionConnection;
  }

  export interface IExtensionPropertyCollection {
    get(name: string): IExtensionProperty;
  }
  export interface IExtensionProperty {
    getName(): string;
    getValue(): any;
    getDataType(): string;
  }

  export interface IExtensionConnection {
    open(method: string, relativeUrl?: string): void;
    addRequestHeader(name: string, value: string): void;
    setContentType(contentType: string): void;
    setResponseType(dataType: string): void;
    send(payload?: any): IExtensionPromise<any>;
    getName(): string;
    getExtensionName(): string;
  }

  export interface IChatAPIInfo {
    getChatAPIURL(): string;
  }

  export interface IScriptContext {
    getScriptsById(scriptId: number): IExtensionPromise<IScriptEntry[]>;
    getScriptsByName(scriptName: string): IExtensionPromise<IScriptEntry[]>;
  }

  export interface IScriptQuestion {
    getQuestion(): string;
    getResponse(): string | string[];
  }

  export interface IScriptEntry {
    getScriptId(): number;
    getQuestionList(): IScriptQuestion[];
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
declare type IExtensionReport = ORACLE_SERVICE_CLOUD.IExtensionReport;
declare type IExtensionFilterDetails = ORACLE_SERVICE_CLOUD.IExtensionFilterDetails;
declare type IReportRelatedEntity = ORACLE_SERVICE_CLOUD.IReportRelatedEntity;
declare type IReportRelatedField = ORACLE_SERVICE_CLOUD.IReportRelatedField;
declare type IReportRelatedEntityDetails = ORACLE_SERVICE_CLOUD.IReportRelatedEntityDetails;
declare type IReportRelatedFieldDetails = ORACLE_SERVICE_CLOUD.IReportRelatedFieldDetails;
declare type IReportWorkspaceContext = ORACLE_SERVICE_CLOUD.IReportWorkspaceContext;
declare type IReportWorkspaceContextDetails = ORACLE_SERVICE_CLOUD.IReportWorkspaceContextDetails;
declare type IExtensionFilter = ORACLE_SERVICE_CLOUD.IExtensionFilter;
declare type IExtensionRangeFilter = ORACLE_SERVICE_CLOUD.IExtensionRangeFilter;
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
declare type IGlobalActionResult = ORACLE_SERVICE_CLOUD.IGlobalActionResult;
declare type ISubscriptionResult = ORACLE_SERVICE_CLOUD.ISubscriptionResult;
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
declare type INotificationContext = ORACLE_SERVICE_CLOUD.INotificationContext;
declare type INotificationConfig = ORACLE_SERVICE_CLOUD.INotificationConfig;
declare type INotificationAction = ORACLE_SERVICE_CLOUD.INotificationAction;
declare type IExtensionLogger = ORACLE_SERVICE_CLOUD.IExtensionLogger;
declare var ExtensionPromise: { new (handler?: (resolve: any, reject: any) => void): IExtensionPromise<any> };
declare type IStandardText = ORACLE_SERVICE_CLOUD.IStandardText;
declare type IStandardTextFocusChange = ORACLE_SERVICE_CLOUD.IStandardTextFocusChange;
declare type IStandardTextContext = ORACLE_SERVICE_CLOUD.IStandardTextContext;
declare type IStandardTextPane = ORACLE_SERVICE_CLOUD.IStandardTextPane;
declare type IStandardTextDataNode = ORACLE_SERVICE_CLOUD.IStandardTextDataNode;
declare type IRecordCommandContext = ORACLE_SERVICE_CLOUD.IRecordCommandContext;
declare type IReportRow = ORACLE_SERVICE_CLOUD.IReportRow;
declare type IReportCell = ORACLE_SERVICE_CLOUD.IReportCell;
declare type IReportRecord = ORACLE_SERVICE_CLOUD.IReportRecord;
declare type IIcon = ORACLE_SERVICE_CLOUD.IIcon;
declare type IReportConfiguration = ORACLE_SERVICE_CLOUD.IReportConfiguration;
declare type IGroupSidePaneAttributes = ORACLE_SERVICE_CLOUD.IGroupSidePaneAttributes;
declare type IReportInfo = ORACLE_SERVICE_CLOUD.IReportInfo;
declare type IUserInterface = ORACLE_SERVICE_CLOUD.IUserInterface;
declare type IReportDisplayDefinition = ORACLE_SERVICE_CLOUD.IReportDisplayDefinition;
declare type IReportColumnDisplayDefinition = ORACLE_SERVICE_CLOUD.IReportColumnDisplayDefinition;
declare type IExtensionCommandContext = ORACLE_SERVICE_CLOUD.IExtensionCommandContext;
declare type IExtensionCommandInfo = ORACLE_SERVICE_CLOUD.IExtensionCommandInfo;
declare type IWorkspaceCommandContext = ORACLE_SERVICE_CLOUD.IWorkspaceCommandContext;
declare type IWorkspaceInfo = ORACLE_SERVICE_CLOUD.IWorkspaceInfo;
declare type IReportRecordInfo = ORACLE_SERVICE_CLOUD.IReportRecordInfo;
declare type IReportExecutionContext = ORACLE_SERVICE_CLOUD.IReportExecutionContext;
declare type ISearchFieldDataKey = ORACLE_SERVICE_CLOUD.ISearchFieldDataKey;
declare type ISearchReportContext = ORACLE_SERVICE_CLOUD.ISearchReportContext;
declare type IRecordSelectionContext = ORACLE_SERVICE_CLOUD.IRecordSelectionContext;
declare type IRibbonBarItem = ORACLE_SERVICE_CLOUD.IRibbonBarItem;
declare type IExtensionOptListContext = ORACLE_SERVICE_CLOUD.IExtensionOptListContext;
declare type IGetOptListRequest = ORACLE_SERVICE_CLOUD.IGetOptListRequest;
declare type IOptListSearchFilter = ORACLE_SERVICE_CLOUD.IOptListSearchFilter;
declare type IOptListItem = ORACLE_SERVICE_CLOUD.IOptListItem;
declare type IMailRecipient = ORACLE_SERVICE_CLOUD.IMailRecipient;
declare type IMailAccount = ORACLE_SERVICE_CLOUD.IMailAccount;
declare type IMailGroup = ORACLE_SERVICE_CLOUD.IMailGroup;
declare type IEmailRecipient = ORACLE_SERVICE_CLOUD.IEmailRecipient;
declare type ICustomerResponseThreadEntry = ORACLE_SERVICE_CLOUD.ICustomerResponseThreadEntry;
declare type IMailErrorData = ORACLE_SERVICE_CLOUD.IMailErrorData;
declare type IEditedReportDataContext = ORACLE_SERVICE_CLOUD.IEditedReportDataContext;
declare type ILogOutEventDetails = ORACLE_SERVICE_CLOUD.ILogOutEventDetails;
declare type IIntelligentAdvisorControl = ORACLE_SERVICE_CLOUD.IIntelligentAdvisorControl;
declare type IWorkGroupEntity = ORACLE_SERVICE_CLOUD.IWorkGroupEntity;
declare type IExtensionWorkGroupContext = ORACLE_SERVICE_CLOUD.IExtensionWorkGroupContext;
declare type IGlobalContextEventDetails = ORACLE_SERVICE_CLOUD.IGlobalContextEventDetails;
declare type IWorkspaceConfig = ORACLE_SERVICE_CLOUD.IWorkspaceConfig;
declare type IExternalMenuItem = ORACLE_SERVICE_CLOUD.IExternalMenuItem;
declare type IExternalMenuSearchCriteria = ORACLE_SERVICE_CLOUD.IExternalMenuSearchCriteria;
declare type IExternalMenuResultSet = ORACLE_SERVICE_CLOUD.IExternalMenuResultSet;
declare type IExternalMenuExecutionContext = ORACLE_SERVICE_CLOUD.IExternalMenuExecutionContext;
declare type IExtensionContext = ORACLE_SERVICE_CLOUD.IExtensionContext;
declare type IExtensionPropertyCollection = ORACLE_SERVICE_CLOUD.IExtensionPropertyCollection;
declare type IExtensionProperty = ORACLE_SERVICE_CLOUD.IExtensionProperty;
declare type IContainerContext = ORACLE_SERVICE_CLOUD.IContainerContext;
declare type IChatAPIInfo = ORACLE_SERVICE_CLOUD.IChatAPIInfo;
declare type IScriptContext = ORACLE_SERVICE_CLOUD.IScriptContext;
declare type IScriptEntry = ORACLE_SERVICE_CLOUD.IScriptEntry;
declare type IScriptQuestion = ORACLE_SERVICE_CLOUD.IScriptQuestion;
declare type IExtensionConnection = ORACLE_SERVICE_CLOUD.IExtensionConnection;
declare type IExtensionConnectionCollection = ORACLE_SERVICE_CLOUD.IExtensionConnectionCollection;
declare type IChatEngagementRemovedEventArgs = ORACLE_SERVICE_CLOUD.IChatEngagementRemovedEventArgs;
declare type IChatEngagementAssignmentEventArgs = ORACLE_SERVICE_CLOUD.IChatEngagementAssignmentEventArgs;
declare type IChatEngagementAcceptedEventArgs = ORACLE_SERVICE_CLOUD.IChatEngagementAcceptedEventArgs;
declare type IChatSessionStatusEventArgs = ORACLE_SERVICE_CLOUD.IChatSessionStatusEventArgs;
declare type IChatMessage = ORACLE_SERVICE_CLOUD.IChatMessage;
