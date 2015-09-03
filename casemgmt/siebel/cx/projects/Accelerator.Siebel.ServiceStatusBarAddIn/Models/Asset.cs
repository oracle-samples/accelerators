/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC Contact Center + Siebel Case Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.5 (May 2015)
 *  Siebel release: 8.1.1.15
 *  reference: 141216-000121
 *  date: Wed Sep  2 23:14:41 PDT 2015

 *  revision: rnw-15-8-fixes-release-01
 *  SHA1: $Id: f23122f3aef9b34e4200ea6fcc5c3ab0ffa92021 $
 * *********************************************************************************************
 *  File: Asset.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accelerator.Siebel.SharedServices.Providers;

namespace Accelerator.Siebel.SharedServices
{
    public class Asset : ModelObjectBase
    {
        public static string LookupURL { get; set; }

        public string AssetID { get; set; }
        public string AccountOrgID { get; set; }
        public string SerialNumber { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public string ErrorMessage { get; set; }

        public static ISiebelProvider _provider;

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
                    string logMessage = "Error in init Provider in Service Request Model. Error: " + ex.Message;
                    string logNote = "";
                    ConfigurationSetting.logWrap.DebugLog(logMessage: logMessage, logNote: logNote);
                }

                throw;
            }
        }


        public static Dictionary<string, string> getAssetSchema()
        {
            return Asset._provider.getAssetSchema();
        }

        public static Dictionary<string, string> LookupAsset(IList<string> columns, string serialNum, string orgId, int _logIncidentId = 0, int _logContactId = 0)
        {
            return Asset._provider.LookupAssetDetail(columns, serialNum, orgId, _logIncidentId, _logContactId);
        }

        public static List<Dictionary<string, string>> LookupAssetList(IList<string> columns, string siebelContactId, int _logIncidentId = 0, int _logContactId = 0)
        {
            return Asset._provider.LookupAssetList(columns, siebelContactId, _logIncidentId, _logContactId);
        }

        public static Asset SerialNumberValidation(string serialNum, string orgId, int _logIncidentId = 0, int _logContactId = 0)
        {
            return Asset._provider.SerialNumberValidation(serialNum, orgId, _logIncidentId, _logContactId);
        } 
    }
}
