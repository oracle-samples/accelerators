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
 *  date: Thu Nov 12 00:52:45 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: 56d805512462e8bea8266fa03eccb8b4f42821db $
 * *********************************************************************************************
 *  File: EBSVirtualReportTablesPackage.cs
 * *********************************************************************************************/

using System.AddIn;
using System.Collections.Generic;
using System.Reflection;
using Accelerator.EBS.SharedServices;
using RightNow.AddIns.AddInViews;
using Accelerator.EBS.SharedServices.ProxyClasses.OrderMgmt;

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
                                new RepairLogisticsVirtualTable(this),
                                new OrderHeaderByContactVirtualTable(this),
                                new OrderHeaderByIncidentVirtualTable(this)
                            };

                IReportTable  orderHeaderTable = new OrderMgmtHeaderVirtualTable(this, ref reportTables);
                reportTables.Add(orderHeaderTable);

                return reportTables;
            }
        }

        #endregion
    }
}
