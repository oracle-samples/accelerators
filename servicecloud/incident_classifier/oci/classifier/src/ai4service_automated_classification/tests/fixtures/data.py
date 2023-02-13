################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Tue Jan 31 13:02:48 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: 6e47467e328dd83a9d658b7f85a7fd3fc1bcec78 $
################################################################################################
#  File: data.py
################################################################################################
import pandas as pd
import pytest
# from dsdk_utils.io.data.ocs_bucket import TemporaryOcsBucket
# from dsdk_utils.io.ocs_utilities import OcsPath

from ai4service_automated_classification.constants import APP_DIR, HIERARCHY_COLUMNS, INCIDENTS_COLUMNS, \
    INQUIRY_COLUMN, PRODUCT_ID_COLUMN, CATEGORY_ID_COLUMN, DISPOSITION_ID_COLUMN, \
    PRODUCT_COLUMN, CATEGORY_COLUMN, DISPOSITION_COLUMN, SUBJECT_COLUMN, TEXT_COLUMN
from ai4service_automated_classification.ml.util.data_util import preprocess_hierarchy, preprocess_incidents
from ai4service_automated_classification.utils.object_storage.os import TemporaryOcsBucket
from ai4service_automated_classification.utils.object_storage.utils import OcsPath


@pytest.fixture(scope="session")
def hierarchy_data():
    product_hierarchy_data = [
        [1, "Internal Systems", 1, None, None, None, None, None],
        [2, "Techmail", 1, 2, None, None, None, None]
    ]
    category_hierarchy_data = [
        [1, "Category 1", 1, None, None, None, None, None],
        [2, "Categ 2", 1, 2, None, None, None, None]
    ]
    disposition_hierarchy_data = [
        [1, "Disposition 1", 1, None, None, None, None, None],
        [2, "Dispo 2", 1, 2, None, None, None, None]
    ]

    hierarchies = {
        PRODUCT_ID_COLUMN: pd.DataFrame(product_hierarchy_data, columns=HIERARCHY_COLUMNS),
        CATEGORY_ID_COLUMN: pd.DataFrame(category_hierarchy_data, columns=HIERARCHY_COLUMNS),
        DISPOSITION_ID_COLUMN: pd.DataFrame(disposition_hierarchy_data, columns=HIERARCHY_COLUMNS)
    }
    return hierarchies


@pytest.fixture(scope="session")
def hierarchy_data_empty():
    product_hierarchy_data = [
    ]

    category_hierarchy_data = [
    ]

    disposition_hierarchy_data = [
    ]

    hierarchies = {
        PRODUCT_ID_COLUMN: pd.DataFrame(product_hierarchy_data, columns=HIERARCHY_COLUMNS),
        CATEGORY_ID_COLUMN: pd.DataFrame(category_hierarchy_data, columns=HIERARCHY_COLUMNS),
        DISPOSITION_ID_COLUMN: pd.DataFrame(disposition_hierarchy_data, columns=HIERARCHY_COLUMNS)
    }
    return hierarchies


