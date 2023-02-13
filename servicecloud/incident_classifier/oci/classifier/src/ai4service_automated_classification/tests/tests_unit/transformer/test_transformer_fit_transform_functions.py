################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Tue Jan 31 13:02:51 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: 65985be0c22c8215df118d2f12a9c242e41481fd $
################################################################################################
#  File: test_transformer_fit_transform_functions.py
################################################################################################
import pytest
from numpy.testing import assert_array_almost_equal
from sklearn.base import BaseEstimator
from sklearn.feature_extraction.text import (TfidfVectorizer, _VectorizerMixin)

from ai4service_automated_classification.constants import DATA_COLUMN
from ai4service_automated_classification.ml.Transformer import Transformer, DefaultTransformer


@pytest.fixture(scope='function')
def featurizer(dummy_data):
    df_0, df_1 = dummy_data
    data_test = df_0[DATA_COLUMN]
    featurizer = Transformer()
    fitted_featurizer = featurizer.fit(data_test)
    yield fitted_featurizer


def test_canFit(featurizer):
    fitted_featurizer = featurizer
    expected = ['hello', 'hi', 'specific', 'text', 'class',
                'specific text', 'text class', 'world',
                'hello world', 'lukas', 'soon', 'hi lukas',
                'lukas soon', 'fabrice', 'mandy', 'hi fabrice',
                'fabrice mandy', 'daniel', 'hello daniel',
                'sleep']
    assert fitted_featurizer.vocabulary == expected


@pytest.fixture(scope='function')
def featurizer_topNNone(dummy_data):
    df_0, df_1 = dummy_data
    data_test = df_0[DATA_COLUMN]
    featurizer = Transformer(top_n=None)
    fitted_featurizer = featurizer.fit(data_test)
    yield fitted_featurizer


def test_canFit_topNNone(featurizer_topNNone):
    fitted_featurizer = featurizer_topNNone
    expected = ['hello', 'hi', 'specific', 'text', 'class',
                'specific text', 'text class', 'world',
                'hello world', 'lukas', 'soon', 'hi lukas',
                'lukas soon', 'fabrice', 'mandy', 'hi fabrice',
                'fabrice mandy', 'daniel', 'hello daniel',
                'sleep']
    assert fitted_featurizer.vocabulary == expected


@pytest.fixture(scope='function')
def featurizer_topN(dummy_data):
    df_0, df_1 = dummy_data
    data_test = df_0[DATA_COLUMN]
    featurizer = Transformer(top_n=3)
    fitted_featurizer = featurizer.fit(data_test)
    yield fitted_featurizer


def test_canFit_topN(featurizer_topN):
    fitted_featurizer = featurizer_topN
    expected = ['hello', 'hi', 'specific']
    assert fitted_featurizer.vocabulary == expected


@pytest.fixture(scope='function')
def featurizer_topNzero(dummy_data):
    df_0, df_1 = dummy_data
    data_test = df_0[DATA_COLUMN]
    featurizer = Transformer(top_n=0)
    fitted_featurizer = featurizer.fit(data_test)
    yield fitted_featurizer


def test_canFit_topNzero(featurizer_topNzero):
    fitted_featurizer = featurizer_topNzero
    expected = None
    assert fitted_featurizer.vocabulary == expected


def test_transformer_setters_topN():
    featurizer = Transformer(top_n=2000)
    assert featurizer.top_n == 2000


def test_transformer_setters_topN_default():
    featurizer = Transformer()
    assert featurizer.top_n == 1000


def test_transformer_setters_percent():
    featurizer = Transformer(percentage=20)
    assert featurizer.percentage == 20


def test_transformer_setters_percent_default():
    featurizer = Transformer()
    assert featurizer.percentage == 10


def test_transformer_setters_transformer_default():
    featurizer = Transformer()
    assert isinstance(featurizer.transformer, DefaultTransformer)


def test_transformer_setters_transformer_default_none():
    featurizer = Transformer(transformer=None)
    assert isinstance(featurizer.transformer, DefaultTransformer)


def test_transformer_setters_transformer_default_min_df():
    featurizer = Transformer()
    assert featurizer.transformer.min_df == 1


def test_transformer_setters_transformer_default_max_features():
    featurizer = Transformer()
    assert featurizer.transformer.max_features is None


# function to test re-setting the Transformer default params
def test_transformer_params_setters():
    default_trans_params = {'min_df': 0.001,
                            # 'max_df': 0.9,
                            'max_features': None,
                            # 'lowercase': True,
                            'strip_accents': 'unicode',
                            'analyzer': 'word',
                            'token_pattern': r'\b[^\d\W]+\b',
                            'ngram_range': (1, 2),
                            'use_idf': True,
                            'smooth_idf': True,
                            'sublinear_tf': True,
                            'stop_words': 'english',
                            }
    DefaultTransformer = TfidfVectorizer(**default_trans_params)
    featurizer = Transformer(transformer=DefaultTransformer)
    assert featurizer.transformer.min_df == 0.001


# test which class it extends
def test_transformer_Class_extensions_BaseEstimator():
    assert issubclass(Transformer, BaseEstimator)


def test_transformer_Class_extensions_VectorizerMixin():
    assert issubclass(Transformer, _VectorizerMixin)


def test_canTransform(dummy_data, transformed_data_array):
    featurizer = Transformer()
    df_0, df_1 = dummy_data
    data_test = df_0[DATA_COLUMN]
    fitted_featurizer = featurizer.fit(data_test)
    fitted_t_data = fitted_featurizer.transform(data_test).toarray()
    assert_array_almost_equal(fitted_t_data, transformed_data_array, decimal=8)


def test_canFit_Transform_topN(dummy_data, transformed_data_array):
    featurizer = Transformer(top_n=1000)
    df_0, df_1 = dummy_data
    data_test = df_0[DATA_COLUMN].dropna()
    fitted_t_data = featurizer.fit_transform(data_test).toarray()
    assert_array_almost_equal(fitted_t_data, transformed_data_array, decimal=8)


def test_canFit_Transform_topNNone(dummy_data, transformed_data_array):
    featurizer = Transformer(top_n=None)
    df_0, df_1 = dummy_data
    data_test = df_0[DATA_COLUMN].dropna()
    fitted_t_data = featurizer.fit_transform(data_test).toarray()
    assert_array_almost_equal(fitted_t_data, transformed_data_array, decimal=8)


def test_canFit_Transform_topNzero(dummy_data, transformed_data_array_topNzero):
    featurizer = Transformer(top_n=0)
    df_0, df_1 = dummy_data
    data_test = df_0[DATA_COLUMN].dropna()
    fitted_t_data = featurizer.fit_transform(data_test).toarray()
    assert_array_almost_equal(fitted_t_data, transformed_data_array_topNzero, decimal=8)
