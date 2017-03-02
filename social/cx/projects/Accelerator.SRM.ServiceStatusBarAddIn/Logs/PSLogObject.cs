/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  Copyright (c) 2014, 2015, 2016, 2017 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + SRM Integration Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 17.2 (February 2017) 
 *  reference: 160628-000117
 *  date: Fri Feb 10 19:47:43 PST 2017
 
 *  revision: rnw-17-2-fixes-release-2
 *  SHA1: $Id: deeabb35e8b808b48deeb90b306b3fc68f32ae79 $
 * *********************************************************************************************
 *  File: PSLogObject.cs
 * *********************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accelerator.SRM.SharedServices.RightNowServiceReference;
using System.Reflection;
using System.ComponentModel;

namespace PS
{
    namespace Log
    {
        /*
            * Query: desc PSLog$Log
            Rows: 19  (0.0 sec)  
            Field	            Type	    Null	Key	Default	Extra
            Account				int(11)		YES		MUL	NULL	
            Answer	            int(11)		YES		MUL	NULL	
            Contact				int(11)		YES		MUL	NULL	
            CreatedByAccount	int(11)		YES		MUL	NULL	
            CreatedTime	        timestamp	YES			NULL	
            ID	                int(11)		NO	    PRI	NULL	auto_increment
            Incident			int(11)		YES		MUL	NULL	
            Interface			int(11)		YES		MUL	NULL	
            Message				varchar(255)YES			NULL	
            Note				mediumtext	YES			NULL	
            Opportunity	    	int(11)		YES		MUL	NULL	
            Organization		int(11)		YES		MUL	NULL	
            Severity			int(11)		YES		MUL	NULL	
            StackTrace			mediumtext	YES			NULL	
            SubType				varchar(80)	YES			NULL	
            Task				int(11)		YES		MUL	NULL	
            Type	            int(11)		YES		MUL	NULL	
            UpdatedByAccount	int(11)		YES		MUL	NULL	
            UpdatedTime	        timestamp	YES			NULL	
            */

        /// <summary>
        /// Extension Class for RNT Soap Client
        /// 
        /// Allows for logs to be written to the custom object PSLog$Log
        /// </summary>
        public static class Log
        {
            #region enums
            /// <summary>
            /// What type of application is this
            /// </summary>
            public enum Type
            {
                /// <summary>
                /// a .net component that executes from within the CX console
                /// </summary>
                AddIn = 1,

                /// <summary>
                /// a customization on the customer's end-user pages
                /// </summary>
                CP = 2,

                /// <summary>
                /// a process that runs on a schedule
                /// </summary>
                Cron = 3,

                /// <summary>
                /// A process or script in the custom directory
                /// </summary>
                CustomAPI = 4,

                /// <summary>
                /// Pushing data from rightnow
                /// </summary>
                Export = 5,

                /// <summary>
                /// A process that fires on an external event (ex contact create)
                /// </summary>
                ExternalEvent = 6,

                /// <summary>
                /// Pulling data into rightnow
                /// </summary>
                Import = 7,

                /// <summary>
                /// A process that fires on a Custom Process Model event (ex contact create)
                /// </summary>
                CPM = 8,

                /// <summary>
                /// Only usable when updating an existing error
                /// </summary>
                None
            }

            /// <summary>
            /// how bad is it?
            /// </summary>
            public enum Severity
            {
                /// <summary>
                /// there is no way for the application to continue
                /// ex: import file missing
                /// </summary>
                Fatal = 1,

                /// <summary>
                /// something bad happened, but we'll get through it
                /// ex: import file missing email addr for line #3
                /// </summary>
                Error = 2,

                /// <summary>
                /// this really shouldn't happen, but it's not a big deal
                /// ex: contact with that email addr already exists
                /// </summary>
                Warning = 3,

                /// <summary>
                /// failures are expected, how bad was it?
                /// ex: contact import had 32 duplicate entries
                /// </summary>
                Debug = 4,

                /// <summary>
                /// what's the status?
                /// ex: contact import complete: 200 contacts created
                /// </summary>
                Info = 5,

                /// <summary>
                /// messages that should not go out to production
                /// ex: entering ContactCreate()
                /// </summary>
                Notice = 6,

                /// <summary>
                /// Don't set a severity
                /// </summary>
                None
            }
            #endregion

