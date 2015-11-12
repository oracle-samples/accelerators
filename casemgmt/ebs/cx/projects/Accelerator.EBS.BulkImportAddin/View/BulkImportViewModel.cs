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
 *  date: Thu Nov 12 00:52:42 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: f9721e86f2f1464d1eca4924269234331e62b3ca $
 * *********************************************************************************************
 *  File: BulkImportViewModel.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Threading;

namespace Accelerator.EBS.BulkImportAddin
{
    public sealed class BulkImportViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(String propertyName)
        {
            if (null != PropertyChanged)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private ImportFile _CurrentFile;
        public ImportFile CurrentFile
        {
            get { return _CurrentFile; }
            set
            {
                if (_CurrentFile != value)
                {
                    _CurrentFile = value;
                    OnPropertyChanged("CurrentFile");
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
                    _Control.ResetGrid();
                    using (FileList.DeferRefresh())
                    {
                        _AddSortDescription();
                        OnPropertyChanged("Filter");
                    }
                }
            }
        }

        private string _FilterUpper;

        private bool _IsExpanded;
        internal BulkImportControl _Control;
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
                foreach (var f in ImportFileList)
                {
                    switch (f._JobStatus)
                    {
                        case ImportFile.JobStatus.Done:
                        case ImportFile.JobStatus.DoneError:
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
                }
            }
        }

        private ObservableCollection<ImportFile> _ImportFileList;
        public ObservableCollection<ImportFile> ImportFileList
        {
            get { return _ImportFileList; }
        }

        private ICollectionView _FileList;
        public ICollectionView FileList 
        {
            get { return _FileList; }
        }
            // construct the view model
        internal BulkImportViewModel(Proxy proxy, BulkImportControl control)
        {
            _Control = control;
            _Proxy = proxy;
            _Filter = String.Empty;
            _FilterUpper = string.Empty;
            _IsExpanded = true;
            _ImportFileList = new ObservableCollection<ImportFile>();
            _FileList = new ListCollectionView(ImportFileList);
            FileList.Filter = FileList_Filter;
            using (FileList.DeferRefresh())
            {
                _AddSortDescription();
            }
            _Proxy._InitializedControl(this);
        }

        private void _AddSortDescription()
        {
            FileList.SortDescriptions.Clear();
            FileList.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Ascending));
        }
            
            // file list filter
        private bool FileList_Filter(object obj)
        {
            var file = obj as ImportFile;
            if (null == file) return false;
            if (string.IsNullOrWhiteSpace(Filter)) return true;
            if (file._TitleUpper.Contains(_FilterUpper))
                return true;
            return false;
        }

            // load the file
        internal void loadFile(ImportFile importFile)
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
            _Control.ResetGrid();
            Filter = string.Empty;
            ImportFileList.Clear();
            foreach (var file in _Proxy.GetImportFileList())
            {
                ImportFileList.Add(file);
            }
            using (FileList.DeferRefresh())
            {
                _AddSortDescription();
                IsExpanded = true;
            }
            IsRendered = true;
        }

        private bool AlreadyPresent(ImportFile file)
        {
            foreach (var f in ImportFileList)
            {
                if (f.FileId == file.FileId)
                    return true;
            }
            return false;
        }

        internal void ShowNotificationWindow(string msg)
        {
            if (String.IsNullOrWhiteSpace(msg)) return;
            new NotificationBubble(msg);
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
        public bool InDemo
        {
            get { return _InDemo; }
            set
            {
                if (_InDemo != value)
                {
                    _InDemo = value;
                    OnPropertyChanged("InDemo");
                }
            }
        }

        public int MaxPreview { get; set; }
    }

}
