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
#  SHA1: $Id: 44f0f29ba11bcfe048037dbceff906398dd16f41 $
################################################################################################
#  File: data_util.py
################################################################################################
"""
Utility functions to read the qa data and the product hierarchy.

Might be useful beyond the qa data depending on the format of future datasets.
"""
import logging
import os
import re
from operator import attrgetter
from typing import List, Dict, Union

import numpy as np
import spacy
from bs4 import BeautifulSoup as bs

from pandas import DataFrame, to_datetime

from ai4service_automated_classification.constants import (
    INITIAL_PRODUCT_ID_COLUMN, DATE_COLUMN, PRODUCT_COLUMN, PRODUCT_ID_COLUMN,
    INITIAL_CATEGORY_ID_COLUMN, INITIAL_DISPOSITION_ID_COLUMN, DATA_COLUMN, SUBJECT_COLUMN, TEXT_COLUMN,
    CATEGORY_ID_COLUMN, DISPOSITION_ID_COLUMN, CATEGORY_COLUMN, DISPOSITION_COLUMN, ID_COLUMN, NAME_COLUMN,
    INCIDENTS_COLUMNS, LOGGING_FORMAT)
from ai4service_automated_classification.ml.util.ocs_util import CSV_PATTERN
from ai4service_automated_classification.utils.data import text_clean
from ai4service_automated_classification.utils.object_storage.os import OcsBucket
from ai4service_automated_classification.utils.object_storage.strategies import _extract_relative_path
from ai4service_automated_classification.utils.object_storage.utils import FileInfo

HTML_PARSER = 'html5lib'
logger = logging.getLogger(os.path.basename(__file__))


def build_filter_dict(incidents_data: DataFrame,
                      hierarchy_data: DataFrame,
                      target: str) -> dict:
    """
    Construct a dictionary with product_id (keys) -- product name (values).

    Used to filter out products that are in the product hierarchy, but not in the training data.

    @type incidents_data: DataFrame
    @param incidents_data: incidents DataFrame.
    @type hierarchy_data: DataFrame
    @param hierarchy_data: hierarchy DataFrame to be used for filtering.
    @param target: target feature used for filtering
    @rtype: dict
    @return: a dictionary with id (keys) -- name (values)

    """
    available_ids = incidents_data[target].unique()
    filter_dict = dict(hierarchy_data[[ID_COLUMN, NAME_COLUMN]][hierarchy_data[ID_COLUMN]
                       .isin(available_ids)].values)
    return filter_dict


def prune_out_min_count(df: DataFrame,
                        min_count: int = 2,
                        field: str = PRODUCT_ID_COLUMN) -> DataFrame:
    """
    Remove products based on the minimum number of counts.

    @type df: DataFrame
    @param df: input dataframe
    @type min_count: int
    @param min_count: minimum number of products / label
    @type field: str
    @param field: Label/Field to prune
    @rtype: DataFrame
    @return: pruned Dataframe
    """
    result = DataFrame(columns=INCIDENTS_COLUMNS)
    if field in df.columns:
        df = df[df[field] != 0]
        vcopy = df[[field]]
        result = df[vcopy.replace(vcopy.stack().value_counts()).ge(min_count).all(1)]
    return result


def preprocess_hierarchy(hierarchy_data: DataFrame) -> DataFrame:
    """
    Lowercase the columns in the hierarchy dataframe to make it case insensitive.

    Also, generalise the columns for product, category and disposition

    @type hierarchy_data: DataFrame
    @param hierarchy_data: hierarchy DataFrame to preprocess.
    @rtype DataFrame
    @return: a processed hierarchy data
    """
    hierarchy_data.columns = hierarchy_data.columns \
        .str.lower() \
        .str.replace(PRODUCT_COLUMN, '') \
        .str.replace(CATEGORY_COLUMN, '') \
        .str.replace(DISPOSITION_COLUMN, '') \
        .str.strip()
    for index, column in enumerate(hierarchy_data.columns):
        if hierarchy_data.dtypes[index] == 'float64':
            hierarchy_data[column] = hierarchy_data[column].fillna(0).astype('Int32')
    hierarchy_data = hierarchy_data.drop_duplicates(subset=[ID_COLUMN])
    return hierarchy_data


