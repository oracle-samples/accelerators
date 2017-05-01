/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  Copyright (c) 2014, 2015, 2016, 2017 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + SRM Integration Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 17.2 (February 2017) 
 *  reference: 160628-000117
 *  date: Fri Feb 10 19:47:42 PST 2017
 
 *  revision: rnw-17-2-fixes-release-2
 *  SHA1: $Id: 4b2966eaa9c516fb635cdb256bfc74f34461945d $
 * *********************************************************************************************
 *  File: SrmRepliesListVirtualTable.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RightNow.AddIns.AddInViews;
using RightNow.AddIns.Common;
using System.Web.Script.Serialization;
using System.Net.Http;
using System.Net.Http.Headers;
using Accelerator.SRM.SharedServices;
using System.Windows.Forms;
using System.Diagnostics;

namespace Accelerator.SRM.ReportTablesAddIn
{
    public class SrmRepliesListVirtualTable : ReportTable
    {
        //List<Item> _results = new List<Item>();

        public SrmRepliesListVirtualTable(IReportTablePackage2 package)
            : base(package)
        {
            this.Name = "SrmRepliesListTable";
            this.Label = "SRM Replies List Table";
            this.Description = "SRM Replies List Table";

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = "ReplyID",
                Name = "ReplyID",
                CanDisplay = true,
                CanFilter = true,
                IsNullable = false,
                IsKey = true
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.Integer,
                Label = "ConversationID",
                Name = "ConversationID",
                CanDisplay = true,
                CanFilter = true,
                IsNullable = true,
            });           

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.Integer,
                Label = "BundleID",
                Name = "BundleID",
                CanDisplay = true,
                CanFilter = true,
                IsNullable = true,
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = "externalId",
                Name = "externalId",
                CanDisplay = true,
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = "type",
                Name = "type",
                CanDisplay = true,
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = "authorName",
                Name = "authorName",
                CanDisplay = true,
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = "authorProfileUrl",
                Name = "authorProfileUrl",
                CanDisplay = true,
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = "authorImage",
                Name = "authorImage",
                CanDisplay = true,
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = "postedAt",
                Name = "postedAt",
                CanDisplay = true,
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = "status",
                Name = "status",
                CanDisplay = true,
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = "body",
                Name = "body",
                CanDisplay = true,
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = "labels",
                Name = "labels",
                CanDisplay = true,
            });
           
            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.Boolean,
                Label = "liked",
                Name = "liked",
                CanDisplay = true,
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.Integer,
                Label = "likesCount",
                Name = "likesCount",
                CanDisplay = true,
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = "attachmentType",
                Name = "attachmentType",
                CanDisplay = true,
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = "attachmentUrl",
                Name = "attachmentUrl",
                CanDisplay = true,
            });
        }

        public override IList<IReportRow2> GetRows(IList<string> columns, IReportFilterNode filterNode)
        {
            Stopwatch clickLogStopwatch = new Stopwatch();
            clickLogStopwatch.Start();
            IList<IReportRow2> reportRows = getRows(columns, filterNode);
            clickLogStopwatch.Stop();
            ConfigurationSetting.logWrap.ClickLog(0, 0, "SrmRepliesListVirtualTable report AddIn GetRows() total time:", null, (int)clickLogStopwatch.ElapsedMilliseconds);
            return reportRows;
        }

        private IList<IReportRow2> getRows(IList<string> columns, IReportFilterNode filterNode)
        {
            IList<IReportRow2> reportRows = new List<IReportRow2>();

            IRecordContext _context = ((SrmReportTablePackage)this.Parent)._globalContext.AutomationContext.CurrentWorkspace;

            IIncident incidentRecord = null;
            int convId = 0, bundleId = 0;
            String endpoint = null;

            if (_context != null)
            {
                incidentRecord = _context.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Incident) as IIncident;
                convId = ConfigurationSetting.getSrmCustomAttr(incidentRecord, "srm_conversation_id");
                bundleId = ConfigurationSetting.getSrmCustomAttr(incidentRecord, "srm_bundle_id");

                if (convId == 0 || bundleId == 0)
                    return reportRows;
            }
            else
            {
                if (filterNode != null && filterNode.FilterNodes != null)
                {
                    IReportFilterNode convIDFilterNode = filterNode.FilterNodes.ToList<IReportFilterNode>().Find(fn => fn.ReportFilter.Expression == string.Format("{0}${1}.ConversationID", this.Parent.Name, this.Name));

                    if (convIDFilterNode != null)
                        convId = Convert.ToInt32(convIDFilterNode.ReportFilter.Value);

                    if (convId == 0)
                        return reportRows;

                    IReportFilterNode bundleIDFilterNode = filterNode.FilterNodes.ToList<IReportFilterNode>().Find(fn => fn.ReportFilter.Expression == string.Format("{0}${1}.BundleID", this.Parent.Name, this.Name));

                    if (bundleIDFilterNode != null)
                        bundleId = Convert.ToInt32(bundleIDFilterNode.ReportFilter.Value);                    
                }
            }

            endpoint = String.Format(ConfigurationSetting.convReplyGETEndpoint, convId, bundleId, ConfigurationSetting.max_srm_rows_fetch);

            // determine whether to close or re-open a conversation
            String jsonStr = "{\"status\" : \"active\"}";
            HttpContent content = new StringContent(jsonStr, Encoding.UTF8, "application/json");
            ConfigurationSetting.logWrap.DebugLog(logMessage: Accelerator.SRM.SharedServices.Properties.Resources.GETRequestMessage, logNote: endpoint);
            var results = RESTHelper.PerformGET(endpoint, ref ((SrmReportTablePackage)this.Parent)._globalContext);

            if (results != null)
            {
                if (!results.Success)
                {
                    MessageBox.Show(Properties.Resources.GetRowsError, Properties.Resources.Error);
                    ConfigurationSetting.logWrap.ErrorLog(logMessage: "Response GET Conversation message error", logNote: results.JSON);
                }

                var jsonData = results.JSON;

                if (jsonData == null || jsonData == "")
                    return reportRows;

                JavaScriptSerializer ser = new JavaScriptSerializer();
                RootObject replies = ser.Deserialize<RootObject>(jsonData);

                foreach (Item req in replies.items)
                {
                    ReportDataRow reportDataRow = new ReportDataRow(this.Columns.Count);
                    object reportDataKey = req.id;
                    foreach (var column in columns)
                    {
                        ReportDataCell reportDataCell = new ReportDataCell();

                        switch (column)
                        {
                            case "SRM_Data$SrmRepliesListTable.ReplyID":
                                reportDataCell.GenericValue = req.content.id;
                                break;
                            case "SRM_Data$SrmRepliesListTable.liked":
                                reportDataCell.GenericValue = Convert.ToBoolean(req.content.liked);
                                break;
                            case "SRM_Data$SrmRepliesListTable.likesCount":
                                reportDataCell.GenericValue = Convert.ToInt32(req.content.likesCount);
                                break;
                            case "SRM_Data$SrmRepliesListTable.authorName":
                                reportDataCell.GenericValue = req.content.author.name;
                                break;
                            case "SRM_Data$SrmRepliesListTable.authorImage":
                                reportDataCell.GenericValue = req.content.author.authorImage;
                                break;
                            case "SRM_Data$SrmRepliesListTable.attachmentType":
                                reportDataCell.GenericValue = req.content.attachments != null ? req.content.attachments[0].type : null;
                                break;
                            case "SRM_Data$SrmRepliesListTable.attachmentUrl":
                                reportDataCell.GenericValue = req.content.attachments != null ? req.content.attachments[0].url : null;
                                break;
                            case "SRM_Data$SrmRepliesListTable.authorProfileUrl":
                                reportDataCell.GenericValue = req.content.author.authorProfileUrl;
                                break;
                            case "SRM_Data$SrmRepliesListTable.externalId":
                                reportDataCell.GenericValue = req.content.externalId;
                                break;
                            case "SRM_Data$SrmRepliesListTable.type":
                                reportDataCell.GenericValue = req.content.type;
                                break;
                            case "SRM_Data$SrmRepliesListTable.postedAt":
                                DateTime utcTime = DateTime.Parse(req.content.postedAt);
                                DateTime localTime = utcTime.ToLocalTime();
                                reportDataCell.GenericValue = localTime != null ? localTime.ToString() : "";
                                break;
                            case "SRM_Data$SrmRepliesListTable.status":
                                reportDataCell.GenericValue = req.content.status;
                                break;
                            case "SRM_Data$SrmRepliesListTable.body":
                                reportDataCell.GenericValue = req.content.body;
                                break;
                            case "SRM_Data$SrmRepliesListTable.labels":
                                if (req.content.labels.Count == 0)
                                    reportDataCell.GenericValue = "No Value";
                                else
                                {
                                    foreach (String label in req.content.labels)
                                    {
                                        reportDataCell.GenericValue += ", " + label;
                                    }
                                }
                                break;
                        }

                        reportDataRow.Cells.Add(reportDataCell);
                    }
                    //Please set the Key, it is necessary to edit report cell
                    reportDataRow.Key = reportDataKey;
                    reportRows.Add(reportDataRow);
                }
            }

            return reportRows;
        }

        public override void CommitEdits(IDictionary<object, IDictionary<string, object>> editedData)
        {
        }
    }
}
