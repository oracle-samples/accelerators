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
*  SHA1: $Id: af2b46eeff4263185e3f19f5374ade9a4b332237 $
* *********************************************************************************************
*  File: LeadModel.cs
* ****************************************************************************************** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accelerator.SalesCloud.Client.Model
{
    public class LeadModel
    {
        private string _name;
        private long _customerId;
        private bool _customerIdSpecified;
        private long _leadId;
        private long _primaryContactId;
        private bool _primaryContactIdSpecified;
        private long _ownerId;
        private bool _ownerIdSpecified;
        private MklLeadResourcesModel _mklLeadResourcesModel;

        public bool PrimaryContactIdSpecified
        {
            get { return _primaryContactIdSpecified; }
            set { _primaryContactIdSpecified = value; }
        }
        
        public bool CustomerIdSpecified
        {
            get { return _customerIdSpecified; }
            set { _customerIdSpecified = value; }
        }

        public bool OwnerIdSpecified
        {
            get { return _ownerIdSpecified; }
            set { _ownerIdSpecified = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public long CustomerId
        {
            get { return _customerId; }
            set { _customerId = value; }
        }
        
        public long PrimaryContactId
        {
            get { return _primaryContactId; }
            set { _primaryContactId = value; }
        }
        
        public long OwnerId
        {
            get { return _ownerId; }
            set { _ownerId = value; }
        }
        
        public long LeadId
        {
            get { return _leadId; }
            set { _leadId = value; }
        }
        
        public MklLeadResourcesModel MklLeadResourcesModel
        {
            get { return _mklLeadResourcesModel; }
            set { _mklLeadResourcesModel = value; }
        }
    }
}
