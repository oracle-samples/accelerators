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
 *  date: Thu Nov 12 00:55:35 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: ac8d23ef75680893a7320f64d65234b5dd4440ca $
 * *********************************************************************************************
 *  File: ServiceRequestAddIn.cs
 * *********************************************************************************************/

using Accelerator.Siebel.SharedServices;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.AddIn;
using System.Drawing;
using System.Windows.Forms;
using RightNow.AddIns.AddInViews;
using System.ServiceModel;
using Accelerator.Siebel.SharedServices.RightNowServiceReference;
using Accelerator.Siebel.SharedServices.Logs;
using System.ServiceModel.Channels;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Net;
using System.Reflection;

namespace Accelerator.Siebel.ServiceRequestAddin
{
    public class ServiceRequestAddIn : Panel, IWorkspaceComponent2
    {
        /// <summary>
        /// The current workspace record context.
        /// </summary>
        private IRecordContext _recordContext;
        public IGlobalContext GlobalContext { get; set; }
        public IIncident Incident { get; set; }
        public IContact Contact { get; set; }

        private ServiceRequestInformationControl _serviceRequestControl;

        //Case Management System stored value
        private string storedContactPartyId;
        private string storedSeverity;
        private string storedRequestType;
        private string storedRequestStatus;
        private string storedSubject;
        private string siebelStoredSerialNum;
        private string rnStoredSerialNum;
        private string siebelStoredProductID;
        
        private int selected_rn_severity_id, selected_rn_request_type_id, selected_rn_request_status_id;
        private string selected_server_severity, selected_server_request_type, selected_server_request_status, current_serial_num, current_subject;        
        
        private string selectedContactPartyId;
        private string selectedContactOrgId;
        private int currentContactID;
        private string currentProductID;

        //public RightNowSyncPortClient _rnowClient;
        public RightNowService _rnSrv;
        public LogWrapper _log;
        public int _logIncidentId;
        public string _siebelServiceUserId;
        public string _siebelDefaultSrOwnerId;
        private string notesTruncated; // note truncated content list

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="inDesignMode">Flag which indicates if the control is being drawn on the Workspace Designer. (Use this flag to determine if code should perform any logic on the workspace record)</param>
        /// <param name="RecordContext">The current workspace record context.</param>
        public ServiceRequestAddIn(bool inDesignMode, IRecordContext RecordContext)
        {
            // do nothing so framework won't throw exception once it gets to GetControl
            if (!ConfigurationSetting.configVerbPerfect && ConfigurationSetting.loginUserIsAdmin)
            {
                // do nothing
            }
            else
            {
                _recordContext = RecordContext;
                if (_recordContext != null)
                {
                    _recordContext.DataLoaded += _recordContext_DataLoaded;
                    _recordContext.Saving += _recordContext_Saving;
                    _recordContext.Saved += _recordContext_Saved;
                }
                _serviceRequestControl = new ServiceRequestInformationControl();
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
                return this._serviceRequestControl;
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
            this.Incident = incident;

            /// Set reference incident in log 
            this._logIncidentId = incident.ID;

            this.currentContactID = 0;
            foreach (IInc2Contact c in incident.Contact)
            {
                if (c.Prmry == true)
                {
                    this.currentContactID = (int)c.Cid;
                }
            }
            //this.currentContactID = (Contact == null)?0:incident.ID;

            this.Controls.Clear();

            _serviceRequestControl.Name = "ServiceRequestInfo";
            _serviceRequestControl.incident = incident;
            _serviceRequestControl._gc = GlobalContext;
            _serviceRequestControl._rc = _recordContext;
            _serviceRequestControl._log = _log;
            _serviceRequestControl._logIncidentId = _logIncidentId;

            _serviceRequestControl.LoadInfo();
            
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
                string logMessage = "Error in loading Service Request. Cancel Async Thread. Error: " + ex.Message;
                _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage);
            }
        }

        void _recordContext_Saving(object sender, CancelEventArgs e)
        {
            string logMessage, logNote;
            storedContactPartyId = _serviceRequestControl.storedContactPartyId;
            storedSeverity = _serviceRequestControl.storedSeverity;
            storedRequestStatus = _serviceRequestControl.storedRequestStatus;
            storedRequestType = _serviceRequestControl.storedRequestType;
            storedSubject = _serviceRequestControl.storedSubject;
            siebelStoredSerialNum = _serviceRequestControl.siebelStoredSerialNum;
            siebelStoredProductID = _serviceRequestControl.siebelStoredProductID;

            this._serviceRequestControl.LeaveFocusWhenSaving();

            Contact = _recordContext.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Contact) as IContact;
            if (Contact == null)
            {
                _serviceRequestControl.tbSerialNo.Enabled = true;
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

            // get custom attribute Accelerator$siebel_sr_id value
            var sr_id = "";

            IList<ICustomAttribute> incCustomAttributes = Incident.CustomAttributes;
            string[] incCustomAttrSrId = { "Accelerator$siebel_sr_id", "Accelerator$siebel_serial_number"};
            Dictionary<String, Object> incCustomAttrSrIdResult = CustomAttrHelper.fetchCustomAttrValue(incCustomAttributes, incCustomAttrSrId, this._logIncidentId, 0);
            sr_id = incCustomAttrSrIdResult["Accelerator$siebel_sr_id"] != null ? incCustomAttrSrIdResult["Accelerator$siebel_sr_id"].ToString() : "";
            rnStoredSerialNum = !String.IsNullOrWhiteSpace((string)incCustomAttrSrIdResult["Accelerator$siebel_serial_number"])? incCustomAttrSrIdResult["Accelerator$siebel_serial_number"].ToString() : "";

            // If Incident's Contact has associated to an Siebel Contact, it does not support to edit it.
            if (!String.IsNullOrWhiteSpace(sr_id) && currentContactID != Contact.ID)
            {
                string message = "This incident has been associated to a Siebel Contact, we cannot allow changing the contact via this addin. Please change the primary contact back.";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                // Show message box to mention the assigned contact party id
                result = System.Windows.Forms.MessageBox.Show(this, message, "Message", buttons, MessageBoxIcon.Information);

                logMessage = "Cannot changing incident's primary contact. This incident has been associated to an Siebel Contact, Contact ID = " + Contact.ID;
                logNote = "";
                _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);

                if (result == DialogResult.OK)
                {
                    e.Cancel = true;
                    return;
                }
            }

