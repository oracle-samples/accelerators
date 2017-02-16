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

import assets.Asset;
import assets.AssetAttributes;

import oracle.adfmf.framework.api.AdfmfJavaUtilities;

import util.Util;

public class AnswerController {
    private Answer answer;
    
    public AnswerController() {
        super();
    }

    public void setAnswer(Answer answer) {
        this.answer = answer;
    }

    public Answer getAnswer() {        
        Integer id;
        Object ido = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.answerId}");

        if (ido instanceof String) {
            id = Integer.valueOf((String) ido);
        } else {
            id = (Integer) ido;
        }

        answer = new Answer();
        System.out.println("loadAsset id: " + id);
        Object obj = Util.loadObject("answers.Answer", "Answers", id.toString(), AnswerAttributes.answerDetailsAttrs);

        if (obj != null) {
            answer = (Answer) obj;
        }
        return answer;
    }
}
