/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + OSC Lead Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.11 (November 2015)
 *  OSC release: Release 10
 *  reference: 150505-000122
 *  date: Tue Dec  1 21:42:22 PST 2015

 *  revision: rnw-15-11-fixes-release-2
 *  SHA1: $Id: ed782e8bfe3809285e3659c39590b9f5ccc63f58 $
 * *********************************************************************************************
 *  File: LeadOpportunityViewModel.cs
 * ****************************************************************************************** */
using Accelerator.SalesCloud.Client.Model;
using Accelerator.SalesCloud.Client.RightNow;
using Accelerator.SalesCloud.Client.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RightNow.AddIns.Common;
using RightNow.AddIns.AddInViews;
using Accelerator.SalesCloud.Client.Common;
using Accelerator.SalesCloud.Client.Logs;
using System.Windows;
using Oracle.RightNow.OSC.Client.Services;
using Oracle.RightNow.OSC.Client.Model;

namespace Accelerator.SalesCloud.LeadOpportunityAddIn.ViewModel
{
    public class LeadOpportunityViewModel
    {
        private IOSCLog logger;

        public LeadOpportunityViewModel()
        {
            logger = OSCLogService.GetLog();
        }

        /// <summary>
        /// Get ExternalReference for Contact/Org
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isContact"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public long GetContactOrgExternalReference(int id, bool isContact, String applicationId)
        {
            IRightNowConnectService rightNowConnect = RightNowConnectService.GetService();
            String extRef = rightNowConnect.GetContactOrgExternalReference(id, isContact, applicationId);
            long externalRef = Convert.ToInt64((String.IsNullOrWhiteSpace(extRef) ? "-1": extRef));
            return externalRef;
        }

        /// <summary>
        /// Get the OwnerPartyId
        /// </summary>
        /// <param name="partyId"></param>
        /// <returns></returns>
        public long? GetOwnerPartyId(long? partyId)
        {
            IAccountService accountService = AccountService.GetService();
            AccountModel accountModel = new AccountModel();
            accountModel.PartyId = (long)partyId;
            AccountModel accountModelResult = accountService.GetOwnerPartyId(accountModel);
            return accountModelResult.OwnerPartyId;
        }

        /// <summary>
        /// Get the OwnerPartyId for Contact
        /// </summary>
        /// <param name="partyId"></param>
        /// <returns></returns>
        public long? GetOwnerPartyIdForContact(long? partyId)
        {
            IContactService contactService = ContactService.GetService();
            ContactModel contactModel = new ContactModel();
            contactModel.PartyId = (long)partyId;
            ContactModel contactModelResult = contactService.GetOwnerPartyId(contactModel);
            return contactModelResult.OwnerPartyId;
        }

        /// <summary>
        /// Create an opportunity
        /// </summary>
        /// <param name="ownerPartyId"></param>
        /// <param name="orgExternalRef"></param>
        /// <param name="contactExternalRef"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public long? CreateOpportunity(long? ownerPartyId, long? orgExternalRef, long? contactExternalRef, string name)
        {
            IOpportunityService service = OpportunityService.GetService();
            OpportunityModel model = new OpportunityModel();
            model.Name = name;

            if (orgExternalRef != null)
            {
                model.TargetPartyId = (long)orgExternalRef;
                model.TargetPartyIdSpecified = true;
            }
            
            model.OwnerResourcePartyId = (long)ownerPartyId;
            model.OwnerResourcePartyIdSpecified = true;
            model.KeyContactId = contactExternalRef;
            model.KeyContactIdSpecified = true;

            OpportunityResourceModel resourceModel = new OpportunityResourceModel();
            resourceModel.OwnerFlag = true;
            resourceModel.OwnerFlagSpecified = true;
            resourceModel.ResourceId = (long)ownerPartyId;
            resourceModel.ResourceIdSpecified = true;

            model.OpportunityResourceModel = resourceModel;

            OpportunityModel result = service.CreateOpportunity(model);
            if (result != null && result.OpportunityId != null)
            {
                return result.OpportunityId;
            }
            return null;
        }

