################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Mon Jun 26 10:43:26 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: 1a2873cc77b11a0624332d766405dbe2d39436e9 $
################################################################################################
#  File: job_utility.py
################################################################################################
import os

import oci
from ads.jobs import Job, DataScienceJob, PythonRuntime
from ads.model.deployment import ModelDeploymentProperties
from oci.config import from_file
from os.path import dirname, abspath

from training_scripts.constants import COMPARTMENT_ID, PROJECT_ID, SUBNET_ID, LOG_GROUP_ID, BUCKET_URL, \
    CONDA_ENV_PATH, REPORT_LIST, CX_DOMAIN, ROQL_AI_INCIDENT_OFFSET, \
    ROQL_AI_INCIDENT_LIMIT, VERSION, INGESTION_JOB_INITIAL_DATA_FETCH_DAYS, INCIDENT_REPORT_NAME, \
    INGESTION_JOB, BUILD_MODEL_JOB, OAUTH_USER, OAUTH_ENTITY, OAUTH_PATH, INGESTION_SECRET_ID, AUTH_TYPE, \
    JWT_TIME_EXPIRY_IN_MINUTE

base_dir = dirname(dirname(dirname(abspath(__file__))))


def is_resource_principals_enabled() -> bool:
    """
    Check, if resource principals is enabled.`

    If AIACS_RESOURCE_PRINCIPALS_ENABLED is set and not None, True is returned.
    """
    if os.getenv("OCI_RESOURCE_PRINCIPAL_VERSION") is not None:
        return True
    else:
        return False


def get_logging_client():
    if is_resource_principals_enabled():
        # Initialize service client with default config file
        signer = oci.auth.signers.get_resource_principals_signer()
        logging_client = oci.logging.LoggingManagementClient(config={}, signer=signer)
    else:
        logging_client = oci.logging.LoggingManagementClient(config=from_file())
    return logging_client


def get_ds_client():
    if is_resource_principals_enabled():
        # Initialize service client with default config file
        signer = oci.auth.signers.get_resource_principals_signer()
        data_science_client = oci.data_science.DataScienceClient(config={}, signer=signer)
    else:
        data_science_client = oci.data_science.DataScienceClient(config=from_file())
    return data_science_client


def get_infrastructure():
    compartment_id = os.getenv(COMPARTMENT_ID)

    infrastructure = DataScienceJob() \
        .with_compartment_id(compartment_id) \
        .with_project_id(os.getenv(PROJECT_ID)) \
        .with_subnet_id(os.getenv(SUBNET_ID)).with_shape_name("VM.Standard2.1") \
        .with_block_storage_size(50) \
        .with_log_group_id(os.getenv(LOG_GROUP_ID))
    return infrastructure


def get_ingestion_run_time(entry_point):
    return PythonRuntime().with_working_dir(working_dir='src') \
        .with_source("./") \
        .with_entrypoint(entry_point) \
        .with_environment_variable(COMPARTMENT_ID=os.getenv(COMPARTMENT_ID),
                                   BUCKET_URL=os.getenv(BUCKET_URL),
                                   REPORT_LIST=os.getenv(REPORT_LIST),
                                   CX_DOMAIN=os.getenv(CX_DOMAIN),
                                   OAUTH_USER=os.getenv(OAUTH_USER),
                                   OAUTH_ENTITY=os.getenv(OAUTH_ENTITY),
                                   OAUTH_PATH=os.getenv(OAUTH_PATH),
                                   INGESTION_SECRET_ID=os.getenv(INGESTION_SECRET_ID),
                                   ROQL_AI_INCIDENT_OFFSET=os.getenv(ROQL_AI_INCIDENT_OFFSET),
                                   ROQL_AI_INCIDENT_LIMIT=os.getenv(ROQL_AI_INCIDENT_LIMIT),
                                   INGESTION_JOB_INITIAL_DATA_FETCH_DAYS=os.getenv(INGESTION_JOB_INITIAL_DATA_FETCH_DAYS),
                                   INCIDENT_REPORT_NAME=os.getenv(INCIDENT_REPORT_NAME),
                                   JWT_TIME_EXPIRY_IN_MINUTE=os.getenv(JWT_TIME_EXPIRY_IN_MINUTE),
                                   AUTH_TYPE=os.getenv(AUTH_TYPE),
                                   JOB_NAME=INGESTION_JOB) \
        .with_python_path('src') \
        .with_custom_conda(os.getenv(CONDA_ENV_PATH))


def get_build_model_run_time(entry_point):
    return PythonRuntime().with_working_dir(working_dir='src') \
        .with_source("./") \
        .with_entrypoint(entry_point) \
        .with_environment_variable(COMPARTMENT_ID=os.getenv(COMPARTMENT_ID),
                                   PROJECT_ID=os.getenv(PROJECT_ID),
                                   BUCKET_URL=os.getenv(BUCKET_URL),
                                   REPORT_LIST=os.getenv(REPORT_LIST),
                                   VERSION=os.getenv(VERSION),
                                   JOB_NAME=BUILD_MODEL_JOB) \
        .with_python_path('src') \
        .with_custom_conda(os.getenv(CONDA_ENV_PATH))


