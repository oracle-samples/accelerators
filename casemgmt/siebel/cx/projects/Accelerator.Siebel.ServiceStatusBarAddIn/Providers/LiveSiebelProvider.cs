/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC Contact Center + Siebel Case Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  Siebel release: 8.1.1.15
 *  reference: 150520-000047
 *  date: Mon Nov 30 20:14:30 PST 2015

 *  revision: rnw-15-11-fixes-release-2
 *  SHA1: $Id: 96da9d78664cbc6f84f50590a26e2dc854d232c6 $
 * *********************************************************************************************
 *  File: LiveSiebelProvider.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using Accelerator.Siebel.SharedServices.Logs;
using System.Web.Script.Serialization;
using System.Diagnostics;
using RightNow.AddIns.AddInViews;

using Accelerator.Siebel.SharedServices;
using System.ServiceModel;
using System.ServiceModel.Channels;
using SRSVC = Accelerator.Siebel.SharedServices.ServiceRequestRef;
using CONTACTSVC = Accelerator.Siebel.SharedServices.ContactRef;
using ASSETSVC = Accelerator.Siebel.SharedServices.AssetMgmtRef;

namespace Accelerator.Siebel.SharedServices.Providers
{
    internal class LiveSiebelProvider : ISiebelProvider
    {
        #region Properties
        private HttpBindingBase binding;
        private EndpointAddress addr;

        public string SRURL
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

        public int SRServiceTimeout
        {
            get;
            set;
        }

        public string ContactURL
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

        public string NoteURL
        {
            get;
            set;
        }

        public string NoteServiceUsername
        {
            get;
            set;
        }

        public string NoteServicePassword
        {
            get;
            set;
        }

        public int NoteServiceTimeout
        {
            get;
            set;
        }

        public string ActivityURL
        {
            get;
            set;
        }

        public string ActivityServiceUsername
        {
            get;
            set;
        }

        public string ActivityServicePassword
        {
            get;
            set;
        }

        public int ActivityServiceTimeout
        {
            get;
            set;
        }




        public Logs.LogWrapper log { get; set; }

        #endregion

        #region Init Methods

        public void InitForContact(string contact_url, string user_name, string password, int timeout)
        {
            ContactURL = contact_url;
            ContactServiceUsername = user_name;
            ContactServicePassword = password;
            ContactServiceTimeout = timeout;
            InitBindings(contact_url);
        }

        public void InitForSR(string sr_url, string user_name, string password, int timeout)
        {
            SRURL = sr_url;
            SRServiceUsername = user_name;
            SRServicePassword = password;
            SRServiceTimeout = timeout;
            InitBindings(sr_url);
        }



        public void InitForNote(string note_url, string user_name, string password, int timeout)
        {
            NoteURL = note_url;
            NoteServiceUsername = user_name;
            NoteServicePassword = password;
            NoteServiceTimeout = timeout;
            InitBindings(note_url);
        }

        // Activity
        public void InitForActivity(string activity_url, string user_name, string password, int timeout)
        {
            ActivityURL = activity_url;
            ActivityServiceUsername = user_name;
            ActivityServicePassword = password;
            ActivityServiceTimeout = timeout;
            InitBindings(activity_url);
        }
        


        private void InitBindings(string url)
        {
            addr = new EndpointAddress(url);
            if (url.StartsWith("https://"))
            {
                binding = new BasicHttpsBinding();
            }
            else
            {
                binding = new BasicHttpBinding();
            }
        }
        #endregion

        #region Save/Lookup Methods

