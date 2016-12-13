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
 *  SHA1: $Id: bc684bae7495bafb4800dc969c2d2556b2c6d3e5 $
 * *********************************************************************************************
 *  File: IOTControl.xaml.cs
 * ****************************************************************************************** */

using System.Windows;
using System.Windows.Controls;
using Accelerator.IOTArea.ViewModel;
using RightNow.AddIns.AddInViews;

namespace Accelerator.IOTArea.View
{
    /// <summary>
    /// Interaction logic for IOTControl.xaml
    /// </summary>
    public partial class IOTControl : UserControl
    {
        public IOTControl(bool inDesignMode, IRecordContext RecordContext)
        {
            var viewModel = new IOTAreaViewModel(inDesignMode, RecordContext);
            DataContext = viewModel;
            InitializeComponent();
        }

        /// <summary>
        /// Executes and fetches messages on Get Details button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetDetailsButton_OnClick(object sender, RoutedEventArgs e)
        {
            IOTAreaViewModel viewModel = DataContext as IOTAreaViewModel;
            if (viewModel != null)
            {
                viewModel.FetchMessage(FilterCriteriaLabel.Content.ToString(), FilterValueComboBox.Text);
            }
        }

        /// <summary>
        /// Executes and sets the selected attibutes new value on set button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetButton_OnClick(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as IOTAreaViewModel;
            if (viewModel != null)
            {
                viewModel.SetDeviceProperty(PropertiesCombobox.Text);
            }
        }
    }
}
