################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Mon Jun 26 10:43:22 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: bfea4c9e41dd4566996270c9dd7cedf7a9b4085b $
################################################################################################
#  File: test_safe_truncated_svd.py
################################################################################################
import pytest
import numpy as np
from scipy.sparse import csr_matrix

from ai4service_automated_classification.ml.SafeTruncatedSVD import SafeTruncatedSVD


@pytest.fixture(scope="module")
def sparse_array():
    test_array = np.array(
        [[1, 1, 1, 0, 0, 0],
         [0, 0, 0, 1, 1, 1],
         [0, 1, 0, 1, 0, 1],
         [0, 1, 0, 1, 1, 0],
         [1, 1, 1, 1, 1, 0]])
    sparse_array = csr_matrix(test_array)
    return sparse_array


def test_safe_truncated_svd__n_components_under_ncol(sparse_array):
    stsvd_5 = SafeTruncatedSVD(n_components=5)
    stsvd_5.fit(x=sparse_array)
    xt_5 = stsvd_5.transform(x=sparse_array)
    assert xt_5.shape == (5, 5)


def test_safe_truncated_svd__n_components_equal_ncol(sparse_array):
    stsvd_6 = SafeTruncatedSVD(n_components=6)
    stsvd_6.fit(x=sparse_array)
    xt_6 = stsvd_6.transform(x=sparse_array).toarray()
    assert (xt_6 == sparse_array).all()


def test_safe_truncated_svd__n_components_over_ncol(sparse_array):
    stsvd_10 = SafeTruncatedSVD(n_components=10)
    stsvd_10.fit(x=sparse_array)
    xt_10 = stsvd_10.transform(x=sparse_array).toarray()
    assert (xt_10 == sparse_array).all()
