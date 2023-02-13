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
#  SHA1: $Id: ec077a35e27094db6211ffc371cd04578be9cc3c $
################################################################################################
#  File: conftest.py
################################################################################################
from ai4service_automated_classification.tests.fixtures.data import incidents_data, hierarchy_data, dummy_data  # NOQA: F401
from ai4service_automated_classification.tests.fixtures.data_test_transformer import transformed_data_array  # NOQA: F401
from ai4service_automated_classification.tests.fixtures.data_test_transformer import transformed_data_array_topNzero  # NOQA: F401
from ai4service_automated_classification.tests.fixtures.data_test_transformer import corpus  # NOQA: F401
