################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Mon Jun 26 10:43:25 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: fe8af1dcf9dea3e046b46ad56742b239e4b3e3b5 $
################################################################################################
#  File: data_fixtures.py
################################################################################################
import pytest

from ai4service_automated_classification.constants import PRODUCT_COLUMN, INQUIRY_COLUMN, CATEGORY_COLUMN, \
    DISPOSITION_COLUMN


@pytest.fixture(scope="session")
def data_request_payload():
    yield {
        "jsonData": {
            INQUIRY_COLUMN: "Sample Inbound email subject",
            PRODUCT_COLUMN: 0,
            CATEGORY_COLUMN: 0,
            DISPOSITION_COLUMN: 0
        }
    }


@pytest.fixture(scope="session")
def data_request_stop_word_payload():
    yield {
        "jsonData": {
            INQUIRY_COLUMN: "Why",
            PRODUCT_COLUMN: 0,
            CATEGORY_COLUMN: 0,
            DISPOSITION_COLUMN: 0
        }
    }


@pytest.fixture(scope="session")
def data_request_empty_payload():
    yield {
        "jsonData": {
            INQUIRY_COLUMN: "",
            PRODUCT_COLUMN: 0,
            CATEGORY_COLUMN: 0,
            DISPOSITION_COLUMN: 0
        }
    }


@pytest.fixture(scope="session")
def data_request_null_payload():
    yield {
        "jsonData": {
            INQUIRY_COLUMN: "Sample Inbound email subject",
            PRODUCT_COLUMN: None,
            CATEGORY_COLUMN: None,
            DISPOSITION_COLUMN: None
        }
    }


@pytest.fixture(scope="session")
def data_request_null_inquiry_payload():
    yield {
        "jsonData": {
            INQUIRY_COLUMN: None,
            PRODUCT_COLUMN: None,
            CATEGORY_COLUMN: None,
            DISPOSITION_COLUMN: None
        }
    }
