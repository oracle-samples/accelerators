/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + OFSC Reference Integration
 *  link: http://www-content.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.2 (Feb 2015)
 *  OFSC release: 15.2 (Feb 2015)
 *  reference: 150622-000130
 *  date: Thu Sep  3 23:14:04 PDT 2015

 *  revision: rnw-15-11-fixes-release-03
 *  SHA1: $Id: a07467275738af64c553447c977a1329611f32f9 $
 * *********************************************************************************************
 *  File: OracleRightNowToaWorkOrderAddIn.cs
 * ****************************************************************************************** */

using System.AddIn;
using RightNow.AddIns.AddInViews;
using Image = System.Drawing.Image;
using Oracle.RightNow.Toa.WorkOrderAddIn.EventHandlers;
using Oracle.RightNow.Toa.Client.Rightnow;
using System;
using System.Collections.Generic;
using System.Windows;
using Oracle.RightNow.Toa.Client.RightNowProxyService;
using Oracle.RightNow.Toa.Client.Logs;
using Oracle.RightNow.Toa.Client.Common;

////////////////////////////////////////////////////////////////////////////////
//
// File: OracleRightNowToaWorkOrderAddIn.cs
//
// Comments:
//
// Notes: 
//
// Pre-Conditions: 
//
////////////////////////////////////////////////////////////////////////////////
namespace Oracle.RightNow.Toa.WorkOrderAddIn
{

    public class OracleRightNowToaWorkOrderAddIn : System.Windows.Forms.Panel, IWorkspaceComponent2
    {
        private System.Windows.Forms.Label label1;
        /// <summary>
        /// The current workspace record context.
        /// </summary>
        private IRecordContext _recordContext;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="inDesignMode">Flag which indicates if the control is being drawn on the Workspace Designer. (Use this flag to determine if code should perform any logic on the workspace record)</param>
        /// <param name="RecordContext">The current workspace record context.</param>
        public OracleRightNowToaWorkOrderAddIn(bool inDesignMode, IRecordContext RecordContext)
        {
            if (!inDesignMode)
            {
                _recordContext = RecordContext;
                //_recordContext.Saved += WorkOrder_Saved;
                _recordContext.Saving += _recordContext_Saving;

            }
            else
            {
                InitializeComponent();
            }
        }



        
        #region IAddInControl Members

        /// <summary>
        /// Method called by the Add-In framework to retrieve the control.
        /// </summary>
        /// <returns>The control, typically 'this'.</returns>
        public System.Windows.Forms.Control GetControl()
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
            switch (ActionName) {
                case "PopulateContactDetails" :
                     _recordContext.ExecuteEditorCommand(global::RightNow.AddIns.Common.EditorCommand.Save);
                    PopulateContactDetailsHandler populatecontact = new PopulateContactDetailsHandler(_recordContext);
                    populatecontact.PopulateContactDetails();
                    break;
                case "WOTypeDuration" :
                    PopulateManualDuration populateDuration = new PopulateManualDuration(_recordContext);
                    populateDuration.Handler();
                    break;
                default:
                    break;
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

        #region WorkOrder Saving Handler
        void WorkOrder_Saved(object sender, EventArgs e)
        {

        }

        void _recordContext_Saving(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var log = ToaLogService.GetLog();
            IHandler workOrderHandler = new WorkOrderSaveHandler(_recordContext, e);
            workOrderHandler.Handler();          
        }
        #endregion

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "WorkOrderAddIn";
            this.label1.Size = new System.Drawing.Size(20, 10);
            this.label1.TabIndex = 0;
            this.label1.Text = "WorkOrderAddIn :)";
            Controls.Add(this.label1);
            // 
            // OracleRightNowToaWorkOrderAddIn
            // 
            this.Size = new System.Drawing.Size(20, 10);
            this.ResumeLayout(false);

        }

        //Unsubsribing events.
        protected override void Dispose(bool disposing)
        {
            if (disposing && (null != _recordContext))
            {
                // unsubscribe from all the events
                _recordContext.Saving -= _recordContext_Saving;
            }
            base.Dispose(disposing);
        }

    }

    [AddIn("Workspace Factory AddIn", Version = "1.0.0.0")]
    public class WorkspaceAddInFactory : IWorkspaceComponentFactory2
    {
        #region IWorkspaceComponentFactory2 Members

        /// <summary>
        /// Method which is invoked by the AddIn framework when the control is created.
        /// </summary>
        /// <param name="inDesignMode">Flag which indicates if the control is being drawn on the Workspace Designer. (Use this flag to determine if code should perform any logic on the workspace record)</param>
        /// <param name="RecordContext">The current workspace record context.</param>
        /// <returns>The control which implements the IWorkspaceComponent2 interface.</returns>
        public IWorkspaceComponent2 CreateControl(bool inDesignMode, IRecordContext RecordContext)
        {
            
            return new OracleRightNowToaWorkOrderAddIn(inDesignMode, RecordContext);
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
            get { return "WorkOrderAddIn"; }
        }

        /// <summary>
        /// The tooltip displayed when hovering over the Add-In in the Ribbon of the Workspace Designer.
        /// </summary>
        public string Tooltip
        {
            get { return "WorkOrderAddIn Tooltip"; }
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
            return true;
        }
        #endregion
    }
}