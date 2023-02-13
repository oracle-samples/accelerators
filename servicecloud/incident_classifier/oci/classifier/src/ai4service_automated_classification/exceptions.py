################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Tue Jan 31 13:02:46 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: 0ebe32383039eb357331d5f5d1f55c339ab332fc $
################################################################################################
#  File: exceptions.py
################################################################################################
"""Module containing custom exceptions."""


class ModelAlreadyExistsError(Exception):
    """Raised when a templates with the same parameters already exists & the force flag was not set."""
