/* * *******************************************************************************************
*  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC Out of Office Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 16.5 (May 2016) 
 *  reference: 150916-000080
 *  date: Thu Mar 17 23:37:54 PDT 2016
 
 *  revision: rnw-16-5-fixes-release-1
*  SHA1: $Id: 9531c18059e0b01f518cb3877fe60d7874130d7b $
* *********************************************************************************************
*  File: OutOfOfficeControl.xaml.cs
* ****************************************************************************************** */

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using Accelerator.OutOfOffice.Client.Logs;
using Accelerator.OutOfOffice.Client.Model;
using Accelerator.OutOfOffice.Client.Common;
using Accelerator.OutOfOffice.Client.RightNow;
using Accelerator.OutOfOffice.Client.Services;
using Color = System.Drawing.Color;
using UserControl = System.Windows.Controls.UserControl;

namespace Accelerator.OutOfOffice.View
{
    /// <summary>
    /// Interaction logic for OutOfOfficeControl.xaml
    /// </summary>
    public partial class OutOfOfficeControl : UserControl
    {
        private StatusBarControl _statusBarControl;
        private StaffAccount _staffAccount;

        private IOSvCLog logger;

        public OutOfOfficeControl(StatusBarControl statusBarControl)
        {
            _statusBarControl = statusBarControl;
            logger = OSvCLogService.GetLog();
            InitializeComponent();
        }

        /// <summary>
        /// Populates value of control on intialization of the control
        /// </summary>
        /// <param name="staffAccount"></param>
        public void PopulateControls(StaffAccount staffAccount)
        {
            logger.Debug("OutOfOfficeControl - PopulateControls() - Entry");
            
            var OutOfOfficeViewModel = new OutOfOfficeViewModel(staffAccount);
            DataContext = OutOfOfficeViewModel;
            _staffAccount = staffAccount;

            logger.Debug("OutOfOfficeControl - PopulateControls() - Exit");
        }

