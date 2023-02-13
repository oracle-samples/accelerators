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
#  SHA1: $Id: 9d0e890bed58bc03a16945ba296a8664d96cf207 $
################################################################################################
#  File: DummyModel.py
################################################################################################
"""Dummy Model to return prediction 0 and confidenceScore 0 for no/low data case."""
from scipy.sparse import csr_matrix
from sklearn.base import ClassifierMixin, BaseEstimator
from pandas import DataFrame


class DummyModel(BaseEstimator, ClassifierMixin):
    """Model returns default response {prediction: 0, confidenceScore: 0} when there is no training data."""

    def fit(self, x: csr_matrix, y: DataFrame) -> 'DummyModel':
        """
        Return same templates because the real templates had no data to training_scripts on.

        @param x: Training data
        @param y: Training labels
        @return: DummyModel
        """
        return self

    def predict(self, x: DataFrame) -> list:
        """
        Return default response 0 because the real templates had no data to training_scripts on.

        @type x: DataFrame
        @param x: input
        @return: list
        """
        return [0] * len(x)

    def predict_proba(self, x: DataFrame) -> dict:
        """
        Return 0 confidence and 0 prediction, because the real templates had no data to training_scripts on.

        Real templates could not training_scripts on no/few data points.
        @type x: DataFrame
        @param x: input
        @rtype: dict
        @return: dictionary with 0 confidenceScore and prediction 0
        """
        return {
            "confidenceScore": 0,
            "prediction": 0
        }
