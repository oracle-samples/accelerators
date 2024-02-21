
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
#  SHA1: $Id: d3eda70fa2e7be18680ea6ef2b3b0197d1289d87 $
################################################################################################
#  File: constants.py
################################################################################################
####################################
# DataScience related resources
####################################

""" Language Service Related Constants
"""
COMPARTMENT_ID = "COMPARTMENT_ID"
PROJECT_NAME = "PROJECT_NAME"
MODEL_ENDPOINT_NAME = "MODEL_ENDPOINT_NAME"
INFERENCE_UNIT = "INFERENCE_UNIT"
BUCKET_URL = "BUCKET_URL"
MAIN_SOURCE_BUCKET_LOCATION = "MAIN_SOURCE_BUCKET_LOCATION"
INITIAL_JOB_RUN = "INITIAL_JOB_RUN"
ENCODING = "utf-8"
LANGUAGE_SERVICE_CHAR_LIMIT = 5000

# job interval related constant
INGESTION_JOB_DATA_FETCH_DAYS = "INGESTION_JOB_DATA_FETCH_DAYS"
INGESTION_JOB_REPEAT_INTERVAL_IN_HOUR = "INGESTION_JOB_REPEAT_INTERVAL_IN_HOUR"
RECTIFY_JOB_REPEAT_INTERVAL_IN_HOUR = "RECTIFY_JOB_REPEAT_INTERVAL_IN_HOUR"
MODEL_TRAINING_INTERVAL = "MODEL_TRAINING_INTERVAL"
INGESTION_JOB_INITIAL_DATA_FETCH_DAYS = "INGESTION_JOB_INITIAL_DATA_FETCH_DAYS"

# job related constants
JOB_NAME = "JOB_NAME"
INGESTION_JOB = "ingestion_job"
RECTIFY_JOB = "rectify_job"
BUILD_MODEL_JOB = "build_model"

DEFAULT_ONE_MINUTE_TIMEOUT = 60
MODEL_TRAINING_TIMEOUT = 60 * 60 * 24
MODEL_ENDPOINT_CREATION_TIMEOUT = 60 * 10
LIMIT_200K = 200000

LOGGING_FORMAT = '[%(levelname)s] %(asctime)s %(name)s:%(lineno)d - %(message)s'

AIA_REPORT_LIST_QUERY = "select value from Configurations where LookupName= 'CUSTOM_CFG_LANG_REPORT_LIST' LIMIT 1"
AIA_INCIDENTS_REPORT_PATH = "/services/rest/connect/v1.3/queryResults/"
INCIDENT_REPORT_NAME = "incident_report_name"

# Environment related constants
PROJECT_ID = "PROJECT_ID"
CX_DOMAIN = "CX_DOMAIN"
THREAD_ID = "Thread Id"
OAUTH_USER = "OAUTH_USER"
OAUTH_ENTITY = "OAUTH_ENTITY"
OAUTH_PATH = "OAUTH_PATH"
INGESTION_SECRET_ID = "INGESTION_SECRET_ID"
AUTH_TYPE = "AUTH_TYPE"
AUTH_TYPE_VALUE = "OAUTH"
JWT_TIME_EXPIRY_IN_MINUTE = "JWT_TIME_EXPIRY_IN_MINUTE"
INCIDENT_CLASSIFIER = "INCIDENT_CLASSIFIER"
MODEL_OCID = "MODEL_OCID"
FEATURE_COLUMNS = "FEATURE_COLUMNS"
TARGET_COLUMNS = "TARGET_COLUMNS"
TEXT_COLUMN = "text"
LABEL_COLUMN = "labels"
LANGUAGE_BUCKET_LOCATION = "LANGUAGE_BUCKET_LOCATION"
MINIMUM_SAMPLES_PER_TARGET = "MINIMUM_SAMPLES_PER_TARGET"