            #region static values
            /// <summary>
            /// the number of characters in a medium text
            /// </summary>
            private static readonly int MEDIUM_TEXT_LENGTH = 1333;
            #endregion

            /// <summary>
            /// An error occurred while writing to the PSLog
            /// </summary>
            /// <param name="error">details on the error</param>
            public delegate void LogFailedDelegate(Exception error);

            /// <summary>
            /// Fired when an exception is handled by the log method
            /// </summary>
            public static event LogFailedDelegate LogFailedEvent;


            public delegate void CreateLogAsyncCompleteDelegate(int logID);
            public static event CreateLogAsyncCompleteDelegate CreateLogAsyncCompleteEvent;


            public delegate void UpdateLogAsyncCompleteDelegate();
            public static event UpdateLogAsyncCompleteDelegate UpdateLogAsyncCompleteEvent;

            /// <summary>
            /// Create a log entry in the custom object PSLog$Log
            /// 
            /// If this table does not exists, please retrieve the package from: https://tools.src.rightnow.com/spaces/logging/documents
            /// </summary>
            /// <param name="client">Extend the RightNow soap client</param>
            /// <param name="type">Required: application type causing the log message</param>
            /// <param name="message">Required: human readable friendly message</param>
            /// <param name="subtype">Required: friendly name of the application</param>
            /// <param name="note">Optional (default=blank): additional details about the message</param>
            /// <param name="source">Optional (default=current assembly name): the name of the script that is executing</param>
            /// <param name="interfaceID">Optional (default=blank): the interface this error occurred on. (GlobalContext.InterfaceId)</param>
            /// <param name="severity">Optional (default=blank): mark the importance of the message</param>
            /// <param name="exception">Optional (default=blank): if populated the stack trace from the exception will be stored in the log</param>
            /// <param name="account">Optional (default=blank): reference to an Account</param>
            /// <param name="answer">Optional (default=blank): reference to an Answer</param>
            /// <param name="contact">Optional (default=blank): reference to a Contact</param>
            /// <param name="incident">Optional (default=blank): reference to an Incident</param>
            /// <param name="opportunity">Optional (default=blank): reference to an Opportunity</param>
            /// <param name="org">Optional (default=blank): reference to an Organization</param>
            /// <param name="task">Optional (default=blank): reference to a Task</param>
            /// <param name="customObjects">Optional (default=blank): an array of custom object references. the name of the object must match the name of the database column</param>
            /// <returns>ID of the newly created log object</returns>
            public static void CreateLogAsync(this RightNowSyncPortClient client,
                Type type,
                string message,
                string subtype = null,
                string note = null,
                string source = null,
                int? interfaceID = null,
                Severity severity = Severity.None,
                Exception exception = null,
                Account account = null,
                Answer answer = null,
                Contact contact = null,
                Incident incident = null,
                Opportunity opportunity = null,
                Organization org = null,
                Task task = null,
                GenericObject[] customObjects = null)
            {
                BackgroundWorker asyncWorker = new BackgroundWorker();
                asyncWorker.DoWork += delegate(object sender, DoWorkEventArgs e) { e.Result = CreateLog(client, type, message, subtype, note, source, interfaceID, severity, exception, account, answer, contact, incident, opportunity, org, task, customObjects); };
                asyncWorker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e) { if (CreateLogAsyncCompleteEvent != null) { CreateLogAsyncCompleteEvent((int)e.Result); } };
                asyncWorker.RunWorkerAsync();
            }


