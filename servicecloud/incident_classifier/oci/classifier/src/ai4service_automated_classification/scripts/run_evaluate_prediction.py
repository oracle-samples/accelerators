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
#  date: Tue Jan 31 13:02:48 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: 4e879a6d8c958d616967d3ab2177679e444b0014 $
################################################################################################
#  File: run_evaluate_prediction.py
################################################################################################

"""Run Evaluation Dataset Script."""
import logging
import os
from argparse import Namespace
import time

from run_dataset import build_argparser, validate_csv_paths, get_processed_dataset

from ai4service_automated_classification.constants import GDFS_THRESHOLD, ACCURACY_METRIC, BUILD_MODEL_PARAMS, \
    INITIAL_ID_COLUMN, FEATURE_COLUMNS, TEST_RATIO, RESPONSE_KEY
from ai4service_automated_classification.ml.pipeline_train_evaluate import split_train_evaluate, get_pipeline
from ai4service_automated_classification.ml.DummyModel import DummyModel

LOGGING_LEVEL = logging.INFO
LOGGING_FORMAT = '[%(levelname)s] %(asctime)s %(name)s:%(lineno)d - %(message)s'
logging.basicConfig(level=LOGGING_LEVEL, format=LOGGING_FORMAT)
logger = logging.getLogger(os.path.basename(__file__))

# since this is a helper script, we can set all loggers to verbose
loggers = [logging.getLogger(name) for name in logging.root.manager.loggerDict]
for logger in loggers:
    logger.setLevel(LOGGING_LEVEL)


def main(arguments: Namespace):
    """
    Driver method for run_evaluate_prediction.

    @param arguments: Parsed command line arguments
    """
    data_paths = validate_csv_paths(arguments)
    incidents_data, hierarchy_data = get_processed_dataset(data_paths)

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

        transformed_X_test = model[0].transform(x_test)

        actual_accuracy = metrics[ACCURACY_METRIC][target]
        logger.info("*** {} Initial Accuracy : {} ***".format(target, round(actual_accuracy * 100, 2)))

        for k in [1, 2, 3, 10]:
            logger.info("Method: Normal Group DFS rankings based on threshold")
            starttime = time.time()
            accuracy = model[1].predict_and_evaluate_ranking(transformed_X_test, y_test.values,
                                                             threshold=GDFS_THRESHOLD, k=k)
            total_inference_time = time.time() - starttime
            logger.info(f"Accuracy: {round(accuracy * 100, 2)}% & Time taken: {total_inference_time}s at K: {k}")

            logger.info("Method: Only DFS based rankings")
            starttime = time.time()
            accuracy = model[1].predict_and_evaluate_ranking(transformed_X_test, y_test.values,
                                                             threshold=-99, k=k)
            total_inference_time = time.time() - starttime
            logger.info(f"Accuracy: {round(accuracy * 100, 2)}% & Time taken: {total_inference_time}s at K: {k}")


if __name__ == '__main__':
    args = build_argparser().parse_args()
    delattr(args, 'output')  # currently, we are not outputting anything.
    main(args)
