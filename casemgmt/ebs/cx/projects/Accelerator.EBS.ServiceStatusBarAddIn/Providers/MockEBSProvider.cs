/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + EBS Enhancement
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.5 (May 2015)
 *  EBS release: 12.1.3
 *  reference: 150202-000157
 *  date: Wed Sep  2 23:11:41 PDT 2015

 *  revision: rnw-15-8-fixes-release-01
 *  SHA1: $Id: 9e48bf43ac8426fd2a9af44e02f65879a739d51a $
 * *********************************************************************************************
 *  File: MockEBSProvider.cs
 * *********************************************************************************************/

using Accelerator.EBS.SharedServices.MockEBSServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Reflection;
using RightNow.AddIns.AddInViews;

namespace Accelerator.EBS.SharedServices.Providers
{
    internal class MockEBSProvider : IEBSProvider
    {
        #region Properties
        public string SRServiceURL
        {
            get;
            set;
        }

        public string SRLookupURL
        {
            get;
            set;
        }

        public string SRServiceUsername
        {
            get;
            set;
        }

        public string SRServicePassword
        {
            get;
            set;
        }

        public string ROCreateURL
        {
            get;
            set;
        }
        public string ROUpdateURL
        {
            get;
            set;
        }

        public string ROLookupURL
        {
            get;
            set;
        }
        public string ROListLookupURL
        {
            get;
            set;
        }

        public string ROServiceUsername
        {
            get;
            set;
        }

        public string ROServicePassword
        {
            get;
            set;
        }


        public string InteractionURL
        {
            get;
            set;
        }

        public string InteractionUsername
        {
            get;
            set;
        }

        public string InteractionPassword
        {
            get;
            set;
        }

        public string ContactListLookupURL 
        { 
            get; 
            set; 
        }
        public string ContactServiceUsername 
        { 
            get; 
            set; 
        }
        public string ContactServicePassword 
        { 
            get; 
            set; 
        }
        public int ContactServiceTimeout
        {
            get;
            set;
        }

        public string ItemListURL { get; set; }
        public string ItemServiceUsername { get; set; }
        public string ItemServicePassword { get; set; }
        public int ItemServiceTimeout { get; set; }

        public string EntitlementListURL { get; set; }
        public string EntitlementServiceUsername { get; set; }
        public string EntitlementServicePassword { get; set; }
        public int EntitlementServiceTimeout { get; set; }

        public string RepairOrderListURL { get; set; }
        public string RepairOrderListServiceUsername { get; set; }
        public string RepairOrderListServicePassword { get; set; }
        public int RepairOrderListServiceTimeout { get; set; }

        public string RepairLogisticsListURL { get; set; }
        public string RepairLogisticsListServiceUsername { get; set; }
        public string RepairLogisticsListServicePassword { get; set; }
        public int RepairLogisticsListServiceTimeout { get; set; }

        public Logs.LogWrapper log { get; set; }
        public int _logIncidentId { get; set; }
        public int _logContactId { get; set; }
        #endregion

        private BasicHttpBinding binding;
        private EndpointAddress addr;

        #region Init Methods
        public void InitForSR(string service_url, string lookup_url, string user_name, string password, int timeout = -1)
        {
            SRServiceURL = service_url;
            SRServiceUsername = user_name;
            SRServicePassword = password;
            // mock proxy client doesn't have .Timeout attribute, so not used for now
            InitBindings(service_url);
        }

        public void InitForRO(string report_url,  string create_url, string update_url, string user_name, string password, int timeout = -1)
        {
            ROCreateURL = create_url;
            ROServiceUsername = user_name;
            ROServicePassword = password;
            InitBindings(create_url);
        }

        public void InitForInteraction(string interaction_url, string user_name, string password, int timeout = -1)
        {
            InteractionURL = interaction_url;
            InteractionUsername = user_name;
            InteractionPassword = password;
            InitBindings(interaction_url);
        }

        public void InitForContact(string contact_url, string user_name, string password, int timeout)
        {
            ContactListLookupURL = contact_url;
            ContactServiceUsername = user_name;
            ContactServicePassword = password;
            ContactServiceTimeout = timeout;
        }

        public void InitForItem(string list_url, string user_name, string password, int timeout)
        {
            ItemListURL = list_url;
            ItemServiceUsername = user_name;
            ItemServicePassword = password;
            ItemServiceTimeout = timeout;
        }

