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
 *  date: Thu Nov 12 00:55:32 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: 6784c72412237230e48201bc72d3bbe8ea1a9959 $
 * *********************************************************************************************
 *  File: ActivityAddIn.cs
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

namespace Accelerator.Siebel.ActivityAddin
{
    public class ActivityAddIn : Panel, IWorkspaceComponent2
    {
        /// <summary>
        /// The current workspace record context.
        /// </summary>
        private IRecordContext _recordContext;
        public IGlobalContext GlobalContext { get; set; }
        public IIncident Incident { get; set; }
        public IContact Contact { get; set; }

        private ActivityInformationControl _activityControl;

        //Case Management System stored value


        //public RightNowSyncPortClient _rnowClient;
        public RightNowService _rnSrv;
        public LogWrapper _log;
        public int _logIncidentId;
        public string _siebelServiceUserId;
        public string _siebelDefaultSrOwnerId;


        private bool canAddNewActivity;


        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="inDesignMode">Flag which indicates if the control is being drawn on the Workspace Designer. (Use this flag to determine if code should perform any logic on the workspace record)</param>
        /// <param name="RecordContext">The current workspace record context.</param>
        public ActivityAddIn(bool inDesignMode, IRecordContext RecordContext)
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
                _activityControl = new ActivityInformationControl();
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
                return this._activityControl;
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
            if (ActionName == "CancelActivityCreation")
            {
                _activityControl.resetInputForm(); // clean up input form
                canAddNewActivity = false;
            }
            else if (ActionName == "AddNewActivity") {
                _activityControl.resetInputForm(); // clean up input form
                canAddNewActivity = true;
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
            this.Incident = incident;

            /// Set reference incident in log 
            this._logIncidentId = incident.ID;

            this.Controls.Clear();

            _activityControl.Name = "Activity";
            _activityControl.incident = incident;
            _activityControl._gc = GlobalContext;
            _activityControl._rc = _recordContext;
            _activityControl._log = _log;
            _activityControl._logIncidentId = _logIncidentId;

        }
        void _recordContext_DataLoaded(object sender, System.EventArgs e)
        {
            try
            {
                IIncident i = _recordContext.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Incident) as IIncident;
                this.EnableControl(i);
                canAddNewActivity = false;
            }
            catch (Exception ex)
            {
                string logMessage = "Error in loading Activity. Cancel Async Thread. Error: " + ex.Message;
                _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage);
            }
        }

        void _recordContext_Saving(object sender, CancelEventArgs e)
        {
        }

        // Triggered when the workspace is saved
        void _recordContext_Saved(object sender, EventArgs e)
        {
            //Update incident record because updated incident has been saved.
            Incident = _recordContext.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Incident) as IIncident;

            //get custom fields sr_id
            var sr_id = "";
            IList<ICustomAttribute> incCustomAttributes = Incident.CustomAttributes;
            string[] incCustomAttrSrId = { "Accelerator$siebel_sr_id" };
            Dictionary<String, Object> incCustomAttrResultSrId = CustomAttrHelper.fetchCustomAttrValue(incCustomAttributes, incCustomAttrSrId, this._logIncidentId, 0);
            sr_id = incCustomAttrResultSrId["Accelerator$siebel_sr_id"] != null ? incCustomAttrResultSrId["Accelerator$siebel_sr_id"].ToString() : "";


            if (!String.IsNullOrWhiteSpace(sr_id) && canAddNewActivity)
            {
                // Create Activity
                SaveActivityToSiebel(sr_id);
            }

           
        }


        // Save Activity to Siebel
        private void SaveActivityToSiebel(string sr_id)
        {
            string logMessage; 

            // check SR status,
            if (checkSRStatus(sr_id) == false)
            {
                return;
            }

            SharedServices.Activity activity = new Activity();
            activity.SrID = sr_id;
            activity.Comment = this._activityControl.InputComment;
            activity.Description = this._activityControl.InputDescription;
            activity.ActivityType = this._activityControl.InputType;
            activity.Due = this._activityControl.InputDue;
            activity.Priority = this._activityControl.InputPriority;
            activity.Status = this._activityControl.InputStatus;

            bool activity_saved;
            try
            {
                // Create Interaction in Siebel
                logMessage = "Ready to propagate activity to Siebel.";
                _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage);

                activity_saved = activity.Create(_logIncidentId, 0);

            }
            catch (Exception ex)
            {
                logMessage = "Error in Propagating activity." + "SR ID = " + sr_id + "; Exception: " + ex.Message;
                _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage);

                activity_saved = false;

                string message = "There has been an error communicating with Siebel. Please check log for detail.";
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (activity_saved == false)
            {
                logMessage = "Activity is not saved in Siebel.  SR ID = " + sr_id + "; Response Error Message: " + activity.ErrorMessage;
                _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage);

                MessageBox.Show("There has been an error communicating with Siebel. Please check log for detail.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                logMessage = "Activity is saved in Siebel. ID = " + activity.ID + "; SR ID = " + sr_id;
                _log.DebugLog(incidentId: _logIncidentId, logMessage: logMessage);
            }


        }

        private Boolean checkSRStatus(string sr_id)
        {

            string logMessage, logNote;

            if (String.IsNullOrWhiteSpace(sr_id))
                return false;

            // load SR
            ServiceRequest sr = new ServiceRequest();
            try
            {
                sr = sr.Lookup(sr_id, _logIncidentId, 0);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                logMessage = "Error in loading Service Request. Error: " + ex.Message;
                logNote = "";
                _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);
                return false;
            }

            if (sr.ErrorMessage != null)
            {
                logMessage = "Loading Service Request is failed. SR ID = " + sr_id;
                logNote = "Response shows error code when loading service request. Response's error message: " + sr.ErrorMessage;
                _log.ErrorLog(incidentId: _logIncidentId, logMessage: logMessage, logNote: logNote);

                return false;
            }

            if (sr.Status == "Closed")
            {
                String message = "Activity cannot be saved in Siebel because the Service Request is closed.";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                result = System.Windows.Forms.MessageBox.Show(this, message, "Warning", buttons, MessageBoxIcon.Warning);
                logMessage = "Cannot propagate the activity to Siebel.";
                logNote = message;
                _log.ErrorLog(logMessage: logMessage, logNote: logNote, incidentId: _logIncidentId);

                return false;
            }
            else
            {
                return true;
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

    [AddIn("Activity AddIn", Version = "1.0.0.0")]
    public class ActivityAddInFactory : IWorkspaceComponentFactory2
    {
        #region IWorkspaceComponentFactory2 Members
        private IRecordContext _rContext;
        private IGlobalContext _gContext;
        private ActivityAddIn _wsAddIn;
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
                    MessageBox.Show("Activity Add-In is not initialized properly. \nPlease contact your system administrator.\n You are now logged out.");
                    _gContext.Logout();
                }
                else // don't want to logout admin
                {
                    MessageBox.Show("Activity Add-In is not loaded because of invalid configuration verb.");
                    return new ActivityAddIn(inDesignMode, RecordContext);
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

                Activity.ServiceProvider = ConfigurationSetting.SiebelProvider;
                Activity.LookupURL = ConfigurationSetting.CreateSR_WSDL;
                Activity.ServiceUsername = String.IsNullOrEmpty(_usr) ? "ebusiness" : _usr;
                Activity.ServicePassword = String.IsNullOrEmpty(_pwd) ? "password" : _pwd;
                Activity.ServiceClientTimeout = ConfigurationSetting.SiebelServiceTimeout;
                Activity.InitSiebelProvider();
            }
            _wsAddIn = new ActivityAddIn(inDesignMode, _rContext);

            _wsAddIn._rnSrv = _rnSrv;
            _wsAddIn._log = _log;
            _wsAddIn._siebelServiceUserId = _siebelServiceUserId;


            if (_log != null)
            {
                string logMessage = "Activity AddIn is setup.";
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
            get { return "SiebelActivityWorkspaceAddIn"; }
        }

        /// <summary>
        /// The tooltip displayed when hovering over the Add-In in the Ribbon of the Workspace Designer.
        /// </summary>
        public string Tooltip
        {
            get { return "Siebel Activity WorkspaceAddIn Tooltip"; }
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
            ConfigurationSetting.logAssemblyVersion(Assembly.GetAssembly(typeof(ActivityAddInFactory)), null);

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