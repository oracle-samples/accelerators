/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC Contact Center + Siebel Case Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  Siebel release: 8.1.1.15
 *  reference: 150520-000047
 *  date: Mon Nov 30 20:14:28 PST 2015

 *  revision: rnw-15-11-fixes-release-2
 *  SHA1: $Id: 4b299135c3f0f88abb254ab5e4230626bd35fc3a $
 * *********************************************************************************************
 *  File: ConfigurationSetting.cs
 * *********************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using Accelerator.Siebel.SharedServices.Logs;
using Accelerator.Siebel.SharedServices.RightNowServiceReference;
using RightNow.AddIns.AddInViews;

namespace Accelerator.Siebel.SharedServices
{
    /*  class ConfigurationSetting is for Configuration Verb Parsing.
     *   
     *  It will be called when CX Console is launched and cache the parsing result
     */
    public sealed class ConfigurationSetting
    {
        //Using lock and Singleton - to cache parsing result
        private static ConfigurationSetting instance = null;
        public static readonly object padlock = new object();

        public static RightNowService rnSrv;
        public static RightNowSyncPortClient client;

        // for status bar
        public const int LOGHISTORYROW = 10;
        public static IconLabelControl iconLabelControl;
        public static Queue logHistory;
        public static int logHistoryIndex = 0;
        public static System.Timers.Timer clearStatusBarTimer;
        public static DateTime lastLoggedTime;

        //Information parsing from configuration
        public static LogWrapper logWrap;
        //public static string logClass = "Accelerator.Siebel.SharedServices.Logs.Logger";  //SCLog
        public static string logClass = "Accelerator.Siebel.SharedServices.Logs.DefaultLog";
        public static String rnt_host;
        public static String username;
        public static String password;
        public static int SiebelServiceTimeout;
        public static int AdminProfileID; // from config verb: AdminProfileID
        public static string log_level; // Error, Notice, Debug
        public static int contactSearchReportID;
        public static int incidentsByContactReportID;
        public static Dictionary<String, String> requestTypeMapping;

        //Web Service Endpoints defined in configuration
        public static String SiebelProvider;
        public static String CreateRepair_WSDL;
        public static String UpdateRepair_WSDL;
        public static String LookupRepair_WSDL;
        public static String LookupRepairList_WSDL;
        public static String CreateSR_WSDL;
        public static String UpdateSR_WSDL;
        public static String LookupSR_WSDL;
        public static String CreateInteraction_WSDL;
        public static String LookupContactList_WSDL;
        public static String LookupSRbyContactPartyID_WSDL;
        public static String ItemList_WSDL;
        public static String EntitlementList_WSDL;
        public static String RepairOrderList_WSDL;
        public static String RepairLogisticsList_WSDL;

        public static string ext_address_validate_url;
        public static string uspsUsername;

        public static bool loginUserIsAdmin; 
        public enum LogLevelEnum { Error, Notice, Debug };
        public static LogLevelEnum logLevel;

        public static int cwssApiSiebelServiceUserId = 0;

        public static string siebelServiceUserId;
        public static string siebelDefaultSrOwnerId;

        private Dictionary<string, object> allServerConfig = new Dictionary<string, object>();

        public static bool configVerbPerfect;
        public static IGlobalContext _gContext;
        public static string host;
        public const string scLogProductExtSignature = "11acaed5cb6dc95783ea1d0b194347c542017bfc";
        public const string scLogProductExtName = "ACC Extention";
        public const string scLogBusinessFunction = "CX Addin";


        ConfigurationSetting(IGlobalContext gContext)
        {
            _gContext = gContext;
            logHistory = new Queue();
            iconLabelControl = new IconLabelControl(_gContext);
            clearStatusBarTimer = new System.Timers.Timer();
            clearStatusBarTimer.Interval = 5000;
            clearStatusBarTimer.Enabled = true;
            configVerbPerfect = false;
            initCWSS(_gContext);
            getCurrentHost(_gContext);
            initLog();
            getConfigVerb(_gContext);
            getSiebelServiceUserId(_gContext);
        }

