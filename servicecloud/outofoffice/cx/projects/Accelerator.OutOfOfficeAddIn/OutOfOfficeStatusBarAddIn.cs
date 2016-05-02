/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC Out of Office Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 16.5 (May 2016) 
 *  reference: 150916-000080
 *  date: Thu Mar 17 23:37:54 PDT 2016
 
 *  revision: rnw-16-5-fixes-release-1
 *  SHA1: $Id: cd0c1ec975e099674e4b2a80fd84daf9b54b9988 $
 * *********************************************************************************************
 *  File: OutOfOfficeStatusBarAddIn.cs
 * *********************************************************************************************/
using System;
using System.AddIn;
using System.Windows.Forms;
using Accelerator.OutOfOffice.Client.Model;
using Accelerator.OutOfOffice.Client.RightNow;
using Accelerator.OutOfOffice.View;
using RightNow.AddIns.AddInViews;

////////////////////////////////////////////////////////////////////////////////
//
// File: OutOfOfficeStatusBarAddIn.cs
//
// Comments:
//
// Notes: 
//
// Pre-Conditions: 
//
////////////////////////////////////////////////////////////////////////////////
namespace Accelerator.OutOfOffice
{
    [AddIn("Accelerator Out of Office add-in", Version = "1.0.0.0")]
    public class OutOfOfficeStatusBarAddIn : Panel, IStatusBarItem
    {
        #region IAddInBase Members

        private IGlobalContext _context;
        /// <summary>
        /// Default constructor.
        /// </summary>
        public OutOfOfficeStatusBarAddIn()
        {   
            
        }       

        /// <summary>
        /// Method which is invoked from the Add-In framework and is used to programmatically control whether to load the Add-In.
        /// </summary>
        /// <param name="GlobalContext">The Global Context for the Add-In framework.</param>
        /// <returns>If true the Add-In to be loaded, if false the Add-In will not be loaded.</returns>
        public bool Initialize(IGlobalContext context)
        {
            _context = context;
            this.BackColor = System.Drawing.Color.Transparent;
            this.AutoSize = true;
            this.Width = 300;

            StatusBarControl statusBarControl = new StatusBarControl(context);
            this.Controls.Add(statusBarControl);

            return true;
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
    }
}
