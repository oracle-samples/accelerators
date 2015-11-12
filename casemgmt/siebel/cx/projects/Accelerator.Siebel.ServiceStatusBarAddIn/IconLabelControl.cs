/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC Contact Center + Siebel Case Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  Siebel release: 8.1.1.15
 *  reference: 150520-000047
 *  date: Thu Nov 12 00:55:35 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: 71f8133f7d47b46c9eabac086ce8d0cc4a0026cd $
 * *********************************************************************************************
 *  File: IconLabelControl.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RightNow.AddIns.AddInViews;

namespace Accelerator.Siebel.SharedServices
{
    public partial class IconLabelControl : UserControl
    {
        private IGlobalContext _gContext;

        public IconLabelControl(IGlobalContext gContext)
        {
            InitializeComponent();
            _gContext = gContext;
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {
        
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // upd label if admin
            if (ConfigurationSetting.loginUserIsAdmin)
            {
                LogHistoryNSettingForm logForm = new LogHistoryNSettingForm();

                for (int i = ConfigurationSetting.logHistoryIndex - 1; i >= 0; i--)
                {
                    ListViewItem listViewItem = new ListViewItem();
                    listViewItem.Text = (string) ConfigurationSetting.logHistory.ToArray().ToList()[i];
                    logForm.logHistoryListView.Items.Add(listViewItem);
                }
            logForm.ShowDialog();
            }

            if (!ConfigurationSetting.configVerbPerfect && !ConfigurationSetting.loginUserIsAdmin)
            {
                String logMessage = "You will be logged out. Please contact your system administrator.";
                MessageBox.Show(logMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _gContext.Logout();
            }
        }
    }
}
