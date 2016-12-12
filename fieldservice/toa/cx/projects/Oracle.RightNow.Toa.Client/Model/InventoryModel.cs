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
 *  date: Thu Sep  3 23:14:01 PDT 2015

 *  revision: rnw-15-11-fixes-release-03
*  SHA1: $Id: bddbfc9023ffaf4b0197347c2bcce65fb4b8bff1 $
* *********************************************************************************************
*  File: InventoryModel.cs
* ****************************************************************************************** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.RightNow.Toa.Client.Common;
using Oracle.RightNow.Toa.Client.InboundProxyService;

namespace Oracle.RightNow.Toa.Client.Model
{
    public class InventoryModel : ToaModel
    {
        private UploadType _uploadType;
        private string _type;
        private string _serial_number;
        private string _model;
        private int _inventoryID;

        public string Model
        {
            get { return _model; }
            set { _model = value; }
        }
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public string SerialNumber
        {
            get { return _serial_number; }
            set { _serial_number = value; }
        }

        public int InventoryID
        {
            get { return _inventoryID; }
            set { _inventoryID = value; }
        }

        ///
        /// This method is only used within toa client to create InventoryElement object
        /// 
        internal InventoryElement GetInventoryElement()
        {
            var inventoryElement = new InventoryElement();
            PropertyElement[] properties = null;
            int size = 1;
            if (null != _serial_number && _serial_number.Trim().Length > 0)
            {
                size = 2;
            }
            properties = new PropertyElement[size];
            properties[0] = new PropertyElement();
            properties[0].label = ActivityProperty.InvTypeLabel;
            properties[0].value = Type;

            if (size == 2)
            {
                properties[1] = new PropertyElement();
                properties[1].label = ActivityProperty.Invsn;
                properties[1].value = SerialNumber;
            }
            inventoryElement.properties = properties;
            return inventoryElement;
        }
    }
}
