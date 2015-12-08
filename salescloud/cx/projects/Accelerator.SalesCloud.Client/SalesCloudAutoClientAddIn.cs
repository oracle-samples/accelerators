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
 *  date: Tue Dec  1 21:42:20 PST 2015

 *  revision: rnw-15-11-fixes-release-2
*  SHA1: $Id: d456b85808977082abd75c453cfd4b6711da8a5e $
* *********************************************************************************************
*  File: SalesCloudAutoClientAddIn.cs
* ****************************************************************************************** */

using System;
using RightNow.AddIns.AddInViews;
using System.AddIn;
using Accelerator.SalesCloud.Client.RightNow;

namespace Accelerator.SalesCloud.Client
{
    [AddIn("SalesCloudAutoClient AddIn", Version = "1.0.0.0")]    
    public class SalesCloudAutoClientAddIn : IAutomationClient
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
