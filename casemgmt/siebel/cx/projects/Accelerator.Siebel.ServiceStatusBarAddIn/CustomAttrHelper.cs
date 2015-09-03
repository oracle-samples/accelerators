/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC Contact Center + Siebel Case Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.5 (May 2015)
 *  Siebel release: 8.1.1.15
 *  reference: 141216-000121
 *  date: Wed Sep  2 23:14:40 PDT 2015

 *  revision: rnw-15-8-fixes-release-01
 *  SHA1: $Id: e3b4d38e32796daa3377b216a93dfbb16af4e56b $
 * *********************************************************************************************
 *  File: CustomAttrHelper.cs
 * *********************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RightNow.AddIns.AddInViews;
using System.Windows.Forms;

namespace Accelerator.Siebel.SharedServices
{
    public class CustomAttrHelper
    {
        public static Dictionary<String, Object> fetchCustomAttrValue(IList<ICustomAttribute> customAttributes, string[] customAttrsNames, int _logIncidentId, int _logContactId)
        {
            Dictionary<String,Object> results = new Dictionary<String,Object>();

            foreach (ICustomAttribute cusAttr in customAttributes)
            {
                if (cusAttr.PackageName == "Accelerator" && customAttrsNames.Contains(cusAttr.GenericField.Name))
                {
                    results.Add(cusAttr.GenericField.Name, cusAttr.GenericField.DataValue.Value);
                    if (customAttrsNames.Length <= 1)
                        break;
                }
            }

            //Check whether custom attribute is defined
            bool foundAll = true;
            foreach(String customAttr in customAttrsNames){
                if (!results.ContainsKey(customAttr))
                {
                    foundAll = false;
                    results.Add(customAttr, null);
                    
                    string logMessage = "Custom attribute is not defined. Cannot get " + customAttr + ".";
                    string logNote = "";
                    ConfigurationSetting.logWrap.ErrorLog(_logIncidentId, _logContactId, logMessage, logNote);
                }
            }
            if (foundAll == false)
            {
                MessageBox.Show("Custom Attribute configuration missing. Please check log for detail.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return results;
        }
    }
}
