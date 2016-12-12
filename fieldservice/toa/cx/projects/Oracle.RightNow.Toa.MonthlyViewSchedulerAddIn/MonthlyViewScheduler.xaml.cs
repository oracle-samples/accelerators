/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + OFSC Reference Integration
 *  link: http://www-content.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.2 (Feb 2015)
 *  OFSC release: 15.2 (Feb 2015)
 *  reference: 150622-000130
 *  date: Thu Sep  3 23:14:03 PDT 2015

 *  revision: rnw-15-11-fixes-release-03
 *  SHA1: $Id: 5e0c0b75a67e91d38d49449f2dc5dc3c15052579 $
 * *********************************************************************************************
 *  File: MonthlyViewScheduler.xaml.cs
 * ****************************************************************************************** */

using System;
using System.Collections.Generic;
using System.Linq;
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
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Data;
using System.Reflection;
using RightNow.AddIns.AddInViews;
using Oracle.RightNow.Toa.Client.Rightnow;
using Oracle.RightNow.Toa.Client.Common;
using Oracle.RightNow.Toa.Client.Model;
using Oracle.RightNow.Toa.Client.Logs;

namespace Oracle.RightNow.Toa.MonthlyViewSchedulerAddIn
{
    /// <summary>
    /// Interaction logic for MonthlyViewScheduler.xaml
    /// </summary>
    public partial class MonthlyViewScheduler : UserControl
    {
        private IRecordContext _recordContext;
        private bool isMonthChanging;
        //private IToaLog _log;

        public MonthlyViewScheduler(IRecordContext _rcontext)
        {
            InitializeComponent();
            _recordContext = _rcontext;
            //_log = ToaLogService.GetLog();
        }

        /// <summary>
        /// Initialize the Scheduler
        /// </summary>
        /// <param name="isManagerOverride"></param>
        /// <returns>True if the scheduler is properly initialized</returns>
        public bool InitializeScheduler(bool isManagerOverride)
        {
            //_log.Info("Inside InitializeScheduler");
            string bucket = null;
            string workOrderType = null;
            string postalCode = null;

            ICustomObject record = _recordContext.GetWorkspaceRecord(this._recordContext.WorkspaceTypeName) as ICustomObject;

            IList<IGenericField> fields = record.GenericFields;
            foreach (IGenericField field in fields)
            {
                if (field.Name == "WO_Area")
                {
                    bucket = (string)field.DataValue.Value;
                }
                else if (field.Name == "WO_Type")
                {
                    int workorderTypeId = (Int32)field.DataValue.Value;
                    string[] workordertype = RightNowConnectService.GetService().GetWorkOrderTypeFromID(workorderTypeId);
                    workOrderType = workordertype[0];
                }
                else if (field.Name == "Contact_Postal_Code")
                {
                    postalCode = field.DataValue.Value.ToString();
                }
            }

            var monthlyViewSchedulerViewModel = new MonthlyViewSchedulerViewModel();
            monthlyViewSchedulerViewModel.CalendarFirstDayOfWeek = (int)MonthlyViewCalendar.FirstDayOfWeek;
            monthlyViewSchedulerViewModel.Bucket = bucket;
            monthlyViewSchedulerViewModel.PostalCode = postalCode;
            monthlyViewSchedulerViewModel.WorkOrderType = workOrderType;
            monthlyViewSchedulerViewModel.RedQuotaCutoff = float.Parse(RightNowConfigService.GetConfigValue(RightNowConfigKeyNames.RedQuotaCutoff));
            monthlyViewSchedulerViewModel.GreenQuotaCutoff = float.Parse(RightNowConfigService.GetConfigValue(RightNowConfigKeyNames.GreenQuotaCutoff));
            monthlyViewSchedulerViewModel.IsManagerOverride = isManagerOverride;
            DataContext = monthlyViewSchedulerViewModel;

            //_log.Info("Bucket: " + bucket + " WorkOrderType: " + workOrderType + " Postal Code: " + postalCode + " IsManagerOverride: " + isManagerOverride);
            return monthlyViewSchedulerViewModel.InitializeCalendar();
        }

        /// <summary>
        /// Timeslot selection change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimeSlotComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //_log.Info("Inside TimeSlotComboBox_SelectionChanged");
            var monthlyViewSchedulerViewModel = (MonthlyViewSchedulerViewModel)DataContext;
            string currentTimeSlot = (string)TimeSlotComboBox.SelectedItem;

