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
#  SHA1: $Id: 5702a5356208a19b8295707925f446081a7e4d8b $
################################################################################################
#  File: test_ProductTreeClassifier.py
################################################################################################
import json
from unittest.mock import patch

import numpy as np
import pandas as pd
import pytest
from sklearn.model_selection import train_test_split
from sklearn.svm import LinearSVC
from sklearn.utils._testing import assert_almost_equal

from ai4service_automated_classification.ml.Node import Node
from ai4service_automated_classification.ml.ProductTreeClassifier import ProductTreeClassifier, \
    DefaultClassifier, default_params
from ai4service_automated_classification.ml.Transformer import Transformer
from ai4service_automated_classification.ml.model_selection import AutoKFold
from ai4service_automated_classification.ml.util.data_util import build_filter_dict, preprocess_incidents, \
    preprocess_hierarchy
from ai4service_automated_classification.constants import DATA_COLUMN, PRODUCT_ID_COLUMN, GDFS_THRESHOLD


def test_instantiate_classifier(processed_dummy_hierarchy_PTC):
    full_classifier = ProductTreeClassifier(processed_dummy_hierarchy_PTC[PRODUCT_ID_COLUMN])
    assert isinstance(full_classifier, ProductTreeClassifier)


def test_PTC_grid_src_no_cv(processed_dummy_hierarchy_PTC):
    grid_src = {'C': [1], "penalty": ["l2"]}
    raw_classifier = DefaultClassifier(**default_params)
    with pytest.raises(ValueError) as e:
        ProductTreeClassifier(processed_dummy_hierarchy_PTC[PRODUCT_ID_COLUMN],
                              grid_search=grid_src,
                              classifier=raw_classifier)
    assert "no grid search method" in str(e.value)


def test_PTC_grid_src_cv_no_grid_src(processed_dummy_hierarchy_PTC):
    raw_classifier = DefaultClassifier(**default_params)
    with pytest.raises(ValueError) as e:
        ProductTreeClassifier(processed_dummy_hierarchy_PTC[PRODUCT_ID_COLUMN],
                              grid_search_cv=AutoKFold(),
                              classifier=raw_classifier)
    assert "no grid search parameters" in str(e.value)


def test_PTC_predict(processed_dummy_hierarchy_PTC, train_test_split_fixture, transformer_fixture):
    _, _, y_train, y_test = train_test_split_fixture
    X_trainf, X_testf = transformer_fixture
    raw_classifier = DefaultClassifier(**default_params)
    full_classifier = ProductTreeClassifier(processed_dummy_hierarchy_PTC[PRODUCT_ID_COLUMN], classifier=raw_classifier)
    full_classifier.fit(X_trainf, y_train)
    pred = full_classifier.predict(X_testf)
    assert pred == [1, 1, 1, 1, 2, 2]


def test_PTC_score_predict_transform(dummy_data_PTC_extra_product):
    data, hierarchy = dummy_data_PTC_extra_product
    hierarchy = preprocess_hierarchy(hierarchy[PRODUCT_ID_COLUMN])
    data = preprocess_incidents(data)[PRODUCT_ID_COLUMN]
    filter_dict = build_filter_dict(data, hierarchy, PRODUCT_ID_COLUMN)
    labels = data[PRODUCT_ID_COLUMN]
    text = data[DATA_COLUMN]
    X_train, X_test, y_train, y_test = train_test_split(text, labels, stratify=labels, test_size=0.1, random_state=1)
    X_train = X_train.reset_index(drop=True)
    X_test = X_test.reset_index(drop=True)
    raw_classifier = DefaultClassifier(**default_params)
    full_classifier = ProductTreeClassifier(hierarchy,
                                            classifier=raw_classifier,
                                            filter_dict=filter_dict,
                                            transformer=Transformer())
    full_classifier.fit(X_train, y_train)
    score_predict_result = full_classifier.score_predict(X_test, y_test)
    assert_almost_equal(score_predict_result[0], 0.8333333333333334)
    assert score_predict_result[1] == [1, 1, 1, 2, 2, 2]
    assert score_predict_result[2].tolist() == [[3, 0, 0], [0, 2, 0], [0, 1, 0]]


