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
 *  date: Mon Nov 30 20:14:26 PST 2015

 *  revision: rnw-15-11-fixes-release-2
 *  SHA1: $Id: fc4ae92081301c9672ec0fdc6188ba2f1ec71687 $
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

namespace Accelerator.Siebel.AddressValidationAddIn
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
