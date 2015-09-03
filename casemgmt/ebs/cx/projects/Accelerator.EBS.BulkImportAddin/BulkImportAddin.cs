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
 *  date: Wed Sep  2 23:11:36 PDT 2015

 *  revision: rnw-15-8-fixes-release-01
 *  SHA1: $Id: 47d257a3dff0744c0a9ecf9b64edd0cf1d762aad $
 * *********************************************************************************************
 *  File: BulkImportAddin.cs
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

namespace Accelerator.EBS.BulkImportAddin
{
    public class BulkImportAddin : System.Windows.Forms.Panel, IWorkspaceComponent2
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="inDesignMode">Flag which indicates if the control is being drawn on the Workspace Designer. (Use this flag to determine if code should perform any logic on the workspace record)</param>
        /// <param name="RecordContext">The current workspace record context.</param>
        public BulkImportAddin(bool inDesignMode, IRecordContext recordContext, IGlobalContext globalContext)
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
                bulkImportControl = new BulkImportControl(new Proxy(this));
                _Model = bulkImportControl._Model;
                _Model.InDesignMode = inDesignMode;
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

                Accelerator.EBS.SharedServices.Contact.ServiceProvider = ConfigurationSetting.EBSProvider;
                Accelerator.EBS.SharedServices.Contact.ListLookupURL = ConfigurationSetting.LookupContactList_WSDL;
                Accelerator.EBS.SharedServices.Contact.ServiceUsername = String.IsNullOrEmpty(_usr) ? "ebusiness" : _usr;
                Accelerator.EBS.SharedServices.Contact.ServicePassword = String.IsNullOrEmpty(_pwd) ? "password" : _pwd;
                Accelerator.EBS.SharedServices.Contact.InitEBSProvider();

                Accelerator.EBS.SharedServices.RepairOrder.ServiceProvider = ConfigurationSetting.EBSProvider;
                Accelerator.EBS.SharedServices.RepairOrder.ListLookupURL = ConfigurationSetting.LookupRepairList_WSDL;
                Accelerator.EBS.SharedServices.RepairOrder.ListURL = ConfigurationSetting.RepairOrderList_WSDL;
                Accelerator.EBS.SharedServices.RepairOrder.LookupURL = ConfigurationSetting.LookupRepair_WSDL;
                Accelerator.EBS.SharedServices.RepairOrder.CreateURL = ConfigurationSetting.CreateRepair_WSDL;
                Accelerator.EBS.SharedServices.RepairOrder.UpdateURL = ConfigurationSetting.UpdateRepair_WSDL;
                Accelerator.EBS.SharedServices.RepairOrder.ServiceUsername = ConfigurationSetting.username;
                Accelerator.EBS.SharedServices.RepairOrder.ServicePassword = ConfigurationSetting.password;
                Accelerator.EBS.SharedServices.RepairOrder.ServiceClientTimeout = ConfigurationSetting.EBSServiceTimeout;
                Accelerator.EBS.SharedServices.RepairOrder.InitEBSProvider();

                _Model.EbsOwnerId = ConfigurationSetting.ebsDefaultSrOwnerId;

