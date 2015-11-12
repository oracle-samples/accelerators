/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + EBS Enhancement
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  EBS release: 12.1.3
 *  reference: 150505-000099, 150420-000127
 *  date: Thu Nov 12 00:52:48 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: a7edaf6260d3ba46eb34812269ee0efab2ecdb63 $
 * *********************************************************************************************
 *  File: RepairOrder.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using Accelerator.EBS.SharedServices.Providers;
using RightNow.AddIns.AddInViews;

namespace Accelerator.EBS.SharedServices
{
    public class RepairOrder : ModelObjectBase, IReport
    {
        public static string CreateURL { get; set; }
        public static string UpdateURL { get; set; }
        public static string LookupURL { get; set; }
        public static string ListLookupURL { get; set; }
        public static string ListURL { get; set; }

        public string RepairNumber { get; set; }
        public decimal? RepairLineID { get; set; }

        public string ErrorMessage { get; set; }

        public decimal? ServiceRequestID { get; set; }
        public decimal? InventoryItemID { get; set; }
        public string UnitOfMeasure { get; set; }
        public string RepairType { get; set; }
        public decimal? RepairTypeID { get; set; }
        public decimal? Quantity { get; set; }
        public string Currency { get; set; }
        public string ProblemDescription { get; set; }
        public decimal? ResourceID { get; set; }
        public string ApprovalRequired { get; set; }
        public string ApprovalStatus { get; set; }
        public string RepairOrg { get; set; }
        public string RepairOwner { get; set; }

        public DateTime? ResolveByDate { get; set; }
        public DateTime? PromiseDate { get; set; }

        public string RepairStatus { get; set; }
        public string StoredRepairStatus { get; set; }
        public int RepairStatusID { get; set; }
        public int StoredRepairStatusID { get; set; }
        public string SerialNumber { get; set; }
        public string Product { get; set; }
        public string ProdDescription { get; set; }
        public string HasValidSerialNumber { get; set; }

        private decimal IncidentID { get; set; }
        private decimal EBSContactID { get; set; }
        private Dictionary<string, object> Details { get; set; }
        private static Dictionary<string, ReportColumnType> schema { get; set; }


        public RepairOrder() : base()
        {
            // intentionally left blank
        }


        public RepairOrder(Dictionary<string, object> details, decimal ebs_contact_id, decimal incident_id)
            : base()
        {
            this.Details = details;
            this.IncidentID = incident_id;
            this.EBSContactID = ebs_contact_id;
            this.ApprovalRequired = Convert.ToString(details["APPROVAL_REQUIRED_FLAG"]);
            this.RepairNumber = Convert.ToString(details["REPAIR_NUMBER"]);
            this.RepairLineID = Convert.ToDecimal(details["REPAIR_LINE_ID"]);
            this.ServiceRequestID = Convert.ToDecimal(details["INCIDENT_ID"]);
            this.InventoryItemID = Convert.ToDecimal(details["INVENTORY_ITEM_ID"]);
            this.UnitOfMeasure = Convert.ToString(details["UNIT_OF_MEASURE"]);
            this.RepairTypeID = Convert.ToDecimal(details["REPAIR_TYPE_ID"]);
            this.Quantity = Convert.ToDecimal(details["QUANTITY"]);
            this.Currency = Convert.ToString(details["CURRENCY_CODE"]);
            this.ProblemDescription = Convert.ToString(details["PROBLEM_DESCRIPTION"]);
            this.ResourceID = Convert.ToDecimal(details["RESOURCE_ID"]);
            this.ApprovalStatus = "";
            this.RepairOrg = "";
            this.RepairOwner ="";
            this.ResolveByDate = Convert.ToDateTime(details["RESOLVE_BY_DATE"]);
            this.PromiseDate = Convert.ToDateTime(details["PROMISE_DATE"]);
            this.RepairStatus = Convert.ToString(details["FLOW_STATUS"]);
            this.StoredRepairStatus = Convert.ToString(details["FLOW_STATUS_CODE"]);
            this.RepairStatusID = Convert.ToInt32(details["FLOW_STATUS_ID"]);
            this.StoredRepairStatusID = Convert.ToInt32(details["FLOW_STATUS_ID"]);
            this.RepairType = Convert.ToString(details["REPAIR_TYPE_REF"]);
            this.SerialNumber = Convert.ToString(details["SERIAL_NUMBER"]);
            this.Product = Convert.ToString(details["INVENTORY_ITEM_NAME"]);
            this.ProdDescription = Convert.ToString(details["INVENTORY_ITEM_DESC"]);
            this.HasValidSerialNumber = Convert.ToString(details["ATTRIBUTE15"]);
        }

