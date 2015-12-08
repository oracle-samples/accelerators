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
 *  SHA1: $Id: 3a0b2718837cc6a67c60de08ae1e78213e2882bd $
 * *********************************************************************************************
 *  File: ReportDataCell.cs
 * ****************************************************************************************** */

using RightNow.AddIns.AddInViews;

namespace Accelerator.SalesCloud.OpportunityReport.ViewModel
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
