/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + EBS Enhancement
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.5 (May 2015)
 *  EBS release: 12.1.3
 *  reference: 150202-000157
 *  date: Wed Sep  2 23:11:36 PDT 2015

 *  revision: rnw-15-8-fixes-release-01
 *  SHA1: $Id: def271a0da7659dcfbdbb5b3aa39ef1e30456158 $
 * *********************************************************************************************
 *  File: RepairOrder.cs
 * *********************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Accelerator.EBS.BulkImportAddin
{
    public sealed class RepairOrder : INotifyPropertyChanged
    {
        private bool _Selected;
        public bool Selected
        {
            get
            { return _Selected; }
            internal set
            {
                if (_Selected != value)
                {
                    _Selected = value;
                    OnPropertyChanged("Selected");
                }
            }
        }

        private ImportStatus _Status;
        [DisplayName("ImportStatus")]
        public ImportStatus Status
        {
            get
            { return _Status; }
            set
            {
                if (_Status != value)
                {
                    _Status = value;
                    OnPropertyChanged("Status");
                }
            }
        }

        public enum ImportStatus
        {
            New = 10,
            Imported = 20,
            Error = 30,
        }

        [DisplayName("UNIT__OF__MEASURE")]
        public string UnitOfMeasure { get; private set; }

        [DisplayName("REPAIR__TYPE__ID")]
        public decimal? RepairTypeId { get; private set; }

        [DisplayName("QUANTITY")]
        public decimal? Quantity { get; private set; }

        [DisplayName("INVENTORY__ITEM__ID")]
        internal decimal? InventoryItemId { get; private set; }

        [DisplayName("APPROVAL__REQUIRED__FLAG")]
        public string ApprovalRequiredFlag { get; private set; }

        [DisplayName("CURRENCY__CODE")]
        public string Currency { get; private set; }

        [DisplayName("PROBLEM__DESCRIPTION")]
        public string ProblemDescription { get; private set; }

        [DisplayName("RESOURCE__ID")]
        internal decimal? ResourceId { get; private set; }

        [DisplayName("SERIAL__NUMBER")]
        public string SerialNumber { get; internal set; }

        private decimal? _serviceRequestId;

        internal RepairOrder(
            ImportFile file,
            decimal serviceRequestId, 
            string problemDescription,
            decimal inventoryItemID,
            string approvalRequiredFlag,
            decimal typeID,
            decimal quantity,
            string unit,
            string currency,
            decimal? resourceID
            )
        {
            _File = file;
            _serviceRequestId = serviceRequestId;
            ProblemDescription = problemDescription;
            InventoryItemId = inventoryItemID;
            ApprovalRequiredFlag = approvalRequiredFlag;
            RepairTypeId = typeID;
            Quantity = quantity;
            UnitOfMeasure = unit;
            Currency = currency;
            ResourceId = resourceID;
            _Status = ImportStatus.New;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(String propertyName)
        {
            if (null != PropertyChanged)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        internal bool Import()
        {
            if(!this.Selected)
            {
                return true;
            }
            bool result = _File._Proxy.Import(this);
            if (result)
                Status = ImportStatus.Imported;
            else
                Status = ImportStatus.Error;
            Selected = false;
            return result;
        }

        internal ImportFile _File { get; set; }
    }
}
