/* * *******************************************************************************************
*  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC Out of Office Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 16.5 (May 2016) 
 *  reference: 150916-000080
 *  date: Thu Mar 17 23:37:53 PDT 2016
 
 *  revision: rnw-16-5-fixes-release-1
*  SHA1: $Id: dceabb75f269c84b912f07312652e505999cbf27 $
* *********************************************************************************************
*  File: OutOfOfficeClientAddIn.cs
* ****************************************************************************************** */

using System.AddIn;
using RightNow.AddIns.AddInViews;

namespace Accelerator.OutOfOffice.Client
{
    [AddIn("OutOfOfficeClient AddIn", Version = "1.0.0.0")]    
    public class OutOfOfficeClientAddIn : IAutomationClient
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
            return true;
        }

        #endregion
    }
}
