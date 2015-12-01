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
 *  date: Mon Nov 30 20:14:29 PST 2015

 *  revision: rnw-15-11-fixes-release-2
 *  SHA1: $Id: 398c4c984c6bdc7817f71dcd4826fc228d2e77bc $
 * *********************************************************************************************
 *  File: LogHistoryNSettingForm.cs
 * *********************************************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Accelerator.Siebel.SharedServices
{
    public partial class LogHistoryNSettingForm : Form
    {
        public LogHistoryNSettingForm()
        {
            InitializeComponent();
        }

        private void logHistoryListView_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void logHistoryMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConfigurationSetting.logLevel = (ConfigurationSetting.LogLevelEnum)logLevelSettingMenu.SelectedIndex;
        }

        private void validateConfig_Click(object sender, EventArgs e)
        {
            if (ConfigurationSetting.getConfigVerb(ConfigurationSetting._gContext))
            {
                DialogResult result = MessageBox.Show(
                    "Successful.\nYou must restart the console for the changes to take effect.\nClick OK to logout, Cancel to continue.", "Configuration Verb Validation",
                    MessageBoxButtons.OKCancel);

                if (result == DialogResult.OK)
                {
                    ConfigurationSetting._gContext.Logout();
                }
            
            }
        }
    }
}
