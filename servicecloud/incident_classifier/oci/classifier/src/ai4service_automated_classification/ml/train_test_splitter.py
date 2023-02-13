################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Tue Jan 31 13:02:47 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: c2e6f5328e5ddfce59c0f52344a40757218c89ed $
################################################################################################
#  File: train_test_splitter.py
################################################################################################
"""Abstraction used to split the data into training and test."""

from abc import ABC, abstractmethod
from typing import List, Optional, Tuple

import pandas as pd
from sklearn.model_selection import train_test_split

TrainTestSplitType = Tuple[pd.DataFrame, pd.DataFrame, pd.DataFrame, pd.DataFrame]


class TrainTestSplitter(ABC):
    """Base class to define any kind of training_scripts-test splitter.

    Provide a utility method `_get_splits(...)` to split a Pandas data frame into 4 bits:
    training_scripts X and Y and test X and Y, which could be used by child classes.
    """

    def __init__(self,
                 target_columns: List[str],
                 time_column: Optional[str] = None,
                 train_test_ratio: float = 0.8,
                 random_seed: int = 42,
                 drop_columns: Optional[List[str]] = None
                 ) -> None:
        """Initialise TrainTestSplitter."""
        self.target_columns = target_columns
        self.time_column = time_column
        self.train_test_ratio = train_test_ratio
        self.random_seed = random_seed
        self.drop_columns = drop_columns

    @abstractmethod
    def split(self, df: pd.DataFrame) -> TrainTestSplitType:
        """Return x_train, x_test, y_train, y_test from the Data Frame."""
        raise NotImplementedError("Need to implement the split method.")

    def _drop_unwanted_columns(self, data: pd.DataFrame) -> pd.DataFrame:
        if self.drop_columns:
            data = data.drop(self.drop_columns, axis=1)
        return data

    def _get_splits(self, data: pd.DataFrame) -> TrainTestSplitType:
        data = self._drop_unwanted_columns(data)
        train, test = self._cut_train_test(data)
        x_train, y_train = self._extract_features_and_labels(train,
                                                             self.target_columns)
        x_test, y_test = self._extract_features_and_labels(test,
                                                           self.target_columns)
        return x_train, x_test, y_train, y_test

    def _cut_train_test(self, data: pd.DataFrame) -> Tuple[pd.DataFrame,
                                                           pd.DataFrame]:
        train_index_end = int(data.shape[0] * self.train_test_ratio)
        train = data.iloc[:train_index_end]
        test = data.iloc[train_index_end:]
        return train, test

    @staticmethod
    def _extract_features_and_labels(data: pd.DataFrame,
                                     target_columns: List[str],
                                     time_column: Optional[str] = None
                                     ) -> Tuple[pd.DataFrame, pd.DataFrame]:
        features_ignore = target_columns + ([time_column] if time_column else [])
        features = data[[col for col in data.columns
                         if col not in features_ignore]]
        labels = data[target_columns]
        return features, labels

    def __eq__(self, o: object) -> bool:
        """Override the default implementation."""
        class_attributes = [att for att in self.__dict__.keys()]
        for att in class_attributes:
            if self.__dict__[att] != o.__dict__[att]:
                return False
        return True


class RandomTrainTestSplitter(TrainTestSplitter):
    """Randomly split the data into training_scripts and test."""

    def __init__(self,
                 target_columns: List[str],
                 time_column: Optional[str] = None,
                 train_test_ratio: float = 0.8,
                 random_seed: int = 42,
                 drop_columns: Optional[List[str]] = None,
                 stratify_targets: bool = False):
        """
        Build a RandomTrainTestSplitter object.

        It will randomly split the data using the provided ratio and optionally will
        stratify the sample.
        """
        super().__init__(target_columns,
                         time_column,
                         train_test_ratio,
                         random_seed,
                         drop_columns)
        self.stratify_targets = stratify_targets

    def split(self, df: pd.DataFrame) -> TrainTestSplitType:
        """
        Split the data randomly using scikit-learn learn train_test_split function.

        :param df: Data to be split.
        :return: tuple containing (x_train, x_test, y_train, y_test).
        """
        data = self._drop_unwanted_columns(df)
        data_to_stratify_by = data[self.target_columns] if self.stratify_targets else None
        train_df, test_df = train_test_split(data,
                                             train_size=self.train_test_ratio,
                                             stratify=data_to_stratify_by,
                                             random_state=self.random_seed)
        x_train, y_train = self._extract_features_and_labels(train_df,
                                                             self.target_columns)
        x_test, y_test = self._extract_features_and_labels(test_df,
                                                           self.target_columns)
        return x_train, x_test, y_train, y_test
