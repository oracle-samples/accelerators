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
 *  date: Tue Dec 13 13:23:39 PST 2016
 
 *  revision: rnw-16-11-fixes-release
 *  SHA1: $Id: 2f9d4a2b41db7d68b0815026e79500a114cfad17 $
 * *********************************************************************************************
 *  File: DeviceMetadataModel.cs
 * ****************************************************************************************** */
using System.Collections.Generic;

namespace Accelerator.IOTCloud.Client.Model.IoT.V2
{
    public class DeviceMetadataModel
    {
        public List<DeviceModels> deviceModels { get; set; }
    }

    public class DeviceModels
    {
        public string urn { get; set; }
        public List<Attributes> attributes { get; set; }
    }

    public class Attributes
    {
        public string name { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public bool writable { get; set; }
    }
}
