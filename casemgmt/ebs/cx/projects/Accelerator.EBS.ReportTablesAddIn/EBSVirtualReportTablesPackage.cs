/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + EBS Enhancement
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.5 (May 2015)
 *  EBS release: 12.1.3
 *  reference: 150202-000157
 *  date: Wed Sep  2 23:11:38 PDT 2015

 *  revision: rnw-15-8-fixes-release-01
 *  SHA1: $Id: 393fc313733f60ca3eb2a0755ad4de3a5e453b23 $
 * *********************************************************************************************
 *  File: EBSVirtualReportTablesPackage.cs
 * *********************************************************************************************/

using System.AddIn;
using System.Collections.Generic;
using System.Reflection;
using Accelerator.EBS.SharedServices;
using RightNow.AddIns.AddInViews;

/*    class EBSVirtualReportTablesPackage is the tables package
 *    and Initialize() is the entry point for the add-in
 *    Tables property/member let you add more virtual tables, 
 *    and you need to create a new class for each virtual table
 */
namespace Accelerator.EBS.ReportTablesAddin
{
    [AddIn("SR Data", Version = "1.0.0.0")]
    public class EBSVirtualReportTablesPackage : IReportTablePackage
    {
        #region Implementation of IAddInBase
        public IGlobalContext _globalContext;

        public EBSVirtualReportTablesPackage()
        {
            ConfigurationSetting.logAssemblyVersion(Assembly.GetAssembly(typeof(ConfigurationSetting)), null);
            ConfigurationSetting.logAssemblyVersion(Assembly.GetAssembly(typeof(EBSVirtualReportTablesPackage)), null);
        }

        public bool Initialize(IGlobalContext context)
        {
            _globalContext = context;
            ConfigurationSetting instance = ConfigurationSetting.Instance(context);
            if (!ConfigurationSetting.configVerbPerfect)
            {
                string logMessage = "All Accelerator Add-Ins are not initialized properly because of invalid config verb.";
                ConfigurationSetting.logWrap.ErrorLog(logMessage: logMessage);
                return false;
            }
            else
                return true;
        }

        #endregion

        #region Implementation of IReportTablePackage

        public string Name
        {
            get { return "SRData"; }
        }

        public IList<IReportTable> Tables
        {
            get
            {
                IList<IReportTable> reportTables = new List<IReportTable>
                            {
                                new SRlistVirtualTable(this),
                                new SRdetailVirtualTable(this),
                                new ContactDetailVirtualTable(this),
                                new ItemListVirtualTable(this),
                                new EntitlementListVirtualTable(this),
                                new RepairOrderVirtualTable(this),
                                new RepairLogisticsVirtualTable(this)
                            };
                return reportTables;
            }
        }

        #endregion
    }
}
