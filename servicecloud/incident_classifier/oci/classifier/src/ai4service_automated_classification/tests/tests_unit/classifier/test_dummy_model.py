################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Mon Jun 26 10:43:21 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: b932271b4aa7bf7d3fc29870b37a8ec0abb6b18d $
################################################################################################
#  File: test_dummy_model.py
################################################################################################
from pandas import DataFrame
from scipy.sparse import csr_matrix

from ai4service_automated_classification.ml.DummyModel import DummyModel


def test_dummy_model_fit_predict():
    model = DummyModel()
    model = model.fit(csr_matrix([[]]), DataFrame())
    response = model.predict(DataFrame([[]]))
    assert response == [0]


def test_dummy_model_fit_predict_proba():
    model = DummyModel()
    expected = {'prediction': 0, 'confidenceScore': 0}
    model = model.fit(csr_matrix([[]]), DataFrame())
    response = model.predict_proba(DataFrame())
    assert response == expected