def preprocess_incidents(incidents_data: DataFrame,
                         remove_html: bool = True) -> Dict[str, DataFrame]:
    """Clean and fix the hierarchy and incidents.

    It cleans the incidents and sorts the data based on date.
    @type incidents_data: DataFrame
    @param incidents_data: input dataframe
    @type remove_html: boolean
    @param remove_html: remove html from text
    @type signatures_to_remove: List
    @param signatures_to_remove: regex to remove from text

    @rtype Dict[str, DataFrame]
    @return Clean Incidents Data
    """
    # Lowercase the columns in the incidents dataframe to make it case insensitive
    incidents_data.columns = incidents_data.columns.str.lower()

    # If the data column is not provided, firts remove incidents with empty subject
    # then use the subject+text instead
    if DATA_COLUMN not in incidents_data.columns:
        incidents_data.dropna(subset=[SUBJECT_COLUMN], inplace=True)
        incidents_data[DATA_COLUMN] = incidents_data[SUBJECT_COLUMN] + " " + incidents_data[TEXT_COLUMN]

    # First remove NaN values in the data column that come from the report
    # BeautifulSoup doesn't support removing html from NoneType
    incidents_data.dropna(subset=[DATA_COLUMN], inplace=True)

    if remove_html:
        try:
            nlp = spacy.load("en_core_web_lg")
        except IOError as err:
            nlp = None
            logger.error(f"### {str(err)} ###")
        incidents_data[DATA_COLUMN] = incidents_data[DATA_COLUMN].apply(lambda text: text_clean(text, nlp))
        incidents_data.dropna(subset=[DATA_COLUMN], inplace=True)

    # Fill Nan values with 0 for the training features
    incidents_data.fillna(value={INITIAL_PRODUCT_ID_COLUMN: 0}, inplace=True)
    incidents_data.fillna(value={INITIAL_CATEGORY_ID_COLUMN: 0}, inplace=True)
    incidents_data.fillna(value={INITIAL_DISPOSITION_ID_COLUMN: 0}, inplace=True)

    # Ideally we would want to have the more recent incidents at the bottom.
    if DATE_COLUMN in incidents_data:
        logger.info(f'Date column "{DATE_COLUMN}"" found in data; ordering by date...')
        incidents_data[DATE_COLUMN] = to_datetime(incidents_data[DATE_COLUMN],
                                                  dayfirst=False,
                                                  infer_datetime_format=True)
        logger.debug(f'Date range found between "{incidents_data[DATE_COLUMN].min()}"'
                     f' and "{incidents_data[DATE_COLUMN].max()}".')
        incidents_data.sort_values(by=DATE_COLUMN, ascending=True, inplace=True)

    # Remove empty targets for product, category and disposition
    # create 3 datasets and then remove empty on each
    product_incidents_data = incidents_data.dropna(subset=[PRODUCT_ID_COLUMN])
    category_incidents_data = incidents_data.dropna(subset=[CATEGORY_ID_COLUMN])
    disposition_incidents_data = incidents_data.dropna(subset=[DISPOSITION_ID_COLUMN])

    # Drop class 0 from each dataset
    product_incidents_data = product_incidents_data[product_incidents_data[PRODUCT_ID_COLUMN] != 0]
    category_incidents_data = category_incidents_data[category_incidents_data[CATEGORY_ID_COLUMN] != 0]
    disposition_incidents_data = disposition_incidents_data[disposition_incidents_data[DISPOSITION_ID_COLUMN] != 0]

    return {
        PRODUCT_ID_COLUMN: product_incidents_data,
        CATEGORY_ID_COLUMN: category_incidents_data,
        DISPOSITION_ID_COLUMN: disposition_incidents_data
    }


def remove_html_tags_single(val: str) -> Union[str, float]:
    """
    Remove HTML from list of texts.

    @type val: str
    @param val: text
    @rtype: str
    @return: text without HTML tags

    """
    result = bs(val, HTML_PARSER).get_text().strip()
    if not result:
        return np.nan
    return result


def get_recent_intents_files(ocs_client: OcsBucket,
                             path: str) -> List[FileInfo]:
    """
    Get recent intents files from oci bucket, if having any.

    @param ocs_client: connected instance of OCI bucket.
    @param path: location where CSV file would be stored.
    """
    logging.basicConfig(level="INFO", format=LOGGING_FORMAT)
    logger.info("Getting previously stored CSV files, If exists.")
    ocs_files = [FileInfo(*ocs_file) for ocs_file in
                 ocs_client.get_objects_with_prefix(path)]
    ocs_files = [_extract_relative_path(path, ocs_file) for ocs_file in ocs_files]
    filtered_ocs_files = [ocs_file for ocs_file in
                          ocs_files if re.match(CSV_PATTERN, ocs_file.name)]
    logger.info(f"{len(filtered_ocs_files)} csv file(s) exists")
    sorted_files = sorted(filtered_ocs_files, key=attrgetter('time_created'), reverse=True)
    return sorted_files
