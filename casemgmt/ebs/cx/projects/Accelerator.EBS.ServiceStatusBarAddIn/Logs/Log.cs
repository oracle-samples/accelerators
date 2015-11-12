/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + EBS Enhancement
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  EBS release: 12.1.3
 *  reference: 150505-000099, 150420-000127
 *  date: Thu Nov 12 00:52:47 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: 5c8c65026ee72cca1a1ad0d078e7e470a4e70350 $
 * *********************************************************************************************
 *  File: Log.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accelerator.EBS.SharedServices.RightNowServiceReference;

namespace Accelerator.EBS.SharedServices.Logs
{
    public interface Log
    { 
        void ErrorLog(Incident incident = null, RightNowServiceReference.Contact contact = null, string LogMessage = null, string LogNote = null, string source = null, int timeElapsed = 0);
        void DebugLog(Incident incident = null, RightNowServiceReference.Contact contact = null, string LogMessage = null, string LogNote = null, string source = null, int timeElapsed = 0);
        void NoticeLog(Incident incident = null, RightNowServiceReference.Contact contact = null, string LogMessage = null, string LogNote = null, string source = null, int timeElapsed = 0);
        void ClickLog(Incident incident = null, RightNowServiceReference.Contact contact = null, string LogMessage = null, string LogNote = null, string source = null, int timeElapsed = 0);

    }
}
