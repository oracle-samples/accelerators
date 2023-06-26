################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Mon Jun 26 10:43:20 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: d1aa4c4108819e3df1f43b87333a4a5e20612e48 $
################################################################################################
#  File: conftest.py
################################################################################################
# This module adds PyTest configuration options.
from ai4service_automated_classification.tests.fixtures.data import incidents_data, hierarchy_data, dummy_data  # noqa: F401
from ai4service_automated_classification.tests.fixtures.data import temp_uri_with_dummy_data  # noqa: F401
from ai4service_automated_classification.tests.fixtures.data import data_request_payload  # noqa: F401