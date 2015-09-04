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
 *  date: Thu Sep  3 23:13:59 PDT 2015

 *  revision: rnw-15-11-fixes-release-03
*  SHA1: $Id: e20d68d91d254c5ddccf20b9356b9299cbbd7cde $
* *********************************************************************************************
*  File: ToaEnumsUtil.cs
* ****************************************************************************************** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oracle.RightNow.Toa.Client.Common
{
    public static class ToaEnumsUtil
    {
        public static ActivityStatus GetActivityStatusEnum(string status)
        {
            var ret = ActivityStatus.None;
            switch (status)
            {
                case "started" :
                    ret = ActivityStatus.Started;
                    break;
                default:
                    break;
            }
            return ret;
        }

        public static ActivityCommandType GetActivityCommandTypeEnum(string commandType)
        {
            var ret = ActivityCommandType.None;
            switch (commandType)
            {
                case "start_activity" :
                    ret = ActivityCommandType.Start;
                    break;
                case "complete_activity" :
                    ret = ActivityCommandType.Complete;
                    break;
                case "notdone_activity" :
                    ret = ActivityCommandType.NotDone;
                    break;
                case "suspend_activity" :
                    ret = ActivityCommandType.Suspend;
                    break;
                case "update_activity":
                    ret = ActivityCommandType.Update;
                    break;
                case "cancel_activity" :
                    ret = ActivityCommandType.Cancel;
                    break;
                case "delete_activity" :
                    ret = ActivityCommandType.Delete;
                    break;
                case "update_appointment":
                    ret = ActivityCommandType.Update;
                    break;
                default:
                    break;
            }
            return ret;
        }

        public static InventoryCommandType GetString(string commandType)
        {
            var ret = InventoryCommandType.None;
            switch (commandType)
            {
                case "set_inventory" :
                    ret = InventoryCommandType.Set;
                    break;
                case "update_inventory":
                    ret = InventoryCommandType.Update;
                    break;
                case "delete_inventory":
                    ret = InventoryCommandType.Delete;
                    break;
                default:
                    break;
            }
            return ret;
        }

        public static ProcessingMode GetProcessingModeEnum(string mode)
        {
            var ret = ProcessingMode.None;
            switch (mode)
            {
                case "appointment_only" :
                    ret = ProcessingMode.Activity;
                    break;
                case "inventory_only" :
                    ret = ProcessingMode.Inventory;
                    break;
                default:
                    break;
            }
            return ret;
        }

        public static AllowChangeDate GetAllowChangeDateEnum(string allowChangeDate)
        {
            var ret = AllowChangeDate.None;
            switch (allowChangeDate)
            {
                case "yes":
                    ret = AllowChangeDate.Yes;
                    break;
                case "no":
                    ret = AllowChangeDate.Yes;
                    break;
                default:
                    break;
            }
            return ret;
        }

        public static PropertiesMode GetPropertiesModeEnum(string mode)
        {
            var ret = PropertiesMode.None;
            switch (mode)
            {
                case "replace":
                    ret = PropertiesMode.Replace;
                    break;
                case "update":
                    ret = PropertiesMode.Update;
                    break;
                default:
                    break;
            }
            return ret;
        }

        public static UploadType GetUploadTypeEnum(string uploadType)
        {
            var ret = UploadType.None;
            switch (uploadType)
            {
                case "full":
                    ret = UploadType.Full;
                    break;
                case "incremental":
                    ret = UploadType.Incremental;
                    break;
                default:
                    ret = UploadType.Full;
                    break;
            }
            return ret;
        }

        public static ActionIfCompleted GetActionIfCompletedEnum(string actionIfCompleted)
        {
            var ret = ActionIfCompleted.None;
            switch (actionIfCompleted)
            {
                case "create":
                    ret = ActionIfCompleted.Create;
                    break;
                case "ignore":
                    ret = ActionIfCompleted.Ignore;
                    break;
                case "update":
                    ret = ActionIfCompleted.Update;
                    break;
                default:
                    ret = ActionIfCompleted.CreateIfAssignOrReschedule;
                    break;
            }
            return ret;
        }
    }
}
