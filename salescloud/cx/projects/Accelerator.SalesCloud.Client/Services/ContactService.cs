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
 *  date: Mon Sep 19 02:05:28 PDT 2016

 *  revision: rnw-15-11-fixes-release-3
 *  SHA1: $Id: 8a6f445212e169b717306f09fab9aa93dd624eb5 $
 * *********************************************************************************************
 *  File: ContactService.cs
 * ****************************************************************************************** */

using Accelerator.SalesCloud.Client.Common;
using Accelerator.SalesCloud.Client.ContactProxyService;
using Accelerator.SalesCloud.Client.Logs;
using Accelerator.SalesCloud.Client.RightNow;
using Oracle.RightNow.OSC.Client.Model;
using System;
using System.ServiceModel;
using System.Windows.Forms;

namespace Oracle.RightNow.OSC.Client.Services
{
    public class ContactService : IContactService
    {
        private static ContactService _contactService;
        private static object _sync = new object();
        private ContactServiceClient _contactClient;
        private IOSCLog _logger;

        /// <summary>
        /// Get Contact Service object
        /// </summary>
        /// <returns></returns>
        public static IContactService GetService()
        {
            if (_contactService != null)
            {
                return _contactService;
            }

            if (!RightNowConfigService.IsConfigured())
            {
                return null;
            }

            try
            {
                lock (_sync)
                {
                    if (_contactService == null)
                    {
                        var contactServiceUrl = RightNowConfigService.GetConfigValue(RightNowConfigKeyNames.OSCContactServiceUrl);

                        EndpointAddress endpoint = new EndpointAddress(contactServiceUrl);
                        
                        BasicHttpBinding binding = new BasicHttpBinding();
                        binding.Security.Mode = BasicHttpSecurityMode.Transport;
                        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                        
                        _contactService = new ContactService();
                        _contactService._contactClient = new ContactServiceClient(binding, endpoint);

                        _contactService._contactClient.ClientCredentials.UserName.UserName = RightNowConfigService.GetConfigValue(RightNowConfigKeyNames.UserName);
                        _contactService._contactClient.ClientCredentials.UserName.Password = RightNowConfigService.GetConfigValue(RightNowConfigKeyNames.Password);
                                                                                  
                        //_accountService._log = ToaLogService.GetLog();    
                    }
                }
            }
            catch (Exception e)
            {
                _contactService = null;
                MessageBox.Show(OSCExceptionMessages.AccountServiceNotInitialized, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return _contactService;
        }

        /// <summary>
        /// Get the OwnerPartyId
        /// </summary>
        /// <param name="contactModel">ContactModel</param>
        /// <returns></returns>
        public ContactModel GetOwnerPartyId(ContactModel contactModel)
        {
            ContactModel resultModel = null;
            if (contactModel != null)
            {
                try
                {
                    DataObjectResult dataObjResult = _contactService._contactClient.getContact(contactModel.PartyId);
                    if (null == dataObjResult)
                    {
                        _logger.Debug("No Contact found matching external reference " + contactModel.PartyId);
                        resultModel = new ContactModel();
                        resultModel.OwnerPartyId = null;
                        return resultModel;
                    }
                    object[] dataObjResultValue = dataObjResult.Value;


                    foreach (object value in dataObjResultValue)
                    {
                        resultModel = new ContactModel();
                        Contact contact = (Contact) value;
                        resultModel.OwnerPartyId = (long) contact.OwnerPartyId;
                    }
                }
                catch (Exception e)
                {
                    _logger.Debug("Error occured while creating lead. Lead Not Created in Sales Cloud. Exception: " + e.StackTrace);
                    _logger.Debug("Setting OwnerPartyId for Contact to null");
                    resultModel = new ContactModel();
                    resultModel.OwnerPartyId = null;
                }
            }

            return resultModel;
        }

        private ContactService()
        {
            _logger = OSCLogService.GetLog();
        }
    }
}
