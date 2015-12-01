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
 *  date: Mon Nov 30 19:59:37 PST 2015

 *  revision: rnw-15-11-fixes-release-2
 *  SHA1: $Id: d84fadbc6c70940f8bb385d89eaada9ee45ffe18 $
 * *********************************************************************************************
 *  File: LogWrapper.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accelerator.Siebel.SharedServices.RightNowServiceReference;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Accelerator.Siebel.SharedServices.Logs
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
            
            if (incidentId != 0)
            {
                incident.ID.id = incidentId;
                
            }else{
                incident.ID.id = 0;
            }
            
            if (contactId != 0)
            {
                contact.ID.id = contactId;
            }
            else
            {
                contact.ID.id = 0;
            }

            string fileName = sourceFilePath.Substring(sourceFilePath.LastIndexOf('\\') + 1);
            string source = fileName + ":" + memberName + ":" + sourceLineNumber;
            m_log.ErrorLog(incident, contact, logMessage, logNote, source, timeElapsed);
            ConfigurationSetting.updateStatusBar("Error", logMessage);
        }


        public void DebugLog(int incidentId = 0, int contactId = 0, string logMessage = null, string logNote = null, int timeElapsed = 0,
             [CallerMemberName] string memberName = "",
             [CallerFilePath] string sourceFilePath = "",
             [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (ConfigurationSetting.logLevel >= ConfigurationSetting.LogLevelEnum.Debug)
            {
                if (incidentId != 0)
                {
                    incident.ID.id = incidentId;

                }
                else
                {
                    incident.ID.id = 0;
                }

                if (contactId != 0)
                {
                    contact.ID.id = contactId;
                }
                else
                {
                    contact.ID.id = 0;
                }
                string fileName = sourceFilePath.Substring(sourceFilePath.LastIndexOf('\\') + 1);
                string source = fileName + ":" + memberName + ":" + sourceLineNumber;
                m_log.DebugLog(incident, contact, logMessage, logNote, source, timeElapsed);
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
                if (incidentId != 0)
                {
                    incident.ID.id = incidentId;

                }
                else
                {
                    incident.ID.id = 0;
                }

                if (contactId != 0)
                {
                    contact.ID.id = contactId;
                }
                else
                {
                    contact.ID.id = 0;
                }
                string fileName = sourceFilePath.Substring(sourceFilePath.LastIndexOf('\\') + 1);
                string source = fileName + ":" + memberName + ":" + sourceLineNumber;
                m_log.NoticeLog(incident, contact, logMessage, logNote, source, timeElapsed);
            }
            ConfigurationSetting.updateStatusBar("Notice", logMessage);
        }
    }
}
