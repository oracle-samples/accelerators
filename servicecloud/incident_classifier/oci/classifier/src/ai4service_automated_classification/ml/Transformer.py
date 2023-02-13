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
#  SHA1: $Id: d036b9d73b4e88965a314a7adbfd19cdaf456ada $
################################################################################################
#  File: Transformer.py
################################################################################################
"""Transformer here means something that converts text into numbers."""

import logging
import os
from typing import Any, Optional

from pandas import Series
from sklearn.base import BaseEstimator, clone
from sklearn.feature_extraction.text import (CountVectorizer, TfidfVectorizer, _VectorizerMixin)

import numpy as np
import html5lib  # NOQA

logger = logging.getLogger(os.path.basename(__file__))

DefaultTransformer = TfidfVectorizer
default_trans_params = {
    'min_df': 1,
    'max_features': None,
    'strip_accents': 'unicode',
    'analyzer': 'word',
    'token_pattern': r'\b[^\d\W]+\b',
    'ngram_range': (1, 2),
    'use_idf': True,
    'smooth_idf': True,
    'sublinear_tf': True,
    'stop_words': 'english'
}
DefaultCountVect = CountVectorizer(strip_accents='unicode',  # noqa: S106
                                   stop_words='english',
                                   analyzer='word',
                                   ngram_range=default_trans_params['ngram_range'],
                                   # lowercase=True,
                                   token_pattern=r'\b[^\d\W]+\b')


def get_top_n_words(corpus: list, top_n: Optional[int] = None) -> list:
    """
    Count the first most frequent words in a list of texts.

    It uses the CountVectorizer by default.

    @type corpus: list
    @param corpus: list of texts
    @type top_n: int or None
    @param top_n: how many of the first n words you'd want (Default value = None)
    @rtype: list
    @return: list of most frequent top_n words

    """
    if top_n:
        logging.debug(f"Getting top {top_n} words")
    else:
        logging.debug("Getting all the words")
    try:
        vec = clone(DefaultCountVect)
        vec.fit(corpus)
    except ValueError as e:
        logger.exception("Can't get top n words :(. " + str(e))
        return []
    bag_of_words = vec.transform(corpus)
    sum_words = bag_of_words.sum(axis=0)
    words_freq = [(word, sum_words[0, idx])
                  for word, idx in vec.vocabulary_.items()]
    words_freq = sorted(words_freq, key=lambda value: value[1], reverse=True)
    logger.info(f"{top_n} words done.")
    return [wd for wd, _ in words_freq[:top_n]]


class Transformer(_VectorizerMixin, BaseEstimator):
    """Transformer class as wrapper over tf-idf."""

    def __init__(self,
                 top_n: Optional[int] = 1000,
                 percentage: int = 10,
                 transformer: Optional[_VectorizerMixin] = None):
        # pylint: disable=too-many-arguments
        """
        Transform raw texts with a pre-provided transformer or with pandas.DataFrame columns.

        @type top_n: int
        @param top_n: top n words to include, if auto, it tries an experimental thing to auto-detect the best number
        @type percentage: int
        @param percentage: experimental, in case we want to use the percentage instead of the frequency
        @param transformer: scikit-learn like text Transformer
        (e.g., ['Year', 'has_been_reassigned'])
        """
        self.top_n = top_n
        self.percentage = min([percentage, 100])
        if transformer is None:
            transformer = DefaultTransformer(**default_trans_params)

        DefaultCountVect.ngram_range = transformer.ngram_range
        self.transformer = transformer

    def fit(self, dataframe: Series, _: Any = None) -> object:
        """
        Sklearn-like fit function for both dataframes and raw texts.

        pandas.DataFrames require a set of columns in the constructor of the object.

        @type dataframe: L{pandas.Series}
        @param dataframe: input data
        @type _: Any
        @param _: Not used. It's for Sklearn Pipeline compliance
        @rtype: object
        @return: object with the fitted vectorizer

        """
        dataframe.dropna(inplace=True)
        # top_n =>
        # None -  Select ALL words
        # top_n > 0 - Select User Specified Number of Words
        # top_n == 0 - Set Vocabulary to None to be able to use (min_df, max_df) or max_features
        if self.top_n is None:
            self.transformer.vocabulary = get_top_n_words(dataframe.values)  # get all the words, including stop-words
        elif self.top_n > 0:
            self.transformer.vocabulary = get_top_n_words(dataframe.values,
                                                          self.top_n)  # use user-specified top_n words
        else:
            self.transformer.vocabulary = None  # use (min_df, max_df) or max_features
        logging.debug(f"Fitting the transformer with {self.top_n} words.")
        result = self.transformer.fit(dataframe.values)
        logging.debug("Done fitting the transformer.")
        return result

    def transform(self, dataframe: Series, _: Any = None) -> np.ndarray:
        """
        Sklearn-like transform function for both dataframes and raw texts.

        pandas.DataFrames require a set of columns in the constructor of the object.

        @type dataframe: L{pandas.Series}
        @param dataframe: input data
        @type _: Any
        @param _: Not used. It's for Sklearn Pipeline compliance.
        @rtype: array
        @return: array with the transformed data

        """
        logging.debug("Transforming data...")
        result = self.transformer.transform(dataframe.values)
        logging.debug("Done transforming data.")
        return result

    def fit_transform(self, dataframe: Series, _: Any = None) -> np.ndarray:
        """
        Fit transform.

        @type dataframe: Series
        @param dataframe: input data
        @type _: Any
        @param _: Used for Sklearn Pipeline compliance.
        @rtype: array
        @return: array with the fitted and transformed data

        """
        values = dataframe.values.tolist()
        # Start to develop the options
        if self.top_n is None:
            self.transformer.vocabulary = get_top_n_words(values)  # get all the words, including stop-words
        elif self.top_n > 0:
            self.transformer.vocabulary = get_top_n_words(values, self.top_n)  # use user-specified top_n words
        else:
            self.transformer.vocabulary = None  # use (min_df, max_df) or max_features
        logging.debug("Fitting and transforming data all at once..")
        result = self.transformer.fit_transform(values)
        logging.debug("Done fitting and transforming.")
        return result
