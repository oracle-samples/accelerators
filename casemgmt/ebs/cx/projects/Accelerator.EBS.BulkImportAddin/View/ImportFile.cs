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
 *  SHA1: $Id: f35f780784eea96efdd621e402df4da4b44424d5 $
 * *********************************************************************************************
 *  File: ImportFile.cs
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

namespace Accelerator.EBS.BulkImportAddin
{
    public sealed class ImportFile : INotifyPropertyChanged
    {
        private int _FileId;
        public int FileId { get { return _FileId; } }
        public int StatusCode { get { return (int)_JobStatus; } }

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

        private bool _IsImportEnabled;
        public bool IsImportEnabled
        {
            get 
            {
                if (!_LoadedAll || CumulativeCountProcessed == RepairOrders.Count) return false;
                return _IsImportEnabled; 
            }
            set
            {
                if (_IsImportEnabled != value)
                {
                    _IsImportEnabled = value;
                    OnPropertyChanged("IsImportEnabled");
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
                return _Title;
            }
            set
            {
                if (_Title != value)
                {
                    _Title = value;
                    if (_Title.Length > 40)
                    {
                        _Title = _Title.Substring(0, 30) + "~" + _Title.Substring(_Title.Length - 9, 9);
                    } 
                    OnPropertyChanged("Title");
                }
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
                if (JobStatus.AllDone == PreviousStatus) ps = string.Format(" (previous import: {0})", JobStatus.AllDone.ToString());
                if (JobStatus.AllDoneError == PreviousStatus) ps = string.Format(" (previous import: {0})", JobStatus.AllDoneError.ToString());
                string n = Name;
                if (String.IsNullOrWhiteSpace(n)) return String.Empty;
                string t = Title;
                    //// uncomment only if for preview
                //if (JobStatus.Loading == _JobStatus && !_LoadedAll && !IsShowAllEnabled) t = "Loading " + t;
                //else if (IsShowAllEnabled) t = "Loading " + t;
                    //// uncomment only if the name and title are different
                //return n == Title ? t : String.Format("{0} ({1})", t, n);
                return t + ps;
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
                        _LongStatus = "click on the file to load";
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

        public SolidColorBrush ErrorColor
        {
            get 
            {
                var clr = Brushes.Gray;
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
                        clr = Brushes.Gray;
                        break;
                }
                return clr;
            }                        
        }

        private ObservableCollection<RepairOrder> _RepairOrders;
        public ObservableCollection<RepairOrder> RepairOrders 
        {
            get { return _RepairOrders; }
            set
            {
                if (_RepairOrders != value)
                {
                    _RepairOrders = value;
                    OnPropertyChanged("RepairOrders");
                }
            }
        }
        internal bool _ShowAll;
        private int _MaxPreview;
        private BulkImportViewModel _Model;

            // construct the file
        internal ImportFile(BulkImportViewModel model, int fileId, string name, string title, string description)
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
            Title = title;
            _TitleUpper = Title.ToUpperInvariant();
            _FileId = fileId;
            _ProgressPercent = 0;
            _IsShowAllEnabled = false;
            _ShowAll = false;
            _RepairOrders = new ObservableCollection<RepairOrder>();
            _LoadedAll = false;
            _JobStatus = JobStatus.Load;
            _IsImportEnabled = false;
            _Status = String.Empty + _JobStatus;
            _LongStatus = _Status;
            _Stopwatch = new Stopwatch();
            _ProgressTimer = new Stopwatch();
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
                    IsImportEnabled = false;
                    IsShowAllEnabled = false;
                    IsLoading = true;
                    break;
                case JobStatus.ParseError:
                    IsRemoveEnabled = false;
                    IsImportEnabled = false;
                    IsShowAllEnabled = false;
                    IsLoading = false;
                    break;
                case JobStatus.Loaded:
                    IsRemoveEnabled = true;
                    IsImportEnabled = true;
                    IsLoading = false;
                    break;
                case JobStatus.Importing:
                    IsRemoveEnabled = false;
                    IsImportEnabled = false;
                    IsShowAllEnabled = false;
                    IsLoading = false;
                    break;
                case JobStatus.Done:
                    IsRemoveEnabled = true;
                    IsImportEnabled = true;
                    IsLoading = false;
                    break;
                case JobStatus.DoneError:
                    IsRemoveEnabled = true;
                    IsImportEnabled = true;
                    IsLoading = false;
                    break;
                case JobStatus.AllDone:
                    IsRemoveEnabled = true;
                    IsImportEnabled = false;
                    IsLoading = false;                    
                    break;
                case JobStatus.AllDoneError:
                    IsRemoveEnabled = true;
                    IsImportEnabled = false;
                    IsLoading = false;
                    break;
            }
            OnPropertyChanged("ErrorColor");
            OnPropertyChanged("PreviewName");
        }

