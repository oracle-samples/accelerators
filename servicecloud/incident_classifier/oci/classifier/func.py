################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Mon Jun 26 10:43:17 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: bfd747222fef36a3dab5eb54cc5a2cddf5b54632 $
################################################################################################
#  File: func.py
################################################################################################
import io
import json
import logging
import os
import oci
import tempfile
import zipfile

from cloudpickle import cloudpickle
from fdk import response
from src.templates import score
from functools import lru_cache
from src.training_scripts.constants import LOGGING_FORMAT, MODEL_OCID
from training_scripts.utils import set_config

logging.basicConfig(level=logging.INFO, format=LOGGING_FORMAT)
logger = logging.getLogger(os.path.basename(__file__))


def load_model_from_catalog(model_id):
    base_dir = tempfile.mkdtemp()
    model_zip_path = os.path.join(base_dir, f"{model_id}.zip")
    if os.getenv("OCI_RESOURCE_PRINCIPAL_VERSION") is not None:
        auth = oci.auth.signers.get_resource_principals_signer()
        data_science_client = oci.data_science.DataScienceClient({}, signer=auth)
    else:
        from oci.config import from_file
        data_science_client = oci.data_science.DataScienceClient(from_file())
    logger.info(f"Model to download in : {model_zip_path}")
    get_model_artifact_content_response = data_science_client.get_model_artifact_content(model_id=model_id)
    logger.info(f"Received Model {model_id}'s Artifacts")
    with open(model_zip_path, 'wb') as file:
        file.write(get_model_artifact_content_response.data.content)
    logger.info(f"Artifacts written successfully on {model_zip_path}")
    with zipfile.ZipFile(model_zip_path, 'r') as zip_ref:
        zip_ref.extractall(base_dir)
    logger.info(f"Artifacts extracted successfully from {model_zip_path}")
    pickle_path = os.path.join(base_dir, 'templates.pkl')
    logger.info(f"pickle_path : {pickle_path} : {os.path.exists(pickle_path)}")
    return pickle_path


@lru_cache(maxsize=10)
def load_model(model_id=None):
    """
    Loads templates from the serialized format

    @param model_id: model OCID from Model Catalog
    @return: templates:  a templates instance on which predict API can be invoked

    """
    os.environ['GIT_PYTHON_REFRESH'] = "quiet"
    set_config()

    if model_id is not None:
        model_path = load_model_from_catalog(model_id)
        if os.path.exists(model_path):
            logger.info(f'Start loading model from templates directory {model_path} ...')
            with open(model_path, "rb") as file:
                loaded_model = cloudpickle.load(file)
            logger.info("Model is successfully loaded.")
            return loaded_model
        else:
            raise Exception(f'artifacts of {model_id} is not found in templates directory {model_path}')
    else:
        raise Exception(f'Model id not found in the context config')


def handler(ctx, data: io.BytesIO = None):
    logger.info(f"Received input of type: {type(data)}")
    input_json = json.loads(data.getvalue())
    config = ctx.Config()
    model_id = config.get(MODEL_OCID)
    logger.info(f"model id : {model_id}")
    model = load_model(model_id)
    logger.info(f"datascience model loaded")
    prediction = score.predict(input_json, model)
    prediction['modelId'] = model_id

    return response.Response(
        ctx, response_data=json.dumps(prediction),
        headers={"Content-Type": "application/json"}
    )