def get_project_runtime(entry_point):
    # .with_source should be relative to current directory
    # os.path.join(self.runtime.working_dir, self.runtime.entrypoint)
    return PythonRuntime().with_working_dir(working_dir='src') \
        .with_source("./") \
        .with_entrypoint(entry_point) \
        .with_environment_variable(COMPARTMENT_ID=os.getenv(COMPARTMENT_ID),
                                   BUCKET_URL=os.getenv(BUCKET_URL),
                                   PROJECT_ID=os.getenv(PROJECT_ID),
                                   SUBNET_ID=os.getenv(SUBNET_ID),
                                   LOG_GROUP_ID=os.getenv(LOG_GROUP_ID))


def create_job(name, infrastructure, time):
    """
    Create job to push required reports in bucket

    @param name: name of the job
    @param entrypoint: relative script location to run in job. Ex: ./../jobs/get_report.py
    @param infrastructure: configuration of infrastructure
    @param time: creation time of the job

    @return DataScience Ingestion Job
    """
    create_new_job = True
    create_new_job, current_job_id, data_science_client = get_job(create_new_job, name)
    if create_new_job:
        job = Job(name=f"{name}-{time}")
        job.with_infrastructure(infrastructure)
        entrypoint = "./training_scripts/jobs/schedule_job.py"
        if "ingest-data" in name:
            job.with_runtime(get_ingestion_run_time(entrypoint))
        elif "build" in name:
            job.with_runtime(get_build_model_run_time(entrypoint))
        else:
            job.with_runtime(get_project_runtime(entrypoint))
        return job
    else:
        return data_science_client.get_job(current_job_id)


def get_job(create_new_job, name):
    data_science_client = get_ds_client()
    current_job_id = None
    list_jobs_response = data_science_client.list_job_runs(os.getenv(COMPARTMENT_ID), lifecycle_state='IN_PROGRESS')
    data = list_jobs_response.data
    if len(data) > 0:
        for item in data:
            if name in item.display_name:
                print(item.display_name + "   " + item.lifecycle_state)
                create_new_job = False
                current_job_id = item.job_id
                break
    return create_new_job, current_job_id, data_science_client


def execute_job(job: Job, wait: bool = False):
    """
    Execute the given job.

    @param job: configuration of job
    @param wait: should wait for job to complete. True | False

    @return job succeed or not
    """
    job.create()
    job_run = job.run()
    if wait:
        job_run.watch()
        job.delete()
    return job_run.status == job_run.LIFECYCLE_STATE_SUCCEEDED


def get_model_properties(model_id, access_log_ocid, predict_log_ocid):
    model_deployment_properties = ModelDeploymentProperties(
        model_id
    ).with_prop(
        'display_name', "Incident ClassificationModel Deployment"
    ).with_prop(
        "project_id", os.getenv(PROJECT_ID)
    ).with_prop(
        "compartment_id", os.getenv(COMPARTMENT_ID)
    ).with_logging_configuration(
        os.getenv(LOG_GROUP_ID), access_log_ocid, os.getenv(LOG_GROUP_ID), predict_log_ocid
    ).with_instance_configuration(
        config={"INSTANCE_SHAPE": "VM.Standard2.1", "INSTANCE_COUNT": "1", 'bandwidth_mbps': 10}
    )


def get_log_group_ocid(name, compartment_id=os.getenv(COMPARTMENT_ID)):
    logging_client = get_logging_client()

    # Send the request to service, some parameters are not required, see API
    # doc for more info
    list_log_groups_response = logging_client.list_log_groups(
        compartment_id=compartment_id,
        display_name=name,
        sort_by="timeCreated",
        sort_order="ASC")

    # Get the data from response
    return list_log_groups_response.data[0].id


def get_log_ocid(name, group_log_id=os.getenv(LOG_GROUP_ID)):
    logging_client = get_logging_client()

    list_logs_response = logging_client.list_logs(
        log_group_id=group_log_id,
        display_name=name,
        sort_by="timeCreated",
        sort_order="ASC")
    # Get the data from response
    print(list_logs_response.data)
    return list_logs_response.data[0].id


def create_log(name, group_log_id=os.getenv(LOG_GROUP_ID), compartment_id=os.getenv(COMPARTMENT_ID)):
    logging_client = get_logging_client()

    log_config = oci.logging.models.Configuration(
        source=oci.logging.models.OciService(
            source_type="OCISERVICE",
            service="ai4service-accelerator",
            resource="ai4service-accelerator-resource",
            category="ai4service-acceleratoer-category"),
        compartment_id=compartment_id,
        archiving=oci.logging.models.Archiving(
            is_enabled=False))

    create_log_details = oci.logging.models.CreateLogDetails(
        display_name=name,
        log_type="CUSTOM",
        is_enabled=True,
        configuration=log_config,
        retention_duration=60)

    if group_log_id is not None:
        # Send the request to service, some parameters are not required, see API
        # doc for more info
        create_log_response = logging_client.create_log(
            log_group_id=group_log_id,
            create_log_details=create_log_details)

        return create_log_response
    else:
        # Send the request to service, some parameters are not required, see API
        # doc for more info
        create_log_group_response = logging_client.create_log_group(
            create_log_group_details=oci.logging.models.CreateLogGroupDetails(
                compartment_id=compartment_id,
                display_name=name,
                description="Log Group for ai4service accelerator"))
        return create_log_group_response
