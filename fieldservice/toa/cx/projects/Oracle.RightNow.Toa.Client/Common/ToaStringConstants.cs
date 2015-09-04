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
 *  date: Mon Aug 24 09:01:16 PDT 2015

 *  revision: rnw-15-11-fixes-release-01
*  SHA1: $Id: 701d352c0749befe273c4157e1cebbbffc9cf81c $
* *********************************************************************************************
*  File: ToaStringConstants.cs
* ****************************************************************************************** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oracle.RightNow.Toa.Client.Common
{
    
    public static class OracleRightNowToaAddInNames
    {
        public static readonly string OracleRightNowToaClient = "FS Client AddIn";
        public static readonly string WorkOrderAddIn = "WorkOrder AddIn";
    }

    public static class ToaWebServiceNames
    {
        public static readonly string InboundService = "inbound_api";
        public static readonly string ActivityService = "activity_api";
        public static readonly string HistoryService = "history_api";
        public static readonly string CapacityService = "capacity_api";
    }

    public static class RightNowConfigKeyNames
    {
        public static readonly string ToaInboundServiceUrl = "inbound_api_url";
        public static readonly string ToaCapacityServiceUrl = "capacity_api_url";
        public static readonly string ToaActivityServiceUrl = "activity_api_url";
        public static readonly string ToaHistoryServiceUrl = "history_api_url";
        public static readonly string UserName = "username";
        public static readonly string Password = "password";
        public static readonly string CompanyName = "company_name";
        public static readonly string FallbackId = "fallback_id";
        public static readonly string ServerType = "server_type";
        public static readonly string RightnowHost = "rnt_host";
        public static readonly string FsBaseUrl = "fs_base_url";
        public static readonly string FSServiceTimeout = "FSServiceTimeout";        
        public static readonly string CustomCfgFsAccelIntegrations = "CUSTOM_CFG_FS_Accel_Integrations";        
        public static readonly string RedQuotaCutoff = "red_quota_cutoff";
        public static readonly string GreenQuotaCutoff = "green_quota_cutoff";
    }

    public static class RightNowQueries
    {
        public static readonly string GetConfigVerbQuery = "select Configuration.Value from Configuration where lookupname=";
        public static readonly string GetMilestonesQuery = "select MilestoneInstances.ResolutionDueTime from Incident where Incident.ID = {0}";
    }

    public static class ToaExceptionMessages
    {
        public static readonly string RightNowConnectServiceNotInitialized = "RightNowConnectService is not initialized";
        public static readonly string ConfigurationNotInitialized = "FS Configuration Service is not initialized";
        public static readonly string ConfigVerbIsNotSetOrIncorrect = "FS Integration Configuration Verb is not set or is incorrect";
        public static readonly string UnexpectedError = "Unexpected ERROR, Contact Administrator";
        public static readonly string InboudServiceNotInitialized = "Inbound Service is not initialized";
        public static readonly string LogServiceNotInitialized = "Log Service is not initialized";
        public static readonly string CapacityServiceNotInitialized = "Capacity Service is not initialized";
        public static readonly string FieldServiceIntegrationConfigurationError = "Field Service Integration Configuration Error";
        public static readonly string IntegrationWithFieldServiceIsNotSetupForThisServiceSite = "Please contact your support, the integration with Field Service is not setup for this Service site.";


        //Titles
        public static readonly string TitleActivityCreateFailed = "Error:  Error with Activity Creation or Update";
        public static readonly string TitleSystemExperiencesProblem = "Warning:  The System Experienced A Problem.";
        public static readonly string TitleWrkOrderDefnNotSuccessful = "Warning:  Work Order Predecessor / Successor Definition Not Successful.";
        public static readonly string TitleErrorRetreivingWrkOrderArea = "Error:  Error retrieving Work Order Area.";
        public static readonly string TitleAssetOrInventorySystemExperiencedProblem = "Warning:  The System Experienced A Problem Related to Assets or Inventory.";
        public static readonly string TitleWrkOrderAreaSystemExperiencedProblem = "Warning:  The System Experienced A Problem Related to Work Order Area.";
        public static readonly string TitleChangingWrkOrderSystemExperiencedProblem = "Warning:  The System Experienced A Problem While Changing The Work Order";
        public static readonly string TitleUpdateNotSuccessful = "Warning:  Update not successful.";
        public static readonly string TitleError = "Error";

        //Messages
        public static readonly string MsgActivityNotCreated = "Failed to update / create activity.  Please contact support for assistance.";
        public static readonly string MsgWorkOrderNotCreated = "Failed to update / create work order.  Please contact support for assistance.";
        public static readonly string MsgWorkOrderCreatedWithIssues = "The Work Order has been created as an Activity in Field Service; however, one or many issues are associated with it.  Please contact support for assistance.";
        public static readonly string MsgWrkOrderRelnshipNotDefined = "Work Order relationship linking is not defined or has been used incorrectly.  Please contact support for assistance.";
        public static readonly string MsgVerifyAddressDetails = "Please verify the address details.  The Work Order has been created as an Activity in Field Service; however, one or many issues are associated with it.  If problem is not resolved by updating address details, please contact support for assistance.";
        public static readonly string MsgCanNoLongerBeUpdated = "The information in Field Service can no longer be updated, please either contact dispatch directly or support for assistance.";

    }

    public static class ToaLogMessages
    {
        public static readonly string InboundServiceRequest = "Inbound Service Request";
        public static readonly string InboundServiceResponse = "Inbound Service Response";
    }
}
