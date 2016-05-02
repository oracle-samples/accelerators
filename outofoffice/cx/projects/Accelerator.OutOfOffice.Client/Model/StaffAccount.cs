/* * *******************************************************************************************
*  $ACCELERATOR_HEADER_PLACE_HOLDER$
*  SHA1: $Id: 8650be601ad642ad72cc1b2713b9028563de7f2a $
* *********************************************************************************************
*  File: $ACCELERATOR_HEADER_FILE_NAME_PLACE_HOLDER$
* ****************************************************************************************** */

using System;

namespace Accelerator.OutOfOffice.Client.Model
{
    public class StaffAccount
    {
        private int _accountId;
        private DateTime _oooStart;
        private DateTime _oooEnd;
        private bool _oooFlag;
        private string _oooMsg;
        private string _oooMsgOption;
        private string _oooTimezone;

        public int AccountId
        {
            get { return _accountId; }
            set { _accountId = value; }
        }

        public DateTime OooStart
        {
            get { return _oooStart; }
            set { _oooStart = value; }
        }

        public DateTime OooEnd
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
            get { return _oooMsg; }
            set { _oooMsg = value; }
        }

        public string OooTimezone
        {
            get { return _oooTimezone; }
            set { _oooTimezone = value; }
        }
    }
}