            /// <summary>
            /// Create a log entry in the custom object PSLog$Log
            /// 
            /// If this table does not exists, please retrieve the package from: https://tools.src.rightnow.com/spaces/logging/documents
            /// </summary>
            /// <param name="client">Extend the RightNow soap client</param>
            /// <param name="type">Required: application type causing the log message</param>
            /// <param name="message">Required: human readable friendly message</param>
            /// <param name="subtype">Required: friendly name of the application</param>
            /// <param name="note">Optional (default=blank): additional details about the message</param>
            /// <param name="source">Optional (default=current assembly name): the name of the script that is executing</param>
            /// <param name="interfaceID">Optional (default=blank): the interface this error occurred on. (GlobalContext.InterfaceId)</param>
            /// <param name="severity">Optional (default=blank): mark the importance of the message</param>
            /// <param name="exception">Optional (default=blank): if populated the stack trace from the exception will be stored in the log</param>
            /// <param name="account">Optional (default=blank): reference to an Account</param>
            /// <param name="answer">Optional (default=blank): reference to an Answer</param>
            /// <param name="contact">Optional (default=blank): reference to a Contact</param>
            /// <param name="incident">Optional (default=blank): reference to an Incident</param>
            /// <param name="opportunity">Optional (default=blank): reference to an Opportunity</param>
            /// <param name="org">Optional (default=blank): reference to an Organization</param>
            /// <param name="task">Optional (default=blank): reference to a Task</param>
            /// <param name="customObjects">Optional (default=blank): an array of custom object references. the name of the object must match the name of the database column</param>
            /// <returns>ID of the newly created log object</returns>
            public static int CreateLog(this RightNowSyncPortClient client,
                Type type,
                string message,
                string subtype = null,
                string note = null,
                string source = null,
                int? interfaceID = null,
                Severity severity = Severity.None,
                Exception exception = null,
                Account account = null,
                Answer answer = null,
                Contact contact = null,
                Incident incident = null,
                Opportunity opportunity = null,
                Organization org = null,
                Task task = null,
                GenericObject[] customObjects = null)
            {
                #region sanity checks
                if (type == Type.None)
                    throw new Exception("Type can not be None when creating a new Log entry");

                if (string.IsNullOrWhiteSpace(message))
                    throw new Exception("Message can not be empty");
                #endregion

                try
                {
                    if (source == null)
                        source = Assembly.GetExecutingAssembly().ManifestModule.Name + " (" + Assembly.GetExecutingAssembly().GetName().Version + ")";

                    GenericObject go = CreateLogGenericObject(type, subtype, message, note, source, interfaceID, severity, exception, account, answer, contact, incident, opportunity, org, task, customObjects);

                    RNObject[] results = client.Create(
                        new ClientInfoHeader { AppID = "PSLog" },
                        new RNObject[] { go },
                        new CreateProcessingOptions { SuppressExternalEvents = true, SuppressRules = false });

                    if (results == null || results.Length == 0)
                        return 0;
                    else
                        return (int)results[0].ID.id;
                }
                catch (Exception ex)
                {
                    if (LogFailedEvent != null)
                        LogFailedEvent(ex);

                    return 0;
                }
            }

            /// <summary>
            /// Create a log entry in the custom object PSLog$Log
            /// 
            /// If this table does not exists, please retrieve the package from: https://tools.src.rightnow.com/spaces/logging/documents
            /// </summary>
            /// <param name="client">Extend the RightNow soap client</param>
            /// <param name="existingID">ID of the existing PSLog object</param>
            /// <param name="type">Optional (default=blank): application type causing the log m
            /// <param name="message">Optional (default=blank): human readable friendly message</param>essage</param>
            /// <param name="subtype">Optional (default=blank): friendly name of the application</param>
            /// <param name="source">Optional (default=blank): name of the current assembly or script</param>
            /// <param name="note">Optional (default=blank): additional details about the message</param>
            /// <param name="interfaceID">Optional (default=blank): the interface this error occurred on. (GlobalContext.InterfaceId)</param>
            /// <param name="severity">Optional (default=blank): mark the importance of the message</param>
            /// <param name="exception">Optional (default=blank): if populated the stack trace from the exception will be stored in the log</param>
            /// <param name="account">Optional (default=blank): reference to an Account</param>
            /// <param name="answer">Optional (default=blank): reference to an Answer</param>
            /// <param name="contact">Optional (default=blank): reference to a Contact</param>
            /// <param name="incident">Optional (default=blank): reference to an Incident</param>
            /// <param name="opportunity">Optional (default=blank): reference to an Opportunity</param>
            /// <param name="org">Optional (default=blank): reference to an Organization</param>
            /// <param name="task">Optional (default=blank): reference to a Task</param>
            /// <param name="customObjects">Optional (default=blank): an array of custom object references. the name of the object must match the name of the database column</param>
            /// <returns>ID of the newly created log object</returns>
            public static void UpdateLogAsync(this RightNowSyncPortClient client,
                int existingID,
                Type type = Type.None,
                string message = null,
                string subtype = null,
                string note = null,
                string source = null,
                int? interfaceID = null,
                Severity severity = Severity.None,
                Exception exception = null,
                Account account = null,
                Answer answer = null,
                Contact contact = null,
                Incident incident = null,
                Opportunity opportunity = null,
                Organization org = null,
                Task task = null,
                GenericObject[] customObjects = null)
            {
                BackgroundWorker asyncWorker = new BackgroundWorker();
                asyncWorker.DoWork += delegate(object sender, DoWorkEventArgs e) { UpdateLog(client, existingID, type, message, subtype, note, source, interfaceID, severity, exception, account, answer, contact, incident, opportunity, org, task, customObjects); };
                asyncWorker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e) { if (UpdateLogAsyncCompleteEvent != null) { UpdateLogAsyncCompleteEvent(); } };
                asyncWorker.RunWorkerAsync();
            }