def test_PTC_score_predict_transform_already_fit(dummy_data_PTC_extra_product):
    data, hierarchy = dummy_data_PTC_extra_product
    hierarchy = preprocess_hierarchy(hierarchy[PRODUCT_ID_COLUMN])
    data = preprocess_incidents(data)[PRODUCT_ID_COLUMN]
    filter_dict = build_filter_dict(data, hierarchy, PRODUCT_ID_COLUMN)
    labels = data[PRODUCT_ID_COLUMN]
    text = data[DATA_COLUMN]
    X_train, X_test, y_train, y_test = train_test_split(text, labels, stratify=labels, test_size=0.1, random_state=1)
    X_train = X_train.reset_index(drop=True)
    X_test = X_test.reset_index(drop=True)
    featurizer = Transformer()
    featurizer.fit(X_train)
    raw_classifier = DefaultClassifier(**default_params)
    full_classifier = ProductTreeClassifier(hierarchy,
                                            classifier=raw_classifier,
                                            filter_dict=filter_dict,
                                            transformer=featurizer)
    full_classifier.fit(X_train, y_train)
    score_predict_result = full_classifier.score_predict(X_test, y_test)
    assert_almost_equal(score_predict_result[0], 0.8333333333333334)
    assert score_predict_result[1] == [1, 1, 1, 2, 2, 2]
    assert score_predict_result[2].tolist() == [[3, 0, 0], [0, 2, 0], [0, 1, 0]]


def test_PTC_score_predict_filter_dict(dummy_data_PTC_extra_product):
    data, hierarchy = dummy_data_PTC_extra_product
    hierarchy = preprocess_hierarchy(hierarchy[PRODUCT_ID_COLUMN])
    data = preprocess_incidents(data)[PRODUCT_ID_COLUMN]
    filter_dict = build_filter_dict(data, hierarchy, PRODUCT_ID_COLUMN)
    labels = data[PRODUCT_ID_COLUMN]
    text = data[DATA_COLUMN]
    X_train, X_test, y_train, y_test = train_test_split(text, labels, stratify=labels, test_size=0.1, random_state=1)
    featurizer = Transformer()
    X_trainf = featurizer.fit_transform(X_train)
    X_testf = featurizer.transform(X_test)
    raw_classifier = DefaultClassifier(**default_params)
    full_classifier = ProductTreeClassifier(hierarchy,
                                            classifier=raw_classifier,
                                            filter_dict=filter_dict)
    full_classifier.fit(X_trainf, y_train)
    score_predict_result = full_classifier.score_predict(X_testf, y_test)
    assert_almost_equal(score_predict_result[0], 0.8333333333333334)
    assert score_predict_result[1] == [1, 1, 1, 2, 2, 2]
    assert score_predict_result[2].tolist() == [[3, 0, 0], [0, 2, 0], [0, 1, 0]]


def test_PTC_predict_Value_Error(processed_dummy_hierarchy_PTC, transformer_fixture):
    _, X_testf = transformer_fixture
    raw_classifier = DefaultClassifier(**default_params)
    full_classifier = ProductTreeClassifier(processed_dummy_hierarchy_PTC[PRODUCT_ID_COLUMN], classifier=raw_classifier)
    with pytest.raises(ValueError) as e:
        full_classifier.predict(X_testf)
    assert "Internal Tree is not built. Call fit first." == str(e.value)


def test_PTC_predict_proba_Value_Error(processed_dummy_hierarchy_PTC, transformer_fixture):
    _, X_testf = transformer_fixture
    raw_classifier = DefaultClassifier(**default_params)
    full_classifier = ProductTreeClassifier(processed_dummy_hierarchy_PTC[PRODUCT_ID_COLUMN], classifier=raw_classifier)
    with pytest.raises(ValueError) as e:
        full_classifier.predict_proba(X_testf)
    assert "Internal Tree is not built. Call fit first." == str(e.value)


def test_PTC_explain_Value_Error(processed_dummy_hierarchy_PTC, transformer_fixture):
    _, X_testf = transformer_fixture
    raw_classifier = DefaultClassifier(**default_params)
    full_classifier = ProductTreeClassifier(processed_dummy_hierarchy_PTC[PRODUCT_ID_COLUMN], classifier=raw_classifier)
    with pytest.raises(ValueError) as e:
        full_classifier.explain(X_testf)
    assert "Internal Tree is not built. Call fit first." == str(e.value)