        internal void Clean()
        {
            RepairOrders.Clear();
            _LoadedAll = false;
            IsRemoveEnabled = false;
            IsShowAllEnabled = false;
            Name = string.Empty;
            _Title = string.Empty;
            ProgressPercent = 0;
            IsImportEnabled = false;
            Status = String.Empty;
        }

        internal void SelectAll()
        {
            foreach (RepairOrder item in RepairOrders)
                if (RepairOrder.ImportStatus.New == item.Status)
                    item.Selected = true;
        }

        internal Proxy _Proxy;

        private Stopwatch _Stopwatch;
        private Stopwatch _ProgressTimer;
        Collection<RepairOrder> _FinishedImporting = new Collection<RepairOrder>();

            // start the import
        internal void Import()
        {
            ImportAsync(() =>
            {
                int selected = 0;
                foreach (RepairOrder item in RepairOrders)
                {
                    if (item.Selected)
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
                string msg = string.Format("EBS Bulk Import is processing {0} records from {1}",
                        CountSelected, Title);
                _Model.IsRunning = true;
                _ProgressTimer.Start();
                CountProcessed = 0;
                CountError = 0;
                RefreshProgress(0);
                foreach (RepairOrder item in _FinishedImporting)
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
            string msg = string.Format("EBS Bulk Import completed importing {0} records from {1} in {2:0.00} minutes. {3} errors",
                    CountProcessed, Title, (double)_Stopwatch.ElapsedMilliseconds / 60000d, CountError);
            if (RepairOrders.Count == CumulativeCountProcessed)
            {
                msg = string.Format("EBS Bulk Import finished importing {0} records from {1} in {2:0.00} minutes. {3} errors. Nothing left to import.",
                    CountProcessed, Title, (double)_Stopwatch.ElapsedMilliseconds / 60000d, CountError);
                _JobStatus = 0 == CumulativeCountError ? JobStatus.AllDone : JobStatus.AllDoneError;
                _Model._Proxy.MarkFileImported(this);
            }
            _Model.ShowNotificationWindow(msg);
            RefreshStatus();
            CountSelected = 0;
            if (0 == CountError)
                _Proxy.DebugLog(msg, null);
            else
                _Proxy.NoticeLog(msg, null);
        }

        Collection<RepairOrder> _FinishedLoading = new Collection<RepairOrder>();

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
                    _Proxy.NoticeLog(string.Format("EBS Bulk Import cannot load file {0} id {1}",
                    Title, FileId), e.Message);
                    RepairOrders.Clear();
                    return;
                }
            }
        }

        Collection<RepairOrder> _LoadedList;
        internal bool _LoadedAll;

