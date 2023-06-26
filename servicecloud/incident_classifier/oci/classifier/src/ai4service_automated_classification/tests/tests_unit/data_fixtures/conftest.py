################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Mon Jun 26 10:43:21 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: 7ab12df14a9e4de114b670070f52f8dfdca329cb $
################################################################################################
#  File: conftest.py
################################################################################################
from ai4service_automated_classification.tests.fixtures.data_test_PTC import data_most_frequent_label_0, dummy_data_PTC, text_labels_PTC  # NOQA: F401
from ai4service_automated_classification.tests.fixtures.data_test_PTC import train_test_split_fixture, transformer_fixture  # NOQA: F401
from ai4service_automated_classification.tests.fixtures.data_test_PTC import filter_dict_PTC  # NOQA: F401
from ai4service_automated_classification.tests.fixtures.data_test_PTC import dummy_hierarchy_PTC, processed_dummy_data_PTC  # NOQA: F401
from ai4service_automated_classification.tests.fixtures.data_test_PTC import processed_dummy_hierarchy_PTC  # NOQA: F401
