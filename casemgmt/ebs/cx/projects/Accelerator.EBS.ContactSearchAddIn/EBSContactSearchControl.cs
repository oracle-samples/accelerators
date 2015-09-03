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
 *  date: Wed Sep  2 23:11:37 PDT 2015

 *  revision: rnw-15-8-fixes-release-01
 *  SHA1: $Id: 9abcc7a3d441cce444c100a3dbe53a3c634deab2 $
 * *********************************************************************************************
 *  File: EBSContactSearchControl.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RightNow.AddIns.AddInViews;
using System.ServiceModel;
using Accelerator.EBS.SharedServices.RightNowServiceReference;
using Accelerator.EBS.SharedServices.Logs;
using Accelerator.EBS.SharedServices;
using Oracle.RightNow.Cti.MediaBar;
using System.Reflection;

namespace Accelerator.EBS.ContactSearchAddIn
{
    public partial class EBSContactSearchControl : UserControl
    {
        public delegate void searchButtonClickHandler(object sender, EBSContactSearchControl ucontrol);
        public IGlobalContext _gContext;
        public IRecordContext _rContext;
        public BackgroundWorker _bw_searchEBSContact; // make it a member so that ContactWorkspaceAddIn can call its CancelAsync()
        public LogWrapper _log
        {
            get;
            set;
        }
        public int _logIncidentId;
        public int _logContactId;
        public RightNowService _rnSrv
        {
            get;
            set;
        }
        public string _wsTypeName;
        public int _contactSearchReportId;

        private Cursor standardCursor = Cursors.Arrow;
        private Cursor differentCursor = Cursors.Hand;
        private Cursor waitingCursor = Cursors.WaitCursor;

        private BackgroundWorker bw_ebsContactSearch = new BackgroundWorker();
        private BackgroundWorker bw_rnContactSearch = new BackgroundWorker();
        private bool isSearchingEBSContact = false;
        private bool isSearchingRnContact = false;
        //private Cursor myCursor;

        public IContact contactRecord;
        public IIncident incidentRecord;
        public IChat chatRecord;
        public static bool useCTI = false;
        private static bool assemChecked = false;

        public EBSContactSearchControl(IRecordContext recordContext, IGlobalContext globalContext)
        {
            // Set up Add-In UI
            InitializeComponent();

            ebsContactSearchListView.FullRowSelect = true;
            rnContactSearchListView.FullRowSelect = true;

            // Set necessary values
            _rContext = recordContext;
            _gContext = globalContext;

            bw_ebsContactSearch.WorkerSupportsCancellation = true;
            bw_ebsContactSearch.DoWork += new DoWorkEventHandler(bw_SearchEBSContact);
            bw_ebsContactSearch.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunSearchEBSContactCompleted);

            bw_rnContactSearch.WorkerSupportsCancellation = true;
            bw_rnContactSearch.DoWork += new DoWorkEventHandler(bw_SearchRnContact);
            bw_rnContactSearch.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunSearchRnContactCompleted);

