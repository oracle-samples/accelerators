################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Mon Jun 26 10:43:18 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: d0ab9583504fd62218917a30f4c51b7c4878d407 $
################################################################################################
#  File: MultiClassifier.py
################################################################################################
"""Extension of the MultiTailClassifier to support already transformed inputs (csr matrix)."""
import logging
import os
from typing import Iterable, Union, Dict, NamedTuple, Sequence, Any

from pandas import DataFrame
from ai4service_automated_classification.ml.multi_tail_classifier import MultiTailClassifier, NamedClassifier
from scipy.sparse import csr_matrix
from sklearn.base import BaseEstimator
from sklearn.pipeline import Pipeline

logger = logging.getLogger(os.path.basename(__file__))


class AutoClassifNamedClassifier(NamedTuple):
    """Class override for DSDK NamedClassifier."""

    classifier: Union[BaseEstimator, Pipeline]
    target_name: Union[str, Sequence[str]]
    response_key: str
    prune_min_count: int


class MultiClassifier(MultiTailClassifier):
    """MultiTailClassifier extension for Autoclassif Product, Category and Disposition."""

    def __init__(self, named_classifiers: Iterable[Union[NamedClassifier, AutoClassifNamedClassifier]]) -> None:
        """
        Build a MultiTailClassifier using a collection of classifiers.

        @param named_classifiers: An iterable of NamedClassifier objects, where the 'classifier'
                                  argument is the classifier to be used to predict the column specified
                                  in 'target_name'.
        """
        super().__init__(named_classifiers)  # type: ignore

    def fit(self, x: csr_matrix, y: DataFrame) -> 'MultiClassifier':
        """
        Fit the classifiers to the data provided.

        Caution: if NamedClassifier set has the field fit_none_targets=False,
        the fit will skip the rows where target is None for this NamedClassifier.
        x and y of the fitted data frames must be aligned in this case (have the same index).

        @param x: Features DataFrame.
        @param y: Targets DataFrame.
        @return: The trained templates.
        """
        # Bypass the fit, because we fit and training_scripts before we initialize the classifier
        return self

    def predict(self, x: DataFrame) -> Dict[str, Any]:
        """
        Predict the labels for the provided data.

        @param x: The features to predict from.
        @return: A dictionary with the target columns as keys and the prediction series as values
                 for each target.
        """
        multi_target_result = dict()
        named_classifier: AutoClassifNamedClassifier
        for named_classifier in self.named_classifiers:  # type: ignore
            predictions = named_classifier.classifier.predict(x)
            multi_target_result[named_classifier.response_key] = predictions
        return multi_target_result

    def predict_proba(self, x: DataFrame) -> Dict[str, Any]:
        """
        Predict the probabilities for all labels for each target.

        @param x: The features to predict from.
        @return: A dictionary with the target columns as keys and the matrix of probabilities
                 as the values.
        """
        multi_target_proba_result = dict()
        named_classifier: AutoClassifNamedClassifier
        for named_classifier in self.named_classifiers:  # type: ignore
            probabilities = named_classifier.classifier.predict_proba(x)
            multi_target_proba_result[named_classifier.response_key] = probabilities
        return multi_target_proba_result
