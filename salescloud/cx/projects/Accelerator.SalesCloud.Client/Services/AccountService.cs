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
*  SHA1: $Id: 6b0187fc95511b2e84fb3e663a4af190f66862a1 $
* *********************************************************************************************
*  File: AccountService.cs
* ****************************************************************************************** */

using Accelerator.SalesCloud.Client.RightNow;
using Accelerator.SalesCloud.Client.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accelerator.SalesCloud.Client.AccountProxyService;
using System.ServiceModel;
using Accelerator.SalesCloud.Client.Logs;
using Accelerator.SalesCloud.Client.Model;

namespace Accelerator.SalesCloud.Client.Services
{
    public class AccountService : IAccountService
    {
        
        private static AccountService _accountService;
        private static object _sync = new object();
        private AccountServiceClient _accountClient;
        private IOSCLog _logger;

        /// <summary>
        /// Get Inbound Service object
        /// </summary>
        /// <returns></returns>
        public static IAccountService GetService()
        {
            if (_accountService != null)
            {
                return _accountService;
            }

            if (!RightNowConfigService.IsConfigured())
            {
                return null;
            }

            try
            {
                lock (_sync)
                {
                    if (_accountService == null)
                    {
                        var accountServiceUrl = RightNowConfigService.GetConfigValue(RightNowConfigKeyNames.OSCAcctServiceUrl);

                        EndpointAddress endpoint = new EndpointAddress(accountServiceUrl);
                        
                        BasicHttpBinding binding = new BasicHttpBinding();
                        binding.Security.Mode = BasicHttpSecurityMode.Transport;
                        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                        
                        _accountService = new AccountService();
                        _accountService._accountClient = new AccountServiceClient(binding, endpoint);

                        _accountService._accountClient.ClientCredentials.UserName.UserName = RightNowConfigService.GetConfigValue(RightNowConfigKeyNames.UserName);
                        _accountService._accountClient.ClientCredentials.UserName.Password = RightNowConfigService.GetConfigValue(RightNowConfigKeyNames.Password);
                                                                                  
                        //_accountService._log = ToaLogService.GetLog();    
                    }
                }
            }
            catch (Exception e)
            {
                _accountService = null;
                MessageBox.Show(OSCExceptionMessages.AccountServiceNotInitialized, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return _accountService;
        }

        /// <summary>
        /// Get the OwnerPartyId
        /// </summary>
        /// <param name="accountModel">AccountModel</param>
        /// <returns></returns>
        public AccountModel GetOwnerPartyId(AccountModel accountModel)
        {
            AccountModel resultModel = null;
            if (accountModel != null)
            {
                try
                {
                    DataObjectResult dataObjResult = _accountService._accountClient.getAccount(accountModel.PartyId);
                    if (null == dataObjResult)
                    {
                        _logger.Debug("No Account found matching external reference " + accountModel.PartyId);
                        resultModel = new AccountModel();
                        resultModel.OwnerPartyId = null;
                        return resultModel;
                    }
                    object[] dataObjResultValue = dataObjResult.Value;


                    foreach (object value in dataObjResultValue)
                    {
                        resultModel = new AccountModel();
                        Account account = (Account) value;
                        resultModel.OwnerPartyId = (long) account.OwnerPartyId;
                    }
                }
                catch (Exception e)
                {
                    _logger.Debug("Error occured while creating lead. Lead Not Created in Sales Cloud. Exception: " + e.StackTrace);
                    _logger.Debug("Setting OwnerPartyId for Account to null");
                    resultModel = new AccountModel();
                    resultModel.OwnerPartyId = null;
                }
            }

            return resultModel;
        }

        private AccountService()
        {
            _logger = OSCLogService.GetLog();
        }
    }
}
