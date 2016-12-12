/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC Contact Center + Siebel Case Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  Siebel release: 8.1.1.15
 *  reference: 150520-000047
 *  date: Mon Nov 30 20:14:27 PST 2015

 *  revision: rnw-15-11-fixes-release-2
 *  SHA1: $Id: 264e5654d0f56caf2ca92878755dd234e647c7da $
 * *********************************************************************************************
 *  File: SRlistVirtualTable.cs
 * *********************************************************************************************/
using ADDIN = RightNow.AddIns;
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
using Accelerator.Siebel.SharedServices.RightNowServiceReference;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections;

/*   class ServiceRequestVirtualTable is for the SR list report
 *   It show SR list based on Contact Workspace custom attribute
 *   Accelerator$Siebel_contact_party_id (Siebel contact).
 *   
 *   The SR list is a combined "incidents by contact" rnow report and
 *   SRs from Siebel
 * 
 *   Add columns and implement GetRows()
 */
namespace Accelerator.Siebel.ReportTablesAddin
{
    public class SRlistVirtualTable : ReportTable
    {
        public SRlistVirtualTable(SiebelVirtualReportTablesPackage package)
            : base(package)
        {
            this.Name = "SRlistTable";
            this.Label = "Siebel Service Request List Table";
            this.Description = "Siebel Service Request List Table";

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.Integer,
                Label = "SR ID",
                Name = "SrID",
                CanDisplay = true,
                CanFilter = true
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = "SR Number",
                Name = "SrNumber",
                CanDisplay = true,
                CanFilter = true
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.Integer,
                Label = "Contact Party ID",
                Name = "ContactPartyID",
                CanDisplay = true,
                CanFilter = true
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = "Incident Reference",
                Name = "IncidentRef",
                CanDisplay = true,
                CanFilter = false
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = "Status",
                Name = "Status",
                CanDisplay = true,
                CanFilter = true
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = "Summary",
                Name = "Summary",
                CanDisplay = true
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.Date,
                Label = "Created",
                Name = "Created",
                CanDisplay = true
            });
            // format is either SrID_ OR _IncidentID
            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = "HiddenSRconcatIncident_ID",
                Name = "HiddenSRconcatIncident_ID",
                CanDisplay = true,
                CanFilter = true
            });
        }

        /*  this method is called from framework to show the report row (data)
         *  refer to QA 140910-000144 for the story
         *  It combines the incident report (ConfigurationSetting.incidentsByContactReportID)
         *  and the ServiceRequest.LookupSRbyContactPartyID(contactPartyID)
         *  Currently this list is only showing certain fields (because of combining 2 lists with common fields)
         *  The Right Now incidents by a contact report is hidden, meaning the Report control of a contact
         *  workspace tab is based on the Siebel Service Request List Table report definition
         *  Also, do not change the default column heading of Right Now incidents by a contact report 
         *  (they are hard coded to uppercase). Because they are hidden anyway.
         *  The Siebel Service Request List Table report definition column headings can be changed and those are
         *  the ones being displayed.
         */
        public override IList<IReportRow> GetRows(IList<string> columns, IReportFilterNode filterNode)
        {
            IList<IReportRow> reportRows = new List<IReportRow>();
            IRecordContext _context = ((SiebelVirtualReportTablesPackage)this.Parent)._globalContext.AutomationContext.CurrentWorkspace;

            if (_context == null)
                return reportRows;

            IContact contactRecord = _context.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Contact) as IContact;

             /* if auto refresh every x sec is enabled even though the tab is not active
             * so need to check contactRecord for null (when the editor is different)
             */
            if (contactRecord == null)
                return reportRows;

            string contactPartyID = null;
            // get the ebs contact party custom attribute on the contact workspace
            contactPartyID = getContactPartyIdCustomAttr(contactRecord);

            // following to get the rNow incidents report and filter is the rNow contactID
            AnalyticsReport reportIncident = new AnalyticsReport();
            ID rId = new ID();
            rId.id = ConfigurationSetting.incidentsByContactReportID;
            rId.idSpecified = true;
            reportIncident.ID = rId;
            byte[] outByte = new byte[1000];

            AnalyticsReportFilter[] filter = new AnalyticsReportFilter[3];
            filter[0] = new AnalyticsReportFilter();

            String[] filterString = new String[1];
            filterString[0] = "" + contactRecord.ID;
            filter[0].Values = filterString;
            filter[0].Name = "Contact"; // incidents by a contact, thus Contact filter

            NamedID datatype = new NamedID();
            datatype.Name = "Integer";
            filter[0].DataType = datatype;

            reportIncident.Filters = filter;
            
            ClientInfoHeader _cih = new ClientInfoHeader();
            _cih.AppID = "Accelerator Report Add-In";
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            CSVTableSet tableSet = ConfigurationSetting.client.RunAnalyticsReport(
              _cih, reportIncident, 100, 0, "\t", false, false, out outByte
                );
            stopwatch.Stop();
            string logMessage = "Called RightNowSyncPortClient.RunAnalyticsReport." +
                "reportID: " + ConfigurationSetting.incidentsByContactReportID;
            ConfigurationSetting.logWrap.DebugLog(0, contactRecord.ID, logMessage: logMessage, timeElapsed: (int)stopwatch.ElapsedMilliseconds);

            CSVTable[] csvTables = tableSet.CSVTables;
            CSVTable table = csvTables[0];
            string[] rowData = table.Rows;
            int rNowIncidentCount = table.Rows.Length;
            int srVirtualTableCount = this.Columns.Count;
            string[] colHeadingIncidentReport = table.Columns.Split('\t');
            Hashtable srHashtable = new Hashtable();

            foreach (String commaRow in table.Rows)
            {
                ReportDataRow reportDataRow = new ReportDataRow(srVirtualTableCount);
                string[] colValue = commaRow.Split('\t');

                // the report output is stored as <columnHeading, value>
                Dictionary<string, string> dictRow = new Dictionary<string, string>();
                int i = 0;
                foreach (string val in colValue)
                {   /* make the column heading upper case (because the custom attribute heading 
                     * in the report designer sometime all in lower case, sometime the reverse)
                     */
                    dictRow.Add(colHeadingIncidentReport[i].ToUpper(), val);
                    i++;
                }

                addRnowIncidentRow(ref columns, ref reportDataRow, ref reportRows, ref srHashtable, dictRow);
            }

            if (contactPartyID != null)
            {
                ServiceRequest[] sRs = ServiceRequest.LookupSRbyContactPartyID(columns, contactPartyID, 0, contactRecord.ID);

                if (sRs == null)
                    return reportRows;

                foreach (ServiceRequest req in sRs)
                {
                    ReportDataRow reportDataRow = new ReportDataRow(this.Columns.Count);
                    if (req != null) // live ebs row 316 of 319 of contact 4431 return null 
                        addSiebelSrRow(ref columns, ref reportDataRow, ref reportRows, srHashtable, req);
                }
            }
            return reportRows;
        }

        // add Right Now Incident report row
        private void addRnowIncidentRow(ref IList<string> columns, ref ReportDataRow reportDataRow, 
            ref  IList<IReportRow> reportRows, ref Hashtable srHashtable, Dictionary<string, string> colValue)
        {
            string heading = null;
            string dateTimeString = null;
            try
            {
                foreach (var column in columns)
                {
                    ReportDataCell reportDataCell = new ReportDataCell();
                    switch (column)
                    {
                        case "Siebel$SRlistTable.SrNumber":
                            heading = "SIEBEL_SR_NUM";
                            reportDataCell.GenericValue = colValue[heading];
                            if (!String.IsNullOrEmpty(colValue[heading]))
                                srHashtable.Add(colValue[heading], true);
                            break;
                        case "Siebel$SRlistTable.IncidentRef":
                            heading = "REFERENCE #";
                            reportDataCell.GenericValue = colValue[heading];
                            break;
                        case "Siebel$SRlistTable.Created":
                            heading = "DATE CREATED";
                            dateTimeString = colValue[heading];
                            // remove the single '
                            dateTimeString = dateTimeString.TrimStart('\'');
                            dateTimeString = dateTimeString.TrimEnd('\'');
                            reportDataCell.GenericValue = Convert.ToDateTime(dateTimeString);
                            break;
                        case "Siebel$SRlistTable.Summary":
                            heading = "SUBJECT";
                            reportDataCell.GenericValue = colValue[heading];
                            break;
                        case "Siebel$SRlistTable.Status":
                            heading = "STATUS";
                            reportDataCell.GenericValue = colValue[heading];
                            break;
                        case "Siebel$SRlistTable.HiddenSRconcatIncident_ID":
                            heading = "INCIDENT ID";
                            // colValue[5] is hiddent IncidentID on the incidentsByContact report
                            reportDataCell.GenericValue = "_" + colValue[heading];
                            break;
                    }
                    reportDataRow.Cells.Add(reportDataCell);
                }
                reportRows.Add(reportDataRow);
            }
            catch (Exception ex)
            {
                string errMsg = "rnow_incidentsByContact report columns Heading: " + heading + " is missing";
                MessageBox.Show(errMsg, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                string logMessage = errMsg;
                ConfigurationSetting.logWrap.ErrorLog(logMessage: logMessage);
                throw ex;
            }
        }

        // Add Siebel Service Request row
        private void addSiebelSrRow(ref IList<string> columns, ref ReportDataRow reportDataRow, ref  IList<IReportRow> reportRows, Hashtable srHashtable, ServiceRequest req)
        {
            bool srInRnow = false;

            foreach (var column in columns)
            {
                ReportDataCell reportDataCell = new ReportDataCell();

                switch (column)
                {
                    case "Siebel$SRlistTable.SrNumber":
                        reportDataCell.GenericValue = req.RequestNumber;
                        if (srHashtable.ContainsKey(req.RequestNumber))
                            srInRnow = true;
                        break;
                    case "Siebel$SRlistTable.Status":
                        reportDataCell.GenericValue = req.Status;
                        break;
                    case "Siebel$SRlistTable.Summary":
                        reportDataCell.GenericValue = req.Summary;
                        break;
                    case "Siebel$SRlistTable.Created":
                        reportDataCell.GenericValue = req.RequestDate;
                        break;
                    case "Siebel$SRlistTable.IncidentRef":
                        reportDataCell.GenericValue = "";
                        break;
                    case "Siebel$SRlistTable.HiddenSRconcatIncident_ID":
                        reportDataCell.GenericValue = req.RequestID + "_";
                        break;
                }
                reportDataRow.Cells.Add(reportDataCell);
            }
            if (!srInRnow)
            {
                reportRows.Add(reportDataRow);
                srInRnow = false;
            }
        }
    }

}
