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
 *  SHA1: $Id: ae83f599c2de183695bbc8ccea581849f1b05f23 $
 * *********************************************************************************************
 *  File: RESTHelper.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RightNow.AddIns.AddInViews;
using System.Net.Http;

namespace Accelerator.SRM.SharedServices
{
    /// <summary>
    /// A helper class that contains reused code when making API calls to the SRM API.
    /// </summary>
    public abstract class RESTHelper
    {
        #region Properties

        #endregion

        #region API Communication Methods

        /// <summary>
        /// Method to perform a GET opertation to an SRM API endpoint.
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="globalContext"></param>
        /// <returns></returns>
        public static RESTResults PerformGET(String endpoint, ref IGlobalContext globalContext)
        {
            string jsonData = null;
            string message = null;
            bool result = false;
            Stopwatch stopwatch = new Stopwatch();

            try
            {
                var request = CreateHTTPRequest(HttpMethod.Get, endpoint, ref globalContext);
                if (request != null)
                {                    
                    stopwatch.Start();
                    result = RunRequest(request, ref jsonData, ref globalContext);
                    stopwatch.Stop();

                    if (result)
                    {
                        ConfigurationSetting.logWrap.DebugLog(logMessage: Properties.Resources.GETResponseMessage, logNote: String.Format("{0} {1}", endpoint, jsonData), timeElapsed: (int)stopwatch.ElapsedMilliseconds);
                    }
                    else
                    {
                        ConfigurationSetting.logWrap.ErrorLog(logMessage: Properties.Resources.GETErrorMessage, logNote: String.Format("{0} {1}", endpoint, jsonData), timeElapsed: (int)stopwatch.ElapsedMilliseconds);
                    }
                }
                else
                {
                    message = Properties.Resources.OAuthTokenError;
                }
            }
            catch (Exception e)
            {
                message = Properties.Resources.CommunicationErrorMessage;
                ConfigurationSetting.logWrap.ErrorLog(logMessage: e.Message, logNote: e.StackTrace);
            }

            return new RESTResults(result, message, jsonData);
        }

        /// <summary>
        /// Perform a POST request to an SRM API endpoint
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="postData"></param>
        /// <param name="globalContext"></param>
        /// <returns></returns>
        public static RESTResults PerformPOST(string endpoint, string postData, ref IGlobalContext globalContext)
        {
            string jsonData = null;
            string message = null;
            bool result = false;
            Stopwatch stopwatch = new Stopwatch();

            try
            {
                var request = CreateHTTPRequest(HttpMethod.Post, endpoint, ref globalContext);
                if (request != null)
                {
                    request.Content = new StringContent(postData, Encoding.UTF8, "application/json");

                    stopwatch.Start();
                    result = RunRequest(request, ref jsonData, ref globalContext);
                    stopwatch.Stop();

                    if (result)
                    {
                        ConfigurationSetting.logWrap.DebugLog(logMessage: Properties.Resources.POSTResponseMessage, logNote: String.Format("{0} {1}", endpoint, jsonData), timeElapsed: (int)stopwatch.ElapsedMilliseconds);
                    }
                    else
                    {
                        ConfigurationSetting.logWrap.ErrorLog(logMessage: Properties.Resources.POSTErrorMessage, logNote: String.Format("{0} {1}", endpoint, jsonData), timeElapsed: (int)stopwatch.ElapsedMilliseconds);
                    }
                }
                else
                {
                    message = Properties.Resources.OAuthTokenError;
                }
            }
            catch (Exception e)
            {
                message = Properties.Resources.CommunicationErrorMessage;
                ConfigurationSetting.logWrap.ErrorLog(logMessage: e.Message,
                    logNote: e.InnerException != null ? e.InnerException.Message : null + e.InnerException != null ? e.InnerException.InnerException != null ? e.InnerException.InnerException.Message : null : null);
            }

            return new RESTResults(result, message, jsonData);
        }

