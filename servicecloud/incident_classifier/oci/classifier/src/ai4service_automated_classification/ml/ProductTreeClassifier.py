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
#  SHA1: $Id: 09ce05b6b0616865134437d0d9c8654523c5b257 $
################################################################################################
#  File: ProductTreeClassifier.py
################################################################################################
"""
ProductTreeClassifier.

ProductTreeClassifier module has everything required to build a classifier using a pre-defined product tree
hierarchy.
"""
# pylint: disable=attribute-defined-outside-init,too-many-instance-attributes,too-many-public-methods,too-many-arguments

import json
import logging
import os
from typing import Optional, List

import numpy as np
from pandas import DataFrame, Series
from sklearn.base import ClassifierMixin, BaseEstimator, TransformerMixin
from sklearn.feature_extraction.text import _VectorizerMixin
from sklearn.metrics import accuracy_score, confusion_matrix

from ai4service_automated_classification.constants import ID_COLUMN, LVL_1
from ai4service_automated_classification.ml.Node import Node
from ai4service_automated_classification.ml.util.classifier_util import (DefaultClassifier,
                                                                             default_params,
                                                                             accuracy_at_K, assert_first_rank)
from ai4service_automated_classification.ml.util.util import build_id2idx, build_id2name

logger = logging.getLogger(os.path.basename(__file__))


