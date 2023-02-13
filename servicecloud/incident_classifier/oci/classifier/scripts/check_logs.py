################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Tue Jan 31 13:02:45 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: ce01572b4825c7d30cf0f03622898d068b61016b $
################################################################################################
#  File: check_logs.py
################################################################################################

import os

from ads.common.oci_logging import OCILogGroup
from oci.config import from_file

from training_scripts.constants import LOG_GROUP_ID

config = from_file(LOG_GROUP_ID)

# Get the log group OCID
log_group_ocid = os.getenv

# Get a existing log group by OCID
log_group = OCILogGroup.from_ocid(log_group_ocid)

# Get a list of existing log resources in a log group
# A list of ads.common.oci_logging.OCILog objects will be returned
logs = log_group.list_logs()

logs = sorted(logs)

# Get the last 50 log messages as a list
logs[-1].tail(limit=50)

# Stream the log messages to terminal or screen
# This block sthe main process until user interruption.
logs[-1].stream()