        /// <summary>
        /// Saves date on OK button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="routedEventArgs"></param>
        public void OkClick(object sender, RoutedEventArgs routedEventArgs)
        {
            logger.Debug("OutOfOfficeControl - OkClick() - Entry");

            var updatedAccount = new StaffAccount();

            updatedAccount.OooFlag = OutOfOfficeCheckbox.IsChecked.GetValueOrDefault();
            updatedAccount.OooTimezone = (TimezoneDropdown.SelectedValue != null) ? TimezoneDropdown.SelectedValue.ToString() : null;
            DateTime? fromDateTime = null;
            DateTime? toDateTime = null;

            if(FromDateTime.SelectedDate != null || ToDateTime.SelectedDate != null){
            //Combining date and time to store
            string fromDate = FromDateTime.SelectedDate.GetValueOrDefault().Date.ToString("d");
            string fromTime = FromTimeDropdown.SelectedValue.ToString();

            string toDate = ToDateTime.SelectedDate.GetValueOrDefault().Date.ToString("d");
            string toTime = ToTimeDropdown.SelectedValue.ToString();

            fromDateTime = Convert.ToDateTime(fromDate + " " + fromTime);
            toDateTime = Convert.ToDateTime(toDate + " " + toTime);

            //Validating if user entered date/time without timezone.
            if ((fromDateTime != default(DateTime) || toDateTime != default(DateTime)) && TimezoneDropdown.SelectedValue == null)
            {
                System.Windows.Forms.MessageBox.Show(OOOExceptionMessages.TimezoneCannotBeEmpty, 
                    Common.ErrorLabel, MessageBoxButtons.OK, MessageBoxIcon.Error);
                TimezoneLabel.Content = "*" + Common.TimezoneLabel;
                TimezoneLabel.Foreground = new SolidColorBrush(Colors.Red);
                return;
            }
            //Validating if user entered From Date/Time is less than To Date/Time.
            if (fromDateTime > toDateTime)
            {
                System.Windows.Forms.MessageBox.Show(OOOExceptionMessages.FromDateGreaterThanToDate,
                 Common.ErrorLabel, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (updatedAccount.OooTimezone != null)
            {
                string windowsTimezone = TimezoneService.GetService().GetWindowsTimezone(updatedAccount.OooTimezone);
                TimeZoneInfo timeszone =  TimeZoneInfo.FindSystemTimeZoneById(windowsTimezone);

                fromDateTime = TimeZoneInfo.ConvertTimeToUtc((DateTime)fromDateTime, timeszone);
                toDateTime = TimeZoneInfo.ConvertTimeToUtc((DateTime)toDateTime, timeszone);
                
            }
            }

            updatedAccount.OooStart = fromDateTime;
            updatedAccount.OooEnd = toDateTime;
            updatedAccount.OooMsgOption = PersonalMsgOptionsDropdown.SelectedValue.ToString();
            updatedAccount.OooMsg = PersonalMsgOptionsDropdown.SelectedValue.ToString().Equals(PersonalMsgOptions.StandardMessage) ? " " : PersonalMsgTextbox.Text;

            var result = RightNowConnectService.GetService().updateCustomFields(updatedAccount);
            if (result)
            {
                    _statusBarControl.statusButton.Text = (updatedAccount.OooFlag) ? Common.OutOfOfficeLabel : Common.AvailableLabel;
                    _statusBarControl.statusButton.ForeColor = (updatedAccount.OooFlag) ? Color.DarkRed : Color.DarkGreen;
            }

            Window.GetWindow(this).Close();

            logger.Debug("OutOfOfficeControl - OkClick() - Exit");
        }

        /// <summary>
        /// Closes the window on cancel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="routedEventArgs"></param>
        public void CancelClick(object sender, RoutedEventArgs routedEventArgs)
        {
            Window.GetWindow(this).Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OutOfOfficeCheckbox_OnChecked(object sender, RoutedEventArgs e)
        {
            logger.Debug("OutOfOfficeControl - OutOfOfficeCheckbox_OnChecked() - Entry");

            if (_staffAccount.OooFlag)
                return;
            OkButton.IsEnabled = false;
            
            FromDateTime.SelectedDate = DateTime.Now;
            FromTimeDropdown.SelectedValue = "12:00 AM";
            ToDateTime.SelectedDate = null;
            ToTimeDropdown.SelectedValue = null;

            ToLabel.Content = "*" + ToLabel.Content;
            ToLabel.Foreground = new SolidColorBrush(Colors.Red);

            //PersonalMsgOptionsDropdown.SelectedValue = PersonalMsgOptions.StandardMessage;
            //PersonalMsgTextbox.Text = " ";

            logger.Debug("OutOfOfficeControl - OutOfOfficeCheckbox_OnChecked() - Exit");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OutOfOfficeCheckbox_OnUnchecked(object sender, RoutedEventArgs e)
        {
            logger.Debug("OutOfOfficeControl - OutOfOfficeCheckbox_OnUnchecked() - Entry");

            ToLabel.Content = Common.ToLabel;           

            OkButton.IsEnabled = true;

            FromDateTime.SelectedDate = null;
            FromTimeDropdown.SelectedValue = String.Format(Common.HourMinuteFormat, _staffAccount.OooStart);
            ToDateTime.SelectedDate = null;
            ToTimeDropdown.SelectedValue = String.Format(Common.HourMinuteFormat, _staffAccount.OooEnd);
            PersonalMsgOptionsDropdown.SelectedValue = _staffAccount.OooMsgOption;
            PersonalMsgTextbox.Text = _staffAccount.OooMsg;

            ToLabel.Foreground = new SolidColorBrush(Colors.Black);

            logger.Debug("OutOfOfficeControl - OutOfOfficeCheckbox_OnUnchecked() - Exit");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToDateTime_OnSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ToDateTime.SelectedDate != null &&  ToTimeDropdown.SelectedValue != null)
            {
                OkButton.IsEnabled = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimezoneDropdown_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ToDateTime.SelectedDate != null && ToTimeDropdown.SelectedValue != null)
            {
                OkButton.IsEnabled = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToTimeDropdown_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ToDateTime.SelectedDate != null && ToTimeDropdown.SelectedValue != null)
            {
                OkButton.IsEnabled = true;
            }
        }
    }
}
