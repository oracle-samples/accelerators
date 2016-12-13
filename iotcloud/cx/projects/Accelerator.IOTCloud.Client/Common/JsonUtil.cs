/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: IoT OSvC Bi-directional Integration Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 16.11 (November 2016) 
 *  reference: 151217-000026
 *  date: Tue Dec 13 13:23:38 PST 2016
 
 *  revision: rnw-16-11-fixes-release
 *  SHA1: $Id: 5d51180d87f13dd1199a89cc893c9e30dc072907 $
 * *********************************************************************************************
 *  File: JsonUtil.cs
 * ****************************************************************************************** */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using Accelerator.IOTCloud.Client.Logs;

namespace Accelerator.IOTCloud.Client.Model
{
    public class JsonUtil
    {
        public static T FromJson<T>(String jsonString)
        {
            LogService.GetLog().Notice("FromJson invoked ", jsonString);
            T finalObject = default(T);
            try
            {
                var s = new JavaScriptSerializer();

                finalObject = s.Deserialize<T>(jsonString);

                return finalObject;
            }
            catch (Exception exception)
            {
                LogService.GetLog().Error(ExceptionMessages.JSON_PARSER_ERROR, exception.StackTrace);
                MessageBox.Show(ExceptionMessages.JSON_PARSER_ERROR, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return finalObject;
        }

        public static string ToJson<T>(T obj)
        {
            LogService.GetLog().Notice("ToJson invoked!");

            string jsonString = String.Empty;
            try
            {
                var s = new JavaScriptSerializer();

                jsonString = s.Serialize(obj);

                return jsonString;
            }
            catch (Exception exception)
            {
                LogService.GetLog().Error(ExceptionMessages.JSON_PARSER_ERROR, exception.StackTrace);
                MessageBox.Show(ExceptionMessages.JSON_PARSER_ERROR, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            LogService.GetLog().Notice("ToJson returned ", jsonString);
            return jsonString;
        }
    }
}
