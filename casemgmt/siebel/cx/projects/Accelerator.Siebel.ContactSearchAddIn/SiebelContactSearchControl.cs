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
 *  SHA1: $Id: bfd15b562e2f3274a42998a0956c44bbf0533cbe $
 * *********************************************************************************************
 *  File: SiebelContactSearchControl.cs
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
using Accelerator.Siebel.SharedServices.RightNowServiceReference;
using Accelerator.Siebel.SharedServices.Logs;
using Accelerator.Siebel.SharedServices;
using RightNow.AddIns.Common;
using Oracle.RightNow.Cti.MediaBar;
using System.Reflection;

namespace Accelerator.Siebel.ContactSearchAddIn
{
    public partial class SiebelContactSearchControl : UserControl
    {
        public delegate void searchButtonClickHandler(object sender, SiebelContactSearchControl ucontrol);
        public IGlobalContext _gContext;
        public IRecordContext _rContext;
        public BackgroundWorker _bw_searchSiebelContact; // make it a member so that ContactWorkspaceAddIn can call its CancelAsync()
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
        //public string _wsTypeName;
        public int _contactSearchReportId;

        private Cursor standardCursor = Cursors.Arrow;
        private Cursor differentCursor = Cursors.Hand;
        private Cursor waitingCursor = Cursors.WaitCursor;

        private BackgroundWorker bw_siebelContactSearch = new BackgroundWorker();
        private BackgroundWorker bw_rnContactSearch = new BackgroundWorker();
        private bool isSearchingSiebelContact = false;
        private bool isSearchingRnContact = false;
        //private Cursor myCursor;

        public IContact contactRecord;
        public IIncident incidentRecord;
        public IChat chatRecord;

        public static bool useCTI = false;
        private static bool assemChecked = false;

        public SiebelContactSearchControl(IRecordContext recordContext, IGlobalContext globalContext)
        {
            // Set up Add-In UI
            InitializeComponent();

            siebelContactSearchListView.FullRowSelect = true;
            rnContactSearchListView.FullRowSelect = true;

            // Set necessary values
            _rContext = recordContext;
            _gContext = globalContext;

            bw_siebelContactSearch.WorkerSupportsCancellation = true;
            bw_siebelContactSearch.DoWork += new DoWorkEventHandler(bw_SearchSiebelContact);
            bw_siebelContactSearch.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunSearchSiebelContactCompleted);

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


        private void siebelContactSearchButton_Click(object sender, EventArgs e)
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
                        string message = "RightNow Contact Search need whole phone number. Continue search Siebel contact without area code?";
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

            if (_rContext.WorkspaceType == WorkspaceRecordType.Incident  ||  _rContext.WorkspaceType == WorkspaceRecordType.Contact)
            {
                if (!this.siebelContactSearchListView.Columns.Contains(this.columnHeader12))
                {
                    this.siebelContactSearchListView.Columns.Insert(6, this.columnHeader12);
                }
                if (!this.rnContactSearchListView.Columns.Contains(this.columnHeader13))
                {
                    this.rnContactSearchListView.Columns.Insert(6, this.columnHeader13);
                }
            }

            string logMessage = "Invoke Contact Search. ";
            string logNote = "";
            _log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);

            //Retrieve Siebel Contact Search Result
            this.siebelContactSearchListView.Items.Clear();
            this.rnContactSearchListView.Items.Clear();
            string email = this.emailTextBox.Text;
            email = (email == "") ? null : email;
            string firstName = this.firstNameTextBox.Text;
            string lastName = this.lastNameTextBox.Text;

            if (phoneDigits == "" && email == null && String.IsNullOrEmpty(firstName) && String.IsNullOrEmpty(lastName))
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
            searchFields.Add("firstname", firstName);
            searchFields.Add("lastname", lastName);

