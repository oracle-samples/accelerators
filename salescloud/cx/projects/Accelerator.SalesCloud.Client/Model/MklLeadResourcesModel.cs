/* * *******************************************************************************************
*  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + OSC Lead Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.11 (November 2015)
 *  OSC release: Release 10
 *  reference: 150505-000122
 *  date: Tue Dec  1 21:42:19 PST 2015

 *  revision: rnw-15-11-fixes-release-2
*  SHA1: $Id: 081769cdfb6787576911bff362a73d99d1979578 $
* *********************************************************************************************
*  File: MklLeadResourcesModel.cs
* ****************************************************************************************** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accelerator.SalesCloud.Client.Model
{
    public class MklLeadResourcesModel
    {
        private long _resourceId;
        private bool _primaryFlag;
        private bool _resourceIdSpecified;
        private bool _primaryFlagSpecified;

        public long ResourceId
        {
            get { return _resourceId; }
            set { _resourceId = value; }
        }
        
        public bool PrimaryFlag
        {
            get { return _primaryFlag; }
            set { _primaryFlag = value; }
        }
        
        public bool ResourceIdSpecified
        {
            get { return _resourceIdSpecified; }
            set { _resourceIdSpecified = value; }
        }
        
        public bool PrimaryFlagSpecified
        {
            get { return _primaryFlagSpecified; }
            set { _primaryFlagSpecified = value; }
        }
    }
}
