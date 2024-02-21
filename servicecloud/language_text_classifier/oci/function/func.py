
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
#  SHA1: $Id: f389c9bee9d218f217a6a63351ebe28b22d2e4bc $
################################################################################################
#  File: func.py
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
