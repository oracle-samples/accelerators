################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Tue Jan 31 13:02:47 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: f757ce28a4cb1731f29fb5bc4d392732bb98d120 $
################################################################################################
#  File: confusion_util.py
################################################################################################
"""Utility functions for confusion matrix processing."""
from typing import List
import numpy as np


def merge_confusion_matrix(conf_mat: np.ndarray, idxs: List) -> np.ndarray:
    """
    Merge indices in confusion matrix.

    @type conf_mat: array
    @param conf_mat: confusion matrix input with raw counts
    @type idxs: list
    @param idxs: which indices
    @rtype: array
    @return: new confusion matrix with merged indices
    """
    min_idx = np.min(idxs)
    min_idx_pos = idxs.index(min_idx)
    conf_copy = np.copy(conf_mat)
    conf_copy[min_idx, :] = np.sum(conf_copy[idxs, :], axis=0)
    conf_copy[:, min_idx] = np.sum(conf_copy[:, idxs], axis=1)
    del idxs[min_idx_pos]
    conf_copy = np.delete(conf_copy, idxs, axis=1)
    conf_copy = np.delete(conf_copy, idxs, axis=0)
    return conf_copy


def merge_product_ids(conf_mat: np.ndarray, prod_ids: list, id2idx: dict) -> np.ndarray:
    """
    Merge products in confusion matrix.

    @type conf_mat: array
    @param conf_mat: confusion matrix input with raw counts
    @type prod_ids: list
    @param prod_ids: which indices products
    @type id2idx: dict
    @param id2idx: something that gives the index in the confusion matrix for a given product
    @rtype: array
    @return: new confusion matrix with merged indices

    """
    idxs = [id2idx[product] for product in prod_ids]
    return merge_confusion_matrix(conf_mat, idxs)
