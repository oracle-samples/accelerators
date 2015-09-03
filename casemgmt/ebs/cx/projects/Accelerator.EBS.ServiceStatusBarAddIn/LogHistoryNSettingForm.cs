/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + EBS Enhancement
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.5 (May 2015)
 *  EBS release: 12.1.3
 *  reference: 150202-000157
 *  date: Wed Sep  2 23:11:40 PDT 2015

 *  revision: rnw-15-8-fixes-release-01
 *  SHA1: $Id: 5cf82c0993beaa05471888c70112a9b44d8d2569 $
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

namespace Accelerator.EBS.SharedServices
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
