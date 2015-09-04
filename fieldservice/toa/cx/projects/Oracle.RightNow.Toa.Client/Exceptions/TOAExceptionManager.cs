/* * *******************************************************************************************
*  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + OFSC Reference Integration
 *  link: http://www-content.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.2 (Feb 2015)
 *  OFSC release: 15.2 (Feb 2015)
 *  reference: 150622-000130
 *  date: Mon Aug 24 09:01:16 PDT 2015

 *  revision: rnw-15-11-fixes-release-01
*  SHA1: $Id: 5f0b9ee7cc27c0dd85459c33fb61eb6f992f784c $
* *********************************************************************************************
*  File: TOAExceptionManager.cs
* ****************************************************************************************** */

using System.Windows.Forms;
using System.Text;
using System.Collections.Generic;
using Oracle.RightNow.Toa.Client.Services;
using Oracle.RightNow.Toa.Client.Model;
using Oracle.RightNow.Toa.Client.Logs;
using Oracle.RightNow.Toa.Client.Common;
using System;


namespace Oracle.RightNow.Toa.Client.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class ToaExceptionManager
    {

        private static HashSet<string> ErrorsNotToSave = new HashSet<string>
        {
            "60080", "69001", "69002", "69003", "69005", "69006", "69007", "69008", "69009", "69010", "69011", "69012", "69013", "69014", "69015", "69016", "69017", "69018", "69019", "69020",
            "69021", "69022", "69023", "69024", "69025", "69026", "69027", "69028", "69029", "69030", "69031", "69032", "69033", "69034", "69035", "69050", "69065", "69066", "69067" ,"69068",
            "69069", "69070", "69079", "69080", "69086", "69087", "69088", "69094", "69095", "69096", "69097", "69100", "69102", "69105", "69106", "69107", "69108", "69109", "69110", "69111",
            "69113", "69124", "69126", "69127", "69128", "69129", "69130", "69131", "69132", "69133", "69134", "69135", "69136", "69137", "69138", "69141", "69142", "69143", "69144", "69145",
            "69146", "69147", "69148", "69149", "69150", "69151", "69152", "69153", "69154", "69155", "69160", "69161", "69162", "69165", "69166", "69167", "69168", "69170", "69174", "69175",
            "69176", "69177", "69179", "69189", "69190"
        };

        private static HashSet<string> ActivityNotCreated = new HashSet<string>
        {
            "60080", "69001", "69002", "69003", "69004", "69005", "69006", "69007", "69008", "69009", "69010", "69011", "69012", "69013", "69014", "69015", "69016", "69017", "69018", "69019",
            "69020", "69021", "69022", "69023", "69024", "69025", "69026", "69027", "69028", "69029", "69037"
        };

        private static HashSet<string> WorkOrderNotCreated = new HashSet<string>
        {
            "69030", "69031", "69032", "69033", "69034", "69035", "69050", "69065", "69066", "69067", "69068", "69069", "69070", "69079", "69080", "69086", "69087", "69088", "69094", "69095",
            "69096", "69097", "69100", "69102", "69105", "69106", "69107", "69108", "69109", "69110", "69111", "69113", "69124", "69126", "69127", "69128", "69129", "69130", "69131", "69132",
            "69133", "69134", "69135", "69136", "69137", "69138", "69141", "69142", "69143", "69144", "69145", "69146", "69147", "69148", "69150", "69151", "69152", "69154", "69155", "69160",
            "69161", "69162"
        };

        private static HashSet<string> WorkOrderCreatedWithIssues = new HashSet<string>
        {
            "69045", "69046", "69049", "69051", "69052", "69053", "69054", "69055", "69056", "69057", "69058", "69059", "69060", "69061", "69062", "69063", "69064", "69098", "69101", "69116",
            "69121", "69122", "69123"
        };

        private static HashSet<string> WrkOrderRelnshipNotDefined = new HashSet<string>
        {
            "69164", "69169", "69099", "69103", "69104", "69114", "69115", "69156", "69157"
        };

        private static HashSet<string> ErrorRetreivingWrkOrderArea = new HashSet<string>
        {
            "69165", "69166", "69167", "69170", "69174", "69175", "69176", "69177", "69179", "69189", "69190", "69149", "69153"
        };

        private static HashSet<string> AssetOrInventorySystemExperiencedProblem = new HashSet<string>
        {
            "69180", "69181", "69184", "69183", "69185", "69186", "69187", "69188", "69117", "69118", "69119", "69120"
        };

        private static HashSet<string> WrkOrderAreaSystemExperiencedProblem = new HashSet<string> 
        {
            "69125"
        };

        private static HashSet<string> ChangingWrkOrderSystemExperiencedProblem = new HashSet<string> 
        {
            "69139", "69140", "69158", "69159", "69171"
        };

        private static HashSet<string> UpdateNotSuccessful = new HashSet<string>
        {
            "69172", "69173"
        };

        private IToaLog _log;

        public ToaExceptionManager()
        {
            _log = ToaLogService.GetLog();
        }

        public void ShowResponseReportMessages(ToaRequestResult requestResult)
        {
            // Todo: Show report dialog with result
        }


        public int? ProcessInboundResult(ToaRequestResult inboundResult)
        {
            int? aid = null;
            HashSet<string> errors = new HashSet<string>();
            bool ErrorDialogShown = false;
            
            {
                if (inboundResult.DataModels.Count > 0)
                {
                    WorkOrderModel workOrderModel = null;
                    List<IToaModel>.Enumerator e = inboundResult.DataModels.GetEnumerator();
                    while (e.MoveNext())
                    {
                        IToaModel model = e.Current;
                        if (model is WorkOrderModel)
                        {
                            workOrderModel = (WorkOrderModel)model;

                            foreach (ReportMessageModel reportMessage in workOrderModel.ReportMessages)
                            {
                                if (!reportMessage.Code.Equals(ActivityProperty.TOA_Report_Success_Code))
                                {
                                    errors.Add(reportMessage.Code);
                                    _log.Error(reportMessage.Code, reportMessage.Description);
                                }
                            }

                            List<InventoryModel> inventories = workOrderModel.ActivityInventories;
                            if (null != inventories && inventories.Count > 0)
                            {
                                foreach (InventoryModel inventory in inventories)
                                {
                                    List<ReportMessageModel> reportMessages = inventory.ReportMessages;
                                    foreach (ReportMessageModel reportMessage in reportMessages)
                                    {
                                        if (!reportMessage.Code.Equals(ActivityProperty.TOA_Report_Success_Code))
                                        {
                                            errors.Add(reportMessage.Code);
                                            _log.Error(reportMessage.Code, reportMessage.Description);
                                        }
                                    }
                                }
                            }

                            foreach (string errorcode in errors)
                            {
                                if (!ErrorDialogShown)
                                {
                                    ShowErrorOrWarningDialog(errorcode);
                                    ErrorDialogShown = true;
                                }

                                if (ErrorsNotToSave.Contains(errorcode))
                                {
                                    return null;
                                }
                            }

                            aid = workOrderModel.TOA_AID;
                        }
                    }
                }
            }
            return aid;
        }

        /// <summary>
        /// Process CapacityServiceResult
        /// </summary>
        /// <param name="capacityServiceResult"></param>
        public void ProcessCapacityServiceResult(ToaRequestResult capacityServiceResult)
        {
            if (capacityServiceResult.ResultCode != Common.ToaRequestResultCode.Success)
            {
                StringBuilder errorMessage = new StringBuilder();
                foreach (ReportMessageModel reportMessage in capacityServiceResult.ReportMessages)
                {
                    errorMessage.Append(reportMessage.Result);
                    _log.Error("Error Result: " + reportMessage.Result);
                    _log.Error("Error Description: " + reportMessage.Description);
                }
                MessageBox.Show(errorMessage.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void ShowErrorOrWarningDialog(string errorCode)
        {
            if(ActivityNotCreated.Contains(errorCode))
            {
                MessageBox.Show(ToaExceptionMessages.MsgActivityNotCreated, 
                    ToaExceptionMessages.TitleActivityCreateFailed,  MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if(WorkOrderNotCreated.Contains(errorCode))
            {
                MessageBox.Show(ToaExceptionMessages.MsgWorkOrderNotCreated, 
                    ToaExceptionMessages.TitleActivityCreateFailed, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if(WorkOrderCreatedWithIssues.Contains(errorCode))
            {
                MessageBox.Show(ToaExceptionMessages.MsgWorkOrderCreatedWithIssues, 
                    ToaExceptionMessages.TitleSystemExperiencesProblem, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if(WrkOrderRelnshipNotDefined.Contains(errorCode))
            {
                MessageBox.Show(ToaExceptionMessages.MsgWrkOrderRelnshipNotDefined, 
                    ToaExceptionMessages.TitleWrkOrderDefnNotSuccessful, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if(ErrorRetreivingWrkOrderArea.Contains(errorCode))
            {
                MessageBox.Show(ToaExceptionMessages.MsgWorkOrderNotCreated, 
                    ToaExceptionMessages.TitleErrorRetreivingWrkOrderArea, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if(AssetOrInventorySystemExperiencedProblem.Contains(errorCode))
            {
                MessageBox.Show(ToaExceptionMessages.MsgWorkOrderCreatedWithIssues, 
                    ToaExceptionMessages.TitleAssetOrInventorySystemExperiencedProblem, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if(WrkOrderAreaSystemExperiencedProblem.Contains(errorCode))
            {
                MessageBox.Show(ToaExceptionMessages.MsgVerifyAddressDetails, 
                    ToaExceptionMessages.TitleWrkOrderAreaSystemExperiencedProblem, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if(ChangingWrkOrderSystemExperiencedProblem.Contains(errorCode))
            {
                MessageBox.Show(ToaExceptionMessages.MsgWorkOrderCreatedWithIssues, 
                    ToaExceptionMessages.TitleChangingWrkOrderSystemExperiencedProblem, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if(UpdateNotSuccessful.Contains(errorCode))
            {
                MessageBox.Show(ToaExceptionMessages.MsgCanNoLongerBeUpdated, 
                    ToaExceptionMessages.TitleUpdateNotSuccessful, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
