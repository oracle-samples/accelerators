/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  Copyright (c) 2014, 2015, 2016, 2017 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + SRM Integration Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 17.2 (February 2017) 
 *  reference: 160628-000117
 *  date: Fri Feb 10 19:47:42 PST 2017
 
 *  revision: rnw-17-2-fixes-release-2
 *  SHA1: $Id: 7746e1c8777033c0463d31648bd57af48e20165f $
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
using Accelerator.SRM.SharedServices.Logs;
using Accelerator.SRM.SharedServices.RightNowServiceReference;
using RightNow.AddIns.AddInViews;
using System.Threading.Tasks;

namespace Accelerator.SRM.SharedServices
{
    /// <summary>
    /// Singleton class used to retrieve configuration settings and to maintain parameters across add-ins.
    /// </summary>
    public sealed class ConfigurationSetting
    {
        #region Properties

        #region Singleton Config

        /// <summary>
        /// Singleton istance
        /// </summary>
        private static ConfigurationSetting instance = null;

        /// <summary>
        /// Padlock
        /// </summary>
        public static readonly object padlock = new object();

        #endregion

        #region OSvC Properties

        public static RightNowService rnSrv;
        public static RightNowSyncPortClient client;

        #endregion

        #region Status Bar Properties

        // for status bar
        public const int LOGHISTORYROW = 10;
        public static IconLabelControl iconLabelControl;
        public static Queue logHistory;
        public static int logHistoryIndex = 0;
        public static System.Timers.Timer clearStatusBarTimer;
        public static DateTime lastLoggedTime;

        #endregion

        #region Logger Properties

        public static LogWrapper logWrap;
        // You can change the log class to your own log implementation to replace DefaultLog
        // For Example: public static string logClass = "Accelerator.SRM.SharedServices.Logs.SCLogWrapper";
        public static string logClass = "Accelerator.SRM.SharedServices.Logs.DefaultLog";
        public static String rnt_host;
        public static String convReplyGETEndpoint;
        public static String convReplyPOSTEndpoint;
        public static String convPUTEndpoint;
        public static List<ChannelConstraints> convChannelConstraints;
        public static string log_level;
        public static int max_srm_rows_fetch = 200; // default if config verb is missing this setting
        public static bool loginUserIsAdmin;
        
        public enum LogLevelEnum { Error, Notice, Debug, Click };
        public static LogLevelEnum logLevel;

        #endregion

        #region Server Side Config Properties

        private Dictionary<string, object> allServerConfig = new Dictionary<string, object>();
        public static bool configVerbPerfect;
        public static IGlobalContext _gContext;

        public static string host;
        public const string scLogProductExtSignature = "66acaed5cb6dc95783ea1d0b194347c542017bfc";
        public const string scLogProductExtName = "ACC Extention";
        public const string scLogBusinessFunction = "CX Addin";

        public static long openConversationStatusId = 1;
        public static long closedConversationStatusId = 2;

        #endregion

        #region SRM Properties

        /// <summary>
        /// Array to store the one or more channel IDs for engage.
        /// </summary>
        public static long[] SrmEngageChannelId;

        /// <summary>
        /// Field to cache the known auth config state for SRM.
        /// This is read only to other classes.  ValidateSRMAuthorizationTokenConfigs must be called to set this value.
        /// </summary>
        private static bool _SRMAuthTokenConfigsExist = false;
        public static bool SRMAuthTokenConfigsExist
        {
            get
            {
                return _SRMAuthTokenConfigsExist;
            }
        }

        #endregion

        #endregion

        #region Constructors

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
            SrmEngageChannelId = new long[10];
            getNamedIds();
        }

        #endregion

        #region Singleton Setup

        /// <summary>
        /// Locker method for singleton instance
        /// </summary>
        /// <param name="_gContext"></param>
        /// <returns></returns>
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

        #endregion

        #region CWS Setup

        /// <summary>
        /// Intializes connect web services
        /// </summary>
        /// <param name="_gContext"></param>
        public static void initCWSS(IGlobalContext _gContext)
        {
            rnSrv = new RightNowService(_gContext);
            client = rnSrv._rnowClient;
        }

        /// <summary>
        /// Get the current host URL from the Global Context
        /// </summary>
        /// <param name="_gContext"></param>
        public static void getCurrentHost(IGlobalContext _gContext)
        {
            // Get the server url of current site
            string _interfaceUrl = _gContext.InterfaceURL;
            string[] splittedInterfaceUrl = _interfaceUrl.Split('/');
            host = splittedInterfaceUrl[2];
        }

