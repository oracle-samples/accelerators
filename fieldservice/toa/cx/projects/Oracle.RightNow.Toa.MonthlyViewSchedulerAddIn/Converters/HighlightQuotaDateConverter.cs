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
 *  SHA1: $Id: 7923e0989845822de6b8884b52e5ad8551f25043 $
 * *********************************************************************************************
 *  File: HighlightQuotaDateConverter.cs
 * ****************************************************************************************** */

using System;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Oracle.RightNow.Toa.MonthlyViewSchedulerAddIn
{
    public class HighlightQuotaDateConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Exit if values not set
            if ((values[0] == null) || (values[1] == null)) return null;

            // Get values passed in
            var targetDate = (DateTime)values[0];
            var parent = (MonthlyViewCalendar)values[1];

            DateTime dateTime = parent.DisplayDate;

            // Exit if highlighting turned off
            if (parent.ShowDateHighlighting == false) return null;

            // Exit if no HighlightedDateText array
            //if (parent.HighlightedDateText == null) return null;

            /* The WPF calendar always displays six rows of dates, and it fills out those rows 
             * with dates from the preceding and following month. These 'gray' date numbers (29,
             * 30, 31, and so on, and 1, 2, 3, and so on) duplicate date numbers in the current 
             * month, so we ignore them. The tool tips for these gray dates will appear in their 
             * own display months. */

            // Exit if target date not in the current display month
            //	if (!targetDate.IsSameMonthAs(parent.DisplayDate)) return null;

            if (dateTime.Year == 1)
            {
                return "Gray";
            }

            var firstdayofmonth = new DateTime(dateTime.Year, dateTime.Month, 1);
            var dayofweek = (int)firstdayofmonth.DayOfWeek;
            if (dayofweek == 0) dayofweek = 7; // set sunday to day 7.
            if (dayofweek == (int)parent.FirstDayOfWeek) dayofweek = 8; // show a whole week ahead
            if (parent.FirstDayOfWeek == (int)DayOfWeek.Sunday) dayofweek += 1;
            DateTime firstdate = firstdayofmonth.AddDays(-((Double)dayofweek) + 1);
            var index = (targetDate - firstdate).Days;

            var quota = parent.HighlightedDateText[index];

            if (quota == null)
            {
                return "Gray";
            }
            else
            {
                return quota;
            }
        }

        /// <summary>
        /// Not used.
        /// </summary>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return new object[0];
        }

        #endregion
    }
}
