/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC Contact Center + Siebel Case Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  Siebel release: 8.1.1.15
 *  reference: 150520-000047
 *  date: Thu Nov 12 00:55:34 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: e32ded298232cea044d1e9a011b767d8a46eb117 $
 * *********************************************************************************************
 *  File: SiebelVirtualReportTablesPackage.cs
 * *********************************************************************************************/

using System.AddIn;
using System.Collections.Generic;
using System.Reflection;
using Accelerator.Siebel.SharedServices;
using RightNow.AddIns.AddInViews;

/*    class SiebelVirtualReportTablesPackage is the tables package
 *    and Initialize() is the entry point for the add-in
 *    Tables property/member let you add more virtual tables, 
 *    and you need to create a new class for each virtual table
 */
namespace Accelerator.Siebel.ReportTablesAddin
{
    [AddIn("Siebel Data", Version = "1.0.0.0")]
    public class SiebelVirtualReportTablesPackage : IReportTablePackage
    {
        #region Implementation of IAddInBase
        public IGlobalContext _globalContext;

        public SiebelVirtualReportTablesPackage()
        {
            ConfigurationSetting.logAssemblyVersion(Assembly.GetAssembly(typeof(ConfigurationSetting)), null);
            ConfigurationSetting.logAssemblyVersion(Assembly.GetAssembly(typeof(SiebelVirtualReportTablesPackage)), null);
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
            get { return "Siebel"; }
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
                                new AssetVirtualTable(this),
                                new AssetListVirtualTable(this),
                                new ActivityListVirtualTable(this),
                                new ActivityDetailVirtualTable(this)
                            };
                return reportTables;
            }
        }

        #endregion
    }
}
