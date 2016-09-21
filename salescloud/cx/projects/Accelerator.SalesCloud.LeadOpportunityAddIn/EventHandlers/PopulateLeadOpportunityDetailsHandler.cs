/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015,2016 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + OSC Lead Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.11 (November 2015)
 *  OSC release: Release 10
 *  reference: 150505-000122, 160620-000160
 *  date: Mon Sep 19 02:05:29 PDT 2016

 *  revision: rnw-15-11-fixes-release-3
 *  SHA1: $Id: df11a38d55529c4413dc7495a730ed0ff5d5073f $
 * *********************************************************************************************
 *  File: PopulateLeadOpportunityDetailsHandler.cs
 * ****************************************************************************************** */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RightNow.AddIns.AddInViews;
using RightNow.AddIns.Common;
using Accelerator.SalesCloud.Client.RightNow;
using Accelerator.SalesCloud.Client.Common;

namespace Accelerator.SalesCloud.LeadOpportunityAddIn.EventHandlers
{
    public class PopulateLeadOpportunityDetailsHandler
    {
        /// <summary>
        /// The current workspace record context.
        /// </summary>
        /// 

        private IRecordContext _recordContext;

        ICustomObject _leadRecord { get; set; }

        public PopulateLeadOpportunityDetailsHandler(IRecordContext RecordContext)
        {
            _recordContext = RecordContext;
            _leadRecord = _recordContext.GetWorkspaceRecord(_recordContext.WorkspaceTypeName) as ICustomObject;
        }

        public void PopulateLeadDetails()
        {
            IContact contactRecord = _recordContext.GetWorkspaceRecord(WorkspaceRecordType.Contact) as IContact;
            IIncident incidentRecord = _recordContext.GetWorkspaceRecord(WorkspaceRecordType.Incident) as IIncident;
            string leadType = populateLeadType();
            IList<IGenericField> fields = _leadRecord.GenericFields;

            foreach (IGenericField field in fields)
            {
                switch (field.Name)
                {
                    case "lead_name":
                        if (contactRecord != null && incidentRecord != null)
                        {
                            string contactName = contactRecord.NameFirst + " " + contactRecord.NameLast;
                            if (leadType.Equals(OSCOpportunitiesCommon.OpportunityRecordType))
                            {
                                field.DataValue.Value = String.Format(OSCOpportunitiesCommon.OpportunityName, contactName, incidentRecord.RefNo);
                            }
                            else if (leadType.Equals(OSCOpportunitiesCommon.SalesLeadRecordType))
                            {
                                field.DataValue.Value = String.Format(OSCOpportunitiesCommon.SalesLeadName, contactName, incidentRecord.RefNo);
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Get the lead type
        /// </summary>
        /// <returns></returns>
        private string populateLeadType()
        {
            String leadOpptyType = RightNowConfigService.GetConfigValue(RightNowConfigKeyNames.OpportunityLeadType);
            if (String.IsNullOrWhiteSpace(leadOpptyType))
            {
                leadOpptyType = OSCOpportunitiesCommon.OpportunityRecordType;
            }
            IList<IGenericField> fields = _leadRecord.GenericFields;
            foreach (IGenericField field in fields)
            {
                switch (field.Name)
                {
                    case "lead_type":
                        field.DataValue.Value = leadOpptyType;
                        break;
                }
            }

            return leadOpptyType;
        }
    }
}
