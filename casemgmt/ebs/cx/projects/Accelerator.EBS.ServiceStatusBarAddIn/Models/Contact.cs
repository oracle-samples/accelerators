/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + EBS Enhancement
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.5 (May 2015)
 *  EBS release: 12.1.3
 *  reference: 150202-000157
 *  date: Wed Sep  2 23:11:40 PDT 2015

 *  revision: rnw-15-8-fixes-release-01
 *  SHA1: $Id: 9a69b1a1e5ae53a02d44004f995804c868241837 $
 * *********************************************************************************************
 *  File: Contact.cs
 * *********************************************************************************************/

using Accelerator.EBS.SharedServices.Providers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accelerator.EBS.SharedServices
{
    public class Contact : ModelObjectBase
    {
        public static string ListLookupURL { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public decimal? ContactPartyID { get; set; }
        public decimal? ContactOrgID { get; set; }
        public string ErrorMessage { get; set; }

        private static IEBSProvider _provider = null;


        public Contact[] LookupList(string firstname, string lastname, string phone, string email, int _logIncidentId = 0, int _logContactId = 0)
        {
            Contact[] contactArr = null;
            //Switch Provider to call web service    
            contactArr = Contact._provider.LookupContactList(firstname, lastname, phone, email, _logIncidentId, _logContactId);
            return contactArr;
        }

        public static Dictionary<string, string> LookupDetail(decimal party_id, int _logIncidentId = 0, int _logContactId = 0)
        {
            return Contact._provider.LookupContactDetail(party_id, _logIncidentId, _logContactId);
        }

        public static Dictionary<string, string> getDetailSchema()
        {
            return Contact._provider.getContactDetailSchema();
        }

        public static void InitEBSProvider()
        {
            Type t = Type.GetType(ServiceProvider);

            try
            {
                _provider = Activator.CreateInstance(t) as IEBSProvider;
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
