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
 *  SHA1: $Id: 6bfc92896dd8b8534f3b771719c0fda81ea47d40 $
 * *********************************************************************************************
 *  File: ReportTable.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RightNow.AddIns.AddInViews;

namespace Accelerator.SRM.ReportTablesAddIn
{
    public abstract class ReportTable : IReportTable2
    {

        public ReportTable(IReportTablePackage2 package)
        {
            this.parent = package;
            this.columns = new List<IReportTableColumn2>();
        }

        private IReportTablePackage2 parent;
        public IReportTablePackage2 Parent
        {
            get
            {
                return this.parent;
            }
        }

        #region IReportTable Members

        private IList<IReportTableColumn2> columns;
        public IList<IReportTableColumn2> Columns
        {
            get { return this.columns; }
        }

        public string Description { get; set; }
        public string Label { get; set; }
        public string Name { get; set; }

        public abstract IList<IReportRow2> GetRows(IList<string> columns, IReportFilterNode node);
        public abstract void CommitEdits(IDictionary<object, IDictionary<string, object>> editedData);
        
        #endregion
    }
}
