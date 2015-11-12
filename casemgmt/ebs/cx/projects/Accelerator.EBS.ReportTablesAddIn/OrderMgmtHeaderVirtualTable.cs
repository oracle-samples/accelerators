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
 *  SHA1: $Id: 9f101f82b010f29ee5a211191f1b9a8f589b9b70 $
 * *********************************************************************************************
 *  File: OrderMgmtHeaderVirtualTable.cs
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
using System.Text.RegularExpressions;
using Accelerator.EBS.SharedServices.ProxyClasses.OrderMgmt;
using System.Reflection;

namespace Accelerator.EBS.ReportTablesAddin
{
    class OrderMgmtHeaderVirtualTable : ReportTable
    {
        public static OutputParameters2 _orderCompleteOutput;

        public OrderMgmtHeaderVirtualTable(EBSVirtualReportTablesPackage package, ref IList<IReportTable> reportTables)
            : base(package)
        {
            this.Name = "OrderMgmtHeaderValTable";
            this.Label = "EBS Order Mgmt Header Value Table";
            this.Description = "EBS Order Mgmt Header Value Table";

            // from generated proxy HEADER_VAL_REC
            APPSOE_ORDER_PUB_HX219471X29X254 orderHeader = new APPSOE_ORDER_PUB_HX219471X29X254();
            addColumns(orderHeader.GetType());

            //HEADER_VAL_REC has no ID, add one for report linking
            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.Integer,
                Label = "ID",
                Name = "ID",
                CanDisplay = true,
                CanFilter = true
            });

            OutputParameters2 order = new OutputParameters2();
            // add the order child tables         
            foreach (PropertyInfo propertyInfo in order.GetType().GetProperties())
            {
                if (propertyInfo.PropertyType.IsArray)
                {
                    OrderMgmtGenericChildTable newTable = new OrderMgmtGenericChildTable(package, propertyInfo);
                    reportTables.Add(newTable);
                }
            }
        }

        public override IList<IReportRow> GetRows(IList<string> columns, IReportFilterNode filterNode)
        {
            IList<IReportRow> reportRows = new List<IReportRow>();

            if (((EBSVirtualReportTablesPackage)this.Parent)._globalContext == null)
                return reportRows;

            IRecordContext _context = ((EBSVirtualReportTablesPackage)this.Parent)._globalContext.AutomationContext.CurrentWorkspace;

            int orderID = 0, incidentID = 0;

            if (_context == null)
            {
                // filter is ID
                if (filterNode != null && filterNode.FilterNodes != null)
                {
                    IReportFilterNode IDFilterNode = filterNode.FilterNodes.ToList<IReportFilterNode>().Find(fn => fn.ReportFilter.Expression == string.Format("{0}${1}.ID", this.Parent.Name, this.Name));

                    if (IDFilterNode != null)
                        orderID = Convert.ToInt32(IDFilterNode.ReportFilter.Value);
                }

            }
            else
            {
                IIncident incidentRecord = _context.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Incident) as IIncident;
                orderID = getEbsOrderIdCustomAttr(incidentRecord);               
                incidentID = incidentRecord.ID;
            }

            if (orderID == 0)
                return reportRows;

            OutputParameters2 op = null;

            op = Order.LookupOrder(orderID, incidentID, 0);
            _orderCompleteOutput = op;
            ReportDataRow reportDataRow = new ReportDataRow(this.Columns.Count);
            foreach (string column in columns)
            {
                ReportDataCell reportDataCell = new ReportDataCell();
                // put the "SRData$SRDetailTable." package name back
                string pkgNtblName = "SRData$" + this.Name + ".";
                string removePkgTblName = column.Replace(pkgNtblName, "");

                if (removePkgTblName == "ID")
                {
                    reportDataCell.GenericValue = orderID;
                }
                else
                {
                    string type = op.X_HEADER_VAL_REC.GetType().GetProperty(removePkgTblName).PropertyType.Name;
                    object propVal = op.X_HEADER_VAL_REC.GetType().GetProperty(removePkgTblName).GetValue(op.X_HEADER_VAL_REC, null);

                    if (propVal == null)
                        reportDataCell.GenericValue = null;
                    else if (type == "Nullable`1" &&
                       op.X_HEADER_VAL_REC.GetType().GetProperty(removePkgTblName).PropertyType.GetGenericArguments()[0].ToString() == "System.Decimal")
                        reportDataCell.GenericValue = Convert.ToInt32(propVal); // need to convert, otherwise show up as 0
                    else
                        reportDataCell.GenericValue = propVal;
                }

                reportDataRow.Cells.Add(reportDataCell);
            }
            reportRows.Add(reportDataRow);
            return reportRows;
        }
    }
}
