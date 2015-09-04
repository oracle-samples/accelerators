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
 *  date: Thu Sep  3 23:14:02 PDT 2015

 *  revision: rnw-15-11-fixes-release-03
*  SHA1: $Id: f19cde46465062ac38c2453778f27c36232a2d49 $
* *********************************************************************************************
*  File: ICapacityService.cs
* ****************************************************************************************** */

using Oracle.RightNow.Toa.Client.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oracle.RightNow.Toa.Client.Services
{
    public delegate void CapacityServiceDelegate(ToaRequestResult result);

    public interface ICapacityService
    {
        void GetWorkOrderArea(CapacityModel model, CapacityServiceDelegate capacityCallback);
        ToaRequestResult GetQuotaForMonth(CapacityModel model);
    }
}