        private string _Description;
        public string Description
        {
            get
            {
                return _Description;
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
            _LoadedList = new Collection<RepairOrder>();            
            _JobStatus = JobStatus.Loading;
            RefreshStatus();
            _Stopwatch.Restart();
            LoadAllAsync(() =>
            {
                try
                {
                    _LoadedList = _Proxy.LoadFile(this);
                }
                catch (Exception e)
                {
                    _Proxy.NoticeLog(string.Format("EBS Bulk Import cannot load file {0} id {1} in the background.",
                    Title, FileId), e.Message);
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
            RepairOrders.Clear();
            if (!(JobStatus.ParseError == _JobStatus))
            {
                foreach (RepairOrder item in _LoadedList)
                {
                    RepairOrders.Add(item);
                }
            }
            _LoadedList.Clear();
            _Stopwatch.Stop();
            string msg = string.Format("EBS Bulk Import finished loading {0} records from {1} in {2:0.00} minutes",
                RepairOrders.Count, Title, (double)_Stopwatch.ElapsedMilliseconds / 60000d);
            if (JobStatus.ParseError == _JobStatus)
            {
                _Proxy.NoticeLog(msg, null);
            }
            else
            {
                _Proxy.DebugLog(msg, null);
                _JobStatus = JobStatus.Loaded;
            }            
            RefreshStatus();
        }

            // parse the CSV file
        internal Collection<RepairOrder> Parse(Stream stream)
        {
            var items = new Collection<RepairOrder>();
            int max = _MaxPreview;
            int ret = max;
            if (_ShowAll | 0 == max)
            {
                max = 0;
            }
            else
            {
                max = _MaxPreview + 1;
            }
            CSV file = new CSV(stream, 
                hasHeaders: true, charDelimiter:',', charQuote:'"', charEscapeQuote:'\\', fileEncoding:Encoding.UTF8, maxRecords:max, name:Title);
            DataTable table = file.ToDataTable();
            if (null == table)
            {
                _Proxy.NoticeLog(string.Format("EBS Bulk Import cannot parse file {0} id {1}. Cannot create data table.", 
                    Title, FileId), null);
                _JobStatus = JobStatus.ParseError;
                stream.Dispose();
                items.Clear();
                return items;
            }
            stream.Dispose();
            DataRowCollection rows = table.Rows;
            int rowCount = rows.Count;
            ret = rowCount;
            if (0 == rowCount || (rowCount <= _MaxPreview) || (0 == max))
            {
                _LoadedAll = true;
                IsShowAllEnabled = false;
            }
            else if (rowCount == (_MaxPreview + 1))
            {
                _LoadedAll = false;
                IsShowAllEnabled = true;
                ret = rowCount - 1;
            }
            else
            {
                _LoadedAll = false;
                IsShowAllEnabled = true;
            }
            int i = 0;
            foreach (DataRow row in rows)
            {
                if (++i > ret)
                {
                    break;
                }
                var item = new RepairOrder(
                        this,
                        this._Model.EbsSrId,
                        Convert.ToString(row.Field<object>("PROBLEM_DESCRIPTION")),
                        0,
                        Convert.ToString(row.Field<object>("APPROVAL_REQUIRED_FLAG")),
                        Convert.ToDecimal(row.Field<object>("REPAIR_TYPE_ID")),
                        Convert.ToDecimal(row.Field<object>("QUANTITY")),
                        Convert.ToString(row.Field<object>("UNIT_OF_MEASURE")),
                        Convert.ToString(row.Field<object>("CURRENCY_CODE")),
                        0
                    );
                item.SerialNumber = Convert.ToString(row.Field<object>("SERIAL_NUMBER"));                
                items.Add(item);
            }
            return items;
        }

        internal bool RemoveSelectedRows()
        {
            bool result = false;
            Collection<RepairOrder> deleted = new Collection<RepairOrder>();
            foreach (RepairOrder item in RepairOrders)
            {
                if (true == item.Selected)
                {
                    deleted.Add(item);
                    result = true;
                }
            }
            foreach (RepairOrder item in deleted)
            {
                RepairOrders.Remove(item);
            }
            if (CumulativeCountProcessed == RepairOrders.Count)
            {
                _JobStatus = 0 == CumulativeCountError ? JobStatus.AllDone : JobStatus.AllDoneError;
                ProgressPercent = 100;
                RefreshStatus();
                _Model._Proxy.MarkFileImported(this);
            }
            return result;
        }
    }
}
