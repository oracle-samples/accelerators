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
#  SHA1: $Id: bb1dd451f19de906da9dd8424298b4689a36c202 $
################################################################################################
#  File: data_test_transformer.py
################################################################################################
import pkg_resources
import pytest
import numpy as np


@pytest.fixture(scope="function")
def corpus():
    return [
        'troubleshoot and fix script failures',
        'Rolling back to Multi Datasource - For tracking InfoManger and Rest API application side.',
        'CI Failure: Auto Filter - As a report consumer I can see that the auto filter is not retained.',
        'Rolling back to Multi Datasource - For tracking InfoManger and Rest API application side. '
        'I just reproduced the issue in Chrome from IM.',
        'Rolling back to Multi Datasource - For tracking InfoManger and Rest API application side. '
        'I created GNC ticket 191115-001557 to request the datasource configuration'
    ]


@pytest.fixture(scope="function")
def transformed_data_array():
    test_data_location = pkg_resources.resource_filename('tests', 'resources')
    data = np.loadtxt(f'{test_data_location}/transformed_data_array.csv', delimiter=",")
    return data


@pytest.fixture(scope="function")
def transformed_data_array_topNzero():
    test_data_location = pkg_resources.resource_filename('tests', 'resources')
    data = np.loadtxt(f'{test_data_location}/transformed_data_array_topNzero.csv', delimiter=",")
    return data
