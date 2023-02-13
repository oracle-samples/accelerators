################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Tue Jan 31 13:02:46 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: ae04a25e272f257fab2174e97364511435065c10 $
################################################################################################
#  File: build_model.py
################################################################################################
from typing import Dict, Tuple

from pandas import DataFrame

from ai4service_automated_classification.constants import TEST_RATIO, BUILD_MODEL_PARAMS, ACCURACY_METRIC, \
    INITIAL_ID_COLUMN, FEATURE_COLUMNS, RESPONSE_KEY, PRUNE_MIN_COUNT
from ai4service_automated_classification.ml.DummyModel import DummyModel
from ai4service_automated_classification.ml.MultiClassifier import MultiClassifier, logger, \
    AutoClassifNamedClassifier
from ai4service_automated_classification.ml.pipeline_train_evaluate import get_pipeline, split_train_evaluate


def build_and_train(incidents: Dict[str, DataFrame],
                    hierarchy: Dict[str, DataFrame],
                    test_ratio: float = TEST_RATIO) -> Tuple[MultiClassifier, Dict[str, Dict]]:
    """
    Build and training_scripts the individual classifiers and compose the MultiClassifier.

    @param incidents: Dataframe with the incidents' data
    @param hierarchy: Dictionary with different hierarchies (product, category, disposition)
    @param test_ratio: Train-Test Split ratio
    @rtype Tuple
    @return: Tuple consisting of the MultiClassifier and the templates metrics
    """
    params = BUILD_MODEL_PARAMS
    classifiers = []
    model_metrics: Dict[str, Dict] = {ACCURACY_METRIC: {}}
    for key, param in params.items():
        # If we have at least two nodes in the hierarchy, it makes sense to consider training
        if len(hierarchy[key]) > 1:
            pipeline = get_pipeline(hierarchy[key], incidents[key], param[INITIAL_ID_COLUMN], key)
            model, metrics, _ = split_train_evaluate(incidents_data=incidents[key],
                                                     pipeline=pipeline,
                                                     feature_columns=param[FEATURE_COLUMNS],
                                                     target_column=key,
                                                     test_ratio=test_ratio)
        else:
            model = DummyModel()
            metrics = {ACCURACY_METRIC: {}}
            logger.warning(f"Not enough hierarchy data for {key}. Using DummyModel.")
        classifiers.append(AutoClassifNamedClassifier(model, key, str(param[RESPONSE_KEY]), PRUNE_MIN_COUNT))
        for metric_key, value in model_metrics.items():
            value.update(metrics[metric_key])
    multitail = MultiClassifier(classifiers)
    return multitail, model_metrics