@pytest.fixture(scope="session")
def incidents_data():
    data_most_frequent_label_0 = [
        ("Text1 Text1  Text1  Text1  Text1  Text1  Text1  Text1 ", "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Text1  Text2 text3 Text1  Text2 text3Text1  Text2 text3", "Internal Systems", 1, 1, 1, 1, 1, 1),
        ("Text1 Text1  Text2 text3Text1  Text2 text3 Text1  Text2 text3", "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Text1  Text2 text3Text1  Text2 text3Text1  Text2 text3", "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Text1 Text1 Text1  Text2 text3", "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Text1 Text1 Text1  Text2 text3", "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Text1 Text1  Text2 text3", "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Text2 Text1  Text2 text3 Text1  Text2 text3 Text1  Text2 text3 Text1  Text3",
         "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Text4", "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Text4", "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Text3 Text4 Text1  Text5 text6 Textl3 Text4 Text1  Text5 text6 ", "Techmail", 2, 2, 2, 2, 2, 2),
        ("TextText3 Text4 Text1  Text5 text6  3 ", "Techmail", 2, 0, 2, 0, 2, 0),
        ("Text3Text3 Text4 Text1  Text5 text6 ", "Techmail", 2, 1, 2, 1, 2, 1),
        ("Text3 Text3 Text4 Text1  Text5 text6 ", "Techmail", 2, 0, 2, 0, 2, 0),
        ("Text3 specific text in class one ", "Techmail", 2, 0, 2, 0, 2, 0),
        ("Text4 hello world", "Techmail", 2, 0, 2, 0, 2, 0),
        ("Text5 hi lukas, get well soon", "Techmail", 2, 0, 2, 0, 2, 0),
        ("Text6 hi fabrice and mandy", "Techmail", 2, 0, 2, 0, 2, 0),
        ("Text6 hello daniel", "Techmail", 2, 0, 2, 0, 2, 0),
        ("Text6 sleep well", "Techmail", 2, 0, 2, 0, 2, 0),
        ("Text6", "Techmail", 2, 0, 2, 0, 2, 0),
        ("Text6", "Techmail", 2, 0, 2, 0, 2, 0),
        ("Text6", "Techmail", 2, 0, 2, 0, 2, 0),
        ("Text7", "Techmail", 2, 0, 2, 0, 2, 0),
        ("Text7", "Techmail", 2, 0, 2, 0, 2, 0),
        (None, "Techmail", 2, 0, 2, 0, 2, 0),
        (None, "Techmail", 2, None, 2, None, 2, None),
    ]
    df_0 = pd.DataFrame(data_most_frequent_label_0, columns=INCIDENTS_COLUMNS)
    return df_0


@pytest.fixture(scope="session")
def dummy_data(incidents_data, hierarchy_data):
    return incidents_data, hierarchy_data


