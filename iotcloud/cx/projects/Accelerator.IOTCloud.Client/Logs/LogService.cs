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
 *  date: Tue Dec 13 13:23:39 PST 2016
 
 *  revision: rnw-16-11-fixes-release
*  SHA1: $Id: 7603deff06d91c4e0f04ab39f289b741788d9ef4 $
* *********************************************************************************************
*  File: LogService.cs
* ****************************************************************************************** */

using Accelerator.IOTCloud.Client.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Accelerator.IOTCloud.Client.Logs
{
    public class LogService
    {
        private static object _sync = new object();
        private static ILog _log;        

        public static ILog GetLog()
        {
            if (_log != null)
                return _log;

            lock (_sync)
            {
                
                try
                {
                    if (_log == null)
                    {
                        _log = new DefaultLog();
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(ExceptionMessages.LOG_SERVICE_NOT_INITIALIZED, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
            }

            return _log;
        }
    }
}
