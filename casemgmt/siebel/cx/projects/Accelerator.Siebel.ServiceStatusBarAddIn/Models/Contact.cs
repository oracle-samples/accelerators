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
 *  SHA1: $Id: fa71ce09002905ecc92e9eacd64442e8ce34c890 $
 * *********************************************************************************************
 *  File: Contact.cs
 * *********************************************************************************************/

using Accelerator.Siebel.SharedServices.Providers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accelerator.Siebel.SharedServices
{
    public class Contact : ModelObjectBase
    {
        public static ISiebelProvider _provider;
        public static string ListLookupURL { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string ContactPartyID { get; set; }
        public string ContactOrgID { get; set; }
        public string ErrorMessage { get; set; }

        public Contact[] LookupList(string firstname, string lastname, string phone, string email, int _logIncidentId = 0, int _logContactId = 0)
        {
            Contact[] contactArr = null;
            //Switch Provider to call web service    
            contactArr = Contact._provider.LookupContactList(firstname, lastname, phone, email, _logIncidentId, _logContactId);
            return contactArr;
        }

        public static Dictionary<string, string> LookupDetail(IList<string> columns, string party_id, int _logIncidentId = 0, int _logContactId = 0)
        {
            return Contact._provider.LookupContactDetail(columns, party_id, _logIncidentId, _logContactId);
        }

        public static Dictionary<string, string> getDetailSchema()
        {
            return Contact._provider.getContactDetailSchema();
        }

        public static void InitSiebelProvider()
        {
            Type t = Type.GetType(ServiceProvider);

            try
            {
                _provider = Activator.CreateInstance(t) as ISiebelProvider;
                _provider.InitForContact(ListLookupURL, ServiceUsername, ServicePassword, ServiceClientTimeout);
                _provider.log = ConfigurationSetting.logWrap;
            }
            catch (Exception ex)
            {
                if (ConfigurationSetting.logWrap != null)
                {
                    string logMessage = "Error in init Provider in Contact Model. Error: " + ex.Message;
                    string logNote = "";
                    ConfigurationSetting.logWrap.DebugLog(logMessage: logMessage, logNote: logNote);
                }
                throw;
            }
        }

    }
}
