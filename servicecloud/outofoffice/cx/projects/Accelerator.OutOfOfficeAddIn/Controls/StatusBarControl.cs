/* *********************************************************************************************
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
 *  SHA1: $Id: ac6d3355a7a78d445306c990160eb966acd34260 $
 * *********************************************************************************************
 *  File: StatusBarControl.cs
 * *********************************************************************************************/

using System;
using System.Windows;
using System.Windows.Forms;
using Accelerator.OutOfOffice.Client.Common;
using Accelerator.OutOfOffice.Client.Model;
using Accelerator.OutOfOffice.Client.RightNow;
using Accelerator.OutOfOffice.View;
using RightNow.AddIns.AddInViews;

namespace Accelerator.OutOfOffice
{
    public partial class StatusBarControl : UserControl
    {
        private IGlobalContext _gContext;
        private StaffAccount _staffAccount;

        public StatusBarControl(IGlobalContext gContext)
        {
            _staffAccount = RightNowConnectService.GetService().GetAccountDetails();

            InitializeComponent(_staffAccount);
            _gContext = gContext;
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {
        
        }

        /// <summary>
        /// Initializes and displays out of office dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void statusButton_Click(object sender, EventArgs e)
        {
            _staffAccount = RightNowConnectService.GetService().GetAccountDetails();

            if (_staffAccount != null)
            {
                var OOOControl = new OutOfOfficeControl(this);
                OOOControl.PopulateControls(_staffAccount);
                Window window = new Window
                {
                    Title = Common.Title,
                    Content = OOOControl,
                    SizeToContent = SizeToContent.WidthAndHeight,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    ResizeMode = ResizeMode.NoResize
                };
                window.ShowDialog();
            }
        }
    }
}
