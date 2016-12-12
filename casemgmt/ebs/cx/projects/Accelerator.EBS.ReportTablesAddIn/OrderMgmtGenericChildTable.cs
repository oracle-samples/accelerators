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
 *  date: Thu Nov 12 00:52:45 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: f68f0ea39aa27dc6a90da138006dea1e46e99402 $
 * *********************************************************************************************
 *  File: OrderMgmtGenericChildTable.cs
 * *********************************************************************************************/

using Accelerator.EBS.SharedServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RightNow.AddIns.AddInViews;
using RightNow.AddIns.Common;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Reflection;
using Accelerator.EBS.SharedServices.ProxyClasses.OrderMgmt;

namespace Accelerator.EBS.ReportTablesAddin
{
    class OrderMgmtGenericChildTable : ReportTable
    {
        public OrderMgmtGenericChildTable(EBSVirtualReportTablesPackage package, PropertyInfo tableInfo)
            : base(package)
        {
            this.Name = tableInfo.Name;
            this.Label = "EBS " + tableInfo.Name;
            this.Description = "EBS " + tableInfo.Name;

            addColumns(tableInfo.PropertyType.GetElementType());

            /* _VAL_TBL has no HEADER_ID, add it 
             * this filter for report linking
             */
            if (this.Name.Contains("_VAL_TBL"))
            {
                this.Columns.Add(new ReportColumn()
                {
                    DataType = ReportColumnType.Integer,
                    Label = "HEADER_ID",
                    Name = "HEADER_ID",
                    CanDisplay = true,
                    CanFilter = true
                });
            }
        }

        public override IList<IReportRow> GetRows(IList<string> columns, IReportFilterNode filterNode)
        {
            IList<IReportRow> reportRows = new List<IReportRow>();
            OutputParameters2 op = OrderMgmtHeaderVirtualTable._orderCompleteOutput;

            /* get the table name based on the filter
             * filter is "{0}${1}.HEADER_ID"
             * {1} == this.Name (the child table name)
             */
            string headerId = null;
            if (filterNode != null && filterNode.FilterNodes != null)
            {
                IReportFilterNode headerIdFilterNode = filterNode.FilterNodes.ToList<IReportFilterNode>().Find(
                    fn => fn.ReportFilter.Expression == string.Format("{0}${1}.HEADER_ID", this.Parent.Name, this.Name));

                if (headerIdFilterNode != null)
                {
                    headerId = headerIdFilterNode.ReportFilter.Value;

                }
            }
            if (headerId == "" || headerId == null)
                return reportRows;

            int headerIdInt = Convert.ToInt32(headerId);
            // call the ws if the header is not the same
            if (op == null || (op != null && op.X_HEADER_REC.HEADER_ID != headerIdInt))
                op = Order.LookupOrder(headerIdInt, 0, 0);
        
            // get the table array
            object[] obj = (object[])op.GetType().GetProperty(this.Name).GetValue(op, null);
            foreach (object orderChild in obj)
            {
                ReportDataRow reportDataRow = new ReportDataRow(this.Columns.Count);
                foreach (string column in columns)
                {
                    ReportDataCell reportDataCell = new ReportDataCell();
                    // put the "SRData$SRDetailTable." package name back, that's required by the report framework
                    string pkgNtblName = "SRData$" + this.Name + ".";
                    string removePkgTblName = column.Replace(pkgNtblName, "");

                    if (removePkgTblName == "HEADER_ID")
                    {
                        reportDataCell.GenericValue = headerIdInt;
                    }
                    else
                    {
                        string type = orderChild.GetType().GetProperty(removePkgTblName).PropertyType.Name;
                        object propVal = orderChild.GetType().GetProperty(removePkgTblName).GetValue(orderChild, null);

                        if (propVal == null)
                            reportDataCell.GenericValue = null;
                        else if (type == "Nullable`1" &&
                            orderChild.GetType().GetProperty(removePkgTblName).PropertyType.GetGenericArguments()[0].ToString() == "System.Decimal")
                                reportDataCell.GenericValue = Convert.ToInt32(propVal); // need to convert, otherwise show up as 0
                        else
                            reportDataCell.GenericValue = propVal;
                    }
                    reportDataRow.Cells.Add(reportDataCell);
                }
                reportRows.Add(reportDataRow);
            }
            return reportRows;
        }
    }
}
