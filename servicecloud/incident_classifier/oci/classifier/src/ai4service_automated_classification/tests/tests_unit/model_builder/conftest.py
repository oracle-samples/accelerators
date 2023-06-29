################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Mon Jun 26 10:43:22 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: 5259eb5a7440e0754786eceaa21ce9568d35776a $
################################################################################################
#  File: conftest.py
################################################################################################
from ai4service_automated_classification.tests.fixtures.data import hierarchy_data, incidents_data, incidents_no_disposition  # NOQA: F401
from ai4service_automated_classification.tests.fixtures.data import incidents_data_disposition_class_0, disposition_class_0  # NOQA: F401
from ai4service_automated_classification.tests.fixtures.data import incidents_no_category_disposition, incidents_no_data  # NOQA: F401
from ai4service_automated_classification.tests.fixtures.data import dummy_data, processed_dummy_data  # NOQA: F401
from ai4service_automated_classification.tests.fixtures.data import hierarchy_data_empty, processed_incidents_empty_hierarchy  # NOQA: F401
from ai4service_automated_classification.tests.fixtures.data import no_disposition_data, no_category_disposition_data, no_data  # NOQA: F401
from ai4service_automated_classification.tests.fixtures.data_test_PTC import data_most_frequent_label_0  # NOQA: F401
from ai4service_automated_classification.tests.fixtures.data_test_PTC import dummy_data_PTC, dummy_hierarchy_PTC  # NOQA: F401
from ai4service_automated_classification.tests.fixtures.data_test_PTC import processed_dummy_data_PTC, processed_dummy_hierarchy_PTC  # NOQA: F401
