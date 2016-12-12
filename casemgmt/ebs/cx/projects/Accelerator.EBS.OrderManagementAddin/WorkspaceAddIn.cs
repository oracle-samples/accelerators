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
 *  date: Thu Nov 12 00:52:44 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: 0f9c4e4b001387a2516d4980eec0b8f07d03d341 $
 * *********************************************************************************************
 *  File: WorkspaceAddIn.cs
 * *********************************************************************************************/

using System.AddIn;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using RightNow.AddIns.AddInViews;
using Image = System.Drawing.Image;
using Accelerator.EBS.SharedServices;
using Accelerator.EBS.SharedServices.Logs;
using System.Reflection;
using RightNow.AddIns.Common;
using System.Collections.Generic;
using System;
using System.Text;
using System.IO;
using System.Windows;
using Accelerator.EBS.SharedServices.RightNowServiceReference;
using System.Diagnostics;

////////////////////////////////////////////////////////////////////////////////
//
// File: WorkspaceAddIn.cs
//
// Comments:
//
// Notes: 
//
// Pre-Conditions: 
//
////////////////////////////////////////////////////////////////////////////////
namespace Accelerator.EBS.OrderManagementAddin
{
    public class WorkspaceAddIn : Panel, IWorkspaceComponent2
    {

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="inDesignMode">Flag which indicates if the control is being drawn on the Workspace Designer. (Use this flag to determine if code should perform any logic on the workspace record)</param>
        /// <param name="RecordContext">The current workspace record context.</param>
        public WorkspaceAddIn(bool inDesignMode, IRecordContext recordContext, IGlobalContext globalContext, string version)
        {
            // do nothing so framework won't throw exception once it gets to GetControl
            if (!ConfigurationSetting.configVerbPerfect && ConfigurationSetting.loginUserIsAdmin)
            {
                // do nothing
            }
            else
            {
                _gContext = globalContext;
                _recordContext = recordContext;
                bulkImportControl = new OrderManagementControl(new Proxy(this));
                _Model = bulkImportControl._Model;
                _Model.InDesignMode = inDesignMode;
                _Model.Version = version;
                var elementHost = new ElementHost
                {
                    Dock = DockStyle.Fill,
                    Child = bulkImportControl,
                };

                Controls.Add(elementHost);
                if (inDesignMode)
                {
                    return;
                }

                //Get configuration
                ConfigurationSetting instance = ConfigurationSetting.Instance(globalContext);
                _usr = ConfigurationSetting.username;
                _pwd = ConfigurationSetting.password;
                _client = ConfigurationSetting.client;
                _rnSrv = ConfigurationSetting.rnSrv;

                Accelerator.EBS.SharedServices.Order.ServiceProvider = ConfigurationSetting.EBSProvider;
                Accelerator.EBS.SharedServices.Order.GetOrderURL = ConfigurationSetting.GetOrder_WSDL;
                Accelerator.EBS.SharedServices.Order.OrderInboundURL = ConfigurationSetting.OrderInboundURL_WSDL;
                Accelerator.EBS.SharedServices.Order.InitEBSProvider();

                _Model.EbsOwnerId = ConfigurationSetting.ebsDefaultSrOwnerId;

                _recordContext.DataLoaded += _rContext_DataLoaded;
                _recordContext.Closing += _recordContext_Closing;
                _recordContext.Saving += _recordContext_Saving;
                _gContext.AutomationContext.CurrentEditorTabChanged += AutomationContext_CurrentEditorTabChanged;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (null != _recordContext))
            {
                // unsubscribe from all the events
                _recordContext.DataLoaded -= _rContext_DataLoaded;
                _recordContext.Closing -= _recordContext_Closing;
                _recordContext.Saving -= _recordContext_Saving;
                _gContext.AutomationContext.CurrentEditorTabChanged -= AutomationContext_CurrentEditorTabChanged;
            }
            base.Dispose(disposing);
        }

        void AutomationContext_CurrentEditorTabChanged(object sender, EditorTabChangedEventArgs e)
        {
            _Model.Render(false);
        }

        void _recordContext_Saving(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _Model.Save();
        }

