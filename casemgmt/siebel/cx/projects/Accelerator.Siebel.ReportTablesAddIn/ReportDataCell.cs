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
 *  date: Mon Nov 30 20:14:27 PST 2015

 *  revision: rnw-15-11-fixes-release-2
 *  SHA1: $Id: b1bfbeb65beb2204de09926e4860b30c15ffaa14 $
 * *********************************************************************************************
 *  File: ReportDataCell.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RightNow.AddIns.AddInViews;

/*    class ReportDataCell comes with the sample code
 *    It has Name, Value, and GenericValue
 */
namespace Accelerator.Siebel.ReportTablesAddin
{
    public class ReportDataCell : IReportCell
    {
        #region IReportCell Members

        public string Name
        {
            get { return ""; }
        }

        public string Value { get; set; }
        public object GenericValue { get; set; }

        #endregion
    }
}
