################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Mon Jun 26 10:43:21 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: 67240acf127548083595cf3d6048c1007b4243a1 $
################################################################################################
#  File: test_classifier_util.py
################################################################################################
import numpy as np
import pytest

from ai4service_automated_classification.constants import PRODUCT_ID_COLUMN, GDFS_THRESHOLD
from ai4service_automated_classification.ml.ProductTreeClassifier import ProductTreeClassifier
from ai4service_automated_classification.ml.util.classifier_util import populate_alternate_rankings, \
    DefaultClassifier, default_params, group_nodes, \
    assert_first_rank, accuracy_at_K, get_difference


@pytest.mark.parametrize("k", [1, 2, 3, 4, 5, 10])
def test_populate_alternate_rankings(k):
    a_ranks = [1, 3, 5, 7, 9]
    b_ranks = [2, 4, 6, 8, 10]
    c_ranks = populate_alternate_rankings(a_ranks, b_ranks, k=k)
    assert len(c_ranks) == k
    for index in range(0, len(c_ranks)):
        assert (index + 1) == c_ranks[index]


def test_group_nodes(processed_dummy_hierarchy_PTC, train_test_split_fixture, transformer_fixture):
    _, _, y_train, y_test = train_test_split_fixture
    X_trainf, _ = transformer_fixture
    raw_classifier = DefaultClassifier(**default_params)
    full_classifier = ProductTreeClassifier(processed_dummy_hierarchy_PTC[PRODUCT_ID_COLUMN], classifier=raw_classifier)
    full_classifier.fit(X_trainf, y_train)
    top_children = [node for node in full_classifier.id2node.values() if node.id != -1]
    dfs_ranking = []
    bfs_ranking = []
    group_nodes(X_trainf[0], top_children, dfs_ranking, GDFS_THRESHOLD)
    assert len(bfs_ranking) == len(set(bfs_ranking))
    assert top_children[0].id == dfs_ranking[0]
    assert top_children[1].id == dfs_ranking[1]
    assert top_children[0].children[0].id == dfs_ranking[2]


def test_evaluate_top_rank():
    y_pred = [1, 2, 3, 4]
    rankings = [[1, 2, 3, 4],
                [2, 1, 3, 4],
                [3, 2, 1, 4],
                [4, 3, 2, 1]]
    assert_first_rank(y_pred, rankings)


def test_assert_first_rank_with_exception():
    with pytest.raises(LookupError) as e:
        y_pred = [1, 2, 3, 4]
        rankings = [[1, 2, 3, 4],
                    [3, 1, 3, 4],
                    [3, 2, 1, 4],
                    [4, 3, 2, 1]]
        assert_first_rank(y_pred, rankings)
        assert e.type == LookupError
        assert "Got mismatch" in e.value


def test_accuracy_at_K():
    y_pred = [1, 2, 3, 4]
    rankings = [[1, 2, 3, 4],
                [1, 2, 3, 4],
                [1, 2, 3, 4],
                [1, 3, 2, 4]]
    accuracy_at_one = accuracy_at_K(rankings, y_pred, 1)
    assert accuracy_at_one == 0.25
    accuracy_at_two = accuracy_at_K(rankings, y_pred, 2)
    assert accuracy_at_two == 0.50
    accuracy_at_three = accuracy_at_K(rankings, y_pred, 3)
    assert accuracy_at_three == 0.75
    accuracy_at_four = accuracy_at_K(rankings, y_pred, 4)
    assert accuracy_at_four == 1.0


def test_get_difference():
    probas = np.array([0.00563617, 0.03057876, 0.02198178, 0.00600429, 0.00781377, 0.00777289, 0.02130334, 0.00339385,
                       0.04523813, 0.00543612, 0.03063779, 0.0344867, 0.00852404, 0.00147243, 0.00719149, 0.0107368,
                       0.01520774, 0.00435952, 0.04260971, 0.1312892, 0.01538904, 0.08105558, 0.08733318, 0.02921751,
                       0.00471923, 0.01118514, 0.00444702, 0.00885796, 0.00963983, 0.2387534, 0.02743576, 0.04029187])

    top_predictions = np.argsort(-probas)
    threshold = 0.1
    difference = get_difference(probas, top_predictions, threshold)
    assert difference == 0.11
    assert difference > threshold


def test_get_difference_with_two_decimal_threshold():
    probas = np.array([0.00563618, 0.03057876, 0.02198178, 0.00600429, 0.00781377, 0.00777289, 0.02130334, 0.00339385,
                       0.04523813, 0.00543612, 0.03063779, 0.0344867, 0.00852404, 0.00147243, 0.00719149, 0.0107368,
                       0.01520774, 0.00435952, 0.04260971, 0.1312892, 0.01538904, 0.08105558, 0.08733318, 0.02921751,
                       0.00471923, 0.01118514, 0.00444702, 0.00885796, 0.00963983, 0.2387534, 0.02743576, 0.04029187])

    top_predictions = np.argsort(-probas)
    threshold = 0.01
    difference = get_difference(probas, top_predictions, threshold)
    assert difference == 0.107
    assert difference > threshold


def test_get_difference_with_three_decimal_threshold():
    probas = np.array([0.3346, 0.3349, 0.3305])
    top_predictions = np.argsort(-probas)
    threshold = 0.001
    difference = get_difference(probas, top_predictions, threshold)
    assert difference == 0.0003
    assert difference <= threshold


def test_get_difference_with_non_zero_difference():
    probas = np.array([0.3342, 0.3349, 0.3309])
    top_predictions = np.argsort(-probas)
    threshold = 0.001
    difference = get_difference(probas, top_predictions, threshold)
    assert difference != 0
    assert difference <= threshold
