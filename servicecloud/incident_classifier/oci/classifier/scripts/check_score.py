################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Mon Jun 26 10:43:18 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: 82c5db35da094261a56115093508245e97c1bddc $
################################################################################################
#  File: check_score.py
################################################################################################

import score

body = {"jsonData": {
    "inquiry": "Hello Team, This is Peter Parker and my spider web is not working. Please help.",
    "product": 0,
    "category": 0,
    "disposition": 0
}}

print(score.predict(body))