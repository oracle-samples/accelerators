/* * *******************************************************************************************
*  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: IoT OSvC Bi-directional Integration Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 16.11 (November 2016) 
 *  reference: 151217-000026
 *  date: Tue Dec 13 13:23:39 PST 2016
 
 *  revision: rnw-16-11-fixes-release
*  SHA1: $Id: a0e6412e0dc1e5dd42922d9a3373db7021141db3 $
* *********************************************************************************************
*  File: IRightNowConnectService.cs
* ****************************************************************************************** */

using Accelerator.IOTCloud.Client.RightNowProxyService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerator.IOTCloud.Client.RightNow
{
    public interface IRightNowConnectService
    {
        string GetRightNowConfigVerbValue(string configVerbName);

        RightNowSyncPortClient GetRightNowClient();

        string GetDeviceId(int incident_id);

        string GetRightNowEndPointURIHost();
    }
}
