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
*  SHA1: $Id: 7217fe87734900c27c1591c712e83ada503223f9 $
* *********************************************************************************************
*  File: DefaultLog.cs
* ****************************************************************************************** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerator.IOTCloud.Client.Logs
{
    public class DefaultLog : ILog
    {
        public void Debug(string logMessage, string logNote = null)
        {

        }

        public void Notice(string logMessage, string logNote = null)
        {

        }

        public void Error(string logMessage, string logNote = null)
        {

        }

        public void Fatal(string logMessage, string logNote = null)
        {

        }

        public void None(string logMessage, string logNote = null)
        {

        }

        public void Click(string logMessage, string logNote = null)
        {

        }

        public void Warning(string logMessage, string logNote = null)
        {

        }        
    }
}
