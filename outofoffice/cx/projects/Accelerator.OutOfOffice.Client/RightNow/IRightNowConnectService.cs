/* * *******************************************************************************************
*  $ACCELERATOR_HEADER_PLACE_HOLDER$
*  SHA1: $Id: b8ed2774135d89079cdd5f0e2c4b2e2397a598ba $
* *********************************************************************************************
*  File: $ACCELERATOR_HEADER_FILE_NAME_PLACE_HOLDER$
* ****************************************************************************************** */

using System;
using Accelerator.OutOfOffice.Client.Model;
using Accelerator.OutOfOffice.Client.RightNowProxyService;

namespace Accelerator.OutOfOffice.Client.RightNow
{
    public interface IRightNowConnectService
    {

        RightNowSyncPortClient GetRightNowClient();

        string GetRightNowEndPointURIHost();

        StaffAccount GetAccountDetails();

        bool updateCustomFields(StaffAccount staffAccount);
    }
}
