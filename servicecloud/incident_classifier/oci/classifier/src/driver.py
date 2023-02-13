################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Tue Jan 31 13:02:52 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: ac0fcabdefcd729a939ea7db4d10f791764a7c75 $
################################################################################################
#  File: driver.py
################################################################################################
import ads
import logging
import os
import datetime

from ads.model.deployment import ModelDeployer
from dateutil.tz import tzutc

from training_scripts.constants import COMPARTMENT_ID, BUCKET_URL, PROJECT_ID, SUBNET_ID, LOG_GROUP_ID, \
    CONDA_ENV_PATH, REPORT_LIST, \
    LOGGING_FORMAT, VERSION
from training_scripts.jobs import model_artifact
from training_scripts.utils import set_config
from training_scripts.utils.job_utility import get_infrastructure, create_job, execute_job, get_model_properties

logging.basicConfig(level=logging.INFO, format=LOGGING_FORMAT)
logger = logging.getLogger(os.path.basename(__file__))


def main():
    """Driver method to run ingestion"""

    set_config()

    if not os.getenv(COMPARTMENT_ID) or not os.getenv(PROJECT_ID) or not os.getenv(SUBNET_ID) or not os.getenv(
            LOG_GROUP_ID) or not os.getenv(REPORT_LIST) or not os.getenv(BUCKET_URL):
        # get project info from here https://cloud.oracle.com/data-science/projects?region=us-ashburn-1
        # get subnet info from here https://cloud.oracle.com/networking/vcns?region=us-ashburn-1
        # create log ids from here https://cloud.oracle.com/logging/logs?region=us-ashburn-1
        raise RuntimeError(f"Please set required environment variables. {COMPARTMENT_ID}, "
                           f"{BUCKET_URL}, {PROJECT_ID}, {SUBNET_ID}, {LOG_GROUP_ID},"
                           f"{CONDA_ENV_PATH}, {REPORT_LIST}")

    current_time = datetime.datetime.now(tz=tzutc())
    infrastructure = get_infrastructure()

    # logger.info("*** SET UP CONDA ENVIRONMENT ***")
    # TODO: LV-18098https://jira.oraclecorp.com/jira/browse/LV-18098
    #  NEED TO FIGURE OUT, HOW WE COULD SETUP CONDA ENVIRONMENT IN OCI JOBS
    # setup_conda_env = create_job("create_conda_env", "./training_scripts/jobs/conda_environment.py",
    #                              infrastructure, current_time)
    # execute_job(setup_conda_env, wait=True)
    # logger.info("*** SET UP CONDA COMPLETE ***")

    """The bottom code might not needed, once we are able to figure out creating the conda environment from OCI Jobs"""
    os.environ.setdefault(CONDA_ENV_PATH,
                          "oci://idcs-mkanojiy-internal@bmtazk5pqzzh/conda_environments/cpu/b2c_env/1.0/b2c_env_slug")

    ingestion_job = create_job(f"ingest-data-{os.getenv(VERSION)}", "./training_scripts/jobs/get_report.py",
                               infrastructure, current_time)
    execute_job(ingestion_job, False)

    logger.info("*** Train Automated Classification Model and Prepare ADS Artifacts ***")
    model = model_artifact.build()

    model_deployment_properties = get_model_properties(model.id, "access_log_id")
    deployer = ModelDeployer()
    deployment = deployer.deploy(model_deployment_properties, wait_for_completion=False)

    logger.info(f"Deployment {deployment.model_deployment_id} is {deployment.state.name}")


if __name__ == '__main__':
    if os.getenv("OCI_RESOURCE_PRINCIPAL_VERSION") is not None:
        ads.set_auth(auth='resource_principal')
    main()