        public void InitForEntitlement(string list_url, string user_name, string password, int timeout)
        {
            EntitlementListURL = list_url;
            EntitlementServiceUsername = user_name;
            EntitlementServicePassword = password;
            EntitlementServiceTimeout = timeout;
        }

        public void InitForRepairLogisticsList(string list_url, string user_name, string password, int timeout)
        {
            RepairLogisticsListURL = list_url;
            RepairLogisticsListServiceUsername = user_name;
            RepairLogisticsListServicePassword = password;
            RepairLogisticsListServiceTimeout = timeout;
        }

        private void InitBindings(string url)
        {
            addr = new EndpointAddress(url);
            binding = new BasicHttpBinding();
        }
        #endregion

        #region Save/Lookup Methods
        public ServiceRequest CreateSR(ServiceRequest sr, int _logIncidentId = 0, int _logContactId = 0)
        {
            return null;
            //throw new NotImplementedException();
            /*ServiceRequest retval = new ServiceRequest();

            EBSMockSvcClient _client = new EBSMockSvcClient(binding, addr);
            
                //IEBSMockSvc _client = factory.CreateChannel();

                
            try
            {
                MockEBSServiceRequest mock_sr = new MockEBSServiceRequest()
                {                                        
                    Notes = sr.Notes,
                    Owner = sr.Owner,
                    RequestDate = sr.RequestDate,
                    RequestType = sr.RequestType,                    
                    SerialNumber = sr.SerialNumber,
                    Severity = sr.Severity,                    
                    Status = sr.Status,                    
                    Summary = sr.Summary,
                    IncidentID = Convert.ToInt64(sr.IncidentID),
                    IncidentRef = sr.IncidentRef
                };

                if (!String.IsNullOrEmpty(sr.EbsContactID))
                {
                    mock_sr.ContactID = Convert.ToInt64(sr.EbsContactID);
                }

                if (sr.IncidentOccurredDate.HasValue)
                {
                    mock_sr.IncidentOccurredDate = sr.IncidentOccurredDate.Value;
                }                
                else
                {
                    mock_sr.IncidentOccurredDate = DateTime.Today;
                }

                if (sr.RequestTypeID.HasValue)
                {
                    mock_sr.RequestTypeID = Convert.ToInt64(sr.RequestTypeID.Value);
                }
                else
                {
                    mock_sr.RequestTypeID = 0;
                }
                if (sr.SeverityID.HasValue)
                {
                    mock_sr.SeverityID = Convert.ToInt64(sr.SeverityID.Value);
                }
                else
                {
                    mock_sr.SeverityID = 0;
                }
                if (sr.StatusID.HasValue)
                {
                    mock_sr.StatusID = Convert.ToInt64(sr.StatusID.Value);
                }
                else
                {
                    mock_sr.StatusID = 0;
                }
                if (sr.ContractID.HasValue)
                {
                    mock_sr.ContractID = Convert.ToInt64(sr.ContractID.Value);
                }
                else
                {
                    mock_sr.ContractID = 0;
                }

                SROutputParameters output;
                using (new OperationContextScope(_client.InnerChannel))
                {
                    Credential userInfo = new Credential();
                    userInfo.username = ROServiceUsername;
                    userInfo.password = ROServicePassword;

                    MessageHeader aMessageHeader = MessageHeader.CreateHeader("Credential", "", userInfo);
                    OperationContext.Current.OutgoingMessageHeaders.Add(aMessageHeader);

                    output = _client.wssCreateSR(mock_sr);
                }
                
                if (!String.IsNullOrWhiteSpace(output.Error_Message))
                {
                    retval.ErrorMessage = output.Error_Message;
                }
                else
                {
                    retval.RequestID = (decimal?)Convert.ToDecimal(output.RequestID);
                    retval.RequestNumber =  Convert.ToString(output.RequestNumber);
                }
            }
            catch (CommunicationException e)
            {
                retval.ErrorMessage = e.ToString();
                _client.Abort();
            }
            catch (TimeoutException e)
            {
                retval.ErrorMessage = e.ToString();
                _client.Abort();
            }            
            catch (Exception ex)
            {
                retval.ErrorMessage = String.Format("The following error occurred: {0}", ex); 
                _client.Abort();
            }

            return retval;*/
        }

