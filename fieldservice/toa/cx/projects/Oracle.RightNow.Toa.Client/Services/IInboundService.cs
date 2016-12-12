/* * *******************************************************************************************
*  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + OFSC Reference Integration
 *  link: http://www-content.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.2 (Feb 2015)
 *  OFSC release: 15.2 (Feb 2015)
 *  reference: 150622-000130
 *  date: Thu Sep  3 23:14:02 PDT 2015

 *  revision: rnw-15-11-fixes-release-03
*  SHA1: $Id: c5486724a3a45c149b151c1b29901898ad3fa366 $
* *********************************************************************************************
*  File: IInboundService.cs
* ****************************************************************************************** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.RightNow.Toa.Client.Common;
using Oracle.RightNow.Toa.Client.Model;

namespace Oracle.RightNow.Toa.Client.Services
{
    public delegate void InboundServiceDelegate(ToaRequestResult result);

    /// <summary>
    /// Inbound Service contract
    /// </summary>
    public interface IInboundService
    {
        /// <summary>
        /// Begin Async SOAP request
        /// </summary>
        /// <param name="inboundRequest"></param>
        /// <param name="inboundServiceCallback"></param>
        void BeginRequest(InboundRequest inboundRequest, InboundServiceDelegate inboundServiceCallback);
    }
}
