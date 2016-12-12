/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC Contact Center + Siebel Case Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  Siebel release: 8.1.1.15
 *  reference: 150520-000047
 *  date: Mon Nov 30 20:14:27 PST 2015

 *  revision: rnw-15-11-fixes-release-2
 *  SHA1: $Id: 74de822c234f685704a8e9e237c874a32d4ebb35 $
 * *********************************************************************************************
 *  File: ReportDataRow.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RightNow.AddIns.AddInViews;

/*    class ReportDataRow comes with the sample code
 *    The ReportDataRow contains cells
 */
namespace Accelerator.Siebel.ReportTablesAddin
{
    public class ReportDataRow : IReportRow
    {
        public ReportDataRow(int columns)
        {
            this.cells = new List<IReportCell>(columns);
        }

        #region IReportRow Members

        private IList<IReportCell> cells;
        public IList<IReportCell> Cells
        {
            get { return this.cells; }
        }

        #endregion
    }
}
