/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + EBS Enhancement
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  EBS release: 12.1.3
 *  reference: 150505-000099, 150420-000127
 *  date: Thu Nov 12 00:52:44 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: 5b55711e499b7a550d524e52fe3d962baad37b13 $
 * *********************************************************************************************
 *  File: SalesItemViewModel.cs
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

namespace Accelerator.EBS.OrderManagementAddin
{
    public sealed class SalesItemViewModel : INotifyPropertyChanged
    {
        public string Title { get { return string.Format("{0}    ${1}", ItemName, UnitSellingPrice); } }

        internal int LineID = 0;

        [DisplayName("ItemId")]
        public decimal? InventoryItemId { get; set; }

        private string _ItemName;
        [DisplayName("ItemName")]
        public string ItemName 
        {
            get { return _ItemName; }
            set
            {
                if (_ItemName != value)
                {
                    _ItemName = value;
                    _ItemNameUpper = _ItemName.ToUpperInvariant();
                    OnPropertyChanged("ItemName");
                    OnPropertyChanged("Title");
                }
            }
        }
        internal string _ItemNameUpper = "";

        private bool _Selected;
        internal bool Booked
        {
            get
            { return _Selected; }
            set
            {
                if (_Selected != value)
                {
                    _Selected = value;
                    OnPropertyChanged("Booked");
                }
            }
        }

        public enum ImportStatus
        {
            ENTERED = 10,
            Backordered = 20,
            Waiting = 30,
        }

        private decimal? _UnitSellingPrice;
        [DisplayName("UnitSellingPrice")]
        public decimal? UnitSellingPrice
        {
            get { return _UnitSellingPrice; }
            set
            {
                if (_UnitSellingPrice != value)
                {
                    _UnitSellingPrice = value;
                    OnPropertyChanged("UnitSellingPrice");
                    OnPropertyChanged("TotalPrice");
                    OnPropertyChanged("Title");
                }
            }
        }

        [DisplayName("Currency")]
        public string Currency { get; set; }

        private decimal? _Quantity;
        [DisplayName("Qty")]
        public decimal? Quantity {
            get { return _Quantity; }
            set
            {
                if (_Quantity != value)
                {
                    _Quantity = value;
                    OnPropertyChanged("Quantity");
                    OnPropertyChanged("TotalPrice");
                }
            }
        }

        [DisplayName("Units")]
        public string UnitOfMeasure { get; set; }

        [DisplayName("TotalPrice")]
        public decimal? TotalPrice
        {
            get { return Quantity * (null == UnitSellingPrice? 0 : UnitSellingPrice); }
        }

        [DisplayName("CANCELLED__FLAG")]
        internal string ApprovalRequiredFlag { get; set; }

        [DisplayName("RESOURCE__ID")]
        internal decimal? ResourceId { get; set; }

        [DisplayName("ItemType")]
        internal string SerialNumber { get; set; }

        private ImportStatus _Status;
        [DisplayName("Status")]
        internal ImportStatus Status
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

        private decimal? _serviceRequestId;

        internal SalesItemViewModel(
            SalesOrderViewModel file,
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
            ItemName = problemDescription;
            InventoryItemId = inventoryItemID;
            ApprovalRequiredFlag = approvalRequiredFlag;
            _UnitSellingPrice = typeID;
            _Quantity = quantity;
            UnitOfMeasure = unit;
            Currency = currency;
            ResourceId = resourceID;
            _Status = ImportStatus.ENTERED;
        }

        public SalesItemViewModel(SalesItemViewModel selected)
        {
            _File = selected._File;
            _serviceRequestId = selected._serviceRequestId;
            ItemName = selected.ItemName;
            InventoryItemId = selected.InventoryItemId;
            ApprovalRequiredFlag = selected.ApprovalRequiredFlag;
            _UnitSellingPrice = selected._UnitSellingPrice;
            _Quantity = selected._Quantity;
            UnitOfMeasure = selected.UnitOfMeasure;
            Currency = selected.Currency;
            ResourceId = selected.ResourceId;
            _Status = selected._Status;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(String propertyName)
        {
            if (null != PropertyChanged)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        internal bool Import()
        {
            if(!this.Booked)
            {
                return true;
            }
            bool result = _File._Proxy.Import(this);
            if (result)
                Status = ImportStatus.Backordered;
            else
                Status = ImportStatus.Waiting;
            Booked = false;
            return result;
        }

        internal SalesOrderViewModel _File { get; set; }
    }
}
