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
 *  SHA1: $Id: a740ccdfda6cac562aea05222c6ea7b594675a7b $
 * *********************************************************************************************
 *  File: OsvcResource.java
 * *********************************************************************************************/

package util;

import java.util.Collection;

/**
 * A simple wrapper to hold the service cloud resource object. It should be implemented by the resource objects.
 */
public interface OsvcResource {

    /**
     * Should return the string name of the service cloud resource like "incidents"
     * @return name of the resource
     */
    public String getResourceName();

    /**
     * Returns the Attribute map for create
     * @return
     */
    public Collection<Attribute> getCreateAttributes();

    /**
     * Returns the Attribute map for read
     * @return
     */
    public Collection<Attribute> getReadAttributes();

    /**
     * Returns the Attribute map for update
     * @return
     */
    public Collection<Attribute> getUpdateAttributes();

}
