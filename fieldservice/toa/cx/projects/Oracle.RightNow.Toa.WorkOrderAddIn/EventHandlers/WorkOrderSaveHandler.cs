/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + OFSC Reference Integration
 *  link: http://www-content.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.2 (Feb 2015)
 *  OFSC release: 15.2 (Feb 2015)
 *  reference: 150622-000130
 *  date: Thu Sep  3 23:14:04 PDT 2015

 *  revision: rnw-15-11-fixes-release-03
 *  SHA1: $Id: de9dd99ab0e561ad99a0ea5b7d35852eb6f3f6c7 $
 * *********************************************************************************************
 *  File: WorkOrderSaveHandler.cs
 * ****************************************************************************************** */

using Oracle.RightNow.Toa.Client.Logs;
using Oracle.RightNow.Toa.Client.Model;
using Oracle.RightNow.Toa.Client.Rightnow;
using Oracle.RightNow.Toa.Client.Exceptions;
using Oracle.RightNow.Toa.Client.Services;
using Oracle.RightNow.Toa.WorkOrderAddIn.Common;
using RightNow.AddIns.AddInViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Oracle.RightNow.Toa.Client.Common;
using System.Globalization;

namespace Oracle.RightNow.Toa.WorkOrderAddIn.EventHandlers
{
    public class WorkOrderSaveHandler : IHandler
    {
        
        /// <summary>
        /// The current workspace record context.
        /// </summary>
        private IRecordContext _recordContext;
        private IToaLog _log;
        System.ComponentModel.CancelEventArgs _cancelEventArgs;

        ICustomObject _workOrderRecord { get; set; }

        // TODO : Remove below line
        //public WorkOrderSaveHandler()
        public WorkOrderSaveHandler(IRecordContext RecordContext, System.ComponentModel.CancelEventArgs e)
        {
            _recordContext = RecordContext;
            _cancelEventArgs = e;
            _workOrderRecord = _recordContext.GetWorkspaceRecord(_recordContext.WorkspaceTypeName) as ICustomObject;
            _log = ToaLogService.GetLog();
        }

