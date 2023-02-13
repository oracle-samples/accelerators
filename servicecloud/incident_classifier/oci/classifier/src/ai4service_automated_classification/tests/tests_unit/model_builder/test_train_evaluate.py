################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Tue Jan 31 13:02:50 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: 4a388e9a03ce476c2cf5c2a3c7f31084791a25a8 $
################################################################################################
#  File: test_train_evaluate.py
################################################################################################
from unittest.mock import patch

import numpy as np
import pandas as pd
from pandas import DataFrame
from sklearn.pipeline import Pipeline
from sklearn.utils._testing import assert_almost_equal

from ai4service_automated_classification.constants import FEATURE_COLUMNS_TRAIN, \
    DATE_COLUMN, PRODUCT_ID_COLUMN, \
    TARGET_COLUMNS_TRAIN, INCIDENTS_COLUMNS, INITIAL_PRODUCT_ID_COLUMN, DATA_COLUMN, CATEGORY_ID_COLUMN, \
    DISPOSITION_ID_COLUMN, ACCURACY_METRIC, PRODUCT_FEATURE_COLUMNS, DISPOSITION_COLUMN, CATEGORY_COLUMN, \
    BUILD_MODEL_PARAMS, INITIAL_ID_COLUMN
from ai4service_automated_classification.ml.DummyModel import DummyModel
from ai4service_automated_classification.ml.MultiClassifier import MultiClassifier, AutoClassifNamedClassifier
from ai4service_automated_classification.ml.build_model import build_and_train
from ai4service_automated_classification.ml.pipeline_train_evaluate import get_pipeline, \
    split_train_evaluate
from ai4service_automated_classification.ml.util.data_util import preprocess_incidents, preprocess_hierarchy


def test_returned_obj_type(dummy_data):
    """Test if get_pipeline function returns a scikit-learn Pipeline object."""
    incident_data, hierarchy_data = dummy_data
    instance = get_pipeline(hierarchy_data[PRODUCT_ID_COLUMN], incident_data,
                            INITIAL_PRODUCT_ID_COLUMN, PRODUCT_ID_COLUMN)
    assert isinstance(instance, Pipeline)


def test_split_train_evaluate_data(dummy_hierarchy_PTC, dummy_data_PTC):
    initial_id_column = INITIAL_PRODUCT_ID_COLUMN
    target_column = PRODUCT_ID_COLUMN
    feature_columns = PRODUCT_FEATURE_COLUMNS
    dummy_data_PTC = preprocess_incidents(dummy_data_PTC)
    dummy_hierarchy_PTC[target_column] = preprocess_hierarchy(dummy_hierarchy_PTC[target_column])
    pipeline = get_pipeline(dummy_hierarchy_PTC[target_column], dummy_data_PTC[target_column],
                            initial_id_column, target_column)
    trained, metrics, test_idx = split_train_evaluate(dummy_data_PTC[target_column],
                                                      pipeline,
                                                      feature_columns,
                                                      target_column,
                                                      test_ratio=0.1)
    assert isinstance(trained, Pipeline)
    assert trained.steps[1][1].hierarchy.equals(dummy_hierarchy_PTC[PRODUCT_ID_COLUMN])
    assert isinstance(metrics, dict)
    assert_almost_equal(metrics[ACCURACY_METRIC][PRODUCT_ID_COLUMN], 0.5)
    assert np.array_equal(np.where(test_idx)[0], np.array([52, 53, 54, 57, 58, 59]))


def test_preprocess_data_no_date_col(dummy_hierarchy_PTC, dummy_data_PTC):
    with patch(
            'ai4service_automated_classification.ml.pipeline_train_evaluate.TemporalStratifiedSplit') as mock:
        try:
            _ = preprocess_incidents(dummy_data_PTC)
            _ = preprocess_hierarchy(dummy_hierarchy_PTC[PRODUCT_ID_COLUMN])
        except:  # NOQA: E722
            # when no DATE_COLUMN exists, the TemporalStratifiedSplit is called
            mock.assert_called()


