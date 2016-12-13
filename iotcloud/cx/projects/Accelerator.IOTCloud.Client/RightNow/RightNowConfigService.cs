/* * *******************************************************************************************
*  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: IoT OSvC Bi-directional Integration Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 16.11 (November 2016) 
 *  reference: 151217-000026
 *  date: Tue Dec 13 13:23:39 PST 2016
 
 *  revision: rnw-16-11-fixes-release
*  SHA1: $Id: 7b7c37d852dfbb659b5fde559953a0388246d42e $
* *********************************************************************************************
*  File: RightNowConfigService.cs
* ****************************************************************************************** */

using Accelerator.IOTCloud.Client.Logs;
using Accelerator.IOTCloud.Client.Model;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Accelerator.IOTCloud.Client.Model.Common;

namespace Accelerator.IOTCloud.Client.RightNow
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
                                LogService.GetLog().Error(ExceptionMessages.CONFIG_VERB_IS_NOT_SET_OR_INCORRECT);
                                MessageBox.Show("_isConfigured is false");
                                MessageBox.Show(ExceptionMessages.CONFIG_VERB_IS_NOT_SET_OR_INCORRECT, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        _isConfigured = false;
                        LogService.GetLog().Error(ExceptionMessages.CONFIGURATION_NOT_INITIALIZED, e.StackTrace);
                        MessageBox.Show(ExceptionMessages.CONFIGURATION_NOT_INITIALIZED, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            return _isConfigured;
        }

        private static bool InitializedConfigVerbs()
        {

            // set 'CUSTOM_CFG_IOT_ACCEL_INTEGRATIONS' config verb 
            var iotIntCfgVerbValue = _rightNowService.GetRightNowConfigVerbValue(RightNowConfigKeyNames.CUSTOM_CFG_IOT_ACCEL_INTEGRATIONS);
            if (iotIntCfgVerbValue == null || iotIntCfgVerbValue.Trim().Equals(""))
            {
                LogService.GetLog().Error(ExceptionMessages.CONFIG_VERB_IS_NOT_SET_OR_INCORRECT);
                MessageBox.Show(ExceptionMessages.CONFIG_VERB_IS_NOT_SET_OR_INCORRECT, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else
            {
                _registeredConfigVerbs[RightNowConfigKeyNames.CUSTOM_CFG_IOT_ACCEL_INTEGRATIONS] = iotIntCfgVerbValue;
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
                string jsonString = GetConfigValue(RightNowConfigKeyNames.CUSTOM_CFG_IOT_ACCEL_INTEGRATIONS);

                var jsonTrim = jsonString.Replace("\"\"", "\"");

                // jsonString has extra " at start, end and each " 
                int i = jsonTrim.IndexOf("\"");
                int j = jsonTrim.LastIndexOf("\"");
                var finalJson = jsonTrim.Substring(i + 1, j - 1);
                
                var configVerb = JsonUtil.FromJson<ConfigurationModel>(finalJson);

                // set 'rn_host' config verb
                if (configVerb.rnt_host == null || configVerb.rnt_host.Trim().Equals(""))
                    errorList = errorList + RightNowConfigKeyNames.RIGHTNOW_HOST;
                else
                    _registeredConfigVerbs[RightNowConfigKeyNames.RIGHTNOW_HOST] = configVerb.rnt_host;                

                // ICS CONFIGS
                // set ics 'username' config verb
                if (configVerb.integration.ics_service.username == null || configVerb.integration.ics_service.username.Trim().Equals(""))
                    errorList = errorList + " , " + RightNowConfigKeyNames.ICS_USERNAME;
                else
                    _registeredConfigVerbs[RightNowConfigKeyNames.ICS_USERNAME] = configVerb.integration.ics_service.username;

                // set ics 'password' config verb
                if (configVerb.integration.ics_service.password == null || configVerb.integration.ics_service.password.Trim().Equals(""))
                    errorList = errorList + "  " + RightNowConfigKeyNames.ICS_PASSWORD;
                else
                    _registeredConfigVerbs[RightNowConfigKeyNames.ICS_PASSWORD] = configVerb.integration.ics_service.password;

                // set ics 'base_url' config verb
                bool isICSBaseURLSet = false;
                if (configVerb.integration.ics_service.base_url == null || configVerb.integration.ics_service.base_url.Trim().Equals(""))
                {
                    errorList = errorList + "  " + RightNowConfigKeyNames.ICS_BASE_URL;
                }
                else
                {
                    _registeredConfigVerbs[RightNowConfigKeyNames.ICS_BASE_URL] = configVerb.integration.ics_service.base_url;
                    isICSBaseURLSet = true;
                }

                if (isICSBaseURLSet)
                {
                    // set ics 'get_messages_url' config verb
                    if (configVerb.integration.ics_service.get_messages_url == null || configVerb.integration.ics_service.get_messages_url.Trim().Equals(""))
                        errorList = errorList + "  " + RightNowConfigKeyNames.ICS_GET_MESSAGES_URL;
                    else
                        _registeredConfigVerbs[RightNowConfigKeyNames.ICS_GET_MESSAGES_URL] = configVerb.integration.ics_service.base_url + configVerb.integration.ics_service.get_messages_url;
                }

                if (isICSBaseURLSet)
                {
                    // set iot 'get_attributes_url' config verb
                    if (configVerb.integration.ics_service.get_attributes_url == null || configVerb.integration.ics_service.get_attributes_url.Trim().Equals(""))
                        errorList = errorList + "  " + RightNowConfigKeyNames.ICS_GET_ATTRIBUTES_URL;
                    else
                        _registeredConfigVerbs[RightNowConfigKeyNames.ICS_GET_ATTRIBUTES_URL] = configVerb.integration.ics_service.base_url + configVerb.integration.ics_service.get_attributes_url;
                }

                if (isICSBaseURLSet)
                {
                    // set iot 'set_attributes_url' config verb
                    if (configVerb.integration.ics_service.set_attributes_url == null || configVerb.integration.ics_service.set_attributes_url.Trim().Equals(""))
                        errorList = errorList + "  " + RightNowConfigKeyNames.ICS_SET_ATTRIBUTES_URL;
                    else
                        _registeredConfigVerbs[RightNowConfigKeyNames.ICS_SET_ATTRIBUTES_URL] = configVerb.integration.ics_service.base_url + configVerb.integration.ics_service.set_attributes_url;
                }

                if (errorList.Equals(""))
                {
                    isParsed = true;
                }

            }
            catch (Exception e)
            {
                isParsed = false;
                LogService.GetLog().Error(ExceptionMessages.CONFIG_VERB_IS_NOT_SET_OR_INCORRECT, e.StackTrace);
                MessageBox.Show(ExceptionMessages.CONFIG_VERB_IS_NOT_SET_OR_INCORRECT, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                _registeredConfigVerbs.Add(RightNowConfigKeyNames.CUSTOM_CFG_IOT_ACCEL_INTEGRATIONS, "");    
                _registeredConfigVerbs.Add(RightNowConfigKeyNames.ICS_BASE_URL, "");                            
                _registeredConfigVerbs.Add(RightNowConfigKeyNames.ICS_PASSWORD, "");
                _registeredConfigVerbs.Add(RightNowConfigKeyNames.RIGHTNOW_HOST, "");                
                _registeredConfigVerbs.Add(RightNowConfigKeyNames.ICS_USERNAME, "");                
                _registeredConfigVerbs.Add(RightNowConfigKeyNames.ICS_GET_MESSAGES_URL, "");
                _registeredConfigVerbs.Add(RightNowConfigKeyNames.ICS_GET_ATTRIBUTES_URL, "");
                _registeredConfigVerbs.Add(RightNowConfigKeyNames.ICS_SET_ATTRIBUTES_URL, "");
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
