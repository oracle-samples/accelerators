/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  Copyright (c) 2014, 2015, 2016, 2017 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + SRM Integration Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 17.2 (February 2017) 
 *  reference: 160628-000117
 *  date: Fri Feb 10 19:47:45 PST 2017
 
 *  revision: rnw-17-2-fixes-release-2
 *  SHA1: $Id: da74b04f6815ec4c45b9efb480b36890d6b4d148 $
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

namespace Accelerator.SRM.SharedServices
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
