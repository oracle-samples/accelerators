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
#  SHA1: $Id: d431642ff02f1667f1f339d77034c9a1cee64b58 $
################################################################################################
#  File: conftest.py
################################################################################################
from ai4service_automated_classification.tests.fixtures.data_test_PTC import data_most_frequent_label_0, dummy_data_PTC, text_labels_PTC  # NOQA: F401
from ai4service_automated_classification.tests.fixtures.data_test_PTC import train_test_split_fixture, transformer_fixture  # NOQA: F401
from ai4service_automated_classification.tests.fixtures.data_test_PTC import filter_dict_PTC  # NOQA: F401
from ai4service_automated_classification.tests.fixtures.data_test_PTC import dummy_hierarchy_PTC, processed_dummy_data_PTC  # NOQA: F401
from ai4service_automated_classification.tests.fixtures.data_test_PTC import processed_dummy_hierarchy_PTC  # NOQA: F401
