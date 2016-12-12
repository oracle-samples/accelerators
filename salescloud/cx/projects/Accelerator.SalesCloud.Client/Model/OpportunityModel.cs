/* * *******************************************************************************************
*  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + OSC Lead Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.11 (November 2015)
 *  OSC release: Release 10
 *  reference: 150505-000122, 160620-000160
 *  date: Mon Sep 19 02:05:27 PDT 2016

 *  revision: rnw-15-11-fixes-release-3
*  SHA1: $Id: 2d5eee501cdbd249b380b4ae476827ea618d3e85 $
* *********************************************************************************************
*  File: OpportunityModel.cs
* ****************************************************************************************** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerator.SalesCloud.Client.Model
{
    public class OpportunityModel
    {
        private long? _keyContactId;
        private string _primaryContactPartyName;
        private long? _salesAccountId;
        private string _targetPartyName;
        private long _optyId;
        private string _optyNumber;
        private string _partyName1;
        private string _emailAddress;
        private string _name;
        private string _description;
        private string _statusCode;
        private string _salesMethod;
        private string _salesStage;
        private string _salesChannelCd;
        private string _currencyCode;
        private decimal? _revenue;
        private decimal? _winProb;
        private string _createdBy;
        private DateTime _creationDate;
        private DateTime? _forecastedCloseDate;
        private long _targetPartyId;
        private long _ownerResourcePartyId;
        private long _opportunityId;
        private OpportunityResourceModel _opportunityResourceModel;
        private bool _targetPartyIdSpecified;
        private bool _ownerResourcePartyIdSpecified;
        private bool _keyContactIdSpecified;

        public bool TargetPartyIdSpecified
        {
            get { return _targetPartyIdSpecified; }
            set { _targetPartyIdSpecified = value; }
        }
        
        public bool OwnerResourcePartyIdSpecified
        {
            get { return _ownerResourcePartyIdSpecified; }
            set { _ownerResourcePartyIdSpecified = value; }
        }
        
        public bool KeyContactIdSpecified
        {
            get { return _keyContactIdSpecified; }
            set { _keyContactIdSpecified = value; }
        }

        public OpportunityResourceModel OpportunityResourceModel
        {
            get { return _opportunityResourceModel; }
            set { _opportunityResourceModel = value; }
        }

        public long? KeyContactId
        {
            get { return _keyContactId; }
            set
            {
                if (value != null)
                {
                    _keyContactId = value;
                }
            }
        }

        public string PrimaryContactPartyName
        {
            get { return _primaryContactPartyName; }
            set { _primaryContactPartyName = value; }
        }

        public long? SalesAccountId
        {
            get { return _salesAccountId; }
            set { _salesAccountId = value; }
        }

        public string TargetPartyName
        {
            get { return _targetPartyName; }
            set { _targetPartyName = value; }
        }

        public long OptyId
        {
            get { return _optyId; }
            set { _optyId = value; }
        }

        public string OptyNumber
        {
            get { return _optyNumber; }
            set { _optyNumber = value; }
        }
        
        public string PartyName1
        {
            get { return _partyName1; }
            set { _partyName1 = value; }
        }
        
        public string EmailAddress
        {
            get { return _emailAddress; }
            set { _emailAddress = value; }
        }
        
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        
        public string StatusCode
        {
            get { return _statusCode; }
            set { _statusCode = value; }
        }
        
        public string SalesMethod
        {
            get { return _salesMethod; }
            set { _salesMethod = value; }
        }
        
        public string SalesStage
        {
            get { return _salesStage; }
            set { _salesStage = value; }
        }
        
        public string SalesChannelCd
        {
            get { return _salesChannelCd; }
            set { _salesChannelCd = value; }
        }
        
        public string CurrencyCode
        {
            get { return _currencyCode; }
            set { _currencyCode = value; }
        }
        
        public decimal? Revenue
        {
            get { return _revenue; }
            set { _revenue = value; }
        }
        
        public decimal? WinProb
        {
            get { return _winProb; }
            set { _winProb = value; }
        }
        
        public string CreatedBy
        {
            get { return _createdBy; }
            set { _createdBy = value; }
        }
        
        public DateTime CreationDate
        {
            get { return _creationDate; }
            set { _creationDate = value; }
        }

        public DateTime? ForecastedCloseDate
        {
            get { return _forecastedCloseDate; }
            set { _forecastedCloseDate = value; }
        }

        public long TargetPartyId
        {
            get { return _targetPartyId; }
            set { _targetPartyId = value; }
        }

        public long OwnerResourcePartyId
        {
            get { return _ownerResourcePartyId; }
            set { _ownerResourcePartyId = value; }
        }

        public long OpportunityId
        {
            get { return _opportunityId; }
            set { _opportunityId = value; }
        }
    }
}
