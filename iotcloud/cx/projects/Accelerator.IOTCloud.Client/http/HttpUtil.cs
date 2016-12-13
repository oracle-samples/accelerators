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
 *  SHA1: $Id: 71a1eb48e60a20d7ea9b4b856186e4aa1779fb38 $
 * *********************************************************************************************
 *  File: HttpUtil.cs
 * ****************************************************************************************** */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accelerator.IOTCloud.Client.Http
{
    public class HttpUtil
    {
        public static string QueryString(string key, string value)
        {

            if (key == null || value == null) 
                return String.Empty;

            return string.Format(string.Join("&", string.Format("{0}={1}", key, value)));
        }

        public static string QueryString(Dictionary<string, string> parameters)
        {
            if (parameters == null) 
                return String.Empty;

            return string.Format(string.Join("&",
                                        parameters.Select(kvp =>
                                                    string.Format("{0}={1}", kvp.Key, kvp.Value))));
        }
    }
}