        public RepairOrder CreateRO(RepairOrder ro, int _logIncidentId = 0, int _logContactId = 0)
        {return null;
            /*RepairOrder retval = new RepairOrder();
            EBSMockSvcClient _client = new EBSMockSvcClient(binding, addr);
            
                //IEBSMockSvc _client = factory.CreateChannel();

            try
            {
                MockEBSRepairOrder mock_ro = new MockEBSRepairOrder()
                {
                    ApprovalRequired = ro.ApprovalRequired,
                    ApprovalStatus = ro.ApprovalStatus,
                    Currency = ro.Currency,
                    CustomerProductID = 110001,
                    ServiceRequestID = Convert.ToInt64(ro.ServiceRequestID.Value),
                    InventoryItemID = Convert.ToInt64(ro.InventoryItemID.Value),
                    ProblemDescription = ro.ProblemDescription,
                    Quantity = Convert.ToInt64(ro.Quantity.Value),
                    ResourceID = Convert.ToInt64(ro.ResourceID.Value),
                    UnitOfMeasure = ro.UnitOfMeasure
                };

                RepairOrderOutputParameters output;
                using (new OperationContextScope(_client.InnerChannel))
                {
                    Credential userInfo = new Credential();
                    userInfo.username = ROServiceUsername;
                    userInfo.password = ROServicePassword;

                    MessageHeader aMessageHeader = MessageHeader.CreateHeader("Credential", "", userInfo);
                    OperationContext.Current.OutgoingMessageHeaders.Add(aMessageHeader);

                    output = _client.SaveRepairOrder(mock_ro);
                }


                if (!String.IsNullOrWhiteSpace(output.Error_Message))
                {
                    retval.ErrorMessage = output.Error_Message;
                }
                else
                {
                    retval.RepairLineID = (decimal?)Convert.ToDecimal(output.Repair_Line_ID);
                    retval.RepairNumber = output.Repair_Number;
                }
            }
            catch (CommunicationException e)
            {
                retval.ErrorMessage = e.ToString();
                _client.Abort();
            }
            catch (TimeoutException e)
            {
                retval.ErrorMessage = e.ToString();
                _client.Abort();
            }            
            catch (Exception ex)
            {
                retval.ErrorMessage = String.Format("The following error occurred: {0}", ex); 
                _client.Abort();
            }
           

            return retval;*/
        }

        public ServiceRequest LookupSR(decimal incident_id, string incident_num, int _logIncidentId = 0, int _logContactId = 0)
        {return null;
            /*ServiceRequest retval = new ServiceRequest();

            EBSMockSvcClient _client = new EBSMockSvcClient(binding, addr);
            
                //IEBSMockSvc _client = factory.CreateChannel();

            try
            {
                MockEBSServiceRequest mock_sr;
                using (new OperationContextScope(_client.InnerChannel))
                {
                    Credential userInfo = new Credential();
                    userInfo.username = SRServiceUsername;
                    userInfo.password = SRServicePassword;

                    MessageHeader aMessageHeader = MessageHeader.CreateHeader("Credential", "", userInfo);
                    OperationContext.Current.OutgoingMessageHeaders.Add(aMessageHeader);

                    mock_sr = _client.LookupSR(incident_id.ToString()); 
                }
                

                if (!String.IsNullOrWhiteSpace(mock_sr.ErrorMessage))
                {
                    retval.ErrorMessage = mock_sr.ErrorMessage;
                }
                else
                {
                    retval.ContractID = (decimal?)Convert.ToDecimal(mock_sr.ContractID);
                    retval.IncidentOccurredDate = (DateTime?)mock_sr.IncidentOccurredDate;
                    retval.Notes = mock_sr.Notes;
                    retval.Owner = mock_sr.Owner;
                    retval.RequestDate = mock_sr.RequestDate;
                    retval.RequestID = (decimal?)Convert.ToDecimal(mock_sr.RequestID);
                    retval.RequestNumber = mock_sr.RequestNumber;
                    retval.RequestType = mock_sr.RequestType;
                    retval.RequestTypeID = (decimal?)Convert.ToDecimal(mock_sr.RequestTypeID);
                    retval.SerialNumber = mock_sr.SerialNumber;
                    retval.Severity = mock_sr.Severity;
                    retval.SeverityID = (decimal?)Convert.ToDecimal(mock_sr.SeverityID);
                    retval.Status = mock_sr.Status;
                    retval.StatusID = (decimal?)Convert.ToDecimal(mock_sr.StatusID);
                    retval.Summary = mock_sr.Summary;
                    retval.EbsContactID = mock_sr.ContactID.ToString();
                }

                _client.Close();
            }
            catch (CommunicationException e)
            {
                retval.ErrorMessage = e.ToString();
                _client.Abort();
            }
            catch (TimeoutException e)
            {
                retval.ErrorMessage = e.ToString();
                _client.Abort();
            }
            catch (Exception e)
            {
                retval.ErrorMessage = e.ToString();
                _client.Abort();
            }
            

            return retval;*/
        }

