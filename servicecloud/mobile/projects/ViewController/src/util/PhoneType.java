/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: Mobile Agent App Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 16.8 (August 2016)
 *  MAF release: 2.3
 *  reference: 151217-000185
 *  date: Tue Aug 23 16:36:01 PDT 2016

 *  revision: rnw-16-8-fixes-release-01
 *  SHA1: $Id: 421705dcd4a7cff4b54b33b24d8553d58992ad2c $
 * *********************************************************************************************
 *  File: PhoneType.java
 * *********************************************************************************************/

package util;

public enum PhoneType {
    
    OFFICE(0),
    MOBILE(1),
    HOME(4);
    
    private Integer _id;

    private PhoneType(Integer id) {
        this._id = id;
    }
    
    public int getId() {
        return _id;
    }
    
    public String getString() {
        return _id.toString();
    }
}