@pytest.fixture(scope="session")
def incidents_no_disposition():
    data = [
        ("Text1 Text1  Text1  Text1  Text1  Text1  Text1  Text1 ", "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Text1  Text2 text3 Text1  Text2 text3Text1  Text2 text3", "Internal Systems", 1, 1, 1, 1, 1, 1),
        ("Text1 Text1  Text2 text3Text1  Text2 text3 Text1  Text2 text3", "Internal Systems", 1, 0, 1, 0, 2, 0),
        ("Text1  Text2 text3Text1  Text2 text3Text1  Text2 text3", "Internal Systems", 1, 0, 1, 0, 3, 0),
        ("Text1 Text1 Text1  Text2 text3", "Internal Systems", 1, 0, 1, 0, 4, 0),
        ("Text1 Text1 Text1  Text2 text3", "Internal Systems", 1, 0, 1, 0, 5, 0),
        ("Text1 Text1  Text2 text3", "Internal Systems", 1, 0, 1, 0, 6, 0),
        ("Text2 Text1  Text2 text3 Text1  Text2 text3 Text1  Text2 text3 Text1  Text3",
         "Internal Systems", 1, 0, 1, 0, 7, 0),
        ("Text4", "Internal Systems", 1, 0, 1, 0, 8, 0),
        ("Text4", "Internal Systems", 1, 0, 1, 0, None, 0),
        ("Text3 Text4 Text1  Text5 text6 Textl3 Text4 Text1  Text5 text6 ", "Techmail", 2, 2, 2, 2, None, 2),
        ("TextText3 Text4 Text1  Text5 text6  3 ", "Techmail", 2, 0, 2, 0, 9, 0),
        ("Text3Text3 Text4 Text1  Text5 text6 ", "Techmail", 2, 1, 2, 1, 2, 1),
        ("Text3 Text3 Text4 Text1  Text5 text6 ", "Techmail", 2, 0, 2, 0, 2, 0),
        ("Text3 specific text in class one ", "Techmail", 2, 0, 2, 0, 3, 0),
        ("Text4 hello world", "Techmail", 2, 0, 2, 0, 4, 0),
        ("Text5 hi lukas, get well soon", "Techmail", 2, 0, 2, 0, 5, 0),
        ("Text6 hi fabrice and mandy", "Techmail", 2, 0, 2, 0, 6, 0),
        ("Text6 hello daniel", "Techmail", 2, 0, 2, 0, 7, 0),
        ("Text6 sleep well", "Techmail", 2, 0, 2, 0, 8, 0),
        ("Text6", "Techmail", 2, 0, 2, 0, 9, 0),
        ("Text6", "Techmail", 2, 0, 2, 0, 10, 0),
        ("Text6", "Techmail", 2, 0, 2, 0, 11, 0),
        ("Text7", "Techmail", 2, 0, 2, 0, 12, 0),
        ("Text7", "Techmail", 2, 0, 2, 0, 13, 0),
        (None, "Techmail", 2, 0, 2, 0, 13, 0),
        (None, "Techmail", 2, None, 2, None, 0, None),
        # Additional samples for prune min count
        ("Additional samples0 for class one.", "Internal Systems", 1, 0, 1, 0, 9, 0),
        ("Additional samples1 for class one.", "Internal Systems", 1, 0, 1, 0, 10, 0),
        ("Additional samples2 for class one.", "Internal Systems", 1, 0, 1, 0, 11, 0),
        ("Additional samples3 for class one.", "Internal Systems", 1, 0, 1, 0, 12, 0),
        ("Additional samples4 for class one.", "Internal Systems", 1, 0, 1, 0, 13, 0),
        ("Additional samples5 for class one.", "Internal Systems", 1, 0, 1, 0, 14, 0),
        ("Additional samples6 for class one.", "Internal Systems", 1, 0, 1, 0, 15, 0),
        ("Additional samples7 for class one.", "Internal Systems", 1, 0, 1, 0, 16, 0),
        ("Additional samples8 for class one.", "Internal Systems", 1, 0, 1, 0, 17, 0),
        ("Additional samples9 for class one.", "Internal Systems", 1, 0, 1, 0, 18, 0),
        ("Additional sample10 for class one.", "Internal Systems", 1, 0, 1, 0, 19, 0),
        ("Additional sample11 for class one.", "Internal Systems", 1, 0, 1, 0, 20, 0),
        ("Additional sample12 for class one.", "Internal Systems", 1, 0, 1, 0, 21, 0),
        ("Additional sample13 for class one.", "Internal Systems", 1, 0, 1, 0, 22, 0),
        ("Additional sample14 for class one.", "Internal Systems", 1, 0, 1, 0, 23, 0),
        ("Sample0 for class two", "Techmail", 2, 0, 2, 0, 14, 0),
        ("Sample1 for class two", "Techmail", 2, 0, 2, 0, 15, 0),
        ("Sample2 for class two", "Techmail", 2, 0, 2, 0, 16, 0),
        ("Sample3 for class two", "Techmail", 2, 0, 2, 0, 17, 0),
        ("Sample4 for class two", "Techmail", 2, 0, 2, 0, 18, 0),
        ("Sample5 for class two", "Techmail", 2, 0, 2, 0, 19, 0),
        ("Sample6 for class two", "Techmail", 2, 0, 2, 0, 20, 0),
        ("Sample7 for class two", "Techmail", 2, 0, 2, 0, 21, 0),
        ("Sample8 for class two", "Techmail", 2, 0, 2, 0, 22, 0),
        ("Sample9 for class two", "Techmail", 2, 0, 2, 0, 23, 0),
        ("Sampl10 for class two", "Techmail", 2, 0, 2, 0, 24, 0),
    ]
    df = pd.DataFrame(data, columns=INCIDENTS_COLUMNS)
    return df


