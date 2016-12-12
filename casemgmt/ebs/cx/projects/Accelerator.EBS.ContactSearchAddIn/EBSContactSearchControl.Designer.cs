/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + EBS Enhancement
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  EBS release: 12.1.3
 *  reference: 150505-000099, 150420-000127
 *  date: Thu Nov 12 00:52:43 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: 32b864e188acfb41e772b3c243f4553f629e5b80 $
 * *********************************************************************************************
 *  File: EBSContactSearchControl.Designer.cs
 * *********************************************************************************************/

namespace Accelerator.EBS.ContactSearchAddIn
{
    partial class EBSContactSearchControl
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
            this.ebsContactSearchListView = new System.Windows.Forms.ListView();
            this.ebsContactOrgId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ebsContactFN = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ebsContactLN = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ebsContactPhone = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ebsContactEmail = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ebsContactPartyId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.emailTextBox = new System.Windows.Forms.TextBox();
            this.emailLabel = new System.Windows.Forms.Label();
            this.phoneTextBox = new System.Windows.Forms.TextBox();
            this.phoneLabel = new System.Windows.Forms.Label();
            this.ebsContactSearchButton = new System.Windows.Forms.Button();
            this.rnContactSearchListView = new System.Windows.Forms.ListView();
            this.rnContactId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.rnContactFN = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.rnContactLN = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.rnContactPhone = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.rnContactEmail = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.rnContactPartyId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ebsContactListLabel = new System.Windows.Forms.Label();
            this.rnContactListLabel = new System.Windows.Forms.Label();
            this.chatWsMsg = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ebsContactSearchListView
            // 
            this.ebsContactSearchListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ebsContactOrgId,
            this.ebsContactFN,
            this.ebsContactLN,
            this.ebsContactPhone,
            this.ebsContactEmail,
            this.ebsContactPartyId});
            this.ebsContactSearchListView.GridLines = true;
            this.ebsContactSearchListView.Location = new System.Drawing.Point(3, 97);
            this.ebsContactSearchListView.Name = "ebsContactSearchListView";
            this.ebsContactSearchListView.Size = new System.Drawing.Size(711, 116);
            this.ebsContactSearchListView.TabIndex = 0;
            this.ebsContactSearchListView.UseCompatibleStateImageBehavior = false;
            this.ebsContactSearchListView.View = System.Windows.Forms.View.Details;
            this.ebsContactSearchListView.SelectedIndexChanged += new System.EventHandler(this.ebsContactSearchListView_SelectedIndexChanged);
            this.ebsContactSearchListView.MouseLeave += new System.EventHandler(this.ebsContactSearchListView_MouseLeave);
            this.ebsContactSearchListView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ebsContactSearchListView_MouseMove);
            // 
            // ebsContactOrgId
            // 
            this.ebsContactOrgId.Text = "Org ID";
            this.ebsContactOrgId.Width = 50;
            // 
            // ebsContactFN
            // 
            this.ebsContactFN.Text = "First Name";
            this.ebsContactFN.Width = 95;
            // 
            // ebsContactLN
            // 
            this.ebsContactLN.Text = "Last Name";
            this.ebsContactLN.Width = 95;
            // 
            // ebsContactPhone
            // 
            this.ebsContactPhone.Text = "Phone";
            this.ebsContactPhone.Width = 125;
            // 
            // ebsContactEmail
            // 
            this.ebsContactEmail.Text = "Email";
            this.ebsContactEmail.Width = 209;
            // 
            // ebsContactPartyId
            // 
            this.ebsContactPartyId.Text = "Party ID";
            this.ebsContactPartyId.Width = 64;
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "Action";
            // 
            // emailTextBox
            // 
            this.emailTextBox.Location = new System.Drawing.Point(78, 29);
            this.emailTextBox.Name = "emailTextBox";
            this.emailTextBox.Size = new System.Drawing.Size(100, 20);
            this.emailTextBox.TabIndex = 6;
            this.emailTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // emailLabel
            // 
            this.emailLabel.AutoSize = true;
            this.emailLabel.Location = new System.Drawing.Point(40, 32);
            this.emailLabel.Name = "emailLabel";
            this.emailLabel.Size = new System.Drawing.Size(32, 13);
            this.emailLabel.TabIndex = 5;
            this.emailLabel.Text = "Email";
            this.emailLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // phoneTextBox
            // 
            this.phoneTextBox.Location = new System.Drawing.Point(262, 32);
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
            // ebsContactSearchButton
            // 
            this.ebsContactSearchButton.Location = new System.Drawing.Point(414, 30);
            this.ebsContactSearchButton.Name = "ebsContactSearchButton";
            this.ebsContactSearchButton.Size = new System.Drawing.Size(161, 23);
            this.ebsContactSearchButton.TabIndex = 9;
            this.ebsContactSearchButton.Text = "Search EBS Contacts";
            this.ebsContactSearchButton.UseVisualStyleBackColor = true;
            this.ebsContactSearchButton.Click += new System.EventHandler(this.ebsContactSearchButton_Click);
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
            // ebsContactListLabel
            // 
            this.ebsContactListLabel.AutoSize = true;
            this.ebsContactListLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ebsContactListLabel.Location = new System.Drawing.Point(6, 71);
            this.ebsContactListLabel.Name = "ebsContactListLabel";
            this.ebsContactListLabel.Size = new System.Drawing.Size(154, 18);
            this.ebsContactListLabel.TabIndex = 11;
            this.ebsContactListLabel.Text = "Contacts from EBS";
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
            // EBSContactSearchControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chatWsMsg);
            this.Controls.Add(this.rnContactListLabel);
            this.Controls.Add(this.ebsContactListLabel);
            this.Controls.Add(this.rnContactSearchListView);
            this.Controls.Add(this.ebsContactSearchButton);
            this.Controls.Add(this.phoneTextBox);
            this.Controls.Add(this.phoneLabel);
            this.Controls.Add(this.emailTextBox);
            this.Controls.Add(this.emailLabel);
            this.Controls.Add(this.ebsContactSearchListView);
            this.Name = "EBSContactSearchControl";
            this.Size = new System.Drawing.Size(737, 437);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.ColumnHeader ebsContactOrgId;
        public System.Windows.Forms.ColumnHeader ebsContactFN;
        public System.Windows.Forms.ColumnHeader ebsContactLN;
        public System.Windows.Forms.ColumnHeader ebsContactPhone;
        public System.Windows.Forms.ColumnHeader ebsContactEmail;
         /*
          * EBS Contact Search API does not support firstname/lastname search. 
          * 
        public System.Windows.Forms.Label firstNameLabel;
        public System.Windows.Forms.TextBox firstNameTextBox;
        public System.Windows.Forms.TextBox lastNameTextBox;
        public System.Windows.Forms.Label lastNameLabel;
          * */
        public System.Windows.Forms.TextBox emailTextBox;
        public System.Windows.Forms.Label emailLabel;
        public System.Windows.Forms.TextBox phoneTextBox;
        public System.Windows.Forms.Label phoneLabel;
        public System.Windows.Forms.Button ebsContactSearchButton;
        public System.Windows.Forms.ListView ebsContactSearchListView;
        public System.Windows.Forms.ListView rnContactSearchListView;
        public System.Windows.Forms.ColumnHeader rnContactFN;
        public System.Windows.Forms.ColumnHeader rnContactLN;
        public System.Windows.Forms.ColumnHeader rnContactPhone;
        public System.Windows.Forms.ColumnHeader rnContactEmail;
        public System.Windows.Forms.ColumnHeader rnContactPartyId;
        public System.Windows.Forms.ColumnHeader rnContactId;
        public System.Windows.Forms.Label ebsContactListLabel;
        public System.Windows.Forms.Label rnContactListLabel;
        public System.Windows.Forms.ColumnHeader columnHeader12;
        public System.Windows.Forms.ColumnHeader columnHeader13;
        public System.Windows.Forms.Label chatWsMsg;
        private System.Windows.Forms.ColumnHeader ebsContactPartyId;
    }
}
