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
 *  SHA1: $Id: 48a6a2269c9de667f0fa4a0dc6252157787ad0e8 $
 * *********************************************************************************************
 *  File: RESTResults.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerator.SRM.SharedServices
{
    /// <summary>
    /// A class to help broker data between the RESTHelper and a calling object.
    /// </summary>
    public class RESTResults
    {
        #region Properties

        /// <summary>
        /// The JSON data returned by the API
        /// </summary>
        private string _JSON;
        public string JSON
        {
            get
            {
                return _JSON;
            }
        }

        /// <summary>
        /// A Message constructed by the REST helper to pass back to the calling object, such as an error message.
        /// </summary>
        private string _Message;
        public string Message
        {
            get
            {
                return _Message;
            }
        }

        /// <summary>
        /// Indicates if the API call was successful or not.
        /// </summary>
        private bool _Success = false;
        public bool Success
        {
            get
            {
                return _Success;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Constructor that only includes the success property
        /// </summary>
        /// <param name="success"></param>
        public RESTResults(bool success)
        {
            _Success = success;

            // If the result was not successful, then invalidate any local OAuth cache.
            if (!success) OAuthHelper.InvalidateCurrentCache();
        }

        /// <summary>
        /// Constructor that passes only success and message
        /// </summary>
        /// <param name="success"></param>
        /// <param name="message"></param>
        public RESTResults(bool success, string message) : this(success)
        {
            _Message = message;
        }

        /// <summary>
        /// Constructor with all properties
        /// </summary>
        /// <param name="success"></param>
        /// <param name="message"></param>
        /// <param name="json"></param>
        public RESTResults(bool success, string message, string json) : this(success, message)
        {
            _JSON = json;
        }

        #endregion
    }
}
