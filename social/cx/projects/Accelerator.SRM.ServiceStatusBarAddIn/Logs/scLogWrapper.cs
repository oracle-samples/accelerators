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
 *  SHA1: $Id: d219b32b8fa7e681166d936a985a1c21139eed85 $
 * *********************************************************************************************
 *  File: scLogWrapper.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accelerator.SRM.SharedServices.RightNowServiceReference;

namespace Accelerator.SRM.SharedServices.Logs
{
    internal class SCLogWrapper : ServiceVentures.SCLog, Log
    {
        public RightNowSyncPortClient _rnowClient;
        private string _businessFunction;
        private string _host;

        public SCLogWrapper(string productExtSignature = null, string productExtName = null, string businessFunction = null)
            : base(productExtName, productExtName, default(DateTime), logLevel.Debug, businessFunction)
        {
            _rnowClient = ConfigurationSetting.client;
            _businessFunction = businessFunction;
            base.initializeLogger(_rnowClient);
            _host = ConfigurationSetting.host;
        }

        public void ClickLog(Incident incident = null, RightNowServiceReference.Contact contact = null, string LogMessage = null, string LogNote = null, string Source = null, int TimeElapsed = 0)
        {
            this.click(LogMessage, LogNote, incident, contact, Source, _businessFunction, TimeElapsed, _host);
        }

        public void ErrorLog(Incident incident = null, RightNowServiceReference.Contact contact = null, string LogMessage = null, string LogNote = null, string source = null, int timeElapsed = 0)
        {
            this.error(LogMessage, LogNote, incident, contact, source, _businessFunction, timeElapsed, _host);
        }

        public void DebugLog(Incident incident = null, RightNowServiceReference.Contact contact = null, string LogMessage = null, string LogNote = null, string source = null, int timeElapsed = 0)
        {
            this.debug(LogMessage, LogNote, incident, contact, source, _businessFunction, timeElapsed, _host);
        }

        public void NoticeLog(Incident incident = null, RightNowServiceReference.Contact contact = null, string LogMessage = null, string LogNote = null, string source = null, int timeElapsed = 0)
        {
            this.notice(LogMessage, LogNote, incident, contact, source, _businessFunction, timeElapsed, _host);
        }
    }
}