            current_serial_num = _serviceRequestControl.tbSerialNo.Text;
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

                if (isValidSerial)
                {
                    currentProductID = _serviceRequestControl.siebelProductId;
                }
                else
                {
                    //Invalid Error
                    logMessage = "The serial number (" + current_serial_num + ") is invalid. It does not belong to current contact's organization.";
                    logNote = "";
                    _log.NoticeLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);
                    
                    string message = "The serial number is invalid. It does not belong to current contact's organization.";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    DialogResult result;

                    // Show message box to mention the assigned contact party id
                    result = MessageBox.Show(this, message, "Error", buttons, MessageBoxIcon.Error);
                    if (result == DialogResult.OK)
                    {
                        //current_serial_num = siebelStoredSerialNum;
                        //currentInstanceID = siebelStoredInstanceID;
                        e.Cancel = true;
                        return;
                    }
                }
            }
            else
            {
                currentProductID = "";
            }
                
            //Update current incident's serial number
            if (current_serial_num != rnStoredSerialNum)
                this.setSerialNum(current_serial_num);

            //Get Oracle Service Clould product ID according to Siebel product information
            string rnProductId = null;
            if (currentProductID != null)
            {
                rnProductId = this.getProductId(currentProductID);
            }

            //Set current incident's product
            if (!String.IsNullOrEmpty(rnProductId))
            {
                Incident.ProductID = Convert.ToInt32(rnProductId);
            }
            else
            {
                Incident.ProductID = null;
            }

            ServiceRequest sr = new ServiceRequest();
            current_subject = Incident.Subject;
            //get current incident status id and convert to Siebel status id
            selected_rn_request_status_id = Incident.Status.StatusID;
            selected_server_request_status = sr.rnStatusToServerStatus(selected_rn_request_status_id).Key;

            //get current incident severity id and convert to Siebel severity id
            if (!Incident.SeverityID.Equals(null))
            {
                selected_rn_severity_id = (int)Incident.SeverityID;
            }
            else
            {
                selected_rn_severity_id = 0;
            }
            selected_server_severity = sr.rnSeverityToServerSeverity(selected_rn_severity_id).Key;

            //get current incident type id and serial number
            selected_rn_request_type_id = 0;

            string[] incCustomAttrs = { "Accelerator$siebel_sr_request_type"};
            Dictionary<String, Object> incCustomAttrsResults = CustomAttrHelper.fetchCustomAttrValue(incCustomAttributes, incCustomAttrs, this._logIncidentId, 0);
            selected_rn_request_type_id = incCustomAttrsResults["Accelerator$siebel_sr_request_type"] != null ? (int)incCustomAttrsResults["Accelerator$siebel_sr_request_type"] : 0;   
            
            //convert to Siebel type id 
            selected_server_request_type = sr.rnRequestTypeToServerRequestType(selected_rn_request_type_id).Key;

            logMessage = "In CheckIncidentUpdates, get all current value.";
            logNote = "incident status id = " + selected_rn_request_status_id + ", sr status id = " + selected_server_request_status +
                "incident severity id = " + selected_rn_request_type_id + ", sr severity id = " + selected_server_severity +
                "incident type id = " + selected_rn_request_type_id + ", sr type id = " + selected_server_request_type +
                "incident serial num = " + current_serial_num + ", incident subject = " + current_subject;
            _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);

            
        }
        void _recordContext_Saved(object sender, EventArgs e)
        {
            string logMessage, logNote;
            notesTruncated = "";
            //Update incident record because updated incident has been saved.
            Incident = _recordContext.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Incident) as IIncident;
            
            //get custom fields - sr id, contact party id and contact org id
            var sr_id = "";

            IList<ICustomAttribute> incCustomAttributes = Incident.CustomAttributes;
            string[] incCustomAttrSrId = { "Accelerator$siebel_sr_id" };
            Dictionary<String, Object> incCustomAttrResultSrId = CustomAttrHelper.fetchCustomAttrValue(incCustomAttributes, incCustomAttrSrId, this._logIncidentId, 0);
            sr_id = incCustomAttrResultSrId["Accelerator$siebel_sr_id"] != null ? incCustomAttrResultSrId["Accelerator$siebel_sr_id"].ToString() : "";

            selectedContactPartyId = "0";
            selectedContactOrgId = "0";

            IList<ICustomAttribute> conCustomAttributes = Contact.CustomAttributes;
            string[] conCustomAttrs = { "Accelerator$siebel_contact_party_id", "Accelerator$siebel_contact_org_id" };
            Dictionary<String, Object> conCustomAttrResults = CustomAttrHelper.fetchCustomAttrValue(conCustomAttributes, conCustomAttrs, this._logIncidentId, 0);
            selectedContactPartyId = conCustomAttrResults["Accelerator$siebel_contact_party_id"] != null ? conCustomAttrResults["Accelerator$siebel_contact_party_id"].ToString() : "0";
            selectedContactOrgId = conCustomAttrResults["Accelerator$siebel_contact_org_id"] != null ? conCustomAttrResults["Accelerator$siebel_contact_org_id"].ToString() : "0";


            if (String.IsNullOrWhiteSpace(sr_id))
            {
                // Current Incident is not associated to Service Request
                logMessage = "Current Incident is not associated to Service Request. SR ID is empty. It is going to create Service Request.";
                logNote = "";
                _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);

                if (Contact == null || selectedContactPartyId == "0")
                {
                    logMessage = "Cannot create Service Request, since contact party id is null.";
                    logNote = "";
                    _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);
                }
                else
                {
                    // Create Service Request and update Accelerator$siebel_sr_id field of incident
                    SaveToSiebel(true);

                    incCustomAttrResultSrId = CustomAttrHelper.fetchCustomAttrValue(incCustomAttributes, incCustomAttrSrId, this._logIncidentId, 0);
                    sr_id = incCustomAttrResultSrId["Accelerator$siebel_sr_id"] != null ? incCustomAttrResultSrId["Accelerator$siebel_sr_id"].ToString() : "";

                }

            }
            else if (!String.IsNullOrWhiteSpace(sr_id) && (CheckIncidentUpdates()))
            {
                logMessage = "Incident is updated. Need to update associated Service Request. SR ID = " + sr_id;
                logNote = "";
                _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);

                SaveToSiebel(false);

                incCustomAttrResultSrId = CustomAttrHelper.fetchCustomAttrValue(incCustomAttributes, incCustomAttrSrId, this._logIncidentId, 0);
                sr_id = incCustomAttrResultSrId["Accelerator$siebel_sr_id"] != null ? incCustomAttrResultSrId["Accelerator$siebel_sr_id"].ToString() : "";

            }

            if (!String.IsNullOrWhiteSpace(sr_id))
            {
                int currentThreadCount = Incident.Threads.Count == 0 ? 0 : Incident.Threads[0].ID;
                int storedThreadsCount = 0;

                string[] incCustomAttrThread = { "Accelerator$siebel_max_thread_id" };
                Dictionary<String, Object> incCustomAttrResultThread= CustomAttrHelper.fetchCustomAttrValue(incCustomAttributes, incCustomAttrThread, this._logIncidentId, 0);
                storedThreadsCount = incCustomAttrResultThread["Accelerator$siebel_max_thread_id"] != null ? (int)incCustomAttrResultThread["Accelerator$siebel_max_thread_id"] : 0;


                if (currentThreadCount != storedThreadsCount && !String.IsNullOrWhiteSpace(sr_id))
                {
                    logMessage = "Need to store new threads to Siebel System. Stored Thread Count = " + storedThreadsCount + "; Current Thread Count = " + currentThreadCount;
                    logNote = "";
                    _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);
                    SaveNoteToSiebel(sr_id, currentThreadCount, storedThreadsCount, false);
                }
            }

            if (notesTruncated != "")
                MessageBox.Show("This incident contains the following note(s) that will be truncated to first 1k characters in Siebel.\nPlease summarize the truncated detail in a private note for Siebel Agents as appropriate.\n" + notesTruncated);

            //_recordContext.RefreshWorkspace();

        }
      
        //Check whether update status, severity, type and serial number fields
        //Since all these field are set in standard incident fields instead of addin, need to compare  current value with value in Siebel
        bool CheckIncidentUpdates()
        {
            if (current_serial_num != siebelStoredSerialNum)
            {
                return true;
            }
            if (currentProductID != siebelStoredProductID)
            {
                return true;
            }


            if (current_subject != storedSubject)
            {
                return true;
            }

            if (selected_server_request_status != storedRequestStatus)
            {
                return true;
            }

            if ( selected_server_severity!= storedSeverity)
            {
                return true;
            }


            if (selected_server_request_type != storedRequestType)
            {
                return true;
            }

            string logMessage = "Do not need to update Service Request. The fields changed in incident did not impact Service Request value.";
            _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage);
            return false;
        }

        //Current product logic is not decided, implement it later.
        public string getProductId(String siebelInventoryItemName)
        {
            String rnProductId = null;

            if (String.IsNullOrEmpty(siebelInventoryItemName))
            {
                return null;
            }

            //Note for this sample to work we assume there is an organization with a contact assigned
            String queryString = "SELECT S.ServiceProduct.ID FROM SalesProduct S WHERE S.PARTNUMBER= '" + siebelInventoryItemName + "'";
            String[] rowData = null;
            try
            {
                rowData = _rnSrv.queryData(queryString);
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    string logMessage = "Error in query Product ID from CWSS. Error query: " + queryString;
                    _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage);
                }
                MessageBox.Show("Exception in query " + queryString + " from CWSS: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            foreach (String data in rowData)
            {
                rnProductId = data;
            }
            return rnProductId;
        }

        public bool validateSerialNumber(String serialNum)
        {
            IList<ICustomAttribute> conCustomAttributes = Contact.CustomAttributes;
            string[] conCustomAttrs = { "Accelerator$siebel_contact_org_id" };
            Dictionary<String, Object> conCustomAttrResults = CustomAttrHelper.fetchCustomAttrValue(conCustomAttributes, conCustomAttrs, this._logIncidentId, 0);
            string contactOrgId = conCustomAttrResults["Accelerator$siebel_contact_org_id"] != null ? conCustomAttrResults["Accelerator$siebel_contact_org_id"].ToString() : "0";

            if (contactOrgId == "0")
            {
                DialogResult result = MessageBox.Show("Current contact did not associate to a Siebel contact correctly. Cannot do the serial number validation. ", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (result == DialogResult.OK)
                {
                    string logMessage = "Current contact did not associate to a Siebel contact correctly. Cannot do the serial number validation.";
                    _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage);
                    return false;
                }
            }
            try
            {
                bool isValid = _serviceRequestControl.SerialNumberValidation(serialNum, contactOrgId);
                _serviceRequestControl.SetValidedResult(isValid);
                return isValid;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void setSerialNum(string serialNum)
        {
            IList<ICustomAttribute> incCustomAttributes = Incident.CustomAttributes;
            bool sr_sn_found = false;
            foreach (ICustomAttribute cusAttr in incCustomAttributes)
            {
                if (cusAttr.PackageName == "Accelerator" && cusAttr.GenericField.Name == "Accelerator$siebel_serial_number")
                {
                    sr_sn_found = true;
                    if (serialNum == "")
                    {
                        //TODO::Cannot set as Empty
                        cusAttr.GenericField.DataValue.Value = " ";
                    }
                    else
                    {
                        cusAttr.GenericField.DataValue.Value = serialNum;
                    }
                }
            }

            if (sr_sn_found == false)
            {
                string logMessage = "Custom attribute is not defined. Cannot get Accelerator$siebel_serial_number.";
                _log.ErrorLog(incidentId: _logIncidentId, logMessage:logMessage);
                MessageBox.Show("Custom Attribute configuration missing. Please check log for detail.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        // Update Custom Attribute via SOAP API
        // After create/update incident, the custom attribute may need to update.
        protected void UpdateIncCustomAttr(int incidentID, string key, string value)
        {
            //Create an Incident object
            Accelerator.Siebel.SharedServices.RightNowServiceReference.Incident updateIncident = new Accelerator.Siebel.SharedServices.RightNowServiceReference.Incident();


            //Create and set the Id property
            ID incID = new ID();
            //Set the Id on the Incident Object
            incID.id = incidentID;
            incID.idSpecified = true;

            updateIncident.ID = incID;

            
            if (value != null && value != "")
            {
                GenericField customField = new GenericField();
                // Check update which custom attribute
                switch (key)
                {
                    case "siebel_sr_id":
                        customField.name = "siebel_sr_id";
                        customField.dataType = DataTypeEnum.STRING;
                        customField.dataTypeSpecified = true;
                        customField.DataValue = new DataValue();
                        customField.DataValue.Items = new object[1];
                        customField.DataValue.ItemsElementName = new ItemsChoiceType[1];
                        customField.DataValue.Items[0] = value;
                        customField.DataValue.ItemsElementName[0] = ItemsChoiceType.StringValue;
                        break;
                    case "siebel_sr_num":
                        customField.name = "siebel_sr_num";
                        customField.dataType = DataTypeEnum.STRING;
                        customField.dataTypeSpecified = true;
                        customField.DataValue = new DataValue();
                        customField.DataValue.Items = new object[1];
                        customField.DataValue.ItemsElementName = new ItemsChoiceType[1];
                        customField.DataValue.Items[0] = value;
                        customField.DataValue.ItemsElementName[0] = ItemsChoiceType.StringValue;
                        break;
                    case "siebel_max_thread_id":
                        customField.name = "siebel_max_thread_id";
                        customField.dataType = DataTypeEnum.INTEGER;
                        customField.dataTypeSpecified = true;
                        customField.DataValue = new DataValue();
                        customField.DataValue.Items = new object[1];
                        customField.DataValue.ItemsElementName = new ItemsChoiceType[1];
                        customField.DataValue.Items[0] = Convert.ToInt32(value);
                        customField.DataValue.ItemsElementName[0] = ItemsChoiceType.IntegerValue;
                        break;
                    default:
                        return;
                }

                GenericObject customFieldsc = new GenericObject();
                customFieldsc.GenericFields = new GenericField[1];
                customFieldsc.GenericFields[0] = customField;
                customFieldsc.ObjectType = new RNObjectType();
                customFieldsc.ObjectType.TypeName = "IncidentCustomFieldsc";

                GenericField cField = new GenericField();
                cField.name = "Accelerator";
                cField.dataType = DataTypeEnum.OBJECT;
                cField.dataTypeSpecified = true;
                cField.DataValue = new DataValue();
                cField.DataValue.Items = new object[1];
                cField.DataValue.Items[0] = customFieldsc;
                cField.DataValue.ItemsElementName = new ItemsChoiceType[1];
                cField.DataValue.ItemsElementName[0] = ItemsChoiceType.ObjectValue;

                updateIncident.CustomFields = new GenericObject();
                updateIncident.CustomFields.GenericFields = new GenericField[1];
                updateIncident.CustomFields.GenericFields[0] = cField;
                updateIncident.CustomFields.ObjectType = new RNObjectType();
                updateIncident.CustomFields.ObjectType.TypeName = "IncidentCustomFields";
            }
            else
            {
                return;
            }
            //Create the RNObject array
            RNObject[] objects = new RNObject[] { updateIncident };

            try
            {
                _rnSrv.updateObject(objects);  
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    string logMessage = "Error in updating incident custom field via Cloud Service SOAP. Try to update incident(ID = " + incidentID + ") custom attribute " + key + " to value" + value + "; Error Message: "+ ex.Message;
                    _log.ErrorLog(incidentId:_logIncidentId, logMessage: logMessage);
                }
                MessageBox.Show("There has been an error communicating with Cloud Service SOAP. Please check log for detail.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void SaveToSiebel(bool isCreate)
        {
            bool sr_saved;
            ServiceRequest sr = null;
            string logMessage, logNote;

            // Make sure we actually have an asset and a saved incident
            if (Contact != null)
            {
                //Compose Service Request
                sr = new ServiceRequest();

                // Set Service Request attributes - severity, status, request type, contact party id, etc. 
                KeyValuePair<String, String> severityKeyPair = sr.rnSeverityToServerSeverity(selected_rn_severity_id);
                selected_server_severity = severityKeyPair.Key;
                sr.Severity = severityKeyPair.Key;

                KeyValuePair<String, String> statusKeyPair = sr.rnStatusToServerStatus(selected_rn_request_status_id);
                selected_server_request_status = statusKeyPair.Key;
                sr.Status = statusKeyPair.Key;

                KeyValuePair<String, String> requestTypeKeyPair = sr.rnRequestTypeToServerRequestType(selected_rn_request_type_id);
                selected_server_request_type = requestTypeKeyPair.Key;
                sr.RequestType = requestTypeKeyPair.Key;

                sr.ContactID = selectedContactPartyId;
                if (String.IsNullOrEmpty(selectedContactOrgId) || selectedContactOrgId == "0")
                {
                    sr.ContactOrgID = null;
                }
                else
                {
                    sr.ContactOrgID = selectedContactOrgId;
                }
                
                sr.Summary = Incident.Subject;
                sr.RequestDate = (Incident.Created.HasValue) ? Incident.Created.Value : DateTime.Now;
                sr.IncidentOccurredDate = (Incident.Created.HasValue) ? Incident.Created.Value : DateTime.Now;

                sr.IncidentRef = Incident.RefNo;
                sr.IncidentID = Incident.ID.ToString();

                sr.SerialNumber = current_serial_num;

                if(!String.IsNullOrEmpty(currentProductID)){
                    sr.ProductID = currentProductID;
                }
                else
                {
                    sr.ProductID = null;
                }

                if (_siebelDefaultSrOwnerId != null)
                {
                    sr.OwnerID = _siebelDefaultSrOwnerId;
                }


                try
                {
                    if (isCreate)
                    {
                        //Create Service Request
                        sr.RnowHost = ConfigurationSetting.rnt_host;
                        logMessage = "Ready to create Service Request";
                        _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage);
                        sr_saved = sr.Create(_logIncidentId, 0);
                    }
                    else
                    {
                        //Update Service Request
                        //Set custom attribute fields for update
                        var sr_id = "";
                        var sr_num = "";
                        var sr_owner_id = "";
                        IList<ICustomAttribute> incCustomAttributes = Incident.CustomAttributes;
                        string[] incCustomAttrs = { "Accelerator$siebel_sr_id", "Accelerator$siebel_sr_num", "Accelerator$siebel_sr_owner_id"};
                        Dictionary<String, Object> incCustomAttrsResults = CustomAttrHelper.fetchCustomAttrValue(incCustomAttributes, incCustomAttrs, this._logIncidentId, 0);
                        sr_id = incCustomAttrsResults["Accelerator$siebel_sr_id"] != null ? incCustomAttrsResults["Accelerator$siebel_sr_id"].ToString() : "";
                        sr_num = incCustomAttrsResults["Accelerator$siebel_sr_num"] != null ? incCustomAttrsResults["Accelerator$siebel_sr_num"].ToString() : "";
                        sr_owner_id = incCustomAttrsResults["Accelerator$siebel_sr_owner_id"] != null ? incCustomAttrsResults["Accelerator$siebel_sr_owner_id"].ToString() : "";

                        sr.RequestID = sr_id;
                        sr.RequestNumber = sr_num;
                        sr.RnowHost = ConfigurationSetting.rnt_host;
                        if (sr_owner_id != "")
                        {
                            sr.OwnerID = sr_owner_id;
                        }

                        logMessage = "Ready to update Service Request. SR ID = " + sr_id;
                        logNote = "";
                        _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);

                        sr_saved = sr.Update(_logIncidentId, 0);
                    }
                }
                catch (Exception ex)
                {
                    logMessage = "Error in creating/updating Service Request.Error Message: " + ex.Message;
                    logNote = "";
                    _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);

                    string message = ex.Message;
                    MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!sr_saved)
                {
                    // If Service Request is not saved successfully, show error.
                    string message = "There has been an error communicating with Siebel. Please check log for detail.";
                    MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    logMessage = "Error in creating/updating Service Request.";
                    logNote = "Response shows error code. Response's error message: " + sr.ErrorMessage;
                    _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);

                    return;
                }
                else
                {
                    logMessage = "Created/Updated Service Request successfully. SR ID = " + sr.RequestID;
                    logNote = "";
                    _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);

                    // After update successfully, update the values which are stored information from Siebel side
                    try
                    {
                        storedRequestType = selected_server_request_type;
                        storedRequestStatus = selected_server_request_status;
                        storedSeverity = selected_server_severity;
                        siebelStoredSerialNum = current_serial_num;
                    }
                    catch (Exception ex)
                    {
                        logMessage = "Error in updating incident fields with Service Request information, after created/updated SR. Error Message: " + ex.Message;
                        _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage);
                        return;
                    }

                    if (isCreate)
                    {
                        //Check the interaction, we need created, after created a new service request
                        string sr_id = sr.RequestID;
                        int currentThreadCount = Incident.Threads.Count == 0 ? 0 : Incident.Threads[0].ID;
                        int storedThreadsCount = 0;
                        IList<ICustomAttribute> incCustomAttributes = Incident.CustomAttributes;
                        string[] incCustomAttrThread = { "Accelerator$siebel_max_thread_id" };
                        Dictionary<String, Object> incCustomAttrResultThread = CustomAttrHelper.fetchCustomAttrValue(incCustomAttributes, incCustomAttrThread, this._logIncidentId, 0);
                        storedThreadsCount = incCustomAttrResultThread["Accelerator$siebel_max_thread_id"] != null ? (int)incCustomAttrResultThread["Accelerator$siebel_max_thread_id"] : 0;
                        

                        // If have new thread, then call function SaveInteractionToSiebel to create interaction
                        if (currentThreadCount != storedThreadsCount && !String.IsNullOrWhiteSpace(sr_id))
                        {
                            logMessage = "After created new SR, need to store new threads to Siebel System. Stored Thread Count = " + storedThreadsCount + "; Current Thread Count = " + currentThreadCount;
                            logNote = "";
                            _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);

                            SaveNoteToSiebel(sr_id, currentThreadCount, storedThreadsCount, true);
                        }

                        // Iterate through the incident custom attributes and set values
                        try
                        {
                            bool sr_id_found = false;
                            bool sr_num_found = false;
                            bool sr_owner_id_found = false;
                            foreach (ICustomAttribute cusAttr in incCustomAttributes)
                            {
                                if (cusAttr.PackageName == "Accelerator" && cusAttr.GenericField.Name == "Accelerator$siebel_sr_id")
                                {
                                    sr_id_found = true;
                                    cusAttr.GenericField.DataValue.Value = sr.RequestID;
                                    UpdateIncCustomAttr(Incident.ID, "siebel_sr_id", sr.RequestID);
                                }
                                if (cusAttr.PackageName == "Accelerator" && cusAttr.GenericField.Name == "Accelerator$siebel_sr_num")
                                {
                                    ServiceRequest createdSR = sr.Lookup(sr.RequestID, _logIncidentId);
                                    sr_num_found = true;
                                    if (createdSR != null)
                                    {
                                        cusAttr.GenericField.DataValue.Value = createdSR.RequestNumber;
                                        UpdateIncCustomAttr(Incident.ID, "siebel_sr_num", createdSR.RequestNumber);
                                    }
                                }
                                if (cusAttr.PackageName == "Accelerator" && cusAttr.GenericField.Name == "Accelerator$siebel_sr_owner_id")
                                {
                                    sr_owner_id_found = true;
                                    cusAttr.GenericField.DataValue.Value = sr.OwnerID;
                                    UpdateIncCustomAttr(Incident.ID, "siebel_sr_owner_id", sr.OwnerID);
                                }
                            }

                            if (sr_id_found == false)
                            {
                                logMessage = "Custom attribute is not defined. Cannot get Accelerator$siebel_sr_id.";
                                _log.ErrorLog(incidentId:_logIncidentId, logMessage: logMessage);
                            }

                            if (sr_num_found == false)
                            {
                                logMessage = "Custom attribute is not defined. Cannot get Accelerator$siebel_sr_num.";
                                _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage);
                            }

                            if (sr_owner_id_found == false)
                            {
                                logMessage = "Custom attribute is not defined. Cannot get Accelerator$siebel_sr_owner_id.";
                                _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage);
                            }

                            if (sr_id_found == false || sr_num_found == false || sr_owner_id_found == false)
                            {
                                MessageBox.Show("Custom Attribute configuration missing. Please check log for detail.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            logMessage = "Error in updating incident fields with Service Request information, after created new SR.Error Message: " + ex.Message;
                            _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage);
                            return;
                        }

                        logMessage = " Updating incident fields with Service Request information, after created new SR successfully.siebel_sr_id = " + sr.RequestID.ToString() + "; siebel_sr_num = " + sr.RequestNumber + "; siebel_sr_owner_id = " + sr.OwnerID.ToString() + ".";
                        _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage);
                    }
                }
            }
        }

        // Save CX thread to Siebel side
        private void SaveNoteToSiebel(string sr_id, int currentThreadCount, int storedThreadCount, bool includeCustomerThreads)
        {
            string logMessage, logNote;
            if (storedRequestStatus == "Close")
            {
                string message = "Message cannot be saved in Siebel because the Service Request is closed.";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                result = System.Windows.Forms.MessageBox.Show(this, message, "Warning", buttons, MessageBoxIcon.Warning);
                logMessage = "Cannot propagate the message to Siebel.";
                logNote = message;
                _log.ErrorLog(logMessage: logMessage, logNote: logNote, incidentId: _logIncidentId);

                if (result == DialogResult.OK)
                {
                    return;
                }
                else
                {
                    return;
                }
            }
            IThread thread;
            SharedServices.Note note;
            bool interaction_saved = false;
            bool attachment_saved = false;
            // Get thread count of current incident
            int i = Incident.Threads.Count;
            while (i > 0)
            {
                i--;
                thread = Incident.Threads[i];

                // If thread id is larger than last time synced thread id, may need save it to Siebel.
                if (thread.ID <= storedThreadCount)
                {
                    continue;
                }
                
                // If thread is created by Custom Portal, ignore it.
                if (thread.AcctID == null && thread.ChanID == 6 && includeCustomerThreads == false)
                {
                    logMessage = "Thread is created from CP. Do not need to propagate to Siebel again. Thread ID = " + thread.ID;
                    _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage);
                    storedThreadCount = thread.ID;
                    continue;
                }

                // If thread is created by CWSS_API_Siebel_Service_User as a private note, ignore it.
                if (thread.AcctID == ConfigurationSetting.cwssApiSiebelServiceUserId && thread.ChanID == null)
                {
                    logMessage = "Thread is created from Siebel side. Do not need to propagate to Siebel again. Thread ID = " + thread.ID;
                    _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage);
                    storedThreadCount = thread.ID;
                    continue;
                }

                // Set Interaction Object 
                note = new SharedServices.Note();

                if (thread.AcctID != null)
                {
                    String queryString = "SELECT A.Name.First, A.Name.Last FROM ACCOUNT A WHERE A.ID=" + thread.AcctID;
                    String[] rowData = null;
                    try
                    {
                        rowData = _rnSrv.queryData(queryString);
                    }
                    catch (Exception ex)
                    {
                        if (_log != null)
                        {
                            logMessage = "Error in query thread's author name from Cloud Service. Thread ID = " + thread.ID;
                            logNote = "Error in query thread's author name. Error query: " + queryString + "; Error Message: " + ex.Message;
                            _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);
                        }
                        MessageBox.Show("There has been an error communicating with Siebel. Please check log for detail.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }


                    string[] name = rowData[0].Split(',');
                    note.Author = name[0] + " " + name[1];
                }
                else if (thread.CId != null)
                {
                    String queryString = "SELECT A.Name.First, A.Name.Last FROM CONTACT A WHERE A.ID=" + thread.CId;
                    String[] rowData = null;
                    try
                    {
                        rowData = _rnSrv.queryData(queryString);
                    }
                    catch (Exception ex)
                    {
                        if (_log != null)
                        {
                            logMessage = "Error in query thread's author name from Cloud Service. Thread ID = " + thread.ID;
                            logNote = "Error in query thread's author name. Error query: " + queryString + "; Error Message: " + ex.Message;
                            _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);
                        }
                        MessageBox.Show("There has been an error communicating with Siebel. Please check log for detail.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    string[] name = rowData[0].Split(',');
                    note.Author = name[0] + " " + name[1];
                }

                string content = thread.Note;
                if (thread.ContentType == RightNow.AddIns.Common.ThreadContentType.HTML)
                {
                    string thread_note = thread.Note;
                    string note_raw = Regex.Replace(thread_note, "<.*?>", string.Empty);
                    if(note_raw.Length > 0)
                        note_raw =  note_raw.TrimStart('\n').TrimEnd('\n');
                    content = WebUtility.HtmlDecode(note_raw);
                }

                // truncate up to 1000 excluding ... (total would be 1003)
                if (content.Length > 1000)
                {
                    content = content.Substring(0, 1000);
                    content += "...";
                    logMessage = "Thread ID = " + thread.ID + " content: \"" + content.Substring(0, 50) + "\" is truncated to first 1k characters in Siebel";
                    _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage);

                    // can be more than one, the caller _recordContext_Saved will show the list
                    notesTruncated += "- Note: \" " + content.Substring(0, 50) + " ...\" \n"; 
                }

                note.Content = content;
                note.Created = (DateTime)thread.Entered;
                note.SrID = sr_id;
                string channel = null;
                string rnChannel = null;
                switch (thread.ChanID)
                {
                    case 1:
                        channel = "Email";
                        rnChannel = "Service Mailbox";
                        break;
                    case 2:
                        channel = "Email";
                        rnChannel = "Marketing Mailbox";
                        break;
                    case 3:
                        channel = "Call";
                        rnChannel = "Phone";
                        break;
                    case 4:
                        channel = "Fax";
                        rnChannel = "Fax";
                        break;
                    case 5:
                        channel = "Letter - Inbound";
                        rnChannel = "Mail";
                        break;
                    case 6:
                        channel = "Web - Inbound";
                        rnChannel = "Service web form";
                        break;
                    case 7:
                        channel = "Web - Inbound";
                        rnChannel = "Marketing web form";
                        break;
                    case 8:
                        channel = "Chat - Transfer";
                        rnChannel = "Chat";
                        break;
                    case 9:
                        channel = "Email";
                        rnChannel = "Email";
                        break;
                    default:
                        if (thread.EntryType == 1)
                            channel = "Note";
                        else
                            channel = "Other";
                        rnChannel = "No Channel";
                        break;
                }
                note.Channel = channel;
                if (thread.EntryType == 1)
                    note.Status = true;
                else
                    note.Status = false;

                if (thread.ChanID != null && (int)thread.ChanID == 8)
                {
                    note.Summary = note.Author + " via [" + rnChannel + "] " + this.Incident.Subject;
                    if (note.Content.Length > 500)
                        note.Content = note.Content.Substring(0, 497) + "...";
                }
                else
                {
                    note.Summary = note.Author + " via [" + rnChannel + "] " + content;
                    if (note.Content.Length > 1500)
                        note.Content = note.Content.Substring(0, 1497) + "...";
                }


                if (note.Summary.Length > 100)
                    note.Summary = note.Summary.Substring(0, 97) + "...";

                /** TODO:: Note Owner
                if (thread.AcctID != null)
                {
                    if(_siebelServiceUserId == -1){
                        MessageBox.Show("There has been an error communicating with Siebel. Please check log for detail.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        logMessage = "Cannot propagate the note created by agent to Siebel. siebel_service_user_id is missing in Configration verb. Thread ID = " + thread.ID;
                        _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage);
                        return;
                    }
                    else
                    {
                        note.CreatedByID = Convert.ToDecimal(_siebelServiceUserId);
                    }

                }
                else if(thread.CId != null)
                {
                    note.CreatedByID = 0;
                }
                 * */

                try
                {
                    // Create Interaction in Siebel
                    logMessage = "Ready to propagate thread to Siebel. Thread ID = " + thread.ID;
                    _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage);

                    interaction_saved = note.Create(_logIncidentId, 0);

                }
                catch (Exception ex)
                {
                    logMessage = "Error in Propagating thread. Thread ID = " + thread.ID + "; SR ID = " + sr_id + "; Exception: " + ex.Message;
                    _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage);

                    interaction_saved = false;

                    string message = "There has been an error communicating with Siebel. Please check log for detail.";
                    MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }

                if (interaction_saved == false)
                {
                    logMessage = "Thread is not saved in Siebel. Thread ID = " + thread.ID + "; SR ID = " + sr_id + "; Response Error Message: " + note.ErrorMessage;
                    _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage);

                    MessageBox.Show("There has been an error communicating with Siebel. Please check log for detail.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
                else
                {
                    if (thread.ChanID == 8)
                    {
                        try
                        {
                            note.Content = content;
                            note.SrID = sr_id;
                            note.NoteID = note.NoteID;
                            note.Created = (DateTime)thread.Entered;

                            // Create Attachment in Siebel
                            logMessage = "Save Chat Transcript to Siebel. Thread ID = " + thread.ID;
                            logNote = note.ToString();
                            _log.DebugLog(logMessage: logMessage, logNote: logNote, incidentId: _logIncidentId);
                            attachment_saved = note.AddAttachment(_logIncidentId:_logIncidentId);

                        }
                        catch (Exception ex)
                        {
                            attachment_saved = false;
                            MessageBox.Show("A problem occurred with saving chat transcript: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        }
                    }

                    logMessage = "Thread is saved in Siebel. Thread ID = " + thread.ID + "; SR ID = " + sr_id;
                    _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage);
                }

                storedThreadCount = thread.ID;
            }

            //Save current max thread id after creating thread
            IList<ICustomAttribute> incCustomAttributes = Incident.CustomAttributes;
            bool max_thread_id = false;
            foreach (ICustomAttribute cusAttr in incCustomAttributes)
            {
                if (cusAttr.PackageName == "Accelerator" && cusAttr.GenericField.Name == "Accelerator$siebel_max_thread_id")
                {
                    max_thread_id = true;
                    cusAttr.GenericField.DataValue.Value = storedThreadCount;
                    UpdateIncCustomAttr(Incident.ID, "siebel_max_thread_id", Convert.ToString(storedThreadCount));
                    
                    logMessage = "Save current max thread id after creating thread. Thread count is " + cusAttr.GenericField.DataValue.Value;
                    _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage);
                    break;
                }
            }

            if (max_thread_id == false)
            {
                logMessage = "Custom attribute is not defined. Cannot get Accelerator$siebel_sr_id.";
                _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage);
                MessageBox.Show("Custom Attribute configuration missing. Please check log for detail.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //_recordContext.RefreshWorkspace();
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

    [AddIn("Service Request AddIn", Version = "1.0.0.0")]
    public class ServiceRequestAddInFactory : IWorkspaceComponentFactory2
    {
        #region IWorkspaceComponentFactory2 Members
        private IRecordContext _rContext;
        private IGlobalContext _gContext;
        private ServiceRequestAddIn _wsAddIn;
        private String _usr;
        private String _pwd;
        private string _siebelServiceUserId;
        private string _siebelDefaultSrOwnerId;

        //private RightNowSyncPortClient _client;
        private RightNowService _rnSrv;
        private LogWrapper _log;
        


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
                    MessageBox.Show("Service Request Add-In is not initialized properly. \nPlease contact your system administrator.\n You are now logged out.");
                    _gContext.Logout();
                }
                else // don't want to logout admin
                {
                    MessageBox.Show("Service Request Add-In is not loaded because of invalid configuration verb.");
                    return new ServiceRequestAddIn(inDesignMode, RecordContext);
                }
            }

            _rContext = RecordContext;
            if (!inDesignMode && _rContext != null)
            {
                ConfigurationSetting instance = ConfigurationSetting.Instance(_gContext);

                _usr = ConfigurationSetting.username;
                _pwd = ConfigurationSetting.password;
                //_client = ConfigurationSetting.client;
                _rnSrv = ConfigurationSetting.rnSrv;
                _log = ConfigurationSetting.logWrap;
                _siebelServiceUserId = ConfigurationSetting.siebelServiceUserId;
                _siebelDefaultSrOwnerId = ConfigurationSetting.siebelDefaultSrOwnerId;

                ServiceRequest.ServiceProvider = ConfigurationSetting.SiebelProvider;
                ServiceRequest.CreateUpdateURL = ConfigurationSetting.CreateSR_WSDL;
                ServiceRequest.LookupURL = ConfigurationSetting.LookupSR_WSDL;
                ServiceRequest.ServiceUsername = String.IsNullOrEmpty(_usr) ? "ebusiness" : _usr;
                ServiceRequest.ServicePassword = String.IsNullOrEmpty(_pwd) ? "password" : _pwd;
                ServiceRequest.ServiceClientTimeout = ConfigurationSetting.SiebelServiceTimeout;
                ServiceRequest.InitSiebelProvider();

                SharedServices.Note.ServiceProvider = ConfigurationSetting.SiebelProvider;
                SharedServices.Note.CreateInteractionURL = ConfigurationSetting.CreateInteraction_WSDL;
                SharedServices.Note.ServiceUsername = String.IsNullOrEmpty(_usr) ? "ebusiness" : _usr;
                SharedServices.Note.ServicePassword = String.IsNullOrEmpty(_pwd) ? "password" : _pwd;
                SharedServices.Note.ServiceClientTimeout = ConfigurationSetting.SiebelServiceTimeout;
                SharedServices.Note.InitSiebelProvider();

                /*
                Item.ServiceProvider = ConfigurationSetting.SiebelProvider;
                Item.ListURL = ConfigurationSetting.ItemList_WSDL;
                Item.ServiceUsername = ConfigurationSetting.username;
                Item.ServicePassword = ConfigurationSetting.password;
                Item.ServiceClientTimeout = ConfigurationSetting.SiebelServiceTimeout;
                Item.InitSiebelProvider();
                 * */
            }
            _wsAddIn = new ServiceRequestAddIn(inDesignMode, _rContext);

            _wsAddIn._rnSrv = _rnSrv;
            _wsAddIn._log = _log;
            _wsAddIn._siebelServiceUserId = _siebelServiceUserId;
            _wsAddIn._siebelDefaultSrOwnerId = _siebelDefaultSrOwnerId;

            if (_log != null)
            {
                string logMessage = "Service Request AddIn is setup.";
                _log.DebugLog(logMessage: logMessage);
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
            get { return "SiebelServiceRequestWorkspaceAddIn"; }
        }

        /// <summary>
        /// The tooltip displayed when hovering over the Add-In in the Ribbon of the Workspace Designer.
        /// </summary>
        public string Tooltip
        {
            get { return "Siebel Service Request WorkspaceAddIn Tooltip"; }
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
            ConfigurationSetting.logAssemblyVersion(Assembly.GetAssembly(typeof(ServiceRequestAddInFactory)), null);

            ConfigurationSetting instance = ConfigurationSetting.Instance(_gContext);
            /* log it, but return true because will show the messagebox when the addin is opened in createControl.
             * if return false, the add-in is not loaded, and cannot show the error when add-in is opened.
             */
            if (!ConfigurationSetting.configVerbPerfect)
            {
                string logMessage = "ServiceRequestAddIn is not initialized properly because of invalid config verb.";
                ConfigurationSetting.logWrap.ErrorLog(logMessage: logMessage);
            }
                
            return true;       
        }

        #endregion
    }
}