            //_log.Info("Current TimeSlot: " + currentTimeSlot);
            monthlyViewSchedulerViewModel.CurrentTimeSlot = currentTimeSlot;
            DataContext = monthlyViewSchedulerViewModel;
            MonthlyViewCalendar.Refresh();
            if (!currentTimeSlot.Equals("All Slots"))
            //if (currentTimeSlot == null || !currentTimeSlot.Equals("All Slots"))
            {
                DayQuota dayQuota = monthlyViewSchedulerViewModel.MonthlyQuota[monthlyViewSchedulerViewModel.CurrentDateSelected];
                
                if (dayQuota != null)
                {
                    float timeSlotQuota = dayQuota.TimeSlotQuota[currentTimeSlot];
                    if (timeSlotQuota > monthlyViewSchedulerViewModel.RedQuotaCutoff || monthlyViewSchedulerViewModel.IsManagerOverride)
                    {
                        updateRecordFields(currentTimeSlot, monthlyViewSchedulerViewModel.CurrentDateSelected);
                    }
                }
                TimeSlotDataGrid.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                monthlyViewSchedulerViewModel.ShowTimeSlotInDataGrid(monthlyViewSchedulerViewModel.CurrentDateSelected);
                TimeSlotDataGrid.Visibility = System.Windows.Visibility.Visible;
            }
        }

        /// <summary>
        /// Selected dates changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MonthlyViewCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            //_log.Info("Inside MonthlyViewCalendar_SelectedDatesChanged");
            var calendar = sender as Calendar;

