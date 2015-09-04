/* * *******************************************************************************************
*  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + OFSC Reference Integration
 *  link: http://www-content.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.2 (Feb 2015)
 *  OFSC release: 15.2 (Feb 2015)
 *  reference: 150622-000130
 *  date: Thu Sep  3 23:14:00 PDT 2015

 *  revision: rnw-15-11-fixes-release-03
*  SHA1: $Id: 9a311bec924e0bb04d33cfff1a17fb69a331b7a0 $
* *********************************************************************************************
*  File: ToaLogService.cs
* ****************************************************************************************** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.RightNow.Toa.Client.Common;
using Oracle.RightNow.Toa.Client.Rightnow;

namespace Oracle.RightNow.Toa.Client.Logs
{
    public class ToaLogService
    {
        private static object _sync = new object();
        private static IToaLog _logWrapper;
        

        public static IToaLog GetLog()
        {
            if (_logWrapper != null)
                return _logWrapper;

            lock (_sync)
            {
                
                try
                {
                    if (_logWrapper == null)
                    {
                        _logWrapper = new ToaDefaultLog();
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(ToaExceptionMessages.LogServiceNotInitialized, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
            }

            return _logWrapper;
        }
    }
}