        /// <summary>
        /// Method to perform PUT operations to the SRM API endpoint
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="postData"></param>
        /// <param name="globalContext"></param>
        /// <returns></returns>
        public static RESTResults PerformPUT(string endpoint, string postData, ref IGlobalContext globalContext)
        {
            string jsonData = null;
            string message = null;
            bool result = false;
            Stopwatch stopwatch = new Stopwatch();

            try
            {
                var request = CreateHTTPRequest(HttpMethod.Put, endpoint, ref globalContext);
                if (request != null)
                {
                    request.Content = new StringContent(postData, Encoding.UTF8, "application/json"); ;
                    
                    stopwatch.Start();
                    result = RunRequest(request, ref jsonData, ref globalContext);
                    stopwatch.Stop();

                    if (result)
                    {
                        ConfigurationSetting.logWrap.DebugLog(logMessage: Properties.Resources.PUTResponseMessage, logNote: String.Format("{0} {1}", endpoint, jsonData), timeElapsed: (int)stopwatch.ElapsedMilliseconds);
                    }
                    else
                    {
                        ConfigurationSetting.logWrap.ErrorLog(logMessage: Properties.Resources.PUTErrorMessage, logNote: String.Format("{0} {1}", endpoint, jsonData), timeElapsed: (int)stopwatch.ElapsedMilliseconds);
                    }
                }
                else
                {
                    message = Properties.Resources.OAuthTokenError;
                }
            }
            catch (Exception e)
            {
                message = Properties.Resources.CommunicationErrorMessage;
                ConfigurationSetting.logWrap.ErrorLog(logMessage: e.Message, logNote: e.StackTrace);
            }

            return new RESTResults(result, message, jsonData);
        }

        /// <summary>
        /// Reusable method that will run the HTTP requests.
        /// This method will attempt a request with the cached oauth token.  If that fails, it will attempt by getting a refreshed server token.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="responseData">A reference to the string that will be used to store a json response</param>
        /// <param name="globalContext">A reference to the global context</param>
        /// <returns></returns>
        private static bool RunRequest(HttpRequestMessage request, ref string responseData, ref IGlobalContext globalContext)
        {
            int count = 0;
            bool results = false;

            do{
                if (count > 0)
                {
                    OAuthHelper.InvalidateCurrentCache();
                    OAuthHelper.Instance.SetCurrentAccessTokenInvalid(globalContext);
                }

                var client = new HttpClient();

                //Create a copy of the request because if the ouath fails, then we cannot re-run the same request.
                //This allows us to run the request twice to check the tokens, if needed.
                var requestCopy = new HttpRequestMessage();
                requestCopy.Content = request.Content;
                requestCopy.Method = request.Method;
                requestCopy.RequestUri = request.RequestUri;

                SetAuthHeader(ref requestCopy, ref globalContext); 
                
                ConfigurationSetting.logWrap.DebugLog(logMessage: String.Format("API Request {0}", count));

                var response = client.SendAsync(requestCopy).Result;

                if (response.IsSuccessStatusCode)
                {
                    responseData = response.Content.ReadAsStringAsync().Result;
                    results = true;
                }
                else
                {
                    ConfigurationSetting.logWrap.ErrorLog(logMessage: String.Format("API Request Attempt {0} Failed with Token: {1}", count, requestCopy.Headers.Authorization.Parameter));
                }

                count++;
            }
            while(count < 2 && !results);

            return results;
        }

        #endregion

        #region Request Configuration Methods

        /// <summary>
        /// Helper method to construct the HTTPRequest Object that is consistant across all requests to the SRM API
        /// </summary>
        /// <param name="method"></param>
        /// <param name="endpoint"></param>
        /// <param name="globalContext"></param>
        /// <returns></returns>
        private static HttpRequestMessage CreateHTTPRequest(HttpMethod method, string endpoint, ref IGlobalContext globalContext)
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri(endpoint),
                Method = method
            };

            return request;
        }

        /// <summary>
        /// Sets the oauth headers on an HTTP request.
        /// This may be used multiple times depending on OAuth token cache invalidation performed by run methods.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="globalContext"></param>
        private static void SetAuthHeader(ref HttpRequestMessage request, ref IGlobalContext globalContext)
        {
            OAuthHelper oauth = OAuthHelper.Instance;
            String oauthToken = oauth.GetOAuthToken(globalContext);

            request.Headers.Remove(@"Authorization");
            request.Headers.Add(@"Authorization", String.Format(@"Bearer {0}", oauthToken));

            ConfigurationSetting.logWrap.DebugLog(logMessage: "Auth header value: " + oauthToken);
        }

        #endregion
    }
}
