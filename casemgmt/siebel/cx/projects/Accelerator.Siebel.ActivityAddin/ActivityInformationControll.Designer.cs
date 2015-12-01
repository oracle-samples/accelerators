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
 *  date: Mon Nov 30 20:14:25 PST 2015

 *  revision: rnw-15-11-fixes-release-2
 *  SHA1: $Id: 42b456c9bc12b8d7fd7fff331278f063be9cc998 $
 * *********************************************************************************************
 *  File: ActivityInformationControll.Designer.cs
 * *********************************************************************************************/

namespace Accelerator.Siebel.ActivityAddin
{
    partial class ActivityInformationControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;


        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gbActivity = new System.Windows.Forms.GroupBox();
            this.comboBoxStatus = new System.Windows.Forms.ComboBox();
            this.comboBoxPriority = new System.Windows.Forms.ComboBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblPriority = new System.Windows.Forms.Label();
            this.dateTimePickerDue = new System.Windows.Forms.DateTimePicker();
            this.lblDue = new System.Windows.Forms.Label();
            this.comboBoxType = new System.Windows.Forms.ComboBox();
            this.lblType = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.txtComment = new System.Windows.Forms.TextBox();
            this.lblComment = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.gbActivity.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbActivity
            // 
            this.gbActivity.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbActivity.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gbActivity.Controls.Add(this.comboBoxStatus);
            this.gbActivity.Controls.Add(this.comboBoxPriority);
            this.gbActivity.Controls.Add(this.lblStatus);
            this.gbActivity.Controls.Add(this.lblPriority);
            this.gbActivity.Controls.Add(this.dateTimePickerDue);
            this.gbActivity.Controls.Add(this.lblDue);
            this.gbActivity.Controls.Add(this.comboBoxType);
            this.gbActivity.Controls.Add(this.lblType);
            this.gbActivity.Controls.Add(this.txtDescription);
            this.gbActivity.Controls.Add(this.txtComment);
            this.gbActivity.Controls.Add(this.lblComment);
            this.gbActivity.Controls.Add(this.lblDescription);
            this.gbActivity.Location = new System.Drawing.Point(4, 4);
            this.gbActivity.Margin = new System.Windows.Forms.Padding(4);
            this.gbActivity.Name = "gbActivity";
            this.gbActivity.Padding = new System.Windows.Forms.Padding(4);
            this.gbActivity.Size = new System.Drawing.Size(750, 405);
            this.gbActivity.TabIndex = 1;
            this.gbActivity.TabStop = false;
            this.gbActivity.Text = "Activity";
            // 
            // comboBoxStatus
            // 
            this.comboBoxStatus.FormattingEnabled = true;
            this.comboBoxStatus.Items.AddRange(new object[] {
            "Open",
            "Closed"});
            this.comboBoxStatus.Location = new System.Drawing.Point(474, 74);
            this.comboBoxStatus.Name = "comboBoxStatus";
            this.comboBoxStatus.Size = new System.Drawing.Size(232, 24);
            this.comboBoxStatus.TabIndex = 25;
            this.comboBoxStatus.SelectedIndexChanged += new System.EventHandler(this.comboBoxStatus_SelectedIndexChanged);
            // 
            // comboBoxPriority
            // 
            this.comboBoxPriority.FormattingEnabled = true;
            this.comboBoxPriority.Items.AddRange(new object[] {
            "1-ASAP",
            "2-High",
            "3-Medium",
            "4-Low"});
            this.comboBoxPriority.Location = new System.Drawing.Point(126, 74);
            this.comboBoxPriority.Name = "comboBoxPriority";
            this.comboBoxPriority.Size = new System.Drawing.Size(184, 24);
            this.comboBoxPriority.TabIndex = 24;
            this.comboBoxPriority.SelectedIndexChanged += new System.EventHandler(this.comboBoxPriority_SelectedIndexChanged);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(402, 74);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(48, 17);
            this.lblStatus.TabIndex = 23;
            this.lblStatus.Text = "Status";
            // 
            // lblPriority
            // 
            this.lblPriority.AutoSize = true;
            this.lblPriority.Location = new System.Drawing.Point(68, 74);
            this.lblPriority.Name = "lblPriority";
            this.lblPriority.Size = new System.Drawing.Size(52, 17);
            this.lblPriority.TabIndex = 22;
            this.lblPriority.Text = "Priority";
            // 
            // dateTimePickerDue
            // 
            this.dateTimePickerDue.CustomFormat = "MM/dd/yyyy hh:mm:ss tt";
            this.dateTimePickerDue.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerDue.Location = new System.Drawing.Point(474, 37);
            this.dateTimePickerDue.Name = "dateTimePickerDue";
            this.dateTimePickerDue.Size = new System.Drawing.Size(232, 22);
            this.dateTimePickerDue.TabIndex = 20;
            this.dateTimePickerDue.ValueChanged += new System.EventHandler(this.dateTimePickerDue_ValueChanged);
            // 
            // lblDue
            // 
            this.lblDue.AutoSize = true;
            this.lblDue.Location = new System.Drawing.Point(416, 37);
            this.lblDue.Name = "lblDue";
            this.lblDue.Size = new System.Drawing.Size(34, 17);
            this.lblDue.TabIndex = 19;
            this.lblDue.Text = "Due";
            // 
            // comboBoxType
            // 
            this.comboBoxType.FormattingEnabled = true;
            this.comboBoxType.Items.AddRange(new object[] {
            "Depot Repair",
            "Appointment"});
            this.comboBoxType.Location = new System.Drawing.Point(127, 34);
            this.comboBoxType.Name = "comboBoxType";
            this.comboBoxType.Size = new System.Drawing.Size(183, 24);
            this.comboBoxType.TabIndex = 18;
            this.comboBoxType.SelectedIndexChanged += new System.EventHandler(this.comboBoxType_SelectedIndexChanged);
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(80, 34);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(40, 17);
            this.lblType.TabIndex = 17;
            this.lblType.Text = "Type";
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(126, 127);
            this.txtDescription.MaxLength = 100;
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(580, 44);
            this.txtDescription.TabIndex = 15;
            this.txtDescription.TextChanged += new System.EventHandler(this.txtDescription_TextChanged);
            // 
            // txtComment
            // 
            this.txtComment.Location = new System.Drawing.Point(127, 195);
            this.txtComment.MaxLength = 1000;
            this.txtComment.Multiline = true;
            this.txtComment.Name = "txtComment";
            this.txtComment.Size = new System.Drawing.Size(579, 156);
            this.txtComment.TabIndex = 14;
            this.txtComment.TextChanged += new System.EventHandler(this.txtComment_TextChanged);
            // 
            // lblComment
            // 
            this.lblComment.AutoSize = true;
            this.lblComment.Location = new System.Drawing.Point(54, 195);
            this.lblComment.Name = "lblComment";
            this.lblComment.Size = new System.Drawing.Size(67, 17);
            this.lblComment.TabIndex = 13;
            this.lblComment.Text = "Comment";
            this.lblComment.Click += new System.EventHandler(this.lblComment_Click);
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(41, 127);
            this.lblDescription.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(79, 17);
            this.lblDescription.TabIndex = 3;
            this.lblDescription.Text = "Description";
            // 
            // ActivityInformationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.gbActivity);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ActivityInformationControl";
            this.Size = new System.Drawing.Size(766, 421);
            this.gbActivity.ResumeLayout(false);
            this.gbActivity.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbActivity;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.TextBox txtComment;
        private System.Windows.Forms.Label lblComment;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.ComboBox comboBoxType;
        private System.Windows.Forms.Label lblDue;
        private System.Windows.Forms.DateTimePicker dateTimePickerDue;
        private System.Windows.Forms.ComboBox comboBoxStatus;
        private System.Windows.Forms.ComboBox comboBoxPriority;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblPriority;
    }
}
