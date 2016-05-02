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
*  SHA1: $Id: eaeb9f0cc009606ed137fb6317a74abdef8585a4 $
* *********************************************************************************************
*  File: IOSvCLog.cs
* ****************************************************************************************** */

namespace Accelerator.OutOfOffice.Client.Logs
{
    public interface IOSvCLog
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
