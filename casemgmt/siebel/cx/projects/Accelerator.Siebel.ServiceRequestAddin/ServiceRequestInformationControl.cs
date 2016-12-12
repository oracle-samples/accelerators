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
 *  SHA1: $Id: 9e2e476fde7d37e09de8ce288958154a98bfb08e $
 * *********************************************************************************************
 *  File: ServiceRequestInformationControl.cs
 * *********************************************************************************************/

using Accelerator.Siebel.SharedServices;

using RightNow.AddIns.AddInViews;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Accelerator.Siebel.SharedServices.Logs;

namespace Accelerator.Siebel.ServiceRequestAddin
{
    public partial class ServiceRequestInformationControl : UserControl
    {
        public IRecordContext _rc { get; set; }
        public IGlobalContext _gc { get; set; }
        public LogWrapper _log { get; set; }
        public int _logIncidentId = 0;
        public IIncident incident { get; set; }

        public string storedContactPartyId;
        public string storedSeverity;
        public string storedRequestType;
        public string storedRequestStatus;
        public string storedSubject;
        public string siebelStoredSerialNum;
        public string siebelProductId;
        public string inputSerialNum;
        public string siebelStoredProductID;
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
                string logMessage = "Error in loading Siebel Service Request. Cancel Async Thread. Error: " + ex.Message;
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
            string[] incCustomAttrs = { "Accelerator$siebel_sr_id", "Accelerator$siebel_sr_num", "Accelerator$siebel_serial_number" };
            Dictionary<String, Object> incCustomAttrsResults = CustomAttrHelper.fetchCustomAttrValue(customAttributes, incCustomAttrs, this._logIncidentId, 0);
            sr_id = incCustomAttrsResults["Accelerator$siebel_sr_id"] != null ? incCustomAttrsResults["Accelerator$siebel_sr_id"].ToString() : "";
            sr_num = incCustomAttrsResults["Accelerator$siebel_sr_num"] != null ? incCustomAttrsResults["Accelerator$siebel_sr_num"].ToString() : "";

            e.Result = null;
            if (!String.IsNullOrWhiteSpace(sr_id))
            {
                // Call to SR Lookup and Display SR Details
                ServiceRequest sr = new ServiceRequest();
                try
                {
                    sr = sr.Lookup(sr_id,  _logIncidentId, 0);
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                    MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    e.Cancel = true;
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
                storedContactPartyId = (sr.ContactID == "0") ? "" : sr.ContactID;
                storedSeverity = sr.Severity;
                storedRequestStatus = sr.Status;
                storedRequestType =  sr.RequestType;
                storedSubject = sr.Summary;
                siebelStoredSerialNum = (sr.SerialNumber == null ? "" : sr.SerialNumber);
                siebelStoredProductID = (sr.ProductID == null ? "" : sr.ProductID);

                bool sr_owner_id = false;
                bool sr_num_ca = false;
                foreach (ICustomAttribute cusAttr in customAttributes)
                {
                    if (cusAttr.PackageName == "Accelerator" && cusAttr.GenericField.Name == "Accelerator$siebel_sr_owner_id")
                    {
                        sr_owner_id = true;
                        cusAttr.GenericField.DataValue.Value = !String.IsNullOrEmpty(sr.OwnerID)? sr.OwnerID : "";
                    }
                    if (cusAttr.PackageName == "Accelerator" && cusAttr.GenericField.Name == "Accelerator$siebel_sr_num")
                    {
                        sr_num_ca = true;
                        cusAttr.GenericField.DataValue.Value = !String.IsNullOrEmpty(sr.RequestNumber) ? sr.RequestNumber : "";
                    }

                }
                if (sr_num_ca == false)
                {
                    logMessage = "Custom attribute is not defined. Cannot get Accelerator$siebel_sr_num.";
                    ConfigurationSetting.logWrap.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage);
                }

                if (sr_owner_id == false)
                {
                    logMessage = "Custom attribute is not defined. Cannot get Accelerator$siebel_sr_owner_id.";
                    ConfigurationSetting.logWrap.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage);
                }

                if (sr_owner_id == false||sr_num_ca == false)
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
            this.ignoreValidation = true;
            if (e.KeyChar == (char)13)
            {
                this.inputSerialNum = tbSerialNo.Text;
                checkSerialNum();
            }
            this.ignoreValidation = true;
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
                string logMessage = "Error in Validating Siebel Service Request's Serial Number. Cancel Async Thread. Error: " + ex.Message;
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
            string[] conCustomAttrs = { "Accelerator$siebel_contact_org_id" };
            Dictionary<String, Object> conCustomAttrResults = CustomAttrHelper.fetchCustomAttrValue(conCustomAttributes, conCustomAttrs, this._logIncidentId, 0);
            string contactOrgId = conCustomAttrResults["Accelerator$siebel_contact_org_id"] != null ? conCustomAttrResults["Accelerator$siebel_contact_org_id"].ToString() : "0";

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
            String contact_org_id = org_id;
            try
            {
                Asset asset = Asset.SerialNumberValidation(serial_number, contact_org_id, _logIncidentId, 0);
                if (asset != null)
                {
                    this.siebelProductId = asset.ProductID;
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
