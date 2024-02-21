
################################################################################################
#  $ACCELERATOR_HEADER_PLACE_HOLDER$
#  SHA1: $Id:  $
################################################################################################
#  File: $ACCELERATOR_HEADER_FILE_NAME_PLACE_HOLDER$
################################################################################################
####################################
# DataScience related resources
####################################

import logging
import os

from oci.ai_language.models import CreateProjectDetails

from constants import COMPARTMENT_ID, DEFAULT_ONE_MINUTE_TIMEOUT, LOGGING_FORMAT
from utils import get_language_client, wait_for_resource_status

LOGGING_LEVEL = logging.INFO
logging.basicConfig(level=LOGGING_LEVEL, format=LOGGING_FORMAT)
logger = logging.getLogger()


def create_project(client=None, display_name="text_classification", opc_request_id="rnow-accelerator"):
    if client is None:
        client = get_language_client()
    project_details = CreateProjectDetails(display_name=display_name,
                                           compartment_id=os.getenv(COMPARTMENT_ID))
    logger.info("*** CREATING PROJECT ***")
    response = client.create_project(project_details)
    project_id = response.data.id
    logger.info(f"*** PROJECT CREATED: {project_id}")

    logger.info("*** WAITING PROJECT CREATION TO BE ACTIVE ***")
    wait_for_resource_status(client, project_id, "project",
                             response.data.LIFECYCLE_STATE_ACTIVE, opc_request_id=opc_request_id,
                             timeout=DEFAULT_ONE_MINUTE_TIMEOUT)

    return project_id
