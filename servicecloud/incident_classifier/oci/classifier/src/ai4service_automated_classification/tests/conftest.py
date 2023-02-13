################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Tue Jan 31 13:02:48 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: 5ea35396be573312b21efbe1b90167a5656ec5ae $
################################################################################################
#  File: conftest.py
################################################################################################
# This module adds PyTest configuration options.
from ai4service_automated_classification.tests.fixtures.data import incidents_data, hierarchy_data, dummy_data  # noqa: F401
from ai4service_automated_classification.tests.fixtures.data import temp_uri_with_dummy_data  # noqa: F401
from ai4service_automated_classification.tests.fixtures.data import data_request_payload  # noqa: F401