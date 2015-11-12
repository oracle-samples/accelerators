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
 *  SHA1: $Id: 1186a8e1a17b472bad5ff4c1af67c8b6a013d672 $
 * *********************************************************************************************
 *  File: OrderHeaderByContactVirtualTable.cs
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
    class OrderHeaderByContactVirtualTable : ReportTable
    {
        public OrderHeaderByContactVirtualTable(EBSVirtualReportTablesPackage package)
            : base(package)
        {
            this.Name = "OrdersByContactVirtualTable";
            this.Label = "EBS Order Header By Contact Table";
            this.Description = "EBS Order Header By Contact Table";

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
            int contactID = 0, rnowContactId = 0;

            if (_context == null)
            {
                /* cannot create filter based on the ebs contact party id because this column is not in the payload response
                 * there is SOLD_TO_CONTACT_ID, it does not match the input of the request payload
                 * so, cannot run the report in Report Explorer
                 */ 
                return reportRows;
            }
            else
            {
                IContact contactRecord = _context.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Contact) as IContact;
                contactID = getContactPartyIdCustomAttr(contactRecord);
                rnowContactId = contactRecord.ID;
            }

            if (contactID == 0)
                return reportRows;

            OutputParameters op = null;

            op = Order.LookupOrdersByContact(contactID, 0, rnowContactId);

            foreach (APPSOE_ORDER_CUST_ORX3349377X1X3 order in op.X_ORDERS)
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