                _recordContext.DataLoaded += _rContext_DataLoaded;
                _recordContext.Closing += _recordContext_Closing;
                _recordContext.Saving += _recordContext_Saving;
                _gContext.AutomationContext.CurrentEditorTabChanged += AutomationContext_CurrentEditorTabChanged;
            }
        }

        #region IAddInControl Members

        /// <summary>
        /// Method called by the Add-In framework to retrieve the control.
        /// </summary>
        /// <returns>The control, typically 'this'.</returns>
        public System.Windows.Forms.Control GetControl()
        {
            // return empty control so framework won't throw exception
            if (!ConfigurationSetting.configVerbPerfect && ConfigurationSetting.loginUserIsAdmin)
                return new Control();

            else
                return this;
        }

        public string HeaderText
        {
            get
            {
                return "EBS Bulk Import";
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

        #region Private members

        /// <summary>
        /// The current workspace record context.
        /// </summary>
        private IRecordContext _recordContext;
        private BulkImportControl bulkImportControl;
        private IGlobalContext _gContext;
        private string _usr;
        private string _pwd;
        private SharedServices.RightNowServiceReference.RightNowSyncPortClient _client;
        private Stopwatch sw = new Stopwatch();

        void _recordContext_Saving(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_Model.IsRunning)
            {
                e.Cancel = true;
                System.Windows.MessageBox.Show("Wait for the current job to complete.", "EBS Bulk Import", MessageBoxButton.OK, MessageBoxImage.Stop);
                sw.Restart();
                return;
            }
            if (!_Model.InDemo && _Model.IsIncomplete)
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("Some files are partially imported. Do you want to leave?",
                    "EBS Bulk Import", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (MessageBoxResult.No == result)
                    e.Cancel = true;
            }
            sw.Restart();
        }

        void _recordContext_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (sw.IsRunning && sw.ElapsedMilliseconds < 10000)
            {
                return;
            }
            if (_Model.IsRunning)
            {
                e.Cancel = true;
                System.Windows.MessageBox.Show("Wait for the current job to complete.", "EBS Bulk Import", MessageBoxButton.OK, MessageBoxImage.Stop);
                sw.Stop();
                return;
            }
            if (!_Model.InDemo && _Model.IsIncomplete)
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("Some files are partially imported. Do you want to leave?", 
                    "EBS Bulk Import", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (MessageBoxResult.No == result)
                    e.Cancel = true;
            }
            sw.Stop();
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
            _Model.InDemo = (0 == EbsIncidentId || 0 == _Model.EbsOwnerId);
            _Model.Render(true);
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

        #endregion

        internal RightNowService _rnSrv;
        private decimal EbsIncidentId { get; set; }
        internal IIncident _Incident { get; set; }
        private IContact _Contact { get; set; }
        internal BulkImportViewModel _Model;
        private decimal _EbsContactOrgId;

        internal void MarkFileImported(ImportFile importFile)
        {
            int id = importFile.FileId;
            IFAttachInc2 attach = null;
            if (null != _Incident && null != _Incident.FAttach2)
            {
                foreach (var f in _Incident.FAttach2)
                {
                    if (f.ID == id)
                    {
                        attach = f;
                        break;
                    }
                }
            }
            if (null == attach) return;
            // update the description of an incident's attachment
            Incident incident = new Incident();
            incident.ID = new ID();
            incident.ID.id = _Incident.ID;
            incident.ID.idSpecified = true;
            FileAttachmentIncident file = new FileAttachmentIncident();
            file.ID = new ID();
            file.ID.id = id;
            file.ID.idSpecified = true;
            file.action = ActionEnum.update;
            file.actionSpecified = true;
            file.Private = attach.Private;
            file.PrivateSpecified = true;
            bool updated;
            switch (importFile.StatusCode)
            {
                case 70:
                case 75:
                    file.Description = importFile.Description +  string.Format("{{{0:00}}}", importFile.StatusCode);
                    updated = true;
                    break;
                default:
                    updated = false;
                    break;
            }
            if (updated)
            {
                incident.FileAttachments = new FileAttachmentIncident[1] { file };
                try
                {
                    _rnSrv.updateObject(new RNObject[1] { incident });
                }
                catch (Exception ex)
                {
                    _Model._Proxy.NoticeLog(String.Format("EBS Bulk Import failed to update the description of file {0}, id {1}",
                        importFile.Name, id), ex.Message);
                }
            }
        }
    }

    [AddIn("EBS Bulk Import Addin", Version = "1.0.0.0")]
    public class WorkspaceAddInFactory : IWorkspaceComponentFactory2
    {
        private IGlobalContext _gContext;
        #region IWorkspaceComponentFactory2 Members

        /// <summary>
        /// Method which is invoked by the AddIn framework when the control is created.
        /// </summary>
        /// <param name="inDesignMode">Flag which indicates if the control is being drawn on the Workspace Designer. (Use this flag to determine if code should perform any logic on the workspace record)</param>
        /// <param name="RecordContext">The current workspace record context.</param>
        /// <returns>The control which implements the IWorkspaceComponent2 interface.</returns>
        public IWorkspaceComponent2 CreateControl(bool inDesignMode, IRecordContext RecordContext)
        {
            ConfigurationSetting instance = ConfigurationSetting.Instance(_gContext);

            if (!ConfigurationSetting.configVerbPerfect)
            {
                if (!ConfigurationSetting.loginUserIsAdmin)
                {
                    System.Windows.Forms.MessageBox.Show("BulkImportAddin is not initialized properly. \nPlease contact your system administrator.\n You are now logged out.");
                    _gContext.Logout();
                }
                else // don't want to logout admin
                {
                    System.Windows.Forms.MessageBox.Show("BulkImportAddin is not loaded because of invalid configuration verb.");
                    return new BulkImportAddin(inDesignMode, RecordContext, _gContext);
                }
            }

            return new BulkImportAddin(inDesignMode, RecordContext, _gContext);
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
            get { return "EBS Bulk Import"; }
        }

        /// <summary>
        /// The tooltip displayed when hovering over the Add-In in the Ribbon of the Workspace Designer.
        /// </summary>
        public string Tooltip
        {
            get { return "Bulk Import EBS Repair Orders"; }
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
            ConfigurationSetting.logAssemblyVersion(Assembly.GetAssembly(typeof(BulkImportViewModel)), null);

            ConfigurationSetting instance = ConfigurationSetting.Instance(_gContext);
            /* log it, but return true because will show the messagebox when the addin is opened in createControl.
             * if return false, the add-in is not loaded, and cannot show the error when add-in is opened.
             */
            if (!ConfigurationSetting.configVerbPerfect)
            {
                string logMessage = "BulkImportAddin is not initialized properly because of invalid config verb.";
                ConfigurationSetting.logWrap.ErrorLog(logMessage: logMessage);
            }
            return true;
        }

        #endregion
    }
}
