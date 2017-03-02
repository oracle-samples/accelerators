/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  Copyright (c) 2014, 2015, 2016, 2017 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + SRM Integration Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 17.2 (February 2017) 
 *  reference: 160628-000117
 *  date: Fri Feb 10 19:47:44 PST 2017
 
 *  revision: rnw-17-2-fixes-release-2
 *  SHA1: $Id: a994475cae271e4326f468b34c0d38c47dd6f6e9 $
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

namespace Accelerator.SRM.SharedServices
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
