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
 *  date: Wed Sep  2 23:14:37 PDT 2015

 *  revision: rnw-15-8-fixes-release-01
 *  SHA1: $Id: 13d051e80f813c7a87d28494e2e3d1116aa2311e $
 * *********************************************************************************************
 *  File: ContactSearchAddIn.cs
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
using System.Reflection;
using RightNow.AddIns.AddInViews;
using System.ServiceModel;
using Accelerator.Siebel.SharedServices.RightNowServiceReference;
using Accelerator.Siebel.SharedServices.Logs;
using System.ServiceModel.Channels;
using RightNow.AddIns.Common;
using System.IO;

namespace Accelerator.Siebel.ContactSearchAddIn
{
    public class ContactWorkspaceAddIn : Panel, IWorkspaceComponent2
    {
        /// <summary>
        /// The current workspace record context.
        /// </summary>
        private IRecordContext _recordContext;
        public bool _inDesingMode;

        // Create an instance of the custom user control
        private SiebelContactSearchControl _siebelContactSearchControl;

        // Configuration
        public IGlobalContext _globalContext;

        // Config verb fields
        public string _endpoint;
        public string _siebelUser;
        public string _siebelPwd;
        public int _contactSearchReportId = 0;

        // RN SOAP
        public RightNowService _rnSrv;

        // Log
        public Accelerator.Siebel.SharedServices.Logs.LogWrapper _log;
        public int _logIncidentId;
        public int _logContactId;

        protected bool _enableAutoSearchInChat = false;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="inDesignMode">Flag which indicates if the control is being drawn on the Workspace Designer. (Use this flag to determine if code should perform any logic on the workspace record)</param>
        /// <param name="RecordContext">The current workspace record context.</param>
        public ContactWorkspaceAddIn(bool inDesignMode, IRecordContext RecordContext, IGlobalContext GlobalContext)
        {
            // do nothing so framework won't throw exception once it gets to GetControl
            if (!ConfigurationSetting.configVerbPerfect && ConfigurationSetting.loginUserIsAdmin)
            {
                // do nothing
            }
            else
            {
                _recordContext = RecordContext;
                _globalContext = GlobalContext;
                _inDesingMode = inDesignMode;
                if (_recordContext != null)
                {
                    _recordContext.DataLoaded += _recordContext_DataLoaded;
                }
                // Instantiate the custom control and add it to the panel controls
                _siebelContactSearchControl = new SiebelContactSearchControl(_recordContext, _globalContext);
            }
        }

        #region IAddInControl Members

        /// <summary>
        /// Method called by the Add-In framework to retrieve the control.
        /// </summary>
        /// <returns>The control, typically 'this'.</returns>
        public Control GetControl()
        {   // return empty control so framework won't throw exception
            if (!ConfigurationSetting.configVerbPerfect && ConfigurationSetting.loginUserIsAdmin)
                return new Control();

            else
            {
                _siebelContactSearchControl._rnSrv = _rnSrv;
                _siebelContactSearchControl._log = _log;
                _siebelContactSearchControl._contactSearchReportId = _contactSearchReportId;
                return this._siebelContactSearchControl;
            }
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
            string logMessage = "Invoke Action : " + ActionName;
            _log.DebugLog(logMessage: logMessage);

            // contact search with AddIn WS has 2 action rules
            if (ActionName == "searchSiebelContact")
            {
                string _wsTypeName = this._recordContext.WorkspaceTypeName;

                if (_wsTypeName == "Chat")
                {
                    this._enableAutoSearchInChat = true;
                }
                else
                {
                    _siebelContactSearchControl.autoSearchParameterInvoke();
                }

            }
            else if (ActionName == "searchSiebelNewContactCtiPhone")
            {
                try
                {
                    _siebelContactSearchControl.autoSearchNewContactInvoke();
                }
                catch (FileNotFoundException ex)
                {
                    /* for case where CTI is not enabled, and it will throw
                     * FileNotFoundException (dll not loaded). Simply ignore 
                     * it, so that the normal new Contact can resume.
                     */
                }
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

        internal void EnableControl(IIncident incident = null, IContact contact = null, IChat chat = null)
        {           
            this.Controls.Clear();
            _siebelContactSearchControl.contactRecord = contact;
            _siebelContactSearchControl.incidentRecord = incident;
            _siebelContactSearchControl.chatRecord = chat;

            this._logIncidentId = 0;
            this._logContactId = 0;
            if (incident != null)
                this._logIncidentId = incident.ID;
            else if (contact != null)
                this._logContactId = contact.ID;

            _siebelContactSearchControl._logIncidentId = _logIncidentId;
            _siebelContactSearchControl._logContactId = _logContactId;

        }

        void _recordContext_DataLoaded(object sender, System.EventArgs e)
        {
            WorkspaceRecordType recordType = _recordContext.WorkspaceType;

            if (recordType == WorkspaceRecordType.Incident)
            {
                IIncident i = _recordContext.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Incident) as IIncident;
                IContact c = _recordContext.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Contact) as IContact;
                this.EnableControl(incident: i, contact: c);
                return;
            }
            else if (recordType == WorkspaceRecordType.Contact)
            {
                IContact c = _recordContext.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Contact) as IContact;
                this.EnableControl(contact: c);
                return;
            }
            else
            {
                IChat chat = _recordContext.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Chat) as IChat;
                this.EnableControl(chat: chat);
                
