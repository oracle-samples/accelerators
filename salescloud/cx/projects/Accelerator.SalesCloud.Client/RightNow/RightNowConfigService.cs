/* * *******************************************************************************************
*  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015,2016 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + OSC Lead Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.11 (November 2015)
 *  OSC release: Release 10
 *  reference: 150505-000122, 160620-000160
 *  date: Mon Sep 19 02:05:27 PDT 2016

 *  revision: rnw-15-11-fixes-release-3
*  SHA1: $Id: fc103271a3945acaceba57dd5b9eca1b4e9c1345 $
* *********************************************************************************************
*  File: RightNowConfigService.cs
* ****************************************************************************************** */

using Accelerator.SalesCloud.Client.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using System.Web.Services.Description;

namespace Accelerator.SalesCloud.Client.RightNow
{
    public class RightNowConfigService
    {
        private static bool _isConfigured = false;
        private static object _sync = new object();
        private static IRightNowConnectService _rightNowService;
        private static Dictionary<string, string> _registeredConfigVerbs;

        internal static bool Config()
        {
            lock (_sync)
            {
                if (!_isConfigured)
                {
                    try
                    {
                        // Register config verbs
                        RegisterConfigVerb();

                        // Get right now connect service
                        _rightNowService = RightNowConnectService.GetService();

                        // Initialized Configverbs
                        var isCfgReceived = InitializedConfigVerbs();

                        if (isCfgReceived)
                        {
                            // Parse 'CUSTOM_CFG_Sales_Accel_Integrations' config verb
                            _isConfigured = ParseCustomCfgAccelExtIntegrations();
                            if (!_isConfigured)
                            {
                                MessageBox.Show(OSCExceptionMessages.ConfigVerbIsNotSetOrIncorrect, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        _isConfigured = false;
                        MessageBox.Show(OSCExceptionMessages.ConfigurationNotInitialized, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            return _isConfigured;
        }

        private static bool InitializedConfigVerbs()
        {

            // set 'CUSTOM_CFG_Sales_Accel_Integrations' config verb 
            var salesIntCfgVerbValue = _rightNowService.GetRightNowConfigVerbValue(RightNowConfigKeyNames.CustomCfgSalesAccelIntegrations);
            if (salesIntCfgVerbValue == null || salesIntCfgVerbValue.Trim().Equals(""))
            {
                MessageBox.Show(OSCExceptionMessages.ConfigVerbIsNotSetOrIncorrect, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else
            {
                _registeredConfigVerbs[RightNowConfigKeyNames.CustomCfgSalesAccelIntegrations] = salesIntCfgVerbValue;
            }            

            return true;
        }

        private static bool ParseCustomCfgAccelExtIntegrations()
        {
            bool isParsed = false;
            try
            {
                string errorList = "";

                // Get configuration value
                string jsonString = GetConfigValue(RightNowConfigKeyNames.CustomCfgSalesAccelIntegrations);

                var jsonTrim = jsonString.Replace("\"\"", "\"");

                // jsonString has extra " at start, end and each " 
                int i = jsonTrim.IndexOf("\"");
                int j = jsonTrim.LastIndexOf("\"");
                var finalJson = jsonTrim.Substring(i + 1, j - 1);

                var s = new JavaScriptSerializer();

                var configVerb = s.Deserialize<OSCConfigVerb>(finalJson);

                // set 'rn_host' config verb
                if (configVerb.rnt_host == null || configVerb.rnt_host.Trim().Equals(""))
                    errorList = errorList + RightNowConfigKeyNames.RightnowHost;
                else
                    _registeredConfigVerbs[RightNowConfigKeyNames.RightnowHost] = configVerb.rnt_host;

                // set 'server_type' config verb
                if (configVerb.integration.server_type == null || configVerb.integration.server_type.Trim().Equals(""))
                    errorList = errorList + " , " + RightNowConfigKeyNames.ServerType;
                else
                    _registeredConfigVerbs[RightNowConfigKeyNames.ServerType] = configVerb.integration.server_type;

                // set 'ext_base_url' config verb
                if (configVerb.integration.sales_base_url == null || configVerb.integration.sales_base_url.Trim().Equals(""))
                    errorList = errorList + " , " + RightNowConfigKeyNames.SalesBaseUrl;
                else
                    _registeredConfigVerbs[RightNowConfigKeyNames.SalesBaseUrl] = configVerb.integration.sales_base_url;

                // set 'username' config verb
                if (configVerb.integration.username == null || configVerb.integration.username.Trim().Equals(""))
                    errorList = errorList + " , " + RightNowConfigKeyNames.UserName;
                else
                    _registeredConfigVerbs[RightNowConfigKeyNames.UserName] = configVerb.integration.username;

                // set 'password' config verb
                if (configVerb.integration.password == null || configVerb.integration.password.Trim().Equals(""))
                    errorList = errorList + "  " + RightNowConfigKeyNames.Password;
                else
                    _registeredConfigVerbs[RightNowConfigKeyNames.Password] = configVerb.integration.password;

                // set 'sales_timeout' config verb
                if (configVerb.integration.sales_timeout == null || configVerb.integration.sales_timeout.Trim().Equals(""))
                    errorList = errorList + "  " + RightNowConfigKeyNames.SalesTimeout;
                else
                    _registeredConfigVerbs[RightNowConfigKeyNames.SalesTimeout] = configVerb.integration.sales_timeout;

                // set 'oppty_service_url' config verb
                if (configVerb.integration.oppty_service_url == null || configVerb.integration.oppty_service_url.Trim().Equals(""))
                    errorList = errorList + "  " + RightNowConfigKeyNames.OSCOpptyServiceUrl;
                else
                    _registeredConfigVerbs[RightNowConfigKeyNames.OSCOpptyServiceUrl] = configVerb.integration.oppty_service_url;

                // set 'acct_service_url' config verb
                if (configVerb.integration.acct_service_url == null || configVerb.integration.acct_service_url.Trim().Equals(""))
                    errorList = errorList + "  " + RightNowConfigKeyNames.OSCAcctServiceUrl;
                else
                    _registeredConfigVerbs[RightNowConfigKeyNames.OSCAcctServiceUrl] = configVerb.integration.acct_service_url;

                // set 'lead_service_url' config verb
                if (configVerb.integration.lead_service_url == null || configVerb.integration.lead_service_url.Trim().Equals(""))
                    errorList = errorList + "  " + RightNowConfigKeyNames.OSCLeadServiceUrl;
                else
                    _registeredConfigVerbs[RightNowConfigKeyNames.OSCLeadServiceUrl] = configVerb.integration.lead_service_url;

                // set 'sales_max_rcvd_msg_size' config verb
                if (configVerb.integration.sales_max_rcvd_msg_size == null || configVerb.integration.sales_max_rcvd_msg_size.Trim().Equals(""))
                    errorList = errorList + "  " + RightNowConfigKeyNames.SalesMaxReceivedMessageSize;
                else
                    _registeredConfigVerbs[RightNowConfigKeyNames.SalesMaxReceivedMessageSize] = configVerb.integration.sales_max_rcvd_msg_size;
                
                // set 'oppty_lead_type' config verb
                if (configVerb.integration.oppty_lead_type == null || configVerb.integration.oppty_lead_type.Trim().Equals(""))
                    errorList = errorList + "  " + RightNowConfigKeyNames.OpportunityLeadType;
                else
                    _registeredConfigVerbs[RightNowConfigKeyNames.OpportunityLeadType] = configVerb.integration.oppty_lead_type;

                // set 'ctc_service_url' config verb
                if (configVerb.integration.ctc_service_url == null || configVerb.integration.ctc_service_url.Trim().Equals(""))
                    errorList = errorList + "  " + RightNowConfigKeyNames.OSCContactServiceUrl;
                else
                    _registeredConfigVerbs[RightNowConfigKeyNames.OSCContactServiceUrl] = configVerb.integration.ctc_service_url;

                if (errorList.Equals(""))
                {
                    isParsed = true;
                }

            }
            catch (Exception e)
            {
                isParsed = false;
                MessageBox.Show(OSCExceptionMessages.ConfigVerbIsNotSetOrIncorrect, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return isParsed;
        }



        /// <summary>
        /// Register new config verb
        /// </summary>
        private static void RegisterConfigVerb()
        {
            if (!_isConfigured)
            {
                _registeredConfigVerbs = new Dictionary<string, string>();
                _registeredConfigVerbs.Add(RightNowConfigKeyNames.CustomCfgSalesAccelIntegrations, "");                
                _registeredConfigVerbs.Add(RightNowConfigKeyNames.OSCAcctServiceUrl, "");
                _registeredConfigVerbs.Add(RightNowConfigKeyNames.OSCLeadServiceUrl, "");
                _registeredConfigVerbs.Add(RightNowConfigKeyNames.OSCOpptyServiceUrl, "");                
                _registeredConfigVerbs.Add(RightNowConfigKeyNames.SalesBaseUrl, "");
                _registeredConfigVerbs.Add(RightNowConfigKeyNames.SalesTimeout, "");                
                _registeredConfigVerbs.Add(RightNowConfigKeyNames.Password, "");
                _registeredConfigVerbs.Add(RightNowConfigKeyNames.RightnowHost, "");
                _registeredConfigVerbs.Add(RightNowConfigKeyNames.ServerType, "");
                _registeredConfigVerbs.Add(RightNowConfigKeyNames.UserName, "");
                _registeredConfigVerbs.Add(RightNowConfigKeyNames.SalesMaxReceivedMessageSize, "");                
                _registeredConfigVerbs.Add(RightNowConfigKeyNames.OpportunityLeadType, "");
                _registeredConfigVerbs.Add(RightNowConfigKeyNames.OSCContactServiceUrl, "");

                // Add all predefined config verb.
            }

        }

        /// <summary>
        /// Return Config value if config verb is registered, otherwise return NULL 
        /// </summary>
        /// <param name="configKey"></param>
        /// <returns></returns>
        public static string GetConfigValue(string configKey)
        {
            string value = null;
            if (_registeredConfigVerbs != null)
            {
                _registeredConfigVerbs.TryGetValue(configKey, out value);
            }
            return value;
        }

        /// <summary>
        /// Check if configuration is set correctly
        /// </summary>
        /// <returns></returns>
        public static bool IsConfigured()
        {
            return true;
        }
    }
}
