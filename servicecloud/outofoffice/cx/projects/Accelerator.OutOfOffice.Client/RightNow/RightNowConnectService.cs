/* * *******************************************************************************************
*  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC Out of Office Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 16.5 (May 2016) 
 *  reference: 150916-000080
 *  date: Thu Mar 17 23:37:53 PDT 2016
 
 *  revision: rnw-16-5-fixes-release-1
*  SHA1: $Id: ac94155276fd7fb3e55ef659f790b44f87e362b8 $
* *********************************************************************************************
*  File: RightNowConnectService.cs
* ****************************************************************************************** */


using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Windows.Forms;
using System.Xml;
using Accelerator.OutOfOffice.Client.Common;
using Accelerator.OutOfOffice.Client.Logs;
using Accelerator.OutOfOffice.Client.Model;
using Accelerator.OutOfOffice.Client.RightNowProxyService;
using Accelerator.OutOfOffice.Client.Services;
using RightNow.AddIns.AddInViews;

namespace Accelerator.OutOfOffice.Client.RightNow
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
                        string url = OutOfOfficeClientAddIn.GlobalContext.GetInterfaceServiceUrl(ConnectServiceType.Soap);
                        EndpointAddress endpoint = new EndpointAddress(url);
                        BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportWithMessageCredential);
                        binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;

                        // Optional depending upon use cases
                        binding.MaxReceivedMessageSize = 1024 * 1024;
                        binding.MaxBufferSize = 1024 * 1024;
                        binding.MessageEncoding = WSMessageEncoding.Mtom;

                        _rightNowClient = new RightNowSyncPortClient(binding, endpoint);

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
                MessageBox.Show(OOOExceptionMessages.RightNowConnectServiceNotInitialized, Common.Common.ErrorLabel, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        /// Return individual fields as per query
        /// </summary>
        /// <param name="ApplicationID"></param>
        /// <param name="Query"></param>
        /// <returns> array of string delimited by '|'</returns>
        private string[] GetRNData(string ApplicationID, string Query)
        {
            Debug("RightNowConnectService - GetRNData() - Entry");

            string[] rn_data = null;

            OutOfOfficeClientAddIn.GlobalContext.PrepareConnectSession(_rightNowClient.ChannelFactory);

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
                Error(ex.Message, ex.StackTrace);
                throw ex;
            }

            Debug("RightNowConnectService - GetRNData() - Exit");
            return rn_data;
        }


        /// <summary>
        /// Return Account's custom fields
        /// </summary>
        /// <returns></returns>
        public StaffAccount GetAccountDetails()
        {
            try
            {

                Debug("RightNowConnectService - GetAccountDetails() - Entry");

                int accountId = OutOfOfficeClientAddIn.GlobalContext.AccountId;
                string query = String.Format(RightNowQueries.GetAccountDetailsQuery, accountId);

                string[] data = GetRNData(OracleRightNowOOOAddInNames.OutOfOfficeAddIn, query);

                string[] accountdetails = data[0].Split('|');

                var staffAccount = new StaffAccount();
                staffAccount.AccountId = accountId;

                //Fetching timezone
                string windowsTimezone = null;
                TimeZoneInfo timeszone = null;

                if (!string.IsNullOrEmpty(accountdetails[5]))
                {
                    staffAccount.OooTimezone = accountdetails[5];
                    windowsTimezone = TimezoneService.GetService().GetWindowsTimezone(accountdetails[5]);
                    timeszone = TimeZoneInfo.FindSystemTimeZoneById(windowsTimezone);
                }

                if (!string.IsNullOrEmpty(accountdetails[0]))
                {
                    //staffAccount.OooStart = TimeZoneInfo.ConvertTime(XmlConvert.ToDateTime(accountdetails[0]), timeszone);
                    DateTime dateTime = XmlConvert.ToDateTime(accountdetails[0], XmlDateTimeSerializationMode.Utc);
                    staffAccount.OooStart = OutOfOfficeClientAddIn.GlobalContext.GetTimeZoneDateTime(staffAccount.OooTimezone, dateTime) ?? DateTime.Now;
                }
                else
                {
                    staffAccount.OooStart = DateTime.Now;
                }

                if (!string.IsNullOrEmpty(accountdetails[1]))
                {
                    //staffAccount.OooEnd = TimeZoneInfo.ConvertTime(XmlConvert.ToDateTime(accountdetails[1]), timeszone);
                    DateTime dateTime = XmlConvert.ToDateTime(accountdetails[1], XmlDateTimeSerializationMode.Utc);
                    staffAccount.OooEnd = OutOfOfficeClientAddIn.GlobalContext.GetTimeZoneDateTime(staffAccount.OooTimezone, dateTime) ?? DateTime.Now;
                }
                else
                {
                    staffAccount.OooEnd = DateTime.Now;
                }

                if (!string.IsNullOrEmpty(accountdetails[2]))
                    staffAccount.OooFlag = (accountdetails[2].Equals("1")) ? true : false;
                if (!string.IsNullOrEmpty(accountdetails[3]))
                    staffAccount.OooMsg = accountdetails[3];
                if (!string.IsNullOrEmpty(accountdetails[4]))
                    staffAccount.OooMsgOption = accountdetails[4];

                
                return staffAccount;
            }
            catch (Exception e)
            {
                MessageBox.Show(OOOExceptionMessages.UnexpectedError, Common.Common.ErrorLabel, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Error(e.Message, e.StackTrace);
            }

            Debug("RightNowConnectService - GetAccountDetails() - Exit");
            return null;
        }


        /// <summary>
        /// Updates the latest values of the custome fields into database.
        /// </summary>
        /// <param name="staffAccount"></param>
        /// <returns></returns>
        public bool updateCustomFields(StaffAccount staffAccount)
        {
            try
            {
                Debug("RightNowConnectService - updateCustomFields() - Entry");

                GenericField csFldOOOEnd = null;
                GenericField csFldOOOStart = null;

                Account account = new Account();
                ID accountID = new ID();
                accountID.id = OutOfOfficeClientAddIn.GlobalContext.AccountId;
                accountID.idSpecified = true;
                account.ID = accountID;


                // Out of Office Flag
                DataValue dv = new DataValue();
                dv.Items = new Object[] { staffAccount.OooFlag };
                dv.ItemsElementName = new ItemsChoiceType[] { ItemsChoiceType.BooleanValue };

                GenericField csFldOOOFlag = new GenericField();
                csFldOOOFlag.name = CustomField.OooFlag;
                csFldOOOFlag.dataType = DataTypeEnum.BOOLEAN;
                csFldOOOFlag.dataTypeSpecified = true;
                csFldOOOFlag.DataValue = dv;

                // Out of Office Start
                csFldOOOStart = new GenericField();
                csFldOOOStart.name = CustomField.OooStart;
                csFldOOOStart.dataType = DataTypeEnum.DATETIME;
                csFldOOOStart.dataTypeSpecified = true;
                DataValue dvOooStart = null;
                if (staffAccount.OooStart != null && staffAccount.OooStart != default(DateTime))
                {
                    dvOooStart = new DataValue();
                    dvOooStart.Items = new Object[] { staffAccount.OooStart };
                    dvOooStart.ItemsElementName = new ItemsChoiceType[] { ItemsChoiceType.DateTimeValue };
                }
                csFldOOOStart.DataValue = dvOooStart;

                // Out of Office End
                csFldOOOEnd = new GenericField();
                csFldOOOEnd.name = CustomField.OooEnd;
                csFldOOOEnd.dataType = DataTypeEnum.DATETIME;
                csFldOOOEnd.dataTypeSpecified = true;
                DataValue dvOooEnd = null;
                if (staffAccount.OooEnd != null)
                {
                    dvOooEnd = new DataValue();
                    dvOooEnd.Items = new Object[] { staffAccount.OooEnd };
                    dvOooEnd.ItemsElementName = new ItemsChoiceType[] { ItemsChoiceType.DateTimeValue };
                }
                csFldOOOEnd.DataValue = dvOooEnd;

                // Out of Office Message Option
                DataValue dvOooMsgOption = new DataValue();
                dvOooMsgOption.Items = new Object[] { staffAccount.OooMsgOption };
                dvOooMsgOption.ItemsElementName = new ItemsChoiceType[] { ItemsChoiceType.StringValue };

                GenericField csFldOOOMsgOption = new GenericField();
                csFldOOOMsgOption.name = CustomField.OooMsgOption;
                csFldOOOMsgOption.dataType = DataTypeEnum.STRING;
                csFldOOOMsgOption.dataTypeSpecified = true;
                csFldOOOMsgOption.DataValue = dvOooMsgOption;

                // Out of Office Message
                DataValue dvOooMsg = new DataValue();
                dvOooMsg.Items = new Object[] { staffAccount.OooMsg };
                dvOooMsg.ItemsElementName = new ItemsChoiceType[] { ItemsChoiceType.StringValue };

                GenericField csFldOOOMsg = new GenericField();
                csFldOOOMsg.name = CustomField.OooMsg;
                csFldOOOMsg.dataType = DataTypeEnum.STRING;
                csFldOOOMsg.dataTypeSpecified = true;
                csFldOOOMsg.DataValue = dvOooMsg;

                // Out of Office Timezone
                GenericField csFldOOOTimezone = new GenericField();
                csFldOOOTimezone.name = CustomField.OooTimezone;
                csFldOOOTimezone.dataType = DataTypeEnum.STRING;
                csFldOOOTimezone.dataTypeSpecified = true;
                DataValue dvOooTimezone = null;
                if (null != staffAccount.OooTimezone)
                {
                    dvOooTimezone = new DataValue();
                    dvOooTimezone.Items = new Object[] { staffAccount.OooTimezone };
                    dvOooTimezone.ItemsElementName = new ItemsChoiceType[] { ItemsChoiceType.StringValue };
                }
                csFldOOOTimezone.DataValue = dvOooTimezone;


                GenericObject customFieldsc = new GenericObject();

                customFieldsc.GenericFields = new GenericField[] { csFldOOOFlag, csFldOOOStart, csFldOOOEnd, csFldOOOMsgOption, csFldOOOMsg, csFldOOOTimezone };

                customFieldsc.ObjectType = new RNObjectType() { TypeName = CustomField.AccountCustomFieldCollectionTypeName };

                GenericField customFieldsPackage = new GenericField();
                customFieldsPackage.name = "c";
                customFieldsPackage.dataType = DataTypeEnum.OBJECT;
                customFieldsPackage.dataTypeSpecified = true;
                customFieldsPackage.DataValue = new DataValue();
                customFieldsPackage.DataValue.Items = new[] { customFieldsc };
                customFieldsPackage.DataValue.ItemsElementName = new[] { ItemsChoiceType.ObjectValue };

                account.CustomFields = new GenericObject
                {
                    GenericFields = new[] { customFieldsPackage },
                    ObjectType = new RNObjectType { TypeName = CustomField.AccountCustomFieldsTypeName }
                };

                ClientInfoHeader clientInfoHeader = new ClientInfoHeader();
                clientInfoHeader.AppID = OracleRightNowOOOAddInNames.OutOfOfficeAddIn;

                RNObject[] contactObjects = new RNObject[] { account };
                UpdateProcessingOptions updateProcessingOptions = new UpdateProcessingOptions();
                _rightNowClient.Update(clientInfoHeader, contactObjects, updateProcessingOptions);

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(OOOExceptionMessages.UnexpectedError, Common.Common.ErrorLabel, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Error(e.Message, e.StackTrace);
            }

            Debug("RightNowConnectService - updateCustomFields() - Exit");
            return false;
        }

        private void Error(string logMessage, string logNote = null)
        {
            var log = OSvCLogService.GetLog();
            log.Error(logMessage, logNote);
        }

        private void Debug(string logMessage, string logNote = null)
        {
            var log = OSvCLogService.GetLog();
            log.Debug(logMessage, logNote);
        }

    }
}
