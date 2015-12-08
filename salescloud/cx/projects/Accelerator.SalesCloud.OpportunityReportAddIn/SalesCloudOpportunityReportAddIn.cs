/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + OSC Lead Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.11 (November 2015)
 *  OSC release: Release 10
 *  reference: 150505-000122
 *  date: Tue Dec  1 21:42:22 PST 2015

 *  revision: rnw-15-11-fixes-release-2
 *  SHA1: $Id: 41dd83be3c8c134fa478fb5b1b8e45f2494c8e84 $
 * *********************************************************************************************
 *  File: SalesCloudOpportunityReportAddIn.cs
 * ****************************************************************************************** */

using System.Collections.Generic;
using System.AddIn;
using Accelerator.SalesCloud.OpportunityReport.ViewModel;
using RightNow.AddIns.AddInViews;
namespace Accelerator.SalesCloud.OpportunityReportAddIn
{

    [AddIn("Opportunity", Version = "1.0.0.0")]
    public class OpportunityReportAddIn : IReportTablePackage
    {
        #region Implementation of IAddInBase
        public IGlobalContext _globalContext;
        public bool Initialize(IGlobalContext context)
        {
            _globalContext = context;
            return true;
        }

        #endregion

        #region Implementation of IReportTablePackage

        public string Name
        {
            get { return "Opportunity"; }
        }

        public IList<IReportTable> Tables
        {
            get
            {
                IList<IReportTable> reportTables = new List<IReportTable>
                            {
                                new OpportunityVirtualTable(this)
                            };
                return reportTables;
            }
        }

        #endregion
    }
}
