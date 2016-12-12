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
 *  date: Thu Nov 12 00:52:46 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: 3b233e8302922b33a0753289ead58adce1ec39eb $
 * *********************************************************************************************
 *  File: ServiceRequestInformationControl.cs
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

namespace Accelerator.EBS.ServiceRequestAddin
{
    public partial class ServiceRequestInformationControl : UserControl
    {
        public IRecordContext _rc { get; set; }
        public IGlobalContext _gc { get; set; }
        public LogWrapper _log { get; set; }
        public int _logIncidentId = 0;
        public IIncident incident { get; set; }

        public string storedContactPartyId;
        public int storedSeverityId;
        public int storedRequestTypeId;
        public int storedRequestStatusId;
        public string storedSubject;
        public string ebsStoredSerialNum;
        public string ebsInventoryItemName;
        public string inputSerialNum;
        public decimal ebsStoredInstanceID;
        public decimal inputInstanceID;
        public bool ignoreValidation = false;

        private BackgroundWorker bw_srDetails= new BackgroundWorker();
        private BackgroundWorker bw_serialNumValidation = new BackgroundWorker();
        // Used for multi-threading
        delegate void SetTextCallback(Control c, string text);
        delegate void SetVisibilityCallback(Control c, bool visible);

        public ServiceRequestInformationControl()
        {
            InitializeComponent();

            bw_srDetails.WorkerSupportsCancellation = true;
            bw_srDetails.DoWork += new DoWorkEventHandler(bw_LoadSRDetails);
            bw_srDetails.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunLoadSRDetailsCompleted);

