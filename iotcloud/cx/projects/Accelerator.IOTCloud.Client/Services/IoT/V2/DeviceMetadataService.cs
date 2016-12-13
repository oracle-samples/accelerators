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
 *  SHA1: $Id: 3ffe1a3e453974e763b5c321f933ef114dc44c35 $
 * *********************************************************************************************
 *  File: DeviceMetadataService.cs
 * ****************************************************************************************** */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Accelerator.IOTCloud.Client.Logs;
using Accelerator.IOTCloud.Client.Model;
using Accelerator.IOTCloud.Client.Http;
using Accelerator.IOTCloud.Client.Model.IoT.V2;
using Accelerator.IOTCloud.Client.RightNow;

namespace Accelerator.IOTCloud.Client.Services
{
    public class DeviceMetadataService : IDeviceMetadataService
    {
        private static DeviceMetadataService _deviceMetadataService;
        private static object _sync = new object();
        private static string GET_ATTRIBUTES_URL;
        private static string SET_ATTRIBUTES_URL;
        private static string USERNAME;
        private static string PASSWORD;
        private Dictionary<string, Attributes> _attributes;

        public static DeviceMetadataService GetService()
        {
            if (_deviceMetadataService != null)
            {
                return _deviceMetadataService;
            }

            try
            {
                lock (_sync)
                {
                    _deviceMetadataService = new DeviceMetadataService();

                    GET_ATTRIBUTES_URL = RightNowConfigService.GetConfigValue(RightNowConfigKeyNames.ICS_GET_ATTRIBUTES_URL);
                    SET_ATTRIBUTES_URL = RightNowConfigService.GetConfigValue(RightNowConfigKeyNames.ICS_SET_ATTRIBUTES_URL);
                    USERNAME = RightNowConfigService.GetConfigValue(RightNowConfigKeyNames.ICS_USERNAME);
                    PASSWORD = RightNowConfigService.GetConfigValue(RightNowConfigKeyNames.ICS_PASSWORD);
                }
            }
            catch (Exception e)
            {
                _deviceMetadataService = null;
                LogService.GetLog().Error(ExceptionMessages.IOT_ENDPOINT_SERVICE_NOT_INITIALIZED, e.StackTrace);
                MessageBox.Show(ExceptionMessages.IOT_ENDPOINT_SERVICE_NOT_INITIALIZED, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return _deviceMetadataService;
        }

        public void PutProperty(string deviceId, string propertyDesc, string postData, DeviceMetadataServiceDelegate callback)
        {
            //Validate the current sitename.
            if (!CommonUtil.ValidateCurrentSiteName())
            {
                return;
            }

            var endpointUrl = SET_ATTRIBUTES_URL;
            endpointUrl = endpointUrl.Replace("{device_id}", deviceId);

            Attributes attribute;
            _attributes.TryGetValue(propertyDesc, out attribute);
            if (attribute != null)
            {
                var attributName = attribute.name;
                endpointUrl = endpointUrl + "/" + attributName;
                LogService.GetLog().Debug(string.Format("Set Property {0} with value {1} for device {2}", attribute.name, postData, deviceId));
            }

            var backgroundService = new BackgroundServiceUtil();
            backgroundService.RunAsync(() =>
            {
                var httpClient = new HttpClient(endpointUrl, HttpMethodEnum.PUT, postData, USERNAME, PASSWORD);
                var httpresponse = httpClient.Put();
                callback.Invoke(httpresponse);
            });
        }


        public void GetDeviceProperties(string deviceId, GetDeviceMetadataDelegate callback)
        {
            //Validate the current sitename.
            if (!CommonUtil.ValidateCurrentSiteName())
            {
                return;
            }

            var backgroundService = new BackgroundServiceUtil();
            LogService.GetLog().Debug(string.Format("Get Properties for device {0}", deviceId));
            backgroundService.RunAsync(() =>
            {
                var device_url = GET_ATTRIBUTES_URL + "/" + deviceId;
                var httpClient = new HttpClient(device_url, HttpMethodEnum.GET, USERNAME, PASSWORD);
                var httpresponse = httpClient.Get("");

                var deviceMetadataModel = JsonUtil.FromJson<DeviceMetadataModel>(httpresponse);
                var deviceModels = deviceMetadataModel.deviceModels;

                if (deviceModels != null)
                {
                    DeviceModels deviceModel = null;
                    foreach (var model in deviceModels)
                    {
                        if (model.attributes != null && model.attributes.Count > 0)
                        {
                            LogService.GetLog().Debug(string.Format
                                ("No of Properties for device {0} retrieved : {1}", deviceId, model.attributes.Count));
                            deviceModel = model;
                            break;
                        }
                    }

                    _attributes = new Dictionary<string, Attributes>();
                    if (deviceModel != null && deviceModel.attributes != null)
                    {
                        foreach (var attribute in deviceModel.attributes)
                        {
                            _attributes.Add(attribute.description, attribute);
                        }
                    }
                    callback.Invoke(_attributes);
                }

                
            });
        }
    }
}
