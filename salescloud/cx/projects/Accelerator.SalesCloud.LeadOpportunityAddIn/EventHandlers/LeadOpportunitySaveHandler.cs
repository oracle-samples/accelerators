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
 *  SHA1: $Id: 377719dc1eff7f79b229094fd2516248e6838612 $
 * *********************************************************************************************
 *  File: LeadOpportunitySaveHandler.cs
 * ****************************************************************************************** */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RightNow.AddIns.AddInViews;
using Accelerator.SalesCloud.LeadOpportunityAddIn.ViewModel;
using RightNow.AddIns.Common;
using Accelerator.SalesCloud.Client.Common;
using Accelerator.SalesCloud.Client.Logs;
using System.Windows;

namespace Accelerator.SalesCloud.LeadOpportunityAddIn.EventHandlers
{
    public class LeadOpportunitySaveHandler : IHandler
    {
        private IRecordContext _recordContext;
        private IOSCLog logger;

        ICustomObject _leadOpportunityRecord { get; set; }
        private System.ComponentModel.CancelEventArgs cancelSaveOperation;

        public LeadOpportunitySaveHandler(IRecordContext RecordContext,System.ComponentModel.CancelEventArgs e)
        {
            _recordContext = RecordContext;
            _leadOpportunityRecord = _recordContext.GetWorkspaceRecord(_recordContext.WorkspaceTypeName) as ICustomObject;
            cancelSaveOperation = e;
            logger = OSCLogService.GetLog();
        }

        public void Handler()
        {
            LeadOpportunityViewModel leadOpportunityViewModel = new LeadOpportunityViewModel();
            IContact contactRecord = _recordContext.GetWorkspaceRecord(WorkspaceRecordType.Contact) as IContact;
            IOrganization orgRecord = _recordContext.GetWorkspaceRecord(WorkspaceRecordType.Organization) as IOrganization;

            long? orgExternalRef = null, contactExternalRef = null;
            int? contactId = null, orgID = null;
            if (orgRecord != null)
            {
                orgExternalRef = leadOpportunityViewModel.GetContactOrgExternalReference(orgRecord.ID, false, OracleRightNowOSCAddInNames.LeadOpportunityAddIn);
                orgID = orgRecord.ID;
            }
            if (contactRecord != null)
            {
                contactExternalRef = leadOpportunityViewModel.GetContactOrgExternalReference(contactRecord.ID, true, OracleRightNowOSCAddInNames.LeadOpportunityAddIn);
                contactId = contactRecord.ID;
            }

            //validate external refId
            logger.Debug("Validating request");
            if (!leadOpportunityViewModel.validateRequest(_recordContext, orgExternalRef, contactExternalRef, contactId, orgID,cancelSaveOperation))
            {
                logger.Debug("Request validation failed, see logs for more details");
                return;
            }
            logger.Debug("Request validation passed");
            long? ownerPartyId = null;
            if (orgExternalRef != null)
            {
                ownerPartyId = leadOpportunityViewModel.GetOwnerPartyId(orgExternalRef);
            }
            else if (contactExternalRef != null)
            {
                ownerPartyId = leadOpportunityViewModel.GetOwnerPartyIdForContact(contactExternalRef);
            }

            String opportunityName = (String)leadOpportunityViewModel.getFieldFromLeadOpportunity(_leadOpportunityRecord, OSCOpportunitiesCommon.LeadOpportunityLeadName);
            if (opportunityName != null)
            {
                opportunityName = opportunityName + " " + System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            }
            String leadType = (String)leadOpportunityViewModel.getFieldFromLeadOpportunity(_leadOpportunityRecord, OSCOpportunitiesCommon.LeadTypeFieldName);

            if (ownerPartyId == null)
            {
                logger.Debug("Invalid ownerPartyId. Lead Not Created in Sales Cloud");
                leadOpportunityViewModel.setFieldLeadOpportunity(_leadOpportunityRecord, OSCOpportunitiesCommon.LeadOpportunityStatusFieldName, OSCOpportunitiesCommon.LeadOpportunityFailedStatus);
                MessageBox.Show(OSCExceptionMessages.SalesAccountNotExistMessage,
                        OSCExceptionMessages.LeadNotCreatedTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (leadType.Equals(OSCOpportunitiesCommon.OpportunityRecordType)) 
            {
                long? opportunityId = null;
                if (ownerPartyId != null)
                {
                    opportunityId = leadOpportunityViewModel.CreateOpportunity(ownerPartyId, orgExternalRef, contactExternalRef, opportunityName);
                    logger.Debug("Got the opportunity Id " + opportunityId);
                }

                leadOpportunityViewModel.setFieldLeadOpportunity(_leadOpportunityRecord, OSCOpportunitiesCommon.LeadOpportunityStatusFieldName, opportunityId != null && opportunityId > 0 ? OSCOpportunitiesCommon.LeadOpportunitySuccessStatus : OSCOpportunitiesCommon.LeadOpportunityFailedStatus);
                if (opportunityId != null)
                {
                    leadOpportunityViewModel.setFieldLeadOpportunity(_leadOpportunityRecord, OSCOpportunitiesCommon.LeadOpportunityExtRef, opportunityId.ToString());
                    leadOpportunityViewModel.setFieldLeadOpportunity(_leadOpportunityRecord, OSCOpportunitiesCommon.LeadOpportunityLeadName, opportunityName);
                }
            }
            else if (leadType.Equals(OSCOpportunitiesCommon.SalesLeadRecordType))
            {
                long? leadId = null;
                if (ownerPartyId != null)
                {
                    leadId = leadOpportunityViewModel.CreateServiceLead(ownerPartyId, orgExternalRef, contactExternalRef, opportunityName);
                    logger.Debug("Got the lead Id " + leadId);
                }

                leadOpportunityViewModel.setFieldLeadOpportunity(_leadOpportunityRecord, OSCOpportunitiesCommon.LeadOpportunityStatusFieldName, leadId != null && leadId > 0 ? OSCOpportunitiesCommon.LeadOpportunitySuccessStatus : OSCOpportunitiesCommon.LeadOpportunityFailedStatus);
                if (leadId != null)
                {
                    leadOpportunityViewModel.setFieldLeadOpportunity(_leadOpportunityRecord, OSCOpportunitiesCommon.LeadOpportunityExtRef, leadId.ToString());
                    leadOpportunityViewModel.setFieldLeadOpportunity(_leadOpportunityRecord, OSCOpportunitiesCommon.LeadOpportunityLeadName, opportunityName);
                }
            }   
        }
    }
}