            bw_serialNumValidation.WorkerSupportsCancellation = true;
            bw_serialNumValidation.DoWork += new DoWorkEventHandler(bw_ValidateSerialNum);
            bw_serialNumValidation.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunValidateSerialNumCompleted);

        }

        public void LoadInfo()
        {
            try
            {
                this.tbSerialNo.Enabled = false;
                SetVisibility(this.lblInvalid, false);
                SetVisibility(this.lblValid, false);
                bw_srDetails.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                string logMessage = "Error in loading EBS Service Request. Cancel Async Thread. Error: " + ex.Message;
                _log.ErrorLog(incidentId:_logIncidentId, logMessage: logMessage);
                bw_srDetails.CancelAsync();
            }
        }

        private void bw_LoadSRDetails(object sender, DoWorkEventArgs e)
        {
            var sr_id = "";
            var sr_num = "";
            string logMessage, logNote;
            IList<ICustomAttribute> customAttributes = incident.CustomAttributes;
            string[] incCustomAttrs = { "Accelerator$ebs_sr_id", "Accelerator$ebs_sr_num", "Accelerator$ebs_serial_number" };
            Dictionary<String, Object> incCustomAttrsResults = CustomAttrHelper.fetchCustomAttrValue(customAttributes, incCustomAttrs, this._logIncidentId, 0);
            sr_id = incCustomAttrsResults["Accelerator$ebs_sr_id"] != null ? incCustomAttrsResults["Accelerator$ebs_sr_id"].ToString() : "";
            sr_num = incCustomAttrsResults["Accelerator$ebs_sr_num"] != null ? incCustomAttrsResults["Accelerator$ebs_sr_num"].ToString() : "";

            e.Result = null;
            if (!String.IsNullOrWhiteSpace(sr_id) || !String.IsNullOrWhiteSpace(sr_num))
            {
                // Call to SR Lookup and Display SR Details
                ServiceRequest sr = new ServiceRequest();
                try
                {
                    sr = sr.Lookup(Convert.ToDecimal(sr_id), sr_num, _logIncidentId, 0);
                }
                catch (Exception ex)
                {
                    e.Cancel = true;
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    logMessage = "Error in loading Service Request. Error: " + ex.Message;
                    logNote = "";
                    _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);
                    return;
                }

                if (sr.ErrorMessage != null)
                {
                    e.Cancel = true;
                    logMessage = "Loading Service Request is failed. SR ID = " + sr_id;
                    logNote = "Response shows error code when loading service request. Response's error message: " + sr.ErrorMessage;
                    _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);

                    return;
                }

                e.Result = sr;

                //Set stored value (need them to decide whether field is changed)
                storedContactPartyId = (sr.EbsContactID == "0") ? "" : sr.EbsContactID;
                storedSeverityId = Convert.ToInt32(sr.SeverityID == null ? 0 : sr.SeverityID);
                storedRequestStatusId = Convert.ToInt32(sr.StatusID == null ? 0 : sr.StatusID);
                storedRequestTypeId = Convert.ToInt32(sr.RequestTypeID == null ? 0 : sr.RequestTypeID);
                storedSubject = sr.Summary;
                ebsStoredSerialNum = (sr.SerialNumber == null ? "" : sr.SerialNumber);
                ebsStoredInstanceID = (decimal)(sr.ProductID == null ? 0 : sr.ProductID);

                bool sr_obj_ver_num = false;
                bool sr_owner_id = false;
                foreach (ICustomAttribute cusAttr in customAttributes)
                {
                    if (cusAttr.PackageName == "Accelerator" && cusAttr.GenericField.Name == "Accelerator$ebs_sr_obj_ver_num")
                    {
                        sr_obj_ver_num = true;
                        cusAttr.GenericField.DataValue.Value = Convert.ToInt32(sr.SrObjVerNum == null ? 0 : sr.SrObjVerNum);
                    }
                    if (cusAttr.PackageName == "Accelerator" && cusAttr.GenericField.Name == "Accelerator$ebs_sr_owner_id")
                    {
                        sr_owner_id = true;
                        cusAttr.GenericField.DataValue.Value = sr.OwnerID.HasValue ? Convert.ToString(sr.OwnerID) : "";
                    }

                }

                if (sr_obj_ver_num == false)
                {
                    logMessage = "Custom attribute is not defined. Cannot get Accelerator$ebs_sr_obj_ver_num.";
                    ConfigurationSetting.logWrap.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage);
                }

                if (sr_owner_id == false)
                {
                    logMessage = "Custom attribute is not defined. Cannot get Accelerator$ebs_sr_owner_id.";
                    ConfigurationSetting.logWrap.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage);
                }

                if (sr_obj_ver_num == false || sr_owner_id == false)
                {
                    MessageBox.Show("Custom Attribute configuration missing. Please check log for detail.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                logMessage = "Loaded Service Request. SR ID = " + sr_id;
                _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage);

            }
            else
            {
                //Display Empty page if no Service Request associated
                logMessage = "No Service Request associated. Show empty form.";
                logNote = "";
                _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);
            }
        }

        private void bw_RunLoadSRDetailsCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled == true))
            {
                this.setEmptySRLayout();
                this.tbSerialNo.Enabled = true;
            }

            else if (!(e.Error == null))
            {
                this.setEmptySRLayout();
                this.tbSerialNo.Enabled = true;
            }

            else
            {
                if(e.Result == null)
                {
                    //Display Empty page if no Service Request associated
                    this.setEmptySRLayout();
                    this.tbSerialNo.Enabled = true;
                }
                else
                {
                    ServiceRequest sr = (ServiceRequest)e.Result;
                    SetText(lblRequestType, sr.RequestType);
                    SetText(lblOwnerInfo, sr.Owner);
                    SetText(txtSummary, sr.Summary);
                    SetText(tbSerialNo, sr.SerialNumber);
                    SetText(lblSRNo, sr.RequestNumber);
                    SetText(lblStatus, sr.Status);
                    SetText(lblSeverity, sr.Severity);
                    SetText(lblProduct, sr.Product);
                    SetText(lblProdDescription, sr.ProductDescription);
                    this.tbSerialNo.Enabled = true;
                }
            }
        }

        private void setEmptySRLayout()
        {
            SetVisibility(gbServReqInfo, true);
            SetText(lblRequestType, String.Empty);
            SetText(lblSeverity, String.Empty);
            SetText(lblOwnerInfo, String.Empty);
            SetText(txtSummary, String.Empty);
            SetText(tbSerialNo, String.Empty);
            SetText(lblSRNo, String.Empty);
            SetText(lblStatus, String.Empty);
            SetText(lblProduct, String.Empty);
            SetText(lblProdDescription, String.Empty);
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
                this.bw_serialNumValidation.RunWorkerAsync(serialNum);
            }
            catch (Exception ex) {
                string logMessage = "Error in Validating EBS Service Request's Serial Number. Cancel Async Thread. Error: " + ex.Message;
                _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage);
                bw_serialNumValidation.CancelAsync();
            }
        }

        private void bw_ValidateSerialNum(object sender, DoWorkEventArgs e)
        {
            string serialNum = (string)e.Argument;
            string logMessage;
            IContact Contact = _rc.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Contact) as IContact;
            if (Contact == null)
            {
                e.Cancel = true;
                logMessage = "Contact is empty. Cannot do the serial number validation. ";
                _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage);
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
            }
        }

        public bool SerialNumberValidation(string serial_number, string org_id){
            decimal contact_org_id = Convert.ToDecimal(org_id);
            try
            {
                Item[] items = Item.LookupItemList(serial_number, contact_org_id, "F", _logIncidentId, 0);
                if (items.Length > 0)
                {
                    this.ebsInventoryItemName = items[0].InventoryItemName;
                    this.inputInstanceID = (decimal)items[0].InstanceID;
                    return true;
                }
                string logMessage = "The serial number validation is failed. (serial number = " + serial_number + ", org id = " + org_id + ")";
                _log.NoticeLog(incidentId: _logIncidentId, logMessage: logMessage);
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

        public void LeaveFocusWhenSaving()
        {
            this.ignoreValidation = true;
            this.lblSRNo.Focus();
            this.ignoreValidation = false;
        }
    }
}
