/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC Contact Center + Siebel Case Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  Siebel release: 8.1.1.15
 *  reference: 150520-000047
 *  date: Thu Nov 12 00:55:37 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: c1bcbce36ceb9b148d1a7270b3915c48409065f5 $
 * *********************************************************************************************
 *  File: ModelObjectBase.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Accelerator.Siebel.SharedServices.Providers;

namespace Accelerator.Siebel.SharedServices
{
    public abstract class ModelObjectBase
    {
        public static string ServiceUsername { get; set; }
        public static string ServicePassword { get; set; }
        public static string ServiceProvider { get; set; }
        public static int ServiceClientTimeout { get; set; } // come from ConfigurationSetting SiebelServiceTimeout

        public string ToJSON()
        {
            StringBuilder result = new StringBuilder(1024).Append("{\"").Append(this.GetType().Name)
            .Append("\":[{\"Attribute\": \"Value\"");
            foreach (PropertyInfo property in this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                result.Append(",\"" + property.Name + "\": \"" + Convert.ToString(property.GetValue(this, null)) + "\"");
            }
            return result.Append("}]}").ToString();
        }
    }
}
