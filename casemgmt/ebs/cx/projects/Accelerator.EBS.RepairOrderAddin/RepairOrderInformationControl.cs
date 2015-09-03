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
 *  SHA1: $Id: 7b78186ff9b14c91e0b2e4b13fe6d208f7f7c4ea $
 * *********************************************************************************************
 *  File: RepairOrderInformationControl.cs
 * *********************************************************************************************/

using Accelerator.EBS.SharedServices;

using RightNow.AddIns.AddInViews;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Accelerator.EBS.SharedServices.Logs;

namespace Accelerator.EBS.RepairOrderAddin
{
    public partial class RepairOrderInformationControl : UserControl
    {

        public IRecordContext _rc { get; set; }
        public IGlobalContext _gc { get; set; }

        public IIncident incident { get; set; }
        //public IContact contact { get; set; }

        //Cursor for repair order list
        private Cursor standardCursor = Cursors.Arrow;
        private Cursor differentCursor = Cursors.Hand;
        private Cursor waitingCursor = Cursors.WaitCursor;

        // Used for multi-threading
        delegate void SetTextCallback(Control c, string text);
        delegate void SetVisibilityCallback(Control c, bool visible);

        //Editable fields value
        public string SelectedApprovalRequired { get; set; }
        public string SelectedApprovalStatus { get; set; }
        public string SelectedUnit { get; set; }
        public string InputQuantity { get; set; }
        public string InputProblem { get; set; }
        public string SelectedRepairStatus { get; set; }
        public string StoredRepairStatus { get; set; }

        public string inputSerialNum;
        public string ebsInventoryItemName;
        public decimal inputInventoryItemID;
        public bool ignoreValidation = false;

        //Adding/Editing RO flag
        public bool isAddingRO { get; set; }
        public bool isChangingRO { get; set; }

        //Get the previous display RO - if cancel adding/editing RO, show it
        public string changingRONum { get; set; }
        public string previousRONum { get; set; }

        public LogWrapper _log { get; set; }
        public int _logIncidentId;

        bool isEnabledEditing = false;

        private BackgroundWorker bw_roList = new BackgroundWorker();
        private BackgroundWorker bw_roDetails = new BackgroundWorker();
        private BackgroundWorker bw_serialNumValidation = new BackgroundWorker();
        public RepairOrderInformationControl(bool isDesign, bool isEnabledEditing)
        {
            InitializeComponent();
            this.isEnabledEditing = isEnabledEditing;
            if (!isEnabledEditing)
            {
                this.gbRepairOrder.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
                this.repairOrderListView.Visible = false;
                this.addROButton.Visible = false;
                this.label8.Visible = false;
                this.Height = this.Size.Height - this.repairOrderListView.Height;
            }
            else
            {
                if (!isDesign)
                {
                    this.SetVisibility(gbRepairOrder, false);
                    addROButton.Enabled = false;
                }

            }

            bw_roList.WorkerSupportsCancellation = true;
            bw_roList.DoWork += new DoWorkEventHandler(bw_LoadROList);
            bw_roList.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunLoadROListCompleted);

            bw_roDetails.WorkerSupportsCancellation = true;
            bw_roDetails.DoWork += new DoWorkEventHandler(bw_LoadRODetails);
            bw_roDetails.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunLoadRODetailsCompleted);

