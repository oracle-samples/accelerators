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
 *  SHA1: $Id: 593adc797985973858c235a76b68388d482fdd48 $
 * *********************************************************************************************
 *  File: MonthlyViewCalendar.cs
 * ****************************************************************************************** */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Oracle.RightNow.Toa.MonthlyViewSchedulerAddIn
{
    public class MonthlyViewCalendar : Calendar
    {
        #region Dependency Properties

        // The background brush used for the date highlight.
        public static DependencyProperty DateHighlightBrushProperty = DependencyProperty.Register
             (
                  "DateHighlightBrush",
                  typeof(Brush),
                  typeof(MonthlyViewCalendar),
                  new PropertyMetadata(new SolidColorBrush(Colors.Red))
             );

        // The list of dates to be highlighted.
        public static DependencyProperty HighlightedDateTextProperty = DependencyProperty.Register
            (
                "HighlightedDateText",
                typeof(String[]),
                typeof(MonthlyViewCalendar),
                new PropertyMetadata()
            );


        // Whether highlights should be shown.
        public static DependencyProperty ShowDateHighlightingProperty = DependencyProperty.Register
             (
                  "ShowDateHighlighting",
                  typeof(bool),
                  typeof(MonthlyViewCalendar),
                  new PropertyMetadata(true)
             );

        // Whether tool tips should be shown with highlights.
        public static DependencyProperty ShowHighlightedDateTextProperty = DependencyProperty.Register
             (
                  "ShowHighlightedDateText",
                  typeof(bool),
                  typeof(MonthlyViewCalendar),
                  new PropertyMetadata(true)
             );

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor.
        /// </summary>
        static MonthlyViewCalendar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MonthlyViewCalendar),
                 new FrameworkPropertyMetadata(typeof(MonthlyViewCalendar)));
        }

        /// <summary>
        /// Instance constructor.
        /// </summary>
        public MonthlyViewCalendar()
        {
            /* We initialize the HighlightedDateText property to an array of 31 strings,
             * since 31 is the maximum number of days in any month. */

            // Initialize HighlightedDateText property
            //this.HighlightedDateText = new string[31];
            this.HighlightedDateText = new string[42];
        }

        #endregion

        #region CLR Properties

        /// <summary>
        /// The background brush used for the date highlight.
        /// </summary>
        [Browsable(true)]
        [Category("Highlighting")]
        public Brush DateHighlightBrush
        {
            get { return (Brush)GetValue(DateHighlightBrushProperty); }
            set { SetValue(DateHighlightBrushProperty, value); }
        }


        /// <summary>
        /// The tool tips for highlighted dates.
        /// </summary>
        [Browsable(true)]
        [Category("Highlighting")]
        public String[] HighlightedDateText
        {
            get { return (String[])GetValue(HighlightedDateTextProperty); }
            set { SetValue(HighlightedDateTextProperty, value); }
        }

        /// <summary>
        /// Whether highlights should be shown.
        /// </summary>
        [Browsable(true)]
        [Category("Highlighting")]
        public bool ShowDateHighlighting
        {
            get { return (bool)GetValue(ShowDateHighlightingProperty); }
            set { SetValue(ShowDateHighlightingProperty, value); }
        }

        /// <summary>
        /// Whether tool tips should be shown with highlights.
        /// </summary>
        [Browsable(true)]
        [Category("Highlighting")]
        public bool ShowHighlightedDateText
        {
            get { return (bool)GetValue(ShowHighlightedDateTextProperty); }
            set { SetValue(ShowHighlightedDateTextProperty, value); }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Refreshes the calendar highlighting
        /// </summary>
        public void Refresh()
        {
            var realDisplayDate = this.DisplayDate;
            this.DisplayDate = DateTime.MinValue;
            this.DisplayDate = realDisplayDate;
        }

        #endregion
        public Button PrevBtn;
        public Button NextBtn;

        protected bool _HidePrevNextBtns;
        public bool HidePrevNextBtns
        {
            get
            {
                return (_HidePrevNextBtns);
            }
            set
            {
                _HidePrevNextBtns = value;
                if (PrevBtn != null)
                {
                    PrevBtn.Visibility = _HidePrevNextBtns ? Visibility.Hidden : Visibility.Visible;
                    NextBtn.Visibility = _HidePrevNextBtns ? Visibility.Hidden : Visibility.Visible;
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var cal = this.Template.FindName("PART_CalendarItem", this) as CalendarItem;

            cal.Loaded += new RoutedEventHandler(cal_Loaded);
        }

        public void hidePreviousButton(bool hide)
        {
            if (PrevBtn == null)
            {
                return;
            }

            if (hide)
            {
                PrevBtn.Visibility = Visibility.Hidden;
            }
            else
            {
                PrevBtn.Visibility = Visibility.Visible;
            }
        }

        public void hideNextButton(bool hide)
        {
            if (NextBtn == null)
            {
                return;
            }

            if (hide)
            {
                NextBtn.Visibility = Visibility.Hidden;
            }
            else
            {
                NextBtn.Visibility = Visibility.Visible;
            }
        }

        void cal_Loaded(object sender, RoutedEventArgs e)
        {
            var cal = sender as CalendarItem;
            PrevBtn = cal.Template.FindName("PART_PreviousButton", cal) as Button;
            NextBtn = cal.Template.FindName("PART_NextButton", cal) as Button;

            PrevBtn.Visibility = Visibility.Hidden;
            NextBtn.Visibility = Visibility.Visible;
        }
    }
}