        /// <summary>
        /// Get the status and channel IDs
        /// </summary>
        public static void getNamedIds()
        {
            ClientInfoHeader clientInfoHeader = new ClientInfoHeader();
            clientInfoHeader.AppID = "Get NamedIDs";

            NamedID[] valuesForNamedID = client.GetValuesForNamedID(clientInfoHeader, null, "Incident.StatusWithType.Status");
               
                //Display the Name and Id properties for each entry
                foreach (NamedID namedID in valuesForNamedID)
                {
                    if (namedID.Name.Equals("Open Conversation"))
                    {
                        openConversationStatusId = namedID.ID.id;
                    }
                    if (namedID.Name.Equals("Closed Conversation"))
                    {
                        closedConversationStatusId = namedID.ID.id;
                    }
                }

                valuesForNamedID = client.GetValuesForNamedID(clientInfoHeader, null, "Incident.Channel");
                int i = 0;
                //Display the Name and Id properties for each entry
                foreach (NamedID namedID in valuesForNamedID)
                {
                    if (namedID.Name.Equals("SRM Engage"))
                    {
                        SrmEngageChannelId[i] = namedID.ID.id;
                        i++;
                    }
                }           
        }

        /// <summary>
        /// get custom attribute on Incident Workspace
        /// </summary>
        /// <param name="incidentRecord"></param>
        /// <param name="attrName"></param>
        /// <returns></returns>
        public static int getSrmCustomAttr(IIncident incidentRecord, String attrName)
        {
            attrName = "Accelerator$" + attrName;
            IList<ICustomAttribute> customAttributes = incidentRecord.CustomAttributes;
            foreach (ICustomAttribute cusAttr in customAttributes)
            {
                if (cusAttr.PackageName == "Accelerator" && cusAttr.GenericField.Name == attrName)
                    return cusAttr.GenericField.DataValue.Value != null ? (int)cusAttr.GenericField.DataValue.Value : 0;
            }
            return 0;
        }

        /// <summary>
        /// Get a string custom attribute
        /// </summary>
        /// <param name="incidentRecord"></param>
        /// <param name="attrName"></param>
        /// <returns></returns>
        public static String getSrmStringCustomAttr(IIncident incidentRecord, String attrName)
        {
            attrName = "Accelerator$" + attrName;
            IList<ICustomAttribute> customAttributes = incidentRecord.CustomAttributes;
            foreach (ICustomAttribute cusAttr in customAttributes)
            {
                if (cusAttr.PackageName == "Accelerator" && cusAttr.GenericField.Name == attrName)
                    return cusAttr.GenericField.DataValue.Value != null ? (String)cusAttr.GenericField.DataValue.Value : null;
            }
            logWrap.DebugLog(logMessage: "Custom Attribute Accelerator$" + attrName + " not exist!!!!");
            return null;
        }

        /// <summary>
        /// Get an int custom attribute
        /// </summary>
        /// <param name="incidentRecord"></param>
        /// <param name="attrName"></param>
        /// <returns></returns>
        public static int getSrmStringCustomAttrInt(IIncident incidentRecord, String attrName)
        {
            attrName = "Accelerator$" + attrName;
            IList<ICustomAttribute> customAttributes = incidentRecord.CustomAttributes;
            foreach (ICustomAttribute cusAttr in customAttributes)
            {
                if (cusAttr.PackageName == "Accelerator" && cusAttr.GenericField.Name == attrName)
                    return cusAttr.GenericField.DataValue.Value != null ? (int)cusAttr.GenericField.DataValue.Value : 0;
            }
            logWrap.DebugLog(logMessage: "Custom Attribute Accelerator$" + attrName + " not exist!!!!");
            return 0;
        }
        
