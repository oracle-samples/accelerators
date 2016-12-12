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
 *  date: Thu Nov 12 00:52:48 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: 772142ae6a2e551c848a8550773d822de4f26b00 $
 * *********************************************************************************************
 *  File: Item.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using Accelerator.EBS.SharedServices.Providers;
using RightNow.AddIns.AddInViews;

namespace Accelerator.EBS.SharedServices
{
    public class Item : ModelObjectBase, IReport
    {
        public static string ListURL { get; set; }
        private static IEBSProvider _provider = null;
        private static Dictionary<string, ReportColumnType> schema { get; set; }
        private Dictionary<string, object> Details { get; set; }

        public decimal? InventoryItemID { get; set; }
        public decimal? InstanceID { get; set; }
        public string InstanceNumber { get; set; }
        public string SerialNumber { get; set; }
        public string InstanceStatus { get; set; }
        public string InventoryItemName { get; set; }

        private string HiddenSerialNumber { get; set; }
        private decimal HiddenContactOrgID { get; set; }
        private string HiddenActiveInstanceOnly { get; set; }

        public Item() : base()
        {
            // intentionally left blank
        }

        public Item(Dictionary<string, object> details, string serial_number, decimal contact_org_id, string active_instance_only)
            : base()
        {
            this.Details = details;
            this.HiddenSerialNumber = serial_number;
            this.HiddenContactOrgID = contact_org_id;
            this.HiddenActiveInstanceOnly = active_instance_only;
            this.InstanceID = Convert.ToDecimal(details["INSTANCE_ID"]);
            this.InstanceNumber = Convert.ToString(details["INSTANCE_NUMBER"]);
            this.InstanceStatus = Convert.ToString(details["INSTANCE_STATUS"]);
            this.InventoryItemID = Convert.ToDecimal(details["INVENTORY_ITEM_ID"]);
            this.InventoryItemName = Convert.ToString(details["INVENTORY_ITEM_NAME"]);
            this.SerialNumber = Convert.ToString(details["SERIAL_NUMBER"]);
        }

        public static Item[] LookupItemList(string serial_number, decimal contact_org_id, string active_instance_only,
            int rntIncidentId = 0, int rntContactId = 0)
        {
            Item[] itemArr = null;
            try
            {
                    //Switch Provider to call web service
                itemArr = Item._provider.LookupItemList(serial_number, contact_org_id, active_instance_only, rntIncidentId, rntContactId);
            }
            catch (Exception)
            {
                throw;
            }
            return itemArr;
        }

        public Tuple<ReportColumnType, object> getVirtualColumnValue(string name)
        {
            return new Tuple<ReportColumnType, object>(Item.schema[name], this.Details[name]);
        }

        public static Dictionary<string, ReportColumnType> getDetailedSchema()
        {
            if (null == Item.schema)
                Item.schema = Item._provider.getItemSchema();

            return Item.schema;
        }

        public static void InitEBSProvider()
        {
            Type t = Type.GetType(ServiceProvider);

            try
            {
                _provider = Activator.CreateInstance(t) as IEBSProvider;
                _provider.InitForItem(ListURL, ServiceUsername, ServicePassword, ServiceClientTimeout);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
