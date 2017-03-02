/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  Copyright (c) 2014, 2015, 2016, 2017 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + SRM Integration Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 17.2 (February 2017) 
 *  reference: 160628-000117
 *  date: Fri Feb 10 19:47:43 PST 2017
 
 *  revision: rnw-17-2-fixes-release-2
 *  SHA1: $Id: 1799f72d762ea09ad3d0f0c1a72a71b6e37d53e9 $
 * *********************************************************************************************
 *  File: OAuthHelper.cs
 * *********************************************************************************************/

using RightNow.AddIns.AddInViews;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net.Http;

namespace Accelerator.SRM.SharedServices
{
    /// <summary>
    /// Class used to communicate with a custom script to help broker communcation with SRM
    /// </summary>
    public sealed class OAuthHelper
    {
        #region Properties

        /// <summary>
        /// The default path to reach the SRM broker custom script on a given interface.
        /// </summary>
        private const String _customScriptURI = "custom/src/srmoauth.php";

        /// <summary>
        /// Field to store the Oauth token for caching
        /// </summary>
        private static string _OauthToken = "";

        /// <summary>
        /// Field to store the last request time of the oauth token
        /// </summary>
        private static DateTime _LastRequestTime = DateTime.Today;

        /// <summary>
        /// Field to store the time in which this oauth token expires
        /// </summary>
        private static DateTime _TokenValidUntil = DateTime.Today;

        /// <summary>
        /// The number of seconds that the token is valid
        /// </summary>
        private static long _TokenValidLength = 0;
        
        /// <summary>
        /// The integer timestamp used to validate oauth requests to our custom script.
        /// The add-in must update this timestamp on a regular interval to create a time-based security mechanism
        /// when communicating to the custom script for an OAuth token.
        /// </summary>
        public static long OAuthRequestTimeout = -1;

        /// <summary>
        /// Event handler called when a token is retrieved from the server.
        /// </summary>
        public event EventHandler TokenRetrievedFromServer;

        /// <summary>
        /// Event handler that can be subscribed to when tokens are removed.
        /// </summary>
        public event EventHandler TokensRemovedFromServer;

        #endregion

        #region Singleton Config

        /// <summary>
        /// Singleton istance
        /// </summary>
        private static OAuthHelper instance = null;

        /// <summary>
        /// Padlock
        /// </summary>
        private static readonly object padlock = new object();

