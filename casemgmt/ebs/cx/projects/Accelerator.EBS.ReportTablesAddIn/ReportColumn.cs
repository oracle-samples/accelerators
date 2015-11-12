/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + EBS Enhancement
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  EBS release: 12.1.3
 *  reference: 150505-000099, 150420-000127
 *  date: Thu Nov 12 00:52:45 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: 3711510c6e860441562d6d1f405cda97255a8b1e $
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
namespace Accelerator.EBS.ReportTablesAddin
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