        /*  call EBSMockSvcClient : GetSRInfoById()
         *  Output : op.X_SR_LIST 
         *  call dictAddProperty() to add the individual property name, type, and value
         *  for dynamic columns feature
         */
        public Dictionary<string, string> LookupSRDetail(decimal incident_id, string incident_number, int _logIncidentId = 0, int _logContactId = 0)
        {
            EBSMockSvcClient _client = new EBSMockSvcClient(binding, addr);
            SRListOutputParameters op = null;

            using (new OperationContextScope(_client.InnerChannel))
            {
                Credential userInfo = new Credential();
                userInfo.username = SRServiceUsername;
                userInfo.password = SRServicePassword;

                MessageHeader aMessageHeader = MessageHeader.CreateHeader("Credential", "", userInfo);
                OperationContext.Current.OutgoingMessageHeaders.Add(aMessageHeader);

                op = _client.GetSRInfoById(incident_id.ToString());
            }
          
            Dictionary<string, string> dictDetail = new Dictionary<string, string>();

            foreach (ServiceRequestItem sr in op.X_SR_LIST)
            {
                foreach (PropertyInfo propertyInfo in sr.GetType().GetProperties())
                {
                    Object propVal = sr.GetType().GetProperty(propertyInfo.Name).GetValue(sr, null);
                    dictAddProperty(propertyInfo, propVal, ref dictDetail);                                      
                }
            }

            return dictDetail;
        }

        public Dictionary<string, string> LookupContactDetail(decimal contact_id, int _logIncidentId = 0, int _logContactId = 0)
        {
            EBSMockSvcClient _client = new EBSMockSvcClient(binding, addr);
            ContactListOutputParameters op = null;

            using (new OperationContextScope(_client.InnerChannel))
            {
                Credential userInfo = new Credential();
                userInfo.username = ContactServiceUsername;
                userInfo.password = ContactServicePassword;

                MessageHeader aMessageHeader = MessageHeader.CreateHeader("Credential", "", userInfo);
                OperationContext.Current.OutgoingMessageHeaders.Add(aMessageHeader);

                op = _client.getContactDetailsByPartyID(contact_id.ToString());
            }

            Dictionary<string, string> dictDetail = new Dictionary<string, string>();

            foreach (ContactPartyItem contact in op.X_CONTACT_LIST)
            {
                foreach (PropertyInfo propertyInfo in contact.GetType().GetProperties())
                {
                    Object propVal = contact.GetType().GetProperty(propertyInfo.Name).GetValue(contact, null);
                    dictAddProperty(propertyInfo, propVal, ref dictDetail);
                }
            }

            return dictDetail;
        }

        /*
         * common function used for adding a property to the dictDetail (a dictionary map)
         * Dictionary<string, string> :
         * key is the property name
         * value is the property type + TYPE_VALUE_DELIMITER + property value
         */
        public void dictAddProperty(PropertyInfo propertyInfo, Object propVal, ref Dictionary<string, string> dictDetail)
        {
            const string TYPE_VALUE_DELIMITER = " ###  ";
            string propName = propertyInfo.Name;
            string typeName = propertyInfo.PropertyType.Name;
            string typeValue = null;
            string value = null;

            switch (typeName)
            {
                case "String":
                    value = propVal != null ? propVal.ToString() : null;
                    typeValue = "String" + TYPE_VALUE_DELIMITER + value;
                    dictDetail.Add(propName, typeValue);
                    break;
                case "Int64":
                    value = propVal != null ? propVal.ToString() : null;
                    typeValue = "Integer" + TYPE_VALUE_DELIMITER + value;
                    dictDetail.Add(propName, typeValue);
                    break;
                case "DateTime":
                    value = propVal != null ? propVal.ToString() : null;                  
                    typeValue = "DateTime" + TYPE_VALUE_DELIMITER + value;
                    dictDetail.Add(propName, typeValue);
                    break;
                case "Boolean": // eg: xxxSpecified is for EBS internal use, and should not show in the report schema
                    if (!propName.EndsWith("Specified"))
                    {
                        value = propVal != null ? propVal.ToString() : null;
                        typeValue = "Boolean" + TYPE_VALUE_DELIMITER + value;
                        dictDetail.Add(propName, typeValue);
                    }
                    break;
            }
        }

