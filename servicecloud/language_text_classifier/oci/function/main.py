
################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2024, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 24B (February 2024) 
#  date: Thu Feb 15 09:26:01 IST 2024
 
#  revision: rnw-24-02-initial
#  SHA1: $Id: 98fe1172ae11b38ece653788327e200d21c2d59d $
################################################################################################
#  File: main.py
################################################################################################
####################################
# DataScience related resources
####################################

import oci


def predict(client, endpoint, text):
    txtc_text_for_testing = oci.ai_language.models.BatchDetectLanguageTextClassificationDetails(
        endpoint_id=endpoint,
        documents=[oci.ai_language.models.TextDocument(key="1", text=text)])
    txtc_inference_result = client.batch_detect_language_text_classification(txtc_text_for_testing)
    response = txtc_inference_result.data.documents[0].text_classification[0]
    return response.label, response.score
