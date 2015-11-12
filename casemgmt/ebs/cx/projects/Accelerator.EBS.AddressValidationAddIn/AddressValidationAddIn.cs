/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + EBS Enhancement
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  EBS release: 12.1.3
 *  reference: 150505-000099, 150420-000127
 *  date: Thu Nov 12 00:52:41 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: 7c612f01cde61d984ab8b924c49e928045823056 $
 * *********************************************************************************************
 *  File: AddressValidationAddIn.cs
 * *********************************************************************************************/

using System.AddIn;
using System.Drawing;
using System.Windows.Forms;
using RightNow.AddIns.AddInViews;
using System.Text;
using System;
using System.Net;
using Accelerator.EBS.SharedServices;
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
namespace Accelerator.EBS.AddressValidationAddIn
{
    public class AddressValidationAddIn : Panel, IWorkspaceComponent2
    {
        /// <summary>
        /// The current workspace record context.
        /// </summary>
        private IRecordContext _recordContext;
        private Address _responseAddr;
        private string _uspsURL;
        private string _uspsUsername;
        private bool _isErrorFromResponse;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="inDesignMode">Flag which indicates if the control is being drawn on the Workspace Designer. (Use this flag to determine if code should perform any logic on the workspace record)</param>
        /// <param name="RecordContext">The current workspace record context.</param>
        public AddressValidationAddIn(bool inDesignMode, IRecordContext RecordContext)
        {
            _recordContext = RecordContext;
        }

        #region IAddInControl Members

        /// <summary>
        /// Method called by the Add-In framework to retrieve the control.
        /// </summary>
        /// <returns>The control, typically 'this'.</returns>
        public Control GetControl()
        {
            _uspsURL = ConfigurationSetting.ext_address_validate_url;
            _uspsUsername = ConfigurationSetting.uspsUsername;
            AddressValidationControl addressValidationControl = new AddressValidationControl();
            addressValidationControl.VerifyAddressClicked += new AddressValidationControl.VerifyAddressHandler(AddressValidationControl_VerifyAddressClicked);
            addressValidationControl.UsedSuggestedChanged += new AddressValidationControl.UsedSuggestedHandler(AddressValidationControl_UsedSuggestedChanged);
            return addressValidationControl;
        }

        void AddressValidationControl_VerifyAddressClicked(AddressValidationControl ucontrol)
        {
            ucontrol.useSuggested.CheckState = CheckState.Unchecked;
            _isErrorFromResponse = true;
            IContact contactRecord = _recordContext.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Contact) as IContact;
            Address contactAddress = new Address();
            try
            {
                if (contactRecord.AddrCountryID != 1)
                {
                    string errorMsg = "Please--USA only postal address verification!";
                    ucontrol.addrValidResponseText.Text = errorMsg;
                    ConfigurationSetting.logWrap.NoticeLog(0, contactRecord.ID, logMessage: errorMsg);
                    return;
                }

                // get the contactRecord.Addrxxx
                contactAddress.Address2 = contactRecord.AddrStreet;
                contactAddress.City = contactRecord.AddrCity;
                if (contactRecord.AddrProvID != null)
                    contactAddress.StateID = (int)contactRecord.AddrProvID;
                contactAddress.Zip = contactRecord.AddrPostalCode;

                _responseAddr = null;

                _responseAddr = ValidateAddress(contactAddress);
                
                string addressStg = _responseAddr.Address2 + ", " + _responseAddr.City 
                    + ", " + _responseAddr.State + " " + _responseAddr.Zip5 + "-" + _responseAddr.Zip4;

                 ucontrol.addrValidResponseText.Text = "Address verified: " + addressStg;
                 _isErrorFromResponse = false;
                 ConfigurationSetting.logWrap.DebugLog(0, contactRecord.ID, logMessage: ucontrol.addrValidResponseText.Text);
                
                if (_responseAddr.ReturnText != null)
                {
                    string sanitizeMsg = _responseAddr.ReturnText.Replace("Default address:", "Address verified:");
                    ucontrol.addrValidResponseText.Text = sanitizeMsg + " " + addressStg; 
                }
            }
            catch (Exception e)
            {
                // log e.Message  
                if (e.Message.Contains("Authorization failure") ||
                    e.Message.Contains("Username exceeds maximum length") ||
                    e.Message.Contains("Misconfiguration or some other invalid data received."))
                    ConfigurationSetting.logWrap.ErrorLog(0, contactRecord.ID, logMessage: e.Message);
                else
                    ConfigurationSetting.logWrap.NoticeLog(0, contactRecord.ID, logMessage: e.Message);
                ucontrol.addrValidResponseText.Text = "Not valid: " + e.Message;
                _isErrorFromResponse = true;
            }
        }

        void AddressValidationControl_UsedSuggestedChanged(AddressValidationControl ucontrol)
        {
            if (ucontrol.addrValidResponseText.Text == null || _isErrorFromResponse)
                return;

            if (ucontrol.useSuggested.Checked && !_isErrorFromResponse)
            {
                IContact contactRecord = _recordContext.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Contact) as IContact;
                contactRecord.AddrStreet = _responseAddr.Address2 + " " + _responseAddr.Address1;
                contactRecord.AddrCity = _responseAddr.City;
                contactRecord.AddrProvID = _responseAddr.StateID;
                contactRecord.AddrPostalCode = _responseAddr.Zip;
            }
        }

        public Address ValidateAddress(Address address)
        {
            try
            {
                WebClient web = new WebClient();

                string validateUrl = "?API=Verify&XML=<AddressValidateRequest USERID=\"{0}\"><Address ID=\"{1}\"><Address1>{2}</Address1><Address2>{3}</Address2><City>{4}</City><State>{5}</State><Zip5>{6}</Zip5><Zip4>{7}</Zip4></Address></AddressValidateRequest>";            
                string url = _uspsURL + validateUrl;
                url = String.Format(url, _uspsUsername, address.ID.ToString(), address.Address1, address.Address2, 
                    address.City, address.State, address.Zip5, address.Zip4);

                string addressxml; 
                try
                {
                    addressxml = web.DownloadString(url);
                }
                catch (Exception ex)
                {
                    string msg = "Misconfiguration or some other invalid data received.";
                    throw new Exception(msg);
                }

                if (addressxml.Contains("<Error>"))
                {
                    int idx1 = addressxml.IndexOf("<Description>") + 13;
                    int idx2 = addressxml.IndexOf("</Description>");
                    int l = addressxml.Length;
                    string errDesc = addressxml.Substring(idx1, idx2 - idx1);
                    throw new Exception(errDesc);
                }

                return Address.getAddress(addressxml);
            }
            catch (Exception ex)
            {
                throw ex;
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

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);

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
            return new AddressValidationAddIn(inDesignMode, RecordContext);
        }

        #endregion

        #region IFactoryBase Members

        /// <summary>
        /// The 16x16 pixel icon to represent the Add-In in the Ribbon of the Workspace Designer.
        /// </summary>
        public Image Image16
        {
            get { return Accelerator.EBS.AddressValidationAddIn.Properties.Resources.AddIn16; }
        }

        /// <summary>
        /// The text to represent the Add-In in the Ribbon of the Workspace Designer.
        /// </summary>
        public string Text
        {
            get { return "EBS AddressValidationAddIn"; }
        }

        /// <summary>
        /// The tooltip displayed when hovering over the Add-In in the Ribbon of the Workspace Designer.
        /// </summary>
        public string Tooltip
        {
            get { return "EBS AddressValidationAddIn Tooltip"; }
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