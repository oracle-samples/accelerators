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
 *  date: Thu Sep  3 23:14:03 PDT 2015

 *  revision: rnw-15-11-fixes-release-03
*  SHA1: $Id: d0463cc1c43c0ba1ab49691326052717dc90f2d4 $
* *********************************************************************************************
*  File: ToaAutoClientAddIn.cs
* ****************************************************************************************** */

using System;
using RightNow.AddIns.AddInViews;
using System.AddIn;

namespace Oracle.RightNow.Toa.Client.Rightnow
{
    [AddIn("ToaAutoClient AddIn", Version = "1.0.0.0")]
    public class ToaAutoClientAddIn : IAutomationClient
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
