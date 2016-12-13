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
*  SHA1: $Id: 53071f5eff7efe84ddd76023cfb49597912ca225 $
* *********************************************************************************************
*  File: IOTClouddAutoClientAddIn.cs
* ****************************************************************************************** */

using System;
using RightNow.AddIns.AddInViews;
using System.AddIn;
using Accelerator.IOTCloud.Client.RightNow;

namespace Accelerator.IOTCloud.Client
{
    [AddIn("IOTCloudAutoClient AddIn", Version = "1.0.0.0")]    
    public class IOTClouddAutoClientAddIn : IAutomationClient
    {
        public static IGlobalContext GlobalContext { get; private set; }
        public static IAutomationContext AutoContext { get; private set; }

        #region IAutomationClient Members

        public void SetAutomationContext(IAutomationContext context)
        {
            AutoContext = context;
        }

        #endregion

        #region IAddInBase Members

        public bool Initialize(IGlobalContext context)
        {
            GlobalContext = context;

            return RightNowConfigService.Config();            
        }

        #endregion
    }
}
