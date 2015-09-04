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
*  SHA1: $Id: c80864d66e3b255599bf24eee6f44459781a7f81 $
* *********************************************************************************************
*  File: IRightNowConnectService.cs
* ****************************************************************************************** */

using Oracle.RightNow.Toa.Client.RightNowProxyService;
using System;

namespace Oracle.RightNow.Toa.Client.Rightnow
{
    public interface IRightNowConnectService
    {
        string GetRightNowConfigVerbValue(string configVerbName);
        
        RightNowSyncPortClient GetRightNowClient();

        string[] GetWorkOrderTypeFromID(int workorderTypeId);

        string[] GetReminder_TimeFromID(int reminderTimeId);

        string[] GetResolutionDueFromID(int incidentId);

         string[] GetIncidentPrimaryAssetFromID(int assetId);         

         string[] GetAssetDetailsFromAssetID(string assetId);

         string[] GetRequiredInventoryDetailsFromWorkOrderType(int workOrderTypeId);

         string[] GetProductDetailsFromProductID(string productID);

        string GetProvinceName(int provinceId);

         string GetRightNowEndPointURIHost();
    }
}
