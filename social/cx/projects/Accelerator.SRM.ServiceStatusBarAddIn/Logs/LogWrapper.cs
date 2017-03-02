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
 *  SHA1: $Id: 25e788c3d1b1221cbadf8a026267241eb9e1eb76 $
 * *********************************************************************************************
 *  File: LogWrapper.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accelerator.SRM.SharedServices.RightNowServiceReference;
using System.Runtime.CompilerServices;
using System.Diagnostics; 

namespace Accelerator.SRM.SharedServices.Logs
{
    public class LogWrapper
    {
        private Log m_log;
        private Incident incident;
        private RightNowServiceReference.Contact contact;
        public LogWrapper(Log log)
        {
            m_log = log;

            incident = new Incident();
            ID incId = new ID();
            incId.id = 0;
            incident.ID = incId;

            contact = new RightNowServiceReference.Contact();
            ID contactId = new ID();
            contactId.id = 0;
            contact.ID = contactId;
        }


        public void ErrorLog(int incidentId = 0, int contactId = 0, string logMessage = null, string logNote = null, int timeElapsed = 0,
             [CallerMemberName] string memberName = "",
             [CallerFilePath] string sourceFilePath = "",
             [CallerLineNumber] int sourceLineNumber = 0)
        {
            Contact logContact = null;
            Incident logIncident = null;

            if (incidentId > 0)
            {
                incident.ID.id = incidentId;
                logIncident = incident;
            }

            if (contactId > 0)
            {
                contact.ID.id = contactId;
                logContact = contact;
            }

            string fileName = sourceFilePath.Substring(sourceFilePath.LastIndexOf('\\') + 1);
            string source = fileName + ":" + memberName + ":" + sourceLineNumber;
            m_log.ErrorLog(logIncident, logContact, logMessage, logNote, source, timeElapsed);
            ConfigurationSetting.updateStatusBar("Error", logMessage);
        }


        public void DebugLog(int incidentId = 0, int contactId = 0, string logMessage = null, string logNote = null, int timeElapsed = 0,
             [CallerMemberName] string memberName = "",
             [CallerFilePath] string sourceFilePath = "",
             [CallerLineNumber] int sourceLineNumber = 0)
        {
            Contact logContact = null;
            Incident logIncident = null;
            if (ConfigurationSetting.logLevel >= ConfigurationSetting.LogLevelEnum.Debug)
            {
                if (incidentId > 0)
                {
                    incident.ID.id = incidentId;
                    logIncident = incident;
                }

                if (contactId > 0)
                {
                    contact.ID.id = contactId;
                    logContact = contact;
                }

                string fileName = sourceFilePath.Substring(sourceFilePath.LastIndexOf('\\') + 1);
                string source = fileName + ":" + memberName + ":" + sourceLineNumber;
                m_log.DebugLog(logIncident, logContact, logMessage, logNote, source, timeElapsed);
            }
            ConfigurationSetting.updateStatusBar("Debug", logMessage);
        }

        public void NoticeLog(int incidentId = 0, int contactId = 0, string logMessage = null, string logNote = null, int timeElapsed = 0,
             [CallerMemberName] string memberName = "",
             [CallerFilePath] string sourceFilePath = "",
             [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (ConfigurationSetting.logLevel >= ConfigurationSetting.LogLevelEnum.Notice)
            {
                Contact logContact = null;
                Incident logIncident = null;

                if (incidentId > 0)
                {
                    incident.ID.id = incidentId;
                    logIncident = incident;
                }

                if (contactId > 0)
                {
                    contact.ID.id = contactId;
                    logContact = contact;
                }

                string fileName = sourceFilePath.Substring(sourceFilePath.LastIndexOf('\\') + 1);
                string source = fileName + ":" + memberName + ":" + sourceLineNumber;
                m_log.NoticeLog(logIncident, logContact, logMessage, logNote, source, timeElapsed);
            }
            ConfigurationSetting.updateStatusBar("Notice", logMessage);
        }

        public void ClickLog(int incidentId = 0, int contactId = 0, string logMessage = null, string logNote = null, int timeElapsed = 0,
             [CallerMemberName] string memberName = "",
             [CallerFilePath] string sourceFilePath = "",
             [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (ConfigurationSetting.logLevel >= ConfigurationSetting.LogLevelEnum.Click)
            {
                Contact logContact = null;
                Incident logIncident = null;

                if (incidentId > 0)
                {
                    incident.ID.id = incidentId;
                    logIncident = incident;
                }

                if (contactId > 0)
                {
                    contact.ID.id = contactId;
                    logContact = contact;
                }

                string fileName = sourceFilePath.Substring(sourceFilePath.LastIndexOf('\\') + 1);
                string source = fileName + ":" + memberName + ":" + sourceLineNumber;
                m_log.ClickLog(logIncident, logContact, logMessage, logNote, source, timeElapsed);
            }
            ConfigurationSetting.updateStatusBar("Click", logMessage);
        }
    }
}