def test_PTC_dump_json_Value_Error(processed_dummy_hierarchy_PTC):
    raw_classifier = DefaultClassifier(**default_params)
    full_classifier = ProductTreeClassifier(processed_dummy_hierarchy_PTC[PRODUCT_ID_COLUMN], classifier=raw_classifier)
    with pytest.raises(ValueError) as e:
        full_classifier.dump_json()
    assert "Internal Tree is not built. Call fit or build_product_tree." == str(e.value)


def test_PTC_predict_proba(processed_dummy_hierarchy_PTC, train_test_split_fixture, transformer_fixture):
    _, _, y_train, _ = train_test_split_fixture
    X_trainf, X_testf = transformer_fixture
    raw_classifier = DefaultClassifier(**default_params)
    full_classifier = ProductTreeClassifier(processed_dummy_hierarchy_PTC[PRODUCT_ID_COLUMN], classifier=raw_classifier)
    full_classifier.fit(X_trainf, y_train)
    pred_proba = full_classifier.predict_proba(X_testf)
    assert pred_proba['prediction'] == [1, 1, 1, 1, 2, 2]
    assert_almost_equal(pred_proba['confidenceScore'], [0.5534105527824921, 0.5702985628030365, 0.5622483642132973,
                                                        0.7432462651463125, 0.5671407151599492, 0.5947212032704199])
    # assert pred_proba[0] == [1, 2, 2]
    # assert_almost_equal(pred_proba[1], [0.5192688751850155, 0.5679298523297411, 0.5544875164591956])
    # this change in necessary due to changes in the code for LV-11062


def test_PTC_explain(processed_dummy_hierarchy_PTC, train_test_split_fixture, transformer_fixture):
    _, _, y_train, _ = train_test_split_fixture
    X_trainf, X_testf = transformer_fixture
    raw_classifier = DefaultClassifier(**default_params)
    full_classifier = ProductTreeClassifier(processed_dummy_hierarchy_PTC[PRODUCT_ID_COLUMN], classifier=raw_classifier)
    full_classifier.fit(X_trainf, y_train)
    explain_result = full_classifier.explain(X_testf)
    assert explain_result == ([1, 1, 1, 1, 2, 2], [[1], [1], [1], [1], [1, 2], [1, 2]])


def test_PTC_score_predict(processed_dummy_hierarchy_PTC, train_test_split_fixture, transformer_fixture):
    _, _, y_train, y_test = train_test_split_fixture
    X_trainf, X_testf = transformer_fixture
    raw_classifier = DefaultClassifier(**default_params)
    full_classifier = ProductTreeClassifier(processed_dummy_hierarchy_PTC[PRODUCT_ID_COLUMN], classifier=raw_classifier)
    full_classifier.fit(X_trainf, y_train)
    score_predict_result = full_classifier.score_predict(X_testf, y_test)
    assert score_predict_result[0] == 0.8333333333333334
    assert score_predict_result[1] == [1, 1, 1, 1, 2, 2]
    assert score_predict_result[2].tolist() == [[3, 0], [1, 2]]


def test_PTC_score_predict_classifierNone(processed_dummy_hierarchy_PTC, train_test_split_fixture, transformer_fixture):
    _, _, y_train, y_test = train_test_split_fixture
    X_trainf, X_testf = transformer_fixture
    full_classifier = ProductTreeClassifier(processed_dummy_hierarchy_PTC[PRODUCT_ID_COLUMN], grid_search_cv=None,
                                            grid_search=None, classifier=None, transformer=None)
    full_classifier.fit(X_trainf, y_train)
    score_predict_result = full_classifier.score_predict(X_testf, y_test)
    assert score_predict_result[0] == 0.8333333333333334
    assert score_predict_result[1] == [1, 1, 1, 1, 2, 2]
    assert score_predict_result[2].tolist() == [[3, 0], [1, 2]]


def test_is_built_T(processed_dummy_hierarchy_PTC, train_test_split_fixture, transformer_fixture):
    _, _, y_train, _ = train_test_split_fixture
    X_trainf, _ = transformer_fixture
    raw_classifier = DefaultClassifier(**default_params)
    full_classifier = ProductTreeClassifier(processed_dummy_hierarchy_PTC[PRODUCT_ID_COLUMN], classifier=raw_classifier)
    full_classifier.fit(X_trainf, y_train)
    assert full_classifier.is_built() is True