        /* Called by SRdetailVirtualTable() constructor 
         * when report addIn is loaded when CX launch
         */
        public Dictionary<string, string> getServiceRequestDetailSchema()
        {
            ServiceRequestItem srDetailObject = new ServiceRequestItem();
            Dictionary<string, string> dictDetail = new Dictionary<string, string>();
            foreach (PropertyInfo propertyInfo in srDetailObject.GetType().GetProperties())
            {
                dictAddProperty(propertyInfo, null, ref dictDetail);
            }
            return dictDetail;
        }

        /* Called by ContactDetailVirtualTable() constructor 
         * when report addIn is loaded when CX launch
         */
        public Dictionary<string, string> getContactDetailSchema()
        {
            ContactPartyItem contactDetailObject = new ContactPartyItem();
            Dictionary<string, string> dictDetail = new Dictionary<string, string>();
            foreach (PropertyInfo propertyInfo in contactDetailObject.GetType().GetProperties())
            {
                dictAddProperty(propertyInfo, null, ref dictDetail);
            }
            return dictDetail;
        }

        public ServiceRequest[] LookupSRbyContactPartyID(decimal contact_id, int _logIncidentId = 0, int _logContactId = 0)
        {
            EBSMockSvcClient _client = new EBSMockSvcClient(binding, addr);
            SRListOutputParameters op = null;
            using (new OperationContextScope(_client.InnerChannel))
            {
                Credential userInfo = new Credential();
                userInfo.username = SRServiceUsername;
                userInfo.password = SRServicePassword;

                MessageHeader aMessageHeader = MessageHeader.CreateHeader("Credential", "", userInfo);
                OperationContext.Current.OutgoingMessageHeaders.Add(aMessageHeader);

                op = _client.GetSRInfoByContact(contact_id.ToString(), 100);
            }

            ServiceRequest[] retvals = new ServiceRequest[op.X_SR_LIST.Length];
            int i = 0;
            foreach (ServiceRequestItem mock_sr in op.X_SR_LIST)
            {
                /*  only want to get SRs where there is no incidents.
                *  incidentID 0 means null in mock service, to be safe
                *  also check for IncidentRef as well
                */
                if (mock_sr.EXTATTRIBUTE14 == null || mock_sr.EXTATTRIBUTE15 == null)
                {
                    ServiceRequest retval = new ServiceRequest();

                    retval.RequestDate = mock_sr.CREATION_DATE;
                    retval.RequestID = (decimal?)Convert.ToDecimal(mock_sr.INCIDENT_ID);
                    retval.RequestNumber = mock_sr.INCIDENT_NUMBER;
                    retval.RequestType = mock_sr.INCIDENT_TYPE;
                    retval.RequestTypeID = (decimal?)Convert.ToDecimal(mock_sr.INCIDENT_TYPE_ID);
                    retval.SerialNumber = mock_sr.SERIAL_NUMBER;
                    retval.Severity = mock_sr.INCIDENT_SEVERITY;
                    retval.SeverityID = (decimal?)Convert.ToDecimal(mock_sr.INCIDENT_SEVERITY);
                    retval.Status = mock_sr.INCIDENT_STATUS;
                    retval.StatusID = (decimal?)Convert.ToDecimal(mock_sr.INCIDENT_STATUS_ID);
                    retval.Summary = mock_sr.SUMMARY;

                    retvals[i] = retval;
                    i++;
                }
            }
            return retvals;
        }