def test_split_train_evaluate_with_date_col(dummy_hierarchy_PTC, dummy_data_PTC):
    initial_id_column = INITIAL_PRODUCT_ID_COLUMN
    target_column = PRODUCT_ID_COLUMN
    feature_columns = PRODUCT_FEATURE_COLUMNS
    test_ratio = 0.1
    incidents = dummy_data_PTC.copy()
    # generate some dates in reverse order
    incidents[DATE_COLUMN] = pd.to_datetime(np.arange(len(incidents), 0, -1), unit='D')
    most_recent = incidents[DATE_COLUMN].values[0]
    with patch(
            'ai4service_automated_classification.ml.pipeline_train_evaluate.TemporalStratifiedSplit') as mock:
        try:
            _ = preprocess_incidents(incidents)
            _ = preprocess_hierarchy(dummy_hierarchy_PTC)
        except:  # NOQA: E722
            # when DATE_COLUMN exists, the TemporalStratifiedSplit is not called
            mock.assert_not_called()
    # instead we sort the data and the indices will be the incidents at the end
    pipeline = get_pipeline(dummy_hierarchy_PTC[target_column], incidents, initial_id_column, target_column)
    trained, metrics, test_idx = split_train_evaluate(incidents, pipeline, feature_columns, target_column, test_ratio)
    # after calling the function the most recent should be at the end
    assert most_recent == incidents[DATE_COLUMN].values[-1]
    incidents_size = len(incidents)
    assert np.array_equal(test_idx, range(round(incidents_size * (1 - test_ratio)), incidents_size))
    assert isinstance(trained, Pipeline)
    assert isinstance(metrics, dict)


def test_split_train_evaluate(dummy_hierarchy_PTC, dummy_data_PTC):
    # Set up Feature and Target Columns
    initial_id_column = INITIAL_PRODUCT_ID_COLUMN
    feature_columns = [DATA_COLUMN, initial_id_column]
    target_column = PRODUCT_ID_COLUMN
    expected_metrics = {
        ACCURACY_METRIC:
            {
                PRODUCT_ID_COLUMN: 0.5833333333333334
            }
    }
    # Preprocess Data
    dummy_data_PTC = preprocess_incidents(dummy_data_PTC)
    dummy_hierarchy_PTC[target_column] = preprocess_hierarchy(dummy_hierarchy_PTC[target_column])
    # Get and evaluate the templates
    pipeline = get_pipeline(dummy_hierarchy_PTC[target_column], dummy_data_PTC[target_column],
                            initial_id_column, target_column)
    trained, metrics, test_idx = split_train_evaluate(dummy_data_PTC[target_column],
                                                      pipeline,
                                                      feature_columns,
                                                      target_column,
                                                      0.2)
    assert expected_metrics == metrics


def test_build_model(dummy_data_PTC, dummy_hierarchy_PTC):
    # Preprocess the Incidents Data, handle NaNs, prune min count etc
    all_data = preprocess_incidents(dummy_data_PTC)
    hierarchy_data = dummy_hierarchy_PTC
    expected_metrics = {
        ACCURACY_METRIC:
            {
                PRODUCT_ID_COLUMN: 0.5,
                CATEGORY_ID_COLUMN: 0.5,
                DISPOSITION_ID_COLUMN: 0.5
            }
    }

    model, metrics = build_and_train(all_data, hierarchy_data, test_ratio=0.1)
    assert MultiClassifier == model.__class__
    assert 3 == len(model.named_classifiers)
    for classifier in model.named_classifiers:
        assert AutoClassifNamedClassifier == classifier.__class__
    assert expected_metrics == metrics