@pytest.fixture(scope="session")
def incidents_no_category_disposition():
    data = [
        ("Text1 Text1  Text1  Text1  Text1  Text1  Text1  Text1 ", "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Text1  Text2 text3 Text1  Text2 text3Text1  Text2 text3", "Internal Systems", 1, 1, 1, 1, 1, 1),
        ("Text1 Text1  Text2 text3Text1  Text2 text3 Text1  Text2 text3", "Internal Systems", 1, 0, 2, 0, 2, 0),
        ("Text1  Text2 text3Text1  Text2 text3Text1  Text2 text3", "Internal Systems", 1, 0, 3, 0, 3, 0),
        ("Text1 Text1 Text1  Text2 text3", "Internal Systems", 1, 0, 4, 0, 4, 0),
        ("Text1 Text1 Text1  Text2 text3", "Internal Systems", 1, 0, 5, 0, 5, 0),
        ("Text1 Text1  Text2 text3", "Internal Systems", 1, 0, 6, 0, 6, 0),
        ("Text2 Text1  Text2 text3 Text1  Text2 text3 Text1  Text2 text3 Text1  Text3",
         "Internal Systems", 1, 0, 7, 0, 7, 0),
        ("Text4", "Internal Systems", 1, 0, 8, 0, 8, 0),
        ("Text4", "Internal Systems", 1, 0, 1, 0, None, 0),
        ("Text3 Text4 Text1  Text5 text6 Textl3 Text4 Text1  Text5 text6id ", "Techmail", 2, 2, 88, 2, None, 2),
        ("TextText3 Text4 Text1  Text5 text6  3 ", "Techmail", 2, 0, 9, 0, 9, 0),
        ("Text3Text3 Text4 Text1  Text5 text6 ", "Techmail", 2, 1, 2, 1, 2, 1),
        ("Text3 Text3 Text4 Text1  Text5 text6 ", "Techmail", 2, 0, 2, 0, 2, 0),
        ("Text3 specific text in class one ", "Techmail", 2, 0, 3, 0, 3, 0),
        ("Text4 hello world", "Techmail", 2, 0, 4, 0, 4, 0),
        ("Text5 hi lukas, get well soon", "Techmail", 2, 0, 5, 0, 5, 0),
        ("Text6 hi fabrice and mandy", "Techmail", 2, 0, 6, 0, 6, 0),
        ("Text6 hello daniel", "Techmail", 2, 0, 7, 0, 7, 0),
        ("Text6 sleep well", "Techmail", 2, 0, 8, 0, 8, 0),
        ("Text6", "Techmail", 2, 0, 9, 0, 9, 0),
        ("Text6", "Techmail", 2, 0, 10, 0, 10, 0),
        ("Text6", "Techmail", 2, 0, 11, 0, 11, 0),
        ("Text7", "Techmail", 2, 0, 12, 0, 12, 0),
        ("Text7", "Techmail", 2, 0, 13, 0, 13, 0),
        (None, "Techmail", 2, 0, 13, 0, 13, 0),
        (None, "Techmail", 2, None, 2, None, 0, None),
        # Additional samples for prune min count
        ("Additional samples0 for class one.", "Internal Systems", 1, 0, 9, 0, 9, 0),
        ("Additional samples1 for class one.", "Internal Systems", 1, 0, 10, 0, 10, 0),
        ("Additional samples2 for class one.", "Internal Systems", 1, 0, 11, 0, 11, 0),
        ("Additional samples3 for class one.", "Internal Systems", 1, 0, 12, 0, 12, 0),
        ("Additional samples4 for class one.", "Internal Systems", 1, 0, 13, 0, 13, 0),
        ("Additional samples5 for class one.", "Internal Systems", 1, 0, 14, 0, 14, 0),
        ("Additional samples6 for class one.", "Internal Systems", 1, 0, 15, 0, 15, 0),
        ("Additional samples7 for class one.", "Internal Systems", 1, 0, 16, 0, 16, 0),
        ("Additional samples8 for class one.", "Internal Systems", 1, 0, 17, 0, 17, 0),
        ("Additional samples9 for class one.", "Internal Systems", 1, 0, 18, 0, 18, 0),
        ("Additional sample10 for class one.", "Internal Systems", 1, 0, 19, 0, 19, 0),
        ("Additional sample11 for class one.", "Internal Systems", 1, 0, 20, 0, 20, 0),
        ("Additional sample12 for class one.", "Internal Systems", 1, 0, 21, 0, 21, 0),
        ("Additional sample13 for class one.", "Internal Systems", 1, 0, 22, 0, 22, 0),
        ("Additional sample14 for class one.", "Internal Systems", 1, 0, 23, 0, 23, 0),
        ("Sample0 for class two", "Techmail", 2, 0, 14, 0, 14, 0),
        ("Sample1 for class two", "Techmail", 2, 0, 15, 0, 15, 0),
        ("Sample2 for class two", "Techmail", 2, 0, 16, 0, 16, 0),
        ("Sample3 for class two", "Techmail", 2, 0, 17, 0, 17, 0),
        ("Sample4 for class two", "Techmail", 2, 0, 18, 0, 18, 0),
        ("Sample5 for class two", "Techmail", 2, 0, 19, 0, 19, 0),
        ("Sample6 for class two", "Techmail", 2, 0, 20, 0, 20, 0),
        ("Sample7 for class two", "Techmail", 2, 0, 21, 0, 21, 0),
        ("Sample8 for class two", "Techmail", 2, 0, 22, 0, 22, 0),
        ("Sample9 for class two", "Techmail", 2, 0, 23, 0, 23, 0),
        ("Sampl10 for class two", "Techmail", 2, 0, 24, 0, 24, 0),
    ]
    df = pd.DataFrame(data, columns=INCIDENTS_COLUMNS)
    return df


