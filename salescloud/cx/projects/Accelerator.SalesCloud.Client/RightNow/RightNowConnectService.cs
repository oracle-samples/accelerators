/* * *******************************************************************************************
*  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + OSC Lead Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.11 (November 2015)
 *  OSC release: Release 10
 *  reference: 150505-000122, 160620-000160
 *  date: Mon Sep 19 02:05:27 PDT 2016

 *  revision: rnw-15-11-fixes-release-3
*  SHA1: $Id: 0a964ac8c82a88f321e2ac06638863f7119295cf $
* *********************************************************************************************
*  File: RightNowConnectService.cs
* ****************************************************************************************** */


using System;
using System.Collections.Generic;
using System.Web.Services.Description;
using System.Windows.Forms;
using Accelerator.SalesCloud.Client.Common;
using System.Web.Script.Serialization;
using Accelerator.SalesCloud.Client;
using System.ServiceModel;
using Accelerator.SalesCloud.Client.RightNowProxyService;
using System.ServiceModel.Channels;
using System.Configuration;
using Accelerator.SalesCloud.Client.Logs;
using RightNow.AddIns.AddInViews;


namespace Accelerator.SalesCloud.Client.RightNow
{
    public class RightNowConnectService : IRightNowConnectService
    {

        private static RightNowConnectService _rightnowConnectService;
        private static object _sync = new object();
        private static RightNowSyncPortClient _rightNowClient;

        private RightNowConnectService()
        {

        }

