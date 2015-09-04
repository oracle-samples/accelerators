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
 *  date: Mon Aug 24 09:01:18 PDT 2015

 *  revision: rnw-15-11-fixes-release-01
*  SHA1: $Id: 806ff37a09c421c54600ea2e389431b55e0b962f $
* *********************************************************************************************
*  File: RequiredInventoryModel.cs
* ****************************************************************************************** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.RightNow.Toa.Client.InboundProxyService;

namespace Oracle.RightNow.Toa.Client.Model
{
    public class RequiredInventoryModel: InventoryModel
    {
        
        private string _quantity;

        public string Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }
        internal RequiredInventoryElement GetRequiredInventoryElement()
        {
            var requiredInventoryElement = new RequiredInventoryElement();
            requiredInventoryElement.type = Type;
            requiredInventoryElement.quantity = Quantity;
            requiredInventoryElement.model = (Model != null) ? Model : null;
            return requiredInventoryElement;
        }
    }
}
