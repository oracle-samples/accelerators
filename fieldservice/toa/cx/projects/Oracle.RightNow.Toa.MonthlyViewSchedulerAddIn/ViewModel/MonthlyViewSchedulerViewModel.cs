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
 *  date: Mon Aug 24 09:01:20 PDT 2015

 *  revision: rnw-15-11-fixes-release-01
 *  SHA1: $Id: eb6c2e64ce02cf49a118547a91e72c1ef0c85ac1 $
 * *********************************************************************************************
 *  File: MonthlyViewSchedulerViewModel.cs
 * ****************************************************************************************** */

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Oracle.RightNow.Toa.Client.Model;
using Oracle.RightNow.Toa.Client.Services;
using Oracle.RightNow.Toa.Client.Exceptions;
using Oracle.RightNow.Toa.Client.Logs;
using Oracle.RightNow.Toa.Client.Common;
using System.Windows.Forms;

namespace Oracle.RightNow.Toa.MonthlyViewSchedulerAddIn
{
    class MonthlyViewSchedulerViewModel : ViewModelBase
    {
        private ObservableCollection<String> timeSlotItems;
        private int calendarFirstDayOfWeek;
        private DateTime p_DisplayDate;
        private string[] p_HighlightedDateText;
        private float redQuotaCutoff;
        private float greenQuotaCutoff;
        private Dictionary<DateTime, DayQuota> monthlyQuota;
        private String bucket;
        private String workOrderType;
        private DateTime m_OldDisplayDate;
        private IToaLog _log;

        public DateTime OldDisplayDate
        {
            get { return m_OldDisplayDate; }
            set { m_OldDisplayDate = value; }
        }
        private String postalCode;
        public event EventHandler RefreshRequested;
        private string currentTimeSlot;
        private ObservableCollection<TimeSlotRow> timeSlotRowInDataGrid;
        private DateTime currentDateSelected;
        private bool isRefreshRequested;
        private bool isManagerOverride;

        public bool IsManagerOverride
        {
            get { return isManagerOverride; }
            set { isManagerOverride = value; }
        }

        public bool IsRefreshRequested
        {
            get { return isRefreshRequested; }
            set { isRefreshRequested = value; }
        }

        public DateTime CurrentDateSelected
        {
            get { return currentDateSelected; }
            set { currentDateSelected = value; }
        }

        public ObservableCollection<TimeSlotRow> TimeSlotRowInDataGrid
        {
            get { return timeSlotRowInDataGrid; }
            set {
                timeSlotRowInDataGrid = value;
                base.RaisePropertyChangedEvent("TimeSlotRowInDataGrid");
            }
        }

        public string CurrentTimeSlot
        {
            get { return currentTimeSlot; }
            set { currentTimeSlot = value; }
        }

        public ObservableCollection<string> TimeSlotItems
        {
            get { return timeSlotItems; }
            set
            {
                timeSlotItems = value;
                base.RaisePropertyChangedEvent("TimeSlotItems");
            }
        }

        public int CalendarFirstDayOfWeek
        {
            get { return calendarFirstDayOfWeek; }
            set { calendarFirstDayOfWeek = value; }
        }

        public DateTime DisplayDate
        {
            get { return p_DisplayDate; }

            set
            {
                base.RaisePropertyChangingEvent("DisplayDate");
                p_DisplayDate = value;
                base.RaisePropertyChangedEvent("DisplayDate");
            }
        }

        public string[] HighlightedDateText
        {
            get { return p_HighlightedDateText; }
            set
            {
                base.RaisePropertyChangingEvent("HighlightedDateText");
                p_HighlightedDateText = value;
                base.RaisePropertyChangedEvent("HighlightedDateText");
            }
        }

        public float RedQuotaCutoff
        {
            get { return redQuotaCutoff; }
            set { redQuotaCutoff = value; }
        }

        public float GreenQuotaCutoff
        {
            get { return greenQuotaCutoff; }
            set { greenQuotaCutoff = value; }
        }

        public String Bucket
        {
            get { return bucket; }
            set { bucket = value; }
        }

        public String WorkOrderType
        {
            get { return workOrderType; }
            set { workOrderType = value; }
        }

        public String PostalCode
        {
            get { return postalCode; }
            set { postalCode = value; }
        }

