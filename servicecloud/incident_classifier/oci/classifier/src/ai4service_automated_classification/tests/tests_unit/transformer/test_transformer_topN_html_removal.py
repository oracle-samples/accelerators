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
#  SHA1: $Id: 16c4b97d5c5194a1d1949cdec77b4720f0448a3e $
################################################################################################
#  File: test_transformer_topN_html_removal.py
################################################################################################
from ai4service_automated_classification.ml.Transformer import Transformer, \
    get_top_n_words, DefaultCountVect
from sklearn.base import clone

import pytest


def test_instantiate_transformer():
    featurizer = Transformer()
    assert isinstance(featurizer, Transformer)


def test_get_top_n_words_with_topn_none_not_declared():
    expected = ['previous', 'release', 'date', 'previous release', 'release date', 'email', 'address',
                'displayed', 'email address', 'address displayed', 'impact', 'business', 'impact business']
    assert get_top_n_words(["previous release date", "email address is not displayed",
                            "impact their business"]) == expected


def test_get_top_n_words_with_topn_none():
    expected = ['previous', 'release', 'date', 'previous release', 'release date', 'email', 'address',
                'displayed', 'email address', 'address displayed', 'impact', 'business', 'impact business']
    assert get_top_n_words(["previous release date", "email address is not displayed", "impact their business"],
                           top_n=None) == expected


def test_get_top_n_words_with_topn_3(corpus):
    expected = ['datasource', 'rolling', 'multi']
    assert get_top_n_words(corpus, top_n=3) == expected
# Notice that the order of the values in the list does matter.
# TODO: investigate if it might be an issue


# this test allows us to get to 100%
def test_get_top_n_words_with_corpus_empty():
    corpus_test = []
    expected = []
    assert get_top_n_words(corpus_test) == expected


# to test value error
def test_get_top_n_words_with_corpus_empty_Value_error():
    corpus_test = []
    with pytest.raises(ValueError) as e:
        vec = clone(DefaultCountVect)
        vec.fit(corpus_test)
        expected = []
        assert vec.fit(corpus_test) == expected
    assert "empty vocabulary; perhaps the documents only contain stop words" == str(e.value)
