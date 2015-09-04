/* * *******************************************************************************************
*  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + OFSC Reference Integration
 *  link: http://www-content.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.2 (Feb 2015)
 *  OFSC release: 15.2 (Feb 2015)
 *  reference: 150622-000130
 *  date: Mon Aug 24 09:01:16 PDT 2015

 *  revision: rnw-15-11-fixes-release-01
*  SHA1: $Id: 1460e4b7c6f5b34bbaad5e49270c58cb354a65c0 $
* *********************************************************************************************
*  File: ToaConfigVerbs.cs
* ****************************************************************************************** */

using System.Collections.Generic;
using System.Web.Services.Description;

namespace Oracle.RightNow.Toa.Client.Common
{
    internal class ToaConfigVerbs
    {
        public string rnt_host { get; set; }
        public Integration integration { get; set; } 
    }

    internal class Integration
    {
        public string server_type { get; set; }
        public string fs_base_url { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string company_name { get; set; }
        public string fallback_id { get; set; }
        public string FSServiceTimeout { get; set; }
        public string inbound_api_url { get; set; }
        public string capacity_api_url { get; set; }
        public string activity_api_url { get; set; }
        public string history_api_url { get; set; }        
        public string red_quota_cutoff { get; set; }
        public string green_quota_cutoff { get; set; }
    }
}