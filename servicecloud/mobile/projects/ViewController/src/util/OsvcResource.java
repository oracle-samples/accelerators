/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published 
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 
 *  included in the original distribution. 
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved. 
  ***********************************************************************************************
 *  Accelerator Package: OSVC Mobile Application Accelerator 
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html 
 *  OSvC release: 16.11 (November 2016) 
 *  date: Mon Dec 12 02:05:30 PDT 2016 
 *  revision: rnw-16-11

 *  SHA1: $Id$
 * *********************************************************************************************
 *  File: This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.

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
