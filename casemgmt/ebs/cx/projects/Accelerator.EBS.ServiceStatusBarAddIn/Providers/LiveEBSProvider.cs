/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + EBS Enhancement
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  EBS release: 12.1.3
 *  reference: 150505-000099, 150420-000127
 *  date: Thu Nov 12 00:52:49 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: f0fad2e5487974568c4bf2e03ecd68248ce3c0d7 $
 * *********************************************************************************************
 *  File: LiveEBSProvider.cs
 * *********************************************************************************************/

using SR = Accelerator.EBS.SharedServices.ProxyClasses.ServiceRequest;
using SR_CREATE_CONTACT = Accelerator.EBS.SharedServices.ProxyClasses.ServiceRequest.APPSCS_SERVICEREQUX230189X13X250;
using SR_CREATE_NOTES = Accelerator.EBS.SharedServices.ProxyClasses.ServiceRequest.APPSCS_SERVICEREQUX230189X13X239;
using SR_CREATE_FIELDS = Accelerator.EBS.SharedServices.ProxyClasses.ServiceRequest.APPSCS_SERVICEREQUEX230189X13X14;
using SR_CREATE = Accelerator.EBS.SharedServices.ProxyClasses.ServiceRequest.APPSCS_SERVICEREQUEX230189X2X262;
using SR_UPDATE_CONTACT = Accelerator.EBS.SharedServices.ProxyClasses.ServiceRequest.APPSCS_SERVICEREQUEX230189X5X252;
using SR_UPDATE_NOTES = Accelerator.EBS.SharedServices.ProxyClasses.ServiceRequest.APPSCS_SERVICEREQUEX230189X5X241;
using SR_UPDATE_FIELDS = Accelerator.EBS.SharedServices.ProxyClasses.ServiceRequest.APPSCS_SERVICEREQUESX230189X5X16;
using SR_UPDATE = Accelerator.EBS.SharedServices.ProxyClasses.ServiceRequest.APPSCS_SERVICEREQUEX230189X4X268;
using SRL = Accelerator.EBS.SharedServices.ProxyClasses.ServiceRequest.APPSCS_SERVICEREQUESX230189X16X7;
using SR_LOOKUP_CONTACT = Accelerator.EBS.SharedServices.ProxyClasses.ServiceRequest.APPSCS_SERVICEREQUX230189X15X199;
using SR_LOOKUP_TASK = Accelerator.EBS.SharedServices.ProxyClasses.ServiceRequest.APPSCS_SERVICEREQUX230189X15X113;
using ITEM_LIST = Accelerator.EBS.SharedServices.ProxyClasses.Item.APPSCSI_DATASTRUCTUX219474X16X14;
using ENTITLEMENT_LIST = Accelerator.EBS.SharedServices.ProxyClasses.Entitlement.APPSOKS_ENTITLEMENTSX226932X4X19;
using REPAIR_ORDER_LIST = Accelerator.EBS.SharedServices.ProxyClasses.RepairOrderList.APPSCSD_REPAIR_ORDEX16435810X2X4;
using REPAIR_LOGISTICS_LIST = Accelerator.EBS.SharedServices.ProxyClasses.RepairLogisticsList.APPSCSD_LOGISTICS_WX16435811X3X4;
using Accelerator.EBS.SharedServices.ProxyClasses.OrderMgmt;
using ORDERS = Accelerator.EBS.SharedServices.ProxyClasses.OrdersByContact;

using NOTE = Accelerator.EBS.SharedServices.ProxyClasses.Interaction;
using NOTE_CREATE = Accelerator.EBS.SharedServices.ProxyClasses.Interaction.InputParameters;
using RO = Accelerator.EBS.SharedServices.ProxyClasses.RepairOrder;
using CONTACT = Accelerator.EBS.SharedServices.ProxyClasses.Contact;
using ITEM = Accelerator.EBS.SharedServices.ProxyClasses.Item;
using ENTITLEMENT = Accelerator.EBS.SharedServices.ProxyClasses.Entitlement;
using REPAIR_ORDER = Accelerator.EBS.SharedServices.ProxyClasses.RepairOrderList;
using REPAIR_LOGISTICS = Accelerator.EBS.SharedServices.ProxyClasses.RepairLogisticsList;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using Accelerator.EBS.SharedServices.Logs;
using System.Web.Script.Serialization;
using System.Diagnostics;
using RightNow.AddIns.AddInViews;

namespace Accelerator.EBS.SharedServices.Providers
{
    internal class LiveEBSProvider : IEBSProvider
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

        public int SRServiceTimeout
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

        public int ROServiceTimeout
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

        public int InteractionServiceTimeout
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

        public string OrderLookupURL
        {
            get;
            set;
        }

        public string OrderInboundURL
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
        
        public static JavaScriptSerializer serializer = new JavaScriptSerializer();

        #endregion

        #region Init Methods
        public void InitForSR(string service_url, string lookup_url, string user_name, string password, int timeout = -1)
        {
            SRServiceURL = service_url;
            SRLookupURL = lookup_url;
            SRServiceUsername = user_name;
            SRServicePassword = password;
            SRServiceTimeout = timeout;
        }
        public void InitForInteraction(string interaction_url, string user_name, string password, int timeout)
        {
            InteractionURL = interaction_url;
            InteractionUsername = user_name;
            InteractionPassword = password;
            InteractionServiceTimeout = timeout;
        }
        public void InitForRO(string report_url, string create_url, string update_url, string user_name, string password, int timeout)
        {            
            ROCreateURL = create_url;
            ROUpdateURL = update_url;
            RepairOrderListURL = report_url;
            ROServiceUsername = user_name;
            ROServicePassword = password;
            RepairOrderListServiceUsername = user_name;
            RepairOrderListServicePassword = password;
            RepairOrderListServiceTimeout = timeout;
            ROServiceTimeout = timeout;
        }

        public void InitForContact(string contact_url, string user_name, string password, int timeout)
        {
            ContactListLookupURL = contact_url;
            ContactServiceUsername = user_name;
            ContactServicePassword = password;
            ContactServiceTimeout = timeout;
        }

        public void InitForOrder(string order_url, string inboundURL)
        {
            OrderLookupURL = order_url;
            OrderInboundURL = inboundURL;
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

        #endregion

        #region Save/Lookup Methods
        public ServiceRequest CreateSR(ServiceRequest sr, int _logIncidentId = 0, int _logContactId = 0)
        {
            if (String.IsNullOrWhiteSpace(SRServiceURL) || String.IsNullOrWhiteSpace(SRServiceUsername) || String.IsNullOrWhiteSpace(SRServicePassword))
            {
                throw new Exception("Provider's InitForSR not run.");
            }

            ServiceRequest result = sr;
            string request = "";
            string response = "";
            string logMessage, logNote;

            SR.CS_SERVICEREQUEST_PUB_Service client = EBSProxyFactory.GetSRServiceInstance(SRServiceURL, SRServiceUsername, SRServicePassword, SRServiceTimeout);

            SR.SOAHeader hdr = new SR.SOAHeader();
            hdr.Responsibility = "SERVICE";
            hdr.RespApplication = "CS";
            hdr.Org_Id = "204";
            hdr.SecurityGroup = "STANDARD";
            hdr.NLSLanguage = "AMERICAN";

            client.SOAHeaderValue = hdr;


            // Now to set the values for the request            

            if (!sr.RequestID.HasValue)
            {
                SR_CREATE_FIELDS sr_info = new SR_CREATE_FIELDS();
                // SR.APPSCS_SERVICEREQUEST_PUB_SERVIC7 req_info = new SR.APPSCS_SERVICEREQUEST_PUB_SERVIC7();

                sr_info.REQUEST_DATE = sr.RequestDate;
                sr_info.REQUEST_DATESpecified = true;
                sr_info.TYPE_ID = sr.RequestTypeID;
                sr_info.TYPE_IDSpecified = true;
                sr_info.STATUS_ID = sr.StatusID;
                sr_info.STATUS_IDSpecified = true;
                sr_info.SEVERITY_ID = sr.SeverityID;
                sr_info.SEVERITY_IDSpecified = true;
                sr_info.SUMMARY = sr.Summary;
                sr_info.CURRENT_SERIAL_NUMBER = sr.SerialNumber;
                if (sr.ProductID.HasValue)
                {
                    sr_info.CUSTOMER_PRODUCT_ID = sr.ProductID;
                    sr_info.CUSTOMER_PRODUCT_IDSpecified = true;
                    sr_info.INVENTORY_ORG_ID = 204;
                    sr_info.INVENTORY_ORG_IDSpecified = true;

                }

                // add RnowHost to extra attr 13
                sr_info.EXTERNAL_ATTRIBUTE_13 = sr.RnowHost;

                // added by rranaa for oow 2014
                sr_info.EXTERNAL_ATTRIBUTE_14 = sr.IncidentRef;
                sr_info.EXTERNAL_ATTRIBUTE_15 = sr.IncidentID;

                sr_info.CUSTOMER_ID = Convert.ToDecimal(sr.ContactOrgID);
                sr_info.CUSTOMER_IDSpecified = true;
                sr_info.CALLER_TYPE = "ORGANIZATION";
                sr_info.GROUP_TYPE = "RS_GROUP";

                if (sr.CreatedByID.HasValue)
                {
                    sr_info.CREATED_BYSpecified = true;
                    sr_info.CREATED_BY = sr.CreatedByID;
                }

                if (sr.OwnerID.HasValue)
                {
                    sr_info.OWNER_IDSpecified = true;
                    sr_info.OWNER_ID = sr.OwnerID;
                }
                /*
                if (sr.InstanceID.HasValue)
                {
                    sr_info.CUSTOMER_PRODUCT_ID = sr.InstanceID.Value;
                    sr_info.CUSTOMER_PRODUCT_IDSpecified = true;
                }
                */
                if (sr.IncidentOccurredDate.HasValue)
                {
                    sr_info.INCIDENT_OCCURRED_DATE = sr.IncidentOccurredDate.Value;
                    sr_info.INCIDENT_OCCURRED_DATESpecified = true;
                }

                SR.InputParameters ip_create_sr = new SR.InputParameters();
                ip_create_sr.P_API_VERSION = 4.0M;
                ip_create_sr.P_API_VERSIONSpecified = true;
                ip_create_sr.P_SERVICE_REQUEST_REC = sr_info;

                // Hard-coding the contact for the demo
                SR_CREATE_CONTACT sr_contact = new SR_CREATE_CONTACT();
                //c.CONTACT_POINT_ID = 7675;
                sr_contact.CONTACT_POINT_IDSpecified = true;
                sr_contact.CONTACT_POINT_TYPE = "PHONE";
                sr_contact.PARTY_ID = Convert.ToDecimal(sr.EbsContactID);

                sr_contact.PARTY_IDSpecified = true;
                sr_contact.PRIMARY_FLAG = "Y";
                sr_contact.PARTY_ROLE_CODE = "CONTACT";
                sr_contact.CONTACT_TYPE = "PARTY_RELATIONSHIP";

                List<SR_CREATE_CONTACT> sr_clist = new List<SR_CREATE_CONTACT>();
                sr_clist.Add(sr_contact);

                ip_create_sr.P_CONTACTS = sr_clist.ToArray();

                // Notes
                List<SR_CREATE_NOTES> sr_notes = new List<SR_CREATE_NOTES>();
                if (!String.IsNullOrWhiteSpace(sr.Notes))
                {
                    SR_CREATE_NOTES note = new SR_CREATE_NOTES();
                    note.NOTE = sr.Notes;
                    note.NOTE_CONTEXT_TYPE_01 = String.Empty;
                    note.NOTE_CONTEXT_TYPE_02 = String.Empty;
                    note.NOTE_CONTEXT_TYPE_03 = String.Empty;
                    note.NOTE_DETAIL = String.Empty;
                    note.NOTE_TYPE = String.Empty;

                    sr_notes.Add(note);

                }
                ip_create_sr.P_NOTES = sr_notes.ToArray();

                ip_create_sr.P_AUTO_ASSIGN = String.Empty;
                ip_create_sr.P_AUTO_GENERATE_TASKS = String.Empty;
                ip_create_sr.P_DEFAULT_CONTRACT_SLA_IND = "Y";
                ip_create_sr.P_INIT_MSG_LIST = "T";
                ip_create_sr.P_COMMIT = "T";
                ip_create_sr.P_REQUEST_NUMBER = String.Empty;
                Stopwatch stopwatch = new Stopwatch();
                try
                {
                    request = serializer.Serialize(ip_create_sr);
                    stopwatch.Start();
                    SR.OutputParameters op_create_sr = client.CREATE_SERVICEREQUEST(ip_create_sr);
                    stopwatch.Stop();
                    response =  serializer.Serialize(op_create_sr);

                    if (op_create_sr.X_RETURN_STATUS == "S")
                    {
                        SR_CREATE data = op_create_sr.X_SR_CREATE_OUT_REC;
                        result.RequestID = data.REQUEST_ID;
                        result.RequestNumber = data.REQUEST_NUMBER;

                        logMessage = "Request of creating Service Request (Success). Created SR ID: " + result.RequestID;
                        logNote = "Request Payload: " + request;
                        log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);

                        logMessage = "Response of creating Service Request (Success). Created SR ID: " + result.RequestID;
                        logNote = "Response Payload: " + response;
                        log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote, (int)stopwatch.ElapsedMilliseconds);
                    }
                    else
                    {
                        result.ErrorMessage = "There has been an error communicating with EBS. Please check log for detail.";

                        logMessage = "Request of creating Service Request (Failure). " + op_create_sr.X_MSG_DATA;
                        logNote = "Request Payload: " + request;
                        log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                        logMessage = "Response of creating Service Request (Failure). " + op_create_sr.X_MSG_DATA;
                        logNote = "Response Payload: " + response;
                        log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
                    }

                }
                catch (Exception ex)
                {
                    result.ErrorMessage = "There has been an error communicating with EBS. Please check log for detail.";
                    
                    logMessage = "Request of creating Service Request (Failure). " + ex.Message;
                    logNote = "Request Payload: " + request;
                    log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                    logMessage = "Response of creating Service Request (Failure). " + ex.Message;
                    logNote = "Response Payload: " + response;
                    log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                    handleEBSException(ex, "Create Service Request", _logIncidentId, _logContactId);
                }
            }