        public ServiceRequest UpdateSR(ServiceRequest sr, int _logIncidentId = 0, int _logContactId = 0)
        {
            return null;
            //throw new NotImplementedException();
            /*ServiceRequest retval = new ServiceRequest();

            EBSMockSvcClient _client = new EBSMockSvcClient(binding, addr);

            //IEBSMockSvc _client = factory.CreateChannel();


            try
            {
                MockEBSServiceRequest mock_sr = new MockEBSServiceRequest()
                {
                    RequestID = Convert.ToInt64(sr.RequestID),
                    Notes = sr.Notes,
                    Owner = sr.Owner,
                    RequestDate = sr.RequestDate,
                    RequestType = sr.RequestType,
                    SerialNumber = sr.SerialNumber,
                    Severity = sr.Severity,
                    Status = sr.Status,
                    Summary = sr.Summary,
                    IncidentID = Convert.ToInt64(sr.IncidentID),
                    IncidentRef = sr.IncidentRef,
                    IncidentOccurredDate = sr.IncidentOccurredDate.Value,
                    RequestTypeID = Convert.ToInt64(sr.RequestTypeID.Value),
                    SeverityID = Convert.ToInt64(sr.SeverityID.Value),
                    StatusID = Convert.ToInt64(sr.StatusID.Value),
                    ContractID = Convert.ToInt64(sr.ContractID.Value),
                    ErrorMessage = null,
                    RequestNumber = sr.RequestNumber
                };

                if (!String.IsNullOrEmpty(sr.EbsContactID))
                {
                    mock_sr.ContactID = Convert.ToInt64(sr.EbsContactID);
                }

                SROutputParameters output;
                using (new OperationContextScope(_client.InnerChannel))
                {
                    Credential userInfo = new Credential();
                    userInfo.username = SRServiceUsername;
                    userInfo.password = SRServicePassword;

                    MessageHeader aMessageHeader = MessageHeader.CreateHeader("Credential", "", userInfo);
                    OperationContext.Current.OutgoingMessageHeaders.Add(aMessageHeader);

                    output = _client.wssUpdateSR(mock_sr);
                }
                 
                if (!String.IsNullOrWhiteSpace(output.Error_Message))
                {
                    retval.ErrorMessage = output.Error_Message;
                }
                else
                {
                    retval.RequestID = (decimal?)Convert.ToDecimal(output.RequestID);
                    retval.RequestNumber = Convert.ToString(output.RequestNumber);
                }
            }
            catch (CommunicationException e)
            {
                retval.ErrorMessage = e.ToString();
                _client.Abort();
            }
            catch (TimeoutException e)
            {
                retval.ErrorMessage = e.ToString();
                _client.Abort();
            }
            catch (Exception ex)
            {
                retval.ErrorMessage = String.Format("The following error occurred: {0}", ex);
                _client.Abort();
            }

            return retval;*/
        }
        public RepairOrder UpdateRO(RepairOrder ro, int _logIncidentId = 0, int _logContactId = 0)
        {
            //throw new NotImplementedException();
            RepairOrder retval = new RepairOrder();
            /*
            EBSMockSvcClient _client = new EBSMockSvcClient(binding, addr);

            try
            {
                MockEBSRepairOrder mock_ro = new MockEBSRepairOrder()
                {
                    ApprovalStatus = ro.ApprovalStatus,
                    Quantity = Convert.ToInt64(ro.Quantity),

                    //Sophia: Change later
                    ProblemDescription = ro.ProblemDescription,
                    RepairLineID = Convert.ToInt64(ro.RepairLineID)-2     

                };

                RepairOrderOutputParameters output;
                using (new OperationContextScope(_client.InnerChannel))
                {
                    Credential userInfo = new Credential();
                    userInfo.username = ROServiceUsername;
                    userInfo.password = ROServicePassword;

                    MessageHeader aMessageHeader = MessageHeader.CreateHeader("Credential", "", userInfo);
                    OperationContext.Current.OutgoingMessageHeaders.Add(aMessageHeader);

                    output = _client.UpdateRepairOrder(mock_ro);
                }

                if (!String.IsNullOrWhiteSpace(output.Error_Message))
                {
                    retval.ErrorMessage = output.Error_Message;
                }
                else
                {
                    retval.RepairLineID = (decimal?)Convert.ToDecimal(output.Repair_Line_ID);
                }
            }
            catch (CommunicationException e)
            {
                retval.ErrorMessage = e.ToString();
                _client.Abort();
            }
            catch (TimeoutException e)
            {
                retval.ErrorMessage = e.ToString();
                _client.Abort();
            }
            catch (Exception ex)
            {
                retval.ErrorMessage = String.Format("The following error occurred: {0}", ex);
                _client.Abort();
            }
            */
            retval.RepairLineID = ro.RepairLineID;
            return retval;
        }
        public Interaction CreateInteraction(Interaction interaction, int _logIncidentId = 0, int _logContactId = 0)
        {
            return null;
            //throw new NotImplementedException();
            /*Interaction retval = new Interaction();

            EBSMockSvcClient _client = new EBSMockSvcClient(binding, addr);
            try
            {
                MockEBSInteraction mock_interaction = new MockEBSInteraction()
                {
                    Author = interaction.Author,
                    Channel = interaction.Channel,
                    Content = interaction.Content,
                    Created = interaction.Created,
                    InteractionID = Convert.ToInt64(interaction.InteractionID),
                    SrID = Convert.ToInt64(interaction.SrID),
                };

                InteractionOutputParameters output;
                using (new OperationContextScope(_client.InnerChannel))
                {
                    Credential userInfo = new Credential();
                    userInfo.username = InteractionUsername;
                    userInfo.password = InteractionPassword;

                    MessageHeader aMessageHeader = MessageHeader.CreateHeader("Credential", "", userInfo);
                    OperationContext.Current.OutgoingMessageHeaders.Add(aMessageHeader);

                    output = _client.wssCreateInteractionsForSR_ID(mock_interaction);
                }

                if (!String.IsNullOrWhiteSpace(output.Error_Message))
                {
                    retval.ErrorMessage = output.Error_Message;
                }
                else
                {
                    retval.InteractionID = (decimal?)Convert.ToDecimal(output.InteractionID);
                }
            }
            catch (CommunicationException e)
            {
                retval.ErrorMessage = e.ToString();
                _client.Abort();
            }
            catch (TimeoutException e)
            {
                retval.ErrorMessage = e.ToString();
                _client.Abort();
            }
            catch (Exception ex)
            {
                retval.ErrorMessage = String.Format("The following error occurred: {0}", ex);
                _client.Abort();
            }
            

            return retval;*/

        }

