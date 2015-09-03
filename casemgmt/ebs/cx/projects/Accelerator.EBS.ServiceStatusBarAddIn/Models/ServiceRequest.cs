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
 *  SHA1: $Id: 85f32c8188e77639a25535f3f1a479c5074c3c4a $
 * *********************************************************************************************
 *  File: ServiceRequest.cs
 * *********************************************************************************************/

using Accelerator.EBS.SharedServices.Providers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accelerator.EBS.SharedServices
{
    public class ServiceRequest : ModelObjectBase
    {
        public static string CreateUpdateURL { get; set; }
        public static string LookupURL { get; set; }
        

        public string ErrorMessage { get; set; }

        public decimal? RequestID { get; set; }
        public string RequestNumber { get; set; }

        public DateTime RequestDate { get; set; }
        public DateTime? IncidentOccurredDate { get; set; }
        public string RequestType { get; set; }
        public decimal? RequestTypeID { get; set; }
        public string Status { get; set; }
        public decimal? StatusID { get; set; }
        public string Severity { get; set; }
        public decimal? SeverityID { get; set; }
        public string Summary { get; set; }
        public string Owner { get; set; }
        public string Notes { get; set; }
        public string SerialNumber { get; set; }
        public decimal? ContractID { get; set; }
        public string ContactOrgID { get; set; }

        public string IncidentRef { get; set; }
        public string IncidentID { get; set; }
        public string EbsContactID { get; set; }
        public decimal? SrObjVerNum { get; set; }
        public decimal? ProductID { get; set; }
        public string Product { get; set; }
        public string ProductDescription { get; set; }
        public decimal? OwnerID { get; set; }
        public decimal? CreatedByID { get; set; }

        public string RnowHost { get; set; }

        private static IEBSProvider _provider = null;


        public bool Create(int _logIncidentId = 0, int _logContactId = 0)
        {

            if (_provider == null)
            {
                throw new Exception("EBS Provider not initialized.");
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
                throw new Exception("EBS Provider not initialized.");
            }

            ServiceRequest req = ServiceRequest._provider.UpdateSR(this, _logIncidentId, _logContactId);

            this.RequestID = req.RequestID;
            this.RequestNumber = req.RequestNumber;
            this.ErrorMessage = req.ErrorMessage;

            return String.IsNullOrWhiteSpace(this.ErrorMessage);
        }

        public ServiceRequest Lookup(decimal incident_id, string incident_num, int _logIncidentId = 0, int _logContactId = 0)
        {
            return ServiceRequest._provider.LookupSR(incident_id, incident_num, _logIncidentId,  _logContactId);
        }

        public static Dictionary<string, string> LookupDetail(decimal incident_id, string incident_num, int _logIncidentId = 0, int _logContactId = 0)
        {
            return ServiceRequest._provider.LookupSRDetail(incident_id, incident_num, _logIncidentId, _logContactId);
        }

        public static Dictionary<string, string> getDetailSchema()
        {
            return ServiceRequest._provider.getServiceRequestDetailSchema();
        }

        public static ServiceRequest[] LookupSRbyContactPartyID(decimal contact_id, int _logIncidentId = 0, int _logContactId = 0)
        {
            return ServiceRequest._provider.LookupSRbyContactPartyID(contact_id, _logIncidentId, _logContactId);
        }

        public static void InitEBSProvider()
        {
            Type t = Type.GetType(ServiceProvider);

            try
            {
                _provider = Activator.CreateInstance(t) as IEBSProvider;
                _provider.InitForSR(CreateUpdateURL, LookupURL, ServiceUsername, ServicePassword, ServiceClientTimeout);
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
