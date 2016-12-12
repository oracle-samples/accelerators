/* * *******************************************************************************************
*  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + OSC Lead Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.11 (November 2015)
 *  OSC release: Release 10
 *  reference: 150505-000122, 160620-000160
 *  date: Mon Sep 19 02:05:30 PDT 2016

 *  revision: rnw-15-11-fixes-release-3
*  SHA1: $Id: deefa8b921ffabcfe29e67e926d218111d9d6d67 $
* *********************************************************************************************
*  File: OpportunityVirtualTable.cs
* ****************************************************************************************** */

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accelerator.SalesCloud.Client.Common;
using Accelerator.SalesCloud.Client.Logs;
using Accelerator.SalesCloud.Client.Model;
using Accelerator.SalesCloud.Client.RightNow;
using Accelerator.SalesCloud.Client.Services;
using RightNow.AddIns.AddInViews;
using RightNow.AddIns.Common;
using Accelerator.SalesCloud.OpportunityReportAddIn;

namespace Accelerator.SalesCloud.OpportunityReport.ViewModel
{
    public class OpportunityVirtualTable : ReportTable
    {

        private IOSCLog _logger;

        public OpportunityVirtualTable(IReportTablePackage package)
            : base(package)
        {
            _logger = OSCLogService.GetLog();

            this.Name = OSCOpportunitiesTableMetadata.Name;
            this.Label = OSCOpportunitiesTableMetadata.Label;
            this.Description = OSCOpportunitiesTableMetadata.Description;

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = OSCOpportunitiesTableColumnLabels.KeyContactId,
                Name = OSCOpportunitiesTableColumnNames.KeyContactId,
                CanDisplay = true,
                CanFilter = true,
                Description = OSCOpportunitiesTableColumnDesc.KeyContactId
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = OSCOpportunitiesTableColumnLabels.PrimaryContactPartyName,
                Name = OSCOpportunitiesTableColumnNames.PrimaryContactPartyName,
                CanDisplay = true,
                CanFilter = true,
                Description = OSCOpportunitiesTableColumnDesc.PrimaryContactPartyName
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = OSCOpportunitiesTableColumnLabels.SalesAccountId,
                Name = OSCOpportunitiesTableColumnNames.SalesAccountId,
                CanDisplay = true,
                CanFilter = true,
                Description = OSCOpportunitiesTableColumnDesc.SalesAccountId
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = OSCOpportunitiesTableColumnLabels.TargetPartyName,
                Name = OSCOpportunitiesTableColumnNames.TargetPartyName,
                CanDisplay = true,
                CanFilter = true,
                Description = OSCOpportunitiesTableColumnDesc.TargetPartyName
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = OSCOpportunitiesTableColumnLabels.OptyId,
                Name = OSCOpportunitiesTableColumnNames.OptyId,
                CanDisplay = true,
                CanFilter = true,
                Description = OSCOpportunitiesTableColumnDesc.OptyId
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = OSCOpportunitiesTableColumnLabels.OptyNumber,
                Name = OSCOpportunitiesTableColumnNames.OptyNumber,
                CanDisplay = true,
                CanFilter = true,
                Description = OSCOpportunitiesTableColumnDesc.OptyNumber
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = OSCOpportunitiesTableColumnLabels.PartyName1,
                Name = OSCOpportunitiesTableColumnNames.PartyName1,
                CanDisplay = true,
                CanFilter = true,
                Description = OSCOpportunitiesTableColumnDesc.PartyName1
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = OSCOpportunitiesTableColumnLabels.EmailAddress,
                Name = OSCOpportunitiesTableColumnNames.EmailAddress,
                CanDisplay = true,
                CanFilter = true,
                Description = OSCOpportunitiesTableColumnDesc.EmailAddress
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = OSCOpportunitiesTableColumnLabels.Name,
                Name = OSCOpportunitiesTableColumnNames.Name,
                CanDisplay = true,
                CanFilter = true,
                Description = OSCOpportunitiesTableColumnDesc.Name
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = OSCOpportunitiesTableColumnLabels.Description,
                Name = OSCOpportunitiesTableColumnNames.Description,
                CanDisplay = true,
                CanFilter = true,
                Description = OSCOpportunitiesTableColumnDesc.Description
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = OSCOpportunitiesTableColumnLabels.StatusCode,
                Name = OSCOpportunitiesTableColumnNames.StatusCode,
                CanDisplay = true,
                CanFilter = true,
                Description = OSCOpportunitiesTableColumnDesc.StatusCode
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = OSCOpportunitiesTableColumnLabels.SalesMethod,
                Name = OSCOpportunitiesTableColumnNames.SalesMethod,
                CanDisplay = true,
                CanFilter = true,
                Description = OSCOpportunitiesTableColumnDesc.SalesMethod
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = OSCOpportunitiesTableColumnLabels.SalesStage,
                Name = OSCOpportunitiesTableColumnNames.SalesStage,
                CanDisplay = true,
                CanFilter = true,
                Description = OSCOpportunitiesTableColumnDesc.SalesStage
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = OSCOpportunitiesTableColumnLabels.SalesChannelCd,
                Name = OSCOpportunitiesTableColumnNames.SalesChannelCd,
                CanDisplay = true,
                CanFilter = true,
                Description = OSCOpportunitiesTableColumnDesc.SalesChannelCd
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = OSCOpportunitiesTableColumnLabels.CurrencyCode,
                Name = OSCOpportunitiesTableColumnNames.CurrencyCode,
                CanDisplay = true,
                CanFilter = true,
                Description = OSCOpportunitiesTableColumnDesc.CurrencyCode
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = OSCOpportunitiesTableColumnLabels.Revenue,
                Name = OSCOpportunitiesTableColumnNames.Revenue,
                CanDisplay = true,
                CanFilter = true,
                Description = OSCOpportunitiesTableColumnDesc.Revenue
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = OSCOpportunitiesTableColumnLabels.WinProb,
                Name = OSCOpportunitiesTableColumnNames.WinProb,
                CanDisplay = true,
                CanFilter = true,
                Description = OSCOpportunitiesTableColumnDesc.Revenue
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = OSCOpportunitiesTableColumnLabels.ForecastedCloseDate,
                Name = OSCOpportunitiesTableColumnNames.ForecastedCloseDate,
                CanDisplay = true,
                Description = OSCOpportunitiesTableColumnDesc.ForecastedCloseDate
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = OSCOpportunitiesTableColumnLabels.CreatedBy,
                Name = OSCOpportunitiesTableColumnNames.CreatedBy,
                CanDisplay = true,
                Description = OSCOpportunitiesTableColumnDesc.CreatedBy
            });

            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = OSCOpportunitiesTableColumnLabels.CreationDate,
                Name = OSCOpportunitiesTableColumnNames.CreationDate,
                CanDisplay = true,
                Description = OSCOpportunitiesTableColumnDesc.CreationDate
            });
        }

        #region Overridden Members

        /// <summary>
        /// Overriden method responsible for populating a report with opportunity data
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="filterNode"></param>
        /// <returns>Rows with opportunity data</returns>
        public override IList<IReportRow> GetRows(IList<string> columns, IReportFilterNode filterNode)
        {
            IList<IReportRow> reportRows = new List<IReportRow>();

            IRecordContext _context = ((OpportunityReportAddIn.OpportunityReportAddIn)this.Parent)._globalContext.AutomationContext.CurrentWorkspace;
            List<OpportunityModel> opportunitesList = getOpportunitesList(_context);
            if (null != opportunitesList)
            {
                foreach (OpportunityModel oppData in opportunitesList)
                {
                    ReportDataRow reportDataRow = new ReportDataRow(this.Columns.Count);
                    foreach (var column in columns)
                    {
                        var reportDataCell = new ReportDataCell();

                        switch (column)
                        {
                            case "Opportunity$OSCOpportunity.osc_contact_party_id":
                                reportDataCell.GenericValue = (oppData.KeyContactId != null) ? oppData.KeyContactId.ToString() : OSCOpportunitiesCommon.NoValue;
                                break;
                            case "Opportunity$OSCOpportunity.osc_account_party_id":
                                reportDataCell.GenericValue = (oppData.SalesAccountId != null) ? oppData.SalesAccountId.ToString() : OSCOpportunitiesCommon.NoValue;
                                break;
                            case "Opportunity$OSCOpportunity.org_name":
                                reportDataCell.GenericValue = oppData.TargetPartyName ?? OSCOpportunitiesCommon.NoValue;
                                break;
                            case "Opportunity$OSCOpportunity.osc_op_id":
                                reportDataCell.GenericValue = oppData.OptyId.ToString();
                                break;
                            case "Opportunity$OSCOpportunity.osc_op_owner_name":
                                reportDataCell.GenericValue = oppData.PartyName1 ?? OSCOpportunitiesCommon.NoValue;
                                break;
                            case "Opportunity$OSCOpportunity.osc_op_owner_email":
                                reportDataCell.GenericValue = oppData.EmailAddress ?? OSCOpportunitiesCommon.NoValue;
                                break;
                            case "Opportunity$OSCOpportunity.osc_op_name":
                                reportDataCell.GenericValue = oppData.Name ?? OSCOpportunitiesCommon.NoValue;
                                break;
                            case "Opportunity$OSCOpportunity.osc_op_desc":
                                reportDataCell.GenericValue = oppData.Description ?? OSCOpportunitiesCommon.NoValue;
                                break;
                            case "Opportunity$OSCOpportunity.osc_op_status":
                                reportDataCell.GenericValue = oppData.StatusCode ?? OSCOpportunitiesCommon.NoValue;
                                break;
                            case "Opportunity$OSCOpportunity.osc_op_method":
                                reportDataCell.GenericValue = oppData.SalesMethod ?? OSCOpportunitiesCommon.NoValue;
                                break;
                            case "Opportunity$OSCOpportunity.osc_op_stage":
                                reportDataCell.GenericValue = oppData.SalesStage ?? OSCOpportunitiesCommon.NoValue;
                                break;
                            case "Opportunity$OSCOpportunity.osc_op_channel":
                                reportDataCell.GenericValue = oppData.SalesChannelCd ?? OSCOpportunitiesCommon.NoValue;
                                break;
                            case "Opportunity$OSCOpportunity.osc_op_currency":
                                reportDataCell.GenericValue = oppData.CurrencyCode ?? OSCOpportunitiesCommon.NoValue;
                                break;
                            case "Opportunity$OSCOpportunity.osc_op_revenue":
                                reportDataCell.GenericValue = (oppData.Revenue != null) ? oppData.Revenue.ToString() : OSCOpportunitiesCommon.NoValue;
                                break;
                            case "Opportunity$OSCOpportunity.osc_op_win_percent":
                                reportDataCell.GenericValue = (oppData.WinProb != null) ? oppData.WinProb + "%" : OSCOpportunitiesCommon.NoValue;
                                break;
                            case "Opportunity$OSCOpportunity.osc_fcst_close_date":
                                reportDataCell.GenericValue = (oppData.ForecastedCloseDate != null) ? oppData.ForecastedCloseDate.ToString() : OSCOpportunitiesCommon.NoValue;
                                break;
                            case "Opportunity$OSCOpportunity.osc_created_by":
                                reportDataCell.GenericValue = oppData.CreatedBy ?? OSCOpportunitiesCommon.NoValue;
                                break;
                            case "Opportunity$OSCOpportunity.osc_created":
                                reportDataCell.GenericValue = (oppData.CreationDate != null) ? oppData.CreationDate.ToString() : OSCOpportunitiesCommon.NoValue;
                                break;
                            case "Opportunity$OSCOpportunity.osc_op_number":
                                reportDataCell.GenericValue = oppData.OptyNumber ?? OSCOpportunitiesCommon.NoValue;
                                break;
                            case "Opportunity$OSCOpportunity.osc_contact_name":
                                reportDataCell.GenericValue = oppData.PrimaryContactPartyName ?? OSCOpportunitiesCommon.NoValue;
                                break;

                        }

                        reportDataRow.Cells.Add(reportDataCell);
                    }

                    reportRows.Add(reportDataRow);
                }
            }

            return reportRows;
        }

        #endregion

        #region Private Members

        /// <summary>
        /// Method to fetch Opportunity and return respective models.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>List of Opportunity Models</returns>
        private List<OpportunityModel> getOpportunitesList(IRecordContext context)
        {

            var opportunitiesList = new List<OpportunityModel>();

            if (context != null)
            {
                var tokenSource = new CancellationTokenSource();
                CancellationToken token = tokenSource.Token;
                Boolean timeout = false;
                string timeoutValue = RightNowConfigService.GetConfigValue(RightNowConfigKeyNames.SalesTimeout);

                var orgRecord = context.GetWorkspaceRecord(WorkspaceRecordType.Organization) as IOrganization;
                if (orgRecord != null)
                {
                    string ext_ref = RightNowConnectService.GetService().GetContactOrgExternalReference(orgRecord.ID,
                        false, OracleRightNowOSCAddInNames.OpportunityReportTableAddIn);

                    //If the contact is a non sales contact
                    if (String.IsNullOrEmpty(ext_ref))
                    {
                        _logger.Debug("External reference is empty or null for the org. Returning empty opportunity list.");
                        return opportunitiesList;
                    }
                    
                    if (!String.IsNullOrEmpty(timeoutValue))
                    {
                        var task = Task.Factory.StartNew(() => OpportunityService.GetService().FindOpenOpportunities(OSCOpportunitiesTableMetadata.OrgFilterColumn, ext_ref), token);

                        if (!task.Wait(Convert.ToInt32(timeoutValue)))
                        {
                            timeout = true;
                            tokenSource.Cancel();
                        }
                        else
                        {
                            opportunitiesList = task.Result;
                            task.Dispose();
                        }
                    }
                    else
                    {
                        _logger.Debug("Sales Timesout value is either empty or null");
                        opportunitiesList = OpportunityService.GetService().FindOpenOpportunities(OSCOpportunitiesTableMetadata.OrgFilterColumn, ext_ref);
                    }
                    _logger.Debug("No. of opportunities fetched for Org #" + ext_ref + " : " + ((opportunitiesList != null) ? opportunitiesList.Count.ToString() : "null"));
                }
                else
                {
                    var contactRecord = context.GetWorkspaceRecord(WorkspaceRecordType.Contact) as IContact;
                    if (contactRecord != null)
                    {
                        string ext_ref = RightNowConnectService.GetService().GetContactOrgExternalReference(contactRecord.ID,
                        true, OracleRightNowOSCAddInNames.OpportunityReportTableAddIn);

                        //If the contact is a non sales contact
                        if (String.IsNullOrEmpty(ext_ref))
                        {
                            _logger.Debug("External reference is empty or null for contact. Returning empty opportunity list.");
                            return opportunitiesList;
                        }

                        if (!String.IsNullOrEmpty(timeoutValue))
                        {
                            var task = Task.Factory.StartNew(() => OpportunityService.GetService().FindOpenOpportunities(OSCOpportunitiesTableMetadata.ContactFilterColumn, ext_ref), token);

                            if (!task.Wait(Convert.ToInt32(timeoutValue)))
                            {
                                timeout = true;
                                tokenSource.Cancel();
                            }
                            else
                            {
                                opportunitiesList = task.Result;
                                task.Dispose();
                            }
                        }
                        else
                        {
                            _logger.Debug("Sales Timesout value is either empty or null");
                            opportunitiesList = OpportunityService.GetService().FindOpenOpportunities(OSCOpportunitiesTableMetadata.ContactFilterColumn, ext_ref);
                        }
                        _logger.Debug("No. of opportunities fetched for Contact #" + ext_ref + " : " + ((opportunitiesList != null) ? opportunitiesList.Count.ToString() : "null"));
                    }
                }

                if (timeout)
                {
                    _logger.Debug("FindOpportunity request timed out!");
                    MessageBox.Show(OSCExceptionMessages.FindOppTimedOut, OSCOpportunitiesCommon.FindOppTimedOutTitle, 
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

            }

            return opportunitiesList;
        }
        #endregion

    }
}
