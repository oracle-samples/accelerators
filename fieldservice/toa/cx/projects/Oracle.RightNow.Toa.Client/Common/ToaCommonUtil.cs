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
*  SHA1: $Id: c9bf43f5eafa7cc0a8f1a04f6e4faf5e8b715b21 $
* *********************************************************************************************
*  File: ToaCommonUtil.cs
* ****************************************************************************************** */

using Oracle.RightNow.Toa.Client.Rightnow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Oracle.RightNow.Toa.Client.Common
{
    public static class ToaCommonUtil
    {
        /// <summary>
        /// Validate site names
        /// </summary>
        /// <returns>bool based on if the site name and current site name is same or not</returns>
        public static bool ValidateCurrentSiteName()
        {
            string RightNowSiteNameFromConfig = RightNowConfigService.GetConfigValue(RightNowConfigKeyNames.RightnowHost);
            string CurrentRightNowSiteName = RightNowConnectService.GetService().GetRightNowEndPointURIHost();

            if (RightNowSiteNameFromConfig.Equals(CurrentRightNowSiteName))
            {
                return true;
            }
            
            MessageBox.Show(ToaExceptionMessages.IntegrationWithFieldServiceIsNotSetupForThisServiceSite, 
                ToaExceptionMessages.FieldServiceIntegrationConfigurationError, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        } 
    }
}
