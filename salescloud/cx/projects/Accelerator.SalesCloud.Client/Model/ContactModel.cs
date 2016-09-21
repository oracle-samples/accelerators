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
 *  date: Mon Sep 19 02:05:26 PDT 2016

 *  revision: rnw-15-11-fixes-release-3
 *  SHA1: $Id: eaa548eb65f80f7c089ba4bbf59e69dbd278befd $
 * *********************************************************************************************
 *  File: ContactModel.cs
 * ****************************************************************************************** */

namespace Oracle.RightNow.OSC.Client.Model
{
    public class ContactModel
    {
        private long partyId;

        public long PartyId
        {
            get { return partyId; }
            set { partyId = value; }
        }

        private long? ownerPartyId;

        public long? OwnerPartyId
        {
            get { return ownerPartyId; }
            set { ownerPartyId = value; }
        }
    }
}
