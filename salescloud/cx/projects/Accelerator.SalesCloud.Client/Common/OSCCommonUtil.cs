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
 *  date: Tue Dec  1 21:42:18 PST 2015

 *  revision: rnw-15-11-fixes-release-2
 *  SHA1: $Id: f878eedf9d917355bf414c4e43ecc2ea081895f8 $
 * *********************************************************************************************
 *  File: OSCCommonUtil.cs
 * ****************************************************************************************** */
using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accelerator.SalesCloud.Client.RightNow;


namespace Accelerator.SalesCloud.Client.Common
{
    public static class OSCCommonUtil
    {
        public static bool ValidateCurrentSiteName()
        {
            string RightNowSiteNameFromConfig = RightNowConfigService.GetConfigValue(RightNowConfigKeyNames.RightnowHost);
            string CurrentRightNowSiteName = RightNowConnectService.GetService().GetRightNowEndPointURIHost();

            if (RightNowSiteNameFromConfig.Equals(CurrentRightNowSiteName))
            {
                return true;
            }

            MessageBox.Show(OSCExceptionMessages.OracleSalesIntegrationSiteWarningMessage, OSCExceptionMessages.OracleSalesIntegrationSiteWarningTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
    }
}
