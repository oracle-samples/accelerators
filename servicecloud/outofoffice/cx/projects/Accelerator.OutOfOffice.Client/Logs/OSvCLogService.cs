/* * *******************************************************************************************
*  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC Out of Office Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 16.5 (May 2016) 
 *  reference: 150916-000080
 *  date: Thu Mar 17 23:37:53 PDT 2016
 
 *  revision: rnw-16-5-fixes-release-1
*  SHA1: $Id: c259ccdf851c57d08aeeced191f62c421b735390 $
* *********************************************************************************************
*  File: OSvCLogService.cs
* ****************************************************************************************** */

using System;
using System.Windows.Forms;
using Accelerator.OutOfOffice.Client.Common;

namespace Accelerator.OutOfOffice.Client.Logs
{
    public class OSvCLogService
    {
        private static object _sync = new object();
        private static IOSvCLog _log;        

        public static IOSvCLog GetLog()
        {
            if (_log != null)
                return _log;

            lock (_sync)
            {
                
                try
                {
                    if (_log == null)
                    {
                        _log = new IOSvCDefaultLog();
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
