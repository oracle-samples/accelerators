/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  Copyright (c) 2014, 2015, 2016, 2017 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + SRM Integration Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 17.2 (February 2017) 
 *  reference: 160628-000117
 *  date: Fri Feb 10 19:47:41 PST 2017
 
 *  revision: rnw-17-2-fixes-release-2
 *  SHA1: $Id: 3e7f6541e735648e2784cfd766bfa61a6bce16e2 $
 * *********************************************************************************************
 *  File: SrmCreateReplyAddIn.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.AddIn;
using System.Drawing;
using System.Windows.Forms;
using RightNow.AddIns.AddInViews;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Accelerator.SRM.SharedServices;
using System.Diagnostics;
using Accelerator.SRM.SharedServices.RightNowServiceReference;
using System.Reflection;

namespace Accelerator.SRM.CreateReplyAddIn
{
    public class SrmCreateReplyAddIn : IWorkspaceComponent2, IDisposable
    {
        #region Properties & Fields

        private IRecordContext _recordContext;
        private IGlobalContext _GlobalContext;
        public bool ReadOnly { get; set; }

        private CreateReplyControl _createReplyControl;
        private int _threadCountWhenLoaded; // existing count. more than 1 threads can be saved when saving
        private int _incidentStatusWhenLoaded;
        private int _incidentContactIdWhenLoaded;
        private string _socialChannelType;
        private string _socialChannelUserName;
        private int _socialChannelAccountId;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="inDesignMode"></param>
        /// <param name="RecordContext"></param>
        public SrmCreateReplyAddIn(bool inDesignMode, IRecordContext RecordContext, IGlobalContext GlobalContext)
        {
            _GlobalContext = GlobalContext;
            _recordContext = RecordContext;
            if (_recordContext != null)
            {
                _recordContext.DataLoaded += _recordContext_DataLoaded;
                _recordContext.Saving += _recordContext_Saving;
            }
            _createReplyControl = new CreateReplyControl();
        }

        /// <summary>
        /// Add-in framework method to get the UI for this control
        /// </summary>
        /// <returns></returns>
        public Control GetControl()
        {
            // Add an event handler for the custom control         
            return this._createReplyControl;
        }

        /// <summary>
        /// IDispoosable support
        /// </summary>
        /// <param name="disposing"></param>
        public void Dispose()
        {
            if (_recordContext != null)
            {
                _recordContext.DataLoaded -= _recordContext_DataLoaded;
                _recordContext.Saving -= _recordContext_Saving;
            }

            if (this._createReplyControl != null)
            {
                this._createReplyControl.Dispose();
                this._createReplyControl = null;
            }
        }

        /// <summary>
        /// Method triggered when a workspace rule is invoked
        /// </summary>
        /// <param name="ActionName"></param>
        public void RuleActionInvoked(string ActionName)
        {
        }

        /// <summary>
        /// Method invoked on rule condition
        /// </summary>
        /// <param name="ConditionName"></param>
        /// <returns></returns>
        public string RuleConditionInvoked(string ConditionName)
        {
            return string.Empty;
        }

        /// <summary>
        /// Event handler to manage record saving
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="evt"></param>
        void _recordContext_Saving(object sender, CancelEventArgs evt)
        {
            var success = SaveRecord();
            evt.Cancel = !success;
        }

        /// <summary>
        /// Method to save the SRM record
        /// </summary>
        /// <param name="attempts"></param>
        /// <returns></returns>
        private bool SaveRecord(int attempts = 0)
        {
            Stopwatch clickLogStopwatch = new Stopwatch();
            clickLogStopwatch.Start();
            IThread thread;
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                IIncident incident = _recordContext.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Incident) as IIncident;

                String endpoint = null;
                int convId = 0, bundleId = 0, externalRefId = 0;
                convId = ConfigurationSetting.getSrmCustomAttr(incident, "srm_conversation_id");
                bundleId = ConfigurationSetting.getSrmCustomAttr(incident, "srm_bundle_id");
                externalRefId = ConfigurationSetting.getSrmCustomAttr(incident, "srm_external_reference_id");

                //get custom field for retrieving social channel constraints
                string socialConversationReplyMode = ConfigurationSetting.getSrmStringCustomAttr(incident, "srm_conversation_reply_mode");
                
