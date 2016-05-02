/* * *******************************************************************************************
*  $ACCELERATOR_HEADER_PLACE_HOLDER$
*  SHA1: $Id: e299fa29017e90e29824f08fcb8f6b4c5bf7ecf4 $
* *********************************************************************************************
*  File: $ACCELERATOR_HEADER_FILE_NAME_PLACE_HOLDER$
* ****************************************************************************************** */


using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Windows.Forms;
using System.Xml;
using Accelerator.OutOfOffice.Client.Common;
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
        /// Return individual fields as per query
        /// </summary>
        /// <param name="ApplicationID"></param>
        /// <param name="Query"></param>
        /// <returns> array of string delimited by '|'</returns>
        private string[] GetRNData(string ApplicationID, string Query)
        {
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
                throw ex;
            }

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
                int accountId = OutOfOfficeClientAddIn.GlobalContext.AccountId;
                string query = String.Format(RightNowQueries.GetAccountDetailsQuery, accountId);

                string[] data = GetRNData(OracleRightNowOSCAddInNames.OutOfOfficeAddIn, query);

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
                    staffAccount.OooStart = TimeZoneInfo.ConvertTime(XmlConvert.ToDateTime(accountdetails[0]), timeszone);
                }
                else
                {
                    staffAccount.OooStart = DateTime.Now;
                }

                if (!string.IsNullOrEmpty(accountdetails[1]))
                {
                    staffAccount.OooEnd = TimeZoneInfo.ConvertTime(XmlConvert.ToDateTime(accountdetails[1]), timeszone);
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
                MessageBox.Show(OSCExceptionMessages.UnexpectedError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                if (staffAccount.OooStart != default(DateTime))
                {
                    DataValue dvOooStart = new DataValue();
                    dvOooStart.Items = new Object[] { staffAccount.OooStart };
                    dvOooStart.ItemsElementName = new ItemsChoiceType[] { ItemsChoiceType.DateTimeValue };

                    csFldOOOStart = new GenericField();
                    csFldOOOStart.name = CustomField.OooStart;
                    csFldOOOStart.dataType = DataTypeEnum.DATETIME;
                    csFldOOOStart.dataTypeSpecified = true;
                    csFldOOOStart.DataValue = dvOooStart;
                }

                // Out of Office End
                if (staffAccount.OooEnd != default(DateTime))
                {
                    DataValue dvOooEnd = new DataValue();
                    dvOooEnd.Items = new Object[] { staffAccount.OooEnd };
                    dvOooEnd.ItemsElementName = new ItemsChoiceType[] { ItemsChoiceType.DateTimeValue };

                    csFldOOOEnd = new GenericField();
                    csFldOOOEnd.name = CustomField.OooEnd;
                    csFldOOOEnd.dataType = DataTypeEnum.DATETIME;
                    csFldOOOEnd.dataTypeSpecified = true;
                    csFldOOOEnd.DataValue = dvOooEnd;
                }

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
                DataValue dvOooTimezone = new DataValue();
                dvOooTimezone.Items = new Object[] { staffAccount.OooTimezone };
                dvOooTimezone.ItemsElementName = new ItemsChoiceType[] { ItemsChoiceType.StringValue };

                GenericField csFldOOOTimezone = new GenericField();
                csFldOOOTimezone.name = CustomField.OooTimezone;
                csFldOOOTimezone.dataType = DataTypeEnum.STRING;
                csFldOOOTimezone.dataTypeSpecified = true;
                csFldOOOTimezone.DataValue = dvOooTimezone;


                GenericObject customFieldsc = new GenericObject();

                if (staffAccount.OooStart != default(DateTime) && staffAccount.OooEnd != default(DateTime))
                {
                    customFieldsc.GenericFields = new GenericField[] { csFldOOOFlag, csFldOOOStart, csFldOOOEnd, csFldOOOMsgOption, csFldOOOMsg, csFldOOOTimezone };
                }
                else
                {
                    customFieldsc.GenericFields = new GenericField[] { csFldOOOFlag, csFldOOOMsgOption, csFldOOOMsg };
                }

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
                clientInfoHeader.AppID = OracleRightNowOSCAddInNames.OutOfOfficeAddIn;

                RNObject[] contactObjects = new RNObject[] { account };
                UpdateProcessingOptions updateProcessingOptions = new UpdateProcessingOptions();
                _rightNowClient.Update(clientInfoHeader, contactObjects, updateProcessingOptions);

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(OSCExceptionMessages.UnexpectedError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

    }
}