@pytest.fixture(scope="session")
def incidents_no_data():
    data = [
        ("Text1 Text1  Text1  Text1  Text1  Text1  Text1  Text1 ", "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Text1  Text2 text3 Text1  Text2 text3Text1  Text2 text3", "Internal Systems", 1, 1, 1, 1, 1, 1),
        ("Text1 Text1  Text2 text3Text1  Text2 text3 Text1  Text2 text3", "Internal Systems", 2, 0, 2, 0, 2, 0),
        ("Text1  Text2 text3Text1  Text2 text3Text1  Text2 text3", "Internal Systems", 3, 0, 3, 0, 3, 0),
        ("Text1 Text1 Text1  Text2 text3", "Internal Systems", 4, 0, 4, 0, 4, 0),
        ("Text1 Text1 Text1  Text2 text3", "Internal Systems", 5, 0, 5, 0, 5, 0),
        ("Text1 Text1  Text2 text3", "Internal Systems", 6, 0, 6, 0, 6, 0),
        ("Text2 Text1  Text2 text3 Text1  Text2 text3 Text1  Text2 text3 Text1  Text3",
         "Internal Systems", 7, 0, 7, 0, 7, 0),
        ("Text4", "Internal Systems", 8, 0, 8, 0, 8, 0),
        ("Text4", "Internal Systems", 9, 0, 1, 0, None, 0),
        ("Text3 Text4 Text1  Text5 text6 Textl3 Text4 Text1  Text5 text6 ", "Techmail", 77, 2, 88, 2, None, 2),
        ("TextText3 Text4 Text1  Text5 text6  3 ", "Techmail", 9, 0, 9, 0, 9, 0),
        ("Text3Text3 Text4 Text1  Text5 text6 ", "Techmail", 2, 1, 2, 1, 2, 1),
        ("Text3 Text3 Text4 Text1  Text5 text6 ", "Techmail", 2, 0, 2, 0, 2, 0),
        ("Text3 specific text in class one ", "Techmail", 3, 0, 3, 0, 3, 0),
        ("Text4 hello world", "Techmail", 4, 0, 4, 0, 4, 0),
        ("Text5 hi lukas, get well soon", "Techmail", 5, 0, 5, 0, 5, 0),
        ("Text6 hi fabrice and mandy", "Techmail", 6, 0, 6, 0, 6, 0),
        ("Text6 hello daniel", "Techmail", 7, 0, 7, 0, 7, 0),
        ("Text6 sleep well", "Techmail", 8, 0, 8, 0, 8, 0),
        ("Text6", "Techmail", 9, 0, 9, 0, 9, 0),
        ("Text6", "Techmail", 10, 0, 10, 0, 10, 0),
        ("Text6", "Techmail", 11, 0, 11, 0, 11, 0),
        ("Text7", "Techmail", 12, 0, 12, 0, 12, 0),
        ("Text7", "Techmail", 13, 0, 13, 0, 13, 0),
        (None, "Techmail", 13, 0, 13, 0, 13, 0),
        (None, "Techmail", 2, None, 2, None, 0, None),
    ]
    df = pd.DataFrame(data, columns=INCIDENTS_COLUMNS)
    return df


