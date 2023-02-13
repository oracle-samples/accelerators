################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Tue Jan 31 13:02:49 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: b9fa763a4b53f86a82f6e21b6bcd02f2fe2a351a $
################################################################################################
#  File: test_model_selection.py
################################################################################################
import pytest
import numpy as np
import pandas as pd
from ai4service_automated_classification.ml.model_selection import TemporalStratifiedSplit, \
    AutoKFold, TemporalStochasticSplit
from sklearn.model_selection import KFold, RepeatedKFold, RepeatedStratifiedKFold, \
    ShuffleSplit, StratifiedKFold, TimeSeriesSplit


def test_TemporalStratifiedSplit_nsplits():
    splitter = TemporalStratifiedSplit(test_ratio=0.2)
    # this thing is set to one split by default
    assert splitter.get_n_splits(X=None, y=None) == 1


def test_TemporalStratifiedSplit_split_nothing_to_split():
    splitter = TemporalStratifiedSplit(test_ratio=0.2)
    y = np.array([1, 2])
    # when there's no class to be split in two, it uses
    # the same element in both training_scripts and test
    splits = list(splitter.split(X=None, y=y))
    assert len(splits) == 1
    assert np.array_equal(splits[0], (np.ones(2, dtype=bool), np.ones(2, dtype=bool)))


def test_TemporalStratifiedSplit_split_different_inputs_ok():
    splitter = TemporalStratifiedSplit(test_ratio=0.1)
    # works with lists
    y = ['a', 'b', 'c', 'a', 'b', 'a']
    splits = list(splitter.split(X=None, y=y))
    print(1)
    assert len(splits) == 1
    splits = splits[0]
    # training_scripts test made from the first n elements
    assert np.array_equal([0, 1, 2, 3], np.where(splits[0])[0])
    # test set made from the last elements
    # + 'c' in both sets because its size is too small to split
    assert np.array_equal([2, 4, 5], np.where(splits[1])[0])
    # works with column arrays
    y = np.array(y)
    splits = list(splitter.split(X=None, y=y.reshape(y.size, 1)))[0]
    # training_scripts test made from the first n elements
    assert np.array_equal([0, 1, 2, 3], np.where(splits[0])[0])
    # test set made from the last elements
    # + 'c' in both sets because its size is too small to split
    assert np.array_equal([2, 4, 5], np.where(splits[1])[0])
    # works with DataFrames
    y = pd.DataFrame(['a', 'b', 'c', 'a', 'b', 'a'])
    splits = list(splitter.split(X=None, y=y))[0]
    # training_scripts test made from the first n elements
    assert np.array_equal([0, 1, 2, 3], np.where(splits[0])[0])
    # test set made from the last elements
    # + 'c' in both sets because its size is too small to split
    assert np.array_equal([2, 4, 5], np.where(splits[1])[0])


def test_TemporalStratifiedSplit_split_different_inputs_bad():
    splitter = TemporalStratifiedSplit(test_ratio=0.1)
    # does not work with multilabel indicator
    # a conversion would have to be done first
    y = [(1, 2), (1, 2), (2, 2), (1, 2), (2, 2)]
    with pytest.raises(ValueError):
        splits = list(splitter.split(X=None, y=y))
    # does not work with mixed labels
    # can't compare them for np.unique
    y = pd.DataFrame(['a', (1, 2), 'a', (1, 2), 5, 5])
    with pytest.raises(TypeError):
        splits = list(splitter.split(X=None, y=y))
    # must not have Nones
    y = np.array(['a', None, 'c', None, 'b', 'a'])
    with pytest.raises(TypeError):
        list(splitter.split(X=None, y=y))
    # does not work with one-hot encodings
    y = np.identity(10)
    with pytest.raises(ValueError):
        list(splitter.split(X=None, y=y))
    # works with a single class, it returns nothing
    y = np.zeros(10)
    splits = list(splitter.split(X=None, y=y))
    assert splits == []


def test_AutoKFold_nsplits():
    splitter = AutoKFold(n_splits=-1)
    y = [1, 2, 1, 1, 1, 2]
    n_splits = splitter.get_n_splits(X=None, y=y)
    assert n_splits == 2
    splitter = AutoKFold(n_splits=10)
    n_splits = splitter.get_n_splits(X=None, y=y)
    assert n_splits == 10


