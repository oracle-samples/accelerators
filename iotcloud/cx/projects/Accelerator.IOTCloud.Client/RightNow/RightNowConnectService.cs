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
*  SHA1: $Id: dc3a31b29da8a3d6fbb4e8082793e0559c96cb09 $
* *********************************************************************************************
*  File: RightNowConnectService.cs
* ****************************************************************************************** */


using System;
using System.Windows.Forms;
using Accelerator.IOTCloud.Client.Model;
using System.ServiceModel;
using Accelerator.IOTCloud.Client.RightNowProxyService;
using System.ServiceModel.Channels;
using Accelerator.IOTCloud.Client.Logs;
using RightNow.AddIns.AddInViews;


namespace Accelerator.IOTCloud.Client.RightNow
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
                        string url = IOTClouddAutoClientAddIn.GlobalContext.GetInterfaceServiceUrl(ConnectServiceType.Soap);
                        EndpointAddress endpoint = new EndpointAddress(url);
                        //EndpointAddress endpoint = new EndpointAddress("https://day04-16500-sql-80h.qb.lan/cgi-bin/day04_16500_sql_80h.cfg/services/soap");

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
                        
                        _rightNowClient.ClientCredentials.UserName.UserName = "oracle";
                        _rightNowClient.ClientCredentials.UserName.Password = "oracle";*/

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
                MessageBox.Show(ExceptionMessages.RIGHTNOW_CONNECT_SERVICE_NOT_INITIALIZED, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            IOTClouddAutoClientAddIn.GlobalContext.PrepareConnectSession(_rightNowClient.ChannelFactory);

            Debug("RightNowConnectService - GetRightNowConfigVerbValue() - Enter");
            string configVerbValue = String.Empty;
            try
            {
                // Set up query and set request
                ClientInfoHeader cih = new ClientInfoHeader();
                cih.AppID = OracleRightNowIOTAddInNames.ORACLE_RIGHTNOW_IOT_CLIENT;

                byte[] outByte = new byte[1000];
                string query = RightNowQueries.GET_CONFIG_VERB_QUERY + "'" + configVerbName + "'";
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

            IOTClouddAutoClientAddIn.GlobalContext.PrepareConnectSession(_rightNowClient.ChannelFactory);

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

        public string GetDeviceId(int incident_id)
        {
            //Fetching asset id for the incident.
            var asset_id = GetIncidentAssetId(incident_id);

            if (asset_id != null)
            {
                string query = String.Format(RightNowQueries.GET_DEVICE_ID_QUERY, asset_id);

                string[] result = GetRNData(OracleRightNowIOTAddInNames.ORACLE_RIGHTNOW_IOT_CLIENT, query)[0].Split(new char[] { '|' });

                if (null != result && null != result[0])
                {
                    return result[0];
                }
            }

            return null;
        }

        private string GetIncidentAssetId(int incident_id)
        {
            string query = String.Format(RightNowQueries.GET_INCIDENT_ASSET_QUERY, incident_id);

            string[] result = GetRNData(OracleRightNowIOTAddInNames.ORACLE_RIGHTNOW_IOT_CLIENT, query)[0].Split(new char[] { '|' });

            if (null != result && null != result[0])
            {
                return result[0];
            }
            return null;
        }

        private void Notice(string logMessage, string logNote = null)
        {
            if (RightNowConfigService.IsConfigured())
            {
                var log = LogService.GetLog();
                log.Notice(logMessage, logNote);
            }
        }

        private void Error(string logMessage, string logNote = null)
        {
            if (RightNowConfigService.IsConfigured())
            {
                var log = LogService.GetLog();
                log.Error(logMessage, logNote);
            }
        }

        private void Debug(string logMessage, string logNote = null)
        {
            if (RightNowConfigService.IsConfigured())
            {
                var log = LogService.GetLog();
                log.Debug(logMessage, logNote);
            }
        }

    }
}