                if (_incidentStatusWhenLoaded != incident.Status.StatusID)
                {
                    endpoint = String.Format(ConfigurationSetting.convPUTEndpoint, convId, externalRefId, bundleId);

                    // determine whether to close or re-open a conversation
                    String jsonStr = "{\"status\" : \"active\"}";
                    if (incident.Status.StatusID == ConfigurationSetting.closedConversationStatusId)
                    {
                        jsonStr = "{\"status\" : \"completed\"}"; // close the SRM conversation
                    }

                    ConfigurationSetting.logWrap.DebugLog(logMessage: Accelerator.SRM.SharedServices.Properties.Resources.PUTRequestMessage, logNote: String.Format("{0} {1}", endpoint, jsonStr));
                    var results = RESTHelper.PerformPUT(endpoint, jsonStr, ref _GlobalContext);

                    if (!results.Success)
                    {
                        MessageBox.Show(results.Message);
                    }
                }

                int threadsCreated = incident.Threads.Count - _threadCountWhenLoaded;

                //Get current conversation channel constraints - including char limit and social handle 
                int charLimit = 0;
                bool includeSocialHandle = false;
                if (threadsCreated > 0)
                {
                    Constraint constraint = GetCurrentIncidentConversationConstraints(_socialChannelType, socialConversationReplyMode);
                    if (constraint != null)
                    {
                        charLimit = constraint.char_limit < 0 ? 0 : constraint.char_limit;
                        includeSocialHandle = constraint.include_social_handle == true ? true : false;
                    }
                    else
                    {
                        ConfigurationSetting.logWrap.DebugLog(logMessage: String.Format(Properties.Resources.ConstraintsNotDefinedError, _socialChannelType));
                    }
                }

                //Check char limit, if one of the threads exceed the max limit, cancel saving event
                if (charLimit > 0)
                {
                    bool isSucceed = CharLimitChecking(incident, charLimit, _socialChannelType);
                    if (!isSucceed)
                    {
                        return false;
                    }
                }
                else
                {
                    ConfigurationSetting.logWrap.DebugLog(logMessage: Properties.Resources.CharacterLimitError);
                }

                //Set social handel
                string socialHandle = "";
                if (includeSocialHandle && !String.IsNullOrEmpty(_socialChannelUserName))
                {
                    socialHandle = "@" + _socialChannelUserName + " ";
                }

                for (int i = 1; i < threadsCreated + 1; i++)
                {
                    thread = incident.Threads[incident.Threads.Count - i];

                    string contentThread = thread.Note;
                    if (thread.ContentType == RightNow.AddIns.Common.ThreadContentType.HTML)
                    {
                        string note = thread.Note;
                        string note_raw = Regex.Replace(note, "<.*?>", string.Empty);
                        if (note_raw.Length > 0)
                            note_raw = note_raw.TrimStart('\n').TrimEnd('\n');
                        contentThread = WebUtility.HtmlDecode(note_raw);
                    }
                    else
                    {
                        contentThread = contentThread.Remove(contentThread.Length - 1, 1);
                    }

                    bool isSrmEngage = false;
                    foreach (int chanId in ConfigurationSetting.SrmEngageChannelId)
                    {
                        if (thread.ChanID == chanId)
                        {
                            isSrmEngage = true;
                            break;
                        }
                    }

                    if (isSrmEngage)
                    {
                        endpoint = String.Format(ConfigurationSetting.convReplyPOSTEndpoint, convId, bundleId);

                        String jsonStr = "{\"body\" : \"";
                        jsonStr += socialHandle + contentThread + "\",\"externalType\": \"rightnow\"}";
                        ConfigurationSetting.logWrap.DebugLog(logMessage: Accelerator.SRM.SharedServices.Properties.Resources.POSTRequestMessage, logNote: String.Format("{0} {1}", endpoint, jsonStr));
                        var results = RESTHelper.PerformPOST(endpoint, jsonStr, ref _GlobalContext);

                        if (!results.Success)
                        {
                            MessageBox.Show(results.Message);
                        }
                        if (incident.Status.StatusID != ConfigurationSetting.closedConversationStatusId)
                            incident.Status.StatusID = (int)ConfigurationSetting.openConversationStatusId;
                        
                    }
                }

