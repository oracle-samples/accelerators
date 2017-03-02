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
 *  SHA1: $Id: e6e2ee0a8c1bd88c4ebd85391a1a38bfb0bbd321 $
 * *********************************************************************************************
 *  File: SCLog.cs
 * *********************************************************************************************/

/**
 * SCLog is a logging class that groups output from several endpoints under a single file tree or database
 * grouping for a specific Product Extension (customization). The idea is to make it easy to see what Product Extension
 * exist on a site, which files comprise a given Product Extension and what they are doing as a system regardless of
 * what the pieces are; Add-ins, CP controllers, CPMs, Custom Scripts, etc.
 *
 * SCLog was created by Service Ventures LLC. (servicecloudedge.com) and is being released under an MIT License.
 *
 * @copyright   (c) Service Ventures LLC
 * @license     MIT
 * @package     ServiceVentures
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.AddIn;
using System.ServiceModel;
using System.ServiceModel.Channels;
using RightNow.AddIns.AddInViews;
using System.Threading.Tasks;
using Accelerator.SRM.SharedServices.RightNowServiceReference;
using System.Web.Script.Serialization;
using System.Diagnostics;

namespace ServiceVentures
{
    public class SCLog
    {
        public enum logLevel
        {
            Fatal = 1,
            Error = 2,
            Warning = 3,
            Notice = 4,
            Debug = 5,
            Click = 6,
            None = 0
        }

        #region private properties
        private const string DEFAULT_EXT = "General";
        private const string DEFAULT_SIG = "92acaed5cb6dc95783ea1d0b194347c542017bfc";
        private const string DEFAULT_CONFIG = "{"
                            + "\"logPessimistic\": \"0\","
                            + "\"logtoFile\": \"1\","
                            + "\"logtoDatabase\": \"1\","
                            + "\"logThreshHold\": \"6\","
                            + "\"logClicks\": \"1\","
                            + "\"pruningReport\": \"Automatic scLog Pruning\""
                            + "}";

        private string extName;
        private string extSignature;
        private RNObject extObject;
        private DateTime startTime;
        private string functionServed;
        private string callingFile;
        private Boolean callerHadErrors;
        private logLevel stdOutputThreshold;
        private extConfig extConfigs;
        private RightNowSyncPortClient client;
        private const int maxStringBytes = 1048576;
        #endregion private properties

        public SCLog(
            string extensionName = DEFAULT_EXT,
            string extensionSignature = DEFAULT_SIG,
            DateTime startTime = new DateTime(),
            logLevel stdOutLevel = logLevel.None,
            string functionServed = "Not Specified",
            string callingFile = "Not Specified"
            )
        {
            this.extName = extensionName;
            this.extSignature = extensionSignature;
            this.startTime = startTime;
            this.stdOutputThreshold = stdOutLevel;
            this.functionServed = functionServed;
            this.callingFile = callingFile;
        }

        public Boolean log(string message, string detail = null, Incident incident = null, Contact contact = null, string source = null, string function = null, int timeElapsed = 0, logLevel messageType = logLevel.Debug, string host = null, int processID = 0)
        {
            if (this.extConfigs == null)
            {
                throw new Exception("SCLog has not been initialized");
            }
            if ((int)messageType > this.extConfigs.logThreshhold)
            {
                return false;
            }
            GenericObject go = new GenericObject
            {
                ObjectType = new RNObjectType
                {
                    Namespace = "SvcVentures", // Custom Object
                    TypeName = "scLog" // Custom Object
                }
            };
            List<GenericField> fields = new List<GenericField>();
            if (message.Length > 255)
                message = message.Substring(0, 255);

            fields.Add(createGenericField("Message", ItemsChoiceType.StringValue, message));
            fields.Add(createGenericField("MsgType", ItemsChoiceType.IntegerValue, (int)messageType));
            long memory = GC.GetTotalMemory(true) / (1024 * 1024);
            fields.Add(createGenericField("PeakMemory", ItemsChoiceType.IntegerValue, (int)memory));

            if (string.IsNullOrWhiteSpace(source) == false)
                fields.Add(createGenericField("File", ItemsChoiceType.StringValue, trimMaxLengthString(source)));

            if (string.IsNullOrWhiteSpace(function) == false)
                fields.Add(createGenericField("Function", ItemsChoiceType.StringValue, trimMaxLengthString(function)));

            if (string.IsNullOrWhiteSpace(detail) == false)
                fields.Add(createGenericField("Detail", ItemsChoiceType.StringValue, trimMaxLengthString(detail)));

            if (string.IsNullOrWhiteSpace(host) == false)
                fields.Add(createGenericField("Host", ItemsChoiceType.StringValue, trimMaxLengthString(host)));

            fields.Add(createGenericField("ProcessID", ItemsChoiceType.IntegerValue, processID));
            fields.Add(createGenericField("TimeElapsed", ItemsChoiceType.IntegerValue, timeElapsed));
            fields.Add(createGenericField("scProductExtension", ItemsChoiceType.NamedIDValue,
                new NamedID
                {
                    ID = new ID
                    {
                        id = (int)this.extObject.ID.id,
                        idSpecified = true
                    }
                }
            ));

            if (!string.IsNullOrWhiteSpace(detail))
                fields.Add(createGenericField("Detail", ItemsChoiceType.StringValue, trimMaxLengthString(detail)));

            go.GenericFields = fields.ToArray();

            CreateProcessingOptions cpo = new CreateProcessingOptions();
            cpo.SuppressExternalEvents = false;
            cpo.SuppressRules = false;

            ClientInfoHeader clientInfoHeader = new ClientInfoHeader();
            clientInfoHeader.AppID = "Insert log";
            RNObject[] results = this.client.Create(clientInfoHeader, new RNObject[] { go }, cpo);

            // check result and save incident id
            if (results != null && results.Length > 0)
            {
                bool xRefsPresent = false;
                GenericObject xRefObj = new GenericObject
                {
                    ObjectType = new RNObjectType
                    {
                        Namespace = "SvcVentures", // Custom Object
                        TypeName = "scLogXref" // Custom Object
                    }
                };
                List<GenericField> xRefFields = new List<GenericField>();
                if (incident != null)
                {
                    xRefsPresent = true;
                    xRefFields.Add(createGenericField("Incident", ItemsChoiceType.NamedIDValue,
                        new NamedID
                        {
                            ID = new ID
                            {
                                id = (int)incident.ID.id,
                                idSpecified = true
                            }
                        }));
                }
                if (contact != null)
                {
                    xRefsPresent = true;
                    xRefFields.Add(createGenericField("Contact", ItemsChoiceType.NamedIDValue,
                        new NamedID
                        {
                            ID = new ID
                            {
                                id = (int)contact.ID.id,
                                idSpecified = true
                            }
                        }));
                }
                if (xRefsPresent == false)
                {
                    return true;
                }
                xRefFields.Add(createGenericField("scLog", ItemsChoiceType.NamedIDValue,
                    new NamedID
                    {
                        ID = new ID
                        {
                            id = (int)results[0].ID.id,
                            idSpecified = true
                        }
                    }
                ));
                xRefObj.GenericFields = xRefFields.ToArray();
                RNObject[] xRefResults = this.client.Create(clientInfoHeader, new RNObject[] { xRefObj }, cpo);
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean error(string message, string detail = null, Incident incident = null, Contact contact = null, string source = null, string function = null, int timeElapsed = 0, string host = null)
        {
            return this.log(message, detail, incident, contact, source, function, timeElapsed, logLevel.Error, host);
        }

        public Boolean debug(string message, string detail = null, Incident incident = null, Contact contact = null, string source = null, string function = null, int timeElapsed = 0, string host = null)
        {
            return this.log(message, detail, incident, contact, source, function, timeElapsed, logLevel.Debug, host);
        }

        public Boolean notice(string message, string detail = null, Incident incident = null, Contact contact = null, string source = null, string function = null, int timeElapsed = 0, string host = null)
        {
            return this.log(message, detail, incident, contact, source, function, timeElapsed, logLevel.Notice, host);
        }

        public Boolean click(string message, string detail = null, Incident incident = null, Contact contact = null, string source = null, string function = null, int timeElapsed = 0, string host = null)
        {
            return this.log(message, detail, incident, contact, source, function, timeElapsed, logLevel.Click, host);
        }

        public Boolean initializeLogger(IGlobalContext globalContext)
        {
            EndpointAddress endPointAddr = new EndpointAddress(globalContext.GetInterfaceServiceUrl(ConnectServiceType.Soap));

            // Minimum required
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportWithMessageCredential);
            binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;

            // Optional depending upon use cases
            binding.MaxReceivedMessageSize = 1024 * 1024;
            binding.MaxBufferSize = 1024 * 1024;
            binding.MessageEncoding = WSMessageEncoding.Mtom;

            // Create client proxy class
            RightNowSyncPortClient client = new RightNowSyncPortClient(binding, endPointAddr);

            // Ask the client to not send the timestamp
            BindingElementCollection elements = client.Endpoint.Binding.CreateBindingElements();
            elements.Find<SecurityBindingElement>().IncludeTimestamp = false;
            client.Endpoint.Binding = new CustomBinding(elements);

            // Ask the Add-In framework the handle the session logic
            globalContext.PrepareConnectSession(client.ChannelFactory);

            this.client = client;
            this.extObject = getExtension();
            return true;
        }

        public Boolean initializeLogger(RightNowSyncPortClient client)
        {
            // Ask the client to not send the timestamp
            BindingElementCollection elements = client.Endpoint.Binding.CreateBindingElements();
            elements.Find<SecurityBindingElement>().IncludeTimestamp = false;
            client.Endpoint.Binding = new CustomBinding(elements);

            this.client = client;
            this.extObject = getExtension();
            return true;
        }

        protected RNObject getExtension()
        {
            RNObject currentExt = lookupExtension();
            if (currentExt == null)
            {
                currentExt = createExtension();
            }
            GenericObject extentionObj = (GenericObject)currentExt;
            string extConfigJson = extentionObj.GenericFields.FirstOrDefault<GenericField>(c => c.name == "ExtConfiguration").DataValue.Items[0].ToString();
            var serializer = new JavaScriptSerializer();
            this.extConfigs = serializer.Deserialize<extConfig>(extConfigJson);
            return currentExt;
        }

        private RNObject lookupExtension()
        {
            GenericObject generic = new GenericObject();
            RNObjectType rnObj = new RNObjectType();
            rnObj.Namespace = "SvcVentures";
            rnObj.TypeName = "scProductExtension";
            generic.ObjectType = rnObj;

            string query = "SELECT SvcVentures.scProductExtension FROM SvcVentures.scProductExtension where Signature like '" + this.extSignature + "';";

            RNObject[] objectTemplate = new RNObject[] { generic };
            RNObject[] result = null;
            try
            {
                QueryResultData[] queryObjects = client.QueryObjects(
                    new ClientInfoHeader { AppID = "SCLog" },
                    query,
                    objectTemplate, 10000);
                result = queryObjects[0].RNObjectsResult;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }

            if (result != null && result.Length > 0)
            {
                return result[0];
            }
            else
            {
                return null;
            }
        }

        private RNObject createExtension()
        {
            GenericObject go = new GenericObject();

            //Set the object type
            RNObjectType objType = new RNObjectType();
            objType.Namespace = "SvcVentures";
            objType.TypeName = "scProductExtension";
            go.ObjectType = objType;

            List<GenericField> gfs = new List<GenericField>();

            gfs.Add(createGenericField("ExtensionName", ItemsChoiceType.StringValue, this.extName));
            gfs.Add(createGenericField("Signature", ItemsChoiceType.StringValue, this.extSignature));
            gfs.Add(createGenericField("Description", ItemsChoiceType.StringValue, "Not Specified"));
            gfs.Add(createGenericField("Authors", ItemsChoiceType.StringValue, "Not Specified"));
            gfs.Add(createGenericField("ExtConfiguration", ItemsChoiceType.StringValue, DEFAULT_CONFIG));
            go.GenericFields = gfs.ToArray();

            CreateProcessingOptions cpo = new CreateProcessingOptions();
            cpo.SuppressExternalEvents = false;
            cpo.SuppressRules = false;

            ClientInfoHeader clientInfoHeader = new ClientInfoHeader();
            clientInfoHeader.AppID = "Create Extension";
            RNObject[] results = client.Create(clientInfoHeader, new RNObject[] { go }, cpo);

            // check result and save incident id
            if (results != null && results.Length > 0)
            {
                return go;
            }
            else
            {
                return null;
            }
        }

        private GenericField createGenericField(string Name, ItemsChoiceType itemsChoiceType, object Value)
        {
            GenericField gf = new GenericField();
            gf.name = Name;
            gf.DataValue = new DataValue();
            gf.DataValue.ItemsElementName = new ItemsChoiceType[] { itemsChoiceType };
            gf.DataValue.Items = new object[] { Value };
            return gf;
        }

        private string trimMaxLengthString(string value)
        {
            int stringBytes = ASCIIEncoding.Unicode.GetByteCount(value);
            if (stringBytes > maxStringBytes)
            {
                int maxChars = maxStringBytes / sizeof(Char);
                return value.Substring(0, maxChars - 1);
            }
            else
            {
                return value;
            }
        }
    }

    public class extConfig
    {
        public Int32 logToDatabase { get; set; }
        public Int32 logThreshhold { get; set; }
        public Int32 logClicks { get; set; }
        public Int32 logPessimistic { get; set; }
        public Int32 logToFile { get; set; }
    }
}
