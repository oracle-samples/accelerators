/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: IoT OSvC Bi-directional Integration Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 16.11 (November 2016) 
 *  reference: 151217-000026
 *  date: Tue Dec 13 13:23:37 PST 2016
 
 *  revision: rnw-16-11-fixes-release
 *  SHA1: $Id: 6f25b2bea3de1df3c262d2a430dba5b96ba303f1 $
 * *********************************************************************************************
 *  File: IOTAreaAddIn.cs
 * *********************************************************************************************/
using System;
using System.AddIn;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Accelerator.IOTArea.Properties;
using Accelerator.IOTArea.View;
using RightNow.AddIns.AddInViews;

////////////////////////////////////////////////////////////////////////////////
//
// File: IOTAreaAddIn.cs
//
// Comments:
//
// Notes: 
//
// Pre-Conditions: 
//
////////////////////////////////////////////////////////////////////////////////
namespace Accelerator.IOTArea
{
    public class IOTAreaAddIn : Panel, IWorkspaceComponent2
    {
        #region IAddInBase Members

        /// <summary>
        /// Default constructor.
        /// </summary>
        public IOTAreaAddIn()
        {   
            
        }       

        public IOTAreaAddIn(bool inDesignMode, IRecordContext RecordContext)
        {
            IOTControl iot_control = new IOTControl(inDesignMode, RecordContext);
            var elementHost = new ElementHost
            {
                Dock = DockStyle.Fill,
                Child = iot_control,
            };

            Controls.Add(elementHost);
        }

        #endregion

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

        public string RuleConditionInvoked(string conditionName)
        {
            return string.Empty;
        }

        public void RuleActionInvoked(string actionName)
        {
            return;
        }

        public bool ReadOnly { get; set; }
    }

    [AddIn("IOTAccelerator Factory AddIn", Version = "1.0.0.0")]
    public class IOTAddInFactory : IWorkspaceComponentFactory2
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
            return new IOTAreaAddIn(inDesignMode, RecordContext);
        }

        #endregion

        #region IFactoryBase Members

        /// <summary>
        /// The 16x16 pixel icon to represent the Add-In in the Ribbon of the Workspace Designer.
        /// </summary>
        public Image Image16
        {
            get { return Resources.PersonalSettings32; }
        }

        /// <summary>
        /// The text to represent the Add-In in the Ribbon of the Workspace Designer.
        /// </summary>
        public string Text
        {
            get { return "IOTAccelerator"; }
        }

        /// <summary>
        /// The tooltip displayed when hovering over the Add-In in the Ribbon of the Workspace Designer.
        /// </summary>
        public string Tooltip
        {
            get { return "IOTAccelerator Tooltip"; }
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
