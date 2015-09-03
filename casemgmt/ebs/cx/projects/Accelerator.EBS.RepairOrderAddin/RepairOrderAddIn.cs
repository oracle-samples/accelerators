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
 *  SHA1: $Id: a80b0d707e8caca7cfedcd1a71b9865c6128fe25 $
 * *********************************************************************************************
 *  File: RepairOrderAddIn.cs
 * *********************************************************************************************/

using Accelerator.EBS.SharedServices;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.AddIn;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using RightNow.AddIns.AddInViews;
using System.ServiceModel;
using Accelerator.EBS.RepairOrderAddin.RightNowServiceReference;
using System.ServiceModel.Channels;
using System.Text.RegularExpressions;
using System.ComponentModel;
using Accelerator.EBS.SharedServices.Logs;


namespace Accelerator.EBS.RepairOrderAddin
{
    public class RepairOrderAddIn : Panel, IWorkspaceComponent2
    {
        /// <summary>
        /// The current workspace record context.
        /// </summary>
        private IRecordContext _recordContext;
        public IGlobalContext GlobalContext { get; set; }
        public IIncident Incident { get; set; }
        public IContact Contact { get; set; }
        public LogWrapper _log;
        public int _logIncidentId;

        private RepairOrderInformationControl _repairOrderControl;
       
        private string selected_status, stored_status;
        private int selected_status_id, stored_status_id;

        public int _ebsDefaultSrOwnerId;
        private decimal currentInventoryItemID;
        private string current_serial_num;

        private bool isEnabledEditing;
        private bool creatingRO = false;
        ///
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="inDesignMode">Flag which indicates if the control is being drawn on the Workspace Designer. (Use this flag to determine if code should perform any logic on the workspace record)</param>
        /// <param name="RecordContext">The current workspace record context.</param>
        public RepairOrderAddIn(bool inDesignMode, IRecordContext RecordContext, bool isEnabledEditing)
        {
              // do nothing so framework won't throw exception once it gets to GetControl
            if (!ConfigurationSetting.configVerbPerfect && ConfigurationSetting.loginUserIsAdmin)
            {
                // do nothing
            }
            else
            {
                _recordContext = RecordContext;
                this.isEnabledEditing = isEnabledEditing;
                if (_recordContext != null)
                {
                    _recordContext.DataLoaded += _recordContext_DataLoaded;
                    //Add saving/saved events
                    _recordContext.Saving += _recordContext_Saving;
                    _recordContext.Saved += _recordContext_Saved;
                }
                _repairOrderControl = new RepairOrderInformationControl(inDesignMode, isEnabledEditing);
            }
        }

        #region IAddInControl Members

        /// <summary>
        /// Method called by the Add-In framework to retrieve the control.
        /// </summary>
        /// <returns>The control, typically 'this'.</returns>
        public Control GetControl()
        {
            // return empty control so framework won't throw exception
            if (!ConfigurationSetting.configVerbPerfect && ConfigurationSetting.loginUserIsAdmin)
                return new Control();

            else
            // Add an event handler for the custom control
                return _repairOrderControl;
        }

        #endregion

        #region IWorkspaceComponent2 Members

        /// <summary>
        /// Sets the ReadOnly property of this control.
        /// </summary>
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Method which is called when any Workspace Rule Action is invoked.
        /// </summary>
        /// <param name="ActionName">The name of the Workspace Rule Action that was invoked.</param>
        public void RuleActionInvoked(string ActionName)
        {
            if (ActionName == "StartCreatingRO")
            {
                this.creatingRO = true;
            }
            else if (ActionName == "CancelCreatingRO")
            {
                this.creatingRO = false;
            }
        }

        /// <summary>
        /// Method which is called when any Workspace Rule Condition is invoked.
        /// </summary>
        /// <param name="ConditionName">The name of the Workspace Rule Condition that was invoked.</param>
        /// <returns>The result of the condition.</returns>
        public string RuleConditionInvoked(string ConditionName)
        {
            return string.Empty;
        }

        #endregion

        internal void EnableControl(IIncident incident)
        {
            //get current incident record and its contact
            this.Incident = incident;

            /// Set reference incident in log 
            this._logIncidentId = incident.ID;
            this.creatingRO = false;

            this.Controls.Clear();

            //Set repair order control and load it
            _repairOrderControl.Name = "RepairOrderInfo";
            _repairOrderControl.incident = incident;
            _repairOrderControl._gc = GlobalContext;
            _repairOrderControl._rc = _recordContext;
            _repairOrderControl._log = _log;
            _repairOrderControl._logIncidentId = _logIncidentId;

            _repairOrderControl.LoadInfo();
        }

