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
 *  date: Thu Nov 12 00:52:46 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: 5aa539cef52282ad3007b0d58a287288c9f8a96a $
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
namespace Accelerator.EBS.ReportTablesAddin
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
