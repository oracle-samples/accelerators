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
 *  date: Tue Dec 13 13:23:38 PST 2016
 
 *  revision: rnw-16-11-fixes-release
 *  SHA1: $Id: 7ce0397c944acfc01f7e3cf91d911d2cf2c6e398 $
 * *********************************************************************************************
 *  File: CommonUtil.cs
 * ****************************************************************************************** */
using System.Windows;
using Accelerator.IOTCloud.Client.Logs;
using Accelerator.IOTCloud.Client.RightNow;


namespace Accelerator.IOTCloud.Client.Model
{
    public static class CommonUtil
    {
        public static bool ValidateCurrentSiteName()
        {
            string RightNowSiteNameFromConfig = RightNowConfigService.GetConfigValue(RightNowConfigKeyNames.RIGHTNOW_HOST);
            string CurrentRightNowSiteName = RightNowConnectService.GetService().GetRightNowEndPointURIHost();
            if (RightNowSiteNameFromConfig.Equals(CurrentRightNowSiteName))
            {
                LogService.GetLog().Notice("Sitename validated!");
                return true;
            }

            LogService.GetLog()
                .Error(string.Format("RightNowSiteNameFromConfig [{0}] is different from CurrentRightNowSiteName[{1}]",
                    RightNowSiteNameFromConfig, CurrentRightNowSiteName));

            MessageBox.Show(ExceptionMessages.IOT_CONF_ERROR_MESSAGE, ExceptionMessages.IOT_CONF_ERROR_TITLE, MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
    }
}
