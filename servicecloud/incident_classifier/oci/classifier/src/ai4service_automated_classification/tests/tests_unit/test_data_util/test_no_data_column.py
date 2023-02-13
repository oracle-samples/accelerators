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
#  SHA1: $Id: a15bf24cd7a2bc87857cbc88654cae9cc847669f $
################################################################################################
#  File: test_no_data_column.py
################################################################################################

from ai4service_automated_classification.constants import DATA_COLUMN, PRODUCT_ID_COLUMN
from ai4service_automated_classification.ml.util.data_util import \
    preprocess_incidents


def test_no_data_column(incidents_no_data_column):
    incidents_data = incidents_no_data_column
    assert DATA_COLUMN not in incidents_data.columns
    processed_incidents_data = preprocess_incidents(incidents_data)
    product_incidents_data = processed_incidents_data[PRODUCT_ID_COLUMN]
    assert DATA_COLUMN in product_incidents_data.columns
    assert product_incidents_data.shape[0] == 6
