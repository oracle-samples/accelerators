/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + EBS Enhancement
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  EBS release: 12.1.3
 *  reference: 150505-000099, 150420-000127
 *  date: Thu Nov 12 00:52:42 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: 9e05103dbbde70495896a084b49d7533a292235f $
 * *********************************************************************************************
 *  File: Proxy.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Accelerator.EBS.SharedServices;

namespace Accelerator.EBS.BulkImportAddin
{
    public class Proxy
    {
        internal BulkImportAddin _Addin;
        internal BulkImportViewModel _Model;

        internal Proxy()
        {
        }

        public Proxy(BulkImportAddin addin)
        {
            _Addin = addin;
        }

            // get the import files
        internal Collection<ImportFile> GetImportFileList()
        {
            Collection<ImportFile> result = new Collection<ImportFile>();
            if (_Model.InDesignMode)
            {
                for (int i = 1; i < 50; i++)
                {
                    string name = string.Format("Repair_Orders_{0:000}.csv", i);
                    ImportFile file;
                    file = new ImportFile(_Model, i, name, name, name);
                    switch (i)
                    {
                        case 1:
                            file._JobStatus = ImportFile.JobStatus.Load;
                            break;
                        case 2:
                            file._JobStatus = ImportFile.JobStatus.Loaded;
                            AddDummyRecords(file);
                            break;
                        case 3:
                            file._JobStatus = ImportFile.JobStatus.ParseError;
                            break;
                        case 4:
                            file._JobStatus = ImportFile.JobStatus.Loading;
                            break;
                        case 5:
                            file._JobStatus = ImportFile.JobStatus.Importing;
                            file.ProgressPercent = 50;
                            break;
                        case 6:
                            file._JobStatus = ImportFile.JobStatus.Done;
                            file.ProgressPercent = 100;
                            break;
                        case 7:
                            file._JobStatus = ImportFile.JobStatus.DoneError;
                            file.ProgressPercent = 100;
                            break;
                        case 8:
                            file._JobStatus = ImportFile.JobStatus.AllDone;
                            file.ProgressPercent = 100;
                            break;
                        case 9:
                            file._JobStatus = ImportFile.JobStatus.AllDoneError;
                            file.ProgressPercent = 100;
                            break;
                        default:
                            break;
                    }
                    file.RefreshStatus();
                    result.Add(file);
                }
            }
            else if (_Model.InDemo)
            {
                for (int i = 1; i <= 50; i++)
                {
                    string name = string.Format("Repair_Orders_{0:000}.csv", i);
                    string description = name;
                    if (i % 5 == 0) description += " {70}";
                    if (i % 7 == 0) description += " {75}";
                    var file = new ImportFile(_Model, i, name, name, description);
                    result.Add(file);
                }
            }
            else if (null != _Addin && null != _Addin._Incident)
            {
                foreach (var f in _Addin._Incident.FAttach2)
                {
                    if (isValid(f))
                    {
                        string name = String.IsNullOrWhiteSpace(f.Name) ? f.UserFName : f.Name;
                        var impf = new ImportFile(_Model, f.ID, f.UserFName,
                            name,  
                            f.Descr );
                        result.Add(impf);
                    }
                }
            }
            return result;
        }

        private bool isValid(RightNow.AddIns.AddInViews.IFAttachInc2 f)
        {
            if(!validContentType(f.ContentType)) return false;
            if (f.Size >= 2097152) return false; // file should be under 2 mb
            return true;
        }

            // get a dummy file
        internal ImportFile GetDummyFile()
        {
            string name = string.Format("Repair_Orders_{0:000}.csv", 2);
            var file = new ImportFile(this._Model, 2, name, name, name);
            AddDummyRecords(file);
            return file;
        }

            // check for content type
        private bool validContentType(string ct)
        {
            bool result = false;
            switch (ct)
            {
                case "application/vnd.ms-excel":
                    result = true;
                    break;
                default:
                    result = false;
                    break;
            }
            return result;
        }

            // load the file
        internal Collection<RepairOrder> LoadFile(ImportFile importFile)
        {
            var items = new Collection<RepairOrder>();
            if (_Model.InDesignMode || _Model.InDemo)
            {
                items = GetDummyRecords(importFile);
                if (_Model.InDemo && importFile.FileId % 7 == 0)
                {
                    importFile._JobStatus = ImportFile.JobStatus.ParseError;
                    items.Clear();
                }
            }
            else if (!_Model.InDesignMode && null != _Addin)
            {
                items = importFile.Parse(new MemoryStream(_Addin._rnSrv.getFileData(_Model.RntIncidentId, importFile.FileId)));
            }
            return items;
        }

