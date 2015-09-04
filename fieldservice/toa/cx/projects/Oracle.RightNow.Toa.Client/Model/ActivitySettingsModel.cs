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
 *  date: Thu Sep  3 23:14:00 PDT 2015

 *  revision: rnw-15-11-fixes-release-03
*  SHA1: $Id: 427498c2e67f423dbbc93356ffde51fd9848a768 $
* *********************************************************************************************
*  File: ActivitySettingsModel.cs
* ****************************************************************************************** */

using Oracle.RightNow.Toa.Client.Common;
using Oracle.RightNow.Toa.Client.InboundProxyService;

namespace Oracle.RightNow.Toa.Client.Model
{
    public class ActivitySettingsModel
    {
        private string[] _keyFields;
        private ActionIfCompleted _actionIfCompleted;

        public ActivitySettingsModel()
        {
            _keyFields = new string[] {ActivityProperty.ApptNumber, ActivityProperty.CustomerNumber};
            _actionIfCompleted = ActionIfCompleted.CreateIfAssignOrReschedule;
        }

        public string[] KeyFields
        {
            get { return _keyFields; }
            set { _keyFields = value; }
        }

        public ActionIfCompleted ActionIfCompleted
        {
            get { return _actionIfCompleted; }
            set { _actionIfCompleted = value; }
        }

        internal AppointmentSettings GetActivitySettings()
        {
            var settings = new AppointmentSettings();
            settings.action_if_completed = ToaStringsUtil.GetString(_actionIfCompleted);
            settings.keys = _keyFields;
            return settings;
        }
    }
}
