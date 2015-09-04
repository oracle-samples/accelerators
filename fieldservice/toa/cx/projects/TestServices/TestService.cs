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
 *  date: Mon Aug 24 09:01:22 PDT 2015

 *  revision: rnw-15-11-fixes-release-01
*  SHA1: $Id: 6b348ef4f0f18b4d2e033445b84e645051172cd2 $
* *********************************************************************************************
*  File: TestService.cs
* ****************************************************************************************** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.RightNow.Toa.Client.Common;
using Oracle.RightNow.Toa.Client.Exceptions;
using Oracle.RightNow.Toa.Client.Rightnow;
using Oracle.RightNow.Toa.Client.Services;
using Oracle.RightNow.Toa.WorkOrderAddIn.EventHandlers;
using Oracle.RightNow.Toa.Client.RightNowProxyService;
using Oracle.RightNow.Toa.Client.Logs;

namespace TestServices
{
    public class TestService
    {
        private static void Main(string[] args)
        {            
            //RightNowConfigService.Config();          
            //RightNowConnectService.GetService();

            //QueryResultData[] result = RightNowConnectService.GetService().GetProductCatalogDetailsFromId(21);
            //Console.WriteLine("Data:"+result);
            var service = RightNowConnectService.GetService();
            var config = service.GetRightNowConfigVerbValue("CUSTOM_CFG_Sales_Accel_Integrations");
            var log = ToaLogService.GetLog();

            log.Debug("Deepak: Hello Word!! : 11");
            log.Error("Deepak: Hello Word!! : 12");
            log.Fatal("Deepak: Hello Word!! : 13");
            log.Notice("Deepak: Hello Word!! : 14");
            log.Warning("Deepak: Hello Word!! : 15");
            log.Click("Deepak: Hello Word!! : 16");
        }
    }
}