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
 *  date: Mon Nov 30 19:59:35 PST 2015

 *  revision: rnw-15-11-fixes-release-2
 *  SHA1: $Id: 8687332a3f070f2a05927853940f5f471014525d $
 * *********************************************************************************************
 *  File: ActivityListVirtualTable.cs
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
    class ActivityListVirtualTable : ReportTable
    {
        //  Build the columns schema dynamically 
        public ActivityListVirtualTable(SiebelVirtualReportTablesPackage package)
            : base(package)
        {
            this.Name = "ActivityListTable";
            this.Label = "Siebel Activity List Table";
            this.Description = "Siebel Activity List Table";
            Dictionary<string, string> dictDetail = Activity.getActivitySchema();

            addColumns(dictDetail);
        }

        /*  For showing the Siebel Activity Details.
         *  It can also be run in Report Explorer by creating a required filter on PERSON_PARTY_ID
         */
        public override IList<IReportRow> GetRows(IList<string> columns, IReportFilterNode filterNode)
        {
            IList<IReportRow> reportRows = new List<IReportRow>();
            IRecordContext _context = ((SiebelVirtualReportTablesPackage)this.Parent)._globalContext.AutomationContext.CurrentWorkspace;
            List<Dictionary<string, string>> dictDetailList = null;

            if (_context == null)
                return reportRows;

            IIncident incidentRecord = _context.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Incident) as IIncident;
            string siebelSrId = null;
            if (incidentRecord != null)
            {
                siebelSrId = getIncidentWSCustomAttr(incidentRecord, "siebel_sr_id");
            }

            if (siebelSrId == "" || siebelSrId == null)
                return reportRows;

            string logMessage = "Calling LookupActivityList for a Siebel SR." +
                "siebelSrId: " + siebelSrId;
            ConfigurationSetting.logWrap.DebugLog(incidentRecord.ID, 0, logMessage: logMessage);

            // call Siebel Activity.LookupActivityList, which return <columnName, type+TYPE_VALUE_DELIMITER+value)         
            dictDetailList = Activity.LookupActivityList(columns, siebelSrId, incidentRecord.ID, 0);
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
