
################################################################################################
#  $ACCELERATOR_HEADER_PLACE_HOLDER$
#  SHA1: $Id:  $
################################################################################################
#  File: $ACCELERATOR_HEADER_FILE_NAME_PLACE_HOLDER$
################################################################################################
####################################
# DataScience related resources
####################################

import os
import time

import oci

from oci.ai_language import AIServiceLanguageClient
from oci.ai_language.models import ObjectStorageDataset, ObjectListDataset

from constants import COMPARTMENT_ID, MODEL_ENDPOINT_CREATION_TIMEOUT, MODEL_TRAINING_TIMEOUT
from utils import get_language_client, wait_for_resource_status


def define_text_classification_model():
    return oci.ai_language.models.TextClassificationModelDetails(
        model_type="TEXT_CLASSIFICATION",
        language_code="en",
        classification_mode=oci.ai_language.models.ClassificationMultiLabelModeDetails(
            classification_mode="MULTI_LABEL"))


def define_object_storage_dataset(namespace, bucket_name, object_names):
    location_details = ObjectListDataset(namespace_name=namespace,
                                         bucket_name=bucket_name,
                                         object_names=object_names)
    return ObjectStorageDataset(location_details=location_details)


def define_model_details(project_id, namespace, bucket_name, object_names):
    return oci.ai_language.models.CreateModelDetails(
        compartment_id=os.getenv(COMPARTMENT_ID),
        project_id=project_id,
        model_details=define_text_classification_model(),
        training_dataset=define_object_storage_dataset(namespace, bucket_name, object_names))


def create_language_model(project_id, namespace,
                          bucket_name, object_names,
                          client: AIServiceLanguageClient = None, opc_request_id="rnow-accelerator"):
    if client is None:
        client = get_language_client()

    create_model_response = client.create_model(
        create_model_details=define_model_details(project_id, namespace, bucket_name, object_names),
        opc_request_id=opc_request_id)

    model_id = create_model_response.data.id
    wait_for_resource_status(client, model_id, "model",
                             create_model_response.data.LIFECYCLE_STATE_ACTIVE, opc_request_id=opc_request_id,
                             timeout=MODEL_TRAINING_TIMEOUT)
    return model_id


def define_language_endpoint(model_id, model_endpoint_name, num_of_inference_unit=1):
    return oci.ai_language.models.CreateEndpointDetails(
        compartment_id=os.getenv(COMPARTMENT_ID),
        model_id=model_id,
        display_name=model_endpoint_name,
        description="Language Endpoint for Accelerator",
        inference_units=num_of_inference_unit)


def create_language_endpoint(model_id,
                             model_endpoint_name,
                             client: AIServiceLanguageClient = None,
                             num_of_inference_unit=1,
                             opc_request_id="rnow-accelerator"):
    create_endpoint_response = client.create_endpoint(
        create_endpoint_details=define_language_endpoint(model_id, model_endpoint_name, num_of_inference_unit),
        opc_request_id=opc_request_id)
    endpoint_id = create_endpoint_response.data.id
    wait_for_resource_status(client, endpoint_id, "endpoint",
                             create_endpoint_response.data.LIFECYCLE_STATE_ACTIVE,
                             start_time=time.time(),
                             opc_request_id=opc_request_id,
                             timeout=MODEL_ENDPOINT_CREATION_TIMEOUT)
    return endpoint_id


def delete_model(model_id, client: AIServiceLanguageClient = None, opc_request_id="rnow-accelerator"):
    client.delete_model(model_id=model_id, opc_request_id=opc_request_id)


def get_active_models(project_id, client: AIServiceLanguageClient = None, page=None,
                      opc_request_id="rnow-accelerator"):
    list_models_response = client.list_models(
        compartment_id=os.getenv(COMPARTMENT_ID),
        project_id=project_id,
        sort_by="timeCreated",
        sort_order="DESC",
        lifecycle_state="ACTIVE",
        page=page,
        opc_request_id=opc_request_id)

    models = []
    for item in list_models_response.data.items:
        models.append(item.id)

    if list_models_response.has_next_page:
        next_models = get_active_models(project_id, client, list_models_response.next_page, opc_request_id)
        models.extend(next_models)

    return models


def get_linked_model_endpoints(project_id, client: AIServiceLanguageClient = None, page=None,
                               opc_request_id="rnow-accelerator"):
    list_endpoints_response = client.list_endpoints(
        lifecycle_state="ACTIVE",
        sort_by="timeCreated",
        sort_order="DESC",
        compartment_id=os.getenv(COMPARTMENT_ID),
        project_id=project_id,
        page=page,
        opc_request_id=opc_request_id)

    models = []
    endpoints = []
    for item in list_endpoints_response.data.items:
        models.append(item.model_id)
        endpoints.append(item.id)

    if list_endpoints_response.has_next_page:
        next_endpoints, next_models = get_linked_model_endpoints(project_id, client, list_endpoints_response.next_page,
                                                                 opc_request_id)
        endpoints.extend(next_endpoints)
        models.extend(next_models)

    return endpoints, models
