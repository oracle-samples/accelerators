/* * *******************************************************************************************
*  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + OSC Lead Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.11 (November 2015)
 *  OSC release: Release 10
 *  reference: 150505-000122
 *  date: Tue Dec  1 21:42:19 PST 2015

 *  revision: rnw-15-11-fixes-release-2
*  SHA1: $Id: 008a450e930aba7da91c35c5f90f7900340967e9 $
* *********************************************************************************************
*  File: IRightNowConnectService.cs
* ****************************************************************************************** */

using Accelerator.SalesCloud.Client.RightNowProxyService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerator.SalesCloud.Client.RightNow
{
    public interface IRightNowConnectService
    {
        string GetRightNowConfigVerbValue(string configVerbName);

        RightNowSyncPortClient GetRightNowClient();

        string GetContactOrgExternalReference(int id, bool isContact, String applicationId);

        string GetRightNowEndPointURIHost();
    }
}
