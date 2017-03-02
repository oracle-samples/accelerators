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
 *  SHA1: $Id: 2c061a1f61ca1107683003627593898698dfb38b $
 * *********************************************************************************************
 *  File: ReportDataRow.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RightNow.AddIns.AddInViews;

namespace Accelerator.SRM.ReportTablesAddIn
{
    public class ReportDataRow : IReportRow2
    {
        public ReportDataRow(int columns)
        {
            this.cells = new List<IReportCell>(columns);
            this.key = new ReportDataCell();
        }

        #region IReportRow Members

        private IList<IReportCell> cells;
        public IList<IReportCell> Cells
        {
            get { return this.cells; }
        }

        private object key;
        public object Key
        {
            get { return this.key; }
            set { this.key = value; }
        }
        #endregion
    }
}