class ProductTreeClassifier(BaseEstimator, TransformerMixin):
    """
    Product Tree Classifier.

    Object that is used to instantiate a classifier on the product hierarchy.
    In terms of final product accuracy, the classifier is not significantly
    better than the LogisticRegression, however the confusions are restricted
    between different siblings on the tree. It also has more mechanisms that can
    be used to switch back on a higher level node and to follow the likelihood
    scores at each level.

    """

    def __init__(self,
                 hierarchy: DataFrame,
                 classifier: Optional[ClassifierMixin] = None,
                 transformer: Optional[_VectorizerMixin] = None,
                 grid_search: Optional[dict] = None,
                 grid_search_cv: Optional[object] = None,
                 grid_search_n_jobs: Optional[int] = 3,
                 filter_dict: Optional[dict] = None) -> None:
        """
        Constructor.

        @type hierarchy: L{pandas.DataFrame}
        @param hierarchy: contains the exported product hierarchy table.

        @type classifier: sklearn-like classifier
        @param classifier: classifier to be used at each node; if no classifier is provided it uses the
                           DefaultClassifier from classifier_util
        @type transformer: sklearn-like transformer
        @param transformer: transformer to be used at each node; if the transformer is fit, it uses it as it;
                            otherwise it fits a transformer at each node in the tree; if nothing is provided,
                            it expects data that is already transformed
        @type grid_search: dict
        @param grid_search: depending on the classifier, grid search params
        @type: int, cross-validation generator or an iterable, default None
        @param grid_search_cv : sklearn-like CV parameter that determines
        the cross-validation splitting strategy for GridSearchCV.
        If None, it uses an automatic StratifiedKfold
        @type grid_search_n_jobs: int
        @param grid_search_n_jobs: number of parallel jobs for grid_search cross validation; to avoid oversubscription,
        make sure grid_search_n_jobs do not exceed the total number of available resources
        @type filter_dict: dict
        @param filter_dict: contains the product_id to name information from the training data to filter out the
                            products from the entire hierarchy
        """
        if classifier is None:
            classifier = DefaultClassifier(**default_params)
        self.transformer = transformer
        self.classifier = classifier

        if grid_search is not None and grid_search_cv is None:
            raise ValueError(
                f'Got grid search parameters: {grid_search}, and no grid search method.')
        if grid_search_cv is not None and grid_search is None:
            raise ValueError(
                f'Got grid search method: {grid_search_cv}, and no grid search parameters.')

        self.grid_search = grid_search
        self.grid_search_cv = grid_search_cv
        self.grid_search_n_jobs = grid_search_n_jobs
        self.hierarchy = hierarchy
        self.filter_dict = filter_dict

    def dump_json(
            self,
            file: str = 'tree.json',
            only_labels: bool = True,
            include_root: bool = False) -> None:
        """
        Product tree hierarchy dump json.

        @type file: str
        @param file: path to file where to save the json
        @type only_labels: bool
        @param only_labels: if set to false it can dump the products that are not prediction labels in the training data
        @type include_root: bool
        @param include_root: in case one might want to have the root node (0) included in the json

        @raise ValueError: If the tree has not been built or fitted there's no json to dump.
        """
        if not self.is_built():
            raise ValueError(
                "Internal Tree is not built. Call fit or build_product_tree.")
        dict_json = []
        if include_root:
            dict_json.append(self.root_node.to_json(only_labels))
        else:
            for node in self.root_node.children:
                if only_labels and (
                        not node.is_label and not node.subtree_labels):
                    continue
                dict_json.append(node.to_json(only_labels))
        with open(file, 'w') as fout:
            json.dump(dict_json, fout, indent=4)

    def is_built(self) -> bool:
        """
        Check if the tree has been built.

        @rtype: bool
        @return: bool
        """
        try:
            getattr(self, "root_node")
            return True
        except AttributeError:
            return False

    def __build_root_node(self, roots: list) -> Node:
        """
        Create artificial root node using all top level products.

        This one creates an artificial root node using all
        the top level products. Make sure the hierarchy doesn't
        have id=0.

        @rtype:  Node
        @return: root node of all products.
        """
        parent_of_all = Node(-1, 'root',
                             classifier=self.classifier,
                             transformer=self.transformer,
                             grid_search=self.grid_search,
                             grid_search_cv=self.grid_search_cv,
                             grid_search_n_jobs=self.grid_search_n_jobs)
        parent_of_all.is_label = False
        parent_of_all.children = roots
        parent_of_all.children_ids = set([root.id for root in roots])
        for root in roots:
            root.add_parent(parent_of_all)
        self.id2node[0] = parent_of_all
        return parent_of_all

    def build_product_tree(self, y: np.ndarray) -> None:
        """
        Build the product tree using the prediction labels.

        This one creates attributes for the object:
        - root_node - the root node
        - id2node - a dictionary that stores pairs of node ids to node object
        for fast retrieval.

        @type y: array
        @param y: array of target labels to know which products should be used for prediction

        """
        label_ids = set(list(y))
        id2name = build_id2name(self.hierarchy, self.filter_dict)
        id2node = dict()  # type: dict
        roots = []
        for _, row in self.hierarchy.iterrows():
            current_child = row[ID_COLUMN]
            current_parent = row[LVL_1]

            if current_parent in id2node:
                parent = id2node[current_parent]
            else:
                name = id2name.get(current_parent, str(current_parent))
                parent = Node(current_parent,
                              name=name,
                              classifier=self.classifier,
                              transformer=self.transformer,
                              grid_search=self.grid_search,
                              grid_search_cv=self.grid_search_cv,
                              grid_search_n_jobs=self.grid_search_n_jobs)
                id2node[current_parent] = parent

            if current_parent == current_child:
                roots.append(parent)

            for child_id in row[3:]:
                if child_id == 0:
                    break
                name = id2name.get(child_id, str(child_id))
                if child_id in id2node:
                    child = id2node[child_id]
                else:
                    child = Node(child_id, name=name,
                                 classifier=self.classifier,
                                 transformer=self.transformer,
                                 grid_search=self.grid_search,
                                 grid_search_cv=self.grid_search_cv,
                                 grid_search_n_jobs=self.grid_search_n_jobs)
                    id2node[child_id] = child
                parent.add_child(child)
                parent = child

        for _, row in self.hierarchy.iterrows():
            child_id = row[ID_COLUMN]
            for parent_id in row[2:]:
                if parent_id in (0, child_id):
                    break
                id2node[child_id].add_parent(id2node[parent_id])
        self.id2node = id2node
        self.root_node = self.__build_root_node(roots)
        self.root_node.update_is_label(label_ids)
        self.root_node.update_labels()
        self.root_node.remove_non_predictions()

    def compute_scores_conf(self, conf_mat: np.ndarray, y_test: np.ndarray) -> None:
        """
        Compute confusion scores on each hierarchical node using a confusion matrix and the actual labels/targets.

        Set confusion scores on each node (hierarchically) using a confusion matrix
        computed externally and the y_test. Note, this method requires the labels in
        y_test to be found in the training set, otherwise indices can get mixed.

        @type conf_mat: array
        @param conf_mat: confusion matrix computed externally
        @type y_test: array
        @param y_test: target labels, gold standard predictions
        @raise ValueError: If the tree has not been built or fitted there's no classifier.

        """
        if not self.is_built():
            raise ValueError(
                "Internal Tree is not built. Call fit or build_product_tree.")
        pred_id2idx = build_id2idx(y_test)
        self.root_node.compute_scores(conf_mat, pred_id2idx)

    def score_predict(self, X_test: np.ndarray, y_test: np.ndarray) -> tuple:
        """
        Get the accuracy score, predictions and confusion matrix from a test set.

        If we have trained the classifier, we can get the accuracy score, predictions
        and confusion matrix from a test set

        @type X_test: array
        @param X_test: must have the same format as the data used for training
        @type y_test: array
        @param y_test: gold standard values for the X_test
        @rtype: tuple
        @return: (accuracy, predictions, confusion matrix)

        """
        preds = self.predict(X_test)
        acc = accuracy_score(y_test, preds)
        conf_mat = confusion_matrix(y_test, preds)
        return acc, preds, conf_mat

    def compute_scores(self, X_test: np.ndarray, y_test: np.ndarray) -> tuple:
        """
        Set the hierarchical test scores on each node.

        Requires y_test to contain the same labels as the trained data.

        @type X_test: array
        @param X_test: must be the same format as training data
        @type y_test: array
        @param y_test: prediction labels
        @rtype: tuple
        @return: (accuracy, predictions, confusion matrix)

        """
        if not self.is_built():
            raise ValueError(
                "Internal Tree is not built. Call fit or build_product_tree.")
        pred_id2idx = build_id2idx(y_test)
        acc, preds, conf_mat = self.score_predict(X_test, y_test)
        self.root_node.compute_scores(conf_mat, pred_id2idx)
        return acc, preds, conf_mat

    def fit(self, X: np.ndarray, y: Series) -> None:
        """
        Fit function for each classifier in each node.

            X -- array-like, sparse matrix of shape (n_samples, n_features)
            Can be string data, if a transformer is provided.
            y -- array-like of shape (n_samples,)
            Target vector relative to X.

        @type X: array
        @param X: sparse matrix of shape (n_samples, n_features). Can be string data, if a transformer is provided.
        @type y: array
        @param y: Target vector relative to X of shape (n_samples,).

        """
        self.build_product_tree(y.values)
        # TODO: here we can do paralellization
        # if set children=false and call for each subtree
        # node that can be stored in a list
        self.root_node.fit(X, y.values)

    def predict(self, data: np.ndarray) -> list:
        """
        Predict on new data.

        @type data: array
        @param data: depending on the transformer, it can also accept dataframes, etc.
        @rtype: array
        @return: final prediction values for each element in the data
        @raise ValueError: If the tree has not been built or fitted there's no classifier.

        """
        if not self.is_built():
            raise ValueError("Internal Tree is not built. Call fit first.")
        return self.root_node.predict(data)

    def predict_proba(self, data: np.ndarray) -> dict:
        """
        Call predict_proba on data, if it's available for the classifier.

        @type data: array
        @param data: depending on the transformer, it can also accept dataframes, etc.
        @rtype: dict
        @return: dict -- 3 values - 1) the predictions, 2) the probabilities
            at each level for all the candidates, 3) the probabilities at
            each level for the best candidate that was used to make
            the final judgement.
        @raise ValueError: if we can't predict_proba, it's an error

        """
        if not self.is_built():
            raise ValueError("Internal Tree is not built. Call fit first.")
        return self.root_node.predict_proba(data)

    def predict_ranking(self, data: np.ndarray,
                        threshold: float = 0.01, k: int = 5) -> List[List[int]]:
        """
        Call predict_ranking on data, if it's available for the classifier.

        @type data: array
        @param data: array/batch of transformed samples, etc.
        @param threshold to group two nodes which have difference in probability based on threshold
        @param k: The number of node ids to retrieve in ranking. By default, it is 5
        @rtype: List
        @return: nearby rankings according to probabilities of given node.
        @raise ValueError: if we can't predict_proba, it's an error

        """
        if not self.is_built():
            raise ValueError("Internal Tree is not built. Call fit first.")
        return self.root_node.predict_ranking(data, threshold, k)

    def predict_and_evaluate_ranking(self, data: np.ndarray, target_labels: List[int],
                                     threshold: float = 0.1, k: int = 5) -> float:
        """
        Call predict_ranking on data, if it's available for the classifier.

        @param data: array/batch of transformed samples, etc.
        @param target_labels: actual target ids for given sample data
        @param threshold: to group two nodes which have difference based on threshold
        @param k: find distance score for k labels
        @rtype: float
        @return: accuracy at k

        """
        rankings = self.predict_ranking(data, threshold=threshold, k=k)
        assert_first_rank(self.predict(data), rankings)
        return accuracy_at_K(rankings, target_labels, k)

    def explain(self, data: np.ndarray) -> tuple:
        """
        Explain function to explain predictions.

        TODO: explain should return more informative stuff at each leve,
        try ktrain library

        @type data: array
        @param data: depending on the transformer, it can also accept dataframes, etc.
        @rtype: tuple
        @return: tuple -- 1) the predictions, 2) the explanations which currently
            consist only on the hierarchical path.
        @raise ValueError: If the tree has not been built or fitted there's no classifier.

        """
        if not self.is_built():
            raise ValueError("Internal Tree is not built. Call fit first.")
        predictions, hierarchy = self.root_node.explain(data)
        return predictions, hierarchy
