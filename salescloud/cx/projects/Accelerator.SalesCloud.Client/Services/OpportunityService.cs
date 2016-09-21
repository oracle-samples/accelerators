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
*  SHA1: $Id: 8e22657d7486ad8ccb20f51b741d58132dd0f9c2 $
* *********************************************************************************************
*  File: OpportunityService.cs
* ****************************************************************************************** */

using System.IO;
using System.Net;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Security;
using System.Xml;
using Accelerator.SalesCloud.Client.Logs;
using Accelerator.SalesCloud.Client.Model;
using Accelerator.SalesCloud.Client.RightNow;
using Accelerator.SalesCloud.Client.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accelerator.SalesCloud.Client.OpportunityProxyService;
using System.ServiceModel;
using Accelerator.SalesCloud.Client.Interceptors;

namespace Accelerator.SalesCloud.Client.Services
{
    public class OpportunityService : IOpportunityService
    {
        private static OpportunityService _opportunityService;
        private static object _sync = new object();
        private OpportunityServiceClient _opportunityClient;
        private IOSCLog _logger;

        /// <summary>
        /// Get Inbound Service object
        /// </summary>
        /// <returns></returns>
        public static IOpportunityService GetService()
        {
            if (_opportunityService != null)
            {
                return _opportunityService;
            }

            if (!RightNowConfigService.IsConfigured())
            {
                return null;
            }

            try
            {
                lock (_sync)
                {
                    if (_opportunityService == null)
                    {
                        var opptyServiceUrl = RightNowConfigService.GetConfigValue(RightNowConfigKeyNames.OSCOpptyServiceUrl);

                        EndpointAddress endpoint = new EndpointAddress(opptyServiceUrl);
                        BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
                        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

                        //honoring config verb's value only if it is greater than 0 bytes.
                        string maxResponseSize = RightNowConfigService.GetConfigValue(RightNowConfigKeyNames.SalesMaxReceivedMessageSize);
                        if (!String.IsNullOrEmpty(maxResponseSize) && Convert.ToInt32(maxResponseSize) > 0)
                        {
                            binding.MaxReceivedMessageSize = Convert.ToInt32(maxResponseSize);
                        }

                        _opportunityService = new OpportunityService();
                        _opportunityService._opportunityClient = new OpportunityServiceClient(binding, endpoint);
                        _opportunityService._opportunityClient.ClientCredentials.UserName.UserName = 
                            RightNowConfigService.GetConfigValue(RightNowConfigKeyNames.UserName);
                        _opportunityService._opportunityClient.ClientCredentials.UserName.Password = 
                            RightNowConfigService.GetConfigValue(RightNowConfigKeyNames.Password);
                        _opportunityService._opportunityClient.Endpoint.Behaviors.Add(new EmptyElementBehavior());

                        //_inboundService._log = ToaLogService.GetLog();                        
                    }
                }
            }
            catch (Exception e)
            {
                _opportunityService = null;
                MessageBox.Show(OSCExceptionMessages.OpportunityServiceNotInitialized, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return _opportunityService;
        }

        #region FindOpportunity

        /// <summary>
        /// Find open opportunities based on contact or org.
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<OpportunityModel> FindOpenOpportunities(string attribute, string value)
        {
            try
            {
                ViewCriteriaItem item = new ViewCriteriaItem();
                item.upperCaseCompare = false;
                item.attribute = attribute;
                item.@operator = "=";
                item.Items = new[] {value};

                ViewCriteriaItem item2 = new ViewCriteriaItem();
                item2.upperCaseCompare = true;
                item2.attribute = "StatusCode";
                item2.@operator = "=";
                item2.Items = new[] {"Open"};

                ViewCriteriaRow condition1 = new ViewCriteriaRow();
                condition1.upperCaseCompare = false;
                condition1.item = new[] {item, item2};

                ViewCriteria viewCriteria = new ViewCriteria();
                viewCriteria.conjunction = Conjunction.And;
                viewCriteria.group = new[] {condition1};

                //Sorting by Creation date.
                SortAttribute sortAttr = new SortAttribute();
                sortAttr.name = "CreationDate";
                sortAttr.descending = true;

                FindCriteria findCriteria = new FindCriteria();
                findCriteria.fetchStart = 0;
                findCriteria.fetchSize = 500;
                findCriteria.filter = viewCriteria;
                findCriteria.sortOrder = new[] {sortAttr};

                findCriteria.findAttribute = new string[]
                {
                    "KeyContactId",
                    "PrimaryContactPartyName",
                    "SalesAccountId",
                    "TargetPartyName",
                    "OptyId",
                    "OptyNumber",
                    "PartyName1",
                    "EmailAddress",
                    "Name",
                    "Description",
                    "StatusCode",
                    "SalesMethod",
                    "SalesStage",
                    "SalesChannelCd",
                    "CurrencyCode",
                    "Revenue",
                    "WinProb",
                    "CreatedBy",
                    "CreationDate",
                    "EffectiveDate"
                };

                FindControl findControl = new FindControl();

                Opportunity[] opp = _opportunityClient.findOpportunity(findCriteria, findControl);

                return getOpportunityModels(opp);
            }
            catch (EndpointNotFoundException wex)
            {
                //Handling incorrect opportunity endpoint
                _logger.Error(wex.Message, wex.StackTrace);
                MessageBox.Show(OSCExceptionMessages.EndpointNotFound,
                    OSCOpportunitiesCommon.EndpointNotFound, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (MessageSecurityException mse)
            {
                //Handling incorrect credentials
                _logger.Error(mse.Message, mse.StackTrace);
                MessageBox.Show(OSCExceptionMessages.OpportunityAuthError,
                    OSCOpportunitiesCommon.OpportunityAuthError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (CommunicationException ce)
            {
                //Handling maximum reponse size exceeded
                _logger.Error(ce.Message, ce.StackTrace);
                MessageBox.Show(OSCExceptionMessages.MaxReceivedMessageSizeExceeded,
                        OSCOpportunitiesCommon.MaxReceivedMessageSizeExceededTitle, MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex.StackTrace);
                MessageBox.Show(OSCExceptionMessages.UnexpectedError,
                       "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        /// <summary>
        /// Returns list od opportunity models.
        /// </summary>
        /// <param name="opportunities"></param>
        /// <returns></returns>
        private List<OpportunityModel> getOpportunityModels(Opportunity[] opportunities)
        {
            List<OpportunityModel> opportunityModels = new List<OpportunityModel>();

            if (opportunities == null)
            {
                return opportunityModels;
            }

            foreach (var opportunity in opportunities)
            {
                var opp = new OpportunityModel
                {
                    KeyContactId = opportunity.KeyContactId,
                    PrimaryContactPartyName = opportunity.PrimaryContactPartyName,
                    SalesAccountId = opportunity.SalesAccountId,
                    TargetPartyName = opportunity.TargetPartyName,
                    OptyId = opportunity.OptyId,
                    OptyNumber = opportunity.OptyNumber,
                    PartyName1 = opportunity.PartyName1,
                    EmailAddress = opportunity.EmailAddress,
                    Name = opportunity.Name,
                    Description = opportunity.Description,
                    StatusCode = opportunity.StatusCode,
                    SalesMethod = opportunity.SalesMethod,
                    SalesStage = opportunity.SalesStage,
                    SalesChannelCd = opportunity.SalesChannelCd,
                    CurrencyCode = opportunity.CurrencyCode,
                    Revenue = opportunity.Revenue,
                    WinProb = opportunity.WinProb,
                    CreatedBy = opportunity.CreatedBy,
                    CreationDate = opportunity.CreationDate,
                    ForecastedCloseDate = opportunity.EffectiveDate
                };

                opportunityModels.Add(opp);

            }

            return opportunityModels;

        }

        #endregion

        #region CreateOpportunity

        /// <summary>
        /// Create an Opportunity in OSC
        /// </summary>
        /// <param name="opportunityModel">OpportunityModel</param>
        /// <returns></returns>
        public OpportunityModel CreateOpportunity(OpportunityModel opportunityModel)
        {
            OpportunityModel resultModel = null;
            try
            {
                if (opportunityModel != null)
                {
                    Opportunity opportunity = new Opportunity();
                    opportunity.Name = opportunityModel.Name;
                    opportunity.TargetPartyId = opportunityModel.TargetPartyId;
                    opportunity.TargetPartyIdSpecified = opportunityModel.TargetPartyIdSpecified;
                    opportunity.OwnerResourcePartyId = opportunityModel.OwnerResourcePartyId;
                    opportunity.OwnerResourcePartyIdSpecified = opportunityModel.OwnerResourcePartyIdSpecified;
                    opportunity.KeyContactId = opportunityModel.KeyContactId;
                    opportunity.KeyContactIdSpecified = opportunityModel.KeyContactIdSpecified;

                    OpportunityResource resource = new OpportunityResource();
                    resource.ResourceId = opportunityModel.OpportunityResourceModel.ResourceId;
                    resource.ResourceIdSpecified = opportunityModel.OpportunityResourceModel.ResourceIdSpecified;
                    resource.OwnerFlag = opportunityModel.OpportunityResourceModel.OwnerFlag;
                    resource.OwnerFlagSpecified = opportunityModel.OpportunityResourceModel.OwnerFlagSpecified;

                    OpportunityResource[] resources = new OpportunityResource[] { resource };
                    opportunity.OpportunityResource = resources;
                    if (!OSCCommonUtil.ValidateCurrentSiteName())
                    {
                        resultModel = new OpportunityModel();
                        resultModel.OpportunityId = OSCOpportunitiesCommon.DefaultOpportunitySalesLeadID;
                        return resultModel;
                    }
                    Opportunity result = _opportunityService._opportunityClient.createOpportunity(opportunity);

                    resultModel = new OpportunityModel();
                    resultModel.OpportunityId = result.OptyId;
                }
            }
            catch (Exception exception)
            {
                _logger.Debug("Error occured while creating opportunity. Opportunity Not Created in Sales Cloud.", exception.StackTrace);
                MessageBox.Show(OSCExceptionMessages.LeadOpportunityCannotBeCreated, OSCExceptionMessages.LeadNotCreatedTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            return resultModel;
        }

        #endregion

        private OpportunityService()
        {
            _logger = OSCLogService.GetLog();
        }
    }
}
