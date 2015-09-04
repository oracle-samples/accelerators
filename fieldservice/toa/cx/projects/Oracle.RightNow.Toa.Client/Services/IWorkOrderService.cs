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
 *  date: Thu Sep  3 23:14:02 PDT 2015

 *  revision: rnw-15-11-fixes-release-03
*  SHA1: $Id: a09cc1028ed028d03933b4eef357d1e9bae9ead7 $
* *********************************************************************************************
*  File: IWorkOrderService.cs
* ****************************************************************************************** */

using System;
using System.Collections.Generic;
using Oracle.RightNow.Toa.Client.Common;
using Oracle.RightNow.Toa.Client.Model;

namespace Oracle.RightNow.Toa.Client.Services
{
    public delegate void ActivityServiceDelegate(ToaRequestResult result);
    /// <summary>
    /// Activity Management Service Inteface
    /// </summary>
    public interface IWorkOrderService
    {
        /// <summary>
        /// Get activity
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        void GetActivity(string activityId, ActivityServiceDelegate activityCallback);
    }
}