@pytest.fixture(scope="session")
def incidents_no_data_column():
    data = [
        ("Text1 Text1", "Text1  Text1  Text1  Text1  Text1  Text1", "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Text1 Text2 text3", "Text1  Text2 text3Text1  Text2 text3", "Internal Systems", 1, 1, 1, 1, 1, 1),
        ("Text1", "Text1  Text2 text3Text1  Text2 text3 Text1  Text2 text3", "Internal Systems", 2, 0, 2, 0, 2, 0),
        ("Text1  Text2 text3Text1  Text2 text3Text1", "Text2 text3", "Internal Systems", 3, 0, 3, 0, 3, 0),
        ("Text1 Text1 Text1", "Text2 text3", "Internal Systems", 4, 0, 4, 0, 4, 0),
        ("Text1 Text1", "", "Internal Systems", 5, 0, 5, 0, 5, 0),
        ("", "", "Internal Systems", 5, 0, 5, 0, 5, 0),
        (None, None, "Techmail", 13, 0, 13, 0, 13, 0)
    ]
    cols = [SUBJECT_COLUMN, TEXT_COLUMN]
    cols.extend(INCIDENTS_COLUMNS[1:])
    df = pd.DataFrame(data, columns=cols)
    return df


@pytest.fixture(scope="session")
def incidents_data_disposition_class_0():
    data_most_frequent_label_0 = [
        ("Text1 Text1  Text1  Text1  Text1  Text1  Text1  Text1 ", "Internal Systems", 1, 0, 1, 0, 0, 0),
        ("Text1  Text2 text3 Text1  Text2 text3Text1  Text2 text3", "Internal Systems", 1, 1, 1, 1, 0, 1),
        ("Text1 Text1  Text2 text3Text1  Text2 text3 Text1  Text2 text3", "Internal Systems", 1, 0, 1, 0, 0, 0),
        ("Text1  Text2 text3Text1  Text2 text3Text1  Text2 text3", "Internal Systems", 1, 0, 1, 0, 0, 0),
        ("Text1 Text1 Text1  Text2 text3", "Internal Systems", 1, 0, 1, 0, 0, 0),
        ("Text1 Text1 Text1  Text2 text3", "Internal Systems", 1, 0, 1, 0, 0, 0),
        ("Text1 Text1  Text2 text3", "Internal Systems", 1, 0, 1, 0, 0, 0),
        ("Text2 Text1  Text2 text3 Text1  Text2 text3 Text1  Text2 text3 Text1  Text3",
         "Internal Systems", 1, 0, 1, 0, 0, 0),
        ("Text4", "Internal Systems", 1, 0, 1, 0, 0, 0),
        ("Text4", "Internal Systems", 1, 0, 1, 0, 0, 0),
        ("Text3 Text4 Text1  Text5 text6 Textl3 Text4 Text1  Text5 text6 ", "Techmail", 2, 2, 2, 2, 0, 2),
        ("TextText3 Text4 Text1  Text5 text6  3 ", "Techmail", 2, 0, 2, 0, 0, 0),
        ("Text3Text3 Text4 Text1  Text5 text6 ", "Techmail", 2, 1, 2, 1, 0, 1),
        ("Text3 Text3 Text4 Text1  Text5 text6 ", "Techmail", 2, 0, 2, 0, 0, 0),
        ("Text3 specific text in class one ", "Techmail", 2, 0, 2, 0, 2, 0),
        ("Text4 hello world", "Techmail", 2, 0, 2, 0, 0, 0),
        ("Text5 hi lukas, get well soon", "Techmail", 2, 0, 2, 0, 0, 0),
        ("Text6 hi fabrice and mandy", "Techmail", 2, 0, 2, 0, 0, 0),
        ("Text6 hello daniel", "Techmail", 2, 0, 2, 0, 0, 0),
        ("Text6 sleep well", "Techmail", 2, 0, 2, 0, 0, 0),
        ("Text6", "Techmail", 2, 0, 2, 0, 0, 0),
        ("Text6", "Techmail", 2, 0, 2, 0, 0, 0),
        ("Text6", "Techmail", 2, 0, 2, 0, 0, 0),
        ("Text7", "Techmail", 2, 0, 2, 0, 0, 0),
        ("Text7", "Techmail", 2, 0, 2, 0, 0, 0),
        (None, "Techmail", 2, 0, 2, 0, 0, 0),
        (None, "Techmail", 2, None, 2, None, 0, None),
        # Additional samples for prune min count
        ("Additional samples0 for class one.", "Internal Systems", 1, 0, 1, 0, 0, 0),
        ("Additional samples1 for class one.", "Internal Systems", 1, 0, 1, 0, 0, 0),
        ("Additional samples2 for class one.", "Internal Systems", 1, 0, 1, 0, 0, 0),
        ("Additional samples3 for class one.", "Internal Systems", 1, 0, 1, 0, 0, 0),
        ("Additional samples4 for class one.", "Internal Systems", 1, 0, 1, 0, 0, 0),
        ("Additional samples5 for class one.", "Internal Systems", 1, 0, 1, 0, 0, 0),
        ("Additional samples6 for class one.", "Internal Systems", 1, 0, 1, 0, 0, 0),
        ("Additional samples7 for class one.", "Internal Systems", 1, 0, 1, 0, 0, 0),
        ("Additional samples8 for class one.", "Internal Systems", 1, 0, 1, 0, 0, 0),
        ("Additional samples9 for class one.", "Internal Systems", 1, 0, 1, 0, 0, 0),
        ("Additional sample10 for class one.", "Internal Systems", 1, 0, 1, 0, 0, 0),
        ("Additional sample11 for class one.", "Internal Systems", 1, 0, 1, 0, 0, 0),
        ("Additional sample12 for class one.", "Internal Systems", 1, 0, 1, 0, 0, 0),
        ("Additional sample13 for class one.", "Internal Systems", 1, 0, 1, 0, 0, 0),
        ("Additional sample14 for class one.", "Internal Systems", 1, 0, 1, 0, 0, 0),
        ("Sample0 for class two", "Techmail", 2, 0, 2, 0, 0, 0),
        ("Sample1 for class two", "Techmail", 2, 0, 2, 0, 0, 0),
        ("Sample2 for class two", "Techmail", 2, 0, 2, 0, 0, 0),
        ("Sample3 for class two", "Techmail", 2, 0, 2, 0, 0, 0),
        ("Sample4 for class two", "Techmail", 2, 0, 2, 0, 0, 0),
        ("Sample5 for class two", "Techmail", 2, 0, 2, 0, 0, 0),
        ("Sample6 for class two", "Techmail", 2, 0, 2, 0, 0, 0),
        ("Sample7 for class two", "Techmail", 2, 0, 2, 0, 0, 0),
        ("Sample8 for class two", "Techmail", 2, 0, 2, 0, 0, 0),
        ("Sample9 for class two", "Techmail", 2, 0, 2, 0, 0, 0),
        ("Sampl10 for class two", "Techmail", 2, 0, 2, 0, 0, 0),

    ]
    df_0 = pd.DataFrame(data_most_frequent_label_0, columns=INCIDENTS_COLUMNS)
    return df_0


