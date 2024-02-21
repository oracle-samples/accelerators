
################################################################################################
#  $ACCELERATOR_HEADER_PLACE_HOLDER$
#  SHA1: $Id:  $
################################################################################################
#  File: $ACCELERATOR_HEADER_FILE_NAME_PLACE_HOLDER$
################################################################################################
####################################
# DataScience related resources
####################################

import io
import json
import logging
from functools import lru_cache

import oci
from fdk import response

from main import predict
from utility import clean

logger = logging.getLogger()


@lru_cache()
def get_client():
    signer = oci.auth.signers.get_resource_principals_signer()
    ai_language_client = oci.ai_language.AIServiceLanguageClient(config={}, signer=signer)
    return ai_language_client


def handler(ctx, data: io.BytesIO = None):  # noqa: E999
    logger.info(f"Received input of type: {type(data)}")
    input_json = json.loads(data.getvalue())
    config = ctx.Config()
    endpoint_id = config.get("ENDPOINT_OCID")
    language_client = get_client()
    try:
        text = clean(input_json["text"])
        if len(text) == 0:
            logger.info("After preprocessing text, got empty string")
            return response.Response(
                ctx, response_data=json.dumps({
                    "error": True,
                    "message": str(e)
                }),
                headers={"Content-Type": "application/json"}
            )
        prediction, confidence = predict(language_client, endpoint_id, text)
        return response.Response(
            ctx, response_data=json.dumps({
                "prediction": prediction,
                "confidence": confidence
            }),
            headers={"Content-Type": "application/json"}
        )
    except Exception as e:
        logger.error(str(e))
        return response.Response(
            ctx, response_data=json.dumps({
                "error": True,
                "message": str(e)
            }),
            headers={"Content-Type": "application/json"}
        )
