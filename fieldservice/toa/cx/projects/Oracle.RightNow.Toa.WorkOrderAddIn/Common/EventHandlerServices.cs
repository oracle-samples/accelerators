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
 *  SHA1: $Id: 5fd33d04c0afba6480a18075b150047913a8164c $
 * *********************************************************************************************
 *  File: EventHandlerServices.cs
 * ****************************************************************************************** */

using Oracle.RightNow.Toa.Client.Model;
using Oracle.RightNow.Toa.Client.Rightnow;
using Oracle.RightNow.Toa.Client.RightNowProxyService;
using RightNow.AddIns.AddInViews;
using RightNow.AddIns.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.RightNow.Toa.Client.Logs;
using System.Windows;
using Oracle.RightNow.Toa.Client.Common;

namespace Oracle.RightNow.Toa.WorkOrderAddIn.Common
{
    public class EventHandlerServices
    {
        private static EventHandlerServices _eventHandlerService;
        private static IToaLog _log;

        public static EventHandlerServices GetEventHandlerService()
        {
            if (_eventHandlerService == null)
            {
                _eventHandlerService = new EventHandlerServices();
                _log = ToaLogService.GetLog();
                return _eventHandlerService;
            }
            return _eventHandlerService;
        }

        public WorkOrderModel GetWorkOrderModel(IRecordContext RecordContext)
        {
            ICustomObject workOrder = RecordContext.GetWorkspaceRecord(RecordContext.WorkspaceTypeName) as ICustomObject;
            IContact contact = RecordContext.GetWorkspaceRecord(WorkspaceRecordType.Contact) as IContact;

            WorkOrderModel workOrderModel = new WorkOrderModel();
            _log.Notice("Processing WorkOrder:" + workOrder.Id.ToString());
            if (workOrder != null)
            {
                IList<IGenericField> fields = workOrder.GenericFields;
                workOrderModel.AppointmentNumber = workOrder.Id.ToString();
                InventoryModel primaryAssetInventoryModel = null;
                int workorderTypeId = (Int32)getFieldFromWorOrderRecord(workOrder, "WO_Type");
                string[] workordertype = RightNowConnectService.GetService().GetWorkOrderTypeFromID(workorderTypeId);
                _log.Notice("WorkOrder Type ID:" + workordertype[0]);               
                foreach (IGenericField field in fields)
                {
                    if (field.DataValue.Value != null)
                    {
                        switch (field.Name)
                        {
                             case "Asset" :
                                //cannot use record context here as Asset WorkspaceType is not exposed through RightNow.AddIns.Common.
                                //TODO Un-Comment below lines when RN Connect Services are available
                                if (null != field.DataValue.Value)
                                {
                                    primaryAssetInventoryModel = new InventoryModel();
                                    int assetid = (int)field.DataValue.Value;
                                    _log.Notice("WorkOrder Primary Asset ID:" + assetid);
                                    string[] details = RightNowConnectService.GetService().GetAssetDetailsFromAssetID(assetid.ToString());
                                    string[] productDetails = RightNowConnectService.GetService().GetProductDetailsFromProductID(details[0]);//Type = Product's ID (i.e. SalesProduct.PartNumber)
                                    if (null != productDetails && productDetails.Length > 0)
                                    {
                                        primaryAssetInventoryModel.Type = productDetails[0];
                                    }
                                    else
                                    {
                                        _log.Warning("invtype_label is not valid for primary asset.");
                                    }
                                    if (details.Length > 1)
                                    {
                                        primaryAssetInventoryModel.SerialNumber = details[1]; //model = Serial Number
                                    }
                                }
                                break;
                            case "Case_Note":
                                workOrderModel.SetPropertyValue("XA_CASE_NOTES", field.DataValue.Value.ToString());
                                break;
                            case "Cancel_Reason":                                
                                workOrderModel.CancelReason = field.DataValue.Value.ToString();                                
                                break;
                            case "Contact":
                                if (contact != null)
                                {
                                    workOrderModel.CustomerNumber = contact.ID.ToString();
                                    _log.Notice("WorkOrder Contact ID:" + contact.ID.ToString());
                                    workOrderModel.Name = contact.NameFirst + " " + contact.NameLast;
                                }
                                break;
                            case "Contact_City":
                                workOrderModel.City = field.DataValue.Value.ToString();
                                break;
                            case "Contact_Email":
                                workOrderModel.EmailAddress = field.DataValue.Value.ToString();
                                break;
                            case "Contact_Mobile_Phone":
                                workOrderModel.MobileNumber = field.DataValue.Value.ToString();
                                break;
                            case "Contact_Phone":
                                workOrderModel.PhoneNumber = field.DataValue.Value.ToString();
                                break;
                            case "Contact_Postal_Code":
                                workOrderModel.ZipCode = field.DataValue.Value.ToString();
                                break;
                            case "Contact_Province_State":
                                int province_id = (Int32)field.DataValue.Value;
                                string name = RightNowConnectService.GetService().GetProvinceName(province_id);
                                if (name != null)
                                {
                                    workOrderModel.State = name;
                                }
                                break;
                            case "Contact_Street":
                                workOrderModel.Address = field.DataValue.Value.ToString();
                                break;
                            case "Duration":
                                if(null == field.DataValue.Value)
                                {
                                    if (workordertype[1].Equals("1"))
                                    {
                                        workOrderModel.Duration = workordertype[2];
                                    }
                                    _log.Notice("WorkOrder Duration :" + workOrderModel.Duration);
                                }else
                                {
                                    workOrderModel.Duration = field.DataValue.Value.ToString();
                                    _log.Notice("WorkOrder Duration :" + workOrderModel.Duration);
                                }
                                break;
                            case "Reminder_Time":
                                if (null != field.DataValue.Value)
                                {
                                    int id = Convert.ToInt32(field.DataValue.Value);
                                    string[] remindertime1 = RightNowConnectService.GetService().GetReminder_TimeFromID(id);
                                    workOrderModel.ReminderTime = Convert.ToInt32(remindertime1[0]);
                                }
                                
                                break;
                            case "Requested_Service_End":
                                workOrderModel.ServiceWindowEnd = field.DataValue.Value.ToString();
                                break;
                            case "Requested_Service_Start":
                                workOrderModel.ServiceWindowStart = field.DataValue.Value.ToString();
                                break;
                            case "Resolution_Due":
                                workOrderModel.SlaWindowEnd = Convert.ToDateTime(field.DataValue.Value);
                                break;
                           case "WO_Area":
                                workOrderModel.ExternalId = field.DataValue.Value.ToString();
                                break;
                            case "WO_Date":                                
                                workOrderModel.AssignedDate = ((DateTime)field.DataValue.Value).ToString("yyyy-MM-dd");
                                break;
                            case "WO_Status":
                                //Set it for each handler and not in generic code.
                                break;
                            case "WO_Time_Slot":
                                workOrderModel.TimeSlot = field.DataValue.Value.ToString();
                                break;
                            case "WO_Type":                                
                                workOrderModel.WorkType = workordertype[0];
                                break;
                        }
                    }
                }

                //Set Duration
                if (workOrderModel.Duration == null)
                {
                    if (workordertype[1].Equals("1"))
                    {
                        workOrderModel.Duration = workordertype[2];
                    }
                    _log.Notice("WorkOrder Duration :" + workOrderModel.Duration);
                }

                
                // Set Cancel Activity
                if (workOrderModel.CancelReason != null && !workOrderModel.CancelReason.Trim().Equals(""))
                {                    
                    workOrderModel.CommandType = Client.Common.ActivityCommandType.Cancel;
                }
                else // Set Update Activity
                {
                    workOrderModel.Status = Client.Common.ActivityStatus.Pending;
                    workOrderModel.CommandType = Client.Common.ActivityCommandType.Update;
                }
                _log.Notice("WorOrder Command Type is set as " + ToaStringsUtil.GetString(workOrderModel.CommandType));

                workOrderModel.SetActionIfCompleted = Client.Common.ActionIfCompleted.CreateIfAssignOrReschedule;       

                //TODO UnComment below code
                SetInventoryModel(workOrderModel, workOrder.Id, primaryAssetInventoryModel, workorderTypeId);
            }

            return workOrderModel;
        }