            /// <summary>
            /// Create a log entry in the custom object PSLog$Log
            /// 
            /// If this table does not exists, please retrieve the package from: https://tools.src.rightnow.com/spaces/logging/documents
            /// </summary>
            /// <param name="client">Extend the RightNow soap client</param>
            /// <param name="existingID">ID of the existing PSLog object</param>
            /// <param name="type">Optional (default=blank): application type causing the log m
            /// <param name="message">Optional (default=blank): human readable friendly message</param>essage</param>
            /// <param name="subtype">Optional (default=blank): friendly name of the application</param>
            /// <param name="source">Optional (default=blank): name of the current assembly or script</param>
            /// <param name="note">Optional (default=blank): additional details about the message</param>
            /// <param name="interfaceID">Optional (default=blank): the interface this error occurred on. (GlobalContext.InterfaceId)</param>
            /// <param name="severity">Optional (default=blank): mark the importance of the message</param>
            /// <param name="exception">Optional (default=blank): if populated the stack trace from the exception will be stored in the log</param>
            /// <param name="account">Optional (default=blank): reference to an Account</param>
            /// <param name="answer">Optional (default=blank): reference to an Answer</param>
            /// <param name="contact">Optional (default=blank): reference to a Contact</param>
            /// <param name="incident">Optional (default=blank): reference to an Incident</param>
            /// <param name="opportunity">Optional (default=blank): reference to an Opportunity</param>
            /// <param name="org">Optional (default=blank): reference to an Organization</param>
            /// <param name="task">Optional (default=blank): reference to a Task</param>
            /// <param name="customObjects">Optional (default=blank): an array of custom object references. the name of the object must match the name of the database column</param>
            /// <returns>ID of the newly created log object</returns>
            public static void UpdateLog(this RightNowSyncPortClient client,
                int existingID,
                Type type = Type.None,
                string message = null,
                string subtype = null,
                string note = null,
                string source = null,
                int? interfaceID = null,
                Severity severity = Severity.None,
                Exception exception = null,
                Account account = null,
                Answer answer = null,
                Contact contact = null,
                Incident incident = null,
                Opportunity opportunity = null,
                Organization org = null,
                Task task = null,
                GenericObject[] customObjects = null)
            {
                try
                {
                    GenericObject go = CreateLogGenericObject(type, subtype, message, note, source, interfaceID, severity, exception, account, answer, contact, incident, opportunity, org, task, customObjects);
                    go.ID = new ID
                    {
                        id = existingID,
                        idSpecified = true
                    };

                    client.Update(
                        new ClientInfoHeader { AppID = "PSLog" },
                        new RNObject[] { go },
                        new UpdateProcessingOptions { SuppressExternalEvents = true, SuppressRules = false });

                }
                catch (Exception ex)
                {
                    if (LogFailedEvent != null)
                        LogFailedEvent(ex);
                }
            }

