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
 *  date: Thu Sep  3 23:14:03 PDT 2015

 *  revision: rnw-15-11-fixes-release-03
*  SHA1: $Id: eba46a88c6f72de75e27b8db51757902d0161b0c $
* *********************************************************************************************
*  File: WorkOrderService.cs
* ****************************************************************************************** */

using System;
using System.Collections.Generic;
using Oracle.RightNow.Toa.Client.ActivityProxyService;
using Oracle.RightNow.Toa.Client.Common;
using Oracle.RightNow.Toa.Client.Exceptions;
using Oracle.RightNow.Toa.Client.Logs;
using Oracle.RightNow.Toa.Client.Model;
using Oracle.RightNow.Toa.Client.Rightnow;

namespace Oracle.RightNow.Toa.Client.Services
{
    /// <summary>
    /// A wrapper service class for "activity" web serivce client 
    /// </summary>
    public class WorkOrderService : IWorkOrderService
    {
       
        // Todo: Fetch config verb info and initialize activity object, this service should stateless
        private ApiPortTypeClient _activityClient;
        private IToaLog _log;
       
        public static WorkOrderService GetService()
        {
            //RightNowConfigService.Config();
            var workOrderService = new WorkOrderService();
            //var workOrderUrl = RightNowConfigService.GetConfigValue(RightNowConfigKeys.ToaActivityWsdlUrlKey);
            workOrderService._activityClient = new ApiPortTypeClient(); //Todo: add endpoint address
            workOrderService._log = ToaLogService.GetLog();
            return workOrderService;
        }

        /// <summary>
        /// Private Workorder Service constructor
        /// </summary>
        private WorkOrderService()
        {
            
        }
        
        /// <summary>
        /// Get activity 
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public void GetActivity(string activityId, ActivityServiceDelegate activityCallback)
        {

            _log.Debug("Enter - GetActivity()");
            if (activityId == null || activityId.Trim().Equals("") || activityCallback == null)
                return;
            var backgroundService = new ToaBackgroundServiceUtil();
            /*backgroundService.RunAsync(() =>
                { */
                    try
                    {
                        var activityModel = new WorkOrderModel();
                        var getActivityParam = new get_activity_parameters();
                        getActivityParam.activity_id = activityId;
                        getActivityParam.user = ToaUserUtil.GetActivityUser();
                        activity_response response = _activityClient.get_activity(getActivityParam);

                        // initialize  toa result and activity model object
                        var toaRequestResult = new ToaRequestResult();
                        toaRequestResult.DataModels.Add(activityModel);

                        activityCallback.Invoke(toaRequestResult);
                        
                    }
                    catch(Exception exception)
                    {
                        // Todo: logg exception
                    }
                     
                /*});*/
            _log.Debug("Exit - GetActivity()");
        }
    }
}
