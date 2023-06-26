################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Mon Jun 26 10:43:23 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: 181c1dfff2a7c4fe234316d72e66a6f1d44b0119 $
################################################################################################
#  File: metrics.py
################################################################################################
"""Abstract classes for templates metrics."""
import warnings

import numpy as np
import pandas as pd
from abc import ABC, abstractmethod
from typing import List, Dict, Union, TypeVar, Generic, Any

TrueLabelType = TypeVar('TrueLabelType')
PredictionType = TypeVar('PredictionType')
FeatureType = Union[pd.DataFrame, pd.Series, np.array]


warnings.warn("This is now deprecated and will be removed in a future version. See `dsdk-utils` "
              "for updated metrics setup.",
              DeprecationWarning, stacklevel=2)


class Metric(ABC):
    """Parent class for ModelMetric and DataMetric."""

    @abstractmethod
    def __init__(self, metric_type: str) -> None:
        """
        Initialise Metric with parameters.

        :param metric_type: The type of metric, it can be 'model_metric' and 'data_metric'.
        """
        self.metric_name = self.__class__.__name__
        self.metric_type = metric_type


class ModelMetric(Metric, Generic[TrueLabelType, PredictionType]):
    """Defines the general type of metric that is to be calculated."""

    @abstractmethod
    def __init__(self) -> None:
        """Initialise ModelMetric by setting metric_type as 'model_metric'."""
        super().__init__(metric_type='model_metric')

    @abstractmethod
    def calc_metric(self,
                    true_labels: List[TrueLabelType],
                    predicted_labels: List[PredictionType]) -> float:
        """
        Calculate the metric.

        :param true_labels: The true labels.
        :param predicted_labels: The predicted labels.
        :return: The metric evaluated on true_labels and predicted_labels.
        """


class ModelMetricCollection(Generic[TrueLabelType, PredictionType]):
    """Defines the metrics that should be returned."""

    def __init__(self, collections_name: str,
                 metrics: List[ModelMetric[TrueLabelType, PredictionType]]) -> None:
        """
        Initialise ModelMetricCollection with parameters.

        :param collections_name: Name of the collection.
        :param metrics: List of ModelMetric to be included in the collection.
        """
        self.collections_name = collections_name
        self.metrics = metrics

    def calc_metrics(self,
                     true_labels: List[TrueLabelType],
                     predicted_labels: List[PredictionType]
                     ) -> Dict[str, float]:
        """
        Calculate all the metrics in the collection.

        :param true_labels: The true labels.
        :param predicted_labels: The predicted labels.
        :return: A dictionary of metrics evaluated on true_labels and predicted_labels.
        """
        calculated_metrics = {metric.metric_name: metric.calc_metric(true_labels, predicted_labels)
                              for metric in self.metrics}
        return calculated_metrics

    def __eq__(self, o: object) -> bool:
        """Override the default implementation."""
        class_attributes = [att for att in self.__dict__.keys()]
        for att in class_attributes:
            if self.__dict__[att] != o.__dict__[att]:
                return False
        return True


class DimensionMismatchException(Exception):
    """Raise when the actuals and the predictions have differing cardinalities."""


def length_match_check(actuals: List[Any],
                       predictions: List[Any]) -> None:
    """
    Verify that actuals and predictions are the same size.

    :param actuals: The true labels.
    :param predictions: The predicted labels.
    :raises: DimensionMismatchException
    """
    if len(actuals) != len(predictions):
        raise DimensionMismatchException("Size of actuals and "
                                         "predictions does not match. "
                                         f"Actuals: {len(actuals)}. "
                                         f"Predictions: {len(predictions)}.")


class NoLabelException(Exception):
    """Raise when some samples in the actuals do not contain any label."""


class DataMetric(Metric):
    """Abstraction for any metrics that could be calculated on data."""

    @abstractmethod
    def __init__(self) -> None:
        """Initialise DataMetric."""
        super().__init__(metric_type='data_metric')

    @abstractmethod
    def calculate(self, data: FeatureType) -> float:
        """
        Calculate the metric.

        :param data: One or more data features.
        :return: The metric value.
        """


class DataMetricCollection:
    """A collection of data metrics."""

    def __init__(self, collections_name: str, metrics: List[DataMetric]) -> None:
        """
        Initialise DataMetricCollection with parameters.

        :param collections_name: Name of the collection.
        :param metrics: List of DataMetric to be included in the collection.
        """
        self.collections_name = collections_name
        self.metrics = metrics

    def calculate(self, data: FeatureType) -> Dict[str, float]:
        """
        Calculate all the metrics in the collection.

        :param data: One or more data features.
        :return: A dictionary of metrics evaluated on data.
        """
        calculated_metrics = {metric.metric_name: metric.calculate(data)
                              for metric in self.metrics}
        return calculated_metrics


class UnivariateDataMetric(DataMetric):
    """Abstraction for data metrics that apply to a single feature."""

    @abstractmethod
    def __init__(self) -> None:
        """Initialise UnivariateDataMetric."""
        super().__init__()

    @abstractmethod
    def _calc_metric(self, data: pd.Series) -> float:
        """
        Calculate the metric of a pandas Series.

        :param data: Data to calculate the metric on.
        :return: The metric value.
        """

    def calculate(self, data: FeatureType) -> float:
        """
        Calculate the metric.

        :param data: A univariate feature.
        :return: The metric value.
        """
        validate_shape_univariate(data)
        data_as_series = _as_series(data)
        return self._calc_metric(data_as_series)


class InvalidDataException(Exception):
    """Raise when the (feature) data have an invalid form."""


def validate_shape_univariate(data: FeatureType) -> None:
    """
    Validate that data has a single column.

    :param data: Data containing a single feature.
    :return: None
    :raises InvalidDataException
    """
    try:
        dims = data.shape
        ndims = len(dims)
    except AttributeError:
        raise TypeError(f'data must be a pandas Series, DataFrame or numpy array '
                        f'not {type(data)}.')

    if ndims not in [1, 2]:
        raise InvalidDataException(f'Univariate metrics require data with a single column but '
                                   f'{ndims} dimensions encountered.')

    try:
        ncols = dims[1]
        if ncols != 1:
            raise InvalidDataException(f'Univariate metrics require data with a single column but '
                                       f'received {ncols} columns instead.')
    except IndexError:
        pass


def _as_series(data: FeatureType) -> pd.Series:
    """
    Convert a univariate feature to a pandas series.

    This method does not verify the number of columns passed to it.

    :param data: A univariate feature.
    :return: The univariate feature cast as a pandas series.
    """
    try:
        series = pd.Series(data)
    except TypeError:  # Occurs if data is a pd.DataFrame
        series = pd.Series(data.iloc[:, 0])
    return series
