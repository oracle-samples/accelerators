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
 *  date: Thu Sep  3 23:14:03 PDT 2015

 *  revision: rnw-15-11-fixes-release-03
 *  SHA1: $Id: da7f92d4d795b73203c03788231119c2cf4e873c $
 * *********************************************************************************************
 *  File: OracleRightnowToaMonthlyViewSchedulerAddIn.cs
 * ****************************************************************************************** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.AddIn;
using RightNow.AddIns.AddInViews;
using Image = System.Drawing.Image;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace Oracle.RightNow.Toa.MonthlyViewSchedulerAddIn
{
    public class OracleRightnowToaMonthlyViewSchedulerAddIn : System.Windows.Forms.Panel, IWorkspaceComponent2
    {/// <summary>
        /// The current workspace record context.
        /// </summary>
        private IRecordContext _recordContext;
        private bool isSchedulerInitialized;
        private ElementHost elementHost;
        private bool isManagerOverride;

        public bool IsManagerOverride
        {
            get { return isManagerOverride; }
            set { isManagerOverride = value; }
        }


        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="inDesignMode">Flag which indicates if the control is being drawn on the Workspace Designer. (Use this flag to determine if code should perform any logic on the workspace record)</param>
        /// <param name="RecordContext">The current workspace record context.</param>
        /// <param name="managerOverride">Scheduler Override Allowed flag</param>
        public OracleRightnowToaMonthlyViewSchedulerAddIn(bool inDesignMode, IRecordContext RecordContext, bool managerOverride)
        {
            _recordContext = RecordContext;
            IsManagerOverride = managerOverride;

            if (inDesignMode)
            {
                var monthlyViewScheduler = new MonthlyViewScheduler(_recordContext);
                var elementHost = new ElementHost
                {
                    Dock = DockStyle.Fill,
                    Child = monthlyViewScheduler,
                };
                
                Controls.Add(elementHost);
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
            if (ActionName == "ShowScheduler")
            {
                if (isSchedulerInitialized)
                {
                    Controls.Remove(elementHost);
                }
                else
                {
                    isSchedulerInitialized = true;                    
                }
                var monthlyViewScheduler = new MonthlyViewScheduler(_recordContext);
                bool isSuccess = monthlyViewScheduler.InitializeScheduler(IsManagerOverride);
                if (isSuccess)
                {
                    elementHost = new ElementHost
                    {
                        Dock = DockStyle.Fill,
                        Child = monthlyViewScheduler,
                    };

                    Controls.Add(elementHost);
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
    }

    [AddIn("Workspace Factory AddIn", Version = "1.0.0.0")]
    public class WorkspaceAddInFactory : IWorkspaceComponentFactory2
    {
        #region IWorkspaceComponentFactory2 Members

        [ServerConfigProperty(DefaultValue = "false")]
        public bool Scheduling_Override_Allowed { get; set; }

        /// <summary>
        /// Method which is invoked by the AddIn framework when the control is created.
        /// </summary>
        /// <param name="inDesignMode">Flag which indicates if the control is being drawn on the Workspace Designer. (Use this flag to determine if code should perform any logic on the workspace record)</param>
        /// <param name="RecordContext">The current workspace record context.</param>
        /// <returns>The control which implements the IWorkspaceComponent2 interface.</returns>
        public IWorkspaceComponent2 CreateControl(bool inDesignMode, IRecordContext RecordContext)
        {
            return new OracleRightnowToaMonthlyViewSchedulerAddIn(inDesignMode, RecordContext, Scheduling_Override_Allowed);
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
            get { return "MonthlySchedulerAddIn"; }
        }

        /// <summary>
        /// The tooltip displayed when hovering over the Add-In in the Ribbon of the Workspace Designer.
        /// </summary>
        public string Tooltip
        {
            get { return "MonthlySchedulerAddIn Tooltip"; }
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