        /// <summary>
        /// Retrieve configuration verb via RNow SOAP and Parse it as JSON
        /// </summary>
        /// <param name="_gContext">The add-in global context</param>
        /// <returns></returns>
        public static bool getConfigVerb(IGlobalContext _gContext)
        {
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
                    logWrap.ErrorLog(logMessage: logMessage, logNote: logNote);
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
            jsonString = jsonString.TrimStart('"');
            jsonString = jsonString.TrimEnd('"');
            JavaScriptSerializer ser = new JavaScriptSerializer();
            ConfigVerb configVerb = null;

            try
            {
                configVerb = ser.Deserialize<ConfigVerb>(jsonString);
            }
            catch (System.ArgumentException ex)
            {
                String message = "Accelerator Integration Configuration verb JSON is invalid. Please contact your system administrator.";
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                logMessage = "Accelerator Integration Configuration verb JSON is invalid.";
                logNote = "Configuration cannot be parsed, the string is: " + jsonString;
                logWrap.ErrorLog(logMessage: logMessage, logNote: logNote);

                System.Diagnostics.Debug.WriteLine(ex.Message);

                return false;
            }

            if (configVerb.AdminProfileID == 0)
            {
                String message = "Integration Configuration verb does not contain AdminProfileID as first member of JSON.\n Please contact your system administrator";
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (logWrap != null)
                {
                    logMessage = "Integration Configuration verb does not contain AdminProfileID as first member of JSON";
                    logNote = "The string is: " + jsonString;
                    logWrap.ErrorLog(logMessage: logMessage, logNote: logNote);
                }
                return false;
            }

            loginUserIsAdmin = _gContext.ProfileId == configVerb.AdminProfileID;           
            bool found = false;

            // Get the server url of current site
            string _interfaceUrl = _gContext.InterfaceURL;
            string[] splittedInterfaceUrl = _interfaceUrl.Split('/');
            string _configServerUrl = splittedInterfaceUrl[2];

            // Get values in configuration settings
            foreach (Host host in configVerb.hosts)
            {
                if (host.rnt_host.Contains(_configServerUrl))
                {
                    string extBaseUrl = host.integration.ext_base_url;
                                       
                    log_level = host.integration.log_level;
                    max_srm_rows_fetch = host.integration.max_srm_rows_fetch;
                    assignLogLevelEnum();
                    rnt_host = host.rnt_host;
                    convReplyPOSTEndpoint = host.integration.ext_base_url + host.integration.ext_services.conversation_reply.create.relative_path;
                    convReplyGETEndpoint = host.integration.ext_base_url + host.integration.ext_services.conversation_reply.list.relative_path;
                    convPUTEndpoint = host.integration.ext_base_url + host.integration.ext_services.conversation.update.relative_path;
                    convChannelConstraints = host.integration.channel_constraints;
                    found = true; 
                    break;
                }
            }
            
            // If current site is not set in configuration.
            if (!found)
            {
                String message = "The current host, " + _configServerUrl + ", is not present in Integration Configuration verb.\n Please contact your system administrator";
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

        #endregion

        #region Log Methods

        /// <summary>
        /// Assign the log_level to meaningful name (enum)
        /// </summary>
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
                case "Click":
                    logLevel = LogLevelEnum.Click;
                    break;
                default:
                    logLevel = LogLevelEnum.Debug;
                    break;
            }
        }

        /// <summary>
        /// Initiate Log based on configuration
        /// </summary>
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

        /// <summary>
        /// Log the assembly verion
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="aLog"></param>
        /// <returns></returns>
        public static string logAssemblyVersion(Assembly assembly, LogWrapper aLog)
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
            return logMessage;
        }

        #endregion

        #region Status Bar Events

        /// <summary>
        /// timer event to reset the status bar
        /// </summary>
        /// <param name="src"></param>
        /// <param name="eea"></param>
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

        /// <summary>
        /// Update status bar color, text, and tooltip
        /// Called by logging class.
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="LogMessage"></param>
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
                case "Click":
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
                    clearStatusBarTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnStatusBarResetEvent);
                    clearStatusBarTimer.Stop();
                    clearStatusBarTimer.Start();
                    break;
            }
        }

        #endregion

        #region SRM Configuration Methods

        /// <summary>
        /// Method that will validate that an active access token record is in the database.
        /// If not, then we assume that the admin of this site has not yet setup the SRM OAuth configs.
        /// </summary>
        /// <param name="accountId">Account ID of the logged in user</param>
        /// <returns>Tuple of bools. The first result is for configs and the second is for tokens.</returns>
        public async Task<Tuple<bool,bool>> ValidateSRMAuthorizationTokenConfigs(int accountId)
        {
            var configCheck = false;
            var tokenCheck = false;

            var stopwatch = new System.Diagnostics.Stopwatch();
            try
            {
                stopwatch.Start();

                var configQuery = "SELECT COUNT(ID) FROM Accelerator.OAuthConfig";
                var results = await rnSrv.queryDataAsync(configQuery);

                if (results is string[] && results.Length > 0)
                {
                    var count = Convert.ToInt32(results[0]);
                    if (count > 0)
                    {
                        configCheck = true;
                        _SRMAuthTokenConfigsExist = true;
                    }
                }

                var tokenQuery = String.Format("SELECT COUNT(ID) FROM Accelerator.OAuthTokens WHERE Account = {0} AND RefreshTokenFailure != 1", accountId);
                var tokenResults = await rnSrv.queryDataAsync(tokenQuery);

                if (tokenResults is string[] && tokenResults.Length > 0)
                {
                    var count = Convert.ToInt32(tokenResults[0]);
                    if (count > 0)
                    {
                        tokenCheck = true;
                        _SRMAuthTokenConfigsExist = true;
                    }
                }

                stopwatch.Stop();

                if(logWrap != null)
                    logWrap.DebugLog(logMessage: Properties.Resources.ValidatingServerConfigsLabel, logNote: Properties.Resources.ValidatingServerConfigsLabel, timeElapsed: (int)stopwatch.ElapsedMilliseconds);

                return new Tuple<bool, bool>(configCheck, tokenCheck);
            }
            catch (Exception ex)
            {
                if (logWrap != null)
                    logWrap.DebugLog(logMessage: Properties.Resources.ValidatingServerConfigsError, logNote: ex.Message);
            }

            _SRMAuthTokenConfigsExist = false;
            return null;
        }

        #endregion

    }
}