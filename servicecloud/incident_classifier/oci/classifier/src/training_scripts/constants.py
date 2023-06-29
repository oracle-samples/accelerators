################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Mon Jun 26 10:43:25 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: c89bed90542effaa3279429fee0ef26947793f5d $
################################################################################################
#  File: constants.py
################################################################################################
"""Constants used in package accelerator."""
import re
import datetime

LOGGING_FORMAT = '[%(levelname)s] %(asctime)s %(name)s:%(lineno)d - %(message)s'

TIMESTAMP_CHECK_RANGE = 3600  # 1 hour

# Enviorment related constants
BUCKET_URL = "BUCKET_URL"
PROJECT_ID = "project_id"
COMPARTMENT_ID = "compartment_id"
SUBNET_ID = "subnet_id"
LOG_GROUP_ID = "log_group_id"
CUSTOM_LOG_ID = "custom_log_id"
REPORT_LIST = "report_list"
B2C_SITE_AUTH = "B2C_SITE_AUTH"
CX_DOMAIN = "CX_DOMAIN"
VERSION = "version"
INCIDENT_ID = "Incident ID"
THREAD_ID = "Thread Id"
OAUTH_USER = "OAUTH_USER"
OAUTH_ENTITY = "OAUTH_ENTITY"
OAUTH_PATH = "OAUTH_PATH"
INGESTION_SECRET_ID = "INGESTION_SECRET_ID"
AUTH_TYPE = "AUTH_TYPE"
AUTH_TYPE_VALUE = "OAUTH"
JWT_TIME_EXPIRY_IN_MINUTE = "jwt_time_expiry_in_minute"
INCIDENT_CLASSIFIER = "INCIDENT_CLASSIFIER"
MODEL_OCID = "MODEL_OCID"

# job interval related constant
INGESTION_JOB_INITIAL_DATA_FETCH_DAYS = "ingestion_job_initial_data_fetch_days"
INGESTION_JOB_DATA_FETCH_DAYS = "ingestion_job_data_fetch_days"
INGESTION_JOB_REPEAT_INTERVAL_IN_HOUR = "ingestion_job_repeat_interval_in_hour"
MODEL_TRAINING_INTERVAL = "model_training_interval"

# conda env name which is in bucket
CONDA_ENV_NAME = "b2c_env"
CONDA_ENV_PATH = "CONDA_ENV_PATH"

# job related constants
JOB_NAME = "JOB_NAME"
INGESTION_JOB = "ingestion_job"
BUILD_MODEL_JOB = "build_model"

# File Patterns
CSV_PATTERN = ".*\\.csv"

# ADS Artifact file paths
PREDICTION_SCRIPT = "score.py"

AIA_INCIDENTS_REPORT_ROQL = "aia_incidents_report_roql"
AIA_INCIDENTS_REPORT_PATH = "/services/rest/connect/v1.3/queryResults/"
ROQL_AI_INCIDENT_OFFSET = "roql_ai_incident_offset"
INCIDENT_REPORT_NAME = "incident_report_name"
ROQL_AI_INCIDENT_LIMIT = "roql_ai_incident_limit"

AIA_INCIDENTS_ANALYTICS_REPORT_PATH = "/services/rest/connect/v1.4/analyticsReportResults"
AIA_REPORT_LIST_QUERY = "select value from Configurations where LookupName= 'CUSTOM_CFG_REPORT_LIST' LIMIT 1"
AIA_HIERARCHY_DATA_FROM_ROQL = "aia_hierarchy_data_from_roql"
YEAR_MONTH_DATE_FORMAT = "%Y-%m-%d"