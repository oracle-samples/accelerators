/* * *******************************************************************************************
*  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC Out of Office Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 16.5 (May 2016) 
 *  reference: 150916-000080
 *  date: Thu Mar 17 23:37:54 PDT 2016
 
 *  revision: rnw-16-5-fixes-release-1
*  SHA1: $Id: 053bd204e2b9dc940701ffca9a9b29b2dcd83e70 $
* *********************************************************************************************
*  File: OutOfOfficeViewModel.cs
* ****************************************************************************************** */

using System;
using System.Collections.ObjectModel;
using System.Linq;
using Accelerator.OutOfOffice.Client.Common;
using Accelerator.OutOfOffice.Client.Model;
using Accelerator.OutOfOffice.Client.Services;

namespace Accelerator.OutOfOffice
{
    public class OutOfOfficeViewModel
    {
        private bool _outOfOfficeFlag;
        private DateTime? _fromDateTime;
        private DateTime? _toDateTime;
        private string _timezone;
        private string _personalMsgOption;
        private string _personalMsg;
        private string _toTime;
        private string _fromTime;

        private StaffAccount _staffAccount;
        private ObservableCollection<String> _personalMsgOptions;
        private ObservableCollection<String> _timeOptions;
        private IOrderedEnumerable<String> _timezoneCollection;

        public OutOfOfficeViewModel(StaffAccount staffAccount)
        {
            _staffAccount = staffAccount;
            OutOfOfficeFlag = _staffAccount.OooFlag;
            FromDateTime = _staffAccount.OooStart;
            ToDateTime = _staffAccount.OooEnd;
            PersonalMsgOption = _staffAccount.OooMsgOption;
            PersonalMsg = _staffAccount.OooMsg;
            ToTime = String.Format(Common.HourMinuteFormat, _staffAccount.OooEnd);
            FromTime = String.Format(Common.HourMinuteFormat, _staffAccount.OooStart);
            Timezone = _staffAccount.OooTimezone;
        }

        #region Properties
        public ObservableCollection<String> PersonalMsgOptions
        {
            get
            {
                _personalMsgOptions = new ObservableCollection<string>
                {
                    Client.Common.PersonalMsgOptions.StandardMessage,
                    Client.Common.PersonalMsgOptions.AppendToStandardMessage,
                    Client.Common.PersonalMsgOptions.ReplaceStandardMessage
                };
                return _personalMsgOptions;
            }
        }

        public ObservableCollection<String> TimeOptions
        {
            get
            {
                if (_timeOptions != null)
                {
                    return _timeOptions;
                }
                else
                {
                    _timeOptions = new ObservableCollection<string>
                    {
                        "12:00 AM", "12:30 AM", "01:00 AM", "01:30 AM", "02:00 AM", "02:30 AM", 
                        "03:00 AM", "03:30 AM", "04:00 AM", "04:30 AM", "05:00 AM", "05:30 AM",
                        "06:00 AM", "06:30 AM", "07:00 AM", "07:30 AM","08:00 AM", "08:30 AM", 
                        "09:00 AM", "09:30 AM","10:00 AM", "10:30 AM", "11:00 AM", "11:30 AM",
                        "12:00 PM", "12:30 PM", "01:00 PM", "01:30 PM", "02:00 PM", "02:30 PM", 
                        "03:00 PM", "03:30 PM", "04:00 PM", "04:30 PM", "05:00 PM", "05:30 PM",
                        "06:00 PM", "06:30 PM", "07:00 PM", "07:30 PM","08:00 PM", "08:30 PM", 
                        "09:00 PM", "09:30 PM","10:00 PM", "10:30 PM", "11:00 PM", "11:30 PM"
                    };
                    return _timeOptions;
                }
            }
        }

        public IOrderedEnumerable<String> TimezonesCollection
        {
            get
            {
                var timezoneArray = TimezoneService.GetService().TimezoneMap.Keys.ToArray();
                _timezoneCollection = timezoneArray.OrderBy(i => i);
                return _timezoneCollection;
            }
        }

        public string Timezone
        {
            get { return _timezone; }
            set { _timezone = value; }
        }

        public bool OutOfOfficeFlag
        {
            get { return _outOfOfficeFlag; }
            set
            {
                _outOfOfficeFlag = value;
            }
        }

        public DateTime? FromDateTime
        {
            get { return _fromDateTime; }
            set { _fromDateTime = value; }
        }

        public DateTime? ToDateTime
        {
            get { return _toDateTime; }
            set { _toDateTime = value; }
        }

        public string PersonalMsgOption
        {
            get { return _personalMsgOption; }
            set { _personalMsgOption = value; }
        }

        public string PersonalMsg
        {
            get { return _personalMsg; }
            set { _personalMsg = value; }
        }

        public string ToTime
        {
            get { return _toTime; }
            set { _toTime = (!TimeOptions.Contains(value)) ? "12:00 AM" : value; }
        }

        public string FromTime
        {
            get { return _fromTime; }
            set { _fromTime = (!TimeOptions.Contains(value)) ? "12:00 AM" : value; }
        }

        #endregion

    }
}
