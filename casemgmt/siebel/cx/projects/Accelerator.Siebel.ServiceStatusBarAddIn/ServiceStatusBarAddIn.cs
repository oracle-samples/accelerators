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
 *  date: Wed Sep  2 23:14:43 PDT 2015

 *  revision: rnw-15-8-fixes-release-01
 *  SHA1: $Id: ff9b895d474b1f1139c77f69ef1d3e8d4e644124 $
 * *********************************************************************************************
 *  File: ServiceStatusBarAddIn.cs
 * *********************************************************************************************/
using System;
using System.AddIn;
using System.Windows.Forms;
using RightNow.AddIns.AddInViews;

////////////////////////////////////////////////////////////////////////////////
//
// File: StatusBarAddIn.cs
//
// Comments:
//
// Notes: 
//
// Pre-Conditions: 
//
////////////////////////////////////////////////////////////////////////////////
namespace Accelerator.Siebel.SharedServices
{
    [AddIn("Accelerator Status bar add-in", Version = "1.0.0.0")]
    public class ServiceStatusBarAddIn : Panel, IStatusBarItem
    {
        #region IAddInBase Members
        private IGlobalContext _globalContext;
 
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ServiceStatusBarAddIn()
        {         
        }       

        /// <summary>
        /// Method which is invoked from the Add-In framework and is used to programmatically control whether to load the Add-In.
        /// </summary>
        /// <param name="GlobalContext">The Global Context for the Add-In framework.</param>
        /// <returns>If true the Add-In to be loaded, if false the Add-In will not be loaded.</returns>
        public bool Initialize(IGlobalContext context)
        {
            _globalContext = context;

            ConfigurationSetting instance = ConfigurationSetting.Instance(_globalContext);
            this.BackColor = System.Drawing.Color.Transparent;
            this.AutoSize = true;
            this.Width = 300;

            this.Controls.Add(ConfigurationSetting.iconLabelControl);

            if (ConfigurationSetting.SiebelProvider != null)
            {
                ServiceRequest.ServiceProvider = ConfigurationSetting.SiebelProvider;
                ServiceRequest.CreateUpdateURL = ConfigurationSetting.LookupSRbyContactPartyID_WSDL;
                ServiceRequest.LookupURL = ConfigurationSetting.LookupSR_WSDL;
                ServiceRequest.ServiceUsername = ConfigurationSetting.username;
                ServiceRequest.ServicePassword = ConfigurationSetting.password;
                ServiceRequest.ServiceClientTimeout = ConfigurationSetting.SiebelServiceTimeout;

                ServiceRequest.InitSiebelProvider();

                Accelerator.Siebel.SharedServices.Contact.ServiceProvider = ConfigurationSetting.SiebelProvider;
                Accelerator.Siebel.SharedServices.Contact.ListLookupURL = ConfigurationSetting.LookupSRbyContactPartyID_WSDL;
                Accelerator.Siebel.SharedServices.Contact.ServiceUsername = ConfigurationSetting.username;
                Accelerator.Siebel.SharedServices.Contact.ServicePassword = ConfigurationSetting.password;
                Accelerator.Siebel.SharedServices.Contact.ServiceClientTimeout = ConfigurationSetting.SiebelServiceTimeout;

                Accelerator.Siebel.SharedServices.Contact.InitSiebelProvider();

                Asset.ServiceProvider = ConfigurationSetting.SiebelProvider;
                Asset.LookupURL = ConfigurationSetting.LookupSRbyContactPartyID_WSDL;
                Asset.ServiceUsername = ConfigurationSetting.username;
                Asset.ServicePassword = ConfigurationSetting.password;
                Asset.ServiceClientTimeout = ConfigurationSetting.SiebelServiceTimeout;
                Asset.InitSiebelProvider();

                Activity.ServiceProvider = ConfigurationSetting.SiebelProvider;
                Activity.LookupURL = ConfigurationSetting.LookupSRbyContactPartyID_WSDL;
                Activity.ServiceUsername = ConfigurationSetting.username;
                Activity.ServicePassword = ConfigurationSetting.password;
                Activity.ServiceClientTimeout = ConfigurationSetting.SiebelServiceTimeout;
                Activity.InitSiebelProvider();
            }

            if (!ConfigurationSetting.configVerbPerfect)
            {
                String logMessage = "All Accelerator Add-Ins are not initialized properly. Please contact your system administrator.";
                MessageBox.Show(logMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                logMessage = "All Accelerator Add-Ins are not initialized properly because of invalid config verb.";
                ConfigurationSetting.logWrap.ErrorLog(logMessage: logMessage);
            }

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
