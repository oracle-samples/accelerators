################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Tue Jan 31 13:02:48 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: d4b99952373e34a7cf8eeb004dd28779c1d62f4d $
################################################################################################
#  File: util.py
################################################################################################
"""Generic utility functions."""

import os
import logging
from typing import Optional
import numpy as np
from pandas import DataFrame
from ai4service_automated_classification.constants import ID_COLUMN, NAME_COLUMN

logger = logging.getLogger(os.path.basename(__file__))


def build_id2name(hierarchy: DataFrame, filter_dict: Optional[dict] = None) -> dict:
    """
    Build id2names.

    @type hierarchy: pandas.DataFrame
    @param hierarchy: exported product tree hierarchy
    @type filter_dict: dict
    @param filter_dict: in case we want to filter by some products
                        and not include everythin in the dictionary (Default value = None)
    @rtype: dict
    @return: id2name is a dictionary containing the ids (keys) and nodes (values)

    """
    id2name = dict()
    for _, row in hierarchy.iterrows():
        current_child = row[ID_COLUMN]
        current_child_name = row[NAME_COLUMN]
        id2name[current_child] = current_child_name
    if filter_dict is not None:
        for key, val in filter_dict.items():
            id2name[key] = val
    return id2name


def build_id2idx(ids: np.ndarray) -> dict:
    """
    Build id to product index.

    @type ids: list
    @param ids: list of product ids
    @rtype: dict
    @return: product ids (keys) to product index in the confusion matrix (keys)
    """
    labs = sorted(np.unique(ids))
    logger.info("Nr of unique labels: %d", len(labs))
    id2idx = dict()
    for idx, label in enumerate(labs):
        id2idx[label] = idx
    return id2idx
