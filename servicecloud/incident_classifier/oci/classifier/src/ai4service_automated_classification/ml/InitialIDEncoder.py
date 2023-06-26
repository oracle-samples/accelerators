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
#  SHA1: $Id: 1f0175920d92819d39f4922f51792da9416a7d47 $
################################################################################################
#  File: InitialIDEncoder.py
################################################################################################
"""
Initial Product ID Encoder.

Wrapper over Scikit Learn OneHotEncoder for data preparation for fitting and transforming.
"""
import logging
import os
from typing import Any

from pandas import Series
from sklearn.preprocessing import OneHotEncoder
from numpy import ndarray

logger = logging.getLogger(os.path.basename(__file__))


class InitialIDEncoder:
    """Wrapper Class for the OneHotEncoder."""

    def __init__(self) -> None:
        """Construct Initial Product ID Encoder."""
        self.encoder = OneHotEncoder(handle_unknown='ignore')

    def fit(self, data: Series, labels: ndarray, **fit_params: Any) -> 'InitialIDEncoder':
        """
        Fit function for the OneHotEncoder Wrapper.

        @param data: Incidents Data with Initial Product ID
        @param labels: dummy parameter to comply with the Pipeline expected Transformer interface/contract.
        @param fit_params: dummy param to comply with the Pipeline expected Transformer interface/contract.
        @return: The fitted InitialIDEncoder
        """
        logger.debug("Fitting the OneHotEncoder...")
        self.encoder = self.encoder.fit(data.values.reshape(-1, 1))
        logger.debug("Done fitting the OneHotEncoder on the Product List.")
        return self

    def transform(self, data: Series) -> Any:
        """
        Transform function for the encoder.

        @param data: Incident Data with Initial Product ID
        @return: Transformed Incident Data
        """
        logger.debug("Doing transform with OneHotEncoder...")
        result = self.encoder.transform(data.values.reshape(-1, 1))
        logger.debug("Done transforming with OneHotEncoder.")
        return result
