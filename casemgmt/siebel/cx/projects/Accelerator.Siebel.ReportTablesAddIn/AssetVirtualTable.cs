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
 *  SHA1: $Id: e8cf3b1eaf35811cf2655dabf31d68c0318ae0f9 $
 * *********************************************************************************************
 *  File: AssetVirtualTable.cs
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
    class AssetVirtualTable : ReportTable
    {
         //  Build the columns schema dynamically 
        public AssetVirtualTable(SiebelVirtualReportTablesPackage package)
            : base(package)
        {
            this.Name = "AssetTable";
            this.Label = "Siebel Asset Table";
            this.Description = "Siebel Asset Table";
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
            Dictionary<string, string> dictDetail = null;

            if (_context == null)
                return reportRows;

            IIncident incidentRecord = _context.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Incident) as IIncident;

            if (incidentRecord == null)
                return reportRows;

            string serialNum= null;
            // get the serialNum custom attribute on the incident workspace
            serialNum = getIncidentWSCustomAttr(incidentRecord, "siebel_serial_number");

            if (serialNum == "" || serialNum == null)
                return reportRows;

            string logMessage = "Calling LookupAsset for Asset." +
                "serialNum: " + serialNum;
            ConfigurationSetting.logWrap.DebugLog(incidentRecord.ID, 0, logMessage: logMessage);

            string orgId = null;
            IContact contactRecord = _context.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Contact) as IContact;
           
            if (contactRecord != null)
            {
                orgId = getContactOrgIdCustomAttr(contactRecord);
            }

            if (orgId == "" || orgId == null)
                return reportRows;

            // call Siebel Asset.LookupAsset, which return <columnName, type+TYPE_VALUE_DELIMITER+value)         
            dictDetail = Asset.LookupAsset(columns, serialNum, orgId, 0, incidentRecord.ID);
            if (dictDetail == null)
                return reportRows;

            ReportDataRow reportDataRow = new ReportDataRow(this.Columns.Count);
            addDetailRow(dictDetail, ref columns, ref reportDataRow, ref reportRows);

            return reportRows;
        }
    }
}
