/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  Copyright (c) 2014, 2015, 2016, 2017 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + SRM Integration Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 17.2 (February 2017) 
 *  reference: 160628-000117
 *  date: Fri Feb 10 19:47:44 PST 2017
 
 *  revision: rnw-17-2-fixes-release-2
 *  SHA1: $Id: df36e6c1cf617a66587eb758cb7163047cdba738 $
 * *********************************************************************************************
 *  File: ServiceStatusBarAddIn.cs
 * *********************************************************************************************/

using System;
using System.AddIn;
using System.Windows.Forms;
using RightNow.AddIns.AddInViews;

namespace Accelerator.SRM.SharedServices
{
    [AddIn("Accelerator Status bar add-in", Version = "1.0.0.0")]
    public class ServiceStatusBarAddIn : Panel, IStatusBarItem, IDisposable
    {
        #region Properties

        /// <summary>
        /// Field to store the global context
        /// </summary>
        private IGlobalContext _globalContext;

        #endregion

        #region IAddInBase Members

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

            var instance = ConfigurationSetting.Instance(_globalContext);
            this.BackColor = System.Drawing.Color.Transparent;
            this.AutoSize = true;
            this.Width = 300;

            this.Controls.Add(ConfigurationSetting.iconLabelControl);

           // initial provider delete

            if (!ConfigurationSetting.configVerbPerfect)
            {
                MessageBox.Show(Properties.Resources.AcceleratorNotInitializedNotifyAdminError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ConfigurationSetting.logWrap.ErrorLog(logMessage: Properties.Resources.AccleratorNotInitializedConfigVerbError);
            }
            
            //Check if SRM configs exist asynchronously.
            
            if (!ConfigurationSetting.SRMAuthTokenConfigsExist)
            {
                ValidateSRMConfigs();
            }

            //Setup admin event handlers
            var oauthInstance = OAuthHelper.Instance;
            oauthInstance.TokensRemovedFromServer += oauthInstance_TokensRemovedFromServer;

            return true;
        }

        public void Dispose()
        {
            var oauthInstance = OAuthHelper.Instance;
            oauthInstance.TokensRemovedFromServer -= oauthInstance_TokensRemovedFromServer;
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

        #region Custom Methods

        /// <summary>
        /// Asynchronous method to check for SRM configs while login is happening.
        /// Once the request is completed, then notications methods are called in the main UI thread.
        /// </summary>
        private async void ValidateSRMConfigs()
        {
            var conf = ConfigurationSetting.Instance(_globalContext);
            Tuple<bool, bool> checkConfigResult = await conf.ValidateSRMAuthorizationTokenConfigs(_globalContext.AccountId);
            if (checkConfigResult == null || checkConfigResult.Item1 == false || checkConfigResult.Item2 == false)
            {
                //Invoke the token configs not set
                //This happens on the main thread since there is a UI call.
                Action action;
                if (ConfigurationSetting.loginUserIsAdmin)
                {
                    action = new Action(() => SRMTokenConfigsNotSetAdmin());
                }
                else
                {
                    action = new Action(() => SRMTokenConfigsNotSet(checkConfigResult.Item1));
                }

                this.Invoke(action);
            }
        }

        /// <summary>
        /// Method called when SRM Tokens have not been configured.  The admin will be given the option to configure the setup.
        /// If rejected, then tokens will not work on the site and errors will occure each time a workspace is opened.
        /// The admin for any site should do this the first login after installing SRM add-ins on a site.
        /// </summary>
        private void SRMTokenConfigsNotSetAdmin()
        {
            var dialogReults = MessageBox.Show(Properties.Resources.ServerConfigsDoNotExistAdminLabel, Properties.Resources.Error, MessageBoxButtons.YesNo);
            if (dialogReults == DialogResult.Yes)
            {
                var setupURL = String.Format("{0}custom/srmoauthinit.php?session_id={1}", _globalContext.InterfaceURL, _globalContext.SessionId);
                setupURL = setupURL.Replace(@"http://", @"https://"); //make sure we're using HTTPS.  The interface URL will try HTTP sometimes.
                System.Diagnostics.Process.Start(setupURL);
            }
            else
            {
                ConfigurationSetting.logWrap.DebugLog(logMessage: Properties.Resources.SRMTokenConfigLabel, logNote: Properties.Resources.SRMTokenConfigRejectedError);
            }
        }

        /// <summary>
        /// Method called when SRM Tokens have not been configured.  The user will be notified to request that an administrator setup the OAuth tokens again.
        /// </summary>
        /// <param name="configsExist">Flag to indicate if an admin has setup a configuration</param>
        private void SRMTokenConfigsNotSet(bool configsExist)
        {
            if (configsExist)
            {
                var dialogReults = MessageBox.Show(Properties.Resources.UserTokenDoesNotExistLabel, Properties.Resources.Error, MessageBoxButtons.YesNo);
                if (dialogReults == DialogResult.Yes)
                {
                    var setupURL = String.Format("{0}custom/srmoauthinit.php?session_id={1}", _globalContext.InterfaceURL, _globalContext.SessionId);
                    setupURL = setupURL.Replace(@"http://", @"https://"); //make sure we're using HTTPS.  The interface URL will try HTTP sometimes.
                    System.Diagnostics.Process.Start(setupURL);
                }
            }
            else
            {
                var dialogReults = MessageBox.Show(Properties.Resources.ServerConfigsDoNotExistUserLabel, Properties.Resources.Error, MessageBoxButtons.OK);
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Event handler that will display a message that configs have been removed if a token refreh error occurs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void oauthInstance_TokensRemovedFromServer(object sender, EventArgs e)
        {
            if (ConfigurationSetting.loginUserIsAdmin)
            {
                SRMTokenConfigsNotSetAdmin();
            }
            else
            {
                SRMTokenConfigsNotSet(false);
            }
        }

        #endregion
    }
}