            //-------------------------------PRIVATE--------------------------------//
            /// <summary>
            /// Creates a generic object, used by both create and update
            /// </summary>
            /// <param name="type">application type causing the log message</param>
            /// <param name="subtype">name of the application</param>
            /// <param name="message">human readable friendly message</param>
            /// <param name="note">additional details about the message</param>
            /// <param name="interfaceID">the interface this error occurred on. (GlobalContext.InterfaceId)</param>
            /// <param name="severity">mark the importance of the message</param>
            /// <param name="exception">if populated the stack trace from the exception will be stored in the log</param>
            /// <param name="account">reference to an Account</param>
            /// <param name="answer">reference to an Answer</param>
            /// <param name="contact">reference to a Contact</param>
            /// <param name="incident">reference to an Incident</param>
            /// <param name="opportunity">reference to an Opportunity</param>
            /// <param name="org">reference to an Organization</param>
            /// <param name="task">reference to a Task</param>
            /// <param name="customObjects">an array of custom object references. the name of the object must match the name of the database column</param>
            /// <returns>populated generic object</returns>
            private static GenericObject CreateLogGenericObject(Type type, string subtype, string message, string note, string source, int? interfaceID, Severity severity, Exception exception, Account account, Answer answer, Contact contact, Incident incident, Opportunity opportunity, Organization org, Task task, GenericObject[] customObjects)
            {
                GenericObject go = new GenericObject
                {
                    ObjectType = new RNObjectType
                    {
                        Namespace = "PSLog",
                        TypeName = "Log"
                    }
                };

                List<GenericField> fields = new List<GenericField>();

                //------------strings-----------//
                #region subtype
                if (string.IsNullOrWhiteSpace(subtype) == false)
                    fields.Add(CreateGenericField("SubType", substring(subtype, 80), DataTypeEnum.STRING, ItemsChoiceType.StringValue));
                #endregion

                #region message
                if (string.IsNullOrWhiteSpace(message) == false)
                    fields.Add(CreateGenericField("Message", substring(message, 255), DataTypeEnum.STRING, ItemsChoiceType.StringValue));
                #endregion

                #region note
                if (string.IsNullOrWhiteSpace(note) == false)
                    fields.Add(CreateGenericField("Note", substring(note, MEDIUM_TEXT_LENGTH), DataTypeEnum.STRING, ItemsChoiceType.StringValue));
                #endregion

                #region source
                if (string.IsNullOrWhiteSpace(source) == false)
                    fields.Add(CreateGenericField("Source", substring(source, 255), DataTypeEnum.STRING, ItemsChoiceType.StringValue));
                #endregion

                #region exception - stacktrace
                if (exception != null)
                    fields.Add(CreateGenericField("StackTrace", substring(ExceptionToString(exception), MEDIUM_TEXT_LENGTH), DataTypeEnum.STRING, ItemsChoiceType.StringValue));
                #endregion

                //------menus & foreign refs----//

                #region type
                if (type != Type.None)
                {
                    fields.Add(CreateGenericField("Type",
                        new NamedID
                        {
                            ID = new ID
                            {
                                id = (int)type,
                                idSpecified = true
                            }
                        }, DataTypeEnum.NAMED_ID, ItemsChoiceType.NamedIDValue));
                }
                #endregion

                #region interfaceID
                if (interfaceID != null && interfaceID > 0)
                {
                    fields.Add(CreateGenericField("Interface",
                        new NamedID
                        {
                            ID = new ID
                            {
                                id = (long)interfaceID,
                                idSpecified = true
                            }
                        }, DataTypeEnum.NAMED_ID, ItemsChoiceType.NamedIDValue));
                }
                #endregion

                #region Severity
                if (severity != Severity.None)
                {
                    fields.Add(CreateGenericField("Severity",
                        new NamedID
                        {
                            ID = new ID
                            {
                                id = (int)severity,
                                idSpecified = true
                            }
                        }, DataTypeEnum.NAMED_ID, ItemsChoiceType.NamedIDValue));
                }
                #endregion

                #region Account
                if (account != null && account.ID != null && account.ID.id > 0)
                {
                    fields.Add(CreateGenericField("Account",
                        new NamedID
                        {
                            ID = new ID
                            {
                                id = (int)account.ID.id,
                                idSpecified = true
                            }
                        }, DataTypeEnum.NAMED_ID, ItemsChoiceType.NamedIDValue));
                }
                #endregion

                #region Answer
                if (answer != null && answer.ID != null && answer.ID.id > 0)
                {
                    fields.Add(CreateGenericField("Answer",
                        new NamedID
                        {
                            ID = new ID
                            {
                                id = (int)answer.ID.id,
                                idSpecified = true
                            }
                        }, DataTypeEnum.NAMED_ID, ItemsChoiceType.NamedIDValue));
                }
                #endregion

                #region Contact
                if (contact != null && contact.ID != null && contact.ID.id > 0)
                {
                    fields.Add(CreateGenericField("Contact",
                        new NamedID
                        {
                            ID = new ID
                            {
                                id = (int)contact.ID.id,
                                idSpecified = true
                            }
                        }, DataTypeEnum.NAMED_ID, ItemsChoiceType.NamedIDValue));
                }
                #endregion

                #region Incident
                if (incident != null && incident.ID != null && incident.ID.id > 0)
                {
                    fields.Add(CreateGenericField("Incident",
                        new NamedID
                        {
                            ID = new ID
                            {
                                id = (int)incident.ID.id,
                                idSpecified = true
                            }
                        }, DataTypeEnum.NAMED_ID, ItemsChoiceType.NamedIDValue));
                }
                #endregion

                #region Opportunity
                if (opportunity != null && opportunity.ID != null && opportunity.ID.id > 0)
                {
                    fields.Add(CreateGenericField("Opportunity",
                        new NamedID
                        {
                            ID = new ID
                            {
                                id = (int)opportunity.ID.id,
                                idSpecified = true
                            }
                        }, DataTypeEnum.NAMED_ID, ItemsChoiceType.NamedIDValue));
                }
                #endregion

                #region Organization
                if (org != null && org.ID != null && org.ID.id > 0)
                {
                    fields.Add(CreateGenericField("Organization",
                        new NamedID
                        {
                            ID = new ID
                            {
                                id = (int)org.ID.id,
                                idSpecified = true
                            }
                        }, DataTypeEnum.NAMED_ID, ItemsChoiceType.NamedIDValue));
                }
                #endregion

                #region Task
                if (task != null && task.ID != null && task.ID.id > 0)
                {
                    fields.Add(CreateGenericField("Task",
                        new NamedID
                        {
                            ID = new ID
                            {
                                id = (int)task.ID.id,
                                idSpecified = true
                            }
                        }, DataTypeEnum.NAMED_ID, ItemsChoiceType.NamedIDValue));
                }
                #endregion

                #region custom object references
                if (customObjects != null)
                {
                    foreach (GenericObject customRef in customObjects)
                    {
                        fields.Add(CreateGenericField(
                            customRef.ObjectType.TypeName,
                            new NamedID { ID = customRef.ID, },
                            DataTypeEnum.NAMED_ID,
                            ItemsChoiceType.NamedIDValue));
                    }
                }
                #endregion

                go.GenericFields = fields.ToArray();

                return go;
            }