        /// <summary>
        /// Property to return the singleton instance of this class.
        /// </summary>
        public static OAuthHelper Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new OAuthHelper();
                    }
                    return instance;
                }
            }
        }

        #endregion

        #region Configuration Methods

        /// <summary>
        /// Invalidate the cachced token
        /// </summary>
        public static void InvalidateCurrentCache()
        {
            _TokenValidUntil = DateTime.Today;
            _LastRequestTime = DateTime.Today; 
            _OauthToken = "";
        }

        #endregion

        #region Communication Methods

        /// <summary>
        /// Submits a request to the custom script to request an OAuth token for SRM.
        /// </summary>
        /// <param name="_gContext">The Global Context</param>
        /// <returns></returns>
        public string GetOAuthToken(IGlobalContext _gContext)
        {
            return GetOAuthToken(_gContext, false);
        }

        /// <summary>
        /// Submits a request to the custom script to request an OAuth token for SRM.
        /// </summary>
        /// <param name="_gContext">The Global Context</param>
        /// <param name="forceFromServer">If set to true, then we will force an OAuth token refresh from the server.</param>
        /// <returns>The OAuth token, if successful.</returns>
        public string GetOAuthToken(IGlobalContext _gContext, bool forceFromServer)
        {
            var requestTime = System.DateTime.UtcNow;
            if (!ValidateRequestTime(requestTime))
                SetValidRequestTime(_gContext);

            //Check to see if the cached token is within a valid time range.
            //Force an update if the token expires in the next 5 minutes or sooner
            var tokenExpireCheck = _TokenValidUntil.Subtract(TimeSpan.FromMinutes(5));
            if (!forceFromServer && requestTime < tokenExpireCheck && _OauthToken != null && _OauthToken != "")
            {
                ConfigurationSetting.logWrap.DebugLog(logMessage: Properties.Resources.OAuthLocalCacheMessage, logNote: _OauthToken);
                return _OauthToken;
            }

            _OauthToken = GetTokenFromServer(_gContext, requestTime);

            return _OauthToken;
        }

        /// <summary>
        /// Retrieves the stored token on the OSvC server (NOT THE SRM SERVER) and caches the results for reuse.
        /// </summary>
        /// <param name="_gContext">Add-in global context</param>
        /// <param name="forceRefresh">Will indicate that we want to perform a force refresh.</param>
        private string GetTokenFromServer(IGlobalContext _gContext, DateTime requestTime)
        {
            var oauthToken = "";
            var sessionId = _gContext.SessionId;
            var interfaceURL = _gContext.InterfaceURL;
            var postData = GetAgentPostData(_gContext, requestTime, "get_token");
            var endpoint = string.Format("{0}{1}?session_id={2}", interfaceURL, _customScriptURI, sessionId);
            endpoint = endpoint.Replace(@"http://", @"https://"); //Our endpoint expects HTTPS, so force it here if the GlobalContext returns HTTP
            var stopwatch = new Stopwatch();

            // Create a request for the URL. 		
            var client = new HttpClient();
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(endpoint),
                Method = HttpMethod.Post,
                Content = new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded")
            };
            ConfigurationSetting.logWrap.DebugLog(logMessage: Properties.Resources.GETRequestAgentSessionMessage, logNote: endpoint);
            stopwatch.Start();
            // Get the response.
            var response = client.SendAsync(request).Result;
            stopwatch.Stop();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseString = response.Content.ReadAsStringAsync().Result;
                ConfigurationSetting.logWrap.DebugLog(logMessage: Properties.Resources.GETOAuthTokenRequestMessage, logNote: responseString, timeElapsed: (int)stopwatch.ElapsedMilliseconds);

                if (responseString.Length == 0)
                {
                    LogSRMOAuthTokenError(responseString);
                }
                else if (responseString.Contains("<title>Access Denied</title>"))
                {
                    LogSessionAuthError(Properties.Resources.AccessDeniedError);
                }
                else
                {
                    oauthToken = ParseResponse(responseString);

                    if (TokenRetrievedFromServer != null)
                    {
                        TokenRetrievedFromServer(this, EventArgs.Empty);
                    }
                }
            }
            else
            {
                LogSessionAuthError(response.ToString());
            }

            return oauthToken;
        }

        /// <summary>
        /// Method used to remove the tokens object from the database.  This is called when a refresh fails; the refresh token is invalid and therefor the data is invalid.
        /// The admin must reconfigure the SRM Oauth process if/when this method is called.
        /// </summary>
        /// <param name="globalContext"></param>
        /// <returns></returns>
        public bool RemoveOauthTokens(IGlobalContext globalContext)
        {
            ConfigurationSetting.logWrap.NoticeLog(logMessage: Properties.Resources.RemovingTokenNoticeLabel, logNote: Properties.Resources.RemovingTokenNoticeLabel);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var tokenQuery = String.Format("SELECT ID FROM Accelerator.OAuthTokens WHERE Account = {0} LIMIT 1", globalContext.AccountId);
            var results = ConfigurationSetting.rnSrv.queryData(tokenQuery);
            if (results.Length > 0)
            {
                var id = results[0];
                var token = GenericObjectFactory.CreateGenericObject("OAuthTokens", "Accelerator", Convert.ToInt32(id));
                var objArr = new RightNowServiceReference.RNObject[] { token };
                ConfigurationSetting.rnSrv.destroyObjects(objArr);

                if (TokensRemovedFromServer != null)
                {
                    TokensRemovedFromServer(this, EventArgs.Empty);
                }
            }
            stopwatch.Stop();
            ConfigurationSetting.logWrap.DebugLog(logMessage: Properties.Resources.RemovedTokenLabel, logNote: Properties.Resources.RemovedTokenLabel, timeElapsed: (int)stopwatch.ElapsedMilliseconds);

            return false;
        }

        /// <summary>
        /// Sets the RefreshTokenFailure flag to true to indicate that the access token failed and the controller should try to
        /// request a new token on the next token request.
        /// </summary>
        /// <param name="globalContext"></param>
        /// <returns></returns>
        public bool SetCurrentAccessTokenInvalid(IGlobalContext globalContext)
        {
            ConfigurationSetting.logWrap.NoticeLog(logMessage: Properties.Resources.RemovingTokenNoticeLabel, logNote: Properties.Resources.RemovingTokenNoticeLabel);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var tokenQuery = String.Format("SELECT ID FROM Accelerator.OAuthTokens WHERE Account = {0} LIMIT 1", globalContext.AccountId);
            var results = ConfigurationSetting.rnSrv.queryData(tokenQuery);
            if (results.Length > 0)
            {
                var id = results[0];
                var token = GenericObjectFactory.CreateGenericObject("OAuthTokens", "Accelerator", Convert.ToInt32(id));
                token.GenericFields = new RightNowServiceReference.GenericField[1];
                token.GenericFields[0] = GenericObjectFactory.CreateGenericBoolField("RefreshTokenFailure", true);

                var objArr = new RightNowServiceReference.RNObject[] { token };
                ConfigurationSetting.rnSrv.updateObject(objArr);
            }
            stopwatch.Stop();
            ConfigurationSetting.logWrap.DebugLog(logMessage: Properties.Resources.RemovedTokenLabel, logNote: Properties.Resources.RemovedTokenLabel, timeElapsed: (int)stopwatch.ElapsedMilliseconds);

            return false;
        }

        /// <summary>
        /// Parses the response from the custom script.
        /// </summary>
        /// <param name="responseString"></param>
        /// <returns></returns>
        private string ParseResponse(string responseString){
            string oauthToken = "";

            // Get configuration value
            JavaScriptSerializer ser = new JavaScriptSerializer();
            Dictionary<string, string> oauthTokenPairs = new Dictionary<string, string>();

            try
            {
                oauthTokenPairs = ser.Deserialize<Dictionary<string, string>>(responseString);
                if (oauthTokenPairs.ContainsKey("access_token"))
                {
                    oauthToken = oauthTokenPairs["access_token"];
                    _OauthToken = oauthToken;
                }
                else
                {
                    LogSRMOAuthTokenError(responseString);
                }

                if (oauthTokenPairs.ContainsKey("expires_in"))
                {
                    int expires;
                    int.TryParse(oauthTokenPairs["expires_in"], out expires);
                    _TokenValidLength = expires;
                }

                if (oauthTokenPairs.ContainsKey("last_request_time"))
                {
                    var requestTime = Convert.ToDateTime(oauthTokenPairs["last_request_time"]);
                    _LastRequestTime = System.TimeZoneInfo.ConvertTimeToUtc(requestTime);
                    _TokenValidUntil = _LastRequestTime.AddSeconds(_TokenValidLength);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                LogSRMOAuthTokenError(responseString);
            }
            return oauthToken;
        }

        /// <summary>
        /// Creates the base64 encoded string from a string passed in.
        /// </summary>
        /// <param name="str">String to encode</param>
        /// <returns>Base64 encoded string.</returns>
        private string GetEncodedData(string str)
        {
            var jsonBytes = System.Text.Encoding.UTF8.GetBytes(str);
            return System.Convert.ToBase64String(jsonBytes);
        }

        /// <summary>
        /// Get's a form formatted string of information that is sent to the srmoauth controller for validation.
        /// </summary>
        /// <param name="globalContext"></param>
        /// <param name="requestTime"></param>
        /// <param name="method">The function that should be called from the custom script</param>
        /// <returns></returns>
        private string GetAgentPostData(IGlobalContext globalContext, System.DateTime requestTime, string method = "get_token")
        {
            var session = globalContext.SessionId;
            var accountId = globalContext.AccountId;
            var process = System.Diagnostics.Process.GetCurrentProcess();
            var processId = process.Id;
            var time = ConvertToTimestamp(requestTime);
            var postData = String.Format("f={0}&session={1}&process_id={2}&request_time={3}", method, session.Trim(), processId, time);

            return postData;
        }

        /// <summary>
        /// Validates the request timestamp against our oauth timestamp window.
        /// If the timestamp is within 1 minute of the expiration timeout, then we update the agent record
        /// with a new timeout and then change our config setting to match the value that is stored on the
        /// agent record.
        /// 
        /// The value on the agent record is used by the OAuth script to validate time of request.
        /// </summary>
        /// <param name="requestTime"></param>
        /// <returns>True if the timestamp of the request is less than 1 minute from the expiration time; else false.</returns>
        private bool ValidateRequestTime(System.DateTime requestTime)
        {
            var requestTimestamp = ConvertToTimestamp(requestTime);
            var timeout = OAuthRequestTimeout;

            return requestTimestamp <= timeout - 60;
        }

        /// <summary>
        /// Update the agent account oauth custom field timestamp with a new value 15 mins in the future.
        /// Only add-ins that leverage this library will update the timestamp, so scripts that validate against the timestamp will only succeed
        /// if this add-in is active and updating this field every 15 mins.
        /// </summary>
        /// <param name="globalContext"></param>
        private void SetValidRequestTime(IGlobalContext globalContext)
        {
            var now = System.DateTime.UtcNow;
            var validTime = now.AddSeconds(900);

            //Update the agent field through the SOAP API
            var rntService = new RightNowService(globalContext);
            var agent = new RightNowServiceReference.Account()
            {
                ID = new RightNowServiceReference.ID()
                {
                    id = globalContext.AccountId,
                    idSpecified = true
                }
            };

            //Get the current process so that we can use process ID as one of the validation mechanisms that we use when posting a request
            //To the Oauth controller.
            var process = System.Diagnostics.Process.GetCurrentProcess();

            //Setup the custom fields to be saved to the agent record via the SOAP API
            var processIdField = GenericObjectFactory.CreateGenericIntegerField(@"OAuthProcessId", process.Id);
            var timestampField = GenericObjectFactory.CreateGenericDateTimeField(@"OAuthRequestValidUntil", validTime);
            var accountCustomFieldsAccelerator = GenericObjectFactory.CreateGenericObject(@"AccountCustomFieldsAccelerator", null);
            accountCustomFieldsAccelerator.GenericFields = new RightNowServiceReference.GenericField[] { timestampField, processIdField };
            var cField = GenericObjectFactory.CreateGenericObjectField("Accelerator", accountCustomFieldsAccelerator);
            agent.CustomFields = GenericObjectFactory.CreateGenericObject("AgentCustomFields", null);
            agent.CustomFields.GenericFields = new RightNowServiceReference.GenericField[] { cField };

            rntService.updateObject(new RightNowServiceReference.RNObject[] { agent });

            //Update our stored config object with the new timestamp
            OAuthRequestTimeout = ConvertToTimestamp(validTime);
        }

        #endregion

        #region Time Methods

        /// <summary>
        /// Converts a DateTime to a unix timestamp
        /// </summary>
        /// <param name="value">The datatime to convert</param>
        /// <returns>The unix timestamp</returns>
        private long ConvertToTimestamp(DateTime value)
        {
            long ticks = value.Ticks - DateTime.Parse("01/01/1970 00:00:00").Ticks;
            ticks /= 10000000; //Convert windows ticks to seconds
            return ticks;
        }

        #endregion

        #region Logging Methods

        /// <summary>
        /// Log a session error returned from the custom script.
        /// </summary>
        /// <param name="responseString"></param>
        private void LogSessionAuthError(String responseString){
            ConfigurationSetting.logWrap.ErrorLog(logMessage: "Error in Authenticate Agent Session: ", logNote: responseString);
        }

        /// <summary>
        /// Log an oauth error returned from the custom script
        /// </summary>
        /// <param name="responseString"></param>
        private void LogSRMOAuthTokenError(String responseString){
            ConfigurationSetting.logWrap.ErrorLog(logMessage: Properties.Resources.SRMTokenErrorMessage, logNote: responseString);
        }

        #endregion
    }
}
