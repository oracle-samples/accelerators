
################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2024, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 24B (February 2024) 
#  date: Thu Feb 15 09:26:00 IST 2024
 
#  revision: rnw-24-02-initial
#  SHA1: $Id: 1f76df53c1cbfe26ccf5d7a3245a7d846041173a $
################################################################################################
#  File: project.py
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