def test_is_built_F(processed_dummy_hierarchy_PTC):
    raw_classifier = DefaultClassifier(**default_params)
    full_classifier = ProductTreeClassifier(processed_dummy_hierarchy_PTC[PRODUCT_ID_COLUMN], classifier=raw_classifier)
    assert full_classifier.is_built() is False


def test_compute_scores(processed_dummy_hierarchy_PTC, train_test_split_fixture, transformer_fixture):
    _, _, y_train, y_test = train_test_split_fixture
    X_trainf, X_testf = transformer_fixture
    raw_classifier = DefaultClassifier(**default_params)
    full_classifier = ProductTreeClassifier(processed_dummy_hierarchy_PTC[PRODUCT_ID_COLUMN], classifier=raw_classifier)
    full_classifier.fit(X_trainf, y_train)
    result = full_classifier.compute_scores(X_testf, y_test)
    assert result[0] == 0.8333333333333334
    assert result[1] == [1, 1, 1, 1, 2, 2]
    assert result[2].tolist() == [[3, 0], [1, 2]]


def test_grid_search_n_jobs(processed_dummy_hierarchy_PTC, train_test_split_fixture, transformer_fixture):
    n_jobs = 18
    _, _, y_train, _ = train_test_split_fixture
    X_trainf, _ = transformer_fixture
    grid_src = {'C': [1], "penalty": ["l2"]}
    raw_classifier = DefaultClassifier(**default_params)
    full_classifier = ProductTreeClassifier(processed_dummy_hierarchy_PTC[PRODUCT_ID_COLUMN],
                                            grid_search=grid_src,
                                            grid_search_cv=AutoKFold(),
                                            classifier=raw_classifier,
                                            grid_search_n_jobs=n_jobs)
    assert full_classifier.grid_search_n_jobs == n_jobs
    with patch('ai4service_automated_classification.ml.Node.GridSearchCV') as mock:
        full_classifier.fit(X_trainf, y_train)
        args, kwargs = mock.call_args
        assert kwargs.get('n_jobs') == n_jobs


def test_grid_search_cv(processed_dummy_hierarchy_PTC, train_test_split_fixture, transformer_fixture):
    n_jobs = 18
    _, _, y_train, _ = train_test_split_fixture
    X_trainf, _ = transformer_fixture
    grid_src = {'C': [1], "penalty": ["l2"]}
    raw_classifier = DefaultClassifier(**default_params)
    full_classifier = ProductTreeClassifier(processed_dummy_hierarchy_PTC[PRODUCT_ID_COLUMN],
                                            grid_search=grid_src,
                                            grid_search_cv=AutoKFold(),
                                            classifier=raw_classifier,
                                            grid_search_n_jobs=n_jobs)
    assert full_classifier.grid_search_n_jobs == n_jobs
    with patch('ai4service_automated_classification.ml.Node.GridSearchCV') as mock:
        full_classifier.fit(X_trainf, y_train)
        args, kwargs = mock.call_args
        assert kwargs.get('cv').__class__ == AutoKFold().__class__


def test_compute_scores_Value_error(processed_dummy_hierarchy_PTC, train_test_split_fixture, transformer_fixture):
    _, _, _, y_test = train_test_split_fixture
    _, X_testf = transformer_fixture
    raw_classifier = DefaultClassifier(**default_params)
    full_classifier = ProductTreeClassifier(processed_dummy_hierarchy_PTC[PRODUCT_ID_COLUMN], classifier=raw_classifier)
    with pytest.raises(ValueError) as e:
        full_classifier.compute_scores(X_testf, y_test)
    assert "Internal Tree is not built. Call fit or build_product_tree." == str(e.value)


