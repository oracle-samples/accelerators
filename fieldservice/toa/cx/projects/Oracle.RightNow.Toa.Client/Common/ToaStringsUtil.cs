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
*  SHA1: $Id: df54e9e4886fe5e9a57b8e55eecf27a7261b6612 $
* *********************************************************************************************
*  File: ToaStringsUtil.cs
* ****************************************************************************************** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oracle.RightNow.Toa.Client.Common
{
    public static class ToaStringsUtil
    {        
        public static string GetString(ActivityStatus statuses)
        {
            string ret = "";
            switch (statuses)
            {
                case ActivityStatus.Cancelled:
                    ret = "cancelled";
                    break;
                case ActivityStatus.Started:
                    ret = "started";
                    break;
                case ActivityStatus.Suspended:
                    ret = "suspended";
                    break;
                case ActivityStatus.Complete:
                    ret = "complete";
                    break;
                case ActivityStatus.Pending:
                    ret = "pending";
                    break;
                case ActivityStatus.NotDone:
                    ret = "notdone";
                    break;
                case ActivityStatus.Deleted:
                    ret = "deleted";
                    break;
                case ActivityStatus.NotCreated:
                    ret = "activity create failed";
                    break;
                default:
                    break;
            }
            return ret;
        }

        public static string GetString(ActivityCommandType commandType)
        {
            string ret = "";
            switch (commandType)
            {
                case ActivityCommandType.Start:
                    ret = "start_activity";
                    break;
                case ActivityCommandType.Complete:
                    ret = "complete_activity";
                    break;
                case ActivityCommandType.NotDone:
                    ret = "notdone_activity";
                    break;
                case ActivityCommandType.Suspend:
                    ret = "suspend_activity";
                    break;
                case ActivityCommandType.Update:
                    ret = "update_activity";
                    break;
                case ActivityCommandType.Cancel:
                    ret = "cancel_activity";
                    break;
                case ActivityCommandType.Delete:
                    ret = "delete_activity";
                    break;
                default:
                    break;
            }
            return ret;
        }

        public static string GetString(InventoryCommandType commandType)
        {
            string ret = "";
            switch (commandType)
            {
                case InventoryCommandType.Set:
                    ret = "set_inventory";
                    break;
                default:
                    break;
            }
            return ret;
        }

        public static string GetString(ProcessingMode mode)
        {
            string ret = "";
            switch (mode)
            {
                case ProcessingMode.Activity:
                    ret = "appointment_only";
                    break;
                case ProcessingMode.Inventory:
                    ret = "inventory_only";
                    break;
                default:
                    break;
            }
            return ret;
        }

        public static string GetString(AllowChangeDate allowChangeDate)
        {
            string ret = "";
            switch (allowChangeDate)
            {
                case AllowChangeDate.Yes:
                    ret = "yes";
                    break;
                case AllowChangeDate.No:
                    ret = "no";
                    break;
                default:
                    break;
            }
            return ret;
        }

        public static string GetString(PropertiesMode mode)
        {
            string ret = "";
            switch (mode)
            {
                case PropertiesMode.Replace:
                    ret = "replace";
                    break;
                case PropertiesMode.Update:
                    ret = "update";
                    break;
                default:
                    break;
            }
            return ret;
        }

        public static string GetString(UploadType uploadType)
        {
            string ret = "";
            switch (uploadType)
            {
                case UploadType.Full:
                    ret = "full";
                    break;
                case UploadType.Incremental:
                    ret = "incremental";
                    break;
                default:
                    ret = "full";
                    break;
            }
            return ret;
        }

        public static string GetString(ActionIfCompleted actionIfCompleted)
        {
            
            string ret = "";
            switch (actionIfCompleted)
            {
                case ActionIfCompleted.Create:
                    ret = "create";
                    break;
                case ActionIfCompleted.Ignore:
                    ret = "ignore";
                    break;
                case ActionIfCompleted.Update:
                    ret = "update";
                    break;
                default:
                    ret = "create_if_reassign_or_reschedule";
                    break;
            }
            return ret;
        }
    }
}