            bw_serialNumValidation.WorkerSupportsCancellation = true;
            bw_serialNumValidation.DoWork += new DoWorkEventHandler(bw_ValidateSerialNum);
            bw_serialNumValidation.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunValidateSerialNumCompleted);


        }


        public void LoadInfo()
        {
            // Disable UI component during loading
            try{
                if (!isEnabledEditing)
                {
                    this.isChangingRO = false;
                    this.changingRONum = null;
                    this.isAddingRO = true;
                    SetRepairOrder(null);
                }
                else
                {
                    this.enableOrDisableControlComponents(false);
                    this.bw_roList.RunWorkerAsync();
                }
            }
            catch (Exception ex)
            {
                string logMessage = "Error in Loading EBS Repair Order List. Cancel Async Thread. Error: " + ex.Message;
                _log.ErrorLog(incidentId:_logIncidentId, logMessage: logMessage);
                bw_roList.CancelAsync();
            }
        }


        // General method to display Repair Order UI
        // Case 1: If repair id is existed, retrieve Repair Order details and display
        // Case 2: If repair id is null, display creating Repair Order UI
        private void SetRepairOrder(string repairNum)
        {
            //this.enableOrDisableControlComponents(false);
            //Create Repair Order UI
            if (String.IsNullOrEmpty(repairNum))
            {
                SetVisibility(lblRepairNo, true);
                SetVisibility(lblApprovalRequired, false);
                SetVisibility(cbApprovalRequired, true);
                SetVisibility(lblQuantity, false);
                SetVisibility(tbQuantity, true);
                SetVisibility(lblUnit, false);
                SetVisibility(cbUnit, true);
                SetVisibility(textProblem, false);
                SetVisibility(textProblemInput, true);
                SetVisibility(gbRepairOrder, true);
                SetVisibility(lblRepairOrderStatus, true);
                SetVisibility(cbRepairOrderStatus, false);
                SetVisibility(tbSerialNo, true);
                SetVisibility(lblSerialNo, false);
                SetVisibility(lblInvalid, false);
                SetVisibility(lblValid, false);
                SetVisibility(addROButton, false);
                SetVisibility(editROButton, true);

                SetText(editROButton, "Cancel");
                SetText(lblRepairNo, String.Empty);
                SetText(lblApprovalRequired, String.Empty);
                SetText(lblQuantity, String.Empty);
                SetText(lblUnit, String.Empty);
                SetText(textProblem, String.Empty);
                SetText(lblRepairOrderStatus, String.Empty);
                SetText(lblSerialNo, String.Empty);
                SetText(tbSerialNo, String.Empty);
                SetText(lblProduct, String.Empty);
                SetText(lblProdDescription, String.Empty);

                SetText(cbApprovalRequired, "Y");
                SetText(tbQuantity, "1");
                SetText(cbUnit, "Ea");
                SetText(textProblemInput, String.Empty);

                if (this.isAddingRO == false)
                {
                    //Click "Cancel" button during inputing the first RO
                    SetVisibility(gbRepairOrder, false);
                    SetVisibility(addROButton, true);
                }
            }
            else
            {
                //Display RO
                SetVisibility(lblRepairNo, true);
                SetVisibility(lblApprovalRequired, true);
                SetVisibility(cbApprovalRequired, false);
                SetVisibility(lblQuantity, true);
                SetVisibility(tbQuantity, false);
                SetVisibility(lblUnit, true);
                SetVisibility(cbUnit, false);
                SetVisibility(textProblem, true);
                SetVisibility(textProblemInput, false);
                SetVisibility(gbRepairOrder, true);
                SetVisibility(lblRepairOrderStatus, true);
                SetVisibility(cbRepairOrderStatus, false);
                SetVisibility(tbSerialNo, false);
                SetVisibility(lblSerialNo, true);
                SetVisibility(lblInvalid, false);
                SetVisibility(lblValid, false);

                SetVisibility(addROButton, true);
                SetVisibility(editROButton, true);
                SetText(editROButton, "Edit Repair Order");

                try {
                    this.enableOrDisableControlComponents(false);
                    this.bw_roDetails.RunWorkerAsync(repairNum);
                }
                catch (Exception ex)
                {
                    string logMessage = "Error in Loading EBS Repair Order Details. Cancel Async Thread. Error: " + ex.Message;
                    _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage);
                    bw_roDetails.CancelAsync();
                }
            }
        }

        private void bw_LoadROList(object sender, DoWorkEventArgs e)
        {
            string logMessage, logNote;
            var sr_id = "";
            var sr_num = "";
            IList<ICustomAttribute> customAttributes = incident.CustomAttributes;
            string[] incCustomAttrs = { "Accelerator$ebs_sr_id", "Accelerator$ebs_sr_num" };
            Dictionary<String, Object> incCustomAttrsResults = CustomAttrHelper.fetchCustomAttrValue(customAttributes, incCustomAttrs, this._logIncidentId, 0);
            sr_id = incCustomAttrsResults["Accelerator$ebs_sr_id"] != null ? incCustomAttrsResults["Accelerator$ebs_sr_id"].ToString() : "";
            sr_num = incCustomAttrsResults["Accelerator$ebs_sr_num"] != null ? incCustomAttrsResults["Accelerator$ebs_sr_num"].ToString() : "";

            //Check incident whether associated to a service request
            if (!String.IsNullOrWhiteSpace(sr_id))
            {
                //If incident is related to a SR
                //Lookup repair order by service request id
                RepairOrder ro = new RepairOrder();

                RepairOrder[] results = null;
                try
                {
                    results = ro.LookupList(sr_id, sr_num, _logIncidentId, 0);
                    e.Result = results;
                }
                catch (Exception ex)
                {
                    e.Cancel = true;
                    bw_roList.CancelAsync();

                    string message = ex.Message;
                    MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    logMessage = "Error in loading Repair Order List. Error: " + ex.Message;
                    _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage);
                    return;
                }

                if (ro.ErrorMessage != null)
                {
                    e.Cancel = true;
                    logMessage = "Loading Repair Order List is failed. SR ID = " + sr_id;
                    logNote = "Response shows error code when loading repair order list. Response's error message: " + ro.ErrorMessage;
                    _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);
                    return;
                }
                else
                {
                    logMessage = "Loading Repair Order List Successfully. Current Service Request has " + results.Length + " Repair Order(s)";
                    logNote = "";
                    _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);
                }
            }
            else
            {
                e.Result = null;
            }
        }

        private void bw_RunLoadROListCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled == true))
            {
                this.enableOrDisableControlComponents(true);
            }

            else if (!(e.Error == null))
            {
                this.enableOrDisableControlComponents(true);
            }

            else
            {
                repairOrderListView.Items.Clear();
                
                if (e.Result == null)
                {
                    //No Service Request Related
                    this.SetVisibility(gbRepairOrder, false);
                    // Enable all UI component after loading
                    this.enableOrDisableControlComponents(true);
                    addROButton.Enabled = false;
                }else
                {
                    RepairOrder[] results = (RepairOrder[])e.Result;
                    //If there is no previous repair order id, show details of the 1st ro in the list
                    bool hasPreviousRepairOrderNum = false;

                    // Display return result in standard ListView
                    foreach (RepairOrder req in results)
                    {

                        ListViewItem item = null;
                        item = repairOrderListView.Items.Add(req.RepairNumber);
                        item.SubItems.Add(req.RepairType);
                        item.SubItems.Add(req.RepairStatus);
                        item.SubItems.Add(req.ProblemDescription);
                        item.UseItemStyleForSubItems = false;

                        //Add select action
                        item.SubItems.Add("Select");
                        item.SubItems[4].ForeColor = System.Drawing.Color.Blue;
                        item.SubItems[4].Font = new Font(item.Font, item.Font.Style | FontStyle.Underline);

                        if (req.RepairNumber== this.previousRONum)
                        {
                            hasPreviousRepairOrderNum = true;
                        }

                    }

                    //If have associated repair orders, display one RO details
                    if (results.Length > 0)
                    {
                        string repairNum;

                        // Display previous edited Repair Order or the first one in the list
                        if (hasPreviousRepairOrderNum == true)
                        {
                            repairNum = this.previousRONum;
                        }
                        else
                        {
                            repairNum = Convert.ToString(results[0].RepairNumber);
                        }

                        SetRepairOrder(repairNum);
                    }
                    else
                    {
                        //If there is no Repair Order of this Service Request, show empty space for RO detials
                        SetText(lblRepairNo, String.Empty);
                        SetText(lblApprovalRequired, String.Empty);
                        SetText(lblQuantity, String.Empty);
                        SetText(lblUnit, String.Empty);
                        SetText(lblSerialNo, String.Empty);
                        SetText(lblProduct, String.Empty);
                        SetText(lblProdDescription, String.Empty);

                        SetVisibility(addROButton, true);
                        SetVisibility(gbRepairOrder, false);
                    }


                    // Enable all UI component after loading
                    this.enableOrDisableControlComponents(true);
                }
            }
        }

        private void bw_LoadRODetails(object sender, DoWorkEventArgs e)
        {
            string logMessage, logNote;
            string repairNum = (string)e.Argument;
            RepairOrder ro = new RepairOrder();
            try
            {
                ro = ro.Lookup(repairNum, _logIncidentId, 0);
                e.Result = ro;
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                bw_roList.CancelAsync();

                string message = ex.Message;
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                logMessage = "Error in loading Repair Order. Error: " + ex.Message;
                _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage);
                return;
            }
            if (ro.ErrorMessage != null)
            {
                e.Cancel = true;
                logMessage = "Loading Repair Order is failed. RO Num = " + repairNum;
                logNote = "Response shows error code when loading repair order. Response's error message: " + ro.ErrorMessage;
                _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);
                return;
            }
        }

        private void bw_RunLoadRODetailsCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled == true))
            {
                this.enableOrDisableControlComponents(true);
            }

            else if (!(e.Error == null))
            {
                this.enableOrDisableControlComponents(true);
            }

            else
            {
                if (e.Result != null)
                {
                    RepairOrder ro = (RepairOrder)e.Result;
                    this.SetVisibility(lblValid, false);
                    this.SetVisibility(lblInvalid, false);
                    SetText(cbApprovalRequired, String.Empty);
                    SetText(tbQuantity, Convert.ToString(ro.Quantity));
                    SetText(cbUnit, String.Empty);
                    SetText(textProblemInput, String.Empty);
                    SetText(cbRepairOrderStatus, String.Empty);
                    SetText(tbSerialNo, String.Empty);

                    SetText(lblRepairNo, ro.RepairNumber);
                    SetText(lblApprovalRequired, ro.ApprovalRequired);
                    SetText(lblQuantity, Convert.ToString(ro.Quantity));
                    SetText(lblUnit, ro.UnitOfMeasure);
                    SetText(textProblem, ro.ProblemDescription);
                    SetText(lblRepairOrderStatus, ro.RepairStatus);
                    SetText(lblSerialNo, ro.SerialNumber);
                    SetText(lblProduct, ro.Product);
                    SetText(lblProdDescription, ro.ProdDescription);

                    this.previousRONum = ro.RepairNumber;
                }
                
                // Enable all UI component after loading
                this.enableOrDisableControlComponents(true);
            }
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

        // Select RO to display
        private void repairOrderListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.isAddingRO == true || this.isChangingRO == true)
            {
                // If select RO during editing a RO, ask for discarding current changes and display selected Repair Order
                string message = "Discard input information of repair order and display selected repair order?";
                MessageBoxButtons buttons = MessageBoxButtons.OKCancel;
                DialogResult result;

                // Show message box to mention the assigned contact party id
                result = System.Windows.Forms.MessageBox.Show(this, message, "Message", buttons, MessageBoxIcon.Question);
                if (result == DialogResult.OK)
                {
                    this.isAddingRO = false;
                    this.isChangingRO = false;
                    ListView.SelectedListViewItemCollection repairOrder = this.repairOrderListView.SelectedItems;
                    String repairNum = "";

                    //
                    //Get Column values
                    //
                    foreach (ListViewItem item in repairOrder)
                    {
                        repairNum = item.SubItems[0].Text;
                        break;
                    }
                    SetRepairOrder(repairNum);
                }
            }
            else
            {
                ListView.SelectedListViewItemCollection repairOrder = this.repairOrderListView.SelectedItems;
                String repairNum = "";

                //
                //Get Column values
                //
                foreach (ListViewItem item in repairOrder)
                {
                    repairNum = item.SubItems[0].Text;
                    break;
                }
                SetRepairOrder(repairNum);
            }

        }

        // Add Repair Order button - Add new RO to current Service Request
        private void addROButton_Click(object sender, EventArgs e)
        {
            if (this.isChangingRO == true)
            {
                string message = "Discard edited information of repair order and add a new repair order?";
                MessageBoxButtons buttons = MessageBoxButtons.OKCancel;
                DialogResult result;

                // Show message box to mention the assigned contact party id
                result = System.Windows.Forms.MessageBox.Show(this, message, "Message", buttons, MessageBoxIcon.Question);
                if (result == DialogResult.OK)
                {
                    this.isChangingRO = false;
                    this.changingRONum = null;
                    this.isAddingRO = true;
                    SetRepairOrder(null);
                }
            }
            else
            {
                // If click add RO button when editing, discard current editing and show empty form for adding RO 
                this.isChangingRO = false;
                this.changingRONum = null;
                this.isAddingRO = true;
                SetRepairOrder(null);
            }
        }

        // Edit Repair Order Button Event
        // Case 1: Work as Edit button, after clicking, edit current displayed RO
        // Case 2: Work as Cancel button, after clicking, cancel on-going change and display RO as previous version
        private void editROButton_Click(object sender, EventArgs e)
        {
            // Enable "Cancel" function during editing
            if (editROButton.Text == "Cancel")
            {
                if (!isEnabledEditing)
                {
                    this.changingRONum = null;
                    this.isAddingRO = true;
                    this.isChangingRO = false;
                    this.previousRONum = null;
                    SetRepairOrder(null);
                    _rc.TriggerNamedEvent("CancelCreatingRO");
                }
                else
                {
                    this.changingRONum = null;
                    this.isAddingRO = false;
                    this.isChangingRO = false;
                    SetRepairOrder(this.previousRONum);
                }
                return;
            }
            string currentRoNum = lblRepairNo.Text;
            //Edit RO UI
            if (!String.IsNullOrEmpty(currentRoNum))
            {
                this.changingRONum = currentRoNum;
                this.isAddingRO = false;
                this.isChangingRO = true;
                this.StoredRepairStatus = lblRepairOrderStatus.Text;
                SetText(cbRepairOrderStatus, lblRepairOrderStatus.Text);
                SetVisibility(lblRepairOrderStatus, false);
                SetVisibility(cbRepairOrderStatus, true);
                SetVisibility(addROButton, false);
                SetVisibility(editROButton, true);
                SetText(editROButton, "Cancel");
                this.InputQuantity = lblQuantity.Text;
            }
        }

        //Change values in Repair Order fields

        private void cbApprovalRequired_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SelectedApprovalRequired = cbApprovalRequired.SelectedItem.ToString();
        }


        private void cbRepairOrderStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SelectedRepairStatus = cbRepairOrderStatus.SelectedItem.ToString();
        }


        private void tbQuantity_TextChanged(object sender, EventArgs e)
        {
            this.InputQuantity = tbQuantity.Text;
        }

        private void cbUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SelectedUnit = cbUnit.SelectedItem.ToString();
        }

        private void textProblemInput_TextChanged(object sender, EventArgs e)
        {
            this.InputProblem = textProblemInput.Text;
        }



        //Cursor-change event
        private void repairOrderListView_MouseLeave(object sender, EventArgs e)
        {
            if (this.Cursor == differentCursor)
                this.Cursor = standardCursor;
        }

        private void repairOrderListView_MouseMove(object sender, MouseEventArgs e)
        {
            bool found = false;
            ListViewItem.ListViewSubItem lvsi = null;
            int i = 0;
            while (!found && i < repairOrderListView.Items.Count)
            {
                lvsi = repairOrderListView.Items[i].GetSubItemAt(e.Location.X, e.Location.Y);
                if (lvsi == repairOrderListView.Items[i].SubItems[4])
                    found = true;
                i++;
            }
            if (found)
                this.Cursor = differentCursor;
            else if (this.Cursor == differentCursor)
                this.Cursor = standardCursor;
        }
        public void enableOrDisableControlComponents(bool enableState)
        {
            this.Cursor = (enableState == true) ? standardCursor : waitingCursor;
            gbRepairOrder.Enabled = enableState;
            repairOrderListView.Enabled = enableState;
            addROButton.Enabled = enableState;
        }

        #region
        private void tbSerialNo_LostFocus(object sender, EventArgs e)
        {
            if (this.ignoreValidation == false)
            {
                this.inputSerialNum = tbSerialNo.Text;
                checkSerialNum();
            }
        }

        
        private void tbSerialNo_EnterKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                this.ignoreValidation = true;
                this.inputSerialNum = tbSerialNo.Text;
                checkSerialNum();
                this.ignoreValidation = false;
            }
        }

        private void checkSerialNum()
        {
            string logMessage, logNote;
            String serialNum = this.inputSerialNum;
            this.SetVisibility(lblValid, false);
            this.SetVisibility(lblInvalid, false);
            if (String.IsNullOrWhiteSpace(serialNum))
            {
                return;
            }
            try
            {
                this.tbSerialNo.Enabled = false;
                this.editROButton.Enabled = false;
                this.repairOrderListView.Enabled = false;
                this.bw_serialNumValidation.RunWorkerAsync(serialNum);
            }
            catch (Exception ex)
            {
                logMessage = "Error in Validating EBS Repair Order's Serial Number. Cancel Async Thread. Error: " + ex.Message;
                logNote = "";
                _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);
                bw_serialNumValidation.CancelAsync();
            }
        }

        private void bw_ValidateSerialNum(object sender, DoWorkEventArgs e)
        {
            string logMessage, logNote;
            string serialNum = (string)e.Argument;
            IContact Contact = _rc.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Contact) as IContact;
            if (Contact == null)
            {
                e.Cancel = true;
                logMessage = "Contact is empty. Cannot do the serial number validation. ";
                logNote = "";
                _log.DebugLog(incidentId: _logIncidentId,  logMessage: logMessage, logNote: logNote);
                e.Result = logMessage;
                return;
            }
            IList<ICustomAttribute> conCustomAttributes = Contact.CustomAttributes;
            string[] conCustomAttrs = { "Accelerator$ebs_contact_org_id" };
            Dictionary<String, Object> conCustomAttrResults = CustomAttrHelper.fetchCustomAttrValue(conCustomAttributes, conCustomAttrs, this._logIncidentId, 0);
            string contactOrgId = conCustomAttrResults["Accelerator$ebs_contact_org_id"] != null ? conCustomAttrResults["Accelerator$ebs_contact_org_id"].ToString() : "0";
            
            if (contactOrgId == "0")
            {
                logMessage = "Current contact did not associate to a EBS contact correctly. Cannot do the serial number validation.";
                _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage);
                e.Result = null;
                return;
            }

            try
            {
                bool isValid = SerialNumberValidation(serialNum, contactOrgId);
                e.Result = isValid;
            }
            catch (Exception ex)
            {
                logMessage = "Serial Number Validation thread is cancelled, because error: " + ex.Message;
                _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage);
                e.Cancel = true;
            }
 
        }

        private void bw_RunValidateSerialNumCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled == true))
            {
                this.tbSerialNo.Enabled = true;           
            }

            else if (!(e.Error == null))
            {
                this.tbSerialNo.Enabled = true;
                this.editROButton.Enabled = true;
                this.repairOrderListView.Enabled = true;
            }

            else
            {
                if (e.Result == null)
                {
                    String msg = "Current contact is not set or did not associate to a EBS contact correctly. Cannot do the serial number validation.";
                    DialogResult result = MessageBox.Show(msg, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (result == DialogResult.OK)
                    {
                        this.tbSerialNo.Enabled = true;
                    }
                    return;
                }
                bool isValid = (bool)e.Result;
                this.SetValidedResult(isValid);
                this.tbSerialNo.Enabled = true;
                this.editROButton.Enabled = true;
                this.repairOrderListView.Enabled = true;
            }
        }

        public bool SerialNumberValidation(string serial_number, string org_id)
        {
            decimal contact_org_id = Convert.ToDecimal(org_id);
            try
            {
                Item[] items = Item.LookupItemList(serial_number, contact_org_id, "F", _logIncidentId, 0);
                if (items.Length > 0)
                {
                    this.ebsInventoryItemName = items[0].InventoryItemName;
                    this.inputInventoryItemID = (decimal)items[0].InventoryItemID;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                string mesg = ex.Message;
                MessageBox.Show(mesg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw ex;
            }
        }

        public void SetValidedResult(bool isValid)
        {
            //If still in editing mode
            if (isAddingRO || isChangingRO)
            {
                if (isValid)
                {
                    this.SetVisibility(lblValid, true);
                    this.SetVisibility(lblInvalid, false);
                }
                else
                {
                    this.SetVisibility(lblValid, false);
                    this.SetVisibility(lblInvalid, true);
                }
            }
        }

        public void LeaveFocusWhenSaving()
        {
            this.ignoreValidation = true;
            this.lblRepairNo.Focus();
            this.ignoreValidation = false;
        }

        #endregion
    } 
}
