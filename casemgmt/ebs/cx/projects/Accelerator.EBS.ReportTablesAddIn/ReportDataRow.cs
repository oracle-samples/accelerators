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
 *  SHA1: $Id: 71844e88de2ad365ccd5f71e1ad9f12936818386 $
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
namespace Accelerator.EBS.ReportTablesAddin
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
