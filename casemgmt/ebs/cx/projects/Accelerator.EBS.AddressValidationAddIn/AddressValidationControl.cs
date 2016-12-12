/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + EBS Enhancement
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  EBS release: 12.1.3
 *  reference: 150505-000099, 150420-000127
 *  date: Thu Nov 12 00:52:41 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: c0debbea4e08bf784095ac317eb296c826352d5a $
 * *********************************************************************************************
 *  File: AddressValidationControl.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Accelerator.EBS.AddressValidationAddIn
{
    public partial class AddressValidationControl : UserControl
    {
        public delegate void VerifyAddressHandler(AddressValidationControl uc);
        public event VerifyAddressHandler VerifyAddressClicked;
        public delegate void UsedSuggestedHandler(AddressValidationControl uc);
        public event UsedSuggestedHandler UsedSuggestedChanged;

        public AddressValidationControl()
        {
            InitializeComponent();
        }

        private void verifyAddress_click(object sender, EventArgs e)
        {
            if (VerifyAddressClicked != null)
            {
                VerifyAddressClicked(this);
            }
        }

        private void useSuggested_changed(object sender, EventArgs e)
        {
            if (UsedSuggestedChanged != null)
            {
                UsedSuggestedChanged(this);
            }
        }
    }
}