            /// <summary>
            /// Helper function for creating generic fields
            /// </summary>
            /// <param name="fieldName">column name</param>
            /// <param name="value">value to store</param>
            /// <param name="dataType">type of data for value</param>
            /// <param name="itemType">should be the same as dataType</param>
            /// <returns>a new generic field</returns>
            private static GenericField CreateGenericField(string fieldName, object value, DataTypeEnum dataType, ItemsChoiceType itemType)
            {
                return new GenericField
                {
                    dataType = dataType,
                    dataTypeSpecified = true,
                    DataValue = new DataValue
                    {
                        Items = new object[] { value },
                        ItemsElementName = new ItemsChoiceType[] { itemType }
                    },
                    name = fieldName
                };
            }

            /// <summary>
            /// Grabs the entire contents of an exception
            /// </summary>
            /// <param name="ex">an exception</param>
            /// <returns>the string representation of the exception</returns>
            private static string ExceptionToString(Exception ex)
            {
                //sanity check
                if (ex == null)
                    return "";

                string results = ex.ToString();

                Exception inner = ex.InnerException;
                while (inner != null)
                {
                    results += "---------------------Inner Exception----------------------\r\n";
                    results += inner.ToString();

                    inner = inner.InnerException;
                }

                return results;
            }

            /// <summary>
            /// if the string is null then return ""
            /// if the string is greater than max length then trim to length
            /// else return string
            /// </summary>
            /// <param name="value">the string to verify</param>
            /// <param name="maxLength">the string will be trimmed to this length if it is too long</param>
            /// <returns>a portion of the input string</returns>
            private static string substring(string value, int maxLength)
            {
                if (string.IsNullOrWhiteSpace(value))
                    return "";
                else if (value.Length > maxLength)
                    return value.Substring(0, maxLength);
                else
                    return value;
            }


        }
    }
}