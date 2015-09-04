/* * *******************************************************************************************
*  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + OFSC Reference Integration
 *  link: http://www-content.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.2 (Feb 2015)
 *  OFSC release: 15.2 (Feb 2015)
 *  reference: 150622-000130
 *  date: Thu Sep  3 23:14:02 PDT 2015

 *  revision: rnw-15-11-fixes-release-03
*  SHA1: $Id: 4397aa47f140b7444f3dd0d22e5e32a2db193939 $
* *********************************************************************************************
*  File: RightNowConnectService.cs
* ****************************************************************************************** */

using System;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Windows.Forms;
using Oracle.RightNow.Toa.Client.Common;
using Oracle.RightNow.Toa.Client.RightNowProxyService;
using Oracle.RightNow.Toa.Client.Services;
using RightNow.AddIns.AddInViews;
using System.Collections.Generic;
using Oracle.RightNow.Toa.Client.Model;
using Oracle.RightNow.Toa.Client.Logs;
using System.Text;

namespace Oracle.RightNow.Toa.Client.Rightnow
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
                        string url = ToaAutoClientAddIn.GlobalContext.GetInterfaceServiceUrl(ConnectServiceType.Soap);
                        EndpointAddress endpoint = new EndpointAddress(url);                            

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
                MessageBox.Show(ToaExceptionMessages.RightNowConnectServiceNotInitialized,"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _rightnowConnectService;
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
            Debug("RightNowConnectService - GetRightNowConfigVerbValue() - Enter");
            string configVerbValue = String.Empty;
            try
            {
                //Prepare session
                ToaAutoClientAddIn.GlobalContext.PrepareConnectSession(_rightNowClient.ChannelFactory);

                // Set up query and set request
                ClientInfoHeader cih = new ClientInfoHeader();
                cih.AppID = OracleRightNowToaAddInNames.OracleRightNowToaClient;

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
        /// Returns the object as per query
        /// Ex Query : SELECT Contact FROM Contact WHERE Contact.ID = 21 LIMIT 1
        /// </summary>
        /// <param name="ApplicationID"></param>
        /// <param name="Query"></param>
        /// <param name="RNObjects"></param>
        //public object GetRNObject(string ApplicationID, string Query, RNObject[] RNObjects)
        //{
        //    ToaAutoClientAddIn.GlobalContext.PrepareConnectSession(_rightNowClient.ChannelFactory);
        //
        //    ClientInfoHeader hdr = new ClientInfoHeader() { AppID = ApplicationID };
        //
        //    if (_rightNowClient != null)
        //    {
        //        QueryResultData[] data = _rightNowClient.QueryObjects(hdr, Query, RNObjects, 10);
        //        RNObject[] rn_objects = data[0].RNObjectsResult;
        //        return rn_objects[0];
        //    }

        //    return null;
        //}

        /// <summary>
        /// Return individual fields as per query
        /// </summary>
        /// <param name="ApplicationID"></param>
        /// <param name="Query"></param>
        /// <returns> array of string delimited by '|'</returns>
        private string[] GetRNData(string ApplicationID, string Query)
        {
            string[] rn_data = null;

            ToaAutoClientAddIn.GlobalContext.PrepareConnectSession(_rightNowClient.ChannelFactory);

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

        private void Notice(string logMessage, string logNote = null)
        {
             if(RightNowConfigService.IsConfigured())
             {
                 var log = ToaLogService.GetLog();
                 log.Notice(logMessage, logNote);
             }             
        }

        private void Error(string logMessage, string logNote = null)
        {
            if (RightNowConfigService.IsConfigured())
            {
                var log = ToaLogService.GetLog();
                log.Error(logMessage, logNote);
            }             
        }

        private void Debug(string logMessage, string logNote = null)
        {
            if (RightNowConfigService.IsConfigured())
            {
                var log = ToaLogService.GetLog();
                log.Debug(logMessage, logNote);
            }             
        }

        /**
         * Commeting out the code to be reused if customer decides to have dynamic field mapping.
         * This part of code fetches the data fro the mapping table.
         * 
        private void InitializeMappingFields(string ApplicationID)
        {
            string query = "SELECT TOA.WO_Data_Mapping.WO_Field, TOA.WO_Data_Mapping.WS_Field, TOA.WO_Data_Mapping.Related_Object_Field_Lvl_1, TOA.WO_Data_Mapping.Related_Object_Lvl_1, TOA.WO_Data_Mapping.Data_Sync FROM TOA.WO_Data_Mapping where TOA.WO_Data_Mapping.Data_Sync = 1";
            try
            {
                string[] datas = _rightnowConnectService.GetRNData(ApplicationID, query);

                _workOrderFieldMappings = new Dictionary<string, WorkOrderFieldMapping>();

                foreach (string data in datas)
                {
                    string[] row = data.Split(new char[] { '|' });
                    WorkOrderFieldMapping woFieldMapping = new WorkOrderFieldMapping();
                    woFieldMapping.Wo_Field = row[0];
                    woFieldMapping.WS_Field = row[1];
                    woFieldMapping.Related_Object_Field_Lvl_1 = row[2];
                    woFieldMapping.Related_Object_Lvl_1 = row[3];
                    woFieldMapping.Data_Sync = Convert.ToInt32(row[4]);

                    //TODO: Currently zeroth level field are being fetched so skipping duplicate work order field, but this would
                    // need handling while implementing first level drill down.
                    if (!_workOrderFieldMappings.ContainsKey(woFieldMapping.Wo_Field))
                    {
                        _workOrderFieldMappings.Add(woFieldMapping.Wo_Field, woFieldMapping);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }*/

        /*
         * This is a way to fetch an menu item through the connect api.
        public NamedID[] getNamedID(string fieldName)
        {
            ToaAutoClientAddIn.GlobalContext.PrepareConnectSession(_rightNowClient.ChannelFactory);
            ClientInfoHeader hdr = new ClientInfoHeader() { AppID = OracleRightNowToaAddInNames.WorkOrderAddIn };

            NamedID[] namedId = null;
            if (_rightNowClient != null)
            {
                namedId = _rightNowClient.GetValuesForNamedID(hdr, null, fieldName);
                
            }
            return namedId;
        }*/

        public string GetProvinceName(int provinceId)
        {
            string query = String.Format("SELECT Provinces.Name from Country WHERE Provinces.ID = {0} limit 1", provinceId);
            string[] resultset = GetRNData(OracleRightNowToaAddInNames.WorkOrderAddIn, query);
            if (resultset.Length > 0)
            {
                return resultset[0];
            }
            return null;
        }

        public string[] GetWorkOrderTypeFromID(int workorderTypeId)
        {
            string query = String.Format("select WO_Type_Code, Manual_Duration, Manual_Duration_Default from TOA.Work_Order_Type where ID = {0}", workorderTypeId);
            string[] workordertype = GetRNData(OracleRightNowToaAddInNames.WorkOrderAddIn, query)[0].Split(new char[] { '|' });
            return workordertype;
        }

        public string[] GetReminder_TimeFromID(int reminderTimeId)
        {
            string query = String.Format("select Name from TOA.Reminder_Time where ID = {0}", reminderTimeId);
            string[] remindertime = GetRNData(OracleRightNowToaAddInNames.WorkOrderAddIn, query);
            return remindertime;
        }

        public string[] GetResolutionDueFromID(int incidentId)
        {
            string query = String.Format(RightNowQueries.GetMilestonesQuery,incidentId);
            string[] milestones = GetRNData(OracleRightNowToaAddInNames.WorkOrderAddIn, query);
            return milestones;
        }

        public string[] GetIncidentPrimaryAssetFromID(int incidentId)
        {
            string asset_query = String.Format("select Asset from Incident where ID = {0}", incidentId);
            string[] asset = GetRNData(OracleRightNowToaAddInNames.WorkOrderAddIn, asset_query);
            return asset;
        }        

        public string[] GetAssetDetailsFromAssetID(string assetId)
        {
            string asset_details_query = String.Format("select Product.ID, SerialNumber from Asset where ID = {0}", assetId);
            string[] assetdetails = GetRNData(OracleRightNowToaAddInNames.WorkOrderAddIn, asset_details_query);
            if (null != assetdetails && null != assetdetails[0])
            {
                return assetdetails[0].Split('|');
            }
            return assetdetails;
        }

        public string[] GetRequiredInventoryDetailsFromWorkOrderType(int workOrderTypeId)
        {
            string workOrder_type_inventory_query = String.Format("select Product.ID, Quantity, Model from TOA.WO_Type_Inventory where Work_Order_Type = {0}", workOrderTypeId);
            string[] workOrder_type_inventories = GetRNData(OracleRightNowToaAddInNames.WorkOrderAddIn, workOrder_type_inventory_query);
            return workOrder_type_inventories;
        }

        public string[] GetProductDetailsFromProductID(string productID)
        {
            string salesProductQuery = String.Format("select PartNumber from SalesProduct where ID = {0}", productID);
            string[] salesProductDetails = GetRNData(OracleRightNowToaAddInNames.WorkOrderAddIn, salesProductQuery);            
            return salesProductDetails;
        }

        //public string[] GetRequiredInventoryTypeCodeFromRequiredInventoryID(string requiredInventoryId)
        //{
        //    string workOrder_inventory_query = String.Format("select Inventory_Code, Model_Property from TOA.Work_Order_Inventory where ID = {0}", requiredInventoryId);
        //    string[] workOrderInventoryCode = GetRNData(OracleRightNowToaAddInNames.WorkOrderAddIn, workOrder_inventory_query);
        //    if (null != workOrderInventoryCode && null != workOrderInventoryCode[0])
        //    {
        //        return workOrderInventoryCode[0].Split('|');
        //    }
        //    return workOrderInventoryCode;
        //}

        //public QueryResultData[] GetProductCatalogDetailsFromId(int productId)
        //{
        //    string workOrder_inventory_query = String.Format("select SalesProduct from SalesProduct where ID = {0}", productId);
        //    ClientInfoHeader hdr = new ClientInfoHeader() { AppID = "TOAClient" };
        //    SalesProduct salesProduct = new SalesProduct();
        //    RNObject[] salesProductTemplates = new RNObject[] { salesProduct };
        //    QueryResultData[] querySalesProductsObj = null;
        //    try
        //    {
        //        querySalesProductsObj = _rightNowClient.QueryObjects(hdr, workOrder_inventory_query, salesProductTemplates, 1);
        //        return querySalesProductsObj;
        //    }catch(Exception ex)
        //    {
        //        throw ex;
        //    }
        //    //string[] workOrderInventoryCode = GetRNData(OracleRightNowToaAddInNames.WorkOrderAddIn, workOrder_inventory_query);
            
        //}


        public RightNowSyncPortClient GetRightNowClient()
        {
            return _rightNowClient;
        }
    }

}