            // get dummy records
        private Collection<RepairOrder> GetDummyRecords(ImportFile importFile)
        {
            int j = _Model.MaxPreview == 0 ? 50 : _Model.MaxPreview;
            var items = new Collection<RepairOrder>();
            for (int i = 0; i < j; i++)
            {
                Thread.Sleep(100);
                var item = new RepairOrder(
                        importFile,
                        60932,
                        string.Format("Lorem ipsum dolor sit amet {0:000}", i + 1),
                        67661,
                        "Y",
                        5,
                        (i+1),
                        "Ea",
                        "USD",
                        100003628
                    );
                item.SerialNumber = "EM1234567";
                items.Add(item);
            }
            importFile._LoadedAll = true;
            importFile.RefreshStatus();
            return items;
        }

        private void AddDummyRecords(ImportFile importFile)
        {
            foreach (var item in GetDummyRecords(importFile))
            {
                importFile.RepairOrders.Add(item);
            }
        }

            // import the repair order
        internal bool Import(RepairOrder repairOrder)
        {
            if (_Model.InDemo)
            {
                Thread.Sleep(1000);
                if (repairOrder.Quantity % 10 == 0 && repairOrder._File.FileId % 10 == 0)
                    return false;
                return true;
            }
            var ro = new Accelerator.EBS.SharedServices.RepairOrder();
            ro.Quantity = repairOrder.Quantity;
            ro.UnitOfMeasure = repairOrder.UnitOfMeasure;
            ro.ApprovalRequired = repairOrder.ApprovalRequiredFlag;
            ro.Currency = repairOrder.Currency;
            ro.ResourceID = _Model.EbsOwnerId;
            ro.ProblemDescription = repairOrder.ProblemDescription;
            ro.ServiceRequestID = _Model.EbsSrId;
            ro.SerialNumber = repairOrder.SerialNumber;
            if (ro.ValidateSerialNumber(_Model.EbsContactOrgId, _Model.RntIncidentId, _Model.RntContactId))
            {
                ro.Create(_Model.RntIncidentId, _Model.RntContactId);
            }
            else
            {
                NoticeLog(string.Format("EBS Bulk Import cannot create a repair order because the serial number {0} is not valid", repairOrder.SerialNumber), null);
                return false;
            }

            if (String.IsNullOrWhiteSpace(ro.RepairNumber))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

            // wrappers for logging
        private void Log(ConfigurationSetting.LogLevelEnum level, string msg, string note, int _RntIncidentId, int _RntContactId)
        {
            string sub = msg + String.Format(" for iid {0}, cid {1}", _RntIncidentId, _RntContactId);
            var log = ConfigurationSetting.logWrap;
            switch(level)
            {
                case ConfigurationSetting.LogLevelEnum.Error:
                    log.ErrorLog(_RntIncidentId, _RntContactId, sub, note);
                    break;
                case ConfigurationSetting.LogLevelEnum.Notice:
                    log.NoticeLog(_RntIncidentId, _RntContactId, sub, note);
                    break;
                default:
                    log.DebugLog(_RntIncidentId, _RntContactId, sub, note);
                    break;
            }
        }

        internal void DebugLog(string msg, string note)
        {
            Log(ConfigurationSetting.LogLevelEnum.Debug, msg, note, _Model.RntIncidentId, _Model.RntContactId);
        }

        internal void ErrorLog(string msg, string note)
        {
            Log(ConfigurationSetting.LogLevelEnum.Error, msg, note, _Model.RntIncidentId, _Model.RntContactId);
        }

        internal void NoticeLog(string msg, string note)
        {
            Log(ConfigurationSetting.LogLevelEnum.Notice, msg, note, _Model.RntIncidentId, _Model.RntContactId);
        }

        internal void MarkFileImported(ImportFile file)
        {
            if (!_Model.InDemo)
            {
                _Addin.MarkFileImported(file);
            }
        }

        internal void _InitializedControl(BulkImportViewModel bulkImportViewModel)
        {
            _Model = bulkImportViewModel;
        }
    }
}