                if (this._enableAutoSearchInChat == true)
                {
                    _siebelContactSearchControl.autoSearchParameterInvoke();
                }
                return;
            }

        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && (_recordContext != null))
            {
                // unsubscribe from all the events
                _recordContext.DataLoaded -= _recordContext_DataLoaded;
            }
            base.Dispose(disposing);
        }

    }

    [AddIn("Contact Search AddIn", Version = "1.0.0.0")]
    public class ContactSearchAddInFactory : IWorkspaceComponentFactory2
    {
        #region IWorkspaceComponentFactory2 Members

        public IGlobalContext _globalContext;
        public IRecordContext _rContext;
        private ContactWorkspaceAddIn _wsAddIn;
        private String _usr;
        private String _pwd;
        private  RightNowSyncPortClient _client;
        private RightNowService _rnSrv;
        private Accelerator.Siebel.SharedServices.Logs.LogWrapper _log;
       
        public int Contact_Party_ID_Field { get; set; }
        public int Contact_Search_Report_ID { get; set; }
        public int Contact_Org_ID_Field { get; set; }
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
                    MessageBox.Show("Contact Search Add-In is not initialized properly. \nPlease contact your system administrator.\n You are now logged out.");
                    _globalContext.Logout();
                }
                else // don't want to logout admin
                {
                    MessageBox.Show("Contact Search Add-In is not loaded because of invalid configuration verb.");
                    return new ContactWorkspaceAddIn(inDesignMode, RecordContext, _globalContext);
                }
            }
            _rContext = RecordContext;
            if (!inDesignMode && RecordContext != null)
            {
                //Get configuration
                ConfigurationSetting instance = ConfigurationSetting.Instance(_globalContext);
                _usr = ConfigurationSetting.username;
                _pwd = ConfigurationSetting.password;
                _client = ConfigurationSetting.client;
                _rnSrv = ConfigurationSetting.rnSrv;
                _log = ConfigurationSetting.logWrap;

                Accelerator.Siebel.SharedServices.Contact.ServiceProvider = ConfigurationSetting.SiebelProvider;
                Accelerator.Siebel.SharedServices.Contact.ListLookupURL = ConfigurationSetting.LookupContactList_WSDL;
                Accelerator.Siebel.SharedServices.Contact.ServiceUsername = String.IsNullOrEmpty(_usr) ? "ebusiness" : _usr;
                Accelerator.Siebel.SharedServices.Contact.ServicePassword = String.IsNullOrEmpty(_pwd) ? "password" : _pwd;
                Accelerator.Siebel.SharedServices.Contact.ServiceClientTimeout = ConfigurationSetting.SiebelServiceTimeout;
                Accelerator.Siebel.SharedServices.Contact.InitSiebelProvider();

                Contact_Search_Report_ID = ConfigurationSetting.contactSearchReportID;
            }
            _wsAddIn = new ContactWorkspaceAddIn(inDesignMode, RecordContext, _globalContext);
            _wsAddIn._contactSearchReportId = Contact_Search_Report_ID;
            _wsAddIn._rnSrv = _rnSrv;
            _wsAddIn._log = _log;
            return _wsAddIn;
        }

        

        #endregion

        #region IFactoryBase Members

        /// <summary>
        /// The 16x16 pixel icon to represent the Add-In in the Ribbon of the Workspace Designer.
        /// </summary>
        public Image Image16
        {
            get { return Accelerator.Siebel.ContactSearchAddin.Properties.Resources.AddIn16; }
        }

        /// <summary>
        /// The text to represent the Add-In in the Ribbon of the Workspace Designer.
        /// </summary>
        public string Text
        {
            get { return "SiebelContactSearchAddIn"; }
        }

        /// <summary>
        /// The tooltip displayed when hovering over the Add-In in the Ribbon of the Workspace Designer.
        /// </summary>
        public string Tooltip
        {
            get { return "Siebel Contact Search WorkspaceAddIn Tooltip"; }
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
            _globalContext = GlobalContext;
            ConfigurationSetting.logAssemblyVersion(Assembly.GetAssembly(typeof(ContactSearchAddInFactory)), null);

            ConfigurationSetting instance = ConfigurationSetting.Instance(_globalContext);
            /* log it, but return true because will show the messagebox when the addin is opened in createControl.
             * if return false, the add-in is not loaded, and cannot show the error when add-in is opened.
             */ 
            if (!ConfigurationSetting.configVerbPerfect)
            {
                string logMessage = "ContactWorkspaceAddIn is not initialized properly because of invalid config verb.";
                ConfigurationSetting.logWrap.ErrorLog(logMessage: logMessage);
            }
            return true;
        }
        #endregion
    }
}