        public Contact[] LookupContactList(string firstname, string lastname, string phone, string email, int _logIncidentId = 0, int _logContactId = 0)
        {
            return null;
           /* Contact retval = new Contact();
            EBSMockSvcClient _client = new EBSMockSvcClient(binding, addr);

            try
            {
                MockEBSContact[] contacts;
                using (new OperationContextScope(_client.InnerChannel))
                {
                    Credential userInfo = new Credential();
                    userInfo.username = ROServiceUsername;
                    userInfo.password = ROServicePassword;

                    MessageHeader aMessageHeader = MessageHeader.CreateHeader("Credential", "", userInfo);
                    OperationContext.Current.OutgoingMessageHeaders.Add(aMessageHeader);

                    contacts = _client.cxLookupContact(phone, email, firstname, lastname);
                }
                Contact[] retvals = new Contact[contacts.Length];
                int i = 0;
                foreach (MockEBSContact contact in contacts)
                {
                    retval = new Contact();
                    if (!String.IsNullOrWhiteSpace(contact.ErrorMessage))
                    {
                        retval.ErrorMessage = contact.ErrorMessage;
                        break;
                    }
                    else
                    {
                        retval.ContactPartyID = Convert.ToDecimal(contact.ContactID);
                        retval.Email = contact.EmailAddress;
                        retval.PhoneNumber = contact.PhoneNumber;
                        retval.FirstName = contact.FirstName;
                        retval.LastName = contact.LastName;
                    }
                    retvals[i] = retval;
                    i++;
                }
                return retvals;
            }
            catch (CommunicationException e)
            {
                retval.ErrorMessage = e.ToString();
                _client.Abort();
            }
            catch (TimeoutException e)
            {
                retval.ErrorMessage = e.ToString();
                _client.Abort();
            }
            catch (Exception e)
            {
                retval.ErrorMessage = e.ToString();
                _client.Abort();
            }
            Contact[] retvals_error = { retval };
            return retvals_error;*/
         }
 
        public Item[] LookupItemList(string list_url, decimal contact_org_id, string active_instance_only,
            int rntIncidentId = 0, int rntContactId = 0)
        {
            return null;
        }

