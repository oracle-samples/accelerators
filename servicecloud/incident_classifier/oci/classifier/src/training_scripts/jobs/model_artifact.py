#!/usr/bin/env python3
################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Tue Jan 31 13:02:54 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: 8addc72e9cb3e5c256a2019b54913c34e8d76572 $
################################################################################################
#  File: model_artifact.py
################################################################################################

"""Run Dataset Script."""
import ads
import datetime
import json
import logging
import os
import shutil
import subprocess
import sys
import pandas as pd
import cloudpickle

from typing import List, Optional, Any
from ads.common.model_export_util import prepare_generic_model
from dateutil.tz import tzutc
from pandas import DataFrame
from os.path import dirname, abspath

base_dir = dirname(dirname(dirname(abspath(__file__))))
sys.path.append(base_dir)

from ai4service_automated_classification.constants import PRODUCT_ID_COLUMN, DATA_COLUMN, TEST_RATIO, \
    CATEGORY_ID_COLUMN, DISPOSITION_ID_COLUMN, HIERARCHY_COLUMNS, TARGET_COLUMNS_TRAIN, FEATURE_COLUMNS_SERVE, \
    FEATURE_COLUMNS_TRAIN, INCIDENT_COLUMN, PRODUCT_COLUMN, CATEGORY_COLUMN, DISPOSITION_COLUMN
from ai4service_automated_classification.ml.build_model import build_and_train
from ai4service_automated_classification.ml.util.Strategy import IngestionInFolderStrategy
from ai4service_automated_classification.ml.util.data_util import preprocess_incidents, preprocess_hierarchy, \
    remove_html_tags_single
from ai4service_automated_classification.ml.util.ocs_util import retrieve_hierarchy_data
from training_scripts.constants import BUCKET_URL, CSV_PATTERN, PROJECT_ID, COMPARTMENT_ID, REPORT_LIST, VERSION, \
    MODEL_TRAINING_INTERVAL, INGESTION_JOB_INITIAL_DATA_FETCH_DAYS
from training_scripts.utils import set_config
from training_scripts.utils.ads_utils import generate_artifact_path
from ai4service_automated_classification.utils.object_storage.bucket_vault import set_up_ocs_connection
from ai4service_automated_classification.utils.object_storage.ocs_loaders import OcsCSVLoader
from ai4service_automated_classification.utils.object_storage.utils import parse_ocs_uri, FileInfo

base_dir = dirname(dirname(dirname(abspath(__file__))))

HAS_PYCM = True
try:
    from pycm import ConfusionMatrix
except ModuleNotFoundError:
    HAS_PYCM = False

LOGGING_LEVEL = logging.INFO
LOGGING_FORMAT = '[%(levelname)s] %(asctime)s %(name)s:%(lineno)d - %(message)s'
logging.basicConfig(level=LOGGING_LEVEL, format=LOGGING_FORMAT)
logger = logging.getLogger(os.path.basename(__file__))

# since this is a helper script, we can set all loggers to verbose
loggers = [logging.getLogger(name) for name in logging.root.manager.loggerDict]
for logger in loggers:
    logger.setLevel(LOGGING_LEVEL)


def retrieve_hierarchy(data_frames: List, column: str) -> pd.DataFrame:
    """
    Retrieve hierarchy data frame based on given column name.

    @data_frames: list of hierarchy data frames
    @column: column name which should be exists in data frame
    @rtype DataFrame
    @return: hierarchy data frame
    """
    for i, data_frame in enumerate(data_frames):
        if column in data_frame.columns.str.lower():
            return data_frames.pop(i)
    return pd.DataFrame(columns=HIERARCHY_COLUMNS)


def retrieve_incidents_data(data_frames: List) -> Optional[Any]:
    """
    Retrieve incidents dataset from given data frames.

    @param data_frames: List of dataframes.
    @return incidents data frame and index of data frame, if exists else None.
    """
    for i, data_frame in enumerate(data_frames):
        if DATA_COLUMN in data_frame.columns.str.lower():
            return data_frames.pop(i)
    return None


def transform_json(input_json: Any) -> DataFrame:
    input_json = json.loads(input_json)
    json_data = input_json.get('jsonData', {})
    feature_available = [f in json_data for f in FEATURE_COLUMNS_SERVE]
    if all(feature_available):
        train_dict = {
            train_df_feature: [json_data[input_field]]
            for input_field, train_df_feature in zip(FEATURE_COLUMNS_SERVE, FEATURE_COLUMNS_TRAIN)
        }
        input_df = DataFrame(train_dict, index=[0])
        input_df[DATA_COLUMN] = input_df[DATA_COLUMN].apply(remove_html_tags_single)
        return input_df[FEATURE_COLUMNS_TRAIN]
    else:
        return DataFrame()


