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
 *  date: Thu Nov 12 00:52:43 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: b0322ecfce8aaf8409fadcc1aba979c6cadc5587 $
 * *********************************************************************************************
 *  File: Proxy.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Accelerator.EBS.SharedServices;

namespace Accelerator.EBS.OrderManagementAddin 
{
    sealed public class Proxy
    {
        internal OrderManagementViewModel _Model;
        private WorkspaceAddIn workspaceAddIn;

        internal Proxy()
        {
        }

        public Proxy(WorkspaceAddIn workspaceAddIn)
        {
            this.workspaceAddIn = workspaceAddIn;
        }

        internal Collection<SalesOrderViewModel> GetParentList()
        {
            Collection<SalesOrderViewModel> result = new Collection<SalesOrderViewModel>();
            if (_Model.InDesignMode)
            {
                for (int i = 1; i < 10; i++)
                {
                    string name = string.Format("Sales_Order_{0:000}", i);
                    SalesOrderViewModel order;
                    order = new SalesOrderViewModel(_Model, i, name, name, name);
                    switch (i)
                    {
                        case 1:
                            order._JobStatus = SalesOrderViewModel.JobStatus.Load;
                            break;
                        case 2:
                            order._JobStatus = SalesOrderViewModel.JobStatus.Loaded;
                            AddDummyRecords(order);
                            break;
                        case 3:
                            order._JobStatus = SalesOrderViewModel.JobStatus.ParseError;
                            break;
                        case 4:
                            order._JobStatus = SalesOrderViewModel.JobStatus.Loading;
                            break;
                        case 5:
                            order._JobStatus = SalesOrderViewModel.JobStatus.Importing;
                            order.ProgressPercent = 50;
                            break;
                        case 6:
                            order._JobStatus = SalesOrderViewModel.JobStatus.Done;
                            order.ProgressPercent = 100;
                            break;
                        case 7:
                            order._JobStatus = SalesOrderViewModel.JobStatus.DoneError;
                            order.ProgressPercent = 100;
                            break;
                        case 8:
                            order._JobStatus = SalesOrderViewModel.JobStatus.AllDone;
                            order.ProgressPercent = 100;
                            break;
                        case 9:
                            order._JobStatus = SalesOrderViewModel.JobStatus.AllDoneError;
                            order.ProgressPercent = 100;
                            break;
                        default:
                            break;
                    }
                    order.RefreshStatus();
                    result.Add(order);
                }
            }
            else
            {
                foreach (var order in Order.GetOrdersByIncident(_Model.RntIncidentId, _Model.RntContactId))
                {
                    string name = string.Format("Order {0}", order.HEADER_ID);
                    string description = name;
                    var vm = new SalesOrderViewModel(_Model, (int)order.HEADER_ID, name, name, description);
                    vm.OrderStatus = order.FLOW_STATUS_CODE;
                    vm.DateCreated = order.CREATION_DATE;
                    vm.DateUpdated = order.LAST_UPDATE_DATE;
                    result.Add(vm);
                }
            }
            return result;
        }

        internal Collection<SalesItemViewModel> LoadParent(SalesOrderViewModel orderVM)
        {
            Collection<SalesItemViewModel> items = new Collection<SalesItemViewModel>();

            Order order = Order.GetOrderDetails(orderVM.HeaderId, _Model.RntIncidentId, _Model.RntContactId);
            foreach (var item in order.Items)
            {
                var itemVM = new SalesItemViewModel(
                        orderVM,
                        _Model.EbsSrId,
                        "",
                        (decimal) item.INVENTORY_ITEM_ID,
                        "Y",
                        5,
                        (decimal) item.ORDERED_QUANTITY,
                        "Ea",
                        "USD",
                        _Model.EbsOwnerId
                    );
                itemVM.UnitSellingPrice = item.UNIT_SELLING_PRICE;
                itemVM.ItemName = item.INVENTORY_ITEM;
                itemVM.LineID = (int) item.LINE_ID;
                itemVM.SerialNumber = "EM1234567";
                items.Add(itemVM);
            }
            orderVM._LoadedAll = true;
            orderVM.RefreshStatus();
            return items;
        }

        internal bool Import(SalesItemViewModel repairOrder)
        {
            return true;
        }

        // wrappers for logging
        private void Log(ConfigurationSetting.LogLevelEnum level, string msg, string note)
        {
            var sb = new StringBuilder(string.Format("{0} Order Management ", _Model.ServerType));
            var sub = sb.Append(msg )
                .Append(String.Format(" for iid {0}, cid {1}", _Model.RntIncidentId, _Model.RntContactId))
                .ToString();
            var log = ConfigurationSetting.logWrap;
            switch (level)
            {
                case ConfigurationSetting.LogLevelEnum.Error:
                    log.ErrorLog(_Model.RntIncidentId, _Model.RntContactId, sub, note);
                    break;
                case ConfigurationSetting.LogLevelEnum.Notice:
                    log.NoticeLog(_Model.RntIncidentId, _Model.RntContactId, sub, note);
                    break;
                default:
                    log.DebugLog(_Model.RntIncidentId, _Model.RntContactId, sub, note);
                    break;
            }
        }

