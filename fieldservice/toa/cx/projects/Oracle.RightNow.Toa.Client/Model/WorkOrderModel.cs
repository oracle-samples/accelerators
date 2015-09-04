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
 *  date: Thu Sep  3 23:14:01 PDT 2015

 *  revision: rnw-15-11-fixes-release-03
*  SHA1: $Id: cf5dc2a8507269d2c3bf1331c9750242d37727e1 $
* *********************************************************************************************
*  File: WorkOrderModel.cs
* ****************************************************************************************** */

using System;
using System.Windows.Forms;
using System.Collections.Generic;
using Oracle.RightNow.Toa.Client.Common;
using Oracle.RightNow.Toa.Client.InboundProxyService;

namespace Oracle.RightNow.Toa.Client.Model
{
    /// <summary>
    /// Model class for Activity
    /// </summary>
    public class WorkOrderModel : ToaModel
    {
        /// <summary>
        /// These three fields are for creating the command object for this workorder
        /// </summary>
        private string _assignedDate;
        private string _resourceId;
        private ActivityCommandType _commandType;

        private string _fallbackResourceId;
        private string _positionInRoute;
        private string _address;
        private string _apptNumber;
        private string _cell;
        private string _city;
        private string _coordinateX;
        private string _coordinateY;
        private string _customerNumber;
        private string _email;
        private string _duration;
        private string _language;
        private string _name;
        private int? _points;
        private string _phone;
        private int? _reminderTime;
        private string _serviceWindowStart;
        private string _serviceWindowEnd;
        private DateTime? _slaWindowStart;
        private DateTime? _slaWindowEnd;
        private string _state;
        private string _teamId;
        private string _timeSlot;
        private string _timeZone;
        private string _workType;
        private string _workTypeLabel;
        private string _zip;
        private string _cancelReason;
        private List<InventoryModel> _inventories;
        private List<RequiredInventoryModel> _requiredInventories;
        private List<ProviderPreferenceModel> _providerPreferences;
        private ActivityStatus _status;
        private ActionIfCompleted _actionIfCompleted;

        //Response Field
        private int? _aid;

        public WorkOrderModel()
        {
            // _assignedDate = DateTime.Now.ToString("yyyy-MM-dd");
            // _resourceId = "";
            // _fallbackResourceId = "";
            // _positionInRoute = "";
            // _address = "";
            // _apptNumber = "";
            // _cell = "";
            // _city = "";
            // _customerNumber = "";
            // _email = "";
            // _duration = "";
            // _language = "";
            // _name = "";
            // _phone = "";
            // _serviceWindowStart = "";
            // _serviceWindowEnd = "";
            // _slaWindowStart = DateTime.Now;
            // _slaWindowEnd = DateTime.Now;
            // _state = "";
            // _teamId = "";
            // _timeSlot = false;
            // _timeZone = "";
            // _workType = "";
            // _workTypeLabel = "";
            // _zip = "";
            // _inventories = new List<InventoryModel>();
            // _requiredInventories = new List<RequiredInventoryModel>();
            //_providerPreferences = new List<ProviderPreferenceModel>();
            _status = ActivityStatus.Pending;
            //_commandType = ActivityCommandType.Update;
            //_actionIfCompleted = ActionIfCompleted.CreateIfAssignOrReschedule;
        }

        /// <summary>
        /// Required Inventories 
        /// </summary>
        public List<RequiredInventoryModel> RequiredInventories
        {
            get { return _requiredInventories; }
            set { _requiredInventories = value; }
        }

        /// <summary>
        /// List of Provder preferences
        /// </summary>
        public List<ProviderPreferenceModel> ProviderPreferences
        {
            get { return _providerPreferences; }
            set { _providerPreferences = value; }
        }

        /// <summary>
        /// Inventories for Activity
        /// </summary>
        public List<InventoryModel> ActivityInventories
        {
            get { return _inventories; }
            set { _inventories = value; }
        }

        /// <summary>
        /// Cancel Reason
        /// </summary>
        public string CancelReason
        {
            get { return _cancelReason; }
            set { _cancelReason = value; }
        }

        /// <summary>
        /// Zip Code / Pin Code
        /// </summary>
        public string ZipCode
        {
            get { return _zip; }
            set { _zip = value; }
        }

        /// <summary>
        /// SLA Window End (YYYY-MM-DD HH:MM)
        /// </summary>
        public DateTime? SlaWindowEnd
        {
            get { return _slaWindowEnd; }
            set { _slaWindowEnd = value; }
        }

        /// <summary>
        /// SLA Window Start (YYYY-MM-DD HH:MM)
        /// </summary>
        public DateTime? SlaWindowStart
        {
            get { return _slaWindowStart; }
            set { _slaWindowStart = value; }
        }

