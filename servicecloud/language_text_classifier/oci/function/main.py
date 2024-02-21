
################################################################################################
#  $ACCELERATOR_HEADER_PLACE_HOLDER$
#  SHA1: $Id:  $
################################################################################################
#  File: $ACCELERATOR_HEADER_FILE_NAME_PLACE_HOLDER$
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
