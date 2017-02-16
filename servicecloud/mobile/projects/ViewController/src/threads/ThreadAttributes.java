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

package threads;

import java.util.ArrayList;
import util.AttrType;
import util.Attribute;

public class ThreadAttributes {
    public ThreadAttributes() {
        super();
    }
    
    protected static ArrayList<Attribute> threadDetailsAttrs = new ArrayList<Attribute>();
   
    static
    {
        // detail attrs
        //threadDetailsAttrs.add(new Attribute("Id", AttrType.INT, "id"));
        threadDetailsAttrs.add(new Attribute("Text", AttrType.STRING, "text"));
        threadDetailsAttrs.add(new Attribute("EntryTypeId", AttrType.MENU, "entryType.id"));
        threadDetailsAttrs.add(new Attribute("ChannelId", AttrType.MENU, "channel.id"));
        
    }
}
