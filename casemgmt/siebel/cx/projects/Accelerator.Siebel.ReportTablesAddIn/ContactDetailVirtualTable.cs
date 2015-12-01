/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC Contact Center + Siebel Case Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  Siebel release: 8.1.1.15
 *  reference: 150520-000047
 *  date: Mon Nov 30 20:14:27 PST 2015

 *  revision: rnw-15-11-fixes-release-2
 *  SHA1: $Id: e7e69710ff7c241da7c0efae7baecd5a5921b0a2 $
 * *********************************************************************************************
 *  File: ContactDetailVirtualTable.cs
 * *********************************************************************************************/

using Accelerator.Siebel.SharedServices;
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

namespace Accelerator.Siebel.ReportTablesAddin
{
    class ContactDetailVirtualTable : ReportTable
    {
        //  Build the columns schema dynamically based on Contact.getDetailSchema()
        public ContactDetailVirtualTable(SiebelVirtualReportTablesPackage package)
            : base(package)
        {
            this.Name = "ContactDetailTable";
            this.Label = "Siebel Contact Detail Table";
            this.Description = "Siebel Contact Detail Table";
            Dictionary<string, string> dictDetail = ContactModel.getDetailSchema();

            addColumns(dictDetail);       
        }

        /*  For showing the Siebel Contact Details.
         *  It can also be run in Report Explorer by creating a required filter on PERSON_PARTY_ID
         */
        public override IList<IReportRow> GetRows(IList<string> columns, IReportFilterNode filterNode)
        {
            IList<IReportRow> reportRows = new List<IReportRow>();
            if (((SiebelVirtualReportTablesPackage)this.Parent)._globalContext == null)
                return reportRows;

            IRecordContext _context = ((SiebelVirtualReportTablesPackage)this.Parent)._globalContext.AutomationContext.CurrentWorkspace;
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

                dictDetail = ContactModel.LookupDetail(columns, partyID);
            }
            else
            {
                IContact contactRecord = _context.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Contact) as IContact;
                string contactPartyID = "";

                contactPartyID = getContactPartyIdCustomAttr(contactRecord);

                if (contactPartyID == "")
                    return reportRows;

                string logMessage = "Calling LookupDetail for Contact." +
                    "contactPartyID: " + contactPartyID;
                ConfigurationSetting.logWrap.DebugLog(0, contactRecord.ID, logMessage: logMessage);

                // call Siebel Contact.LookupDetail, which return <columnName, type+TYPE_VALUE_DELIMITER+value)
                dictDetail = ContactModel.LookupDetail(columns, contactPartyID, 0, contactRecord.ID);               
            }
            
            ReportDataRow reportDataRow = new ReportDataRow(this.Columns.Count);
            addDetailRow(dictDetail, ref columns, ref reportDataRow, ref reportRows);

            return reportRows;
        }
    }
}
