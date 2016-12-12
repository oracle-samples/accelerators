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
 *  date: Tue Aug 23 16:36:01 PDT 2016

 *  revision: rnw-16-8-fixes-release-01
 *  SHA1: $Id: 79718ef1894b9892ba361c441ac7a0c3233ec5d7 $
 * *********************************************************************************************
 *  File: TaskStatus.java
 * *********************************************************************************************/

package util;

public enum TaskStatus {  
    
    // IDs could be customized based on the settings in the data source 
    COMPLETED(18),
    IN_PROGRESS(17),
    NOT_STARTED(16);
    
    private int _id;

    private TaskStatus(int id) {
        this._id = id;
    }
    
    public int getId() {
        return _id;
    }
}
