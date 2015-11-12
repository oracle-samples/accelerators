/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + EBS Enhancement
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  EBS release: 12.1.3
 *  reference: 150505-000099, 150420-000127
 *  date: Thu Nov 12 00:52:42 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: b077028bd32ae0432ed58ec09ff45d252f9b0af1 $
 * *********************************************************************************************
 *  File: ContactSearchAddIn.cs
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
using System.Reflection;
using RightNow.AddIns.AddInViews;
using System.ServiceModel;
using Accelerator.EBS.SharedServices.RightNowServiceReference;
using Accelerator.EBS.SharedServices.Logs;
using System.ServiceModel.Channels;
using System.IO;

namespace Accelerator.EBS.ContactSearchAddIn
{
    public class ContactWorkspaceAddIn : Panel, IWorkspaceComponent2
    {
        /// <summary>
        /// The current workspace record context.
        /// </summary>
        private IRecordContext _recordContext;
        public bool _inDesingMode;

        // Create an instance of the custom user control
        private EBSContactSearchControl _ebsContactSearchControl;

        // Configuration
        public IGlobalContext _globalContext;

        // Config verb fields
        public string _endpoint;
        public string _ebsUser;
        public string _ebsPwd;
        public int _contactSearchReportId = 0;

        // RN SOAP
        public RightNowService _rnSrv;

        // Log
        public Accelerator.EBS.SharedServices.Logs.LogWrapper _log;
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
                _ebsContactSearchControl = new EBSContactSearchControl(_recordContext, _globalContext);
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
            {
                _ebsContactSearchControl._rnSrv = _rnSrv;
                _ebsContactSearchControl._log = _log;
                _ebsContactSearchControl._contactSearchReportId = _contactSearchReportId;
                return this._ebsContactSearchControl;
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
            if (ActionName == "searchEBSContact")
            {
                 string _wsTypeName = this._recordContext.WorkspaceTypeName;

                 if (_wsTypeName == "Chat")
                 {
                     this._enableAutoSearchInChat = true;
                 }
                 else
                 {
                     _ebsContactSearchControl.autoSearchParameterInvoke();
                 }
                
            }            
            else if (ActionName == "searchEBSNewContactCtiPhone")
            {
                try
                {
                    _ebsContactSearchControl.autoSearchNewContactInvoke();
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
            _ebsContactSearchControl.contactRecord = contact;
            _ebsContactSearchControl.incidentRecord = incident;
            _ebsContactSearchControl.chatRecord = chat;

            this._logIncidentId = 0;
            this._logContactId = 0;
            if (incident != null)
                this._logIncidentId = incident.ID;
            else if (contact != null)
                this._logContactId = contact.ID;

            _ebsContactSearchControl._logIncidentId = _logIncidentId;
            _ebsContactSearchControl._logContactId = _logContactId;

        }

        void _recordContext_DataLoaded(object sender, System.EventArgs e)
        {
            string recordTypeName = _recordContext.WorkspaceTypeName;

            if (recordTypeName == "Incident")
            {
                IIncident i = _recordContext.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Incident) as IIncident;
                IContact c = _recordContext.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Contact) as IContact;
                this.EnableControl(incident: i, contact: c);
                return;
            }
            if (recordTypeName == "Contact")
            {
                IContact c = _recordContext.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Contact) as IContact;
                this.EnableControl(contact: c);
                return;
            }
            if (recordTypeName == "Chat")
            {
                IChat chat = _recordContext.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Chat) as IChat;
                this.EnableControl(chat: chat);
                if (this._enableAutoSearchInChat == true)
                {
                    _ebsContactSearchControl.autoSearchParameterInvoke();
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
        private Accelerator.EBS.SharedServices.Logs.LogWrapper _log;
       
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

                Accelerator.EBS.SharedServices.ContactModel.ServiceProvider = ConfigurationSetting.EBSProvider;
                Accelerator.EBS.SharedServices.ContactModel.ListLookupURL = ConfigurationSetting.LookupContactList_WSDL;
                Accelerator.EBS.SharedServices.ContactModel.ServiceUsername = String.IsNullOrEmpty(_usr) ? "ebusiness" : _usr;
                Accelerator.EBS.SharedServices.ContactModel.ServicePassword = String.IsNullOrEmpty(_pwd) ? "password" : _pwd;
                Accelerator.EBS.SharedServices.ContactModel.ServiceClientTimeout = ConfigurationSetting.EBSServiceTimeout;
                Accelerator.EBS.SharedServices.ContactModel.InitEBSProvider();

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
            get { return Accelerator.EBS.ContactSearchAddin.Properties.Resources.AddIn16; }
        }

        /// <summary>
        /// The text to represent the Add-In in the Ribbon of the Workspace Designer.
        /// </summary>
        public string Text
        {
            get { return "EBSContactSearchAddIn"; }
        }

        /// <summary>
        /// The tooltip displayed when hovering over the Add-In in the Ribbon of the Workspace Designer.
        /// </summary>
        public string Tooltip
        {
            get { return "EBS Contact Search WorkspaceAddIn Tooltip"; }
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
