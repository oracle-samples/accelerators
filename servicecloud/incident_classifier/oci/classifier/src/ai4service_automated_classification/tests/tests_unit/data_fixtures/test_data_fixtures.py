################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Tue Jan 31 13:02:49 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: 7b8e46b4af2c55f1b6a1a44531cf9807783659e8 $
################################################################################################
#  File: test_data_fixtures.py
################################################################################################
from sklearn.utils._testing import assert_almost_equal


def test_filter_dict(filter_dict_PTC):
    filter_dict = filter_dict_PTC
    assert filter_dict == {1: 'Internal Systems', 2: 'Techmail'}


def test_labels(text_labels_PTC):
    _, labels = text_labels_PTC
    expected = [1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 2, 2,
                2, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2]
    assert labels.values.astype(int).tolist() == expected


def test_data(text_labels_PTC):
    text, _ = text_labels_PTC
    expected = 'Replication of Organization ignores shipping address While replicating an Organization as ' \
               'a Sales Account, ' \
               'the shipping address is not considered if Billing Address is not present'
    assert text.tolist()[0] == expected


def test_train_test_split(train_test_split_fixture):
    X_train, _, _, _ = train_test_split_fixture
    expected = 'Example with NoneNONE Target'
    assert X_train.tolist()[0] == expected


def test_transformed_data(transformer_fixture):
    _, X_testf = transformer_fixture
    expected = 0.25712475099970317
    assert_almost_equal(X_testf[0, 297], expected)
