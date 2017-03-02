/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  Copyright (c) 2014, 2015, 2016, 2017 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + SRM Integration Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 17.2 (February 2017) 
 *  reference: 160628-000117
 *  date: Fri Feb 10 19:47:43 PST 2017
 
 *  revision: rnw-17-2-fixes-release-2
 *  SHA1: $Id: 3f667b00986fd3013fc0740d32718b429f833f62 $
 * *********************************************************************************************
 *  File: Log.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accelerator.SRM.SharedServices.RightNowServiceReference;

namespace Accelerator.SRM.SharedServices.Logs
{
    public interface Log
    { 
        void ErrorLog(Incident incident = null, Contact contact = null, string LogMessage = null, string LogNote = null, string source = null, int timeElapsed = 0);
        void DebugLog(Incident incident = null, Contact contact = null, string LogMessage = null, string LogNote = null, string source = null, int timeElapsed = 0);
        void NoticeLog(Incident incident = null, Contact contact = null, string LogMessage = null, string LogNote = null, string source = null, int timeElapsed = 0);
        void ClickLog(Incident incident = null, Contact contact = null, string LogMessage = null, string LogNote = null, string source = null, int timeElapsed = 0);

    }
}
