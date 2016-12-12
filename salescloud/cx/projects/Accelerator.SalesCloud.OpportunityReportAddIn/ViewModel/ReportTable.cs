/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + OSC Lead Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.11 (November 2015)
 *  OSC release: Release 10
 *  reference: 150505-000122, 160620-000160
 *  date: Mon Sep 19 02:05:31 PDT 2016

 *  revision: rnw-15-11-fixes-release-3
 *  SHA1: $Id: 0181d7f45376530a9e17c9d2b4f51a43949e064d $
 * *********************************************************************************************
 *  File: ReportTable.cs
 * ****************************************************************************************** */

using System.Collections.Generic;
using RightNow.AddIns.AddInViews;

namespace Accelerator.SalesCloud.OpportunityReport.ViewModel
{
    public abstract class ReportTable : IReportTable
    {

        public ReportTable(IReportTablePackage package)
        {
            this.parent = package;
            this.columns = new List<IReportTableColumn>();
        }

        private IReportTablePackage parent;
        public IReportTablePackage Parent
        {
            get
            {
                return this.parent;
            }
        }

        #region IReportTable Members

        private IList<IReportTableColumn> columns;
        public IList<IReportTableColumn> Columns
        {
            get { return this.columns; }
        }

        public string Description { get; set; }
        public string Label { get; set; }
        public string Name { get; set; }

        public abstract IList<IReportRow> GetRows(IList<string> columns, IReportFilterNode node);

        #endregion
    }
}
