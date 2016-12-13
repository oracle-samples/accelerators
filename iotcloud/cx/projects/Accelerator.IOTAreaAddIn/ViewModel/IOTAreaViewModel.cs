/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: IoT OSvC Bi-directional Integration Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 16.11 (November 2016) 
 *  reference: 151217-000026
 *  date: Tue Dec 13 13:23:38 PST 2016
 
 *  revision: rnw-16-11-fixes-release
 *  SHA1: $Id: 3c9a9f944eed79a7a472a363cf7af0974d0d01c7 $
 * *********************************************************************************************
 *  File: IOTAreaViewModel.cs
 * ****************************************************************************************** */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Accelerator.IOTCloud.Client.Logs;
using Accelerator.IOTCloud.Client.Model;
using Accelerator.IOTCloud.Client.Model.IoT.V2;
using Accelerator.IOTCloud.Client.RightNow;
using Accelerator.IOTCloud.Client.Services;
using RightNow.AddIns.AddInViews;
using RightNow.AddIns.Common;

namespace Accelerator.IOTArea.ViewModel
{
    class IOTAreaViewModel : INotifyPropertyChanging, INotifyPropertyChanged
    {
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;
        private string _detailsText;
        private string _propertyValue;
        private List<String> _deviceProperties;
        private Dictionary<string, Attributes> _attributes;
        private Dictionary<string, string> _propertyNames;
        private string _resultImg;
        private string _deviceId;

        private IRecordContext _recordContext;

        private ILog _logger;

        public IOTAreaViewModel(bool inDesignMode, IRecordContext RecordContext)
        {
            _logger = LogService.GetLog();
            _recordContext = RecordContext;
            if (!inDesignMode)
            {
                _recordContext.DataLoaded += new EventHandler(GetDeviceProperties);
            }            
        }

        #region PropertyChangeEvent Handlers

        // <summary>
        /// Whether the view model should ignore property-change events.
        /// </summary>
        public virtual bool IgnorePropertyChangeEvents { get; set; }

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

        # endregion

        #region Properties
        public string DetailsText
        {
            get
            {
                return _detailsText;
            }
            set
            {
                _detailsText = value;
                RaisePropertyChangedEvent("DetailsText");
            }
        }

        public string PropertyValue
        {
            get
            {
                return _propertyValue;
            }
            set
            {
                _propertyValue = value;
                RaisePropertyChangedEvent("PropertyValue");
            }
        }

        public List<String> DeviceProperties
        {
            get { return _deviceProperties; }
            set
            {
                _deviceProperties = value;
                RaisePropertyChangedEvent("DeviceProperties");
            }
        }

        public string ResultImg
        {
            get { return _resultImg; }
            set
            {
                _resultImg = value;
                RaisePropertyChangedEvent("ResultImg");
            }
        }

        public Dictionary<string, Attributes> Attributes
        {
            get { return _attributes; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Fetches specified number of messages from ICS-IOT.
        /// </summary>
        /// <param name="filterCriteria"></param>
        /// <param name="filterValue"></param>
        public void FetchMessage(string filterCriteria, string filterValue)
        {
            if (_deviceId == null)
            {
                DetailsText = ExceptionMessages.NO_DEVICE;
                return;
            }

            var parameters = new Dictionary<string, string>();
            parameters.Add("device", _deviceId);
            parameters.Add(filterCriteria.ToLower(), filterValue);

            _logger.Debug(string.Format("Fetching messages for device Id : {0}", _deviceId));
            
            MessageService.GetService().GetMessage(parameters, GetDetailsCallback);
        }

        /// <summary>
        /// Sets new value to the specified property
        /// </summary>
        /// <param name="propertyDesc"></param>
        public void SetDeviceProperty(string propertyDesc)
        {
            ResultImg = string.Empty;

            if (_deviceId == null || string.IsNullOrEmpty(propertyDesc) || string.IsNullOrEmpty(PropertyValue))
            {
                ResultImg = "/Accelerator.IOTAreaAddIn;component/Resources/Failure32.png";
                return;
            }

            var postdata = "{ \"value\" : " + PropertyValue + " }";

            _logger.Debug(string.Format("Setting property {0} : {1}", propertyDesc, postdata));
            DeviceMetadataService.GetService().PutProperty(_deviceId, propertyDesc, postdata, SetCallback);
        }

        /// <summary>
        /// Fetches the list of setable properties for the device.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void GetDeviceProperties(object sender, EventArgs e)
        {
            //Fetching device Id from incident
            var incident = _recordContext.GetWorkspaceRecord(WorkspaceRecordType.Incident) as IIncident;
            if (incident != null)
            {
                var incidentId = incident.ID;
                _deviceId = RightNowConnectService.GetService().GetDeviceId(incidentId);
                DeviceMetadataService.GetService().GetDeviceProperties(_deviceId, GetDevicePropertiesCallback);
            }
        }

        #endregion

        #region Callback Methods

        /// <summary>
        /// Callback method that will be invoked after the get details request is successful.
        /// </summary>
        /// <param name="result">TOARequestResult object</param>
        public void GetDetailsCallback(string result)
        {
            DetailsText = result;
        }

        /// <summary>
        /// Callback method that will be invoked after the set temperature request is successful.
        /// </summary>
        /// <param name="result">TOARequestResult object</param>
        public void SetCallback(string result)
        {
            ResultImg = result.Equals("OK") || result.Equals("Accepted") ? "/Accelerator.IOTAreaAddIn;component/Resources/Success32.png" : "/Accelerator.IOTAreaAddIn;component/Resources/Failure32.png";
        }

        /// <summary>
        /// Callback method that will be invoked after the get properties request is successful.
        /// </summary>
        /// <param name="result"></param>
        public void GetDevicePropertiesCallback(Dictionary<string, Attributes> result)
        {
            _attributes = result;
            _propertyNames = new Dictionary<string, string>();

            var lov = new List<string>();

            foreach (var element in result)
            {
                var attribute = element.Value;
                if (attribute.writable)
                {
                    lov.Add(attribute.description);
                    _propertyNames.Add(attribute.description, attribute.name);
                }
            }

            DeviceProperties = lov;
        }

        #endregion

    }
}