def build():
    """
    Driver method for runx_dataset.

    @param args: Parsed command line arguments
    """

    if os.getenv(PROJECT_ID) is None:
        raise RuntimeError("Please set PROJECT_ID in the environment")

    if os.getenv(BUCKET_URL) is None:
        raise RuntimeError("Please set BUCKET_URL in the environment")
    bucket_url = os.getenv(BUCKET_URL)

    # Get the path for the product hierarchy
    report_list = os.getenv(REPORT_LIST).split(',')
    paths = {}
    for report_data in report_list:
        data_path = parse_ocs_uri(bucket_url, report_data)
        if INCIDENT_COLUMN in report_data:
            paths[INCIDENT_COLUMN] = data_path
        elif PRODUCT_COLUMN in report_data:
            paths[PRODUCT_ID_COLUMN] = data_path
        elif CATEGORY_COLUMN in report_data:
            paths[CATEGORY_ID_COLUMN] = data_path
        elif DISPOSITION_COLUMN in report_data:
            paths[DISPOSITION_ID_COLUMN] = data_path
        else:
            logger.info("Unexpected dataset given")

    # paths = {
    #     PRODUCT_ID_COLUMN: parse_ocs_uri(os.path.join(os.path.dirname(bucket_url.rstrip("/")), 'products/')),
    #     CATEGORY_ID_COLUMN: parse_ocs_uri(os.path.join(os.path.dirname(bucket_url.rstrip("/")), 'category/')),
    #     DISPOSITION_ID_COLUMN: parse_ocs_uri(os.path.join(os.path.dirname(bucket_url.rstrip("/")), 'disposition/'))
    # }

    # data_path = parse_ocs_uri(bucket_url, "incidents")
    logger.info(f'Setting up OCI Connection: {paths[INCIDENT_COLUMN]}...')
    ocs_client = set_up_ocs_connection(paths[INCIDENT_COLUMN].ocs_bucket)
    logger.info("Connection with OCI Established")

    incident_path_info = FileInfo(paths[INCIDENT_COLUMN].ocs_path)
    incidents_data: DataFrame = OcsCSVLoader(
        IngestionInFolderStrategy(ocs_client,
                                  num_files_to_load=int(os.getenv(INGESTION_JOB_INITIAL_DATA_FETCH_DAYS)),
                                  regex_filter=CSV_PATTERN)).load(incident_path_info)
    logger.info('Done loading incidents data.')

    # Get the product hierarchy CSV/DataFrame
    logger.info(f"Loading and preprocess hierarchy data files from: \n"
                f"{paths[PRODUCT_ID_COLUMN]}, {paths[CATEGORY_ID_COLUMN]}, {paths[DISPOSITION_ID_COLUMN]} ")

    # Retrieve Hierarchy Files with Data
    hierarchy_data = {
        PRODUCT_ID_COLUMN: DataFrame(columns=HIERARCHY_COLUMNS),
        CATEGORY_ID_COLUMN: DataFrame(columns=HIERARCHY_COLUMNS),
        DISPOSITION_ID_COLUMN: DataFrame(columns=HIERARCHY_COLUMNS)
    }
    for column in TARGET_COLUMNS_TRAIN:
        hierarchy = None
        try:
            hierarchy = retrieve_hierarchy_data(paths[column])
        except Exception as e:
            logger.warning(f"Could not retrieve {column} hierarchy. ", e)
        hierarchy_data[column] = preprocess_hierarchy(hierarchy) \
            if hierarchy is not None else DataFrame(columns=HIERARCHY_COLUMNS)

    # Preprocess the Incidents Data, handle NaNs, prune min count etc
    all_data = preprocess_incidents(incidents_data)

    # Split into training_scripts/ test and return a trained templates
    # on training_scripts and the test set to be evaluated
    multitail, model_metrics = build_and_train(all_data, hierarchy_data, TEST_RATIO)

    logger.info("*** Model Training Completed. Preparing it for ADS Artifact ***")

    # prepare the templates artifact template
    # path_to_generic_model_artifact = tempfile.mkdtemp()
    path_to_generic_model_artifact = generate_artifact_path()
    generic_model_artifact = prepare_generic_model(path_to_generic_model_artifact,
                                                   model=multitail,
                                                   X_sample=[],
                                                   y_sample=[],
                                                   fn_artifact_files_included=False,
                                                   force_overwrite=True,
                                                   data_science_env=True)

    # save_generic_model(generic_model_artifact, path_to_generic_model_artifact)
    # list_model_template_artifacts(path_to_generic_model_artifact)
    # Serialize the templates
    with open(os.path.join(path_to_generic_model_artifact, "templates.pkl"), "wb") as outfile:
        cloudpickle.dump(multitail, outfile)

    # print(generic_model_artifact.metadata_taxonomy.to_dataframe())

    logger.info("*** Copying score.py and conda runtime.yaml to the artifacts")
    shutil.copy(os.path.join(base_dir, './templates/score.py'), path_to_generic_model_artifact)
    # shutil.copy(os.path.join(os.getcwd(), './templates/score.py'), path_to_generic_model_artifact)

    logger.info(f"*** Copying {base_dir} a src folder to the artifacts")
    ai4service_code_path = os.path.join(base_dir, "ai4service_automated_classification")
    src_folder_copy_command = f"rsync -a {ai4service_code_path} {path_to_generic_model_artifact}"

    conda_create_process = subprocess.Popen(src_folder_copy_command, shell=True, stderr=subprocess.PIPE)
    conda_create_process.wait()
    logger.info(f"*** command: {src_folder_copy_command} status: {conda_create_process.returncode}")

    # shutil.copy(os.path.join(os.getcwd(), './templates/runtime.yaml'), path_to_generic_model_artifact)

    generic_model_artifact.reload(model_file_name='templates.pkl')
    payload = {"jsonData": {
        "inquiry": "My iPhone is not working",
        "product": 0,
        "category": 0,
        "disposition": 0
    }}
    logger.info(generic_model_artifact.model.predict(transform_json(json.dumps(payload))))

    mc_model = generic_model_artifact.save(project_id=os.environ[PROJECT_ID],
                                           compartment_id=os.environ[COMPARTMENT_ID],
                                           display_name=f"Incident Classifier-{os.getenv(VERSION)}"
                                                        f"-{datetime.datetime.now(tz=tzutc())}",
                                           description="B2C Automated Classifier",
                                           ignore_pending_changes=True,
                                           timeout=100,
                                           ignore_introspection=True)

    shutil.rmtree(path_to_generic_model_artifact)
    return mc_model


if __name__ == '__main__':
    set_config()
    if os.getenv("OCI_RESOURCE_PRINCIPAL_VERSION") is not None:
        ads.set_auth(auth='resource_principal')
    build()
