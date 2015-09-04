/* * *******************************************************************************************
*  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + OFSC Reference Integration
 *  link: http://www-content.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.2 (Feb 2015)
 *  OFSC release: 15.2 (Feb 2015)
 *  reference: 150622-000130
 *  date: Thu Sep  3 23:14:01 PDT 2015

 *  revision: rnw-15-11-fixes-release-03
*  SHA1: $Id: 864aa66222ebc3ae563df4c85c817c67eefb7d9e $
* *********************************************************************************************
*  File: WorkOrderFieldMapping.cs
* ****************************************************************************************** */

using Oracle.RightNow.Toa.Client.Rightnow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oracle.RightNow.Toa.Client.Model
{
    public class WorkOrderFieldMapping
    {

        private string _Wo_Field;
        private string _WS_Field;
        private int _Data_Sync;
        private string _Related_Object_Field_Lvl_1;
        private string _Related_Object_Lvl_1;

        public string Related_Object_Field_Lvl_1
        {
            get { return _Related_Object_Field_Lvl_1; }
            set { _Related_Object_Field_Lvl_1 = value; }
        }

        public string Related_Object_Lvl_1
        {
            get { return _Related_Object_Lvl_1; }
            set { _Related_Object_Lvl_1 = value; }
        }

        public int Data_Sync
        {
            get { return _Data_Sync; }
            set { _Data_Sync = value; }
        }

        public string Wo_Field
        {
            get { return _Wo_Field; }
            set { _Wo_Field = value; }
        }

        public string WS_Field
        {
            get { return _WS_Field; }
            set { _WS_Field = value; }
        }
    }
}
