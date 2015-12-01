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
 *  SHA1: $Id: 75c2f2d8a25b360a85cd9c57c4adcd8709f0266e $
 * *********************************************************************************************
 *  File: SRdetailVirtualTable.cs
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
    class SRdetailVirtualTable : ReportTable
    {
        /*  Build the columns schema dynamically based on ServiceRequest.getDetailSchema()
         *  and add one more column, HiddenSRconcatIncident_ID.
         *  The reason is the Service Request List view has "Subject" as report linking
         *  to this detail table, and it is based on this special column
         */
        public SRdetailVirtualTable(SiebelVirtualReportTablesPackage package)
            : base(package)
        {
            this.Name = "SRDetailTable";
            this.Label = "Siebel SR Detail Table";
            this.Description = "Siebel SR Detail Table";

            Dictionary<string, string> dictSRDetail = ServiceRequest.getDetailSchema();
            addColumns(dictSRDetail);

            // format is either SrID_ OR _IncidentID
            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = "HiddenSRconcatIncident_ID",
                Name = "HiddenSRconcatIncident_ID",
                CanDisplay = false,
                CanFilter = true
            });
        }

        /*  For showing the either Siebel Service Request Detail data (SrID_) or 
         *  open an incident workspace (_IncidentID) based on SrID_ OR _IncidentID
         *  When creating the SR Detail report, specify the filter on HiddenSRconcatIncident_ID
         *  so that the navigation can be determined
         */
        public override IList<IReportRow> GetRows(IList<string> columns, IReportFilterNode filterNode)
        {
            string logMessage;
            IList<IReportRow> reportRows = new List<IReportRow>();
            if (((SiebelVirtualReportTablesPackage)this.Parent)._globalContext == null)
                return reportRows;

            // for report linking, define filter RequestID
            String srID = "";
            string[] hiddenSR_Inc = null;
            string filterValue = null;
            // filter is HiddenSRconcatIncident_ID
            if (filterNode != null && filterNode.FilterNodes != null)
            {
                IReportFilterNode srIDFilterNode = filterNode.FilterNodes.ToList<IReportFilterNode>().Find(fn => fn.ReportFilter.Expression == string.Format("{0}${1}.HiddenSRconcatIncident_ID", this.Parent.Name, this.Name));
                filterValue = srIDFilterNode.ReportFilter.Value;
                if (srIDFilterNode != null)
                {
                    if (!filterValue.Contains("_"))
                    {
                        filterValue = "_" + filterValue;
                    }

                    //srID = srIDFilterNode.ReportFilter.Value;
                    hiddenSR_Inc = filterValue.Split('_');
                    srID = hiddenSR_Inc[0];
                    System.Diagnostics.Debug.WriteLine(srIDFilterNode.ReportFilter.OperatorType.ToString());
                }
            }

            // [1] is incidentID not null, open the incident WS, and return empty row in the detail report
            if (hiddenSR_Inc != null && hiddenSR_Inc[1] != "")
            {
                logMessage = "Opening Incident WS." +
                    "incidentID: " + hiddenSR_Inc[1];

                ConfigurationSetting.logWrap.DebugLog(logMessage: logMessage);

                ((SiebelVirtualReportTablesPackage)this.Parent)._globalContext.AutomationContext.EditWorkspaceRecord(
                    WorkspaceRecordType.Incident,
                    Convert.ToInt32(hiddenSR_Inc[1])
                    );

                return reportRows;
            }

            if (srID == "")
                return reportRows;

            logMessage = "Calling LookupDetail for srID: " + srID; ;
            ConfigurationSetting.logWrap.DebugLog(logMessage: logMessage);

            // call Siebel ServiceRequest.LookupDetail, which return <columnName, type+TYPE_VALUE_DELIMITER+value)
            Dictionary<string, string> dictSRDetail = ServiceRequest.LookupDetail(columns, srID);

            ReportDataRow reportDataRow = new ReportDataRow(this.Columns.Count);
            addDetailRow (dictSRDetail, ref columns, ref reportDataRow, ref reportRows);

            return reportRows;
        }
    }
}

