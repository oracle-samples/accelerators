/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC Contact Center + Siebel Case Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  Siebel release: 8.1.1.15
 *  reference: 150520-000047
 *  date: Mon Nov 30 20:14:25 PST 2015

 *  revision: rnw-15-11-fixes-release-2
 *  SHA1: $Id: 6d1db880fecdad723e8dbf57aaad1763f8c716fe $
 * *********************************************************************************************
 *  File: ActivityInformationControll.cs
 * *********************************************************************************************/

using Accelerator.Siebel.SharedServices;

using RightNow.AddIns.AddInViews;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Accelerator.Siebel.SharedServices.Logs;

namespace Accelerator.Siebel.ActivityAddin
{
    public partial class ActivityInformationControl : UserControl
    {
        public IRecordContext _rc { get; set; }
        public IGlobalContext _gc { get; set; }
        public LogWrapper _log { get; set; }
        public int _logIncidentId = 0;
        public IIncident incident { get; set; }

        public string InputComment;
        public string InputDescription;
        public string InputType;
        public System.DateTime InputDue;
        public string InputPriority;
        public string InputStatus;

        // Used for multi-threading
        delegate void SetTextCallback(Control c, string text);
        delegate void SetVisibilityCallback(Control c, bool visible);

        public ActivityInformationControl()
        {
            InitializeComponent();
        }


        private void SetVisibility(Control c, bool visible)
        {
            if (c.InvokeRequired)
            {
                SetVisibilityCallback cb = new SetVisibilityCallback(SetVisibility);
                this.Invoke(cb, new object[] { c, visible });
            }
            else
            {
                c.Visible = visible;
            }
        }

        private void SetText(Control c, string txt)
        {
            if (c.InvokeRequired)
            {
                SetTextCallback cb = new SetTextCallback(SetText);
                this.Invoke(cb, new object[] { c, txt });
            }
            else
            {
                c.Text = txt;
            }
        }


        // Description
        private void txtDescription_TextChanged(object sender, EventArgs e)
        {
            this.InputDescription = txtDescription.Text;
        }


        // Comment
        private void txtComment_TextChanged(object sender, EventArgs e)
        {
            this.InputComment = txtComment.Text;
        }

        // Type
        private void comboBoxType_SelectedIndexChanged(object sender, EventArgs e) {
            this.InputType = comboBoxType.Text;
        }


        // Due
        private void dateTimePickerDue_ValueChanged(object sender, EventArgs e)
        {
            this.InputDue = dateTimePickerDue.Value;
        }

        // Priority
        private void comboBoxPriority_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.InputPriority = comboBoxPriority.Text;
        }

        // Status
        private void comboBoxStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.InputStatus = comboBoxStatus.Text;
        }

        private void lblComment_Click(object sender, EventArgs e)
        {

        }

        public void resetInputForm()
        {
            txtDescription.ResetText();
            txtComment.ResetText();
            comboBoxType.ResetText();
            dateTimePickerDue.ResetText();
            comboBoxPriority.ResetText();
            comboBoxStatus.ResetText();         
        }

       
    }
}
