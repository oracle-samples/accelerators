/* * *******************************************************************************************
*  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015,2016 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + OSC Lead Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.11 (November 2015)
 *  OSC release: Release 10
 *  reference: 150505-000122, 160620-000160
 *  date: Mon Sep 19 02:05:27 PDT 2016

 *  revision: rnw-15-11-fixes-release-3
*  SHA1: $Id: aba21878e415ae7f5666cf5e508544e0f8e6e2f7 $
* *********************************************************************************************
*  File: OpportunityResourceModel.cs
* ****************************************************************************************** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accelerator.SalesCloud.Client.Model
{
    public class OpportunityResourceModel
    {
        private long _resourceId;

        public long ResourceId
        {
            get { return _resourceId; }
            set { _resourceId = value; }
        }

        private bool _resourceIdSpecified;

        public bool ResourceIdSpecified
        {
            get { return _resourceIdSpecified; }
            set { _resourceIdSpecified = value; }
        }

        private bool _ownerFlagSpecified;

        public bool OwnerFlagSpecified
        {
            get { return _ownerFlagSpecified; }
            set { _ownerFlagSpecified = value; }
        }

        private bool _ownerFlag;

        public bool OwnerFlag
        {
            get { return _ownerFlag; }
            set { _ownerFlag = value; }
        }
    }
}
