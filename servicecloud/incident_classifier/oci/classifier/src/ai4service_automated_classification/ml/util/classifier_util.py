################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Mon Jun 26 10:43:19 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: 6f97d4d7e3ddbc81501ce1cb7edd5e3e55f68567 $
################################################################################################
#  File: classifier_util.py
################################################################################################
"""Classification utility functions."""

import decimal
import numpy as np
from sklearn.linear_model import LogisticRegression
from typing import List
from itertools import zip_longest

DefaultClassifier = LogisticRegression
default_params = {'penalty': 'l2',
                  'tol': 0.0001,
                  'C': 1,
                  'fit_intercept': True,
                  'intercept_scaling': 1.0,
                  'max_iter': 10000,
                  'solver': 'liblinear',
                  'multi_class': 'ovr',
                  'warm_start': False,
                  'random_state': None,
                  'dual': False
                  }
default_grid = {"C": [0.001, 0.01, 0.1, 1, 2, 10], "penalty": ["l1", "l2"]}


def populate_alternate_rankings(one_dfs_rankings: List[int],
                                another_dfs_rankings: List[int],
                                k: int = 5) -> List[int]:
    """
    Populate rankings by grouping alternate top value from two rank groups.

    @param one_dfs_rankings list of DFS based ranked predictions of one node
    @param another_dfs_rankings list of DFS based ranked predictions of another node
    @param k: The number of node ids to retrieve in ranking. By default, it is 5
    @return: merged list of dfs_rankings of two nodes

    """
    alternate_rankings: List = []
    for node_one, node_alt in zip_longest(one_dfs_rankings, another_dfs_rankings):
        if node_one and len(alternate_rankings) != k:
            alternate_rankings.append(node_one)
        if node_alt and len(alternate_rankings) != k:
            alternate_rankings.append(node_alt)
    return alternate_rankings


def group_nodes(data: np.ndarray, top_children: List, dfs_rankings: List[int],
                threshold: float, k: int = 5) -> None:
    """
    Group two nodes by adding top alternate child in the list and get the rankings.

    @param data: array/batch of transformed samples
    @param top_children sorted child node based on parent's probabilities
    @param dfs_rankings DFS ranking based on given node's sorted predict_proba
    @param k: The number of node ids to retrieve in ranking. By default, it is 5
    @param threshold to group two nodes which have difference based on threshold
    @rtype: None
    @return: None

    """
    if len(dfs_rankings) >= k:
        return

    pred_node = top_children[0]
    alt_pred_node = top_children[1]

    pred_dfs_rankings: List[int] = []
    pred_node.build_dfs_rankings(pred_dfs_rankings, data, k, threshold)

    alt_dfs_rankings: List[int] = []
    alt_pred_node.build_dfs_rankings(alt_dfs_rankings, data, k, threshold)

    # adding children rankings alternatively, if have any
    alternate_rankings = populate_alternate_rankings(pred_dfs_rankings, alt_dfs_rankings, k)
    dfs_rankings.extend(alternate_rankings[:(k - len(dfs_rankings))])


def assert_first_rank(y_pred: List[int], rankings: List[List[int]]) -> None:
    """Check top position of rankings matches with actual predictions.

    @param y_pred: list of actual prediction with predict method
    @param rankings: list of predictions ranking with predict_ranking.
    """
    for index in range(0, len(y_pred)):
        if not y_pred[index] == rankings[index][0]:
            raise LookupError("Got mismatch on {} index : ".format(index))


def accuracy_at_K(rankings: List[List[int]], target_labels: List[int], k: int) -> float:
    """
    Find the templates's potential accuracy for K prediction labels.

    @param k: total number of nearby predictions to get
    @param rankings: nearby prediction rankings
    @param target_labels: actual label of each data point
    @rtype float
    @return accuracy: accuracy at K labels

    """
    positive = 0
    for i in range(0, len(rankings)):
        if target_labels[i] in rankings[i][:k]:
            positive += 1
    accuracy = positive / len(target_labels)
    return accuracy


def get_difference(probabilities: np.ndarray, top_predictions: np.ndarray, threshold: float) -> float:
    """
    Find difference between top predictions.

    @param probabilities: probabilities of Node's classifier
    @param top_predictions: top predictions according to sorted probabilities
    @param threshold: to get difference upto decimal places which threshold is having
    @return difference: difference between top two probabilities

    """
    difference = .0
    if len(probabilities) > 1:
        n_decimal_places = abs(decimal.Decimal(str(threshold)).as_tuple().exponent) + 1
        difference = round(probabilities[top_predictions[0]] - probabilities[top_predictions[1]], n_decimal_places)
    return difference
