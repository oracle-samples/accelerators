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
 *  date: Thu Nov 12 00:52:45 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: 6bc3416130a10af150973011ff4e7873218bf36f $
 * *********************************************************************************************
 *  File: RepairLogisticsVirtualTable.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using Accelerator.EBS.SharedServices;
using RightNow.AddIns.AddInViews;

/*   
 *   It show Repair Logistics list.
 * 
 */
namespace Accelerator.EBS.ReportTablesAddin 
{

    public class RepairLogisticsVirtualTable : ReportTable
    {
        public RepairLogisticsVirtualTable(EBSVirtualReportTablesPackage package)
            : base(package)
        {
            this.Name = "RepairLogisticsTable";
            this.Label = "EBS Repair Logistics Table";
            this.Description = "EBS Repair Logistics Table";
            addColumns(RepairLogistics.getRepairLogisticsListSchema());

            //add filter
            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.Integer,
                Label = "HiddenRepairOrderID",
                Name = "HiddenRepairOrderID",
                CanDisplay = false,
                CanFilter = true
            });
        }

        public override IList<IReportRow> GetRows(IList<string> columns, IReportFilterNode filterNode)
        {
            decimal hiddenRepairOrderID = 0;
            object filter_value = getEqualsFilterValue(filterNode, "HiddenRepairOrderID", true);
            if (null != filter_value)
            {
                hiddenRepairOrderID = Convert.ToDecimal(filter_value);
            }
            IList<IReportRow> reportRows = new List<IReportRow>();
            if (hiddenRepairOrderID <= 0)
            {
                string logMessage = "Provide a valid EBS Repair Order ID to search for Repair Logistics. Ignoring repair_order_id "
                    + hiddenRepairOrderID;
                string logNote = null;
                ConfigurationSetting.logWrap.DebugLog(logMessage: logMessage, logNote: logNote);
                return reportRows;
            }
            RepairLogistics[] items = RepairLogistics.LookupRepairLogisticsList(hiddenRepairOrderID);

            foreach (RepairLogistics item in items)
            {
                ReportDataRow reportDataRow = new ReportDataRow(this.Columns.Count);
                if (item != null)
                        addItem(ref columns, ref reportDataRow, ref reportRows, item);
            }            
            return reportRows;
        }
    }
}