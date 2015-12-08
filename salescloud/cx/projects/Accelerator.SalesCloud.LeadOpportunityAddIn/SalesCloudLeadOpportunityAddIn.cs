/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + OSC Lead Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.11 (November 2015)
 *  OSC release: Release 10
 *  reference: 150505-000122
 *  date: Tue Dec  1 21:42:21 PST 2015

 *  revision: rnw-15-11-fixes-release-2
 *  SHA1: $Id: ed8386765900a9a63ba1a205da9adcdacc19d605 $
 * *********************************************************************************************
 *  File: SalesCloudLeadOpportunityAddIn.cs
 * ****************************************************************************************** */
using System.AddIn;
using RightNow.AddIns.AddInViews;
using Image = System.Drawing.Image;
using System;
using System.Collections.Generic;
using System.Windows;
using Accelerator.SalesCloud.LeadOpportunityAddIn.EventHandlers;
using Accelerator.SalesCloud.Client.RightNow;
using Accelerator.SalesCloud.Client.Common;


namespace Accelerator.SalesCloud.LeadOpportunityAddIn
{
    public class SalesCloudLeadOpportunityAddIn : System.Windows.Forms.Panel, IWorkspaceComponent2
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
        public SalesCloudLeadOpportunityAddIn(bool inDesignMode, IRecordContext RecordContext)
        {
            if (!inDesignMode)
            {
                _recordContext = RecordContext;
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
            switch (ActionName)
            {
                case "PopulateLeadDetails":
                    //_recordContext.ExecuteEditorCommand(global::RightNow.AddIns.Common.EditorCommand.Save);
                    PopulateLeadOpportunityDetailsHandler populateLead = new PopulateLeadOpportunityDetailsHandler(_recordContext);
                    populateLead.PopulateLeadDetails();
                    //validateExternalReference();
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

        void _recordContext_Saving(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //var logger = ToaLogService.GetLog();
            IHandler leadOpportunityHandler = new LeadOpportunitySaveHandler(_recordContext,e);
            leadOpportunityHandler.Handler();
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
            this.label1.Name = "LeadOpportunityAddIn";
            this.label1.Size = new System.Drawing.Size(20, 10);
            this.label1.TabIndex = 0;
            this.label1.Text = "LeadOpportunity :)";
            Controls.Add(this.label1);
            // 
            // OSCLeadOpportunityAddIn
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

            return new SalesCloudLeadOpportunityAddIn(inDesignMode, RecordContext);
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
            get { return "LeadOpportunityAddIn"; }
        }

        /// <summary>
        /// The tooltip displayed when hovering over the Add-In in the Ribbon of the Workspace Designer.
        /// </summary>
        public string Tooltip
        {
            get { return "LeadOpportunityAddIn Tooltip"; }
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