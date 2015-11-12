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
 *  SHA1: $Id: dd90f2a83555fb8450bdb35397d0ef60b7fe358f $
 * *********************************************************************************************
 *  File: OrderManagementViewModel.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace Accelerator.EBS.OrderManagementAddin
{
    public class OrderManagementViewModel : INotifyPropertyChanged
    {
        private string _ServerType;
        public string ServerType
        {
            get { return _ServerType; }
            set
            {
                if (_ServerType != value)
                {
                    _ServerType = value;
                    OnPropertyChanged("ServerType");
                    OnPropertyChanged("HelpLine01");
                }
            }
        }

        public string HelpLine01
        {
            get 
            {
                return string.Format(@"{0} Order Management Addin manages sales orders in {0}.
It is designed to work inside an incident workspace that has been customized for {0}. 
You can use Analytics to supplement the summary information shown in this Addin with customizable details.", ServerType); 
            }
        }

        private string _Version = "";
        public string Version
        {
            get { return _Version; }
            set
            {
                if (_Version != value)
                {
                    _Version = value;
                    OnPropertyChanged("Version");
                }
            }
        }

        public async void Save()
        {
            IsRunning = true;
            await SaveAsync();
            IsRunning = false;
        }

        internal async Task<bool> SaveAsync()
        {
            bool result = false;
            foreach (var parent in ParentList)
            {
                await Task.Run(() => { parent.Save(); });
            }
            result = true;
            return result;
        }

        public void SaveAndClose()
        {
        }

        public void Refresh()
        {
            Render(true);
        }

        public bool IsDirty()
        {
            foreach (var parent in ParentList)
            {
                if (0 == parent.HeaderId)
                {
                    return true;
                }
            }
            return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(String propertyName)
        {
            if (null != PropertyChanged)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private SalesOrderViewModel _CurrentParent;
        public SalesOrderViewModel CurrentParent
        {
            get 
            {
                //if (FileList.IsEmpty) return _DefaultFile;
                //return FileList.CurrentItem as SalesOrderViewModel;
                return _CurrentParent; 
            }
            set
            {
                if (_CurrentParent != value)
                {
                    _CurrentParent = value;
                    OnPropertyChanged("CurrentParent");
                }
            }
        }

        internal string _Filter;
        internal Proxy _Proxy;
        public string Filter
        {
            get { return _Filter; }
            set
            {
                if (_Filter != value)
                {
                    _Filter = value;
                    _FilterUpper = _Filter.ToUpperInvariant();
                    _Control.ResetGrid(true);
                    using (FileList.DeferRefresh())
                    {
                        _AddSortDescription();
                        OnPropertyChanged("Filter");
                    }
                    if (!FileList.IsEmpty) _Control.ResetGrid(false);
                }
            }
        }

        internal string _ItemFilterUpper;
        internal string _ItemsFilter = "";
        public string ItemsFilter
        {
            get { return _ItemsFilter; }
            set
            {
                if (_ItemsFilter != value)
                {
                    _ItemsFilter = value;
                    _ItemFilterUpper = _ItemsFilter.ToUpperInvariant();
                    using (AvailableItemsFiltered.DeferRefresh())
                    {
                        ItemsAddSortDescription();
                        OnPropertyChanged("ItemsFilter");
                    }
                }
            }
        }


        private string _FilterUpper;

        private bool _IsExpanded;
        internal OrderManagementControl _Control;
        public bool IsExpanded
        {
            get { return _IsExpanded; }
            set
            {
                if (_IsExpanded != value)
                {
                    _IsExpanded = value;
                    OnPropertyChanged("IsExpanded");
                    OnPropertyChanged("StatusHeight");
                }
            }
        }

        public int StatusHeight
        {
            get { return IsExpanded ? 0 : 30; }
        }

        public bool IsIncomplete
        {
            get
            {
                bool result = false;
                bool exitLoop;
                foreach (var f in ParentList)
                {
                    switch (f._JobStatus)
                    {
                        case SalesOrderViewModel.JobStatus.Done:
                        case SalesOrderViewModel.JobStatus.DoneError:
                            result = true;
                            exitLoop = true;
                            break;
                        default:
                            result = false;
                            exitLoop = false;
                            break;
                    }
                    if (exitLoop) break;
                }
                return result;
            }
        }

        private bool _IsRunning;
        public bool IsRunning
        {
            get { return _IsRunning; }
            set
            {
                if (_IsRunning != value)
                {
                    _IsRunning = value;
                    OnPropertyChanged("IsRunning");
                    OnPropertyChanged("IsNotRunning");
                }
            }
        }

        public bool IsNotRunning
        {
            get { return !IsRunning || InDemo; }
        }


        private ObservableCollection<SalesOrderViewModel> _ParentList;
        public ObservableCollection<SalesOrderViewModel> ParentList
        {
            get { return _ParentList; }
        }

        private ICollectionView _FileList;
        public ICollectionView FileList 
        {
            get { return _FileList; }
        }

        internal SalesOrderViewModel _DefaultFile;

            // construct the view model
        internal OrderManagementViewModel(Proxy proxy, OrderManagementControl control)
        {
            _Control = control;
            _Proxy = proxy;
            _Filter = String.Empty;
            _FilterUpper = string.Empty;
            _IsExpanded = true;
            _ParentList = new ObservableCollection<SalesOrderViewModel>();
            _FileList = new ListCollectionView(ParentList);
            FileList.Filter = FileList_Filter;
            using (FileList.DeferRefresh())
            {
                _AddSortDescription();
            }
            _LoadedAvailableItems = false;
            _AvailableItems = new ObservableCollection<SalesItemViewModel>();
            _AvailableItemsFiltered = new ListCollectionView(_AvailableItems);
            AvailableItemsFiltered.Filter = AvailableItemsFilter;
            using (AvailableItemsFiltered.DeferRefresh())
            {
                ItemsAddSortDescription();
            }
            ItemsFilter = "";
            _Proxy._InitializedControl(this);
            _DefaultFile = new SalesOrderViewModel(this, 0, string.Empty, string.Empty, string.Empty);
        }

        private void _AddSortDescription()
        {
            FileList.SortDescriptions.Clear();
        }
           
            // file list filter
        private bool FileList_Filter(object obj)
        {
            var file = obj as SalesOrderViewModel;
            if (null == file) return false;
            if (string.IsNullOrWhiteSpace(Filter)) return true;
            if (file._TitleUpper.Contains(_FilterUpper))
                return true;
            return false;
        }

        private void ItemsAddSortDescription()
        {
            AvailableItemsFiltered.SortDescriptions.Clear();
        }

        // Items filter
        private bool AvailableItemsFilter(object obj)
        {
            var item = obj as SalesItemViewModel;
            if (null == item) return false;
            if (string.IsNullOrWhiteSpace(ItemsFilter)) return true;
            if (item._ItemNameUpper.Contains(_ItemFilterUpper))
                return true;
            return false;
        }

            // load the file
        internal void loadFile(SalesOrderViewModel importFile)
        {
            importFile.Load();
        }

        internal void Render(bool reload)
        {
            if (!reload)
            {
                IsExpanded = !IsExpanded;
                IsExpanded = !IsExpanded;
                return;
            }
            IsExpanded = false;
            _Control.ResetGrid(true);
            Filter = string.Empty;
            ParentList.Clear();
            foreach (var file in _Proxy.GetParentList())
            {
                ParentList.Add(file);
            }
            using (FileList.DeferRefresh())
            {
                _AddSortDescription();
                IsExpanded = true;
            }
            CurrentParent = _DefaultFile;
            if (0 != ParentList.Count)
            {
                CurrentParent = ParentList.First();
                _Control.ResetGrid(false);
            }
            CurrentParent.Load();            
            IsRendered = true;
            PopulateAvailableItems();        
        }

        private void PopulateAvailableItems()
        {
            if (_LoadedAvailableItems) return;            
            _LoadedAvailableItems = true;
            var items = _Proxy.GetAvailableItems();
            foreach (var item in items)
            {
                AvailableItems.Add(item);
            }
        }

        private ObservableCollection<SalesItemViewModel> _AvailableItems;
        public ObservableCollection<SalesItemViewModel> AvailableItems
        {
            get { return _AvailableItems; }
            set
            {
                if (_AvailableItems != value)
                {
                    _AvailableItems = value;
                    OnPropertyChanged("AvailableItems");
                    OnPropertyChanged("AvailableItemsFiltered");
                }
            }
        }

        private ICollectionView _AvailableItemsFiltered;
        public ICollectionView AvailableItemsFiltered
        {
            get { return _AvailableItemsFiltered; }
            set
            {
                if (_AvailableItemsFiltered != value)
                {
                    _AvailableItemsFiltered = value;
                    OnPropertyChanged("AvailableItemsFiltered");
                }
            }
        }


        private bool AlreadyPresent(SalesOrderViewModel file)
        {
            foreach (var f in ParentList)
            {
                if (f.FileId == file.FileId)
                    return true;
            }
            return false;
        }



        public bool IsRendered { get; set; }
        public decimal EbsSrId { get; set; }
        public decimal EbsContactOrgId { get; set; }
        public decimal EbsOwnerId { get; set; }
        public int RntIncidentId { get; set; }
        public int RntContactId { get; set; }

        public bool IsRecordLoaded { get; set; }

        public bool InDesignMode { get; set; }

        private bool _InDemo;
        public bool _LoadedAvailableItems;
        public bool InDemo
        {
            get { return _InDemo; }
            set
            {
                if (_InDemo != value)
                {
                    _InDemo = value;
                    OnPropertyChanged("InDemo");
                    EbsSrId = 98765;
                    EbsOwnerId = 100003628;
                }
            }
        }

        public int MaxPreview { get; set; }

        internal SalesOrderViewModel StartNewOrder()
        {
            SalesOrderViewModel order = new SalesOrderViewModel(this, 0, "New Order", "New Order", "New Order");
            order._LoadedAll = true;
            order._JobStatus = SalesOrderViewModel.JobStatus.Done;
            return order;
        }
    }
}
