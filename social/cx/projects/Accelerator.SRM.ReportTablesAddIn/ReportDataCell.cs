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
 *  SHA1: $Id: 7924c4961f685fa4a2b188cb65dd5b9d13c293e9 $
 * *********************************************************************************************
 *  File: ReportDataCell.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RightNow.AddIns.AddInViews;

namespace Accelerator.SRM.ReportTablesAddIn
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
