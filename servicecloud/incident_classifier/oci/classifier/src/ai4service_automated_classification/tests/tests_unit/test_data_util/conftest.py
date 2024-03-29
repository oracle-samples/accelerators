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
#  SHA1: $Id: 33ad80b3d538be0fd1cf3ec45ead8cc26933ff81 $
################################################################################################
#  File: conftest.py
################################################################################################
from ai4service_automated_classification.tests.fixtures.data import dummy_data_contains_html  # NOQA: F401
from ai4service_automated_classification.tests.fixtures.data import incidents_no_data_column  # NOQA: F401
from ai4service_automated_classification.tests.fixtures.data_test_PTC import dummy_hierarchy_PTC, dummy_category_hierarchy_PTC  # NOQA: F401
from ai4service_automated_classification.tests.fixtures.data_test_PTC import dummy_disposition_hierarchy_PTC  # NOQA: F401
