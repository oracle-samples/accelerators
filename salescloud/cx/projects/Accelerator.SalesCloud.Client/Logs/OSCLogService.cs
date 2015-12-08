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
*  SHA1: $Id: c24107b5f92e7afb558433ded3e2cc74553e69bc $
* *********************************************************************************************
*  File: OSCLogService.cs
* ****************************************************************************************** */

using Accelerator.SalesCloud.Client.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Accelerator.SalesCloud.Client.Logs
{
    public class OSCLogService
    {
        private static object _sync = new object();
        private static IOSCLog _log;        

        public static IOSCLog GetLog()
        {
            if (_log != null)
                return _log;

            lock (_sync)
            {
                
                try
                {
                    if (_log == null)
                    {
                        _log = new OSCDefaultLog();
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(OSCExceptionMessages.LogServiceNotInitialized, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
            }

            return _log;
        }
    }
}
