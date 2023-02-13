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
#  SHA1: $Id: d873db67be2226a535f1df510df85af3350c0b7a $
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