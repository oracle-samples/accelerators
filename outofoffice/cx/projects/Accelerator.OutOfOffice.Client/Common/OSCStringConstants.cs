/* * *******************************************************************************************
*  $ACCELERATOR_HEADER_PLACE_HOLDER$
*  SHA1: $Id: 041a1395903d8cc45e455912d5e0c388a7b6b57b $
* *********************************************************************************************
*  File: $ACCELERATOR_HEADER_FILE_NAME_PLACE_HOLDER$
* ****************************************************************************************** */

namespace Accelerator.OutOfOffice.Client.Common
{
    
    public static class OracleRightNowOSCAddInNames
    {
        public static readonly string OutOfOfficeAddIn = "OutOfOfficeAddIn";
    }

    public static class RightNowQueries
    {
        public static readonly string GetAccountDetailsQuery = "SELECT CustomFields.c.ooo_start, CustomFields.c.ooo_end,CustomFields.c.ooo_flag, " +
                                                           "CustomFields.c.ooo_msg, CustomFields.c.ooo_msg_option, CustomFields.c.ooo_timezone FROM Account WHERE ID = {0}";
    }

    public static class OSCExceptionMessages
    {
        public static readonly string RightNowConnectServiceNotInitialized = "RightNowConnectService is not initialized";
        public static readonly string TimezoneServiceNotInitialized = "TimezoneService is not initialized";
        public static readonly string UnexpectedError = "Unexpected ERROR, Please contact support.";
        public static readonly string OracleSalesIntegrationSiteWarningMessage = "Please contact your support, the integration with Oracle Sales is not setup correctly for this Service site.";
        public static readonly string OracleSalesIntegrationSiteWarningTitle = "Oracle Sales Integration Configuration Error";
        public static readonly string EndpointNotFound = "Problem with the configuration, Incorrect endpoint found. \nPlease contact support.";
        public static readonly string TimezoneCannotBeEmpty = "Timezone is empty. Cannot save dates without timezone.";
        public static readonly string FromDateGreaterThanToDate = "From Date/Time cannot be greater than To Date/Time.";
    }

    public static class CustomField
    {
        public static readonly string OooFlag = "ooo_flag";
        public static readonly string OooStart = "ooo_start";
        public static readonly string OooEnd = "ooo_end";
        public static readonly string OooMsgOption = "ooo_msg_option";
        public static readonly string OooMsg = "ooo_msg";
        public static readonly string OooTimezone = "ooo_timezone";

        public static readonly string AccountCustomFieldsTypeName = "AccountCustomFields";
        public static readonly string AccountCustomFieldCollectionTypeName = "AccountCustomFieldsc";
    }

    public static class PersonalMsgOptions
    {
        public const string StandardMessage = "Standard Message";
        public const string AppendToStandardMessage = "Append to Standard Message";
        public const string ReplaceStandardMessage = "Replace Standard Message";

        public const string StandardMessageValue = "1";
        public const string AppendToStandardMessageValue = "2";
        public const string ReplaceStandardMessageValue = "3";
    }

    public static class Common
    {
        public const string Title = "Oracle Service cloud Options";
        public const string OutOfOfficeLabel = "Out of Office";
        public const string AvailableLabel = "Available";
    }

}
