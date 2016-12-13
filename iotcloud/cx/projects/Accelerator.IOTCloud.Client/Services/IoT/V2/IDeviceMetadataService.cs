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
 *  SHA1: $Id: 0892b3a87ef6006e71b5063b2cdf6a1d30b93f06 $
 * *********************************************************************************************
 *  File: IDeviceMetadataService.cs
 * ****************************************************************************************** */

using System.Collections.Generic;
using Accelerator.IOTCloud.Client.Model.IoT.V2;

namespace Accelerator.IOTCloud.Client.Services
{
    public delegate void DeviceMetadataServiceDelegate(string result);
    public delegate void GetDeviceMetadataDelegate(Dictionary<string, Attributes> result);
    public interface IDeviceMetadataService
    {
        void PutProperty(string deviceId, string PropertyDesc, string postData, DeviceMetadataServiceDelegate callback);
        void GetDeviceProperties(string deviceId, GetDeviceMetadataDelegate callback);
    }
}
