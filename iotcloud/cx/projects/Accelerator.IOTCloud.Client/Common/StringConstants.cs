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
 *  date: Tue Dec 13 13:23:38 PST 2016
 
 *  revision: rnw-16-11-fixes-release
*  SHA1: $Id: 649bd4e153ca56114ee5b42c70195b28e190f036 $
* *********************************************************************************************
*  File: StringConstants.cs
* ****************************************************************************************** */

namespace Accelerator.IOTCloud.Client.Model
{
    
    public static class OracleRightNowIOTAddInNames
    {
        public static readonly string ORACLE_RIGHTNOW_IOT_CLIENT = "IOT Client AddIn";        
    }

    public static class RightNowConfigKeyNames
    {
        public static readonly string CUSTOM_CFG_IOT_ACCEL_INTEGRATIONS = "CUSTOM_CFG_IOT_Accel_Integrations";
        public static readonly string RIGHTNOW_HOST = "rnt_host";

        public static readonly string ICS_USERNAME = "username";
        public static readonly string ICS_PASSWORD = "password";
        public static readonly string ICS_BASE_URL = "base_url";
        public static readonly string ICS_GET_MESSAGES_URL = "get_messages_url";
        public static readonly string ICS_GET_ATTRIBUTES_URL = "get_attributes_url";
        public static readonly string ICS_SET_ATTRIBUTES_URL = "set_attributes_url";
    }

    public static class RightNowQueries
    {
        public static readonly string GET_CONFIG_VERB_QUERY = "select Configuration.Value from Configuration where lookupname=";
        public static readonly string GET_CONTACT_EXTERNAL_REFERENCE_QUERY = "SELECT ExternalReference FROM Contact WHERE ID = {0}";
        public static readonly string GET_ORGANIZATION_EXTERNAL_REFERENCE_QUERY = "SELECT ExternalReference FROM Organization WHERE ID = {0}";
        public static readonly string GET_DEVICE_ID_QUERY = "SELECT CO.AssetIoTExtension.iot_device_id FROM CO.AssetIoTExtension WHERE CO.AssetIoTExtension.asset_id = {0}";
        public static readonly string GET_INCIDENT_ASSET_QUERY = "SELECT Asset.ID FROM Incident WHERE ID = {0}";
    }

    public static class ExceptionMessages
    {
        public static readonly string RIGHTNOW_CONNECT_SERVICE_NOT_INITIALIZED = "RightNowConnectService is not initialized";
        public static readonly string CONFIGURATION_NOT_INITIALIZED = "IOT Configuration Service is not initialized";
        public static readonly string CONFIG_VERB_IS_NOT_SET_OR_INCORRECT = "IOT Integration Configuration Verb is not set or is incorrect";
        public static readonly string UNEXPECTED_ERROR = "Unexpected ERROR, Please contact support.";
        public static readonly string JSON_PARSER_ERROR = "Parsing ERROR, Kindly check your json string.";
        public static readonly string LOG_SERVICE_NOT_INITIALIZED = "Logging Service is not initialized.";
        public static readonly string IOT_MESSAGE_SERVICE_NOT_INITIALIZED = "IOT MessageService is not initialized";
        public static readonly string IOT_ENDPOINT_SERVICE_NOT_INITIALIZED = "IOT EndpointService is not initialized";
        public static readonly string IOT_CONF_ERROR_TITLE = "Internet Of Things Integration Configuration Error";
        public static readonly string IOT_CONF_ERROR_MESSAGE = "Please contact your support, the integration with Internet of Things is not setup for this Service site.";
        public static readonly string NO_DEVICE = "No device mapped to this asset, Please contact your support";
    }

}