        /* This method calls
         * WC_Contacts_BSClient.WC_Contacts_BSQueryPage(ip)
         * Its output is a table, foreach loop is used 
         * (even though only one row is returned by setting record limit = 1)
         * ContactData is generated and need to be updated 
         * if proxy is regenerated
         *  call dictAddProperty() to add the individual property name, type, and value
         *  for dynamic columns feature
         */
        public Dictionary<string, string> LookupContactDetail(IList<string> columns, string party_id, int _logIncidentId = 0, int _logContactId = 0)
        {
            // can reuse SRURL, service username and password, is all the same 
            if (String.IsNullOrWhiteSpace(ContactURL) || String.IsNullOrWhiteSpace(ContactServiceUsername) || String.IsNullOrWhiteSpace(ContactServicePassword))
            {
                throw new Exception("Provider's InitForContact not run.");
            }
            string request, response, logMessage, logNote;
            CONTACTSVC.WC_Contacts_BSClient client = new CONTACTSVC.WC_Contacts_BSClient(binding, addr);
            CONTACTSVC.WC_Contacts_BSQueryPage_Input ip = new CONTACTSVC.WC_Contacts_BSQueryPage_Input();
            MyEndpointBehavior eBehavior = new MyEndpointBehavior();
            client.Endpoint.Behaviors.Add(eBehavior);
            if(ContactServiceTimeout > 0)
                client.InnerChannel.OperationTimeout = TimeSpan.FromMilliseconds(ContactServiceTimeout);

            ip.ListOfWc_Contacts_Io = new CONTACTSVC.ListOfWc_Contacts_IoQuery();
            ip.ListOfWc_Contacts_Io.Contact = new CONTACTSVC.ContactQuery();

            foreach (PropertyInfo propertyInfo in ip.ListOfWc_Contacts_Io.Contact.GetType().GetProperties())
            {
                foreach (string column in columns)
                {
                    if (propertyInfo.Name == column.Split('.')[1])
                    {
                        if (propertyInfo.PropertyType == typeof(CONTACTSVC.queryType))
                        {
                            CONTACTSVC.queryType queryType = new CONTACTSVC.queryType();
                            propertyInfo.SetValue(ip.ListOfWc_Contacts_Io.Contact, queryType, null);
                        }
                        break;
                    }
                }
            }

            if (ip.ListOfWc_Contacts_Io.Contact.Id == null)
                ip.ListOfWc_Contacts_Io.Contact.Id = new CONTACTSVC.queryType();

            ip.ListOfWc_Contacts_Io.Contact.Id.Value = "='" + party_id + "'";

            ip.LOVLanguageMode = "LIC";
            ip.ViewMode = "All";

            CONTACTSVC.WC_Contacts_BSQueryPage_Output opList;
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                using (new OperationContextScope(client.InnerChannel))
                {
                    MessageHeader usrMsgHdr = MessageHeader.CreateHeader("UsernameToken", "http://siebel.com/webservices", ContactServiceUsername);
                    OperationContext.Current.OutgoingMessageHeaders.Add(usrMsgHdr);
                    MessageHeader pwdMsgHdr = MessageHeader.CreateHeader("PasswordText", "http://siebel.com/webservices", ContactServicePassword);
                    OperationContext.Current.OutgoingMessageHeaders.Add(pwdMsgHdr);
                    stopwatch.Start();
                    opList = client.WC_Contacts_BSQueryPage(ip);
                    stopwatch.Stop();
                    request = eBehavior.msgInspector.reqPayload;
                    logMessage = "Request of contact detail (Success). Siebel Contact ID = " + party_id;
                    logNote = "Request Payload: " + request;
                    log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);
                    response = eBehavior.msgInspector.resPayload;
                    logMessage = "Response of contact detail (Success). Siebel Contact ID = " + party_id;
                    logNote = "Response Payload: " + response;
                    log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);
                }
            }
            catch (Exception ex)
            {
                request = eBehavior.msgInspector.reqPayload;
                logMessage = "Request of contact detail (Failure). Siebel Contact ID = " + party_id;
                logNote = "Request Payload: " + request;
                log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);
                response = eBehavior.msgInspector.resPayload;
                logMessage = "Response of contact detail (Failure). Siebel Contact ID = " + party_id;
                logNote = "Response Payload: " + response;
                log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote, (int)stopwatch.ElapsedMilliseconds);
                handleSiebelException(ex, "client.WC_Contacts_BSQueryPage(ip)");
                throw ex;
            }
            Dictionary<string, string> dictDetail = new Dictionary<string, string>();

            CONTACTSVC.ContactData contactData = opList.ListOfWc_Contacts_Io.Contact[0];
            foreach (PropertyInfo propertyInfo in contactData.GetType().GetProperties())
            {
                Object propVal = contactData.GetType().GetProperty(propertyInfo.Name).GetValue(contactData, null);
                dictAddProperty(propertyInfo, propVal, ref dictDetail);
            }

            return dictDetail;
        }

        /*  call WC_Service_Request_BSClient : WC_Service_Request_BSQueryPage
         *  Output : WC_Service_Request_BSQueryPage_Output.ListOfWc_Service_Request_Io.ServiceRequest
         *  call dictAddProperty() to add the individual property name, type, and value
         *  for dynamic columns feature
         */
        public Dictionary<string, string> LookupSRDetail(IList<string> columns, string srId, int _logIncidentId = 0, int _logContactId = 0)
        {
            // can reuse Contact URL, service username and password, is all the same 
            if (String.IsNullOrWhiteSpace(SRURL) || String.IsNullOrWhiteSpace(SRServiceUsername) || String.IsNullOrWhiteSpace(SRServicePassword))
            {
                throw new Exception("Provider's InitForContact not run.");
            }
            string request, response, logMessage, logNote;
            SRSVC.WC_Service_Request_BSClient client = new SRSVC.WC_Service_Request_BSClient(binding, addr);
            MyEndpointBehavior eBehavior = new MyEndpointBehavior();
            client.Endpoint.Behaviors.Add(eBehavior);
            if(SRServiceTimeout > 0)
                client.InnerChannel.OperationTimeout = TimeSpan.FromMilliseconds(SRServiceTimeout );
            SRSVC.WC_Service_Request_BSQueryPage_Input ip = new SRSVC.WC_Service_Request_BSQueryPage_Input();
            ip.ListOfWc_Service_Request_Io = new SRSVC.ListOfWc_Service_Request_IoQuery();
            ip.ListOfWc_Service_Request_Io.ServiceRequest = new SRSVC.ServiceRequestQuery();

            foreach (PropertyInfo propertyInfo in ip.ListOfWc_Service_Request_Io.ServiceRequest.GetType().GetProperties())
            {
                foreach (string column in columns)
                {
                    if (propertyInfo.Name == column.Split('.')[1])
                    {
                        if (propertyInfo.PropertyType == typeof(SRSVC.queryType))
                        {
                            SRSVC.queryType queryType = new SRSVC.queryType();
                            propertyInfo.SetValue(ip.ListOfWc_Service_Request_Io.ServiceRequest, queryType, null);
                        }
                        break;
                    }
                }
            }

            ip.ListOfWc_Service_Request_Io.ServiceRequest.Id.Value = "='" + srId + "'";

            ip.LOVLanguageMode = "LIC";
            ip.ViewMode = "All";

            Stopwatch stopwatch = new Stopwatch();
            SRSVC.WC_Service_Request_BSQueryPage_Output opList;
            try
            {
                using (new OperationContextScope(client.InnerChannel))
                {
                    MessageHeader usrMsgHdr = MessageHeader.CreateHeader("UsernameToken", "http://siebel.com/webservices", SRServiceUsername);
                    OperationContext.Current.OutgoingMessageHeaders.Add(usrMsgHdr);
                    MessageHeader pwdMsgHdr = MessageHeader.CreateHeader("PasswordText", "http://siebel.com/webservices", SRServicePassword);
                    OperationContext.Current.OutgoingMessageHeaders.Add(pwdMsgHdr);
                    stopwatch.Start();
                    opList = client.WC_Service_Request_BSQueryPage(ip);
                    stopwatch.Stop();
                    request = eBehavior.msgInspector.reqPayload;
                    logMessage = "Request of SR detail (Success). Siebel srId = " + srId;
                    logNote = "Request Payload: " + request;
                    log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);
                    response = eBehavior.msgInspector.resPayload;
                    logMessage = "Response of SR detail (Success). Siebel srId = " + srId;
                    logNote = "Response Payload: " + response;
                    log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote, (int)stopwatch.ElapsedMilliseconds);
                }
            }
            catch (Exception ex)
            {
                opList = client.WC_Service_Request_BSQueryPage(ip);
                request = eBehavior.msgInspector.reqPayload;
                logMessage = "Request of SR detail (Failure). Siebel srId = " + srId;
                logNote = "Request Payload: " + request;
                log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);
                response = eBehavior.msgInspector.resPayload;
                logMessage = "Response of SR detail (Failure). Siebel srId = " + srId;
                logNote = "Response Payload: " + response;
                log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);
                handleSiebelException(ex, "client.WC_Contacts_BSQueryPage(ip)");
                throw ex;
            }
            Dictionary<string, string> dictDetail = new Dictionary<string, string>();

            SRSVC.ServiceRequestData srData = opList.ListOfWc_Service_Request_Io.ServiceRequest[0];
            foreach (PropertyInfo propertyInfo in srData.GetType().GetProperties())
            {
                Object propVal = srData.GetType().GetProperty(propertyInfo.Name).GetValue(srData, null);
                dictAddProperty(propertyInfo, propVal, ref dictDetail);
            }

            return dictDetail;
        }

        public ContactModel[] LookupContactList(string firstname, string lastname, string phone, string email, int _logIncidentId = 0, int _logContactId = 0)
        {
            string request, response, logMessage, logNote;
            CONTACTSVC.WC_Contacts_BSClient client = new CONTACTSVC.WC_Contacts_BSClient(binding, addr);
            MyEndpointBehavior eBehavior = new MyEndpointBehavior();
            client.Endpoint.Behaviors.Add(eBehavior);
            if (ContactServiceTimeout > 0)
                client.InnerChannel.OperationTimeout = TimeSpan.FromMilliseconds(ContactServiceTimeout );
            ContactModel[] retvals = null;


            if (String.IsNullOrWhiteSpace(ContactURL) || String.IsNullOrWhiteSpace(ContactServiceUsername) || String.IsNullOrWhiteSpace(ContactServicePassword))
            {
                throw new Exception("Provider's InitForContact not run.");
            }

            //WC_Contacts_BS client = SiebelProxyFactory.GetContactInstance(ContactListLookupURL, ContactServiceUsername, ContactServicePassword, ContactServiceTimeout);
            CONTACTSVC.WC_Contacts_BSQueryPage_Input ip = new CONTACTSVC.WC_Contacts_BSQueryPage_Input();

            ip.ListOfWc_Contacts_Io = new CONTACTSVC.ListOfWc_Contacts_IoQuery();
            ip.ListOfWc_Contacts_Io.Contact = new CONTACTSVC.ContactQuery();

            ip.ListOfWc_Contacts_Io.Contact.EmailAddress = new CONTACTSVC.queryType();
            ip.ListOfWc_Contacts_Io.Contact.EmailAddress.Value = String.Empty;

            ip.ListOfWc_Contacts_Io.Contact.WorkPhone = new CONTACTSVC.queryType();
            ip.ListOfWc_Contacts_Io.Contact.WorkPhone.Value = String.Empty;

            ip.ListOfWc_Contacts_Io.Contact.FirstName = new CONTACTSVC.queryType();
            ip.ListOfWc_Contacts_Io.Contact.FirstName.Value = String.Empty;

            ip.ListOfWc_Contacts_Io.Contact.LastName = new CONTACTSVC.queryType();
            ip.ListOfWc_Contacts_Io.Contact.LastName.Value = String.Empty;

            string searchSpec = "";

            string firstNameSearchSpec = "";
            if (!String.IsNullOrEmpty(firstname))
            {
                firstNameSearchSpec = "[FirstName]~LIKE '" + firstname + "'";
                searchSpec = firstNameSearchSpec;
            }

            string lastNameSearchSpec = "";
            if (!String.IsNullOrEmpty(lastname))
            {
                lastNameSearchSpec = "[LastName]~LIKE '" + lastname + "'";
                searchSpec = lastNameSearchSpec;
            }

            string nameSearchSpec = "";
            if (firstNameSearchSpec != "" && lastNameSearchSpec != "")
            {
                nameSearchSpec = "(" + firstNameSearchSpec + " AND " + lastNameSearchSpec + ")";
                searchSpec = nameSearchSpec;
            }

            if (!String.IsNullOrEmpty(email))
            {
                if (searchSpec != "")
                {
                    searchSpec = searchSpec + " OR [EmailAddress]~LIKE '" + email + "'";
                }
                else
                {
                    searchSpec = searchSpec + "[EmailAddress]~LIKE '" + email + "'";
                }
            }

           

            if (!String.IsNullOrEmpty(phone))
            {
                if (searchSpec != "")
                {
                    searchSpec = searchSpec + " OR [WorkPhone]~LIKE '" + phone + "'";
                }
                else
                {
                    searchSpec = searchSpec + "[WorkPhone]~LIKE '" + phone + "'";
                }
            }

            ip.ListOfWc_Contacts_Io.Contact.searchspec = searchSpec;
            ip.ListOfWc_Contacts_Io.pagesize = "20";

            ip.ListOfWc_Contacts_Io.Contact.Id = new CONTACTSVC.queryType();
            ip.ListOfWc_Contacts_Io.Contact.Id.Value = String.Empty;

            ip.ListOfWc_Contacts_Io.Contact.PrimaryOrganizationId = new CONTACTSVC.queryType();
            ip.ListOfWc_Contacts_Io.Contact.PrimaryOrganizationId.Value = String.Empty;

            ip.LOVLanguageMode = "LIC";
            ip.ViewMode = "All";
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                CONTACTSVC.WC_Contacts_BSQueryPage_Output opList;
                using (new OperationContextScope(client.InnerChannel))
                {
                    MessageHeader usrMsgHdr = MessageHeader.CreateHeader("UsernameToken", "http://siebel.com/webservices", ContactServiceUsername);
                    OperationContext.Current.OutgoingMessageHeaders.Add(usrMsgHdr);
                    MessageHeader pwdMsgHdr = MessageHeader.CreateHeader("PasswordText", "http://siebel.com/webservices", ContactServicePassword);
                    OperationContext.Current.OutgoingMessageHeaders.Add(pwdMsgHdr);
                    stopwatch.Start();
                    opList = client.WC_Contacts_BSQueryPage(ip);
                    stopwatch.Stop();
                    request = eBehavior.msgInspector.reqPayload;
                    response = eBehavior.msgInspector.resPayload;
                }

                if (opList.ListOfWc_Contacts_Io.Contact == null)
                    return retvals;

                List<ContactModel> contacts = new List<ContactModel>();

                foreach (CONTACTSVC.ContactData op in opList.ListOfWc_Contacts_Io.Contact)
                {
                    ContactModel contact = new ContactModel();
                    contact.ContactPartyID = op.Id;
                    contact.FirstName = op.FirstName;
                    contact.LastName = op.LastName;
                    contact.Email = op.EmailAddress;
                    contact.PhoneNumber = op.WorkPhone;
                    contact.ContactOrgID = op.PrimaryOrganizationId;
                    contacts.Add(contact);
                }
                if (contacts.Count > 0)
                    retvals = contacts.ToArray();

                logMessage = "Request of search Contact (Success). FirstName = " + firstname + "; LastName = " + lastname + "Email = " + email + "; Phone = " + phone;
                logNote = "Request Payload: " + request;
                log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);

                logMessage = "Response of search Contact (Success). FirstName = " + firstname + "; LastName = " + lastname + "Email = " + email + "; Phone = " + phone;
                logNote = "Response Payload: " + response;
                log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote, (int)stopwatch.ElapsedMilliseconds);
            }

            catch (Exception ex)
            {
                request = eBehavior.msgInspector.reqPayload;
                response = eBehavior.msgInspector.resPayload;

                List<ContactModel> contacts = new List<ContactModel>();
                ContactModel contact = new ContactModel();
                contact.ErrorMessage = "There has been an error communicating with Siebel. Please check log for detail.";
                contacts.Add(contact);
                retvals = contacts.ToArray();

                logMessage = "Request of search Contact (Failure). Email = " + email + "; Phone = " + phone + ". Error: " + ex.Message;
                logNote = "Request Payload: " + request;
                log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                logMessage = "Response of search Contact (Failure). Email = " + email + "; Phone = " + phone + ". Error: " + ex.Message;
                logNote = "Response Payload: " + response;
                log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                handleSiebelException(ex, "Search Contact", _logIncidentId, _logContactId);
            }

            return retvals;
        }

        /*  This method calls
         *  WC_Contacts_BSClient : WC_Contacts_BSQueryPage
         *  Called by SRlistVirtualTable GetRows() to display Service Requests for a contact
         */
        public ServiceRequest[] LookupSRbyContactPartyID(IList<string> columns, string contact_id, int _logIncidentId = 0, int _logContactId = 0)
        {
            if (String.IsNullOrWhiteSpace(SRURL) || String.IsNullOrWhiteSpace(SRServiceUsername) || String.IsNullOrWhiteSpace(SRServicePassword))
            {
                throw new Exception("Provider's InitForSR is not run.");
            }
            string request, response, logMessage, logNote;
            binding.MaxReceivedMessageSize = 2147483647;
            CONTACTSVC.WC_Contacts_BSClient client = new CONTACTSVC.WC_Contacts_BSClient(binding, addr);
            MyEndpointBehavior eBehavior = new MyEndpointBehavior();
            client.Endpoint.Behaviors.Add(eBehavior);
            if(SRServiceTimeout > 0)
                client.InnerChannel.OperationTimeout = TimeSpan.FromMilliseconds(SRServiceTimeout );
            CONTACTSVC.WC_Contacts_BSQueryPage_Input ip = new CONTACTSVC.WC_Contacts_BSQueryPage_Input();

            ip.ListOfWc_Contacts_Io = new CONTACTSVC.ListOfWc_Contacts_IoQuery();
            ip.ListOfWc_Contacts_Io.Contact = new CONTACTSVC.ContactQuery();

            ip.ListOfWc_Contacts_Io.Contact.Id = new CONTACTSVC.queryType();
            ip.ListOfWc_Contacts_Io.Contact.Id.Value = "='" + contact_id + "'";

            ip.ListOfWc_Contacts_Io.Contact.ListOfServiceRequest = new CONTACTSVC.ListOfServiceRequestQuery();
            ip.ListOfWc_Contacts_Io.Contact.ListOfServiceRequest.ServiceRequest = new CONTACTSVC.ServiceRequestQuery();
            ip.ListOfWc_Contacts_Io.Contact.ListOfServiceRequest.ServiceRequest.SRNumber = new CONTACTSVC.queryType();
           
            foreach (PropertyInfo propertyInfo in ip.ListOfWc_Contacts_Io.Contact.ListOfServiceRequest.ServiceRequest.GetType().GetProperties())
            {
                /* bc Incident/SR report tab is a special case, the columns are hard coded and fixed
                 * to show the combined rnow and siebel rows, IntegratrionId, summary, Id
                 * are diffent name
                 */
                foreach (string column in columns)
                {
                    if (propertyInfo.Name == column.Split('.')[1] ||
                        propertyInfo.Name == "IntegrationId" ||
                        propertyInfo.Name == "Id" ||
                        propertyInfo.Name == "Abstract"
                       )
                    {
                        if (propertyInfo.PropertyType == typeof(CONTACTSVC.queryType))
                        {
                            CONTACTSVC.queryType queryType = new CONTACTSVC.queryType();
                            propertyInfo.SetValue(ip.ListOfWc_Contacts_Io.Contact.ListOfServiceRequest.ServiceRequest, queryType, null);
                        }
                        break;
                    }
                }
            }

            ip.LOVLanguageMode = "LIC";
            ip.ViewMode = "All";

            Stopwatch stopwatch = new Stopwatch();
            CONTACTSVC.WC_Contacts_BSQueryPage_Output opList;
            try
            {
                using (new OperationContextScope(client.InnerChannel))
                {
                    MessageHeader usrMsgHdr = MessageHeader.CreateHeader("UsernameToken", "http://siebel.com/webservices", SRServiceUsername);
                    OperationContext.Current.OutgoingMessageHeaders.Add(usrMsgHdr);
                    MessageHeader pwdMsgHdr = MessageHeader.CreateHeader("PasswordText", "http://siebel.com/webservices", SRServicePassword);
                    OperationContext.Current.OutgoingMessageHeaders.Add(pwdMsgHdr);
                    stopwatch.Start();
                    opList = client.WC_Contacts_BSQueryPage(ip);
                    stopwatch.Stop();
                    request = eBehavior.msgInspector.reqPayload;
                    logMessage = "Request of SRs by contactID (Success). Siebel Contact ID = " + contact_id;
                    logNote = "Request Payload: " + request;
                    log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);
                    response = eBehavior.msgInspector.resPayload;
                    logMessage = "Response of SRs by contactID (Success). Siebel Contact ID = " + contact_id;
                    logNote = "Response Payload: " + response;
                    log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote, (int)stopwatch.ElapsedMilliseconds);
                }
            }
            catch (Exception ex)
            {
                request = eBehavior.msgInspector.reqPayload;
                logMessage = "Request of SRs by contactID (Failure). Siebel Contact ID = " + contact_id;
                logNote = "Request Payload: " + request;
                log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);
                response = eBehavior.msgInspector.resPayload;
                logMessage = "Response of SRs by contactID (Failure). Siebel Contact ID = " + contact_id;
                logNote = "Response Payload: " + response;
                log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);
                handleSiebelException(ex, "LookupSRbyContactPartyID");
                throw ex;
            }

            CONTACTSVC.ContactData contactData = opList.ListOfWc_Contacts_Io.Contact[0];
            if (contactData.ListOfServiceRequest.ServiceRequest == null)
                return null;

            ServiceRequest[] retvals = new ServiceRequest[contactData.ListOfServiceRequest.ServiceRequest.Length];
            int i = 0;
            foreach (CONTACTSVC.ServiceRequestData sr in contactData.ListOfServiceRequest.ServiceRequest)
            {
                if (sr.IntegrationId == null || sr.IntegrationId == "")
                {
                    ServiceRequest req = new ServiceRequest();
                    req.RequestID = sr.Id;
                    req.RequestNumber = sr.SRNumber;
                    req.Status = sr.Status;
                    req.Summary = sr.Abstract;
                    req.RequestDate = (DateTime)sr.Created;
                    retvals[i] = req;
                    i++;
                }
            }

            return retvals;
        }

        public ServiceRequest LookupSR(string incident_id, int _logIncidentId = 0, int _logContactId = 0)
        {
            if (String.IsNullOrWhiteSpace(SRURL) || String.IsNullOrWhiteSpace(SRServiceUsername) || String.IsNullOrWhiteSpace(SRServicePassword))
            {
                throw new Exception("Provider's InitForSR not run.");
            }
            string request, response, logMessage, logNote;

            SRSVC.WC_Service_Request_BSClient client = new SRSVC.WC_Service_Request_BSClient(binding, addr);
            MyEndpointBehavior eBehavior = new MyEndpointBehavior();
            client.Endpoint.Behaviors.Add(eBehavior);
            if(SRServiceTimeout > 0)
                client.InnerChannel.OperationTimeout = TimeSpan.FromMilliseconds(SRServiceTimeout );

            SRSVC.WC_Service_Request_BSQueryPage_Input ip = new SRSVC.WC_Service_Request_BSQueryPage_Input();

            ServiceRequest sr = new ServiceRequest();

            ip.ListOfWc_Service_Request_Io = new SRSVC.ListOfWc_Service_Request_IoQuery();
            ip.ListOfWc_Service_Request_Io.ServiceRequest = new SRSVC.ServiceRequestQuery();
            ip.ListOfWc_Service_Request_Io.ServiceRequest.Id = new SRSVC.queryType();
            if (!String.IsNullOrEmpty(incident_id))
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest.Id.Value = "='" + incident_id + "'";
            }
            else
            {
                throw new Exception("Service Request ID is empty.");
            }

            ip.ListOfWc_Service_Request_Io.ServiceRequest.Description = new SRSVC.queryType();
            ip.ListOfWc_Service_Request_Io.ServiceRequest.Description.Value = String.Empty;

            ip.ListOfWc_Service_Request_Io.ServiceRequest.ContactId = new SRSVC.queryType();
            ip.ListOfWc_Service_Request_Io.ServiceRequest.ContactId.Value = String.Empty;

            ip.ListOfWc_Service_Request_Io.ServiceRequest.OwnedById = new SRSVC.queryType();
            ip.ListOfWc_Service_Request_Io.ServiceRequest.OwnedById.Value = String.Empty;

            ip.ListOfWc_Service_Request_Io.ServiceRequest.Owner = new SRSVC.queryType();
            ip.ListOfWc_Service_Request_Io.ServiceRequest.Owner.Value = String.Empty;

            ip.ListOfWc_Service_Request_Io.ServiceRequest.Priority = new SRSVC.queryType();
            ip.ListOfWc_Service_Request_Io.ServiceRequest.Priority.Value = String.Empty;

            ip.ListOfWc_Service_Request_Io.ServiceRequest.SRNumber = new SRSVC.queryType();
            ip.ListOfWc_Service_Request_Io.ServiceRequest.SRNumber.Value = String.Empty;

            ip.ListOfWc_Service_Request_Io.ServiceRequest.SRType = new SRSVC.queryType();
            ip.ListOfWc_Service_Request_Io.ServiceRequest.SRType.Value = String.Empty;

            ip.ListOfWc_Service_Request_Io.ServiceRequest.SerialNumber = new SRSVC.queryType();
            ip.ListOfWc_Service_Request_Io.ServiceRequest.SerialNumber.Value = String.Empty;

            ip.ListOfWc_Service_Request_Io.ServiceRequest.Severity = new SRSVC.queryType();
            ip.ListOfWc_Service_Request_Io.ServiceRequest.Severity.Value = String.Empty;

            ip.ListOfWc_Service_Request_Io.ServiceRequest.Status = new SRSVC.queryType();
            ip.ListOfWc_Service_Request_Io.ServiceRequest.Status.Value = String.Empty;

            ip.ListOfWc_Service_Request_Io.ServiceRequest.Type = new SRSVC.queryType();
            ip.ListOfWc_Service_Request_Io.ServiceRequest.Type.Value = String.Empty;

            ip.ListOfWc_Service_Request_Io.ServiceRequest.ProductId = new SRSVC.queryType();
            ip.ListOfWc_Service_Request_Io.ServiceRequest.ProductId.Value = String.Empty;

            ip.ListOfWc_Service_Request_Io.ServiceRequest.Product = new SRSVC.queryType();
            ip.ListOfWc_Service_Request_Io.ServiceRequest.Product.Value = String.Empty;


            ip.LOVLanguageMode = "LIC";
            ip.ViewMode = "All";
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                SRSVC.WC_Service_Request_BSQueryPage_Output op;
                using (new OperationContextScope(client.InnerChannel))
                {
                    MessageHeader usrMsgHdr = MessageHeader.CreateHeader("UsernameToken", "http://siebel.com/webservices", SRServiceUsername);
                    OperationContext.Current.OutgoingMessageHeaders.Add(usrMsgHdr);
                    MessageHeader pwdMsgHdr = MessageHeader.CreateHeader("PasswordText", "http://siebel.com/webservices", SRServicePassword);
                    OperationContext.Current.OutgoingMessageHeaders.Add(pwdMsgHdr);
                    stopwatch.Start();
                    op = client.WC_Service_Request_BSQueryPage(ip);
                    stopwatch.Stop();
                    request = eBehavior.msgInspector.reqPayload;
                    response = eBehavior.msgInspector.resPayload;

                    SRSVC.ServiceRequestData opData = op.ListOfWc_Service_Request_Io.ServiceRequest[0];
                    sr.RequestID = opData.Id;
                    sr.RequestNumber = opData.SRNumber;
                    //sr.IncidentID
                    //sr.IncidentRef
                    sr.Status = opData.Status;
                    sr.Severity = opData.Severity;
                    sr.RequestType = opData.SRType;
                    sr.Summary = opData.Description;
                    sr.SerialNumber = opData.SerialNumber;
                    sr.Owner = opData.Owner;
                    sr.OwnerID = opData.OwnedById;
                    sr.Product = opData.ProductId;
                    sr.ProductDescription = opData.Product;
                }

                logMessage = "Request of loading Service Request (Success). SR ID = " + incident_id;
                logNote = "Request Payload: " + request;
                log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);

                logMessage = "Response of loading Service Request (Success). SR ID = " + incident_id;
                logNote = "Response Payload: " + response;
                log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote, (int)stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                request = eBehavior.msgInspector.reqPayload;
                response = eBehavior.msgInspector.resPayload;

                sr.ErrorMessage = "There has been an error communicating with Siebel. Please check log for detail.";
                logMessage = "Request of loading Service Request (Failure). SR ID = " + incident_id + " Error: " + ex.Message;
                logNote = "Request Payload: " + request;
                log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                logMessage = "Response of loading Service Request (Failure). SR ID = " + incident_id + " Error: " + ex.Message;
                logNote = "Response Payload: " + response;
                log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                handleSiebelException(ex, "Lookup Service Request", _logIncidentId, _logContactId);
            }
            return sr;
        }

        public ServiceRequest CreateSR(ServiceRequest sr, int _logIncidentId = 0, int _logContactId = 0)
        {
            if (String.IsNullOrWhiteSpace(SRURL) || String.IsNullOrWhiteSpace(SRServiceUsername) || String.IsNullOrWhiteSpace(SRServicePassword))
            {
                throw new Exception("Provider's InitForSR not run.");
            }

            string request, response, logMessage, logNote;

            SRSVC.WC_Service_Request_BSClient client = new SRSVC.WC_Service_Request_BSClient(binding, addr);
            MyEndpointBehavior eBehavior = new MyEndpointBehavior();
            client.Endpoint.Behaviors.Add(eBehavior);
            if (SRServiceTimeout > 0)
                client.InnerChannel.OperationTimeout = TimeSpan.FromMilliseconds(SRServiceTimeout );

            SRSVC.WC_Service_Request_BSInsert_Input ip = new SRSVC.WC_Service_Request_BSInsert_Input();


            ip.ListOfWc_Service_Request_Io = new SRSVC.ListOfWc_Service_Request_IoData();
            ip.ListOfWc_Service_Request_Io.ServiceRequest = new SRSVC.ServiceRequestData[1];
            ip.ListOfWc_Service_Request_Io.ServiceRequest[0] = new SRSVC.ServiceRequestData();

            if (sr.Summary != null)
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].Description = sr.Summary;
            }
            else
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].Description = String.Empty;
            }

            if (sr.IncidentID != null || sr.IncidentRef != null)
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].IntegrationId = sr.IncidentID + "," + sr.IncidentRef;
            }

            if (sr.ContactID != null)
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].ContactId = sr.ContactID;
            }
            else
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].ContactId = String.Empty;
            }

            if (sr.Severity != null)
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].Severity = sr.Severity;
            }
            else
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].Severity = String.Empty;
            }

            if (sr.RequestType != null)
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].SRType = sr.RequestType;
            }
            else
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].SRType = String.Empty;
            }

            if (sr.SerialNumber != null)
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].SerialNumber = sr.SerialNumber;
            }
            else
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].SerialNumber = String.Empty;
            }

            if (sr.Status != null)
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].Status = sr.Status;
            }
            else
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].Status = String.Empty;
            }

            if (sr.OwnerID != null)
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].OwnedById = sr.OwnerID;
            }
            else
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].OwnedById = String.Empty;
            }

            if (sr.RnowHost != null)
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].IntegrationSite = sr.RnowHost;
            }
            else
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].IntegrationSite = String.Empty;
            }

            ip.LOVLanguageMode = "LIC";
            ip.ViewMode = "All";
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                SRSVC.WC_Service_Request_BSInsert_Output op;
                using (new OperationContextScope(client.InnerChannel))
                {
                    MessageHeader usrMsgHdr = MessageHeader.CreateHeader("UsernameToken", "http://siebel.com/webservices", SRServiceUsername);
                    OperationContext.Current.OutgoingMessageHeaders.Add(usrMsgHdr);
                    MessageHeader pwdMsgHdr = MessageHeader.CreateHeader("PasswordText", "http://siebel.com/webservices", SRServicePassword);
                    OperationContext.Current.OutgoingMessageHeaders.Add(pwdMsgHdr);
                    stopwatch.Start();
                    op = client.WC_Service_Request_BSInsert(ip);
                    stopwatch.Stop();
                    request = eBehavior.msgInspector.reqPayload;
                    response = eBehavior.msgInspector.resPayload;

                    SRSVC.ServiceRequestId opData = op.ListOfWc_Service_Request_Io[0];
                    sr.RequestID = opData.Id;
                }

                logMessage = "Request of creating Service Request (Success). Created SR ID: " + sr.RequestID;
                logNote = "Request Payload: " + request;
                log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);

                logMessage = "Response of creating Service Request (Success). Created SR ID: " + sr.RequestID;
                logNote = "Response Payload: " + response;
                log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote, (int)stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                request = eBehavior.msgInspector.reqPayload;
                response = eBehavior.msgInspector.resPayload;

                sr.ErrorMessage = "There has been an error communicating with Siebel. Please check log for detail.";

                logMessage = "Request of creating Service Request (Failure). " + ex.Message;
                logNote = "Request Payload: " + request;
                log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                logMessage = "Response of creating Service Request (Failure). " + ex.Message;
                logNote = "Response Payload: " + response;
                log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                handleSiebelException(ex, "Create Service Request", _logIncidentId, _logContactId);
            }
            return sr;
        }

        public ServiceRequest UpdateSR(ServiceRequest sr, int _logIncidentId = 0, int _logContactId = 0)
        {
            if (String.IsNullOrWhiteSpace(SRURL) || String.IsNullOrWhiteSpace(SRServiceUsername) || String.IsNullOrWhiteSpace(SRServicePassword))
            {
                throw new Exception("Provider's InitForSR not run.");
            }

            string request, response, logMessage, logNote;

            SRSVC.WC_Service_Request_BSClient client = new SRSVC.WC_Service_Request_BSClient(binding, addr);
            MyEndpointBehavior eBehavior = new MyEndpointBehavior();
            client.Endpoint.Behaviors.Add(eBehavior);
            if(SRServiceTimeout > 0)
                client.InnerChannel.OperationTimeout = TimeSpan.FromMilliseconds(SRServiceTimeout );

            SRSVC.WC_Service_Request_BSUpdate_Input ip = new SRSVC.WC_Service_Request_BSUpdate_Input();


            ip.ListOfWc_Service_Request_Io = new SRSVC.ListOfWc_Service_Request_IoData();
            ip.ListOfWc_Service_Request_Io.ServiceRequest = new SRSVC.ServiceRequestData[1];
            ip.ListOfWc_Service_Request_Io.ServiceRequest[0] = new SRSVC.ServiceRequestData();
            if (sr.RequestID != null)
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].Id = sr.RequestID;
            }
            else
            {
                sr.ErrorMessage = "The following error occurred when doing the Create: SR ID is empty";
                return sr;
            }


            if (sr.Summary != null)
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].Description = sr.Summary;
            }
            else
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].Description = String.Empty;
            }

            if (sr.ContactID != null)
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].ContactId = sr.ContactID;
            }
            else
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].ContactId = String.Empty;
            }

            if (sr.Severity != null)
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].Severity = sr.Severity;
            }
            else
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].Severity = String.Empty;
            }

            if (sr.RequestType != null)
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].SRType = sr.RequestType;
            }
            else
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].SRType = String.Empty;
            }

            if (sr.SerialNumber != null)
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].SerialNumber = sr.SerialNumber;
            }
            else
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].SerialNumber = String.Empty;
            }

            if (sr.Status != null)
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].Status = sr.Status;
            }
            else
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].Status = String.Empty;
            }

            if (sr.IncidentID != null || sr.IncidentRef != null)
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].IntegrationId = sr.IncidentID + "," + sr.IncidentRef;
            }

            if (sr.RnowHost != null)
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].IntegrationSite = sr.RnowHost;
            }
            else
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].IntegrationSite = String.Empty;
            }

            ip.LOVLanguageMode = "LIC";
            ip.ViewMode = "All";
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                SRSVC.WC_Service_Request_BSUpdate_Output op;
                using (new OperationContextScope(client.InnerChannel))
                {
                    MessageHeader usrMsgHdr = MessageHeader.CreateHeader("UsernameToken", "http://siebel.com/webservices", SRServiceUsername);
                    OperationContext.Current.OutgoingMessageHeaders.Add(usrMsgHdr);
                    MessageHeader pwdMsgHdr = MessageHeader.CreateHeader("PasswordText", "http://siebel.com/webservices", SRServicePassword);
                    OperationContext.Current.OutgoingMessageHeaders.Add(pwdMsgHdr);
                    stopwatch.Start();
                    op = client.WC_Service_Request_BSUpdate(ip);
                    stopwatch.Stop();
                    request = eBehavior.msgInspector.reqPayload;
                    response = eBehavior.msgInspector.resPayload;

                    SRSVC.ServiceRequestId opData = op.ListOfWc_Service_Request_Io[0];
                    sr.RequestID = opData.Id;
                }

                logMessage = "Request of updating Service Request (Success). SR ID = " + sr.RequestID;
                logNote = "Request Payload: " + request;
                log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);

                logMessage = "Response of updating Service Request (Success). SR ID = " + sr.RequestID;
                logNote = "Response Payload: " + response;
                log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote, (int)stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                request = eBehavior.msgInspector.reqPayload;
                response = eBehavior.msgInspector.resPayload;
                sr.ErrorMessage = "There has been an error communicating with Siebel. Please check log for detail.";

                logMessage = "Request of updating Service Request (Failure). SR ID = " + sr.RequestID + " Error: " + ex.Message;
                logNote = "Request Payload: " + request;
                log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                logMessage = "Response of updating Service Request (Failure). SR ID = " + sr.RequestID + " Error: " + ex.Message;
                logNote = "Response Payload: " + response;
                log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                handleSiebelException(ex, "Update Service Request", _logIncidentId, _logContactId);
            }
            return sr;
        }


        public Note CreateNote(Note note, int _logIncidentId = 0, int _logContactId = 0)
        {
            if (String.IsNullOrWhiteSpace(NoteURL) || String.IsNullOrWhiteSpace(NoteServiceUsername) || String.IsNullOrWhiteSpace(NoteServicePassword))
            {
                throw new Exception("Provider's InitForSR not run.");
            }

            string request, response, logMessage, logNote;

            SRSVC.WC_Service_Request_BSClient client = new SRSVC.WC_Service_Request_BSClient(binding, addr);
            MyEndpointBehavior eBehavior = new MyEndpointBehavior();
            client.Endpoint.Behaviors.Add(eBehavior);
            if(NoteServiceTimeout > 0)
                client.InnerChannel.OperationTimeout = TimeSpan.FromMilliseconds(NoteServiceTimeout );

            SRSVC.WC_Service_Request_BSInsert_Input ip = new SRSVC.WC_Service_Request_BSInsert_Input();


            ip.ListOfWc_Service_Request_Io = new SRSVC.ListOfWc_Service_Request_IoData();
            ip.ListOfWc_Service_Request_Io.ServiceRequest = new SRSVC.ServiceRequestData[1];
            ip.ListOfWc_Service_Request_Io.ServiceRequest[0] = new SRSVC.ServiceRequestData();
            if (note.SrID != null)
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].Id = note.SrID;
            }
            else
            {
                note.ErrorMessage = "The following error occurred when doing the Create: SR ID is empty";
                return note;
            }

            ip.ListOfWc_Service_Request_Io.ServiceRequest[0].ListOfAction = new SRSVC.ListOfActionData();
            ip.ListOfWc_Service_Request_Io.ServiceRequest[0].ListOfAction.Action = new SRSVC.ActionData[1];
            ip.ListOfWc_Service_Request_Io.ServiceRequest[0].ListOfAction.Action[0] = new SRSVC.ActionData();
            if (note.Content != null)
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].ListOfAction.Action[0].Description2 = note.Summary;
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].ListOfAction.Action[0].Type = note.Channel;
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].ListOfAction.Action[0].Comment = note.Content;
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].ListOfAction.Action[0].Private = note.Status;
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].ListOfAction.Action[0].PrivateSpecified = true;
                //ip.ListOfWc_Service_Request_Io.ServiceRequest[0].ListOfAction.Action[0].TypeName = "Notes";
            }



            ip.LOVLanguageMode = "LIC";
            ip.ViewMode = "All";
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                SRSVC.WC_Service_Request_BSInsert_Output op;
                using (new OperationContextScope(client.InnerChannel))
                {
                    MessageHeader usrMsgHdr = MessageHeader.CreateHeader("UsernameToken", "http://siebel.com/webservices", NoteServiceUsername);
                    OperationContext.Current.OutgoingMessageHeaders.Add(usrMsgHdr);
                    MessageHeader pwdMsgHdr = MessageHeader.CreateHeader("PasswordText", "http://siebel.com/webservices", NoteServicePassword);
                    OperationContext.Current.OutgoingMessageHeaders.Add(pwdMsgHdr);
                    stopwatch.Start();
                    op = client.WC_Service_Request_BSInsert(ip);
                    stopwatch.Stop();
                    request = eBehavior.msgInspector.reqPayload;
                    response = eBehavior.msgInspector.resPayload;

                    SRSVC.ServiceRequestId opData = op.ListOfWc_Service_Request_Io[0];
                    note.NoteID = opData.ListOfAction[0].Id;
                }
                logMessage = "Request of creating Note (Success). Created Note ID = " + note.NoteID;
                logNote = "Request Payload: " + request;
                log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);

                logMessage = "Response of creating Note(Success). Created Note ID = " + note.NoteID;
                logNote = "Response Payload: " + response;
                log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote, (int)stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                request = eBehavior.msgInspector.reqPayload;
                response = eBehavior.msgInspector.resPayload;

                note.ErrorMessage = "There has been an error communicating with Siebel. Please check log for detail.";

                logMessage = "Request of creating Note (Failure). " + ex.Message;
                logNote = "Request Payload: " + request;
                log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                logMessage = "Response of creating Note(Failure). " + ex.Message;
                logNote = "Response Payload: " + response;
                log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                handleSiebelException(ex, "Create Note of Service Request", _logIncidentId, _logContactId);
            }
            return note;
        }

        public Note CreateNoteAttachment(Note note, int _logIncidentId = 0, int _logContactId = 0)
        {
            if (String.IsNullOrWhiteSpace(NoteURL) || String.IsNullOrWhiteSpace(NoteServiceUsername) || String.IsNullOrWhiteSpace(NoteServicePassword))
            {
                throw new Exception("Provider's InitForSR not run.");
            }

            string request, response, logMessage, logNote;

            ActivityRef.ActivityWSPortClient client = new ActivityRef.ActivityWSPortClient(binding, addr);
            MyEndpointBehavior eBehavior = new MyEndpointBehavior();
            client.Endpoint.Behaviors.Add(eBehavior);
            if(NoteServiceTimeout > 0)
                client.InnerChannel.OperationTimeout = TimeSpan.FromMilliseconds(NoteServiceTimeout );

            ActivityRef.ActivityUpdateMSO_Input ip = new ActivityRef.ActivityUpdateMSO_Input();


            ip.ListOfActivity = new ActivityRef.Activity[1];
            ip.ListOfActivity[0] = new ActivityRef.Activity();

            if (note.NoteID != null)
            {
                ip.ListOfActivity[0].Id = note.NoteID;
            }
            else
            {
                note.ErrorMessage = "There is no Activity ID.";
                return note;
            }

            if (note.SrID != null)
            {
                ip.ListOfActivity[0].ServiceRequestId = note.SrID;
            }
            else
            {
                note.ErrorMessage = "There is no Service Request ID.";
                return note;
            }

            if (note.Content != null)
            {
                ip.ListOfActivity[0].ListOfRelatedAttachment = new ActivityRef.RelatedAttachment[1];
                ip.ListOfActivity[0].ListOfRelatedAttachment[0] = new ActivityRef.RelatedAttachment();
                ip.ListOfActivity[0].ListOfRelatedAttachment[0].FileName = "Chat Transcript From RNow - " + note.Created.Ticks.ToString();
                ip.ListOfActivity[0].ListOfRelatedAttachment[0].FileExt = "txt";
                ip.ListOfActivity[0].ListOfRelatedAttachment[0].ActivityId = note.NoteID;
                var plainContent = System.Text.Encoding.UTF8.GetBytes(note.Content);
                ip.ListOfActivity[0].ListOfRelatedAttachment[0].Attachment = plainContent;
            }
            else
            {
                note.ErrorMessage = "There is no content to save.";
                return note;
            }
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                ActivityRef.ActivityUpdateMSO_Output op;
                using (new OperationContextScope(client.InnerChannel))
                {
                    MessageHeader usrMsgHdr = MessageHeader.CreateHeader("UsernameToken", "http://siebel.com/webservices", NoteServiceUsername);
                    OperationContext.Current.OutgoingMessageHeaders.Add(usrMsgHdr);
                    MessageHeader pwdMsgHdr = MessageHeader.CreateHeader("PasswordText", "http://siebel.com/webservices", NoteServicePassword);
                    OperationContext.Current.OutgoingMessageHeaders.Add(pwdMsgHdr);
                    stopwatch.Start();
                    op = client.ActivityUpdateMSO(ip);
                    stopwatch.Stop();
                    request = eBehavior.msgInspector.reqPayload;
                    response = eBehavior.msgInspector.resPayload;

                    ActivityRef.Activity[] opData = op.ListOfActivity;
                    note.NoteID = opData[0].Id;
                }

                logMessage = "Request of adding attachment to Note (Success). Updated Note ID = " + note.NoteID;
                logNote = "Request Payload: " + request;
                log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);

                logMessage = "Response of adding attachment to Note (Success). Updated Note ID = " + note.NoteID;
                logNote = "Response Payload: " + response;
                log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote, (int)stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                request = eBehavior.msgInspector.reqPayload;
                response = eBehavior.msgInspector.resPayload;

                note.ErrorMessage = "There has been an error communicating with Siebel. Please check log for detail.";

                logMessage = "Request of dding attachment to Note (Failure). " + ex.Message;
                logNote = "Request Payload: " + request;
                log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                logMessage = "Response of dding attachment to Note (Failure). " + ex.Message;
                logNote = "Response Payload: " + response;
                log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                handleSiebelException(ex, "Add attchment to Note", _logIncidentId, _logContactId);
            }
            return note;
        }

        #endregion

        #region get schema Methods
        /* Called by ContactDetailVirtualTable() constructor 
         * when report addIn is loaded when CX launch
         */
        public Dictionary<string, string> getContactDetailSchema()
        {
            CONTACTSVC.ContactData contactDetailObject = new CONTACTSVC.ContactData();

            Dictionary<string, string> dictDetail = new Dictionary<string, string>();
            foreach (PropertyInfo propertyInfo in contactDetailObject.GetType().GetProperties())
            {
                // exclude the siebel internal columns
                if (!propertyInfo.Name.StartsWith("DeDup") &&
                    !propertyInfo.Name.StartsWith("Dedup") &&
                    !propertyInfo.Name.Equals("DockFlag"))
                    dictAddProperty(propertyInfo, null, ref dictDetail);
            }
            return dictDetail;
        }

        public Dictionary<string, string> getServiceRequestDetailSchema()
        {
            ServiceRequestRef.ServiceRequestData srDetailObject = new ServiceRequestRef.ServiceRequestData();

            Dictionary<string, string> dictDetail = new Dictionary<string, string>();
            foreach (PropertyInfo propertyInfo in srDetailObject.GetType().GetProperties())
            {
                // exclude the siebel internal columns
                if (!propertyInfo.Name.StartsWith("DeDup") &&
                    !propertyInfo.Name.StartsWith("Dedup") &&
                    !propertyInfo.Name.Equals("DockFlag"))
                    dictAddProperty(propertyInfo, null, ref dictDetail);
            }
            return dictDetail;
        }

        public Dictionary<string, string> getAssetSchema()
        {
            AssetMgmtRef.AssetMgmtAssetData assetObject = new AssetMgmtRef.AssetMgmtAssetData();

            Dictionary<string, string> dictDetail = new Dictionary<string, string>();
            foreach (PropertyInfo propertyInfo in assetObject.GetType().GetProperties())
            {
                // exclude the siebel internal columns
                if (!propertyInfo.Name.StartsWith("DeDup") &&
                    !propertyInfo.Name.StartsWith("Dedup") &&
                    !propertyInfo.Name.Equals("DockFlag"))
                    dictAddProperty(propertyInfo, null, ref dictDetail);
            }
            return dictDetail;
        }

        public List<Dictionary<string, string>> LookupAssetList(IList<string> columns, string contactId, int _logIncidentId = 0, int _logContactId = 0)
        {
            // can reuse SRURL, service username and password, is all the same 
            if (String.IsNullOrWhiteSpace(SRURL) || String.IsNullOrWhiteSpace(SRServiceUsername) || String.IsNullOrWhiteSpace(SRServicePassword))
            {
                throw new Exception("Provider's InitForAsset not run.");
            }
            string request, response, logMessage, logNote;
            binding.MaxReceivedMessageSize = 2147483647;
            ASSETSVC.AssetManagementPortClient client = new ASSETSVC.AssetManagementPortClient(binding, addr);
            MyEndpointBehavior eBehavior = new MyEndpointBehavior();
            client.Endpoint.Behaviors.Add(eBehavior);

            if (SRServiceTimeout > 0)
                client.InnerChannel.OperationTimeout = TimeSpan.FromMilliseconds(SRServiceTimeout);

            ASSETSVC.AssetManagementQueryPage_Input ip = new ASSETSVC.AssetManagementQueryPage_Input();

            ip.ListOfAsset_Management_Io = new ASSETSVC.ListOfAsset_Management_IoQuery();
            ip.ListOfAsset_Management_Io.AssetMgmtAsset = new ASSETSVC.AssetMgmtAssetQuery();

            foreach (PropertyInfo propertyInfo in ip.ListOfAsset_Management_Io.AssetMgmtAsset.GetType().GetProperties())
            {
                foreach (string column in columns)
                {
                    if (propertyInfo.Name == column.Split('.')[1])
                    {
                        if (propertyInfo.PropertyType == typeof(ASSETSVC.queryType))
                        {
                            ASSETSVC.queryType queryType = new ASSETSVC.queryType();
                            propertyInfo.SetValue(ip.ListOfAsset_Management_Io.AssetMgmtAsset, queryType, null);
                        }
                        break;
                    }
                }
            }

            ip.ListOfAsset_Management_Io.AssetMgmtAsset.PrimaryContactId = new ASSETSVC.queryType();
            ip.ListOfAsset_Management_Io.AssetMgmtAsset.PrimaryContactId.Value = "='" + contactId + "'";

            ip.LOVLanguageMode = "LIC";
            ip.ViewMode = "All";

            ASSETSVC.AssetManagementQueryPage_Output opList;
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                using (new OperationContextScope(client.InnerChannel))
                {
                    MessageHeader usrMsgHdr = MessageHeader.CreateHeader("UsernameToken", "http://siebel.com/webservices", SRServiceUsername);
                    OperationContext.Current.OutgoingMessageHeaders.Add(usrMsgHdr);
                    MessageHeader pwdMsgHdr = MessageHeader.CreateHeader("PasswordText", "http://siebel.com/webservices", SRServicePassword);
                    OperationContext.Current.OutgoingMessageHeaders.Add(pwdMsgHdr);
                    stopwatch.Start();
                    opList = client.AssetManagementQueryPage(ip);
                    stopwatch.Stop();
                    request = eBehavior.msgInspector.reqPayload;
                    logMessage = "Request of asset list (Success). Siebel Contact ID = " + contactId;
                    logNote = "Request Payload: " + request;
                    log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);
                    response = eBehavior.msgInspector.resPayload;
                    logMessage = "Response of asset list (Success). Siebel Contact ID = " + contactId;
                    logNote = "Response Payload: " + response;
                    log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote, (int)stopwatch.ElapsedMilliseconds);
                }
            }
            catch (Exception ex)
            {
                request = eBehavior.msgInspector.reqPayload;
                logMessage = "Request of asset list (Failure). Siebel Contact ID = " + contactId;
                logNote = "Request Payload: " + request;
                log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);
                response = eBehavior.msgInspector.resPayload;
                logMessage = "Response of asset list (Failure). Siebel Contact ID = " + contactId;
                logNote = "Response Payload: " + response;
                log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);
                handleSiebelException(ex, "client.AssetManagementQueryPage(ip)");
                throw ex;
            }

            if (opList.ListOfAsset_Management_Io.AssetMgmtAsset == null)
            {
                return null;
            }

            List<Dictionary<string, string>> dictDetailList = new List<Dictionary<string, string>>();

            foreach (ASSETSVC.AssetMgmtAssetData assetData in opList.ListOfAsset_Management_Io.AssetMgmtAsset)
            {
                Dictionary<string, string> dictDetail = new Dictionary<string, string>();

                foreach (PropertyInfo propertyInfo in assetData.GetType().GetProperties())
                {
                    Object propVal = assetData.GetType().GetProperty(propertyInfo.Name).GetValue(assetData, null);
                    dictAddProperty(propertyInfo, propVal, ref dictDetail);
                }

                dictDetailList.Add(dictDetail);
            }

            return dictDetailList;
        }
        public Dictionary<string, string> LookupAssetDetail(IList<string> columns, string serialNum, string orgId, int _logIncidentId = 0, int _logContactId = 0)
        {
            // can reuse SRURL, service username and password, is all the same 
            if (String.IsNullOrWhiteSpace(SRURL) || String.IsNullOrWhiteSpace(SRServiceUsername) || String.IsNullOrWhiteSpace(SRServicePassword))
            {
                throw new Exception("Provider's InitForAsset not run.");
            }
            string request, response, logMessage, logNote;

            ASSETSVC.AssetManagementPortClient client = new ASSETSVC.AssetManagementPortClient(binding, addr);
            MyEndpointBehavior eBehavior = new MyEndpointBehavior();
            client.Endpoint.Behaviors.Add(eBehavior);
            if (SRServiceTimeout > 0)
                client.InnerChannel.OperationTimeout = TimeSpan.FromMilliseconds(SRServiceTimeout);

            ASSETSVC.AssetManagementQueryPage_Input ip = new ASSETSVC.AssetManagementQueryPage_Input();

            ip.ListOfAsset_Management_Io = new ASSETSVC.ListOfAsset_Management_IoQuery();
            ip.ListOfAsset_Management_Io.AssetMgmtAsset = new ASSETSVC.AssetMgmtAssetQuery();

            foreach (PropertyInfo propertyInfo in ip.ListOfAsset_Management_Io.AssetMgmtAsset.GetType().GetProperties())
            {
                foreach (string column in columns)
                {
                    if (propertyInfo.Name == column.Split('.')[1])
                    {
                        if (propertyInfo.PropertyType == typeof(ASSETSVC.queryType))
                        {
                            ASSETSVC.queryType queryType = new ASSETSVC.queryType();
                            propertyInfo.SetValue(ip.ListOfAsset_Management_Io.AssetMgmtAsset, queryType, null);
                        }
                        break;
                    }
                }
            }

            ip.ListOfAsset_Management_Io.AssetMgmtAsset.SerialNumber = new ASSETSVC.queryType();
            ip.ListOfAsset_Management_Io.AssetMgmtAsset.SerialNumber.Value = "='" + serialNum + "'";

            ip.ListOfAsset_Management_Io.AssetMgmtAsset.OrganizationId = new ASSETSVC.queryType();
            ip.ListOfAsset_Management_Io.AssetMgmtAsset.OrganizationId.Value = "='" + orgId + "'";

            ip.LOVLanguageMode = "LIC";
            ip.ViewMode = "All";

            ASSETSVC.AssetManagementQueryPage_Output opList;
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                using (new OperationContextScope(client.InnerChannel))
                {
                    MessageHeader usrMsgHdr = MessageHeader.CreateHeader("UsernameToken", "http://siebel.com/webservices", SRServiceUsername);
                    OperationContext.Current.OutgoingMessageHeaders.Add(usrMsgHdr);
                    MessageHeader pwdMsgHdr = MessageHeader.CreateHeader("PasswordText", "http://siebel.com/webservices", SRServicePassword);
                    OperationContext.Current.OutgoingMessageHeaders.Add(pwdMsgHdr);
                    stopwatch.Start();
                    opList = client.AssetManagementQueryPage(ip);
                    stopwatch.Stop();
                    request = eBehavior.msgInspector.reqPayload;
                    logMessage = "Request of asset (Success). Siebel serialNum = " + serialNum + ", orgId = " + orgId;
                    logNote = "Request Payload: " + request;
                    log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);
                    response = eBehavior.msgInspector.resPayload;
                    logMessage = "Response of asset (Success). Siebel serialNum = " + serialNum + ", orgId = " + orgId;
                    logNote = "Response Payload: " + response;
                    log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote, (int)stopwatch.ElapsedMilliseconds);
                }
            }
            catch (Exception ex)
            {
                request = eBehavior.msgInspector.reqPayload;
                logMessage = "Request of asset (Failure). Siebel serialNum = " + serialNum + ", orgId = " + orgId;
                logNote = "Request Payload: " + request;
                log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
                response = eBehavior.msgInspector.resPayload;
                logMessage = "Response of asset (Failure). Siebel serialNum = " + serialNum + ", orgId = " + orgId;
                logNote = "Response Payload: " + response;
                log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
                handleSiebelException(ex, "client.AssetManagementQueryPage(ip)");
                throw ex;
            }

            if (opList.ListOfAsset_Management_Io.AssetMgmtAsset == null)
            {
                return null;
            }

            Dictionary<string, string> dictDetail = new Dictionary<string, string>();

            ASSETSVC.AssetMgmtAssetData assetData = opList.ListOfAsset_Management_Io.AssetMgmtAsset[0];
            foreach (PropertyInfo propertyInfo in assetData.GetType().GetProperties())
            {
                Object propVal = assetData.GetType().GetProperty(propertyInfo.Name).GetValue(assetData, null);
                dictAddProperty(propertyInfo, propVal, ref dictDetail);
            }

            return dictDetail;
        }


        public Asset SerialNumberValidation(string serialNum, string orgId, int _logIncidentId = 0, int _logContactId = 0)
        {
            // can reuse SRURL, service username and password, is all the same 
            if (String.IsNullOrWhiteSpace(SRURL) || String.IsNullOrWhiteSpace(SRServiceUsername) || String.IsNullOrWhiteSpace(SRServicePassword))
            {
                throw new Exception("Provider's InitForAsset not run.");
            }

            string request, response, logMessage, logNote;

            ASSETSVC.AssetManagementPortClient client = new ASSETSVC.AssetManagementPortClient(binding, addr);
            MyEndpointBehavior eBehavior = new MyEndpointBehavior();
            client.Endpoint.Behaviors.Add(eBehavior);
            if (SRServiceTimeout > 0)
                client.InnerChannel.OperationTimeout = TimeSpan.FromMilliseconds(SRServiceTimeout);

            Asset retval = null;

            ASSETSVC.AssetManagementQueryPage_Input ip = new ASSETSVC.AssetManagementQueryPage_Input();

            ip.ListOfAsset_Management_Io = new ASSETSVC.ListOfAsset_Management_IoQuery();
            ip.ListOfAsset_Management_Io.AssetMgmtAsset = new ASSETSVC.AssetMgmtAssetQuery();


            ip.ListOfAsset_Management_Io.AssetMgmtAsset.SerialNumber = new ASSETSVC.queryType();
            ip.ListOfAsset_Management_Io.AssetMgmtAsset.SerialNumber.Value = "='" + serialNum + "'";

            ip.ListOfAsset_Management_Io.AssetMgmtAsset.OrganizationId = new ASSETSVC.queryType();
            ip.ListOfAsset_Management_Io.AssetMgmtAsset.OrganizationId.Value = "='" + orgId + "'";

            ip.ListOfAsset_Management_Io.AssetMgmtAsset.Id= new ASSETSVC.queryType();
            ip.ListOfAsset_Management_Io.AssetMgmtAsset.Id.Value = String.Empty;

            ip.ListOfAsset_Management_Io.AssetMgmtAsset.ProductId = new ASSETSVC.queryType();
            ip.ListOfAsset_Management_Io.AssetMgmtAsset.ProductId.Value = String.Empty;

            ip.ListOfAsset_Management_Io.AssetMgmtAsset.ProductName = new ASSETSVC.queryType();
            ip.ListOfAsset_Management_Io.AssetMgmtAsset.ProductName.Value = String.Empty;

            ip.LOVLanguageMode = "LIC";
            ip.ViewMode = "All";
            Stopwatch stopwatch = new Stopwatch();
            ASSETSVC.AssetManagementQueryPage_Output opList;
            try
            {
                using (new OperationContextScope(client.InnerChannel))
                {
                    MessageHeader usrMsgHdr = MessageHeader.CreateHeader("UsernameToken", "http://siebel.com/webservices", SRServiceUsername);
                    OperationContext.Current.OutgoingMessageHeaders.Add(usrMsgHdr);
                    MessageHeader pwdMsgHdr = MessageHeader.CreateHeader("PasswordText", "http://siebel.com/webservices", SRServicePassword);
                    OperationContext.Current.OutgoingMessageHeaders.Add(pwdMsgHdr);
                    stopwatch.Start();
                    opList = client.AssetManagementQueryPage(ip);
                    stopwatch.Stop();
                    request = eBehavior.msgInspector.reqPayload;
                    response = eBehavior.msgInspector.resPayload;
                }

                ASSETSVC.AssetMgmtAssetData[] assets = opList.ListOfAsset_Management_Io.AssetMgmtAsset;
                if (assets == null)
                {
                    retval = null;

                    logMessage = "Request of serial number validation (Failure). Serial Number = " + serialNum + "; Account Org ID = " + orgId;
                    logNote = "Request Payload: " + request;
                    log.NoticeLog(_logIncidentId, _logContactId, logMessage, logNote);

                    logMessage = "Response of serial number validation (Failure). Serial Number = " + serialNum + "; Account Org ID = " + orgId;
                    logNote = "Response Payload: " + response;
                    log.NoticeLog(_logIncidentId, _logContactId, logMessage, logNote, (int)stopwatch.ElapsedMilliseconds);
                }
                else
                {
                    ASSETSVC.AssetMgmtAssetData asset = assets[0];
                    retval = new Asset();
                    retval.AssetID = asset.Id;
                    retval.SerialNumber = asset.SerialNumber;
                    retval.AccountOrgID = asset.AccountOrgId;
                    retval.ProductID = asset.ProductId;
                    retval.ProductName = asset.ProductName;

                    logMessage = "Request of serial number validation (Success). Serial Number = " + serialNum + "; Account Org ID = " + orgId;
                    logNote = "Request Payload: " + request;
                    log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);

                    logMessage = "Response of serial number validation (Success). Serial Number = " + serialNum + "; Account Org ID = " + orgId;
                    logNote = "Response Payload: " + response;
                    log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote, (int)stopwatch.ElapsedMilliseconds);
                }
            }
            catch (Exception ex)
            {

                request = eBehavior.msgInspector.reqPayload;
                response = eBehavior.msgInspector.resPayload;

                retval = new Asset();
                retval.ErrorMessage = "There has been an error communicating with Siebel. Please check log for detail.";
                logMessage = "Request of serial number validation (Failure). Serial Number = " + serialNum + "; Account Org ID = " + orgId + "; Error: " + ex.Message;
                logNote = "Request Payload: " + request;
                log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                logMessage = "Response of serial number validation (Failure). Serial Number = " + serialNum + "; Account Org ID = " + orgId + "; Error: " + ex.Message;
                logNote = "Response Payload: " + response;
                log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                handleSiebelException(ex, "Lookup Service Request", _logIncidentId, _logContactId);
            }
            

            return retval;
        }

        public Dictionary<string, string> getActivitySchema()
        {
            ServiceRequestRef.ActionData ActivityObject = new ServiceRequestRef.ActionData();

            Dictionary<string, string> dictDetail = new Dictionary<string, string>();
            foreach (PropertyInfo propertyInfo in ActivityObject.GetType().GetProperties())
            {
                // exclude the siebel internal columns
                if (!propertyInfo.Name.StartsWith("DeDup") &&
                    !propertyInfo.Name.StartsWith("Dedup") &&
                    !propertyInfo.Name.Equals("DockFlag"))
                    dictAddProperty(propertyInfo, null, ref dictDetail);
            }
            return dictDetail;
        }

        public List<Dictionary<string, string>> LookupActivityList(IList<string> columns, string srId, int _logIncidentId = 0, int _logContactId = 0)
        {
            // can reuse SRURL, service username and password, is all the same 
            if (String.IsNullOrWhiteSpace(SRURL) || String.IsNullOrWhiteSpace(SRServiceUsername) || String.IsNullOrWhiteSpace(SRServicePassword))
            {
                throw new Exception("Provider's InitForActivity not run.");
            }
            string request, response, logMessage, logNote;

            binding.MaxReceivedMessageSize = 2147483647;
            SRSVC.WC_Service_Request_BSClient client = new SRSVC.WC_Service_Request_BSClient(binding, addr);
            MyEndpointBehavior eBehavior = new MyEndpointBehavior();
            client.Endpoint.Behaviors.Add(eBehavior);

            if (SRServiceTimeout > 0)
                client.InnerChannel.OperationTimeout = TimeSpan.FromMilliseconds(SRServiceTimeout);

            SRSVC.WC_Service_Request_BSQueryPage_Input ip = new SRSVC.WC_Service_Request_BSQueryPage_Input();
            ip.ListOfWc_Service_Request_Io = new SRSVC.ListOfWc_Service_Request_IoQuery();
            ip.ListOfWc_Service_Request_Io.ServiceRequest = new SRSVC.ServiceRequestQuery();
            ip.ListOfWc_Service_Request_Io.ServiceRequest.ListOfAction = new SRSVC.ListOfActionQuery();
            ip.ListOfWc_Service_Request_Io.ServiceRequest.ListOfAction.pagesize = "100";
            ip.ListOfWc_Service_Request_Io.ServiceRequest.ListOfAction.Action = new SRSVC.ActionQuery();
            ip.ListOfWc_Service_Request_Io.ServiceRequest.Id = new SRSVC.queryType();

            foreach (PropertyInfo propertyInfo in ip.ListOfWc_Service_Request_Io.ServiceRequest.ListOfAction.Action.GetType().GetProperties())
            {
                foreach (string column in columns)
                {
                    if (propertyInfo.Name == column.Split('.')[1])
                    {
                        if (propertyInfo.PropertyType == typeof(SRSVC.queryType))
                        {
                            SRSVC.queryType queryType = new SRSVC.queryType();
                            propertyInfo.SetValue(ip.ListOfWc_Service_Request_Io.ServiceRequest.ListOfAction.Action, queryType, null);
                        }
                        break;
                    }
                }
            }

            ip.ListOfWc_Service_Request_Io.ServiceRequest.Id.Value = "='" + srId + "'";
            ip.LOVLanguageMode = "LIC";
            ip.ViewMode = "All";

            SRSVC.WC_Service_Request_BSQueryPage_Output opList;
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                using (new OperationContextScope(client.InnerChannel))
                {
                    MessageHeader usrMsgHdr = MessageHeader.CreateHeader("UsernameToken", "http://siebel.com/webservices", SRServiceUsername);
                    OperationContext.Current.OutgoingMessageHeaders.Add(usrMsgHdr);
                    MessageHeader pwdMsgHdr = MessageHeader.CreateHeader("PasswordText", "http://siebel.com/webservices", SRServicePassword);
                    OperationContext.Current.OutgoingMessageHeaders.Add(pwdMsgHdr);
                    stopwatch.Start();
                    opList = client.WC_Service_Request_BSQueryPage(ip);
                    stopwatch.Stop();
                    request = eBehavior.msgInspector.reqPayload;
                    logMessage = "Request of Activity list (Success). Siebel SR ID = " + srId;
                    logNote = "Request Payload: " + request;
                    log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);
                    response = eBehavior.msgInspector.resPayload;
                    logMessage = "Response of Activity list (Success). Siebel SR ID = " + srId;
                    logNote = "Response Payload: " + response;
                    log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote, (int)stopwatch.ElapsedMilliseconds);
                }
            }
            catch (Exception ex)
            {
                request = eBehavior.msgInspector.reqPayload;
                logMessage = "Request of Activity list (Failure). Siebel SR ID = " + srId;
                logNote = "Request Payload: " + request;
                log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
                response = eBehavior.msgInspector.resPayload;
                logMessage = "Response of Activity list (Failure). Siebel SR ID = " + srId;
                logNote = "Response Payload: " + response;
                log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
                handleSiebelException(ex, "client.WC_Service_Request_BSQueryPage(ip)");
                throw ex;
            }

            if (opList.ListOfWc_Service_Request_Io.ServiceRequest == null || 
                opList.ListOfWc_Service_Request_Io.ServiceRequest[0].ListOfAction.Action == null)
            {
                return null;
            }

            List<Dictionary<string, string>> dictDetailList = new List<Dictionary<string, string>>();
           
            foreach (SRSVC.ActionData ActivityData in opList.ListOfWc_Service_Request_Io.ServiceRequest[0].ListOfAction.Action)
            {
                Dictionary<string, string> dictDetail = new Dictionary<string, string>();

                foreach (PropertyInfo propertyInfo in ActivityData.GetType().GetProperties())
                {
                    Object propVal = ActivityData.GetType().GetProperty(propertyInfo.Name).GetValue(ActivityData, null);
                    dictAddProperty(propertyInfo, propVal, ref dictDetail);
                }

                dictDetailList.Add(dictDetail);
            }

            return dictDetailList;
        }

        public Dictionary<string, string> LookupActivityDetail(IList<string> columns, string srId, string actvtyId, int _logIncidentId = 0, int _logContactId = 0)
        {
            // can reuse SRURL, service username and password, is all the same 
            if (String.IsNullOrWhiteSpace(SRURL) || String.IsNullOrWhiteSpace(SRServiceUsername) || String.IsNullOrWhiteSpace(SRServicePassword))
            {
                throw new Exception("Provider's InitForActivity not run.");
            }
            string request, response, logMessage, logNote;

            SRSVC.WC_Service_Request_BSClient client = new SRSVC.WC_Service_Request_BSClient(binding, addr);
            MyEndpointBehavior eBehavior = new MyEndpointBehavior();
            client.Endpoint.Behaviors.Add(eBehavior);

            if (SRServiceTimeout > 0)
                client.InnerChannel.OperationTimeout = TimeSpan.FromMilliseconds(SRServiceTimeout);

            SRSVC.WC_Service_Request_BSQueryPage_Input ip = new SRSVC.WC_Service_Request_BSQueryPage_Input();
            ip.ListOfWc_Service_Request_Io = new SRSVC.ListOfWc_Service_Request_IoQuery();
            ip.ListOfWc_Service_Request_Io.ServiceRequest = new SRSVC.ServiceRequestQuery();
            ip.ListOfWc_Service_Request_Io.ServiceRequest.ListOfAction = new SRSVC.ListOfActionQuery();
            ip.ListOfWc_Service_Request_Io.ServiceRequest.ListOfAction.Action = new SRSVC.ActionQuery();
            ip.ListOfWc_Service_Request_Io.ServiceRequest.Id = new SRSVC.queryType();
            ip.ListOfWc_Service_Request_Io.ServiceRequest.ListOfAction.Action.Id = new SRSVC.queryType();
            foreach (PropertyInfo propertyInfo in ip.ListOfWc_Service_Request_Io.ServiceRequest.ListOfAction.Action.GetType().GetProperties())
            {
                foreach (string column in columns)
                {
                    if (propertyInfo.Name == column.Split('.')[1])
                    {
                        if (propertyInfo.PropertyType == typeof(SRSVC.queryType))
                        {
                            SRSVC.queryType queryType = new SRSVC.queryType();
                            propertyInfo.SetValue(ip.ListOfWc_Service_Request_Io.ServiceRequest.ListOfAction.Action, queryType, null);
                        }
                        break;
                    }
                }
            }

            ip.ListOfWc_Service_Request_Io.ServiceRequest.Id.Value = "='" + srId + "'";
            ip.ListOfWc_Service_Request_Io.ServiceRequest.ListOfAction.Action.Id.Value = "='" + actvtyId + "'";

            ip.LOVLanguageMode = "LIC";
            ip.ViewMode = "All";

            SRSVC.WC_Service_Request_BSQueryPage_Output opList;
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                using (new OperationContextScope(client.InnerChannel))
                {
                    MessageHeader usrMsgHdr = MessageHeader.CreateHeader("UsernameToken", "http://siebel.com/webservices", SRServiceUsername);
                    OperationContext.Current.OutgoingMessageHeaders.Add(usrMsgHdr);
                    MessageHeader pwdMsgHdr = MessageHeader.CreateHeader("PasswordText", "http://siebel.com/webservices", SRServicePassword);
                    OperationContext.Current.OutgoingMessageHeaders.Add(pwdMsgHdr);
                    stopwatch.Start();
                    opList = client.WC_Service_Request_BSQueryPage(ip);
                    stopwatch.Stop();
                    request = eBehavior.msgInspector.reqPayload;
                    logMessage = "Request of Activity list (Success). Siebel SR ID = " + srId + ", ActivityId = " + actvtyId;
                    logNote = "Request Payload: " + request;
                    log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);
                    response = eBehavior.msgInspector.resPayload;
                    logMessage = "Response of Activity list (Success). Siebel SR ID = " + srId + ", ActivityId = " + actvtyId;
                    logNote = "Response Payload: " + response;
                    log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote, (int)stopwatch.ElapsedMilliseconds);
                }
            }
            catch (Exception ex)
            {
                request = eBehavior.msgInspector.reqPayload;
                logMessage = "Request of Activity list (Failure). Siebel SR ID = " + srId + ", ActivityId = " + actvtyId;
                logNote = "Request Payload: " + request;
                log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);
                response = eBehavior.msgInspector.resPayload;
                logMessage = "Response of Activity list (Failure). Siebel SR ID = " + srId + ", ActivityId = " + actvtyId;
                logNote = "Response Payload: " + response;
                log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);
                handleSiebelException(ex, "client.WC_Service_Request_BSQueryPage(ip)");
                throw ex;
            }

            if (opList.ListOfWc_Service_Request_Io.ServiceRequest == null)
            {
                return null;
            }

            List<Dictionary<string, string>> dictDetailList = new List<Dictionary<string, string>>();
            Dictionary<string, string> dictDetail = new Dictionary<string, string>();

            foreach (SRSVC.ActionData ActivityData in opList.ListOfWc_Service_Request_Io.ServiceRequest[0].ListOfAction.Action)
            {
                foreach (PropertyInfo propertyInfo in ActivityData.GetType().GetProperties())
                {
                    Object propVal = ActivityData.GetType().GetProperty(propertyInfo.Name).GetValue(ActivityData, null);
                    dictAddProperty(propertyInfo, propVal, ref dictDetail);
                }             
            }

            return dictDetail;
        }
        #endregion

        #region Helper functions

        //Status Convert
        public KeyValuePair<String, String> rnStatusToServerStatus(int rnStatusID)
        {
            KeyValuePair<String, String> rnStatusToMockStatus;
            switch (rnStatusID)
            {
                case 2:
                    rnStatusToMockStatus = new KeyValuePair<String, String>("Close", "Close");
                    break;
                default:
                    rnStatusToMockStatus = new KeyValuePair<String, String>("Open", "Open");
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
                    rnSeverityToMockSeverity = new KeyValuePair<String, String>("2-High", "High");
                    break;
                case 2:
                    rnSeverityToMockSeverity = new KeyValuePair<String, String>("3-Medium", "Medium");
                    break;
                case 3:
                    rnSeverityToMockSeverity = new KeyValuePair<String, String>("4-Low", "Low");
                    break;
                default:
                    rnSeverityToMockSeverity = new KeyValuePair<String, String>("3-Medium", "Medium");
                    break;
            }
            return rnSeverityToMockSeverity;

        }

        public KeyValuePair<String, String> rnRequestTypeToServerRequestType(int rnRequestTypeID)
        {

            KeyValuePair<String, String> rnRequestTypeToMockRequestType;
            switch (rnRequestTypeID)
            {
                default:
                    rnRequestTypeToMockRequestType = new KeyValuePair<String, String>("Incident", "Incident");
                    break;
            }
            return rnRequestTypeToMockRequestType;

        }


        /*  see QA 140825-000196 Acceptance Criteria
         *  If timed out, show such message, else generic one
         *  The error details are in the log (eg: pslog)
         */
        public void handleSiebelException(Exception ex, string wsMethod, int _logIncidentId = 0, int _logContactId = 0)
        {
            string logMessage = wsMethod + " web service error.";
            string logNote = ex.Message;
            log.ErrorLog(logMessage: logMessage, logNote: logNote);

            string errorMsg = "There has been an error communicating with Siebel. Please check log for detail.";
            if (ex.Message.StartsWith("The request channel timed out"))
            {
                errorMsg = "The connection to Siebel has timed out.";
            }

            Exception eNew = new Exception(errorMsg);
            //MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            throw eNew;  
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
                case "Decimal":
                case "Int32":
                    value = propVal != null ? propVal.ToString() : null;
                    typeValue = "Integer" + TYPE_VALUE_DELIMITER + value;
                    dictDetail.Add(propName, typeValue);
                    break;
                case "DateTime":
                    value = propVal != null ? propVal.ToString() : null;
                    typeValue = "DateTime" + TYPE_VALUE_DELIMITER + value;
                    dictDetail.Add(propName, typeValue);
                    break;
                case "Boolean": // eg: xxxSpecified is for Siebel internal use, and should not show in the report schema
                    if (!propName.EndsWith("Specified"))
                    {
                        value = propVal != null ? propVal.ToString() : null;
                        typeValue = "Boolean" + TYPE_VALUE_DELIMITER + value;
                        dictDetail.Add(propName, typeValue);
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion



        // Activity

        public Activity CreateActivity(Activity activity, int _logIncidentId = 0, int _logContactId = 0)
        {
            // reuse SR web service
            if (String.IsNullOrWhiteSpace(SRURL) || String.IsNullOrWhiteSpace(SRServiceUsername)
                || String.IsNullOrWhiteSpace(SRServicePassword))
            {
                throw new Exception("Provider's InitForSR not run.");
            }

            string request, response, logMessage, logNote;

            SRSVC.WC_Service_Request_BSClient client = new SRSVC.WC_Service_Request_BSClient(binding, addr);
            MyEndpointBehavior eBehavior = new MyEndpointBehavior();
            client.Endpoint.Behaviors.Add(eBehavior);
            if (ActivityServiceTimeout > 0)
                client.InnerChannel.OperationTimeout = TimeSpan.FromMilliseconds(ActivityServiceTimeout);

            SRSVC.WC_Service_Request_BSInsert_Input ip = new SRSVC.WC_Service_Request_BSInsert_Input();


            ip.ListOfWc_Service_Request_Io = new SRSVC.ListOfWc_Service_Request_IoData();
            ip.ListOfWc_Service_Request_Io.ServiceRequest = new SRSVC.ServiceRequestData[1];
            ip.ListOfWc_Service_Request_Io.ServiceRequest[0] = new SRSVC.ServiceRequestData();
            if (activity.SrID != null)
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].Id = activity.SrID;
            }
            else
            {
                activity.ErrorMessage = "The following error occurred when doing the Create: SR ID is empty";
                return activity;
            }

            ip.ListOfWc_Service_Request_Io.ServiceRequest[0].ListOfAction = new SRSVC.ListOfActionData();
            ip.ListOfWc_Service_Request_Io.ServiceRequest[0].ListOfAction.Action = new SRSVC.ActionData[1];
            ip.ListOfWc_Service_Request_Io.ServiceRequest[0].ListOfAction.Action[0] = new SRSVC.ActionData();
            if (activity.Comment != null)
            {
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].ListOfAction.Action[0].Description2 = activity.Description;
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].ListOfAction.Action[0].Type = activity.ActivityType;
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].ListOfAction.Action[0].Comment = activity.Comment;
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].ListOfAction.Action[0].Description2 = activity.Description;
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].ListOfAction.Action[0].Due = activity.Due;
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].ListOfAction.Action[0].DueSpecified = true;
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].ListOfAction.Action[0].Priority = activity.Priority;
                ip.ListOfWc_Service_Request_Io.ServiceRequest[0].ListOfAction.Action[0].Status = activity.Status;
            }

            ip.LOVLanguageMode = "LIC";
            ip.ViewMode = "All";
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                SRSVC.WC_Service_Request_BSInsert_Output op;
                using (new OperationContextScope(client.InnerChannel))
                {
                    MessageHeader usrMsgHdr = MessageHeader.CreateHeader("UsernameToken", "http://siebel.com/webservices", SRServiceUsername);
                    OperationContext.Current.OutgoingMessageHeaders.Add(usrMsgHdr);
                    MessageHeader pwdMsgHdr = MessageHeader.CreateHeader("PasswordText", "http://siebel.com/webservices", SRServicePassword);
                    OperationContext.Current.OutgoingMessageHeaders.Add(pwdMsgHdr);
                    stopwatch.Start();
                    op = client.WC_Service_Request_BSInsert(ip);
                    stopwatch.Stop();
                    request = eBehavior.msgInspector.reqPayload;
                    response = eBehavior.msgInspector.resPayload;

                    SRSVC.ServiceRequestId opData = op.ListOfWc_Service_Request_Io[0];
                    activity.ID = opData.ListOfAction[0].Id;
                }
                logMessage = "Request of creating Activity (Success). Created Activity ID = " + activity.ID;
                logNote = "Request Payload: " + request;
                log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);

                logMessage = "Response of creating Activity(Success). Created Activity ID = " + activity.ID;
                logNote = "Response Payload: " + response;
                log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote, (int)stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                request = eBehavior.msgInspector.reqPayload;
                response = eBehavior.msgInspector.resPayload;

                activity.ErrorMessage = "There has been an error communicating with Siebel. Please check log for detail.";

                logMessage = "Request of creating  Activity(Failure). " + ex.Message;
                logNote = "Request Payload: " + request;
                log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                logMessage = "Response of creating  Activity(Failure). " + ex.Message;
                logNote = "Response Payload: " + response;
                log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                handleSiebelException(ex, "Create  Activity of Service Request", _logIncidentId, _logContactId);
            }
            return activity;
        }


    }
}