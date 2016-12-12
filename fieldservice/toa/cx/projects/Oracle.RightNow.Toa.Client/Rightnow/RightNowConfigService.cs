/* * *******************************************************************************************
*  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + OFSC Reference Integration
 *  link: http://www-content.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.2 (Feb 2015)
 *  OFSC release: 15.2 (Feb 2015)
 *  reference: 150622-000130
 *  date: Thu Sep  3 23:14:02 PDT 2015

 *  revision: rnw-15-11-fixes-release-03
*  SHA1: $Id: e170dae41422449baddb215d7f0601e5b9a071dd $
* *********************************************************************************************
*  File: RightNowConfigService.cs
* ****************************************************************************************** */

using System;
using System.Collections.Generic;
using System.Web.Services.Description;
using System.Windows.Forms;
using Oracle.RightNow.Toa.Client.Common;
using System.Web.Script.Serialization;
using Oracle.RightNow.Toa.Client.Exceptions;

namespace Oracle.RightNow.Toa.Client.Rightnow
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
                            // Parse 'CUSTOM_CFG_FS_Accel_Integrations' config verb
                            _isConfigured = ParseCustomCfgAccelExtIntegrations();
                            if (!_isConfigured)
                            {
                                MessageBox.Show(ToaExceptionMessages.ConfigVerbIsNotSetOrIncorrect, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        _isConfigured = false;
                        MessageBox.Show(ToaExceptionMessages.ConfigurationNotInitialized, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            return _isConfigured;
        }

        private static bool InitializedConfigVerbs()
        {           

            // set 'CUSTOM_CFG_FS_Accel_Integrations' config verb 
            var fsIntCfgVerbValue = _rightNowService.GetRightNowConfigVerbValue(RightNowConfigKeyNames.CustomCfgFsAccelIntegrations);
            if (fsIntCfgVerbValue == null || fsIntCfgVerbValue.Trim().Equals(""))
            {
                MessageBox.Show(ToaExceptionMessages.ConfigVerbIsNotSetOrIncorrect, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else
            {
                _registeredConfigVerbs[RightNowConfigKeyNames.CustomCfgFsAccelIntegrations] = fsIntCfgVerbValue;
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
                string jsonString = GetConfigValue(RightNowConfigKeyNames.CustomCfgFsAccelIntegrations);

                var jsonTrim = jsonString.Replace("\"\"", "\"");

                // jsonString has extra " at start, end and each " 
                int i = jsonTrim.IndexOf("\"");
                int j = jsonTrim.LastIndexOf("\"");
                var finalJson = jsonTrim.Substring(i + 1, j - 1);

                var s = new JavaScriptSerializer();

                var configVerb = s.Deserialize<ToaConfigVerbs>(finalJson);

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
                if (configVerb.integration.fs_base_url == null || configVerb.integration.fs_base_url.Trim().Equals(""))
                    errorList = errorList + " , " + RightNowConfigKeyNames.FsBaseUrl;
                else
                    _registeredConfigVerbs[RightNowConfigKeyNames.FsBaseUrl] = configVerb.integration.fs_base_url;

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

                // set 'company_name' config verb
                if (configVerb.integration.company_name == null || configVerb.integration.company_name.Trim().Equals(""))
                    errorList = errorList + "  " + RightNowConfigKeyNames.CompanyName;
                else
                    _registeredConfigVerbs[RightNowConfigKeyNames.CompanyName] = configVerb.integration.company_name;

                // set 'fallback_id' config verb
                if (configVerb.integration.fallback_id == null || configVerb.integration.fallback_id.Trim().Equals(""))
                    errorList = errorList + "  " + RightNowConfigKeyNames.FallbackId;
                else
                    _registeredConfigVerbs[RightNowConfigKeyNames.FallbackId] = configVerb.integration.fallback_id;

                // set 'FSServiceTimeout' config verb
                if (configVerb.integration.FSServiceTimeout == null || configVerb.integration.FSServiceTimeout.Trim().Equals(""))
                    errorList = errorList + "  " + RightNowConfigKeyNames.FSServiceTimeout;
                else
                    _registeredConfigVerbs[RightNowConfigKeyNames.FSServiceTimeout] = configVerb.integration.FSServiceTimeout;

                // set 'activity_api_url' config verb
                if (configVerb.integration.activity_api_url == null || configVerb.integration.activity_api_url.Trim().Equals(""))
                    errorList = errorList + "  " + RightNowConfigKeyNames.ToaActivityServiceUrl;
                else
                    _registeredConfigVerbs[RightNowConfigKeyNames.ToaActivityServiceUrl] = configVerb.integration.activity_api_url;

                // set 'inbound_api_url' config verb
                if (configVerb.integration.inbound_api_url == null || configVerb.integration.inbound_api_url.Trim().Equals(""))
                    errorList = errorList + "  " + RightNowConfigKeyNames.ToaInboundServiceUrl;
                else
                    _registeredConfigVerbs[RightNowConfigKeyNames.ToaInboundServiceUrl] = configVerb.integration.inbound_api_url;

                // set 'capacity_api_url' config verb
                if (configVerb.integration.capacity_api_url == null || configVerb.integration.capacity_api_url.Trim().Equals(""))
                    errorList = errorList + "  " + RightNowConfigKeyNames.ToaCapacityServiceUrl;
                else
                    _registeredConfigVerbs[RightNowConfigKeyNames.ToaCapacityServiceUrl] = configVerb.integration.capacity_api_url;

                // set 'history_api_url' config verb
                if (configVerb.integration.history_api_url == null || configVerb.integration.history_api_url.Trim().Equals(""))
                    errorList = errorList + "  " + RightNowConfigKeyNames.ToaHistoryServiceUrl;
                else
                    _registeredConfigVerbs[RightNowConfigKeyNames.ToaHistoryServiceUrl] = configVerb.integration.history_api_url;                             

                // set 'red_quota_cutoff' config verb
                if (configVerb.integration.red_quota_cutoff == null || configVerb.integration.red_quota_cutoff.Trim().Equals(""))
                    errorList = errorList + "  " + RightNowConfigKeyNames.RedQuotaCutoff;
                else
                    _registeredConfigVerbs[RightNowConfigKeyNames.RedQuotaCutoff] = configVerb.integration.red_quota_cutoff;

                // set 'green_quota_cutoff' config verb
                if (configVerb.integration.green_quota_cutoff == null || configVerb.integration.green_quota_cutoff.Trim().Equals(""))
                    errorList = errorList + "  " + RightNowConfigKeyNames.GreenQuotaCutoff;
                else
                    _registeredConfigVerbs[RightNowConfigKeyNames.GreenQuotaCutoff] = configVerb.integration.green_quota_cutoff;

                if (errorList.Equals(""))
                {
                    isParsed = true;
                }                

            }
            catch (Exception e)
            {
                isParsed = false;
                MessageBox.Show(ToaExceptionMessages.ConfigVerbIsNotSetOrIncorrect, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                _registeredConfigVerbs.Add(RightNowConfigKeyNames.CustomCfgFsAccelIntegrations, "");                
                _registeredConfigVerbs.Add(RightNowConfigKeyNames.ToaCapacityServiceUrl, "");
                _registeredConfigVerbs.Add(RightNowConfigKeyNames.ToaActivityServiceUrl, "");
                _registeredConfigVerbs.Add(RightNowConfigKeyNames.ToaHistoryServiceUrl, "");
                _registeredConfigVerbs.Add(RightNowConfigKeyNames.ToaInboundServiceUrl, "");
                _registeredConfigVerbs.Add(RightNowConfigKeyNames.FsBaseUrl, "");
                _registeredConfigVerbs.Add(RightNowConfigKeyNames.FSServiceTimeout, "");
                _registeredConfigVerbs.Add(RightNowConfigKeyNames.FallbackId, "");
                _registeredConfigVerbs.Add(RightNowConfigKeyNames.Password, "");
                _registeredConfigVerbs.Add(RightNowConfigKeyNames.RightnowHost, "");
                _registeredConfigVerbs.Add(RightNowConfigKeyNames.ServerType, "");
                _registeredConfigVerbs.Add(RightNowConfigKeyNames.UserName, "");
                _registeredConfigVerbs.Add(RightNowConfigKeyNames.CompanyName, "");                
                _registeredConfigVerbs.Add(RightNowConfigKeyNames.RedQuotaCutoff, "");
                _registeredConfigVerbs.Add(RightNowConfigKeyNames.GreenQuotaCutoff, "");

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
            return _isConfigured;
        }

    }
}
