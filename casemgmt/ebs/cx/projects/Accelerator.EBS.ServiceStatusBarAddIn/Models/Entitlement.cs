/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + EBS Enhancement
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  EBS release: 12.1.3
 *  reference: 150505-000099, 150420-000127
 *  date: Thu Nov 12 00:52:48 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: 621aa624e8c4668a082c8a42052e664009ff3e3c $
 * *********************************************************************************************
 *  File: Entitlement.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using Accelerator.EBS.SharedServices.Providers;
using RightNow.AddIns.AddInViews;

namespace Accelerator.EBS.SharedServices
{
    public class Entitlement : ModelObjectBase, IReport
    {
        public static string ListURL { get; set; }
        private static IEBSProvider _provider = null;
        private static Dictionary<string, ReportColumnType> schema { get; set; }
        private Dictionary<string, object> Details { get; set; }
        private decimal HiddenInstanceId { get; set; }
        private string HiddenValidateFlag { get; set; }

        public string ContractID { get; set; }
        public string ContractNumber { get; set; }
        public string ServiceName { get; set; }
        public DateTime? CoverageLevelStartDate { get; set; }
        public DateTime? CoverageLevelEndDate { get; set; }


        public Entitlement() : base()
        {
            // intentionally left blank
        }


        public Entitlement(Dictionary<string, object> details, decimal instance_id, string validate_flag)
            : base()
        {
            this.Details = details;
            this.HiddenInstanceId = instance_id;
            this.HiddenValidateFlag = validate_flag;
            this.ContractID = Convert.ToString(details["CONTRACT_ID"]);
            this.ContractNumber = Convert.ToString(details["CONTRACT_NUMBER"]);
            this.ServiceName = Convert.ToString(details["SERVICE_NAME"]);
            this.CoverageLevelStartDate = Convert.ToDateTime(details["COVERAGE_LEVEL_START_DATE"]);
            this.CoverageLevelEndDate = Convert.ToDateTime(details["COVERAGE_LEVEL_END_DATE"]);
        }

        public static Entitlement[] LookupEntitlementList(decimal instance_id, string validate_flag)
        {
            Entitlement[] itemArr = null;
            try
            {
                // //Switch Provider to call web service
                itemArr = Entitlement._provider.LookupEntitlementList(instance_id, validate_flag);
            }
            catch (Exception)
            {
                throw;
            }
            return itemArr;
        }


        public Tuple<ReportColumnType, object> getVirtualColumnValue(string name)
        {
            return new Tuple<ReportColumnType, object>(Entitlement.schema[name], this.Details[name]);
        }

        public static Dictionary<string, ReportColumnType> getDetailedSchema()
        {
            if (null == Entitlement.schema)
                Entitlement.schema = Entitlement._provider.getEntitlementSchema();
            return Entitlement.schema;
        }

        public static void InitEBSProvider()
        {
            Type t = Type.GetType(ServiceProvider);

            try
            {
                _provider = Activator.CreateInstance(t) as IEBSProvider;
                _provider.InitForEntitlement(ListURL, ServiceUsername, ServicePassword, ServiceClientTimeout);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
