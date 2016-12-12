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
 *  date: Thu Nov 12 00:52:51 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: c6355eaa0c5407dafa0125891bb8d755534baf7a $
 * *********************************************************************************************
 *  File: ServiceStatusBarAddIn.cs
 * *********************************************************************************************/

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
namespace Accelerator.EBS.SharedServices
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

            if (ConfigurationSetting.EBSProvider != null)
            {
                ServiceRequest.ServiceProvider = ConfigurationSetting.EBSProvider;
                ServiceRequest.CreateUpdateURL = ConfigurationSetting.LookupSRbyContactPartyID_WSDL;
                ServiceRequest.LookupURL = ConfigurationSetting.LookupSR_WSDL;
                ServiceRequest.ServiceUsername = ConfigurationSetting.username;
                ServiceRequest.ServicePassword = ConfigurationSetting.password;
                ServiceRequest.ServiceClientTimeout = ConfigurationSetting.EBSServiceTimeout;

                ServiceRequest.InitEBSProvider();

                Accelerator.EBS.SharedServices.ContactModel.ServiceProvider = ConfigurationSetting.EBSProvider;
                Accelerator.EBS.SharedServices.ContactModel.ListLookupURL = ConfigurationSetting.LookupContactList_WSDL;
                Accelerator.EBS.SharedServices.ContactModel.ServiceUsername = ConfigurationSetting.username;
                Accelerator.EBS.SharedServices.ContactModel.ServicePassword = ConfigurationSetting.password;
                Accelerator.EBS.SharedServices.ContactModel.ServiceClientTimeout = ConfigurationSetting.EBSServiceTimeout;

                Accelerator.EBS.SharedServices.ContactModel.InitEBSProvider();

                Accelerator.EBS.SharedServices.Item.ServiceProvider = ConfigurationSetting.EBSProvider;
                Accelerator.EBS.SharedServices.Item.ListURL = ConfigurationSetting.ItemList_WSDL;
                Accelerator.EBS.SharedServices.Item.ServiceUsername = ConfigurationSetting.username;
                Accelerator.EBS.SharedServices.Item.ServicePassword = ConfigurationSetting.password;
                Accelerator.EBS.SharedServices.Item.ServiceClientTimeout = ConfigurationSetting.EBSServiceTimeout;
                Accelerator.EBS.SharedServices.Item.InitEBSProvider();

                Accelerator.EBS.SharedServices.Entitlement.ServiceProvider = ConfigurationSetting.EBSProvider;
                Accelerator.EBS.SharedServices.Entitlement.ListURL = ConfigurationSetting.EntitlementList_WSDL;
                Accelerator.EBS.SharedServices.Entitlement.ServiceUsername = ConfigurationSetting.username;
                Accelerator.EBS.SharedServices.Entitlement.ServicePassword = ConfigurationSetting.password;
                Accelerator.EBS.SharedServices.Entitlement.ServiceClientTimeout = ConfigurationSetting.EBSServiceTimeout;
                Accelerator.EBS.SharedServices.Entitlement.InitEBSProvider();

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

                Accelerator.EBS.SharedServices.RepairLogistics.ServiceProvider = ConfigurationSetting.EBSProvider;
                Accelerator.EBS.SharedServices.RepairLogistics.ListURL = ConfigurationSetting.RepairLogisticsList_WSDL;
                Accelerator.EBS.SharedServices.RepairLogistics.ServiceUsername = ConfigurationSetting.username;
                Accelerator.EBS.SharedServices.RepairLogistics.ServicePassword = ConfigurationSetting.password;
                Accelerator.EBS.SharedServices.RepairLogistics.ServiceClientTimeout = ConfigurationSetting.EBSServiceTimeout;
                Accelerator.EBS.SharedServices.RepairLogistics.InitEBSProvider();

                Accelerator.EBS.SharedServices.Order.ServiceProvider = ConfigurationSetting.EBSProvider;
                Accelerator.EBS.SharedServices.Order.GetOrderURL = ConfigurationSetting.GetOrder_WSDL;
                Accelerator.EBS.SharedServices.Order.OrderInboundURL = ConfigurationSetting.OrderInboundURL_WSDL;
                Accelerator.EBS.SharedServices.Order.InitEBSProvider();
            }

            if (!ConfigurationSetting.configVerbPerfect)
            {
                string logMessage = "All Accelerator Add-Ins are not initialized properly. Please contact your system administrator.";
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
