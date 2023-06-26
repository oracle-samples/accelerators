#!/usr/bin/env python3
################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Mon Jun 26 10:43:23 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: 7e4673dd6c7403931a590b9406da144c56b56f5a $
################################################################################################
#  File: test_remove_signature.py
################################################################################################

import os

import spacy

from ai4service_automated_classification.constants import CLEANUP_SCOPE
from ai4service_automated_classification.ml.util.data_util import text_clean


def test_remove_signatue_single():
    try:
        nlp = spacy.load("en_core_web_lg")
    except IOError as err:
        nlp = None
        pass

    os.environ[CLEANUP_SCOPE] = "html, urls, emails, phone, numbers, date, person"
    query = "<b> Hi, I am Peter Parker from https://oracle.com. " \
            "I am writing mail regarding my broken laptop. " \
            "Contact me on: me@example.com " \
            "or call me on +1800 212 132 on thursday.</b>"
    expected = 'hi write mail broken laptop contact'
    assert text_clean(query, nlp) == expected
