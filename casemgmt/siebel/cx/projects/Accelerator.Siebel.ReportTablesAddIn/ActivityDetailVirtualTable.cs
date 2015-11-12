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
 *  date: Thu Nov 12 00:55:33 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: b119a2c172cbe6b1139beabba1b7d7748d227fa3 $
 * *********************************************************************************************
 *  File: ActivityDetailVirtualTable.cs
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
    class ActivityDetailVirtualTable : ReportTable
    {
        //  Build the columns schema dynamically 
        public ActivityDetailVirtualTable(SiebelVirtualReportTablesPackage package)
            : base(package)
        {
            this.Name = "ActivityDetailTable";
            this.Label = "Siebel Activity Detail Table";
            this.Description = "Siebel Activity Detail Table";
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
            Dictionary<string, string> dictDetail = null;

            if (_context == null)
                return reportRows;

            String actvtyId = "";
            string filterValue = null;
            // filter is ActivityId
            if (filterNode != null && filterNode.FilterNodes != null)
            {
                IReportFilterNode actvtyIDFilterNode = filterNode.FilterNodes.ToList<IReportFilterNode>().Find(fn => fn.ReportFilter.Expression == string.Format("{0}${1}.ActivityId", this.Parent.Name, this.Name));
                if (actvtyIDFilterNode != null)
                    actvtyId = actvtyIDFilterNode.ReportFilter.Value;
            }

            IIncident incidentRecord = _context.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Incident) as IIncident;
            string siebelSrId = null;
            if (incidentRecord != null)
            {
                siebelSrId = getIncidentWSCustomAttr(incidentRecord, "siebel_sr_id");
            }

            if (siebelSrId == "" || siebelSrId == null)
                return reportRows;

            string logMessage = "Calling LookupActivityDetail for a Siebel SR." +
                "siebelSrId: " + siebelSrId + ", actvtyId: " + actvtyId;
            ConfigurationSetting.logWrap.DebugLog(incidentRecord.ID, 0, logMessage: logMessage);

            // call Siebel Activity.LookupActivityDetail, which return <columnName, type+TYPE_VALUE_DELIMITER+value)         
            dictDetail = Activity.LookupActivityDetail(columns,siebelSrId, actvtyId, incidentRecord.ID, 0);
            if (dictDetail == null)
                return reportRows;

            ReportDataRow reportDataRow = new ReportDataRow(this.Columns.Count);
            addDetailRow(dictDetail, ref columns, ref reportDataRow, ref reportRows);
  
            return reportRows;
        }
    }
}
