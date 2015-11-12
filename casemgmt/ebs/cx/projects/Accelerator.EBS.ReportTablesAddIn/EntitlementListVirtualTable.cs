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
 *  SHA1: $Id: a894eca7282a009ed5d10b764167796f61edf9f6 $
 * *********************************************************************************************
 *  File: EntitlementListVirtualTable.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using Accelerator.EBS.SharedServices;
using RightNow.AddIns.AddInViews;

/*   
 *   It show Entitlement list for an item instance.
 */
namespace Accelerator.EBS.ReportTablesAddin
{

    public class EntitlementListVirtualTable : ReportTable
    {
        public EntitlementListVirtualTable(EBSVirtualReportTablesPackage package)
            : base(package)
        {
            this.Name = "EntitlementTable";
            this.Label = "EBS Entitlement Table";
            this.Description = "EBS Entitlement Table";
            addColumns(Entitlement.getDetailedSchema());

            //add filter
            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.Integer,
                Label = "HiddenInstanceId",
                Name = "HiddenInstanceId",
                CanDisplay = false,
                CanFilter = true
            }); 
            
            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = "HiddenValidateFlag",
                Name = "HiddenValidateFlag",
                CanDisplay = false,
                CanFilter = true
            }); 

        }

        public override IList<IReportRow> GetRows(IList<string> columns, IReportFilterNode filterNode)
        {
            decimal hiddenInstanceId = 0;
            object filter_value = getEqualsFilterValue(filterNode, "HiddenInstanceId", true);
            if (null != filter_value)
            {
                hiddenInstanceId = Convert.ToDecimal(filter_value);
            }
            IList<IReportRow> reportRows = new List<IReportRow>();
            Entitlement[] entitlements = Entitlement.LookupEntitlementList(hiddenInstanceId, getValidateFlag(filterNode));
            foreach (Entitlement e in entitlements)
            {
                ReportDataRow reportDataRow = new ReportDataRow(this.Columns.Count);
                if (e != null)
                {   
                    addItem(ref columns, ref reportDataRow, ref reportRows, e);
                }
            }
            return reportRows;
        }
        
        private string getValidateFlag(IReportFilterNode filterNode)
        {
            string flag = "";
            object filter_value = getEqualsFilterValue(filterNode, "HiddenValidateFlag", false);
            if (null != filter_value)
            {
                flag = Convert.ToString(filter_value);
            }            
            if (!String.IsNullOrEmpty(flag))
            {
                return flag;
            }
            return "F";
        }
    }

}