def test_split_train_evaluate_test_ratio(dummy_hierarchy_PTC, dummy_data_PTC):
    # Set up Feature and Target Columns
    initial_id_column = INITIAL_PRODUCT_ID_COLUMN
    feature_columns = [DATA_COLUMN, initial_id_column]
    target_column = PRODUCT_ID_COLUMN
    dummy_data_PTC = preprocess_incidents(dummy_data_PTC)
    dummy_hierarchy_PTC[target_column] = preprocess_hierarchy(dummy_hierarchy_PTC[target_column])
    ratio = 0.2
    pipeline = get_pipeline(dummy_hierarchy_PTC[target_column], dummy_data_PTC[target_column],
                            initial_id_column, target_column)
    trained, metrics, test_idx = split_train_evaluate(dummy_data_PTC[target_column],
                                                      pipeline,
                                                      feature_columns,
                                                      target_column,
                                                      test_ratio=ratio)
    int_test_idxs = np.where(test_idx)[0]
    len_test_ids_ratio = round(len(dummy_data_PTC[target_column]) * ratio)
    assert len(int_test_idxs) == len_test_ids_ratio


def test_split_train_evaluate_test_ratio_with_date(dummy_hierarchy_PTC, dummy_data_PTC):
    # Set up Feature and Target Columns
    initial_id_column = INITIAL_PRODUCT_ID_COLUMN
    feature_columns = [DATA_COLUMN, initial_id_column]
    target_column = PRODUCT_ID_COLUMN
    incidents = dummy_data_PTC.copy()
    # generate some dates in reverse order
    ratio = 0.2
    incidents[DATE_COLUMN] = pd.to_datetime(np.arange(len(incidents), 0, -1), unit='D')
    incidents = preprocess_incidents(incidents)
    pipeline = get_pipeline(dummy_hierarchy_PTC[target_column], incidents[target_column],
                            initial_id_column, target_column)
    trained, metrics, test_idx = split_train_evaluate(incidents[target_column], pipeline,
                                                      feature_columns,
                                                      target_column,
                                                      test_ratio=ratio)
    len_test_data = int(round(len(incidents[target_column]) * ratio))
    assert len(test_idx) == len_test_data


def test_get_pipeline_predict_proba(dummy_data_PTC, dummy_hierarchy_PTC):
    # Set up Feature and Target Columns
    initial_id_column = INITIAL_PRODUCT_ID_COLUMN
    feature_columns = [DATA_COLUMN, initial_id_column]
    target_column = PRODUCT_ID_COLUMN
    incidents = preprocess_incidents(dummy_data_PTC)
    for key in TARGET_COLUMNS_TRAIN:
        dummy_hierarchy_PTC[key] = preprocess_hierarchy(dummy_hierarchy_PTC[key])
    pipeline = get_pipeline(dummy_hierarchy_PTC[target_column], incidents[target_column],
                            initial_id_column, target_column)
    trained, metrics, test_idx = split_train_evaluate(incidents[target_column],
                                                      pipeline,
                                                      feature_columns,
                                                      target_column,
                                                      test_ratio=0.5)
    df = pd.DataFrame([("Replication of Organization ignores shipping address While replicating an Organization as a"
                        " Sales Account, the shipping address is not considered if Billing Address is not present",
                        "Product Name", 1, 0, 1, 0, 1, 0)], columns=INCIDENTS_COLUMNS)
    result = trained.predict_proba(df[FEATURE_COLUMNS_TRAIN])
    assert type(result) == dict
    assert len(result.keys()) == 2
    for key in result.keys():
        assert key in ["prediction", "confidenceScore"]


def test_build_and_train_class_0(disposition_class_0):
    incidents, hierarchies = disposition_class_0
    input = DataFrame(data=[["Text me here on my mobile", 0, 0, 0]], columns=FEATURE_COLUMNS_TRAIN)
    expected = {'prediction': 0, 'confidenceScore': 0}
    ratio = 0.1
    model, metrics = build_and_train(incidents, hierarchies, ratio)
    assert isinstance(model, MultiClassifier)
    assert model.named_classifiers[2].target_name == DISPOSITION_ID_COLUMN
    assert isinstance(model.named_classifiers[2].classifier, DummyModel)
    assert len(metrics[ACCURACY_METRIC]) == 2
    prediction = model.predict_proba(input)
    assert isinstance(prediction, dict)
    assert prediction[DISPOSITION_COLUMN] == expected