        private static IEBSProvider _provider = null;

        public string Create(int _logIncidentId = 0, int _logContactId = 0)
        {
            if (_provider == null)
            {
                throw new Exception("EBS Provider not initialized.");
            }

            RepairOrder ro = RepairOrder._provider.CreateRO(this, _logIncidentId, _logContactId);
            this.RepairNumber = ro.RepairNumber;
            this.ErrorMessage = ro.ErrorMessage;

            if (!String.IsNullOrWhiteSpace(this.ErrorMessage))
            {
                return null;
            }
            else
            {
                return this.RepairNumber;
            }
        }

        public string Update(int _logIncidentId = 0, int _logContactId = 0)
        {
            if (_provider == null)
            {
                throw new Exception("EBS Provider not initialized.");
            }

            RepairOrder ro = RepairOrder._provider.UpdateRO(this, _logIncidentId, _logContactId);
            this.RepairNumber = ro.RepairNumber;
            this.ErrorMessage = ro.ErrorMessage;

            if (!String.IsNullOrWhiteSpace(this.ErrorMessage))
            {
                if (this.RepairNumber == "-1")
                {
                    return this.RepairNumber;
                }
                else
                {
                    return null;
                } 
            }
            else
            {
                return this.RepairNumber;
            }
        }

        public RepairOrder Lookup(string repair_number, int _logIncidentId = 0, int _logContactId = 0)
        {
            if (_provider == null)
            {
                throw new Exception("EBS Provider not initialized.");
            }

            RepairOrder[] list = RepairOrder._provider.LookupRepairOrderList(0, 0, null, repair_number,  _logIncidentId, _logContactId);
            if (list.Length > 0)
            {
                return list[0];
            }
            return null;
        }

        public RepairOrder[] LookupList(string sr_id, string sr_num, int _logIncidentId = 0, int _logContactId = 0)
        {
            if (_provider == null)
            {
                throw new Exception("EBS Provider not initialized.");
            }

            return RepairOrder._provider.LookupRepairOrderList(0, 0, sr_num, null, _logIncidentId, _logContactId);
        }

        public static RepairOrder[] LookupRepairOrderList(decimal contact_id, decimal incident_id, int rntIncidentId, int rntContactId)
        {
            return RepairOrder._provider.LookupRepairOrderList(contact_id, incident_id, null, null, rntIncidentId, rntContactId);
        }

        public static void InitEBSProvider()
        {
            Type t = Type.GetType(ServiceProvider);

            try
            {
                _provider = Activator.CreateInstance(t) as IEBSProvider;
                _provider.InitForRO(ListURL,  CreateURL, UpdateURL, ServiceUsername, ServicePassword, ServiceClientTimeout);
                _provider.log = ConfigurationSetting.logWrap;
            }
            catch (Exception ex)
            {
                if (ConfigurationSetting.logWrap != null)
                {
                    string logMessage = "Error in init Provider in Repair Order Model. Error: " + ex.Message;
                    string logNote = "";
                    ConfigurationSetting.logWrap.ErrorLog(logMessage: logMessage, logNote: logNote);
                }
                throw;
            }
        }

        public Tuple<ReportColumnType, object> getVirtualColumnValue(string name)
        {
            if (name == "SR_DETAIL_LINK")
                return new Tuple<ReportColumnType, object>(ReportColumnType.String, this.ServiceRequestID + "_");

            return new Tuple<ReportColumnType, object>(RepairOrder.schema[name], this.Details[name]);
        }

        public static Dictionary<string, ReportColumnType> getRepairOrderListSchema()
        {
            if (null == RepairOrder.schema)
                RepairOrder.schema = RepairOrder._provider.getRepairOrderListSchema();

            return RepairOrder.schema;
        }

        public bool ValidateSerialNumber(decimal contactOrgId, int _RntIncidentId, int _RntContactId)
        {
            if (0 == contactOrgId || string.IsNullOrWhiteSpace(SerialNumber)) return false;
            Item[] items = Item.LookupItemList(SerialNumber, contactOrgId, "T", _RntIncidentId, _RntContactId);
            if (items.Length > 0)
            {
                HasValidSerialNumber = "Y";
                InventoryItemID = items[0].InventoryItemID;
                return true;
            }
            return false;
        }
    }
}