def test_compute_scores_conf(processed_dummy_hierarchy_PTC, train_test_split_fixture, transformer_fixture):
    _, _, y_train, y_test = train_test_split_fixture
    X_trainf, _ = transformer_fixture
    raw_classifier = DefaultClassifier(**default_params)
    full_classifier = ProductTreeClassifier(processed_dummy_hierarchy_PTC[PRODUCT_ID_COLUMN], classifier=raw_classifier)
    full_classifier.fit(X_trainf, y_train)
    conf_mat = np.asarray([[1, 0], [0, 2]])
    full_classifier.compute_scores_conf(conf_mat, y_test)
    for child in full_classifier.root_node.children:
        assert child.has_scores_computed() is False
    for child1 in child.children:
        assert child1.has_scores_computed() is False


def test_compute_scores_conf_value_error(processed_dummy_hierarchy_PTC, train_test_split_fixture):
    _, _, _, y_test = train_test_split_fixture
    raw_classifier = DefaultClassifier(**default_params)
    full_classifier = ProductTreeClassifier(processed_dummy_hierarchy_PTC[PRODUCT_ID_COLUMN], classifier=raw_classifier)
    conf_mat = np.asarray([[1, 0], [0, 2]])
    with pytest.raises(ValueError) as e:
        full_classifier.compute_scores_conf(conf_mat, y_test)
    assert "Internal Tree is not built. Call fit or build_product_tree." == str(e.value)


@pytest.fixture
def tree_json_dir(tmpdir_factory):
    tmpdir = tmpdir_factory.mktemp("json_files")
    return tmpdir


def test_dump_json(processed_dummy_hierarchy_PTC, train_test_split_fixture, transformer_fixture, tree_json_dir):
    tmpdir = tree_json_dir
    _, _, y_train, _ = train_test_split_fixture
    X_trainf, _ = transformer_fixture
    raw_classifier = DefaultClassifier(**default_params)
    full_classifier = ProductTreeClassifier(processed_dummy_hierarchy_PTC[PRODUCT_ID_COLUMN], classifier=raw_classifier)
    full_classifier.fit(X_trainf, y_train)
    full_classifier.dump_json(file=tmpdir.join("tree.json"))
    with open(tmpdir.join("tree.json"), "r") as f:
        json_data = json.load(f)
    expected = [{'id': 1, 'title': 'Internal Systems', 'is_label': True,
                 'children': [{'id': 2, 'title': 'Techmail', 'is_label': True, 'children': [], 'subtree_labels': [2],
                               'parents': [1]}],
                 'subtree_labels': [1, 2], 'parents': [-1]}]
    assert json_data == expected


# Below I start adding the tests for the Node class
def test_Node_is_leaf(Node_PTC):
    node_1, node_2 = Node_PTC
    assert node_1.is_leaf() is False
    assert node_2.is_leaf() is True


def test_Node_get_subproducts(Node_PTC):
    node_1 = Node(1, name="Node 1")
    node_2 = Node(2, name="Node 2")
    node_3 = Node(3, name="Node 3")
    node_2.add_child(node_3)
    node_1.add_child(node_2)
    products = []
    node_1.get_subproducts(products)
    assert [1, 2, 3] == products
    products = []
    node_2.get_subproducts(products)
    assert [2, 3] == products
    products = []
    node_3.get_subproducts(products)
    assert [3] == products


# This shows node_1, even if it is not a label, because the only_label parameter only applies to the children.
def test_to_json_labels_true(Node_PTC):
    node_1, node_2 = Node_PTC
    assert node_1.to_json(only_labels=True) == {'id': 1, 'title': 'Internal Systems', 'is_label': False,
                                                'children': [],
                                                'subtree_labels': [],
                                                'parents': []}


def test_add_child(Node_PTC):
    node_1, node_2 = Node_PTC
    assert node_1.children_ids == {2}


# TODO: to change once we fix to_json function
def test_to_json_labels_false_children(Node_PTC):
    node_1, node_2 = Node_PTC
    dj_children = node_1.to_json(only_labels=False)["children"]
    assert dj_children == [
        {'id': 2, 'title': 'Techmail', 'is_label': False, 'children': [], 'subtree_labels': [], 'parents': []}]


# TODO: to change once we fix to_json function
def test_to_json_labels_true_children(Node_PTC):
    node_1, node_2 = Node_PTC
    node_1.update_is_label(ids={1, 2})
    dj_children = node_1.to_json(only_labels=True)["children"]
    assert dj_children == [
        {'id': 2, 'title': 'Techmail', 'is_label': True, 'children': [], 'subtree_labels': [], 'parents': []}]