def test_build_and_train_no_disposition(no_disposition_data):
    incidents, hierarchies = no_disposition_data
    input = DataFrame(data=[["Text me here on my mobile", 0, 0, 0]], columns=FEATURE_COLUMNS_TRAIN)
    expected = {'prediction': 0, 'confidenceScore': 0}
    ratio = 0.1
    model, metrics = build_and_train(incidents, hierarchies, ratio)
    assert isinstance(model, MultiClassifier)
    assert model.named_classifiers[2].target_name == DISPOSITION_ID_COLUMN
    assert isinstance(model.named_classifiers[2].classifier, DummyModel)
    assert len(metrics[ACCURACY_METRIC]) == 2
    prediction = model.predict_proba(input)
    assert isinstance(prediction, dict)
    assert prediction[DISPOSITION_COLUMN] == expected


def test_build_and_train_no_category_disposition(no_category_disposition_data):
    incidents, hierarchies = no_category_disposition_data
    input = DataFrame(data=[["Text me here on my mobile", 0, 0, 0]], columns=FEATURE_COLUMNS_TRAIN)
    expected = {'prediction': 0, 'confidenceScore': 0}
    ratio = 0.1
    model, metrics = build_and_train(incidents, hierarchies, ratio)
    assert isinstance(model, MultiClassifier)
    assert model.named_classifiers[1].target_name == CATEGORY_ID_COLUMN
    assert model.named_classifiers[2].target_name == DISPOSITION_ID_COLUMN
    assert isinstance(model.named_classifiers[1].classifier, DummyModel)
    assert isinstance(model.named_classifiers[2].classifier, DummyModel)
    assert len(metrics[ACCURACY_METRIC]) == 1
    prediction = model.predict_proba(input)
    assert isinstance(prediction, dict)
    assert prediction[CATEGORY_COLUMN] == expected
    assert prediction[DISPOSITION_COLUMN] == expected


def test_build_and_train_no_data(no_data):
    incidents, hierarchies = no_data
    input = DataFrame(data=[["Text me here on my mobile", 0, 0, 0]], columns=FEATURE_COLUMNS_TRAIN)
    expected = {'prediction': 0, 'confidenceScore': 0}
    ratio = 0.2
    model, metrics = build_and_train(incidents, hierarchies, ratio)
    assert isinstance(model, MultiClassifier)
    targets = [PRODUCT_ID_COLUMN, CATEGORY_ID_COLUMN, DISPOSITION_ID_COLUMN]
    for i, classifier in enumerate(model.named_classifiers):
        assert classifier.target_name == targets[i]
        assert isinstance(classifier.classifier, DummyModel)
    assert len(metrics[ACCURACY_METRIC]) == 0
    prediction = model.predict_proba(input)
    assert isinstance(prediction, dict)
    for key in prediction:
        assert prediction[key] == expected


def test_build_and_train_empty_hierarchy(processed_incidents_empty_hierarchy):
    incidents, hierarchies = processed_incidents_empty_hierarchy
    ratio = 0.2
    model, metrics = build_and_train(incidents, hierarchies, ratio)
    assert isinstance(model, MultiClassifier)
    targets = [PRODUCT_ID_COLUMN, CATEGORY_ID_COLUMN, DISPOSITION_ID_COLUMN]
    for i, classifier in enumerate(model.named_classifiers):
        assert classifier.target_name == targets[i]
        assert isinstance(classifier.classifier, DummyModel)
    assert len(metrics[ACCURACY_METRIC]) == 0
