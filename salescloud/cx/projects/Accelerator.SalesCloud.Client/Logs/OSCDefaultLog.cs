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
 *  date: Tue Dec  1 21:42:18 PST 2015

 *  revision: rnw-15-11-fixes-release-2
*  SHA1: $Id: 23150d96923e27b4f776fa31376d2e9b4049d78b $
* *********************************************************************************************
*  File: OSCDefaultLog.cs
* ****************************************************************************************** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerator.SalesCloud.Client.Logs
{
    public class OSCDefaultLog : IOSCLog
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
