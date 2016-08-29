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
 *  date: Tue Aug 23 16:36:00 PDT 2016

 *  revision: rnw-16-8-fixes-release-01
 *  SHA1: $Id: ccd193dfb99440dada499ac278136438885cbd40 $
 * *********************************************************************************************
 *  File: AttrType.java
 * *********************************************************************************************/

package util;

public enum AttrType {
    INT(1),
    STRING(2),
    BOOLEAN(3),
    DATETIME(4),
    LONG(5),
    MENU(6),
    DATE(7),
    DECIMAL(8);
    
    private int _value;

    private AttrType(int value) {
        this._value = value;
    }

    public int getValue() {
        return _value;
    }
}
