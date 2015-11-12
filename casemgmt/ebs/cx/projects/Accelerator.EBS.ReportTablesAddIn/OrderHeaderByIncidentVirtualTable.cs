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
 *  SHA1: $Id: 9cf106b30de4209c61f4eab193c3e5fd0285072c $
 * *********************************************************************************************
 *  File: OrderHeaderByIncidentVirtualTable.cs
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
using Accelerator.EBS.SharedServices.ProxyClasses.OrdersByContact;
using System.Reflection;

namespace Accelerator.EBS.ReportTablesAddin
{
    class OrderHeaderByIncidentVirtualTable : ReportTable
    {
        public OrderHeaderByIncidentVirtualTable(EBSVirtualReportTablesPackage package)
            : base(package)
        {
            this.Name = "OrdersByIncidentVirtualTbl";
            this.Label = "EBS Order Header By Incident Table";
            this.Description = "EBS Order Header By Incident Table";

            // from generated proxy
            APPSOE_ORDER_CUST_ORX3349377X1X3 orderHeader = new APPSOE_ORDER_CUST_ORX3349377X1X3();
            addColumns(orderHeader.GetType());       
        }

        public override IList<IReportRow> GetRows(IList<string> columns, IReportFilterNode filterNode)
        {
            IList<IReportRow> reportRows = new List<IReportRow>();

            if (((EBSVirtualReportTablesPackage)this.Parent)._globalContext == null)
                return reportRows;

            IRecordContext _context = ((EBSVirtualReportTablesPackage)this.Parent)._globalContext.AutomationContext.CurrentWorkspace;
            int incidentID = 0;

            if (_context == null)
            {
                /* cannot create filter based on the Rnow incidentid because this column is not in the payload response
                 * so, cannot run the report in Report Explorer
                 */
                if (filterNode != null && filterNode.FilterNodes != null)
                {
                    IReportFilterNode headerIdFilterNode = filterNode.FilterNodes.ToList<IReportFilterNode>().Find(
                        fn => fn.ReportFilter.Expression == string.Format("{0}${1}.ATTRIBUTE15", this.Parent.Name, this.Name));

                    if (headerIdFilterNode != null)
                    {
                        incidentID = Convert.ToInt32(headerIdFilterNode.ReportFilter.Value);

                    }
                 }
            }
            else
            {
                IIncident incidentRecord = _context.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Incident) as IIncident;
                incidentID = incidentRecord.ID;
            }

            OutputParameters1 op = null;
           // incidentID = 123456;
            op = Order.LookupOrdersByIncident(incidentID, incidentID, 0);

            foreach (APPSOE_ORDER_CUST_ORX3349377X2X3 order in op.X_ORDERS)
            {
                ReportDataRow reportDataRow = new ReportDataRow(this.Columns.Count);
                foreach (string column in columns)
                {
                    ReportDataCell reportDataCell = new ReportDataCell();
                    // put the "SRData$SRDetailTable." package name back
                    string pkgNtblName = "SRData$" + this.Name + ".";
                    string removePkgTblName = column.Replace(pkgNtblName, "");

                    string type = order.GetType().GetProperty(removePkgTblName).PropertyType.Name;

                    object propVal = order.GetType().GetProperty(removePkgTblName).GetValue(order, null);

                    if (propVal == null)
                        reportDataCell.GenericValue = null;
                    else if (type == "Nullable`1" &&
                       order.GetType().GetProperty(removePkgTblName).PropertyType.GetGenericArguments()[0].ToString() == "System.Decimal")
                        reportDataCell.GenericValue = Convert.ToInt32(propVal); // need to convert, otherwise show up as 0
                    else
                        reportDataCell.GenericValue = propVal;

                    reportDataRow.Cells.Add(reportDataCell);
                }
                reportRows.Add(reportDataRow);
            }
            return reportRows;
        }
    }
}
