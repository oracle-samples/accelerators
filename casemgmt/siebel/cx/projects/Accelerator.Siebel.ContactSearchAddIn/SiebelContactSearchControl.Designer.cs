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
 *  date: Thu Nov 12 00:55:33 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: c08360097345521c12992b248a1789e78fbc2e04 $
 * *********************************************************************************************
 *  File: SiebelContactSearchControl.Designer.cs
 * *********************************************************************************************/

namespace Accelerator.Siebel.ContactSearchAddIn
{
    partial class SiebelContactSearchControl
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
            this.siebelContactSearchListView = new System.Windows.Forms.ListView();
            this.siebelContactOrgId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.siebelContactFN = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.siebelContactLN = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.siebelContactPhone = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.siebelContactEmail = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.siebelContactPartyId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.emailTextBox = new System.Windows.Forms.TextBox();
            this.emailLabel = new System.Windows.Forms.Label();
            this.phoneTextBox = new System.Windows.Forms.TextBox();
            this.phoneLabel = new System.Windows.Forms.Label();
            this.siebelContactSearchButton = new System.Windows.Forms.Button();
            this.rnContactSearchListView = new System.Windows.Forms.ListView();
            this.rnContactId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.rnContactFN = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.rnContactLN = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.rnContactPhone = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.rnContactEmail = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.rnContactPartyId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.siebelContactListLabel = new System.Windows.Forms.Label();
            this.rnContactListLabel = new System.Windows.Forms.Label();
            this.chatWsMsg = new System.Windows.Forms.Label();
            this.lastNameTextBox = new System.Windows.Forms.TextBox();
            this.lastNameLabel = new System.Windows.Forms.Label();
            this.firstNameTextBox = new System.Windows.Forms.TextBox();
            this.firstNameLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // siebelContactSearchListView
            // 
            this.siebelContactSearchListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.siebelContactOrgId,
            this.siebelContactFN,
            this.siebelContactLN,
            this.siebelContactPhone,
            this.siebelContactEmail,
            this.siebelContactPartyId});
            this.siebelContactSearchListView.GridLines = true;
            this.siebelContactSearchListView.Location = new System.Drawing.Point(3, 97);
            this.siebelContactSearchListView.Name = "siebelContactSearchListView";
            this.siebelContactSearchListView.Size = new System.Drawing.Size(711, 116);
            this.siebelContactSearchListView.TabIndex = 0;
            this.siebelContactSearchListView.UseCompatibleStateImageBehavior = false;
            this.siebelContactSearchListView.View = System.Windows.Forms.View.Details;
            this.siebelContactSearchListView.SelectedIndexChanged += new System.EventHandler(this.siebelContactSearchListView_SelectedIndexChanged);
            this.siebelContactSearchListView.MouseLeave += new System.EventHandler(this.siebelContactSearchListView_MouseLeave);
            this.siebelContactSearchListView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.siebelContactSearchListView_MouseMove);
            // 
            // siebelContactOrgId
            // 
            this.siebelContactOrgId.Text = "Org ID";
            this.siebelContactOrgId.Width = 50;
            // 
            // siebelContactFN
            // 
            this.siebelContactFN.Text = "First Name";
            this.siebelContactFN.Width = 95;
            // 
            // siebelContactLN
            // 
            this.siebelContactLN.Text = "Last Name";
            this.siebelContactLN.Width = 95;
            // 
            // siebelContactPhone
            // 
            this.siebelContactPhone.Text = "Phone";
            this.siebelContactPhone.Width = 125;
            // 
            // siebelContactEmail
            // 
            this.siebelContactEmail.Text = "Email";
            this.siebelContactEmail.Width = 209;
            // 
            // siebelContactPartyId
            // 
            this.siebelContactPartyId.Text = "Party ID";
            this.siebelContactPartyId.Width = 64;
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "Action";
            // 
            // emailTextBox
            // 
            this.emailTextBox.Location = new System.Drawing.Point(78, 36);
            this.emailTextBox.Name = "emailTextBox";
            this.emailTextBox.Size = new System.Drawing.Size(100, 20);
            this.emailTextBox.TabIndex = 6;
            this.emailTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // emailLabel
            // 
            this.emailLabel.AutoSize = true;
            this.emailLabel.Location = new System.Drawing.Point(40, 36);
            this.emailLabel.Name = "emailLabel";
            this.emailLabel.Size = new System.Drawing.Size(32, 13);
            this.emailLabel.TabIndex = 5;
            this.emailLabel.Text = "Email";
            this.emailLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // phoneTextBox
            // 
            this.phoneTextBox.Location = new System.Drawing.Point(271, 32);
            this.phoneTextBox.Name = "phoneTextBox";
            this.phoneTextBox.Size = new System.Drawing.Size(100, 20);
            this.phoneTextBox.TabIndex = 8;
            this.phoneTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // phoneLabel
            // 
            this.phoneLabel.AutoSize = true;
            this.phoneLabel.Location = new System.Drawing.Point(218, 36);
            this.phoneLabel.Name = "phoneLabel";
            this.phoneLabel.Size = new System.Drawing.Size(38, 13);
            this.phoneLabel.TabIndex = 7;
            this.phoneLabel.Text = "Phone";
            this.phoneLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // siebelContactSearchButton
            // 
            this.siebelContactSearchButton.Location = new System.Drawing.Point(414, 30);
            this.siebelContactSearchButton.Name = "siebelContactSearchButton";
            this.siebelContactSearchButton.Size = new System.Drawing.Size(161, 23);
            this.siebelContactSearchButton.TabIndex = 9;
            this.siebelContactSearchButton.Text = "Search Siebel Contacts";
            this.siebelContactSearchButton.UseVisualStyleBackColor = true;
            this.siebelContactSearchButton.Click += new System.EventHandler(this.siebelContactSearchButton_Click);
            // 
            // rnContactSearchListView
            // 
            this.rnContactSearchListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.rnContactId,
            this.rnContactFN,
            this.rnContactLN,
            this.rnContactPhone,
            this.rnContactEmail,
            this.rnContactPartyId});
            this.rnContactSearchListView.GridLines = true;
            this.rnContactSearchListView.Location = new System.Drawing.Point(3, 262);
            this.rnContactSearchListView.Name = "rnContactSearchListView";
            this.rnContactSearchListView.Size = new System.Drawing.Size(711, 118);
            this.rnContactSearchListView.TabIndex = 10;
            this.rnContactSearchListView.UseCompatibleStateImageBehavior = false;
            this.rnContactSearchListView.View = System.Windows.Forms.View.Details;
            this.rnContactSearchListView.SelectedIndexChanged += new System.EventHandler(this.rnContactSearchListView_SelectedIndexChanged);
            this.rnContactSearchListView.MouseLeave += new System.EventHandler(this.rnContactSearchListView_MouseLeave);
            this.rnContactSearchListView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.rnContactSearchListView_MouseMove);
            // 
            // rnContactId
            // 
            this.rnContactId.Text = "ID";
            this.rnContactId.Width = 50;
            // 
            // rnContactFN
            // 
            this.rnContactFN.Text = "First Name";
            this.rnContactFN.Width = 95;
            // 
            // rnContactLN
            // 
            this.rnContactLN.Text = "Last Name";
            this.rnContactLN.Width = 95;
            // 
            // rnContactPhone
            // 
            this.rnContactPhone.Text = "Phone";
            this.rnContactPhone.Width = 125;
            // 
            // rnContactEmail
            // 
            this.rnContactEmail.Text = "Email";
            this.rnContactEmail.Width = 210;
            // 
            // rnContactPartyId
            // 
            this.rnContactPartyId.Text = "Party ID";
            this.rnContactPartyId.Width = 64;
            // 
            // columnHeader13
            // 
            this.columnHeader13.Text = "Action";
            // 
            // siebelContactListLabel
            // 
            this.siebelContactListLabel.AutoSize = true;
            this.siebelContactListLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.siebelContactListLabel.Location = new System.Drawing.Point(11, 76);
            this.siebelContactListLabel.Name = "siebelContactListLabel";
            this.siebelContactListLabel.Size = new System.Drawing.Size(167, 18);
            this.siebelContactListLabel.TabIndex = 11;
            this.siebelContactListLabel.Text = "Contacts from Siebel";
            // 
            // rnContactListLabel
            // 
            this.rnContactListLabel.AutoSize = true;
            this.rnContactListLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rnContactListLabel.Location = new System.Drawing.Point(6, 241);
            this.rnContactListLabel.Name = "rnContactListLabel";
            this.rnContactListLabel.Size = new System.Drawing.Size(194, 18);
            this.rnContactListLabel.TabIndex = 12;
            this.rnContactListLabel.Text = "Contacts from RightNow";
            // 
            // chatWsMsg
            // 
            this.chatWsMsg.AutoSize = true;
            this.chatWsMsg.Location = new System.Drawing.Point(6, 395);
            this.chatWsMsg.Name = "chatWsMsg";
            this.chatWsMsg.Size = new System.Drawing.Size(594, 13);
            this.chatWsMsg.TabIndex = 13;
            this.chatWsMsg.Text = "In chat session, cannot change or associate contact. Please click on incident and" +
    " update the contact information, if needed.";
            this.chatWsMsg.Visible = false;
            // 
            // lastNameTextBox
            // 
            this.lastNameTextBox.Location = new System.Drawing.Point(271, 6);
            this.lastNameTextBox.Name = "lastNameTextBox";
            this.lastNameTextBox.Size = new System.Drawing.Size(100, 20);
            this.lastNameTextBox.TabIndex = 17;
            this.lastNameTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // lastNameLabel
            // 
            this.lastNameLabel.AutoSize = true;
            this.lastNameLabel.Location = new System.Drawing.Point(198, 10);
            this.lastNameLabel.Name = "lastNameLabel";
            this.lastNameLabel.Size = new System.Drawing.Size(58, 13);
            this.lastNameLabel.TabIndex = 16;
            this.lastNameLabel.Text = "Last Name";
            this.lastNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // firstNameTextBox
            // 
            this.firstNameTextBox.Location = new System.Drawing.Point(78, 7);
            this.firstNameTextBox.Name = "firstNameTextBox";
            this.firstNameTextBox.Size = new System.Drawing.Size(100, 20);
            this.firstNameTextBox.TabIndex = 15;
            this.firstNameTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // firstNameLabel
            // 
            this.firstNameLabel.AutoSize = true;
            this.firstNameLabel.Location = new System.Drawing.Point(18, 10);
            this.firstNameLabel.Name = "firstNameLabel";
            this.firstNameLabel.Size = new System.Drawing.Size(57, 13);
            this.firstNameLabel.TabIndex = 14;
            this.firstNameLabel.Text = "First Name";
            this.firstNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SiebelContactSearchControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lastNameTextBox);
            this.Controls.Add(this.lastNameLabel);
            this.Controls.Add(this.firstNameTextBox);
            this.Controls.Add(this.firstNameLabel);
            this.Controls.Add(this.chatWsMsg);
            this.Controls.Add(this.rnContactListLabel);
            this.Controls.Add(this.siebelContactListLabel);
            this.Controls.Add(this.rnContactSearchListView);
            this.Controls.Add(this.siebelContactSearchButton);
            this.Controls.Add(this.phoneTextBox);
            this.Controls.Add(this.phoneLabel);
            this.Controls.Add(this.emailTextBox);
            this.Controls.Add(this.emailLabel);
            this.Controls.Add(this.siebelContactSearchListView);
            this.Name = "SiebelContactSearchControl";
            this.Size = new System.Drawing.Size(737, 451);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.ColumnHeader siebelContactOrgId;
        public System.Windows.Forms.ColumnHeader siebelContactFN;
        public System.Windows.Forms.ColumnHeader siebelContactLN;
        public System.Windows.Forms.ColumnHeader siebelContactPhone;
        public System.Windows.Forms.ColumnHeader siebelContactEmail;
 
        public System.Windows.Forms.TextBox emailTextBox;
        public System.Windows.Forms.Label emailLabel;
        public System.Windows.Forms.TextBox phoneTextBox;
        public System.Windows.Forms.Label phoneLabel;
        public System.Windows.Forms.Button siebelContactSearchButton;
        public System.Windows.Forms.ListView siebelContactSearchListView;
        public System.Windows.Forms.ListView rnContactSearchListView;
        public System.Windows.Forms.ColumnHeader rnContactFN;
        public System.Windows.Forms.ColumnHeader rnContactLN;
        public System.Windows.Forms.ColumnHeader rnContactPhone;
        public System.Windows.Forms.ColumnHeader rnContactEmail;
        public System.Windows.Forms.ColumnHeader rnContactPartyId;
        public System.Windows.Forms.ColumnHeader rnContactId;
        public System.Windows.Forms.Label siebelContactListLabel;
        public System.Windows.Forms.Label rnContactListLabel;
        public System.Windows.Forms.ColumnHeader columnHeader12;
        public System.Windows.Forms.ColumnHeader columnHeader13;
        public System.Windows.Forms.Label chatWsMsg;
        private System.Windows.Forms.ColumnHeader siebelContactPartyId;
        public System.Windows.Forms.TextBox lastNameTextBox;
        public System.Windows.Forms.Label lastNameLabel;
        public System.Windows.Forms.TextBox firstNameTextBox;
        public System.Windows.Forms.Label firstNameLabel;
    }
}