            // Using background thread to get search result and show it in list view
            try
            {
                bw_siebelContactSearch.RunWorkerAsync(searchFields);
                bw_rnContactSearch.RunWorkerAsync(searchFields);
            }
            catch (Exception ex)
            {
                // kill the thread (stop the spinner) if Siebel api throws exception, like timed out
                logMessage = "Error in searching contacts. Cancel Async Thread. Error: " + ex.Message;
                logNote = "";
                _log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);

                _bw_searchSiebelContact.CancelAsync();
            }

        }

        private void bw_SearchSiebelContact(object sender, DoWorkEventArgs e)
        {
            string logMessage;
            string logNote;
            this.isSearchingSiebelContact = true; 
            Dictionary<string, string> searchField = (Dictionary<string, string>)e.Argument;
            string email = searchField["email"];
            string phoneDigits = searchField["phone"];
            string firstname = searchField["firstname"];
            string lastname = searchField["lastname"];
            //Send request to Siebel Server, if endpoint is set
            SharedServices.Contact cont = new SharedServices.Contact();
            SharedServices.Contact[] results = null;
            try
            {
                results = cont.LookupList(firstname, lastname, phoneDigits, email, _logIncidentId, _logContactId);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // kill the thread (stop the spinner) if Siebel api throws exception
                logMessage = "Error in searching Siebel contacts. Cancel Async Thread. Error: " + ex.Message;
                logNote = "";
                _log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
                e.Cancel = true;
                return;
            }

            /****TODO::
            if (results.Length > 0 && results[0].ErrorMessage != null)
            {
                logMessage = "Response shows error code when searching Siebel contacts. email = " + email + "; phone = " + phoneDigits;
                logNote = "";
                _log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
                e.Cancel = true;
                return;
            }
            */
            e.Result = results;
            // check for null, otherwise results.Length hit Object Reference not found
            if (results != null)
            {

                logMessage = "Contact search result. Count = " + results.Length;
                logNote = "";
                _log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);
            }
        }
        
        private void bw_RunSearchSiebelContactCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(e.Cancelled == false && e.Error == null)
            {
                try
                {
                    if (_rContext.WorkspaceType != WorkspaceRecordType.Incident  &&  _rContext.WorkspaceType != WorkspaceRecordType.Contact)
                    {
                        this.chatWsMsg.Visible = true;
                    }
                    if (e.Result != null)
                    {
                        foreach (Accelerator.Siebel.SharedServices.Contact contact in (SharedServices.Contact[])e.Result)
                        {

                            ListViewItem item = null;
                            item = this.siebelContactSearchListView.Items.Add(contact.ContactOrgID.ToString());
                            item.SubItems.Add(contact.FirstName);
                            item.SubItems.Add(contact.LastName);
                            item.SubItems.Add(contact.PhoneNumber);
                            item.SubItems.Add(contact.Email);
                            item.SubItems.Add(contact.ContactPartyID.ToString());

                            item.UseItemStyleForSubItems = false;
                            if (_rContext.WorkspaceType == WorkspaceRecordType.Incident || _rContext.WorkspaceType == WorkspaceRecordType.Contact)
                            {
                                this.chatWsMsg.Visible = false;
                                item.SubItems.Add("Select");
                                item.SubItems[6].ForeColor = System.Drawing.Color.Blue;
                                item.SubItems[6].Font = new Font(item.Font, item.Font.Style | FontStyle.Underline);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.isSearchingSiebelContact = false;
                    string logMessage = "Error in displaying Siebel Contact Search result. Error: " + ex.Message;
                    string logNote = "";
                    _log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
                }
            }
            this.isSearchingSiebelContact = false;
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
            string firstname = searchField["firstname"];
            string lastname = searchField["lastname"];
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

            AnalyticsReportFilter lastNameFilter = new AnalyticsReportFilter();
            lastNameFilter.Name = "Contact Last Name";
            lastNameFilter.Values = new String[] { lastname };
            filterList.Add(lastNameFilter);

            AnalyticsReportFilter firstNameFilter = new AnalyticsReportFilter();
            firstNameFilter.Name = "Contact First Name";
            firstNameFilter.Values = new String[] { firstname };
            filterList.Add(firstNameFilter);
            
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
                    if (_rContext.WorkspaceType != WorkspaceRecordType.Incident  &&  _rContext.WorkspaceType != WorkspaceRecordType.Contact)
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

                            if (_rContext.WorkspaceType == WorkspaceRecordType.Incident || _rContext.WorkspaceType == WorkspaceRecordType.Contact)
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
                    string logMessage = "Error in displaying Siebel Contact Search result. Error: " + ex.Message;
                    string logNote = "";
                    _log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
                }
            }
            this.isSearchingRnContact = false;
            if (this.isSearchingSiebelContact == false)
            {
                this.enableOrDisableControlComponents(true);
            }
        }

        // Enable/Disable UI component during searching or after searching
        private void enableOrDisableControlComponents(bool enableState)
        {
            this.Cursor = (enableState == true) ? standardCursor : waitingCursor;
            siebelContactSearchButton.Enabled = enableState;
            siebelContactSearchListView.Enabled = enableState;
            rnContactSearchListView.Enabled = enableState;
            firstNameTextBox.Enabled = enableState;
            lastNameTextBox.Enabled = enableState;
            emailTextBox.Enabled = enableState;
            phoneTextBox.Enabled = enableState;
        }

        // Select Row in Siebel Contact Search Result
        private void siebelContactSearchListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_rContext.WorkspaceType != WorkspaceRecordType.Incident  &&  _rContext.WorkspaceType != WorkspaceRecordType.Contact)
            {
                return;
            }

            ListView.SelectedListViewItemCollection siebelContacts = this.siebelContactSearchListView.SelectedItems;
            if (siebelContacts.Count == 0)
            {
                return;
            }

            // Get current Workspace type
            this.enableOrDisableControlComponents(false);


            String contactPartyId = "";
            String firstName = "";
            String lastName = "";
            String phone = "";
            String email = "";
            String orgId = "";

            //
            //Get Column values
            //
            foreach (ListViewItem item in siebelContacts)
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
                   

                // Check whether selected Siebel Contact has been associated with any other RNow Contact
                String associatedContactId = checkSiebelContactPartyID(contactPartyId, null);
                if (associatedContactId == "0")
                {
                    this.enableOrDisableControlComponents(true);
                    return;
                }

                if (associatedContactId == null)
                {
                    // Selected Siebel Contact is not associated to any RNow Contact
                    logMessage = "Selected Siebel Contact (Party ID = "+ contactPartyId +") is not associated to any RNow Contact";
                    logNote = "";
                    _log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);

                    if (_rContext.WorkspaceType == WorkspaceRecordType.Contact)
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
                                if (cusAttr.PackageName == "Accelerator" && cusAttr.GenericField.Name == "Accelerator$siebel_contact_party_id")
                                {
                                    custom_attr_contact_party_id = true;
                                    cusAttr.GenericField.DataValue.Value = contactPartyId;
                                }
                                if (cusAttr.PackageName == "Accelerator" && cusAttr.GenericField.Name == "Accelerator$siebel_contact_org_id")
                                {
                                    custom_attr_contact_org_id = true;
                                    cusAttr.GenericField.DataValue.Value = orgId;
                                }
                                    
                            }
                            if (custom_attr_contact_party_id == false)
                            {
                                logMessage = "Custom attribute is not defined. Cannot get Accelerator$siebel_contact_party_id.";
                                logNote = "";
                                _log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
                            }

                            if (custom_attr_contact_org_id == false)
                            {
                                logMessage = "Custom attribute is not defined. Cannot get Accelerator$siebel_contact_org_id.";
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
                            string message = "Associated current Contact with Siebel Contact (Siebel Contact ID = " + contactPartyId + ")";
                            MessageBoxButtons buttons = MessageBoxButtons.OKCancel;
                            DialogResult result;

                            // Show message box to mention the assigned contact party id
                            result = System.Windows.Forms.MessageBox.Show(this, message, "Message", buttons, MessageBoxIcon.Question);
                            if (result == DialogResult.OK)
                            {
                                //
                                // Get current contact record
                                // Set siebel contact id as co_siebelContactPartyId 
                                //
                                IList<ICustomAttribute> customAttributes = contactRecord.CustomAttributes;
                                bool custom_attr_contact_party_id = false;
                                bool custom_attr_contact_org_id = false;

                                foreach (ICustomAttribute cusAttr in customAttributes)
                                {
                                    if (cusAttr.PackageName == "Accelerator" && cusAttr.GenericField.Name == "Accelerator$siebel_contact_party_id")
                                    {
                                        custom_attr_contact_party_id = true;
                                        cusAttr.GenericField.DataValue.Value = contactPartyId;
                                    }
                                    if (cusAttr.PackageName == "Accelerator" && cusAttr.GenericField.Name == "Accelerator$siebel_contact_org_id")
                                    {
                                        custom_attr_contact_org_id = true;
                                        cusAttr.GenericField.DataValue.Value = orgId;
                                    }

                                }
                                if (custom_attr_contact_party_id == false)
                                {
                                    logMessage = "Custom attribute is not defined. Cannot get Accelerator$siebel_contact_party_id.";
                                    logNote = "";
                                    _log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
                                }

                                if (custom_attr_contact_org_id == false)
                                {
                                    logMessage = "Custom attribute is not defined. Cannot get Accelerator$siebel_contact_org_id.";
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
                    }else if (_rContext.WorkspaceType == WorkspaceRecordType.Incident)
                    {
                        bool currentContactHasAssociation = true;
                        
                        if (contactRecord != null) 
                        {
                            IList<ICustomAttribute> customAttributes = contactRecord.CustomAttributes;
                            string[] conCustomAttr = { "Accelerator$siebel_contact_party_id" };
                            Dictionary<String, Object> conCustomAttrResults = CustomAttrHelper.fetchCustomAttrValue(customAttributes, conCustomAttr, this._logIncidentId, this._logContactId);
                            String contactPartyIdStr = conCustomAttrResults["Accelerator$siebel_contact_party_id"] != null ? conCustomAttrResults["Accelerator$siebel_contact_party_id"].ToString() : "0";
                            currentContactHasAssociation = (contactPartyIdStr == "0") ? false : true;
                        }

                        if (currentContactHasAssociation == false)
                        {
                            string message = "The Siebel Contact has not associtated with any RNow Contact. Associated to current assigned contact?";
                            MessageBoxButtons buttons = MessageBoxButtons.OKCancel;
                            DialogResult result;

                            result = System.Windows.Forms.MessageBox.Show(this, message, "Message", buttons, MessageBoxIcon.Question);

                            if (result == DialogResult.OK)
                            {
                                logMessage = "In incident WS, Siebel Contact (Party ID = " + contactPartyId + ") need to associate to current RNow contact";
                                logNote = "";
                                _log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);

                                IList<ICustomAttribute> customAttributes = contactRecord.CustomAttributes;
                                bool custom_attr_contact_party_id = false;
                                bool custom_attr_contact_org_id = false;

                                foreach (ICustomAttribute cusAttr in customAttributes)
                                {
                                    if (cusAttr.PackageName == "Accelerator" && cusAttr.GenericField.Name == "Accelerator$siebel_contact_party_id")
                                    {
                                        custom_attr_contact_party_id = true;
                                        cusAttr.GenericField.DataValue.Value = contactPartyId;
                                    }
                                    if (cusAttr.PackageName == "Accelerator" && cusAttr.GenericField.Name == "Accelerator$siebel_contact_org_id")
                                    {
                                        custom_attr_contact_org_id = true;
                                        cusAttr.GenericField.DataValue.Value = orgId;
                                    }

                                }

                                if (custom_attr_contact_party_id == false)
                                {
                                    logMessage = "Custom attribute is not defined. Cannot get Accelerator$siebel_contact_party_id.";
                                    logNote = "";
                                    _log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
                                }

                                if (custom_attr_contact_org_id == false)
                                {
                                    logMessage = "Custom attribute is not defined. Cannot get Accelerator$siebel_contact_org_id.";
                                    logNote = "";
                                    _log.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
                                }

                                if (custom_attr_contact_party_id == false || custom_attr_contact_org_id == false)
                                {
                                    MessageBox.Show("Custom Attribute configuration missing. Please check log for detail.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }

                                result = System.Windows.Forms.MessageBox.Show(this, "Siebel Contact has been associated to current RNow contact.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        else
                        {
                            string message = "The Siebel Contact has not been associtated with a RNow Contact. Save it as new RNow contact and assigned to current incident?";
                            MessageBoxButtons buttons = MessageBoxButtons.OKCancel;
                            DialogResult result;

                            // Show message box to mention the assigned contact party id
                            result = System.Windows.Forms.MessageBox.Show(this, message, "Message", buttons, MessageBoxIcon.Question);


                            // Create new contact with selected Siebel Contact and assign it as the Primary Contact of current incident record
                            if (result == DialogResult.OK)
                            {
                                logMessage = "In incident WS, Siebel Contact (Party ID = " + contactPartyId + ") need to Create a RNow contact and set selected Siebel Contact as related Siebel Contact";
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
                                    string duplicatedEmailContactId = checkSiebelContactPartyID(null, email);
                                    if (duplicatedEmailContactId == "0")
                                    {
                                        this.enableOrDisableControlComponents(true);
                                        return;
                                    }
                                    if (duplicatedEmailContactId != null)
                                    {
                                        logMessage = "Cannot create the contact! The Siebel Contact's email address is duplicated with another RNow Contact. Duplicated Contact ID = " + duplicatedEmailContactId;
                                        logNote = "";
                                        _log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);

                                        string additionalMessage = "Cannot create the contact! The Siebel Contact's email address is duplicated with another RNow Contact. Open that RNow contact?";
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
                                        contactRecord = _rContext.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Contact) as IContact;
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
                    // Selected Siebel Contact has been associated with another RNow Contact
                    // Two different RNow contacts cannot associated with same Siebel Contact
                    logMessage = "Selected Siebel Contact is associated to RNow Contact. Associated RNow Contact ID = " + associatedContactId;
                    logNote = "";
                    _log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);

                    if (_rContext.WorkspaceType == WorkspaceRecordType.Contact)
                    {

                        string message = "The Siebel Contact has already associtated with another contact. Open that contact?";
                        MessageBoxButtons buttons = MessageBoxButtons.OKCancel;
                        DialogResult result;

                        // Show message box to mention the assigned contact party id
                        result = System.Windows.Forms.MessageBox.Show(this, message, "Message", buttons, MessageBoxIcon.Question);
                        if (result == DialogResult.OK)
                        {
                            // Close current contact record and open the one associated to the Siebel Contact
                            int id = Int32.Parse(associatedContactId);
                            _rContext.ExecuteEditorCommand(RightNow.AddIns.Common.EditorCommand.Close);
                            _gContext.AutomationContext.EditWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Contact, (long)id);

                            logMessage = "Close current contact record and open associated RNow contact record. contact id = " + associatedContactId;
                            logNote = "";
                            _log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);
                        }
                    } else if (_rContext.WorkspaceType == WorkspaceRecordType.Incident)
                    {
                        string message = "The Siebel Contact has already associtated with a RNow Contact. Assign the RNow contact to current incident?";
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
                            contactRecord = _rContext.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Contact) as IContact;
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

        // Check whether selected Siebel Contact has associated to a RightNow Contact - using search report (via SOAP API)
        private String checkSiebelContactPartyID(String contactPartyId, String email)
        {
            //Create a filter and specify the filter name
            //Assigning the filter created in desktop agent to the new analytics filter
            List<AnalyticsReportFilter> filterList = new List<AnalyticsReportFilter>();

            if (contactPartyId != null)
            {
                AnalyticsReportFilter contactPartyIdFilter = new AnalyticsReportFilter();
                contactPartyIdFilter.Name = "Siebel Contact Party ID";
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

            // get associated id, if searched Siebel Contact has been related to a RNow Contact
            String[] rowData = null;
            String associatedContactId = null;
            foreach (String row in searchResults)
            {
                rowData = row.Split(',');
                associatedContactId = rowData[0];
            }
            return associatedContactId;
        }

        // Create new contact via SOAP API - Created when an Siebel Contact without association is selected
        public long CreateContact(String firstName, String lastName, String phone, String emailAdd, String contactPartyId, string orgId)
        {
            string logMessage;
            string logNote;
            //Create a Contact
            Accelerator.Siebel.SharedServices.RightNowServiceReference.Contact newContact = new Accelerator.Siebel.SharedServices.RightNowServiceReference.Contact();

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

            //Set Siebel Contact Party ID (custom field) to the new Contact
            if (contactPartyId != null && contactPartyId != "")
            {
                GenericField cfContactPartyIdField = new GenericField();
                cfContactPartyIdField.name = "siebel_contact_party_id";
                cfContactPartyIdField.dataType = SharedServices.RightNowServiceReference.DataTypeEnum.STRING;
                cfContactPartyIdField.dataTypeSpecified = true;
                cfContactPartyIdField.DataValue = new DataValue();
                cfContactPartyIdField.DataValue.Items = new object[1];
                cfContactPartyIdField.DataValue.ItemsElementName = new ItemsChoiceType[1];
                cfContactPartyIdField.DataValue.Items[0] = contactPartyId;
                cfContactPartyIdField.DataValue.ItemsElementName[0] = ItemsChoiceType.StringValue;

                GenericField cfContactOrgIdField = new GenericField();
                cfContactOrgIdField.name = "siebel_contact_org_id";
                cfContactOrgIdField.dataType = SharedServices.RightNowServiceReference.DataTypeEnum.STRING;
                cfContactOrgIdField.dataTypeSpecified = true;
                cfContactOrgIdField.DataValue = new DataValue();
                cfContactOrgIdField.DataValue.Items = new object[1];
                cfContactOrgIdField.DataValue.ItemsElementName = new ItemsChoiceType[1];
                cfContactOrgIdField.DataValue.Items[0] = orgId;
                cfContactOrgIdField.DataValue.ItemsElementName[0] = ItemsChoiceType.StringValue;

                GenericObject customFieldsc = new GenericObject();
                customFieldsc.GenericFields = new GenericField[2];
                customFieldsc.GenericFields[0] = cfContactPartyIdField;
                customFieldsc.GenericFields[1] = cfContactOrgIdField;
                customFieldsc.ObjectType = new RNObjectType();
                customFieldsc.ObjectType.TypeName = "ContactCustomFieldsc";

                GenericField cField = new GenericField();
                cField.name = "Accelerator";
                cField.dataType = SharedServices.RightNowServiceReference.DataTypeEnum.OBJECT;
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
            if (_rContext.WorkspaceType != WorkspaceRecordType.Incident  &&  _rContext.WorkspaceType != WorkspaceRecordType.Contact)
            {
                return;
            }

            ListView.SelectedListViewItemCollection rnContacts = this.rnContactSearchListView.SelectedItems;
            if (rnContacts.Count == 0)
            {
                return;
            }

            this.enableOrDisableControlComponents(false);
            //IContact contactRecord = _rContext.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Contact) as IContact;
            int currentContactId = (contactRecord==null)?0:contactRecord.ID;

            String contactId = "";
            String firstName = "";
            String lastName = "";
            String phone = "";
            String email = "";

            //
            //Get Column1 value - Siebel Contact ID
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
                    if (_rContext.WorkspaceType == WorkspaceRecordType.Contact)
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
                    }else if (_rContext.WorkspaceType == WorkspaceRecordType.Incident)
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
                            int id = Int32.Parse(contactId);
                            bool foundId = false;
                            //IList<IInc2Contact>contacts = incidentRecord.Contact;
                            foreach (IInc2Contact con in incidentRecord.Contact)
                            {
                                if (con.Cid == id)
                                {
                                    foundId = true;
                                    con.Prmry = true;
                                }else{
                                    con.Prmry = false;
                                    if(con.Prmry == true)
                                        incidentRecord.Contact.Remove(con);
                                }
                            }
                            if (foundId == false)
                            {
                                IInc2Contact newContact = AddInViewsDataFactory.Create<IInc2Contact>();
                                newContact.Cid = id;
                                newContact.Prmry = true;
                                incidentRecord.Contact.Add(newContact);
                            }
                            
                            _rContext.RefreshWorkspace();
                            result = System.Windows.Forms.MessageBox.Show(this, "Record is refreshed.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            contactRecord = _rContext.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Contact) as IContact;

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
                siebelContactSearchButton.PerformClick();
            }
        }

        // Invoke by RightNow Workspace Rule
        public void autoSearchParameterInvoke()
        {
            string logMessage = "Invoke auto contact search";
            string logNote = "";
            _log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);

            if (_rContext.WorkspaceType != WorkspaceRecordType.Incident  &&  _rContext.WorkspaceType != WorkspaceRecordType.Contact)
            {
                if (this.chatRecord == null)
                    chatRecord = _rContext.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Chat) as IChat;

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

                if (chatRecord.Email != null || chatRecord.Email != "")
                {
                    emailTextBox.Text = chatRecord.Email;
                }
                phoneTextBox.Text = "";
            }

            if (_rContext.WorkspaceType == WorkspaceRecordType.Contact || _rContext.WorkspaceType == WorkspaceRecordType.Incident)
            {
                //IContact contactRecord = _rContext.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Contact) as IContact;
                if (this.contactRecord == null)
                {
                    contactRecord = _rContext.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Contact) as IContact;
                    if(contactRecord == null)
                        return;
                }
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
                this.siebelContactSearchButton_Click(null, EventArgs.Empty);

                logMessage = "Automatically search siebel contact. Search fields: phone = " + phoneTextBox.Text + "; email = " + emailTextBox.Text;
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

            if (_rContext.WorkspaceType == WorkspaceRecordType.Contact)
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
                    this.siebelContactSearchButton_Click("NoMessageBox", EventArgs.Empty);

                logMessage = "Automatically search siebel contact. Search fields: phone = " + phoneTextBox.Text;
                logNote = "";
                _log.DebugLog(_logIncidentId, _logContactId, logMessage, logNote);
            }

        }

        private void siebelContactSearchListView_MouseLeave(object sender, EventArgs e)
        {
            if (this.Cursor == differentCursor)
                this.Cursor = standardCursor;
        }

        //Change cursor when mouse is on the "Select" Column
        private void siebelContactSearchListView_MouseMove(object sender, MouseEventArgs e)
        {
            if (_rContext.WorkspaceType != WorkspaceRecordType.Incident  &&  _rContext.WorkspaceType != WorkspaceRecordType.Contact)
            {
                return;
            }

            bool found = false;
            ListViewItem.ListViewSubItem lvsi = null;
            int i = 0;
            while (!found && i < siebelContactSearchListView.Items.Count)
            {
                lvsi = siebelContactSearchListView.Items[i].GetSubItemAt(e.Location.X, e.Location.Y);
                if (lvsi == siebelContactSearchListView.Items[i].SubItems[6])
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
            if (_rContext.WorkspaceType != WorkspaceRecordType.Incident  &&  _rContext.WorkspaceType != WorkspaceRecordType.Contact)
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
