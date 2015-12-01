/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC Contact Center + Siebel Case Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  Siebel release: 8.1.1.15
 *  reference: 150520-000047
 *  date: Mon Nov 30 19:59:38 PST 2015

 *  revision: rnw-15-11-fixes-release-2
 *  SHA1: $Id: c9170beac3995910d6da214f2625e29aca7d4d65 $
 * *********************************************************************************************
 *  File: Activity.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accelerator.Siebel.SharedServices.Providers;

namespace Accelerator.Siebel.SharedServices
{
    public class Activity : ModelObjectBase
    {
        public static string LookupURL { get; set; }
        public string ErrorMessage { get; set; }
        public string ID { get; set; }
        public string SrID { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }
        public string ActivityType { get; set; }
        public System.DateTime Due { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }

        public static ISiebelProvider _provider;

        public bool Create(int _logIncidentId = 0, int _logContactId = 0)
        {
            if (_provider == null)
            {
                throw new Exception("Siebel Provider not initialized.");
            }

            Activity activity = Activity._provider.CreateActivity(this, _logIncidentId, _logContactId);
            this.ID = activity.ID;
            this.ErrorMessage = activity.ErrorMessage;

            return String.IsNullOrWhiteSpace(this.ErrorMessage);
        }

        //public static void InitSiebelProviderToCreateActivity()
        //{
        //    Type t = Type.GetType(ServiceProvider);

        //    try
        //    {
        //        // reuse InitForSR
        //        _provider = Activator.CreateInstance(t) as ISiebelProvider;
        //        _provider.InitForActivity(CreateURL, ServiceUsername, ServicePassword, ServiceClientTimeout);
        //        _provider.log = ConfigurationSetting.logWrap;
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ConfigurationSetting.logWrap != null)
        //        {
        //            string logMessage = "Error in init Provider in Activity Model. Error: " + ex.Message;
        //            string logNote = "";
        //            ConfigurationSetting.logWrap.ErrorLog(logMessage: logMessage, logNote: logNote);
        //        }

        //        throw;
        //    }
        //}


 
        public static void InitSiebelProvider()
        {
            Type t = Type.GetType(ServiceProvider);

            try
            {
                // reuse InitForSR
                _provider = Activator.CreateInstance(t) as ISiebelProvider;
                _provider.InitForSR(LookupURL, ServiceUsername, ServicePassword, ServiceClientTimeout);
                _provider.log = ConfigurationSetting.logWrap;
            }
            catch (Exception ex)
            {
                if (ConfigurationSetting.logWrap != null)
                {
                    string logMessage = "Error in init Provider in Activity Model. Error: " + ex.Message;
                    string logNote = "";
                    ConfigurationSetting.logWrap.ErrorLog(logMessage: logMessage, logNote: logNote);
                }

                throw;
            }
        }

        public static Dictionary<string, string> getActivitySchema()
        {
            return Activity._provider.getActivitySchema();
        }
     
        public static List<Dictionary<string, string>> LookupActivityList(IList<string> columns, string siebelSrId, int _logIncidentId = 0, int _logContactId = 0)
        {
            return Activity._provider.LookupActivityList(columns, siebelSrId, _logIncidentId, _logContactId);
        }

        public static Dictionary<string, string> LookupActivityDetail(IList<string> columns, string siebelSrId, string siebelActvtyId, int _logIncidentId = 0, int _logContactId = 0)
        {
            return Activity._provider.LookupActivityDetail(columns, siebelSrId, siebelActvtyId, _logIncidentId, _logContactId);
        }
       
    }
}
