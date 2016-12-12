/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + EBS Enhancement
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  EBS release: 12.1.3
 *  reference: 150505-000099, 150420-000127
 *  date: Thu Nov 12 00:52:48 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: 411d03a5ed0aa03a64ec9410e48433b1d2eb9ea1 $
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
    public class ContactModel : ModelObjectBase
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


        public ContactModel[] LookupList(string firstname, string lastname, string phone, string email, int _logIncidentId = 0, int _logContactId = 0)
        {
            ContactModel[] contactArr = null;
            //Switch Provider to call web service    
            contactArr = ContactModel._provider.LookupContactList(firstname, lastname, phone, email, _logIncidentId, _logContactId);
            return contactArr;
        }

        public static Dictionary<string, string> LookupDetail(decimal party_id, int _logIncidentId = 0, int _logContactId = 0)
        {
            return ContactModel._provider.LookupContactDetail(party_id, _logIncidentId, _logContactId);
        }

        public static Dictionary<string, string> getDetailSchema()
        {
            return ContactModel._provider.getContactDetailSchema();
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
                    ConfigurationSetting.logWrap.ErrorLog(logMessage: logMessage, logNote: logNote);
                }
                throw;
            }
        }

    }
}
