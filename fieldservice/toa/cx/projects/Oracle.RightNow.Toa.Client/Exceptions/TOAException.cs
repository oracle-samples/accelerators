/* * *******************************************************************************************
*  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + OFSC Reference Integration
 *  link: http://www-content.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.2 (Feb 2015)
 *  OFSC release: 15.2 (Feb 2015)
 *  reference: 150622-000130
 *  date: Thu Sep  3 23:14:00 PDT 2015

 *  revision: rnw-15-11-fixes-release-03
*  SHA1: $Id: ac441a2675bc0b5f0911a3e5d8be330ba5421c18 $
* *********************************************************************************************
*  File: TOAException.cs
* ****************************************************************************************** */

using System;

namespace Oracle.RightNow.Toa.Client.Exceptions
{
    public class ToaException : Exception
    {
        protected int _resultCode;
        protected string _errorMessage;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resultcode"></param>
        /// <param name="message"></param>
        public ToaException(int resultcode, string errormessage)
            : base(errormessage)
        {
            _resultCode = resultcode;
            _errorMessage = errormessage;
        }

        /// <summary>
        /// 
        /// </summary>
        public int ResultCode
        {
            get { return _resultCode; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ErrorMessage
        {
            get { return _errorMessage; }
        }

    }
}
