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
 *  date: Wed Sep  2 23:11:39 PDT 2015

 *  revision: rnw-15-8-fixes-release-01
 *  SHA1: $Id: ea68cc46f648406b215c280f69d476fbcff36704 $
 * *********************************************************************************************
 *  File: ServiceRequestAddIn.cs
 * *********************************************************************************************/

using Accelerator.EBS.SharedServices;

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
using Accelerator.EBS.SharedServices.RightNowServiceReference;
using Accelerator.EBS.SharedServices.Logs;
using System.ServiceModel.Channels;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Net;
using System.Reflection;

namespace Accelerator.EBS.ServiceRequestAddin
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
        private int storedSeverityId;
        private int storedRequestTypeId;
        private int storedRequestStatusId;
        private string storedSubject;
        private string ebsStoredSerialNum;
        private string rnStoredSerialNum;
        private decimal ebsStoredInstanceID;
        
        private int selected_rn_severity_id, selected_rn_request_type_id, selected_rn_request_status_id;
        private string selected_server_severity_id, selected_server_request_type_id, selected_server_request_status_id, current_serial_num, current_subject;        
        
        private string selectedContactPartyId;
        private string selectedContactOrgId;
        private int currentContactID;
        private decimal currentInstanceID;

        //public RightNowSyncPortClient _rnowClient;
        public RightNowService _rnSrv;
        public LogWrapper _log;
        public int _logIncidentId;
        public int _ebsServiceUserId;
        public int _ebsDefaultSrOwnerId;
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
            storedSeverityId = _serviceRequestControl.storedSeverityId;
            storedRequestStatusId = _serviceRequestControl.storedRequestStatusId;
            storedRequestTypeId = _serviceRequestControl.storedRequestTypeId;
            storedSubject = _serviceRequestControl.storedSubject;
            ebsStoredSerialNum = _serviceRequestControl.ebsStoredSerialNum;
            ebsStoredInstanceID = _serviceRequestControl.ebsStoredInstanceID;

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

            // get custom attribute Accelerator$ebs_sr_id value
            var sr_id = "";

            IList<ICustomAttribute> incCustomAttributes = Incident.CustomAttributes;
            string[] incCustomAttrSrId = { "Accelerator$ebs_sr_id", "Accelerator$ebs_serial_number"};
            Dictionary<String, Object> incCustomAttrSrIdResult = CustomAttrHelper.fetchCustomAttrValue(incCustomAttributes, incCustomAttrSrId, this._logIncidentId, 0);
            sr_id = incCustomAttrSrIdResult["Accelerator$ebs_sr_id"] != null ? incCustomAttrSrIdResult["Accelerator$ebs_sr_id"].ToString() : "";
            rnStoredSerialNum = !String.IsNullOrWhiteSpace((string)incCustomAttrSrIdResult["Accelerator$ebs_serial_number"])? incCustomAttrSrIdResult["Accelerator$ebs_serial_number"].ToString() : "";

            // If Incident's Contact has associated to an EBS Contact, it does not support to edit it.
            if (!String.IsNullOrWhiteSpace(sr_id) && currentContactID != Contact.ID)
            {
                string message = "This incident has been associated to a EBS Contact, we cannot allow changing the contact via this addin. Please change the primary contact back.";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                // Show message box to mention the assigned contact party id
                result = System.Windows.Forms.MessageBox.Show(this, message, "Message", buttons, MessageBoxIcon.Information);

                logMessage = "Cannot changing incident's primary contact. This incident has been associated to an EBS Contact, Contact ID = " + Contact.ID;
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
            string ebsProduct = null;
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
                
                currentInstanceID = _serviceRequestControl.inputInstanceID;
                if (isValidSerial)
                {
                    ebsProduct = _serviceRequestControl.ebsInventoryItemName;
                }
                else
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
                currentInstanceID = 0;
            }
                
            //Update current incident's serial number
            if (current_serial_num != rnStoredSerialNum)
                this.setSerialNum(current_serial_num);

            //Get Oracle Service Clould product ID according to EBS product information
            string rnProductId = null;
            if (ebsProduct != null)
            {
                rnProductId = this.getProductId(ebsProduct);
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
            //get current incident status id and convert to EBS status id
            selected_rn_request_status_id = Incident.Status.StatusID;
            selected_server_request_status_id = sr.rnStatusToServerStatus(selected_rn_request_status_id).Key;

            //get current incident severity id and convert to EBS severity id
            if (!Incident.SeverityID.Equals(null))
            {
                selected_rn_severity_id = (int)Incident.SeverityID;
            }
            else
            {
                selected_rn_severity_id = 0;
            }
            selected_server_severity_id = sr.rnSeverityToServerSeverity(selected_rn_severity_id).Key;

            //get current incident type id and serial number
            selected_rn_request_type_id = 0;

            string[] incCustomAttrs = { "Accelerator$ebs_sr_request_type"};
            Dictionary<String, Object> incCustomAttrsResults = CustomAttrHelper.fetchCustomAttrValue(incCustomAttributes, incCustomAttrs, this._logIncidentId, 0);
            selected_rn_request_type_id = incCustomAttrsResults["Accelerator$ebs_sr_request_type"] != null ? (int)incCustomAttrsResults["Accelerator$ebs_sr_request_type"] : 0;   
            
            //convert to EBS type id 
            selected_server_request_type_id = sr.rnRequestTypeToServerRequestType(selected_rn_request_type_id).Key;

            logMessage = "In CheckIncidentUpdates, get all current value.";
            logNote = "incident status id = " + selected_rn_request_status_id + ", sr status id = " + selected_server_request_status_id +
                "incident severity id = " + selected_rn_request_type_id + ", sr severity id = " + selected_server_severity_id +
                "incident type id = " + selected_rn_request_type_id + ", sr type id = " + selected_server_request_type_id +
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
            string[] incCustomAttrSrId = { "Accelerator$ebs_sr_id" };
            Dictionary<String, Object> incCustomAttrResultSrId = CustomAttrHelper.fetchCustomAttrValue(incCustomAttributes, incCustomAttrSrId, this._logIncidentId, 0);
            sr_id = incCustomAttrResultSrId["Accelerator$ebs_sr_id"] != null ? incCustomAttrResultSrId["Accelerator$ebs_sr_id"].ToString() : "";

            selectedContactPartyId = "0";
            selectedContactOrgId = "0";

            IList<ICustomAttribute> conCustomAttributes = Contact.CustomAttributes;
            string[] conCustomAttrs = { "Accelerator$ebs_contact_party_id", "Accelerator$ebs_contact_org_id" };
            Dictionary<String, Object> conCustomAttrResults = CustomAttrHelper.fetchCustomAttrValue(conCustomAttributes, conCustomAttrs, this._logIncidentId, 0);
            selectedContactPartyId = conCustomAttrResults["Accelerator$ebs_contact_party_id"] != null ? conCustomAttrResults["Accelerator$ebs_contact_party_id"].ToString() : "0";
            selectedContactOrgId = conCustomAttrResults["Accelerator$ebs_contact_org_id"] != null ? conCustomAttrResults["Accelerator$ebs_contact_org_id"].ToString() : "0";


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
                    // Create Service Request and update Accelerator$ebs_sr_id field of incident
                    SaveToEBS(true);

                    incCustomAttrResultSrId = CustomAttrHelper.fetchCustomAttrValue(incCustomAttributes, incCustomAttrSrId, this._logIncidentId, 0);
                    sr_id = incCustomAttrResultSrId["Accelerator$ebs_sr_id"] != null ? incCustomAttrResultSrId["Accelerator$ebs_sr_id"].ToString() : "";

                }

            }
            else if (!String.IsNullOrWhiteSpace(sr_id) && (CheckIncidentUpdates()))
            {
                logMessage = "Incident is updated. Need to update associated Service Request. SR ID = " + sr_id;
                logNote = "";
                _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);

                SaveToEBS(false);

                incCustomAttrResultSrId = CustomAttrHelper.fetchCustomAttrValue(incCustomAttributes, incCustomAttrSrId, this._logIncidentId, 0);
                sr_id = incCustomAttrResultSrId["Accelerator$ebs_sr_id"] != null ? incCustomAttrResultSrId["Accelerator$ebs_sr_id"].ToString() : "";

            }

            if (!String.IsNullOrWhiteSpace(sr_id))
            {
                int currentThreadCount = Incident.Threads.Count == 0 ? 0 : Incident.Threads[0].ID;
                int storedThreadsCount = 0;

                string[] incCustomAttrThread = { "Accelerator$ebs_max_thread_id" };
                Dictionary<String, Object> incCustomAttrResultThread= CustomAttrHelper.fetchCustomAttrValue(incCustomAttributes, incCustomAttrThread, this._logIncidentId, 0);
                storedThreadsCount = incCustomAttrResultThread["Accelerator$ebs_max_thread_id"] != null ? (int)incCustomAttrResultThread["Accelerator$ebs_max_thread_id"] : 0;


                if (currentThreadCount != storedThreadsCount && !String.IsNullOrWhiteSpace(sr_id))
                {
                    logMessage = "Need to store new threads to EBS System. Stored Thread Count = " + storedThreadsCount + "; Current Thread Count = " + currentThreadCount;
                    logNote = "";
                    _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);
                    SaveInteractionToEBS(sr_id, currentThreadCount, storedThreadsCount, false);
                }
            }

            if (notesTruncated != "")
                MessageBox.Show("This incident contains the following note(s) that will be truncated to first 1k characters in EBS.\nPlease summarize the truncated detail in a private note for EBS Agents as appropriate.\n" + notesTruncated);

            //_recordContext.RefreshWorkspace();

        }
      
        //Check whether update status, severity, type and serial number fields
        //Since all these field are set in standard incident fields instead of addin, need to compare  current value with value in EBS
        bool CheckIncidentUpdates()
        {
            if (current_serial_num != ebsStoredSerialNum)
            {
                return true;
            }

            if (currentInstanceID != ebsStoredInstanceID)
            {
                return true;
            }


            if (current_subject != storedSubject)
            {
                return true;
            }

            if (selected_server_request_status_id != storedRequestStatusId.ToString())
            {
                return true;
            }

            if ( selected_server_severity_id!= storedSeverityId.ToString())
            {
                return true;
            }


            if (selected_server_request_type_id != storedRequestTypeId.ToString())
            {
                return true;
            }

            string logMessage = "Do not need to update Service Request. The fields changed in incident did not impact Service Request value.";
            _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage);
            return false;
        }

        //Current product logic is not decided, implement it later.
        public string getProductId(String ebsInventoryItemName)
        {
            String rnProductId = null;

            if (String.IsNullOrEmpty(ebsInventoryItemName))
            {
                return null;
            }

            //Note for this sample to work we assume there is an organization with a contact assigned
            String queryString = "SELECT S.ServiceProduct.ID FROM SalesProduct S WHERE S.PARTNUMBER= '" + ebsInventoryItemName + "'";
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
            string[] conCustomAttrs = { "Accelerator$ebs_contact_org_id" };
            Dictionary<String, Object> conCustomAttrResults = CustomAttrHelper.fetchCustomAttrValue(conCustomAttributes, conCustomAttrs, this._logIncidentId, 0);
            string contactOrgId = conCustomAttrResults["Accelerator$ebs_contact_org_id"] != null ? conCustomAttrResults["Accelerator$ebs_contact_org_id"].ToString() : "0";

            if (contactOrgId == "0")
            {
                DialogResult result = MessageBox.Show("Current contact did not associate to a EBS contact correctly. Cannot do the serial number validation. ", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (result == DialogResult.OK)
                {
                    string logMessage = "Current contact did not associate to a EBS contact correctly. Cannot do the serial number validation.";
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
                if (cusAttr.PackageName == "Accelerator" && cusAttr.GenericField.Name == "Accelerator$ebs_serial_number")
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
                string logMessage = "Custom attribute is not defined. Cannot get Accelerator$ebs_serial_number.";
                _log.ErrorLog(incidentId: _logIncidentId, logMessage:logMessage);
                MessageBox.Show("Custom Attribute configuration missing. Please check log for detail.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        // Update Custom Attribute via SOAP API
        // After create/update incident, the custom attribute may need to update.
        protected void UpdateIncCustomAttr(int incidentID, string key, string value)
        {
            //Create an Incident object
            Accelerator.EBS.SharedServices.RightNowServiceReference.Incident updateIncident = new Accelerator.EBS.SharedServices.RightNowServiceReference.Incident();


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
                    case "ebs_sr_id":
                        customField.name = "ebs_sr_id";
                        customField.dataType = DataTypeEnum.INTEGER;
                        customField.dataTypeSpecified = true;
                        customField.DataValue = new DataValue();
                        customField.DataValue.Items = new object[1];
                        customField.DataValue.ItemsElementName = new ItemsChoiceType[1];
                        customField.DataValue.Items[0] = Convert.ToInt32(value);
                        customField.DataValue.ItemsElementName[0] = ItemsChoiceType.IntegerValue;
                        break;
                    case "ebs_sr_num":
                        customField.name = "ebs_sr_num";
                        customField.dataType = DataTypeEnum.STRING;
                        customField.dataTypeSpecified = true;
                        customField.DataValue = new DataValue();
                        customField.DataValue.Items = new object[1];
                        customField.DataValue.ItemsElementName = new ItemsChoiceType[1];
                        customField.DataValue.Items[0] = value;
                        customField.DataValue.ItemsElementName[0] = ItemsChoiceType.StringValue;
                        break;
                    case "ebs_max_thread_id":
                        customField.name = "ebs_max_thread_id";
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

        private void SaveToEBS(bool isCreate)
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
                selected_server_severity_id = severityKeyPair.Key;
                sr.SeverityID = Convert.ToDecimal(severityKeyPair.Key);
                sr.Severity = severityKeyPair.Value;

                KeyValuePair<String, String> statusKeyPair = sr.rnStatusToServerStatus(selected_rn_request_status_id);
                selected_server_request_status_id = statusKeyPair.Key;
                sr.StatusID = Convert.ToDecimal(statusKeyPair.Key);
                sr.Status = statusKeyPair.Value;

                KeyValuePair<String, String> requestTypeKeyPair = sr.rnRequestTypeToServerRequestType(selected_rn_request_type_id);
                selected_server_request_type_id = requestTypeKeyPair.Key;
                sr.RequestTypeID = Convert.ToDecimal(requestTypeKeyPair.Key);
                sr.RequestType = requestTypeKeyPair.Value;

                sr.EbsContactID = selectedContactPartyId;
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

                //String ebsProductId = (Incident.ProductID.HasValue)?Incident.ProductID.ToString():null;
                if(currentInstanceID != 0){
                    sr.ProductID = currentInstanceID;
                }
                else
                {
                    sr.ProductID = null;
                }

                if (_ebsServiceUserId >= 0)
                {
                    sr.CreatedByID = Convert.ToDecimal(_ebsServiceUserId);
                }
                if (_ebsDefaultSrOwnerId >= 0)
                {
                    sr.OwnerID = Convert.ToDecimal(_ebsDefaultSrOwnerId);
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
                        var sr_id = 0;
                        var sr_num = "";
                        var sr_obj_ver_num = 0;
                        var sr_owner_id = "";
                        IList<ICustomAttribute> incCustomAttributes = Incident.CustomAttributes;
                        string[] incCustomAttrs = { "Accelerator$ebs_sr_id", "Accelerator$ebs_sr_num", "Accelerator$ebs_sr_obj_ver_num", "Accelerator$ebs_sr_owner_id"};
                        Dictionary<String, Object> incCustomAttrsResults = CustomAttrHelper.fetchCustomAttrValue(incCustomAttributes, incCustomAttrs, this._logIncidentId, 0);
                        sr_id = incCustomAttrsResults["Accelerator$ebs_sr_id"] != null ? (int)incCustomAttrsResults["Accelerator$ebs_sr_id"] : 0;
                        sr_num = incCustomAttrsResults["Accelerator$ebs_sr_num"] != null ? incCustomAttrsResults["Accelerator$ebs_sr_num"].ToString() : "";
                        sr_obj_ver_num = incCustomAttrsResults["Accelerator$ebs_sr_obj_ver_num"] != null ? (int)incCustomAttrsResults["Accelerator$ebs_sr_obj_ver_num"] : 0;
                        sr_owner_id = incCustomAttrsResults["Accelerator$ebs_sr_owner_id"] != null ? incCustomAttrsResults["Accelerator$ebs_sr_owner_id"].ToString() : "";

                        sr.RequestID = Convert.ToDecimal(sr_id);
                        sr.RequestNumber = sr_num;
                        sr.SrObjVerNum = Convert.ToDecimal(sr_obj_ver_num);
                        if (sr_owner_id != "")
                        {
                            sr.OwnerID = Convert.ToDecimal(sr_owner_id);
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
                    //string message = "There has been an error communicating with EBS. Please check log for detail.";
                    MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!sr_saved)
                {
                    // If Service Request is not saved successfully, show error.
                    string message = "There has been an error communicating with EBS. Please check log for detail.";
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

                    if (isCreate)
                    {
                        //Check the interaction, we need created, after created a new service request
                        string sr_id = sr.RequestID.Value.ToString();
                        int currentThreadCount = Incident.Threads.Count == 0 ? 0 : Incident.Threads[0].ID;
                        int storedThreadsCount = 0;
                        IList<ICustomAttribute> incCustomAttributes = Incident.CustomAttributes;
                        string[] incCustomAttrThread = { "Accelerator$ebs_max_thread_id" };
                        Dictionary<String, Object> incCustomAttrResultThread = CustomAttrHelper.fetchCustomAttrValue(incCustomAttributes, incCustomAttrThread, this._logIncidentId, 0);
                        storedThreadsCount = incCustomAttrResultThread["Accelerator$ebs_max_thread_id"] != null ? (int)incCustomAttrResultThread["Accelerator$ebs_max_thread_id"] : 0;
                        

                        // If have new thread, then call function SaveInteractionToEBS to create interaction
                        if (currentThreadCount != storedThreadsCount && !String.IsNullOrWhiteSpace(sr_id))
                        {
                            logMessage = "After created new SR, need to store new threads to EBS System. Stored Thread Count = " + storedThreadsCount + "; Current Thread Count = " + currentThreadCount;
                            logNote = "";
                            _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);

                            SaveInteractionToEBS(sr_id, currentThreadCount, storedThreadsCount, true);
                        }

                        // Iterate through the incident custom attributes and set values
                        try
                        {
                            bool sr_id_found = false;
                            bool sr_num_found = false;
                            bool sr_owner_id_found = false;
                            foreach (ICustomAttribute cusAttr in incCustomAttributes)
                            {
                                if (cusAttr.PackageName == "Accelerator" && cusAttr.GenericField.Name == "Accelerator$ebs_sr_id")
                                {
                                    sr_id_found = true;
                                    cusAttr.GenericField.DataValue.Value = Convert.ToInt32(sr.RequestID);
                                    UpdateIncCustomAttr(Incident.ID, "ebs_sr_id", Convert.ToString(sr.RequestID));
                                }
                                if (cusAttr.PackageName == "Accelerator" && cusAttr.GenericField.Name == "Accelerator$ebs_sr_num")
                                {
                                    sr_num_found = true;
                                    cusAttr.GenericField.DataValue.Value = sr.RequestNumber;
                                    UpdateIncCustomAttr(Incident.ID, "ebs_sr_num", sr.RequestNumber);
                                }
                                if (cusAttr.PackageName == "Accelerator" && cusAttr.GenericField.Name == "Accelerator$ebs_sr_owner_id")
                                {
                                    sr_owner_id_found = true;
                                    cusAttr.GenericField.DataValue.Value = sr.RequestNumber;
                                    UpdateIncCustomAttr(Incident.ID, "ebs_sr_owner_id", Convert.ToString(sr.OwnerID));
                                }
                            }

                            if (sr_id_found == false)
                            {
                                logMessage = "Custom attribute is not defined. Cannot get Accelerator$ebs_sr_id.";
                                _log.ErrorLog(incidentId:_logIncidentId, logMessage: logMessage);
                            }

                            if (sr_num_found == false)
                            {
                                logMessage = "Custom attribute is not defined. Cannot get Accelerator$ebs_sr_num.";
                                _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage);
                            }

                            if (sr_owner_id_found == false)
                            {
                                logMessage = "Custom attribute is not defined. Cannot get Accelerator$ebs_sr_owner_id.";
                                _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage);
                            }

                            if (sr_id_found == false || sr_num_found == false || sr_owner_id_found == false)
                            {
                                MessageBox.Show("Custom Attribute configuration missing. Please check log for detail.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                            storedRequestTypeId = Convert.ToInt32(selected_server_request_type_id);
                            storedRequestStatusId = Convert.ToInt32(selected_server_request_status_id);
                            storedSeverityId = Convert.ToInt32(selected_server_severity_id);
                            ebsStoredSerialNum = current_serial_num;
                        }
                        catch (Exception ex)
                        {
                            logMessage = "Error in updating incident fields with Service Request information, after created new SR.Error Message: " + ex.Message;
                            _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage);
                            return;
                        }

                        logMessage = " Updating incident fields with Service Request information, after created new SR successfully.ebs_sr_id = " + sr.RequestID.ToString() + "; ebs_sr_num = " + sr.RequestNumber + "; ebs_sr_owner_id = " + sr.OwnerID.ToString() + ".";
                        _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage);
                    }
                    else
                    {
                        // After update successfully, update the values which are stored information from EBS side
                        try
                        {
                            storedRequestTypeId = Convert.ToInt32(selected_server_request_type_id);
                            storedRequestStatusId = Convert.ToInt32(selected_server_request_status_id);
                            storedSeverityId = Convert.ToInt32(selected_server_severity_id);
                            ebsStoredSerialNum = current_serial_num;
                        }
                        catch (Exception ex)
                        {
                            logMessage = "Error in updating incident fields with Service Request information, after updated  SR. Error Message: " + ex.Message;
                            _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage);
                            return;
                        }
                    }

                    //_recordContext.RefreshWorkspace();


                }
            }
        }

        // Save CX thread to EBS side
        private void SaveInteractionToEBS(string sr_id, int currentThreadCount, int storedThreadCount, bool includeCustomerThreads)
        {
            IThread thread;
            Interaction interaction;
            bool interaction_saved = false;
            string logMessage, logNote;
            // Get thread count of current incident
            int i = Incident.Threads.Count;
            while (i > 0)
            {
                i--;
                thread = Incident.Threads[i];

                // If thread id is larger than last time synced thread id, may need save it to EBS.
                if (thread.ID <= storedThreadCount)
                {
                    continue;
                }
                
                // If thread is created by Custom Portal, ignore it.
                if (thread.AcctID == null && thread.ChanID == 6 && includeCustomerThreads == false)
                {
                    logMessage = "Thread is created from CP. Do not need to propagate to EBS again. Thread ID = " + thread.ID;
                    _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage);
                    storedThreadCount++;
                    continue;
                }

                // If thread is created by CWSS_API_EBS_Service_User as a private note, ignore it.
                if (thread.AcctID == ConfigurationSetting.cwssApiEbsServiceUserId && thread.ChanID == null)
                {
                    logMessage = "Thread is created from EBS side. Do not need to propagate to EBS again. Thread ID = " + thread.ID;
                    _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage);
                    storedThreadCount++;
                    continue;
                }

                // Set Interaction Object 
                interaction = new Interaction();
                //interaction._logIncidentId = _logIncidentId;
                //interaction._logContactId = 0;

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
                        MessageBox.Show("There has been an error communicating with EBS. Please check log for detail.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }


                    string[] name = rowData[0].Split(',');
                    interaction.Author = name[0] + " " + name[1];
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
                        MessageBox.Show("There has been an error communicating with EBS. Please check log for detail.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    string[] name = rowData[0].Split(',');
                    interaction.Author = name[0] + " " + name[1];
                }

                string content = thread.Note;
                if (thread.ContentType == RightNow.AddIns.Common.ThreadContentType.HTML)
                {
                    string note = thread.Note;
                    string note_raw = Regex.Replace(note, "<.*?>", string.Empty);
                    if(note_raw.Length > 0)
                        note_raw =  note_raw.TrimStart('\n').TrimEnd('\n');
                    content = WebUtility.HtmlDecode(note_raw);
                }

                // truncate up to 1000 excluding ... (total would be 1003)
                if (content.Length > 1000)
                {
                    content = content.Substring(0, 1000);
                    content += "...";
                    logMessage = "Thread ID = " + thread.ID + " content: \"" + content.Substring(0, 50) + "\" is truncated to first 1k characters in EBS";
                    _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage);

                    // can be more than one, the caller _recordContext_Saved will show the list
                    notesTruncated += "- Note: \" " + content.Substring(0, 50) + " ...\" \n"; 
                }

                interaction.Content = content;
                interaction.Created = (DateTime)thread.Entered;
                interaction.SrID = Convert.ToDecimal(sr_id);
                string channel = null;
                switch (thread.ChanID)
                {
                    case 1:
                        channel = "Service Mailbox";
                        break;
                    case 2:
                        channel = "Marketing Mailbox";
                        break;
                    case 3:
                        channel = "Phone";
                        break;
                    case 4:
                        channel = "Fax";
                        break;
                    case 5:
                        channel = "Mail";
                        break;
                    case 6:
                        channel = "Service web form";
                        break;
                    case 7:
                        channel = "Marketing web form";
                        break;
                    case 8:
                        channel = "Chat";
                        break;
                    case 9:
                        channel = "E-mail";
                        break;
                    default:
                        channel = "No Channel";
                        break;
                }

                interaction.Channel = channel;
                if (thread.EntryType == 1)
                    interaction.Status = "P";
                else
                    interaction.Status = "E";

                interaction.Summary = interaction.Author + " via [" + channel + "]";

                if (thread.AcctID != null)
                {
                    if(_ebsServiceUserId == -1){
                        MessageBox.Show("There has been an error communicating with EBS. Please check log for detail.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        logMessage = "Cannot propagate the note created by agent to EBS. ebs_service_user_id is missing in Configration verb. Thread ID = " + thread.ID;
                        _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage);
                        return;
                    }
                    else
                    {
                        interaction.CreatedByID = Convert.ToDecimal(_ebsServiceUserId);
                    }

                }
                else if(thread.CId != null)
                {
                    interaction.CreatedByID = 0;
                }

                try
                {
                    // Create Interaction in EBS
                    logMessage = "Ready to propagate thread to EBS. Thread ID = " + thread.ID;
                    _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage);

                    interaction_saved = interaction.Create(_logIncidentId, 0);

                }
                catch (Exception ex)
                {
                    logMessage = "Error in Propagating thread. Thread ID = " + thread.ID + "; SR ID = " + sr_id + "; Exception: " + ex.Message;
                    _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage);

                    interaction_saved = false;

                    string message = "There has been an error communicating with EBS. Please check log for detail.";
                    MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }

                if (interaction_saved == false)
                {
                    logMessage = "Thread is not saved in EBS. Thread ID = " + thread.ID + "; SR ID = " + sr_id + "; Response Error Message: " + interaction.ErrorMessage;
                    _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage);

                    MessageBox.Show("There has been an error communicating with EBS. Please check log for detail.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
                else
                {
                    logMessage = "Thread is saved in EBS. Thread ID = " + thread.ID + "; SR ID = " + sr_id;
                    _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage);
                }

                storedThreadCount = thread.ID;
            }

            //Save current max thread id after creating thread
            IList<ICustomAttribute> incCustomAttributes = Incident.CustomAttributes;
            bool max_thread_id = false;
            foreach (ICustomAttribute cusAttr in incCustomAttributes)
            {
                if (cusAttr.PackageName == "Accelerator" && cusAttr.GenericField.Name == "Accelerator$ebs_max_thread_id")
                {
                    max_thread_id = true;
                    cusAttr.GenericField.DataValue.Value = storedThreadCount;
                    UpdateIncCustomAttr(Incident.ID, "ebs_max_thread_id", Convert.ToString(storedThreadCount));
                    
                    logMessage = "Save current max thread id after creating thread. Thread count is " + cusAttr.GenericField.DataValue.Value;
                    _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage);
                    break;
                }
            }

            if (max_thread_id == false)
            {
                logMessage = "Custom attribute is not defined. Cannot get Accelerator$ebs_sr_id.";
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
        private int _ebsServiceUserId;
        private int _ebsDefaultSrOwnerId;

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
                _ebsServiceUserId = ConfigurationSetting.ebsServiceUserId;
                _ebsDefaultSrOwnerId = ConfigurationSetting.ebsDefaultSrOwnerId;

                ServiceRequest.ServiceProvider = ConfigurationSetting.EBSProvider;
                ServiceRequest.CreateUpdateURL = ConfigurationSetting.CreateSR_WSDL;
                ServiceRequest.LookupURL = ConfigurationSetting.LookupSR_WSDL;
                ServiceRequest.ServiceUsername = String.IsNullOrEmpty(_usr) ? "ebusiness" : _usr;
                ServiceRequest.ServicePassword = String.IsNullOrEmpty(_pwd) ? "password" : _pwd;
                ServiceRequest.ServiceClientTimeout = ConfigurationSetting.EBSServiceTimeout;
                ServiceRequest.InitEBSProvider();

                Interaction.ServiceProvider = ConfigurationSetting.EBSProvider;
                Interaction.CreateInteractionURL = ConfigurationSetting.CreateInteraction_WSDL;
                Interaction.ServiceUsername = String.IsNullOrEmpty(_usr) ? "ebusiness" : _usr;
                Interaction.ServicePassword = String.IsNullOrEmpty(_pwd) ? "password" : _pwd;
                Interaction.ServiceClientTimeout = ConfigurationSetting.EBSServiceTimeout;
                Interaction.InitEBSProvider();

                Item.ServiceProvider = ConfigurationSetting.EBSProvider;
                Item.ListURL = ConfigurationSetting.ItemList_WSDL;
                Item.ServiceUsername = ConfigurationSetting.username;
                Item.ServicePassword = ConfigurationSetting.password;
                Item.ServiceClientTimeout = ConfigurationSetting.EBSServiceTimeout;
                Item.InitEBSProvider();
            }
            _wsAddIn = new ServiceRequestAddIn(inDesignMode, _rContext);

            _wsAddIn._rnSrv = _rnSrv;
            _wsAddIn._log = _log;
            _wsAddIn._ebsServiceUserId = _ebsServiceUserId;
            _wsAddIn._ebsDefaultSrOwnerId = _ebsDefaultSrOwnerId;

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
            get { return "EBSServiceRequestWorkspaceAddIn"; }
        }

        /// <summary>
        /// The tooltip displayed when hovering over the Add-In in the Ribbon of the Workspace Designer.
        /// </summary>
        public string Tooltip
        {
            get { return "EBS Service Request WorkspaceAddIn Tooltip"; }
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