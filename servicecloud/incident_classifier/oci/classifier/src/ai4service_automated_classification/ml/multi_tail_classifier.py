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
#  SHA1: $Id: 436f37b4904439fea221aa17795dd9f519ad008e $
################################################################################################
#  File: multi_tail_classifier.py
################################################################################################
"""Module which adds abstractions to have a separate templates per target within one estimator."""
import logging
from typing import NamedTuple, Dict, Iterable, Union, List

import numpy as np
import pandas as pd
from sklearn.base import BaseEstimator, ClassifierMixin


class NamedClassifier(NamedTuple):
    """
    An abstraction for a classifier with fit and transform methods.

    If fit_none_targets is True, then try to fit rows with None targets. This can
    cause issues with some classifiers. E.g. DecisionTreeClassifier will throw ValueError
    "Input contains NaN" if you try. Still, you might want to have this behavior if you
    don't expect None in targets.
    Otherwise, if False passed, skip the rows where target is None. Caution: x and y of
    the fitted data frames must be aligned (have the same index).
    """

    classifier: BaseEstimator
    target_name: str
    fit_none_targets: bool


class MultiTailClassifier(BaseEstimator, ClassifierMixin):
    """Allows to have a separate templates per target within one estimator."""

    def __init__(self, named_classifiers: Iterable[NamedClassifier]) -> None:
        """
        Build a MultiTailClassifier using a collection of classifiers.

        :param named_classifiers: An iterable of NamedClassifier objects, where the 'classifier'
            argument is the classifier to be used to predict the column specified
            in 'target_name'.
        """
        super().__init__()
        self.named_classifiers = named_classifiers
        self.classes_: Dict[str, Union[np.array, List[np.array]]] = {}

    @staticmethod
    def _report_dropped_rows(
            original_df: pd.DataFrame,
            new_df: pd.DataFrame, target_name: str) -> None:
        num_rows_dropped = original_df.shape[0] - new_df.shape[0]
        percentage_dropped = 100 * (num_rows_dropped / original_df.shape[0])
        if percentage_dropped > 0:
            logging.getLogger(__name__).warning(f"Target '{target_name}': "
                                                f"dropped {num_rows_dropped} rows "
                                                f"with empty target values "
                                                f"({percentage_dropped:.2f}%)")

    def fit(self, x: pd.DataFrame, y: pd.DataFrame) -> 'MultiTailClassifier':
        """
        Fit the classifiers to the data provided.

        Caution: if NamedClassifier set has the field fit_none_targets=False,
        the fit will skip the rows where target is None for this NamedClassifier.
        x and y of the fitted data frames must be aligned in this case (have the same index).

        :param x: Features DataFrame.
        :param y: Targets DataFrame.
        :return: The trained templates.
        """
        for named_classifier in list(self.named_classifiers):
            if named_classifier.fit_none_targets:
                targets = y[named_classifier.target_name]
                features = x
            else:
                targets = y[named_classifier.target_name].dropna()
                self._report_dropped_rows(original_df=y[named_classifier.target_name],
                                          new_df=targets,
                                          target_name=named_classifier.target_name)
                features = x.loc[targets.index]
            try:
                named_classifier.classifier.fit(features, targets)
                self.classes_[named_classifier.target_name] = named_classifier.classifier.classes_
            except ValueError as err:
                if "Input contains NaN" in str(err):
                    raise ValueError("Input contains NaN which are not supported by your "
                                     "NamedClassifier. If NaN values are in targets, "
                                     "this could be fixed by dropping these rows by initializing "
                                     "NamedClassifier with fit_none_targets=False", err)
                else:
                    raise err

        return self

    def predict(self, x: pd.DataFrame) -> Dict[str, np.ndarray]:
        """
        Predict the labels for the provided data.

        :param x: The features to predict from.
        :return: A dictionary with the target columns as keys and the prediction series as values
            for each target.
        """
        multi_target_result = dict()
        for named_classifier in list(self.named_classifiers):
            predictions = named_classifier.classifier.predict(x)
            multi_target_result[named_classifier.target_name] = predictions
        return multi_target_result

    def predict_proba(self, x: pd.DataFrame) -> Dict[str, np.ndarray]:
        """
        Predict the probabilities for all labels for each target.

        :param x: The features to predict from.
        :return: A dictionary with the target columns as keys and the matrix of probabilities
            as the values.
        """
        multi_target_proba_result = dict()
        for named_classifier in list(self.named_classifiers):
            target_name = named_classifier.target_name
            probabilities = named_classifier.classifier.predict_proba(x)
            multi_target_proba_result[target_name] = probabilities
        return multi_target_proba_result