        void _recordContext_DataLoaded(object sender, System.EventArgs e)
        {
            try
            {
                IIncident i = _recordContext.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Incident) as IIncident;
                this.EnableControl(i);
            }
            catch (Exception ex)
            {
                string logMessage = "Error in loading Repair Order. Cancel Async Thread. Error: " + ex.Message;
                string logNote = "";
                _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);
            }
        }

        void _recordContext_Saved(object sender, EventArgs e)
        {
            if (!isEnabledEditing && this.creatingRO == false)
            {
                return;
            }
            string logMessage, logNote;
            //get current sr id
            var sr_id = "";
            IList<ICustomAttribute> incCustomAttributes = Incident.CustomAttributes;
            string[] incCustomAttrs = { "Accelerator$ebs_sr_id" };
            Dictionary<String, Object> incCustomAttrsResults = CustomAttrHelper.fetchCustomAttrValue(incCustomAttributes, incCustomAttrs, this._logIncidentId, 0);
            sr_id = incCustomAttrsResults["Accelerator$ebs_sr_id"] != null ? incCustomAttrsResults["Accelerator$ebs_sr_id"].ToString() : "";
            
            //Check whether need to send request to server
            if (!String.IsNullOrWhiteSpace(sr_id) && _repairOrderControl.isAddingRO == true)
            {
                //Add new RO to server
                logMessage = "Need to add a Repair Order for SR. SR ID = " + sr_id;
                logNote = "";
                _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);

                SaveRepairOrderToServer(Convert.ToDecimal(sr_id));
                _repairOrderControl.isAddingRO = false;
            }
            else if (!String.IsNullOrWhiteSpace(_repairOrderControl.changingRONum) && _repairOrderControl.isChangingRO == true)
            {
                //Update RO to server
                logMessage = "Need to edit Repair Order. Repair Order id = " + _repairOrderControl.changingRONum;
                logNote = "";
                _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);

                SaveRepairOrderToServer(Convert.ToDecimal(sr_id));
                _repairOrderControl.isChangingRO = false;
            }


        }

        void _recordContext_Saving(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this._repairOrderControl.LeaveFocusWhenSaving();
            string logMessage, logNote;
            Contact = _recordContext.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Contact) as IContact;
            if (Contact == null)
            {
                _repairOrderControl.tbSerialNo.Enabled = true;
                DialogResult result = MessageBox.Show("Contact is empty. Cannot do the serial number validation. ", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (result == DialogResult.OK)
                {
                    logMessage = "Contact is empty. Cannot do the serial number validation. ";
                    logNote = "";
                    _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);
                    e.Cancel = true;
                    return;
                }
            }
            current_serial_num = _repairOrderControl.tbSerialNo.Text;
            //Validate serial number
            bool isValidSerial;

            if (current_serial_num != "")
            {
                try
                {
                    isValidSerial = this.validateSerialNumber(current_serial_num);
                }
                catch (Exception ex)
                {
                    logMessage = "Incident saving is cancelled, because error: " + ex.Message;
                    _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage);
                    e.Cancel = true;
                    return;
                }

                currentInventoryItemID = _repairOrderControl.inputInventoryItemID;
                if (!isValidSerial)
                {
                    //Invalid Error
                    logMessage = "The serial number (" + current_serial_num + ") is invalid. It does not belong to current contact's organization.";
                    logNote = "";
                    _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);

                    string message = "The serial number is invalid. It does not belong to current contact's organization.";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    DialogResult result;

                    // Show message box to mention the assigned contact party id
                    result = MessageBox.Show(this, message, "Error", buttons, MessageBoxIcon.Error);
                    if (result == DialogResult.OK)
                    {
                        //current_serial_num = ebsStoredSerialNum;
                        //currentInstanceID = ebsStoredInstanceID;
                        e.Cancel = true;
                        return;
                    }
                }
            }
            else
            {
                currentInventoryItemID = 6761;
            }

            // get current selected repair status id 
            switch (_repairOrderControl.SelectedRepairStatus)
            {
                case "Open":
                    selected_status = "Open";
                    selected_status_id = 1002;
                    break;
                case "Hold":
                    selected_status = "Hold";
                    selected_status_id = 1001;
                    break;
                case "Close":
                    selected_status = "Close";
                    selected_status_id = 1000;
                    break;
                case "Item Received":
                    selected_status = "Item Received";
                    selected_status_id = 1140;
                    break;
                case "Item Shipped":
                    selected_status = "Item Shipped";
                    selected_status_id = 1141;
                    break;
                case"Awaiting Estimate Approval":
                    selected_status = "Awaiting Estimate Approval";
                    selected_status_id = 1142;
                    break;
                case "Awaiting QA":
                    selected_status = "Awaiting QA";
                    selected_status_id = 1143;
                    break;
                case "Awaiting Repair":
                    selected_status = "Awaiting Repair";
                    selected_status_id = 1144;
                    break;
                case "Awaiting Shipping":
                    selected_status = "Awaiting Shipping";
                    selected_status_id = 1145;
                    break;
                case "Estimate Approved":
                    selected_status = "Estimate Approved";
                    selected_status_id = 1146;
                    break;
                case "Estimate Rejected":
                    selected_status = "Estimate Rejected";
                    selected_status_id = 1147;
                    break;
                case "Exchange and Refurbish":
                    selected_status = "Exchange and Refurbish";
                    selected_status_id = 1148;
                    break;
                case "Repair Complete":
                    selected_status = "Repair Complete";
                    selected_status_id = 1149;
                    break;
                case "Repair In Progress":
                    selected_status = "Repair In Progress";
                    selected_status_id = 1150;
                    break;
                case "Awaiting Receipt":
                    selected_status = "Awaiting Receipt";
                    selected_status_id = 1151;
                    break;
                default:
                    selected_status = "Open";
                    selected_status_id = 1002;
                    break;
            }

            // get EBS stored repair status id 
            switch (_repairOrderControl.StoredRepairStatus)
            {
                case "Open":
                    stored_status = "Open";
                    stored_status_id = 1002;
                    break;
                case "Hold":
                    stored_status = "Hold";
                    stored_status_id = 1001;
                    break;
                case "Close":
                    stored_status = "Close";
                    stored_status_id = 1000;
                    break;
                case "Item Received":
                    stored_status = "Item Received";
                    stored_status_id = 1140;
                    break;
                case "Item Shipped":
                    stored_status = "Item Shipped";
                    stored_status_id = 1141;
                    break;
                case "Awaiting Estimate Approval":
                    stored_status = "Awaiting Estimate Approval";
                    stored_status_id = 1142;
                    break;
                case "Awaiting QA":
                    stored_status = "Awaiting QA";
                    stored_status_id = 1143;
                    break;
                case "Awaiting Repair":
                    stored_status = "Awaiting Repair";
                    stored_status_id = 1144;
                    break;
                case "Awaiting Shipping":
                    stored_status = "Awaiting Shipping";
                    stored_status_id = 1145;
                    break;
                case "Estimate Approved":
                    stored_status = "Estimate Approved";
                    stored_status_id = 1146;
                    break;
                case "Estimate Rejected":
                    stored_status = "Estimate Rejected";
                    stored_status_id = 1147;
                    break;
                case "Exchange and Refurbish":
                    stored_status = "Exchange and Refurbish";
                    stored_status_id = 1148;
                    break;
                case "Repair Complete":
                    stored_status = "Repair Complete";
                    stored_status_id = 1149;
                    break;
                case "Repair In Progress":
                    stored_status = "Repair In Progress";
                    stored_status_id = 1150;
                    break;
                case "Awaiting Receipt":
                    stored_status = "Awaiting Receipt";
                    stored_status_id = 1151;
                    break;
                default:
                    stored_status = "Open";
                    stored_status_id = 1002;
                    break;
            }
        }

        public bool validateSerialNumber(String serialNum)
        {
            IList<ICustomAttribute> conCustomAttributes = Contact.CustomAttributes;
            string[] conCustomAttrs = { "Accelerator$ebs_contact_org_id" };
            Dictionary<String, Object> conCustomAttrResults = CustomAttrHelper.fetchCustomAttrValue(conCustomAttributes, conCustomAttrs, this._logIncidentId, 0);
            string contactOrgId = conCustomAttrResults["Accelerator$ebs_contact_org_id"] != null ? conCustomAttrResults["Accelerator$ebs_contact_org_id"].ToString() : "0";
            if (contactOrgId == "0")
            {
                DialogResult result = MessageBox.Show("Current contact did not associate to a EBS contact correctly. Cannot do the serial number validation. ", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (result == DialogResult.OK)
                {
                    string logMessage = "Current contact did not associate to a EBS contact correctly. Cannot do the serial number validation.";
                    string logNote = "";
                    _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);
                    return false;
                }
            }
            try
            {
                bool isValid = _repairOrderControl.SerialNumberValidation(serialNum, contactOrgId);
                if (!isValid)
                    _repairOrderControl.SetValidedResult(isValid);
                return isValid;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        void SaveRepairOrderToServer(decimal sr_id)
        {
            string logMessage = "Create/Update RO.";
            string logNote = "";
            _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);

            string ro_saved_num;
            RepairOrder ro = null;
            ro = new RepairOrder();

            ro.ServiceRequestID = sr_id;
            if (_repairOrderControl.isAddingRO == true)
            {
                //Create Additional RO
                ro.Quantity = Convert.ToDecimal(_repairOrderControl.InputQuantity);
                ro.UnitOfMeasure = _repairOrderControl.SelectedUnit;
                ro.InventoryItemID = this.currentInventoryItemID;
                ro.SerialNumber = this.current_serial_num;
                ro.ApprovalRequired = _repairOrderControl.SelectedApprovalRequired;
                ro.Currency = "USD";
                ro.ResourceID = _ebsDefaultSrOwnerId;
                ro.ProblemDescription = _repairOrderControl.InputProblem;
                ro.HasValidSerialNumber = "Y";
            }
            else if (_repairOrderControl.isChangingRO == true)
            {
                //Edit RO
                ro.RepairNumber = _repairOrderControl.changingRONum;
                ro.RepairStatus = selected_status;
                ro.RepairStatusID = selected_status_id;
                ro.StoredRepairStatus = stored_status;
                ro.StoredRepairStatusID = stored_status_id;
            }
           

            try
            {
                if (_repairOrderControl.isChangingRO == true)
                {
                    logMessage = "Ready to update Repair Order. SR ID = " + sr_id + "; RO Number = " + ro.RepairLineID;
                    logNote = "";
                    _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);
                    ro_saved_num = ro.Update( _logIncidentId, 0);
                }
                else
                {
                    logMessage = "Ready to update Repair Order. RO Number = " + ro.RepairLineID;
                    logNote = "";
                    _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);

                    ro_saved_num = ro.Create(_logIncidentId, 0);
                }

            }
            catch (Exception ex)
            {
                string message = ex.Message;
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                logMessage = "Error in creating/updating Repair Order.Error Message: " + ex.Message;
                logNote = "";
                _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);

                return;
            }

            if (ro_saved_num == "-1")
            {
                //Cannot change status between statuses
                string message = ro.ErrorMessage;
                MessageBox.Show(message, "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);

                logMessage = "Error in creating/updating RepairOrder. " + message;
                _log.NoticeLog(incidentId: _logIncidentId, logMessage: logMessage);
                return;
            }else if (String.IsNullOrEmpty(ro_saved_num))
            {
                string message = "There has been an error communicating with EBS. Please check log for detail.";
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                logMessage = "Error in creating/updating RepairOrder. Response's error message: " + ro.ErrorMessage;
                logNote = "";
                _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);
                return;
            }
            else
            {
                // Save successfully
                logMessage = "Creating/updating Repair Order successfully. RO Num = " + ro.RepairLineID;
                logNote = "";
                _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);

                _repairOrderControl.previousRONum = ro_saved_num;

                if (!isEnabledEditing)
                {
                    string message = "Repair Order has been created in EBS. Do you want to refresh current Repair Order List?";
                    DialogResult result = MessageBox.Show(message, "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (result == DialogResult.Yes)
                    {
                        _recordContext.TriggerNamedEvent("CancelCreatingRO");
                        _recordContext.ExecuteEditorCommand(RightNow.AddIns.Common.EditorCommand.Refresh);
                    }
                }
            }
            
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (_recordContext != null))
            {
                // unsubscribe from all the events
                _recordContext.DataLoaded -= _recordContext_DataLoaded;
                _recordContext.Saved -= _recordContext_Saved;
                _recordContext.Saving -= _recordContext_Saving;
            }
            base.Dispose(disposing);
        }

    }

    [AddIn("Repair Order AddIn", Version = "1.0.0.0")]
    public class RepairOrderAddInFactory : IWorkspaceComponentFactory2
    {
        #region IWorkspaceComponentFactory2 Members
        private IRecordContext _rContext;
        private IGlobalContext _gContext;
        private RepairOrderAddIn _wsAddIn;
        private String _usr;
        private String _pwd;
        private int _ebsDefaultSrOwnerId;
        private LogWrapper _log;

        [ServerConfigProperty(DefaultValue = "true")]
        public bool isEnabledEditing{get; set;}

        /// <summary>
        /// Method which is invoked by the AddIn framework when the control is created.
        /// </summary>
        /// <param name="inDesignMode">Flag which indicates if the control is being drawn on the Workspace Designer. (Use this flag to determine if code should perform any logic on the workspace record)</param>
        /// <param name="RecordContext">The current workspace record context.</param>
        /// <returns>The control which implements the IWorkspaceComponent2 interface.</returns>
        public IWorkspaceComponent2 CreateControl(bool inDesignMode, IRecordContext RecordContext)
        {
            if (!ConfigurationSetting.configVerbPerfect)
            {
                if (!ConfigurationSetting.loginUserIsAdmin)
                {
                    MessageBox.Show("RepairOrderAddIn is not initialized properly. \nPlease contact your system administrator.\n You are now logged out.");
                    _gContext.Logout();
                }
                else // don't want to logout admin
                {
                    MessageBox.Show("RepairOrderAddIn is not loaded because of invalid configuration verb.");
                    return new RepairOrderAddIn(inDesignMode, RecordContext, isEnabledEditing);
                }
            }

            _rContext = RecordContext;
            string logMessage, logNote;
            if (!inDesignMode && _rContext != null)
            {
                // Set config according to custom configuration verb 
                ConfigurationSetting instance = ConfigurationSetting.Instance(_gContext);

                _usr = ConfigurationSetting.username;
                _pwd = ConfigurationSetting.password;
                _log = ConfigurationSetting.logWrap;
                _ebsDefaultSrOwnerId = ConfigurationSetting.ebsDefaultSrOwnerId;
                RepairOrder.ServiceProvider = ConfigurationSetting.EBSProvider;
                RepairOrder.CreateURL = ConfigurationSetting.CreateRepair_WSDL;
                RepairOrder.UpdateURL = ConfigurationSetting.UpdateRepair_WSDL;
                //RepairOrder.LookupURL = ConfigurationSetting.LookupRepair_WSDL;
                //RepairOrder.ListLookupURL = ConfigurationSetting.LookupRepairList_WSDL;
                RepairOrder.ListURL = ConfigurationSetting.RepairOrderList_WSDL;
                RepairOrder.ServiceUsername = _usr;
                RepairOrder.ServicePassword = _pwd;
                RepairOrder.ServiceClientTimeout = ConfigurationSetting.EBSServiceTimeout;
                RepairOrder.InitEBSProvider();
                logMessage = "Repair Order is initiated.";
                logNote = "";
                _log.DebugLog(logMessage: logMessage, logNote: logNote);
            }
            /*
            bool isEnabled = false;
            if (isEnabledEditing == "true")
            {
                isEnabled = true;
            }*/
            _wsAddIn = new RepairOrderAddIn(inDesignMode, _rContext, isEnabledEditing);
            _wsAddIn._log = _log;
            _wsAddIn._ebsDefaultSrOwnerId = _ebsDefaultSrOwnerId;
            if (_log != null)
            {
                logMessage = "Repair Order AddIn is setup.";
                logNote = "";
                _log.DebugLog(logMessage: logMessage, logNote: logNote);
            }
            return _wsAddIn;
        }

        #endregion

        #region IFactoryBase Members

        /// <summary>
        /// The 16x16 pixel icon to represent the Add-In in the Ribbon of the Workspace Designer.
        /// </summary>
        public Image Image16
        {
            get { return Properties.Resources.AddIn16; }
        }

        /// <summary>
        /// The text to represent the Add-In in the Ribbon of the Workspace Designer.
        /// </summary>
        public string Text
        {
            get { return "EBSRepairOrderWorkspaceAddIn"; }
        }

        /// <summary>
        /// The tooltip displayed when hovering over the Add-In in the Ribbon of the Workspace Designer.
        /// </summary>
        public string Tooltip
        {
            get { return "EBS Repair Order WorkspaceAddIn Tooltip"; }
        }

        #endregion

        #region IAddInBase Members

        /// <summary>
        /// Method which is invoked from the Add-In framework and is used to programmatically control whether to load the Add-In.
        /// </summary>
        /// <param name="GlobalContext">The Global Context for the Add-In framework.</param>
        /// <returns>If true the Add-In to be loaded, if false the Add-In will not be loaded.</returns>
        public bool Initialize(IGlobalContext GlobalContext)
        {
            _gContext = GlobalContext;
            ConfigurationSetting.logAssemblyVersion(Assembly.GetAssembly(typeof(RepairOrderAddInFactory)), null);

            ConfigurationSetting instance = ConfigurationSetting.Instance(_gContext);
            /* log it, but return true because will show the messagebox when the addin is opened in createControl.
             * if return false, the add-in is not loaded, and cannot show the error when add-in is opened.
             */
            if (!ConfigurationSetting.configVerbPerfect)
            {
                string logMessage = "RepairOrderAddIn is not initialized properly because of invalid config verb.";
                ConfigurationSetting.logWrap.ErrorLog(logMessage: logMessage);
            }

            return true;
        }

        #endregion
    }
}