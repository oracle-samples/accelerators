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
 *  date: Thu Sep  3 23:13:59 PDT 2015

 *  revision: rnw-15-11-fixes-release-03
*  SHA1: $Id: d1eedf9ed6941f276fc7bca7a91776ecaf716657 $
* *********************************************************************************************
*  File: ToaEnums.cs
* ****************************************************************************************** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oracle.RightNow.Toa.Client.Common
{
    public enum ActivityStatus
    {
        Pending,
        Cancelled,
        Started,
        Suspended,
        Complete,
        NotDone,
        Deleted,
        NotCreated,
        None
    }

    public enum ActivityCommandType
    {
        Update,
        Cancel,
        Start,
        Complete,
        NotDone,
        Suspend,
        Delete,
        None
    }

    public enum InventoryCommandType
    {
        Set,
        Update,
        Delete,
        None
    }

    public enum UploadType
    {
        Full,
        Incremental,
        None
    }

    public enum ProcessingMode
    {
        Activity,
        Inventory,
        None
    }

    public enum AllowChangeDate
    {
        Yes,
        No,
        None
    }

    public enum PropertiesMode
    {
        Replace,
        Update,
        None
    }

    public enum ActionIfCompleted
    {
        Ignore,
        Update,
        Create,
        CreateIfAssignOrReschedule,
        None
    }

    public enum PreferenceType
    {
        Required,
        Preferred,
        Forbidden,
        None
    }

    public enum ToaRequestResultCode
    {
        Success,
        Failure,
        Warnning,
        None
    }
}