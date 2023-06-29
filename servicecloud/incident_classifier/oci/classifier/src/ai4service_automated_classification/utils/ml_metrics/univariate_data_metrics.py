################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Mon Jun 26 10:43:24 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: f4b7a8b30646fafbde5ca2aca211cd3086e59a08 $
################################################################################################
#  File: univariate_data_metrics.py
################################################################################################
"""Metrics for univariate data features."""
import warnings
from typing import List, Union, Optional

import numpy as np
import pandas as pd

from ai4service_automated_classification.utils.ml_metrics.metrics import UnivariateDataMetric

MissingOptionsType = Optional[List[Union[float, int, bool, str]]]


def data_count(data: pd.Series) -> float:
    """Count the total number of data points.

    :param data: A univariate feature.
    :return: The total number of data points.
    """
    return data.shape[0]


def sparsity(data: pd.Series, treat_as_missing: MissingOptionsType = None) -> float:
    """Calculate the sparsity defined as the fraction of missing values.

    :param data: A univariate feature.
    :param treat_as_missing: A list of values that should be treated as missing for the purposes
        of calculating sparsity. By default this includes empty strings.
    :return: The fraction of missing values in the data.
    """
    data = data.copy(deep=True)
    if treat_as_missing is None:
        treat_as_missing = [""]
    for value in treat_as_missing:
        data[data == value] = np.nan
    n_total = data.shape[0]
    n_missing = data.isna().sum()
    sparsity_metric = n_missing / n_total
    return sparsity_metric


def cardinality(data: pd.Series, dropna: bool = True) -> float:
    """Calculate the set cardinality, i.e. the number of unique values, of the data.

    :param data: A univariate feature.
    :param dropna: Don’t include NaN in the calculations.
    :return: The set cardinality of the feature.
    """
    return data.nunique(dropna=dropna)


class DataCountMetric(UnivariateDataMetric):
    """A measure of the number of data points in a feature."""

    warnings.warn("This is now deprecated and will be removed in a future version. "
                  "Please use data_count function from this module instead.",
                  DeprecationWarning, stacklevel=2)

    def __init__(self) -> None:
        """Initialise DataCountMetric."""
        super().__init__()

    # noinspection PyMethodMayBeStatic
    def _calc_metric(self, data: pd.Series) -> float:
        """
        Count the total number of data points.

        :param data: A univariate feature.
        :return: The total number of data points.
        """
        return data.shape[0]


class SparsityMetric(UnivariateDataMetric):
    """A measure of the sparsity of a feature."""

    warnings.warn("This is now deprecated and will be removed in a future version. "
                  "Please use sparsity function from this module instead.",
                  DeprecationWarning, stacklevel=2)

    def __init__(self, treat_as_missing: MissingOptionsType = None) -> None:
        """
        Initialise a SparsityMetric object.

        :param treat_as_missing: A list of values that should be treated as missing for
        the purposes of calculating sparsity.
        """
        super().__init__()
        self.treat_as_missing = treat_as_missing

    # noinspection PyMethodMayBeStatic
    def _calc_metric(self, data: pd.Series) -> float:
        """
        Calculate the sparsity defined as the fraction of missing values.

        :param data: A univariate feature.
        :return: The fraction of missing values in the data.
        """
        data = data.copy(deep=True)
        data[data == ""] = np.nan
        if self.treat_as_missing:
            for value in self.treat_as_missing:
                data[data == value] = np.nan
        n_total = data.shape[0]
        n_missing = data.isna().sum()
        sparsity = n_missing / n_total
        return sparsity


class CardinalityMetric(UnivariateDataMetric):
    """
    A measure of set cardinality.

    This metric is intended for categorical features. Note that missing data are not counted
    towards the cardinality. If you want to count missing data as a level, replace them with a
    suitable, non-null representation e.g. 'UNKNOWN'.
    """

    warnings.warn("This is now deprecated and will be removed in a future version. "
                  "Please use cardinality function from this module instead.",
                  DeprecationWarning, stacklevel=2)

    def __init__(self, dropna: bool = True) -> None:
        """
        Initialise a CardinalityMetric object.

        :param dropna: Don’t include NaN in the calculations.
        """
        super().__init__()
        self.dropna = dropna

    # noinspection PyMethodMayBeStatic
    def _calc_metric(self, data: pd.Series) -> float:
        """
        Calculate the set cardinality of the data.

        This is the number of unique values present in the data.
        :param data: A univariate feature.
        :return: The set cardinality of the feature.
        """
        return data.nunique(dropna=self.dropna)
