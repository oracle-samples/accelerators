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
 *  SHA1: $Id: e879fd38f6f718fae6416cff91b8c92b079530c9 $
 * *********************************************************************************************
 *  File: OptlItem.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RightNow.AddIns.AddInViews;

namespace Accelerator.SRM.ReportTablesAddIn
{
    public class OptlItem : IOptlistItem
    {
        #region IOptlistItem Members
        private int? id;
        public int? ID
        {
            get { return this.id; }
            set { this.id = value; }
        }

        private string label;
        public string Label
        {
            get { return this.label; }
            set { this.label = value; }
        }

        private int? parentID;
        public int? ParentID
        {
            get { return this.parentID; }
            set { this.parentID = value; }
        }

        private int? parentType;
        public int? ParentType
        {
            get { return this.parentType; }
            set { this.parentType = value; }
        }

        private string tooltip;
        public string Tooltip
        {
            get { return this.tooltip; }
            set { this.tooltip = value; }
        }

        private int? type;
        public int? Type
        {
            get { return this.type; }
            set { this.type = value; }
        }
        #endregion
    }
}
