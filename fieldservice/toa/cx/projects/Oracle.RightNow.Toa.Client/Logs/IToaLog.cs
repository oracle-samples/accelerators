/* * *******************************************************************************************
*  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + OFSC Reference Integration
 *  link: http://www-content.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.2 (Feb 2015)
 *  OFSC release: 15.2 (Feb 2015)
 *  reference: 150622-000130
 *  date: Thu Sep  3 23:14:00 PDT 2015

 *  revision: rnw-15-11-fixes-release-03
*  SHA1: $Id: 16e867e0a6e8db5a36bd6e50caa3cf56cd9ea45f $
* *********************************************************************************************
*  File: IToaLog.cs
* ****************************************************************************************** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oracle.RightNow.Toa.Client.Logs
{
    public interface IToaLog
    {
        void Debug(string logMessage, string logNote = null);
        void Notice(string logMessage, string logNote = null);
        void Error(string logMessage, string logNote = null);
        void Fatal(string logMessage, string logNote = null);
        void None(string logMessage, string logNote = null);
        void Click(string logMessage, string logNote = null);
        void Warning(string logMessage, string logNote = null);
    }
}