        /// <summary>
        /// Work Type
        /// </summary>
        public string WorkType
        {
            get { return _workType; }
            set { _workType = value; }
        }

        /// <summary>
        /// Work Label
        /// </summary>
        public string WorkLabel
        {
            get { return _workTypeLabel; }
            set { _workTypeLabel = value; }
        }

        /// <summary>
        /// Time Zone
        /// </summary>
        public string TimeZone
        {
            get { return _timeZone; }
            set { _timeZone = value; }
        }

        /// <summary>
        /// Time Slot 
        /// </summary>
        public String TimeSlot
        {
            get { return _timeSlot; }
            set { _timeSlot = value; }
        }

        /// <summary>
        /// Team Id
        /// </summary>
        public string TeamId
        {
            get { return _teamId; }
            set { _teamId = value; }
        }

        /// <summary>
        /// State
        /// </summary>
        public string State
        {
            get { return _state; }
            set { _state = value; }
        }

        /// <summary>
        /// Service Window End (HH:MM)
        /// </summary>
        public string ServiceWindowEnd
        {
            get { return _serviceWindowEnd; }
            set { _serviceWindowEnd = value; }
        }

        /// <summary>
        /// Service Window start (HH:MM)
        /// </summary>
        public string ServiceWindowStart
        {
            get { return _serviceWindowStart; }
            set { _serviceWindowStart = value; }
        }

        /// <summary>
        /// Reminder Time
        /// </summary>
        public int? ReminderTime
        {
            get { return _reminderTime; }
            set { _reminderTime = value; }
        }

        /// <summary>
        /// Phone Number 
        /// </summary>
        public string PhoneNumber
        {
            get { return _phone; }
            set { _phone = value; }
        }

        /// <summary>
        /// Points
        /// </summary>
        public int? Points
        {
            get { return _points; }
            set { _points = value; }
        }

        /// <summary>
        /// Name - Customer Name 
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Language 
        /// </summary>
        public string Language
        {
            get { return _language; }
            set { _language = value; }
        }

        /// <summary>
        /// Duration - 'length' of activity in minutes 
        /// </summary>
        public string Duration
        {
            get { return _duration; }
            set { _duration = value; }
        }

        /// <summary>
        /// Email Address 
        /// </summary>
        public string EmailAddress
        {
            get { return _email; }
            set { _email = value; }
        }

        /// <summary>
        /// Customer Number 
        /// </summary>
        public string CustomerNumber
        {
            get { return _customerNumber; }
            set { _customerNumber = value; }
        }

        /// <summary>
        /// Longitude of customer location 
        /// </summary>
        public string CoordinateX
        {
            get { return _coordinateX; }
            set { _coordinateX = value; }
        }

        /// <summary>
        /// Latitude of customer location 
        /// </summary>
        public string CoordinateY
        {
            get { return _coordinateY; }
            set { _coordinateY = value; }
        }

        /// <summary>
        /// City (Todo: should be max 40 characters long.)
        /// </summary>
        public string City
        {
            get { return _city; }
            set { _city = value; }
        }

        /// <summary>
        /// Mobile Number (Todo: should be valid mobile number)
        /// </summary>
        public string MobileNumber
        {
            get { return _cell; }
            set { _cell = value; }
        }

        /// <summary>
        /// Appointment Number (Todo: Max character are 40 only)
        /// </summary>
        public string AppointmentNumber
        {
            get { return _apptNumber; }
            set { _apptNumber = value; }
        }

        /// <summary>
        /// Address (Todo: Max character are 100 only)
        /// </summary>
        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }

        /// <summary>
        /// Date to which activity is to be assigned in YYYY-MM-DD formate only 
        /// </summary>
        public string AssignedDate
        {
            get { return _assignedDate; }
            set { _assignedDate = value; }
        }

        /// <summary>
        /// External ID of the resource to which the activity is to be assigned
        /// </summary>
        public string ExternalId
        {
            get { return _resourceId; }
            set { _resourceId = value; }
        }

        /// <summary>
        /// External ID of the resource to which the activity is to be assigned
        /// </summary>
        public string FallbackExternalId
        {
            get { return _fallbackResourceId; }
            set { _fallbackResourceId = value; }
        }

        /// <summary>
        /// ID of the activity followed by the activity to be created
        /// </summary>
        public string PositionInRoute
        {
            get { return _positionInRoute; }
            set { _positionInRoute = value; }
        }

        public ActivityStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public ActivityCommandType CommandType
        {
            get { return _commandType; }
            set { _commandType = value; }
        }

        public String Cell { get { return _cell; } set { _cell = value; } }

        public ActionIfCompleted SetActionIfCompleted
        {
            get { return _actionIfCompleted; }
            set { _actionIfCompleted = value; }

        }

        /// <summary>
        /// 
        /// </summary>
        public int? TOA_AID
        {
            get { return _aid; }
            set { _aid = value; }
        }

