#!/usr/bin/env python3
################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Tue Jan 31 13:02:51 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: f1517bc0f27c9b93bc340953007087526521c94e $
################################################################################################
#  File: test_remove_html.py
################################################################################################

import pytest

from ai4service_automated_classification.constants import DATA_COLUMN, PRODUCT_ID_COLUMN, PRODUCT_COLUMN, \
    CATEGORY_COLUMN, DISPOSITION_COLUMN
from ai4service_automated_classification.ml.util.data_util import remove_html_tags_single, \
    preprocess_incidents, preprocess_hierarchy


def test_remove_html_tags_single():
    expected = 'This text is bold'
    assert remove_html_tags_single('<b>This text is bold</b>') == expected


def test_remove_html_tags_single_error():
    val = ['<b>This text is bold</b>', '<i>This text is italic</i>']
    with pytest.raises(AttributeError):
        remove_html_tags_single(val)


def test_preprocess_data_with_drop_html(dummy_data_contains_html):
    incidents_data, _ = dummy_data_contains_html
    processed_incidents_data = preprocess_incidents(incidents_data)
    processed_incidents_data = processed_incidents_data[PRODUCT_ID_COLUMN]
    assert "<b>" not in processed_incidents_data.iloc[0, 0]
    assert "<span>" not in processed_incidents_data.iloc[1, 0]
    assert "<div>" not in processed_incidents_data.iloc[2, 0]
    assert "<p>" not in processed_incidents_data.iloc[3, 0]
    assert 4 == len(processed_incidents_data)


def test_preprocess_data_without_drop_html(dummy_data_contains_html):
    incidents_data, hierarchy_data = dummy_data_contains_html
    processed_incidents_data = preprocess_incidents(incidents_data, remove_html=False)
    processed_incidents_data = processed_incidents_data[PRODUCT_ID_COLUMN]
    for index, row in processed_incidents_data.iterrows():
        assert incidents_data.iloc[index, 0] == row[DATA_COLUMN]


def test_preprocess_hierarchy_with_product(dummy_hierarchy_PTC):
    for key in dummy_hierarchy_PTC.keys():
        hierarchy = preprocess_hierarchy(dummy_hierarchy_PTC[key])
        for column in hierarchy.columns:
            assert column.islower()
            assert PRODUCT_COLUMN not in column
            assert CATEGORY_COLUMN not in column
            assert DISPOSITION_COLUMN not in column
