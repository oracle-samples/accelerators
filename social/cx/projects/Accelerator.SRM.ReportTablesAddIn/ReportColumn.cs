/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  Copyright (c) 2014, 2015, 2016, 2017 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + SRM Integration Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 17.2 (February 2017) 
 *  reference: 160628-000117
 *  date: Fri Feb 10 19:47:41 PST 2017
 
 *  revision: rnw-17-2-fixes-release-2
 *  SHA1: $Id: 8b8223c94704a67bfd574e241530b955e2bfbe59 $
 * *********************************************************************************************
 *  File: ReportColumn.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RightNow.AddIns.AddInViews;

namespace Accelerator.SRM.ReportTablesAddIn
{
    public class ReportColumn : IReportTableColumn2
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

        private bool canEdit;
        public bool CanEdit
        {
            get { return this.canEdit; }
            set { this.canEdit = value; }
        }

        private bool isKey;
        public bool IsKey
        {
            get { return this.isKey; }
            set { this.isKey = value; }
        }

        private bool isNullable;
        public bool IsNullable
        {
            get { return this.isNullable; }
            set { this.isNullable = value; }
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

        private IList<IOptlistItem> optlItems;
        public IList<IOptlistItem> OptlItems
        {
            get { return this.optlItems; }
            set { this.optlItems = value; }
        }
        #endregion
    }
}