        void _recordContext_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_Model.IsDirty())
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("Some orders are not yet saved. Do you want to leave?",
                    string.Format("{0} Order Management", _Model.ServerType), MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (MessageBoxResult.No == result)
                    e.Cancel = true;
            }
        }

        void _rContext_DataLoaded(object sender, System.EventArgs e)
        {
            if (_Model.IsRunning)
            {
                _Model.Render(false);
                return;
            }
            WorkspaceRecordType wsType = _recordContext.WorkspaceType;

            if (WorkspaceRecordType.Incident == wsType)
            {
                _Incident = _recordContext.GetWorkspaceRecord(WorkspaceRecordType.Incident) as IIncident;
                _Contact = _recordContext.GetWorkspaceRecord(WorkspaceRecordType.Contact) as IContact;

                //get the ebs incident id                    
                if (null != _Incident)
                {
                    IList<ICustomAttribute> customAttributes = _Incident.CustomAttributes;
                    foreach (ICustomAttribute cusAttr in customAttributes)
                    {
                        if (cusAttr.PackageName == "Accelerator" && cusAttr.GenericField.Name == "Accelerator$ebs_sr_id")
                        {
                            object id = cusAttr.GenericField.DataValue.Value;
                            EbsIncidentId = (id != null) ? Convert.ToDecimal(id) : 0;
                        }
                    }
                }
            }

            // get ebs contact org id
            if (null != _Contact)
            {
                IList<ICustomAttribute> customAttributes = _Contact.CustomAttributes;
                foreach (ICustomAttribute cusAttr in customAttributes)
                {
                    if (cusAttr.PackageName == "Accelerator" && cusAttr.GenericField.Name == "Accelerator$ebs_contact_org_id")
                    {
                        object id = cusAttr.GenericField.DataValue.Value;
                        _EbsContactOrgId = (id != null) ? Convert.ToDecimal(id) : 0;
                    }
                }

            }

            _Model.EbsSrId = EbsIncidentId;
            _Model.EbsContactOrgId = _EbsContactOrgId;
            _Model.RntIncidentId = null == _Incident ? 0 : _Incident.ID;
            _Model.RntContactId = null == _Contact ? 0 : _Contact.ID;
            _Model.IsRecordLoaded = true;
            _Model.InDemo = false;
            //_Model.InDemo = (0 == EbsIncidentId || 0 == _Model.EbsOwnerId);
            _Model.Render(true);
        }

        #region IAddInControl Members

        /// <summary>
        /// Method called by the Add-In framework to retrieve the control.
        /// </summary>
        /// <returns>The control, typically 'this'.</returns>
        public Control GetControl()
        {
            return this;
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

        private IRecordContext _recordContext;
        private OrderManagementControl bulkImportControl;
        private IGlobalContext _gContext;
        private string _usr;
        private string _pwd;
        private SharedServices.RightNowServiceReference.RightNowSyncPortClient _client;
        internal RightNowService _rnSrv;
        private decimal EbsIncidentId { get; set; }
        internal IIncident _Incident { get; set; }
        private IContact _Contact { get; set; }
        internal OrderManagementViewModel _Model;
        private decimal _EbsContactOrgId;
    }

    [AddIn("EBS Order Management AddIn", Version = "1.0.0.0")]
    public class WorkspaceAddInFactory : IWorkspaceComponentFactory2
    {
        private IGlobalContext _gContext;
        private string _Version;
        #region IWorkspaceComponentFactory2 Members

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
                    System.Windows.Forms.MessageBox.Show("OrderManagementAddin is not initialized properly. \nPlease contact your system administrator.\n You are now logged out.");
                    _gContext.Logout();
                }
                else // don't want to logout admin
                {
                    System.Windows.Forms.MessageBox.Show("OrderManagementAddin is not loaded because of invalid configuration verb.");
                    return new WorkspaceAddIn(inDesignMode, RecordContext, _gContext, _Version);
                }
            }

            return new WorkspaceAddIn(inDesignMode, RecordContext, _gContext, _Version);
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
            get { return "EBS Order Management"; }
        }

        /// <summary>
        /// The tooltip displayed when hovering over the Add-In in the Ribbon of the Workspace Designer.
        /// </summary>
        public string Tooltip
        {
            get { return "EBS Order Management Addin"; }
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
            _Version = ConfigurationSetting.logAssemblyVersion(Assembly.GetAssembly(typeof(OrderManagementViewModel)), null);

            ConfigurationSetting instance = ConfigurationSetting.Instance(_gContext);
            /* log it, but return true because will show the messagebox when the addin is opened in createControl.
             * if return false, the add-in is not loaded, and cannot show the error when add-in is opened.
             */
            if (!ConfigurationSetting.configVerbPerfect)
            {
                string logMessage = "OrderManagementAddin is not initialized properly because of invalid config verb.";
                ConfigurationSetting.logWrap.ErrorLog(logMessage: logMessage);
            }
            return true;
        }

        #endregion
    }
}