            return result;
        }

        public RepairOrder CreateRO(RepairOrder ro, int _logIncidentId = 0, int _logContactId = 0)
        {
            RepairOrder result = ro;
            string request = "";
            string response = "";
            string logMessage, logNote;

            if (String.IsNullOrWhiteSpace(ROCreateURL) || String.IsNullOrWhiteSpace(ROServiceUsername) || String.IsNullOrWhiteSpace(ROServicePassword))
            {
                throw new Exception("Provider's InitForRO not run.");
            }

            RO.CSD_REPAIRS_PUB_Service client = EBSProxyFactory.GetDepotInstance(ROCreateURL, ROServiceUsername, ROServicePassword, ROServiceTimeout);
            RO.SOAHeader hdr = new RO.SOAHeader();
            hdr.Responsibility = "ORACLE_SUPPORT";
            hdr.RespApplication = "CSS";
            hdr.Org_Id = "204";
            hdr.SecurityGroup = "STANDARD";
            hdr.NLSLanguage = "AMERICAN";

            client.SOAHeaderValue = hdr;

            if (ro.RepairNumber == null)
            {
                RO.InputParameters ip_create_ro = new RO.InputParameters();
                ip_create_ro.P_API_VERSION_NUMBER = 1.0M;
                ip_create_ro.P_API_VERSION_NUMBERSpecified = true;
                ip_create_ro.P_INIT_MSG_LIST = "T";
                ip_create_ro.P_COMMIT = "F";
                ip_create_ro.P_CREATE_DEFAULT_LOGISTICS = "N";

                RO.APPSCSD_REPAIRS_PUB_RX220752X1X6 ro_info = new RO.APPSCSD_REPAIRS_PUB_RX220752X1X6();
                ro_info.INCIDENT_ID = ro.ServiceRequestID;
                ro_info.INCIDENT_IDSpecified = true;
                ro_info.UNIT_OF_MEASURE = ro.UnitOfMeasure;
                ro_info.REPAIR_TYPE_ID = ro.RepairTypeID;
                ro_info.REPAIR_TYPE_IDSpecified = true;
                ro_info.QUANTITY = ro.Quantity;
                ro_info.QUANTITYSpecified = true;
                ro_info.INVENTORY_ITEM_ID = ro.InventoryItemID;
                ro_info.INVENTORY_ITEM_IDSpecified = true;
                ro_info.APPROVAL_REQUIRED_FLAG = ro.ApprovalRequired;
                ro_info.CURRENCY_CODE = ro.Currency;
                ro_info.PROBLEM_DESCRIPTION = ro.ProblemDescription;
                ro_info.RESOURCE_ID = ro.ResourceID;
                ro_info.RESOURCE_IDSpecified = true;
                ro_info.AUTO_PROCESS_RMA = String.Empty;
                ro_info.SERIAL_NUMBER = ro.SerialNumber;
                if (!string.IsNullOrWhiteSpace(ro.HasValidSerialNumber))
                    ro_info.ATTRIBUTE15 = ro.HasValidSerialNumber;

                ip_create_ro.P_REPLN_REC = ro_info;
                Stopwatch stopwatch = new Stopwatch();
                try
                {
                    request = serializer.Serialize(ip_create_ro);
                    stopwatch.Start();
                    RO.OutputParameters op_create_ro = client.CREATE_REPAIR_ORDER(ip_create_ro);
                    stopwatch.Stop();
                    response = serializer.Serialize(op_create_ro);

                    if (op_create_ro.X_RETURN_STATUS == "S")
                    {
                        result.RepairNumber = op_create_ro.X_REPAIR_NUMBER;
                        result.RepairLineID = Convert.ToDecimal(op_create_ro.X_REPAIR_LINE_ID);

                        logMessage = "Request of creating Repair Order (Success). Created RO Number: " + result.RepairNumber;
                        logNote = "Request Payload: " + request;
                        log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);

                        logMessage = "Response of creating Repair Order (Success). Created RO Number: " + result.RepairNumber;
                        logNote = "Response Payload: " + response;

                        log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote, (int)stopwatch.ElapsedMilliseconds);
                    }
                    else
                    {
                        result.ErrorMessage = "There has been an error communicating with EBS. Please check log for detail.";

                        logMessage = "Request of creating Repair Order (Failure). " + op_create_ro.X_MSG_DATA;
                        logNote = "Request Payload: " + request;
                        log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                        logMessage = "Response of creating Repair Order (Failure). " + op_create_ro.X_MSG_DATA;
                        logNote = "Response Payload: " + response;
                        log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
                    }
                }
                catch (Exception ex)
                {
                    result.ErrorMessage = "There has been an error communicating with EBS. Please check log for detail.";

                    logMessage = "Request of creating Repair Order (Failure). " + ex.Message;
                    logNote = "Request Payload: " + request;
                    log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                    logMessage = "Response of creating Repair Order (Failure). " + ex.Message;
                    logNote = "Response Payload: " + response;
                    log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                    handleEBSException(ex, "Create Repair Order", _logIncidentId, _logContactId);
                }

            }

            return result;
        }

        public ServiceRequest LookupSR(decimal incident_id, string incident_number, int _logIncidentId = 0, int _logContactId = 0)
        {
            if (String.IsNullOrWhiteSpace(SRLookupURL) || String.IsNullOrWhiteSpace(SRServiceUsername) || String.IsNullOrWhiteSpace(SRServicePassword))
            {
                throw new Exception("Provider's InitForSR not run.");
            }

            SR.CS_SERVICEREQUEST_PUB_Service client = EBSProxyFactory.GetSRServiceInstance(SRLookupURL, SRServiceUsername, SRServicePassword, SRServiceTimeout);

            ServiceRequest result = new ServiceRequest();
            string request = "";
            string response = "";
            string logMessage, logNote;

            SR.SOAHeader hdr = new SR.SOAHeader();
            hdr.Responsibility = "SERVICE";
            hdr.RespApplication = "CS";
            hdr.Org_Id = "204";
            hdr.SecurityGroup = "STANDARD";
            hdr.NLSLanguage = "AMERICAN";

            client.SOAHeaderValue = hdr;

            SR.InputParameters3 ip_get_sr = new SR.InputParameters3();
            ip_get_sr.P_API_VERSION = 1;
            ip_get_sr.P_API_VERSIONSpecified = true;
            ip_get_sr.P_INCIDENT_ID = incident_id;
            ip_get_sr.P_INCIDENT_IDSpecified = true;
            ip_get_sr.P_INCIDENT_NUMBER = incident_number;
            List<SR_LOOKUP_CONTACT> contacts = new List<SR_LOOKUP_CONTACT>();
            ip_get_sr.X_CONTACT = contacts.ToArray();
            List<SR_LOOKUP_TASK> tasks = new List<SR_LOOKUP_TASK>();
            ip_get_sr.X_TASKS = tasks.ToArray();
            //ip.X_GETSR_OUT_REC = new SRL.APPSCS_SERVICEREQUEX16438971X1X2();
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                request = serializer.Serialize(ip_get_sr);
                stopwatch.Start();
                SR.OutputParameters3 op = client.GET_SR_INFO(ip_get_sr);
                stopwatch.Stop();
                //response = client.ResponseSoapContext.Envelope.Envelope.InnerXml;
                response = serializer.Serialize(op);

                if (op.X_RETURN_STATUS == "S")
                {
                    result.RequestID = op.X_GETSR_OUT_REC.INCIDENT_ID;
                    result.RequestNumber = op.X_GETSR_OUT_REC.INCIDENT_NUMBER;
                    result.IncidentID = op.X_GETSR_OUT_REC.EXTATTRIBUTE15;
                    result.IncidentRef = op.X_GETSR_OUT_REC.EXTATTRIBUTE14;
                    result.StatusID = op.X_GETSR_OUT_REC.INCIDENT_STATUS_ID;
                    result.Status = op.X_GETSR_OUT_REC.INCIDENT_STATUS;
                    result.SeverityID = op.X_GETSR_OUT_REC.INCIDENT_SEVERITY_ID;
                    result.Severity = op.X_GETSR_OUT_REC.INCIDENT_SEVERITY;
                    result.RequestTypeID = op.X_GETSR_OUT_REC.INCIDENT_TYPE_ID;
                    result.RequestType = op.X_GETSR_OUT_REC.INCIDENT_TYPE;
                    result.Summary = op.X_GETSR_OUT_REC.SUMMARY;
                    result.SrObjVerNum = op.X_GETSR_OUT_REC.OBJECT_VERSION_NUMBER;
                    result.SerialNumber = op.X_GETSR_OUT_REC.SERIAL_NUMBER;
                    result.ProductID = op.X_GETSR_OUT_REC.CUSTOMER_PRODUCT_ID;
                    result.Product = op.X_GETSR_OUT_REC.PRODUCT;
                    result.ProductDescription = op.X_GETSR_OUT_REC.PRODUCT_DESCRIPTION;

                    result.Owner = op.X_GETSR_OUT_REC.SR_OWNER;
                    result.OwnerID = op.X_GETSR_OUT_REC.SR_OWNER_ID;

                    logMessage = "Request of loading Service Request (Success). SR ID = " + incident_id;
                    logNote = "Request Payload: " + request;
                    log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);

                    logMessage = "Response of loading Service Request (Success). SR ID = " + incident_id;
                    logNote = "Response Payload: " + response;
                    log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote, (int)stopwatch.ElapsedMilliseconds);
                }
                else
                {
                    result.ErrorMessage = "There has been an error communicating with EBS. Please check log for detail.";

                    logMessage = "Request of loading Service Request (Failure). SR ID = " + incident_id + " Error: " + op.X_MSG_DATA;
                    logNote = "Request Payload: " + request;
                    log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                    logMessage = "Response of loading Service Request (Failure). SR ID = " + incident_id + " Error: " + op.X_MSG_DATA;
                    logNote = "Response Payload: " + response;
                    log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);               
                }
            }
            catch (Exception ex)
            {
                result.ErrorMessage = "There has been an error communicating with EBS. Please check log for detail.";
                logMessage = "Request of loading Service Request (Failure). SR ID = " + incident_id + " Error: " + ex.Message;
                logNote = "Request Payload: " + request;
                log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                logMessage = "Response of loading Service Request (Failure). SR ID = " + incident_id + " Error: " + ex.Message;
                logNote = "Response Payload: " + response;
                log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                handleEBSException(ex, "Lookup Service Request", _logIncidentId, _logContactId);
            }

            return result;
        }

        /*  call OE_ORDER_CUST_Service : GET_ORDERS_BY_CONTACT() 
         *  Output : op
         */
        public ORDERS.OutputParameters LookupOrdersByContact(decimal contact_id, int _logIncidentId = 0, int _logContactId = 0)
        {
            string request = "";
            string response = "";
            string logMessage, logNote;

            ORDERS.OE_ORDER_CUST_Service client = EBSProxyFactory.GetOrdersByContactServiceInstance(ConfigurationSetting.GetOrdersByCust_WSDL, ConfigurationSetting.username, ConfigurationSetting.password, ConfigurationSetting.EBSServiceTimeout);
            ORDERS.SOAHeader hdr = new ORDERS.SOAHeader();
            hdr.Responsibility = "ORDER_MGMT_SUPER_USER";
            hdr.RespApplication = "ONT";
            hdr.Org_Id = "204";
            hdr.SecurityGroup = "STANDARD";
            hdr.NLSLanguage = "AMERICAN";

            client.SOAHeaderValue = hdr;
     
            ORDERS.InputParameters ip = new ORDERS.InputParameters();

            ip.P_CONTACT_PARTY_ID = contact_id;
            ip.P_CONTACT_PARTY_IDSpecified = true;

            ORDERS.OutputParameters op = null;
            Stopwatch stopwatch = new Stopwatch();

            try
            {
                request = serializer.Serialize(ip);

                logMessage = "Request of getting Orders by contact (GET_ORDERS_BY_CONTACT). ";
                logNote = "Request Payload: " + request;
                log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);
                // call the web service, catch the exception right away
                stopwatch.Start();
                op = client.GET_ORDERS_BY_CONTACT(ip);
                stopwatch.Stop();
                response = serializer.Serialize(op);
            }
            catch (Exception ex)
            {
                handleEBSException(ex, "GET_ORDERS_BY_CONTACT", _logIncidentId, _logContactId);
                // will throw the new exception (either timeout or error communicating ...)
                throw;
            }

                logMessage = "Response of getting Order(GET_ORDERS_BY_CONTACT). ";
                logNote = "Response Payload: " + response;

                log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote, (int)stopwatch.ElapsedMilliseconds);           

            return op;
        }

        /*  call OE_ORDER_CUST_Service : GET_ORDERS_BY_INCIDENT() 
         *  Output : op
         */
        public ORDERS.OutputParameters1 LookupOrdersByIncident(decimal incident_id, int _logIncidentId = 0, int _logContactId = 0)
        {
            string request = "";
            string response = "";
            string logMessage, logNote;

            ORDERS.OE_ORDER_CUST_Service client = EBSProxyFactory.GetOrdersByContactServiceInstance(ConfigurationSetting.GetOrdersByCust_WSDL, ConfigurationSetting.username, ConfigurationSetting.password, ConfigurationSetting.EBSServiceTimeout);
            ORDERS.SOAHeader hdr = new ORDERS.SOAHeader();
            hdr.Responsibility = "ORDER_MGMT_SUPER_USER";
            hdr.RespApplication = "ONT";
            hdr.Org_Id = "204";
            hdr.SecurityGroup = "STANDARD";
            hdr.NLSLanguage = "AMERICAN";

            client.SOAHeaderValue = hdr;

            ORDERS.InputParameters1 ip = new ORDERS.InputParameters1();

            ip.P_INCIDENT_NUMBER = incident_id.ToString();

            ORDERS.OutputParameters1 op = null;
            Stopwatch stopwatch = new Stopwatch();

            try
            {
                request = serializer.Serialize(ip);

                logMessage = "Request of getting Orders by incident (GET_ORDERS_BY_INCIDENT). ";
                logNote = "Request Payload: " + request;
                log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);
                // call the web service, catch the exception right away
                stopwatch.Start();
                op = client.GET_ORDERS_BY_INCIDENT(ip);
                stopwatch.Stop();
                response = serializer.Serialize(op);
            }
            catch (Exception ex)
            {
                handleEBSException(ex, "GET_ORDERS_BY_INCIDENT", _logIncidentId, _logContactId);
                // will throw the new exception (either timeout or error communicating ...)
                throw;
            }

            logMessage = "Response of getting Order(GET_ORDERS_BY_INCIDENT). ";
            logNote = "Response Payload: " + response;

            log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote, (int)stopwatch.ElapsedMilliseconds);

            return op;
        }

        /*  call OE_ORDER_PUB_Service : GET_ORDER() 
         *  Output : op.X_HEADER_VAL_REC
         */
        public OutputParameters2 LookupOrderDetail(decimal order_id, int _logIncidentId = 0, int _logContactId = 0)
        {
            string request = "";
            string response = "";
            string logMessage, logNote;
            // reuse SR user, pwd, timeout settting, the same
            
            if (String.IsNullOrWhiteSpace(OrderLookupURL))
            {
                throw new Exception("Provider's InitForOrder not run.");
            }

            OE_ORDER_PUB_Service client = EBSProxyFactory.GetOrderServiceInstance(OrderLookupURL, ConfigurationSetting.username, ConfigurationSetting.password, ConfigurationSetting.EBSServiceTimeout);
            SOAHeader hdr = new SOAHeader();
            hdr.Responsibility = "ORDER_MGMT_SUPER_USER";
            hdr.RespApplication = "ONT";
            hdr.Org_Id = "204";
            hdr.SecurityGroup = "STANDARD";
            hdr.NLSLanguage = "AMERICAN";

            client.SOAHeaderValue = hdr;

            InputParameters2 ip = new InputParameters2();

            ip.P_API_VERSION_NUMBER = 1;
            ip.P_API_VERSION_NUMBERSpecified = true;
            ip.P_INIT_MSG_LIST = "T";
            ip.P_RETURN_VALUES = "T";
            ip.P_HEADER_ID = order_id;
            ip.P_HEADER_IDSpecified = true;

            OutputParameters2 op = null;
            Stopwatch stopwatch = new Stopwatch();

            try
            {
                request = serializer.Serialize(ip);

                logMessage = "Request of getting Order (GET_ORDER). ";
                logNote = "Request Payload: " + request;
                log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);
                // call the web service, catch the exception right away
                stopwatch.Start();
                op = client.GET_ORDER(ip);
                stopwatch.Stop();
                response = serializer.Serialize(op);
            }
            catch (Exception ex)
            {
                handleEBSException(ex, "GET_ORDER", _logIncidentId, _logContactId);
                // will throw the new exception (either timeout or error communicating ...)
                throw;
            }

            if (op.X_RETURN_STATUS == "S")
            {
                logMessage = "Response of getting Order(GET_ORDER). ";
                logNote = "Response Payload: " + response;

                log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote, (int)stopwatch.ElapsedMilliseconds);
            }
            else
            {
                logMessage = "Response of getting Order (GET_ORDER (Failure). " + op.X_MSG_DATA;
                logNote = "Response Payload: " + response;
                log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
            }

            return op;
        }
        /*  call CS_SERVICEREQUEST_PUB_Service : GET_SR_INFO
         *  Output : op.X_GETSR_OUT_REC
         *  call dictAddProperty() to add the individual property name, type, and value
         *  for dynamic columns feature
         */
        public Dictionary<string, string> LookupSRDetail(decimal incident_id, string incident_number, int _logIncidentId = 0 , int _logContactId = 0)
        {
            string request = "";
            string response = "";
            string logMessage, logNote;

            if (String.IsNullOrWhiteSpace(SRLookupURL) || String.IsNullOrWhiteSpace(SRServiceUsername) || String.IsNullOrWhiteSpace(SRServicePassword))
            {
                throw new Exception("Provider's InitForSR not run.");
            }

            SR.CS_SERVICEREQUEST_PUB_Service client = EBSProxyFactory.GetSRServiceInstance(SRLookupURL, SRServiceUsername, SRServicePassword, SRServiceTimeout);

            SR.InputParameters3 ip = new SR.InputParameters3();
            ip.P_API_VERSION = 1;
            ip.P_API_VERSIONSpecified = true;
            ip.P_INCIDENT_ID = incident_id;
            ip.P_INCIDENT_IDSpecified = true;
            ip.P_INCIDENT_NUMBER = incident_number;
            List<SR_LOOKUP_CONTACT> contacts = new List<SR_LOOKUP_CONTACT>();
            ip.X_CONTACT = contacts.ToArray();
            List<SR_LOOKUP_TASK> tasks = new List<SR_LOOKUP_TASK>();
            ip.X_TASKS = tasks.ToArray();
            SR.OutputParameters3 op = null;
            Stopwatch stopwatch = new Stopwatch();
           
            try
            {
                request = serializer.Serialize(ip);

                logMessage = "Request of getting SR details (GET_SR_INFO). ";
                logNote = "Request Payload: " + request;
                log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);
                // call the web service, catch the exception right away
                stopwatch.Start();
                op = client.GET_SR_INFO(ip);
                stopwatch.Stop();
                response = serializer.Serialize(op);
            }
            catch (Exception ex)
            {
                handleEBSException(ex, "GET_SR_INFO", _logIncidentId, _logContactId);
                // will throw the new exception (either timeout or error communicating ...)
                // b/c caller SRdetailVirtualTable GetRows expect reportRows, so need to throw it to show the msg box
                throw;
            }

            if (op.X_RETURN_STATUS == "S")
            {
                logMessage = "Response of getting SR details (GET_SR_INFO). ";
                logNote = "Response Payload: " + response;

                log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote, (int)stopwatch.ElapsedMilliseconds);
            }
            else
            {
                logMessage = "Response of getting SR details (GET_SR_INFO (Failure). " + op.X_MSG_DATA;
                logNote = "Response Payload: " + response;
                log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
            }
            Dictionary<string, string> dictDetail = new Dictionary<string, string>();

            foreach (PropertyInfo propertyInfo in op.X_GETSR_OUT_REC.GetType().GetProperties())
            {
                Object propVal = op.X_GETSR_OUT_REC.GetType().GetProperty(propertyInfo.Name).GetValue(op.X_GETSR_OUT_REC, null);
                dictAddProperty(propertyInfo, propVal, ref dictDetail);                     
            }

            return dictDetail;
        }

        /*  This method calls
         *  CS_SERVICEREQUEST_PUB_Service : GET_SRINFO_BYCONTACT
         *  Called by SRlistVirtualTable GetRows() to display Service Requests for a contact
         */
        public ServiceRequest[] LookupSRbyContactPartyID(decimal contact_id, int _logIncidentId = 0, int _logContactId = 0)
        {
            string request = "";
            string response = "";
            string logMessage, logNote;

            if (String.IsNullOrWhiteSpace(SRLookupURL) || String.IsNullOrWhiteSpace(SRServiceUsername) || String.IsNullOrWhiteSpace(SRServicePassword))
            {
                throw new Exception("Provider's InitForSR not run.");
            }

            SR.CS_SERVICEREQUEST_PUB_Service client = EBSProxyFactory.GetSRServiceInstance(SRLookupURL, SRServiceUsername, SRServicePassword, SRServiceTimeout);

            SR.InputParameters1 ip = new SR.InputParameters1();
            ip.P_API_VERSION = 1;
            ip.P_API_VERSIONSpecified = true;
            ip.P_CONTACT = contact_id;
            ip.P_CONTACTSpecified = true;
            ip.P_COMMIT = "T";
            ip.P_INIT_MSG_LIST = "T";
            SR.OutputParameters1 op = null;
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                request = serializer.Serialize(ip);
                logMessage = "Request of getting SRs by contactId (GET_SRINFO_BYCONTACT). ";
                logNote = "Request Payload: " + request;
                log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);
                // call the web service, catch the exception right away
                stopwatch.Start();
                op = client.GET_SRINFO_BYCONTACT(ip);
                stopwatch.Stop();
                response = serializer.Serialize(op);
            }
            catch (Exception ex)
            {
                handleEBSException(ex, "GET_SRINFO_BYCONTACT", _logIncidentId, _logContactId);
                // will throw the new exception (either timeout or error communicating ...)
                // b/c caller SRlistVirtualTable GetRows expect reportRows, so need to throw it to show the msg box
                throw; 
            }

            if (op.X_RETURN_STATUS == "S")
            {
                logMessage = "Response of getting SRs by contactId (GET_SRINFO_BYCONTACT). ";
                logNote = "Response Payload: " + response;
                log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote, (int)stopwatch.ElapsedMilliseconds);
            }
            else
            {
                logMessage = "Response of getting SRs by contactId (GET_SRINFO_BYCONTACT) (Failure). " + op.X_MSG_DATA;
                logNote = "Response Payload: " + response;
                log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
            }

            ServiceRequest[] retvals = new ServiceRequest[op.X_SR_DET_TBL.Length];
            int i = 0;
            foreach (SR.APPSCS_SERVICEREQUESX230189X17X6 sr in op.X_SR_DET_TBL)
            {
                // sr.EXTATTRIBUTE15 is Rnow IncidentID
                if (sr.EXTATTRIBUTE15 == null || sr.EXTATTRIBUTE15 == "")
                {
                    ServiceRequest req = new ServiceRequest();
                    req.RequestID = sr.INCIDENT_ID;
                    req.RequestNumber = sr.INCIDENT_NUMBER;
                    req.StatusID = sr.INCIDENT_STATUS_ID;
                    req.Status = sr.INCIDENT_STATUS;
                    req.SeverityID = sr.INCIDENT_SEVERITY_ID;
                    req.Severity = sr.INCIDENT_SEVERITY;
                    req.RequestTypeID = sr.INCIDENT_TYPE_ID;
                    req.RequestType = sr.INCIDENT_TYPE;
                    req.Summary = sr.SUMMARY;
                    req.RequestDate = (DateTime)sr.CREATION_DATE;
                    retvals[i] = req;
                    i++;
                }
            }
            return retvals;
        }

        public ServiceRequest UpdateSR(ServiceRequest sr, int _logIncidentId = 0, int _logContactId = 0)
        {
            if (String.IsNullOrWhiteSpace(SRServiceURL) || String.IsNullOrWhiteSpace(SRServiceUsername) || String.IsNullOrWhiteSpace(SRServicePassword))
            {
                throw new Exception("Provider's InitForSR not run.");
            }

            ServiceRequest result = sr;
            string request = "";
            string response = "";
            string logMessage, logNote;

            SR.CS_SERVICEREQUEST_PUB_Service client = EBSProxyFactory.GetSRServiceInstance(SRServiceURL, SRServiceUsername, SRServicePassword, SRServiceTimeout);

            SR.SOAHeader hdr = new SR.SOAHeader();
            hdr.Responsibility = "SERVICE";
            hdr.RespApplication = "CS";
            hdr.Org_Id = "204";
            hdr.SecurityGroup = "STANDARD";
            hdr.NLSLanguage = "AMERICAN";

            client.SOAHeaderValue = hdr;


            // Now to set the values for the request            

            if (sr.RequestID.HasValue)
            {
                SR_UPDATE_FIELDS sr_info = new SR_UPDATE_FIELDS();
                // SR.APPSCS_SERVICEREQUEST_PUB_SERVIC7 req_info = new SR.APPSCS_SERVICEREQUEST_PUB_SERVIC7();

                sr_info.REQUEST_DATE = sr.RequestDate;
                sr_info.REQUEST_DATESpecified = true;
                sr_info.TYPE_ID = sr.RequestTypeID;
                sr_info.TYPE_IDSpecified = true;
                sr_info.STATUS_ID = sr.StatusID;
                sr_info.STATUS_IDSpecified = true;
                sr_info.SEVERITY_ID = sr.SeverityID;
                sr_info.SEVERITY_IDSpecified = true;
                sr_info.SUMMARY = sr.Summary;
                sr_info.CURRENT_SERIAL_NUMBER = sr.SerialNumber;
                //req_info.CONTRACT_ID = 38373;
                sr_info.CONTRACT_IDSpecified = true;

                // added by rranaa for oow 2014
                sr_info.EXTERNAL_ATTRIBUTE_14 = sr.IncidentRef;
                sr_info.EXTERNAL_ATTRIBUTE_15 = sr.IncidentID;

                // Hard-coding some values
                if (String.IsNullOrEmpty(sr.ContactOrgID) || sr.ContactOrgID == "0")
                    sr_info.CUSTOMER_ID = 4429;
                else
                    sr_info.CUSTOMER_ID = Convert.ToDecimal(sr.ContactOrgID);
                sr_info.CUSTOMER_IDSpecified = true;
                sr_info.CALLER_TYPE = "ORGANIZATION";

                if (sr.OwnerID.HasValue)
                {
                    sr_info.OWNER_ID = sr.OwnerID;
                    sr_info.OWNER_IDSpecified = true;
                }
                /*

                if (sr.InstanceID.HasValue)
                {
                    sr_info.CUSTOMER_PRODUCT_ID = sr.InstanceID.Value;
                    sr_info.CUSTOMER_PRODUCT_IDSpecified = true;
                }
                */
                sr_info.VERIFY_CP_FLAG = "N";
                if (sr.ProductID.HasValue)
                {
                    sr_info.CUSTOMER_PRODUCT_ID = sr.ProductID;
                    sr_info.CUSTOMER_PRODUCT_IDSpecified = true;
                    sr_info.INVENTORY_ORG_ID = 204;
                    sr_info.INVENTORY_ORG_IDSpecified = true;

                }
                else
                {
                    sr_info.CUSTOMER_PRODUCT_ID = null;
                    sr_info.CUSTOMER_PRODUCT_IDSpecified = true;
                    sr_info.INVENTORY_ORG_ID = 204;
                    sr_info.INVENTORY_ORG_IDSpecified = true;
                }


                SR.InputParameters8 ip_update_sr = new SR.InputParameters8();
                ip_update_sr.P_REQUEST_ID = sr.RequestID;
                ip_update_sr.P_REQUEST_IDSpecified = true;
                ip_update_sr.P_REQUEST_NUMBER = sr.RequestNumber;
                ip_update_sr.P_API_VERSION = 4.0M;
                ip_update_sr.P_API_VERSIONSpecified = true;
                ip_update_sr.P_SERVICE_REQUEST_REC = sr_info;

                ip_update_sr.P_OBJECT_VERSION_NUMBER = sr.SrObjVerNum;
                ip_update_sr.P_OBJECT_VERSION_NUMBERSpecified = true;

                // Hard-coding the contact for the demo


                List<SR_UPDATE_CONTACT> clist = new List<SR_UPDATE_CONTACT>();

                ip_update_sr.P_CONTACTS = clist.ToArray();

                // Notes
                List<SR_UPDATE_NOTES> notes = new List<SR_UPDATE_NOTES>();
                if (!String.IsNullOrWhiteSpace(sr.Notes))
                {
                    SR_UPDATE_NOTES note = new SR_UPDATE_NOTES();
                    note.NOTE = sr.Notes;
                    note.NOTE_CONTEXT_TYPE_01 = String.Empty;
                    note.NOTE_CONTEXT_TYPE_02 = String.Empty;
                    note.NOTE_CONTEXT_TYPE_03 = String.Empty;
                    note.NOTE_DETAIL = String.Empty;
                    note.NOTE_TYPE = String.Empty;

                    notes.Add(note);

                }
                ip_update_sr.P_NOTES = notes.ToArray();

                ip_update_sr.P_AUTO_ASSIGN = String.Empty;
                ip_update_sr.P_DEFAULT_CONTRACT_SLA_IND = "Y";
                ip_update_sr.P_INIT_MSG_LIST = "T";
                ip_update_sr.P_COMMIT = "T";
                ip_update_sr.P_REQUEST_NUMBER = String.Empty;
                ip_update_sr.P_CALLED_BY_WORKFLOW = String.Empty;
                Stopwatch stopwatch = new Stopwatch();
                try
                {
                    request = serializer.Serialize(ip_update_sr);
                    stopwatch.Start();
                    SR.OutputParameters8 op_update_sr = client.UPDATE_SERVICEREQUEST(ip_update_sr);
                    stopwatch.Stop();
                    response = serializer.Serialize(op_update_sr);

                    if (op_update_sr.X_RETURN_STATUS == "S")
                    {
                        SR_UPDATE data = op_update_sr.X_SR_UPDATE_OUT_REC;

                        logMessage = "Request of updating Service Request (Success). SR ID = " + sr.RequestID;
                        logNote = "Request Payload: " + request;
                        log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);

                        logMessage = "Response of updating Service Request (Success). SR ID = " + sr.RequestID;
                        logNote = "Response Payload: " + response;
                        log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote, (int)stopwatch.ElapsedMilliseconds);
                    }
                    else
                    {
                        result.ErrorMessage = "There has been an error communicating with EBS. Please check log for detail.";
                        
                        logMessage = "Request of updating Service Request (Failure). SR ID = " + sr.RequestID + " Error: " + op_update_sr.X_MSG_DATA;
                        logNote = "Request Payload: " + request;
                        log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                        logMessage = "Response of updating Service Request (Failure). SR ID = " + sr.RequestID + " Error: " + op_update_sr.X_MSG_DATA;
                        logNote = "Response Payload: " + response;
                        log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
                    }

                }
                catch (Exception ex)
                {
                    result.ErrorMessage = "There has been an error communicating with EBS. Please check log for detail.";
                    
                    logMessage = "Request of updating Service Request (Failure). SR ID = " + sr.RequestID + " Error: " + ex.Message;
                    logNote = "Request Payload: " + request;
                    log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                    logMessage = "Response of updating Service Request (Failure). SR ID = " + sr.RequestID + " Error: " + ex.Message;
                    logNote = "Response Payload: " + response;
                    log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                    handleEBSException(ex, "Update Service Request", _logIncidentId, _logContactId);
                }
            }


            return result;

        }
        public RepairOrder UpdateRO(RepairOrder ro, int _logIncidentId = 0, int _logContactId = 0)
        {
            //throw new NotImplementedException();
            RepairOrder result = ro;
            string request = "";
            string response = "";

            string logMessage, logNote;

            if (String.IsNullOrWhiteSpace(ROUpdateURL) || String.IsNullOrWhiteSpace(ROServiceUsername) || String.IsNullOrWhiteSpace(ROServicePassword))
            {
                throw new Exception("Provider's InitForRO not run.");
            }

            RO.CSD_REPAIRS_PUB_Service client = EBSProxyFactory.GetDepotInstance(ROCreateURL, ROServiceUsername, ROServicePassword, ROServiceTimeout);
            RO.SOAHeader hdr = new RO.SOAHeader();
            hdr.Responsibility = "ORACLE_SUPPORT";
            hdr.RespApplication = "CSS";
            hdr.Org_Id = "204";
            hdr.SecurityGroup = "STANDARD";
            hdr.NLSLanguage = "AMERICAN";

            client.SOAHeaderValue = hdr;

            if (ro.RepairNumber != null)
            {
                RO.InputParameters1 ip_update_ro_status = new RO.InputParameters1();

                ip_update_ro_status.P_API_VERSION = 1.0M;
                ip_update_ro_status.P_API_VERSIONSpecified = true;
                ip_update_ro_status.P_INIT_MSG_LIST = "F";
                ip_update_ro_status.P_COMMIT = "T";

                RO.APPSCSD_REPAIRS_PUB_RX220752X3X7 repair_status = new RO.APPSCSD_REPAIRS_PUB_RX220752X3X7();

                repair_status.REPAIR_NUMBER = ro.RepairNumber;
                repair_status.REPAIR_STATUS_ID = ro.RepairStatusID;
                repair_status.REPAIR_STATUS_IDSpecified = true;
                repair_status.FROM_STATUS_ID = ro.StoredRepairStatusID;
                repair_status.FROM_STATUS_IDSpecified = true;
                repair_status.OBJECT_VERSION_NUMBER = 0;
                repair_status.OBJECT_VERSION_NUMBERSpecified = true;
                repair_status.REASON_CODE = String.Empty;
                ip_update_ro_status.P_REPAIR_STATUS_REC = repair_status;

                RO.APPSCSD_REPAIRS_PUB_X220752X3X18 other = new RO.APPSCSD_REPAIRS_PUB_X220752X3X18();
                other.CHECK_TASK_WIP = String.Empty;
                ip_update_ro_status.P_STATUS_UPD_CONTROL_REC = other;

                Stopwatch stopwatch = new Stopwatch();
                try
                {
                    request = serializer.Serialize(ip_update_ro_status);
                    stopwatch.Start();
                    RO.OutputParameters1 op_update_ro_status = client.UPDATE_RO_STATUS(ip_update_ro_status);
                    stopwatch.Stop();
                    response = serializer.Serialize(op_update_ro_status);
                    if (op_update_ro_status.X_RETURN_STATUS == "S")
                    {

                        result.RepairNumber = ro.RepairNumber;


                        logMessage = "Request of updating Repair Order (Success). RO Number = " + result.RepairNumber;
                        logNote = "Request Payload: " + request;
                        log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);

                        logMessage = "Response of updating Repair Order  (Success). RO Number = " + result.RepairNumber;
                        logNote = "Response Payload: " + response;

                        log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote, (int)stopwatch.ElapsedMilliseconds);

                    }
                    else if (op_update_ro_status.X_RETURN_STATUS == "U")
                    {
                        result.ErrorMessage = String.Format("Cannot set status from {0} to {1}", ro.StoredRepairStatus, ro.RepairStatus);

                        logMessage = "Request of updating Repair Order(Failure). RO Number = " + result.RepairNumber + " Error: " + result.ErrorMessage;
                        logNote = "Request Payload: " + request;
                        log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                        logMessage = "Response of updating Repair Order (Failure). RO Number = " + result.RepairNumber + " Error: " + result.ErrorMessage;
                        logNote = "Response Payload: " + response;
                        log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                        result.RepairNumber = "-1";
                    }
                    else
                    {
                        result.ErrorMessage = "There has been an error communicating with EBS. Please check log for detail.";

                        logMessage = "Request of updating Repair Order(Failure). RO Number = "  +result.RepairNumber + " Error: " + op_update_ro_status.X_MSG_DATA;
                        logNote = "Request Payload: " + request;
                        log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                        logMessage = "Response of updating Repair Order (Failure). RO Number = "  +result.RepairNumber + " Error: " + op_update_ro_status.X_MSG_DATA;
                        logNote = "Response Payload: " + response;
                        log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
                    }
                }
                catch (Exception ex)
                {
                    result.ErrorMessage = "There has been an error communicating with EBS. Please check log for detail.";

                    logMessage = "Request of updating Repair Order(Failure). RO Number = " + result.RepairNumber + " Error: " + ex.Message;
                    logNote = "Request Payload: " + request;
                    log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                    logMessage = "Response of updating Repair Order (Failure). RO Number = " + result.RepairNumber + " Error: " + ex.Message;
                    logNote = "Response Payload: " + response;
                    log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                    handleEBSException(ex, "Update Repair Order Status");
                }

            }

            return result;
        }
        public Interaction CreateInteraction(Interaction interaction, int _logIncidentId = 0, int _logContactId = 0)
        {

            if (String.IsNullOrWhiteSpace(InteractionURL) || String.IsNullOrWhiteSpace(InteractionUsername) || String.IsNullOrWhiteSpace(InteractionPassword))
            {
                throw new Exception("Provider's InitForSR not run.");
            }

            Interaction result = new Interaction();
            string request = "";
            string response = "";
            string logMessage, logNote;

            NOTE.JTF_NOTES_PUB_Service client = EBSProxyFactory.GetNoteInstance(InteractionURL, InteractionUsername, InteractionPassword, InteractionServiceTimeout);

            NOTE.SOAHeader hdr = new NOTE.SOAHeader();
            hdr.Responsibility = "SERVICE";
            hdr.RespApplication = "CS";
            hdr.Org_Id = "204";
            hdr.SecurityGroup = "STANDARD";
            hdr.NLSLanguage = "AMERICAN";

            client.SOAHeaderValue = hdr;


            // Now to set the values for the request            

            if (interaction.SrID.HasValue)
            {
                NOTE_CREATE ip_create_note = new NOTE_CREATE();
                // SR.APPSCS_SERVICEREQUEST_PUB_SERVIC7 req_info = new SR.APPSCS_SERVICEREQUEST_PUB_SERVIC7();
                ip_create_note.P_API_VERSION = 1;
                ip_create_note.P_API_VERSIONSpecified = true;
                ip_create_note.P_ORG_ID = 204;
                ip_create_note.P_ORG_IDSpecified = true;
                ip_create_note.P_SOURCE_OBJECT_ID = interaction.SrID;
                ip_create_note.P_SOURCE_OBJECT_IDSpecified = true;
                ip_create_note.P_SOURCE_OBJECT_CODE = "SR";
                //ip.P_NOTES = "This is test.";

                ip_create_note.P_NOTES = interaction.Summary;
                ip_create_note.P_NOTES_DETAIL = interaction.Content;
                ip_create_note.P_NOTE_STATUS = interaction.Status;
                ip_create_note.P_LAST_UPDATE_DATE = interaction.Created;
                ip_create_note.P_LAST_UPDATE_DATESpecified = true;
                ip_create_note.P_CREATION_DATE = interaction.Created;
                ip_create_note.P_CREATION_DATESpecified = true;
                ip_create_note.P_ENTERED_DATE = interaction.Created;
                ip_create_note.P_ENTERED_DATESpecified = true;

                if (interaction.CreatedByID.HasValue)
                {
                    ip_create_note.P_CREATED_BY = interaction.CreatedByID;
                    ip_create_note.P_CREATED_BYSpecified = true;
                    ip_create_note.P_ENTERED_BY = interaction.CreatedByID;
                    ip_create_note.P_ENTERED_BYSpecified = true;
                    ip_create_note.P_LAST_UPDATED_BY = interaction.CreatedByID;
                    ip_create_note.P_LAST_UPDATED_BYSpecified = true;
                }
                else
                {
                    ip_create_note.P_CREATED_BY = 0;
                    ip_create_note.P_CREATED_BYSpecified = true;
                    ip_create_note.P_ENTERED_BY = 0;
                    ip_create_note.P_ENTERED_BYSpecified = true;
                    ip_create_note.P_LAST_UPDATED_BY = 0;
                    ip_create_note.P_LAST_UPDATED_BYSpecified = true;
                }

                ip_create_note.P_NOTE_TYPE = "CS_PROBLEM";

                ip_create_note.P_ATTRIBUTE1 = String.Empty;
                ip_create_note.P_ATTRIBUTE2 = String.Empty;
                ip_create_note.P_ATTRIBUTE3 = String.Empty;
                ip_create_note.P_ATTRIBUTE4 = String.Empty;
                ip_create_note.P_ATTRIBUTE5 = String.Empty;
                ip_create_note.P_ATTRIBUTE6 = String.Empty;
                ip_create_note.P_ATTRIBUTE7 = String.Empty;
                ip_create_note.P_ATTRIBUTE8 = String.Empty;
                ip_create_note.P_ATTRIBUTE9 = String.Empty;
                ip_create_note.P_ATTRIBUTE10 = String.Empty;
                ip_create_note.P_ATTRIBUTE11 = String.Empty;
                ip_create_note.P_ATTRIBUTE12 = String.Empty;
                ip_create_note.P_ATTRIBUTE13 = String.Empty;
                ip_create_note.P_ATTRIBUTE14 = String.Empty;
                ip_create_note.P_ATTRIBUTE15 = String.Empty;
                ip_create_note.P_PARENT_NOTE_ID = null;
                ip_create_note.P_INIT_MSG_LIST = String.Empty;
                ip_create_note.P_COMMIT = String.Empty;
                ip_create_note.P_USE_AOL_SECURITY = String.Empty;

                //ip.P_NOTES_DETAIL = String.Empty;
                //ip.P_JTF_NOTE_CONTEXTS_TAB = new NOTE.APPSJTF_NOTES_PUB_JX227585X15X42();
                List<NOTE.APPSJTF_NOTES_PUB_JX227585X15X42> notes = new List<NOTE.APPSJTF_NOTES_PUB_JX227585X15X42>();

                ip_create_note.P_JTF_NOTE_CONTEXTS_TAB = notes.ToArray();
                Stopwatch stopwatch = new Stopwatch();
                try
                {
                    request = serializer.Serialize(ip_create_note);
                    stopwatch.Start();
                    NOTE.OutputParameters op_create_note = client.SECURE_CREATE_NOTE(ip_create_note);
                    stopwatch.Stop();
                    response = serializer.Serialize(op_create_note);

                    if (op_create_note.X_RETURN_STATUS == "S")
                    {
                        result.InteractionID = op_create_note.X_JTF_NOTE_ID;

                        logMessage = "Request of creating Note (Success). Created Note ID = " + result.InteractionID;
                        logNote = "Request Payload: " + request;
                        log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);

                        logMessage = "Response of creating Note(Success). Created Note ID = " + result.InteractionID;
                        logNote = "Response Payload: " + response;
                        log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote, (int)stopwatch.ElapsedMilliseconds);
                    }
                    else
                    {
                        result.ErrorMessage = "There has been an error communicating with EBS. Please check log for detail.";

                        logMessage = "Request of creating Note (Failure). " + op_create_note.X_MSG_DATA;
                        logNote = "Request Payload: " + request;
                        log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                        logMessage = "Response of creating Note(Failure). " + op_create_note.X_MSG_DATA;
                        logNote = "Response Payload: " + response;
                        log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
                    }

                }
                catch (Exception ex)
                {
                    result.ErrorMessage = "There has been an error communicating with EBS. Please check log for detail.";

                    logMessage = "Request of creating Note (Failure). " + ex.Message;
                    logNote = "Request Payload: " + request;
                    log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                    logMessage = "Response of creating Note(Failure). " + ex.Message;
                    logNote = "Response Payload: " + response;
                    log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                    handleEBSException(ex, "Create Note of Service Request", _logIncidentId, _logContactId);
                }
            }

            return result;

        }

        public ContactModel[] LookupContactList(string firstname, string lastname, string phone, string email, int _logIncidentId = 0, int _logContactId = 0)
        {
            ContactModel[] retvals = null;
            string request = "";
            string response = "";
            string logMessage, logNote;

            if (String.IsNullOrWhiteSpace(ContactListLookupURL) || String.IsNullOrWhiteSpace(ContactServiceUsername) || String.IsNullOrWhiteSpace(ContactServicePassword))
            {
                throw new Exception("Provider's InitForContact not run.");
            }

            CONTACT.HZ_INTEGRATION_PUB_Service client = EBSProxyFactory.GetContactInstance(ContactListLookupURL, ContactServiceUsername, ContactServicePassword, ContactServiceTimeout);

            CONTACT.SOAHeader hdr = new CONTACT.SOAHeader();
            hdr.Responsibility = "SYSTEM_ADMINISTRATOR";
            hdr.RespApplication = "SYSADMIN";
            hdr.SecurityGroup = "STANDARD";
            hdr.NLSLanguage = "AMERICAN";
            hdr.Org_Id = "204";

            client.SOAHeaderValue = hdr;

            CONTACT.InputParameters ip_contact_list = new CONTACT.InputParameters();

            /* not supported in new official api
            ip.P_FIRST_NAME = firstname;
            ip.P_LAST_NAME = lastname;
             */

            if (email != null)
                ip_contact_list.P_EMAIL = email;

            ip_contact_list.P_RECORD_LIMIT = 200;
            ip_contact_list.P_RECORD_LIMITSpecified = true;

            if (!String.IsNullOrEmpty(phone) && phone.Length >= 7)
            {
                if (phone[phone.Length - 5] == '-' || phone[phone.Length - 5] == ' ')
                {
                    ip_contact_list.P_PHONE = phone.Substring(phone.Length - 8);
                }
                else
                {
                    ip_contact_list.P_PHONE = phone.Substring(phone.Length - 7);
                }
            }

            Stopwatch stopwatch = new Stopwatch();
            try
            {
                request = serializer.Serialize(ip_contact_list);
                stopwatch.Start();
                CONTACT.OutputParameters op_contact_list = client.GET_CONTACT_DETAIL(ip_contact_list);
                stopwatch.Stop();
                response = serializer.Serialize(op_contact_list);

                if (op_contact_list.X_RETURN_STATUS == "S")
                {
                    List<ContactModel> contacts = new List<ContactModel>();

                    foreach (CONTACT.APPSHZ_INTEGRATION_PX3348183X1X7 op in op_contact_list.X_CONTACT_REC_TBL)
                    {
                        ContactModel contact = new ContactModel();
                        contact.ContactPartyID = op.RELATIONSHIP_PARTY_ID;
                        contact.FirstName = op.PERSON_FIRST_NAME;
                        contact.LastName = op.PERSON_LAST_NAME;
                        contact.Email = op.PRIMARY_EMAIL;
                        contact.ContactOrgID = op.ORG_PARTY_ID;
                        contact.PhoneNumber = op.PRIMARY_PHONE;
                        contacts.Add(contact);
                    }
                    retvals = contacts.ToArray();

                    logMessage = "Request of search Contact (Success). Email = " + email + "; Phone = " + phone;
                    logNote = "Request Payload: " + request;
                    log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);

                    logMessage = "Response of search Contact (Success). Email = " + email + "; Phone = " + phone;
                    logNote = "Response Payload: " + response;
                    log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote, (int)stopwatch.ElapsedMilliseconds);
                }
                else
                {
                    List<ContactModel> contacts = new List<ContactModel>();
                    ContactModel contact = new ContactModel();
                    contact.ErrorMessage = "There has been an error communicating with EBS. Please check log for detail.";
                    contacts.Add(contact);
                    retvals = contacts.ToArray();

                    logMessage = "Request of search Contact (Failure). Email = " + email + "; Phone = " + phone;
                    logNote = "Request Payload: " + request;
                    log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                    logMessage = "Response of search Contact (Failure). Email = " + email + "; Phone = " + phone;
                    logNote = "Response Payload: " + response;
                    log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
                }
            }

            catch (Exception ex)
            {
                List<ContactModel> contacts = new List<ContactModel>();
                ContactModel contact = new ContactModel();
                contact.ErrorMessage = "There has been an error communicating with EBS. Please check log for detail.";
                contacts.Add(contact);
                retvals = contacts.ToArray();

                logMessage = "Request of search Contact (Failure). Email = " + email + "; Phone = " + phone + ". Error: " + ex.Message;
                logNote = "Request Payload: " + request;
                log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                logMessage = "Response of search Contact (Failure). Email = " + email + "; Phone = " + phone + ". Error: " + ex.Message;
                logNote = "Response Payload: " + response;
                log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                handleEBSException(ex, "Search Contact", _logIncidentId, _logContactId);
            }

            return retvals;
        }

        /* This method calls
         * HZ_INTEGRATION_PUB_Service : GET_CONTACT_DETAIL()
         * Its output is a table, foreach loop is used 
         * (even though only one row is returned by setting record limit = 1)
         * APPSHZ_INTEGRATION_PX3348183X1X7 is generated and need to be updated 
         * if proxy is regenerated
         *  call dictAddProperty() to add the individual property name, type, and value
         *  for dynamic columns feature
         */
        public Dictionary<string, string> LookupContactDetail(decimal party_id, int _logIncidentId = 0, int _logContactId = 0)
        {
            string request = "";
            string response = "";
            string logMessage, logNote;

            if (String.IsNullOrWhiteSpace(ContactListLookupURL) || String.IsNullOrWhiteSpace(ContactServiceUsername) || String.IsNullOrWhiteSpace(ContactServicePassword))
            {
                throw new Exception("Provider's InitForContact not run.");
            }

            CONTACT.HZ_INTEGRATION_PUB_Service client = EBSProxyFactory.GetContactInstance(ContactListLookupURL, ContactServiceUsername, ContactServicePassword, ContactServiceTimeout);

            CONTACT.InputParameters ip = new CONTACT.InputParameters();
            ip.P_PARTY_ID = party_id;
            ip.P_PARTY_IDSpecified = true;
            ip.P_RECORD_LIMIT = 1; // only return one contact
            ip.P_RECORD_LIMITSpecified = true;
            CONTACT.OutputParameters op = null;
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                request = serializer.Serialize(ip);

                logMessage = "Request of getting EBS contact details (GET_CONTACT_DETAIL). ";
                logNote = "Request Payload: " + request;
                log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);
                // call the web service, catch the exception right away
                stopwatch.Start();
                op = client.GET_CONTACT_DETAIL(ip);
                stopwatch.Stop();
                response = serializer.Serialize(op);
            }
            catch (Exception ex)
            {
                handleEBSException(ex, "GET_CONTACT_DETAIL", 0, (int)party_id);
                // will throw the new exception (either timeout or error communicating ...)
                // b/c caller ContactDetailVirtualTable GetRows expect reportRows, so need to throw it to show the msg box
                throw;
            }
            if (op.X_RETURN_STATUS == "S")
            {
                logMessage = "Response of getting EBS contact details (GET_CONTACT_DETAIL). ";
                logNote = "Response Payload: " + response;
                log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote, (int) stopwatch.ElapsedMilliseconds);
            }
            else
            {
                logMessage = "Response of getting EBS contact details (GET_CONTACT_DETAIL) (Failure). ";
                logNote = "Response Payload: " + response;
                log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
            }
            Dictionary<string, string> dictDetail = new Dictionary<string, string>();
            ContactModel[] retvals = new ContactModel[op.X_CONTACT_REC_TBL.Length];

            foreach (CONTACT.APPSHZ_INTEGRATION_PX3348183X1X7 contact in op.X_CONTACT_REC_TBL)
            {
                foreach (PropertyInfo propertyInfo in contact.GetType().GetProperties())
                {
                    Object propVal = contact.GetType().GetProperty(propertyInfo.Name).GetValue(contact, null);
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
                case 1:
                    rnStatusToMockStatus = new KeyValuePair<String, String>("1", "Open");
                    break;
                case 3:
                case 8:
                    rnStatusToMockStatus = new KeyValuePair<String, String>("1", "Open");
                    break;
                case 2:
                    rnStatusToMockStatus = new KeyValuePair<String, String>("2", "Close");
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

        private Dictionary<string, ReportColumnType> getVirtualTableColumns(object o)
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


        public KeyValuePair<String, String> rnRequestTypeToServerRequestType(int rnRequestTypeID)
        {

            KeyValuePair<String, String> rnRequestTypeToMockRequestType;
            switch (rnRequestTypeID)
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
            if (ConfigurationSetting.requestTypeMapping != null)
            {
                String ebsRequestType = (from f in ConfigurationSetting.requestTypeMapping where f.Key == rnRequestTypeID.ToString() select f.Value).FirstOrDefault();
                if (ebsRequestType != null)
                {
                    rnRequestTypeToMockRequestType = new KeyValuePair<String, String>(ebsRequestType, "");
                }

            }
            return rnRequestTypeToMockRequestType;

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
                case "Nullable`1": // because of System.Nullable<generic>
                    value = propVal != null ? propVal.ToString() : null;
                    string nullableType = propertyInfo.PropertyType.GetGenericArguments()[0].ToString();
                    if (nullableType == "System.Decimal")
                        typeValue = "Integer" + TYPE_VALUE_DELIMITER + value;
                    else if (nullableType == "System.DateTime")
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



        /*  If timed out, show such message, else generic one
         *  The error details are in the log (eg: pslog)
         */
        public void handleEBSException(Exception ex, string wsMethod, int _logIncidentId = 0, int _logContactId = 0)
        {
            string logMessage = wsMethod + " web service error.";
            string logNote = ex.Message;
            ConfigurationSetting.logWrap.ErrorLog(_logIncidentId,_logContactId, logMessage, logNote);
            string errorMsg = "There has been an error communicating with EBS. Please check log for detail.";
            if (ex.Message == "The operation has timed out")
            {
                errorMsg = "The connection to EBS has timed out.";
            }

            Exception eNew = new Exception(errorMsg);
            //MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            throw eNew;  
        }

        /* Called by ContactDetailVirtualTable() constructor 
         * when report addIn is loaded when CX launch
         * APPSCS_SERVICEREQUESX230189X15X4 is generated and need to be updated 
         * if proxy is regenerated
         */
        public Dictionary<string, string> getContactDetailSchema()
        {
            CONTACT.APPSHZ_INTEGRATION_PX3348183X1X7 contactDetailObject = new CONTACT.APPSHZ_INTEGRATION_PX3348183X1X7();
            Dictionary<string, string> dictDetail = new Dictionary<string, string>();
            foreach (PropertyInfo propertyInfo in contactDetailObject.GetType().GetProperties())
            {
                dictAddProperty(propertyInfo, null, ref dictDetail);
            }
            return dictDetail;
        }

        /* Called by SRdetailVirtualTable() constructor 
         * when report addIn is loaded when CX launch
         * APPSCS_SERVICEREQUESX230189X15X4 is generated and need to be updated 
         * if proxy is regenerated
         */
        public Dictionary<string, string> getServiceRequestDetailSchema()
        {
            SR.APPSCS_SERVICEREQUESX230189X15X4 srDetailObject = new SR.APPSCS_SERVICEREQUESX230189X15X4();
            Dictionary<string, string> dictDetail = new Dictionary<string, string>();
            foreach (PropertyInfo propertyInfo in srDetailObject.GetType().GetProperties())
            {
                dictAddProperty(propertyInfo, null, ref dictDetail);
            }
            return dictDetail;
        }

        /// <summary>
        /// Returns EBS item instances for a given serial number and contact
        /// </summary>
        /// <param name="serial_number"></param>
        /// <param name="contact_org_id"></param>
        /// <param name="active_instance_only"></param>
        /// <param name="rntIncidentId"></param>
        /// <param name="rntContactId"></param>
        public Item[] LookupItemList (string serial_number, decimal contact_org_id, string active_instance_only,
            int rntIncidentId = 0, int rntContactId = 0)
        {
            Item[] retvals = new Item[0];
            string logMessage, logNote;
                //validate ebs service user and password
            if (String.IsNullOrWhiteSpace(ItemListURL) || String.IsNullOrWhiteSpace(ItemServiceUsername) || String.IsNullOrWhiteSpace(ItemServicePassword))
            {
                throw new Exception("Provider's InitForItem not run.");
            }
                //create ebs soap header
            ITEM.CSI_ITEM_INSTANCE_PUB_Service client = EBSProxyFactory.GetItemInstance(ItemListURL, ItemServiceUsername, ItemServicePassword, ItemServiceTimeout);
            ITEM.SOAHeader hdr = new ITEM.SOAHeader();
            hdr.Responsibility = "INSTALLED_BASE";
            hdr.RespApplication = "CSI";
            hdr.SecurityGroup = "STANDARD";
            hdr.NLSLanguage = "AMERICAN";
            hdr.Org_Id = "204";

            client.SOAHeaderValue = hdr;

                //create ebs soap request payload
            ITEM.InputParameters2 ip = new ITEM.InputParameters2();
            ip.P_API_VERSION = 1;
            ip.P_API_VERSIONSpecified = true;
            ip.P_COMMIT = "F";
            ip.P_INIT_MSG_LIST = "T";
            ip.P_VALIDATION_LEVEL = 1;
            ip.P_VALIDATION_LEVELSpecified = true;
            ip.P_RESOLVE_ID_COLUMNS = "T";
            ip.P_ACTIVE_INSTANCE_ONLY = active_instance_only;
                
                //log input parameters
            string parameters = String.Format("GET_INSTANCES_BY_ITEM_SERIAL for serial_number {0}, contact_org_id {1}",
                serial_number, contact_org_id.ToString());

            string error = "No valid parameters found. No search performed.";
            if (String.IsNullOrWhiteSpace(serial_number))
            {
                logMessage = parameters + ". " + error;
                logNote = null;
                ConfigurationSetting.logWrap.DebugLog(incidentId: rntIncidentId, contactId: rntContactId, logMessage: logMessage, logNote: logNote);
                return retvals;
            }
            else
            {
                ip.P_SERIAL_NUMBER = serial_number;
            }
            if (contact_org_id > 0)
            {
                ip.P_PARTY_ID = contact_org_id;
                ip.P_PARTY_IDSpecified = true;
                ip.P_PARTY_REL_TYPE_CODE = "OWNER";
            }
            else
            {
                logMessage = parameters + ". " + error;
                logNote = null;
                ConfigurationSetting.logWrap.DebugLog(incidentId: rntIncidentId, contactId: rntContactId, logMessage: logMessage, logNote: logNote);
                return retvals;
            }

            logMessage = parameters + ". Request payload.";
            logNote = serializer.Serialize(ip);
            ConfigurationSetting.logWrap.DebugLog(incidentId: rntIncidentId, contactId: rntContactId, logMessage: logMessage, logNote: logNote);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

                //invoke the ebs web service
            StringBuilder response = new StringBuilder("[{}");
            try
            {
                ITEM.OutputParameters2 opList = client.GET_INSTANCES_BY_ITEM_SERIAL(ip);               
                List<Item> items = new List<Item>();

                foreach (ITEM_LIST op in opList.X_INSTANCE_HEADER_TBL)
                {
                    Item item = new Item(getPropertyValues(op), serial_number, contact_org_id, active_instance_only);
                    items.Add(item);
                    response.Append(",").Append(item.ToJSON());
                }
                retvals = items.ToArray();
            }
            catch (Exception ex)
            {
                handleEBSException(ex, parameters, rntIncidentId, rntContactId);
                throw;
            }

            stopwatch.Stop();
            int timeElapsed = stopwatch.Elapsed.Milliseconds;
            logMessage = new StringBuilder(parameters)
                .Append(" returned ").Append(retvals.Count()).Append(" records in ")
                .Append(timeElapsed).Append("ms")
                .ToString(); logNote = response.Append(",{\"Count\":").Append("\"")
                .Append(retvals.Count()).Append("\"}]").ToString();
            ConfigurationSetting.logWrap.DebugLog(incidentId: rntIncidentId, contactId: rntContactId, logMessage: logMessage, logNote: logNote, timeElapsed: timeElapsed);
            return retvals;
        }

        private Dictionary<string, object> getPropertyValues(object o)
        {
            PropertyInfo[] properties = o.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            Dictionary<string, object> result = new Dictionary<string, object>();
            foreach (PropertyInfo p in properties)
            {
                result.Add(p.Name, p.GetValue(o,null));
            }
            return result;
        }

        public Dictionary<string, ReportColumnType> getItemSchema()
        {
            return getVirtualTableColumns(new ITEM_LIST());
        }

        public Entitlement[] LookupEntitlementList(decimal instance_id, string validate_flag)
        {
            Entitlement[] retvals = new Entitlement[0];
            string logMessage, logNote;

            if (String.IsNullOrWhiteSpace(EntitlementListURL) || 
                String.IsNullOrWhiteSpace(EntitlementServiceUsername) ||
                String.IsNullOrWhiteSpace(EntitlementServicePassword))
            {
                throw new Exception("Provider's InitForEntitlement not run.");
            }
            ENTITLEMENT.OKS_ENTITLEMENTS_PUB_Service client = EBSProxyFactory.GetEntitlementInstance(EntitlementListURL, EntitlementServiceUsername, EntitlementServicePassword, EntitlementServiceTimeout);
            ENTITLEMENT.SOAHeader hdr = new ENTITLEMENT.SOAHeader();
            hdr.Responsibility = "OKS_MANAGER";
            hdr.RespApplication = "OKS";
            hdr.SecurityGroup = "STANDARD";
            hdr.NLSLanguage = "AMERICAN";
            hdr.Org_Id = "204";

            client.SOAHeaderValue = hdr;

            ENTITLEMENT.InputParameters5 ip = new ENTITLEMENT.InputParameters5();
            ip.P_API_VERSION = 1;
            ip.P_API_VERSIONSpecified = true;
            ip.P_INIT_MSG_LIST = "T";
            ip.P_INP_REC = new Accelerator.EBS.SharedServices.ProxyClasses.Entitlement.APPSOKS_ENTITLEMENTS_X226932X4X3(); 
            ip.P_INP_REC.VALIDATE_FLAG = validate_flag;
            ip.P_INP_REC.CONTRACT_NUMBER = "";
            //log input parameters
            string parameters = String.Format("GET_CONTRACTS__1 for instance_id {0}", instance_id.ToString());

            if (instance_id > 0)
            {
                ip.P_INP_REC.PRODUCT_ID = instance_id;
                ip.P_INP_REC.PRODUCT_IDSpecified = true;
            }
            else
            {
                logMessage = parameters + ". No valid parameters found. No search performed.";
                logNote = null;
                ConfigurationSetting.logWrap.DebugLog(logMessage: logMessage, logNote: logNote);
                return retvals;
            }
            
            logMessage = parameters + ". Request payload.";
            logNote = serializer.Serialize(ip);
            ConfigurationSetting.logWrap.DebugLog(logMessage: logMessage, logNote: logNote);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            
            StringBuilder response = new StringBuilder("[{}");
            try
            {
                ENTITLEMENT.OutputParameters5 opList = client.GET_CONTRACTS__1(ip);
                List<Entitlement> entitlements = new List<Entitlement>();

                foreach (ENTITLEMENT_LIST op in opList.X_ENT_CONTRACTS)
                {
                    Entitlement e = new Entitlement(getPropertyValues(op), instance_id, validate_flag);
                    entitlements.Add(e);
                    response.Append(",").Append(e.ToJSON());
                }
                retvals = entitlements.ToArray();
            }

            catch (Exception ex)
            {
                handleEBSException(ex, parameters);
                throw;
            }

            stopwatch.Stop();
            int timeElapsed = stopwatch.Elapsed.Milliseconds;
            logMessage = new StringBuilder(parameters)
                .Append(" returned ").Append(retvals.Count()).Append(" records in ")
                .Append(timeElapsed).Append("ms")
                .ToString();
            logNote = response.Append(",{\"Count\":").Append("\"")
                .Append(retvals.Count()).Append("\"}]").ToString();
            ConfigurationSetting.logWrap.DebugLog(logMessage: logMessage, logNote: logNote, timeElapsed: timeElapsed);
            return retvals;
        }

        public Dictionary<string, ReportColumnType> getEntitlementSchema()
        {
            return getVirtualTableColumns(new ENTITLEMENT_LIST());
        }

        private Dictionary<string, string> getVirtualTableSchema(object o)
        {
            Dictionary<string, string> dictDetail = new Dictionary<string, string>();
            foreach (PropertyInfo propertyInfo in o.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                dictAddProperty(propertyInfo, null, ref dictDetail);
            }
            return dictDetail;
        }
        /// <summary>
        /// Returns a list of repair orders from EBS
        /// </summary>
        /// <param name="contact_id"></param>
        /// <param name="incident_id"></param>
        /// <param name="incident_number"></param>
        /// <param name="repair_number"></param>
        /// <param name="rntIncidentId"></param>
        /// <param name="rntContactId"></param>
        public RepairOrder[] LookupRepairOrderList(decimal contact_id, decimal incident_id, 
            string incident_number, string repair_number,
            int rntIncidentId = 0, int rntContactId = 0)
        {
            RepairOrder[] retvals = new RepairOrder[0];
            string logMessage, logNote;
                //validate user name and password for EBS web service
            if (String.IsNullOrWhiteSpace(RepairOrderListURL) ||
                String.IsNullOrWhiteSpace(RepairOrderListServiceUsername) ||
                String.IsNullOrWhiteSpace(RepairOrderListServicePassword))
            {
                throw new Exception("Provider's InitForRO not run.");
            }
                //create a soap header
            REPAIR_ORDER.CSD_REPAIR_ORDERS_WEB_Service client = EBSProxyFactory.GetRepairOrderListInstance(RepairOrderListURL, RepairOrderListServiceUsername, RepairOrderListServicePassword, RepairOrderListServiceTimeout);
            REPAIR_ORDER.SOAHeader hdr = new REPAIR_ORDER.SOAHeader();
            hdr.Responsibility = "DEPOT REPAIR";
            hdr.RespApplication = "CSD";
            hdr.SecurityGroup = "STANDARD";
            hdr.NLSLanguage = "AMERICAN";
            hdr.Org_Id = "204";

            client.SOAHeaderValue = hdr;
                //create request payload
            REPAIR_ORDER.InputParameters ip = new REPAIR_ORDER.InputParameters();
            bool has_incident = false;
            bool has_contact = false;
            if (incident_id > 0)
            {
                ip.P_INCIDENT_ID = Convert.ToString(incident_id);
                ip.P_INCIDENT_IDSpecified = true;
                has_incident = true;
            }
            else
            {
                ip.P_INCIDENT_ID = "";
                ip.P_INCIDENT_IDSpecified = false;
            }
            if (contact_id > 0)
            {
                has_contact = true;
                ip.P_SR_CONTACT_PARTY_ID = Convert.ToString(contact_id);
                ip.P_SR_CONTACT_PARTY_IDSpecified = true;
                ip.P_SR_CONTACT_TYPE = "PARTY_RELATIONSHIP";
            }
            else
            {
                ip.P_SR_CONTACT_PARTY_ID = "";
                ip.P_SR_CONTACT_PARTY_IDSpecified = false;
                ip.P_SR_CONTACT_TYPE = "";
            }
            bool has_repair_number = false;
            if (String.IsNullOrWhiteSpace(repair_number))
            {
                ip.P_REPAIR_NUMBER = "";
            }
            else
            {
                has_repair_number = true;
                ip.P_REPAIR_NUMBER = repair_number;
            }
            bool has_incident_number = false;
            if (String.IsNullOrWhiteSpace(incident_number))
            {
                ip.P_INCIDENT_NUMBER = "";
            }
            else
            {
                has_incident_number = true;
                ip.P_INCIDENT_NUMBER = incident_number;
            }
            string parameters = String.Format("GET_REPAIR_ORDERS_WEBSRVC for contact_id {0}, incident_id {1}, incident_number {2}, repair_number {3}",
                Convert.ToString(contact_id), Convert.ToString(incident_id), Convert.ToString(incident_number), Convert.ToString(repair_number));

            if ( !(has_incident | has_contact | has_repair_number | has_incident_number) )
            {
                logMessage = parameters + ". No valid parameters found. No search performed.";
                logNote = null;
                ConfigurationSetting.logWrap.DebugLog(incidentId: rntIncidentId, contactId: rntContactId, logMessage: logMessage, logNote: logNote);
                return retvals;
            }
            ip.P_RECORD_LIMIT = 100;
            ip.P_RECORD_LIMITSpecified = true;
            ip.P_REPAIR_LINE_ID = "";
            ip.P_REPAIR_LINE_IDSpecified = false;
            ip.P_SR_CUSTOMER_PARTY_ID = "";
            ip.P_SR_CUSTOMER_PARTY_IDSpecified = false;
            ip.P_SR_CUSTOMER_TYPE = "";

            logMessage = parameters + ". Request payload.";
            logNote = serializer.Serialize(ip);
            ConfigurationSetting.logWrap.DebugLog(incidentId: rntIncidentId, contactId: rntContactId, logMessage: logMessage, logNote: logNote);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
                //invoke ebs
            StringBuilder response = new StringBuilder("[{}");
            try
            {
                REPAIR_ORDER.OutputParameters opList = client.GET_REPAIR_ORDERS_WEBSRVC(ip);
                List<RepairOrder> items = new List<RepairOrder>();

                foreach (REPAIR_ORDER_LIST op in opList.X_REPLN_TBL)
                {
                    RepairOrder ro = new RepairOrder(getPropertyValues(op), contact_id, incident_id);
                    items.Add(ro);
                    response.Append(",").Append(ro.ToJSON());
                }
                retvals = items.ToArray();
            }
            catch (Exception ex)
            {
                handleEBSException(ex, parameters, rntIncidentId, rntContactId);
                throw;
            }
            stopwatch.Stop();
            int timeElapsed = stopwatch.Elapsed.Milliseconds;
            logMessage = new StringBuilder(parameters)
                .Append(" returned ").Append(retvals.Count()).Append(" records in ")
                .Append(timeElapsed).Append("ms")
                .ToString();
            logNote = response.Append(",{\"Count\":").Append("\"")
                .Append(retvals.Count()).Append("\"}]").ToString();
            ConfigurationSetting.logWrap.DebugLog(incidentId: rntIncidentId, contactId: rntContactId, logMessage: logMessage, logNote: logNote, timeElapsed: timeElapsed);
            return retvals;
        }

        public Dictionary<string, ReportColumnType> getRepairOrderListSchema()
        {
            return getVirtualTableColumns(new REPAIR_ORDER_LIST());
        }

        public RepairLogistics[] LookupRepairLogisticsList(decimal repair_order_id)
        {
            RepairLogistics[] retvals = new RepairLogistics[0];
            string logMessage, logNote;

            if (String.IsNullOrWhiteSpace(RepairLogisticsListURL) ||
                String.IsNullOrWhiteSpace(RepairLogisticsListServiceUsername) ||
                String.IsNullOrWhiteSpace(RepairLogisticsListServicePassword))
            {
                throw new Exception("Provider's InitForRepairLogisticsList not run.");
            }
            REPAIR_LOGISTICS.CSD_LOGISTICS_WEB_Service client = EBSProxyFactory.GetRepairLogisticsInstance(RepairLogisticsListURL, RepairLogisticsListServiceUsername, RepairLogisticsListServicePassword, RepairLogisticsListServiceTimeout);
            REPAIR_LOGISTICS.SOAHeader hdr = new REPAIR_LOGISTICS.SOAHeader();
            hdr.Responsibility = "DEPOT REPAIR";
            hdr.RespApplication = "CSD";
            hdr.SecurityGroup = "STANDARD";
            hdr.NLSLanguage = "AMERICAN";
            hdr.Org_Id = "204";

            client.SOAHeaderValue = hdr;

            REPAIR_LOGISTICS.InputParameters ip = new REPAIR_LOGISTICS.InputParameters();
            ip.P_RECORD_LIMIT = 101;
            ip.P_RECORD_LIMITSpecified = true;
            string parameters = String.Format("GET_LOGISTICS_LINES_WEBSRVC for repair_order_id {0}",
                Convert.ToString(repair_order_id));
            if (repair_order_id <= 0)
            {
                logMessage = parameters + ". No valid parameters found. No search performed.";
                logNote = null;
                ConfigurationSetting.logWrap.DebugLog(logMessage: logMessage, logNote: logNote);
                return retvals;
            }
            ip.P_REPAIR_LINE_ID = repair_order_id;
            ip.P_REPAIR_LINE_IDSpecified = true;
            ip.P_REPAIR_NUMBER = "";

            logMessage = parameters + ". Request payload.";
            logNote = serializer.Serialize(ip);
            ConfigurationSetting.logWrap.DebugLog(logMessage: logMessage, logNote: logNote);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            StringBuilder response = new StringBuilder("[{}");
            try
            {
                REPAIR_LOGISTICS.OutputParameters opList = client.GET_LOGISTICS_LINES_WEBSRVC(ip);
                List<RepairLogistics> items = new List<RepairLogistics>();

                foreach (REPAIR_LOGISTICS_LIST op in opList.X_PROD_TXN_TBL)
                {
                    RepairLogistics ro = new RepairLogistics(getPropertyValues(op), repair_order_id);
                    items.Add(ro);
                    response.Append(",").Append(ro.ToJSON());
                }
                retvals = items.ToArray();
            }

            catch (Exception ex)
            {
                handleEBSException(ex, parameters);
                throw;
            }

            stopwatch.Stop();
            int timeElapsed = stopwatch.Elapsed.Milliseconds;
            logMessage = new StringBuilder(parameters)
                .Append(" returned ").Append(retvals.Count()).Append(" records in ")
                .Append(timeElapsed).Append("ms")
                .ToString(); 
            logNote = response.Append(",{\"Count\":").Append("\"")
                .Append(retvals.Count()).Append("\"}]").ToString();
            ConfigurationSetting.logWrap.DebugLog(logMessage: logMessage, logNote: logNote, timeElapsed: timeElapsed);
            return retvals;
        }

        public Dictionary<string, ReportColumnType> getRepairLogisticsListSchema()
        {
            return getVirtualTableColumns(new REPAIR_LOGISTICS_LIST());
        }

        #endregion
    }
}
