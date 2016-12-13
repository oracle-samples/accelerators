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
 *  SHA1: $Id: d76dd57b774e3f26a59e332956b5c9b265dab2c2 $
 * *********************************************************************************************
 *  File: ConfigurationModel.cs
 * ****************************************************************************************** */
namespace Accelerator.IOTCloud.Client.Model.Common
{
    internal class ConfigurationModel
    {
        public string rnt_host { get; set; }
        public Integration integration { get; set; }
    }

    internal class Integration
    {        
        public ICSService ics_service { get; set; }
    }

    internal class ICSService
    {
        public string base_url { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string get_attributes_url { get; set; }
        public string set_attributes_url { get; set; }
        public string get_messages_url { get; set; }
    }

}