        public Dictionary<DateTime, DayQuota> MonthlyQuota
        {
            get { return monthlyQuota; }

            set
            {
                monthlyQuota = value;
            }
        }

        public MonthlyViewSchedulerViewModel()
        {
            TimeSlotRowInDataGrid = new ObservableCollection<TimeSlotRow>();
            _log = ToaLogService.GetLog();
        }

        /// <summary>
        /// Initialize the Calendar Control
        /// </summary>
        /// <returns>True if Calendar is properly initialized</returns>
        public bool InitializeCalendar()
        {
            _log.Notice("Inside InitializeCalendar");
            p_HighlightedDateText = new string[42];

            monthlyQuota = new Dictionary<DateTime, DayQuota>();

            currentDateSelected = OldDisplayDate = p_DisplayDate = DateTime.Today;

            ToaRequestResultCode result = this.getQuotaForMonth();

            if (result == ToaRequestResultCode.Failure)
            {
                return false;
            }
           
            // Subscribe to PropertyChanged event
            this.PropertyChanged += OnPropertyChanged;

            return true;
        }

        /// <summary>
        /// Property Changed Event Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _log.Notice("Inside OnPropertyChanged");
            // Ignore properties other than DisplayDate
            if (e.PropertyName != "DisplayDate") return;

            // Ignore change if date is DateTime.MinValue
            if (p_DisplayDate == DateTime.MinValue) return;

            if (!IsRefreshRequested && IsSameMonthAs(p_DisplayDate, OldDisplayDate))
            {
                this.showQuotaForTimeSlots();
            }
            else
            {
                this.getQuotaForMonth();
            }
            IsRefreshRequested = false;    
            // Update OldDisplayDate
            OldDisplayDate = p_DisplayDate;
        }

        /// <summary>
        /// Determines whether a subject date is in the same month as a date passed in.
        /// </summary>
        /// <param name="subjectDate"> The subject date.</param>
        /// <param name="dateToCompare">The date passed in.</param>
        /// <returns>True if the two DateTime objects are in the same month; false otherwise.</returns>
        private bool IsSameMonthAs(DateTime subjectDate, DateTime dateToCompare)
        {
            _log.Notice("Inside IsSameMonthAs");
            var monthIsSame = subjectDate.Month == dateToCompare.Month;
            var yearIsSame = subjectDate.Year == dateToCompare.Year;
            return monthIsSame && yearIsSame;
        }

        /// <summary>
        /// Show Quota for TimeSlots
        /// </summary>
        private void showQuotaForTimeSlots()
        {
            _log.Notice("Inside showQuotaForTimeSlots");
            int i = getStartDateIndex();
            
            for (int index = 0; index < 42; index++)
            {
                p_HighlightedDateText[index] = null;
            }

            foreach (KeyValuePair<DateTime, DayQuota> date in monthlyQuota.OrderBy(key => key.Key))
            //foreach (var date in monthlyQuota.Keys)
            {
                //DayQuota dayQuota = monthlyQuota[date];
                DayQuota dayQuota = date.Value;
                if (CurrentTimeSlot != null && CurrentTimeSlot.Equals("All Slots"))
                {
                    int totalSlots = dayQuota.TimeSlotQuota.Count;
                    int availableSlots = 0;
                    int unAvailableSlots = 0;
                    foreach (KeyValuePair<string, float> timeSlotQuota in dayQuota.TimeSlotQuota)
                    {
                        if (timeSlotQuota.Value <= RedQuotaCutoff)
                        {
                            unAvailableSlots++;
                        }
                        else if (timeSlotQuota.Value >= GreenQuotaCutoff)
                        {
                            availableSlots++;
                        }
                    }
                    if (unAvailableSlots == totalSlots)
                    {
                        p_HighlightedDateText[i] = "Red";
                    }
                    else if (availableSlots == totalSlots)
                    {
                        p_HighlightedDateText[i] = "Green";
                    }
                    else
                    {
                        p_HighlightedDateText[i] = "Yellow";
                    }

                }
                else
                {
                    float quota = dayQuota.TimeSlotQuota[CurrentTimeSlot];
                    if (quota <= RedQuotaCutoff)
                    {
                        p_HighlightedDateText[i] = "Red";
                    }
                    else if (quota >= GreenQuotaCutoff)
                    {
                        p_HighlightedDateText[i] = "Green";
                    }
                    else
                    {
                        p_HighlightedDateText[i] = "Yellow";
                    }

                }
                i++;
            }
        }

