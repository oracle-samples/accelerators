/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC Contact Center + Siebel Case Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  Siebel release: 8.1.1.15
 *  reference: 150520-000047
 *  date: Thu Nov 12 00:55:36 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: 1b9bd7177019a3b426e4d28aa6ff81b8d77bfe7f $
 * *********************************************************************************************
 *  File: Log.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accelerator.Siebel.SharedServices.RightNowServiceReference;

namespace Accelerator.Siebel.SharedServices.Logs
{
    public interface Log
    {
        void ErrorLog(Incident incident = null, RightNowServiceReference.Contact contact = null, string LogMessage = null, string LogNote = null, string source = null, int timeElapsed = 0);
        void DebugLog(Incident incident = null, RightNowServiceReference.Contact contact = null, string LogMessage = null, string LogNote = null, string source = null, int timeElapsed = 0);
        void NoticeLog(Incident incident = null, RightNowServiceReference.Contact contact = null, string LogMessage = null, string LogNote = null, string source = null, int timeElapsed = 0);
    }
}
