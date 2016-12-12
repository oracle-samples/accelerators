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
 *  date: Mon Sep 19 02:05:25 PDT 2016

 *  revision: rnw-15-11-fixes-release-3
*  SHA1: $Id: ee78ed864cc591d8a2ed38c201eeb7fbc43178e9 $
* *********************************************************************************************
*  File: OSCConfigVerb.cs
* ****************************************************************************************** */

using System.Collections.Generic;
using System.Web.Services.Description;

namespace Accelerator.SalesCloud.Client.Common
{
    internal class OSCConfigVerb
    {
        public string rnt_host { get; set; }
        public Integration integration { get; set; }
    }

    internal class Integration
    {
        public string server_type { get; set; }
        public string sales_base_url { get; set; }
        public string username { get; set; }
        public string password { get; set; }        
        public string sales_timeout { get; set; }
        public string oppty_lead_type { get; set; }
        public string oppty_service_url { get; set; }
        public string acct_service_url { get; set; }
        public string lead_service_url { get; set; }
        public string sales_max_rcvd_msg_size { get; set; }
        public string ctc_service_url { get; set; }
    }
}