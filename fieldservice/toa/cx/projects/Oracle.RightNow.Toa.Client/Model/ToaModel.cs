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
*  SHA1: $Id: de1673ba2bd72e102aa290ee696f147944ed4cd4 $
* *********************************************************************************************
*  File: ToaModel.cs
* ****************************************************************************************** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oracle.RightNow.Toa.Client.Model
{
    public abstract class ToaModel : IToaModel
    {
        private Dictionary<string, string> _properties;
        private string _userData;
        private List<ReportMessageModel> _reportMessages;

        public ToaModel()
        {
            _properties = new Dictionary<string, string>();
            _reportMessages = new List<ReportMessageModel>();
            _userData = "";

        }

        /// <summary>
        /// 
        /// </summary>
        public List<ReportMessageModel> ReportMessages {
            get { return _reportMessages; }
            internal set { _reportMessages = value; }
        }

        public void AddReportMessage(string Result,string Type, string Code, string Description) 
        {
            _reportMessages.Add(new ReportMessageModel(Result, Type, Code, Description));
        }
        /// <summary>
        /// Properties
        /// </summary>
        public Dictionary<string, string> Properties
        {
            get { return _properties; }
            set { _properties = value; }
        }

        public string UserData
        {
            get { return _userData; }
            set { _userData = value; }
        }

        public string GetPropertyValue(string key)
        {
            if (_properties != null)
            {
                string value;
                _properties.TryGetValue(key, out value);
                return value;
            }
            return null;
        }

        public void SetPropertyValue(string key,string value)
        {
            if (_properties != null)
            {            
                _properties.Add(key, value);
            }
        }
    }
}