        /// <summary>
        /// RequestRefresh
        /// </summary>
        private void RequestRefresh()
        {
            if (this.RefreshRequested != null)
            {
                this.RefreshRequested(this, new EventArgs());
            }
        }

        /// <summary>
        /// Fetch the StartDate Index
        /// </summary>
        /// <returns></returns>
        private int getStartDateIndex()
        {
            _log.Notice("Inside getStartDateIndex");
            var displayMonth = this.DisplayDate.Month;
            var displayYear = this.DisplayDate.Year;

            // Get the last day of the display month
            var month = this.DisplayDate.Month;
            var year = this.DisplayDate.Year;
            var lastDayOfMonth = DateTime.DaysInMonth(year, month);

            var firstdayofmonth = new DateTime(displayYear, displayMonth, 1);
            var dayofweek = (int)firstdayofmonth.DayOfWeek;
            if (dayofweek == 0) dayofweek = 7; // set sunday to day 7.
            if (dayofweek == calendarFirstDayOfWeek) dayofweek = 8; // show a whole week ahead
            if (calendarFirstDayOfWeek == (int)DayOfWeek.Sunday) dayofweek += 1;
            DateTime firstdate = firstdayofmonth.AddDays(-((Double)dayofweek) + 1);
            DateTime lastdate = firstdate.AddDays(41);

            int i = 0;
            if (DateTime.Today.Month == displayMonth)
            {
                //
                i = (DateTime.Today - firstdate).Days;
            }

            _log.Notice("StartDate Index: " + i);
            return i;
        }

        /// <summary>
        /// Get Quota Dates for the month
        /// </summary>
        /// <returns></returns>
        private DateTime[] getQuotaDates()
        {
            _log.Notice("Inside getQuotaDates");
            var displayMonth = this.DisplayDate.Month;
            var displayYear = this.DisplayDate.Year;

            var lastDayOfMonth = DateTime.DaysInMonth(displayYear, displayMonth);
            var firstdayofmonth = new DateTime(displayYear, displayMonth, 1);

            var dayofweek = (int)firstdayofmonth.DayOfWeek;
            if (dayofweek == 0) dayofweek = 7; // set sunday to day 7.
            if (dayofweek == calendarFirstDayOfWeek) dayofweek = 8; // show a whole week ahead
            if (calendarFirstDayOfWeek == (int)DayOfWeek.Sunday) dayofweek += 1;
            DateTime firstdate = firstdayofmonth.AddDays(-((Double)dayofweek) + 1);
            DateTime lastdate = firstdate.AddDays(41);

            DateTime[] quotaDates = null;
            int i = 0;
            if (DateTime.Today.Month == displayMonth)
            {
                //
                i = (DateTime.Today - firstdate).Days;
                lastdate = DateTime.Today.AddDays(41 - i);
                quotaDates = Enumerable.Range(0, 1 + lastdate.Subtract(DateTime.Today).Days).Select(offset => DateTime.Today.AddDays(offset)).ToArray();
            }
            else
            {
                //Next months
                quotaDates = Enumerable.Range(0, 1 + lastdate.Subtract(firstdate).Days).Select(offset => firstdate.AddDays(offset)).ToArray();
            }

            return quotaDates;
        }

