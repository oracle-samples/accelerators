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
 *  SHA1: $Id: 701afd25bbb3bf36f4737ccc4b00a7a5a7b0b579 $
 * *********************************************************************************************
 *  File: IEBSProvider.cs
 * *********************************************************************************************/

using Accelerator.EBS.SharedServices;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using RightNow.AddIns.AddInViews;
using Accelerator.EBS.SharedServices.ProxyClasses.OrderMgmt;
using ORDERS = Accelerator.EBS.SharedServices.ProxyClasses.OrdersByContact;

namespace Accelerator.EBS.SharedServices.Providers
{
    public interface IEBSProvider
    {
        string SRServiceURL { get; set; }
        string SRLookupURL { get; set; }
        string SRServiceUsername { get; set; }
        string SRServicePassword { get; set; }
        string ROCreateURL { get; set; }
        string ROUpdateURL { get; set; }
        string ROLookupURL { get; set; }
        string ROListLookupURL { get; set; }
        string ROServiceUsername { get; set; }
        string ROServicePassword { get; set; }

        string InteractionURL { get; set; }
        string InteractionUsername { get; set; }
        string InteractionPassword { get; set; }

        string ContactListLookupURL { get; set; }
        string ContactServiceUsername { get; set; }
        string ContactServicePassword { get; set; }
        int ContactServiceTimeout { get; set; } // come from ConfigurationSetting EBSServiceTimeout, and pass in to InitForXx()

        Logs.LogWrapper log { get; set; }
        //int _logIncidentId { get; set; }
        //int _logContactId { get; set; }

        void InitForSR(string service_url, string lookup_url, string user_name, string password, int timeout = -1);
        void InitForRO(string report_url, string create_url, string update_url, string user_name, string password, int timeout = -1);

        void InitForInteraction(string service_url, string user_name, string password, int timeout = -1);
        void InitForContact(string search_url, string user_name, string password, int timeout = -1);

        ServiceRequest CreateSR(ServiceRequest sr, int _logIncidentId = 0, int _logContactId = 0);
        ServiceRequest UpdateSR(ServiceRequest sr, int _logIncidentId = 0, int _logContactId = 0);
        RepairOrder CreateRO(RepairOrder ro, int _logIncidentId = 0, int _logContactId = 0);
        ServiceRequest LookupSR(decimal incident_id, string incident_number, int _logIncidentId = 0, int _logContactId = 0);
        Dictionary<string, string> LookupSRDetail(decimal incident_id, string incident_number, int _logIncidentId = 0, int _logContactId = 0);
        ServiceRequest[] LookupSRbyContactPartyID(decimal contact_id, int _logIncidentId = 0, int _logContactId = 0);
        RepairOrder UpdateRO(RepairOrder ro, int _logIncidentId = 0, int _logContactId = 0);
        Interaction CreateInteraction(Interaction interaction, int _logIncidentId = 0, int _logContactId = 0);
        ContactModel[] LookupContactList(string firstname, string lastname, string phone, string email, int _logIncidentId = 0, int _logContactId = 0);
        Dictionary<string, string> LookupContactDetail(decimal party_id, int _logIncidentId = 0, int _logContactId = 0);
        Dictionary<string, string> getContactDetailSchema();
        Dictionary<string, string> getServiceRequestDetailSchema();
        KeyValuePair<String, String> rnStatusToServerStatus(int rnStatusID);
        KeyValuePair<String, String> rnSeverityToServerSeverity(int rnSeverityID);
        KeyValuePair<String, String> rnRequestTypeToServerRequestType(int rnSeverityID);
        void dictAddProperty(PropertyInfo propertyInfo, Object propVal, ref Dictionary<string, string> dictDetail);
        OutputParameters2 LookupOrderDetail(decimal order_id, int _logIncidentId = 0, int _logContactId = 0);
        ORDERS.OutputParameters LookupOrdersByContact(decimal contact_id, int _logIncidentId = 0, int _logContactId = 0);
        ORDERS.OutputParameters1 LookupOrdersByIncident(decimal incident_id, int _logIncidentId = 0, int _logContactId = 0);

        string ItemServiceUsername { get; set; }
        string ItemServicePassword { get; set; }
        int ItemServiceTimeout { get; set; } // comes from ConfigurationSetting EBSServiceTimeout, and passed to InitForXx()
        string ItemListURL { get; set; }
        void InitForItem(string list_url, string user_name, string password, int timeout = -1);
        Item[] LookupItemList(string serial_number, decimal contact_org_id, string active_instance_only,
            int rntIncidentId = 0, int rntContactId = 0);
        Dictionary<string, ReportColumnType> getItemSchema();

        string EntitlementServiceUsername { get; set; }
        string EntitlementServicePassword { get; set; }
        int EntitlementServiceTimeout { get; set; } // comes from ConfigurationSetting EBSServiceTimeout, and passed to InitForXx()
        string EntitlementListURL { get; set; }
        void InitForEntitlement(string list_url, string user_name, string password, int timeout = -1);
        Entitlement[] LookupEntitlementList(decimal instance_id, string validate_flag);
        Dictionary<string, ReportColumnType> getEntitlementSchema();

        //RepairOrderList
        string RepairOrderListServiceUsername { get; set; }
        string RepairOrderListServicePassword { get; set; }
        int RepairOrderListServiceTimeout { get; set; } // comes from ConfigurationSetting EBSServiceTimeout, and passed to InitForXx()
        string RepairOrderListURL { get; set; }
        RepairOrder[] LookupRepairOrderList(decimal contact_id, decimal incident_id, string incident_number, string repair_number,
            int rntIncidentId = 0, int rntContactId = 0);
        Dictionary<string, ReportColumnType> getRepairOrderListSchema();

        //RepairLogisticsList
        string RepairLogisticsListURL { get; set; }
        string RepairLogisticsListServiceUsername { get; set; }
        string RepairLogisticsListServicePassword { get; set; }
        int RepairLogisticsListServiceTimeout { get; set; }
        void InitForRepairLogisticsList(string list_url, string user_name, string password, int timeout = -1);
        RepairLogistics[] LookupRepairLogisticsList(decimal repair_order_id);
        Dictionary<string, ReportColumnType> getRepairLogisticsListSchema();


        void InitForOrder(string GetOrderURL, string InboundURL);
    }
}
