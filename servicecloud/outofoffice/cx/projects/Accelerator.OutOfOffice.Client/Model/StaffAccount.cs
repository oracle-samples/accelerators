/* * *******************************************************************************************
*  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC Out of Office Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 16.5 (May 2016) 
 *  reference: 150916-000080
 *  date: Thu Mar 17 23:37:53 PDT 2016
 
 *  revision: rnw-16-5-fixes-release-1
*  SHA1: $Id: 29a952abb5be97d9a37e6b7a23e387d0afb8a1ab $
* *********************************************************************************************
*  File: StaffAccount.cs
* ****************************************************************************************** */

using System;

namespace Accelerator.OutOfOffice.Client.Model
{
    public class StaffAccount
    {
        private int _accountId;
        private DateTime? _oooStart;
        private DateTime? _oooEnd;
        private bool _oooFlag;
        private string _oooMsg;
        private string _oooMsgOption;
        private string _oooTimezone;
        private char[] _charsToRemove = new[] {'"'};

        public int AccountId
        {
            get { return _accountId; }
            set { _accountId = value; }
        }

        public DateTime? OooStart
        {
            get { return _oooStart; }
            set { _oooStart = value; }
        }

        public DateTime? OooEnd
        {
            get { return _oooEnd; }
            set { _oooEnd = value; }
        }

        public bool OooFlag
        {
            get { return _oooFlag; }
            set { _oooFlag = value; }
        }

        public string OooMsgOption
        {
            get { return _oooMsgOption; }
            set { _oooMsgOption = value; }
        }

        public string OooMsg
        {
            get
            {
                if (null != _oooMsg)
                {
                    if (_oooMsg.StartsWith("\"") && _oooMsg.EndsWith("\""))
                    {
                        return _oooMsg.Trim(_charsToRemove);
                    }
                }
                return _oooMsg;
            }
            set { _oooMsg = value;}
        }

        public string OooTimezone
        {
            get { return _oooTimezone; }
            set { _oooTimezone = value; }
        }
    }
}