            // ... See if a date is selected.
            if (calendar.SelectedDate.HasValue)
            {
                DateTime date = DateTime.Parse(calendar.SelectedDate.Value.ToString("yyyy-MM-dd"));

                if (date < DateTime.Today)
                {
                    return;
                }

                var monthlyViewSchedulerViewModel = (MonthlyViewSchedulerViewModel)DataContext;

                if (!monthlyViewSchedulerViewModel.MonthlyQuota.ContainsKey(date))
                {
                    return;
                }
                isMonthChanging = false;
                if (date.Month != monthlyViewSchedulerViewModel.CurrentDateSelected.Month)
                {
                    isMonthChanging = true;
                }
                monthlyViewSchedulerViewModel.CurrentDateSelected = date;
                DataContext = monthlyViewSchedulerViewModel;
                string timeSlot = (string)TimeSlotComboBox.SelectedItem;
                monthlyViewSchedulerViewModel.CurrentTimeSlot = timeSlot;

                //if (monthlyViewSchedulerViewModel.CurrentTimeSlot.Equals("All Slots"))
                //{
                //    //TimeSlotDataGrid.Visibility = System.Windows.Visibility.Visible;
                //    monthlyViewSchedulerViewModel.ShowTimeSlotInDataGrid(date);
                //}
                //else
                //{
                //    updateRecordFields(monthlyViewSchedulerViewModel.CurrentTimeSlot, date);
                //}

                if (timeSlot.Equals("All Slots"))
                {
                    monthlyViewSchedulerViewModel.ShowTimeSlotInDataGrid(date);
                    TimeSlotDataGrid.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    DayQuota dayQuota = monthlyViewSchedulerViewModel.MonthlyQuota[monthlyViewSchedulerViewModel.CurrentDateSelected];
                    if (dayQuota != null)
                    {
                        float timeSlotQuota = dayQuota.TimeSlotQuota[monthlyViewSchedulerViewModel.CurrentTimeSlot];
                        if (timeSlotQuota > monthlyViewSchedulerViewModel.RedQuotaCutoff || monthlyViewSchedulerViewModel.IsManagerOverride)
                        {
                            updateRecordFields(monthlyViewSchedulerViewModel.CurrentTimeSlot, date);
                        }
                        TimeSlotDataGrid.Visibility = System.Windows.Visibility.Collapsed;
                    }
                }
            }
        }

        /// <summary>
        /// Hide/Unhide the navigation month option
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MonthlyViewCalendar_DisplayDateChanged(object sender, CalendarDateChangedEventArgs e)
        {
            //_log.Info("Inside MonthlyViewCalendar_DisplayDateChanged");
            MonthlyViewCalendar calendar = (MonthlyViewCalendar)sender;
            int monthDiff = Math.Abs((calendar.DisplayDate.Month - DateTime.Today.Month) + 12 * (calendar.DisplayDate.Year - DateTime.Today.Year));
            int maxMonthNavigationAllowed = 11;

            if (TimeSlotDataGrid != null)
            {
                TimeSlotDataGrid.Visibility = System.Windows.Visibility.Collapsed;
            }
            
            if (DataContext != null)
            {
                var monthlyViewSchedulerViewModel = (MonthlyViewSchedulerViewModel)DataContext;
                if (monthlyViewSchedulerViewModel != null)
                {
                    
                    if (calendar.DisplayDate != DateTime.MinValue)
                    {
                        if (calendar.DisplayDate.Month != monthlyViewSchedulerViewModel.CurrentDateSelected.Month)
                        {
                            //User has clicked on the Month Navigation button
                            //Reset the TimeSlot in TimeSlotComboBox to "All Slots"
                            monthlyViewSchedulerViewModel.CurrentTimeSlot = "All Slots";
                            TimeSlotComboBox.SelectedIndex = 0;
                            if (calendar.DisplayDate <= DateTime.Today)
                            {
                                monthlyViewSchedulerViewModel.CurrentDateSelected = DateTime.Today;
                            }
                            else
                            {
                                monthlyViewSchedulerViewModel.CurrentDateSelected = calendar.DisplayDate;
                            }
                        }
                        else if(isMonthChanging)
                        {
                            monthlyViewSchedulerViewModel.CurrentTimeSlot = "All Slots";
                            TimeSlotComboBox.SelectedIndex = 0;
                            isMonthChanging = false;
                        }
                    }
                }
            }
            
            if (monthDiff == 0)
            {
                calendar.hidePreviousButton(true);
                calendar.hideNextButton(false);
            }
            else if (monthDiff > maxMonthNavigationAllowed)
            {
                calendar.hidePreviousButton(false);
                calendar.hideNextButton(true);
            }
            else
            {
                calendar.hidePreviousButton(false);
                calendar.hideNextButton(false);
            }
        }

        /// <summary>
        /// Refresh the scheduler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshScheduler(object sender, RoutedEventArgs e)
        {
            //_log.Info("Inside RefreshScheduler");
            var monthlyViewSchedulerViewModel = (MonthlyViewSchedulerViewModel)DataContext;
            monthlyViewSchedulerViewModel.IsRefreshRequested = true;
            monthlyViewSchedulerViewModel.CurrentTimeSlot = "All Slots";
            TimeSlotComboBox.SelectedIndex = 0;
            DataContext = monthlyViewSchedulerViewModel;
            MonthlyViewCalendar.Refresh();
        }

        /// <summary>
        /// Handle selection of TimeSlot from DataGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimeSlotDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //_log.Info("Inside TimeSlotDataGrid_MouseDoubleClick");
            TimeSlotRow row = (TimeSlotRow)TimeSlotDataGrid.CurrentItem;
            TextBlock timeSlot = (TextBlock)TimeSlotDataGrid.CurrentColumn.GetCellContent(row);
            var monthlyViewSchedulerViewModel = (MonthlyViewSchedulerViewModel)DataContext;
            DayQuota dayQuota = monthlyViewSchedulerViewModel.MonthlyQuota[monthlyViewSchedulerViewModel.CurrentDateSelected];
            if (dayQuota != null)
            {
                float timeSlotQuota = dayQuota.TimeSlotQuota[timeSlot.Text];
                if (timeSlotQuota > monthlyViewSchedulerViewModel.RedQuotaCutoff || monthlyViewSchedulerViewModel.IsManagerOverride)
                {
                    updateRecordFields(timeSlot.Text, monthlyViewSchedulerViewModel.CurrentDateSelected);
                }
            }
        }

        /// <summary>
        /// Update the Workspace Record fields
        /// </summary>
        /// <param name="timeSlot">TimeSlot</param>
        /// <param name="date">DateTime</param>
        private void updateRecordFields(string timeSlot, DateTime date)
        {
            //_log.Info("Inside updateRecordFields");
            ICustomObject record = this._recordContext.GetWorkspaceRecord(this._recordContext.WorkspaceTypeName) as ICustomObject;
            IList<IGenericField> fields = record.GenericFields;

            foreach (IGenericField field in fields)
            {
                if (field.Name == "WO_Date")
                {
                    field.DataValue.Value = date;
                }
                else if (field.Name == "WO_Time_Slot")
                {
                    field.DataValue.Value = timeSlot;
                }
                _recordContext.RefreshWorkspace();
            }
        }
    }
}