        //Using lock and Singleton
        public static ConfigurationSetting Instance(IGlobalContext _gContext)
        {

                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new ConfigurationSetting(_gContext);
                    }
                    return instance;
                }
            
        }

        //Retrieve configuration verb via RNow SOAP and Parse it as JSON
        public static bool getConfigVerb(IGlobalContext _gContext)
        {
            //Init the RightNow Service
            rnSrv = new RightNowService(_gContext);
            client = rnSrv._rnowClient;
            string logMessage, logNote;

            //Query configuration from Cloud Service
            String query = "select Configuration.Value from Configuration where lookupname='CUSTOM_CFG_Accel_Ext_Integrations'";
            String[] rowData = null;
            try
            {
                rowData = rnSrv.queryData(query);
            }
            catch (Exception ex)
            {
                if (logWrap != null)
                {
                    logMessage = "Integration Configuration verb is not set properly. ";
                    logNote = "Error in query config 'CUSTOM_CFG_Accel_Ext_Integrations' from Cloud Service. Error query: " + query + "; Error Message: " + ex.Message;
                    logWrap.ErrorLog();
                }
                MessageBox.Show("Integration Configuration verb is not set properly. Please contact your system administrator. ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Check whether configuration is set
            if (rowData.Length == 0)
            {
                String message = "Integration Configuration verb does not exist. Please contact your system administrator";
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (logWrap != null)
                {
                    logMessage = "Integration Configuration verb does not exist.";
                    logNote = "CUSTOM_CFG_Accel_Ext_Integrations is missing in Configuration Settings.";
                    logWrap.ErrorLog(logMessage: logMessage, logNote: logNote);
                }
                return false;
            }
            
            // Get configuration value
            string jsonString = rowData[0];
            jsonString = jsonString.Replace("\"\"", "\"");
           
            // get the AdminProfileID
            // make sure it is located before "hosts" (if this is also missing, well, the config
            // verb is in bad shape.
            // can't use "{"AdminProfileID" because { can be followed by newline in config verb       
            if (jsonString.Contains("AdminProfileID") && 
                jsonString.Contains("hosts") &&
                jsonString.IndexOf("AdminProfileID") < jsonString.IndexOf("hosts"))
            {              
                // get the AdminProfileID value
                AdminProfileID = Convert.ToInt32(jsonString.Split(',')[0].Split(':')[1]);
            }
            else
            {
                String message = "Integration Configuration verb does not contain AdminProfileID as first member of JSON.\n Please contact your system administrator";
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (logWrap != null)
                {
                    logMessage = "Integration Configuration verb does not contain AdminProfileID as first member of JSON";
                    logNote = "The string is: " + jsonString;
                    logWrap.ErrorLog(logMessage: logMessage, logNote: logNote);
                }

                AdminProfileID = 0;
                return false;
                
            }

            loginUserIsAdmin = _gContext.ProfileId == ConfigurationSetting.AdminProfileID;
            // Parse endpoint configuration
            int i = jsonString.IndexOf("[");
            int j = jsonString.LastIndexOf("]");  
            if (i < 0 || i > j)
            {
                String message = "Siebel Integration Configuration verb is not set properly. Please contact your system administrator";
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (logWrap != null)
                {
                    logMessage = "Accelerator Integration Configuration verb [] array is not set properly.";
                    logNote = "Configuration cannot be parsed, the string is: " + jsonString;
                    logWrap.ErrorLog(logMessage: logMessage, logNote: logNote);
                }
                return false;
            }
            string jsonTrim = jsonString.Substring(i, j - i + 1);
            
            /*  Using Serializer to parse Configuration JSON String  
             *  The parsing is based on the classes definition in the following region
             */
            var serializer = new JavaScriptSerializer();
            List<ConfigurationVerb> configVerbArray = null;
            try
            {
                configVerbArray = serializer.Deserialize<List<ConfigurationVerb>>(jsonTrim);
            }
            catch (System.ArgumentException ex)
            {
                String message = "Accelerator Integration Configuration verb JSON is invalid. Please contact your system administrator";
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                logMessage = "Accelerator Integration Configuration verb JSON is invalid.";
                logNote = "Configuration cannot be parsed, the string is: " + jsonString;
                logWrap.ErrorLog(logMessage: logMessage, logNote: logNote);

                return false;
            }
            bool found = false;

            // Get the server url of current site
            string _interfaceUrl = _gContext.InterfaceURL;
            string[] splittedInterfaceUrl = _interfaceUrl.Split('/');
            string _configServerUrl = splittedInterfaceUrl[2];

            // Get values in configuration settings
            foreach (ConfigurationVerb configVerb in configVerbArray)
            {
                if (configVerb.rnt_host.Contains(_configServerUrl))
                {
                    string extBaseUrl = configVerb.integration.ext_base_url;
                    
                    //Get Siebel server type and initiate Siebel Provider
                    string serverType = configVerb.integration.server_type;
                    SiebelProvider = serverTypeToProvider(serverType);

                    username = configVerb.integration.username;
                    password = configVerb.integration.password;
                    siebelServiceUserId = configVerb.integration.siebel_service_user_id;
                    siebelDefaultSrOwnerId = configVerb.integration.siebel_default_sr_owner_id;
                    SiebelServiceTimeout = configVerb.integration.SiebelServiceTimeout;
                    log_level = configVerb.integration.log_level;
                    assignLogLevelEnum();
                    rnt_host = configVerb.rnt_host;
                    //Prepare the Service Request Type Mapping
                    if (configVerb.integration.request_type_mapping != null)
                    {
                        requestTypeMapping = setRequestTypeMapping(configVerb.integration.request_type_mapping);
                    }

                    incidentsByContactReportID = configVerb.integration.incidentsByContactReportID;
                    contactSearchReportID = configVerb.integration.contactSearchReportID;

                    uspsUsername = configVerb.postalValidation.username;
                    ext_address_validate_url = configVerb.postalValidation.ext_address_validate_url;

                    Dictionary<String, Service> extServices = configVerb.integration.ext_services;
                    foreach (KeyValuePair<String, Service> extService in extServices)
                    {
                        switch (extService.Key)
                        {
                            case "service_request_detail":
                                if (extService.Value.create != null)
                                    CreateSR_WSDL = extBaseUrl + extService.Value.create.relative_path;
                                if (extService.Value.update != null)
                                    UpdateSR_WSDL = extBaseUrl + extService.Value.update.relative_path;
                                if (extService.Value.read != null)
                                    LookupSR_WSDL = extBaseUrl + extService.Value.read.relative_path;
                                break;
                            case "service_request_list":
                                if (extService.Value.read != null)
                                    LookupSRbyContactPartyID_WSDL = extBaseUrl + extService.Value.read.relative_path;
                                break;
                            case "repair_order_detail":
                                if (extService.Value.create != null)
                                    CreateRepair_WSDL = extBaseUrl + extService.Value.create.relative_path;
                                if (extService.Value.update != null)
                                    UpdateRepair_WSDL = extBaseUrl + extService.Value.update.relative_path;
                                if (extService.Value.read != null)
                                    LookupRepair_WSDL = extBaseUrl + extService.Value.read.relative_path;
                                break;
                            case "repair_order_list":
                                if (extService.Value.read != null)
                                    RepairOrderList_WSDL = extBaseUrl + extService.Value.read.relative_path;
                                break;
                            case "service_request_note":
                                if (extService.Value.create != null)
                                    CreateInteraction_WSDL = extBaseUrl + extService.Value.create.relative_path;
                                break;
                            case "contact_list":
                                if (extService.Value.read != null)
                                    LookupContactList_WSDL = extBaseUrl + extService.Value.read.relative_path;
                                break;
                            case "item_list":
                                if (extService.Value.read != null)
                                    ItemList_WSDL = extBaseUrl + extService.Value.read.relative_path;
                                break;
                            case "entitlement_list":
                                if (extService.Value.read != null)
                                    EntitlementList_WSDL = extBaseUrl + extService.Value.read.relative_path;
                                break;
                            case "repair_logistics_list":
                                if (extService.Value.read != null)
                                    RepairLogisticsList_WSDL = extBaseUrl + extService.Value.read.relative_path;
                                break;
                            case "repair_list":
                                if (extService.Value.read != null)
                                    RepairOrderList_WSDL = extBaseUrl + extService.Value.read.relative_path;
                                break;
                        }
                    }
                    found = true; 
                    break;
                }
            }
            
            // If current site is not set in configuration.
            if (!found)
            {
                String message =  "The current host, " + _configServerUrl + ", is not present in Integration Configuration verb.\n Please contact your system administrator";
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (logWrap != null)
                {
                    logMessage = "Current Host is not present in Integration Configuration verb. ";
                    logNote = "Current site is not set in configuration CUSTOM_CFG_Accel_Ext_Integrations. The configuration verb is: " + jsonString;
                    logWrap.ErrorLog(logMessage: logMessage, logNote: logNote);
                }
                return false;
            }

            configVerbPerfect = true;
            return true;
        }

        // assign the log_level to meaningful name (enum)
        private static void assignLogLevelEnum()
        {
            switch (log_level)
            {
                case "Error":
                    logLevel = LogLevelEnum.Error;
                    break;
                case "Notice":
                    logLevel = LogLevelEnum.Notice;
                    break;
                case "Debug":
                    logLevel = LogLevelEnum.Debug;
                    break;
                default:
                    logLevel = LogLevelEnum.Debug;
                    break;
            }
        }

        // Set Provider based on configuration
        public static string serverTypeToProvider(string serverType)
        {
            string provider = "";
            serverType = serverType.ToLower();
            switch (serverType)
            {
                case "mock":
                    provider = "Accelerator.Siebel.SharedServices.Providers.MockSiebelProvider";
                    break;
                case "siebel":
                    provider = "Accelerator.Siebel.SharedServices.Providers.LiveSiebelProvider";
                    break; 
                default:
                    String message = "Siebel Integration Configuration verb is not set properly. ";
                    MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (logWrap != null)
                    {
                        string logMessage = "Provider setting in configruration is incorrect.";
                        string logNote = "server_type field in CUSTOM_CFG_Accel_Ext_Integrations is incorrect. server_type is defined as " + serverType + ", which should be MOCK or Siebel.";
                        logWrap.ErrorLog(logMessage: logMessage, logNote: logNote);
                    }
                    break;
            }
            return provider;
        }

        // Initiate Log based on configuration
        public void initLog()
        {
            Type t = Type.GetType(logClass);

            try
            {
                Log log = Activator.CreateInstance(t, scLogProductExtSignature, scLogProductExtName, scLogBusinessFunction) as Log;

                logWrap = new LogWrapper(log);

                if (logWrap != null)
                {
                    string logMessage = "Log is set up.";
                    string logNote = "Log information " + logWrap.ToString();
                    logWrap.DebugLog(logMessage: logMessage, logNote: logNote);
                }
            }
            catch (Exception)
            {
                String message = "Error in setting up log. Please contact the administrator.";
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        // Initialize CWSS
        public static void initCWSS(IGlobalContext _gContext)
        {
            rnSrv = new RightNowService(_gContext);
            client = rnSrv._rnowClient;
        }

        // Get the current host url
        public static void getCurrentHost(IGlobalContext _gContext)
        {
            // Get the server url of current site
            string _interfaceUrl = _gContext.InterfaceURL;
            string[] splittedInterfaceUrl = _interfaceUrl.Split('/');
            host = splittedInterfaceUrl[2];
        }

        // Set up Service Request Type Mapping based on configuration
        public static Dictionary<String, String> setRequestTypeMapping(List<TypeMapping> requestTypeMappings)
        {
            Dictionary<String, String> requestTypes = new Dictionary<string,string>();
            foreach(TypeMapping requestTypeMapping in requestTypeMappings){
                requestTypes.Add(requestTypeMapping.inc_type_id, requestTypeMapping.sr_type_id);
            }
            return requestTypes;
        }

        // Get the CWSS_API_Siebel_Service_User ID
        public void getSiebelServiceUserId(IGlobalContext _gContext)
        {
            string query = "select Account.ID from Account where Login='CWSS_API_Siebel_Service_User'";

            String[] rowData = null;
            try
            {
                rowData = rnSrv.queryData(query);
            }
            catch (Exception ex)
            {
                String message = "Error in query default Siebel user id from Cloud Service. Please contact the administrator.";
                if (logWrap != null)
                {
                    string logMessage = "Error in query default Siebel user id from Cloud Service. ";
                    string logNote = "Error query: " + query + "; Error message: " + ex.Message;
                    logWrap.ErrorLog(logMessage: logMessage, logNote: logNote);
                }
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                return;
            }

            // Check whether account CWSS_API_Siebel_Service_User is set in Staff
            if (rowData.Length == 0)
            {
                cwssApiSiebelServiceUserId = 0;
                return;
            }
            string jsonString = rowData[0];

            cwssApiSiebelServiceUserId = Convert.ToInt32(jsonString);
        }

        // timer event to reset the status bar
        private static void OnStatusBarResetEvent(Object src, EventArgs eea)
        {
            if (ConfigurationSetting.lastLoggedTime.AddSeconds(4) < System.DateTime.Now)
            {
                if (ConfigurationSetting.iconLabelControl.statusText.InvokeRequired)
                {
                    System.Action act = () => ConfigurationSetting.iconLabelControl.statusText.Text = "Integration Status";                 
                    ConfigurationSetting.iconLabelControl.statusText.Invoke(act);                 
                    ConfigurationSetting.iconLabelControl.statusColorBar.BackColor = System.Drawing.Color.Black;
                    clearStatusBarTimer.Stop();
                }
                else
                {
                    ConfigurationSetting.iconLabelControl.statusText.Text = "Integration Status";
                    ConfigurationSetting.iconLabelControl.statusColorBar.BackColor = System.Drawing.Color.Black;
                    clearStatusBarTimer.Stop();
                }
            }
        }

        /* update status bar color, text, and tooltip
         * called by logging
         */ 
        public static void updateStatusBar(string logLevel, string LogMessage)
        {
            if (ConfigurationSetting.loginUserIsAdmin)
            {
                if (ConfigurationSetting.iconLabelControl.statusText.InvokeRequired)
                {
                    System.Action act = () => ConfigurationSetting.iconLabelControl.statusText.Text = LogMessage;
                    ConfigurationSetting.iconLabelControl.statusText.Invoke(act);
                }
                else
                {
                    ConfigurationSetting.iconLabelControl.statusText.Text = LogMessage;
                }
            }

            if (ConfigurationSetting.logHistoryIndex < ConfigurationSetting.LOGHISTORYROW)
            {
                ConfigurationSetting.logHistory.Enqueue(logLevel + ": " + LogMessage + "  " + DateTime.Now);
                ConfigurationSetting.logHistoryIndex++;
            }
            else
            {
                ConfigurationSetting.logHistory.Dequeue();
                ConfigurationSetting.logHistory.Enqueue(logLevel + ": " + LogMessage + "  " + DateTime.Now);
            }

            lastLoggedTime = System.DateTime.Now;
            switch (logLevel)
            {
                case "Debug":
                    ConfigurationSetting.iconLabelControl.statusColorBar.BackColor = System.Drawing.Color.Green;
                    clearStatusBarTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnStatusBarResetEvent);
                    clearStatusBarTimer.Stop();
                    clearStatusBarTimer.Start();
                    break;
                case "Error":
                    ConfigurationSetting.iconLabelControl.statusColorBar.BackColor = System.Drawing.Color.Red;
                    if (!ConfigurationSetting.configVerbPerfect && !loginUserIsAdmin)
                    {
                        ConfigurationSetting.iconLabelControl.statusText.Text = "All Accelerator Add-Ins failed. Click here to exit.";
                    }
                    clearStatusBarTimer.Stop();
                    break;
                case "Notice":
                    ConfigurationSetting.iconLabelControl.statusColorBar.BackColor = System.Drawing.Color.Yellow;
                    //clearStatusBarTimer.Tick += new EventHandler(OnStatusBarResetEvent);
                    clearStatusBarTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnStatusBarResetEvent);
                    clearStatusBarTimer.Stop();
                    clearStatusBarTimer.Start();
                    break;
            }

        }

        public static void logAssemblyVersion(Assembly assembly, LogWrapper aLog)
        {
            // Log the build version of this assembly
            LogWrapper log = (null == aLog) ? ConfigurationSetting.logWrap : aLog;
            AssemblyName assemblyName = assembly.GetName();
            StringBuilder sb = new StringBuilder(256);
            sb.Append(assemblyName.Name)
                .Append(" ")
                .Append(assemblyName.Version.ToString());
            string logMessage = sb.ToString();
            string logNote = null;
            log.DebugLog(logMessage: logMessage, logNote: logNote);
        }
        
    }

    #region Classes for Json Parsing

    public class Action
    {
        public string service_name { get; set; }
        public string soap_action { get; set; }
        public string relative_path { get; set; }
    }

    public class Service
    {
        public Action read { get; set; }
        public Action create { get; set; }
        public Action update { get; set; }
    }

    public class TypeMapping
    {
        public String inc_type_id;
        public String sr_type_id;
    }

    public class Integration
    {
        public string server_type { get; set; }
        public string log_type { get; set; }
        public string ext_base_url { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string siebel_service_user_id { get; set; }
        public string siebel_default_sr_owner_id { get; set; }
        public int SiebelServiceTimeout { get; set; }
        public int AdminProfileID { get; set; }
        public string log_level { get; set; }
        public int incidentsByContactReportID { get; set; }
        public int contactSearchReportID { get; set; }
        public Dictionary<String, Service> ext_services { get; set; }
        public List<TypeMapping> request_type_mapping{ get; set; }
    }

    public class PostalValidation
    {
        public string ext_address_validate_url { get; set; }
        public string username { get; set; }
    }

    public class ConfigurationVerb
    {
        public string rnt_host { get; set; }
        public Integration integration { get; set; }
        public PostalValidation postalValidation { get; set; }
    }
#endregion
}