def test_AutoKFold_split_sklearn_kfolders():
    kfolders = [KFold, RepeatedKFold, RepeatedStratifiedKFold,
                ShuffleSplit, StratifiedKFold, TimeSeriesSplit]
    y = np.array([1, 2, 1, 1, 1, 2, 3, 3, 3])
    for kfolder in kfolders:
        splitter = AutoKFold(n_splits=-1, kfolder=kfolder)
        print(kfolder.__class__)
        splits = list(splitter.split(X=y, y=y))
        assert len(splits) >= 2
    # kwargs must be used by the kfolder
    splitter = AutoKFold(n_splits=-1, kfolder=RepeatedKFold, n_repeats=100)
    splits = list(splitter.split(X=y, y=y))
    assert len(splits) == 100 * 2


def test_AutoKFold_split_ok():
    splitter = AutoKFold(n_splits=-1)
    y = np.array([1, 2, 1, 1, 1, 2])
    x = np.zeros(len(y))
    # X is a mandatory param for splitting
    # BaseKFold uses X to derive the number of splits
    splits = list(splitter.split(X=x, y=y))
    assert len(splits) == 2


def test_AutoKFold_split_bad():
    y = np.array([1, 2, 1, 1, 1, 2])
    # if we pass more n_splits,
    # the kfolder will complain
    splitter = AutoKFold(n_splits=10, kfolder=StratifiedKFold,
                         shuffle=False, random_state=1)
    with pytest.raises(ValueError):
        list(splitter.split(X=y, y=y))


def test_AutoKFold_split_non_indexable():
    y = set([1, 2, 1, 1, 1, 2])
    # non idexable stuff should fail
    splitter = AutoKFold(n_splits=10, kfolder=StratifiedKFold,
                         shuffle=False, random_state=1)
    with pytest.raises(TypeError):
        list(splitter.split(X=y, y=y))


def test_TemporalStochasticSplit_nsplits():
    splitter = TemporalStochasticSplit(n_splits=-1)
    y = [1, 2, 1, 1, 1, 2]
    n_splits = splitter.get_n_splits(X=None, y=y)
    assert n_splits == 2
    splitter = TemporalStochasticSplit(n_splits=10)
    n_splits = splitter.get_n_splits(X=None, y=y)
    assert n_splits == 10


def test_TemporalStochasticSplit_split():
    splitter = TemporalStochasticSplit(n_splits=-1)
    y = np.array([1, 2, 1, 1, 1, 2])
    splits = list(splitter.split(X=None, y=y))
    assert len(splits) == 2
    # even if we pass more n_splits, the total number
    # will be dynamically decided based on the size of each class
    splitter = TemporalStochasticSplit(n_splits=10)
    splits = list(splitter.split(X=None, y=y))
    assert len(splits) == 10


def test_TemporalStochasticSplit_split_different_inputs_ok():
    splitter = TemporalStochasticSplit(n_splits=-1)
    # works with lists
    y = ['a', 'b', 'c', 'a', 'b', 'a']
    splits = list(splitter.split(X=None, y=y))
    assert len(splits) == 1
    # works with arrays
    y = np.array(['a', 'b', 'c', 'a', 'b', 'a'])
    splits = list(splitter.split(X=None, y=y))
    assert len(splits) == 1
    # works with column arrays
    splits = list(splitter.split(X=None, y=y.reshape(y.size, 1)))
    assert len(splits) == 1
    # works with DataFrames
    y = pd.DataFrame(['a', 'b', 'c', 'a', 'b', 'a'])
    splits = list(splitter.split(X=None, y=y))
    assert len(splits) == 1


def test_TemporalStochasticSplit_split_different_inputs_bad():
    splitter = TemporalStochasticSplit(n_splits=-1)
    # does not work with multilabel indicator
    # a conversion would have to be done first
    y = [(1, 2), (1, 2), (2, 2), (1, 2), (2, 2)]
    with pytest.raises(ValueError):
        splits = list(splitter.split(X=None, y=y))
    # does not work with mixed labels
    # can't compare them for np.unique
    y = pd.DataFrame(['a', (1, 2), 'a', (1, 2), 5, 5])
    with pytest.raises(TypeError):
        splits = list(splitter.split(X=None, y=y))
    # must not have Nones
    y = np.array(['a', None, 'c', None, 'b', 'a'])
    with pytest.raises(TypeError):
        list(splitter.split(X=None, y=y))
    # does not work with one-hot encodings
    y = np.identity(10)
    with pytest.raises(ValueError):
        list(splitter.split(X=None, y=y))
    # works with a single class, it returns nothing
    y = np.zeros(10)
    splits = list(splitter.split(X=None, y=y))
    assert splits == []