@pytest.fixture(scope="session")
def disposition_class_0(incidents_data_disposition_class_0, hierarchy_data):
    incidents = preprocess_incidents(incidents_data_disposition_class_0)
    return incidents, hierarchy_data


@pytest.fixture(scope="session")
def no_disposition_data(incidents_no_disposition, hierarchy_data):
    incidents = preprocess_incidents(incidents_no_disposition)
    return incidents, hierarchy_data


@pytest.fixture()
def no_category_disposition_data(incidents_no_category_disposition, hierarchy_data):
    incidents = preprocess_incidents(incidents_no_category_disposition)
    return incidents, hierarchy_data


@pytest.fixture()
def no_data(incidents_no_data, hierarchy_data):
    incidents = preprocess_incidents(incidents_no_data)
    return incidents, hierarchy_data


@pytest.fixture()
def processed_incidents_empty_hierarchy(incidents_no_disposition, hierarchy_data_empty):
    incidents = preprocess_incidents(incidents_no_disposition)
    return incidents, hierarchy_data_empty


@pytest.fixture(scope="session")
def dummy_data_contains_html():
    data = [
        ("<b> Text1 Text1  Text1  Text1  Text1  Text1  Text1  Text1 <b>", "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("<span> <b> Text1  Text2 text3 Text1  Text2 text3Text1  Text2 text3 </b> </span>",
         "Internal Systems", 1, 1, 1, 0, 1, 0),
        ("<div> <span> <b> Text3 text6 Text3 Text4 Text1  Text5 text6 </b> </span> </div>",
         "Techmail", 2, 2, 2, 2, 2, 2),
        ("<p> Text6 sleep well </p>", "Techmail", 2, 0, 2, 0, 2, 0),
        ("<p></p>", "Techmail", 2, 0, 2, 0, 2, 0),
    ]
    df_0 = pd.DataFrame(data, columns=INCIDENTS_COLUMNS)

    hierarchy_data = [
        [1, "Internal Systems", 1, None, None, None, None, None],
        [2, "Techmail", 1, 2, None, None, None, None]
    ]
    df_1 = pd.DataFrame(hierarchy_data, columns=HIERARCHY_COLUMNS)
    return df_0, df_1


