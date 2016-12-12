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
 *  SHA1: $Id: 2edd9fb61594c8a1ed9adbfd14545b92b8ef998f $
 * *********************************************************************************************
 *  File: RepairLogistics.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using Accelerator.EBS.SharedServices.Providers;
using RightNow.AddIns.AddInViews;

namespace Accelerator.EBS.SharedServices
{
    public class RepairLogistics : ModelObjectBase, IReport
    {
        public static string ListURL { get; set; }
        public decimal ProductTransactionID { get; set; }
        public decimal InventoryItemID { get; set; }
        public string InventoryItemName { get; set; }
        public string ActionType { get; set; }
        
        private static IEBSProvider _provider = null;
        private static Dictionary<string, ReportColumnType> schema { get; set; }
        private Dictionary<string, object> Details { get; set; }
        private decimal HiddenRepairOrderID { get; set; }

        public RepairLogistics() : base()
        {
            // intentionally left blank
        }


        public RepairLogistics(Dictionary<string, object> details, decimal repair_order_id)
            : base()
        {
            this.Details = details;
            this.HiddenRepairOrderID = repair_order_id;
            this.ProductTransactionID = Convert.ToDecimal(details["PRODUCT_TRANSACTION_ID"]);
            this.InventoryItemID = Convert.ToDecimal(details["INVENTORY_ITEM_ID"]);
            this.InventoryItemName = Convert.ToString(details["INVENTORY_ITEM_NAME"]);
            this.ActionType = Convert.ToString(details["ACTION_TYPE"]);
        }

        public static RepairLogistics[] LookupRepairLogisticsList(decimal repair_order_id)
        {
            RepairLogistics[] itemArr = null;
            try
            {
                // //Switch Provider to call web service
                itemArr = RepairLogistics._provider.LookupRepairLogisticsList(repair_order_id);
            }
            catch (Exception)
            {
                throw;
            }
            return itemArr;
        }


        public Tuple<ReportColumnType, object> getVirtualColumnValue(string name)
        {
            return new Tuple<ReportColumnType, object>(RepairLogistics.schema[name], this.Details[name]);
        }

        public static Dictionary<string, ReportColumnType> getRepairLogisticsListSchema()
        {
            if (null == RepairLogistics.schema)
                RepairLogistics.schema = RepairLogistics._provider.getRepairLogisticsListSchema();

            return RepairLogistics.schema;
        }

        public static void InitEBSProvider()
        {
            Type t = Type.GetType(ServiceProvider);

            try
            {
                _provider = Activator.CreateInstance(t) as IEBSProvider;
                _provider.InitForRepairLogisticsList(ListURL, ServiceUsername, ServicePassword, ServiceClientTimeout);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
