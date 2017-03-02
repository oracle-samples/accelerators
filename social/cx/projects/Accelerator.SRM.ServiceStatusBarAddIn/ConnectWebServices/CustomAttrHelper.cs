/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  Copyright (c) 2014, 2015, 2016, 2017 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + SRM Integration Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 17.2 (February 2017) 
 *  reference: 160628-000117
 *  date: Fri Feb 10 19:47:42 PST 2017
 
 *  revision: rnw-17-2-fixes-release-2
 *  SHA1: $Id: d949b278a9ce7f2e68e966c7fe1b92a2b4021e27 $
 * *********************************************************************************************
 *  File: CustomAttrHelper.cs
 * *********************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RightNow.AddIns.AddInViews;
using System.Windows.Forms;

namespace Accelerator.SRM.SharedServices
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
