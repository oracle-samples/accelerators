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

package answers;

import java.util.ArrayList;

import util.AttrType;
import util.Attribute;

public class AnswerAttributes {
    public AnswerAttributes() {
        super();
    }
    
    protected static ArrayList<Attribute> answerDetailsAttrs = new ArrayList<Attribute>();
    
    static
    {
        // detail attrs
        answerDetailsAttrs.add(new Attribute("Id", AttrType.INT, "id"));
        answerDetailsAttrs.add(new Attribute("Summary", AttrType.STRING, "summary"));
        answerDetailsAttrs.add(new Attribute("Question", AttrType.STRING, "question"));
        //answerDetailsAttrs.add(new Attribute("StatusId", AttrType.MENU, "statusWithType.status.id"));
        answerDetailsAttrs.add(new Attribute("Status", AttrType.STRING, "statusWithType.status.lookupName"));
        answerDetailsAttrs.add(new Attribute("Solution", AttrType.STRING, "solution"));
        answerDetailsAttrs.add(new Attribute("Product", AttrType.STRING, "products.lookupName"));
        answerDetailsAttrs.add(new Attribute("UpdatedTime", AttrType.DATETIME, "updatedTime"));

      
    }
}