def test_can_predict_true(dummy_hierarchy_PTC):
    raw_classifier = DefaultClassifier(**default_params)
    full_classifier = ProductTreeClassifier(dummy_hierarchy_PTC[PRODUCT_ID_COLUMN], classifier=raw_classifier)
    node_1 = Node(1, name='Internal Systems', classifier=full_classifier)
    assert node_1.can_predict() is True


def test_can_predict_false():
    node_2 = Node(2, name='Techmail')
    assert node_2.can_predict() is False


def test_can_transform_true(transformer_fixture):
    featurizer = transformer_fixture
    node_1 = Node(1, name='Internal Systems', transformer=featurizer)
    assert node_1.can_transform() is True


def test_can_transform_false():
    node_2 = Node(2, name='Techmail', transformer=None)
    assert node_2.can_transform() is False


def test_can_predict_proba_true(dummy_hierarchy_PTC):
    raw_classifier = DefaultClassifier(**default_params)
    full_classifier = ProductTreeClassifier(dummy_hierarchy_PTC[PRODUCT_ID_COLUMN], classifier=raw_classifier)
    node_1 = Node(1, name='Internal Systems', classifier=full_classifier)
    assert node_1.can_predict_proba() is True


def test_can_predict_proba_false():
    full_classifier = LinearSVC()
    node_1 = Node(1, name='Internal Systems', classifier=full_classifier)
    assert node_1.can_predict_proba() is False


def test_add_parent():
    node_2 = Node(2, name="Techmail")
    node_3 = Node(3, name='Tech Issue')
    node_3.add_parent(node_2)
    assert node_3.parents_ids == [2]


# need to review if we need this function in the code
def test_get_parents_ids():
    node_1 = Node(1, name="Internal Systems")
    node_2 = Node(2, name="Techmail")
    node_2.add_parent(node_1)
    assert node_2.get_parents_ids() == [0, 1, 2]


def test_update_is_label():
    node_1 = Node(1, name="Internal Systems")
    node_1.update_is_label(ids={1})
    assert node_1.is_label is True


def test_update_is_label_child(Node_PTC):
    node_1, node_2 = Node_PTC
    node_1.update_is_label(ids={1, 2})
    assert node_1.is_label is True
    assert node_2.is_label is True


def test_update_labels():
    node_1 = Node(1, name="Internal Systems")
    node_1.update_is_label(ids={1})
    node_1.update_labels()
    assert node_1.subtree_labels == [1]


def test_update_labels_child(Node_PTC):
    node_1, node_2 = Node_PTC
    node_1.update_is_label(ids={1, 2})
    node_1.update_labels()
    assert node_1.subtree_labels == [1, 2]
    assert node_2.subtree_labels == [2]


def test_build_idx_positions_labels(train_test_split_fixture, Node_PTC):
    X_train, X_test, y_train, y_test = train_test_split_fixture
    root = Node(0, name='root')
    node_1, node_2 = Node_PTC
    root.add_child(node_1)
    root.update_is_label(ids={1, 2})
    root.update_labels()
    assert root._build_idx_positions_labels(y_train)[0] == [0, 3, 6, 7, 9, 10, 12, 15, 17, 18, 19, 21, 26, 27, 30, 31,
                                                            33, 34, 35, 38, 39, 40, 43, 44, 45, 47, 48, 49, 50, 51, 1,
                                                            2, 4, 5, 8, 11, 13, 14, 16, 20, 22, 23, 24, 25, 28, 29, 32,
                                                            36, 37, 41, 42, 46, 52, 53]

    assert_almost_equal(root._build_idx_positions_labels(y_train)[1],
                        [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0., 0, 0, 0, 0, 0, 0, 0,
                         0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0., 0, 0, 0, 0, 0, 0])


def test_fit_raise_value_error(processed_dummy_hierarchy_PTC):
    raw_classifier = DefaultClassifier(**default_params)
    full_classifier = ProductTreeClassifier(processed_dummy_hierarchy_PTC[PRODUCT_ID_COLUMN], classifier=raw_classifier)
    node_4 = Node(4, name='Troubleshooting', classifier=full_classifier, transformer=None)
    node_5 = Node(5, name='Tech systems', classifier=full_classifier, transformer=None)
    node_4.add_child(node_5)
    node_4.update_is_label(ids={5})
    node_4.update_labels()
    X = [np.str_('test'), np.str_('test2')]
    Y = pd.Series([1, 2])
    with pytest.raises(ValueError) as e:
        node_4.fit(X, Y, children=False)
    assert "Fit input is string, but no transformer has been provided." == str(e.value)


