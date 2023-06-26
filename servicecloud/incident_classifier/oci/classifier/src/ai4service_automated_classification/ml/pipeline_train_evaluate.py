################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Mon Jun 26 10:43:19 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: a34e4333e5cffe2204bca199037f5c8871c42ad4 $
################################################################################################
#  File: pipeline_train_evaluate.py
################################################################################################
"""Defines the pipeline and training flow used in the Athena app."""
import logging
import os
from typing import Dict, List, Iterable, Union, Sequence

import numpy as np
from numpy import ndarray, arange
from pandas import DataFrame
from sklearn.compose import ColumnTransformer
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.linear_model import LogisticRegression
from sklearn.metrics import accuracy_score
from sklearn.pipeline import Pipeline

from ai4service_automated_classification.constants import TRANSFORMER_TOP_N_WORDS, \
    SVD_COMPONENTS, DATE_COLUMN, DATA_COLUMN, TEST_RATIO, ACCURACY_METRIC, PRUNE_MIN_COUNT
from ai4service_automated_classification.ml.DummyModel import DummyModel
from ai4service_automated_classification.ml.InitialIDEncoder import InitialIDEncoder
from ai4service_automated_classification.ml.ProductTreeClassifier import ProductTreeClassifier
from ai4service_automated_classification.ml.SafeTruncatedSVD import SafeTruncatedSVD
from ai4service_automated_classification.ml.Transformer import Transformer, default_trans_params
from ai4service_automated_classification.ml.model_selection import TemporalStochasticSplit, \
    TemporalStratifiedSplit
from ai4service_automated_classification.ml.util.classifier_util import default_params
from ai4service_automated_classification.ml.util.data_util import build_filter_dict, prune_out_min_count

logger = logging.getLogger(os.path.basename(__file__))


def get_pipeline(hierarchy_data: DataFrame,
                 incidents_data: DataFrame,
                 initial_id_column: Sequence[str],
                 target_column: str) -> Pipeline:
    """
    Generate a classifier pipeline.

    This classifier pipeline predicts the appropriate label from the training set.

    @type hierarchy_data: DataFrame
    @param hierarchy_data: DataFrame with the Hierarchy Data
    @type incidents_data: DataFrame
    @param incidents_data: DataFrame with the Incidents Data
    @type initial_id_column: str
    @param initial_id_column: Initial Feature Column
    @type target_column: str
    @param target_column: target column for which we need to build the templates
    @rtype Pipeline
    @return a ScikitLearn Pipeline object
    """
    # TF-IDF Transformer
    my_vectorizer = TfidfVectorizer(**default_trans_params)
    subject_text_transformer = Transformer(top_n=TRANSFORMER_TOP_N_WORDS, transformer=my_vectorizer)

    # Initial Id Transformer
    initial_product_encoder = InitialIDEncoder()
    id_transformer = Pipeline(steps=[('one hot encode', initial_product_encoder),
                                     ('SVD', SafeTruncatedSVD(n_components=SVD_COMPONENTS))])

    # Column Transformer
    column_transformer = ColumnTransformer([  # Name of the Column Transformer; Transformer; Column Name/Index
        ('Subject + Text Transformer', subject_text_transformer, DATA_COLUMN),
        ('Initial Id Transformer', id_transformer, initial_id_column)
    ])

    # Product Tree Classifier
    filter_dict = build_filter_dict(incidents_data, hierarchy_data, target_column)
    tree_classifier = ProductTreeClassifier(hierarchy_data,
                                            classifier=LogisticRegression(**default_params),
                                            transformer=None,
                                            grid_search={'C': [0.001, 0.01, 0.1, 1, 2, 10]},
                                            grid_search_cv=TemporalStochasticSplit(),
                                            grid_search_n_jobs=5,
                                            filter_dict=filter_dict)

    return Pipeline(steps=[
        ('transformer', column_transformer),
        ('classifier', tree_classifier)
    ])


