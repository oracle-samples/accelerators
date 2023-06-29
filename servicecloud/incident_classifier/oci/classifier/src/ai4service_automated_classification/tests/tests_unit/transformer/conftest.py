################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Mon Jun 26 10:43:23 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: f5cb7e2bb95e02b95014eb40318de44e34899449 $
################################################################################################
#  File: conftest.py
################################################################################################
from ai4service_automated_classification.tests.fixtures.data import incidents_data, hierarchy_data, dummy_data  # NOQA: F401
from ai4service_automated_classification.tests.fixtures.data_test_transformer import transformed_data_array  # NOQA: F401
from ai4service_automated_classification.tests.fixtures.data_test_transformer import transformed_data_array_topNzero  # NOQA: F401
from ai4service_automated_classification.tests.fixtures.data_test_transformer import corpus  # NOQA: F401
