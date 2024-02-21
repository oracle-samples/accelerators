
################################################################################################
#  $ACCELERATOR_HEADER_PLACE_HOLDER$
#  SHA1: $Id:  $
################################################################################################
#  File: $ACCELERATOR_HEADER_FILE_NAME_PLACE_HOLDER$
################################################################################################
####################################
# DataScience related resources
####################################

import argparse
import logging
import os

from uuid import uuid4

from constants import PROJECT_NAME, MODEL_ENDPOINT_NAME, INFERENCE_UNIT, LOGGING_FORMAT, \
    LANGUAGE_BUCKET_LOCATION, PROJECT_ID
from language import create_language_model, create_language_endpoint, get_linked_model_endpoints, get_active_models, \
    delete_model
from storage import get_object_list
from utils import get_language_client
from project import create_project

LOGGING_LEVEL = logging.INFO
logging.basicConfig(level=LOGGING_LEVEL, format=LOGGING_FORMAT)
logger = logging.getLogger()


def run(is_first_time=False):
    bucket_url = os.getenv(LANGUAGE_BUCKET_LOCATION)

    if not bucket_url.startswith("oci://"):
        raise ValueError("!!! Please provide valid bucket url !!!")

    project_name = os.getenv(PROJECT_NAME, "language_text_classifier")
    model_endpoint_name = os.getenv(MODEL_ENDPOINT_NAME, "language_classifier_endpoint")
    num_of_inference_unit = int(os.getenv(INFERENCE_UNIT, 1))

    opc_request_id = f"rnow-accelerator-{uuid4()}"

    logger.info(f"*** For tracking the flow: OPC_REQUEST_ID: {opc_request_id} ***")

    language_client = get_language_client()
    project_id = os.getenv(PROJECT_ID)
    if project_id is None:
        project_id = create_project(client=language_client,
                                    display_name=project_name,
                                    opc_request_id=opc_request_id)
        os.environ[PROJECT_ID] = project_id

    bucket_name, namespace, object_names = get_object_list(bucket_url)

    logger.info(f"""
    ***
    
    Project Id: {project_id}
    Bucket Name: {bucket_name}
    Namespace:  {namespace}
    Training Files: {object_names}
    
    ***
    """)

    model_id = create_language_model(project_id, namespace, bucket_name, object_names, language_client, opc_request_id)

    logger.info(f"*** Model Id: {model_id} ***")

    if is_first_time:
        endpoint_id = create_language_endpoint(model_id,
                                               model_endpoint_name,
                                               language_client,
                                               num_of_inference_unit=num_of_inference_unit,
                                               opc_request_id=opc_request_id)
        logger.info(f"*** Model Endpoint Id: {endpoint_id} ***")
    else:
        endpoints, models = get_linked_model_endpoints(project_id, language_client, opc_request_id=opc_request_id)
        models = set(models)
        active_models = get_active_models(project_id, language_client, opc_request_id=opc_request_id)
        active_models = set(active_models)
        models_to_remove = list(models.difference(active_models)) + list(active_models.difference(models))
        for old_model in models_to_remove[:-5]:
            if old_model != model_id:
                delete_model(old_model, language_client, opc_request_id=opc_request_id)
                logger.info(f" {old_model} Model Deleted")


if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument("--is_first_time", type=bool, default=True, help="Whether it is the first time job run")

    args = parser.parse_args()
    is_first_time = args.is_first_time
    run(is_first_time)
