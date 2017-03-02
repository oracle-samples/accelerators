/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  Copyright (c) 2014, 2015, 2016, 2017 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + SRM Integration Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 17.2 (February 2017) 
 *  reference: 160628-000117
 *  date: Fri Feb 10 19:47:42 PST 2017
 
 *  revision: rnw-17-2-fixes-release-2
 *  SHA1: $Id: 903c7ab04c35e4b59118f5b60541ced442d9a20d $
 * *********************************************************************************************
 *  File: SrmReportTablePackage.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.AddIn;
using RightNow.AddIns.AddInViews;
using Accelerator.SRM.SharedServices;
using System.Reflection;

namespace Accelerator.SRM.ReportTablesAddIn
{
    [AddIn("SRM Data", Version = "1.0.0.0")]
    public class SrmReportTablePackage : IReportTablePackage2
    {
        #region Implementation of IAddInBase
        public IGlobalContext _globalContext;

        public SrmReportTablePackage()
        {
            ConfigurationSetting.logAssemblyVersion(Assembly.GetAssembly(typeof(ConfigurationSetting)), null);
            ConfigurationSetting.logAssemblyVersion(Assembly.GetAssembly(typeof(SrmReportTablePackage)), null);
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
            get { return "SRM_Data"; }
        }

        public IList<IReportTable2> Tables
        {
            get
            {
                IList<IReportTable2> reportTables = new List<IReportTable2>
                            {
                                new SrmRepliesListVirtualTable(this)
                            };
                return reportTables;
            }
        }

        #endregion
    }
}
