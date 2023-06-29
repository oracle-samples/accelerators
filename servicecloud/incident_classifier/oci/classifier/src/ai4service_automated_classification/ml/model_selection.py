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
#  SHA1: $Id: 72bb99fe507969cc2902c74f1c2202808d50cdd2 $
################################################################################################
#  File: model_selection.py
################################################################################################
"""Model selection classes for training_scripts test splits.

The `model_selection` module includes classes and
functions to split the data based on a preset strategy and
it is strongly based on scikit-learn patterns.
"""

import logging
import os
from typing import Generator, Optional
import numpy as np
from scipy.special import softmax
from sklearn.model_selection import StratifiedKFold
from sklearn.model_selection._split import _BaseKFold
from sklearn.utils import indexable
from sklearn.utils.multiclass import type_of_target
from sklearn.utils.validation import check_array
from sklearn.utils.validation import column_or_1d

logger = logging.getLogger(os.path.basename(__file__))


class TemporalStratifiedSplit:
    """Stratified training_scripts test splits based on the indices of the data.

    A sklearn-like class that creates a training_scripts-test split based
    in a simple manner using a constant percentage from all classes.
    The splitter expects data order ascending by date since it prioritizes
    in the test set the most recent elements (or the ones at the end of the list).
    """

    def __init__(self, test_ratio: float = 0.1):
        """Constructor, expects the test_ratio that will be used to create the test set.

        @type test_ratio: float
        @param test_ratio: Default value 0.1
        """
        self.test_ratio = test_ratio
        self.n_splits = 1

    def get_n_splits(self, X: Optional[np.ndarray] = None,
                     y: Optional[np.ndarray] = None,
                     groups: Optional[np.ndarray] = None) -> int:
        """Numbers the splitting iterations in the cross-validator.

        @type X: object
        @param X: Always ignored, exists for compatibility.

        @type y: object
        @param y: Always ignored, exists for compatibility.

        @type groups: array-like, with shape (n_samples,)
        @param groups: Always ignored, exists for compatibility.

        @rtype n_splits : int
        @return: the number of splitting iterations in the cross-validator.
        ----
        This particular splitter always returns one split.
        """
        return self.n_splits

    def split(self, X: Optional[np.ndarray],
              y: Optional[np.ndarray] = None,
              groups: Optional[np.ndarray] = None) -> Generator[tuple, None, None]:
        """Generate indices to split data into training and test set.

        @type X: array-like, shape (n_samples, n_features)
        @param X: Training data, where n_samples is the number of samples
            and n_features is the number of features.
            Note that providing ``y`` is sufficient to generate the splits and
            hence ``np.zeros(n_samples)`` may be used as a placeholder for
            ``X`` instead of actual training data.

        @type y : array-like, shape (n_samples,)
        @param y: The target variable for supervised learning problems.
            Stratification is done based on the y labels.

        @type groups: object
        @param groups: Always ignored, exists for compatibility.

        @rtype: tupple
        @return:
            training_scripts : ndarray - The training set indices for that split.
            test : ndarray - The testing set indices for that split.
            if no split is possible, then it doesn't yield anything.
        """
        y = np.asarray(y)
        X, y, groups = indexable(X, y, groups)
        self.n_splits = self.get_n_splits(X, y, groups)
        type_of_target_y = type_of_target(y)
        allowed_target_types = ('binary', 'multiclass')
        if type_of_target_y not in allowed_target_types:
            raise ValueError(
                'Supported target types are: {}. Got {!r} instead.'.format(
                    allowed_target_types, type_of_target_y))

        y = column_or_1d(y)
        _, y_idx, y_inv = np.unique(y, return_index=True, return_inverse=True)  # type: ignore
        _, class_perm = np.unique(y_idx, return_inverse=True)
        y_encoded = class_perm[y_inv]
        class_unique, _ = np.unique(y_encoded, return_counts=True)

        for _ in range(0, self.n_splits):
            train_indices: np.ndarray = np.zeros(y_encoded.size, dtype=bool)
            test_indices: np.ndarray = np.zeros(y_encoded.size, dtype=bool)
            for cls in class_unique:
                class_idxs = np.where(y_encoded == cls)[0]
                # if only one element in a class, add it both to training_scripts and test
                if len(class_idxs) == 1:
                    train_indices[class_idxs] = True
                    test_indices[class_idxs] = True
                    continue
                # if the class is really small, add at least one element to the test set
                test_size = round(self.test_ratio * len(class_idxs)) or 1
                train_indices[class_idxs[:-test_size]] = True
                test_indices[class_idxs[-test_size:]] = True
            # if there's only one class in the training set, don't bother
            if np.unique(y_encoded[train_indices]).size == 1:
                continue
            yield train_indices, test_indices


