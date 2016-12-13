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
 *  date: Tue Dec 13 13:23:40 PST 2016
 
 *  revision: rnw-16-11-fixes-release
 *  SHA1: $Id: 03d4189f1b4af4c804e690a005b18a4eae05b16e $
 * *********************************************************************************************
 *  File: IHttpClient.cs
 * ****************************************************************************************** */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accelerator.IOTCloud.Client.Http
{
    public interface IHttpClient
    {
        string Get(string queryString);
        string Post(string queryString);
        string Put();
        string Delete(string queryString);
    }
}
