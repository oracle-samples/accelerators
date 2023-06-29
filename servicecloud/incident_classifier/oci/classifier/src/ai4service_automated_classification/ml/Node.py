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
#  SHA1: $Id: c381f2b9b245b3719fb428f21585906de449fcde $
################################################################################################
#  File: Node.py
################################################################################################
"""
Node.

A Node represents a item of the hierarchy.
"""
# pylint: disable=attribute-defined-outside-init,too-many-instance-attributes,too-many-public-methods,too-many-arguments
import logging
import os
from typing import Optional, List, Dict, Union

import numpy as np
from pandas import Series
from sklearn.base import ClassifierMixin, clone
from sklearn.exceptions import NotFittedError
from sklearn.feature_extraction.text import _VectorizerMixin
from sklearn.model_selection import GridSearchCV

from ai4service_automated_classification.ml.util.classifier_util import (group_nodes,
                                                                             get_difference)
from ai4service_automated_classification.ml.util.confusion_util import merge_product_ids

logger = logging.getLogger(os.path.basename(__file__))


class Node:
    """
    A Node represents a item of the hierarchy.

    Each node by default has an untrained clone of the parent's
    classifier and settings related to grid search.
    A node also has a transformer, if the parent transformer is trained
    it uses the trained one, otherwise it trains an individual transformer
    for the data that is under its hierarchy.
    If no transformer is provided, it assumes the input data is already
    transformed.

    """

    def __init__(self,
                 nid: int,
                 name: str = 'no_name',
                 classifier: Optional[ClassifierMixin] = None,
                 transformer: Optional[_VectorizerMixin] = None,
                 grid_search: Optional[dict] = None,
                 grid_search_cv: Optional[object] = None,
                 grid_search_n_jobs: Optional[int] = 3) -> None:
        """
        Initialize the Node using the constructor function.

        @type nid: int
        @param nid: Node id
        @type name: str
        @param name: Node name
        @type grid_search: dict
        @param grid_search: grid search settings
        @type: int, cross-validation generator or an iterable
        @param grid_search_cv : sklearn-like CV parameter that determines
        the cross-validation splitting strategy for GridSearchCV.
        If None, it uses an automatic StratifiedKfold
        @type grid_search_n_jobs: int
        @param grid_search_n_jobs: number of parallel jobs for grid_search cross validation; to avoid oversubscription,
        make sure grid_search_n_jobs do not exceed the total number of available resources
        """
        self.children = []  # type: list
        self.parents = []  # type: list
        self.children_ids = set()  # type: set
        # all labels from self to leaves (is_label)
        self.subtree_labels = []  # type: list
        self.parents_ids = []  # type: list
        self.id = nid
        self.child_idx_pred = -1  # child index for prediction
        self.name = name
        self.is_label = False
        self.grid_search_n_jobs = grid_search_n_jobs
        # no of samples used to training_scripts the node to get a sorted list of the parent’s children, in order of probability
        self.sample_size = -1

        # ### classifier stuff ###
        self.grid_search = grid_search
        self.grid_search_cv = grid_search_cv
        if classifier:
            self.classifier = clone(classifier)
        if transformer:
            self.transformer = transformer

    def add_child(self, child: 'Node') -> None:
        """
        Add a child node.

        @type child: Node
        @param child: Node
        """
        if child.id not in self.children_ids and child.id != self.id:
            self.children.append(child)
            self.children_ids.add(child.id)

    def add_parent(self, parent: 'Node') -> None:
        """
        Add a parent node.

        @type parent: Node
        @param parent: Node
        """
        self.parents.append(parent)
        self.parents_ids.append(parent.id)

    def is_leaf(self) -> bool:
        """
        Check if node is leaf.

        @rtype: bool
        @return: True or False
        """
        if len(self.children) == 0:
            return True
        return False

    def get_subproducts(self, products: list) -> None:
        """
        Create a list of products that are below this node's hierarchy.

        @type products: list
        @param products: it's used as return value
        """
        products.append(self.id)
        for child in self.children:
            child.get_subproducts(products)

    def to_json(self, only_labels: bool = True) -> dict:
        """
        Generate json string compatible with ojet tree hierarchies.

        @type only_labels:  bool
        @param only_labels: generate tree hierarchy from all the prediction labels or the entire hierarchy regardless
                            of the trained classifier (Default value = True)
        @rtype: dict
        @return: json hierarchy as dict
        """
        dj = {
            'id': self.id,
            'title': self.name,
            'is_label': self.is_label
        }  # type: dict
        if self.has_scores_computed():
            cnf = dict()  # type: dict
            cnf['h_score'] = self.hierarchical_accuracy
            cnf['h_test_incidents'] = int(self.test_hierarchical_incidents)
            cnf['o_score'] = self.original_accuracy
            cnf['o_test_incidents'] = int(self.original_hierarchical_incidents)
            dj['prediction_info'] = cnf
        dj['children'] = []
        dj['subtree_labels'] = self.subtree_labels
        dj['parents'] = list(self.parents_ids)
        for child in self.children:
            if only_labels:
                if child.is_label:
                    dj['children'].append(child.to_json())
            else:
                dj['children'].append(child.to_json())
        return dj

    def can_predict(self) -> bool:
        """
        Look for the predict method on Node.

        For leaf nodes we should have the predict method.

        @rtype: bool
        @return: bool -- True or False
        """
        try:
            getattr(self, "classifier")
            return True
        except AttributeError:
            return False

    def has_scores_computed(self) -> bool:
        """
        Check if it has scores computed.

        For confusion matrix purposes, we can use a classifier
        to compute the scores it gets on a test set. In case we
        call compute_scores, it will create some extra parameters
        that can be json serialized to be displayed in the Admin UI.
        Also we can use confusion information to adjust the probability
        or confidence score for the current prediction. In some cases
        it might be useful to return all the likely predictions.

        @rtype: bool
        @return: True, if compute scores has been called, else False
        """
        try:
            getattr(self, "h_score")
            getattr(self, "h_test_incidents")
            getattr(self, "o_score")
            getattr(self, "o_test_incidents")
            return True
        except AttributeError:
            return False

    def is_fitted_transformer(self) -> bool:
        """
        Check if the transformer is already fitted.

        Checks if the transformer is already fitted, in which case
        it will not refit on the current node's data.

        @rtype: bool
        @return: bool
        """
        try:
            self.transformer.transform(Series(['dummy']))
            return True
        except NotFittedError:
            return False

    def can_predict_proba(self) -> bool:
        """
        Check if the classifier has predict_proba function.

        Checks if the classifier has predict_proba or if it can
        be used to call the predict proba methods.

        @rtype: bool
        @return: bool
        """
        try:
            getattr(self.classifier, "predict_proba")
            return True
        except AttributeError:
            return False

    def can_transform(self) -> bool:
        """
        Check if there is a transformer.

        If not, it will expect the data to be already transformed.

        @rtype: bool
        @return: bool
        """
        try:
            getattr(self, "transformer")
            return True
        except AttributeError:
            return False

    def fit(self, X: np.ndarray, y: np.ndarray, children: bool = True) -> None:
        """
        Fit method for the classifier.

        @type X: array
        @param X: array-like, sparse matrix of shape (n_samples, n_features)
            Training vector, where n_samples in the number of samples and
            n_features is the number of features. Can be string data if a
            transformer is provided.
        @type y: array
        @param y: array-like of shape (n_samples,). Target vector relative to X.
        @type children: bool
        @param children: bool -- if children is set it, also trains the children
            of the classifier; useful for parallelization where we don't want
            children to be trained recursively
        @raise ValueError: if the input data is text and no transformer has been provided it will raise an exception

        """
        if self.is_leaf() or not self.subtree_labels:
            self.sample_size = len(self._build_idx_positions_labels(y)[0])
            return
        logger.info(f"Fitting node with id '{self.id}' and name '{self.name}'")
        current_data = X
        transformed = False
        if self.can_transform():
            logger.info("Transforming...")
            if not self.is_fitted_transformer():
                self.transformer = clone(self.transformer)
                current_data = self.transformer.fit_transform(X)
            else:
                current_data = self.transformer.transform(X)
            transformed = True
        elif isinstance(X[0], np.str_):
            raise ValueError(
                'Fit input is string, but no transformer has been provided.')

        positions, labels = self._build_idx_positions_labels(y)
        unq_labels, cnt_labels = np.unique(labels[positions], return_counts=True)
        self.sample_size = len(positions)
        if len(unq_labels) == 1:
            logger.debug('Final label, nothing to fit.')
            self.child_idx_pred = 0
        else:
            logger.info(f"The total number of examples is: '{len(positions)}' across '{len(unq_labels)}' classes.")
            logger.debug(f"Label distribution is: '{cnt_labels}'")
            self.fit_no_transform(current_data[positions], labels[positions])
        # if it was transformed, revert to initial untransformed text
        if transformed:
            current_data = X.reset_index(drop=True)  # type: ignore
        if children:
            # TODO: comment the subchild fit and paralellize
            # on the nodelist using id2node
            for child in self.children:
                child.fit(current_data[positions], y[positions])

    def fit_no_transform(self, X: np.ndarray, y: np.ndarray) -> None:
        """
        Fit function, similar to fit.

        @type X: array
        @param X: array-like, sparse matrix of shape (n_samples, n_features)
            Training vector, where n_samples in the number of samples and
            n_features is the number of features. It can only process
            numerical data.
        @type y: array
        @param y: array of shape (n_samples,). Target vector relative to X.

        """
        if self.grid_search and self.grid_search_cv:
            logger.info("Running Grid Search...")
            self.classifier = GridSearchCV(self.classifier,
                                           self.grid_search,
                                           cv=self.grid_search_cv,
                                           n_jobs=self.grid_search_n_jobs)
        self.classifier.fit(X, y)

    def _build_idx_positions_labels(self, y: np.ndarray) -> tuple:
        """
        Build the indices/positions of the products that are within its hierarchy.

        @type y: array
        @param y: array of shape (n_samples,)
        @rtype: tuple
        @return: positions of the products in input and the labels for each subchild
        """
        labels = np.zeros(len(y))
        positions = []  # type: list
        if self.is_label:
            current_positions = np.where(y == self.id)[0].tolist()
            labels[current_positions] = len(self.children)
            positions.extend(current_positions)

        for child_nr, child in enumerate(self.children):
            child_positions = []  # type: list
            for subid in child.subtree_labels:
                current_positions = np.where(y == subid)[0].tolist()
                child_positions.extend(current_positions)
                positions.extend(current_positions)
            labels[child_positions] = child_nr

        return positions, labels

    def predict_one(self,
                    data_point: np.ndarray,
                    h_explanations: Optional[List] = None,
                    proba: Optional[Dict[int, str]] = None
                    ) -> Union[str, int]:
        """
        Predict one datapoint.

        @type data_point: string|array
        @param data_point: string or array
        @type h_explanations: list
        @param h_explanations: return argument list to explain the
            decisions made by each classifier at each step in the
            hierarchy; currently it only saves the hierarchical path, best
            not to use it yet. (Default value: None)
        @type proba: dict
        @param proba: return argument dict to save the predict_proba
            values at each step, in case predict proba is available for
            the classifier of the nodes (Default value = None)
        @rtype: str|int
        @return: label -- the prediction value of the final leaf

        """
        logger.debug(f"Predicting node with id '{self.id}' and name '{self.name}'")
        if self.is_leaf() and self.is_label:
            if isinstance(h_explanations, list):
                h_explanations.append(self.id)
            return self.id

        if self.child_idx_pred >= 0:
            return self.children[self.child_idx_pred].predict_one(data_point, h_explanations, proba)
        if len(self.subtree_labels) == 1:
            if isinstance(h_explanations, list):
                h_explanations.append(self.subtree_labels[0])
            return self.subtree_labels[0]

        if self.can_transform():
            data_point_pred = self.transformer.transform(Series([data_point]))
            data_point_pred = data_point_pred.reshape(1, -1)
        else:
            data_point_pred = data_point.reshape(1, -1)

        if isinstance(proba, dict):
            pred_proba = self.classifier.predict_proba(data_point_pred)
            predicted = int(np.argmax(pred_proba))
            proba[self.id] = pred_proba.tolist()
        else:
            predicted = int(self.classifier.predict(data_point_pred)[0])
        if self.is_label and predicted == len(self.children):
            if isinstance(h_explanations, list):
                h_explanations.append(self.id)
            return self.id
        child = self.children[predicted]
        if isinstance(h_explanations, list):
            h_explanations.append(self.id)
        return child.predict_one(data_point, h_explanations, proba)

    def predict(self, data: np.ndarray) -> list:
        """
        Predict function, ScikitLearn-like, calls predict_one on each datapoint. Can be made embarrassingly parallel.

        @type data: array
        @param data: depending on the transformer, it can also accept dataframes, etc.
        @rtype: list
        @return: list of predictions for each datapoint
        """
        logger.info("Predicting...")
        return_value: List[Union[str, int, None]] = [None] * data.shape[0]
        for idx in range(0, data.shape[0]):
            return_value[idx] = self.predict_one(data[idx], None, None)
        return return_value

    def predict_proba(self, data: np.ndarray) -> dict:
        """
        Call predict_proba on data, if it's available at the current nodes' classifier.

        @type data: array
        @param data: depending on the transformer, it can also accept dataframes, etc.
        @rtype: dict
        @return: dict -- 3 values - 1) the predictions, 2) the probabilities
            at each level for all the candidates, 3) the probabilities at
            each level for the best candidate that was used to make
            the final judgement.
        @raise ValueError: if we can't predict_proba, it's an error

        """
        if not self.can_predict_proba():
            raise ValueError("The classifier provided can't predict_proba.")
        logger.info("Predicting proba...")
        retval: List[Union[str, int, None]] = [None] * data.shape[0]
        cumulative_p = [None] * data.shape[0]
        probas: List[Optional[dict]] = [None] * data.shape[0]
        for idx in range(0, data.shape[0]):
            proba = dict()  # type: dict
            retval[idx] = self.predict_one(data[idx], None, proba)
            probas[idx] = proba
            avg_probability = []
            for vals in list(proba.values()):
                avg_probability.append(np.max(vals))
            cumulative_p[idx] = np.mean(avg_probability).tolist()
        result = {
            "prediction": retval[0] if len(retval) == 1 else retval,
            "confidenceScore": cumulative_p[0] if len(cumulative_p) == 1 else cumulative_p,
            # "bestCandidatesProbabilities": probas  #  this is commented out intentionally to only return the
            # confidence score alongside the prediction accuracy. https://jira.oraclecorp.com/jira/browse/LV-11062
        }
        return result

    def predict_ranking(self, data: np.ndarray,
                        threshold: float = 0.01, k: int = 5) -> List[List[int]]:
        """
        Call predict_ranking on data, if it's available at the current nodes' classifier.

        @type data: array of arrays | csr_matrix
        @param data: array/batch of transformed samples
        @param threshold: to group two nodes which have difference in probability based on threshold
        @param k: The number of node ids to retrieve in ranking. By default, it is 5
        @rtype: List
        @return: Predictions list according to ranking position.
        @raise ValueError: if we can't predict_ranking, it's an error

        """
        if not self.can_predict_proba():
            raise ValueError("The node provided can't predict_proba.")

        ret_val: List[List[int]] = [[0]] * data.shape[0]

        for idx in range(0, data.shape[0]):
            dfs_rankings: List[int] = []
            self.build_dfs_rankings(dfs_rankings, data[idx], k=k, threshold=threshold)
            ret_val[idx] = dfs_rankings[:k]
        return ret_val

    def build_dfs_rankings(self, dfs_rankings: List[int], data: np.ndarray,
                           k: int = 5, threshold: float = 0.01) -> List[int]:
        """
        Get DFS_ranking and BFS_ranking of all children of given node.

        @param dfs_rankings DFS ranking based on given node's sorted predict_proba
        @param data: array/batch of transformed samples
        @param k: The number of node ids to retrieve in ranking. By default, it is 5
        @param threshold to group two nodes which have difference based on threshold
        @rtype: tuple
        @return: tuple list of GDFS rankings of given node.

        """
        if len(dfs_rankings) >= k:
            return dfs_rankings

        if self.is_leaf() and self.is_label:
            dfs_rankings.append(self.id)
            return dfs_rankings

        if self.child_idx_pred >= 0:
            self.children[self.child_idx_pred].build_dfs_rankings(dfs_rankings, data, k, threshold)
            return dfs_rankings

        if len(self.subtree_labels) == 1:
            dfs_rankings.append(self.subtree_labels[0])
            return dfs_rankings

        data_point_pred = data.reshape(1, -1)

        probas = self.classifier.predict_proba(data_point_pred)[0]
        predicted = np.argmax(probas)

        if self.is_label and predicted == len(self.children):
            dfs_rankings.append(self.id)

        if self.is_label and len(probas) > len(self.children):
            probas = probas[:-1]

        top_predictions = np.argsort(-probas)
        difference = get_difference(probas, top_predictions, threshold)
        # TODO Created LV-15746 for considering difference on all nodes

        top_children = [self.children[pred] for pred in top_predictions]
        no_of_grp_nodes = 0
        if difference <= threshold and len(self.children) > 1:
            logger.debug(f"Grouping top 2 prediction nodes and processing them together, because their probability "
                         f"difference is less than {threshold}")
            group_nodes(data, top_children, dfs_rankings, threshold, k)
            no_of_grp_nodes = 2

        for child in top_children[no_of_grp_nodes:]:  # getting the rank of pending nodes
            child.build_dfs_rankings(dfs_rankings, data, k, threshold)

        if self.id != -1 and self.id not in dfs_rankings and len(dfs_rankings) < k:
            dfs_rankings.append(self.id)

        return dfs_rankings

    def explain(self, data: np.ndarray) -> tuple:
        """
        Explain predictions.

        TODO: explain should return more informative stuff at each level; try ktrain library

        @type data: array
        @param data: depending on the transformer, it can also accept dataframes, etc.
        @rtype: tuple
        @return: tuple -- 1) the predictions, 2) the explanations which currently
            consist only on the hierarchical path.

        """
        logger.info("Predicting and explaining...")
        retval: List[Union[str, int, None]] = [None] * data.shape[0]
        hierarchy: List[Optional[list]] = [None] * data.shape[0]
        for idx in range(0, data.shape[0]):
            current_h = []  # type: list
            retval[idx] = self.predict_one(data[idx], current_h, None)
            hierarchy[idx] = current_h
        return retval, hierarchy

    def get_parents_ids(self) -> list:
        """
        Get the parent ids of the node.

        @rtype: list
        @return: list of parent ids

        """
        ids = [self.id]
        for pid in self.parents_ids[::-1]:
            ids.insert(0, pid)
        if ids[0] != 0:
            ids.insert(0, 0)
        return ids

    def __str__(self) -> str:
        """
        Convert node to string.

        @rtype: str
        @return: name: [child1, ..., childn]
        """
        return self.name + " " + str(self.id) + ": " + ", ".join(str(x.name) for x in self.children)

    def update_is_label(self, ids: Union[dict, set]) -> None:
        """
        Update which nodes, including children, are labels.

        Method to update which nodes, including children,
        are labels to be predicted or not. It needs a dictionary
        or set that contains the ids that are labels.

        @type ids: dict or set
        @param ids: which ids are labels
        """
        if self.id in ids:
            self.is_label = True
        else:
            self.is_label = False
        for child in self.children:
            child.update_is_label(ids)

    def update_labels(self) -> None:
        """
        Update node's list of subtree labels.

        A node stores all the subtree labels in a list.
        When we know which nodes are labels and which aren't, we
        update the lists of subtree labels for each node.

        """
        if self.is_label:
            self.subtree_labels.append(self.id)
        children_labels = []  # type: list
        for child in self.children:
            child.update_labels()
            children_labels.extend(child.subtree_labels)
        self.subtree_labels.extend(children_labels)

    def remove_non_predictions(self) -> None:
        """
        Remove all subchildren not used for predictions.

        This one removes all the subchildren that will not
        be used to make predictions. It's not usefull to keep nodes
        for the entire product hierarchy that will not be used for
        classification.

        """
        idx = len(self.children) - 1
        while idx >= 0:
            child = self.children[idx]
            if not child.is_label and not child.subtree_labels:
                self.children_ids.remove(child.id)
                del self.children[idx]
            idx -= 1
        for child in self.children:
            child.remove_non_predictions()

    # TODO: to be used in build_model.evaluate method
    def compute_scores(self, conf_mat: np.ndarray, id2idx: dict) -> None:
        """
        Compute prediction scores from a given confusion matrix.

        Computes prediction scores given a confusion matrix and
        a dictionary of node id to confusion matrix position. It creates
        new attributes in the object:
        - h_score - hierarchical accuracy score
        - h_test_incidents - hierarchical number of test incidents
        - o_score - original accuracy score (if the node is label, otherwise 0)
        - o_test_incidents - original number of test incidents (if the node is
        label, otherwise 0)
        TODO: make this method create a separate cloned object that contiains
        only the confusion info to avoid not defining new attributes outside __init__.

        @type conf_mat: array
        @param conf_mat: confusion matrix
        @type id2idx: dict
        @param id2idx: node-id to confusion matrix index position

        """
        # TODO: the score is not really the score
        # it's the raw number of correctly classifierd incidents
        # if you want the score, do: self.h_score/ self.h_test_incidents * 100
        self.hierarchical_accuracy = 0.0
        self.test_hierarchical_incidents = 0
        self.original_accuracy = 0.0
        self.original_hierarchical_incidents = 0
        score_list = []
        test_incidents_list = [0]
        if self.is_label:
            pos = id2idx.get(self.id, -1)
            # TODO: treat the other case when it's -1
            if pos > 0:
                correctly_classified = conf_mat[pos, pos]
                score_list.append(correctly_classified)
                self.original_accuracy = correctly_classified
                nr_incidents = np.sum(conf_mat[pos, :])
                self.original_hierarchical_incidents = nr_incidents
                test_incidents_list.append(nr_incidents)
        for child in self.children:
            child.compute_scores(conf_mat, id2idx)
            if child.is_label:
                score_list.append(child.hierarchical_accuracy)
                test_incidents_list.append(child.test_hierarchical_incidents)
        self.hierarchical_accuracy = np.mean(score_list) if score_list else 0.0
        self.test_hierarchical_incidents = np.sum(test_incidents_list)
        # TODO: this one can also be replaced by self.subtree_labels
        childred_ids_with_labels = [child.id for child in self.children if child.is_label]
        if len(childred_ids_with_labels) > 0:
            conf_mat = merge_product_ids(conf_mat, childred_ids_with_labels, id2idx)
            id2idx[self.id] = id2idx[np.min(childred_ids_with_labels)]
