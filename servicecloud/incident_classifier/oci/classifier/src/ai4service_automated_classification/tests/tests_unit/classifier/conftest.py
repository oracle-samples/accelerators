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
#  SHA1: $Id: 6c555b810fd09c9d483a2db7e6105fd4543371ab $
################################################################################################
#  File: conftest.py
################################################################################################
from ai4service_automated_classification.tests.fixtures.data_test_PTC import data_most_frequent_label_0, text_labels_PTC, Node_PTC  # NOQA: F401
from ai4service_automated_classification.tests.fixtures.data_test_PTC import train_test_split_fixture, dummy_data_PTC, dummy_hierarchy_PTC  # NOQA: F401
from ai4service_automated_classification.tests.fixtures.data_test_PTC import transformer_fixture, dummy_data_PTC_extra_product  # NOQA: F401
from ai4service_automated_classification.tests.fixtures.data_test_PTC import processed_dummy_hierarchy_PTC  # NOQA: F401
