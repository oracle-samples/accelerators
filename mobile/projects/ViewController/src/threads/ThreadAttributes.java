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
 *  SHA1: $Id: aa9ce935a0850e09ce4fe23012821c37be99dfbb $
 * *********************************************************************************************
 *  File: ThreadAttributes.java
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
