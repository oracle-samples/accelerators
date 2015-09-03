/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC Contact Center + Siebel Case Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.5 (May 2015)
 *  Siebel release: 8.1.1.15
 *  reference: 141216-000121
 *  date: Wed Sep  2 23:14:38 PDT 2015

 *  revision: rnw-15-8-fixes-release-01
 *  SHA1: $Id: 32a069ea6a9dc0430e870afea40649ec3fa9d7bc $
 * *********************************************************************************************
 *  File: RepairOrderInformationControl.Designer.cs
 * *********************************************************************************************/

namespace Accelerator.Siebel.RepairOrderAddin
{
    partial class RepairOrderInformationControl
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
            this.gbRepairOrder = new System.Windows.Forms.GroupBox();
            this.lblRepairOrderStatus = new System.Windows.Forms.Label();
            this.cbRepairOrderStatus = new System.Windows.Forms.ComboBox();
            this.lblRepairOrderStatusFieldName = new System.Windows.Forms.Label();
            this.editROButton = new System.Windows.Forms.Button();
            this.lblQuantityFieldName = new System.Windows.Forms.Label();
            this.lblApprovalRequiredFieldName = new System.Windows.Forms.Label();
            this.lblProblemInputFieldName = new System.Windows.Forms.Label();
            this.lblUnitFieldName = new System.Windows.Forms.Label();
            this.lblRepairNo = new System.Windows.Forms.Label();
            this.lblRepairNoFieldName = new System.Windows.Forms.Label();
            this.lblQuantity = new System.Windows.Forms.Label();
            this.lblApprovalRequired = new System.Windows.Forms.Label();
            this.lblUnit = new System.Windows.Forms.Label();
            this.cbApprovalRequired = new System.Windows.Forms.ComboBox();
            this.tbQuantity = new System.Windows.Forms.TextBox();
            this.cbUnit = new System.Windows.Forms.ComboBox();
            this.textProblemInput = new System.Windows.Forms.TextBox();
            this.textProblem = new System.Windows.Forms.TextBox();
            this.addROButton = new System.Windows.Forms.Button();
            this.repairOrderListView = new System.Windows.Forms.ListView();
            this.repairOrderNum = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.repairOrderType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.repairOrderStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.repairOrderProblem = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.repairOrderRowAction = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label8 = new System.Windows.Forms.Label();
            this.lblProdDescription = new System.Windows.Forms.Label();
            this.lblProdDescriptionFieldName = new System.Windows.Forms.Label();
            this.lblProduct = new System.Windows.Forms.Label();
            this.lblProductFieldName = new System.Windows.Forms.Label();
            this.lblSerialNoFieldName = new System.Windows.Forms.Label();
            this.tbSerialNo = new System.Windows.Forms.TextBox();
            this.lblValid = new System.Windows.Forms.Label();
            this.lblInvalid = new System.Windows.Forms.Label();
            this.lblSerialNo = new System.Windows.Forms.Label();
            this.gbRepairOrder.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbRepairOrder
            // 
            this.gbRepairOrder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbRepairOrder.Controls.Add(this.lblSerialNo);
            this.gbRepairOrder.Controls.Add(this.lblProdDescription);
            this.gbRepairOrder.Controls.Add(this.lblProdDescriptionFieldName);
            this.gbRepairOrder.Controls.Add(this.lblProduct);
            this.gbRepairOrder.Controls.Add(this.lblProductFieldName);
            this.gbRepairOrder.Controls.Add(this.lblSerialNoFieldName);
            this.gbRepairOrder.Controls.Add(this.tbSerialNo);
            this.gbRepairOrder.Controls.Add(this.lblValid);
            this.gbRepairOrder.Controls.Add(this.lblInvalid);
            this.gbRepairOrder.Controls.Add(this.lblRepairOrderStatus);
            this.gbRepairOrder.Controls.Add(this.cbRepairOrderStatus);
            this.gbRepairOrder.Controls.Add(this.lblRepairOrderStatusFieldName);
            this.gbRepairOrder.Controls.Add(this.editROButton);
            this.gbRepairOrder.Controls.Add(this.lblQuantityFieldName);
            this.gbRepairOrder.Controls.Add(this.lblApprovalRequiredFieldName);
            this.gbRepairOrder.Controls.Add(this.lblProblemInputFieldName);
            this.gbRepairOrder.Controls.Add(this.lblUnitFieldName);
            this.gbRepairOrder.Controls.Add(this.lblRepairNo);
            this.gbRepairOrder.Controls.Add(this.lblRepairNoFieldName);
            this.gbRepairOrder.Controls.Add(this.lblQuantity);
            this.gbRepairOrder.Controls.Add(this.lblApprovalRequired);
            this.gbRepairOrder.Controls.Add(this.lblUnit);
            this.gbRepairOrder.Controls.Add(this.cbApprovalRequired);
            this.gbRepairOrder.Controls.Add(this.tbQuantity);
            this.gbRepairOrder.Controls.Add(this.cbUnit);
            this.gbRepairOrder.Controls.Add(this.textProblemInput);
            this.gbRepairOrder.Controls.Add(this.textProblem);
            this.gbRepairOrder.Location = new System.Drawing.Point(3, 197);
            this.gbRepairOrder.Name = "gbRepairOrder";
            this.gbRepairOrder.Size = new System.Drawing.Size(707, 336);
            this.gbRepairOrder.TabIndex = 2;
            this.gbRepairOrder.TabStop = false;
            this.gbRepairOrder.Text = "Repair Order Info";
            // 
            // lblRepairOrderStatus
            // 
            this.lblRepairOrderStatus.AutoSize = true;
            this.lblRepairOrderStatus.Location = new System.Drawing.Point(444, 31);
            this.lblRepairOrderStatus.Name = "lblRepairOrderStatus";
            this.lblRepairOrderStatus.Size = new System.Drawing.Size(141, 13);
            this.lblRepairOrderStatus.TabIndex = 25;
            this.lblRepairOrderStatus.Text = "[REPAIR ORDER STATUS]";
            // 
            // cbRepairOrderStatus
            // 
            this.cbRepairOrderStatus.FormattingEnabled = true;
            this.cbRepairOrderStatus.Items.AddRange(new object[] {
            "Open",
            "Hold",
            "Close",
            "Item Received",
            "Item Shipped",
            "Awaiting Estimate Approval",
            "Awaiting QA",
            "Awaiting Repair",
            "Awaiting Shipping",
            "Estimate Approved",
            "Estimate Rejected",
            "Exchange and Refurbish",
            "Repair Complete",
            "Repair In Progress",
            "Awaiting Receipt"});
            this.cbRepairOrderStatus.Location = new System.Drawing.Point(447, 28);
            this.cbRepairOrderStatus.Name = "cbRepairOrderStatus";
            this.cbRepairOrderStatus.Size = new System.Drawing.Size(146, 21);
            this.cbRepairOrderStatus.TabIndex = 24;
            this.cbRepairOrderStatus.SelectedIndexChanged += new System.EventHandler(this.cbRepairOrderStatus_SelectedIndexChanged);
            // 
            // lblRepairOrderStatusFieldName
            // 
            this.lblRepairOrderStatusFieldName.AutoSize = true;
            this.lblRepairOrderStatusFieldName.Location = new System.Drawing.Point(390, 31);
            this.lblRepairOrderStatusFieldName.Name = "lblRepairOrderStatusFieldName";
            this.lblRepairOrderStatusFieldName.Size = new System.Drawing.Size(40, 13);
            this.lblRepairOrderStatusFieldName.TabIndex = 23;
            this.lblRepairOrderStatusFieldName.Text = "Status:";
            // 
            // editROButton
            // 
            this.editROButton.Location = new System.Drawing.Point(502, 296);
            this.editROButton.Name = "editROButton";
            this.editROButton.Size = new System.Drawing.Size(135, 25);
            this.editROButton.TabIndex = 22;
            this.editROButton.Text = "Edit Repair Order";
            this.editROButton.UseVisualStyleBackColor = true;
            this.editROButton.Click += new System.EventHandler(this.editROButton_Click);
            // 
            // lblQuantityFieldName
            // 
            this.lblQuantityFieldName.Location = new System.Drawing.Point(16, 59);
            this.lblQuantityFieldName.Name = "lblQuantityFieldName";
            this.lblQuantityFieldName.Size = new System.Drawing.Size(93, 13);
            this.lblQuantityFieldName.TabIndex = 6;
            this.lblQuantityFieldName.Text = "Quantity:";
            this.lblQuantityFieldName.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblApprovalRequiredFieldName
            // 
            this.lblApprovalRequiredFieldName.AutoSize = true;
            this.lblApprovalRequiredFieldName.Location = new System.Drawing.Point(332, 84);
            this.lblApprovalRequiredFieldName.Name = "lblApprovalRequiredFieldName";
            this.lblApprovalRequiredFieldName.Size = new System.Drawing.Size(98, 13);
            this.lblApprovalRequiredFieldName.TabIndex = 5;
            this.lblApprovalRequiredFieldName.Text = "Approval Required:";
            // 
            // lblProblemInputFieldName
            // 
            this.lblProblemInputFieldName.AutoSize = true;
            this.lblProblemInputFieldName.Location = new System.Drawing.Point(5, 171);
            this.lblProblemInputFieldName.Name = "lblProblemInputFieldName";
            this.lblProblemInputFieldName.Size = new System.Drawing.Size(104, 13);
            this.lblProblemInputFieldName.TabIndex = 5;
            this.lblProblemInputFieldName.Text = "Problem Description:";
            // 
            // lblUnitFieldName
            // 
            this.lblUnitFieldName.AutoSize = true;
            this.lblUnitFieldName.Location = new System.Drawing.Point(345, 58);
            this.lblUnitFieldName.Name = "lblUnitFieldName";
            this.lblUnitFieldName.Size = new System.Drawing.Size(85, 13);
            this.lblUnitFieldName.TabIndex = 5;
            this.lblUnitFieldName.Text = "Unit of Measure:";
            // 
            // lblRepairNo
            // 
            this.lblRepairNo.AutoSize = true;
            this.lblRepairNo.Location = new System.Drawing.Point(119, 32);
            this.lblRepairNo.Name = "lblRepairNo";
            this.lblRepairNo.Size = new System.Drawing.Size(63, 13);
            this.lblRepairNo.TabIndex = 2;
            this.lblRepairNo.Text = "[REPAIR #]";
            // 
            // lblRepairNoFieldName
            // 
            this.lblRepairNoFieldName.AutoSize = true;
            this.lblRepairNoFieldName.Location = new System.Drawing.Point(58, 32);
            this.lblRepairNoFieldName.Name = "lblRepairNoFieldName";
            this.lblRepairNoFieldName.Size = new System.Drawing.Size(51, 13);
            this.lblRepairNoFieldName.TabIndex = 1;
            this.lblRepairNoFieldName.Text = "Repair #:";
            // 
            // lblQuantity
            // 
            this.lblQuantity.AutoSize = true;
            this.lblQuantity.Location = new System.Drawing.Point(119, 59);
            this.lblQuantity.Name = "lblQuantity";
            this.lblQuantity.Size = new System.Drawing.Size(68, 13);
            this.lblQuantity.TabIndex = 2;
            this.lblQuantity.Text = "[QUANTITY]";
            // 
            // lblApprovalRequired
            // 
            this.lblApprovalRequired.AutoSize = true;
            this.lblApprovalRequired.Location = new System.Drawing.Point(444, 85);
            this.lblApprovalRequired.Name = "lblApprovalRequired";
            this.lblApprovalRequired.Size = new System.Drawing.Size(130, 13);
            this.lblApprovalRequired.TabIndex = 2;
            this.lblApprovalRequired.Text = "[APPROVAL REQUIRED]";
            // 
            // lblUnit
            // 
            this.lblUnit.AutoSize = true;
            this.lblUnit.Location = new System.Drawing.Point(444, 58);
            this.lblUnit.Name = "lblUnit";
            this.lblUnit.Size = new System.Drawing.Size(112, 13);
            this.lblUnit.TabIndex = 2;
            this.lblUnit.Text = "[UNIT OF MEASURE]";
            // 
            // cbApprovalRequired
            // 
            this.cbApprovalRequired.FormattingEnabled = true;
            this.cbApprovalRequired.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cbApprovalRequired.Location = new System.Drawing.Point(447, 82);
            this.cbApprovalRequired.Name = "cbApprovalRequired";
            this.cbApprovalRequired.Size = new System.Drawing.Size(146, 21);
            this.cbApprovalRequired.TabIndex = 15;
            this.cbApprovalRequired.SelectedIndexChanged += new System.EventHandler(this.cbApprovalRequired_SelectedIndexChanged);
            // 
            // tbQuantity
            // 
            this.tbQuantity.Location = new System.Drawing.Point(115, 56);
            this.tbQuantity.Name = "tbQuantity";
            this.tbQuantity.Size = new System.Drawing.Size(146, 20);
            this.tbQuantity.TabIndex = 18;
            this.tbQuantity.TextChanged += new System.EventHandler(this.tbQuantity_TextChanged);
            // 
            // cbUnit
            // 
            this.cbUnit.FormattingEnabled = true;
            this.cbUnit.Items.AddRange(new object[] {
            "Ea"});
            this.cbUnit.Location = new System.Drawing.Point(447, 55);
            this.cbUnit.Name = "cbUnit";
            this.cbUnit.Size = new System.Drawing.Size(146, 21);
            this.cbUnit.TabIndex = 21;
            this.cbUnit.SelectedIndexChanged += new System.EventHandler(this.cbUnit_SelectedIndexChanged);
            // 
            // textProblemInput
            // 
            this.textProblemInput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textProblemInput.Location = new System.Drawing.Point(122, 171);
            this.textProblemInput.Multiline = true;
            this.textProblemInput.Name = "textProblemInput";
            this.textProblemInput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textProblemInput.Size = new System.Drawing.Size(549, 119);
            this.textProblemInput.TabIndex = 20;
            this.textProblemInput.TextChanged += new System.EventHandler(this.textProblemInput_TextChanged);
            // 
            // textProblem
            // 
            this.textProblem.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textProblem.Location = new System.Drawing.Point(122, 171);
            this.textProblem.Multiline = true;
            this.textProblem.Name = "textProblem";
            this.textProblem.ReadOnly = true;
            this.textProblem.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textProblem.Size = new System.Drawing.Size(549, 119);
            this.textProblem.TabIndex = 14;
            this.textProblem.Text = "[Problem description goes here]";
            // 
            // addROButton
            // 
            this.addROButton.Location = new System.Drawing.Point(505, 157);
            this.addROButton.Name = "addROButton";
            this.addROButton.Size = new System.Drawing.Size(135, 23);
            this.addROButton.TabIndex = 25;
            this.addROButton.Text = "Add Repair Order";
            this.addROButton.UseVisualStyleBackColor = true;
            this.addROButton.Click += new System.EventHandler(this.addROButton_Click);
            // 
            // repairOrderListView
            // 
            this.repairOrderListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.repairOrderNum,
            this.repairOrderType,
            this.repairOrderStatus,
            this.repairOrderProblem,
            this.repairOrderRowAction});
            this.repairOrderListView.FullRowSelect = true;
            this.repairOrderListView.GridLines = true;
            this.repairOrderListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.repairOrderListView.Location = new System.Drawing.Point(14, 41);
            this.repairOrderListView.Name = "repairOrderListView";
            this.repairOrderListView.Size = new System.Drawing.Size(581, 100);
            this.repairOrderListView.TabIndex = 23;
            this.repairOrderListView.UseCompatibleStateImageBehavior = false;
            this.repairOrderListView.View = System.Windows.Forms.View.Details;
            this.repairOrderListView.SelectedIndexChanged += new System.EventHandler(this.repairOrderListView_SelectedIndexChanged);
            this.repairOrderListView.MouseLeave += new System.EventHandler(this.repairOrderListView_MouseLeave);
            this.repairOrderListView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.repairOrderListView_MouseMove);
            // 
            // repairOrderNum
            // 
            this.repairOrderNum.Text = "Repair Order #";
            this.repairOrderNum.Width = 83;
            // 
            // repairOrderType
            // 
            this.repairOrderType.Text = "Type";
            this.repairOrderType.Width = 92;
            // 
            // repairOrderStatus
            // 
            this.repairOrderStatus.Text = "Status";
            this.repairOrderStatus.Width = 115;
            // 
            // repairOrderProblem
            // 
            this.repairOrderProblem.Text = "Problem Description";
            this.repairOrderProblem.Width = 195;
            // 
            // repairOrderRowAction
            // 
            this.repairOrderRowAction.Text = "Action";
            this.repairOrderRowAction.Width = 70;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(11, 14);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(75, 13);
            this.label8.TabIndex = 24;
            this.label8.Text = "Repair Orders:";
            // 
            // lblProdDescription
            // 
            this.lblProdDescription.AutoSize = true;
            this.lblProdDescription.Location = new System.Drawing.Point(444, 111);
            this.lblProdDescription.Name = "lblProdDescription";
            this.lblProdDescription.Size = new System.Drawing.Size(142, 13);
            this.lblProdDescription.TabIndex = 33;
            this.lblProdDescription.Text = "[PRODUCT DESCRIPTION]";
            // 
            // lblProdDescriptionFieldName
            // 
            this.lblProdDescriptionFieldName.AutoSize = true;
            this.lblProdDescriptionFieldName.Location = new System.Drawing.Point(327, 111);
            this.lblProdDescriptionFieldName.Name = "lblProdDescriptionFieldName";
            this.lblProdDescriptionFieldName.Size = new System.Drawing.Size(103, 13);
            this.lblProdDescriptionFieldName.TabIndex = 32;
            this.lblProdDescriptionFieldName.Text = "Product Description:";
            // 
            // lblProduct
            // 
            this.lblProduct.AutoSize = true;
            this.lblProduct.Location = new System.Drawing.Point(121, 112);
            this.lblProduct.Name = "lblProduct";
            this.lblProduct.Size = new System.Drawing.Size(66, 13);
            this.lblProduct.TabIndex = 31;
            this.lblProduct.Text = "[PRODUCT]";
            // 
            // lblProductFieldName
            // 
            this.lblProductFieldName.AutoSize = true;
            this.lblProductFieldName.Location = new System.Drawing.Point(62, 112);
            this.lblProductFieldName.Name = "lblProductFieldName";
            this.lblProductFieldName.Size = new System.Drawing.Size(47, 13);
            this.lblProductFieldName.TabIndex = 30;
            this.lblProductFieldName.Text = "Product:";
            // 
            // lblSerialNoFieldName
            // 
            this.lblSerialNoFieldName.AutoSize = true;
            this.lblSerialNoFieldName.Location = new System.Drawing.Point(64, 85);
            this.lblSerialNoFieldName.Name = "lblSerialNoFieldName";
            this.lblSerialNoFieldName.Size = new System.Drawing.Size(46, 13);
            this.lblSerialNoFieldName.TabIndex = 26;
            this.lblSerialNoFieldName.Text = "Serial #:";
            // 
            // tbSerialNo
            // 
            this.tbSerialNo.Location = new System.Drawing.Point(116, 82);
            this.tbSerialNo.Name = "tbSerialNo";
            this.tbSerialNo.Size = new System.Drawing.Size(145, 20);
            this.tbSerialNo.TabIndex = 27;
            this.tbSerialNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbSerialNo_EnterKeyPress);
            this.tbSerialNo.LostFocus += new System.EventHandler(this.tbSerialNo_LostFocus);
            // lblValid
            // 
            this.lblValid.AutoSize = true;
            this.lblValid.BackColor = System.Drawing.Color.Transparent;
            this.lblValid.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.lblValid.Location = new System.Drawing.Point(268, 86);
            this.lblValid.Name = "lblValid";
            this.lblValid.Size = new System.Drawing.Size(33, 13);
            this.lblValid.TabIndex = 28;
            this.lblValid.Text = "Valid!";
            this.lblValid.Visible = false;
            // 
            // lblInvalid
            // 
            this.lblInvalid.AutoSize = true;
            this.lblInvalid.BackColor = System.Drawing.Color.Transparent;
            this.lblInvalid.ForeColor = System.Drawing.Color.Red;
            this.lblInvalid.Location = new System.Drawing.Point(268, 86);
            this.lblInvalid.Name = "lblInvalid";
            this.lblInvalid.Size = new System.Drawing.Size(41, 13);
            this.lblInvalid.TabIndex = 29;
            this.lblInvalid.Text = "Invalid!";
            this.lblInvalid.Visible = false;
            // 
            // lblSerialNo
            // 
            this.lblSerialNo.AutoSize = true;
            this.lblSerialNo.Location = new System.Drawing.Point(121, 86);
            this.lblSerialNo.Name = "lblSerialNo";
            this.lblSerialNo.Size = new System.Drawing.Size(61, 13);
            this.lblSerialNo.TabIndex = 34;
            this.lblSerialNo.Text = "[SERIAL #]";
            this.lblSerialNo.Visible = false;
            // 
            // RepairOrderInformationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.addROButton);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.gbRepairOrder);
            this.Controls.Add(this.repairOrderListView);
            this.Name = "RepairOrderInformationControl";
            this.Size = new System.Drawing.Size(728, 552);
            this.gbRepairOrder.ResumeLayout(false);
            this.gbRepairOrder.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbRepairOrder;
        private System.Windows.Forms.Button addROButton;
        private System.Windows.Forms.Button editROButton;
        private System.Windows.Forms.ListView repairOrderListView;
        private System.Windows.Forms.ColumnHeader repairOrderNum;
        private System.Windows.Forms.ColumnHeader repairOrderProblem;
        private System.Windows.Forms.ColumnHeader repairOrderRowAction;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblQuantityFieldName;
        private System.Windows.Forms.Label lblApprovalRequiredFieldName;
        private System.Windows.Forms.Label lblProblemInputFieldName;
        private System.Windows.Forms.Label lblUnitFieldName;
        private System.Windows.Forms.Label lblRepairNo;
        private System.Windows.Forms.Label lblRepairNoFieldName;
        private System.Windows.Forms.Label lblQuantity;
        private System.Windows.Forms.Label lblApprovalRequired;
        private System.Windows.Forms.Label lblUnit;
        private System.Windows.Forms.ComboBox cbApprovalRequired;
        private System.Windows.Forms.TextBox tbQuantity;
        private System.Windows.Forms.ComboBox cbUnit;
        private System.Windows.Forms.TextBox textProblemInput;
        private System.Windows.Forms.TextBox textProblem;
        private System.Windows.Forms.ColumnHeader repairOrderType;
        private System.Windows.Forms.ColumnHeader repairOrderStatus;
        private System.Windows.Forms.Label lblRepairOrderStatusFieldName;
        private System.Windows.Forms.Label lblRepairOrderStatus;
        private System.Windows.Forms.ComboBox cbRepairOrderStatus;
        private System.Windows.Forms.Label lblProdDescription;
        private System.Windows.Forms.Label lblProdDescriptionFieldName;
        private System.Windows.Forms.Label lblProduct;
        private System.Windows.Forms.Label lblProductFieldName;
        private System.Windows.Forms.Label lblSerialNoFieldName;
        public System.Windows.Forms.TextBox tbSerialNo;
        private System.Windows.Forms.Label lblValid;
        private System.Windows.Forms.Label lblInvalid;
        private System.Windows.Forms.Label lblSerialNo;
    }
}