        /// <summary>
        /// Create a Service Lead 
        /// </summary>
        /// <param name="ownerPartyId"></param>
        /// <param name="orgExternalRef"></param>
        /// <param name="contactExternalRef"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public long? CreateServiceLead(long? ownerPartyId, long? orgExternalRef, long? contactExternalRef, string name)
        {
            ILeadService service = LeadService.GetService();
            LeadModel model = new LeadModel();
            model.Name = name;

            if (orgExternalRef != null)
            {
                model.CustomerId = (long)orgExternalRef;
                model.CustomerIdSpecified = true;
            }
            
            model.OwnerId = (long)ownerPartyId;
            model.OwnerIdSpecified = true;
            model.PrimaryContactId = (long)contactExternalRef;
            model.PrimaryContactIdSpecified = true;

            LeadModel result = service.CreateServiceLead(model);

            if (result != null && result.LeadId != null)
            {
                return result.LeadId;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="recordContext"></param>
        /// <param name="orgExternalRef"></param>
        /// <param name="contactExternalRef"></param>
        /// <param name="contactID"></param>
        /// <param name="orgID"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool validateRequest(IRecordContext recordContext, long? orgExternalRef, long? contactExternalRef, int? contactID, int? orgID ,System.ComponentModel.CancelEventArgs e)
        {
            logger.Debug("ValidateRequest Start");
            bool isSuccess = true;
            logger.Debug("Validate Create or Update");
            //Validate Record Status
            isSuccess = validateCreateOrUpdate(recordContext);
            logger.Debug("Validate Create or Update done. Validation status="+isSuccess);
            if (!isSuccess)
            {
                //validation failed, cancel save operation
                e.Cancel = true;
                return isSuccess;
            }
            logger.Debug("Validate opportunity type");
            //Validate lead_oppty_type
            isSuccess = validateLeadOpportunityType(recordContext);
            logger.Debug("validate opportunity type done. validation status="+isSuccess);
            if (!isSuccess)
            {
                //Set submit_status to failed
                //ICustomObject leadOpportunityObject = recordContext.GetWorkspaceRecord(recordContext.WorkspaceTypeName) as ICustomObject;
                //setFieldLeadOpportunity(leadOpportunityObject, OSCOpportunitiesCommon.LeadOpportunityStatusFieldName, (object)OSCOpportunitiesCommon.LeadOpportunityFailedStatus);
                //validation failed, cancel save operation
                e.Cancel = true;
                return isSuccess;
            }
            logger.Debug("Validate contact is mandatory or not");
            //Validate Contact mandatory
            isSuccess = validateMandatoryContact(contactID);
            logger.Debug("Validation mandatory contact is done. Validation status="+isSuccess);
            if (!isSuccess)
            {
                //validation failed, cancel save operation
                e.Cancel = true;
                return isSuccess;
            }
            logger.Debug("Validate external reference");
            //Validate External Reference
            isSuccess = validateExternalReference(recordContext, orgExternalRef, contactExternalRef, orgID);
            if (!isSuccess)
            {
                //Set submit_status to failed
                ICustomObject leadOpportunityObject = recordContext.GetWorkspaceRecord(recordContext.WorkspaceTypeName) as ICustomObject;
                setFieldLeadOpportunity(leadOpportunityObject, OSCOpportunitiesCommon.LeadOpportunityStatusFieldName, (object)OSCOpportunitiesCommon.LeadOpportunityFailedStatus);
            }
            logger.Debug("Validate external reference done. validation status="+isSuccess);
            return isSuccess;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="recordContext"></param>
        /// <returns></returns>
        private bool validateCreateOrUpdate(IRecordContext recordContext)
        {
            ICustomObject leadOpportunityRecord = recordContext.GetWorkspaceRecord(recordContext.WorkspaceTypeName) as ICustomObject;
            String leadOpportunityExtRef = (String)getFieldFromLeadOpportunity(leadOpportunityRecord, OSCOpportunitiesCommon.LeadOpportunityExtRef);
            logger.Debug("Lead/Opportunity ExtRef is "+leadOpportunityExtRef);
            if (!String.IsNullOrWhiteSpace(leadOpportunityExtRef) && !leadOpportunityExtRef.Equals(OSCOpportunitiesCommon.DefaultOpportunitySalesLeadID.ToString(),StringComparison.CurrentCultureIgnoreCase))
            {
                //This  is update
                MessageBox.Show(OSCExceptionMessages.LeadOpportunityUpdateNotSupportedMessage,
                   OSCExceptionMessages.LeadOpportunityUpdateNotSupportedTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
            //This is create
            logger.Debug("This is create Opportunity or Lead");
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contactID"></param>
        /// <returns></returns>
        private bool validateMandatoryContact(int? contactID)
        {
            //Contact is mandatory.
            if (null == contactID)
            {
                MessageBox.Show(OSCExceptionMessages.ContactMandatoryMessage, OSCExceptionMessages.LeadNotCreatedTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            logger.Debug("Contact is specified");
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="recordContext"></param>
        /// <param name="orgExternalRef"></param>
        /// <param name="contactExternalRef"></param>
        /// <param name="orgID"></param>
        /// <returns></returns>       
        private bool validateExternalReference(IRecordContext recordContext, long? orgExternalRef, long? contactExternalRef, int? orgID)
        {

            //B2C Case: If Contact is there but if no external reference is associated it needs to be first synced to create lead.
            logger.Debug("Validate B2C case, contact is specified and there is no Org");
            if (-1 == contactExternalRef && (null == orgID))
            {
                IContact contactRecord = recordContext.GetWorkspaceRecord(WorkspaceRecordType.Contact) as IContact;
                MessageBox.Show(String.Format(OSCExceptionMessages.ContactDoesNotExistMessage, contactRecord.NameFirst, contactRecord.NameLast),
                    OSCExceptionMessages.LeadNotCreatedTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            //B2B Case
            logger.Debug("Validate B2B Case");
            if (null != orgID)
            {
                //B2B Case 1: Contact is synced but org is still not synced.
                if (-1 == orgExternalRef && contactExternalRef > 0)
                {
                    IOrganization organizationRecord = recordContext.GetWorkspaceRecord(WorkspaceRecordType.Organization) as IOrganization;
                    MessageBox.Show(String.Format(OSCExceptionMessages.OrganizationDoesNotExistMessage, organizationRecord.Name), 
                        OSCExceptionMessages.LeadNotCreatedTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                    logger.Debug("Contact is synced but Organization is not synced");
                    return false;
                }
                //B2B Case 2: Contact is not synced but org is synced.
                if (orgExternalRef > 0 && contactExternalRef == -1)
                {
                    IContact contactRecord = recordContext.GetWorkspaceRecord(WorkspaceRecordType.Contact) as IContact;
                    MessageBox.Show(String.Format(OSCExceptionMessages.ContactDoesNotExistMessage, contactRecord.NameFirst, contactRecord.NameLast),
                        OSCExceptionMessages.LeadNotCreatedTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                    logger.Debug("Contact is not synced but Organization is synced");
                    return false;
                }
                //B2B Case 3: Both Contact and Org not synced.
                if (-1 == orgExternalRef && -1 == contactExternalRef)
                {
                    IContact contactRecord = recordContext.GetWorkspaceRecord(WorkspaceRecordType.Contact) as IContact;
                    IOrganization organizationRecord = recordContext.GetWorkspaceRecord(WorkspaceRecordType.Organization) as IOrganization;
                    MessageBox.Show(String.Format(OSCExceptionMessages.OrganizationContactDoesNotExistMessage,organizationRecord.Name,contactRecord.NameFirst,contactRecord.NameLast), 
                        OSCExceptionMessages.LeadNotCreatedTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                    logger.Debug("Contact and Organization not synced");
                    return false;
                }
            }
            //B2C or B2B Cases all valid.
            logger.Debug("B2B/B2C cases valid ExtRef");
            return true;
        }


        private bool validateLeadOpportunityType(IRecordContext recordContext)
        {
            ICustomObject leadOpportunityRecord = recordContext.GetWorkspaceRecord(recordContext.WorkspaceTypeName) as ICustomObject;
            String leadOpportunityType = (String)getFieldFromLeadOpportunity(leadOpportunityRecord, OSCOpportunitiesCommon.LeadTypeFieldName);
            logger.Debug("Lead/Opportunity Type "+leadOpportunityType);
            if(String.IsNullOrWhiteSpace(leadOpportunityType) || (!leadOpportunityType.Equals(OSCOpportunitiesCommon.OpportunityRecordType) && !leadOpportunityType.Equals(OSCOpportunitiesCommon.SalesLeadRecordType)))
            {
                //Show appropriate message
                MessageBox.Show(OSCExceptionMessages.MisConfiguredLeadOpportunityTypeMessage,
                        OSCExceptionMessages.LeadNotCreatedTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            logger.Debug("Correct type specified");
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="leadOpportunityObject"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public object getFieldFromLeadOpportunity(ICustomObject leadOpportunityObject, String fieldName)
        {
            object woType = null;
            IList<IGenericField> fields = leadOpportunityObject.GenericFields;
            if (null != fields)
            {
                foreach (IGenericField field in fields)
                {
                    if (field.Name.Equals(fieldName))
                    {
                        woType = field.DataValue.Value;
                        return woType;
                    }
                }
            }
            return woType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="leadOpportunityObject"></param>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        public void setFieldLeadOpportunity(ICustomObject leadOpportunityObject, String fieldName, object value)
        {
            IList<IGenericField> fields = leadOpportunityObject.GenericFields;
            if (null != fields)
            {
                foreach (IGenericField field in fields)
                {
                    if (field.Name.Equals(fieldName))
                    {
                        field.DataValue.Value = value;
                    }
                }
            }
        }        
    }
}