        internal void DebugLog(string msg, string note)
        {
            Log(ConfigurationSetting.LogLevelEnum.Debug, msg, note);
        }

        internal void ErrorLog(string msg, string note)
        {
            Log(ConfigurationSetting.LogLevelEnum.Error, msg, note);
        }

        internal void NoticeLog(string msg, string note)
        {
            Log(ConfigurationSetting.LogLevelEnum.Notice, msg, note);
        }

        internal SalesOrderViewModel GetDummyFile()
        {
            string name = string.Format("Sales_Order_{0:000}", 2);
            var file = new SalesOrderViewModel(this._Model, 2, name, name, name);
            AddDummyRecords(file);
            return file;
        }

        private void AddDummyRecords(SalesOrderViewModel importFile)
        {
            foreach (var item in GetDummyRecords(importFile))
            {
                importFile.Items.Add(item);
            }
        }

        private Collection<SalesItemViewModel> GetDummyRecords(SalesOrderViewModel order)
        {
            var items = new Collection<SalesItemViewModel>();
            int n = _Model.MaxPreview == 0 ? 5 : _Model.MaxPreview;
            for (int i = 0; i < n; i++)
            {
                Thread.Sleep(10);
                var item = new SalesItemViewModel(
                        order,
                        60932,
                        string.Format("AS100{0:000}", i + 1),
                        67661,
                        "Y",
                        5,
                        (i + 1),
                        "Ea",
                        "USD",
                        100003628
                    );
                item.SerialNumber = "EM1234567";
                items.Add(item);
            }
            order._LoadedAll = true;
            order.RefreshStatus();
            return items;
        }

        internal Collection<SalesItemViewModel> GetAvailableItems(int limit = 0, SalesOrderViewModel parent = null)
        {
            var items = new Collection<SalesItemViewModel>();
            String query = @"SELECT sp.PartNumber, sp.Name , sp.Schedules.SalesProductScheduleList.Price.Value 
                FROM SalesProduct sp 
                WHERE sp.Folder.Name = 'EBS Products' 
                AND sp.Schedules.SalesProductScheduleList.Price.Currency.Name = 'USD' 
                AND sp.Schedules.SalesProductScheduleList.Price.Value > 0 
                LIMIT 1000";
            String[] rows = null;
            string row = null;
            try
            {
                rows = workspaceAddIn._rnSrv.queryData(query);
            }
            catch (Exception e)
            {
                string logMessage = "Error in getting products from CWSS. Error query: " + query;
                ErrorLog(logMessage, e.Message);                
            }
            try
            {
                foreach (string r in rows)
                {
                    row = r;
                    string[] s = r.Split(',');
                    int id = Convert.ToInt32(s[0]);
                    string name = s[1];
                    decimal usp = Math.Round(Convert.ToDecimal(s[2]), 2);
                    var item = new SalesItemViewModel(
                            parent,
                            _Model.EbsSrId,
                            name,
                            id,
                            "Y",
                            0,
                            1,
                            "Ea",
                            "USD",
                            _Model.EbsOwnerId
                        );
                    item.UnitSellingPrice = usp;
                    item.SerialNumber = "EM1234567";
                    items.Add(item);
                }
            }
            catch (Exception e)
            {
                string logMessage = "Bad data returned by products query. Data: " + row;
                NoticeLog(logMessage, e.Message);
            }
            return items;
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

        internal void MarkFileImported(SalesOrderViewModel file)
        {
            // intentionally left blank
        }

        internal int SaveParent(SalesOrderViewModel view)
        {
            Order order = new Order();
            order.ATTRIBUTE15 = Convert.ToString(_Model.RntIncidentId);
            order.ContactId = _Model.RntContactId;
            order.IncidentId = _Model.RntIncidentId; 
            foreach (var i in view.Items)
            {
                var item = new OrderItem();
                item.INVENTORY_ITEM_ID = i.InventoryItemId;
                item.ORDERED_QUANTITY = i.Quantity;
                item.UNIT_SELLING_PRICE = i.UnitSellingPrice;
                order.Items.Add(item);
            }
            try
            {
                order.Save();
            }
            catch (Exception e)
            {
                NoticeLog("There was a problem while saving the order. " + e.Message, null);
            }
            view.DateCreated = order.CREATION_DATE;
            view.DateUpdated = order.LAST_UPDATE_DATE;
            view.OrderStatus = order.FLOW_STATUS_CODE;
            return null == order.HEADER_ID ? 0 : (int) order.HEADER_ID;
        }

        internal void _InitializedControl(OrderManagementViewModel model)
        {
            _Model = model;
            _Model.ServerType = "EBS";
        }

        internal void MainWindowLoaded()
        {
            // intentionally left blank
        }
    }
}