        private void SetInventoryModel(WorkOrderModel workOrderModel, int workOrderId, InventoryModel primaryAssetInventoryModel, int workOrderType)
        {
            List<InventoryModel> InventoryModelList = new List<InventoryModel>();

            if (null != primaryAssetInventoryModel && null != primaryAssetInventoryModel.Type)
            {
                InventoryModelList.Add(primaryAssetInventoryModel);
                _log.Notice("WorkOrder Primary Asset Type :" + primaryAssetInventoryModel.Type);
            }            

            //adding asset inventory model into workorder.
            if (InventoryModelList.Count > 0)
            {
                workOrderModel.ActivityInventories = InventoryModelList;
            }

            //Adding work Order Type Inventory into Required Inventory List
            List<RequiredInventoryModel> RequiredInventoryModelList = new List<RequiredInventoryModel>();
            //string salesProductQuery = String.Format("select Quantity, Serial_Number, WO_Inventory from TOA.WO_Type_Inventory where Work_Order_Type = {0}", workOrderModel.WorkType);
            string[] workOrder_type_inventories = RightNowConnectService.GetService().GetRequiredInventoryDetailsFromWorkOrderType(workOrderType);
            if (workOrder_type_inventories != null)
            {
                foreach (string workOrder_type_inventory in workOrder_type_inventories)
                {
                    string[] details = workOrder_type_inventory.Split('|');
                    RequiredInventoryModel reqInventoryModel = new RequiredInventoryModel();
                    reqInventoryModel.Quantity = details[1]; //Quantity = Quantity
                    reqInventoryModel.Model = details[2]; //model = Model
                    string[] productDetails = RightNowConnectService.GetService().GetProductDetailsFromProductID(details[0]);//Type = Product's ID (i.e. SalesProduct.PartNumber)
                    if (null != productDetails && productDetails.Length > 0)
                    {
                        reqInventoryModel.Type = productDetails[0];
                    }
                    else
                    {
                        _log.Warning("invtype_label is not valid for required inventory.");
                    }
                    
                    RequiredInventoryModelList.Add(reqInventoryModel);
                    _log.Notice("Adding WorkOrder Required Inventory with ID :" + workOrderId);
                }
            }

            if (RequiredInventoryModelList.Count > 0)
            {
                workOrderModel.RequiredInventories = RequiredInventoryModelList;
            }
        }

        public object getFieldFromWorOrderRecord(ICustomObject workOrderRecord, String fieldName)
        {
            object woType = null;
            IList<IGenericField> fields = workOrderRecord.GenericFields;
            if (null != fields)
            {
                foreach (IGenericField field in fields)
                {
                    if(field.Name.Equals(fieldName))
                    {
                        woType = field.DataValue.Value;
                        return woType;
                    }
                }
            }
            return woType;
        }
    }
}
