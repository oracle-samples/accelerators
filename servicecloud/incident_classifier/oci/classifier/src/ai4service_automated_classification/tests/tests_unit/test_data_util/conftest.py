################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Tue Jan 31 13:02:50 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: e9e5952cad9b49aca23ba976e08885617c6a429b $
################################################################################################
#  File: conftest.py
################################################################################################
from ai4service_automated_classification.tests.fixtures.data import dummy_data_contains_html  # NOQA: F401
from ai4service_automated_classification.tests.fixtures.data import incidents_no_data_column  # NOQA: F401
from ai4service_automated_classification.tests.fixtures.data_test_PTC import dummy_hierarchy_PTC, dummy_category_hierarchy_PTC  # NOQA: F401
from ai4service_automated_classification.tests.fixtures.data_test_PTC import dummy_disposition_hierarchy_PTC  # NOQA: F401
