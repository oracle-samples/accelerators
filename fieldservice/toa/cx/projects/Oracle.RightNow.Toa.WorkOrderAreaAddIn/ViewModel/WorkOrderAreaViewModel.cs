/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + OFSC Reference Integration
 *  link: http://www-content.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.2 (Feb 2015)
 *  OFSC release: 15.2 (Feb 2015)
 *  reference: 150622-000130
 *  date: Mon Aug 24 09:01:22 PDT 2015

 *  revision: rnw-15-11-fixes-release-01
 *  SHA1: $Id: 20ed488d4e0d98b38dbc76f4eafd5a78bf664d50 $
 * *********************************************************************************************
 *  File: WorkOrderAreaViewModel.cs
 * ****************************************************************************************** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Oracle.RightNow.Toa.Client.Services;
using Oracle.RightNow.Toa.Client.Model;
using System.Windows;
using RightNow.AddIns.AddInViews;
using System.Windows.Controls;
using Oracle.RightNow.Toa.Client.Logs;
using Oracle.RightNow.Toa.Client.Exceptions;

namespace Oracle.RightNow.Toa.WorkOrderAreaAddIn
{
    class WorkOrderAreaViewModel : INotifyPropertyChanging, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Administrative Properties

        /// <summary>
        /// Whether the view model should ignore property-change events.
        /// </summary>
        public virtual bool IgnorePropertyChangeEvents { get; set; }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the changed property.</param>
        internal virtual void RaisePropertyChangedEvent(string propertyName)
        {
            // Exit if changes ignored
            if (IgnorePropertyChangeEvents) return;

            // Exit if no subscribers
            if (PropertyChanged == null) return;

            // Raise event
            var e = new PropertyChangedEventArgs(propertyName);
            PropertyChanged(this, e);
        }

        /// <summary>
        /// Raises the PropertyChanging event.
        /// </summary>
        /// <param name="propertyName">The name of the changing property.</param>
        internal virtual void RaisePropertyChangingEvent(string propertyName)
        {
            // Exit if changes ignored
            if (IgnorePropertyChangeEvents) return;

            // Exit if no subscribers
            if (PropertyChanging == null) return;

            // Raise event
            var e = new PropertyChangingEventArgs(propertyName);
            PropertyChanging(this, e);
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        #endregion

        #region Fields

        private ObservableCollection<String> workOrderAreaItems;
        private IToaLog _log;

        #endregion

        public WorkOrderAreaViewModel()
        {
            _log = ToaLogService.GetLog();
        }

        public ObservableCollection<String> WorkOrderAreaItems
        {
            get
            {
                return workOrderAreaItems;
            }
            set
            {
                workOrderAreaItems = value;
                RaisePropertyChangedEvent("WorkOrderAreaItems");
            }
        }

        /// <summary>
        /// Fetches the Work Order Areas for a particular zipcode
        /// </summary>
        /// <param name="zipCode">Zipcode</param>
        public void getWorkOrderArea(String zipCode)
        {
            _log.Notice("Inside getWorkOrderArea. ZipCode: " + zipCode);
            CapacityModel capacityModel = new CapacityModel();
            capacityModel.DetermineLocationByWorkZone = true;
            capacityModel.DetermineLocationByWorkZoneSpecified = true;

            ActivityField zipField = new ActivityField();
            zipField.Name = "czip";
            zipField.Value = zipCode;

            DateTime[] dateTimes = new DateTime[] {
                    DateTime.Today,
            };
            capacityModel.QuotaDates = dateTimes;

            List<ActivityField> activityFields = new List<ActivityField>();
            activityFields.Add(zipField);

            capacityModel.ActivityField = activityFields.ToArray();

            ICapacityService service = CapacityService.GetService();
            if (service != null)
            {
                _log.Notice("Got CapacityService Object");
                service.GetWorkOrderArea(capacityModel, WorkOrderAreaCallback);
            }
        }

        /// <summary>
        /// Callback method that will be invoked after the work order area request is successful.
        /// </summary>
        /// <param name="result">TOARequestResult object</param>
        public void WorkOrderAreaCallback(ToaRequestResult result)
        {
            _log.Notice("Inside Callback Method WorkOrderAreaCallback");
            CapacityModel model = (CapacityModel)result.DataModels[0];
            WorkOrderAreaItems = new ObservableCollection<string>(model.Location);
            ToaExceptionManager manager = new ToaExceptionManager();
            manager.ProcessCapacityServiceResult(result);
        }
    }
}