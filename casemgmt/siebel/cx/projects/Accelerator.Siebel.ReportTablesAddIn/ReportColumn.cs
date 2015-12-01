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
 *  SHA1: $Id: 8a0ab355912eaad013fcda8b7dc8728033160591 $
 * *********************************************************************************************
 *  File: ReportColumn.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RightNow.AddIns.AddInViews;

/*    class ReportColumn comes with the sample code
 *    It has column Description, Label, Name, 
 *    etc ...
 * 
 */
namespace Accelerator.Siebel.ReportTablesAddin
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