            if (!assemChecked)
            {
                Assembly[] arAssem = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly assem in arAssem)
                {
                    if (assem.FullName.Contains("Oracle.RightNow.Cti"))
                    {
                        useCTI = true;
                        break;
                    }
                }
                assemChecked = true;
            }
        }


        private void ebsContactSearchButton_Click(object sender, EventArgs e)
        {
            string phone = phoneTextBox.Text;
            string phoneDigits = phone;
            this.enableOrDisableControlComponents(false);
            if (!String.IsNullOrWhiteSpace(phone))
            {
                phoneDigits = new String(phone.Where(Char.IsDigit).ToArray());
                // MessageBox is hanging if integrated with CTI because of the not main ui thread.
                if (sender != "NoMessageBox" && phoneDigits.Length < 10)
                {
                    if (phoneDigits.Length == 7)
                    {
                        string message = "RightNow Contact Search need whole phone number. Continue search EBS contact without area code?";
                        MessageBoxButtons buttons = MessageBoxButtons.OKCancel;
                        DialogResult result;

                        result = System.Windows.Forms.MessageBox.Show(this, message, "Message", buttons, MessageBoxIcon.Question);
                        // Search contact if choose continue
                        if (result != DialogResult.OK)
                        {
                            this.enableOrDisableControlComponents(true);
                            return;
                        }
                    }
                    else
                    {
                        string message = "The number is less than 10 digits. Please input the whole number.";
                        MessageBoxButtons buttons = MessageBoxButtons.OK;
                        DialogResult result;

                        result = System.Windows.Forms.MessageBox.Show(this, message, "Message", buttons, MessageBoxIcon.Information);
                        // Search contact if choose continue
                        if (result == DialogResult.OK)
                        {
                            this.enableOrDisableControlComponents(true);
                            return;
                        }
                    }
                }
            }

            if (_rContext.WorkspaceTypeName != "Chat")
            {
                if (!this.ebsContactSearchListView.Columns.Contains(this.columnHeader12))
                {
                    this.ebsContactSearchListView.Columns.Insert(6, this.columnHeader12);
                }
                if (!this.rnContactSearchListView.Columns.Contains(this.columnHeader13))
                {
                    this.rnContactSearchListView.Columns.Insert(6, this.columnHeader13);
                }
            }

            string logMessage = "Invoke Contact Search. ";
            string logNote = "";
            _log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);

            //Retrieve EBS Contact Search Result
            this.ebsContactSearchListView.Items.Clear();
            this.rnContactSearchListView.Items.Clear();
            string email = this.emailTextBox.Text;
            email = (email == "") ? null : email;

            if (phoneDigits == "" && email == null)
            {
                this.enableOrDisableControlComponents(true);
                return;
            }

            logMessage = "Contact search field : email = " + email + "; phone = " + phoneDigits;
            logNote = "";
            _log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);

            Dictionary<string, string> searchFields = new Dictionary<string, string>();

            // add an item
            searchFields.Add("email", email);
            searchFields.Add("phone", phoneDigits);

            // Using background thread to get search result and show it in list view
            try
            {
                bw_ebsContactSearch.RunWorkerAsync(searchFields);
                bw_rnContactSearch.RunWorkerAsync(searchFields);
            }
            catch (Exception ex)
            {
                // kill the thread (stop the spinner) if EBS api throws exception, like timed out
                logMessage = "Error in searching contacts. Cancel Async Thread. Error: " + ex.Message;
                logNote = "";
                _log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                _bw_searchEBSContact.CancelAsync();
            }

        }

        private void bw_SearchEBSContact(object sender, DoWorkEventArgs e)
        {
            string logMessage;
            string logNote;
            this.isSearchingEBSContact = true; 
            Dictionary<string, string> searchField = (Dictionary<string, string>)e.Argument;
            string email = searchField["email"];
            string phoneDigits = searchField["phone"];
            //Send request to EBS Server, if endpoint is set
            SharedServices.Contact cont = new SharedServices.Contact();
            SharedServices.Contact[] results = null;
            try
            {
                results = cont.LookupList(null, null, phoneDigits, email, _logIncidentId, _logContactId);
            }
            catch (Exception ex)
            {
                // kill the thread (stop the spinner) if EBS api throws exception
                string message = ex.Message;
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                logMessage = "Error in searching EBS contacts. Cancel Async Thread. Error: " + ex.Message;
                logNote = "";
                _log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
                e.Cancel = true;
                return;
            }

            if (results.Length > 0 && results[0].ErrorMessage != null)
            {
                logMessage = "Response shows error code when searching EBS contacts. email = " + email + "; phone = " + phoneDigits;
                logNote = "";
                _log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
                e.Cancel = true;
                return;
            }

            e.Result = results;
            logMessage = "Ebs contact search result. Count = " + results.Length;
            logNote = "";
            _log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);
        }
        
        private void bw_RunSearchEBSContactCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(e.Cancelled == false && e.Error == null)
            {
                try
                {
                    if (_rContext.WorkspaceTypeName == "Chat")
                    {
                        this.chatWsMsg.Visible = true;
                    }
                    if (e.Result != null)
                    {
                        foreach (Accelerator.EBS.SharedServices.Contact contact in (SharedServices.Contact[])e.Result)
                        {

                            ListViewItem item = null;
                            item = this.ebsContactSearchListView.Items.Add(contact.ContactOrgID.ToString());
                            item.SubItems.Add(contact.FirstName);
                            item.SubItems.Add(contact.LastName);
                            item.SubItems.Add(contact.PhoneNumber);
                            item.SubItems.Add(contact.Email);
                            item.SubItems.Add(contact.ContactPartyID.ToString());

                            item.UseItemStyleForSubItems = false;
                            if (_rContext.WorkspaceTypeName != "Chat")
                            {
                                this.chatWsMsg.Visible = true;
                                item.SubItems.Add("Select");
                                item.SubItems[6].ForeColor = System.Drawing.Color.Blue;
                                item.SubItems[6].Font = new Font(item.Font, item.Font.Style | FontStyle.Underline);
                            }
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    this.isSearchingEBSContact = false;
                    string logMessage = "Error in displaying EBS Contact Search result. Error: " + ex.Message;
                    string logNote = "";
                    _log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
                }
            }
            this.isSearchingEBSContact = false;
            if (this.isSearchingRnContact == false)
            {
                this.enableOrDisableControlComponents(true);
            }
        }

        private void bw_SearchRnContact(object sender, DoWorkEventArgs e)
        {
            string logMessage;
            string logNote;
            this.isSearchingRnContact = true;
            Dictionary<string, string> searchField = (Dictionary<string, string>)e.Argument;
            string email = searchField["email"];
            string phoneDigits = searchField["phone"];
            //Retireve RNow Contact Search Result
            //Check contact search report Id before running report
            if (_contactSearchReportId == 0)
            {
                logMessage = "Contact search report is not assigned in configuration.";
                logNote = "";
                _log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
                e.Cancel = true;
                return;
            }

            logMessage = "Search contacts from CWSS. Search Report ID = " + _contactSearchReportId.ToString();
            logNote = "";
            _log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);


            //Create a filter and specify the filter name
            //Assigning the filter created in desktop agent to the new analytics filter
            List<AnalyticsReportFilter> filterList = new List<AnalyticsReportFilter>();
            /*
             * EBS Contact Search API does not support firstname/lastname search. 
             * 
            AnalyticsReportFilter lastNameFilter = new AnalyticsReportFilter();
            lastNameFilter.Name = "Contact Last Name";
            lastNameFilter.Values = new String[] { lastname };
            filterList.Add(lastNameFilter);

            AnalyticsReportFilter firstNameFilter = new AnalyticsReportFilter();
            firstNameFilter.Name = "Contact First Name";
            firstNameFilter.Values = new String[] { firstname };
            filterList.Add(firstNameFilter);
            */
            AnalyticsReportFilter emailFilter = new AnalyticsReportFilter();
            emailFilter.Name = "Email";
            emailFilter.Values = new String[] { email };
            filterList.Add(emailFilter);

            AnalyticsReportFilter phoneFilter = new AnalyticsReportFilter();
            phoneFilter.Name = "Phone";
            phoneFilter.Values = new String[] { phoneDigits };
            filterList.Add(phoneFilter);

            String[] searchResults = null;
            try
            {
                searchResults = _rnSrv.getReportResult(_contactSearchReportId, filterList);
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    logMessage = "Error in searching Oracle Service Cloud contacts from CWSS. Error in running report: " + _contactSearchReportId + ": " + ex.Message;
                    logNote = "";
                    _log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
                }
                MessageBox.Show("Exception in searching Oracle Service Cloud contacts via CWSS. Please check log for detail. ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
                return;
            }

            e.Result = searchResults;
            logMessage = "Report result is returned. Count = " + searchResults.Length;
            logNote = "";
            _log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote); ;
        }

        private void bw_RunSearchRnContactCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

             if(e.Cancelled == false && e.Error == null)
            {
                try
                {
                    if (_rContext.WorkspaceTypeName == "Chat")
                    {
                        this.chatWsMsg.Visible = true;
                    }
                    String[] rowData;
                    int index = 0;
                    if (e.Result != null)
                    {
                        foreach (String row in (String[])e.Result)
                        {
                            index++;
                            rowData = row.Split(',');
                            ListViewItem item = null;
                            item = this.rnContactSearchListView.Items.Add(rowData[0]);
                            item.SubItems.Add(rowData[1]);
                            item.SubItems.Add(rowData[2]);
                            item.SubItems.Add(rowData[3]);
                            item.SubItems.Add(rowData[4]);
                            item.SubItems.Add(rowData[5]);

                            item.UseItemStyleForSubItems = false;

                            if (_rContext.WorkspaceTypeName != "Chat")
                            {
                                item.SubItems.Add("Select");
                                item.SubItems[6].ForeColor = System.Drawing.Color.Blue;
                                item.SubItems[6].Font = new Font(item.Font, item.Font.Style | FontStyle.Underline);
                            }

                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    this.isSearchingRnContact = false;
                    string logMessage = "Error in displaying EBS Contact Search result. Error: " + ex.Message;
                    string logNote = "";
                    _log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
                }
            }
            this.isSearchingRnContact = false;
            if (this.isSearchingEBSContact == false)
            {
                this.enableOrDisableControlComponents(true);
            }
        }

        // Enable/Disable UI component during searching or after searching
        private void enableOrDisableControlComponents(bool enableState)
        {
            this.Cursor = (enableState == true) ? standardCursor : waitingCursor;
            ebsContactSearchButton.Enabled = enableState;
            ebsContactSearchListView.Enabled = enableState;
            rnContactSearchListView.Enabled = enableState;
            /*
             * 
             * 
            firstNameTextBox.Enabled = enableState;
            lastNameTextBox.Enabled = enableState;
            */
            emailTextBox.Enabled = enableState;
            phoneTextBox.Enabled = enableState;
        }

        // Select Row in EBS Contact Search Result
        private void ebsContactSearchListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_wsTypeName == "Chat")
            {
                return;
            }
            // Get current Workspace type
            this.enableOrDisableControlComponents(false);
            _wsTypeName = _rContext.WorkspaceTypeName;

            ListView.SelectedListViewItemCollection ebsContacts = this.ebsContactSearchListView.SelectedItems;
            String contactPartyId = "";
            String firstName = "";
            String lastName = "";
            String phone = "";
            String email = "";
            String orgId = "";

            //
            //Get Column values
            //
            foreach (ListViewItem item in ebsContacts)
            {
                orgId = item.SubItems[0].Text;
                firstName = item.SubItems[1].Text;
                lastName = item.SubItems[2].Text;
                phone = item.SubItems[3].Text;
                email = item.SubItems[4].Text;
                contactPartyId = item.SubItems[5].Text;
            }

            string logMessage = "Selected row fields firstname = " + firstName + "; lastname = " + lastName + "; email = " + email + "; phone = " + phone;
            string logNote = "";
            _log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);

            // Check Whether contactPartyId is set in this row
            // If it is none, the record should have error in MockServer
            if (contactPartyId != "")
            {
                // Check wheteher the contact Search Report Id is set in config verb.
                if (_contactSearchReportId == 0)
                {
                    logMessage = "Contact Search Report is not set in Configuration.";
                    logNote = "";
                    _log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
                    return;
                }
                   

                // Check whether selected EBS Contact has been associated with any other RNow Contact
                String associatedContactId = checkEBSContactPartyID(contactPartyId, null);
                if (associatedContactId == "0")
                {
                    this.enableOrDisableControlComponents(true);
                    return;
                }

                if (associatedContactId == null)
                {
                    // Selected EBS Contact is not associated to any RNow Contact
                    logMessage = "Selected EBS Contact (Party ID = "+ contactPartyId +") is not associated to any RNow Contact";
                    logNote = "";
                    _log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);

                    if (_wsTypeName == "Contact")
                    {
                        //IContact contactRecord = _rContext.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Contact) as IContact;

                        if (contactRecord.ID < 1)
                        {
                            contactRecord.NameFirst = firstName;
                            contactRecord.NameLast = lastName;
                            contactRecord.PhOffice = phone;
                            contactRecord.EmailAddr = email;

                            bool custom_attr_contact_party_id = false;
                            bool custom_attr_contact_org_id = false;
                            IList<ICustomAttribute> customAttributes = contactRecord.CustomAttributes;
                            foreach (ICustomAttribute cusAttr in customAttributes)
                            {
                                if (cusAttr.PackageName == "Accelerator" && cusAttr.GenericField.Name == "Accelerator$ebs_contact_party_id")
                                {
                                    custom_attr_contact_party_id = true;
                                    cusAttr.GenericField.DataValue.Value = Convert.ToInt32(contactPartyId);
                                }
                                if (cusAttr.PackageName == "Accelerator" && cusAttr.GenericField.Name == "Accelerator$ebs_contact_org_id")
                                {
                                    custom_attr_contact_org_id = true;
                                    cusAttr.GenericField.DataValue.Value = Convert.ToInt32(orgId);
                                }
                                    
                            }
                            if (custom_attr_contact_party_id == false)
                            {
                                logMessage = "Custom attribute is not defined. Cannot get Accelerator$ebs_contact_party_id.";
                                logNote = "";
                                _log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
                            }

                            if (custom_attr_contact_org_id == false)
                            {
                                logMessage = "Custom attribute is not defined. Cannot get Accelerator$ebs_contact_org_id.";
                                logNote = "";
                                _log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
                            }

                            if (custom_attr_contact_party_id == false || custom_attr_contact_org_id == false)
                            {
                                MessageBox.Show("Custom Attribute configuration missing. Please check log for detail.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            // Initializzeds the variables to pass to the MessageBox.Show method
                            string message = "Associated current Contact with EBS Contact (EBS Contact ID = " + contactPartyId + ")";
                            MessageBoxButtons buttons = MessageBoxButtons.OKCancel;
                            DialogResult result;

                            // Show message box to mention the assigned contact party id
                            result = System.Windows.Forms.MessageBox.Show(this, message, "Message", buttons, MessageBoxIcon.Question);
                            if (result == DialogResult.OK)
                            {
                                //
                                // Get current contact record
                                // Set ebs contact id as co_ebsContactPartyId 
                                //
                                IList<ICustomAttribute> customAttributes = contactRecord.CustomAttributes;
                                bool custom_attr_contact_party_id = false;
                                bool custom_attr_contact_org_id = false;

                                foreach (ICustomAttribute cusAttr in customAttributes)
                                {
                                    if (cusAttr.PackageName == "Accelerator" && cusAttr.GenericField.Name == "Accelerator$ebs_contact_party_id")
                                    {
                                        custom_attr_contact_party_id = true;
                                        cusAttr.GenericField.DataValue.Value = Convert.ToInt32(contactPartyId);
                                    }
                                    if (cusAttr.PackageName == "Accelerator" && cusAttr.GenericField.Name == "Accelerator$ebs_contact_org_id")
                                    {
                                        custom_attr_contact_org_id = true;
                                        cusAttr.GenericField.DataValue.Value = Convert.ToInt32(orgId);
                                    }

                                }
                                if (custom_attr_contact_party_id == false)
                                {
                                    logMessage = "Custom attribute is not defined. Cannot get Accelerator$ebs_contact_party_id.";
                                    logNote = "";
                                    _log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
                                }

                                if (custom_attr_contact_org_id == false)
                                {
                                    logMessage = "Custom attribute is not defined. Cannot get Accelerator$ebs_contact_org_id.";
                                    logNote = "";
                                    _log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
                                }

                                if (custom_attr_contact_party_id == false || custom_attr_contact_org_id == false)
                                {
                                    MessageBox.Show("Custom Attribute configuration missing. Please check log for detail.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }

                                //
                                // Save the updated Contact
                                // Refresh the workspace
                                //
                                _rContext.RefreshWorkspace();
                                result = System.Windows.Forms.MessageBox.Show(this, "Record is refreshed.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                logMessage = "In contact WS, " + message;
                                logNote = "";
                                _log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);
                            }
                        }
                    }
                    if (_wsTypeName == "Incident")
                    {
                        bool currentContactHasAssociation = true;
                        
                        if (contactRecord != null) 
                        {
                            IList<ICustomAttribute> customAttributes = contactRecord.CustomAttributes;
                            string[] conCustomAttr = { "Accelerator$ebs_contact_party_id" };
                            Dictionary<String, Object> conCustomAttrResults = CustomAttrHelper.fetchCustomAttrValue(customAttributes, conCustomAttr, this._logIncidentId, this._logContactId);
                            String contactPartyIdStr = conCustomAttrResults["Accelerator$ebs_contact_party_id"] != null ? conCustomAttrResults["Accelerator$ebs_contact_party_id"].ToString() : "0";
                            currentContactHasAssociation = (contactPartyIdStr == "0") ? false : true;
                        }

                        if (currentContactHasAssociation == false)
                        {
                            string message = "The EBS Contact has not associtated with any RNow Contact. Associated to current assigned contact?";
                            MessageBoxButtons buttons = MessageBoxButtons.OKCancel;
                            DialogResult result;

                            result = System.Windows.Forms.MessageBox.Show(this, message, "Message", buttons, MessageBoxIcon.Question);

                            if (result == DialogResult.OK)
                            {
                                logMessage = "In incident WS, EBS Contact (Party ID = " + contactPartyId + ") need to associate to current RNow contact";
                                logNote = "";
                                _log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);

                                IList<ICustomAttribute> customAttributes = contactRecord.CustomAttributes;
                                bool custom_attr_contact_party_id = false;
                                bool custom_attr_contact_org_id = false;

                                foreach (ICustomAttribute cusAttr in customAttributes)
                                {
                                    if (cusAttr.PackageName == "Accelerator" && cusAttr.GenericField.Name == "Accelerator$ebs_contact_party_id")
                                    {
                                        custom_attr_contact_party_id = true;
                                        cusAttr.GenericField.DataValue.Value = Convert.ToInt32(contactPartyId);
                                    }
                                    if (cusAttr.PackageName == "Accelerator" && cusAttr.GenericField.Name == "Accelerator$ebs_contact_org_id")
                                    {
                                        custom_attr_contact_org_id = true;
                                        cusAttr.GenericField.DataValue.Value = Convert.ToInt32(orgId);
                                    }

                                }

                                if (custom_attr_contact_party_id == false)
                                {
                                    logMessage = "Custom attribute is not defined. Cannot get Accelerator$ebs_contact_party_id.";
                                    logNote = "";
                                    _log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
                                }

                                if (custom_attr_contact_org_id == false)
                                {
                                    logMessage = "Custom attribute is not defined. Cannot get Accelerator$ebs_contact_org_id.";
                                    logNote = "";
                                    _log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
                                }

                                if (custom_attr_contact_party_id == false || custom_attr_contact_org_id == false)
                                {
                                    MessageBox.Show("Custom Attribute configuration missing. Please check log for detail.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }

                                result = System.Windows.Forms.MessageBox.Show(this, "EBS Contact has been associated to current RNow contact.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        else
                        {
                            string message = "The EBS Contact has been associtated with a RNow Contact. Save it as new RNow contact and assigned to current incident?";
                            MessageBoxButtons buttons = MessageBoxButtons.OKCancel;
                            DialogResult result;

                            // Show message box to mention the assigned contact party id
                            result = System.Windows.Forms.MessageBox.Show(this, message, "Message", buttons, MessageBoxIcon.Question);


                            // Create new contact with selected EBS Contact and assign it as the Primary Contact of current incident record
                            if (result == DialogResult.OK)
                            {
                                logMessage = "In incident WS, EBS Contact (Party ID = " + contactPartyId + ") need to Create a RNow contact and set selected EBS Contact as related EBS Contact";
                                logNote = "";
                                _log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);

                                //Check email before create 
                                if (String.IsNullOrWhiteSpace(email))
                                {
                                    string additionalMessage = "Cannot create the contact without email address!";
                                    MessageBoxButtons additionalButtons = MessageBoxButtons.OK;
                                    DialogResult additionalResult;

                                    // Show message box to mention duplicated email
                                    additionalResult = System.Windows.Forms.MessageBox.Show(this, additionalMessage, "Error", additionalButtons, MessageBoxIcon.Error);

                                }
                                else
                                {
                                    string duplicatedEmailContactId = checkEBSContactPartyID(null, email);
                                    if (duplicatedEmailContactId == "0")
                                    {
                                        this.enableOrDisableControlComponents(true);
                                        return;
                                    }
                                    if (duplicatedEmailContactId != null)
                                    {
                                        logMessage = "Cannot create the contact! The EBS Contact's email address is duplicated with another RNow Contact. Duplicated Contact ID = " + duplicatedEmailContactId;
                                        logNote = "";
                                        _log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);

                                        string additionalMessage = "Cannot create the contact! The EBS Contact's email address is duplicated with another RNow Contact. Open that RNow contact?";
                                        MessageBoxButtons additionalButtons = MessageBoxButtons.OKCancel;
                                        DialogResult additionalResult;

                                        // Show message box to mention duplicated email
                                        additionalResult = System.Windows.Forms.MessageBox.Show(this, additionalMessage, "Error", additionalButtons, MessageBoxIcon.Error);

                                        if (result == DialogResult.OK)
                                        {
                                            int id = Int32.Parse(duplicatedEmailContactId);
                                            _gContext.AutomationContext.EditWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Contact, (long)id);
                                        }
                                        else
                                        {
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        //Create new Contact
                                        long newContactId = CreateContact(firstName, lastName, phone, email, contactPartyId, orgId);

                                        //Associated New Contact to Incident
                                        //IIncident incidentRecord = _rContext.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Incident) as IIncident;
                                        IInc2Contact newAssociatedContact = AddInViewsDataFactory.Create<IInc2Contact>();
                                        newAssociatedContact.Cid = (int)newContactId;
                                        newAssociatedContact.Prmry = true;
                                        incidentRecord.Contact.Add(newAssociatedContact);
                                        _rContext.RefreshWorkspace();
                                        result = System.Windows.Forms.MessageBox.Show(this, "Record is refreshed.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);


                                        logMessage = "In incident WS, associated new contact with current incident. set contact " + newContactId.ToString() + " as Primary Contact.";
                                        logNote = "";
                                        _log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);
                                    }
                                }
                                
                            }
                        }
                    }
                    
                }
                else
                {
                    // Selected EBS Contact has been associated with another RNow Contact
                    // Two different RNow contacts cannot associated with same EBS Contact
                    logMessage = "Selected EBS Contact is associated to RNow Contact. Associated RNow Contact ID = " + associatedContactId;
                    logNote = "";
                    _log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);

                    if (_wsTypeName == "Contact")
                    {

                        string message = "The EBS Contact has already associtated with another contact. Open that contact?";
                        MessageBoxButtons buttons = MessageBoxButtons.OKCancel;
                        DialogResult result;

                        // Show message box to mention the assigned contact party id
                        result = System.Windows.Forms.MessageBox.Show(this, message, "Message", buttons, MessageBoxIcon.Question);
                        if (result == DialogResult.OK)
                        {
                            // Close current contact record and open the one associated to the EBS Contact
                            int id = Int32.Parse(associatedContactId);
                            _rContext.ExecuteEditorCommand(RightNow.AddIns.Common.EditorCommand.Close);
                            _gContext.AutomationContext.EditWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Contact, (long)id);

                            logMessage = "Close current contact record and open associated RNow contact record. contact id = " + associatedContactId;
                            logNote = "";
                            _log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);
                        }
                    }
                    if (_wsTypeName == "Incident")
                    {
                        string message = "The EBS Contact has already associtated with a RNow Contact. Assign the RNow contact to current incident?";
                        MessageBoxButtons buttons = MessageBoxButtons.OKCancel;
                        DialogResult result;

                        // Show message box to mention the assigned contact party id
                        result = System.Windows.Forms.MessageBox.Show(this, message, "Message", buttons, MessageBoxIcon.Question);
                        if (result == DialogResult.OK)
                        {
                            // Set associated RNow Contact as Primary contact of current incident record.
                            //IIncident incidentRecord = _rContext.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Incident) as IIncident;
                            IInc2Contact newAssociatedContact = AddInViewsDataFactory.Create<IInc2Contact>();
                            int id = Int32.Parse(associatedContactId);
                            newAssociatedContact.Cid = id;
                            newAssociatedContact.Prmry = true;
                            incidentRecord.Contact.Add(newAssociatedContact);
                            _rContext.RefreshWorkspace();
                            result = System.Windows.Forms.MessageBox.Show(this, "Record is refreshed.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);


                            logMessage = "Set associated RNow Contact as Primary contact of current incident record and refresh record. Contact ID = " + associatedContactId;
                            logNote = "";
                            _log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);
                        }
                    }
                   
                }
            }
            else
            {
                logMessage = "Selected row does not have contact party id. ";
                logNote = "";
                _log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                string message = "Selected row does not have contact party id. ";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;
                result = System.Windows.Forms.MessageBox.Show(this, message, "Error", buttons, MessageBoxIcon.Error);
            }
            this.enableOrDisableControlComponents(true);
        }

        // Check whether selected EBS Contact has associated to a RightNow Contact - using search report (via SOAP API)
        private String checkEBSContactPartyID(String contactPartyId, String email)
        {
            //Create a filter and specify the filter name
            //Assigning the filter created in desktop agent to the new analytics filter
            List<AnalyticsReportFilter> filterList = new List<AnalyticsReportFilter>();

            if (contactPartyId != null)
            {
                AnalyticsReportFilter contactPartyIdFilter = new AnalyticsReportFilter();
                contactPartyIdFilter.Name = "EBS Contact Party ID";
                contactPartyIdFilter.Values = new String[] { contactPartyId };
                filterList.Add(contactPartyIdFilter);
            }

            if (email != null)
            {
                AnalyticsReportFilter emailFilter = new AnalyticsReportFilter();
                emailFilter.Name = "Email";
                emailFilter.Values = new String[] { email };
                filterList.Add(emailFilter);
            }

            String[] searchResults = null;
            // Get Report Search Result
            try{
                searchResults = _rnSrv.getReportResult(_contactSearchReportId, filterList);
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    string logMessage = "Error in running contact search report via CWSS. Report ID = " + _contactSearchReportId + "; Error: " + ex.Message;
                    string logNote = "";
                    _log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
                }
                MessageBox.Show("Error in checking selected contact. Please check log for detail. ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "0";
            }

            // get associated id, if searched EBS Contact has been related to a RNow Contact
            String[] rowData = null;
            String associatedContactId = null;
            foreach (String row in searchResults)
            {
                rowData = row.Split(',');
                associatedContactId = rowData[0];
            }
            return associatedContactId;
        }

        // Create new contact via SOAP API - Created when an EBS Contact without association is selected
        public long CreateContact(String firstName, String lastName, String phone, String emailAdd, String contactPartyId, string orgId)
        {
            string logMessage;
            string logNote;
            //Create a Contact
            Accelerator.EBS.SharedServices.RightNowServiceReference.Contact newContact = new Accelerator.EBS.SharedServices.RightNowServiceReference.Contact();

            //Build a PersonName object for the Contact and add the PersonName to the new Contact object
            PersonName personName = new PersonName();
            if (firstName != null && firstName != "")
                personName.First = firstName;
            if (lastName != null && lastName != "")
                personName.Last = lastName;
            newContact.Name = personName;

            //Build an Email object and add the Email to the new Contact object
            if (emailAdd != null && emailAdd != "")
            {
                newContact.Emails = new Email[1];
                newContact.Emails[0] = new Email();
                newContact.Emails[0].Address = emailAdd;
                newContact.Emails[0].action = ActionEnum.add;
                newContact.Emails[0].actionSpecified = true;
                newContact.Emails[0].AddressType = new NamedID();
                newContact.Emails[0].AddressType.ID = new ID();
                newContact.Emails[0].AddressType.ID.id = 0;
                newContact.Emails[0].AddressType.ID.idSpecified = true;
            }

            //Build a Phone object and add the Phone to the new Contact object
            if (phone != null && phone != "")
            {
                newContact.Phones = new Phone[1];
                newContact.Phones[0] = new Phone();
                newContact.Phones[0].Number = phone;
                newContact.Phones[0].action = ActionEnum.add;
                newContact.Phones[0].actionSpecified = true;
                newContact.Phones[0].PhoneType = new NamedID();
                newContact.Phones[0].PhoneType.ID = new ID();
                newContact.Phones[0].PhoneType.ID.id = 0;
                newContact.Phones[0].PhoneType.ID.idSpecified = true;
            }

            //Set EBS Contact Party ID (custom field) to the new Contact
            if (contactPartyId != null && contactPartyId != "")
            {
                GenericField cfContactPartyIdField = new GenericField();
                cfContactPartyIdField.name = "ebs_contact_party_id";
                cfContactPartyIdField.dataType = DataTypeEnum.INTEGER;
                cfContactPartyIdField.dataTypeSpecified = true;
                cfContactPartyIdField.DataValue = new DataValue();
                cfContactPartyIdField.DataValue.Items = new object[1];
                cfContactPartyIdField.DataValue.ItemsElementName = new ItemsChoiceType[1];
                cfContactPartyIdField.DataValue.Items[0] = Convert.ToInt32(contactPartyId);
                cfContactPartyIdField.DataValue.ItemsElementName[0] = ItemsChoiceType.IntegerValue;

                GenericField cfContactOrgIdField = new GenericField();
                cfContactOrgIdField.name = "ebs_contact_org_id";
                cfContactOrgIdField.dataType = DataTypeEnum.INTEGER;
                cfContactOrgIdField.dataTypeSpecified = true;
                cfContactOrgIdField.DataValue = new DataValue();
                cfContactOrgIdField.DataValue.Items = new object[1];
                cfContactOrgIdField.DataValue.ItemsElementName = new ItemsChoiceType[1];
                cfContactOrgIdField.DataValue.Items[0] = Convert.ToInt32(orgId);
                cfContactOrgIdField.DataValue.ItemsElementName[0] = ItemsChoiceType.IntegerValue;

                GenericObject customFieldsc = new GenericObject();
                customFieldsc.GenericFields = new GenericField[2];
                customFieldsc.GenericFields[0] = cfContactPartyIdField;
                customFieldsc.GenericFields[1] = cfContactOrgIdField;
                customFieldsc.ObjectType = new RNObjectType();
                customFieldsc.ObjectType.TypeName = "ContactCustomFieldsc";

                GenericField cField = new GenericField();
                cField.name = "Accelerator";
                cField.dataType = DataTypeEnum.OBJECT;
                cField.dataTypeSpecified = true;
                cField.DataValue = new DataValue();
                cField.DataValue.Items = new object[1];
                cField.DataValue.Items[0] = customFieldsc;
                cField.DataValue.ItemsElementName = new ItemsChoiceType[1];
                cField.DataValue.ItemsElementName[0] = ItemsChoiceType.ObjectValue;

                newContact.CustomFields = new GenericObject();
                newContact.CustomFields.GenericFields = new GenericField[1];
                newContact.CustomFields.GenericFields[0] = cField;
                newContact.CustomFields.ObjectType = new RNObjectType();
                newContact.CustomFields.ObjectType.TypeName = "ContactCustomFields";


            }
            //Build the RNObject[]
            RNObject[] newObjects = new RNObject[] { newContact };
            RNObject[] results = null;
            try
            {
                results = _rnSrv.createObject(newObjects);
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    logMessage = "Error in creating contact via CWSS. Contact " + firstName + " " + lastName + ": " + ex.Message;
                    logNote = "";
                    _log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
                }
                MessageBox.Show("Cannot create contact: " + firstName + " " + lastName + ". Please check log for detail. ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
            logMessage = "New Contact is created successfully. Contact ID = " + results[0].ID.id;
            logNote = "";
            _log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);

            return results[0].ID.id;

        }

        // Show RightNow Contact Search Result
        private void rnContactSearchListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_wsTypeName == "Chat")
            {
                return;
            }

            this.enableOrDisableControlComponents(false);
            _wsTypeName = _rContext.WorkspaceTypeName;
            //IContact contactRecord = _rContext.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Contact) as IContact;
            int currentContactId = (contactRecord==null)?0:contactRecord.ID;
            ListView.SelectedListViewItemCollection rnContacts = this.rnContactSearchListView.SelectedItems;
            String contactId = "";
            String firstName = "";
            String lastName = "";
            String phone = "";
            String email = "";

            //
            //Get Column1 value - EBS Contact ID
            //
            foreach (ListViewItem item in rnContacts)
            {
                contactId = item.SubItems[0].Text;
                firstName = item.SubItems[1].Text;
                lastName = item.SubItems[2].Text;
                phone = item.SubItems[3].Text;
                email = item.SubItems[4].Text;
            }


            if (contactId != "")
            {
                string logMessage = "Selected RNow Contact ID =  " + contactId;
                string logNote = "";
                _log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);

                if (contactId == currentContactId.ToString())
                {
                    string message = "Selected contact is current contact.";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    DialogResult result;

                    // Show message box to mention the assigned contact party id
                    result = System.Windows.Forms.MessageBox.Show(this, message, "Message", buttons, MessageBoxIcon.Information);
                }
                else
                {
                    if (_wsTypeName == "Contact")
                    {
                        // Initializzeds the variables to pass to the MessageBox.Show method
                        string message = "Open Contact (Contact ID = " + contactId + ")";
                        MessageBoxButtons buttons = MessageBoxButtons.OKCancel;
                        DialogResult result;

                        // Show message box to mention the assigned contact party id
                        result = System.Windows.Forms.MessageBox.Show(this, message, "Message", buttons, MessageBoxIcon.Question);
                        if (result == DialogResult.OK)
                        {
                            // Close current contact record and open selected RNow Contact
                            int id = Int32.Parse(contactId);
                            _rContext.ExecuteEditorCommand(RightNow.AddIns.Common.EditorCommand.Close);
                            _gContext.AutomationContext.EditWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Contact, (long)id);

                            logMessage = "Open Selected RNow Contact Record. Contact id =  " + contactId;
                            logNote = "";
                            _log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);
                        }
                    }
                    if (_wsTypeName == "Incident")
                    {
                        string message = "Assign the RNow contact?";
                        MessageBoxButtons buttons = MessageBoxButtons.OKCancel;
                        DialogResult result;

                        // Show message box to mention the assigned contact party id
                        result = System.Windows.Forms.MessageBox.Show(this, message, "Message", buttons, MessageBoxIcon.Question);
                        if (result == DialogResult.OK)
                        {
                            // Set selected RNow Contact as Primary Contact of current incident record.
                            //IIncident incidentRecord = _rContext.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Incident) as IIncident;
                            IInc2Contact newContact = AddInViewsDataFactory.Create<IInc2Contact>();
                            int id = Int32.Parse(contactId);
                            newContact.Cid = id;
                            newContact.Prmry = true;
                            incidentRecord.Contact.Add(newContact);
                            _rContext.RefreshWorkspace();
                            result = System.Windows.Forms.MessageBox.Show(this, "Record is refreshed.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);


                            logMessage = "Set selected RNow Contact as Primary Contact. Contact ID =  " + contactId;
                            logNote = "";
                            _log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);
                        }
                    }
                
                }



            }
            else
            {
                string logMessage = "Selected RNow Contact is without Contact ID.";
                string logNote = "";
                _log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
            }
            this.enableOrDisableControlComponents(true);
        }

        // Press "Enter", start searching
        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                ebsContactSearchButton.PerformClick();
            }
        }

        // Invoke by RightNow Workspace Rule
        public void autoSearchParameterInvoke()
        {
            string logMessage = "Invoke auto contact search";
            string logNote = "";
            _log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);

            // Get current Workspace type
            _wsTypeName = _rContext.WorkspaceTypeName;

            if (_wsTypeName == "Chat")
            {
                if (this.chatRecord == null)
                    chatRecord = _rContext.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Chat) as IChat;
                /*
                 * EBS Contact Search API does not support firstname/lastname search. 
                 * 
                if (chatRecord.FirstName != null || chatRecord.FirstName != "")
                {
                    firstNameTextBox.Text = chatRecord.FirstName;
                }
                else
                {
                    firstNameTextBox.Text = "";
                }

                if (chatRecord.LastName != null || chatRecord.LastName != "")
                {
                    lastNameTextBox.Text = chatRecord.LastName;
                }
                else
                {
                    lastNameTextBox.Text = "";
                }
                 * */

                if (chatRecord.Email != null || chatRecord.Email != "")
                {
                    emailTextBox.Text = chatRecord.Email;
                }
                phoneTextBox.Text = "";
            }

            if (_wsTypeName == "Contact" || _wsTypeName == "Incident")
            {
                //IContact contactRecord = _rContext.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Contact) as IContact;
                if (this.contactRecord == null)
                {
                    contactRecord = _rContext.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Contact) as IContact;
                    if(contactRecord == null)
                        return;
                }
                /*
                 * EBS Contact Search API does not support firstname/lastname search. 
                 * 
                if (contactRecord.NameFirst != null || contactRecord.NameFirst != "")
                {
                    firstNameTextBox.Text = contactRecord.NameFirst;
                }
                else
                {
                    firstNameTextBox.Text = "";
                }

                if (contactRecord.NameLast != null || contactRecord.NameLast != "")
                {
                    lastNameTextBox.Text = contactRecord.NameLast;
                }
                else
                {
                    lastNameTextBox.Text = "";
                }
                */

                if (contactRecord.PhOffice != null || contactRecord.PhOffice != "")
                {
                    phoneTextBox.Text = contactRecord.PhOffice;
                }
                else
                {
                    phoneTextBox.Text = "";
                }

                if (contactRecord.EmailAddr != null || contactRecord.EmailAddr != "")
                {
                    emailTextBox.Text = contactRecord.EmailAddr;
                }
            }

            if (emailTextBox.Text != null || emailTextBox.Text != "" ||
                    phoneTextBox.Text != null || phoneTextBox.Text != "")
            {
                this.ebsContactSearchButton_Click(null, EventArgs.Empty);

                logMessage = "Automatically search ebs contact. Search fields: phone = " + phoneTextBox.Text + "; email = " + emailTextBox.Text;
                logNote = "";
                _log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);
            }

        }

        // for useCTI case when creating a new contact 
        public void autoSearchNewContactInvoke()
        {
            string logMessage = "Invoke autoSearchNewContact search";
            string logNote = "";
            _log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);

            if (_rContext.WorkspaceTypeName == "Contact")
            {
                if (this.contactRecord == null)
                {
                    contactRecord = _rContext.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Contact) as IContact;
                    if (contactRecord == null)
                        return;
                }

                if (useCTI)
                {
                    if (contactRecord.ID < 1 && RightNowMediaBar.phoneNumANIfromCti != null)
                    {
                        contactRecord.PhOffice = RightNowMediaBar.phoneNumANIfromCti;
                        RightNowMediaBar.phoneNumANIfromCti = null;

                    }
                }

                if (contactRecord.PhOffice != null || contactRecord.PhOffice != "")
                {
                    phoneTextBox.Text = contactRecord.PhOffice;
                }
                else
                {
                    phoneTextBox.Text = "";
                }

            }

            if (phoneTextBox.Text != null || phoneTextBox.Text != "")
            {
                // MessageBox is hanging if integrated with CTI because of the not main ui thread.
                if (useCTI)
                    this.ebsContactSearchButton_Click("NoMessageBox", EventArgs.Empty);

                logMessage = "Automatically search ebs contact. Search fields: phone = " + phoneTextBox.Text;
                logNote = "";
                _log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);
            }

        }

        private void ebsContactSearchListView_MouseLeave(object sender, EventArgs e)
        {
            if (this.Cursor == differentCursor)
                this.Cursor = standardCursor;
        }

        //Change cursor when mouse is on the "Select" Column
        private void ebsContactSearchListView_MouseMove(object sender, MouseEventArgs e)
        {
            if (_wsTypeName == "Chat")
            {
                return;
            }

            bool found = false;
            ListViewItem.ListViewSubItem lvsi = null;
            int i = 0;
            while (!found && i < ebsContactSearchListView.Items.Count)
            {
                lvsi = ebsContactSearchListView.Items[i].GetSubItemAt(e.Location.X, e.Location.Y);
                if (lvsi == ebsContactSearchListView.Items[i].SubItems[6])
                    found = true;
                i++;
            }
            if (found)
                this.Cursor = differentCursor;
            else if (this.Cursor == differentCursor)
                this.Cursor = standardCursor;
        }

        private void rnContactSearchListView_MouseLeave(object sender, EventArgs e)
        {
            if (this.Cursor == differentCursor)
                this.Cursor = standardCursor;
        }

        //Change cursor when mouse is on the "Select" Column
        private void rnContactSearchListView_MouseMove(object sender, MouseEventArgs e)
        {
            if (_wsTypeName == "Chat")
            {
                return;
            }

            bool found = false;
            ListViewItem.ListViewSubItem lvsi = null;
            int i = 0;
            while (!found && i < rnContactSearchListView.Items.Count)
            {
                lvsi = rnContactSearchListView.Items[i].GetSubItemAt(e.Location.X, e.Location.Y);
                if (lvsi == rnContactSearchListView.Items[i].SubItems[6])
                    found = true;
                i++;
            }
            if (found)
                this.Cursor = differentCursor;
            else if (this.Cursor == differentCursor)
                this.Cursor = standardCursor;
        }     
    }
}