        public static IRightNowConnectService GetService()
        {
            if (_rightnowConnectService != null)
            {
                return _rightnowConnectService;
            }

            try
            {
                lock (_sync)
                {
                    if (_rightnowConnectService == null)
                    {
                        // Initialize client with current interface soap url 
                        string url = SalesCloudAutoClientAddIn.GlobalContext.GetInterfaceServiceUrl(ConnectServiceType.Soap);
                        EndpointAddress endpoint = new EndpointAddress(url);
                        //EndpointAddress endpoint = new EndpointAddress("https://osc-svc-integration-qb1--dvp.qb.lan/cgi-bin/osc_svc_integration_qb1.cfg/services/soap");

                        BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportWithMessageCredential);
                        binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;

                        // Optional depending upon use cases
                        binding.MaxReceivedMessageSize = 1024 * 1024;
                        binding.MaxBufferSize = 1024 * 1024;
                        binding.MessageEncoding = WSMessageEncoding.Mtom;

                        _rightNowClient = new RightNowSyncPortClient(binding, endpoint);
                        

                        // Initialize credentials for rightnow client
                        /*var rightNowConnectUser = ConfigurationManager.AppSettings["rightnow_user"];
                        var rightNowConnectPassword = ConfigurationManager.AppSettings["rightnow_password"];

                        _rightNowClient.ClientCredentials.UserName.UserName = rightNowConnectUser;
                        _rightNowClient.ClientCredentials.UserName.Password = rightNowConnectPassword;*/

                        BindingElementCollection elements = _rightNowClient.Endpoint.Binding.CreateBindingElements();
                        elements.Find<SecurityBindingElement>().IncludeTimestamp = false;
                        _rightNowClient.Endpoint.Binding = new CustomBinding(elements);
                        _rightnowConnectService = new RightNowConnectService();                        
                    }

                }
            }
            catch (Exception e)
            {
                _rightnowConnectService = null;
                MessageBox.Show(OSCExceptionMessages.RightNowConnectServiceNotInitialized, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _rightnowConnectService;
        }

        public RightNowSyncPortClient GetRightNowClient()
        {
            return _rightNowClient;
        }

        public string GetRightNowEndPointURIHost()
        {
            string endpointurihost = null;
            if (_rightNowClient.Endpoint != null && _rightNowClient.Endpoint.Address != null && _rightNowClient.Endpoint.Address.Uri != null)
            {
                endpointurihost = _rightNowClient.Endpoint.Address.Uri.Host;
            }

            return endpointurihost;
        }

        /// <summary>
        /// Get config verb value
        /// </summary>
        /// <param name="configVerbName"></param>
        /// <returns></returns>
        public string GetRightNowConfigVerbValue(string configVerbName)
        {
            //Prepare session
            SalesCloudAutoClientAddIn.GlobalContext.PrepareConnectSession(_rightNowClient.ChannelFactory);

            Debug("RightNowConnectService - GetRightNowConfigVerbValue() - Enter");
            string configVerbValue = String.Empty;
            try
            {
                // Set up query and set request
                ClientInfoHeader cih = new ClientInfoHeader();
                cih.AppID = OracleRightNowOSCAddInNames.OracleRightNowOSCClient;

                byte[] outByte = new byte[1000];
                string query = RightNowQueries.GetConfigVerbQuery + "'" + configVerbName + "'";
                Notice("Sending query to fetch " + configVerbName + " Config verb value");
                CSVTableSet tableSet = _rightNowClient.QueryCSV(cih, query, 100, ",", false, false, out outByte);
                CSVTable[] csvTables = tableSet.CSVTables;
                CSVTable table = csvTables[0];
                string[] rowData = table.Rows;

                // Check whether configuration is set
                if (rowData.Length == 0)
                {
                    return String.Empty;
                }

                // Get configuration value
                Notice("Returning Config verb '" + configVerbName + "' value: " + rowData[0]);
                configVerbValue = rowData[0];
            }
            catch (Exception e)
            {
                Debug("RightNowConnectService - GetRightNowConfigVerbValue() - Error while fetching config verb" + e.Message);
                Error("RightNowConnectService - GetRightNowConfigVerbValue() - Error while fetching config verb", e.StackTrace);
                MessageBox.Show("RightNowConnectService - Error while fetching config verb", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Debug("RightNowConnectService - GetRightNowConfigVerbValue() - Exit");
            return configVerbValue;
        }
        
       


        /// <summary>
        /// Return individual fields as per query
        /// </summary>
        /// <param name="ApplicationID"></param>
        /// <param name="Query"></param>
        /// <returns> array of string delimited by '|'</returns>
        private string[] GetRNData(string ApplicationID, string Query)
        {
            string[] rn_data = null;

            SalesCloudAutoClientAddIn.GlobalContext.PrepareConnectSession(_rightNowClient.ChannelFactory);

            ClientInfoHeader hdr = new ClientInfoHeader() { AppID = ApplicationID };

            byte[] output = null;
            CSVTableSet data = null;

            try
            {
                data = _rightNowClient.QueryCSV(hdr, Query, 50, "|", false, false, out output);

                string data_row = String.Empty;

                if (data != null && data.CSVTables.Length > 0 && data.CSVTables[0].Rows.Length > 0)
                {
                    return data.CSVTables[0].Rows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return rn_data;
        }

        public string GetContactOrgExternalReference(int id, bool isContact, String applicationId)
        {     
            string query;
            if (isContact)
            {
                query = String.Format(RightNowQueries.GetContactExternalReferenceQuery, id);
            }
            else
            {
                query = String.Format(RightNowQueries.GetOrganizationExternalReferenceQuery, id);
            }

            string[] ext_ref = GetRNData(applicationId, query)[0].Split(new char[] { '|' });

            if (null != ext_ref && null != ext_ref[0])
            {
                string[] data = ext_ref[0].Split('|');
                return data[0];
            }
            return null;
        }

        private void Notice(string logMessage, string logNote = null)
        {
            if (RightNowConfigService.IsConfigured())
            {
                var log = OSCLogService.GetLog();
                log.Notice(logMessage, logNote);
            }
        }

        private void Error(string logMessage, string logNote = null)
        {
            if (RightNowConfigService.IsConfigured())
            {
                var log = OSCLogService.GetLog();
                log.Error(logMessage, logNote);
            }
        }

        private void Debug(string logMessage, string logNote = null)
        {
            if (RightNowConfigService.IsConfigured())
            {
                var log = OSCLogService.GetLog();
                log.Debug(logMessage, logNote);
            }
        }

    }
}
