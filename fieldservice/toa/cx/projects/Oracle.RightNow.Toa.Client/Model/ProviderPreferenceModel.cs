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
*  SHA1: $Id: 500d64c0106bf760767c648a628ed49a24c579a1 $
* *********************************************************************************************
*  File: ProviderPreferenceModel.cs
* ****************************************************************************************** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.RightNow.Toa.Client.Common;
using Oracle.RightNow.Toa.Client.InboundProxyService;

namespace Oracle.RightNow.Toa.Client.Model
{
    public class ProviderPreferenceModel
    {
        private string _externalId;
        private PreferenceType _preferenceType;

        /// <summary>
        /// External Id - ID of the resource for whom the preference is set
        /// </summary>
        public string ExternalId
        {
            get { return _externalId; }
            set { _externalId = value; }
        }

        /// <summary>
        /// Preference Type
        /// </summary>
        public PreferenceType PreferenceType
        {
            get { return _preferenceType; }
            set { _preferenceType = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal ProviderPreferenceElement GetProviderPreferenceElement()
        {
            var providerPreferenceElement = new ProviderPreferenceElement();
            return providerPreferenceElement;
        }
    }
}
