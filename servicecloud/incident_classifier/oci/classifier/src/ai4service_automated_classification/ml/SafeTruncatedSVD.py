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
#  SHA1: $Id: 9dedb032f1c09dfbaba55b20f8e05044a8d1c44c $
################################################################################################
#  File: SafeTruncatedSVD.py
################################################################################################
"""This implements a safe version of Truncated SVD which does not fail on a small data sets."""

from sklearn.base import BaseEstimator, TransformerMixin
from sklearn.decomposition import TruncatedSVD
import scipy
from typing import Optional
import pandas as pd


class SafeTruncatedSVD(BaseEstimator, TransformerMixin):
    """
    Implement TruncatedSVD that works on small data.

    If the n_components is smaller than number of features in fit, this becomes identity transformer.
    Otherwise, this becomes TruncatedSVD with n_components.
    """

    def __init__(self, n_components: int):
        """Initialize SafeTruncatedSVD with n_components parameter.

        :param n_components: Number of components for TruncatedSVD.
        """
        self.svd_transformer: Optional[TruncatedSVD] = None
        self.identity = False
        self.n_components = n_components

    def fit(self, x: scipy.sparse, y: Optional[pd.Series] = None) -> 'SafeTruncatedSVD':
        """
        Fit SafeTruncatedSVD and note whether at least n_components features were present.

        :param x: Features matrix.
        :param y: Targets.
        :return: self
        """
        if x.shape[1] <= self.n_components:
            self.identity = True
        else:
            self.svd_transformer = TruncatedSVD(n_components=self.n_components)
            self.svd_transformer.fit(x, y)
        return self

    def transform(self, x: scipy.sparse) -> scipy.sparse:
        """
        Perform dimensionality reduction if a large dataset was passed in fit or return identity matrix.

        :param x: Features matrix.
        :return: Matrix either the input or with reduced dimensionality with truncatedSVD
        """
        if self.identity:
            xt = x
        else:
            xt = self.svd_transformer.transform(x)  # type: ignore
        return xt
