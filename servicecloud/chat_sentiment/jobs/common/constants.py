################################################################################################
#  This file is part of the Oracle B2C Service Accelerator Reference Integration set published
#  by Oracle B2C Service licensed under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23C (August 2023) 
#  date: Tue Aug 22 11:57:48 IST 2023
 
#  revision: RNW-23C
#  SHA1: $Id: 16810c245c96e26e961d6839b38023cbeb8c8622 $
################################################################################################
#  File: constants.py
################################################################################################

DEFAULT_ENCODING = 'utf-8'
LANGUAGE_SERVICE_MAX_CHAT_TEXT_LIMIT = 5000
MAX_WORKER_FOR_REPORT_CALL = 15
LOGGING_FORMAT = '[%(levelname)s] %(asctime)s %(name)s:%(lineno)d - %(message)s'
MODEL_OCID = 'MODEL_OCID'
COMPARTMENT_ID = 'COMPARTMENT_ID'
APPLICATION_OCID = 'APPLICATION_OCID'
FUNCTION_NAME = 'FUNCTION_NAME'
OAUTH_USER = "OAUTH_USER"
OAUTH_ENTITY = "OAUTH_ENTITY"
OAUTH_PATH = "OAUTH_PATH"
INGESTION_SECRET_ID = "INGESTION_SECRET_ID"
AUTH_TYPE = "AUTH_TYPE"
AUTH_TYPE_VALUE = "OAUTH"
JOB_INTERVAL_IN_MIN = "JOB_INTERVAL_IN_MIN"
JWT_TIME_EXPIRY_IN_MINUTE = "JWT_TIME_EXPIRY_IN_MINUTE"
INCIDENT_CLASSIFIER = "INCIDENT_CLASSIFIER"
EMOTION_THRESHOLD = "EMOTION_THRESHOLD"
ADD_ON_PERCENTAGE_FOR_NEUTRAL_SAMPLE = "ADD_ON_PERCENTAGE_FOR_NEUTRAL_SAMPLE"
ADD_ON_PERCENTAGE_FOR_POSITIVE_SAMPLE = "ADD_ON_PERCENTAGE_FOR_POSITIVE_SAMPLE"
LANGUAGE_BUCKET_LOCATION = 'LANGUAGE_BUCKET_LOCATION'
MAIN_SOURCE_BUCKET_LOCATION = "MAIN_SOURCE_BUCKET_LOCATION"
CX_DOMAIN = "CX_DOMAIN"
EMOTION_MODEL = "EMOTION_MODEL"
DATE_TIME_FORMAT = '%Y-%m-%dT%H:%M:%SZ'
DATE_FORMAT = '%Y-%m-%d'
negative_list = ["disappointment", "anger", "annoyance", "sadness", "disgust", "disapproval"]
SUBSET_SIZE = 10
POSITIVE_CHAT = 1
NEUTRAL_CHAT = 0
NEGATIVE_CHAT = 2

FEATURE_COLUMNS = "FEATURE_COLUMNS"
TARGET_COLUMNS = "TARGET_COLUMNS"
TEXT_COLUMN = "text"
LABEL_COLUMN = "labels"
MINIMUM_SAMPLES_PER_TARGET = "MINIMUM_SAMPLES_PER_TARGET"
LIMIT_200K = 200000

# job interval related constant
INITIAL_JOB_RUN = "INITIAL_JOB_RUN"
INGESTION_JOB_DATA_FETCH_DAYS = "INGESTION_JOB_DATA_FETCH_DAYS"
INGESTION_JOB_REPEAT_INTERVAL_IN_DAYS = "INGESTION_JOB_REPEAT_INTERVAL_IN_DAYS"
EMOTION_INGESTION_JOB_REPEAT_INTERVAL_IN_DAYS = "EMOTION_INGESTION_JOB_REPEAT_INTERVAL_IN_DAYS"
RECTIFY_JOB_REPEAT_INTERVAL_IN_DAYS = "RECTIFY_JOB_REPEAT_INTERVAL_IN_DAYS"
RECTIFY_EMOTIONS_JOB_REPEAT_INTERVAL_IN_DAYS = "RECTIFY_EMOTIONS_JOB_REPEAT_INTERVAL_IN_DAYS"
MODEL_TRAINING_INTERVAL = "MODEL_TRAINING_INTERVAL"
INGESTION_JOB_INITIAL_DATA_FETCH_DAYS = "INGESTION_JOB_INITIAL_DATA_FETCH_DAYS"

# job related constants
JOB_NAME = "JOB_NAME"
INGESTION_JOB = "ingestion_job"
EMOTION_INGESTION_JOB = "emotion_ingestion_job"
RECTIFY_JOB = "rectify_job"
RECTIFY_EMOTIONS_JOB = "rectify_emotions_job"
BUILD_MODEL_JOB = "build_model"
CHAT_STATUS_UPDATE = "chat_status_update"

# language service related timeouts
DEFAULT_ONE_MINUTE_TIMEOUT = 120
MODEL_TRAINING_TIMEOUT = 60 * 60 * 24
MODEL_ENDPOINT_CREATION_TIMEOUT = 60 * 10
PROJECT_NAME = "PROJECT_NAME"
MODEL_ENDPOINT_NAME = "MODEL_ENDPOINT_NAME"
INFERENCE_UNIT = "INFERENCE_UNIT"
PROJECT_ID = "PROJECT_ID"

# EMOTION RELATED CONFIG
CHAT_ID_PLACEHOLDER = "[CHAT_ID]"
CUSTOM_EMOTION_REPORT_NAME = "ChatDataToBeTaggedFromEmoRoberta"
MANAGER_ASK_FEEDBACK_QUERY = "USE REPORT; select ChatText 'text', SuggestManagerIntervene 'labels' " \
                             "from AIML.ChatAIPredictionInfo where SuggestManagerIntervene is not null " \
                             "and ChatRole == 2 and createdTime >= 'START_DATE' and createdTime < 'END_DATE'"
EMOTION_FEEDBACK_QUERY = f"USE REPORT; select id 'Chat ID', ChatText 'text', SuggestEmotion.LookUpName 'labels', " \
                         f"Emotion 'emotion', EmotionConf 'score' from AIML.ChatAIPredictionInfo " \
                         f"where SuggestEmotion is not null and ChatRole == 2 and id > {CHAT_ID_PLACEHOLDER}"
