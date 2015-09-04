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
 *  date: Thu Sep  3 23:14:01 PDT 2015

 *  revision: rnw-15-11-fixes-release-03
*  SHA1: $Id: b67a2e9b8730c910b1a07a9c85e219ba30137b97 $
* *********************************************************************************************
*  File: ReportMessageModel.cs
* ****************************************************************************************** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oracle.RightNow.Toa.Client.Model
{
    public class ReportMessageModel
    {
        public ReportMessageModel(string result, string type, string code, string description)
        {
            Result = result;
            Type = type;
            Code = code;
            Description = description;
        }

        public string Result { get; internal set; }
        public string Type { get; internal set; }
        public string Code { get; internal set; }
        public string Description { get; internal set; }
    }
}
