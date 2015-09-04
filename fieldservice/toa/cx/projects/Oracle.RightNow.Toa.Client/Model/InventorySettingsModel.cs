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
 *  date: Mon Aug 24 09:01:18 PDT 2015

 *  revision: rnw-15-11-fixes-release-01
*  SHA1: $Id: aae54e39419d9d851a22b4cca7b068987b386e7d $
* *********************************************************************************************
*  File: InventorySettingsModel.cs
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
    public class InventorySettingsModel
    {
        private string[] _keyFields;
        private UploadType _uploadType;

        public InventorySettingsModel()
        {
            _keyFields = new string[] {ActivityProperty.InvTypeLabel};
            _uploadType = UploadType.Full;
        }

        public string[] KeyFields
        {
            get { return _keyFields; }
            set { _keyFields = value; }
        }

        public UploadType UploadType
        {
            get { return _uploadType; }
            set { _uploadType = value; }
        }

        internal InventorySettings GetInventorySettings()
        {
            var settings = new InventorySettings();
            settings.upload_type = ToaStringsUtil.GetString(_uploadType);
            settings.keys = _keyFields;
            return settings;
        }
    }
}
