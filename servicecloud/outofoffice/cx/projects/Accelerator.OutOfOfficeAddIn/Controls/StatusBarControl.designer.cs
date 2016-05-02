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
 *  SHA1: $Id: c12c53b09ee9478ac27e7c6a3ab3cd4e92a70029 $
 * *********************************************************************************************
 *  File: StatusBarControl.designer.cs
 * *********************************************************************************************/

using Accelerator.OutOfOffice.Client.Model;
using Accelerator.OutOfOffice.Client.RightNow;

namespace Accelerator.OutOfOffice
{
    partial class StatusBarControl
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
        private void InitializeComponent(StaffAccount staffAccount)
        {
            //this.components = new System.ComponentModel.Container();
            this.statusButton = new System.Windows.Forms.Button();
            //this.OutOfOfficeToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();

            // 
            // statusButton
            // 
            this.statusButton.BackColor = System.Drawing.Color.White;
            this.statusButton.Location = new System.Drawing.Point(0, -1);
            this.statusButton.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.statusButton.Name = "statusButton";
            this.statusButton.Size = new System.Drawing.Size(162, 23);
            this.statusButton.TabIndex = 0;
            if (staffAccount.OooFlag)
            {
                this.statusButton.Text = "Out of Office" ;
                this.statusButton.ForeColor = System.Drawing.Color.DarkRed;
            }
            else
            {
                this.statusButton.Text = "Available";
                this.statusButton.ForeColor = System.Drawing.Color.DarkGreen;
            }
            this.statusButton.UseVisualStyleBackColor = false;
            this.statusButton.Click += new System.EventHandler(this.statusButton_Click);

            // 
            // StatusBarControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.statusButton);
            this.Name = "StatusBarControl";
            this.Size = new System.Drawing.Size(162, 28);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Button statusButton;
        //public System.Windows.Forms.ToolTip OutOfOfficeToolTip;
    }
}
