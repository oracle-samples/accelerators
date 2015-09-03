/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + EBS Enhancement
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.5 (May 2015)
 *  EBS release: 12.1.3
 *  reference: 150202-000157
 *  date: Wed Sep  2 23:11:40 PDT 2015

 *  revision: rnw-15-8-fixes-release-01
 *  SHA1: $Id: 9e44fc3afad487d7e50013434824981823ffdddd $
 * *********************************************************************************************
 *  File: IReport.cs
 * *********************************************************************************************/

using System;
using RightNow.AddIns.AddInViews;

namespace Accelerator.EBS.SharedServices
{
    public interface IReport
    {
        //Returns the type and value for a given virtual column
        Tuple<ReportColumnType, object> getVirtualColumnValue(string name);
    }
}