class AutoKFold:
    """Automatically determine the number of splits based on class size.

    The default StratifiedKFold approach has an automatic method of
    determining the minimum number of splits based on the smallest class.
    This KFolder can be called with other KFold methods if needed.
    It is recommended to use StratifiedKFold with shuffle=False and a fixed random_state=1.
    """

    def __init__(self, n_splits: int = -1, kfolder: _BaseKFold = StratifiedKFold, **kwargs: object):
        """Derive the n_splits based on the smallest class.

        @type n_splits: int
        @param n_splits: Default value -1, which implies auto selecting the n_splits.
        A value larger than 0 will override the auto-selected n_splits.

        @type kfolder: _BaseKFold
        @param kfolder: Any scikit learn KFold validator or similar.

        @type **kwargs: object
        @param **kwargs: used to send custom parameters to the kfolder
        """
        self.n_splits = n_splits
        self.kwargs = kwargs
        self.kfolder = kfolder

    def get_n_splits(self, X: Optional[np.ndarray] = None,
                     y: Optional[np.ndarray] = None,
                     groups: Optional[np.ndarray] = None) -> int:
        """Return the number of splitting iterations in the cross-validator.

        @type X: object
        @param X: Always ignored, exists for compatibility.

        @type y: object
        @param y: Always ignored, exists for compatibility.

        @type groups: array-like, with shape (n_samples,)
        @param groups: Group labels for the samples used while splitting the dataset into
            training_scripts/test set. This 'groups' parameter must always be specified to
            calculate the number of splits, though the other parameters can be
            omitted.

        @rtype n_splits : int
        @return: the number of splitting iterations in the cross-validator.
        """
        if self.n_splits < 1:
            _, counts = np.unique(y, return_counts=True)  # type: ignore
            minimum_nr_of_samples = counts.min()
            self.n_splits = np.max([2, minimum_nr_of_samples])
            logger.info(f"StratifiedKFold n_splits set to smallest class: {self.n_splits}")
        return self.n_splits

    def split(self, X: Optional[np.ndarray],
              y: Optional[np.ndarray] = None,
              groups: Optional[np.ndarray] = None) -> Generator[tuple, None, None]:
        """Generate indices to split data into training and test set.

        @type X: array-like, shape (n_samples, n_features)
        @param X: Training data, where n_samples is the number of samples
            and n_features is the number of features.
            Note that providing ``y`` is sufficient to generate the splits and
            hence ``np.zeros(n_samples)`` may be used as a placeholder for
            ``X`` instead of actual training data.

        @type y : array-like, shape (n_samples,)
        @param y: The target variable for supervised learning problems.
            Stratification is done based on the y labels.

        @type groups: object
        @param groups: Always ignored, exists for compatibility.

        @rtype: tupple
        @return:
            training_scripts : ndarray - The training set indices for that split.
            test : ndarray - The testing set indices for that split.

        Notes
        -----
        Randomized CV splitters may return different results for each call of
        split. You can make the results identical by setting ``random_state``
        to an integer.

        """
        X, y, groups = indexable(X, y, groups)

        self.n_splits = self.get_n_splits(X, y, groups=groups)
        cv = self.kfolder(n_splits=self.n_splits, **self.kwargs)
        return cv.split(X, y, groups=groups)


