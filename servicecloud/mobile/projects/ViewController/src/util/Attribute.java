/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: Mobile Agent App Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 16.8 (August 2016)
 *  MAF release: 2.3
 *  reference: 151217-000185
 *  date: Tue Aug 23 16:36:00 PDT 2016

 *  revision: rnw-16-8-fixes-release-01
 *  SHA1: $Id: 5297d2f2061f865717055307509ca1d3b0a97739 $
 * *********************************************************************************************
 *  File: Attribute.java
 * *********************************************************************************************/

package util;

public class Attribute {
    protected String attrName;
    protected AttrType attrType;
    protected String attrPath;
    
    public Attribute(String attrName, AttrType attrType, String attrPath) {
        this.attrName = attrName;
        this.attrType = attrType;
        this.attrPath = attrPath;
    }
}