def test_predict_proba_raise_value_error(dummy_data_PTC):
    full_classifier = LinearSVC()
    node_1 = Node(1, name='Internal Systems', classifier=full_classifier)
    with pytest.raises(ValueError) as e:
        node_1.predict_proba(dummy_data_PTC)
    assert "The classifier provided can't predict_proba." == str(e.value)


def test_str_function(Node_PTC):
    node_1, node_2 = Node_PTC
    assert node_1.__str__() == 'Internal Systems 1: Techmail'


def test_remove_non_predictions():
    root_node = Node(0, name='root')
    node_1 = Node(1, name='Internal Systems')
    root_node.add_child(node_1)
    assert root_node.children_ids == {1}
    root_node.remove_non_predictions()
    assert root_node.children_ids == set()


def test_remove_non_predictions_children():
    root_node = Node(0, name='root')
    node_1 = Node(1, name='Internal Systems')
    node_2 = Node(2, name='Techmail')
    root_node.add_child(node_1)
    node_1.add_child(node_2)
    assert node_1.children_ids == {2}
    node_1.remove_non_predictions()
    assert node_1.children_ids == set()


def test_remove_non_predictions_false():
    root_node = Node(0, name='root')
    node_1 = Node(1, name='Internal Systems')
    root_node.add_child(node_1)
    node_2 = Node(2, name='Techmail')
    root_node.add_child(node_2)
    assert root_node.children_ids == {1, 2}
    root_node.update_is_label(ids={1})
    root_node.update_labels()
    assert root_node.subtree_labels == [1]
    root_node.remove_non_predictions()
    assert root_node.children_ids == {1}


def test_PTC_predict_ranking(processed_dummy_hierarchy_PTC, train_test_split_fixture, transformer_fixture):
    _, _, y_train, y_test = train_test_split_fixture
    X_trainf, X_testf = transformer_fixture
    raw_classifier = DefaultClassifier(**default_params)
    full_classifier = ProductTreeClassifier(processed_dummy_hierarchy_PTC[PRODUCT_ID_COLUMN], classifier=raw_classifier)
    full_classifier.fit(X_trainf, y_train)
    pred = full_classifier.predict(X_testf)
    ranked_pred = full_classifier.predict_ranking(X_testf, k=5)
    assert len(pred) == len(ranked_pred)
    for index in range(0, len(pred)):
        assert pred[index] == ranked_pred[index][0]


def test_PTC_evaluate_prediction_normal(processed_dummy_hierarchy_PTC, train_test_split_fixture, transformer_fixture):
    _, _, y_train, y_test = train_test_split_fixture
    X_trainf, X_testf = transformer_fixture
    raw_classifier = DefaultClassifier(**default_params)
    full_classifier = ProductTreeClassifier(processed_dummy_hierarchy_PTC[PRODUCT_ID_COLUMN], classifier=raw_classifier)
    full_classifier.fit(X_trainf, y_train)
    accuracy = full_classifier.predict_and_evaluate_ranking(X_testf, y_test.values, k=1)
    assert accuracy > 0.83


def test_build_dfs_rankings(processed_dummy_hierarchy_PTC, train_test_split_fixture, transformer_fixture):
    _, _, y_train, y_test = train_test_split_fixture
    X_trainf, X_testf = transformer_fixture
    raw_classifier = DefaultClassifier(**default_params)
    full_classifier = ProductTreeClassifier(processed_dummy_hierarchy_PTC[PRODUCT_ID_COLUMN], classifier=raw_classifier)
    full_classifier.fit(X_trainf, y_train)

    for sample in X_testf:
        dfs_ranking = []
        full_classifier.root_node.build_dfs_rankings(dfs_ranking, sample,
                                                     k=len(full_classifier.id2node) - 1, threshold=GDFS_THRESHOLD)
        for node in full_classifier.id2node.values():
            if node.id != -1:
                assert node.id in dfs_ranking
