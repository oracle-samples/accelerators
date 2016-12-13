/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: IoT OSvC Bi-directional Integration Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 16.11 (November 2016) 
 *  reference: 151217-000026
 *  date: Tue Dec 13 13:23:40 PST 2016
 
 *  revision: rnw-16-11-fixes-release
 *  SHA1: $Id: c53405b3f49182fcb183afd9d1ef611ae0dc8103 $
 * *********************************************************************************************
 *  File: HttpClient.cs
 * ****************************************************************************************** */

using System.Windows.Forms;
using Accelerator.IOTCloud.Client.Http;
using System;
using System.IO;
using System.Net;
using System.Text;
using Accelerator.IOTCloud.Client.Logs;

namespace Accelerator.IOTCloud.Client.Model
{
    

    public class HttpClient : IHttpClient
    {
        private ILog _logger = LogService.GetLog();

        public string UrlEndPoint { get; set; }
        public HttpMethodEnum Method { get; set; }
        public string ContentType { get; set; }
        public string PostData { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public HttpClient()
        {
            UrlEndPoint = "";
            Method = HttpMethodEnum.GET;
            PostData = "";
            UserName = "";
            Password = "";
        }

        public HttpClient(string endpoint, string userName, string password)
        {
            UrlEndPoint = endpoint;
            Method = HttpMethodEnum.GET;
            PostData = "";
            UserName = userName;
            Password = password;
        }

        public HttpClient(string endpoint, HttpMethodEnum method, string userName, string password)
        {
            UrlEndPoint = endpoint;
            Method = method;
            PostData = "";
            UserName = userName;
            Password = password;
        }

        public HttpClient(string endpoint, HttpMethodEnum method, string postData, string userName, string password)
        {
            UrlEndPoint = endpoint;
            Method = method;
            PostData = postData;
            UserName = userName;
            Password = password;
        }

        private string GetEncodedCredentials()
        {
            string mergedCredentials = string.Format("{0}:{1}", UserName, Password);
            byte[] byteCredentials = UTF8Encoding.UTF8.GetBytes(mergedCredentials);
            return Convert.ToBase64String(byteCredentials);
        }


        private string MakeRequest(string parameters)
        {
            var requestUrl = (String.IsNullOrEmpty(parameters) ? (UrlEndPoint) : (UrlEndPoint + "?" + parameters));

            var request = (HttpWebRequest) WebRequest.Create(requestUrl);

            request.Method = Method.ToString();
            request.ContentLength = 0;

            string base64Credentials = GetEncodedCredentials();            
            request.Headers.Add("Authorization", "Basic " + base64Credentials);

            if (!string.IsNullOrEmpty(PostData) && Method == HttpMethodEnum.PUT)
            {
                var encoding = new UTF8Encoding();
                var bytes = encoding.GetBytes(PostData);
                request.ContentLength = bytes.Length;
                request.ContentType = "application/json";

                using (var writeStream = request.GetRequestStream())
                {
                    writeStream.Write(bytes, 0, bytes.Length);
                }
            }

            //logging Request
            _logger.Debug("Request : ", JsonUtil.ToJson(request));

            using (var response = (HttpWebResponse) request.GetResponse())
            {
                
                // grab the response
                var responseValue = string.Empty;
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                        using (var reader = new StreamReader(responseStream))
                        {
                            responseValue = reader.ReadToEnd();
                        }
                }

                //logging Response
                _logger.Debug("Response : ", responseValue);

                if (Method == HttpMethodEnum.PUT)
                {
                    return response.StatusCode.ToString();
                }
                
                if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Accepted)
                {
                    var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                    MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return "";
                }

                return responseValue;
            }
        }

        public string Get(string queryString)
        {
            var result = MakeRequest(queryString);

            return result;
        }

        public string Post(string queryString)
        {
            var result = MakeRequest(queryString);

            return result;        
        }

        public string Put()
        {
            var result = MakeRequest(null);
            return result;           
        }

        public string Delete(string queryString)
        {
            var result = MakeRequest(queryString);

            return result;
        }
    }
}