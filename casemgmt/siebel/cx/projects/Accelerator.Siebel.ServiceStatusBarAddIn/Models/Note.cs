/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC Contact Center + Siebel Case Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  Siebel release: 8.1.1.15
 *  reference: 150520-000047
 *  date: Mon Nov 30 20:14:30 PST 2015

 *  revision: rnw-15-11-fixes-release-2
 *  SHA1: $Id: 5170907e2eb9eee7b34ceea6238ab0f79210053d $
 * *********************************************************************************************
 *  File: Note.cs
 * *********************************************************************************************/

using Accelerator.Siebel.SharedServices.Providers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accelerator.Siebel.SharedServices
{
    public class Note : ModelObjectBase
    {
        public static ISiebelProvider _provider;
        public static string CreateInteractionURL { get; set; }

        public string ErrorMessage { get; set; }
        public string SrID { get; set; }
        public string Author { get; set; }
        public DateTime Created { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public string Channel { get; set; }
        public string NoteID { get; set; }
        public bool Status { get; set; }

        public bool Create(int _logIncidentId = 0, int _logContactId  = 0)
        {
            if (_provider == null)
            {
                throw new Exception("Siebel Provider not initialized.");
            }

            //Switch Provider to call web service
            Note note = Note._provider.CreateNote(this, _logIncidentId, _logContactId);
            this.NoteID = note.NoteID;
            this.ErrorMessage = note.ErrorMessage;

            return String.IsNullOrWhiteSpace(this.ErrorMessage);
        }

        public bool AddAttachment(int _logIncidentId = 0, int _logContactId = 0)
        {
            if (_provider == null)
            {
                throw new Exception("Siebel Provider not initialized.");
            }

            //Switch Provider to call web service
            Note note = Note._provider.CreateNoteAttachment(this, _logIncidentId, _logContactId);
            this.NoteID = note.NoteID;
            this.ErrorMessage = note.ErrorMessage;

            return String.IsNullOrWhiteSpace(this.ErrorMessage);
        }

        public static void InitSiebelProvider()
        {
            Type t = Type.GetType(ServiceProvider);

            try
            {
                _provider = Activator.CreateInstance(t) as ISiebelProvider;
                _provider.InitForNote(CreateInteractionURL, ServiceUsername, ServicePassword, ServiceClientTimeout);
                _provider.log = ConfigurationSetting.logWrap;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
