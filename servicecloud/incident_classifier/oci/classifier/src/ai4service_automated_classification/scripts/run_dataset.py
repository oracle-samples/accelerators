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
#  date: Mon Jun 26 10:43:20 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: 4db985178feb019ba54dc679ca4323f6da176fb2 $
################################################################################################
#  File: run_dataset.py
################################################################################################

"""Run Dataset Script."""
import logging
import os
from argparse import ArgumentParser, Namespace
from typing import Dict, List, Tuple, Optional, Any

import pandas as pd
from sklearn.metrics import confusion_matrix

from ai4service_automated_classification.constants import ID_COLUMN, PRODUCT_ID_COLUMN, NAME_COLUMN, \
    INITIAL_PRODUCT_ID_COLUMN, DATA_COLUMN, TEST_RATIO, CATEGORY_ID_COLUMN, DISPOSITION_ID_COLUMN, ACCURACY_METRIC, \
    BUILD_MODEL_PARAMS, INITIAL_ID_COLUMN, FEATURE_COLUMNS, RESPONSE_KEY, INITIAL_CATEGORY_ID_COLUMN, \
    INITIAL_DISPOSITION_ID_COLUMN, HIERARCHY_COLUMNS
from ai4service_automated_classification.ml.DummyModel import DummyModel
from ai4service_automated_classification.ml.pipeline_train_evaluate import split_train_evaluate, get_pipeline
from ai4service_automated_classification.ml.util.data_util import preprocess_incidents, preprocess_hierarchy

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


def build_argparser() -> ArgumentParser:
    """
    Parse command line arguments.

    @return: ArgumentParser
    """
    parser = ArgumentParser()
    parser.add_argument("-d", "--directory", required=True, type=str,
                        help="Directory Path to Local Data Location for Incidents and Hierarchies Dataset."
                             "\n Ex: run_dataset.py -d ./data -o ./reports")
    parser.add_argument("-o", "--output", required=False, type=str,
                        help="Directory Path to save the reports of confusion matrix")
    return parser


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


def validate_csv_paths(args: Namespace) -> List:
    """
    Validate parsed arguments.

    @return list of valid csv file paths.
    """
    if not os.path.exists(args.directory):
        logger.error("Please provide correct directory location which contains incidents data and hierarchies data.")

    data_paths = [os.path.join(args.directory, path) for path in os.listdir(args.directory)]
    csv_file_paths = [path for path in data_paths if path.lower().endswith('.csv') and os.path.isfile(path)]
    if len(csv_file_paths) == 0:
        raise FileNotFoundError("Please pass directory that contains incidents and hierarchies CSV files.")
    if len(csv_file_paths) < 2:
        raise FileNotFoundError("At least, 2 CSV files required. One for incidents data and another for hierarchy.")
    return csv_file_paths


def get_processed_dataset(paths: List) -> Tuple:
    """
    Retrieve processed incidents and relevant hierarchies dataset.

    @param paths: list of file paths which contains csv files of incidents and hierarchies.
    @return tuple of incidents data and hierarchies.
    """
    data_frames = [pd.read_csv(path) for path in paths]
    incidents_data = retrieve_incidents_data(data_frames)

    if incidents_data is None:
        raise FileNotFoundError("Please make sure that incident data exists in directory")

    product_hierarchy = category_hierarchy = disposition_hierarchy = pd.DataFrame(columns=HIERARCHY_COLUMNS)
    incident_data_features = set(incidents_data.columns.str.lower())
    if {PRODUCT_ID_COLUMN, INITIAL_PRODUCT_ID_COLUMN}.issubset(incident_data_features):
        product_hierarchy = retrieve_hierarchy(data_frames, PRODUCT_ID_COLUMN)
    if {CATEGORY_ID_COLUMN, INITIAL_CATEGORY_ID_COLUMN}.issubset(incident_data_features):
        category_hierarchy = retrieve_hierarchy(data_frames, CATEGORY_ID_COLUMN)
    if {DISPOSITION_ID_COLUMN, INITIAL_DISPOSITION_ID_COLUMN}.issubset(incident_data_features):
        disposition_hierarchy = retrieve_hierarchy(data_frames, DISPOSITION_ID_COLUMN)

    if all([len(product_hierarchy) == 0, len(category_hierarchy) == 0, len(disposition_hierarchy) == 0]):
        raise InterruptedError("Please provide at least one hierarchy dataset")

    return preprocess_incidents(incidents_data), {
        PRODUCT_ID_COLUMN: preprocess_hierarchy(product_hierarchy),
        CATEGORY_ID_COLUMN: preprocess_hierarchy(category_hierarchy),
        DISPOSITION_ID_COLUMN: preprocess_hierarchy(disposition_hierarchy)
    }


def main(arguments: Namespace):
    """
    Driver method for run_dataset.

    @param arguments: Parsed command line arguments
    """
    data_paths = validate_csv_paths(arguments)
    # Load Data
    incidents_data, hierarchy_data = get_processed_dataset(data_paths)

    model_metrics: Dict[str, Dict] = {ACCURACY_METRIC: {}}
    for target, param in BUILD_MODEL_PARAMS.items():
        if len(hierarchy_data[target]) == 0:
            continue
        pipeline = get_pipeline(hierarchy_data[target], incidents_data[target], param[INITIAL_ID_COLUMN], target)
        model, metrics, test_idx = split_train_evaluate(incidents_data=incidents_data[target],
                                                        pipeline=pipeline,
                                                        feature_columns=param[FEATURE_COLUMNS],
                                                        target_column=target,
                                                        test_ratio=TEST_RATIO)
        if isinstance(model, DummyModel):
            logger.warning(f"{param[RESPONSE_KEY]} does not have enough data to train a model")
            continue
        # recover the test set for further processing, if needed
        x_test = incidents_data[target][param[FEATURE_COLUMNS]].iloc[test_idx]
        y_test = incidents_data[target][target].iloc[test_idx]
        predictions = model.predict(x_test)
        conf_mat = confusion_matrix(y_test, predictions)

        logger.info(f"{param[RESPONSE_KEY]}'s confusion matrix example: \n{conf_mat}")

        if HAS_PYCM:
            if len(y_test.values) > 0:
                cm = ConfusionMatrix(actual_vector=y_test.values, predict_vector=predictions)
                id2name = dict(zip(hierarchy_data[target][ID_COLUMN].values,
                                   hierarchy_data[target][NAME_COLUMN].values))
                relabel = {}
                for label in cm.classes:
                    relabel[label] = id2name.get(label, 'unk') + '_' + str(label)
                cm.relabel(mapping=relabel)

                if arguments.output:
                    if os.path.isabs(arguments.output):
                        output_dir = arguments.output
                    else:
                        output_dir = os.path.join(os.getcwd(), arguments.output)

                    if not os.path.exists(output_dir):
                        os.mkdir(output_dir)

                    logger.info(f"Output Directory: {output_dir}")
                    cm.save_html(os.path.join(output_dir, "confusion_matrix_{}".format(param[RESPONSE_KEY])),
                                 shortener=False)
                else:
                    cm.save_html("confusion_matrix_{}".format(param[RESPONSE_KEY]), shortener=False)

        for metric_key, value in model_metrics.items():
            value.update(metrics[metric_key])

    logger.info(f"Metrics: {model_metrics}")


if __name__ == '__main__':
    args = build_argparser().parse_args()
    main(args)