class TemporalStochasticSplit:
    """Probabilistic split of temporal data.

    This method splits the data according to a probability distribution over
    the order of the data. It expects the data to be sorted by date with the
    oldest elements being in the first rows and the most recent ones in the
    last rows. The test set will be constructed preferably from data points
    that have higher indices. Each test set split will contain a proportion
    of test_ratio of elements from the whole dataset, while the rest will be
    used to build the training set.

    Assumption: at some point in time the probability to observe
    a number of incidents of a class can be modelled by a multinomial
    distribution. The process has two steps:
    1. sample from the multinomial distribution the size of each
    class in the test set
    2. sample from the total nr of elements of a class the test set
    according to a softmax over ranks that assigns higher probabilities
    to more recent elements
    """

    def __init__(self, n_splits: int = -1, test_ratio: float = 0.2):
        """Auto-detect the number of splits.

        At the same time it is possible to
        use a test ratio to select test samples in each split.

        @type n_splits: int
        @param n_splits: Default value -1, which implies auto selecting the n_splits.
        A value larger than 0 will override the auto-selected n_splits.

        @type test_ratio: float
        @param test_ratio: Default value 0.2
        """
        self.n_splits = n_splits
        self.test_ratio = test_ratio

    def get_n_splits(self, X: Optional[np.ndarray] = None,
                     y: Optional[np.ndarray] = None,
                     groups: Optional[np.ndarray] = None) -> int:
        """Get the number of splitting iterations in the cross-validator.

        This function computes the number of n_splits based on the smallest class.
        @type X: object
        @param X: Always ignored, exists for compatibility.

        @type y: object
        @param y: Always ignored, exists for compatibility.

        @type groups: array-like, with shape (n_samples,)
        @param groups: Group labels for the samples used while splitting the dataset into
            training_scripts/test set. This 'groups' parameter must always be specified to
            calculate the number of splits, though the other parameters can be
            omitted.

        @rtype n_splits : int
        @return: the number of splitting iterations in the cross-validator.
        """
        y = check_array(y, ensure_2d=False, dtype=None)
        if self.n_splits < 1:
            _, y_counts = np.unique(y, return_counts=True)  # type: ignore
            self.n_splits = np.min(y_counts)
            logger.info(f"TemporalStochasticSplit n_splits set to smallest class: {self.n_splits}")
        return self.n_splits

    def split(self, X: np.ndarray,
              y: Optional[np.ndarray] = None,
              groups: Optional[np.ndarray] = None) -> Generator[tuple, None, None]:
        """Generate indices to split data into training and test set.

        This method splits samples classes in the test set based on
        a multinomial distribution. The final test set contains elements prioritized
        using a distribution over their ranks.
        @type X: array-like, shape (n_samples, n_features)
        @param X: Training data, where n_samples is the number of samples
            and n_features is the number of features.
            Note that providing ``y`` is sufficient to generate the splits and
            hence ``np.zeros(n_samples)`` may be used as a placeholder for
            ``X`` instead of actual training data.

        @type y : array-like, shape (n_samples,)
        @param y: The target variable for supervised learning problems.
            Stratification is done based on the y labels.

        @type groups: object
        @param groups: Always ignored, exists for compatibility.

        @rtype: tupple
        @return:
            training_scripts : ndarray - The training set indices for that split.
            test : ndarray - The testing set indices for that split.

        Notes
        -----
        Randomized CV splitters may return different results for each call of
        split. This splitter does not have the option to fix a random state.

        """
        y = np.asarray(y)
        X, y, groups = indexable(X, y, groups)
        self.n_splits = self.get_n_splits(X, y, groups)
        type_of_target_y = type_of_target(y)
        allowed_target_types = ('binary', 'multiclass')
        if type_of_target_y not in allowed_target_types:
            raise ValueError(
                'Supported target types are: {}. Got {!r} instead.'.format(
                    allowed_target_types, type_of_target_y))

        y = column_or_1d(y)

        _, y_idx, y_inv = np.unique(y, return_index=True, return_inverse=True)  # type: ignore
        # y_inv encodes y according to lexicographic order. We invert y_idx to
        # map the classes so that they are encoded by order of appearance:
        # 0 represents the first label appearing in y, 1 the second, etc.
        _, class_perm = np.unique(y_idx, return_inverse=True)
        y_encoded = class_perm[y_inv]
        class_unique, classes_counts = np.unique(y_encoded, return_counts=True)
        class_proba = softmax(classes_counts / classes_counts.sum())
        population = np.round(self.test_ratio * classes_counts.sum())
        multinomial_sample = np.random.multinomial(population,
                                                   class_proba,
                                                   size=self.n_splits)
        for idx in range(0, multinomial_sample.shape[0]):
            test_indices: np.ndarray = np.zeros(y_encoded.size, dtype=bool)
            for cls in class_unique:
                class_idxs = np.where(y_encoded == cls)[0]
                class_size = len(class_idxs) - 1
                nr_of_class_elems = np.min([multinomial_sample[idx, cls], class_size]) or 1
                proba_of_class_elems = (class_idxs + 1)
                proba_of_class_elems = proba_of_class_elems / proba_of_class_elems.sum()
                indices = np.random.choice(class_idxs,
                                           size=nr_of_class_elems,
                                           p=proba_of_class_elems,
                                           replace=False)
                test_indices[indices] = True
            train_indices = np.logical_not(test_indices)
            # if there's only one class in the training set, don't bother
            if np.unique(y_encoded[train_indices]).size == 1:
                continue
            yield train_indices, test_indices
