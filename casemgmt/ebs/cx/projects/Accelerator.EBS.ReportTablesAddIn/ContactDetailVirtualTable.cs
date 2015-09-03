/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + EBS Enhancement
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.5 (May 2015)
 *  EBS release: 12.1.3
 *  reference: 150202-000157
 *  date: Wed Sep  2 23:11:38 PDT 2015

 *  revision: rnw-15-8-fixes-release-01
 *  SHA1: $Id: 538befea3648a94c134ba808a743d26e1b368b8b $
 * *********************************************************************************************
 *  File: ContactDetailVirtualTable.cs
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

namespace Accelerator.EBS.ReportTablesAddin
{
    class ContactDetailVirtualTable : ReportTable
    {
        //  Build the columns schema dynamically based on Contact.getDetailSchema()
        public ContactDetailVirtualTable(EBSVirtualReportTablesPackage package)
            : base(package)
        {
            this.Name = "ContactDetailTable";
            this.Label = "EBS Contact Detail Table";
            this.Description = "EBS Contact Detail Table";
            Dictionary<string, string> dictDetail = Contact.getDetailSchema();

            addColumns(dictDetail);       
        }

        /*  For showing the EBS Contact Details.
         *  It can also be run in Report Explorer by creating a required filter on PERSON_PARTY_ID
         */
        public override IList<IReportRow> GetRows(IList<string> columns, IReportFilterNode filterNode)
        {
            IList<IReportRow> reportRows = new List<IReportRow>();
            if (((EBSVirtualReportTablesPackage)this.Parent)._globalContext == null)
                return reportRows;

            IRecordContext _context = ((EBSVirtualReportTablesPackage)this.Parent)._globalContext.AutomationContext.CurrentWorkspace;
            Dictionary<string, string> dictDetail = null;
            String partyID = "";

            // for running standalone report (from Report Explorer)
            if (_context == null)
            {
                // filter is PERSON_PARTY_ID
                if (filterNode != null && filterNode.FilterNodes != null)
                {
                    IReportFilterNode partyIDFilterNode = filterNode.FilterNodes.ToList<IReportFilterNode>().Find(fn => fn.ReportFilter.Expression == string.Format("{0}${1}.PERSON_PARTY_ID", this.Parent.Name, this.Name));

                    if (partyIDFilterNode != null)
                    {
                        partyID = partyIDFilterNode.ReportFilter.Value;
                        System.Diagnostics.Debug.WriteLine(partyIDFilterNode.ReportFilter.OperatorType.ToString());
                    }

                }
                if (partyID == "" || partyID == null)
                    return reportRows;

                dictDetail = Contact.LookupDetail(Convert.ToInt32(partyID));
            }
            else
            {
                IContact contactRecord = _context.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Contact) as IContact;
                int contactPartyID = 0;

                contactPartyID = getContactPartyIdCustomAttr(contactRecord);

                if (contactPartyID == 0)
                    return reportRows;

                string logMessage = "Calling LookupDetail for Contact." +
                    "contactPartyID: " + contactPartyID;
                ConfigurationSetting.logWrap.DebugLog(0, contactRecord.ID, logMessage: logMessage);

                // call EBS Contact.LookupDetail, which return <columnName, type+TYPE_VALUE_DELIMITER+value)
                dictDetail = Contact.LookupDetail(contactPartyID, 0, contactRecord.ID);
            }
            
            ReportDataRow reportDataRow = new ReportDataRow(this.Columns.Count);
            addDetailRow(ref dictDetail, ref columns, ref reportDataRow, ref reportRows);

            return reportRows;
        }
    }
}