        ///
        /// This method is only used within toa client to create AppointmentElement object
        /// 
        internal AppointmentElement GetActivityElement()
        {
            var activity = new AppointmentElement();
            //key fields 
            activity.customer_number = _customerNumber;
            activity.appt_number = _apptNumber;
            //mandatory fields
            activity.worktype = _workType;
            //Other fields
            if (null != _name)
                activity.name = _name;

            activity.action_if_completed = ToaStringsUtil.GetString(_actionIfCompleted);

            if (null != _address)
                activity.address = _address;

            if (null != _cell)
                activity.cell = _cell;

            if (null != _city)
                activity.city = _city;

            if (null != _coordinateX)
                activity.coordx = _coordinateX;

            if (null != _coordinateY)
                activity.coordy = _coordinateY;

            if (null != _email)
                activity.email = _email;

            if (null != _duration)
                activity.duration = _duration;

            if (null != _language)
                activity.language = _language;

            if (null != _points)
                activity.points = _points.ToString();

            if (null != _phone)
                activity.phone = _phone;

            if (null != _reminderTime)
                activity.reminder_time = _reminderTime.ToString();

            if (null != _serviceWindowStart)
                activity.service_window_start = _serviceWindowStart;

            if (null != _serviceWindowEnd)
                activity.service_window_end = _serviceWindowEnd;

            //if(null != _slaWindowStart)
            //    activity.sla_window_start = _slaWindowStart.ToString("yyyy'-'MM'-'dd HH':'mm");

            if (null != _slaWindowEnd)
            {
                activity.sla_window_end = ((DateTime)_slaWindowEnd).ToString("yyyy'-'MM'-'dd HH':'mm");

                //Below 2 null checks and fix is required as ETA is not behaving correctly.
                //If it has sla_window_end parameter then it should override the date and timeslot as mandatory fields in Inbound Request.
                //But it is not doing so, hence we are explictly required to send TimeSlot and Date.
                //If we do not provide them and if they are empty then activity creation fails in ETA.
                if (null == TimeSlot)
                {
                    TimeSlot = ActivityProperty.ALL_DAY_TIME_SLOT_LABEL;
                }
                if (null == AssignedDate)
                {
                    AssignedDate = "";
                }
            }


            if (null != _state)
                activity.state = _state;

            if (null != _teamId)
                activity.team_id = _teamId;

            if (null != _timeSlot)
                activity.time_slot = _timeSlot;

            if (null != _timeZone)
                activity.time_zone = _timeZone;
            //activity.worktype = _workType;
            //Below code is for using worktype label. We plan to use only WorkType and not the label.
            //if (null != _workTypeLabel)
            //    activity.worktype_label = _workTypeLabel;

            if (null != _zip)
                activity.zip = _zip;


            // Activity Properties
            if (null != Properties)
            {
                var noOfInventories = Properties.Count;                
                var propertyElements = new PropertyElement[noOfInventories];
                foreach (var property in Properties)
                {
                    var propertyElement = new PropertyElement();
                    propertyElement.label = property.Key;
                    propertyElement.value = property.Value;
                    propertyElements[--noOfInventories] = propertyElement;                    
                }
                activity.properties = propertyElements;
            }


            // Activity Inventories
            if (null != _inventories)
            {
                var noOfInventories = _inventories.Count;
                var inventoryElements = new InventoryElement[noOfInventories];
                foreach (var inventoryModel in _inventories)
                {
                    var inventoryElement = inventoryModel.GetInventoryElement();
                    inventoryElements[--noOfInventories] = inventoryElement;
                }
                activity.inventories = inventoryElements;
            }

            // Preference Provideres
            if (null != _providerPreferences)
            {
                var noOfProviders = _providerPreferences.Count;
                var providers = new ProviderPreferenceElement[noOfProviders];
                foreach (var providerModel in _providerPreferences)
                {
                    var ProviderPreferenceElement = providerModel.GetProviderPreferenceElement();
                    providers[--noOfProviders] = ProviderPreferenceElement;
                }
                activity.provider_preferences = providers;
            }

            // Required Inventories
            if (null != _requiredInventories && _requiredInventories.Count > 0)
            {
                var noOfRequiedInventories = _requiredInventories.Count;                
                var requiredInventoryElements = new RequiredInventoryElement[noOfRequiedInventories];
                foreach (var requiredInventoryModel in _requiredInventories)
                {
                    var requiredInventory = requiredInventoryModel.GetRequiredInventoryElement();
                    requiredInventoryElements[--noOfRequiedInventories] = requiredInventory;
                }
                activity.required_inventories = requiredInventoryElements;
            }
            if (null != UserData && UserData.Trim().Length > 0)
            {
                activity.userdata = UserData;
            }

            return activity;
        }

    }
}
