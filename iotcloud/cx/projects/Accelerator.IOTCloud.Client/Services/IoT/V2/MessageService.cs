/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: IoT OSvC Bi-directional Integration Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 16.11 (November 2016) 
 *  reference: 151217-000026
 *  date: Tue Dec 13 13:23:40 PST 2016
 
 *  revision: rnw-16-11-fixes-release
 *  SHA1: $Id: a02ee146eee2303ee285a93a9be6ac5fde529845 $
 * *********************************************************************************************
 *  File: MessageService.cs
 * ****************************************************************************************** */

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Accelerator.IOTCloud.Client.Logs;
using Accelerator.IOTCloud.Client.Model;
using Accelerator.IOTCloud.Client.Http;
using Accelerator.IOTCloud.Client.RightNow;

namespace Accelerator.IOTCloud.Client.Services
{
    public class MessageService : IMessageService
    {
        private static MessageService _messageService;
        private static object _sync = new object();
        private static string MESSAGE_URL;
        private static string USERNAME;
        private static string PASSWORD;

        public static MessageService GetService()
        {
            if (_messageService != null)
            {
                return _messageService;
            }

            try
            {
                lock (_sync)
                {
                    _messageService = new MessageService();
                    MESSAGE_URL = RightNowConfigService.GetConfigValue(RightNowConfigKeyNames.ICS_GET_MESSAGES_URL);
                    USERNAME = RightNowConfigService.GetConfigValue(RightNowConfigKeyNames.ICS_USERNAME);
                    PASSWORD = RightNowConfigService.GetConfigValue(RightNowConfigKeyNames.ICS_PASSWORD);

                }
            }
            catch (Exception e)
            {
                _messageService = null;
                LogService.GetLog().Error(ExceptionMessages.IOT_MESSAGE_SERVICE_NOT_INITIALIZED, e.StackTrace);
                MessageBox.Show(ExceptionMessages.IOT_MESSAGE_SERVICE_NOT_INITIALIZED, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return _messageService;
        }

        public void GetMessage(Dictionary<string, string> parameters, MessageServiceDelegate callback)
        {
            //Validate the current sitename.
            if (!CommonUtil.ValidateCurrentSiteName())
            {
                return;
            }

            var backgroundService = new BackgroundServiceUtil();

            backgroundService.RunAsync(() =>
            {
                var httpClient = new HttpClient(MESSAGE_URL, HttpMethodEnum.GET, USERNAME, PASSWORD);
                var queryString = HttpUtil.QueryString(parameters);
                var httpresponse = httpClient.Get(queryString);
                callback.Invoke(httpresponse);
            });
        }
    }
}
