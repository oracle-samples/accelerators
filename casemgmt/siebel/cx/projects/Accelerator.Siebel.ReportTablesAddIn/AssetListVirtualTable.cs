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
 *  SHA1: $Id: 3aeca7ced2166c6272e5213910700ffc773e813b $
 * *********************************************************************************************
 *  File: AssetListVirtualTable.cs
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
    class AssetListVirtualTable : ReportTable
    {
        //  Build the columns schema dynamically 
        public AssetListVirtualTable(SiebelVirtualReportTablesPackage package)
            : base(package)
        {
            this.Name = "AssetListTable";
            this.Label = "Siebel Asset List Table";
            this.Description = "Siebel Asset List Table";
            Dictionary<string, string> dictDetail = Asset.getAssetSchema();

            addColumns(dictDetail);
        }

        /*  For showing the Siebel Contact Details.
         *  It can also be run in Report Explorer by creating a required filter on PERSON_PARTY_ID
         */
        public override IList<IReportRow> GetRows(IList<string> columns, IReportFilterNode filterNode)
        {
            IList<IReportRow> reportRows = new List<IReportRow>();
            IRecordContext _context = ((SiebelVirtualReportTablesPackage)this.Parent)._globalContext.AutomationContext.CurrentWorkspace;
            List<Dictionary<string, string>> dictDetailList = null;

            if (_context == null)
                return reportRows;

            IContact contactRecord = _context.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Contact) as IContact;
            string siebelContactId = null;
            if (contactRecord != null)
            {
                siebelContactId = getContactPartyIdCustomAttr(contactRecord);
            }

            if (siebelContactId == "" || siebelContactId == null)
                return reportRows;

            string logMessage = "Calling LookupAssetList for a Siebel Contact." +
                "siebelContactId: " + siebelContactId;
            ConfigurationSetting.logWrap.DebugLog(0, contactRecord.ID, logMessage: logMessage);
         
            // call Siebel Asset.LookupAssetList, which return <columnName, type+TYPE_VALUE_DELIMITER+value)         
            dictDetailList = Asset.LookupAssetList(columns, siebelContactId, 0, contactRecord.ID);
            if (dictDetailList == null)
                return reportRows;

            foreach (Dictionary<string, string> dictDetail in dictDetailList)
            {
                ReportDataRow reportDataRow = new ReportDataRow(this.Columns.Count);
                addDetailRow(dictDetail, ref columns, ref reportDataRow, ref reportRows);
            }
            return reportRows;
        }
    }
}
