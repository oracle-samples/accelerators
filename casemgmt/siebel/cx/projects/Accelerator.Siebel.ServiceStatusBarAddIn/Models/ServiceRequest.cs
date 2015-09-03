/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC Contact Center + Siebel Case Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.5 (May 2015)
 *  Siebel release: 8.1.1.15
 *  reference: 141216-000121
 *  date: Wed Sep  2 23:14:41 PDT 2015

 *  revision: rnw-15-8-fixes-release-01
 *  SHA1: $Id: 1de90612e2d2b5f2021f18b240ffdcb2021dbce0 $
 * *********************************************************************************************
 *  File: ServiceRequest.cs
 * *********************************************************************************************/

using Accelerator.Siebel.SharedServices.Providers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accelerator.Siebel.SharedServices
{
    public class ServiceRequest : ModelObjectBase
    {
        public static ISiebelProvider _provider;

        public static string CreateUpdateURL { get; set; }
        public static string LookupURL { get; set; }
        

        public string ErrorMessage { get; set; }

        public string RequestID { get; set; }
        public string RequestNumber { get; set; }

        public DateTime RequestDate { get; set; }
        public DateTime? IncidentOccurredDate { get; set; }
        public string RequestType { get; set; }
        public string Status { get; set; }
        public string Severity { get; set; }
        public string Summary { get; set; }
        public string Owner { get; set; }
        public string Notes { get; set; }
        public string SerialNumber { get; set; }
        public string ContactOrgID { get; set; }

        public string IncidentRef { get; set; }
        public string IncidentID { get; set; }
        public string ContactID { get; set; }
        public string ProductID { get; set; }
        public string Product { get; set; }
        public string ProductDescription { get; set; }
        public string OwnerID { get; set; }

        public string RnowHost { get; set; }

        public bool Create(int _logIncidentId = 0, int _logContactId = 0)
        {

            if (_provider == null)
            {
                throw new Exception("Siebel Provider not initialized.");
            }

            ServiceRequest req = ServiceRequest._provider.CreateSR(this, _logIncidentId, _logContactId);

            this.RequestID = req.RequestID;
            this.RequestNumber = req.RequestNumber;
            this.ErrorMessage = req.ErrorMessage;

            return String.IsNullOrWhiteSpace(this.ErrorMessage);
        }

        public bool Update(int _logIncidentId = 0, int _logContactId = 0)
        {

            if (_provider == null)
            {
                throw new Exception("Siebel Provider not initialized.");
            }

            ServiceRequest req = ServiceRequest._provider.UpdateSR(this, _logIncidentId, _logContactId);

            this.RequestID = req.RequestID;
            this.RequestNumber = req.RequestNumber;
            this.ErrorMessage = req.ErrorMessage;

            return String.IsNullOrWhiteSpace(this.ErrorMessage);
        }

        public ServiceRequest Lookup(string incident_id,int _logIncidentId = 0, int _logContactId = 0)
        {
            return ServiceRequest._provider.LookupSR(incident_id, _logIncidentId,  _logContactId);
        }

        public static Dictionary<string, string> LookupDetail(IList<string> columns, string incident_id, int _logIncidentId = 0, int _logContactId = 0)
        {
            return ServiceRequest._provider.LookupSRDetail(columns, incident_id, _logIncidentId, _logContactId);
        }

        public static Dictionary<string, string> getDetailSchema()
        {
            return ServiceRequest._provider.getServiceRequestDetailSchema();
        }

        public static ServiceRequest[] LookupSRbyContactPartyID(IList<string> columns, string contact_id, int _logIncidentId = 0, int _logContactId = 0)
        {
            return ServiceRequest._provider.LookupSRbyContactPartyID(columns, contact_id, _logIncidentId, _logContactId);
        }

        public static void InitSiebelProvider()
        {
            Type t = Type.GetType(ServiceProvider);

            try
            {
                _provider = Activator.CreateInstance(t) as ISiebelProvider;
                _provider.InitForSR(LookupURL, ServiceUsername, ServicePassword, ServiceClientTimeout);
                _provider.log = ConfigurationSetting.logWrap;
            }
            catch (Exception ex)
            {
                if (ConfigurationSetting.logWrap != null)
                {
                    string logMessage = "Error in init Provider in Service Request Model. Error: " + ex.Message;
                    string logNote = "";
                    ConfigurationSetting.logWrap.DebugLog(logMessage: logMessage, logNote: logNote);
                }

                throw;
            }
        }

        public  KeyValuePair<String, String> rnStatusToServerStatus(int rnStatusID){
            return ServiceRequest._provider.rnStatusToServerStatus(rnStatusID);
        }
        public  KeyValuePair<String, String> rnSeverityToServerSeverity(int rnSeverityID)
        {
            return ServiceRequest._provider.rnSeverityToServerSeverity(rnSeverityID);
        }
        public  KeyValuePair<String, String> rnRequestTypeToServerRequestType(int rnRequestTypeID)
        {
            return ServiceRequest._provider.rnRequestTypeToServerRequestType(rnRequestTypeID);
        }
    }
}
