/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + EBS Enhancement
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  EBS release: 12.1.3
 *  reference: 150505-000099, 150420-000127
 *  date: Thu Nov 12 00:52:47 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: 3d877834aa16326bc2fc184115c63fe82f896cf8 $
 * *********************************************************************************************
 *  File: IconLabelControl.Designer.cs
 * *********************************************************************************************/
namespace Accelerator.EBS.SharedServices
{
    partial class IconLabelControl
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
            this.components = new System.ComponentModel.Container();
            this.statusColorBar = new System.Windows.Forms.Button();
            this.statusText = new System.Windows.Forms.Label();
            this.logHistoryToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // statusColorBar
            // 
            this.statusColorBar.BackColor = System.Drawing.Color.Black;
            this.statusColorBar.Location = new System.Drawing.Point(0, -1);
            this.statusColorBar.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.statusColorBar.Name = "statusColorBar";
            this.statusColorBar.Size = new System.Drawing.Size(10, 23);
            this.statusColorBar.TabIndex = 0;
            this.statusColorBar.UseVisualStyleBackColor = false;
            this.statusColorBar.Click += new System.EventHandler(this.button1_Click);
            // 
            // statusText
            // 
            this.statusText.AutoSize = true;
            this.statusText.ForeColor = System.Drawing.Color.Gray;
            this.statusText.Location = new System.Drawing.Point(13, 4);
            this.statusText.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.statusText.Name = "statusText";
            this.statusText.Size = new System.Drawing.Size(88, 13);
            this.statusText.TabIndex = 1;
            this.statusText.Text = "Integration Status";
            this.statusText.Click += new System.EventHandler(this.button1_Click);
            // 
            // logHistoryToolTip
            // 
            this.logHistoryToolTip.AutoPopDelay = 5000;
            this.logHistoryToolTip.InitialDelay = 1000;
            this.logHistoryToolTip.ReshowDelay = 500;
            this.logHistoryToolTip.ShowAlways = true;
            // 
            // IconLabelControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.statusText);
            this.Controls.Add(this.statusColorBar);
            this.Name = "IconLabelControl";
            this.Size = new System.Drawing.Size(565, 28);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Button statusColorBar;
        public System.Windows.Forms.Label statusText;
        public System.Windows.Forms.ToolTip logHistoryToolTip;
    }
}
