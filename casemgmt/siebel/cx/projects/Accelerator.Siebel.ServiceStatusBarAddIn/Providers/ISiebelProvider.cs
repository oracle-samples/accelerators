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
 *  date: Mon Nov 30 20:14:30 PST 2015

 *  revision: rnw-15-11-fixes-release-2
 *  SHA1: $Id: e106aaea70905df5e98a995c849f5c5f92cf96d9 $
 * *********************************************************************************************
 *  File: ISiebelProvider.cs
 * *********************************************************************************************/

using Accelerator.Siebel.SharedServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerator.Siebel.SharedServices.Providers
{
    public interface ISiebelProvider
    {

        // Contact
        string ContactURL { get; set; }
        string ContactServiceUsername { get; set; }
        string ContactServicePassword { get; set; }
        int ContactServiceTimeout { get; set; } // come from ConfigurationSetting SiebelServiceTimeout, and pass in to InitForXx()
        void InitForContact(string contact_url, string user_name, string password, int timeout = -1);

        // SR
        string SRURL { get; set; }
        string SRServiceUsername { get; set; }
        string SRServicePassword { get; set; }
        int SRServiceTimeout { get; set; } // come from ConfigurationSetting SiebelServiceTimeout, and pass in to InitForXx()
        void InitForSR(string sr_url, string user_name, string password, int timeout = -1);

        string NoteURL { get; set; }
        string NoteServiceUsername { get; set; }
        string NoteServicePassword { get; set; }
        int NoteServiceTimeout { get; set; } // come from ConfigurationSetting SiebelServiceTimeout, and pass in to InitForXx()
        void InitForNote(string note_url, string user_name, string password, int timeout = -1);
        Logs.LogWrapper log { get; set; }

        ContactModel[] LookupContactList(string firstname, string lastname, string phone, string email, int _logIncidentId = 0, int _logContactId = 0);
        Dictionary<string, string> LookupContactDetail(IList<string> columns, string party_id, int _logIncidentId = 0, int _logContactId = 0);
        Dictionary<string, string> getContactDetailSchema();
        ServiceRequest[] LookupSRbyContactPartyID(IList<string> columns, string contact_id, int _logIncidentId = 0, int _logContactId = 0);
        Dictionary<string, string> getServiceRequestDetailSchema();
        Dictionary<string, string> LookupSRDetail(IList<string> columns, string srId, int _logIncidentId = 0, int _logContactId = 0);
        ServiceRequest LookupSR(string incident_id, int _logIncidentId = 0, int _logContactId = 0);
        ServiceRequest CreateSR(ServiceRequest sr, int _logIncidentId = 0, int _logContactId = 0);
        ServiceRequest UpdateSR(ServiceRequest sr, int _logIncidentId = 0, int _logContactId = 0);
        Note CreateNote(Note note, int _logIncidentId = 0, int _logContactId = 0);
        Note CreateNoteAttachment(Note note, int _logIncidentId = 0, int _logContactId = 0);
        Dictionary<string, string> getAssetSchema();
        Dictionary<string, string> LookupAssetDetail(IList<string> columns, string serialNum, string orgId, int _logIncidentId = 0, int _logContactId = 0);
        List<Dictionary<string, string>> LookupAssetList(IList<string> columns, string siebelContactId, int _logIncidentId = 0, int _logContactId = 0);
        Asset SerialNumberValidation(string serialNum, string orgId, int _logIncidentId = 0, int _logContactId = 0);
        Dictionary<string, string> getActivitySchema();
        List<Dictionary<string, string>> LookupActivityList(IList<string> columns, string siebelSrId, int _logIncidentId = 0, int _logContactId = 0);
        Dictionary<string, string> LookupActivityDetail(IList<string> columns, string siebelSrId, string siebelActvtyId, int _logIncidentId = 0, int _logContactId = 0);
        KeyValuePair<String, String> rnStatusToServerStatus(int rnStatusID);
        KeyValuePair<String, String> rnSeverityToServerSeverity(int rnSeverityID);
        KeyValuePair<String, String> rnRequestTypeToServerRequestType(int rnRequestTypeID);


        // Activity
        Activity CreateActivity(Activity activity, int _logIncidentId = 0, int _logContactId = 0);
        void InitForActivity(string activity_url, string user_name, string password, int timeout = -1);
    }
}
