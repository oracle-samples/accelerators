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
 *  date: Tue Dec  1 21:42:18 PST 2015

 *  revision: rnw-15-11-fixes-release-2
*  SHA1: $Id: 1ccda7a52b702cec695233fcc96a2ded8407c95a $
* *********************************************************************************************
*  File: OSCStringConstants.cs
* ****************************************************************************************** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerator.SalesCloud.Client.Common
{
    
    public static class OracleRightNowOSCAddInNames
    {
        public static readonly string OracleRightNowOSCClient = "OSC Client AddIn";
        public static readonly string LeadOpportunityAddIn = "LeadOpportunityAddIn";
        public static readonly string OpportunityReportTableAddIn = "OpportunityReportTableAddIn";
    }

    public static class RightNowConfigKeyNames
    {
        public static readonly string OSCOpptyServiceUrl = "oppty_service_url";
        public static readonly string OSCAcctServiceUrl = "acct_service_url";
        public static readonly string OSCLeadServiceUrl = "lead_service_url";        
        public static readonly string UserName = "username";
        public static readonly string Password = "password";
        public static readonly string ServerType = "server_type";
        public static readonly string RightnowHost = "rnt_host";
        public static readonly string SalesBaseUrl = "sales_base_url";
        public static readonly string SalesTimeout = "sales_timeout";
        public static readonly string SalesMaxReceivedMessageSize = "sales_max_rcvd_msg_size";        
        public static readonly string CustomCfgSalesAccelIntegrations = "CUSTOM_CFG_Sales_Accel_Integrations";
        public static readonly string OpportunityLeadType = "oppty_lead_type";
        public static readonly string OSCContactServiceUrl = "ctc_service_url";
    }

    public static class RightNowQueries
    {
        public static readonly string GetConfigVerbQuery = "select Configuration.Value from Configuration where lookupname=";
        public static readonly string GetContactExternalReferenceQuery = "SELECT ExternalReference FROM Contact WHERE ID = {0}";
        public static readonly string GetOrganizationExternalReferenceQuery = "SELECT ExternalReference FROM Organization WHERE ID = {0}";
    }

    public static class OSCExceptionMessages
    {
        public static readonly string RightNowConnectServiceNotInitialized = "RightNowConnectService is not initialized";
        public static readonly string ConfigurationNotInitialized = "Sales Configuration Service is not initialized";
        public static readonly string ConfigVerbIsNotSetOrIncorrect = "Sales Integration Configuration Verb is not set or is incorrect";
        public static readonly string UnexpectedError = "Unexpected ERROR, Please contact support.";
        public static readonly string AccountServiceNotInitialized = "Account Service is not initialized";
        public static readonly string LeadServiceNotInitialized = "Lead Service is not initialized";
        public static readonly string LogServiceNotInitialized = "Log Service is not initialized";
        public static readonly string OpportunityServiceNotInitialized = "Opportunity Service is not initialized";
        public static readonly string LeadNotCreatedTitle = "Lead Not Created in Sales Cloud";
        public static readonly string ContactMandatoryMessage = "Contact is required, lead cannot be created.  Please cancel and try to create the lead again.  If the error continues, please contact support.";
        public static readonly string ContactDoesNotExistMessage = "The Contact, {0} {1}, does not exist in Sales Cloud, lead cannot be created. Please try again later by opening and saving the lead.  "
                    + "If the error continues, please contact support.";
        public static readonly string OrganizationDoesNotExistMessage = "The Organization, {0}, does not exist in Sales Cloud, lead cannot be created.  "
            + "Please try again later by opening and saving the lead.  If the error continues, please contact support.";
        public static readonly string OrganizationContactDoesNotExistMessage = "The Organization, {0},  and Contact, {1} {2}" +
                        ", do not exist in Sales Cloud, lead cannot be created.  Please try again later by opening and saving the lead.  If the error continues, please contact support.";
        public static readonly string FindOppTimedOut = "Sales Opportunities request timed out. \nContact Administrator if issue persists.";
        public static readonly string LeadOpportunityUpdateNotSupportedMessage = "Lead already created in Sales Cloud and update not supported.  Changes will not be saved.";
        public static readonly string LeadOpportunityUpdateNotSupportedTitle = "Lead Update Not Supported";
        public static readonly string OracleSalesIntegrationSiteWarningMessage = "Please contact your support, the integration with Oracle Sales is not setup correctly for this Service site.";
        public static readonly string OracleSalesIntegrationSiteWarningTitle = "Oracle Sales Integration Configuration Error";
        public static readonly string MisConfiguredLeadOpportunityTypeMessage = "Problem with the configuration, Lead type of Opportunity or SalesLead must be set.  Please contact support.";
        public static readonly string MaxReceivedMessageSizeExceeded = "Size of the response exceeds the limitation. Please contact support.";
        public static readonly string MaxReceivedMessageSizeMsg = "To increase the quota, use the MaxReceivedMessageSize property on the appropriate binding element";
        public static readonly string LeadOpportunityCannotBeCreated = "Problem with the configuration, lead cannot be created in Sales Cloud. Please contact support.";
        public static readonly string SalesAccountNotExistMessage = "The Sales Account Primary Account Owner is not configured for this Contact and/or Organization." +
                                                                        " Lead cannot be created in Sales Cloud.";
        public static readonly string EndpointNotFound = "Problem with the configuration, Incorrect endpoint found. \nPlease contact support.";
        public static readonly string OpportunityAuthError = "Problem with the configuration, Incorrect credentials found. \nPlease contact support.";
    }

    public static class OSCOpportunitiesCommon
    {
        public static readonly string NoValue = "No Value";
        public static readonly string LeadOpportunitySuccessStatus = "Successful";
        public static readonly string LeadOpportunityFailedStatus = "Failed";
        public static readonly string LeadOpportunityStatusFieldName = "submit_status";
        public static readonly string LeadTypeFieldName = "lead_type";
        public static readonly string LeadOpportunityExtRef = "ext_ref";
        public static readonly string OpportunityRecordType = "opportunity";
        public static readonly string SalesLeadRecordType = "saleslead";
        public static readonly string FindOppTimedOutTitle = "Request Timeout";
        public static readonly string MaxReceivedMessageSizeExceededTitle = "Limitation Exceeded";
        public static readonly string LeadOpportunityLeadName = "lead_name";
        public static readonly long DefaultOpportunitySalesLeadID = -2;
        public static readonly string SalesLeadName = "New Service Sales Lead for {0}: See Incident #{1}";
        public static readonly string OpportunityName = "New Service Opportunity for {0}: See Incident #{1}";
        public static readonly string EndpointNotFound = "Endpoint Not Found";
        public static readonly string OpportunityAuthError = "Authentication Error";
    }

    public static class OSCOpportunitiesTableMetadata
    {
        public static readonly string Name = "OSCOpportunity";
        public static readonly string Label = "OSC Opportunities Table";
        public static readonly string Description = "OSC Opportunities Table";
        public static readonly string OrgFilterColumn = "TargetPartyId";
        public static readonly string ContactFilterColumn = "PrimaryContactPartyId";
    }

    public static class OSCOpportunitiesTableColumnNames
    {
        public static readonly string KeyContactId = "osc_contact_party_id";
        public static readonly string PrimaryContactPartyName = "osc_contact_name";
        public static readonly string SalesAccountId = "osc_account_party_id";
        public static readonly string TargetPartyName = "org_name";
        public static readonly string OptyId = "osc_op_id";
        public static readonly string OptyNumber = "osc_op_number";
        public static readonly string PartyName1 = "osc_op_owner_name";
        public static readonly string EmailAddress = "osc_op_owner_email";
        public static readonly string Name = "osc_op_name";
        public static readonly string Description = "osc_op_desc";
        public static readonly string StatusCode = "osc_op_status";
        public static readonly string SalesMethod = "osc_op_method";
        public static readonly string SalesStage = "osc_op_stage";
        public static readonly string SalesChannelCd = "osc_op_channel";
        public static readonly string CurrencyCode = "osc_op_currency";
        public static readonly string Revenue = "osc_op_revenue";
        public static readonly string WinProb = "osc_op_win_percent";
        public static readonly string CreatedBy = "osc_created_by";
        public static readonly string CreationDate = "osc_created";
        public static readonly string ForecastedCloseDate = "osc_fcst_close_date";
    }

    public static class OSCOpportunitiesTableColumnLabels
    {
        public static readonly string KeyContactId = "Contact";
        public static readonly string PrimaryContactPartyName = "Contact Name";
        public static readonly string SalesAccountId = "Organization";
        public static readonly string TargetPartyName = "Organization Name";
        public static readonly string OptyId = "Opportunity ID";
        public static readonly string OptyNumber = "Opportunity Number";
        public static readonly string PartyName1 = "Owner Name";
        public static readonly string EmailAddress = "Owner Email";
        public static readonly string Name = "Name";
        public static readonly string Description = "Description";
        public static readonly string StatusCode = "Status";
        public static readonly string SalesMethod = "Sales Method";
        public static readonly string SalesStage = "Stage";
        public static readonly string SalesChannelCd = "Channel";
        public static readonly string CurrencyCode = "Currency Code";
        public static readonly string Revenue = "Revenue";
        public static readonly string WinProb = "Win Percent";
        public static readonly string CreatedBy = "Created By";
        public static readonly string CreationDate = "Date Created";
        public static readonly string ForecastedCloseDate = "Forecasted Close Date";
    }

    public static class OSCOpportunitiesTableColumnDesc
    {
        public static readonly string KeyContactId = "The Sales Cloud Party ID of the contact associated with the opportunity.";
        public static readonly string PrimaryContactPartyName = "The opportunity contact name.";
        public static readonly string SalesAccountId = "The organization associated with the opportunity.";
        public static readonly string TargetPartyName = "The opportunity organization name.";
        public static readonly string OptyId = "The Sales Cloud ID number of the opportunity.";
        public static readonly string OptyNumber = "The Sales Cloud opportunity number.";
        public static readonly string PartyName1 = "The Sales Cloud opportunity owner.";
        public static readonly string EmailAddress = "The Sales Cloud opportunity owner email.";
        public static readonly string Name = "The Sales Cloud opportunity name.";
        public static readonly string Description = "The Sales Cloud opportunity description.";
        public static readonly string StatusCode = "The Sales Cloud opportunity status.";
        public static readonly string SalesMethod = "The Sales Cloud opportunity sales method.";
        public static readonly string SalesStage = "The Sales Cloud opportunity sales stage.";
        public static readonly string SalesChannelCd = "The Sales Cloud opportunity sales channel.";
        public static readonly string CurrencyCode = "The Sales Cloud opportunity currency code.";
        public static readonly string Revenue = "The Sales Cloud opportunity summary revenue.";
        public static readonly string WinProb = "The Sales Cloud opportunity win probability percentage.";
        public static readonly string CreatedBy = "The Sales Cloud userid who created the opportunity.";
        public static readonly string CreationDate = "The date and time that the opportunity record was created in Sales Cloud.";
        public static readonly string ForecastedCloseDate = "The Sales Cloud opportunity estimated close date.";
    }

}
