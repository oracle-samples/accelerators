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
 *  date: Mon Nov 30 20:14:28 PST 2015

 *  revision: rnw-15-11-fixes-release-2
 *  SHA1: $Id: bcf07b82f6fbbbe0d613525db7cbbe15e3cd6864 $
 * *********************************************************************************************
 *  File: ServiceRequestInformationControl.Designer.cs
 * *********************************************************************************************/

namespace Accelerator.Siebel.ServiceRequestAddin
{
    partial class ServiceRequestInformationControl
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
            this.gbServReqInfo = new System.Windows.Forms.GroupBox();
            this.lblSeverity = new System.Windows.Forms.Label();
            this.lblRequestType = new System.Windows.Forms.Label();
            this.lblOwnerInfoFieldName = new System.Windows.Forms.Label();
            this.lblRequestTypeFieldName = new System.Windows.Forms.Label();
            this.lblSRNoFieldName = new System.Windows.Forms.Label();
            this.txtSummary = new System.Windows.Forms.TextBox();
            this.lblSummaryFieldName = new System.Windows.Forms.Label();
            this.lblOwnerInfo = new System.Windows.Forms.Label();
            this.lblSeverityFieldName = new System.Windows.Forms.Label();
            this.lblSRNo = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblStatusFieldName = new System.Windows.Forms.Label();
            this.lblProdDescription = new System.Windows.Forms.Label();
            this.lblProdDescriptionFieldName = new System.Windows.Forms.Label();
            this.lblProduct = new System.Windows.Forms.Label();
            this.lblProductFieldName = new System.Windows.Forms.Label();
            this.lblSerialNoFieldName = new System.Windows.Forms.Label();
            this.tbSerialNo = new System.Windows.Forms.TextBox();
            this.lblValid = new System.Windows.Forms.Label();
            this.lblInvalid = new System.Windows.Forms.Label();
            this.lblSerialNo = new System.Windows.Forms.Label();
            this.gbServReqInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbServReqInfo
            // 
            this.gbServReqInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbServReqInfo.Controls.Add(this.lblSeverity);
            this.gbServReqInfo.Controls.Add(this.lblRequestType);
            this.gbServReqInfo.Controls.Add(this.lblOwnerInfoFieldName);
            this.gbServReqInfo.Controls.Add(this.lblRequestTypeFieldName);
            this.gbServReqInfo.Controls.Add(this.lblSRNoFieldName);
            this.gbServReqInfo.Controls.Add(this.txtSummary);
            this.gbServReqInfo.Controls.Add(this.lblSummaryFieldName);
            this.gbServReqInfo.Controls.Add(this.lblOwnerInfo);
            this.gbServReqInfo.Controls.Add(this.lblSeverityFieldName);
            this.gbServReqInfo.Controls.Add(this.lblSRNo);
            this.gbServReqInfo.Controls.Add(this.lblStatus);
            this.gbServReqInfo.Controls.Add(this.lblStatusFieldName);
            this.gbServReqInfo.Controls.Add(this.lblProdDescription);
            this.gbServReqInfo.Controls.Add(this.lblProdDescriptionFieldName);
            this.gbServReqInfo.Controls.Add(this.lblProduct);
            this.gbServReqInfo.Controls.Add(this.lblProductFieldName);
            this.gbServReqInfo.Controls.Add(this.lblSerialNoFieldName);
            this.gbServReqInfo.Controls.Add(this.tbSerialNo);
            this.gbServReqInfo.Controls.Add(this.lblValid);
            this.gbServReqInfo.Controls.Add(this.lblInvalid);
            this.gbServReqInfo.Controls.Add(this.lblSerialNo);
            this.gbServReqInfo.Location = new System.Drawing.Point(3, 3);
            this.gbServReqInfo.Name = "gbServReqInfo";
            this.gbServReqInfo.Size = new System.Drawing.Size(659, 321);
            this.gbServReqInfo.TabIndex = 1;
            this.gbServReqInfo.TabStop = false;
            this.gbServReqInfo.Text = "Service Request Info";
            // 
            // lblSeverity
            // 
            this.lblSeverity.AutoSize = true;
            this.lblSeverity.Location = new System.Drawing.Point(356, 26);
            this.lblSeverity.Name = "lblSeverity";
            this.lblSeverity.Size = new System.Drawing.Size(66, 13);
            this.lblSeverity.TabIndex = 2;
            this.lblSeverity.Text = "[SEVERITY]";
            // 
            // lblRequestType
            // 
            this.lblRequestType.AutoSize = true;
            this.lblRequestType.Location = new System.Drawing.Point(97, 28);
            this.lblRequestType.Name = "lblRequestType";
            this.lblRequestType.Size = new System.Drawing.Size(96, 13);
            this.lblRequestType.TabIndex = 2;
            this.lblRequestType.Text = "[REQUEST TYPE]";
            // 
            // lblOwnerInfoFieldName
            // 
            this.lblOwnerInfoFieldName.AutoSize = true;
            this.lblOwnerInfoFieldName.Location = new System.Drawing.Point(309, 69);
            this.lblOwnerInfoFieldName.Name = "lblOwnerInfoFieldName";
            this.lblOwnerInfoFieldName.Size = new System.Drawing.Size(41, 13);
            this.lblOwnerInfoFieldName.TabIndex = 5;
            this.lblOwnerInfoFieldName.Text = "Owner:";
            // 
            // lblRequestTypeFieldName
            // 
            this.lblRequestTypeFieldName.AutoSize = true;
            this.lblRequestTypeFieldName.Location = new System.Drawing.Point(13, 26);
            this.lblRequestTypeFieldName.Name = "lblRequestTypeFieldName";
            this.lblRequestTypeFieldName.Size = new System.Drawing.Size(77, 13);
            this.lblRequestTypeFieldName.TabIndex = 5;
            this.lblRequestTypeFieldName.Text = "Request Type:";
            // 
            // lblSRNoFieldName
            // 
            this.lblSRNoFieldName.AutoSize = true;
            this.lblSRNoFieldName.Location = new System.Drawing.Point(54, 47);
            this.lblSRNoFieldName.Name = "lblSRNoFieldName";
            this.lblSRNoFieldName.Size = new System.Drawing.Size(35, 13);
            this.lblSRNoFieldName.TabIndex = 5;
            this.lblSRNoFieldName.Text = "SR #:";
            // 
            // txtSummary
            // 
            this.txtSummary.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSummary.Location = new System.Drawing.Point(93, 145);
            this.txtSummary.Multiline = true;
            this.txtSummary.Name = "txtSummary";
            this.txtSummary.ReadOnly = true;
            this.txtSummary.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSummary.Size = new System.Drawing.Size(554, 146);
            this.txtSummary.TabIndex = 4;
            this.txtSummary.Text = "[Summary Info goes here]";
            // 
            // lblSummaryFieldName
            // 
            this.lblSummaryFieldName.AutoSize = true;
            this.lblSummaryFieldName.Location = new System.Drawing.Point(40, 145);
            this.lblSummaryFieldName.Name = "lblSummaryFieldName";
            this.lblSummaryFieldName.Size = new System.Drawing.Size(53, 13);
            this.lblSummaryFieldName.TabIndex = 3;
            this.lblSummaryFieldName.Text = "Summary:";
            // 
            // lblOwnerInfo
            // 
            this.lblOwnerInfo.AutoSize = true;
            this.lblOwnerInfo.Location = new System.Drawing.Point(356, 69);
            this.lblOwnerInfo.Name = "lblOwnerInfo";
            this.lblOwnerInfo.Size = new System.Drawing.Size(83, 13);
            this.lblOwnerInfo.TabIndex = 2;
            this.lblOwnerInfo.Text = "[OWNER INFO]";
            // 
            // lblSeverityFieldName
            // 
            this.lblSeverityFieldName.AutoSize = true;
            this.lblSeverityFieldName.Location = new System.Drawing.Point(302, 26);
            this.lblSeverityFieldName.Name = "lblSeverityFieldName";
            this.lblSeverityFieldName.Size = new System.Drawing.Size(48, 13);
            this.lblSeverityFieldName.TabIndex = 1;
            this.lblSeverityFieldName.Text = "Severity:";
            // 
            // lblSRNo
            // 
            this.lblSRNo.AutoSize = true;
            this.lblSRNo.Location = new System.Drawing.Point(95, 47);
            this.lblSRNo.Name = "lblSRNo";
            this.lblSRNo.Size = new System.Drawing.Size(38, 13);
            this.lblSRNo.TabIndex = 2;
            this.lblSRNo.Text = "[SR #]";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(356, 47);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(56, 13);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "[STATUS]";
            // 
            // lblStatusFieldName
            // 
            this.lblStatusFieldName.AutoSize = true;
            this.lblStatusFieldName.Location = new System.Drawing.Point(310, 47);
            this.lblStatusFieldName.Name = "lblStatusFieldName";
            this.lblStatusFieldName.Size = new System.Drawing.Size(40, 13);
            this.lblStatusFieldName.TabIndex = 1;
            this.lblStatusFieldName.Text = "Status:";
            // 
            // lblProdDescription
            // 
            this.lblProdDescription.AutoSize = true;
            this.lblProdDescription.Location = new System.Drawing.Point(356, 92);
            this.lblProdDescription.Name = "lblProdDescription";
            this.lblProdDescription.Size = new System.Drawing.Size(142, 13);
            this.lblProdDescription.TabIndex = 12;
            this.lblProdDescription.Text = "[PRODUCT DESCRIPTION]";
            // 
            // lblProdDescriptionFieldName
            // 
            this.lblProdDescriptionFieldName.AutoSize = true;
            this.lblProdDescriptionFieldName.Location = new System.Drawing.Point(302, 92);
            this.lblProdDescriptionFieldName.Name = "lblProdDescriptionFieldName";
            this.lblProdDescriptionFieldName.Size = new System.Drawing.Size(47, 13);
            this.lblProdDescriptionFieldName.TabIndex = 11;
            this.lblProdDescriptionFieldName.Text = "Product:";
            // 
            // lblProduct
            // 
            this.lblProduct.AutoSize = true;
            this.lblProduct.Location = new System.Drawing.Point(95, 92);
            this.lblProduct.Name = "lblProduct";
            this.lblProduct.Size = new System.Drawing.Size(66, 13);
            this.lblProduct.TabIndex = 10;
            this.lblProduct.Text = "[PRODUCT]";
            // 
            // lblProductFieldName
            // 
            this.lblProductFieldName.AutoSize = true;
            this.lblProductFieldName.Location = new System.Drawing.Point(28, 92);
            this.lblProductFieldName.Name = "lblProductFieldName";
            this.lblProductFieldName.Size = new System.Drawing.Size(61, 13);
            this.lblProductFieldName.TabIndex = 9;
            this.lblProductFieldName.Text = "Product ID:";
            // 
            // lblSerialNoFieldName
            // 
            this.lblSerialNoFieldName.AutoSize = true;
            this.lblSerialNoFieldName.Location = new System.Drawing.Point(43, 69);
            this.lblSerialNoFieldName.Name = "lblSerialNoFieldName";
            this.lblSerialNoFieldName.Size = new System.Drawing.Size(46, 13);
            this.lblSerialNoFieldName.TabIndex = 5;
            this.lblSerialNoFieldName.Text = "Serial #:";
            // 
            // tbSerialNo
            // 
            this.tbSerialNo.Location = new System.Drawing.Point(93, 66);
            this.tbSerialNo.Name = "tbSerialNo";
            this.tbSerialNo.Size = new System.Drawing.Size(100, 20);
            this.tbSerialNo.TabIndex = 6;
            this.tbSerialNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbSerialNo_EnterKeyPress);
            this.tbSerialNo.LostFocus += new System.EventHandler(this.tbSerialNo_LostFocus);
            // 
            // lblValid
            // 
            this.lblValid.AutoSize = true;
            this.lblValid.BackColor = System.Drawing.Color.Transparent;
            this.lblValid.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.lblValid.Location = new System.Drawing.Point(199, 69);
            this.lblValid.Name = "lblValid";
            this.lblValid.Size = new System.Drawing.Size(33, 13);
            this.lblValid.TabIndex = 7;
            this.lblValid.Text = "Valid!";
            this.lblValid.Visible = false;
            // 
            // lblInvalid
            // 
            this.lblInvalid.AutoSize = true;
            this.lblInvalid.BackColor = System.Drawing.Color.Transparent;
            this.lblInvalid.ForeColor = System.Drawing.Color.Red;
            this.lblInvalid.Location = new System.Drawing.Point(199, 69);
            this.lblInvalid.Name = "lblInvalid";
            this.lblInvalid.Size = new System.Drawing.Size(41, 13);
            this.lblInvalid.TabIndex = 8;
            this.lblInvalid.Text = "Invalid!";
            this.lblInvalid.Visible = false;
            // 
            // lblSerialNo
            // 
            this.lblSerialNo.AutoSize = true;
            this.lblSerialNo.Location = new System.Drawing.Point(95, 69);
            this.lblSerialNo.Name = "lblSerialNo";
            this.lblSerialNo.Size = new System.Drawing.Size(61, 13);
            this.lblSerialNo.TabIndex = 2;
            this.lblSerialNo.Text = "[SERIAL #]";
            this.lblSerialNo.Visible = false;
            // 
            // ServiceRequestInformationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbServReqInfo);
            this.Name = "ServiceRequestInformationControl";
            this.Size = new System.Drawing.Size(688, 346);
            this.gbServReqInfo.ResumeLayout(false);
            this.gbServReqInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbServReqInfo;
        private System.Windows.Forms.Label lblSeverity;
        private System.Windows.Forms.Label lblRequestType;
        private System.Windows.Forms.Label lblOwnerInfoFieldName;
        private System.Windows.Forms.Label lblRequestTypeFieldName;
        private System.Windows.Forms.Label lblSRNoFieldName;
        private System.Windows.Forms.Label lblSerialNoFieldName;
        private System.Windows.Forms.TextBox txtSummary;
        private System.Windows.Forms.Label lblSummaryFieldName;
        private System.Windows.Forms.Label lblOwnerInfo;
        private System.Windows.Forms.Label lblSeverityFieldName;
        private System.Windows.Forms.Label lblSRNo;
        private System.Windows.Forms.Label lblSerialNo;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblStatusFieldName;
        public System.Windows.Forms.TextBox tbSerialNo;
        private System.Windows.Forms.Label lblInvalid;
        private System.Windows.Forms.Label lblValid;
        private System.Windows.Forms.Label lblProdDescription;
        private System.Windows.Forms.Label lblProdDescriptionFieldName;
        private System.Windows.Forms.Label lblProduct;
        private System.Windows.Forms.Label lblProductFieldName;
    }
}
