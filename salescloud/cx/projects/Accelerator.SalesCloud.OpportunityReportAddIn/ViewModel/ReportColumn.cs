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
 *  date: Mon Sep 19 02:05:30 PDT 2016

 *  revision: rnw-15-11-fixes-release-3
 *  SHA1: $Id: c7447868b5f41ee9056b0ae8c244adf564805897 $
 * *********************************************************************************************
 *  File: ReportColumn.cs
 * ****************************************************************************************** */

using RightNow.AddIns.AddInViews;

namespace Accelerator.SalesCloud.OpportunityReport.ViewModel
{
    public class ReportColumn : IReportTableColumn
    {
        #region IReportTableColumn Members

        private bool canDisplay;
        public bool CanDisplay
        {
            get { return this.canDisplay; }
            set { this.canDisplay = value; }
        }

        private bool canFilter;
        public bool CanFilter
        {
            get { return this.canFilter; }
            set { this.canFilter = value; }
        }

        private ReportColumnType dataType;
        public ReportColumnType DataType
        {
            get { return this.dataType; }
            set { this.dataType = value; }
        }

        private string description;
        public string Description
        {
            get { return this.description; }
            set { this.description = value; }
        }

        private string label;
        public string Label
        {
            get { return this.label; }
            set { this.label = value; }
        }

        private string name;
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        #endregion
    }
}
