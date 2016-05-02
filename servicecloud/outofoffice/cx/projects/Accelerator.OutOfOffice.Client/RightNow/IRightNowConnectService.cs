/* * *******************************************************************************************
*  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC Out of Office Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 16.5 (May 2016) 
 *  reference: 150916-000080
 *  date: Thu Mar 17 23:37:53 PDT 2016
 
 *  revision: rnw-16-5-fixes-release-1
*  SHA1: $Id: b8ed2774135d89079cdd5f0e2c4b2e2397a598ba $
* *********************************************************************************************
*  File: IRightNowConnectService.cs
* ****************************************************************************************** */

using System;
using Accelerator.OutOfOffice.Client.Model;
using Accelerator.OutOfOffice.Client.RightNowProxyService;

namespace Accelerator.OutOfOffice.Client.RightNow
{
    public interface IRightNowConnectService
    {

        RightNowSyncPortClient GetRightNowClient();

        string GetRightNowEndPointURIHost();

        StaffAccount GetAccountDetails();

        bool updateCustomFields(StaffAccount staffAccount);
    }
}
