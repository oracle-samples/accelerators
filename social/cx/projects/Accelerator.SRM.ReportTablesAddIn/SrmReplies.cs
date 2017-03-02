/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  Copyright (c) 2014, 2015, 2016, 2017 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + SRM Integration Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 17.2 (February 2017) 
 *  reference: 160628-000117
 *  date: Fri Feb 10 19:47:41 PST 2017
 
 *  revision: rnw-17-2-fixes-release-2
 *  SHA1: $Id: e2840501863f04af2377827723ac5ba239d38112 $
 * *********************************************************************************************
 *  File: SrmReplies.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerator.SRM.ReportTablesAddIn
{
    public class Resource
    {
        public int id { get; set; }
        public string resourceName { get; set; }
        public string resourceType { get; set; }
        public string externalId { get; set; }
    }

    public class Author
    {
        public string name { get; set; }
        public string authorImage { get; set; }
        public string authorProfileUrl { get; set; }
    }

    public class Link
    {
        public string rel { get; set; }
        public string href { get; set; }
    }

    public class Link2
    {
        public string rel { get; set; }
        public string href { get; set; }
    }

    public class Attachment
    {
        public string type { get; set; }
        public string url { get; set; }
    }

    public class Content
    {
        public string id { get; set; }
        public string type { get; set; }
        public string externalId { get; set; }
        public Resource resource { get; set; }
        public Author author { get; set; }
        public string postedAt { get; set; }
        public string status { get; set; }
        public string body { get; set; }
        public int bundleId { get; set; }
        public List<object> labels { get; set; }
        public bool liked { get; set; }
        public int? likesCount { get; set; }
        public List<Link2> links { get; set; }
        public List<Attachment> attachments { get; set; }
    }

    public class Item
    {
        public int id { get; set; }
        public int? parentId { get; set; }
        public string type { get; set; }
        public string postUrl { get; set; }
        public Content content { get; set; }
    }

    public class Link3
    {
        public string rel { get; set; }
        public string href { get; set; }
    }

    public class RootObject
    {
        public int totalResults { get; set; }
        public int count { get; set; }
        public bool hasMore { get; set; }
        public int offset { get; set; }
        public int limit { get; set; }
        public List<Item> items { get; set; }
        public List<Link3> links { get; set; }
    }
}
