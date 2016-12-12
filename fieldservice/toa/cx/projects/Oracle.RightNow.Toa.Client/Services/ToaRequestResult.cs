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
 *  date: Thu Sep  3 23:14:03 PDT 2015

 *  revision: rnw-15-11-fixes-release-03
*  SHA1: $Id: f9f266b7b482dba9005120d0ed2f4874c8806e40 $
* *********************************************************************************************
*  File: ToaRequestResult.cs
* ****************************************************************************************** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Oracle.RightNow.Toa.Client.Common;
using Oracle.RightNow.Toa.Client.Model;
using System.Windows.Forms;
using Oracle.RightNow.Toa.Client.InboundProxyService;
using Oracle.RightNow.Toa.Client.Logs;

namespace Oracle.RightNow.Toa.Client.Services
{
    public class ToaRequestResult
    {
        private List<IToaModel> _dataModels;
        private List<ReportMessageModel> _reportMessages;
        private ToaRequestResultCode _resultCode;
        private IToaLog _log;

        public ToaRequestResult()
        {
            _dataModels = new List<IToaModel>();
            _resultCode = ToaRequestResultCode.Failure;
            _log = ToaLogService.GetLog();
        }

        public ToaRequestResultCode ResultCode
        {
            get { return _resultCode; }
            set { _resultCode = value; }
        }

        public List<ReportMessageModel> ReportMessages
        {
            get { return _reportMessages; }
            internal set { _reportMessages = value; }
        }

        public List<IToaModel> DataModels
        {
            get { return _dataModels; }
            set { _dataModels = value; }
        }

        public void processResponseElement(InboundInterfaceResponseElement inboundResponseElement)
        {
            if (inboundResponseElement.report != null)
            {
                ReportMessageElement[] reportMessages = inboundResponseElement.report;
                WorkOrderModel workOrderModel = new WorkOrderModel();
                if (null != reportMessages && reportMessages.Length > 0)
                {
                    _log.Notice("Processing Report Element for WorkOrder");
                    foreach (ReportMessageElement reportMessage in reportMessages)
                    {
                        workOrderModel.AddReportMessage(reportMessage.result, reportMessage.type, reportMessage.code, reportMessage.description);
                        if (!reportMessage.code.Equals(ActivityProperty.TOA_Report_Success_Code))
                        {
                            _resultCode = Common.ToaRequestResultCode.Failure;
                            _log.Error("Error creating appointment ",
                                "Result:" + reportMessage.result + ", Type:" + reportMessage.type + ", Code:" + reportMessage.code + ", Description:" + reportMessage.description);
                        }
                    }
                    _log.Notice("Processing of Report Element for WorkOrder is done");
                }
                _dataModels.Add(workOrderModel);
            }
            else if (inboundResponseElement.data != null)
            {
                CommandResponseElement[] commands = inboundResponseElement.data.commands;
                _resultCode = Common.ToaRequestResultCode.Success;
                if (null != commands && commands.Length > 0)
                {
                    foreach (CommandResponseElement command in commands)
                    {
                        WorkOrderModel workOrderModel = new WorkOrderModel();
                        ReportMessageElement[] reportMessages = command.appointment.report;
                        if (null != reportMessages && reportMessages.Length > 0)
                        {
                            _log.Notice("Processing Report Element for WorkOrder");
                            foreach (ReportMessageElement reportMessage in reportMessages)
                            {
                                workOrderModel.AddReportMessage(reportMessage.result, reportMessage.type, reportMessage.code, reportMessage.description);
                                if (!reportMessage.code.Equals(ActivityProperty.TOA_Report_Success_Code))
                                {
                                    _resultCode = Common.ToaRequestResultCode.Failure;
                                    _log.Error("Error creating appointment:" + command.appointment.appt_number,
                                        "Result:" + reportMessage.result + ", Type:" + reportMessage.type + ", Code:" + reportMessage.code + ", Description:" + reportMessage.description);
                                }
                                else
                                {
                                    _log.Notice("Appointment created:" + command.appointment.appt_number,
                                        "Result:" + reportMessage.result + ", Type:" + reportMessage.type + ", Code:" + reportMessage.code + ", Description:" + reportMessage.description);
                                }

                            }
                            _log.Notice("Processing of Report Element for WorkOrder is done");
                        }
                        workOrderModel.AppointmentNumber = command.appointment.appt_number;
                        _log.Notice("WorkOrder Id:" + command.appointment.appt_number);
                        workOrderModel.TOA_AID = command.appointment.aid;
                        _log.Notice("ETA Direct Activity Id:" + command.appointment.aid);
                        workOrderModel.CustomerNumber = command.appointment.customer_number;
                        workOrderModel.CommandType = ToaEnumsUtil.GetActivityCommandTypeEnum(command.type);
                        workOrderModel.AssignedDate = command.date;
                        workOrderModel.ExternalId = command.external_id;

                        _dataModels.Add(workOrderModel);

                        //Processing Inventories
                        InventoryResponseElement[] inventoriesResponseElement = command.appointment.inventories;
                        if (null != inventoriesResponseElement && inventoriesResponseElement.Length > 0)
                        {
                            List<InventoryModel> inventoryResponseModels = new List<InventoryModel>();
                            foreach (InventoryResponseElement inventoryResponseElement in inventoriesResponseElement)
                            {
                                ReportMessageElement[] inventoryReportMessages = inventoryResponseElement.report;
                                InventoryModel responseInventoryModel = new InventoryModel();
                                if (inventoryResponseElement.invid > 0)
                                {
                                    responseInventoryModel.InventoryID = inventoryResponseElement.invid;
                                    _log.Notice("Inventory ID is:" + inventoryResponseElement.invid);
                                }

                                _log.Notice("Processing Report Element for Inventory");
                                if (null != inventoryReportMessages)
                                {
                                    foreach (ReportMessageElement reportMessage in inventoryReportMessages)
                                    {
                                        responseInventoryModel.AddReportMessage(reportMessage.result, reportMessage.type, reportMessage.code, reportMessage.description);
                                        if (!reportMessage.code.Equals(ActivityProperty.TOA_Report_Success_Code))
                                        {
                                            _resultCode = Common.ToaRequestResultCode.Failure;
                                            _log.Error("Error creating/updating inventory",
                                       "Result:" + reportMessage.result + ", Type:" + reportMessage.type + ", Code:" + reportMessage.code + ", Description:" + reportMessage.description);
                                        }
                                        else
                                        {
                                            _log.Notice("Inventory added/updated",
                                        "Result:" + reportMessage.result + ", Type:" + reportMessage.type + ", Code:" + reportMessage.code + ", Description:" + reportMessage.description);
                                        }
                                    }
                                }

                                _log.Notice("Processing of Report Element for Inventory is done");
                                if (null != inventoryResponseElement.properties)
                                {
                                    foreach (PropertyElement inventoryProperty in inventoryResponseElement.properties)
                                    {
                                        switch (inventoryProperty.label)
                                        {
                                            case "invsn":
                                                responseInventoryModel.SerialNumber = inventoryProperty.value;
                                                _log.Notice("Inventory Serial Number is:" + inventoryProperty.value);
                                                break;
                                            case "invtype_label":
                                                responseInventoryModel.Type = inventoryProperty.value;
                                                _log.Notice("Inventory Type is:" + inventoryProperty.value);
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                }
                                //Adding individual inventory to list
                                inventoryResponseModels.Add(responseInventoryModel);
                            }
                            //Adding list to WorkOrder Model
                            workOrderModel.ActivityInventories = inventoryResponseModels;
                        }
                    }
                }
            }
        }
    }
}

        