def split_train_evaluate(incidents_data: DataFrame,
                         pipeline: Pipeline,
                         feature_columns: Union[List[str], Iterable[str]],
                         target_column: str,
                         test_ratio: float = TEST_RATIO) -> tuple:
    """Split data, training_scripts a classifier, and evaluate it.

    1. Expects preprocessed data
    2. Split data
    3. Train templates
    4. Evaluate templates

    @type incidents_data: DataFrame
    @param incidents_data: data bucket containing the data.
    @type pipeline: Pipeline
    @param pipeline: SciKit Learn Pipeline
    @type feature_columns: List[Str]
    @param feature_columns: Columns to training_scripts on
    @param target_column: Target Column
    @param test_ratio: the test size ratio from the total no of incidents.
    @return: tuple of trained templates, templates metrics and test set indices.
    """
    test_size = int(round(test_ratio * len(incidents_data)))
    if DATE_COLUMN in incidents_data:
        logger.info("Splitting data by latest incidents in order.")
        # if we have the date, better create a test set with the latest incidents
        idx = arange(len(incidents_data))
        train_idx = idx[:-test_size]
        test_idx = idx[-test_size:]
    else:
        # if don't have an explicit date, preserve the order of the incidents
        # and apply a stratified split
        logger.info("Splitting data using a stratified approach.")
        splitter = TemporalStratifiedSplit(test_ratio=test_ratio)
        try:
            train_idx, test_idx = next(splitter.split(X=None, y=incidents_data[target_column].values))
        except StopIteration as e:
            logger.warning(f"Could not split dataset for training/testing for {target_column}. "
                           f"Dataset size: {len(incidents_data[target_column])}. {e}")
            return DummyModel(), {ACCURACY_METRIC: {}}, []

    logger.info(f"Training Data Size for {target_column}: {np.count_nonzero(train_idx)}")
    logger.info(f"Testing Data Size for {target_column}: {np.count_nonzero(test_idx)}")

    # Select training samples
    training_data = incidents_data.iloc[train_idx]

    # Count values before pruning
    counts = training_data[target_column].value_counts()
    logger.debug(f"Number of samples per class before pruning: \n{counts}")

    # Prune classes with less than PRUNE_MIN_COUNT samples
    training_data = prune_out_min_count(training_data, PRUNE_MIN_COUNT, target_column)

    # Count values after pruning
    counts = training_data[target_column].value_counts()
    logger.debug(f"Number of samples per class after pruning: \n{counts}")

    # Check if there are minimum 2 classes and if there are enough data samples per class
    enough_data_per_class = (counts >= PRUNE_MIN_COUNT).all(0)
    if len(counts) >= 2 and enough_data_per_class:
        x_train = training_data[feature_columns]
        y_train = training_data[target_column]

        x_test = incidents_data[feature_columns].iloc[test_idx]
        y_test = incidents_data[target_column].iloc[test_idx]

        logger.info(f"Training the '{target_column.upper()}' templates...")
        fitted_model = pipeline.fit(x_train, y_train)

        model_metrics = evaluate(fitted_model, x_test, y_test, feature_columns, target_column)
        logger.info(f"{target_column} Model Metrics: {model_metrics}")
    else:
        logger.info("Not enough data to training_scripts a Model. Using DummyModel that always returns 0.")
        fitted_model = DummyModel()
        model_metrics = {ACCURACY_METRIC: {}}
        test_idx = []

    return fitted_model, model_metrics, test_idx


def evaluate(fitted_model: Pipeline,
             x_test: DataFrame,
             y_test: ndarray,
             feature_columns: Union[List[str], Iterable[str]],
             target_column: str) -> Dict[str, Dict]:
    """
    Calculate the templates performance - F1Metric, Recall and ROCAUC as examples.

    @param fitted_model: The sklearn-like templates that is already fitted.
    @param x_test: Test dataset's input features.
    @param y_test: Test dataset's labels.
    @param feature_columns: the feature columns to evaluate
    @param target_column: the target column to evaluate

    @rtype Dict[str, Dict]
    @return: Calculated templates scores as a dictionary, where the key is the target
    name and value is the scores.

    """
    y_pred = fitted_model.predict(x_test[feature_columns])
    evaluation_scores = {
        ACCURACY_METRIC: {
            target_column: accuracy_score(y_test, y_pred)
        }
    }
    return evaluation_scores