@pytest.fixture(scope="session")
def processed_dummy_data(dummy_data):
    incident_data, hierarchy_data = dummy_data
    incident_data = preprocess_incidents(incident_data)
    for key in hierarchy_data.keys():
        hierarchy_data[key] = preprocess_hierarchy(hierarchy_data[key])
    return incident_data, hierarchy_data


@pytest.fixture(scope="session")
def temp_uri_with_dummy_data(dummy_data) -> OcsPath:
    data_folder_incidents = 'data/0/autoclassif/'
    product_hierarchy_folder = 'data/0/products/'
    category_hierarchy_folder = 'data/0/category/'
    disposition_hierarchy_folder = 'data/0/disposition/'
    incidents, hierarchy = dummy_data
    # Use this compartment ID with the B2C Integration user OCI Config to be able to test locally
    # Use the Temporary Bucket with the compartment id to be able to test locally
    # compartment_id = 'ocid1.compartment.oc1..aaaaaaaa7e5piq5o7hyctljq6yybqeh6hxmacyzxcwwm2c4hkwsvxodtzema'
    # with TemporaryOcsBucket(bucket_name_prefix=APP_DIR, compartment_id=compartment_id) as temporary_bucket:
    with TemporaryOcsBucket(bucket_name_prefix=APP_DIR) as temporary_bucket:
        temporary_bucket.save_dataframe_as_csv(incidents, f'{data_folder_incidents}incidents-20210816165400-1.csv')
        temporary_bucket.save_dataframe_as_csv(hierarchy[PRODUCT_ID_COLUMN],
                                               f'{product_hierarchy_folder}hierarchy-20210816165400-1.csv')
        temporary_bucket.save_dataframe_as_csv(hierarchy[CATEGORY_ID_COLUMN],
                                               f'{category_hierarchy_folder}hierarchy-20210816165400-1.csv')
        temporary_bucket.save_dataframe_as_csv(hierarchy[DISPOSITION_ID_COLUMN],
                                               f'{disposition_hierarchy_folder}hierarchy-20210816165400-1.csv')
        bucket_name = temporary_bucket.bucket_name
        uri = f"oci://{bucket_name}/{data_folder_incidents}"
        yield uri


@pytest.fixture(scope="session")
def data_request_payload():
    yield {
        "jsonData": {
            INQUIRY_COLUMN: "Sample Inbound email subject",
            PRODUCT_COLUMN: 0,
            CATEGORY_COLUMN: 0,
            DISPOSITION_COLUMN: 0
        }
    }