        public Dictionary<string, ReportColumnType> getItemSchema()
        {
            return getVirtualTableSchema(new Item());
        }

        public Entitlement[] LookupEntitlementList(decimal instance_id, string validate_flag)
        {
            return null;
        }

        public Dictionary<string, ReportColumnType> getEntitlementSchema()
        {
            return getVirtualTableSchema(new Entitlement());
        }

        public RepairOrder[] LookupRepairOrderList(decimal contact_id, decimal incident_id, string incident_number, string repair_number,
            int rntIncidentId = 0, int rntContactId = 0)
        {
            return null;
        }

        public Dictionary<string, ReportColumnType> getRepairOrderListSchema()
        {
            return getVirtualTableSchema(new RepairOrder());
        }

        public RepairLogistics[] LookupRepairLogisticsList(decimal repair_order_id)
        {
            return null;
        }

        public Dictionary<string, ReportColumnType> getRepairLogisticsListSchema()
        {
            return getVirtualTableSchema(new RepairLogistics());
        }

        #endregion

        #region Helper function
        //Status Convert
        public KeyValuePair<String,String> rnStatusToServerStatus (int rnStatusID)
        {
            KeyValuePair<String, String> rnStatusToMockStatus;
            switch (rnStatusID)
            {
                case 1:
                    rnStatusToMockStatus = new KeyValuePair<String, String>("1", "Open");
                    break;
                case 3:
                case 8: 
                    rnStatusToMockStatus = new KeyValuePair<String, String>("2", "In Progress");
                    break;
                case 2:
                    rnStatusToMockStatus = new KeyValuePair<String, String>("3", "Close");
                    break;
                default:
                    rnStatusToMockStatus = new KeyValuePair<String, String>("1", "Open");
                    break;
            }
            return rnStatusToMockStatus;

        }

        public KeyValuePair<String, String> rnSeverityToServerSeverity(int rnSeverityID)
        {
            KeyValuePair<String, String> rnSeverityToMockSeverity;
            switch (rnSeverityID)
            {
                case 1:
                    rnSeverityToMockSeverity = new KeyValuePair<String, String>("4", "High");
                    break;
                case 2:
                    rnSeverityToMockSeverity = new KeyValuePair<String, String>("5", "Medium");
                    break;
                case 3:
                    rnSeverityToMockSeverity = new KeyValuePair<String, String>("6", "Low");
                    break;
                default:
                    rnSeverityToMockSeverity = new KeyValuePair<String, String>("5", "Medium");
                    break;
            }
            return rnSeverityToMockSeverity;

        }

        public KeyValuePair<String, String> rnRequestTypeToServerRequestType(int rnSeverityID)
        {
            KeyValuePair<String, String> rnRequestTypeToMockRequestType;
            switch (rnSeverityID)
            {
                case 129:
                    rnRequestTypeToMockRequestType = new KeyValuePair<String, String>("13150", "Break/Fix Repair");
                    break;
                case 130:
                    rnRequestTypeToMockRequestType = new KeyValuePair<String, String>("86", "RMA - Advanced Replacement");
                    break;
                default:
                    rnRequestTypeToMockRequestType = new KeyValuePair<String, String>("86", "RMA - Advanced Replacement");
                    break;
            }
            return rnRequestTypeToMockRequestType;

        }

        private Dictionary<string, ReportColumnType> getVirtualTableSchema(object o)
        {
            Dictionary<string, ReportColumnType> dictSchema = new Dictionary<string, ReportColumnType>();
            foreach (PropertyInfo propertyInfo in o.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                string propName = propertyInfo.Name;
                string typeName = propertyInfo.PropertyType.Name;
                switch (typeName)
                {
                    case "String":
                        dictSchema.Add(propName, ReportColumnType.String);
                        break;
                    case "Nullable`1":
                        string nullableType = propertyInfo.PropertyType.GetGenericArguments()[0].ToString();
                        if (nullableType == "System.Decimal")
                            dictSchema.Add(propName, ReportColumnType.Integer);
                        else if (nullableType == "System.DateTime")
                            dictSchema.Add(propName, ReportColumnType.DateTime);
                        break;
                    case "Boolean":
                        if (!propName.EndsWith("Specified"))
                        {
                            dictSchema.Add(propName, ReportColumnType.Boolean);
                        }
                        break;
                }
            }
            return dictSchema;
        }



        #endregion
    }
   
}
