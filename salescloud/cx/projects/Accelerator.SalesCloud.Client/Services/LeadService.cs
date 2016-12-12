/* * *******************************************************************************************
*  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + OSC Lead Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.11 (November 2015)
 *  OSC release: Release 10
 *  reference: 150505-000122, 160620-000160
 *  date: Mon Sep 19 02:05:29 PDT 2016

 *  revision: rnw-15-11-fixes-release-3
*  SHA1: $Id: 3febd36f930923d4725acd7adbd014fa0f46f33d $
* *********************************************************************************************
*  File: LeadService.cs
* ****************************************************************************************** */

using Accelerator.SalesCloud.Client.RightNow;
using Accelerator.SalesCloud.Client.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerator.SalesCloud.Client.LeadProxyService;
using System.ServiceModel;
using Accelerator.SalesCloud.Client.Model;
using Accelerator.SalesCloud.Client.Logs;
using System.Windows.Forms;
using Accelerator.SalesCloud.Client.Interceptors;

namespace Accelerator.SalesCloud.Client.Services
{
    public class LeadService : ILeadService
    {
        private static LeadService _leadService;
        private static object _sync = new object();
        private LeadIntegrationServiceClient _leadClient;
        private IOSCLog _logger;

        /// <summary>
        /// Get Lead Service object
        /// </summary>
        /// <returns></returns>
        public static ILeadService GetService()
        {
            if (_leadService != null)
            {
                return _leadService;
            }

            if (!RightNowConfigService.IsConfigured())
            {
                return null;
            }

            try
            {
                lock (_sync)
                {
                    if (_leadService == null)
                    {
                        var leadServiceUrl = RightNowConfigService.GetConfigValue(RightNowConfigKeyNames.OSCLeadServiceUrl);

                        EndpointAddress endpoint = new EndpointAddress(leadServiceUrl);
                        BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
                        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

                        _leadService = new LeadService();
                        _leadService._leadClient = new LeadIntegrationServiceClient(binding, endpoint);
                        _leadService._leadClient.ClientCredentials.UserName.UserName = RightNowConfigService.GetConfigValue(RightNowConfigKeyNames.UserName);
                        _leadService._leadClient.ClientCredentials.UserName.Password = RightNowConfigService.GetConfigValue(RightNowConfigKeyNames.Password);
                        _leadService._leadClient.Endpoint.Behaviors.Add(new EmptyElementBehavior());

                        // TODO: Need to work on this while working on SCLog story.
                        //_inboundService._log = ToaLogService.GetLog();
                        
                    }
                }
            }
            catch (Exception e)
            {
                _leadService = null;
                MessageBox.Show(OSCExceptionMessages.LeadServiceNotInitialized, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return _leadService;
        }

        private LeadService()
        {
            _logger = OSCLogService.GetLog();
        }

        /// <summary>
        /// Create a ServiceLead in OSC
        /// </summary>
        /// <param name="leadModel">LeadModel</param>
        /// <returns></returns>
        public LeadModel CreateServiceLead(LeadModel leadModel)
        {
            LeadModel resultModel = null;
            try
            {
                if (leadModel != null)
                {
                    SalesLead lead = new SalesLead();
                    lead.Name = leadModel.Name;
                    lead.CustomerId = leadModel.CustomerId;
                    lead.CustomerIdSpecified = leadModel.CustomerIdSpecified;
                    lead.PrimaryContactId = leadModel.PrimaryContactId;
                    lead.PrimaryContactIdSpecified = leadModel.PrimaryContactIdSpecified;
                    lead.OwnerId = leadModel.OwnerId;
                    lead.OwnerIdSpecified = leadModel.OwnerIdSpecified;

                    if (!OSCCommonUtil.ValidateCurrentSiteName())
                    {
                        resultModel = new LeadModel();
                        resultModel.LeadId = OSCOpportunitiesCommon.DefaultOpportunitySalesLeadID;
                        return resultModel;
                    }

                    SalesLeadResult leadResult = _leadService._leadClient.createSalesLead(lead);
                    var result = leadResult.Value.FirstOrDefault();

                    if (result != null)
                    {
                        resultModel = new LeadModel();
                        resultModel.LeadId = result.LeadId;
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.Debug("Error occured while creating lead. Lead Not Created in Sales Cloud. Exception: " + exception.StackTrace);
                MessageBox.Show(OSCExceptionMessages.LeadOpportunityCannotBeCreated, OSCExceptionMessages.LeadNotCreatedTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return resultModel;
        }
    }
}
