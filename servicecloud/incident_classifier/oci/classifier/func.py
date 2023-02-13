################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Tue Jan 31 13:02:45 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: 5b67a58d949f9e8c2d29c369cec711e7ee962634 $
################################################################################################
#  File: func.py
################################################################################################
import io
import json

from fdk import response
import logging
import src.templates.score as score
import os

from src.training_scripts.constants import LOGGING_FORMAT

model = score.load_model()

logging.basicConfig(level=logging.INFO, format=LOGGING_FORMAT)
logger = logging.getLogger(os.path.basename(__file__))


def handler(ctx, data: io.BytesIO = None):
    logger.info(f"Received input of type: {type(data)}")
    input_json = json.loads(data.getvalue())
    prediction = score.predict(input_json, model)

    return response.Response(
        ctx, response_data=json.dumps(prediction),
        headers={"Content-Type": "application/json"}
    )
