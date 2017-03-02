/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  Copyright (c) 2014, 2015, 2016, 2017 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + SRM Integration Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 17.2 (February 2017) 
 *  reference: 160628-000117
 *  date: Fri Feb 10 19:47:42 PST 2017
 
 *  revision: rnw-17-2-fixes-release-2
 *  SHA1: $Id: bd78277aa888e26345034994940bebc1195e64a1 $
 * *********************************************************************************************
 *  File: ConfigVerb.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerator.SRM.SharedServices
{
    public class Header
    {
        public string name { get; set; }
        public object value { get; set; }
    }

    public class Update
    {
        public string relative_path { get; set; }
    }

    public class Conversation
    {
        public Update update { get; set; }
    }

    public class List
    {
        public string relative_path { get; set; }
    }

    public class Create
    {
        public string relative_path { get; set; }
    }

    public class ConversationReply
    {
        public List list { get; set; }
        public Create create { get; set; }
    }

    public class ExtServices
    {
        public Conversation conversation { get; set; }
        public ConversationReply conversation_reply { get; set; }
    }

    public class Integration
    {
        public string server_type { get; set; }
        public string ext_base_url { get; set; }
        public List<ChannelConstraints> channel_constraints { get; set; }
        public string log_type { get; set; }
        public string log_level { get; set; }
        public int max_srm_rows_fetch { get; set; }
        public ExtServices ext_services { get; set; }
    }

    public class Host
    {
        public string rnt_host { get; set; }
        public Integration integration { get; set; }
    }

    public class ConfigVerb
    {
        public int AdminProfileID { get; set; }
        public List<Host> hosts { get; set; }
    }

    public class ChannelConstraints
    {
        public string channel { get; set; }
        public List<Constraint> constraints { get; set; }
    }

    public class Constraint{
        public string reply_mode { get; set; }
        public int char_limit { get; set; }
        public bool include_social_handle { get; set; }

    }
}