        public void Handler()
        {
            InboundRequest inboundRequest = null;
            ToaRequestResult inboundResult = null;
            int? aid = null;
            WorkOrderModel workOrderModel = null;
            try
            {
                _log.Notice("Starting WorkOrderSave Handler."); 
                object wotype = EventHandlerServices.GetEventHandlerService().getFieldFromWorOrderRecord(_workOrderRecord,"WO_Type");
                object overrideRequest = EventHandlerServices.GetEventHandlerService().getFieldFromWorOrderRecord(_workOrderRecord, "Override_Request");
                object timeSlot = EventHandlerServices.GetEventHandlerService().getFieldFromWorOrderRecord(_workOrderRecord, "WO_Time_Slot");
                object wodate = EventHandlerServices.GetEventHandlerService().getFieldFromWorOrderRecord(_workOrderRecord, "WO_Date");
                object resolutionDue = EventHandlerServices.GetEventHandlerService().getFieldFromWorOrderRecord(_workOrderRecord, "Resolution_Due");
 
                if (null == wotype)
                {                    
                    return;
                }

                if (overrideRequest != null)
                {
                    if (timeSlot == null || wodate == null)
                    {
                            return;
                    }
                }
                else
                {
                    if(timeSlot == null || wodate == null)
                    {
                        if (resolutionDue == null)
                        {
                            return;
                        }
                    }
                }

                _log.Notice("Converting from RecordContext to WorkOrder Model started");
                workOrderModel = EventHandlerServices.GetEventHandlerService().GetWorkOrderModel(_recordContext); 
                _log.Notice("Convertion from RecordContext to WorkOrder Model completed");

                //Appointments 'key' fields are set in the constuctor of below object
                var activitySettings = new ActivitySettingsModel();
                //Inventory 'key' fields are set in the constuctor of below object
                var inventorySettings = new InventorySettingsModel();

                _log.Notice("Initialize InboundRequest");
                inboundRequest = new InboundRequest(new List<IToaModel>() { workOrderModel });
                inboundRequest.PropertiesMode = Client.Common.PropertiesMode.Replace;
                inboundRequest.AllowChangeDate = Client.Common.AllowChangeDate.Yes;
                inboundRequest.ActivitySettings = activitySettings;
                inboundRequest.InventorySettings = inventorySettings;
                _log.Notice("Invoking TOA Server using Inbound API Started");
                inboundResult = InboundService.GetService().BeginSyncRequest(inboundRequest);

                if (inboundResult != null)
                {
                    _log.Notice("Invoking TOA Server using Inbound API Completed");
                    ToaExceptionManager manager = new ToaExceptionManager();
                    _log.Notice("Processing Inbound API Response Result");
                    aid = manager.ProcessInboundResult(inboundResult);
                    _log.Notice("Completed Processing Inbound API Response Result");
                    if (aid == null)
                    {
                        _cancelEventArgs.Cancel = true;
                        return;
                    }
                }
                else
                {
                    foreach (IGenericField field in _workOrderRecord.GenericFields)
                    {
                        if (field.Name.Equals("WO_Status"))
                        {
                            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                            field.DataValue.Value = textInfo.ToTitleCase(ToaStringsUtil.GetString(Client.Common.ActivityStatus.NotCreated));
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message,ex.StackTrace);
                MessageBox.Show(ToaExceptionMessages.UnexpectedError, ToaExceptionMessages.TitleError, 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                _log.Notice("Updating WorkOrder Record.");
                workOrderModel.TOA_AID = (aid != null) ? aid : 0;
                UpdateWorkOrderRecord(inboundResult, workOrderModel);
                _log.Notice("WorkOrder Record Updated.");
                _log.Notice("Exiting WorkOrderSave Handler.");

            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex.StackTrace);
                MessageBox.Show(ToaExceptionMessages.UnexpectedError, ToaExceptionMessages.TitleError,
                    MessageBoxButton.OK, MessageBoxImage.Error);

            }
            
        }

        private void UpdateWorkOrderRecord(ToaRequestResult inboundResult, WorkOrderModel workOrderModel)
        {
            IList<IGenericField> fields = _workOrderRecord.GenericFields;
            if (null != fields)
            {
                foreach (IGenericField field in fields)
                {
                    switch (field.Name)
                    {
                        case "External_ID":
                            if (null != workOrderModel.TOA_AID)
                                field.DataValue.Value = workOrderModel.TOA_AID;                            
                            break;
                        case "WO_Status":
                            
                            if (null != workOrderModel.TOA_AID && workOrderModel.TOA_AID != 0)
                            {
                                // Change work order status if activity is cancelled in TOA
                                if (workOrderModel.CancelReason != null && !workOrderModel.CancelReason.Trim().Equals(""))
                                {
                                    workOrderModel.Status = Client.Common.ActivityStatus.Cancelled;                                                                        
                                }
                                else // Set Update Activity
                                {
                                    workOrderModel.Status = Client.Common.ActivityStatus.Pending;  
                                }                                
                            }
                            else
                            {
                                workOrderModel.Status = Client.Common.ActivityStatus.NotCreated;  
                            }
                            field.DataValue.Value = ToaStringsUtil.GetString(workOrderModel.Status);
                            break;
                        case "Duration":
                            if (null != workOrderModel.Duration)
                            {
                                field.DataValue.Value = Int32.Parse(workOrderModel.Duration);
                            }
                            break;
                        default:
                            break;
                    }
                }
                _recordContext.RefreshWorkspace();
                _log.Notice("WorkOrder saved");
            }
        }

        /**
         * Commeting out the code to be reused if customer decides to have dynamic field mapping.
         * This part of code fetches the data from the map and maps its value to approriate webservice field.
         * 
        private Dictionary<string, object> GetETADirectFieldValuePair()
        {
            Dictionary<string, object> ETADirectFieldValuePair = new Dictionary<string, object>();

            if (_workOrderRecord != null)
            {

                Dictionary<string, WorkOrderFieldMapping> WorkOrderFieldMappings = RightNowConnectService.GetService().WorkOrderFieldMappings;

                IList<IGenericField> fields = _workOrderRecord.GenericFields;
                foreach (IGenericField field in fields)
                {
                    if(field.DataValue.Value != null)
                    {
                        string fieldName = field.Name;

                        WorkOrderFieldMapping wofm = null;

                        WorkOrderFieldMappings.TryGetValue(fieldName, out wofm);

                        if (wofm != null)
                        {
                            ETADirectFieldValuePair.Add(wofm.WS_Field, field.DataValue.Value);
                        }
                    }
                }
            }
            return ETADirectFieldValuePair;
        }
         */


    }


}