                updateSocialChannelAccount(incident);
                clickLogStopwatch.Stop();
                ConfigurationSetting.logWrap.ClickLog(0, 0, "SrmCreateReplyAddIn _recordContext_Saving() total time:", null, (int)clickLogStopwatch.ElapsedMilliseconds);
            }
            catch (Exception e)
            {
                MessageBox.Show(Properties.Resources.CommunicationError, Properties.Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                ConfigurationSetting.logWrap.ErrorLog(logMessage: e.Message, logNote: e.InnerException != null ? e.InnerException.Message : null);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Method called to update the social channel
        /// </summary>
        /// <param name="incident"></param>
        private void updateSocialChannelAccount(IIncident incident)
        {
            // get it again
            _socialChannelAccountId = ConfigurationSetting.getSrmStringCustomAttrInt(incident, "srm_social_channel_account_id");

            if (_socialChannelAccountId == 0)
            {
                ConfigurationSetting.logWrap.DebugLog(logMessage: String.Format(Properties.Resources.SocialChannelEmptyError, incident.ID));
                return;
            }

            /* Custom Object: SocialChannelAccount
             * 
             */
            int incidentContactId = 0;

            foreach (IInc2Contact c in incident.Contact)
            {
                if (c.Prmry == true)
                {
                    incidentContactId = (int)c.Cid;
                }
            }

            Boolean toUpdate = false;
            // if incident's contact is changed
            if (_incidentContactIdWhenLoaded != incidentContactId)
            {
                // check if updated incidentContactId same as SocialChannelAccount.ContactId
                if (_socialChannelAccountId != 0 && incidentContactId != _socialChannelAccountId)
                {
                    DialogResult result = MessageBox.Show(String.Format(Properties.Resources.ChangeContactReassignMessage, _socialChannelUserName), Properties.Resources.Info, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (result == DialogResult.OK)
                        toUpdate = true;
                }
                // update in Accelerator.SocialChannelAccount
                if (toUpdate)
                {
                    // create a row in SocialChannelAccount 
                    ClientInfoHeader clientInfoHeader = new ClientInfoHeader();
                    clientInfoHeader.AppID = "Update a SocialChannelAccount";

                    GenericObject go = new GenericObject();

                    //Set the object type
                    RNObjectType objType = new RNObjectType();
                    objType.Namespace = "Accelerator";
                    objType.TypeName = "SocialChannelAccount";
                    go.ObjectType = objType;

                    List<GenericField> gfs = new List<GenericField>();

                    ID socialAccountId = new ID();
                    socialAccountId.id = _socialChannelAccountId;
                    socialAccountId.idSpecified = true;

                    go.ID = socialAccountId;
                   
                    NamedID contactIdNamedID =
                        new NamedID
                        {
                            ID = new ID
                            {
                                id = incidentContactId,
                                idSpecified = true
                            }
                        };

                    gfs.Add(createGenericField("ContactId", ItemsChoiceType.NamedIDValue, contactIdNamedID));
                    go.GenericFields = gfs.ToArray();

                    RNObject[] objects = new RNObject[] { go };

                    UpdateProcessingOptions cpo = new UpdateProcessingOptions();
                    cpo.SuppressExternalEvents = false;
                    cpo.SuppressRules = false;

                    ConfigurationSetting.logWrap.DebugLog(logMessage: String.Format(Properties.Resources.UpdateSocialChannelMessage, _socialChannelAccountId, incidentContactId));
                    ConfigurationSetting.client.Update(clientInfoHeader, objects, cpo);

                    DialogResult result = MessageBox.Show(String.Format(Properties.Resources.ReassignAllIncidentsMessage, _socialChannelUserName), Properties.Resources.Info, MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (result == DialogResult.OK)
                        updateIncidents(incidentContactId);
                }
            }
        }

        private void updateIncidents(int incidentContactId)
        {
            // query incidents ids with the _socialChannelAccountId
            String query = "select I.ID from Incident I where I.CustomFields.Accelerator.srm_social_channel_account_id=" + _socialChannelAccountId;
            String[] rowData = null;

            rowData = ConfigurationSetting.rnSrv.queryData(query);

            if (rowData.Length == 0)
                return;

            RNObject[] updateIncidents = new RNObject[rowData.Length];
            int i = 0;
            UpdateProcessingOptions updateProcessingOptions = new UpdateProcessingOptions();
            updateProcessingOptions.SuppressExternalEvents = false;
            updateProcessingOptions.SuppressRules = false;
            foreach (String incidentIdString in rowData)
            {                
                Incident incidentToUpdate = new Incident();
                ID incidentToUpdateId = new ID();
                incidentToUpdateId.id = Convert.ToInt32(incidentIdString);
                incidentToUpdateId.idSpecified = true;

                incidentToUpdate.ID = incidentToUpdateId;

                NamedID contactIdNamedID =
                            new NamedID
                            {
                                ID = new ID
                                {
                                    id = incidentContactId,
                                    idSpecified = true
                                }
                            };

                incidentToUpdate.PrimaryContact = new IncidentContact();
                incidentToUpdate.PrimaryContact.Contact = contactIdNamedID;
                updateIncidents[i] = incidentToUpdate;
                i++;
                ConfigurationSetting.logWrap.DebugLog(logMessage: String.Format(Properties.Resources.UpdateIncidentMessage, incidentToUpdateId.id, incidentContactId));
            }

            ClientInfoHeader clientInfoHeader = new ClientInfoHeader();
            clientInfoHeader.AppID = "Update incidents";

            ConfigurationSetting.client.Update(clientInfoHeader, updateIncidents, updateProcessingOptions);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="itemsChoiceType"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        private GenericField createGenericField(string Name, ItemsChoiceType itemsChoiceType, object Value)
        {
            GenericField gf = new GenericField();
            gf.name = Name;
            gf.DataValue = new DataValue();
            gf.DataValue.ItemsElementName = new ItemsChoiceType[] { itemsChoiceType };
            gf.DataValue.Items = new object[] { Value };
            return gf;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socialChannelType"></param>
        /// <param name="socialConversationReplyMode"></param>
        /// <returns></returns>
        private Constraint GetCurrentIncidentConversationConstraints(string socialChannelType, string socialConversationReplyMode)
        {
            List<ChannelConstraints> convChannelConstraints = ConfigurationSetting.convChannelConstraints;
            List<Constraint> constraints = null;
            Constraint currentConstraint = null;

            if (convChannelConstraints == null)
            {
                return currentConstraint;
            }

            foreach (ChannelConstraints channelConstraints in convChannelConstraints)
            {
                if (channelConstraints.channel == socialChannelType)
                {
                    constraints = channelConstraints.constraints;
                    break;
                }
            }

            if (constraints == null)
            {
                return currentConstraint;
            }

            socialConversationReplyMode = socialConversationReplyMode == "private" ? "private" : "public";
            foreach (Constraint constraint in constraints)
            {
                if (constraint.reply_mode == socialConversationReplyMode)
                {
                    currentConstraint = constraint;
                    break;
                }
            }
            return currentConstraint;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="incident"></param>
        /// <param name="charLimit"></param>
        /// <param name="socialChannelType"></param>
        /// <returns></returns>
        private bool CharLimitChecking(IIncident incident, int charLimit, string socialChannelType)
        {
            bool isNotExceed = true;
            int threadsCreated = incident.Threads.Count - _threadCountWhenLoaded;
            IThread thread;
            for (int i = 1; i < threadsCreated + 1; i++)
            {
                thread = incident.Threads[incident.Threads.Count - i];

                string contentThread = thread.Note;
                if (thread.ContentType == RightNow.AddIns.Common.ThreadContentType.HTML)
                {
                    string note = thread.Note;
                    string note_raw = Regex.Replace(note, "<.*?>", string.Empty);
                    if (note_raw.Length > 0)
                        note_raw = note_raw.TrimStart('\n').TrimEnd('\n');
                    contentThread = WebUtility.HtmlDecode(note_raw);
                }
                else
                {
                    contentThread = contentThread.Remove(contentThread.Length - 1, 1);
                }

                bool isSrmEngage = false;
                foreach (int chanId in ConfigurationSetting.SrmEngageChannelId)
                {
                    if (thread.ChanID == chanId)
                    {
                        isSrmEngage = true;
                        break;
                    }
                }

                if (isSrmEngage)
                {
                    if (contentThread.Length> charLimit)
                    {
                        isNotExceed = false;
                        string message = String.Format(Properties.Resources.ContentLongerError, charLimit, socialChannelType);
                        MessageBox.Show(message, Properties.Resources.Info, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ConfigurationSetting.logWrap.DebugLog(logMessage: message + ": " + contentThread);
                        return isNotExceed;
                    }
                }
            }
            return isNotExceed;
        }

        /// <summary>
        /// Event handler for the data loaded event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _recordContext_DataLoaded(object sender, System.EventArgs e)
        {           
            try
            {                
                IIncident incident = _recordContext.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Incident) as IIncident;

                _incidentStatusWhenLoaded = incident.Status.StatusID;

                if (incident.Threads.Count != 0 && incident.Threads[incident.Threads.Count - 1].ID == 0)
                    _threadCountWhenLoaded = incident.Threads.Count - 1;
                else
                    _threadCountWhenLoaded = incident.Threads.Count;

                if (incident.Subject == null)
                    return;
                
                    foreach (IInc2Contact c in incident.Contact)
                    {
                        if (c.Prmry == true)
                        {
                            _incidentContactIdWhenLoaded = (int)c.Cid;
                        }
                    }
                
                /* get the Incident's custom attribute: srm_social_channel_id 
                 * then, lookup in the Custom Object: SocialChannelAccount to get its ContactId,on ChannelUserName and ChannelType
                 * then, compare SocialChannelAccount.ContactId with incident.Contact.Cid 
                 */
                _socialChannelAccountId = ConfigurationSetting.getSrmStringCustomAttrInt(incident, "srm_social_channel_account_id");

                if (_socialChannelAccountId != 0)
                {
                    //Query SocialChannelAccount from Cloud Service
                    String query = "select ChannelType,ChannelUserName,ContactId from Accelerator.SocialChannelAccount where Id=" + _socialChannelAccountId;
                    String[] rowData = null;
                    int contactIdSocialChannelAccount = 0;

                    rowData = ConfigurationSetting.rnSrv.queryData(query);

                    if (rowData.Length != 0)
                    {
                        // rowData is "ChannelType,ChannelUserName,ContactId", eg: "facebook,jill.smith,1" 
                        String[] socialData = rowData[0].Split(',');
                        // socialData[0]  is ChannelType, socialData[1] is ChannelUserName, socialData[2] is ContactId
                        if (!String.IsNullOrEmpty (socialData[2] ))
                            contactIdSocialChannelAccount = Convert.ToInt32(socialData[2]);
                        else
                        {
                            var message = String.Format(Properties.Resources.ContactIDEmptyError, _socialChannelAccountId);
                            MessageBox.Show(message, Properties.Resources.Info, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ConfigurationSetting.logWrap.DebugLog(logMessage: message);
                            contactIdSocialChannelAccount = 0; // just default empty ContactId to 0
                        }
                        _socialChannelType = socialData[0];
                        _socialChannelUserName = socialData[1];

                        if (_incidentContactIdWhenLoaded != contactIdSocialChannelAccount)
                        {
                            MessageBox.Show(Properties.Resources.ContactDoesNotMatchSocialError, Properties.Resources.Info, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                        MessageBox.Show(Properties.Resources.SRMNotFoundError, Properties.Resources.Info, MessageBoxButtons.OK, MessageBoxIcon.Information); ;
                }
                else
                {
                    ConfigurationSetting.logWrap.DebugLog(logMessage: String.Format(Properties.Resources.SocialChannelEmptyError, incident.ID));
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Properties.Resources.CommunicationError, Properties.Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                ConfigurationSetting.logWrap.ErrorLog(logMessage: ex.Message, logNote: ex.InnerException != null ? ex.InnerException.Message : null);
            }
        }
    }

    [AddIn("SRM Engage AddIn", Version = "1.0.0.0")]
    public class SrmEngageAddInFactory : IWorkspaceComponentFactory2
    {
        #region IWorkspaceComponentFactory2 Members
        private IRecordContext _rContext;
        private IGlobalContext _gContext;
        private SrmCreateReplyAddIn _wsAddIn;

        public IWorkspaceComponent2 CreateControl(bool inDesignMode, IRecordContext RecordContext)
        {
            _wsAddIn = new SrmCreateReplyAddIn(inDesignMode, RecordContext, _gContext);
            return _wsAddIn;
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
            get { return "SRMEngageWorkspaceAddIn"; }
        }

        /// <summary>
        /// The tooltip displayed when hovering over the Add-In in the Ribbon of the Workspace Designer.
        /// </summary>
        public string Tooltip
        {
            get { return "SRM Engage WorkspaceAddIn Tooltip"; }
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
            ConfigurationSetting.logAssemblyVersion(Assembly.GetAssembly(typeof(SrmEngageAddInFactory)), null);
            return true;
        }
        #endregion
    }
}