        /// <summary>
        /// Get the quota for a given month
        /// </summary>
        public ToaRequestResultCode getQuotaForMonth()
        {
            _log.Notice("Inside getQuotaForMonth");
            CapacityModel request = new CapacityModel();
            request.QuotaDates = getQuotaDates();
            request.CalculateDuration = true;
            request.CalculateDurationSpecified = true;
            request.CalculateTravelTime = true;
            request.CalculateTravelTimeSpecified = true;
            request.CalculateWorkSkill = true;
            request.CalculateWorkSkillSpecified = true;
            request.ReturnTimeSlotInfo = true;
            request.ReturnTimeSlotInfoSpecified = true;
            request.Location = new string[] { Bucket };

            ActivityField zipField = new ActivityField();
            zipField.Name = "czip";
            zipField.Value = PostalCode;

            ActivityField workType = new ActivityField();
            workType.Name = "aworktype";
            workType.Value = WorkOrderType;

            List<ActivityField> activityFields = new List<ActivityField>();
            activityFields.Add(zipField);
            activityFields.Add(workType);

            request.ActivityField = activityFields.ToArray();

            ICapacityService service = CapacityService.GetService();
            ToaRequestResult response = service.GetQuotaForMonth(request);

            if (response.ResultCode == ToaRequestResultCode.Failure)
            {
                return response.ResultCode;
            }

            CapacityModel model = (CapacityModel)response.DataModels[0];
            monthlyQuota = null;
            monthlyQuota = model.DayQuota;

            List<string> timeSlots = new List<string>();
            timeSlots.Add("All Slots");
            timeSlots.AddRange(model.Timeslots);
            
            TimeSlotItems = new ObservableCollection<string>(timeSlots);

            CurrentTimeSlot = "All Slots";
            //CurrentDateSelected = DisplayDate;
            this.showQuotaForTimeSlots();
            //this.RequestRefresh();
            return response.ResultCode;
        }

        /// <summary>
        /// Show the time slots in the DataGrid
        /// </summary>
        /// <param name="date"></param>
        public void ShowTimeSlotInDataGrid(DateTime date)
        {
            _log.Notice("Inside ShowTimeSlotInDataGrid");
            TimeSlotRowInDataGrid.Clear();
            //CurrentDateSelected = date;
            DayQuota dailyQuota = monthlyQuota[date];
            Dictionary<string, float> tsQuotas = dailyQuota.TimeSlotQuota;

            List<TimeSlotCell[]> listTimeSlots = new List<TimeSlotCell[]>();
            TimeSlotCell[] listTimeSlot = new TimeSlotCell[4];
            int i = 0;

            foreach (KeyValuePair<string, float> tsQuota in tsQuotas.OrderBy(key => key.Key))
            {
                string quotaColor;
                if (tsQuota.Value <= RedQuotaCutoff)
                {
                    quotaColor = "Red";
                }
                else if (tsQuota.Value >= GreenQuotaCutoff)
                {
                    quotaColor = "Green";
                }
                else
                {
                    quotaColor = "Yellow";
                }
                TimeSlotCell cell = new TimeSlotCell(tsQuota.Key, quotaColor);
                listTimeSlot[i] = cell;
                if (i == 3)
                {
                    listTimeSlots.Add(listTimeSlot);
                    listTimeSlot = new TimeSlotCell[4];
                    i = 0;
                }
                else
                {
                    i++;
                }
            }

            if (listTimeSlot.Length > 0)
            {
                listTimeSlots.Add(listTimeSlot);
            }

            foreach (TimeSlotCell[] superList in listTimeSlots)
            {
                TimeSlotRow timeSlotRow = new TimeSlotRow(superList[0], superList[1], superList[2], superList[3]);
                if (superList[0] != null)
                {
                    TimeSlotRowInDataGrid.Add(timeSlotRow);
                }
            }
        }
    }

    /// <summary>
    /// TimeSlotRow class
    /// </summary>
    public partial class TimeSlotRow
    {
        private TimeSlotCell tsC1, tsC2, tsC3, tsC4;

        public TimeSlotCell TsC1 { get { return tsC1; } set { tsC1 = value; } }
        public TimeSlotCell TsC2 { get { return tsC2; } set { tsC2 = value; } }
        public TimeSlotCell TsC3 { get { return tsC3; } set { tsC3 = value; } }
        public TimeSlotCell TsC4 { get { return tsC4; } set { tsC4 = value; } }

        public TimeSlotRow(TimeSlotCell s1, TimeSlotCell s2, TimeSlotCell s3, TimeSlotCell s4)
        {
            tsC1 = s1;
            tsC2 = s2;
            tsC3 = s3;
            tsC4 = s4;
        }
    }

    /// <summary>
    /// TimeSlotCell class
    /// </summary>
    public partial class TimeSlotCell
    {
        private String slot;
        private String color;
        public string Slot { get { return slot; } set { slot = value; } }
        public string Color { get { return color; } set { color = value; } }

        public TimeSlotCell(String slot1, String color1)
        {
            slot = slot1;
            color = color1;
        }
    }
}
