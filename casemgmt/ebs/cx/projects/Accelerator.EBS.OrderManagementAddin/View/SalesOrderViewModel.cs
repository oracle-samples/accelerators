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
 *  SHA1: $Id: f34d72ab174ea311460284a9a129b0bb5a1e6bab $
 * *********************************************************************************************
 *  File: SalesOrderViewModel.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Accelerator.EBS.OrderManagementAddin
{
    public sealed class SalesOrderViewModel : INotifyPropertyChanged
    {
        private int _FileId;
        public int FileId { get { return _FileId; } }
        public int StatusCode { get { return (int)_JobStatus; } }

        private string _OrderStatus;
        public string OrderStatus
        {
            get
            {
                return _OrderStatus;
            }
            set
            {
                if (_OrderStatus != value)
                {
                    _OrderStatus = value;
                    OnPropertyChanged("OrderStatus");
                }
            }
        }

        private DateTime? _DateCreated;
        public DateTime? DateCreated
        {
            get
            {
                return _DateCreated;
            }
            set
            {
                if (_DateCreated != value)
                {
                    _DateCreated = value;
                    OnPropertyChanged("DateCreated");
                }
            }
        }

        private DateTime? _DateUpdated;
        public DateTime? DateUpdated
        {
            get
            {
                return _DateUpdated;
            }
            set
            {
                if (_DateUpdated != value)
                {
                    _DateUpdated = value;
                    OnPropertyChanged("DateUpdated");
                }
            }
        }


        internal JobStatus GetStatusFromCode(int statusCode)
        {
            if (Enum.IsDefined(typeof(JobStatus), statusCode))
                return (JobStatus)statusCode;
            else
                return JobStatus.Undefined;
        }

        internal JobStatus _JobStatus;
        internal enum JobStatus
            {
                Load = 10,
                Reload = 15,
                Loading = 20,
                ParseError = 30,
                Loaded = 40,
                Importing = 50,
                Done = 60,
                DoneError = 65,
                AllDone = 70,
                AllDoneError = 75,
                Undefined = 90,
            }
    
        private bool _IsShowAllEnabled;
        public bool IsShowAllEnabled
        {
            get { return _IsShowAllEnabled; }
            set
            {
                if (_IsShowAllEnabled != value)
                {
                    _IsShowAllEnabled = value;
                    OnPropertyChanged("IsShowAllEnabled");
                }
            }
        }

        

        private bool _IsNewItemEnabled;
        public bool IsNewItemEnabled
        {
            get 
            {
                if (0 == HeaderId) return true;
                return _IsNewItemEnabled; 
            }
            set
            {
                if (_IsNewItemEnabled != value)
                {
                    _IsNewItemEnabled = value;
                    OnPropertyChanged("IsNewItemEnabled");
                }
            }
        }

        private bool _IsRemoveEnabled;
        public bool IsRemoveEnabled
        {
            get 
            { return _IsRemoveEnabled; }
            set
            {
                if (_IsRemoveEnabled != value)
                {   
                    _IsRemoveEnabled = value;
                    OnPropertyChanged("IsRemoveEnabled");
                }
            }
        }

        private bool _IsLoading;
        public bool IsLoading   
        {
            get
            { return _IsLoading; }
            set
            {
                if (_IsLoading != value)
                {
                    _IsLoading = value;
                    OnPropertyChanged("IsLoading");
                }
            }
        }

        private int _ProgressPercent;
        public int ProgressPercent
        {
            get { return _ProgressPercent; }
            set
            {
                if (_ProgressPercent != value)
                {
                    _ProgressPercent = value;
                    OnPropertyChanged("ProgressPercent");
                }
            }
        }

        private int _HeaderId;
        public int HeaderId
        {
            get { return _HeaderId; }
            set
            {
                if (_HeaderId != value)
                {
                    _HeaderId = value;
                    _TitleUpper = Title.ToUpperInvariant();
                    OnPropertyChanged("HeaderId");
                    OnPropertyChanged("Description");
                    OnPropertyChanged("Title");
                    OnPropertyChanged("IsNewItemEnabled");
                }
            }
        }

        private int _CountProcessed;
        public int CountProcessed
        {
            get { return _CountProcessed; }
            set
            {
                if (_CountProcessed != value)
                {
                    _CountProcessed = value;
                    OnPropertyChanged("CountProcessed");
                }
            }
        }

        private int _CountError;
        public int CountError
        {
            get { return _CountError; }
            set
            {
                if (_CountError != value)
                {
                    _CountError = value;
                    OnPropertyChanged("CountError");
                }
            }
        }

        private int _CountCumulativeError;
        public int CumulativeCountError
        {
            get { return _CountCumulativeError; }
            set
            {
                if (_CountCumulativeError != value)
                {
                    _CountCumulativeError = value;
                    OnPropertyChanged("CumulativeCountError");
                }
            }
        }

        private int _CumulativeCountProcessed;
        public int CumulativeCountProcessed
        {
            get { return _CumulativeCountProcessed; }
            set
            {
                if (_CumulativeCountProcessed != value)
                {
                    _CumulativeCountProcessed = value;
                    OnPropertyChanged("CumulativeCountProcessed");
                }
            }
        }

        private int _CountSelected;
        public int CountSelected
        {
            get { return _CountSelected; }
            set
            {
                if (_CountSelected != value)
                {
                    _CountSelected = value;
                    OnPropertyChanged("CountSelected");
                }
            }
        }

        private string _Name;
        public string Name
        {
            get 
            {
                return _Name; 
            }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        internal string _TitleUpper;

        private string _Title;
        public string Title
        {
            get
            {
                if (0 == HeaderId) return _Title;
                return string.Format("Order {0}", HeaderId);
            }
        }

        private JobStatus _PreviousStatus;
        internal JobStatus PreviousStatus
        {
            get
            {
                return _PreviousStatus;
            }
        }

        public string PreviewName
        {
            get
            {
                string ps = string.Empty;
                return ps;

                //if (JobStatus.AllDone == PreviousStatus) ps = string.Format(" (previous import: {0})", JobStatus.AllDone.ToString());
                //if (JobStatus.AllDoneError == PreviousStatus) ps = string.Format(" (previous import: {0})", JobStatus.AllDoneError.ToString());
                //string n = Name;
                //if (String.IsNullOrWhiteSpace(n)) return String.Empty;
                //string t = Title;

                    //// uncomment only if for preview
                //if (JobStatus.Loading == _JobStatus && !_LoadedAll && !IsShowAllEnabled) t = "Loading " + t;
                //else if (IsShowAllEnabled) t = "Loading " + t;
                    //// uncomment only if the name and title are different
                //return n == Title ? t : String.Format("{0} ({1})", t, n);
                //return "Order Number: " + t + ps;
            }
        }

        private string _Status;
        public string Status
        {
            get 
            { return _Status; }
            set
            {
                if (_Status != value)
                {
                    _Status = value;
                    OnPropertyChanged("Status");
                    OnPropertyChanged("LongStatus");
                    OnPropertyChanged("ImportStatus");
                }
            }
        }

        private string _LongStatus;
        public string LongStatus
        {
            get
            {
                switch (_JobStatus)
                {
                    case JobStatus.ParseError:
                        _LongStatus = "error parsing the file";
                        break;
                    case JobStatus.Done:
                        _LongStatus = "imported some rows";
                        break;
                    case JobStatus.DoneError:
                        _LongStatus = "error importing some rows";
                        break;
                    case JobStatus.Loading:
                        _LongStatus = "file is being loaded";
                        break;
                    case JobStatus.Importing:
                        _LongStatus = "file is being imported";
                        break;
                    case JobStatus.Loaded:
                        _LongStatus = "file has been loaded";
                        break;
                    case JobStatus.AllDone:
                        _LongStatus = "no more rows to import";
                        break;
                    case JobStatus.AllDoneError:
                        _LongStatus = "error in importing file";
                        break;
                    case JobStatus.Reload:
                        _LongStatus = "click on the file to reload";
                        break;
                    default:
                        _LongStatus = "click on the order to load";
                        break;
                }
                return _LongStatus;
            }
        }

        private string _ImportStatus;
        public string ImportStatus
        {
            get
            {
                _ImportStatus = String.Format("{{\"ImportStatus\":\"{0:000}\"}}", (int) _JobStatus);
                return _ImportStatus;
            }
        }

        internal static int GetJobStatus(string ImportStatus)
        {
            return 0; //TODO
        }

        private SalesItemViewModel _SelectedItem;
        public SalesItemViewModel SelectedItem
        {
            get
            {
                return _SelectedItem;
            }
            set
            {
                if (_SelectedItem != value)
                {
                    _SelectedItem = value;
                    OnPropertyChanged("SelectedItem");
                }
            }
        }

        public SolidColorBrush ErrorColor
        {
            get 
            {
                var clr = Brushes.Black;
                switch (_JobStatus)
                {
                    case JobStatus.ParseError:
                    case JobStatus.AllDoneError:
                    case JobStatus.DoneError:
                        clr = Brushes.Red;
                        break;
                    case JobStatus.Loading:
                    case JobStatus.Importing:
                    case JobStatus.Done:
                    case JobStatus.AllDone:
                        clr = Brushes.Black;
                        break;
                    case JobStatus.Loaded:
                        clr = IsShowAllEnabled ? Brushes.Gray : Brushes.Black;
                        break;
                    default:
                        clr = Brushes.Black;
                        break;
                }
                return clr;
            }                        
        }

        private ObservableCollection<SalesItemViewModel> _Items;
        public ObservableCollection<SalesItemViewModel> Items 
        {
            get { return _Items; }
            set
            {
                if (_Items != value)
                {
                    _Items = value;
                    OnPropertyChanged("Items");
                }
            }
        }

        
        internal bool _ShowAll;
        private int _MaxPreview;
        private OrderManagementViewModel _Model;

            // construct the file
        internal SalesOrderViewModel(OrderManagementViewModel model, int fileId, string name, string title, string description)
        {
            _Model = model;
            _Proxy = _Model._Proxy;
            _MaxPreview = _Model.MaxPreview;
            _Description = string.IsNullOrWhiteSpace(description) ? string.Empty : description;
            if (!string.IsNullOrEmpty(_Description))
            {
                if (_Description.Contains("{70}"))
                {
                    _PreviousStatus = JobStatus.AllDone;
                    _Description = _Description.Replace("{70}", "");
                }
                if (_Description.Contains("{75}"))
                {
                    _PreviousStatus = JobStatus.AllDoneError;
                    _Description = _Description.Replace("{75}", "");
                }
            }
            _Name = name;
            _Title = title;
            _TitleUpper = Title.ToUpperInvariant();
            _FileId = fileId;
            HeaderId = fileId;
            _ProgressPercent = 0;
            _IsShowAllEnabled = false;
            _ShowAll = false;
            _Items = new ObservableCollection<SalesItemViewModel>();
            _LoadedAll = false;
            _JobStatus = JobStatus.Load;
            _IsNewItemEnabled = false;
            _Status = String.Empty + _JobStatus;
            _LongStatus = _Status;
            _Stopwatch = new Stopwatch();
            _ProgressTimer = new Stopwatch();
            _OrderStatus = "";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(String propertyName)
        {
            if (null != PropertyChanged)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private int _NumProcessed = 0, _NumError = 0, _PerTen = 2;
            
            // show incremental progress
        private void IncrementProgress(bool success)
        {
            ++_NumProcessed;
            if (!success)
                ++_NumError;
            int pp = 0;
            if (_NumProcessed == CountSelected)
            {
                pp = 100;
            }
            RefreshProgress(pp);
        }

            // refresh the progress bar
        private void RefreshProgress(int pp)
        {
            if ( (_NumProcessed % _PerTen != 0 && _ProgressTimer.ElapsedMilliseconds < 2000) && (100 != pp)) return;
            ProgressPercent = (100 == pp) ? 100 : (int)Math.Ceiling(((double)_NumProcessed / (double)CountSelected) * 100.0d);
            CountProcessed = _NumProcessed;
            CountError = _NumError;
            if (100 == pp)
            {
                _JobStatus = (0 == _NumError) ? JobStatus.Done : JobStatus.DoneError;
            }
            _ProgressTimer.Restart();
            RefreshStatus();
        }

            // refresh the status
        internal void RefreshStatus()
        {
            Status = String.Empty + _JobStatus;
            switch (_JobStatus)
            {
                case JobStatus.Loading:
                    IsRemoveEnabled = false;
                    IsShowAllEnabled = false;
                    IsLoading = true;
                    break;
                case JobStatus.ParseError:
                    IsRemoveEnabled = false;
                    IsShowAllEnabled = false;
                    IsLoading = false;
                    break;
                case JobStatus.Loaded:
                    IsRemoveEnabled = true;
                    IsLoading = false;
                    break;
                case JobStatus.Importing:
                    IsRemoveEnabled = false;
                    IsShowAllEnabled = false;
                    IsLoading = false;
                    break;
                case JobStatus.Done:
                    IsRemoveEnabled = true;
                    IsLoading = false;
                    break;
                case JobStatus.DoneError:
                    IsRemoveEnabled = true;
                    IsLoading = false;
                    break;
                case JobStatus.AllDone:
                    IsRemoveEnabled = true;
                    IsLoading = false;                    
                    break;
                case JobStatus.AllDoneError:
                    IsRemoveEnabled = true;
                    IsLoading = false;
                    break;
            }
            OnPropertyChanged("ErrorColor");
            OnPropertyChanged("PreviewName");
        }

        internal void Clean()
        {
            Items.Clear();
            _LoadedAll = false;
            IsRemoveEnabled = false;
            IsShowAllEnabled = false;
            Name = string.Empty;
            _Title = string.Empty;
            ProgressPercent = 0;
            Status = String.Empty;
        }

        internal void SelectAll()
        {
            foreach (SalesItemViewModel item in Items)
                if (SalesItemViewModel.ImportStatus.ENTERED == item.Status)
                    item.Booked = true;
        }

        internal Proxy _Proxy;

        private Stopwatch _Stopwatch;
        private Stopwatch _ProgressTimer;
        Collection<SalesItemViewModel> _FinishedImporting = new Collection<SalesItemViewModel>();

            // start the import
        internal void Import()
        {
            ImportAsync(() =>
            {
                int selected = 0;
                foreach (SalesItemViewModel item in Items)
                {
                    if (item.Booked)
                    {
                        ++selected;
                        _FinishedImporting.Add(item);
                    }
                }
                if (0 == selected) return;
                _NumProcessed = 0;
                _NumError = 0;
                double pt = (double)selected / 9.0d;
                pt = Math.Max(pt, 10.0d);
                _PerTen = (int)Math.Ceiling(pt);
                _JobStatus = JobStatus.Importing;
                _Stopwatch.Restart();
                CountSelected = selected;
                string msg = string.Format("{2} Order Management is processing {0} records from {1}",
                        CountSelected, HeaderId, _Model.ServerType);
                _Model.IsRunning = true;
                _ProgressTimer.Start();
                CountProcessed = 0;
                CountError = 0;
                RefreshProgress(0);
                foreach (SalesItemViewModel item in _FinishedImporting)
                {
                    Thread.Yield();
                    IncrementProgress(item.Import());
                }
                _Model.IsRunning = false;
                _ProgressTimer.Reset();
            });
        }

        private void ImportAsync(Action action)
        {
            var wrk = new BackgroundWorker();
            DoWorkEventHandler hdlr = null;
            hdlr = (sender, e) =>
            {
                action();
                wrk.DoWork -= hdlr;
            };
            wrk.DoWork += hdlr;
            wrk.RunWorkerCompleted += FinishedImporting;
            wrk.RunWorkerAsync();
        }

        private void FinishedImporting(object sender, RunWorkerCompletedEventArgs e)
        {
            _Stopwatch.Stop();
            if (0 == CountSelected) return;
            _FinishedImporting.Clear();
            CumulativeCountError += CountError;
            CumulativeCountProcessed += CountProcessed;
            string msg = string.Format("completed importing {0} records from {1} in {2:0.00} minutes. {3} errors",
                    CountProcessed, HeaderId, (double)_Stopwatch.ElapsedMilliseconds / 60000d, CountError);
            if (Items.Count == CumulativeCountProcessed)
            {
                msg = string.Format("finished importing {0} records from {1} in {2:0.00} minutes. {3} errors. Nothing left to import.",
                    CountProcessed, HeaderId, (double)_Stopwatch.ElapsedMilliseconds / 60000d, CountError);
                _JobStatus = 0 == CumulativeCountError ? JobStatus.AllDone : JobStatus.AllDoneError;
                _Model._Proxy.MarkFileImported(this);
            }
            RefreshStatus();
            CountSelected = 0;
            if (0 == CountError)
                _Proxy.DebugLog(msg, null);
            else
                _Proxy.ErrorLog(msg, null);
        }

        Collection<SalesItemViewModel> _FinishedLoading = new Collection<SalesItemViewModel>();

            // start the load
        internal void Load()
        {
            if (!_LoadedAll 
                && !(JobStatus.ParseError == _JobStatus)
                && !(JobStatus.Loading == _JobStatus)
                && !(JobStatus.Importing == _JobStatus)
                && !( (JobStatus.Loaded == _JobStatus) && false == _ShowAll))
            {
                try
                {
                    LoadAll();
                }
                catch (Exception e)
                {
                    _JobStatus = JobStatus.ParseError;
                    _Proxy.ErrorLog(string.Format("cannot load order {0} id {1}",
                    HeaderId, FileId), e.Message);
                    Items.Clear();
                    return;
                }
            }
        }

        Collection<SalesItemViewModel> _LoadedList;
        internal bool _LoadedAll;

        private string _Description;
        public string Description
        {
            get
            {
                return 0 == HeaderId ? _Description : Convert.ToString(HeaderId);
            }
            set
            {
                if (_Description != value)
                {
                    _Description = value;
                    OnPropertyChanged("Description");
                }
            }
        }

        void LoadAll()
        {
            if (_LoadedAll) return;
            _LoadedList = new Collection<SalesItemViewModel>();            
            _JobStatus = JobStatus.Loading;
            RefreshStatus();
            _Stopwatch.Restart();
            LoadAllAsync(() =>
            {
                try
                {                    
                    if (0 < HeaderId) _LoadedList = _Proxy.LoadParent(this);
                }
                catch (Exception e)
                {
                    _Proxy.ErrorLog(string.Format("cannot load order {0} id {1} in the background",
                     HeaderId, FileId), e.Message);
                    _JobStatus = JobStatus.ParseError;
                    return;
                }
            });            
        }

        private void LoadAllAsync(Action action)
        {
            var wrk = new BackgroundWorker();
            DoWorkEventHandler hdlr = null;
            hdlr = (sender, e) =>
            {
                action();
                wrk.DoWork -= hdlr;
            };
            wrk.DoWork += hdlr;
            wrk.RunWorkerCompleted += FinishedLoadingAll;
            wrk.RunWorkerAsync();
        }

        private void FinishedLoadingAll(object sender, RunWorkerCompletedEventArgs e)
        {
            _Stopwatch.Stop();
            Items.Clear();
            if (!(JobStatus.ParseError == _JobStatus))
            {
                foreach (SalesItemViewModel item in _LoadedList)
                {
                    Items.Add(item);
                }
            }
            _LoadedList.Clear();
            _Stopwatch.Stop();
            string msg = string.Format("finished loading {0} records from {1} in {2:0.00} minutes",
                Items.Count, HeaderId, (double)_Stopwatch.ElapsedMilliseconds / 60000d);
            if (JobStatus.ParseError == _JobStatus)
            {
                _Proxy.ErrorLog(msg, null);
            }
            else
            {
                _Proxy.DebugLog(msg, null);
                _JobStatus = JobStatus.Loaded;
            }            
            RefreshStatus();
        }

            // parse the CSV file
        internal Collection<SalesItemViewModel> Parse(Stream stream)
        {
            var items = new Collection<SalesItemViewModel>();
            return items;
        }

        internal bool RemoveSelectedRows()
        {
            bool result = false;
            Collection<SalesItemViewModel> deleted = new Collection<SalesItemViewModel>();
            foreach (SalesItemViewModel item in Items)
            {
                if (true == item.Booked)
                {
                    deleted.Add(item);
                    result = true;
                }
            }
            foreach (SalesItemViewModel item in deleted)
            {
                Items.Remove(item);
            }
            if (CumulativeCountProcessed == Items.Count)
            {
                _JobStatus = 0 == CumulativeCountError ? JobStatus.AllDone : JobStatus.AllDoneError;
                ProgressPercent = 100;
                RefreshStatus();
                _Model._Proxy.MarkFileImported(this);
            }
            return result;
        }

        internal int Save()
        {
            int result = 0;
            if (0 == HeaderId)
            {
                HeaderId = result = _Model._Proxy.SaveParent(this);
                _JobStatus = 0 == result? JobStatus.DoneError: JobStatus.Done;
                RefreshStatus();
            }
            return result;
        }
    }
}
